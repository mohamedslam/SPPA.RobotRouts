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
    public class StiStyle25 : StiChartStyle
    {
        #region Properties
        public override bool AllowDashboard => true;

        public override string DashboardName => StiLocalization.Get("PropertyColor", "Green");

        public override StiElementStyleIdent StyleIdent => StiElementStyleIdent.Green;
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
            return new StiStyle25();
        }
        #endregion

        public StiStyle25()
        {
            this.Core = new StiStyleCoreXF25();
        }
    }
}