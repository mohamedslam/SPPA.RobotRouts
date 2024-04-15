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
    /// <summary>
    /// A class helps to output EmptyBands.
    /// </summary>
    internal class StiEmptyBandsV2Helper
    {
        #region Fields
        /// <summary>
        /// Selected EmptyBand. Бэнд выбирается методом Register.
        /// </summary>
        private StiEmptyBand emptyBand;
        #endregion

        #region Properties
        internal StiEngine Engine { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Registers the list of EmptyBands. Only the first EmptyBand from a co;;ection is used.
        /// </summary>
        public void Register(StiComponentsCollection emptyBands)
        {
            if (emptyBands != null && emptyBands.Count > 0)
                this.emptyBand = emptyBands[0] as StiEmptyBand;
        }

        /// <summary>
        /// Clears selected EmptyBand.
        /// </summary>
        public void Clear()
        {
            this.emptyBand = null;
        }

        /// <summary>
        /// Creates a container on the base of selected EmptyBand.
        /// </summary>
        private StiContainer CreateEmptyBandContainer(StiContainer containerForRender)
        {
            Engine.Report.Line++;
            Engine.Report.LineThrough++;

            //Runs the Rendering event of the EmptyBand.
            emptyBand.InvokeRendering();

            emptyBand.ParentBookmark = containerForRender.CurrentBookmark;
            emptyBand.ParentPointer = containerForRender.CurrentPointer;

            var comp = emptyBand.Render();
            if (comp == null) return null;

            //Apply styles to EmptyBand.
            StiOddEvenStylesHelper.ApplyOddEvenStyles(Engine.Report, emptyBand, comp as StiContainer);

            comp.DockStyle = StiDockStyle.None;
            comp.Left = Engine.PositionX;
            comp.Top = Engine.PositionY;

            return comp as StiContainer;
        }

        public StiDataBand FindDataBand(StiEmptyBand emptyBand)
        {
            if (emptyBand == null) return null;
            if (emptyBand.Parent == null) return null;

            var index = emptyBand.Parent.Components.IndexOf(emptyBand) - 1;

            while (index >= 0)
            {
                var comp = emptyBand.Parent.Components[index];
                if (comp is StiEmptyBand) break;
                if (comp is StiDataBand)
                {
                    return comp as StiDataBand;
                }
                index--;
            }
            return null;
        }

        /// <summary>
        /// Renders the EmptyBand in the specified container. EmptyBand will take 
        /// free space in the specified container.
        /// </summary>
        /// <param name="containerForRender">A container to output EmptyBand.</param>
        /// <param name="selectedContainer">A container-marker that is used to find Keep containers.</param>
        public void Render(StiContainer containerForRender, StiContainer selectedContainer)
        {
            if (emptyBand == null) return;

            Engine.IsDynamicBookmarksMode = true;

            try
            {
                var dataBand = FindDataBand(emptyBand);
                if (dataBand != null)
                {
                    var obj = Engine.HashDataBandLastLine[dataBand.Name];
                    if (obj is int)
                    {
                        emptyBand.Report.Line = (int)obj;
                        emptyBand.Report.LineThrough = (int)obj;
                    }
                }

                //Runs the BeginRender event for the EmptyBand.
                emptyBand.InvokeBeginRender();

                var startIndex = -1;

                if (containerForRender != null)
                    startIndex = containerForRender.Components.IndexOf(selectedContainer);

                if (startIndex == -1)
                    startIndex = containerForRender.Components.Count;

                StiContainer comp;
                StiContainer latestRenderedEmptyBand = null;

                #region Render EmptyBand until there is a free space in the specified container for output
                while (true)
                {
                    comp = CreateEmptyBandContainer(containerForRender);
                    if (comp == null)break;

                    if (comp.Height == 0)
                        comp.Height = Engine.Report.Unit.ConvertFromHInches(10d);

                    if (comp.Height <= Engine.FreeSpace)
                    {
                        Engine.PositionY += comp.Height;
                        Engine.FreeSpace -= comp.Height;

                        containerForRender.Components.Insert(startIndex, comp);
                        startIndex++;
                        latestRenderedEmptyBand = comp;

                        bool isNewGuidCreated = comp.DoBookmark();
                        comp.DoPointer(!isNewGuidCreated);
                    }
                    else break;

                    if (Engine.FreeSpace > 999999999) break;
                }
                #endregion

                /* Process the DecreaseLastRow mode. Add the last rendered report (that cannot be placed, because
                 * there is no enough free space in the container of output)  
                 * into the container of output and set the same height as in the free space in the container of output. */
                if (emptyBand.SizeMode == StiEmptySizeMode.DecreaseLastRow)
                {
                    comp.Height = Engine.FreeSpace;
                    Engine.PositionY += comp.Height;
                    Engine.FreeSpace -= comp.Height;
                    containerForRender.Components.Insert(startIndex, comp);
                    bool isNewGuidCreated = comp.DoBookmark();
                    comp.DoPointer(!isNewGuidCreated);
                }

                /* Increase the height of the last rendered EmptyBand on the height of FreeSpace. */
                else if (emptyBand.SizeMode == StiEmptySizeMode.IncreaseLastRow)
                {
                    if (latestRenderedEmptyBand == null)
                    {
                        if (Engine.FreeSpace > 0)
                        {
                            comp = CreateEmptyBandContainer(containerForRender);
                            comp.Top = Engine.PositionY;
                            comp.Height = Engine.FreeSpace;
                        }
                    }
                    else
                        latestRenderedEmptyBand.Height += Engine.FreeSpace;
                }

                /* Move PrintAtBottom containers on the top on the height of FreeSpace. */
                else if (emptyBand.SizeMode == StiEmptySizeMode.AlignFooterToTop)
                {
                    lock (((ICollection) containerForRender.Components).SyncRoot)
                    {
                        foreach (StiComponent component in containerForRender.Components)
                        {
                            if (component.Top > Engine.PositionY && !(component is StiCrossLinePrimitive))
                            {
                                component.Top -= Engine.FreeSpace;
                            }
                        }
                    }
                }

                //Runs the EndRender event for EmptyBand.
                emptyBand.InvokeEndRender();
            }
            finally
            {
                Engine.IsDynamicBookmarksMode = false;
            }
        }
        #endregion

        internal StiEmptyBandsV2Helper(StiEngine engine)
        {
            this.Engine = engine;
        }
    }
}
