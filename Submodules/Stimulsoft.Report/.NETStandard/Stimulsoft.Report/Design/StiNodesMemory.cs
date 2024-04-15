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
using System.Collections;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Design
{
	/// <summary>
	/// Class allows to save and restore condition a treeview
	/// </summary>
	public sealed class StiNodesMemory
	{
        #region Fields
        private Hashtable savedNodes = new Hashtable();
		private string selectedText;
		private bool nodesSave;
        #endregion

        #region Methods
        private static string GetFullPath(TreeNode node)
		{
			string path = node.Text;

			while (node.Parent != null)
			{
				node = node.Parent;
				path += "." + node.Text;
			}
			return path;
		}

		private void Clear()
		{
			savedNodes.Clear();
		}

		public void SaveExpNodes(TreeView treeView)
		{
			SaveExpNodes(treeView, true);
		}

		public void SaveExpNodes(TreeView treeView, bool forceNodesSave)
		{
			if (treeView.SelectedNode == null)
                selectedText = null;

            else
                selectedText = GetFullPath(treeView.SelectedNode);

            if (nodesSave || forceNodesSave)
			{
				Clear();
				SaveExpNodes(treeView.Nodes);
			}

			nodesSave = true;
		}

		private void SaveExpNodes(TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes)
			{
				if (node.IsExpanded)
                    savedNodes[GetFullPath(node)] = node;

				SaveExpNodes(node.Nodes);
			}
		}

		public void LoadExpNodes(TreeView treeView, bool restoreSelectedNode)
		{
			LoadExpNodes(treeView.Nodes, treeView, restoreSelectedNode);
		}

		private void LoadExpNodes(TreeNodeCollection nodes, TreeView treeView, bool restoreSelectedNode)
		{
			foreach (TreeNode node in nodes)
			{
				string path = GetFullPath(node);
				if (restoreSelectedNode)
				{					
					if (path == selectedText)
					{
						treeView.SelectedNode = node;
						treeView.SelectedNode.EnsureVisible();
					}
				}

				if (savedNodes[path] != null)
                    node.Expand();
				
				LoadExpNodes(node.Nodes, treeView, restoreSelectedNode);
			}
		}
        #endregion
    }
}
