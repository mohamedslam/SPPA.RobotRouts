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
    public class StiCrossGroupHeaderNotEqualToCrossGroupFooterOnContainerCheck : StiComponentCheck
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
                return StiLocalizationExt.Get("CheckComponent", "StiGroupHeaderNotEqualToGroupFooterShort");
            }
        }

        public override string LongMessage
        {
            get
            {
                return string.Format(StiLocalizationExt.Get("CheckComponent", "StiCrossGroupHeaderNotEqualToGroupCrossFooterOnContainerLong"), this.ElementName);
            }
        }

        public override StiCheckStatus Status
        {
            get
            {
                return StiCheckStatus.Information;
            }
        }
        #endregion

        #region Methods
        private bool Check()
        {
            StiContainer container = Element as StiContainer;

            if (container != null)
            {
                int countCrossGroupHeader = 0;
                int countCrossGroupFooter = 0;
                for (int index = 0; index < container.Components.Count; index++)
                {
                    if (container.Components[index] is StiCrossGroupHeaderBand)
                    {
                        countCrossGroupHeader++;
                    }
                    else if (container.Components[index] is StiCrossGroupFooterBand)
                    {
                        countCrossGroupFooter++;
                    }
                }

                if (countCrossGroupHeader < countCrossGroupFooter)
                {
                    return true;
                }
            }

            return false;
        }

        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;

            try
            {
                bool failed = Check();

                if (failed)
                {
                    StiCrossGroupHeaderNotEqualToCrossGroupFooterOnContainerCheck check = new StiCrossGroupHeaderNotEqualToCrossGroupFooterOnContainerCheck();
                    check.Element = obj;
                    return check;
                }
                else return null;
            }
            finally
            {
                this.Element = null;
            }
        }
        #endregion
    }
}