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

using System;
using System.Drawing;

namespace Stimulsoft.Drawing
{
    public sealed class Pens
    {
        private static Pen aliceblue;
        private static Pen antiquewhite;
        private static Pen aqua;
        private static Pen aquamarine;
        private static Pen azure;
        private static Pen beige;
        private static Pen bisque;
        private static Pen black;
        private static Pen blanchedalmond;
        private static Pen blue;
        private static Pen blueviolet;
        private static Pen brown;
        private static Pen burlywood;
        private static Pen cadetblue;
        private static Pen chartreuse;
        private static Pen chocolate;
        private static Pen coral;
        private static Pen cornflowerblue;
        private static Pen cornsilk;
        private static Pen crimson;
        private static Pen cyan;
        private static Pen darkblue;
        private static Pen darkcyan;
        private static Pen darkgoldenrod;
        private static Pen darkgray;
        private static Pen darkgreen;
        private static Pen darkkhaki;
        private static Pen darkmagenta;
        private static Pen darkolivegreen;
        private static Pen darkorange;
        private static Pen darkorchid;
        private static Pen darkred;
        private static Pen darksalmon;
        private static Pen darkseagreen;
        private static Pen darkslateblue;
        private static Pen darkslategray;
        private static Pen darkturquoise;
        private static Pen darkviolet;
        private static Pen deeppink;
        private static Pen deepskyblue;
        private static Pen dimgray;
        private static Pen dodgerblue;
        private static Pen firebrick;
        private static Pen floralwhite;
        private static Pen forestgreen;
        private static Pen fuchsia;
        private static Pen gainsboro;
        private static Pen ghostwhite;
        private static Pen gold;
        private static Pen goldenrod;
        private static Pen gray;
        private static Pen green;
        private static Pen greenyellow;
        private static Pen honeydew;
        private static Pen hotpink;
        private static Pen indianred;
        private static Pen indigo;
        private static Pen ivory;
        private static Pen khaki;
        private static Pen lavender;
        private static Pen lavenderblush;
        private static Pen lawngreen;
        private static Pen lemonchiffon;
        private static Pen lightblue;
        private static Pen lightcoral;
        private static Pen lightcyan;
        private static Pen lightgoldenrodyellow;
        private static Pen lightgray;
        private static Pen lightgreen;
        private static Pen lightpink;
        private static Pen lightsalmon;
        private static Pen lightseagreen;
        private static Pen lightskyblue;
        private static Pen lightslategray;
        private static Pen lightsteelblue;
        private static Pen lightyellow;
        private static Pen lime;
        private static Pen limegreen;
        private static Pen linen;
        private static Pen magenta;
        private static Pen maroon;
        private static Pen mediumaquamarine;
        private static Pen mediumblue;
        private static Pen mediumorchid;
        private static Pen mediumpurple;
        private static Pen mediumseagreen;
        private static Pen mediumslateblue;
        private static Pen mediumspringgreen;
        private static Pen mediumturquoise;
        private static Pen mediumvioletred;
        private static Pen midnightblue;
        private static Pen mintcream;
        private static Pen mistyrose;
        private static Pen moccasin;
        private static Pen navajowhite;
        private static Pen navy;
        private static Pen oldlace;
        private static Pen olive;
        private static Pen olivedrab;
        private static Pen orange;
        private static Pen orangered;
        private static Pen orchid;
        private static Pen palegoldenrod;
        private static Pen palegreen;
        private static Pen paleturquoise;
        private static Pen palevioletred;
        private static Pen papayawhip;
        private static Pen peachpuff;
        private static Pen peru;
        private static Pen pink;
        private static Pen plum;
        private static Pen powderblue;
        private static Pen purple;
        private static Pen red;
        private static Pen rosybrown;
        private static Pen royalblue;
        private static Pen saddlebrown;
        private static Pen salmon;
        private static Pen sandybrown;
        private static Pen seagreen;
        private static Pen seashell;
        private static Pen sienna;
        private static Pen silver;
        private static Pen skyblue;
        private static Pen slateblue;
        private static Pen slategray;
        private static Pen snow;
        private static Pen springgreen;
        private static Pen steelblue;
        private static Pen tan;
        private static Pen teal;
        private static Pen thistle;
        private static Pen tomato;
        private static Pen transparent;
        private static Pen turquoise;
        private static Pen violet;
        private static Pen wheat;
        private static Pen white;
        private static Pen whitesmoke;
        private static Pen yellow;
        private static Pen yellowgreen;

