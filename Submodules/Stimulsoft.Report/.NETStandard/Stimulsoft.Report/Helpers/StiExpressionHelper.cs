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

using System;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Engine;

namespace Stimulsoft.Report.Helpers
{
    public class StiExpressionHelper
    {
        public static string ParseText(StiPage page, string text, bool allowReturnNull = false)
        {
            if (string.IsNullOrEmpty(text) || page == null)
                return text;
            
            if (text.Contains("{") && text.Contains("}") && !text.Contains("{{") && !text.Contains("}}"))
            {
                try
                {
                    var textComp = new StiText { Page = page };
                    var result = StiParser.ParseTextValue(text, textComp);

                    if (result != null)
                        return result.ToString();

                    else if (allowReturnNull)
                        return null;
                }
                catch
                {
                }
            }

            return text;
        }

        public static bool ParseBool(StiPage page, string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            try
            {
                var textComp = new StiText { Page = page };

                text = text.Trim();
                if (!text.StartsWith("{"))text = "{" + text;
                if (!text.EndsWith("}"))text = text + "}";

                var value = StiParser.ParseTextValue(text, textComp);
                if (value is bool)
                    return (bool) value;

                if (value is string)
                {
                    bool res;
                    if (bool.TryParse((string) value, out res)) return res;
                }
            }
            catch
            {
            }

            return false;
        }
    }
}
