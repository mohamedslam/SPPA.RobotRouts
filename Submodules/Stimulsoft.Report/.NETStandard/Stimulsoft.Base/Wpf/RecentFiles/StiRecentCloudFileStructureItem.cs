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
using System;

namespace Stimulsoft.Base.Wpf.RecentFiles
{
    public sealed class StiRecentCloudFileStructureItem :
        IStiRecentCloudFileStructureItem
    {
        #region Properties
        public string Name { get; set; }

        public string ItemKey { get; set; }

        public string FolderKey { get; set; }

        public StiRecentItemContentType ContentType { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsFolder { get; set; }
        #endregion

        #region Methods.override
        public override string ToString() => this.Name;
        #endregion
    }
}