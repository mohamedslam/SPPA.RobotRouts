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

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Stimulsoft.Base.Drawing
{
    public class StiUX
    {
        #region Properties.Static
        public static StiControlTheme Theme { get; set; } = StiControlTheme.Light;

        public static StiUXIconSet IconSet { get; set; } = StiUXIconSet.Regular;

        public static bool IsDark => Theme == StiControlTheme.Dark;

        public static bool IsBaseAccent { get; }

        public static int DefaultTabHeaderHeight => StiScaleUI.I(30);
        
        public static int EditorTabHeaderHeight => StiScaleUI.I(36);

        public static int DefaultTabHeaderWidth => StiScaleUI.I(100);

        public static int DefaultBoxHeight => StiScaleUI.I(24);//24-minimum for datetimepicker

        public static int DefaultButtonPanelHeight => StiScaleUI.I(50);

        public static int DefaultButtonHeight => StiScaleUI.I(26);

        public static int DefaultLongRangeParameterWidth => StiScaleUI.I(350);

        public static int DefaultParameterWidth => StiScaleUI.I(230);

        public static int DefaultParameterHeight => StiScaleUI.I(28);

        public static int DefaultToolbarHeight => StiScaleUI.I(32);

        public static int DefaultCheckBoxSize => StiScaleUI.I(15);

        public static int DefaultRadioButtonSize => StiScaleUI.I(15);

        public static int DefaultSwitchBoxWidth => StiScaleUI.I(38);

        public static int DefaultSwitchBoxHeight => StiScaleUI.I(16);

        public static int DefaultItemHeight => StiScaleUI.I(25);

        public static int DefaultTreeNodeHeight => StiScaleUI.I(20);

        public static int DefaultDropDownButtonWidth => StiScaleUI.I(20);

        public static float DefaultInputBoxCornerRadius { get; set; } = 3;

        public static float DefaultGroupBoxCornerRadius { get; set; } = 3;

        public static float DefaultCheckBoxCornerRadius { get; set; } = 2;

        public static float DefaultButtonCornerRadius { get; set; } = 3;

        public static float DefaultItemCornerRadius { get; set; } = 2;
        #endregion

        #region Properties.General
        public static Color Accent => IsBaseAccent ? GetColor("#038387") : GetColor("#3d6bba");

        public static Color BackgroundDark => GetColor("#2b2b2b", "#fafafa");

        public static Color Background => GetColor("#333333", "#ffffff");

        public static Color HeaderForeground => GetColor("#a7a7a7", "#767676");

        public static Color HeaderBackground => GetColor("#484848", "#f3f3f3");

        public static Color Separator => GetColor("#4f4f4f", "#e0e0e0");

        public static Color Foreground => GetColor("#a7a7a7", "#767676");

        public static Color Hyperlink => IsBaseAccent ? StiColorUtils.Light(Accent, 5) : GetColor("#23a6e8", "#2c79d0");

        public static Color HyperlinkMouseOver => IsBaseAccent ? StiColorUtils.Light(Accent, 19) : GetColor("#79cdfe", "#2261a6");

        public static Color ErrorForeground => GetColor("#ffffff");

        public static Color ErrorBackground => GetColor("#eb5558");
        #endregion

        #region Properties.Ribbon
        public static Color RibbonBorder => IsDark ? GetColor("#797775") : Accent;

        public static Color RibbonCaption => IsBaseAccent ? StiColorUtils.Dark(Accent, 23) : GetColor("#0a0a0a", "#2b579a");

        public static Color RibbonControl => GetColor("#323130", "#ffffff");

        public static Color RibbonWorkspace => GetColor("#2b2b2b", "#ffffff");

        public static Color RibbonStartMenu => IsBaseAccent ? StiColorUtils.Dark(Accent, 23) : GetColor("#363636", "#2b579a");

        public static Color RibbonStartMenuSelected => IsBaseAccent ? StiColorUtils.Dark(Accent, 60) : GetColor("#2b579a", "#002050");

        public static Color RibbonStartMenuMouseOver => IsBaseAccent ? StiColorUtils.Dark(Accent, 40) : GetColor("#124078", "#124078");

        public static Color RibbonStartMenuSelectedMouseOver => IsBaseAccent ? StiColorUtils.Dark(Accent, 30) : GetColor("#124078", "#124078");

        public static Color RibbonSeparator => GetColor("#484644", "#d8d6d4");

        public static Color RibbonBarText => GetColor("#ffffff", "#484644");

        public static Color RibbonStatusBarText => GetColor("#ffffff");

        public static Color RibbonBarDialog => GetColor("#ffffff", "#686664");
        #endregion

        #region Properties.GroupBox
        public static Color GroupBoxBorder => GetColor("#4f4f4f", "#c0c0c0");

        public static Color GroupBoxBorderDisabled => GetColor("#4e4e4e", "#dcdcdc");

        public static Color GroupBoxForeground => GetColor("#ffffff", "#2b2b2b");

        public static Color GroupBoxForegroundDisabled => GetColor("#727272", "#dcdcdc");
        #endregion

        #region Properties.Track
        public static Color TrackBackground => GetColor("#474747", "#bbbcbc");

        public static Color TrackBackgroundDisabled => GetColor("#626262", "#e5e5e5");

        public static Color TrackIndicator => GetColor("#888888", "#ffffff");

        public static Color TrackIndicatorDisabled => GetColor("#b1b1b1", "#d5d5d5");

        public static Color TrackIndicatorMouseOver => GetColor("#aaaaaa", "#eeeeee");
        #endregion

        #region Properties.PropertyGrid
        public static Color PropertyGridSeparator => GetColor("#2e2e2e", "#e2e2e2");

        public static Color PropertyGridForeground => GetColor("#e5e5e5", "#1e1e1e");

        public static Color PropertyGridBackground => GetColor("#262626", "#f5f5f5");

        public static Color PropertyGridBackgroundSelected => Accent;

        public static Color PropertyGridForegroundSelected => GetColor("#ffffff");

        public static Color PropertyGridViewBackground => GetColor("#1f1f1f", "#ffffff");

        public static Color PropertyGridViewForeground => GetColor("#e5e5e5", "#1e1e1e");
        #endregion

        #region Properties.Button
        public static Color ButtonAppBackgroundActive => IsBaseAccent ? StiColorUtils.Dark(Accent, 23) : GetColor("#2b579a");

        public static Color ButtonBackgroundActive => Accent;

        public static Color ButtonBackgroundActiveMouseOver => IsBaseAccent ? StiColorUtils.Light(Accent, 13) : GetColor("#4377d0");

        public static Color ButtonBackgroundActivePressed => IsBaseAccent ? StiColorUtils.Dark(Accent, 16) : GetColor("#355da0");

        public static Color ButtonBackgroundActiveDisabled => GetColor("#2e4961", "#b8d7f0");

        public static Color ButtonBackground => GetColor("#4e4e4e", "#dddddd");

        public static Color ButtonBackgroundMouseOver => GetColor("#555555", "#d5d5d5");

        public static Color ButtonBackgroundChecked => GetColor("#4e4e4e", "#dddddd");

        public static Color ButtonBackgroundPressed => GetColor("#484848", "#e2e2e2");

        public static Color ButtonBackgroundDisabled => GetColor("#333333", "#ffffff");

        public static Color ButtonForegroundActive => GetColor("#ffffff");

        public static Color ButtonForegroundActiveDisabled => GetColor("#757575", "#ffffff");

        public static Color ButtonForeground => GetColor("#ffffff", "#2b2b2b");

        public static Color ButtonForegroundDisabled => GetColor("#646464", "#dcdcdc");

        public static Color ButtonBorder => GetColor("#5a5a5a", "#aeaeae");

        public static Color ButtonBorderChecked => Accent;

        public static Color ButtonBorderFocused => GetColor("#909090", "#838383");

        public static Color ButtonBorderActiveFocused => IsBaseAccent ? StiColorUtils.Dark(Accent, 30) : GetColor("#264c7d");

        public static Color ButtonBorderDisabled => GetColor("#4e4e4e", "#dcdcdc");
        #endregion

        #region Properties
        public static Color GridHeaderBackground => GetColor("#3e3e3e", "#f0f0f0");

        public static Color GridHeaderBackgroundSelected => GetColor("#4e4e4e", "#d2d2d2");

        public static Color GridHeaderBackgroundMouseOver => GetColor("#555555", "#e0e0e0");

        public static Color GridHeaderBackgroundActiveMouseOver => IsBaseAccent ? Accent : GetColor("#4377d0");

        public static Color GridBackgroundSelected => Accent;

        public static Color GridForeground => GetColor("#ffffff", "#2b2b2b");

        public static Color GridForegroundActiveSelected => GetColor("#ffffff");

        public static Color GridCellBackground => GetColor("#2b2b2b", "#ffffff");

        public static Color GridCellBackgroundError => GetColor("#454545", "#e5e5e5");

        public static Color GridSeparator => GetColor("#4f4f4f", "#e0e0e0");
        #endregion

        #region Properties.Check
        public static Color CheckBorder => GetColor("#858585", "#929292");

        public static Color CheckBorderMouseOver => GetColor("#acacac", "#6b6b6b");

        public static Color CheckBorderPressed => GetColor("#909090", "#a9a9a9");

        public static Color CheckBorderFocused => Accent;

        public static Color CheckBorderDisabled => GetColor("#666666", "#dcdcdc");

        public static Color CheckBackground => GetColor("#3a3a3a", "#ffffff");

        public static Color CheckBackgroundMouseOver => GetColor("#4a4a4a", "#f5f5f5");
        #endregion

        #region Properties.TabControl
        public static Color TabHeaderBackground => GetColor("#333333", "#ffffff");

        public static Color TabHeaderBackgroundMouseOver => GetColor("#404040", "#eaeaea");

        public static Color TabHeaderBackgroundPressed => GetColor("#3b3b3b", "#efefef");

        public static Color TabHeaderBackgroundSelected => GetColor("#454545", "#e5e5e5");

        public static Color TabHeaderForeground => GetColor("#ffffff", "#262626");

        public static Color TabHeaderForegroundMouseOver => GetColor("#ffffff", "#262626");

        public static Color TabHeaderForegroundPressed => GetColor("#ffffff", "#262626");

        public static Color TabHeaderForegroundSelected => GetColor("#ffffff", "#262626");

        public static Color TabHeaderSelection => Accent;

        public static Color TabPageBackground => GetColor("#333333", "#ffffff");

        public static Color TabBorder => GetColor("#4f4f4f", "#e0e0e0");
        #endregion

        #region Properties.Designer
        public static Color DesignerWorkspaceBackground => StiUX.GetColor("#262626", "#f5f5f5");

        public static Color DesignerRulerForeground => StiUX.GetColor("#ffffff", "#525252");

        public static Color DesignerRulerLightBackground => StiUX.GetColor("#3b3b3b", "#ffffff");

        public static Color DesignerRulerDarkBackground => StiUX.GetColor("#0a0a0a", "#e4e4e4");

        public static Color DesignerRulerBorder => StiUX.GetColor("#505050", "#e2e2e2");

        public static Color DesignerRulerSelection => StiUX.GetColor("#5b5b5b", "#c4c4c4");
        #endregion

        #region Properties.Item
        public static Color ItemBackground => GetColor("#2b2b2b", "#ffffff");

        public static Color ItemBackgroundMouseOver => GetColor("#505050", "#dadada");

        public static Color ItemBackgroundPressed => GetColor("#3b3b3b", "#efefef");

        public static Color ItemBackgroundSelected => GetColor("#454545", "#e5e5e5");

        public static Color ItemBackgroundActiveSelected => Accent;

        public static Color ItemBackgroundActiveMouseOver => IsBaseAccent ? StiColorUtils.Light(Accent, 13) : GetColor("#4377d0");

        public static Color ItemBackgroundDisabled => GetColor("#333333", "#ffffff");

        public static Color ItemGroupBackground => GetColor("#484848", "#f3f3f3");

        public static Color ItemSeparator => GetColor("#4f4f4f", "#e0e0e0");

        public static Color ItemBorder => GetColor("#4b4b4b", "#aaaaaa");

        public static Color ItemForeground => GetColor("#ffffff", "#26262d");

        public static Color ItemForegroundMouseOver => GetColor("#ffffff", "#26262d");

        public static Color ItemForegroundSelected => GetColor("#ffffff", "#26262d");

        public static Color ItemForegroundActiveMouseOver => GetColor("#ffffff");

        public static Color ItemForegroundActiveSelected => GetColor("#ffffff");

        public static Color ItemForegroundDisabled => GetColor("#727272", "#bcbcbc");

        public static Color ItemSelection => IsBaseAccent ? StiColorUtils.Light(Accent, 40) : GetColor("#5da1de");
        #endregion

        #region Properties.Input
        public static Color InputBorder => GetColor("#bebebe", "#929292");

        public static Color InputBorderMouseOver => GetColor("#a2a2a2", "#6b6b6b");

        public static Color InputBorderFocused => Accent;

        public static Color InputBorderDisabled => GetColor("#4e4e4e", "#dcdcdc");

        public static Color InputButtonBackgroundMouseOver => GetColor("#575757", "#f2f2f2");

        public static Color InputButtonBackgroundReadOnly => GetColor("#525252", "#f7f7f7");

        public static Color InputButtonBackgroundPressed => GetColor("#525252", "#f7f7f7");

        public static Color InputBackgroundMouseOver => GetColor("#5e5e5e", "#e3e3e3");

        public static Color InputBackground => GetColor("#2b2b2b", "#ffffff");

        public static Color InputBackgroundDisabled => GetColor("#333333", "#ffffff");

        public static Color InputBackgroundReadOnly => GetColor("#3b3b3b", "#ffffff");

        public static Color InputForeground => GetColor("#ffffff", "#2b2b2b");

        public static Color InputForegroundDisabled => GetColor("#727272", "#acacac");

        public static Color InputForegroundReadOnly => GetColor("#727272", "#c0c0c0");

        public static Color InputForegroundHint => GetColor("#878787", "#b6b6b6");

        public static Color InputForegroundHintDisabled => GetColor("#5b5b5b", "#d6d6d6");

        public static Color InputGlyph => GetColor("#ffffff", "#2b2b2b");

        public static Color InputGlyphDisabled => GetColor("#4e4e4e", "#ececec");

        public static Color InputGlyphReadOnly => GetColor("#ababaa", "#9e9e9e");
        #endregion

        #region Methods.Import
        [DllImport("uxtheme.dll", EntryPoint = "#95")]
        private static extern uint GetImmersiveColorFromColorSetEx(uint dwImmersiveColorSet, uint dwImmersiveColorType, bool bIgnoreHighContrast, uint dwHighContrastCacheMode);

        [DllImport("uxtheme.dll", EntryPoint = "#96")]
        private static extern uint GetImmersiveColorTypeFromName(IntPtr pName);

        [DllImport("uxtheme.dll", EntryPoint = "#98")]
        private static extern int GetImmersiveUserColorSetPreference(bool bForceCheckRegistry, bool bSkipCheckOnFail);
        #endregion

        #region Methods.Helpers
        public static void NextTheme()
        {
            switch (Theme)
            {
                case StiControlTheme.Light:
                    Theme = StiControlTheme.Dark;
                    break;

                case StiControlTheme.Dark:
                    Theme = StiControlTheme.Light;
                    break;
            }
        }
 
        public static Color GetColor(string colorDark, string colorLight = null)
        {
            if (colorLight == null)
                return StiColor.Get(colorDark);

            switch (Theme)
            {
                case StiControlTheme.Dark:
                    return StiColor.Get(colorDark);

                default:
                    return StiColor.Get(colorLight);
            }
        }

        public static Color GetThemeColor(string color)
        {
            var colorSetEx = GetImmersiveColorFromColorSetEx(
                (uint)GetImmersiveUserColorSetPreference(false, false),
                GetImmersiveColorTypeFromName(Marshal.StringToHGlobalUni(color)),
                false, 0);

            var r = (byte)(0x000000FF & colorSetEx);
            var g = (byte)((0x0000FF00 & colorSetEx) >> 8);
            var b = (byte)((0x00FF0000 & colorSetEx) >> 16);
            var a = (byte)((0xFF000000 & colorSetEx) >> 24);

            return Color.FromArgb(a, r, g, b);
        }
        #endregion
    }
}
