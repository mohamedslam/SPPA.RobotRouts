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
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Blockly.Blocks.Lists
{
    public class ListsSplit : IronBlock
    {
        public override object Evaluate(Context context)
        {
            var mode = this.Fields.Get("MODE");
            var input = this.Values.Evaluate("INPUT", context);
            var delim = this.Values.Evaluate("DELIM", context);

            switch (mode)
            {
                case "SPLIT":
                    return input
                        .ToString()
                        .Split(new string[] {delim.ToString() }, StringSplitOptions.None)
						.Select(x => x as object)
                        .ToList();

                case "JOIN":
                    return string
                        .Join(delim.ToString(), (input as IEnumerable<object>)
						.Select(x => x.ToString()));

                default:
                    throw new NotSupportedException($"unknown mode: {mode}");

            }
        }
	}
}