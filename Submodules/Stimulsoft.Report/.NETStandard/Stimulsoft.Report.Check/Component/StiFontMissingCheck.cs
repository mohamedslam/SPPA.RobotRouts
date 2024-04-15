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
    public class StiFontMissingCheck : StiComponentCheck
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
                return StiLocalizationExt.Get("CheckComponent", "StiFontMissingCheckShort");
            }
        }

        public override string LongMessage
        {
            get
            {
                var fontElement = this.Element as IStiFont;
                var font = fontElement != null ? fontElement.Font : null;
                var fontName = font != null ? font.OriginalFontName : string.Empty;

                return string.Format(StiLocalizationExt.Get("CheckComponent", "StiFontMissingCheckLong"), fontName, this.ElementName);
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
            IStiFont comp = Element as IStiFont;

            if (comp != null) // && StiOptions.Engine.AllowUseOriginalFontName)
            {
                if (!string.IsNullOrEmpty(comp.Font.OriginalFontName) && comp.Font.Name != comp.Font.OriginalFontName) return true;
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
                    StiFontMissingCheck check = new StiFontMissingCheck();
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