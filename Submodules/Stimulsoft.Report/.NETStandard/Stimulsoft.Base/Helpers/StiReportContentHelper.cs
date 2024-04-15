#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using System;
using System.IO;
using System.Text;

namespace Stimulsoft.Base.Helpers
{
    public static class StiReportContentHelper
    {
        #region class StiReportContent
        public sealed class StiReportContent
        {
            #region Properties
            public byte[] ReportIcon { get; set; }
            public string Path { get; set; }
            public string ReportName { get; set; } = string.Empty;
            public string ReportDescription { get; set; }
            public bool IsDbs { get; set; }
            public bool ContainsDbs { get; set; }
            public bool ContainsForms { get; set; }
            public bool IsMultiPages { get; set; }
            public bool IsLandscape { get; set; }
            public DateTime Modified { get; set; } = DateTime.Now;
            #endregion
        }
        #endregion

        #region Methods
        public static StiReportContentType GetReportType(string path)
        {
            try
            {
                var text = GetFileText(path);
                if (string.IsNullOrEmpty(text))
                    return StiReportContentType.Report;

                if (text.StartsWith("<"))
                {
                    return GetItemTypeFromXml(text);
                }
                else if (text.StartsWith("{"))
                {
                    return GetItemTypeFromJson(text);
                }
            }
            catch
            {
            }

            return StiReportContentType.Report;
        }

        public static StiReportContent Get(string path)
        {
            try
            {
                var text = GetFileText(path);
                if (string.IsNullOrEmpty(text)) 
                    return new StiReportContent();

                StiReportContent info = null;
                if (text.StartsWith("<"))
                {
                    info = GetReportInfoFromXml(text);
                }
                else if (text.StartsWith("{"))
                {
                    info = GetReportInfoFromJson(text);
                }

                if (info != null)
                {
                    info.Modified = File.GetLastWriteTimeUtc(path);
                    return info;
                }
            }
            catch { }

            return new StiReportContent();
        }

        public static bool ContainsDbs(string path)
        {
            try
            {
                var text = GetFileText(path);
                if (!string.IsNullOrEmpty(text))
                {
                    if (text.StartsWith("<"))
                    {
                        return IsReportXmlContainsDbs(text);
                    }
                    else if (text.StartsWith("{"))
                    {
                        return IsReportJsonContainsDbs(text);
                    }
                }
            }
            catch { }

            return false;
        }

        public static bool ContainsForms(string path)
        {
            try
            {
                var text = GetFileText(path);
                if (!string.IsNullOrEmpty(text))
                {
                    /*if (text.StartsWith("<"))
                    {
                        return IsReportXmlContainsDbs(text);
                    }
                    else */
                    if (text.StartsWith("{"))
                    {
                        return IsReportJsonContainsForms(text);
                    }
                }
            }
            catch { }

            return false;
        }

        private static string GetFileText(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            if (ext == ".mrt")
            {
                return File.ReadAllText(path);
            }
            else if (ext == ".mrz")
            {
                var bytes = File.ReadAllBytes(path);
                bytes = StiGZipHelper.Unpack(bytes);

                return Encoding.Default.GetString(bytes);
            }

            return null;
        }

        private static bool IsReportXmlContainsDbs(string text)
        {
            const string IdentStiDashboard1 = "type=\"Stimulsoft.Dashboard.Components.StiDashboard\"";
            const string IdentStiDashboard2 = "type=\"Dashboard\"";

            int indexDbs = text.IndexOfInvariant(IdentStiDashboard1);
            if (indexDbs == -1)
                indexDbs = text.IndexOfInvariant(IdentStiDashboard2);

            return indexDbs != -1;
        }