        private Pens() { }

        public static Pen AliceBlue
        {
            get
            {
                if (aliceblue == null)
                {
                    aliceblue = new Pen(Color.AliceBlue);

                }
                return aliceblue;
            }
        }

        public static Pen AntiqueWhite
        {
            get
            {
                if (antiquewhite == null)
                {
                    antiquewhite = new Pen(Color.AntiqueWhite);

                }
                return antiquewhite;
            }
        }

        public static Pen Aqua
        {
            get
            {
                if (aqua == null)
                {
                    aqua = new Pen(Color.Aqua);

                }
                return aqua;
            }
        }

        public static Pen Aquamarine
        {
            get
            {
                if (aquamarine == null)
                {
                    aquamarine = new Pen(Color.Aquamarine);

                }
                return aquamarine;
            }
        }

        public static Pen Azure
        {
            get
            {
                if (azure == null)
                {
                    azure = new Pen(Color.Azure);

                }
                return azure;
            }
        }

        public static Pen Beige
        {
            get
            {
                if (beige == null)
                {
                    beige = new Pen(Color.Beige);

                }
                return beige;
            }
        }

        public static Pen Bisque
        {
            get
            {
                if (bisque == null)
                {
                    bisque = new Pen(Color.Bisque);

                }
                return bisque;
            }
        }

        public static Pen Black
        {
            get
            {
                if (black == null)
                {
                    black = new Pen(Color.Black);

                }
                return black;
            }
        }

        public static Pen BlanchedAlmond
        {
            get
            {
                if (blanchedalmond == null)
                {
                    blanchedalmond = new Pen(Color.BlanchedAlmond);

                }
                return blanchedalmond;
            }
        }

        public static Pen Blue
        {
            get
            {
                if (blue == null)
                {
                    blue = new Pen(Color.Blue);

                }
                return blue;
            }
        }

        public static Pen BlueViolet
        {
            get
            {
                if (blueviolet == null)
                {
                    blueviolet = new Pen(Color.BlueViolet);

                }
                return blueviolet;
            }
        }

        public static Pen Brown
        {
            get
            {
                if (brown == null)
                {
                    brown = new Pen(Color.Brown);

                }
                return brown;
            }
        }

        public static Pen BurlyWood
        {
            get
            {
                if (burlywood == null)
                {
                    burlywood = new Pen(Color.BurlyWood);

                }
                return burlywood;
            }
        }

        public static Pen CadetBlue
        {
            get
            {
                if (cadetblue == null)
                {
                    cadetblue = new Pen(Color.CadetBlue);

                }
                return cadetblue;
            }
        }

        public static Pen Chartreuse
        {
            get
            {
                if (chartreuse == null)
                {
                    chartreuse = new Pen(Color.Chartreuse);

                }
                return chartreuse;
            }
        }

        public static Pen Chocolate
        {
            get
            {
                if (chocolate == null)
                {
                    chocolate = new Pen(Color.Chocolate);

                }
                return chocolate;
            }
        }

        public static Pen Coral
        {
            get
            {
                if (coral == null)
                {
                    coral = new Pen(Color.Coral);

                }
                return coral;
            }
        }

        public static Pen CornflowerBlue
        {
            get
            {
                if (cornflowerblue == null)
                {
                    cornflowerblue = new Pen(Color.CornflowerBlue);

                }
                return cornflowerblue;
            }
        }

