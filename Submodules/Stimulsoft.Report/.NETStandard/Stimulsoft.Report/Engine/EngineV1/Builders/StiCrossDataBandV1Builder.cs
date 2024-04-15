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
	public class StiCrossDataBandV1Builder : StiDataBandV1Builder
	{
		#region Methods.Helpers
		/// <summary>
		/// Moves on the next row.
		/// </summary>
		public override void MoveNext(StiDataBand masterDataBand)
		{
			base.MoveNext(masterDataBand);

			StiCrossDataBand masterCrossDataBand = masterDataBand as StiCrossDataBand;

			masterCrossDataBand.SetColumnModeToParent();

			masterCrossDataBand.ColumnCurrent ++;
		}

		public override void SetDataIsPrepared(StiDataBand masterDataBand, bool value)
		{
			StiCrossDataBand crossDataBand = masterDataBand as StiCrossDataBand;

			if (crossDataBand.ColumnMode && crossDataBand.Parent is StiDataBand)
				((StiDataBand)crossDataBand.Parent).DataBandInfoV1.DataIsPrepared = value;
			else
				crossDataBand.DataBandInfoV1.DataIsPrepared = value;
		}

		public override bool GetDataIsPrepared(StiDataBand masterDataBand)
		{
			StiCrossDataBand crossDataBand = masterDataBand as StiCrossDataBand;

			if (crossDataBand.ColumnMode && crossDataBand.Parent is StiDataBand)
				return ((StiDataBand)crossDataBand.Parent).DataBandInfoV1.DataIsPrepared;
			return crossDataBand.DataBandInfoV1.DataIsPrepared;
		}

		public override void SetReportVariables(StiDataBand masterDataBand)
		{
			base.SetReportVariables(masterDataBand);
			
			StiCrossDataBand masterCrossDataBand = masterDataBand as StiCrossDataBand;

			if (masterCrossDataBand.ColumnMode)
			{
				masterCrossDataBand.Report.Line = masterCrossDataBand.Position + 1;
				masterCrossDataBand.Report.Column = masterCrossDataBand.ColumnCurrent + 1;
			}
		}

		private void AddDelimiterComponent(StiDataBand masterDataBand, StiComponent lastComponent, double width, StiContainer outContainer)
		{
		    var cont = new StiText
		    {
		        DelimiterComponent = true,
		        Brush = new StiSolidBrush(Color.Transparent),
		        Width = width
		    };

		    outContainer.Components.Add(cont);
			outContainer.Components.Remove(lastComponent);
			outContainer.Components.Add(lastComponent);
			cont.DockStyle = StiDockStyle.Left;
			DecFreeSpace(masterDataBand, cont);

		    if (masterDataBand.Page.PageInfoV1.DelimiterComponentsLeft[lastComponent.Left] == null)
		        masterDataBand.Page.PageInfoV1.DelimiterComponentsLeft[lastComponent.Left] = cont.Width;
		}

		public override bool CheckFreeSpace(StiDataBand masterDataBand, StiContainer outContainer)
		{
			StiCrossDataBand masterCrossDataBand = masterDataBand as StiCrossDataBand;

			if (masterCrossDataBand.ColumnMode)return false;//fixed column
			if (masterCrossDataBand.Page.UnlimitedWidth)
			{
				StiComponent lastComponent = outContainer.Components[outContainer.Components.Count - 1];
				if (masterCrossDataBand.Page.PageInfoV1.DelimiterComponentsLeft[lastComponent.Left] != null && 
					(!lastComponent.DelimiterComponent))
				{
					double width = (double)masterCrossDataBand.Page.PageInfoV1.DelimiterComponentsLeft[lastComponent.Left];
					AddDelimiterComponent(masterCrossDataBand, lastComponent, width, outContainer);

					return false;
				}

			    if (masterCrossDataBand.DataBandInfoV1.FreeSpace < 0)
			    {
			        if (masterCrossDataBand.Page.UnlimitedBreakable)
			            AddDelimiterComponent(masterCrossDataBand, lastComponent, lastComponent.Width + masterCrossDataBand.DataBandInfoV1.FreeSpace, outContainer);

			        DecFreeSpace(masterCrossDataBand, outContainer);
			        outContainer.AddSize(true);
			        IncFreeSpace(masterCrossDataBand, outContainer);
			        return false;
			    }
			    return false;
			}

		    return CheckFreeSpace(masterCrossDataBand, outContainer);

		}
		#endregion

		#region Methods.Helpers.FreeSpace
		public override void IncFreeSpace(StiDataBand masterDataBand, StiComponent component)
		{
			masterDataBand.DataBandInfoV1.FreeSpace += component.Width;
		}

		public override void DecFreeSpace(StiDataBand masterDataBand, StiComponent component)
		{
			masterDataBand.DataBandInfoV1.FreeSpace -= component.Width;
		}

		public override double GetFreeSpaceFromRectangle(RectangleD rect)
		{
			return rect.Width;
		}

		public override double GetFreeSpaceFromComponent(StiComponent component)
		{
			return component.Width;
		}
		#endregion

		#region Methods.Render
		/// <summary>
		/// Prepares a component for rendering.
		/// </summary>
		public override void Prepare(StiComponent masterComp)
		{
			base.Prepare(masterComp);

			StiCrossDataBand masterCrossDataBand = masterComp as StiCrossDataBand;

			masterCrossDataBand.ColumnCurrent = 0;
		}

		/// <summary>
		/// Renders a component in the specified container without taking generation of BeforePrintEvent and AfterPrintEvent into consideration and without taking 
		/// Conditions into consideration. A rendered component is returned in the renderedComponent argument.
		/// </summary>
		/// <param name="renderedComponent">A rendered component.</param>
		/// <param name="outContainer">A panel in what rendering will be done.</param>
		/// <returns>Is rendering finished or not.</returns>
		public override bool InternalRender(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
		{
			StiCrossDataBand masterCrossDataBand = masterComp as StiCrossDataBand;

			masterCrossDataBand.GetColumnModeFromParent();
			bool result;
			do
			{
				int startIndex = outContainer.Components.Count;

				result = base.InternalRender(masterCrossDataBand, ref renderedComponent, outContainer);
				if (masterCrossDataBand.IsRightToLeft)
				{
					for (int index = startIndex; index < outContainer.Components.Count; index++)
					{
						StiContainer cont = outContainer.Components[index] as StiContainer;
						if (cont != null)
						    cont.OffsetLocation(masterCrossDataBand.ColumnGaps, 0);
					}
				}
			}
			while (!result && masterCrossDataBand.Page.UnlimitedWidth);
			
			return true;
		}
		#endregion
	}
}
