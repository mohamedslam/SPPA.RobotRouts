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

using Stimulsoft.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Dictionary
{
    public class StiFunctions
    {
        #region Methods
        /// <summary>
        /// Removes all functions from report dictionary with specified name.
        /// </summary>
        /// <param name="functionName"></param>
        public static void RemoveFunction(string functionName)
        {
            if (StiAppFunctions.FunctionsToCompile[functionName] != null)
                StiAppFunctions.FunctionsToCompile.Remove(functionName);

            if (StiAppFunctions.FunctionsToCompileLower[functionName.ToLowerInvariant()] != null)
                StiAppFunctions.FunctionsToCompileLower.Remove(functionName.ToLowerInvariant());

            if (StiAppFunctions.Functions[functionName] != null)
                StiAppFunctions.Functions.Remove(functionName);

            if (StiAppFunctions.FunctionsLower[functionName.ToLowerInvariant()] != null)
                StiAppFunctions.FunctionsLower.Remove(functionName.ToLowerInvariant());
        }

        public static List<StiFunction> GetFunctionsList(string functionName)
        {
            if (StiAppFunctions.Functions[functionName] == null)
                return null;

            return (StiAppFunctions.Functions[functionName] as List<IStiAppFunction>)?.Cast<StiFunction>().ToList();
        }

        public static Hashtable GetFunctionsGrouppedInCategories()
        {
            var hash = new Hashtable();

            var functions = GetFunctions(false);
            foreach (var function in functions)
            {
                var list = hash[function.Category] as List<StiFunction>;
                if (list == null)
                {
                    list = new List<StiFunction>();
                    hash[function.Category] = list;
                }

                list.Add(function);
            }

            return hash;
        }

        public static List<StiFunction> GetFunctions(string category)
        {
            var functions = GetFunctions(false);
            var func = new List<StiFunction>();

            foreach (var function in functions)
            {
                if (function.Category == category)
                    func.Add(function);
            }

            return func;
        }

        public static List<string> GetCategories()
        {
            var hash = new Hashtable();

            var functions = GetFunctions(false);
            var categories = new List<string>();

            foreach (var function in functions)
            {
                if (hash[function.Category] == null)
                {
                    categories.Add(function.Category);
                    hash[function.Category] = function.Category;
                }
            }

            return categories;
        }

        /// <summary>
        /// Returns array of asseblies which contains functions.
        /// </summary>
        public static string[] GetAssebliesOfFunctions()
        {
            var functions = GetFunctions(true);
            var assemblies = new Hashtable();

            foreach (var function in functions)
            {
                if (function != null && function.TypeOfFunction.Assembly != null &&
                    !string.IsNullOrWhiteSpace(function.TypeOfFunction.Assembly.Location))
                    assemblies[function.TypeOfFunction.Assembly.Location] = function.TypeOfFunction.Assembly.FullName;
            }

            var asms = new string[assemblies.Count];
            assemblies.Keys.CopyTo(asms, 0);
            return asms;
        }

        /// <summary>
        /// Returns array of all functions.
        /// </summary>
        public static StiFunction[] GetFunctions(bool isCompile)
        {
            var list = new List<StiFunction>();
            var tempFuncs = isCompile ? StiAppFunctions.FunctionsToCompile : StiAppFunctions.Functions;

            foreach (string functionName in tempFuncs.Keys)
            {
                var functionsList = GetFunctions(null, functionName, isCompile);
                foreach (var function in functionsList)
                {
                    list.Add(function);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns array of functions with spefified name.
        /// </summary>
        public static StiFunction[] GetFunctions(StiReport report, string functionName, bool isCompile)
        {
            return StiAppFunctions.GetFunctions(functionName, isCompile, report == null || report.ScriptLanguage != StiReportLanguageType.VB)?
                .Cast<StiFunction>()?.ToArray();
        }

        /// <summary>
        /// Adds new function with specified parameters.
        /// </summary>
        /// <param name="category">Category of function.</param>
        /// <param name="functionName">Name of function.</param>
        /// <param name="typeOfFunction">Type which contain method of function.</param>
        /// <param name="returnType">Return type of function.</param>
        public static StiFunction AddFunction(string category, string functionName, Type typeOfFunction, Type returnType)
        {
            return AddFunction(category, functionName, string.Empty, typeOfFunction, returnType,
                string.Empty, null, null);
        }

        /// <summary>
        /// Adds new function with specified parameters.
        /// </summary>
        /// <param name="category">Category of function.</param>
        /// <param name="functionName">Name of function.</param>
        /// <param name="description">Description of function.</param>
        /// <param name="typeOfFunction">Type which contain method of function.</param>
        /// <param name="returnType">Return type of function.</param>
        /// <param name="returnDescription">Description of returns.</param>
        public static StiFunction AddFunction(string category, string functionName, string description, Type typeOfFunction, Type returnType,
            string returnDescription)
        {
            return AddFunction(category, functionName, description, typeOfFunction, returnType,
                returnDescription, null, null);
        }

        /// <summary>
        /// Adds new function with specified parameters.
        /// </summary>
        /// <param name="category">Category of function.</param>
        /// <param name="functionName">Name of function.</param>
        /// <param name="description">Description of function.</param>
        /// <param name="typeOfFunction">Type which contain method of function.</param>
        /// <param name="returnType">Return type of function.</param>
        /// <param name="returnDescription">Description of returns.</param>
        /// <param name="argumentTypes">Array which contain types of arguments.</param>
        /// <param name="argumentNames">Array which contain names of arguments.</param>
        public static StiFunction AddFunction(string category, string functionName, string description, Type typeOfFunction, Type returnType,
            string returnDescription, Type[] argumentTypes, string[] argumentNames)
        {
            return AddFunction(category, functionName, functionName, description, typeOfFunction, returnType,
                returnDescription, argumentTypes, argumentNames, null);
        }

        /// <summary>
		/// Adds new function with specified parameters.
		/// </summary>
		/// <param name="category">Category of function.</param>
		/// <param name="functionName">Name of function.</param>
		/// <param name="description">Description of function.</param>
		/// <param name="typeOfFunction">Type which contain method of function.</param>
		/// <param name="returnType">Return type of function.</param>
		/// <param name="returnDescription">Description of returns.</param>
		/// <param name="argumentTypes">Array which contain types of arguments.</param>
		/// <param name="argumentNames">Array which contain names of arguments.</param>
		/// <param name="argumentDescriptions">Array which contain descriptions of arguments.</param>
		public static StiFunction AddFunction(string category, string functionName, string description, Type typeOfFunction, Type returnType,
            string returnDescription, Type[] argumentTypes, string[] argumentNames, string[] argumentDescriptions)
        {
            return AddFunction(category, functionName, functionName, description, typeOfFunction, returnType,
                returnDescription, argumentTypes, argumentNames, null);
        }

        /// <summary>
        /// Adds new function with specified parameters.
        /// </summary>
        /// <param name="category">Category of function.</param>
        /// <param name="groupFunctionName">Name of group function. Can be same as function name.</param>
        /// <param name="functionName">Name of function.</param>
        /// <param name="description">Description of function.</param>
        /// <param name="typeOfFunction">Type which contain method of function.</param>
        /// <param name="returnType">Return type of function.</param>
        /// <param name="returnDescription">Description of returns.</param>
        /// <param name="argumentTypes">Array which contain types of arguments.</param>
        /// <param name="argumentNames">Array which contain names of arguments.</param>
        /// <param name="argumentDescriptions">Array which contain descriptions of arguments.</param>
        public static StiFunction AddFunction(string category, string groupFunctionName, string functionName, string description, Type typeOfFunction, Type returnType,
        string returnDescription, Type[] argumentTypes, string[] argumentNames, string[] argumentDescriptions)
        {
            if (string.IsNullOrEmpty(groupFunctionName))
                groupFunctionName = functionName;

            var function = new StiFunction(
                category,
                groupFunctionName,
                functionName, description,
                typeOfFunction, returnType,
                returnDescription, argumentTypes, argumentNames, argumentDescriptions);

            #region Functions
            var list = StiAppFunctions.Functions[groupFunctionName] as List<IStiAppFunction>;
            if (list == null)
            {
                list = new List<IStiAppFunction>();
                StiAppFunctions.Functions[groupFunctionName] = list;
                StiAppFunctions.FunctionsLower[groupFunctionName.ToLowerInvariant()] = list;
            }
            else
            {
                //check for duplicates
                foreach (object obj in list)
                {
                    var func = obj as StiFunction;
                    if ((func != null) && (func.FunctionName == function.FunctionName) && IsEqual(func, function))
                        return function;
                }
            }
            list.Add(function);
            #endregion

            #region Functions to Compile
            list = StiAppFunctions.FunctionsToCompile[functionName] as List<IStiAppFunction>;
            if (list == null)
            {
                list = new List<IStiAppFunction>();
                StiAppFunctions.FunctionsToCompile[functionName] = list;
                StiAppFunctions.FunctionsToCompileLower[functionName.ToLowerInvariant()] = list;
            }
            list.Add(function);
            #endregion

            return function;
        }

        private static bool IsEqual(StiFunction func1, StiFunction func2)
        {
            return (func1.Category == func2.Category) &&
                (func1.GroupFunctionName == func2.GroupFunctionName) &&
                (func1.FunctionName == func2.FunctionName) &&
                (func1.Description == func2.Description) &&
                (func1.TypeOfFunction.ToString() == func2.TypeOfFunction.ToString()) &&
                (func1.ReturnType.ToString() == func2.ReturnType.ToString()) &&
                (func1.ReturnDescription == func2.ReturnDescription) &&
                IsEqualArray(func1.ArgumentTypes, func2.ArgumentTypes) &&
                IsEqualArray(func1.ArgumentNames, func2.ArgumentNames) &&
                IsEqualArray(func1.ArgumentDescriptions, func2.ArgumentDescriptions);
        }

        private static bool IsEqualArray(Array arr1, Array arr2)
        {
            if (arr1 == null && arr2 == null)
                return true;

            if (arr1 == null || arr2 == null)
                return false;

            if (arr1.Length != arr2.Length)
                return false;

            for (int index = 0; index < arr1.Length; index++)
            {
                if (arr1.GetValue(index) != arr2.GetValue(index))
                    return false;
            }

            return true;
        }
        #endregion

        static StiFunctions()
        {
        }
    }
}
