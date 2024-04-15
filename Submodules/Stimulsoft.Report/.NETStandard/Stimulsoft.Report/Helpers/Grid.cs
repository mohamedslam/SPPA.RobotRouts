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

using Stimulsoft.Report.Components;
using Stimulsoft.Report.Design;

namespace Stimulsoft.Report.Helpers
{
    public static class Grid
    {
        public static bool IsAlign(StiComponent comp)
        {
            return comp.Report.Info.AlignToGrid;
        }

        public static bool IsAlign(IStiDesignerBase designer)
        {
            return designer.Report.Info.AlignToGrid;
        }

        public static double SizeInPoints(StiComponent comp)
        {
            return comp.Report.Info.GridSizePoints;
        }

        public static double SizeInPoints(IStiDesignerBase designer)
        {
            return designer.Report.Info.GridSizePoints;
        }

        public static double SizeInCentimetres(IStiDesignerBase designer)
        {
            return designer.Report.Info.GridSizeCentimetres;
        }

        public static double SizeInHundredthsOfInch(IStiDesignerBase designer)
        {
            return designer.Report.Info.GridSizeHundredthsOfInch;
        }

        public static double SizeInInch(IStiDesignerBase designer)
        {
            return designer.Report.Info.GridSizeInch;
        }

        public static double SizeInMillimeters(IStiDesignerBase designer)
        {
            return designer.Report.Info.GridSizeMillimeters;
        }

        public static double SizeInPixels(IStiDesignerBase designer)
        {
            return designer.Report.Info.GridSizePixels;
        }
    }
}