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
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Map.Gis.Cache
{
    internal sealed class StiGisMemoryCache : IDisposable
    {
        ~StiGisMemoryCache()
        {
            Dispose(false);
        }

        #region Fields
        private readonly Dictionary<StiGisRawTile, byte[]> cache = new Dictionary<StiGisRawTile, byte[]>(new StiGisRawTileComparer());
        #endregion

        #region Methods
        public void Clear()
        {
            cache.Clear();
        }

        internal byte[] Get(StiGisRawTile tile)
        {
            byte[] ret;
            if (cache.TryGetValue(tile, out ret))
                return ret;

            return null;
        }

        internal void Add(StiGisRawTile tile, byte[] data)
        {
            lock (cache)
            {
                if (data != null && !cache.ContainsKey(tile))
                    cache.Add(tile, data);
            }
        }

        public void ClearMemory(int zoom)
        {
            lock (cache)
            {
                var keys = this.cache.Keys.Where(x => x.Zoom != zoom).ToArray();
                foreach (var key in keys)
                {
                    cache.Remove(key);
                }
            }
        }
        #endregion

        #region IDisposable.override
        public void Dispose(bool disposing)
        {
            if (disposing)
                Clear();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}