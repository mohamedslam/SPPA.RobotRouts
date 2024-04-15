﻿#region Copyright (C) 2003-2022 Stimulsoft
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
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;

namespace Stimulsoft.Blockly.StiBlocks.Data
{
    public class StiSetDataSourceSqlCommand : IronBlock
    {
        #region Methods
        public override object Evaluate(Context context)
        {
            if (this.Values.Evaluate("DATA", context) is StiSqlSource sqlSource)
            {
                var value = this.Values.Evaluate("VALUE", context).ToString();

                var isInterpretationMode = context.Report?.CalculationMode == StiCalculationMode.Interpretation;

                if (isInterpretationMode)
                {
                    context.Report.Variables["**StoredDataSourceSqlCommandForInterpretationMode**" + sqlSource.Name] = value;
                }
                else
                {
                    sqlSource.SqlCommand = value;
                }
            }

            return base.Evaluate(context);
        } 
        #endregion
    }
}