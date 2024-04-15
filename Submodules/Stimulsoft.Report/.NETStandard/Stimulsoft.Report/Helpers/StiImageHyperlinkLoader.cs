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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using System;
using System.Collections;
using System.Threading.Tasks;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Helpers
{
    public static class StiImageHyperlinkLoader
    {
        #region Fields
        private static Hashtable failedAttempts = new Hashtable();
        #endregion

        #region Properties
        public static bool AllowAsyncLoading { get; set; } = true;
        
        public static bool AllowCacheFails { get; set; } = true;
        #endregion

        #region Methods
        public static void Load(StiImage image, string url)
        {            
            if (AllowCacheFails && failedAttempts[url] != null) return;            

            var task = new Task(() =>
            {
                try
                {
                    var bytes = StiDownloadCache.Get(url, image.Report?.CookieContainer, image.Report?.HttpHeadersContainer);

                    //Mark null bytes as an error to prevent its loading next time
                    if (bytes == null && AllowCacheFails)
                    {
                        failedAttempts[url] = "";
                        return;
                    }
                    
                    image.PutImageToDraw(bytes);

                    if (image.IsDesigning)
                    {
                        var designer = image.Report.Designer as Control;
                        designer?.BeginInvoke((Action)(() => image?.Report?.Designer?.Invalidate(image)));
                    }
                }
                catch
                {
                    if (AllowCacheFails)
                        failedAttempts[url] = url;
                }
            });
            task.Start();
        }
        #endregion
    }
}
