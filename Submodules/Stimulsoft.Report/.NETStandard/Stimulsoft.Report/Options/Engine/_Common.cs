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
using System.Text;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Engine;
using System.Drawing;
using System.Drawing.Printing;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// A class which controls of settings of the report engine. 
        /// </summary>
		public sealed partial class Engine
        {
            /// <summary>
            /// This property stores units? values by default. If null, then millimeters of or hundredths of inches are used, depending on the regional settings.
            /// </summary>
            [StiOptionsUniversalTypeObject(StiOptionsUniversalType.Enum, "Stimulsoft.Report.StiReportUnitType, Stimulsoft.Report")]
            [Description("This property stores units? values by default. If null, then millimeters of or hundredths of inches are used, depending on the regional settings.")]
            [StiSerializable]
            [Category("Engine")]
            [DefaultValue(null)]
            public static object DefaultUnit { get; set; }

            /// <summary>
            /// This value indicates that the OriginalFontName property should be used when serializing a report. The OriginalFontName property stores 
            /// the name of the original font name, even if this font is not present in the system. Using this option is not recommended in ASP.NET.
            /// </summary>
            [DefaultValue(true)]
            [Description("This value indicates that the OriginalFontName property should be used when serializing a report. The OriginalFontName property " +
                         "stores the name of the original font name, even if this font is not present in the system. Using this option is not recommended in ASP.NET.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool AllowUseOriginalFontName { get; set; } = true;

            /// <summary>
            /// Allows to reset properties (TagValue, HyperlinkValue, BookmarkValue and ToolTipValue) to null before report rendering.
            /// </summary>
            [DefaultValue(true)]
            [Description("Allows to reset properties (TagValue, HyperlinkValue, BookmarkValue and ToolTipValue) to null before report rendering.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool AllowResetValuesAtComponent { get; set; } = true;

            /// <summary>
            /// The default value for the PrintIfDetailEmpty property of the DataBand component.
            /// </summary>
            [DefaultValue(false)]
            [Description("The default value for the PrintIfDetailEmpty property of the DataBand component.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool PrintIfDetailEmptyDefaultValue { get; set; }

            /// <summary>
            /// If the property is set to true, then the library may change the CurrentDurectory property.
            /// </summary>
            [DefaultValue(true)]
            [Description("If the property is set to true, then the library may change the CurrentDurectory property.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool AllowSetCurrentDirectory
            {
                get
                {
                    return StiBaseOptions.AllowSetCurrentDirectory;
                }
                set
                {
                    StiBaseOptions.AllowSetCurrentDirectory = value;
                }
            }

            [DefaultValue(true)]
            [StiSerializable]
            [Category("Engine")]
            public static bool AllowResetAssemblyAfterCompilation { get; set; } = true;

            /// <summary>
            /// If the value is set to true, then the progress window of report rendering is run in another thread.
            /// </summary>
            [DefaultValue(false)]
            [Description("If the value is set to true, then the progress window of report rendering is run in another thread.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool AllowProgressInThread { get; set; }

            /// <summary>
            /// If the value is set to true, then the Progress property of the StiReport object can be used for the progress customization.
            /// </summary>
            [DefaultValue(false)]
            [Description("If the value is set to true, then the Progress property of the StiReport object can be used for the progress customization.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool AllowProgressExternalAccess { get; set; }

            /// <summary>
            /// Gets or sets value which indicates that config file "Stimulsoft.Report.config" is not saved from report engine.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets value which indicates that config file 'Stimulsoft.Report.config' is not saved from report engine.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool DontSaveConfig { get; set; }

            /// <summary>
            /// Gets or sets value which indicates that config file "Stimulsoft.Report.settings" is not saved from report engine.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets value which indicates that config file 'Stimulsoft.Report.settings' is not saved from report engine.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool DontSaveSettings { get; set; }

            /// <summary>
            /// Gets or sets the default report engine for new report.
            /// </summary>
            [DefaultValue(StiEngineVersion.EngineV2)]
            [Description("Gets or sets the default report engine for new report.")]
            [StiSerializable]
            [Category("Engine")]
            public static StiEngineVersion DefaultEngineVersion { get; set; } = StiEngineVersion.EngineV2;

            [Obsolete("UseAdvancedPrintOn property is obsolete. Please use UseAdvancedPrintOnEngineV1 property instead.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static bool UseAdvancedPrintOn
			{
				get
				{
					return UseAdvancedPrintOnEngineV1;
				}
				set
				{
					UseAdvancedPrintOnEngineV1 = value;
				}
			}

            /// <summary>
            /// If the property is set to true, the report generator will use the PrintOn property when rendering a report.
            /// Using this mode is not recommended because of problems with page numbers.
            /// If the property is set to false, then the property will be processed after rendering a report.          
            /// </summary>
            [DefaultValue(true)]
            [Description("If the property is set to true, the report generator will use the PrintOn property when rendering a report. Using this mode is not recommended because of problems with page numbers. If the property is set to false, then the property will be processed after rendering a report.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool UseAdvancedPrintOnEngineV1 { get; set; } = true;

            /// <summary>
            /// If the property is set to true, the report generator will use the PrintOn property when rendering a report.
            /// Using this mode is not recommended because of problems with page numbers.
            /// If the property is set to false, then the property will be processed after rendering a report.          
            /// </summary>
            [DefaultValue(false)]
            [Description("If the property is set to true, the report generator will use the PrintOn property when rendering a report. Using this mode is not recommended because of problems with page numbers. If the property is set to false, then the property will be processed after rendering a report.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool UseAdvancedPrintOnEngineV2 { get; set; }

            /// <summary>
            /// If the value is set to true, the names of the new components are generated as localized regardless the settings of the report designer.
            /// </summary>
            [DefaultValue(false)]
            [Description("If the value is set to true, the names of the new components are generated as localized regardless the settings of the report designer.")]
            [StiSerializable]
            [Category("Naming")]
            public static bool ForceGenerationLocalizedName { get; set; }

            /// <summary>
            /// If the value is set to true, the names of the new components are generated as not localized regardless the settings of the report designer.
            /// </summary>
            [DefaultValue(false)]
            [Description("If the value is set to true, the names of the new components are generated as not localized regardless the settings of the report designer.")]
            [StiSerializable]
            [Category("Naming")]
            public static bool ForceGenerationNonLocalizedName { get; set; }

            /// <summary>
            /// If the property is set to true, the report generator may use messages of report rendering.
            /// </summary>
            [DefaultValue(true)]
            [Description("If the property is set to true, the report generator may use messages of report rendering.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool ShowReportRenderingMessages { get; set; } = true;

            /// <summary>
            /// If the property is set to true, the report generator creates a report class with the modifier partial.
            /// </summary>
            [DefaultValue(false)]
            [Description("If the property is set to true, the report generator creates a report class with the modifier partial.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool GeneratePartialReportClass { get; set; }

            /// <summary>
            /// Gets or sets encoding which used for saving soure code.
            /// </summary>
            [DefaultValue(null)]
            [Description("Gets or sets encoding which used for saving soure code.")]
            [StiSerializable]
            [Category("Designer")]
            public static Encoding SourceCodeEncoding { get; set; }

            /// <summary>
            /// Gets or sets path to AssemblyKeyFile for signing all compiled reports.
            /// </summary>
            [DefaultValue(null)]
            [Description("Gets or sets path to AssemblyKeyFile for signing all compiled reports.")]
            [StiSerializable]
            [Category("Designer")]
            public static string PathToAssemblyKeyFile { get; set; }

            /// <summary>
            /// A value indicates that the ParentReport property of a report is not cleared after rendering process.
            /// </summary>
            [DefaultValue(false)]
            [Description("A value indicates that the ParentReport property of a report is not cleared after rendering process.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool NotClearParentReport { get; set; }

            /// <summary>
            /// A value indicates which side of a page must use the report designer to dock a page footer component. If the value is true, the report designer will use bottom side of the page.
            /// </summary>
            [DefaultValue(true)]
            [Description("A value indicates which side of a page must use the report designer to dock a page footer component. If the value is true, the report designer will use bottom side of the page.")]
            [StiSerializable]
            [Category("Designer")]
            public static bool DockPageFooterToBottom { get; set; } = true;
            
            /// <summary>
            /// Gets or sets default namespace for the report.
            /// </summary>
            [DefaultValue("Reports")]
            [Description("Gets or sets default namespace for the report.")]
            [StiSerializable]
            [Category("Engine")]
            public static string DefaultNamespace { get; set; } = "Reports";

            private static string[] defaultNamespaces =
			{
			    "System",
			    "System.Drawing",
			    "System.Windows.Forms",
			    "System.Data",
			    "Stimulsoft.Controls",
			    "Stimulsoft.Base.Drawing",
			    "Stimulsoft.Report",
			    "Stimulsoft.Report.Dialogs",
			    "Stimulsoft.Report.Components"
			};

            private static string[] namespaces;
			/// <summary>
			/// Gets or sets namespaces which are to be added in to the report class being generated.
			/// </summary>
            [Category("Engine")]
            public static string[] Namespaces
			{
				get
				{
				    return namespaces ?? defaultNamespaces;
				}
				set
				{
					namespaces = value;
				}
			}

            /// <summary>
            /// Returns true if Namespaces property has default value;
            /// </summary>
            public static bool IsDefaultNamespaces => namespaces == null;

            private static string[] defaultReferencedAssemblies =
			{
			    "System.Dll",
			    "System.Drawing.Dll",
			    "System.Windows.Forms.Dll",
			    "System.Data.Dll",
			    "System.Xml.Dll",
			    "Stimulsoft.Controls.Dll",
			    "Stimulsoft.Base.Dll",
			    "Stimulsoft.Report.Dll"
			};

            private static string[] referencedAssemblies;
            /// <summary>
            /// Gets or sets assemblies which are being attached to a new report.
            /// </summary>
            [Category("Engine")]
            public static string[] ReferencedAssemblies
			{
                get
                {
                    return referencedAssemblies ?? defaultReferencedAssemblies;
                }
                set
                {
                    referencedAssemblies = value;
                }
			}

            /// <summary>
            /// Returns true if Namespaces property has default value;
            /// </summary>
            public static bool IsDefaultReferencedAssemblies => referencedAssemblies == null;

            /// <summary>
            /// Gets or sets a value which controls of naming of new components in the report.
            /// </summary>
            [DefaultValue(StiNamingRule.Advanced)]
            [Description("Gets or sets a value which controls of naming of new components in the report.")]
            [StiSerializable]
            [Category("Naming")]
            public static StiNamingRule NamingRule { get; set; } = StiNamingRule.Advanced;

            /// <summary>
            /// Gets or sets the default programming language of the report.
            /// </summary>
            [DefaultValue(StiReportLanguageType.CSharp)]
            [Description("Gets or sets the default programming language of the report.")]
            [StiSerializable]
            [Category("Engine")]
            public static StiReportLanguageType DefaultReportLanguage { get; set; } = StiReportLanguageType.CSharp;

            /// <summary>
            /// Gets or sets the base type for the report creation in the designer.
            /// </summary>
            [Category("Engine")]
            public static Type BaseReportType { get; set; } = typeof (StiReport);

            [DefaultValue(true)]
            [StiSerializable]
            [Category("Engine")]
            public static bool FullTrust
            {
                get
                {
                    return StiBaseOptions.FullTrust;
                }
                set
                {
                    StiBaseOptions.FullTrust = value;
                }
            }

            [DefaultValue(false)]
            [StiSerializable]
            [Category("Engine")]
            public static bool FIPSCompliance
            {
                get
                {
                    return StiBaseOptions.FIPSCompliance;
                }
                set
                {
                    StiBaseOptions.FIPSCompliance = value;
                }
            }

            [DefaultValue(false)]
            [StiSerializable]
            [Category("Engine")]
            public static bool HideRenderingProgress { get; set; }

            /// <summary>
			/// Gets or sets a value indicating whether not to show the message from engine of the message.
			/// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value indicating whether not to show the message from engine of the message.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool HideMessages
			{
				get
				{
					return StiExceptionProvider.HideMessages;
				}
				set
				{
                    StiExceptionProvider.HideMessages = value;
				}
			}

			/// <summary>
			/// Gets or sets the value, which indicates not to show the exception from engine of the exception.
			/// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets the value, which indicates not to show the exception from engine of the exception.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool HideExceptions
			{
				get
				{
                    return StiExceptionProvider.HideExceptions;
				}
				set
				{
					StiExceptionProvider.HideExceptions = value;
				}
			}

            /// <summary>
            /// Gets or sets a value which enables or disables log of the report.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which enables or disables log of the report.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool LogEnabled
			{
				get
				{
					return StiLogService.LogEnabled;
				}
				set
				{
					StiLogService.LogEnabled = value;
				}
			}

            /// <summary>
            /// Gets or sets a value, which indicates whether it is necessary to show tracing messages 
            /// of the report engine in the Visual Studio Message Window.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value, which indicates whether it is necessary to show tracing messages of the report engine in the Visual Studio Message Window.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool TraceEnabled
			{
				get
				{
					return StiLogService.TraceEnabled;
				}
				set
				{
					StiLogService.TraceEnabled = value;
				}
			}
            
            /// <summary>
            /// Gets or sets default paper size for new created pages.
            /// </summary>
            [DefaultValue(PaperKind.Custom)]
            [Description("Gets or sets default paper size for new created pages.")]
            [StiSerializable]
            [Category("Engine")]
            public static PaperKind DefaultPaperSize { get; set; } = PaperKind.Custom;

            [DefaultValue(false)]
            [StiSerializable]
            [Category("Engine")]
            public static bool IgnoreFirstPassForGroup { get; set; }

            [DefaultValue(false)]
            [StiSerializable]
            [Category("Engine")]
            public static bool OldModeOfRenderingEventInEngineV2 { get; set; }

            [DefaultValue(true)]
            [StiSerializable]
            [Category("Engine")]
            public static bool UseGarbageCollectorForImageCache { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            [Category("Export")]
            public static bool ConvertBmpDataToPng { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            [Category("Engine")]
            public static bool UsePercentageProgress { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            [Category("Engine")]
            public static bool UseTemplateForPagePrintEvents { get; set; } = true;

            [DefaultValue(false)]
            [StiSerializable]
            [Category("Engine")]
            public static bool UseParentStylesOldMode { get; set; }

            [DefaultValue(false)]
            [StiSerializable]
            [Category("Engine")]
            public static bool ThrowErrorIfFontIsNotInstalledInSystem { get; set; }

            [DefaultValue(false)]
            [StiSerializable]
            [Category("Engine")]
            public static bool UseCollateOldMode { get; set; }

            [DefaultValue(false)]
            [StiSerializable]
            [Category("Engine")]
            public static bool ForceNewPageInSubReports { get; set; }

            [DefaultValue(true)]
            [StiSerializable]
            [Category("Engine")]
            public static bool DpiAware { get; set; } = true;

            [DefaultValue(true)]
            [StiSerializable]
            [Category("Data")]
            public static bool AllowUseResetMethodInBusinessObject { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates whether use the back compatibility when Style property of primitives is set to None
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which indicates whether use the back compatibility when Style property of primitives is set to None.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool PrimitivesStyleNoneBackCompatibility { get; set; }

            /// <summary>
            /// Gets or sets a value which indicates whether use the CheckSize method for Continued containers
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value which indicates whether use the CheckSize method for Continued containers.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool UseCheckSizeForContinuedContainers { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates whether check DockToContainer if component is disabled
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which indicates whether check DockToContainer if component is disabled.")]
            [StiSerializable]
            [Category("Designer")]
            public static bool CheckDockToContainerIfComponentDisabled { get; set; }

            /// <summary>
            /// Gets or sets a value which indicates whether force the NewPage method for the extra columns
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value which indicates whether force the NewPage method for the extra columns.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool ForceNewPageForExtraColumns { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates whether use Round() for the ToCurrencyWords functions
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value which indicates whether use Round() for the ToCurrencyWords functions.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool UseRoundForToCurrencyWordsFunctions { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates whether force the normalization of EndOfLine symbols
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value which indicates whether force the normalization of EndOfLine symbols.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool ForceNormalizeEndOfLineSymbols { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which indicates whether force to disable the collapsing
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which indicates whether force to disable the collapsing.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool ForceDisableCollapsing { get; set; }

            /// <summary>
            /// This value indicates that it is necessary to pack script after report have been loaded.
            /// </summary>
            [DefaultValue(true)]
            [Description("This value indicates that it is necessary to pack script after report have been loaded.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool PackScriptAfterReportLoaded { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which allow to use the PrintOnAllPages property of headers in subreports
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value which allow to use the PrintOnAllPages property of headers in subreports.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool UsePrintOnAllPagesPropertyOfHeadersInSubreports { get; set; } = true;

            /// <summary>
            /// Gets or sets a value indicating whether it is necessary to force the TopMost property of the ProgressForm.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value indicating whether it is necessary to force the TopMost property of the ProgressForm.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool ForceTopMostPropertyOfProgressForm { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether it is necessary to fix state of first collapsing band.
            /// For compatibility with 2012.1 set this property to true.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value indicating whether it is necessary to fix state of first collapsing band. For compatibility with 2012.1 set this property to true.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool FixFirstCollapsingBandState { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether it is necessary to render external subreports with help of unlimited height pages.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating whether it is necessary to render external subreports with help of unlimited height pages.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool RenderExternalSubReportsWithHelpOfUnlimitedHeightPages { get; set; } = true;
            
            /// <summary>
            /// Gets or sets a value forcing optimization in LoadDocument method.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value forcing optimization in LoadDocument method.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool DocumentLoadingOptimization { get; set; }

            /// <summary>
            /// Gets or sets a value forcing optimization in SaveDocument method.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value forcing optimization in SaveDocument method.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool DocumentSavingOptimization { get; set; }

            /// <summary>
            /// Gets or sets a value forcing optimization in serialization methods.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value forcing optimization in serialization methods.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool SerializationOptimization { get; set; } = true;

            /// <summary>
            /// Gets or sets a value forcing removing comments from events script.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value forcing removing comments from events script.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool RemoveCommentsFromEventScript { get; set; } = true;

            /// <summary>
            /// Gets or sets a value forcing fix conversion of argument for SumTime function. Obsolete.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value forcing fix conversion of argument for SumTime function.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool AllowFixSumTimeArgumentConversion { get; set; }

            /// <summary>
            /// Gets or sets a value forcing fix conversion of argument for SumTime function.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value forcing fix conversion of DateTime argument for functions.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool AllowFixDateTimeArgumentConversion { get; set; } = true;

            /// <summary>
            /// Gets or sets a value forcing aggregate functions exception processing.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value forcing aggregate functions exception processing.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool ForceAggregateFunctionsExceptionProcessing { get; set; } = true;

            /// <summary>
            /// Gets or sets a value forcing interpretation mode of the report calculation.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value forcing interpretation mode of the report calculation.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool ForceInterpretationMode { get; set; } = false;
            
            /// <summary>
            /// Gets or sets a value which defines a previous realization of the Paint mode of the ZipCode component.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which defines a previous realization of the Paint mode of the ZipCode component.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool UseOldZipCodePaintMode { get; set; }

            /// <summary>
            /// Gets or sets a value forcing removing bottom border for breaked container.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value forcing removing bottom border for breaked container.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool RemoveBottomBorderOfSplitContainer { get; set; }

            /// <summary>
            /// Gets or sets a value forcing stop processing conditions at first true condition.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value forcing stop processing conditions at first true condition.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool StopProcessingAtFirstTrueCondition { get; set; }

            /// <summary>
            /// Gets or sets a value forcing invert the order of the conditions processing.
            /// For compatibility with 2015.2 set this property to true.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value forcing invert the order of the conditions processing. For compatibility with 2015.2 set this property to true.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool InvertConditionsProcessingOrder { get; set; }

            /// <summary>
            /// Gets or sets a value forcing apply condition style for disabled components.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value forcing apply condition style for disabled components.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool ApplyConditionStyleForDisabledComponents { get; set; }

            /// <summary>
            /// Gets or sets a value which allow BreakContainer optimization.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value which allow BreakContainer optimization.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool AllowBreakContainerOptimization { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which allow use generic types in CodeDom serialization.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which allow use generic types in CodeDom serialization.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool AllowUseGenericTypesInCodeDom { get; set; }

            /// <summary>
            /// Gets or sets a value which defines a midpoint rounding mode.
            /// </summary>
            [DefaultValue(MidpointRounding.ToEven)]
            [Description("Gets or sets a value which defines a midpoint rounding mode.")]
            [StiSerializable]
            [Category("Engine")]
            public static MidpointRounding MidpointRounding { get; set; } = MidpointRounding.ToEven;

            /// <summary>
            /// Gets or sets a value which allow extended check for PrintIfDetailEmpty property.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which allow extended check for PrintIfDetailEmpty property.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool PrintIfDetailEmptyNesting { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether it is necessary to fix the PageNumber value in the rendering events.
            /// For compatibility with 2016.3 set this property to false.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating whether it is necessary to fix the PageNumber value in the rendering events. For compatibility with 2016.3 set this property to false.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool FixPageNumberInEvents { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which allow insert line breaks when saving byte array to Base64 string.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value which allow insert line breaks when saving byte array to Base64 string.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool AllowInsertLineBreaksWhenSavingByteArray
            {
                get
                {
                    return StiBaseOptions.AllowInsertLineBreaksWhenSavingByteArray;
                }
                set
                {
                    StiBaseOptions.AllowInsertLineBreaksWhenSavingByteArray = value;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether it is possible to return zero result for Min/Max aggregate functions if there is no acceptable input data.
            /// For compatibility with 2017.2 set this property to false.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating whether it is possible to return zero result for Min/Max aggregate functions if there is no acceptable input data. For compatibility with 2017.2 set this property to false.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool AllowZeroResultForMinMaxAggregateFunctions { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which allows to load Stimulsoft Form assembly.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which allows to load Stimulsoft Form assembly.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool SkipLoadingFormAssembly { get; set; }

            /// <summary>
            /// Gets or sets a value which allows to load Stimulsoft Dashboards assembly.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which allows to load Stimulsoft Dashboards assembly.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool SkipLoadingDashboardAssembly { get; set; }

            /// <summary>
            /// Gets or sets a value which allows to load Stimulsoft Apps assembly.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which allows to load Stimulsoft Apps assembly.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool SkipLoadingAppAssembly { get; set; }

            /// <summary>
            /// Gets or sets a value which allows converting values to required types in the text formatting.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value which allows converting values to required types in the text formatting.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool AllowConvertingInFormatting { get; set; } = true;

            /// <summary>
            /// Gets or sets a value which allows old style formatting of a time span value.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which allows old style formatting of a time span value.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool AllowOldTimeSpanFormatting { get; set; }

            /// <summary>
            /// Gets or sets a color which is used for coloring negative values in the text formatting.
            /// </summary>
            [Description("Gets or sets a color which is used for coloring negative values in the text formatting.")]
            [StiSerializable]
            [Category("Engine")]
            public static Color NegativeColor { get; set; } = Color.Red;

            /// <summary>
            /// Gets or sets a value which force disable Antialiasing in painters.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which force disable Antialiasing in painters.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool DisableAntialiasingInPainters { get; set; } = false;


            /// <summary>
            /// Gets or sets a value which force using gap for last column of container.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which force using gap for last column of container.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool AllowGapForLastColumn { get; set; } = false;


            /// <summary>
            /// Gets or sets a value which force sort the list using the ordinal value of the character code points (StringComparer.Ordinal).
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which force sort the list using the ordinal value of the character code points (StringComparer.Ordinal).")]
            [StiSerializable]
            [Category("Engine")]
            public static bool OrdinalStringComparison { get; set; } = false;

            /// <summary>
            /// Gets or sets a value which indicates that method WaitForPendingFinalizers can be used.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which indicates that method WaitForPendingFinalizers can be used.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool AllowWaitForPendingFinalizers { get; set; }

            /// <summary>
            /// Gets or sets a value which force show Bookmark for disabled band.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value which force show Bookmark for disabled band.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool ShowBookmarkToDisabledBand { get; set; } = false;

        }
    }
}