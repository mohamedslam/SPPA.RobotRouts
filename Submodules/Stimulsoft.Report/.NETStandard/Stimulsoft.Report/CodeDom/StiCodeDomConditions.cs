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
using System.CodeDom;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.CrossTab;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.CodeDom
{
    internal class StiCodeDomConditions
    {
        private static CodeStatement[] GetConditionActions(StiCodeDomSerializator serializator, StiComponent component, StiCondition condition, bool isAssignTextExpression)
        {
            var list = new List<CodeStatement>();

            if (!isAssignTextExpression)
            {
                if (condition.Enabled || StiOptions.Engine.ApplyConditionStyleForDisabledComponents)
                {
                    #region Style
                    if (string.IsNullOrEmpty(condition.Style))
                    {
                        #region IStiTextBrush
                        if (component is IStiTextBrush && ((condition.Permissions & StiConditionPermissions.TextColor) > 0))
                        {
                            list.Add(new CodeAssignStatement(
                                new CodeFieldReferenceExpression(
                                new CodeCastExpression(typeof(IStiTextBrush),
                                new CodeArgumentReferenceExpression("sender")), "TextBrush"),
                                serializator.GetObjectCreateExpression(typeof(StiSolidBrush),
                                new StiSolidBrush(condition.TextColor))));
                        }
                        #endregion

                        #region IStiBrush
                        if (component is IStiBrush && ((condition.Permissions & StiConditionPermissions.BackColor) > 0))
                        {
                            list.Add(new CodeAssignStatement(
                                new CodeFieldReferenceExpression(
                                new CodeCastExpression(typeof(IStiBrush),
                                new CodeArgumentReferenceExpression("sender")), "Brush"),
                                serializator.GetObjectCreateExpression(typeof(StiSolidBrush),
                                new StiSolidBrush(condition.BackColor))));
                        }
                        #endregion

                        #region IStiFont
                        if (component is IStiFont &&
                            ((condition.Permissions & StiConditionPermissions.Font) > 0 ||
                            (condition.Permissions & StiConditionPermissions.FontSize) > 0 ||
                            (condition.Permissions & StiConditionPermissions.FontStyleBold) > 0 ||
                            (condition.Permissions & StiConditionPermissions.FontStyleItalic) > 0 ||
                            (condition.Permissions & StiConditionPermissions.FontStyleUnderline) > 0 ||
                            (condition.Permissions & StiConditionPermissions.FontStyleStrikeout) > 0))
                        {
                            var args = new[]
                            {
                            new CodeSnippetExpression("sender"),
                            serializator.GetObjectCreateExpression(typeof(Font), condition.Font),
                            serializator.GetPropertyExpression(condition.Permissions)
                        };

                            list.Add(new CodeExpressionStatement(
                                new CodeMethodInvokeExpression(
                                new CodeMethodReferenceExpression(
                                new CodeTypeReferenceExpression(typeof(StiConditionHelper)), "ApplyFont"),
                                args)));
                        }
                        #endregion

                        #region IStiBorder
                        if (component is IStiBorder && condition.BorderSides != StiConditionBorderSides.NotAssigned &&
                             ((condition.Permissions & StiConditionPermissions.Borders) > 0))
                        {
                            #region StiConditionBorderSides -> StiBorderSides
                            var sides = StiBorderSides.None;

                            if ((condition.BorderSides & StiConditionBorderSides.Left) > 0)
                                sides |= StiBorderSides.Left;

                            if ((condition.BorderSides & StiConditionBorderSides.Right) > 0)
                                sides |= StiBorderSides.Right;

                            if ((condition.BorderSides & StiConditionBorderSides.Top) > 0)
                                sides |= StiBorderSides.Top;

                            if ((condition.BorderSides & StiConditionBorderSides.Bottom) > 0)
                                sides |= StiBorderSides.Bottom;
                            #endregion

                            list.Add(new CodeAssignStatement(
                                new CodePropertyReferenceExpression(
                                new CodeCastExpression(typeof(IStiBorder),
                                new CodeArgumentReferenceExpression("sender")), "Border"),

                                new CodeCastExpression(typeof(StiBorder),
                                new CodeMethodInvokeExpression(
                                new CodePropertyReferenceExpression(
                                new CodeCastExpression(typeof(IStiBorder),
                                new CodeArgumentReferenceExpression("sender")), "Border"),
                                "Clone"))));

                            list.Add(new CodeAssignStatement(
                                new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(
                                new CodeCastExpression(typeof(IStiBorder),
                                new CodeArgumentReferenceExpression("sender")), "Border"), "Side"),
                                serializator.GetPropertyExpression(sides)));
                        }
                        #endregion
                    }
                    #endregion

                    else
                    {
                        var args = new CodeExpression[]
                        {
                        new CodeSnippetExpression("sender"),
                        new CodePrimitiveExpression(condition.Style)
                        };

                        list.Add(new CodeExpressionStatement(
                            new CodeMethodInvokeExpression(
                            new CodeMethodReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(StiConditionHelper)), "Apply"),
                            args)));
                    }
                }

                list.Add(new CodeAssignStatement(
                        new CodeFieldReferenceExpression(
                        new CodeCastExpression(typeof(StiComponent),
                        new CodeArgumentReferenceExpression("sender")), "Enabled"),
                        new CodePrimitiveExpression(condition.Enabled)));
            }
            else
            {
                #region CanAssignExpression
                if (component is StiText && condition.CanAssignExpression && condition.AssignExpression != null && condition.AssignExpression.Trim().Length > 0)
                {
                    string expression = condition.AssignExpression;
                    expression = StiCodeDomFunctions.ParseFunctions(serializator, expression);
                    expression = StiCodeDomTotalsFunctionsParser.ProcessTotals(serializator, expression, component.Name);

                    list.Add(new CodeAssignStatement(
                        new CodeFieldReferenceExpression(
                        new CodeCastExpression(typeof(StiText),
                        new CodeArgumentReferenceExpression("sender")), "TextValue"),
                        new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(), "ToString", 
                        new CodeSnippetExpression("sender"), 
                        new CodeSnippetExpression(expression), 
                        new CodeSnippetExpression("true"))));
                }
                #endregion
            }

            if (StiOptions.Engine.StopProcessingAtFirstTrueCondition || condition.BreakIfTrue)
            {
                list.Add(
                    serializator.report.ScriptLanguage == StiReportLanguageType.CSharp || 
                    serializator.report.ScriptLanguage == StiReportLanguageType.JS
                        ? new CodeSnippetStatement("return;") 
                        : new CodeSnippetStatement("Return"));
            }

            return list.ToArray();
        }


        internal static void AddConditions(
            StiCodeDomSerializator serializator, string parent, StiPropertyInfo prop)
        {
            var component = prop.Parent.Value as StiComponent;

            bool isCrossField = prop.Parent.Value is IStiCrossTabField;
            //bool isTextField = prop.Parent.Value is StiText;

            var conds = prop.Value as StiConditionsCollection;

            var conditions = new List<StiBaseCondition>();
            var indicatorConditions = new List<StiBaseCondition>();
            foreach (StiBaseCondition cond in conds)
            {
                if (cond is StiCondition)
                    conditions.Add(cond);
                else if (cond is IStiIndicatorCondition)
                    indicatorConditions.Add(cond);
            };

            #region Process StiCondition classes
            if (conditions != null && conditions.Count > 0)
            {
                int countOfPass = 1;
                countOfPass++;
                for (int index2 = 0; index2 < countOfPass; index2++)
                {
                    bool isAssignTextField = index2 == 1;

                    CodeMemberMethod memberMethod = new CodeMemberMethod();
                    memberMethod.Name = parent + "_Conditions";
                    if (isAssignTextField) memberMethod.Name += "TextValue";

                    #region Parameters
                    memberMethod.Parameters.Add(
                        new CodeParameterDeclarationExpression(typeof(object), "sender"));

                    if (component is StiText && (isCrossField || isAssignTextField))//isTextField cheeck removed because we need check conditions before if (comp.Enabled checking
                    {
                        memberMethod.Parameters.Add(
                            new CodeParameterDeclarationExpression(typeof(StiValueEventArgs), "e"));
                    }
                    else
                    {
                        memberMethod.Parameters.Add(
                            new CodeParameterDeclarationExpression(typeof(EventArgs), "e"));
                    }
                    #endregion

                    #region Tag, ToolTip, Hyperlink
                    if (component is StiText && (isCrossField || isAssignTextField))//isTextField cheeck removed because we need check conditions before if (comp.Enabled checking
                    {
                        string conditionStr = null;
                        string tagStr = null;
                        string tooltipStr = null;
                        string hypelinkStr = null;

                        #region Create declarations of special variables
                        if (serializator.report.ScriptLanguage == StiReportLanguageType.VB || serializator.report.ScriptLanguage == StiReportLanguageType.JS)
                        {
                            conditionStr = isCrossField ? "TypeOf e.Value is Decimal" : "TypeOf e.Value is String";
                            tagStr = "Dim tag As String = CType(CType(sender, StiComponent).TagValue, String)";
                            tooltipStr = "Dim tooltip As String = CType(CType(sender, StiComponent).TooltipValue, String)";
                            hypelinkStr = "Dim hyperlink As String = CType(CType(sender, StiComponent).HyperlinkValue, String)";
                        }
                        else
                        {
                            conditionStr = isCrossField ? "e.Value is decimal" : "e.Value is string";
                            tagStr = "string tag = ((StiComponent)sender).TagValue as string;";
                            tooltipStr = "string tooltip = ((StiComponent)sender).ToolTipValue as string;";
                            hypelinkStr = "string hyperlink = ((StiComponent)sender).HyperlinkValue as string;";
                        }
                        #endregion

                        #region Assign special variable
                        if (isCrossField)
                        {
                            StiCrossSummary summary = component as StiCrossSummary;

                            Type type = typeof(decimal);
                            if (summary != null && summary.Summary == Stimulsoft.Report.CrossTab.Core.StiSummaryType.None)
                            {
                                type = typeof(object);

                                if (serializator.report.ScriptLanguage == StiReportLanguageType.VB)
                                    conditionStr = "TypeOf e.Value is Object";

                                else
                                    conditionStr = "e.Value is object";
                            }

                            var createVariable =
                                new CodeVariableDeclarationStatement(
                                 type, "value", new CodePrimitiveExpression(0));

                            var ifStat = new CodeConditionStatement(
                                new CodeSnippetExpression(conditionStr), new CodeAssignStatement(
                                    new CodeVariableReferenceExpression("value"),
                                    new CodeCastExpression(type,
                                        new CodePropertyReferenceExpression(
                                            new CodeArgumentReferenceExpression("e"), "Value"))));

                            memberMethod.Statements.Add(createVariable);
                            memberMethod.Statements.Add(ifStat);
                        }
                        else
                        {
                            var createVariable = new CodeVariableDeclarationStatement(
                                typeof(string), "value",
                                new CodeCastExpression(typeof(string),
                                new CodePropertyReferenceExpression(
                                new CodeArgumentReferenceExpression("e"), "Value")));

                            memberMethod.Statements.Add(createVariable);
                        }
                        #endregion

                        memberMethod.Statements.Add(new CodeSnippetStatement(tagStr));
                        memberMethod.Statements.Add(new CodeSnippetStatement(tooltipStr));
                        memberMethod.Statements.Add(new CodeSnippetStatement(hypelinkStr));
                    }
                    #endregion

                    #region Condition Actions
                    //for (int index = conditions.Count - 1; index >= 0; index--)
                    for (int index = 0; index < conditions.Count; index++)
                    {
                        int conditionIndex = StiOptions.Engine.InvertConditionsProcessingOrder ? conditions.Count - 1 - index : index;
                        StiCondition condition = conditions[conditionIndex] as StiCondition;

                        CodeStatement[] stats = GetConditionActions(serializator,
                            prop.Parent.Value as StiComponent, condition, isAssignTextField);

                        if (stats != null && stats.Length > 0)
                        {
                            CodeExpression exprCondition = null;

                            #region MultiCondition
                            if (condition is StiMultiCondition)
                            {
                                exprCondition = StiCodeDomFilters.AddFilters(
                                    serializator, condition as IStiFilter);
                            }
                            #endregion

                            #region Condition
                            else
                            {
                                exprCondition = StiCodeDomFilters.GetFilterExpression(
                                    serializator, prop.Parent.Value as IStiFilter, condition, true);
                            }
                            #endregion

                            if (exprCondition != null)
                            {
                                CodeConditionStatement codeCondition =
                                    new CodeConditionStatement(exprCondition, stats, new CodeStatement[0]);

                                memberMethod.Statements.Add(codeCondition);
                            }
                        }
                    }
                    #endregion

                    #region Assign Expression
                    bool allowAssignTextField = false;
                    if (isAssignTextField)
                    {
                        foreach (StiCondition condition in conditions)
                        {
                            if (condition.CanAssignExpression && condition.AssignExpression != null && condition.AssignExpression.Trim().Length > 0)
                            {
                                allowAssignTextField = true;
                                break;
                            }
                        }
                    }

                    if (memberMethod.Statements.Count > 0 && (allowAssignTextField || (!isAssignTextField)))
                    {
                        if (memberMethod.Statements.Count > 0)
                        {
                            memberMethod.Statements.Insert(0, new CodeCommentStatement(StiCodeDomSerializator.GetCheckerInfoString(parent, prop.Name)));
                        }

                        serializator.Members.Add(memberMethod);

                        #region Attach method to event
                        CodeExpression ce = new CodeFieldReferenceExpression(
                            new CodeThisReferenceExpression(), parent);

                        #region Process Text Components
                        if (component is StiText && (isCrossField || isAssignTextField))    //isTextField check removed because we need check conditions before if (comp.Enabled checking
                        {
                            serializator.Statements.Add(
                                new CodeAttachEventStatement(
                                ce, "TextProcess",
                                new CodeDelegateCreateExpression(
                                new CodeTypeReference(typeof(StiValueEventHandler)),
                                new CodeThisReferenceExpression(), memberMethod.Name)));
                        }
                        #endregion

                        #region Process CrossTab Components
                        else
                        {
                            serializator.Statements.Add(
                                new CodeAttachEventStatement(
                                ce, "BeforePrint",
                                new CodeDelegateCreateExpression(
                                new CodeTypeReference(typeof(EventHandler)),
                                new CodeThisReferenceExpression(), memberMethod.Name)));
                        }
                        #endregion

                        #endregion
                    }
                    #endregion
                }
            }
            #endregion

            #region Process IStiIndicator classes
            if (indicatorConditions.Count > 0)
            {
                foreach (StiBaseCondition condition in indicatorConditions)
                {
                    CodeExpression expr = new CodeObjectCreateExpression(condition.GetType(),
                        serializator.GetArguments(condition.GetType(), condition));

                    serializator.Statements.Add(new CodeMethodInvokeExpression(
                        new CodePropertyReferenceExpression(
                        new CodePropertyReferenceExpression(
                        new CodeThisReferenceExpression(), parent), "Conditions"),
                        "Add", expr
                        ));

                    break;//Process only first indicator condition, only one indicator per component!
                }
            }
            #endregion
        }
    }
}
