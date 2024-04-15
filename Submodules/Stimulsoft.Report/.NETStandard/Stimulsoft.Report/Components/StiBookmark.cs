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

using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Engine;

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// The class describes a base bookmark class.
    /// </summary>
    public class StiBookmark : IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyStringNullOrEmpty(nameof(Text), Text);
            jObject.AddPropertyEnum(nameof(Engine), Engine, StiEngineVersion.EngineV2);

            if (Engine == StiEngineVersion.EngineV1)
            {
                jObject.AddPropertyStringNullOrEmpty(nameof(BookmarkText), BookmarkText);
            }
            else
            {
                jObject.AddPropertyStringNullOrEmpty(nameof(ComponentGuid), ComponentGuid);
                jObject.AddPropertyBool(nameof(IsManualBookmark), IsManualBookmark);
                jObject.AddPropertyInt(nameof(PageIndex), PageIndex, -1);
            }

            jObject.AddPropertyJObject(nameof(Bookmarks), Bookmarks.SaveToJsonObject(mode));

            return jObject;
        }
       
        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Text):
                        this.Text = property.DeserializeString();
                        break;

                    case nameof(Engine):
                        this.Engine = property.DeserializeEnum<StiEngineVersion>();
                        break;

                    case nameof(BookmarkText):
                        this.BookmarkText = property.DeserializeString();
                        break;

                    case nameof(ComponentGuid):
                        this.ComponentGuid = property.DeserializeString();
                        break;

                    case nameof(IsManualBookmark):
                        this.IsManualBookmark = property.DeserializeBool();
                        break;

                    case nameof(PageIndex):
                        this.PageIndex = property.DeserializeInt();
                        break;

                    case nameof(Bookmarks):
                        this.Bookmarks.LoadFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }
        #endregion

        #region Methods
        public void Add(string name)
        {
            if (Bookmarks.IndexOf(name) == -1) 
                Bookmarks.Add(new StiBookmark(name, this));
        }
        #endregion

        #region Properties
        private StiBookmarksCollection bookmarks;
        /// <summary>
        /// Gets or sets a collection of childs bookmarks.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        public StiBookmarksCollection Bookmarks
        {
            get
            {
                return bookmarks ?? (bookmarks = new StiBookmarksCollection());
            }
            set
            {
                bookmarks = value;
            }
        }

        /// <summary>
        /// Gets or sets a tree node text.
        /// </summary>
        [StiSerializable]
        [DefaultValue("")]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets a engine version.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiEngineVersion.EngineV1)]
        public StiEngineVersion Engine { get; set; } = StiEngineVersion.EngineV1;

        public StiBookmark this[string name] => this.Bookmarks[name];
        #endregion

        #region Properties EngineV1 Only
        /// <summary>
        /// Gets or sets a bookmark text.
        /// </summary>
        [StiSerializable]
        [DefaultValue("")]
        [StiEngine(StiEngineVersion.EngineV1)]
        public string BookmarkText { get; set; }

        [StiEngine(StiEngineVersion.EngineV1)]
        public object ParentComponent { get; set; }
        #endregion

        #region Properties EngineV2 Only
        /// <summary>
        /// Gets or sets a guid of component.
        /// </summary>
        [StiSerializable]
        [StiEngine(StiEngineVersion.EngineV2)]
        [DefaultValue(null)]
        public string ComponentGuid { get; set; }

        /// <summary>
        /// Report engine will be use simplified search if this property is true (Direct compare with BookmarkValue).
        /// </summary>
        [StiSerializable]
        [StiEngine(StiEngineVersion.EngineV2)]
        [DefaultValue(false)]
        public bool IsManualBookmark { get; set; }

        /// <summary>
        /// Page number at which bookmark is located This property is work only for EngineV2 and Stimulsoft Server.
        /// </summary>
        [StiSerializable]
        [StiEngine(StiEngineVersion.EngineV2)]
        [DefaultValue(-1)]
        public int PageIndex { get; set; } = -1;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new bookmark.
        /// </summary>
        public StiBookmark()
            : this(string.Empty, null)
        {
        }

        /// <summary>
        /// Creates a new bookmark.
        /// </summary>
        /// <param name="text">Tree text.</param>
        public StiBookmark(string text, object parentComponent)
            : this(text, "", parentComponent)
        {
        }

        /// <summary>
        /// Creates a new bookmark.
        /// </summary>
        /// <param name="text">Tree text.</param>
        /// <param name="bookmarkText">Boormark text.</param>
        public StiBookmark(string text, string bookmarkText, object parentComponent)
        {
            this.Text = text;
            this.BookmarkText = bookmarkText;
            this.ParentComponent = parentComponent;
        }
        #endregion
    }
}