        public static Pen Cornsilk
        {
            get
            {
                if (cornsilk == null)
                {
                    cornsilk = new Pen(Color.Cornsilk);

                }
                return cornsilk;
            }
        }

        public static Pen Crimson
        {
            get
            {
                if (crimson == null)
                {
                    crimson = new Pen(Color.Crimson);

                }
                return crimson;
            }
        }

        public static Pen Cyan
        {
            get
            {
                if (cyan == null)
                {
                    cyan = new Pen(Color.Cyan);

                }
                return cyan;
            }
        }

        public static Pen DarkBlue
        {
            get
            {
                if (darkblue == null)
                {
                    darkblue = new Pen(Color.DarkBlue);

                }
                return darkblue;
            }
        }

        public static Pen DarkCyan
        {
            get
            {
                if (darkcyan == null)
                {
                    darkcyan = new Pen(Color.DarkCyan);

                }
                return darkcyan;
            }
        }

        public static Pen DarkGoldenrod
        {
            get
            {
                if (darkgoldenrod == null)
                {
                    darkgoldenrod = new Pen(Color.DarkGoldenrod);

                }
                return darkgoldenrod;
            }
        }

        public static Pen DarkGray
        {
            get
            {
                if (darkgray == null)
                {
                    darkgray = new Pen(Color.DarkGray);

                }
                return darkgray;
            }
        }

        public static Pen DarkGreen
        {
            get
            {
                if (darkgreen == null)
                {
                    darkgreen = new Pen(Color.DarkGreen);

                }
                return darkgreen;
            }
        }

        public static Pen DarkKhaki
        {
            get
            {
                if (darkkhaki == null)
                {
                    darkkhaki = new Pen(Color.DarkKhaki);

                }
                return darkkhaki;
            }
        }

        public static Pen DarkMagenta
        {
            get
            {
                if (darkmagenta == null)
                {
                    darkmagenta = new Pen(Color.DarkMagenta);

                }
                return darkmagenta;
            }
        }

        public static Pen DarkOliveGreen
        {
            get
            {
                if (darkolivegreen == null)
                {
                    darkolivegreen = new Pen(Color.DarkOliveGreen);

                }
                return darkolivegreen;
            }
        }

        public static Pen DarkOrange
        {
            get
            {
                if (darkorange == null)
                {
                    darkorange = new Pen(Color.DarkOrange);

                }
                return darkorange;
            }
        }

        public static Pen DarkOrchid
        {
            get
            {
                if (darkorchid == null)
                {
                    darkorchid = new Pen(Color.DarkOrchid);

                }
                return darkorchid;
            }
        }

        public static Pen DarkRed
        {
            get
            {
                if (darkred == null)
                {
                    darkred = new Pen(Color.DarkRed);

                }
                return darkred;
            }
        }

        public static Pen DarkSalmon
        {
            get
            {
                if (darksalmon == null)
                {
                    darksalmon = new Pen(Color.DarkSalmon);

                }
                return darksalmon;
            }
        }

        public static Pen DarkSeaGreen
        {
            get
            {
                if (darkseagreen == null)
                {
                    darkseagreen = new Pen(Color.DarkSeaGreen);

                }
                return darkseagreen;
            }
        }

        public static Pen DarkSlateBlue
        {
            get
            {
                if (darkslateblue == null)
                {
                    darkslateblue = new Pen(Color.DarkSlateBlue);

                }
                return darkslateblue;
            }
        }

        public static Pen DarkSlateGray
        {
            get
            {
                if (darkslategray == null)
                {
                    darkslategray = new Pen(Color.DarkSlateGray);

                }
                return darkslategray;
            }
        }

        public static Pen DarkTurquoise
        {
            get
            {
                if (darkturquoise == null)
                {
                    darkturquoise = new Pen(Color.DarkTurquoise);

                }
                return darkturquoise;
            }
        }

