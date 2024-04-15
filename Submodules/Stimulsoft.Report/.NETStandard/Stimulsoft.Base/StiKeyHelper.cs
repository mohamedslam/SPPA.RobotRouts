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
using System.Linq;
using System.Text.RegularExpressions;

namespace Stimulsoft.Base
{
    /// <summary>
    /// This class helps work with keys.
    /// </summary>
    public static class StiKeyHelper
    {
        /// <summary>
        /// Returns new generated key.
        /// </summary>
        public static string GenerateKey()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }

        /// <summary>
        /// Returns new generated short key.
        /// </summary>
        internal static string GenerateShortKey(int max)
        {
            return GenerateKey().Substring(0, max).ToLowerInvariant();
        }

        /// <summary>
        /// Returns true if the specified key is not empty.
        /// </summary>
        public static bool IsKey(string key)
        {
            return !IsEmptyKey(key);
        }

        /// <summary>
        /// Returns true if the specified key is correct.
        /// </summary>
        public static bool IsCorrectKey(string key)
        {
            if (IsEmptyKey(key)) return true;
            if (key.Length != 32) return false;

            return Regex.IsMatch(key, "^[a-zA-Z0-9]*$");
        }

        /// <summary>
        /// Returns true if the specifeid key is empty.
        /// </summary>
        public static bool IsEmptyKey(string key)
        {
            return string.IsNullOrWhiteSpace(key);
        }

        /// <summary>
        /// Returns true if both specifeid keys is empty.
        /// </summary>
        public static bool IsEmptyKey(string key1, string key2)
        {
            return IsEmptyKey(key1) && IsEmptyKey(key2);
        }

        /// <summary>
        /// Returns key1 if it is not empty. Otherwise returns key2.
        /// </summary>
        public static string SelectKey(string key1, string key2)
        {
            if (!IsEmptyKey(key1)) return key1;
            else return key2;
        }

        /// <summary>
        /// Returns true if both keys equals.
        /// </summary>
        public static bool IsEqualKeys(string key1, string key2)
        {
            if (IsEmptyKey(key1) && IsEmptyKey(key2)) return true;
            key1 = key1.Trim().ToLowerInvariant();
            key2 = key2.Trim().ToLowerInvariant();

            return string.Compare(key1, key2, StringComparison.Ordinal) == 0;
        }

        /// <summary>
        /// Returns specified key if it is not empty. Otherwise returns new generated key.
        /// </summary>
        public static string GetOrGeneratedKey(string key)
        {
            return IsEmptyKey(key) ? GenerateKey() : key;
        }

        /// <summary>
        /// Returns key1 if it is not empty or key2 if it is not empty. Returns new generated key if both key1 and key2 is empty.
        /// </summary>
        public static string GetOrGeneratedKey(string key1, string key2)
        {
            return GetOrGeneratedKey(SelectKey(key1, key2));
        }
    }
}