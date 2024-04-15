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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Components.MathFormula
{
    internal static class StiMathFormulaCache
    {
        #region class StiCacheSvgKey
        private struct StiCacheSvgKey
        {
            #region Properties
            public string Formula { get; set; }

            public float FontSize { get; set; }

            public string ColorHex { get; set; }
            #endregion

            public void Create(string latextContent, float fontSize, string colorHex)
            {
                FontSize = fontSize;
                Formula = latextContent;
                ColorHex = colorHex;
            }
        }
        #endregion

        #region class StiCacheKey
        private struct StiCacheKey
        {
            #region Properties
            public string Formula { get; set; }

            public SizeD Size { get; set; }

            public float FontSize { get; set; }

            public int Argb { get; set; }

            public StiVertAlignment VertAlignment { get; set; }

            public StiHorAlignment HorAlignment { get; set; }

            public double Zoom { get; set; }
            #endregion

            public void Create(StiMathFormula mathFormula)
            {
                VertAlignment = mathFormula.VertAlignment;
                HorAlignment = mathFormula.HorAlignment;
                Argb = StiBrush.ToColor(mathFormula.TextBrush).ToArgb();
                FontSize = mathFormula.Font.Size;
                Size = mathFormula.ClientRectangle.Size;
                Formula = mathFormula.Report != null && mathFormula.Report.IsDesigning ? mathFormula.LaTexExpression : mathFormula.Value;
                Zoom = mathFormula.Report.Info.Zoom;
            }
        }
        #endregion

        #region Fields.Static
        private static Dictionary<StiCacheSvgKey, string> svgCache = new Dictionary<StiCacheSvgKey, string>();

        private static Dictionary<StiCacheKey, Bitmap> imageCache = new Dictionary<StiCacheKey, Bitmap>();

        private static Hashtable latextMathMLCache = new Hashtable();
        #endregion

        #region Methods
        internal static void Clear()
        {
            svgCache.Clear();

            imageCache.Values.ToList().ForEach(v => v.Dispose());
            imageCache.Clear();
        }
        #endregion

        #region Methods.Svg
        internal static string GetSvg(string latextContent, float fontSize, string colorHex)
        {
            lock (svgCache)
            {
                var key = new StiCacheSvgKey();
                key.Create(latextContent, fontSize, colorHex);

                if (svgCache.ContainsKey(key))
                {
                    return svgCache[key];
                }

                return null;
            }
        }

        internal static void SetSvg(string svg, string latextContent, float fontSize, string colorHex)
        {
            lock (svgCache)
            {
                var key = new StiCacheSvgKey();
                key.Create(latextContent, fontSize, colorHex);

                svgCache[key] = svg;
            }
        }
        #endregion

        #region Methods.Image
        internal static Bitmap GetImage(StiMathFormula mathFormula)
        {
            lock (imageCache)
            {
                var key = new StiCacheKey();
                key.Create(mathFormula);

                if (imageCache.ContainsKey(key))
                {
                    return imageCache[key];
                }

                return null;
            }
        }

        internal static void SetImage(StiMathFormula mathFormula, string svgMathFormula, RectangleF rect)
        {
            lock (imageCache)
            {
                var image = new Bitmap((int)rect.Width, (int)rect.Height);
                var gg = Graphics.FromImage(image);

                var rectSvg = StiMathFormulaHelper.GetSvgRect(mathFormula, svgMathFormula, rect);

                StiSvgHelper.DrawWithSvg(svgMathFormula, new RectangleF(0, 0, rect.Width, rect.Height), rectSvg, mathFormula.Report.Info.Zoom, gg);

                SetImage(mathFormula, image);
            }
        }

        internal static void SetImage(StiMathFormula mathFormula, Bitmap image)
        {
            var key = new StiCacheKey();
            key.Create(mathFormula);

            imageCache[key] = image;
        }
        #endregion

        #region Methods.LatextMathML
        internal static void SetLatextMatml(string latextContent, string matmMLContent)
        {
            latextMathMLCache.Add(latextContent, matmMLContent);
        }

        internal static string GetLatextMatml(string latextContent)
        {
            if (latextMathMLCache.ContainsKey(latextContent))
                return latextMathMLCache[latextContent] as string;

            return null;
        }
        #endregion
    }
}
