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
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Images;
using System.Drawing;

namespace Stimulsoft.Report.Export
{
    public static class StiImageExportHelper
    {
        public static string GetPathImageOffice2013FromBitmapService(StiService service, double xFactor = 1)
        {
            var xFactorName = "";

            if (xFactor == 1.5)
                xFactorName = "_x1_5";

            if (xFactor == 2)
                xFactorName = "_x2";

            if (xFactor == 3)
                xFactorName = "_x3";

            if (xFactor == 4)
                xFactorName = "_x4";

            if (service is StiPdfExportService)
                return $"Stimulsoft.Report.Images.Dictionary.ResourcePdf{xFactorName}.png";

            if (service is StiXpsExportService)
                return $"Stimulsoft.Report.Images.Dictionary.ResourceXps{xFactorName}.png";

            if (service is StiPpt2007ExportService)
                return $"Stimulsoft.Report.Images.Dictionary.ResourcePowerPoint{xFactorName}.png";

            if (service is StiHtmlExportService ||
                service is StiHtml5ExportService ||
                service is StiMhtExportService)
                return $"Stimulsoft.Report.Images.Dictionary.ResourceHtml{xFactorName}.png";

            if (service is StiTxtExportService)
                return $"Stimulsoft.Report.Images.Dictionary.ResourceTxt{xFactorName}.png";

            if (service is StiRtfExportService)
                return $"Stimulsoft.Report.Images.Dictionary.ResourceRtf{xFactorName}.png";

            if (service is StiWord2007ExportService)
                return $"Stimulsoft.Report.Images.Dictionary.ResourceWord{xFactorName}.png";

            if (service is StiOdtExportService)
                return $"Stimulsoft.Report.Images.Dictionary.ResourceOdt{xFactorName}.png";

            if (service is StiExcelExportService ||
                service is StiExcel2007ExportService ||
                service is StiExcelXmlExportService)
                return $"Stimulsoft.Report.Images.Dictionary.ResourceExcel{xFactorName}.png";

            if (service is StiOdsExportService)
                return $"Stimulsoft.Report.Images.Dictionary.ResourceOdc{xFactorName}.png";

            if (service is StiImageExportService)
                return $"Stimulsoft.Report.Images.Dictionary.ResourceImage{xFactorName}.png";

            if (service is StiDataExportService ||
                service is StiCsvExportService ||
                service is StiDbfExportService ||
                service is StiDifExportService ||
                service is StiSylkExportService ||
                service is StiXmlExportService)
                return $"Stimulsoft.Report.Images.Dictionary.ResourceData{xFactorName}.png";

            return null;
        }

        public static Image GetImageFromService(StiService service, StiImageSize imageSize = StiImageSize.Normal)
        {
            if (service is StiPdfExportService)
                return StiReportImages.Dictionary.ResourcePdf(imageSize);

            if (service is StiXpsExportService)
                return StiReportImages.Dictionary.ResourceXps(imageSize);

            if (service is StiPpt2007ExportService)
                return StiReportImages.Dictionary.ResourcePowerPoint(imageSize);

            if (service is StiHtmlExportService || 
                service is StiHtml5ExportService || 
                service is StiMhtExportService)
                return StiReportImages.Dictionary.ResourceHtml(imageSize);

            if (service is StiTxtExportService)
                return StiReportImages.Dictionary.ResourceTxt(imageSize);

            if (service is StiRtfExportService)
                return StiReportImages.Dictionary.ResourceRtf(imageSize);

            if (service is StiWord2007ExportService)
                return StiReportImages.Dictionary.ResourceWord(imageSize);

            if (service is StiOdtExportService)
                return StiReportImages.Dictionary.ResourceOdt(imageSize);

            if (service is StiExcelExportService || 
                service is StiExcel2007ExportService || 
                service is StiExcelXmlExportService)
                return StiReportImages.Dictionary.ResourceExcel(imageSize);

            if (service is StiOdsExportService)
                return StiReportImages.Dictionary.ResourceOdc(imageSize);

            if (service is StiImageExportService)
                return StiReportImages.Dictionary.ResourceImage(imageSize);

            if (service is StiDataExportService ||
                service is StiCsvExportService ||
                service is StiDbfExportService ||
                service is StiDifExportService ||
                service is StiSylkExportService ||
                service is StiXmlExportService)
                return StiReportImages.Dictionary.ResourceData(imageSize);

            return null;
        }
    }
}
