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

using Stimulsoft.Base.Localization;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Stimulsoft.Base
{
    public partial class StiExceptionPreviewForm : Form
    {
        #region class ExceptionButton
        [ToolboxItem(false)]
        public class ExceptionButton : Button
        {
            #region Fields
            private bool isMouseOver;
            private bool isPressed;
            #endregion

            #region Properties
            public bool IsContinueButton { get; set; }
            #endregion

            #region Methods.override
            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                this.isMouseOver = true;
                this.Invalidate();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                this.isMouseOver = false;
                this.Invalidate();
            }

            protected override void OnMouseDown(MouseEventArgs mevent)
            {
                base.OnMouseDown(mevent);
                this.isPressed = true;
                this.Invalidate();
            }

            protected override void OnMouseUp(MouseEventArgs mevent)
            {
                base.OnMouseUp(mevent);
                this.isPressed = false;
                this.Invalidate();
            }

            protected override void OnPaint(PaintEventArgs pevent)
            {
                base.OnPaint(pevent);

                var g = pevent.Graphics;

                var rect = new Rectangle(0, 0, this.Width - StiScale.I(1), this.Height - StiScale.I(1));

                if (this.isPressed)
                {
                    using (var fill = new SolidBrush(Color.FromArgb(163, 189, 227)))
                        g.FillRectangle(fill, rect);
                    using (var pen = new Pen(Color.FromArgb(63, 110, 181)))
                        g.DrawRectangle(pen, rect);

                }
                else if (this.isMouseOver)
                {
                    using (var fill = new SolidBrush(Color.FromArgb(213, 225, 242)))
                        g.FillRectangle(fill, rect);
                    using (var pen = new Pen(Color.FromArgb(163, 189, 227)))
                        g.DrawRectangle(pen, rect);
                }
                else
                {
                    if (this.IsContinueButton)
                    {
                        using (var fill = new SolidBrush(Color.FromArgb(0, 121, 219)))
                            g.FillRectangle(fill, rect);
                        using (var pen = new Pen(Color.FromArgb(43, 87, 154)))
                            g.DrawRectangle(pen, rect);
                    }
                    else
                    {
                        g.FillRectangle(Brushes.White, rect);
                        using (var pen = new Pen(Color.FromArgb(163, 189, 227)))
                            g.DrawRectangle(pen, rect);
                    }
                }

                using (var foreground = new SolidBrush(this.ForeColor))
                using (var format = new StringFormat())
                {
                    format.LineAlignment = StringAlignment.Center;
                    format.Alignment = StringAlignment.Center;

                    g.DrawString(this.Text, this.Font, foreground, new RectangleF(new PointF(0, 0), new SizeF(Width, Height)), format);
                }
            }
            #endregion
        }
        #endregion

        public StiExceptionPreviewForm(Exception ex)
        {
            InitializeComponent();

            this.ShowIcon = false;
            this.ex = ex;

            if (StiScale.Id == SystemScaleID.x2)
            {
                var resources = new ComponentResourceManager(typeof(StiExceptionPreviewForm));
                this.imageMessage.Image = (Image)resources.GetObject("imageMessage_x2.Image");
            }

            Localize();
        }

        #region Fields
        private Exception ex;
        #endregion       

        #region Methods.Localize
        private void Localize()
        {
            this.Text = StiLocalization.Get("ExceptionProvider", "ExceptionReport");

            this.textBlockTitle.Text = ex?.Message;

            this.buttonContinue.Text = Loc.Get("DesignerFx", "Continue");
            this.buttonReport.Text = Loc.Get("Components", "StiReport");
        }
        #endregion

        #region Handlers.This
        private void This_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                this.Close();
            }
        }
        #endregion

        #region Handlers
        private void ButtonReport_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void ButtonContinue_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion
    }
}