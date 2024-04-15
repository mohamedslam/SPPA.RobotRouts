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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Licenses;
using Stimulsoft.Base.Plans;
using Stimulsoft.Report.Components;
using System;
using System.CodeDom;
using System.Collections;
using System.Linq;

#if NETSTANDARD
using Stimulsoft.System.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif

namespace Stimulsoft.Report.Engine
{
    internal sealed class StiPostProcessProviderV2
    {
        #region Constants
        private const string nullGuid = "nullGuid";
		#endregion

		#region Methods
		internal static void PostProcessPages(StiPagesCollection pages)
		{
            var pageNumber = 0; 
            var totalPageCount = 0;
            foreach (StiPage page in pages)
            {
                totalPageCount += page.SegmentPerWidth * page.SegmentPerHeight;
            }

            var isFirstPage = new bool[totalPageCount + 1];
            var isLastPage = new bool[totalPageCount + 1];
            if (totalPageCount > 0) isFirstPage[1] = true;
            isLastPage[totalPageCount] = true;

            StiPageNumberHelper pageNumbers = null;
            if (pages.Report != null && pages.Report.Engine != null && pages.Report.Engine.PageNumbers != null)
            {
                if (pages.Report.ReportPass == StiReportPass.Second)
                    pageNumbers = pages.Report.Engine.PageNumbers;

                //prepare isFirstPage and isLastPage tables
                var pageNumbers2 = pages.Report.Engine.PageNumbers;
                pageNumber = 1;
                var maxPageNumber = Math.Min(pages.Count, pageNumbers2.PageNumbers.Count);
                for (var index = 0; index < maxPageNumber; index++)
                {
                    if (pageNumbers2.PageNumbers[index].ResetPageNumber)
                    {
                        isFirstPage[pageNumber] = true;
                        isLastPage[pageNumber - 1] = true;
                    }
                    pageNumber += pageNumbers2.PageNumbers[index].SegmentPerWidth * pageNumbers2.PageNumbers[index].SegmentPerHeight;
                }
            }

			pageNumber = 1;
			foreach (StiPage page in pages)
			{
			    if (pageNumbers != null && pageNumbers.IsFinished)
			    {
			        pageNumber = pageNumbers.GetPageNumber(page);
			        totalPageCount = pageNumbers.GetTotalPageCount(page);
			        PostProcessPage(page, pageNumber == 1, pageNumber == totalPageCount);
			    }
			    else
			        PostProcessPage(page, isFirstPage[pageNumber], isLastPage[pageNumber]);

			    pageNumber += page.SegmentPerWidth * page.SegmentPerHeight;
			}

			PostProcessPrimitives(pages);
		}
	
		private static void RemoveAllPointPrimitives(StiContainer container)
		{
			var index = 0;
			while (index < container.Components.Count)
			{
				if (container.Components[index] is StiPointPrimitive)
				    container.Components.RemoveAt(index);
				else
				{
					var cont = container.Components[index] as StiContainer;
					if (cont != null)
					    RemoveAllPointPrimitives(cont);

					index++;
				}			
			}
		}

		internal static void PostProcessPrimitives(StiPage page)
		{
		    var pages = new StiPagesCollection(null) { page };
		    PostProcessPrimitives(pages);
		}

		internal static void PostProcessPrimitives(StiPagesCollection pages)
		{
			var startPointsHash = new Hashtable();
			var endPointsHash = new Hashtable();

			var lines = new Hashtable();
			var endPoints = new ArrayList();

			foreach (StiPage page in pages)
			{
				PostProcessPrimitivesInContainer(page, pages, ref startPointsHash, ref endPointsHash,
					ref lines, ref endPoints);
			}

			foreach (StiPage page in pages)
			{
				RemoveAllPointPrimitives(page);
			}
		}

