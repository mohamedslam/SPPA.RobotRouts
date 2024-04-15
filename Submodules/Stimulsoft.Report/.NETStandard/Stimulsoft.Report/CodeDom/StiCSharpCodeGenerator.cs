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
using System.Collections;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Text;
using System.Globalization;

namespace Stimulsoft.Report.CodeDom
{
	/// <summary>
	/// Class describes CSharp CodeGenerator.
	/// </summary>
	public class StiCSharpCodeGenerator : StiCodeGenerator
	{
		#region	Consts
		private const GeneratorSupport LanguageSupport = 
			GeneratorSupport.ArraysOfArrays |
			GeneratorSupport.EntryPointMethod |
			GeneratorSupport.GotoStatements |
			GeneratorSupport.MultidimensionalArrays |
			GeneratorSupport.StaticConstructors |
			GeneratorSupport.TryCatchStatements |
			GeneratorSupport.ReturnTypeAttributes |
			GeneratorSupport.AssemblyAttributes |
			GeneratorSupport.DeclareValueTypes |
			GeneratorSupport.DeclareEnums | 
			GeneratorSupport.DeclareEvents | 
			GeneratorSupport.DeclareDelegates |
			GeneratorSupport.DeclareInterfaces |
			GeneratorSupport.ParameterAttributes |
			GeneratorSupport.ReferenceParameters |
			GeneratorSupport.ChainedConstructorArguments |
			GeneratorSupport.NestedTypes |
			GeneratorSupport.MultipleInterfaceMembers |
			GeneratorSupport.PublicStaticMembers |
			GeneratorSupport.ComplexExpressions;
		#endregion

		#region keywords
		private static Hashtable keywordsHashtable = new Hashtable();

		private static readonly string[] keywords = 
		{
			"as",
			"do",
			"if",
			"in",
			"is",
			"for",
			"int",
			"new",
			"out",
			"ref",
			"try",
			"base",
			"bool",
			"byte",
			"case",
			"char",
			"else",
			"enum",
			"goto",
			"lock",
			"long",
			"null",
			"this",
			"true",
			"uint",
			"void",
			"break",
			"catch",
			"class",
			"const",
			"event",
			"false",
			"fixed",
			"float",
			"sbyte",
			"short",
			"throw",
			"ulong",
			"using",
			"while",
			"double",
			"extern",
			"object",
			"params",
		    "public",
            "return",
			"sealed",
			"sizeof",
			"static",
			"string",
			"struct",
			"switch",
			"typeof",
			"unsafe",
			"ushort",
			"checked",
			"decimal",
			"default",
			"exfloat",
			"finally",
			"foreach",
			"private",
			"virtual",
			"abstract",
			"continue",
			"delegate",
			"exdouble",
			"explicit",
			"implicit",
			"internal",
			"operator",
			"override",
			"readonly",
			"interface",
			"namespace",
			"protected",
			"unchecked"
		};
		#endregion

		private bool forLoopHack = false;
		
		protected override string NullToken 
		{
			get 
			{
				return "null";
			}
		}

		public static bool MaximizeStringLength { get; set; } = true;

		protected string QuoteSnippetStringCStyle(string value)
		{
			// CS1647: An expression is too long or complex to compile near '...'
			// happens if number of line wraps is too many (335440 is max for x64, 926240 is max for x86)

			// CS1034: Compiler limit exceeded: Line cannot exceed 16777214 characters
			// theoretically every character could be escaped unicode (6 chars), plus quotes, etc.

			const int MinLineLength = 1024;
			const int MaxLineWrapWidth = (16777214 / 6) - 4;

			int lineWrapWidth = value.Length / 100; //max 100 lines for one image
			if (lineWrapWidth < MinLineLength) lineWrapWidth = MinLineLength;
			if (lineWrapWidth > MaxLineWrapWidth) lineWrapWidth = MaxLineWrapWidth;
			if (MaximizeStringLength)
			{
				double cf = value.Length / (double)MaxLineWrapWidth;
				if (cf <= 1)
					cf = 1;
				else
					cf = Math.Ceiling(cf);
				lineWrapWidth = (int)(MaxLineWrapWidth / cf);
			}

			StringBuilder b = new StringBuilder(value.Length + 5);

			b.Append("\"");

			for (int i = 0; i < value.Length; i++) 
			{
				switch(value[i])
                {
                    case '\u2028':
                    case '\u2029':
                        b.Append(@"\u");
                        b.Append(((int)value[i]).ToString("X4", CultureInfo.InvariantCulture));
                        break;
					case '\r':
						b.Append("\\r");
						break;
					case '\t':
						b.Append("\\t");
						break;
					case '\"':
						b.Append("\\\"");
						break;
					case '\'':
						b.Append("\\\'");
						break;
					case '\\':
						b.Append("\\\\");
						break;
					case '\0':
						b.Append("\\0");
						break;
					case '\n':
						b.Append("\\n");
						break;
					default:
						b.Append(value[i]);
						break;
				}

                if ((i > 0) && ((i % lineWrapWidth) == 0))
                {
                    if (Char.IsHighSurrogate(value[i]) && (i < (value.Length - 1)) && Char.IsLowSurrogate(value[i + 1]))
                    {
                        b.Append(value[++i]);
                    }
                    b.Append("\" +\r\n\"");
                }
			}

			b.Append("\"");

			return b.ToString();
		}

