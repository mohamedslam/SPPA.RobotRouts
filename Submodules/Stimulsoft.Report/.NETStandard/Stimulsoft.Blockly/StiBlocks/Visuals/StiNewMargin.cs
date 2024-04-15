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
using Stimulsoft.Report.Components;
using System;

namespace Stimulsoft.Blockly.StiBlocks.Visuals
{
    public class StiNewMargin : IronBlock
    {
        #region Methods
        public override object Evaluate(Context context)
        {
            var left = Convert.ToDouble(this.Values.Evaluate("LEFT", context));
            var top = Convert.ToDouble(this.Values.Evaluate("TOP", context));
            var right = Convert.ToDouble(this.Values.Evaluate("RIGHT", context));
            var bottom = Convert.ToDouble(this.Values.Evaluate("BOTTOM", context));

            return new StiMargins(left, right, top, bottom);
        } 
        #endregion
    }
}