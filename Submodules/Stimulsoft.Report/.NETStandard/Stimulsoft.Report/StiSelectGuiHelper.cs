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
using System.ComponentModel;

namespace Stimulsoft.Report
{
	public static class StiSelectGuiHelper
	{
        /// <summary>
        /// StiSelectGuiHelper.IsRibbonGui is obsolete. Please use StiOptions.Windows.GlobalGuiStyle property.
        /// </summary>
        [Obsolete("StiSelectGuiHelper.IsRibbonGui is obsolete. Please use StiOptions.Windows.GlobalGuiStyle property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool IsRibbonGui
		{
			get
			{
                return StiOptions.Windows.IsRibbonGuiEnabled();
			}
			set
			{
			    StiOptions.Windows.GlobalGuiStyle = value 
			        ? StiGlobalGuiStyle.Office2007Blue 
			        : StiGlobalGuiStyle.Office2003Blue;
			}
		}
	}
}
