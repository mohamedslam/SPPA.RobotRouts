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

using System.ComponentModel;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Painters;
using Stimulsoft.Base.Json.Linq;
using System;
using Stimulsoft.Base;
using System.Drawing;

namespace Stimulsoft.Report.Components
{
    [StiGdiIndicatorTypePainter(typeof(Stimulsoft.Report.Painters.StiIconSetGdiIndicatorTypePainter))]
    [StiWpfIndicatorTypePainter("Stimulsoft.Report.Painters.StiIconSetWpfIndicatorTypePainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    public class StiIconSetIndicator : 
        StiIndicator
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiIconSetIndicator
            jObject.AddPropertyEnum("Icon", Icon, StiIcon.None);
            jObject.AddPropertyEnum("Alignment", Alignment, ContentAlignment.MiddleLeft);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Icon":
                        this.Icon = property.DeserializeEnum<StiIcon>();
                        break;

                    case "Alignment":
                        this.Alignment = property.DeserializeEnum<ContentAlignment>();
                        break;
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a icon of indicator.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiIcon.None)]
        public StiIcon Icon { get; set; } = StiIcon.None;

        /// <summary>
        /// Gets or sets a icon alignment of indicator.
        /// </summary>
        [StiSerializable]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        public ContentAlignment Alignment { get; set; } = ContentAlignment.MiddleLeft;

        public byte[] CustomIcon { get; set; }

        public Size? CustomIconSize { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new object of the type StiIconSetIndicator.
		/// </summary>
        public StiIconSetIndicator()
        {
        }

        /// <summary>
        /// Creates a new object of the type StiIconSetIndicator.
		/// </summary>
        public StiIconSetIndicator(StiIcon icon, ContentAlignment alignment)
		{
            this.Icon = icon;
            this.Alignment = alignment;
        }
        #endregion
    }
}