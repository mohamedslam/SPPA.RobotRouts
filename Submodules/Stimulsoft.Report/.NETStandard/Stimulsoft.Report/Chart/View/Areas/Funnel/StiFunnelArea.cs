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
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Chart
{
    public class StiFunnelArea :
        StiArea,
        IStiFunnelArea
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.RemoveProperty(nameof(ColorEach));

            return jObject;
        }
        #endregion

        #region IStiPropertyGridObject

        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiFunnelArea;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();

            objHelper.Add(StiPropertyCategories.Main, new[]
            {
                propertyGrid.PropertiesHelper.FunnelArea(),
            });

            return objHelper;
        }
        #endregion

        #region Methods.Types
        public override Type GetDefaultSeriesType()
        {
            return typeof(StiFunnelSeries);
        }

        public override Type[] GetSeriesTypes()
        {
            return new[]
            {
                typeof(StiFunnelSeries)
            };
        }

        public override Type GetDefaultSeriesLabelsType()
        {
            return typeof(StiNoneLabels);
        }

        public override Type[] GetSeriesLabelsTypes()
        {
            return new[]
            {
                typeof(StiNoneLabels),
                typeof(StiCenterFunnelLabels),
                typeof(StiOutsideRightFunnelLabels),
                typeof(StiOutsideLeftFunnelLabels)
            };
        }
        #endregion

        #region Properties
        [DefaultValue(true)]
        [StiNonSerialized]
        [Browsable(false)]
        public override bool ColorEach
        {
            get
            {
                return base.ColorEach;
            }
            set
            {
                base.ColorEach = value;
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override bool ColorEachAllowed => false;
        #endregion

        #region Methods.override
        public override StiArea CreateNew()
        {
            return new StiFunnelArea();
        }
        #endregion

        [StiUniversalConstructor("Area")]
        public StiFunnelArea()
        {
            this.Core = new StiFunnelAreaCoreXF(this);
            this.ColorEach = true;
        }
    }
}
