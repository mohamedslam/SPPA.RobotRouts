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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Blockly.Blocks;
using Stimulsoft.Blockly.Model;

namespace Stimulsoft.Blockly.StiBlocks.Visuals
{
    public class StiNewCornerRadius : IronBlock
    {
        #region Methods
        public override object Evaluate(Context context)
        {
            
            var topLeft = StiConvert.ChangeType(this.Values.Evaluate("TOPLEFT", context), typeof(float));
            var topRight = StiConvert.ChangeType(this.Values.Evaluate("TOPRIGHT", context), typeof(float));
            var bottomRight = StiConvert.ChangeType(this.Values.Evaluate("BOTTOMRIGHT", context), typeof(float));
            var bottomLeft = StiConvert.ChangeType(this.Values.Evaluate("BOTTOMLEFT", context), typeof(float));

            return new StiCornerRadius((float)topLeft, (float)topRight, (float)bottomRight, (float)bottomLeft);
        } 
        #endregion
    }
}