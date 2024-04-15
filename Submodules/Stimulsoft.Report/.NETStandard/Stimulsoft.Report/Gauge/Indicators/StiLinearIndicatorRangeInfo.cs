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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.PropertyGrid;
using System.ComponentModel;
using System.Drawing;

namespace Stimulsoft.Report.Components.Gauge
{
    public class StiLinearIndicatorRangeInfo : StiIndicatorRangeInfo
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            jObject.AddPropertyColor(nameof(Color), Color, this.Color);
            jObject.AddPropertyBrush(nameof(Brush), Brush);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Color):
                        this.Color = property.DeserializeColor();
                        break;
                        
                    case nameof(Brush):
                        this.Brush = property.DeserializeBrush();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiLinearIndicatorRangeInfo;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var checkBoxHelper = new StiPropertyCollection();

            // ValueCategory
            var list = new[]
            {
                propHelper.ValueF()
            };
            checkBoxHelper.Add(StiPropertyCategories.Value, list);

            // AppearanceCategory
            list = new[]
            {
                propHelper.Brush(),
                propHelper.Color(),
            };
            checkBoxHelper.Add(StiPropertyCategories.Appearance, list);

            return checkBoxHelper;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the color of the indicator for a given Value.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the color of the indicator for a given Value.")]
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Gets or sets a brush to fill a component.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [Description("Gets or sets a brush to fill a component.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiBrush Brush { get; set; } = new StiEmptyBrush();
        #endregion

        #region Properties override
        internal override StiBarRangeListType RangeListType => StiBarRangeListType.LinearBar;
        #endregion

        #region Methods.override
        public override StiIndicatorRangeInfo CreateNew() => new StiLinearIndicatorRangeInfo();
        #endregion
    }
}