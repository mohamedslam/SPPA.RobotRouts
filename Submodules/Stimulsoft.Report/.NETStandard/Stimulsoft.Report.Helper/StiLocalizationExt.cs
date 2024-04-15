#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using System.Xml;
using System.Collections;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Helper
{
    public class StiLocalizationExt
    {
        #region Fields
        private static Hashtable categories;
        private static object lockObject = new object();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets name of file with localized resource.
        /// </summary>
        public static string Localization { get; set; } = string.Empty;

        public static bool BlockLocalizationLoading { get; set; }
        #endregion

        #region Methods
        public static void Load()
        {
            if (BlockLocalizationLoading) return;

            Stream stream = null;
            try
            {
                LoadDefaultLocalization();

                var loc = Localization;
                var index = loc.LastIndexOf(".", StringComparison.Ordinal);

                if (index > 0)
                {
                    var fileName = loc.Insert(index, ".ext");
                    
                    if (fileName == "ua.ext.xml" || fileName == "be.ext.xml")
                        fileName = "ru.ext.xml";

                    if (fileName.ToLowerInvariant() == "en-gb.ext.xml")
                        fileName = "en.ext.xml";

                    if (fileName != "en.ext.xml")
                    {
                        stream = typeof(StiLocalizationExt).Assembly.GetManifestResourceStream($"Stimulsoft.Report.Helper.{fileName}");
                        if (stream == null)
                            Localization = "en.xml";
                    }
                }

                if (stream != null)
                    Load(stream);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }

        public static void Load(string file)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(file, FileMode.Open, FileAccess.Read);
                Load(stream);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }

        private static void LoadDefaultLocalization()
        {
            Stream stream = null;
            try
            {
                stream = typeof(StiLocalizationExt).Assembly.GetManifestResourceStream("Stimulsoft.Report.Helper.en.ext.xml");
                if (stream == null)
                    throw new Exception("The resourse \'Stimulsoft.Report.Helper.en.ext.xml\' is not founded!");

                if (categories != null)
                    categories.Clear();

                Load(stream);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }

        public static void Load(Stream stream)
        {
            if (BlockLocalizationLoading) return;

            var tr = new XmlTextReader(stream);

            tr.Read();
            tr.Read();
            tr.Read();
            if (tr.Name == "Localization")
            {
                var category = string.Empty;

                while (tr.Read())
                {
                    if (tr.IsStartElement())
                    {
                        var name = tr.Name;
                        if (tr.Depth == 1)
                            category = name;

                        else
                            Add(category, name, tr.ReadString());
                    }
                    tr.Read();
                }
            }
        }

        private static void Add(string category, string key, string value)
        {
            lock (lockObject)
            {
                if (categories == null)
                    categories = new Hashtable();

                var categoryHash = categories[category] as Hashtable;
                if (categoryHash == null)
                {
                    categoryHash = new Hashtable();
                    categories[category] = categoryHash;
                }

                categoryHash[key] = value;
            }
        }

        public static string Get(string category, string key)
        {
            return Get(category, key, true);
        }

        public static string GetValue(string category, string key)
        {
            return Get(category, key);
        }

        public static string Get(string category, string key, bool throwError)
        {
            lock (lockObject)
            {
                if ($"{StiLocalization.CultureName}.xml" != Localization)
                {
                    Localization = $"{StiLocalization.CultureName}.xml";
                    Load();
                }

                if (categories == null)
                    Load();

                if (categories == null)
                {
                    if (throwError)
                        throw new Exception("The extended localization file is not loaded!");

                    return null;
                }

                var categoryHash = categories[category] as Hashtable;
                if (categoryHash == null)
                {
                    categoryHash = categories["Universal"] as Hashtable;
                    var value2 = categoryHash[key] as string;
                    if (value2 != null)
                        return value2;

                    if (throwError)
                        throw new Exception($"Category '{category}' is not found!");

                    return null;
                }

                var value = categoryHash[key] as string;
                if (value == null)
                {
                    if (key.EndsWith("Category"))
                        return key.Substring(0, key.Length - 8);

                    #region Try find description in Universal Key List
                    categoryHash = categories["Universal"] as Hashtable;
                    value = categoryHash[key] as string;

                    if (value != null)
                        return value;
                    #endregion

                    if (throwError)
                        throw new Exception($"Key '{key}' is not found!");

                    return null;
                }

                return value;
            }
        }
        #endregion
    }
}