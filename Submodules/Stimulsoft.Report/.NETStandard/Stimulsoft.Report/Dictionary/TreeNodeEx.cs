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

using System.Collections.Generic;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dictionary
{
    public class TreeNodeEx : TreeNode
    {
        #region Fields.Internal
        private int level;
        private List<StiDataSource> datas;
        #endregion

        #region Methods
        internal void FillFromNodeEx(ref StiDataSourcesCollection datas, ref int level)
        {
            if (this.datas != null && this.datas.Count > 0)
            {
                foreach (var data in this.datas)
                {
                    datas.Add(data);
                }
            }

            level = this.level;
        }

        internal void FillNodeEx(StiDataSourcesCollection datas, int level)
        {
            this.level = level;

            if (datas.Count > 0)
            {
                this.datas = new List<StiDataSource>(datas.Count);
                foreach (StiDataSource dataSource in datas)
                {
                    this.datas.Add(dataSource);
                }
            }
            else
                this.datas = null;
        }
        #endregion

        public TreeNodeEx()
        {
        }

        public TreeNodeEx(string text)
            : base(text)
        {
        }
    }
}