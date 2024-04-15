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

using System.Collections;
using System.ComponentModel;

namespace Stimulsoft.Report.Controls
{
	[ToolboxItem(false)]
	public class StiDialogLookUpBox : StiDialogComboBox
	{
		public ArrayList Keys { get; set; } = new ArrayList();

		public object SelectedKey
		{
			get
			{
				if (Items == null || Items.Count == 0)
					return string.Empty;

				int index = 0;
				foreach (object obj in Items)
				{
					if (this.SelectedItem == obj)					
                        return Keys[index];
										
					index++;
					
					if (index == Keys.Count)
						return string.Empty;
				}
				return null;
			}
			set
			{
				if (Items == null || Items.Count == 0)return;

				int index = 0;
				foreach (object obj in Keys)
				{
					if (value == obj)
					{
						SelectedIndex = index;
						return;
					}					
					index++;

					if (index == Items.Count)return;
				}
			}
		}
	}
}
