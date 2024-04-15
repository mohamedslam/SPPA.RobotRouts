#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
using System.Collections;
using System.Drawing;
using Stimulsoft.Base;
using Stimulsoft.Report.Helpers;

namespace Stimulsoft.Report.Web
{
    internal class StiWebFontIconsHelper
    {        
        public static ArrayList GetIconItems()
        {
            var iconsCollection = new ArrayList();
            var iconsGroups = Enum.GetValues(typeof(StiFontIconGroup));

            for (var index = 0; index < iconsGroups.Length; index++)
            {
                var iconGroup = (StiFontIconGroup)iconsGroups.GetValue(index);
                var listIcons = StiFontIconsHelper.GetFontIcons(iconGroup);

                var group = new Hashtable();
                iconsCollection.Add(group);

                group["groupName"] = iconGroup.ToString();

                var items = new ArrayList();
                group["items"] = items;

                listIcons.ForEach(icon =>
                {
                    Hashtable item = new Hashtable();
                    item["text"] = StiFontIconsHelper.GetContent(icon);
                    item["key"] = icon.ToString();
                    items.Add(item);
                });
            }

            return iconsCollection;
        }

        public static ArrayList GetIconSetItems()
        {
            ArrayList items = new ArrayList();
            var iconSets = Enum.GetValues(typeof(StiFontIconSet));

            for (var index = 0; index < Enum.GetValues(typeof(StiFontIconSet)).Length; index++)
            {
                var iconSet = (StiFontIconSet)iconSets.GetValue(index);

                Hashtable item = new Hashtable();
                item["text"] = StiFontIconsHelper.GetIsonSetContent(iconSet);
                item["key"] = iconSet.ToString();
                items.Add(item);
            }

            return items;
        }
    }
}