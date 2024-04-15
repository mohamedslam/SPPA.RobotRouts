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
	    public class Trial : StiCloudPlan
		{
			public override StiPlanIdent Ident => StiPlanIdent.CloudTrial;

			public override string Name => "Trial";

			public override bool AllowDatabases => false;
			
			public override bool AllowDataTransformation => false;

			public override bool AllowWhiteLabel => false;

			public override int MaxDataRows => 5 * K;

			public override int MaxFileSize => 5 * M;

			public override int? MaxItems => 20;

			public override int MaxReportPages => 20;

			public override int MaxRefreshes => 500;

			public override int MaxResources { get; } = 3;

			public override int MaxResourceSize => 2 * M;

			public override int? MaxUsers => 1;

			public override int? MaxViewers => null;

			public override bool IsBI => true;			

		}
    }
}
