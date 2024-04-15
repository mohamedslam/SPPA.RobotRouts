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
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
	public class StiSimpleTextV1Builder : StiComponentV1Builder
	{
		#region Methods.Render
		/// <summary>
		/// Renders a component in the specified container without taking generation of BeforePrintEvent and AfterPrintEvent events into consideration and without taking Conditions into consideration.
		/// The rendered component is returned in the renderedComponent.
		/// </summary>
		/// <param name="renderedComponent">A rendered component.</param>
		/// <param name="outContainer">A panel in what rendering will be done.</param>
		/// <returns>Is rendering finished or not.</returns>
		public override bool InternalRender(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
		{
			var masterSimpleText = masterComp as StiSimpleText;
			var textBox = (StiSimpleText)masterSimpleText.Clone(false);
			//textBox.Conditions = null;

			
			var args = new StiGetValueEventArgs();
			
			#region If no TextValue, get line
			if (string.IsNullOrEmpty(masterSimpleText.TextValue))
			{
				masterSimpleText.InvokeGetValue(textBox, args);

				textBox.InvokeEvents();

                masterSimpleText.CheckDuplicates(textBox, args);

                StiValueEventArgs a = new StiValueEventArgs(args.Value);
				textBox.InvokeTextProcess(textBox, a);

				textBox.SetTextInternal(masterSimpleText.ProcessText(masterSimpleText.GetTextWithoutZero(a.Value as string)));

				masterSimpleText.InvokeRenderTo(textBox);
			}
			#endregion

			else
			{
				textBox.InvokeEvents();

				var a = new StiValueEventArgs(masterSimpleText.TextValue);
				textBox.InvokeTextProcess(textBox, a);

				textBox.SetTextInternal(masterSimpleText.GetTextWithoutZero(a.Value as string));
			}			
		
			outContainer.Components.Add(textBox);
			
			#region If StoreToPrinted
			if (args.StoreToPrinted)
			{
				var list = masterSimpleText.Report.Totals[masterSimpleText.Name] as ArrayList;
				if (list == null)
				{
					list = new ArrayList();
					masterSimpleText.Report.Totals[masterSimpleText.Name] = list;
				}
				var runtimeVariables = new StiRuntimeVariables(masterSimpleText.Report);
				runtimeVariables.PageIndex = masterSimpleText.Report.RenderedPages.Count;
				runtimeVariables.TextBox = textBox;
				list.Add(runtimeVariables);

                masterSimpleText.Report.Totals["#%#" + masterSimpleText.Name] = masterSimpleText.Text.Value;
            }
			#endregion

			#region ExcelValue
			if (masterSimpleText is StiText)
			{
				if (string.IsNullOrEmpty(((StiText)masterSimpleText).ExcelDataValue) || 
					((StiText)masterSimpleText).ExcelDataValue == "-")
				{
					StiGetExcelValueEventArgs ee = new StiGetExcelValueEventArgs();
					((StiText)masterSimpleText).InvokeGetExcelValue(textBox, ee);

					if (ee.StoreToPrinted)
					{
						ArrayList list = masterSimpleText.Report.Totals[masterSimpleText.Name + "Excel"] as ArrayList;
						if (list == null)
						{
							list = new ArrayList();
							masterSimpleText.Report.Totals[masterSimpleText.Name + "Excel"] = list;
						}
						StiRuntimeVariables runtimeVariables = new StiRuntimeVariables(masterSimpleText.Report);
						runtimeVariables.PageIndex = masterSimpleText.Report.RenderedPages.Count;
						runtimeVariables.TextBox = textBox;
						list.Add(runtimeVariables);
					}
				}
				else
				{
					((StiText)masterSimpleText).ExcelDataValue = ((StiText)masterSimpleText).ExcelDataValue;
				}
			
			}
			#endregion

			renderedComponent = textBox;
			return true;
		}
		#endregion
	}
}
