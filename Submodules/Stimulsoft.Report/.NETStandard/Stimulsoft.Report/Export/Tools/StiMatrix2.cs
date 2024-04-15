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
using System.Globalization;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Stimulsoft.Base;
using System.Threading;

namespace Stimulsoft.Report.Export
{
    public class StiMatrixCacheManager
    {
        private int matrixWidth = 0;
        private int matrixHeight = 0;
        private int segmentHeight = 0;
        private StiMatrix matrix = null;
        private int amountOfProcessedPagesForStartGCCollect = 0;

        private string cachePath = null;

        internal bool flagForceSaveSegment = false;

        #region Properties.Private
        internal StiMatrixCacheSegment[] segments = null;
        public StiMatrixCacheSegment[] Segments
        {
            get
            {
                if (segments == null)
                {
                    int count = matrixHeight / segmentHeight + (matrixHeight % segmentHeight == 0 ? 0 : 1);
                    segments = new StiMatrixCacheSegment[count];
                }
                return segments;
            }
        }

        private List<int> quickAccessSegments = null;
        public List<int> QuickAccessSegments
        {
            get
            {
                if (quickAccessSegments == null)
                {
                    quickAccessSegments = new List<int>();
                }
                return quickAccessSegments;
            }
        }
        #endregion

        #region Properties.Public
        private int amountOfQuickAccessSegments = 3;
        public int AmountOfQuickAccessSegments
        {
            get
            {
                return amountOfQuickAccessSegments;
            }
            set
            {
                amountOfQuickAccessSegments = value;
            }
        }
        #endregion

        #region Methods
        public StiMatrixCacheSegment GetMatrixSegment(int index)
        {
            StiMatrixCacheSegment[] segments = Segments;
            if (segments[index] == null)
            {
                segments[index] = new StiMatrixCacheSegment(segmentHeight);
            }

            if (!matrix.useCacheMode) return segments[index];
            if (QuickAccessSegments.Contains(index)) return segments[index];

            quickAccessSegments.Add(index);
            if (segments[index].IsSaved)
            {
                LoadSegment(segments[index]);
                if (flagForceSaveSegment)
                {
                    segments[index].IsSaved = false;
                }
            }

            if (quickAccessSegments.Count > AmountOfQuickAccessSegments)
            {
                int number = quickAccessSegments[0];
                quickAccessSegments.RemoveAt(0);
                StiMatrixCacheSegment segment = segments[number];
                if (!segment.IsSaved)
                {
                    SaveSegment(segment);
                    segment.IsSaved = true;
                }

                //segment.Lines = null;
                segment.Clear();

                if (StiOptions.Engine.ReportCache.AllowGCCollect)
                {
                    amountOfProcessedPagesForStartGCCollect++;
                    if (amountOfProcessedPagesForStartGCCollect >= StiOptions.Engine.ReportCache.AmountOfProcessedPagesForStartGCCollect)
                    {
                        amountOfProcessedPagesForStartGCCollect = 0;
                        StiMatrix.GCCollect();
                    }
                }
            }

            return segments[index];
        }


        public StiMatrixLineData GetMatrixLineData(int lineNumber)
        {
            int segmentIndex = lineNumber / segmentHeight;
            int lineIndex = lineNumber % segmentHeight;

            StiMatrixCacheSegment cs = GetMatrixSegment(segmentIndex);

            StiMatrixLineData[] lines = cs.Lines;
            //if (lines == null)
            //{
            //    lines = new StiMatrix.StiMatrixLineData[segmentHeight];
            //    cs.Lines = lines;
            //}
            if (lines[lineIndex] == null)
            {
                lines[lineIndex] = new StiMatrixLineData(matrixWidth);
            }
            return lines[lineIndex];
        }
        #endregion

