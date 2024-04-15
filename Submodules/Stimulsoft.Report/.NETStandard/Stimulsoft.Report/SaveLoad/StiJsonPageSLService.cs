#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	Stimulsoft.Report Library										}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft     							}
{	ALL RIGHTS RESERVED												}
{																	}
{	The entire contents of this file is protected by U.S. and		}
{	International Copyright Laws. Unauthorized reproduction,		}
{	reverse-engineering, and distribution of all or any portion of	}
{	the code contained in this file is strictly prohibited and may	}
{	result in severe civil and criminal penalties and will be		}
{	prosecuted to the maximum extent possible under the law.		}
{																	}
{	RESTRICTIONS													}
{																	}
{	THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES			}
{	ARE CONFIDENTIAL AND PROPRIETARY								}
{	TRADE SECRETS OF Stimulsoft										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System;
using System.IO;
using Stimulsoft.Report.Units;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dialogs;

namespace Stimulsoft.Report.SaveLoad
{
    /// <summary>
	/// Describes the class that allows to save / load pages.
	/// </summary>
	public class StiJsonPageSLService : StiPageSLService
    {
        #region StiService override
        /// <summary>
        /// Gets a service type.
        /// </summary>
        public override Type ServiceType => typeof(StiPageSLService);
        #endregion

        #region Methods
        /// <summary>
        /// Saves a page in the stream.
        /// </summary>
        /// <param name="page">Page for saving.</param>
        /// <param name="stream">Stream to save page.</param>
        public override void Save(StiPage page, Stream stream)
        {
            StiSerializing sr = new StiSerializing(new StiReportObjectStringConverter());

            string name = page.Name;
            page.IgnoreNamingRule = true;
            page.Name += ":Unit:" + page.Unit.GetType();
            page.IgnoreNamingRule = false;

            StiReport report = page.Report;
            page.ReportUnit = page.Unit;
            page.Report = null;

            string typePage;

            if (page is IStiForm) typePage = "StiForm";
            else typePage = "StiPage";

            sr.Serialize(page, stream, typePage);
            page.Name = name;

            page.Report = report;
            page.ReportUnit = null;
        }

        /// <summary>
        /// Loads a page from the stream.
        /// </summary>
        /// <param name="page">The page in which loading will be done.</param>
        /// <param name="stream">Stream to load pages.</param>
        public override void Load(StiPage page, Stream stream)
        {
            StiReport report = page.Report;
            string name = page.Name;
            page.IgnoreNamingRule = true;
            
            report.IsSerializing = true;

            string jsonStr;
            using (var readed = new StreamReader(stream))
            {
                jsonStr = readed.ReadToEnd();
            }

            page.LoadFromJsonInternal(jsonStr);
            
            report.IsSerializing = false;

            if (page.Name.IndexOf(":Unit", StringComparison.InvariantCulture) != -1)
            {
                StiUnit unit = null;
                if (page.Name.IndexOf("Centimeters", StringComparison.InvariantCulture) != -1) unit = new StiCentimetersUnit();
                if (page.Name.IndexOf("HundredthsOfInch", StringComparison.InvariantCulture) != -1) unit = new StiHundredthsOfInchUnit();
                if (page.Name.IndexOf("Inches", StringComparison.InvariantCulture) != -1) unit = new StiInchesUnit();
                if (page.Name.IndexOf("Millimeters", StringComparison.InvariantCulture) != -1) unit = new StiMillimetersUnit();

                if (unit != report.Unit)
                {
                    page.Convert(unit, report.Unit);
                }
            }

            page.Name = name;
            page.IgnoreNamingRule = false;

            page.Report = report;

        }

        /// <summary>
        /// Returns actions available for the provider.
        /// </summary>
        /// <returns>Available actions.</returns>
        public override StiSLActions GetAction()
        {
            return StiSLActions.Load | StiSLActions.Save;
        }

        /// <summary>
        /// Returns a filter for the provider.
        /// </summary>
        /// <returns>String with filter.</returns>
        public override string GetFilter()
        {
            return StiLocalization.Get("FileFilters", "PageFiles");
        }
        #endregion
    }
}
