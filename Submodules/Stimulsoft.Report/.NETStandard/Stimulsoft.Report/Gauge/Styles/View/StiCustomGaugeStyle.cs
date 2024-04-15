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

namespace Stimulsoft.Report.Gauge
{
    public class StiCustomGaugeStyle : StiGaugeStyleXF27
    {
        #region ServiceName
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string ServiceName => "CustomStyle";
        #endregion

        #region Properties
        [Browsable(false)]
        public StiCustomGaugeStyleCoreXF CustomCore => Core as StiCustomGaugeStyleCoreXF;
        #endregion

        public StiCustomGaugeStyle()
			: this(null)
		{
            this.Core = new StiCustomGaugeStyleCoreXF(null);
        }

        public StiCustomGaugeStyle(StiGaugeStyle style)
        {
            this.Core = new StiCustomGaugeStyleCoreXF(style);
        }
    }
}
