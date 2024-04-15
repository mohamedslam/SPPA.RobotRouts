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

using System;
using System.Collections.Generic;

namespace Stimulsoft.Report.Toolbox
{
    public sealed class StiInfographicsToolboxInfo
    {
        #region Properties
        public object Tag { get; set; }

        public string LocName { get; set; }

        public string Icon { get; set; }

        public bool BeginGroup { get; set; }

        public bool SkipCategory { get; set; }

        public bool IsShowMore { get; set; }

        public StiInfographicsItemType ItemType { get; set; }

        public List<StiInfographicsToolboxInfo> Infos { get; set; }

        public byte[] CustomIcon { get; set; }
        #endregion

        #region Methods.override
        public override string ToString() => LocName;
        #endregion

        public StiInfographicsToolboxInfo(object tag, string locName, string icon)
        {
            this.Tag = tag;
            this.LocName = locName;
            this.Icon = icon;
        }

        public StiInfographicsToolboxInfo(Type type, string locName, string icon, bool beginGroup)
        {
            this.Tag = type;
            this.LocName = locName;
            this.Icon = icon;

            this.BeginGroup = beginGroup;
        }

        public StiInfographicsToolboxInfo(StiInfographicsItemType itemType, object tag, string locName, string icon)
        {
            this.ItemType = itemType;
            this.Tag = tag;
            this.LocName = locName;
            this.Icon = icon;
        }

        public StiInfographicsToolboxInfo(StiInfographicsItemType itemType, Type type, string locName, string icon, bool beginGroup)
        {
            this.ItemType = itemType;
            this.Tag = type;
            this.LocName = locName;
            this.Icon = icon;

            this.BeginGroup = beginGroup;
        }
    }
}