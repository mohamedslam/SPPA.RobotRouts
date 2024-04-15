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
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Chart
{
    public class StiPie3dArea : StiPieArea
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.RemoveProperty(nameof(ColorEach));

            return jObject;
        }
        #endregion

        #region Methods.Types
        public override Type GetDefaultSeriesLabelsType()
        {
            return typeof(StiCenterPie3dLabels);
        }

        public override Type[] GetSeriesLabelsTypes()
        {
            return new[]
            {
                 typeof(StiNoneLabels),
                 typeof(StiCenterPie3dLabels)
            };
        }

        public override Type GetDefaultSeriesType()
        {
            return typeof(StiPie3dSeries);
        }

        public override Type[] GetSeriesTypes()
        {
            return new[]
            {
                typeof(StiPie3dSeries)
            };
        }
        #endregion

        #region Methods.override
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiPie3dArea;

        public override StiArea CreateNew()
        {
            return new StiPie3dArea();
        }
        #endregion

        [StiUniversalConstructor("Area")]
        public StiPie3dArea()
        {
            this.Core = new StiPie3dAreaCoreXF(this);
            this.ColorEach = true;
        }
    }
}
