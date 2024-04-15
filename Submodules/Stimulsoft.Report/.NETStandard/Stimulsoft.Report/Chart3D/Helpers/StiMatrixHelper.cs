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

namespace Stimulsoft.Report.Chart
{
    public static class StiMatrixHelper
    {
        #region Methods
        public static StiMatrix Translate(double tx, double ty, double tz)
        {
            return new StiMatrix(new double[,]{
                {1, 0, 0, 0 },
                {0, 1, 0, 0 },
                {0, 0, 1, 0 },
                { tx, ty, tz, 1 }
            });
        }

        public static StiMatrix Scale(double n)
        {
            return new StiMatrix(new double[,]{
                {n, 0, 0, 0 },
                {0, n, 0, 0 },
                {0, 0, n, 0 },
                {0, 0, 0, 1 }
            });
        }

        public static StiMatrix RotateX(double a)
        {
            return new StiMatrix(new double[,]{
                {1, 0, 0, 0},
                {0, Math.Cos(a), Math.Sin(a), 0 },
                {0, -Math.Sin(a), Math.Cos(a), 0 },
                {0, 0, 0, 1 }
            });
        }

        public static StiMatrix RotateY(double a)
        {
            return new StiMatrix(new double[,]{
                {Math.Cos(a), 0, -Math.Sin(a), 0},
                {0, 1, 0, 0 },
                {Math.Sin(a), 0, Math.Cos(a), 0 },
                {0, 0, 0, 1 }
            });
        }

        public static StiMatrix RotateZ(double a)
        {
            return new StiMatrix(new double[,]{
                {Math.Cos(a), Math.Sin(a), 0, 0},
                {-Math.Sin(a), Math.Cos(a), 0, 0 },
                {0, 0, 1, 0 },
                {0, 0, 0, 1 }
            });
        }
        #endregion
    }
}
