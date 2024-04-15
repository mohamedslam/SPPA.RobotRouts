#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	Stimulsoft.Report Library										}
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.SaveLoad
{
	/// <summary>
	/// Describes the class that allows to save / load documents.
	/// </summary>
	public class StiXmlDocumentSLService : StiDocumentSLService
	{
		#region StiService override
		/// <summary>
		/// Gets a service type.
		/// </summary>
		public override Type ServiceType => typeof(StiDocumentSLService);
        #endregion

	    #region Methods
        /// <summary>
        /// Saves the current document into the stream.
        /// </summary>
        /// <param name="report">Rendered report for saving.</param>
        /// <param name="stream">Stream to save documents.</param>
        public override void Save(StiReport report, Stream stream)
		{
            var originalReport = report;
            if (report.RenderedPages.Count > 0) originalReport = report.RenderedPages[0].Report;

            var document = new StiDocument(report);
            try
            {
                foreach (StiPage page in document.Pages)
                {
                    page.Report = null;
                    page.Document = document;
                }

                // Prepare resources
                document.Resources = new StiResourcesCollection();
                foreach (StiResource resource in report.Dictionary.Resources)
                {
                    if (resource.AvailableInTheViewer || 
                        resource.Type == StiResourceType.FontEot ||
                        resource.Type == StiResourceType.FontOtf ||
                        resource.Type == StiResourceType.FontTtc ||
                        resource.Type == StiResourceType.FontTtf ||
                        resource.Type == StiResourceType.FontWoff ||
                        resource.Type == StiResourceType.Map)
                        document.Resources.Add(resource);
                }

                var sr = new StiSerializing(new StiReportObjectStringConverter());
                RegPropertyNames(sr);
                try
                {
                    sr.Serializing += OnSaving;
                    sr.SortProperties = false;
                    sr.CheckSerializable = true;

                    if (!(StiOptions.Engine.DocumentSavingOptimization || report.RenderedPages.CacheMode) || (document.Pages.Count < 1))
                    {
                        sr.Serialize(document, stream, "StiDocument", StiSerializeTypes.SerializeToDocument);
                    }
                    else
                    {
                        var position = stream.Position;
                        var result = SaveOptimized(stream, document, sr);
                        if (!result)
                        {
                            // Use standard mode
                            stream.Seek(position, SeekOrigin.Begin);
                            sr.Serialize(document, stream, "StiDocument", StiSerializeTypes.SerializeToDocument);
                        }
                        buffer = null;
                    }
                }
                finally
                {
                    sr.Serializing -= OnSaving;
                }
            }
            finally
            {
                foreach (StiPage page in document.Pages)
                {
                    page.Report = originalReport;
                    page.Document = null;
                }
            }
		}

        private static object lockObj = new object();

        public static void RegPropertyNames(StiSerializing sr)
		{
            sr.IsDocument = true;
            if (sr.StringToProperty.Count == 0)
            {
                lock (lockObj)
                {
                    //sr.ClearPropertyString();
                    sr.AddStringProperty("Brush", "bh");
                    sr.AddStringProperty("TextBrush", "tb");
                    sr.AddStringProperty("Border", "br");
                    sr.AddStringProperty("Dock", "d");
                    sr.AddStringProperty("Font", "fn");
                    sr.AddStringProperty("Text", "text");
                    sr.AddStringProperty("Rectangle", "rc");
                    sr.AddStringProperty("TextOptions", "to");
                    sr.AddStringProperty("VertAlignment", "va");
                    sr.AddStringProperty("HorAlignment", "ha");
                    sr.AddStringProperty("TextFormat", "tf");
                }
            }
		}
        
		/// <summary>
		/// Loads a document from the stream.
		/// </summary>
		/// <param name="report">Report in which loading will be done.</param>
		/// <param name="stream">Stream to load document.</param>
		public override void Load(StiReport report, Stream stream)
		{
			try
			{
				var sr = new StiSerializing(new StiReportObjectStringConverter(true));
                RegPropertyNames(sr);
                var document = new StiDocument(report);
				try
				{
					sr.Deserializing += OnLoading;

                    if (!StiOptions.Engine.DocumentLoadingOptimization && report.ReportCacheMode != StiReportCacheMode.On)
                    {
                        sr.Deserialize(document, stream, "StiDocument");
                    }
                    else
                    {
                        var position = stream.Position;
                        var result = LoadOptimized(stream, document, sr);
                        if (!result)
                        {
                            //Use standard mode
                            stream.Seek(position, SeekOrigin.Begin); 
                            sr.Deserialize(document, stream, "StiDocument");
                        }
                    }

					report.IsRendered = true;
					report.NeedsCompiling = false;
				}
				finally
				{
					sr.Deserializing -= OnLoading;
				}
			}
			finally
			{
				foreach (StiPage page in report.RenderedPages)
				{
					page.Report = report;

					var comps = page.GetComponents();

					foreach (StiComponent comp in comps)
					{
						comp.Page = page;
					}
				}
			}
        }

        private bool LoadOptimized(Stream stream, StiDocument document, StiSerializing sr)
        {
            #region Prepare offsets
            var pagesBegin = FindIndexOfForward(stream, "<Pages isList=", 0);
            if (pagesBegin == -1) return false;

            var pages = new List<long>();

            var lastPagePosition = pagesBegin;
            while (true)
            {
                var tempBegin = FindIndexOfForward(stream, "type=\"Page\" isKey=\"true\"", lastPagePosition);
                if (tempBegin == -1) break; //no pages

                var pageBegin = FindIndexOfBackward("<", tempBegin);
                if (pageBegin == -1) return false;

                var pageName = GetPageName(pageBegin + 1);
                if (pageName == null) return false;

                pageBegin = FindIndexOfForward(stream, ">", pageBegin);
                if (pageBegin == -1) return false;
                pageBegin++;

                var pageEnd = FindIndexOfForward(stream, pageName, pageBegin);
                if (pageEnd == -1) return false;

                pages.Add(pageBegin);
                pages.Add(pageEnd);

                lastPagePosition = pageEnd + pageName.Length;
            }

            //rewind to the last page
            stream.Seek(lastPagePosition, SeekOrigin.Begin);
            lengthData = stream.Read(buffer, sizeBuffer, sizeBuffer * 2);
            startBuffer = lastPagePosition;

            var pagesEnd = FindIndexOfForward(stream, "</Pages>", lastPagePosition + 1);
            if (pagesEnd == -1) return false;
            pagesEnd += "</Pages>".Length;
            #endregion

            #region Read data
            var msBegin = new MemoryStream();
            CopyStreamBlock(stream, 0, msBegin, -1, (int)pagesBegin);

            var msEnd = new MemoryStream();
            CopyStreamBlock(stream, pagesEnd, msEnd, -1, (int)(stream.Length - pagesEnd));

            var startBytes = Encoding.UTF8.GetBytes("<Pages isList=\"true\" count=\"1\">\r\n<Page1 Ref=\"2\" type=\"Page\" isKey=\"true\">");
            var endBytes = Encoding.UTF8.GetBytes("</Page1>\r\n</Pages>");

            var tempReport = StiActivator.CreateObject(StiOptions.Engine.BaseReportType) as StiReport;
            var pagesCollection = new StiPagesCollection(tempReport);

            if (document.Report != null && document.Report.ReportCacheMode != StiReportCacheMode.Off)
            {
                if (document.Report.ReportCacheMode == StiReportCacheMode.On || pages.Count > StiOptions.Engine.ReportCache.LimitForStartUsingCache * 2)
                {
                    document.Report.ReportCachePath = StiReportCache.CreateNewCache();
                    tempReport.ReportCachePath = document.Report.ReportCachePath;

                    document.Pages.CacheMode = true;
                    document.Pages.CanUseCacheMode = false;
                    pagesCollection.CacheMode = true;
                    pagesCollection.CanUseCacheMode = true;
                    pagesCollection.CheckCacheL2();
                }
            }

            //Optimization lists
            var repositoryItems = new List<StiRepositoryItems>();
            var fonts = new List<Font>();
            var componentPlacements = new List<string>();
            var guids = new List<string>();
            var brushes = new List<StiBrush>();
            var borders = new List<StiBorder>();

            var index = 0;
            while (index < pages.Count)
            {
                var pageBegin = pages[index];
                var pageEnd = pages[index + 1];

                var pageMS = new MemoryStream();
                msBegin.WriteTo(pageMS);
                pageMS.Write(startBytes, 0, startBytes.Length);
                CopyStreamBlock(stream, pageBegin, pageMS, -1, (int)(pageEnd - pageBegin));
                pageMS.Write(endBytes, 0, endBytes.Length);
                msEnd.WriteTo(pageMS);
                pageMS.Seek(0, SeekOrigin.Begin);

                sr.Deserialize(document, pageMS, "StiDocument");

                if (document.Pages.Count == 0) return false;
                pagesCollection.Add(document.Pages[0]);

                #region Page optimization
                var comps = document.Pages[0].GetComponentsList();
                for (var index1 = 0; index1 < comps.Count; index1++)
                {
                    var comp = comps[index1];

                    if (comp.IsPropertiesInitialized())
                    {
                        var flag = true;
                        foreach (var repositoryItem in repositoryItems)
                        {
                            if (comp.Properties.Equals(repositoryItem))
                            {
                                comp.Properties = repositoryItem;
                                flag = false;
                                break;
                            }
                        }
                        if (flag) repositoryItems.Add(comp.Properties);
                    }

                    if (comp.ComponentPlacement != null)
                    {
                        int pos = componentPlacements.IndexOf(comp.ComponentPlacement);
                        if (pos == -1)
                            componentPlacements.Add(comp.ComponentPlacement);
                        else
                            comp.ComponentPlacement = componentPlacements[pos];
                    }

                    if (comp.Guid != null)
                    {
                        int pos = guids.IndexOf(comp.Guid);
                        if (pos == -1)
                        {
                            guids.Add(comp.Guid);
                        }
                        else
                        {
                            comp.Guid = null;
                            comp.Guid = guids[pos];
                        }
                    }

                    var compBrush = comp as IStiBrush;
                    if (compBrush != null && compBrush.Brush != null)
                    {
                        int pos = brushes.IndexOf(compBrush.Brush);
                        if (pos == -1)
                            brushes.Add(compBrush.Brush);
                        else
                            compBrush.Brush = brushes[pos];
                    }

                    var compTextBrush = comp as IStiTextBrush;
                    if (compTextBrush != null && compTextBrush.TextBrush != null)
                    {
                        int pos = brushes.IndexOf(compTextBrush.TextBrush);
                        if (pos == -1)
                            brushes.Add(compTextBrush.TextBrush);
                        else
                            compTextBrush.TextBrush = brushes[pos];
                    }

                    var compFont = comp as IStiFont;
                    if (compFont != null && compFont.Font != null)
                    {
                        int pos = fonts.IndexOf(compFont.Font);
                        if (pos == -1)
                            fonts.Add(compFont.Font);
                        else
                            compFont.Font = fonts[pos];
                    }

                    var compBorder = comp as IStiBorder;
                    if (compBorder != null)
                    {
                        int pos = borders.IndexOf(compBorder.Border);
                        if (pos == -1)
                            borders.Add(compBorder.Border);
                        else
                            compBorder.Border = borders[pos];
                    }

                    if (comp.Interaction != null && comp.Interaction.IsDefault)
                        comp.Interaction = null;
                }
                #endregion

                index += 2;
            }

            var ms = new MemoryStream();
            msBegin.WriteTo(ms);
            msEnd.WriteTo(ms);
            ms.Seek(0, SeekOrigin.Begin);
            sr.Deserialize(document, ms, "StiDocument");

            document.Pages.Clear();
            document.Pages.AddRange(pagesCollection);
            tempReport.ReportCachePath = string.Empty;
            tempReport.Dispose();
            #endregion

            return true;
        }

        private bool SaveOptimized(Stream stream, StiDocument document, StiSerializing sr)
        {
            var oldPages = document.Report.RenderedPages;

            try
            {
                var tempPages = new StiPagesCollection(document.Report);
                document.Report.RenderedPages = tempPages;

                #region Prepare begin/end of document
                var ms = new MemoryStream();
                sr.Serialize(document, ms, "StiDocument", StiSerializeTypes.SerializeToDocument);
                ms.Seek(0, SeekOrigin.Begin);

                var pagesBegin = FindIndexOfForward(ms, "<Pages isList=", 0);
                if (pagesBegin == -1) return false;

                var msBegin = new MemoryStream();
                CopyStreamBlock(ms, 0, msBegin, -1, (int)pagesBegin);

                var pagesEnd = FindIndexOfForward(stream, "<", pagesBegin + 1);
                if (pagesEnd == -1) return false;

                var msEnd = new MemoryStream();
                CopyStreamBlock(ms, pagesEnd, msEnd, -1, (int)(ms.Length - pagesEnd));
                ms.Close();

                var startBytes = Encoding.UTF8.GetBytes($"<Pages isList=\"true\" count=\"{oldPages.Count}\">\r\n  ");
                var endBytes = Encoding.UTF8.GetBytes("</Pages>\r\n  ");
                #endregion

                tempPages.Add(StiActivator.CreateObject("Stimulsoft.Report.Components.StiPage") as StiPage);

                #region Write data
                msBegin.WriteTo(stream);
                stream.Write(startBytes, 0, startBytes.Length);

                var tempMs = new MemoryStream();
                var baseOldRef = CorrectRefs(msBegin, tempMs, 0, (int)msBegin.Length, 0, 0);
                tempMs.Close();
                var baseNewRef = baseOldRef;

                for (var index = 0; index < oldPages.Count; index++)
                {
                    oldPages.GetPage(oldPages[index]);
                    tempPages[0] = oldPages[index];

                    buffer = null;
                    ms = new MemoryStream();
                    sr.Serialize(document, ms, "StiDocument", StiSerializeTypes.SerializeToDocument);
                    ms.Seek(0, SeekOrigin.Begin);

                    pagesBegin = FindIndexOfForward(ms, "<Pages isList=", 0);
                    if (pagesBegin == -1) return false;
                    pagesBegin = FindIndexOfForward(ms, "<", pagesBegin + 1);
                    if (pagesBegin == -1) return false;

                    pagesEnd = FindIndexOfForward(ms, "</Pages>", pagesBegin);
                    if (pagesEnd == -1) return false;

                    stream.WriteByte(0x20);
                    stream.WriteByte(0x20);

                    baseNewRef = CorrectRefs(ms, stream, pagesBegin, pagesEnd, baseNewRef, baseOldRef);

                    ms.Close();
                }

                stream.Write(endBytes, 0, endBytes.Length);

                CorrectRefs(msEnd, stream, 0, (int)msEnd.Length, baseNewRef, baseOldRef);
                #endregion
            }
            finally
            {
                document.Report.RenderedPages = oldPages;
            }
            return true;
        }

        private string GetPageName(long position)
        {
            position += sizeBuffer - startBuffer;
            var sb = new StringBuilder();
            while (!char.IsWhiteSpace((char)buffer[position]))
            {
                sb.Append((char)buffer[position]);
                position++;
            }
            if (sb.Length == 0) return null;
            return $"</{sb}>";
        }

        private void CopyStreamBlock(Stream inputStream, long inputPosition, Stream outputStream, long outputPosition, int length)
        {
            inputStream.Seek(inputPosition, SeekOrigin.Begin);
            if (outputPosition != -1) outputStream.Seek(outputPosition, SeekOrigin.Begin);

            var buf = new byte[sizeBuffer * 2];
            while (length > 0)
            {
                var len = length;
                if (len > sizeBuffer * 2) len = sizeBuffer * 2;
                length -= len;
                len = inputStream.Read(buf, 0, len);
                if (len > 0)
                    outputStream.Write(buf, 0, len);
                else
                    break;
            }
        }

        private long FindIndexOfForward(Stream stream, string expr, long offset)
        {
            if (buffer == null)
            {
                buffer = new byte[sizeBuffer * 3];
                stream.Seek(0, SeekOrigin.Begin);
                lengthData = stream.Read(buffer, sizeBuffer, sizeBuffer * 2);
                startBuffer = 0;
            }

            offset -= startBuffer;

            while (true)
            {
                //find string, without encoding
                var findLen = lengthData;
                if (findLen > sizeBuffer) findLen = sizeBuffer;
                var bb = (byte)expr[0];
                for (var index = offset; index < findLen; index++)
                {
                    if (buffer[sizeBuffer + index] == bb)
                    {
                        var flag = true;
                        for (var index2 = 1; index2 < expr.Length; index2++)
                        {
                            if (buffer[sizeBuffer + index + index2] != (byte)expr[index2])
                            {
                                flag = false;
                                break;
                            }
                        }

                        if (flag)
                            return startBuffer + index;
                    }
                }

                //end of stream, not found
                if (lengthData < sizeBuffer)
                {
                    return -1;
                }

                //move segment
                Array.Copy(buffer, sizeBuffer, buffer, 0, sizeBuffer * 2);
                lengthData -= sizeBuffer;
                startBuffer += sizeBuffer;
                offset = 0;

                if (lengthData == sizeBuffer)
                {
                    //read next segment
                    lengthData = stream.Read(buffer, sizeBuffer * 2, sizeBuffer);
                    lengthData += sizeBuffer;
                }
            }
        }

        private long FindIndexOfBackward(string expr, long offset)
        {
            if (buffer == null)
            {
                return -1;
            }

            offset -= startBuffer;

            //Find string, without encoding
            var bb = (byte)expr[0];
            var count = offset;
            if (startBuffer > 0) count += sizeBuffer;
            while (count >= 0)
            {
                if (buffer[sizeBuffer + offset] == bb)
                {
                    var flag = true;
                    for (var index2 = 1; index2 < expr.Length; index2++)
                    {
                        if (buffer[sizeBuffer + offset + index2] != (byte)expr[index2])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        return startBuffer + offset;
                    }
                }
                count--;
                offset--;
            }

            //not found
            return -1;
        }

        private int CorrectRefs(MemoryStream input, Stream output, long startPosition, long endPosition, int baseNewRef, int baseOldRef)
        {
            input.Seek(startPosition, SeekOrigin.Begin);

            long pos;
            int newRef;
            var maxRef = 0;
            var lastSym = 0;
            while (input.Position < endPosition)
            {
                var sym = input.ReadByte();
                pos = input.Position;
                if (sym == 'R' && lastSym == ' ')
                {
                    if (input.ReadByte() == 'e' && input.ReadByte() == 'f' && input.ReadByte() == '=' && input.ReadByte() == '"')
                    {
                        var reff = GetRefNum(input);
                        if (reff != -1)
                        {
                            newRef = reff - baseOldRef + baseNewRef;
                            if (newRef > maxRef) 
                                maxRef = newRef;
                            WriteStringToStream(output, $"Ref=\"{newRef}\"");
                            lastSym = 0;
                            continue;
                        }
                    }
                    input.Seek(pos, SeekOrigin.Begin);
                }
                else if (sym == 'i' && lastSym == ' ')
                {
                    if (input.ReadByte() == 's' && input.ReadByte() == 'R' && input.ReadByte() == 'e' && 
                        input.ReadByte() == 'f' && input.ReadByte() == '=' && input.ReadByte() == '"')
                    {
                        var reff = GetRefNum(input);
                        if (reff != -1)
                        {
                            newRef = reff - baseOldRef + baseNewRef;
                            if (newRef > maxRef) 
                                maxRef = newRef;
                            WriteStringToStream(output, $"isRef=\"{newRef}\"");
                            lastSym = 0;
                            continue;
                        }
                    }
                    input.Seek(pos, SeekOrigin.Begin);
                }
                output.WriteByte((byte)sym);
                lastSym = sym;
            }

            return maxRef;
        }

        private static void WriteStringToStream(Stream ms, string st)
        {
            foreach (var ch in st)
            {
                ms.WriteByte((byte)ch);
            }
        }

        private static int GetRefNum(MemoryStream ms)
        {
            var sb = new StringBuilder();
            var sym = 0;
            while ((ms.Position < ms.Length) && (char.IsDigit((char)(sym = ms.ReadByte()))))
            {
                sb.Append((char)sym);
            }

            if (sym == '\"' && sb.Length > 0)
                return int.Parse(sb.ToString());

            return -1;
        }

        /// <summary>
        /// Returns actions available for the provider.
        /// </summary>
        /// <returns>Available actions.</returns>
        public override StiSLActions GetAction()
		{
			return StiSLActions.Load | StiSLActions.Save;
		}

		/// <summary>
		/// Returns a filter for the provider.
		/// </summary>
		/// <returns>String with filter.</returns>
		public override string GetFilter()
		{
			return StiLocalization.Get("FileFilters", "DocumentFiles");
		}
        #endregion

        #region Consts
        private const int sizeBuffer = 32768;
        #endregion

        #region Fields
        private byte[] buffer;
        private long startBuffer;
	    private int lengthData;
        #endregion

        #region Properties
        /// <summary>
        /// If the provider handles with multitude of files then true. If does not then false.
        /// </summary>
        public override bool MultiplePages => false;
	    #endregion
    }
}
