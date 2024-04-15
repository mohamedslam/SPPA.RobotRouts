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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Stimulsoft.Report.Dictionary
{
    public class StiFunctionsXmlParser
    {
        public static bool TryLoadDefaultFunctions()
        {
            try
            {
                LoadDefaultFunctions();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void LoadDefaultFunctions()
        {
            LoadFunctionsFromXml(typeof(StiReport).Assembly, new[]
            {
                "Stimulsoft.Report.Dictionary.Functions.FunctionsMath.xml",
                "Stimulsoft.Report.Dictionary.Functions.FunctionsDate.xml",
                "Stimulsoft.Report.Dictionary.Functions.FunctionsDrawing.xml",
                "Stimulsoft.Report.Dictionary.Functions.FunctionsProgramming.xml",
                "Stimulsoft.Report.Dictionary.Functions.FunctionsPrintState.xml",
                "Stimulsoft.Report.Dictionary.Functions.FunctionsStrings.xml",
                "Stimulsoft.Report.Dictionary.Functions.FunctionsTotals.xml"
            });
        }

        public static void LoadFunctionsFromXml(Assembly assembly, string[] resourceNames)
        {
            foreach (var resourceName in resourceNames)
            {
                LoadFunctionsFromXml(assembly, resourceName);
            }
        }

        public static void LoadFunctionsFromXml(Assembly assembly, string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    LoadFunctionsFromXml(stream);
                    stream.Close();
                }
                else
                    throw new Exception($"Can't find xml file with function descriptions '{resourceName}' in resources");
            }
        }

        public static void LoadFunctionsFromXml(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                LoadFunctionsFromXml(stream);
                stream.Close();
            }
        }

        public static void LoadFunctionsFromXml(Stream stream)
        {
            var reader = new XmlTextReader(stream);
            reader.DtdProcessing = DtdProcessing.Ignore;

            var category = string.Empty;
            var groupFunctionName = string.Empty;
            var functionName = string.Empty;
            var description = string.Empty;

            Type typeOfFunction = null;
            Type returnType = null;

            var returnDescription = string.Empty;
            var argumentTypes = new List<Type>();
            var argumentNames = new List<string>();
            var argumentDescriptions = new List<string>();

            reader.ReadStartElement();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name.ToLower(CultureInfo.InvariantCulture))
                    {
                        case "category":
                            category = reader.GetAttribute(0);
                            break;

                        case "group":
                            groupFunctionName = reader.ReadString();
                            break;

                        case "name":
                            functionName = reader.ReadString();
                            break;

                        case "description":
                            description = reader.ReadString();
                            break;

                        case "typeoffunction":
                            var value = reader.ReadString();
                            if (value.ToLower(CultureInfo.InvariantCulture) == "isnull")
                                typeOfFunction = null;
                            else
                            {
                                typeOfFunction = StiTypeFinder.GetType(value);
                                if (typeOfFunction == null)
                                    throw new Exception($"Function Xml Parser: {functionName}, Type {value} is not founded!");
                            }
                            break;

                        case "returntype":
                            returnType = StiTypeFinder.GetType(reader.ReadString());
                            break;

                        case "returndescription":
                            returnDescription = reader.ReadString();
                            break;

                        case "argumenttype":
                            {
#if STIDRAWING
                                var type = reader.ReadString();
                                type = type.Replace("System.Drawing.Drawing2D.HatchStyle", "Stimulsoft.Drawing.Drawing2D.HatchStyle");
                                argumentTypes.Add(StiTypeFinder.GetType(type));
#else
                                argumentTypes.Add(StiTypeFinder.GetType(reader.ReadString()));
#endif

                                break;
                            }
                        case "argumentname":
                            argumentNames.Add(reader.ReadString());
                            break;

                        case "argumentdescription":
                            argumentDescriptions.Add(reader.ReadString());
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.ToLower(CultureInfo.InvariantCulture) == "function")
                {
                    var argTypes = argumentTypes.ToArray();
                    var argNames = argumentNames.ToArray();
                    var argDescriptions = argumentDescriptions.ToArray();

                    if (typeOfFunction == null)
                    {
                        StiFunctions.AddFunction(category, groupFunctionName, functionName,
                            description, typeof(object),
                            returnType, returnDescription,
                            argTypes, argNames,
                            argDescriptions).UseFullPath = false;
                    }
                    else
                    {
                        StiFunctions.AddFunction(category, groupFunctionName, functionName,
                            description, typeOfFunction,
                            returnType, returnDescription,
                            argTypes, argNames,
                            argDescriptions);
                    }

                    groupFunctionName = string.Empty;
                    functionName = string.Empty;
                    description = string.Empty;
                    typeOfFunction = null;
                    returnType = null;
                    returnDescription = string.Empty;

                    argumentTypes.Clear();
                    argumentNames.Clear();
                    argumentDescriptions.Clear();
                }
            }
            reader.Close();
        }
    }
}