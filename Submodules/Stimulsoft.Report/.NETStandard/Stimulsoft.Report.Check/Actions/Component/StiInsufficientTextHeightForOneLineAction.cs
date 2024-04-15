#region Copyright (C) 2003-2021 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
{																	}
{	Copyright (C) 2003-2021 Stimulsoft     							}
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
#endregion Copyright (C) 2003-2021 Stimulsoft

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helper;
using System;
using System.Drawing;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Check
{
    public class StiInsufficientTextHeightForOneLineAction : StiAction
    {
        public override string Name => StiLocalizationExt.Get("CheckActions", "StiInsufficientTextHeightForOneLineShort");

        public override string Description => StiLocalizationExt.Get("CheckActions", "StiInsufficientTextHeightForOneLineLong");

        public override void Invoke(StiReport report, object element, string elementName)
        {
            base.Invoke(report, null, null);

            var comp = element as StiText;
            if (comp == null) return;

            SizeF size;
            using (Bitmap bmp = new Bitmap(1, 1))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    var tempFont = comp.Font;
                    if (StiDpiHelper.NeedDeviceCapsScale)
                    {
                        tempFont = StiFontUtils.ChangeFontSize(tempFont, (float)(comp.Font.Size * StiDpiHelper.DeviceCapsScale));
                    }
                    size = g.MeasureString("a", tempFont);
                }
            }
            var newHeight = comp.Report.Unit.ConvertFromHInches(size.Height);
            comp.Height = newHeight;
            if (comp.Height < newHeight) comp.Height += 0.01;

            if (report?.Designer?.Info != null && report.Designer.Info.AlignToGrid)
            {
                var c = Math.Ceiling(comp.Height / report.Designer.Info.GridSize);
                comp.Height = c * report.Designer.Info.GridSize;
            }
            else if (report?.Info != null && report.Info.AlignToGrid)
            {
                var c = Math.Ceiling(comp.Height / report.Info.GridSize);
                comp.Height = c * report.Info.GridSize;
            }
        }
    }
}