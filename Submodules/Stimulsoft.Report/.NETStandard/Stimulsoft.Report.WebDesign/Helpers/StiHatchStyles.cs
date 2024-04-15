#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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

using System.Collections;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Web
{
    internal class StiHatchStyles
    {
        private static Hashtable Item(string value, string key) 
        {
            Hashtable item = new Hashtable();
            item["key"] = key;
            item["value"] = value;

            return item;
        }

        public static ArrayList GetItems()
        {
            ArrayList items = new ArrayList();
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "BackwardDiagonal"), "3"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "LargeGrid"), "4"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "DarkDownwardDiagonal"), "20"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "DarkHorizontal"), "29"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "DarkUpwardDiagonal"), "21"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "DarkVertical"), "28"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "DashedDownwardDiagonal"), "30"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "DashedHorizontal"), "32"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "DashedUpwardDiagonal"), "31"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "DashedVertical"), "33"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "DiagonalBrick"), "38"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "DiagonalCross"), "5"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Divot"), "42"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "DottedDiamond"), "44"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "DottedGrid"), "43"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "ForwardDiagonal"), "2"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Horizontal"), "0"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "HorizontalBrick"), "39"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "LargeCheckerBoard"), "50"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "LargeConfetti"), "35"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "LightDownwardDiagonal"), "18"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "LightHorizontal"), "25"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "LightUpwardDiagonal"), "19"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "LightVertical"), "24"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "NarrowHorizontal"), "27"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "NarrowVertical"), "26"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "OutlinedDiamond"), "51"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Percent05"), "6"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Percent10"), "7"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Percent20"), "8"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Percent25"), "9"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Percent30"), "10"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Percent40"), "11"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Percent50"), "12"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Percent60"), "13"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Percent70"), "14"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Percent75"), "15"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Percent80"), "16"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Percent90"), "17"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Plaid"), "41"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Shingle"), "45"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "SmallCheckerBoard"), "49"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "SmallConfetti"), "34"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "SmallGrid"), "48"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "SolidDiamond"), "52"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Sphere"), "47"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Trellis"), "46"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Vertical"), "1"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "Weave"), "40"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "WideDownwardDiagonal"), "22"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "WideUpwardDiagonal"), "23"));
                items.Add(Item(StiLocalization.Get("PropertyHatchStyle", "ZigZag"), "36"));

            return items;
        }
    }
}