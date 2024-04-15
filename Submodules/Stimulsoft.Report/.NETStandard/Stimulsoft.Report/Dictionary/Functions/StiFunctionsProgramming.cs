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

namespace Stimulsoft.Report.Dictionary
{
    public class StiFunctionsProgramming
	{
		public static object Choose(int index, params object[] args)
		{
			if (args.Length == 0)
				return null;

			if (index < 1 || index > args.Length)
				return null;

			return args[index - 1];
		}

		public static object Choose(int? index, params object[] args)
		{
			if (!index.HasValue) 
				return null;

			if (args.Length == 0)
				return null;

			if (index.Value < 1 || index.Value > args.Length)
				return null;

			return args[index.Value - 1];
		}

		public static object Switch(params object[] args)
		{
			for (int index = 0; index < args.Length; index += 2)
			{
                var condition = args[index];
                var value = args[index + 1];

				if (condition is bool && ((bool)condition) == true)
                    return value;

                if (condition is bool? && ((bool?)condition) == true)
                    return value;
			}
			return null;
		}
	}
}
