#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports 									            }
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System;
using System.IO;
using System.Reflection;

namespace Stimulsoft.Base.Design
{
    public static class StiLibStatus
    {
        public static bool IsJsAvailable()
        {
            return IsAvailable(StiLibType.Php) || IsAvailable(StiLibType.Js);
        }

        public static bool IsAvailable(StiLibType type)
        {
            var path = GetLibPath(type);
            return path != null && Directory.Exists(path);
        }

        public static string GetJsLibPathIfExists()
        {
            if (IsAvailable(StiLibType.Js))
            {
                var folder = GetLibPath(StiLibType.Js);
                return folder != null && Directory.Exists(folder) ? folder : null;
            }

            if (IsAvailable(StiLibType.Php))
            {
                var folder = Path.Combine(GetLibPath(StiLibType.Php), "JS");
                return folder != null && Directory.Exists(folder) ? folder : null;
            }

            var folderApp = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            folderApp = Path.Combine(folderApp, "Libs-JS");
            if (Directory.Exists(folderApp))
                return folderApp;

            return null;
        }

        public static string GetLibPath(StiLibType type)
        {
            var designerPath = StiDesignerAppStatus.GetDesignerPath(StiPlatformType.Wpf);
            if (designerPath == null)
                return null;

            var designerFolder = Path.GetDirectoryName(designerPath);
            var libFolder = Path.Combine(designerFolder, "..", "..", "Libs");

            switch (type)
            {
                case StiLibType.Net:
                    return Path.Combine(libFolder, "Reports.Net");

                case StiLibType.Wpf:
                    return Path.Combine(libFolder, "Reports.Wpf");

                case StiLibType.Web:
                    return Path.Combine(libFolder, "Reports.Web");

                case StiLibType.Js:
                    return Path.Combine(libFolder, "Reports.JS");

                case StiLibType.Php:
                    return Path.Combine(libFolder, "Reports.PHP");

                case StiLibType.Java:
                    return Path.Combine(libFolder, "Reports.Java");

                case StiLibType.Flex:
                    return Path.Combine(libFolder, "Reports.Flex");

                default:
                    throw new NotSupportedException();
            }
        }
    }
}