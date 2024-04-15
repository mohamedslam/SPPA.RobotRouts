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

namespace Stimulsoft.Base.Design
{	
	internal sealed class StiCategoryIndexHelper
	{
		public static int GetCategoryIndex(string name)
		{
			switch (name)
			{
                case "ValueOpen":
                    return 24;
                                    
                case "Cell":
                case "Value":
                case "ValueEnd":                
                case "ValueClose":
                case "ValueHigh":
                case "ValueLow":
                    return 25;

                case "Argument":
                    return 50;

                case "Weight":
                    return 75;

				case "BarCode":
				case "Button":
				case "Cards":
                case "Chart":
				case "Check":
				case "CheckBox":
				case "CrossTab":
                case "ComboBox":
                case "Dashboard":
                case "DatePicker":
                case "Description":
                case "Gauge":
                case "Hierarchical":
                case "Image":
				case "Signature":
				case "Indicator":
                case "ListBox":
                case "Main":
                case "Map":
				case "MathFormula":
				case "OnlineMap":
                case "Page":
                case "PivotTable":
                case "Primitive":
                case "Progress":
                case "RegionMap":
                case "Scale":
                case "Shape":
                case "SubReport":
                case "Text":
                case "Tick":
                case "TreeView":
                case "TreeViewBox":
                case "WinControl":
                case "ZipCode":
				case "TableOfContents":
					return 100;

                case "Data":
                    return 200;

			    case "ChartAdditional":
                case "BarCodeAdditional":
			    case "ImageAdditional":
                case "TextAdditional":
				case "PageAdditional":
                case "OnlineMapAdditional":
                    return 190;				

                case "Table":
                    return 260;

                case "HeaderTable":
                    return 261;

                case "FooterTable":
                    return 262;

				case "PageColumnBreak":				
					return 280;

				case "Position":
				case "Columns":
					return 300;

				case "Appearance":
					return 400;

				case "Behavior":
					return 450;

				case "Design":
					return 460;

				case "Navigation":
					return 470;

				case "Export":
					return 500;							
				
				case "Colors":
					return 500;

				case "Control":
					return 500;

				case "ControlsEvents":
					return 500;

				case "Display":
					return 500;
				
				case "ExportEvents":
					return 500;
				
				case "Marker":
					return 500;

				case "Misc":
					return 500;

				case "Options":
					return 500;

				case "Parameters":
					return 500;

				case "ReportStop":
					return 500;				

				case "Globalization":
					return 600;

				case "Script":
					return 700;

				case "MouseEvents":
				case "NavigationEvents":
				case "PrintEvents":
				case "RenderEvents":
				case "ValueEvents":		
					return 0;
			}

			return int.MaxValue;
		}
	}
}
