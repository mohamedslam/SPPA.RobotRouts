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
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Base.Serializing;
using System.ComponentModel;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Design;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components.Gauge.Primitives
{
    public class StiElementBase : ICloneable, IStiApplyStyleGauge
    {
        #region IStiApplyStyleGauge
        public virtual void ApplyStyle(IStiGaugeStyle style)
        {
            
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public virtual object Clone() => this.MemberwiseClone();
        #endregion

        #region Properties
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [DefaultValue(true)]
        [StiCategory("Appearance")]
        public virtual bool AllowApplyStyle { get; set; } = true;
        #endregion

        #region Methods
        protected internal virtual void DrawElement(StiGaugeContextPainter context)
        {

        }
        #endregion
    }
}