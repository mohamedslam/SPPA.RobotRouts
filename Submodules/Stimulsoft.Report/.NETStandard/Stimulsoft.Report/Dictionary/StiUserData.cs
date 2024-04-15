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
using System.ComponentModel;
using System.Drawing;

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Class describes data definied by user.
	/// </summary>
	[ToolboxBitmap(typeof(StiUserData), "Toolbox.StiUserData.bmp")]
	public class StiUserData : Component
	{
        #region Properties
        /// <summary>
        /// Gets or sets name of user data source.
        /// </summary>
        [DefaultValue("UserData")]
		public string Name { get; set; } = "UserData";

	    /// <summary>
		/// Gets or sets alias of user data source.
		/// </summary>
		[DefaultValue("UserData")]
		public string Alias { get; set; } = "UserData";

	    /// <summary>
		/// Gets or sets count rows in user data source.
		/// </summary>
		[DefaultValue(0)]
		public int Count { get; set; }

	    /// <summary>
		/// Gets collection of columns of data source.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public StiDataColumnsCollection Columns { get; } = new StiDataColumnsCollection();
        #endregion

        #region Events
        #region OnConnect
        /// <summary>
        /// Occurs when connect to data.
        /// </summary>
        public event EventHandler Connect;

		/// <summary>
		/// Raises the Connect event for this component.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		public void InvokeConnect(EventArgs e)
		{
            this.Connect?.Invoke(this, e);
        }
		#endregion

		#region OnDisconnent
		/// <summary>
		/// Occurs when disconnect from data.
		/// </summary>
		public event EventHandler Disconnent;

		/// <summary>
		/// Raises the Disconnent event for this component.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		public void InvokeDisconnent(EventArgs e)
		{
            this.Disconnent?.Invoke(this, e);
        }
		#endregion

		#region OnGetData
		/// <summary>
		/// Occurs when get data from source.
		/// </summary>
		public event StiUserGetDataEventHandler GetData;

		/// <summary>
		/// Raises the GetData event for this component.
		/// </summary>
		/// <param name="e">An StiUserGetDataEventArgs that contains the event data.</param>
		public void InvokeGetData(StiUserGetDataEventArgs e)
		{
            this.GetData?.Invoke(this, e);
        }
        #endregion
        #endregion
    }
}
