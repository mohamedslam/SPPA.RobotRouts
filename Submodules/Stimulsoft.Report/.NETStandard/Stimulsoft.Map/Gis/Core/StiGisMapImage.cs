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

using System;
using System.Drawing;
using System.IO;

#if NETSTANDARD
using Stimulsoft.System.Windows.Media.Imaging;
#else
using System.Windows.Media.Imaging;
#endif

namespace Stimulsoft.Map.Gis.Core
{
    public sealed class StiGisMapImage : IDisposable
    {
        public StiGisMapImage(byte[] data)
        {
            this.Data = data;
        }

        #region Fields
        public byte[] Data;
        public Image BitmapGdi;
        #endregion

        #region Methods
        public static StiGisMapImage FromByteArray(byte[] buffer, StiGeoRenderMode mode)
        {
            try
            {
                var image = new StiGisMapImage(buffer);

                if (mode == StiGeoRenderMode.Gdi)
                {
                    image.BitmapGdi = Image.FromStream(new MemoryStream(buffer));
                }

                return image;
            }
            catch { }

            return null;
        }
        #endregion

        #region IDisposable.override
        public void Dispose()
        {
            if (BitmapGdi != null)
            {
                BitmapGdi.Dispose();
                BitmapGdi = null;
            }

            Data = null;
        }
        #endregion
    }
}