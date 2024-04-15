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

using Stimulsoft.Base.Drawing;
using System.Drawing;

#if STIDRAWING
using Brush = Stimulsoft.Drawing.Brush;
#endif

namespace Stimulsoft.Report.Export
{
    public class StiMapGroup
    {
        #region Properties
        public double MinValue { get; set; }

        public double MaxValue { get; set; }

        public Brush Fill { get; set; }

        public StiBrush Fill1 { get; set; }
        #endregion

        #region Methods
        public double GetOpacity(double value)
        {
            if (MaxValue == MinValue)
                return 1;

            var rest = MaxValue - MinValue;
            var percent = (value - MinValue) / rest;
            return percent * 0.8 + 0.2;
        }
        #endregion
    }
}