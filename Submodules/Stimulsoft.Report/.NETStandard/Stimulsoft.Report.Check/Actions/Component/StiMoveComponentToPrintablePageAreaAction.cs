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
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiMoveComponentToPrintablePageAreaAction : StiAction
    {
        public override string Name => StiLocalizationExt.Get("CheckActions", "StiMoveComponentToPrintablePageAreaActionShort");

        public override string Description => StiLocalizationExt.Get("CheckActions", "StiMoveComponentToPrintablePageAreaActionLong");

        public override void Invoke(StiReport report, object element, string elementName)
        {
            base.Invoke(report, null, null);

            var comp = element as StiComponent;
            if (comp == null) return;

            var page = comp.Page;
            if (page == null) return;

            var compRect = comp.GetPaintRectangle(false, false);
            var pageRect = page.ClientRectangle;

            if (compRect.Left < pageRect.Left)
            {
                comp.Left = 0;
                compRect.X = 0;
            }
            
            if (compRect.Top < pageRect.Top)
            {
                comp.Top = 0;
                compRect.Y = 0;
            }

            if (compRect.Right > pageRect.Right)
            {
                var offset = compRect.Right - pageRect.Right;
                comp.Left -= offset;
                compRect.X -= offset;
                if (compRect.Left < pageRect.Left)
                {
                    comp.Width -= pageRect.Left - compRect.Left;
                    comp.Left = 0;
                }
            }

            if (compRect.Bottom > pageRect.Bottom)
            {
                var offset = compRect.Bottom - pageRect.Bottom;
                comp.Top -= offset;
                compRect.Y -= offset;
                if (compRect.Top < pageRect.Top)
                {
                    comp.Height -= pageRect.Top - compRect.Top;
                    comp.Top = 0;
                }
            }
        }
    }
}