        private static StiReportContent GetReportInfoFromXml(string text)
        {
            const string BeginReportNameTag = "<ReportName>";
            const string EndReportNameTag = "</ReportName>";
            const string BeginReportDescriptionTag = "<ReportDescription>";
            const string EndReportDescriptionTag = "</ReportDescription>";

            const string IdentStiPage1 = "type=\"Stimulsoft.Dashboard.Components.StiPage\"";
            const string IdentStiDashboard1 = "type=\"Stimulsoft.Dashboard.Components.StiDashboard\"";
            const string IdentStiPage2 = "type=\"Page\"";
            const string IdentStiDashboard2 = "type=\"Dashboard\"";
            const string BeginReportIconTag = "<ReportIcon>";
            const string EndReportIconTag = "</ReportIcon>";

            var info = new StiReportContent();

            int index10 = text.IndexOfInvariant(BeginReportIconTag);
            int index11 = text.IndexOfInvariant(EndReportIconTag);
            if (index10 != -1 && index11 != -1 && index11 > index10)
            {
                try
                {
                    var iconStr = text.Substring(index10 + BeginReportIconTag.Length, index11 - index10 - BeginReportIconTag.Length);
                    info.ReportIcon = Convert.FromBase64String(iconStr);
                }
                catch { }
            }

            int index1 = text.IndexOfInvariant(BeginReportNameTag);
            int index2 = text.IndexOfInvariant(EndReportNameTag);
            if (index1 != -1 && index2 != -1 && index2 > index1)
            {
                info.ReportName = text.Substring(index1 + BeginReportNameTag.Length, index2 - index1 - BeginReportNameTag.Length);
            }

            index1 = text.IndexOfInvariant(BeginReportDescriptionTag);
            index2 = text.IndexOfInvariant(EndReportDescriptionTag);
            if (index1 != -1 && index2 != -1 && index2 > index1)
            {
                info.ReportDescription = text.Substring(index1 + BeginReportDescriptionTag.Length, index2 - index1 - BeginReportDescriptionTag.Length);
            }

            int indexPage = text.IndexOfInvariant(IdentStiPage1);
            if (indexPage == -1)
                indexPage = text.IndexOfInvariant(IdentStiPage2);
            int indexDbs = text.IndexOfInvariant(IdentStiDashboard1);
            if (indexDbs == -1)
                indexDbs = text.IndexOfInvariant(IdentStiDashboard2);

            info.ContainsDbs = indexDbs != -1;
            info.IsDbs = indexDbs != -1 && (indexPage == -1 || indexDbs < indexPage);

            int minIndex1 = 0;
            if (indexDbs == -1)
                minIndex1 = indexPage;
            else if (indexPage == -1)
                minIndex1 = indexDbs;
            else
                minIndex1 = Math.Min(indexPage, indexDbs);

            if (minIndex1 == -1) return info;

            indexPage = text.IndexOfInvariant(IdentStiPage1, minIndex1 + 10);
            if (indexPage == -1)
                indexPage = text.IndexOfInvariant(IdentStiPage2, minIndex1 + 10);
            indexDbs = text.IndexOfInvariant(IdentStiDashboard1, minIndex1 + 10);
            if (indexDbs == -1)
                indexDbs = text.IndexOfInvariant(IdentStiDashboard2, minIndex1 + 10);

            int minIndex2;
            if (indexDbs == -1)
                minIndex2 = indexPage;
            else if (indexPage == -1)
                minIndex2 = indexDbs;
            else
                minIndex2 = Math.Min(indexPage, indexDbs);

            if (minIndex2 > 0)
                info.IsMultiPages = true;

            var orientationValue = GetXmlProperty(text, "Orientation", minIndex1, minIndex2);
            if (orientationValue == "Landscape")
                info.IsLandscape = true;

            return info;
        }

        private static StiReportContentType GetItemTypeFromXml(string text)
        {
            //const string IdentStiPage1 = "type=\"Stimulsoft.Dashboard.Components.StiPage\"";
            const string IdentStiDashboard1 = "type=\"Stimulsoft.Dashboard.Components.StiDashboard\"";
            //const string IdentStiPage2 = "type=\"Page\"";
            const string IdentStiDashboard2 = "type=\"Dashboard\"";

            //int indexPage = text.IndexOfInvariant(IdentStiPage1);
            //if (indexPage == -1)
            //    indexPage = text.IndexOfInvariant(IdentStiPage2);
            int indexDbs = text.IndexOfInvariant(IdentStiDashboard1);
            if (indexDbs == -1)
                indexDbs = text.IndexOfInvariant(IdentStiDashboard2);

            if (indexDbs != -1)
                return StiReportContentType.Dashboard;
            else 
                return StiReportContentType.Report;
        }

