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

using Stimulsoft.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Components.Design
{
    /// <summary>
    /// Converts StiCondition from one data type to another. 
    /// </summary>
    public class StiConditionConverter : TypeConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return false; 
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, 
			object value, Type destinationType) 
		{
			#region Convert to InstanceDescriptor
			if (destinationType == typeof(InstanceDescriptor) && value != null)
            {
                var descriptor = ConvertMultiConditionToInstanceDescriptor(value);
                if (descriptor != null) return descriptor;

                descriptor = ConvertExpressionConditionToInstanceDescriptor(value);
                if (descriptor != null) return descriptor;

                descriptor = ConvertConditionToInstanceDescriptor(value);
                if (descriptor != null) return descriptor;

                descriptor = ConvertDataBarConditionToInstanceDescriptor(value);
                if (descriptor != null) return descriptor;

                descriptor = ConvertColorScaleConditionToInstanceDescriptor(value);
                if (descriptor != null) return descriptor;

                descriptor = ConvertIconSetConditionToInstanceDescriptor(value);
                if (descriptor != null) return descriptor;
            }
			#endregion
			
			#region Convert to string
			else if (destinationType == typeof(string))
            {
                var str = ConvertMultiConditionToString(value);
                if (str != null) return str;

                str = ConvertExpressionConditionToString(value);
                if (str != null) return str;

                str = ConvertConditionToString(value);
                if (str != null) return str;
                
                str = ConvertDataBarConditionToString(value);
                if (str != null) return str;

                str = ConvertColorScaleToString(value);
                if (str != null) return str;

                str = ConvertIconSetConditionToString(value);
                if (str != null) return str;
            }
			#endregion

			return base.ConvertTo(context, culture, value, destinationType);
		}

        private string ConvertConditionToString(object value)
        {
            var condition = value as StiCondition;
            if (condition == null) return null;

            #region condition.BorderSides == StiConditionBorderSides.NotAssigned, Args(11)
            if (string.IsNullOrEmpty(condition.Style) && condition.BorderSides == StiConditionBorderSides.NotAssigned)
            {
                return string.Format(
                    "{0}{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                    condition.BreakIfTrue ? "BreakIfTrue," : string.Empty,
                    XmlConvert.EncodeName(condition.Column),
                    condition.Condition,
                    XmlConvert.EncodeName(condition.Value1),
                    XmlConvert.EncodeName(condition.Value2),
                    condition.DataType,
                    StiReportObjectStringConverter.ConvertColorToString(condition.TextColor),
                    StiReportObjectStringConverter.ConvertColorToString(condition.BackColor),
                    XmlConvert.EncodeName(StiReportObjectStringConverter.ConvertFontToString(condition.Font)),
                    condition.Enabled,
                    condition.CanAssignExpression,
                    XmlConvert.EncodeName(condition.AssignExpression));
            }
            #endregion

            #region condition.Permissions == StiConditionPermissions.All, Args(13)
            if (condition.Permissions == StiConditionPermissions.All)
            {
                return string.Format(
                    "{0}{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}",
                    condition.BreakIfTrue ? "BreakIfTrue," : string.Empty,
                    XmlConvert.EncodeName(condition.Column),
                    condition.Condition,
                    XmlConvert.EncodeName(condition.Value1),
                    XmlConvert.EncodeName(condition.Value2),
                    condition.DataType,
                    StiReportObjectStringConverter.ConvertColorToString(condition.TextColor),
                    StiReportObjectStringConverter.ConvertColorToString(condition.BackColor),
                    XmlConvert.EncodeName(StiReportObjectStringConverter.ConvertFontToString(condition.Font)),
                    condition.Enabled,
                    condition.CanAssignExpression,
                    XmlConvert.EncodeName(condition.AssignExpression),
                    XmlConvert.EncodeName(condition.Style),
                    XmlConvert.EncodeName(condition.BorderSides.ToString()));
            }
            #endregion

            #region Condition with Permissions, Args(14)
            return string.Format(
                "{0}{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
                condition.BreakIfTrue ? "BreakIfTrue," : string.Empty,
                XmlConvert.EncodeName(condition.Column),
                condition.Condition,
                XmlConvert.EncodeName(condition.Value1),
                XmlConvert.EncodeName(condition.Value2),
                condition.DataType,
                StiReportObjectStringConverter.ConvertColorToString(condition.TextColor),
                StiReportObjectStringConverter.ConvertColorToString(condition.BackColor),
                XmlConvert.EncodeName(StiReportObjectStringConverter.ConvertFontToString(condition.Font)),
                condition.Enabled,
                condition.CanAssignExpression,
                XmlConvert.EncodeName(condition.AssignExpression),
                XmlConvert.EncodeName(condition.Style),
                XmlConvert.EncodeName(condition.BorderSides.ToString()),
                XmlConvert.EncodeName(condition.Permissions.ToString()));
            #endregion
        }

        private string ConvertExpressionConditionToString(object value)
        {
            var condition = value as StiCondition;
            if (condition == null || condition.Item != StiFilterItem.Expression) return null;

            #region condition.BorderSides == StiConditionBorderSides.NotAssigned, Args(7)
            if (string.IsNullOrEmpty(condition.Style) && (condition.BorderSides == StiConditionBorderSides.NotAssigned) && (condition.Permissions == StiConditionPermissions.All))
            {
                var str = string.Format(
                    "{0}{1},{2},{3},{4},{5},{6}{7}",
                    condition.BreakIfTrue ? "BreakIfTrue," : string.Empty,
                    XmlConvert.EncodeName(condition.Expression),
                    StiReportObjectStringConverter.ConvertColorToString(condition.TextColor),
                    StiReportObjectStringConverter.ConvertColorToString(condition.BackColor),
                    XmlConvert.EncodeName(StiReportObjectStringConverter.ConvertFontToString(condition.Font)),
                    condition.Enabled,
                    condition.CanAssignExpression,
                    XmlConvert.EncodeName(condition.AssignExpression));

                return str;
            }
            #endregion

            #region condition.Permissions == StiConditionPermissions.All, Args(9)
            if (condition.Permissions == StiConditionPermissions.All)
            {
                return string.Format(
                    "{0}{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                    condition.BreakIfTrue ? "BreakIfTrue," : string.Empty,
                    XmlConvert.EncodeName(condition.Expression),
                    StiReportObjectStringConverter.ConvertColorToString(condition.TextColor),
                    StiReportObjectStringConverter.ConvertColorToString(condition.BackColor),
                    XmlConvert.EncodeName(StiReportObjectStringConverter.ConvertFontToString(condition.Font)),
                    condition.Enabled,
                    condition.CanAssignExpression,
                    XmlConvert.EncodeName(condition.AssignExpression),
                    XmlConvert.EncodeName(condition.Style),
                    XmlConvert.EncodeName(condition.BorderSides.ToString()));
            }
            #endregion

            #region Condition with Permissions, Args(10)
            return string.Format(
                "{0}{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                condition.BreakIfTrue ? "BreakIfTrue," : string.Empty,
                XmlConvert.EncodeName(condition.Expression),
                StiReportObjectStringConverter.ConvertColorToString(condition.TextColor),
                StiReportObjectStringConverter.ConvertColorToString(condition.BackColor),
                XmlConvert.EncodeName(StiReportObjectStringConverter.ConvertFontToString(condition.Font)),
                condition.Enabled,
                condition.CanAssignExpression,
                XmlConvert.EncodeName(condition.AssignExpression),
                XmlConvert.EncodeName(condition.Style),
                XmlConvert.EncodeName(condition.BorderSides.ToString()),
                XmlConvert.EncodeName(condition.Permissions.ToString()));
            #endregion
        }

        private string ConvertMultiConditionToString(object value)
	    {
	        var condition = value as StiMultiCondition;
	        if (condition == null) return null;

            return string.Format("Multi{0}", StiObjectStateSaver.WriteObjectStateToString(new StiMultiConditionContainer
            {
                BackColor = condition.BackColor,
                Enabled = condition.Enabled,
                FilterMode = condition.FilterMode,
                Filters = condition.Filters,
                Font = condition.Font,
                TextColor = condition.TextColor,
                CanAssignExpression = condition.CanAssignExpression,
                AssignExpression = condition.AssignExpression,
                Style = condition.Style,
                BorderSides = condition.BorderSides,
                Permissions = condition.Permissions,
                BreakIfTrue = condition.BreakIfTrue
            }));
	    }

	    private string ConvertDataBarConditionToString(object value)
	    {
	        var condition = value as StiDataBarCondition;
	        if (condition == null) return null;

	        return string.Format(
	            "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
	            "DataBar",
	            XmlConvert.EncodeName(condition.Column),
	            XmlConvert.EncodeName(condition.BrushType.ToString()),
	            StiReportObjectStringConverter.ConvertColorToString(condition.PositiveColor),
	            StiReportObjectStringConverter.ConvertColorToString(condition.NegativeColor),
	            condition.ShowBorder,
	            StiReportObjectStringConverter.ConvertColorToString(condition.PositiveBorderColor),
	            StiReportObjectStringConverter.ConvertColorToString(condition.NegativeBorderColor),
	            XmlConvert.EncodeName(condition.Direction.ToString()),
	            XmlConvert.EncodeName(condition.MinimumType.ToString()),
	            condition.MinimumValue,
	            XmlConvert.EncodeName(condition.MaximumType.ToString()),
	            condition.MaximumValue);
	    }

	    private string ConvertColorScaleToString(object value)
	    {
	        var condition = value as StiColorScaleCondition;
	        if (condition == null) return null;

	        return string.Format(
	            "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
	            "ColorScale",
	            XmlConvert.EncodeName(condition.Column),
	            XmlConvert.EncodeName(condition.ScaleType.ToString()),
	            StiReportObjectStringConverter.ConvertColorToString(condition.MinimumColor),
	            StiReportObjectStringConverter.ConvertColorToString(condition.MidColor),
	            StiReportObjectStringConverter.ConvertColorToString(condition.MaximumColor),
	            XmlConvert.EncodeName(condition.MinimumType.ToString()),
	            condition.MinimumValue,
	            XmlConvert.EncodeName(condition.MidType.ToString()),
	            condition.MidValue,
	            XmlConvert.EncodeName(condition.MaximumType.ToString()),
	            condition.MaximumValue);
	    }

	    private string ConvertIconSetConditionToString(object value)
	    {
	        var condition = value as StiIconSetCondition;
	        if (condition == null) return null;

	        return string.Format(
	            "{0},{1},{2},{3},{4},{5},{6},{7},{8}",
	            "IconSet",
	            XmlConvert.EncodeName(condition.Column),
	            XmlConvert.EncodeName(condition.IconSet.ToString()),
	            XmlConvert.EncodeName(condition.ContentAlignment.ToString()),
	            XmlConvert.EncodeName(ConvertIconSetItemToString(condition.IconSetItem1)),
	            XmlConvert.EncodeName(ConvertIconSetItemToString(condition.IconSetItem2)),
	            XmlConvert.EncodeName(ConvertIconSetItemToString(condition.IconSetItem3)),
	            XmlConvert.EncodeName(ConvertIconSetItemToString(condition.IconSetItem4)),
	            XmlConvert.EncodeName(ConvertIconSetItemToString(condition.IconSetItem5)));
	    }

	    private InstanceDescriptor ConvertMultiConditionToInstanceDescriptor(object value)
        {
            var multiCondition = value as StiMultiCondition;
            if (multiCondition == null) return null;

            var info = typeof(StiMultiCondition).GetConstructor(new[]
            {
                typeof(Color),
                typeof(Color),
                typeof(Font),
                typeof(bool),
                typeof(StiFilterMode),
                typeof(StiFilter[]),
                typeof(string),
                typeof(StiConditionBorderSides),
                typeof(StiConditionPermissions)
            });
            if (info == null) return null;

            return CreateNewInstanceDescriptor(info, new object[]	
            {	
                multiCondition.TextColor,
                multiCondition.BackColor,
                multiCondition.Font,
                multiCondition.Enabled,
                multiCondition.FilterMode,
                multiCondition.Filters.ToList().ToArray(),
                multiCondition.Style,
                multiCondition.BorderSides,
                multiCondition.Permissions
            });
        }

	    private InstanceDescriptor ConvertExpressionConditionToInstanceDescriptor(object value)
	    {
	        var condition = value as StiCondition;
	        if (condition == null || condition.Item != StiFilterItem.Expression) return null;

	        var types = new List<Type>
	        {
	            typeof (string),
	            typeof (Color),
	            typeof (Color),
	            typeof (Font),
	            typeof (bool),
	            typeof (string),
	            typeof (StiConditionBorderSides),
	            typeof (StiConditionPermissions)
	        };
	        if (condition.BreakIfTrue) types.Add(typeof (bool));

	        var info = typeof (StiCondition).GetConstructor(types.ToArray());
	        if (info == null) return null;

	        var args = new List<object>
	        {
	            condition.Expression,
	            condition.TextColor,
	            condition.BackColor,
	            condition.Font,
	            condition.Enabled,
	            condition.Style,
	            condition.BorderSides,
	            condition.Permissions
	        };
            if (condition.BreakIfTrue) args.Add(condition.BreakIfTrue);

            return CreateNewInstanceDescriptor(info, args.ToArray());
	    }

        private InstanceDescriptor ConvertConditionToInstanceDescriptor(object value)
        {
            var condition = value as StiCondition;
            if (condition == null) return null;

            var types = new List<Type>
            {
                typeof (string),
                typeof (StiFilterCondition),
                typeof (string),
                typeof (string),
                typeof (StiFilterDataType),
                typeof (Color),
                typeof (Color),
                typeof (Font),
                typeof (bool),
                typeof (bool),
                typeof (string),
                typeof (string),
                typeof (StiConditionBorderSides),
                typeof (StiConditionPermissions)
            };
            if (condition.BreakIfTrue) types.Add(typeof(bool));

            var info = typeof(StiFilter).GetConstructor(types.ToArray());
            if (info == null) return null;

            var args = new List<object>
            {
                condition.Column,
                condition.Condition,
                condition.Value1,
                condition.Value2,
                condition.DataType,
                condition.TextColor,
                condition.BackColor,
                condition.Font,
                condition.Enabled,
                condition.CanAssignExpression,
                condition.AssignExpression,
                condition.Style,
                condition.BorderSides,
                condition.Permissions
            };
            if (condition.BreakIfTrue) args.Add(condition.BreakIfTrue);
            
            return CreateNewInstanceDescriptor(info, args.ToArray());
        }

	    private InstanceDescriptor ConvertDataBarConditionToInstanceDescriptor(object value)
	    {
	        var dataBarCondition = value as StiDataBarCondition;
            if (dataBarCondition == null) return null;

            var info = typeof (StiDataBarCondition).GetConstructor(new[]
            {
                typeof (string),
                typeof (StiBrushType),
                typeof (Color),
                typeof (Color),
                typeof (bool),
                typeof (Color),
                typeof (Color),
                typeof (StiDataBarDirection),
                typeof (StiMinimumType),
                typeof (float),
                typeof (StiMaximumType),
                typeof (float)
            });
            if (info == null) return null;

	        
	        return CreateNewInstanceDescriptor(info, new object[]
	        {
	            dataBarCondition.Column,
	            dataBarCondition.BrushType,
	            dataBarCondition.PositiveColor,
	            dataBarCondition.NegativeColor,
	            dataBarCondition.ShowBorder,
	            dataBarCondition.PositiveBorderColor,
	            dataBarCondition.NegativeBorderColor,
	            dataBarCondition.Direction,
	            dataBarCondition.MinimumType,
	            dataBarCondition.MinimumValue,
	            dataBarCondition.MaximumType,
	            dataBarCondition.MaximumValue
	        });
	    }

	    private InstanceDescriptor ConvertColorScaleConditionToInstanceDescriptor(object value)
	    {
	        var condition = value as StiColorScaleCondition;
	        if (condition == null) return null;

	        var info = typeof (StiColorScaleCondition).GetConstructor(new[]
	        {
	            typeof (string),
	            typeof (StiColorScaleType),
	            typeof (Color),
	            typeof (Color),
	            typeof (Color),
	            typeof (StiMinimumType),
	            typeof (float),
	            typeof (StiMidType),
	            typeof (float),
	            typeof (StiMaximumType),
	            typeof (float)
	        });
	        if (info == null) return null;

	        
	        return CreateNewInstanceDescriptor(info, new object[]
	        {
	            condition.Column,
	            condition.ScaleType,
	            condition.MinimumColor,
	            condition.MidColor,
	            condition.MaximumColor,
	            condition.MinimumType,
	            condition.MinimumValue,
	            condition.MidType,
	            condition.MidValue,
	            condition.MaximumType,
	            condition.MaximumValue
	        });
	    }

	    private InstanceDescriptor ConvertIconSetConditionToInstanceDescriptor(object value)
	    {
	        var condition = value as StiIconSetCondition;
            if (condition == null) return null;

            var info = typeof (StiIconSetCondition).GetConstructor(new[]
            {
                typeof (string),
                typeof (StiIconSet),
                typeof (ContentAlignment),
                typeof (StiIconSetItem),
                typeof (StiIconSetItem),
                typeof (StiIconSetItem),
                typeof (StiIconSetItem),
                typeof (StiIconSetItem)
            });
            if (info == null) return null;

	        
	        return CreateNewInstanceDescriptor(info, new object[]
	        {
	            condition.Column,
	            condition.IconSet,
	            condition.ContentAlignment,
	            condition.IconSetItem1,
	            condition.IconSetItem2,
	            condition.IconSetItem3,
	            condition.IconSetItem4,
	            condition.IconSetItem5
	        });
	    }

	    private string ConvertIconSetItemToString(StiIconSetItem item)
	    {
	        return item == null ? "null" : new StiIconSetItemConverter().ConvertToInvariantString(item);
	    }

	    private StiIconSetItem ConvertIconSetItemFromString(string str)
	    {
	        return str == "null" ? null : new StiIconSetItemConverter().ConvertFromInvariantString(str) as StiIconSetItem;
	    }


	    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))return true; 
			return base.CanConvertFrom(context, sourceType); 
		} 

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor)) return true;
			if (destinationType == typeof(string)) return true;
			return base.CanConvertTo(context, destinationType); 
		}

        private bool IsBorderSides(string str)
        {
            str = XmlConvert.DecodeName(str);
            if (str == "False" || str == "True")
                return false;
            else 
                return 
                    str.Contains("Left") || 
                    str.Contains("Top") || 
                    str.Contains("Bottom") || 
                    str.Contains("Right") ||
                    str.Contains("All") ||
                    str.Contains("None") ||
                    str.Contains("NotAssigned");
        }

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				var text = value as string;
                var breakIfTrue = false;
                if (text.StartsWith("BreakIfTrue,"))
			    {
			        breakIfTrue = true;
			        text = text.Replace("BreakIfTrue,", string.Empty);
			    }
				var splits = new[]{','};
				var words = text.Split(splits);

                #region StiDataBarCondition
                if (words[0] == "DataBar")
                {
                    return 
                        new StiDataBarCondition(
                            XmlConvert.DecodeName(words[1]),
                            (StiBrushType)Enum.Parse(typeof(StiBrushType), XmlConvert.DecodeName(words[2])),
                            StiReportObjectStringConverter.ConvertStringToColor(words[3]),
                            StiReportObjectStringConverter.ConvertStringToColor(words[4]),
                            words[5].ToLower(CultureInfo.InvariantCulture) == "true",
                            StiReportObjectStringConverter.ConvertStringToColor(words[6]),
                            StiReportObjectStringConverter.ConvertStringToColor(words[7]),
                            (StiDataBarDirection)Enum.Parse(typeof(StiDataBarDirection), XmlConvert.DecodeName(words[8])),
                            (StiMinimumType)Enum.Parse(typeof(StiMinimumType), XmlConvert.DecodeName(words[9])), float.Parse(words[10]),
                            (StiMaximumType)Enum.Parse(typeof(StiMaximumType), XmlConvert.DecodeName(words[11])), float.Parse(words[12]));
                }
                #endregion

                #region StiColorScaleCondition
                if (words[0] == "ColorScale")
                {
                    return
                        new StiColorScaleCondition(
                            XmlConvert.DecodeName(words[1]),
                            (StiColorScaleType)Enum.Parse(typeof(StiColorScaleType), XmlConvert.DecodeName(words[2])),
                            StiReportObjectStringConverter.ConvertStringToColor(words[3]),
                            StiReportObjectStringConverter.ConvertStringToColor(words[4]),
                            StiReportObjectStringConverter.ConvertStringToColor(words[5]),
                            (StiMinimumType)Enum.Parse(typeof(StiMinimumType), XmlConvert.DecodeName(words[6])), float.Parse(words[7]),
                            (StiMidType)Enum.Parse(typeof(StiMidType), XmlConvert.DecodeName(words[8])), float.Parse(words[9]),
                            (StiMaximumType)Enum.Parse(typeof(StiMaximumType), XmlConvert.DecodeName(words[10])), float.Parse(words[11]));
                }
                #endregion

                #region StiIconSetCondition
                if (words[0] == "IconSet")
                {
                    return
                        new StiIconSetCondition(
                            XmlConvert.DecodeName(words[1]),
                            (StiIconSet)Enum.Parse(typeof(StiIconSet), XmlConvert.DecodeName(words[2])),
                            (ContentAlignment)Enum.Parse(typeof(ContentAlignment), XmlConvert.DecodeName(words[3])),
                            ConvertIconSetItemFromString(XmlConvert.DecodeName(words[4])),
                            ConvertIconSetItemFromString(XmlConvert.DecodeName(words[5])),
                            ConvertIconSetItemFromString(XmlConvert.DecodeName(words[6])),
                            ConvertIconSetItemFromString(XmlConvert.DecodeName(words[7])),
                            ConvertIconSetItemFromString(XmlConvert.DecodeName(words[8])));
                }
                #endregion

                #region Multi
			    if (text.StartsWith("Multi<", StringComparison.InvariantCulture))
			    {
			        var container = new StiMultiConditionContainer();
			        var condition = new StiMultiCondition();
			        StiObjectStateSaver.ReadObjectStateFromString(container, text.Substring(5));

			        condition.BackColor = container.BackColor;
			        condition.Enabled = container.Enabled;
			        condition.FilterMode = container.FilterMode;
			        condition.Filters = container.Filters;
			        condition.Font = container.Font;
			        condition.TextColor = container.TextColor;
			        condition.CanAssignExpression = container.CanAssignExpression;
			        condition.AssignExpression = container.AssignExpression;
			        condition.Style = container.Style;
			        condition.Permissions = container.Permissions;
			        condition.BorderSides = container.BorderSides;
                    condition.BreakIfTrue = container.BreakIfTrue;

			        return condition;
			    }
			    #endregion

			    #region Args(5)
			    if (words.Length == 5)
			    {
			        return new StiCondition(
			            XmlConvert.DecodeName(words[0]),
			            StiReportObjectStringConverter.ConvertStringToColor(words[1]),
			            StiReportObjectStringConverter.ConvertStringToColor(words[2]),
			            StiReportObjectStringConverter.ConvertStringToFont(XmlConvert.DecodeName(words[3])),
			            words[4].ToLower(CultureInfo.InvariantCulture) == "true");
			    }
			    #endregion

                #region Args(6)
                if (words.Length == 6)
                {
                    return new StiCondition(
                        XmlConvert.DecodeName(words[0]),
                        StiReportObjectStringConverter.ConvertStringToColor(words[1]),
                        StiReportObjectStringConverter.ConvertStringToColor(words[2]),
                        StiReportObjectStringConverter.ConvertStringToFont(XmlConvert.DecodeName(words[3])),
                        words[4].ToLower(CultureInfo.InvariantCulture) == "true",
                        words[5].ToLower(CultureInfo.InvariantCulture) == "true", 
                        string.Empty);
                }
                #endregion

			    #region Args(7)
			    if (words.Length == 7)
			    {
			        return new StiCondition(
			            XmlConvert.DecodeName(words[0]),
			            StiReportObjectStringConverter.ConvertStringToColor(words[1]),
			            StiReportObjectStringConverter.ConvertStringToColor(words[2]),
			            StiReportObjectStringConverter.ConvertStringToFont(XmlConvert.DecodeName(words[3])),
			            words[4].ToLower(CultureInfo.InvariantCulture) == "true",
			            words[5].ToLower(CultureInfo.InvariantCulture) == "true",
			            XmlConvert.DecodeName(words[6]));
			    }
			    #endregion

			    #region Args(9)
			    if (words.Length == 9 && IsBorderSides(words[8]))
			    {
			        return new StiCondition(
			            XmlConvert.DecodeName(words[0]),
			            StiReportObjectStringConverter.ConvertStringToColor(words[1]),
			            StiReportObjectStringConverter.ConvertStringToColor(words[2]),
			            StiReportObjectStringConverter.ConvertStringToFont(XmlConvert.DecodeName(words[3])),
			            words[4].ToLower(CultureInfo.InvariantCulture) == "true",
			            words[5].ToLower(CultureInfo.InvariantCulture) == "true",
			            XmlConvert.DecodeName(words[6]),
			            XmlConvert.DecodeName(words[7]),
			            (StiConditionBorderSides) Enum.Parse(typeof (StiConditionBorderSides), XmlConvert.DecodeName(words[8])))
			        {
			            BreakIfTrue = breakIfTrue
			        };
			    }
			    #endregion

			    #region Args(10)
			    if (words.Length == 10 && IsBorderSides(words[8]))
			    {
			        return new StiCondition(
			            XmlConvert.DecodeName(words[0]),
			            StiReportObjectStringConverter.ConvertStringToColor(words[1]),
			            StiReportObjectStringConverter.ConvertStringToColor(words[2]),
			            StiReportObjectStringConverter.ConvertStringToFont(XmlConvert.DecodeName(words[3])),
			            words[4].ToLower(CultureInfo.InvariantCulture) == "true",
			            words[5].ToLower(CultureInfo.InvariantCulture) == "true",
			            XmlConvert.DecodeName(words[6]),
			            XmlConvert.DecodeName(words[7]),
			            (StiConditionBorderSides) Enum.Parse(typeof (StiConditionBorderSides), XmlConvert.DecodeName(words[8])),
			            (StiConditionPermissions) Enum.Parse(typeof (StiConditionPermissions), XmlConvert.DecodeName(words[9])))
			        {
			            BreakIfTrue = breakIfTrue
			        };
			    }
			    #endregion

			    var filterCondition = words[1];

			    #region Correct Filter condition spelling
			    if (filterCondition == "GreaterThen")filterCondition = "GreaterThan";
			    if (filterCondition == "GreaterThenOrEqualTo")filterCondition = "GreaterThanOrEqualTo";
			    if (filterCondition == "LessThen")filterCondition = "LessThan";
			    if (filterCondition == "LessThenOrEqualTo")filterCondition = "LessThanOrEqualTo";
			    #endregion

			    #region Args(9)
			    if (words.Length == 9)
			    {
			        return new StiCondition(
			            XmlConvert.DecodeName(words[0]),
			            (StiFilterCondition) Enum.Parse(typeof (StiFilterCondition), filterCondition),
			            XmlConvert.DecodeName(words[2]),
			            XmlConvert.DecodeName(words[3]),
			            (StiFilterDataType) Enum.Parse(typeof (StiFilterDataType), words[4]),
			            StiReportObjectStringConverter.ConvertStringToColor(words[5]),
			            StiReportObjectStringConverter.ConvertStringToColor(words[6]),
			            StiReportObjectStringConverter.ConvertStringToFont(XmlConvert.DecodeName(words[7])),
			            words[8].ToLower(CultureInfo.InvariantCulture) == "true")
			        {
			            BreakIfTrue = breakIfTrue
			        };
			    }
			    #endregion

			    #region Args(11)
			    if (words.Length == 11)
			    {
			        return new StiCondition(
			            XmlConvert.DecodeName(words[0]),
			            (StiFilterCondition) Enum.Parse(typeof (StiFilterCondition), filterCondition),
			            XmlConvert.DecodeName(words[2]),
			            XmlConvert.DecodeName(words[3]),
			            (StiFilterDataType) Enum.Parse(typeof (StiFilterDataType), words[4]),
			            StiReportObjectStringConverter.ConvertStringToColor(words[5]),
			            StiReportObjectStringConverter.ConvertStringToColor(words[6]),
			            StiReportObjectStringConverter.ConvertStringToFont(XmlConvert.DecodeName(words[7])),
			            words[8].ToLower(CultureInfo.InvariantCulture) == "true",
			            words[9].ToLower(CultureInfo.InvariantCulture) == "true",
			            XmlConvert.DecodeName(words[10]))
			        {
			            BreakIfTrue = breakIfTrue
			        };
			    }
			    #endregion

			    #region StiConditionBorderSides, Args(13)
			    if (words.Length == 13)
			    {
			        return new StiCondition(
			            XmlConvert.DecodeName(words[0]),
			            (StiFilterCondition) Enum.Parse(typeof (StiFilterCondition), filterCondition),
			            XmlConvert.DecodeName(words[2]),
			            XmlConvert.DecodeName(words[3]),
			            (StiFilterDataType) Enum.Parse(typeof (StiFilterDataType), words[4]),
			            StiReportObjectStringConverter.ConvertStringToColor(words[5]),
			            StiReportObjectStringConverter.ConvertStringToColor(words[6]),
			            StiReportObjectStringConverter.ConvertStringToFont(XmlConvert.DecodeName(words[7])),
			            words[8].ToLower(CultureInfo.InvariantCulture) == "true",
			            words[9].ToLower(CultureInfo.InvariantCulture) == "true",
			            XmlConvert.DecodeName(words[10]),
			            XmlConvert.DecodeName(words[11]),
			            (StiConditionBorderSides) Enum.Parse(typeof (StiConditionBorderSides), XmlConvert.DecodeName(words[12])))
			        {
			            BreakIfTrue = breakIfTrue
			        };
			    }
			    #endregion

			    #region StiConditionPermissions, Args(14)
			    if (words.Length == 14)
			    {
			        return new StiCondition(
			            XmlConvert.DecodeName(words[0]),
			            (StiFilterCondition) Enum.Parse(typeof (StiFilterCondition), filterCondition),
			            XmlConvert.DecodeName(words[2]),
			            XmlConvert.DecodeName(words[3]),
			            (StiFilterDataType) Enum.Parse(typeof (StiFilterDataType), words[4]),
			            StiReportObjectStringConverter.ConvertStringToColor(words[5]),
			            StiReportObjectStringConverter.ConvertStringToColor(words[6]),
			            StiReportObjectStringConverter.ConvertStringToFont(XmlConvert.DecodeName(words[7])),
			            words[8].ToLower(CultureInfo.InvariantCulture) == "true",
			            words[9].ToLower(CultureInfo.InvariantCulture) == "true",
			            XmlConvert.DecodeName(words[10]),
			            XmlConvert.DecodeName(words[11]),
			            (StiConditionBorderSides) Enum.Parse(typeof (StiConditionBorderSides), XmlConvert.DecodeName(words[12])),
			            (StiConditionPermissions) Enum.Parse(typeof (StiConditionPermissions), XmlConvert.DecodeName(words[13])))
			        {
			            BreakIfTrue = breakIfTrue
			        };
			    }
			    #endregion
			}
			return base.ConvertFrom(context, culture, value); 
		}

        private InstanceDescriptor CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
	}
}
