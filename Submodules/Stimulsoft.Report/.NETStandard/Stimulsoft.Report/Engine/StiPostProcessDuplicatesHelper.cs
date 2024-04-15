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
using System.Collections;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helpers;

namespace Stimulsoft.Report.Engine
{
	internal sealed class StiPostProcessDuplicatesHelper
	{
        #region enum TypeOfDuplicates
        internal enum TypeOfDuplicates
        {
            Text,
            Image
        }
        #endregion

        #region class StiMergeComparer
        private class StiMergeComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return ((StiComponent)x).Top.CompareTo(((StiComponent)y).Top);
            }
        }
        #endregion

		#region Methods
	    internal static void PostProcessDuplicates(StiComponentsCollection comps, Hashtable parentCont)
        {
            PostProcessDuplicates(comps, parentCont, TypeOfDuplicates.Text);
            PostProcessDuplicates(comps, parentCont, TypeOfDuplicates.Image);
        }

        internal static void PostProcessDuplicates(StiComponentsCollection comps, Hashtable parentCont, TypeOfDuplicates typeOfDuplicates)
        {
            var widths = new SortedList();

            var requre = false;

            #region Build tree-width-x-y
            var components = new StiComponentsCollection();
            lock (((ICollection) comps).SyncRoot)
            {
                foreach (StiComponent comp in comps)
                {
                    if ((comp is IStiText && ((IStiText) comp).ProcessingDuplicates != StiProcessingDuplicatesType.None && typeOfDuplicates == TypeOfDuplicates.Text) ||
                        (comp is StiImage && ((StiImage) comp).ProcessingDuplicates != StiImageProcessingDuplicatesType.None && typeOfDuplicates == TypeOfDuplicates.Image))
                    {
                        requre = true;
                        components.Add(comp);

                        var coordX = widths[comp.Width] as SortedList;
                        if (coordX == null)
                        {
                            coordX = new SortedList();
                            widths[comp.Width] = coordX;
                        }

                        var coordY = coordX[comp.Left] as ArrayList;
                        if (coordY == null)
                        {
                            coordY = new ArrayList();
                            coordX[comp.Left] = coordY;
                        }

                        coordY.Add(comp);
                    }
                }
            }
            #endregion

            if (!requre) return;

            #region Merge or hide components
            foreach (double wd in widths.Keys)
            {
                var coordX = widths[wd] as SortedList;

                foreach (double cdX in coordX.Keys)
                {
                    var coordY = coordX[cdX] as ArrayList;
                    coordY.Sort(new StiMergeComparer());

                    var index = 0;
                    while (index < coordY.Count)
                    {
                        var component = coordY[index] as StiComponent;
                        var cont = parentCont[component] as StiContainer;

                        var bottom = component.Bottom;

                        var startIndex = index + 1;
                        while (startIndex < coordY.Count)
                        {
                            var componentTo = coordY[startIndex] as StiComponent;
                            var contTo = parentCont[componentTo] as StiContainer;

                            var duplicatesType = StiProcessingDuplicatesType.None;

                            var isGlobal = false;
                            if (typeOfDuplicates == TypeOfDuplicates.Text)
                            {
                                duplicatesType = ((IStiText)component).ProcessingDuplicates;
                                if (duplicatesType == StiProcessingDuplicatesType.BasedOnValueRemoveText)
                                    duplicatesType = StiProcessingDuplicatesType.RemoveText;

                                if (duplicatesType == StiProcessingDuplicatesType.BasedOnValueAndTagHide)
                                    duplicatesType = StiProcessingDuplicatesType.BasedOnTagHide;

                                if (duplicatesType == StiProcessingDuplicatesType.BasedOnValueAndTagMerge)
                                    duplicatesType = StiProcessingDuplicatesType.BasedOnTagMerge;

                                isGlobal =
                                    duplicatesType == StiProcessingDuplicatesType.GlobalHide ||
                                    duplicatesType == StiProcessingDuplicatesType.GlobalMerge ||
                                    duplicatesType == StiProcessingDuplicatesType.GlobalRemoveText ||
                                    duplicatesType == StiProcessingDuplicatesType.GlobalBasedOnValueRemoveText ||
                                    duplicatesType == StiProcessingDuplicatesType.GlobalBasedOnValueAndTagHide ||
                                    duplicatesType == StiProcessingDuplicatesType.GlobalBasedOnValueAndTagMerge;

                                if (duplicatesType == StiProcessingDuplicatesType.GlobalBasedOnValueRemoveText)
                                    duplicatesType = StiProcessingDuplicatesType.BasedOnTagRemoveText;

                                if (duplicatesType == StiProcessingDuplicatesType.GlobalBasedOnValueAndTagHide)
                                    duplicatesType = StiProcessingDuplicatesType.BasedOnTagHide;

                                if (duplicatesType == StiProcessingDuplicatesType.GlobalBasedOnValueAndTagMerge)
                                    duplicatesType = StiProcessingDuplicatesType.BasedOnTagMerge;

                            }
                            else
                            {
                                isGlobal =
                                    ((StiImage)component).ProcessingDuplicates == StiImageProcessingDuplicatesType.GlobalHide ||
                                    ((StiImage)component).ProcessingDuplicates == StiImageProcessingDuplicatesType.GlobalMerge ||
                                    ((StiImage)component).ProcessingDuplicates == StiImageProcessingDuplicatesType.GlobalRemoveImage;
                            }

                            if (componentTo.Name == component.Name || isGlobal)
                            {
                                if (Math.Round(cont.Bottom, 2) == Math.Round(contTo.Top, 2) && (cont.Name == contTo.Name || isGlobal ||
                                    (cont.Name == "Continued" || (cont.Name != null && cont.Name.StartsWith("Continued_")) ||
                                    contTo.Name == "Breaked" || (contTo.Name != null && contTo.Name.StartsWith("Breaked_")))))
                                {
                                    #region Text
                                    if (typeOfDuplicates == TypeOfDuplicates.Text)
                                    {
                                        var componentToString = ((IStiText)componentTo).GetTextInternal();
                                        var componentString = ((IStiText)component).GetTextInternal();

                                        #region Based on Tag processing
                                        if (duplicatesType == StiProcessingDuplicatesType.BasedOnTagHide ||
                                            duplicatesType == StiProcessingDuplicatesType.BasedOnTagMerge ||
                                            duplicatesType == StiProcessingDuplicatesType.BasedOnTagRemoveText)
                                        {
                                            componentToString = componentTo.TagValue == null ? string.Empty : componentTo.TagValue.ToString();
                                            componentString = component.TagValue == null ? string.Empty : component.TagValue.ToString();
                                        };
                                        #endregion

                                        if (Math.Round(componentTo.Top, 2) >= Math.Round(bottom, 2) && componentString == componentToString)
                                        {
                                            if (duplicatesType == StiProcessingDuplicatesType.Merge ||
                                                duplicatesType == StiProcessingDuplicatesType.GlobalMerge ||
                                                duplicatesType == StiProcessingDuplicatesType.BasedOnTagMerge)
                                            {
                                                component.Height += componentTo.Bottom - component.Bottom;
                                            }

                                            if (duplicatesType == StiProcessingDuplicatesType.RemoveText ||
                                                duplicatesType == StiProcessingDuplicatesType.GlobalRemoveText ||
                                                duplicatesType == StiProcessingDuplicatesType.BasedOnTagRemoveText)
                                            {
                                                ((IStiText)componentTo).SetTextInternal(string.Empty);

                                                index++;
                                                startIndex++;
                                            }
                                            else
                                            {
                                                bottom += componentTo.Height;
                                                componentTo.Parent.Components.Remove(componentTo);
                                                coordY.RemoveAt(startIndex);
                                            }
                                        }
                                        else break;
                                    }
                                    #endregion

                                    #region Image
                                    else if (typeOfDuplicates == TypeOfDuplicates.Image)
                                    {
                                        if (Math.Round(componentTo.Top, 2) >= Math.Round(bottom, 2) &&
                                            StiImageHelper.IsEqualImages(((StiImage)componentTo).TakeImageToDraw(), ((StiImage)component).TakeImageToDraw()))
                                        {
                                            if (((StiImage)component).ProcessingDuplicates == StiImageProcessingDuplicatesType.Merge ||
                                                ((StiImage)component).ProcessingDuplicates == StiImageProcessingDuplicatesType.GlobalMerge)
                                            {
                                                component.Height += componentTo.Bottom - component.Bottom;
                                            }

                                            if (((StiImage)component).ProcessingDuplicates == StiImageProcessingDuplicatesType.RemoveImage ||
                                                ((StiImage)component).ProcessingDuplicates == StiImageProcessingDuplicatesType.GlobalRemoveImage)
                                            {
                                                ((StiImage)componentTo).ResetImageToDraw();

                                                index++;
                                                startIndex++;
                                            }
                                            else
                                            {
                                                bottom += componentTo.Height;
                                                componentTo.Parent.Components.Remove(componentTo);
                                                coordY.RemoveAt(startIndex);
                                            }
                                        }
                                        else break;
                                    }
                                    #endregion

                                }
                                else break;
                            }
                            else startIndex++;
                            cont = contTo;
                        }
                        index++;
                    }
                }
            }
            #endregion
        }
		#endregion
	}
}
