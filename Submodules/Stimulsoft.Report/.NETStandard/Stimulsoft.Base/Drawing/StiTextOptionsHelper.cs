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

using System;
using System.ComponentModel;
using System.Globalization;
using Stimulsoft.Base.Localization;
using System.Drawing.Text;
using System.Drawing;

namespace Stimulsoft.Base.Drawing
{
	public class StiTextOptionsHelper : TypeConverter
	{
		public static string ConvertTextOptionsToString(StiTextOptions op, char separator)
		{
            var format = op.FirstTabOffset == 40f && op.DistanceBetweenTabs == 20f 
                ? "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}" 
                : "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}";

            return string.Format(format, separator,
                op.HotkeyPrefix != HotkeyPrefix.None ? "HotkeyPrefix=" + op.HotkeyPrefix : string.Empty,
                op.LineLimit ? "LineLimit=True" : string.Empty,
                op.RightToLeft ? "RightToLeft=True" : string.Empty,
                op.Trimming != StringTrimming.None ? "Trimming=" + op.Trimming : string.Empty,
                op.WordWrap ? "WordWrap=True" : string.Empty,
                op.Angle == 0f ? "A=0" : "Angle=" + op.Angle,
                op.FirstTabOffset == 40f ? "O=40" : "FirstTabOffset=" + op.FirstTabOffset,
                op.DistanceBetweenTabs == 20f ? "D=20" : "DistanceBetweenTabs=" + op.DistanceBetweenTabs);
		}

        public static string ConvertTextOptionsToLocalizedString(StiTextOptions op, bool wordWrap)
        {
            var ch = CultureInfo.CurrentCulture.TextInfo.ListSeparator[0];

            return ConvertTextOptionsToLocalizedString(op, ch, wordWrap);
        }

        public static string ConvertTextOptionsToLocalizedString(StiTextOptions op, char separator, bool wordWrap)
        {
            var boolTrueStr = Loc.Get("PropertyEnum", "boolTrue");
            var boolFalseStr = Loc.Get("PropertyEnum", "boolFalse");

            if (wordWrap)
            {
                return string.Format(
                    "{0}={1}\n{2}={3}\n{4}={5}\n{6}={7}\n{8}={9}\n{10}={11}\n{12}={13}\n{14}={15}",
                    Loc.Get("PropertyMain", "HotkeyPrefix"),
                    Loc.Get("PropertyEnum", "HotkeyPrefix" + op.HotkeyPrefix),

                    Loc.Get("PropertyMain", "LineLimit"),
                    op.LineLimit ? boolTrueStr : boolFalseStr,

                    Loc.Get("PropertyMain", "RightToLeft"),
                    op.RightToLeft ? boolTrueStr : boolFalseStr,

                    Loc.Get("PropertyMain", "Trimming"),
                    Loc.Get("PropertyEnum", "StringTrimming" + op.Trimming),

                    Loc.Get("PropertyMain", "WordWrap"),
                    op.WordWrap ? boolTrueStr : boolFalseStr,

                    Loc.Get("PropertyMain", "Angle"),
                    op.Angle,

                    Loc.Get("PropertyMain", "FirstTabOffset"),
                    op.FirstTabOffset,

                    Loc.Get("PropertyMain", "DistanceBetweenTabs"),
                    op.DistanceBetweenTabs);

            }
            else
            {
                return string.Format(
                    "{0}={1}{2} {3}={4}{5} {6}={7}{8} {9}={10}{11} {12}={13}{14} {15}={16}{17} {18}={19}{20} {21}={22}",
                    Loc.Get("PropertyMain", "HotkeyPrefix"),
                    Loc.Get("PropertyEnum", "HotkeyPrefix" + op.HotkeyPrefix), separator,

                    Loc.Get("PropertyMain", "LineLimit"),
                    op.LineLimit ? boolTrueStr : boolFalseStr, separator,

                    Loc.Get("PropertyMain", "RightToLeft"),
                    op.RightToLeft ? boolTrueStr : boolFalseStr, separator,

                    Loc.Get("PropertyMain", "Trimming"),
                    Loc.Get("PropertyEnum", "StringTrimming" + op.Trimming), separator,

                    Loc.Get("PropertyMain", "WordWrap"),
                    op.WordWrap ? boolTrueStr : boolFalseStr, separator,

                    Loc.Get("PropertyMain", "Angle"),
                    op.Angle, separator,

                    Loc.Get("PropertyMain", "FirstTabOffset"),
                    op.FirstTabOffset, separator,

                    Loc.Get("PropertyMain", "DistanceBetweenTabs"),
                    op.DistanceBetweenTabs);
            }
        }

		
		public static StiTextOptions ConvertStringToTextOptions(string str, char separator)
		{
			var words = str.Split(separator);

			var to = new StiTextOptions();
                
			#region HotkeyPrefix
            if (words[0].IndexOf("Hide", StringComparison.InvariantCulture) >= 0)
                to.HotkeyPrefix = HotkeyPrefix.Hide;

            else if (words[0].IndexOf("Show", StringComparison.InvariantCulture) >= 0)
                to.HotkeyPrefix = HotkeyPrefix.Show;

			else
                to.HotkeyPrefix = HotkeyPrefix.None;
			#endregion

			#region LineLimit
            to.LineLimit = words[1].IndexOf("True", StringComparison.InvariantCulture) >= 0;
			#endregion

			#region RightToLeft
            to.RightToLeft = words[2].IndexOf("True", StringComparison.InvariantCulture) >= 0;
			#endregion

			#region Trimming			
            if (words[3].IndexOf("EllipsisCharacter", StringComparison.InvariantCulture) >= 0)
                to.Trimming = StringTrimming.EllipsisCharacter;

            else if (words[3].IndexOf("Character", StringComparison.InvariantCulture) >= 0)
                to.Trimming = StringTrimming.Character;

            else if (words[3].IndexOf("EllipsisPath", StringComparison.InvariantCulture) >= 0)
                to.Trimming = StringTrimming.EllipsisPath;

            else if (words[3].IndexOf("EllipsisWord", StringComparison.InvariantCulture) >= 0)
                to.Trimming = StringTrimming.EllipsisWord;

            else if (words[3].IndexOf("None", StringComparison.InvariantCulture) >= 0)
                to.Trimming = StringTrimming.None;

            else if (words[3].IndexOf("Word", StringComparison.InvariantCulture) >= 0)
                to.Trimming = StringTrimming.Word;
			#endregion

			#region WordWrap
            to.WordWrap = words[4].IndexOf("True", StringComparison.InvariantCulture) >= 0;
			#endregion

			#region Angle
			var angles = words[5].Split('=');
			to.Angle = float.Parse(angles[1].Trim());
			
			#endregion

			if (words.Length > 6)
			{
				#region FirstTabOffset
				var firsts = words[6].Split('=');
				to.FirstTabOffset = float.Parse(firsts[1].Trim());			
				#endregion

				#region DistanceBetweenTabs
				var tabs = words[7].Split('=');
				to.DistanceBetweenTabs = float.Parse(tabs[1].Trim());			
				#endregion
			}

			return to;
		}
	}
}
