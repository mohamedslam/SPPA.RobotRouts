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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Maps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
#endif

namespace Stimulsoft.Report.Painters
{
    public sealed class StiMapInteractionContainer : IDisposable
    {
        #region Fields
        private Dictionary<string, List<GraphicsPath>> items = new Dictionary<string, List<GraphicsPath>>();
        private Dictionary<string, StiMapSvg> hashPaths;
        private double mapGeomWidth;
        private double mapGeomHeight;
        #endregion

        #region Propeties
        public float Zoom { get; set; } = 1;
        #endregion

        #region Method.Dispose
        public void Dispose()
        {
            if (this.items != null)
            {
                foreach (var pair in this.items)
                {
                    foreach (var item in pair.Value)
                    {
                        item.Dispose();
                    }

                    pair.Value.Clear();
                }

                this.items.Clear();
                this.items = null;
            }
        }
        #endregion

        #region Methods
        public void SetValue(string key, string value)
        {
            if (hashPaths.TryGetValue(key, out var result))
            {
                result.Value = value;
            }
        }

        public void Add(string key, List<GraphicsPath> path)
        {
            this.items.Add(key, path);
        }

        public string GetKeyByPosition(Point pos, float zoom, Point globalMovePoint, RectangleD rect)
        {
            var moveX = (float)(((rect.Width / zoom) - mapGeomWidth) / 2);
            var moveY = (float)(((rect.Height / zoom) - mapGeomHeight) / 2);

            var x = (float)(pos.X / zoom - moveX) - globalMovePoint.X;
            var y = (float)(pos.Y / zoom - moveY) - globalMovePoint.Y;

            foreach (var pair in items)
            {
                foreach (var item in pair.Value)
                {
                    if (item.IsVisible(x, y))
                        return pair.Key;
                }
            }

            return null;
        }

        public StiMapSvg GetMapSvgByPosition(Point pos)
        {
            foreach (var pair in items)
            {
                foreach (var item in pair.Value)
                {
                    if (item.IsVisible(pos.X, pos.Y))
                        return hashPaths[pair.Key];
                }
            }

            return null;
        }
        #endregion

        public StiMapInteractionContainer(Dictionary<string, StiMapSvg> hashPaths, double mapGeomWidth, double mapGeomHeight)
        {
            this.hashPaths = hashPaths;
            this.mapGeomWidth = mapGeomWidth;
            this.mapGeomHeight = mapGeomHeight;
        }
    }
}