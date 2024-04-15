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

namespace Stimulsoft.Report.Print
{
    public delegate void StiPrintPageEventHandler(object sender, StiPrintPageEventArgs e);

    public class StiPrintPageEventArgs : EventArgs
    {
        public int CurrentPage { get; set; }

        public int FromPage { get; set; }

        public int ToPage { get; set; }

        internal StiPrintPageEventArgs(int currentPage, int fromPage, int toPage)
        {
            this.CurrentPage = currentPage;
            this.FromPage = fromPage;
            this.ToPage = toPage;
        }
    }
}
