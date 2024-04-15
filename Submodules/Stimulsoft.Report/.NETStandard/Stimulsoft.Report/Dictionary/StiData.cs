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

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Class describes the data, which is store in DataStore.
	/// </summary>
    [Serializable]
	public class StiData
	{
		#region Properties
		/// <summary>
		/// Gets or sets ViewData.
		/// </summary>
		public object ViewData { get; set; }
		
		/// <summary>
		/// Gets or sets Data.
		/// </summary>
		public object Data { get; set; }

		private string name;
		/// <summary>
		/// Gets or sets name of Data.
		/// </summary>
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
			    if (name == value) return;

			    if (Alias == name)
			        Alias = value;

			    name = value;
			}
		}

		/// <summary>
		/// Gets or sets alias of Data.
		/// </summary>
		public string Alias { get; set; }

		/// <summary>
		/// Gets or sets value, which indicates that data registered of report engine.
		/// </summary>
		public bool IsReportData { get; set; }

        /// <summary>
        /// Gets or sets value, which indicates that data is business object.
        /// </summary>
        public bool IsBusinessObjectData { get; set; }
		
		internal object OriginalConnectionState { get; set; }
		#endregion

		#region Methods
		public override string ToString()
		{
		    return Name == Alias 
		        ? $"{Name}({ViewData.GetType().Name})" 
		        : $"{Name}({Alias})";
		}
		#endregion

		/// <summary>
		/// Creates a new object of the type StiData.
		/// </summary>
		/// <param name="name">Name of Data.</param>
		/// <param name="data">Data.</param>
		public StiData(string name, object data) : this(name, data, data)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiData.
		/// </summary>
		/// <param name="name">Name of Data.</param>
		/// <param name="data">Data.</param>
		/// <param name="viewData">Data for view.</param>
		public StiData(string name, object data, object viewData)
		{
			this.name = name;
			this.Data = data;
			this.ViewData = viewData;
		}
	}
}
