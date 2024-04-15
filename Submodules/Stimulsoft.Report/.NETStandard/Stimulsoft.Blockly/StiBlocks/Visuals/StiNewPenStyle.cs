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

namespace Stimulsoft.Blockly.StiBlocks.Visuals
{
    public class StiNewPenStyle : IronBlock
    {
        #region Methods
        public override object Evaluate(Context context)
        {
            var value = this.Fields.Get("VALUE");

            switch (value)
            {
                case "SOLID":
                    return StiPenStyle.Solid;

                case "DASH":
                    return StiPenStyle.Dash;

                case "DASHDOT":
                    return StiPenStyle.DashDot;

                case "DASHDOTDOT":
                    return StiPenStyle.DashDotDot;

                case "DOT":
                    return StiPenStyle.Dot;

                case "DOUBLE":
                    return StiPenStyle.Double;

                case "NONE":
                    return StiPenStyle.None;
            }

            return StiPenStyle.Solid;
        } 
        #endregion
    }
}
