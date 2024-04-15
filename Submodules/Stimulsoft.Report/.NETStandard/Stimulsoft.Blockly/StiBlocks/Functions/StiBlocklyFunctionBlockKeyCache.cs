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

using Stimulsoft.Report.Dictionary;
using System.Collections;

namespace Stimulsoft.Blockly.StiBlocks.Functions
{
    public static class StiBlocklyFunctionBlockKeyCache
    {
        #region Fields
        private static Hashtable cache;
        #endregion

        #region Methods
        public static string CreateKey(StiFunction function)
        {
            var key = $"{function.FunctionName}.";

            if (function.ArgumentNames != null)
            {
                for (int index = 0; index < function.ArgumentNames.Length; index++)
                {
                    key += $"{function.ArgumentNames[index]}.{function.ArgumentTypes[index].Name}";
                }
            }

            return key;
        }

        public static StiFunction GetFunction(string key)
        {
            if (cache != null)
                FillCache();

            return cache[key] as StiFunction;
        }

        private static void FillCache()
        {
            cache = new Hashtable();

            var functions = StiFunctions.GetFunctions(false);

            foreach (var func in functions)
            {
                cache.Add(CreateKey(func), func);
            }
        }

        public static Hashtable GetBlockKeyTable()
        {
            if (cache == null)
                FillCache();

            return cache;
        }
        #endregion
    }
}
