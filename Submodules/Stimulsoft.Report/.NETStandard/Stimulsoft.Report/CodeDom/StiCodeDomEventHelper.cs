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
using System.IO;
using System.Text;
using System.CodeDom.Compiler;
using System.CodeDom;
using Stimulsoft.Report.Events;

namespace Stimulsoft.Report.CodeDom
{
	/// <summary>
	/// Describes the class which helps generate GetEventMethod for CodeDom serializator.
	/// </summary>
	public class StiCodeDomEventHelper
	{
		#region Methods
		/// <summary>
		/// Returns body of event handlers.
		/// </summary>
		public static string GetEventMethod(StiEvent ev, StiReport report)
		{
			var generator =  report.Language.GetProvider().CreateGenerator() as StiCodeGenerator;

			var sb = new StringBuilder();
			var sw = new StringWriter(sb);

		    var generatorOptions = new CodeGeneratorOptions
		    {
		        BlankLinesBetweenMembers = true,
		        BracingStyle = "C"
		    };

		    var eventMethod = new CodeMemberMethod
		    {
		        Attributes = MemberAttributes.Public | MemberAttributes.Final,
		        Name = ev.Script
		    };

		    var parametersCollection = 
				new CodeParameterDeclarationExpressionCollection();

			var parameters = ev.GetParameters();

			foreach (var parameter in parameters)
			{
				var cp = new CodeParameterDeclarationExpression(parameter.Type, parameter.Name);
					
				parametersCollection.Add(cp);
			}

			eventMethod.Parameters.AddRange(parametersCollection);


		    var typeDeclaration = new CodeTypeDeclaration("Sample")
		    {
		        IsClass = true
		    };

		    eventMethod.Comments.Add(new StiCodeRegionStart("Sample"));
			eventMethod.Comments.Add(new StiCodeRegionEnd("Sample"));

			typeDeclaration.Members.Add(eventMethod);
			eventMethod.Statements.Add(new CodeCommentStatement(""));

			generator.GenerateCodeFromType(typeDeclaration, sw, generatorOptions);

			string s = sb.ToString();

			string start = generator.GetRegionStart("Sample");
			string end = generator.GetRegionEnd("Sample");

            int indexStart = s.IndexOf(start, StringComparison.InvariantCulture);
								
			s = s.Remove(0, indexStart + start.Length);
            int indexEnd = s.IndexOf(end, StringComparison.InvariantCulture);
			s = s.Remove(indexEnd, s.Length - indexEnd);

			sw.Flush();
			sw.Close();
            
			return s;
		}
		#endregion
	}
}
