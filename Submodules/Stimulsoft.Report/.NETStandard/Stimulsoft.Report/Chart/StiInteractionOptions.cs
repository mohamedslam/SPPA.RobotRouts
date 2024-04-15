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

using Stimulsoft.Base.Context;
using System;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiInteractionOptions
    {
        #region Properties
        public bool SelectOnClick { get; set; } = true;

        public bool UpdateContext { get; set; }

        public bool RecallEvent { get; set; }

        public TimeSpan RecallTime { get; set; } = TimeSpan.Zero;

        public bool IsRecalled { get; set; }

        public PointF MousePoint { get; set; } = PointF.Empty;

        public bool DragEnabled { get; set; }

        public SizeF DragDelta { get; set; } = SizeF.Empty;

        public string InteractionToolTip { get; set; }

        public StiInteractionToolTipPointOptions InteractionToolTipPoint { get; set; }

        public string InteractionHyperlink { get; set; }

        public StiSeriesInteractionData SeriesInteractionData { get; set; } 
        #endregion
    }
}