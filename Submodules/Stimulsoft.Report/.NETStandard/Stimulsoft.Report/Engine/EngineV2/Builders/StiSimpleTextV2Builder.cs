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

using System.Collections;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.CrossTab;
using System.Drawing;

namespace Stimulsoft.Report.Engine
{
	public class StiSimpleTextV2Builder : StiComponentV2Builder
	{
        /// <summary>
        /// Prepares a component for rendering.
        /// </summary>
        public override void Prepare(StiComponent masterComp)
        {
            base.Prepare(masterComp);

            if (StiOptions.Engine.AllowResetValuesAtComponent)
                ((StiSimpleText)masterComp).TextValue = null;
        }

		public override StiComponent InternalRender(StiComponent masterComp)
		{
			var masterSimpleText = masterComp as StiSimpleText;
			var textBox = (StiSimpleText)masterSimpleText.Clone(false);

            #region Conditions
            if (masterSimpleText.Conditions != null && masterSimpleText.Conditions.Count > 0 && textBox is StiText)
            {
                foreach (StiBaseCondition condition in masterSimpleText.Conditions)
                {
                    var indicatorCondition = condition as IStiIndicatorCondition;
                    if (indicatorCondition != null)
                    {
                        StiBrush resBrush = null;
                        if (indicatorCondition is StiColorScaleCondition)
                            resBrush = ((StiText)masterSimpleText).Brush;

                        var indicator = indicatorCondition.CreateIndicator(masterSimpleText as StiText);
                        if (indicator != null)
                        {
                            ((StiText)textBox).Indicator = indicator;
                            break;//Render only one indicator condition
                        }

                        if (((StiText)masterSimpleText).Brush != resBrush)
                        {
                            ((StiText)textBox).Brush = ((StiText)masterSimpleText).Brush;
                            ((StiText)masterSimpleText).Brush = resBrush;
                        }
                    }
                }
            }
            #endregion

            if ((masterComp.Report.CalculationMode == StiCalculationMode.Interpretation) && (masterSimpleText.ProcessAt == StiProcessAt.EndOfPage)) return textBox;

            var args = new StiGetValueEventArgs();
			
			#region If no TextValue, get line
			if (string.IsNullOrEmpty(masterSimpleText.TextValue))
			{
				masterSimpleText.InvokeGetValue(textBox, args);

                if (masterComp is StiRichText && !string.IsNullOrEmpty(args.Value) && StiHyperlinkProcessor.IsServerHyperlink(args.Value))
                {
                    var richTextValue = StiStimulsoftServerResource.GetRichText(masterComp as StiRichText, StiHyperlinkProcessor.GetServerNameFromHyperlink(args.Value));
                    args.Value = StiRichText.PackRtf(richTextValue);
                }

				textBox.InvokeEvents();

                masterSimpleText.CheckDuplicates(textBox, args);

                var a = new StiValueEventArgs(args.Value);
				textBox.InvokeTextProcess(textBox, a);

				var str = args.Value;
                if (StiOptions.Engine.AllowAssignEmptyStringInCondition) //backward compatibility with 2016.2
                {
                    if (textBox.TextValue != null)
                        str = textBox.TextValue;

                    if (textBox.TextValue == "" && args.Value != null && args.Value.StartsWith("#%#") && textBox.CanShrink)
                        str = "#";
                }
                else
                {
                    if (!string.IsNullOrEmpty(textBox.TextValue))
                        str = textBox.TextValue;
                }

				textBox.SetTextInternal(masterSimpleText.ProcessText(masterSimpleText.GetTextWithoutZero(str)));

				masterSimpleText.InvokeRenderTo(textBox);

                if (masterSimpleText is StiCrossField)
                    (masterSimpleText as StiCrossField).DisabledByCondition = !textBox.Enabled;

            }
			#endregion

			else
			{
				textBox.InvokeEvents();

				var a = new StiValueEventArgs(masterSimpleText.TextValue);
				textBox.InvokeTextProcess(textBox, a);

				textBox.SetTextInternal(masterSimpleText.GetTextWithoutZero(a.Value as string));
			}	
			
		
			#region If StoreToPrinted
			if (args.StoreToPrinted)
			{
				var list = masterSimpleText.Report.Totals[masterSimpleText.Name] as ArrayList;
				if (list == null)
				{
					list = new ArrayList();
					masterSimpleText.Report.Totals[masterSimpleText.Name] = list;
				}

			    var runtimeVariables = new StiRuntimeVariables(masterSimpleText.Report)
			    {
			        PageIndex = masterSimpleText.Report.RenderedPages.Count,
			        TextBox = textBox
			    };
			    list.Add(runtimeVariables);

                masterSimpleText.Report.Totals["#%#" + masterSimpleText.Name] = masterSimpleText.Text.Value;

                //check for OverlayBand
                var parentComp = masterComp.Parent;
                while (parentComp != null && !(parentComp is StiPage) && !(parentComp is StiOverlayBand))
                    parentComp = parentComp.Parent;

                if (parentComp != null && parentComp is StiOverlayBand)
                {
                    runtimeVariables.PageIndex++;
                    runtimeVariables.CurrentPrintPage++;
                }
			}
			#endregion

			#region ExcelValue
			if (masterSimpleText is StiText)
			{
				if (string.IsNullOrEmpty(((StiText)masterSimpleText).ExcelDataValue) || 
					((StiText)masterSimpleText).ExcelDataValue == "-")
				{
					var ee = new StiGetExcelValueEventArgs();
					((StiText)masterSimpleText).InvokeGetExcelValue(textBox, ee);

					if (ee.StoreToPrinted)
					{
						var list = masterSimpleText.Report.Totals[$"{masterSimpleText.Name}Excel"] as ArrayList;
						if (list == null)
						{
							list = new ArrayList();
							masterSimpleText.Report.Totals[$"{masterSimpleText.Name}Excel"] = list;
						}

					    var runtimeVariables = new StiRuntimeVariables(masterSimpleText.Report)
					    {
					        PageIndex = masterSimpleText.Report.RenderedPages.Count,
					        TextBox = textBox
					    };
					    list.Add(runtimeVariables);

                        //check for OverlayBand
                        var parentComp = masterComp.Parent;
                        while (parentComp != null && !(parentComp is StiPage) && !(parentComp is StiOverlayBand))
                            parentComp = parentComp.Parent;

                        if (parentComp != null && parentComp is StiOverlayBand)
                        {
                            runtimeVariables.PageIndex++;
                            runtimeVariables.CurrentPrintPage++;
                        }
                    }
				}
				else
				{
					((StiText)masterSimpleText).ExcelDataValue = ((StiText)masterSimpleText).ExcelDataValue;
				}
			
			}
			#endregion

			return textBox;
		}
	}
}
