#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

namespace Stimulsoft.Report.Dashboard
{
    public delegate void StiElementChangedEventHandler(object sender, StiElementChangedArgs e);

    public sealed class StiElementChangedArgs : EventArgs
    {
        #region Properties
        public StiElementMeterAction Action { get; set; } = StiElementMeterAction.None;

        public string OldName { get; set; }

        public string NewName { get; set; }
        #endregion

        #region Methods
        public static StiElementChangedArgs CreateEmptyArgs()
        {
            return new StiElementChangedArgs();
        }

        public static StiElementChangedArgs CreateRenamingArgs(string oldName, string newName)
        {
            return new StiElementChangedArgs
            {
                Action = StiElementMeterAction.Rename,
                OldName = oldName,
                NewName = newName
            };
        }

        public static StiElementChangedArgs CreateDeletingArgs(string name)
        {
            return new StiElementChangedArgs
            {
                Action = StiElementMeterAction.Delete,
                OldName = name
            };
        }

        public static StiElementChangedArgs CreateClearingAllArgs()
        {
            return new StiElementChangedArgs
            {
                Action = StiElementMeterAction.ClearAll
            };
        }
        #endregion
    }
}