	    public override string QuoteSnippetString(string value)
	    {
	        return value != null ? QuoteSnippetStringCStyle(value) : "";
	    }

		protected override void GenerateArrayCreateExpression(CodeArrayCreateExpression e) 
		{
			Output.Write("new ");

			CodeExpressionCollection init = e.Initializers;
			if (init.Count > 0) 
			{
				OutputType(e.CreateType);
				if (e.CreateType.ArrayRank == 0)Output.Write("[]");
				Output.WriteLine(" {");
				Indent++;

				OutputExpressionList(init, true);
				Indent--;
				Output.Write("}");
			}
			else 
			{
				Output.Write(GetBaseTypeOutput(e.CreateType.BaseType));
				Output.Write("[");
				if (e.SizeExpression != null) 
				{
					GenerateExpression(e.SizeExpression);
				}
				else 
				{
					Output.Write(e.Size);
				}
				Output.Write("]");
			}
		}
		protected override void GenerateBaseReferenceExpression(CodeBaseReferenceExpression e) 
		{
			Output.Write("base");
		}

		protected override void GenerateCastExpression(CodeCastExpression e) 
		{
			Output.Write("((");
			OutputType(e.TargetType);
			Output.Write(")(");
			GenerateExpression(e.Expression);
			Output.Write("))");
		}

		protected override void GenerateDelegateCreateExpression(CodeDelegateCreateExpression e) 
		{
			Output.Write("new ");
			OutputType(e.DelegateType);
			Output.Write("(");
			GenerateExpression(e.TargetObject);
			Output.Write(".");
			OutputIdentifier(e.MethodName);
			Output.Write(")");
		}

		protected override void GenerateFieldReferenceExpression(CodeFieldReferenceExpression e) 
		{
			if (e.TargetObject != null) 
			{
				GenerateExpression(e.TargetObject);
				Output.Write(".");
			}
			OutputIdentifier(e.FieldName);
		}

		protected override void GenerateArgumentReferenceExpression(CodeArgumentReferenceExpression e) 
		{
			OutputIdentifier(e.ParameterName);
		}

		protected override void GenerateVariableReferenceExpression(CodeVariableReferenceExpression e) 
		{
			OutputIdentifier(e.VariableName);
		}

		protected override void GenerateIndexerExpression(CodeIndexerExpression e) 
		{
			GenerateExpression(e.TargetObject);
			Output.Write("[");
			bool first = true;
			foreach (CodeExpression exp in e.Indices) 
			{            
				if (first)first = false;
				else Output.Write(", ");
				GenerateExpression(exp);
			}
			Output.Write("]");

		}

		protected override void GenerateArrayIndexerExpression(CodeArrayIndexerExpression e) 
		{
			GenerateExpression(e.TargetObject);
			Output.Write("[");
			bool first = true;
			foreach (CodeExpression exp in e.Indices) 
			{            
				if (first)first = false;
				else Output.Write(", ");
				GenerateExpression(exp);
			}
			Output.Write("]");

		}

		protected override void GenerateSnippetExpression(CodeSnippetExpression e) 
		{
			Output.Write(e.Value);
		}

		protected override void GenerateMethodInvokeExpression(CodeMethodInvokeExpression e) 
		{
			GenerateMethodReferenceExpression(e.Method);
			Output.Write("(");
			OutputExpressionList(e.Parameters);
			Output.Write(")");
		}

		protected override void GenerateMethodReferenceExpression(CodeMethodReferenceExpression e) 
		{
			if (e.TargetObject != null) 
			{
				if (e.TargetObject is CodeBinaryOperatorExpression) 
				{
					Output.Write("(");
					GenerateExpression(e.TargetObject);
					Output.Write(")");
				}
				else GenerateExpression(e.TargetObject);
				Output.Write(".");
			}
			OutputIdentifier(e.MethodName);
		}

