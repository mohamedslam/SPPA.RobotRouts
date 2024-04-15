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

using System.CodeDom;
using Stimulsoft.Report.Dictionary;

namespace Stimulsoft.Report.CodeDom
{
	internal class StiCodeDomResources
	{
		internal static void Serialize(StiCodeDomSerializator serializator, StiReport report)
		{
		    if (report.Dictionary.Resources.Count <= 0) return;

		    foreach (StiResource resource in report.Dictionary.Resources)
		    {
		        if (string.IsNullOrEmpty(resource.Name)) continue;

                var expr = new CodeObjectCreateExpression(resource.GetType(),
                        serializator.GetArguments(resource.GetType(), resource));

                serializator.memberMethod.Statements.Add(new CodeMethodInvokeExpression(
                    new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(
                    new CodeThisReferenceExpression(), "Dictionary"), "Resources"),
                    "Add", expr
                    ));
		    }
		}
	}
}
