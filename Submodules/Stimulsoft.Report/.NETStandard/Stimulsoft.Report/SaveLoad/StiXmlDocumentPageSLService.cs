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
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.SaveLoad
{
	/// <summary>
	/// Describes the class that allows to save / load rendered pages.
	/// </summary>
    public class StiXmlDocumentPageSLService : StiDocumentPageSLService
	{
		#region StiService override
		/// <summary>
		/// Gets a service type.
		/// </summary>
		public override Type ServiceType => typeof(StiDocumentPageSLService);
        #endregion

	    #region Methods
        /// <summary>
        /// Saves a page in the stream.
        /// </summary>
        /// <param name="page">Page for saving.</param>
        /// <param name="stream">Stream to save page.</param>
        public override void Save(StiPage page, Stream stream)
		{
            StiReport oldReport = page.Report;
            var report = StiActivator.CreateObject(StiOptions.Engine.BaseReportType) as StiReport;
            try
            {                
                report.Unit = page.Unit;

                report.RenderedPages.Clear();
                report.RenderedPages.Add(page);
                page.Report = report;

                report.SaveDocument(stream);
            }
            finally
            {
                page.Report = oldReport;
                report.Dispose();
            }
		}
		
		/// <summary>
		/// Loads a page from the stream.
		/// </summary>
		/// <param name="page">The page in which loading will be done.</param>
		/// <param name="stream">Stream to load pages.</param>
        public override void Load(StiPage page, Stream stream)
        {
            StiReport oldReport = page.Report;
            var report = StiActivator.CreateObject(StiOptions.Engine.BaseReportType) as StiReport;
            report.LoadDocument(stream);

            if (report.RenderedPages.Count == 0) return;

            if (report.Unit != page.Unit)
            {
                report.Convert(report.Unit, page.Unit);
            }

            StiPage pageToLoad = report.RenderedPages[0];
            pageToLoad.Report = null;
            StiComponentsCollection comps = pageToLoad.Components;
            pageToLoad.Components = new StiComponentsCollection();
            
            MemoryStream tempStream = new MemoryStream();
            StiSerializing sr = new StiSerializing(new StiReportObjectStringConverter(true));
            sr.CheckSerializable = true;
            sr.Serialize(pageToLoad, tempStream, "temp");
            tempStream.Seek(0, SeekOrigin.Begin);
            sr.Deserialize(page, tempStream, "temp");

            tempStream.Close();
            tempStream.Dispose();

            page.Report = oldReport;
            page.Components.AddRange(comps);
            comps = page.GetComponents();
            foreach (StiComponent comp in comps)
            {
                comp.Page = page;
                comp.Report = oldReport;
            }
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