		protected override void GenerateEventReferenceExpression(CodeEventReferenceExpression e) 
		{
			if (e.TargetObject != null) 
			{
				GenerateExpression(e.TargetObject);
				Output.Write(".");
			}
			OutputIdentifier(e.EventName);
		}

		protected override void GenerateDelegateInvokeExpression(CodeDelegateInvokeExpression e) 
		{
			if (e.TargetObject != null)GenerateExpression(e.TargetObject);
			Output.Write("(");
			OutputExpressionList(e.Parameters);
			Output.Write(")");
		}

		protected override void GenerateObjectCreateExpression(CodeObjectCreateExpression e) 
		{
			Output.Write("new ");
			OutputType(e.CreateType);
			Output.Write("(");
			OutputExpressionList(e.Parameters);
			Output.Write(")");
		}
        
		protected override void GeneratePrimitiveExpression(CodePrimitiveExpression e) 
		{
			if (e.Value is char)GeneratePrimitiveChar((char)e.Value);
			else base.GeneratePrimitiveExpression(e);
		}

		protected override void GeneratePropertySetValueReferenceExpression(CodePropertySetValueReferenceExpression e) 
		{
			Output.Write("value");
		}

		protected override void GenerateThisReferenceExpression(CodeThisReferenceExpression e) 
		{
			Output.Write("this");
		}

		protected override void GenerateParameterDeclarationExpression(CodeParameterDeclarationExpression e) 
		{
			if (e.CustomAttributes.Count > 0)GenerateAttributes(e.CustomAttributes, null, true);
			OutputDirection(e.Direction);
			OutputTypeNamePair(e.Type, e.Name);
		}

		
		protected override void GenerateExpressionStatement(CodeExpressionStatement e) 
		{
			GenerateExpression(e.Expression);
			if (!forLoopHack)Output.WriteLine(";");
		}

		protected override void GenerateIterationStatement(CodeIterationStatement e) 
		{
			forLoopHack = true;
			Output.Write("for (");
			GenerateStatement(e.InitStatement);
			Output.Write("; ");
			GenerateExpression(e.TestExpression);
			Output.Write("; ");
			GenerateStatement(e.IncrementStatement);
			Output.Write(")");
			OutputStartingBrace();
			forLoopHack = false;
			Indent++;
			GenerateStatements(e.Statements);
			Indent--;
			Output.WriteLine("}");
		}
		protected override void GenerateThrowExceptionStatement(CodeThrowExceptionStatement e) 
		{
			Output.Write("throw");
			if (e.ToThrow != null) 
			{
				Output.Write(" ");
				GenerateExpression(e.ToThrow);
			}
			Output.WriteLine(";");
		}

		protected override void GenerateComment(CodeComment e) 
		{
			String text = ConvertToCommentEscapeCodes(e.Text);
			if (e.DocComment)Output.Write("/// ");
			else Output.Write("// ");
			Output.WriteLine(text);
		}

		protected override void GenerateMethodReturnStatement(CodeMethodReturnStatement e) 
		{
			Output.Write("return");
			if (e.Expression != null) 
			{
				Output.Write(" ");
				GenerateExpression(e.Expression);
			}
			Output.WriteLine(";");
		}
		protected override void GenerateConditionStatement(CodeConditionStatement e) 
		{
			Output.Write("if (");
			GenerateExpression(e.Condition);
			Output.Write(")");
			OutputStartingBrace();            
			Indent++;
			GenerateStatements(e.TrueStatements);
			Indent--;

			CodeStatementCollection falseStatemetns = e.FalseStatements;
			if (falseStatemetns.Count > 0) 
			{
				Output.Write("}");
				if (Options.ElseOnClosing)Output.Write(" ");
				else Output.WriteLine("");
				Output.Write("else");
				OutputStartingBrace();
				Indent++;
				GenerateStatements(e.FalseStatements);
				Indent--;
			}
			Output.WriteLine("}");
		}
		protected override void GenerateTryCatchFinallyStatement(CodeTryCatchFinallyStatement e) 
		{
			Output.Write("try");
			OutputStartingBrace();
			Indent++;
			GenerateStatements(e.TryStatements);
			Indent--;
			CodeCatchClauseCollection catches = e.CatchClauses;
			if (catches.Count > 0) 
			{
				IEnumerator en = catches.GetEnumerator();
				while (en.MoveNext()) 
				{
					Output.Write("}");
					if (Options.ElseOnClosing)Output.Write(" ");
					else Output.WriteLine("");
					CodeCatchClause current = (CodeCatchClause)en.Current;
					Output.Write("catch (");
					OutputType(current.CatchExceptionType);
					Output.Write(" ");
					OutputIdentifier(current.LocalName);
					Output.Write(")");
					OutputStartingBrace();
					Indent++;
					GenerateStatements(current.Statements);
					Indent--;
				}
			}

			CodeStatementCollection finallyStatements = e.FinallyStatements;
			if (finallyStatements.Count > 0) 
			{
				Output.Write("}");
				if (Options.ElseOnClosing)Output.Write(" ");
				else Output.WriteLine("");
				Output.Write("finally");
				OutputStartingBrace();
				Indent++;
				GenerateStatements(finallyStatements);
				Indent--;
			}
			Output.WriteLine("}");
		}
		protected override void GenerateAssignStatement(CodeAssignStatement e) 
		{
			GenerateExpression(e.Left);
			Output.Write(" = ");
			GenerateExpression(e.Right);
			if (!forLoopHack)Output.WriteLine(";");
		}

