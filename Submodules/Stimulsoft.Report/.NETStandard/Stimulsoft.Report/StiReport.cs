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
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Globalization;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Design;
using Stimulsoft.Report.CodeDom;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Print;
using Stimulsoft.Report.Units;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Viewer;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.Print.Design;
using System.Text.RegularExpressions;
using System.Threading;
using Stimulsoft.Base.Blocks;
using Stimulsoft.Base.Blockly;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.Design.Forms;
using System.Drawing;
using System.Drawing.Printing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Drawing;
using ToolboxBitmapAttribute = Stimulsoft.System.Drawing.ToolboxBitmapAttribute;
#else
using System.Windows.Forms;
using System.Drawing.Design;
#endif

#if STIDRAWING
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report
{
    /// <summary>
    /// Report class.
    /// </summary>
    [Designer("Stimulsoft.Report.Design.StiReportDesigner, Stimulsoft.Design, " + StiVersion.VersionInfo)]
    [ToolboxBitmap(typeof(StiReport), "Toolbox.StiReport.bmp")]
    [Guid("5A067BAF-802D-4cc8-AAD3-5615641F7050")]
    [StiServiceBitmap(typeof(StiReport), "Stimulsoft.Report.Images.Components.StiReport.png")]
    public partial class StiReport :
        Component,
        IStiReport,
        IStiSelect,
        ICloneable,
        IStiStateSaveRestore,
        IStiUnitConvert,
        IStiEngineVersionProperty,
        IStiPropertyGridObject,
        IStiGetFonts
    {
        #region IStiPropertyGridObject
        [Browsable(false)]
        [StiBrowsable(false)]
        public StiComponentId ComponentId => StiComponentId.StiReport;

        [Browsable(false)]
        [StiBrowsable(false)]
        public string PropName { get; private set; } = string.Empty;

        public StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var helper = new StiPropertyCollection();

            var propHelper = propertyGrid.PropertiesHelper;
            var list = new[]
            {
                propHelper.ReportName(),
                propHelper.ReportAlias(),
                propHelper.ReportAuthor(),
                propHelper.ReportDescription(),
                propHelper.ReportIcon(),
                propHelper.ReportImage(),
            };
            helper.Add(StiPropertyCategories.Design, list);

            if (level == StiLevel.Standard)
            {
                list = new[]
                {
                    propHelper.ConvertNulls(),
                };

                helper.Add(StiPropertyCategories.Data, list);
            }
            else if (level == StiLevel.Professional)
            {
                list = new[]
                {
                    propHelper.CacheAllData(),
                    propHelper.ConvertNulls(),
                    propHelper.RetrieveOnlyUsedData()
                };

                helper.Add(StiPropertyCategories.Data, list);
            }

            if (level == StiLevel.Standard)
            {
                list = new[]
                {
                    propHelper.ReportCulture(),
                };

                helper.Add(StiPropertyCategories.Globalization, list);
            }
            else if (level == StiLevel.Professional)
            {
                list = new[]
                {
                    propHelper.AutoLocalizeReportOnRun(),
                    propHelper.ReportCulture(),
                    propHelper.GlobalizationStrings()
                };

                helper.Add(StiPropertyCategories.Globalization, list);
            }

            if (level == StiLevel.Standard)
            {
                list = new[]
                {
                    propHelper.NumberOfPass(),
                    propHelper.ReportUnit(),
                    propHelper.ScriptLanguage(),
                    propHelper.ReportStyles()
                };

                helper.Add(StiPropertyCategories.Engine, list);
            }
            else if (level == StiLevel.Professional)
            {
                list = new[]
                {
                    propHelper.CacheTotals(),
                    propHelper.CalculationMode(),
                    propHelper.Collate(),
                    propHelper.ReportEngineVersion(),
                    propHelper.NumberOfPass(),
                    propHelper.ReferencedAssemblies(),
                    propHelper.ReportCacheMode(),
                    propHelper.ReportUnit(),
                    propHelper.ScriptLanguage(),
                    propHelper.StopBeforePage(),
                    propHelper.ReportStyles()
                };

                helper.Add(StiPropertyCategories.Engine, list);
            }

            if (level == StiLevel.Standard)
            {
                list = new[]
                {
                    propHelper.PreviewSettings(),
                    propHelper.ReportPrinterSettings(),
                    propHelper.ParametersOrientation(),
                    propHelper.RequestParameters(),
                };

                helper.Add(StiPropertyCategories.View, list);
            }
            else if (level == StiLevel.Professional)
            {
                list = new[]
                {
                    propHelper.PreviewSettings(),
                    propHelper.ReportPrinterSettings(),
                    propHelper.RefreshTime(),
                    propHelper.ParametersOrientation(),
                    propHelper.RequestParameters(),
                    propHelper.ParameterWidth()
                };
                helper.Add(StiPropertyCategories.View, list);
            }

            return helper;
        }

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            var objectHelper = new StiEventCollection();

            // RenderEventsCategory
            var list = new[] { StiPropertyEventId.BeginRenderEvent, StiPropertyEventId.RenderingEvent, StiPropertyEventId.EndRenderEvent };
            objectHelper.Add(StiPropertyCategories.RenderEvents, list);

            // ExportEventsCategory
            list = new[] { StiPropertyEventId.ExportingEvent, StiPropertyEventId.ExportedEvent };
            objectHelper.Add(StiPropertyCategories.ExportEvents, list);

            // ExportEventsCategory
            list = new[] { StiPropertyEventId.PrintingEvent, StiPropertyEventId.PrintedEvent };
            objectHelper.Add(StiPropertyCategories.PrintEvents, list);

            return objectHelper;
        }
        #endregion

        #region IStiStateSaveRestore
        /// <summary>
        /// Gets the component states manager.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiStatesManager States { get; } = new StiStatesManager();

        /// <summary>
        /// Saves the current state of an object.
        /// </summary>
        /// <param name="stateName">A name of the state being saved.</param>
        public virtual void SaveState(string stateName)
        {
            Pages.SaveState(stateName);
            Dictionary.SaveState(stateName);

            if (AggregateFunctions == null || AggregateFunctions.Length <= 0) return;
            foreach (StiAggregateFunctionService func in AggregateFunctions)
            {
                func.SaveState(stateName);
            }
        }

        /// <summary>
        /// Restores the earlier saved object state.
        /// </summary>
        /// <param name="stateName">A name of the state being restored.</param>
        public virtual void RestoreState(string stateName)
        {
            Pages.RestoreState(stateName);
            Dictionary.RestoreState(stateName);

            if (AggregateFunctions == null || AggregateFunctions.Length <= 0) return;
            foreach (StiAggregateFunctionService func in AggregateFunctions)
            {
                func.RestoreState(stateName);
            }
        }

        /// <summary>
        /// Clears all earlier saved object states. Internal use only.
        /// </summary>
        public void ClearAllStates()
        {
            States.Clear();

            Pages.ClearAllStates();

            if (AggregateFunctions == null || AggregateFunctions.Length <= 0) return;
            foreach (StiAggregateFunctionService func in AggregateFunctions)
            {
                func.ClearAllStates();
            }
        }
        #endregion

        #region IStiSelect
        /// <summary>
        /// Internal use only.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int SelectionTick { get; private set; }

        private bool isSelected;
        /// <summary>
        /// Gets or sets value, which indicates whether the report in the designer is selected or not.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (!isSelected)
                    SelectionTick = Environment.TickCount;

                isSelected = value;
            }
        }

        /// <summary>
        /// Selects a report.
        /// </summary>
        public void Select()
        {
            if (!isSelected)
                SelectionTick = Environment.TickCount;

            isSelected = true;
        }

        /// <summary>
        /// Inverts selection of a report.
        /// </summary>
        public void Invert()
        {
            if (!isSelected)
                SelectionTick = Environment.TickCount;

            isSelected = !isSelected;
        }

        /// <summary>
        /// Resets selection of a report.
        /// </summary>
        public void Reset()
        {
            isSelected = false;
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public virtual object Clone()
        {
            var report = StiActivator.CreateObject(StiOptions.Engine.BaseReportType) as StiReport;
            var reportFileStored = this.ReportFile;
            bool isModifiedState = this.IsModified;
            var str = this.SaveToString();
            report.LoadFromString(str);
            this.IsModified = isModifiedState;

            if (this.GotoComp != null) report.GotoComp = (StiGotoCompEventHandler)this.GotoComp.Clone();
            if (this.Paint != null) report.Paint = (EventHandler)this.Paint.Clone();
            if (this.BeginRender != null) report.BeginRender = (EventHandler)this.BeginRender.Clone();
            if (this.Rendering != null) report.Rendering = (EventHandler)this.Rendering.Clone();
            if (this.EndRender != null) report.EndRender = (EventHandler)this.EndRender.Clone();

            if (this.Refreshing != null) report.Refreshing = (EventHandler)this.Refreshing.Clone();
            if (this.Printing != null) report.Printing = (EventHandler)this.Printing.Clone();
            if (this.Printed != null) report.Printed = (EventHandler)this.Printed.Clone();
            if (this.Exporting != null) report.Exporting = (StiExportEventHandler)this.Exporting.Clone();
            if (this.Exported != null) report.Exported = (StiExportEventHandler)this.Exported.Clone();

            if (this.BeginRenderEvent != null) report.BeginRenderEvent = (StiBeginRenderEvent)this.BeginRenderEvent.Clone();
            if (this.RenderingEvent != null) report.RenderingEvent = (StiRenderingEvent)this.RenderingEvent.Clone();
            if (this.EndRenderEvent != null) report.EndRenderEvent = (StiEndRenderEvent)this.EndRenderEvent.Clone();

            if (this.PrintingEvent != null) report.PrintingEvent = (StiPrintingEvent)this.PrintingEvent.Clone();
            if (this.PrintedEvent != null) report.PrintedEvent = (StiPrintedEvent)this.PrintedEvent.Clone();
            if (this.ExportingEvent != null) report.ExportingEvent = (StiExportingEvent)this.ExportingEvent.Clone();
            if (this.ExportedEvent != null) report.ExportedEvent = (StiExportedEvent)this.ExportedEvent.Clone();

            report.RegData(this.DataStore);
            report.RegBusinessObject(this.BusinessObjectsStore);
            report.CookieContainer = this.CookieContainer;
            report.HttpHeadersContainer = this.HttpHeadersContainer;
            report.ReportFile = reportFileStored;
            this.ReportFile = reportFileStored;

            return report;
        }
        #endregion

        #region IStiApp
        /// <summary>
        /// Returns reference to the data dictionary of the report.
        /// </summary>
        /// <returns>Data dictioanary from the report.</returns>
        IStiAppDictionary IStiApp.GetDictionary()
        {
            return Dictionary;
        }
        #endregion

        #region IStiAppCell
        /// <summary>
        /// Returns unique key to this application.
        /// </summary>
        /// <returns></returns>
        string IStiAppCell.GetKey()
        {
            return Key;
        }

        void IStiAppCell.SetKey(string key)
        {
            Key = key;
        }
        #endregion

        #region IStiReport
        /// <summary>
        /// Returns an enumeration of the pages from this report.
        /// </summary>
        /// <returns>The enumeration of the pages.</returns>
        IEnumerable<IStiReportPage> IStiReport.FetchPages()
        {
            return Pages?.ToList();
        }
        #endregion

        #region IStiGetFonts
        public List<StiFont> GetFonts()
        {
            var result = new List<StiFont>();
            foreach (var page in Pages)
            {
                if (page is IStiGetFonts)
                {
                    result.AddRange((page as IStiGetFonts).GetFonts());
                }
            }
            return result.Distinct().ToList();
        }
        #endregion

        #region Dictionary
        private List<StiBusinessObjectData> businessObjectsStore;
        /// <summary>
        /// Gets collection of the registered business objects.
        /// </summary>
        [StiBrowsable(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<StiBusinessObjectData> BusinessObjectsStore
        {
            get
            {
                return businessObjectsStore ?? (businessObjectsStore = new List<StiBusinessObjectData>());
            }
        }

        /// <summary>
        /// Returns the collection of DataSources of the report.
        /// </summary>
		[Editor("Stimulsoft.Report.Design.StiReportDataSourcesEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [StiBrowsable(false)]
        [StiCategory("Data")]
        [Category("Data")]
        public StiReportDataSourceCollection ReportDataSources { get; }

        /// <summary>
        /// Returns the collection of code defined report variables.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [StiBrowsable(false)]
        [Browsable(false)]
        public Hashtable Variables { get; internal set; }

#if SERVER
        /// <summary>
        /// This is special hashtable which contains names of the report parameters which assigned from the server side.
        /// The report engine should skip this parameters when it inits content of the parameters from database.
        /// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [StiBrowsable(false)]
        [Browsable(false)]
        public Hashtable ServerParameters { get; set; }
#endif

        /// <summary>
        /// Indicates the collection of user-defined variables declared in a report from code. 
        /// </summary>
        public object this[string name]
        {
            get
            {
                var report = this.CompiledReport ?? this;

                var field = report.GetType().GetField(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    try//We need to use try-catch for issues for predefined properties of a report like Language
                    {
                        return field.GetValue(report);
                    }
                    catch
                    {
                    }
                }

                var property = report.GetType().GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    try//We need to use try-catch for issues for predefined properties of a report like Language
                    {
                        return property.GetValue(report, new object[0]);
                    }
                    catch
                    {
                    }
                }

                if (report.Variables == null)
                    return null;

                return report.Variables[name];
            }
            set
            {
                var report = this.CompiledReport ?? this;

                if (report.Dictionary != null && report.Dictionary.Variables.Contains(name))
                    report.ModifiedVariables[name] = null;

                var field = report.GetType().GetField(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    try//We need to use try-catch for issues for predefined properties of a report like Language
                    {
                        field.SetValue(report, value);
                        return;
                    }
                    catch
                    {
                    }
                }

                var property = report.GetType().GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property != null && property.CanWrite)
                {
                    try//We need to use try-catch for issues for predefined properties of a report like Language
                    {
                        property.SetValue(report, value, new object[0]);
                        return;
                    }
                    catch
                    {
                    }
                }

                if (report.Variables == null)
                    StiParser.PrepareReportVariables(report);

                report.Variables[name] = value;
            }
        }

        /// <summary>
        /// Gets or sets the array for registration of aggregate function objects in the compiled report.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object[] AggregateFunctions { get; set; }

        /// <summary>
        /// Gets or sets a dictionary of data.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [StiSerializable(StiSerializationVisibility.Class, StiSerializeTypes.SerializeToAll)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public StiDictionary Dictionary { get; set; }

        /// <summary>
        /// Gets data sources of the report.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiDataSourcesCollection DataSources => Dictionary.DataSources;

        /// <summary>
        /// Gets data available for the current report.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiDataCollection DataStore => Dictionary.DataStore;

        /// <summary>
        /// Checks if specified variable exists in the dictionary.
        /// </summary>
        /// <param name="variableName">Name of the variable to check.</param>
        public bool IsVariableExist(string variableName)
        {
            if (CompiledReport != null || !CheckNeedsCompiling())
            {
                var report = CompiledReport ?? this;

                var field = report.GetType().GetField(variableName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.IgnoreCase);

                if (field != null) return true;

                var prop = report.GetType().GetProperty(variableName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.IgnoreCase);

                return prop != null;
            }

            return Dictionary.Variables.IndexOf(variableName) != -1;
        }
        #endregion		

        #region Language
        private string script;
        /// <summary>
        /// Gets or sets a script of the report.
        /// </summary>
        [StiSerializable(
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad |
             StiSerializeTypes.SerializeToDocument)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public string Script
        {
            get
            {
                if (script == null)
                    ScriptNew();
                return script;
            }
            set
            {
                script = value;
            }
        }

        private static StiCSharpLanguage CSharpLanguage;
        private static StiVBLanguage VBLanguage;

        /// <summary>
        /// Instead of this property, please, use script language property.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiLanguage Language
        {
            get
            {
                if (ScriptLanguage == StiReportLanguageType.VB)
                    return VBLanguage ?? (VBLanguage = new StiVBLanguage());

                else
                    return CSharpLanguage ?? (CSharpLanguage = new StiCSharpLanguage());
            }
            set
            {
                ScriptLanguage = value is StiVBLanguage
                    ? StiReportLanguageType.VB
                    : StiReportLanguageType.CSharp;
            }
        }

        /// <summary>
        /// Packs a script.
        /// </summary>
        public StiReport ScriptPack()
        {
            script = StiLanguage.RemoveGeneratedCode(Script, Language);

            int index;
            int length;
            StiLanguage.GetGeneratedCodePos(script, out index, out length, Language);

            if (StiOptions.Engine.PackScriptAfterReportLoaded && index > 0)
                script = StiLanguage.InsertGeneratedCode(script, "\r\n\t\t", Language);

            return this;
        }

        /// <summary>
        /// Unpacks a script.
        /// </summary>
        public StiReport ScriptUnpack()
        {
            ScriptUnpack(false);

            return this;
        }

        /// <summary>
        /// Unpacks a script.
        /// </summary>
        public StiReport ScriptUnpack(bool saveForInheritedReports)
        {
            var serializator = new StiCodeDomSerializator();
            var text = serializator.Serialize(this, this.GetReportName(), Language, saveForInheritedReports);
            script = StiLanguage.ReplaceGeneratedCode(Script, text, Language);

            return this;
        }

        /// <summary>
        /// Updates a script to the current state of an object.
        /// </summary>
        public StiReport ScriptUpdate()
        {
            ScriptUpdate(null, true);

            return this;
        }

        /// <summary>
        /// Updates a script to the current state of an object.
        /// </summary>
        public StiReport ScriptUpdate(bool allowUseResources)
        {
            ScriptUpdate(null, allowUseResources);

            return this;
        }

        /// <summary>
        /// Updates a script to the current state of an object.
        /// </summary>
        private void ScriptUpdate(object standaloneReportType, bool allowUseResources)
        {
            UpdateReportVersion();

            var serializator = new StiCodeDomSerializator();
            var text = serializator.Serialize(this, this.GetReportName(), Language, standaloneReportType);

            script = StiLanguage.ReplaceGeneratedCode(Script, text, Language);
        }

        /// <summary>
        /// Forms a new script.
        /// </summary>
        public StiReport ScriptNew()
        {
            var sb = new StringBuilder();

            #region C Sharp
            if (this.ScriptLanguage == StiReportLanguageType.CSharp || this.ScriptLanguage == StiReportLanguageType.JS)
            {
                #region Process Namespaces
                if (StiOptions.Engine.Namespaces != null)
                {
                    foreach (string name in StiOptions.Engine.Namespaces)
                    {
                        sb = sb.AppendFormat("using {0};", name);
                        sb = sb.AppendLine();
                    }
                }
                #endregion

                sb = sb.AppendLine();

                sb = sb.AppendFormat("namespace {0}", StiOptions.Engine.DefaultNamespace);
                sb = sb.AppendLine();
                sb = sb.AppendLine("{");
                sb = sb.AppendFormat("    public class {0} : {1}", this.GetReportName(), StiOptions.Engine.BaseReportType);
                sb = sb.AppendLine();
                sb = sb.AppendLine("    {");
                sb = sb.AppendFormat("        public {0}()", this.GetReportName());
                sb = sb.AppendLine("        {");
                sb = sb.AppendLine("            this.InitializeComponent();");
                sb = sb.AppendLine("        }");
                sb = sb.AppendLine();
                sb = sb.AppendLine("        #region StiReport Designer generated code - do not modify");
                sb = sb.AppendLine("        #endregion StiReport Designer generated code - do not modify");
                sb = sb.AppendLine("    }");
                sb = sb.AppendLine("}");

            }
            #endregion

            #region VB.Net
            else
            {
                #region Process Namespaces
                if (StiOptions.Engine.Namespaces != null)
                {
                    foreach (string name in StiOptions.Engine.Namespaces)
                    {
                        sb = sb.AppendFormat("Imports {0}", name);
                        sb = sb.AppendLine();
                    }
                }
                #endregion

                sb = sb.AppendLine();

                sb = sb.AppendFormat("Namespace {0}", StiOptions.Engine.DefaultNamespace);
                sb = sb.AppendLine();
                sb = sb.AppendFormat("    Public Class {0}", this.GetReportName());
                sb = sb.AppendLine();
                sb = sb.AppendFormat("        Inherits {0}", StiOptions.Engine.BaseReportType);
                sb = sb.AppendLine();
                sb = sb.AppendLine();
                sb = sb.AppendLine("        Public Sub New()");
                sb = sb.AppendLine("            MyBase.New");
                sb = sb.AppendLine("            Me.InitializeComponent");
                sb = sb.AppendLine("        End Sub");
                sb = sb.AppendLine();
                sb = sb.AppendLine("        #Region \"StiReport Designer generated code - do not modify\"");
                sb = sb.AppendLine("        #End Region 'StiReport Designer generated code - do not modify");
                sb = sb.AppendLine("    End Class");
                sb = sb.AppendLine("End Namespace");

            }
            #endregion

            this.script = sb.ToString();

            return this;
        }
        #endregion

        #region Events
        #region RefreshControl
        public event StiReportControlUpdateEventHandler RefreshControl;

        public void InvokeRefreshControl(object sender, string propertyName)
        {
            InvokeRefreshControl(sender, new StiReportControlUpdateEventArgs(propertyName));
        }

        public void InvokeRefreshControl(object sender, StiReportControlUpdateEventArgs e)
        {
            this.RefreshControl?.Invoke(sender, e);
            this.CompiledReport?.InvokeRefreshControl(sender, e);
        }
        #endregion

        #region RefreshViewer
        /// <summary>
        /// Occurs when report rendering is finished and viewer control needs to be updated.
        /// </summary>
        [Obsolete("Please use event 'RefreshViewer' instead event 'RefreshPreview'.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler RefreshPreview;

        /// <summary>
        /// Occurs when report rendering is finished and viewer control needs to be updated.
        /// </summary>
        public event EventHandler RefreshViewer;

        /// <summary>
        /// Raises the RefreshPreview event for this report.
        /// </summary>
        [Obsolete("Please use method 'InvokeRefreshViewer' instead method 'InvokeRefreshPreview'.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void InvokeRefreshPreview()
        {
            InvokeRefreshViewer();
        }

        /// <summary>
        /// Raises the RefreshViewer event for this report.
        /// </summary>
        public void InvokeRefreshViewer()
        {
            this.RefreshPreview?.Invoke(this, EventArgs.Empty);
            this.RefreshViewer?.Invoke(this, EventArgs.Empty);

            if (CompiledReport != null)
                CompiledReport.InvokeRefreshViewer();
        }
        #endregion

        #region Click
        /// <summary>
        /// Occurs when user clicks in the viewer window.
        /// </summary>
        public event EventHandler Click;

        /// <summary>
        /// Raises the Click event for this report.
        /// </summary>
        /// <param name="sender">A sender which raises the event.</param>
        /// <param name="e">An EventArgs which contains data of the event.</param>
        public void InvokeClick(object sender, EventArgs e)
        {
            this.Click?.Invoke(sender, e);
        }
        #endregion

        #region DoubleClick
        /// <summary>
        /// Occurs when user double clicks in the viewer window.
        /// </summary>
        public event EventHandler DoubleClick;

        /// <summary>
        /// Raises the DoubleClick event for this report.
        /// </summary>
        /// <param name="sender">A sender which raises the event.</param>
        /// <param name="e">An EventArgs which contains data of the event.</param>
        public void InvokeDoubleClick(object sender, EventArgs e)
        {
            this.DoubleClick?.Invoke(sender, e);
        }
        #endregion

        #region GotoComp
        /// <summary>
        /// Occurs when the Engine.GotoComponent method is called.
        /// </summary>
        public event StiGotoCompEventHandler GotoComp;

        /// <summary>
        /// Raises the GotoComp event for this report.
        /// </summary>
        /// <param name="e">A StiGotoCompEventArgs parameter which contains the event data.</param>
        public void InvokeGotoComp(StiGotoCompEventArgs e)
        {
            this.GotoComp?.Invoke(this, e);
        }
        #endregion

        #region Paint
        /// <summary>
        /// Occurs when it is necessary to repaint in the window of viewer.
        /// </summary>
        public event EventHandler Paint;

        /// <summary>
        /// Raises the Paint event for this report.
        /// </summary>
        /// <param name="e">An EventArgs which contains the event data.</param>
        public void InvokePaint(object sender, EventArgs e)
        {
            this.Paint?.Invoke(sender ?? this, e);
        }
        #endregion

        #region OnBeginRender
        /// <summary>
        /// Occurs when the report rendering starts.
        /// </summary>
        public event EventHandler BeginRender;

        /// <summary>
        /// Raises the BeginRender event for this report.
        /// </summary>
        public void InvokeBeginRender()
        {
            try
            {
                this.BeginRender?.Invoke(this, EventArgs.Empty);

                StiBlocklyHelper.InvokeBlockly(this, this, BeginRenderEvent);

                StiOptions.Engine.GlobalEvents.InvokeReportBeginRender(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                StiLogService.Write(typeof(StiReport), "InvokeBeginRender...ERROR");
                StiLogService.Write(this.GetType(), ex.Message);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
        }

        /// <summary>
        /// Occurs when the report rendering starts.
        /// </summary>
        [StiSerializable]
        [StiCategory("RenderEvents")]
        [Description("Occurs when the report rendering starts.")]
        [Category("RenderEvents")]
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiBeginRenderEvent BeginRenderEvent { get; set; } = new StiBeginRenderEvent();
        #endregion

        #region OnRendering
        /// <summary>
        /// Occurs when a page is rendered.
        /// </summary>
        public event EventHandler Rendering;

        /// <summary>
        /// Raises the Rendering event for the report.
        /// </summary>
        public void InvokeRendering()
        {
            try
            {
                this.Rendering?.Invoke(this, EventArgs.Empty);

                StiBlocklyHelper.InvokeBlockly(this, this, RenderingEvent);

                StiOptions.Engine.GlobalEvents.InvokeReportRendering(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                StiLogService.Write(typeof(StiReport), "InvokeRendering...ERROR");
                StiLogService.Write(this.GetType(), ex.Message);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
        }

        /// <summary>
        /// Occurs when a page is rendered.
        /// </summary>
        [Description("Occurs when a page is rendered.")]
        [StiSerializable]
        [StiCategory("RenderEvents")]
        [Category("RenderEvents")]
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiRenderingEvent RenderingEvent { get; set; } = new StiRenderingEvent();
        #endregion

        #region OnEndRender
        /// <summary>
        /// Occurs when the report rendering is finished.
        /// </summary>
        public event EventHandler EndRender;

        /// <summary>
        /// Raises the EndRender event for the report.
        /// </summary>
        public void InvokeEndRender()
        {
            try
            {
                #region CalculationMode == StiCalculationMode.Interpretation
                if (CalculationMode == StiCalculationMode.Interpretation)
                {
                    var totalsCopy = new Hashtable(Totals);
                    foreach (DictionaryEntry de in totalsCopy)
                    {
                        string compName = de.Key as string;
                        if (compName != null && compName.StartsWith("#%#"))
                        {
                            (GetComponentByName(compName.Substring(3)).Clone() as StiText).SetText(null, null);
                        }
                    }
                    StiSimpleText.ProcessEndRenderSetText(this);
                }
                #endregion

                this.EndRender?.Invoke(this, EventArgs.Empty);

                StiBlocklyHelper.InvokeBlockly(this, this, EndRenderEvent);

                StiOptions.Engine.GlobalEvents.InvokeReportEndRender(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                StiLogService.Write(typeof(StiReport), "InvokeEndRender...ERROR");
                StiLogService.Write(this.GetType(), ex.Message);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
        }

        /// <summary>
        /// Occurs when the report rendering is finished.
        /// </summary>
        [StiSerializable]
        [Description("Occurs when the report rendering is finished.")]
        [StiCategory("RenderEvents")]
        [Category("RenderEvents")]
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiEndRenderEvent EndRenderEvent { get; set; } = new StiEndRenderEvent();
        #endregion

        #region OnStatusChanged
        /// <summary>
        /// Occurs when status of the report is changed when report rendering.
        /// </summary>
        public event EventHandler StatusChanged;

        /// <summary>
        /// Raises the StatusChanged event for the report.
        /// </summary>
        public void InvokeStatusChanged()
        {
            try
            {
                this.StatusChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                StiLogService.Write(typeof(StiReport), "InvokeStatusChanged...ERROR");
                StiLogService.Write(this.GetType(), ex.Message);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
        }
        #endregion

        #region OnExporting
        /// <summary>
        /// Occurs when report starts exporting.
        /// </summary>
        public event StiExportEventHandler Exporting;

        protected virtual void OnExporting(StiExportEventArgs e)
        {
            this.Exporting?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when report starts exporting.
        /// </summary>
        [StiSerializable]
        [Description("Occurs when report starts exporting.")]
        [StiCategory("ExportEvents")]
        [Category("ExportEvents")]
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiExportingEvent ExportingEvent { get; set; } = new StiExportingEvent();

        /// <summary>
        /// Raises the Exporting event for this report.
        /// </summary>
        public void InvokeExporting(StiExportFormat exportFormat)
        {
            this.IsExporting = true;
            if (this.CompiledReport != null) CompiledReport.IsExporting = true;

            var args = new StiExportEventArgs(exportFormat);
            OnExporting(args);

            StiBlocklyHelper.InvokeBlockly(this, this, ExportingEvent);

            StiOptions.Engine.GlobalEvents.InvokeReportExporting(this, args);
        }
        #endregion

        #region OnExported
        /// <summary>
        /// Occurs when report ends exporting.
        /// </summary>
        public event StiExportEventHandler Exported;

        protected virtual void OnExported(StiExportEventArgs e)
        {
            this.Exported?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when report ends exporting.
        /// </summary>
        [StiSerializable]
        [Description("Occurs when report ends exporting.")]
        [StiCategory("ExportEvents")]
        [Category("ExportEvents")]
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiExportedEvent ExportedEvent { get; set; } = new StiExportedEvent();

        /// <summary>
        /// Raises the Exported event for this report.
        /// </summary>
        public void InvokeExported(StiExportFormat exportFormat)
        {
            this.IsExporting = false;
            if (this.CompiledReport != null) CompiledReport.IsExporting = false;

            var args = new StiExportEventArgs(exportFormat);
            OnExported(args);
            
            StiBlocklyHelper.InvokeBlockly(this, this, ExportedEvent);

            StiOptions.Engine.GlobalEvents.InvokeReportExported(this, args);
        }
        #endregion

        #region OnPrinting
        /// <summary>
        /// Occurs when report starts printing.
        /// </summary>
        public event EventHandler Printing;

        protected virtual void OnPrinting(EventArgs e)
        {
            this.Printing?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when report starts printing.
        /// </summary>
        [StiSerializable]
        [Description("Occurs when report starts printing.")]
        [StiCategory("PrintEvents")]
        [Category("PrintEvents")]
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiPrintingEvent PrintingEvent { get; set; } = new StiPrintingEvent();

        /// <summary>
        /// Raises the Printing event for this report.
        /// </summary>
        public void InvokePrinting()
        {
            OnPrinting(EventArgs.Empty);

            StiBlocklyHelper.InvokeBlockly(this, this, PrintingEvent);

            StiOptions.Engine.GlobalEvents.InvokeReportPrinting(this, EventArgs.Empty);
        }
        #endregion

        #region OnPrinted
        /// <summary>
        /// Occurs when report ends printing.
        /// </summary>
        public event EventHandler Printed;

        protected virtual void OnPrinted(EventArgs e)
        {
            this.Printed?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when report ends printing.
        /// </summary>
        [StiSerializable]
        [Description("Occurs when report ends printing.")]
        [StiCategory("PrintEvents")]
        [Category("PrintEvents")]
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiPrintedEvent PrintedEvent { get; set; } = new StiPrintedEvent();

        /// <summary>
        /// Raises the Printed event for this report.
        /// </summary>
        public void InvokePrinted()
        {
            OnPrinted(EventArgs.Empty);

            StiBlocklyHelper.InvokeBlockly(this, this, PrintedEvent);

            StiOptions.Engine.GlobalEvents.InvokeReportPrinted(this, EventArgs.Empty);
        }
        #endregion

        #region OnRefreshing
        /// <summary>
        /// Occurs when report starts refreshing.
        /// </summary>
        public event EventHandler Refreshing;

        protected virtual void OnRefreshing(EventArgs e)
        {
            this.Refreshing?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when report starts refreshing.
        /// </summary>
        [StiSerializable]
        [Description("Occurs when report starts refreshing.")]
        [StiCategory("RenderEvents")]
        [Category("RenderEvents")]
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiRefreshingEvent RefreshingEvent { get; set; } = new StiRefreshingEvent();

        /// <summary>
        /// Raises the Refreshing event for this report.
        /// </summary>
        public void InvokeRefreshing()
        {
            OnRefreshing(EventArgs.Empty);

            StiBlocklyHelper.InvokeBlockly(this, this, RefreshingEvent);

            StiOptions.Engine.GlobalEvents.InvokeReportRefreshing(this, EventArgs.Empty);
        }
        #endregion

        #region OnGetSubReport
        /// <summary>
        /// Occurs when report engine require Sub-Report.
        /// </summary>
        public event StiGetSubReportEventHandler GetSubReport;

        protected virtual void OnGetSubReport(StiGetSubReportEventArgs e)
        {
            this.GetSubReport?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the GetSubReport event for this report.
        /// </summary>
        internal void InvokeGetSubReport(StiGetSubReportEventArgs e)
        {
            OnGetSubReport(e);

            if (this.CompiledReport != null) CompiledReport.InvokeGetSubReport(e);
            if (this.ParentReport != null) ParentReport.OnGetSubReport(e);

            StiOptions.Engine.GlobalEvents.InvokeReportGetSubReport(this, e);
        }
        #endregion
        #endregion

        #region Units
        /// <summary>
        /// Gets the current unit.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [StiNonSerialized]
        public StiUnit Unit
        {
            get
            {
                return StiUnit.GetUnitFromReportUnit(ReportUnit);
            }
            set
            {
                if (value is StiCentimetersUnit)
                    ReportUnit = StiReportUnitType.Centimeters;

                else if (value is StiMillimetersUnit)
                    ReportUnit = StiReportUnitType.Millimeters;

                else if (value is StiInchesUnit)
                    ReportUnit = StiReportUnitType.Inches;

                else if (value is StiHundredthsOfInchUnit)
                    ReportUnit = StiReportUnitType.HundredthsOfInch;
            }
        }

        /// <summary>
        /// Converts all components coordinates from one unit into another.
        /// </summary>
        /// <param name="oldUnit">Old units.</param>
        /// <param name="newUnit">New units.</param>
        public void Convert(StiUnit oldUnit, StiUnit newUnit, bool isReportSnapshot = false)
        {
            if (oldUnit.GetType() == newUnit.GetType() || Pages == null) return;

            foreach (StiPage page in Pages)
            {
                page.Convert(oldUnit, newUnit);
            }
        }
        #endregion

        #region Methods.ChangeType
        /// <summary>
        /// Changes a type of the specified value. 
        /// </summary>
        /// <param name="value">A parameter which type will be changed.</param>
        /// <param name="conversionType">A type to which the value parameter will be converted.</param>
        /// <returns>Returns a value of the converted type.</returns>
        public static object ChangeType(object value, Type conversionType)
        {
            return ChangeType(value, conversionType, true);
        }

        /// <summary>
        /// Changes a type of the specified value. 
        /// </summary>
        /// <param name="value">A parameter which type will be changed.</param>
        /// <param name="conversionType">A type to which the value parameter will be converted.</param>
        /// <param name="convertNulls">A parameter which, when converting zero values, instead of null, returns String.Empty, false for Boolean, ' ' for char or null for DateTime.</param>
        /// <returns>Returns a value of the converted type.</returns>
        public static object ChangeType(object value, Type conversionType, bool convertNulls)
        {
            return StiConvert.ChangeType(value, conversionType, convertNulls);
        }
        #endregion

        #region Methods.Styles
        /// <summary>
        /// Applies styles associated with the specified style collection to all components in the report.
        /// </summary>
        public StiReport ApplyStyleCollection(string collectionName)
        {
            if (string.IsNullOrEmpty(collectionName))
                return this;

            collectionName = collectionName.ToLowerInvariant().Trim();

            var themStyles = new List<StiBaseStyle>();
            foreach (StiBaseStyle style in Styles)
            {
                if (style.CollectionName.ToLowerInvariant().Trim() == collectionName)
                    themStyles.Add(style);
            }

            var comps = this.GetComponents();

            foreach (StiComponent comp in comps)
            {
                if (comp is StiPage) continue;
                foreach (var style in themStyles)
                {
                    if (!StiStyleConditionHelper.IsAllowStyle(comp, style)) continue;
                    if (!(comp is StiChart))
                    {
                        if (comp is StiDataBand)
                        {
                            var isOddStyleDataBand = false;
                            var isEvenStyleDataBand = false;
                            foreach (StiStyleCondition condition in style.Conditions)
                            {
                                if ((condition.Placement & StiStyleComponentPlacement.DataOddStyle) > 0)
                                {
                                    isOddStyleDataBand = true;
                                    break;
                                }

                                if ((condition.Placement & StiStyleComponentPlacement.DataEvenStyle) > 0)
                                {
                                    isEvenStyleDataBand = true;
                                    break;
                                }
                            }

                            if (isOddStyleDataBand)
                                ((StiDataBand)comp).OddStyle = style.Name;

                            else if (isEvenStyleDataBand)
                                ((StiDataBand)comp).EvenStyle = style.Name;

                            else
                                comp.ComponentStyle = style.Name;
                        }
                        else comp.ComponentStyle = style.Name;
                    }
                }
            }

            this.ApplyStyles();

            return this;
        }

        /// <summary>
        /// Applies styles associated with the specified style collection to all components in the report.
        /// </summary>
        public StiReport ApplyStyleCollection(string collectionName, List<StiComponent> comps)
        {
            if (string.IsNullOrEmpty(collectionName))
                return this;

            collectionName = collectionName.ToLowerInvariant().Trim();

            var themStyles = new List<StiBaseStyle>();
            foreach (StiBaseStyle style in Styles)
            {
                if (style.CollectionName.ToLowerInvariant().Trim() == collectionName)
                    themStyles.Add(style);
            }

            foreach (StiComponent comp in comps)
            {
                if (comp is StiPage) continue;
                foreach (StiBaseStyle style in themStyles)
                {
                    if (!StiStyleConditionHelper.IsAllowStyle(comp, style)) continue;
                    if (!(comp is StiChart))
                    {
                        if (comp is StiDataBand)
                        {
                            var isOddStyleDataBand = false;
                            var isEvenStyleDataBand = false;
                            foreach (StiStyleCondition condition in style.Conditions)
                            {
                                if ((condition.Placement & StiStyleComponentPlacement.DataOddStyle) > 0)
                                {
                                    isOddStyleDataBand = true;
                                    break;
                                }

                                if ((condition.Placement & StiStyleComponentPlacement.DataEvenStyle) > 0)
                                {
                                    isEvenStyleDataBand = true;
                                    break;
                                }
                            }

                            if (isOddStyleDataBand)
                                ((StiDataBand)comp).OddStyle = style.Name;

                            else if (isEvenStyleDataBand)
                                ((StiDataBand)comp).EvenStyle = style.Name;

                            else
                                comp.ComponentStyle = style.Name;
                        }
                        else comp.ComponentStyle = style.Name;
                    }
                }
            }

            this.ApplyStyles();

            return this;
        }

        /// <summary>
        /// Applies styles of a report to all components in the report.
        /// </summary>
        public StiReport ApplyStyles()
        {
            var comps = this.GetComponents();

            foreach (StiComponent comp in comps)
            {
                var chart = comp as StiChart;
                if (chart != null && chart.AllowApplyStyle && chart.Style != null)
                {
                    var chartStyle = chart.Style;

                    chart.Core.ApplyStyle(chartStyle);

                    var customStyle = chartStyle as StiCustomStyle;
                    if (customStyle == null) continue;

                    var style = ((StiCustomStyleCoreXF)customStyle.Core).ReportStyle;
                    if (style != null)
                        style.SetStyleToComponent(comp);
                }
                else
                {
                    if (!string.IsNullOrEmpty(comp.ComponentStyle))
                    {
                        #region Apply styles to component
                        var style = this.Styles[comp.ComponentStyle];
                        if (style != null)
                            style.SetStyleToComponent(comp);
                        #endregion
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Renames a style with the specified name to a new name.
        /// </summary>
		internal void RenameStyle(string oldStylename, string newStyleName)
        {
            var comps = this.GetComponents();

            foreach (StiComponent comp in comps)
            {
                StiStylesHelper.ChangeComponentStyleName(comp, oldStylename, newStyleName);
            }
        }
        #endregion

        #region AppDomain
        internal AppDomain ReportDomain { get; set; }

        public StiReport CreateReportInNewAppDomain()
        {
#if NETSTANDARD || NETCOREAPP
            AppDomain appDomain = AppDomain.CreateDomain(StiGuidUtils.NewGuid());
            var report = StiActivator.CreateObject(StiOptions.Engine.BaseReportType) as StiReport;
#else
            var appDomainSetup = new AppDomainSetup
            {
                ShadowCopyFiles = "false",
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory
            };
            AppDomain appDomain = AppDomain.CreateDomain(StiGuidUtils.NewGuid(), AppDomain.CurrentDomain.Evidence, appDomainSetup);
            var report = appDomain.CreateInstanceAndUnwrap(this.GetType().Assembly.FullName, this.GetType().FullName) as StiReport;
#endif
            ReportDomain = appDomain;

            report.LoadFromString(this.SaveToString());

            foreach (StiData data in this.DataStore)
            {
                report.RegData(data.Name, data.Data);
                report.DataStore[report.DataStore.Count - 1].ViewData = data.ViewData;
            }

            return report;
        }

        public void UnloadReportAppDomain()
        {
            if (ReportDomain == null) return;

            AppDomain.Unload(ReportDomain);
            ReportDomain = null;
        }
        #endregion

        #region Inherited mode
        public void UpdateInheritedReport(StiReport masterReport = null)
        {
            if (masterReport != null)
            {
                masterReport.SetInheritedMode(true);
                StiInheritedReportComparer.Compare(this, masterReport);
                return;
            }

            if (string.IsNullOrEmpty(MasterReport)) return;

            masterReport = StiActivator.CreateObject(StiOptions.Engine.BaseReportType) as StiReport;
            masterReport.LoadFromString(MasterReport);
            masterReport.SetInheritedMode(true);

            StiInheritedReportComparer.Compare(this, masterReport);
        }

        private string masterReport = string.Empty;
        [StiBrowsable(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string MasterReport
        {
            get
            {
                return masterReport;
            }
            set
            {
                if (masterReport != value)
                {
                    masterReport = value;
                    UpdateInheritedReport();
                }
            }
        }

        public void SetInheritedMode(bool inherited)
        {
            foreach (StiVariable variable in this.Dictionary.Variables)
            {
                variable.Inherited = inherited;
            }

            foreach (StiDataSource dataSource in this.Dictionary.DataSources)
            {
                dataSource.Inherited = inherited;
            }

            foreach (StiBusinessObject businessObject in this.Dictionary.BusinessObjects)
            {
                businessObject.Inherited = inherited;
            }

            foreach (StiDatabase database in this.Dictionary.Databases)
            {
                database.Inherited = inherited;
            }

            foreach (StiDataRelation relation in this.Dictionary.Relations)
            {
                relation.Inherited = inherited;
            }

            StiComponentsCollection comps = this.GetComponents();
            foreach (StiComponent comp in comps)
            {
                comp.Inherited = inherited;
            }
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (CompiledReport != null)
                {
                    CompiledReport.Dispose();
                    CompiledReport = null;
                }
                if (!string.IsNullOrEmpty(ReportCachePath))
                {
                    StiReportCache.DeleteCache(ReportCachePath);
                    ReportCachePath = null;
                }

                if (reportMeasureGraphics != null)
                {
                    reportMeasureGraphics.Dispose();
                    reportMeasureGraphics = null;
                }

                if (!string.IsNullOrEmpty(ImageCachePath))
                {
                    StiFileImageCacheV2.DeleteCache(ImageCachePath);
                    ImageCachePath = null;
                }

                if (!string.IsNullOrEmpty(RtfCachePath))
                {
                    StiRtfCache.DeleteCache(RtfCachePath);
                    RtfCachePath = null;
                }

                #region Dispose All Components
                foreach (StiPage page in this.Pages)
                {
                    page.Watermark.ResetImage();

                    foreach (StiComponent comp in page.GetComponents())
                    {
                        var imageComp = comp as StiImage;
                        if (imageComp == null) continue;

                        imageComp.ResetImage();
                        imageComp.ResetImageToDraw();
                    }
                }

                foreach (StiPage page in this.RenderedPages)
                {
                    page.Watermark.ResetImage();

                    foreach (StiComponent comp in page.GetComponents())
                    {
                        var imageComp = comp as StiImage;
                        if (imageComp == null) continue;

                        imageComp.ResetImage();
                        imageComp.ResetImageToDraw();
                    }
                }
                #endregion

                if (this.InteractionCollapsingStates != null)
                {
                    this.InteractionCollapsingStates.Clear();
                    this.InteractionCollapsingStates = null;
                }

                if (this.RenderedPages != null)
                {
                    this.RenderedPages.Clear();
                }

                if (HashViewWpfPainter != null)
                {
                    HashViewWpfPainter.Clear();
                    HashViewWpfPainter = null;
                }

                if (RichTextImageCache != null)
                {
                    RichTextImageCache.Clear();
                    RichTextImageCache = null;
                }

                Helpers.FontVHelper.RemoveFontsFromResources(this);

                StiOptions.Engine.GlobalEvents.InvokeReportDisposed(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Methods.This
        /// <summary>
        /// Resets selection from all selected objects in the report.
        /// </summary>
        public void ResetSelection()
        {
            isSelected = false;
            foreach (StiPage page in Pages)
            {
                page.ResetSelection();
            }
        }

        /// <summary>
        /// Returns the current page.
        /// </summary>
        /// <returns>Current page.</returns>
        public StiPage GetCurrentPage()
        {
            if (CurrentPage < 0)
                CurrentPage = 0;

            if (CurrentPage > Pages.Count - 1) return null;

            return Pages[CurrentPage];
        }

        /// <summary>
        /// Returns the array of the selected components on the current page (including pages and modifiers).
        /// </summary>
        /// <returns>The array of the selected components.</returns>
        public object[] GetSelected()
        {
            var page = GetCurrentPage();

            var selectedComps = page?.GetSelectedComponents();
            if (selectedComps != null && selectedComps.Count > 0)
                return selectedComps.ToList().ToArray();

            if (page != null && page.IsSelected)
                return new object[] { page };

            if (IsSelected)
                return new object[] { this };

            return new object[0];
        }

        /// <summary>
        /// Dispose all cached rich text objects.
        /// </summary>
        internal void DisposeCachedRichText()
        {
            if (this.cachedRichText == null) return;

            this.cachedRichText.Dispose();
            this.cachedRichText = null;
        }

        /// <summary>
        /// Gets the report name.
        /// </summary>
        /// <returns></returns>
        internal string GetReportName()
        {
            return StiOptions.Engine.FullTrust
                ? StiNameValidator.CorrectName(ReportName, this)
                : ReportName;
        }

        /// <summary>
        /// Returns report engine version.
        /// </summary>
        public static string GetReportVersion()
        {
            var assembly = Assembly.GetAssembly(typeof(StiReport));
            var version = assembly.GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private void UpdateReportVersion()
        {
            ReportVersion = GetReportVersion();
        }

        public void WriteToReportRenderingMessages(string str)
        {
            if (ReportRenderingMessages == null)
                ReportRenderingMessages = new StringCollection();

            ReportRenderingMessages.Add(str);
        }

        public StiComponent GetComponentByName(string componentName)
        {
            foreach (StiPage page in Pages)
            {
                if (page.Name == componentName)
                    return page;
            }
            return Pages.GetComponentByName(componentName);
        }

        public StiComponent GetComponentByGuid(string guid)
        {
            foreach (StiPage page in Pages)
            {
                if (page.Guid == guid)
                    return page;
            }
            return Pages.GetComponentByGuid(guid);
        }

        /// <summary>
        /// Internal use only. Used only for converting an object to the string.
        /// </summary>
        protected internal string ToString(object obj)
        {
            return obj == null || obj == DBNull.Value ? string.Empty : obj.ToString();
        }

        protected internal object CheckExcelValue(object sender, object value)
        {
            var text = sender as StiText;

            var valueDec = StiValueHelper.TryToNullableDecimal(value);
            if (valueDec != null && valueDec < 0)
            {
                if (StiNegativeColorChecker.IsNegativeInRed(text.TextFormat))
                {
                    text.TextBrush = new StiSolidBrush(StiOptions.Engine.NegativeColor);
                }
                else if (!string.IsNullOrWhiteSpace(text.ComponentStyle))
                {
                    var style = Styles[text.ComponentStyle] as StiStyle;
                    if (style != null && style.AllowUseNegativeTextBrush)
                        text.TextBrush = style.NegativeTextBrush;
                }
            }

            if (text != null && text.Format != null && text.Format != "G" && text.Format.Length != 0)
                text.ExcelDataValue = ToString(value);

            return value;
        }

        protected internal string ToString(object sender, object obj)
        {
            return ToString(sender, obj, false);
        }

        protected internal string ToString(object sender, object obj, bool allowExcelCheck)
        {
            return ToString(allowExcelCheck ? CheckExcelValue(sender, obj) : obj);
        }

        internal void DisposeCachedRichTextFormat()
        {
            if (this.cachedRichTextFormat == null) return;

            this.cachedRichTextFormat.Dispose();
            this.cachedRichTextFormat = null;
        }

        /// <summary>
        /// Internal use only. Used for converting series of strings to one rtf text.
        /// </summary>
        protected string ConvertRtf(params object[] args)
        {
            if (args.Length < 2)
                return StiRichText.PackRtf(args[0] as string);

            if (cachedRichText == null)
                cachedRichText = new Controls.StiRichTextBox(false);

            cachedRichText.Text = string.Empty;

            var check = false;

            for (var index = 0; index < args.Length; index++)
            {
                var str = args[index] as string;

                var startIndex = cachedRichText.Text.Length;
                cachedRichText.SelectionStart = startIndex;
                cachedRichText.SelectionLength = 0;

                #region Rtf Text
                if (str.StartsWithInvariant("{\\"))
                {
                    if (cachedRichTextFormat == null)
                        cachedRichTextFormat = new Controls.StiRichTextBox(false);

                    cachedRichTextFormat.Rtf = str;

                    cachedRichTextFormat.SelectionStart = 0;
                    cachedRichTextFormat.SelectionLength = 1;

                    if (cachedRichTextFormat.SelectedText == "{")
                        check = true;
                    else
                        cachedRichText.SelectedRtf = str;
                }
                #endregion

                #region Expression in Rtf
                else
                {
                    cachedRichText.SelectedText = str;
                    cachedRichText.SelectionStart = startIndex;
                    cachedRichText.SelectionLength = cachedRichText.Text.Length - startIndex;

                    #region Copy format
                    if (check && cachedRichTextFormat != null)
                    {
                        check = false;

                        (cachedRichText as Controls.StiRichTextBox).SelectionAlignment = (cachedRichTextFormat as Controls.StiRichTextBox).SelectionAlignment;
                        cachedRichText.SelectionBullet = cachedRichTextFormat.SelectionBullet;
                        cachedRichText.SelectionCharOffset = cachedRichTextFormat.SelectionCharOffset;
                        cachedRichText.SelectionColor = cachedRichTextFormat.SelectionColor;
                        cachedRichText.SelectionFont = cachedRichTextFormat.SelectionFont;
                        cachedRichText.SelectionHangingIndent = cachedRichTextFormat.SelectionHangingIndent;
                        cachedRichText.SelectionIndent = cachedRichTextFormat.SelectionIndent;
                        cachedRichText.SelectionProtected = cachedRichTextFormat.SelectionProtected;
                        cachedRichText.SelectionRightIndent = cachedRichTextFormat.SelectionRightIndent;
                        cachedRichText.SelectionTabs = cachedRichTextFormat.SelectionTabs;
                    }
                    #endregion
                }
                #endregion
            }

            string result = cachedRichText.Rtf;

            #region Remove last empty line
            try
            {
                string endMarker = "\\par\r\n}\r\n";
                if (result != null && result.EndsWith(endMarker))
                {
                    int pos = result.Substring(0, result.Length - endMarker.Length).LastIndexOf("\r\n");
                    if (pos != -1)
                    {
                        string res2 = result.Substring(pos + 2);
                        Match m = Regex.Match(res2, @"\\f(?'font'\d{1,2})\\fs17\\lang\d{1,4}\\par", RegexOptions.Compiled | RegexOptions.Multiline);
                        if (!m.Success) m = Regex.Match(res2, @"\\f(?'font'\d{1,2})\\fs17\\par", RegexOptions.Compiled | RegexOptions.Multiline);
                        if (m.Success)
                        {
                            string fontNum = m.Groups["font"].Value;
                            MatchCollection ms = Regex.Matches(result, @"{\\f(?'font'\d{1,2})\\fnil\\fcharset\d{1,3} Microsoft Sans Serif;}", RegexOptions.Compiled | RegexOptions.Multiline);
                            foreach (Match match in ms)
                            {
                                if (match.Groups["font"].Value == fontNum)
                                {
                                    result = result.Substring(0, pos + 2 + m.Index) + "}\r\n";
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            #endregion

            return StiRichText.PackRtf(result);
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        public void SetReportSource(string value)
        {
            this.reportSource = value;
        }

        /// <summary>
        /// Gets or sets the last opened or saved file name.
        /// </summary>
        /// <returns>A name of the file.</returns>
        public string GetReportAssemblyCacheName()
        {
            return GetReportAssemblyCacheName(ReportFile);
        }

        private string GetReportAssemblyCacheName(string reportFile)
        {
            return string.IsNullOrEmpty(reportFile)
                ? $"{GetReportVersion()}.{ReportGuid}.dll"
                : $"{Path.GetFileNameWithoutExtension(reportFile)}.{GetReportVersion()}.{ReportGuid}.dll";
        }

        internal string GenerateReportGuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        /// <summary>
        /// Returns the name of the report and its alias.
        /// </summary>
        public override string ToString()
        {
            var useAliases = Designer?.UseAliases;

            if (useAliases.GetValueOrDefault(false) && !string.IsNullOrWhiteSpace(ReportAlias))
                return ReportAlias;

            if (ReportName == ReportAlias || string.IsNullOrWhiteSpace(ReportAlias))
                return ReportName;

            return $"{ReportName} [{ReportAlias}]";
        }

        protected DateTime ParseDateTime(string value)
        {
            return StiVariable.GetDateTimeFromValue(value);
        }

        protected DateTimeOffset ParseDateTimeOffset(string value)
        {
            return StiVariable.GetDateTimeOffsetFromValue(value);
        }

        protected TimeSpan ParseTimeSpan(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value == "null") 
                return TimeSpan.Zero;

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                return TimeSpan.Parse(value);
            }
            catch
            { 
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
            return TimeSpan.Zero;
        }

        /// <summary>
        /// Returns events collection of this component.
        /// </summary>
        public virtual StiEventsCollection GetEvents()
        {
            var events = new StiEventsCollection();

            if (BeginRenderEvent != null)
                events.Add(BeginRenderEvent);

            if (EndRenderEvent != null)
                events.Add(EndRenderEvent);

            if (ExportingEvent != null)
                events.Add(ExportingEvent);

            if (ExportedEvent != null)
                events.Add(ExportedEvent);

            if (PrintingEvent != null)
                events.Add(PrintingEvent);

            if (PrintedEvent != null)
                events.Add(PrintedEvent);

            if (RenderingEvent != null)
                events.Add(RenderingEvent);

            return events;
        }

        /// <summary>
        /// Returns a list of all components, including pages in the report.
        /// </summary>
        /// <returns>List of all components.</returns>
        public StiComponentsCollection GetComponents()
        {
            var comps = new StiComponentsCollection();

            foreach (StiPage page in Pages)
            {
                comps.Add(page);
                page.GetComponents(ref comps);
            }
            return comps;
        }

        /// <summary>
        /// Returns a list of all rendered components, including rendered pages in the report.
        /// </summary>
        /// <returns>List of all rendered components.</returns>
        public StiComponentsCollection GetRenderedComponents()
        {
            var comps = new StiComponentsCollection();

            foreach (StiPage page in RenderedPages)
            {
                comps.Add(page);
                page.GetComponents(ref comps);
            }
            return comps;
        }

        /// <summary>
        /// Returns a list of all rendered components, including rendered pages in the report.
        /// </summary>
        /// <returns>List of all rendered components.</returns>
        public StiComponentsCollection GetRenderedComponents<T>() where T : StiComponent
        {
            var comps = new StiComponentsCollection();

            foreach (StiPage page in RenderedPages)
            {
                if (page is T)
                    comps.Add(page);

                page.GetComponents<T>(ref comps);
            }

            return comps;
        }

        /// <summary>
        /// Returns count of all components of the report.
        /// </summary>
        public int GetComponentsCount()
        {
            int count = 0;

            foreach (StiPage page in Pages)
            {
                count += 1;
                count += page.GetComponentsCount();
            }

            return count;
        }

        /// <summary>
        /// Returns count of all rendered components of the report.
        /// </summary>
        public int GetRenderedComponentsCount()
        {
            int count = 0;

            foreach (StiPage page in RenderedPages)
            {
                count += 1;
                count += page.GetComponentsCount();
            }

            return count;
        }

        /// <summary>
        /// Shows the specified component in the window of viewer.
        /// </summary>
        public void GotoComponent(StiComponent component)
        {
            InvokeGotoComp(new StiGotoCompEventArgs(component));
        }

        /// <summary>
        /// Returns the component by its tag value.
        /// </summary>
        /// <param name="tag">The tag value to find a component.</param>
        /// <returns>A Component which was found. If the component is not found then null.</returns>
        public StiComponent FindComponentTag(object tag)
        {
            var comps = new StiComponentsCollection();

            foreach (StiPage page in RenderedPages)
            {
                comps.Add(page);
                comps.AddRange(page.GetComponents());
            }

            foreach (StiComponent comp in comps)
            {
                if (comp.TagValue is string && tag is string)
                {
                    if (comp.TagValue.ToString() == tag.ToString())
                    {
                        return comp;
                    }
                }
                else if (comp.TagValue == tag)
                {
                    return comp;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the component by the bookmark value.
        /// </summary>
        /// <param name="bookmark">The bookmark value to find a component.</param>
        /// <returns>Found component. If the component is not found then null.</returns>
        public StiComponent FindComponentBookmark(object bookmark)
        {
            var comps = new StiComponentsCollection();

            foreach (StiPage page in RenderedPages)
            {
                comps.Add(page);
                comps.AddRange(page.GetComponents());
            }

            foreach (StiComponent comp in comps)
            {
                if (comp.BookmarkValue is string && bookmark is string)
                {
                    if (comp.BookmarkValue.ToString() == bookmark.ToString())
                    {
                        return comp;
                    }
                }
                else if (comp.TagValue == bookmark)
                {
                    return comp;
                }
            }

            return null;
        }

        /// <summary>
        /// Shows the component with the specified value of the tag in the window of viewer.
        /// </summary>
        /// <param name="tag">A Tag value of the component.</param>
        public void GotoComponentTag(object tag)
        {
            var component = FindComponentTag(tag);
            if (component != null)
                GotoComponent(component);
        }

        /// <summary>
        /// Shows the component with the specified value of the bookmark in the window of viewer.
        /// </summary>
        /// <param name="tag">A Tag value of the component.</param>
        public void GotoComponentBookmark(object tag)
        {
            var component = FindComponentBookmark(tag);
            if (component != null)
                GotoComponent(component);
        }

        /// <summary>
        /// Draws again the window of viewer.
        /// </summary>
        public void Invalidate(StiComponent comp = null)
        {
            InvokePaint(comp, EventArgs.Empty);
        }

        /// <summary>
        /// Adds anchor.
        /// </summary>
        /// <param name="value">An anchor value.</param>
        public void AddAnchor(object value)
        {
            Anchors[value] = new StiRuntimeVariables(this);
        }

        /// <summary>
        /// Adds anchor.
        /// </summary>
        /// <param name="value">An anchor value.</param>
        /// <param name="component">The sender object from the GetTag event.</param>
        public void AddAnchor(object value, object component)
        {
            if (this.EngineVersion == StiEngineVersion.EngineV1)
            {
                Anchors[value] = new StiRuntimeVariables(this);
            }
            else
            {
                if (!Anchors.ContainsKey(value))
                {
                    Anchors[value] = new KeyValuePair<StiRuntimeVariables, object>(new StiRuntimeVariables(this), component);
                    var comp = component as StiComponent;
                    if (comp != null && comp.Guid == null)
                        comp.Guid = value as string;
                }
            }
        }

        /// <summary>
        /// Gets the page number by the value of an anchor.
        /// </summary>
        /// <param name="value">An anchor value.</param>
        /// <returns>Page number.</returns>
        public int GetAnchorPageNumber(object value)
        {
            #region Get real component placement; for AddAnchor overload with 2 parameters
            var obj = value != null ? Anchors[value] : null;
            if (this.EngineVersion == StiEngineVersion.EngineV2 && obj is KeyValuePair<StiRuntimeVariables, object>)
            {
                var pair = (KeyValuePair<StiRuntimeVariables, object>)obj;
                var startPageNumber = 0;
                if (pair.Key != null)
                    startPageNumber = this.Engine.PageNumbers.GetPageNumber(pair.Key.Page);

                var indexPageNumber = startPageNumber - 1;
                while (indexPageNumber < this.RenderedPages.Count)
                {
                    var page = this.RenderedPages[indexPageNumber];
                    this.RenderedPages.GetPage(page);

                    //var comps = page.GetComponents();
                    //for optimization, we will assume that it is called only in the EndRender, when the page is already flat
                    var comps = page.Components;
                    foreach (StiComponent comp in comps)
                    {
                        if (comp == pair.Value)
                        {
                            if (page == pair.Key.Page)
                                return startPageNumber;

                            return this.Engine.PageNumbers.GetPageNumber(page);
                        }
                    }
                    indexPageNumber++;
                }
                return startPageNumber;
            }
            #endregion

            var variables = GetAnchor(value);
            if (variables == null) return 0;

            return this.EngineVersion == StiEngineVersion.EngineV1
                ? variables.Page.PageInfoV1.PageNumber
                : this.Engine.PageNumbers.GetPageNumber(variables.Page);
        }

        /// <summary>
        /// Gets the page number through by the value of an anchor.
        /// </summary>
        /// <param name="value">An anchor value.</param>
        /// <returns>Page number through.</returns>
        public int GetAnchorPageNumberThrough(object value)
        {
            #region Get real component placement; for AddAnchor overload with 2 parameters
            var obj = Anchors[value];
            if (this.EngineVersion == StiEngineVersion.EngineV2 && obj is KeyValuePair<StiRuntimeVariables, object>)
            {
                var pair = (KeyValuePair<StiRuntimeVariables, object>)obj;
                var startPageNumber = 0;
                if (pair.Key != null)
                    startPageNumber = this.Engine.PageNumbers.GetPageNumberThrough(pair.Key.Page);

                var indexPageNumber = startPageNumber - 1;
                while (indexPageNumber < this.RenderedPages.Count)
                {
                    var page = this.RenderedPages[indexPageNumber];
                    this.RenderedPages.GetPage(page);
                    var comps = page.GetComponents();
                    foreach (StiComponent comp in comps)
                    {
                        if (comp == pair.Value)
                        {
                            return page == pair.Key.Page
                                ? startPageNumber
                                : this.Engine.PageNumbers.GetPageNumberThrough(page);
                        }
                    }
                    indexPageNumber++;
                }
                return startPageNumber;
            }
            #endregion

            var variables = GetAnchor(value);
            if (variables == null) return 0;

            return this.EngineVersion == StiEngineVersion.EngineV1
                ? variables.Page.PageInfoV1.PageNumber
                : this.Engine.PageNumbers.GetPageNumberThrough(variables.Page);
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        /// <returns></returns>
        public int GetTotalRenderedPageCount()
        {
            int totalRenderedPageCount = 0;
            foreach (StiPage page in RenderedPages)
            {
                totalRenderedPageCount += page.SegmentPerWidth * page.SegmentPerHeight;
            }
            return totalRenderedPageCount;
        }

        /// <summary>
        /// Gets runtime values by the value of an anchor.
        /// </summary>
        /// <param name="value">An anchor value.</param>
        /// <returns>A runtime value.</returns>
        public StiRuntimeVariables GetAnchor(object value)
        {
            if (value == null)
                return new StiRuntimeVariables(this);

            return Anchors[value] as StiRuntimeVariables;
        }

        /// <summary>
        /// Gets the page index by the page number.
        /// </summary>
        /// <param name="pageNumber">A page number.</param>
        /// <returns>Page index.</returns>
        internal int GetPageIndex(int pageNumber)
        {
            int x;
            int y;
            return GetPageIndex(pageNumber, out x, out y);
        }

        /// <summary>
        /// Returns the page index by page number and indexes of the segment. Used for segmented pages.
        /// </summary>
        /// <param name="pageNumber">A page number.</param>
        /// <param name="x">A segment index of X-direction.</param>
        /// <param name="y">A segment index of Y-direction.</param>
        /// <returns>A page index.</returns>
        public int GetPageIndex(int pageNumber, out int x, out int y)
        {
            int index = 0;
            StiPagesCollection pages = null;
            if (IsDesigning) pages = Pages;
            else pages = RenderedPages;
            pageNumber--;

            int num = 0;

            foreach (StiPage page in pages)
            {
                int start = num;
                int segmentPerWidth = Math.Max(page.SegmentPerWidth, 1);
                int segmentPerHeight = Math.Max(page.SegmentPerHeight, 1);

                int end = num + segmentPerWidth * segmentPerHeight;

                if (pageNumber >= start && pageNumber < end)
                {
                    pageNumber = pageNumber - start;

                    y = pageNumber / segmentPerWidth;
                    x = pageNumber - y * segmentPerWidth;
                    return index;
                }
                num = end;
                index++;
            }
            x = y = 0;

            return -1;
        }

        /// <summary>
        /// Returns a page number by the index. Internal use only.
        /// </summary>
        /// <param name="index">A page index.</param>
        /// <returns>A page number.</returns>
        public int GetPageNumber(int index)
        {
            int startPage = 0;
            int endPage = 0;
            GetPageNumber(index, out startPage, out endPage);
            return startPage;
        }

        /// <summary>
        /// Returns page numbers by the index. Internal use only.
        /// </summary>
        /// <param name="index">A page index.</param>
        /// <param name="startPageNumber">The start page number.</param>
        /// <param name="endPageNumber">The end page number.</param>
        public void GetPageNumber(int index, out int startPageNumber, out int endPageNumber)
        {
            int num = 1;
            int ind = 0;
            startPageNumber = 0;
            endPageNumber = 0;

            var pages = IsDesigning ? Pages : RenderedPages;

            foreach (StiPage page in pages)
            {
                if (ind == index)
                {
                    startPageNumber = num;
                    endPageNumber = num + page.SegmentPerWidth * page.SegmentPerHeight - 1;
                    return;
                }
                num += page.SegmentPerWidth * page.SegmentPerHeight;
                ind++;
            }
        }
        #endregion

        #region Methods.Static
        /// <summary>
        /// Clear all report cache.
        /// </summary>
        public static void ClearReportCache()
        {
            StiReportCache.ClearCache();
        }

        /// <summary>
        /// Clear all image cache.
        /// </summary>
        public static void ClearImageCache()
        {
            StiFileImageCacheV2.ClearCache();
        }

        /// <summary>
        /// Returns new StiReport object implementation. Method use type from StiOptions.Engine.BaseReportType property.
        /// </summary>
        /// <returns></returns>
        public static StiReport GetReport()
        {
            return StiActivator.CreateObject(StiOptions.Engine.BaseReportType) as StiReport;
        }
        #endregion

        #region Fields
        private RichTextBox cachedRichText;

        private RichTextBox cachedRichTextFormat;
        #endregion

        #region Properties.Component
        [Browsable(false)]
        [StiBrowsable(false)]
        public new IContainer Container => base.Container;


        [Browsable(false)]
        [StiBrowsable(false)]
        public override ISite Site
        {
            get
            {
                return base.Site;
            }
            set
            {
                base.Site = value;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Internal use only.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        public virtual bool UseProgressInThread { get; set; }

        /// <summary>
        /// Gets the collection of meta tags. Meta tags can be used for storing specific information in report. Meta tags will be saved in report file.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public virtual StiMetaTagCollection MetaTags { get; } = new StiMetaTagCollection();

        private StiReportResourceCollection reportResources;
        /// <summary>
        /// Gets collection of report resources. Internal use only.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List, StiSerializeTypes.SerializeToCode)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public virtual StiReportResourceCollection ReportResources
        {
            get
            {
                return reportResources ?? (reportResources = new StiReportResourceCollection());
            }
        }

        /// <summary>
        /// Gets or sets version of the report engine which changed the report last time.
        /// </summary>
        [StiSerializable]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        [DefaultValue("")]
        [Editor("Stimulsoft.Report.Components.Design.StiSimpleTextEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public string ReportVersion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets report engine core (EngineV2). Internal use only.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public StiEngine Engine { get; set; }

        /// <summary>
        /// Gets or sets report engine code (EngineV1). Internal use only.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public StiEngineV1 EngineV1 { get; }

        /// <summary>
        /// Gets or sets collection which contains report rendering messages.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public StringCollection ReportRenderingMessages { get; set; }

        /// <summary>
        /// Internal use only.
        /// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public Hashtable InteractionCollapsingStates { get; set; }

        private string statusString = "";
        /// <summary>
        /// Gets or sets string representation of the current report status when report is rendered.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public string StatusString
        {
            get
            {
                return statusString;
            }
            set
            {
                if (statusString == value) return;

                statusString = value;

                if (CompiledReport != null)
                    CompiledReport.StatusString = value;

                if (ParentReport != null)
                    ParentReport.StatusString = value;

                if (Progress != null)
                    Progress.Update(StatusString);

                InvokeStatusChanged();
            }
        }

        private int progressOfRendering;
        /// <summary>
        /// Gets or sets value of progress of report rendering (0-100).
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public int ProgressOfRendering
        {
            get
            {
                return progressOfRendering;
            }
            set
            {
                if (progressOfRendering == value) return;

                progressOfRendering = value;

                if (CompiledReport != null)
                    CompiledReport.ProgressOfRendering = value;

                if (ParentReport != null)
                    ParentReport.ProgressOfRendering = value;
            }
        }

        /// <summary>
        /// Gets or sets progress information.
        /// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public IStiProgressInformation Progress { get; set; }

        /// <summary>
        /// Gets or sets the collection of the subreports of the report.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public StiReportsCollection SubReports { get; set; }

        private string reportSource;
        /// <summary>
        /// Gets or sets string representation of report template file. This property used when report placed on form.
        /// </summary>
		[Browsable(false)]
        [StiBrowsable(false)]
        public string ReportSource
        {
            get
            {
                return reportSource;
            }
            set
            {
                reportSource = value;
                if (string.IsNullOrEmpty(value) || DesignMode) return;

                if (value.Length > 2 && value[0] == '<' && value[1] == '?')
                    this.LoadFromString(value);

                else
                    this.LoadPackedReportFromString(value);
            }
        }

        /// <summary>
        /// Gets or sets the report key.
        /// </summary>
        [StiBrowsable(false)]
        [Browsable(false)]
        [StiSerializable]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the report Guid.
        /// </summary>
        [StiBrowsable(false)]
        [Browsable(false)]
        [StiSerializable]
        public string ReportGuid { get; set; }

        /// <summary>
        /// Gets or sets Tag property of the report.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Tag
        {
            get;
            set;
        }

        private string reportCachePath = "";
        /// <summary>
        /// Gets path to the report cache path. Path can't be changed.
        /// </summary>
		[Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ReportCachePath
        {
            get
            {
                return CompiledReport == null ? this.reportCachePath : CompiledReport.ReportCachePath;
            }
            set
            {
                if (CompiledReport == null)
                    this.reportCachePath = value;
                else
                    CompiledReport.ReportCachePath = value;
            }
        }

        /// <summary>
        /// Gets path to the report image cache path. Path can't be changed.
        /// </summary>
		[Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ImageCachePath { get; set; } = "";

        /// <summary>
        /// Gets path to the report rtf cache path. Path can't be changed.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string RtfCachePath { get; set; } = "";

        /// <summary>
        /// Gets or sets the report which is a template (is not compiled) for this report.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiReport ParentReport { get; set; }

        /// <summary>
        /// Gets a collection which consists of the template pages from the currect report.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [StiSerializable(StiSerializationVisibility.List)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiPagesCollection Pages { get; }

        private StiPagesCollection renderedPages;
        /// <summary>
        /// Gets a collection of rendered pages from the current report.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiPagesCollection RenderedPages
        {
            get
            {
                return CompiledReport != null ? CompiledReport.RenderedPages : renderedPages;
            }
            set
            {
                if (CompiledReport != null)
                    CompiledReport.renderedPages = value;
                else
                    renderedPages = value;
            }
        }

        /// <summary>
        /// Gets or sets information which is necessary for the report designer.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiDesignerInfo Info { get; set; }

        /// <summary>
        /// Gets a report designer.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IStiDesignerBase Designer { get; set; }

        internal StiBookmark pointerValue;
        /// <summary>
        /// Gets or sets the root bookmark. Internal use only.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiBookmark Pointer
        {
            get
            {
                return CompiledReport != null ? CompiledReport.Pointer : pointerValue;
            }
            set
            {
                pointerValue = value;

                if (CompiledReport != null)
                    CompiledReport.Pointer = value;
            }
        }

        internal StiBookmark bookmarkValue;
        /// <summary>
        /// Gets or sets the root bookmark. Internal use only.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiBookmark Bookmark
        {
            get
            {
                return CompiledReport != null ? CompiledReport.Bookmark : bookmarkValue;
            }
            set
            {
                bookmarkValue = value;
                if (CompiledReport != null)
                    CompiledReport.Bookmark = value;
            }
        }

        private StiBookmark manualBookmark = new StiBookmark();
        /// <summary>
        /// Gets or sets the root manual bookmark. Please use Bookmark property in EngineV2.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiBookmark ManualBookmark
        {
            get
            {
                return this.EngineVersion == StiEngineVersion.EngineV1 ? manualBookmark : this.Bookmark;
            }
            set
            {
                if (this.EngineVersion == StiEngineVersion.EngineV1)
                    manualBookmark = value;
                else
                    this.Bookmark = value;
            }
        }

        private Graphics reportMeasureGraphics;
        /// <summary>
        /// Graphics used for report rendering measurement. Internal use only.
        /// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public Graphics ReportMeasureGraphics
        {
            get
            {
                if (reportMeasureGraphics == null)
                {
                    if (StiOptions.Engine.OldWYSIWYG)
                    {
                        reportMeasureGraphics = Graphics.FromImage(new Bitmap(1, 1));
                        reportMeasureGraphics.PageUnit = GraphicsUnit.Inch;
                        reportMeasureGraphics.PageScale = .01f;
                    }
                    else
                    {
                        reportMeasureGraphics = Graphics.FromImage(new Bitmap(1, 1));
                        reportMeasureGraphics.PageUnit = GraphicsUnit.Pixel;
                        reportMeasureGraphics.PageScale = 1f;
                    }
                }
                return reportMeasureGraphics;
            }
            private set
            {
                reportMeasureGraphics = value;
            }
        }

        private Hashtable totals = new Hashtable();
        /// <summary>
        /// Internal use only.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        internal Hashtable Totals
        {
            get
            {
                return ParentReport != null ? ParentReport.totals : totals;
            }
            set
            {
                if (ParentReport != null)
                    ParentReport.totals = value;
                else
                    totals = value;
            }
        }

        private StiCells cells;
        /// <summary>
        /// Property used for access to internal CrossTab variables. Internal use only.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        protected internal StiCells Cells => cells ?? (cells = new StiCells());

        /// <summary>
        /// Internal use only.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Password { get; private set; }

        /// <summary>
        /// Internal use only.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [StiBrowsable(false)]
        [Browsable(false)]
        public string[] DataBandsUsedInPageTotals { get; set; }

        /// <summary>
        /// Internal use only. List of used datasources for RetrieveOnlyUsedData feature.
        /// </summary>
        //[StiSerializable(StiSerializationVisibility.List, StiSerializeTypes.SerializeToCode)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [StiBrowsable(false)]
        [Browsable(false)]
        public string[] ListOfUsedData { get; set; }

        /// <summary>
        /// Gets or sets a technology a report was rendered with.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        public StiRenderedWith RenderedWith { get; internal set; } = StiRenderedWith.Unknown;

        /// <summary>
        /// Internal use only.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        public CookieContainer CookieContainer { get; set; }

        /// <summary>
        /// Internal use only.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        public NameValueCollection HttpHeadersContainer { get; set; }
        #endregion

        #region Properties.Internal
        /// <summary>
        /// The master report for subreports. Do not use this field.
        /// </summary>
        internal StiReport SubReportsMasterReport { get; set; }

        internal bool SubReportsResetPageNumber { get; set; }

        internal bool SubReportsPrintOnPreviousPage { get; set; }

        internal List<StiRichText> RichTextImageCache { get; set; }

        internal int IndexName { get; set; } = 1;

        internal bool ContainsTables { get; set; }

        internal Hashtable HashViewWpfPainter { get; set; }

        internal Hashtable CachedTotals { get; set; }

        internal bool CachedTotalsLocked { get; set; }

        internal bool FlagFontSmoothing { get; set; }

        internal AppDomain WpfRichTextDomain { get; set; }

        //a list of RequestFromUser variables, that have been modified from the outside
        internal Hashtable ModifiedVariables { get; } = new Hashtable();

        internal Hashtable Anchors { get; set; } = new Hashtable();
        #endregion

        #region Properties.States
        /// <summary>
        /// Gets or sets value which indicates which report pass is rendered now. Internal use only.
        /// </summary>
        [StiBrowsable(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal StiReportPass ReportPass { get; set; } = StiReportPass.None;

        /// <summary>
        /// Gets or sets value which indicates whether the rendered report is edited in viewer window.
        /// Please use property 'IsEditedInViewer' instead property 'IsEditedInPreview'.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        [Obsolete("Please use property 'IsEditedInViewer' instead property 'IsEditedInPreview'.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsEditedInPreview
        {
            get
            {
                return this.IsEditedInViewer;
            }
            set
            {
                this.IsEditedInViewer = value;
            }
        }

        /// <summary>
        /// Gets or sets value which indicates whether the rendered report is edited in preview window.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public bool IsEditedInViewer { get; set; }

        /// <summary>
        /// Gets or sets value which indicates whether the report is rendered or not.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public bool IsRendered { get; set; }

        /// <summary>
        /// Gets or sets value which indicates whether the report rendering is in process.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public bool IsRendering { get; set; }

        private bool isModified;
        /// <summary>
        /// Gets or sets value, which indicates that the report was changed in the designer.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsModified
        {
            get
            {
                return isModified;
            }
            set
            {
                if (isModified == value) return;

                isModified = value;

                var newReportGuid = GenerateReportGuid();

                ReportGuid = newReportGuid;
            }
        }

        private bool isStopped;
        /// <summary>
        /// Gets or sets value, which indicates that the report rendering / printing was stopped.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsStopped
        {
            get
            {
                return CompiledReport != null ? CompiledReport.IsStopped : isStopped;
            }
            set
            {
                if (CompiledReport != null)
                    CompiledReport.IsStopped = value;

                isStopped = value;
            }
        }

        /// <summary>
        /// Please use property IsStopped' instead 'Stop' property.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("Please use property IsStopped' instead 'Stop' property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Stop
        {
            get
            {
                return this.IsStopped;
            }
            set
            {
                this.IsStopped = value;
            }
        }

        /// <summary>
        /// Gets value, which indicates that the report is exporting.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsExporting { get; internal set; }

        /// <summary>
        /// Gets or sets value, which indicates that the designer of the report is designing a page from viewer.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPageDesigner { get; set; }

        /// <summary>
        /// Gets or sets value, which indicates that serialization and deserialization is in process.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSerializing { get; set; }

        /// <summary>
        /// Gets or sets value, which indicates that the report printing is in process.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPrinting { get; set; }

        /// <summary>
        /// Gets or sets value, which indicates that the report contains dashboard pages.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ContainsDashboard => Pages.ToList().Any(p => p.IsDashboard && p.Enabled);

        /// <summary>
        /// Gets or sets value, which indicates that the report contains only dashboard pages.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ContainsOnlyDashboard => Pages.ToList().Where(p => p.Enabled).All(p => p.IsDashboard);

        /// <summary>
        /// Gets or sets value, which indicates that the report contains form.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ContainsForm => Pages.ToList().Any(p => p is StiFormContainer && p.Enabled);

        /// <summary>
        /// Gets value, which indicates that the report is being designed.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDesigning => Designer != null || Info.ForceDesigningMode || IsPageDesigner;

        /// <summary>
        /// Gets value, which indicates that the report is compiled.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsCompiled => CompiledReport != null;

        /// <summary>
        /// Gets value, which indicates that report template contain only one template page and 
        /// this template page does not contain any components.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public bool IsReportEmpty => Pages.Count == 1 && Pages[0].Components.Count == 0;

        /// <summary>
        /// Gets value, which indicates that rendered report contain only one rendered page and 
        /// this rendered page does not contain any components.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public bool IsRenderedReportEmpty => RenderedPages.Count == 1 && RenderedPages[0].Components.Count == 0;

        /// <summary>
        /// Gets value which indicates that report loaded from packed format.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public bool IsPackedReport { get; private set; }

        /// <summary>
        /// Gets value which indicates that report loaded from JSON format.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public bool IsJsonReport { get; private set; }

        /// <summary>
        /// Gets value which indicates that this report is rendered with using WPF technology.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public bool IsWpf { get; internal set; }

        /// <summary>
        /// Internal use only.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        [Obsolete("Please use 'Report.IsReportRenderingAfterSubmit' property instead 'Report.IsPreviewDialogs' property.")]
        public bool IsPreviewDialogs
        {
            get
            { 
                return IsReportRenderingAfterSubmit; 
            }
            set
            {
                IsReportRenderingAfterSubmit = value;
            }
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public bool IsReportRenderingAfterSubmit { get; set; }

        /// <summary>
        /// Internal use only.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        [Obsolete("The property 'IsSLPreviewDialogs' is not used more.")]
        public bool IsSLPreviewDialogs { get; set; }

        /// <summary>
        /// Gets value which indicates that this report contain rendered document which loaded from file or other source.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public bool IsDocument { get; set; }

        /// <summary>
        /// Gets value which indicates that current rendering of report is used for interaction purposes. Internal use only.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public bool IsInteractionRendering { get; set; }
        #endregion

        #region Properties.Description
        /// <summary>
        /// Gets or sets a report name.
        /// </summary>
        [StiSerializable]
        [StiCategory("Design")]
        [Category("Design")]
        [ParenthesizePropertyName(true)]
        [RefreshProperties(RefreshProperties.All)]
        [Description("Gets or sets a report name.")]
        [StiOrder(StiPropertyOrder.ReportDescriptionReportName)]
        [Editor("Stimulsoft.Report.Components.Design.StiSimpleTextEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public string ReportName
        {
            get
            {
                return PropName;
            }
            set
            {
                if (PropName == ReportAlias)
                    ReportAlias = value;

                PropName = StiOptions.Designer.AutoCorrectReportName
                    ? StiNameValidator.CorrectName(value, this)
                    : value;

                StiOptions.Engine.GlobalEvents.InvokeReportNameChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a report alias.
        /// </summary>
        [StiSerializable]
        [StiCategory("Design")]
        [Category("Design")]
        [ParenthesizePropertyName(true)]
        [Description("Gets or sets a report alias.")]
        [StiOrder(StiPropertyOrder.ReportDescriptionReportAlias)]
        [Editor("Stimulsoft.Report.Components.Design.StiSimpleTextEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public string ReportAlias { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a report author.
        /// </summary>
        [StiSerializable]
        [DefaultValue("")]
        [StiCategory("Design")]
        [Category("Design")]
        [ParenthesizePropertyName(true)]
        [Description("Gets or sets a report author.")]
        [StiOrder(StiPropertyOrder.ReportDescriptionReportAuthor)]
        [Editor("Stimulsoft.Report.Components.Design.StiSimpleTextEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public string ReportAuthor { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a report description.
        /// </summary>
        [StiSerializable]
        [DefaultValue("")]
        [StiCategory("Design")]
        [Category("Design")]
        [ParenthesizePropertyName(true)]
        [Description("Gets or sets a report description.")]
        [StiOrder(StiPropertyOrder.ReportDescriptionReportDescription)]
        [Editor("Stimulsoft.Report.Components.Design.StiSimpleTextEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public string ReportDescription { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a report image.
        /// </summary>
        [StiSerializable]
        [DefaultValue(null)]
        [StiCategory("Design")]
        [Category("Design")]
        [ParenthesizePropertyName(true)]
        [Description("Gets or sets a report image.")]
        [StiOrder(StiPropertyOrder.ReportDescriptionReportImage)]
        [Editor("Stimulsoft.Report.Components.Design.StiSimpleImageEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiSimpeImageConverter))]
        public Image ReportImage { get; set; }

        /// <summary>
        /// Gets or sets a report icon.
        /// </summary>
        [StiSerializable]
        [DefaultValue(null)]
        [StiCategory("Design")]
        [Category("Design")]
        [ParenthesizePropertyName(true)]
        [Description("Gets or sets a report icon.")]
        [StiOrder(StiPropertyOrder.ReportDescriptionReportIcon)]
        [Editor("Stimulsoft.Report.Components.Design.StiReportIconEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiReportIconConverter))]
        public byte[] ReportIcon { get; set; }

        /// <summary>
        /// Gets or sets the date of the report creation.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets or sets the date of the report creation.")]
        public DateTime ReportCreated { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the date of the last report changes.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets or sets the date of the last report changes.")]
        public DateTime ReportChanged { get; set; } = DateTime.Now;
        #endregion

        #region Properties.Main
        private StiEngineVersion engineVersion = StiEngineVersion.EngineV2;
        /// <summary>
        /// Gets or sets version of engine which will be used for report rendering. 
        /// EngineV1 is old version of engine. EngineV2 is newest version of engine. Its more powerful and more flexible.
        /// </summary>
        [StiSerializable]
        [StiCategory("Engine")]
        [Category("Engine")]
        [StiOrder(StiPropertyOrder.ReportMainEngineVersion)]
        [Description("Gets or sets version of engine which will be used for report rendering. EngineV1 is old version of engine. EngineV2 is newest version of engine. Its more powerful and more flexible.")]
        [Editor("Stimulsoft.Report.Design.StiEngineVersionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Professional)]
        public StiEngineVersion EngineVersion
        {
            get
            {
                return engineVersion;
            }
            set
            {
                if (engineVersion == value) return;

                if (value == StiEngineVersion.All)
                    value = StiEngineVersion.EngineV1;

                engineVersion = value;
            }
        }

        /// <summary>
        /// Gets a collection which consists of report styles.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Engine")]
        [StiCategory("Engine")]
        [Editor("Stimulsoft.Report.Design.StiStylesEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(StiStylesConverter))]
        [Description("Gets a collection which consists of report styles.")]
        [Browsable(false)]
        [StiOrder(StiPropertyOrder.ReportMainStyles)]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiStylesCollection Styles { get; }

        /// <summary>
        /// Gets or sets the number of passes which the report generator makes while report rendering.
        /// </summary>
        [StiSerializable]
        [StiCategory("Engine")]
        [Category("Engine")]
        [DefaultValue(StiNumberOfPass.SinglePass)]
        [StiOrder(StiPropertyOrder.ReportMainNumberOfPass)]
        [Description("Gets or sets the number of passes which the report generator makes while report rendering.")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiNumberOfPass NumberOfPass { get; set; } = StiNumberOfPass.SinglePass;

        /// <summary>
        /// Gets or sets the method of calculation in report.
        /// </summary>
        [StiSerializable]
        [StiCategory("Engine")]
        [Category("Engine")]
        [DefaultValue(StiCalculationMode.Compilation)]
        [StiOrder(StiPropertyOrder.ReportMainCalculationMode)]
        [Description("Gets or sets the method of calculation in report.")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor("Stimulsoft.Report.Design.StiCalculationModeEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Professional)]
        public StiCalculationMode CalculationMode { get; set; } = StiCalculationMode.Compilation;

        private StiReportUnitType reportUnit = StiReportUnitType.Centimeters;
        /// <summary>
        /// Gets the current measurement unit of the report.
        /// </summary>
        [Browsable(false)]
        [StiSerializable()]
        [StiCategory("Engine")]
        [Category("Engine")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets the current measurement unit of the report.")]
        [StiOrder(StiPropertyOrder.ReportMainReportUnit)]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiReportUnitType ReportUnit
        {
            get
            {
                return reportUnit;
            }
            set
            {
                if (!IsSerializing && reportUnit != value)
                    this.Convert(StiUnit.GetUnitFromReportUnit(reportUnit), StiUnit.GetUnitFromReportUnit(value));

                reportUnit = value;
            }
        }

        /// <summary>
        /// Gets or sets value, which indicates whether it is necessary to cache all data of the report in one DataSet or not.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [DefaultValue(false)]
        [StiCategory("Data")]
        [Category("Data")]
        [Description("Gets or sets value, which indicates whether it is necessary to cache all data of the report in one DataSet or not.")]
        [StiOrder(StiPropertyOrder.ReportMainCacheAllData)]
        [StiPropertyLevel(StiLevel.Professional)]
        public bool CacheAllData { get; set; }

        /// <summary>
        /// Gets or sets value, which indicates how report engine retrieve data - only used in the report data or all data.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [DefaultValue(false)]
        [StiCategory("Data")]
        [Category("Data")]
        [Description("Gets or sets value, which indicates how report engine retrieve data - only used in the report data or all data.")]
        [StiOrder(StiPropertyOrder.ReportMainRetrieveOnlyUsedData)]
        [StiPropertyLevel(StiLevel.Professional)]
        public bool RetrieveOnlyUsedData { get; set; }

        /// <summary>
        /// Gets or sets value, which indicates how report engine use report cache.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [DefaultValue(StiReportCacheMode.Off)]
        [StiCategory("Engine")]
        [Category("Engine")]
        [Description("Gets or sets value, which indicates how report engine use report cache.")]
        [StiOrder(StiPropertyOrder.ReportMainReportCacheMode)]
        [StiPropertyLevel(StiLevel.Professional)]
        public StiReportCacheMode ReportCacheMode { get; set; } = StiReportCacheMode.Off;

        /// <summary>
        /// Gets or sets value which shows whether it is necessary to convert null or DBNull.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [DefaultValue(true)]
        [StiCategory("Data")]
        [Category("Data")]
        [StiOrder(StiPropertyOrder.ReportMainConvertNulls)]
        [Description("Gets or sets value which shows whether it is necessary to convert null or DBNull.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public bool ConvertNulls { get; set; } = true;

        /// <summary>
        /// Property 'StoreImagesInResources' is obsolete.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [Description("Property 'StoreImagesInResources' is obsolete.")]
        [Obsolete("Property 'StoreImagesInResources' is obsolete.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool StoreImagesInResources { get; set; } = true;

        /// <summary>
        /// Gets or sets the preview mode of the report.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiPreviewMode.Standard)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("View")]
        [Category("View")]
        [Description("Gets or sets the preview mode of the report.")]
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [StiOrder(StiPropertyOrder.ReportMainPreviewMode)]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiPreviewMode PreviewMode { get; set; } = StiPreviewMode.Standard;

        /// <summary>
        /// Specifies the HTML mode which is used for viewing report in the webviewer.
        /// </summary>		
        [StiSerializable]
        [DefaultValue(StiHtmlPreviewMode.Div)]
        [Description("Specifies the HTML mode which is used for viewing report in the webviewer.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [StiBrowsable(false)]
        public StiHtmlPreviewMode HtmlPreviewMode { get; set; } = StiHtmlPreviewMode.Div;

        private int stopBeforePage;
        /// <summary>
        /// Gets or sets a page number. When this page is reached then the report rendering is stopped.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(0)]
        [StiSerializable]
        [StiCategory("Engine")]
        [Category("Engine")]
        [Description("Gets or sets a page number. When this page is reached then the report rendering is stopped.")]
        [StiOrder(StiPropertyOrder.ReportMainStopBeforePage)]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual int StopBeforePage
        {
            get
            {
                return stopBeforePage;
            }
            set
            {
                if (value >= 0)
                    stopBeforePage = value;
            }
        }

        private int stopBeforeTime;
        /// <summary>
        /// Gets or sets a time in seconds. When this time is reached then the report rendering is stopped.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DefaultValue(0)]
        [StiSerializable]
        public virtual int StopBeforeTime
        {
            get
            {
                return stopBeforeTime;
            }
            set
            {
                if (value >= 0)
                    stopBeforeTime = value;
            }
        }

        /// <summary>
        /// Gets or sets controls which will be shown in the Viewer Window.
        /// </summary>		
        [StiSerializable]
        [DefaultValue((int)StiPreviewSettings.Default)]
        [StiCategory("View")]
        [Category("View")]
        [Description("Gets or sets controls which will be shown in the Viewer Window.")]
        [TypeConverter(typeof(Stimulsoft.Report.Viewer.Design.StiPreviewSettingsConverter))]
        [Editor("Stimulsoft.Report.Viewer.Design.StiPreviewSettingsEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [StiOrder(StiPropertyOrder.ReportMainPreviewSettings)]
        [StiPropertyLevel(StiLevel.Standard)]
        public int PreviewSettings { get; set; } = (int)StiPreviewSettings.Default;
                
        /// <summary>
        /// Gets or sets controls which will be shown in the Dashboard Viewer.
        /// </summary>		
        [StiSerializable]
        [DefaultValue(StiDashboardViewerSettings.All)]
        [Description("Gets or sets controls which will be shown in the Dashboard Viewer.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [StiBrowsable(false)]
        public StiDashboardViewerSettings DashboardViewerSettings { get; set; } = StiDashboardViewerSettings.All;

        /// <summary>
        /// Gets or sets options of the preview toolbar of the current report.
        /// </summary>		
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiCategory("View")]
        [Category("View")]
        [Description("Gets or sets options of the preview toolbar of the current report.")]
        [Browsable(false)]
        [StiBrowsable(false)]
        [TypeConverter(typeof(StiPrinterSettingsConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiPreviewToolBarOptions PreviewToolBarOptions { get; set; } = new StiPreviewToolBarOptions();

        private bool ShouldSerializePreviewToolBarOptions()
        {
            return PreviewToolBarOptions == null || !PreviewToolBarOptions.IsDefault;
        }

        private int collate = 1;
        /// <summary>
        /// Gets or sets value which can be used for pages collating. The value of the property cannot be less then 1.
        /// </summary>		
        [StiSerializable]
        [DefaultValue(1)]
        [StiCategory("Engine")]
        [Category("Engine")]
        [Description("Gets or sets value which can be used for pages collating. The value of the property cannot be less then 1.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [StiOrder(StiPropertyOrder.ReportMainCollate)]
        [StiPropertyLevel(StiLevel.Professional)]
        public int Collate
        {
            get
            {
                return collate;
            }
            set
            {
                if (collate != value)
                {
                    collate = Math.Max(1, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets printer settings of current report.
        /// </summary>		
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiCategory("View")]
        [Category("View")]
        [Description("Gets or sets printer settings of current report.")]
        [TypeConverter(typeof(StiPrinterSettingsConverter))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [StiOrder(StiPropertyOrder.ReportMainPrinterSettings)]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiPrinterSettings PrinterSettings { get; set; } = new StiPrinterSettings();

        private bool ShouldSerializePrinterSettings()
        {
            return PrinterSettings == null || !PrinterSettings.IsDefault;
        }

        /// <summary>
        /// Gets or sets an array of referenced assemblies.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        [StiCategory("Engine")]
        [Category("Engine")]
        [Description("Gets or sets an array of referenced assemblies.")]
        [StiOrder(StiPropertyOrder.ReportMainReferencedAssemblies)]
        [TypeConverter(typeof(StiReferencedAssembliesConverter))]
        [StiPropertyLevel(StiLevel.Professional)]
        [Editor("Stimulsoft.Report.Components.Design.StiSimpleTextEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public string[] ReferencedAssemblies { get; set; }

        private StiReportLanguageType scriptLanguage;
        /// <summary>
        /// Gets or sets the current script language.
        /// </summary>
        [StiSerializable]
        [StiCategory("Engine")]
        [Category("Engine")]
        [StiOrder(StiPropertyOrder.ReportMainScriptLanguage)]
        [Description("Gets or sets the current script language.")]
        [StiPropertyLevel(StiLevel.Standard)]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public StiReportLanguageType ScriptLanguage
        {
            get
            {
                return scriptLanguage;
            }
            set
            {
                if (scriptLanguage != value)
                {
                    scriptLanguage = value;
                    if (!IsSerializing)
                    {
                        ScriptNew();
                        IsModified = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a date format for datetime parameters in parameters panel.
        /// </summary>
        [StiSerializable]
        [DefaultValue("")]
        [StiBrowsable(false)]
        [Browsable(false)]
        [Category("View")]
        [StiCategory("View")]
        [Description("Gets or sets a date format for datetime parameters in parameters panel.")]
        public string ParametersDateFormat { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets value which indicates parameters panel orientation.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [DefaultValue(StiOrientation.Horizontal)]
        [Category("View")]
        [StiCategory("View")]
        [StiOrder(StiPropertyOrder.ReportMainParametersPanelOrientation)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates parameters panel orientation.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiOrientation ParametersOrientation { get; set; } = StiOrientation.Horizontal;

        /// <summary>
        /// Gets or sets value which indicates whether to request parameters from a user before report rendering or render a report with the default value of parameters.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [DefaultValue(false)]
        [Category("View")]
        [StiCategory("View")]
        [StiOrder(StiPropertyOrder.ReportMainRequestParameters)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates whether to request parameters from a user before report rendering or render a report with the default value of parameters.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public bool RequestParameters { get; set; }

        private int parameterWidth = 0;
        /// <summary>
        /// Gets or sets a width in pixels of a parameter in the viewer. The default value is used if a zero value is specified.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0)]
        [StiCategory("View")]
        [Category("View")]
        [StiBrowsable(true)]
        [StiPropertyLevel(StiLevel.Professional)]
        [StiOrder(StiPropertyOrder.ReportMainParameterWidth)]
        [Description("Gets or sets a width in pixels of a parameter in the viewer. The default value is used if a zero value is specified.")]
        public int ParameterWidth
        {
            get
            {
                return parameterWidth;
            }
            set
            {
                parameterWidth = value > 0 ? value : 0;
            }
        }

        /// <summary>
        /// Gets or sets value, which allows to save report in resources. 
        /// Property can be used when StiReport component placed on Form or WebForm.
        /// </summary>
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [DefaultValue(true)]
        [StiCategory("Main")]
        [Category("Main")]
        [StiBrowsable(false)]
        [Description("Gets or sets value, which allows to save report in resources.")]
        public bool SaveReportInResources { get; set; } = true;

        /// <summary>
        /// Returns information about the report.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Editor("Stimulsoft.Report.Design.StiAboutEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(StiAboutConverter))]
        [StiCategory("Main")]
        [Category("Main")]
        [StiBrowsable(false)]
        [Description("Returns information about the report.")]
        public object About => null;

        /// <summary>
        /// Gets or sets value, which allows to cache totals
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Engine")]
        [Category("Engine")]
        [Description("Gets or sets value, which allows to cache totals.")]
        [StiOrder(StiPropertyOrder.ReportMainCacheTotals)]
        [StiPropertyLevel(StiLevel.Professional)]
        public bool CacheTotals { get; set; }

        private int refreshTime = (int)StiRefreshTimeValues.None;
        /// <summary>
        /// Gets or sets the time (in seconds) after which the report will be automatically rebuilt in the viewer.
        /// </summary>
        [StiSerializable]
        [DefaultValue((int)StiRefreshTimeValues.None)]
        [StiCategory("View")]
        [Category("View")]
        [StiBrowsable(true)]
        [StiPropertyLevel(StiLevel.Professional)]
        [StiOrder(StiPropertyOrder.ReportMainRefreshTime)]
        [Description("Gets or sets the time (in seconds) after which the report will be automatically rebuilt in the viewer.")]
        [Editor("Stimulsoft.Report.Design.StiRefreshTimeEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public int RefreshTime
        {
            get
            {
                return refreshTime;
            }
            set
            {
                refreshTime = value > 0 ? value : 0;
            }
        }
        #endregion

        #region Properties.Static
        /// <summary>
        /// Returns the default language of the report. StiReport.DefaultReportLanguage property is obsolete. 
        /// Please use StiOptions.Engine.DefaultReportLanguage property.
        /// </summary>
        [Obsolete("StiReport.DefaultReportLanguage property is obsolete. Please use StiOptions.Engine.DefaultReportLanguage property instead it.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static StiReportLanguageType DefaultReportLanguage
        {
            get
            {
                return StiOptions.Engine.DefaultReportLanguage;
            }
            set
            {
                StiOptions.Engine.DefaultReportLanguage = value;
            }
        }

        /// <summary>
        /// Returns the default report type. Default report type will be used in report designer to specify type of created report. 
        /// StiReport.DefaultReportLanguage property is obsolete. StiReport.ReportType property is obsolete. 
        /// Please use StiOptions.Engine.BaseReportType property instead it.
        /// </summary>
        [Obsolete("StiReport.ReportType property is obsolete. Please use StiOptions.Engine.BaseReportType property instead it.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Type ReportType
        {
            get
            {
                return StiOptions.Engine.BaseReportType;
            }
            set
            {
                StiOptions.Engine.BaseReportType = value;
            }
        }

        /// <summary>
        /// Gets or sets the value, which does not allow to show the messages from the engine of messages. 
        /// StiReport.HideMessages property is obsolete. Please use StiOptions.Engine.HideMessages property instead it.
        /// </summary>
        [Obsolete("StiReport.HideMessages property is obsolete. Please use StiOptions.Engine.HideMessages property instead it.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool HideMessages
        {
            get
            {
                return StiOptions.Engine.HideMessages;
            }
            set
            {
                StiOptions.Engine.HideMessages = value;
            }
        }

        /// <summary>
        /// Gets or sets the value, which does not allow to show the exceptions from the engine of exceptions.
        /// StiReport.HideExceptions property is obsolete. Please use StiOptions.Engine.HideExceptions property instead it.
        /// </summary>
        [Obsolete("StiReport.HideExceptions property is obsolete. Please use StiOptions.Engine.HideExceptions property instead it.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool HideExceptions
        {
            get
            {
                return StiOptions.Engine.HideExceptions;
            }
            set
            {
                StiOptions.Engine.HideExceptions = value;
            }
        }

        /// <summary>
        /// Graphics used for report rendering measurement. Internal use only.
        /// </summary>
        [Obsolete("Please use GlobalMeasureGraphics property instead MeasureGraphics property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Graphics MeasureGraphics
        {
            get
            {
                return GlobalMeasureGraphics;
            }
        }

        private static object lockGlobalMeasureGraphics = new object();

        private static Graphics globalMeasureGraphics;
        /// <summary>
        /// Graphics used for report rendering measurement. Internal use only.
        /// </summary>
        public static Graphics GlobalMeasureGraphics
        {
            get
            {
                lock (lockGlobalMeasureGraphics)
                {
                    if (globalMeasureGraphics == null)
                    {
                        if (StiOptions.Engine.OldWYSIWYG)
                        {
                            globalMeasureGraphics = Graphics.FromImage(new Bitmap(1, 1));
                            globalMeasureGraphics.PageUnit = GraphicsUnit.Inch;
                            globalMeasureGraphics.PageScale = .01f;
                        }
                        else
                        {
                            globalMeasureGraphics = Graphics.FromImage(new Bitmap(1, 1));
                            globalMeasureGraphics.PageUnit = GraphicsUnit.Pixel;
                            globalMeasureGraphics.PageScale = 1f;
                        }
                    }
                    return globalMeasureGraphics;
                }
            }
        }

        private static object objLockGlobalRichTextMeasureGraphics = new object();
        private static Graphics globalRichTextMeasureGraphics;

        /// <summary>
        /// Property used for RichText rendering measurement. Internal use only.
        /// </summary>
        public static Graphics GlobalRichTextMeasureGraphics
        {
            get
            {
                if (globalRichTextMeasureGraphics == null)
                {
                    #region Create graphics
                    lock (objLockGlobalRichTextMeasureGraphics)
                    {
                        if (globalRichTextMeasureGraphics != null)
                            return globalRichTextMeasureGraphics;

                        try
                        {
                            var InstalledPrinters = global::System.Drawing.Printing.PrinterSettings.InstalledPrinters;
                            if (StiOptions.Engine.CheckInstalledPrintersForRichTextMeasureGraphics && InstalledPrinters.Count > 0)
                            {
                                using (var pd = new PrintDocument())
                                {
                                    globalRichTextMeasureGraphics = pd.PrinterSettings.CreateMeasurementGraphics();
                                    long ticks1 = Environment.TickCount;
                                    foreach (string printerName in InstalledPrinters)
                                    {
                                        pd.PrinterSettings.PrinterName = printerName;
                                        var gr = pd.PrinterSettings.CreateMeasurementGraphics();

                                        if (gr.DpiX > globalRichTextMeasureGraphics.DpiX)
                                            globalRichTextMeasureGraphics = gr;

                                        //possible slow network printers
                                        if (Environment.TickCount - ticks1 > 1000) break;
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }

                        if (globalRichTextMeasureGraphics == null)
                        {
                            globalRichTextMeasureGraphics = Graphics.FromImage(new Bitmap(1, 1));
                            globalRichTextMeasureGraphics.PageUnit = GraphicsUnit.Pixel;
                            globalRichTextMeasureGraphics.PageScale = 1f;
                        }
                    }
                    #endregion
                }
                return globalRichTextMeasureGraphics;
            }
            set
            {
                if (globalRichTextMeasureGraphics != value)
                    globalRichTextMeasureGraphics = value;
            }
        }
        #endregion

        /// <summary>
        /// Creates a new object of the StiReport type.
        /// </summary>
        public StiReport()
        {
            try
            {
                Key = StiKeyHelper.GenerateKey();
                SubReports = new StiReportsCollection(this);
                engineVersion = StiOptions.Engine.DefaultEngineVersion;

                ReportGuid = GenerateReportGuid();
                ReportDataSources = new StiReportDataSourceCollection(this);
                Info = new StiDesignerInfo(this);

                #region UnitFormat
                try
                {
                    if (StiOptions.Engine.DefaultUnit is StiReportUnitType)
                        this.ReportUnit = (StiReportUnitType)StiOptions.Engine.DefaultUnit;
                    else
                        this.ReportUnit = RegionInfo.CurrentRegion.IsMetric ? StiReportUnitType.Centimeters : StiReportUnitType.Inches;
                }
                catch
                {
                }
                #endregion

                Dictionary = new StiDictionary(this);
                Styles = new StiStylesCollection(this);
                GlobalizationStrings = new StiGlobalizationContainerCollection(this);
                
                try
                {
                    Pages = new StiPagesCollection(this);
                    renderedPages = new StiPagesCollection(this);
                }
                catch
                {
                }

                #region StiReferencedAssembliesService
                ReferencedAssemblies = StiOptions.Engine.ReferencedAssemblies ?? new[]
                {
                    "System.Dll",
                    "System.Drawing.Dll",
                    "System.Windows.Forms.Dll",
                    "System.Data.Dll",
                    "System.Xml.Dll",
                    "Stimulsoft.Controls.Dll",
                    "Stimulsoft.Base.Dll",
                    "Stimulsoft.Data.Dll",
                    "Stimulsoft.Report.Dll"
                };
                #endregion

                var page = StiActivator.CreateObject("Stimulsoft.Report.Components.StiPage", new object[] { this }) as StiPage;

                try
                {
                    if (!RegionInfo.CurrentRegion.IsMetric)
                    {
                        page.PaperSize = PaperKind.Letter;
                        page.PageWidth = this.Unit.ConvertFromHInches(850d);
                        page.PageHeight = this.Unit.ConvertFromHInches(1100d);
                    }
                }
                catch
                {
                }

                Pages.Add(page);

                page.Name = StiNameCreation.GenerateName(page) + "1";

                RenderedPages.Add(page);

                #region Init Name & Alias
                try
                {
                    StiSettings.Load();
                    var isLocalized = StiSettings.GetBool("StiDesigner", "GenerateLocalizedName");

                    if (StiLocalization.CultureName == "en")
                        isLocalized = false;

                    else if (StiOptions.Engine.ForceGenerationNonLocalizedName)
                        isLocalized = false;

                    if (StiOptions.Engine.ForceGenerationLocalizedName)
                        isLocalized = true;

                    if (isLocalized)
                    {
                        PropName = StiLocalization.Get("Components", "StiReport");
                        ReportAlias = StiLocalization.Get("Components", "StiReport");
                    }
                    else
                    {
                        PropName = "Report";
                        ReportAlias = "Report";
                    }
                }
                catch
                {
                    PropName = "Report";
                    ReportAlias = "Report";
                }
                #endregion

                EngineV1 = new StiEngineV1(this);
                pointerValue = StiBookmarksV1Helper.CreateBookmark(this.ReportAlias, this);
                bookmarkValue = StiBookmarksV1Helper.CreateBookmark(this.ReportAlias, this);
                manualBookmark = StiBookmarksV1Helper.CreateBookmark(this.ReportAlias, this);
                scriptLanguage = StiOptions.Engine.DefaultReportLanguage;

                NeedsCompiling = true;
            }
            catch
            {
            }
        }

        static StiReport()
        {
            if (!StiOptions.Configuration.IsWeb)
                StiOptions.Localization.TryLoadDefault();

            StiFunctionsXmlParser.TryLoadDefaultFunctions();

            StiDpiHelper.CheckWysiwygScaling();
        }
    }
}