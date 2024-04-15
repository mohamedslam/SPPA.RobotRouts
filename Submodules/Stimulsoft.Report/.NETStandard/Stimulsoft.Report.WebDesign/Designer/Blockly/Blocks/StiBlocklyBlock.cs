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

using Stimulsoft.Base.Blocks;
using Stimulsoft.Base.Localization;
using Stimulsoft.Blockly.StiBlocks.Functions;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Helpers;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Stimulsoft.Report.Web
{
    public class StiBlocklyBlock
    {
        #region Properties
        public string Id { get; set; }

        public string Name { get; set; }

        public string Tooltip { get; set; }

        public string HelpUrl { get; set; }

        public bool PreviousStatement { get; set; }

        public bool NextStatement { get; set; }

        public bool InputsInline { get; set; }

        public bool Output { get; set; } = true;

        public string Color { get; set; } = "#2969b0";

        public List<StiBlocklyInput> ValueInputs { get; set; } = new List<StiBlocklyInput>();

        public StiFunction Function { get; set; }
        #endregion

        #region Methods
        public string GetJsDefinition()
        {
            var inputDefinitions = GetInputJsDefinitions();


            var result = $"Blockly.Blocks['{this.Id}'] = {{" +
                            "init: function() {" +
                                inputDefinitions +
                                $"this.setInputsInline({this.InputsInline.ToString().ToLower()});" +
                                $"this.setPreviousStatement({this.PreviousStatement.ToString().ToLower()}, null);" +
                                $"this.setNextStatement({this.NextStatement.ToString().ToLower()}, null);" +
                                $"this.setOutput({this.Output.ToString().ToLower()}, null);" +
                                $"this.setColour(\"{this.Color}\");" +
                                $"this.setTooltip(\"{this.Tooltip?.Replace("\"", "\\\"")}\");" +
                                $"this.setHelpUrl(\"{this.HelpUrl}\");" +
                                                "}" +
                            "};";

            return result;
        }

        private string GetInputJsDefinitions()
        {
            var result = string.Empty;

            foreach (var item in this.ValueInputs)
            {
                result += item.GetDefinitions();
            }

            return result;
        }

        public string GetXmlToolboxDefinition()
        {
            var result = $"<block type=\"{this.Id}\"></block>";

            return result;
        }

        public string GetTooltip(StiFunction function)
        {
            var informationText = function.GetLongFunctionString(Report.StiReportLanguageType.CSharp) + $" \\n\\n ";
            var description = function.Description;
            var argumentDescriptions = function.ArgumentDescriptions;
            var argumentNames = function.ArgumentNames;
            var argumentTypes = function.ArgumentTypes;
            var returnDescription = function.ReturnDescription;

            #region Function
            informationText += description + $" \\n\\n ";
            #endregion

            #region Arguments
            if (argumentNames != null && argumentNames.Length > 0)
            {
                informationText += $" {Loc.Get("PropertyMain", "Parameters")} \\n\\n ";

                var argIndex = 0;
                foreach (var arg in argumentNames)
                {
                    if (argumentDescriptions != null && argIndex < argumentDescriptions.Length)
                    {
                        if (argumentTypes != null && argumentTypes[argIndex].IsArray)
                            informationText += argumentDescriptions[argIndex++] + $" \\n\\n ";
                        else
                            informationText += $" {arg} - {argumentDescriptions[argIndex++].Replace("\"", "").Replace("\r\n", "").Replace("\n", "").ToLower()} " + $" \\n\\n ";
                    }

                    else
                        informationText += arg + $" \\n\\n ";
                }
            }
            #endregion

            #region Return value
            if (!string.IsNullOrEmpty(returnDescription))
            {
                informationText += $" \\n\\n {Loc.Get("PropertyMain", "ReturnValue")} \\n\\n ";

                informationText += returnDescription;
            }
            #endregion

            var options = RegexOptions.None;
            var regex = new Regex("[ ]{2,}", options);
            informationText = regex.Replace(informationText, " ");

            return informationText;
        }
        #endregion

        public StiBlocklyBlock(StiFunction function)
        {
            this.Id = StiBlocklyFunctionBlockKeyCache.CreateKey(function);

            this.Function = function;

            this.Name = function.FunctionName;

            var functionStr = function.GetLongFunctionString(Report.StiReportLanguageType.CSharp);

            this.Tooltip = $"{functionStr} \\n\\n {function.Description}";

            this.Tooltip = GetTooltip(function);

            #region Name
            var dummyInput = new StiBlocklyDummyInput();
            dummyInput.Fields.Add(new StiBlocklyField() { Name = functionStr });
            this.ValueInputs.Add(dummyInput);
            #endregion

            #region Arguments
            foreach (var item in function.ArgumentNames)
            {
                var valueInput = new StiBlocklyValueInput() { Name = item };

                valueInput.Fields.Add(new StiBlocklyField() { Name = item });

                this.ValueInputs.Add(valueInput);
            }
            #endregion
        }
    }
}