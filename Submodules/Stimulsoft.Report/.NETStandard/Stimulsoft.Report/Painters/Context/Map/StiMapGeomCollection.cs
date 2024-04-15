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
using System.Collections.Generic;

namespace Stimulsoft.Base.Maps.Geoms
{
    public sealed class StiMapGeomCollection : List<StiMapGeom>
    {
        #region Methods
        public PointD GetLastPoint()
        {
            if (this.Count == 0) return new PointD();
            var lastGeom = this[this.Count - 1];

            if (lastGeom.GeomType == StiMapGeomType.Close)
            {
                if (this.Count > 0)
                {
                    lastGeom = this[this.Count - 2];
                    return lastGeom.GetLastPoint();
                }
            }

            return lastGeom.GetLastPoint();
        }
        #endregion
    }
}