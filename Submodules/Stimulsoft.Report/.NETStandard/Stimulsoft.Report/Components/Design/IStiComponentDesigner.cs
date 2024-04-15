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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Design;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// Interface describes a component designer.
	/// </summary>
	public interface IStiComponentDesigner
	{
		/// <summary>
		/// Returns StiAction for specified component from point.
		/// </summary>
		/// <param name="x">x coordinate.</param>
		/// <param name="y">y coordinate.</param>
		/// <param name="component">Component for checking.</param>
		/// <returns>Action for this point.</returns>
		StiAction GetActionFromPoint(double x, double y, StiComponent component);
		
		/// <summary>
		/// Occurs when user DoubleClick on a the component in the designer.
		/// </summary>
		/// <param name="sender">Component on what DoubleClick occured.</param>
		void OnDoubleClick(StiComponent sender);

		/// <summary>
		/// Calls the designer of the component.
		/// </summary>
		/// <param name="component">Component for edition.</param>
		/// <returns>Result of showing the component designer.</returns>
		DialogResult Design(StiComponent component);

		/// <summary>
		/// Creates a component of the specified type.
		/// </summary>
		/// <param name="componentType">Type of conmponent being ceated.</param>
		/// <param name="region">The rectangle describes the component size.</param>
		/// <returns>Created component.</returns>
		StiComponent CreateComponent(Type componentType, RectangleD region);

		/// <summary>
		/// Report designer.
		/// </summary>
        IStiDesignerBase Designer
		{
			get;
			set;
		}
	}
}