		internal static void PostProcessPrimitivesInContainer(StiContainer container)
		{
			var startPointsHash = new Hashtable();
			var endPointsHash = new Hashtable();

			var lines = new Hashtable();
			var endPoints = new ArrayList();

			PostProcessPrimitivesInContainer(container, null, ref startPointsHash, ref endPointsHash,
				ref lines, ref endPoints);

			RemoveAllPointPrimitives(container);			
		}


		internal static void PostProcessPrimitivesInContainer(StiContainer container, StiPagesCollection pages,
			ref Hashtable startPointsHash, ref Hashtable endPointsHash, ref Hashtable lines, ref ArrayList endPoints)
		{
			var comps = new StiComponentsCollection();
			comps.AddRange(container.Components);

			#region Find All Cross Lines
			foreach (StiComponent comp in comps)
			{
				var line = comp as StiCrossLinePrimitive;
				if (line != null && line.Guid != null && line.Guid != nullGuid)
				{
					lines[line.Guid] = line;
					line.Parent.Components.Remove(comp);
				}
			}
			#endregion

			#region Process Cross Lines
			if (lines.Count > 0)
			{
				comps = container.GetComponents();
				endPoints.Clear();

				foreach (StiComponent comp in comps)
				{
					if (!(comp is StiPrimitive)) continue;

					var startPoint = comp as StiStartPointPrimitive;
					if (startPoint != null)
					{
						if (startPoint.ReferenceToGuid == null) continue;

						var panel = container as StiPanel;

						var list = startPointsHash[startPoint.ReferenceToGuid] as ArrayList;
						if (list == null)
						{
							list = new ArrayList();
							startPointsHash[startPoint.ReferenceToGuid] = list;
							
							if (panel != null && panel.Columns > 1)
							    list.Add(startPoint);
						}

						if (panel != null && panel.Columns < 2 || panel == null)
						    list.Add(startPoint);

						continue;
					}

					var endPoint = comp as StiEndPointPrimitive;
					if (endPoint == null) continue;

					if (!ProcessOneEndPoint(startPointsHash, lines, container, pages, endPoint))
					    endPoints.Add(endPoint);
				}

				foreach (StiEndPointPrimitive endPoint in endPoints)
				{
					ProcessOneEndPoint(startPointsHash, lines, container, pages, endPoint);
				}
			}
			#endregion
		}

		private static bool ProcessOneEndPoint(
            Hashtable startPointsHash, Hashtable lines, StiContainer page, 
            StiPagesCollection pages, StiEndPointPrimitive endPoint)
		{
			if (endPoint.ReferenceToGuid == null)return false;

			var startPointList = startPointsHash[endPoint.ReferenceToGuid] as ArrayList;
			if (startPointList == null || startPointList.Count == 0)return false;

			startPointsHash[endPoint.ReferenceToGuid] = null;

            var firstPoint = startPointList[0] as StiStartPointPrimitive;

            var crossLine = lines[firstPoint.ReferenceToGuid] as StiCrossLinePrimitive;
			if (crossLine == null)return true;

            var startPoint = (StiStartPointPrimitive)startPointList[0];
            ProcessOnePrimitive(pages, page, crossLine, startPoint, null, endPoint);

			return true;
		}

