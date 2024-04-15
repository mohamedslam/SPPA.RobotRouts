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
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Maps
{
    public static class StiToolboxDoubleClickHelper
    {
        #region Methods
        public static void Do(StiPage page, StiContainer parent, StiComponent component, RectangleD rect)
        {
            component.ClientRectangle = rect;
            component.Dockable = true;

            if (component is StiBand)
            {
                parent.Components.Add(component);
                parent.Components.SortBandsByLeftPosition();
            }
            else
            {
                var containerRect = new RectangleD(0, 0, parent.ClientRectangle.Width, parent.ClientRectangle.Height);

                var comps = new List<StiComponent>();
                foreach (StiComponent comp in parent.Components)
                {
                    comps.Add(comp);
                }

                #region Check bands
                var pageFooterBands = comps.Where(x => x is StiPageFooterBand).ToArray();
                if (pageFooterBands.Length > 0)
                {
                    var top = containerRect.Bottom;
                    foreach (var band in pageFooterBands)
                    {
                        comps.Remove(band);

                        if (band.ClientRectangle.Top < top)
                            top = band.ClientRectangle.Top;
                    }

                    containerRect.Height = top;
                }

                var bands = comps.Where(x => x is StiBand && !((StiBand)x).IsCross).ToArray();
                var crossBands = comps.Where(x => x is StiBand && ((StiBand)x).IsCross).ToArray();

                if (bands.Length > 0)
                {
                    var top = containerRect.Top;
                    foreach (var band in bands)
                    {
                        comps.Remove(band);

                        if (band.ClientRectangle.Bottom > top)
                            top = band.ClientRectangle.Bottom;
                    }

                    containerRect.Y = top;
                    containerRect.Height -= top;
                }
                #endregion

                bool isFindPosision = false;
                if (comps.Count > 0)
                {
                    double yPos = containerRect.Top;
                    if (containerRect.Top > 0)
                        yPos += page.GridSize;

                    // sort components by top position
                    comps = comps.OrderBy(x => x.Top).ToList();
                    foreach (var comp in comps)
                    {
                        // Top-Left
                        if (CheckRectangle(new RectangleD(comp.Left, comp.Top - page.GridSize - component.Height, component.Width, component.Height), containerRect, comps, comp, component))
                        {
                            isFindPosision = true;
                            break;
                        }

                        // Right-Top
                        if (CheckRectangle(new RectangleD(comp.Right + page.GridSize, comp.Top, component.Width, component.Height), containerRect, comps, comp, component))
                        {
                            isFindPosision = true;
                            break;
                        }
                    }

                    if (!isFindPosision)
                    {
                        foreach (var comp in comps)
                        {
                            // Bottom-Left
                            if (CheckRectangle(new RectangleD(comp.Left, comp.Bottom + page.GridSize, component.Width, component.Height), containerRect, comps, comp, component))
                            {
                                isFindPosision = true;
                                break;
                            }

                            // Right-Bottom
                            if (CheckRectangle(new RectangleD(comp.Right + page.GridSize, comp.Bottom - component.Height, component.Width, component.Height), containerRect, comps, comp, component))
                            {
                                isFindPosision = true;
                                break;
                            }

                            // Left-Top
                            if (CheckRectangle(new RectangleD(comp.Left - page.GridSize - component.Width, comp.Top, component.Width, component.Height), containerRect, comps, comp, component))
                            {
                                isFindPosision = true;
                                break;
                            }

                            // Left-Bottom
                            if (CheckRectangle(new RectangleD(comp.Left - page.GridSize - component.Width, comp.Bottom - component.Height, component.Width, component.Height), containerRect, comps, comp, component))
                            {
                                isFindPosision = true;
                                break;
                            }

                        }
                    }
                }

                if (!isFindPosision)
                {
                    component.Top = (containerRect.Top > 0) 
                        ? containerRect.Top + page.GridSize 
                        : containerRect.Top;
                }

                parent.Components.Add(component);
                parent.Components.SortBandsByTopPosition();
            }

            parent.Page.Correct(true);
            parent.Page.Normalize();

            page.ResetSelection();
            component.Select();
        }

        private static bool CheckRectangle(RectangleD newRect, RectangleD containerRect, List<StiComponent> comps, 
            StiComponent comp, StiComponent newComponent)
        {
            if (newRect.Left >= 0 && newRect.Y >= containerRect.Top && newRect.Right < containerRect.Width &&
                comps.FirstOrDefault(x => x != comp && x.ClientRectangle.IntersectsWith(newRect)) == null)
            {
                newComponent.Left = newRect.Left;
                newComponent.Top = newRect.Top;

                return true;
            }

            return false;
        }
        #endregion
    }
}