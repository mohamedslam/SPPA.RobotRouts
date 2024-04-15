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

using Stimulsoft.Blockly.Blocks;
using Stimulsoft.Blockly.Model;
using System;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Blockly.StiBlocks.Visuals
{
    public class StiNewFont : IronBlock
    {
        #region Methods
        public override object Evaluate(Context context)
        {
            var name = this.Fields.Get("NAME").ToString();
            var size = (float)Convert.ToDouble(this.Values.Evaluate("SIZE", context));

            var bold = Convert.ToBoolean(this.Fields.Get("BOLD"));
            var italic = Convert.ToBoolean(this.Fields.Get("ITALIC"));
            var underline = Convert.ToBoolean(this.Fields.Get("UNDERLINE"));
            var strikeout = Convert.ToBoolean(this.Fields.Get("STRIKEOUT"));

            var fontStyle = FontStyle.Regular;

            if (bold)
                fontStyle |= FontStyle.Bold;

            if (italic)
                fontStyle |= FontStyle.Italic;

            if (underline)
                fontStyle |= FontStyle.Underline;

            if (strikeout)
                fontStyle |= FontStyle.Strikeout;


            return new Font(name, size, fontStyle);
        } 
        #endregion
    }
}
