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
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Gauge.Collections;
using Stimulsoft.Report.PropertyGrid;
using System.Collections;
using System.ComponentModel;

namespace Stimulsoft.Report.Components.Gauge.Primitives
{
    public abstract class StiScaleRangeList : StiGaugeElement
    {
        #region IStiJsonReportObject.override

        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyJObject(nameof(Ranges), this.Ranges.SaveToJsonObject(mode));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Ranges):
                        this.Ranges.LoadFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiScaleRangeList;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var checkBoxHelper = new StiPropertyCollection();

            // MiscCategory
            var list = new[]
            {
                propHelper.AllowApplyStyle()
            };
            checkBoxHelper.Add(StiPropertyCategories.Misc, list);

            return checkBoxHelper;
        }
        #endregion

        #region ICloneable
        public override object Clone()
        {
            var rangeList = (StiScaleRangeList)base.Clone();

            rangeList.Ranges = new StiRangeCollection(rangeList);
            lock (((ICollection)this.Ranges).SyncRoot)
            {
                foreach (StiRangeBase range in this.Ranges)
                {
                    rangeList.Ranges.Add((StiRangeBase)range.Clone());
                }
            }

            return rangeList;
        }
        #endregion

        #region Properties
        [Browsable(false)]
        [StiSerializable(StiSerializationVisibility.List)]
        public StiRangeCollection Ranges { get; set; }
        #endregion

        public StiScaleRangeList()
        {
            this.Ranges = new StiRangeCollection(this);
        }
    }
}