		private static void ProcessOnePrimitive(
            StiPagesCollection pages, StiContainer page, StiCrossLinePrimitive crossLine, 
            StiPrimitive startPoint, StiPrimitive nextStartPoint, StiPrimitive endPoint)
		{
			var startPos = startPoint.ComponentToPage(new PointD(startPoint.Left, startPoint.Top));
			var endPos = endPoint.ComponentToPage(new PointD(endPoint.Left, endPoint.Top));

            var startIndexColumn = ((StiPointPrimitive)startPoint).StoredColumn;
            var endIndexColumn = ((StiPointPrimitive)endPoint).StoredColumn;
            startIndexColumn = Math.Max(1, startIndexColumn);
            endIndexColumn = Math.Max(1, endIndexColumn);

			#region Primitive from one page to other page
			if (startPoint.Page != endPoint.Page || startIndexColumn != endIndexColumn)
			{
				var startIndexPage = pages == null ? 1 : pages.IndexOf(startPoint.Page);
				var endIndexPage = pages == null ? 1 : pages.IndexOf(endPoint.Page);

                var nextStartIndexPage = -1;
			    if (nextStartPoint != null && pages != null)
			        nextStartIndexPage = pages.IndexOf(nextStartPoint.Page);

			    for (var indexPage = startIndexPage; indexPage <= endIndexPage; indexPage++)
				{
				    if (startIndexPage != endIndexPage && nextStartPoint != null && 
				        nextStartIndexPage == endIndexPage && indexPage == endIndexPage) continue;

				    var page2 = pages[indexPage];
                                        
					if (page2 != startPoint.Page)
					    startIndexColumn = 1;

					if (page2 != endPoint.Page)
					    endIndexColumn = Math.Max(page2.Columns, endIndexColumn);

					double position = 0;
					for (var indexColumn = startIndexColumn; indexColumn <= endIndexColumn; indexColumn++)
					{
						var topPosition = page2.PageInfoV2.PositionFromTop;
						var bottomPosition = page2.PageInfoV2.PositionFromBottom;

						var columnWidth = page2.GetColumnWidth() + page2.ColumnGaps;

						var stX = startPos.X;
						var stY = startPos.Y;
						var enX = endPos.X;
						var enY = endPos.Y;

						#region Column Correction
						if (startIndexColumn != endIndexColumn)
						{
							enX -= columnWidth * (Math.Min(endIndexColumn - startIndexColumn, page2.Columns));
							while (enX < stX && columnWidth > 0)
							{
								enX += columnWidth;
							}
						}
						#endregion

						if (indexPage == startIndexPage && startIndexColumn == indexColumn)
						{
							AddPrimitive(crossLine, 
								new PointD(position + stX, stY),
								new PointD(position + enX, bottomPosition), page2);
						}
						else if (indexPage == endIndexPage && endIndexColumn == indexColumn)
						{
							AddPrimitive(crossLine, 
								new PointD(position + stX, topPosition), 
								new PointD(position + enX, enY),
								page2);
						}
						else 
						{
							AddPrimitive(crossLine, 
								new PointD(position + stX, topPosition), 
								new PointD(position + enX, bottomPosition), page2);
						}

						position += columnWidth;
					}
				}
			}
			#endregion
	
			//Primitive with start and end point on one page
			else
			    AddPrimitive(crossLine, startPos, endPos, page);
		}

