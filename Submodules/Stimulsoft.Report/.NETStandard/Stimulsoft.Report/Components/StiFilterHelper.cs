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

using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Events;
using System;

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Class contains methods to work with filters.
    /// </summary>
	public sealed class StiFilterHelper
    {
        #region Methods
        public static StiMarkerType ConvertStringToMarkerType(string item)
        {
            if (item == StiLocalization.Get("PropertyEnum", "StiMarkerTypeCircle"))
            {
                return StiMarkerType.Circle;
            }
            else if (item == StiLocalization.Get("PropertyEnum", "StiMarkerTypeHexagon"))
            {
                return StiMarkerType.Hexagon;
            }
            else if (item == StiLocalization.Get("PropertyEnum", "StiMarkerTypeHalfCircle"))
            {
                return StiMarkerType.HalfCircle;
            }
            else if (item == StiLocalization.Get("PropertyEnum", "StiMarkerTypeRectangle"))
            {
                return StiMarkerType.Rectangle;
            }
            else if (item == StiLocalization.Get("PropertyEnum", "StiMarkerTypeStar5"))
            {
                return StiMarkerType.Star5;
            }
            else if (item == StiLocalization.Get("PropertyEnum", "StiMarkerTypeStar6"))
            {
                return StiMarkerType.Star6;
            }
            else if (item == StiLocalization.Get("PropertyEnum", "StiMarkerTypeStar7"))
            {
                return StiMarkerType.Star7;
            }
            else if (item == StiLocalization.Get("PropertyEnum", "StiMarkerTypeStar8"))
            {
                return StiMarkerType.Star8;
            }
            else if (item == StiLocalization.Get("PropertyEnum", "StiMarkerTypeTriangle"))
            {
                return StiMarkerType.Triangle;
            }

            return StiMarkerType.Circle;
        }

        public static StiFilterItem ConvertStringToItem(string item)
        {
            if (item == StiLocalization.Get("PropertyEnum", "StiFilterItemValue"))
            {
                return StiFilterItem.Value;
            }
            else if (item == StiLocalization.Get("PropertyEnum", "StiFilterItemValueEnd"))
            {
                 return StiFilterItem.ValueEnd;
            }
            else if (item == StiLocalization.Get("PropertyEnum", "StiFilterItemArgument"))
            {
                 return StiFilterItem.Argument;
            }
            else if (item == StiLocalization.Get("PropertyEnum", "StiFilterItemValueOpen"))
            {
                 return StiFilterItem.ValueOpen;
            }
            else if (item == StiLocalization.Get("PropertyEnum", "StiFilterItemValueClose"))
            {
                 return StiFilterItem.ValueClose;
            }
            else if (item == StiLocalization.Get("PropertyEnum", "StiFilterItemValueLow"))
            {
                 return StiFilterItem.ValueLow;
            }
            else if (item == StiLocalization.Get("PropertyEnum", "StiFilterItemValueHigh"))
            {
                 return StiFilterItem.ValueHigh;
            }

            return StiFilterItem.Argument;
        }

        public static string ConvertItemToString(StiFilterItem filterItem)
        {
            string item = null;

            switch (filterItem)
            {
                case StiFilterItem.Value:
                    item = StiLocalization.Get("PropertyEnum", "StiFilterItemValue");
                    break;

                case StiFilterItem.ValueEnd:
                    item = StiLocalization.Get("PropertyEnum", "StiFilterItemValueEnd");
                    break;

                case StiFilterItem.Argument:
                    item = StiLocalization.Get("PropertyEnum", "StiFilterItemArgument");
                    break;

                case StiFilterItem.ValueOpen:
                    item = StiLocalization.Get("PropertyEnum", "StiFilterItemValueOpen");
                    break;

                case StiFilterItem.ValueClose:
                    item = StiLocalization.Get("PropertyEnum", "StiFilterItemValueClose");
                    break;

                case StiFilterItem.ValueLow:
                    item = StiLocalization.Get("PropertyEnum", "StiFilterItemValueLow");
                    break;

                case StiFilterItem.ValueHigh:
                    item = StiLocalization.Get("PropertyEnum", "StiFilterItemValueHigh");
                    break;
            }

            return item;
        }

        public static StiFilterCondition ConvertStringToCondition(string condition)
		{
		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionEqualTo"))
		        return StiFilterCondition.EqualTo;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionNotEqualTo"))
		        return StiFilterCondition.NotEqualTo;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionBetween"))
		        return StiFilterCondition.Between;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionNotBetween"))
		        return StiFilterCondition.NotBetween;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionGreaterThan"))
		        return StiFilterCondition.GreaterThan;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionGreaterThanOrEqualTo"))
		        return StiFilterCondition.GreaterThanOrEqualTo;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionLessThan"))
		        return StiFilterCondition.LessThan;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionLessThanOrEqualTo"))
		        return StiFilterCondition.LessThanOrEqualTo;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionContaining"))
		        return StiFilterCondition.Containing;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionNotContaining"))
		        return StiFilterCondition.NotContaining;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionBeginningWith"))
		        return StiFilterCondition.BeginningWith;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionEndingWith"))
		        return StiFilterCondition.EndingWith;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionIsNull"))
		        return StiFilterCondition.IsNull;

		    if (condition == Loc.Get("PropertyEnum", "StiFilterConditionIsNotNull"))
		        return StiFilterCondition.IsNotNull;

		    return StiFilterCondition.EqualTo;
		}
        
        public static string ConvertConditionToString(StiFilterCondition condition)
		{
			switch (condition)
			{
				case StiFilterCondition.EqualTo:
					return Loc.Get("PropertyEnum", "StiFilterConditionEqualTo");

				case StiFilterCondition.NotEqualTo:
					return Loc.Get("PropertyEnum", "StiFilterConditionNotEqualTo");

				case StiFilterCondition.Between:
					return Loc.Get("PropertyEnum", "StiFilterConditionBetween");

				case StiFilterCondition.NotBetween:
					return Loc.Get("PropertyEnum", "StiFilterConditionNotBetween");

				case StiFilterCondition.GreaterThan:
					return Loc.Get("PropertyEnum", "StiFilterConditionGreaterThan");

				case StiFilterCondition.GreaterThanOrEqualTo:
					return Loc.Get("PropertyEnum", "StiFilterConditionGreaterThanOrEqualTo");

				case StiFilterCondition.LessThan:
					return Loc.Get("PropertyEnum", "StiFilterConditionLessThan");

				case StiFilterCondition.LessThanOrEqualTo:
					return Loc.Get("PropertyEnum", "StiFilterConditionLessThanOrEqualTo");

				case StiFilterCondition.Containing:
					return Loc.Get("PropertyEnum", "StiFilterConditionContaining");

				case StiFilterCondition.NotContaining:
					return Loc.Get("PropertyEnum", "StiFilterConditionNotContaining");

				case StiFilterCondition.BeginningWith:
					return Loc.Get("PropertyEnum", "StiFilterConditionBeginningWith");

				case StiFilterCondition.EndingWith:
					return Loc.Get("PropertyEnum", "StiFilterConditionEndingWith");
            
                case StiFilterCondition.IsNull:
                    return Loc.Get("PropertyEnum", "StiFilterConditionIsNull");

                case StiFilterCondition.IsNotNull:
                    return Loc.Get("PropertyEnum", "StiFilterConditionIsNotNull");
            }
			return string.Empty;
		}
		
		public static StiFilterDataType ConvertStringToDataType(string dataType)
		{
		    if (dataType == Loc.Get("PropertyEnum", "StiFilterDataTypeString"))
		        return StiFilterDataType.String;

		    if (dataType == Loc.Get("PropertyEnum", "StiFilterDataTypeNumeric"))
		        return StiFilterDataType.Numeric;

		    if (dataType == Loc.Get("PropertyEnum", "StiFilterDataTypeDateTime"))
		        return StiFilterDataType.DateTime;

		    if (dataType == Loc.Get("PropertyEnum", "StiFilterDataTypeBoolean"))
		        return StiFilterDataType.Boolean;

		    if (dataType == Loc.Get("PropertyEnum", "StiFilterDataTypeExpression"))
		        return StiFilterDataType.Expression;

		    return StiFilterDataType.String;
		}

		public static string ConvertDataTypeToString(StiFilterDataType dataType)
		{
			switch (dataType)
			{
				case StiFilterDataType.String:
					return Loc.Get("PropertyEnum", "StiFilterDataTypeString");

				case StiFilterDataType.Numeric:
					return Loc.Get("PropertyEnum", "StiFilterDataTypeNumeric");

				case StiFilterDataType.DateTime:
					return Loc.Get("PropertyEnum", "StiFilterDataTypeDateTime");

				case StiFilterDataType.Boolean:
					return Loc.Get("PropertyEnum", "StiFilterDataTypeBoolean");

				case StiFilterDataType.Expression:
					return Loc.Get("PropertyEnum", "StiFilterDataTypeExpression");
			}
			return string.Empty;
		}

		public static void SetFilter(StiComponent comp)
		{
			var filter = comp as IStiFilter;			
            var dataSource = comp as IStiDataSource;
            var businessObject = comp as IStiBusinessObject;

		    if (dataSource.IsDataSourceEmpty && businessObject.IsBusinessObjectEmpty) return;
		    if (filter.FilterMethodHandler != null || !filter.FilterOn) return;

		    var correctedDataName = StiNameValidator.CorrectName(comp.Name, comp.Report);

		    var type = comp.Report.GetType();
		    var method = type.GetMethod($"{correctedDataName}__GetFilter", new[]
		    {
		        typeof(object),
		        typeof(StiFilterEventArgs)
		    });

		    if (method == null) return;

		    try
		    {
		        filter.FilterMethodHandler = Delegate.CreateDelegate(
		            typeof(StiFilterEventHandler), comp.Report, correctedDataName + "__GetFilter") 
		            as StiFilterEventHandler;
						
		    }
		    catch (Exception e)
		    {
		        StiLogService.Write(comp.GetType(), "StiFilterEventHandler...ERROR");
		        StiLogService.Write(comp.GetType(), e);
		    }
		}
        #endregion
    }
}
