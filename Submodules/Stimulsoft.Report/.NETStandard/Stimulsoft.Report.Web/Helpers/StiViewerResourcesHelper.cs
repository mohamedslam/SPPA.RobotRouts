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

using System;
using System.Text;
using System.Reflection;
using System.IO;
using System.Collections;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using Stimulsoft.Base;
using Stimulsoft.Report.Dashboard;
using System.Collections.Generic;
using Stimulsoft.Base.Drawing;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiViewerResourcesHelper
    {
        #region Methods
        public static StiWebActionResult Get(StiRequestParams requestParams)
        {
            if (requestParams.Resource == "scripts")
            {
                var bytes = GetScripts(requestParams);
                return new StiWebActionResult(bytes, "text/javascript");
            }

            if (requestParams.Resource == "styles")
            {
                var bytes = GetStyles(requestParams);
                return new StiWebActionResult(bytes, "text/css");
            }

            if (requestParams.Resource == "images")
            {
                var data = new Hashtable();
                data["images"] = GetImagesArray(requestParams);

                //Only for Cloud Sharing
                var sharingLocalization = requestParams.GetString("sharingLocalization");
                if (!string.IsNullOrEmpty(sharingLocalization))
                {
                    requestParams.Localization = sharingLocalization;
                    data["localizationItems"] = StiCollectionsHelper.GetLocalizationItems(requestParams);
                }

                if (requestParams.GetBoolean("useCompression"))
                    requestParams.Server.UseCompression = true;

                return StiWebActionResult.JsonResult(requestParams, data);
            }

            return new StiWebActionResult();
        }

        public static async Task<StiWebActionResult> GetAsync(StiRequestParams requestParams)
        {
            return await Task.Run(() => Get(requestParams));
        }
        #endregion

        #region Images
        /// <summary>
        /// Get Bitmap image for Visual Studio design mode
        /// </summary>
        public static Bitmap GetBitmap(StiRequestParams requestParams, string imageName)
        {
            var assembly = typeof(StiViewerResourcesHelper).Assembly;
            var resourcePath = $"{typeof(StiViewerResourcesHelper).Namespace}.{requestParams.ComponentType}.Images.{imageName}";
            var stream = assembly.GetManifestResourceStream(resourcePath);
            if (stream == null) return null;
            return new Bitmap(stream);
        }

        public static Hashtable GetImagesArray(StiRequestParams requestParams, double? scaleFactor_ = null)
        {
            var scaleFactor = scaleFactor_ != null ? StiObjectConverter.ConvertToDouble(scaleFactor_) : Math.Max(requestParams.GetDouble("imagesScalingFactor"), 1);
            var images = new Hashtable();
            var assembly = typeof(StiViewerResourcesHelper).Assembly;
            List<string> resourcesNames = new List<string>(assembly.GetManifestResourceNames());
            var imagesPath = $"{typeof(StiViewerResourcesHelper).Namespace}.{requestParams.ComponentType}.Images.";
            var currScaleSuffix = StiScalingImagesHelper.GetScaleSuffix(scaleFactor);
            var imagesThemePath = $"{imagesPath}Themes.{requestParams.Theme}.";

            foreach (var resourceName in resourcesNames)
            {
                if (resourceName.EndsWith(".png") || resourceName.EndsWith(".gif"))
                {
                    if (!requestParams.Scripts.IncludeDashboards && resourceName.IndexOf(".Dashboards") >= 0) continue;

                    if (resourceName.StartsWith(imagesThemePath))
                    {
                        var imageName = resourceName.Replace(imagesThemePath, string.Empty);
                        images[imageName] = GetImageData(assembly, resourceName);
                    }
                    else if (resourceName.StartsWith(imagesPath))
                    {
                        var imageFullName = resourceName.Replace(imagesPath, string.Empty);
                        var imageScaleSuffix = StiScalingImagesHelper.GetScaleSuffixFromName(imageFullName);
                        var imageName = imageFullName.Replace($"{imageScaleSuffix}.", ".");
                        var imageNameWithoutExt = imageName.Substring(0, imageName.LastIndexOf("."));
                        var imageExt = imageName.Substring(imageName.LastIndexOf(".") + 1);
                        var resourcePathToScalingImage = $"{imagesPath}{imageNameWithoutExt}{currScaleSuffix}.{imageExt}";
                       
                        if (imageScaleSuffix != currScaleSuffix && resourcesNames.IndexOf(resourcePathToScalingImage) >= 0) continue;

                        if (scaleFactor > 1 && string.IsNullOrEmpty(imageScaleSuffix))
                        {
#if BLAZOR
                            images[imageName] = GetImageData(assembly, resourceName);
#else
                            try
                            {
                                //Get scaling image or resize if not exists in scaling collection
                                var bitmap = StiScalingImagesHelper.GetScalingImage(assembly, imagesPath, imageNameWithoutExt, imageExt, scaleFactor);
                                using (var ms = new MemoryStream())
                                {
                                    bitmap.Save(ms, ImageFormat.Png);
                                    images[imageName] = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                images[imageName] = GetImageData(assembly, resourceName);
                            }
#endif
                        }
                        else if (imageScaleSuffix == currScaleSuffix)
                        {
                            if (!images.Contains(imageName)) images[imageName] = GetImageData(assembly, resourceName);
                        }
                    }
                }
            }

            return images;
        }

        internal static string GetImageData(Assembly assembly, string name, double scaleFactor = 1)
        {
            var stream = assembly.GetManifestResourceStream(name);
            if (stream == null) return null;

            var ms = new MemoryStream();
            stream.CopyTo(ms);
            var buffer = ms.ToArray();
            var imageSizes = "";

            if (scaleFactor != 1)
            {
                ms.Position = 0;
                var image = new Bitmap(ms);
                imageSizes = $"imageSizes={Math.Round(image.Width / scaleFactor)};{Math.Round(image.Height / scaleFactor)}";
            }

            ms.Dispose();
            stream.Dispose();

            var ext = name.Substring(name.Length - 3);
            var base64 = Convert.ToBase64String(buffer);

            return $"{imageSizes}data:image/{ext};base64,{base64}";
        }
        #endregion

        #region Scripts
        public static string GetCollections(StiRequestParams requestParams, bool includeImages = false)
        {
            var collections = new Hashtable();
            collections["loc"] = StiCollectionsHelper.GetLocalizationItems(requestParams);
            collections["encodingData"] = StiCollectionsHelper.GetEncodingDataItems();
            collections["dateRanges"] = StiCollectionsHelper.GetDateRangesItems();
            collections["months"] = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            collections["paperSizes"] = StiCollectionsHelper.GetPaperSizes();
            collections["pdfSecurityCertificates"] = StiCollectionsHelper.GetPdfSecurityCertificatesItems();
            if (includeImages) collections["images"] = StiViewerResourcesHelper.GetImagesArray(requestParams);

            return JsonConvert.SerializeObject(collections, Formatting.None, new StringEnumConverter());
        }

        internal static byte[] GetScripts(StiRequestParams requestParams, bool forJSProject = false)
        {
#if !BLAZOR
            var jsCacheGuid = "ViewerScripts_" + StiMD5Helper.ComputeHash(requestParams.Theme + requestParams.Localization + requestParams.Version);
            if (requestParams.ComponentType == StiComponentType.Viewer)
            {
                var jsCacheResult = requestParams.Cache.Helper.GetResourceInternal(requestParams, jsCacheGuid);
                if (!string.IsNullOrEmpty(jsCacheResult)) return Encoding.UTF8.GetBytes(jsCacheResult);
            }
#endif
            
            var assembly = typeof(StiViewerResourcesHelper).Assembly;
            var resourceNames = assembly.GetManifestResourceNames();
            var jsPath = $"{typeof(StiViewerResourcesHelper).Namespace}.{requestParams.ComponentType}.Scripts.";
            var jsResult = string.Empty;

            foreach (var name in resourceNames)
            {
                if (name.IndexOf(jsPath) == 0 && name.EndsWith(".js"))
                {
                    if (!requestParams.Scripts.IncludeDashboards && name.IndexOf(".Dashboards.") >= 0) continue;
                    if (!requestParams.Scripts.IncludeClient && name.EndsWith(".Scripts.Client.js")) continue;
#if !SERVER && !CLOUD
                    if (name.IndexOf(".LoginControls.") >= 0 || name.EndsWith("InitializeFolderReportsPanel.js")) continue;
#endif
                    Stream stream = assembly.GetManifestResourceStream(name);
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string script = reader.ReadToEnd();
                        if (name.EndsWith(".Main.js") && requestParams.ComponentType == StiComponentType.Viewer)
                        {
                            var collections = string.Format("this.collections = {0}", GetCollections(requestParams, forJSProject));
                            script = script.Replace("this.collections = {};", collections);
                        }
                        jsResult += script + "\r\n";
                    }
                    stream.Dispose();
                }
            }

#if !BLAZOR
            if (requestParams.ComponentType == StiComponentType.Viewer)
                requestParams.Cache.Helper.SaveResourceInternal(requestParams, jsResult, jsCacheGuid);
#endif

            return Encoding.UTF8.GetBytes(jsResult);
        }
        #endregion

        #region Styles
        private static Hashtable GetStylesConstants(string css)
        {
            var constants = new Hashtable();
            var constantsArray = css.Split(';');
            for (var i = 0; i < constantsArray.Length; i++)
            {
                var index = constantsArray[i].IndexOf('@');
                var values = constantsArray[i].Substring(index >= 0 ? index : 0).Split('=');
                if (values.Length == 2) constants[values[0].Trim()] = values[1].Trim();
            }

            return constants;
        }

        public static byte[] GetStyles(StiRequestParams requestParams)
        {
            var stylesCacheGuid = "ViewerStyles_" + StiMD5Helper.ComputeHash(requestParams.Theme.ToString() + requestParams.Version);
            var stylesCacheResult = requestParams.Cache.Helper.GetResourceInternal(requestParams, stylesCacheGuid);
            if (!string.IsNullOrEmpty(stylesCacheResult)) return Encoding.UTF8.GetBytes(stylesCacheResult);

            var assembly = typeof(StiViewerResourcesHelper).Assembly;
            var resourceNames = assembly.GetManifestResourceNames();
            var stylesResult = string.Empty;

            var stylesFolder = requestParams.Theme;
            if (requestParams.Theme.StartsWith("Simple")) stylesFolder = "Simple";
            else if (requestParams.Theme.StartsWith("Office2007")) stylesFolder = "Office2007";
            else if (requestParams.Theme.StartsWith("Office2010")) stylesFolder = "Office2010";
            else if (requestParams.Theme.StartsWith("Office2013")) stylesFolder = "Office2013";
            else if (requestParams.Theme.StartsWith("Office2022")) stylesFolder = "Office2022";

            var stylesPath = $"{typeof(StiViewerResourcesHelper).Namespace}.{requestParams.ComponentType}.Styles.{stylesFolder}.";
            Hashtable constants = null;

            foreach (var name in resourceNames)
            {
                if (name.IndexOf(stylesPath) == 0 && name.EndsWith(".css"))
                {
#if !SERVER
                    if (name.EndsWith("FolderReportsPanelStyles.css")) continue;
#endif
                    Stream stream = assembly.GetManifestResourceStream(name);
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string styleText = reader.ReadToEnd();
                        if (name.EndsWith(requestParams.Theme + ".Constants.css")) constants = GetStylesConstants(styleText);
                        else if (!name.EndsWith(".Constants.css")) stylesResult += styleText + "\r\n";
                    }
                }
            }

            if (constants != null)
            {
                foreach (DictionaryEntry constant in constants)
                {
                    stylesResult = stylesResult.Replace((string)constant.Key, (string)constant.Value);
                }
            }

            requestParams.Cache.Helper.SaveResourceInternal(requestParams, stylesResult, stylesCacheGuid);

            return Encoding.UTF8.GetBytes(stylesResult);
        }
        #endregion
    }
}
