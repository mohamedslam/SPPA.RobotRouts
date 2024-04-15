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

using Stimulsoft.Blockly.Model;
using System;
using System.Collections;

namespace Stimulsoft.Blockly.Blocks.Lists
{
    public class ListsGetIndex : IronBlock
	{
		static Random rnd = new Random();

		public override object Evaluate(Context context)
		{
			var values = this.Values.Evaluate("VALUE", context) as IList;

			var mode = this.Fields.Get("MODE");
			var where = this.Fields.Get("WHERE");

			var index = -1;
			switch (where)
			{
				case "FROM_START":
					index = Convert.ToInt32(this.Values.Evaluate("AT", context)) - 1;
					break;

				case "FROM_END":
					index = values.Count - Convert.ToInt32(this.Values.Evaluate("AT", context));
					break;

				case "FIRST":
					index = 0;
					break;

				case "LAST":
					index = values.Count - 1;
					break;					
				
				case "RANDOM":
					index = rnd.Next(values.Count);
					break;

				default: 		
					throw new NotSupportedException($"unsupported where ({where})");
			}

			switch(mode)
			{
				case "GET":
					return values[index];

				case "GET_REMOVE":
					var value = values[index];
					values.RemoveAt(index);
					return value;

				case "REMOVE":
					values.RemoveAt(index);
					return null;

				default:
					throw new NotSupportedException($"unsupported mode ({mode})");
			}

			
		}
	}
}