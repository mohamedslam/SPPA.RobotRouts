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

using Stimulsoft.Base.Blockly;
using Stimulsoft.Base.Blocks;
using Stimulsoft.Report.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stimulsoft.Report.Helpers
{
    internal static class StiBlocklyHelper
    {
        #region Methods
        public static void InvokeBlockly(StiReport report, object sender, StiEvent stiEvent, EventArgs e = null)
        {
            try
            {
                if (stiEvent.Script.StartsWith(StiBlocksConst.IdentXml))
                {
                    var blocksParser = StiBlocksCreator.GetBlockParse();

                    if (blocksParser != null)
                        blocksParser.Evaluate(report, sender, stiEvent, e);
                }
            }
            catch { }
        }
        #endregion
    }
}
