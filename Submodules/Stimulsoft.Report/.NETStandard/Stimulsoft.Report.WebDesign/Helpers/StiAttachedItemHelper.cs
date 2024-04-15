#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports 									            }
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.Base;

#if SERVER
using Stimulsoft.Server.Objects;
#endif

namespace Stimulsoft.Report.Web
{
#if SERVER       
    internal static class StiAttachedItemHelper
    {
        public static bool CanAttach(StiItem toItem, StiItem item)
        {
            if (item.Deleted) return false;

            var attachedItems = toItem as IStiAttachedItems;
            if (attachedItems == null) return false;
            return attachedItems.CanAttach(item);
        }

        public static bool CanAttachToReportTemplate(StiItem item)
        {
            var reportTemplateItem = new StiReportTemplateItem();
            return reportTemplateItem.CanAttach(item);
        }

        public static string GetDictionaryCloudItemsGroupName(StiItem item)
        {
            if (item is StiReportTemplateItem) 
                return "SubReport";

            if (item is StiDataSourceItem || item is StiDataElementItem) 
                return "Data";

            var fileItem = item as StiFileItem;
            if (fileItem != null)
            {
                if (fileItem.FileType == StiFileType.RichText) 
                    return "RichText";

                if (fileItem.FileType == StiFileType.Text)
                    return "Text";

                if (item is StiDataSourceItem || 
                    item is StiDataElementItem ||
                    fileItem.FileType == StiFileType.Data ||
                    fileItem.FileType == StiFileType.Json ||
                    fileItem.FileType == StiFileType.Excel ||
                    fileItem.FileType == StiFileType.Xml ||
                    fileItem.FileType == StiFileType.Xsd ||
                    fileItem.FileType == StiFileType.Csv ||
                    fileItem.FileType == StiFileType.Dif ||
                    fileItem.FileType == StiFileType.Sylk ||
                    fileItem.FileType == StiFileType.Dbf)
                    return "Data";

                if (fileItem.FileType == StiFileType.Image)
                    return "Image";
            }

            return "Unknown";
        }        
    }

    public enum StiCloudDesignerItemType
    {
        SubReport,
        DataSource,
        Data,
        Image,
        RichText,
        Text,
        Unknown
    }
#endif
}