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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Base.Drawing.Design;
using System.Drawing.Design;
using Stimulsoft.Report.Gauge;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes the class of Container. This class visible only in EngiveV1.
    /// EngiveV2 uses StiPanel class.
    /// </summary>
    [StiToolbox(true)]
    [StiServiceBitmap(typeof(StiContainer), "Stimulsoft.Report.Images.Components.StiContainer.png")]
    [StiGdiPainter(typeof(StiContainerGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiContainerWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiV1Builder(typeof(StiContainerV1Builder))]
    [StiV2Builder(typeof(StiContainerV2Builder))]
    [StiContextTool(typeof(IStiCanGrow))]
    [StiContextTool(typeof(IStiCanShrink))]
    [StiContextTool(typeof(IStiShift))]
    [StiContextTool(typeof(IStiGrowToHeight))]
    [StiEngine(StiEngineVersion.EngineV1)]
    public class StiContainer :
        StiComponent,
        IStiBorder,
        IStiBrush,
        IStiSerializable,
        IStiBreakable,
        IStiIgnoryStyle,
        IStiGetFonts,
        IStiCornerRadius
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiContainer
            jObject.AddPropertyBool("CanBreak", CanBreak);
            jObject.AddPropertyBorder("Border", Border);
            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyJObject("Components", Components.SaveToJsonObject(mode));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "CanBreak":
                        this.CanBreak = property.DeserializeBool();
                        break;

                    case "Border":
                        this.Border = property.DeserializeBorder();
                        break;

                    case "Brush":
                        this.Brush = property.DeserializeBrush();
                        break;

                    case "Components":
                        this.Components.LoadFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiContainer;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

            // PositionCategory
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
                    propHelper.Conditions(),
                    propHelper.ComponentStyle()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle(),
                    propHelper.UseParentStyles()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.GrowToHeight(),
                    propHelper.Enabled()
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.AnchorMode(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.GrowToHeight(),
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
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.GrowToHeight(),
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
        public override string HelpUrl => null;
        #endregion

        #region IStiBreakable
        private static object PropertyCanBreak = new object();

        /// <summary>
        /// Gets or sets value which indicates whether the component can or cannot break its contents on several pages.
        /// </summary>
        [DefaultValue(false)]
        [Browsable(false)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorCanBreak)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates whether the component can or cannot break its contents on several pages.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool CanBreak
        {
            get
            {
                return Properties.GetBool(PropertyCanBreak, false);
            }
            set
            {
                Properties.SetBool(PropertyCanBreak, value, false);
            }
        }

        /// <summary>
        /// Divides content of components in two parts. Returns result of dividing. If true, then component is successful divided.
        /// </summary>
        /// <param name="dividedComponent">Component for store part of content.</param>
        /// <returns>If true, then component is successful divided.</returns>
        public virtual bool Break(StiComponent dividedComponent, double devideFactor, ref double divideLine)
        {
            var newCont = StiComponentDivider.BreakContainer(this.Height, this);
            ((StiContainer)dividedComponent).Components.Clear();
            ((StiContainer)dividedComponent).Components.AddRange(newCont.Components);
            divideLine = this.Height;

            if (StiOptions.Engine.UsePrintOnAllPagesPropertyOfHeadersInSubreports)
            {
                #region find headers with PrintOnAllPages property
                var headerOnAllPages = new List<StiBand>();//StiHeaderBand
                var newHeaders = new List<StiContainer>();//StiContainer
                var dataBands = new List<StiBand>();

                foreach (StiComponent comp in this.Components)
                {
                    if (!(comp is StiContainer)) continue;

                    var band = (comp as StiContainer).ContainerInfoV2.ParentBand;

                    if (!(band is StiHeaderBand) || !(band as IStiPrintOnAllPages).PrintOnAllPages) continue;

                    int indexBand = headerOnAllPages.IndexOf(band);
                    if (indexBand != -1)
                    {
                        newHeaders[indexBand] = comp.Clone() as StiContainer;
                    }
                    else
                    {
                        headerOnAllPages.Add(band as StiHeaderBand);
                        newHeaders.Add(comp.Clone() as StiContainer);

                        if (band.Parent != null)
                            dataBands.Add(StiHeaderBandV2Builder.GetMaster(band as StiHeaderBand));
                        else
                            dataBands.Add(band);
                    }
                }
                #endregion

                #region check output container for master databands
                if (headerOnAllPages.Count > 0)
                {
                    var newComps = ((StiContainer)dividedComponent).Components;
                    var addedComps = new List<StiComponent>();
                    var counter = headerOnAllPages.Count;
                    foreach (StiComponent comp in newComps)
                    {
                        if (!(comp is StiContainer)) continue;

                        var band = (comp as StiContainer).ContainerInfoV2.ParentBand;

                        var dataBand = band as StiDataBand;
                        if (dataBand == null) continue;

                        var band2 = band;
                        if (band is StiTable && dataBand.DataBandInfoV2.Headers != null)
                        {
                            foreach (StiComponent comp2 in dataBand.DataBandInfoV2.Headers)
                            {
                                if (comp2 is StiHeaderBand && comp2.Name == band.Name.Substring(0, band.Name.Length - 3) + "_Hd")
                                {
                                    band2 = comp2 as StiBand;
                                    break;
                                }
                            }
                        }

                        for (var index = headerOnAllPages.Count - 1; index >= 0; index--)
                        {
                            if (headerOnAllPages[index] != null && (band == dataBands[index] || band2 == dataBands[index]))
                            {
                                //check if header is already present
                                var needAdd = true;
                                for (var index1 = 0; index1 < headerOnAllPages.Count; index1++)
                                {
                                    StiContainer tempCont = null;
                                    if (index1 < newComps.Count)
                                        tempCont = newComps[index1] as StiContainer;

                                    if (tempCont != null && tempCont.ContainerInfoV2.ParentBand == headerOnAllPages[index])
                                        needAdd = false;
                                }

                                if (needAdd)
                                    addedComps.Add(newHeaders[index]);

                                headerOnAllPages[index] = null;
                                counter--;
                            }
                        }
                        if (counter == 0) break;
                    }

                    if (addedComps.Count > 0)
                    {
                        foreach (var newHeader in addedComps)
                        {
                            foreach (StiComponent comp2 in newComps)
                            {
                                comp2.Top += newHeader.Height;
                            }
                            newHeader.Top = 0;
                            newComps.Insert(0, newHeader);
                        }
                    }
                }
                #endregion
            }

            return true;
        }
        #endregion

        #region IStiSerializable
        private void Write(StiObjectStringConverter converter, string name, object obj, XmlTextWriter tw)
        {
            var s = converter.ObjectToString(obj);
            if (s != null)
                tw.WriteAttributeString(name, s);
        }

        /// <summary>
        /// Serializes object into XmlTextWriter.
        /// </summary>
        /// <param name="converter">The converter to convert objects into strings.</param>
        /// <param name="tw">XmlTextWriter for serialization.</param>
        public void Serialize(StiObjectStringConverter converter, XmlTextWriter tw)
        {
            tw.WriteAttributeString("name", this.Name);

            if (!this.Printable)
                tw.WriteAttributeString("pr", this.Printable.ToString());

            if (this.Guid != null)
                tw.WriteAttributeString("guid", this.Guid);

            if (this.PointerValue != null)
                tw.WriteAttributeString("pointer", this.PointerValue.ToString());

            if (this.BookmarkValue != null)
                tw.WriteAttributeString("bookmark", this.BookmarkValue.ToString());

            if (this.HyperlinkValue != null)
                tw.WriteAttributeString("hyperlink", this.HyperlinkValue.ToString());

            if (this.TagValue != null)
                tw.WriteAttributeString("tag", this.TagValue.ToString());

            if (this.ToolTipValue != null)
                tw.WriteAttributeString("toolTip", this.ToolTipValue.ToString());

            if (!string.IsNullOrWhiteSpace(this.ComponentPlacement))
                tw.WriteAttributeString("pl", this.ComponentPlacement);

            Write(converter, "rc", this.ClientRectangle, tw);
            Write(converter, "bh", this.Brush, tw);
            Write(converter, "br", this.Border, tw);
        }

        /// <summary>
        /// Deserilizes object from XmlTextReader.
        /// </summary>
        /// <param name="converter">The converter to convert strings into objects.</param>
        /// <param name="tr">XmlTextWriter for deserialization.</param>
        public void Deserialize(StiObjectStringConverter converter, XmlTextReader tr)
        {
            var value = tr.GetAttribute("name");
            if (value != null)
                this.Name = value;

            value = tr.GetAttribute("pr");
            if (value != null)
                this.Printable = false;

            this.Guid = tr.GetAttribute("guid");
            this.PointerValue = tr.GetAttribute("pointer");
            this.BookmarkValue = tr.GetAttribute("bookmark");
            this.HyperlinkValue = tr.GetAttribute("hyperlink");
            this.TagValue = tr.GetAttribute("tag");
            this.ToolTipValue = tr.GetAttribute("toolTip");

            value = tr.GetAttribute("pl");
            if (!string.IsNullOrWhiteSpace(value))
                this.ComponentPlacement = value;

            value = tr.GetAttribute("rc");
            if (value != null)
                this.ClientRectangle = (RectangleD)converter.StringToObject(value, typeof(RectangleD));

            value = tr.GetAttribute("bh");
            if (value != null)
                this.Brush = converter.StringToObject(value, typeof(StiBrush)) as StiBrush;

            value = tr.GetAttribute("br");
            if (value != null)
                this.Border = converter.StringToObject(value, typeof(StiBorder)) as StiBorder;
        }
        #endregion

        #region ICloneable override
        public override object Clone(bool cloneProperties)
        {
            return Clone(cloneProperties, true);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public virtual object Clone(bool cloneProperties, bool cloneComponents)
        {
            var cont = (StiContainer)base.Clone(cloneProperties);

            cont.containerInfoV1 = this.ContainerInfoV1.Clone() as StiContainerInfoV1;
            cont.containerInfoV2 = this.ContainerInfoV2.Clone() as StiContainerInfoV2;

            if (this.Border != null)
                cont.Border = (StiBorder)this.Border.Clone();
            else
                cont.Border = null;

            if (this.Brush != null)
                cont.Brush = (StiBrush)this.Brush.Clone();
            else
                cont.Brush = null;

            cont.components = new StiComponentsCollection(cont);
            if (cloneComponents)
            {
                lock (((ICollection)this.Components).SyncRoot)
                {
                    foreach (StiComponent comp in this.components)
                        cont.components.Add((StiComponent)comp.Clone());
                }
            }
            return cont;
        }
        #endregion

        #region IStiUnitConvert
        /// <summary>
        /// Converts a component out of one unit into another.
        /// </summary>
        /// <param name="oldUnit">Old units.</param>
        /// <param name="newUnit">New units.</param>
        public void Convert(StiUnit oldUnit, StiUnit newUnit, bool isReportSnapshot, bool convertComponents)
        {
            base.Convert(oldUnit, newUnit, isReportSnapshot);

            if (!convertComponents) return;

            lock (((ICollection)components).SyncRoot)
            {
                foreach (StiComponent component in components)
                    component.Convert(oldUnit, newUnit, isReportSnapshot);
            }
        }

        /// <summary>
        /// Converts a component out of one unit into another.
        /// </summary>
        /// <param name="oldUnit">Old units.</param>
        /// <param name="newUnit">New units.</param>
        public override void Convert(StiUnit oldUnit, StiUnit newUnit, bool isReportSnapshot = false)
        {
            Convert(oldUnit, newUnit, isReportSnapshot, true);
        }
        #endregion

        #region IStiBorder
        /// <summary>
        /// The appearance and behavior of the component border.
        /// </summary>
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBorder)]
        [StiSerializable]
        [Description("The appearance and behavior of the component border.")]
        public virtual StiBorder Border { get; set; } = new StiBorder();

        private bool ShouldSerializeBorder()
        {
            return Border == null || !Border.IsDefault;
        }
        #endregion

        #region IStiBrush
        /// <summary>
        /// The brush, which is used to draw background.
        /// </summary>
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBrush)]
        [StiSerializable]
        [Description("The brush, which is used to draw background.")]
        [TypeConverter(typeof(StiExpressionBrushConverter))]
        [Editor(StiEditors.ExpressionBrush, typeof(UITypeEditor))]
        [StiExpressionAllowed]
        public virtual StiBrush Brush { get; set; } = new StiSolidBrush(Color.Transparent);

        private bool ShouldSerializeBrush()
        {
            return !(Brush is StiSolidBrush && ((StiSolidBrush) Brush).Color == Color.Transparent);
        }
        #endregion

        #region IStiStateSaveRestore
        /// <summary>
        /// Saves the current state of an object.
        /// </summary>
        /// <param name="stateName">A name of the state being saved.</param>
        public override void SaveState(string stateName)
        {
            base.SaveState(stateName);
            States.Push(stateName, this, "CurrentComponent", this.ContainerInfoV1.CurrentComponent);

            this.Components.SaveState(stateName);
        }

        /// <summary>
        /// Restores the earlier saved object state.
        /// </summary>
        /// <param name="stateName">A name of the state being restored.</param>
        public override void RestoreState(string stateName)
        {
            base.RestoreState(stateName);

            if (States.IsExist(stateName, this))
                this.ContainerInfoV1.CurrentComponent = States.Pop(stateName, this, "CurrentComponent") as StiComponent;

            this.Components.RestoreState(stateName);
        }

        /// <summary>
        /// Clear all earlier saved object states.
        /// </summary>
        public override void ClearAllStates()
        {
            base.ClearAllStates();

            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent component in this.Components)
                    component.ClearAllStates();
            }
        }
        #endregion

        #region IStiGetActualSize
        public override SizeD GetActualSize()
        {
            var needSecondPass = false;
            return GetActualSize(false, ref needSecondPass);
        }

        internal SizeD GetActualSize(bool isFirstPass, ref bool needSecondPass)
        {
            var newSize = new SizeD(this.Width, this.Height);
            if (this.CanGrow || this.CanShrink)
            {
                double maxHeight = 0;
                double maxWidth = 0;

                if (!(this is StiPage))
                {
                    lock (((ICollection)this.Components).SyncRoot)
                    {
                        foreach (StiComponent comp in this.Components)
                        {
                            if (!comp.Enabled) continue;
                            if (comp is StiPointPrimitive)
                            {
                                maxHeight = Math.Max(comp.Top, maxHeight);
                                maxWidth = Math.Max(comp.Left, maxWidth);
                                continue;
                            }

                            if (comp is StiPrimitive && !(comp is StiHorizontalLinePrimitive)) continue;
                            if (!(comp.Width == 0 || comp.Height == 0))
                            {
                                var compSize = new SizeD(comp.ClientRectangle.Width, comp.ClientRectangle.Height);
                                if (comp.DockStyle != StiDockStyle.None && comp.Dockable)
                                {
                                    if (!isFirstPass)
                                    {
                                        comp.DockToContainer();

                                        if (StiOptions.Engine.AllowCacheForGetActualSize)
                                        {
                                            if (comp.Report != null && comp.Report.Engine != null && comp.Report.Engine.HashCheckSize != null)
                                            {
                                                comp.Report.Engine.HashCheckSize[comp] = null;
                                            }
                                        }

                                        compSize = comp.GetActualSize();
                                    }

                                    needSecondPass = true;
                                }

                                if (comp is StiHorizontalLinePrimitive)
                                    maxHeight = Math.Max(comp.Top, maxHeight);

                                else
                                    maxHeight = Math.Max(comp.Top + compSize.Height, maxHeight);

                                maxWidth = Math.Max(comp.Right, maxWidth);
                            }
                        }
                    }

                    if (this.CanGrow)
                    {
                        newSize.Width = Math.Max(maxWidth, newSize.Width);
                        newSize.Height = Math.Max(maxHeight, newSize.Height);
                    }

                    if (this.CanShrink)
                    {
                        newSize.Width = Math.Min(maxWidth, newSize.Width);
                        newSize.Height = Math.Min(maxHeight, newSize.Height);
                    }
                }
            }

            if (this is StiPage && (this as StiPage).UnlimitedHeight)
            {
                double maxHeight = 0;
                double maxWidth = 0;
                lock (((ICollection)this.Components).SyncRoot)
                {
                    foreach (StiComponent comp in this.Components)
                    {
                        if (!comp.Enabled) continue;
                        if (comp is StiPointPrimitive)
                        {
                            maxHeight = Math.Max(comp.Top, maxHeight);
                            maxWidth = Math.Max(comp.Left, maxWidth);
                            continue;
                        }

                        if (comp is StiPrimitive && !(comp is StiHorizontalLinePrimitive)) continue;
                        if (!(comp.Width == 0 || comp.Height == 0))
                        {
                            if (comp is StiHorizontalLinePrimitive)
                                maxHeight = Math.Max(comp.Top, maxHeight);
                            else
                                maxHeight = Math.Max(comp.Bottom, maxHeight);

                            maxWidth = Math.Max(comp.Right, maxWidth);
                        }
                    }
                }

                newSize.Width = Math.Max(maxWidth, newSize.Width);
                newSize.Height = Math.Max(maxHeight, newSize.Height);
            }

            return newSize;
        }
        #endregion

        #region IStiGetFonts
        public override List<StiFont> GetFonts()
        {
            var result = base.GetFonts();
            foreach (var component in Components)
            {
                if (component is IStiGetFonts)
                {
                    result.AddRange((component as IStiGetFonts).GetFonts());
                }
            }
            return result.Distinct().ToList();
        }
        #endregion

        #region Render
        private StiContainerInfoV1 containerInfoV1;
        [Browsable(false)]
        public StiContainerInfoV1 ContainerInfoV1
        {
            get
            {
                return containerInfoV1 ?? (containerInfoV1 = new StiContainerInfoV1());
            }
        }

        private StiContainerInfoV2 containerInfoV2;
        [Browsable(false)]
        public StiContainerInfoV2 ContainerInfoV2
        {
            get
            {
                return containerInfoV2 ?? (containerInfoV2 = new StiContainerInfoV2());
            }
        }

        /// <summary>
        /// Rendering a container without invokes events.
        /// </summary>
        /// <param name="renderedComponent">Rendered container.</param>
        /// <param name="outContainer">Container for rendering.</param>
        /// <returns>If the container is rendered completely then true.</returns>
        public virtual bool RenderContainer(ref StiComponent renderedComponent, StiContainer outContainer)
        {
            var builder = StiV1Builder.GetBuilder(this.GetType()) as StiContainerV1Builder;
            return builder.RenderContainer(this, ref renderedComponent, outContainer);
        }
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets a component priority.
        /// </summary>
        public override int Priority => (int)StiComponentPriority.Container;

        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.Container;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Components;

        /// <summary>
        /// Gets the type of processing when printing.
        /// </summary>
        public override StiComponentType ComponentType
        {
            get
            {
                if (containerInfoV2 != null && containerInfoV2.ParentBand != null)
                    return containerInfoV2.ParentBand.ComponentType;

                else
                    return StiComponentType.Simple;
            }
        }

        /// <summary>
        /// Gets a localized name of the component category.
        /// </summary>
        public override string LocalizedCategory => StiLocalization.Get("Report", "Components");

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiContainer");
        #endregion

        #region Properties
        /// <summary>
        /// If true, then the Parent component of this container is a Band.
        /// </summary>
        [Browsable(false)]
        internal bool ParentComponentIsBand { get; set; }

        /// <summary>
        /// If true, then the Parent component of this container is a CrossBand.
        /// </summary>
        [Browsable(false)]
        internal bool ParentComponentIsCrossBand { get; set; }

        /// <summary>
        /// Gets or sets collapsed value of the component.
        /// </summary>
        [Browsable(false)]
        public object CollapsedValue { get; set; }

        [Browsable(false)]
        public virtual int CollapsingIndex { get; set; }

        [Browsable(false)]
        public virtual string CollapsingTreePath { get; set; }

        /// <summary>
        /// Internal use only.
        /// </summary>
        [Browsable(false)]
        public bool HasSelected
        {
            get
            {
                lock (((ICollection)Components).SyncRoot)
                {
                    foreach (StiComponent comp in Components)
                    {
                        if (comp.IsSelected) return true;

                        var cont = comp as StiContainer;
                        if (cont != null && cont.HasSelected) return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets the default client area of a component.
        /// </summary>
        [Browsable(false)]
        public override RectangleD DefaultClientRectangle => new RectangleD(0, 0, 100, 100);

        private StiComponentsCollection components;
        /// <summary>
        /// Gets or sets a collection of components.
        /// </summary>
        [Browsable(false)]
        [StiSerializable(StiSerializationVisibility.List)]
        public virtual StiComponentsCollection Components
        {
            get
            {
                return components;
            }
            set
            {
                components = value;
            }
        }

        [Browsable(false)]
        public virtual StiCornerRadius CornerRadius { get; set; } = null;
        #endregion

        #region Methods.Block
        protected void CheckBlockedException(string propertyName)
        {
            if (!Blocked) return;

            throw new InvalidOperationException(
                $"You can't change property {propertyName} of {Name} component because since this property" +
                " presently blocked by operation of the report rendering. ");
        }

        protected static object PropertyBlocked = new object();

        /// <summary>
        /// internal use only. This property is used to block changing of band properties, 
        /// changing of which during band construction can distort the result.
        /// </summary>
        [Browsable(false)]
        protected internal virtual bool Blocked
        {
            get
            {
                return (bool)Properties.Get(PropertyBlocked, false);
            }
            set
            {
                Properties.Set(PropertyBlocked, value, false);
            }
        }
        #endregion

        #region Methods
        private void OnComponentAdded(object sender, EventArgs e)
        {
            OnComponentAdded(e);
        }

        protected virtual void OnComponentAdded(EventArgs e)
        {
        }

        private void OnComponentRemoved(object sender, EventArgs e)
        {
            OnComponentRemoved(e);
        }

        protected virtual void OnComponentRemoved(EventArgs e)
        {
        }

        protected internal void SetParentStylesToChilds()
        {
            StiBaseStyle style = null;

            var flag = true;
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in this.Components)
                {
                    if (comp.UseParentStyles)
                    {
                        if (flag)
                        {
                            style = StiBaseStyle.GetStyle(this);
                            flag = false;
                        }

                        if (style != null)
                            style.SetStyleToComponent(comp);
                    }
                }
            }
        }

        protected internal void SetParentStylesToChilds(StiBaseStyle style)
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in this.Components)
                {
                    if (comp.UseParentStyles)
                        style.SetStyleToComponent(comp);
                }
            }
        }

        public void OffsetLocation(double offsetX, double offsetY)
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    comp.Left += offsetX;
                    comp.Top += offsetY;
                }
            }
        }

        /// <summary>
        /// Changes the position of the selected component.
        /// </summary>
        /// <param name="delta">Parameters of changing.</param>
        public void ChangePosition(RectangleD delta)
        {
            var form = this as IStiForm;
            if (form != null && this.IsSelected)
            {
                form.Size = new Size(
                    Math.Max(form.Size.Width + (int)delta.Width, 112),
                    Math.Max(form.Size.Height + (int)delta.Height, 28));
            }

            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected && !(comp is IStiIgnoreOffset))
                        comp.DisplayRectangle = DoOffsetRect(comp, comp.DisplayRectangle, delta);

                    else
                    {
                        var cont = comp as StiContainer;
                        if (cont != null)
                            cont.ChangePosition(delta);
                    }
                }
            }
        }

        /// <summary>
        /// Normalizes all selected objects in the container.
        /// </summary>
        public void Normalize()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected)
                        comp.ClientRectangle = comp.ClientRectangle.Normalize();

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.Normalize();
                }
            }
        }

        /// <summary>
        /// Sets Link from all selected objects in the container. 
        /// </summary>
        public void SetLink(bool linked)
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected)
                        comp.Linked = linked;

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.SetLink(linked);
                }
            }
        }

        /// <summary>
        /// Returns linked of the object being met first.
        /// Scans all selected objects in the container. 
        /// </summary>
        /// <returns>Linked.</returns>
        public object GetLink()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected && StiRestrictionsHelper.IsAllowChange(comp))
                        return comp.Linked;

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var br = cont.GetLink();
                        if (br != null)
                            return br;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Sets Lock from all selected objects in the container. 
        /// </summary>
        public void SetLock(bool locked)
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected)
                        comp.Locked = locked;

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.SetLock(locked);
                }
            }
        }

        /// <summary>
        /// Returns lock of the object being met first.
        /// Scans all selected objects in the container. 
        /// </summary>
        /// <returns>Lock.</returns>
        public object GetLock()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected && StiRestrictionsHelper.IsAllowChange(comp))
                        return comp.Locked;

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var br = cont.GetLock();
                        if (br != null)
                            return br;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Sets StiBorder from all selected objects in the container.
        /// Sets Side, DropShadow, Color, Style. All other elements are ignored.
        /// </summary>
        public void SetBorder(StiBorder border)
        {
            if (IsSelected)
            {
                var br = (IStiBorder)this;
                br.Border.Side = border.Side;
                br.Border.DropShadow = border.DropShadow;
                br.Border.Color = border.Color;
                br.Border.Style = border.Style;
            }

            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected && comp is IStiSimpleBorder && !(comp is IStiHideBorderFromDesigner))
                    {
                        var br = (IStiBorder)comp;
                        br.Border.Side = border.Side;
                        br.Border.DropShadow = border.DropShadow;
                        br.Border.Color = border.Color;
                        br.Border.Style = border.Style;
                    }

                    else if (comp.IsSelected && comp is IStiBorder && !(comp is IStiHideBorderFromDesigner))
                    {
                        var br = (IStiBorder)comp;
                        br.Border.Side = border.Side;
                        br.Border.DropShadow = border.DropShadow;
                        br.Border.Color = border.Color;
                        br.Border.Style = border.Style;
                    }

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.SetBorder(border);
                }
            }
        }

        /// <summary>
        /// Returns a frame of the object of the type IStiBorder being met first. 
        /// Scans all selected objects in the container. 
        /// </summary>
        /// <returns>The frame of the object type IStiBorder found. If nothing is found then null is returned.</returns>
        public object GetBorder()
        {
            if (IsSelected)
                return this is IStiDashboard ? null : Border;

            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    var style = StiOptions.Designer.PropertyGrid.ShowPropertiesWhichUsedFromStyles ? null : comp.GetComponentStyle() as StiStyle;
                    if (comp.IsSelected && (comp is IStiBorder || comp is IStiSimpleBorder) && !(comp is IStiHideBorderFromDesigner) &&
                        (style == null || !(style.AllowUseBorderFormatting && style.AllowUseBorderSides)) && StiRestrictionsHelper.IsAllowChange(comp))
                    {
                        if (comp is StiGauge && ((StiGauge)comp).AllowApplyStyle)
                            return null;

                        if (comp is IStiSimpleBorder)
                            return ((IStiSimpleBorder)comp).Border;

                        else if (comp is IStiBorder)
                            return ((IStiBorder) comp).Border;
                    }

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var br = cont.GetBorder();
                        if (br != null)
                            return br;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a frame of the object of the type IStiBorderColor being met first. 
        /// Scans all selected objects in the container. 
        /// </summary>
        /// <returns>The frame of the object type IStiBorderColor found. If nothing is found then null is returned.</returns>
        public object GetBorderColor()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected && comp is IStiBorderColor borderColor && StiRestrictionsHelper.IsAllowChange(comp))
                        return borderColor.BorderColor;

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var br = cont.GetBorderColor();
                        if (br != null)
                            return br;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a frame of the object of the type IStiSimpleShadow being met first. 
        /// Scans all selected objects in the container. 
        /// </summary>
        /// <returns>The frame of the object type IStiSimpleShadow found. If nothing is found then null is returned.</returns>
        public object GetSimpleShadow()
        {
            if (IsSelected && this is IStiSimpleShadow)
                return (this as IStiSimpleShadow)?.Shadow;

            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected && comp is IStiSimpleShadow simpleShadow)
                        return simpleShadow.Shadow;

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var shadow = cont.GetSimpleShadow();
                        if (shadow != null)
                            return shadow;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Sets StiVertAlignment from all selected objects in the container.
        /// </summary>
        /// <param name="vl">StiVertAlignment being set.</param>
        public void SetVertAlign(StiVertAlignment vl)
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected && comp is IStiVertAlignment alignment)
                        alignment.VertAlignment = vl;

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.SetVertAlign(vl);
                }
            }
        }

        /// <summary>
        /// Returns a vertical alignment of the object of the type IStiVertAlignment being met first. 
        /// Scans all selected objects in the container. 
        /// </summary>
        /// <returns>Vertical alignment of the object of the type IStiVertAlignment found. If nothing is found then null is returned.</returns>
        public object GetVertAlignment()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp is StiTextInCells)
                        return null;

                    var style = StiOptions.Designer.PropertyGrid.ShowPropertiesWhichUsedFromStyles ? null : comp.GetComponentStyle() as StiStyle;
                    if (comp.IsSelected &&
                        comp is IStiVertAlignment &&
                        (style == null || !style.AllowUseVertAlignment) &&
                        StiRestrictionsHelper.IsAllowChange(comp))
                        return ((IStiVertAlignment)comp).VertAlignment;

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var tp = cont.GetVertAlignment();
                        if (tp != null)
                            return tp;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Sets StiHorAlignment from all selected objects in the container. 
        /// </summary>
        /// <param name="hl">StiHorAlignment being set.</param>
        public void SetHorAlign(StiHorAlignment hl)
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected)
                    {
                        #region DBS
                        var objs = comp.GetFormatObjects();
                        if (objs != null && objs.Count > 0)
                        {
                            objs.Where(o => o is IStiHorAlignment)
                                .Cast<IStiHorAlignment>().ToList()
                                .ForEach(o => o.HorAlignment = hl);
                        }
                        #endregion
                    }

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.SetHorAlign(hl);
                }
            }
        }

        /// <summary>
        /// Returns a horizontal alignment of the object of the type IStiHorAlignment being met first. 
        /// Scans all selected objects in the container. 
        /// </summary>
        /// <returns>Horizontal alignment of an object of the type IStiHorAlignment found. If nothing is found then null is returned.</returns>
        public object GetHorAlignment()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    var style = StiOptions.Designer.PropertyGrid.ShowPropertiesWhichUsedFromStyles
                        ? null
                        : comp.GetComponentStyle() as StiStyle;

                    #region DBS
                    if (comp.IsSelected && (style == null || !style.AllowUseHorAlignment) && StiRestrictionsHelper.IsAllowChange(comp))
                    {
                        var obj = comp.GetFirstFormatObject() as IStiHorAlignment;
                        if (obj != null)
                            return obj.HorAlignment;
                    }
                    #endregion

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var tp = cont.GetHorAlignment();
                        if (tp != null)
                            return tp;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Sets StiTextHorAlignment from all selected objects in the container.
        /// </summary>
        /// <param name="hl">StiTextHorAlignment being set.</param>
        public void SetTextHorAlign(StiTextHorAlignment hl)
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected && comp is IStiTextHorAlignment)
                        ((IStiTextHorAlignment)comp).HorAlignment = hl;

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.SetTextHorAlign(hl);
                }
            }
        }

        /// <summary>
        /// Returns a horizontal alignment of an object of the type IStiTextHorAlignment being met first. 
        /// Scans all selected objects in the container.
        /// </summary>
        /// <returns>Horizontal alignment of the object of the type IStiTextHorAlignment found. If nothing is found then null is returned.</returns>
        public object GetTextHorAlignment()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    var style = StiOptions.Designer.PropertyGrid.ShowPropertiesWhichUsedFromStyles
                        ? null
                        : comp.GetComponentStyle() as StiStyle;

                    if (comp.IsSelected &&
                        comp is IStiTextHorAlignment &&
                        (style == null || !style.AllowUseHorAlignment) &&
                        StiRestrictionsHelper.IsAllowChange(comp))
                        return ((IStiTextHorAlignment)comp).HorAlignment;

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var tp = cont.GetTextHorAlignment();
                        if (tp != null)
                            return tp;
                    }
                }
            }

            return null;
        }

        public void SetCrossHorAlign(StiCrossHorAlignment vl)
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected && comp is StiCrossTab)
                        ((StiCrossTab)comp).HorAlignment = vl;

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.SetCrossHorAlign(vl);
                }
            }
        }

        public object GetCrossHorAlignment()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    var style = StiOptions.Designer.PropertyGrid.ShowPropertiesWhichUsedFromStyles
                        ? null
                        : comp.GetComponentStyle() as StiStyle;

                    if (comp.IsSelected &&
                        comp is StiCrossTab &&
                        (style == null || !style.AllowUseHorAlignment) &&
                        StiRestrictionsHelper.IsAllowChange(comp))
                        return ((StiCrossTab)comp).HorAlignment;

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var tp = cont.GetTextHorAlignment();
                        if (tp != null)
                            return tp;
                    }
                }
            }

            return null;
        }

        public object GetWordWrap()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected && comp is IStiTextOptions && StiRestrictionsHelper.IsAllowChange(comp))
                        return ((IStiTextOptions)comp).TextOptions.WordWrap;

                    if (comp.IsSelected && comp is StiCrossTab && StiRestrictionsHelper.IsAllowChange(comp))
                        return ((StiCrossTab)comp).Wrap;

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var tp = cont.GetWordWrap();
                        if (tp != null)
                            return tp;
                    }
                }
            }

            return null;
        }

        public void SetWordWrap(bool wordWrap)
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected && comp is IStiTextOptions && StiRestrictionsHelper.IsAllowChange(comp))
                        ((IStiTextOptions)comp).TextOptions.WordWrap = wordWrap;

                    else if (comp.IsSelected && comp is StiCrossTab && StiRestrictionsHelper.IsAllowChange(comp))
                        ((StiCrossTab)comp).Wrap = wordWrap;

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.SetWordWrap(wordWrap);
                }
            }
        }

        public void SetFont(Font font)
        {
            SetFont(font, true, true, true, true, true, true, true, true, true);
        }

        /// <summary>
        /// Sets IStiFont from all selected objects in the container. 
        /// </summary>
        public void SetFont(Font font, bool setFontFamily, bool setSize, bool setStyleBold, bool setStyleItalic,
            bool setStyleUnderline, bool setStyleStrikeout,
            bool setUnit, bool setGdiCharSet, bool setGdiVerticalFont)
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected && (comp is IStiFont || comp is IStiTextFont))
                    {
                        var compFont = comp is IStiFont ? ((IStiFont)comp).Font : ((IStiTextFont)comp).GetFont();

                        var fontFamily = setFontFamily ? font.FontFamily : compFont.FontFamily;
                        var size = setSize ? font.Size : compFont.Size;

                        var styleBold = setStyleBold ? font.Bold : compFont.Bold;
                        var styleItalic = setStyleItalic ? font.Italic : compFont.Italic;
                        var styleUnderline = setStyleUnderline ? font.Underline : compFont.Underline;
                        var styleStrikeout = setStyleStrikeout ? font.Strikeout : compFont.Strikeout;

                        var gdiCharSet = setGdiCharSet ? font.GdiCharSet : compFont.GdiCharSet;
                        var gdiVerticalFont = setGdiVerticalFont ? font.GdiVerticalFont : compFont.GdiVerticalFont;
                        var unit = setUnit ? font.Unit : compFont.Unit;

                        if (!(
                            compFont.FontFamily == fontFamily &&
                            compFont.Size == size &&
                            compFont.Bold == styleBold &&
                            compFont.Italic == styleItalic &&
                            compFont.Underline == styleUnderline &&
                            compFont.Strikeout == styleStrikeout &&
                            compFont.GdiCharSet == gdiCharSet &&
                            compFont.GdiVerticalFont == gdiVerticalFont &&
                            compFont.Unit == unit))
                        {
                            var style = (FontStyle)0;

                            if (styleBold)
                                style |= FontStyle.Bold;

                            if (styleItalic)
                                style |= FontStyle.Italic;

                            if (styleUnderline)
                                style |= FontStyle.Underline;

                            if (styleStrikeout)
                                style |= FontStyle.Strikeout;

                            if (comp is IStiFont)
                                ((IStiFont)comp).Font = new Font(fontFamily, size, StiFontUtils.CorrectStyle(fontFamily.Name, style), unit, gdiCharSet, gdiVerticalFont);

                            else
                            {
                                var textFont = comp as IStiTextFont;

                                if (setFontFamily)
                                    textFont.SetFontName(fontFamily.Name);

                                if (setSize)
                                    textFont.SetFontSize(size);

                                if (setStyleBold)
                                    textFont.SetFontBoldStyle(styleBold);

                                if (setStyleItalic)
                                    textFont.SetFontItalicStyle(styleItalic);

                                if (setStyleUnderline)
                                    textFont.SetFontUnderlineStyle(styleUnderline);
                            }
                        }

                        if (comp is IStiTextFont && compFont != null)
                            compFont.Dispose();
                    }

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        cont.SetFont(
                            font, setFontFamily, setSize, setStyleBold, setStyleItalic, setStyleUnderline,
                            setStyleStrikeout,
                            setUnit, setGdiCharSet, setGdiVerticalFont);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a font of an object of the type IStiFont being met first. 
        /// Scans all selected objects in the container. 
        /// </summary>
        /// <returns>The font of an object of the type IStiFont found. If nothing is found then null is returned.</returns>
        public object GetFont()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    var style = StiOptions.Designer.PropertyGrid.ShowPropertiesWhichUsedFromStyles ? null : comp.GetComponentStyle() as StiStyle;
                    if (comp.IsSelected && (style == null || !style.AllowUseFont) && StiRestrictionsHelper.IsAllowChange(comp))
                    {
                        if (comp is IStiFont)
                            return ((IStiFont)comp).Font;

                        #region DBS
                        if (comp is IStiTextFont)
                            return ((IStiTextFont)comp).GetFont();
                        #endregion
                    }

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var fnt = cont.GetFont();
                        if (fnt != null)
                            return fnt;
                    }
                }
            }

            return null;
        }

        public void ChangeFontName(string fontName)
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected)
                    {
                        if (comp is IStiFont && comp.IsSelected)
                            ((IStiFont)comp).Font = StiFontUtils.ChangeFontName(((IStiFont)comp).Font, fontName);

                        if (comp is IStiTextFont && comp.IsSelected)
                            ((IStiTextFont)comp).SetFontName(fontName);
                    }

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.ChangeFontName(fontName);
                }
            }
        }

        public void ChangeFontSize(float fontSize)
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp is IStiFont && comp.IsSelected)
                        ((IStiFont)comp).Font = StiFontUtils.ChangeFontSize(((IStiFont)comp).Font, fontSize);

                    if (comp is IStiTextFont && comp.IsSelected)
                        ((IStiTextFont)comp).SetFontSize(fontSize);

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.ChangeFontSize(fontSize);
                }
            }
        }

        public void GrowFont()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp is IStiFont && comp.IsSelected)
                        ((IStiFont)comp).Font =
                            StiFontUtils.ChangeFontSize(((IStiFont)comp).Font, ((IStiFont)comp).Font.Size + 1);

                    #region DBS
                    if (comp is IStiTextFont && comp.IsSelected)
                        ((IStiTextFont)comp).GrowFontSize();
                    #endregion

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.GrowFont();
                }
            }
        }

        public void ShrinkFont()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp is IStiFont && comp.IsSelected && ((IStiFont)comp).Font.Size - 1 > 0)
                        ((IStiFont)comp).Font =
                            StiFontUtils.ChangeFontSize(((IStiFont)comp).Font, ((IStiFont)comp).Font.Size - 1);

                    #region DBS
                    if (comp is IStiTextFont && comp.IsSelected)
                        ((IStiTextFont)comp).ShrinkFontSize();
                    #endregion

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.ShrinkFont();
                }
            }
        }

        /// <summary>
		/// Sets IStiBrush from all selected objects in the container. 
		/// </summary>
		public void SetBrush(StiBrush brush)
        {
            if (IsSelected)
                this.Brush = (StiBrush)brush.Clone();

            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected && comp is IStiBrush)
                    {
                        var br = (IStiBrush)comp;
                        br.Brush = (StiBrush)brush.Clone();
                    }

                    if (comp.IsSelected && comp is IStiBackColor)
                    {
                        var br = (IStiBackColor)comp;
                        br.BackColor = StiBrush.ToColor(brush);
                    }

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.SetBrush(brush);
                }
            }
        }

        /// <summary>
        /// Returns a brush of an object of the type StiBrush being met first. 
        /// Scans all selected objects in the container.
        /// </summary>
        /// <returns>The brush of an object of the type IStiBrush found. If nothing is found then null is returned.</returns>
        public object GetBrush()
        {
            if (IsSelected)
                return this.Brush;

            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    var style = StiOptions.Designer.PropertyGrid.ShowPropertiesWhichUsedFromStyles ? null : comp.GetComponentStyle() as StiStyle;
                    if (comp.IsSelected && (comp is IStiBrush || comp is IStiBackColor) &&
                        (style == null || !style.AllowUseBrush) &&
                        StiRestrictionsHelper.IsAllowChange(comp))
                    {
                        if (comp is StiGauge && ((StiGauge)comp).AllowApplyStyle)
                            return null;

                        if (comp is IStiBackColor)
                            return new StiSolidBrush(((IStiBackColor)comp).BackColor);
                        else
                            return ((IStiBrush)comp).Brush;
                    }

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var br = cont.GetBrush();
                        if (br != null)
                            return br;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Sets StiFormatService from all selected objects in the container. 
        /// </summary>
        /// <param name="format">Format being set.</param>
        public void SetTextFormat(StiFormatService format)
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected)
                    {
                        #region DBS
                        var objs = comp.GetFormatObjects();
                        if (objs != null && objs.Count > 0)
                        {
                            objs.Where(o => o is IStiTextFormat)
                                .Cast<IStiTextFormat>().ToList()
                                .ForEach(o => o.TextFormat = (StiFormatService)format.Clone());
                        }
                        #endregion
                    }

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.SetTextFormat(format);
                }
            }
        }

        /// <summary>
        /// Returns a format of an object of the type IStiText being met first.  
        /// Scans all selected objects in the container. 
        /// </summary>
        /// <returns>The format of an object of the type IStiText found. If nothing is found then null is returned.</returns>
        public StiFormatService GetTextFormat()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected && StiRestrictionsHelper.IsAllowChange(comp) && !(comp is IStiButtonElement))
                    {
                        #region DBS
                        var obj = comp.GetFirstFormatObject() as IStiTextFormat;
                        if (obj != null)
                            return obj.TextFormat;
                        #endregion
                    }

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var format = cont.GetTextFormat();
                        if (format != null)
                            return format;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Sets IStiTextBrush from all selected objects in the container. 
        /// </summary>
        /// <param name="brush">TextBrush being set.</param>
        public void SetTextBrush(StiBrush brush)
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected && comp is IStiTextBrush)
                    {
                        #region DBS
                        var objs = comp.GetFormatObjects();
                        if (objs != null && objs.Count > 0)
                        {
                            objs.Where(o => o is IStiTextBrush)
                                .Cast<IStiTextBrush>().ToList()
                                .ForEach(o => o.TextBrush = brush);
                            continue;
                        }
                        #endregion

                        var br = (IStiTextBrush)comp;
                        br.TextBrush = (StiBrush)brush.Clone();
                    }

                    if (comp.IsSelected && comp is IStiForeColor)
                    {
                        #region DBS
                        var objs = comp.GetFormatObjects();
                        if (objs != null && objs.Count > 0)
                        {
                            objs.Where(o => o is IStiForeColor)
                                .Cast<IStiForeColor>().ToList()
                                .ForEach(o => o.ForeColor = StiBrush.ToColor(brush));
                            continue;
                        }
                        #endregion

                        var br = (IStiForeColor)comp;
                        br.ForeColor = StiBrush.ToColor(brush);
                    }

                    #region DBS
                    var objFirstFormat = comp.GetFirstFormatObject() as IStiForeColor;
                    if (comp.IsSelected && objFirstFormat != null)
                    {
                        objFirstFormat.ForeColor = StiBrush.ToColor(brush);
                    }
                    #endregion

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.SetTextBrush(brush);
                }
            }
        }

        /// <summary>
        /// Returns a brush of an object of the type IStiTextBrush being met first. 
        /// Scans all selected objects in the container. 
        /// </summary>
        /// <returns>The brush of an object of the type IStiTextBrush found. 
        /// If nothing is found then null is returned.</returns>
        public object GetTextBrush()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    var style = StiOptions.Designer.PropertyGrid.ShowPropertiesWhichUsedFromStyles ? null : comp.GetComponentStyle() as StiStyle;
                    var objFirstFormat = comp.GetFirstFormatObject() as IStiForeColor;
                    if (comp.IsSelected && (comp is IStiTextBrush || comp is IStiForeColor || objFirstFormat != null) &&
                        (style == null || !style.AllowUseTextBrush) && StiRestrictionsHelper.IsAllowChange(comp))
                    {
                        #region DBS                        
                        if (objFirstFormat != null)
                            return new StiSolidBrush(objFirstFormat.ForeColor);
                        #endregion

                        if (comp is IStiForeColor)
                            return new StiSolidBrush(((IStiForeColor)comp).ForeColor);
                        else
                            return ((IStiTextBrush)comp).TextBrush;
                    }

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var br = cont.GetTextBrush();
                        if (br != null)
                            return br;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Sets conditions to all selected objects in the container. 
        /// </summary>
        public void SetConditions(StiConditionsCollection conditions)
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected)
                        comp.Conditions = conditions.Clone() as StiConditionsCollection;

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.SetConditions(conditions);
                }
            }
        }

        /// <summary>
        /// Returns conditions of an object of the type. 
        /// Scans all selected objects in the container. 
        /// </summary>
        public StiConditionsCollection GetConditions()
        {
            var conditions = new StiConditionsCollection();

            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected &&
                        comp.Conditions.Count > 0 &&
                        StiRestrictionsHelper.IsAllowChange(comp))
                    {
                        conditions.AddRange(comp.Conditions, true);
                    }

                    var cont = comp as StiContainer;
                    if (cont != null)
                        conditions.AddRange(cont.GetConditions(), true);
                }
            }

            return conditions;
        }

        /// <summary>
        /// Sets Dock from all selected objects in the container.
        /// </summary>
        /// <param name="dockStyle">DockStyle being set.</param>
        public void SetDockStyle(StiDockStyle dockStyle)
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected && !comp.IsAutomaticDock) comp.DockStyle = dockStyle;

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.SetDockStyle(dockStyle);
                }
            }
        }

        /// <summary>
        /// Returns the style of docking of the component being met first.
        /// Scans all selected objects in the container. 
        /// </summary>
        /// <returns>The style of docking of the component found. If nothing is found then null is returned.</returns>
        public object GetDockStyle()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected &&
                        !comp.IsAutomaticDock &&
                        StiRestrictionsHelper.IsAllowChange(comp))
                        return comp.DockStyle;

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var dock = cont.GetDockStyle();
                        if (dock != null)
                            return dock;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// The rectangle includes all selected objects in the container and 
        /// children containers which are located in the selected containers.
        /// The rectangle is represented in page coordinates.
        /// </summary>
        /// <returns>The rectangle includes all selected objects.</returns>
        public RectangleD GetSelectedRectangleWithChilds()
        {
            var rect = RectangleD.Empty;
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected)
                    {
                        rect = rect.IsEmpty
                            ? comp.GetDisplayRectangle()
                            : rect.FitToRectangle(comp.GetDisplayRectangle());
                    }

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var contRect = comp.IsSelected
                            ? cont.GetRectangle()
                            : cont.GetSelectedRectangleWithChilds();

                        if (!contRect.IsEmpty)
                            rect = rect.FitToRectangle(contRect);
                    }
                }
            }

            return rect;
        }

        /// <summary>
        /// Returns the rectangle that includes all selected objects in the container. 
        /// The rectangle is represented in page coordinates.
        /// </summary>
        /// <returns>The rectangle includes all selected objects.</returns>
        public RectangleD GetSelectedRectangle()
        {
            var rect = RectangleD.Empty;
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected)
                    {
                        rect = rect.IsEmpty
                            ? comp.GetDisplayRectangle()
                            : rect.FitToRectangle(comp.GetDisplayRectangle());
                    }

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var contRect = cont.GetSelectedRectangle();
                        if (!contRect.IsEmpty)
                            rect = rect.FitToRectangle(contRect);
                    }
                }
            }

            return rect;
        }

        /// <summary>
        /// The rectangle includes all selected objects in the container.
        /// The rectangle is represented in page coordinates.
        /// </summary>
        /// <returns>The rectangle includes all objects.</returns>
        public RectangleD GetRectangle()
        {
            var rect = ComponentToPage(this.DisplayRectangle);
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    rect = rect.IsEmpty
                        ? ContainerToPage(comp.DisplayRectangle)
                        : rect.FitToRectangle(ContainerToPage(comp.DisplayRectangle));

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var contRect = cont.GetRectangle();
                        if (!contRect.IsEmpty)
                            rect = rect.FitToRectangle(contRect);
                    }
                }
            }

            return rect;
        }

        /// <summary>
        /// Returns maximum sizes among selected objects.
        /// </summary>
        /// <returns>Maximum sizes.</returns>
        public SizeD GetMaxSize()
        {
            var size = new SizeD(0, 0);
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected)
                    {
                        if (size.Width < comp.Width)
                            size.Width = comp.Width;

                        if (size.Height < comp.Height)
                            size.Height = comp.Height;
                    }

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var cnSize = cont.GetMaxSize();

                        if (size.Width < cnSize.Width)
                            size.Width = cnSize.Width;

                        if (size.Height < cnSize.Height)
                            size.Height = cnSize.Height;
                    }
                }
            }

            return size;
        }

        /// <summary>
        /// Sets the size to all selected objects.
        /// </summary>
        /// <param name="size">Size to be set.</param>
        public void MakeSameSize(SizeD size)
        {
            var comps = this.GetSelectedComponents();
            lock (((ICollection)comps).SyncRoot)
            {
                foreach (StiComponent comp in comps)
                {
                    comp.Width = size.Width;
                    comp.Height = size.Height;
                }
            }
        }

        /// <summary>
        /// Sets the width to all selected objects.
        /// </summary>
        /// <param name="width">Width to be set.</param>
        public void MakeSameWidth(double width)
        {
            var comps = this.GetSelectedComponents();
            lock (((ICollection)comps).SyncRoot)
            {
                foreach (StiComponent comp in comps)
                {
                    comp.Width = width;
                }
            }
        }

        /// <summary>
        /// Sets the height to all selected objects.
        /// </summary>
        /// <param name="height">Height to be set.</param>
        public void MakeSameHeight(double height)
        {
            var comps = this.GetSelectedComponents();
            lock (((ICollection)comps).SyncRoot)
            {
                foreach (StiComponent comp in comps)
                {
                    comp.Height = height;
                }
            }
        }

        /// <summary>
        /// Puts components which are equivalent to their width.
        /// </summary>
        public void MakeHorizontalSpacingEqual()
        {
            var comps = GetSelectedComponents();
            var oldRectCenter = GetSelectedRectangle();

            double totalWidth = 0;
            lock (((ICollection)comps).SyncRoot)
            {
                foreach (StiComponent comp in comps)
                {
                    totalWidth += comp.Width;
                }
            }

            var dist = (oldRectCenter.Width - totalWidth) / Math.Max(comps.Count - 1, 1);

            comps.SortByLeftPosition();
            var pos = comps[0].Right + dist;
            for (var index = 1; index < comps.Count - 1; index++)
            {
                comps[index].Left = pos;
                pos = comps[index].Right + dist;
            }
        }

        /// <summary>
        /// Puts components which are equal in their height.
        /// </summary>
        public void MakeVerticalSpacingEqual()
        {
            var comps = GetSelectedComponents();
            var oldRectCenter = GetSelectedRectangle();

            double totalHeight = 0;
            lock (((ICollection)comps).SyncRoot)
            {
                foreach (StiComponent comp in comps)
                {
                    totalHeight += comp.Height;
                }
            }

            var dist = (oldRectCenter.Height - totalHeight) / Math.Max(comps.Count - 1, 1);

            comps.SortByTopPosition();
            var pos = comps[0].Bottom + dist;
            for (var index = 1; index < comps.Count - 1; index++)
            {
                comps[index].Top = pos;
                pos = comps[index].Bottom + dist;
            }
        }

        /// <summary>
        /// Centers horizontally all selected objects.
        /// </summary>
        public void SetCenterHorizontally()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected)
                        comp.Left = (comp.Parent.Width - comp.Width) / 2;

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.SetCenterHorizontally();
                }
            }
        }

        /// <summary>
        /// Centers vertically all selected objects.
        /// </summary>
        public void SetCenterVertically()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected)
                        comp.Top = (comp.Parent.Height - comp.Height) / 2;

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.SetCenterVertically();
                }
            }
        }

        /// <summary>
        /// Aligns to grid all selected components in the container.
        /// </summary>
        public void AlignToGrid()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected &&
                        !comp.Locked &&
                        !comp.Inherited &&
                        StiRestrictionsHelper.IsAllowChangePosition(comp))
                    {
                        comp.ClientRectangle = comp.ClientRectangle.AlignToGrid(
                            Page.GridSize, true);
                    }

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.AlignToGrid();
                }
            }
        }

        public void AlignToGrid(StiComponent comp)
        {
            if (comp.IsSelected &&
                !comp.Locked &&
                !comp.Inherited &&
                StiRestrictionsHelper.IsAllowChangePosition(comp))
            {
                comp.ClientRectangle = comp.ClientRectangle.AlignToGrid(
                    Page.GridSize, true);
            }

            var cont = comp as StiContainer;
            if (cont != null)
                cont.AlignToGrid();
        }

        public void SortByPriority()
        {
            this.Components.SortByPriority();
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.SortByPriority();
                }
            }
        }

        /// <summary>
        /// Aligns, in the rectangle of selected objects, all selected objects.
        /// </summary>
        /// <param name="aligning">Type of aligning.</param>
        public void AlignTo(StiAligning aligning)
        {
            var comps = GetSelectedComponents();

            #region Check Method Align
            var alignBySelectionTick = true;
            for (var index = 0; index < comps.Count; index++)
            {
                for (var indexCompare = 0; indexCompare < comps.Count; indexCompare++)
                {
                    if (index == indexCompare) continue;
                    if (comps[index].SelectionTick == comps[indexCompare].SelectionTick)
                    {
                        alignBySelectionTick = false;
                        break;
                    }
                }
                if (!alignBySelectionTick) break;
            }
            #endregion

            var compsSortBySelectionTick = (StiComponentsCollection)comps.Clone();
            compsSortBySelectionTick.SortBySelectionTick();

            var alignRectangle = alignBySelectionTick ? compsSortBySelectionTick[0].ClientRectangle : GetSelectedRectangle();

            switch (aligning)
            {
                #region StiAligning.Left
                case StiAligning.Left:
                    comps.SortByLeftPosition();
                    for (var index = 0; index < comps.Count; index++)
                    {
                        comps[index].Left = alignRectangle.Left;
                    }
                    break;
                #endregion

                #region StiAligning.Center
                case StiAligning.Center:
                    for (var index = 0; index < comps.Count; index++)
                    {
                        comps[index].Left = alignRectangle.Left + (alignRectangle.Width - comps[index].Width) / 2;
                    }
                    break;
                #endregion

                #region StiAligning.Right
                case StiAligning.Right:
                    comps.SortByRightPosition();
                    for (var index = 0; index < comps.Count; index++)
                        comps[index].Left = alignRectangle.Right - comps[index].Width;
                    break;
                #endregion

                #region StiAligning.Top
                case StiAligning.Top:
                    comps.SortByTopPosition();
                    for (var index = 0; index < comps.Count; index++)
                        comps[index].Top = alignRectangle.Top;
                    break;
                #endregion

                #region StiAligning.Middle
                case StiAligning.Middle:
                    for (var index = 0; index < comps.Count; index++)
                    {
                        comps[index].Top = alignRectangle.Top + (alignRectangle.Height - comps[index].Height) / 2;
                    }
                    break;
                #endregion

                #region StiAligning.Bottom
                case StiAligning.Bottom:
                    comps.SortByBottomPosition();
                    for (var index = 0; index < comps.Count; index++)
                        comps[index].Top = alignRectangle.Bottom - comps[index].Height;
                    break;
                    #endregion
            }
        }

        /// <summary>
        /// Brings to front objects of the list in the container.
        /// </summary>
        public void BringToFront()
        {
            var count = Components.Count;
            var k = 0;
            while (k < count)
            {
                var comp = Components[k];

                var cont = comp as StiContainer;
                if (cont != null)
                    cont.BringToFront();

                if (comp.IsSelected)
                {
                    Components.Remove(comp);
                    Components.Add(comp);
                    count--;
                }
                else k++;
            }
            SortByPriority();
        }

        /// <summary>
        /// Sends to back objects of the list in the container.
        /// </summary>
        public void SendToBack()
        {
            var count = Components.Count;
            var k = 0;
            while (k < count)
            {
                var comp = Components[k];

                var cont = comp as StiContainer;
                if (cont != null)
                    cont.SendToBack();

                if (comp.IsSelected)
                {
                    Components.Remove(comp);
                    Components.Insert(0, comp);
                }
                k++;
            }
            SortByPriority();
        }

        /// <summary>
        /// Moves forward objects of the list in the container.
        /// </summary>
        public void MoveForward()
        {
            var k = Components.Count - 1;
            while (k >= 0)
            {
                var comp = Components[k];

                var cont = comp as StiContainer;
                if (cont != null)
                    cont.MoveForward();

                if (comp.IsSelected)
                {
                    var pos = Components.IndexOf(comp);
                    if (pos < Components.Count - 1)
                    {
                        Components.Remove(comp);
                        Components.Insert(pos + 1, comp);
                    }
                }
                k--;
            }
            SortByPriority();
        }

        /// <summary>
        /// Moves backward objects of the list in the container.
        /// </summary>
        public void MoveBackward()
        {
            var count = Components.Count;
            var k = 0;
            while (k < count)
            {
                var comp = Components[k];

                var cont = comp as StiContainer;
                if (cont != null) cont.MoveBackward();

                if (comp.IsSelected)
                {
                    var pos = Components.IndexOf(comp);
                    if (pos > 0)
                    {
                        Components.Remove(comp);
                        Components.Insert(pos - 1, comp);
                    }
                }
                k++;
            }
            SortByPriority();
        }

        /// <summary>
        /// Returns an object and controls of actions for the object in the specified position.
        /// </summary>
        /// <param name="obj">Object being checked.</param>
        /// <param name="action">Returns the type of action in the variable of the specified object position.</param>
        /// <param name="x">Coordinate X for checking.</param>
        /// <param name="y">Coordinate Y for checking.</param>
        private void ControlInPoint(ref StiComponent obj, ref StiAction action, double x, double y, StiGuiMode guiMode, bool forGetDesigner = false)
        {
            var pos = PageToContainer(new PointD(x, y));

            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    var compDesigner = StiComponentDesigner.GetComponentDesigner(null, comp.GetType(), guiMode);
                    if (compDesigner == null) continue;

                    var actionObj = forGetDesigner ? compDesigner.GetActionFromPoint(pos.X, pos.Y, comp, true) : compDesigner.GetActionFromPoint(pos.X, pos.Y, comp);

                    if (actionObj != StiAction.None &&
                        (actionObj != StiAction.Move || actionObj == StiAction.Move &&
                         action == StiAction.Move || action == StiAction.None))
                    {
                        action = actionObj;
                        obj = comp;
                    }

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.ControlInPoint(ref obj, ref action, x, y, guiMode, forGetDesigner);
                }
            }
        }

        /// <summary>
		/// Returns an object in which zone of controlling coordinates are got.
		/// </summary>
		/// <param name="x">Coordinate X for checking.</param>
		/// <param name="y">Coordinate Y for checking.</param>
		/// <param name="usePage">Checking is fulfiled taking a page into consideration.</param>
        public StiComponent GetComponentInControl(double x, double y, bool usePage)
        {
            return GetComponentInControl(x, y, usePage, StiGuiMode.Gdi, false);
        }

        /// <summary>
		/// Returns an object in which zone of controlling coordinates are got.
		/// </summary>
		/// <param name="x">Coordinate X for checking.</param>
		/// <param name="y">Coordinate Y for checking.</param>
		/// <param name="usePage">Checking is fulfiled taking a page into consideration.</param>
		public StiComponent GetComponentInControl(double x, double y, bool usePage, StiGuiMode guiMode)
        {
            return GetComponentInControl(x, y, usePage, guiMode, false);
        }

        /// <summary>
        /// Returns an object in which zone of controlling coordinates are got.
        /// </summary>
        /// <param name="x">Coordinate X for checking.</param>
        /// <param name="y">Coordinate Y for checking.</param>
        /// <param name="usePage">Checking is fulfiled taking a page into consideration.</param>
        /// <param name="forGetDesigner">True for call from GetComponentDesigner method.</param>
        public StiComponent GetComponentInControl(double x, double y, bool usePage, StiGuiMode guiMode, bool forGetDesigner = false)
        {
            StiComponent obj = null;
            var action = StiAction.None;

            ControlInPoint(ref obj, ref action, x, y, guiMode, forGetDesigner);
            if (obj == null && usePage)
                return this.Page;

            return obj;
        }

        /// <summary>
        /// Returns an object in which zone of controlling coordinates are got.
        /// </summary>
        /// <param name="x">Coordinate X for checking.</param>
        /// <param name="y">Coordinate Y for checking.</param>
        public StiComponent GetComponentInPoint(double x, double y)
        {
            StiComponent obj = null;

            var pos = PageToContainer(new PointD(x, y));

            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (StiActionUtils.PointInRect(pos.X, pos.Y, comp.ClientRectangle))
                        obj = comp;

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var comp2 = cont.GetComponentInPoint(x, y);
                        if (comp2 != null)
                            obj = comp2;
                    }
                }
            }

            return obj;
        }


        /// <summary>
        /// Returns an container in which client zone coordinates are got.
        /// </summary>
        /// <param name="component">The component for which positions are being looked for.</param>
        private StiContainer getContainerInRect(RectangleD rect, StiComponent component)
        {
            StiContainer obj = null;

            var pos = this.PageToContainer(rect);

            var Comps = this.GetComponents();
            for (var index = Comps.Count - 1; index >= 0; index--)
            {
                var comp = Comps[index];

                if (comp != component && !comp.CheckForParentComponent(component))
                {
                    var cont = comp as StiContainer;
                    if (cont != null && component.CanContainIn(cont))
                    {
                        var compRect = this.PageToContainer(comp.ComponentToPage(comp.ClientRectangle));

                        if (compRect.IntersectsWith(pos))
                        {
                            if (component.IsCross)
                                obj = cont;
                        }

                        if (comp is StiBand && !comp.IsCross)
                        {
                            var size = Page.Unit.ConvertFromHInches(1d);
                            if (component is StiPrimitive) compRect.Width += size;
                            if (Report != null && Report.Info.ShowHeaders)
                            {
                                compRect.Height += size;
                                var headerSize = Page.Unit.ConvertFromHInches((comp as StiBand).HeaderSize);
                                compRect = compRect.OffsetRect(new RectangleD(0, headerSize, 0, headerSize));
                            }
                        }
                        if (component is StiEndPointPrimitive || component is StiStartPointPrimitive)
                        {
                            var posLeft = Math.Round((decimal)pos.X, 2);
                            var posTop = Math.Round((decimal)pos.Y, 2);

                            var compLeft = Math.Round((decimal)compRect.Left, 2);
                            var compTop = Math.Round((decimal)compRect.Top, 2);
                            var compRight = Math.Round((decimal)compRect.Right, 2);
                            var compBottom = Math.Round((decimal)compRect.Bottom, 2);

                            if (compTop <= posTop && compBottom >= posTop &&
                                compLeft <= posLeft && compRight >= posLeft) obj = cont;
                        }

                        if (StiActionUtils.PointInRect(pos.X, pos.Y, compRect))
                            obj = cont;

                        var container = cont.getContainerInRect(rect, component);
                        if (container != null)
                            obj = container;
                    }
                }
            }
            if (component.IsCross) return obj;

            if (component is StiTable && obj is StiTable) return null;
            if (component is StiTable && obj is StiDataBand) return obj;
            if (!(component is IStiTableCell) && obj is StiTable) return null;
            if (component is IStiTableCell && !(obj is StiTable)) return null;
            if (component is StiBand && obj is StiBand) return null;

            return obj;
        }

        /// <summary>
        /// Returns an container in which client zone coordinates are got.
        /// </summary>
        /// <param name="component">The component for which positions are being looked for.</param>
        public StiContainer GetContainerInRect(RectangleD rect, StiComponent component)
        {
            //take into account the height of the band header
            if (component.Page != null && component is StiBand)
            {
                var headerSize = component.Page.Unit.ConvertFromHInches((component as StiBand).HeaderSize);
                rect = rect.OffsetRect(new RectangleD(0, headerSize, 0, headerSize));
            }

            //get the container in which the component falls
            var container = getContainerInRect(rect, component);

            //if null - the component hits the page
            if (container == null) return Page;

            //if there are only nested containers in this area, then the component gets to the page
            if (container.CheckForParentComponent(component))
                return Page;

            return container;
        }

        /// <summary>
        /// Returns the list of all objects which require motion between containers.
        /// </summary>
        /// <returns>List of objects.</returns>
        public StiComponentsCollection GetIncorrect()
        {
            return GetIncorrect(false);
        }

        private Hashtable GetSizesTable(StiComponent component)
        {
            var hash = new Hashtable();
            var rect = new RectangleD();
            GetNodeSize(hash, component, ref rect);
            return hash;
        }

        private void GetNodeSize(Hashtable hash, StiComponent component, ref RectangleD baseRect)
        {
            var rect = ContainerToPage(component.ClientRectangle);

            if (component.Page != null && component is StiBand && !component.IsCross)
            {
                var headerSize = component.Page.Unit.ConvertFromHInches((component as StiBand).HeaderSize);
                rect = rect.OffsetRect(new RectangleD(0, headerSize, 0, headerSize));

                //fix, for StiPrimitives
                var correction = Page.Unit.ConvertFromHInches(1d);
                rect.Width += correction;

                if (Report != null && Report.Info.ShowHeaders)
                    rect.Height += correction;
            }

            var cont = component as StiContainer;
            if (cont != null)
            {
                foreach (StiComponent comp in cont.Components)
                {
                    cont.GetNodeSize(hash, comp, ref rect);
                }
            }

            if (rect.X < baseRect.X)
            {
                baseRect.Width += baseRect.X - rect.X;
                baseRect.X = rect.X;
            }

            if (rect.Y < baseRect.Y)
            {
                baseRect.Height += baseRect.Y - rect.Y;
                baseRect.Y = rect.Y;
            }

            if (rect.X + rect.Width > baseRect.X + baseRect.Width)
                baseRect.Width = rect.X + rect.Width - baseRect.X;

            if (rect.Y + rect.Height > baseRect.Y + baseRect.Height)
                baseRect.Height = rect.Y + rect.Height - baseRect.Y;

            hash[component] = rect;
        }

        private StiContainer getContainerInRect2(RectangleD rect, StiComponent component, Hashtable hash)
        {
            StiContainer obj = null;

            //RectangleD size2 = this.ContainerToPage(rect);
            var size2 = (RectangleD)hash[this];

            if (!StiActionUtils.PointInRect(rect.X, rect.Y, size2)) return null;
            if (this == component || this.CheckForParentComponent(component)) return null;

            var comps = this.Components;
            for (var index = comps.Count - 1; index >= 0; index--)
            {
                var comp = comps[index];

                if (comp == component) continue;

                if (comp.Width == 0 || comp.Height == 0)
                {
                    if (index < comps.Count - 1 && comps[index + 1].Left == comp.Left && comps[index + 1].Top == comp.Top)
                        continue;
                }

                var cont = comp as StiContainer;
                if (cont != null)
                {
                    var compRect = (RectangleD)hash[comp];
                    if (!StiActionUtils.PointInRect(rect.X, rect.Y, compRect)) continue;

                    var container = cont.getContainerInRect2(rect, component, hash);
                    if (container != null)
                    {
                        obj = container;
                        break;
                    }
                }
            }

            if (obj == null && this.Parent != null && component.CanContainIn(this))
            {
                size2 = this.Parent.ContainerToPage(this.ClientRectangle);

                if (this.Page != null && this is StiBand && !this.IsCross)
                {
                    var headerSize = this.Page.Unit.ConvertFromHInches((this as StiBand).HeaderSize);
                    size2 = size2.OffsetRect(new RectangleD(0, headerSize, 0, headerSize));

                    //fix, for StiPrimitives
                    var correction = Page.Unit.ConvertFromHInches(1d);
                    if (component is StiPrimitive)
                        size2.Width += correction;

                    if (Report != null && Report.Info.ShowHeaders)
                        size2.Height += correction;
                }

                if (StiActionUtils.PointInRect(rect.X, rect.Y, size2))
                    obj = this;
            }

            if (component.IsCross) return obj;

            if (component is StiTable && obj is StiTable) return null;
            if (component is StiTable && obj is StiDataBand) return obj;
            if (!(component is IStiTableCell) && obj is StiTable) return null;
            if (component is IStiTableCell && !(obj is StiTable)) return null;
            if (component is StiBand && obj is StiBand) return null;

            return obj;
        }

        public StiContainer GetContainerInRect2(RectangleD rect, StiComponent component, Hashtable hash)
        {
            //take into account the height of the band header
            var isCross = component is StiCrossDataBand || component is StiCrossHeaderBand ||
                          component is StiCrossFooterBand || component is StiCrossGroupHeaderBand ||
                          component is StiCrossGroupFooterBand;

            if (component.Page != null && component is StiBand && !isCross)
            {
                var headerSize = component.Page.Unit.ConvertFromHInches((component as StiBand).HeaderSize);
                rect = rect.OffsetRect(new RectangleD(0, headerSize, 0, headerSize));
            }

            //get the container in which the component falls
            var container = getContainerInRect2(rect, component, hash);
            //if null - the component hits the page
            if (container == null)
                return Page;

            //if there are only nested containers in this area, then the component gets to the page
            if (container.CheckForParentComponent(component))
                return Page;

            return container;
        }

        public StiComponentsCollection GetIncorrect2(bool onlySelect)
        {
            return GetIncorrect2(onlySelect, null);
        }

        private StiComponentsCollection GetIncorrect2(bool onlySelect, Hashtable hash)
        {
            var comps = new StiComponentsCollection();

            if (hash == null)
                hash = GetSizesTable(this);

            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    var linked = comp.Linked || comp.Inherited || !StiRestrictionsHelper.IsAllowChangePosition(comp);

                    if (!linked && (!onlySelect || comp.IsSelected))
                    {
                        var rect = ContainerToPage(comp.ClientRectangle);

                        //get the container in which the component falls
                        StiComponent cont = Page.GetContainerInRect2(rect, comp, hash);

                        //if this container is not the current parent, and this container is not nested within a component,
                        //then add the component to the collection of components to be moved
                        if (cont != comp.Parent && !cont.CheckForParentComponent(comp)) //fix
                        {
                            var rectCont = ContainerToPage(cont.ClientRectangle);
                            var rectComp = ContainerToPage(comp.ClientRectangle);

                            if (!(comp.Left == 0 && rectCont.Left == rectComp.Left && comp.DockStyle != StiDockStyle.None))
                                comps.Add(comp);
                        }
                        else if (comp.IsCross && !comp.CanContainIn(comp.Parent))
                            comps.Add(comp);
                    }

                    var cont2 = comp as StiContainer;
                    if (cont2 != null)
                        comps.AddRange(cont2.GetIncorrect2(onlySelect, hash));
                }
            }

            return comps;
        }

        public void Correct2(bool onlySelect)
        {
            var hash = GetSizesTable(this);
            var objs = GetIncorrect2(onlySelect, hash);

            lock (((ICollection)objs).SyncRoot)
            {
                foreach (StiComponent comp in objs)
                {
                    if (comp.Parent == null) continue;

                    comp.ClientRectangle = comp.ComponentToPage(comp.ClientRectangle);

                    var compParent = comp.Parent;

                    comp.Parent.Components.Remove(comp);

                    var container = GetContainerInRect2(comp.ClientRectangle, comp, hash);
                    if (container == null)
                        container = Page;

                    if (comp.CanContainIn(container))
                    {
                        comp.ClientRectangle = container.PageToContainer(comp.ClientRectangle);
                        container.Components.Add(comp);
                    }
                    else
                    {
                        if (!(compParent is StiPage))
                        {
                            comp.ClientRectangle = comp.PageToComponent(comp.ClientRectangle);
                            compParent.Components.Add(comp);
                        }
                        else
                        {
                            StiLogService.Write(GetType(),
                                string.Format(StiLocalization.Get("Errors", "ContainerIsNotValidForComponent"), container.Name, comp.Name));

                            if (!StiOptions.Engine.HideMessages)
                                MessageBox.Show(string.Format(
                                    StiLocalization.Get("Errors", "ContainerIsNotValidForComponent"), container.Name,
                                    comp.Name));
                        }
                    }
                }
            }

            DockToContainer();
            SortByPriority();
        }

        /// <summary>
        /// Returns the list of all objects which require motion between containers.
        /// </summary>
        /// <returns>List of objects.</returns>
        public StiComponentsCollection GetIncorrect(bool onlySelect)
        {
            var comps = new StiComponentsCollection();

            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    var linked = comp.Linked || comp.Inherited || !StiRestrictionsHelper.IsAllowChangePosition(comp);

                    if (!linked && (!onlySelect || comp.IsSelected))
                    {
                        var rect = ContainerToPage(comp.ClientRectangle);

                        //get the container in which the component falls
                        var cont = Page.GetContainerInRect(rect, comp);

                        //if this container is not the current parent, and this container is not nested within a component,
                        //then add the component to the collection of components to be moved
                        if (cont != comp.Parent && !cont.CheckForParentComponent(comp)) //fix
                        {
                            var rectCont = ContainerToPage(cont.ClientRectangle);
                            var rectComp = ContainerToPage(comp.ClientRectangle);

                            if (!(comp.Left == 0 && rectCont.Left == rectComp.Left && comp.DockStyle != StiDockStyle.None))
                                comps.Add(comp);
                        }
                        else if (comp.IsCross && !comp.CanContainIn(comp.Parent))
                            comps.Add(comp);
                    }

                    var cont2 = comp as StiContainer;
                    if (cont2 != null)
                        comps.AddRange(cont2.GetIncorrect(onlySelect));
                }
            }

            return comps;
        }

        /// <summary>
        /// Corrects in the container all objects which require motion between containers.
        /// </summary>
        public void Correct()
        {
            Correct(false);
        }

        /// <summary>
        /// Corrects in container all objects which require moving.
        /// </summary>
        /// <param name="onlySelect">If true then correct only selected components.</param>
        public void Correct(bool onlySelect, bool isCreatingComponent = false)
        {
            if (StiOptions.Designer.UseComponentPlacementOptimization && !isCreatingComponent)
            {
                Correct2(onlySelect);
                CheckLargeHeight();
                return;
            }

            var objs = GetIncorrect(onlySelect);

            lock (((ICollection)objs).SyncRoot)
            {
                foreach (StiComponent comp in objs)
                {
                    if (comp.Parent == null) continue;

                    comp.ClientRectangle = comp.ComponentToPage(comp.ClientRectangle);

                    var compParent = comp.Parent;

                    comp.Parent.Components.Remove(comp);

                    var container = GetContainerInRect(comp.ClientRectangle, comp);
                    if (container == null)
                        container = Page;

                    if (comp.CanContainIn(container))
                    {
                        comp.ClientRectangle = container.PageToContainer(comp.ClientRectangle);
                        container.Components.Add(comp);
                    }
                    else
                    {
                        if (!(compParent is StiPage))
                        {
                            comp.ClientRectangle = comp.PageToComponent(comp.ClientRectangle);
                            compParent.Components.Add(comp);
                        }
                        else
                        {
                            StiLogService.Write(GetType(),
                                string.Format(StiLocalization.Get("Errors", "ContainerIsNotValidForComponent"), container.Name, comp.Name));

                            if (!StiOptions.Engine.HideMessages)
                                MessageBox.Show(
                                    string.Format(StiLocalization.Get("Errors", "ContainerIsNotValidForComponent"), container.Name, comp.Name));
                        }
                    }
                }
            }

            DockToContainer();
            SortByPriority();
            CheckLargeHeight();
        }

        public void CheckLargeHeight(bool needFullCalculation = false)
        {
            var lhPage = this as StiPage;
            if (lhPage == null) return;
            if (lhPage.LargeHeight || !StiOptions.Designer.AutoLargeHeight || !IsDesigning) return;

            double topY = 0;
            var bottomY = lhPage.Height;
            if (bottomY <= 0) return;

            if (needFullCalculation)
            {
                foreach (StiComponent comp in lhPage.Components)
                {
                    if (comp is StiBand && !comp.IsCross)
                    {
                        if (comp is StiPageFooterBand)
                        {
                            bottomY -= comp.DisplayRectangle.Height;
                        }
                        else
                        {
                            var compHeight = comp.DisplayRectangle.Height;

                            //check only first nested level
                            var cont = comp as StiContainer;
                            if (cont != null)
                            {
                                foreach (StiComponent comp2 in cont.Components)
                                {
                                    if (comp2.DisplayRectangle.Bottom > compHeight) compHeight = comp2.DisplayRectangle.Bottom;
                                }
                            }

                            topY += compHeight;
                        }
                    }
                }
            }
            else
            {
                foreach (StiComponent comp in lhPage.Components)
                {
                    if (comp is StiBand && !comp.IsCross)
                    {
                        if (comp is StiPageFooterBand)
                        {
                            bottomY = Math.Min(bottomY, comp.Top);
                        }
                        else
                        {
                            topY = Math.Max(topY, comp.Bottom);

                            //check only first nested level
                            var cont = comp as StiContainer;
                            if (cont != null)
                            {
                                var compTop = comp.DisplayRectangle.Top;
                                foreach (StiComponent comp2 in cont.Components)
                                {
                                    if (compTop + comp2.DisplayRectangle.Bottom > topY) topY = compTop + comp2.DisplayRectangle.Bottom;
                                }
                            }
                        }
                    }
                }
            }

            var step = 0.2;
            var stepValue = lhPage.Unit.ConvertFromHInches(30 * 3d);

            while (lhPage.PageHeight * step < stepValue)
                step += 0.2d;

            var storedFactor = lhPage.LargeHeightAutoFactor;

            while ((decimal)lhPage.LargeHeightAutoFactor < 20 && bottomY - topY < lhPage.PageHeight * step)
            {
                var tmpHeight = lhPage.Height;
                var storeLargeHeightAutoFactor = lhPage.LargeHeightAutoFactor;
                lhPage.LargeHeightAutoFactor += step;
                if (lhPage.LargeHeightAutoFactor == storeLargeHeightAutoFactor) break;
                bottomY += lhPage.Height - tmpHeight;
            }

            while (bottomY - topY > lhPage.PageHeight * (step * 2) && (decimal)lhPage.LargeHeightAutoFactor > 1)
            {
                var tmpHeight = lhPage.Height;
                var storeLargeHeightAutoFactor = lhPage.LargeHeightAutoFactor;
                lhPage.LargeHeightAutoFactor -= step;
                if (lhPage.LargeHeightAutoFactor == storeLargeHeightAutoFactor) break;
                bottomY -= tmpHeight - lhPage.Height;
            }

            if (lhPage.LargeHeightAutoFactor != storedFactor)
                StiOptions.Engine.GlobalEvents.InvokeAutoLargeHeightChanged(lhPage.Report, new StiAutoLargeHeightEventArgs(lhPage.Report, lhPage));
        }

        /// <summary>
        /// Returns the first docked and selected component.
        /// </summary>
        public StiComponent GetDockableComponent()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in this.Components)
                {
                    if (comp.DockStyle != StiDockStyle.None && comp.IsSelected)
                        return comp;
                }
            }

            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in this.Components)
                {
                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var cmp = cont.GetDockableComponent();
                        if (cmp != null)
                            return cmp;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the first selected component.
        /// </summary>
        /// <returns></returns>
        public StiComponent GetFirstSelectableCompanent()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in this.Components)
                {
                    if (comp.IsSelected)
                        return comp;
                }
            }

            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in this.Components)
                {
                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        var cmp = cont.GetFirstSelectableCompanent();
                        if (cmp != null)
                            return cmp;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Resets selection from all selected objects in the container.
        /// </summary>
        public void ResetSelection()
        {
            Reset();

            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    comp.Reset();

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.ResetSelection();
                }
            }
        }

        /// <summary>
        /// Returns the array of the selected components.
        /// </summary>
        /// <returns>The array of selected components.</returns>
        public StiComponentsCollection GetSelectedComponents()
        {
            var comps = new StiComponentsCollection();
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected)
                        comps.Add(comp);

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.GetSelectedComponents(ref comps);
                }
            }


            return comps;
        }

        /// <summary>
        /// Returns the array of the selected components.
        /// </summary>
        public void GetSelectedComponents(ref StiComponentsCollection comps)
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected)
                        comps.Add(comp);

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.GetSelectedComponents(ref comps);
                }
            }
        }

        /// <summary>
        /// Returns the array of the selected objects including child of the selected objects.
        /// </summary>
        /// <returns>The array of selected components.</returns>
        public StiComponentsCollection GetSelectedComponentsWithChilds()
        {
            var comps = new StiComponentsCollection();
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected)
                        comps.Add(comp);

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        if (comp.IsSelected)
                            comps.AddRange(cont.GetComponents());

                        else
                            cont.GetSelectedComponentsWithChilds(ref comps);
                    }
                }
            }

            return comps;
        }

        /// <summary>
        /// Returns the array of the selected objects including child of the selected objects.
        /// </summary>
        private void GetSelectedComponentsWithChilds(ref StiComponentsCollection comps)
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp.IsSelected)
                        comps.Add(comp);

                    var cont = comp as StiContainer;
                    if (cont != null)
                    {
                        if (comp.IsSelected)
                            cont.GetComponents(ref comps);

                        else
                            cont.GetSelectedComponentsWithChilds(ref comps);
                    }
                }
            }
        }

        public void SelectInRectangleCheckBands(RectangleD rect)
        {
            SelectInRectangle(rect);
            var comps = GetSelectedComponents();

            var bandsCount = 0;

            lock (((ICollection)comps).SyncRoot)
            {
                foreach (StiComponent comp in comps)
                {
                    if (comp is StiBand)
                        bandsCount++;
                }
            }

            if (bandsCount > 0 && bandsCount < comps.Count)
            {
                lock (((ICollection)comps).SyncRoot)
                {
                    foreach (StiComponent comp in comps)
                    {
                        if (comp is StiBand)
                            comp.Reset();
                    }
                }
            }
        }

        /// <summary>
        /// Selects all objects in the container which intersect with the specified rectangle.
        /// </summary>
        /// <param name="rect">Specified rectangle.</param>
        public void SelectInRectangle(RectangleD rect)
        {
            rect = rect.Normalize();

            for (var a = Components.Count - 1; a >= 0; a--)
            {
                var comp = Components[a];
                var compRect = comp.ComponentToPage(comp.SelectRectangle);

                if (rect.IntersectsWith(compRect))
                    comp.Select();

                var cont = comp as StiContainer;
                if (cont != null)
                    cont.SelectInRectangle(rect);
            }
        }

        /// <summary>
        /// Select all components in container.
        /// </summary>
        public void SelectAll()
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    comp.Select();

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.SelectAll();
                }
            }
        }

        /// <summary>
        /// Selects the specified component in the container.
        /// </summary>
        /// <param name="component">Object for selection.</param>
        public void SelectComponent(StiComponent component)
        {
            lock (((ICollection)this.Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp == component)
                    {
                        comp.Select();
                        return;
                    }

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.SelectComponent(component);
                }
            }
        }

        /// <summary>
        /// Converts a point of coordinates of the container into coodinates of a page.
        /// </summary>
        /// <param name="point">Point to be converted.</param>
        /// <returns>Converted point.</returns>
        public PointD ContainerToPage(PointD point)
        {
            var cont = this;

            while (cont != null)
            {
                var region = cont.ClientRectangle;
                if (cont.IsSelected)
                    region = DoOffsetRect(cont, region, Page.OffsetRectangle);

                point.X = point.X + region.Left;
                point.Y = point.Y + region.Top;

                cont = cont.Parent;
            }

            return point;
        }

        /// <summary>
        /// Converts a point of coordinates of a page into coodinates of a container.
        /// </summary>
        /// <param name="point">Point to be converted.</param>
        /// <returns>Converted point.</returns>
        public PointD PageToContainer(PointD point)
        {
            StiComponent cont = this;

            while (cont != null)
            {
                var region = cont.ClientRectangle;
                if (cont.IsSelected)
                    region = DoOffsetRect(cont, region, Page.OffsetRectangle);

                point.X -= region.Left;
                point.Y -= region.Top;

                cont = cont.Parent;
            }
            return point;
        }

        /// <summary>
        /// Converts a rectangle from container coordinates into coordinates of a page.
        /// </summary>
        /// <param name="rect">Rectangle to be converted.</param>
        /// <returns>Converted rectangle.</returns>
        public RectangleD ContainerToPage(RectangleD rect)
        {
            StiComponent cont = this;

            while (cont != null && !(cont is StiPage))
            {
                var region = cont.ClientRectangle;

                if (cont.IsSelected)
                    region = DoOffsetRect(cont, region, Page.OffsetRectangle);

                rect.X += region.Left;
                rect.Y += region.Top;

                cont = cont.Parent;
            }
            return rect;
        }

        /// <summary>
        /// Converts a rectangle from coordinates of a page into container coordinates.
        /// </summary>
        /// <param name="rect">Rectangle to be converted.</param>
        /// <returns>Converted rectangle.</returns>
        public RectangleD PageToContainer(RectangleD rect)
        {
            StiComponent cont = this;

            while (cont != null)
            {
                var region = cont.ClientRectangle;
                if (cont.IsSelected)
                    region = DoOffsetRect(cont, region, Page.OffsetRectangle);

                rect.X -= region.Left;
                rect.Y -= region.Top;

                cont = cont.Parent;
            }
            return rect;
        }

        /// <summary>
        /// Returns the list of all components which are located in the container.
        /// </summary>
        /// <returns>List of components.</returns>
        public StiComponentsCollection GetComponents()
        {
            var comps = new StiComponentsCollection();

            if (this is StiClone)
                return comps;

            comps.AddRange(Components);
            lock (((ICollection)Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.GetComponents(ref comps);
                }
            }

            return comps;
        }

        /// <summary>
        /// Returns the list of all components which are located in the container.
        /// </summary>
        /// <returns>List of components.</returns>
        public void GetComponents(ref StiComponentsCollection comps)
        {
            if (this is StiClone) return;

            comps.AddRange(Components);
            lock (((ICollection)Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.GetComponents(ref comps);
                }
            }
        }

        /// <summary>
        /// Returns the list of all components which are located in the container.
        /// </summary>
        /// <returns>List of components.</returns>
        public void GetComponents<T>(ref StiComponentsCollection comps) where T : StiComponent
        {
            if (this is StiClone) return;

            lock (((ICollection)Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    if (comp is T)
                        comps.Add(comp);

                    var cont = comp as StiContainer;
                    if (cont != null)
                        cont.GetComponents(ref comps);
                }
            }
        }

        /// <summary>
        /// Returns the list of all components which are located in the container.
        /// </summary>
        /// <returns>List of components.</returns>
        public List<StiComponent> GetComponentsList()
        {
            var comps = new List<StiComponent>();

            if (this is StiClone)
                return comps;

            foreach (StiComponent comp in Components)
            {
                comps.Add(comp);
                var cont = comp as StiContainer;
                if (cont != null)
                    comps.AddRange(cont.GetComponentsList());
            }
            return comps;
        }

        public virtual int GetComponentsCount()
        {
            var count = 0;

            if (this is StiClone)
                return 1;

            count += Components.Count;
            lock (((ICollection)Components).SyncRoot)
            {
                foreach (StiComponent comp in Components)
                {
                    var cont = comp as StiContainer;
                    if (cont != null)
                        count += cont.GetComponentsCount();
                }
            }

            return count;
        }

        /// <summary>
        /// Removes all selected object from the container.
        /// </summary>
        public virtual void RemoveAllSelected()
        {
            var index = 0;
            while (index < components.Count)
            {
                var comp = components[index];

                var cont = comp as StiContainer;
                if (cont != null)
                    cont.RemoveAllSelected();

                if (comp.IsSelected && comp.AllowDelete)
                {
                    var obj = components[index];
                    obj.OnRemoveComponent();

                    if (obj.Report != null && obj.Report.Designer != null)
                        obj.Report.GlobalizationStrings.RemoveComponent(obj);

                    components.Remove(obj);
                    StiOptions.Engine.GlobalEvents.InvokeComponentRemoved(obj.Report.Designer, new StiComponentRemovingEventArgs(obj.Report.Designer, obj));
                }
                else
                    index++;
            }
        }

        /// <summary>
        /// Removes all selected object from the container.
        /// </summary>
        public void RemoveAllSelected(ref List<StiComponent> removeComps)
        {
            var index = 0;
            while (index < components.Count)
            {
                var comp = components[index];

                var cont = comp as StiContainer;
                if (cont != null)
                    cont.RemoveAllSelected(ref removeComps);

                if (comp.IsSelected && comp.AllowDelete)
                {
                    var obj = components[index];
                    obj.OnRemoveComponent();

                    if (obj.Report != null && obj.Report.Designer != null)
                        obj.Report.GlobalizationStrings.RemoveComponent(obj);

                    components.Remove(obj);
                    removeComps.Add(obj);

                    StiOptions.Engine.GlobalEvents.InvokeComponentRemoved(obj.Report.Designer, new StiComponentRemovingEventArgs(obj.Report.Designer, obj));
                }
                else
                    index++;
            }
        }

        /// <summary>
        /// Converts coordinates of all objects into coordinates of a page
        /// and moves objects on the page.
        /// </summary>
        public void MoveComponentsToPage()
        {
            var comps = this.GetComponentsList();
            foreach (var comp in comps)
            {
                if (comp.Parent != comp.Page)
                {
                    comp.SetDirectDisplayRectangle(comp.ComponentToPage(comp.DisplayRectangle));
                    comp.Page.Components.Add(comp);
                }
            }

            //speed optimization
            foreach (var comp in comps)
            {
                var cont = comp as StiContainer;
                if (cont != null && cont.Components.Count > 0)
                    cont.Components.Clear();
            }
        }

        /// <summary>
        /// Converts coordinates of all objects into coordinates of a page.
        /// </summary>
        public void ConvertSelectedToPage()
        {
            var index = 0;
            while (index < components.Count)
            {
                var comp = components[index];
                var cont = comp as StiContainer;
                if (cont != null)
                    cont.MoveComponentsToPage();

                if (!(comp.Parent is StiPage) && comp.IsSelected)
                {
                    comp.DisplayRectangle = this.ContainerToPage(comp.DisplayRectangle);
                    comp.Parent.components.Remove(comp);
                    comp.Page.Components.Add(comp);
                }
                else
                    index++;
            }
        }

        /// <summary>
        /// Converts objects from all coordinates into coordinates of a page without move them on a page.
        /// </summary>
        public void ConvertToPage()
        {
            foreach (StiComponent comp in Components)
            {
                comp.ClientRectangle = comp.ComponentToPage(comp.ClientRectangle);

                var cont = comp as StiContainer;
                if (cont != null)
                    cont.ConvertToPage();
            }
        }

        public void InvertComponentsPosition()
        {
            foreach (StiComponent comp in Components)
            {
                comp.Left = this.Width - comp.Left - comp.Width;

                var cont = comp as StiContainer;
                if (cont != null)
                    cont.InvertComponentsPosition();
            }
        }
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiContainer();
        }
        #endregion

        /// <summary>
        /// Creates a new container.
        /// </summary>
        public StiContainer() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new container.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the container.</param>
        public StiContainer(RectangleD rect) : base(rect)
        {
            components = new StiComponentsCollection(this);
            components.ComponentAdded += OnComponentAdded;
            components.ComponentRemoved += OnComponentRemoved;

            PlaceOnToolbox = true;
        }
    }
}
