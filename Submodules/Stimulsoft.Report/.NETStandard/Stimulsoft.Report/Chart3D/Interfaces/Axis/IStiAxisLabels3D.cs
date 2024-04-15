﻿#region Copyright (C) 2003-2022 Stimulsoft
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

using Stimulsoft.Base;
using System;
using Stimulsoft.Base.Design;
using System.Drawing;
using Stimulsoft.Report.Components.TextFormats;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Chart
{
    public interface IStiAxisLabels3D :
        ICloneable,
        IStiJsonReportObject,
        IStiDefault
    {
        #region Properties
        StiAxisLabelsCoreXF3D Core { get; set; }

        bool AllowApplyStyle { get; set; }

        Color Color { get; set; }

        Font Font { get; set; }

        string TextBefore { get; set; }

        string TextAfter { get; set; }

        string Format { get; set; }

        StiFormatService FormatService { get; set; }

        float CalculatedStep { get; set; }
        #endregion
    }
}
