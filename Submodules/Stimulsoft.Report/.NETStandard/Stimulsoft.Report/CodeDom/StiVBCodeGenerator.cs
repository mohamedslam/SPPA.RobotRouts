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

using System.Diagnostics;
using System;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Security.Permissions;


namespace Stimulsoft.Report.CodeDom
{
	/// <summary>
	/// Class describes VB code generator. 
	/// </summary>
	public class StiVBCodeGenerator : StiCodeGenerator
	{
		static StiVBCodeGenerator()
		{
			foreach (string str in keywords)
			{
				keywordsHashtable.Add(str, str);
			}
		}


		protected bool AllowLateBound(CodeCompileUnit e)
		{
			object obj1 = e.UserData["AllowLateBound"];
			if ((obj1 != null) && (bool)obj1)
			{
				return ((bool) obj1); 
			}
			return true; 
		} 


		private static void AppendEscapedChar(StringBuilder b, char value)
		{
			b.Append("&Microsoft.VisualBasic.ChrW(");
			int num1 = value;
			b.Append(num1.ToString(CultureInfo.InvariantCulture));
			b.Append(")");
		} 


		protected override void ContinueOnNewLine(string st)
		{
			this.Output.Write(st);
			this.Output.WriteLine(" _");
			//this.Output.WriteLine(string.Empty);
		}

		
		private static string ConvertToCommentEscapeCodes(string value)
		{
			StringBuilder builder1;
			int num1;
			char ch1;
			builder1 = new StringBuilder(value.Length);
			num1 = 0;
			goto L_00AC;
 
			L_0013:
				ch1 = value[num1];
			if (ch1 != 10)
			{
				if (ch1 != 13)
				{
					switch ((ch1 - 8232))
					{
						case 0:
						{
							goto L_0081;
 
						}
						case 1:
						{
							goto L_0081;
 
						}
 
					}
					goto L_009A;
 
				}
				if ((num1 < (value.Length - 1)) && (value[(num1 + 1)] == 10))
				{
					builder1.Append(" '");
					num1 = (num1 + 1);
					goto L_00A8;
 
				}
				builder1.Append(" '");
				goto L_00A8;
 
			}
			builder1.Append(" '");
			goto L_00A8;
 
			L_0081:
				builder1.Append(value[num1]);
			builder1.Append(39);
			goto L_00A8;
 
			L_009A:
				builder1.Append(value[num1]);
 
			L_00A8:
				num1 = (num1 + 1);
 
			L_00AC:
				if (num1 < value.Length)
				{
					goto L_0013;
 
				}
			return builder1.ToString(); 
		}
		
		protected override string CreateEscapedIdentifier(string name)
		{
			if (IsKeyword(name))
			{
				return string.Concat("[", name, "]"); 
			}
			return name; 
		}
		
		protected override string CreateValidIdentifier(string name)
		{
			if (IsKeyword(name))
			{
				return string.Concat("_", name); 
			}
			return name; 
		}

		//
		private void EnsureInDoubleQuotes(ref bool fInDoubleQuotes, StringBuilder b)
		{
			if (fInDoubleQuotes != false)
			{
				return; 
			}
			b.Append("&\"");
			fInDoubleQuotes = true;
 
		}
		
		//
		private void EnsureNotInDoubleQuotes(ref bool fInDoubleQuotes, StringBuilder b)
		{
			if (!fInDoubleQuotes)return; 
			b.Append('"');
			fInDoubleQuotes = false;
 
		} 


		protected override void GenerateArgumentReferenceExpression(CodeArgumentReferenceExpression e)
		{
			this.OutputIdentifier(e.ParameterName);
		} 

		protected override void GenerateArrayCreateExpression(CodeArrayCreateExpression e)
		{
			CodeExpressionCollection collection1;
			this.Output.Write("New ");
			OutputType(e.CreateType);
			collection1 = e.Initializers;
			if (collection1.Count > 0)
			{
				this.Output.Write("() {");
				this.Indent = (this.Indent + 1);
				this.OutputExpressionList(collection1);
				this.Indent = (this.Indent - 1);
				this.Output.Write("}");
				return; 
			}
			this.Output.Write("(");
			if (e.SizeExpression != null)
			{
				this.Output.Write("(");
				this.GenerateExpression(e.SizeExpression);
				this.Output.Write(") - 1");
 
			}
			else
			{
				this.Output.Write((e.Size - 1));
			}
			this.Output.Write(") {}");
 
		}
		protected override void GenerateArrayIndexerExpression(CodeArrayIndexerExpression e)
		{
			bool flag1;
			CodeExpression expression1;
			IEnumerator enumerator1;
			IDisposable disposable1;
			this.GenerateExpression(e.TargetObject);
			this.Output.Write("(");
			flag1 = true;
			enumerator1 = e.Indices.GetEnumerator();
			try
			{
				while (enumerator1.MoveNext())
				{
					expression1 = ((CodeExpression) enumerator1.Current);
					if (flag1)flag1 = false;
					else this.Output.Write(", ");
					this.GenerateExpression(expression1);
				}
 
			}
			finally
			{
				disposable1 = (enumerator1 as IDisposable);
				if (disposable1 != null)
				{
					disposable1.Dispose();
 
				}
 
			}
			this.Output.Write(")");
 
		} 

		protected override void GenerateAssignStatement(CodeAssignStatement e)
		{
			this.GenerateExpression(e.Left);
			this.Output.Write(" = ");
			this.GenerateExpression(e.Right);
			this.Output.WriteLine(string.Empty);
 
		} 

		protected override void GenerateAttachEventStatement(CodeAttachEventStatement e)
		{
			this.Output.Write("AddHandler ");
			this.GenerateFormalEventReferenceExpression(e.Event);
			this.Output.Write(", ");
			this.GenerateExpression(e.Listener);
			this.Output.WriteLine(string.Empty);
 
		} 

		protected override void GenerateAttributeDeclarationsEnd(CodeAttributeDeclarationCollection attributes)
		{
			this.Output.Write(">");
		}
		protected override void GenerateAttributeDeclarationsStart(CodeAttributeDeclarationCollection attributes)
		{
			this.Output.Write("<");
		} 

		protected override void GenerateBaseReferenceExpression(CodeBaseReferenceExpression e)
		{
			this.Output.Write("MyBase");
		} 

		protected override void GenerateBinaryOperatorExpression(CodeBinaryOperatorExpression e)
		{
			if (e.Operator != CodeBinaryOperatorType.IdentityInequality)
			{
				base.GenerateBinaryOperatorExpression(e);
				return; 
			}
			if (((e.Right as CodePrimitiveExpression) != null) && (((CodePrimitiveExpression) e.Right).Value == null))
			{
				this.GenerateNotIsNullExpression(e.Left);
				return; 
			}
			if (((e.Left as CodePrimitiveExpression) != null) && (((CodePrimitiveExpression) e.Left).Value == null))
			{
				this.GenerateNotIsNullExpression(e.Right);
				return; 
			}
			base.GenerateBinaryOperatorExpression(e);
 
		}
		protected override void GenerateCastExpression(CodeCastExpression e)
		{
			this.Output.Write("CType(");
			this.GenerateExpression(e.Expression);
			this.Output.Write(",");
			OutputType(e.TargetType);
			this.OutputArrayPostfix(e.TargetType);
			this.Output.Write(")");
 
		} 

		protected override void GenerateComment(CodeComment e)
		{
			this.Output.Write("'");
			this.Output.WriteLine(ConvertToCommentEscapeCodes(e.Text));
 
		} 

