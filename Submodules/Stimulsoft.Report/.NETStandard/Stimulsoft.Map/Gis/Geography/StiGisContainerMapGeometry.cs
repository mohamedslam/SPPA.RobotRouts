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
using System.Collections.Generic;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Media;
#else
using System.Windows.Media;
#endif

namespace Stimulsoft.Map.Gis.Geography
{
    public abstract class StiGisContainerMapGeometry : 
        StiGisMapGeometry
    {
        #region Fields
        public List<StiGisMapGeometry> Geoms = new List<StiGisMapGeometry>();
        #endregion

        #region Methods.override
        public override void Draw(Graphics g, StiGisCore core)
        {
            if (Geoms == null) return;

            foreach (var geom in Geoms)
            {
                geom.Draw(g, core);
            }
        }

        public override void Draw(DrawingContext dc, StiGisCore core)
        {
            if (Geoms == null) return;

            foreach (var geom in Geoms)
            {
                geom.Draw(dc, core);
            }
        }

        public override void UpdateLocalPosition(StiGisCore core)
        {
            if (Geoms == null) return;

            foreach (var geom in Geoms)
            {
                geom.UpdateLocalPosition(core);
            }
        }

        public override void GetAllPoints(ref List<StiGisPointLatLng> points)
        {
            foreach (var geom in Geoms)
            {
                geom.GetAllPoints(ref points);
            }
        }

        public override void Dispose()
        {
            if (Geoms == null) return;

            foreach (var geom in Geoms)
            {
                geom.Dispose();
            }

            Geoms.Clear();
            Geoms = null;
        }
        #endregion
    }
}