		protected override void GenerateAttachEventStatement(CodeAttachEventStatement e) 
		{
			GenerateEventReferenceExpression(e.Event);
			Output.Write(" += ");
			GenerateExpression(e.Listener);
			Output.WriteLine(";");
		}

		protected override void GenerateRemoveEventStatement(CodeRemoveEventStatement e) 
		{
			GenerateEventReferenceExpression(e.Event);
			Output.Write(" -= ");
			GenerateExpression(e.Listener);
			Output.WriteLine(";");
		}

		protected override void GenerateSnippetStatement(CodeSnippetStatement e) 
		{
			Output.WriteLine(e.Value);
		}

		protected override void GenerateGotoStatement(CodeGotoStatement e) 
		{
			Output.Write("goto ");
			Output.Write(e.Label);
			Output.WriteLine(";");
		}

		protected override void GenerateLabeledStatement(CodeLabeledStatement e) 
		{
			Indent--;
			Output.Write(e.Label);
			Output.WriteLine(":");
			Indent++;
			if (e.Statement != null)GenerateStatement(e.Statement);
		}

		protected override void GenerateVariableDeclarationStatement(CodeVariableDeclarationStatement e) 
		{
			OutputTypeNamePair(e.Type, e.Name);
			if (e.InitExpression != null) 
			{
				Output.Write(" = ");
				GenerateExpression(e.InitExpression);
			}
			if (!forLoopHack) 
			{
				Output.WriteLine(";");
			}
		}
		

		protected override void GenerateLinePragmaStart(CodeLinePragma e) 
		{
			Output.WriteLine("");
			Output.Write("#line ");
			Output.Write(e.LineNumber);
			Output.Write(" \"");
			Output.Write(e.FileName);
			Output.Write("\"");
			Output.WriteLine("");
		}
		protected override void GenerateLinePragmaEnd(CodeLinePragma e) 
		{
			Output.WriteLine();
			Output.WriteLine("#line default");
		}

		protected override void GenerateEvent(CodeMemberEvent e, CodeTypeDeclaration c) 
		{
			if (IsCurrentDelegate || IsCurrentEnum) return;

			if (e.CustomAttributes.Count > 0) 
			{
				GenerateAttributes(e.CustomAttributes);
			}

			if (e.PrivateImplementationType == null) 
			{
				OutputMemberAccessModifier(e.Attributes);
			}
			Output.Write("event ");
			string name = e.Name;
			if (e.PrivateImplementationType != null) 
			{
				name = e.PrivateImplementationType.BaseType + "." + name;
			}
			OutputTypeNamePair(e.Type, name);
			Output.WriteLine(";");
		}

		protected override void GenerateField(CodeMemberField e) 
		{
			if (IsCurrentDelegate || IsCurrentInterface) return;

			if (IsCurrentEnum) 
			{
				if (e.CustomAttributes.Count > 0) 
				{
					GenerateAttributes(e.CustomAttributes);
				}
				OutputIdentifier(e.Name);
				if (e.InitExpression != null) 
				{
					Output.Write(" = ");
					GenerateExpression(e.InitExpression);
				}
				Output.WriteLine(",");
			}
			else 
			{
				if (e.CustomAttributes.Count > 0) 
				{
					GenerateAttributes(e.CustomAttributes);
				}

				OutputMemberAccessModifier(e.Attributes);
				OutputVTableModifier(e.Attributes);
				OutputFieldScopeModifier(e.Attributes);
				OutputTypeNamePair(e.Type, e.Name);
				if (e.InitExpression != null) 
				{
					Output.Write(" = ");
					GenerateExpression(e.InitExpression);
				}
				Output.WriteLine(";");
			}
		}
		
