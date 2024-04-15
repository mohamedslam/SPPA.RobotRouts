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
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Globalization;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Security.Permissions;

namespace Stimulsoft.Report.CodeDom
{
	/// <summary>
	/// Class describes CodeGenerator for a report.
	/// </summary>
	public abstract class StiCodeGenerator : ICodeGenerator
	{
		private const int ParameterMultilineThreshold = 15;
		private IndentedTextWriter output;
		private CodeGeneratorOptions options;

		private CodeTypeDeclaration currentClass;
		private CodeTypeMember currentMember;

		private bool inNestedBinary = false;

		protected string CurrentTypeName 
		{
			get 
			{
				if (currentClass != null) 
				{
					return currentClass.Name;
				}
				return "<% unknown %>";
			}
		}

		protected CodeTypeMember CurrentMember 
		{
			get 
			{
				return currentMember;
			}
			set
			{
				currentMember = value;
			}
		}

		protected string CurrentMemberName 
		{
			get 
			{
				if (currentMember != null) 
				{
					return currentMember.Name;
				}
				return "<% unknown %>";
			}
		}

		protected bool IsCurrentInterface 
		{
			get 
			{
				if (currentClass != null && !(currentClass is CodeTypeDelegate)) 
				{
					return currentClass.IsInterface;
				}
				return false;
			}
		}

		protected bool IsCurrentClass 
		{
			get 
			{
				if (currentClass != null && !(currentClass is CodeTypeDelegate))return currentClass.IsClass;
				return false;
			}
		}

		protected bool IsCurrentStruct 
		{
			get 
			{
				if (currentClass != null && !(currentClass is CodeTypeDelegate))return currentClass.IsStruct;
				return false;
			}
		}

		protected bool IsCurrentEnum 
		{
			get 
			{
				if (currentClass != null && !(currentClass is CodeTypeDelegate))return currentClass.IsEnum;
				return false;
			}
		}

		protected bool IsCurrentDelegate 
		{
			get 
			{
				if (currentClass != null && currentClass is CodeTypeDelegate)return true;
				return false;
			}
		}

		protected int Indent 
		{
			get 
			{
				return output.Indent;
			}
			set 
			{
				output.Indent = value;
			}
		}


		protected abstract string NullToken { get; }

		protected TextWriter Output 
		{
			get 
			{
				return output;
			}
		}

		protected CodeGeneratorOptions Options 
		{
			get 
			{
				return options;
			}
		}

		
		private	void GenerateType(CodeTypeDeclaration e) 
		{
			currentClass = e;
			GenerateRegionStarts(e.Comments);
			GenerateTypeStart(e);
			GenerateTypeConstructors(e);
			GenerateConstructors(e);
			GenerateFields(e);
			GenerateSnippetMembers(e);
			GenerateProperties(e);
			GenerateEvents(e);
			GenerateMethods(e);
			GenerateNestedTypes(e);
			currentClass = e;
			GenerateTypeEnd(e);
			GenerateRegionEnds(e.Comments);
		}

		private	void GenerateTypeConstructors(CodeTypeDeclaration e) 
		{
			IEnumerator en = e.Members.GetEnumerator();
			while (en.MoveNext()) 
			{
				if (en.Current is CodeTypeConstructor) 
				{
					currentMember = (CodeTypeMember)en.Current;

					if (options.BlankLinesBetweenMembers)Output.WriteLine();
					GenerateRegionStarts(currentMember.Comments);
					CodeTypeConstructor imp = (CodeTypeConstructor)en.Current;
					if (imp.LinePragma != null) GenerateLinePragmaStart(imp.LinePragma);
					GenerateTypeConstructor(imp);
					if (imp.LinePragma != null) GenerateLinePragmaEnd(imp.LinePragma);
					GenerateRegionEnds(currentMember.Comments);
				}
			}
		}

		protected virtual void GenerateNamespaces(CodeCompileUnit e) 
		{
			foreach (CodeNamespace n in e.Namespaces) 
			{
				((ICodeGenerator)this).GenerateCodeFromNamespace(n, output.InnerWriter, options);
			}
		}

		protected	void GenerateTypes(CodeNamespace e) 
		{
			foreach (CodeTypeDeclaration c in e.Types) 
			{
				if (options.BlankLinesBetweenMembers)Output.WriteLine();
				((ICodeGenerator)this).GenerateCodeFromType(c, output.InnerWriter, options);
			}
		}

		
		bool ICodeGenerator.Supports(GeneratorSupport support) 
		{
			return this.Supports(support);
		}

		public void GenerateCodeFromType(CodeTypeDeclaration e, TextWriter w, CodeGeneratorOptions o) 
		{
			bool setLocal = false;
			if (output != null && w != output.InnerWriter) 
			{
				throw new InvalidOperationException("CodeGenOutputWriter");
			}
			if (output == null) 
			{
				setLocal = true;
				options = (o == null) ? new CodeGeneratorOptions() : o;
				output = new IndentedTextWriter(w, options.IndentString);
			}

			try 
			{
				GenerateType(e);
			}
			finally 
			{
				if (setLocal) 
				{
					output = null;
					options = null;
				}
			}
		}

