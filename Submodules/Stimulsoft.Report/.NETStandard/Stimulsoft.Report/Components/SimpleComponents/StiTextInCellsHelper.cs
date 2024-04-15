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

namespace Stimulsoft.Report.Components
{
	public static class StiTextInCellsHelper
	{
        #region Methods
        public static string TrimEndWhiteSpace(string inputString)
		{
			if (StiOptions.Engine.MeasureTrailingSpaces)
			{
				return inputString;
			}
			else
			{
				string outputString = string.Empty;
				int index = inputString.Length;
				while ((index > 0) && (char.IsWhiteSpace(inputString[index - 1])))
				{
					index--;
				}
				if (index == inputString.Length)
				{
					outputString = inputString;
				}
				else
				{
					if (index > 0)
					{
						outputString = inputString.Substring(0, index);
					}
				}
				return outputString;
			}
		}
        #endregion
    }
}