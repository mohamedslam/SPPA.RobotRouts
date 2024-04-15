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

namespace Stimulsoft.Map.Gis.Core
{
    internal sealed class StiGisTileMatrix : IDisposable
    {
        public StiGisTileMatrix()
        {
            for (int index = 0; index < layers.Capacity; index++)
            {
                layers.Add(new Dictionary<StiGisPoint, StiGisTile>(55, new StiGisPointComparer()));
            }
        }

        ~StiGisTileMatrix()
        {
            Dispose(false);
        }

        #region Fields
        private List<Dictionary<StiGisPoint, StiGisTile>> layers = new List<Dictionary<StiGisPoint, StiGisTile>>(24);
        #endregion

        #region Methods
        public void ClearAllLevels()
        {
            if (layers == null) return;

            foreach (var layer in layers)
            {
                foreach (var pair in layer)
                {
                    pair.Value.Dispose();
                }

                layer.Clear();
            }
        }

        public void ClearLevelsBelove(int zoom)
        {
            if (zoom - 1 < layers.Count)
            {
                for (int index = zoom - 1; index >= 0; index--)
                {
                    var layer = layers[index];

                    var values = layer.Values.ToList();
                    for (int index1 = 0; index1 < values.Count; index1++)
                    {
                        values[index1].Dispose();
                    }

                    layer.Clear();
                }
            }
        }

        public void ClearLevelsAbove(int zoom)
        {
            if (zoom + 1 < layers.Count)
            {
                for (int index = zoom + 1; index < layers.Count; index++)
                {
                    var layer = layers[index];

                    var values = layer.Values.ToList();
                    for (int index1 = 0; index1 < values.Count; index1++)
                    {
                        values[index1].Dispose();
                    }

                    layer.Clear();
                }
            }
        }

        public bool Contains(int zoom, StiGisPoint p)
        {
            return layers[zoom].ContainsKey(p);
        }

        public StiGisTile Get(int zoom, StiGisPoint p)
        {
            StiGisTile ret;
            layers[zoom].TryGetValue(p, out ret);

            return ret;
        }

        public void Set(StiGisTile t)
        {
            if (t.Zoom < layers.Count)
                layers[t.Zoom][t.Pos] = t;
        }
        #endregion

        #region IDisposable Members
        public void Dispose(bool disposing)
        {
            if (disposing)
                ClearAllLevels();

            layers?.Clear();
            layers = null;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}