		public void GenerateCodeFromExpression(CodeExpression e, TextWriter w, CodeGeneratorOptions o) 
		{
			bool setLocal = false;
			if (output != null && w != output.InnerWriter) 
			{
				throw new InvalidOperationException("CodeGenOutputWriter");
			}
			if (output == null) 
			{
				setLocal = true;
				options = (o == null) ? new CodeGeneratorOptions() : o;
				output = new IndentedTextWriter(w, options.IndentString);
			}

			try 
			{
				GenerateExpression(e);
			}
			finally 
			{
				if (setLocal) 
				{
					output = null;
					options = null;
				}
			}
		}

		public void GenerateCodeFromCompileUnit(CodeCompileUnit e, TextWriter w, CodeGeneratorOptions o) 
		{
			bool setLocal = false;
			if (output != null && w != output.InnerWriter) 
			{
				throw new InvalidOperationException("CodeGenOutputWriter");
			}
			if (output == null) 
			{
				setLocal = true;
				options = (o == null) ? new CodeGeneratorOptions() : o;
				output = new IndentedTextWriter(w, options.IndentString);
			}

			try 
			{
				if (e is CodeSnippetCompileUnit)GenerateSnippetCompileUnit((CodeSnippetCompileUnit) e);
				else GenerateCompileUnit(e);
			}
			finally 
			{
				if (setLocal) 
				{
					output = null;
					options = null;
				}
			}
		}

		public void GenerateCodeFromNamespace(CodeNamespace e, TextWriter w, CodeGeneratorOptions o) 
		{
			bool setLocal = false;
			if (output != null && w != output.InnerWriter) 
			{
				throw new InvalidOperationException("CodeGenOutputWriter");
			}
			if (output == null) 
			{
				setLocal = true;
				options = (o == null) ? new CodeGeneratorOptions() : o;
				output = new IndentedTextWriter(w, options.IndentString);
			}

			try 
			{
				GenerateNamespace(e);
			}
			finally 
			{
				if (setLocal) 
				{
					output = null;
					options = null;
				}
			}
		}

		public void GenerateCodeFromStatement(CodeStatement e, TextWriter w, CodeGeneratorOptions o) 
		{
			bool setLocal = false;
			if (output != null && w != output.InnerWriter) 
			{
				throw new InvalidOperationException("CodeGenOutputWriter");
			}
			if (output == null) 
			{
				setLocal = true;
				options = (o == null) ? new CodeGeneratorOptions() : o;
				output = new IndentedTextWriter(w, options.IndentString);
			}

			try 
			{
				GenerateStatement(e);
			}
			finally 
			{
				if (setLocal) 
				{
					output = null;
					options = null;
				}
			}
		}

		
		protected abstract string GetTypeOutput(CodeTypeReference value);
		
		bool ICodeGenerator.IsValidIdentifier(string value) 
		{
			return this.IsValidIdentifier(value);
		}

		void ICodeGenerator.ValidateIdentifier(string value) 
		{
			this.ValidateIdentifier(value);
		}


		string ICodeGenerator.CreateEscapedIdentifier(string value) 
		{
			return this.CreateEscapedIdentifier(value);
		}

		string ICodeGenerator.CreateValidIdentifier(string value) 
		{
			return this.CreateValidIdentifier(value);
		}

		string ICodeGenerator.GetTypeOutput(CodeTypeReference type) 
		{
			return this.GetTypeOutput(type);
		}

		private void GenerateConstructors(CodeTypeDeclaration e) 
		{
			IEnumerator en = e.Members.GetEnumerator();
			while (en.MoveNext()) 
			{
				if (en.Current is CodeConstructor) 
				{
					currentMember = (CodeTypeMember)en.Current;

					if (options.BlankLinesBetweenMembers)Output.WriteLine();
					GenerateRegionStarts(currentMember.Comments);
					CodeConstructor imp = (CodeConstructor)en.Current;
					if (imp.LinePragma != null) GenerateLinePragmaStart(imp.LinePragma);
					GenerateConstructor(imp, e);
					if (imp.LinePragma != null) GenerateLinePragmaEnd(imp.LinePragma);
					GenerateRegionEnds(currentMember.Comments);
				}
			}
		}

		private void GenerateEvents(CodeTypeDeclaration e) 
		{
			IEnumerator en = e.Members.GetEnumerator();
			while (en.MoveNext()) 
			{
				if (en.Current is CodeMemberEvent) 
				{
					currentMember = (CodeTypeMember)en.Current;

					if (options.BlankLinesBetweenMembers)Output.WriteLine();
					GenerateRegionStarts(currentMember.Comments);
					CodeMemberEvent imp = (CodeMemberEvent)en.Current;
					if (imp.LinePragma != null) GenerateLinePragmaStart(imp.LinePragma);
					GenerateEvent(imp, e);
					if (imp.LinePragma != null) GenerateLinePragmaEnd(imp.LinePragma);
					GenerateRegionEnds(currentMember.Comments);
				}
			}
		}

