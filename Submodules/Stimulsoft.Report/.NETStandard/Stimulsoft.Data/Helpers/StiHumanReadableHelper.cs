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

namespace Stimulsoft.Data.Helpers
{
    public static class StiHumanReadableHelper
    {
        #region Methods
        public static string GetSize(long size)
        {
            var sizes = new[] { "B", "KB", "MB", "GB" };
            var order = 0;
            while (size >= 1024 && ++order < sizes.Length)
            {
                size = size / 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }

        public static string GetHumanReadableName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            var builder = new StringBuilder(name.Trim());
            builder.Replace('_', ' ');

            #region Add spaces between lower and upper letter case
            for (int index = 1; index < builder.Length; index++)
            {
                var prev = builder[index - 1];
                var current = builder[index];

                if (char.IsLetter(prev) && char.IsLetter(current) && char.IsLower(prev) && char.IsUpper(current))
                {
                    builder.Insert(index, ' ');
                    index++;
                }
            }
            #endregion

            #region Then add spaces between letter and digit
            for (int index = 1; index < builder.Length; index++)
            {
                var prev = builder[index - 1];
                var current = builder[index];

                if ((char.IsLetter(prev) && char.IsDigit(current)) || (char.IsDigit(prev) && char.IsLetter(current)))
                {
                    builder.Insert(index, ' ');
                    index++;
                }
            }
            #endregion

            #region Remove symbols which is not letter, digit or spaces
            for (int index = 0; index < builder.Length; index++)
            {
                var current = builder[index];
                if (!char.IsLetterOrDigit(current) && current != ' ')
                    builder.Remove(index, 1);
            }
            #endregion

            if (builder.Length > 0 && !char.IsUpper(builder[0]))
                builder[0] = builder[0].ToString().ToUpperInvariant()[0];

            return builder.ToString();
        }
        #endregion
    }
}