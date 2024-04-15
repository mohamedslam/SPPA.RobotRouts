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
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
	public class StiPageV1Builder : StiContainerV1Builder
	{
		#region Methods.Helpers
		internal void ProcessPageAfterRender(StiPage masterPage)
		{
			if (!masterPage.PageInfoV1.Processed)return;

			masterPage.PageInfoV1.Overlays = null;

			#region UnPreparing after rendering multicolumn page
			if (masterPage.Columns > 1)
			{
				#region Remove container
				foreach(StiComponent component in masterPage.Components)
				{
					StiContainer container = component as StiContainer;
					if (container != null && container.ContainerInfoV1.CurrentColumn > 0)
					{
						masterPage.Components.AddRange(container.Components);
						masterPage.Components.Remove(container);
						masterPage.Components.SetParent(masterPage);
						break;
					}
				}
				#endregion

				#region Remove Clones
				for (int columnIndex = 0; columnIndex < masterPage.Columns - 1; columnIndex++)
				{
					foreach (StiComponent component in masterPage.Components)
					{
						StiClone clone = component as StiClone;
						if (clone != null && clone.ContainerInfoV1.CurrentColumn > 0)
						{
							masterPage.Components.Remove(clone);
							break;
						}
					}
				}			
				#endregion
			}
			#endregion

			masterPage.PageInfoV1.Processed = false;
		}

		internal void ProcessPageBeforeRender(StiPage masterPage)
		{
			if (masterPage.PageInfoV1.Processed)return;

			#region Prepare Overlaybands
			foreach (StiComponent comp in masterPage.Components)
			{
				StiOverlayBand overlayBand = comp as StiOverlayBand;
				if (overlayBand != null)
				{
					if (masterPage.PageInfoV1.Overlays == null)masterPage.PageInfoV1.Overlays = new StiComponentsCollection(null);
					masterPage.PageInfoV1.Overlays.Add(overlayBand);
				}
			}
			#endregion

			#region Preparing for render multicolumn page
			if (masterPage.Columns > 1)
			{
				#region Adds container
				StiContainer container = new StiContainer(new RectangleD(0, 0, 0, 0));
				container.ContainerInfoV1.CurrentColumn = 1;
				container.Name = "#Column#1";
				container.ContainerInfoV1.IsColumn = true;
				container.DockStyle = masterPage.RightToLeft ? StiDockStyle.Right : StiDockStyle.Left;
				masterPage.Components.Add(container);
				container.Width = masterPage.GetColumnWidth() + masterPage.ColumnGaps;
				//container.BeforePrint += new EventHandler(OnColumnBeginRender);
				//container.AfterPrint += new EventHandler(OnColumnEndRender);

				int index = 0;
				while (index < masterPage.Components.Count)
				{				
					StiComponent component = masterPage.Components[index];

					if (container != component)
					{
						if (component.CanContainIn(container))
						{
							container.Components.Add(component);
							masterPage.Components.Remove(component);
							component.Parent = container;
						}
						else index++;

					}
					else index++;
				}
				#endregion

				#region Clones
				for (int columnIndex = 0; columnIndex < masterPage.Columns - 1; columnIndex ++)
				{
					StiClone clone = new StiClone(new RectangleD(0, 0, masterPage.GetColumnWidth() + masterPage.ColumnGaps, 0));
					clone.ContainerInfoV1.IsColumn = true;
					clone.ContainerInfoV1.CurrentColumn = 2 + columnIndex;
					clone.Name = "#Column#" + (2 + columnIndex).ToString();
					clone.DockStyle = masterPage.RightToLeft ? StiDockStyle.Right : StiDockStyle.Left;
					clone.Container = container;
					//clone.BeforePrint += new EventHandler(OnColumnBeginRender);
					//clone.AfterPrint += new EventHandler(OnColumnEndRender);
					
					masterPage.Components.Add(clone);
				}
				#endregion
			}
			#endregion

			masterPage.PageInfoV1.Processed = true;
		}
		#endregion

		#region Methods.Render
		/// <summary>
		/// Prepares a component for rendering.
		/// </summary>
		public override void Prepare(StiComponent masterComp)
		{
			StiPage masterPage = masterComp as StiPage;

			foreach (StiComponent comp in masterPage.Components)
			{
				StiDataBand dataBand = comp as StiDataBand;
				if (dataBand != null)
				{
					StiDataBandV1Builder builder = GetBuilder(typeof(StiDataBand)) as StiDataBandV1Builder;
					builder.SetDataIsPrepared(dataBand, false);
					dataBand.LineThrough = 1;
				}
			}

			base.Prepare(masterComp);
		}

		/// <summary>
		/// Clears a component after rendering.
		/// </summary>
		public override void UnPrepare(StiComponent masterComp)
		{
			base.UnPrepare(masterComp);

			StiPage masterPage = masterComp as StiPage;
			masterPage.PageInfoV1.Overlays = null;
		}

		/// <summary>
		/// Renders a component in the specified container without taking generation of BeforePrintEvent and AfterPrintEvent events into consideration and without taking Conditions into consideration.
		/// Rendered component is returned in the renderedComponent.
		/// </summary>
		/// <param name="renderedComponent">A rendered component.</param>
		/// <param name="outContainer">A panel in which rendering will be done.</param>
		/// <returns>Is rendering finished or not.</returns>
		public override bool InternalRender(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
		{
			StiPage masterPage = masterComp as StiPage;

			masterPage.PageInfoV1.IsFirstDataBandOnPage = true;

			#region Render overlays
			if (masterPage.PageInfoV1.Overlays != null)
			{
				ArrayList overlayTop = new ArrayList();
				ArrayList overlayCenter = new ArrayList();
				ArrayList overlayBottom = new ArrayList();

				foreach (StiOverlayBand overlay in masterPage.PageInfoV1.Overlays)
				{
					overlay.Render(ref renderedComponent, outContainer);
					renderedComponent.DockStyle = StiDockStyle.None;					

					if (overlay.VertAlignment == StiVertAlignment.Top)overlayTop.Add(renderedComponent);
					else if (overlay.VertAlignment == StiVertAlignment.Center)overlayCenter.Add(renderedComponent);
					else if (overlay.VertAlignment == StiVertAlignment.Bottom)overlayBottom.Add(renderedComponent);

					renderedComponent = null;
				}

				double top = - masterPage.Margins.Top;
				foreach (StiComponent comp in overlayTop)
				{
					comp.Top = top;
					top += comp.Height;
				}

				double bottom = masterPage.Height + masterPage.Margins.Bottom;
				foreach (StiComponent comp in overlayBottom)
				{
					comp.Top = bottom - comp.Height;
					bottom -= comp.Height;
				}

				double height = 0;
				foreach (StiComponent comp in overlayCenter)
				{
					height += comp.Height;
				}

				top = (masterPage.Height - height) / 2;
				foreach (StiComponent comp in overlayCenter)
				{
					comp.Top = top;
					top += comp.Height;
				}
			}
			#endregion			

			masterPage.PageInfoV1.DelimiterComponentsLeft.Clear();

			renderedComponent = outContainer;
			
			masterPage.InvokeEvents();
			var renderResult = base.RenderContainer(masterPage, ref renderedComponent, outContainer);

			#region Copy Values to rendered page
			if (outContainer != null)
			{				
				outContainer.PointerValue = masterPage.PointerValue;
				outContainer.BookmarkValue = masterPage.BookmarkValue;
				outContainer.TagValue = masterPage.TagValue;
				outContainer.ToolTipValue = masterPage.ToolTipValue;
				outContainer.HyperlinkValue = masterPage.HyperlinkValue;
				
				if (outContainer is StiPage)
					((StiPage)outContainer).ExcelSheetValue = masterPage.ExcelSheetValue;
			}

			masterPage.PointerValue = null;
			masterPage.BookmarkValue = null;
			masterPage.TagValue = null;
			masterPage.ToolTipValue = null;
			masterPage.HyperlinkValue = null;
			#endregion

			return renderResult;
		}	
		#endregion
	}
}
