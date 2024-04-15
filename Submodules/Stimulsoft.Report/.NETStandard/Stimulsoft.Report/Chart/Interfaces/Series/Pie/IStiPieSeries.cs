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

using System.Drawing;
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Report.Chart
{
    public interface IStiPieSeries : 
        IStiSeries,
        IStiFontIconsSeries,
        IStiSeriesBorderThickness,
        IStiAllowApplyBorderColor, 
        IStiAllowApplyBrush,
        IStiShowZerosSeries
    {
        #region Properties
        float StartAngle { get; set; }

        Color BorderColor { get; set; }

        StiBrush Brush { get; set; }

		bool Lighting { get; set; }
        
        float Diameter { get; set; }

        float Distance { get; set; }

        double[] CutPieListValues { get; set; }
        #endregion
	}
}
