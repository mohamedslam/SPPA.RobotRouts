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
using Stimulsoft.Report.Events;

namespace Stimulsoft.Report.CodeDom
{
	internal class StiCodeDomDatabases
	{
		internal static void Serialize(StiCodeDomSerializator serializator, StiReport report)
		{
			if (report.Dictionary.Databases.Count > 0)
			{
				foreach (StiDatabase database in report.Dictionary.Databases)
				{
					#region this.Dictionary.Database.Add(data)
					CodeExpression expr = new CodeObjectCreateExpression(database.GetType(), 
						serializator.GetArguments(database.GetType(), database));

					serializator.memberMethod.Statements.Add(new CodeMethodInvokeExpression(
						new CodePropertyReferenceExpression(
						new CodePropertyReferenceExpression(
						new CodeThisReferenceExpression(), "Dictionary"), "Databases"),
						"Add", expr
						));

                    if ((database is StiSqlDatabase) && !string.IsNullOrWhiteSpace(database.Name) && !string.IsNullOrWhiteSpace((database as StiSqlDatabase).ConnectionStringEncrypted))
                    {
                        serializator.memberMethod.Statements.Add(new CodeAssignStatement(
                            new CodePropertyReferenceExpression(
                            new CodeCastExpression(typeof(StiSqlDatabase),
                            new CodeIndexerExpression(
                            new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                            new CodeThisReferenceExpression(), "Dictionary"), "Databases"), new CodePrimitiveExpression(database.Name))), "ConnectionStringEncrypted"),
                            new CodePrimitiveExpression((database as StiSqlDatabase).ConnectionStringEncrypted)));
                    }
					#endregion

                    AddEvent(serializator, report, database.Name, database.ConnectingEvent);
                    AddEvent(serializator, report, database.Name, database.ConnectedEvent);
                    AddEvent(serializator, report, database.Name, database.DisconnectingEvent);
                    AddEvent(serializator, report, database.Name, database.DisconnectedEvent);
				}				
			}			
		}

        internal static void AddEvent(StiCodeDomSerializator serializator, StiReport report, 
            string databaseName, StiEvent ev)
        {
            if (ev.Script.Length <= 0) return;

            var text = ev.Script;
            text = StiCodeDomFunctions.ParseFunctions(serializator, text);

            string eventName;

            if (databaseName.Length > 0)
                eventName = databaseName + "_" + ev;
            else
                eventName = report.GetReportName() + "_" + ev;

            var addScript = report.ScriptLanguage == 
                StiReportLanguageType.CSharp || report.ScriptLanguage == StiReportLanguageType.JS
                    ? $"Dictionary.Databases[\"{databaseName}\"]"
                    : $"Dictionary.Databases.Item(\"{databaseName}\")";

            serializator.GenAddEvent(eventName, addScript, ev);
            serializator.GenEventMethod(eventName, text, ev, databaseName);
        }
	}
}
