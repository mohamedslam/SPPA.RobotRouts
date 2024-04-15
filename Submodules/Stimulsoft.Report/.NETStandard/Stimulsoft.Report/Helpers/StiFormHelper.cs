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
using System;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Helpers
{
    public static class StiFormHelper
    {
        #region Methods
        public static void LoadStateForm(Form form, string key, ref Point position, ref Size size, bool allowMaximize = false)
        {
            StiSettings.Load();

            int primaryIndex = GetPrimaryScreenNumber();
            if (string.IsNullOrEmpty(key))
                key = form.GetType().Name;

            int screenNumber = StiSettings.GetInt(key, "ScreenNumber", primaryIndex);
            position.X = StiScale.I(StiSettings.GetInt(key, "WindowXPos", 0));
            position.Y = StiScale.I(StiSettings.GetInt(key, "WindowYPos", 0));
            size.Width = StiScale.I(StiSettings.GetInt(key, "WindowWidth", size.Width));
            size.Height = StiScale.I(StiSettings.GetInt(key, "WindowHeight", size.Height));

            if (Screen.AllScreens.Length == 1)
            {
                position.X = Math.Max(0, position.X);
                position.Y = Math.Max(0, position.Y);
                position.X = Math.Min(Screen.PrimaryScreen.WorkingArea.Width, position.X);
                position.Y = Math.Min(Screen.PrimaryScreen.WorkingArea.Height, position.Y);
            }

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

                if (StiSettings.GetInt(key, "ScreenNumber", -1) == -1)
                    position = currentScreen.Bounds.Location;
            }

            if (allowMaximize)
            {
                if (StiSettings.GetInt(key, "WindowXPos", int.MaxValue) == int.MaxValue && 
                    StiSettings.GetInt(key, "WindowYPos", int.MaxValue) == int.MaxValue)
                {
                    Rectangle rect = currentScreen.WorkingArea;
                    position = new Point(rect.Left + ((rect.Width - size.Width) / 2), rect.Top + ((rect.Height - size.Height) / 2));
                }

                form.WindowState = StiSettings.GetBool(key, "IsMaximize", true)
                    ? FormWindowState.Maximized
                    : FormWindowState.Normal;

                return;
            }

            if (form.WindowState == FormWindowState.Normal &&
                StiSettings.GetInt(key, "WindowXPos", int.MaxValue) == int.MaxValue && 
                StiSettings.GetInt(key, "WindowYPos", int.MaxValue) == int.MaxValue)
            {
                Rectangle rect = currentScreen.WorkingArea;
                position = new Point(rect.Left + ((rect.Width - size.Width) / 2), rect.Top + ((rect.Height - size.Height) / 2));
            }
        }

        public static void LoadStateForm(Form form, string key, ref Point position, ref Size size, 
            ref FormWindowState wndState, bool allowMaximize = false)
        {
            StiSettings.Load();

            int primaryIndex = GetPrimaryScreenNumber();
            if (string.IsNullOrEmpty(key)) key = form.GetType().Name;

            int screenNumber = StiSettings.GetInt(key, "ScreenNumber", primaryIndex);
            position.X = StiScale.I(StiSettings.GetInt(key, "WindowXPos", 0));
            position.Y = StiScale.I(StiSettings.GetInt(key, "WindowYPos", 0));
            size.Width = StiScale.I(StiSettings.GetInt(key, "WindowWidth", size.Width));
            size.Height = StiScale.I(StiSettings.GetInt(key, "WindowHeight", size.Height));

            if (Screen.AllScreens.Length == 1)
            {
                position.X = Math.Max(0, position.X);
                position.Y = Math.Max(0, position.Y);
                position.X = Math.Min(Screen.PrimaryScreen.WorkingArea.Width, position.X);
                position.Y = Math.Min(Screen.PrimaryScreen.WorkingArea.Height, position.Y);
            }

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

                if (StiSettings.GetInt(key, "ScreenNumber", -1) == -1)
                    position = currentScreen.Bounds.Location;
            }

            if (allowMaximize)
            {
                if (StiSettings.GetInt(key, "WindowXPos", int.MaxValue) == int.MaxValue && 
                    StiSettings.GetInt(key, "WindowYPos", int.MaxValue) == int.MaxValue)
                {
                    Rectangle rect = currentScreen.WorkingArea;
                    position = new Point(rect.Left + ((rect.Width - size.Width) / 2), rect.Top + ((rect.Height - size.Height) / 2));
                }

                wndState = StiSettings.GetBool(key, "IsMaximize", true)
                    ? FormWindowState.Maximized
                    : FormWindowState.Normal;
                return;
            }

            if (form.WindowState == FormWindowState.Normal &&
                StiSettings.GetInt(key, "WindowXPos", int.MaxValue) == int.MaxValue && 
                StiSettings.GetInt(key, "WindowYPos", int.MaxValue) == int.MaxValue)
            {
                Rectangle rect = currentScreen.WorkingArea;
                position = new Point(rect.Left + ((rect.Width - size.Width) / 2), rect.Top + ((rect.Height - size.Height) / 2));
            }
        }

        public static void SaveStateForm(Form form, string key, bool allowMaximize = false)
        {
            Point p;
            Size formSize;
            int screenNumber = GetScreenNumber(form, out p, out formSize);

            if (string.IsNullOrEmpty(key)) 
                key = form.GetType().Name;

            StiSettings.Load();

            if (allowMaximize)
                StiSettings.Set(key, "IsMaximize", form.WindowState == FormWindowState.Maximized);

            StiSettings.Set(key, "ScreenNumber", screenNumber);

            if (form.WindowState != FormWindowState.Minimized)
            {   
                StiSettings.Set(key, "WindowXPos", (int)(p.X / StiScale.Factor));
                StiSettings.Set(key, "WindowYPos", (int)(p.Y / StiScale.Factor));
                StiSettings.Set(key, "WindowWidth", (int)(formSize.Width / StiScale.Factor));
                StiSettings.Set(key, "WindowHeight", (int)(formSize.Height / StiScale.Factor));
            }
            StiSettings.Save();
        }
        #endregion

        #region Methods.Helpers
        private static int GetScreenNumber(Form form, out Point p, out Size formSize)
        {
            if (form.WindowState == FormWindowState.Maximized)
            {
                p = form.RestoreBounds.Location;
                formSize = form.RestoreBounds.Size;
            }
            else
            {
                p = form.Location;
                formSize = form.Size;
            }

            if (Screen.AllScreens.Length > 1)
            {
                for (int index = 0; index < Screen.AllScreens.Length; index++)
                {
                    var screen = Screen.AllScreens[index];
                    if (screen.Bounds.IntersectsWith(new Rectangle(p, formSize)))
                        return index;
                }
            }

            return GetPrimaryScreenNumber();
        }

        public static int GetPrimaryScreenNumber()
        {
            int index = 0;
            int primaryIndex = 0;
            while (index < Screen.AllScreens.Length)
            {
                if (Screen.AllScreens[index].Primary)
                {
                    primaryIndex = index;
                    break;
                }
                index++;
            }

            return primaryIndex;
        }
        #endregion
    }
}