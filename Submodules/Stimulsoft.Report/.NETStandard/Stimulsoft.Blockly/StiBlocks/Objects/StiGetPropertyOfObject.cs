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

namespace Stimulsoft.Blockly.StiBlocks.Objects
{
    public class StiGetPropertyOfObject : IronBlock
    {
        #region Methods
        public override object Evaluate(Context context)
        {
            try
            {
                var obj = this.Values.Evaluate("OBJECT", context);
                if (obj != null)
                {
                    var propertyName = this.Values.Evaluate("PROPERTY", context).ToString();
                    var propertyNames = propertyName.Split('.');

                    return GetValue(obj, propertyNames);
                }
            }
            catch { }

            return base.Evaluate(context);
        } 

        private object GetValue(object baseObj, string[] properties)
        {
            object valueObj = null;

            for (var index = 0; index < properties.Length; index++)
            {
                if (index == 0)
                    valueObj = baseObj;

                var propertyName = properties[index];
                valueObj = valueObj.GetType().GetProperty(propertyName).GetValue(valueObj, null);

            }
            return valueObj;
        }
        #endregion
    }
}
