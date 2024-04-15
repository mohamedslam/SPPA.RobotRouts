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

using Stimulsoft.Base.Drawing;
using System;

namespace Stimulsoft.Base.Wpf
{
    public static class StiWpfSkinHelper
    {
        static StiWpfSkinHelper()
        {
            var parameters = StiGuiParameters.LoadParameters();
            if (parameters != null)
            {
                CurrentForeground = parameters.Style;
                CurrentBackground = parameters.Background;
            }
            else
            {
                CurrentForeground = StiGuiParameters.DefaultForeground;
                CurrentBackground = StiGuiParameters.DefaultBackground;

                StiGuiParameters.SaveThemeAndStyle(CurrentForeground, CurrentBackground);
            }
        }

        #region Fields
        public static event EventHandler SkinChanged;
        #endregion

        #region Properties
        private static bool isWinFormsMode;
        internal static bool IsWinFormsMode
        {
            get
            {
                return isWinFormsMode;
            }
            set
            {
                isWinFormsMode = value;
                if (value)
                {
                    var newBackground = StiUX.Theme == StiControlTheme.Dark
                        ? StiSkinBackground.Black
                        : StiSkinBackground.White;

                    if (CurrentBackground != newBackground)
                    {
                        CurrentBackground = newBackground;
                        SkinChanged?.Invoke(null, null);
                    }
                }
            }
        }

        internal static bool DisableDarkMode { get; set; }

        public static StiSkinForeground CurrentForeground { get; private set; }

        public static StiSkinBackground CurrentBackground { get; private set; }
        #endregion

        #region Methods
        public static void ChangeThemeToWhite()
        {
            CurrentBackground = StiSkinBackground.White;
        }

        public static void ChangeDarkMode()
        {
            CurrentBackground = (CurrentBackground == StiSkinBackground.White)
                ? StiSkinBackground.Black
                : StiSkinBackground.White;

            StiGuiParameters.SaveThemeAndStyle(CurrentForeground, CurrentBackground);
            SkinChanged?.Invoke(null, null);
        }

        public static void ChangeDarkMode(StiControlTheme theme)
        {
            if (theme == StiControlTheme.Light)
                CurrentBackground = StiSkinBackground.White;
            else
                CurrentBackground = StiSkinBackground.Black;

            SkinChanged?.Invoke(null, null);
        }

        public static void SetNewSkin(StiSkinBackground background, StiSkinForeground foreground)
        {
            CurrentForeground = foreground;
            CurrentBackground = background;

            StiGuiParameters.SaveThemeAndStyle(foreground, background);
            SkinChanged?.Invoke(null, null);
        }
        #endregion
    }
}