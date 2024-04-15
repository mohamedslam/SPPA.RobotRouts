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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Text;

using Stimulsoft.Base;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiInterlacingCoreXF :
        IStiApplyStyle,
        ICloneable
	{
        #region ICloneable
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region IStiApplyStyle
        public virtual void ApplyStyle(IStiChartStyle style)
        {
            if (this.Interlacing.AllowApplyStyle)
            {
                if (this.Interlacing is IStiInterlacingVert)
                {
                    this.Interlacing.InterlacedBrush = style.Core.InterlacingVertBrush;
                }
                else
                {
                    this.Interlacing.InterlacedBrush = style.Core.InterlacingHorBrush;
                }
            }
        }
        #endregion

        #region Properties
        private IStiInterlacing interlacing;
        public IStiInterlacing Interlacing
        {
            get
            {
                return interlacing;
            }
            set
            {
                interlacing = value;
            }
        }
        #endregion

        public StiInterlacingCoreXF(IStiInterlacing interlacing)
        {
            this.interlacing = interlacing;
        }
	}
}
