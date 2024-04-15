#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports.Net											}
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

using Stimulsoft.Base.Maps.Geoms;
using System.Collections.Generic;

namespace Stimulsoft.Report.Maps
{
    public class StiMapSvgContainer
    {
        #region Fields
        private Dictionary<string, object> hashGeometryWpf;
        #endregion

        #region Properties
        internal bool IsNotCorrect { get; set; }

        internal bool IsCustom { get; set; }

        public string Name { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public double? TextScale { get; set; }

        public string Icon { get; set; }

        public List<StiMapSvg> Paths { get; set; } = new List<StiMapSvg>();

        public List<StiMapGeomsObject> Geoms { get; set; }

        public Dictionary<string, StiMapSvg> HashPaths { get; set; }
        #endregion

        #region Methods
        public object GetGeometryWpf(string key)
        {
            if (hashGeometryWpf == null)
                hashGeometryWpf = new Dictionary<string, object>();

            if (hashGeometryWpf.ContainsKey(key))
                return hashGeometryWpf[key];
            return null;
        }

        public void SetGeometryWpf(string key, object geom)
        {
            if (hashGeometryWpf == null)
                hashGeometryWpf = new Dictionary<string, object>();

            hashGeometryWpf[key] = geom;
        }

        public void Prepare()
        {
            if (this.Paths == null)
                this.Paths = new List<StiMapSvg>();

            if (HashPaths == null)
            {
                HashPaths = new Dictionary<string, StiMapSvg>();

                foreach (var pt in this.Paths)
                {
                    HashPaths.Add(pt.Key, pt);
                }

                this.Paths.Clear();
                this.Paths = null;
            }
        }
        #endregion
    }
}