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

using Stimulsoft.Report.Components.Table;

namespace Stimulsoft.Report.Components
{
	public sealed class StiRestrictionsHelper
	{
		public static bool IsAllowChange(StiComponent comp)
		{
		    return 
		        comp == null 
		        || (comp.Restrictions & StiRestrictions.AllowChange) > 0 
		        || StiOptions.Designer.Restrictions.IgnoreAllowChange;
		}

        public static bool IsAllowDelete(StiRestrictions restrictions)
        {
            return
                (restrictions & StiRestrictions.AllowDelete) > 0 
                || StiOptions.Designer.Restrictions.IgnoreAllowDelete;
        }

		public static bool IsAllowDelete(StiComponent comp)
		{
		    return 
		        comp == null 
		        || (comp.Restrictions & StiRestrictions.AllowDelete) > 0 
		        || StiOptions.Designer.Restrictions.IgnoreAllowDelete;
		}

		public static bool IsAllowMove(StiComponent comp)
		{
		    return 
		        comp == null 
		        || (comp.Restrictions & StiRestrictions.AllowMove) > 0 
		        || StiOptions.Designer.Restrictions.IgnoreAllowMove;
		}

		public static bool IsAllowSelect(StiComponent comp)
		{
		    return 
		        comp == null 
		        || (comp.Restrictions & StiRestrictions.AllowSelect) > 0 
		        || StiOptions.Designer.Restrictions.IgnoreAllowSelect;
		}

		public static bool IsAllowResize(StiComponent comp)
		{
		    return 
		        comp == null || comp is IStiTableCell
		        || (comp.Restrictions & StiRestrictions.AllowResize) > 0 
		        || StiOptions.Designer.Restrictions.IgnoreAllowResize;
		}

		public static bool IsAllowChangePosition(StiComponent comp)
		{
			return 
				IsAllowMove(comp) || 
				IsAllowResize(comp);
		}
	}
}
