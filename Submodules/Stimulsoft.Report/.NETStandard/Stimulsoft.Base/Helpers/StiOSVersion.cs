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

using System;

namespace Stimulsoft.Base.Helpers
{
    public static class StiOSVersion
    {
        static StiOSVersion()
        {
#if DEBUG
            var settings = Type.GetType("Stimulsoft.Report.StiSettings, Stimulsoft.Report");
            if (settings != null)
            {
                var method = settings.GetMethod("GetBool", new Type[] { typeof(string), typeof(string), typeof(bool) });
                var result = method.Invoke(null, new object[] { "AppTheme", "ActivateWin11Support", false });
                if (result != null && result is bool && (bool)result == true)
                {
                    isWindows11 = true;
                    return;
                }
            }
#endif
#if !BLAZOR
            try
            {
                var registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                var currentBuild = registryKey.GetValue("CurrentBuild").ToString();
                if (int.TryParse(currentBuild, out var build))
                {
                    if (build >= 22000)
                        isWindows11 = true;
                }
            }
            catch
            {
            }
#endif
        }

        #region Fields
        private static bool isWindows11;
        #endregion

        #region Methods
        public static bool IsWindows11()
        {
            return isWindows11;
        }
        #endregion
    }
}