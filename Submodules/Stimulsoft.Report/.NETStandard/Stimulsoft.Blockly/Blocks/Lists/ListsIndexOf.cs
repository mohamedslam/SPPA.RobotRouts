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
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Blockly.Blocks.Lists
{
    public class ListsIndexOf : IronBlock
	{
		public override object Evaluate(Context context)
		{
			var direction = this.Fields.Get("END");
			var value = this.Values.Evaluate("VALUE", context) as IEnumerable<object>;
			var find = this.Values.Evaluate("FIND", context);

			switch (direction)
			{
				case "FIRST": 
					return value.ToList().IndexOf(find) + 1;
				
				case "LAST": 
					return value.ToList().LastIndexOf(find) + 1;

				default:
					throw new NotSupportedException("$Unknown end: {direction}");
			}
		}
	}
}