		protected override void GenerateSnippetMember(CodeSnippetTypeMember e) 
		{
			Output.Write(e.Text);
		}

		
		protected override void GenerateEntryPointMethod(CodeEntryPointMethod e, CodeTypeDeclaration c) 
		{
			Output.Write("public static void Main()");
			OutputStartingBrace();
			Indent++;

			GenerateStatements(e.Statements);

			Indent--;
			Output.WriteLine("}");
		}

		protected override void GenerateMethod(CodeMemberMethod e, CodeTypeDeclaration c) 
		{
			if (!(IsCurrentClass || IsCurrentStruct || IsCurrentInterface)) return;

			if (e.CustomAttributes.Count > 0) 
			{
				GenerateAttributes(e.CustomAttributes);
			}
			if (e.ReturnTypeCustomAttributes.Count > 0) 
			{
				GenerateAttributes(e.ReturnTypeCustomAttributes, "return: ");
			}

			if (!IsCurrentInterface) 
			{
				if (e.PrivateImplementationType == null) 
				{
					OutputMemberAccessModifier(e.Attributes);
					OutputVTableModifier(e.Attributes);
					OutputMemberScopeModifier(e.Attributes);
				}
			}
			else 
			{
				OutputVTableModifier(e.Attributes);
			}
			OutputType(e.ReturnType);
			Output.Write(" ");
			if (e.PrivateImplementationType != null) 
			{
				Output.Write(e.PrivateImplementationType.BaseType);
				Output.Write(".");
			}
			OutputIdentifier(e.Name);
			Output.Write("(");
			OutputParameters(e.Parameters);
			Output.Write(")");
            
			if (!IsCurrentInterface 
				&& (e.Attributes & MemberAttributes.ScopeMask) != MemberAttributes.Abstract) 
			{

				OutputStartingBrace();
				Indent++;

				GenerateStatements(e.Statements);

				Indent--;
				Output.WriteLine("}");
			}
			else 
			{
				Output.WriteLine(";");
			}
		}
		protected override void GenerateProperty(CodeMemberProperty e, CodeTypeDeclaration c) 
		{
			if (!(IsCurrentClass || IsCurrentStruct || IsCurrentInterface)) return;

			if (e.CustomAttributes.Count > 0) 
			{
				GenerateAttributes(e.CustomAttributes);
			}

			if (!IsCurrentInterface) 
			{
				if (e.PrivateImplementationType == null) 
				{
					OutputMemberAccessModifier(e.Attributes);
					OutputVTableModifier(e.Attributes);
					OutputMemberScopeModifier(e.Attributes);
				}
			}
			else 
			{
				OutputVTableModifier(e.Attributes);
			}
			OutputType(e.Type);
			Output.Write(" ");

			if (e.PrivateImplementationType != null && !IsCurrentInterface) 
			{
				Output.Write(e.PrivateImplementationType.BaseType);
				Output.Write(".");
			}

			if (e.Parameters.Count > 0 && String.Compare(e.Name, "Item", true, CultureInfo.InvariantCulture) == 0) 
			{
				Output.Write("this[");
				OutputParameters(e.Parameters);
				Output.Write("]");
			}
			else 
			{
				OutputIdentifier(e.Name);
			}

			OutputStartingBrace();
			Indent++;

			if (e.HasGet) 
			{
				if (IsCurrentInterface || (e.Attributes & MemberAttributes.ScopeMask) == MemberAttributes.Abstract) 
				{
					Output.WriteLine("get;");
				}
				else 
				{
					Output.Write("get");
					OutputStartingBrace();
					Indent++;
					GenerateStatements(e.GetStatements);
					Indent--;
					Output.WriteLine("}");
				}
			}
			if (e.HasSet) 
			{
				if (IsCurrentInterface || (e.Attributes & MemberAttributes.ScopeMask) == MemberAttributes.Abstract) 
				{
					Output.WriteLine("set;");
				}
				else 
				{
					Output.Write("set");
					OutputStartingBrace();
					Indent++;
					GenerateStatements(e.SetStatements);
					Indent--;
					Output.WriteLine("}");
				}
			}

			Indent--;
			Output.WriteLine("}");
		}
        
		protected override void GenerateSingleFloatValue(Single s) 
		{
			Output.Write(s.ToString(CultureInfo.InvariantCulture));
			Output.Write('F');
		}

		protected override void GenerateDecimalValue(Decimal d) 
		{
			Output.Write(d.ToString(CultureInfo.InvariantCulture));
			Output.Write('m');
		}

		protected override void GeneratePropertyReferenceExpression(CodePropertyReferenceExpression e) 
		{
			if (e.TargetObject != null) 
			{
				GenerateExpression(e.TargetObject);
				Output.Write(".");
			}
			OutputIdentifier(e.PropertyName);
		}