		internal static void AddPrimitive(
            StiCrossLinePrimitive crossLine, PointD startPos, PointD endPos, StiContainer page)
		{
            if (crossLine is StiRoundedRectanglePrimitive)
            {
                var newCrossLine = crossLine.Clone() as StiCrossLinePrimitive;
                newCrossLine.Guid = nullGuid;

                newCrossLine.Left = startPos.X;
                newCrossLine.Top = startPos.Y;
                newCrossLine.Width = endPos.X - startPos.X;
                newCrossLine.Height = endPos.Y - startPos.Y;

                page.Components.Add(newCrossLine);
            }
			else if (crossLine is StiRectanglePrimitive)
			{
				if (((StiRectanglePrimitive)crossLine).LeftSide)
				{
			        page.Components.Add(new StiVerticalLinePrimitive
			        {
			            Name = crossLine.Name + "_Left",
			            Guid = nullGuid,
			            Page = crossLine.Page,
			            Left = startPos.X,
			            Top = startPos.Y,
			            Height = endPos.Y - startPos.Y,
			            Color = crossLine.Color,
			            Style = crossLine.Style,
			            Size = crossLine.Size
			        });
				}

				if (((StiRectanglePrimitive)crossLine).RightSide)
				{
				    page.Components.Add(new StiVerticalLinePrimitive
				    {
				        Name = crossLine.Name + "_Right",
				        Guid = nullGuid,
				        Page = crossLine.Page,
				        Left = endPos.X,
				        Top = startPos.Y,
				        Height = endPos.Y - startPos.Y,
				        Color = crossLine.Color,
				        Style = crossLine.Style,
				        Size = crossLine.Size
				    });
				}

				if (((StiRectanglePrimitive)crossLine).TopSide)
				{
				    page.Components.Add(new StiHorizontalLinePrimitive
				    {
				        Name = crossLine.Name + "_Top",
				        Page = crossLine.Page,
				        Left = startPos.X,
				        Top = startPos.Y,
				        Height = crossLine.Page.Unit.ConvertFromHInches(1d),
				        Width = endPos.X - startPos.X,
				        Color = crossLine.Color,
				        Style = crossLine.Style,
				        Size = crossLine.Size
				    });
				}

				if (((StiRectanglePrimitive)crossLine).BottomSide)
				{
				    page.Components.Add(new StiHorizontalLinePrimitive
				    {
				        Name = crossLine.Name + "_Bottom",
				        Page = crossLine.Page,
				        Left = startPos.X,
				        Top = endPos.Y,
				        Height = crossLine.Page.Unit.ConvertFromHInches(1d),
				        Width = endPos.X - startPos.X,
				        Color = crossLine.Color,
				        Style = crossLine.Style,
				        Size = crossLine.Size
				    });
				}
			}
			else
			{
				var newCrossLine = crossLine.Clone() as StiCrossLinePrimitive;
				newCrossLine.Guid = nullGuid;
			
				newCrossLine.Left = startPos.X;
				newCrossLine.Top = startPos.Y;
				newCrossLine.Width = endPos.X - startPos.X;
				newCrossLine.Height = endPos.Y - startPos.Y;

				page.Components.Add(newCrossLine);
			}
		}

		internal static void CopyStyles(StiLinePrimitive dest, StiLinePrimitive source)
		{
			dest.Style = source.Style;
			dest.Color = source.Color;
			dest.Size = source.Size;
		}

        internal static void PostProcessPage(StiPage page, bool isFirstPage, bool isLastPage)
        {
            PostProcessPage(page, isFirstPage, isLastPage, true);
        }

