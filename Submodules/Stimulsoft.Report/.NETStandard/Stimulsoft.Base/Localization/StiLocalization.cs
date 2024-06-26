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
using System.Globalization;
using System.IO;
using System.Xml;
using System.Collections;
using System.Reflection;
using Stimulsoft.Base.Json;
using System.Linq;

namespace Stimulsoft.Base.Localization
{          
	public class StiLocalization
	{
		#region Fields
		private static Hashtable categories;
        private static bool isLocalizationLoaded;
        private static string prevLocalization = string.Empty;
	    private static object lockObject = new object();
        #endregion

        #region Properties
        public static bool SearchLocalizationFromRegistry { get; set; } = true;

		/// <summary>
		/// Gets or sets string that containing path to directory in which are located files with localized resource.
		/// </summary>
		public static string DirectoryLocalization { get; set; } = "Localization";
		
		/// <summary>
		/// Gets or sets name of file with localized resource.
		/// </summary>
		public static string Localization { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets name of the language  of the localization.
		/// </summary>
		public static string Language { get; set; }

		/// <summary>
		/// Gets or sets description of the localization.
		/// </summary>
		public static string Description { get; set; }

		/// <summary>
		/// Gets or sets the name of the culture of the localization.
		/// </summary>
		public static string CultureName { get; set; }

        public static bool IsEn => CultureName.ToLowerInvariant() == "en" || 
            CultureName.ToLowerInvariant() == "en-gb";

		public static bool IsDe => CultureName.ToLowerInvariant() == "de";

		public static bool IsRu => CultureName.ToLowerInvariant() == "ru";

		public static bool IsRussian => CultureName.ToLowerInvariant() == "ru" || 
            CultureName.ToLowerInvariant() == "be" || 
            CultureName.ToLowerInvariant() == "uk";

        public static bool IsCyrillic => StiLocalization.IsRussian ||
            StiLocalization.CultureName?.ToLowerInvariant() == "bg";

        /// <summary>
        /// Gets or sets the name of the culture of the localization.
        /// </summary>
        public static bool BlockLocalizationExceptions { get; set; }

        public static bool BlockLocalizationLoading { get; set; }
        #endregion

        #region Methods
        public static string GetDirectoryLocalizationFromRegistry()
        {
            if (!SearchLocalizationFromRegistry)
                return null;

            try
            {
                var path = Registry.GetValue("Localization");
                return path != null && Directory.Exists(path) ? path : null;
            }
            catch
            {
            }

            return null;
        }

	    public static string GetDirectoryLocales()
	    {
	        try
	        {
	            var assembly = Assembly.GetEntryAssembly();
	            if (assembly == null)
	                assembly = Assembly.GetExecutingAssembly();

	            var entryFolder = Path.GetDirectoryName(assembly.Location);
	            var localesFolder = Path.Combine(entryFolder, "locales");

				if (Directory.Exists(localesFolder))
				{
					var xmlFiles = Directory.EnumerateFiles(localesFolder, "*.xml").ToArray();

					if (xmlFiles.Length > 0)
                        return localesFolder;
                }
	        }
	        catch
	        {
	        }

            return null;
        }

	    public static string GetEnumValue(string key)
        {
            if (categories == null)
                LoadDefaultLocalization();

            if (categories == null)
                return null;

            var categoryHash = categories["PropertyEnum"] as Hashtable;
            if (categoryHash == null)
                return null;

            var value = categoryHash[key] as string;
            if (value == null)
                return key.EndsWith("Category") ? key.Substring(0, key.Length - 8) : null;

            return value;
        }

		public static void LoadDefaultLocalization()
		{
			Stream stream = null;

			try
			{
				stream = typeof(StiLocalization).Assembly.GetManifestResourceStream("Stimulsoft.Base.Localization.en.xml");
                if (stream == null)
                    throw new Exception("Resourse 'Stimulsoft.Base.Localization.en.xml' is not found!");

				LoadInternal(stream);
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

        private static DirectoryInfo GetDirectoryInfoFromRegister()
        {
            if (!SearchLocalizationFromRegistry)
                return null;

            var dirPath = Registry.GetValue("Localization");
            return dirPath != null && Directory.Exists(dirPath) 
                ? new DirectoryInfo(dirPath) 
                : null;
        }

	    private static string GetDirectoryLocalization()
	    {
	        if (Directory.Exists(DirectoryLocalization))
	            return DirectoryLocalization;

            if (Directory.Exists("locales"))
                return "locales";

            var dir = GetDirectoryInfoFromRegister();
            return dir != null && dir.Exists ? dir.Name : null;
	    }

        private static void Check()
        {
            if (isLocalizationLoaded && prevLocalization == Localization)
                return;

            isLocalizationLoaded = true;
            prevLocalization = Localization;
            LoadCurrentLocalization();
        }

		/// <summary>
		/// Loads the current localization.
		/// </summary>
		public static void LoadCurrentLocalization()
		{
            if (BlockLocalizationLoading) return;

		    var locDir = GetDirectoryLocalization();

			var di = Directory.Exists(locDir) 
                ? new DirectoryInfo(locDir)
                : null;

			if (di != null && di.Exists)
			{
				var files = di.GetFiles()
                    .Where(f => IsXmlExtension(f.FullName))
                    .ToArray();
			
				if (!string.IsNullOrWhiteSpace(Localization))
				{
				    var locFile = files.FirstOrDefault(f => f.Name.ToLowerInvariant() == Localization.ToLowerInvariant());
				    if (locFile != null)
				    {
                        Load(locFile.FullName);
                        return;
                    }
				}
				else
				{
					var cultureName = CultureInfo.CurrentCulture.Name;
					var cultureISOName = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

                    foreach (var file in files)
					{
						var fileName = Path.GetFileNameWithoutExtension(file.Name);
						if (fileName == cultureName || fileName == cultureISOName)
						{
							Load(file.FullName);
							return;
						}
					}
				}
			}
            LoadDefaultLocalization();
		}

		/// <summary>
		/// Loads a localization from the file.
		/// </summary>
		/// <param name="file">The name of the file to load a localization.</param>
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

		/// <summary>
		/// Loads a localization from the stream.
		/// </summary>
		public static void Load(Stream stream)
		{
            if (BlockLocalizationLoading) return;

			if (categories != null)
			{
				categories.Clear();
				categories = null;
			}

			LoadDefaultLocalization();
			LoadInternal(stream);
		}

		private static void LoadInternal(Stream stream)
		{
            if (BlockLocalizationLoading) return;

			var tr = new XmlTextReader(stream);
			tr.Read();
			tr.Read();
			tr.Read();

			if (tr.Name == "Localization")
			{
				Language = tr.GetAttribute("language");
				Description = tr.GetAttribute("description");
				CultureName = tr.GetAttribute("cultureName");

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

		/// <summary>
		/// Loads localization parameters from the file.
		/// </summary>
		/// <param name="file">The name of the file of a localization.</param>
		public static void GetParam(string file, out string language, out string description, out string cultureName, string folder = null)
		{
		    if (folder != null && Directory.Exists(folder))
		        file = Path.Combine(folder, file);
            
		    if (File.Exists(file) && IsXmlExtension(file))
		    {
		        var stream = new FileStream(file, FileMode.Open, FileAccess.Read);

		        GetParam(stream, out language, out description, out cultureName);
		        
		        stream.Close();
		        stream.Dispose();
		    }
		    else
		    {
		        language = null;
		        description = null;
		        cultureName = null;
		    }
		}

	    /// <summary>
	    /// Loads localization parameters from the file.
	    /// </summary>
	    public static void GetParam(Stream stream, out string language, out string description, out string cultureName)
	    {
	        var tr = new XmlTextReader(stream);

            tr.Read();
	        tr.Read();
	        tr.Read();

	        language = tr.GetAttribute("language");
	        description = tr.GetAttribute("description");
	        cultureName = tr.GetAttribute("cultureName");
	    }

	    private static bool IsXmlExtension(string file)
	    {
            var ext = Path.GetExtension(file);
	        return !string.IsNullOrEmpty(ext) && ext.ToLowerInvariant() == ".xml";
	    }

		public static void Add(string category, string key, string value)
		{
            Check();

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

	    public static string GetCleaned(string category, string key)
	    {
	        return Loc.GetCleaned(category, key);
	    }

        public static string GetValue(string category, string key)
		{
			return Get(category, key);
		}

		public static string Get(string category, string key, bool throwError)
		{
            Check();

            var baseThrowError = throwError;
            if (BlockLocalizationExceptions)
                throwError = false;
   
			if (categories == null)
			    LoadDefaultLocalization();

			if (categories == null)
			{
				if (throwError)
				    throw new Exception("Localization file is not loaded!");

				return null;
			}

			var categoryHash = categories[category] as Hashtable;
			if (categoryHash == null)
			{
                if (baseThrowError)
                {
                    LoadDefaultLocalization();
                    categoryHash = categories[category] as Hashtable;
                }

			    if (categoryHash == null)
                {
                    if (throwError)
                        throw new Exception($"Category '{category}' is not found!");

                    return null;
                }
			}

			var value = categoryHash[key] as string;
            if (value == null)
            {
                if (key.EndsWith("Category"))
                    return key.Substring(0, key.Length - 8);

                if (baseThrowError)
                {
                    LoadDefaultLocalization();
                    value = categoryHash[key] as string;
                }

                if (value == null)
                {
                    if (throwError)
                        throw new Exception($"Key '{key}' is not found!");

                    return null;
                }
            }
			
			return value;
		}

        public static void Set(string category, string key, string value)
        {
            Check();

            if (categories == null) LoadDefaultLocalization();
            if (categories == null)
                throw new Exception("Localization file is not loaded!");
            
            var categoryHash = categories[category] as Hashtable;
            if (categoryHash == null)
                throw new Exception($"Category '{category}' is not found!");

            lock (lockObject)
            {
                categoryHash[key] = value;
            }
        }

		public static string[] GetKeys(string category)
		{
            Check();

		    if (categories == null)
		        throw new Exception("Localization file is not loaded!");

		    var categoryHash = categories[category] as Hashtable;
			if (categoryHash == null)
				throw new Exception($"Category '{category}' is not found!");

            lock (lockObject)
            {
                var keys = new string[categoryHash.Keys.Count];
                categoryHash.Keys.CopyTo(keys, 0);

                Array.Sort(keys);

                return keys;
            }
		}

		public static string[] GetValues(string category)
		{
            Check();

		    if (categories == null)
		        throw new Exception("Localization file is not loaded!");

		    var categoryHash = categories[category] as Hashtable;
			if (categoryHash == null)
				throw new Exception($"Category '{category}' is not found!");

            lock (lockObject)
            {
                var values = new string[categoryHash.Values.Count];
                categoryHash.Values.CopyTo(values, 0);

                Array.Sort(values);

                return values;
            }
		}

        public static string[] GetCategories()
        {
            Check();

            if (categories == null)
                throw new Exception("Localization file is not loaded!");

            var keys = new string[categories.Count];
            categories.Keys.CopyTo(keys, 0);

            return keys;
        }

        public static string GetLocalization(bool format)
        {
            if (categories == null)
                LoadDefaultLocalization();

            return JsonConvert.SerializeObject(categories, format ? Stimulsoft.Base.Json.Formatting.Indented : Stimulsoft.Base.Json.Formatting.None);
        }
		#endregion
	}
}