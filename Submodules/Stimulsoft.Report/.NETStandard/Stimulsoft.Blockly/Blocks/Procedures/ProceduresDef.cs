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
using System.Linq;

namespace Stimulsoft.Blockly.Blocks.Procedures
{
    public class ProceduresDef : IronBlock
    {
        public override object Evaluate(Context context)
        {
            var name = this.Fields.Get("NAME");
            var statement = this.Statements.FirstOrDefault(x => x.Name == "STACK");

            if (string.IsNullOrWhiteSpace(name)) return null;

            // if the statement is missing, create a stub one
            if (null == statement)
            {
                statement = new Statement
                {
                    Block = null,
                    Name = "STACK"
                };
            }

            // tack the return value on as a block at the end of the statement
            if (this.Values.Any(x => x.Name == "RETURN"))
            {
                var valueBlock = new ValueBlock(this.Values.First(x => x.Name == "RETURN"));
                if (statement.Block == null)
                {
                    statement.Block = valueBlock;
                }
                else
                {
                    FindEndOfChain(statement.Block).Next = valueBlock;
                }
            }

            if (context.Functions.ContainsKey(name))
            {
                context.Functions[name] = statement;
            }
            else
            {
                context.Functions.Add(name, statement);
            }

            return null;
        }

        static IronBlock FindEndOfChain(IronBlock block)
        {
            if (null == block.Next) return block;
            return FindEndOfChain(block.Next);
        }        
    }
}