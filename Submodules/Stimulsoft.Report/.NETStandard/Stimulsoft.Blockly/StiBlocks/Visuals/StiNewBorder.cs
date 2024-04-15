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
using Stimulsoft.Base.Drawing;
using System;
using System.Drawing;

namespace Stimulsoft.Blockly.StiBlocks.Visuals
{
    public class StiNewBorder : IronBlock
    {
        #region Methods
        public override object Evaluate(Context context)
        {
            var color = ColorTranslator.FromHtml(this.Values.Evaluate("COLOR", context).ToString());
            var size = Convert.ToDouble(this.Values.Evaluate("SIZE", context));
            var penStyle = (StiPenStyle)this.Values.Evaluate("STYLE", context);

            var top = Convert.ToBoolean(this.Fields.Get("TOP"));
            var left = Convert.ToBoolean(this.Fields.Get("LEFT"));
            var right = Convert.ToBoolean(this.Fields.Get("RIGHT"));
            var bottom = Convert.ToBoolean(this.Fields.Get("BOTTOM"));

            var borderSides = StiBorderSides.None;

            if (top)
                borderSides |= StiBorderSides.Top;

            if (left)
                borderSides |= StiBorderSides.Left;

            if (right)
                borderSides |= StiBorderSides.Right;

            if (bottom)
                borderSides |= StiBorderSides.Bottom;

            return new StiBorder(borderSides, color, size, penStyle);
        } 
        #endregion
    }
}
