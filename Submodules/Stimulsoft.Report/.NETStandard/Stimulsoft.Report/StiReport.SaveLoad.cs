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
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Linq;
using System.Collections;
using System.Globalization;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.SaveLoad;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.CodeDom;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Json;
using Stimulsoft.Report.Viewer;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Base.Drawing;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Stimulsoft.Report
{
    public partial class StiReport
    {
        #region Fields
        internal StiReportJsonLoaderHelper jsonLoaderHelper;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the last opened or saved file name.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [StiSerializable]
        [DefaultValue("")]
        public string ReportFile { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value that allows to save the interaction settings of components in a document (report snapshot) file.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        internal bool SaveInteractionParametersToDocument { get; set; }
        #endregion

        #region Methods.Detect
        /// <summary>
        /// Returns true if specified stream contains packed report.
        /// </summary>
        public static bool IsPackedFile(Stream stream)
        {
            if (stream.Length < 4) return false;

            var first = stream.ReadByte();
            var second = stream.ReadByte();
            var third = stream.ReadByte();
            stream.Seek(-3, SeekOrigin.Current);

            return IsPackedFile(first, second, third);
        }

        /// <summary>
        /// Returns true if specified stream contains packed report.
        /// </summary>
        public static bool IsJsonFile(Stream stream)
        {
            if (stream.Length < 2) return false;

            var first = stream.ReadByte();
            if (first == 0xEF && stream.Length > 3) //maybe BOM
            {
                var second = stream.ReadByte();
                var third = stream.ReadByte();
                first = stream.ReadByte();
                stream.Seek(-3, SeekOrigin.Current);
                if (!(second == 0xBB && third == 0xBF)) first = 0;
            }
            stream.Seek(-1, SeekOrigin.Current);

            return IsJsonFile(first);
        }

        public static bool IsJsonFile(int first)
        {
            return first == 0x0000007b;
        }

        public static bool IsPackedFile(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 3)
                return false;

            return IsPackedFile(bytes[0], bytes[1], bytes[2]);
        }

        public static bool IsPackedFile(int first, int second, int third)
        {
            //Variant checks bytes for zip signature
            if (first == 0x1F && second == 0x8B && third == 0x08) return true;//Gzip
            if (first == 0x50 && second == 0x4B && third == 0x03) return true;//PKZip

            return false;
        }

        /// <summary>
        /// Returns true if specified stream contains encrypted report.
        /// </summary>
        public static bool IsEncryptedFile(Stream stream)
        {
            if (stream.Length < 4) return false;

            var first = stream.ReadByte();
            var second = stream.ReadByte();
            var third = stream.ReadByte();
            stream.Seek(-3, SeekOrigin.Current);

            return IsEncryptedFile(first, second, third);
        }

        public static bool IsEncryptedFile(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 3)
                return false;

            return IsEncryptedFile(bytes[0], bytes[1], bytes[2]);
        }

        public static bool IsEncryptedFile(int first, int second, int third)
        {
            return (first == (byte)'m' && second == (byte)'d' && third == (byte)'x') ||
                (first == (byte)'m' && second == (byte)'r' && third == (byte)'x');
        }
        #endregion

        #region Methods.Load report from resources
        /// <summary>
        /// Loads a report template from the resource.
        /// </summary>
        /// <param name="reportAssembly">Assembly in which is the report is placed.</param>
        /// <param name="reportResource">A name of resources which contains a report template.</param>
        public virtual StiReport LoadReportFromResource(Assembly reportAssembly, string reportResource)
        {
            var stream = reportAssembly.GetManifestResourceStream(reportResource);
            if (stream != null)
            {
                this.Load(stream);
                stream.Close();
                stream.Dispose();
            }
            else
                throw new Exception($"Can't find the report '{reportResource}' in the resources!");

            return this;
        }

        /// <summary>
        /// Loads a report template from the resource.
        /// </summary>
        /// <param name="assemblyName">The name of assembly in which the report is placed.</param>
        /// <param name="reportResource">A name of resources which contains a report template.</param>
        public virtual StiReport LoadReportFromResource(string assemblyName, string reportResource)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == assemblyName);

            if (assembly != null)
            {
                LoadReportFromResource(assembly, reportResource);
                return this;
            }

            throw new Exception($"Can't find the assembly '{assemblyName}'!");
        }
        #endregion

        #region Methods.Save report source code
        /// <summary>
        /// Saves the report source code to the string.
        /// </summary>
        public string SaveReportSourceCode()
        {
            return SaveReportSourceCode(false);
        }

        /// <summary>
        /// Saves the report source code to the string.
        /// </summary>
        public string SaveReportSourceCode(bool saveForInheritedReports)
        {
#if NETSTANDARD || NETCOREAPP
            if (CheckNeedForceInterpretationMode()) return this.Script;
#endif

            if (saveForInheritedReports)
            {
                var serializator = new StiCodeDomSerializator();
                return serializator.Serialize(this, this.GetReportName(), Language, saveForInheritedReports);
            }
            else
            {
                string storedScript = this.Script;
                try
                {
                    this.ScriptUnpack(saveForInheritedReports);
                    return this.Script;
                }
                finally
                {
                    this.Script = storedScript;
                }
            }
        }

        /// <summary>
        /// Saves the report source code to the stream.
        /// </summary>
        /// <param name="stream">Stream for saving the source code.</param>
        public StiReport SaveReportSourceCode(Stream stream)
        {
            SaveReportSourceCode(stream, false);

            return this;
        }

        /// <summary>
        /// Saves the report source code to the stream.
        /// </summary>
        /// <param name="stream">Stream for saving the source code.</param>
        public StiReport SaveReportSourceCode(Stream stream, bool saveForInheritedReports)
        {
            StiLogService.Write(typeof(StiReport), "Saving report to source code");
            StreamWriter writer = null;
            var encoding = Encoding.UTF8;
            try
            {
                if (StiOptions.Engine.SourceCodeEncoding != null)
                    encoding = StiOptions.Engine.SourceCodeEncoding;

                writer = new StreamWriter(stream, encoding);
                writer.Write(SaveReportSourceCode(saveForInheritedReports));
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving report to source code...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            finally
            {
                if (writer != null) writer.Flush();
            }

            return this;
        }

        /// <summary>
        /// Saves the report source code to the file.
        /// </summary>
        /// <param name="path">Parameter specifies a path to the file the report is exported to.</param>
        public virtual StiReport SaveReportSourceCode(string path)
        {
            return SaveReportSourceCode(path, false);
        }

        /// <summary>
        /// Saves the report source code to the file.
        /// </summary>
        /// <param name="path">Parameter specifies a path to the file the report is exported to.</param>
        public virtual StiReport SaveReportSourceCode(string path, bool saveForInheritedReports)
        {
            FileStream stream = null;
            try
            {
                if (!this.IsDesigning || !StiOptions.Designer.ReadOnlyAlertOnSave)
                {
                    StiFileUtils.ProcessReadOnly(path);
                }
                stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                SaveReportSourceCode(stream, saveForInheritedReports);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving report to source code...ERROR");
                StiLogService.Write(this.GetType(), e);

                if ((!StiOptions.Engine.HideExceptions) || this.IsDesigning) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            return this;
        }

        /// <summary>
        /// Saves the report source code for Silverlight to the file.
        /// </summary>
        /// <param name="path">Parameter specifies a path to the file the report is exported to.</param>
        public virtual StiReport SaveReportSourceCodeForSilverlight(string path)
        {
            FileStream stream = null;
            try
            {
                if (!this.IsDesigning || !StiOptions.Designer.ReadOnlyAlertOnSave)
                {
                    StiFileUtils.ProcessReadOnly(path);
                }
                stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                SaveReportSourceCodeForSilverlight(stream);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving report to source code...ERROR");
                StiLogService.Write(this.GetType(), e);

                if ((!StiOptions.Engine.HideExceptions) || this.IsDesigning) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            return this;
        }

        /// <summary>
        /// Saves the report source code for Silverlight to the stream.
        /// </summary>
        /// <param name="stream">Stream for saving the source code.</param>
        public StiReport SaveReportSourceCodeForSilverlight(Stream stream)
        {
            StiLogService.Write(typeof(StiReport), "Saving report to source code for Silverlight");
            StreamWriter writer = null;
            var encoding = Encoding.UTF8;
            try
            {
                if (StiOptions.Engine.SourceCodeEncoding != null)
                    encoding = StiOptions.Engine.SourceCodeEncoding;

                writer = new StreamWriter(stream, encoding);
                writer.Write(SaveReportSourceCodeForSilverlight());
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving report to source code for Silverlight...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            finally
            {
                if (writer != null) writer.Flush();
            }
            return this;
        }

        /// <summary>
        /// Saves the report source code for Silverlight to the string.
        /// </summary>
        public string SaveReportSourceCodeForSilverlight()
        {
            this.ReportResources.Clear();

            var resRelations = this.Dictionary.Relations;
            var resDataSources = this.Dictionary.DataSources;
            var resDatabases = this.Dictionary.Databases;

            try
            {
                this.Dictionary.DataSources = new StiDataSourcesCollection(this.Dictionary);
                this.Dictionary.Relations = new StiDataRelationsCollection(this.Dictionary);
                this.Dictionary.Databases = new StiDatabaseCollection();

                string storedScript = this.Script;
                this.ScriptUnpack();
                string text = this.Script;
                this.Script = storedScript;

                text = text.Replace("System.Drawing.Color.FromArgb", "System.Windows.Media.Color.FromArgb");

                var colorStr = "System.Drawing.Color.";
                while (true)
                {
                    int index = text.IndexOf(colorStr, StringComparison.Ordinal);
                    if (index != -1)
                    {
                        int colorNameIndex = index + colorStr.Length;
                        string colorName = "";
                        while (true)
                        {
                            char chr = text[colorNameIndex];
                            if (char.IsLetter(chr))
                                colorName += chr;
                            else
                                break;
                            colorNameIndex++;
                        }
                        var color = Color.FromName(colorName);
                        var oldColorName = colorStr + colorName;
                        var newColorName = $"System.Windows.Media.Color.FromArgb({color.A}, {color.R}, {color.G}, {color.B})";

                        var oldLen = oldColorName.Length;
                        var newLen = newColorName.Length;
                        var indexStr = 0;
                        while (true)
                        {
                            indexStr = text.IndexOf(oldColorName, indexStr, StringComparison.Ordinal);
                            if (indexStr < 0) break;
                            if (indexStr + oldLen < text.Length && !char.IsLetter(text[indexStr + oldLen]))
                            {
                                text = text.Substring(0, indexStr) + newColorName + text.Substring(indexStr + oldLen);
                                indexStr += newLen;
                            }
                            else
                            {
                                indexStr += oldLen;
                            }
                        }
                    }
                    else break;
                }
                return text;
            }
            finally
            {
                this.Dictionary.Relations = resRelations;
                this.Dictionary.DataSources = resDataSources;
                this.Dictionary.Databases = resDatabases;
            }
        }
        #endregion

        #region Methods.Save Editable Fields
        /// <summary>
        /// Saves the editable fields of rendered report to the stream.
        /// </summary>
        public StiReport SaveEditableFields(string path)
        {
            FileStream stream = null;
            try
            {
                StiFileUtils.ProcessReadOnly(path);
                stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                SaveEditableFields(stream);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving the editable fields of rendered report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if ((!StiOptions.Engine.HideExceptions) || this.IsDesigning) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            return this;
        }

        /// <summary>
        /// Saves the editable fields of rendered report to the stream.
        /// </summary>
        /// <param name="stream">Stream for saving the editable fields.</param>
        public StiReport SaveEditableFields(Stream stream)
        {
            StiLogService.Write(typeof(StiReport), "Saving the editable fields of rendered report.");
            try
            {
                var container = new StiEditableItemsContainer();
                int pageIndex = 0;
                foreach (StiPage page in this.RenderedPages)
                {
                    var pos = new Hashtable();
                    var comps = page.GetComponents();
                    foreach (StiComponent comp in comps)
                    {
                        var editable = comp as IStiEditable;
                        if (editable != null && editable.Editable)
                        {
                            int position = 1;
                            if (pos[comp.Name] != null) position = ((int)pos[comp.Name]) + 1;
                            container.Items.Add(new StiEditableItem(pageIndex, position, comp.Name, editable.SaveState()));
                            //position ++;
                            pos[comp.Name] = position;

                        }
                    }
                    pageIndex++;
                }

                StiSerializing ser = new StiSerializing(new StiReportObjectStringConverter());
                ser.Serialize(container, stream, "Editable Fields of Rendered Report");
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving the editable fields of rendered report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            return this;
        }

        /// <summary>
        /// Loads the editable fields of rendered report from the stream.
        /// </summary>
        public StiReport LoadEditableFields(string path)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                LoadEditableFields(stream);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Loading the editable fields of rendered report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if ((!StiOptions.Engine.HideExceptions) || this.IsDesigning) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            return this;
        }

        /// <summary>
        /// Loads the editable fields of rendered report from the stream.
        /// </summary>
        /// <param name="stream">Stream for loading the editable fields.</param>
        public StiReport LoadEditableFields(Stream stream)
        {
            StiLogService.Write(typeof(StiReport), "Loading the editable fields of rendered report.");
            try
            {
                var container = new StiEditableItemsContainer();
                var ser = new StiSerializing(new StiReportObjectStringConverter());
                ser.Deserialize(container, stream, "Editable Fields of Rendered Report");

                foreach (StiEditableItem item in container.Items)
                {
                    var pageIndex = item.PageIndex;
                    var position = item.Position;

                    if (pageIndex < this.RenderedPages.Count)
                    {
                        var page = this.RenderedPages[pageIndex];
                        var comps = page.GetComponents();
                        foreach (StiComponent comp in comps)
                        {
                            var editable = comp as IStiEditable;
                            if (editable != null && editable.Editable && comp.Name == item.ComponentName)
                            {
                                if (position == 1)
                                {
                                    editable.RestoreState(item.TextValue);
                                    break;
                                }
                                else position--;
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Loading the editable fields of rendered report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            return this;
        }
        #endregion

        #region Methods.Load report from assembly
        /// <summary>
        /// Returns the array of rendered reports from assembly.
        /// </summary>
        /// <param name="assembly">Assembly with reports.</param>
        /// <returns>The array of created reports.</returns>
        public static StiReport[] GetReportsFromAssembly(Assembly assembly)
        {
            try
            {
                var reports = new List<StiReport>();

                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(StiReport)))
                    {
                        var report = StiActivator.CreateObject(type) as StiReport;
                        StiReportResourcesLoader.LoadReportResourcesFromAssembly(report);
                        report.ApplyStyles();
                        reports.Add(report);
                    }
                }

                return reports.ToArray();
            }
            catch (Exception e)
            {
                StiLogService.Write(typeof(StiReport), "Get report from assembly...ERROR");
                StiLogService.Write(typeof(StiReport), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            return null;
        }

        /// <summary>
        /// Returns a created report from assembly.
        /// </summary>
        /// <param name="assembly">Assembly with a report.</param>
        /// <returns>Created report.</returns>
        public static StiReport GetReportFromAssembly(Assembly assembly)
        {
            try
            {
                var report = GetReportsFromAssembly(assembly)[0];
                report.UpdateReportVersion();
                return report;
            }
            catch (Exception e)
            {
                StiLogService.Write(typeof(StiReport), "Get report from assembly...ERROR");
                StiLogService.Write(typeof(StiReport), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            return null;
        }

        /// <summary>
        /// Returns a created report from assembly.
        /// </summary>
        /// <param name="assemblyFile">The path to assembly.</param>
        /// <returns>Created report.</returns>
        public static StiReport GetReportFromAssembly(string assemblyFile)
        {
            return GetReportFromAssembly(assemblyFile, false);
        }

        /// <summary>
        /// Returns a created report from assembly.
        /// </summary>
        /// <param name="assemblyFile">The path to assembly.</param>
        /// <param name="lockFile">If true then file with assembly is locked but assembly is loaded to memory only once.</param>
        /// <returns>Created report.</returns>
        public static StiReport GetReportFromAssembly(string assemblyFile, bool lockFile)
        {
            if (lockFile)
            {
                try
                {
                    var a = Assembly.LoadFrom(assemblyFile);
                    return GetReportFromAssembly(a);
                }
                catch (Exception e)
                {
                    StiLogService.Write(typeof(StiReport), "Get report from assembly...ERROR");
                    StiLogService.Write(typeof(StiReport), e);

                    if (!StiOptions.Engine.HideExceptions) throw;
                }
            }
            else
            {

                FileStream stream = null;
                try
                {
                    stream = new FileStream(assemblyFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return GetReportFromAssembly(stream);
                }
                catch (Exception e)
                {
                    StiLogService.Write(typeof(StiReport), "Get report from assembly...ERROR");
                    StiLogService.Write(typeof(StiReport), e);

                    if (!StiOptions.Engine.HideExceptions) throw;
                }
                finally
                {
                    stream.Close();
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a created report from assembly.
        /// </summary>
        /// <param name="assemblyStream">The stream with assembly.</param>
        /// <returns>Created report.</returns>
        public static StiReport GetReportFromAssembly(Stream assemblyStream)
        {
            try
            {
                var bytes = new byte[assemblyStream.Length];
                assemblyStream.Seek(0, SeekOrigin.Begin);
                assemblyStream.Read(bytes, 0, (int)assemblyStream.Length);
                var a = Assembly.Load(bytes);

                return GetReportsFromAssembly(a)[0];
            }
            catch (Exception e)
            {
                StiLogService.Write(typeof(StiReport), "Get report from assembly...ERROR");
                StiLogService.Write(typeof(StiReport), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            return null;
        }

        /// <summary>
        /// Returns a created report from the byte array.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <returns>Created report.</returns>
        public static StiReport GetReportFromAssembly(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            var report = GetReportFromAssembly(stream);
            report.IsDocument = false;
            stream.Close();
            return report;
        }
        #endregion

        #region Methods.Load document
        /// <summary>
        /// Loads a rendered report from the string.
        /// </summary>
        /// <param name="json">A string for loading a report from it.</param>
        public StiReport LoadDocumentFromJson(string json)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json ?? ""));
            return LoadDocumentFromJson(stream);
        }

        /// <summary>
        /// Loads a rendered report from the stream.
        /// </summary>
        /// <param name="stream"> A stream for loading a report template.</param>
        public StiReport LoadDocumentFromJson(Stream stream)
        {
            LoadFromJsonInternal(stream);

            IsRendered = true;
            NeedsCompiling = false;
            IsDocument = true;
            LoadDocumentFonts();

            return this;
        }

        /// <summary>
        /// Loads a encrypted rendered report from the stream.
        /// </summary>
        /// <param name="stream">The stream to load a encrypted rendered report.</param>
        /// <param name="key">The key for encryption.</param>
        public virtual StiReport LoadEncryptedDocument(Stream stream, string key)
        {
            #region Stream can't provide Seek operation
            if (!stream.CanSeek)
            {
                var tempBuffer = new byte[stream.Length - stream.Position];
                stream.Read(tempBuffer, (int)stream.Position, (int)(stream.Length - stream.Position));
                stream = new MemoryStream(tempBuffer);
            }
            #endregion

            var service = new StiEncryptedDocumentSLService { Key = key };
            LoadDocument(service, stream);

            return this;
        }

        /// <summary>
        /// Loads a encrypted rendered report from the file.
        /// </summary>
        /// <param name="path">The file which contains a encrypted rendered report.</param>
        /// <param name="key">The key for encryption.</param>
        public virtual StiReport LoadEncryptedDocument(string path, string key)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                LoadEncryptedDocument(stream, key);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Loading encrypted rendered report from file '" + path + "'...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            ReportFile = path;

            return this;
        }

        /// <summary>
        /// Loads a encrypted rendered report from the string.
        /// </summary>
        /// <param name="reportStr">A string from which the encrypted rendered report will be loaded from.</param>
        /// <param name="key">The key for encryption.</param>
        public virtual StiReport LoadEncryptedDocumentFromString(string reportStr, string key)
        {
            var bytes = global::System.Convert.FromBase64String(reportStr);

            if ((char)bytes[0] != 'm' || (char)bytes[1] != 'd' || (char)bytes[2] != 'x')
            {
                throw new Exception("This file is a not '.mdx' format.");
            }

            var dest = new byte[bytes.Length - 3];
            Array.Copy(bytes, 3, dest, 0, bytes.Length - 3);
            dest = StiEncryption.Decrypt(dest, key);

            try
            {
                dest = StiGZipHelper.Unpack(dest);
            }
            catch
            {
                throw new Exception("File decryption error: wrong key.");
            }

            var str = Encoding.UTF8.GetString(dest);
            this.LoadDocumentFromString(str);

            return this;
        }

        /// <summary>
        /// Loads a encrypted rendered report from the byte array.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <param name="key">The key for encryption.</param>
        public virtual StiReport LoadEncryptedDocument(byte[] bytes, string key)
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(bytes);
                LoadEncryptedDocument(stream, key);
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            return this;
        }

        /// <summary>
        /// Loads a packed rendered report from the stream.
        /// </summary>
        /// <param name="stream">The stream to load a packed rendered report.</param>
        public virtual StiReport LoadPackedDocument(Stream stream)
        {
            #region Stream can't provide Seek operation
            if (!stream.CanSeek)
            {
                var tempBuffer = new byte[stream.Length - stream.Position];
                stream.Read(tempBuffer, (int)stream.Position, (int)(stream.Length - stream.Position));
                stream = new MemoryStream(tempBuffer);
            }
            #endregion

            if (!IsPackedFile(stream))
            {
                LoadDocument(stream);
            }
            else
            {
                var service = new StiPackedDocumentSLService();
                LoadDocument(service, stream);
            }

            return this;
        }

        /// <summary>
        /// Loads a packed rendered report from the file.
        /// </summary>
        /// <param name="path">The file which contains a packed rendered report.</param>
        public virtual StiReport LoadPackedDocument(string path)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                LoadPackedDocument(stream);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Loading packed rendered report from file '" + path + "'...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            ReportFile = path;

            return this;
        }

        /// <summary>
        /// Loads a packed rendered report from the string.
        /// </summary>
        /// <param name="reportStr">A string from which the packed rendered report will be loaded from.</param>
        public virtual StiReport LoadPackedDocumentFromString(string reportStr)
        {
            var str = StiGZipHelper.Unpack(reportStr);
            this.LoadDocumentFromString(str);

            return this;
        }

        /// <summary>
        /// Loads a packed rendered report from the byte array.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        public virtual StiReport LoadPackedDocument(byte[] bytes)
        {
            if (!IsPackedFile(bytes))
            {
                LoadDocument(bytes);
            }
            else
            {
                MemoryStream stream = null;
                try
                {
                    stream = new MemoryStream(bytes);
                    LoadPackedDocument(stream);
                }
                finally
                {
                    if (stream != null) stream.Close();
                }
            }

            return this;
        }

        /// <summary>
        /// Loads a rendered report from the string.
        /// </summary>
        /// <param name="reportStr">A string for loading a report from it.</param>
        public virtual StiReport LoadDocumentFromString(string reportStr)
        {
            MemoryStream ms = null;
            StreamWriter sw = null;
            try
            {
                ms = new MemoryStream();
                sw = new StreamWriter(ms);
                sw.Write(reportStr);
                sw.Flush();
                ms.Flush();

                ms.Seek(0, SeekOrigin.Begin);

                LoadDocument(ms);

            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Loading rendered report from string");
                StiLogService.Write(this.GetType(), e);

                if ((!StiOptions.Engine.HideExceptions) || this.IsDesigning) throw;
            }
            finally
            {
                if (sw != null) sw.Close();
                if (ms != null) ms.Close();
            }
            return this;
        }

        /// <summary>
        /// Loads a rendered report from the byte array.
        /// </summary>
        /// <param name="bytes">The byte frray for loading a rendered report from it.</param>
        public virtual StiReport LoadDocument(byte[] bytes)
        {
            if (IsPackedFile(bytes))
            {
                LoadPackedDocument(bytes);
            }
            else
            {
                MemoryStream stream = null;
                try
                {
                    stream = new MemoryStream(bytes);
                    LoadDocument(stream);
                }
                finally
                {
                    if (stream != null) stream.Close();
                }
            }
            return this;
        }

        /// <summary>
        /// Loads a rendered report from the stream.
        /// </summary>
        /// <param name="stream">A stream for loading a rendered report.</param>
        public virtual StiReport LoadDocument(Stream stream)
        {
            #region Stream can't provide Seek operation
            if (!stream.CanSeek)
            {
                var tempBuffer = new byte[stream.Length - stream.Position];
                stream.Read(tempBuffer, (int)stream.Position, (int)(stream.Length - stream.Position));
                stream = new MemoryStream(tempBuffer);
            }
            #endregion

            if (IsJsonFile(stream))
                LoadDocumentFromJson(stream);

            else if (IsPackedFile(stream))
                LoadPackedDocument(stream);

            else
            {
                var service = new StiXmlDocumentSLService();
                LoadDocument(service, stream);
            }

            return this;
        }

        /// <summary>
        /// Loads a rendered report from the file.
        /// </summary>
        /// <param name="path">A file which contains a rendered report.</param>
        public virtual StiReport LoadDocument(string path)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                LoadDocument(stream);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Loading rendered report from file '" + path + "'...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            ReportFile = path;

            return this;
        }

        /// <summary>
        /// Loads a rendered report from the stream using the provider.
        /// </summary>
        /// <param name="service">A provider for loading a rendered report.</param>
        /// <param name="stream">A stream for loading a rendered report.</param>
        public virtual StiReport LoadDocument(StiDocumentSLService service, Stream stream)
        {
            try
            {
                StiLogService.Write(typeof(StiReport), "Loading rendered report");
                service.Load(this, stream);
                IsRendered = true;
                NeedsCompiling = false;
                IsDocument = true;
                LoadDocumentFonts();
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Loading rendered report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }

            return this;
        }

        /// <summary>
        /// Loads a rendered report from the file using the provider.
        /// </summary>
        /// <param name="service">A provider for loading a rendered report.</param>
        /// <param name="path">A file for loading a rendered report.</param>
        public virtual StiReport LoadDocument(StiDocumentSLService service, string path)
        {
            StiLogService.Write(typeof(StiReport), "Loading rendered report");
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                service.Load(this, stream);
                IsRendered = true;
                NeedsCompiling = false;
                IsDocument = true;
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Loading rendered report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            finally
            {
                stream.Close();
            }
            return this;
        }

        /// <summary>
        /// Loads a rendered report template from specified url.
        /// </summary>
        /// <param name="url">Url which will be used for report loading.</param>
        public virtual StiReport LoadDocumentFromUrl(string url)
        {
            using (var cl = new WebClient())
            {
                cl.Proxy = StiProxy.GetProxy();
                cl.Credentials = CredentialCache.DefaultCredentials;

                var bytes = cl.DownloadData(url);
                var stream = new MemoryStream(bytes);
                LoadDocument(stream);
            }

            return this;
        }
        #endregion

        #region Methods.Load report
        /// <summary>
        /// Loads a report template from the string.
        /// </summary>
        /// <param name="json">A string which contains the report template.</param>
        public StiReport LoadFromJson(string json)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json ?? ""));
            return LoadFromJson(stream);
        }

        /// <summary>
        /// Loads a report template from the stream.
        /// </summary>
        /// <param name="stream">A stream for loading a report template.</param>
        public StiReport LoadFromJson(Stream stream)
        {
            Clear(false);
            LoadFromJsonInternal(stream);
            LoadFonts();

            return this;
        }

        /// <summary>
        /// Loads a encrypted report template from the stream.
        /// </summary>
        /// <param name="stream">A stream to load a encrypted report template.</param>
        /// <param name="key">The key for encryption.</param>
        public virtual StiReport LoadEncryptedReport(Stream stream, string key)
        {
            #region Stream can't provide Seek operation
            if (!stream.CanSeek)
            {
                var tempBuffer = new byte[stream.Length - stream.Position];
                stream.Read(tempBuffer, (int)stream.Position, (int)(stream.Length - stream.Position));
                stream = new MemoryStream(tempBuffer);
            }
            #endregion

            ReportFile = string.Empty;
            Clear();
            var service = new StiEncryptedReportSLService { Key = key };

            StiOptions.Engine.GlobalEvents.InvokeReportLoading(this, EventArgs.Empty);
            Load(service, stream);
            StiOptions.Engine.GlobalEvents.InvokeReportLoaded(this, EventArgs.Empty);

            IsPackedReport = true;
            IsJsonReport = false;
            isModified = false;
            ReportFile = string.Empty;
            Password = key;

            return this;
        }

        /// <summary>
        /// Loads a encrypted report template from the file.
        /// </summary>
        /// <param name="path">A file which contains a encrypted report template.</param>
        /// <param name="key">The key for encryption.</param>
        public virtual StiReport LoadEncryptedReport(string path, string key)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                LoadEncryptedReport(stream, key);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Loading encrypted report from file '" + path + "'...ERROR");
                StiLogService.Write(this.GetType(), e);

                if ((!StiOptions.Engine.HideExceptions) || this.IsDesigning) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
                this.Password = key;
            }
            ReportFile = path;

            return this;
        }

        /// <summary>
        /// Loads a encrypted report template from the string.
        /// </summary>
        /// <param name="reportStr">A string which contains the encrypted template.</param>
        /// <param name="key">The key for encryption.</param>
        public virtual StiReport LoadEncryptedReportFromString(string reportStr, string key)
        {
            var bytes = global::System.Convert.FromBase64String(reportStr);

            if ((char)bytes[0] != 'm' || (char)bytes[1] != 'r' || (char)bytes[2] != 'x')
            {
                throw new Exception("This file is a not '.mrx' format.");
            }

            var dest = new byte[bytes.Length - 3];
            Array.Copy(bytes, 3, dest, 0, bytes.Length - 3);
            dest = StiEncryption.Decrypt(dest, key);

            try
            {
                dest = StiGZipHelper.Unpack(dest);
            }
            catch
            {
                throw new Exception("File decryption error: wrong key.");
            }

            var str = Encoding.UTF8.GetString(dest);
            this.LoadFromString(str);
            this.Password = key;

            return this;
        }

        /// <summary>
        /// Loads a encrypted report template from the byte array.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <param name="key">The key for encryption.</param>
        public virtual StiReport LoadEncryptedReport(byte[] bytes, string key)
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(bytes);
                LoadEncryptedReport(stream, key);
            }
            finally
            {
                if (stream != null) stream.Close();
                this.Password = key;
            }

            return this;
        }

        /// <summary>
        /// Loads a packed report template from the stream.
        /// </summary>
        /// <param name="stream">A stream to load a packed report template.</param>
        public virtual StiReport LoadPackedReport(Stream stream)
        {
            #region Stream can't provide Seek operation
            if (!stream.CanSeek)
            {
                var tempBuffer = new byte[stream.Length - stream.Position];
                stream.Read(tempBuffer, (int)stream.Position, (int)(stream.Length - stream.Position));
                stream = new MemoryStream(tempBuffer);
            }
            #endregion

            if (!IsPackedFile(stream))
            {
                Load(stream);
            }
            else
            {
                ReportFile = string.Empty;
                Clear();
                var service = new StiPackedReportSLService();

                StiOptions.Engine.GlobalEvents.InvokeReportLoading(this, EventArgs.Empty);
                Load(service, stream);
                StiOptions.Engine.GlobalEvents.InvokeReportLoaded(this, EventArgs.Empty);

                IsPackedReport = true;
                isModified = false;
                ReportFile = string.Empty;
            }
            return this;
        }

        /// <summary>
        /// Loads a packed report template from the file.
        /// </summary>
        /// <param name="path">A file which contains a packed report template.</param>
        public virtual StiReport LoadPackedReport(string path)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                LoadPackedReport(stream);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Loading packed report from file '" + path + "'...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions || this.IsDesigning) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            ReportFile = path;

            return this;
        }

        /// <summary>
        /// Loads a packed report template from the string.
        /// </summary>
        /// <param name="reportStr">A string which contains the report template.</param>
        public virtual StiReport LoadPackedReportFromString(string reportStr)
        {
            var str = StiGZipHelper.Unpack(reportStr);
            this.LoadFromString(str);

            return this;
        }

        /// <summary>
        /// Loads a packed report template from the byte array.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        public virtual StiReport LoadPackedReport(byte[] bytes)
        {
            if (!IsPackedFile(bytes))
            {
                Load(bytes);
            }
            else
            {
                MemoryStream stream = null;
                try
                {
                    stream = new MemoryStream(bytes);
                    LoadPackedReport(stream);
                }
                finally
                {
                    if (stream != null) stream.Close();
                }
            }

            return this;
        }

        /// <summary>
        /// Loads a report template from the stream.
        /// </summary>
        /// <param name="stream">A stream for loading a report template.</param>
        public virtual StiReport Load(Stream stream)
        {
            #region Stream can't provide Seek operation
            if (!stream.CanSeek)
            {
                var tempBuffer = new byte[stream.Length - stream.Position];
                stream.Read(tempBuffer, (int)stream.Position, (int)(stream.Length - stream.Position));
                stream = new MemoryStream(tempBuffer);
            }
            #endregion

            if (IsPackedFile(stream))
            {
                LoadPackedReport(stream);
            }
            else
            {
                ReportFile = string.Empty;
                IsJsonReport = false;
                Clear(false);

                StiReportSLService service;

                if (IsJsonFile(stream))
                {
                    service = new StiJsonReportSLService();
                    IsJsonReport = true;
                }
                else
                {
                    service = new StiXmlReportSLService();
                }

                Load(service, stream);
                IsPackedReport = false;
                isModified = false;
                ReportFile = string.Empty;
                IsDocument = false;
            }

            return this;
        }

        /// <summary>
        /// Loads a report template from the byte array.
        /// </summary>
        /// <param name="bytes">A byte array which contains the report template.</param>
        public virtual StiReport Load(byte[] bytes)
        {
            if (IsPackedFile(bytes))
            {
                LoadPackedReport(bytes);
            }
            else
            {
                MemoryStream stream = null;
                try
                {
                    stream = new MemoryStream(bytes);
                    Load(stream);
                }
                finally
                {
                    if (stream != null) stream.Close();
                }
            }

            return this;
        }

        /// <summary>
        /// Loads a report template from the file.
        /// </summary>
        /// <param name="path">A file which contains the report template.</param>
        public virtual StiReport Load(string path)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                Load(stream);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Loading report from file '" + path + "'...ERROR");
                StiLogService.Write(this.GetType(), e);

                if ((!StiOptions.Engine.HideExceptions) || this.IsDesigning) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            ReportFile = path;

            return this;
        }

        /// <summary>
        /// Loads a report template from the string.
        /// </summary>
        /// <param name="reportStr">A string which contains the report template.</param>
        public virtual StiReport LoadFromString(string reportStr)
        {
            MemoryStream ms = null;
            StreamWriter sw = null;
            try
            {
                ms = new MemoryStream();
                sw = new StreamWriter(ms);
                sw.Write(reportStr);
                sw.Flush();
                ms.Flush();

                ms.Seek(0, SeekOrigin.Begin);

                Load(ms);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Loading report from string");
                StiLogService.Write(this.GetType(), e);

                if ((!StiOptions.Engine.HideExceptions) || this.IsDesigning) throw;
            }
            finally
            {
                if (sw != null) sw.Close();
                if (ms != null) ms.Close();
            }

            return this;
        }

        /// <summary>
        /// Loads a report template from the stream using provider.
        /// </summary>
        /// <param name="service">A provider which loads a report template.</param>
        /// <param name="stream">A stream for loading a report template.</param>
        public virtual StiReport Load(StiReportSLService service, Stream stream)
        {
            try
            {
                Clear(false);
                StiLogService.Write(typeof(StiReport), "Loading report");
                StiOptions.Engine.GlobalEvents.InvokeReportLoading(this, EventArgs.Empty);

                this.EngineVersion = StiEngineVersion.EngineV1;
                service.Load(this, stream);

                StiOptions.Engine.GlobalEvents.InvokeReportLoaded(this, EventArgs.Empty);
                ApplyStyles();
                LoadFonts();
                CorrectFormatProperties();
                if (StiOptions.Engine.FullTrust && StiOptions.Engine.PackScriptAfterReportLoaded) ScriptPack();
                this.UpdateInheritedReport();

                IsJsonReport = service is StiJsonReportSLService;

                IsPackedReport = false;
                isModified = false;
                IsDocument = false;

                ReportFile = string.Empty;
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Loading report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if ((!StiOptions.Engine.HideExceptions) || this.IsDesigning) throw;
            }

            return this;
        }

        /// <summary>
        /// Fix - correct Format property of the Text components. 
        /// It's serialization order issue, Format property is calculated before all properties are applied to the TextFormat.
        /// </summary>
        private void CorrectFormatProperties()
        {
            var comps = this.GetComponents();
            foreach (StiComponent comp in comps)
            {
                var text = comp as StiText;
                if (text != null && !(text.TextFormat is Stimulsoft.Report.Components.TextFormats.StiGeneralFormatService))
                {
                    var storedService = text.TextFormat;
                    text.TextFormat = null;
                    text.TextFormat = storedService;
                }
            }
        }

        /// <summary>
        /// Loads a report template from the file using the provider.
        /// </summary>
        /// <param name="service">A provider which loads a report template.</param>
        /// <param name="path">A file for loading a report template.</param>
        public virtual StiReport Load(StiReportSLService service, string path)
        {
            Clear(false);
            StiLogService.Write(typeof(StiReport), "Loading report");
            FileStream stream = null;

            try
            {
                this.EngineVersion = StiEngineVersion.EngineV1;
                stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                service.Load(this, stream);
                IsDocument = false;

                this.UpdateInheritedReport();
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Loading report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if ((!StiOptions.Engine.HideExceptions) || this.IsDesigning) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            ReportFile = path;

            return this;
        }

        /// <summary>
        /// Loads a report template from specified url.
        /// </summary>
        /// <param name="url">Url which will be used for report loading.</param>
        public virtual StiReport LoadFromUrl(string url)
        {
            using (var cl = new WebClient())
            {
                cl.Proxy = StiProxy.GetProxy();
                cl.Credentials = CredentialCache.DefaultCredentials;
                var bytes = cl.DownloadData(url);
                var stream = new MemoryStream(bytes);
                Load(stream);
            }
            return this;
        }
        #endregion

        #region Methods.Save&Load Json Helper
        internal string SaveToJsonInternal(StiJsonSaveMode mode)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                bool needOptimize = (mode == StiJsonSaveMode.Document) && (StiOptions.Engine.DocumentSavingOptimization || renderedPages.CacheMode) && (renderedPages.Count > 1);

                #region Save
                var rootJsonObject = new JObject();

                rootJsonObject.AddPropertyStringNullOrEmpty(nameof(ReportVersion), ReportVersion);
                rootJsonObject.AddPropertyStringNullOrEmpty(nameof(ReportGuid), ReportGuid);
                rootJsonObject.AddPropertyStringNullOrEmpty(nameof(ReportName), PropName);
                rootJsonObject.AddPropertyStringNullOrEmpty(nameof(ReportAlias), ReportAlias);
                rootJsonObject.AddPropertyStringNullOrEmpty(nameof(ReportAuthor), ReportAuthor);
                rootJsonObject.AddPropertyStringNullOrEmpty(nameof(ReportDescription), ReportDescription);
                rootJsonObject.AddPropertyDateTime(nameof(ReportCreated), ReportCreated);              // DateTime !!!!!!!!!!!!
                rootJsonObject.AddPropertyDateTime(nameof(ReportChanged), ReportChanged);              // DateTime !!!!!!!!!!!!
                rootJsonObject.AddPropertyEnum(nameof(EngineVersion), EngineVersion);
                rootJsonObject.AddPropertyEnum(nameof(NumberOfPass), NumberOfPass, StiNumberOfPass.SinglePass);
                rootJsonObject.AddPropertyEnum(nameof(CalculationMode), CalculationMode, StiCalculationMode.Compilation);
                rootJsonObject.AddPropertyEnum(nameof(ReportUnit), ReportUnit, StiReportUnitType.Centimeters);
                rootJsonObject.AddPropertyBool(nameof(CacheAllData), CacheAllData);
                rootJsonObject.AddPropertyEnum(nameof(ReportCacheMode), ReportCacheMode, StiReportCacheMode.Off);
                rootJsonObject.AddPropertyBool(nameof(ConvertNulls), ConvertNulls, true);
                rootJsonObject.AddPropertyBool(nameof(RetrieveOnlyUsedData), RetrieveOnlyUsedData);
                rootJsonObject.AddPropertyEnum(nameof(PreviewMode), PreviewMode, StiPreviewMode.Standard);
                rootJsonObject.AddPropertyEnum(nameof(HtmlPreviewMode), HtmlPreviewMode, StiHtmlPreviewMode.Div);
                rootJsonObject.AddPropertyInt(nameof(StopBeforePage), StopBeforePage);
                rootJsonObject.AddPropertyInt(nameof(StopBeforeTime), StopBeforeTime);
                rootJsonObject.AddPropertyInt(nameof(PreviewSettings), PreviewSettings, (int)(StiPreviewSettings.Default));
                rootJsonObject.AddPropertyInt(nameof(DashboardViewerSettings), (int)DashboardViewerSettings, (int)StiDashboardViewerSettings.All);
                rootJsonObject.AddPropertyInt(nameof(Collate), Collate, 1);
                rootJsonObject.AddPropertyEnum(nameof(ScriptLanguage), ScriptLanguage, StiReportLanguageType.CSharp);
                rootJsonObject.AddPropertyBool(nameof(AutoLocalizeReportOnRun), AutoLocalizeReportOnRun);
                rootJsonObject.AddPropertyEnum(nameof(ParametersOrientation), ParametersOrientation, StiOrientation.Horizontal);
                rootJsonObject.AddPropertyInt(nameof(ParameterWidth), ParameterWidth, 0);
                rootJsonObject.AddPropertyBool(nameof(RequestParameters), RequestParameters);                
                rootJsonObject.AddPropertyBool(nameof(CacheTotals), CacheTotals);
                rootJsonObject.AddPropertyStringNullOrEmpty(nameof(Culture), Culture);

                if (ReportIcon != null)
                    rootJsonObject.AddPropertyByteArray(nameof(ReportIcon), ReportIcon);

                if (ReportImage != null)
                    rootJsonObject.AddPropertyImage(nameof(ReportImage), ReportImage);

                rootJsonObject.AddPropertyStringNullOrEmpty(nameof(Script), Script);

                if (mode == StiJsonSaveMode.Document)
                {
                    if (needOptimize)
                        rootJsonObject.AddPropertyString(nameof(RenderedPages), placeForRenderedPages);
                    else
                        rootJsonObject.AddPropertyJObject(nameof(RenderedPages), this.RenderedPages.SaveToJsonObject(mode));
                    rootJsonObject.AddPropertyJObject(nameof(Styles), Styles.SaveToJsonObject(mode));
                    rootJsonObject.AddPropertyJObject("Resources", Dictionary.Resources.SaveToJsonObjectEx(mode));
                    rootJsonObject.AddPropertyJObject(nameof(Bookmark), Bookmark?.SaveToJsonObject(mode));
                }
                else
                {
                    rootJsonObject.AddPropertyInt(nameof(RefreshTime), RefreshTime);

                    rootJsonObject.AddPropertyJObject(nameof(PrinterSettings), PrinterSettings.SaveToJsonObject(mode));
                    rootJsonObject.AddPropertyJObject(nameof(PreviewToolBarOptions), PreviewToolBarOptions.SaveToJsonObject(mode));
                    rootJsonObject.AddPropertyJObject(nameof(MetaTags), MetaTags.SaveToJsonObject(mode));
                    rootJsonObject.AddPropertyJObject(nameof(Styles), Styles.SaveToJsonObject(mode));
                    rootJsonObject.AddPropertyStringArray(nameof(ReferencedAssemblies), ReferencedAssemblies);

                    rootJsonObject.AddPropertyJObject(nameof(BeginRenderEvent), BeginRenderEvent.SaveToJsonObject(mode));
                    rootJsonObject.AddPropertyJObject(nameof(RenderingEvent), RenderingEvent.SaveToJsonObject(mode));
                    rootJsonObject.AddPropertyJObject(nameof(EndRenderEvent), EndRenderEvent.SaveToJsonObject(mode));
                    rootJsonObject.AddPropertyJObject(nameof(ExportingEvent), ExportingEvent.SaveToJsonObject(mode));
                    rootJsonObject.AddPropertyJObject(nameof(ExportedEvent), ExportedEvent.SaveToJsonObject(mode));
                    rootJsonObject.AddPropertyJObject(nameof(PrintingEvent), PrintingEvent.SaveToJsonObject(mode));
                    rootJsonObject.AddPropertyJObject(nameof(PrintedEvent), PrintedEvent.SaveToJsonObject(mode));
                    rootJsonObject.AddPropertyJObject(nameof(RefreshingEvent), RefreshingEvent.SaveToJsonObject(mode));

                    rootJsonObject.AddPropertyJObject(nameof(ReportResources), ReportResources.SaveToJsonObject(mode));
                    rootJsonObject.AddPropertyJObject(nameof(GlobalizationStrings), GlobalizationStrings.SaveToJsonObject(mode));
                    rootJsonObject.AddPropertyJObject(nameof(Dictionary), Dictionary.SaveToJsonObject(mode));

                    rootJsonObject.AddPropertyJObject(nameof(Pages), Pages.SaveToJsonObject(mode));
                }

                if (needOptimize)
                {
                    #region Save optimized
                    string fullMdc1 = rootJsonObject.ToString();
                    int posPlace = fullMdc1.IndexOf(placeForRenderedPages);
                    if (posPlace == -1)
                    {
                        //something is wrong, using standard method
                        rootJsonObject.AddPropertyJObject("RenderedPages", this.RenderedPages.SaveToJsonObject(mode));
                        return rootJsonObject.ToString();
                    }

                    StringBuilder res = new StringBuilder();
                    res.Append(fullMdc1.Substring(0, posPlace - 1));
                    res.AppendLine("{");

                    int index = 0;
                    foreach (StiPage page in renderedPages)
                    {
                        string st = page.SaveToJsonObject(mode).ToString();
                        res.Append("\"" + index.ToString() + "\": ");
                        res.Append(st);
                        if (index < renderedPages.Count - 1)
                            res.AppendLine(",");
                        else
                            res.AppendLine();
                        index++;
                    }

                    res.Append("}");
                    res.Append(fullMdc1.Substring(posPlace + placeForRenderedPages.Length + 1));
                    return res.ToString();
                    #endregion
                }

                return rootJsonObject.ToString();
                #endregion
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        private const string placeForRenderedPages = "-=*PlaceForRenderedPages*=-";

        internal void LoadFromJsonInternal(Stream stream)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                #region Load

                jsonLoaderHelper = new StiReportJsonLoaderHelper();

                this.Pages.Clear();
                this.Dictionary.Clear();
                this.renderedPages.Clear();
                this.ReportUnit = StiReportUnitType.Centimeters;

                string reportString;
                string[] pagesString;
                StiJsonReportObjectHelper.SplitReportJson(stream, out reportString, out pagesString);
                var reportJObject = JObject.Parse(reportString);

                foreach (var property in reportJObject.Properties())
                {
                    switch (property.Name)
                    {
                        case nameof(ReportVersion):
                            this.ReportVersion = property.DeserializeString();
                            break;

                        case nameof(ReportGuid):
                            this.ReportGuid = property.DeserializeString();
                            break;

                        case nameof(ReportName):
                            this.PropName = property.DeserializeString();
                            break;

                        case nameof(ReportAlias):
                            this.ReportAlias = property.DeserializeString();
                            break;

                        case nameof(ReportFile):
                            this.ReportFile = property.DeserializeString();
                            break;

                        case nameof(ReportAuthor):
                            this.ReportAuthor = property.DeserializeString();
                            break;

                        case nameof(RetrieveOnlyUsedData):
                            this.RetrieveOnlyUsedData = property.DeserializeBool();
                            break;

                        case nameof(ReportIcon):
                            this.ReportIcon = property.DeserializeByteArray();
                            break;

                        case nameof(ReportImage):
                            this.ReportImage = property.DeserializeImage();
                            break;

                        case nameof(ReportDescription):
                            this.ReportDescription = property.DeserializeString();
                            break;

                        case nameof(ReportCreated):
                            this.ReportCreated = property.DeserializeDateTime();
                            break;

                        case nameof(ReportChanged):
                            this.ReportChanged = property.DeserializeDateTime();
                            break;

                        case nameof(EngineVersion):
                            this.EngineVersion = property.DeserializeEnum<StiEngineVersion>();
                            break;

                        case nameof(NumberOfPass):
                            this.NumberOfPass = property.DeserializeEnum<StiNumberOfPass>();
                            break;

                        case nameof(CalculationMode):
                            this.CalculationMode = property.DeserializeEnum<StiCalculationMode>();
                            break;

                        case nameof(ReportUnit):
                            this.ReportUnit = property.DeserializeEnum<StiReportUnitType>();
                            break;

                        case nameof(CacheAllData):
                            this.CacheAllData = property.DeserializeBool();
                            break;

                        case nameof(ReportCacheMode):
                            this.ReportCacheMode = property.DeserializeEnum<StiReportCacheMode>();
                            break;

                        case nameof(ConvertNulls):
                            this.ConvertNulls = property.DeserializeBool();
                            break;

                        case nameof(PreviewMode):
                            this.PreviewMode = property.DeserializeEnum<StiPreviewMode>();
                            break;

                        case nameof(HtmlPreviewMode):
                            this.HtmlPreviewMode = property.DeserializeEnum<StiHtmlPreviewMode>();
                            break;

                        case nameof(StopBeforePage):
                            this.StopBeforePage = property.DeserializeInt();
                            break;

                        case nameof(StopBeforeTime):
                            this.StopBeforeTime = property.DeserializeInt();
                            break;

                        case nameof(PreviewSettings):
                            this.PreviewSettings = property.DeserializeInt();
                            break;

                        case nameof(DashboardViewerSettings):
                            this.DashboardViewerSettings = (StiDashboardViewerSettings)property.DeserializeInt();
                            break;

                        case nameof(Collate):
                            this.Collate = property.DeserializeInt();
                            break;

                        case nameof(ReferencedAssemblies):
                            this.ReferencedAssemblies = property.DeserializeStringArray();
                            break;

                        case nameof(ScriptLanguage):
                            this.ScriptLanguage = property.DeserializeEnum<StiReportLanguageType>();
                            break;

                        case nameof(AutoLocalizeReportOnRun):
                            this.AutoLocalizeReportOnRun = property.DeserializeBool();
                            break;

                        case nameof(ParametersOrientation):
                            this.ParametersOrientation = property.DeserializeEnum<StiOrientation>();
                            break;

                        case nameof(ParameterWidth):
                            this.ParameterWidth = property.DeserializeInt();
                            break;

                        case nameof(RequestParameters):
                            this.RequestParameters = property.DeserializeBool();
                            break;

                        case nameof(CacheTotals):
                            this.CacheTotals = property.DeserializeBool();
                            break;

                        case nameof(Culture):
                            this.Culture = property.DeserializeString();
                            break;

                        case nameof(Script):
                            this.Script = property.DeserializeString();
                            break;

                        case nameof(BeginRenderEvent):
                            this.BeginRenderEvent.LoadFromJsonObject((JObject)property.Value);
                            break;

                        case nameof(RenderingEvent):
                            this.RenderingEvent.LoadFromJsonObject((JObject)property.Value);
                            break;

                        case nameof(EndRenderEvent):
                            this.EndRenderEvent.LoadFromJsonObject((JObject)property.Value);
                            break;

                        case nameof(ExportingEvent):
                            this.ExportingEvent.LoadFromJsonObject((JObject)property.Value);
                            break;

                        case nameof(ExportedEvent):
                            this.ExportedEvent.LoadFromJsonObject((JObject)property.Value);
                            break;

                        case nameof(PrintingEvent):
                            this.PrintingEvent.LoadFromJsonObject((JObject)property.Value);
                            break;

                        case nameof(PrintedEvent):
                            this.PrintedEvent.LoadFromJsonObject((JObject)property.Value);
                            break;

                        case nameof(RefreshingEvent):
                            this.RefreshingEvent.LoadFromJsonObject((JObject)property.Value);
                            break;

                        case nameof(MetaTags):
                            this.MetaTags.LoadFromJsonObject((JObject)property.Value);
                            break;

                        case nameof(ReportResources):
                            this.ReportResources.LoadFromJsonObject((JObject)property.Value);
                            break;

                        case nameof(GlobalizationStrings):
                            this.GlobalizationStrings.LoadFromJsonObject((JObject)property.Value);
                            break;

                        case nameof(PrinterSettings):
                            this.PrinterSettings.LoadFromJsonObject((JObject)property.Value);
                            break;

                        case nameof(PreviewToolBarOptions):
                            this.PreviewToolBarOptions.LoadFromJsonObject((JObject)property.Value);
                            break;

                        case nameof(Styles):
                            this.Styles.LoadFromJsonObject((JObject)property.Value);
                            break;

                        case nameof(Dictionary):
                            this.Dictionary.LoadFromJsonObject((JObject)property.Value);
                            break;

                        case "Resources":
                            this.Dictionary.Resources.LoadFromJsonObjectEx((JObject)property.Value, this);
                            break;

                        case nameof(Pages):
                            this.Pages.LoadFromJsonObject((JObject)property.Value);
                            break;

                        case nameof(RenderedPages):
                            for (var index = 0; index < pagesString.Length; index++)
                            {
                                var pageJObject = JObject.Parse(pagesString[index]);
                                this.RenderedPages.LoadFromJsonObjectInternal((JObject)pageJObject);
                                pageJObject = null;
                                pagesString[index] = "";
                            }
                            break;

                        case nameof(RefreshTime):
                            this.RefreshTime = property.DeserializeInt();
                            break;

                        case nameof(RenderedWith):
                            this.RenderedWith = property.DeserializeEnum<StiRenderedWith>();
                            break;

                        case nameof(Bookmark):
                            this.Bookmark.LoadFromJsonObject((JObject)property.Value);
                            break;

                        default:
                            throw new Exception("Property is not supported!");
                    }
                }

                reportString = null;
                reportJObject = null;
                pagesString = null;
                #region Check

                StiComponentsCollection reportComps = null;

                if (jsonLoaderHelper.MasterComponents.Count > 0)
                {
                    reportComps = this.GetComponents();

                    foreach (var masterComponent in jsonLoaderHelper.MasterComponents)
                    {
                        var chart = masterComponent as Stimulsoft.Report.Chart.StiChart;
                        if (chart != null)
                        {
                            chart.MasterComponent = reportComps[chart.jsonMasterComponentTemp];
                            chart.jsonMasterComponentTemp = null;
                            continue;
                        }

                        var dataBand = masterComponent as StiDataBand;
                        if (dataBand != null)
                        {
                            dataBand.MasterComponent = reportComps[dataBand.jsonMasterComponentTemp];
                            dataBand.jsonMasterComponentTemp = null;
                            continue;
                        }
                    }
                }

                if (jsonLoaderHelper.Clones.Count > 0)
                {
                    if (reportComps == null)
                        reportComps = this.GetComponents();

                    foreach (var clone in jsonLoaderHelper.Clones)
                    {
                        clone.Container = reportComps[clone.jsonContainerValueTemp] as StiContainer;
                        clone.jsonContainerValueTemp = null;
                    }
                }

                if (jsonLoaderHelper.DialogInfo.Count > 0)
                {
                    foreach (var info in jsonLoaderHelper.DialogInfo)
                    {
                        info.BindingVariable = this.Dictionary.Variables[info.jsonLoadedBindingVariableName];
                        info.jsonLoadedBindingVariableName = null;
                    }
                }

                jsonLoaderHelper.Clean();
                jsonLoaderHelper = null;

                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), "Loading report...ERROR");
                StiLogService.Write(this.GetType(), ex);

                if ((!StiOptions.Engine.HideExceptions) || this.IsDesigning) throw;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }
        #endregion

        #region Methods.Save report
        /// <summary>
        /// Saves a report template to the string.
        /// </summary>
        /// <returns>A string which contains the report template.</returns>
        public string SaveToJsonString()
        {
            return SaveToJsonInternal(StiJsonSaveMode.Report);
        }

        /// <summary>
        /// Saves a encrypted report template in the stream.
        /// </summary>
        /// <param name="stream">A stream to save a encrypted report template.</param>
        public virtual StiReport SaveEncryptedReport(Stream stream, string key)
        {
            var service = new StiEncryptedReportSLService { Key = key };

            StiOptions.Engine.GlobalEvents.InvokeReportSaving(this, EventArgs.Empty);
            Save(service, stream);
            StiOptions.Engine.GlobalEvents.InvokeReportSaved(this, EventArgs.Empty);

            ReportFile = string.Empty;
            IsModified = false;
            Password = key;

            return this;
        }

        /// <summary>
        /// Saves a encrypted report template in the file.
        /// </summary>
        /// <param name="path">A file to save a encrypted report template.</param>
        public virtual StiReport SaveEncryptedReport(string path, string key)
        {
            FileStream stream = null;
            try
            {
                if (!IsDesigning || !StiOptions.Designer.ReadOnlyAlertOnSave)
                {
                    StiFileUtils.ProcessReadOnly(path);
                }
                stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                SaveEncryptedReport(stream, key);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving encrypted report to file '" + path + "'...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions || this.IsDesigning) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
                Password = key;
            }
            ReportFile = path;

            return this;
        }

        /// <summary>
        /// Saves a encrypted report template in the byte array.
        /// </summary>
        /// <returns>A new byte array containing the encrypted report template.</returns>
        public virtual byte[] SaveEncryptedReportToByteArray(string key)
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream();
                SaveEncryptedReport(stream, key);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Flush();
                    stream.Close();
                }
            }
            return stream.ToArray();
        }

        /// <summary>
        /// Saves a encrypted report template to the string.
        /// </summary>
        /// <returns>A string which contains a encrypted report template.</returns>
        public virtual string SaveEncryptedReportToString(string key)
        {
            var str = this.SaveToString();
            var bytes = Encoding.UTF8.GetBytes(str);
            bytes = StiEncryption.Encrypt(StiGZipHelper.Pack(bytes), key);

            var dest = new byte[bytes.Length + 3];
            dest[0] = (byte)'m';
            dest[1] = (byte)'r';
            dest[2] = (byte)'x';
            Array.Copy(bytes, 0, dest, 3, bytes.Length);

            return global::System.Convert.ToBase64String(dest);
        }

        /// <summary>
        /// Saves a packed report template in the stream.
        /// </summary>
        /// <param name="stream">A stream to save a packed report template.</param>
        public virtual StiReport SavePackedReport(Stream stream)
        {
            var service = new StiPackedReportSLService();
            StiOptions.Engine.GlobalEvents.InvokeReportSaving(this, EventArgs.Empty);
            Save(service, stream);
            StiOptions.Engine.GlobalEvents.InvokeReportSaved(this, EventArgs.Empty);
            ReportFile = string.Empty;
            IsModified = false;

            return this;
        }

        /// <summary>
        /// Saves a packed report template in the file.
        /// </summary>
        /// <param name="path">A file to save a packed report template.</param>
        public virtual StiReport SavePackedReport(string path)
        {
            FileStream stream = null;
            try
            {
                if (!this.IsDesigning || !StiOptions.Designer.ReadOnlyAlertOnSave)
                {
                    StiFileUtils.ProcessReadOnly(path);
                }
                stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                SavePackedReport(stream);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving packed report to file '" + path + "'...ERROR");
                StiLogService.Write(this.GetType(), e);

                if ((!StiOptions.Engine.HideExceptions) || this.IsDesigning) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            ReportFile = path;

            return this;
        }

        /// <summary>
        /// Saves a packed report template in the byte array.
        /// </summary>
        /// <returns>A new byte array containing the packed report template.</returns>
        public virtual byte[] SavePackedReportToByteArray()
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream();
                SavePackedReport(stream);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Flush();
                    stream.Close();
                }
            }
            return stream.ToArray();
        }

        /// <summary>
        /// Saves a packed report template to the string.
        /// </summary>
        /// <returns>A string which contains a packed report template.</returns>
        public virtual string SavePackedReportToString()
        {
            var str = this.SaveToString();
            return StiGZipHelper.Pack(str);
        }

        /// <summary>
        /// Saves a report template in the stream.
        /// </summary>
        /// <param name="stream">A stream for saving a report template.</param>
        public virtual StiReport Save(Stream stream)
        {
            StiReportSLService service;
            if (this.IsJsonReport)
                service = new StiJsonReportSLService();
            else
                service = new StiXmlReportSLService();

            Save(service, stream);
            ReportFile = string.Empty;
            IsModified = false;

            return this;
        }

        /// <summary>
        /// Saves a report template in the file using the provider.
        /// </summary>
        /// <param name="service">A provider which saves the report template.</param>
        /// <param name="stream">A stream for saving a report template.</param>
        public virtual StiReport Save(StiReportSLService service, Stream stream)
        {
            try
            {
                UpdateReportVersion();
                StiLogService.Write(typeof(StiReport), "Saving report");
                StiOptions.Engine.GlobalEvents.InvokeReportSaving(this, EventArgs.Empty);
                service.Save(this, stream);
                StiOptions.Engine.GlobalEvents.InvokeReportSaved(this, EventArgs.Empty);
                IsModified = false;
                ReportFile = string.Empty;

                IsJsonReport = service is StiJsonReportSLService;
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions || this.IsDesigning) throw;
            }
            return this;
        }

        /// <summary>
        /// Saves a report template in the file using the provider.
        /// </summary>
        /// <param name="service">A provider for saving a rendered report.</param>
        /// <param name="path">A file for saving the report template.</param>
        public virtual StiReport Save(StiReportSLService service, string path)
        {
            UpdateReportVersion();
            StiLogService.Write(typeof(StiReport), "Saving report");
            FileStream stream = null;

            try
            {
                if (!this.IsDesigning || !StiOptions.Designer.ReadOnlyAlertOnSave)
                {
                    StiFileUtils.ProcessReadOnly(path);
                }
                stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                service.Save(this, stream);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if ((!StiOptions.Engine.HideExceptions) || this.IsDesigning) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            ReportFile = path;
            IsModified = false;

            IsJsonReport = service is StiJsonReportSLService;

            return this;
        }

        /// <summary>
        /// Saves a report template in the file.
        /// </summary>
        /// <param name="path">A file to save a report template.</param>
        public virtual StiReport Save(string path)
        {
            FileStream stream = null;
            try
            {
                if (!this.IsDesigning || !StiOptions.Designer.ReadOnlyAlertOnSave)
                {
                    StiFileUtils.ProcessReadOnly(path);
                }
                stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                Save(stream);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving report to file '" + path + "'...ERROR");
                StiLogService.Write(this.GetType(), e);

                if ((!StiOptions.Engine.HideExceptions) || this.IsDesigning) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            ReportFile = path;

            return this;
        }

        /// <summary>
        /// Saves a report template with embedded data
        /// </summary>
        /// <param name="path">A file to save a report template with embedded data.</param>
        public virtual StiReport SaveSnapshot(string path)
        {
            FileStream stream = null;
            try
            {
                if (!this.IsDesigning || !StiOptions.Designer.ReadOnlyAlertOnSave)
                    StiFileUtils.ProcessReadOnly(path);

                StiDataResourceHelper.SaveSnapshot(this);

                stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                Save(stream);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving report to file '" + path + "'...ERROR");
                StiLogService.Write(this.GetType(), e);

                if ((!StiOptions.Engine.HideExceptions) || this.IsDesigning) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            ReportFile = path;

            return this;
        }

        /// <summary>
        ///Saves a report template with embedded data to the stream.
        /// </summary>
        /// <param name="stream">A stream to save a report template with embedded data.</param>
        public virtual StiReport SaveSnapshot(Stream stream)
        {
            try
            {
                StiDataResourceHelper.SaveSnapshot(this);
                Save(stream);
                ReportFile = string.Empty;
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving report template with embedded data ...ERROR");
                StiLogService.Write(this.GetType(), e);
            }
            return this;
        }

        /// <summary>
        ///Saves a report template with embedded data to a byte byte array.
        /// </summary>
        /// <returns>The new byte array containing a report snapshot.</returns>
        public virtual byte[] SaveSnapshotToByteArray()
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream();
                SaveSnapshot(stream);
            }
            finally
            {
                stream?.Flush();
                stream?.Close();
            }
            return stream?.ToArray();
        }

        /// <summary>
        /// Saves a report template in the file.
        /// </summary>
        /// <param name="path">A file to save a report template.</param>
        public virtual StiReport SaveToJson(string path)
        {
            FileStream stream = null;
            try
            {
                if (!this.IsDesigning || !StiOptions.Designer.ReadOnlyAlertOnSave)
                    StiFileUtils.ProcessReadOnly(path);

                stream = new FileStream(path, FileMode.Create, FileAccess.Write);

                var service = new StiJsonReportSLService();

                StiOptions.Engine.GlobalEvents.InvokeReportSaving(this, EventArgs.Empty);
                Save(service, stream);
                StiOptions.Engine.GlobalEvents.InvokeReportSaved(this, EventArgs.Empty);

                ReportFile = string.Empty;
                IsModified = false;
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving report to file '" + path + "'...ERROR");
                StiLogService.Write(this.GetType(), e);

                if ((!StiOptions.Engine.HideExceptions) || this.IsDesigning) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            ReportFile = path;

            return this;
        }

        /// <summary>
        /// Saves a report template in the byte array.
        /// </summary>
        /// <returns>A byte array which contains the report template.</returns>
        public virtual byte[] SaveToByteArray()
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream();
                Save(stream);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Flush();
                    stream.Close();
                }
            }

            return stream.ToArray();
        }

        /// <summary>
        /// Saves a report template to the string.
        /// </summary>
        /// <returns>A string which contains the report template.</returns>
        public virtual string SaveToString()
        {
            MemoryStream ms = null;
            StreamReader sr = null;
            string reportStr = null;
            try
            {
                ms = new MemoryStream();
                sr = new StreamReader(ms);

                this.Save(ms);
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                reportStr = sr.ReadToEnd();

            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving report to string");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions || this.IsDesigning) throw;
            }
            finally
            {
                if (sr != null) sr.Close();
                if (ms != null) ms.Close();
            }
            return reportStr;
        }
        #endregion

        #region Methods.Save document
        /// <summary>
        /// Saves a rendered report to the string.
        /// </summary>
        /// <returns>A string which contains the report template.</returns>
        public string SaveDocumentJsonToString()
        {
            return SaveToJsonInternal(StiJsonSaveMode.Document);
        }

        /// <summary>
        /// Saves a encrypted rendered report to the stream.
        /// </summary>
        /// <param name="stream">A stream to save a encrypted rendered report.</param>
        /// <param name="key">The key for encryption.</param>
        public virtual StiReport SaveEncryptedDocument(Stream stream, string key)
        {
            var service = new StiEncryptedDocumentSLService { Key = key };
            SaveDocument(service, stream);
            ReportFile = string.Empty;

            return this;
        }

        /// <summary>
        /// Saves a encrypted rendered report to the string.
        /// </summary>
        /// <returns>A string which contains a encrypted rendered report.</returns>
        public virtual string SaveEncryptedDocumentToString(string key)
        {
            var str = this.SaveDocumentToString();
            var bytes = Encoding.UTF8.GetBytes(str);
            bytes = StiEncryption.Encrypt(StiGZipHelper.Pack(bytes), key);

            var dest = new byte[bytes.Length + 3];
            dest[0] = (byte)'m';
            dest[1] = (byte)'d';
            dest[2] = (byte)'x';
            Array.Copy(bytes, 0, dest, 3, bytes.Length);

            return global::System.Convert.ToBase64String(dest);
        }

        /// <summary>
        /// Saves a encrypted rendered report to the byte array.
        /// </summary>
        /// <returns>A byte array which contains a encrypted rendered report.</returns>
        public virtual byte[] SaveEncryptedDocumentToByteArray(string key)
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream();
                SaveEncryptedDocument(stream, key);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Flush();
                    stream.Close();
                }
            }
            return stream.ToArray();
        }

        /// <summary>
        /// Saves a packed rendered report in the file.
        /// </summary>
        /// <param name="path">A file for saving a packed rendered report.</param>
        public virtual StiReport SaveEncryptedDocument(string path, string key)
        {
            FileStream stream = null;
            try
            {
                StiFileUtils.ProcessReadOnly(path);
                stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                SaveEncryptedDocument(stream, key);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving encrypted rendered report to file '" + path + "'...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            ReportFile = path;

            return this;
        }

        /// <summary>
        /// Saves a packed rendered report to the stream.
        /// </summary>
        /// <param name="stream">A stream to save a packed rendered report.</param>
        public virtual StiReport SavePackedDocument(Stream stream)
        {
            var service = new StiPackedDocumentSLService();
            SaveDocument(service, stream);
            ReportFile = string.Empty;

            return this;
        }

        /// <summary>
        /// Saves a packed rendered report to the string.
        /// </summary>
        /// <returns>A string which contains a packed rendered report.</returns>
        public virtual string SavePackedDocumentToString()
        {
            var str = this.SaveDocumentToString();
            return StiGZipHelper.Pack(str);
        }

        /// <summary>
        /// Saves a packed rendered report to the byte array.
        /// </summary>
        /// <returns>A byte array which contains a packed rendered report.</returns>
        public virtual byte[] SavePackedDocumentToByteArray()
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream();
                SavePackedDocument(stream);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Flush();
                    stream.Close();
                }
            }
            return stream.ToArray();
        }

        /// <summary>
        /// Saves a packed rendered report in the file.
        /// </summary>
        /// <param name="path">A file for saving a packed rendered report.</param>
        public virtual StiReport SavePackedDocument(string path)
        {
            FileStream stream = null;
            try
            {
                StiFileUtils.ProcessReadOnly(path);
                stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                SavePackedDocument(stream);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving packed rendered report to file '" + path + "'...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            ReportFile = path;

            return this;
        }

        /// <summary>
        /// Saves a rendered report in the stream.
        /// </summary>
        /// <param name="stream">A stream to save a rendered report.</param>
        public virtual StiReport SaveDocument(Stream stream)
        {
            var service = new StiXmlDocumentSLService();
            SaveDocument(service, stream);
            ReportFile = string.Empty;

            return this;
        }

        /// <summary>
        /// Saves a rendered report to the string.
        /// </summary>
        /// <returns>A string which contains the report template.</returns>
        public virtual string SaveDocumentToString()
        {
            MemoryStream ms = null;
            StreamReader sr = null;
            string reportStr = null;
            try
            {
                ms = new MemoryStream();
                sr = new StreamReader(ms);
                this.SaveDocument(ms);
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                reportStr = sr.ReadToEnd();
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving rendered report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if ((!StiOptions.Engine.HideExceptions) || this.IsDesigning) throw;
            }
            finally
            {
                sr.Close();
                ms.Close();
            }
            return reportStr;
        }

        /// <summary>
        /// Saves a rendered report to the byte array.
        /// </summary>
        /// <returns>Returns a byte array which contains a rendered report.</returns>
        public virtual byte[] SaveDocumentToByteArray()
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream();
                SaveDocument(stream);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Flush();
                    stream.Close();
                }
            }

            return stream.ToArray();
        }

        /// <summary>
        /// Saves a rendered report to the stream.
        /// </summary>
        /// <param name="service">A provider which saves a rendered report.</param>
        /// <param name="stream">A stream to save a rendered report.</param>
        public virtual StiReport SaveDocument(StiDocumentSLService service, Stream stream)
        {
            try
            {
                StiLogService.Write(typeof(StiReport), "Saving rendered report");
                service.Save(this, stream);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving rendered report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }

            return this;

        }

        /// <summary>
        /// Saves a rendered report using the provider in the file.
        /// </summary>
        /// <param name="service">A provider that saves a rendered report.</param>
        /// <param name="path">A file to save a rendered report.</param>
        public virtual StiReport SaveDocument(StiDocumentSLService service, string path)
        {
            StiLogService.Write(typeof(StiReport), "Saving rendered report");
            FileStream stream = null;
            try
            {
                StiFileUtils.ProcessReadOnly(path);
                stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                service.Save(this, stream);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving rendered report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            ReportFile = path;

            return this;
        }

        /// <summary>
        /// Saves a rendered report in the file.
        /// </summary>
        /// <param name="path">A file to save a rendered report.</param>
        public virtual StiReport SaveDocument(string path)
        {
            FileStream stream = null;
            try
            {
                StiFileUtils.ProcessReadOnly(path);
                stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                SaveDocument(stream);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving rendered report to file '" + path + "'...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            ReportFile = path;

            return this;
        }
        #endregion
    }
}