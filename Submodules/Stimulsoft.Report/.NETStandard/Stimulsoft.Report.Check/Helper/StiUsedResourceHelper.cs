#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
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

using Stimulsoft.Base;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Check.Helper
{
    public static class StiUsedResourceHelper
    {
        #region Methods
        public static List<StiDatabase> GetDatabasesUsedResource(StiReport report, StiResource resource)
        {
            var resName = $"resource://{resource.Name}";

            var fileDatabases = new List<StiDatabase>();

            foreach (var database in report.Dictionary.Databases)
            {
                StiFileDatabase fileDatabase = null;

                switch (resource.Type)
                {
                    case StiResourceType.Csv:
                        fileDatabase = database as StiCsvDatabase;
                        break;

                    case StiResourceType.Dbf:
                        fileDatabase = database as StiDBaseDatabase;
                        break;

                    case StiResourceType.Excel:
                        fileDatabase = database as StiExcelDatabase;
                        break;

                    case StiResourceType.Json:
                        fileDatabase = database as StiJsonDatabase;
                        break;

                    case StiResourceType.Gis:
                        fileDatabase = database as StiGisDatabase;
                        break;

                    case StiResourceType.Xml:
                    case StiResourceType.Xsd:
                        fileDatabase = database as StiXmlDatabase;
                        break;
                }

                if (database is StiXmlDatabase && resource.Type == StiResourceType.Xsd)
                {
                    if (((StiXmlDatabase)database).PathSchema == resName)
                        fileDatabases.Add(fileDatabase);
                }
                else if (fileDatabase != null)
                {
                    if (fileDatabase.PathData == resName)
                        fileDatabases.Add(fileDatabase);
                }
            }

            return fileDatabases;
        }

        public static List<StiComponent> GetComponentsUsedResource(StiReport report, StiResource resource)
        {
            var usedComponents = new List<StiComponent>();

            switch (resource.Type)
            {      
                case StiResourceType.Image:
                    var usedImages = StiUsedResourceHelper.GetImageComponentsUsedResource(report, resource);
                    usedComponents.AddRange(usedImages);
                    break;

                case StiResourceType.Rtf:
                case StiResourceType.Txt:
                    var usedRichText = StiUsedResourceHelper.GetRichTextComponentsUsedResource(report, resource);
                    usedComponents.AddRange(usedRichText);
                    break;

                case StiResourceType.Report:
                case StiResourceType.ReportSnapshot:
                    var usedReports = StiUsedResourceHelper.GetReportsUsedResource(report, resource);
                    usedComponents.AddRange(usedReports);
                    break;

                case StiResourceType.FontEot:
                case StiResourceType.FontOtf:
                case StiResourceType.FontTtc:
                case StiResourceType.FontTtf:
                case StiResourceType.FontWoff:
                    var usedFonts = StiUsedResourceHelper.GetTextsUsedResource(report, resource);
                    usedComponents.AddRange(usedFonts);
                    break;
            }

            return usedComponents;
        }

        private static List<StiComponent> GetImageComponentsUsedResource(StiReport report, StiResource resource)
        {
            var resName = $"resource://{resource.Name}";

            var images = report.GetComponents().ToList().Where(d => d.GetType() == typeof(StiImage) || d is IStiImageElement);

            var usedImages = images.Where(d =>
            {
                if (d is StiImage image)
                    return image.ImageURL.Value.Equals(resName);

                if (d is IStiImageElement imageElement && !string.IsNullOrEmpty(imageElement.ImageHyperlink))
                    return imageElement.ImageHyperlink.Equals(resName);

                return false;

            }).ToList();

            return usedImages;
        }

        private static List<StiComponent> GetRichTextComponentsUsedResource(StiReport report, StiResource resource)
        {
            var resName = $"resource://{resource.Name}";

            var images = report.GetComponents().ToList().Where(d => d.GetType() == typeof(StiRichText));
            var usedImages = images.Where(d => ((StiRichText)d).DataUrl.Value.Equals(resName)).ToList();

            return usedImages;
        }

        private static List<StiComponent> GetReportsUsedResource(StiReport report, StiResource resource)
        {
            var resName = $"resource://{resource.Name}";

            var reports = report.GetComponents().ToList().Where(d => d.GetType() == typeof(StiSubReport));
            var usedImages = reports.Where(d => ((StiSubReport)d).SubReportUrl != null && ((StiSubReport)d).SubReportUrl.Equals(resName)).ToList();

            return usedImages;
        }

        private static List<StiComponent> GetTextsUsedResource(StiReport report, StiResource resource)
        {
            var fontName = report.GetResourceFontName(resource.Name);

            var textComp = report.GetComponents().ToList().Where(d => d.GetType() == typeof(StiText));
            var usedTextComp = textComp.Where(d => ((StiText)d).Font.Name.Equals(fontName)).ToList();

            return usedTextComp;
        }
        #endregion
    }
}
