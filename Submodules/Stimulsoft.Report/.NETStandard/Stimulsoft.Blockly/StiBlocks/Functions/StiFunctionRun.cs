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
using Stimulsoft.Blockly.Model;
using System;
using System.Collections.Generic;
using Stimulsoft.Blockly.Blocks;

namespace Stimulsoft.Blockly.StiBlocks.Functions
{
    public class StiFunctionRun : IronBlock
    {
        public override object Evaluate(Context context)
        {
            var function = StiBlocklyFunctionBlockKeyCache.GetFunction(this.Type);

            var argValues = new List<object>();

            for (int index = 0; index < function.ArgumentNames.Length; index++)
            {
                var arg = function.ArgumentNames[index];
                var type = function.ArgumentTypes[index];
                var value = this.Values.Evaluate(arg, context);

                var valueArg = value is IConvertible
                    ? Convert.ChangeType(value, type)
                    : value;

                argValues.Add(valueArg);
            }

            return ((IStiAppFunction)function).Invoke(argValues.ToArray());
        }
    }
}
