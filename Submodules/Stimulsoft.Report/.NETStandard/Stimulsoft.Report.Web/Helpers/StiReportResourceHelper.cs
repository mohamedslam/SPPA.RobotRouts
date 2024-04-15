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

using Stimulsoft.Base;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Helpers;
using System;
using System.Collections;

namespace Stimulsoft.Report.Web
{
    internal class StiReportResourceHelper
    {
        public static ArrayList GetResourcesItems(StiReport report)
        {
            var items = new ArrayList();
            foreach (StiResource resource in report.Dictionary.Resources)
            {
                if (resource.AvailableInTheViewer)
                {
                    Hashtable resourceItem = new Hashtable();
                    resourceItem["name"] = resource.Name;
                    resourceItem["alias"] = resource.Alias;
                    resourceItem["type"] = resource.Type;
                    resourceItem["size"] = resource.Content != null ? resource.Content.Length : 0;

                    items.Add(resourceItem);
                }
            }

            return items.Count > 0 ? items : null;
        }

        public static string GetStimulsoftFontBase64Data()
        {
            using (var fontStream = typeof(StiFontIconsHelper).Assembly.GetManifestResourceStream("Stimulsoft.Base.FontIcons.Stimulsoft.ttf"))
            {
                byte[] buffer = new byte[fontStream.Length];
                fontStream.Read(buffer, 0, buffer.Length);

                return String.Format("data:application/x-font-ttf;base64,{0}", Convert.ToBase64String(buffer));
            }
        }        

        public static string GetBase64DataFromFontResourceContent(StiResourceType resourceType, byte[] content)
        {
            if (content != null)
            {
                var mimeType = "application/octet-stream";
                switch (resourceType)
                {
                    case StiResourceType.FontEot:
                        {
                            mimeType = "application/vnd.ms-fontobject";
                            break;
                        }
                    case StiResourceType.FontTtf:
                        {
                            mimeType = "application/x-font-ttf";
                            break;
                        }
                    case StiResourceType.FontWoff:
                        {
                            mimeType = "application/font-woff";
                            break;
                        }
                    case StiResourceType.FontOtf:
                        {
                            mimeType = "application/x-font-opentype";
                            break;
                        }
                }
                return String.Format("data:{0};base64,{1}", mimeType, Convert.ToBase64String(content));
            }
            else
                return String.Empty;
        }

        public static bool IsFontResourceType(StiResourceType resourceType)
        {
            return
                //resourceType == StiResourceType.FontEot ||
                //resourceType == StiResourceType.FontWoff |
                resourceType == StiResourceType.FontOtf ||
                resourceType == StiResourceType.FontTtc ||
                resourceType == StiResourceType.FontTtf;
        }

        public static ArrayList GetFontResourcesArray(StiReport report)
        {
            ArrayList fontResources = new ArrayList();

            if (report != null)
            {
                foreach (StiResource resource in report.Dictionary.Resources)
                {
                    if (IsFontResourceType(resource.Type))
                    {
                        Hashtable fontResourceItem = new Hashtable();
                        fontResourceItem["contentForCss"] = StiReportResourceHelper.GetBase64DataFromFontResourceContent(resource.Type, resource.Content);
                        fontResourceItem["originalFontFamily"] = StiFontCollection.GetFontFamily(report.GetResourceFontName(resource.Name)).Name;
                        fontResources.Add(fontResourceItem);
                    }
                }
            }

            return fontResources;
        }

        public static void LoadResourcesToReport(StiReport report, StiResourcesCollection resources)
        {
            report.Dictionary.Resources.Clear();

            foreach (StiResource resource in resources)
            {
                report.Dictionary.Resources.Add(resource);
            }
        }
    }
}