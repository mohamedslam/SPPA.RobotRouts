#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using System;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Base.Drawing
{
    /// <summary>
    /// Class describes a cap.
    /// </summary>
    [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiCapConverter))]
    [RefreshProperties(RefreshProperties.All)]
    [StiReferenceIgnore]
    [JsonObject]
    public class StiCap : 
        ICloneable
    {
        #region enum CapOrder
        public enum CapOrder
        {
            Color = 100,
            Fill = 110,
            Width = 120,
            Height = 130,
            Style = 140
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public virtual object Clone()
        {
            var cloneCap = (StiCap)this.MemberwiseClone();

            cloneCap.Width = this.Width;
            cloneCap.Height = this.Height;
            cloneCap.Style = this.Style;
            cloneCap.Fill = this.Fill;
            cloneCap.Color = this.Color;

            return cloneCap;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets width of the cap.
        /// </summary>
        [Browsable(true)]
        [StiSerializable]
        [DefaultValue(10)]
        [RefreshProperties(RefreshProperties.All)]
        [StiOrder((int)CapOrder.Width)]
        [Description("Gets or sets width of the cap.")]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets a cap style.
        /// </summary>
        [Browsable(true)]
        [StiSerializable]
        [DefaultValue(StiCapStyle.None)]
        [RefreshProperties(RefreshProperties.All)]
        [StiOrder((int)CapOrder.Style)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Description("Gets or sets a cap style.")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiCapStyleEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public StiCapStyle Style { get; set; }

        /// <summary>
        /// Gets or sets height of the cap.
        /// </summary>
        [Browsable(true)]
        [StiSerializable]
        [DefaultValue(10)]
        [RefreshProperties(RefreshProperties.All)]
        [StiOrder((int)CapOrder.Height)]
        [Description("Gets or sets height of the cap.")]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets fill mode of the cap.
        /// </summary>
        [Browsable(true)]
        [StiSerializable]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiOrder((int)CapOrder.Fill)]
        [Description("Gets or sets fill mode of the cap.")]
        public bool Fill { get; set; }

        /// <summary>
        /// Gets or sets cap color.
        /// </summary>
        [Browsable(true)]
        [StiSerializable]
        [StiOrder((int)CapOrder.Color)]
        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter(typeof(StiColorConverter))]
        [Description("Gets or sets cap color.")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color Color { get; set; }
        #endregion

        public StiCap() : this(10, StiCapStyle.None, 10, true, Color.Black)
        {
        }

        public StiCap(int width, StiCapStyle style, int height, bool fill, Color color)
        {
            this.Width = width;
            this.Style = style;
            this.Height = height;
            this.Fill = fill;
            this.Color = color;
        }
    }
}
