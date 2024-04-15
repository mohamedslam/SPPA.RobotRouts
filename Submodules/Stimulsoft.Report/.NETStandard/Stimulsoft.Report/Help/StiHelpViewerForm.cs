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

using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Helpers;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Stimulsoft.Report.Help
{
    public partial class StiHelpViewerForm : Form, IStiThreadForm
    {
        private StiHelpViewerForm()
        {
            InitializeComponent();

            currentForm = this;

            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);

            this.StartPosition = FormStartPosition.Manual;

            StiFormSettings.Load(this);

            this.Text = StiLocalization.Get("HelpDesigner", "StimulsoftHelp");
        }

        #region Fields
        private System.Windows.Forms.Timer timerUrl;
        private static StiHelpViewerForm currentForm;
        private bool isCustomHelp;
        #endregion

        #region Properties
        internal static string Url { get; set; }
        #endregion

        #region Methods
        public void InvokeCustomHelp()
        {
            isCustomHelp = StiOptions.Engine.GlobalEvents.InvokeOpenCustomHelp(this);
        }

        private void LoadPosition()
        {
            StiSettings.Load();

            Point position;
            var size = new Size(800, 700);
            int screenNumber = StiSettings.GetInt("HelpViewerForm", "ScreenNumber", -999);

            if (screenNumber == -999)
            {
                var screenRect = Rectangle.Empty;
                if (Screen.AllScreens.Length == 1 || ActiveForm == null)
                {
                    screenRect = Screen.PrimaryScreen.Bounds;
                }
                else
                {
                    Point p;
                    Size formSize;
                    var activeForm = ActiveForm;

                    if (activeForm.WindowState == FormWindowState.Maximized)
                    {
                        p = activeForm.RestoreBounds.Location;
                        formSize = activeForm.RestoreBounds.Size;
                    }
                    else
                    {
                        p = activeForm.Location;
                        formSize = activeForm.Size;
                    }

                    bool isFind = false;
                    for (int index = 0; index < Screen.AllScreens.Length; index++)
                    {
                        var screen = Screen.AllScreens[index];
                        if (screen.Bounds.IntersectsWith(new Rectangle(p, formSize)))
                        {
                            screenRect = screen.Bounds;
                            isFind = true;
                            break;
                        }
                    }

                    if (!isFind)
                        screenRect = Screen.PrimaryScreen.Bounds;
                }

                position = new Point(screenRect.Right - size.Width, screenRect.Top + (screenRect.Height - size.Height) / 2);

                base.SetBoundsCore(position.X, position.Y, size.Width, size.Height, BoundsSpecified.All);
                return;
            }

            int primaryIndex = StiFormHelper.GetPrimaryScreenNumber();
            position = new Point(StiSettings.GetInt("HelpViewerForm", "WindowXPos", 0), StiSettings.GetInt("HelpViewerForm", "WindowYPos", 0));
            size.Width = StiSettings.GetInt("HelpViewerForm", "WindowWidth", size.Width);
            size.Height = StiSettings.GetInt("HelpViewerForm", "WindowHeight", size.Height);

            Screen currentScreen;
            if (screenNumber != primaryIndex)
            {
                if (screenNumber + 1 > Screen.AllScreens.Length)
                {
                    currentScreen = Screen.PrimaryScreen;
                    position = currentScreen.Bounds.Location;
                }
                else
                {
                    currentScreen = Screen.AllScreens[screenNumber];
                    if (!currentScreen.Bounds.Contains(new Rectangle(position, size)))
                    {
                        position = currentScreen.Bounds.Location;
                    }
                }
            }
            else
            {
                currentScreen = Screen.PrimaryScreen;

                if (StiSettings.GetInt("HelpViewerForm", "ScreenNumber", -1) == -1)
                {
                    var screenRect = currentScreen.Bounds;
                    position = new Point(screenRect.Right - size.Width, screenRect.Top + screenRect.Height - size.Height);
                }
            }

            base.SetBoundsCore(position.X, position.Y, size.Width, size.Height, BoundsSpecified.All);
        }

        private void SetUrl(string url)
        {
            if (isCustomHelp)
            {
                currentForm.webBrowser.Url = new Uri(url); 
                return;
            }

            string file = $"{StiOptions.Configuration.ApplicationDirectory}\\Help\\{StiLocalization.CultureName}\\{url}";
            if (File.Exists(file))
                currentForm.webBrowser.Url = new Uri(file, UriKind.Absolute);

            else
            {
                string language;
                switch (StiLocalization.CultureName)
                {
                    case "ru":
                        language = "ru";
                        break;

                    default:
                        language = "en";
                        break;
                }

                url = $"https://www.stimulsoft.com/{language}/documentation/online/{url}";
                currentForm.webBrowser.Url = new Uri(url);
            }
        }
        #endregion

        #region Methods
        public static void Show(string url)
        {
            if (currentForm == null)
            {
                var thread = new Thread(RunHelp);
                thread.TrySetApartmentState(ApartmentState.STA);
                thread.Name = "StiHelp";
                thread.Start();
            }

            Url = url;
        }

        private static void RunHelp()
        {
            Application.Run(new StiHelpViewerForm());
        }
        #endregion

        #region Handlers.This
        private void This_FormClosed(object sender, FormClosedEventArgs e)
        {
            StiFormSettings.Save(this);
            currentForm = null;
        }

        private void This_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InvokeCustomHelp();

            if (Url != null)
            {
                string url = Url;
                Url = null;

                this.SetUrl(url);
            }

            timerUrl = new System.Windows.Forms.Timer
            {
                Interval = 1500
            };
            timerUrl.Tick += TimerUrl_Tick;
            timerUrl.Start();
        }
        #endregion

        #region Handlers
        private void TimerUrl_Tick(object sender, EventArgs e)
        {
            timerUrl.Stop();

            if (Url != null)
            {
                string url = Url;
                Url = null;

                this.SetUrl(url);
                this.Activate();
            }

            timerUrl.Start();
        }

        private void WebBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }
        #endregion        
    }
}