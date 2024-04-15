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

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report
{
    /// <summary>
    /// Represents the method that handles the SaveChangesInDialogBoxDesigner event.
    /// </summary>
    public delegate void StiSaveChangesInDialogBoxDesignerEventHandler(object sender, StiSaveChangesInDialogBoxDesignerEvenArgs e);

    /// <summary>
    /// Describes the class that contains data for the event SaveChangesInDialogBoxDesigner.
    /// </summary>
    public class StiSaveChangesInDialogBoxDesignerEvenArgs : EventArgs
    {
        public DialogResult DialogResult { get; set; } = DialogResult.None;

        public bool ShowDefaultDialogBox { get; set; } = true;

        public string Message { get; set; } = "";
    }
}
