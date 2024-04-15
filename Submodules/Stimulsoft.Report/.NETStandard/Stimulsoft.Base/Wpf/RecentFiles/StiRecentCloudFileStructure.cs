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
    public sealed class StiRecentCloudFileStructure :
        IStiRecentCloudFileStructure
    {
        public StiRecentCloudFileStructure()
        {
            RootLevel = new StiRecentCloudFileStructureLevel
            {
                Name = "Stimulsoft Cloud",
                Key = null,
                FolderKey = null,
                IsRoot = true,
            };

            Levels.Add((StiRecentCloudFileStructureLevel)RootLevel);
        }

        #region Properties
        public IStiRecentCloudFileStructureLevel RootLevel { get; }

        public StiRecentCloudFileStructureLevel CurrentLevel => Levels[Levels.Count - 1];

        public List<StiRecentCloudFileStructureLevel> Levels { get; set; } = new List<StiRecentCloudFileStructureLevel>();

        public bool AllowLevelUp => Levels.Count > 1;

        public StiRecentFileStructureSort Sort { get; set; } = StiRecentFileStructureSort.NameAsc;
        #endregion

        #region Methods
        public void MoveTo(StiRecentCloudFileStructureItem item)
        {
            var newLevel = new StiRecentCloudFileStructureLevel
            {
                Name = item.Name,
                Key = item.ItemKey,
                FolderKey = item.FolderKey,
                Folders = new List<StiRecentCloudFileStructureItem>(),
                Files = new List<StiRecentCloudFileStructureItem>()
            };

            Levels.Add(newLevel);
        }

        public void MoveBack()
        {
            if (Levels.Count > 1)
                Levels.RemoveAt(Levels.Count - 1);
        }

        public void SetNewLevels(List<IStiRecentCloudFileStructureLevel> levels)
        {
            this.Levels.Clear();
            foreach (var level in levels)
            {
                this.Levels.Add((StiRecentCloudFileStructureLevel)level);
            }
        }

        public IStiRecentCloudFileStructureLevel CreateLevel() => new StiRecentCloudFileStructureLevel();
        #endregion
    }
}