		protected void GenerateExpression(CodeExpression e) 
		{
			if (e is CodeArrayCreateExpression) 
			{
				GenerateArrayCreateExpression((CodeArrayCreateExpression)e);
			}
			else if (e is CodeBaseReferenceExpression) 
			{
				GenerateBaseReferenceExpression((CodeBaseReferenceExpression)e);
			}
			else if (e is CodeBinaryOperatorExpression) 
			{
				GenerateBinaryOperatorExpression((CodeBinaryOperatorExpression)e);
			}
			else if (e is CodeCastExpression) 
			{
				GenerateCastExpression((CodeCastExpression)e);
			}
			else if (e is CodeDelegateCreateExpression) 
			{
				GenerateDelegateCreateExpression((CodeDelegateCreateExpression)e);
			}
			else if (e is CodeFieldReferenceExpression) 
			{
				GenerateFieldReferenceExpression((CodeFieldReferenceExpression)e);
			}
			else if (e is CodeArgumentReferenceExpression) 
			{
				GenerateArgumentReferenceExpression((CodeArgumentReferenceExpression)e);
			}
			else if (e is CodeVariableReferenceExpression) 
			{
				GenerateVariableReferenceExpression((CodeVariableReferenceExpression)e);
			}
			else if (e is CodeIndexerExpression) 
			{
				GenerateIndexerExpression((CodeIndexerExpression)e);
			}
			else if (e is CodeArrayIndexerExpression) 
			{
				GenerateArrayIndexerExpression((CodeArrayIndexerExpression)e);
			}
			else if (e is CodeSnippetExpression) 
			{
				GenerateSnippetExpression((CodeSnippetExpression)e);
			}
			else if (e is CodeMethodInvokeExpression) 
			{
				GenerateMethodInvokeExpression((CodeMethodInvokeExpression)e);
			}
			else if (e is CodeMethodReferenceExpression) 
			{
				GenerateMethodReferenceExpression((CodeMethodReferenceExpression)e);
			}
			else if (e is CodeEventReferenceExpression) 
			{
				GenerateEventReferenceExpression((CodeEventReferenceExpression)e);
			}
			else if (e is CodeDelegateInvokeExpression) 
			{
				GenerateDelegateInvokeExpression((CodeDelegateInvokeExpression)e);
			}
			else if (e is CodeObjectCreateExpression) 
			{
				GenerateObjectCreateExpression((CodeObjectCreateExpression)e);
			}
			else if (e is CodeParameterDeclarationExpression) 
			{
				GenerateParameterDeclarationExpression((CodeParameterDeclarationExpression)e);
			}
			else if (e is CodeDirectionExpression) 
			{
				GenerateDirectionExpression((CodeDirectionExpression)e);
			}
			else if (e is CodePrimitiveExpression) 
			{
				GeneratePrimitiveExpression((CodePrimitiveExpression)e);
			}
			else if (e is CodePropertyReferenceExpression) 
			{
				GeneratePropertyReferenceExpression((CodePropertyReferenceExpression)e);
			}
			else if (e is CodePropertySetValueReferenceExpression) 
			{
				GeneratePropertySetValueReferenceExpression((CodePropertySetValueReferenceExpression)e);
			}
			else if (e is CodeThisReferenceExpression) 
			{
				GenerateThisReferenceExpression((CodeThisReferenceExpression)e);
			}
			else if (e is CodeTypeReferenceExpression) 
			{
				GenerateTypeReferenceExpression((CodeTypeReferenceExpression)e);
			}
			else if (e is CodeTypeOfExpression) 
			{
				GenerateTypeOfExpression((CodeTypeOfExpression)e);
			}
			else 
			{
				if (e == null) 
				{
					throw new ArgumentNullException("e");
				}
				else 
				{
					//throw new ArgumentException(SR.GetString(SR.InvalidElementType, e.GetType().FullName), "e");
				}
			}
		}

		private void GenerateFields(CodeTypeDeclaration e) 
		{
			IEnumerator en = e.Members.GetEnumerator();
			while (en.MoveNext()) 
			{
				if (en.Current is CodeMemberField) 
				{
					currentMember = (CodeTypeMember)en.Current;

					GenerateRegionStarts(currentMember.Comments);
					CodeMemberField imp = (CodeMemberField)en.Current;
					if (imp.LinePragma != null) GenerateLinePragmaStart(imp.LinePragma);
					GenerateField(imp);
					if (imp.LinePragma != null) GenerateLinePragmaEnd(imp.LinePragma);
					GenerateRegionEnds(currentMember.Comments);
				}
			}
		}

