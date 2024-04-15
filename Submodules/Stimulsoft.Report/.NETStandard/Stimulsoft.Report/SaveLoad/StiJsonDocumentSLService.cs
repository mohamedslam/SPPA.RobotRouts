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
using Stimulsoft.Base;
using System.IO;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.SaveLoad
{
    /// <summary>
    /// Describes the class that allows to save / load packed documents.
    /// </summary>
    public class StiJsonDocumentSLService : StiDocumentSLService
    {
        #region StiService override
        /// <summary>
        /// Gets a service type.
        /// </summary>
        public override Type ServiceType => typeof(StiJsonDocumentSLService);
        #endregion

        #region Methods
        /// <summary>
        /// Saves the current document into the stream.
        /// </summary>
        /// <param name="report">Rendered report for saving.</param>
        /// <param name="stream">Stream to save documents.</param>
        public override void Save(StiReport report, Stream stream)
        {
            try
            {
                var jsonStr = report.SaveToJsonInternal(StiJsonSaveMode.Document);
                var writer = new StreamWriter(stream);
                writer.Write(jsonStr);
                writer.Flush();
            }
            finally
            {
                report.IsSerializing = false;
            }
        }

        /// <summary>
        /// Loads a document from the stream.
        /// </summary>
        /// <param name="report">Report in which loading will be done.</param>
        /// <param name="stream">Stream to load document.</param>
        public override void Load(StiReport report, Stream stream)
        {
            try
            {
                report.LoadFromJsonInternal(stream);

                report.IsRendered = true;
                report.NeedsCompiling = false;
                report.IsDocument = true;
            }
            finally
            {
                foreach (StiPage page in report.RenderedPages)
                {
                    page.Report = report;
                    var comps = page.GetComponents();

                    foreach (StiComponent comp in comps)
                    {
                        comp.Page = page;
                    }
                }
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
            return StiLocalization.Get("FileFilters", "JsonDocumentFiles");
        }
        #endregion

        #region Properties
        /// <summary>
        /// If the provider handles with multitude of files then true. If does not then false.
        /// </summary>
        public override bool MultiplePages => false;
        #endregion
    }
}
