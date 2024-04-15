#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using System.ComponentModel;

namespace Stimulsoft.Base.Drawing
{
	public class RectangleMHelper : TypeConverter
	{
		public static string ConvertRectangleMToString(RectangleM rectangle)
		{
			return $"{rectangle.X},{rectangle.Y},{rectangle.Width},{rectangle.Height}";
		}

        public static string ConvertRectangleMToString(RectangleM rectangle, char separator)
        {
            return $"{rectangle.X}{separator}{rectangle.Y}{separator}{rectangle.Width}{separator}{rectangle.Height}";
        }

        public static RectangleM ConvertStringToRectangleM(string str, char separator)
		{
			var words = str.Split(separator);

            return new RectangleM(
				decimal.Parse(words[0]),
                decimal.Parse(words[1]),
                decimal.Parse(words[2]),
                decimal.Parse(words[3]));
		}
	}
}
