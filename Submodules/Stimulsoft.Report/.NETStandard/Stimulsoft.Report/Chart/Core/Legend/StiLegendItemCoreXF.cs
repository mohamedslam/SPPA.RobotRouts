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
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiLegendItemCoreXF
    {
        #region Properties
        public string Text { get; }

        public IStiSeries Series { get; }

        public int Index { get; } = -1;
        
        public int ColorIndex { get; } = 0;
        #endregion

        #region Methods
        public string GetText(StiContext context, StiFontGeom font)
        {
            var width = Series.Chart.Legend.ColumnWidth * context.Options.Zoom;

            if (width == 0 || Series.Chart.Legend.WordWrap)
                return Text;

            return StiTextContentHelper.GetMeasureText(context, Text, font, width);
        }

        public SizeF MeasureString(StiContext context, StiFontGeom font)
        {
            var text = GetText(context, font);
            var width = Series.Chart.Legend.ColumnWidth * context.Options.Zoom;
            var sf = context.GetDefaultStringFormat();

            if (!Series.Chart.Legend.WordWrap)
                sf.FormatFlags = StringFormatFlags.NoWrap;

            var size =  context.MeasureString(text, font, (int)width, sf);
            
            if (size.Width < width)
                size.Width = width;

            return size;
        } 
        #endregion

        public StiLegendItemCoreXF(string text, IStiSeries series, int index, int colorIndex)
        {
            this.Text = text;
            this.Series = series;
            this.Index = index;
            this.ColorIndex = colorIndex;
        }
    }
}
