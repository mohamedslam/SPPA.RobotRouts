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

using System.IO;
using System.Reflection;

namespace Stimulsoft.Base.Design
{
    public static class StiDesignerAppStatus
    {
        public static bool IsAvailable(StiPlatformType type)
        {
            var path = GetDesignerPath(type);
            if (path == null)
                return false;

            try
            {
                return File.Exists(path);
            }
            catch
            {
                return false;
            }
        }

        public static string GetDesignerPath(StiPlatformType type)
        {
            try
            {
                string location = null;

                try
                {
                    location = Assembly.GetEntryAssembly()?.Location;
                }
                catch
                {
                }

                try
                {
                    if (location == null)
                        location = Assembly.GetExecutingAssembly().Location;
                }
                catch
                {
                }

                try
                {
                    if (location == null)
                        location = Assembly.GetCallingAssembly().Location;
                }
                catch
                {
                }

                if (string.IsNullOrWhiteSpace(location))
                    return null;

                var designerFolder = Path.GetDirectoryName(location);
                switch (type)
                {
                    case StiPlatformType.WinForms:
                        return Path.Combine(designerFolder, "Designer.exe");

                    case StiPlatformType.Wpf:
                        return Path.Combine(designerFolder, "DesignerV2.Wpf.exe");

                    case StiPlatformType.Js:
                        return Path.Combine(designerFolder, "..", "JS", "Designer.exe");

                    case StiPlatformType.Flex:
                        return Path.Combine(designerFolder, "..", "Flex", "DesignerFx.exe");

                    default:
                        return null;
                }
            }
            catch
            {
            }

            return null;
        }

        public static bool IsRunning
        {
            get
            {
                try
                {
                    var assembly = Assembly.GetEntryAssembly();
                    if (assembly == null)
                        return false;

                    if (assembly.FullName != null && !assembly.FullName.Contains(StiPublicKeyToken.Key))
                        return false;

                    var codeBase = assembly.GetName().CodeBase;
                    if (string.IsNullOrWhiteSpace(codeBase))
                        return false;

                    return
                        codeBase.EndsWithInvariantIgnoreCase("designer.exe") ||
                        codeBase.EndsWithInvariantIgnoreCase("designer.wpf.exe") ||
                        codeBase.EndsWithInvariantIgnoreCase("designerv2.wpf.exe");
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}