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
using System.Collections.Generic;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
	public class StiSubReportV1Builder : StiContainerV1Builder
	{
		#region Methods.Render
		/// <summary>
		/// Prepares a component for rendering.
		/// </summary>
		public override void Prepare(StiComponent masterComp)
		{
			base.Prepare(masterComp);

			StiSubReport masterSubReport = masterComp as StiSubReport;

			StiPage page = masterSubReport.SubReportPage;
			if (page != null)
			{
				page.ProcessPageBeforeRender();
				page.Prepare();				
			}
		}

		/// <summary>
		/// Renders a component in the specified container without taking generation of BeforePrintEvent and AfterPrintEvent events into consideration and without taking Conditions into consideration.
		/// The rendered component is returned in the renderedComponent.
		/// </summary>
		/// <param name="renderedComponent">A rendered component.</param>
		/// <param name="outContainer">A panel in what rendering will be done.</param>
		/// <returns>Is rendering finished or not.</returns>
		public override bool InternalRender(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
		{
			StiSubReport masterSubReport = masterComp as StiSubReport;

			double maxHeight = 0;
			foreach (StiComponent comp2 in outContainer.Components)
			{
				if ((comp2.Left > masterSubReport.Left && comp2.Left < masterSubReport.Right) ||
					(comp2.Right > masterSubReport.Left && comp2.Right < masterSubReport.Right) ||
					(masterSubReport.Left > comp2.Left && masterSubReport.Left < comp2.Right) ||
					(masterSubReport.Right > comp2.Left && masterSubReport.Right < comp2.Right))
				{
					maxHeight = Math.Max(comp2.Bottom, maxHeight);
				}
			}

            StiReport externalReport = masterSubReport.GetExternalSubReport();

            StiFillParametersEventArgs e = new StiFillParametersEventArgs();
            masterSubReport.InvokeFillParameters(masterSubReport, e);
            if (e.Value != null && e.Value.Count > 0)
            {
                StiReport report = externalReport ?? masterSubReport.Report;
                foreach (KeyValuePair<string, object> entry in e.Value)
                {
                    report[entry.Key] = entry.Value;
                }
            }

			StiDataBand dataBand = null;
			double oldHeight = 0;
            if (externalReport == null)
			{
				base.InternalRender(masterSubReport, ref renderedComponent, outContainer);

				if (masterSubReport.Top < maxHeight)renderedComponent.Top = maxHeight;
			
				oldHeight = outContainer.Height;

				dataBand = masterSubReport.Parent as StiDataBand;
				if (dataBand != null)
				{
					renderedComponent.Height = dataBand.DataBandInfoV1.FreeSpace - renderedComponent.Top;
                    if (renderedComponent.Height > dataBand.DataBandInfoV1.FreeSpace - renderedComponent.Top) renderedComponent.Height -= 0.01;
				}
			}

			StiComponent comp = null;
			bool result = true;
            if ((masterSubReport.SubReportPage != null && renderedComponent != null) || externalReport != null)
			{					
				#region Use External Report
                if (externalReport != null)
                {
                    if (!externalReport.IsDocument)
                    {
                        foreach (StiPage page in externalReport.Pages)
                        {
                            page.UnlimitedHeight = true;
                        }
                        externalReport.Render(false);
                    }

                    double posX = masterSubReport.Left;
                    double posY = masterSubReport.Top;
                    foreach (StiPage page in externalReport.RenderedPages)
                    {
                        outContainer.Components.AddRange(page.Components);
                        foreach (StiComponent comp2 in page.Components)
                        {
                            comp2.Left += posX;
                            comp2.Top += posY;
                        }
                        page.Components.Clear();
                        posY += page.Height;
                    }

                    outContainer.Components.SetParent(renderedComponent as StiContainer);
                }
                #endregion

                #region Use Internal Page
                else if (masterSubReport.SubReportPage != null)
                {
                    masterSubReport.SubReportPage.ParentBookmark = masterSubReport.CurrentBookmark;
                    result = masterSubReport.SubReportPage.Render(ref comp, renderedComponent as StiContainer);

                    #region If SubReportPage have more then one column, then remove from SubReportPage column Container and column Clones.
                    if (masterSubReport.SubReportPage.Columns > 1)
                    {
                        StiComponentsCollection columnComps = ((StiContainer)renderedComponent).Components;
                        foreach (StiComponent columnComp in columnComps)
                        {
                            if (columnComp.Name.StartsWith("#Column#", StringComparison.InvariantCulture))
                            {
                                columnComp.DockStyle = StiDockStyle.None;
                                double maxHeight2 = 0;
                                StiComponentsCollection columnComps2 = ((StiContainer)columnComp).Components;
                                foreach (StiComponent comp2 in columnComps2)
                                {
                                    maxHeight = Math.Max(maxHeight2, comp2.Bottom);
                                }

                                columnComp.Height = maxHeight2;
                            }
                        }
                    }
                    #endregion
                }
				#endregion
			
				if (dataBand != null)
				{
					renderedComponent.CanShrink = true;
					SizeD newSize = renderedComponent.GetActualSize();
					if (newSize.Height != 0)
					{
						renderedComponent.Height = newSize.Height;
						outContainer.Height = Math.Max(renderedComponent.Bottom, outContainer.Height);
					}
					else
					{
						renderedComponent.Height = outContainer.Height = oldHeight;
					}
				}
				masterSubReport.SubReportInfoV1.SubReportRenderResult = result;
			}
			return result;
		}
		#endregion
	}
}