		private void GenerateSnippetMembers(CodeTypeDeclaration e) 
		{
			IEnumerator en = e.Members.GetEnumerator();
			while (en.MoveNext()) 
			{
				if (en.Current is CodeSnippetTypeMember) 
				{
					currentMember = (CodeTypeMember)en.Current;

					if (options.BlankLinesBetweenMembers) 
					{
						Output.WriteLine();
					}
					GenerateRegionStarts(currentMember.Comments);
					CodeSnippetTypeMember imp = (CodeSnippetTypeMember)en.Current;
					if (imp.LinePragma != null) GenerateLinePragmaStart(imp.LinePragma);
					GenerateSnippetMember(imp);
					if (imp.LinePragma != null) GenerateLinePragmaEnd(imp.LinePragma);
					GenerateRegionEnds(currentMember.Comments);

				}
			}
		}

		protected virtual void GenerateSnippetCompileUnit(CodeSnippetCompileUnit e) 
		{
			if (e.LinePragma != null) GenerateLinePragmaStart(e.LinePragma);
			Output.WriteLine(e.Value);
			if (e.LinePragma != null) GenerateLinePragmaEnd(e.LinePragma);
		}

		protected virtual void GenerateMethods(CodeTypeDeclaration e) 
		{
			IEnumerator en = e.Members.GetEnumerator();
			while (en.MoveNext()) 
			{
				if (en.Current is CodeMemberMethod
					&& !(en.Current is CodeTypeConstructor)
					&& !(en.Current is CodeConstructor)) 
				{
					currentMember = (CodeTypeMember)en.Current;

					if (options.BlankLinesBetweenMembers) 
					{
						Output.WriteLine();
					}
					GenerateRegionStarts(currentMember.Comments);
					CodeMemberMethod imp = (CodeMemberMethod)en.Current;
					if (imp.LinePragma != null)GenerateLinePragmaStart(imp.LinePragma);
					if (en.Current is CodeEntryPointMethod) 
					{
						GenerateEntryPointMethod((CodeEntryPointMethod)en.Current, e);
					} 
					else 
					{
						GenerateMethod(imp, e);
					}
					if (imp.LinePragma != null) GenerateLinePragmaEnd(imp.LinePragma);
					GenerateRegionEnds(currentMember.Comments);
				}
			}
		}

		private void GenerateNestedTypes(CodeTypeDeclaration e) 
		{
			IEnumerator en = e.Members.GetEnumerator();
			while (en.MoveNext()) 
			{
				if (en.Current is CodeTypeDeclaration) 
				{
					if (options.BlankLinesBetweenMembers) 
					{
						Output.WriteLine();
					}
					CodeTypeDeclaration currentClass = (CodeTypeDeclaration)en.Current;
					((ICodeGenerator)this).GenerateCodeFromType(currentClass, output.InnerWriter, options);
				}
			}
		}

		protected virtual void GenerateCompileUnit(CodeCompileUnit e) 
		{
			GenerateCompileUnitStart(e);
			GenerateNamespaces(e);
			GenerateCompileUnitEnd(e);
		}

		protected virtual void GenerateNamespace(CodeNamespace e) 
		{
			GenerateNamespaceImports(e);
			Output.WriteLine(string.Empty);
			GenerateRegionStarts(e.Comments);
			GenerateNamespaceStart(e);

			GenerateTypes(e);
			GenerateNamespaceEnd(e);
			GenerateRegionEnds(e.Comments);
		}

		protected virtual void GenerateNamespaceImports(CodeNamespace e) 
		{
			IEnumerator en = e.Imports.GetEnumerator();
			while (en.MoveNext()) 
			{
				CodeNamespaceImport imp = (CodeNamespaceImport)en.Current;
				if (imp.LinePragma != null) GenerateLinePragmaStart(imp.LinePragma);
				GenerateNamespaceImport(imp);
				if (imp.LinePragma != null) GenerateLinePragmaEnd(imp.LinePragma);
			}
		}

		private void GenerateProperties(CodeTypeDeclaration e) 
		{
			IEnumerator en = e.Members.GetEnumerator();
			while (en.MoveNext()) 
			{
				if (en.Current is CodeMemberProperty) 
				{
					currentMember = (CodeTypeMember)en.Current;

					if (options.BlankLinesBetweenMembers) 
					{
						Output.WriteLine();
					}
					GenerateRegionStarts(currentMember.Comments);
					CodeMemberProperty imp = (CodeMemberProperty)en.Current;
					if (imp.LinePragma != null) GenerateLinePragmaStart(imp.LinePragma);
					GenerateProperty(imp, e);
					if (imp.LinePragma != null) GenerateLinePragmaEnd(imp.LinePragma);
					GenerateRegionEnds(currentMember.Comments);
				}
			}
		}