        private static StiReportContent GetReportInfoFromJson(string text)
        {
            const string ReportNameTag = "\"ReportName\":";
            const string ReportDescriptionTag = "\"ReportDescription\":";
            const string IdentStiPage = "\"StiPage\",";
            const string IdentStiDashboard = "\"StiDashboard\",";
            const string ReportIconTag = "\"ReportIcon\":";

            var info = new StiReportContent();
            info.ContainsForms = text.IndexOfInvariant("\"StiFormContainer\",") != -1;


            int index10 = text.IndexOfInvariant(ReportIconTag);
            if (index10 != -1)
            {
                try
                {
                    int index11 = text.IndexOfInvariant("\"", index10 + ReportIconTag.Length) + 1;
                    int index12 = text.IndexOfInvariant("\",", index11);

                    var iconStr = text.Substring(index11, index12 - index11);
                    info.ReportIcon = Convert.FromBase64String(iconStr);
                }
                catch { }
            }

            int index1 = text.IndexOfInvariant(ReportNameTag);
            if (index1 != -1)
            {
                int index2 = text.IndexOfInvariant("\"", index1 + ReportNameTag.Length) + 1;
                int index3 = text.IndexOfInvariant("\",", index2);

                info.ReportName = text.Substring(index2, index3 - index2);
            }

            index1 = text.IndexOfInvariant(ReportDescriptionTag);
            if (index1 != -1)
            {
                int index2 = text.IndexOfInvariant("\"", index1 + ReportDescriptionTag.Length) + 1;
                int index3 = text.IndexOfInvariant("\",", index2);

                info.ReportDescription = text.Substring(index2, index3 - index2);
            }

            int indexPage = text.IndexOfInvariant(IdentStiPage);
            int indexDbs = text.IndexOfInvariant(IdentStiDashboard);

            info.ContainsDbs = indexDbs != -1;
            info.IsDbs = indexDbs != -1 && indexDbs < indexPage;

            int minIndex1;
            if (indexDbs == -1)
                minIndex1 = indexPage;
            else if (indexPage == -1)
                minIndex1 = indexDbs;
            else
                minIndex1 = Math.Min(indexPage, indexDbs);

            if (minIndex1 == -1) return info;

            indexPage = text.IndexOfInvariant(IdentStiPage, minIndex1 + 10);
            indexDbs = text.IndexOfInvariant(IdentStiDashboard, minIndex1 + 10);

            int minIndex2;
            if (indexDbs == -1)
                minIndex2 = indexPage;
            else if (indexPage == -1)
                minIndex2 = indexDbs;
            else
                minIndex2 = Math.Min(indexPage, indexDbs);

            if (minIndex2 > 0)
                info.IsMultiPages = true;

            var orientationValue = GetJsonProperty(text, "\"Orientation\"", minIndex1, minIndex2);
            if (orientationValue == "Landscape")
                info.IsLandscape = true;

            return info;
        }

        private static StiReportContentType GetItemTypeFromJson(string text)
        {
            //const string IdentStiPage = "\"StiPage\",";
            const string IdentStiDashboard = "\"StiDashboard\",";

            if (text.IndexOfInvariant("\"StiFormContainer\",") != -1)
                return StiReportContentType.Form;

            int indexDbs = text.IndexOfInvariant(IdentStiDashboard);
            if (indexDbs != -1)
                return StiReportContentType.Dashboard;
            return StiReportContentType.Report;
        }

        private static bool IsReportJsonContainsDbs(string text)
        {
            const string IdentStiDashboard = "\"StiDashboard\",";

            int indexDbs = text.IndexOfInvariant(IdentStiDashboard);
            return indexDbs != -1;
        }

        private static bool IsReportJsonContainsForms(string text)
        {
            const string IdentStiForm = "\"StiFormContainer\",";

            int index = text.IndexOfInvariant(IdentStiForm);
            return index != -1;
        }

        private static string GetJsonProperty(string text, string propName, int startIndex, int endIndex)
        {
            int index1 = text.IndexOfInvariant(propName, startIndex);
            if (index1 == -1) return string.Empty;

            if (endIndex != -1 && index1 >= endIndex)
                return string.Empty;

            int index2 = text.IndexOfInvariant("\"", index1 + propName.Length + 1);
            if (index2 == -1) return string.Empty;

            int index3 = text.IndexOfInvariant("\"", index2 + 1);
            if (index3 == -1) return string.Empty;

            return text.Substring(index2 + 1, index3 - index2 - 1);
        }

        private static string GetXmlProperty(string text, string propName, int startIndex, int endIndex)
        {
            var startName = $"<{propName}>";
            var endName = $"</{propName}>";

            int index1 = text.IndexOfInvariant(startName, startIndex);
            if (index1 == -1) return string.Empty;

            if (endIndex != -1 && index1 >= endIndex)
                return string.Empty;

            int index2 = text.IndexOfInvariant(endName, index1 + startName.Length);
            if (index2 == -1) return string.Empty;

            return text.Substring(index1 + startName.Length, index2 - index1 - startName.Length);
        }
        #endregion
    }
}