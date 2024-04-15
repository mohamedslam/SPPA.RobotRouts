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

using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Helpers;
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiFilterCircularDependencyElementCheck : StiComponentCheck
    {
        #region Properties
        public override bool DefaultStateEnabled => true;

        public override bool PreviewVisible => true;

        public override string ShortMessage => StiLocalizationExt.Get("CheckComponent", "StiFilterCircularDependencyElementCheckShort");

        public override string LongMessage => string.Format(StiLocalizationExt.Get("CheckComponent", "StiFilterCircularDependencyElementCheckLong"), ElementName);

        public override StiCheckStatus Status => StiCheckStatus.Warning;
        #endregion

        #region Methods
        private bool Check()
        {
            return StiCrossLinkedFilterHelper.IsCrossLinkedFilter(Element as IStiFilterElement);
        }

        public override object ProcessCheck(StiReport report, object obj)
        {
            Element = obj;

            try
            {
                return Check() 
                    ? new StiFilterCircularDependencyElementCheck { Element = obj } 
                    : null;
            }
            finally
            {
                Element = null;
            }
        }
        #endregion
    }
}