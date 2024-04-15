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

using Stimulsoft.Base.Blocks;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Helper;
using System.Linq;

namespace Stimulsoft.Report.Check
{
    public class StiComponentEventsAtInterpretationCheck : StiComponentCheck
    {
        #region Properties
        public override bool PreviewVisible => true;

        public override string ShortMessage => StiLocalizationExt.Get("CheckComponent", "StiEventsAtInterpretationCheckShort");

        public override string LongMessage => string.Format(StiLocalizationExt.Get("CheckComponent", "StiEventsAtInterpretationCheckLong"), EventName, ElementName);
        
        public override StiCheckStatus Status => StiCheckStatus.Warning;

        public string EventName { get; set; }
        #endregion

        #region Methods
        public override object ProcessCheck(StiReport report, object obj)
        {
            Element = obj;

            if (!(Element is StiComponent) || (report.CalculationMode != StiCalculationMode.Interpretation && !StiOptions.Engine.ForceInterpretationMode))
                return null;

            var comp = Element as StiComponent;

            var events = comp.GetEvents();
            if (events == null)
                return null;

            var eventStrs = events.Cast<StiEvent>().Where(e => !string.IsNullOrWhiteSpace(e.Script) && !e.Script.StartsWith(StiBlocksConst.IdentXml)).Select(e => e.PropertyName);

            if (eventStrs == null || !eventStrs.Any())
                return null;

            var str = string.Join(", ", eventStrs);
            return new StiComponentEventsAtInterpretationCheck
            {
                Element = obj,
                EventName = str
            };
        }
        #endregion
    }
}
