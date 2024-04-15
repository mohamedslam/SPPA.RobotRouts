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

namespace Stimulsoft.Report.Chart
{
	internal class StiDataItem
	{
		internal int Index;

		internal object Argument;
		internal object Value;
	    internal object ValueEnd;

        internal object Weight;

	    internal object ValueOpen;
        internal object ValueClose;
        internal object ValueLow;
        internal object ValueHigh;

		internal object Title;
		internal object Key;
		internal object Color;

        internal object ToolTip;

        internal object Tag;

        public StiDataItem(int index, object argument, object value, object valueEnd, object weight,
            object valueOpen, object valueClose, object valueLow, object valueHight,
            object title, object key, object color, object toolTip, object tag)
		{
			this.Index = index;

			this.Argument = argument;
			this.Value = value;
		    this.ValueEnd = valueEnd;

            this.Weight = weight;

		    this.ValueOpen = valueOpen;
		    this.ValueClose = valueClose;
		    this.ValueLow = valueLow;
		    this.ValueHigh = valueHight;

			this.Title = title;
			this.Key = key;
			this.Color = color;

            this.ToolTip = toolTip;

            this.Tag = tag;
		}
	}
}
