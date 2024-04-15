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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiTextColorEqualToBackColorCheck : StiComponentCheck
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
                return StiLocalizationExt.Get("CheckComponent", "StiTextColorEqualToBackColorCheckShort");
            }
        }

        public override string LongMessage
        {
            get
            {
                return string.Format(StiLocalizationExt.Get("CheckComponent", "StiTextColorEqualToBackColorCheckLong"), this.ElementName);
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
            StiText comp = Element as StiText;

            if (comp != null)
            {
                if (comp.Brush is StiEmptyBrush && comp.TextBrush is StiEmptyBrush)
                {
                    return true;
                }

                if (comp.Brush is StiSolidBrush && comp.TextBrush is StiSolidBrush)
                {
                    if (((StiSolidBrush)comp.Brush).Color == ((StiSolidBrush)comp.TextBrush).Color)
                        return true;
                    else
                        return false;
                }

                if (comp.Brush is StiHatchBrush && comp.TextBrush is StiHatchBrush)
                {
                    if (((StiHatchBrush)comp.Brush).ForeColor == ((StiHatchBrush)comp.TextBrush).ForeColor &&
                        ((StiHatchBrush)comp.Brush).BackColor == ((StiHatchBrush)comp.TextBrush).BackColor)
                        return true;
                    else
                        return false;
                }

                if (comp.Brush is StiGradientBrush && comp.TextBrush is StiGradientBrush)
                {
                    if (((StiGradientBrush)comp.Brush).StartColor == ((StiGradientBrush)comp.TextBrush).StartColor &&
                        ((StiGradientBrush)comp.Brush).EndColor == ((StiGradientBrush)comp.TextBrush).EndColor)
                        return true;
                    else
                        return false;
                }

                if (comp.Brush is StiGlareBrush && comp.TextBrush is StiGlareBrush)
                {
                    if (((StiGlareBrush)comp.Brush).StartColor == ((StiGlareBrush)comp.TextBrush).StartColor &&
                        ((StiGlareBrush)comp.Brush).EndColor == ((StiGlareBrush)comp.TextBrush).EndColor)
                        return true;
                    else
                        return false;
                }

                if (comp.Brush is StiGlassBrush && comp.TextBrush is StiGlassBrush &&
                    ((StiGlassBrush)comp.Brush).Color == ((StiGlassBrush)comp.TextBrush).Color)
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
                    StiTextColorEqualToBackColorCheck check = new StiTextColorEqualToBackColorCheck();
                    check.Element = obj;
                    //check.Actions.Add(new StiNegativeSizesOfComponentsAction());
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