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
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Events;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base;
using System.Drawing;

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
	public class StiMultiCondition : 
        StiCondition, 
        IStiFilter
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            // StiCondition
            jObject.AddPropertyBool("Enabled", Enabled, true);
            jObject.AddPropertyColor("TextColor", TextColor, Color.Red);
            jObject.AddPropertyColor("BackColor", BackColor, Color.Transparent);
            jObject.AddPropertyFontArial8("Font", Font);
            jObject.AddPropertyBool("CanAssignExpression", CanAssignExpression);
            jObject.AddPropertyStringNullOrEmpty("Style", Style);
            jObject.AddPropertyEnum("BorderSides", BorderSides, StiConditionBorderSides.NotAssigned);
            jObject.AddPropertyEnum("Permissions", Permissions, StiConditionPermissions.All);

            // StiMultiCondition
            jObject.AddPropertyEnum("FilterMode", FilterMode, StiFilterMode.And);
            jObject.AddPropertyJObject("Filters", Filters.SaveToJsonObject(mode));
            
            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "FilterMode":
                        this.FilterMode = property.DeserializeEnum<StiFilterMode>();
                        break;

                    case "Filters":
                        this.Filters.LoadFromJsonObject((JObject)property.Value);
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

		#region IStiFilter
	    /// <summary>
		/// Gets or sets filter mode.
		/// </summary>
		[DefaultValue(StiFilterMode.And)]
		[StiSerializable]
		[Browsable(false)]
		public StiFilterMode FilterMode { get; set; } = StiFilterMode.And;

	    [StiSerializable(StiSerializationVisibility.List)]
		public StiFiltersCollection Filters { get; set; } = new StiFiltersCollection();

	    [Browsable(false)]
		[StiNonSerialized]
		public StiFilterEventHandler FilterMethodHandler
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[StiNonSerialized]
		[Browsable(false)]
		public bool FilterOn
		{
			get
			{
				return true;
			}
			set
			{
			}
		}
		#endregion

		#region Properties
		[StiNonSerialized]
		public override StiFilterCondition Condition
		{
			get
			{
				throw new Exception("Please, use Filters collection!");
			}
			set
			{
				throw new Exception("Please, use Filters collection!");
			}
		}

		[StiNonSerialized]
		public override StiFilterDataType DataType
		{
			get
			{
				throw new Exception("Please, use Filters collection!");
			}
			set
			{
				throw new Exception("Please, use Filters collection!");
			}
		}

		[StiNonSerialized]
		public override string Column
		{
			get
			{
				throw new Exception("Please, use Filters collection!");
			}
			set
			{
				throw new Exception("Please, use Filters collection!");
			}
		}

		[StiNonSerialized]
		public override StiFilterItem Item
		{
			get
			{
				throw new Exception("Please, use Filters collection!");
			}
			set
			{
				throw new Exception("Please, use Filters collection!");
			}
		}

		[StiNonSerialized]
		public override string Value1
		{
			get
			{
				throw new Exception("Please, use Filters collection!");
			}
			set
			{
				throw new Exception("Please, use Filters collection!");
			}
		}

		[StiNonSerialized]
		public override string Value2
		{
			get
			{
				throw new Exception("Please, use Filters collection!");
			}
			set
			{
				throw new Exception("Please, use Filters collection!");
			}
		}

		[StiNonSerialized]
		public override StiExpression Expression
		{
			get
			{
				throw new Exception("Please, use Filters collection!");
			}
			set
			{
				throw new Exception("Please, use Filters collection!");
			}
		}
		#endregion

		#region Methods
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
        
		public override bool Equals(object obj)
		{
			var condition = obj as StiMultiCondition;

			if (condition == null)return false;

			return 
				this.BackColor.Equals(condition.BackColor) &&
				this.TextColor.Equals(condition.TextColor) &&
				this.Enabled.Equals(condition.Enabled) &&
				this.Font.Equals(condition.Font) &&
				this.FilterMode.Equals(condition.FilterMode) &&
				this.Filters.Equals(condition.Filters) &&
				this.CanAssignExpression.Equals(condition.CanAssignExpression) &&
				this.AssignExpression.Equals(condition.AssignExpression);
		}
		#endregion

		/// <summary>
		/// Creates a new object of the type StiMultiCondition.
		/// </summary>
		public StiMultiCondition()
		{
		}

		/// <summary>
		/// Creates a new object of the type StiMultiCondition.
		/// </summary>
		public StiMultiCondition(Color textColor, Color backColor, Font font, bool enabled, StiFilterMode filterMode) :
            this(textColor, backColor, font, enabled, filterMode, false, string.Empty)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiMultiCondition.
		/// </summary>
		public StiMultiCondition(Color textColor, Color backColor, Font font, bool enabled, StiFilterMode filterMode,
			bool canAssignExpression, string assignExpression) : 
			this(textColor, backColor, font, enabled, filterMode, null, canAssignExpression, assignExpression)
		{
		}

        /// <summary>
        /// Creates a new object of the type StiMultiCondition.
        /// </summary>
        public StiMultiCondition(Color textColor, Color backColor, Font font, bool enabled, StiFilterMode filterMode, StiFilter[] filters) :
            this(textColor, backColor, font, enabled, filterMode, filters, false, string.Empty)
        {
        }

		/// <summary>
		/// Creates a new object of the type StiMultiCondition.
		/// </summary>
		public StiMultiCondition(Color textColor, Color backColor, Font font, bool enabled, StiFilterMode filterMode, StiFilter[] filters, 
            string style, StiConditionBorderSides borderSides) :
            this(textColor, backColor, font, enabled, filterMode, filters, false, string.Empty)
		{
            this.Style = style;
            this.BorderSides = borderSides;
		}

        /// <summary>
		/// Creates a new object of the type StiMultiCondition.
		/// </summary>
        public StiMultiCondition(Color textColor, Color backColor, Font font, bool enabled,
            StiFilterMode filterMode, StiFilter[] filters, bool canAssignExpression, string assignExpression) :
            this(textColor, backColor, font, enabled, filterMode, filters, canAssignExpression, assignExpression, 
            string.Empty, StiConditionBorderSides.NotAssigned)
        {
        }

		/// <summary>
		/// Creates a new object of the type StiMultiCondition.
		/// </summary>
		public StiMultiCondition(Color textColor, Color backColor, Font font, bool enabled, 
			StiFilterMode filterMode, StiFilter[] filters, bool canAssignExpression, string assignExpression, string style, StiConditionBorderSides borderSides) : 
			base(string.Empty, textColor, backColor, font, enabled, canAssignExpression, assignExpression)
		{
            this.Style = style;
            this.BorderSides = borderSides;
			this.FilterMode = filterMode;

			if (filters != null)
			{
				foreach (StiFilter filter in filters)
				{
					this.Filters.Add(filter);
				}
			}
		}
	}
}
