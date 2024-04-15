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

using Stimulsoft.Map.Gis.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Media;
#else
using System.Windows.Media;
#endif

namespace Stimulsoft.Map.Gis.Geography
{
    public abstract class StiGisMapGeometry : 
        IDisposable
    {
        #region Fields
        public global::System.Drawing.Color? ColorGdi;
        public System.Windows.Media.Color? ColorWpf;
        public double? LineSize;
        #endregion

        #region Methods
        public abstract void Draw(Graphics g, StiGisCore core);

        public abstract void Draw(DrawingContext dc, StiGisCore core);

        public abstract void UpdateLocalPosition(StiGisCore core);

        public virtual void Dispose() { }

        public abstract void GetAllPoints(ref List<StiGisPointLatLng> points);

        protected global::System.Drawing.Color GetStrokeGdiColor(StiGisCore core)
        {
            return this.ColorGdi != null ? this.ColorGdi.GetValueOrDefault() : core.GeometryColor;
        }

        protected global::System.Drawing.Color GetFillGdiColor(StiGisCore core)
        {
            return this.ColorGdi != null ? global::System.Drawing.Color.FromArgb(30, this.ColorGdi.GetValueOrDefault()) : global::System.Drawing.Color.FromArgb(30, core.GeometryColor);
        }

        protected System.Windows.Media.Brush GetStrokeWpfColor(StiGisCore core)
        {
            return Extensions.ToWpfBrush(GetStrokeGdiColor(core));
        }

        protected System.Windows.Media.Brush GetFillWpfColor(StiGisCore core)
        {
            return Extensions.ToWpfBrush(GetFillGdiColor(core));
        }

        protected double GetLineSize(StiGisCore core)
        {
            return Stimulsoft.Base.StiScale.XXI((LineSize != null) ? LineSize.GetValueOrDefault() : core.GeometryLineSize);
        }
        #endregion
    }
}