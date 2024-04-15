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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Stimulsoft.Report.Chart
{
    public class StiTreemapAreaCoreXF : StiAreaCoreXF
    {
        #region Methods
        public override StiCellGeom Render(StiContext context, RectangleF rect)
        {   
            var treeAreaGeom = new StiTreemapAreaGeom(this.Area, rect);
            var seriesCollection = GetSeries();
            var mergedData = new List<double>();


            foreach (var series in seriesCollection)
            {
                var listValues = new List<double>();
                foreach (var value in series.Values.ToList())
                    listValues.Add(value.GetValueOrDefault());

                mergedData.Add(listValues.Sum());
            }

            var rectSeries = new RectangleF(0, 0, rect.Width, rect.Height);

            var normalizeData = NormalizeDataForArea(mergedData, rectSeries.Width * rectSeries.Height);
            var rects = Squarify(normalizeData, new List<double?>(), rectSeries, new List<RectangleF>());
 
            var drawEmpty = true;

            for (var index = 0; index < seriesCollection.Count(); index++)
            {
                if (rects.Count - 1 >= index)
                {
                    seriesCollection[index].Core.RenderSeries(context, rects[index], treeAreaGeom, seriesCollection.ToArray());
                    drawEmpty = false;
                }
            }

            if (drawEmpty)
            {
                treeAreaGeom.CreateChildGeoms();
                treeAreaGeom.ChildGeoms.Add(new StiTreemapEmptySeriesElementGeom(rectSeries));
            }
            
            return treeAreaGeom;
        }
        

        private RectangleF CutArea(RectangleF container, double area)
        {
            RectangleF newcontainer;

            if (container.Width >= container.Height)
            {
                var areawidth = area / container.Height;
                var newwidth = container.Width - areawidth;
                newcontainer = new RectangleF(container.X + (float)areawidth, container.Y, (float)newwidth, container.Height);
            }
            else
            {
                var areaheight = area / container.Width;
                var newheight = container.Height - areaheight;
                newcontainer = new RectangleF(container.X, container.Y + (float)areaheight, container.Width, (float)newheight);
            }
            return newcontainer;
        }

        public List<RectangleF> Squarify(List<double> data, List<double?> currentrow, RectangleF container, List<RectangleF> stack)
        {
            // Emulate recursive calls through our own stack - to be able to process huge amounts of data
            var functionStack = new Stack<Tuple<List<double>, List<double?>, RectangleF>>();
            functionStack.Push(Tuple.Create(data, currentrow, container));

            while (functionStack.Count > 0)
            {
                var currParams = functionStack.Pop();
                var currData = currParams.Item1;
                var currCurrentrow = currParams.Item2;
                var currContainer = currParams.Item3;

                if (currData.Count == 0)
                {
                    stack.AddRange(GetCoordinates(currContainer, currCurrentrow));
                    continue;
                }

                var length = Math.Min(currContainer.Width, currContainer.Height);
                var nextdatapoint = currData[0];

                if (ImprovesRatio(currCurrentrow, nextdatapoint, length))
                {
                    currCurrentrow.Add(nextdatapoint);
                    functionStack.Push(Tuple.Create(currData.Skip(1).ToList(), currCurrentrow, currContainer));
                }
                else
                {
                    var newcontainer = CutArea(currContainer, currCurrentrow.Sum().GetValueOrDefault());
                    stack.AddRange(GetCoordinates(currContainer, currCurrentrow));
                    var newRow = new List<double?>();
                    functionStack.Push(Tuple.Create(currData, newRow, newcontainer));
                }
            }

            return stack;
        }

        private bool ImprovesRatio(List<double?> currentrow, double? nextnode, float length)
        {
            if (currentrow.Count == 0)
            {
                return true;
            }

            var newrow = currentrow.ToList();
            newrow.Add(nextnode);

            var currentratio = CalculateRatio(currentrow, length);
            var newratio = CalculateRatio(newrow, length);
            
            return currentratio >= newratio;
        }

        private double CalculateRatio(List<double?> row, float length)
        {
            var sum = row.Sum().GetValueOrDefault();
            var v1 = Math.Pow(length, 2) * row.Max() / Math.Pow(sum, 2);
            var v2 = Math.Pow(sum, 2) / (Math.Pow(length, 2) * row.Min());
            return Math.Max(v1.GetValueOrDefault(), v2.GetValueOrDefault());
        }

        public List<double> NormalizeDataForArea(List<double> data, float area)
        {
            var normalizeddata = new List<double>();
            var sum = data.Sum();
            if (sum == 0)
                return normalizeddata;

            var multiplier = area / sum;
            
            for (var index = 0; index < data.Count; index++)
            {
                normalizeddata.Add(data[index] * multiplier);
            }
            return normalizeddata;
        }

        private List<RectangleF> GetCoordinates(RectangleF rootContainer, List<double?> rowData)
        {
            var coordinates = new List<RectangleF>();
            var rowDataSum = rowData.Sum();
            if (rowDataSum == 0)
                return coordinates;

            var subxoffset = rootContainer.X;
            var subyoffset = rootContainer.Y;
            var areawidth = rowDataSum / rootContainer.Height;
            var areaheight = rowDataSum / rootContainer.Width;

            if (rootContainer.Width >= rootContainer.Height)
            {
                for (var i = 0; i < rowData.Count; i++)
                {
                    var height = areawidth != 0 ? (float)(rowData[i].GetValueOrDefault() / areawidth) : 0;
                    coordinates.Add(new RectangleF(subxoffset, subyoffset, (float)areawidth, height));
                    subyoffset = subyoffset + (float)(rowData[i].GetValueOrDefault() / areawidth);
                }
            }
            else
            {
                for (var i = 0; i < rowData.Count; i++)
                {
                    var width = areaheight != 0 ? (float)(rowData[i].GetValueOrDefault() / areaheight) : 0;
                    coordinates.Add(new RectangleF(subxoffset, subyoffset, width, (float)areaheight));
                    subxoffset = subxoffset + (float)(rowData[i].GetValueOrDefault() / areaheight);
                }
            }
            return coordinates;
        }

        protected override void PrepareInfo(RectangleF rect)
        {
        }

        #endregion

        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string LocalizedName
        {
            get
            {
                return StiLocalization.Get("Chart", "Treemap");
            }
        }
        #endregion

        #region Properties.Settings
        public override int Position
        {
            get
            {
                return (int)StiChartAreaPosition.Treemap;
            }
        }
        #endregion  

        public StiTreemapAreaCoreXF(IStiArea area)
            : base(area)
        {
        }
    }
}
