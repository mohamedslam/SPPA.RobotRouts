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
using Stimulsoft.Report.Images;
using System.Drawing;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// This class helps in the operating with images for a resource object.
    /// </summary>
	public static class StiResourceImageHelper
    {
        public static Image GetImage(StiResourceType type, StiImageSize size)
        {
            switch (type)
            {
                case StiResourceType.Csv:
                    return StiReportImages.Dictionary.ResourceCsv(size);

                case StiResourceType.Dbf:
                    return StiReportImages.Dictionary.ResourceDbf(size);

                case StiResourceType.Excel:
                    return StiReportImages.Dictionary.ResourceExcel(size);

                case StiResourceType.FontEot:
                    return StiReportImages.Dictionary.ResourceFontEot(size);

                case StiResourceType.FontOtf:
                    return StiReportImages.Dictionary.ResourceFontOtf(size);

                case StiResourceType.FontTtc:
                    return StiReportImages.Dictionary.ResourceFontTtc(size);

                case StiResourceType.FontTtf:
                    return StiReportImages.Dictionary.ResourceFontTtf(size);

                case StiResourceType.FontWoff:
                    return StiReportImages.Dictionary.ResourceFontWoff(size);

                case StiResourceType.Gis:
                    return StiReportImages.Dictionary.ResourceGis(size);

                case StiResourceType.Image:
                    return StiReportImages.Dictionary.ResourceImage(size);

                case StiResourceType.Json:
                    return StiReportImages.Dictionary.ResourceJson(size);

                case StiResourceType.Map:
                    return StiReportImages.Dictionary.ResourceMap(size);

                case StiResourceType.Pdf:
                    return StiReportImages.Dictionary.ResourcePdf(size);

                case StiResourceType.Report:
                    return StiReportImages.Dictionary.ResourceReport(size);

                case StiResourceType.ReportSnapshot:
                    return StiReportImages.Dictionary.ResourceReportSnapshot(size);

                case StiResourceType.Rtf:
                    return StiReportImages.Dictionary.ResourceRtf(size);

                case StiResourceType.Txt:
                    return StiReportImages.Dictionary.ResourceTxt(size);

                case StiResourceType.Word:
                    return StiReportImages.Dictionary.ResourceWord(size);

                case StiResourceType.Xml:
                    return StiReportImages.Dictionary.ResourceXml(size);

                case StiResourceType.Xsd:
                    return StiReportImages.Dictionary.ResourceXsd(size);

                default:
                    return StiReportImages.Dictionary.Resource(size);
            }
        }
    }
}