		protected override void GenerateCompileUnit(CodeCompileUnit e)
		{
			SortedList list1;
			CodeNamespace namespace1;
			CodeNamespaceImport import1;
			string text1;
			IEnumerator enumerator1;
			IEnumerator enumerator2;
			IDisposable disposable1;
			this.GenerateCompileUnitStart(e);
			list1 = new SortedList(new CaseInsensitiveComparer(CultureInfo.InvariantCulture));
			enumerator1 = e.Namespaces.GetEnumerator();
			try
			{
				while (enumerator1.MoveNext())
				{
					namespace1 = ((CodeNamespace) enumerator1.Current);
					namespace1.UserData["GenerateImports"] = 0;
					enumerator2 = namespace1.Imports.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							import1 = ((CodeNamespaceImport) enumerator2.Current);
							if (!list1.Contains(import1.Namespace))
							{
								list1.Add(import1.Namespace, import1.Namespace);
 
							}
 
						}
 
					}
					finally
					{
						disposable1 = (enumerator2 as IDisposable);
						if (disposable1 != null)
						{
							disposable1.Dispose();
 
						}
 
					}
 
				}
 
			}
			finally
			{
				disposable1 = (enumerator1 as IDisposable);
				if (disposable1 != null)
				{
					disposable1.Dispose();
 
				}
 
			}
			enumerator1 = list1.Keys.GetEnumerator();
			try
			{
				while (enumerator1.MoveNext())
				{
					text1 = ((string) enumerator1.Current);
					this.Output.Write("Imports ");
					this.OutputIdentifier(text1);
					this.Output.WriteLine(string.Empty);
 
				}
 
			}
			finally
			{
				disposable1 = (enumerator1 as IDisposable);
				if (disposable1 != null)
				{
					disposable1.Dispose();
 
				}
 
			}
			if (e.AssemblyCustomAttributes.Count > 0)
			{
				this.OutputAttributes(e.AssemblyCustomAttributes, false, "Assembly: ", true);
 
			}
			this.GenerateNamespaces(e);
			this.GenerateCompileUnitEnd(e);
 
		}

		protected override void GenerateCompileUnitStart(CodeCompileUnit e)
		{
			this.Output.WriteLine("'------------------------------------------------------------------------------");
			this.Output.WriteLine("' <autogenerated>");
			this.Output.WriteLine("' This code was generated by a tool.");
			this.Output.WriteLine(string.Concat("' Runtime Version: ", Environment.Version.ToString()));
			this.Output.WriteLine("'");
			this.Output.WriteLine("' Changes to this file may cause incorrect behavior and will be lost if ");
			this.Output.WriteLine("' the code is regenerated.");
			this.Output.WriteLine("' </autogenerated>");
			this.Output.WriteLine("'------------------------------------------------------------------------------");
			this.Output.WriteLine(string.Empty);
			if (this.AllowLateBound(e))
			{
				this.Output.WriteLine("Option Strict Off");
 
			}
			else
			{
				this.Output.WriteLine("Option Strict On");
 
			}
			if (!this.RequireVariableDeclaration(e))
			{
				this.Output.WriteLine("Option Explicit Off");
 
			}
			else
			{
				this.Output.WriteLine("Option Explicit On");
 
			}
			this.Output.WriteLine();
 
		}

		protected override void GenerateConditionStatement(CodeConditionStatement e)
		{
			CodeStatementCollection collection1;
			this.Output.Write("If ");
			this.GenerateExpression(e.Condition);
			this.Output.WriteLine(" Then");
			this.Indent = (this.Indent + 1);
			this.GenerateStatements(e.TrueStatements);
			this.Indent = (this.Indent - 1);
			collection1 = e.FalseStatements;
			if (collection1.Count > 0)
			{
				this.Output.Write("Else");
				this.Output.WriteLine(string.Empty);
				this.Indent = (this.Indent + 1);
				this.GenerateStatements(e.FalseStatements);
				this.Indent = (this.Indent - 1);
 
			}
			this.Output.WriteLine("End If");
 
		}

		protected override void GenerateConstructor(CodeConstructor e, CodeTypeDeclaration c)
		{
			CodeExpressionCollection collection1;
			CodeExpressionCollection collection2;
			if (!this.IsCurrentClass && !this.IsCurrentStruct)
			{
				return; 
			}
			if (e.CustomAttributes.Count > 0)
			{
				this.OutputAttributes(e.CustomAttributes, false);
 
			}
			this.OutputMemberAccessModifier(e.Attributes);
			this.Output.Write("Sub New(");
			this.OutputParameters(e.Parameters);
			this.Output.WriteLine(")");
			this.Indent = (this.Indent + 1);
			collection1 = e.BaseConstructorArgs;
			collection2 = e.ChainedConstructorArgs;
			if (collection2.Count > 0)
			{
				this.Output.Write("Me.New(");
				this.OutputExpressionList(collection2);
				this.Output.Write(")");
				this.Output.WriteLine(string.Empty);
 
			}
			else
			{
				if (collection1.Count > 0)
				{
					this.Output.Write("MyBase.New(");
					this.OutputExpressionList(collection1);
					this.Output.Write(")");
					this.Output.WriteLine(string.Empty);
					goto L_0114;
 
				}
				this.Output.WriteLine("MyBase.New");
 
			}
 
			L_0114:
				this.GenerateStatements(e.Statements);
			this.Indent = (this.Indent - 1);
			this.Output.WriteLine("End Sub");
 
		}

		protected override void GenerateDelegateCreateExpression(CodeDelegateCreateExpression e)
		{
			this.Output.Write("AddressOf ");
			this.GenerateExpression(e.TargetObject);
			this.Output.Write(".");
			this.OutputIdentifier(e.MethodName);
 
		} 

		protected override void GenerateDelegateInvokeExpression(CodeDelegateInvokeExpression e)
		{
			CodeExpressionCollection collection1;
			this.Output.Write("RaiseEvent ");
			if (e.TargetObject != null)
			{
				if ((e.TargetObject as CodeEventReferenceExpression) != null)
				{
					this.GenerateFormalEventReferenceExpression(((CodeEventReferenceExpression) e.TargetObject));
 
				}
				else
				{
					this.GenerateExpression(e.TargetObject);
 
				}
 
			}
			collection1 = e.Parameters;
			if (collection1.Count > 0)
			{
				this.Output.Write("(");
				this.OutputExpressionList(e.Parameters);
				this.Output.Write(")");
 
			}
 
		}
		protected override void GenerateDirectionExpression(CodeDirectionExpression e)
		{
			this.GenerateExpression(e.Expression);
		} 

		protected override void GenerateEntryPointMethod(CodeEntryPointMethod e, CodeTypeDeclaration c)
		{
			this.Output.WriteLine("Public Shared Sub Main()");
			this.Indent = (this.Indent + 1);
			this.GenerateStatements(e.Statements);
			this.Indent = (this.Indent - 1);
			this.Output.WriteLine("End Sub");
 
		}
		protected override void GenerateEvent(CodeMemberEvent e, CodeTypeDeclaration c)
		{
			if (this.IsCurrentDelegate || this.IsCurrentEnum)return; 
			if (e.CustomAttributes.Count > 0)this.OutputAttributes(e.CustomAttributes, false);
			this.OutputMemberAccessModifier(e.Attributes);
			this.Output.Write("Event ");
			this.OutputTypeNamePair(e.Type, e.Name);
			this.Output.WriteLine(string.Empty);
 
		} 

		protected override void GenerateEventReferenceExpression(CodeEventReferenceExpression e)
		{
			bool flag1;
			if (e.TargetObject != null)
			{
				flag1 = e.TargetObject is CodeThisReferenceExpression;
				this.GenerateExpression(e.TargetObject);
				this.Output.Write(".");
				if (flag1)
				{
					this.Output.Write(string.Concat(e.EventName, "Event"));
					return; 
				}
				this.Output.Write(e.EventName);
				return; 
			}
			this.OutputIdentifier(string.Concat(e.EventName, "Event"));
 
		}
		protected override void GenerateExpressionStatement(CodeExpressionStatement e)
		{
			this.GenerateExpression(e.Expression);
			this.Output.WriteLine(string.Empty);
		} 

		protected override void GenerateField(CodeMemberField e)
		{
			if (this.IsCurrentDelegate || this.IsCurrentInterface)return; 
			if (this.IsCurrentEnum)
			{
				if (e.CustomAttributes.Count > 0)this.OutputAttributes(e.CustomAttributes, false);
				this.OutputIdentifier(e.Name);
				if (e.InitExpression != null)
				{
					this.Output.Write(" = ");
					this.GenerateExpression(e.InitExpression);
 
				}
				this.Output.WriteLine(string.Empty);
				return; 
			}
			if (e.CustomAttributes.Count > 0)
			{
				this.OutputAttributes(e.CustomAttributes, false);
 
			}
			this.OutputMemberAccessModifier(e.Attributes);
			this.OutputVTableModifier(e.Attributes);
			this.OutputFieldScopeModifier(e.Attributes);
			this.OutputTypeNamePair(e.Type, e.Name);
			if (e.InitExpression != null)
			{
				this.Output.Write(" = ");
				this.GenerateExpression(e.InitExpression);
 
			}
			this.Output.WriteLine(string.Empty);
 
		}
		protected override void GenerateFieldReferenceExpression(CodeFieldReferenceExpression e)
		{
			if (e.TargetObject != null)
			{
				this.GenerateExpression(e.TargetObject);
				this.Output.Write(".");
				this.Output.Write(e.FieldName);
				return; 
			}
			this.OutputIdentifier(e.FieldName);
 
		} 

		protected override void GenerateGotoStatement(CodeGotoStatement e)
		{
			this.Output.Write("goto ");
			this.Output.WriteLine(e.Label);
		}
		protected override void GenerateIndexerExpression(CodeIndexerExpression e)
		{
			bool flag1;
			CodeExpression expression1;
			IEnumerator enumerator1;
			IDisposable disposable1;
			this.GenerateExpression(e.TargetObject);
			this.Output.Write("(");
			flag1 = true;
			enumerator1 = e.Indices.GetEnumerator();
			try
			{
				while (enumerator1.MoveNext())
				{
					expression1 = ((CodeExpression) enumerator1.Current);
					if (flag1)
					{
						flag1 = false;
 
					}
					else
					{
						this.Output.Write(", ");
 
					}
					this.GenerateExpression(expression1);
 
				}
 
			}
			finally
			{
				disposable1 = (enumerator1 as IDisposable);
				if (disposable1 != null)
				{
					disposable1.Dispose();
 
				}
 
			}
			this.Output.Write(")");
 
		}
		protected override void GenerateIterationStatement(CodeIterationStatement e)
		{
			this.GenerateStatement(e.InitStatement);
			this.Output.Write("Do While ");
			this.GenerateExpression(e.TestExpression);
			this.Output.WriteLine(string.Empty);
			this.Indent = (this.Indent + 1);
			this.GenerateStatements(e.Statements);
			this.GenerateStatement(e.IncrementStatement);
			this.Indent = (this.Indent - 1);
			this.Output.WriteLine("Loop");
 
		}
		protected override void GenerateLabeledStatement(CodeLabeledStatement e)
		{
			this.Indent = (this.Indent - 1);
			this.Output.Write(e.Label);
			this.Output.WriteLine(":");
			this.Indent = (this.Indent + 1);
			if (e.Statement != null)
			{
				this.GenerateStatement(e.Statement);
 
			}
 
		} 

		protected override void GenerateLinePragmaEnd(CodeLinePragma e)
		{
			this.Output.WriteLine(string.Empty);
			this.Output.WriteLine("#End ExternalSource");
 
		}
		//
		protected override void GenerateLinePragmaStart(CodeLinePragma e)
		{
			this.Output.WriteLine(string.Empty);
			this.Output.Write("#ExternalSource("+'"');
			this.Output.Write(e.FileName);
			this.Output.Write('"'+",");
			this.Output.Write(e.LineNumber);
			this.Output.WriteLine(")");
 
		}
		//
		protected override void GenerateMethod(CodeMemberMethod e, CodeTypeDeclaration c)
		{
			string text1;
			string text2;
			bool flag1;
			bool flag2;
			CodeTypeReference reference1;
			IEnumerator enumerator1;
			IDisposable disposable1;
			if ((!this.IsCurrentClass && !this.IsCurrentStruct) && !this.IsCurrentInterface)return; 
			if (e.CustomAttributes.Count > 0)this.OutputAttributes(e.CustomAttributes, false);
			text1 = e.Name;
			if (e.PrivateImplementationType != null)
			{
				text2 = e.PrivateImplementationType.BaseType;
				//text2 = text2.Replace(46, 95);
				text2 = text2.Replace('.', '_');
				e.Name = text2 + "_" + e.Name;
 
			}
			if (!this.IsCurrentInterface)
			{
				if (e.PrivateImplementationType == null)
				{
					this.OutputMemberAccessModifier(e.Attributes);
					if (c != null && this.MethodIsOverloaded(e, c))this.Output.Write("Overloads ");
				}
				this.OutputVTableModifier(e.Attributes);
				this.OutputMemberScopeModifier(e.Attributes);
 
			}
			else this.OutputVTableModifier(e.Attributes);

			flag1 = false;
			if ((e.ReturnType.BaseType.Length == 0) 
				||(string.Compare(e.ReturnType.BaseType, typeof(void).FullName, true, 
				CultureInfo.InvariantCulture) == 0))
			{
				flag1 = true;
 
			}
			if (flag1)this.Output.Write("Sub ");
			else this.Output.Write("Function ");
 
			this.OutputIdentifier(e.Name);
			this.Output.Write("(");
			this.OutputParameters(e.Parameters);
			this.Output.Write(")");
			if (!flag1)
			{
				this.Output.Write(" As ");
				if (e.ReturnTypeCustomAttributes.Count > 0)
				{
					this.OutputAttributes(e.ReturnTypeCustomAttributes, true);
 
				}
				this.OutputType(e.ReturnType);
				this.OutputArrayPostfix(e.ReturnType);
 
			}
			if (e.ImplementationTypes.Count > 0)
			{
				this.Output.Write(" Implements ");
				flag2 = true;
				enumerator1 = e.ImplementationTypes.GetEnumerator();
				try
				{
					while (enumerator1.MoveNext())
					{
						reference1 = ((CodeTypeReference) enumerator1.Current);
						if (flag2)
						{
							flag2 = false;
 
						}
						else
						{
							this.Output.Write(" , ");
 
						}
						this.OutputType(reference1);
						this.Output.Write(".");
						this.OutputIdentifier(text1);
 
					}
					goto L_027E;
 
				}
				finally
				{
					disposable1 = (enumerator1 as IDisposable);
					if (disposable1 != null)
					{
						disposable1.Dispose();
 
					}
 
				}
 
			}
			if (e.PrivateImplementationType != null)
			{
				this.Output.Write(" Implements ");
				this.OutputType(e.PrivateImplementationType);
				this.Output.Write(".");
				this.OutputIdentifier(text1);
 
			}
 
			L_027E:
				this.Output.WriteLine(string.Empty);

			if (!this.IsCurrentInterface && ((e.Attributes & MemberAttributes.ScopeMask) != MemberAttributes.Abstract))
			{
				this.Indent = (this.Indent + 1);
				this.GenerateStatements(e.Statements);
				this.Indent = (this.Indent - 1);
				if (flag1)
				{
					this.Output.WriteLine("End Sub");
					goto L_02EF;
 
				}
				this.Output.WriteLine("End Function");
 
			}
 
			L_02EF:
				e.Name = text1;
 
		} 

		protected override void GenerateMethodInvokeExpression(CodeMethodInvokeExpression e)
		{
			CodeExpressionCollection collection1;
			GenerateMethodReferenceExpression(e.Method);
			collection1 = e.Parameters;
			if (collection1.Count > 0)
			{
				this.Output.Write("(");
				this.OutputExpressionList(e.Parameters);
				this.Output.Write(")");
 
			}
 
		} 

		protected override void GenerateMethodReferenceExpression(CodeMethodReferenceExpression e)
		{
			if (e.TargetObject != null)
			{
				this.GenerateExpression(e.TargetObject);
				this.Output.Write(".");
				this.Output.Write(e.MethodName);
				return; 
			}
			this.OutputIdentifier(e.MethodName);
 
		} 

		protected override void GenerateMethodReturnStatement(CodeMethodReturnStatement e)
		{
			if (e.Expression != null)
			{
				this.Output.Write("Return ");
				this.GenerateExpression(e.Expression);
				this.Output.WriteLine(string.Empty);
				return; 
			}
			this.Output.WriteLine("Return");
 
		} 

		//
		protected override void GenerateNamespace(CodeNamespace e)
		{
			if (this.GetUserData(e, "GenerateImports", true))this.GenerateNamespaceImports(e);
			this.Output.WriteLine();
			//GenerateCommentStatements(e.Comments);
			GenerateNamespaceStart(e);
			this.GenerateTypes(e);
			GenerateNamespaceEnd(e);
 
		} 

		protected override void GenerateNamespaceEnd(CodeNamespace e)
		{
			if (!string.IsNullOrEmpty(e.Name))
			{
				this.Indent = (this.Indent - 1);
				this.Output.WriteLine("End Namespace");
			}
		} 

		protected override void GenerateNamespaceImport(CodeNamespaceImport e)
		{
			this.Output.Write("Imports ");
			this.OutputIdentifier(e.Namespace);
			this.Output.WriteLine(string.Empty);
		}

		protected override void GenerateNamespaceStart(CodeNamespace e)
		{
			if (!string.IsNullOrEmpty(e.Name))
			{
				this.Output.Write("Namespace ");
				this.OutputIdentifier(e.Name);
				this.Output.WriteLine();
				this.Indent = (this.Indent + 1);
			}
		} 

		protected override void GenerateObjectCreateExpression(CodeObjectCreateExpression e)
		{
			CodeExpressionCollection collection1;
			this.Output.Write("New ");
			OutputType(e.CreateType);
			collection1 = e.Parameters;
			if (collection1.Count > 0)
			{
				this.Output.Write("(");
				this.OutputExpressionList(collection1);
				this.Output.Write(")");
 
			}
 
		} 

		protected override void GenerateParameterDeclarationExpression(CodeParameterDeclarationExpression e)
		{
			if (e.CustomAttributes.Count > 0) this.OutputAttributes(e.CustomAttributes, true);
			this.OutputDirection(e.Direction);
			this.OutputTypeNamePair(e.Type, e.Name);
 
		} 

		protected override void GeneratePrimitiveExpression(CodePrimitiveExpression e)
		{
			int num1;
			if (e.Value is char)
			{
				num1 = ((IConvertible) e.Value).ToInt32(null);
				this.Output.Write(string.Concat("Microsoft.VisualBasic.ChrW(", num1.ToString(), ")"));
				return; 
			}
			base.GeneratePrimitiveExpression(e);
 
		} 

		//
		protected override void GenerateProperty(CodeMemberProperty e, CodeTypeDeclaration c)
		{
			string text1;
			string text2;
			bool flag1;
			CodeTypeReference reference1;
			IEnumerator enumerator1;
			IDisposable disposable1;
			if ((!this.IsCurrentClass && !this.IsCurrentStruct) && !this.IsCurrentInterface)return; 
			if (e.CustomAttributes.Count > 0)this.OutputAttributes(e.CustomAttributes, false);
			text1 = e.Name;
			if (e.PrivateImplementationType != null)
			{
				text2 = e.PrivateImplementationType.BaseType;
				//text2 = text2.Replace(46, 95);
				text2 = text2.Replace('.', '_');
				e.Name = string.Concat(text2, "_", e.Name);
 
			}
			if (!this.IsCurrentInterface)
			{
				if (e.PrivateImplementationType == null)
				{
					this.OutputMemberAccessModifier(e.Attributes);
					if (this.PropertyIsOverloaded(e, c))
					{
						this.Output.Write("Overloads ");
 
					}
 
				}
				this.OutputVTableModifier(e.Attributes);
				this.OutputMemberScopeModifier(e.Attributes);
 
			}
			else
			{
				this.OutputVTableModifier(e.Attributes);
 
			}
			if ((e.Parameters.Count > 0) && (string.Compare(e.Name, "Item", true, CultureInfo.InvariantCulture) == 0))
			{
				this.Output.Write("Default ");
 
			}
			if (e.HasGet)
			{
				if (e.HasSet)
				{
  
				}
				else
				{
					this.Output.Write("ReadOnly ");
				}
 
			}
			else
			{
				if (e.HasSet)
				{
					this.Output.Write("WriteOnly ");
				}
			}
			this.Output.Write("Property ");
			this.OutputIdentifier(e.Name);
			if (e.Parameters.Count > 0)
			{
				this.Output.Write("(");
				this.OutputParameters(e.Parameters);
				this.Output.Write(")");
 
			}
			this.Output.Write(" As ");
			OutputType(e.Type);
			this.OutputArrayPostfix(e.Type);
			if (e.ImplementationTypes.Count > 0)
			{
				this.Output.Write(" Implements ");
				flag1 = true;
				enumerator1 = e.ImplementationTypes.GetEnumerator();
				try
				{
					while (enumerator1.MoveNext())
					{
						reference1 = ((CodeTypeReference) enumerator1.Current);
						if (flag1)
						{
							flag1 = false;
 
						}
						else
						{
							this.Output.Write(" , ");
 
						}
						OutputType(reference1);
						this.Output.Write(".");
						this.OutputIdentifier(text1);
 
					}
					goto L_028A;
 
				}
				finally
				{
					disposable1 = (enumerator1 as IDisposable);
					if (disposable1 != null)
					{
						disposable1.Dispose();
 
					}
 
				}
 
			}
			if (e.PrivateImplementationType != null)
			{
				this.Output.Write(" Implements ");
				OutputType(e.PrivateImplementationType);
				this.Output.Write(".");
				this.OutputIdentifier(text1);
 
			}
 
			L_028A:
				this.Output.WriteLine("");
			if (!c.IsInterface)
			{
				this.Indent = (this.Indent + 1);
				if (e.HasGet)
				{
					this.Output.WriteLine("Get");
					if (!this.IsCurrentInterface && ((e.Attributes & MemberAttributes.ScopeMask) != MemberAttributes.Abstract))
					{
						this.Indent = (this.Indent + 1);
						this.GenerateStatements(e.GetStatements);
						e.Name = text1;
						this.Indent = (this.Indent - 1);
						this.Output.WriteLine("End Get");
 
					}
 
				}
				if (e.HasSet)
				{
					this.Output.WriteLine("Set");
					if (!this.IsCurrentInterface && ((e.Attributes & MemberAttributes.ScopeMask) != MemberAttributes.Abstract))
					{
						this.Indent = (this.Indent + 1);
						this.GenerateStatements(e.SetStatements);
						this.Indent = (this.Indent - 1);
						this.Output.WriteLine("End Set");
 
					}
 
				}
				this.Indent = (this.Indent - 1);
				this.Output.WriteLine("End Property");
 
			}
			e.Name = text1;
 
		}
		protected override void GeneratePropertyReferenceExpression(CodePropertyReferenceExpression e)
		{
			if (e.TargetObject != null)
			{
				this.GenerateExpression(e.TargetObject);
				this.Output.Write(".");
				this.Output.Write(e.PropertyName);
				return; 
			}
			this.OutputIdentifier(e.PropertyName);
 
		} 

		protected override void GeneratePropertySetValueReferenceExpression(CodePropertySetValueReferenceExpression e)
		{
			this.Output.Write("value");
		}

		protected override void GenerateRemoveEventStatement(CodeRemoveEventStatement e)
		{
			this.Output.Write("RemoveHandler ");
			this.GenerateFormalEventReferenceExpression(e.Event);
			this.Output.Write(", ");
			this.GenerateExpression(e.Listener);
			this.Output.WriteLine("");
 
		}

		protected override void GenerateSingleFloatValue(float s)
		{
			this.Output.Write(s.ToString(CultureInfo.InvariantCulture));
			this.Output.Write('!');
		}
		protected override void GenerateSnippetExpression(CodeSnippetExpression e)
		{
			this.Output.Write(e.Value);
		} 

		protected override void GenerateSnippetMember(CodeSnippetTypeMember e)
		{
			this.Output.Write(e.Text);
 
		} 

		protected override void GenerateSnippetStatement(CodeSnippetStatement e)
		{
			this.Output.WriteLine(e.Value);
 
		} 

		protected override void GenerateThisReferenceExpression(CodeThisReferenceExpression e)
		{
			this.Output.Write("Me");
 
		} 

		protected override void GenerateThrowExceptionStatement(CodeThrowExceptionStatement e)
		{
			this.Output.Write("Throw");
			if (e.ToThrow != null)
			{
				this.Output.Write(" ");
				this.GenerateExpression(e.ToThrow);
 
			}
			this.Output.WriteLine("");
 
		}
		protected override void GenerateTryCatchFinallyStatement(CodeTryCatchFinallyStatement e)
		{
			CodeCatchClauseCollection collection1;
			IEnumerator enumerator1;
			CodeCatchClause clause1;
			CodeStatementCollection collection2;
			this.Output.WriteLine("Try ");
			this.Indent = (this.Indent + 1);
			this.GenerateStatements(e.TryStatements);
			this.Indent = (this.Indent - 1);
			collection1 = e.CatchClauses;
			if (collection1.Count > 0)
			{
				enumerator1 = collection1.GetEnumerator();
				while (enumerator1.MoveNext())
				{
					clause1 = ((CodeCatchClause) enumerator1.Current);
					this.Output.Write("Catch ");
					this.OutputTypeNamePair(clause1.CatchExceptionType, clause1.LocalName);
					this.Output.WriteLine("");
					this.Indent = (this.Indent + 1);
					this.GenerateStatements(clause1.Statements);
					this.Indent = (this.Indent - 1);
 
				}
 
			}
			collection2 = e.FinallyStatements;
			if (collection2.Count > 0)
			{
				this.Output.WriteLine("Finally");
				this.Indent = (this.Indent + 1);
				this.GenerateStatements(collection2);
				this.Indent = (this.Indent - 1);
 
			}
			this.Output.WriteLine("End Try");
 
		} 

		protected override void GenerateTypeConstructor(CodeTypeConstructor e)
		{
			if (!this.IsCurrentClass && !this.IsCurrentStruct)return; 
			this.Output.WriteLine("Shared Sub New()");
			this.Indent = (this.Indent + 1);
			this.GenerateStatements(e.Statements);
			this.Indent = (this.Indent - 1);
			this.Output.WriteLine("End Sub");
		} 

		protected override void GenerateTypeEnd(CodeTypeDeclaration e)
		{
			string text1;
			if (this.IsCurrentDelegate)
			{
				goto L_0058;
 
			}
			this.Indent = (this.Indent - 1);
			if (e.IsEnum)
			{
				text1 = "End Enum";
 
			}
			else
			{
				if (e.IsInterface)
				{
					text1 = "End Interface";
					goto L_004C;
				}
				if (e.IsStruct)
				{
					text1 = "End Structure";
					goto L_004C;
 
				}
				text1 = "End Class";
			}
 
			L_004C:
				this.Output.WriteLine(text1);
 
			L_0058:
				return; 
		} 

		protected override void GenerateTypeOfExpression(CodeTypeOfExpression e)
		{
			this.Output.Write("GetType(");
			this.Output.Write(e.Type.BaseType);
			this.OutputArrayPostfix(e.Type);
			this.Output.Write(")");
 
		} 

		protected override void GenerateTypeStart(CodeTypeDeclaration e)
		{
			CodeTypeDelegate delegate1;
			bool flag1;
			bool flag2;
			CodeTypeReference reference1;
			TypeAttributes attributes1;
			IEnumerator enumerator1;
			IDisposable disposable1;
			if (!this.IsCurrentDelegate)
			{
				goto L_014B;
 
			}
			if (e.CustomAttributes.Count > 0)
			{
				this.OutputAttributes(e.CustomAttributes, false);
 
			}
			attributes1 = (e.TypeAttributes & TypeAttributes.NestedFamANDAssem);
			switch (attributes1)
			{
				case TypeAttributes.AnsiClass:
				{
					goto L_0051;
 
				}
				case TypeAttributes.Public:
				{
					goto L_0041;
 
				}
 
			}
			goto L_0051;
 
			L_0041:
				this.Output.Write("Public ");
 
			L_0051:
				delegate1 = ((CodeTypeDelegate) e);
			if ((delegate1.ReturnType.BaseType.Length > 0) && (string.Compare(delegate1.ReturnType.BaseType, "System.Void", true, CultureInfo.InvariantCulture) != 0))
			{
				this.Output.Write("Delegate Function ");
 
			}
			else
			{
				this.Output.Write("Delegate Sub ");
 
			}
			this.OutputIdentifier(e.Name);
			this.Output.Write("(");
			this.OutputParameters(delegate1.Parameters);
			this.Output.Write(")");
			if ((delegate1.ReturnType.BaseType.Length > 0) && (string.Compare(delegate1.ReturnType.BaseType, "System.Void", true, CultureInfo.InvariantCulture) != 0))
			{
				this.Output.Write(" As ");
				OutputType(delegate1.ReturnType);
				this.OutputArrayPostfix(delegate1.ReturnType);
 
			}
			this.Output.WriteLine("");
			return; 
			L_014B:
				if (e.IsEnum)
				{
					if (e.CustomAttributes.Count > 0)
					{
						OutputAttributes(e.CustomAttributes, false);
 
					}
					this.OutputTypeAttributes(e);
					this.OutputIdentifier(e.Name);
					if (e.BaseTypes.Count > 0)
					{
						this.Output.Write(" As ");
						OutputType(e.BaseTypes[0]);
 
					}
					this.Output.WriteLine("");
					this.Indent = (this.Indent + 1);
					return; 
				}
			if (e.CustomAttributes.Count > 0)
			{
				OutputAttributes(e.CustomAttributes, false);
 
			}
			this.OutputTypeAttributes(e);
			this.OutputIdentifier(e.Name);
			flag1 = false;
			flag2 = false;
			if (e.IsStruct)
			{
				flag1 = true;
 
			}
			if (e.IsInterface)
			{
				flag2 = true;
 
			}
			this.Indent = (this.Indent + 1);
			enumerator1 = e.BaseTypes.GetEnumerator();
			try
			{
				goto L_02A5;
 
			L_0233:
				reference1 = ((CodeTypeReference) enumerator1.Current);
				if (!flag1)
				{
					this.Output.WriteLine("");
					this.Output.Write("Inherits ");
					flag1 = true;
 
				}
				else
				{
					if (!flag2)
					{
						this.Output.WriteLine("");
						this.Output.Write("Implements ");
						flag2 = true;
						goto L_029E;
 
					}
					this.Output.Write(", ");
 
				}
 
			L_029E:
				OutputType(reference1);
 
			L_02A5:
				if (enumerator1.MoveNext())
				{
					goto L_0233;
 
				}
 
			}
			finally
			{
				disposable1 = (enumerator1 as IDisposable);
				if (disposable1 != null)
				{
					disposable1.Dispose();
 
				}
 
			}
			this.Output.WriteLine("");
 
		} 

		protected override void GenerateVariableDeclarationStatement(CodeVariableDeclarationStatement e)
		{
			this.Output.Write("Dim ");
			this.OutputTypeNamePair(e.Type, e.Name);
			if (e.InitExpression != null)
			{
				this.Output.Write(" = ");
				this.GenerateExpression(e.InitExpression);
 
			}
			this.Output.WriteLine("");
 
		} 

		protected override void GenerateVariableReferenceExpression(CodeVariableReferenceExpression e)
		{
			this.OutputIdentifier(e.VariableName);
		} 
		
	
		private void GenerateFormalEventReferenceExpression(CodeEventReferenceExpression e)
		{
			if ((e.TargetObject != null) && ((e.TargetObject as CodeThisReferenceExpression) == null))
			{
				this.GenerateExpression(e.TargetObject);
				this.Output.Write(".");
			}
			this.OutputIdentifier(e.EventName);
 
		} 
		
		private string GetArrayPostfix(CodeTypeReference typeRef)
		{
			string text1;
			char[] array1;
			int num1;
			text1 = "";
			if (typeRef.ArrayElementType != null)text1 = this.GetArrayPostfix(typeRef.ArrayElementType);
			if (typeRef.ArrayRank > 0)
			{
				array1 = new char[((uint) (typeRef.ArrayRank + 1))];
				array1[0] = '(';
				array1[typeRef.ArrayRank] = ')';
				for (num1 = 1; (num1 < typeRef.ArrayRank); num1 = (num1 + 1))array1[num1] = ',';
				text1 = string.Concat(text1, new string(array1));
			}
			return text1; 
		}
		private void GenerateNotIsNullExpression(CodeExpression e)
		{
			this.Output.Write("(Not (");
			this.GenerateExpression(e);
			this.Output.Write(") Is ");
			this.Output.Write(this.NullToken);
			this.Output.Write(")");
 
		}
		//
		public override string GetBaseTypeOutput(string thisType)
		{
			if (thisType.Length == 0)				return "Void";
			else if (thisType == "System.Byte")		return "Byte";
			else if (thisType == "System.Int16")	return "Short";
			else if (thisType == "System.Int32")	return "Integer";
			else if (thisType == "System.Int64")	return "Long";
			else if (thisType == "System.string")	return "String";
			else if (thisType == "System.DateTime")	return "Date";
			else if (thisType == "System.Decimal")	return "Decimal";
			else if (thisType == "System.Single")	return "Single";
			else if (thisType == "System.Double")	return "Double";
			else if (thisType == "System.Boolean")	return "Boolean";
			else if (thisType == "System.Char")		return "Char";
			else if (thisType == "System.Object")	return "Object";

			thisType = thisType.Replace('+', '.');
			thisType = CreateEscapedIdentifier(thisType);
			return thisType; 
		}

		protected override string GetTypeOutput(CodeTypeReference typeRef)
		{
			string text1;
			text1 = this.GetBaseTypeOutput(typeRef.BaseType);
			if (typeRef.ArrayRank > 0)text1 = string.Concat(text1, this.GetArrayPostfix(typeRef));
			return text1; 
		}
		private bool GetUserData(CodeObject e, string property, bool defaultValue)
		{
			object obj1;
			obj1 = e.UserData[property];
			if ((obj1 != null) && (bool)obj1)
			{
				return ((bool) obj1); 
			}
			return defaultValue; 
		}

		public static bool IsKeywordExist(string value) 
		{
			return keywordsHashtable[value] != null;
		}

		
		public override bool IsKeyword(string value) 
		{
			return IsKeywordExist(value);
		}

		protected override bool IsValidIdentifier(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return false; 
			}
			if ((value[0] != 91) || (value[(value.Length - 1)] != 93))
			{
				if (!IsKeyword(value))
				{
					goto L_0045;
 
				}
				return false; 
			}
			value = value.Substring(1, (value.Length - 2));
 
			L_0045:
				return CodeGenerator.IsValidLanguageIndependentIdentifier(value); 
		}
		private bool MethodIsOverloaded(CodeMemberMethod e, CodeTypeDeclaration c)
		{
			IEnumerator enumerator1;
			CodeMemberMethod method1;
			if ((e.Attributes & MemberAttributes.Overloaded) != 0)
			{
				return true; 
			}
			enumerator1 = c.Members.GetEnumerator();
			while (enumerator1.MoveNext())
			{
				if ((enumerator1.Current as CodeMemberMethod) != null)
				{
					method1 = ((CodeMemberMethod) enumerator1.Current);
					if (((((enumerator1.Current as CodeTypeConstructor) == null) && ((enumerator1.Current as CodeConstructor) == null)) && ((method1 != e) && method1.Name.Equals(e.Name))) && (method1.PrivateImplementationType == null))
					{
						return true; 
					}
 
				}
 
			}
			return false; 
		} 

		
		private void OutputArrayPostfix(CodeTypeReference typeRef)
		{
			if (typeRef.ArrayRank > 0)
			{
				this.Output.Write(this.GetArrayPostfix(typeRef));
 
			}
 
		} 

		private void OutputAttributes(CodeAttributeDeclarationCollection attributes, bool inLine)
		{
			this.OutputAttributes(attributes, inLine, null, false);
 
		} 

		private void OutputAttributes(CodeAttributeDeclarationCollection attributes, bool inLine, string prefix, bool closingLine)
		{
			IEnumerator enumerator1;
			bool flag1;
			CodeAttributeDeclaration declaration1;
			bool flag2;
			CodeAttributeArgument argument1;
			IEnumerator enumerator2;
			IDisposable disposable1;
			if (attributes.Count == 0)
			{
				return; 
			}
			enumerator1 = attributes.GetEnumerator();
			flag1 = true;
			GenerateAttributeDeclarationsStart(attributes);
			while (enumerator1.MoveNext())
			{
				if (flag1)
				{
					flag1 = false;
 
				}
				else
				{
					this.Output.Write(", ");
					if (!inLine)
					{
						this.ContinueOnNewLine("");
						this.Output.Write(" ");
 
					}
 
				}
				if (!string.IsNullOrEmpty(prefix))
				{
					this.Output.Write(prefix);
 
				}
				declaration1 = ((CodeAttributeDeclaration) enumerator1.Current);
				this.Output.Write(this.GetBaseTypeOutput(declaration1.Name));
				this.Output.Write("(");
				flag2 = true;
				enumerator2 = declaration1.Arguments.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						argument1 = ((CodeAttributeArgument) enumerator2.Current);
						if (flag2)
						{
							flag2 = false;
 
						}
						else
						{
							this.Output.Write(", ");
 
						}
						this.OutputAttributeArgument(argument1);
 
					}
 
				}
				finally
				{
					disposable1 = (enumerator2 as IDisposable);
					if (disposable1 != null)
					{
						disposable1.Dispose();
 
					}
 
				}
				this.Output.Write(")");
 
			}
			GenerateAttributeDeclarationsEnd(attributes);
			this.Output.Write(" ");
			if (!inLine)
			{
				if (closingLine)
				{
					this.Output.WriteLine();
					return; 
				}
				this.ContinueOnNewLine("");
 
			}
 
		} 

		private void OutputTypeAttributes(CodeTypeDeclaration e)
		{
			TypeAttributes attributes1 = e.TypeAttributes;
			TypeAttributes attributes2 = (attributes1 & TypeAttributes.NestedFamORAssem);

			switch(attributes2)
			{
				case TypeAttributes.Public:
				{
					goto L_0021;
 
				}
				case TypeAttributes.NestedPublic:
				{
					goto L_0021;
 
				}
				case TypeAttributes.NestedPrivate:
				{
					goto L_0033;
				}
 
			}
			goto L_0043;
 
			L_0021:
				this.Output.Write("Public ");
			goto L_0043;
 
			L_0033:
				this.Output.Write("Private ");
 
			L_0043:
				if (e.IsStruct)
				{
					this.Output.Write("Structure ");
					return; 
				}
			if (e.IsEnum)
			{
				this.Output.Write("Enum ");
				return; 
			}
			attributes2 = (attributes1 & TypeAttributes.Interface);
			if (attributes2 != 0)
			{
				if (attributes2 == TypeAttributes.Interface)
				{
					goto L_00D1;
 
				}
				return; 
			}
			if ((attributes1 & TypeAttributes.Sealed) == TypeAttributes.Sealed)
			{
				this.Output.Write("NotInheritable ");
 
			}
			if ((attributes1 & TypeAttributes.Abstract) == TypeAttributes.Abstract)
			{
				this.Output.Write("MustInherit ");
 
			}
			this.Output.Write("Class ");
			return; 
			L_00D1:
				this.Output.Write("Interface ");
 
		}
		private void OutputVTableModifier(MemberAttributes attributes)
		{
			MemberAttributes attributes1 = (attributes & MemberAttributes.VTableMask);
			if (attributes1 != MemberAttributes.New)
			{
				return; 
			}
			this.Output.Write("Shadows ");
 
		} 

		
		protected override void OutputAttributeArgument(CodeAttributeArgument arg)
		{
			if (!string.IsNullOrEmpty(arg.Name))
			{
				this.OutputIdentifier(arg.Name);
				this.Output.Write(":=");
 
			}
			this.GenerateCodeFromExpression(arg.Value, ((IndentedTextWriter) this.Output).InnerWriter, this.Options);
 
		} 

		protected override void OutputDirection(FieldDirection dir)
		{
			FieldDirection direction1;
			direction1 = dir;
			switch (direction1)
			{
				case FieldDirection.In:
				{
					this.Output.Write("ByVal ");
					return; 
				}
				case FieldDirection.Out:
				{
					goto L_0026;
 
				}
				case FieldDirection.Ref:
				{
					goto L_0026;
 
				}
 
			}
			return; 
			L_0026:
				this.Output.Write("ByRef ");
 
		} 

		protected override void OutputFieldScopeModifier(MemberAttributes attributes)
		{
			MemberAttributes attributes1 = (attributes & MemberAttributes.ScopeMask);
			switch(attributes1)
			{
				case MemberAttributes.Final:
				{
					this.Output.Write("");
					return; 
				}
				case MemberAttributes.Static:
				{
					this.Output.Write("Shared ");
					return; 
				}
				case MemberAttributes.Override:
				{
					goto L_0052;
 
				}
				case MemberAttributes.Const:
				{
					this.Output.Write("Const ");
					return; 
				}
 
			}
 
			L_0052:
				this.Output.Write("");
 
		}
		protected override void OutputIdentifier(string ident)
		{
			this.Output.Write(CreateEscapedIdentifier(ident));
 
		}
		protected override void OutputMemberAccessModifier(MemberAttributes attributes)
		{
			MemberAttributes attributes1 = (attributes & MemberAttributes.AccessMask);
			if ((int)attributes1 <= 12288)
			{
				if (attributes1 == MemberAttributes.Assembly)
				{
					this.Output.Write("Friend ");
					return; 
				}
				if (attributes1 == MemberAttributes.FamilyAndAssembly)
				{
					this.Output.Write("Friend ");
					return;
 
				}
				if (attributes1 == MemberAttributes.Family)
				{
					this.Output.Write("Protected ");
					return; 
 
				}
				return; 
			}

			if (attributes1 == MemberAttributes.FamilyOrAssembly)
			{
				this.Output.Write("Protected ");
				return; 
			}
			if (attributes1 == MemberAttributes.Private)
			{
				this.Output.Write("Private ");
				return; 
 
			}
			if (attributes1 == MemberAttributes.Public)
			{
				this.Output.Write("Public ");
 
			}
			return; 
		}

		protected override void OutputMemberScopeModifier(MemberAttributes attributes)
		{
			MemberAttributes attributes1 = (attributes & MemberAttributes.ScopeMask);
			switch(attributes1)
			{
				case MemberAttributes.Abstract:
				{
					this.Output.Write("MustOverride ");
					return; 
				}
				case MemberAttributes.Final:
				{
					this.Output.Write("");
					return; 
				}
				case MemberAttributes.Static:
				{
					this.Output.Write("Shared ");
					return; 
				}
				case MemberAttributes.Override:
				{
					this.Output.Write("Overrides ");
					return; 
				}
			}
			if (attributes1 == MemberAttributes.Private)
			{
				this.Output.Write("Private ");
				return; 
			}
			
			attributes1 = (attributes & MemberAttributes.AccessMask);
			if ((attributes1 != MemberAttributes.Family) && (attributes1 != MemberAttributes.Public))
			{
				return; 
			}
			this.Output.Write("Overridable ");
 
		} 

		protected override void OutputOperator(CodeBinaryOperatorType op)
		{
			CodeBinaryOperatorType type1 = op;
			switch(type1)
			{
				case CodeBinaryOperatorType.Modulus:
				{
					this.Output.Write("Mod");
					return; 
				}
				case CodeBinaryOperatorType.Assign:
				{
					this.OutputOperator(op);
					return;
				}
				case CodeBinaryOperatorType.IdentityInequality:
				{
					this.Output.Write("<>");
					return; 
				}
				case CodeBinaryOperatorType.IdentityEquality:
				{
					this.Output.Write("Is");
					return; 
				}
				case CodeBinaryOperatorType.ValueEquality:
				{
					this.Output.Write("=");
					return; 
				}
				case CodeBinaryOperatorType.BitwiseOr:
				{
					this.Output.Write("Or");
					return; 
				}
				case CodeBinaryOperatorType.BitwiseAnd:
				{
					this.Output.Write("And");
					return; 
				}
				case CodeBinaryOperatorType.BooleanOr:
				{
					this.Output.Write("OrElse");
					return; 
				}
				case CodeBinaryOperatorType.BooleanAnd:
				{
					this.Output.Write("AndAlso");
					return; 
				}
			}
			base.OutputOperator(op);
 
		}
		protected override void OutputType(CodeTypeReference typeRef)
		{
			this.Output.Write(this.GetBaseTypeOutput(typeRef.BaseType));
		}
		protected override void OutputTypeNamePair(CodeTypeReference typeRef, string name)
		{
			this.OutputIdentifier(name);
			this.OutputArrayPostfix(typeRef);
			this.Output.Write(" As ");
			OutputType(typeRef);
 
		} 

		

		private bool PropertyIsOverloaded(CodeMemberProperty e, CodeTypeDeclaration c)
		{
			IEnumerator enumerator1;
			CodeMemberProperty property1;
			if ((e.Attributes & MemberAttributes.Overloaded) != 0)
			{
				return true; 
			}
			enumerator1 = c.Members.GetEnumerator();
			while (enumerator1.MoveNext())
			{
				if ((enumerator1.Current as CodeMemberProperty) != null)
				{
					property1 = ((CodeMemberProperty) enumerator1.Current);
					if (((property1 != e) && property1.Name.Equals(e.Name)) && (property1.PrivateImplementationType == null))
					{
						return true; 
					}
 
				}
 
			}
			return false; 
		} 

		protected bool RequireVariableDeclaration(CodeCompileUnit e)
		{
			object obj1;
			obj1 = e.UserData["RequireVariableDeclaration"];
			if ((obj1 != null) && (bool)obj1)
			{
				return ((bool) obj1); 
			}
			return true; 
		} 

		protected override bool Supports(GeneratorSupport support)
		{
			
			return (((int)support & 2097151) == (int)support); 
		}

		protected override string NullToken 
		{ 
			get
			{
				return "Nothing"; 

			}
		}

		public override string QuoteSnippetString(string value) 
		{
			StringBuilder builder1 = new StringBuilder(value.Length + 5);
			bool flag1 = true;
			builder1.Append("\"");
			for (int num1 = 0; num1 < value.Length; num1++)
			{
				char ch1 = value[num1];
				char ch2 = ch1;
				if (ch2 <= '"')
				{
					if (ch2 == '\0')
					{
						goto Label_0111;
					}
					switch (ch2)
					{
						case '\t':
						{
							this.EnsureNotInDoubleQuotes(ref flag1, builder1);
							builder1.Append("&Microsoft.VisualBasic.ChrW(9)");
							goto Label_0168;
						}
						case '\n':
						{
							this.EnsureNotInDoubleQuotes(ref flag1, builder1);
							builder1.Append("&Microsoft.VisualBasic.ChrW(10)");
							goto Label_0168;
						}
						case '\v':
						case '\f':
						{
							goto Label_0151;
						}
						case '\r':
						{
							this.EnsureNotInDoubleQuotes(ref flag1, builder1);
							if ((num1 >= (value.Length - 1)) || (value[num1 + 1] != '\n'))
							{
								goto Label_00EC;
							}
							builder1.Append("&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)");
							num1++;
							goto Label_0168;
						}
						case '"':
						{
							goto Label_009B;
						}
					}
					goto Label_0151;
				}
				switch (ch2)
				{
					case '\u201c':
					case '\u201d':
					{
						goto Label_009B;
					}
					case '\u2028':
					case '\u2029':
					{
						this.EnsureNotInDoubleQuotes(ref flag1, builder1);
						AppendEscapedChar(builder1, ch1);
						goto Label_0168; 
					}
 
					default:
					{
						if (ch2 != 0xff02)
						{
							goto Label_0151;
						}
						goto Label_009B;
					}
				}
			Label_009B:
				this.EnsureInDoubleQuotes(ref flag1, builder1);
				builder1.Append(ch1);
				builder1.Append(ch1);
				goto Label_0168;
			Label_00EC:
				builder1.Append("&Microsoft.VisualBasic.ChrW(13)");
				goto Label_0168;
			Label_0111:
				this.EnsureNotInDoubleQuotes(ref flag1, builder1);
				builder1.Append("&Microsoft.VisualBasic.ChrW(0)");
				goto Label_0168;
			Label_0151:
				this.EnsureInDoubleQuotes(ref flag1, builder1);
				builder1.Append(value[num1]);
			Label_0168:
				if ((num1 > 0) && ((num1 % 80) == 0))
				{
					if (flag1)
					{
						builder1.Append("\"");
					}
					flag1 = true;
					builder1.Append("& _ \r\n\"");
				}
			}
			if (flag1)
			{
				builder1.Append("\"");
			}
			return builder1.ToString();
		}

	
		#region KeyWords
		private static Hashtable keywordsHashtable = new Hashtable();
		private static readonly string[] keywords = new string[]{
																	"as",
																	"do",
																	"if",
																	"in",
																	"is",
																	"me",
																	"on",
																	"or",
																	"to",
																	"and",
																	"chr",
																	"dim",
																	"end",
																	"for",
																	"get",
																	"let",
																	"lib",
																	"mod",
																	"new",
																	"not",
																	"off",
																	"rem",
																	"set",
																	"sub",
																	"try",
																	"xor",
																	"ansi",
																	"auto",
																	"byte",
																	"call",
																	"case",
																	"cchr",
																	"cdbl",
																	"cdec",
																	"char",
																	"chrw",
																	"cint",
																	"clng",
																	"cobj",
																	"csng",
																	"cstr",
																	"date",
																	"each",
																	"else",
																	"enum",
																	"exit",
																	"goto",
																	"like",
																	"long",
																	"loop",
																	"next",
																	"null",
																	"step",
																	"stop",
																	"text",
																	"then",
																	"true",
																	"when",
																	"with",
																	"alias",
																	"andif",
																	"byref",
																	"byval",
																	"catch",
																	"cbool",
																	"cbyte",
																	"cchar",
																	"cdate",
																	"class",
																	"const",																		
																	"ctype",
																	"endif",
																	"erase",
																	"error",
																	"event",
																	"false",
																	"gosub",
																	"redim",
																	"short",
																	"throw",
																	"until",
																	"while",
																	"binary",
																	"cshort",
																	"double",
																	"elseif",
																	"friend",
																	"module",
																	"mythis",
																	"object",
																	"option",
																	"orelse",
																	"public",
																	"region",
																	"resume",
																	"return",
																	"select",
																	"shared",
																	"single",
																	"static",
																	"strict",
																	"string",
																	"typeof",
																	"andalso",
																	"boolean",
																	"compare",
																	"convert",
																	"decimal",
																	"declare",
																	"default",
																	"finally",
																	"gettype",
																	"handles",
																	"imports",
																	"integer",
																	"myclass",
																	"nothing",
																	"private",
																	"shadows",
																	"unicode",
																	"variant",
																	"assembly",
																	"delegate",
																	"explicit",
																	"function",
																	"inherits",
																	"optional",
																	"preserve",
																	"property",
																	"readonly",
																	"synclock",
																	"addressof",
																	"interface",
																	"namespace",
																	"overloads",
																	"overrides",
																	"protected",
																	"structure",
																	"writeonly",
																	"addhandler",
																	"implements",
																	"paramarray",
																	"raiseevent",
																	"withevents",
																	"endprologue",
																	"mustinherit",
																	"overridable",
																	"mustoverride",
																	"beginepilogue",
																	"removehandler",
																	"externalsource",
																	"notinheritable",
																	"notoverridable"
																};
		#endregion
			
		#region LanguageSupport
		private const GeneratorSupport LanguageSupport = 
			GeneratorSupport.ArraysOfArrays |
			GeneratorSupport.EntryPointMethod | 
			GeneratorSupport.GotoStatements | 
			GeneratorSupport.MultidimensionalArrays |
			GeneratorSupport.StaticConstructors |
			GeneratorSupport.TryCatchStatements | 
			GeneratorSupport.ReturnTypeAttributes |
			GeneratorSupport.DeclareValueTypes | 
			GeneratorSupport.DeclareEnums |
			GeneratorSupport.DeclareDelegates |
			GeneratorSupport.DeclareInterfaces | 
			GeneratorSupport.DeclareEvents |
			GeneratorSupport.AssemblyAttributes |
			GeneratorSupport.ParameterAttributes | 
			GeneratorSupport.ReferenceParameters |
			GeneratorSupport.ChainedConstructorArguments |
			GeneratorSupport.NestedTypes |
			GeneratorSupport.MultipleInterfaceMembers |
			GeneratorSupport.PublicStaticMembers |
			GeneratorSupport.ComplexExpressions |
			GeneratorSupport.Win32Resources;
		#endregion

		private const int MaxLineLength = 80;

		public override string GetRegionStartWord()
		{
			return "#Region";
		}

		public override string GetRegionEndWord()
		{
			return "#End Region";
		}

		public override string  GetRegionStart(string s)
		{
			return GetRegionStartWord() + " " + '"' + s + '"';
		}

		public override string  GetRegionEnd(string s)
		{
			return GetRegionEndWord() + " '" + s;
		}

		protected override void GenerateRegionStart(StiCodeRegionStart e) 
		{
			Output.Write(GetRegionStart(e.Text));
			Output.WriteLine(string.Empty);
		}

		protected override void GenerateRegionEnd(StiCodeRegionEnd e) 
		{
			Output.Write(GetRegionEnd(e.Text));
			Output.WriteLine(string.Empty);
		}

		public override string GetClassKeyword()
		{
			return "Class";
		}
 
	}
 

}
