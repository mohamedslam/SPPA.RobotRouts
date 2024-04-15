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

using System;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Chart;

namespace Stimulsoft.Report.Engine
{
	public class StiChartV2Builder : StiComponentV2Builder
	{
		#region Methods.Helpers
		public static void RenderAtEnd(StiChart masterChart)
		{
			if (masterChart.ChartInfoV2.StoredForProcessAtEndChart != null) RenderChart(masterChart);
			masterChart.ChartInfoV2.StoredForProcessAtEndChart = null; 
		}


		public static StiComponent RenderChart(StiChart masterChart)
		{
			StiChart chartComp;

		    if (masterChart.ChartInfoV2.StoredForProcessAtEndChart != null)
		        chartComp = masterChart.ChartInfoV2.StoredForProcessAtEndChart;
		    else
		    {
		        chartComp = (StiChart) masterChart.Clone();

		        if (masterChart.ProcessAtEnd)
		            return chartComp;
		    }

		    if (!StiOptions.Engine.DontSaveDataSourceBeforeChartRendering)
			    masterChart.SaveState("ChartRender");
			
            if (masterChart.MasterComponent == null)
				StiDataHelper.SetData(masterChart, true, masterChart.Parent);

			StiChartHelper.CreateChart(masterChart, chartComp);
            
            if (!StiOptions.Engine.DontSaveDataSourceBeforeChartRendering)
			    masterChart.RestoreState("ChartRender");

			#region Create Metafile
			if (StiOptions.Engine.RenderChartAsBitmap)
			{
				var image = StiChartHelper.GetImageFromChart(chartComp);
                image.ImageRotation = chartComp.Rotation;

                switch (chartComp.Rotation)
                {
                    case StiImageRotation.Rotate90CCW:
                    case StiImageRotation.Rotate90CW:
                        var width = image.Height;
                        image.Height = image.Width;
                        image.Width = width;
                        break;
                }
                return image;
			}

		    return chartComp;
		    #endregion
		}

		#endregion

		#region Methods.Render
		public override void Prepare(StiComponent masterComp)
		{
			base.Prepare(masterComp);

			StiFilterHelper.SetFilter(masterComp);

		    if (masterComp.Height > masterComp.Page.Height || masterComp.Height > masterComp.Parent.Height)
		        masterComp.Height = Math.Min(masterComp.Page.Height, masterComp.Parent.Height);
		}

		public override StiComponent InternalRender(StiComponent masterComp)
		{
			var masterChart = masterComp as StiChart;
			var component = RenderChart(masterChart);
            var childChart = component as StiChart;

		    if (masterChart.ProcessAtEnd)
			    masterChart.ChartInfoV2.StoredForProcessAtEndChart = component as StiChart;

            StiChartInteractionHelper.ProcessChart(masterChart, childChart);

            return component;
		}
		#endregion
	}
}
