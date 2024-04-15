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

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Stimulsoft.Report.Components
{
	[SuppressUnmanagedCodeSecurity]
	internal class StiPageWin32
	{
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
		internal static extern int GetDeviceCaps(IntPtr hDc, int nIndex);

		internal const short HORZSIZE		 = 4;
		internal const short VERTSIZE		 = 6;
		internal const short HORZRES		 = 8;
		internal const short VERTRES		 = 10;
		internal const short PHYSICALOFFSETX = 112;
		internal const short PHYSICALOFFSETY = 113;
	}
}
