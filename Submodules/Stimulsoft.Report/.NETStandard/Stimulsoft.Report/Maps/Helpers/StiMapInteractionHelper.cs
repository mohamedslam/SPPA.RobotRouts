#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using System.Collections.Generic;

namespace Stimulsoft.Report.Maps.Helpers
{
    internal static class StiMapInteractionHelper
    {
        #region Fields
        private static Dictionary<StiMap, string> cacheKeys = new Dictionary<StiMap, string>();
        private static Dictionary<StiMap, string> newState = new Dictionary<StiMap, string>();
        #endregion

        #region Methods
        public static void BeginInit()
        {
            newState.Clear();
            foreach (var key in cacheKeys.Keys)
            {
                newState.Add(key, null);
            }
        }

        public static bool EndInit()
        {
            bool isChanged = false;

            var maps = new List<StiMap>();
            foreach (var key in cacheKeys.Keys)
            {
                maps.Add(key);
            }

            foreach (var pair in newState)
            {
                if (cacheKeys.ContainsKey(pair.Key))
                {
                    var actualValue = cacheKeys[pair.Key];
                    if (pair.Value == null)
                    {
                        cacheKeys.Remove(pair.Key);
                    }
                    else
                    {
                        cacheKeys[pair.Key] = pair.Value;
                    }

                    maps.Remove(pair.Key);
                    if (actualValue != pair.Value)
                        isChanged = true;
                }
                else
                {
                    if (pair.Value != null)
                    {
                        cacheKeys.Add(pair.Key, pair.Value);
                        isChanged = true;
                    }
                }
            }

            if (maps.Count > 0)
            {
                foreach (var key in maps)
                {
                    if (cacheKeys[key] != null)
                    {
                        cacheKeys.Remove(key);
                        isChanged = true;
                    }
                }
            }

            newState.Clear();

            return isChanged;
        }

        public static void AddKey(StiMap map, string key)
        {
            if (newState.ContainsKey(map))
                newState[map] = key;
            else
                newState.Add(map, key);
        }

        public static string GetKey(StiMap map)
        {
            if (cacheKeys.ContainsKey(map))
                return cacheKeys[map];

            return null;
        }
        #endregion
    }
}
