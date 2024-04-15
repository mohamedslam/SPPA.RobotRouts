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
using Stimulsoft.Report.Engine;
using System;
using System.Collections;

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Summary description for StiContainerHelper.
    /// </summary>
    public class StiContainerHelper
    {
        public const string NotCheckSizeMark = "*NotCheckSizeMark*";

        public static void CheckSize(StiComponent component)
        {
            var cont = component as StiContainer;
            if (cont == null) return;
            if ((cont.TagValue is string) && ((string)cont.TagValue == NotCheckSizeMark)) return;

            Hashtable hashCheckSize = null;
            if (StiOptions.Engine.AllowCacheForGetActualSize)
            {
                var report = component.Report;
                if (report == null && cont?.ContainerInfoV2?.ParentBand != null)
                    report = cont.ContainerInfoV2.ParentBand.Report;

                if (report == null && cont?.Parent?.ContainerInfoV2?.ParentBand != null)
                    report = cont.Parent.ContainerInfoV2.ParentBand.Report;

                if (report?.Engine != null)
                {
                    if (report.Engine.HashCheckSize == null) 
                        report.Engine.HashCheckSize = new Hashtable();

                    if (report.Engine.HashCheckSize.ContainsKey(component)) return;

                    hashCheckSize = report.Engine.HashCheckSize;
                }
            }

            #region Process child
            //Commented for make subreports and panels work
            if (component.Report != null && component.Report.EngineVersion == StiEngineVersion.EngineV1)
            {
                lock (((ICollection)cont.Components).SyncRoot)
                {
                    foreach (StiComponent comp in cont.Components)
                    {
                        if (comp.IsEnabled)
                            CheckSize(comp);
                    }
                }
            }
            #endregion

            var compDist = new Hashtable();
            var compSizes = new Hashtable();

            #region Get size
            var changedHeights = false;
            lock (((ICollection) cont.Components).SyncRoot)
            {
                foreach (StiComponent comp in cont.Components)
                {
                    if (comp.IsEnabled)
                    {
                        SizeD size;
                        if (comp is StiContainer)
                        {
                            var storedHeight = comp.Height;
                            if (StiOptions.Engine.UseCheckSizeForContinuedContainers)
                                CheckSize(comp);

                            var needSecondPass = false;
                            size = (comp as StiContainer).GetActualSize(true, ref needSecondPass);

                            if (needSecondPass)
                            {
                                var oldHeight = comp.Height;

                                //Speed optimization
                                if (comp.Height != size.Height)
                                    comp.Height = size.Height;

                                size = (comp as StiContainer).GetActualSize(false, ref needSecondPass);

                                //Speed optimization
                                if (comp.Height != oldHeight)
                                    comp.Height = oldHeight;
                            }

                            if (StiOptions.Engine.UseCheckSizeForContinuedContainers && comp.Height != storedHeight)
                                comp.Height = storedHeight;
                        }
                        else
                        {
                            size = comp.GetActualSize();
                        }

                        if (!(comp is StiHorizontalLinePrimitive))
                            size.Height = Math.Round(size.Height, 2);

                        size.Width = Math.Round(size.Width, 2);

                        compSizes[comp] = size;

                        if (size.Height != comp.Height)
                            changedHeights = true;
                    }
                    else
                    {
                        compSizes[comp] = new SizeD(0, 0);
                    }
                }
            }
            #endregion

            #region Prepare dist
            if (changedHeights)
            {
                var compsSorted = new StiComponentsCollection();
                if (cont is StiPage)
                {
                    #region Skip CrossLinePrimitives with StartPoint direct on page
                    Hashtable startPoints = new Hashtable();
                    foreach (StiComponent comp in cont.Components)
                    {
                        var sp = comp as StiStartPointPrimitive;
                        if (!string.IsNullOrWhiteSpace(sp?.ReferenceToGuid))
                            startPoints[sp.ReferenceToGuid] = null;
                    }
                    
                    if (startPoints.Count > 0)
                    {
                        foreach (StiComponent comp in cont.Components)
                        {
                            if (!(comp is StiCrossLinePrimitive && startPoints.ContainsKey(comp.Guid)))
                                compsSorted.Add(comp);
                        }
                    }
                    else
                    {
                        compsSorted.AddRange(cont.Components);
                    }
                    #endregion
                }
                else
                {
                    compsSorted.AddRange(cont.Components);
                }
                compsSorted.SortByTopPosition();

                var index = 0;
                lock (((ICollection) compsSorted).SyncRoot)
                {
                    foreach (StiComponent comp in compsSorted)
                    {
                        var dist = ((SizeD) compSizes[comp]).Height - comp.Height;

                        var masterLeft = Math.Round((decimal) comp.Left, 2);
                        var masterRight = Math.Round((decimal) comp.Right, 2);
                        var masterBottom = Math.Round((decimal) comp.Bottom, 2);

                        if (dist != 0)
                        {
                            double parentDist = 0;
                            
                            if (compDist[comp] != null) 
                                parentDist = (double) compDist[comp];

                            for (var pos = index + 1; pos < compsSorted.Count; pos++)
                            {
                                var childComp = compsSorted[pos];

                                var childLeft = Math.Round((decimal) childComp.Left, 2);
                                var childTop = Math.Round((decimal) childComp.Top, 2);

                                if ((childComp.ShiftMode & StiShiftMode.OnlyInWidthOfComponent) != 0 && (masterLeft > childLeft || masterRight <= childLeft)) continue;

                                if (masterBottom <= childTop)
                                {
                                    double childDist = 0;
                                    if (compDist[childComp] != null)
                                        childDist = (double) compDist[childComp];

                                    if (dist > 0 && (childComp.ShiftMode & StiShiftMode.IncreasingSize) != 0)
                                        compDist[childComp] = Math.Max(dist + parentDist, childDist);

                                    if (dist < 0 && (childComp.ShiftMode & StiShiftMode.DecreasingSize) != 0)
                                        compDist[childComp] = Math.Min(dist + parentDist, childDist);
                                }
                            }
                        }

                        index++;
                    }
                }
            }
            #endregion

            #region Set new size and new position
            lock (((ICollection) cont.Components).SyncRoot)
            {
                foreach (StiComponent comp in cont.Components)
                {
                    var newSize = (SizeD) compSizes[comp];

                    #region AutoWidth
                    var autoWidth = comp as IStiAutoWidth;
                    if (autoWidth != null)
                    {
                        if (autoWidth.AutoWidth)
                        {
                            var align = StiHorAlignment.Left;

                            #region Check IStiTextHorAlignment
                            var textHorAlignment = autoWidth as IStiTextHorAlignment;
                            if (textHorAlignment != null)
                            {
                                switch (textHorAlignment.HorAlignment)
                                {
                                    case StiTextHorAlignment.Left:
                                        align = StiHorAlignment.Left;
                                        break;

                                    case StiTextHorAlignment.Center:
                                    case StiTextHorAlignment.Width:
                                        align = StiHorAlignment.Center;
                                        break;

                                    case StiTextHorAlignment.Right:
                                        align = StiHorAlignment.Right;
                                        break;
                                }
                            }
                            #endregion

                            #region IStiHorAlignment
                            var horAlignment = autoWidth as IStiHorAlignment;
                            if (horAlignment != null)
                            {
                                switch (horAlignment.HorAlignment)
                                {
                                    case StiHorAlignment.Left:
                                        align = StiHorAlignment.Left;
                                        break;

                                    case StiHorAlignment.Center:
                                        align = StiHorAlignment.Center;
                                        break;

                                    case StiHorAlignment.Right:
                                        align = StiHorAlignment.Right;
                                        break;
                                }
                            }

                            float angle = 0;
                            if (comp is StiText) angle = (comp as StiText).Angle;
                            if (angle == 90 || angle == 270)
                            {
                                var storedTop = comp.Top;
                                switch (align)
                                {
                                    case StiHorAlignment.Left:
                                        var compHeight2 = newSize.Height;
                                        comp.Top = comp.Bottom - compHeight2;
                                        comp.Height = compHeight2;
                                        break;

                                    case StiHorAlignment.Center:
                                        var compHeight = newSize.Height;
                                        comp.Top += (comp.Height - compHeight) / 2;
                                        comp.Height = compHeight;
                                        break;

                                    case StiHorAlignment.Right:
                                        comp.Height = newSize.Height;
                                        break;
                                }

                                if (storedTop >= 0 && comp.Top < 0)
                                {
                                    var tempCompHeight = comp.Height;
                                    comp.Top = 0;
                                    comp.Height = tempCompHeight;
                                }
                            }
                            else
                            {
                                var storedLeft = comp.Left;
                                switch (align)
                                {
                                    case StiHorAlignment.Left:
                                        comp.Width = newSize.Width;
                                        break;

                                    case StiHorAlignment.Center:
                                        var compWidth = newSize.Width;
                                        comp.Left += (comp.Width - compWidth) / 2;
                                        comp.Width = compWidth;
                                        break;

                                    case StiHorAlignment.Right:
                                        var compWidth2 = newSize.Width;
                                        comp.Left = comp.Right - compWidth2;
                                        comp.Width = compWidth2;
                                        break;
                                }

                                if (storedLeft >= 0 && comp.Left < 0)
                                {
                                    var tempCompWidth = comp.Width;
                                    comp.Left = 0;
                                    comp.Width = tempCompWidth;
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion

                    if (comp.CanGrow)
                    {
                        var tt = Math.Max(newSize.Width, comp.Width);

                        if (comp.Width != tt)
                            comp.Width = tt;

                        tt = Math.Max(newSize.Height, comp.Height);

                        if (comp.Height != tt)
                            comp.Height = tt;
                    }

                    if (comp.CanShrink)
                    {
                        if (component.Report != null && component.Report.EngineVersion == StiEngineVersion.EngineV1)
                            comp.Width = Math.Min(newSize.Width, comp.Width);

                        else if (!(comp is StiContainer && ((StiContainer) comp).ParentComponentIsBand))
                            comp.Width = Math.Min(newSize.Width, comp.Width);                        

                        comp.Height = Math.Min(newSize.Height, comp.Height);
                    }

                    #region Procees GrowToHeight for containers (non bands)
                    CheckContainerGrowToHeight(comp, false);
                    #endregion

                    if ((comp.ShiftMode & StiShiftMode.IncreasingSize) == 0 &&
                        (comp.ShiftMode & StiShiftMode.DecreasingSize) == 0) continue;

                    if (compDist[comp] != null)
                        comp.Top += (double) compDist[comp];
                }
            }
            #endregion

            #region Check size of container
            if (cont.Report != null && cont.Report.EngineVersion == StiEngineVersion.EngineV1)
            {
                //Do not change this!!Never change this!!!
                if (cont.ParentComponentIsBand)
                {
                    var needSecondPass = false;
                    var size = cont.GetActualSize(true, ref needSecondPass);

                    cont.Width = size.Width;
                    cont.Height = size.Height;

                    if (needSecondPass)
                    {
                        size = cont.GetActualSize(false, ref needSecondPass);
                        cont.Width = size.Width;
                        cont.Height = size.Height;
                    }

                    //Procees GrowToHeight for bands
                    CheckContainerGrowToHeight(cont, needSecondPass);
                }
            }
            else
            {
                if (ComponentPlacedOnBand(cont) || cont.ParentComponentIsBand)
                {
                    var needSecondPass = false;
                    var size = cont.GetActualSize(true, ref needSecondPass);

                    if (!cont.ParentComponentIsBand || cont.ParentComponentIsCrossBand)
                        cont.Width = size.Width;

                    //Speed optimization
                    if (cont.Height != size.Height)
                        cont.Height = size.Height;

                    if (needSecondPass)
                    {
                        size = cont.GetActualSize(false, ref needSecondPass);

                        if (!cont.ParentComponentIsBand || cont.ParentComponentIsCrossBand)
                            cont.Width = size.Width;

                        //Speed optimization
                        if (cont.Height != size.Height) 
                            cont.Height = size.Height;
                    }

                    //Procees GrowToHeight for bands
                    CheckContainerGrowToHeight(cont, needSecondPass);
                }

                if (cont is StiPage && (cont as StiPage).UnlimitedHeight)
                {
                    var page = cont as StiPage;
                    var needSecondPass = false;
                    var size = cont.GetActualSize(true, ref needSecondPass);
                    while (size.Height > page.Height)
                    {
                        page.SegmentPerHeight++;
                    }
                }
            }
            #endregion

            if (hashCheckSize != null)
                hashCheckSize[component] = null;
        }

        private static bool ComponentPlacedOnBand(StiComponent component)
        {
            var parent = component.Parent;
            while (true)
            {
                if (parent is StiBand) 
                    return true;

                if (parent is StiPage) 
                    return false;

                if (parent == null) 
                    return false;

                if (parent.ParentComponentIsBand) 
                    return true;

                parent = parent.Parent;
            }
        }

        public static void CheckContainerGrowToHeight(StiComponent component, bool force)
        {
            var contGrowToHeight = component as StiContainer;
            if (contGrowToHeight == null) return;
            if ((contGrowToHeight.TagValue is string) && ((string)contGrowToHeight.TagValue == NotCheckSizeMark)) return;

            var comps = contGrowToHeight.Components;
            lock (((ICollection)comps).SyncRoot)
            {
                var count = comps.Count;
                for (int index = 0; index < count; index++)
                {
                    var comp = comps[index];
                    if (comp.GrowToHeight && comp.IsEnabled)
                    {
                        var newHeight = contGrowToHeight.Height - comp.Top;
                        if ((newHeight > comp.Height) || force)
                        {
                            comp.Height = newHeight;
                            CheckContainerGrowToHeight(comp, force);
                        }
                    }
                }
            }
        }
    }
}
