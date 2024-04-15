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
using Stimulsoft.Base;
using Stimulsoft.Blockly.Model;
using Stimulsoft.Report.Components;
using System.Reflection;
using System.Drawing;
using Stimulsoft.Report;

namespace Stimulsoft.Blockly.StiBlocks.Objects
{
    public class StiSetPropertyOfObjectTo : IronBlock
    {
        #region Methods
        public override object Evaluate(Context context)
        {
            var obj = this.Values.Evaluate("OBJECT", context);

            if (obj != null)
            {
                var propertyName = this.Values.Evaluate("PROPERTY", context).ToString();
                var propertyNames = propertyName.Split('.');

                var value = this.Values.Evaluate("VALUE", context);

                SetValue(obj, propertyNames, value);
            }

            context.Report.Invalidate();

            return base.Evaluate(context);
        } 

        private void SetValue(object baseObj, string[] properties, object value)
        {
            for (var index = 0; index < properties.Length; index++)
            {
                var propertyName = properties[index];

                if (index == properties.Length - 1)
                {
                    var prop = baseObj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    if (null != prop && prop.CanWrite)
                    {
                        if (value is StiExpression exp)
                            value = exp.Value;

                        if (prop.PropertyType == typeof(StiExpression))
                        {
                            var textValue = (string)StiConvert.ChangeType(value, typeof(string));
                            value = new StiExpression() { Value = textValue };
                        }

                        else if (prop.PropertyType == typeof(Color) && value is string valueText)
                        {
                            value = StiReportObjectStringConverter.ConvertStringToColor(valueText);
                        }

                        var valueType = StiConvert.ChangeType(value, prop.PropertyType);

                        prop.SetValue(baseObj, valueType, null);
                    }
                }

                else
                {
                    baseObj = baseObj.GetType().GetProperty(propertyName).GetValue(baseObj, null);
                }

            }
        }
        #endregion
    }
}
