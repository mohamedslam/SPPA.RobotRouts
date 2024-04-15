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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Exceptions;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Server;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Font = Stimulsoft.Drawing.Font;
using Pen = Stimulsoft.Drawing.Pen;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Base.Helpers
{
    public static class StiSvgHelper
    {
        #region Const
        public const string AssemblyName = "Stimulsoft.Svg";
        public const string AssemblyVersion = "2023.1.1";
        #endregion

        #region Fields.Static
        private static object lockObject = new object();
        public static string SvgAssemblyPrefix = "Stimulsoft.";
        private static bool isFirstLoadInWpf;
        #endregion

        #region Properties.Status
        private static Assembly svgAssembly;
        public static Assembly SvgAssembly
        {
            get
            {
                lock (lockObject)
                {
                    if (svgAssembly == null && !TryToLoadAssembly)
                    {
                        TryToLoadAssembly = true;

                        svgAssembly = StiAssemblyFinder.GetAssembly("Stimulsoft.Svg.dll");

                        if (svgAssembly == null)
                        {
                            SvgAssemblyPrefix = string.Empty;
                            svgAssembly = StiAssemblyFinder.GetAssembly("Svg.dll");
                        }

                        if (svgAssembly == null)
                        {
                            if (StiAccountSettings.CurrentAccount != null)
                            {
                                if (!StiBaseOptions.IsDashboardViewerWPF)
                                {
                                    try
                                    {
                                        var downloader = StiAccountSettings.CurrentAccount.AccountCreater.GetNuGetDownloader();
                                        downloader.RunDownloadSvg();

                                        svgAssembly = StiAssemblyFinder.GetAssembly("Stimulsoft.Svg.dll");
                                        SvgAssemblyPrefix = "Stimulsoft.";
                                    }
                                    catch { }
                                }
                            }

                            else
                            {
                                var provider = StiNotFoundProviderCreater.GetNotFoundProvider();
                                provider?.Show("Stimulsoft.Svg");
                            }
                        }
                    }

                    return svgAssembly;
                }
            }
        }

        public static bool IsSvgAssemblyLoaded
        {
            get
            {
                return svgAssembly != null;
            }
        }

        public static bool TryToLoadAssembly { get; set; }
        #endregion

        #region Methods.Static
        internal static bool GetCurrentLoadedState()
        {
            if (svgAssembly != null)
                return true;

            svgAssembly = StiAssemblyFinder.GetAssembly("Stimulsoft.Svg.dll");

            if (svgAssembly == null)
            {
                SvgAssemblyPrefix = string.Empty;
                svgAssembly = StiAssemblyFinder.GetAssembly("Svg.dll");
            }
            
            return svgAssembly != null;
        }

        internal static bool IsLoadedInWPF()
        {
            if (isFirstLoadInWpf)
                return true;

            isFirstLoadInWpf = true;
            return false;
        }

        internal static Image ConvertSvgToImageScaled(byte[] svgObject, float svgScale = 1)
        {
            var unscaledImage = ConvertSvgToImage(svgObject, 100, 100, false, true);
            if (svgScale == 1) return unscaledImage;

            Image scaledImage = null;
            try
            {
                scaledImage = ConvertSvgToImage(svgObject, (int)(unscaledImage.Width * svgScale), (int)(unscaledImage.Height * svgScale), true, true);
            }
            catch (OutOfMemoryException)
            {
                if (svgScale < 1) return unscaledImage;

                #region Try to reduce svg resolution
                svgScale = (svgScale + 1) / 2;  //half scale
                try
                {
                    scaledImage = ConvertSvgToImage(svgObject, (int)(unscaledImage.Width * svgScale), (int)(unscaledImage.Height * svgScale), true, true);
                }
                catch (OutOfMemoryException)
                {
                    //without scaling
                    return unscaledImage;
                }
                #endregion
            }
            return scaledImage;
        }

        public static Image ConvertSvgToImage(byte[] svgObject, int width, int height, bool stretch = true, bool aspectRatio = false, bool rethrowOutOfMemory = false)
        {
            if (svgObject == null || svgObject.Length == 0) return null;

            if (ConvertSvgToImageInternal(svgObject, width, height, stretch, aspectRatio, out var result, rethrowOutOfMemory)) return result;

            if (width < 250 && height < 30) return null;

            var image = new Bitmap(width, height);
            using (var g = Graphics.FromImage(image))
                DrawSvgIsNotFound(g, width, height);

            return image;
        }

        private static bool ConvertSvgToImageInternal(byte[] svgObject, int width, int height, bool stretch, bool aspectRatio, out Image result, bool rethrowOutOfMemory)
        {
            result = null;

            try
            {
                if (SvgAssembly == null) return false;
                if (svgObject == null || svgObject.Length == 0) return true;

                //using (var stream = new MemoryStream(svgObject))
                //{
                //    var typeSvgDocument = SvgAssembly.GetType("Svg.SvgDocument");
                //    if (typeSvgDocument == null) return false;

                //    var methodOpen = typeSvgDocument.GetMethod("Open", new[] { typeof(Stream) });
                //    if (methodOpen == null) return false;

                //    var methodGenericOpen = methodOpen.MakeGenericMethod(typeSvgDocument);
                //    var document = methodGenericOpen.Invoke(null, new object[] { stream });
                //    if (document == null) return false;
                //}

                var document = OpenSvg(svgObject);

                if (document == null) return false;

                #region AspectRatio and stretch correction
                var typeSvgUnit = SvgAssembly.GetType(SvgAssemblyPrefix + "Svg.SvgUnit");
                PropertyInfo propertySvgUnitValue = null;
                if (typeSvgUnit != null)
                {
                    propertySvgUnitValue = typeSvgUnit.GetProperty("Value");
                }
                bool needFixedBitmap = false;
                if ((!stretch || aspectRatio) && (propertySvgUnitValue != null))
                {
                    var svgSize = GetSvgSize(document);

                    if (svgSize.Width != 0 && svgSize.Height != 0)
                    {
                        if (svgSize.Width > width || svgSize.Height > height)
                        {
                            needFixedBitmap = true;
                        }

                        if (aspectRatio)
                        {
                            double svgAspect = svgSize.Width / svgSize.Height;
                            double aspect = (float)width / height;
                            if (svgAspect > aspect)
                            {
                                height = (int)Math.Round(width / svgAspect);
                            }
                            else
                            {
                                width = (int)Math.Round(height * svgAspect);
                            }
                        }
                    }
                }
                #endregion

                if (stretch)
                {
                    var methodDraw = document.GetType().GetMethod("Draw", new[] { typeof(int), typeof(int) });
                    if (methodDraw == null) return false;

                    object res = methodDraw.Invoke(document, new object[] { width, height });
                    result = ConvertToImage(res);
                }
                else
                {
                    if (needFixedBitmap)
                    {
                        var methodDraw = document.GetType().GetMethod("Draw", new[] { typeof(global::System.Drawing.Bitmap) });
                        if (methodDraw == null) return false;

                        var bmp = new global::System.Drawing.Bitmap(width, height);

                        object obj = methodDraw.Invoke(document, new object[] { bmp });
                        result = ConvertToImage(bmp);
                    }
                    else
                    {
                        var methodDraw = document.GetType().GetMethod("Draw", new Type[0]);
                        if (methodDraw == null) return false;

                        object res = methodDraw.Invoke(document, new object[0]);
                        result = ConvertToImage(res);
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                if (ex.InnerException is OutOfMemoryException && rethrowOutOfMemory) throw new OutOfMemoryException();
                return false;
            }
        }

        private static Image ConvertToImage(object obj)
        {
#if STIDRAWING
            if (Graphics.GraphicsEngine == Stimulsoft.Drawing.GraphicsEngine.Gdi)
            {
                return obj as global::System.Drawing.Bitmap;
            }
            else
            {
                var bmp = obj as global::System.Drawing.Bitmap;
                if (bmp != null)
                {
                    MemoryStream ms = new MemoryStream();
                    bmp.Save(ms, global::System.Drawing.Imaging.ImageFormat.Png);
                    ms.Seek(0, SeekOrigin.Begin);
                    return Image.FromStream(ms);
                }
            }
#endif
            return obj as Image;
        }

        public static bool DrawSvg(byte[] svgObject, RectangleF rect, bool stretch, bool aspectRatio, double zoom, Graphics gr)
        {
            if (svgObject == null || svgObject.Length == 0) return true;
            if (DrawSvgInternal(svgObject, rect, stretch, aspectRatio, zoom, gr)) return true;
            return false;
        }

        private static bool DrawSvgInternal(byte[] svgObject, RectangleF rect, bool stretch, bool aspectRatio, double zoom, Graphics gr)
        {
            try
            {
                if (SvgAssembly == null) return false;
                if (svgObject == null || svgObject.Length == 0) return true;

                var document = OpenSvg(svgObject);
                if (document == null) return false;

                var svgSize = GetSvgSize(document);

                #region AspectRatio and stretch correction
                float width = rect.Width;
                float height = rect.Height;
                if ((!stretch || aspectRatio) && (svgSize.Width != 0 && svgSize.Height != 0))
                {
                    if (aspectRatio)
                    {
                        double svgAspect = svgSize.Width / svgSize.Height;
                        double aspect = (float)width / height;
                        if (svgAspect > aspect)
                        {
                            height = (float)Math.Round(width / svgAspect);
                        }
                        else
                        {
                            width = (float)Math.Round(height * svgAspect);
                        }
                    }
                    if (!stretch)
                    {
                        width = svgSize.Width * (float)zoom;
                        height = svgSize.Height * (float)zoom;
                    }
                }
                #endregion

                var state = gr.Save();
                gr.TranslateTransform(rect.X, rect.Y);
                gr.ScaleTransform(width / svgSize.Width, height / svgSize.Height);

                var methodDraw = document.GetType().GetMethod("Draw", new[] { typeof(Graphics) });
                if (methodDraw == null) return false;
                methodDraw.Invoke(document, new object[] { gr });

                gr.Restore(state);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static SizeF GetSvgSize(object document)
        {
            SizeF size = new Size();

            var typeSvgDocument = SvgAssembly.GetType(SvgAssemblyPrefix + "Svg.SvgDocument");
            var typeSvgUnit = SvgAssembly.GetType(SvgAssemblyPrefix + "Svg.SvgUnit");

            PropertyInfo propertySvgUnitValue = null;
            if (typeSvgUnit != null)
            {
                propertySvgUnitValue = typeSvgUnit.GetProperty("Value");
            }

            var typeSvgUnitType = SvgAssembly.GetType(SvgAssemblyPrefix + "Svg.SvgUnitType");
            PropertyInfo propertySvgUnitType = null;
            if (typeSvgUnitType != null)
            {
                propertySvgUnitType = typeSvgUnit.GetProperty("Type");
            }

            if (propertySvgUnitValue != null)
            {
                float svgWidth = 0;
                float svgHeight = 0;

                var propertyWidth = typeSvgDocument.GetProperty("Width");
                if (propertyWidth != null)
                {
                    var propertyValue = propertyWidth.GetValue(document, new object[0]);
                    if (propertyValue != null)
                    {
                        var propertyValueType = propertySvgUnitType.GetValue(propertyValue, new object[0]);
                        if ((propertyValueType != null) && (propertyValueType.ToString().ToLowerInvariant() == "percentage")) return size;

                        var propertyValueValue = propertySvgUnitValue.GetValue(propertyValue, new object[0]);
                        svgWidth = (float)propertyValueValue;
                    }
                }

                var propertyHeight = typeSvgDocument.GetProperty("Height");
                if (propertyHeight != null)
                {
                    var propertyValue = propertyHeight.GetValue(document, new object[0]);
                    if (propertyValue != null)
                    {
                        var propertyValueType = propertySvgUnitType.GetValue(propertyValue, new object[0]);
                        if ((propertyValueType != null) && (propertyValueType.ToString().ToLowerInvariant() == "percentage")) return size;

                        var propertyValueValue = propertySvgUnitValue.GetValue(propertyValue, new object[0]);
                        svgHeight = (float)propertyValueValue;
                    }
                }

                size.Width = svgWidth;
                size.Height = svgHeight;
            }
            return size;
        }

        public static object OpenSvg(byte[] svgObject)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;

            using (var stream = new MemoryStream(svgObject))
            {
                xmlDoc.Load(stream);
            }

            #region Fix width="100%" problem
            var attrWidth = xmlDoc.DocumentElement.Attributes["width"];
            var attrHeight = xmlDoc.DocumentElement.Attributes["height"];
            var attrViewBox = xmlDoc.DocumentElement.Attributes["viewBox"];
            if ((attrWidth != null && attrWidth.Value == "100%") && (attrHeight != null && attrHeight.Value == "100%") && (attrViewBox != null && !string.IsNullOrWhiteSpace(attrViewBox.Value)))
            {
                string[] parts = attrViewBox.Value.Split(new char[] { ' ' });
                if (parts.Length == 4)
                {
                    attrWidth.Value = parts[2];
                    attrHeight.Value = parts[3];
                }
            }
            #endregion

            //Fix text "desc" problem
            CheckDescRecursive(xmlDoc.DocumentElement);

            var typeSvgDocument = SvgAssembly.GetType(SvgAssemblyPrefix + "Svg.SvgDocument");
            if (typeSvgDocument == null) return null;

            var methodOpen = typeSvgDocument.GetMethod("Open", new[] { typeof(XmlDocument) });
            if (methodOpen == null) return null;

            var document = methodOpen.Invoke(null, new object[] { xmlDoc });

            return document;
        }

        private static void CheckDescRecursive(XmlNode baseNode)
        {
            foreach (XmlNode node in baseNode.ChildNodes)
            {
                if (node.HasChildNodes)
                {
                    if ((node.NodeType == XmlNodeType.Element) && (node.Name == "text") && (node.ChildNodes.Count == 2) && (node.ChildNodes[0].Name == "desc") && (node.ChildNodes[1].NodeType == XmlNodeType.Text))
                    {
                        node.RemoveChild(node.ChildNodes[0]);
                    }
                    else
                    {
                        CheckDescRecursive(node);
                    }
                }
            }
        }

        public static bool DrawWithSvg(string svgLocator, RectangleF rectF, RectangleF rectSVG, double zoom, Graphics gr)
        {
            try
            {
                var rect = Rectangle.Ceiling(rectF);

                if (SvgAssembly == null)
                {
                    using (var image = new Bitmap(rect.Width, rect.Height))
                    using (var g = Graphics.FromImage(image))
                    {
                        DrawSvgIsNotFound(g, rect.Width, rect.Height);
                        gr.DrawImage(image, rect);
                    }

                    return false;
                }

                var document = SvgAssembly.GetType(SvgAssemblyPrefix + "Svg.SvgDocument");
                if (document == null)
                    return false;

                var svgSize = new Size(rect.Width, rect.Height);

                var state = gr.Save();

                gr.SetClip(rect, CombineMode.Intersect);
                gr.TranslateTransform(rect.X + rectSVG.X, rect.Y + rectSVG.Y);

                DrawWithSvgInternal(gr, document, svgLocator);

                gr.Restore(state);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void DrawWithSvgInternal(Graphics g, Type typeSvgDocument, string svgLocator)
        {
            try
            {
                var methodFromSvg = typeSvgDocument.GetMethod("FromSvg", new[] { typeof(string) });

                MethodInfo generic = methodFromSvg.MakeGenericMethod(SvgAssembly.GetType(SvgAssemblyPrefix + "Svg.SvgDocument"));
                var svgDoc = generic.Invoke(null, new object[] { svgLocator });

                var methodDraw = svgDoc.GetType().GetMethod("Draw", new[] { typeof(Graphics) });

                methodDraw.Invoke(svgDoc, new object[] { g });

            }
            catch
            {
            }
        }

        private static void DrawSvgIsNotFound(Graphics g, int width, int height)
        {
            using (var font = new Font("Arial", 8, FontStyle.Bold))
            using (var sf = StringFormat.GenericDefault.Clone() as StringFormat)
            using (var pen = new Pen(StiUX.ItemForeground))
            using (var brush = new SolidBrush(StiUX.ItemForeground))
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                g.Clear(StiUX.Background);
                g.DrawString(string.Format(StiLocalization.Get("Notices", "IsNotFound"), "Stimulsoft.Svg.dll"), font,
                    brush, new Rectangle(0, 0, width, height), sf);
            }
        }

        public static Bitmap ConvertSvgToImage(string svgLocator, Rectangle rectF, RectangleF rectSVG, double zoom)
        {
            var rect = Rectangle.Ceiling(rectF);
            var bitmap = new Bitmap(rect.Width, rect.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                DrawWithSvg(svgLocator, rectF, rectSVG, zoom, g);
            }
            return bitmap;
        }
        #endregion

        #region Methods.Status
        public static bool IsSvgFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            if (!File.Exists(fileName)) return false;

            var bytes = File.ReadAllBytes(fileName);
            return IsSvg(bytes);
        }

        public static bool IsSvg(byte[] data)
        {
            if (data == null || data.Length < 5) return false;
            
            var maxSvgOffset = Math.Min(data.Length - 4, 1000);
            var stack = new Stack<bool>();
            var skip = false;
            for (var pos = 0; pos < maxSvgOffset; pos++)
            {
                if (data[pos] == '<')
                {
                    if (stack.Count == 0 && 
                        data[pos] == '<' && data[pos + 1] == 's' && data[pos + 2] == 'v' && data[pos + 3] == 'g' && 
                        char.IsWhiteSpace((char)data[pos + 4]))
                        return true;

                    if (data[pos + 1] == '/')
                    {
                        if (stack.Count > 0)
                            skip = stack.Pop();
                    }
                    else
                    {
                        stack.Push(skip);
                        if (data[pos + 1] == '!' || data[pos + 1] == '?')
                            skip = true;
                    }
                }
                else if (data[pos] == '>' && (skip || (pos > 0 && data[pos - 1] == '/')) && stack.Count > 0)
                    skip = stack.Pop();
            }

            return false;
        }
        #endregion

        #region Events
        internal static event EventHandler SvgLoaded;
        internal static void InvokeSvgLoaded()
        {
            SvgLoaded?.Invoke(null, EventArgs.Empty);
        }
        #endregion
    }
}