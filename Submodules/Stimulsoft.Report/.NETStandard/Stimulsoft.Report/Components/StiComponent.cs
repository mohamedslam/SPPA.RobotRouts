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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Units;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Helpers;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
#else
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes the base class for all components.
    /// </summary>
    [StiServiceBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.StiComponent.png")]
    [StiServiceCategoryBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.catComponents.png")]
	[StiV1Builder(typeof(StiComponentV1Builder))]
	[StiV2Builder(typeof(StiComponentV2Builder))]
    [StiGdiPainter(typeof(StiComponentGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiComponentWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	[StiToolbox(false)]
	[StiDesigner(typeof(StiComponentDesigner))]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.Designer.StiWpfComponentDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
	public abstract class StiComponent :		
		StiBase,
        IStiReportComponent,
        IStiComponentGuid,
		IStiCanGrow, 
		IStiCanShrink, 
		IStiUnitConvert,
		IStiShift,
		IStiGrowToHeight,
        IStiAnchor,
		IStiConditions,
		IStiPrintOn,
		IStiInherited,
		IStiReportProperty,
        IStiInteraction,
        IStiStateSaveRestore,
        IStiSelect,
        IStiPropertyGridObject,
        IStiJsonReportObject,
		IStiGetFonts,
		IStiAppExpressionCollection
	{
        #region class bits
        private class bitsComponent : ICloneable
        {
            #region ICloneable
            public object Clone()
            {
                return this.MemberwiseClone();
            }
            #endregion

            #region Fields
            public object bookmarkValue;
            public object toolTipValue;
            public object hyperlinkValue;
            public object tagValue;

            public bool enabled;
            public StiHighlightState highlightState;
            public bool ignoreNamingRule;

            public StiDockStyle dockStyle;
            public bool printable;
            #endregion

            public bitsComponent(object bookmarkValue, object toolTipValue, object hyperlinkValue, object tagValue,
                bool enabled, StiHighlightState highlightState, bool ignoreNamingRule,
                StiDockStyle dockStyle, bool printable)
            {
                this.bookmarkValue = bookmarkValue;
                this.toolTipValue = toolTipValue;
                this.hyperlinkValue = hyperlinkValue;
                this.tagValue = tagValue;
                this.enabled = enabled;
                this.highlightState = highlightState;
                this.ignoreNamingRule = ignoreNamingRule;
                this.dockStyle = dockStyle;
                this.printable = printable;
            }
        }
        #endregion

        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            // StiComponent
            jObject.AddPropertyStringNullOrEmpty("Name", Name);
            jObject.AddPropertyEnum("ShiftMode", ShiftMode, StiShiftMode.IncreasingSize);
            jObject.AddPropertyStringNullOrEmpty("Guid", Guid);
            jObject.AddPropertyEnum("PrintOn", PrintOn, StiPrintOnType.AllPages);
            jObject.AddPropertyBool("CanShrink", CanShrink);
            jObject.AddPropertyBool("CanGrow", CanGrow);
            jObject.AddPropertyBool("GrowToHeight", GrowToHeight);
            jObject.AddPropertyEnum("Anchor", Anchor, StiAnchorMode.Left | StiAnchorMode.Top);
            jObject.AddPropertyBool("Inherited", Inherited);

            jObject.AddPropertyBool("Printable", Printable, true);
            jObject.AddPropertyEnum("DockStyle", DockStyle, StiDockStyle.None);
			jObject.AddPropertySizeD("MinSize", MinSize, "0,0");
			jObject.AddPropertySizeD("MaxSize", MaxSize, "0,0");
			jObject.AddPropertyRectangleD("ClientRectangle", ClientRectangle);
            jObject.AddPropertyJObject("GetToolTipEvent", GetToolTipEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetHyperlinkEvent", GetHyperlinkEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetTagEvent", GetTagEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetBookmarkEvent", GetBookmarkEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("BeforePrintEvent", BeforePrintEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("AfterPrintEvent", AfterPrintEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetDrillDownReportEvent", GetDrillDownReportEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ClickEvent", ClickEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("DoubleClickEvent", DoubleClickEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("MouseEnterEvent", MouseEnterEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("MouseLeaveEvent", MouseLeaveEvent.SaveToJsonObject(mode));
            
            jObject.AddPropertyStringNullOrEmpty("Alias", Alias);
            jObject.AddPropertyEnum("Restrictions", Restrictions, StiRestrictions.All);
            jObject.AddPropertyStringNullOrEmpty("ComponentPlacement", ComponentPlacement);
            jObject.AddPropertyStringNullOrEmpty("ComponentStyle", ComponentStyle);
            jObject.AddPropertyBool("Locked", Locked);
            jObject.AddPropertyBool("Linked", Linked);
            jObject.AddPropertyBool("Enabled", Enabled, true);
            jObject.AddPropertyBool("UseParentStyles", UseParentStyles);

            if (mode == StiJsonSaveMode.Report)
            {
                jObject.AddPropertyJObject("Conditions", Conditions.SaveToJsonObject(mode));

				if (Expressions != null)
					jObject.AddPropertyJObject("Expressions", Expressions.SaveToJsonObject(mode));

				if (Interaction != null)
                    jObject.AddPropertyJObject("Interaction", Interaction.SaveToJsonObject(mode));

                jObject.AddPropertyJObject("Bookmark", Bookmark.SaveToJsonObject(mode));
                jObject.AddPropertyJObject("ToolTip", ToolTip.SaveToJsonObject(mode));
                jObject.AddPropertyJObject("Hyperlink", Hyperlink.SaveToJsonObject(mode));
                jObject.AddPropertyJObject("Tag", Tag.SaveToJsonObject(mode));
                jObject.AddPropertyJObject("Pointer", Pointer.SaveToJsonObject(mode));
            }
			if (mode == StiJsonSaveMode.Document)
			{
				jObject.AddPropertyStringNullOrEmpty("BookmarkValue", BookmarkValue?.ToString());
				jObject.AddPropertyStringNullOrEmpty("ToolTipValue", ToolTipValue?.ToString());
				jObject.AddPropertyStringNullOrEmpty("HyperlinkValue", HyperlinkValue?.ToString());
				jObject.AddPropertyStringNullOrEmpty("TagValue", TagValue?.ToString());
				jObject.AddPropertyStringNullOrEmpty("PointerValue", PointerValue?.ToString());
			}

			return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Name":
                        base.Name = property.DeserializeString();
                        break;

                    case "ShiftMode":
                        this.shiftMode = property.DeserializeEnum<StiShiftMode>();
                        break;

                    case "Guid":
                        this.Guid = property.DeserializeString();
                        break;

                    case "PrintOn":
                        this.PrintOn = property.DeserializeEnum<StiPrintOnType>();
                        break;

                    case "CanShrink":
                        this.CanShrink = property.DeserializeBool();
                        break;

                    case "CanGrow":
                        this.canGrow = property.DeserializeBool();
                        break;

                    case "GrowToHeight":
                        this.growToHeight = property.DeserializeBool();
                        break;

                    case "Anchor":
                        this.anchor = property.DeserializeEnum<StiAnchorMode>();
                        break;

                    case "Conditions":
                        this.Conditions.LoadFromJsonObject((JObject)property.Value);
                        break;

					case "Expressions":
						this.Expressions.LoadFromJsonObject((JObject)property.Value);
						break;

					case "Inherited":
                        this.Inherited = property.DeserializeBool();
                        break;

                    case "Interaction":
                        this.Interaction = StiInteraction.LoadInteractionFromJsonObject((JObject)property.Value);
                        break;

                    case "Printable":
                        this.Printable = property.DeserializeBool();
                        break;

                    case "DockStyle":
                        this.DockStyle = property.DeserializeEnum<StiDockStyle>();
                        break;

                    case "MinSize":
                        {
                            this.MinSize = property.DeserializeSizeD();
                        }
                        break;

                    case "MaxSize":
                        {
                            this.MaxSize = property.DeserializeSizeD();
                        }
                        break;

                    case "ClientRectangle":
                        this.ClientRectangle = property.DeserializeRectangleD();
                        break;

                    case "GetToolTipEvent":
                        {
                            var _event = new StiGetToolTipEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.GetToolTipEvent = _event;
                        }
                        break;

                    case "GetHyperlinkEvent":
                        {
                            var _event = new StiGetHyperlinkEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.GetHyperlinkEvent = _event;
                        }
                        break;

                    case "GetTagEvent":
                        {
                            var _event = new StiGetTagEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.GetTagEvent = _event;
                        }
                        break;

					case "GetPointerEvent":
						{
							var _event = new StiGetPointerEvent();
							_event.LoadFromJsonObject((JObject)property.Value);
							this.GetPointerEvent = _event;
						}
						break;

					case "GetBookmarkEvent":
                        {
                            var _event = new StiGetBookmarkEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.GetBookmarkEvent = _event;
                        }
                        break;

                    case "BeforePrintEvent":
                        {
                            var _event = new StiBeforePrintEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.BeforePrintEvent = _event;
                        }
                        break;

                    case "AfterPrintEvent":
                        {
                            var _event = new StiAfterPrintEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.AfterPrintEvent = _event;
                        }
                        break;

                    case "GetDrillDownReportEvent":
                        {
                            var _event = new StiGetDrillDownReportEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.GetDrillDownReportEvent = _event;
                        }
                        break;

                    case "ClickEvent":
                        {
                            var _event = new StiClickEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.ClickEvent = _event;
                        }
                        break;

                    case "DoubleClickEvent":
                        {
                            var _event = new StiDoubleClickEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.DoubleClickEvent = _event;
                        }
                        break;

                    case "MouseEnterEvent":
                        {
                            var _event = new StiMouseEnterEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.MouseEnterEvent = _event;
                        }
                        break;

                    case "MouseLeaveEvent":
                        {
                            var _event = new StiMouseLeaveEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.MouseLeaveEvent = _event;
                        }
                        break;

                    case "Bookmark":
                        {
                            var expression = new StiBookmarkExpression();
                            expression.LoadFromJsonObject((JObject)property.Value);
                            this.Bookmark = expression;
                        }
                        break;

                    case "ToolTip":
                        {
                            var expression = new StiToolTipExpression();
                            expression.LoadFromJsonObject((JObject)property.Value);
                            this.ToolTip = expression;
                        }
                        break;

                    case "Hyperlink":
                        {
                            var expression = new StiHyperlinkExpression();
                            expression.LoadFromJsonObject((JObject)property.Value);
                            this.Hyperlink = expression;
                        }
                        break;

                    case "Tag":
                        {
                            var expression = new StiTagExpression();
                            expression.LoadFromJsonObject((JObject)property.Value);
                            this.Tag = expression;
                        }
                        break;

					case "Pointer":
						{
							var expression = new StiPointerExpression();
							expression.LoadFromJsonObject((JObject)property.Value);
							this.Pointer = expression;
						}
						break;

					case "BookmarkValue":
						{
							this.BookmarkValue = property.DeserializeString();
						}
						break;

					case "ToolTipValue":
						{
							this.ToolTipValue = property.DeserializeString();
						}
						break;

					case "HyperlinkValue":
						{
							this.HyperlinkValue = property.DeserializeString();
						}
						break;

					case "TagValue":
						{
							this.TagValue = property.DeserializeString();
						}
						break;

					case "PointerValue":
						{
							this.PointerValue = property.DeserializeString();
						}
						break;

					case "Alias":
                        this.alias = property.DeserializeString();
                        break;

                    case "Restrictions":
                        this.Restrictions = property.DeserializeEnum<StiRestrictions>();
                        break;

                    case "ComponentPlacement":
                        this.ComponentPlacement = property.DeserializeString();
                        break;

                    case "ComponentStyle":
                        this.ComponentStyle = property.DeserializeString();
                        break;

                    case "Locked":
                        this.Locked = property.DeserializeBool();
                        break;

                    case "Linked":
                        this.Linked = property.DeserializeBool();
                        break;

                    case "Enabled":
                        this.Enabled = property.DeserializeBool();
                        break;

                    case "UseParentStyles":
                        this.UseParentStyles = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject

        [Browsable(false)]
        public virtual StiComponentId ComponentId => StiComponentId.StiComponent;

        [Browsable(false)]
        public string PropName => base.Name;

        public virtual StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
	    {
	        return null;
	    }

	    public virtual StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
	    {
	        return null;
	    }
        #endregion

        #region IStiSelect override
        protected static object PropertySelectionTick = new object();
        [Browsable(false)]
        public virtual int SelectionTick
        {
            get
            {
                return Properties.GetInt(PropertySelectionTick, 0);
            }
            set
            {
                Properties.SetInt(PropertySelectionTick, value, 0);
            }
        }

        private bool isSelected;
        /// <summary>
        /// Gets or sets value indicates is the component selected or not.
        /// </summary>
        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDesigner)]
        [DefaultValue(false)]
        public virtual bool IsSelected
        {
            get
			{
				return isSelected;
			}
			set
			{
                if (!IsSelected)
                    SelectionTick = Environment.TickCount;

				isSelected = value; 
			}
        }

        /// <summary>
        /// Select component.
        /// </summary>
        public void Select()
        {
            if (!IsSelected)
                SelectionTick = Environment.TickCount;

            if (StiRestrictionsHelper.IsAllowSelect(this))
                IsSelected = true;
        }

        /// <summary>
        /// Invert selection of component.
        /// </summary>
        public void Invert()
        {
            if (!IsSelected)
                SelectionTick = Environment.TickCount;

            if (StiRestrictionsHelper.IsAllowSelect(this))
                IsSelected = !IsSelected;
        }

        /// <summary>
        /// Reset selection of component.
        /// </summary>
        public void Reset()
        {
            IsSelected = false;
        }
        #endregion

        #region IStiAppComponent
        IStiApp IStiAppComponent.GetApp()
        {
            return Report;
        }

        string IStiAppComponent.GetName()
        {
            return Name;
        }
        #endregion

        #region IStiAppCell
        string IStiAppCell.GetKey()
        {
            if (string.IsNullOrWhiteSpace(Guid))
                NewGuid();

            return Guid;
        }

        void IStiAppCell.SetKey(string key)
        {
            Guid = key;
        }
        #endregion

        #region IStiReportComponent
        IStiReport IStiReportComponent.GetReport()
        {
            return Report;
        }
        #endregion

        #region IStiStateSaveRestore
        private StiStatesManager states;
        /// <summary>
        /// Gets the component states manager.
        /// </summary>
        protected StiStatesManager States
        {
            get
            {
                return states ?? (states = new StiStatesManager());
            }
        }

        /// <summary>
        /// Saves the current state of an object.
        /// </summary>
        /// <param name="stateName">A name of the state being saved.</param>
        public virtual void SaveState(string stateName)
        {
            States.PushBool(stateName, this, "isRendered", IsRendered);
            States.PushInt(stateName, this, "renderedCount", RenderedCount);
        }

        /// <summary>
        /// Restores the earlier saved object state.
        /// </summary>
        /// <param name="stateName">A name of the state being restored.</param>
        public virtual void RestoreState(string stateName)
        {
            if (!States.IsExist(stateName, this)) return;

            RenderedCount = States.PopInt(stateName, this, "renderedCount");
            IsRendered = States.PopBool(stateName, this, "isRendered");
        }

        /// <summary>
        /// Clears all earlier saved object states.
        /// </summary>
        public virtual void ClearAllStates()
        {
            states = null;
        }
        #endregion

		#region IStiShift
        /// <summary>
        /// Gets or sets a value which indicates that this component can be shifted.
        /// </summary>
        [DefaultValue(true)]
        [Browsable(false)]
        [StiCategory("Behavior")]
        [TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets a value which indicates that this component can be shifted.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool Shift
        {
            get
            {
                return ShiftMode == StiShiftMode.IncreasingSize;
            }
            set
            {
                ShiftMode = StiShiftMode.IncreasingSize;
            }
        }

        private StiShiftMode shiftMode = StiShiftMode.IncreasingSize;
        /// <summary>
        /// Gets or sets a value which indicates the shift mode of the component.
        /// </summary>
        [DefaultValue(StiShiftMode.IncreasingSize)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorShiftMode)]
        [TypeConverter(typeof(StiEnumConverter))]
		[Description("Gets or sets value which indicates the shift mode of the component.")]
		[Editor("Stimulsoft.Report.Components.Design.StiShiftModeEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiShiftMode ShiftMode
        {
            get
            {
                return shiftMode;
            }
            set
            {
                shiftMode = value;

                if (value != StiShiftMode.None)
                    Anchor = StiAnchorMode.Left | StiAnchorMode.Top;
            }
        }

        /// <summary>
        /// Gets or sets a guid of component.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [DefaultValue(null)]
        public string Guid { get; set; }

        public void NewGuid()
        {
            Guid = global::System.Guid.NewGuid().ToString("N");
        }
        #endregion

		#region IStiPrintOn
        /// <summary>
		/// Gets or sets a value which indicates on what pages component will be printed.
		/// </summary>
		[StiSerializable]
		[DefaultValue(StiPrintOnType.AllPages)]
		[StiCategory("Behavior")]
		[StiOrder(StiPropertyOrder.BehaviorPrintOn)]
		[TypeConverter(typeof(StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[Description("Gets or sets value which indicates on which pages component will be printed.")]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual StiPrintOnType PrintOn { get; set; } = StiPrintOnType.AllPages;
        #endregion

		#region ICloneable override
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return Clone(true);
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public virtual object Clone(bool cloneProperties)
		{
			var comp = (StiComponent)base.Clone();

			comp.Expressions = this.Expressions?.Clone() as StiAppExpressionCollection;
		
			#region Conditions
			if (this.conditions != null)
                comp.conditions =(StiConditionsCollection)this.conditions.Clone();
			else
                comp.conditions = null;
			#endregion

            #region Interaction
            if (this.interaction != null)
                comp.interaction = (StiInteraction)this.interaction.Clone();
            else
                comp.interaction = null;
            #endregion

            if (comp.interaction != null)
                comp.interaction.ParentComponent = comp;

			if (cloneProperties)
                comp.Properties = this.Properties.Clone() as StiRepositoryItems;

            if (this.bits != null)
                comp.bits = this.bits.Clone() as bitsComponent;

			return comp;
		}
        #endregion

        #region IStiUnitConvert
        /// <summary>
        /// Converts a component out of one unit into another.
        /// </summary>
        /// <param name="oldUnit">Old units.</param>
        /// <param name="newUnit">New units.</param>
        public virtual void Convert(StiUnit oldUnit, StiUnit newUnit, bool isReportSnapshot = false)
        {
            disableCheckWidthHeight = true;

			lockOnResize = true;
            var oldRight = this.Right;
            var oldBottom = this.Bottom;

            this.Left = newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(this.Left));
            this.Top = newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(this.Top));
            this.Width = Math.Round(newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(oldRight)), 2) - this.Left;
            this.Height = Math.Round(newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(oldBottom)), 2) - this.Top;

            disableCheckWidthHeight = false;
			lockOnResize = false;

			if (isReportSnapshot) return;

            this.MinSize = newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(this.MinSize));
            this.MaxSize = newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(this.MaxSize));
        }
		#endregion

		#region IStiCanShrink
        /// <summary>
        /// Gets or sets value which indicates that this object can shrink.
		/// </summary>
		[DefaultValue(false)]
		[StiSerializable]
		[StiCategory("Behavior")]
		[StiOrder(StiPropertyOrder.BehaviorCanShrink)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets value which indicates that this object can shrink.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual bool CanShrink { get; set; }
        #endregion

        #region IStiCanGrow
        //Do not convert to auto-property, otherwise the logic of the StiTableCell component breaks
        private bool canGrow = false;
        /// <summary>
        /// Gets or sets a value which indicates that this object can grow.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorCanGrow)]
        [TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets value which indicates that this object can grow.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual bool CanGrow
        {
            get
            {
                return canGrow;
            }
            set
            {
                canGrow = value;
            }
        }
        #endregion

        #region IStiGrowToHeight
        private bool growToHeight;
        /// <summary>
        /// Gets or sets a value which indicates that the height of this component increases/decreases to the bottom of a container.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorGrowToHeight)]
        [TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets value which indicates that the height of this component increases/decreases to the bottom of a container.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual bool GrowToHeight
        {
            get
            {
                return growToHeight;
            }
            set
            {
                growToHeight = value;

                if (value)
                    Anchor = StiAnchorMode.Left | StiAnchorMode.Top;
            }
        }
        #endregion       

        #region IStiAnchor
        private StiAnchorMode anchor = StiAnchorMode.Left | StiAnchorMode.Top;
        /// <summary>
        /// Gets or sets a value which indicates the mode of linking component location to the parent component size.
        /// </summary>
        [DefaultValue(StiAnchorMode.Left | StiAnchorMode.Top)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorAnchor)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Description("Gets or sets a value which indicates the mode of linking component location to the parent component size.")]
        [Editor("Stimulsoft.Report.Components.Design.StiAnchorModeEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiAnchorMode Anchor
        {
            get
            {
                return anchor;
            }
            set
            {
                anchor = value;

                if (anchor != (StiAnchorMode.Left | StiAnchorMode.Top))
                {
                    GrowToHeight = false;
                    ShiftMode = StiShiftMode.None;
                }
            }
        }
        #endregion

        #region IStiConditions
        private StiConditionsCollection conditions;
		/// <summary>
		/// Gets or sets the collection of conditions.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.List)]
		[StiCategory("Appearance")]
		[StiOrder(StiPropertyOrder.AppearanceConditions)]
		[TypeConverter(typeof(StiConditionsCollectionConverter))]
		[Editor("Stimulsoft.Report.Components.Design.StiConditionsCollectionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual StiConditionsCollection Conditions
		{
			get
			{
			    return conditions ?? (conditions = new StiConditionsCollection());
			}
            set
            {
                conditions = value;
            }
		}

        private bool ShouldSerializeConditions()
        {
            return conditions == null || conditions.Count > 0;
        }
		#endregion

		#region IStiInherited
        protected static object PropertyInherited = new object();
		[Browsable(false)]
		[DefaultValue(false)]
		[StiSerializable]
		public bool Inherited
		{
			get
			{
                return Properties.GetBool(PropertyInherited, false);
			}
			set
			{
                Properties.SetBool(PropertyInherited, value, false); 
			}
		}
		#endregion

		#region IStiGetActualSize
		/// <summary>
		/// Initializes a new instance of the SizeD class from the specified dimensions.
		/// </summary>
		/// <returns>A class of the SizeD type which represents an actual size of the component.</returns>
		public virtual SizeD GetActualSize()
		{
			return new SizeD(this.Width, this.Height);
		}
		#endregion

		#region IStiReportProperty
		public object GetReport()
		{
			return this.Report;
		}

		/// <summary>
		/// Gets or sets the report in which the component is located.
		/// </summary>
		[Browsable(false)]
		[Description("Gets or sets the report in which the component is located.")]
		public virtual StiReport Report
		{
			get
			{
			    return Page?.Report;
			}
			set
			{
			}
		}
		#endregion

		#region IStiInteraction
		private StiInteraction interaction;
		/// <summary>
		/// Gets interaction options of this component.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.Class)]
		[StiCategory("Behavior")]
		[StiOrder(StiPropertyOrder.BehaviorInteractive)]
        [Editor("Stimulsoft.Report.Components.Design.StiInteractionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets interaction options of this component.")]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual StiInteraction Interaction
		{
			get
			{
				return interaction;
			}
			set
			{
				if (this.interaction != value)
				{
					interaction = value;

					if (value != null)
						interaction.ParentComponent = this;
				}
			}
		}

        private bool ShouldSerializeInteraction()
        {
            return interaction == null || !interaction.IsDefault;
        }
        #endregion

        #region IStiGetFonts
        public virtual List<StiFont> GetFonts()
        {
            var result = new List<StiFont>();
            foreach (var condition in Conditions)
            {
                if (condition is IStiGetFonts)
                {
                    result.AddRange((condition as IStiGetFonts).GetFonts());
                }
            }
            return result.Distinct().ToList();
        }
		#endregion

		#region IStiAppExpressionCollection
		/// <summary>
		/// Gets the collection of expressions that are associated with properties.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.List, StiSerializeTypes.SerializeToAllExceptDocument)]
		[Browsable(false)]
		public virtual StiAppExpressionCollection Expressions { get; set; } = new StiAppExpressionCollection();

		private bool ShouldSerializeExpressions()
        {
			return Expressions == null || Expressions.Count != 0;
        }
		#endregion

		#region StiComponent.Properties
		[Browsable(false)]
        public virtual string HelpUrl => null;
        #endregion

        #region StiService override
        /// <summary>
		/// Packs a service.
		/// </summary>
		public override void PackService()
		{
			base.PackService();

            if (conditions != null && conditions.Count == 0)
			    conditions = null;
		}


		/// <summary>
		/// Gets a category of services.
		/// </summary>
		[Browsable(false)]
		public override string ServiceCategory => StiLocalization.Get("Report", "Components");

        /// <summary>
		/// Gets a service type.
		/// </summary>
		[Browsable(false)]
		public override Type ServiceType => typeof(StiComponent);
        #endregion

        #region Methods.virtual
        public virtual StiComponent CreateNew()
        {
            throw new NotImplementedException();
        }
		#endregion

		#region Render
		/// <summary>
		/// Invokes the GetBookmark event.
		/// </summary>
		public virtual void DoPointer(bool createNewGuid = true)
		{
			DoGetPointer(createNewGuid);
		}

		/// <summary>
		/// Raises the GetBookmark event.
		/// </summary>
		internal void DoGetPointer(bool createNewGuid)
		{
			var isCompilationMode = Report == null || Report.CalculationMode == StiCalculationMode.Compilation;

			if ((isCompilationMode && this.Events[EventGetPointer] != null) ||
				(!isCompilationMode && this.Pointer.Value.Length > 0))
			{
				var args = new StiValueEventArgs();

				if (isCompilationMode)
				{
					InvokeGetPointer(this, args);
				}
				else
				{
					var parserResult = StiParser.ParseTextValue(this.Pointer.Value, this);
					args.Value = Report.ToString(parserResult);
				}

				if (args.Value is string && ((string)args.Value).Length > 0)
				{
					if (Report != null && Report.EngineVersion == StiEngineVersion.EngineV2)
					{
						#region EngineV2
						if (createNewGuid) this.NewGuid();
						var text = args.Value as string;

						CurrentPointer = StiBookmarksV2Helper.CreateBookmark(text, this.Guid);

						if (ParentPointer != null)
							ParentPointer.Bookmarks.Add(CurrentPointer);
						#endregion
					}
				}
				else
					CurrentPointer = ParentPointer;
			}
			else
				CurrentPointer = ParentPointer;
		}

		/// <summary>
		/// Invokes the GetBookmark event.
		/// </summary>
		public virtual bool DoBookmark()
		{
			return DoGetBookmark();
		}				
		
		/// <summary>
		/// Raises the GetBookmark event.
		/// </summary>
		internal bool DoGetBookmark()
		{
			bool isGuidCreated = false;
            var isCompilationMode = Report == null || Report.CalculationMode == StiCalculationMode.Compilation;

            if ((isCompilationMode && this.Events[EventGetBookmark] != null) ||
                (!isCompilationMode && this.Bookmark.Value.Length > 0))
            {
                var args = new StiValueEventArgs();

                if (isCompilationMode)
                    InvokeGetBookmark(this, args);

                else
                {
                    var parserResult = StiParser.ParseTextValue(this.Bookmark.Value, this);
                    args.Value = Report.ToString(parserResult);
                }

                if (args.Value is string && ((string)args.Value).Length > 0)
                {
                    if (Report != null && Report.EngineVersion == StiEngineVersion.EngineV2)
                    {
                        #region EngineV2
                        this.NewGuid();
						isGuidCreated = true;
                        var text = args.Value as string;

                        #region % symbol processing
                        //%[delimeter]level1[delimeter]level2[delimeter]level3
                        if (text.Length > 2 && text[0] == '%')
                        {
                            var splitter = text[1];
                            text = text.Substring(2);
                            var strs = text.Split(splitter);

                            var processedBookmark = this.Report.Bookmark;
                            for (var index = 0; index < strs.Length; index++)
                            {
                                var str = strs[index];

                                processedBookmark = StiBookmarksV2Helper.GetBookmark(processedBookmark, str);

                                //Sets to the last bookmark the guid of a component
                                if (index == strs.Length - 1)
                                    processedBookmark.ComponentGuid = this.Guid;
                            }
                            CurrentBookmark = processedBookmark;
                        }
                        #endregion

                        #region Standard bookmarks processing
                        else
                        {
                            CurrentBookmark = StiBookmarksV2Helper.CreateBookmark(text, this.Guid);

                            if (ParentBookmark != null)
                                ParentBookmark.Bookmarks.Add(CurrentBookmark);
                        }
                        #endregion

                        #endregion
                    }
                    else
                    {
                        #region EngineV1
                        CurrentBookmark = StiBookmarksV1Helper.CreateBookmark(args.Value as string, this);
                        ParentBookmark.Bookmarks.Add(CurrentBookmark);

                        var text = args.Value as string;
                        if (text.Length > 2 && text[0] == '%')
                        {
                            var bookmarkText = text.Substring(0, 1);

                            var splitter = text[1];
                            text = text.Substring(2);
                            var strs = text.Split(splitter);

                            var manualBookmark = this.Report.ManualBookmark;
                            foreach (var str in strs)
                            {
                                bookmarkText += splitter + str;

                                manualBookmark = StiBookmarksV1Helper.GetBookmark(manualBookmark, str);
                                manualBookmark.BookmarkText = bookmarkText;
                            }
                            manualBookmark.BookmarkText = args.Value as string;
                        }
                        #endregion
                    }
                }
                else
                    CurrentBookmark = ParentBookmark;
            }
            else
                CurrentBookmark = ParentBookmark;

			return isGuidCreated;
		}
		
		/// <summary>
		/// Gets or sets a value which indicates whether the component is printable or not.
		/// </summary>
		[StiSerializable]
		[DefaultValue(true)]
		[StiCategory("Behavior")]
		[StiOrder(StiPropertyOrder.BehaviorPrintable)]
		[TypeConverter(typeof(StiExpressionBoolConverter))]
		[Editor(StiEditors.ExpressionBool, typeof(UITypeEditor))]
		[Description("Gets or sets value which indicates whether the component is printable or not.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Professional)]
		[StiExpressionAllowed]
		public virtual bool Printable
		{
            get
            {
                if (bits == null)
                    return true;
                else
                    return bits.printable;
            }
            set
            {
                if (value && bits == null)
                    return;

                if (bits != null)
                    bits.printable = value;
                else
                    bits = new bitsComponent(this.BookmarkValue, this.ToolTipValue, this.HyperlinkValue, this.TagValue,
                        this.Enabled, this.HighlightState, this.IgnoreNamingRule,
                        this.DockStyle, value);
            }
		}

        /// <summary>
		/// Gets or sets a value which indicates whether the component is rendered or not.
		/// </summary>
		[Browsable(false)]
		public virtual bool IsRendered { get; set; }

        protected static object PropertyRenderedCount = new object();
		/// <summary>
		/// Gets or sets a value which indicates how many times the component is rendered.
		/// </summary>
		[Browsable(false)]
		public int RenderedCount
		{
			get
			{
                return Properties.GetInt(PropertyRenderedCount, 0);
			}
			set
			{
                Properties.SetInt(PropertyRenderedCount, value, 0); 
			}
		}						
		
		protected bool AllowPrintOn()
		{
            if (this.PrintOn == StiPrintOnType.AllPages) return true;

			#region EngineV2
			if (this.Report != null && this.Report.EngineVersion == StiEngineVersion.EngineV2)
			{				
				var pageNumber = this.Report.PageNumber;
				var totalPageCount = this.Report.TotalPageCount;

				if (!StiOptions.Engine.UseAdvancedPrintOnEngineV2)
				{
					if ((this.PrintOn & StiPrintOnType.ExceptFirstPage) > 0 && pageNumber == 1) return false;
					if (this.PrintOn == StiPrintOnType.OnlyFirstPage && pageNumber > 1) return false;
					return true;
				}				

				#region Correction for Segmented pages
				if (Page.SegmentPerWidth > 1 || Page.SegmentPerHeight > 1)
				{
					pageNumber += Page.SegmentPerWidth * Page.SegmentPerHeight - 1;
				}
				#endregion

				if (this.Report.ReportPass == StiReportPass.First)
				{
					if ((this.PrintOn & StiPrintOnType.OnlyLastPage) > 0) return false;
					if ((this.PrintOn & StiPrintOnType.ExceptLastPage) > 0) return true;
				}

				if ((this.PrintOn & StiPrintOnType.ExceptFirstPage) > 0 && pageNumber == 1) return false;
				if (this.PrintOn == StiPrintOnType.OnlyFirstAndLastPage)
				{
					if (pageNumber > 1 && pageNumber < totalPageCount) return false;
					else return true;
				}
				else
				{
					if ((this.PrintOn & StiPrintOnType.OnlyFirstPage) > 0 && pageNumber > 1) return false;
					if ((this.PrintOn & StiPrintOnType.OnlyLastPage) > 0 && pageNumber < totalPageCount) return false;
				}

				if ((this.PrintOn & StiPrintOnType.ExceptLastPage) > 0 && pageNumber == totalPageCount) return false;

				return true;
			}
			#endregion

			#region EngineV1
			else
			{
				if (!StiOptions.Engine.UseAdvancedPrintOnEngineV1) return true;

				if (this.Report == null) return true;

				var pageNumber = this.Report.PageNumber;
				var totalPageCount = this.Report.TotalPageCount;

				#region Correction for Segmented pages
				if (Page.SegmentPerWidth > 1 || Page.SegmentPerHeight > 1)
				{
					pageNumber += Page.SegmentPerWidth * Page.SegmentPerHeight - 1;
				}
				#endregion

				if (this.Report.ReportPass == StiReportPass.First)
				{
					if ((this.PrintOn & StiPrintOnType.OnlyLastPage) > 0) return false;
					if ((this.PrintOn & StiPrintOnType.ExceptLastPage) > 0) return true;
				}

				if ((this.PrintOn & StiPrintOnType.ExceptFirstPage) > 0 && pageNumber == 1) return false;
				if (this.PrintOn == StiPrintOnType.OnlyFirstAndLastPage)
				{
					if (pageNumber > 1 && pageNumber < totalPageCount) return false;
					else return true;
				}
				else
				{
					if ((this.PrintOn & StiPrintOnType.OnlyFirstPage) > 0 && pageNumber > 1) return false;
					if ((this.PrintOn & StiPrintOnType.OnlyLastPage) > 0 && pageNumber < totalPageCount) return false;
				}

				if ((this.PrintOn & StiPrintOnType.ExceptLastPage) > 0 && pageNumber == totalPageCount) return false;

				return true;
			}
			#endregion
		}

        [Browsable(false)]
        public virtual bool IsEnabled => Enabled && AllowPrintOn();
        #endregion

        #region Render.Main
        /// <summary>
        /// Prepare content for rendering.
        /// </summary>
        public virtual void Prepare()
		{
			if (Report != null && Report.EngineVersion == StiEngineVersion.EngineV1)
				StiV1Builder.GetBuilder(GetType()).Prepare(this);

			else
				StiV2Builder.GetBuilder(GetType()).Prepare(this);
		}

		/// <summary>
		/// Clear component afrer render.
		/// </summary>
		public virtual void UnPrepare()
		{
			if (Report != null && Report.EngineVersion == StiEngineVersion.EngineV1)
				StiV1Builder.GetBuilder(GetType()).UnPrepare(this);

			else
				StiV2Builder.GetBuilder(GetType()).UnPrepare(this);
		}
        #endregion

        #region Render.Main.EngineV1
        /// <summary>
        /// Renders a component in the specified container without generation of the 'BeforePrintEvent' event and the 'AfterPrintEvent' event and 
        /// without checking Conditions. The rendered component will return in 'renderedComponent' argument. 
        /// This is backward compatibility method. Please use InternalRender method for new developed components.
        /// </summary>
        /// <param name="renderedComponent">Rendered component.</param>
        /// <param name="outContainer">Panel in which rendering will be done.</param>
        /// <returns>Is rendering finished or not.</returns>
        protected virtual bool RenderComponent(ref StiComponent renderedComponent, StiContainer outContainer)
        {
            return StiV1Builder.GetBuilder(GetType()).InternalRender(this, ref renderedComponent, outContainer);
        }

        /// <summary>
        /// Renders a component in the specified container without generation of the 'BeforePrintEvent' event and the 'AfterPrintEvent' event and 
        /// without checking Conditions. The rendered component will return in 'renderedComponent' argument.
        /// </summary>
        /// <param name="renderedComponent">Rendered component.</param>
        /// <param name="outContainer">Panel in which rendering will be done.</param>
        /// <returns>Is rendering finished or not.</returns>
        public virtual bool InternalRender(ref StiComponent renderedComponent, StiContainer outContainer)
		{
			return RenderComponent(ref renderedComponent, outContainer);
		}

        /// <summary>
        /// Renders a component in the specified container with events generation. 
        /// The rendered component will return in 'renderedComponent' argument.
        /// </summary>
        /// <param name="renderedComponent">A component which is being rendered.</param>
        /// <param name="outContainer">A container in which rendering will be done.</param>
        /// <returns>A value which indicates whether rendering of the component is finished or not.</returns>
        public virtual bool Render(ref StiComponent renderedComponent, StiContainer outContainer)
		{	
			return StiV1Builder.GetBuilder(GetType()).Render(this, ref renderedComponent, outContainer);
		}
		
		/// <summary>
		/// Renders a component in the specified container with events generation.
		/// </summary>
		/// <param name="outContainer">The container in which rendering will be done.</param>
		/// <returns>A value which indicates whether rendering is finished or not.</returns>
		public virtual bool Render(StiContainer outContainer)
		{
			return StiV1Builder.GetBuilder(GetType()).Render(this, outContainer);
		}	
		#endregion
		
		#region Render.Main.EngineV2
		public virtual void SetReportVariables()
		{
			var builder = StiV2Builder.GetBuilder(this.GetType());
			builder.SetReportVariables(this);
		}

		public virtual StiComponent InternalRender()
		{
			var builder = StiV2Builder.GetBuilder(this.GetType());
			return builder.InternalRender(this);
		}

		public virtual StiComponent Render()
		{
			var builder = StiV2Builder.GetBuilder(this.GetType());
			return builder.Render(this);
		}
		#endregion

		#region Paint
		/// <summary>
        /// Gets a value which indicates whether invalidating of the component occurs when a mouse pointer is above the component.
        /// </summary>
		[Browsable(false)]
		public virtual bool InvalidateOnMouseOver => false;

        /// <summary>
        /// Gets a thumbnail image in the byte aray of the component.
        /// </summary>
        /// <param name="maxSize">Maximum width or height of the thumbnail image.</param>
        /// <returns>A thumbnail image in the byte array of the specified size.</returns>
        public virtual byte[] GetThumbnailAsBytes(int maxSize)
        {
            using (var image = GetThumbnail(maxSize))
            {
                return StiImageConverter.ImageToBytes(image);
            }
        }

        /// <summary>
        /// Gets a thumbnail image of the component.
        /// </summary>
        /// <param name="maxSize">Maximum width or height of the thumbnail image.</param>
        /// <returns>A thumbnail image of the specified size.</returns>
        public virtual Bitmap GetThumbnail(int maxSize, bool isDesignTime = false)
        {
            if (this.Width > this.Height)
                return GetThumbnail(maxSize, (int)(maxSize * this.Height / this.Width), isDesignTime);

            else
                return GetThumbnail((int)(maxSize * this.Height / this.Width), maxSize, isDesignTime);
        }

        /// <summary>
        /// Gets a thumbnail image of the component.
        /// </summary>
        /// <param name="width">Width of the thumbnail image.</param>
        /// <param name="height">Height of the thumbnail image.</param>
        /// <returns>A thumbnail image of the specified size.</returns>
        public virtual Bitmap GetThumbnail(int width, int height)
        {
            return GetThumbnail(width, height, false);
        }

        /// <summary>
        /// Gets a thumbnail image of the component.
        /// </summary>
        /// <param name="width">Width of the thumbnail image.</param>
        /// <param name="height">Height of the thumbnail image.</param>
        /// <returns>A thumbnail image of the specified size.</returns>
		public virtual Bitmap GetThumbnail(int width, int height, bool isDesignTime)
		{
            var painter = StiPainter.GetPainter(this.GetType(), StiGuiMode.Gdi);
            return painter.GetThumbnail(this, width, height, isDesignTime);
		}

        public virtual string GetQuickInfo()
        {
            switch (this.Report.Info.QuickInfoType)
            {
                case StiQuickInfoType.ShowAliases:

                    if (!string.IsNullOrEmpty(this.Alias))
                        return this.Alias;

                    return string.Empty;

                case StiQuickInfoType.ShowEvents:
                    var events = this.GetEvents();

                    var evStrs = string.Empty;
                    foreach (StiEvent ev in events)
                    {
                        if (ev.Script.Length == 0) continue;

                        if (evStrs.Length > 0)
                            evStrs += ',';

                        evStrs += ev.ToString();
                    }
                    return evStrs;

                case StiQuickInfoType.ShowComponentsNames:
                    return (this.Alias.Length == 0 ? this.Name : this.Alias);

                case StiQuickInfoType.ShowFields:
                    var text = this as IStiText;
                    if (text == null)
                        return string.Empty;

                    var startIndex = text.GetTextInternal().LastIndexOf('.');
                    if (startIndex >= 0)
                        return text.GetTextInternal().Substring(startIndex);

                    return string.Empty;

                case StiQuickInfoType.ShowFieldsOnly:
                    return string.Empty;

                default:
                    return string.Empty;
            }
        }

		/// <summary>
		/// Paints a component.
		/// </summary>
		/// <param name="e">Argument for painting.</param>
		public virtual void Paint(StiPaintEventArgs e)
		{
			var guiMode = (e.Context is Graphics)
		        ? StiGuiMode.Gdi
		        : StiGuiMode.Wpf;

		    var painter = StiPainter.GetPainter(this.GetType(), guiMode);
			painter.AnimationEngine = e.AnimationEngine;
			painter.Paint(this, e);
		}

        /// <summary>
        /// Paints the selection of the component.
        /// </summary>
        public virtual void PaintSelection(StiPaintEventArgs e)
        {
			var guiMode = (e.Context is Graphics)
                ? StiGuiMode.Gdi
                : StiGuiMode.Wpf;

            var painter = StiPainter.GetPainter(this.GetType(), guiMode);
            painter.PaintSelection(this, e);
        }

        /// <summary>
        /// Paints the highlight of the component.
        /// </summary>
        public virtual void PaintHighlight(StiPaintEventArgs e)
        {
			var guiMode = (e.Context is Graphics)
                ? StiGuiMode.Gdi
                : StiGuiMode.Wpf;

            var painter = StiPainter.GetPainter(this.GetType(), guiMode);
            painter.PaintHighlight(this, e);
        }

        /// <summary>
        /// Paints a markers specified by a Rectangle structure. This method can be used only in win version.
        /// </summary>
        /// <param name="g">The Graphics to draw on.</param>
        /// <param name="rect">RectangleD structure that represents the rectangle to draw markers.</param>
        public virtual void PaintMarkers(Graphics g, RectangleD rect)
        {
            var painter = StiPainter.GetPainter(this.GetType(), StiGuiMode.Gdi) as StiComponentGdiPainter;
            painter.PaintMarkers(this, g, rect);
        }

        /// <summary>
        /// Paints events of a component.
        /// </summary>
        /// <param name="g">The Graphics to draw on.</param>
        /// <param name="rect">RectangleD structure that represents the rectangle to draw markers.</param>
        public virtual void PaintEvents(Graphics g, RectangleD rect)
        {
            var painter = StiPainter.GetPainter(this.GetType(), StiGuiMode.Gdi) as StiComponentGdiPainter;
            painter.PaintEvents(this, g, rect);
        }
		#endregion

		#region Dock
		public static DockStyle ConvertDockStyle(StiDockStyle dockStyle)
		{
			switch (dockStyle)
			{
			    case StiDockStyle.Fill:
			        return System.Windows.Forms.DockStyle.Fill;

			    case StiDockStyle.Left:
			        return System.Windows.Forms.DockStyle.Left;

			    case StiDockStyle.Right:
			        return System.Windows.Forms.DockStyle.Right;

			    case StiDockStyle.Top:
			        return System.Windows.Forms.DockStyle.Top;

			    case StiDockStyle.Bottom:
			        return System.Windows.Forms.DockStyle.Bottom;

                default:
                    return System.Windows.Forms.DockStyle.None;
            }
		}

		public static StiDockStyle ConvertDock(DockStyle dock)
		{
		    switch (dock)
		    {
		        case System.Windows.Forms.DockStyle.Fill:
		            return StiDockStyle.Fill;

		        case System.Windows.Forms.DockStyle.Left:
		            return StiDockStyle.Left;

		        case System.Windows.Forms.DockStyle.Right:
		            return StiDockStyle.Right;

		        case System.Windows.Forms.DockStyle.Top:
		            return StiDockStyle.Top;

		        case System.Windows.Forms.DockStyle.Bottom:
		            return StiDockStyle.Bottom;

		        default:
		            return StiDockStyle.None;
		    }
		}

		/// <summary>
		/// Gets or sets a type of the component docking.
		/// </summary>
		[StiNonSerialized]
		[Browsable(false)]
		[TypeConverter(typeof(StiEnumConverter))]
		[Description("Gets or sets a type of the component docking.")]
		[Obsolete("Please use DockStyle property instead Dock property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual DockStyle Dock
		{
			get 
			{
				return ConvertDockStyle(DockStyle);
			}
			set 
			{
				this.DockStyle = ConvertDock(value);
			}
		}
		
		/// <summary>
		/// Gets or sets a type of the component docking.
		/// </summary>
		[StiSerializable]
		[DefaultValue(StiDockStyle.None)]
		[StiCategory("Behavior")]
		[StiOrder(StiPropertyOrder.BehaviorDockStyle)]
		[Description("Gets or sets a type of the component docking.")]
        [StiPropertyLevel(StiLevel.Standard)]
		[TypeConverter(typeof(StiExpressionEnumConverter))]
		[Editor(StiEditors.ExpressionEnum, typeof(UITypeEditor))]
		[StiExpressionAllowed]
		public virtual StiDockStyle DockStyle
		{
            get
            {
                return bits == null ? StiDockStyle.None : bits.dockStyle;
            }
            set
            {
                if (value == StiDockStyle.None && bits == null)
                    return;

                if (bits != null)
                    bits.dockStyle = value;
                else
                    bits = new bitsComponent(this.BookmarkValue, this.ToolTipValue, this.HyperlinkValue, this.TagValue,
                        this.Enabled, this.HighlightState, this.IgnoreNamingRule,
                        value, this.Printable);
            }
		}

		/// <summary>
		/// Gets a value which indicates that this is an automatic docking.
		/// </summary>
		[Browsable(false)]
		public virtual bool IsAutomaticDock => false;

        /// <summary>
		/// Retuns an empty rectangle to which docking is possible.
		/// </summary>
		/// <param name="parent">Container being measured.</param>
		/// <returns>An empty rectangle to which docking is possible.</returns>
        public virtual RectangleD GetDockRegion(StiComponent parent)
        {
            return GetDockRegion(parent, true);
        }

		/// <summary>
		/// Retuns an empty rectangle to which docking is possible.
		/// </summary>
		/// <param name="parent">Container being measured.</param>
		/// <returns>An empty rectangle to which docking is possible.</returns>
		public virtual RectangleD GetDockRegion(StiComponent parent, bool useColumns)
		{
		    if (parent == null)
		        return ClientRectangle;

		    #region Gets current coordinates
		    var contRect = parent.ClientRectangle;
		    var page = parent as StiPage;
				
		    if (this.Report != null && this.Report.EngineVersion == StiEngineVersion.EngineV1)
		    {
		        if ((this is StiDynamicBand || this is StiReportSummaryBand) && page != null && page.Columns > 1)
		            contRect.Width = page.GetColumnWidth();
		    }
		    else
		    {
		        if (this.ComponentType != StiComponentType.Static && page != null && page.Columns > 1 && useColumns)
		            contRect.Width = page.GetColumnWidth();

		        var panel = parent as StiPanel;
		        if (this.ComponentType != StiComponentType.Static && panel != null && panel.Columns > 1 && useColumns)
		            contRect.Width = panel.GetColumnWidth();
		    }
		    #endregion

		    //If measured container is selected execute offset
		    if (parent.IsSelected)
		        contRect = DoOffsetRect(parent, contRect, parent.Page.OffsetRectangle);

		    //Since returned rectangle belongs to measured container - zeroize position
		    contRect.X = 0;
		    contRect.Y = 0;

		    var cont = (StiContainer)parent;

		    #region Foreach all components and reduce available zone on their area
		    foreach (StiComponent comp in cont.Components)
		    {
		        if (!comp.Dockable) continue;
		        if (!IsDesigning && !comp.Enabled) continue;
		        if (comp == this)break;

		        var disRect = comp.DisplayRectangle;
		        if (comp.IsSelected)
		            disRect = DoOffsetRect(comp, disRect, comp.Page.OffsetRectangle);
					
		        switch(comp.DockStyle)
		        {
		            case StiDockStyle.Left:
		                contRect.X += disRect.Width;
		                contRect.Width -= disRect.Width;
		                break;

		            case StiDockStyle.Right:
		                contRect.Width -= disRect.Width;
		                break;

		            case StiDockStyle.Top:
		                contRect.Y += disRect.Height;
		                contRect.Height -= disRect.Height;
		                break;

		            case StiDockStyle.Bottom:
		                contRect.Height -= disRect.Height;
		                break;
		        }
		    }
		    #endregion

		    return contRect;
		}

		/// <summary>
		/// Docks the component and all of its subordinate components.
		/// </summary>
		public virtual void DockToContainer()
		{
			if (DockStyle != StiDockStyle.None && Dockable && (IsDesigning || Enabled))
			{
			    if (this.IsCross)
			        ClientRectangle = DockToContainer(ClientRectangle);

			    else
			        DisplayRectangle = DockToContainer(DisplayRectangle);
			}

			var container = this as StiContainer;
			if (container != null)
			{
				foreach (StiComponent component in container.Components)
				{
					if (component == container) continue;
							 
					component.DockToContainer();
				}
			}
		}

		/// <summary>
		/// Docks a rectangle, relatively to the component, into the rectangle.
		/// </summary>
		/// <param name="rect">Rectangle for docking.</param>
		/// <returns>Docked client area of the component.</returns>
		public virtual RectangleD DockToContainer(RectangleD rect)
		{
		    if (Parent == null || DockStyle == StiDockStyle.None || !Dockable) return rect;

		    var contRect = GetDockRegion(Parent);
		    if (Parent.Height == 100000000000 && DockStyle != StiDockStyle.Top)
		    {
		        contRect.Y = 0;
		        contRect.Height = rect.Height;
		    }

		    #region Dock component
		    switch(DockStyle)
		    {
		        case StiDockStyle.Left:
		            rect.X = contRect.X;
		            rect.Y = contRect.Y;
		            rect.Height = contRect.Height;
		            break;

		        case StiDockStyle.Right:
		            rect.X = contRect.Right - rect.Width;
		            rect.Y = contRect.Y;
		            rect.Height = contRect.Height;
		            break;

		        case StiDockStyle.Top:
		            rect.X = contRect.X;
		            rect.Y = contRect.Y;
		            rect.Width = contRect.Width;
		            break;

		        case StiDockStyle.Bottom:
		            rect.X = contRect.X;
		            rect.Y = contRect.Bottom - rect.Height;
		            rect.Width = contRect.Width;
		            break;

		        case StiDockStyle.Fill:
		            rect.X = contRect.X;
		            rect.Y = contRect.Y;
		            rect.Width = contRect.Width;
		            rect.Height = contRect.Height;
		            break;
		    }
		    #endregion

		    return rect;
		}
		#endregion		

		#region Position
        private double CheckWidth(double width)
        {
            if (disableCheckWidthHeight)
                return width;

            if (this.MinSize.Width != 0)
                width = Math.Max(this.MinSize.Width, width);

            if (this.MaxSize.Width != 0)
                width = Math.Min(this.MaxSize.Width, width);

            return width;
        }

        private double CheckHeight(double height)
        {
            if (disableCheckWidthHeight)
                return height;

            if (this.MinSize.Height != 0)
                height = Math.Max(this.MinSize.Height, height);

            if (this.MaxSize.Height != 0)
                height = Math.Min(this.MaxSize.Height, height);

            return height;
        }

        private bool disableCheckWidthHeight;

        protected static object PropertyMinSize = new object();
        /// <summary>
        /// Gets or sets minimal size.
        /// </summary>
        [StiCategory("Position")]
        [StiOrder(500)]
        [Description("Gets or sets minimal size.")]
        [StiSerializable]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual SizeD MinSize
        {
            get
            {
                return (SizeD)Properties.Get(PropertyMinSize, SizeD.Empty);
            }
            set
            {
                var minSize = this.MinSize;
                if (minSize.Width != value.Width || minSize.Height != value.Height)
                {
                    Properties.Set(PropertyMinSize, value, SizeD.Empty);
                    width = CheckWidth(width);
                    height = CheckHeight(height);
                }
            }
        }

        private bool ShouldSerializeMinSize()
        {
            return !MinSize.IsDefault;
        }

        protected static object PropertyMaxSize = new object();
        /// <summary>
        /// Gets or sets maximal size.
        /// </summary>
        [StiCategory("Position")]
        [StiOrder(510)]
        [Description("Gets or sets maximal size.")]
        [StiSerializable]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual SizeD MaxSize
        {
            get
            {
                return (SizeD)Properties.Get(PropertyMaxSize, SizeD.Empty);
            }
            set
            {
                var maxSize = this.MaxSize;
                if (maxSize.Width != value.Width || maxSize.Height != value.Height)
                {
                    Properties.Set(PropertyMaxSize, value, SizeD.Empty);
                    width = CheckWidth(width);
                    height = CheckHeight(height);
                }
            }
        }

        private bool ShouldSerializeMaxSize()
        {
            return !MaxSize.IsDefault;
        }

        private double left;
		/// <summary>
		/// Gets or sets left position of a component.
		/// </summary>
		[StiCategory("Position")]
		[StiOrder(100)]
        [Description("Gets or sets left position of the component.")]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual double Left
		{
			get 
			{
				return left;
			}
			set 
			{
				left = Math.Round(value, 2);
			}
		}

        private double top;
		/// <summary>
		/// Gets or sets top position of a component.
		/// </summary>
		[StiCategory("Position")]
		[StiOrder(110)]
		[Description("Gets or sets top position of the component.")]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual double Top
		{
			get 
			{
				return top;
			}
			set 
			{
				top = Math.Round(value, 2);
			}
		}

		private double width;
		/// <summary>
		/// Gets or sets width of a component.
		/// </summary>
		[StiCategory("Position")]
		[StiOrder(120)]
		[Description("Gets or sets width of the component.")]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual double Width
		{
			get 
			{
				return width;
			}
			set 
			{
                var oldValue = width;
				width = CheckWidth(Math.Round(value, 2));

                if (width != oldValue) 
                    InvokeOnResizeComponent(new SizeD(oldValue, height), new SizeD(width, height));
			}
		}

        private double height;
		/// <summary>
		/// Gets or sets height of a component.
		/// </summary>
		[StiCategory("Position")]
		[StiOrder(130)]
		[Description("Gets or sets height of the component.")]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual double Height
		{
			get 
			{
				return height;
			}
			set 
			{
                var oldValue = height;
                height = CheckHeight(Math.Round(value, 2));

                if (height != oldValue)
                    InvokeOnResizeComponent(new SizeD(width, oldValue), new SizeD(width, height));
            }
		}

		/// <summary>
		/// Gets right position of a component.
		/// </summary>
		[Browsable(false)]
		[Description("Gets right position of the component.")]
		public virtual double Right => Left + Width;

        /// <summary>
		/// Gets bottom position of a component.
		/// </summary>
		[Browsable(false)]
		[Description("Gets bottom position of the component.")]
		public virtual double Bottom => Top + Height;

        /// <summary>
        /// Gets or sets the client area of a component.
        /// </summary>
        [Browsable(false)]
		[StiSerializable]
		public virtual RectangleD ClientRectangle
		{
			get
			{
				return new RectangleD(Left, Top, Width, Height);
			}
			set
			{
                var oldWidth = width;
                var oldHeight = height;

				this.left =		Math.Round(value.Left, 2);
				this.top =		Math.Round(value.Top, 2);
				this.width =	CheckWidth(Math.Round(value.Width, 2));
				this.height =	CheckHeight(Math.Round(value.Height, 2));

                if (width != oldWidth || height != oldHeight)
                    InvokeOnResizeComponent(new SizeD(oldWidth, oldHeight), new SizeD(width, height));
			}
		}

		/// <summary>
		/// Gets or sets a rectangle of the component which it fills. Docking occurs in accordance to the area of a component
		/// (Cross - components are docked by ClientRectangle).
		/// </summary>
		[Browsable(false)]
		public virtual RectangleD DisplayRectangle
		{
			get
			{
				return new RectangleD(Left, Top, Width, Height);
			}
			set
			{
                var oldWidth = width;
                var oldHeight = height;

				this.left =		Math.Round(value.Left, 2);
				this.top =		Math.Round(value.Top, 2);
				this.width =	CheckWidth(Math.Round(value.Width, 2));
				this.height =	CheckHeight(Math.Round(value.Height, 2));

                if (width != oldWidth || height != oldHeight)
                    InvokeOnResizeComponent(new SizeD(oldWidth, oldHeight), new SizeD(width, height));
			}
		}

        protected internal virtual void SetDirectDisplayRectangle(RectangleD rect)
        {
            var oldWidth = width;
            var oldHeight = height;

            this.left = rect.Left;
            this.top = rect.Top;
            this.width = rect.Width;
            this.height = rect.Height;

            if (width != oldWidth || height != oldHeight)
                InvokeOnResizeComponent(new SizeD(oldWidth, oldHeight), new SizeD(width, height));
        }
	
		/// <summary>
		/// Gets or sets a rectangle of a component selection.
		/// </summary>
		[Browsable(false)]
		public virtual RectangleD SelectRectangle
		{
			get
			{
				return DisplayRectangle;
			}
			set
			{
				DisplayRectangle = value;
			}
		}

		/// <summary>
		/// Gets the default client area of a component.
		/// </summary>
		[Browsable(false)]
		public virtual RectangleD DefaultClientRectangle => new RectangleD(0, 0, 60, 20);
        #endregion

		#region Bookmarks
        /// <summary>
		/// Gets or sets parent bookmark for this component.
		/// </summary>
		[Browsable(false)]
		public StiBookmark ParentBookmark { get; set; }

        /// <summary>
		/// Gets or sets current bookmark for this component.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public StiBookmark CurrentBookmark { get; set; }
		#endregion

		#region Pointers
		/// <summary>
		/// Gets or sets parent bookmark for this component.
		/// </summary>
		[Browsable(false)]
		public StiBookmark ParentPointer { get; set; }

		/// <summary>
		/// Gets or sets current bookmark for this component.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public StiBookmark CurrentPointer { get; set; }
		#endregion

		#region Events
		/// <summary>
		/// Invokes all events for this components.
		/// </summary>
		public virtual void InvokeEvents()
		{
			try
			{
                var isCompilationMode = true;

                if (Report != null)
                    isCompilationMode = Report.CalculationMode == StiCalculationMode.Compilation;

                var parserParameters = new StiParserParameters();

				#region GetPointer
				if (!isCompilationMode)
				{
					if (PointerValue == null && this.Pointer.Value.Length > 0)
					{
						var parserResult = StiParser.ParseTextValue(this.Pointer.Value, this);
						this.PointerValue = Report.ToString(parserResult);
					}
				}
				if (this.Events[EventGetPointer] != null && PointerValue == null)
				{
					var e = new StiValueEventArgs();
					InvokeGetPointer(this, e);
					this.PointerValue = e.Value;
				}
				else
				{
					StiBlocklyHelper.InvokeBlockly(this.Report, this, GetPointerEvent, null);
				}
				#endregion

				#region GetBookmark
				if (!isCompilationMode)
				{
					if (BookmarkValue == null && this.Bookmark.Value.Length > 0)
					{
						var parserResult = StiParser.ParseTextValue(this.Bookmark.Value, this);
						this.BookmarkValue = Report.ToString(parserResult);
					}
				}
				if (this.Events[EventGetBookmark] != null && BookmarkValue == null)
				{
					var e = new StiValueEventArgs();
					InvokeGetBookmark(this, e);
					this.BookmarkValue = e.Value;
				}
				else
				{
					StiBlocklyHelper.InvokeBlockly(this.Report, this, GetBookmarkEvent, null);
				}
				#endregion

				#region GetTag
				if (!isCompilationMode)
				{
					if (TagValue == null && this.Tag.Value.Length > 0)
					{
						parserParameters.GlobalizedNameExt = ".Tag";
						var parserResult = StiParser.ParseTextValue(this.Tag.Value, this, parserParameters);
						this.TagValue = Report.ToString(parserResult);
					}
				}
				if (this.Events[EventGetTag] != null && TagValue == null)
				{
					var e2 = new StiValueEventArgs();
					InvokeGetTag(this, e2);
					this.TagValue = e2.Value;
				}
				else
                {
					StiBlocklyHelper.InvokeBlockly(this.Report, this, GetTagEvent, null);
				}
				#endregion

				#region GetToolTip
				if (!isCompilationMode)
				{
					if (ToolTipValue == null && this.ToolTip.Value.Length > 0)
					{
						parserParameters.GlobalizedNameExt = ".ToolTip";
						var parserResult = StiParser.ParseTextValue(this.ToolTip.Value, this, parserParameters);
						this.ToolTipValue = Report.ToString(parserResult);
					}
				}
				if (this.Events[EventGetToolTip] != null && ToolTipValue == null)
				{
					var e3 = new StiValueEventArgs();
					InvokeGetToolTip(this, e3);
					this.ToolTipValue = e3.Value;
				}
				else
				{
					StiBlocklyHelper.InvokeBlockly(this.Report, this, GetToolTipEvent, null);
				}
				#endregion

				#region GetHyperlink
				if (!isCompilationMode)
				{
					if (HyperlinkValue == null && this.Hyperlink.Value.Length > 0)
					{
						parserParameters.GlobalizedNameExt = ".Hyperlink";
						var parserResult = StiParser.ParseTextValue(this.Hyperlink.Value, this, parserParameters);
						this.HyperlinkValue = Report.ToString(parserResult);
					}
				}
				if (this.Events[EventGetHyperlink] != null && HyperlinkValue == null)
				{
					var e4 = new StiValueEventArgs();
					InvokeGetHyperlink(this, e4);
					this.HyperlinkValue = e4.Value;
				}
				else
				{
					StiBlocklyHelper.InvokeBlockly(this.Report, this, GetHyperlinkEvent, null);
				}
				#endregion
			}
			catch (Exception e)
			{
				StiLogService.Write(this.GetType(), "DoEvents...ERROR");
				StiLogService.Write(this.GetType(), e);

                if (Report != null)
                    Report.WriteToReportRenderingMessages(this.Name + ".Events error: " + e.Message);
			}
		}

        #region GetToolTip
        internal bool IsGetToolTipHandlerEmpty => this.Events[EventGetToolTip] == null;

        private static readonly object EventGetToolTip = new object();

		/// <summary>
		/// Occurs when getting the ToolTip for the component.
		/// </summary>
		public event StiValueEventHandler GetToolTip
		{
			add
			{
				this.Events.AddHandler(EventGetToolTip, value);
			}
			remove
			{
                this.Events.RemoveHandler(EventGetToolTip, value);
			}
		}

        /// <summary>
        /// Raises the GetToolTip event.
        /// </summary>
        /// <param name="e">A parameter which contains event data.</param>
        protected virtual void OnGetToolTip(StiValueEventArgs e)
		{
		}

        /// <summary>
        /// Raises the GetToolTip event.
        /// </summary>
        /// <param name="sender">A sender which invokes an event.</param>
        /// <param name="e">A parameter which contains event data.</param>
		public void InvokeGetToolTip(object sender, StiValueEventArgs e)
		{
			try
			{
				OnGetToolTip(e);

                var handler = this.Events[EventGetToolTip] as StiValueEventHandler;
                handler?.Invoke(sender, e);

				StiBlocklyHelper.InvokeBlockly(this.Report, sender, GetToolTipEvent, e);
			}
			catch (Exception ex)
			{
				var str = $"Expression in ToolTip property of '{this.Name}' can't be evaluated!";
				StiLogService.Write(this.GetType(), str);
				StiLogService.Write(this.GetType(), ex.Message);
				Report.WriteToReportRenderingMessages(str);
			}
		}

		/// <summary>
		/// Occurs when getting the ToolTip for the component.
		/// </summary>
		[StiSerializable]
		[StiCategory("ValueEvents")]
		[Browsable(false)]
		[Description("Occurs when getting the ToolTip for the component.")]
		public virtual StiGetToolTipEvent GetToolTipEvent
		{
			get
			{				
				return new StiGetToolTipEvent(this);
			}
			set
			{
				if (value != null)
				    value.Set(this, value.Script);
			}
		}
		#endregion

		#region GetHyperlink
        internal bool IsGetHyperlinkHandlerEmpty => this.Events[EventGetHyperlink] == null;

        private static readonly object EventGetHyperlink = new object();

		/// <summary>
		/// Occurs when getting Hyperlink for the component.
		/// </summary>
		public event StiValueEventHandler GetHyperlink
		{
			add
			{
                this.Events.AddHandler(EventGetHyperlink, value);
			}
			remove
			{
                this.Events.RemoveHandler(EventGetHyperlink, value);
			}
		}

		/// <summary>
		/// Raises the GetHyperlink event.
		/// </summary>
        /// <param name="e">A parameter which contains event data.</param>
        protected virtual void OnGetHyperlink(StiValueEventArgs e)
		{
		}

        /// <summary>
        /// Raises the GetHyperlink event.
        /// </summary>
        /// <param name="sender">A sender which invokes an event.</param>
        /// <param name="e">A parameter which contains event data.</param>
		public void InvokeGetHyperlink(object sender, StiValueEventArgs e)
		{
			try
			{
				OnGetHyperlink(e);

                var handler = this.Events[EventGetHyperlink] as StiValueEventHandler;
                handler?.Invoke(sender, e);

				StiBlocklyHelper.InvokeBlockly(this.Report, sender, GetHyperlinkEvent, e);
			}
			catch (Exception ex)
			{
				var str = $"Expression in Hyperlink property of '{this.Name}' can't be evaluated!";
				StiLogService.Write(this.GetType(), str);
				StiLogService.Write(this.GetType(), ex.Message);
				Report.WriteToReportRenderingMessages(str);
			}
		}

		/// <summary>
        /// Occurs when getting Hyperlink for the component.
		/// </summary>
		[StiSerializable]
		[StiCategory("NavigationEvents")]
		[Browsable(false)]
        [Description("Occurs when getting Hyperlink for the component.")]
		public virtual StiGetHyperlinkEvent GetHyperlinkEvent
		{
			get
			{				
				return new StiGetHyperlinkEvent(this);
			}
			set
			{
				if (value != null)
				    value.Set(this, value.Script);
			}
		}
		#endregion

		#region GetTag
        internal bool IsGetTagHandlerEmpty => this.Events[EventGetTag] == null;

        private static readonly object EventGetTag = new object();

		/// <summary>
		/// Occurs when getting a Tag for a component.
		/// </summary>
		public event StiValueEventHandler GetTag
		{
			add
			{
                this.Events.AddHandler(EventGetTag, value);
			}
			remove
			{
                this.Events.RemoveHandler(EventGetTag, value);
			}
		}

		/// <summary>
		/// Raises the GetTag event.
		/// </summary>
        /// <param name="e">A parameter which contains event data.</param>
        protected virtual void OnGetTag(StiValueEventArgs e)
		{
		}

		/// <summary>
		/// Raises the GetTag event.
		/// </summary>
        /// <param name="sender">A sender which invokes an event.</param>
        /// <param name="e">A parameter which contains event data.</param>
        public void InvokeGetTag(object sender, StiValueEventArgs e)
		{
			try
			{
				OnGetTag(e);

                var handler = this.Events[EventGetTag] as StiValueEventHandler;
                handler?.Invoke(sender, e);

				StiBlocklyHelper.InvokeBlockly(this.Report, sender, GetTagEvent, e);
			}
			catch (Exception ex)
			{
				var str = $"Expression in Tag property of '{this.Name}' can't be evaluated!";
				StiLogService.Write(this.GetType(), str);
				StiLogService.Write(this.GetType(), ex.Message);
				Report.WriteToReportRenderingMessages(str);
			}
		}

		/// <summary>
		/// Occurs when getting a Tag for a component.
		/// </summary>
		[StiSerializable]
		[StiCategory("ValueEvents")]
		[Browsable(false)]
		[Description("Occurs when getting a Tag for a component.")]
		public virtual StiGetTagEvent GetTagEvent
		{
			get
			{				
				return new StiGetTagEvent(this);
			}
			set
			{
				if (value != null)value.Set(this, value.Script);
			}
		}
		#endregion

		#region GetPointer
		internal bool IsGetPointerHandlerEmpty => this.Events[EventGetPointer] == null;

		private static readonly object EventGetPointer = new object();

		/// <summary>
		/// Occurs when getting of the Pointer for the component.
		/// </summary>
		public event StiValueEventHandler GetPointer
		{
			add
			{
				this.Events.AddHandler(EventGetPointer, value);
			}
			remove
			{
				this.Events.RemoveHandler(EventGetPointer, value);
			}
		}

		/// <summary>
		/// Raises the GetPointer event.
		/// </summary>
		/// <param name="e">A parameter which contains event data.</param>
		protected virtual void OnGetPointer(StiValueEventArgs e)
		{
		}

		/// <summary>
		/// Raises the GetPointer event.
		/// </summary>
		/// <param name="sender">A sender which invokes an event.</param>
		/// <param name="e">A parameter which contains event data.</param>
		public void InvokeGetPointer(object sender, StiValueEventArgs e)
		{
			try
			{
				OnGetPointer(e);

				var handler = this.Events[EventGetPointer] as StiValueEventHandler;
				handler?.Invoke(sender, e);

				StiBlocklyHelper.InvokeBlockly(this.Report, sender, GetPointerEvent, e);
			}
			catch (Exception ex)
			{
				var str = $"Expression in Pointer property of '{Name}' can't be evaluated!";

				StiLogService.Write(GetType(), str);
				StiLogService.Write(GetType(), ex.Message);

				Report?.WriteToReportRenderingMessages(str);
			}
		}

		/// <summary>
		/// Occurs when getting of the Pointer for the component.
		/// </summary>
		[StiSerializable]
		[StiCategory("NavigationEvents")]
		[Browsable(false)]
		[Description("Occurs when getting of the Pointer for the component.")]
		public virtual StiGetPointerEvent GetPointerEvent
		{
			get
			{
				return new StiGetPointerEvent(this);
			}
			set
			{
				if (value != null) 
					value.Set(this, value.Script);
			}
		}
		#endregion

		#region GetBookmark
		internal bool IsGetBookmarkHandlerEmpty => this.Events[EventGetBookmark] == null;

        private static readonly object EventGetBookmark = new object();

		/// <summary>
		/// Occurs when getting of the Bookmark for the component.
		/// </summary>
		public event StiValueEventHandler GetBookmark
		{
			add
			{
                this.Events.AddHandler(EventGetBookmark, value);
			}
			remove
			{
                this.Events.RemoveHandler(EventGetBookmark, value);
			}
		}
		
		/// <summary>
		/// Raises the GetBookmark event.
		/// </summary>
        /// <param name="e">A parameter which contains event data.</param>
        protected virtual void OnGetBookmark(StiValueEventArgs e)
		{
		}

		/// <summary>
		/// Raises the GetBookmark event.
		/// </summary>
        /// <param name="sender">A sender which invokes an event.</param>
        /// <param name="e">A parameter which contains event data.</param>
        public void InvokeGetBookmark(object sender, StiValueEventArgs e)
		{
			try
			{
				OnGetBookmark(e);

                var handler = this.Events[EventGetBookmark] as StiValueEventHandler;
                handler?.Invoke(sender, e);

				StiBlocklyHelper.InvokeBlockly(this.Report, sender, GetBookmarkEvent, e);
			}
			catch (Exception ex)
			{
				var str = $"Expression in Bookmark property of '{this.Name}' can't be evaluated!";
				StiLogService.Write(this.GetType(), str);
				StiLogService.Write(this.GetType(), ex.Message);
				Report.WriteToReportRenderingMessages(str);
			}
		}

		/// <summary>
        /// Occurs when getting of the Bookmark for the component.
		/// </summary>
		[StiSerializable]
		[StiCategory("NavigationEvents")]
		[Browsable(false)]
		[Description("Occurs when getting of the Bookmark for the component.")]
		public virtual StiGetBookmarkEvent GetBookmarkEvent
		{
			get
			{				
				return new StiGetBookmarkEvent(this);
			}
			set
			{
				if (value != null)value.Set(this, value.Script);
			}
		}
		#endregion

		#region BeforePrint
		private static readonly object EventBeforePrint = new object();

		/// <summary>
        /// Occurs before printing of the component.
		/// </summary>
		public event EventHandler BeforePrint
		{
			add
			{
                this.Events.AddHandler(EventBeforePrint, value);
			}
			remove
			{
                this.Events.RemoveHandler(EventBeforePrint, value);
			}
		}

		/// <summary>
		/// Raises the BeforePrint event.
		/// </summary>
        /// <param name="e">A parameter which contains event data.</param>
        protected virtual void OnBeforePrint(EventArgs e)
		{
		}

		/// <summary>
		/// Raises the BeforePrint event.
		/// </summary>
        /// <param name="sender">A sender which invokes an event.</param>
        /// <param name="e">A parameter which contains event data.</param>
        public virtual void InvokeBeforePrint(object sender, EventArgs e)
		{
            try
            {
				StiAppExpressionParser.ProcessExpressions(this);

                OnBeforePrint(e);

                var isCompilationMode = true;
                if (Report != null) isCompilationMode = Report.CalculationMode == StiCalculationMode.Compilation;

                if (isCompilationMode)
                {
                    var handler = this.Events[EventBeforePrint] as EventHandler;
                    handler?.Invoke(sender, e);
                }
                else
                {
                    var handler = this.Events[EventBeforePrint] as EventHandler;
                    handler?.Invoke(sender, e);

                    if (Report != null && Report.Engine != null)
                    {
                        var obj = Report.Engine.ParserConversionStore[$"*StiConditionExpression*{Name}"];
                        if (obj != null && !(this is StiCrossCell))
                            ApplyConditions(sender, obj as ArrayList, e);
                    }

                    if (this.Interaction != null)
                    {
                        var comp = sender as StiComponent;
                        comp.DrillDownParameters = new Dictionary<string, object>();
                        try
                        {
                            if (!this.Interaction.DrillDownParameter1.IsDefault)
                                comp.DrillDownParameters.Add(this.Interaction.DrillDownParameter1.Name, StiParser.ParseTextValue("{" + this.Interaction.DrillDownParameter1.Expression.Value + "}", this, sender));

                            if (!this.Interaction.DrillDownParameter2.IsDefault)
                                comp.DrillDownParameters.Add(this.Interaction.DrillDownParameter2.Name, StiParser.ParseTextValue("{" + this.Interaction.DrillDownParameter2.Expression.Value + "}", this, sender));

                            if (!this.Interaction.DrillDownParameter3.IsDefault)
                                comp.DrillDownParameters.Add(this.Interaction.DrillDownParameter3.Name, StiParser.ParseTextValue("{" + this.Interaction.DrillDownParameter3.Expression.Value + "}", this, sender));

                            if (!this.Interaction.DrillDownParameter4.IsDefault)
                                comp.DrillDownParameters.Add(this.Interaction.DrillDownParameter4.Name, StiParser.ParseTextValue("{" + this.Interaction.DrillDownParameter4.Expression.Value + "}", this, sender));

                            if (!this.Interaction.DrillDownParameter5.IsDefault)
                                comp.DrillDownParameters.Add(this.Interaction.DrillDownParameter5.Name, StiParser.ParseTextValue("{" + this.Interaction.DrillDownParameter5.Expression.Value + "}", this, sender));

							if (!this.Interaction.DrillDownParameter6.IsDefault)
								comp.DrillDownParameters.Add(this.Interaction.DrillDownParameter6.Name, StiParser.ParseTextValue("{" + this.Interaction.DrillDownParameter6.Expression.Value + "}", this, sender));

							if (!this.Interaction.DrillDownParameter7.IsDefault)
								comp.DrillDownParameters.Add(this.Interaction.DrillDownParameter7.Name, StiParser.ParseTextValue("{" + this.Interaction.DrillDownParameter7.Expression.Value + "}", this, sender));

							if (!this.Interaction.DrillDownParameter8.IsDefault)
								comp.DrillDownParameters.Add(this.Interaction.DrillDownParameter8.Name, StiParser.ParseTextValue("{" + this.Interaction.DrillDownParameter8.Expression.Value + "}", this, sender));

							if (!this.Interaction.DrillDownParameter9.IsDefault)
								comp.DrillDownParameters.Add(this.Interaction.DrillDownParameter9.Name, StiParser.ParseTextValue("{" + this.Interaction.DrillDownParameter9.Expression.Value + "}", this, sender));

							if (!this.Interaction.DrillDownParameter10.IsDefault)
								comp.DrillDownParameters.Add(this.Interaction.DrillDownParameter10.Name, StiParser.ParseTextValue("{" + this.Interaction.DrillDownParameter10.Expression.Value + "}", this, sender));
						}
                        catch (Exception e2)
                        {
                            var str = $"Expression in DrillDown property of '{Name}' can't be evaluated!";

                            StiLogService.Write(this.GetType(), str);
                            StiLogService.Write(this.GetType(), e2.Message);

                            Report.WriteToReportRenderingMessages(str);
                        }
                    }
                }

				StiBlocklyHelper.InvokeBlockly(this.Report, sender, BeforePrintEvent);

                StiOptions.Engine.GlobalEvents.InvokeBeforePrint(sender, e);
			}
            catch (Exception ex)
            {
				string atCustomFunction = StiCustomFunctionHelper.CheckExceptionForCustomFunction(ex, this.Report, false);
				string str = $"{Name}.BeforePrint event error{atCustomFunction}: {ex.Message}";
				StiLogService.Write(this.GetType(), str);
				Report?.WriteToReportRenderingMessages(str);
			}
		}

        internal void ApplyConditions(object sender, ArrayList conditions, EventArgs e)
        {
            if (conditions == null) return;

            var parameters = new StiParserParameters() { IgnoreGlobalizedName = true };
			if (e is StiValueEventArgs)
			{
				parameters.Constants = new Hashtable();
				parameters.Constants["e.Value"] = (e as StiValueEventArgs).Value;
			}

            foreach (DictionaryEntry de in conditions)
            {
                var result = StiParser.ParseTextValue((string)de.Value, this, sender, parameters);
                if (!(result is bool) || !(bool) result) continue;
                
                var condition = de.Key as StiCondition;
                if (!string.IsNullOrEmpty(condition.Style))
                    StiConditionHelper.Apply(sender, condition.Style);

                else
                {
                    var brush = sender as IStiBrush;
                    if (brush != null && (condition.Permissions & StiConditionPermissions.BackColor) > 0)
                    {
                        brush.Brush = new StiSolidBrush(condition.BackColor);
                        if (sender is StiCrossField)
                            (sender as StiCrossField).ConditionBrush = new StiSolidBrush(condition.BackColor); 
                    }
                        

                    var border = sender as IStiBorder;
                    if (border != null && (condition.Permissions & StiConditionPermissions.Borders) > 0)
                    {
                        if ((condition.BorderSides & StiConditionBorderSides.NotAssigned) == 0)
                        {
                            border.Border = ((StiBorder) (border.Border.Clone()));
                            border.Border.Side = (StiBorderSides) condition.BorderSides;
                        }
                    }

                    var textBrush = sender as IStiTextBrush;
                    if (textBrush != null && (condition.Permissions & StiConditionPermissions.TextColor) > 0)
                    {
                        textBrush.TextBrush = new StiSolidBrush(condition.TextColor);
                        if (sender is StiCrossField)
                            (sender as StiCrossField).ConditionTextBrush = new StiSolidBrush(condition.TextColor);
                    }                        

                    var font = sender as IStiFont;
                    if (font != null)
                        StiConditionHelper.ApplyFont(sender, condition.Font, condition.Permissions);

                    if (condition.Icon != null && sender is StiText)
                    {
                        (sender as StiText).Indicator = new StiIconSetIndicator
                        {
                            CustomIcon = condition.Icon,
                            Alignment = condition.IconAlignment,
                            CustomIconSize = condition.IconSize
                        };
                    }

                    if (sender is StiCrossField)
                        (sender as StiCrossField).ConditionPermissions |= condition.Permissions;
                }

                ((StiComponent)sender).Enabled = condition.Enabled;

                if (condition.BreakIfTrue) break;
            }
        }

		/// <summary>
		/// Occurs before printing of the component.
		/// </summary>
		[StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
		[StiCategory("PrintEvents")]
		[Browsable(false)]
		[Description("Occurs before printing of the component.")]		
        public virtual StiBeforePrintEvent BeforePrintEvent
		{
			get
			{				
				return new StiBeforePrintEvent(this);
			}
			set
			{
				if (value != null)
				    value.Set(this, value.Script);
			}
		}
		#endregion

		#region AfterPrint
		private static readonly object EventAfterPrint = new object();

		/// <summary>
		/// Occurs after the component printing.
		/// </summary>
		public event EventHandler AfterPrint
		{
			add
			{
                this.Events.AddHandler(EventAfterPrint, value);
			}
			remove
			{
                this.Events.RemoveHandler(EventAfterPrint, value);
			}
		}

		/// <summary>
		/// Raises the AfterPrint event.
		/// </summary>
        /// <param name="e">A parameter which contains event data.</param>
        protected virtual void OnAfterPrint(EventArgs e)
		{
		}
		
		/// <summary>
		/// Raises the AfterPrint event.
		/// </summary>
        /// <param name="sender">A sender which invokes an event.</param>
        /// <param name="e">A parameter which contains event data.</param>
        public virtual void InvokeAfterPrint(object sender, EventArgs e)
		{
			try
			{
				OnAfterPrint(e);

				var handler = this.Events[EventAfterPrint] as EventHandler;
				handler?.Invoke(sender, e);

				StiBlocklyHelper.InvokeBlockly(this.Report, sender, AfterPrintEvent);

				StiOptions.Engine.GlobalEvents.InvokeAfterPrint(sender, e);
			}
			catch (Exception ex)
			{
				string atCustomFunction = StiCustomFunctionHelper.CheckExceptionForCustomFunction(ex, this.Report, false);
				string str = $"{Name}.AfterPrint event error{atCustomFunction}: {ex.Message}";
				StiLogService.Write(this.GetType(), str);
				Report?.WriteToReportRenderingMessages(str);
			}
		}

		/// <summary>
		/// Occurs after the component printing.
		/// </summary>
		[StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
		[StiCategory("PrintEvents")]
		[Browsable(false)]
		[Description("Occurs after the component printing.")]
		public virtual StiAfterPrintEvent AfterPrintEvent
		{
			get
			{				
				return new StiAfterPrintEvent(this);
			}
			set
			{
				if (value != null)
				    value.Set(this, value.Script);
			}
		}
		#endregion

		#region GetDrillDownReport
		private static readonly object EventGetDrillDownReport = new object();

		/// <summary>
		/// Occurs when it is required to get a report for the Drill-Down operation.
		/// </summary>
		public event StiGetDrillDownReportEventHandler GetDrillDownReport
		{
			add
			{
                this.Events.AddHandler(EventGetDrillDownReport, value);
			}
			remove
			{
                this.Events.RemoveHandler(EventGetDrillDownReport, value);
			}
		}

		/// <summary>
		/// Raises the GetDrillDownReport event for this report.
		/// </summary>
		/// <param name="e">A parameter which contains event data.</param>
		protected virtual void OnGetDrillDownReport(StiGetDrillDownReportEventArgs e)
		{
		}

		/// <summary>
		/// Raises the GetDrillDownReport event for this report.
		/// </summary>
		/// <param name="sender">A sender which invokes an event.</param>
		/// <param name="e">A parameter which contains event data.</param>
		public virtual void InvokeGetDrillDownReport(object sender, StiGetDrillDownReportEventArgs e)
		{
			OnGetDrillDownReport(e);

            var handler = this.Events[EventGetDrillDownReport] as StiGetDrillDownReportEventHandler;
            handler?.Invoke(sender, e);

			StiBlocklyHelper.InvokeBlockly(this.Report, sender, GetDrillDownReportEvent);
		}

		/// <summary>
		/// Occurs when it is required to get a report for the Drill-Down operation.
		/// </summary>
		[StiSerializable]
		[StiCategory("NavigationEvents")]
		[Browsable(false)]
		[Description("Occurs when it is required to get a report for the Drill-Down operation.")]
		public virtual StiGetDrillDownReportEvent GetDrillDownReportEvent
		{
			get
			{
				return new StiGetDrillDownReportEvent(this);
			}
			set
			{
				if (value != null)
				    value.Set(this, value.Script);
			}
		}
		#endregion

		#region Click
        internal bool IsClickHandlerEmpty => this.Events[EventClick] == null;

        private static readonly object EventClick = new object();

		/// <summary>
		/// Occurs when user clicks on the component in the window of viewer.
		/// </summary>
		public event EventHandler Click
		{
			add
			{
                this.Events.AddHandler(EventClick, value);
			}
			remove
			{
                this.Events.RemoveHandler(EventClick, value);
			}
		}

		/// <summary>
		/// Raises the Click event for this report.
		/// </summary>
        /// <param name="e">A parameter which contains event data.</param>
        protected virtual void OnClick(EventArgs e)
		{
		}
		
		/// <summary>
		/// Raises the Click event for this report.
		/// </summary>
        /// <param name="sender">A sender which invokes an event.</param>
        /// <param name="e">A parameter which contains event data.</param>
        public virtual void InvokeClick(object sender, EventArgs e)
		{
			OnClick(e);

            var handler = this.Events[EventClick] as EventHandler;
            handler?.Invoke(sender, e);

			StiBlocklyHelper.InvokeBlockly(this.Report, sender, ClickEvent);
		}

		/// <summary>
        /// Occurs when user clicks on the component in the window of viewer.
		/// </summary>
		[StiSerializable]
		[StiCategory("MouseEvents")]
		[Browsable(false)]
		[Description("Occurs when user clicks on the component in the window of viewer.")]
		public virtual StiClickEvent ClickEvent
		{
			get
			{				
				return new StiClickEvent(this);
			}
			set
			{
				if (value != null)
				    value.Set(this, value.Script);
			}
		}
		#endregion

		#region DoubleClick
        internal bool IsDoubleClickHandlerEmpty => this.Events[EventDoubleClick] == null;

        private static readonly object EventDoubleClick = new object();

		/// <summary>
		/// Occurs when user double clicks on the component in the window of viewer.
		/// </summary>
		public event EventHandler DoubleClick
		{
			add
			{
                this.Events.AddHandler(EventDoubleClick, value);
			}
			remove
			{
                this.Events.RemoveHandler(EventDoubleClick, value);
			}
		}

		/// <summary>
		/// Raises the DoubleClick event for this report.
		/// </summary>
		/// <param name="e">A parameter which contains event data.</param>
		protected virtual void OnDoubleClick(EventArgs e)
		{
		}
		
		/// <summary>
		/// Raises the DoubleClick event for this report.
		/// </summary>
		/// <param name="sender">A sender which invokes an event.</param>
		/// <param name="e">A parameter which contains event data.</param>
		public virtual void InvokeDoubleClick(object sender, EventArgs e)
		{
			OnDoubleClick(e);

            var handler = this.Events[EventDoubleClick] as EventHandler;
            handler?.Invoke(sender, e);

			StiBlocklyHelper.InvokeBlockly(this.Report, sender, DoubleClickEvent);
		}

		/// <summary>
		/// Occurs when user double clicks on the component in the window of viewer.
		/// </summary>
		[StiSerializable]
		[StiCategory("MouseEvents")]
		[Browsable(false)]
		[Description("Occurs when user double clicks on the component in the window of viewer.")]
		public virtual StiDoubleClickEvent DoubleClickEvent
		{
			get
			{				
				return new StiDoubleClickEvent(this);
			}
			set
			{
				if (value != null)
				    value.Set(this, value.Script);
			}
		}
		#endregion

		#region MouseEnter
        internal bool IsMouseEnterHandlerEmpty => this.Events[EventMouseEnter] == null;

        private static readonly object EventMouseEnter = new object();

		/// <summary>
		/// Occurs when user enters the mouse into the area of the component in the window of viewer.
		/// </summary>
		public event EventHandler MouseEnter
		{
			add
			{
                this.Events.AddHandler(EventMouseEnter, value);
			}
			remove
			{
                this.Events.RemoveHandler(EventMouseEnter, value);
			}
		}

		/// <summary>
		/// Raises the MouseEnter event for this report.
		/// </summary>
        /// <param name="e">A parameter which contains event data.</param>
        protected virtual void OnMouseEnter(EventArgs e)
		{
		}

		/// <summary>
		/// Raises the MouseEnter event for this report.
		/// </summary>
        /// <param name="sender">A sender which invokes an event.</param>
        /// <param name="e">A parameter which contains event data.</param>
        public virtual void InvokeMouseEnter(object sender, EventArgs e)
		{
			OnMouseEnter(e);

            var handler = this.Events[EventMouseEnter] as EventHandler;
            handler?.Invoke(sender, e);

			StiBlocklyHelper.InvokeBlockly(this.Report, sender, MouseEnterEvent);
		}

		/// <summary>
		/// Occurs when user enters the mouse into the area of the component in the window of viewer.
		/// </summary>
		[StiSerializable]
		[StiCategory("MouseEvents")]
		[Browsable(false)]
		[Description("Occurs when user enters the mouse into the area of the component in the window of viewer.")]
		public virtual StiMouseEnterEvent MouseEnterEvent
		{
			get
			{				
				return new StiMouseEnterEvent(this);
			}
			set
			{
				if (value != null)
				    value.Set(this, value.Script);
			}
		}
		#endregion

		#region MouseLeave
        internal bool IsMouseLeaveHandlerEmpty
        {
            get
            {
                return this.Events[EventMouseLeave] == null;
            }
        }

		private static readonly object EventMouseLeave = new object();

		/// <summary>
		/// Occurs when user leaves the mouse out of the area of the component in the window of viewer.
		/// </summary>
		public event EventHandler MouseLeave
		{
			add
			{
                this.Events.AddHandler(EventMouseLeave, value);
			}
			remove
			{
                this.Events.RemoveHandler(EventMouseLeave, value);
			}
		}


		/// <summary>
		/// Raises the MouseLeave event for this report.
		/// </summary>
        /// <param name="e">A parameter which contains event data.</param>
        protected virtual void OnMouseLeave(EventArgs e)
		{
		
		}


		/// <summary>
		/// Raises the MouseLeave event for this report.
		/// </summary>
        /// <param name="sender">A sender which invokes an event.</param>
        /// <param name="e">A parameter which contains event data.</param>
        public virtual void InvokeMouseLeave(object sender, EventArgs e)
		{
			OnMouseLeave(e);
            var handler = this.Events[EventMouseLeave] as EventHandler;
			if (handler != null)handler(sender, e);

			StiBlocklyHelper.InvokeBlockly(this.Report, sender, MouseLeaveEvent);
		}
		

		/// <summary>
		/// Occurs when user leaves the mouse out of the area of the component in the window of viewer.
		/// </summary>
		[StiSerializable]
		[StiCategory("MouseEvents")]
		[Browsable(false)]
		[Description("Occurs when user leaves the mouse out of the area of the component in the window of viewer.")]
		public virtual StiMouseLeaveEvent MouseLeaveEvent
		{
			get
			{				
				return new StiMouseLeaveEvent(this);
			}
			set
			{
				if (value != null)value.Set(this, value.Script);
			}
		}
		#endregion

		#region Painting
		private static readonly object EventPainting = new object();
		/// <summary>
		/// Occurs before the component painting.
		/// </summary>
		public event StiPaintEventHandler Painting
		{
			add
			{
                this.Events.AddHandler(EventPainting, value);
			}
			remove
			{
                this.Events.RemoveHandler(EventPainting, value);
			}
		}

		/// <summary>
		/// Raises the Painting event for this component.
		/// </summary>
        /// <param name="e">A parameter which contains event data.</param>
        protected virtual void OnPainting(StiPaintEventArgs e)
		{
		}

		
		/// <summary>
		/// Raises the Painting event for this component.
		/// </summary>
        /// <param name="sender">A sender which invokes an event.</param>
        /// <param name="e">A parameter which contains event data.</param>
        public void InvokePainting(StiComponent sender, StiPaintEventArgs e)
		{
			OnPainting(e);
            var handler = this.Events[EventPainting] as StiPaintEventHandler;
			if (handler != null)handler(sender, e);
		}
		#endregion

		#region Painted
		private static readonly object EventPainted = new object();
		/// <summary>
		/// Occurs after the component was painted.
		/// </summary>
		public event StiPaintEventHandler Painted
		{
			add
			{
                this.Events.AddHandler(EventPainted, value);
			}
			remove
			{
                this.Events.RemoveHandler(EventPainted, value);
			}
		}

        /// <summary>
        /// Raises the Painted event for this component.
        /// </summary>
        /// <param name="e">A parameter which contains event data.</param>
        protected virtual void OnPainted(StiPaintEventArgs e)
		{
		}

		
		/// <summary>
		/// Raises the Painted event for this component.
		/// </summary>
		/// <param name="sender">A sender which invokes an event.</param>
        /// <param name="e">A parameter which contains event data.</param>
		public void InvokePainted(StiComponent sender, StiPaintEventArgs e)
		{
			OnPainted(e);
            var handler = this.Events[EventPainted] as StiPaintEventHandler;
			if (handler != null)handler(sender, e);
		}
		#endregion
		#endregion

		#region Expressions
		#region Pointer
		/// <summary>
		/// Gets or sets the component pointer in the table of contents.
		/// </summary>
		[Browsable(false)]
		[StiSerializable(StiSerializeTypes.SerializeToDocument)]
		[Description("Gets or sets the component pointer in the table of contents.")]
		public object PointerValue { get; set; }

		/// <summary>
		/// Gets or sets the expression to fill a component pointer in the table of contents.
		/// </summary>
		[Browsable(false)]
		[StiSerializable(
			 StiSerializeTypes.SerializeToCode |
			 StiSerializeTypes.SerializeToDesigner |
			 StiSerializeTypes.SerializeToSaveLoad)]
		[Description("Gets or sets the expression to fill a component pointer in the table of contents.")]
		public virtual StiPointerExpression Pointer
		{
			get
			{
				return new StiPointerExpression(this, "Pointer");
			}
			set
			{
				if (value != null)
					value.Set(this, "Pointer", value.Value);
			}
		}
		#endregion

		#region Bookmark
		/// <summary>
		/// Gets or sets the component bookmark.
		/// </summary>
		[Browsable(false)]
		[StiSerializable(StiSerializeTypes.SerializeToDocument)]
		[Description("Gets or sets the component bookmark.")]			
		public object BookmarkValue
		{
			get
			{
			    return bits?.bookmarkValue;
			}
			set
			{
                if (value == null && bits == null)
                    return;

                if (bits != null)
                    bits.bookmarkValue = value;

                else
                    bits = new bitsComponent(value, this.ToolTipValue, this.HyperlinkValue, this.TagValue, 
                        this.Enabled, this.HighlightState, this.IgnoreNamingRule,
                        this.DockStyle, this.Printable);
			}
		}


		/// <summary>
		/// Gets or sets the expression to fill a component bookmark.
		/// </summary>
		[StiCategory("Navigation")]
		[Browsable(false)]
		[StiOrder(StiPropertyOrder.NavigationBookmark)]
		[StiSerializable(
			 StiSerializeTypes.SerializeToCode |
			 StiSerializeTypes.SerializeToDesigner |
			 StiSerializeTypes.SerializeToSaveLoad)]
		[Description("Gets or sets the expression to fill a component bookmark.")]
		public virtual StiBookmarkExpression Bookmark
		{
			get
			{
				return new StiBookmarkExpression(this, "Bookmark");
			}
			set
			{
				if (value != null)
				    value.Set(this, "Bookmark", value.Value);
			}
		}
		#endregion

		#region ToolTip
		/// <summary>
		/// Gets or sets a component tips.
		/// </summary>
		[Browsable(false)]
		[StiSerializable(StiSerializeTypes.SerializeToDocument)]		
		[Description("Gets or sets a component tips.")]
		public object ToolTipValue
		{
			get
			{
			    return bits?.toolTipValue;
			}
			set
			{
                if (value == null && bits == null)
                    return;

                if (bits != null)
                    bits.toolTipValue = value;

                else
                    bits = new bitsComponent(this.BookmarkValue, value, this.HyperlinkValue, this.TagValue,
                        this.Enabled, this.HighlightState, this.IgnoreNamingRule,
                        this.DockStyle, this.Printable);
			}
		}

		/// <summary>
		/// Gets or sets the expression to fill a component tooltip.
		/// </summary>
		[Browsable(false)]
		[StiCategory("Navigation")]
		[StiOrder(StiPropertyOrder.NavigationToolTip)]
		[StiSerializable(
			 StiSerializeTypes.SerializeToCode |
			 StiSerializeTypes.SerializeToDesigner |
			 StiSerializeTypes.SerializeToSaveLoad)]
		[Description("Gets or sets the expression to fill a component tooltip.")]
		public virtual StiToolTipExpression ToolTip
		{
			get
			{
				return new StiToolTipExpression(this, "ToolTip");
			}
			set
			{
				if (value != null)
				    value.Set(this, "ToolTip", value.Value);
			}
		}
		#endregion

		#region Hyperlink
		/// <summary>
		/// Gets or sets hyperlink of a component.
		/// </summary>
		[Browsable(false)]
		[StiSerializable(StiSerializeTypes.SerializeToDocument)]		
		[Description("Gets or sets hyperlink of the component.")]
		public object HyperlinkValue
		{
			get
			{
			    return bits?.hyperlinkValue;
			}
			set
			{
                if (value == null && bits == null)
                    return;

                if (bits != null)
                    bits.hyperlinkValue = value;

                else
                    bits = new bitsComponent(this.BookmarkValue, this.ToolTipValue, value, this.TagValue,
                        this.Enabled, this.HighlightState, this.IgnoreNamingRule,
                        this.DockStyle, this.Printable);
			}
		}

		/// <summary>
		/// Gets or sets an expression to fill a component hyperlink.
		/// </summary>
		[StiCategory("Navigation")]
		[Browsable(false)]
		[StiOrder(StiPropertyOrder.NavigationHyperlink)]
		[StiSerializable(
			 StiSerializeTypes.SerializeToCode |
			 StiSerializeTypes.SerializeToDesigner |
			 StiSerializeTypes.SerializeToSaveLoad)]
		[Description("Gets or sets an expression to fill the component hyperlink.")]
		public virtual StiHyperlinkExpression Hyperlink
		{
			get
			{
				return new StiHyperlinkExpression(this, "Hyperlink");
			}
			set
			{
				if (value != null)
				    value.Set(this, "Hyperlink", value.Value);
			}
		}
		#endregion

		#region Tag
		/// <summary>
		/// Gets or sets tag of a component.
		/// </summary>
		[Browsable(false)]
		[StiSerializable(StiSerializeTypes.SerializeToDocument)]		
		[Description("Gets or sets tag of a component.")]
		public object TagValue
		{
			get
			{
			    return bits?.tagValue;
			}
			set
			{
                if (value == null && bits == null)
                    return;

                if (bits != null)
                    bits.tagValue = value;

                else
                    bits = new bitsComponent(this.BookmarkValue, this.ToolTipValue, this.HyperlinkValue, value,
                        this.Enabled, this.HighlightState, this.IgnoreNamingRule,
                        this.DockStyle, this.Printable);
			}
		}

		/// <summary>
		/// Gets or sets the expression to fill a component tag.
		/// </summary>
		[StiCategory("Navigation")]
		[Browsable(false)]
		[StiOrder(StiPropertyOrder.NavigationTag)]
		[StiSerializable(
			 StiSerializeTypes.SerializeToCode |
			 StiSerializeTypes.SerializeToDesigner |
			 StiSerializeTypes.SerializeToSaveLoad)]
		[Description("Gets or sets the expression to fill a component tag.")]
		public virtual StiTagExpression Tag
		{
			get
			{
				return new StiTagExpression(this, "Tag");
			}
			set
			{
				if (value != null)
				    value.Set(this, "Tag", value.Value);
			}
		}
        #endregion
        #endregion

        #region Properties
        private string alias = "";
		/// <summary>
		/// Indicates the text that will be shown instead of the component name. 
		/// If the text is not indicated, then the component name is shown.
		/// </summary>
		[StiCategory("Design")]
        [StiOrder(StiPropertyOrder.DesignAlias)]
        [StiSerializable]
        [ParenthesizePropertyName(true)]
        [DefaultValue("")]
        [Description("Indicates the text that will be shown instead of the component name. " +
			"If the text is not indicated, then the component name is shown.")]
        [Editor("Stimulsoft.Report.Components.Design.StiSimpleTextEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual string Alias
        {
            get
            {
                return alias;
            }
            set
            {
                if (alias != value)
                    alias = string.Intern(value);
            }
        }

        private EventHandlerList events;
        protected EventHandlerList Events
        {
            get
            {
                return this.events ?? (this.events = new EventHandlerList());
            }
        }

        protected static object PropertyRestrictions = new object();
        /// <summary>
        /// Gets or sets value which indicates the restrictions of a component.
        /// </summary>
        [StiCategory("Design")]
        [StiOrder(StiPropertyOrder.DesignRestrictions)]
        [StiSerializable]
        [DefaultValue(StiRestrictions.All)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor("Stimulsoft.Report.Components.Design.StiRestrictionsModeEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates the restrictions of a component.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual StiRestrictions Restrictions
        {
            get
            {
                return (StiRestrictions)Properties.Get(PropertyRestrictions, StiRestrictions.All);
            }
            set
            {
                Properties.Set(PropertyRestrictions, value, StiRestrictions.All);
            }
        }

		/// <summary>
		/// Internal use only.
		/// </summary>
		[Browsable(false)]
		public bool IgnoreNamingRule
		{
            get
            {
                return bits != null && bits.ignoreNamingRule;
            }
            set
            {
                if (value == false && bits == null)
                    return;

                if (bits != null)
                    bits.ignoreNamingRule = value;

                else
                    bits = new bitsComponent(this.BookmarkValue, this.ToolTipValue, this.HyperlinkValue, this.TagValue,
                        this.Enabled, this.HighlightState, value,
                        this.DockStyle, this.Printable);
            }
		}
		
        /// <summary>
        /// Gets or sets a name of a component.
        /// </summary>
		public override string Name
		{
			get 
			{
				return base.Name;
			}
			set 
			{
                if (IgnoreNamingRule || Report == null || (!Report.IsDesigning))
                    base.Name = value;

                else
                {
                    if (string.IsNullOrWhiteSpace(value))
                        throw new ArgumentException();

                    if (!StiOptions.Designer.AutoCorrectComponentName)
                        base.Name = value;

                    else
                        base.Name = StiNameValidator.CorrectName(value, Report);
                }
			}
		}
		
		/// <summary>
		/// Gets a value which indicates that all events are empty.
		/// </summary>
		[Browsable(false)]
		public bool IsEventEmpty
		{
			get
			{
				var events = GetEvents();
				
				foreach (StiEvent ev in events)
				{
					if (ev.Script.Length == 0)continue;
					return false;
				}

				return true;
			}
		}

        protected static object PropertyShowQuickButtons = new object();
		/// <summary>
		/// Gets or sets a value which indicates whether it is necessary to show quick buttons.
		/// </summary>
		[Browsable(false)]
		public bool ShowQuickButtons
		{
			get 
			{
                return Properties.GetBool(PropertyShowQuickButtons, false);
			}
			set 
			{
                Properties.SetBool(PropertyShowQuickButtons, value, false);
			}
		}

        protected static object PropertyPlaceOnToolbox = new object();
		/// <summary>
		/// Gets or sets a value which indicates whether it is necessary to place component in toolbox or not.
		/// </summary>
		[Browsable(false)]
		[StiServiceParam]
		[StiCategory("Parameters")]
		[Description("Gets or sets value which shows it is necessary to place component in toolbox or no.")]
		public virtual bool PlaceOnToolbox
		{
			get
			{
                return Properties.GetBool(PropertyPlaceOnToolbox, false);
			}
			set
			{
                Properties.SetBool(PropertyPlaceOnToolbox, value, false); 
			}
		}

		/// <summary>
		/// Gets a value to sort a position in the toolbox.
		/// </summary>
		[Browsable(false)]
		public abstract int ToolboxPosition { get; }

        [Browsable(false)]
        public virtual StiToolboxCategory ToolboxCategory => StiToolboxCategory.Components;

        /// <summary>
		/// Gets a value which indicates whether it is necessary to draw again the whole page when moving the component or
		/// changing its sizes in the designer.
		/// </summary>
		[Browsable(false)]
		public virtual bool ForceRedrawAll => false;

        /// <summary>
		/// Gets a value which indicates that the component has already been printed.
		/// </summary>
		[Browsable(false)]
		public bool IsPrinting => Report != null && (Report.IsPrinting || (Report.CompiledReport != null && Report.CompiledReport.IsPrinting));

        /// <summary>
		/// Gets or sets value, which indicates that the report is exporting.
		/// </summary>
		[Browsable(false)]
		public bool IsExporting => Report != null && Report.IsExporting;

        /// <summary>
		/// Gets a value which indicates that the report in which a component is placed is being designed.
		/// </summary>
		[Browsable(false)]
		public bool IsDesigning => Report != null && Report.IsDesigning;

        /// <summary>
        /// Gets a value indicating that this is the Cross component.
        /// </summary>
        [Browsable(false)]
        public virtual bool IsCross => false;

        /// <summary>
        /// Gets a value which indicates whether deleting of the components is allowed.
        /// </summary>
		[Browsable(false)]
		public virtual bool AllowDelete => !Inherited && StiRestrictionsHelper.IsAllowDelete(this);

        protected static object PropertyDelimiterComponent = new object();
		/// <summary>
		/// Gets or sets a value indicates that this component is used as the delimiter.
		/// </summary>
		[Browsable(false)]
		public bool DelimiterComponent
		{
			get
			{
                return Properties.GetBool(PropertyDelimiterComponent, false);
			}
			set
			{
                Properties.SetBool(PropertyDelimiterComponent, value, false); 
			}
		}

		/// <summary>
		/// Gets a component priority.
		/// </summary>
		[Browsable(false)]
		public virtual int Priority => (int)StiComponentPriority.Component;

        /// <summary>
		/// Gets the type of processing when printing.
		/// </summary>
		[Browsable(false)]
		public virtual StiComponentType ComponentType => StiComponentType.Simple;

        protected static object PropertyDockable = new object();
		/// <summary>
		/// Gets or sets a value which indicates whether the component will be docked or not.
		/// </summary>
		[Browsable(false)]
		public virtual bool Dockable
		{
			get
			{
                return Properties.GetBool(PropertyDockable, true);
			}
			set
			{
                Properties.SetBool(PropertyDockable, value, true); 
			}
		}

		/// <summary>
		/// Gets or sets a state of highlight.
		/// </summary>
		[Browsable(false)]
		public StiHighlightState HighlightState
		{
            get
            {
                return bits == null ? StiHighlightState.Hide : bits.highlightState;
            }
            set
            {
                if (value == StiHighlightState.Hide && bits == null)
                    return;

                if (bits != null)
                    bits.highlightState = value;
                else
                    bits = new bitsComponent(this.BookmarkValue, this.ToolTipValue, this.HyperlinkValue, this.TagValue,
                        this.Enabled, value, this.IgnoreNamingRule,
                        this.DockStyle, this.Printable);
            }
		}
        
        /// <summary>
        /// Gets or sets a value which indicates placement of a simple component. 
        /// Available values: 
        /// "rt" - StiReportTitleBand
        /// "rs" - StiReportSummaryBand
        /// "ph" - StiPageHeaderBand
        /// "pf" - StiPageFooterBand
        /// "h" - StiHeaderBand
        /// "h.ap" - StiHeaderBand (OnAllPages)
        /// "f" - StiFooterBand
        /// "f.ap" - StiFooterBand (OnAllPages)
        /// "d" - StiDataBand
        /// "gh" - StiGroupHeaderBand
        /// "gf" - StiGroupFooterBand
        /// "e" - StiEmptyBand
        /// "p" - StiPage
        /// </summary>
        [StiSerializable]
        [DefaultValue("")]
        [Browsable(false)]
        public string ComponentPlacement { get; set; } = string.Empty;

        [Browsable(false)]
		public Dictionary<string, object> DrillDownParameters { get; set; }
        
        protected static object PropertyComponentStyle = new object();
		/// <summary>
		/// Gets or sets a style of a component.
		/// </summary>
		[StiSerializable]
		[DefaultValue("")]
		[StiCategory("Appearance")]
		[StiOrder(StiPropertyOrder.AppearanceComponentStyle)]
        [Description("Gets or sets a style of a component.")]		
		[StiPropertyLevel(StiLevel.Basic)]
		[Editor("Stimulsoft.Report.Design.StiStyleEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiExpressionStyleConverter))]
		[StiExpressionAllowed]
		[RefreshProperties(RefreshProperties.All)]
		public virtual string ComponentStyle
		{
			get
			{
                return (string)Properties.Get(PropertyComponentStyle, string.Empty);
			}
            set
            {
                Properties.Set(PropertyComponentStyle, value, string.Empty);

                if (this.Report != null && value != null)
                {
                    var style = this.Report.Styles[value];
                    if (style != null)
                        style.SetStyleToComponent(this);
                }

                if (this is StiContainer)
                    ((StiContainer)this).SetParentStylesToChilds();
            }
		}

        protected static object PropertyLocked = new object();
		/// <summary>
		/// Gets or sets a value which indicates that moving is locked.
		/// </summary>
		[StiCategory("Design")]
		[StiOrder(StiPropertyOrder.DesignLocked)]
		[StiSerializable]
		[DefaultValue(false)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets a value which indicates that moving is locked.")]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual bool Locked
		{
			get
			{
                return Properties.GetBool(PropertyLocked, false);
			}
			set
			{
                Properties.SetBool(PropertyLocked, value, false); 
			}
		}

        protected static object PropertyLinked = new object();
		/// <summary>
		/// Gets or sets a value which indicates whether the object snap to the container is turned on.
		/// </summary>
		[StiCategory("Design")]
		[StiOrder(StiPropertyOrder.DesignLinked)]
		[StiSerializable]
		[DefaultValue(false)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets value, indicates that the object snap to the container is turned on.")]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual bool Linked
		{
			get
			{
                return Properties.GetBool(PropertyLinked, false);
			}
			set
			{
                Properties.SetBool(PropertyLinked, value, false); 
			}
		}

		/// <summary>
		/// Indicates that this component will be available or not.
		/// </summary>
		[StiSerializable]
		[DefaultValue(true)]
		[StiCategory("Behavior")]
		[StiOrder(StiPropertyOrder.BehaviorEnabled)]
		[TypeConverter(typeof(StiExpressionBoolConverter))]
		[Editor(StiEditors.ExpressionBool, typeof(UITypeEditor))]
		[Description("Indicates that this component will be available or not.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Basic)]
		[StiExpressionAllowed]
		public virtual bool Enabled
		{
            get
            {
                return bits == null || bits.enabled;
            }
			set
			{
                if (value && bits == null)
                    return;

                if (bits != null)
                    bits.enabled = value;

                else
                    bits = new bitsComponent(this.BookmarkValue, this.ToolTipValue, this.HyperlinkValue, this.TagValue,
                        value, this.HighlightState, this.IgnoreNamingRule,
                        this.DockStyle, this.Printable);

                if ((!IsDesigning) && (!value) && DockStyle != StiDockStyle.None && Parent != null)
                {
                    if (!(this is StiBand && Report != null && Report.EngineVersion == StiEngineVersion.EngineV2) ||
                        StiOptions.Engine.CheckDockToContainerIfComponentDisabled)
                    {
                        Parent.DockToContainer();
                    }
                }
			}
		}

        protected static object PropertyUseParentStyles = new object();
		/// <summary>
		/// Gets or sets a value which indicates that this component must use styles from parent component.
		/// </summary>
		[StiSerializable]
		[DefaultValue(false)]
		[StiCategory("Appearance")]
		[StiOrder(StiPropertyOrder.AppearanceUseParentStyles)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets a value which indicates that this component must use styles from parent component.")]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual bool UseParentStyles
		{
			get
			{
                return Properties.GetBool(PropertyUseParentStyles, false);
			}
			set
			{
			    if (UseParentStyles != value)
                {
                    Properties.SetBool(PropertyUseParentStyles, value, false);

                    if (value && Parent != null)
                        Parent.SetParentStylesToChilds();
                }
			}
		}

        /// <summary>
		/// Gets or sets the page on which an object is located.
		/// </summary>
		[Browsable(false)]
		[StiSerializable(StiSerializationVisibility.Class, 
			 StiSerializeTypes.SerializeToCode | 
			 StiSerializeTypes.SerializeToSaveLoad)]
		[DefaultValue(null)]
		[Description("Gets or sets the page on which an object is located.")]		
		public virtual StiPage Page { get; set; }

        /// <summary>
		/// Gets or sets the container in which an object is located.
		/// </summary>
		[Browsable(false)]
		[StiSerializable(StiSerializationVisibility.Class, 
			 StiSerializeTypes.SerializeToCode | 
			 StiSerializeTypes.SerializeToSaveLoad)]
		[DefaultValue(null)]
		[Description("Gets or sets the container in which an object is located.")]
		public virtual StiContainer Parent { get; set; }		
		#endregion

		#region Methods
		/// <summary>
		/// Internal use only, for LoadDocument optimization.
		/// </summary>
		/// <returns>Returns true if Properties property is initialized.</returns>
		internal bool IsPropertiesInitialized()
		{
			return IsPropertiesInitializedProtected();
		}

		[Browsable(false)]
		public virtual bool IsExportAsImage(StiExportFormat format)
		{
			var ibrush = this as IStiBrush;
			var itextBrush = this as IStiTextBrush;

            if (ibrush != null)
            {
                var brush = ibrush.Brush;
                if (format == StiExportFormat.Pdf &&
                    (brush is StiGradientBrush ||
                    brush is StiGlareBrush ||
                    brush is StiHatchBrush ||
                    brush is StiGlassBrush)) return false;

                if (format == StiExportFormat.Xps &&
                    brush is StiGradientBrush &&
                    ((itextBrush == null) || (!(itextBrush.TextBrush is StiGradientBrush)))) return false;

                if ((format == StiExportFormat.Html || format == StiExportFormat.HtmlDiv || format == StiExportFormat.HtmlSpan || format == StiExportFormat.HtmlTable))
                {
                    if (brush is StiHatchBrush) return true;
                }
                else if (!(format == StiExportFormat.ImageSvg || format == StiExportFormat.ImageSvgz))
                {
                    if (brush is StiGlareBrush ||
                        brush is StiGradientBrush || 
                        brush is StiGlassBrush ||
                        brush is StiHatchBrush) return true;
                }
			}

			if (itextBrush != null)
			{
                var textBrush = itextBrush.TextBrush;
                if (format == StiExportFormat.Pdf && 
					(textBrush is StiGradientBrush ||
                    textBrush is StiGlareBrush ||
                    textBrush is StiHatchBrush)) return false;

				if (textBrush is StiGradientBrush || 
					textBrush is StiGlareBrush ||
                    textBrush is StiGlassBrush || 
					textBrush is StiHatchBrush)return true;
			}
	
			return false;
		}

		public virtual void OnRemoveComponent()
		{
		}        

		private bool lockOnResize = true;

        private void InvokeOnResizeComponent(SizeD oldSize, SizeD newSize)
        {
            if (!lockOnResize) 
				OnResizeComponent(oldSize, newSize);
        }

        public virtual void OnResizeComponent(SizeD oldSize, SizeD newSize)
        {
            var container = this as StiContainer;
            if (container == null) return;

            var distWidth = (decimal)newSize.Width - (decimal)oldSize.Width;
            var distHeight = (decimal)newSize.Height - (decimal)oldSize.Height;
            
            foreach (StiComponent component in container.Components)
            {
                if (distWidth != 0)
                {
                    if ((component.Anchor & StiAnchorMode.Left) > 0 && (component.Anchor & StiAnchorMode.Right) > 0)
                    {
                        var oldValue = component.width;
                        component.width = (double)((decimal)oldValue + distWidth);
                        component.InvokeOnResizeComponent(new SizeD(oldValue, component.height), new SizeD(component.width, component.height));
                    }

                    if ((component.Anchor & StiAnchorMode.Left) == 0 && (component.Anchor & StiAnchorMode.Right) > 0)
                        component.left = (double)((decimal)component.left + distWidth);
                }

                if (distHeight != 0)
                {
                    if ((component.Anchor & StiAnchorMode.Top) > 0 && (component.Anchor & StiAnchorMode.Bottom) > 0)
                    {
                        var oldValue = component.height;
                        component.height = (double)((decimal)oldValue + distHeight);
                        component.InvokeOnResizeComponent(new SizeD(component.width, oldValue), new SizeD(component.width, component.height));
                    }

                    if ((component.Anchor & StiAnchorMode.Top) == 0 && (component.Anchor & StiAnchorMode.Bottom) > 0)
                        component.top = (double)((decimal)component.top + distHeight);
                }
            }
        }

		public Graphics GetMeasureGraphics()
		{
            if (Report == null)
                return StiReport.GlobalMeasureGraphics;
            else
                return Report.ReportMeasureGraphics;
		}

		/// <summary>
		/// Returns events collection of this component.
		/// </summary>
		public virtual StiEventsCollection GetEvents()
		{
			var events = new StiEventsCollection();

			if (GetToolTipEvent != null)
                events.Add(GetToolTipEvent);

			if (GetHyperlinkEvent != null)
                events.Add(GetHyperlinkEvent);

			if (GetTagEvent != null)
                events.Add(GetTagEvent);

			if (GetPointerEvent != null)
				events.Add(GetPointerEvent);

			if (GetBookmarkEvent != null)
                events.Add(GetBookmarkEvent);

			if (BeforePrintEvent != null)
                events.Add(BeforePrintEvent);

			if (AfterPrintEvent != null)
                events.Add(AfterPrintEvent);

			if (ClickEvent != null)
                events.Add(ClickEvent);

			if (DoubleClickEvent != null)
				events.Add(DoubleClickEvent);

			if (MouseEnterEvent != null)
                events.Add(MouseEnterEvent);

			if (MouseLeaveEvent != null)
                events.Add(MouseLeaveEvent);

			if (GetDrillDownReportEvent != null)
				events.Add(GetDrillDownReportEvent);

			return events;
		}

		protected void CopyEventHandlersToComponent(StiComponent component, bool onlyBeforePrint, StiComponent from = null)
        {
			if (from == null) from = this;
			if (onlyBeforePrint)
			{
				var handler = from.Events[EventBeforePrint] as EventHandler;
				if (handler != null) 
					component.Events.AddHandler(EventBeforePrint, handler);
			}
			else
			{
				component.Events.AddHandlers(from.Events);
			}
        }

		/// <summary>
		/// Clears a text of all selected components.
		/// </summary>
		public void ClearContents()
		{
		    if (IsSelected)
		    {
		        var text = this as IStiText;
		        if (text != null)
		            text.SetTextInternal(string.Empty);

		        var textElement = this as IStiTextElement;
		        if (textElement != null)
		            textElement.Text = "";

				if (this is IStiButtonElement button)
					button.Text = "";

		    }

		    var container = this as StiContainer;
			if (container != null)
				container.Components.ToList().ForEach(c => c.ClearContents());
		}
		
		/// <summary>
		/// Increases the amount of pages on width and on height.
		/// Herewith on width or height of the page change all sizes a parent component.
		/// </summary>
        /// <param name="leftToRightDirection">A parameter which speifiess a direction of adding size to the component.</param>
		public void AddSize(bool leftToRightDirection)
		{
			var parent = this is StiPage ? this as StiContainer : this.Parent;

			if (leftToRightDirection)
			{
				if (this.Page.UnlimitedWidth)
                    this.Page.SegmentPerWidth++;

				var cont = parent;

				while (!(cont is StiPage))
				{
					var rect = cont.GetDockRegion(cont.Parent);
					cont.Width += rect.Width;

					if (cont.Right > rect.Width)
						cont.Width = rect.Width - cont.Left;

					cont = cont.Parent;
				}
			}
			else
			{
			    this.Page.SegmentPerHeight++;
				
				//Fix bug with unlimited height and container
				var heightDiscount = this.Page.Height - height;

			    if (!(this is StiPage) && !(this is StiBand))
			        this.height += heightDiscount;

			    var cont = parent;
				while (!(cont is StiPage))
				{
					var rect = cont.GetDockRegion(cont.Parent);
					cont.Height += rect.Height;

					if (cont.Bottom > rect.Height)
						cont.Height = rect.Height - cont.Top;

					cont = cont.Parent;
				}

				#region Place all component on page to one container for speed increasing
				var codeName = "####UNLIMITED";

                var corrCont = new StiContainer
                {
                    Name = codeName,
                    DockStyle = StiDockStyle.Top,
                    Height = Page.PageHeight - Page.Margins.Top - Page.Margins.Bottom
                };

                double totalHeight = 0;
				var index = 0;
				while (index < this.Page.Components.Count)
				{
					var comp = this.Page.Components[index];
					if (comp.Name != codeName && comp.DockStyle == StiDockStyle.Top)
					{
						if ((comp.Height + totalHeight > corrCont.Height))break;

						this.Page.Components.Remove(comp);
						corrCont.Components.Add(comp);						
						totalHeight += comp.Height;
					}
					else
                        index++;
				}				

				if (this.Page.UnlimitedBreakable)
					corrCont.Height = Math.Max(totalHeight, corrCont.Height);

				else
                    corrCont.Height = totalHeight;
				#endregion

				this.Page.Components.Insert(index, corrCont);
			}

			this.Page.DockToContainer();
		}

        /// <summary>
        /// Returns an Name of the component and/or the Alias of the component.
        /// </summary>
        /// <returns>An Name of the component and/or the Alias of the component</returns>
        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool onlyAlias)
        {
            if (onlyAlias && !string.IsNullOrWhiteSpace(Alias))
                return Alias;

            if (Name == Alias || string.IsNullOrWhiteSpace(Alias))
                return Name;

            return $"{Name} [{Alias}]";
        }        

		/// <summary>
        /// Specifies that this component may be located in the specified component.
		/// </summary>
		/// <param name="component">A component for checking.</param>
		/// <returns>Returns true if this container may be located in the specified component.</returns>
		public virtual bool CanContainIn(StiComponent component)
		{
			if (component is IStiCrossTab)
				return this is IStiCrossTabField;

            if (component is IStiReportControl)
                return false;

			if (!(component is StiContainer))
                return false;

			if (component is StiClone)
                return false;

			if (component is StiSubReport)
                return false;

			if (component is IStiElement && !(this is IStiElement))
				return false;

			if (!(component is IStiElement) && this is IStiElement)
				return false;

			return true;		
		}		
		
		/// <summary>
		/// Returns the level of the object nesting.
		/// Level 1 - page, 2 - page and etc.
		/// </summary>
        /// <returns>Level 1 - page, 2 - page and etc.</returns>
		public int GetLevel()
		{
			StiComponent obj = Parent;

			var level = 1;
			while (obj != null)
			{
				obj = obj.Parent;
				level++;
			}
			return level;
		}

		/// <summary>
		/// Converts a rectangle from the parent-container coordinates into coordinates of a page.
		/// The method calls the ContainerToPage method of the parent component.
		/// </summary>
		/// <param name="rect">A rectangle for converting.</param>
		/// <returns>Converted rectangle.</returns>
		public RectangleD ComponentToPage(RectangleD rect)
		{
			if (Parent != null)
                return Parent.ContainerToPage(rect);
			else
                return rect;
		}
		
		/// <summary>
		/// Converts a rectangle from coordinates of a page into the parent-container coordinates.
		/// The method calls the PageToContainer method of the parent component.
		/// </summary>
		/// <param name="rect">A rectangle for converting.</param>
		/// <returns>Converted rectangle.</returns>
		public RectangleD PageToComponent(RectangleD rect) 
		{
			if (Parent != null)
                return Parent.PageToContainer(rect);
			else
                return rect;
		}
		
		/// <summary>
		/// Converts a point from the parent-container coordinates into coordinates of a page.
		/// The method calls the ContainerToPage method of the parent component.
		/// </summary>
		/// <param name="point">Point for converting.</param>
		/// <returns>Converted point.</returns>
		public PointD ComponentToPage(PointD point)
		{
			if (Parent != null)
                return Parent.ContainerToPage(point);
			else
                return point;
		}

		/// <summary>
		/// Converts a value from the parent-container coordinates into coordinates of a page.
		/// The method calls the ContainerToPage method of the parent component.
		/// </summary>
		/// <param name="value">A value for converting.</param>
		/// <returns>Converted value.</returns>
		public double ComponentToPage(double value)
		{
			return ComponentToPage(new PointD(0, value)).Y;
		}

		/// <summary>
		/// Converts a point from coordinates of a page into the parent-container coordinates.
		/// The method calls the PageToContainer method of the parent component.
		/// </summary>
		/// <param name="point">A point for converting.</param>
		/// <returns>Converted point.</returns>
		public PointD PageToComponent(PointD point)
		{
			var container = Parent;

			if (container != null)
                return Parent.PageToContainer(point);
			else
                return point;
		}		

		/// <summary>
		/// Retuns true if one of the parent component is selected.
		/// </summary>
		/// <param name="component">A component for checking.</param>
		/// <returns>True if the parent component is selected.</returns>
		public static bool IsParentSelect(StiComponent component)
		{
            if (component is StiCrossLinePrimitive)
            {
                var crossLine = component as StiCrossLinePrimitive;
                var startPoint = crossLine.GetStartPoint();
                var endPoint = crossLine.GetEndPoint();

                return (startPoint != null && IsParentSelect(startPoint)) || (endPoint != null && IsParentSelect(endPoint));
            }

			var parentComponent = component.Parent;
			while (parentComponent != null && !(parentComponent is StiPage))
			{
				if (parentComponent.IsSelected)return true;

				parentComponent = parentComponent.Parent;
			}
			return false;
		}
		
		/// <summary>
		/// Moves a rectangle to the specified offset.
		/// </summary>
		/// <param name="component">The component, relativly to which offset will be done.</param>
		/// <param name="rect">A rectangle being offset.</param>
		/// <param name="offsetRect">Offset rectangle.</param>
		/// <returns>Offset rectangle.</returns>
		public static RectangleD DoOffsetRect(StiComponent component, RectangleD rect, RectangleD offsetRect)
		{
            if (component.Locked)
                return rect;

            if (component is StiPage)
                return rect;

            var info = component?.Report?.Info;
            if (info != null && info.IsDragDropComponent && info.DraggingComponent != component && info.DraggingLabelComponent != component)
                return rect;

			if ((!IsParentSelect(component)) && component.IsSelected)
                return rect.OffsetRect(offsetRect);
            else 
			    return rect;
		}

        /// <summary>
        /// Makes all necessary conversions for the object output and returns its coordinates.
        /// </summary>
        /// <param name="convertToHInches">Convert into hundredths of inch.</param>
        /// <param name="convertByZoom">Convert by zoom.</param>
        /// <returns>A rectangle of a component.</returns>
		public RectangleD GetPaintRectangle(bool convertToHInches, bool convertByZoom)
		{
			return GetPaintRectangle(convertToHInches, convertByZoom, true);
		}

        /// <summary>
        /// Makes all necessary conversions for the object output and returns its coordinates.
        /// </summary>
        /// <param name="convertToHInches">Convert into hundredths of inch.</param>
        /// <param name="convertByZoom">Convert by zoom.</param>
        /// <param name="docking">A parameter which specifies a docking of the rectangle to the container.</param>
        /// <returns>A rectangle of a component.</returns>
        public RectangleD GetPaintRectangle(bool convertToHInches, bool convertByZoom, bool docking)
        {
            if (Page == null)
                return RectangleD.Empty;

            #region Docks component to container
            if (docking)
            {                
                if (IsCross)
                    ClientRectangle = DockToContainer(ClientRectangle);

                else if (DockStyle != StiDockStyle.None && Dockable)
                    DisplayRectangle = DockToContainer(DisplayRectangle);                
            }
            #endregion

            //Offsets component
            var rect = DoOffsetRect(this, ClientRectangle, Page.OffsetRectangle);

            //Translates coordinates of the component in paged from containerized
            rect = ComponentToPage(rect);

            //Normalizes coordinates
            rect = rect.Normalize();

            //Translates to hundreds of inches
            if (!convertToHInches)
                return convertByZoom ? rect.Multiply(Page.Zoom * StiScale.Factor) : rect;

            return convertByZoom 
                ? Page.Unit.ConvertToHInches(rect).Multiply(Page.Zoom * StiScale.Factor) 
                : Page.Unit.ConvertToHInches(rect);
        }
		
        /// <summary>
        /// Internal use only.
        /// </summary>
		public void SetPaintRectangle(RectangleD rect)
		{
			//Translates coordinates of the component in paged from containerized
			rect = PageToComponent(rect);
			
			//Normalizes coordinates
			rect = rect.Normalize();

			ClientRectangle = rect;
		}
		
		/// <summary>
		/// Makes all necessary coversions for showing an object and returns its coordinates.
		/// </summary>
		/// <returns>Prepared object.</returns>
		public virtual RectangleD GetPaintRectangle()
		{
			return GetPaintRectangle(true, true);			
		}

		/// <summary>
		/// Returns a rectangle of the component showing.
		/// </summary>
		/// <returns>A rectangle.</returns>
		public RectangleD GetDisplayRectangle()
		{
			//Offsets component
			var rect = this.DisplayRectangle;
			rect = DoOffsetRect(this, rect, Page == null ? RectangleD.Empty : Page.OffsetRectangle);
			
			//Translates coordinates of the component in paged from containerized
			rect = this.ComponentToPage(rect);
			
			//Normalizes and returns coordinates
			return rect.Normalize();
		}

		/// <summary>
		/// Returns the DataBand in which the component is located.
		/// Returns null, if nothing is located. 
		/// </summary>
        /// <returns>A DataBand in which the component is located.</returns>
		[Description("Returns DataBand on which the component is located. Returns null, if nothing is located.")]
		public virtual StiDataBand GetDataBand()
		{
			StiComponent obj = Parent;
			if (obj == null)return null;

			while (obj != null && !(obj is StiDataBand) && !(obj is StiPage))
			{
				var reportTitle = obj as StiReportTitleBand;
				if (reportTitle != null)
				{
                    var reportTitleMaster = reportTitle.GetMaster();
					if (reportTitleMaster != null)
                        return reportTitleMaster as StiDataBand;
				}

                var pageHeader = obj as StiPageHeaderBand;
				if (pageHeader != null)
				{
                    var pageHeaderMaster = pageHeader.GetMaster();
					if (pageHeaderMaster != null)
                        return pageHeaderMaster as StiDataBand;
				}

                var reportSummary = obj as StiReportSummaryBand;
				if (reportSummary != null)
				{
                    var reportSummaryMaster = reportSummary.GetMaster();
					if (reportSummaryMaster != null)
                        return reportSummaryMaster as StiDataBand;
				}

                var pageFooter = obj as StiPageFooterBand;
				if (pageFooter != null)
				{
                    var pageFooterMaster = pageFooter.GetMaster();
					if (pageFooterMaster != null)
                        return pageFooterMaster as StiDataBand;
				}

                var header = obj as StiHeaderBand;
				if (header != null)
				{
                    var headerMaster = Report != null && Report.EngineVersion == StiEngineVersion.EngineV1 
                        ? StiHeaderBandV1Builder.GetMaster(header) 
                        : StiHeaderBandV2Builder.GetMaster(header);

					if (headerMaster != null)
                        return headerMaster as StiDataBand;
				}

                var footer = obj as StiFooterBand;
				if (footer != null)
				{
                    var footerMaster = Report != null && Report.EngineVersion == StiEngineVersion.EngineV1 
                        ? StiFooterBandV1Builder.GetMaster(footer) 
                        : StiFooterBandV2Builder.GetMaster(footer);

					if (footerMaster != null)
                        return footerMaster as StiDataBand;
				}
				obj = obj.Parent;
			}

			if (obj is StiDataBand)
                return (StiDataBand)obj;

			return null;
		}

		/// <summary>
		/// Returns the GroupHeaderBand in which the component is located.
		/// Returns null, if nothing is located. 
		/// </summary>
		/// <returns>GroupHeaderBand.</returns>
		[Description("Returns GroupHeaderBand in which the component is located. Returns null, if nothing is located.")]
		public StiGroupHeaderBand GetGroupHeaderBand()
		{
			var obj = this is StiGroupFooterBand ? this : Parent;

			if (obj == null)return null;

            if ((obj is StiChildBand) && (obj.Parent != null))
            {
                var index = obj.Parent.Components.IndexOf(obj) - 1;

                while ((index > 0) && (obj.Parent.Components[index] is StiChildBand))
                    index--;

                if (index >= 0)
                    obj = obj.Parent.Components[index];
            }

			while (obj != null && 
				(!(obj is StiDataBand)) && 
				(!(obj is StiGroupHeaderBand)) && 
				(!(obj is StiGroupFooterBand)) && 
				(!(obj is StiPage)))
			{
				obj = obj.Parent;
			}
			
			if (obj is StiGroupHeaderBand)
                return (StiGroupHeaderBand)obj;

			else if (obj is StiGroupFooterBand || obj is StiDataBand)
			{
				StiDataBand dataBand = null;

				if (!(obj is StiDataBand))
				{
					var index = obj.Parent.Components.IndexOf(obj) - 1;
					for (var ind = index; ind >= 0; ind --)
					{
                        var tempComp = obj.Parent.Components[ind] as StiDataBand;
                        if (tempComp != null)
						{
                            dataBand = tempComp;
							break;
						}
					}
				}
				else dataBand = obj as StiDataBand;

				if (dataBand != null)
				{
					var builder = StiV1Builder.GetBuilder(typeof(StiDataBand)) as StiDataBandV1Builder;

					dataBand.DataBandInfoV1.GroupHeaderComponents = builder.GetGroupHeaders(dataBand);
					dataBand.DataBandInfoV1.GroupFooterComponents = builder.GetGroupFooters(dataBand);

					builder.GroupsComparison(dataBand);

					if (obj is StiGroupFooterBand)
                        return ((StiGroupFooterBand)obj).GroupFooterBandInfoV1.GroupHeader;
					else 
					{
						if (dataBand.DataBandInfoV1.GroupHeaderComponents.Count > 0)
							return dataBand.DataBandInfoV1.GroupHeaderComponents[dataBand.DataBandInfoV1.GroupHeaderComponents.Count - 1] as StiGroupHeaderBand;
					}
				}
			}
			return null;
		}

        /// <summary>
        /// Specifies whether it is possible to place the component on the ColumnBand.
        /// </summary>
        /// <returns>If the component can be placed on the ColumnBand then returns true.</returns>
		public bool PlacedOnColumnBand()
		{
			var obj = this;

			while (obj != null || (!(obj is StiPage)))
			{
				if (obj is StiColumnFooterBand || obj is StiColumnHeaderBand)
                    return true;

				if (obj is StiBand)
                    return false;

				obj = obj.Parent;
			}
			return false;
		}

		/// <summary>
		/// Returns the topmost Container(StiPanel or StiPage) in which the component is located.
		/// Returns null, if nothing is located.
		/// </summary>
		/// <returns>A container or null if nothing is located.</returns>
		public StiContainer GetContainer()
		{
			StiComponent obj = Parent;
			if (obj == null)return null;

			while (obj != null && !(obj is StiContainer) && !(obj is StiPage))
			{
				obj = obj.Parent;
			}

			if (obj is StiContainer)
                return (StiContainer)obj;

			if (obj is StiPage)
                return (StiContainer)obj;

			return obj.Page;
		}

        internal bool CheckForParentComponent(StiComponent comp)
        {
            if (this.Parent == null)
                return false;

            if (this.Parent == comp)
                return true;

            return this.Parent.CheckForParentComponent(comp);
        }

	    internal StiBaseStyle GetComponentStyle()
	    {
            if (string.IsNullOrWhiteSpace(this.ComponentStyle) || Report == null) return null;

	        return Report.Styles[this.ComponentStyle];
	    }

        internal virtual List<object> GetFormatObjects()
        {
            return new List<object> { this };
        }

        internal virtual object GetFirstFormatObject()
        {
            var objects = GetFormatObjects();
            return objects == null ? null : objects.FirstOrDefault();
        }
        #endregion

        #region Fields 
        private bitsComponent bits;
        #endregion

        /// <summary>
        /// Creates a new component of the StiComponent type.
        /// </summary>
        public StiComponent() : this(RectangleD.Empty)
		{
		}

        /// <summary>
        /// Creates a new component of the type StiComponent with the specified location.
        /// </summary>
        /// <param name="rect">Rectangle describes size and position of the component.</param>
        public StiComponent(RectangleD rect)
		{
			ClientRectangle = rect;
            lockOnResize = false;
			
			if (this is StiDataBand || this is StiGroupHeaderBand)
				this.Interaction = new StiBandInteraction();

			else if (this is StiCrossHeader)
                this.Interaction = new StiCrossHeaderInteraction();

            else
				this.Interaction = new StiInteraction();
		}
    }	
}