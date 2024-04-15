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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Licenses;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.QuickButtons;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
    {
        /// <summary>
        /// Class which allows adjustment of the Designer of the report.
        /// </summary>
        public sealed partial class Designer
        {
            #region Fields
            private static bool designerSpecificationLoaded = false;
            #endregion

            #region Methods
            public static event StiGetCulturesEventHandler GetCulturesListForGlobalizationEditor;
            public static CultureInfo[] InvokeGetCulturesListForGlobalizationEditor()
            {
                if (GetCulturesListForGlobalizationEditor == null) return null;

                var args = new StiGetCulturesEventArgs();
                GetCulturesListForGlobalizationEditor(null, args);
                return args.Cultures;
            }
            #endregion

            #region Properties.States
            /// <summary>
            /// Returns true if the designer specification equal to Beginner. Otherwise, returns false.
            /// </summary>
            [Browsable(false)]
            public static bool IsBeginnerDesigner
            {
                get
                {
                    switch (DesignerSpecification)
                    {
                        case StiDesignerSpecification.Beginner:
                            return true;

                        case StiDesignerSpecification.BICreator:
                            return false;

                        case StiDesignerSpecification.Developer:
                            return false;

                        default:
                            return false;
                    }
                }
            }

            /// <summary>
            /// Returns true if the designer specification equal to Creator. Otherwise, returns false.
            /// </summary>
            [Browsable(false)]
            public static bool IsBICreatorDesigner
            {
                get
                {
                    switch (DesignerSpecification)
                    {
                        case StiDesignerSpecification.Beginner:
                            return false;

                        case StiDesignerSpecification.BICreator:
                            return true;

                        case StiDesignerSpecification.Developer:
                            return false;

                        default:
                            if (StiStimulsoftApps.IsDesktopApp)
                                return true;

                            if (string.IsNullOrEmpty(StiLicense.Key))
                                return false;

                            return StiLicenseKey.Get(StiLicense.Key)
                                .Products.All(p => (
                                    p.Ident == StiProductIdent.BIDesigner ||
                                    p.Ident == StiProductIdent.BIDesktop ||
                                    p.Ident == StiProductIdent.BIServer ||
                                    p.Ident == StiProductIdent.BICloud ||
                                    p.Ident == StiProductIdent.CloudDashboards ||
                                    p.Ident == StiProductIdent.CloudReports));
                    }
                }
            }

            /// <summary>
            /// Returns true if the designer specification equal to Developer. Otherwise, returns false.
            /// </summary>
            [Browsable(false)]
            public static bool IsDeveloperDesigner
            {
                get
                {
                    switch (DesignerSpecification)
                    {
                        case StiDesignerSpecification.Beginner:
                            return false;

                        case StiDesignerSpecification.BICreator:
                            return false;

                        case StiDesignerSpecification.Developer:
                            return true;

                        default:
                            if (StiStimulsoftApps.IsDesktopApp)
                                return false;

                            if (string.IsNullOrEmpty(StiLicense.Key))                            
                                return true;
                            
                            return StiLicenseKey.Get(StiLicense.Key).
                                Products.Any(p => (
                                    p.Ident != StiProductIdent.BIDesigner && 
                                    p.Ident != StiProductIdent.BIDesktop && 
                                    p.Ident != StiProductIdent.BIServer &&
                                    p.Ident != StiProductIdent.BICloud &&
                                    p.Ident != StiProductIdent.CloudDashboards &&
                                    p.Ident != StiProductIdent.CloudReports));
                    }
                }
            }

            /// <summary>
            /// Returns true if the designer shows Events Tab in the properties panel. Otherwise, returns false.
            /// </summary>
            [Browsable(false)]
            internal static bool IsShowEventsTab => ShowEventsTab && IsDeveloperDesigner;

            [Browsable(false)]
            public static bool IsCodeTabVisible => CodeTabVisible && IsDeveloperDesigner;

            [Browsable(false)]
            public static bool IsPreviewReportVisible => PreviewReportVisible && IsDeveloperDesigner;
            #endregion

            #region Properties
            private static StiDesignerSpecification designerSpecification = StiDesignerSpecification.Auto;
            /// <summary>
            /// Gets or sets a value which indicates a type of the designer users - developers or BI creators.
            /// </summary>
            [DefaultValue(StiDesignerSpecification.Auto)]
            [Description("Gets or sets a value which indicates a type of the designer users - developers or BI creators.")]
            [StiSerializable]
            public static StiDesignerSpecification DesignerSpecification
            {
                get
                {
                    if (designerSpecificationLoaded)
                        return designerSpecification;

                    StiSettings.Load();
                    designerSpecificationLoaded = true;
                    designerSpecification = (StiDesignerSpecification)StiSettings.Get("StiDesigner", "DesignerSpecification", StiDesignerSpecification.Auto);
                    return designerSpecification;
                }
                set
                {
                    #region Set Property Level
                    switch (value)
                    {
                        case StiDesignerSpecification.Beginner:
                            StiOptions.Designer.PropertyGrid.PropertyLevel = StiLevel.Basic;
                            break;

                        case StiDesignerSpecification.BICreator:
                            StiOptions.Designer.PropertyGrid.PropertyLevel = StiLevel.Standard;
                            break;

                        case StiDesignerSpecification.Developer:
                            StiOptions.Designer.PropertyGrid.PropertyLevel = StiLevel.Professional;
                            break;
                    }
                    #endregion

                    if (designerSpecificationLoaded && designerSpecification == value) return;

                    StiSettings.Load();
                    StiSettings.Set("StiDesigner", "DesignerSpecification", value);
                    designerSpecification = value;
                    designerSpecificationLoaded = true;
                    StiSettings.Save();
                }
            }

            /// <summary>
            /// Gets or sets a value which indicates a scaling mode of the designer.
            /// </summary>
            [DefaultValue(StiDesignerScaleMode.AutomaticScaling)]
            [Description("Gets or sets a value which indicates a scaling mode of the designer.")]
            [StiSerializable]
            public static StiDesignerScaleMode DesignerScale
            {
                get
                {
                    StiSettings.Load();
                    return (StiDesignerScaleMode)StiSettings.Get("StiDesigner", "DesignerScale", StiDesignerScaleMode.AutomaticScaling);
                }
                set
                {
                    StiSettings.Load();
                    StiSettings.Set("StiDesigner", "DesignerScale", value);
                    StiSettings.Save();
                }
            }

            /// <summary>
            /// Gets or sets a value which controls of output of objects in the right to left mode.
            /// </summary>
            [DefaultValue(StiRightToLeftType.No)]
            [Description("Gets or sets a value which controls of output of objects in the right to left mode.")]
            [StiSerializable]
            public static StiRightToLeftType RightToLeft { get; set; } = StiRightToLeftType.No;

            /// <summary>
            /// Internal use only.
            /// </summary>
            public static bool IsRightToLeft => RightToLeft == StiRightToLeftType.Yes;

            /// <summary>
            /// Gets or sets value which indicates that font names will be drawing in simple view mode in font combobox in report designer.
            /// </summary>
            [Description("Gets or sets value which indicates that font names will be drawing in simple view mode in font combobox in report designer.")]
            [DefaultValue(true)]
            [StiSerializable]
            public static bool AllowCustomFontDrawing { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool AllowUseDragDrop { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool AllowCopyControlOperation { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool ShowWatermarkInPageSetup { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool IsComponentEditorEnabled { get; set; } = true;

            /// <summary>
            /// Internal use only.
            /// </summary>
            [Browsable(false)]
            internal static bool AllowAuthorizeWithLicenseKey { get; set; }

            [DefaultValue(true)]
            [StiSerializable]
            public static bool IsHighlightComponentServiceEnabled { get; set; } = true;

            private static Font defaultPropertyGridFont;
            [StiOptionsFontHelper(2)]
            [StiSerializable]
            public static Font DefaultPropertyGridFont
            {
                get
                {
                    return defaultPropertyGridFont ?? (defaultPropertyGridFont = new Font("Arial", 8));
                }
                set
                {
                    defaultPropertyGridFont = value;
                }
            }

            [DefaultValue(true)]
            [StiSerializable]
            public static bool ShowSuperToolTip { get; set; } = true;

            [DefaultValue(StiQuickButtonVisibility.Always)]
            [StiSerializable]
            public static StiQuickButtonVisibility ShowQuickButtons { get; set; } = StiQuickButtonVisibility.Always;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool CanCustomizeRibbon { get; set; } = true;

            [DefaultValue(false)]
            [StiSerializable]
            public static bool MarkComponentsWithErrors { get; set; }

            [DefaultValue(true)]
            [StiSerializable]
            public static bool AdvancedModeOfUndoRedoService { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool SortDictionaryByAliases { get; set; } = true;

            /// <summary>
            /// If property equal true then main menu from designer control showes on StiDesigner form.
            /// </summary>
            [DefaultValue(false)]
            [Description("If property equal true then main menu from designer control showes on StiDesigner form.")]
            [StiSerializable]
            public static bool ShowDesignerControlMainMenu { get; set; }

            /// <summary>
            /// Gets or sets a value which allows to show or to hide Events Tab of the property editor in the report designer. 
            /// </summary>
            [DefaultValue(true)]
            [StiSerializable]
            public static bool ShowEventsTab { get; set; } = true;

            /// <summary>
            /// Gets or sets value which indicates that designer called from VisualStudio.Net.
            /// Internal use only.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets value which indicates that designer called from VisualStudio.Net. Internal use only.")]
            [StiSerializable]
            public static bool RunInDesignMode { get; set; }

            [DefaultValue(true)]
            [StiSerializable]
            [Obsolete("Property 'ShowSelectTypeOfGuiOption' is obsoleted.")]
            public static bool ShowSelectTypeOfGuiOption { get; set; } = true;

            [DefaultValue(false)]
            [StiSerializable]
            public static bool UseSimpleGlobalizationEditor { get; set; }

            [DefaultValue(false)]
            [StiSerializable]
            public static bool UseRightToLeftGlobalizationEditor { get; set; }

            [DefaultValue(false)]
            [StiSerializable]
            public static bool AllowUseWinControl { get; set; }

            private static bool hideConnectionString;
            [DefaultValue(false)]
            [StiSerializable]
            public static bool HideConnectionString
            {
                get
                {
                    return hideConnectionString;
                }
                set
                {
                    hideConnectionString = value;
                    if (hideConnectionString)
                    {
#if !NETSTANDARD
                        Properties.Hide("ConnectionString");
#endif
                        CodeTabVisible = false;
                    }
                    else
                    {
#if !NETSTANDARD
                        Properties.Show("ConnectionString");
#endif
                    }
                }
            }

            /// <summary>
            /// Gets or sets the maximal nested level of dictionary objects in report dictionary panel.
            /// </summary>
            [DefaultValue(0)]
            [Description("Gets or sets the maximal nested level of dictionary objects in report dictionary panel.")]
            [StiSerializable]
            public static int MaxLevelOfDictionaryObjects { get; set; }

            public static object BackgroundColor { get; set; }


            private static StiStylesCollection styles;
            public static StiStylesCollection Styles
            {
                get
                {
                    return styles ?? (styles = new StiStylesCollection
                    {
                        new StiStyle("Normal", "Normal")
                        {
                            Brush = new StiSolidBrush(Color.Transparent),
                            TextBrush = new StiSolidBrush(Color.Black)
                        },
                        new StiStyle("Bad", "Bad")
                        {
                            Brush = new StiSolidBrush(Color.FromArgb(0xFF, 0xFF, 0xC7, 0xCE)),
                            TextBrush = new StiSolidBrush(Color.FromArgb(0xFF, 0xD0, 0x37, 0x05))
                        },
                        new StiStyle("Good", "Good")
                        {
                            Brush = new StiSolidBrush(Color.FromArgb(0xFF, 0xC6, 0xEF, 0xCE)),
                            TextBrush = new StiSolidBrush(Color.FromArgb(0xFF, 0x00, 0x61, 0x5E))
                        },
                        new StiStyle("Neutral", "Neutral")
                        {
                            Brush = new StiSolidBrush(Color.FromArgb(0xFF, 0xFF, 0xEB, 0x9C)),
                            TextBrush = new StiSolidBrush(Color.FromArgb(0xFF, 0xAE, 0x7F, 0x2B))
                        },
                        new StiStyle("Warning", "Warning")
                        {
                            Brush = new StiSolidBrush(Color.Transparent),
                            TextBrush = new StiSolidBrush(Color.Red),
                            Font = new Font("Arial", 8, FontStyle.Bold)
                        },
                        new StiStyle("Note", "Note")
                        {
                            Brush = new StiSolidBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xCC)),
                            TextBrush = new StiSolidBrush(Color.Black)
                        }
                    });
                }
            }

            [DefaultValue(true)]
            [StiSerializable]
            public static bool AllowAsyncDataCommands { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool AutoLargeHeight { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool CalculateBarcodeValueInDesignMode { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            public static bool UseGlobalizationManager { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates that images will be preloaded in the designer.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value which indicates that images will be preloaded in the designer.")]
            [StiSerializable]
            public static bool PreloadImageFromDataColumn { get; set; } = true;
            #endregion

        }
    }
}