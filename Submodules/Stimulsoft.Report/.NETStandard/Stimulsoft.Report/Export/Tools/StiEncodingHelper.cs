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

using System.Text;

namespace Stimulsoft.Report.Export
{
	public sealed class StiEncodingHelper
	{
		public static string[] GetEncodingNames()
		{
            return GetEncodingNames(GetEncodings());
		}

		public static string[] GetEncodingNames(Encoding[] encodings)
		{
			var names = new string[encodings.Length];

			for (var index = 0; index < encodings.Length; index++)
			{
				names[index] = encodings[index].EncodingName;
			}
			return names;
		}

        public static Encoding[] GetEncodings()
        {
            try
            {
                return new[]
				{
					Encoding.Default,
					Encoding.ASCII,
					Encoding.BigEndianUnicode,
					Encoding.Unicode,
					Encoding.UTF7,
					Encoding.UTF8,
                    Encoding.GetEncoding(1250),
                    Encoding.GetEncoding(1251),
                    Encoding.GetEncoding(1252),
                    Encoding.GetEncoding(1253),
                    Encoding.GetEncoding(1254),
                    Encoding.GetEncoding(1255),
                    Encoding.GetEncoding(1256)
				};
            }
            catch
            {
                return new[]
				{
					Encoding.Default,
					Encoding.ASCII,
					Encoding.BigEndianUnicode,
					Encoding.Unicode,
					Encoding.UTF7,
					Encoding.UTF8
				};
            }
        }

		public static Encoding GetEncodingFromName(string encodingName)
		{
			return GetEncodingFromName(encodingName, GetEncodings());
		}

		public static Encoding GetEncodingFromName(string encodingName, Encoding[] encodings)
		{
			foreach (var enc in encodings)
			{
				if (enc.EncodingName == encodingName)
				    return enc;
			}
			return Encoding.UTF8;
		}
	}
}
