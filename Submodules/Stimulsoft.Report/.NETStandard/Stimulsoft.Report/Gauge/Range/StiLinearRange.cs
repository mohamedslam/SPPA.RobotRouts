﻿#region Copyright (C) 2003-2022 Stimulsoft
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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Components.Gauge.Primitives;
using Stimulsoft.Report.Gauge.GaugeGeoms;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using System.ComponentModel;
using System.Drawing;

namespace Stimulsoft.Report.Components.Gauge
{
    public class StiLinearRange : StiRangeBase
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyIdent("Ident", this.GetType().Name);
            
            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);            
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiLinearRange;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var checkBoxHelper = new StiPropertyCollection();

            // ValueCategory
            var list = new[]
            {
                propHelper.EndValue(),
                propHelper.StartValue()
            };
            checkBoxHelper.Add(StiPropertyCategories.Value, list);

            // AppearanceCategory
            list = new[]
            {
                propHelper.Brush(),
                propHelper.BorderBrush(),
                propHelper.BorderWidth()
            };
            checkBoxHelper.Add(StiPropertyCategories.Appearance, list);

            // BehaviorCategory
            list = new[]
            {
                propHelper.EndWidth(),
                propHelper.Offset(),
                propHelper.Placement(),
                propHelper.StartWidth()
            };
            checkBoxHelper.Add(StiPropertyCategories.Behavior, list);

            return checkBoxHelper;
        }
        #endregion

        #region Methods
        protected internal override void DrawRange(StiGaugeContextPainter context, StiScaleBase scale)
        {
            if (scale == null) return;
            
            float startWidth = this.StartWidth;
            float endWidth = this.EndWidth;

            if (startWidth == 0 || endWidth == 0) return;

            var rect = new RectangleF(0, 0, 0, 0);
            var linesGeom = scale.barGeometry.DrawGeometry(context, this.StartValue, this.EndValue,
                startWidth, endWidth, this.Offset, this.Placement, ref rect, false);

            if (linesGeom != null)
            {
                var pathGeom = new StiGraphicsPathGaugeGeom(rect, linesGeom.Points[0], this.Brush, this.BorderBrush, this.BorderWidth);
                pathGeom.Geoms.Add(linesGeom);
                pathGeom.AddGraphicsPathCloseFigureGaugeGeom();

                context.AddGraphicsPathGaugeGeom(pathGeom);
            }
        }
        #endregion

        #region Properties override
        public override string LocalizeName => "LinearRange";
        #endregion

        #region Methods override

        public override StiRangeBase CreateNew() => new StiLinearRange();
        #endregion
    }
}