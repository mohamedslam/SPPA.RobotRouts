#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

#if NETSTANDARD
using Stimulsoft.System.Web.UI;
#else
using System.Web.UI;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiJavaScript : LiteralControl
    {
        public string ScriptUrl { get; set; } = string.Empty;

        protected override void Render(HtmlTextWriter output)
        {
            if (!string.IsNullOrEmpty(ScriptUrl))
                output.Write(string.Format("<script type=\"text/javascript\" src=\"{0}\">", StiUrlHelper.EscapeUrlQuotes(ScriptUrl)));
            else
                output.Write("<script type=\"text/javascript\">");

            base.Render(output);

            output.Write("</script>");
        }

        public StiJavaScript()
        {
            this.Text = string.Empty;
        }
    }
}
