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

using Stimulsoft.Report.Engine;

namespace Stimulsoft.Report.Components
{	
	public class StiGroupHeaderBandInfoV2 : StiComponentInfo
	{
		/// <summary>
		/// Если true, то свойство KeepGroupTogether игнорируется. Используется для игнорирования удержания группы на первой странице.
		/// </summary>
		public bool SkipKeepGroups = false;

		/// <summary>
		/// Gets or sets the group of footers which fits to this header.
		/// </summary>
		public StiGroupFooterBand GroupFooter = null;

		public bool SilentModeEnabled = false;

		public bool OldSilentMode = false;

        internal bool IsTableGroupHeader = false;
	}
}
