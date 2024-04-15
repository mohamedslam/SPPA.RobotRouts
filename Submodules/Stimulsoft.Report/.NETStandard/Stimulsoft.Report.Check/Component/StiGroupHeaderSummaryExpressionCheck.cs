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

using Stimulsoft.Report.Components;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Helper;
using System.Collections.Generic;
using static Stimulsoft.Report.Engine.StiParser;

namespace Stimulsoft.Report.Check
{
    public class StiGroupHeaderSummaryExpressionCheck : StiComponentCheck
    {
        #region Properties
        public override bool PreviewVisible
        {
            get
            {
                return true;
            }
        }

        public override string ShortMessage
        {
            get
            {
                return string.Format(StiLocalizationExt.Get("CheckComponent", "StiGroupHeaderSummaryExpressionCheckShort"), this.ElementName);
            }
        }

        public override string LongMessage
        {
            get
            {
                return string.Format(StiLocalizationExt.Get("CheckComponent", "StiGroupHeaderSummaryExpressionCheckLong"), this.ElementName);
            }
        }

        public override StiCheckStatus Status
        {
            get
            {
                return StiCheckStatus.Error;
            }
        }

        public string PropertyName { get; set; } = "SummaryExpression";
        #endregion

        #region Methods
        private bool Check(StiGroupHeaderBand groupHeader)
        {
            if (groupHeader == null) return false;

            var exp = groupHeader.SummaryExpression;
            if (exp != null && !string.IsNullOrWhiteSpace(exp.Value))
            {
                try
                {
                    StiParserParameters pp = new StiParserParameters()
                    {
                        ReturnAsmList = true
                    };
                    var list = StiParser.ParseTextValue(exp.Value, groupHeader, pp) as List<StiAsmCommand>;
                    foreach (var asmCommand in list)
                    {
                        if (asmCommand.Type == StiAsmCommandType.PushFunction)
                        {
                            StiFunctionType ft = (StiFunctionType)asmCommand.Parameter1;
                            if (ft >= StiFunctionType.Count && ft <= StiFunctionType.MaxStrOnlyChilds) return true;
                        }
                    }
                }
                catch
                {
                }
            }

            return false;
        }
        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;
            StiGroupHeaderBand groupHeader = Element as StiGroupHeaderBand;

            try
            {
                bool failed = Check(groupHeader);

                if (failed)
                {
                    StiGroupHeaderSummaryExpressionCheck check = new StiGroupHeaderSummaryExpressionCheck();
                    check.Element = obj;
                    check.Actions.Add(new StiEditPropertyAction());
                    return check;
                }
            }
            finally
            {
                this.Element = null;
            }
            return null;
        }
        #endregion
    }
}