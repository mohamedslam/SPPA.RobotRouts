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
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report
{
    internal class StiReportJsonLoaderHelper
    {
        #region Fields
        internal List<IStiMasterComponent> MasterComponents { get; set; } = new List<IStiMasterComponent>();

        internal List<StiClone> Clones { get; set; } = new List<StiClone>();

        internal List<StiDialogInfo> DialogInfo { get; set; } = new List<StiDialogInfo>();
        #endregion

        #region Methods
        internal void Clean()
        {
            MasterComponents.Clear();
            MasterComponents = null;

            Clones.Clear();
            Clones = null;

            DialogInfo.Clear();
            DialogInfo = null;
        }
        #endregion
    }
}