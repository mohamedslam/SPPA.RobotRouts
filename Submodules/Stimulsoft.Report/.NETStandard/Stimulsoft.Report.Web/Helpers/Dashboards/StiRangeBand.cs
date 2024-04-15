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

using System.Drawing;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiRangeBand
    {
        #region Properties
        public int Top { get; set; }

        public int Bottom { get; set; }

        public int Height => Bottom - Top;

        public int OriginalTop { get; }

        public int OriginalBottom { get; }

        public int OriginalHeight => OriginalBottom - OriginalTop;

        public bool IsFixed { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            var strFixed = IsFixed ? ", Fixed" : "";
            return $"Top-{Top}, Bottom-{Bottom}, Height-{Height}{strFixed}";
        }

        public bool Intersect(Rectangle rect)
        {
            return rect.Bottom > Top && rect.Top < Bottom;
        }
        #endregion

        public StiRangeBand(int top, int bottom)
        {
            OriginalTop = Top = top;
            OriginalBottom = Bottom = bottom;
        }
    }
}