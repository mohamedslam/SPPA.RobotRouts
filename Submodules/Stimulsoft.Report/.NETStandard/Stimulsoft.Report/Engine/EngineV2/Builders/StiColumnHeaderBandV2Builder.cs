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

using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
	public class StiColumnHeaderBandV2Builder : StiHeaderBandV2Builder
	{
		public override StiComponent InternalRender(StiComponent masterComp)
		{
			var masterColumnHeaderBand = masterComp as StiColumnHeaderBand;

			var dataBand = GetMaster(masterColumnHeaderBand);
            if (dataBand == null || dataBand.Columns < 2) 
                return base.InternalRender(masterColumnHeaderBand);

		    var container = new StiContainer
		    {
		        Name = masterColumnHeaderBand.Name,
		        Width = masterColumnHeaderBand.Width,
		        Height = masterColumnHeaderBand.Height,
		        CanGrow = masterColumnHeaderBand.CanGrow,
		        CanShrink = masterColumnHeaderBand.CanShrink,
		        ParentComponentIsBand = true
		    };

		    var fullColumnWidth = dataBand.GetColumnWidth() + dataBand.ColumnGaps;

			for (var columnIndex = 0; columnIndex < dataBand.Columns; columnIndex++)
			{
				masterColumnHeaderBand.Report.Column = columnIndex + 1;

				if ((dataBand.Position + columnIndex < dataBand.Count && (!masterColumnHeaderBand.PrintIfEmpty)) || masterColumnHeaderBand.PrintIfEmpty)
				{
					var columnContainer = base.InternalRender(masterComp) as StiContainer;                    
					columnContainer.DockStyle = StiDockStyle.None;
					columnContainer.Width = fullColumnWidth;
					container.Components.Add(columnContainer);

					if (dataBand.RightToLeft)
						columnContainer.Left = container.Width - fullColumnWidth * (columnIndex + 1) + dataBand.ColumnGaps;
					else
						columnContainer.Left = fullColumnWidth * columnIndex;

					columnContainer.Top = 0;
				}
				else break;
			}

            var reservedWidth = container.Width;
            StiContainerHelper.CheckSize(container);
		    if (!container.ParentComponentIsCrossBand)
		        container.Width = reservedWidth;

		    return container;
		}
	}
}
