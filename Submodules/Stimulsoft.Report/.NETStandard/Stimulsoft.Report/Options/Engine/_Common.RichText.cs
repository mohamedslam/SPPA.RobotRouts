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

using System.ComponentModel;
using Stimulsoft.Base.Serializing;
using System.Drawing;
using System.Drawing.Imaging;

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
        /// A class which controls of settings of the report engine. 
        /// </summary>
		public sealed partial class Engine
        {
            /// <summary>
            /// Gets or sets a value indicating whether it is necessary to insert breaks in Rtf export.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value indicating whether it is necessary to insert breaks in Rtf export.")]
            [StiSerializable]
            [Category("Export")]
            public static bool ForceRtfBreak { get; set; }

            [DefaultValue(null)]
            [StiSerializable]
            [Category("Export")]
            public static Font DefaultRtfFont { get; set; }

            [DefaultValue(EmfType.EmfOnly)]
            [StiSerializable]
            [Category("Engine")]
            public static EmfType RichTextDrawingMetafileType { get; set; } = EmfType.EmfOnly;

            [DefaultValue(500)]
            [StiSerializable]
            [Category("Engine")]
            public static int RichTextImageCacheSize { get; set; } = 500;

            /// <summary>
            /// Gets or sets a value forcing render RichText components in other application domain.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets a value forcing render RichText components in other application domain.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool RenderRichTextInOtherDomain { get; set; }

            /// <summary>
            /// Gets or sets a value forcing load extended RichText library.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value forcing load extended RichText library.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool? ForceLoadExtendedRichTextLibrary
            {
                get
                {
                    return Stimulsoft.Base.StiBaseOptions.ForceLoadExtendedRichTextLibrary;
                }
                set
                {
                    Stimulsoft.Base.StiBaseOptions.ForceLoadExtendedRichTextLibrary = value;
                }
            }

            /// <summary>
            /// Gets or sets a value which specified the extended RichText library ClassName.
            /// </summary>
            [DefaultValue("RichEdit50W")]
            [Description("Gets or sets a value which specified the extended RichText library ClassName.")]
            [StiSerializable]
            [Category("Engine")]
            public static string ExtendedRichTextLibraryClassName
            {
                get
                {
                    return Stimulsoft.Base.StiBaseOptions.ExtendedRichTextLibraryClassName;
                }
                set
                {
                    Stimulsoft.Base.StiBaseOptions.ExtendedRichTextLibraryClassName = value;

                    //if name is not accepted - exception occurs on following line, and standard name will be restored.
                    var rich = new Stimulsoft.Report.Controls.StiRichTextBox(false);
                    rich.Dispose();
                }
            }

            /// <summary>
            /// Gets or sets a value of the RichText scaling, in percent. 
            /// </summary>
            [DefaultValue(96)]
            [Description("Gets or sets a value of the RichText scaling, in percent.")]
            [StiSerializable]
            [Category("Engine")]
            public static float RichTextScale { get; set; } = 96;

            /// <summary>
            /// Gets or sets a value forcing render RichText in Wpf always in Wysiwyg mode.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value forcing render RichText in Wpf always in Wysiwyg mode.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool RenderRichTextInWpfAlwaysWysiwyg { get; set; } = true;

            /// <summary>
            /// Gets or sets a value indicating whether it is necessary to check InstalledPrinters for get MeasureGraphics with maximum dpi.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating whether it is necessary to check InstalledPrinters for get MeasureGraphics with maximum dpi.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool CheckInstalledPrintersForRichTextMeasureGraphics { get; set; } = true;

            /// <summary>
            /// Gets or sets a value indicating whether it is necessary to lock RichText processing.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating whether it is necessary to lock RichText processing.")]
            [StiSerializable]
            [Category("Engine")]
            public static bool ForceLockRichTextThread { get; set; } = true;

        }
    }
}