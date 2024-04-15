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
using Stimulsoft.Base.Blocks;
using Stimulsoft.Report;
using System;
using Stimulsoft.Report.Events;

namespace Stimulsoft.Blockly
{
    public class StiBlocksParser : IStiBlocksParser
    {
        #region Methods
        public void Evaluate(IStiReport report, object sender, object eventObj, EventArgs args)
        {
            var stiEvent = eventObj as StiEvent;

            new Parser()
              .AddStandardBlocks()
              .Parse(stiEvent.Script)
              .Evaluate((StiReport)report, sender, stiEvent, args);
        }
        #endregion
    }
}
