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
using System.Reflection;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Data.Engine
{
    /// <summary>
    /// Class contains methods to work with filters.
    /// </summary>
	public sealed class StiDataFilterHelper
	{
		public static StiDataFilterCondition ConvertStringToCondition(string condition)
		{
		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionEqualTo"))
		        return StiDataFilterCondition.EqualTo;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionNotEqualTo"))
		        return StiDataFilterCondition.NotEqualTo;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionBetween"))
		        return StiDataFilterCondition.Between;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionNotBetween"))
		        return StiDataFilterCondition.NotBetween;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionGreaterThan"))
		        return StiDataFilterCondition.GreaterThan;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionGreaterThanOrEqualTo"))
		        return StiDataFilterCondition.GreaterThanOrEqualTo;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionLessThan"))
		        return StiDataFilterCondition.LessThan;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionLessThanOrEqualTo"))
		        return StiDataFilterCondition.LessThanOrEqualTo;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionContaining"))
		        return StiDataFilterCondition.Containing;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionNotContaining"))
		        return StiDataFilterCondition.NotContaining;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionBeginningWith"))
		        return StiDataFilterCondition.BeginningWith;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionEndingWith"))
		        return StiDataFilterCondition.EndingWith;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionIsNull"))
		        return StiDataFilterCondition.IsNull;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionIsNotNull"))
		        return StiDataFilterCondition.IsNotNull;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionIsBlank"))
		        return StiDataFilterCondition.IsBlank;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionIsNotBlank"))
		        return StiDataFilterCondition.IsNotBlank;

            return StiDataFilterCondition.EqualTo;
		}
		
		public static string ConvertConditionToString(StiDataFilterCondition condition)
		{
			switch (condition)
			{
				case StiDataFilterCondition.EqualTo:
					return Loc.Get("PropertyEnum", "StiFilterConditionEqualTo");

				case StiDataFilterCondition.NotEqualTo:
					return Loc.Get("PropertyEnum", "StiFilterConditionNotEqualTo");

				case StiDataFilterCondition.Between:
					return Loc.Get("PropertyEnum", "StiFilterConditionBetween");

				case StiDataFilterCondition.NotBetween:
					return Loc.Get("PropertyEnum", "StiFilterConditionNotBetween");

				case StiDataFilterCondition.GreaterThan:
					return Loc.Get("PropertyEnum", "StiFilterConditionGreaterThan");

				case StiDataFilterCondition.GreaterThanOrEqualTo:
					return Loc.Get("PropertyEnum", "StiFilterConditionGreaterThanOrEqualTo");

				case StiDataFilterCondition.LessThan:
					return Loc.Get("PropertyEnum", "StiFilterConditionLessThan");

				case StiDataFilterCondition.LessThanOrEqualTo:
					return Loc.Get("PropertyEnum", "StiFilterConditionLessThanOrEqualTo");

				case StiDataFilterCondition.Containing:
					return Loc.Get("PropertyEnum", "StiFilterConditionContaining");

				case StiDataFilterCondition.NotContaining:
					return Loc.Get("PropertyEnum", "StiFilterConditionNotContaining");

				case StiDataFilterCondition.BeginningWith:
					return Loc.Get("PropertyEnum", "StiFilterConditionBeginningWith");

				case StiDataFilterCondition.EndingWith:
					return Loc.Get("PropertyEnum", "StiFilterConditionEndingWith");
            
                case StiDataFilterCondition.IsNull:
                    return Loc.Get("PropertyEnum", "StiFilterConditionIsNull");

                case StiDataFilterCondition.IsNotNull:
                    return Loc.Get("PropertyEnum", "StiFilterConditionIsNotNull");

			    case StiDataFilterCondition.IsBlank:
			        return Loc.Get("PropertyEnum", "StiFilterConditionIsBlank");

			    case StiDataFilterCondition.IsNotBlank:
			        return Loc.Get("PropertyEnum", "StiFilterConditionIsNotBlank");
            }
			return string.Empty;
		}
	}
}
