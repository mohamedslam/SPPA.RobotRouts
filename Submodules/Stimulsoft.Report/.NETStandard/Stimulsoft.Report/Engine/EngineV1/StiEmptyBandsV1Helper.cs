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
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Report.Engine
{
    /// <summary>
    /// A class helps to output EmptyBands.
    /// </summary>
    internal class StiEmptyBandsV1Helper
    {
		#region Methods
        /// <summary>
        /// Creates a container on the base of a selected EmptyBand.
        /// </summary>
        private static StiContainer CreateEmptyBandContainer(StiReport report, StiContainer containerForRender, StiEmptyBand emptyBand)
        {
            report.Line++;
            report.LineThrough++;
            
            //Runs the Rendering event for an EmptyBand.
            emptyBand.InvokeRendering();

            emptyBand.ParentBookmark = containerForRender.CurrentBookmark;            
			StiComponent comp = null;
			emptyBand.RenderContainer(ref comp, containerForRender);
			containerForRender.Components.Remove(comp);

            //Apply styles to an EmptyBand.
			StiOddEvenStylesHelper.ApplyOddEvenStyles(report, emptyBand, comp as StiContainer);

            return comp as StiContainer;
        }

        /// <summary>
        /// Builds an EmptyBand in the specified container. The EmptyBand will  
        /// take a free space in the specified container.
        /// </summary>
        /// <param name="containerForRender">A container to output an EmptyBand.</param>
        public static void Render(StiReport report, StiContainer containerForRender, StiEmptyBand emptyBand)
        {
            if (emptyBand == null)return;

            //Runs the BeginRender event for an EmptyBand.
            emptyBand.InvokeBeginRender();

            var startIndex = containerForRender.Components.Count;

            StiContainer comp;
            StiContainer latestRenderedEmptyBand = null;

            #region Render an EmptyBand until there is an empty space in the specifeid container for output
            var rect = containerForRender.GetDockRegion(containerForRender);
            var freeSpace = rect.Height;
            var positionY = rect.Top;

            while (true)
            {
                comp = CreateEmptyBandContainer(report, containerForRender, emptyBand);
                if (comp.Height == 0)
                    comp.Height = report.Unit.ConvertFromHInches(10d);

                if (comp.Height <= freeSpace)
                {
                    comp.Top = positionY;
                    freeSpace -= comp.Height;

                    containerForRender.Components.Insert(startIndex, comp);
                    startIndex++;
                    latestRenderedEmptyBand = comp;

                    comp.DoBookmark();
                }
                else break;
            }
            #endregion

            //Process the DecreaseLastRow mode. Add the last rendered container (that is not placed, 
	        //due to lack of free space in the container of output)  
			//to the container of output and set its height equal to free space in the container of output (FreeSpace).
            if (emptyBand.SizeMode == StiEmptySizeMode.DecreaseLastRow)
            {
                comp.Height = freeSpace;
                containerForRender.Components.Insert(startIndex, comp);
                comp.DoBookmark();
            }
            
            //Increase the height of the last rendered EmptyBand on the height of FreeSpace.
            else if (emptyBand.SizeMode == StiEmptySizeMode.IncreaseLastRow)
            {
                if (latestRenderedEmptyBand == null)
                {
                    if (freeSpace > 0)
                    {
                        comp = CreateEmptyBandContainer(report, containerForRender, emptyBand);
                        comp.Top = positionY;
                        comp.Height = freeSpace;
                    }
                }
                else
                    latestRenderedEmptyBand.Height += freeSpace;
            }

            //Move PrintAtBottom containers up on the height of FreeSpace.
            else if (emptyBand.SizeMode == StiEmptySizeMode.AlignFooterToTop)
            {
                foreach (StiComponent component in containerForRender.Components)
                {
                    if (component.Top > positionY)
                    {
                        component.Top -= freeSpace;
                    }
                }
            }

            //Runs the EndRender event for an EmptyBand.
            emptyBand.InvokeEndRender();
        }
        #endregion
    }
}