        #region WriteSegment
        //public void SaveSegment(Stream stream, int number)
        public void WriteSegment(Stream stream, StiMatrixCacheSegment segment)
        {
            //CacheSegment segment = GetMatrixSegment(number);
            int count = segment.Lines.Length;
            for (; count > 0; count--)
            {
                if (segment.Lines[count - 1] != null) break;
            }

            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                XmlTextWriter tw = new XmlTextWriter(stream, Encoding.UTF8);
                tw.Formatting = Formatting.Indented;

                tw.WriteStartDocument(true);
                tw.WriteStartElement("StiMatrixSerializer");
                tw.WriteAttributeString("version", StiFileVersions.ReportFile);
                tw.WriteAttributeString("type", "Net");
                tw.WriteAttributeString("application", "StiMatrixCache");

                tw.WriteStartElement("Lines");
                tw.WriteAttributeString("count", count.ToString());

                for (int index = 0; index < count; index++)
                {
                    StiMatrixLineData line = segment.Lines[index];
                    WriteMatrixLine(tw, line);
                }

                tw.WriteFullEndElement();   //Lines

                tw.WriteFullEndElement();   //StartDocument
                tw.Flush();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        private void WriteMatrixLine(XmlTextWriter tw, StiMatrixLineData line)
        {
            tw.WriteStartElement("line");
            if (line != null)
            {
                //cells
                foreach (StiCell cell in line.Cells)
                {
                    WriteMatrixCell(tw, cell);
                }

                //bookmarks
                if (line.Bookmarks != null)
                {
                    int index = 0;
                    while (index < line.Bookmarks.Length)
                    {
                        string bookmark = line.Bookmarks[index];
                        if (bookmark != null)
                        {
                            int count = 0;
                            while ((index + count + 1 < line.Bookmarks.Length) && (line.Bookmarks[index + count + 1] == bookmark)) count++;

                            tw.WriteStartElement("bkm");
                            tw.WriteAttributeString("x", index.ToString());
                            if (count > 0)
                            {
                                tw.WriteAttributeString("w", count.ToString());
                            }
                            tw.WriteAttributeString("t", bookmark);
                            tw.WriteEndElement();

                            index += count;
                        }
                        index++;
                    }
                }

                //bordersX
                if (line.BordersX != null)
                {
                    int start = 0;
                    while ((start < line.BordersX.Length) && (line.BordersX[start] == 0)) start++;
                    int end = line.BordersX.Length - 1;
                    while ((end > start) && (line.BordersX[end] == 0)) end--;
                    StringBuilder sb = new StringBuilder();
                    if (start < end)
                    {
                        for (int index = start; index <= end; index++)
                        {
                            if (line.BordersX[index] != 0) sb.Append(line.BordersX[index]);
                            if (index != end) sb.Append(",");
                        }

                        tw.WriteStartElement("bordersX");
                        tw.WriteAttributeString("x", start.ToString());
                        tw.WriteAttributeString("v", sb.ToString());
                        tw.WriteEndElement();
                    }
                }

                //bordersY
                if (line.BordersY != null)
                {
                    int start = 0;
                    while ((start < line.BordersY.Length) && (line.BordersY[start] == 0)) start++;
                    int end = line.BordersY.Length - 1;
                    while ((end > start) && (line.BordersY[end] == 0)) end--;
                    if (start < end)
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int index = start; index <= end; index++)
                        {
                            if (line.BordersY[index] != 0) sb.Append(line.BordersY[index]);
                            if (index != end) sb.Append(",");
                        }

                        tw.WriteStartElement("bordersY");
                        tw.WriteAttributeString("x", start.ToString());
                        tw.WriteAttributeString("v", sb.ToString());
                        tw.WriteEndElement();
                    }
                }

                //cellstyles
                if (line.CellStyles != null)
                {
                    int start = 0;
                    while ((start < line.CellStyles.Length) && (line.CellStyles[start] == 0)) start++;
                    int end = line.CellStyles.Length - 1;
                    while ((end > start) && (line.CellStyles[end] == 0)) end--;
                    if (start < end)
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int index = start; index <= end; index++)
                        {
                            if (line.CellStyles[index] != 0) sb.Append(line.CellStyles[index]);
                            if (index != end) sb.Append(",");
                        }

                        tw.WriteStartElement("cellstyles");
                        tw.WriteAttributeString("x", start.ToString());
                        tw.WriteAttributeString("v", sb.ToString());
                        tw.WriteEndElement();
                    }
                }

            }
            tw.WriteEndElement();
        }

        private void WriteMatrixCell(XmlTextWriter tw, StiCell cell)
        {
            if (cell != null)
            {
                tw.WriteStartElement("cell");
                tw.WriteAttributeString("x", cell.Left.ToString());
                if (cell.Width > 0)
                {
                    tw.WriteAttributeString("w", cell.Width.ToString());
                }
                if (cell.Height > 0)
                {
                    tw.WriteAttributeString("h", cell.Height.ToString());
                }
                StiCell2 cell2 = cell as StiCell2;
                if (cell2 != null)
                {
                    tw.WriteAttributeString("pid", cell2.PageId.ToString());
                    tw.WriteAttributeString("cid", cell2.ComponentId.ToString());
                    if (cell2.cellStyleId != -1)
                    {
                        tw.WriteAttributeString("sid", cell2.cellStyleId.ToString());
                    }
                }
                if (!string.IsNullOrEmpty(cell.Text))
                {
                    tw.WriteAttributeString("t", cell.Text);
                }
                tw.WriteEndElement();   //cell
            }
        }
        #endregion

        #region ReadSegment
        //public void LoadSegment(Stream stream, int number)
        public void ReadSegment(Stream stream, StiMatrixCacheSegment segment)
        {
            //CacheSegment segment = GetMatrixSegment(number);

            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                XmlDocument doc = new XmlDocument();
                doc.Load(stream);

                if ((doc.DocumentElement.Name == "StiMatrixSerializer") &&
                    (doc.DocumentElement.ChildNodes.Count > 0) && (doc.DocumentElement.ChildNodes[0].Name == "Lines"))
                {
                    XmlNode rootNode = doc.DocumentElement.ChildNodes[0];

                    for (int index = 0; index < rootNode.ChildNodes.Count; index++)
                    {
                        XmlNode node = rootNode.ChildNodes[index];

                        StiMatrixLineData line = segment.Lines[index];
                        if (line == null)
                        {
                            segment.Lines[index] = new StiMatrixLineData(matrixWidth);
                            line = segment.Lines[index];
                        }

                        foreach (XmlNode childNode in node.ChildNodes)
                        {
                            switch (childNode.Name)
                            {
                                case "cell":
                                    ReadMatrixCell(childNode, line);
                                    break;

                                case "bordersX":
                                    ReadMatrixBorders(childNode, line.BordersX);
                                    break;

                                case "bordersY":
                                    ReadMatrixBorders(childNode, line.BordersY);
                                    break;

                                case "bkm":
                                    ReadMatrixBookmark(childNode, line.Bookmarks);
                                    break;

                                case "cellstyles":
                                    ReadMatrixCellStyles(childNode, line.CellStyles);
                                    break;
                            }
                        }
                    }

                }

                doc = null;

                //initialize the Cell.Top property
                int segmentIndex = Array.IndexOf(segments, segment);
                if (segmentIndex != -1)
                {
                    for (int indexLine = 0; indexLine < segment.Lines.Length; indexLine++)
                    {
                        StiMatrixLineData line = segment.Lines[indexLine];
                        if (line != null)
                        {
                            foreach (StiCell cell in line.Cells)
                            {
                                if (cell != null)
                                {
                                    cell.Top = segmentIndex * segmentHeight + indexLine;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        private void ReadMatrixCell(XmlNode node, StiMatrixLineData line)
        {
            StiCell2 cell = new StiCell2();
            cell.Matrix = matrix;
            XmlElement nodeElement = node as XmlElement;
            if (nodeElement.HasAttribute("x")) cell.Left = Convert.ToInt32(node.Attributes["x"].Value);
            if (nodeElement.HasAttribute("w")) cell.Width = Convert.ToInt32(node.Attributes["w"].Value);
            if (nodeElement.HasAttribute("h")) cell.Height = Convert.ToInt32(node.Attributes["h"].Value);
            if (nodeElement.HasAttribute("pid")) cell.PageId = Convert.ToInt32(node.Attributes["pid"].Value);
            if (nodeElement.HasAttribute("cid")) cell.ComponentId = Convert.ToInt32(node.Attributes["cid"].Value);
            if (nodeElement.HasAttribute("sid")) cell.cellStyleId = Convert.ToInt32(node.Attributes["sid"].Value);
            if (nodeElement.HasAttribute("t")) cell.Text = Convert.ToString(node.Attributes["t"].Value);
            line.Cells[cell.Left] = cell;
        }

        private void ReadMatrixBorders(XmlNode node, int[] borders)
        {
            XmlElement nodeElement = node as XmlElement;
            int offs = 0;
            if (nodeElement.HasAttribute("x")) offs = Convert.ToInt32(node.Attributes["x"].Value);
            string text = Convert.ToString(node.Attributes["v"].Value);
            string[] values = text.Split(new char[] { ',' });
            for (int index = 0; index < values.Length; index++)
            {
                if (values[index].Length > 0)
                {
                    borders[offs + index] = Convert.ToInt32(values[index]);
                }
            }
        }

        private void ReadMatrixBookmark(XmlNode node, string[] bookmarks)
        {
            XmlElement nodeElement = node as XmlElement;
            int offs = 0;
            if (nodeElement.HasAttribute("x")) offs = Convert.ToInt32(node.Attributes["x"].Value);
            int width = 0;
            if (nodeElement.HasAttribute("w")) width = Convert.ToInt32(node.Attributes["w"].Value);
            string bookmark = Convert.ToString(node.Attributes["t"].Value);
            for (int index = 0; index < width; index++)
            {
                bookmarks[offs + index] = bookmark;
            }
        }

        private void ReadMatrixCellStyles(XmlNode node, int[] cellStyles)
        {
            XmlElement nodeElement = node as XmlElement;
            int offs = 0;
            if (nodeElement.HasAttribute("x")) offs = Convert.ToInt32(node.Attributes["x"].Value);
            string text = Convert.ToString(node.Attributes["v"].Value);
            string[] values = text.Split(new char[] { ',' });
            for (int index = 0; index < values.Length; index++)
            {
                if (values[index].Length > 0)
                {
                    cellStyles[offs + index] = Convert.ToInt32(values[index]);
                }
            }
        }
        #endregion

        #region Load/Save segment
        public void SaveSegment(StiMatrixCacheSegment segment)
        {
            string path = StiMatrixCache.GetCacheSegmentName(cachePath, segment.CacheGuid);

            //if (this.SavePageToCache != null)
            //{
            //    SavePageToCache(page, new StiSaveLoadPageEventArgs(page, this.IndexOf(page), path));
            //}
            //else
            //{
            StiFileUtils.ProcessReadOnly(path);
            using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                WriteSegment(stream, segment);
                stream.Flush();
                stream.Close();
            }
            //}
        }

        public void LoadSegment(StiMatrixCacheSegment segment)
        {
            string path = StiMatrixCache.GetCacheSegmentName(cachePath, segment.CacheGuid);

            //if (this.LoadPageFromCache != null)
            //{
            //    LoadPageFromCache(page, new StiSaveLoadPageEventArgs(page, this.IndexOf(page), path));
            //}
            //else
            //{
            if (File.Exists(path))
            {
                using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    ReadSegment(stream, segment);
                    stream.Flush();
                    stream.Close();
                }
            }
            //}
        }
        #endregion

        #region Clear
        public void Clear()
        {
            if ((matrix != null) && matrix.useCacheMode)
            {
                // !!! закомментировано для тестирования, чтобы не сразу стиралось
                StiMatrixCache.DeleteCache(cachePath);
            }

            matrix = null;
            segments = null;
            if (quickAccessSegments != null)
            {
                quickAccessSegments.Clear();
            }
        }
        #endregion

        public StiMatrixCacheManager(StiMatrix matrix, int width, int height, int maxPageHeight)
        {
            this.matrix = matrix;
            this.matrixWidth = width;
            this.matrixHeight = height;

            //empiric value, used AmountOfProcessedPagesForStartGCCollect for case if page contain only few component or one image
            segmentHeight = StiOptions.Engine.ReportCache.AmountOfProcessedPagesForStartGCCollect * 3;  

            if (maxPageHeight * 3 > segmentHeight) segmentHeight = maxPageHeight * 3;

            if (matrix.useCacheMode)
            {
                cachePath = StiMatrixCache.CreateNewCache();
            }
        }
    }
}
