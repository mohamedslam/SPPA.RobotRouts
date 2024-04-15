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

namespace Stimulsoft.Report.Helpers
{
    public class StiSymbolIcon
    {
        #region Properties
        public string Icon { get; }

        public float Size { get; } = 12;
        #endregion

        #region Properties.Static
        public static StiSymbolIcon Print => new StiSymbolIcon(0xE749);
        
        public static StiSymbolIcon Open => new StiSymbolIcon(0xED25);

        public static StiSymbolIcon Save => new StiSymbolIcon(0xE74E);
        
        public static StiSymbolIcon SendEMail => new StiSymbolIcon(0xE89C);

        public static StiSymbolIcon Bookmarks => new StiSymbolIcon(0xE8A4);
        
        public static StiSymbolIcon Parameters => new StiSymbolIcon(0xF306);

        public static StiSymbolIcon Thumbnails => new StiSymbolIcon(0xF308);

        public static StiSymbolIcon Find => new StiSymbolIcon(0xE721, 11);
        
        public static StiSymbolIcon FindPrev => new StiSymbolIcon(0xE74A, 11);
        
        public static StiSymbolIcon FindNext => new StiSymbolIcon(0xE74B, 11);
        
        public static StiSymbolIcon FindClose => new StiSymbolIcon(0xF13D, 12);

        public static StiSymbolIcon TextEditor => new StiSymbolIcon(0xE394, 12);        
        
        public static StiSymbolIcon Sign => new StiSymbolIcon(0xE61B, 11);

        public static StiSymbolIcon FullScreen => new StiSymbolIcon(0xE740, 11);
        
        public static StiSymbolIcon ZoomPageWidth => new StiSymbolIcon(0xE9A7);
        
        public static StiSymbolIcon ZoomOnePage => new StiSymbolIcon(0xE9A6);
        
        public static StiSymbolIcon ZoomTwoPages => new StiSymbolIcon(0xF1D4);
        
        public static StiSymbolIcon ZoomMultiplePages => new StiSymbolIcon(0xF233);        

        public static StiSymbolIcon DotMatrix => new StiSymbolIcon(0xF029);
        
        public static StiSymbolIcon PageNew => new StiSymbolIcon(0xE7C3);
        
        public static StiSymbolIcon PageDelete => new StiSymbolIcon(0xEFBA);
        
        public static StiSymbolIcon PageSize => new StiSymbolIcon(0xEFFC);
        
        public static StiSymbolIcon PageDesign => new StiSymbolIcon(0xEFB6);

        public static StiSymbolIcon PageFirst => new StiSymbolIcon(0xE892);
        
        public static StiSymbolIcon PagePrev => new StiSymbolIcon(0xF3E5);
        
        public static StiSymbolIcon PageNext => new StiSymbolIcon(0xE768);
        
        public static StiSymbolIcon PageLast => new StiSymbolIcon(0xE893);

        public static StiSymbolIcon Help => new StiSymbolIcon(0xE897, 11);
        
        public static StiSymbolIcon Remove => new StiSymbolIcon(0xE74D);
        
        public static StiSymbolIcon ExpandAll => new StiSymbolIcon(0xF859);
        
        public static StiSymbolIcon CollapseAll => new StiSymbolIcon(0xF85A);
        
        public static StiSymbolIcon FontBold => new StiSymbolIcon(0xE8DD);
        
        public static StiSymbolIcon FontItalic => new StiSymbolIcon(0xE8DB);
        
        public static StiSymbolIcon FontUnderline => new StiSymbolIcon(0xE8DC);
        
        public static StiSymbolIcon FontColor => new StiSymbolIcon(0xF4EC);
        
        public static StiSymbolIcon AlignLeft => new StiSymbolIcon(0xE8E4);
        
        public static StiSymbolIcon AlignCenter => new StiSymbolIcon(0xE8E3);
        
        public static StiSymbolIcon AlignRight => new StiSymbolIcon(0xE8E2);
        
        public static StiSymbolIcon AlignTop => new StiSymbolIcon(0xE3E0);
        
        public static StiSymbolIcon AlignMiddle => new StiSymbolIcon(0xE3E1);
        
        public static StiSymbolIcon AlignBottom => new StiSymbolIcon(0xE3E2);
        
        public static StiSymbolIcon Set => new StiSymbolIcon(0xE311, 11);
        
        public static StiSymbolIcon InsertText => new StiSymbolIcon(0xE616);
        
        public static StiSymbolIcon InsertImage => new StiSymbolIcon(0xECAB);
        
        public static StiSymbolIcon Clear => new StiSymbolIcon(0xE75C, 11);
        
        public static StiSymbolIcon Get => new StiSymbolIcon(0xF0D4, 11);

        public static StiSymbolIcon FullScreenClose => new StiSymbolIcon(0xE93B);

        public static StiSymbolIcon FullScreenOpen => new StiSymbolIcon(0xE93A);
        
        public static StiSymbolIcon FilterRemove => new StiSymbolIcon(0xEF8F, 11);

        public static StiSymbolIcon FilterOn => new StiSymbolIcon(0xF412, 11);

        public static StiSymbolIcon FilterOff => new StiSymbolIcon(0xE71C, 11);

        public static StiSymbolIcon Filter => new StiSymbolIcon(0xE71C, 10);

        public static StiSymbolIcon Refresh => new StiSymbolIcon(0xF4A3, 11);
        
        public static StiSymbolIcon Edit => new StiSymbolIcon(0xE70F, 10);
        
        public static StiSymbolIcon Download => new StiSymbolIcon(0xE896, 11);

        public static StiSymbolIcon ViewData => new StiSymbolIcon(0xF3F1);        
        
        public static StiSymbolIcon Sort => new StiSymbolIcon(0xE8CB, 9);

        public static StiSymbolIcon SelectColumns => new StiSymbolIcon(0xE890);
        
        public static StiSymbolIcon DrillDown => new StiSymbolIcon(0xF532, 11);
        
        public static StiSymbolIcon DrillUp => new StiSymbolIcon(0xEBCD, 11);

        public static StiSymbolIcon ZoomPlus => new StiSymbolIcon(0xE948, 10);

        public static StiSymbolIcon ZoomMinus => new StiSymbolIcon(0xE949, 10);

        public static StiSymbolIcon ZoomReset => new StiSymbolIcon(0xE423, 11);

        public static StiSymbolIcon Report => new StiSymbolIcon(0xE7C3, 11);

        public static StiSymbolIcon Dashboard => new StiSymbolIcon(0xE9F9, 11);

        public static StiSymbolIcon Form => new StiSymbolIcon(0xEE19, 11);
        #endregion

        public StiSymbolIcon(int icon, float size = 12)
        {
            Icon = ((char)icon).ToString();
            Size = size;
        }
    }
}