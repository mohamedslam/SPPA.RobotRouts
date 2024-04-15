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
using System.Drawing.Design;
using System.IO;
using System.Linq;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.Units;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Events;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Helpers;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// Describes class that realizes component - SubReport.
	/// </summary>
    [StiServiceBitmap(typeof(StiSubReport), "Stimulsoft.Report.Images.Components.StiSubReport.png")]
	[StiDesigner("Stimulsoft.Report.Components.Design.StiSubReportDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfSubReportDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiGdiPainter(typeof(StiSubReportGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiSubReportWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	[StiV1Builder(typeof(StiSubReportV1Builder))]
    [StiV2Builder(typeof(StiSubReportV2Builder))]
	[StiToolbox(true)]
	[StiContextTool(typeof(IStiShift))]
	[StiContextTool(typeof(IStiComponentDesigner))]
    [StiEngine(StiEngineVersion.All)]
	public class StiSubReport : StiContainer
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("CanShrink");
            jObject.RemoveProperty("CanGrow");
            jObject.RemoveProperty("GrowToHeight");

            // StiSubReport
            jObject.AddPropertyJObject("FillParametersEvent", FillParametersEvent.SaveToJsonObject(mode));
            jObject.AddPropertyBool("KeepSubReportTogether", KeepSubReportTogether);
            jObject.AddPropertyStringNullOrEmpty("SubReportPageGuid", SubReportPageGuid);
            jObject.AddPropertyStringNullOrEmpty("SubReportUrl", SubReportUrl);
            
            if (mode == StiJsonSaveMode.Report)
            {
                jObject.AddPropertyJObject("Parameters", Parameters.SaveToJsonObject(mode));
            }

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "FillParametersEvent":
                        {
                            var _event = new StiFillParametersEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.FillParametersEvent = _event;
                        }
                        break;

                    case "KeepSubReportTogether":
                        this.KeepSubReportTogether = property.DeserializeBool();
                        break;

                    case "SubReportPageGuid":
                        this.SubReportPageGuid = property.DeserializeString();
                        break;

                    case "SubReportUrl":
                        this.SubReportUrl = property.DeserializeString();
                        break;

                    case "Parameters":
                        this.Parameters.LoadFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiSubReport;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

            collection.Add(StiPropertyCategories.ComponentEditor, new[] 
            { 
                propHelper.SubReportDesigner()
            });

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height(),
                    propHelper.MinSize(),
                    propHelper.MaxSize()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.ComponentStyle()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.ComponentStyle(),
                    propHelper.UseParentStyles()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.Enabled(),
                    propHelper.KeepSubReportTogether()
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.AnchorMode(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.KeepSubReportTogether(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.AnchorMode(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.KeepSubReportTogether(),
                    propHelper.Printable(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name()
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name(),
                    propHelper.Alias()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name(),
                    propHelper.Alias(),
                    propHelper.Restrictions(),
                    propHelper.Locked(),
                    propHelper.Linked()
                });
            }

            return collection;
        }

        public override StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return new StiEventCollection
            {
                {
                    StiPropertyCategories.MouseEvents,
                    new[]
                    {
                        StiPropertyEventId.ClickEvent,
                        StiPropertyEventId.DoubleClickEvent,
                        StiPropertyEventId.MouseEnterEvent,
                        StiPropertyEventId.MouseLeaveEvent
                    }
                },
                {
                    StiPropertyCategories.NavigationEvents,
                    new[]
                    {
                        StiPropertyEventId.GetBookmarkEvent,
                        StiPropertyEventId.GetDrillDownReportEvent,
                        StiPropertyEventId.GetHyperlinkEvent,
                        StiPropertyEventId.GetPointerEvent,
                    }
                },
                {
                    StiPropertyCategories.PrintEvents,
                    new[]
                    {
                        StiPropertyEventId.AfterPrintEvent,
                        StiPropertyEventId.BeforePrintEvent,
                    }
                },
                {
                    StiPropertyCategories.ValueEvents,
                    new[]
                    {
                        StiPropertyEventId.FillParametersEvent,
                        StiPropertyEventId.GetTagEvent,
                        StiPropertyEventId.GetToolTipEvent,
                    }
                }
            };
        }
        #endregion

        #region StiComponent.Properties
        public override string HelpUrl => "user-manual/index.html?report_internals_sub-reports.htm";
        #endregion

		#region IStiCanShrink override
		[Browsable(false)]
		[StiNonSerialized]
		public override bool CanShrink
		{
			get
			{
				return base.CanShrink;
			}
			set
			{
                base.CanShrink = value;
			}
		}
		#endregion

		#region IStiCanGrow override
		[Browsable(false)]
		[StiNonSerialized]
		public override bool CanGrow
		{
			get
			{
				return base.CanGrow;
			}
			set
			{
                base.CanGrow = value;
			}
		}
		#endregion

        #region IStiAnchor Off
        [StiNonSerialized]
        [Browsable(false)]
        public override StiAnchorMode Anchor
        {
            get
            {
                return base.Anchor;
            }
            set
            {
                base.Anchor = value;
            }
        }
        #endregion

		#region ICloneable override
        public override object Clone(bool cloneProperties)
        {
            return Clone(cloneProperties, false);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone(bool cloneProperties, bool cloneComponents)
        {
            var subReport = (StiSubReport)base.Clone(cloneProperties, false);

            subReport.subReportInfoV1 = this.SubReportInfoV1.Clone() as StiSubReportInfoV1;

            return subReport;
        }
        #endregion

        #region StiComponent override
        /// <summary>
        /// Returns events collection of this component.
        /// </summary>
        public override StiEventsCollection GetEvents()
        {
            var events = base.GetEvents();

            if (FillParametersEvent != null)
                events.Add(FillParametersEvent);

            return events;
        }

        [StiEngine(StiEngineVersion.EngineV2)]
		public override StiShiftMode ShiftMode
		{
			get 
			{
				return base.ShiftMode;
			}
			set 
			{
				base.ShiftMode = value;
			}
		}

		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition => (int)StiComponentToolboxPosition.SubReport;

        public override StiToolboxCategory ToolboxCategory { get; } = StiToolboxCategory.Components;

        /// <summary>
		/// May this container be located in the specified component.
		/// </summary>
		/// <param name="component">Component for checking.</param>
		/// <returns>true, if this container may is located in the specified component.</returns>
		public override bool CanContainIn(StiComponent component)
		{
			if (this.Report != null && this.Report.EngineVersion == StiEngineVersion.EngineV1)
			{
				if (component is IStiReportControl)return false;
				if (component is StiPage)return true;
				if (component is StiDataBand)return true;
			}
			else
			{
				if (component is StiReportControl)return false;
            	if (component is StiContainer && !(component is StiSubReport || component is StiClone)) return true;
			}
			return false;
		}

		/// <summary>
		/// Gets the type of processing when printing.
		/// </summary>
		public override StiComponentType ComponentType => StiComponentType.Simple;

        /// <summary>
		/// Gets a localized name of the component category.
		/// </summary>
		public override string LocalizedCategory => StiLocalization.Get("Report", "Components");

        /// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName => StiLocalization.Get("Components", "StiSubReport");

        [Browsable(false)]
		[StiNonSerialized]
		public override bool GrowToHeight
		{
			get 
			{
				return base.GrowToHeight;
			}
			set 
			{
			}
		}

		/// <summary>
		/// Gets a component priority.
		/// </summary>
		public override int Priority
		{
			get
			{
                if (this.Report != null && this.Report.EngineVersion == StiEngineVersion.EngineV1)
                    return (int)StiComponentPriority.SubReportsV1;
                else
                    return (int)StiComponentPriority.SubReportsV2;
			}
		}
		#endregion

        #region Convert override
        /// <summary>
        /// Converts a component out of one unit into another.
        /// </summary>
        /// <param name="oldUnit">Old units.</param>
        /// <param name="newUnit">New units.</param>
        public override void Convert(StiUnit oldUnit, StiUnit newUnit, bool isReportSnapshot = false)
        {
            this.Left = newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(this.Left));
            this.Top = newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(this.Top));
            base.Width = newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(this.Width));
            this.Height = newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(this.Height));

            if (!isReportSnapshot)
            {
                this.MinSize = newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(this.MinSize));
                this.MaxSize = newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(this.MaxSize));
            }
        }
        #endregion

		#region Position override
        public override double Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                base.Width = value;
                UpdateSubReportPageWidth();
            }
        }

        public override RectangleD ClientRectangle
		{
			get
			{
				return base.ClientRectangle;
			}
			set
			{
                base.ClientRectangle = value;
                UpdateSubReportPageWidth();
			}
		}
		
		private void UpdateSubReportPageWidth()
        {
            if (SubReportPage == null || Report == null || Report.EngineVersion != StiEngineVersion.EngineV2) return;

            if (Page != null && SubReportPage.Orientation != Page.Orientation)
            {
                var oldHeight = SubReportPage.PageHeight;
                SubReportPage.Orientation = Page.Orientation;
                SubReportPage.PageHeight = oldHeight;
                SubReportPage.Margins = new StiMargins(
                    SubReportPage.Margins.Top,
                    SubReportPage.Margins.Bottom,
                    SubReportPage.Margins.Left,
                    SubReportPage.Margins.Right);
            }

            SubReportPage.Width = this.Width;
        }
        #endregion

		#region Render
		private StiSubReportInfoV1 subReportInfoV1;
		[Browsable(false)]
		public StiSubReportInfoV1 SubReportInfoV1
		{
			get
			{
			    return subReportInfoV1 ?? (subReportInfoV1 = new StiSubReportInfoV1());
			}
		}

        /// <summary>
        /// Returns the SubReport template for rendering.
        /// </summary>
        public StiReport GetExternalSubReport()
        {
            var externalReport = GetSubReportFromUrl(SubReportUrl);
            if (externalReport == null)
                externalReport = GetSubReportFromFile(SubReportUrl);

            if (externalReport == null)
            {
                var e = new StiGetSubReportEventArgs(this.Name);
                this.Report.InvokeGetSubReport(e);
                externalReport = e.Report;
            }

            if (externalReport != null && externalReport.ReportUnit != this.Report.ReportUnit)
            {
                if (externalReport.IsDocument)
                {
                    foreach (StiPage page in externalReport.RenderedPages)
                    {
                        page.Convert(externalReport.Unit, this.Report.Unit);
                    }
                }
                else
                    externalReport.ReportUnit = this.Report.ReportUnit;
            }

            if (externalReport != null && StiOptions.Engine.ForceInterpretationMode)
                externalReport.CalculationMode = StiCalculationMode.Interpretation;

            return externalReport;
        }

        /// <summary>
        /// Returns the SubReport from specified url.
        /// </summary>
        protected StiReport GetSubReportFromUrl(string url)
        {
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    if (StiHyperlinkProcessor.IsServerHyperlink(url))
                        return StiStimulsoftServerResource.GetSubReport(this, StiHyperlinkProcessor.GetServerNameFromHyperlink(this.SubReportPageGuid));
                    
                    var subReport = StiActivator.CreateObject(StiOptions.Engine.BaseReportType) as StiReport;

                    #region Get report from resources
                    var resourceName = StiHyperlinkProcessor.GetResourceNameFromHyperlink(url);
                    if (resourceName != null)
                    {
                        resourceName = resourceName.ToLowerInvariant().Trim();

                        var resource = Report.Dictionary.Resources.ToList()
                            .FirstOrDefault(r => r.Name != null && r.Name.ToLowerInvariant().Trim() == resourceName);

                        if (resource != null)
                        {
                            switch (resource.Type)
                            {
                                case Dictionary.StiResourceType.Report:
                                    subReport.Load(resource.Content);
                                    break;

                                case Dictionary.StiResourceType.ReportSnapshot:
                                    subReport.LoadDocument(resource.Content);
                                    break;
                            }
                        }
                    }
                    #endregion

                    #region Get report from URL
                    else
                    {
                        if (url.EndsWithInvariantIgnoreCase(".mdc") || url.EndsWithInvariantIgnoreCase(".mdz"))
                            subReport.LoadDocumentFromUrl(url);
                        else
                            subReport.LoadFromUrl(url);
                    }
                    #endregion

                    return subReport;
                }
            }
            catch (Exception ex)
            {
                var str = $"SubReport can't be loaded from URL '{url}' in subreport component {Name}!";
                StiLogService.Write(this.GetType(), str);
                StiLogService.Write(this.GetType(), ex.Message);
                Report.WriteToReportRenderingMessages(str);
            }
            return null;
        }

        /// <summary>
        /// Returns the SubReport from specified path.
        /// </summary>
        protected virtual StiReport GetSubReportFromFile(string url)
        {
            var file = StiHyperlinkProcessor.GetFileNameFromHyperlink(url);
            if (file == null) return null;

            if (File.Exists(file))
            {
                try
                {
                    var subReport = StiActivator.CreateObject(StiOptions.Engine.BaseReportType) as StiReport;
                    string extension = Path.GetExtension(file);

                    if (extension == ".mdc" || extension == ".mdz")
                        subReport.LoadDocumentFromUrl(url);

                    else
                        subReport.LoadFromUrl(url);

                    return subReport;
                }
                catch (Exception ex)
                {
                    var str = $"SubReport can't be loaded from file '{file}' in subreport component {Name}!";
                    StiLogService.Write(this.GetType(), str);
                    StiLogService.Write(this.GetType(), ex.Message);
                    Report.WriteToReportRenderingMessages(str);
                }
            }
            else
            {
                var str = $"File '{file}' does not exist in subreport component {Name}!";
                StiLogService.Write(this.GetType(), str);
                Report.WriteToReportRenderingMessages(str);
            }
            return null;
        }
		#endregion

        #region Events
        private static readonly object EventFillParameters = new object();

        /// <summary>
        /// Occurs when the FillParameters is calculated.
        /// </summary>
        public event StiFillParametersEventHandler FillParameters
        {
            add
            {
                base.Events.AddHandler(EventFillParameters, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventFillParameters, value);
            }
        }

        /// <summary>
        /// Raises the FillParameters event.
        /// </summary>
        protected virtual void OnFillParameters(StiFillParametersEventArgs e)
        {
        }

        /// <summary>
        /// Raises the FillParameters event.
        /// </summary>
        public virtual void InvokeFillParameters(StiComponent sender, StiFillParametersEventArgs e)
        {
            try
            {
                OnFillParameters(e);

                var handler = base.Events[EventFillParameters] as StiFillParametersEventHandler;
                if (handler != null) handler(sender, e);

                if (Report == null || Report.CalculationMode != StiCalculationMode.Interpretation) return;
                if (Parameters == null || Parameters.Count <= 0) return;

                foreach (StiParameter param in Parameters)
                {
                    if (!string.IsNullOrEmpty(param.Name) && param.Expression != null && !string.IsNullOrEmpty(param.Expression.Value))
                    {
                        var parserResult = StiParser.ParseTextValue("{" + param.Expression.Value + "}", this);
                        e.Value.Add(param.Name, parserResult);
                    }
                }

                StiBlocklyHelper.InvokeBlockly(this.Report, this, FillParametersEvent, e);
            }
            catch (Exception ex)
            {
                var str = $"Expression in parameters of '{this.Name}' can't be evaluated!";
                StiLogService.Write(this.GetType(), str);
                StiLogService.Write(this.GetType(), ex.Message);
                Report.WriteToReportRenderingMessages(str);
            }
        }

        /// <summary>
        /// Occurs when the FillParameters is calculated.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when the FillParameters is calculated.")]
        public StiFillParametersEvent FillParametersEvent
        {
            get
            {
                return new StiFillParametersEvent(this);
            }
            set
            {
                if (value != null)
                    value.Set(this, value.Script);
            }
        }
        #endregion

        #region Properties
        /// <summary>
		/// Gets or sets value which indicates that this subreport requires the external report.
		/// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [Obsolete("StiSubReport.UseExternalReport property is obsolete.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool UseExternalReport
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

        /// <summary>
		/// Gets or sets value which indicates that the SubReport must to be kept together with DataBand on what it is placed.
		/// </summary>
		[DefaultValue(false)]
		[StiSerializable]
		[StiCategory("Behavior")]
		[StiOrder(StiPropertyOrder.BehaviorKeepSubReportTogether)]
		[TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that the SubReport must to be kept together with DataBand on what it is placed.")]
		[StiEngine(StiEngineVersion.EngineV2)]
		[StiShowInContextMenu]
		public virtual bool KeepSubReportTogether { get; set; }

        /// <summary>
		/// Gets or sets a page that contains SubReport.
		/// </summary>
		[StiNonSerialized]
		[TypeConverter(typeof(StiSubReportConverter))]
		[Editor("Stimulsoft.Report.Components.Design.StiSubReportEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiCategory("SubReport")]
		[StiOrder(StiPropertyOrder.SubReportSubReportPage)]
        [Description("Gets or sets a page that contains SubReport.")]
        [StiEngine(StiEngineVersion.All)]
		public StiPage SubReportPage
		{
			get 
			{
				if (Report == null)
                    return null;

				foreach (StiPage page in Report.Pages)
				{
					if (page.Guid == this.SubReportPageGuid)
					    return page;
				}
				return null;
			}
			set 
			{
                if (value == null)
                {
                    this.SubReportPageGuid = null;
                }
                else
				{
					if (value.Guid == null)
					    value.Guid = StiGuidUtils.NewGuid();

					this.SubReportPageGuid = value.Guid;
					UpdateSubReportPageWidth();
				}
			}
		}

        /// <summary>
		/// Gets or sets a guid of page that contains Subreport.
		/// </summary>
		[Browsable(false)]
		[StiSerializable]
		[DefaultValue(null)]
		public string SubReportPageGuid { get; set; }

        /// <summary>
        /// Gets or sets a URL that contains the SubReport.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [StiCategory("SubReport")]
        [Description("Gets or sets a URL that contains the SubReport.")]
        [DefaultValue(null)]
        public string SubReportUrl { get; set; }

        /// <summary>
        /// Gets or sets a parameter collection.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        [StiCategory("SubReport")]
        [Browsable(false)]
        [StiOrder(StiPropertyOrder.SubReportParameterCollection)]
        [Description("Gets or sets a parameter collection.")]
        [StiEngine(StiEngineVersion.All)]
        public StiParametersCollection Parameters { get; set; } = new StiParametersCollection();
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiSubReport();
        }
        #endregion

        #region Methods
        public static StiSubReport GetSubReportForPage(StiPage page)
        {
            if (page == null)
                return null;

            var comps = page.Report.GetComponents();
            foreach (StiComponent comp in comps)
            {
                if (comp is StiSubReport && ((StiSubReport) comp).SubReportPage == page)
                    return comp as StiSubReport;
            }
            return null;
        }
        #endregion

        /// <summary>
        /// Creates a new component of the type StiSubReport.
        /// </summary>
        public StiSubReport() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new component of the type StiSubReport.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		public StiSubReport(RectangleD rect) : base(rect)
		{
			PlaceOnToolbox = false;
		}
	}
}