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
    public class StiOrientationPageCheck : StiPageCheck
    {
        #region Properties
        public override string ShortMessage => 
            string.Format(StiLocalizationExt.Get("CheckPage", "StiOrientationPageCheckShort"), this.ElementName);

        public override string LongMessage
        {
            get
            {
                StiPage page = this.Element as StiPage;
                if (page != null)
                {
                    if (page.Orientation == StiPageOrientation.Portrait)
                        return string.Format(StiLocalizationExt.Get("CheckPage", "StiOrientationPageCheckLongPortrait"), this.ElementName);
                    else
                        return string.Format(StiLocalizationExt.Get("CheckPage", "StiOrientationPageCheckLongLandscape"), this.ElementName);
                }
                else
                {
                    return string.Format(StiLocalizationExt.Get("CheckPage", "StiOrientationPageCheckLongPortrait"), this.ElementName);
                }
            }
        }

        public override StiCheckStatus Status => StiCheckStatus.Warning;
        #endregion

        #region Methods
        public override object ProcessCheck(StiReport report, object obj)
        {
            StiPage page = obj as StiPage;

            bool failed = false;

            if (page.Orientation == StiPageOrientation.Portrait)
            {
                if (page.PageWidth > page.PageHeight)
                    failed = true;
            }
            else
            {
                if (page.PageWidth < page.PageHeight)
                    failed = true;
            }

            if (page is Stimulsoft.Report.Dialogs.StiForm)
                failed = false;

            //if (page.SegmentPerWidth > 1 || page.SegmentPerHeight > 1)
            //    failed = false;

            if (failed)
            {
                StiOrientationPageCheck check = new StiOrientationPageCheck();
                check.Element = obj;

                if (page.Orientation == StiPageOrientation.Portrait)
                    check.Actions.Add(new StiOrientationPageToLandscapeAction());
                else 
                    check.Actions.Add(new StiOrientationPageToPortraitAction());

                check.Actions.Add(new StiSwitchWidthAndHeightOfPageAction());

                return check;
            }
            else return null;
        }
        #endregion
    }
}