        public static Pen DarkViolet
        {
            get
            {
                if (darkviolet == null)
                {
                    darkviolet = new Pen(Color.DarkViolet);

                }
                return darkviolet;
            }
        }

        public static Pen DeepPink
        {
            get
            {
                if (deeppink == null)
                {
                    deeppink = new Pen(Color.DeepPink);

                }
                return deeppink;
            }
        }

        public static Pen DeepSkyBlue
        {
            get
            {
                if (deepskyblue == null)
                {
                    deepskyblue = new Pen(Color.DeepSkyBlue);

                }
                return deepskyblue;
            }
        }

        public static Pen DimGray
        {
            get
            {
                if (dimgray == null)
                {
                    dimgray = new Pen(Color.DimGray);

                }
                return dimgray;
            }
        }

        public static Pen DodgerBlue
        {
            get
            {
                if (dodgerblue == null)
                {
                    dodgerblue = new Pen(Color.DodgerBlue);

                }
                return dodgerblue;
            }
        }

        public static Pen Firebrick
        {
            get
            {
                if (firebrick == null)
                {
                    firebrick = new Pen(Color.Firebrick);

                }
                return firebrick;
            }
        }

        public static Pen FloralWhite
        {
            get
            {
                if (floralwhite == null)
                {
                    floralwhite = new Pen(Color.FloralWhite);

                }
                return floralwhite;
            }
        }

        public static Pen ForestGreen
        {
            get
            {
                if (forestgreen == null)
                {
                    forestgreen = new Pen(Color.ForestGreen);

                }
                return forestgreen;
            }
        }

        public static Pen Fuchsia
        {
            get
            {
                if (fuchsia == null)
                {
                    fuchsia = new Pen(Color.Fuchsia);

                }
                return fuchsia;
            }
        }

        public static Pen Gainsboro
        {
            get
            {
                if (gainsboro == null)
                {
                    gainsboro = new Pen(Color.Gainsboro);

                }
                return gainsboro;
            }
        }

        public static Pen GhostWhite
        {
            get
            {
                if (ghostwhite == null)
                {
                    ghostwhite = new Pen(Color.GhostWhite);

                }
                return ghostwhite;
            }
        }

        public static Pen Gold
        {
            get
            {
                if (gold == null)
                {
                    gold = new Pen(Color.Gold);

                }
                return gold;
            }
        }

        public static Pen Goldenrod
        {
            get
            {
                if (goldenrod == null)
                {
                    goldenrod = new Pen(Color.Goldenrod);

                }
                return goldenrod;
            }
        }

        public static Pen Gray
        {
            get
            {
                if (gray == null)
                {
                    gray = new Pen(Color.Gray);

                }
                return gray;
            }
        }

        public static Pen Green
        {
            get
            {
                if (green == null)
                {
                    green = new Pen(Color.Green);

                }
                return green;
            }
        }

        public static Pen GreenYellow
        {
            get
            {
                if (greenyellow == null)
                {
                    greenyellow = new Pen(Color.GreenYellow);

                }
                return greenyellow;
            }
        }

        public static Pen Honeydew
        {
            get
            {
                if (honeydew == null)
                {
                    honeydew = new Pen(Color.Honeydew);

                }
                return honeydew;
            }
        }

        public static Pen HotPink
        {
            get
            {
                if (hotpink == null)
                {
                    hotpink = new Pen(Color.HotPink);

                }
                return hotpink;
            }
        }

        public static Pen IndianRed
        {
            get
            {
                if (indianred == null)
                {
                    indianred = new Pen(Color.IndianRed);

                }
                return indianred;
            }
        }

        public static Pen Indigo
        {
            get
            {
                if (indigo == null)
                {
                    indigo = new Pen(Color.Indigo);

                }
                return indigo;
            }
        }

        public static Pen Ivory
        {
            get
            {
                if (ivory == null)
                {
                    ivory = new Pen(Color.Ivory);

                }
                return ivory;
            }
        }

