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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// The class describes the condition.
    /// </summary>
    [RefreshProperties(RefreshProperties.All)]
	[TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiConditionConverter))]
    public class StiCondition : StiBaseCondition,
        IStiGetFonts,
        IStiFont
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiCondition
            jObject.AddPropertyBool("Enabled", Enabled, true);
            jObject.AddPropertyColor("TextColor", TextColor, Color.Red);
            jObject.AddPropertyColor("BackColor", BackColor, Color.Transparent);
            jObject.AddPropertyFontArial8("Font", Font);
            jObject.AddPropertyBool("CanAssignExpression", CanAssignExpression);
            jObject.AddPropertyStringNullOrEmpty("AssignExpression", AssignExpression);
            jObject.AddPropertyStringNullOrEmpty("Style", Style);
            jObject.AddPropertyEnum("BorderSides", BorderSides, StiConditionBorderSides.NotAssigned);
            jObject.AddPropertyEnum("Permissions", Permissions, StiConditionPermissions.All);
            jObject.AddPropertyBool("BreakIfTrue", BreakIfTrue);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Enabled":
                        this.Enabled = property.DeserializeBool();
                        break;

                    case "TextColor":
                        this.TextColor = property.DeserializeColor();
                        break;

                    case "BackColor":
                        this.BackColor = property.DeserializeColor();
                        break;

                    case "Font":
                        this.Font = property.DeserializeFont(this.Font);
                        break;

                    case "CanAssignExpression":
                        this.CanAssignExpression = property.DeserializeBool();
                        break;

                    case "AssignExpression":
                        this.AssignExpression = property.DeserializeString();
                        break;

                    case "Style":
                        this.Style = property.DeserializeString();
                        break;

                    case "BorderSides":
                        this.borderSides = property.DeserializeEnum<StiConditionBorderSides>();
                        break;

                    case "Permissions":
                        this.Permissions = property.DeserializeEnum<StiConditionPermissions>();
                        break;

                    case "BreakIfTrue":
                        this.BreakIfTrue = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

		#region ICloneable
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return (StiCondition)base.Clone();
		}
        #endregion

        #region IStiGetFonts
        public List<StiFont> GetFonts()
        {
            return new List<StiFont>
            {
                new StiFont(Font)
            };
        }
        #endregion

        #region Properties
        [StiCategory("Behavior")]
		[StiOrder(StiPropertyOrder.BehaviorEnabled)]
		[StiSerializable]
		[DefaultValue(true)]
		public bool Enabled { get; set; } = true;

	    /// <summary>
		/// Gets or sets a color to draw text.
		/// </summary>
		[StiSerializable]
		[Description("Gets or sets a color to draw text.")]
		public Color TextColor { get; set; } = Color.Red;

	    /// <summary>
		/// Gets or sets a color to draw background of text.
		/// </summary>
		[StiSerializable]
		[Description("Gets or sets a color to draw background of text.")]
		public Color BackColor { get; set; } = Color.Transparent;

	    /// <summary>
		/// Gets or sets font of text.
		/// </summary>
		[StiSerializable]
		[Description("Gets or sets font of text.")]
        public Font Font { get; set; } = new Font("Arial", 8);

	    [StiSerializable]
		[DefaultValue(false)]
		public bool CanAssignExpression { get; set; }

	    [StiSerializable]
        [DefaultValue(false)]
        public bool BreakIfTrue { get; set; }

	    [StiSerializable]
		[DefaultValue("")]
		public string AssignExpression { get; set; } = string.Empty;

	    [StiSerializable]
        [DefaultValue("")]
        public string Style { get; set; } = string.Empty;

	    private StiConditionBorderSides borderSides = StiConditionBorderSides.NotAssigned;
        [StiSerializable]
        [DefaultValue(StiConditionBorderSides.NotAssigned)]
        public StiConditionBorderSides BorderSides
        {
            get
            {
                return borderSides;
            }
            set
            {
                borderSides = value;

                if (value != StiConditionBorderSides.NotAssigned) return;

                borderSides = StiConditionBorderSides.None;
                if ((Permissions & StiConditionPermissions.Borders) > 0)
                    Permissions -= StiConditionPermissions.Borders;
            }
        }

	    [StiSerializable]
        [DefaultValue(StiConditionPermissions.All)]
        public StiConditionPermissions Permissions { get; set; } = StiConditionPermissions.All;

        public byte[] Icon 
        { 
            get; 
            set; 
        }

        public ContentAlignment IconAlignment = ContentAlignment.MiddleRight;

        public Size? IconSize { get; set; }
        #endregion

        #region Methods
        public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var condition = obj as StiCondition;
			if (condition == null)return false;

			return 
				this.BackColor.Equals(condition.BackColor) &&
				this.TextColor.Equals(condition.TextColor) &&
				this.Column.Equals(condition.Column) &&
				this.Condition.Equals(condition.Condition) &&
				this.DataType.Equals(condition.DataType) &&
				this.Enabled.Equals(condition.Enabled) &&
				this.Font.Equals(condition.Font) &&
				this.Item.Equals(condition.Item) &&
				this.Value1.Equals(condition.Value1) &&
				this.Value2.Equals(condition.Value2) &&
				this.Expression.Value.Trim().Equals(condition.Expression.Value.Trim()) &&
				this.CanAssignExpression.Equals(condition.CanAssignExpression) &&
				this.AssignExpression.Equals(condition.AssignExpression);
		}
		#endregion

        /// <summary>
		/// Creates a new object of the type StiCondition.
		/// </summary>
		public StiCondition()
		{
		}

		/// <summary>
		/// Creates a new object of the type StiCondition.
		/// </summary>
		public StiCondition(string expression, Color textColor, Color backColor, Font font, bool enabled) : base(expression)
		{
			this.TextColor = textColor;
			this.BackColor = backColor;
			this.Font = font;
			this.Enabled = enabled;
		}

        /// <summary>
		/// Creates a new object of the type StiCondition.
		/// </summary>
		public StiCondition(string expression, Color textColor, Color backColor, Font font, bool enabled, 
			bool canAssignExpression, string assignExpression) :
            this(expression, textColor, backColor, font, enabled, canAssignExpression, assignExpression, string.Empty, StiConditionBorderSides.NotAssigned)
        {
            //fix for back compatibility
            if ((this.Permissions & StiConditionPermissions.Borders) > 0)
                this.Permissions -= StiConditionPermissions.Borders;
        }

		/// <summary>
		/// Creates a new object of the type StiCondition.
		/// </summary>
		public StiCondition(string expression, Color textColor, Color backColor, Font font, bool enabled,
            bool canAssignExpression, string assignExpression, string style, StiConditionBorderSides borderSides)
            : 
			this(expression, textColor, backColor, font, enabled)
		{
			this.CanAssignExpression = canAssignExpression;
			this.AssignExpression = assignExpression;
            this.Style = style;
            this.borderSides = borderSides;
	    }

        /// <summary>
        /// Creates a new object of the type StiCondition.
        /// </summary>
        public StiCondition(string expression, Color textColor, Color backColor, Font font, bool enabled,
            bool canAssignExpression, string assignExpression, string style, StiConditionBorderSides borderSides, StiConditionPermissions permissions)
            :
            this(expression, textColor, backColor, font, enabled)
        {
            this.CanAssignExpression = canAssignExpression;
            this.AssignExpression = assignExpression;
            this.Style = style;
            this.borderSides = borderSides;
            this.Permissions = permissions;
        }

        /// <summary>
        /// Creates a new object of the type StiCondition.
        /// </summary>
        public StiCondition(string expression, Color textColor, Color backColor, Font font, bool enabled,
            bool canAssignExpression, string assignExpression, string style, StiConditionBorderSides borderSides, StiConditionPermissions permissions, bool breakIfTrue)
            :
            this(expression, textColor, backColor, font, enabled)
        {
            this.CanAssignExpression = canAssignExpression;
            this.AssignExpression = assignExpression;
            this.Style = style;
            this.borderSides = borderSides;
            this.Permissions = permissions;
            this.BreakIfTrue = breakIfTrue;
        }

		/// <summary>
		/// Creates a new object of the type StiCondition.
		/// </summary>
		public StiCondition(string column, StiFilterCondition condition, DateTime date1,
			Color textColor, Color backColor, Font font, bool enabled) :
			base(column, condition, date1)
		{			
			this.TextColor = textColor;
			this.BackColor = backColor;
			this.Font = font;
			this.Enabled = enabled;
		}

		/// <summary>
		/// Creates a new object of the type StiCondition.
		/// </summary>
		public StiCondition(string column, StiFilterCondition condition, DateTime date1, DateTime date2,
			Color textColor, Color backColor, Font font, bool enabled) :
			base(column, condition, date1, date2)
		{
			this.TextColor = textColor;
			this.BackColor = backColor;
			this.Font = font;
			this.Enabled = enabled;
		}

		/// <summary>
		/// Creates a new object of the type StiCondition.
		/// </summary>
		public StiCondition(string column, StiFilterCondition condition, string value, StiFilterDataType dataType,
			Color textColor, Color backColor, Font font, bool enabled) :
			base(column, condition, value, string.Empty, dataType)
		{
			this.TextColor = textColor;
			this.BackColor = backColor;
			this.Font = font;
			this.Enabled = enabled;
		}

		/// <summary>
		/// Creates a new object of the type StiCondition.
		/// </summary>
		public StiCondition(string column, StiFilterCondition condition, 
			string value1, string value2, StiFilterDataType dataType,
			Color textColor, Color backColor, Font font, bool enabled) :
			this(column, condition, value1, value2, dataType, textColor, backColor, font, enabled, false, string.Empty)
		{
		}

        /// <summary>
		/// Creates a new object of the type StiCondition.
		/// </summary>
        public StiCondition(string column, StiFilterCondition condition,
            string value1, string value2, StiFilterDataType dataType,
            Color textColor, Color backColor, Font font, bool enabled, bool canAssignExpression, string assignExpression) :
            this(column, condition, value1, value2, dataType, textColor, backColor, font, enabled, canAssignExpression, assignExpression,
            string.Empty, StiConditionBorderSides.NotAssigned)
        {
        }

		/// <summary>
		/// Creates a new object of the type StiCondition.
		/// </summary>
		public StiCondition(string column, StiFilterCondition condition, 
			string value1, string value2, StiFilterDataType dataType,
            Color textColor, Color backColor, Font font, bool enabled, bool canAssignExpression, string assignExpression,
            string style, StiConditionBorderSides borderSides)
            :
			base(column, condition, value1, value2, dataType)
		{
			this.TextColor = textColor;
			this.BackColor = backColor;
			this.Font = font;
			this.Enabled = enabled;
			this.CanAssignExpression = canAssignExpression;
			this.AssignExpression = assignExpression;
            this.Style = style;
            this.borderSides = borderSides;
		}

        /// <summary>
        /// Creates a new object of the type StiCondition.
        /// </summary>
        public StiCondition(string column, StiFilterCondition condition,
            string value1, string value2, StiFilterDataType dataType,
            Color textColor, Color backColor, Font font, bool enabled, bool canAssignExpression, string assignExpression,
            string style, StiConditionBorderSides borderSides, StiConditionPermissions permissions)
            :
            base(column, condition, value1, value2, dataType)
        {
            this.TextColor = textColor;
            this.BackColor = backColor;
            this.Font = font;
            this.Enabled = enabled;
            this.CanAssignExpression = canAssignExpression;
            this.AssignExpression = assignExpression;
            this.Style = style;
            this.borderSides = borderSides;
            this.Permissions = permissions;
        }

        /// <summary>
        /// Creates a new object of the type StiCondition.
        /// </summary>
        public StiCondition(string column, StiFilterCondition condition,
            string value1, string value2, StiFilterDataType dataType,
            Color textColor, Color backColor, Font font, bool enabled, bool canAssignExpression, string assignExpression,
            string style, StiConditionBorderSides borderSides, StiConditionPermissions permissions, bool breakIfTrue)
            :
            base(column, condition, value1, value2, dataType)
        {
            this.TextColor = textColor;
            this.BackColor = backColor;
            this.Font = font;
            this.Enabled = enabled;
            this.CanAssignExpression = canAssignExpression;
            this.AssignExpression = assignExpression;
            this.Style = style;
            this.borderSides = borderSides;
            this.Permissions = permissions;
            this.BreakIfTrue = breakIfTrue;
        }

		/// <summary>
		/// Creates a new object of the type StiCondition.
		/// </summary>
		public StiCondition(StiFilterItem item, string column, StiFilterCondition condition, 
			string value1, string value2, StiFilterDataType dataType, string expression,
			Color textColor, Color backColor, Font font, bool enabled) : 
			this (item, column, condition, value1, value2, dataType, expression, 
			textColor, backColor, font, enabled, false, string.Empty)
		{
		}

        /// <summary>
		/// Creates a new object of the type StiCondition.
		/// </summary>
        public StiCondition(StiFilterItem item, string column, StiFilterCondition condition,
            string value1, string value2, StiFilterDataType dataType, string expression,
            Color textColor, Color backColor, Font font, bool enabled, bool canAssignExpression, string assignExpression) : 
            this(item, column, condition, 
            value1, value2, dataType, expression, textColor, backColor, font, enabled, canAssignExpression, assignExpression,
            string.Empty, StiConditionBorderSides.NotAssigned)
        {
        }

        /// <summary>
		/// Creates a new object of the type StiCondition.
		/// </summary>
        public StiCondition(StiFilterItem item, string column, StiFilterCondition condition,
            string value1, string value2, StiFilterDataType dataType, string expression,
            Color textColor, Color backColor, Font font, bool enabled, bool canAssignExpression, string assignExpression,
            string style, StiConditionBorderSides borderSides)
            :
            this(item, column, condition, value1, value2, dataType, expression, textColor, backColor, font, enabled, 
            canAssignExpression, assignExpression, style, borderSides, StiConditionPermissions.All)
        {
        }
		
		/// <summary>
		/// Creates a new object of the type StiCondition.
		/// </summary>
		public StiCondition(StiFilterItem item, string column, StiFilterCondition condition, 
			string value1, string value2, StiFilterDataType dataType, string expression,
			Color textColor, Color backColor, Font font, bool enabled, bool canAssignExpression, string assignExpression,
            string style, StiConditionBorderSides borderSides, StiConditionPermissions permissions)
            : 
			base (item, column, condition, value1, value2, dataType, expression)
		{
			this.TextColor = textColor;
			this.BackColor = backColor;
			this.Font = font;
			this.Enabled = enabled;
			this.CanAssignExpression = canAssignExpression;
			this.AssignExpression = assignExpression;
            this.Style = style;
            this.borderSides = borderSides;
            this.Permissions = permissions;
        }

        /// <summary>
        /// Creates a new object of the type StiCondition.
        /// </summary>
        public StiCondition(StiFilterItem item, string column, StiFilterCondition condition,
            string value1, string value2, StiFilterDataType dataType, string expression,
            Color textColor, Color backColor, Font font, bool enabled, bool canAssignExpression, string assignExpression,
            string style, StiConditionBorderSides borderSides, StiConditionPermissions permissions, bool breakIfTrue)
            :
            base(item, column, condition, value1, value2, dataType, expression)
        {
            this.TextColor = textColor;
            this.BackColor = backColor;
            this.Font = font;
            this.Enabled = enabled;
            this.CanAssignExpression = canAssignExpression;
            this.AssignExpression = assignExpression;
            this.Style = style;
            this.borderSides = borderSides;
            this.Permissions = permissions;
            this.BreakIfTrue = breakIfTrue;
        }
    }
}
