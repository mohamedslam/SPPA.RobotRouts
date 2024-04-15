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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Painters;
using System.Xml;
using System.Drawing;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Export.Services.Helpers
{
    public static class StiSparklineSvgHelper
    {
        public static void WriteSparkline(XmlTextWriter writer, StiSvgData svgData)
        {
            var sparkline = svgData.Component as StiSparkline;

            using (var img = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(img))
                {
                    var painter = new StiGdiContextPainter(g);
                    var context = new StiContext(painter, true, false, sparkline.IsPrinting, 1);

                    StiSparklineGdiPainter.RenderSparkline(context, new RectangleD(svgData.X, svgData.Y, svgData.Width, svgData.Height), sparkline, 1);

                    StiContextSvgHelper.WriteGeoms(writer, context, false);
                }
            }
        }
    }
}
