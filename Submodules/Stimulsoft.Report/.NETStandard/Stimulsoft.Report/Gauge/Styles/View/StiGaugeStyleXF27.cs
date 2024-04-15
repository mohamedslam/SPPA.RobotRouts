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

using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Dashboard;

namespace Stimulsoft.Report.Gauge
{
    public class StiGaugeStyleXF27 : StiGaugeStyleXF
    {
        #region Properties.override
        public override bool AllowDashboard => true;

        public override string DashboardName => StiLocalization.Get("PropertyColor", "SlateGray");

        public override StiElementStyleIdent StyleIdent => StiElementStyleIdent.SlateGray;
        #endregion

        #region Methods.override
        public override StiGaugeStyleXF CreateNew() => new StiGaugeStyleXF27();
        #endregion

        public StiGaugeStyleXF27()
        {
            this.Core = new StiGaugeStyleCoreXF27();
        }
    }
}