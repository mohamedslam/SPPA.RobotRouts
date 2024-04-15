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

using System.Drawing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
	public class StiBandV1Builder : StiContainerV1Builder
	{
		#region Methods.Helper
		/// <summary>
		/// Returns child bands.
		/// </summary>
		public static StiComponentsCollection GetChildBands(StiBand masterBand)
		{
			StiComponentsCollection comps = new StiComponentsCollection();

			if (masterBand.Parent == null)return comps;
			int index = masterBand.Parent.Components.IndexOf(masterBand) + 1;
			
			while (index < masterBand.Parent.Components.Count)
			{
				if (masterBand.Parent.Components[index] is StiChildBand)comps.Add(masterBand.Parent.Components[index]);
				else break;
				index++;
			}
			return comps;
		}

		/// <summary>
		/// Returns true if specified band can be breaked. This method is EngineV1 used only.
		/// </summary>
		/// <param name="masterBand"></param>
		/// <returns></returns>
		private static bool AllowBreak(StiBand masterBand)
		{
			if (masterBand is StiDataBand ||
				masterBand is StiHeaderBand ||
				masterBand is StiFooterBand ||
				masterBand is StiGroupHeaderBand)return masterBand.CanBreak;

			return false;
		}

		/// <summary>
		/// Render childs bands.
		/// </summary>
		public static bool RenderChilds(StiBand masterBand, StiContainer outContainer)
		{
			bool result = true;
			StiComponentsCollection comps = GetChildBands(masterBand);
			foreach (StiBand comp in comps)
			{
				double pos = outContainer.Height;
				StiComponent renderedComponent = null;
				if (!comp.Render(ref renderedComponent, outContainer))
				{
					result = false;
					break;
				}
				if (renderedComponent != null)
				{
					outContainer.Height += renderedComponent.Height;
					renderedComponent.DockStyle = StiDockStyle.None;
					renderedComponent.Top = pos;

					#region Try break rendered child
					StiContainer renderedContainer = renderedComponent as StiContainer;
					if (renderedContainer != null && AllowBreak(masterBand))
					{
						if (renderedContainer.Border.IsDefault && 
							(renderedContainer.Brush is StiEmptyBrush || 
							(renderedContainer.Brush is StiSolidBrush && 
							((StiSolidBrush)renderedContainer.Brush).Color == Color.Transparent)))
						{
							foreach (StiComponent comp2 in renderedContainer.Components)
							{
								outContainer.Components.Add(comp2);
								comp2.Left += renderedContainer.Left;
								comp2.Top += renderedContainer.Top;
								comp2.Parent = outContainer;
							}
							outContainer.Components.Remove(renderedContainer);
						}
					}
					#endregion
				}
			}
			return result;
		}
		#endregion

		#region Methods.Render
		/// <summary>
		/// Renders a component in the specified container without taking generation of BeforePrintEvent and AfterPrintEvent into consideration and without taking 
		/// Conditions into consideration. A rendered component is returned in the renderedComponent argument.
		/// </summary>
		/// <param name="renderedComponent">Rendered component.</param>
		/// <param name="outContainer">Panel in which rendering will be done.</param>
		/// <returns>Is rendering finished or not.</returns>
		public override bool InternalRender(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
		{
			StiBand masterBand = masterComp as StiBand;

			if (!masterBand.IsCross)
			{
				foreach (StiComponent comp in masterBand.Components)
				{
					if (comp.IsCross)comp.Prepare();
					if (comp is StiContainer && (!(comp is StiBand)))comp.Prepare();
				}
			}

			return base.InternalRender(masterBand, ref renderedComponent, outContainer);
		}
		#endregion
	}
}
