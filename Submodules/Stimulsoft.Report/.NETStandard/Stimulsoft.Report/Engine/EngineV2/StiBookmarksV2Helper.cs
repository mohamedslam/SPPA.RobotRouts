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
using Stimulsoft.Report.Components;
using System.Collections;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Engine
{
    /// <summary>
    /// A class helps with bookmarks.
    /// </summary>
    public sealed class StiBookmarksV2Helper
    {
        /// <summary>
        /// Fills the specified TreeNode with bookmarks from the specified bookmark.
        /// </summary>
        private static void FillTreeNodeFromBookmarks(StiBookmark parentBookmark, TreeNode parentNode)
        {
            var indexImageItems = 5;
            var indexImageItem = 4;

            if (StiScale.Step > 1)
            {
                indexImageItems -= 3;
                indexImageItem -= 3;
            }
            if (StiScale.Step >= 2)
            {
                indexImageItems += 3;
                indexImageItem += 3;
            }

            lock (((ICollection) parentBookmark.Bookmarks).SyncRoot)
            {
                foreach (StiBookmark bookmark in parentBookmark.Bookmarks)
                {
                    var createdNode = new TreeNode(bookmark.Text, indexImageItems, indexImageItems);
                    parentNode.Nodes.Add(createdNode);
                    createdNode.Tag = bookmark;

                    FillTreeNodeFromBookmarks(bookmark, createdNode);
                    if (createdNode.Nodes.Count == 0)
                    {
                        createdNode.SelectedImageIndex = indexImageItem;
                        createdNode.ImageIndex = indexImageItem;
                    }
                }
            }
        }

        /// <summary>
        /// Returns an object of the TreeNode type from the StiBookmark object.
        /// </summary>
        /// <param name="bookmark">StiBookmark to form TreeNode.</param>
        /// <returns>Formed TreeNode.</returns>
        public static TreeNode GetTreeNodeFromBookmarks(StiBookmark bookmark)
        {
            var indexImageReport = 3;
            if (StiScale.Step >= 2) indexImageReport = 3;

            var parentNode = new TreeNode(bookmark.Text, indexImageReport, indexImageReport)
            {
                Tag = bookmark
            };

            FillTreeNodeFromBookmarks(bookmark, parentNode);
            return parentNode;
        }

        /// <summary>
        /// Adds a bookmark manually. When adding the IsManualBookmark flag is set.
        /// This flag gives a signal to the report generator that search should be done in a reductive mode
        /// (direct comparison of BookmarkValue).
        /// </summary>
        /// <param name="name"></param>
        public static void Add(StiBookmark bookmark, string name)
        {
            var bookmark2 = CreateBookmark(name);
            bookmark2.IsManualBookmark = true;
            bookmark.Bookmarks.Add(bookmark2);
        }

        public static StiBookmark GetBookmark(StiBookmark bookmark, string name)
        {
            StiBookmark bm;

            var index = bookmark.Bookmarks.IndexOf(name);
            if (index == -1)
            {
                bm = CreateBookmark(name);
                bookmark.Bookmarks.Add(bm);
            }
            else
                bm = bookmark.Bookmarks[index];

            return bm;
        }

        /// <summary>
        /// Prepares a bookmark and all of its child bookmarks for showing.
        /// </summary>
        /// <param name="bookmark">Bookmark for preparing.</param>
        public static void PrepareBookmark(StiBookmark bookmark)
        {
            var pos = 0;
            while (pos < bookmark.Bookmarks.Count - 1)
            {
                if (bookmark.Bookmarks[pos].Text == bookmark.Bookmarks[pos + 1].Text)
                {
                    bookmark.Bookmarks[pos].Bookmarks.AddRange(bookmark.Bookmarks[pos + 1].Bookmarks);
                    bookmark.Bookmarks.Remove(bookmark.Bookmarks[pos + 1]);
                }
                else
                    pos++;
            }

            var index = 0;
            while (index < bookmark.Bookmarks.Count)
            {
                var book1 = bookmark.Bookmarks[index];

                var indexBook = index + 1;
                while (indexBook < bookmark.Bookmarks.Count)
                {
                    var book2 = bookmark.Bookmarks[indexBook];

                    if (book1.Text == book2.Text && book1.ParentComponent == book2.ParentComponent)
                    {
                        book1.Bookmarks.AddRange(book2.Bookmarks);
                        bookmark.Bookmarks.RemoveAt(indexBook);
                    }
                    else
                        indexBook++;
                }
                index++;
            }

            foreach (StiBookmark bk in bookmark.Bookmarks)
                PrepareBookmark(bk);
        }

        public static void Pack(StiBookmark bookmark)
        {
            bookmark.ParentComponent = null;
            foreach (StiBookmark book in bookmark.Bookmarks)
                Pack(book);
        }

        /// <summary>
        /// Creates a new bookmark.
        /// </summary>
        /// <param name="text">Tree text.</param>
        public static StiBookmark CreateBookmark(string text)
        {
            return CreateBookmark(text, null);
        }

        /// <summary>
        /// Creates a new bookmark.
        /// </summary>
        /// <param name="text">Tree text.</param>
        public static StiBookmark CreateBookmark(string text, string componentGuid)
        {
            return new StiBookmark
            {
                Text = text,
                ComponentGuid = componentGuid,
                Engine = StiEngineVersion.EngineV2
            };
        }
    }
}
