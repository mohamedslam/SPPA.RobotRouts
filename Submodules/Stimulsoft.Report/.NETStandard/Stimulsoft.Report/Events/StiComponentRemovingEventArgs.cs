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
using Stimulsoft.Report.Design;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Events
{
	/// <summary>
	/// Represents the method that handles the ComponentRemoved event.
	/// </summary>
	public delegate void StiComponentRemovingHandler(object sender, StiComponentRemovingEventArgs e);

	/// <summary>
	/// Describes an argument for the event ComponentRemoved.
	/// </summary>
	public class StiComponentRemovingEventArgs : EventArgs
	{
	    public IStiDesignerBase Designer { get; }

	    public StiComponent Component { get; }

	    public StiComponentRemovingEventArgs(IStiDesignerBase designer, StiComponent component)
		{
			this.Designer = designer;
			this.Component = component;
		}
	}
}
