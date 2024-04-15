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
using System.CodeDom;

namespace Stimulsoft.Report.CodeDom
{
	internal class StiCodeDomStandalone
	{
		internal static void Serialize(StiCodeDomSerializator serializator, StiReport report, object standaloneReportType,
			CodeTypeDeclaration typeDeclaration, string name)
		{
			if (standaloneReportType != null)
			{
			    var mainMemberMethod = new CodeMemberMethod
			    {
			        Attributes = MemberAttributes.Public | MemberAttributes.Static,
			        Name = "Main"
			    };
			    mainMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string[]), "args"));
                mainMemberMethod.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(STAThreadAttribute))));
                serializator.Members.Add(mainMemberMethod);

                mainMemberMethod.Statements.Add(
					new CodeVariableDeclarationStatement(
					new CodeTypeReference(serializator.GetReportTypeName(name)), "report", 
					new CodeObjectCreateExpression(serializator.GetReportTypeName(name))));

				CodeMethodInvokeExpression expr = null;

				switch ((StiStandaloneReportType)standaloneReportType)
                {
                    #region Show
                    case StiStandaloneReportType.Show:
						expr = new CodeMethodInvokeExpression(
						new CodeVariableReferenceExpression("report"), 
							"Show", new CodePrimitiveExpression(true));
						break;
                    #endregion

                    #region Print
                    case StiStandaloneReportType.Print:
						expr = new CodeMethodInvokeExpression(
							new CodeVariableReferenceExpression("report"), 
							"Print");
						break;
                    #endregion

                    #region ShowWithWpf
                    case StiStandaloneReportType.ShowWithWpf:
                        expr = new CodeMethodInvokeExpression(
                        new CodeVariableReferenceExpression("report"),
                            "ShowWithWpf", new CodePrimitiveExpression(true));
                        break;
                    #endregion

                    #region PrintWithWpf
                    case StiStandaloneReportType.PrintWithWpf:
                        expr = new CodeMethodInvokeExpression(
                            new CodeVariableReferenceExpression("report"),
                            "PrintWithWpf");
                        break;
                    #endregion
                }

				mainMemberMethod.Statements.Add(expr);
			}
		}
	}
}
