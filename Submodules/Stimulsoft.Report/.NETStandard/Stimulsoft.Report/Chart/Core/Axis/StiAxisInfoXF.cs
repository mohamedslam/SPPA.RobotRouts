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

using System;
using System.Collections.Generic;

namespace Stimulsoft.Report.Chart
{
    public class StiAxisInfoXF : ICloneable
    {
        #region ICloneable
        public object Clone()
        {
            var axis = this.MemberwiseClone() as StiAxisInfoXF;
            axis.StripLines = this.StripLines.Clone() as StiStripLinesXF;
            return axis;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Factor transformations value to pixels on axis.
        /// </summary>
        public double Dpi { get; set; } = 0;

        public float Step { get; set; } = 0;

        /// <summary>
        /// Difference between minimum and maximum value on axis.
        /// </summary>
        public double Range
        {
            get
            {
                return Maximum - Minimum;
            }
        }

        /// <summary>
        /// Collection of strip on axis.
        /// </summary>
        public StiStripLinesXF StripLines;

        /// <summary>
        /// Array of strip position on axis.
        /// </summary>
        public float[] StripPositions = null;

        /// <summary>
        /// Array of ticks position on axis.
        /// </summary>
        public List<StiStripPositionXF> TicksCollection = null;

        /// <summary>
        /// Array of labels position on axis.
        /// </summary>
        public List<StiStripPositionXF> LabelsCollection = null;

        /// <summary>
        /// Array of labels to draw
        /// </summary>
        public List<StiAxisLabelInfoXF> LabelInfoCollection = null;

        /// <summary>
        /// Minimum value on axis.
        /// </summary>
        public double Minimum = 0;

        /// <summary>
        /// Maximum value on axis.
        /// </summary>
        public double Maximum = 0;
        #endregion
    }
}
