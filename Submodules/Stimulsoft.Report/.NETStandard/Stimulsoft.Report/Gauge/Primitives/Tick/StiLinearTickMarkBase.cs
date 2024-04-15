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

using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Gauge.Helpers;
using Stimulsoft.Report.Painters;
using System;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Components.Gauge.Primitives
{
    public abstract class StiLinearTickMarkBase :
        StiTickMarkBase
    {
        #region Methods
        protected internal override void DrawElement(StiGaugeContextPainter context)
        {
            var linearScale = this.Scale as StiLinearScale;
            if (linearScale == null) return;

            var size = linearScale.barGeometry.Size;
            var rect = linearScale.barGeometry.RectGeometry;
            var collection = GetPointCollection();
            if (collection.Count == 0) return;

            #region Temporary Variables
            var skipValues = base.SkipValuesObj;
            var skipIndices = base.SkipIndicesObj;
            var skin = this.GetActualSkin();
            float startValue = this.Scale.ScaleHelper.ActualMinimum;
            float endValue = this.Scale.ScaleHelper.ActualMaximum;

            bool isUp = (linearScale.StartWidth < linearScale.EndWidth);

            float distance, scaleOffset;
            if (linearScale.Orientation == Orientation.Horizontal)
            {
                distance = rect.Width;
                scaleOffset = size.Height;
            }
            else
            {
                distance = rect.Height;
                scaleOffset = size.Width;
            }

            float rest = linearScale.barGeometry.GetRestToLenght();
            scaleOffset *= base.Offset;

            float tickWidth = size.Width * this.RelativeWidth;
            float tickHeight = size.Height * this.RelativeHeight;
            #endregion

            int index = -1;
            foreach (float key in collection.Keys)
            {
                index++;

                #region Check Value
                if (key < startValue) continue;
                if (key > endValue) continue;
                if (CheckTickValue(skipValues, skipIndices, key, index)) continue;
                if (this.MinimumValue != null && key < this.MinimumValue.Value) continue;
                if (this.MaximumValue != null && key > this.MaximumValue.Value) continue;
                #endregion

                float offsetByValue = collection[key] * distance;
                float top, left;

                if (linearScale.Orientation == Orientation.Horizontal)
                {
                    left = (linearScale.IsReversed) ? rect.Right - offsetByValue - tickWidth :
                        rect.Left + offsetByValue - (tickWidth / 2);

                    if (this.Placement == StiPlacement.Overlay)
                    {
                        top = StiRectangleHelper.CenterY(rect) - (tickHeight / 2) - scaleOffset;
                    }
                    else
                    {
                        float restValue = (isUp) ? (1 - collection[key]) * rest : rest * collection[key];
                        if (this.Placement == StiPlacement.Outside)
                        {
                            top = rect.Top - tickHeight - scaleOffset + restValue;
                        }
                        else
                        {
                            top = rect.Bottom + scaleOffset - restValue;
                        }
                    }
                }
                else
                {
                    top = (linearScale.IsReversed) ? (rect.Top + offsetByValue - (tickHeight / 2)) :
                        (rect.Bottom - offsetByValue - (tickHeight / 2));

                    if (this.Placement == StiPlacement.Overlay)
                    {
                        left = StiRectangleHelper.CenterX(rect) - (tickWidth / 2) - scaleOffset;
                    }
                    else
                    {
                        float restValue = (isUp) ? (1 - collection[key]) * rest : rest * collection[key];

                        if (this.Placement == StiPlacement.Outside)
                        {
                            left = rect.Left - tickWidth - 3 - scaleOffset + restValue;
                        }
                        else
                        {
                            left = rect.Right + scaleOffset + 3 - restValue;
                        }
                    }
                }

                skin.Draw(context, this, new RectangleF((float)Math.Round(left), (float)Math.Round(top), (float)Math.Ceiling(tickWidth), (float)Math.Ceiling(tickHeight)));
            }
        }
        #endregion
    }
}
