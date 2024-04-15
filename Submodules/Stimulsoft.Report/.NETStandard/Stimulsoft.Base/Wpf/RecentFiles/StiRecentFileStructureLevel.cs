﻿#region Copyright (C) 2003-2022 Stimulsoft
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
    public sealed class StiRecentFileStructureLevel :
        IStiRecentFileStructureLevel
    {
        #region Properties
        public string Name { get; set; }

        public string Path { get; set; }

        public bool IsRoot { get; set; }

        public List<StiRecentFileStructureItem> Folders { get; set; }

        public List<StiRecentFileStructureItem> Files { get; set; }
        #endregion

        #region Methods.override
        public override string ToString() => this.Name;
        #endregion
    }
}