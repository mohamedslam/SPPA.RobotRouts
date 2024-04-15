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

using Stimulsoft.Report.Maps;
using Stimulsoft.Report.Maps.Helpers;
using System.Collections.Generic;

namespace Stimulsoft.Report.Toolbox
{
    public sealed class StiMapToolboxInfo
    {
        #region Properties
        public StiMapCategory? Category { get; }
        public string MapID { get; }
        public string LocEnName { get; }
        public string LocRuName { get; }
        public string Icon { get; }
        public byte[] CustomIcon { get; }
        public string[] Language { get; set; }
        public List<StiMapToolboxInfo> Infos { get; set; }
        #endregion

        #region Methods.override
        public override string ToString() => this.MapID;
        #endregion

        #region Methods
        public string GetLangOriginalName(string id)
        {
            switch (id)
            {
                case "EN":
                    return "English";
                case "DE":
                    return "Deutsch";
                case "FR":
                    return "Français";
                case "IT":
                    return "Italiano";
                case "RU":
                    return "Русский";
                default:
                    return null;
            }
        }
        #endregion

        public StiMapToolboxInfo(StiMapID mapID) : 
            this(mapID, null)
        {

        }

        public StiMapToolboxInfo(StiMapID mapID, string[] localization)
        {
            StiMapLoc.GetEnRu(mapID, out var locEnName, out var locRuName);

            this.MapID = mapID.ToString();
            this.LocEnName = locEnName;
            this.LocRuName = locRuName;
            this.Language = localization;
            this.Icon = this.MapID;
        }

        public StiMapToolboxInfo(string customMapID, byte[] icon)
        {
            this.MapID = customMapID;
            this.LocEnName = customMapID;
            this.LocRuName = customMapID;
            this.CustomIcon = icon;
        }

        public StiMapToolboxInfo(StiMapCategory category, string locEnName, string locRuName)
        {
            this.Category = category;
            this.LocEnName = locEnName;
            this.LocRuName = locRuName;
        }
    }
}