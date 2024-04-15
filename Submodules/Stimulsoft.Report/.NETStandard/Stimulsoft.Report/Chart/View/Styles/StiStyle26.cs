﻿#region Copyright (C) 2003-2022 Stimulsoft
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

namespace Stimulsoft.Report.Chart
{
    public class StiStyle26 : StiChartStyle
    {
        #region Properties
        public override bool AllowDashboard => true;

        public override string DashboardName => StiLocalization.Get("PropertyColor", "Turquoise");

        public override StiElementStyleIdent StyleIdent => StiElementStyleIdent.Turquoise;
        #endregion

        #region Methods.override
        public override bool IsOffice2015Style
        {
            get
            {
                return true;
            }
        }

        public override StiChartStyle CreateNew()
        {
            return new StiStyle26();
        }
        #endregion

        public StiStyle26()
        {
            this.Core = new StiStyleCoreXF26();
        }
    }
}