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
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.SaveLoad
{
    /// <summary>
    /// Describes the class that allows to save / load packed documents.
    /// </summary>
    public class StiPackedDocumentSLService : StiDocumentSLService
    {
        #region StiService override
        /// <summary>
        /// Gets a service type.
        /// </summary>
        public override Type ServiceType => typeof(StiDocumentSLService);
        #endregion

        #region Methods
        /// <summary>
        /// Saves the current document into the stream.
        /// </summary>
        /// <param name="report">Rendered report for saving.</param>
        /// <param name="stream">Stream to save documents.</param>
        public override void Save(StiReport report, Stream stream)
        {
            byte[] bytes = StiGZipHelper.Pack(report.SaveDocumentToByteArray());
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Loads a document from the stream.
        /// </summary>
        /// <param name="report">Report in which loading will be done.</param>
        /// <param name="stream">Stream to load document.</param>
        public override void Load(StiReport report, Stream stream)
        {
            if (report.ReportCacheMode != StiReportCacheMode.Off)
            {
                string tempFilePath = null;
                try
                {
                    tempFilePath = StiReportCache.CreateNewCache();
                    string fileName = Path.Combine(tempFilePath, string.Format("{0}.tmp", StiGuidUtils.NewGuid()));
                    FileStream fs = new FileStream(fileName, FileMode.Create);

                    StiGZipHelper.Unpack(stream, fs);

                    report.LoadDocument(fs);

                    fs.Close();
                }
                finally
                {
                    if (!string.IsNullOrEmpty(tempFilePath)) StiReportCache.DeleteCache(tempFilePath);
                }
            }
            else
            {
                byte[] bytes = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(bytes, 0, bytes.Length);

                bytes = StiGZipHelper.Unpack(bytes);
                report.LoadDocument(bytes);
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
            return StiLocalization.Get("FileFilters", "PackedDocumentFiles");
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
