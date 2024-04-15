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

using System;

namespace Stimulsoft.Base.Wpf.RecentFiles
{
    public sealed class StiRecentFolder
    {
        public StiRecentFolder()
        {
            Guid = global::System.Guid.NewGuid().ToString().Replace("-", "");
        }

        #region Properties
        public string Guid { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string CloudKey { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsPinned { get; set; }

        public bool IsCloud { get; set; }
        #endregion

        #region Methods
        public override string ToString() => Name;
        #endregion
    }
}