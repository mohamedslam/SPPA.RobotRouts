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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using System;
using System.ComponentModel;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public class StiAxisInteraction : IStiAxisInteraction
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyBool(nameof(ShowScrollBar), ShowScrollBar);
            jObject.AddPropertyBool(nameof(RangeScrollEnabled), RangeScrollEnabled, true);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(ShowScrollBar):
                        this.ShowScrollBar = property.DeserializeBool();
                        break;

                    case nameof(RangeScrollEnabled):
                        this.RangeScrollEnabled = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region ICloneable override
        /// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public object Clone()
        {
            return this.MemberwiseClone() as IStiAxisInteraction;
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public bool IsDefault => !ShowScrollBar && RangeScrollEnabled;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets value which indicates that scroll bar will be shown.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that scroll bar will be shown.")]
        [DefaultValue(false)]
        public bool ShowScrollBar { get; set; }

        /// <summary>
        /// Gets or sets value which indicates whether the range of axis can be scrolled.
        /// </summary>
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates whether the range of axis can be scrolled.")]
        public virtual bool RangeScrollEnabled { get; set; } = true;
        #endregion
                
        public StiAxisInteraction()
        {
        }

        [StiUniversalConstructor("Interaction")]
        public StiAxisInteraction(
            bool showScrollBar,
            bool rangeScrollEnabled)
        {
            this.ShowScrollBar = showScrollBar;
            this.RangeScrollEnabled = rangeScrollEnabled;
        }
    }
}
