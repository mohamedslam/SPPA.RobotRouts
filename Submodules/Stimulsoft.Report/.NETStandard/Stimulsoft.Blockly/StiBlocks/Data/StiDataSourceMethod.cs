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
using Stimulsoft.Blockly.Blocks;

namespace Stimulsoft.Blockly.StiBlocks.Data
{
    public class StiDataSourceMethod : IronBlock
    {
        #region Methods
        public override object Evaluate(Context context)
        {
            var obj = this.Values.Evaluate("OBJECT", context);
            if (obj != null)
            {
                var methodName = this.Fields.Get("NAME").ToString();

                var method = obj.GetType().GetMethod(methodName);
                return method.Invoke(obj, new object[] { });
            }

            return base.Evaluate(context);
        }
        #endregion
    }
}
