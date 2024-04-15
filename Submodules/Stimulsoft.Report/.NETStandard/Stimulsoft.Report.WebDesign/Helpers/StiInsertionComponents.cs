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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Web
{
    /// <summary>
    /// This class helps with insertion components to current report page.
    /// </summary>
    public class StiInsertionComponents
	{
        public static void InsertGroups(StiPage currentPage, StiGroup group)
		{
			foreach (StiComponent component in group.Components)
			{
				component.Dockable = false;
			}

            group.InsertIntoPage(currentPage);
            Insert(currentPage);
		}

        public static void InsertComponents(StiPage currentPage, StiComponentsCollection comps)
		{
            InsertComponents(currentPage, comps, false);
		}

        public static void InsertComponents(StiPage currentPage, StiComponentsCollection comps, bool alignToGrid)
		{
            currentPage.ResetSelection();
			foreach (StiComponent comp in comps)
			{
				comp.Select();
				comp.Dockable = false;				
			}
            currentPage.Components.AddRange(comps);
            Insert(currentPage, alignToGrid);
		}

        public static void Insert(StiPage currentPage)
        {
            Insert(currentPage, false);
        }

        public static void Insert(StiPage currentPage, bool alignToGrid)
        {
            currentPage.Report.Info.IsComponentsMoving = true;
            PointD pos = new PointD(0, 0); //designer.PageView.GetCurrentCursorPos();

            double minX = 0;
            double minY = 0;

            var comps = currentPage.GetSelectedComponents();

            if (comps.Count > 0)
            {
                minX = comps[0].Left;
                minY = comps[0].Top;
            }

            foreach (StiComponent component in comps)
            {
                if (component.IsSelected)
                {
                    if (alignToGrid)
                    {
                        component.ClientRectangle =
                            component.ClientRectangle.AlignToGrid(
                            currentPage.GridSize,
                            currentPage.Report.Info.AlignToGrid);
                    }

                    minX = Math.Min(component.Left, minX);
                    minY = Math.Min(component.Top, minY);
                }
            }

            foreach (StiComponent component in comps)
            {
                if (component.IsSelected)
                {
                    component.Left -= minX - pos.X;
                    component.Top -= minY - pos.Y;
                }
            }
        }

		private StiInsertionComponents()
		{
		}
	}
}
