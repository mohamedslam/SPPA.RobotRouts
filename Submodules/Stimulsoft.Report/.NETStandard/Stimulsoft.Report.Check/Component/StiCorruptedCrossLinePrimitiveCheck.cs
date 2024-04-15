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
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiCorruptedCrossLinePrimitiveCheck : StiComponentCheck
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
                StiComponent comp = this.Element as StiComponent;
                return string.Format(StiLocalizationExt.Get("CheckComponent", "StiCorruptedCrossLinePrimitiveCheckShort"), this.ElementName);
            }
        }

        public override string LongMessage
        {
            get
            {
                StiComponent comp = this.Element as StiComponent;

                return string.Format(StiLocalizationExt.Get("CheckComponent", "StiCorruptedCrossLinePrimitiveCheckLong"), this.ElementName);
            }
        }

        public override StiCheckStatus Status
        {
            get
            {
                return StiCheckStatus.Warning;
            }
        }
        #endregion

        #region Methods
        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;
            
            StiCrossLinePrimitive line = obj as StiCrossLinePrimitive;
            if (line != null)
            {            
                StiStartPointPrimitive startPoint = line.GetStartPoint(line.Page);
                StiEndPointPrimitive endPoint = line.GetEndPoint(line.Page);
                if (startPoint == null || endPoint == null)
                {
                    StiCorruptedCrossLinePrimitiveCheck check = new StiCorruptedCrossLinePrimitiveCheck();
                    check.Element = obj;
                    check.Actions.Add(new StiFixCrossLinePrimitiveAction());
                    check.Actions.Add(new StiDeleteComponentAction());
                    return check;
                }
                else return null;
            }
            return null;
        }
        #endregion
    }
}
