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
		public class Team : Single
		{
			public override StiPlanIdent Ident => StiPlanIdent.CloudTeam;

			public override string Name => "Team (4 Creators, Unlimited Viewers)";
						
			public override bool AllowWhiteLabel => true;

			public override int MaxDataRows => 50 * K;
			
			public override int MaxFileSize => 15 * M;

			public override int MaxRefreshes => 10 * K;

			public override int MaxReportPages => 200;			

			public override int MaxResourceSize => 10 * M;			

			public override int? MaxUsers => 4;

			public override int? MaxViewers => null;			
		}
    }
}
