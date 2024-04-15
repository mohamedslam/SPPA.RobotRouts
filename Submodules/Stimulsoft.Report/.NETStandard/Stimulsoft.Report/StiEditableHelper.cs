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

using Stimulsoft.Base.Serializing;

namespace Stimulsoft.Report
{
	public class StiEditableItem
	{
	    [StiSerializable]
		public int PageIndex { get; set; } = -1;

	    [StiSerializable]
		public int Position { get; set; } = -1;

	    [StiSerializable]
		public string ComponentName { get; set; }

	    [StiSerializable]
		public string TextValue { get; set; }

	    public StiEditableItem()
		{
		}

		public StiEditableItem(int pageIndex, int position, string componentName, string textValue)
		{
			this.PageIndex = pageIndex;
			this.Position = position;
			this.ComponentName = componentName;
			this.TextValue = textValue;
		}
	}
}