		protected override void GenerateConstructor(CodeConstructor e, CodeTypeDeclaration c) 
		{
			if (!(IsCurrentClass || IsCurrentStruct)) return;

			if (e.CustomAttributes.Count > 0) 
			{
				GenerateAttributes(e.CustomAttributes);
			}

			OutputMemberAccessModifier(e.Attributes);
			OutputIdentifier(CurrentTypeName);
			Output.Write("(");
			OutputParameters(e.Parameters);
			Output.Write(")");

			CodeExpressionCollection baseArgs = e.BaseConstructorArgs;
			CodeExpressionCollection thisArgs = e.ChainedConstructorArgs;

			if (baseArgs.Count > 0) 
			{
				Output.WriteLine(" : ");
				Indent++;
				Indent++;
				Output.Write("base(");
				OutputExpressionList(baseArgs);
				Output.Write(")");
				Indent--;
				Indent--;
			}

			if (thisArgs.Count > 0) 
			{
				Output.WriteLine(" : ");
				Indent++;
				Indent++;
				Output.Write("this(");
				OutputExpressionList(thisArgs);
				Output.Write(")");
				Indent--;
				Indent--;
			}

			OutputStartingBrace();
			Indent++;
			GenerateStatements(e.Statements);
			Indent--;
			Output.WriteLine("}");
		}
		protected override void GenerateTypeConstructor(CodeTypeConstructor e) 
		{
			if (!(IsCurrentClass || IsCurrentStruct)) return;

			Output.Write("static ");
			Output.Write(CurrentTypeName);
			Output.Write("()");
			OutputStartingBrace();
			Indent++;
			GenerateStatements(e.Statements);
			Indent--;
			Output.WriteLine("}");
		}
		protected override void GenerateTypeStart(CodeTypeDeclaration e) 
		{
			if (e.CustomAttributes.Count > 0) 
			{
				GenerateAttributes(e.CustomAttributes);
			}

			if (IsCurrentDelegate) 
			{
				switch (e.TypeAttributes & TypeAttributes.VisibilityMask) 
				{
					case TypeAttributes.Public:
						Output.Write("public ");
						break;
					case TypeAttributes.NotPublic:
					default:
						break;
				}

				CodeTypeDelegate del = (CodeTypeDelegate)e;
				Output.Write("delegate ");
				OutputType(del.ReturnType);
				Output.Write(" ");
				OutputIdentifier(e.Name);
				Output.Write("(");
				OutputParameters(del.Parameters);
				Output.WriteLine(");");
			} 
			else 
			{
				OutputTypeAttributes(e);                                
				OutputIdentifier(e.Name);             

				bool first = true;
				foreach (CodeTypeReference typeRef in e.BaseTypes) 
				{
					if (first) 
					{
						Output.Write(" : ");
						first = false;
					}
					else 
					{
						Output.Write(", ");
					}                 
					OutputType(typeRef);
				}
				OutputStartingBrace();
				Indent++;                
			}
		}

		protected override void GenerateTypeEnd(CodeTypeDeclaration e) 
		{
			if (!IsCurrentDelegate) 
			{
				Indent--;
				Output.WriteLine("}");
			}
		}
		protected override void GenerateNamespaceStart(CodeNamespace e) 
		{
			if (!string.IsNullOrEmpty(e.Name)) 
			{
				Output.Write("namespace ");
				OutputIdentifier(e.Name);
				OutputStartingBrace();
				Indent++;
			}
		}
        
		protected override void GenerateCompileUnitStart(CodeCompileUnit e) 
		{
			if (e.AssemblyCustomAttributes.Count > 0) 
			{
				GenerateAttributes(e.AssemblyCustomAttributes, "assembly: ");
				Output.WriteLine("");
			}
		}

		protected override void GenerateNamespaceEnd(CodeNamespace e) 
		{
			if (!string.IsNullOrEmpty(e.Name)) 
			{
				Indent--;
				Output.WriteLine("}");
			}
		}
		protected override void GenerateNamespaceImport(CodeNamespaceImport e) 
		{
			Output.Write("using "+e.Namespace);
			Output.WriteLine(";");
		}
		
		protected override void GenerateAttributeDeclarationsStart(CodeAttributeDeclarationCollection attributes) 
		{
			Output.Write("[");
		}
		protected override void GenerateAttributeDeclarationsEnd(CodeAttributeDeclarationCollection attributes) 
		{
			Output.Write("]");
		}

		
		private void GenerateAttributes(CodeAttributeDeclarationCollection attributes) 
		{
			GenerateAttributes(attributes, null, false);
		}

		private void GenerateAttributes(CodeAttributeDeclarationCollection attributes, string prefix) 
		{
			GenerateAttributes(attributes, prefix, false);
		}
    
