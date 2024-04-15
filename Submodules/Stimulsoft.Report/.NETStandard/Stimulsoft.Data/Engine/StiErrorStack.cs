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

using System.Collections.Generic;

namespace Stimulsoft.Data.Engine
{
    public class StiErrorStack
    {
        #region Fields
        private static Dictionary<string, string> keyToError = new Dictionary<string, string>();
        #endregion

        #region Methods
        public static void SetOk(string key)
        {
            lock (keyToError)
            {
                if (keyToError.ContainsKey(key))
                    keyToError.Remove(key);
            }
        }

        public static void SetError(string key, string error)
        {
            lock (keyToError)
            {
                keyToError[key] = error;
            }
        }

        public static string GetError(string key)
        {
            lock (keyToError)
            {
                return keyToError.ContainsKey(key) ? keyToError[key] : null;
            }
        }

        public static bool IsFail(string key)
        {
            lock (keyToError)
            {
                return keyToError.ContainsKey(key);
            }
        }
        #endregion
    }
}