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
using System.Globalization;
using System.Xml;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Reflection;
using System.ComponentModel.Design.Serialization;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using System.Collections.Generic;
using System.Linq;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Gauge;
using System.Threading;
using Stimulsoft.Data.Extensions;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Base.Blocks;
using System.Drawing;
using System.Drawing.Imaging;
    
#if NETSTANDARD
using Stimulsoft.System.Drawing;
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.CodeDom
{
    /// <summary>
    /// The class serves to serialize into a code of a report.
    /// </summary>
    public class StiCodeDomSerializator
    {
        internal static string[] IdentsToEnd =
        {
            "TotalPageCount",
            "PageNofM",
            "PageNumber",
            "TotalPageCountThrough",
            "PageNofMThrough",
            "PageNumberThrough",
            "IsFirstPage",
            "IsFirstPageThrough",
            "IsLastPage",
            "IsLastPageThrough",
            "GetAnchorPageNumber",
            "GetAnchorPageNumberThrough"
        };

        internal static string[] ReservedWords =
        {
            "States",
            "PageNumber",
            "Line",
            "Column",
            "Date",
            "Time",
            "Today",
            "BusinessObjectsStore",
            "ReportDataSources",
            "Variables",
            "AggregateFunctions",
            "Dictionary",
            "DataSources",
            "DataStore",
            "Script",
            "Language",
            "ReportFile",
            "Load",
            "Save",
            "ExportDocument",
            "Design",
            "Render",
            "Show",
            "Print",
            "Click",
            "Paint",
            "Exporting",
            "Exported",
            "Printing",
            "Printed",
            "Unit",
            "Convert",
            "Container",
            "Site",
            "MetaTags",
            "ReportResources",
            "Engine",
            "Progress",
            "SubReports",
            "Tag",
            "Pages",
            "Info",
            "Designer",
            "Bookmark",
            "Password",
            "Styles",
            "PrinterSettings",
            "About"
        };

        #region Consts
        public const string StrGenCode = "StiReport Designer generated code - do not modify";
        public const string StiCheckerInfoString = "CheckerInfo:";
        #endregion

        #region Fields
        internal int TotalIndex = 1;

        internal CodeMemberMethod memberMethod;

        /// <summary>
        /// The report for what the script is generated.
        /// </summary>
        internal StiReport report;

        internal StiComponentsCollection components;

        private string thisReportTypeName;

        private ArrayList reference = new ArrayList();

        /// <summary>
        /// Collection of components for what the events OnBeginRender and OnEndRender should be created.
        /// </summary>
        private ArrayList compsRemittedSE = new ArrayList();

        /// <summary>
        /// Collection of components for what the event OnBuild is to be created.
        /// </summary>
        private ArrayList compsRemittedBuild = new ArrayList();

        private List<StiRemit> compsProcessAtEndRemitted = new List<StiRemit>();

        private List<StiRemit> compsWordsToEndRemitted = new List<StiRemit>();

        /// <summary>
        /// Code provider.
        /// </summary>
        internal StiCodeDomProvider provider;

        /// <summary>
        /// Code generator.
        /// </summary>
        internal StiCodeGenerator generator;

        /// <summary>
        /// Collection of graphs.
        /// </summary>
        private StiGraphs Graphs;

        /// <summary>
        /// Collection of the generating class members.
        /// </summary>
        internal CodeTypeMemberCollection Members = new CodeTypeMemberCollection();

        /// <summary>
        /// Collection of method operators of the generating class initialization.
        /// </summary>
        internal CodeStatementCollection Statements = new CodeStatementCollection();

        /// <summary>
        /// Collection of operators to define a collection of the generated class.
        /// </summary>
        private CodeStatementCollection CollectionStatements = new CodeStatementCollection();
        #endregion

        #region Events generation
        /// <summary>
        /// Generate a code for the GetValue event.
        /// </summary>
        /// <param name="eventName">Event name.</param>
        /// <param name="parent">Event parent.</param>
        /// <param name="comp">Component that generates event.</param>
        private void GenGetValueEvent(StiEvent ev, string propName, string eventName, StiExpression expression, string parent,
            object comp)
        {
            //if (comp == null)return;
            string originalText = expression.Value;

            string compName = string.Empty;

            if (comp is StiComponent) compName = StiNameValidator.CorrectName(((StiComponent)comp).Name, report);

            #region Process Only Text property
            bool onlyText = false;
            if (comp is IStiText)
            {
                onlyText = ((IStiText)comp).OnlyText;
            }
            #endregion

            #region Prepare text from rich text
            string text = null;
            var richText = comp as StiRichText;
            text = richText != null ? XmlConvert.DecodeName(expression.Value) : expression.Value;
            #endregion

            #region Process Functions
            bool notGenEvent = true;
            if ((!onlyText) && comp != null)
            {
                if (comp is StiSeries)
                {
                    var comp2 = new StiText();
                    StiCodeDomFunctions.ParseFunctions(this, propName, comp2, ref text, expression);
                }
                if (comp is StiComponent)
                    notGenEvent = StiCodeDomFunctions.ParseFunctions(this, propName, comp as StiComponent, ref text, expression);
                text = StiCodeDomTotalsFunctionsParser.ProcessTotals(this, text, parent);

            }
            #endregion

            #region Process Format
            StiFormatService format = null;

            if (comp is IStiTextFormat)
                format = ((IStiTextFormat)comp).TextFormat;

            if (comp is IStiCrossTabField)
                format = null;//Do not remove

            if (!expression.ApplyFormat) format = null;

            bool fullConvert = expression.FullConvert;
            #endregion

            bool isTextExpression = propName == "Text";
            string propNameForChecker = propName;

            if (!notGenEvent && (ev is StiGetValueEvent || ev is StiGetExcelValueEvent))
            {
                CodeExpression expr = null;
                if ((comp is StiText) && ((comp as StiText).CanShrink || (comp as StiText).CanGrow) && !text.Contains("sender."))
                {
                    expr = GetTextScriptExpression(format, text, fullConvert, onlyText, compName, null, false);
                }

                GenGetValueEventMethod(eventName, true, "#%#" + originalText, ev, expr);
                if (expression.GenAddEvent) GenAddEvent(eventName, parent, ev);

                if (Members.Count > 0)
                {
                    string parentName = parent;
                    if (comp is StiComponent) parentName = (comp as StiComponent).Name;

                    if (comp is StiSeries)
                    {
                        StiComponent tempComp = (comp as StiSeries).Chart as StiComponent;
                        if (tempComp != null)
                            parentName = tempComp.Name + ".Series";
                    }

                    var ctm = Members[Members.Count - 1] as CodeMemberMethod;
                    if (ctm != null)
                        ctm.Statements.Insert(0, new CodeCommentStatement(GetCheckerInfoString(parentName, propNameForChecker)));
                    //propNameForChecker += "_End";
                }

                string methodName = "_GetValue_End";
                if (ev is StiGetExcelValueEvent) methodName = "_GetExcelValue_End";

                GenReturnMethod(compName, compName + methodName, text, fullConvert, onlyText, format,
                    comp as StiComponent, isTextExpression);
            }
            else
            {
                if (expression.GenAddEvent)
                    GenAddEvent(eventName, parent, ev);

                if (!(comp is StiComponent))
                    compName = "Test";

                GenGetValueEventMethod(compName, eventName, format, text, fullConvert, onlyText,
                    ev, comp as StiComponent, isTextExpression);
            }

            if (Members.Count > 0)
            {
                string parentName = parent;
                if (comp is StiComponent)
                    parentName = (comp as StiComponent).Name;

                if (comp is StiSeries)
                {
                    StiComponent tempComp = (comp as StiSeries).Chart as StiComponent;
                    if (tempComp != null)
                        parentName = tempComp.Name + ".Series";
                }

                var ctm = Members[Members.Count - 1] as CodeMemberMethod;
                if (ctm != null)
                    ctm.Statements.Insert(0, new CodeCommentStatement(GetCheckerInfoString(parentName, propNameForChecker)));
            }
        }


        /// <summary>
        /// Generates a code to turn on a method to the event handler.
        /// </summary>
        /// <param name="eventName">Event name.</param>
        /// <param name="parent">Parent of event.</param>
        /// <param name="ev">Class describes an event.</param>
        internal void GenAddEvent(string eventName, string parent, StiEvent ev)
        {
            Type eventType = ev.GetEventType();

            CodeExpression ce = null;
            if (string.IsNullOrEmpty(parent))
                ce = new CodeThisReferenceExpression();
            else
                ce = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), parent);

            var attachStatement =
                new CodeAttachEventStatement(
                ce, ev.ToString(),
                new CodeDelegateCreateExpression(
                new CodeTypeReference(eventType),
                new CodeThisReferenceExpression(), eventName));

            Statements.Add(attachStatement);
        }


        /// <summary>
        /// Generate a code of the event method GetValue.
        /// </summary>
        /// <param name="eventName">Event name.</param>
        private void GenGetValueEventMethod(string compName, string eventName, StiFormatService format,
            string expression, bool fullConvert, bool onlyText, StiEvent ev, StiComponent component, bool isTextExpression)
        {
            bool needComponent = !(component is StiRichText) || eventName.EndsWith("_GetValue");
            var stats = new CodeStatementCollection();
            CodeStatement stat = new CodeAssignStatement(
                new CodeFieldReferenceExpression(
                new CodeArgumentReferenceExpression("e"), "Value"),
                GetTextScriptExpression(format, expression, fullConvert, onlyText, compName, needComponent ? component : null, isTextExpression));

            #region Process NullValue of text expression
            if (component is StiText && isTextExpression)
            {
                StiText textComp = component as StiText;
                if (textComp.Type == StiSystemTextType.DataColumn && !string.IsNullOrEmpty(textComp.NullValue))
                {
                    if (expression.StartsWith("{", StringComparison.InvariantCulture) && expression.EndsWith("}", StringComparison.InvariantCulture) && expression.Length > 2)
                    {
                        expression = expression.Substring(1, expression.Length - 2);

                        var nullStat = new CodeAssignStatement(
                            new CodeFieldReferenceExpression(
                            new CodeArgumentReferenceExpression("e"), "Value"),
                            new CodePrimitiveExpression(textComp.NullValue));

                        var condition = new CodeMethodInvokeExpression(
                            new CodeTypeReferenceExpression("StiNullValuesHelper"), "IsNull", new CodeThisReferenceExpression(), new CodePrimitiveExpression(expression));

                        stat = new CodeConditionStatement(condition,
                            new[] { nullStat },
                            new[] { stat });
                    }

                }
            }
            #endregion

            stats.Add(stat);

            GenEventMethod(eventName, ev, stats);
        }


        internal static string GetParentRelationName(StiReport report, string name)
        {
            string newName = StiNameValidator.CorrectName(name, report);
            if (newName.IndexOf("Parent", StringComparison.InvariantCulture) != 0) newName = "Parent" + newName;

            if (report != null && report.Dictionary != null && report.Dictionary.DataSources != null)
            {
                foreach (StiDataSource ds in report.Dictionary.DataSources)
                {
                    if (ds.Name == newName || StiNameValidator.CorrectName(ds.Name) == newName)
                    {
                        newName = "Parent" + newName;
                        break;
                    }
                }
            }

            return newName;
        }


        internal CodeExpression GetTextScriptExpression(StiFormatService format, string expression,
            bool fullConvert, bool onlyText, string compName, StiComponent component, bool isTextExpression)
        {
            if (format is StiGeneralFormatService || format == null)
            {
                string text = StiCodeDomExpressionHelper.ConvertToString(generator, expression, fullConvert, onlyText, component, isTextExpression);

                return new CodeSnippetExpression(text);
            }

            string scriptNew = StiCodeDomExpressionHelper.ConvertToString(generator, expression, false, onlyText, component, isTextExpression);

            if (format is StiCustomFormatService)
            {
                CodeExpression codeExpression = null;
                if (format.IsFormatStringFromVariable)
                {
                    codeExpression = new CodeSnippetExpression("\"{0:\" + " + format.StringFormat.Trim().Substring(1, format.StringFormat.Length - 2) + " + \"}\"");
                    return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), "Format", codeExpression, new CodeSnippetExpression(scriptNew));
                }
                else
                {
                    codeExpression = new CodePrimitiveExpression("{0:" + format.StringFormat + "}");
                    return new CodeMethodInvokeExpression(
                        new CodePropertyReferenceExpression(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), compName), "TextFormat"),
                        "Format", new CodeSnippetExpression(scriptNew));
                }
            }

            #region Fill ExcelValue of text component if ExcelValue expression is empty
            StiText textComp = component as StiText;
            if (!(textComp == null || (textComp != null && textComp.ExcelValue.Value != null && textComp.ExcelValue.Value.Length > 0)))
            {
                scriptNew = "CheckExcelValue(sender, " + scriptNew + ")";
            }
            #endregion

            return new CodeMethodInvokeExpression(
                new CodePropertyReferenceExpression(
                new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(), compName), "TextFormat"),
                "Format", new CodeSnippetExpression(scriptNew));
        }


        private static string RemoveComments(string text)
        {
            if ((text == null) || (text.Length < 2) || (text.Trim().Length < 2)) return text;

            bool isMultilineComment = false;
            bool isLineComment = false;
            bool isString = false;
            StringBuilder output = new StringBuilder();
            for (int index = 0; index < text.Length; index++)
            {
                char sym = text[index];
                if (!isString)
                {
                    if (isMultilineComment)
                    {
                        if ((sym == '*') && (index < text.Length - 1) && (text[index + 1] == '/'))
                        {
                            isMultilineComment = false;
                            index++;
                        }
                        continue;
                    }
                    if (isLineComment)
                    {
                        if ((sym == '\r') || (sym == '\n'))
                        {
                            isLineComment = false;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if ((sym == '/') && (index < text.Length - 1) && (text[index + 1] == '*'))
                    {
                        isMultilineComment = true;
                        index++;
                        continue;
                    }
                    if ((sym == '/') && (index < text.Length - 1) && (text[index + 1] == '/'))
                    {
                        isLineComment = true;
                        index++;
                        continue;
                    }
                }
                if (sym == '\\')
                {
                    output.Append(sym);
                    if (index < text.Length - 1)
                    {
                        index++;
                        output.Append(text[index]);
                    }
                    continue;
                }
                if (sym == '"')
                {
                    isString = !isString;
                }
                output.Append(sym);
            }
            return output.ToString();
        }

        /// <summary>
        /// Generate a code of the event method.
        /// </summary>
        /// <param name="eventName">Event name.</param>
        /// <param name="script">Event script.</param>
        /// <param name="ev">Class describes an event.</param>
        internal void GenEventMethod(string eventName, string script, StiEvent ev, string parent)
        {
            CodeStatementCollection stats = new CodeStatementCollection();

            if (StiOptions.Engine.RemoveCommentsFromEventScript)
            {
                script = RemoveComments(script);
            }

            script = StiCodeDomTotalsFunctionsParser.ProcessTotals(this, script, parent);

            stats.Add(new CodeCommentStatement(StiCodeDomSerializator.GetCheckerInfoString(string.IsNullOrEmpty(parent) ? "Report" : parent, ev.ToString() + "Event")));

            stats.Add(new CodeSnippetExpression(script));

            GenEventMethod(eventName, ev, stats);
        }



        /// <summary>
        /// Generate a code of the event method GetValue and indicates are text components to be saved when printing or not.
        /// </summary>
        /// <param name="eventName">Event name.</param>
        /// <param name="storeToPrinted">Save or do not save text components when printing.</param>
        /// <param name="script">Event script.</param>
        /// <param name="ev">Class describes an event..</param>
        private void GenGetValueEventMethod(string eventName, bool storeToPrinted, string script, StiEvent ev, CodeExpression expr = null)
        {
            CodeStatementCollection stats = new CodeStatementCollection();

            if (storeToPrinted)
            {
                stats.Add(new CodeAssignStatement(
                    new CodeFieldReferenceExpression(
                    new CodeArgumentReferenceExpression("e"), "StoreToPrinted"),
                    new CodePrimitiveExpression(true)));
            }

            if (script.StartsWith("#%#", StringComparison.InvariantCulture))
            {
                if (expr != null)
                {
                    stats.Add(new CodeAssignStatement(
                        new CodeFieldReferenceExpression(
                        new CodeArgumentReferenceExpression("e"), "Value"),
                        new CodeBinaryOperatorExpression(
                            new CodePrimitiveExpression("#%#"),
                            CodeBinaryOperatorType.Add,
                            expr)));
                }
                else
                {
                    stats.Add(new CodeAssignStatement(
                        new CodeFieldReferenceExpression(
                        new CodeArgumentReferenceExpression("e"), "Value"),
                        new CodePrimitiveExpression(script)));
                }
            }
            else
            {
                script = StiCodeDomTotalsFunctionsParser.ProcessTotals(this, script, null);

                stats.Add(new CodeAssignStatement(
                    new CodeFieldReferenceExpression(
                    new CodeArgumentReferenceExpression("e"), "Value"),
                    new CodeSnippetExpression(
                    script)));
            }

            GenEventMethod(eventName, ev, stats);
        }


        /// <summary>
        /// Generate a code of the event method.
        /// </summary>
        /// <param name="eventName">Event name.</param>
        /// <param name="ev">Class describes an event.</param>
        /// <param name="stats">Event operators.</param>
        internal void GenEventMethod(string eventName, StiEvent ev, CodeStatementCollection stats)
        {
            CodeMemberMethod eventMethod = new CodeMemberMethod();
            eventMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            eventMethod.Name = eventName;

            CodeParameterDeclarationExpressionCollection parametersCollection =
                new CodeParameterDeclarationExpressionCollection();

            StiParameterInfo[] parameters = ev.GetParameters();

            foreach (StiParameterInfo parameter in parameters)
            {
                CodeParameterDeclarationExpression cp =
                    new CodeParameterDeclarationExpression(parameter.Type, parameter.Name);

                parametersCollection.Add(cp);
            }

            eventMethod.Parameters.AddRange(parametersCollection);

            eventMethod.Statements.AddRange(stats);
            Members.Add(eventMethod);
        }


        /// <summary>
        /// Adds an event.
        /// </summary>
        /// <param name="parent">Parent name.</param>
        /// <param name="prop">Description.</param>
        internal void AddEvent(string parent, StiPropertyInfo prop)
        {
            AddEvent(parent, prop.Value as StiEvent);
        }

        /// <summary>
        /// Adds an event.
        /// </summary>
        /// <param name="parent">Parent name.</param>
        /// <param name="ev">Event.</param>
        internal void AddEvent(string parent, StiEvent ev)
        {
            if (string.IsNullOrWhiteSpace(ev.Script))return;
            
            var text = ev.Script;
            text = StiCodeDomFunctions.ParseFunctions(this, text);

            var eventName = parent.Length > 0 && parent != null 
                ? $"{parent}_{ev}" 
                : $"{report.GetReportName()}_{ev}";

            GenAddEvent(eventName, parent, ev);
            GenEventMethod(eventName, text, ev, parent);
        }
        #endregion

        public static string GetCheckerInfoString(string componentName, string propertyName)
        {
            return string.Format("{0} {1} {2}", StiCheckerInfoString, propertyName, componentName);
        }

        /// <summary>
        /// Generates a method that returns the string expression.
        /// </summary>
        /// <param name="methodName">Method name.</param>
        private void GenReturnMethod(string compName, string methodName,
            string expression, bool fullConvert, bool onlyText, StiFormatService format, StiComponent component,
            bool isTextExpression)
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            method.Name = methodName;
            method.ReturnType = new CodeTypeReference(typeof(string));
            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(StiComponent), "sender"));

            method.Statements.Add(
                new CodeMethodReturnStatement(
                GetTextScriptExpression(format, expression, fullConvert, onlyText, compName, component, isTextExpression)));

            Members.Add(method);
        }


        internal void GenReturnMethodForExpresion(string methodName, string expression, Type returnType)
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            method.Name = methodName;
            method.ReturnType = new CodeTypeReference(returnType);

            method.Statements.Add(
                new CodeMethodReturnStatement(new CodeSnippetExpression(expression)));

            //check for duplicates
            foreach (var member in Members)
            {
                CodeMemberMethod method1 = member as CodeMemberMethod;
                if ((method1 != null) && (method1.Name == methodName) && (method1.Attributes == method.Attributes) && (method1.ReturnType.BaseType == returnType.ToString()) && (method1.Statements.Count == 2))
                {
                    CodeMethodReturnStatement stat = method1.Statements[1] as CodeMethodReturnStatement;
                    if ((stat != null) && (stat.Expression is CodeSnippetExpression) && ((stat.Expression as CodeSnippetExpression).Value == expression)) return;
                }
            }

            Members.Add(method);
        }


        /// <summary>
        ///  Adds a record to the collection of events (OnBeginRender and OnEndRender).
        /// </summary>
        /// <param name="name">A name of the component to what a handler is turned on.</param>
        /// <param name="comp">Component.</param>
        /// <param name="funcName">Function name.</param>
        /// <param name="scriptCalcItem">Script to calculate functions.</param>
        /// <param name="scriptGetValue">Script to get value.</param>
        /// <param name="function">Aggegate function.</param>
        public StiRemit AddToRemittedSE(
            string name,
            StiComponent comp,
            string funcName,
            string scriptCalcItem,
            string scriptGetValue,
            StiAggregateFunctionService function,
            bool usePrintEvent,
            bool useNewColumnEventEvent,
            bool isRunningTotal,
            StiExpression expression)
        {
            name = GetCompNameAggregateBeginEnd(name);
            int index = compsRemittedSE.IndexOf(name);
            if (index == -1)
            {
                compsRemittedSE.Add(name);
                compsRemittedSE.Add(new ArrayList());
                index = compsRemittedSE.Count - 2;
            }

            var al = (ArrayList)compsRemittedSE[index + 1];

            var remit = new StiRemit(comp, funcName, scriptCalcItem, scriptGetValue,
                string.Empty, function, usePrintEvent, useNewColumnEventEvent, expression, isRunningTotal);

            al.Add(remit);
            return remit;
        }


        /// <summary>
        /// Adds a record to the collection of events (OnBuild).
        /// </summary>
        /// <param name="name">A name of the component to what a handler is turned on.</param>
        /// <param name="comp">Component.</param>
        /// <param name="funcName">Function name.</param>
        /// <param name="scriptCalcItem">Script to calculate functions.</param>
        /// <param name="scriptGetValue">Script to get value.</param>
        /// <param name="function">Aggegate function.</param>
        public void AddToRemittedBuild(
            string name,
            StiComponent comp,
            string funcName,
            string scriptCalcItem,
            string scriptGetValue,
            string scriptCondition,
            StiAggregateFunctionService function,
            StiExpression expression,
            bool isRunningTotal)
        {
            name = GetCompNameAggregateBuild(name);
            int index = compsRemittedBuild.IndexOf(name);
            if (index == -1)
            {
                compsRemittedBuild.Add(name);
                compsRemittedBuild.Add(new ArrayList());
                index = compsRemittedBuild.Count - 2;
            }

            var al = (ArrayList)compsRemittedBuild[index + 1];

            al.Add(new StiRemit(comp, funcName, scriptCalcItem, scriptGetValue,
                scriptCondition, function, false, false, expression, isRunningTotal));
        }


        /// <summary>
        ///  Tests whether the collection is located in the component which formation is delayed to the end.
        /// </summary>
        /// <param name="comp">Component.</param>
        /// <returns>Test results.</returns>
        public bool ContainsRemittedComponentInLast(StiComponent comp)
        {
            foreach (StiRemit remit in compsProcessAtEndRemitted)
                if (remit.Component == comp) return true;

            foreach (StiRemit remit in compsWordsToEndRemitted)
                if (remit.Component == comp) return true;
            return false;
        }


        /// <summary>
        /// Tests, is this type simple.
        /// </summary>
        /// <param name="type">Under the test type.</param>
        /// <returns>Test results.</returns>
        private bool IsSimple(Type type)
        {
            if (type == typeof(global::System.Decimal)) return true;
            return !((type.IsEnum || type.IsValueType) && !type.IsPrimitive);
        }


        /// <summary>
        /// Whether it is possible to convert a type into the InstanceDescriptor.
        /// </summary>
        /// <param name="type">Under the test type.</param>
        /// <returns>Result of a test.</returns>
        private bool CanConvertTo(Type type)
        {
            return TypeDescriptor.GetConverter(type).CanConvertTo(typeof(InstanceDescriptor));
        }


        /// <summary>
        /// Forms an expression of the specified value.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Formed expression.</returns>
        private CodeExpression GetValueExpression(object value)
        {
            if (!IsSimple(value.GetType())) return GetPropertyExpression(value);
            else return new CodePrimitiveExpression(value);
        }


        /// <summary>
        /// Returns a type of elements in the collection.
        /// </summary>
        /// <param name="type">Collection type.</param>
        /// <returns>Collection element type.</returns>
        private Type GetTypeCollection(Type type)
        {
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (prop.Name == "Item")
                    return prop.PropertyType;
            }
            return typeof(object);
        }


        public static string ConvertExpressionToString(StiCodeGenerator generator, CodeExpression expression)
        {
            CodeGeneratorOptions generatorOptions = new CodeGeneratorOptions();
            generatorOptions.BlankLinesBetweenMembers = true;
            generatorOptions.BracingStyle = "C";

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            generator.GenerateCodeFromExpression(expression, sw, generatorOptions);

            sw.Flush();
            sw.Close();
            return sb.ToString();
        }


        /// <summary>
        /// Checks whether it is necessary to reserve a formation of a component to the end of the report rendering.
        /// </summary>
        /// <param name="comp">Component for test.</param>
        /// <param name="text">Component script.</param>
        /// <returns>true if it is necessary to reserve a formation of a component to the end of the report rendering.</returns>
        internal bool TestProcessAtEnd(StiComponent comp, string text)
        {
            var textComp = comp as IStiText;
            var processAt = comp as IStiProcessAt;
            if (textComp != null && processAt != null)
            {
                #region If component have ProcessAt property and value of this property eqaul true
                if (processAt.ProcessAt == StiProcessAt.EndOfReport)
                {
                    compsProcessAtEndRemitted.Add(
                        new StiRemit(comp, string.Empty, "", text, "", null, false, false, null, false));
                    return true;
                }
                #endregion

                #region If component contain totals per page
                else if (text.Contains("Totals.c"))
                {
                    foreach (StiRemit remit in compsWordsToEndRemitted)
                    {
                        if (remit.Component == comp)
                            return false;
                    }
                    var remit2 = new StiRemit(comp, string.Empty, "", text, "", null, false, false, null, false);
                    remit2.isPageTotal = true;
                    compsWordsToEndRemitted.Add(remit2);
                    return true;
                }
                #endregion

                #region If component contain in text expression words which must be processed at end of report rendering
                else if (TestWordsToEnd(text))
                {
                    compsWordsToEndRemitted.Add(
                        new StiRemit(comp, string.Empty, "", text, "", null, false, false, null, false));
                    return true;
                }
                #endregion
            }
            return false;
        }


        /// <summary>
        /// Checks a text on the presence of modifiers which calculation is to be done in the end of a report.
        /// </summary>
        /// <param name="text">Under test text.</param>
        /// <returns>Test result.</returns>
        private bool TestWordsToEnd(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var list = StiCodeDomExpressionHelper.GetLexem(text);
                foreach (string item in list)
                {
                    if (item.StartsWith("{", StringComparison.InvariantCulture))
                    {
                        List<StiToken> tokens = null;

                        foreach (string str in IdentsToEnd)
                        {
                            if (item.IndexOf(str, StringComparison.InvariantCulture) >= 0)
                            {
                                #region List of tokens is empty
                                if (tokens == null)
                                {
                                    tokens = new List<StiToken>();
                                    var lexer = new StiLexer(text);
                                    var token = lexer.GetToken();
                                    tokens.Add(token);
                                    while (token.Type != StiTokenType.EOF)
                                    {
                                        if (token.Type == StiTokenType.Ident && token.Data.ToString() == str)
                                        {
                                            return true;
                                        }
                                        token = lexer.GetToken();
                                        tokens.Add(token);
                                    }
                                }
                                #endregion

                                #region List of tokens created in previous case
                                else
                                {
                                    foreach (var token in tokens)
                                    {
                                        if (token.Type == StiTokenType.EOF) break;

                                        if (token.Type == StiTokenType.Ident && token.Data.ToString() == str)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Adds comment.
        /// </summary>
        /// <param name="comment">Comment.</param>
        private void AddComment(string comment)
        {
            Statements.Add(new CodeCommentStatement(comment));
        }


        /// <summary>
        /// Adds a declaration of an object.
        /// </summary>
        /// <param name="prop">Description.</param>
        internal void AddDeclare(StiPropertyInfo prop)
        {
            AddDeclare(prop.Type, prop.Name);
        }


        /// <summary>
        /// Adds a declaration of an object.
        /// </summary>
        /// <param name="type">Field type.</param>
        /// <param name="name">Name.</param>
        internal void AddDeclare(Type type, string name)
        {
            CodeMemberField field = new CodeMemberField(type,
                StiNameValidator.CorrectName(name, report));
            field.Attributes = MemberAttributes.Public;
            Members.Add(field);
        }


        /// <summary>
        ///  Adds an object creation.
        /// </summary>
        /// <param name="parent">Parent name.</param>
        /// <param name="prop">Description.</param>
        internal void AddCreate(string parent, StiPropertyInfo prop, bool isList)
        {
            AddCreate(-1, parent, prop.Value, prop.Name, prop.Type, isList);
        }


        /// <summary>
        /// Adds an object creation.
        /// </summary>
        /// <param name="type">Object type.</param>
        /// <param name="name">Object name.</param>
        internal void AddCreate(Type type, string name)
        {
            Statements.Add(
                new CodeAssignStatement(
                new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(), name),
                new CodeObjectCreateExpression(type)));
        }


        /// <summary>
        /// Adds an object creation.
        /// </summary>
        /// <param name="index">Position for embedding.</param>
        /// <param name="parent">Parent name.</param>
        /// <param name="value">Object value.</param>
        /// <param name="name">Object name.</param>
        /// <param name="type">Object type.</param>
        internal void AddCreate(int index, string parent, object value, string name, Type type, bool isList)
        {
            name = StiNameValidator.CorrectName(name, report);
            if (value != null && parent.Length == 0)
            {
                Statements.Add(new CodeCommentStatement(string.Empty));
                Statements.Add(new CodeCommentStatement(name));
                Statements.Add(new CodeCommentStatement(string.Empty));
            }

            if (!isList)
            {
                CodeExpression left;
                CodeAssignStatement assignStatement;

                if (parent.Length > 0)
                    left = new CodeFieldReferenceExpression(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), parent), name);
                else left = new CodeFieldReferenceExpression(
                         new CodeThisReferenceExpression(), name);

                if (value != null)
                {
                    assignStatement = new CodeAssignStatement(left,
                        GetObjectCreateExpression(type, value));
                }
                else
                {
                    assignStatement = new CodeAssignStatement(left, new CodePrimitiveExpression(null));
                }

                if (index != -1) Statements.Insert(index, assignStatement);
                else Statements.Add(assignStatement);
            }
            else
            {
                if (value != null)
                {
                    CodeExpression create = new CodeMethodInvokeExpression(
                        new CodePropertyReferenceExpression(
                        new CodeThisReferenceExpression(), parent), "Add", GetObjectCreateExpression(type, value));

                    Statements.Add(create);
                }
            }
        }


        /// <summary>
        /// Adds initialization.
        /// </summary>
        /// <param name="parent">Parent name.</param>
        /// <param name="isReference">Whether this object is reference.</param>
        /// <param name="prop">Description</param>
        internal void AddInit(string parent, bool isReference, StiPropertyInfo prop)
        {
            AddInit(parent, isReference, prop, -1);
        }


        private CodeExpression GetVariableReference(string str)
        {
            switch (report.ScriptLanguage)
            {
                case StiReportLanguageType.CSharp:
                case StiReportLanguageType.JS:
                    return new CodeSnippetExpression("this." + str);

                default:
                    return new CodeSnippetExpression("me." + str);
            }
        }


        /// <summary>
        /// Adds initialization.
        /// </summary>
        /// <param name="parent">Parent name.</param>
        /// <param name="isReference">Whether this object is reference.</param>
        /// <param name="prop">Description.</param>
        /// <param name="pos">Position.</param>
        private void AddInit(string parent, bool isReference, StiPropertyInfo prop, int pos)
        {
            CodeExpression right;
            if (isReference)
            {
                if ((string)prop.Value != "this")
                {
                    right = GetVariableReference(prop.Value.ToString());
                }
                else
                {
                    right = new CodeThisReferenceExpression();
                }
            }
            else
            {
                right = GetValueExpression(prop.Value);
            }

            CodeExpression left;

            if (parent.Length > 0)
            {
                left = new CodeFieldReferenceExpression(
                    new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), parent), prop.Name);
            }
            else
            {
                left = new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), prop.Name);
            }

            CodeAssignStatement assignStatement = new CodeAssignStatement(left, right);

            if (pos != -1)
            {
                Statements.Insert(pos, assignStatement);
            }
            else
            {
                Statements.Add(assignStatement);
            }
        }


        /// <summary>
        /// Adds initialization StiDataSource.
        /// </summary>
        /// <param name="parent">Parent name.</param>
        /// <param name="prop">Description.</param>
        private void AddDataSource(string parent, StiPropertyInfo prop)
        {
            CodeExpression right;

            right = new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(),
                ReplaceSymbols(((StiDataSource)prop.Value).Name, report));

            CodeExpression left;

            if (parent.Length > 0)
            {
                left = new CodeFieldReferenceExpression(
                    new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), parent), ReplaceSymbols(prop.Name, report));
            }
            else
            {
                left = new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), ReplaceSymbols(prop.Name, report));
            }

            CodeAssignStatement assignStatement = new CodeAssignStatement(left, right);

            Statements.Add(assignStatement);
        }


        /// <summary>
        /// Adds initialization StiDataRelation.
        /// </summary>
        /// <param name="parent">Parent name.</param>
        /// <param name="prop">Description.</param>
        private void AddDataRelation(string parent, StiPropertyInfo prop)
        {
            CodeExpression right;

            right = new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(),
                GetParentRelationName(report, ((StiDataRelation)prop.Value).Name));

            CodeExpression left;

            if (parent.Length > 0)
            {
                left = new CodeFieldReferenceExpression(
                    new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), parent), GetParentRelationName(report, prop.Name));
            }
            else
            {
                left = new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), GetParentRelationName(report, prop.Name));
            }

            CodeAssignStatement assignStatement = new CodeAssignStatement(left, right);

            Statements.Add(assignStatement);
        }

        /// <summary>
        /// Adds initialization of color array.
        /// </summary>
        /// <param name="parent">Parent name.</param>
        /// <param name="prop">Description.</param>
        private void AddColorArray(string parent, StiPropertyInfo prop)
        {
            Color[] colors = prop.Value as Color[];

            CodeExpression left = new CodeFieldReferenceExpression(
                new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(), parent), prop.Name);

            CodeExpression[] pars = new CodeExpression[colors.Length];
            int index = 0;
            foreach (Color color in colors)
            {
                pars[index] = GetColorExpression(color);//new CodePrimitiveExpression(color);
                index++;
            }

            CodeExpression right = new CodeArrayCreateExpression(typeof(Color), pars);

            CodeAssignStatement assignStatement = new CodeAssignStatement(left, right);

            Statements.Add(assignStatement);
        }


        /// <summary>
        /// Adds initialization of byte array.
        /// </summary>
        /// <param name="parent">Parent name.</param>
        /// <param name="prop">Description.</param>
        private void AddByteArray(string parent, StiPropertyInfo prop)
        {
            var bytes = prop.Value as byte[];

            CodeExpression left;
            if (!string.IsNullOrWhiteSpace(parent))
            {
                left = new CodeFieldReferenceExpression(
                    new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), parent), prop.Name);
            }
            else
            {
                left = new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), prop.Name);
            }

            var bytesStr = Convert.ToBase64String(bytes);

            var right = new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression("System.Convert"), "FromBase64String",
                new CodePrimitiveExpression(bytesStr));

            var assignStatement = new CodeAssignStatement(left, right);

            Statements.Add(assignStatement);
        }


        private void AddImage(string parent, string name, StiPropertyInfo prop)
        {
            var left = string.IsNullOrEmpty(parent) ?
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), name) :
                new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), parent), name);

            if (prop.Value != null)
            {
                var image = prop.Value as Image;

                #region Process Globalization
                if (prop.Name == "Image" && prop.Parent != null)
                {
                    var globalizedName = prop.Parent.Value as IStiGlobalizedName;
                    if (globalizedName != null && !string.IsNullOrEmpty(globalizedName.GlobalizedName))
                    {
                        var comp = prop.Parent.Value as StiComponent;
                        if (comp != null && comp.Report != null && comp.Report.GlobalizationManager != null)
                        {
                            var newImage = comp.Report.GlobalizationManager.GetObject(globalizedName.GlobalizedName) as Image;
                            if (newImage != null) image = newImage;
                        }
                    }
                }
                #endregion

                var imageStr = StiImageConverter.ImageToString(image);

                var right = new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression("StiImageConverter"), "StringToImage",
                    new CodePrimitiveExpression(imageStr));

                Statements.Add(new CodeAssignStatement(left, right));

            }
            else
            {
                Statements.Add(new CodeAssignStatement(left, new CodePrimitiveExpression(null)));
            }
        }


        /// <summary>
        /// Adds an expression.
        /// </summary>
        /// <param name="parent">Parent name.</param>
        /// <param name="prop">Description.</param>
        private void AddExpression(string parent, StiPropertyInfo prop)
        {
            StiExpression expr = prop.Value as StiExpression;

            if (expr != null)
            {
                #region Process Globalization
                IStiGlobalizedName globalizedName = prop.Parent.Value as IStiGlobalizedName;
                bool isGlobalized = (globalizedName != null) && !string.IsNullOrWhiteSpace(globalizedName.GlobalizedName) && (prop.Parent != null);

                if (isGlobalized)
                {
                    #region Text property
                    if (prop.Name == "Text")
                    {
                        StiComponent comp = prop.Parent.Value as StiComponent;
                        if (comp != null && comp.Report != null && comp.Report.GlobalizationManager != null)
                        {
                            string text = comp.Report.GlobalizationManager.GetString(
                                globalizedName.GlobalizedName);

                            if (text != null)
                            {
                                StiExpression newExpr = expr.Clone() as StiExpression;
                                newExpr.Value = text;

                                expr = newExpr;
                            }
                        }
                    }
                    #endregion
                }
                if (isGlobalized && !string.IsNullOrWhiteSpace(expr.Value))
                {
                    #region Tag property
                    if (prop.Name == "Tag")
                    {
                        StiComponent comp = prop.Parent.Value as StiComponent;
                        if (comp != null && comp.Report != null && comp.Report.GlobalizationManager != null)
                        {
                            string text = comp.Report.GlobalizationManager.GetString(
                                globalizedName.GlobalizedName + ".Tag");

                            if (text != null)
                            {
                                StiExpression newExpr = expr.Clone() as StiExpression;
                                newExpr.Value = text;

                                expr = newExpr;
                            }
                        }
                    }
                    #endregion

                    #region ToolTip property
                    if (prop.Name == "ToolTip")
                    {
                        StiComponent comp = prop.Parent.Value as StiComponent;
                        if (comp != null && comp.Report != null && comp.Report.GlobalizationManager != null)
                        {
                            string text = comp.Report.GlobalizationManager.GetString(
                                globalizedName.GlobalizedName + ".ToolTip");

                            if (text != null)
                            {
                                StiExpression newExpr = expr.Clone() as StiExpression;
                                newExpr.Value = text;

                                expr = newExpr;
                            }
                        }
                    }
                    #endregion

                    #region Hyperlink property
                    if (prop.Name == "Hyperlink")
                    {
                        StiComponent comp = prop.Parent.Value as StiComponent;
                        if (comp != null && comp.Report != null && comp.Report.GlobalizationManager != null)
                        {
                            string text = comp.Report.GlobalizationManager.GetString(
                                globalizedName.GlobalizedName + ".Hyperlink");

                            if (text != null)
                            {
                                StiExpression newExpr = expr.Clone() as StiExpression;
                                newExpr.Value = text;

                                expr = newExpr;
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                if (!string.IsNullOrWhiteSpace(expr.Value))
                {
                    StiEvent ev = expr.GetDefaultEvent();
                    string eventName = parent + "__" + ev.ToString() + string.Empty;

                    GenGetValueEvent(ev, prop.Name, eventName, expr, parent, prop.Parent.Value);
                }
            }
        }


        private void AddHighlight(string parent, StiPropertyInfo prop)
        {
        }


        /// <summary>
        /// Adds a collection.
        /// </summary>
        /// <param name="parent">Parent name.</param>
        /// <param name="prop">Description.</param>
        /// <param name="names">List of collection object names.</param>
        private void AddCollection(string parent, StiPropertyInfo prop, List<string> names)
        {
            AddCollection(parent, prop.Name, prop.Type, names);
        }


        private void AddStringArrayList(StiPropertyInfo prop, string name, string parent)
        {
            var list = prop.Value as ArrayList;

            CodeExpression clear;
            if (parent.Length > 0)
                clear = new CodeMethodInvokeExpression(
                    new CodeFieldReferenceExpression(
                    new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), parent), name), "Clear");
            else
                clear = new CodeMethodInvokeExpression(
                    new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), name), "Clear");

            var args = new CodeExpression[list.Count];

            for (int a = 0; a < list.Count; a++)
                args[a] = new CodePrimitiveExpression(list[a]);

            CodeExpression expression;

            if (parent.Length > 0)
                expression = new CodeMethodInvokeExpression(
                    new CodeFieldReferenceExpression(
                    new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), parent), name), "AddRange",
                    new CodeArrayCreateExpression(typeof(string), args));
            else
                expression = new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), name), "AddRange",
                    new CodeArrayCreateExpression(typeof(string), args));

            CollectionStatements.Add(clear);
            CollectionStatements.Add(expression);
        }


        /// <summary>
        /// Adds a collection.
        /// </summary>
        /// <param name="parent">Parent name.</param>
        /// <param name="name">Name of a collection.</param>
        /// <param name="type">Collection type.</param>
        /// <param name="names">List of collection object names.</param>
        private void AddCollection(string parent, string name, Type type, List<string> names)
        {
            if (names != null && names.Count > 0)
            {
                var args = new CodeExpression[names.Count];

                for (int a = 0; a < names.Count; a++)
                {
                    args[a] = GetVariableReference(names[a]);
                }

                CodeExpression clear;
                if (parent.Length > 0)
                {
                    clear = new CodeMethodInvokeExpression(
                        new CodeFieldReferenceExpression(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), parent), name), "Clear");
                }
                else
                {
                    clear = new CodeMethodInvokeExpression(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), name), "Clear");
                }

                CodeExpression expression;

                if (parent.Length > 0)
                    expression = new CodeMethodInvokeExpression(
                        new CodeFieldReferenceExpression(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), parent), name), "AddRange",
                        new CodeArrayCreateExpression(GetTypeCollection(type), args));
                else
                    expression = new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), name), "AddRange",
                        new CodeArrayCreateExpression(GetTypeCollection(type), args));


                CollectionStatements.Add(new CodeCommentStatement(string.Empty, false));
                if (parent.Length > 0)
                {
                    CollectionStatements.Add(new CodeCommentStatement(string.Format("Add to {0}.{1}", parent, name), false));
                }
                else
                {
                    CollectionStatements.Add(new CodeCommentStatement("Add to " + name, false));
                }
                CollectionStatements.Add(new CodeCommentStatement(string.Empty, false));

                CollectionStatements.Add(clear);
                CollectionStatements.Add(expression);
            }
        }


        /// <summary>
        /// Adds an array.
        /// </summary>
        /// <param name="parent">Parent name.</param>
        /// <param name="prop">Description.</param>
        private void AddArray(string parent, StiPropertyInfo prop)
        {
            CodeExpression[] inits = new CodeExpression[prop.Count];
            int pos = 0;
            foreach (StiPropertyInfo property in prop.Properties)
                inits[pos++] = new CodePrimitiveExpression(property.Value);

            CodeArrayCreateExpression right =
                new CodeArrayCreateExpression(
                StiSerializing.GetTypeOfArrayElement(prop.Value), inits);

            CodeExpression left;

            if (parent.Length > 0)
                left = new CodeFieldReferenceExpression(
                    new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), parent), prop.Name);
            else left = new CodeFieldReferenceExpression(
                     new CodeThisReferenceExpression(), prop.Name);

            Statements.Add(new CodeAssignStatement(left, right));

        }

        /// <summary>
        /// Adds an array.
        /// </summary>
        private void AddArray(string componentName, string propertyName, Array array)
        {
            CodeExpression[] inits = new CodeExpression[array.GetLength(0)];


            for (int index = 0; index < array.GetLength(0); index++)
                inits[index] = new CodePrimitiveExpression(array.GetValue(index));

            CodeArrayCreateExpression right =
                new CodeArrayCreateExpression(
                StiSerializing.GetTypeOfArrayElement(array), inits);

            CodeExpression left;

            left = new CodeFieldReferenceExpression(
                new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(), componentName), propertyName);

            Statements.Add(new CodeAssignStatement(left, right));

        }


        /// <summary>
        /// Returns an expression for color.
        /// </summary>
        /// <param name="color">Color.</param>
        private CodeExpression GetColorExpression(Color color)
        {
            string colorName;
            if (color.IsSystemColor)
            {
                colorName = (new StiObjectStringConverter()).ObjectToString(color);
                return new CodePropertyReferenceExpression(
                    new CodeTypeReferenceExpression(typeof(SystemColors)), colorName);
            }

            if (color.IsNamedColor)
            {
                colorName = (new StiObjectStringConverter()).ObjectToString(color);
                return new CodePropertyReferenceExpression(
                    new CodeTypeReferenceExpression(typeof(Color)), colorName);
            }

            return new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(Color)), "FromArgb", new CodePrimitiveExpression(color.A), new CodePrimitiveExpression(color.R), new CodePrimitiveExpression(color.G), new CodePrimitiveExpression(color.B));

        }


        /// <summary>
        /// Returns an expression for an object.
        /// </summary>
        /// <param name="obj">Object.</param>
        internal CodeExpression GetPropertyExpression(object obj)
        {
            #region If this color
            if (obj is Color) return GetColorExpression((Color)obj);
            #endregion

            #region If object is value of the enumeration and he is component.
            if (obj is Enum && (!Enum.IsDefined(obj.GetType(), obj)))
            {
                Type enumType = obj.GetType();

                #region Form the collection of values being kept in array.
                var enumList = new List<int>();
                var enumValues = Enum.GetValues(enumType);

                foreach (int objEnum in enumValues)
                {
                    if (objEnum == 0)
                    {
                        if ((int)obj == objEnum)
                            enumList.Add(objEnum);
                    }
                    else
                    {
                        if ((objEnum & (int)obj) == objEnum)
                            enumList.Add(objEnum);
                    }
                }
                #endregion

                if (enumList.Count >= 2)
                {
                    var cdoe = new CodeBinaryOperatorExpression(
                        GetPropertyExpression(Enum.ToObject(enumType, enumList[0])),
                        CodeBinaryOperatorType.BitwiseOr,
                        GetPropertyExpression(Enum.ToObject(enumType, enumList[1])));

                    for (int index = 2; index < enumList.Count; index++)
                    {
                        var cdoe2 = new CodeBinaryOperatorExpression(
                            cdoe,
                            CodeBinaryOperatorType.BitwiseOr,
                            GetPropertyExpression(Enum.ToObject(enumType, enumList[index])));
                        cdoe = cdoe2;
                    }
                    return cdoe;


                }
            }
            string name = (new StiObjectStringConverter()).ObjectToString(obj);
            return new CodePropertyReferenceExpression(
                new CodeTypeReferenceExpression(obj.GetType()), name);
            #endregion
        }

        /// <summary>
		/// Returns an expression that creates an object.
		/// </summary>
		/// <param name="type">Object type.</param>
		/// <param name="value">Object value.</param>
		/// <returns>Expression.</returns>
        internal CodeExpression GetObjectCreateExpression(Type type, object value)
        {
            return GetObjectCreateExpressionWithParent(type, value, null);
        }

        /// <summary>
        /// Returns an expression that creates an object.
        /// </summary>
        /// <param name="type">Object type.</param>
        /// <param name="value">Object value.</param>
        /// <returns>Expression.</returns>
        internal CodeExpression GetObjectCreateExpressionWithParent(Type type, object value, object parentValue)
        {
            return GetObjectCreateExpressionWithParent(type, new CodeTypeReference(GetTypeString(type)), value, parentValue);
        }

        private CodeExpression GetDashboardDrillDownParametersListExpression(object value)
        {
            var list = ListExt.ToList(value).Cast<IStiDashboardDrillDownParameter>();
            if (list == null)
                return null;

            var parameterType = "Stimulsoft.Dashboard.Interactions.StiDashboardDrillDownParameter";

            var args = list.Select(v =>
            {
                return new CodeObjectCreateExpression(parameterType, new CodeExpression[]
                {
                    new CodePrimitiveExpression(v.Name),
                    new CodePrimitiveExpression(v.Expression)
                });

            }).ToArray();

            var createArrayExpression = new CodeArrayCreateExpression(parameterType, args);

            return new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(
                    new CodeTypeReferenceExpression(parameterType), "ToList"),
                    createArrayExpression);
        }

        private bool IsDashboardDrillDownParameterList(Type type)
        {
            if (type == null || !type.IsGenericType)
                return false;

            type = type.GenericTypeArguments.FirstOrDefault();
            if (type == null)
                return false;

            return type.GetInterfaces().Contains(typeof(IStiDashboardDrillDownParameter));
        }

        private bool IsLocation(Type type)
        {
            if (type == typeof(Size)) return true;
            if (type == typeof(SizeF)) return true;
            if (type == typeof(SizeD)) return true;
            if (type == typeof(StiMargins)) return true;
            if (type == typeof(Point)) return true;
            if (type == typeof(PointF)) return true;
            if (type == typeof(PointD)) return true;
            if (type == typeof(Rectangle)) return true;
            if (type == typeof(RectangleF)) return true;
            if (type == typeof(RectangleD)) return true;

            return false;
        }

        /// <summary>
		/// Returns arguments of an object constructor.
		/// </summary>
		/// <param name="type">Object type.</param>
		/// <param name="value">Object value.</param>
		/// <returns>Aguments array.</returns>
        internal CodeExpression[] GetArguments(Type type, object value)
        {
            return GetArgumentsWithParent(type, value, null);
        }


        /// <summary>
        /// Returns arguments of an object constructor.
        /// </summary>
        /// <param name="type">Object type.</param>
        /// <param name="value">Object value.</param>
        /// <returns>Aguments array.</returns>
        internal CodeExpression[] GetArgumentsWithParent(Type type, object value, object parentValue)
        {
            var instanceDescriptor = (InstanceDescriptor)TypeDescriptor.GetConverter(type).ConvertTo(value, typeof(InstanceDescriptor));

            var args = new CodeExpression[instanceDescriptor.Arguments.Count];

            object[] objs = new Object[instanceDescriptor.Arguments.Count];
            instanceDescriptor.Arguments.CopyTo(objs, 0);
            for (int index = 0; index < objs.Length; index++)
            {
                object obj = objs[index];
                if (obj != null)
                {
                    Type objType = obj.GetType();

                    if (type == typeof(StiDialogInfo) && obj is string[])
                    {
                        var strs = obj as string[];
                        var inits = new CodeExpression[strs.Length];
                        var index2 = 0;

                        foreach (string str in strs)
                        {
                            CodeExpression expr = null;
                            if (str != null && str.StartsWith("{") && str.EndsWith("}"))
                            {
                                var value2 = str.Substring(1, str.Length - 2);
                                if (value2.Contains("<<|>>"))
                                {
                                    var variable = parentValue as StiVariable;
                                    var currentCulture = Thread.CurrentThread.CurrentCulture;
                                    try
                                    {
                                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                                        string[] values = value2.Split(new[] { "<<|>>" }, StringSplitOptions.None);

                                        expr = new CodeMethodInvokeExpression(
                                            new CodeMethodReferenceExpression(
                                            new CodeTypeReferenceExpression(typeof(StiDialogInfo)), "Convert"),
                                            new CodeObjectCreateExpression(variable.Type, new CodeSnippetExpression(values[0]), new CodeSnippetExpression(values[1])));
                                    }
                                    finally
                                    {
                                        Thread.CurrentThread.CurrentCulture = currentCulture;
                                    }
                                }
                                else
                                {
                                    expr = new CodeMethodInvokeExpression(
                                        new CodeMethodReferenceExpression(
                                        new CodeTypeReferenceExpression(typeof(StiDialogInfo)), "Convert"),
                                        new CodeSnippetExpression(value2));
                                }
                            }
                            else
                            {
                                expr = new CodePrimitiveExpression(str);
                            }
                            inits[index2] = expr;
                            index2++;
                        }

                        args[index] = new CodeArrayCreateExpression(typeof(string[]), inits);
                    }

                    else if (type == typeof(StiDialogInfo) && obj is bool[])
                    {
                        var bools = obj as bool[];
                        var inits = bools.Select(b => new CodePrimitiveExpression(b)).Cast< CodeExpression>().ToArray();

                        args[index] = new CodeArrayCreateExpression(typeof(bool[]), inits);
                    }

                    else if (obj is Type) 
                        args[index] = new CodeTypeOfExpression(GetTypeString(obj as Type));
                    
                    else if (obj is Image) 
                        args[index] = GetImageExpression(obj);
                    
                    else if (obj is byte[]) 
                        args[index] = GetByteArrayExpression(obj as byte[]);
                    
                    else if (obj is Color[]) 
                        args[index] = GetColorArrayExpression(obj as Color[]);
                    
                    else if (obj is string[]) 
                        args[index] = GetStringArrayExpression(obj as string[]);
                    
                    else if (IsDashboardDrillDownParameterList(objType)) 
                        args[index] = GetDashboardDrillDownParametersListExpression(obj);
                    
                    else if (IsLocation(objType)) 
                        args[index] = GetObjectCreateExpressionWithParent(objType, obj, value);
                    
                    else if (!IsSimple(objType)) 
                        args[index] = GetPropertyExpression(obj);
                    
                    else if (objType.IsClass) 
                        args[index] = GetObjectCreateExpressionWithParent(objType, obj, value);

                    else 
                        args[index] = new CodePrimitiveExpression(obj);
                }
                else args[index] = new CodePrimitiveExpression(null);

            }
            return args;
        }

        private CodeExpression GetImageExpression(object imageObj)
        {
            Image image = imageObj as Image;

            if (image is Metafile)
            {
                string imageStr = StiMetafileConverter.MetafileToString(image as Metafile);

                return new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression("StiMetafileConverter"), "StringToMetafile", new CodePrimitiveExpression(imageStr));
            }
            else
            {
                string imageStr = StiImageConverter.ImageToString(image);

                return new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression("StiImageConverter"), "StringToImage", new CodePrimitiveExpression(imageStr));
            }
        }

        private CodeExpression GetByteArrayExpression(byte[] bytes)
        {
            var str = StiPacker.PackToString(bytes);

            return new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(StiPacker)), "UnpackFromString",
                new CodePrimitiveExpression(str));
        }

        private CodeExpression GetColorArrayExpression(Color[] colors)
        {
            return new CodeArrayCreateExpression(
                new CodeTypeReference(typeof(Color)),
                colors.Select(GetColorExpression).ToArray());
        }

        private CodeExpression GetStringArrayExpression(string[] strs)
        {
            return new CodeArrayCreateExpression(
                new CodeTypeReference(typeof(string)),
                strs.Select(s => new CodePrimitiveExpression(s)).ToArray());
        }

        internal CodeObjectCreateExpression GetCodePrimitiveForPointF(CodeTypeReference createType, object value)
        {
            var args = new CodeExpression[2];

            var point = (PointF)value;
            args[0] = new CodePrimitiveExpression(point.X);
            args[1] = new CodePrimitiveExpression(point.Y);

            return new CodeObjectCreateExpression(createType, args);
        }

        internal CodeMethodInvokeExpression GetCodeExpressionForFont(CodeTypeReference createType, object value)
        {
            var font = value as Font;

            var args = new CodeExpression[3];
            args[0] = new CodePrimitiveExpression(font.Name);
            args[1] = new CodePrimitiveExpression(font.Size);
            args[2] = GetPropertyExpression(font.Style);

            return new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression("Stimulsoft.Base.StiFontCollection"), "CreateFont", args);
        }

        /// <summary>
        /// Returns an expression that creates a type.
        /// </summary>
        /// <param name="type">Type being create.</param>
        /// <param name="value">Value.</param>
        /// <returns>Expression.</returns>
        private CodeExpression GetObjectCreateExpressionWithParent(Type type, CodeTypeReference createType, object value, object parentValue)
        {
            if (type == typeof(string)) 
                return new CodePrimitiveExpression(value);

            if (type == typeof(PointF)) 
                return GetCodePrimitiveForPointF(createType, value);

            if (type == typeof(Font))
            {
                if (StiFontCollection.IsCustomFont((value as Font).Name))
                    return GetCodeExpressionForFont(createType, value);
            }

            if (TypeDescriptor.GetConverter(type).CanConvertTo(typeof(InstanceDescriptor)))
            {
                var args = GetArgumentsWithParent(type, value, parentValue);

                return new CodeObjectCreateExpression(createType, args);
            }

            return new CodeObjectCreateExpression(type);
        }



        /// <summary>
        /// Replaces impossible symbols in the string on the symbol '_'.
        /// </summary>
        /// <param name="str">String to change.</param>
        /// <returns>Changed string.</returns>
        public static string ReplaceSymbols(string str)
        {
            return ReplaceSymbols(str, true, null);
        }

        public static string ReplaceSymbols(string str, StiReport report)
        {
            return ReplaceSymbols(str, true, report);
        }

        /// <summary>
        /// Replaces impossible symbols in the string on the symbol '_'.
        /// </summary>
        /// <param name="str">String to change.</param>
        /// <returns>Changed string.</returns>
        public static string ReplaceSymbols(string str, bool checkKeywords, StiReport report)
        {
            return StiNameValidator.CorrectName(str, checkKeywords, report);
        }

        public static string ConvertTypeToString(Type type, StiReportLanguageType language)
        {
            if (language == StiReportLanguageType.CSharp || language == StiReportLanguageType.JS)
            {
                if (type == typeof(int)) return "int";
                if (type == typeof(uint)) return "uint";
                if (type == typeof(long)) return "long";
                if (type == typeof(ulong)) return "ulong";
                if (type == typeof(string)) return "string";
                if (type == typeof(bool)) return "bool";
                if (type == typeof(object)) return "object";
                if (type == typeof(void)) return "void";
                if (type == typeof(byte)) return "byte";
                if (type == typeof(sbyte)) return "sbyte";
                if (type == typeof(short)) return "short";
                if (type == typeof(ushort)) return "ushort";
                if (type == typeof(char)) return "char";
                if (type == typeof(double)) return "double";
                if (type == typeof(float)) return "float";
                if (type == typeof(decimal)) return "decimal";
                if (type == typeof(DateTime)) return "DateTime";
                if (type == typeof(TimeSpan)) return "TimeSpan";

                if (type == typeof(byte?)) return "byte?";
                if (type == typeof(sbyte?)) return "sbyte?";
                if (type == typeof(bool?)) return "bool?";
                if (type == typeof(byte?)) return "char?";
                if (type == typeof(short?)) return "short?";
                if (type == typeof(ushort?)) return "ushort?";
                if (type == typeof(int?)) return "int?";
                if (type == typeof(uint?)) return "uint?";
                if (type == typeof(long?)) return "long?";
                if (type == typeof(ulong?)) return "ulong?";
                if (type == typeof(double?)) return "double?";
                if (type == typeof(float?)) return "float?";
                if (type == typeof(decimal?)) return "decimal?";
                if (type == typeof(DateTime?)) return "DateTime?";
                if (type == typeof(TimeSpan?)) return "TimeSpan?";
                if (type == typeof(Guid?)) return "Guid?";
                if (type == null) return null;

                if (Nullable.GetUnderlyingType(type) != null)
                    return string.Format("{0}?", Nullable.GetUnderlyingType(type));

                if (type.IsGenericType && !StiOptions.Engine.AllowUseGenericTypesInCodeDom) return "Object";
            }
            else
            {
                if (type == typeof(int)) return "Integer";
                if (type == typeof(long)) return "Long";
                if (type == typeof(string)) return "String";
                if (type == typeof(bool)) return "Boolean";
                if (type == typeof(object)) return "Object";
                if (type == typeof(void)) return "Void";
                if (type == typeof(byte)) return "Byte";
                if (type == typeof(short)) return "Short";
                if (type == typeof(char)) return "Char";
                if (type == typeof(double)) return "Double";
                if (type == typeof(float)) return "Single";
                if (type == typeof(decimal)) return "Decimal";
                if (type == typeof(DateTime)) return "DateTime";

                if (type == typeof(byte?)) return "Nullable(Of Byte)";
                if (type == typeof(bool?)) return "Nullable(Of Boolean)";
                if (type == typeof(byte?)) return "Nullable(Of Char)";
                if (type == typeof(short?)) return "Nullable(Of Short)";
                if (type == typeof(int?)) return "Nullable(Of Integer)";
                if (type == typeof(long?)) return "Nullable(Of Long)";
                if (type == typeof(double?)) return "Nullable(Of Double)";
                if (type == typeof(float?)) return "Nullable(Of Single)";
                if (type == typeof(decimal?)) return "Nullable(Of Decimal)";
                if (type == typeof(DateTime?)) return "Nullable(Of DateTime)";
                if (type == typeof(TimeSpan?)) return "Nullable(Of TimeSpan)";
                if (type == typeof(Guid?)) return "Nullable(Of Guid)";

                if (Nullable.GetUnderlyingType(type) != null)
                    return string.Format("Nullable(Of {0})", Nullable.GetUnderlyingType(type));

                if (type.IsGenericType && !StiOptions.Engine.AllowUseGenericTypesInCodeDom) 
                    return "Object";
            }

            return null;
        }

        internal string GetTypeString(Type type)
        {
            if (type == null)
                return "Object";

            string typeString = ConvertTypeToString(type, report.ScriptLanguage);
            if (typeString == null) 
                typeString = type.ToString();

            return typeString;
        }

        internal CodeMemberProperty GetColumnProperty(StiDataColumn column, bool buildRelations)
        {
            var prop = new CodeMemberProperty
            {
                Attributes = MemberAttributes.Public,
                HasGet = true,
                Name = ReplaceSymbols(column.Name, true, column.DataSource?.Dictionary?.Report),
                Type = new CodeTypeReference(GetTypeString(column.Type))
            };

            CodeExpression cast;
            
            if (column is StiCalcDataColumn)
            {
                string methodName;
                
                if (column.DataSource == null)                
                    methodName = $"Get{ReplaceSymbols(column.BusinessObject.Name, report)}_{ReplaceSymbols(column.Name, report)}";
                
                else                
                    methodName = $"Get{ReplaceSymbols(column.DataSource.Name, report)}_{ReplaceSymbols(column.Name, report)}";                

                if (!buildRelations)
                {
                    #region Create Return Method
                    var method = new CodeMemberMethod
                    {
                        Attributes = MemberAttributes.Public | MemberAttributes.Final,
                        Name = methodName,
                        ReturnType = new CodeTypeReference(GetTypeString(column.Type))
                    };

                    var expression = ((StiCalcDataColumn)column).Value;
                    expression = StiCodeDomFunctions.ParseFunctions(this, expression);
                    expression = StiCodeDomTotalsFunctionsParser.ProcessTotals(this, expression, column.Name);

                    method.Statements.Add(new CodeMethodReturnStatement(
                        new CodeSnippetExpression(expression)));

                    Members.Add(method);
                    #endregion
                }
                
                cast = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(
                    new CodeCastExpression(new CodeTypeReference(thisReportTypeName),
                    new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(
                    new CodeThisReferenceExpression(), "Dictionary"), "Report")), methodName));
            }
            else
            {
                CodeIndexerExpression ind =
                    new CodeIndexerExpression(
                    new CodeThisReferenceExpression(), new CodePrimitiveExpression(column.NameInSource));

                if (column.Type == typeof(object))
                {
                    cast = ind;
                }
                else
                {
                    cast = new CodeCastExpression(GetTypeString(column.Type), new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression("StiReport"), 
                        "ChangeType", ind, new CodeTypeOfExpression(GetTypeString(column.Type)), new CodePrimitiveExpression(report.ConvertNulls)));
                }
            }

            CodeMethodReturnStatement ret = new CodeMethodReturnStatement(cast);

            prop.GetStatements.Add(ret);
            return prop;
        }

        /// <summary>
        /// Returns databand on which located comp. Returns nothing if component located not on databand.
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        private StiDataBand GetDataBandOnWhichPlacedComponent(StiComponent comp)
        {
            StiComponent parent = comp.Parent;
            if (parent is StiChildBand)
            {
                StiBand tempBand = (parent as StiChildBand).GetMaster();
                if (tempBand != null) parent = tempBand;
            }
            while (!(parent == null || parent is StiDataBand || parent is StiPage))
            {
                parent = parent.Parent;
                if (parent is StiChildBand)
                {
                    StiBand tempBand = (parent as StiChildBand).GetMaster();
                    if (tempBand != null) parent = tempBand;
                }
            }
            return parent as StiDataBand;
        }

        /// <summary>
        /// Connect the delayed collection of aggregate functions to the events on the begin and the end.
        /// </summary>
        public void ConnectAggregatesBeginEnd()
        {
            if (compsRemittedSE.Count > 0)
            {
                var statsStore = new List<StiEventItem>();
                for (int index = 0; index < compsRemittedSE.Count; index += 2)
                {
                    string compName = GetCompNameAggregateBeginEnd((string)compsRemittedSE[index]);
                    string runningFooterBandName = null;
                    if (compName.IndexOf(":", StringComparison.InvariantCulture) != -1)
                    {
                        string[] strs = compName.Split(':');
                        compName = strs[0];
                    }
                    var al = (ArrayList)compsRemittedSE[index + 1];

                    bool usePrintEvent = false;
                    bool useColumnEvent = false;

                    #region BeginRender
                    CodeStatementCollection stats = new CodeStatementCollection();
                    CodeStatementCollection stats2 = new CodeStatementCollection();

                    foreach (StiRemit remit in al)
                    {
                        StiComponent comp = remit.Component;
                        if (runningFooterBandName == null)
                            runningFooterBandName = remit.RunningFooterBandName;

                        if (remit.UsePrintEvent) usePrintEvent = true;
                        if (remit.UseColumn)
                        {
                            useColumnEvent = true;

                            compName = ReplaceSymbols(comp.Page.Name, report);
                        }

                        string methodName = remit.Name;

                        if (remit.Name.Contains("_SumRunning"))
                        {
                            CodeAssignStatement cas2 = new CodeAssignStatement(
                                   new CodeFieldReferenceExpression(
                                   new CodeFieldReferenceExpression(
                                   new CodeThisReferenceExpression(), methodName), "IsFirstInit"),
                                   new CodePrimitiveExpression(true));

                            //stats.Add(cas2);
                            if (remit.ConnectOnlyBegin) stats2.Add(cas2);
                            else stats.Add(cas2);
                        }

                        CodeMethodInvokeExpression mi =
                            new CodeMethodInvokeExpression(
                            new CodeFieldReferenceExpression(
                            new CodeThisReferenceExpression(), methodName), "Init");

                        //stats.Add(mi);
                        if (remit.ConnectOnlyBegin) stats2.Add(mi);
                        else stats.Add(mi);

                        CodeAssignStatement cas = null;

                        if (remit.Expression is StiExcelValueExpression)
                        {
                            cas = new CodeAssignStatement(
                                new CodeFieldReferenceExpression(
                                new CodeFieldReferenceExpression(
                                new CodeThisReferenceExpression(), ReplaceSymbols(comp.Name, report)), "ExcelDataValue"),
                                new CodePrimitiveExpression("-"));
                        }
                        else
                        {
                            cas = new CodeAssignStatement(
                                new CodeFieldReferenceExpression(
                                new CodeFieldReferenceExpression(
                                new CodeThisReferenceExpression(), ReplaceSymbols(comp.Name, report)), "TextValue"),
                                new CodePrimitiveExpression(string.Empty));
                        }
                        //stats.Add(cas);
                        if (remit.ConnectOnlyBegin) stats2.Add(cas);
                        else stats.Add(cas);

                        //if (remit.Name.Contains("_SumRunning"))
                        //{
                        //    CodeAssignStatement cas2 = new CodeAssignStatement(
                        //           new CodeFieldReferenceExpression(
                        //           new CodeFieldReferenceExpression(
                        //           new CodeThisReferenceExpression(), methodName), "IsFirstInit"),
                        //           new CodePrimitiveExpression(true));

                        //    stats2.Add(cas2);
                        //    stats2.Add(mi);
                        //    stats2.Add(cas);
                        //}
                    }

                    string eventName;
                    StiEvent ev;
                    if (useColumnEvent)
                    {
                        eventName = compName + "__ColumnBeginRender";
                        ev = new StiColumnBeginRenderEvent();
                    }
                    else
                    {
                        if (usePrintEvent)
                        {
                            eventName = compName + "__BeforePrint";
                            ev = new StiBeforePrintEvent();
                        }
                        else
                        {
                            eventName = compName + "__BeginRender";
                            ev = new StiBeginRenderEvent();
                        }
                    }
                    if (stats.Count > 0)
                    {
                        GenAddEvent(eventName, compName, ev);
                        GenEventMethod(eventName, ev, stats);
                    }

                    if (stats2.Count > 0)
                    {
                        string eventName2 = compName + "__BeforePrint";
                        if (eventName != eventName2)
                        {
                            StiEvent ev2 = new StiBeforePrintEvent();
                            GenAddEvent(eventName2, compName, ev2);
                            GenEventMethod(eventName2, ev2, stats2);
                        }
                        //else
                        //{
                        //    GenEventMethod(eventName, ev, stats2);
                        //}
                    }
                    #endregion

                    #region EndRender
                    //usePrintEvent = false;
                    //useColumnEvent = false;
                    //StiDataBand useNameFromDataBand = null;
                    //stats = new CodeStatementCollection();

                    #region Process StiRemit Collection
                    Hashtable hashCodesOfRemit = new Hashtable();
                    foreach (StiRemit remit in al)
                    {
                        if (remit.ConnectOnlyBegin) continue;

                        // !!! fix
                        usePrintEvent = false;
                        useColumnEvent = false;
                        StiDataBand useNameFromDataBand = null;
                        stats = new CodeStatementCollection();
                        // !!!

                        if (!ContainsRemittedComponentInLast(remit.Component))
                        {
                            string eventNameEnd = remit.Component.Name + "_GetValue_End";

                            if (remit.Expression is StiExcelValueExpression)
                                eventNameEnd = remit.Component.Name + "_GetExcelValue_End";

                            if (remit.UsePrintEvent) usePrintEvent = true;
                            if (remit.UseColumn)
                            {
                                useColumnEvent = true;

                                compName = remit.Component.Page.Name;
                            }
                            /*If component placed on databand and this is running total then we need use printEvent instead endrender.*/
                            useNameFromDataBand = GetDataBandOnWhichPlacedComponent(remit.Component);
                            if (remit.isRunningTotal && (useNameFromDataBand != null) && !useColumnEvent)
                            {
                                usePrintEvent = true;
                            }
                            else useNameFromDataBand = null;

                            CodeMethodInvokeExpression setMethod = null;

                            string methodName = remit.Expression is StiExcelValueExpression ? "SetExcelText" : "SetText";

                            string codeOfRemit = remit.Component.Name + ":" + methodName + ":" + eventNameEnd;

                            //This checking block from generating duplicated line of code
                            if (hashCodesOfRemit[codeOfRemit] == null)
                            {
                                hashCodesOfRemit[codeOfRemit] = codeOfRemit;

                                if (report.ScriptLanguage == StiReportLanguageType.CSharp || report.ScriptLanguage == StiReportLanguageType.JS)
                                {
                                    Type typeDelegate = typeof(StiGetValue);
                                    if (remit.Expression is StiExcelValueExpression) typeDelegate = typeof(StiGetExcelValue);

                                    if (remit.isRunningTotal)
                                    {
                                        setMethod = new CodeMethodInvokeExpression(
                                            new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), remit.Component.Name),
                                            methodName,
                                            new CodeDelegateCreateExpression(new CodeTypeReference(typeDelegate), new CodeThisReferenceExpression(), eventNameEnd),
                                            new CodePrimitiveExpression(true));
                                    }
                                    else
                                    {
                                        setMethod = new CodeMethodInvokeExpression(
                                            new CodeFieldReferenceExpression(
                                            new CodeThisReferenceExpression(), remit.Component.Name), methodName, new CodeDelegateCreateExpression(
                                                new CodeTypeReference(typeDelegate),
                                                new CodeThisReferenceExpression(), eventNameEnd
                                            ));
                                    }
                                }
                                else
                                {
                                    if (remit.isRunningTotal)
                                    {
                                        setMethod = new CodeMethodInvokeExpression(
                                        new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), remit.Component.Name),
                                        methodName,
                                        new CodeSnippetExpression("AddressOf " + eventNameEnd),
                                        new CodePrimitiveExpression(true));
                                    }
                                    else
                                    {
                                        setMethod = new CodeMethodInvokeExpression(
                                        new CodeFieldReferenceExpression(
                                        new CodeThisReferenceExpression(), remit.Component.Name), methodName, new CodeSnippetExpression("AddressOf " + eventNameEnd));
                                    }
                                }
                                stats.Add(setMethod);
                            }

                            //    }
                            //}
                            //#endregion

                            //if (stats.Count > 0)
                            //{
                            string name = compName;
                            #region If component located on databand and this is running total then use name of databand
                            if (useNameFromDataBand != null)
                            {
                                name = ReplaceSymbols(useNameFromDataBand.Name, report);
                            }
                            #endregion

                            if (useColumnEvent)
                            {
                                eventName = name + "__ColumnEndRender";
                                ev = new StiColumnEndRenderEvent();
                            }
                            else
                            {
                                if (usePrintEvent)
                                {
                                    eventName = name + "__AfterPrint";
                                    ev = new StiAfterPrintEvent();
                                }
                                else
                                {
                                    eventName = name + "__EndRender";
                                    ev = new StiEndRenderEvent();
                                }
                            }
                            if (runningFooterBandName != null)
                            {
                                eventName = runningFooterBandName + "_MoveFooterToBottom";
                                name = runningFooterBandName;
                                ev = new StiMoveFooterToBottomEvent();
                                //GenAddEvent(eventName, name, ev);
                                //GenEventMethod(eventName, ev, stats);
                            }
                            else
                            {
                                //GenAddEvent(eventName, name, ev);
                                //GenEventMethod(eventName, ev, stats);
                            }
                            statsStore.Add(new StiEventItem(eventName, name, ev, codeOfRemit, stats));
                        }
                    }
                    #endregion

                    #endregion
                }

                #region Check for duplicates
                if (statsStore.Count > 0)
                {
                    //check for duplicates of codeOfRemit
                    for (int index1 = statsStore.Count - 1; index1 > 0; index1--)
                    {
                        var evit1 = statsStore[index1];
                        for (int index2 = 0; index2 < index1; index2++)
                        {
                            var evit2 = statsStore[index2];
                            if (evit1.EventName == evit2.EventName && evit1.Name == evit2.Name && evit1.CodeOfRemit == evit2.CodeOfRemit)
                            {
                                statsStore.RemoveAt(index1);
                                break;
                            }
                        }
                    }

                    //check for duplicates of eventName
                    for (int index1 = statsStore.Count - 1; index1 > 0; index1--)
                    {
                        var evit1 = statsStore[index1];
                        for (int index2 = 0; index2 < index1; index2++)
                        {
                            var evit2 = statsStore[index2];
                            if (evit1.EventName == evit2.EventName && evit1.Name == evit2.Name)
                            {
                                evit2.Stats.AddRange(evit1.Stats);
                                statsStore.RemoveAt(index1);
                                break;
                            }
                        }
                    }

                    foreach (var evit in statsStore)
                    {
                        GenAddEvent(evit.EventName, evit.Name, evit.Event);
                        GenEventMethod(evit.EventName, evit.Event, evit.Stats);
                    }
                }
                #endregion
            }
        }

        private class StiEventItem
        {
            public string EventName;
            public string Name;
            public StiEvent Event;
            public string CodeOfRemit;
            public CodeStatementCollection Stats;

            public override string ToString()
            {
                return EventName;
            }

            public StiEventItem(string eventName, string name, StiEvent eventt, string codeOfRemit, CodeStatementCollection stats)
            {
                this.EventName = eventName;
                this.Name = name;
                this.Event = eventt;
                this.CodeOfRemit = codeOfRemit;
                this.Stats = stats;
            }
        }

        private string GetCompNameAggregateBuild(string compName)
        {
            if (compName.IndexOf(":", StringComparison.InvariantCulture) != -1)
            {
                string[] strs = compName.Split(':');
                compName = strs[1];
            }
            return ReplaceSymbols(compName, report);
        }


        private string GetCompNameAggregateBeginEnd(string compName)
        {
            if (compName.IndexOf(":", StringComparison.InvariantCulture) != -1)
            {
                string[] strs = compName.Split(':');
                compName = strs[0];
            }
            return ReplaceSymbols(compName, report);
        }


        /// <summary>
        /// Connect the delayed collection of aggregate functions to the end for building.
        /// </summary>
        public void ConnectAggreagatesBuild()
        {
            if (compsRemittedBuild.Count > 0)
            {
                for (int index = 0; index < compsRemittedBuild.Count; index += 2)
                {
                    var compName = GetCompNameAggregateBuild((string)compsRemittedBuild[index]);
                    var al = (ArrayList)compsRemittedBuild[index + 1];

                    #region Build
                    var stats = new CodeStatementCollection();
                    foreach (StiRemit remit in al)
                    {
                        StiComponent comp = remit.Component;
                        if (remit.UseColumn) 
                            compName = ReplaceSymbols(comp.GetDataBand().Name, report);

                        var methodName = remit.Name;
                        var isSumDistinct2 = (remit.ScriptCondition.Length > 0) && (remit.Name.StartsWith($"{remit.Component.Name}_SumDistinct"));

                        stats.Add(new CodeCommentStatement(GetCheckerInfoString(remit.Component.Name, "Text")));

                        #region method CalcItem
                        CodeMethodInvokeExpression mi = null;
                        CodeStatement st = null;

                        if (remit.Function.RecureParam && (remit.ScriptCalcItem != null && remit.ScriptCalcItem.Trim().Length > 0))
                        {
                            if (isSumDistinct2)
                            {
                                mi = new CodeMethodInvokeExpression(
                                    new CodeFieldReferenceExpression(
                                    new CodeThisReferenceExpression(), methodName), "CalcItem", new CodeSnippetExpression(remit.ScriptCalcItem), new CodeSnippetExpression(remit.ScriptCondition));
                            }
                            else
                            {
                                mi = new CodeMethodInvokeExpression(
                                    new CodeFieldReferenceExpression(
                                    new CodeThisReferenceExpression(), methodName), "CalcItem", new CodeSnippetExpression(remit.ScriptCalcItem));
                            }
                        }
                        else mi = new CodeMethodInvokeExpression(
                                 new CodeFieldReferenceExpression(
                                 new CodeThisReferenceExpression(), methodName), "CalcItem", new CodePrimitiveExpression(null));
                        #endregion

                        if ((remit.ScriptCondition.Length > 0) && (!isSumDistinct2))
                        {
                            st = new CodeConditionStatement(
                                new CodeSnippetExpression(remit.ScriptCondition),
                                new CodeStatement[]
                                {
                                    new CodeExpressionStatement(mi)
                                },
                                new CodeStatement[0]);
                        }

                        #region Generate try..catch
                        if (StiOptions.Engine.ForceAggregateFunctionsExceptionProcessing)
                        {
                            var try1 = new CodeTryCatchFinallyStatement();
                            if (st != null)
                                try1.TryStatements.Add(st);
                            else
                                try1.TryStatements.Add(mi);

                            CodeCatchClause catch2 = new CodeCatchClause("ex");

                            catch2.Statements.Add(new CodeMethodInvokeExpression(
                                new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("StiLogService"), "Write"), new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "GetType")), new CodePrimitiveExpression(compName + " RenderingEvent " + methodName + " ...ERROR")));

                            catch2.Statements.Add(new CodeMethodInvokeExpression(
                                new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("StiLogService"), "Write"), new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "GetType")), new CodeArgumentReferenceExpression("ex")));

                            catch2.Statements.Add(new CodeMethodInvokeExpression(
                                new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "WriteToReportRenderingMessages"), new CodeSnippetExpression(string.Format("\"{0} \" + ex.Message", methodName))));

                            try1.CatchClauses.Add(catch2);
                            st = try1;
                        }
                        #endregion

                        if (st != null)
                            stats.Add(st);
                        else
                            stats.Add(mi);
                    }

                    var eventName = $"{compName}__Rendering";
                    var be = new StiRenderingEvent();

                    GenAddEvent(eventName, compName, be);
                    GenEventMethod(eventName, be, stats);
                    #endregion
                }
            }
        }


        /// <summary>
        /// Connect the delayed collection of aggregate functions to the end of generation.
        /// </summary>
        public void ConnectTextBoxReportEnd(List<StiRemit> comps)
        {
            if (comps.Count > 0)
            {
                #region Report EndRender
                var stats = new CodeStatementCollection();
                foreach (StiRemit remit in comps)
                {
                    var name = ReplaceSymbols(remit.Component.Name, report);
                    var eventNameEnd = $"{name}_GetValue_End";
                    var methodName = remit.Expression is StiExcelValueExpression ? "SetExcelText" : "SetText";

                    CodeMethodInvokeExpression setMethod;
                    if (report.ScriptLanguage == StiReportLanguageType.CSharp || report.ScriptLanguage == StiReportLanguageType.JS)
                    {
                        var typeDelegate = typeof(StiGetValue);
                        
                        if (remit.Expression is StiExcelValueExpression) 
                            typeDelegate = typeof(StiGetExcelValue);

                        setMethod = new CodeMethodInvokeExpression(
                            new CodeFieldReferenceExpression(
                            new CodeThisReferenceExpression(), name), methodName, new CodeDelegateCreateExpression(
                                new CodeTypeReference(typeDelegate),
                                new CodeThisReferenceExpression(), eventNameEnd
                            ));
                    }
                    else
                    {
                        setMethod = new CodeMethodInvokeExpression(
                            new CodeFieldReferenceExpression(
                            new CodeThisReferenceExpression(), name), methodName, new CodeSnippetExpression("AddressOf " + eventNameEnd));
                    }

                    stats.Add(setMethod);
                }

                string eventName;
                if (comps == compsProcessAtEndRemitted) 
                    eventName = "ProcessAtEnd__EndRender";
                else 
                    eventName = $"{report.GetReportName()}WordsToEnd__EndRender";

                var ebr = new StiEndRenderEvent();
                GenAddEvent(eventName, null, ebr);
                GenEventMethod(eventName, ebr, stats);
                #endregion
            }
        }


        public void ConnectChartProcessAtEnd()
        {
            var stats = new CodeStatementCollection();
            var comps2 = components;

            foreach (StiComponent comp in comps2)
            {
                var chartComp = comp as IStiChartComponent;
                var processAtEnd = comp as IStiProcessAtEnd;

                if (chartComp != null && processAtEnd != null && processAtEnd.ProcessAtEnd)
                {
                    var setMethod = new CodeMethodInvokeExpression(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), comp.Name), "RenderAtEnd");

                    stats.Add(setMethod);
                }
            }

            if (stats.Count > 0)
            {
                var eventName = "ChartProcessAtEnd__EndRender";
                var ebr = new StiEndRenderEvent();
                GenAddEvent(eventName, null, ebr);
                GenEventMethod(eventName, ebr, stats);
            }
        }

        /// <summary>
        /// Create the array of all aggregate functions.
        /// </summary>
        public void BuildArrayAggregates()
        {
            if (compsRemittedBuild.Count > 0)
            {
                var inits = new List<CodeExpression>();

                #region Create collection an aggregates functions
                for (int index = 0; index < compsRemittedBuild.Count; index += 2)
                {
                    string compName = (string)compsRemittedBuild[index];
                    var al = (ArrayList)compsRemittedBuild[index + 1];
                    foreach (StiRemit remit in al)
                    {
                        inits.Add(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), remit.Name));
                    }
                }
                #endregion

                #region Create code
                var right = new CodeArrayCreateExpression(typeof(object), inits.ToArray());

                var left = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "AggregateFunctions");

                Statements.Add(new CodeAssignStatement(left, right));
                #endregion
            }
        }



        internal CodeExpression GetArrayCreateExpression(object obj)
        {
            var array = obj as Array;
            var list = new CodeExpression[array.Length];

            for (int index = 0; index < array.Length; index++)
            {
                list[index] = new CodePrimitiveExpression(array.GetValue(index));
            }

            return new CodeArrayCreateExpression(obj.GetType(), list);
        }


        internal void Assign(CodeMemberMethod memberMethod, string fieldName1, string fieldName2, object obj)
        {
            memberMethod.Statements.Add(
                new CodeAssignStatement(
                new CodeFieldReferenceExpression(
                new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(), fieldName1), fieldName2),
                GetArrayCreateExpression(obj)));
        }



        /// <summary>
        /// Serializes a list of properties.
        /// </summary>
        private List<string> SerializeProps(StiPropertyInfoCollection props, bool isList, string parentName, string referenceName)
        {
            var names = new List<string>();

            #region First pass - generation of simple values
            foreach (StiPropertyInfo prop in props)
            {
                var propName = ReplaceSymbols(prop.Name, report);

                if (prop.Value is StiUserViewState && prop?.Parent?.Parent != null)
                    prop.Name = $"{prop.Parent.Parent.Name}_{propName}";

                if (prop.Value is StiBaseStyle && (prop.Parent == null || !(prop.Parent.Value is StiChart)) && (prop.Parent == null || !(prop.Parent.Value is StiGauge))
                     && (prop.Parent == null || !(prop.Parent.Value is Stimulsoft.Report.Components.Table.StiTable)))
                    prop.Name = $"Style{propName}";

                if (!prop.IsReference && prop.ReferenceCode != -1)
                {
                    if (Graphs[propName] == -1)
                    {
                        Graphs.Add(propName, prop.ReferenceCode);
                    }
                    else
                    {
                        if (prop.Name != "TextFormat")
                        {
                            var propNameNew = $"{propName}_copy_{prop.ReferenceCode}";
                            while (Graphs[propNameNew] != -1)
                                propNameNew += "_";

                            Graphs.Add(propNameNew, prop.ReferenceCode);
                        }
                    }
                }

                if (!prop.IsDefaultValueSpecified || !object.Equals(prop.DefaultValue, prop.Value))
                {
                    #region Generate simple types and strings
                    if (prop.Type.IsValueType || prop.Type == typeof(string))
                    {
                        if (
                            prop.Value is DateTime ||
                            prop.Value is Rectangle ||
                            prop.Value is Size ||
                            prop.Value is Point ||
                            prop.Value is RectangleF ||
                            prop.Value is SizeF ||
                            prop.Value is PointF ||
                            prop.Value is RectangleD ||
                            prop.Value is SizeD ||
                            prop.Value is StiMargins ||
                            prop.Value is PointD ||
                            prop.Value is Guid)
                        {
                            AddCreate(parentName, prop, false);
                        }
                        else
                        {
                            AddInit(parentName, false, prop);
                        }
                    }
                    #endregion

                    else if (prop.Value is StiExpression)
                        AddExpression(parentName, prop);

                    else if (prop.Value is StiFiltersCollection)
                        StiCodeDomFilters.AddFilters(this, parentName, prop.Parent.Value as IStiFilter);

                    else if (prop.Value is StiConditionsCollection)
                        StiCodeDomConditions.AddConditions(this, parentName, prop);

                    else if (prop.Value is StiHighlightCondition)
                        AddHighlight(parentName, prop);

                    else if (prop.IsList && prop.Value is Array)
                    {
                        #region Generate Sort Methods
                        if (prop.Name == "Sort")
                        {
                            var strs = ((string[])prop.Value).Clone() as string[];
                            for (var index = 0; index < strs.Length; index++)
                            {
                                var str = strs[index];
                                if (str.StartsWith("{", StringComparison.InvariantCulture) && str.EndsWith("}", StringComparison.InvariantCulture))
                                {
                                    var methodName = $"Get{StiNameValidator.CorrectName(prop.Parent.Name, report)}_Sort{index}";
                                    str = str.Substring(1, str.Length - 2).Trim();

                                    StiDataBand db = prop.Parent.Value as StiDataBand;
                                    if ((db != null) && (db.DataSource != null) && db.DataSource.Columns.Contains(str))
                                    {
                                        str = StiNameValidator.CorrectName(db.DataSource.Name, report) + "." + str;
                                    }

                                    GenReturnMethodForExpresion(methodName, StiCodeDomFunctions.ParseFunctions(this, str), typeof(object));
                                    strs[index] = "{" + methodName + "}";

                                    var ctm = Members[Members.Count - 1] as CodeMemberMethod;
                                    if (ctm != null)
                                        ctm.Statements.Insert(0, new CodeCommentStatement(GetCheckerInfoString(parentName, "Sort")));
                                }
                            }
                            AddArray(StiNameValidator.CorrectName(prop.Parent.Name, report), "Sort", strs);
                        }
                        #endregion

                        else AddArray(parentName, prop);
                    }

                    else if (prop.Value is StiDataSource)
                        AddDataSource(parentName, prop);

                    else if (prop.Value is Image)
                        AddImage(parentName, propName, prop);

                    else if (prop.Value is StiDataRelation)
                        AddDataRelation(parentName, prop);

                    else if (prop.Value is Color[])
                        AddColorArray(parentName, prop);

                    else if (prop.Value is byte[])
                        AddByteArray(parentName, prop);

                    #region Generate references
                    else if (prop.IsReference || prop.Value is IStiSerializeToCodeAsClass)
                    {
                        if (prop.Value is IStiInteractionClass && ((Stimulsoft.Base.Design.IStiDefault)prop.Value).IsDefault)
                        {
                        }
                        else
                        {
                            reference.Add(prop);
                            reference.Add(parentName);
                        }
                    }
                    #endregion
                }
            }
            #endregion

            #region Second pass - generate the classes, collection and etc.
            foreach (StiPropertyInfo prop in props)
            {
                if (!prop.IsDefaultValueSpecified || !object.Equals(prop.DefaultValue, prop.Value))
                {
                    if (prop.Type.IsValueType ||
                        prop.Type == typeof(string) ||
                        prop.IsReference ||
                        prop.Value is StiExpression ||
                        prop.Value is StiFiltersCollection ||
                        prop.Value is StiConditionsCollection ||
                        prop.Value is StiEvent ||
                        prop.Value is Image ||
                        prop.Value is StiHighlightCondition)
                    {

                    }
                    else if (prop.Value is IStiSerializeToCodeAsClass)
                    {
                        string name = null;
                        if (prop.Value is IStiInteractionClass && ((Stimulsoft.Base.Design.IStiDefault)prop.Value).IsDefault)
                        {
                            Statements.Add(
                                new CodeAssignStatement(
                                new CodePropertyReferenceExpression(
                                new CodeFieldReferenceExpression(
                                new CodeThisReferenceExpression(), StiNameValidator.CorrectName(prop.Parent.Name, report)), prop.Name),
                                new CodePrimitiveExpression(null)));
                        }
                        else
                        {
                            name = prop.Parent != null
                                ? $"{StiNameValidator.CorrectName(prop.Parent.Name, report)}_{prop.Name}"
                                : $"{report.GetReportName()}_{prop.Name}";

                            AddDeclare(prop.Type, name);
                            AddCreate(prop.Type, name);
                            SerializeProps(prop.Properties, false, name, name);

                            #region Generate Code for Drill-Down parameters
                            var interaction = prop.Value as StiInteraction;
                            if (interaction != null)
                            {
                                var isDefault1 = interaction.DrillDownParameter1 == null || interaction.DrillDownParameter1.IsDefault;
                                var isDefault2 = interaction.DrillDownParameter2 == null || interaction.DrillDownParameter2.IsDefault;
                                var isDefault3 = interaction.DrillDownParameter3 == null || interaction.DrillDownParameter3.IsDefault;
                                var isDefault4 = interaction.DrillDownParameter4 == null || interaction.DrillDownParameter4.IsDefault;
                                var isDefault5 = interaction.DrillDownParameter5 == null || interaction.DrillDownParameter5.IsDefault;
                                var isDefault6 = interaction.DrillDownParameter6 == null || interaction.DrillDownParameter6.IsDefault;
                                var isDefault7 = interaction.DrillDownParameter7 == null || interaction.DrillDownParameter7.IsDefault;
                                var isDefault8 = interaction.DrillDownParameter8 == null || interaction.DrillDownParameter8.IsDefault;
                                var isDefault9 = interaction.DrillDownParameter9 == null || interaction.DrillDownParameter9.IsDefault;
                                var isDefault10 = interaction.DrillDownParameter10 == null || interaction.DrillDownParameter10.IsDefault;

                                if (!(isDefault1 && isDefault2 && isDefault3 && isDefault4 && isDefault5 && 
                                    isDefault6 && isDefault7 && isDefault8 && isDefault9 && isDefault10))
                                {
                                    var methodName = prop.Parent.Name + "_FillDrillDownParameters";

                                    #region Attach event handler
                                    CodeExpression ce = new CodeFieldReferenceExpression(
                                        new CodeThisReferenceExpression(), prop.Parent.Name);

                                    this.Statements.Add(
                                        new CodeAttachEventStatement(
                                        ce, "BeforePrint",
                                        new CodeDelegateCreateExpression(
                                        new CodeTypeReference(typeof(EventHandler)),
                                        new CodeThisReferenceExpression(), methodName)));
                                    #endregion

                                    #region Create FillDrillDownParameters
                                    var method = new CodeMemberMethod();
                                    method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                                    method.Name = methodName;
                                    method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "sender"));
                                    method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(EventArgs), "e"));

                                    if (report.ScriptLanguage == StiReportLanguageType.CSharp || report.ScriptLanguage == StiReportLanguageType.JS)
                                    {
                                        method.Statements.Add(new CodeSnippetStatement("StiComponent comp = sender as StiComponent;"));
                                        method.Statements.Add(new CodeSnippetStatement("comp.DrillDownParameters = new System.Collections.Generic.Dictionary<string, object>();"));
                                    }
                                    else
                                    {
                                        method.Statements.Add(new CodeSnippetStatement("Dim comp As StiComponent = TryCast(sender,StiComponent)"));
                                        method.Statements.Add(new CodeSnippetStatement("comp.DrillDownParameters = New System.Collections.Generic.Dictionary(Of String, Object)"));
                                    }

                                    if (!isDefault1)
                                        GenerateDrillDownParameter(parentName, 1, interaction.DrillDownParameter1, method);

                                    if (!isDefault2)
                                        GenerateDrillDownParameter(parentName, 2, interaction.DrillDownParameter2, method);

                                    if (!isDefault3)
                                        GenerateDrillDownParameter(parentName, 3, interaction.DrillDownParameter3, method);

                                    if (!isDefault4)
                                        GenerateDrillDownParameter(parentName, 4, interaction.DrillDownParameter4, method);

                                    if (!isDefault5)
                                        GenerateDrillDownParameter(parentName, 5, interaction.DrillDownParameter5, method);

                                    if (!isDefault6)
                                        GenerateDrillDownParameter(parentName, 6, interaction.DrillDownParameter6, method);

                                    if (!isDefault7)
                                        GenerateDrillDownParameter(parentName, 7, interaction.DrillDownParameter7, method);

                                    if (!isDefault8)
                                        GenerateDrillDownParameter(parentName, 8, interaction.DrillDownParameter8, method);

                                    if (!isDefault9)
                                        GenerateDrillDownParameter(parentName, 9, interaction.DrillDownParameter9, method);

                                    if (!isDefault10)
                                        GenerateDrillDownParameter(parentName, 10, interaction.DrillDownParameter10, method);

                                    Members.Add(method);
                                    #endregion

                                }

                                #region Generate method for interactive sorting
                                if (StiOptions.Engine.Interaction.ForceSortingWithFullTypeConversion && !string.IsNullOrWhiteSpace(interaction.SortingColumn))
                                {
                                    int posDot = interaction.SortingColumn.IndexOf(".");
                                    if (posDot != -1)
                                    {
                                        var dataBandName = interaction.SortingColumn.Substring(0, posDot);
                                        var dataBand = report.GetComponentByName(dataBandName) as StiDataBand;
                                        var dataSource = dataBand?.DataSource;
                                        if (dataSource != null)
                                        {
                                            var methodName = $"Get{StiNameValidator.CorrectName(interaction.SortingColumn, report)}_Sort";

                                            #region Change NameInSource to Name for Relations
                                            string[] parts = interaction.SortingColumn.Substring(posDot + 1).Split(new char[] { '.' });
                                            if (parts.Length > 1)
                                            {
                                                for (int indexPart = 0; indexPart < parts.Length - 1; indexPart++)
                                                {
                                                    var relation = report.Dictionary.Relations[parts[indexPart]];
                                                    if (relation != null)
                                                    {
                                                        parts[indexPart] = relation.Name;
                                                    }
                                                }
                                            }
                                            var methodStr = StiNameValidator.CorrectName(dataSource.Name, report) + "." + string.Join(".", parts);
                                            #endregion

                                            GenReturnMethodForExpresion(methodName, StiCodeDomFunctions.ParseFunctions(this, methodStr), typeof(object));

                                            var ctm = Members[Members.Count - 1] as CodeMemberMethod;
                                            if (ctm != null)
                                                ctm.Statements.Insert(0, new CodeCommentStatement(GetCheckerInfoString(parentName, "Sort")));
                                        }
                                    }
                                }
                                #endregion
                            }
                            #endregion
                        }
                    }

                    #region Generate collections
                    else if (prop.IsList && !(prop.Value is Array))
                    {
                        #region ArrayList With strings
                        if (prop.Value is ArrayList && ((ArrayList)prop.Value).Count > 0 &&
                            ((ArrayList)prop.Value)[0] is string)
                        {
                            AddStringArrayList(prop, prop.Name, prop.Parent.Name);
                        }
                        #endregion

                        else if (prop.Value is StiParametersCollection)
                        {
                            var parameters = prop.Value as StiParametersCollection;
                            if (parameters.Count > 0)
                            {
                                var methodName = $"{prop.Parent.Name}_FillParametersItems";

                                #region Attach event handler
                                CodeExpression ce = new CodeFieldReferenceExpression(
                                    new CodeThisReferenceExpression(), prop.Parent.Name);

                                this.Statements.Add(
                                    new CodeAttachEventStatement(
                                    ce, "FillParameters",
                                    new CodeDelegateCreateExpression(
                                    new CodeTypeReference(typeof(StiFillParametersEventHandler)),
                                    new CodeThisReferenceExpression(), methodName)));
                                #endregion

                                #region Create FillParameters
                                var method = new CodeMemberMethod
                                {
                                    Attributes = MemberAttributes.Public | MemberAttributes.Final,
                                    Name = methodName
                                };
                                method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "sender"));
                                method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(StiFillParametersEventArgs), "e"));

                                foreach (StiParameter param in parameters)
                                {
                                    if (!param.IsDefault)
                                    {
                                        method.Statements.Add(new CodeCommentStatement(GetCheckerInfoString(prop.Parent.Name, param.Name)));

                                        switch (report.ScriptLanguage)
                                        {
                                            case StiReportLanguageType.CSharp:
                                            case StiReportLanguageType.JS:
                                                method.Statements.Add(new CodeSnippetStatement($"e.Value.Add(\"{param.Name}\", {param.Expression.Value});"));
                                                break;

                                            default:
                                                method.Statements.Add(new CodeSnippetStatement($"e.Value.Add(\"{param.Name}\", {param.Expression.Value})"));
                                                break;
                                        }
                                    }
                                }

                                Members.Add(method);
                                #endregion
                            }

                        }
                        else
                        {
                            var ns = SerializeProps(prop.Properties, true, string.Empty, prop.Name);

                            if (prop.Parent != null && prop.Parent.Type == typeof(Stimulsoft.Report.Dialogs.StiForm))
                            {
                                foreach (StiPropertyInfo _info in prop.Properties)
                                {
                                    if (_info.Type.IsSubclassOf(typeof(Control)))
                                    {
                                        ns.Remove(_info.Name);
                                    }
                                }
                            }

                            AddCollection(parentName, prop, ns);
                        }
                    }
                    #endregion

                    #region Generate contents a collection
                    else if (prop.IsKey && isList)
                    {
                        var propName = ReplaceSymbols(prop.Name, report);
                        AddDeclare(prop);
                        AddCreate(parentName, prop, false);
                        names.Add(propName);
                        SerializeProps(prop.Properties, false, propName, propName);
                    }
                    #endregion

                    #region Generate classes not being contents to collections
                    else if (prop.IsKey || CanConvertTo(prop.Type))
                    {
                        if (prop.Name == "value" && parentName.Length == 0)
                        {
                            if (prop.Parent.Parent == null)
                                AddCreate(prop.Parent.Name, prop, true);
                            else
                                AddCreate(prop.Parent.Parent.Name + "." + prop.Parent.Name, prop, true);
                        }
                        else
                        {
                            if (prop.Name != "Dictionary")
                                AddCreate(parentName, prop, false);
                        }
                    }
                    #endregion
                }
            }
            #endregion

            #region Third pass - generate events
            foreach (StiPropertyInfo prop in props)
            {
                if (prop.Value is StiEvent propEvent)
                {
                    if (propEvent.Script.StartsWith(StiBlocksConst.IdentXml))
                    {
                        CodeExpression core = new CodeThisReferenceExpression();

                        if (referenceName != "this")
                            core = new CodePropertyReferenceExpression(core, parentName);

                        var left = new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(core, propEvent.PropertyName), "Script");

                        Statements.Add(new CodeAssignStatement(left, new CodePrimitiveExpression(propEvent.Script)));
                    }
                    else
                    {
                        AddEvent(parentName, prop);
                    }
                }
            }
            #endregion

            return names;
        }

        private void GenerateDrillDownParameter(string parentName, int parameterIndex, StiDrillDownParameter parameter, CodeMemberMethod method)
        {
            var value = parameter.Expression.Value;
            if (string.IsNullOrWhiteSpace(value))
                value = "\"\"";

            method.Statements.Add(new CodeCommentStatement(GetCheckerInfoString(parentName, $"DrillDownParameter{parameterIndex}")));
            var str = $"comp.DrillDownParameters.Add(\"{parameter.Name}\", {value})";

            if (report.ScriptLanguage == StiReportLanguageType.CSharp || report.ScriptLanguage == StiReportLanguageType.JS)
                str += ";";
            
            method.Statements.Add(new CodeSnippetStatement(str));            
        }

        internal string GetReportTypeName(string name)
        {
            string script = report.Script;
            int index = 0;
            if (script == null) return name;

            if (report.ScriptLanguage == StiReportLanguageType.CSharp || report.ScriptLanguage == StiReportLanguageType.JS)
                index = script.IndexOf("class", StringComparison.InvariantCulture);
            else
                index = script.IndexOf("Class", StringComparison.InvariantCulture);

            index += 5;
            while (index < script.Length && (!char.IsLetterOrDigit(script[index])))
            {
                index++;
            }
            StringBuilder sb = new StringBuilder(20);

            while (index < script.Length && (char.IsLetterOrDigit(script[index]) || script[index] == '_'))
            {
                sb.Append(script[index]);
                index++;
            }
            return sb.ToString();
        }


        /// <summary>
        ///  Serializes into a code.
        /// </summary>
        /// <param name="report">Object for serialization.</param>
        /// <param name="name">Class name for formation.</param>
        /// <param name="language">Serialization language.</param>
        /// <returns>Serialized code.</returns>
        public string Serialize(StiReport report, string name, StiLanguage language)
        {
            return Serialize(report, name, language, false);
        }

        /// <summary>
        ///  Serializes into a code.
        /// </summary>
        /// <param name="report">Object for serialization.</param>
        /// <param name="name">Class name for formation.</param>
        /// <param name="language">Serialization language.</param>
        /// <returns>Serialized code.</returns>
        public string Serialize(StiReport report, string name, StiLanguage language, object standaloneReportType)
        {
            return Serialize(report, name, language, true, false, standaloneReportType);
        }

        /// <summary>
        ///  Serializes into a code.
        /// </summary>
        /// <param name="report">Object for serialization.</param>
        /// <param name="name">Class name for formation.</param>
        /// <param name="language">Serialization language.</param>
        /// <returns>Serialized code.</returns>
        public string Serialize(StiReport report, string name, StiLanguage language, bool saveForInheritedReports)
        {
            return Serialize(report, name, language, true, saveForInheritedReports);
        }

        /// <summary>
        ///  Serializes into a code.
        /// </summary>
        /// <param name="report">Object for serialization.</param>
        /// <param name="name">Class name for formation.</param>
        /// <param name="language">Serialization language.</param>
        /// <param name="serializeData">Do not serialize the sources of data and relation.</param>
        /// <returns>Serialized code.</returns>
        public string Serialize(StiReport report, string name, StiLanguage language, bool serializeData,
            bool saveForInheritedReports)
        {
            return Serialize(report, name, language, serializeData, saveForInheritedReports, null);
        }

        /// <summary>
        ///  Serializes into a code.
        /// </summary>
        /// <param name="report">Object for serialization.</param>
        /// <param name="name">Class name for formation.</param>
        /// <param name="language">Serialization language.</param>
        /// <param name="serializeData">Do not serialize the sources of data and relation.</param>
        /// <returns>Serialized code.</returns>
        public string Serialize(StiReport report, string name, StiLanguage language, bool serializeData,
            bool saveForInheritedReports, object standaloneReportType)
        {
            if (report.IsPageDesigner) return "";

            this.report = report;
            this.components = report.GetComponents();
            this.provider = language.GetProvider();
            this.thisReportTypeName = GetReportTypeName(name);

            memberMethod = new CodeMemberMethod();

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            generator = provider.CreateGenerator() as StiCodeGenerator;

            if (generator == null) 
                throw new Exception("Provider must be inherited from StiCodeGenerator");

            var generatorOptions = new CodeGeneratorOptions
            {
                BlankLinesBetweenMembers = true,
                BracingStyle = "C"
            };

            var sr = new StiSerializing(new StiReportObjectStringConverter());

            Graphs = new StiGraphs();
            Graphs.Add("this", 0);

            Members.Clear();

            if (!saveForInheritedReports)
            {
                StiCodeDomVariables.Serialize(this, report);
                StiCodeDomResources.Serialize(this, report);

                var props = sr.Serialize(report, StiSerializeTypes.SerializeToCode);
                sr.SetReferenceSerializing();

                SerializeProps(props, false, string.Empty, "this");

                #region Serialize DataBandsUsedInPageTotals property
                if (report.DataBandsUsedInPageTotals != null && report.DataBandsUsedInPageTotals.Length > 0)
                {
                    var propInfo = new StiPropertyInfo(
                        "DataBandsUsedInPageTotals", report.DataBandsUsedInPageTotals, null, false, false, false, true, 
                        report.DataBandsUsedInPageTotals.GetType().ToString());

                    int count2 = 0;
                    foreach (object item in (IList)report.DataBandsUsedInPageTotals)
                    {
                        if (item == null) continue;
                        propInfo.Properties.Add(new StiPropertyInfo("value", item, null, false, false, false, false, null));
                        count2++;
                    }
                    propInfo.Count = count2;

                    var props2 = new StiPropertyInfoCollection();
                    props2.Add(propInfo);
                    SerializeProps(props2, false, string.Empty, "this");
                }
                #endregion

                #region Serialize ListOfUsedData property
                if (report.RetrieveOnlyUsedData)
                {
                    var usedDataSourceNames = StiDataSourceHelper.GetUsedDataSourcesNamesList(report).ToArray();
                    if (usedDataSourceNames.Length > 0)
                    {
                        var propInfo = new StiPropertyInfo(
                            "ListOfUsedData", usedDataSourceNames, null, false, false, false, true, 
                            usedDataSourceNames.GetType().ToString());

                        int count2 = 0;
                        foreach (string item in usedDataSourceNames)
                        {
                            if (item == null) continue;
                            propInfo.Properties.Add(new StiPropertyInfo("value", item, null, false, false, false, false, null));
                            count2++;
                        }
                        propInfo.Count = count2;

                        var props2 = new StiPropertyInfoCollection();
                        props2.Add(propInfo);
                        SerializeProps(props2, false, string.Empty, "this");
                    }
                }
                #endregion
            }

            #region Adds references
            for (int index = 0; index < reference.Count; index += 2)
            {
                var prop = reference[index] as StiPropertyInfo;
                if (prop.Name != "Item")
                {
                    if (prop.Value is IStiSerializeToCodeAsClass)
                    {
                        if (prop.Parent != null) 
                            prop.Value = $"{StiNameValidator.CorrectName(prop.Parent.Name, report)}_{prop.Name}";
                        else 
                            prop.Value = $"{report.GetReportName()}_{prop.Name}";

                        AddInit((string)reference[index + 1], true, prop);
                    }
                    else
                    {
                        prop.Value = Graphs[(int)prop.ReferenceCode];
                        AddInit((string)reference[index + 1], true, prop);
                    }
                }
                else
                {
                    var comp = prop.Value as StiComponent;
                    if (comp != null)
                    {
                        CodeExpression add =
                            new CodeMethodInvokeExpression(
                            new CodeFieldReferenceExpression(
                            new CodeFieldReferenceExpression(
                            new CodeThisReferenceExpression(),
                            prop.Parent.Parent.Name), prop.Parent.Name), "Add", new CodeSnippetExpression(comp.Name));
                        Statements.Add(add);
                    }
                }
            }
            #endregion

            #region StiDefaultNamespace
            var codeNamespace = new CodeNamespace(StiOptions.Engine.DefaultNamespace);
            #endregion

            #region StiNamespaceImportService
            foreach (string str in StiOptions.Engine.Namespaces)
            {
                codeNamespace.Imports.Add(new CodeNamespaceImport(str));
            }
            #endregion

            var typeDeclaration = new CodeTypeDeclaration(name);
            typeDeclaration.IsClass = true;

            if (string.IsNullOrEmpty(report.MasterReport))
                typeDeclaration.BaseTypes.Add(report.GetType());
            else 
                typeDeclaration.BaseTypes.Add(typeof(StiReport));

            typeDeclaration.TypeAttributes = TypeAttributes.Public;

            if (!saveForInheritedReports)
            {
                ConnectAggregatesBeginEnd();
                ConnectAggreagatesBuild();
                ConnectTextBoxReportEnd(compsProcessAtEndRemitted);
                ConnectTextBoxReportEnd(compsWordsToEndRemitted);
                ConnectChartProcessAtEnd();

                BuildArrayAggregates();
            }

            #region CheckEndRenderRuntimes
            bool reportHasPages = false;
            foreach (StiPage page in report.Pages)
            {
                if (page.IsPage)
                {
                    reportHasPages = true;
                    break;
                }
            }
            if (reportHasPages)
            {
                var stats3 = new CodeStatementCollection();
                var setMethod3 = new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(StiSimpleText)), "CheckEndRenderRuntimes"),
                    new CodeExpression[] { new CodeThisReferenceExpression() });
                stats3.Add(setMethod3);

                if (compsWordsToEndRemitted.Count > 0 || compsProcessAtEndRemitted.Count > 0)
                {
                    var setMethod2 = new CodeMethodInvokeExpression(
                        new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(StiSimpleText)), "ProcessEndRenderSetText"),
                        new CodeExpression[] { new CodeThisReferenceExpression() });
                    stats3.Add(setMethod2);
                }

                string eventName3 = "CheckEndRenderRuntimes__EndRender";
                var ebr3 = new StiEndRenderEvent();
                GenAddEvent(eventName3, null, ebr3);
                GenEventMethod(eventName3, ebr3, stats3);
            }
            #endregion

            #region Correction for EndRender event invoke order
            var posEndRenderEvent = -1;
            var nameEndRenderEvent = $"{report.GetReportName()}_EndRender";

            foreach (CodeStatement codeStatement in Statements)
            {
                var stat = codeStatement as CodeAttachEventStatement;
                if (stat != null && stat.Event.EventName == "EndRender" && (stat.Listener as CodeDelegateCreateExpression).MethodName == nameEndRenderEvent)
                {
                    posEndRenderEvent = Statements.IndexOf(codeStatement);
                    break;
                }
            }
            if (posEndRenderEvent != -1)
            {
                var stat = Statements[posEndRenderEvent];
                Statements.RemoveAt(posEndRenderEvent);
                Statements.Add(stat);
            }
            #endregion

            #region Initialize
            Members.Add(memberMethod);
            memberMethod.Attributes = MemberAttributes.Private;
            memberMethod.Name = "InitializeComponent";
            #endregion

            #region Constructor
            var constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;

            constructor.Statements.Add(
                new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(), "InitializeComponent"));

            #region GetMasterString
            if (saveForInheritedReports)
            {
                var assignProperty =
                    new CodeAssignStatement(
                    new CodePropertyReferenceExpression(
                    new CodeThisReferenceExpression(), "MasterReport"),
                    new CodePrimitiveExpression(report.SaveToString()));

                constructor.Statements.Add(assignProperty);
            }
            #endregion

            typeDeclaration.Members.Add(constructor);
            #endregion

            #region NeedsCompiling
            if (!saveForInheritedReports)
            {
                memberMethod.Statements.Add(
                    new CodeAssignStatement(
                    new CodePropertyReferenceExpression(
                    new CodeThisReferenceExpression(), "NeedsCompiling"),
                    new CodePrimitiveExpression(false)));
            }
            #endregion

            memberMethod.Statements.AddRange(Statements);
            Statements.Clear();
            memberMethod.Statements.AddRange(CollectionStatements);
            CollectionStatements.Clear();

            if (serializeData && (!saveForInheritedReports))
            {
                StiCodeDomRelations.Serialize(this, report);
                StiCodeDomDataSources.Serialize(this, report);
                StiCodeDomDatabases.Serialize(this, report);
                StiCodeDomBusinessObjects.Serialize(this, report);
            }

            StiCodeDomCharts.Serialize(this, report);

            memberMethod.Statements.AddRange(Statements);
            memberMethod.Statements.AddRange(CollectionStatements);

            StiCodeDomStandalone.Serialize(this, report, standaloneReportType, typeDeclaration, name);

            #region Wrap big method
            int count = 1000;

            if (memberMethod.Statements.Count > count)
            {
                var methods = new List<CodeMemberMethod>();
                methods.Add(memberMethod);

                int indexInsert = Members.IndexOf(memberMethod) + 1;
                CodeMemberMethod memberMethod2 = null;

                int indexMethod = 2;
                int indexMember = 0;
                while (memberMethod.Statements.Count > count)
                {
                    if (indexMember == 0)
                    {
                        memberMethod2 = new CodeMemberMethod();
                        Members.Insert(indexInsert++, memberMethod2);
                        memberMethod2.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                        memberMethod2.Name = $"InitializeComponent{indexMethod}";
                        indexMethod++;
                        methods.Add(memberMethod2);
                    }

                    memberMethod2.Statements.Add(memberMethod.Statements[count]);
                    memberMethod.Statements.RemoveAt(count);

                    indexMember++;
                    
                    if (indexMember == count) 
                        indexMember = 0;
                }

                indexMethod = 0;
                foreach (var method in methods)
                {
                    if (indexMethod < methods.Count - 1)
                    {
                        CodeMemberMethod nextMethod = methods[indexMethod + 1];

                        //new version
                        memberMethod.Statements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(),
                            nextMethod.Name));

                    }
                    indexMethod++;
                }
            }
            #endregion

            #region Comments add
            CodeTypeMember firstMember = null;
            CodeTypeMember lastMember = null;
            foreach (CodeTypeMember field in Members)
            {
                if (field is CodeMemberField && firstMember == null) 
                    firstMember = field;

                else if (field is CodeMemberMethod && (!(lastMember is CodeTypeDeclaration))) 
                    lastMember = field;

                else if (field is CodeTypeDeclaration) 
                    lastMember = field;
            }

            if (firstMember == null) 
                firstMember = lastMember;

            firstMember.Comments.Add(new StiCodeRegionStart(StrGenCode));
            lastMember.Comments.Add(new StiCodeRegionEnd(StrGenCode));
            #endregion

            typeDeclaration.Members.AddRange(Members);

            codeNamespace.Types.Add(typeDeclaration);

            generator.GenerateCodeFromNamespace(codeNamespace, sw, generatorOptions);

            sw.Flush();
            sw.Close();

            if (!StiOptions.Engine.GeneratePartialReportClass)
            {
                return sb.ToString();
            }
            else
            {
                string str = sb.ToString();
                string strLower = str.ToLower(CultureInfo.InvariantCulture);

                int index = strLower.IndexOf("class", StringComparison.InvariantCulture);

                switch (report.ScriptLanguage)
                {
                    case StiReportLanguageType.CSharp:
                    case StiReportLanguageType.JS:
                        return str.Insert(index, "partial ");

                    default:
                        return str.Insert(index, "Partial ");
                }
            }
        }
    }
}