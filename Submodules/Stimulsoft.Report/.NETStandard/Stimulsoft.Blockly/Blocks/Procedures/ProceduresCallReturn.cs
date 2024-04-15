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

using Stimulsoft.Blockly.Model;
using System;
using System.Linq;

namespace Stimulsoft.Blockly.Blocks.Procedures
{
    public class ProceduresCallReturn : ProceduresCallNoReturn
    {
        public override object Evaluate(Context context)
        {
            // todo: add guard for missing name

            var name = this.Mutations.GetValue("name");

            if (!context.Functions.ContainsKey(name)) throw new MissingMethodException($"Method '{name}' not defined");

            var statement = (IFragment)context.Functions[name];

            var funcContext = new Context() { Parent = context };
            funcContext.Functions = context.Functions;

            var counter = 0;
            foreach (var mutation in this.Mutations.Where(x => x.Domain == "arg" && x.Name == "name"))
            {
                var value = this.Values.Evaluate($"ARG{counter}", context);
                funcContext.Variables.Add(mutation.Value, value);
                counter++;
            }

            return statement.Evaluate(funcContext);
        }
    }
}