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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Components.Gauge.Primitives;
using Stimulsoft.Report.Gauge;
using System.Collections.Generic;
using Stimulsoft.Report.PropertyGrid;
using System.ComponentModel;

namespace Stimulsoft.Report.Components.Gauge
{
    public class StiLinearTickMarkMajor :
        StiLinearTickMarkBase
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            return jObject;
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiLinearTickMarkMajor;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var checkBoxHelper = new StiPropertyCollection();

            // ValueCategory
            var list = new[]
            {
                propHelper.NullableMaximumValue(),
                propHelper.NullableMinimumValue(),
                propHelper.Offset(),
                propHelper.SkipIndices(),
                propHelper.SkipValues()
            };
            checkBoxHelper.Add(StiPropertyCategories.Value, list);

            // TickCategory
            list = new[]
            {
                propHelper.Placement(),
                propHelper.RelativeHeight(),
                propHelper.RelativeWidth(),
                propHelper.Skin()
            };
            checkBoxHelper.Add(StiPropertyCategories.Tick, list);

            // AppearanceCategory
            list = new[]
            {
                propHelper.Brush(),
                propHelper.BorderBrush(),
                propHelper.BorderWidth()
            };
            checkBoxHelper.Add(StiPropertyCategories.Appearance, list);

            // MiscCategory
            list = new[]
            {
                propHelper.AllowApplyStyle()
            };
            checkBoxHelper.Add(StiPropertyCategories.Misc, list);

            return checkBoxHelper;
        }
        #endregion

        #region IStiApplyStyleGauge
        public override void ApplyStyle(IStiGaugeStyle style)
        {
            if (this.AllowApplyStyle)
            {
                this.BorderBrush = style.Core.TickMarkMajorBorder;
                this.Brush = style.Core.TickMarkMajorBrush;
                this.BorderWidth = style.Core.TickMarkMajorBorderWidth;
            }
        }
        #endregion

        #region Properties override
        public override string LocalizeName => "LinearTickMarkMajor";
        #endregion

        #region Methods override
        public override StiGaugeElement CreateNew() => new StiLinearTickMarkMajor();

        protected override Dictionary<float, float> GetPointCollection() => GetMajorCollections();
        #endregion
    }
}