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

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// Class for adjustment of the preview of a report.
        /// </summary>
        public partial class Viewer
		{
            /// <summary>
            /// Class for adjustment of the viewer window.
            /// </summary>
            public partial class Windows
            {
                public sealed class Exports
                {
                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowPdf { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowXps { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowPpt2007 { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowHtml { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowHtml5 { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowMht { get; set; } = true;
                    
                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowText { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowRtf { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowWord2007 { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowOdt { get; set; } = true;
                    
                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowExcel { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowExcelXml { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowExcel2007 { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowOds { get; set; } = true;
                    
                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowData
                    {
                        get
                        {
                            return ShowCsv || ShowDbf || ShowXml || ShowDif || ShowSylk || ShowJson;
                        }
                        set
                        {
                            ShowCsv = ShowDbf = ShowXml = ShowDif = ShowSylk = ShowJson = value;
                        }
                    }

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowCsv { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDbf { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowXml { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowDif { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowSylk { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowJson { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowImage
                    {
                        get
                        {
                            return ShowBmp || ShowGif || ShowJpeg || ShowPcx || ShowPng || ShowSvg || ShowSvgz || ShowTiff || ShowMetafile;
                        }
                        set
                        {
                            ShowBmp = ShowGif = ShowJpeg = ShowPcx = ShowPng = ShowSvg = ShowSvgz = ShowTiff = ShowMetafile = value;
                        }
                    }

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowBmp { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowGif { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowJpeg { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowPcx { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowPng { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowTiff { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowSvg { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowSvgz { get; set; } = true;

                    [DefaultValue(true)]
                    [StiSerializable]
                    public static bool ShowMetafile { get; set; } = true;

                    public static class States
                    {
                        public static bool IsDocumentVisible => ShowPdf || ShowXps || ShowPpt2007;

                        public static bool IsWebVisible => ShowHtml || ShowMht;

                        public static bool IsTextVisible => ShowText || ShowRtf || ShowWord2007 || ShowOdt;

                        public static bool IsCalcVisible => ShowExcel || ShowExcelXml || ShowExcel2007 || ShowOds;

                        public static bool IsDataVisible => ShowCsv || ShowDbf || ShowXml || ShowDif || ShowSylk;

                        public static bool IsImagesVisible => IsBitmapImagesVisible || IsVectorImagesVisible;

                        public static bool IsBitmapImagesVisible => ShowBmp || ShowGif || ShowJpeg || ShowPcx || ShowPng || ShowTiff;

                        public static bool IsVectorImagesVisible => ShowMetafile || ShowSvg || ShowSvgz;
                    }
                }
            }
        }
    }
}