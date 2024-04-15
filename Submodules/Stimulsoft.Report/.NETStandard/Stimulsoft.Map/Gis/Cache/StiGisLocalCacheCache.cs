#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using Stimulsoft.Base.Gis;
using Stimulsoft.Map.Gis.Core;
using System;
using System.IO;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Media.Imaging;
#else
using System.Windows.Media.Imaging;
#endif

namespace Stimulsoft.Map.Gis.Cache
{
    internal static class StiGisLocalCacheCache
    {
        #region Methods
        public static void Set(byte[] buffer, StiGeoMapProviderType providerType, StiLanguageType language, StiGisPoint pos, int zoom)
        {
            if (buffer == null) return;

            var path = GetSettingsPath(providerType, language, zoom);
            if (path == null) return;

            var fileName = Path.Combine(path, $"{pos.X},{pos.Y}_{StiScale.Factor}.gisCache");
            if (File.Exists(fileName)) return;

            try
            {
                File.WriteAllBytes(fileName, buffer);
            }
            catch { }
        }

        public static StiGisMapImage Get(StiGeoMapProviderType providerType, StiLanguageType language, StiGeoRenderMode mode, StiGisPoint pos, int zoom)
        {
            var path = GetSettingsPath(providerType, language, zoom);
            if (path == null) return null;

            try
            {
                var fileName = Path.Combine(path, $"{pos.X},{pos.Y}_{StiScale.Factor}.gisCache");
                if (!File.Exists(fileName)) return null;

                var buffer = File.ReadAllBytes(fileName);
                if (buffer == null || buffer.Length < 10)
                {
                    File.Delete(fileName);
                    return null;
                }

                var result = new StiGisMapImage(buffer);
                if (mode == StiGeoRenderMode.Gdi)
                {
                    var img = Image.FromStream(new MemoryStream(buffer));
                    result.BitmapGdi = img;
                }

                return result;
            }
            catch { }

            return null;
        }

        private static string GetSettingsPath(StiGeoMapProviderType providerType, StiLanguageType language, int zoom)
        {
            try
            {
                var path = string.Format("Stimulsoft{0}GisMap{0}{2}{0}{1}{0}{2}", Path.DirectorySeparatorChar, providerType, zoom, language.ToString());
                if (StiBaseOptions.FullTrust)
                {
                    var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    if (!string.IsNullOrEmpty(folder))
                        path = Path.Combine(folder, path);
                }

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}