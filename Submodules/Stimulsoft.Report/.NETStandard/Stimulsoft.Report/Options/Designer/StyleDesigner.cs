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

using Stimulsoft.Report.Events;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// Class which allows adjustment of the Designer of the report.
        /// </summary>
        public sealed partial class Designer
        {
            public sealed partial class StyleDesigner
            {
                #region Opening
                public static event StiStyleDesignerOpeningEventHandler Opening;

                public static void InvokeOpening(object sender, StiStyleDesignerOpeningEventArgs args)
                {
                    Opening?.Invoke(sender, args);
                }
                #endregion

                #region LoadCustomStyle
                public static event StiLoadCustomEventHandler LoadCustomStyle;
                public static StiBaseStyle InvokeLoadCustomStyle(string ident)
                {
                    var args = new StiLoadCustomEventArgs(ident);
                    LoadCustomStyle?.Invoke(null, args);
                    return args.Style;
                }
                #endregion
            }
        }
    }
}