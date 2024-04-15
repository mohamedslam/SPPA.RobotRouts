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
using System.Drawing.Design;
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes class that realizes component - Clone.
    /// </summary>
    [StiToolbox(true)]
    [StiServiceBitmap(typeof(StiClone), "Stimulsoft.Report.Images.Components.StiClone.png")]
    [StiDesigner("Stimulsoft.Report.Components.Design.StiCloneDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfCloneDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiGdiPainter(typeof(StiCloneGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiCloneWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiV1Builder(typeof(StiCloneV1Builder))]
    [StiV2Builder(typeof(StiCloneV2Builder))]
    [StiContextTool(typeof(IStiClone))]
    [StiContextTool(typeof(IStiShift))]
    [StiContextTool(typeof(IStiComponentDesigner))]
    [StiEngine(StiEngineVersion.All)]
    public class StiClone :
        StiContainer,
        IStiClone
    {
        #region IStiJsonReportObject.override
        internal string jsonContainerValueTemp;

        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("CanShrink");
            jObject.RemoveProperty("CanGrow");
            jObject.RemoveProperty("Conditions");
            jObject.RemoveProperty("GrowToHeight");
            jObject.RemoveProperty("Components");

            // StiClone
            jObject.AddPropertyBool("ScaleHor", ScaleHor);

            if (Container != null)
                jObject.AddPropertyStringNullOrEmpty("Container", Container.Name);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "ScaleHor":
                        ScaleHor = property.DeserializeBool();
                        break;

                    case "Container":
                        jsonContainerValueTemp = property.DeserializeString();
                        Report.jsonLoaderHelper.Clones.Add(this);
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiClone;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

            collection.Add(StiPropertyCategories.ComponentEditor, new[]
            {
                propHelper.CloneEditor()
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
                    propHelper.Enabled()
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
                        StiPropertyEventId.GetTagEvent,
                        StiPropertyEventId.GetToolTipEvent,
                    }
                }
            };
        }
        #endregion

        #region StiComponent.Properties
        public override string HelpUrl => "user-manual/index.html?report_internals_panels_cloning.htm";
        #endregion

        #region IStiStateSaveRestore
        /// <summary>
        /// Saves the current state of an object.
        /// </summary>
        /// <param name="stateName">A name of the state being saved.</param>
        public override void SaveState(string stateName)
        {
            var cont = Container;
            Container = null;
            base.SaveState(stateName);
            Container = cont;
        }

        /// <summary>
        /// Restores the earlier saved object state.
        /// </summary>
        /// <param name="stateName">A name of the state being restored.</param>
        public override void RestoreState(string stateName)
        {
            var cont = Container;
            Container = null;
            base.RestoreState(stateName);
            Container = cont;
        }
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

        #region IStiClone
        /// <summary>
        /// Gets or sets a clone container.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Reference)]
        [TypeConverter(typeof(StiCloneConverter))]
        [Editor("Stimulsoft.Report.Components.Design.StiCloneEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Data")]
        [Description("Gets or sets a clone container.")]
        [StiEngine(StiEngineVersion.All)]
        public StiContainer Container { get; set; }

        private bool ShouldSerializeContainer()
        {
            return Container != null;
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            var clone = this.MemberwiseClone() as StiClone;

            clone.cloneInfoV1 = this.CloneInfoV1.Clone() as StiCloneInfoV1;
            clone.Components = new StiComponentsCollection(clone);

            return clone;
        }
        #endregion

        #region Render
        private StiCloneInfoV1 cloneInfoV1;
        [Browsable(false)]
        public StiCloneInfoV1 CloneInfoV1
        {
            get
            {
                return cloneInfoV1 ?? (cloneInfoV1 = new StiCloneInfoV1());
            }
        }

        /// <summary>
        /// Gets or sets value indicates whether a component is rendered or not.
        /// </summary>
        public override bool IsRendered
        {
            get
            {
                return Container == null || Container.IsRendered;
            }
            set
            {
                if (Container != null)
                    Container.IsRendered = value;
            }
        }
        #endregion

        #region StiComponent override
        [Browsable(false)]
        [StiNonSerialized]
        public sealed override StiConditionsCollection Conditions
        {
            get
            {
                return base.Conditions;
            }
            set
            {
                base.Conditions = value;
            }
        }

        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.Clone;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Components;

        /// <summary>
        /// Gets value, indicates that it is necessary to draw again the whole page when moving the component or
        /// changing its sizes in the designer.
        /// </summary>
        [Browsable(false)]
        public override bool ForceRedrawAll => true;

        /// <summary>
        /// May this container be located in the specified component.
        /// </summary>
        /// <param name="component">Component for checking.</param>
        /// <returns>true, if this container may is located in the specified component.</returns>
        public override bool CanContainIn(StiComponent component)
        {
            if (component is IStiReportControl) 
                return false;

            if (component is StiClone) 
                return false;

            if (component is IStiCrossTab)
                return false;

            if (component is StiContainer) 
                return true;

            return false;
        }

        /// <summary>
        /// Gets a localized name of the component category.
        /// </summary>
        public override string LocalizedCategory => Loc.Get("Report", "Components");

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => Loc.Get("Components", "StiClone");

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
        /// Gets the type of processing when printing.
        /// </summary>
        public override StiComponentType ComponentType
        {
            get
            {
                return Report != null && Report.EngineVersion == StiEngineVersion.EngineV1
                    ? StiComponentType.Detail
                    : StiComponentType.Simple;
            }
        }
        #endregion

        #region StiContainer override
        /// <summary>
        /// Gets or sets a collection of components.
        /// </summary>
        [Browsable(false)]
        [StiNonSerialized]
        public override StiComponentsCollection Components
        {
            get
            {
                return Container != null && !IsDesigning ? Container.Components : base.Components;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Raises the BeforePrint event.
        /// </summary>
        protected override void OnBeforePrint(EventArgs e)
        {
            base.OnBeforePrint(e);

            Container?.InvokeBeforePrint(this, e);
        }

        /// <summary>
        /// Raises the AfterPrint event.
        /// </summary>
        protected override void OnAfterPrint(EventArgs e)
        {
            base.OnAfterPrint(e);

            Container?.InvokeAfterPrint(this, e);
        }
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiClone();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets value which indicates that contents of the container will be shrunk or grown.
        /// </summary>
        [StiCategory("Data")]
        [Browsable(true)]
        [DefaultValue(false)]
        [StiSerializable]
        [Description("Gets or sets value which indicates that contents of the container will be shrunk or grown.")]
        [StiEngine(StiEngineVersion.EngineV1)]
        public bool ScaleHor { get; set; }
        #endregion

        /// <summary>
        /// Creates a new clone.
        /// </summary>
        public StiClone() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new clone.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiClone(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = false;
        }
    }
}