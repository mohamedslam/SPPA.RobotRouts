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

using Stimulsoft.Base.Drawing;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Stimulsoft.Report.Chart.Geoms.Series
{
    internal static class StiSelectedHelper
    {
        #region Methods
        public static StiBrush GetSelectedBrush(StiBrush regularBrush)
        {
            if (regularBrush is StiSolidBrush)
            {
                var color = ((StiSolidBrush)regularBrush).Color;
                return new StiHatchBrush(HatchStyle.WideUpwardDiagonal, StiColorUtils.Light(color, 50), StiColorUtils.Dark(color, 50));
            }
            else
            {
                return new StiHatchBrush(HatchStyle.WideUpwardDiagonal, Color.Transparent, Color.FromArgb(100, Color.White));
            }
        }
        #endregion
    }
}
