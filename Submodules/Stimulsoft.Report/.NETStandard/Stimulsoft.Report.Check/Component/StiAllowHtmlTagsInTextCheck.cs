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
    public class StiAllowHtmlTagsInTextCheck : StiComponentCheck
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
                return StiLocalizationExt.Get("CheckComponent", "StiAllowHtmlTagsInTextCheckShort");
            }
        }

        public override string LongMessage
        {
            get
            {
                return string.Format(StiLocalizationExt.Get("CheckComponent", "StiAllowHtmlTagsInTextCheckLong"), this.ElementName);
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
        private bool Check()
        {
            StiText comp = Element as StiText;

            if (comp != null && !comp.AllowHtmlTags && !string.IsNullOrEmpty(comp.Text.Value))
            {
                if (comp.Text.Value.Contains("&amp;") || comp.Text.Value.Contains("&lt;") || comp.Text.Value.Contains("&gt;") ||
                    comp.Text.Value.Contains("&quot;") || comp.Text.Value.Contains("&nbsp;"))
                {
                    return true;
                }

                int startIndex = 0;
                int endIndex = 0;

                while (startIndex < comp.Text.Value.Length)
                {
                    startIndex = comp.Text.Value.IndexOf("<", startIndex);
                    if (startIndex == -1) break;
                    endIndex = comp.Text.Value.IndexOf(">", startIndex);
                    if (endIndex == -1) break;
                    if (endIndex > startIndex + 1)
                    {
                        string tag = comp.Text.Value.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();
                        if ((tag.Length > 0) && ((tag.Length < 10) && (tag == "b" || tag == "/b" || tag == "i" || tag == "/i" || tag == "u" || tag == "/u" || tag == "s" || tag == "/s" ||
                            tag == "sub" || tag == "/sub" || tag == "sup" || tag == "/sup" ||
                            tag == "br" || tag == "/br" || tag == "strong" || tag == "/strong" ||
                            tag == "p" || tag == "/p" || tag == "em" || tag == "/em") ||
                            tag.StartsWith("font ") || tag.StartsWith("/font") ||
                            tag.StartsWith("font-face") || tag.StartsWith("/font-face") ||
                            tag.StartsWith("font-name") || tag.StartsWith("/font-name") ||
                            tag.StartsWith("font-family") || tag.StartsWith("/font-family") ||
                            tag.StartsWith("font-size") || tag.StartsWith("/font-size") ||
                            tag.StartsWith("font-color") || tag.StartsWith("/font-color") ||
                            tag.StartsWith("color") || tag.StartsWith("/color") ||
                            tag.StartsWith("background-color") || tag.StartsWith("/background-color") ||
                            tag.StartsWith("letter-spacing") || tag.StartsWith("/letter-spacing") ||
                            tag.StartsWith("word-spacing") || tag.StartsWith("/word-spacing") ||
                            tag.StartsWith("line-height") || tag.StartsWith("/line-height") ||
                            tag.StartsWith("text-align") || tag.StartsWith("/text-align")))
                        {
                            return true;
                        }
                    }
                    startIndex++;
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
                    StiAllowHtmlTagsInTextCheck check = new StiAllowHtmlTagsInTextCheck();
                    check.Element = obj;
                    check.Actions.Add(new StiAllowHtmlTagsInTextAction());
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