        public static Pen Khaki
        {
            get
            {
                if (khaki == null)
                {
                    khaki = new Pen(Color.Khaki);

                }
                return khaki;
            }
        }

        public static Pen Lavender
        {
            get
            {
                if (lavender == null)
                {
                    lavender = new Pen(Color.Lavender);

                }
                return lavender;
            }
        }

        public static Pen LavenderBlush
        {
            get
            {
                if (lavenderblush == null)
                {
                    lavenderblush = new Pen(Color.LavenderBlush);

                }
                return lavenderblush;
            }
        }

        public static Pen LawnGreen
        {
            get
            {
                if (lawngreen == null)
                {
                    lawngreen = new Pen(Color.LawnGreen);

                }
                return lawngreen;
            }
        }

        public static Pen LemonChiffon
        {
            get
            {
                if (lemonchiffon == null)
                {
                    lemonchiffon = new Pen(Color.LemonChiffon);

                }
                return lemonchiffon;
            }
        }

        public static Pen LightBlue
        {
            get
            {
                if (lightblue == null)
                {
                    lightblue = new Pen(Color.LightBlue);

                }
                return lightblue;
            }
        }

        public static Pen LightCoral
        {
            get
            {
                if (lightcoral == null)
                {
                    lightcoral = new Pen(Color.LightCoral);

                }
                return lightcoral;
            }
        }

        public static Pen LightCyan
        {
            get
            {
                if (lightcyan == null)
                {
                    lightcyan = new Pen(Color.LightCyan);

                }
                return lightcyan;
            }
        }

        public static Pen LightGoldenrodYellow
        {
            get
            {
                if (lightgoldenrodyellow == null)
                {
                    lightgoldenrodyellow = new Pen(Color.LightGoldenrodYellow);

                }
                return lightgoldenrodyellow;
            }
        }

        public static Pen LightGray
        {
            get
            {
                if (lightgray == null)
                {
                    lightgray = new Pen(Color.LightGray);

                }
                return lightgray;
            }
        }

        public static Pen LightGreen
        {
            get
            {
                if (lightgreen == null)
                {
                    lightgreen = new Pen(Color.LightGreen);

                }
                return lightgreen;
            }
        }

        public static Pen LightPink
        {
            get
            {
                if (lightpink == null)
                {
                    lightpink = new Pen(Color.LightPink);

                }
                return lightpink;
            }
        }

        public static Pen LightSalmon
        {
            get
            {
                if (lightsalmon == null)
                {
                    lightsalmon = new Pen(Color.LightSalmon);

                }
                return lightsalmon;
            }
        }

        public static Pen LightSeaGreen
        {
            get
            {
                if (lightseagreen == null)
                {
                    lightseagreen = new Pen(Color.LightSeaGreen);

                }
                return lightseagreen;
            }
        }

        public static Pen LightSkyBlue
        {
            get
            {
                if (lightskyblue == null)
                {
                    lightskyblue = new Pen(Color.LightSkyBlue);

                }
                return lightskyblue;
            }
        }

        public static Pen LightSlateGray
        {
            get
            {
                if (lightslategray == null)
                {
                    lightslategray = new Pen(Color.LightSlateGray);

                }
                return lightslategray;
            }
        }

        public static Pen LightSteelBlue
        {
            get
            {
                if (lightsteelblue == null)
                {
                    lightsteelblue = new Pen(Color.LightSteelBlue);

                }
                return lightsteelblue;
            }
        }

        public static Pen LightYellow
        {
            get
            {
                if (lightyellow == null)
                {
                    lightyellow = new Pen(Color.LightYellow);

                }
                return lightyellow;
            }
        }

        public static Pen Lime
        {
            get
            {
                if (lime == null)
                {
                    lime = new Pen(Color.Lime);

                }
                return lime;
            }
        }

        public static Pen LimeGreen
        {
            get
            {
                if (limegreen == null)
                {
                    limegreen = new Pen(Color.LimeGreen);

                }
                return limegreen;
            }
        }

