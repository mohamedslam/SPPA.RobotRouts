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

using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Helpers;
using Stimulsoft.Data.Parsers;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Helpers;
using Stimulsoft.Report.Helper;
using System;
using System.Linq.Expressions;

namespace Stimulsoft.Report.Check
{
    public class StiExpressionElementCheck : StiComponentCheck
    {
        #region Properties
        public override bool DefaultStateEnabled => true;

        public override bool PreviewVisible => true;

        public override string ShortMessage => StiLocalizationExt.Get("CheckComponent", "StiExpressionElementCheckShort");

        public override string LongMessage => string.Format(StiLocalizationExt.Get("CheckComponent", "StiExpressionElementCheckLong"), Expression, ElementName, Message);

        public string Expression { get; set; }

        public string Message { get; set; }

        public override StiCheckStatus Status => StiCheckStatus.Warning;
        #endregion

        #region Methods
        public override object ProcessCheck(StiReport report, object obj)
        {
            Element = obj;

            try
            {
                var element = Element as IStiElement;
                if (element == null) 
                    return null;

                var dictionary = element?.GetApp().GetDictionary();
                var meters = element?.FetchAllMeters();
                if (meters == null)
                    return null;

                foreach (IStiMeter meter in meters)
                {
                    if (string.IsNullOrWhiteSpace(meter.Expression)) continue;

                    try
                    {
                        Stimulsoft.Data.Helpers.StiExpressionHelper.Compile(meter.Expression);
                    }
                    catch (Exception exc)
                    {
                        var check = new StiExpressionElementCheck
                        {
                            Element = obj,
                            Message = exc.Message,
                            Expression = meter.Expression
                        };
                        check.Actions.Add(new StiEditPropertyAction());
                        return check;
                    }
                }                
            }
            finally
            {
                Element = null;
            }
            return null;
        }
        #endregion
    }
}