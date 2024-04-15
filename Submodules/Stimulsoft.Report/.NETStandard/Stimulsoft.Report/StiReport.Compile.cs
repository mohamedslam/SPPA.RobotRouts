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
using System.IO;
using System.Globalization;
using System.Collections;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Reflection;
using System.Text;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Data.Engine;
using System.Threading.Tasks;
using System.Drawing;

#if NETSTANDARD || NETCOREAPP
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report
{
    public partial class StiReport
    {
        #region Properties
        /// <summary>
        /// Gets or sets a value which indicates whether it is necessary to compile a report or it has just been compiled.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool NeedsCompiling { get; set; }

        /// <summary>
        /// Gets or sets compiler results.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CompilerResults CompilerResults { get; set; }

        /// <summary>
        /// Gets or sets a compiled report.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StiReport CompiledReport { get; set; }
        #endregion

        #region Methods
        internal bool CheckNeedsCompiling()
        {
#if NETSTANDARD || NETCOREAPP
            if (!StiOptions.Engine.ForceInterpretationMode && CheckNeedForceInterpretationMode()) StiOptions.Engine.ForceInterpretationMode = true;
#endif
            if (!StiOptions.Engine.FullTrust || StiOptions.Engine.ForceInterpretationMode || CalculationMode == StiCalculationMode.Interpretation)
                return false;

            return NeedsCompiling;
        }

        public StiReport CreateInstance(string path)
        {
            var compiledInMemory = string.IsNullOrEmpty(path);

            if (!compiledInMemory)
            {
                CompiledReport = GetReportFromAssembly(path);
            }
            else
            {
                CompiledReport = GetReportFromAssembly(CompilerResults.CompiledAssembly);
                CompiledReport.ReportVersion = this.ReportVersion;
                CompiledReport.ReportName = this.ReportName;
                CompiledReport.ReportAlias = this.ReportAlias;
                CompiledReport.ReportDescription = this.ReportDescription;
            }

            CompiledReport.Dictionary.DataStore.RegData(this.Dictionary.DataStore);
            CompiledReport.RegBusinessObject(this.BusinessObjectsStore);
            CompiledReport.GlobalizationManager = this.GlobalizationManager;
            CompiledReport.ReportFile = this.ReportFile;
            CompiledReport.IsWpf = this.IsWpf;
            CompiledReport.CookieContainer = this.CookieContainer;

            if (compiledInMemory)
                StiReportResourcesLoader.LoadReportResourcesFromReport(this, CompiledReport);
            else
                StiReportResourcesLoader.LoadReportResourcesFromAssembly(CompiledReport);

            return this;
        }

        /// <summary>
        /// Compiles a report.
        /// </summary>
        /// <param name="outputType">A parameter which sets a type of the output object.</param>
        public async Task<StiReport> CompileAsync(StiOutputType outputType)
        {
            return await Task.Run(() => Compile(outputType));
        }

        /// <summary>
        /// Compiles a report.
        /// </summary>
        /// <param name="outputType">A parameter which sets a type of the output object.</param>
        public StiReport Compile(StiOutputType outputType)
        {
            return Compile("", outputType);
        }

        /// <summary>
        /// Compiles a report.
        /// </summary>
        /// <param name="path">A path for the report location.</param>
        public async Task<StiReport> CompileAsync(string path)
        {
            return await Task.Run(() => Compile(path));
        }

        /// <summary>
        /// Compiles a report.
        /// </summary>
        /// <param name="path">A path for the report location.</param>
        public StiReport Compile(string path)
        {
            return Compile(path, StiOutputType.ClassLibrary);
        }

        /// <summary>
        /// Compiles a report.
        /// </summary>
        public async Task<StiReport> CompileAsync()
        {
            return await Task.Run(() => Compile());
        }

        /// <summary>
        /// Compiles a report.
        /// </summary>
        public StiReport Compile()
        {
            return Compile(StiOutputType.ClassLibrary);
        }

        /// <summary>
        /// Compiles a report and locates it to the specified path.
        /// </summary>
        /// <param name="path">A path for the report location.</param>
        /// <param name="outputType">A parameter which sets the type of the output object.</param>
        public async Task<StiReport> CompileAsync(string path, StiOutputType outputType)
        {
            return await Task.Run(() => Compile(path, outputType));
        }

        /// <summary>
        /// Compiles a report and locates it to the specified path.
        /// </summary>
        /// <param name="path">A path for the report location.</param>
        /// <param name="outputType">A parameter which sets the type of the output object.</param>
        public StiReport Compile(string path, StiOutputType outputType)
        {
            return Compile(path, outputType, true);
        }

        /// <summary>
        /// Compiles a report and locates it in the specified stream.
        /// </summary>
        /// <param name="stream">A stream for the report location.</param>
        public async Task<StiReport> CompileAsync(Stream stream)
        {
            return await Task.Run(() => Compile(stream));
        }

        /// <summary>
        /// Compiles a report and locates it in the specified stream.
        /// </summary>
        /// <param name="stream">A stream for the report location.</param>
        public StiReport Compile(Stream stream)
        {
            Compile(null, stream, StiOutputType.ClassLibrary, true);

            return this;
        }

        /// <summary>
        /// Compiles a report and locates it in the specified path.
        /// </summary>
        /// <param name="path">A parameter which sets the location of the compiled report.</param>
        /// <param name="outputType">A parameter which sets the type of the output object.</param>
        /// <param name="autoCreate">If true then the compiled report will be created.</param>
        public async Task<StiReport> CompileAsync(string path, StiOutputType outputType, bool autoCreate)
        {
            return await Task.Run(() => Compile(path, outputType, autoCreate));
        }

        /// <summary>
        /// Compiles a report and locates it in the specified path.
        /// </summary>
        /// <param name="path">A parameter which sets the location of the compiled report.</param>
        /// <param name="outputType">A parameter which sets the type of the output object.</param>
        /// <param name="autoCreate">If true then the compiled report will be created.</param>
        public StiReport Compile(string path, StiOutputType outputType, bool autoCreate)
        {
            Compile(path, null, outputType, autoCreate);

            return this;
        }

        private string[] GetReferencedAssemblies()
        {
            var needRunningAssembly = false;
            var asms = new List<string>();

            #region Check datasources
            foreach (StiDataSource dataSource in Dictionary.DataSources)
            {
                #region StiOptions.Dictionary.BusinessObjects.AddBusinessObjectAssemblyToReferencedAssembliesAutomatically
                if (StiOptions.Dictionary.BusinessObjects.AddBusinessObjectAssemblyToReferencedAssembliesAutomatically)
                {
                    var dataStore = dataSource as StiDataStoreSource;
                    if (dataSource != null && !needRunningAssembly)
                    {
                        foreach (StiData data in this.DataStore)
                        {
                            if (!data.IsReportData && data.Name == dataStore.NameInSource && data.IsBusinessObjectData)
                            {
                                if (data.ViewData != null)
                                {
                                    var a = Assembly.GetEntryAssembly();
                                    if (a != null)
                                    {
                                        var path = Path.GetFileName(a.Location).ToLowerInvariant();
                                        if (ReferencedAssemblies.All(str => str.ToLowerInvariant() != path))
                                            asms.Add(a.Location);

                                        needRunningAssembly = true;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion

            #region Find assembly location
            if (ReferencedAssemblies != null)
            {
                foreach (var referencedAssembly in ReferencedAssemblies)
                {
                    if (referencedAssembly == null || referencedAssembly.Trim().Length == 0) continue;

                    #region Check for postgre, firebird, VistaDb, SqlServerCe MySql, Oracle, DB2 and etc.
                    var refAssemblyStr = referencedAssembly.Trim().ToLowerInvariant();
                    #endregion

                    if (refAssemblyStr == "stimulsoft.controls.dll") continue;
                    if (refAssemblyStr == "stimulsoft.base.dll") continue;
                    if (refAssemblyStr == "stimulsoft.data.dll") continue;
                    if (refAssemblyStr == "stimulsoft.report.dll") continue;

                    #region Remove reference to old dlls
                    if (refAssemblyStr == "stimulsoft.report.design.dll") continue;
                    if (refAssemblyStr == "stimulsoft.report.odbcdatabase.dll") continue;
                    if (refAssemblyStr == "stimulsoft.report.oracledatabase.dll") continue;
                    #endregion

                    var assembly = StiAssemblyFinder.GetAssembly(referencedAssembly);
                    if (assembly == null)
                        throw new Exception($"Referenced assembly '{referencedAssembly}' not found");

                    asms.Add(assembly.Location);
                }
            }

            asms.Add(typeof(StiActivator).Assembly.Location);
            asms.Add(typeof(StiReport).Assembly.Location);
            asms.Add(typeof(StiDataSortDirection).Assembly.Location);

            if (this.GetType().Name != "StiReport")
                asms.Add(this.GetType().Assembly.Location);
            #endregion

            #region Remove Duplicates
            var asmHash = new Hashtable();
            foreach (var asm in asms)
            {
                asmHash[asm.ToLower(CultureInfo.InvariantCulture)] = asm;
            }
            #endregion

            #region Check Functions
            var functionAssemblies = StiFunctions.GetAssebliesOfFunctions();
            foreach (var functionAssembly in functionAssemblies)
            {
                if (asmHash[functionAssembly.ToLower(CultureInfo.InvariantCulture)] == null)
                    asmHash[functionAssembly.ToLower(CultureInfo.InvariantCulture)] = functionAssembly;
            }
            #endregion

            #region Charts
            try
            {
                var stimulsoftChartsAssembly = typeof(Stimulsoft.Report.Chart.StiChartOptions).Assembly.Location;
                asmHash[stimulsoftChartsAssembly] = stimulsoftChartsAssembly;
            }
            catch
            {
            }
            #endregion

            #region DBS-Dashboards
            try
            {
                if (StiDashboardAssembly.IsAssemblyLoaded)
                    asmHash[StiDashboardAssembly.Assembly.Location] = StiDashboardAssembly.Assembly.Location;
            }
            catch
            {
            }
            #endregion

            var refAsm = new string[asmHash.Keys.Count];
            asmHash.Values.CopyTo(refAsm, 0);

            return refAsm;
        }

        /// <summary>
        /// Compiles a report and locates it in the specified path.
        /// </summary>
        /// <param name="path">Path for the report location.</param>
        /// <param name="stream">A stream for the report location.</param>
        /// <param name="outputType">Type of output object.</param>
        /// <param name="autoCreate">If true then the compiled report will be created.</param>
        private void Compile(string path, Stream stream, StiOutputType outputType, bool autoCreate)
        {
            Compile(path, stream, outputType, autoCreate, null);
        }

        /// <summary>
        /// Compiles a standalone report and locates it in the specified path.
        /// </summary>
        /// <param name="path">Path for the report location.</param>
        public async Task<StiReport> CompileStandaloneReportAsync(string path, StiStandaloneReportType standaloneReportType)
        {
            return await Task.Run(() => CompileStandaloneReport(path, standaloneReportType));
        }

        /// <summary>
        /// Compiles a standalone report and locates it in the specified path.
        /// </summary>
        /// <param name="path">Path for the report location.</param>
        public StiReport CompileStandaloneReport(string path, StiStandaloneReportType standaloneReportType)
        {
            return Compile(path, null, StiOutputType.WindowsApplication, false, standaloneReportType);
        }

#if !NETSTANDARD && !NETCOREAPP
        /// <summary>
        /// Compiles a report and locates it in the specified path.
        /// </summary>
        /// <param name="path">Path for the report location.</param>
        /// <param name="stream">A stream for the report location.</param>
        /// <param name="outputType">Type of output object.</param>
        /// <param name="autoCreate">If true then the compiled report will be created.</param>
        private StiReport Compile(string path, Stream stream, StiOutputType outputType, bool autoCreate, object standaloneReportType)
        {
            if (!StiOptions.Engine.FullTrust)
                return this;

            StiOptions.Engine.GlobalEvents.InvokeReportCompiling(this, EventArgs.Empty);

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
                File.Delete(path);

            if (!this.NeedsCompiling)
                throw new Exception("Report already compiled");

            #region Compile to stream
            bool flagPathIsEmpty = string.IsNullOrEmpty(path);
            if (stream != null && flagPathIsEmpty)
            {
                var temp = Environment.GetEnvironmentVariable("Temp");
                if (temp == null)
                    throw new Exception("Can't get Temp directory");

                path = $"{temp}\\{Guid.NewGuid().ToString().Replace("-", "")}.dll";
            }
            #endregion

            if (StiOptions.Engine.AllowSetCurrentDirectory)
                Directory.SetCurrentDirectory(StiOptions.Configuration.ApplicationDirectory);

            StiLogService.Write(this.GetType(), "Compiling report");

            var refAsm = GetReferencedAssemblies();

            ProcessAutoLocalizeReportOnRun();

            #region RemoveUnusedDataBeforeReportRendering
            if (StiOptions.Dictionary.RemoveUnusedDataBeforeReportRendering)
                this.Dictionary.RemoveUnusedData();
            #endregion

            var resScript = this.Script;

            try
            {
                var inMemory = string.IsNullOrWhiteSpace(path);
                using (var resources = new StiReportResourcesCompiler(this, inMemory))
                {
                    ScriptUpdate(standaloneReportType, false);

                    #region LanguageType
                    var languageType = StiCompiler.LanguageType.CSharp;
                    if (this.ScriptLanguage == StiReportLanguageType.VB)
                        languageType = StiCompiler.LanguageType.VB;
                    #endregion

                    #region Sign Assembly
                    var reportScript = this.Script;

                    if (!string.IsNullOrEmpty(StiOptions.Engine.PathToAssemblyKeyFile) && File.Exists(StiOptions.Engine.PathToAssemblyKeyFile))
                    {
                        var assemblyKeyFile = StiOptions.Engine.PathToAssemblyKeyFile;
                        assemblyKeyFile = assemblyKeyFile.Replace("\\", "\\\\");
                        string attributeStr;
                        string namespaceStr;

                        if (this.ScriptLanguage == StiReportLanguageType.CSharp || this.ScriptLanguage == StiReportLanguageType.JS)
                        {
                            namespaceStr = "namespace";
                            attributeStr = $"[assembly: System.Reflection.AssemblyKeyFile(\"{assemblyKeyFile}\")]\n";
                        }
                        else
                        {
                            namespaceStr = "Namespace";
                            attributeStr = $"<Assembly: System.Reflection.AssemblyKeyFile(\"{assemblyKeyFile}\")>\n";
                        }

                        var namespaceIndex = reportScript.IndexOf(namespaceStr, StringComparison.InvariantCulture);
                        reportScript = reportScript.Insert(namespaceIndex, attributeStr);
                    }
                    #endregion

                    CompilerResults = StiCompiler.Compile(reportScript, path, languageType, outputType, refAsm, resources.Files);
                }
            }
            catch (Exception e)
            {
                #region Create Report Compilation Error
                if (CompilerResults == null)
                    CompilerResults = new CompilerResults(null);

                CompilerResults.Errors.Add(new CompilerError
                {
                    Column = -1,
                    Line = -1,
                    IsWarning = false,
                    ErrorText = e.Message
                });
                #endregion

                StiLogService.Write(this.GetType(), "Compiling report...ERROR");
                StiLogService.Write(this.GetType(), e);

                throw;
            }
            finally
            {
                this.Script = resScript;
            }

            #region CompilerResults
            var allowAssemblyCreation = true;
            if (CompilerResults != null && CompilerResults.Errors != null && CompilerResults.Errors.Count > 0 && CompilerResults.Errors.HasErrors)
            {
                var sb = new StringBuilder();
                foreach (CompilerError error in CompilerResults.Errors)
                {
                    sb.Append(error);
                    StiLogService.Write(this.GetType(), error.ToString());
                    if (!error.IsWarning) allowAssemblyCreation = false;
                }
                if (!this.IsDesigning)
                    throw new Exception(sb.ToString());
            }
            #endregion

            #region AssemblyIsCreated
            if (allowAssemblyCreation)
            {
                if (autoCreate)
                    CreateInstance(path);

                if (CompiledReport != null)
                    CompiledReport.Variables = this.Variables;

                if (StiOptions.Engine.AllowResetAssemblyAfterCompilation)
                    CompilerResults.CompiledAssembly = null;

                #region Write dll to stream
                if (stream != null)
                {
                    FileStream dllStream = null;
                    try
                    {
                        StiFileUtils.ProcessReadOnly(path);
                        dllStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                        var bytes = new byte[dllStream.Length];

                        dllStream.Read(bytes, 0, (int)dllStream.Length);
                        stream.Write(bytes, 0, bytes.Length);
                    }
                    finally
                    {
                        if (dllStream != null)
                            dllStream.Close();

                        if (flagPathIsEmpty && File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                }
                #endregion
            }
            #endregion

            IsDocument = false;

            StiOptions.Engine.GlobalEvents.InvokeReportCompiled(this, EventArgs.Empty);

            return this;
        }

#else
        //Compilation for Roslyn
        private StiReport Compile(string path, Stream stream, StiOutputType outputType, bool autoCreate, object standaloneReportType)
        {
            if (!this.NeedsCompiling)
                throw new Exception("Report already compiled");

            if (!string.IsNullOrWhiteSpace(path))
                throw new Exception("Only compilation in memory supported!");

            if (!StiOptions.Engine.ForceInterpretationMode && CheckNeedForceInterpretationMode())
            {
                StiOptions.Engine.ForceInterpretationMode = true;
                return this;
            }

            StiOptions.Engine.GlobalEvents.InvokeReportCompiling(this, EventArgs.Empty);

            if (StiOptions.Engine.AllowSetCurrentDirectory)
                Directory.SetCurrentDirectory(StiOptions.Configuration.ApplicationDirectory);

            StiLogService.Write(this.GetType(), "Compiling report");

            ProcessAutoLocalizeReportOnRun();

            if (StiOptions.Dictionary.RemoveUnusedDataBeforeReportRendering)
                this.Dictionary.RemoveUnusedData();

            var resScript = this.Script;

            try
            {
                bool inMemory = stream == null;
                using (var resources = new StiReportResourcesCompiler(this, inMemory))
                {
                    #region Process resources
                    var resourcesList = new List<ResourceDescription>();
                    if (!inMemory)
                    {
                        foreach (var resource in resources.Resources)
                        {
                            var rd = new ResourceDescription(resource.ResourceName, () => new MemoryStream(resource.Resource), true);
                            resourcesList.Add(rd);
                        }
                    }
                    #endregion

                    ScriptUpdate(standaloneReportType, false);

                    #region Sign Assembly
                    var reportScript = this.Script;

                    if (!string.IsNullOrEmpty(StiOptions.Engine.PathToAssemblyKeyFile) && File.Exists(StiOptions.Engine.PathToAssemblyKeyFile))
                    {
                        var assemblyKeyFile = StiOptions.Engine.PathToAssemblyKeyFile;
                        assemblyKeyFile = assemblyKeyFile.Replace("\\", "\\\\");
                        string attributeStr;
                        string namespaceStr;

                        if (this.ScriptLanguage == StiReportLanguageType.CSharp || this.ScriptLanguage == StiReportLanguageType.JS)
                        {
                            namespaceStr = "namespace";
                            attributeStr = $"[assembly: System.Reflection.AssemblyKeyFile(\"{assemblyKeyFile}\")]\n";
                        }
                        else
                        {
                            namespaceStr = "Namespace";
                            attributeStr = $"<Assembly: System.Reflection.AssemblyKeyFile(\"{assemblyKeyFile}\")>\n";
                        }

                        var namespaceIndex = reportScript.IndexOf(namespaceStr, StringComparison.InvariantCulture);
                        reportScript = reportScript.Insert(namespaceIndex, attributeStr);
                    }
                    #endregion

                    #region Remove unnecessary usings
                    var arrUsing = new[]
                    {
                        "using System.Windows.Forms;",
#if NETSTANDARD
                        "using Stimulsoft.Controls;",
#endif
                    };
                    foreach (string st in arrUsing)
                    {
                        reportScript = reportScript.Replace(st, "");
                    }
                    #endregion

                    CompilerResults = new CompilerResults(null);

#if STIDRAWING
                    var namespaceIndexDrawing = reportScript.IndexOf("namespace", StringComparison.Ordinal);
                    var stiDrawingObjects =
                        "using Image = Stimulsoft.Drawing.Image;\n" +
                        "using Pens = Stimulsoft.Drawing.Pens;\n" +
                        "using Pen = Stimulsoft.Drawing.Pen;\n" +
                        "using Brushes = Stimulsoft.Drawing.Brushes;\n" +
                        "using SolidBrush = Stimulsoft.Drawing.SolidBrush;\n" +
                        "using TextureBrush = Stimulsoft.Drawing.TextureBrush;\n" +
                        "using LinearGradientBrush = Stimulsoft.Drawing.Drawing2D.LinearGradientBrush;\n" +
                        "using HatchBrush = Stimulsoft.Drawing.Drawing2D.HatchBrush;\n" +
                        "using Font = Stimulsoft.Drawing.Font;\n" +
                        "using FontFamily = Stimulsoft.Drawing.FontFamily;\n" +
                        "using StringFormat = Stimulsoft.Drawing.StringFormat;\n";

                    reportScript = reportScript.Insert(namespaceIndexDrawing, stiDrawingObjects + "\n");
#endif

                    if (Stimulsoft.Base.StiCompiler.AssemblyVersion != null)
                    {
                        var namespaceIndex = reportScript.IndexOf("namespace", StringComparison.Ordinal);
                        reportScript = reportScript.Insert(namespaceIndex, "[assembly: System.Reflection.AssemblyVersion(\"" + Stimulsoft.Base.StiCompiler.AssemblyVersion + "\")]\n");
                    }

                    var refFiles = GetReferencedAssembliesRoslyn();
                    var references = new List<PortableExecutableReference>();
                    foreach(var file in refFiles)
                    {
                        lock(hashMetadataReference)
                        {
                            PortableExecutableReference per = hashMetadataReference[file] as PortableExecutableReference;
                            if (per == null)
                            {
                                per = MetadataReference.CreateFromFile(file);
                                hashMetadataReference[file] = per;
                            }
                            references.Add(per);
                        }
                    }

                    Compilation compilation = null;
                    string assemblyName = "a" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 9);
                    if (this.ScriptLanguage == StiReportLanguageType.VB)
                    {
                        SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(reportScript);
                        compilation = VisualBasicCompilation.Create(
                            assemblyName,
                            new[] { syntaxTree },
                            references,
                            new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
                    }
                    else
                    {
                        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(reportScript);
                        compilation = CSharpCompilation.Create(
                            assemblyName,
                            new[] { syntaxTree },
                            references,
                            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
                    }

                    using (var assemblyStream = new MemoryStream())
                    {
                        EmitResult result = compilation.Emit(assemblyStream, manifestResources: resourcesList);

                        if (result.Success)
                        {
                            assemblyStream.Seek(0, SeekOrigin.Begin);
                            var bytes = new byte[assemblyStream.Length];
                            assemblyStream.Read(bytes, 0, (int)assemblyStream.Length);

                            CompilerResults.CompiledAssembly = Assembly.Load(bytes);

                            if (!inMemory) stream.Write(bytes, 0, bytes.Length);
                        }
                        else
                        {
                            #region Prepare Compilation Errors message
                            List<Diagnostic> errors = result.Diagnostics.Where(d =>
                                d.IsWarningAsError && d.Severity > DiagnosticSeverity.Warning ||
                                d.Severity == DiagnosticSeverity.Error).ToList();

                            var sb = new StringBuilder();
                            foreach (var error in errors)
                            {
                                sb.AppendLine(error.GetMessage());
                                StiLogService.Write(this.GetType(), error.GetMessage());

                                if (this.IsDesigning)
                                {
                                    CompilerResults.Errors.Add(new CompilerError
                                    {
                                        Column = -1,
                                        Line = -1,
                                        IsWarning = false,
                                        ErrorText = error.GetMessage()
                                    });
                                }
                            }
                            #endregion

                            if (!this.IsDesigning)
                                throw new Exception("Compilation error: " + sb.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                #region Create Report Compilation Error
                if (CompilerResults == null)
                    CompilerResults = new CompilerResults(null);

                CompilerResults.Errors.Add(new CompilerError
                {
                    Column = -1,
                    Line = -1,
                    IsWarning = false,
                    ErrorText = e.Message
                });
                #endregion

                StiLogService.Write(this.GetType(), "Compiling report...ERROR");
                StiLogService.Write(this.GetType(), e);

                throw;
            }
            finally
            {
                this.Script = resScript;
            }

            #region AssemblyIsCreated
            if (CompilerResults.CompiledAssembly != null)
            {
                if (autoCreate)
                    CreateInstance(path);

                if (CompiledReport != null)
                    CompiledReport.Variables = this.Variables;

                if (StiOptions.Engine.AllowResetAssemblyAfterCompilation)
                    CompilerResults.CompiledAssembly = null;
            }
            #endregion

            IsDocument = false;

            StiOptions.Engine.GlobalEvents.InvokeReportCompiled(this, EventArgs.Empty);

            return this;
        }

        private static Hashtable hashMetadataReference = new Hashtable();

        private string[] GetReferencedAssembliesRoslyn()
        {
            var pathes = new List<string>();

            #region Find ReferencedAssemblies location
            var asms = new List<string>();
            if (ReferencedAssemblies != null)
            {
                foreach (var referencedAssembly in ReferencedAssemblies)
                {
                    if (referencedAssembly == null || referencedAssembly.Trim().Length == 0) continue;

                    var refAssemblyStr = Path.GetFileNameWithoutExtension(referencedAssembly).Trim().ToLowerInvariant();

                    if (refAssemblyStr == "stimulsoft.controls.dll") continue;
                    //if (refAssemblyStr == "stimulsoft.base.dll") continue;
                    //if (refAssemblyStr == "stimulsoft.data.dll") continue;
                    //if (refAssemblyStr == "stimulsoft.report.dll") continue;

                    // Remove reference to old dlls
                    if (refAssemblyStr == "stimulsoft.report.design.dll") continue;
                    if (refAssemblyStr == "stimulsoft.report.odbcdatabase.dll") continue;
                    if (refAssemblyStr == "stimulsoft.report.oracledatabase.dll") continue;

                    asms.Add(refAssemblyStr);
                }
            }

            //add common assemblies
            asms.AddRange(new[]
            {
                "mscorlib",
                "netstandard",
                "System",
                "System.Collections",
                "System.Collections.NonGeneric",
                "System.ComponentModel.Primitives",
                "System.ComponentModel.TypeConverter",
                "System.Data.Common",
                "System.Drawing",
                "System.Drawing.Common",
                "System.Drawing.Primitives",
                "System.Linq",
                "System.Net.Primitives",
                "System.Private.CoreLib",
                "System.Runtime",
                "Microsoft.VisualBasic.Core",
                "Stimulsoft.Base",
                "Stimulsoft.Report",
            });

#if NETSTANDARD
            asms.Add("Stimulsoft.System");
#endif

#if STIDRAWING
            asms.Add("Stimulsoft.Drawing");
#endif

            var neededAssemblies = asms.ToArray();

            string paths = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
            if (!string.IsNullOrWhiteSpace(paths))
            {
                var trustedAssembliesPaths = paths.Split(Path.PathSeparator);
                pathes.AddRange(trustedAssembliesPaths
                    .Where(p => neededAssemblies.Contains(Path.GetFileNameWithoutExtension(p), StringComparer.InvariantCultureIgnoreCase)));
            }
            #endregion

            pathes.Add(typeof(StiActivator).Assembly.Location);
            pathes.Add(typeof(StiReport).Assembly.Location);
            pathes.Add(typeof(StiDataSortDirection).Assembly.Location);

            if (this.GetType().Name != "StiReport")
                pathes.Add(this.GetType().Assembly.Location);

            //get current assembly path
            var a = Assembly.GetEntryAssembly();
            if (a != null) pathes.Add(a.Location);

            bool[] founds = new bool[neededAssemblies.Length];
            CheckAssembliesFromPathesAndAppDomain(founds, pathes, neededAssemblies);

            if (a != null) CheckAssembliesAtLocation(a.Location, founds, pathes, neededAssemblies);
            CheckAssembliesAtLocation(typeof(StiActivator).Assembly.Location, founds, pathes, neededAssemblies);

            pathes.AddRange(StiFunctions.GetAssebliesOfFunctions());

            try
            {
                pathes.Add(typeof(Stimulsoft.Report.Chart.StiChartOptions).Assembly.Location);
            }
            catch { }

            try
            {
                if (StiDashboardAssembly.IsAssemblyLoaded)
                    pathes.Add(StiDashboardAssembly.Assembly.Location);
            }
            catch { }

            #region Remove Duplicates
            var pathesHash = new Hashtable();
            foreach (var path in pathes)
            {
                pathesHash[path.ToLower(CultureInfo.InvariantCulture)] = path;
            }
            #endregion

            var refAsm = new string[pathesHash.Keys.Count];
            pathesHash.Values.CopyTo(refAsm, 0);

            return refAsm;
        }

        private void CheckAssembliesFromPathesAndAppDomain(bool[] founds, List<string> pathes, string[] neededAssemblies)
        {
            for (int index = 0; index < neededAssemblies.Length; index++)
            {
                string name = neededAssemblies[index];
                string name2 = $"\\{name}.dll";
                foreach (var path in pathes)
                {
                    if (path.EndsWith(name2, StringComparison.InvariantCultureIgnoreCase))
                    {
                        founds[index] = true;
                        break;
                    }
                }

                if (!founds[index])
                {
                    try
                    {
                        var asms = AppDomain.CurrentDomain.GetAssemblies();
                        foreach (var assembly in asms)
                        {
                            if (assembly.GetName().Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && (!string.IsNullOrWhiteSpace(assembly.Location)))
                            {
                                pathes.Add(assembly.Location);
                                founds[index] = true;
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        private void CheckAssembliesAtLocation(string location, bool[] founds, List<string> pathes, string[] neededAssemblies)
        {
            try
            {
                DirectoryInfo di = null;
                FileInfo[] fis = null;

                for (int index = 0; index < neededAssemblies.Length; index++)
                {
                    if (!founds[index])
                    {
                        var name = neededAssemblies[index];
                        if (di == null)
                        {
                            di = new DirectoryInfo(Path.GetDirectoryName(location));
                            fis = di.GetFiles();
                        }
                        foreach (var fi in fis)
                        {
                            if (fi.Name.Equals(name + ".dll", StringComparison.InvariantCultureIgnoreCase))
                            {
                                pathes.Add(fi.FullName);
                                founds[index] = true;

                                try
                                {
                                    StiAssemblyFinder.GetAssembly(fi.FullName, false);
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch { }
        }
#endif

        internal void ProcessAutoLocalizeReportOnRun()
        {
            if (!AutoLocalizeReportOnRun) return;

            try
            {
                GlobalizationStrings.SkipException = true;

                var cultureInfo = CultureInfo.CurrentCulture;
                var culture = GetParsedCulture();
                if (!string.IsNullOrWhiteSpace(culture))
                {
                    try
                    {
                        cultureInfo = new CultureInfo(culture, false);
                    }
                    catch
                    {
                    }
                }
                GlobalizationStrings.LocalizeReport(cultureInfo);
            }
            finally
            {
                GlobalizationStrings.SkipException = false;
            }
        }

        public async Task<StiReport> CompileReportsToAssemblyAsync(string assemblyPath, StiReport[] reports)
        {
            await Task.Run(() => CompileReportsToAssembly(assemblyPath, reports));

            return this;
        }

        public CompilerResults CompileReportsToAssembly(string assemblyPath, StiReport[] reports)
        {
            return CompileReportsToAssembly(assemblyPath, reports, StiReportLanguageType.CSharp);
        }

        public async Task<StiReport> CompileReportsToAssemblyAsync(string assemblyPath, StiReport[] reports, StiReportLanguageType languageType)
        {
            await Task.Run(() => CompileReportsToAssembly(assemblyPath, reports, languageType));

            return this;
        }

        public static CompilerResults CompileReportsToAssembly(string assemblyPath, StiReport[] reports, StiReportLanguageType languageType)
        {
            var sources = new string[reports.Length];
            var index = 0;
            var refAsm = new Hashtable();
            var resources = new StiReportResourcesCompiler(null, false);

            foreach (StiReport report in reports)
            {
                StiOptions.Engine.GlobalEvents.InvokeReportCompiling(report, EventArgs.Empty);

                resources.ProcessReport(report);

                report.ScriptUpdate();

                string textToCompile = report.Script;
                if (StiCompiler.AssemblyVersion != null)
                {
                    int namespaceIndex = textToCompile.IndexOf("namespace", StringComparison.Ordinal);
                    textToCompile = textToCompile.Insert(namespaceIndex, $"[assembly: System.Reflection.AssemblyVersion(\"{StiCompiler.AssemblyVersion}\")]\n");
                }
                sources[index++] = textToCompile;

                var asmStrs = report.GetReferencedAssemblies();
                foreach (var str in asmStrs)
                    refAsm[str] = str;

                StiOptions.Engine.GlobalEvents.InvokeReportCompiled(report, EventArgs.Empty);
            }

            var referencedAsms = new string[refAsm.Keys.Count];
            refAsm.Keys.CopyTo(referencedAsms, 0);

            var parms = new CompilerParameters();
            parms.ReferencedAssemblies.AddRange(referencedAsms);
            parms.OutputAssembly = assemblyPath;
            parms.CompilerOptions = parms.CompilerOptions + " /target:library";
            parms.GenerateExecutable = false;
            parms.ReferencedAssemblies.AddRange(referencedAsms);

            if (resources.Files != null)
            {
                foreach (var resource in resources.Files)
                {
                    parms.EmbeddedResources.Add(resource);
                }
            }

            var provider = languageType == StiReportLanguageType.CSharp || languageType == StiReportLanguageType.JS
                ? (CodeDomProvider)new Microsoft.CSharp.CSharpCodeProvider()
                : new Microsoft.VisualBasic.VBCodeProvider();

            var results = provider.CompileAssemblyFromSource(parms, sources);

            provider.Dispose();
            resources.Dispose();

            return results;
        }
        #endregion

        #region CheckNeedForceInterpretationMode
        public static bool CheckNeedForceInterpretationMode()
        {
            bool needForce = false;

            //Check for bug in NetCore2.1 or early
            try
            {
                var res = TypeDescriptor.GetConverter(typeof(DateTime)).ConvertTo(DateTime.Now, typeof(InstanceDescriptor));
#if !(NETSTANDARD && !BLAZOR)
                res = TypeDescriptor.GetConverter(typeof(Font)).ConvertTo(new Font("Arial", 8f), typeof(InstanceDescriptor));
#endif
                res = TypeDescriptor.GetConverter(typeof(StiMargin)).ConvertFrom("0,0,0,0");    //bug if dynamic library loading
            }
            catch
            {
                needForce = true;
            }

            //Check for WebAssembly (try to get DLL file path)
            try
            {
                var asmLocation = typeof(StiReport).Assembly.Location;
                if (string.IsNullOrEmpty(asmLocation))
                    needForce = true;
            }
            catch
            {
                needForce = true;
            }

            return needForce;
        }
        #endregion
    }
}