		protected virtual void GenerateStatement(CodeStatement e) 
		{
			if (e.LinePragma != null) 
			{
				GenerateLinePragmaStart(e.LinePragma);
			}

			if (e is CodeCommentStatement) 
			{
				GenerateCommentStatement((CodeCommentStatement)e);
			}
			else if (e is CodeMethodReturnStatement) 
			{
				GenerateMethodReturnStatement((CodeMethodReturnStatement)e);
			}
			else if (e is CodeConditionStatement) 
			{
				GenerateConditionStatement((CodeConditionStatement)e);
			}
			else if (e is CodeTryCatchFinallyStatement) 
			{
				GenerateTryCatchFinallyStatement((CodeTryCatchFinallyStatement)e);
			}
			else if (e is CodeAssignStatement) 
			{
				GenerateAssignStatement((CodeAssignStatement)e);
			}
			else if (e is CodeExpressionStatement) 
			{
				GenerateExpressionStatement((CodeExpressionStatement)e);
			}
			else if (e is CodeIterationStatement) 
			{
				GenerateIterationStatement((CodeIterationStatement)e);
			}
			else if (e is CodeThrowExceptionStatement) 
			{
				GenerateThrowExceptionStatement((CodeThrowExceptionStatement)e);
			}
			else if (e is CodeSnippetStatement) 
			{
				GenerateSnippetStatement((CodeSnippetStatement)e);
			}
			else if (e is CodeVariableDeclarationStatement) 
			{
				GenerateVariableDeclarationStatement((CodeVariableDeclarationStatement)e);
			}
			else if (e is CodeAttachEventStatement) 
			{
				GenerateAttachEventStatement((CodeAttachEventStatement)e);
			}
			else if (e is CodeRemoveEventStatement) 
			{
				GenerateRemoveEventStatement((CodeRemoveEventStatement)e);
			}
			else if (e is CodeGotoStatement) 
			{
				GenerateGotoStatement((CodeGotoStatement)e);
			}
			else if (e is CodeLabeledStatement) 
			{
				GenerateLabeledStatement((CodeLabeledStatement)e);
			}
			else 
			{
				//throw new ArgumentException(SR.GetString(SR.InvalidElementType, e.GetType().FullName), "e");
			}

			if (e.LinePragma != null) 
			{
				GenerateLinePragmaEnd(e.LinePragma);
			}
		}

		protected void GenerateStatements(CodeStatementCollection stms) 
		{
			IEnumerator en = stms.GetEnumerator();
			while (en.MoveNext()) 
			{
				((ICodeGenerator)this).GenerateCodeFromStatement((CodeStatement)en.Current, output.InnerWriter, options);
			}
		}

		protected virtual void OutputAttributeDeclarations(CodeAttributeDeclarationCollection attributes) 
		{
			if (attributes.Count == 0) return;
			GenerateAttributeDeclarationsStart(attributes);
			bool first = true;
			IEnumerator en = attributes.GetEnumerator();
			while (en.MoveNext()) 
			{
				if (first)first = false;
				else ContinueOnNewLine(", ");

				CodeAttributeDeclaration current = (CodeAttributeDeclaration)en.Current;
				Output.Write(current.Name);
				Output.Write("(");

				bool firstArg = true;
				foreach (CodeAttributeArgument arg in current.Arguments) 
				{
					if (firstArg)firstArg = false;
					else Output.Write(", ");

					OutputAttributeArgument(arg);
				}

				Output.Write(")");

			}
			GenerateAttributeDeclarationsEnd(attributes);
		}


		protected virtual void OutputAttributeArgument(CodeAttributeArgument arg) 
		{
			if (!string.IsNullOrEmpty(arg.Name)) 
			{
				OutputIdentifier(arg.Name);
				Output.Write("=");
			}
			((ICodeGenerator)this).GenerateCodeFromExpression(arg.Value, output.InnerWriter, options);
		}


		protected virtual void OutputDirection(FieldDirection dir) 
		{
			switch (dir) 
			{
				case FieldDirection.In:
					break;
				case FieldDirection.Out:
					Output.Write("out ");
					break;
				case FieldDirection.Ref:
					Output.Write("ref ");
					break;
			}
		}


