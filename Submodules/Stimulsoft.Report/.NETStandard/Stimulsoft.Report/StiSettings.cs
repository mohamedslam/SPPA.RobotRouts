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
using Stimulsoft.Base.Drawing;
using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace Stimulsoft.Report
{
    public sealed class StiSettings
    {
        #region Properties
        private static string ConfigFolder => "Stimulsoft";
        
        private static string ConfigFile => "Stimulsoft.Report.settings";
        #endregion

        #region Methods
        public static string GetStr(string key, string subkey)
        {
            return GetStr(key, subkey, string.Empty);
        }

        public static string GetStr(string key, string subkey, string nullValue)
        {
            object value = Get(key, subkey);

            if (value == null || (!(value is string)))
                return nullValue;

            return value as string;
        }

        public static bool GetBool(string key, string subkey)
        {
            return GetBool(key, subkey, false);
        }

        public static bool GetBool(string key, string subkey, bool nullValue)
        {
            object value = Get(key, subkey);

            if (value == null || (!(value is bool)))
                return nullValue;

            return (bool)value;
        }

        public static int GetInt(string key, string subkey)
        {
            return GetInt(key, subkey, -1);
        }

        public static int GetInt(string key, string subkey, int nullValue, bool isZoom = false)
        {
            object value = Get(key, subkey);

            if (value == null || (!(value is int)))
                return isZoom ? (int)Math.Ceiling(StiScale.Factor * nullValue) : nullValue;

            return isZoom ? (int)Math.Ceiling(StiScale.Factor * (int)value) : (int)value;
        }

        public static Color GetColor(string key, string subkey, Color nullValue)
        {
            object valueA = Get(key, subkey + ".A");
            object valueR = Get(key, subkey + ".R");
            object valueG = Get(key, subkey + ".G");
            object valueB = Get(key, subkey + ".B");

            if (valueA == null || (!(valueA is byte)))
                return nullValue;

            if (valueR == null || (!(valueR is byte)))
                return nullValue;

            if (valueG == null || (!(valueG is byte)))
                return nullValue;

            if (valueB == null || (!(valueB is byte)))
                return nullValue;

            return Color.FromArgb((byte)valueA, (byte)valueR, (byte)valueG, (byte)valueB);
        }

        public static float GetFloat(string key, string subkey)
        {
            return GetFloat(key, subkey, 0);
        }

        public static float GetFloat(string key, string subkey, float nullValue)
        {
            object value = Get(key, subkey);

            if (value == null || (!(value is float)))
                return nullValue;

            return (float)value;
        }

        public static double GetDouble(string key, string subkey)
        {
            return GetDouble(key, subkey, 0);
        }

        public static double GetDouble(string key, string subkey, double nullValue)
        {
            object value = Get(key, subkey);

            if (value == null || (!(value is double)))
                return nullValue;

            return (double)value;
        }

        public static decimal GetDecimal(string key, string subkey)
        {
            return GetDecimal(key, subkey, 0);
        }

        public static decimal GetDecimal(string key, string subkey, decimal nullValue)
        {
            object value = Get(key, subkey);

            if (value == null || (!(value is decimal)))
                return nullValue;

            return (decimal)value;
        }

        public static object Get(string key, string subkey)
        {
            return Get(key, subkey, null);
        }

        public static Hashtable GetCategory(string key)
        {
            var user = settings[Environment.UserName] as Hashtable;
            if (user == null)
                return null;

            return user[key] as Hashtable;
        }

        public static object Get(string key, string subkey, object nullValue)
        {
            Hashtable user = settings[Environment.UserName] as Hashtable;
            if (user == null)
                return nullValue;

            Hashtable category = user[key] as Hashtable;
            if (category == null)
                return nullValue;

            object value = category[subkey];
            if (value == null)
                return nullValue;

            return value;
        }

        public static void Set(string key, string subkey, int value, bool isZoom = false)
        {
            Set(key, subkey, (object)(isZoom ? (int)(value / StiScale.Factor) : value));
        }
        public static void Set(string key, string subkey, object value)
        {
            Hashtable user = settings[Environment.UserName] as Hashtable;
            if (user == null)
            {
                user = new Hashtable();
                settings[Environment.UserName] = user;
            }

            Hashtable category = user[key] as Hashtable;
            if (category == null)
            {
                category = new Hashtable();
                user[key] = category;
            }

            if (value is Color)
            {
                Color color = (Color)value;

                category[subkey + ".A"] = color.A;
                category[subkey + ".R"] = color.R;
                category[subkey + ".G"] = color.G;
                category[subkey + ".B"] = color.B;
            }
            else
            {
                category[subkey] = value;
            }
        }

        public static void ClearKey(string key)
        {
            Hashtable user = settings[Environment.UserName] as Hashtable;
            if (user != null && user.ContainsKey(key))
                user.Remove(key);
        }

        public static void ClearKey(string key, string subkey)
        {
            Hashtable user = settings[Environment.UserName] as Hashtable;
            if (user == null)
                return;

            Hashtable category = user[key] as Hashtable;
            if (category == null)
                return;

            if (category.ContainsKey(subkey))
                category.Remove(subkey);
        }

        public static void Clear()
        {
            StiSettings.settings.Clear();
        }

        public static void Load()
        {
            var path = StiPath.GetPath(GetDefaultReportSettingsPath());
            var fileExists = File.Exists(path);
            var fileAccessTime = fileExists ? new FileInfo(path).LastWriteTimeUtc : (DateTime?)null;

            if (lastAccessTime == null || lastAccessTime != fileAccessTime)
            {
                if (fileExists)
                    Load(path);

                else
                    Clear();

                lastAccessTime = fileAccessTime;
            }
        }

        public static void ReLoad()
        {
            Clear();

            string path = StiPath.GetPath(GetDefaultReportSettingsPath());
            if (File.Exists(path))
                Load(path);
        }


        public static void Load(string file)
        {
            Monitor.Enter(sync);

            try
            {
                if (File.Exists(file))
                {
                    var buffer = File.ReadAllBytes(file);
                    using (var stream = new MemoryStream(buffer))
                    {
                        Load(stream);
                    }
                }
            }
            catch
            {
            }
            finally
            {
                Monitor.Exit(sync);
            }
        }

        public static void Load(Stream stream)
        {
            Clear();
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                XmlTextReader tr = new XmlTextReader(stream);
                tr.DtdProcessing = DtdProcessing.Ignore;
                tr.Read();
                tr.Read();

                StiReportObjectStringConverter converter = new StiReportObjectStringConverter();

                if (tr.IsStartElement())
                {
                    if (tr.Name == "StiSerializer")
                    {
                        string app = tr.GetAttribute("application");
                        if (app == "StiReportSettings")
                        {
                            Hashtable latestUser = null;
                            Hashtable latestCategory = null;

                            tr.Read();
                            while (tr.Read())
                            {
                                if (tr.IsStartElement())
                                {
                                    #region User
                                    if (tr.Name == "User")
                                    {
                                        string userName = tr.GetAttribute("Name");
                                        Hashtable user = settings[userName] as Hashtable;
                                        if (user == null)
                                        {
                                            user = new Hashtable();
                                            settings[userName] = user;
                                        }
                                        latestUser = user;
                                    }
                                    #endregion

                                    #region Category
                                    else if (tr.Name == "Category")
                                    {
                                        string name = tr.GetAttribute("Name");
                                        Hashtable category = latestUser[name] as Hashtable;
                                        if (category == null)
                                        {
                                            category = new Hashtable();
                                            latestUser[name] = category;
                                        }
                                        latestCategory = category;
                                    }
                                    #endregion

                                    #region Item
                                    else if (tr.Name == "Item")
                                    {
                                        string key = tr.GetAttribute("Key");
                                        string valueStr = tr.GetAttribute("Value");
                                        string typeStr = tr.GetAttribute("Type");

                                        Type type = StiTypeFinder.GetType(typeStr);

                                        if (type != null)
                                        {
                                            object value = converter.StringToObject(valueStr, type);

                                            if (value != null)
                                                latestCategory[key] = value;
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                StiLogService.Write(typeof(StiReport), "Loading settings...ERROR");
                StiLogService.Write(typeof(StiReport), e);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        public static void Save()
        {
            Save(StiPath.GetPath(GetDefaultReportSettingsPath()));
        }

        public static void Save(string file)
        {
            int index = 0;

            while (true)
            {
                bool final = false;
                Monitor.Enter(sync);
                try
                {
                    StiFileUtils.ProcessReadOnly(file);
                    var directory = Path.GetDirectoryName(file);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    using (var stream = new MemoryStream())
                    {
                        Save(stream);
                        stream.Flush();

                        var buffer = stream.ToArray();
                        File.WriteAllBytes(file, buffer);
                    }
                }
                catch
                {
                    final = true;
                }
                finally
                {
                    Monitor.Exit(sync);
                }

                if (!final || index > 4)
                    break;

                index++;
            }
        }

        public static void Save(Stream stream)
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                XmlTextWriter tw = new XmlTextWriter(stream, Encoding.UTF8);

                tw.Formatting = Formatting.Indented;
                tw.WriteStartDocument(true);

                tw.WriteStartElement("StiSerializer");
                tw.WriteAttributeString("version", "1.0");
                tw.WriteAttributeString("application", "StiReportSettings");

                ICollection users = settings.Keys;
                foreach (string user in users)
                {
                    #region Write User
                    Hashtable usersTable = settings[user] as Hashtable;
                    if (usersTable != null && usersTable.Count > 0)
                    {
                        tw.WriteStartElement("User");
                        tw.WriteAttributeString("Name", user);

                        ICollection categories = usersTable.Keys;
                        foreach (string category in categories)
                        {
                            #region Write Category
                            Hashtable values = usersTable[category] as Hashtable;
                            if (values != null && values.Count > 0)
                            {
                                tw.WriteStartElement("Category");
                                tw.WriteAttributeString("Name", category);

                                ICollection keys = values.Keys;
                                foreach (string key in keys)
                                {
                                    #region Write item
                                    object value = values[key];
                                    if (value != null)
                                    {
                                        tw.WriteStartElement("Item");
                                        tw.WriteAttributeString("Key", key);
                                        tw.WriteAttributeString("Value", value.ToString());
                                        tw.WriteAttributeString("Type", value.GetType().ToString());
                                        tw.WriteEndElement();
                                    }
                                    #endregion
                                }

                                tw.WriteEndElement();
                            }
                            #endregion
                        }
                        tw.WriteEndElement();
                    }
                    #endregion
                }

                tw.WriteEndElement();
                tw.Flush();
                tw.Close();
            }
            catch (Exception e)
            {
                StiLogService.Write(typeof(StiReport), "Saving settings...ERROR");
                StiLogService.Write(typeof(StiReport), e);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        public static string GetDefaultReportSettingsPath()
        {
            string path = DefaultReportSettingsPath;
            if (string.IsNullOrEmpty(path))
            {
                path = Path.Combine(ConfigFolder, ConfigFile);

                string folder;
                try
                {
                    if (StiOptions.Engine.FullTrust)
                    {
                        folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        if (folder.Length != 0)
                            path = Path.Combine(folder, path);
                    }

                    folder = Path.GetDirectoryName(path);
                    if (StiOptions.Engine.FullTrust && !string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                }
                catch
                {
                    try
                    {
                        path = Path.Combine(ConfigFolder, ConfigFile);
                        folder = ConfigFolder;

                        if (StiOptions.Engine.FullTrust && !Directory.Exists(folder))
                            Directory.CreateDirectory(folder);
                    }
                    catch
                    {
                        path = ConfigFile;
                    }
                }
            }
            return path;
        }

        public static string DefaultReportSettingsPath
        {
            get
            {
                return StiOptions.Configuration.DefaultReportSettingsPath;
            }
            set
            {
                StiOptions.Configuration.DefaultReportSettingsPath = value;
            }
        }
        #endregion

        #region Fields
        private static DateTime? lastAccessTime;
        private static readonly object sync = new object();
        private static Hashtable settings = new Hashtable();
        #endregion
    }
}
