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

using System.Collections;
using System.Collections.Generic;

namespace Stimulsoft.Base
{
    internal class StiAppFunctions
    {
        #region Properties
        internal static Hashtable FunctionsToCompile { get; set; } = new Hashtable();

        internal static Hashtable FunctionsToCompileLower { get; set; } = new Hashtable();

        internal static Hashtable Functions { get; set; } = new Hashtable();

        internal static Hashtable FunctionsLower { get; set; } = new Hashtable();
        #endregion

        #region Methods
        public static IStiAppFunction[] GetFunctions(bool isCompile, bool isCaseSensitive)
        {
            var list = new List<IStiAppFunction>();
            var tempFuncs = isCompile ? FunctionsToCompile : Functions;

            foreach (string functionName in tempFuncs.Keys)
            {
                var functionsList = GetFunctions(functionName, isCompile, isCaseSensitive);
                list.AddRange(functionsList);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns array of functions with spefified name.
        /// </summary>
        public static IStiAppFunction[] GetFunctions(string functionName, bool isCompile, bool isCaseSensitive)
        {
            var functions = isCompile ? FunctionsToCompile : Functions;
            var functionsLower = isCompile ? FunctionsToCompileLower : FunctionsLower;

            var list = functions[functionName] as List<IStiAppFunction>;
            if (list != null)
                return list.ToArray();

            if (isCaseSensitive)
                list = functions[functionName] as List<IStiAppFunction>;

            else
                list = functionsLower[functionName.ToLowerInvariant()] as List<IStiAppFunction>;

            return list?.ToArray();
        }
        #endregion

        static StiAppFunctions()
        {
        }
    }
}
