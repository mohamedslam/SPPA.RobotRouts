#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Stimulsoft.Report.Web
{
    public class StiBlocklyHelper
    {
        internal static string GetResourceFileText(string filename)
        {
            string result = string.Empty;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Stimulsoft.Report.Web.{filename}"))
            {
                using (var sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }

        internal static string GetToolboxXML(bool showCurrentValue)
        {
            var toolboxXML = GetResourceFileText("Designer.Blockly.Xml.BlocklyToolbox.xml");

            toolboxXML = toolboxXML.Replace("<!--CurrentValue-->", showCurrentValue ? GetResourceFileText("Designer.Blockly.Xml.BlocklyToolboxCurrentValue.xml") : "");

            var functionXML = StiFunctionBlocksParser.GetFunctionsGrouppedInCategoriesBlocks();
            toolboxXML = toolboxXML.Replace("<!--Functions-->", functionXML);

            return toolboxXML;
        }

        internal static string GetBlockyWorkspaceXML(string eventValue)
        {
            return string.IsNullOrEmpty(eventValue) ? GetResourceFileText("Designer.Blockly.Xml.BlocklyWorkspace.xml") : eventValue;
        }

        internal static string GetSampleXML()
        {
            return GetResourceFileText("Designer.Blockly.Xml.BlocklyToolbox.xml");
        }

        internal static void GetBlocklyInitParameters(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var eventValue = StiEncodingHelper.DecodeString(param["eventValue"] as string);
            var showCurrentValue = Convert.ToBoolean(param["showCurrentValue"]);

            callbackResult["params"] = new Hashtable()
            {
                ["toolboxXML"] = StiEncodingHelper.Encode(GetToolboxXML(showCurrentValue)),
                ["workspaceXML"] = StiEncodingHelper.Encode(GetBlockyWorkspaceXML(eventValue)),
                ["initBlocksJsCode"] = StiEncodingHelper.Encode(GetInitBlocksJsCode(report))
            };
        }

        internal static string GetInitBlocksJsCode(StiReport report)
        {
            var jsCode = GetResourceFileText("Designer.Blockly.Js.init_blockly_blocks.js");

            #region Drill Down
            var text = GetDrillDownFontList(report);
            if (!string.IsNullOrEmpty(text))
                jsCode = jsCode.Replace("[\"empty\", \"EMPTY\"]", text);

            text = GetDrillDownComponentList(report);
            if (!string.IsNullOrEmpty(text))
                jsCode = jsCode.Replace("[\"Component\", \"EMPTYCOMPONENT\"]", text);

            text = GetDrillDownComponentGetComponentsList(report);
            if (!string.IsNullOrEmpty(text))
                jsCode = jsCode.Replace("[\"ComponentGetComponents\", \"EMPTYCOMPONENTGETCOMPONENTS\"]", text);

            text = GetDrillDownVariableList(report);
            if (!string.IsNullOrEmpty(text))
                jsCode = jsCode.Replace("[\"Variable\", \"EMPTYVARIABLE\"]", text);

            text = GetDrillDownDataSourceList(report);
            if (!string.IsNullOrEmpty(text))
                jsCode = jsCode.Replace("[\"DataSource\", \"EMPTYDATASOURCE\"]", text);

            text = GetDrillDownSystenVariableList(report);
            if (!string.IsNullOrEmpty(text))
                jsCode = jsCode.Replace("[\"empty\", \"EMPTYSYSTEMVARIABLE\"]", text);

            text = GetDrillDownDataSourcePropertyList();
            if (!string.IsNullOrEmpty(text))
                jsCode = jsCode.Replace("[\"empty\", \"EMPTYDATASOURCEPROPERTY\"]", text);

            text = GetDrillDownDataSourceMethodsList();
            if (!string.IsNullOrEmpty(text))
                jsCode = jsCode.Replace("[\"empty\", \"EMPTYDATASOURCEMETHODS\"]", text);
            #endregion

            #region Fill Functions Blocks
            jsCode = jsCode.Replace("/*functions*/", StiFunctionBlocksParser.GetInitFunctionBlocks());
            #endregion

            return jsCode;
        }

        private static string GetDrillDownFontList(StiReport report)
        {
            var list = StiBlocklyFontHelper.LoadFonts(report);

            return GetDrillDownOptions(list);
        }

        private static string GetDrillDownComponentList(StiReport report)
        {
            var list = StiBlocklyComponentHelper.LoadComponents(report);

            return GetDrillDownOptions(list);
        }

        private static string GetDrillDownComponentGetComponentsList(StiReport report)
        {
            var list = StiBlocklyComponentHelper.LoadContainerComponents(report);

            return GetDrillDownOptions(list);
        }

        private static string GetDrillDownVariableList(StiReport report)
        {
            var list = StiBlocklyVariableHelper.LoadVariables(report);

            return GetDrillDownOptions(list);
        }

        private static string GetDrillDownDataSourceList(StiReport report)
        {
            var list = StiBlocklyDataSourceHelper.LoadDataSource(report);

            return GetDrillDownOptions(list);
        }

        private static string GetDrillDownDataSourcePropertyList()
        {
            var list = StiBlocklyDataSourceHelper.GetDataSourceProperties();

            return GetDrillDownOptions(list);
        }

        private static string GetDrillDownDataSourceMethodsList()
        {
            var list = StiBlocklyDataSourceHelper.GetDataSourceMethods();

            return GetDrillDownOptions(list);
        }

        private static string GetDrillDownSystenVariableList(StiReport report)
        {
            var list = StiSystemVariablesHelper.GetSystemVariables(report);

            return GetDrillDownOptions(list);
        }

        private static string GetDrillDownOptions(List<string> list)
        {
            var text = "";
            var index = 1;
            foreach (var name in list)
            {
                text = $"{text}[\"{name}\",\"{name}\"]";
                if (index != list.Count) text += ",";
                index++;
            }

            return text;
        }
    }
}