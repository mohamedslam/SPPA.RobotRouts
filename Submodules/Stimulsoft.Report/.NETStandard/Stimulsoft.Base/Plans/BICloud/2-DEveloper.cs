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
	    public class Developer : Trial
	    {
			public override StiPlanIdent Ident => StiPlanIdent.CloudDeveloper;

			public override string Name => "Developer";

			public override bool AllowDatabases => true;

			public override bool AllowDataTransformation => true;

			public override int MaxDataRows => 10 * K;

			public override int MaxFileSize => 10 * M;

			public override int? MaxItems => null;

			public override int MaxReportPages => 50;

			public override int MaxRefreshes => 1 * K;

			public override int MaxResources => 5;

			public override int MaxResourceSize => 4 * M;			

			public override bool IsBI => false;
		}
    }
}
