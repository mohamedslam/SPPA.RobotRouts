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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Serializing;

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
            public sealed class WYSIWYG
            {
                #region Obsolete
                [DefaultValue(4d)]
                [StiSerializable]
                [Obsolete("StiOptions.Engine.WYSIWYG.PrecisionModeFactor property is obsolete.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static double PrecisionModeFactor
                {
                    get
                    {
                        return StiTextRenderer.PrecisionModeFactor;
                    }
                    set
                    {
                        StiTextRenderer.PrecisionModeFactor = value;
                    }
                }

                [DefaultValue(false)]
                [StiSerializable]
                [Obsolete("StiOptions.Engine.WYSIWYG.PrecisionModeEnabled property is obsolete.")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool PrecisionModeEnabled
                {
                    get
                    {
                        return StiTextRenderer.PrecisionModeEnabled;
                    }
                    set
                    {
                        StiTextRenderer.PrecisionModeEnabled = value;
                    }
                }
                #endregion

                [DefaultValue(true)]
                [StiSerializable]
                public static bool CorrectionEnabled
                {
                    get
                    {
                        return StiTextRenderer.CorrectionEnabled && !StiTextRenderer.Compatibility2009;
                    }
                    set
                    {
                        StiTextRenderer.CorrectionEnabled = value;
                    }
                }

                private static bool interpreteFontSizeInHtmlTagsAsInHtml;
                /// <summary>
                /// Interprete the FontSize attribute in the Html-tags as in Html
                /// </summary>
                [DefaultValue(false)]
                [Description("Interprete the FontSize attribute in the Html-tags as in Html.")]
                [StiSerializable]
                public static bool InterpreteFontSizeInHtmlTagsAsInHtml
                {
                    get
                    {
                        if (FullTrust)
                            return StiTextRenderer.InterpreteFontSizeInHtmlTagsAsInHtml;
                        else
                            return interpreteFontSizeInHtmlTagsAsInHtml;
                    }
                    set
                    {
                        if (FullTrust)
                            StiTextRenderer.InterpreteFontSizeInHtmlTagsAsInHtml = value;
                        else
                            interpreteFontSizeInHtmlTagsAsInHtml = value;
                    }
                }
            }
		}
    }
}