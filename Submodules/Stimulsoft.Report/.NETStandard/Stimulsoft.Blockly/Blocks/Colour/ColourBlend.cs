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
    public class ColourBlend : IronBlock
    {
        #region Methods
        public override object Evaluate(Context context)
        {
            var colour1 = (this.Values.Evaluate("COLOUR1", context) ?? "").ToString();
            var colour2 = (this.Values.Evaluate("COLOUR2", context) ?? "").ToString();
            var ratio = Math.Min(Math.Max((double)this.Values.Evaluate("RATIO", context), 0), 1);

            if (string.IsNullOrWhiteSpace(colour1) || colour1.Length != 7) return null;
            if (string.IsNullOrWhiteSpace(colour2) || colour2.Length != 7) return null;

            var red = (byte)((double)Convert.ToByte(colour1.Substring(1, 2), 16) * (1 - ratio) + (double)Convert.ToByte(colour2.Substring(1, 2), 16) * ratio);
            var green = (byte)((double)Convert.ToByte(colour1.Substring(3, 2), 16) * (1 - ratio) + (double)Convert.ToByte(colour2.Substring(3, 2), 16) * ratio);
            var blue = (byte)((double)Convert.ToByte(colour1.Substring(5, 2), 16) * (1 - ratio) + (double)Convert.ToByte(colour2.Substring(5, 2), 16) * ratio);

            return $"#{red:x2}{green:x2}{blue:x2}";
        }
        #endregion
    }
}