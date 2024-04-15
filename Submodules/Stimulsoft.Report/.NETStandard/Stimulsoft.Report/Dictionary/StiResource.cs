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
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Dictionary.Design;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Drawing.Design;
using Stimulsoft.Base.Design;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

#if NETSTANDARD
using UITypeEditor = Stimulsoft.System.Drawing.Design.UITypeEditor;
#endif

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// This class describes resource which embedded to the report file.
    /// </summary>
    [TypeConverter(typeof(StiResourceFileConverter))]
	[StiSerializable]
	public class StiResource :
		IStiName,
        IStiAlias,
        IStiAppCell,
        ICloneable,
        IStiInherited,
        IStiPropertyGridObject,
        IStiJsonReportObject
    {
        #region enum Order
        public enum Order
        {
            Name = 100,
            Alias = 200,
            Category = 300,
            AvailableInTheViewer = 400
        }
        #endregion

        #region ICloneable
        public object Clone()
        {
            var resource = MemberwiseClone() as StiResource;

            resource.Content = Content?.Clone() as byte[];

            return resource;
        }
        #endregion

        #region IStiAppCell
        string IStiAppCell.GetKey()
        {
            Key = StiKeyHelper.GetOrGeneratedKey(Key);

            return Key;
        }

        void IStiAppCell.SetKey(string key)
        {
            Key = key;
        }
        #endregion

        #region IStiJsonReportObject
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyStringNullOrEmpty("Name", Name);
            jObject.AddPropertyStringNullOrEmpty("Alias", Alias);
            jObject.AddPropertyStringNullOrEmpty("Key", Key);
            jObject.AddPropertyBool("Inherited", Inherited);
            jObject.AddPropertyEnum("Type", Type, StiResourceType.Image);
            jObject.AddPropertyBool("AvailableInTheViewer", AvailableInTheViewer);
            jObject.Add("Image", StiPacker.PackAndEncryptToString(Content));

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Name":
                        this.Name = property.DeserializeString();
                        break;

                    case "Alias":
                        this.Alias = property.DeserializeString();
                        break;

                    case "Key":
                        this.Key = property.DeserializeString();
                        break;

                    case "Inherited":
                        var obj = property.Value.ToObject<bool?>();
                        this.Inherited = obj != null && (bool)obj; ;
                        break;

                    case "Type":
                        this.Type = property.DeserializeEnum<StiResourceType>();
                        break;

                    case "AvailableInTheViewer":
                        this.AvailableInTheViewer = property.DeserializeBool();
                        break;

                    case "Image":
                        this.Content = StiPacker.UnpackAndDecrypt(property.DeserializeString());
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public StiComponentId ComponentId => StiComponentId.StiResource;

        [Browsable(false)]
        public string PropName => this.Name;

        public StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // DataCategory
            var list = new[]
            {
                propHelper.Name(),
                propHelper.Alias(),
                propHelper.AvailableinTheViewer()
            };
            objHelper.Add(StiPropertyCategories.Data, list);
            
            return objHelper;
        }

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }
        #endregion

        #region IStiInherited
        [Browsable(false)]
		[DefaultValue(false)]
		public bool Inherited { get; set; }
        #endregion

        #region IStiName
        private string name;
        /// <summary>
        /// Gets or sets a name of the resource.
		/// </summary>
		[StiCategory("Data")]
		[StiSerializable]
		[ParenthesizePropertyName(true)]
        [StiOrder((int)Order.Name)]
        [Description("Gets or sets a name of the resource.")]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name == value) return;

                if (name == Alias)
                    Alias = value;

                name = value;
            }
        }
        #endregion

        #region IStiAlias
        /// <summary>
        /// Gets or sets an alias of the resource.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [ParenthesizePropertyName(true)]
        [StiOrder((int)Order.Alias)]
        [Description("Gets or sets an alias of the resource.")]
        public string Alias { get; set; }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets value which makes the resource available for download in the viewer.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [DefaultValue(false)]
        [StiOrder((int)Order.AvailableInTheViewer)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which makes the resource available for download in the viewer.")]
        public bool AvailableInTheViewer { get; set; }

        [Browsable(false)]
        public byte[] Content { get; set; }

        /// <summary>
        /// Gets or sets the key of the dictionary object.
        /// </summary>
        [DefaultValue(null)]
        [StiSerializable]
        [Browsable(false)]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the type of the resource.
        /// </summary>
        [DefaultValue(StiResourceType.Image)]
        [StiSerializable]
        [Browsable(false)]
        public StiResourceType Type { get; set; }
        #endregion

        #region Methods
        public Image GetResourceAsImage()
        {
            if (Content == null || this.Type != StiResourceType.Image) return null;

            try
            {
                var stream = new MemoryStream(Content);
                return Image.FromStream(stream);
                
            }
            catch
            {
            }
            return null;
        }

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

        public string GetContentType()
        {
            switch (Type)
            {
                case StiResourceType.ReportSnapshot:
                case StiResourceType.Report:
                    return "application/octet-stream";

                case StiResourceType.Pdf:
                    return "application/pdf";

                case StiResourceType.Txt:
                    return "text/plain";

                case StiResourceType.Json:
                    return "text/plain";

                case StiResourceType.Rtf:
                    return "application/rtf";

                case StiResourceType.Word:
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                case StiResourceType.Excel:
                    return "application/vnd.ms-excel";

                case StiResourceType.Csv:
                    return "text/csv";

                case StiResourceType.Dbf:
                    return "application/dbf";


                case StiResourceType.Image:
                {
                    if (StiImageHelper.IsPng(Content))
                        return "image/png";

                    else if (StiImageHelper.IsMetafile(Content))
                        return "image/x-wmf";

                    else if (StiImageHelper.IsBmp(Content))
                        return "image/bmp";

                    else if (StiImageHelper.IsJpeg(Content))
                        return "image/jpeg";

                    else if (StiImageHelper.IsGif(Content))
                        return "image/gif";

                    else if (StiSvgHelper.IsSvg(Content))
                        return "image/svg+xml";

                    else if (StiImageHelper.IsTiff(Content))
                        return "image/tiff";

                    else if (StiImageHelper.IsEmf(Content))
                        return "image/x-emf";

                    return "image/png";
                }
            }
            return "text/plain";
        }

        public string GetFileExt()
        {
            switch (Type)
            {
                case StiResourceType.ReportSnapshot:
                    return ".mdc";

                case StiResourceType.Report:
                    return ".mrt";

                case StiResourceType.Pdf:
                    return ".pdf";

                case StiResourceType.Txt:
                    return ".txt";

                case StiResourceType.Rtf:
                    return ".rtf";

                case StiResourceType.Word:
                    return ".docx";

                case StiResourceType.Excel:
                    return ".xlsx";

                case StiResourceType.Csv:
                    return ".csv";

                case StiResourceType.Dbf:
                    return ".dbf";

                case StiResourceType.Json:
                    return ".json";

                case StiResourceType.Image:
                {
                    if (StiImageHelper.IsPng(Content))
                        return ".png";

                    else if (StiImageHelper.IsMetafile(Content))
                        return ".wmf";

                    else if (StiImageHelper.IsBmp(Content))
                        return ".bmp";

                    else if (StiImageHelper.IsJpeg(Content))
                        return ".jpg";

                    else if (StiImageHelper.IsGif(Content))
                        return ".gif";

                    else if (StiSvgHelper.IsSvg(Content))
                        return ".svg";

                    else if (StiImageHelper.IsTiff(Content))
                        return ".tiff";

                    else if (StiImageHelper.IsEmf(Content))
                        return ".emf";

                    return ".png";
                }
            }

            return string.Empty;
        }

        public StiFileDatabase CreateFileDatabase()
        {
            switch (Type)
            {
                case StiResourceType.Csv:
                    return new StiCsvDatabase
                    {
                        Name = Name,
                        Alias = Name,
                        PathData = $"resource://{Name}"
                    };

                case StiResourceType.Dbf:
                    return new StiDBaseDatabase
                    {
                        Name = Name,
                        Alias = Name,
                        PathData = $"resource://{Name}"
                    };

                case StiResourceType.Excel:
                    return new StiExcelDatabase
                    {
                        Name = Name,
                        Alias = Name,
                        PathData = $"resource://{Name}"
                    };

                case StiResourceType.Json:
                    return new StiJsonDatabase
                    {
                        Name = Name,
                        Alias = Name,
                        PathData = $"resource://{Name}"
                    };

                case StiResourceType.Gis:
                    return new StiGisDatabase
                    {
                        Name = Name,
                        Alias = Name,
                        PathData = $"resource://{Name}"
                    };

                case StiResourceType.Xml:
                    return new StiXmlDatabase
                    {
                        Name = Name,
                        Alias = Name,
                        PathData = $"resource://{Name}"
                    };

                case StiResourceType.Xsd:
                    return new StiXmlDatabase
                    {
                        Name = Name,
                        Alias = Name,
                        PathSchema = $"resource://{Name}"
                    };

                default:
                    return null;
            }
        }
        #endregion

        /// <summary>
		/// Creates an object of the type StiResource.
		/// </summary>
        public StiResource() : this(string.Empty, StiResourceType.Image, null)
		{
		}

        /// <summary>
        /// Creates an object of the type StiResource. 
        /// </summary>
        /// <param name="name">Name of the resource.</param>
        /// <param name="type">Type of the resource.</param>
        /// <param name="content">Content of the resource.</param>
        public StiResource(string name, StiResourceType type, byte[] content) : this(name, name, type, content, false)
		{
		}

        /// <summary>
        /// Creates an object of the type StiResource. 
        /// </summary>
        /// <param name="name">Name of the resource.</param>
        /// <param name="type">Type of the resource.</param>
        /// <param name="content">Content of the resource.</param>
        /// <param name="availableInTheViewer">Value which makes the resource available for download in the viewer.</param>
        public StiResource(string name, StiResourceType type, byte[] content, bool availableInTheViewer) : this(name, name, type, content, availableInTheViewer)
        {
        }

        /// <summary>
        /// Creates an object of the type StiResource. 
        /// </summary>
        /// <param name="name">Name of the resource.</param>
        /// <param name="alias">Alias of the resource.</param>
        /// <param name="type">Type of the resource.</param>
        /// <param name="content">Content of the resource.</param>
        public StiResource(string name, string alias, StiResourceType type, byte[] content) : this(name, alias, type, content, false)
        {
        }

        /// <summary>
        /// Creates an object of the type StiResource. 
        /// </summary>
        /// <param name="name">Name of the resource.</param>
        /// <param name="alias">Alias of the resource.</param>
        /// <param name="type">Type of the resource.</param>
        /// <param name="content">Content of the resource.</param>
        /// <param name="availableInTheViewer">Value which makes the resource available for download in the viewer.</param>
        public StiResource(string name, string alias, StiResourceType type, byte[] content, bool availableInTheViewer) : this(name, alias, false, type, content, availableInTheViewer)
        {
        }

        /// <summary>
        /// Creates an object of the type StiResource. 
        /// </summary>
        /// <param name="name">Name of the resource.</param>
        /// <param name="alias">Alias of the resource.</param>
        /// <param name="inherited">Value which indicates that this resource inherited in dictionary from the master report.</param>
        /// <param name="type">Type of the resource.</param>
        /// <param name="content">Content of the resource.</param>
        /// <param name="availableInTheViewer">Value which makes the resource available for download in the viewer.</param>
        public StiResource(string name, string alias, bool inherited, StiResourceType type, byte[] content, bool availableInTheViewer)
        {
            this.Name = name;
            this.Alias = alias;
            this.Inherited = inherited;
            this.Type = type;
            this.Content = content;
            this.AvailableInTheViewer = availableInTheViewer;
        }
    }
}