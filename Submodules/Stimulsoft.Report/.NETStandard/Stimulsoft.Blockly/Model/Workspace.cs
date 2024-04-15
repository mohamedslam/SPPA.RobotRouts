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

using Stimulsoft.Blockly.Blocks.Procedures;
using System.Collections.Generic;

namespace Stimulsoft.Blockly.Model
{
    public class Workspace : IFragment
    {
        public Workspace()
        {
            this.Blocks = new List<IronBlock>();
        }

        public IList<IronBlock> Blocks { get; set; }

        public virtual object Evaluate(Context context)

        {
            // TODO: variables
            object returnValue = null;

            // first process procedure def blocks
            var processedProcedureDefBlocks = new List<IronBlock>();
            foreach (IronBlock block in this.Blocks)
            {
                if (block is ProceduresDef)
                {
                    block.Evaluate(context);
                    processedProcedureDefBlocks.Add(block);
                }
            }

            foreach (var block in this.Blocks)
            {
                if (!processedProcedureDefBlocks.Contains(block))
                    returnValue = block.Evaluate(context);
            }

            return returnValue;
        }
    }
}
