#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using Stimulsoft.Data.Extensions;
using System;
using System.Linq;

namespace Stimulsoft.Data.Functions
{
    public partial class Funcs
    {
        public static object Image(object value, int width = 200, int height = 200)
        {
            if (!ListExt.IsList(value))
            {
                var hyperlink = value as string;
                if (!string.IsNullOrWhiteSpace(hyperlink) && IsValidUrl(hyperlink))
                {
                    var bytes = StiBytesFromURL.TryLoad(hyperlink);
                    var image = StiImageConverter.BytesToImage(bytes, width, height);
                    return StiImageConverter.ImageToBytes(image, true);
                }
            }

            return ListExt.ToList(value).Select(v => Image(v, width, height));
        }

        public static bool IsValidUrl(string hyperlink)
        {
            Uri uri;
            return Uri.TryCreate(hyperlink, UriKind.Absolute, out uri)
                   && (uri.Scheme == Uri.UriSchemeHttp
                       || uri.Scheme == Uri.UriSchemeHttps
                       || uri.Scheme == Uri.UriSchemeFtp
                       || uri.Scheme == Uri.UriSchemeMailto);
        }
    }
}