		protected virtual void OutputFieldScopeModifier(MemberAttributes attributes) 
		{
			switch (attributes & MemberAttributes.VTableMask) 
			{
				case MemberAttributes.New:
					Output.Write("new ");
					break;
			}

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

		protected virtual void OutputMemberAccessModifier(MemberAttributes attributes) 
		{
			switch (attributes & MemberAttributes.AccessMask) 
			{
				case MemberAttributes.Assembly:
					Output.Write("internal ");
					break;
				case MemberAttributes.FamilyAndAssembly:
					Output.Write("/*FamANDAssem*/ internal ");
					break;
				case MemberAttributes.Family:
					Output.Write("protected ");
					break;
				case MemberAttributes.FamilyOrAssembly:
					Output.Write("protected internal ");
					break;
				case MemberAttributes.Private:
					Output.Write("private ");
					break;
				case MemberAttributes.Public:
					Output.Write("public ");
					break;
			}
		}

		protected virtual void OutputMemberScopeModifier(MemberAttributes attributes) 
		{
			switch (attributes & MemberAttributes.VTableMask) 
			{
				case MemberAttributes.New:
					Output.Write("new ");
					break;
			}

			switch (attributes & MemberAttributes.ScopeMask) 
			{
				case MemberAttributes.Abstract:
					Output.Write("abstract ");
					break;
				case MemberAttributes.Final:
					Output.Write(string.Empty);
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

		protected abstract void OutputType(CodeTypeReference typeRef);

		protected virtual void OutputTypeAttributes(TypeAttributes attributes, bool isStruct, bool isEnum) 
		{
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
            
			if (isStruct) 
			{
				Output.Write("struct ");
			}
			else if (isEnum) 
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

		protected virtual void OutputTypeNamePair(CodeTypeReference typeRef, string name) 
		{
			OutputType(typeRef);
			Output.Write(" ");
			OutputIdentifier(name);
		}

		protected virtual void OutputIdentifier(string ident) 
		{
			Output.Write(ident);
		}

		protected virtual void OutputExpressionList(CodeExpressionCollection expressions) 
		{
			OutputExpressionList(expressions, false);
		}

		protected virtual void OutputExpressionList(CodeExpressionCollection expressions, bool newlineBetweenItems) 
		{
			bool first = true;
			IEnumerator en = expressions.GetEnumerator();
			Indent++;
			while (en.MoveNext()) 
			{
				if (first) 
				{
					first = false;
				}
				else 
				{
					if (newlineBetweenItems)
						ContinueOnNewLine(",");
					else
						Output.Write(", ");
				}
				((ICodeGenerator)this).GenerateCodeFromExpression((CodeExpression)en.Current, output.InnerWriter, options);
			}
			Indent--;
		}

		protected virtual void OutputOperator(CodeBinaryOperatorType op) 
		{
			switch (op) 
			{
				case CodeBinaryOperatorType.Add:
					Output.Write("+");
					break;
				case CodeBinaryOperatorType.Subtract:
					Output.Write("-");
					break;
				case CodeBinaryOperatorType.Multiply:
					Output.Write("*");
					break;
				case CodeBinaryOperatorType.Divide:
					Output.Write("/");
					break;
				case CodeBinaryOperatorType.Modulus:
					Output.Write("%");
					break;
				case CodeBinaryOperatorType.Assign:
					Output.Write("=");
					break;
				case CodeBinaryOperatorType.IdentityInequality:
					Output.Write("!=");
					break;
				case CodeBinaryOperatorType.IdentityEquality:
					Output.Write("==");
					break;
				case CodeBinaryOperatorType.ValueEquality:
					Output.Write("==");
					break;
				case CodeBinaryOperatorType.BitwiseOr:
					Output.Write("|");
					break;
				case CodeBinaryOperatorType.BitwiseAnd:
					Output.Write("&");
					break;
				case CodeBinaryOperatorType.BooleanOr:
					Output.Write("||");
					break;
				case CodeBinaryOperatorType.BooleanAnd:
					Output.Write("&&");
					break;
				case CodeBinaryOperatorType.LessThan:
					Output.Write("<");
					break;
				case CodeBinaryOperatorType.LessThanOrEqual:
					Output.Write("<=");
					break;
				case CodeBinaryOperatorType.GreaterThan:
					Output.Write(">");
					break;
				case CodeBinaryOperatorType.GreaterThanOrEqual:
					Output.Write(">=");
					break;
			}
		}

		protected virtual void OutputParameters(CodeParameterDeclarationExpressionCollection parameters) 
		{
			bool first = true;
			bool multiline = parameters.Count > ParameterMultilineThreshold;
			if (multiline) 
			{
				Indent += 3;
			}
			IEnumerator en = parameters.GetEnumerator();
			while (en.MoveNext()) 
			{
				CodeParameterDeclarationExpression current = (CodeParameterDeclarationExpression)en.Current;
				if (first) 
				{
					first = false;
				}
				else 
				{
					Output.Write(", ");
				}
				if (multiline) 
				{
					ContinueOnNewLine(string.Empty);
				}
				GenerateExpression(current);
			}
			if (multiline) 
			{
				Indent -= 3;
			}
		}


		protected abstract void GenerateArrayCreateExpression(CodeArrayCreateExpression e);
		protected abstract void GenerateBaseReferenceExpression(CodeBaseReferenceExpression e);
		protected abstract void GenerateCastExpression(CodeCastExpression e);
		protected abstract void GenerateDelegateCreateExpression(CodeDelegateCreateExpression e);
		protected abstract void GenerateFieldReferenceExpression(CodeFieldReferenceExpression e);
		protected abstract void GenerateArgumentReferenceExpression(CodeArgumentReferenceExpression e);
		protected abstract void GenerateVariableReferenceExpression(CodeVariableReferenceExpression e);
		protected abstract void GenerateIndexerExpression(CodeIndexerExpression e);
		protected abstract void GenerateArrayIndexerExpression(CodeArrayIndexerExpression e);
		protected abstract void GenerateSnippetExpression(CodeSnippetExpression e);
		protected abstract void GenerateMethodInvokeExpression(CodeMethodInvokeExpression e);
		protected abstract void GenerateMethodReferenceExpression(CodeMethodReferenceExpression e);
		protected abstract void GenerateEventReferenceExpression(CodeEventReferenceExpression e);
		protected abstract void GenerateDelegateInvokeExpression(CodeDelegateInvokeExpression e);
		protected abstract void GenerateObjectCreateExpression(CodeObjectCreateExpression e);
		protected abstract void GeneratePropertyReferenceExpression(CodePropertyReferenceExpression e);
		protected abstract void GeneratePropertySetValueReferenceExpression(CodePropertySetValueReferenceExpression e);
		protected abstract void GenerateThisReferenceExpression(CodeThisReferenceExpression e);
		protected abstract void GenerateExpressionStatement(CodeExpressionStatement e);
		protected abstract void GenerateIterationStatement(CodeIterationStatement e);
		protected abstract void GenerateThrowExceptionStatement(CodeThrowExceptionStatement e);
		protected abstract void GenerateComment(CodeComment e);
		protected abstract void GenerateRegionStart(StiCodeRegionStart e);
		protected abstract void GenerateRegionEnd(StiCodeRegionEnd e);
		protected abstract void GenerateMethodReturnStatement(CodeMethodReturnStatement e);
		protected abstract void GenerateConditionStatement(CodeConditionStatement e);
		protected abstract void GenerateTryCatchFinallyStatement(CodeTryCatchFinallyStatement e);
		protected abstract void GenerateAssignStatement(CodeAssignStatement e);
		protected abstract void GenerateAttachEventStatement(CodeAttachEventStatement e);
		protected abstract void GenerateRemoveEventStatement(CodeRemoveEventStatement e);
		protected abstract void GenerateGotoStatement(CodeGotoStatement e);
		protected abstract void GenerateLabeledStatement(CodeLabeledStatement e);
		protected abstract void GenerateVariableDeclarationStatement(CodeVariableDeclarationStatement e);
		protected abstract void GenerateLinePragmaStart(CodeLinePragma e);
		protected abstract void GenerateLinePragmaEnd(CodeLinePragma e);
		protected abstract void GenerateEvent(CodeMemberEvent e, CodeTypeDeclaration c);
		protected abstract void GenerateField(CodeMemberField e);
		protected abstract void GenerateSnippetMember(CodeSnippetTypeMember e);
		protected abstract void GenerateEntryPointMethod(CodeEntryPointMethod e, CodeTypeDeclaration c);
		protected abstract void GenerateMethod(CodeMemberMethod e, CodeTypeDeclaration c);
		protected abstract void GenerateProperty(CodeMemberProperty e, CodeTypeDeclaration c);
		protected abstract void GenerateConstructor(CodeConstructor e, CodeTypeDeclaration c);
		protected abstract void GenerateTypeConstructor(CodeTypeConstructor e);
		protected abstract void GenerateTypeStart(CodeTypeDeclaration e);
		protected abstract void GenerateTypeEnd(CodeTypeDeclaration e);
		protected abstract void GenerateNamespaceStart(CodeNamespace e);
		protected abstract void GenerateNamespaceEnd(CodeNamespace e);
		protected abstract void GenerateNamespaceImport(CodeNamespaceImport e);
		protected abstract void GenerateAttributeDeclarationsStart(CodeAttributeDeclarationCollection attributes);
		protected abstract void GenerateAttributeDeclarationsEnd(CodeAttributeDeclarationCollection attributes);
		
		public abstract string GetBaseTypeOutput(string baseType);
		
		public abstract string GetRegionStartWord();
		public abstract string GetRegionEndWord();

		public abstract string GetRegionStart(string s);
		public abstract string GetRegionEnd(string s);
		
		protected virtual void GenerateTypeReferenceExpression(CodeTypeReferenceExpression e) 
		{
			OutputType(e.Type);
		}

		protected virtual void GenerateTypeOfExpression(CodeTypeOfExpression e) 
		{
			Output.Write("typeof(");
			OutputType(e.Type);
			Output.Write(")");
		}

		protected virtual void GenerateSnippetStatement(CodeSnippetStatement e) 
		{
			Output.WriteLine(e.Value);
		}
		
		protected virtual void GenerateCompileUnitStart(CodeCompileUnit e) 
		{
		}
		protected virtual void GenerateCompileUnitEnd(CodeCompileUnit e) 
		{
		}

		protected virtual void GenerateCommentStatement(CodeCommentStatement e) 
		{
			GenerateComment(e.Comment);
		}

		protected virtual void GenerateRegionStarts(CodeCommentStatementCollection e) 
		{
			foreach (CodeCommentStatement comment in e)
			{
				object obj = comment;
				if (obj is StiCodeRegionStart)GenerateRegionStart(obj as StiCodeRegionStart);
				else if (!(obj is StiCodeRegionEnd))GenerateCommentStatement(comment);
			}
		}

		protected virtual void GenerateRegionEnds(CodeCommentStatementCollection e) 
		{
			foreach (CodeCommentStatement comment in e) 
			{
				object obj = comment;
				if (obj is StiCodeRegionEnd)GenerateRegionEnd(obj as StiCodeRegionEnd);
			}
		}

		protected virtual void GenerateParameterDeclarationExpression(CodeParameterDeclarationExpression e) 
		{
			if (e.CustomAttributes.Count > 0) 
			{
				OutputAttributeDeclarations(e.CustomAttributes);
				Output.Write(" ");
			}

			OutputDirection(e.Direction);
			OutputTypeNamePair(e.Type, e.Name);
		}

		protected virtual void GenerateDirectionExpression(CodeDirectionExpression e) 
		{
			OutputDirection(e.Direction);
			GenerateExpression(e.Expression);
		}

		protected virtual void GeneratePrimitiveExpression(CodePrimitiveExpression e) 
		{
			if (e.Value == null) 
			{
				Output.Write(NullToken);
			}
			else if (e.Value is string) 
			{
				Output.Write(QuoteSnippetString((string)e.Value));
			}
			else if (e.Value is char) 
			{
				Output.Write("'" + e.Value.ToString() + "'");
			}
			else if (e.Value is byte) 
			{
				Output.Write(((byte)e.Value).ToString());
			}
			else if (e.Value is Int16) 
			{
				Output.Write(((Int16)e.Value).ToString());
			}
			else if (e.Value is Int32) 
			{
				Output.Write(((Int32)e.Value).ToString());
			}
			else if (e.Value is Int64) 
			{
				Output.Write(((Int64)e.Value).ToString());
			}
			else if (e.Value is Single) 
			{
				GenerateSingleFloatValue((Single)e.Value);
			}
			else if (e.Value is Double) 
			{
				GenerateDoubleValue((Double)e.Value);
			}
			else if (e.Value is Decimal) 
			{
				GenerateDecimalValue((Decimal)e.Value);
			}
			else if (e.Value is bool) 
			{
				if ((bool)e.Value) 
				{
					Output.Write("true");
				}
				else 
				{
					Output.Write("false");
				}
			}
			else 
			{
				//throw new ArgumentException(SR.GetString(SR.InvalidPrimitiveType, e.Value.GetType().ToString()));
			}
		}

		protected virtual void GenerateSingleFloatValue(Single s) 
		{
			Output.Write(s.ToString(CultureInfo.InvariantCulture));
		}

		protected virtual void GenerateDoubleValue(Double d) 
		{
			Output.Write(d.ToString("R", CultureInfo.InvariantCulture));
		}

		protected virtual void GenerateDecimalValue(Decimal d) 
		{
			Output.Write(d.ToString(CultureInfo.InvariantCulture));
		}

		protected virtual void GenerateBinaryOperatorExpression(CodeBinaryOperatorExpression e) 
		{
			bool indentedExpression = false;
			Output.Write("(");

			GenerateExpression(e.Left);
			Output.Write(" ");

			if (e.Left is CodeBinaryOperatorExpression || e.Right is CodeBinaryOperatorExpression) 
			{
				// In case the line gets too long with nested binary operators, we need to output them on
				// different lines. However we want to indent them to maintain readability, but this needs
				// to be done only once;
				if (!inNestedBinary) 
				{
					indentedExpression = true;
					inNestedBinary = true;
					Indent += 3;
				}
				ContinueOnNewLine(string.Empty);
			}
 
			OutputOperator(e.Operator);

			Output.Write(" ");
			GenerateExpression(e.Right);

			Output.Write(")");
			if (indentedExpression) 
			{
				Indent -= 3;
				inNestedBinary = false;
			}
		}

		protected virtual void ContinueOnNewLine(string st) 
		{
			Output.WriteLine(st);
		}


		protected abstract bool Supports(GeneratorSupport support);
		protected abstract bool IsValidIdentifier(string value);

		public abstract bool IsKeyword(string value);
		public abstract string GetClassKeyword();
		
		protected virtual void ValidateIdentifier(string value) 
		{
			if (!IsValidIdentifier(value)) 
			{
				//throw new ArgumentException(SR.GetString(SR.InvalidIdentifier, value));
			}
		}

		protected abstract string CreateEscapedIdentifier(string value);
		protected abstract string CreateValidIdentifier(string value);
		public abstract string QuoteSnippetString(string value);

		public static bool IsValidLanguageIndependentIdentifier(string value)
		{
			char[] chars = value.ToCharArray();

			if (chars.Length == 0)return false;

			if (Char.GetUnicodeCategory(chars[0]) == UnicodeCategory.DecimalDigitNumber)return false;

			foreach (char ch in chars) 
			{
				UnicodeCategory uc = Char.GetUnicodeCategory(ch);
				switch (uc) 
				{
					case UnicodeCategory.UppercaseLetter:        // Lu
					case UnicodeCategory.LowercaseLetter:        // Ll
					case UnicodeCategory.TitlecaseLetter:        // Lt
					case UnicodeCategory.ModifierLetter:         // Lm
					case UnicodeCategory.OtherLetter:            // Lo
					case UnicodeCategory.DecimalDigitNumber:     // Nd
					case UnicodeCategory.NonSpacingMark:         // Mn
					case UnicodeCategory.SpacingCombiningMark:   // Mc
					case UnicodeCategory.ConnectorPunctuation:   // Pc
						break;
					default:
						return false;
				}
			}

			return true;
		}

	}
}
