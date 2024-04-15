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

using Stimulsoft.Map.Gis.Geography;
using System;
using System.Collections.Generic;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Media;
#else
using System.Windows.Media;
#endif

namespace Stimulsoft.Map.Gis.Core
{
    public sealed class StiGisMapViewData : IDisposable
    {
        public StiGisMapViewData(StiGisCore core)
        {
            this.core = core;
        }

        #region Fields
        private StiGisCore core;
        internal readonly List<StiGisMapGeometry> Geoms = new List<StiGisMapGeometry>();
        internal readonly List<StiGisPointMapGeometry> Placemarks = new List<StiGisPointMapGeometry>();
        #endregion

        #region Properties
        public StiGeoRenderMode RenderMode => core.RenderMode;
        #endregion

        #region Methods
        public void Clear()
        {
            this.Geoms.Clear();
        }

        internal void ForceUpdate()
        {
            foreach (var geom in this.Geoms)
            {
                geom.UpdateLocalPosition(this.core);
            }
        }

        internal void Draw(Graphics g)
        {
            foreach (var geom in this.Geoms)
            {
                geom.Draw(g, core);
            }
        }

        internal void Draw(DrawingContext dc)
        {
            foreach (var geom in this.Geoms)
            {
                geom.Draw(dc, core);
            }
        }

        public List<StiGisPointLatLng> GetAllPoints()
        {
            var points = new List<StiGisPointLatLng>();
            foreach(var geom in this.Geoms)
            {
                geom.GetAllPoints(ref points);
            }

            return points;
        }
        #endregion

        #region IDisposable.override
        public void Dispose()
        {
            foreach (var geom in this.Geoms)
            {
                geom.Dispose();
            }

            this.Geoms.Clear();

            this.core = null;
        }
        #endregion
    }
}