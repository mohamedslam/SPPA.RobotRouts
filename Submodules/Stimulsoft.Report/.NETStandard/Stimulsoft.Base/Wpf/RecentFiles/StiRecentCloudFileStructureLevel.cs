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

using Stimulsoft.Base.Server;
using System.Collections.Generic;

namespace Stimulsoft.Base.Wpf.RecentFiles
{
    public sealed class StiRecentCloudFileStructureLevel :
        IStiRecentCloudFileStructureLevel
    {
        #region Properties
        public string Name { get; set; }

        public string Key { get; set; }

        public string FolderKey { get; set; }

        public bool IsRoot { get; set; }

        public List<StiRecentCloudFileStructureItem> Folders { get; set; }

        public List<StiRecentCloudFileStructureItem> Files { get; set; }
        #endregion

        #region Methods.override
        public override string ToString() => this.Name;
        #endregion

        #region Methods
        public void InitDefault()
        {
            Folders = new List<StiRecentCloudFileStructureItem>();
            Files = new List<StiRecentCloudFileStructureItem>();
        }

        public IStiRecentCloudFileStructureItem AddFolder()
        {
            var folder = new StiRecentCloudFileStructureItem();
            this.Folders.Add(folder);

            return folder;
        }

        public IStiRecentCloudFileStructureItem AddFile()
        {
            var file = new StiRecentCloudFileStructureItem();
            this.Files.Add(file);

            return file;
        }
        #endregion
    }
}