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
using System.Globalization;
using System.Linq;

namespace Stimulsoft.Blockly.Blocks.Text
{
    public class TextCaseChange : IronBlock
    {
        public override object Evaluate(Context context)
        {
            var toCase = this.Fields.Get("CASE").ToString();
            var text = (this.Values.Evaluate("TEXT", context) ?? "").ToString();

            switch (toCase)
            {
                case "UPPERCASE":
                    return text.ToUpper();

                case "LOWERCASE":
                    return text.ToLower();

                case "TITLECASE":
                    {
                        if (text.Replace(" ", "").All(char.IsUpper))
                            text = text.ToLower();

                        var textInfo = new CultureInfo("en-US", false).TextInfo;
                        return textInfo.ToTitleCase(text);
                    }

                default:
                    throw new NotSupportedException("unknown case");

            }

        }
    }
}