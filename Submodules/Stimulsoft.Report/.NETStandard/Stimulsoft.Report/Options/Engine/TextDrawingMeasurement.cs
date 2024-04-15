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

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// A class which controls of settings of the report engine. 
        /// </summary>
		public sealed partial class Engine
        {
			public sealed class TextDrawingMeasurement
			{
			    [DefaultValue(1d)]
			    [StiSerializable]
			    public static double MeasurementFactorStandard { get; set; } = 1d;

			    [DefaultValue(1d)]
			    [StiSerializable]
			    public static double MeasurementFactorWysiwyg { get; set; } = 1d;

			    [DefaultValue(1d)]
                [StiSerializable]
                public static double MeasurementFactorTypographic { get; set; } = 1d;

			    [DefaultValue(1d)]
                [StiSerializable]
                public static double MeasurementFactorWpf { get; set; } = 1d;

			    [DefaultValue(1.1d)]
			    [StiSerializable]
			    public static double MeasurementFactorWpfWithHtmlInCrossTab { get; set; } = 1.1d;

                [DefaultValue(false)]
                [StiSerializable]
                public static bool MeasurementWpfCompatibility2016 { get; set; }
            }
        }
    }
}