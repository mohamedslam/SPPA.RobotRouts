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

using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
    public class StiWinControlV2Builder : StiComponentV2Builder
    {
        public override StiComponent InternalRender(StiComponent masterComp)
        {
            var masterWinControl = masterComp as StiWinControl;

            var imageComp = new StiImage(masterWinControl.ClientRectangle);
            imageComp.PutImageToDraw(masterWinControl.GetControlImage());

            imageComp.GetPointer += masterWinControl.InvokeGetPointer;
            imageComp.GetBookmark += masterWinControl.InvokeGetBookmark;
            imageComp.GetTag += masterWinControl.InvokeGetTag;
            imageComp.Click += masterWinControl.InvokeClick;
            imageComp.MouseEnter += masterWinControl.InvokeMouseEnter;
            imageComp.MouseLeave += masterWinControl.InvokeMouseLeave;

            imageComp.DockStyle = masterWinControl.DockStyle;
            imageComp.Name = masterWinControl.Name;
            imageComp.CanGrow = masterWinControl.CanGrow;
            imageComp.CanShrink = masterWinControl.CanShrink;

            imageComp.PointerValue = masterWinControl.PointerValue;
            imageComp.BookmarkValue = masterWinControl.BookmarkValue;
            imageComp.HyperlinkValue = masterWinControl.HyperlinkValue;
            imageComp.ToolTipValue = masterWinControl.ToolTipValue;
            imageComp.TagValue = masterWinControl.TagValue;
            imageComp.InvokeEvents();

            return imageComp;
        }
    }
}
