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

using System.Collections;
using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes a collection to store Bookmark.
    /// </summary>
    public class StiBookmarksCollection :
        CollectionBase,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            if (List.Count == 0)
                return null;

            var jObject = new JObject();

            int index = 0;
            foreach (StiBookmark bookmark in List)
            {
                jObject.AddPropertyJObject(index.ToString(), bookmark.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                var bookmarkJObject = (JObject)property.Value;
                var bookmark = new StiBookmark();
                bookmark.LoadFromJsonObject(bookmarkJObject);
                List.Add(bookmark);
            }
        }
        #endregion

        public void Add(StiBookmark bookmark)
        {
            List.Add(bookmark);
        }

        public void AddRange(StiBookmarksCollection bookmarks)
        {
            foreach (StiBookmark bookmark in bookmarks)
            {
                Add(bookmark);
            }
        }

        public void AddRange(StiBookmark[] bookmarks)
        {
            foreach (StiBookmark bookmark in bookmarks)
            {
                Add(bookmark);
            }
        }

        public bool Contains(StiBookmark bookmark)
        {
            return List.Contains(bookmark);
        }

        public int IndexOf(StiBookmark bookmark)
        {
            return List.IndexOf(bookmark);
        }

        public int IndexOf(string name)
        {
            int index = 0;
            foreach (StiBookmark bm in List)
            {
                if (bm.Text == name) 
                    return index;

                index++;
            }
            return -1;
        }

        public void Insert(int index, StiBookmark bookmark)
        {
            List.Insert(index, bookmark);
        }

        public void Remove(StiBookmark bookmark)
        {
            List.Remove(bookmark);
        }

        public StiBookmark this[string name]
        {
            get
            {
                var index = IndexOf(name);
                StiBookmark bm;
                if (index == -1)
                {
                    bm = new StiBookmark(name, this);
                    Add(bm);
                }
                else
                {
                    bm = List[index] as StiBookmark;
                }

                return bm;
            }
        }

        public StiBookmark this[int index]
        {
            get
            {
                return (StiBookmark)List[index];
            }
            set
            {
                List[index] = value;
            }
        }
    }
}
