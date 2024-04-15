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

using Stimulsoft.Report.Helpers;
using System.Collections.Generic;
using System.Drawing;

namespace Stimulsoft.Base.Gis
{
    public sealed class StiGisMapData
    {
        #region Properties
        public Size Size { get; set; }
        public Color GeometryColor { get; set; }
        public double GeometryLineSize { get; set; }
        public List<string> Commands { get; set; }
        public List<string> Colors { get; set; }
        public List<object> LineSizes { get; set; }
        public List<string> Descriptions { get; set; }
        public bool ShowPlacemark { get; set; }
        public StiLanguageType Language { get; set; }
        public StiFontIcons Icon { get; }
        public Color IconColor { get; }
        #endregion

        public StiGisMapData(StiFontIcons icon, Color iconColor)
        {
            this.Icon = icon;
            this.IconColor = iconColor;
        }
    }
}