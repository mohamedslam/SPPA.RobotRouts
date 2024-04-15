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

using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Gauge.Design;
using Stimulsoft.Report.Gauge.Events;
using System.ComponentModel;

namespace Stimulsoft.Report.Gauge
{
    /// <summary>
    /// Describes the class that contains the expression for calculation of Skip Values.
    /// </summary>
    [TypeConverter(typeof(StiSkipValuesExpressionConverter))]
    public class StiSkipValuesExpression : StiExpression
    {
        [Browsable(false)]
        public override bool ApplyFormat => false;

        /// <summary>
        /// Returns the event for processing of the expession when generation the report script.
        /// </summary>
        /// <returns>The event for processing of the expession.</returns>
        public override StiEvent GetDefaultEvent() => new StiGetSkipValuesEvent();

        /// <summary>
        /// Gets value, indicates that this value is be a string.
        /// </summary>
        public override bool FullConvert => true;

        /// <summary>
        /// Creates a new expression.
        /// </summary>
        public StiSkipValuesExpression()
            : this(string.Empty)
        {
        }


        /// <summary>
        /// Creates a new expression.
        /// </summary>
        /// <param name="val">Expression value.</param>
        public StiSkipValuesExpression(string value)
            : base(value)
        {

        }
    }
}