        public static Pen Linen
        {
            get
            {
                if (linen == null)
                {
                    linen = new Pen(Color.Linen);

                }
                return linen;
            }
        }

        public static Pen Magenta
        {
            get
            {
                if (magenta == null)
                {
                    magenta = new Pen(Color.Magenta);

                }
                return magenta;
            }
        }

        public static Pen Maroon
        {
            get
            {
                if (maroon == null)
                {
                    maroon = new Pen(Color.Maroon);

                }
                return maroon;
            }
        }

        public static Pen MediumAquamarine
        {
            get
            {
                if (mediumaquamarine == null)
                {
                    mediumaquamarine = new Pen(Color.MediumAquamarine);

                }
                return mediumaquamarine;
            }
        }

        public static Pen MediumBlue
        {
            get
            {
                if (mediumblue == null)
                {
                    mediumblue = new Pen(Color.MediumBlue);

                }
                return mediumblue;
            }
        }

        public static Pen MediumOrchid
        {
            get
            {
                if (mediumorchid == null)
                {
                    mediumorchid = new Pen(Color.MediumOrchid);

                }
                return mediumorchid;
            }
        }

        public static Pen MediumPurple
        {
            get
            {
                if (mediumpurple == null)
                {
                    mediumpurple = new Pen(Color.MediumPurple);

                }
                return mediumpurple;
            }
        }

        public static Pen MediumSeaGreen
        {
            get
            {
                if (mediumseagreen == null)
                {
                    mediumseagreen = new Pen(Color.MediumSeaGreen);

                }
                return mediumseagreen;
            }
        }

        public static Pen MediumSlateBlue
        {
            get
            {
                if (mediumslateblue == null)
                {
                    mediumslateblue = new Pen(Color.MediumSlateBlue);

                }
                return mediumslateblue;
            }
        }

        public static Pen MediumSpringGreen
        {
            get
            {
                if (mediumspringgreen == null)
                {
                    mediumspringgreen = new Pen(Color.MediumSpringGreen);

                }
                return mediumspringgreen;
            }
        }

        public static Pen MediumTurquoise
        {
            get
            {
                if (mediumturquoise == null)
                {
                    mediumturquoise = new Pen(Color.MediumTurquoise);

                }
                return mediumturquoise;
            }
        }

        public static Pen MediumVioletRed
        {
            get
            {
                if (mediumvioletred == null)
                {
                    mediumvioletred = new Pen(Color.MediumVioletRed);

                }
                return mediumvioletred;
            }
        }

        public static Pen MidnightBlue
        {
            get
            {
                if (midnightblue == null)
                {
                    midnightblue = new Pen(Color.MidnightBlue);

                }
                return midnightblue;
            }
        }

        public static Pen MintCream
        {
            get
            {
                if (mintcream == null)
                {
                    mintcream = new Pen(Color.MintCream);

                }
                return mintcream;
            }
        }

        public static Pen MistyRose
        {
            get
            {
                if (mistyrose == null)
                {
                    mistyrose = new Pen(Color.MistyRose);

                }
                return mistyrose;
            }
        }

        public static Pen Moccasin
        {
            get
            {
                if (moccasin == null)
                {
                    moccasin = new Pen(Color.Moccasin);

                }
                return moccasin;
            }
        }

        public static Pen NavajoWhite
        {
            get
            {
                if (navajowhite == null)
                {
                    navajowhite = new Pen(Color.NavajoWhite);

                }
                return navajowhite;
            }
        }

        public static Pen Navy
        {
            get
            {
                if (navy == null)
                {
                    navy = new Pen(Color.Navy);

                }
                return navy;
            }
        }

        public static Pen OldLace
        {
            get
            {
                if (oldlace == null)
                {
                    oldlace = new Pen(Color.OldLace);

                }
                return oldlace;
            }
        }

        public static Pen Olive
        {
            get
            {
                if (olive == null)
                {
                    olive = new Pen(Color.Olive);

                }
                return olive;
            }
        }

