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

using Stimulsoft.Report.Components;
using Stimulsoft.Report.Viewer;
using System;
using System.Drawing;

namespace Stimulsoft.Report.Events
{
    public delegate void StiDropFileInViewerEventHandler(object sender, StiDropFileInViewerEventArgs e);

    public class StiDropFileInViewerEventArgs : EventArgs
	{
	    public IStiViewerControl Viewer { get; }

	    public Point Pos { get; }

	    public StiComponent Component { get; }

        public string FileName { get; }

        public StiDropFileInViewerEventArgs(IStiViewerControl viewer, string fileName, Point pos, StiComponent comp)
		{
            this.Viewer = viewer;
            this.Pos = pos;
            this.Component = comp;
            this.FileName = fileName;
        }
	}
}
