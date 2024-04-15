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

using Stimulsoft.Report.Components;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Engine
{
    /// <summary>
    /// A class that helps with bookmarks.
    /// </summary>
    public sealed class StiBookmarksV1Helper
    {
        public static void Pack(StiBookmark bookmark)
        {
            bookmark.ParentComponent = null;
            foreach (StiBookmark book in bookmark.Bookmarks)
                Pack(book);
        }

        public static void Add(StiBookmark bookmark, string name)
        {
            if (bookmark.Bookmarks.IndexOf(name) == -1)
                bookmark.Bookmarks.Add(CreateBookmark(name, bookmark));
        }

        public static void AddBookmarks(StiBookmark bookmark, TreeNode treeNode)
        {
            var indexImageItems = 5;
            var indexImageItem = 4;

            foreach (StiBookmark bm in bookmark.Bookmarks)
            {
                var trNode = new TreeNode(bm.Text, indexImageItems, indexImageItems);
                treeNode.Nodes.Add(trNode);

                if (string.IsNullOrEmpty(bm.BookmarkText)) trNode.Tag = bm.Text;
                else trNode.Tag = bm.BookmarkText;

                AddBookmarks(bm, trNode);
                if (trNode.Nodes.Count == 0)
                {
                    trNode.SelectedImageIndex = indexImageItem;
                    trNode.ImageIndex = indexImageItem;
                }
            }
        }

        /// <summary>
        /// Returns an object of the TreeNode type from the StiBookmark object.
        /// </summary>
        /// <param name="bookmark">StiBookmark to form TreeNode.</param>
        /// <returns>Formed TreeNode.</returns>
        public static TreeNode GetTreeNode(StiBookmark bookmark)
        {
            var indexImageReport = 3;

            var treeNode = new TreeNode(bookmark.Text, indexImageReport, indexImageReport)
            {
                Tag = string.IsNullOrEmpty(bookmark.BookmarkText)
                    ? bookmark.Text
                    : bookmark.BookmarkText
            };


            AddBookmarks(bookmark, treeNode);
            return treeNode;
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
                else pos++;
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

        public static StiBookmark GetBookmark(StiBookmark bookmark, string name)
        {
            StiBookmark bm;

            var index = bookmark.Bookmarks.IndexOf(name);
            if (index == -1)
            {
                bm = CreateBookmark(name, bookmark.Bookmarks);
                bookmark.Bookmarks.Add(bm);
            }
            else
                bm = bookmark.Bookmarks[index];

            return bm;
        }

        /// <summary>
        /// Creates a new bookmark.
        /// </summary>
        /// <param name="text">Tree text.</param>
        public static StiBookmark CreateBookmark(string text, object parentComponent)
        {
            return CreateBookmark(text, string.Empty, parentComponent);
        }

        /// <summary>
        /// Creates a new bookmark.
        /// </summary>
        /// <param name="text">Tree text.</param>
        /// <param name="bookmarkText">Boormark text.</param>
        public static StiBookmark CreateBookmark(string text, string bookmarkText, object parentComponent)
        {
            return new StiBookmark
            {
                Text = text,
                BookmarkText = bookmarkText,
                ParentComponent = parentComponent,
                Engine = StiEngineVersion.EngineV1
            };
        }
    }
}