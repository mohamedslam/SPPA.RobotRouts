#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

namespace Stimulsoft.Report.Dashboard
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public class StiTableColumnSize :
        ICloneable, 
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyInt("Width", Width);
            jObject.AddPropertyInt("MinWidth", MinWidth, 30);
            jObject.AddPropertyInt("MaxWidth", MaxWidth, 300);
            jObject.AddPropertyBool("WordWrap", WordWrap);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {

                    case "Width":
                        this.Width = property.DeserializeInt();
                        break;

                    case "MinWidth":
                        this.MinWidth = property.DeserializeInt();
                        break;

                    case "MaxWidth":
                        this.MaxWidth = property.DeserializeInt();
                        break;

                    case "WordWrap":
                        WordWrap = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region ICloneable
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion        

        #region Properties
        private int width = 0;
        /// <summary>
        /// Gets or sets the specified width for the table column. The width will be calculated automatically by the table element if a value equal to zero.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0)]
        [Description("Gets or sets the specified width for the table column. The width will be calculated automatically by the table element if a value equal to zero.")]
        [RefreshProperties(RefreshProperties.All)]
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                if (width != value)
                {
                    if (value < 0)
                        value = 0;                    

                    width = value;
                }
            }
        }
        
        private int minWidth = 30;
        /// <summary>
        /// Gets or sets a minimum width of the table column.
        /// </summary>
        [StiSerializable]
        [DefaultValue(30)]
        [Description("Gets or sets a minimum width of the table column.")]
        [RefreshProperties(RefreshProperties.All)]
        public int MinWidth
        {
            get
            {
                return minWidth;
            }
            set
            {
                if (minWidth != value)
                {
                    if (value < 0)
                        value = 0;
                    
                    minWidth = value;
                }
            }
        }
                
        private int maxWidth = 300;
        /// <summary>
        /// Gets or sets a maximum width of the table column.
        /// </summary>
        [StiSerializable]
        [DefaultValue(300)]
        [Description("Gets or sets a maximum width of the table column.")]
        [RefreshProperties(RefreshProperties.All)]
        public int MaxWidth
        {
            get
            {
                return maxWidth;
            }
            set
            {
                if (maxWidth != value)
                {
                    if (value <= 0)
                        value = 300;

                    maxWidth = value;
                }
            }
        }

        /// <summary>
		/// Gets or sets word wrap.
		/// </summary>
		[StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets word wrap.")]
        public bool WordWrap { get; set; }

        [Browsable(false)]
        public virtual bool IsDefault =>
            Width == 0 &&
            MinWidth == 30 &&
            MaxWidth == 300 &&
            !WordWrap;
        #endregion

        #region Methods
        public void CheckRules()
        {
            if (MaxWidth < MinWidth)
                MinWidth = MaxWidth;

            if (Width < MinWidth && Width != 0)
                Width = MinWidth;

            if (Width > MaxWidth)
                Width = MaxWidth;
        }

        public int GetUniqueCode()
        {
            unchecked
            {
                var hashCode = (int)Width;
                hashCode = (hashCode * 297) ^ MinWidth;
                hashCode = (hashCode * 297) ^ MaxWidth;
                hashCode = (hashCode * 297) ^ WordWrap.GetHashCode();
                return hashCode;
            }
        }
        #endregion

        public StiTableColumnSize()
        {
        }

        [StiUniversalConstructor("Size")]
        public StiTableColumnSize(int width, int minWidth, int maxWidth, bool wordWrap)
        {
            this.Width = width;
            this.MinWidth = minWidth;
            this.MaxWidth = maxWidth;
            this.WordWrap = wordWrap;
        }
    }
}