        public static Pen OliveDrab
        {
            get
            {
                if (olivedrab == null)
                {
                    olivedrab = new Pen(Color.OliveDrab);

                }
                return olivedrab;
            }
        }

        public static Pen Orange
        {
            get
            {
                if (orange == null)
                {
                    orange = new Pen(Color.Orange);

                }
                return orange;
            }
        }

        public static Pen OrangeRed
        {
            get
            {
                if (orangered == null)
                {
                    orangered = new Pen(Color.OrangeRed);

                }
                return orangered;
            }
        }

        public static Pen Orchid
        {
            get
            {
                if (orchid == null)
                {
                    orchid = new Pen(Color.Orchid);

                }
                return orchid;
            }
        }

        public static Pen PaleGoldenrod
        {
            get
            {
                if (palegoldenrod == null)
                {
                    palegoldenrod = new Pen(Color.PaleGoldenrod);

                }
                return palegoldenrod;
            }
        }

        public static Pen PaleGreen
        {
            get
            {
                if (palegreen == null)
                {
                    palegreen = new Pen(Color.PaleGreen);

                }
                return palegreen;
            }
        }

        public static Pen PaleTurquoise
        {
            get
            {
                if (paleturquoise == null)
                {
                    paleturquoise = new Pen(Color.PaleTurquoise);

                }
                return paleturquoise;
            }
        }

        public static Pen PaleVioletRed
        {
            get
            {
                if (palevioletred == null)
                {
                    palevioletred = new Pen(Color.PaleVioletRed);

                }
                return palevioletred;
            }
        }

        public static Pen PapayaWhip
        {
            get
            {
                if (papayawhip == null)
                {
                    papayawhip = new Pen(Color.PapayaWhip);

                }
                return papayawhip;
            }
        }

        public static Pen PeachPuff
        {
            get
            {
                if (peachpuff == null)
                {
                    peachpuff = new Pen(Color.PeachPuff);

                }
                return peachpuff;
            }
        }

        public static Pen Peru
        {
            get
            {
                if (peru == null)
                {
                    peru = new Pen(Color.Peru);

                }
                return peru;
            }
        }

        public static Pen Pink
        {
            get
            {
                if (pink == null)
                {
                    pink = new Pen(Color.Pink);

                }
                return pink;
            }
        }

        public static Pen Plum
        {
            get
            {
                if (plum == null)
                {
                    plum = new Pen(Color.Plum);

                }
                return plum;
            }
        }

        public static Pen PowderBlue
        {
            get
            {
                if (powderblue == null)
                {
                    powderblue = new Pen(Color.PowderBlue);

                }
                return powderblue;
            }
        }

        public static Pen Purple
        {
            get
            {
                if (purple == null)
                {
                    purple = new Pen(Color.Purple);

                }
                return purple;
            }
        }

        public static Pen Red
        {
            get
            {
                if (red == null)
                {
                    red = new Pen(Color.Red);

                }
                return red;
            }
        }

        public static Pen RosyBrown
        {
            get
            {
                if (rosybrown == null)
                {
                    rosybrown = new Pen(Color.RosyBrown);

                }
                return rosybrown;
            }
        }

        public static Pen RoyalBlue
        {
            get
            {
                if (royalblue == null)
                {
                    royalblue = new Pen(Color.RoyalBlue);

                }
                return royalblue;
            }
        }

        public static Pen SaddleBrown
        {
            get
            {
                if (saddlebrown == null)
                {
                    saddlebrown = new Pen(Color.SaddleBrown);

                }
                return saddlebrown;
            }
        }

        public static Pen Salmon
        {
            get
            {
                if (salmon == null)
                {
                    salmon = new Pen(Color.Salmon);

                }
                return salmon;
            }
        }

        public static Pen SandyBrown
        {
            get
            {
                if (sandybrown == null)
                {
                    sandybrown = new Pen(Color.SandyBrown);

                }
                return sandybrown;
            }
        }

