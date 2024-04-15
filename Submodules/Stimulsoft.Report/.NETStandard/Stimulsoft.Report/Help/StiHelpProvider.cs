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
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Help
{
    public static class StiHelpProvider
    {
        public static void ShowHelpViewer(string url)
        {
            try
            {
                if (!string.IsNullOrEmpty(url) && !url.EndsWithInvariant("?toc=0"))
                    url += "?toc=0";

                var hyperlink = url ?? "user-manual/";
#if !NETSTANDARD
                StiHelpViewerForm.Show(hyperlink);
#endif
            }
            catch (Exception ee)
            {
                StiExceptionProvider.Show(ee);
            }
        }

        public static void ShowHelpDefaultBrowser(string url)
        {
            try
            {
                string language;
                switch (StiLocalization.CultureName)
                {
                    case "ru":
                        language = "ru";
                        break;

                    default:
                        language = "en";
                        break;
                };

                StiProcess.Start($"https://www.stimulsoft.com/{language}/documentation/online/{url}");
      
            }
            catch (Exception ee)
            {
                StiExceptionProvider.Show(ee);
            }
        }
    }
}