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
using System.ComponentModel;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public class StiAxisTicks : IStiAxisTicks
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyFloat(nameof(LengthUnderLabels), lengthUnderLabels, 5f);
            jObject.AddPropertyFloat(nameof(Length), length, 5f);
            jObject.AddPropertyFloat(nameof(MinorLength), minorLength, 2f);
            jObject.AddPropertyInt(nameof(MinorCount), minorCount, 4);
            jObject.AddPropertyInt(nameof(Step), step);
            jObject.AddPropertyBool(nameof(MinorVisible), MinorVisible);
            jObject.AddPropertyBool(nameof(Visible), Visible, true);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(LengthUnderLabels):
                        this.lengthUnderLabels = property.DeserializeFloat();
                        break;

                    case nameof(Length):
                        this.length = property.DeserializeFloat();
                        break;

                    case nameof(MinorLength):
                        this.minorLength = property.DeserializeFloat();
                        break;

                    case nameof(MinorCount):
                        this.minorCount = property.DeserializeInt();
                        break;

                    case nameof(Step):
                        this.step = property.DeserializeInt();
                        break;

                    case nameof(MinorVisible):
                        this.MinorVisible = property.DeserializeBool();
                        break;

                    case nameof(Visible):
                        this.Visible = property.DeserializeBool();
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
            return this.MemberwiseClone();
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public bool IsDefault
        {
            get
            {
                return 
                    LengthUnderLabels == 5f 
                    && Length == 5f 
                    && MinorLength == 2f 
                    && MinorCount == 4 
                    && Step == 0 
                    && !MinorVisible 
                    && Visible;
            }
        }
        #endregion

        #region Properties
        private float lengthUnderLabels = 5f;
        /// <summary>
        /// Gets or sets length of one major tick under labels.
        /// </summary>
        [DefaultValue(5f)]
        [StiSerializable]
        [Description("Gets or sets length of one major tick under labels.")]
        public float LengthUnderLabels
        {
            get
            {
                return lengthUnderLabels;
            }
            set
            {
                if (value > 0)
                    lengthUnderLabels = value;
            }
        }

        private float length = 5f;
        /// <summary>
        /// Gets or sets length of one major tick.
        /// </summary>
		[DefaultValue(5f)]
        [StiSerializable]
        [Description("Gets or sets length of one major tick.")]
        public float Length
        {
            get
            {
                return length;
            }
            set
            {
                if (value > 0)
                    length = value;
            }
        }

        private float minorLength = 2f;
        /// <summary>
        /// Gets or sets length of one minor tick.
        /// </summary>
		[DefaultValue(2f)]
        [StiSerializable]
        [Description("Gets or sets length of one minor tick.")]
        public float MinorLength
        {
            get
            {
                return minorLength;
            }
            set
            {
                if (value > 0)
                    minorLength = value;
            }
        }

        private int minorCount = 4;
        /// <summary>
        /// Gets or sets count of minor ticks between two major ticks.
        /// </summary>
		[DefaultValue(4)]
        [StiSerializable]
        [Description("Gets or sets count of minor ticks between two major ticks.")]
        public int MinorCount
        {
            get
            {
                return minorCount;
            }
            set
            {
                if (value >= 0)
                    minorCount = value;
            }
        }

        private int step;
        /// <summary>
        /// Gets or sets value which indicates on which steps major ticks will be displayed.
        /// </summary>
		[DefaultValue(0)]
        [StiSerializable]
        [Description("Gets or sets value which indicates on which steps major ticks will be displayed.")]
        public int Step
        {
            get
            {
                return step;
            }
            set
            {
                if (value >= 0)
                    step = value;
            }
        }

        /// <summary>
        /// Gets or sets visibility of minor ticks.
        /// </summary>
		[StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets visibility of minor ticks.")]
        public virtual bool MinorVisible { get; set; }

        /// <summary>
        /// Gets or sets visility of major ticks.
        /// </summary>
		[StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets visility of major ticks.")]
        public virtual bool Visible { get; set; } = true;
        #endregion
                
        public StiAxisTicks()
        {
        }

        public StiAxisTicks(
            bool visible,
            float length,
            bool minorVisible,
            float minorLength,
            int minorCount,
            int step
            )
            : this(visible, length, minorVisible, minorLength, minorCount, step, length)
        {
        }

        [StiUniversalConstructor("Ticks")]
        public StiAxisTicks(
            bool visible,
            float length,
            bool minorVisible,
            float minorLength,
            int minorCount,
            int step,
            float lengthUnderLabels
            )
        {
            this.Visible = visible;
            this.length = length;
            this.MinorVisible = minorVisible;
            this.minorLength = minorLength;
            this.minorCount = minorCount;
            this.step = step;
            this.lengthUnderLabels = lengthUnderLabels;
        }
    }
}