		private void GenerateAttributes(CodeAttributeDeclarationCollection attributes, string prefix, bool inLine) 
		{
			if (attributes.Count == 0) return;
			IEnumerator en = attributes.GetEnumerator();
			while (en.MoveNext()) 
			{
				GenerateAttributeDeclarationsStart(attributes);
				if (prefix != null) 
				{
					Output.Write(prefix);
				}

				CodeAttributeDeclaration current = (CodeAttributeDeclaration)en.Current;
				Output.Write(GetBaseTypeOutput(current.Name));
				Output.Write("(");

				bool firstArg = true;
				foreach (CodeAttributeArgument arg in current.Arguments) 
				{
					if (firstArg)firstArg = false;
					else Output.Write(", ");
					OutputAttributeArgument(arg);
				}

				Output.Write(")");
				GenerateAttributeDeclarationsEnd(attributes);
				if (inLine)Output.Write(" ");
				else Output.WriteLine();
			}
		}

		public static bool IsKeywordExist(string value) 
		{
			return keywordsHashtable[value] != null;
		}

		
		public override bool IsKeyword(string value) 
		{
			return IsKeywordExist(value);
		}

		public override string GetClassKeyword()
		{
			return "class";
		}

		protected override bool Supports(GeneratorSupport support) 
		{
			return ((support & LanguageSupport) == support);
		}

		
		protected override bool IsValidIdentifier(string value) 
		{
			if (string.IsNullOrEmpty(value))return false;
			if (value[0] != '@')
			{
				if (IsKeyword(value))
					return false;
			}
			else value = value.Substring(1);

			return CodeGenerator.IsValidLanguageIndependentIdentifier(value);
		}

		protected override string CreateValidIdentifier(string name) 
		{
			if (name == "date") return name;
			if (IsKeyword(name))return "_" + name;
			return name;
		}

		protected override string CreateEscapedIdentifier(string name) 
		{
			if (IsKeyword(name))return "@" + name;
			return name;
		}


		public override string GetBaseTypeOutput(string baseType) 
		{
			string s = baseType;
			if (s.Length == 0)				s = "void";
			else if (s == "System.Byte")	s = "byte";
			else if (s == "System.Int16")	s = "short";
			else if (s == "System.Int32")	s = "int";
			else if (s == "System.Int64")	s = "long";
			else if (s == "System.String")	s = "string";
			else if (s == "System.Object")	s = "object";
			else if (s == "System.Boolean")	s = "bool";
			else if (s == "System.Void")	s = "void";
			else if (s == "System.Char")	s = "char";
			else if (s == "System.Double")	s = "double";
			else if (s == "System.Single")	s = "float";
			else if (s == "System.Decimal")	s = "decimal";
			else s = s.Replace('+', '.');
			return s;
		}

		protected override string GetTypeOutput(CodeTypeReference typeRef)
		{
			string s;
			if (typeRef.ArrayElementType != null) 
			{
				// Recurse up
				s = GetTypeOutput(typeRef.ArrayElementType);
			}
			else 
			{
				s = GetBaseTypeOutput(typeRef.BaseType);

                if (typeRef.TypeArguments.Count > 0)
                {
                    if ((s.Length > 2) && (s[s.Length - 2] == '`') && char.IsDigit(s, s.Length - 1))
                    {
                        s = s.Substring(0, s.Length - 2);
                    }
                    s += "<";
                    for (int index = 0; index < typeRef.TypeArguments.Count; index++)
                    {
                        if (index > 0) s += ",";
                        s += GetTypeOutput(typeRef.TypeArguments[index]);
                    }
                    s += ">";
                }
			}
			// Now spit out the array postfix
			if (typeRef.ArrayRank > 0) 
			{
				char [] results = new char [typeRef.ArrayRank + 1];
				results[0] = '[';
				results[typeRef.ArrayRank] = ']';
				for (int i = 1; i < typeRef.ArrayRank; i++) 
				{
					results[i] = ',';
				}
				s += new string(results);
			}               
			return s;
		}


		private static string ConvertToCommentEscapeCodes(string value) 
		{
			StringBuilder b = new StringBuilder(value.Length);

			for (int i=0; i<value.Length; i++) 
			{
				switch (value[i]) 
				{
					case '\r':
						if (i < value.Length - 1 && value[i+1] == '\n') 
						{
							b.Append("\r\n//");
							i++;
						}
						else 
						{
							b.Append("\r//");
						}
						break;
					case '\n':
						b.Append("\n//");
						break;
					case '\u2028':
						b.Append("\u2028//");
						break;
					case '\u2029':
						b.Append("\u2029//");
						break;
						// Suppress null from being emitted since you get a compiler error
					case '\u0000': 
						break;
					default:
						b.Append(value[i]);
						break;
				}
			}

			return b.ToString();
		}
		
