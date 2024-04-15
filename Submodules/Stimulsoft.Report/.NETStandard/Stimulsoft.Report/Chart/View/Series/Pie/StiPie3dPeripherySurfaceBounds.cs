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

using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public struct StiPie3dPeripherySurfaceBounds
    {
        #region Properties
        public float StartAngle { get; }

        public float EndAngle { get; }

        public PointF StartPoint { get; }

        public PointF EndPoint { get; }

        public float RealStartAngle { get; }

        public float RealEndAngle { get; }
        #endregion

        public StiPie3dPeripherySurfaceBounds(float startAngle, float endAngle, PointF startPoint, PointF endPoint, float realStartAngle, float realEndAngle)
        {
            StartAngle = startAngle;
            EndAngle = endAngle;
            StartPoint = startPoint;
            EndPoint = endPoint;
            RealStartAngle = realStartAngle;
            RealEndAngle = realEndAngle;
        }
    }
}
