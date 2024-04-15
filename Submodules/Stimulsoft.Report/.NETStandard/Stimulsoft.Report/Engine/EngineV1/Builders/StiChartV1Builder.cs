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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Chart;

namespace Stimulsoft.Report.Engine
{
	public class StiChartV1Builder : StiComponentV1Builder
	{
		#region Methods.Helpers
		public static void RenderAtEnd(StiChart masterChart)
		{
			if (masterChart.ChartInfoV1.Cloner != null && masterChart.ChartInfoV1.Cloner.Parent != null)
			{
				StiComponent renderedComponent = null;
				RenderChart(masterChart, ref renderedComponent, masterChart.ChartInfoV1.OutContainer);
				masterChart.ChartInfoV1.OutContainer = null;
				masterChart.ChartInfoV1.Cloner.Parent.Components.Add(renderedComponent);
				masterChart.ChartInfoV1.Cloner.Parent.Components.Remove(masterChart.ChartInfoV1.Cloner);
			}
		}
	
		public static void RenderChart(StiChart masterChart, ref StiComponent renderedComponent, StiContainer outContainer)
		{			
			StiChart chartComp = (StiChart)masterChart.Clone();

            if (!StiOptions.Engine.DontSaveDataSourceBeforeChartRendering)
			    masterChart.SaveState("ChartRender");

			if (masterChart.MasterComponent == null)
				StiDataHelper.SetData(masterChart, true, masterChart.Parent as StiComponent);
			StiChartHelper.CreateChart(masterChart, chartComp);
            
            if (!StiOptions.Engine.DontSaveDataSourceBeforeChartRendering)
			    masterChart.RestoreState("ChartRender");			

			#region Create Metafile
			if (StiOptions.Engine.RenderChartAsBitmap)
			{
                StiImage imageComp = StiChartHelper.GetImageFromChart(chartComp);
				outContainer.Components.Add(imageComp);
				renderedComponent = imageComp;
			}
			else
			{
				outContainer.Components.Add(chartComp);
				renderedComponent = chartComp;
			}
			#endregion

            StiChartInteractionHelper.ProcessChart(masterChart, chartComp);
		}
				
		#endregion

		#region Methods.Render
		/// <summary>
		/// Prepares a component for rendering.
		/// </summary>
		public override void Prepare(StiComponent masterComp)
		{
			StiFilterHelper.SetFilter(masterComp);
		}

		/// <summary>
		/// Renders a component in the specified container without taking generation of BeforePrintEvent and AfterPrintEvent into consideration and without taking 
		/// Conditions into consideration. A rendered component is returned in the renderedComponent argument.
		/// </summary>
		/// <param name="renderedComponent">Rendered component.</param>
		/// <param name="outContainer">Panel in which rendering will be done.</param>
		/// <returns>Is rendering finished or not.</returns>
		public override bool InternalRender(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
		{
			StiChart masterChart = masterComp as StiChart;

			if (!masterChart.ProcessAtEnd)RenderChart(masterChart, ref renderedComponent, outContainer);
			else 
			{
				if (masterChart.ChartInfoV1.OutContainer != null)
				{
					RenderAtEnd(masterChart);
				}
				else
				{
					masterChart.ChartInfoV1.OutContainer = outContainer;
					masterChart.ChartInfoV1.Cloner = new StiContainer();
					masterChart.ChartInfoV1.Cloner.Left = masterChart.Left;
					masterChart.ChartInfoV1.Cloner.Top = masterChart.Top;
					masterChart.ChartInfoV1.Cloner.Width = masterChart.Width;
					masterChart.ChartInfoV1.Cloner.Height = masterChart.Height;
					masterChart.ChartInfoV1.Cloner.Border.Side = StiBorderSides.All;
					masterChart.ChartInfoV1.OutContainer.Components.Add(masterChart.ChartInfoV1.Cloner);
				}
			}
			return true;
		}
		#endregion
	}
}