		private void OutputTypeAttributes(CodeTypeDeclaration e) 
		{
			TypeAttributes attributes = e.TypeAttributes;
			switch(attributes & TypeAttributes.VisibilityMask) 
			{
				case TypeAttributes.Public:                  
				case TypeAttributes.NestedPublic:                    
					Output.Write("public ");
					break;
				case TypeAttributes.NestedPrivate:
					Output.Write("private ");
					break;
			}
            
			if (e.IsStruct) 
			{
				Output.Write("struct ");
			}
			else if (e.IsEnum) 
			{
				Output.Write("enum ");
			}     
			else 
			{            
				switch (attributes & TypeAttributes.ClassSemanticsMask) 
				{
					case TypeAttributes.Class:
						if ((attributes & TypeAttributes.Sealed) == TypeAttributes.Sealed) 
						{
							Output.Write("sealed ");
						}
						if ((attributes & TypeAttributes.Abstract) == TypeAttributes.Abstract)  
						{
							Output.Write("abstract ");
						}
						Output.Write("class ");
						break;                
					case TypeAttributes.Interface:
						Output.Write("interface ");
						break;
				}     
			}   
		}

		private void OutputVTableModifier(MemberAttributes attributes) 
		{
			switch (attributes & MemberAttributes.VTableMask) 
			{
				case MemberAttributes.New:
					Output.Write("new ");
					break;
			}
		}

		protected override void OutputMemberScopeModifier(MemberAttributes attributes) 
		{
			switch (attributes & MemberAttributes.ScopeMask) 
			{
				case MemberAttributes.Abstract:
					Output.Write("abstract ");
					break;
				case MemberAttributes.Final:
					Output.Write("");
					break;
				case MemberAttributes.Static:
					Output.Write("static ");
					break;
				case MemberAttributes.Override:
					Output.Write("override ");
					break;
				default:
				switch (attributes & MemberAttributes.AccessMask) 
				{
					case MemberAttributes.Family:
					case MemberAttributes.Public:
						Output.Write("virtual ");
						break;
					default:
						// nothing;
						break;
				}
					break;
			}
		}

		protected override void OutputFieldScopeModifier(MemberAttributes attributes) 
		{
			switch (attributes & MemberAttributes.ScopeMask) 
			{
				case MemberAttributes.Final:
					break;
				case MemberAttributes.Static:
					Output.Write("static ");
					break;
				case MemberAttributes.Const:
					Output.Write("const ");
					break;
				default:
					break;
			}
		}

		protected override void OutputIdentifier(string ident) 
		{
			Output.Write(CreateEscapedIdentifier(ident));
		}

		protected override void OutputType(CodeTypeReference typeRef) 
		{
			Output.Write(GetTypeOutput(typeRef));
		}

		protected virtual void OutputStartingBrace() 
		{
			if (Options.BracingStyle == "C") 
			{
				Output.WriteLine("");
				Output.WriteLine("{");
			}
			else 
			{
				Output.WriteLine(" {");
			}
		}

		private void GeneratePrimitiveChar(char c) 
		{
			Output.Write('\'');
			switch (c) 
			{
				case '\r':
					Output.Write("\\r");
					break;
				case '\t':
					Output.Write("\\t");
					break;
				case '\"':
					Output.Write("\\\"");
					break;
				case '\'':
					Output.Write("\\\'");
					break;
				case '\\':
					Output.Write("\\\\");
					break;
				case '\0':
					Output.Write("\\0");
					break;
				case '\n':
					Output.Write("\\n");
					break;
				default:
					Output.Write(c);
					break;
			}
			Output.Write('\'');
		}
       
		public override string GetRegionStartWord()
		{
			return "#region";
		}

		public override string GetRegionEndWord()
		{
			return "#endregion";
		}

		public override string  GetRegionStart(string s)
		{
			return GetRegionStartWord() + " " + s;
		}

		public override string  GetRegionEnd(string s)
		{
			return GetRegionEndWord() + " " + s;
		}

		protected override void GenerateRegionStart(StiCodeRegionStart e) 
		{
			Output.Write(GetRegionStart(e.Text));
			Output.WriteLine("");
		}

		protected override void GenerateRegionEnd(StiCodeRegionEnd e) 
		{
			Output.Write(GetRegionEnd(e.Text));
			Output.WriteLine("");
		}

	    static StiCSharpCodeGenerator()
	    {
	        foreach (string str in keywords)
	        {
	            keywordsHashtable.Add(str, str);
	        }
	    }
    }
}

