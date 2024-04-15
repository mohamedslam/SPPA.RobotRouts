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

using Stimulsoft.Report;
using Stimulsoft.Report.Events;
using System;
using System.Collections.Generic;

namespace Stimulsoft.Blockly.Model
{
    public class Context
    {
        public IDictionary<string, object> Variables { get; set; }

        public IDictionary<string, object> Functions { get; set; }

        public EscapeMode EscapeMode { get; set; }

        public Context Parent { get; set; }

        public StiReport Report { get; set; }

        public object Sender { get; set; }

        public StiEvent Event { get; set; }

        public EventArgs EventArgs { get; set; }

        public Context()
        {
            this.Variables = new Dictionary<string, object>();
            this.Functions = new Dictionary<string, object>();
        }
    }
}
