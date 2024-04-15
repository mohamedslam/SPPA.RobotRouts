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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using System;
using System.Drawing;
using Stimulsoft.Blockly.Blocks;
using Stimulsoft.Blockly.Model;

namespace Stimulsoft.Blockly.StiBlocks.Objects
{
    public class StiSetPropertyToValue : IronBlock
    {
        public override object Evaluate(Context context)
        {
            if (this.Values.Evaluate("COMPONENT", context) is StiComponent component)
            {
                var dropdownProperty = this.Fields.Get("PROPERTY");

                var value = this.Values.Evaluate("VALUE", context);

                switch (dropdownProperty)
                {
                    case "ENABLED":
                        component.Enabled = Convert.ToBoolean(value);
                        break;

                    case "BACKGROUND":
                        if (component is IStiBrush brushComponent && value != null)
                        {
                            var color = ColorTranslator.FromHtml(value.ToString());
                            brushComponent.Brush = new StiSolidBrush(color);
                        }
                        break;

                    case "FONT":
                        if (component is IStiFont fontComponent && value != null)
                        {

                        }
                        break;

                    //case "STYLE":
                    //    if (component is StiComponent fontComponent && value != null)
                    //    {

                    //    }
                    //    break;

                    case "WIDTH":
                        component.Width = Convert.ToDouble(value);
                        break;

                    case "HEIGHT":
                        component.Height = Convert.ToDouble(value);
                        break;
                }

                component.Report.Invalidate();
            }            

            return base.Evaluate(context);
        }

    }
}
