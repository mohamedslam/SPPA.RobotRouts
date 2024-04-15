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

using System.Drawing;
using System.Collections.Generic;
using Stimulsoft.Base.Context;
using System;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiCellGeom : 
        StiGeom,
        IStiGeomInteraction
    {
        #region IStiGeomInteraction
        public virtual void InvokeClick(StiInteractionOptions options)
        {
        }

        public virtual void InvokeMouseEnter(StiInteractionOptions options)
        {
        }

        public virtual void InvokeMouseLeave(StiInteractionOptions options)
        {
        }

        public virtual void InvokeMouseDown(StiInteractionOptions options)
        {
        }

        public virtual void InvokeMouseUp(StiInteractionOptions options)
        {
        }

        public virtual void InvokeDrag(StiInteractionOptions options)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets value which indicates that this geom object is inivisible 
        /// </summary>
        public virtual bool Invisible
        {
            get
            {
                return false;
            }
        }

        public override StiGeomType Type
        {
            get
            {
                return StiGeomType.None;
            }
        }

        private List<StiCellGeom> childGeoms = null;
        public List<StiCellGeom> ChildGeoms
        {
            get
            {
                return childGeoms;
            }
        }

        private RectangleF clientRectangle = RectangleF.Empty;
        /// <summary>
        /// Gets or sets client rectangle of this geom object.
        /// </summary>
        public RectangleF ClientRectangle
        {
            get
            {
                return clientRectangle;
            }
            set
            {
                clientRectangle = value;
            }
        }
        #endregion

        #region Methods

        public virtual void Dispose()
        {
            if (childGeoms != null)
            {
                foreach (var cellGeom in childGeoms)
                {
                    cellGeom.Dispose();
                }
                childGeoms.Clear();
                childGeoms = null;
            }
        }

        public virtual bool Contains(float x, float y)
        {
            if (Invisible) return false;
            return this.ClientRectangle.Contains(x, y);
        }

        public StiCellGeom GetGeomAt(StiCellGeom parent, float x, float y, Type typeNotUse = null)
        {
            float xx = x - parent.ClientRectangle.X;
            float yy = y - parent.ClientRectangle.Y;

            if (parent.ChildGeoms != null)
            {
                var axisAreaGeom = parent as StiAxisAreaGeom;

                StiCellGeom selectedGeom = null;
                

                foreach (StiCellGeom childGeom in parent.ChildGeoms)
                {
                    if (childGeom == null || (typeNotUse != null && childGeom.GetType() == typeNotUse)) continue;

                    //Process child of StiAxisAreaGeom only if they placed in rectangle of area.
                    if ((axisAreaGeom != null && axisAreaGeom.IsChildVisibleInView(childGeom)) ||
                        axisAreaGeom == null)
                    {
                        #region Special processing for StiAxisAreaGeom
                        if (childGeom is StiAxisAreaGeom)
                        {
                            if (!((StiAxisAreaGeom)childGeom).View.Contains(x, y))
                                continue;

                            var geom = GetGeomAt(childGeom, xx, yy, typeNotUse);
                            if (geom != null)
                                selectedGeom = geom;
                        }
                        #endregion

                        #region Other geoms
                        else
                        {
                            var geom = GetGeomAt(childGeom, xx, yy, typeNotUse);
                            if (geom != null)
                            {
                                if (typeNotUse != null && geom.GetType().IsSubclassOf(typeNotUse)) continue;

                                selectedGeom = geom;
                                if (selectedGeom is StiMarkerGeom)
                                    return selectedGeom;
                            }
                        }
                        #endregion
                    }

                }

                if (selectedGeom != null)
                    return selectedGeom;
            }
            

            if (parent.Contains(x, y) && !(parent is StiSeriesLabelsGeom))
                return parent;

            return null;
        }

        public List<StiCellGeom> GetSeriesGeoms()
        {
            List<StiCellGeom> geoms = new List<StiCellGeom>();
            if (childGeoms == null)
                return geoms;

            foreach (StiCellGeom childGeom in this.ChildGeoms)
            {
                if (childGeom is StiSeriesGeom || childGeom is StiSeriesElementGeom)
                    geoms.Add(childGeom);

                List<StiCellGeom> geoms2 = childGeom.GetSeriesGeoms();
                foreach (StiCellGeom childGeom2 in geoms2)
                {
                    geoms.Add(childGeom2);
                }
            }
            return geoms;
        }

        public List<StiCellGeom> GetSeriesElementGeoms()
        {
            List<StiCellGeom> geoms = new List<StiCellGeom>();
            if (childGeoms == null)
                return geoms;

            foreach (StiCellGeom childGeom in this.ChildGeoms)
            {
                if (childGeom == null) continue;

                if (childGeom is IStiSeriesElement)
                    geoms.Add(childGeom);

                List<StiCellGeom> geoms2 = childGeom.GetSeriesElementGeoms();
                foreach (StiCellGeom childGeom2 in geoms2)
                {
                    geoms.Add(childGeom2);
                }
            }
            return geoms;
        }

        public RectangleF GetRect(StiGeom geom)
        {
            var rect = RectangleF.Empty;
            if (childGeoms == null)
                return rect;

            foreach (var childGeom in this.ChildGeoms)
            {
                if (childGeom == geom)
                {
                    rect.X += childGeom.ClientRectangle.X;
                    rect.Y += childGeom.ClientRectangle.Y;
                    rect.Size = childGeom.ClientRectangle.Size;
                    return rect;
                }

                var clientRect = childGeom.GetRect(geom);
                if (!clientRect.IsEmpty)
                {
                    rect.X += childGeom.ClientRectangle.X;
                    rect.Y += childGeom.ClientRectangle.Y;

                    rect.X += clientRect.X;
                    rect.Y += clientRect.Y;
                    rect.Size = clientRect.Size;
                    return rect;
                }
            }
            return rect;
        }

        public void CreateChildGeoms()
        {
            if (childGeoms == null)
                childGeoms = new List<StiCellGeom>();
        }

        /// <summary>
        /// Draws cell geom object on spefied context.
        /// </summary>
        public abstract void Draw(StiContext context);

        /// <summary>
        /// Draws area geom object with all child geom objects on spefied context.
        /// </summary>
        public virtual void DrawGeom(StiContext context)
        {
            Draw(context);
            DrawChildGeoms(context);
        }

        public virtual void DrawChildGeoms(StiContext context)
        {
            if (this.ChildGeoms != null)
            {
                context.PushTranslateTransform(this.ClientRectangle.X, this.ClientRectangle.Y);
                foreach (StiCellGeom childGeom in ChildGeoms)
                {
                    if (AllowChildDrawing(childGeom))
                        childGeom.DrawGeom(context);
                }
                context.PopTransform();
            }
        }

        protected virtual bool AllowChildDrawing(StiCellGeom cellGeom)
        {
            return cellGeom != null;
        }
        #endregion

        public StiCellGeom(RectangleF clientRectangle)
        {
            this.clientRectangle = clientRectangle;
        }
    }
}