        public static Pen SeaGreen
        {
            get
            {
                if (seagreen == null)
                {
                    seagreen = new Pen(Color.SeaGreen);

                }
                return seagreen;
            }
        }

        public static Pen SeaShell
        {
            get
            {
                if (seashell == null)
                {
                    seashell = new Pen(Color.SeaShell);

                }
                return seashell;
            }
        }

        public static Pen Sienna
        {
            get
            {
                if (sienna == null)
                {
                    sienna = new Pen(Color.Sienna);

                }
                return sienna;
            }
        }

        public static Pen Silver
        {
            get
            {
                if (silver == null)
                {
                    silver = new Pen(Color.Silver);

                }
                return silver;
            }
        }

        public static Pen SkyBlue
        {
            get
            {
                if (skyblue == null)
                {
                    skyblue = new Pen(Color.SkyBlue);

                }
                return skyblue;
            }
        }

        public static Pen SlateBlue
        {
            get
            {
                if (slateblue == null)
                {
                    slateblue = new Pen(Color.SlateBlue);

                }
                return slateblue;
            }
        }

        public static Pen SlateGray
        {
            get
            {
                if (slategray == null)
                {
                    slategray = new Pen(Color.SlateGray);

                }
                return slategray;
            }
        }

        public static Pen Snow
        {
            get
            {
                if (snow == null)
                {
                    snow = new Pen(Color.Snow);

                }
                return snow;
            }
        }

        public static Pen SpringGreen
        {
            get
            {
                if (springgreen == null)
                {
                    springgreen = new Pen(Color.SpringGreen);

                }
                return springgreen;
            }
        }

        public static Pen SteelBlue
        {
            get
            {
                if (steelblue == null)
                {
                    steelblue = new Pen(Color.SteelBlue);

                }
                return steelblue;
            }
        }

        public static Pen Tan
        {
            get
            {
                if (tan == null)
                {
                    tan = new Pen(Color.Tan);

                }
                return tan;
            }
        }

        public static Pen Teal
        {
            get
            {
                if (teal == null)
                {
                    teal = new Pen(Color.Teal);

                }
                return teal;
            }
        }

        public static Pen Thistle
        {
            get
            {
                if (thistle == null)
                {
                    thistle = new Pen(Color.Thistle);

                }
                return thistle;
            }
        }

        public static Pen Tomato
        {
            get
            {
                if (tomato == null)
                {
                    tomato = new Pen(Color.Tomato);

                }
                return tomato;
            }
        }

        public static Pen Transparent
        {
            get
            {
                if (transparent == null)
                {
                    transparent = new Pen(Color.Transparent);

                }
                return transparent;
            }
        }

        public static Pen Turquoise
        {
            get
            {
                if (turquoise == null)
                {
                    turquoise = new Pen(Color.Turquoise);

                }
                return turquoise;
            }
        }

        public static Pen Violet
        {
            get
            {
                if (violet == null)
                {
                    violet = new Pen(Color.Violet);

                }
                return violet;
            }
        }

        public static Pen Wheat
        {
            get
            {
                if (wheat == null)
                {
                    wheat = new Pen(Color.Wheat);

                }
                return wheat;
            }
        }

        public static Pen White
        {
            get
            {
                if (white == null)
                {
                    white = new Pen(Color.White);

                }
                return white;
            }
        }

        public static Pen WhiteSmoke
        {
            get
            {
                if (whitesmoke == null)
                {
                    whitesmoke = new Pen(Color.WhiteSmoke);

                }
                return whitesmoke;
            }
        }

        public static Pen Yellow
        {
            get
            {
                if (yellow == null)
                {
                    yellow = new Pen(Color.Yellow);

                }
                return yellow;
            }
        }

        public static Pen YellowGreen
        {
            get
            {
                if (yellowgreen == null)
                {
                    yellowgreen = new Pen(Color.YellowGreen);

                }
                return yellowgreen;

            }
        }
    }
}
