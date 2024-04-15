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
using System.Drawing;
using System;

namespace Stimulsoft.Blockly.StiBlocks.Visuals
{
    public class StiNewBrush : IronBlock
    {
        #region Methods
        public override object Evaluate(Context context)
        {
            var value = this.Fields.Get("VALUE").ToLower();

            switch (value)
            {
                case "from style":
                    return new StiStyleBrush();

                case "empty":
                    return new StiEmptyBrush();

                case "solid":
                    {
                        var color = this.Values.Evaluate("COLOR", context).ToString();
                        return new StiSolidBrush(ColorTranslator.FromHtml(color));
                    }

                case "gradient":
                    {
                        var startColor = this.Values.Evaluate("STARTCOLOR", context).ToString();
                        var endColor = this.Values.Evaluate("ENDCOLOR", context).ToString();
                        var angle = this.Values.Evaluate("ANGLE", context);

                        return new StiGradientBrush(ColorTranslator.FromHtml(startColor), ColorTranslator.FromHtml(endColor), Convert.ToDouble(angle));
                    }
            }

            return new StiEmptyBrush();
        } 
        #endregion
    }
}
