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

using System;
using System.Collections;
using System.Text;

namespace Stimulsoft.Report.Export
{
    public class StiEscapeCodesCollection : Hashtable
    {
        #region Properties
        public string Name { get; set; } = string.Empty;
        #endregion

        #region Methods
        public static StiEscapeCodesCollection GetEscapeCodesCollection(string escapeCodesCollectionName)
        {
            var escapeCodesCollection = new StiEscapeCodesCollection();
            foreach (var collection in StiOptions.Export.Text.EscapeCodesCollectionList)
            {
                if (collection.Name == escapeCodesCollectionName)
                    escapeCodesCollection = collection;
            }
            return escapeCodesCollection;
        }

        public static string ConvertEscapeCodes(string text, string escapeCodesCollectionName)
        {
            var escapeCodesCollection = GetEscapeCodesCollection(escapeCodesCollectionName);
            return ConvertEscapeCodes(text, escapeCodesCollection);
        }

        public static string ConvertEscapeCodes(string text, StiEscapeCodesCollection escapeCodesCollection)
        {
            var output = new StringBuilder(text.Length);
            var pos = 0;
            var pos1 = pos;
            while ((pos1 = text.IndexOf("<#", pos1, StringComparison.InvariantCulture)) != -1)
            {
                var pos2 = text.IndexOf(">", pos1 + 3, StringComparison.InvariantCulture);
                if (pos2 == -1) break;
                if (pos2 - pos1 > 64)
                {
                    pos1 += 2;
                    continue;
                }

                output.Append(text.Substring(pos, pos1 - pos));

                var command = text.Substring(pos1 + 2, pos2 - pos1 - 2).Trim();
                var command2 = command.ToLowerInvariant();
                if (escapeCodesCollection.ContainsKey(command))  // !!!
                {
                    var newCommand = (string)escapeCodesCollection[command];
                    output.Append(newCommand);
                }
                else if (escapeCodesCollection.ContainsKey(command2))  // !!!
                {
                    var newCommand = (string)escapeCodesCollection[command2];
                    output.Append(newCommand);
                }
                pos = pos2 + 1;
                pos1 = pos;
            }
            if (pos < text.Length) output.Append(text.Substring(pos));

            return output.ToString();
        }
        #endregion
    }
}
