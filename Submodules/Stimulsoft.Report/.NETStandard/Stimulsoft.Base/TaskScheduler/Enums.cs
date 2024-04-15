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

namespace Stimulsoft.Base.TaskScheduler
{
	#region StiQuickTriggerType
	public enum StiQuickTriggerType
	{
		Once,

		/// <summary>Hourly, starting now.</summary>
		Hourly,

		/// <summary>Daily, starting now.</summary>
		Daily,

		/// <summary>Weekly, starting now.</summary>
		Weekly,

		/// <summary>Monthly, starting now.</summary>
		Monthly,

		/// <summary>At boot.</summary>
		Boot,

		/// <summary>On system idle.</summary>
		Idle,

		/// <summary>At logon of any user.</summary>
		Logon,

		/// <summary>When the task is registered.</summary>
		TaskRegistration,
	}
	#endregion
}