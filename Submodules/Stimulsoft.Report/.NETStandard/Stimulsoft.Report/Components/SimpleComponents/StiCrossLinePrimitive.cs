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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Engine;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes class that realizes component - StiCrossLinePrimitive.
    /// </summary>
    [StiToolbox(false)]
    [StiV2Builder(typeof(StiCrossLinePrimitiveV2Builder))]
    public abstract class StiCrossLinePrimitive : StiLinePrimitive
    {
        #region ICloneable override
        public override object Clone(bool cloneProperties)
        {
            var line = (StiCrossLinePrimitive)base.Clone(cloneProperties);

            line.StoredStartPoint = null;
            line.StoredEndPoint = null;

            return line;
        }
        #endregion

        #region StiComponent override
        public override void OnRemoveComponent()
        {
            var startPoint = GetStartPoint();
            if (startPoint != null &&
                startPoint.Parent != null &&
                startPoint.Parent.Components.Contains(startPoint))
            {
                startPoint.ReferenceToGuid = null;
                startPoint.Parent.Components.Remove(startPoint);
            }

            var endPoint = GetEndPoint();
            if (endPoint != null &&
                endPoint.Parent != null &&
                endPoint.Parent.Components.Contains(endPoint))
            {
                endPoint.ReferenceToGuid = null;
                endPoint.Parent.Components.Remove(endPoint);
            }
        }

        /// <summary>
        /// May this container be located in the specified component.
        /// </summary>
        /// <param name="component">Component for checking.</param>
        /// <returns>true, if this container may is located in the specified component.</returns>
        public override bool CanContainIn(StiComponent component)
        {
            return component is StiPage;
        }

        public override bool Linked
        {
            get
            {
                return base.Linked;
            }
            set
            {
                base.Linked = value;

                var startPoint = GetStartPoint();

                if (startPoint != null)
                    startPoint.Linked = value;

                var endPoint = GetEndPoint();
                if (endPoint != null)
                    endPoint.Linked = value;
            }
        }
        #endregion

        #region Properties.Position
        public override double Left
        {
            get
            {
                var startPoint = GetStartPoint();
                if (startPoint == null)
                    return base.Left;
                else
                {
                    var pos = new PointD(startPoint.Left, startPoint.Top);
                    return Math.Round(startPoint.ComponentToPage(pos).X, 2);
                }
            }
            set
            {
                var startPoint = GetStartPoint();
                if (startPoint == null)
                    base.Left = value;
                else
                {
                    if (!IsParentContainerSelected(startPoint))
                    {
                        var pos = new PointD(value, 0);
                        pos = startPoint.PageToComponent(pos);
                        startPoint.Left = pos.X;
                    }
                }
            }
        }

        public override double Top
        {
            get
            {
                var startPoint = GetStartPoint();
                if (startPoint == null)
                    return base.Top;
                else
                {
                    var pos = new PointD(startPoint.Left, startPoint.Top);
                    return Math.Round(startPoint.ComponentToPage(pos).Y, 2);
                }
            }
            set
            {
                var startPoint = GetStartPoint();
                if (startPoint == null)
                    base.Top = value;
                else
                {
                    if (!IsParentContainerSelected(startPoint))
                    {
                        var pos = new PointD(0, value);
                        pos = startPoint.PageToComponent(pos);
                        startPoint.Top = pos.Y;
                    }
                }
            }
        }

        public override double Height
        {
            get
            {
                var startPoint = GetStartPoint();
                var endPoint = GetEndPoint();
                if (startPoint == null || endPoint == null)
                    return base.Height;
                else
                {
                    var startPos = new PointD(startPoint.Left, startPoint.Top);
                    var endPos = new PointD(endPoint.Left, endPoint.Top);

                    startPos = startPoint.ComponentToPage(startPos);
                    endPos = endPoint.ComponentToPage(endPos);

                    return Math.Round(endPos.Y - startPos.Y, 2);
                }
            }
            set
            {
                base.Height = value;

                var startPoint = GetStartPoint();
                var endPoint = GetEndPoint();
                if (startPoint != null && endPoint != null)
                {
                    var startPos = new PointD(startPoint.Left, startPoint.Top);

                    startPos = startPoint.ComponentToPage(startPos);
                    var endPos = new PointD(startPos.X, startPos.Y + value);
                    endPos = endPoint.PageToComponent(endPos);
                    endPoint.Top = endPos.Y;
                }
            }
        }
        #endregion

        #region Properties
        [Browsable(false)]
        internal StiStartPointPrimitive StoredStartPoint { get; set; }

        [Browsable(false)]
        internal StiEndPointPrimitive StoredEndPoint { get; set; }
        #endregion

        #region Methods
        public StiStartPointPrimitive GetStartPoint()
        {
            if (Report != null && !Report.IsRendering && !IsDesigning)
                return null;

            if (IsDesigning && StoredStartPoint != null)
                return StoredStartPoint;

            if (Page == null)
                return null;

            StoredStartPoint = GetStartPoint(Page);
            return StoredStartPoint;
        }

        public StiStartPointPrimitive GetStartPoint(StiContainer cont)
        {
            foreach (StiComponent comp in cont.Components)
            {
                var startPoint = comp as StiStartPointPrimitive;
                if (startPoint != null && startPoint.ReferenceToGuid == Guid)
                    return startPoint;

                var cont2 = comp as StiContainer;
                if (cont2 != null)
                {
                    var startPoint2 = GetStartPoint(cont2);
                    if (startPoint2 != null)
                        return startPoint2;
                }
            }
            return null;
        }

        public StiEndPointPrimitive GetEndPoint()
        {
            if (Report != null && !Report.IsRendering && !IsDesigning)
                return null;

            if (IsDesigning && StoredEndPoint != null)
                return StoredEndPoint;

            if (Page == null)
                return null;

            StoredEndPoint = GetEndPoint(Page);
            return StoredEndPoint;
        }

        public StiEndPointPrimitive GetEndPoint(StiContainer cont)
        {
            foreach (StiComponent comp in cont.Components)
            {
                var endPoint = comp as StiEndPointPrimitive;
                if (endPoint != null && endPoint.ReferenceToGuid == Guid)
                    return endPoint;

                var cont2 = comp as StiContainer;
                if (cont2 != null)
                {
                    var endPoint2 = GetEndPoint(cont2);
                    if (endPoint2 != null)
                        return endPoint2;
                }
            }
            return null;
        }

        internal bool IsParentContainerSelected(StiPointPrimitive point)
        {
            var parent = point.Parent;
            while (true)
            {
                if (parent == null)
                    return false;

                if (parent.IsSelected)
                    return true;

                parent = parent.Parent;
            }
        }
        #endregion

        /// <summary>
        /// Creates a new StiCrossLinePrimitive.
        /// </summary>
        public StiCrossLinePrimitive() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new StiCrossLinePrimitive.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiCrossLinePrimitive(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = true;
            NewGuid();
        }
    }
}
