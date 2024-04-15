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

namespace Stimulsoft.Blockly.Blocks.Text
{
    public class TextIndexOf : IronBlock
    {
        public override object Evaluate(Context context)
        {
            var mode = this.Fields.Get("END");

            var text = (this.Values.Evaluate("VALUE", context) ?? "").ToString();
            var term = (this.Values.Evaluate("FIND", context) ?? "").ToString();

            switch (mode)
            {
                case "FIRST": return StiObjConverter.ToDouble(text.IndexOf(term)) + 1;
                case "LAST": return StiObjConverter.ToDouble(text.LastIndexOf(term)) + 1;
                default: throw new ApplicationException("unknown mode");
            }
        }
    }
}