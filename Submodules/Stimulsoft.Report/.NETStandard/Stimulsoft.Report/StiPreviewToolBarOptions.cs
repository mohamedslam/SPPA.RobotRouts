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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using System.CodeDom;
using System.ComponentModel;
using System.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
#endif

namespace Stimulsoft.Report
{
    public class StiPreviewToolBarOptions :
        IStiSerializeToCodeAsClass,
        IStiJsonReportObject,
        IStiDefault
    {
        #region IStiDefault
        [Browsable(false)]
        public virtual bool IsDefault => IsDefaultDashboardToolbar && IsDefaultReportToolbar;

        [Browsable(false)]
        public virtual bool IsDefaultDashboardToolbar => 
            DashboardToolbarHorAlignment == StiHorAlignment.Right &&
            !DashboardToolbarReverse;

        [Browsable(false)]
        public virtual bool IsDefaultReportToolbar => 
            ReportToolbarHorAlignment == StiHorAlignment.Left &&
            !ReportToolbarReverse;
        #endregion

        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyEnum(nameof(DashboardToolbarHorAlignment), DashboardToolbarHorAlignment, StiHorAlignment.Right);
            jObject.AddPropertyBool(nameof(DashboardToolbarReverse), DashboardToolbarReverse, false);
            jObject.AddPropertyEnum(nameof(ReportToolbarHorAlignment), ReportToolbarHorAlignment, StiHorAlignment.Left);
            jObject.AddPropertyBool(nameof(ReportToolbarReverse), DashboardToolbarReverse, false);

            if (jObject.Count == 0)
                return null;

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(DashboardToolbarHorAlignment):
                        this.DashboardToolbarHorAlignment = property.DeserializeEnum<StiHorAlignment>();
                        break;

                    case nameof(DashboardToolbarReverse):
                        this.DashboardToolbarReverse = property.DeserializeBool();
                        break;

                    case nameof(ReportToolbarHorAlignment):
                        this.ReportToolbarHorAlignment = property.DeserializeEnum<StiHorAlignment>();
                        break;

                    case nameof(ReportToolbarReverse):
                        this.ReportToolbarReverse = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets os sets a horizontal alignment of the dashboard's toolbar elements.
        /// </summary>
        [DefaultValue(StiHorAlignment.Right)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets os sets a horizontal alignment of the dashboard's toolbar elements.")]
        public StiHorAlignment DashboardToolbarHorAlignment { get; set; } = StiHorAlignment.Right;

        /// <summary>
        /// Gets or sets a value which indicates that the dashboard's toolbar elements 
        /// should be in normal order or a reversible order.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates that the dashboard's toolbar elements" +
            "should be in normal order or a reversible order.")]
        public bool DashboardToolbarReverse { get; set; }

        /// <summary>
        /// Gets os sets a horizontal alignment of the report's toolbar elements.
        /// </summary>
        [DefaultValue(StiHorAlignment.Left)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets os sets a horizontal alignment of the report's toolbar elements.")]
        public StiHorAlignment ReportToolbarHorAlignment { get; set; } = StiHorAlignment.Left;

        /// <summary>
        /// Gets or sets a value which indicates that the report's toolbar elements 
        /// should be in normal order or a reversible order.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates that the report's toolbar elements" +
            "should be in normal order or a reversible order.")]
        public bool ReportToolbarReverse { get; set; }
        #endregion
    }
}