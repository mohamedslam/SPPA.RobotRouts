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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.CodeDom;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.Dictionary
{
    public class StiFunction :
        IComparable,
        IStiAppFunction
    {
        #region IComparable
        public int CompareTo(object obj)
        {
            var function = obj as StiFunction;
            return this.FunctionName.CompareTo(function.FunctionName);
        }
        #endregion

        #region IStiAppCell
        string IStiAppCell.GetKey()
        {
            return Key;
        }

        void IStiAppCell.SetKey(string key)
        {
            Key = key;
        }
        #endregion

        #region IStiAppFunction
        string IStiAppFunction.GetName()
        {
            return FunctionName;
        }

        object IStiAppFunction.Invoke(IEnumerable<object> arguments)
        {
            var typeList = new List<Type>();
            foreach (var arg in arguments)
            {
                typeList.Add(arg.GetType());
            }

            var method = TypeOfFunction.GetMethod(FunctionName, typeList.ToArray());
            return method?.Invoke(null, arguments?.ToArray());
        }
        #endregion

        #region Properties
        public string Key { get; set; } = StiKeyHelper.GenerateKey();

        public bool UseFullPath { get; set; } = true;

        /// <summary>
        /// Gets or sets category of function.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets name of group.
        /// </summary>
        public string GroupFunctionName { get; set; }

        /// <summary>
        /// Gets or sets name of function.
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// Gets or sets description of function.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets type which contain method of function.
        /// </summary>
        public Type TypeOfFunction { get; set; }

        /// <summary>
        /// Gets or sets return type of function.
        /// </summary>
        public Type ReturnType { get; set; }

        /// <summary>
        /// Gets or sets description of returns.
        /// </summary>
        public string ReturnDescription { get; set; }

        /// <summary>
        /// Gets or sets array which contain types of arguments.
        /// </summary>
        public Type[] ArgumentTypes { get; set; }

        /// <summary>
        /// Gets or sets array which contain names of arguments.
        /// </summary>
        public string[] ArgumentNames { get; set; }

        /// <summary>
        /// Gets or sets array which contain descriptions of arguments.
        /// </summary>
        public string[] ArgumentDescriptions { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            return FunctionName;
        }

        public string GetLongFunctionString(StiReportLanguageType language)
        {
            if (language == StiReportLanguageType.CSharp || language == StiReportLanguageType.JS)
            {
                var sb = new StringBuilder();
                sb.Append(ConvertTypeToString(this.ReturnType, language));
                sb.Append("  ");
                sb.Append(this.FunctionName);
                sb.Append(" (");

                var index = 0;
                if (ArgumentTypes != null)
                {
                    foreach (var argumentType in this.ArgumentTypes)
                    {
                        var argumentName = this.ArgumentNames[index];
                        if (!argumentType.IsArray)
                        {
                            sb.Append(ConvertTypeToString(argumentType, language));
                            sb.Append(" ");
                        }
                        sb.Append(argumentName);
                        index++;
                        if (index != this.ArgumentTypes.Length)
                            sb.Append(", ");
                    }
                }

                sb.Append(")");
                return sb.ToString();
            }
            else
            {
                var sb = new StringBuilder();

                sb.Append(this.FunctionName);
                sb.Append("(");

                var index = 0;
                if (ArgumentTypes != null)
                {
                    foreach (var argumentType in this.ArgumentTypes)
                    {
                        var argumentName = this.ArgumentNames[index];
                        sb.Append(argumentName);
                        sb.Append(" As ");
                        sb.Append(ConvertTypeToString(argumentType, language));
                        index++;
                        if (index != this.ArgumentTypes.Length)
                            sb.Append(", ");
                    }
                }

                sb.Append(")");
                if (this.ReturnType != typeof(void))
                {
                    sb.Append(" As " + ConvertTypeToString(this.ReturnType, language));
                }

                return sb.ToString();
            }
        }

        public string GetFunctionString(StiReportLanguageType language)
        {
            return GetFunctionString(language, true);
        }

        public string GetFunctionString(StiReportLanguageType language, bool addFunctionName)
        {
            var sb = new StringBuilder();

            if (addFunctionName)
                sb.Append(this.FunctionName);
            sb.Append(" (");

            var index = 0;
            if (ArgumentTypes != null)
            {
                foreach (var argumentType in this.ArgumentTypes)
                {
                    var argumentName = this.ArgumentNames[index];

                    if (argumentType.IsArray)
                        sb.Append(argumentName);
                    else
                        sb.Append(ConvertTypeToString(argumentType, language));

                    index++;
                    if (index != this.ArgumentTypes.Length)
                        sb.Append(", ");
                }
            }

            sb.Append(")");
            if (this.ReturnType != typeof(void))
            {
                sb.Append(" : " + ConvertTypeToString(this.ReturnType, language));
            }

            return sb.ToString();
        }

        public string ConvertTypeToString(Type type, StiReportLanguageType language)
        {
            var typeStr = StiCodeDomSerializator.ConvertTypeToString(type, language);
            
            if (type == typeof(Color))
                return "Color";

            else if (type == typeof(StiSolidBrush))
                return "SolidBrush";

            else if (type == typeof(StiGradientBrush))
                return "GradientBrush";

            else if (type == typeof(StiGlareBrush))
                return "GlareBrush";

            else if (type == typeof(StiGlassBrush))
                return "GlassBrush";

            else if (type == typeof(StiHatchBrush))
                return "HatchBrush";

            else if (type == typeof(HatchStyle))
                return "HatchStyle";

            return string.IsNullOrEmpty(typeStr) 
                ? type.ToString() 
                : typeStr;
        }
        #endregion

        /// <summary>
        /// Creates a new object of the type StiFunction.
        /// </summary>
        /// <param name="category">Category of function.</param>
        /// <param name="functionName">Name of function.</param>
        /// <param name="typeOfFunction">Type which contain method of function.</param>
        /// <param name="returnType">Return type of function.</param>
        internal StiFunction(string category, string functionName, Type typeOfFunction, Type returnType) :
            this(category, functionName, string.Empty, typeOfFunction, returnType,
            string.Empty, null, null)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiFunction.
        /// </summary>
        /// <param name="category">Category of function.</param>
        /// <param name="functionName">Name of function.</param>
        /// <param name="description">Description of function.</param>
        /// <param name="typeOfFunction">Type which contain method of function.</param>
        /// <param name="returnType">Return type of function.</param>
        /// <param name="returnDescription">Description of returns.</param>
        internal StiFunction(string category, string functionName, string description, Type typeOfFunction, Type returnType,
            string returnDescription) :
            this(category, functionName, description, typeOfFunction, returnType,
            returnDescription, null, null)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiFunction.
        /// </summary>
        /// <param name="category">Category of function.</param>
        /// <param name="functionName">Name of function.</param>
        /// <param name="description">Description of function.</param>
        /// <param name="typeOfFunction">Type which contain method of function.</param>
        /// <param name="returnType">Return type of function.</param>
        /// <param name="returnDescription">Description of returns.</param>
        /// <param name="argumentTypes">Array which contain types of arguments.</param>
        /// <param name="argumentNames">Array which contain names of arguments.</param>
        internal StiFunction(string category, string functionName, string description, Type typeOfFunction, Type returnType,
            string returnDescription, Type[] argumentTypes, string[] argumentNames) :
            this(category, functionName, functionName, description, typeOfFunction, returnType,
                returnDescription, argumentTypes, argumentNames, null)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiFunction.
        /// </summary>
        /// <param name="category">Category of function.</param>
        /// <param name="groupFunctionName">Name of function group.</param>
        /// <param name="functionName">Name of function.</param>
        /// <param name="description">Description of function.</param>
        /// <param name="typeOfFunction">Type which contain method of function.</param>
        /// <param name="returnType">Return type of function.</param>
        /// <param name="returnDescription">Description of returns.</param>
        /// <param name="argumentTypes">Array which contain types of arguments.</param>
        /// <param name="argumentNames">Array which contain names of arguments.</param>
        /// <param name="argumentDescriptions">Array which contain descriptions of arguments.</param>
        internal StiFunction(string category, string groupFunctionName, string functionName, string description, Type typeOfFunction, Type returnType,
            string returnDescription, Type[] argumentTypes, string[] argumentNames, string[] argumentDescriptions)
        {
            this.Category = category;
            this.Description = description;
            this.ReturnDescription = returnDescription;
            this.GroupFunctionName = groupFunctionName;
            this.FunctionName = functionName;
            this.TypeOfFunction = typeOfFunction;
            this.ReturnType = returnType;
            this.ArgumentTypes = argumentTypes;
            this.ArgumentNames = argumentNames;
            this.ArgumentDescriptions = argumentDescriptions;

            if (argumentNames != null && argumentTypes != null && argumentNames.Length != argumentTypes.Length)
                throw new ArgumentException("Length of array 'argumentNames' must be equal to length of array 'argumentTypes'!");
        }
    }
}
