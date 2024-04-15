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
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.QuickButtons
{
	/// <summary>
	/// The class describes the base type for the quick button.
	/// </summary>
	public abstract class StiQuickButton : IDisposable
	{
		#region Properties
		/// <summary>
		/// Gets name of the quick button.
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// Gets image of the quick button.
		/// </summary>
		public abstract Image Image { get; }
        
		/// <summary>
		/// Gets selected image of the quick button.
		/// </summary>
		public abstract Image SelectedImage { get; }

	    /// <summary>
		/// Gets or sets value which indicates this quick button is selected.
		/// </summary>
		public bool IsSelected { get; set; }

	    /// <summary>
		/// Gets alignment of the quick button.
		/// </summary>
		public virtual StiQuickButtonAlignment Alignment => StiQuickButtonAlignment.Right;
	    #endregion

		#region Methods
		/// <summary>
		/// Dispose this quick button.
		/// </summary>
		public void Dispose()
		{
		    if (Image == null) return;

		    Image.Dispose();
		}

        public abstract void OnClick(Control control, Point pos, StiComponent component, IStiDesignerBase designer);
		#endregion
	}
}
