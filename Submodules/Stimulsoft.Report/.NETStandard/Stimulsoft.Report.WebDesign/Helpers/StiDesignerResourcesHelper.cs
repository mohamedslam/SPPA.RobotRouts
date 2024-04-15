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

using System.Text;
using System.Reflection;
using System.IO;
using System.Collections;
using System;
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
    internal class StiDesignerResourcesHelper
    {
        #region Methods

        public static StiWebActionResult Get(StiRequestParams requestParams)
        {
            // Load DesignerFx scripts
            if (requestParams.Resource == "scripts")
            {
                var bytes = GetScripts(requestParams);
                return new StiWebActionResult(bytes, "text/javascript");
            }

            // Load image by file name
            if (requestParams.Resource != null && (requestParams.Resource.EndsWith(".png") || requestParams.Resource.EndsWith(".gif") || requestParams.Resource.EndsWith(".cur") || requestParams.Resource.EndsWith(".svg")))
            {
                var bytes = GetImage(requestParams);
                var contentType = requestParams.Resource.EndsWith(".cur") ? "application/octet-stream" : "image/" + (requestParams.Resource.EndsWith(".svg") ? "svg+xml" : requestParams.Resource.Substring(requestParams.Resource.Length - 3));
                return new StiWebActionResult(bytes, contentType);
            }

            // Load HTML5 designer scripts
            if (!string.IsNullOrEmpty(requestParams.Resource))
            {
                var bytes = GetScripts(requestParams);
                return new StiWebActionResult(bytes, "text/javascript");
            }

            // Load theme CSS styles
            if (!string.IsNullOrEmpty(requestParams.Theme))
            {
                var bytes = GetStyles(requestParams);
                return new StiWebActionResult(bytes, "text/css");
            }

            return new StiWebActionResult();
        }

        public static async Task<StiWebActionResult> GetAsync(StiRequestParams requestParams)
        {
            return await Task.Run(() => Get(requestParams));
        }
        #endregion

        #region Scripts

        public static byte[] GetScripts(StiRequestParams requestParams)
        {
#if !BLAZOR
            var jsCacheParams = requestParams.Theme + requestParams.CloudMode + requestParams.Resource + requestParams.Version;
            var jsCacheGuid = "DesignerScripts_" + StiMD5Helper.ComputeHash(jsCacheParams);
            if (requestParams.Resource == "DesignerScripts" || requestParams.Resource == "AllNotLoadedScripts")
            {
                var jsCacheResult = requestParams.Cache.Helper.GetResourceInternal(requestParams, jsCacheGuid);
                if (!string.IsNullOrEmpty(jsCacheResult)) return Encoding.UTF8.GetBytes(jsCacheResult);
            }
#endif
            
            var assembly = typeof(StiDesignerResourcesHelper).Assembly;
            var resourceNames = assembly.GetManifestResourceNames();
            var jsPath = string.Format("{0}.{1}.Scripts.", typeof(StiDesignerResourcesHelper).Namespace, requestParams.ComponentType);
            var jsFirstResult = new List<byte>();
            var jsResult = new List<byte>();
            var scriptNames = requestParams.Resource.Split(';');

            #region Blockly
            if (requestParams.Resource.StartsWith("BlocklyScripts;") || requestParams.Resource == "scripts")
            {
                var blocklyPath = string.Format("{0}.{1}.Blockly.", typeof(StiDesignerResourcesHelper).Namespace, requestParams.ComponentType);
                var locName = "en";
                if (requestParams.Resource.Split(';').Length > 1)
                    locName = requestParams.Resource.Split(';')[1];
                if (locName != "en" && assembly.GetManifestResourceInfo($"{blocklyPath}Localizations.{locName}.js") == null) locName = "en";

                var blocklyResourceNames = new string[]
                {
                    $"{blocklyPath}Js.blockly_compressed.js",
                    $"{blocklyPath}Localizations.{locName}.js",
                    $"{blocklyPath}Js.blocks_compressed.js",                    
                    $"{jsPath}Initialize.Form.InitializeBlocklyEditorForm_NotLoad.js"
                };

                foreach (var resourceName in blocklyResourceNames)
                {
                    var stream = assembly.GetManifestResourceStream(resourceName);
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        jsResult.AddRange(memoryStream.ToArray());
                        jsResult.Add(10);
                        jsResult.Add(13);
                    }
                    stream.Dispose();
                }

                if (requestParams.Resource != "scripts")
                    return jsResult.ToArray();
            }
            #endregion

            foreach (var resourceName in resourceNames)
            {
                if (resourceName.IndexOf(jsPath) == 0 && resourceName.EndsWith(".js"))
                {
                    foreach (var scriptName in scriptNames)
                    {
                        if ((requestParams.Resource == "DesignerScripts" && !resourceName.EndsWith("_NotLoad.js")) ||
                            (requestParams.Resource == "AllNotLoadedScripts" && resourceName.EndsWith("_NotLoad.js")) ||
                            resourceName.EndsWith(scriptName + "_NotLoad.js") ||
                            requestParams.Resource == "scripts")
                        {
                            if (!requestParams.CloudMode && resourceName.IndexOf(".LoginControls") >= 0) continue;
                            if (!StiDashboardAssembly.IsAssemblyLoaded && resourceName.IndexOf(".Dashboards") >= 0) continue;
                            if (!requestParams.Scripts.IncludeClient && resourceName.EndsWith(".Scripts.Client.js")) continue;

                            var stream = assembly.GetManifestResourceStream(resourceName);
                            using (var memoryStream = new MemoryStream())
                            {
                                stream.CopyTo(memoryStream);

                                if (requestParams.CloudMode && resourceName.IndexOf("jquery-3.6.0.min.js") >= 0)
                                {
                                    jsFirstResult.AddRange(memoryStream.ToArray());
                                    jsFirstResult.Add(10);
                                    jsFirstResult.Add(13);
                                }
                                else
                                {
                                    jsResult.AddRange(memoryStream.ToArray());
                                    jsResult.Add(10);
                                    jsResult.Add(13);
                                }
                            }
                            stream.Dispose();
                        }
                    }
                }
            }

            jsResult.InsertRange(0, jsFirstResult);

            var jsResultArray = jsResult.ToArray();

#if !BLAZOR
            if (requestParams.Resource == "DesignerScripts" || requestParams.Resource == "AllNotLoadedScripts")
                requestParams.Cache.Helper.SaveResourceInternal(requestParams, Encoding.UTF8.GetString(jsResultArray), jsCacheGuid);
#endif

            return jsResultArray;
        }

        #endregion

        #region Styles

        internal static Hashtable GetStylesConstants(string css)
        {
            Hashtable constants = new Hashtable();
            string[] constantsArray = css.Split(';');
            for (int i = 0; i < constantsArray.Length; i++)
            {
                var index = constantsArray[i].IndexOf('@');
                string[] values = constantsArray[i].Substring(index >= 0 ? index : 0).Split('=');
                if (values.Length == 2) constants[values[0].Trim()] = values[1].Trim();
            }

            return constants;
        }

        private static string GetThemeFolderName(StiRequestParams requestParams) {
            var theme = requestParams.Theme;

            if (theme.StartsWith("Office2013"))
                return "Office2013";

            if (theme.StartsWith("Office2022"))
                return "Office2022";

            return theme;
        }

        public static byte[] GetStyles(StiRequestParams requestParams)
        {
            string stylesCacheGuid = "DesignerStyles_" + StiMD5Helper.ComputeHash(requestParams.Theme + requestParams.CloudMode + requestParams.Version);
            string stylesCacheResult = requestParams.Cache.Helper.GetResourceInternal(requestParams, stylesCacheGuid);
            if (!string.IsNullOrEmpty(stylesCacheResult)) return Encoding.UTF8.GetBytes(stylesCacheResult);

            Assembly assembly = typeof(StiDesignerResourcesHelper).Assembly;
            string[] resourceNames = assembly.GetManifestResourceNames();
            string stylesResult = string.Empty;
            string stylesPath = string.Format("{0}.{1}.Styles.{2}.", typeof(StiDesignerResourcesHelper).Namespace, requestParams.ComponentType, GetThemeFolderName(requestParams));
            Hashtable designerConstants = null;

            foreach (var name in resourceNames)
            {
                if (name.IndexOf(stylesPath) == 0 && name.EndsWith(".css"))
                {
                    Stream stream = assembly.GetManifestResourceStream(name);
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string styleText = reader.ReadToEnd();

                        if (name.EndsWith(requestParams.Theme + ".Constants.css"))
                        {
                            designerConstants = GetStylesConstants(styleText);
                        }
                        else if (!name.EndsWith(".Constants.css")) stylesResult += styleText + "\r\n";
                    }
                }
            }

            if (designerConstants != null)
            {
                foreach (DictionaryEntry constant in designerConstants)
                {
                    stylesResult = stylesResult.Replace((string)constant.Key, (string)constant.Value);
                }
            }

            requestParams.Cache.Helper.SaveResourceInternal(requestParams, stylesResult, stylesCacheGuid);

            return Encoding.UTF8.GetBytes(stylesResult);
        }

        #endregion

        #region Images

        /// <summary>
        /// Get Bitmap image for Visual Studio design mode
        /// </summary>
        public static Bitmap GetBitmap(StiRequestParams requestParams, string imageName)
        {
            Assembly assembly = typeof(StiDesignerResourcesHelper).Assembly;
            string resourcePath = string.Format("{0}.{1}.Images.{2}", typeof(StiDesignerResourcesHelper).Namespace, requestParams.ComponentType, imageName);
            Stream stream = assembly.GetManifestResourceStream(resourcePath);
            if (stream == null) return null;
            return new Bitmap(stream);
        }

        public static byte[] GetImage(StiRequestParams requestParams)
        {
            Assembly assembly = typeof(StiDesignerResourcesHelper).Assembly;
            MemoryStream ms = new MemoryStream();
            byte[] buffer = null;

            var imageScaleSuffix = StiScalingImagesHelper.GetScaleSuffixFromName(requestParams.Resource);
            if (!string.IsNullOrEmpty(imageScaleSuffix))
            {
                //Get scaling image or resize if not exists in scaling collection
                var imagesPath = string.Format("{0}.{1}.Images.Office2013.", typeof(StiDesignerResourcesHelper).Namespace, requestParams.ComponentType);
                var scaleFactor = StiScalingImagesHelper.GetScaleFactorFromName(requestParams.Resource);
                var imageName = requestParams.Resource.Replace($"{imageScaleSuffix}.", ".");
                var imageNameWithoutExt = imageName.Substring(0, imageName.LastIndexOf("."));
                var imageExt = imageName.Substring(imageName.LastIndexOf(".") + 1);
                var bitmap = StiScalingImagesHelper.GetScalingImage(assembly, imagesPath, imageNameWithoutExt, imageExt, scaleFactor);
                bitmap.Save(ms, ImageFormat.Png);
                buffer = ms.ToArray();
                ms.Dispose();

                return buffer;
            }

            string resourcePath = string.Format("{0}.{1}.Images.Office2013.{2}", typeof(StiDesignerResourcesHelper).Namespace, requestParams.ComponentType, requestParams.Resource);
            Stream stream = assembly.GetManifestResourceStream(resourcePath);
            if (stream == null) return null;

            stream.CopyTo(ms);
            buffer = ms.ToArray();
            ms.Dispose();
            stream.Dispose();

            return buffer;
        }

        public static Hashtable GetImagesArray(StiRequestParams requestParams, string resourcesUrl, double scaleFactor = 1, bool ignoreFirstLoadingImages = false)
        {
            Hashtable images = new Hashtable();
            Assembly assembly = typeof(StiDesignerResourcesHelper).Assembly;
            List<string> resourcesNames = new List<string>(assembly.GetManifestResourceNames());
            var imagesPath = string.Format("{0}.{1}.Images.Office2013.", typeof(StiDesignerResourcesHelper).Namespace, requestParams.ComponentType);
            var currScaleSuffix = StiScalingImagesHelper.GetScaleSuffix(scaleFactor);

            foreach (var resourceName in resourcesNames)
            {
                if (resourceName.IndexOf(imagesPath) == 0 && (resourceName.EndsWith(".png") || resourceName.EndsWith(".gif") || resourceName.EndsWith(".cur") || resourceName.EndsWith(".svg")))
                {
                    if (!StiDashboardAssembly.IsAssemblyLoaded && resourceName.IndexOf(".Dashboards") >= 0)
                        continue;

                    if (!requestParams.CloudMode && (resourceName.IndexOf(".Gui.") >= 0 || resourceName.IndexOf(".Office2013.Update.") >= 0))
                        continue;

                    var imageFullName = resourceName.Replace(imagesPath, string.Empty);
                    var imageScaleSuffix = StiScalingImagesHelper.GetScaleSuffixFromName(imageFullName);
                    var imageName = imageFullName.Replace($"{imageScaleSuffix}.", ".");
                    var imageNameWithoutExt = imageName.Substring(0, imageName.LastIndexOf("."));
                    var imageExt = imageName.Substring(imageName.LastIndexOf(".") + 1);
                    var resourcePathToScalingImage = $"{imagesPath}{imageNameWithoutExt}{currScaleSuffix}.{imageExt}";
                    var imageKey = imageName.Replace("_FirstLoadingImages.", "");

                    if (imageScaleSuffix != currScaleSuffix && resourcesNames.IndexOf(resourcePathToScalingImage) >= 0) continue;

                    if (string.IsNullOrEmpty(resourcesUrl) || imageFullName.IndexOf("_FirstLoadingImages") >= 0)
                    {
                        if (ignoreFirstLoadingImages && imageName.IndexOf("_FirstLoadingImages") >= 0) continue;

                        if (resourceName.EndsWith(".png") && scaleFactor > 1 && String.IsNullOrEmpty(imageScaleSuffix))
                        {
#if !BLAZOR
                            try
                            {
                                //Get scaling image or resize if not exists in scaling collection
                                var bitmap = StiScalingImagesHelper.GetScalingImage(assembly, imagesPath, imageNameWithoutExt, imageExt, scaleFactor);
                                using (var ms = new MemoryStream())
                                {
                                    bitmap.Save(ms, ImageFormat.Png);
                                    images[imageKey] = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
#endif
                                Stream stream = assembly.GetManifestResourceStream(resourceName);
                                if (stream != null)
                                {
                                    using (var ms = new MemoryStream())
                                    {
                                        stream.CopyTo(ms);
                                        images[imageKey] = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                                        stream.Dispose();
                                    }
                                }
#if !BLAZOR
                            }
#endif
                        }
                        else if (imageScaleSuffix == currScaleSuffix || imageExt == "svg")
                        {
                            Stream stream = assembly.GetManifestResourceStream(resourceName);
                            if (stream != null)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    stream.CopyTo(ms);
                                    images[imageKey] = string.Format("data:{0}{1};base64,{2}", imageExt == "cur" ? "application/" : "image/", imageExt == "svg" ? "svg+xml" : imageExt, Convert.ToBase64String(ms.ToArray()));
                                    stream.Dispose();
                                }
                            }
                        }
                    }
                    else
                    {
                        var urlImageKey = scaleFactor > 1 && imageExt != "svg" ? $"{imageNameWithoutExt}{StiScalingImagesHelper.GetScaleSuffix(scaleFactor)}.{imageExt}" : imageKey;
                        images[imageKey] = StiWebDesigner.GetImageUrl(requestParams, resourcesUrl + urlImageKey);
                    }
                }
            }

            return images;
        }

        #endregion
    }
}
