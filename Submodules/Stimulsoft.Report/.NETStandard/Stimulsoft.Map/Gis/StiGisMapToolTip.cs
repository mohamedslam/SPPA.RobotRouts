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

using Stimulsoft.Base;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Map.Gis
{
    [ToolboxItem(false)]
    public class StiGisMapToolTip : ToolTip
    {
        #region Propeties
        public string Body { get; set; }
        #endregion

        #region Handlers
        private void OnPopup(object sender, PopupEventArgs e) // use this event to set the size of the tool tip
        {
            e.ToolTipSize = MeasureSize();
        }

        private void OnDraw(object sender, DrawToolTipEventArgs e) // use this event to customise the tool tip
        {
            var g = e.Graphics;

            using (var backgroumd = new SolidBrush(Color.White))
            {
                g.FillRectangle(backgroumd, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width + StiScaleUI.XXI(10), e.Bounds.Height + StiScaleUI.YYI(10)));
            }

            using (var borderColor = new SolidBrush(Color.FromArgb(200, 200, 200)))
            using (var borderPen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(borderPen, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1));
            }

            var state = g.Save();

            try
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;

                int ellipseTopPos = e.Bounds.Y + StiScaleUI.YYI(10);
                using (var font = new Font("Microsoft Sans Serif", 9f))
                {
                    var rect = g.MeasureString(this.Body, font, StiScale.XXI(250));
                    g.DrawString(this.Body, font, Brushes.Black, new RectangleF(e.Bounds.X + StiScaleUI.YYI(6), e.Bounds.Y + StiScaleUI.YYI(6), rect.Width, rect.Height));
                }
            }
            finally
            {
                g.Restore(state);
            }
        }
        #endregion

        #region Methods
        private Size MeasureSize()
        {
            using (var img = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(img))
            using (var font = new Font("Microsoft Sans Serif", 9f))
            {
                var size = g.MeasureString(this.Body, font, StiScale.XXI(250));
                return new Size((int)size.Width + StiScaleUI.XXI(12), (int)size.Height + StiScaleUI.YYI(12));
            }
        }
        #endregion

        public StiGisMapToolTip()
        {
            this.OwnerDraw = true;
            this.Popup += this.OnPopup;
            this.Draw += this.OnDraw;
        }
    }
}
