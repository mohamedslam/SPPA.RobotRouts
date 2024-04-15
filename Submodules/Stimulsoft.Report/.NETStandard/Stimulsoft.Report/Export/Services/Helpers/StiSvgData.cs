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

namespace Stimulsoft.Report.Export
{
    public struct StiSvgData
    {
        /// <summary>
        /// X coordinate of the border.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y coordinate of the border.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Width of the border.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Height of the border.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Right (X + Width) coordinate of the border.
        /// </summary>
        public double Right => X + Width;

        /// <summary>
        /// Bottom (Y + Height) coordinate of the border.
        /// </summary>
        public double Bottom => Y + Height;

        /// <summary>
        /// Component.
        /// </summary>
        public StiComponent Component { get; set; }

        public StiSvgData Clone()
        {
            StiSvgData svg = new StiSvgData();
            svg.X = X;
            svg.Y = Y;
            svg.Width = Width;
            svg.Height = Height;
            svg.Component = Component;
            return svg;
        }
    }
}
