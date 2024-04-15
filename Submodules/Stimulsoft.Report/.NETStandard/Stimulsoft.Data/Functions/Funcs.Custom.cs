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
using Stimulsoft.Data.Exceptions;
using Stimulsoft.Data.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Data.Functions
{
    public partial class Funcs
    {
        public static bool ExistsCustomFunction(string funcName)
        {
            return GetCustomFunctions(funcName).Any();
        }

        public static IEnumerable<IStiAppFunction> GetCustomFunctions(string funcName)
        {
            var funcs = StiAppFunctions.GetFunctions(funcName, true, false);
            if (funcs != null)
                return funcs;

            return new List<IStiAppFunction>();
        }

        public static IStiAppFunction GetCustomFunction(string funcName, IEnumerable<Type> argumentTypes)
        {
            var funcs = GetCustomFunctions(funcName);
            return funcs.FirstOrDefault();
        }

        public static object InvokeCustomFunction(string funcName, IEnumerable<object> arguments)
        {
            var functions = GetCustomFunctions(funcName);
            if (functions.Count() == 1)
                return functions.FirstOrDefault().Invoke(arguments);

            var function = GetCustomFunction(funcName, arguments.Select(a => a != null ? a.GetType() : null));
            if (function == null)
                throw new StiFunctionNotFoundException(funcName);

            return function.Invoke(arguments);
        }
    }
}