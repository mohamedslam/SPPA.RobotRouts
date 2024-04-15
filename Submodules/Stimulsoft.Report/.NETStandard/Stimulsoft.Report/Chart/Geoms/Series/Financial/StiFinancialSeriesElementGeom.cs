#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiFinancialSeriesElementGeom : StiCellGeom
    {
        #region IStiGeomInteraction override
        public override void InvokeMouseEnter(StiInteractionOptions options)
        {
            if (!AllowMouseOver) return;

            if (!IsMouseOver)
            {
                IsMouseOver = true;
                options.UpdateContext = series.Interaction.DrillDownEnabled;
            }

            int valueIndex = GetValueIndex();

            options.InteractionToolTip = GetToolTip(valueIndex);
            options.InteractionHyperlink = GetHyperlink(valueIndex);
        }

        public override void InvokeClick(StiInteractionOptions options)
        {
            int valueIndex = GetValueIndex();

            if (Series.Hyperlinks != null && valueIndex < Series.Hyperlinks.Length)
            {
                options.InteractionHyperlink = series.Hyperlinks[valueIndex];
            }

            if (Series.Interaction.DrillDownEnabled)
            {
                options.SeriesInteractionData = this.Interaction;

                IsMouseOver = false;
                options.UpdateContext = series.Interaction.DrillDownEnabled;
            }
        }

        private int GetValueIndex()
        {
            int valueIndex = this.Index;

            if (this.Series.Chart.Area is IStiAxisArea && ((IStiAxisArea)this.Series.Chart.Area).ReverseHor)
                valueIndex = Series.Arguments.Length - valueIndex - 1;

            return valueIndex;
        }

        private string GetHyperlink(int valueIndex)
        {
            if (Series.Hyperlinks != null && valueIndex < Series.Hyperlinks.Length)
                return series.Hyperlinks[valueIndex];
            else
                return null;
        }

        private string GetToolTip(int valueIndex)
        {
            if (Series.ToolTips != null && valueIndex < Series.ToolTips.Length)
                return series.ToolTips[valueIndex];
            else
                return null;
        }

        public virtual bool AllowMouseOver
        {
            get
            {
                int index = GetValueIndex();
                return
                    ((Series.Hyperlinks != null && index < Series.Hyperlinks.Length) ||
                    (Series.ToolTips != null && index < Series.ToolTips.Length)) ||
                    (this.Series.Interaction.DrillDownEnabled && this.Series.Interaction.AllowSeriesElements);
            }
        }

        public virtual bool IsMouseOver
        {
            get
            {
                return this.Series.Core.GetIsMouseOverSeriesElement(this.Index);
            }
            set
            {
                this.Series.Core.SetIsMouseOverSeriesElement(this.Index, value);
            }
        }
        #endregion

        #region Properties
        private IStiSeries series;
        public IStiSeries Series
        {
            get
            {
                return series;
            }
        }

        private StiSeriesInteractionData interaction;
        public StiSeriesInteractionData Interaction
        {
            get
            {
                return interaction;
            }
            set
            {
                interaction = value;
            }
        }

        private float open;
        public float Open
        {
            get
            {
                return open;
            }
        }

        private float close;
        public float Close
        {
            get
            {
                return close;
            }
        }

        private float high;
        public float High
        {
            get
            {
                return high;
            }
        }

        private float low;
        public float Low
        {
            get
            {
                return low;
            }
        }

        private float positionX;
        public float PositionX
        {
            get
            {
                return positionX;
            }
        }

        private StiAreaGeom areaGeom;
        public StiAreaGeom AreaGeom
        {
            get
            {
                return areaGeom;
            }
        }

        private int index;
        public int Index
        {
            get
            {
                return index;
            }
        }
        #endregion        

        public override void Draw(StiContext context)
        {
        }

        public StiFinancialSeriesElementGeom(StiAreaGeom areaGeom, IStiSeries series, RectangleF clientRectangle,
            float open, float close, float high, float low, float positionX, int index)
            : base(clientRectangle)
        {
            this.areaGeom = areaGeom;
            this.series = series;
            this.open = open;
            this.close = close;
            this.high = high;
            this.low = low;
            this.positionX = positionX;
            this.index = index;
        }
    }
}
