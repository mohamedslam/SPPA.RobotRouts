﻿#region Copyright (C) 2003-2022 Stimulsoft
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

namespace Stimulsoft.Base.Maps.Geoms
{
    public class StiMoveToMapGeom : StiMapGeom
    {
        #region Properties
        public double X { get; set; }
        public double Y { get; set; }
        #endregion

        #region Properties.Override
        public override StiMapGeomType GeomType => StiMapGeomType.MoveTo;
        #endregion

        #region Methods.Override
        public override PointD GetLastPoint() => new PointD(X, Y);

        public override string ToString() => $"MoveTo={X},{Y}";
        #endregion
    }
}