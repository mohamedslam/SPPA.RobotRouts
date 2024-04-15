#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using System.Collections;
using Stimulsoft.Base.Localization;
using System.Text;
using System.IO;
using System.Collections.Concurrent;
using Stimulsoft.Base;
using System;
using System.Web;
using System.Linq;
using Stimulsoft.Report.Export.Tools;
using System.Drawing.Printing;
using System.Threading;
using System.Globalization;

#if NETSTANDARD
using Stimulsoft.System.Web;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiCollectionsHelper
    {
        private static Hashtable Item(string value, string key)
        {
            var item = new Hashtable();
            item["key"] = key;
            item["value"] = value;

            return item;
        }

        public static ArrayList GetEncodingDataItems()
        {
            var items = new ArrayList();
            var encodings = new ArrayList
            {
                Encoding.Default.CodePage,
                Encoding.ASCII.CodePage,
                Encoding.BigEndianUnicode.CodePage,
                Encoding.Unicode.CodePage,
#if !NETSTANDARD && !NETCOREAPP
                Encoding.UTF7.CodePage,
#endif
                Encoding.UTF8.CodePage
            };
            for (var i = 1250; i < 1257; i++)
            {
                try
                {
                    encodings.Add(Encoding.GetEncoding(i).CodePage);
                }
                catch
                {
                }
            };

            encodings.Cast<int>().Distinct().ToList().ForEach(e => items.Add(
                Item(Encoding.GetEncoding(e).EncodingName, Encoding.GetEncoding(e).CodePage.ToString())
            ));

            return items;
        }

        public static ArrayList GetPdfSecurityCertificatesItems()
        {
            var items = new ArrayList();
            var certificates = StiPdfSecurity.GetCertificatesList(false);

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

            certificates.ForEach(c =>
            {
                items.Add(new Hashtable()
                {
                    ["name"] = c.Name,
                    ["issuer"] = c.Issuer,
                    ["from"] = c.From.ToString("MM'/'dd'/'yyyy"),
                    ["to"] = c.To.ToString("MM'/'dd'/'yyyy"),
                    ["thumbprint"] = c.Thumbprint
                });
            });

            Thread.CurrentThread.CurrentCulture = currentCulture;

            return items;
        }

        public static void LoadLocalizationFile(HttpContext httpContext, string directoryPath, string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    if (filePath.ToLower() == "default")
                    {
                        StiLocalization.LoadDefaultLocalization();
                    }
                    else
                    {
                        if (!filePath.ToLower().EndsWith(".xml")) filePath += ".xml";
                        if (string.IsNullOrEmpty(directoryPath)) directoryPath = "Localization";

                        var absolutePath = filePath;
                        if (!File.Exists(absolutePath)) absolutePath = httpContext.Server.MapPath(filePath);
                        if (!File.Exists(absolutePath)) absolutePath = httpContext.Server.MapPath(Path.Combine(directoryPath, filePath));
                        if (!File.Exists(absolutePath)) absolutePath = httpContext.Server.MapPath("/" + filePath);
                        if (!File.Exists(absolutePath)) absolutePath = httpContext.Server.MapPath("/" + Path.Combine(directoryPath, filePath));
#if NETSTANDARD
                        if (!File.Exists(absolutePath)) absolutePath = httpContext.Server.MapRootPath(filePath);
                        if (!File.Exists(absolutePath)) absolutePath = httpContext.Server.MapRootPath(Path.Combine(directoryPath, filePath));
#endif
                        if (File.Exists(absolutePath)) StiOptions.Localization.Load(absolutePath);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static ArrayList GetLocalizationsList(HttpContext httpContext, string directoryPath)
        {
            var list = new ArrayList();
            if (string.IsNullOrEmpty(directoryPath)) return list;

            string absolutePath = directoryPath;
            if (!Directory.Exists(absolutePath)) absolutePath = httpContext.Server.MapPath(directoryPath);
            if (!Directory.Exists(absolutePath)) absolutePath = httpContext.Server.MapPath("/" + directoryPath);
#if NETSTANDARD
            if (!Directory.Exists(absolutePath)) absolutePath = httpContext.Server.MapRootPath(directoryPath);
#endif

            #region Get the localization files list

            if (Directory.Exists(absolutePath))
            {
                string language;
                string description;
                string cultureName;

                var dir = new DirectoryInfo(absolutePath);
                var files = dir.GetFiles();
                foreach (var file in files)
                {
                    if (file.FullName.EndsWith(".xml") && !file.FullName.EndsWith(".ext.xml"))
                    {
                        try
                        {
                            StiLocalization.GetParam(file.FullName, out language, out description, out cultureName);
                            if (!string.IsNullOrEmpty(language))
                            {
                                var loc = new Hashtable();
                                loc["FileName"] = file.Name;
                                loc["Language"] = language;
                                loc["Description"] = description;
                                loc["CultureName"] = cultureName;
                                list.Add(loc);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }

            #endregion

            return list;
        }

        private static ConcurrentDictionary<string, Hashtable> LocalizationItems = new ConcurrentDictionary<string, Hashtable>();

        public static Hashtable GetLocalizationItems(StiRequestParams requestParams)
        {
            Hashtable words;
            string path = requestParams.Localization;
            string hash = StiMD5Helper.ComputeHash(path);
            LocalizationItems.TryGetValue(hash, out words);
            if (words != null)
                return words;

            LoadLocalizationFile(requestParams.HttpContext, string.Empty, path);

            words = new Hashtable();
            words["Viewer"] = StiLocalization.Get("FormViewer", "title");
            words["UpgradeNow"] = StiLocalization.Get("Buttons", "UpgradeNow");
            words["Purchase"] = StiLocalization.Get("Cloud", "ButtonPurchase");
            words["EditorToolTip"] = StiLocalization.Get("FormViewer", "Editor");
            words["TellMeMore"] = StiLocalization.Get("HelpDesigner", "TellMeMore");
            words["Print"] = StiLocalization.Get("A_WebViewer", "PrintReport");
            words["PrintToolTip"] = StiLocalization.Get("HelpViewer", "Print");
            words["Save"] = StiLocalization.Get("A_WebViewer", "SaveReport");
            words["SaveToolTip"] = StiLocalization.Get("HelpViewer", "Save");
            words["Open"] = StiLocalization.Get("Buttons", "Open");
            words["OpenToolTip"] = StiLocalization.Get("HelpViewer", "Open");
            words["SendEmail"] = StiLocalization.Get("FormViewer", "SendEMail").Replace("...", string.Empty);
            words["SendEmailToolTip"] = StiLocalization.Get("HelpViewer", "SendEMail");

            words["Resources"] = StiLocalization.Get("PropertyMain", "Resources");
            words["ResourcesToolTip"] = StiLocalization.Get("PropertyMain", "Resources");
            words["Signature"] = StiLocalization.Get("Components", "StiSignature");
            words["SignatureToolTip"] = StiLocalization.Get("Export", "DigitalSignature");

            words["BookmarksToolTip"] = StiLocalization.Get("HelpViewer", "Bookmarks");
            words["ParametersToolTip"] = StiLocalization.Get("HelpViewer", "Parameters");
            words["FindToolTip"] = StiLocalization.Get("HelpViewer", "Find");
            words["FirstPageToolTip"] = StiLocalization.Get("HelpViewer", "PageFirst");
            words["PrevPageToolTip"] = StiLocalization.Get("HelpViewer", "PagePrevious");
            words["NextPageToolTip"] = StiLocalization.Get("HelpViewer", "PageNext");
            words["LastPageToolTip"] = StiLocalization.Get("HelpViewer", "PageLast");
            words["FullScreenToolTip"] = StiLocalization.Get("HelpViewer", "FullScreen");
            words["ZoomToolTip"] = StiLocalization.Get("FormViewer", "Zoom");
            words["Loading"] = StiLocalization.Get("A_WebViewer", "Loading").Replace("...", "");
            words["Bookmarks"] = StiLocalization.Get("FormViewer", "Bookmarks");
            words["Parameters"] = StiLocalization.Get("FormViewer", "Parameters");
            words["Time"] = StiLocalization.Get("FormFormatEditor", "Time");
            words["Version"] = StiLocalization.Get("PropertyMain", "Version");
            words["Maximum"] = StiLocalization.Get("PropertyMain", "Maximum");
            words["Copy"] = StiLocalization.Get("Report", "CopyOf");

            words["FindWhat"] = StiLocalization.Get("FormViewerFind", "FindWhat");
            words["FindPrevious"] = StiLocalization.Get("FormViewerFind", "FindPrevious");
            words["FindNext"] = StiLocalization.Get("FormViewerFind", "FindNext");
            words["MatchCase"] = StiLocalization.Get("Editor", "MatchCase");
            words["MatchWholeWord"] = StiLocalization.Get("Editor", "MatchWholeWord");
            words["TypeToSearch"] = StiLocalization.Get("Editor", "TypeToSearch");

            words["EmailOptions"] = StiLocalization.Get("A_WebViewer", "EmailOptions");
            words["Email"] = StiLocalization.Get("A_WebViewer", "Email").Replace(":", "");
            words["Subject"] = StiLocalization.Get("A_WebViewer", "Subject").Replace(":", "");
            words["Message"] = StiLocalization.Get("A_WebViewer", "Message").Replace(":", "");
            words["Attachment"] = StiLocalization.Get("A_WebViewer", "Attachment").Replace(":", "");

            words["SinglePage"] = StiLocalization.Get("FormViewer", "PageViewModeSinglePage");
            words["Continuous"] = StiLocalization.Get("FormViewer", "PageViewModeContinuous");
            words["MultiplePages"] = StiLocalization.Get("FormViewer", "PageViewModeMultiplePages");

            words["ViewModeToolTip"] = StiLocalization.Get("FormViewer", "ViewMode");
            words["Design"] = StiLocalization.Get("Buttons", "Design");
            words["Page"] = StiLocalization.Get("A_WebViewer", "Page");
            words["PageOf"] = StiLocalization.Get("A_WebViewer", "PageOf");

            words["PrintPdf"] = StiLocalization.Get("A_WebViewer", "PrintToPdf");
            words["PrintWithPreview"] = StiLocalization.Get("A_WebViewer", "PrintWithPreview");
            words["PrintWithoutPreview"] = StiLocalization.Get("A_WebViewer", "PrintWithoutPreview");

            words["ZoomOnePage"] = StiLocalization.Get("Zoom", "PageHeight");
            words["ZoomPageWidth"] = StiLocalization.Get("FormViewer", "ZoomPageWidth");

            words["RemoveAll"] = StiLocalization.Get("Buttons", "RemoveAll");
            words["NewItem"] = StiLocalization.Get("FormDictionaryDesigner", "NewItem");
            words["Close"] = StiLocalization.Get("Buttons", "Close");

            words["Reset"] = StiLocalization.Get("Gui", "cust_pm_reset");
            words["Submit"] = StiLocalization.Get("Buttons", "Submit");

            words["RangeFrom"] = StiLocalization.Get("PropertyMain", "RangeFrom");
            words["RangeTo"] = StiLocalization.Get("PropertyMain", "RangeTo");

            words["ExportFormTitle"] = StiLocalization.Get("Export", "title");
            words["ButtonOk"] = StiLocalization.Get("Gui", "barname_ok");
            words["ButtonCancel"] = StiLocalization.Get("Gui", "barname_cancel");

            words["PagesRange"] = StiLocalization.Get("Report", "RangePage");
            words["PagesRangeAll"] = StiLocalization.Get("Report", "RangeAll");
            words["PagesRangeCurrentPage"] = StiLocalization.Get("Report", "RangeCurrentPage");
            words["PagesRangePages"] = StiLocalization.Get("Report", "RangePages").Replace(":", "");
            words["PagesRangeAllTooltip"] = StiLocalization.Get("HelpViewer", "PageAll");
            words["PagesRangeCurrentPageTooltip"] = StiLocalization.Get("HelpViewer", "CurrentPage");
            words["PagesRangePagesTooltip"] = StiLocalization.Get("HelpViewer", "RangePages");
            words["SettingsGroup"] = StiLocalization.Get("Export", "Settings");
            words["Type"] = StiLocalization.Get("PropertyMain", "Type").Replace(":", "");
            words["TypeTooltip"] = StiLocalization.Get("HelpViewer", "TypeExport");
            words["ZoomHtml"] = StiLocalization.Get("Export", "Scale").Replace(":", ""); ;
            words["ZoomHtmlTooltip"] = StiLocalization.Get("HelpViewer", "ScaleHtml");
            words["ImageFormatForHtml"] = StiLocalization.Get("Export", "ImageFormat").Replace(":", "");
            words["ImageFormatForHtmlTooltip"] = StiLocalization.Get("HelpViewer", "ImageFormat");
            words["SavingReport"] = StiLocalization.Get("DesignerFx", "SavingReport");
            words["EmailSuccessfullySent"] = StiLocalization.Get("DesignerFx", "EmailSuccessfullySent");
            words["SaveReportMdc"] = StiLocalization.Get("FormViewer", "DocumentFile").Replace("...", "") + " (.mdc)";
            words["SaveReportMdz"] = StiLocalization.Get("FormViewer", "CompressedDocumentFile") + " (.mdz)";
            words["SaveReportMdx"] = StiLocalization.Get("FormViewer", "EncryptedDocumentFile") + " (.mdx)";
            words["PasswordEnter"] = StiLocalization.Get("Password", "lbPasswordLoad");
            words["PasswordSaveReport"] = StiLocalization.Get("Report", "LabelPassword");
            words["PasswordSaveReportTooltip"] = StiLocalization.Get("HelpViewer", "UserPassword");
            words["PasswordNotEntered"] = StiLocalization.Get("Password", "PasswordNotEntered");
            words["ExportMode"] = StiLocalization.Get("Export", "ExportMode").Replace(":", "");
            words["ExportModeTooltip"] = StiLocalization.Get("HelpViewer", "ExportMode");
            words["CompressToArchive"] = StiLocalization.Get("Export", "CompressToArchive");
            words["CompressToArchiveTooltip"] = StiLocalization.Get("HelpViewer", "CompressToArchive");
            words["EmbeddedImageData"] = StiLocalization.Get("Export", "EmbeddedImageData");
            words["EmbeddedImageDataTooltip"] = StiLocalization.Get("HelpViewer", "EmbeddedImageData");
            words["AddPageBreaks"] = StiLocalization.Get("Export", "AddPageBreaks");
            words["AddPageBreaksTooltip"] = StiLocalization.Get("HelpViewer", "AddPageBreaks");
            words["ImageResolution"] = StiLocalization.Get("Export", "ImageResolution").Replace(":", "");
            words["ImageResolutionTooltip"] = StiLocalization.Get("HelpViewer", "ImageResolution");
            words["ImageCompressionMethod"] = StiLocalization.Get("Export", "ImageCompressionMethod").Replace(":", "");
            words["ImageCompressionMethodTooltip"] = StiLocalization.Get("HelpViewer", "ImageCompressionMethod");
            words["ImageQuality"] = StiLocalization.Get("Export", "ImageQuality").Replace(":", "");
            words["ImageQualityTooltip"] = StiLocalization.Get("HelpViewer", "ImageQuality");
            words["ContinuousPages"] = StiLocalization.Get("Export", "ContinuousPages");
            words["ContinuousPagesTooltip"] = StiLocalization.Get("HelpViewer", "ContinuousPages");
            words["StandardPDFFonts"] = StiLocalization.Get("Export", "StandardPDFFonts");
            words["StandardPDFFontsTooltip"] = StiLocalization.Get("HelpViewer", "StandardPdfFonts");
            words["EmbeddedFonts"] = StiLocalization.Get("Export", "EmbeddedFonts");
            words["EmbeddedFontsTooltip"] = StiLocalization.Get("HelpViewer", "EmbeddedFonts");
            words["UseUnicode"] = StiLocalization.Get("Export", "UseUnicode");
            words["UseUnicodeTooltip"] = StiLocalization.Get("HelpViewer", "UseUnicode");
            words["Compressed"] = StiLocalization.Get("Export", "Compressed");
            words["CompressedTooltip"] = StiLocalization.Get("HelpViewer", "Compressed");
            words["ExportRtfTextAsImage"] = StiLocalization.Get("Export", "ExportRtfTextAsImage");
            words["ExportRtfTextAsImageTooltip"] = StiLocalization.Get("HelpViewer", "ExportRtfTextAsImage");
            words["PdfACompliance"] = StiLocalization.Get("Export", "PdfACompliance");
            words["PdfAComplianceTooltip"] = StiLocalization.Get("HelpViewer", "PdfACompliance");
            words["KillSpaceLines"] = StiLocalization.Get("Export", "TxtKillSpaceLines");
            words["KillSpaceLinesTooltip"] = StiLocalization.Get("HelpViewer", "KillSpaceLines");
            words["PutFeedPageCode"] = StiLocalization.Get("Export", "TxtPutFeedPageCode");
            words["PutFeedPageCodeTooltip"] = StiLocalization.Get("HelpViewer", "PutFeedPageCode");
            words["DrawBorder"] = StiLocalization.Get("Export", "TxtDrawBorder");
            words["DrawBorderTooltip"] = StiLocalization.Get("HelpViewer", "DrawBorder");
            words["CutLongLines"] = StiLocalization.Get("Export", "TxtCutLongLines");
            words["CutLongLinesTooltip"] = StiLocalization.Get("HelpViewer", "CutLongLines");
            words["BorderType"] = StiLocalization.Get("Export", "TxtBorderType").Replace(":", "");
            words["BorderTypeTooltip"] = StiLocalization.Get("HelpViewer", "BorderType");
            words["BorderTypeSimple"] = StiLocalization.Get("Export", "TxtBorderTypeSimple");
            words["BorderTypeSingle"] = StiLocalization.Get("Export", "TxtBorderTypeSingle");
            words["BorderTypeDouble"] = StiLocalization.Get("Export", "TxtBorderTypeDouble");
            words["ZoomXY"] = StiLocalization.Get("Export", "Zoom").Replace(":", "");
            words["ZoomXYTooltip"] = StiLocalization.Get("HelpViewer", "ZoomTxt");
            words["EncodingData"] = StiLocalization.Get("Export", "Encoding").Replace(":", "");
            words["EncodingDataTooltip"] = StiLocalization.Get("HelpViewer", "EncodingData");
            words["ImageFormat"] = StiLocalization.Get("Export", "ImageType");
            words["ImageFormatTooltip"] = StiLocalization.Get("HelpViewer", "ImageType");
            words["ImageFormatColor"] = StiLocalization.Get("PropertyMain", "Color");
            words["ImageFormatGrayscale"] = StiLocalization.Get("Export", "ImageGrayscale");
            words["ImageFormatMonochrome"] = StiLocalization.Get("Export", "ImageMonochrome").Replace(":", "");
            words["MonochromeDitheringType"] = StiLocalization.Get("Export", "MonochromeDitheringType").Replace(":", "");
            words["MonochromeDitheringTypeTooltip"] = StiLocalization.Get("HelpViewer", "DitheringType");
            words["TiffCompressionScheme"] = StiLocalization.Get("Export", "TiffCompressionScheme").Replace(":", "");
            words["TiffCompressionSchemeTooltip"] = StiLocalization.Get("HelpViewer", "TiffCompressionScheme");
            words["CutEdges"] = StiLocalization.Get("Export", "ImageCutEdges");
            words["CutEdgesTooltip"] = StiLocalization.Get("HelpViewer", "CutEdges");
            words["MultipleFiles"] = StiLocalization.Get("Export", "MultipleFiles");
            words["MultipleFilesTooltip"] = StiLocalization.Get("HelpViewer", "MultipleFiles");
            words["ExportDataOnly"] = StiLocalization.Get("Export", "ExportDataOnly");
            words["ExportDataOnlyTooltip"] = StiLocalization.Get("HelpViewer", "ExportDataOnly");
            words["UseDefaultSystemEncoding"] = StiLocalization.Get("Export", "UseDefaultSystemEncoding");
            words["UseDefaultSystemEncodingTooltip"] = StiLocalization.Get("HelpViewer", "UseDefaultSystemEncoding");
            words["EncodingDifFile"] = StiLocalization.Get("Export", "Encoding").Replace(":", "");
            words["EncodingDifFileTooltip"] = StiLocalization.Get("HelpViewer", "EncodingData");
            words["ExportModeRtf"] = StiLocalization.Get("Export", "ExportMode").Replace(":", "");
            words["ExportModeRtfTooltip"] = StiLocalization.Get("HelpViewer", "ExportModeRtf");
            words["ExportModeRtfTable"] = StiLocalization.Get("Export", "ExportModeTable");
            words["ExportModeRtfFrame"] = StiLocalization.Get("Export", "ExportModeFrame");
            words["UsePageHeadersFooters"] = StiLocalization.Get("Export", "UsePageHeadersAndFooters");
            words["UsePageHeadersFootersTooltip"] = StiLocalization.Get("HelpViewer", "UsePageHeadersAndFooters");
            words["RemoveEmptySpace"] = StiLocalization.Get("Export", "RemoveEmptySpaceAtBottom");
            words["RemoveEmptySpaceTooltip"] = StiLocalization.Get("HelpViewer", "RemoveEmptySpaceAtBottom");
            words["Separator"] = StiLocalization.Get("Export", "Separator").Replace(":", "");
            words["SeparatorTooltip"] = StiLocalization.Get("HelpViewer", "Separator");
            words["SkipColumnHeaders"] = StiLocalization.Get("Export", "SkipColumnHeaders");
            words["SkipColumnHeadersTooltip"] = StiLocalization.Get("HelpViewer", "SkipColumnHeaders");
            words["ExportObjectFormatting"] = StiLocalization.Get("Export", "ExportObjectFormatting");
            words["ExportObjectFormattingTooltip"] = StiLocalization.Get("HelpViewer", "ExportObjectFormatting");
            words["UseOnePageHeaderFooter"] = StiLocalization.Get("Export", "UseOnePageHeaderAndFooter");
            words["UseOnePageHeaderFooterTooltip"] = StiLocalization.Get("HelpViewer", "UseOnePageHeaderAndFooter");
            words["ExportEachPageToSheet"] = StiLocalization.Get("Export", "ExportEachPageToSheet");
            words["ExportEachPageToSheetTooltip"] = StiLocalization.Get("HelpViewer", "ExportEachPageToSheet");
            words["ExportPageBreaks"] = StiLocalization.Get("Export", "ExportPageBreaks");
            words["ExportPageBreaksTooltip"] = StiLocalization.Get("HelpViewer", "ExportPageBreaks");
            words["EncodingDbfFile"] = StiLocalization.Get("Export", "Encoding").Replace(":", "");
            words["EncodingDbfFileTooltip"] = StiLocalization.Get("HelpViewer", "EncodingData");
            words["DocumentSecurityButton"] = StiLocalization.Get("Export", "DocumentSecurity");
            words["DigitalSignatureButton"] = StiLocalization.Get("Export", "DigitalSignature");
            words["OpenAfterExport"] = StiLocalization.Get("Export", "OpenAfterExport");
            words["OpenAfterExportTooltip"] = StiLocalization.Get("HelpViewer", "OpenAfterExport");
            words["AllowEditable"] = StiLocalization.Get("Export", "AllowEditable").Replace(":", "");
            words["AllowEditableTooltip"] = StiLocalization.Get("HelpViewer", "AllowEditable");
            words["NameYes"] = StiLocalization.Get("FormFormatEditor", "nameYes");
            words["NameNo"] = StiLocalization.Get("FormFormatEditor", "nameNo");
            words["UserPassword"] = StiLocalization.Get("Export", "labelUserPassword").Replace(":", "");
            words["UserPasswordTooltip"] = StiLocalization.Get("HelpViewer", "UserPassword");
            words["OwnerPassword"] = StiLocalization.Get("Export", "labelOwnerPassword").Replace(":", "");
            words["OwnerPasswordTooltip"] = StiLocalization.Get("HelpViewer", "OwnerPassword");
            words["BandsFilter"] = StiLocalization.Get("Export", "BandsFilter").Replace(":", "");
            words["BandsFilterTooltip"] = StiLocalization.Get("HelpViewer", "ExportMode");
            words["BandsFilterAllBands"] = StiLocalization.Get("Export", "AllBands");
            words["BandsFilterDataOnly"] = StiLocalization.Get("Export", "DataOnly");
            words["BandsFilterDataAndHeaders"] = StiLocalization.Get("Export", "DataAndHeaders");
            words["BandsFilterDataAndHeadersFooters"] = StiLocalization.Get("Export", "DataAndHeadersFooters");
            words["Null"] = StiLocalization.Get("Report", "Null");
            words["ViewData"] = StiLocalization.Get("FormTitles", "ViewDataForm");
            words["SelectColumns"] = StiLocalization.Get("Wizards", "SelectColumns");
            words["ImageResolutionModeExactly"] = StiLocalization.Get("Export", "Exactly");
            words["ImageResolutionModeNoMoreThan"] = StiLocalization.Get("Export", "NoMoreThan");
            words["ImageResolutionModeAuto"] = StiLocalization.Get("Export", "Auto");
            words["ImageResolutionMode"] = StiLocalization.Get("Export", "ImageResolutionMode").Replace(":", "");

            words["AllowPrintDocument"] = StiLocalization.Get("Export", "AllowPrintDocument");
            words["AllowPrintDocumentTooltip"] = StiLocalization.Get("HelpViewer", "AllowPrintDocument");
            words["AllowModifyContents"] = StiLocalization.Get("Export", "AllowModifyContents");
            words["AllowModifyContentsTooltip"] = StiLocalization.Get("HelpViewer", "AllowModifyContents");
            words["AllowCopyTextAndGraphics"] = StiLocalization.Get("Export", "AllowCopyTextAndGraphics");
            words["AllowCopyTextAndGraphicsTooltip"] = StiLocalization.Get("HelpViewer", "AllowCopyTextAndGraphics");
            words["AllowAddOrModifyTextAnnotations"] = StiLocalization.Get("Export", "AllowAddOrModifyTextAnnotations");
            words["AllowAddOrModifyTextAnnotationsTooltip"] = StiLocalization.Get("HelpViewer", "AllowAddOrModifyTextAnnotations");
            words["EncryptionKeyLength"] = StiLocalization.Get("Export", "labelEncryptionKeyLength");
            words["EncryptionKeyLengthTooltip"] = StiLocalization.Get("HelpViewer", "EncryptionKeyLength");
            words["EnableAnimation"] = StiLocalization.Get("Export", "EnableAnimation");

            words["UseDigitalSignature"] = StiLocalization.Get("Export", "UseDigitalSignature");
            words["UseDigitalSignatureTooltip"] = StiLocalization.Get("HelpViewer", "DigitalSignature");
            words["GetCertificateFromCryptoUI"] = StiLocalization.Get("Export", "GetCertificateFromCryptoUI");
            words["GetCertificateFromCryptoUITooltip"] = StiLocalization.Get("HelpViewer", "GetCertificateFromCryptoUI");
            words["SubjectNameString"] = StiLocalization.Get("Export", "labelSubjectNameString").Replace(":", "");
            words["SubjectNameStringTooltip"] = StiLocalization.Get("HelpViewer", "SubjectNameString");

            words["MonthJanuary"] = StiLocalization.Get("A_WebViewer", "MonthJanuary");
            words["MonthFebruary"] = StiLocalization.Get("A_WebViewer", "MonthFebruary");
            words["MonthMarch"] = StiLocalization.Get("A_WebViewer", "MonthMarch");
            words["MonthApril"] = StiLocalization.Get("A_WebViewer", "MonthApril");
            words["MonthMay"] = StiLocalization.Get("A_WebViewer", "MonthMay");
            words["MonthJune"] = StiLocalization.Get("A_WebViewer", "MonthJune");
            words["MonthJuly"] = StiLocalization.Get("A_WebViewer", "MonthJuly");
            words["MonthAugust"] = StiLocalization.Get("A_WebViewer", "MonthAugust");
            words["MonthSeptember"] = StiLocalization.Get("A_WebViewer", "MonthSeptember");
            words["MonthOctober"] = StiLocalization.Get("A_WebViewer", "MonthOctober");
            words["MonthNovember"] = StiLocalization.Get("A_WebViewer", "MonthNovember");
            words["MonthDecember"] = StiLocalization.Get("A_WebViewer", "MonthDecember");

            words["DayMonday"] = StiLocalization.Get("A_WebViewer", "DayMonday");
            words["DayTuesday"] = StiLocalization.Get("A_WebViewer", "DayTuesday");
            words["DayWednesday"] = StiLocalization.Get("A_WebViewer", "DayWednesday");
            words["DayThursday"] = StiLocalization.Get("A_WebViewer", "DayThursday");
            words["DayFriday"] = StiLocalization.Get("A_WebViewer", "DayFriday");
            words["DaySaturday"] = StiLocalization.Get("A_WebViewer", "DaySaturday");
            words["DaySunday"] = StiLocalization.Get("A_WebViewer", "DaySunday");

            words["AbbreviatedDayMonday"] = StiLocalization.Get("A_WebViewer", "AbbreviatedDayMonday");
            words["AbbreviatedDayTuesday"] = StiLocalization.Get("A_WebViewer", "AbbreviatedDayTuesday");
            words["AbbreviatedDayWednesday"] = StiLocalization.Get("A_WebViewer", "AbbreviatedDayWednesday");
            words["AbbreviatedDayThursday"] = StiLocalization.Get("A_WebViewer", "AbbreviatedDayThursday");
            words["AbbreviatedDayFriday"] = StiLocalization.Get("A_WebViewer", "AbbreviatedDayFriday");
            words["AbbreviatedDaySaturday"] = StiLocalization.Get("A_WebViewer", "AbbreviatedDaySaturday");
            words["AbbreviatedDaySunday"] = StiLocalization.Get("A_WebViewer", "AbbreviatedDaySunday");

            words["FormViewerTitle"] = StiLocalization.Get("FormViewer", "title");
            words["Error"] = StiLocalization.Get("Errors", "Error");
            words["SelectAll"] = StiLocalization.Get("MainMenu", "menuEditSelectAll").Replace("&", "");           

            words["CurrentMonth"] = StiLocalization.Get("DatePickerRanges", "CurrentMonth");
            words["CurrentQuarter"] = StiLocalization.Get("DatePickerRanges", "CurrentQuarter");
            words["CurrentWeek"] = StiLocalization.Get("DatePickerRanges", "CurrentWeek");
            words["CurrentYear"] = StiLocalization.Get("DatePickerRanges", "CurrentYear");
            words["NextMonth"] = StiLocalization.Get("DatePickerRanges", "NextMonth");
            words["NextQuarter"] = StiLocalization.Get("DatePickerRanges", "NextQuarter");
            words["NextWeek"] = StiLocalization.Get("DatePickerRanges", "NextWeek");
            words["NextYear"] = StiLocalization.Get("DatePickerRanges", "NextYear");
            words["PreviousMonth"] = StiLocalization.Get("DatePickerRanges", "PreviousMonth");
            words["PreviousQuarter"] = StiLocalization.Get("DatePickerRanges", "PreviousQuarter");
            words["PreviousWeek"] = StiLocalization.Get("DatePickerRanges", "PreviousWeek");
            words["PreviousYear"] = StiLocalization.Get("DatePickerRanges", "PreviousYear");
            words["FirstQuarter"] = StiLocalization.Get("DatePickerRanges", "FirstQuarter");
            words["SecondQuarter"] = StiLocalization.Get("DatePickerRanges", "SecondQuarter");
            words["ThirdQuarter"] = StiLocalization.Get("DatePickerRanges", "ThirdQuarter");
            words["FourthQuarter"] = StiLocalization.Get("DatePickerRanges", "FourthQuarter");
            words["MonthToDate"] = StiLocalization.Get("DatePickerRanges", "MonthToDate");
            words["QuarterToDate"] = StiLocalization.Get("DatePickerRanges", "QuarterToDate");
            words["WeekToDate"] = StiLocalization.Get("DatePickerRanges", "WeekToDate");
            words["YearToDate"] = StiLocalization.Get("DatePickerRanges", "YearToDate");
            words["Today"] = StiLocalization.Get("DatePickerRanges", "Today");
            words["Tomorrow"] = StiLocalization.Get("DatePickerRanges", "Tomorrow");
            words["Yesterday"] = StiLocalization.Get("DatePickerRanges", "Yesterday");
            words["Last7Days"] = StiLocalization.Get("DatePickerRanges", "Last7Days");
            words["Last14Days"] = StiLocalization.Get("DatePickerRanges", "Last14Days");
            words["Last30Days"] = StiLocalization.Get("DatePickerRanges", "Last30Days");
                        
            words["SaveFile"] = StiLocalization.Get("Cloud", "SaveFile");
            words["ButtonView"] = StiLocalization.Get("Cloud", "ButtonView");

            // Cloud
            words["QuotaMaximumReportPagesCountExceeded"] = StiLocalization.Get("Notices", "QuotaMaximumReportPagesCountExceeded");
            words["QuotaMaximumDataRowsCountExceeded"] = StiLocalization.Get("Notices", "QuotaMaximumDataRowsCountExceeded");
            words["QuotaMaximumResourceSizeExceeded"] = StiLocalization.Get("Notices", "QuotaMaximumResourceSizeExceeded");
            words["QuotaMaximumResourcesCountExceeded"] = StiLocalization.Get("Notices", "QuotaMaximumResourcesCountExceeded");
            words["QuotaMaximumDataRowsCountExceeded"] = StiLocalization.Get("Notices", "QuotaMaximumDataRowsCountExceeded");
            words["QuotaMaximumRefreshCountExceeded"] = StiLocalization.Get("Notices", "QuotaMaximumRefreshCountExceeded");

            words["New"] = StiLocalization.Get("MainMenu", "menuFileNew").Replace("&", "");
            words["Edit"] = StiLocalization.Get("MainMenu", "menuEditEdit");

            //Dashboards
            words["DashboardSortSmallestToLargest"] = StiLocalization.Get("Dashboard", "SortSmallestToLargest");
            words["DashboardSortLargestToSmallest"] = StiLocalization.Get("Dashboard", "SortLargestToSmallest");
            words["DashboardSortAZ"] = StiLocalization.Get("Dashboard", "SortAZ");
            words["DashboardSortZA"] = StiLocalization.Get("Dashboard", "SortZA");
            words["DashboardSortOldestToNewest"] = StiLocalization.Get("Dashboard", "SortOldestToNewest");
            words["DashboardSortNewestToOldest"] = StiLocalization.Get("Dashboard", "SortNewestToOldest");
            words["FormBandNoSort"] = StiLocalization.Get("FormBand", "NoSort");
            words["DashboardNulls"] = StiLocalization.Get("Dashboard", "Nulls");
            words["DashboardBlanks"] = StiLocalization.Get("Dashboard", "Blanks");
            words["DashboardNumberFilters"] = StiLocalization.Get("Dashboard", "NumberFilters");
            words["DashboardDateFilters"] = StiLocalization.Get("Dashboard", "DateFilters");
            words["DashboardBooleanFilters"] = StiLocalization.Get("Dashboard", "BooleanFilters");
            words["DashboardStringFilters"] = StiLocalization.Get("Dashboard", "StringFilters");
            words["DashboardCustomFilter"] = StiLocalization.Get("Dashboard", "CustomFilter").Replace("&", "").Replace("...", "");
            words["DashboardNSelected"] = StiLocalization.Get("Dashboard", "NSelected");
            words["DashboardAllValue"] = $"({StiLocalization.Get("Report", "RangeAll").Replace("&", "")})";
            words["DrillDown"] = StiLocalization.Get("Dashboard", "DrillDown");
            words["DrillUp"] = StiLocalization.Get("Dashboard", "DrillUp");
            words["DrillDownSelected"] = StiLocalization.Get("Dashboard", "DrillDownSelected");
            words["ReportSnapshot"] = StiLocalization.Get("Dashboard", "ReportSnapshot");
            words["Dashboard"] = StiLocalization.Get("Components", "StiDashboard");
            words["Report"] = StiLocalization.Get("Components", "StiReport");
            words["Sort"] = StiLocalization.Get("PropertyMain", "Sort");
            words["SortBy"] = StiLocalization.Get("PropertyMain", "SortBy");
            words["SortAsc"] = StiLocalization.Get("PropertyEnum", "StiSortDirectionAsc");
            words["SortDesc"] = StiLocalization.Get("PropertyEnum", "StiSortDirectionDesc");
            words["SortNone"] = StiLocalization.Get("PropertyEnum", "StiSortDirectionNone");
            words["Variation"] = StiLocalization.Get("PropertyEnum", "StiTargetModeVariation");
            words["DataNotFound"] = StiLocalization.Get("Errors", "DataNotFound");
            words["NoResult"] = StiLocalization.Get("Dashboard", "NoResult");

            words["Filter"] = StiLocalization.Get("PropertyMain", "Filter");
            words["Filters"] = StiLocalization.Get("PropertyMain", "Filters");
            words["AddFilter"] = StiLocalization.Get("FormBand", "AddFilter").Replace("&", "");
            words["RemoveFilter"] = StiLocalization.Get("FormBand", "RemoveFilter").Replace("&", "");
            words["FilterOn"] = StiLocalization.Get("PropertyMain", "FilterOn");
            words["NameTrue"] = StiLocalization.Get("FormFormatEditor", "nameTrue");
            words["NameFalse"] = StiLocalization.Get("FormFormatEditor", "nameFalse");
            words["FilterModeAnd"] = StiLocalization.Get("PropertyEnum", "StiFilterModeAnd");
            words["Expression"] = StiLocalization.Get("PropertyMain", "Expression");
            words["All"] = StiLocalization.Get("Report", "RangeAll");
            words["FilterMode"] = StiLocalization.Get("PropertyMain", "FilterMode");

            words["ConditionEqualTo"] = StiLocalization.Get("PropertyEnum", "StiFilterConditionEqualTo");
            words["ConditionNotEqualTo"] = StiLocalization.Get("PropertyEnum", "StiFilterConditionNotEqualTo");
            words["ConditionContaining"] = StiLocalization.Get("PropertyEnum", "StiFilterConditionContaining");
            words["ConditionNotContaining"] = StiLocalization.Get("PropertyEnum", "StiFilterConditionNotContaining");
            words["ConditionBeginningWith"] = StiLocalization.Get("PropertyEnum", "StiFilterConditionBeginningWith");
            words["ConditionEndingWith"] = StiLocalization.Get("PropertyEnum", "StiFilterConditionEndingWith");
            words["ConditionIsNull"] = StiLocalization.Get("PropertyEnum", "StiFilterConditionIsNull");
            words["ConditionIsNotNull"] = StiLocalization.Get("PropertyEnum", "StiFilterConditionIsNotNull");
            words["ConditionIsBlank"] = StiLocalization.Get("PropertyEnum", "StiFilterConditionIsBlank");
            words["ConditionIsNotBlank"] = StiLocalization.Get("PropertyEnum", "StiFilterConditionIsNotBlank");
            words["ConditionBetween"] = StiLocalization.Get("PropertyEnum", "StiFilterConditionBetween");
            words["ConditionNotBetween"] = StiLocalization.Get("PropertyEnum", "StiFilterConditionNotBetween");
            words["ConditionGreaterThan"] = StiLocalization.Get("PropertyEnum", "StiFilterConditionGreaterThan");
            words["ConditionGreaterThanOrEqualTo"] = StiLocalization.Get("PropertyEnum", "StiFilterConditionGreaterThanOrEqualTo");
            words["ConditionLessThan"] = StiLocalization.Get("PropertyEnum", "StiFilterConditionLessThan");
            words["ConditionLessThanOrEqualTo"] = StiLocalization.Get("PropertyEnum", "StiFilterConditionLessThanOrEqualTo");

            words["Refresh"] = StiLocalization.Get("PropertyMain", "Refresh");
            words["FullScreen"] = StiLocalization.Get("FormViewer", "FullScreen");
            words["Image"] = StiLocalization.Get("PropertyMain", "Image");
            words["Data"] = StiLocalization.Get("PropertyMain", "Data");
            words["Text"] = StiLocalization.Get("PropertyMain", "Text");
            words["RichText"] = StiLocalization.Get("Components", "StiRichText");

            words["PaperSize"] = StiLocalization.Get("PropertyMain", "PaperSize");
            words["Orientation"] = StiLocalization.Get("PropertyMain", "Orientation");
            words["Landscape"] = StiLocalization.Get("PropertyEnum", "StiPageOrientationLandscape");
            words["Portrait"] = StiLocalization.Get("PropertyEnum", "StiPageOrientationPortrait");
            words["Scale"] = StiLocalization.Get("PropertyMain", "Scale").Replace(":", "");
            words["DataType"] = StiLocalization.Get("PropertyMain", "DataType");

            words["AuthorizationWindowTitleLogin"] = StiLocalization.Get("Cloud", "WindowTitleLogin");
            words["AuthorizationWindowTitleSignUp"] = StiLocalization.Get("Cloud", "WindowTitleSignUp");
            words["AuthorizationWindowTitleForgotPassword"] = StiLocalization.Get("Cloud", "WindowTitleForgotPassword");
            words["AuthorizationTextUserName"] = StiLocalization.Get("Cloud", "TextUserName");
            words["AuthorizationTextPassword"] = StiLocalization.Get("Password", "StiSavePasswordForm");
            words["AuthorizationCheckBoxRememberMe"] = StiLocalization.Get("Cloud", "CheckBoxRememberMe");
            words["AuthorizationButtonLogin"] = StiLocalization.Get("Cloud", "Login");
            words["AuthorizationButtonSignUp"] = StiLocalization.Get("Cloud", "ButtonSignUp");

            words["NoticesYourTrialHasExpired"] = StiLocalization.Get("Notices", "YourTrialHasExpired");
            words["NoticesYouUsingTrialVersion"] = StiLocalization.Get("Notices", "YouUsingTrialVersion");
            words["ResetAllFilters"] = StiLocalization.Get("PropertyMain", "ResetAllFilters");
            

            words["FullName"] = StiLocalization.Get("PropertyMain", "FullName");
            words["Initials"] = StiLocalization.Get("PropertyMain", "Initials");
            words["ThemeColors"] = StiLocalization.Get("Gui", "colorpicker_themecolorslabel");
            words["StandardColors"] = StiLocalization.Get("Gui", "colorpicker_standardcolorslabel");
            words["MoreColors"] = StiLocalization.Get("Gui", "colorpicker_morecolors").Replace("&", "");
            words["NoFill"] = StiLocalization.Get("Gui", "colorpicker_nofill").Replace("&", "");
            words["Clear"] = StiLocalization.Get("Gui", "monthcalendar_clearbutton");
            words["Custom"] = StiLocalization.Get("FormColorBoxPopup", "Custom");
            words["Web"] = StiLocalization.Get("FormColorBoxPopup", "Web");
            words["ColorsCategory"] = StiLocalization.Get("PropertyCategory", "ColorsCategory");
            words["RedColor"] = StiLocalization.Get("PropertyColor", "Red");
            words["GreenColor"] = StiLocalization.Get("PropertyColor", "Green");
            words["BlueColor"] = StiLocalization.Get("PropertyColor", "Blue");
            words["Draw"] = StiLocalization.Get("PropertyEnum", "StiSignatureTypeDraw");
            words["Style"] = StiLocalization.Get("ChartRibbon", "Style");
            words["UseBrush"] = StiLocalization.Get("Signature", "UseBrush");
            words["InsertText"] = StiLocalization.Get("Signature", "InsertText");
            words["InsertImage"] = StiLocalization.Get("Signature", "InsertImage");

            words["FontStyleBold"] = StiLocalization.Get("HelpDesigner", "FontStyleBold");
            words["FontStyleItalic"] = StiLocalization.Get("HelpDesigner", "FontStyleItalic");
            words["FontStyleUnderline"] = StiLocalization.Get("HelpDesigner", "FontStyleUnderline");

            words["AlignLeft"] = StiLocalization.Get("HelpDesigner", "AlignLeft");
            words["AlignCenter"] = StiLocalization.Get("HelpDesigner", "AlignCenter");
            words["AlignRight"] = StiLocalization.Get("HelpDesigner", "AlignRight");

            words["AlignTop"] = StiLocalization.Get("HelpDesigner", "AlignTop");
            words["AlignMiddle"] = StiLocalization.Get("HelpDesigner", "AlignMiddle");
            words["AlignBottom"] = StiLocalization.Get("HelpDesigner", "AlignBottom");

            words["ButtonBack"] = StiLocalization.Get("Wizards", "ButtonBack").Replace("&", "");
            words["ButtonNext"] = StiLocalization.Get("Wizards", "ButtonNext").Replace("&", "");
            words["ButtonSign"] = StiLocalization.Get("Buttons", "Sign");
            words["ButtonRemove"] = StiLocalization.Get("Buttons", "Remove");

            words["TextDropImageHere"] = StiLocalization.Get("FormDictionaryDesigner", "TextDropImageHere");
            words["AspectRatio"] = StiLocalization.Get("PropertyMain", "AspectRatio");
            words["Stretch"] = StiLocalization.Get("PropertyMain", "Stretch");


            hash = StiMD5Helper.ComputeHash(path);
            LocalizationItems.GetOrAdd(hash, words);
            return words;
        }

        public static ArrayList GetDateRangesItems()
        {
            var items = new ArrayList();
            foreach (StiDateRangeKind item in StiOptions.Viewer.RequestFromUserDateRanges)
            {
                items.Add(item.ToString());
            }

            return items;
        }

        public static ArrayList GetPaperSizes()
        {
            var items = new ArrayList();

            var paperSizes = StiOptions.Print.CustomPaperSizes;
            try
            {
                if (paperSizes == null)
                {
#if STIDRAWING
                    if (Stimulsoft.Drawing.Graphics.GraphicsEngine == Drawing.GraphicsEngine.Gdi)
                    {
                        var printerSetting = new PrinterSettings();
                        paperSizes = printerSetting.PaperSizes;
                    }
                    else
                    {
                        paperSizes = new global::System.Drawing.Printing.PrinterSettings.PaperSizeCollection(new PaperSize[] { });
                    }
#else
					var printerSetting = new PrinterSettings();
					paperSizes = printerSetting.PaperSizes;
#endif
                }
                foreach (PaperSize paper in paperSizes)
                {
                    if (paper.Kind == PaperKind.Custom) continue;
                    items.Add(paper.PaperName);
                }
            }
            catch { }

            if (items.Count == 0)
                return new ArrayList(new string[] { "Letter", "Tabloid", "Legal", "Statement", "Executive", "A3", "A4", "A5", "B4", "B5" });

            return items;
        }

        private void InitializePaperSizes()
        {

        }
    }
}