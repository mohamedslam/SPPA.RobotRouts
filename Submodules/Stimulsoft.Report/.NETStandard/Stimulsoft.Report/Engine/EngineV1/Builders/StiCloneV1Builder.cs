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
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
	public class StiCloneV1Builder : StiContainerV1Builder
	{
		#region Methods.Helper
		/// <summary>
		/// Set parent as the specified container and a page.
		/// </summary>
		private void SetParent(StiClone masterClone)
		{
			masterClone.CloneInfoV1.ResBottomRenderedFooters = masterClone.ContainerInfoV1.BottomRenderedFooters;
			masterClone.CloneInfoV1.ResBottomRenderedGroupFooters = masterClone.ContainerInfoV1.BottomRenderedGroupFooters;
			masterClone.CloneInfoV1.ResBottomRenderedDataBands = masterClone.ContainerInfoV1.BottomRenderedDataBands;
			masterClone.CloneInfoV1.ResBottomRenderedHeaders = masterClone.ContainerInfoV1.BottomRenderedHeaders;
			masterClone.CloneInfoV1.ResBottomRenderedParentsFooters = masterClone.ContainerInfoV1.BottomRenderedParentsFooters;
			masterClone.CloneInfoV1.ResBottomRenderedParentsGroupFooters = masterClone.ContainerInfoV1.BottomRenderedParentsGroupFooters;
			masterClone.CloneInfoV1.ResBottomRenderedParentsDataBands = masterClone.ContainerInfoV1.BottomRenderedParentsDataBands;
			masterClone.CloneInfoV1.ResBottomRenderedParentsHeaders = masterClone.ContainerInfoV1.BottomRenderedParentsHeaders;
			masterClone.CloneInfoV1.ResLastDataBand = masterClone.ContainerInfoV1.LastDataBand;

			masterClone.ParentBookmark = masterClone.Container.ParentBookmark;
			masterClone.CurrentBookmark = masterClone.Container.CurrentBookmark;
            
			double zoom = masterClone.Width / masterClone.Container.Width;
			
			if (masterClone.ScaleHor)
			{
				masterClone.CloneInfoV1.WidthHor = new Hashtable();
				masterClone.CloneInfoV1.LeftHor = new Hashtable();
			}

			StiComponentsCollection comps = masterClone.Container.GetComponents();
			foreach (StiComponent comp in comps)
			{
				if (comp.Parent == masterClone.Container)comp.Parent = masterClone;
				comp.Page = masterClone.Page;
				if (masterClone.ScaleHor)
				{	
					masterClone.CloneInfoV1.WidthHor[comp] = comp.Width;
					masterClone.CloneInfoV1.LeftHor[comp] = comp.Left;

					comp.Width *= zoom;
					comp.Left *= zoom;
				}
			}
		}

		/// <summary>
		/// Restore parent.
		/// </summary>
		private void RestoreParent(StiClone masterClone)
		{
			masterClone.ContainerInfoV1.BottomRenderedFooters = masterClone.CloneInfoV1.ResBottomRenderedFooters;
			masterClone.ContainerInfoV1.BottomRenderedGroupFooters = masterClone.CloneInfoV1.ResBottomRenderedGroupFooters;
			masterClone.ContainerInfoV1.BottomRenderedDataBands = masterClone.CloneInfoV1.ResBottomRenderedDataBands;
			masterClone.ContainerInfoV1.BottomRenderedHeaders = masterClone.CloneInfoV1.ResBottomRenderedHeaders;
			masterClone.ContainerInfoV1.BottomRenderedParentsFooters = masterClone.CloneInfoV1.ResBottomRenderedParentsFooters;
			masterClone.ContainerInfoV1.BottomRenderedParentsGroupFooters = masterClone.CloneInfoV1.ResBottomRenderedParentsGroupFooters;
			masterClone.ContainerInfoV1.BottomRenderedParentsDataBands = masterClone.CloneInfoV1.ResBottomRenderedParentsDataBands;
			masterClone.ContainerInfoV1.BottomRenderedParentsHeaders = masterClone.CloneInfoV1.ResBottomRenderedParentsHeaders;
			masterClone.ContainerInfoV1.LastDataBand = masterClone.CloneInfoV1.ResLastDataBand;

			StiComponentsCollection comps = masterClone.Container.GetComponents();
			foreach (StiComponent comp in comps)
			{
				if (comp.Parent == masterClone)comp.Parent = masterClone.Container;
				comp.Page = masterClone.Container.Page;
				if (masterClone.ScaleHor)
				{
					comp.Width = (double)masterClone.CloneInfoV1.WidthHor[comp];
					comp.Left = (double)masterClone.CloneInfoV1.LeftHor[comp];
				}
			}
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
			StiClone masterClone = masterComp as StiClone;

			if (masterClone.Container != null)
			{
				SetParent(masterClone);
				
				bool result = base.InternalRender(masterClone, ref renderedComponent, outContainer);

				RestoreParent(masterClone);
				base.CheckBandsAtBottom(masterClone, renderedComponent, outContainer);

				return result;
			}
			return true;
		}
		#endregion
	}
}
