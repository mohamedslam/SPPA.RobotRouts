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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Serializing;
using System.ComponentModel;

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes the base class for static bands.
    /// </summary>
    [StiToolbox(false)]
    public abstract class StiStaticBand : StiBand
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("CanBreak");

            return jObject;
        }
        #endregion

        #region IStiBreakable override
        [StiNonSerialized]
        [Browsable(false)]
        public override bool CanBreak
        {
            get
            {
                return base.CanBreak;
            }
            set
            {
                base.CanBreak = value;
            }
        }

        /// <summary>
        /// Divides content of components in two parts. Returns result of dividing. If true, then component is successful divided.
        /// </summary>
        /// <param name="dividedComponent">Component for store part of content.</param>
        /// <returns>If true, then component is successful divided.</returns>
        public override bool Break(StiComponent dividedComponent, double devideFactor, ref double divLine)
        {
            return false;
        }
        #endregion

        #region Component Override
        /// <summary>
        /// Gets the type of processing when printing.
        /// </summary>
        public override StiComponentType ComponentType => StiComponentType.Static;
        #endregion

        /// <summary>
        /// Creates a new static band.
        /// </summary>
        public StiStaticBand() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new static band.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiStaticBand(RectangleD rect) : base(rect)
        {
        }
    }
}