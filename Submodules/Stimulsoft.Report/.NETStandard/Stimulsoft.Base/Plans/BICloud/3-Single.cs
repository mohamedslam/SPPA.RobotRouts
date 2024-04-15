#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

namespace Stimulsoft.Base.Plans
{
	public static partial class StiCloudPlans
	{
		public class Single : Developer
		{
			public override StiPlanIdent Ident => StiPlanIdent.CloudSingle;

			public override string Name => "Single (1 creator)";

			public override int MaxDataRows => 20 * K;

			public override int MaxFileSize => 10 * M;

			public override int MaxReportPages => 100;

			public override int MaxRefreshes => 3 * K;

			public override int MaxResources => 10;

			public override int MaxResourceSize => 5 * M;			

			public override bool IsBI => true;
		}
    }
}
