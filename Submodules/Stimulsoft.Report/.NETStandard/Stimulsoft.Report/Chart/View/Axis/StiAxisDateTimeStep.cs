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
    public class StiAxisDateTimeStep : IStiAxisDateTimeStep
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyEnum(nameof(Step), Step, StiTimeDateStep.None);
            jObject.AddPropertyInt(nameof(NumberOfValues), NumberOfValues, 1);
            jObject.AddPropertyBool(nameof(Interpolation), Interpolation);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Step):
                        this.Step = property.DeserializeEnum<StiTimeDateStep>();
                        break;

                    case nameof(NumberOfValues):
                        this.NumberOfValues = property.DeserializeInt();
                        break;

                    case nameof(Interpolation):
                        this.Interpolation = property.DeserializeBool();
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
                    Step == StiTimeDateStep.None
                    && NumberOfValues == 1
                    && !Interpolation;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value that indicates with what the time step values will be shown.
        /// </summary>
        [DefaultValue(StiTimeDateStep.None)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets a value that indicates with what the time step values will be shown.")]
        public StiTimeDateStep Step { get; set; } = StiTimeDateStep.None;

        /// <summary>
        /// Gets or sets number of values in step.
        /// </summary>
        [DefaultValue(1)]
        [StiSerializable]
        [Description("Gets or sets number of values in step.")]
        public int NumberOfValues { get; set; } = 1;

        /// <summary>
        /// Gets or sets A value indicates that the values ​​will be interpolated.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [Description("Gets or sets A value indicates that the values ​​will be interpolated.")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        public bool Interpolation { get; set; }
        #endregion        

        public StiAxisDateTimeStep()
        {
        }

        public StiAxisDateTimeStep(StiTimeDateStep step, int numberOfValues)
        {
            this.Step = step;
            this.NumberOfValues = numberOfValues;
        }

        [StiUniversalConstructor("DateTimeStep")]
        public StiAxisDateTimeStep(StiTimeDateStep step, int numberOfValues, bool interpolation)
        {
            this.Step = step;
            this.NumberOfValues = numberOfValues;
            this.Interpolation = interpolation;
        }
    }
}
