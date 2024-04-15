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
using System.Collections.Generic;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Helpers
{
    public class StiClipboardHelper
    {
        #region Properties
        public static bool Enabled { get; set; } = true;
        #endregion

        #region Methods
        public static IDictionary<string, object> GetClipboardData()
        {
            if (!Enabled) return null;

            try
            {
                var dict = new Dictionary<string, object>();
                var dataObject = Clipboard.GetDataObject();
                foreach (var format in dataObject?.GetFormats())
                {
                    dict.Add(format, dataObject.GetData(format));
                }
                return dict;
            }
            catch
            {
            }
            return null;
        }

        public static void SetClipboardData(IDictionary<string, object> dict)
        {
            try
            {
                Clipboard.Clear();
                if (Enabled && dict != null)
                {
                    foreach (var pair in dict)
                    {
                        Clipboard.SetData(pair.Key, pair.Value);
                    }
                }
            }
            catch
            {
            }
        }
        #endregion
    }
}