		internal static void PostProcessPage(StiPage page, bool isFirstPage, bool isLastPage, bool clearPage)
		{
			page.PackService();

			PostProcessPrintOn(page, isFirstPage, isLastPage);

			var comps = page.GetComponents();
			page.DockToContainer();

			#region ShrinkFontToFitMinimumSize
			foreach (StiComponent comp in comps)
			{
				comp.DockStyle = StiDockStyle.None;
				comp.Page = page;
				comp.PackService();

				var text = comp as StiText;
			    if (text != null && text.ShrinkFontToFit)
			        text.Font = text.GetActualFont(text.Text, text.ShrinkFontToFitMinimumSize);
			}
			#endregion

			#region Prepare Parent Collection for PostProcessDuplicates
			var parentCont = new Hashtable();

			foreach (StiComponent comp in comps)
			{
			    if (comp is IStiText && ((IStiText) comp).ProcessingDuplicates != StiProcessingDuplicatesType.None)
			        parentCont[comp] = comp.Parent;

			    if (comp is StiImage && ((StiImage) comp).ProcessingDuplicates != StiImageProcessingDuplicatesType.None)
			        parentCont[comp] = comp.Parent;
			}
			#endregion

			//CheckCanShrinkLaterMark
			for (int index = comps.Count - 1; index >= 0; index--)
            {
				StiComponent comp = comps[index];
				if (comp.TagValue is string && (string)comp.TagValue == StiPageHelper.CheckCanShrinkLaterMark)
                {
					comp.CanShrink = true;
					var size = comp.GetActualSize();
					comp.Height = size.Height;
					comp.TagValue = null;
                }
            }

			if (clearPage)
			{
				page.ClearPage();

				if (isFirstPage || isLastPage)
                {
					#region Trial
#if CLOUD
            var isTrial = StiCloudPlan.IsTrial(page.Report != null ? page.Report.ReportGuid : null);
#elif SERVER
            var isTrial = StiVersionX.IsSvr;
#else
					var key = StiLicenseKeyValidator.GetLicenseKey();

					var isValidInDesigner = StiLicenseKeyValidator.IsValidInReportsDesignerOrOnPlatform(StiProductIdent.Net, key);
					var isTrial = !(isValidInDesigner && Base.Design.StiDesignerAppStatus.IsRunning || StiLicenseKeyValidator.IsValidOnNetFramework(key));

					if (!typeof(StiLicense).AssemblyQualifiedName.Contains(StiPublicKeyToken.Key))
						isTrial = true;

					#region IsValidLicenseKey
					if (!isTrial)
					{
						try
						{
							using (var rsa = new RSACryptoServiceProvider(512))
							using (var sha = new SHA1CryptoServiceProvider())
							{
								rsa.FromXmlString("<RSAKeyValue><Modulus>iyWINuM1TmfC9bdSA3uVpBG6cAoOakVOt+juHTCw/gxz/wQ9YZ+Dd9vzlMTFde6HAWD9DC1IvshHeyJSp8p4H3qXUKSC8n4oIn4KbrcxyLTy17l8Qpi0E3M+CI9zQEPXA6Y1Tg+8GVtJNVziSmitzZddpMFVr+6q8CRi5sQTiTs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
								isTrial = !rsa.VerifyData(key.GetCheckBytes(), sha, key.GetSignatureBytes());
							}
						}
						catch (Exception)
						{
							isTrial = true;
						}
					}
					#endregion
#endif

					if (isTrial && page.Report != null)
					{
						double imageWidth = page.Report.Unit.ConvertFromHInches(200d);
						double imageHeight = page.Report.Unit.ConvertFromHInches(52d);
						RectangleD rect = new RectangleD((page.Width - imageWidth) / 2f, page.Height - imageHeight /* + page.Margins.Bottom*/, imageWidth, imageHeight);
						StiImage image = new StiImage(rect) { Stretch = true, Printable = false };
						image.ImageBytes = Export.StiExportUtils.GetAdditionalData2(192);
						image.Name = StiNameValidator.CorrectName(StiGuidUtils.NewGuid().Substring(0, 8));

						page.Components.Add(image);
					}
					#endregion
				}
			}
			
			StiPostProcessDuplicatesHelper.PostProcessDuplicates(comps, parentCont);

			parentCont.Clear();
		}

		private static void PostProcessPrintOn(StiPage page, bool isFirstPage, bool isLastPage)
		{
            PostProcessPrintOn(page.Components, isFirstPage, isLastPage);			
		}

		private static void PostProcessPrintOn(StiComponentsCollection comps, bool isFirstPage, bool isLastPage)
		{
			var index = 0;
			while (index < comps.Count)
			{
				var comp = comps[index];
			    if (!AllowPrintOn(comp, isFirstPage, isLastPage))
			        comps.RemoveAt(index);

			    else
			    {
			        var cont = comp as StiContainer;
			        if (cont != null) PostProcessPrintOn(cont.Components, isFirstPage, isLastPage);

			        index++;
			    }
			}
		}

		private static bool AllowPrintOn(StiComponent comp, bool isFirstPage, bool isLastPage)
		{
			if (comp.PrintOn == StiPrintOnType.AllPages)return true;

			if ((comp.PrintOn & StiPrintOnType.ExceptFirstPage) > 0 && isFirstPage)return false;
			if (comp.PrintOn == StiPrintOnType.OnlyFirstAndLastPage)
			{
			    if (isFirstPage || isLastPage)return true;
			    return false;
			}
			if ((comp.PrintOn & StiPrintOnType.OnlyFirstPage) > 0 && !isFirstPage)return false;
			if ((comp.PrintOn & StiPrintOnType.OnlyLastPage) > 0 && !isLastPage)return false;
			if ((comp.PrintOn & StiPrintOnType.ExceptLastPage) > 0 && isLastPage)return false;

			return true;
		}
		#endregion
	}
}
