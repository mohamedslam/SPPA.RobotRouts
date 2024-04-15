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

using System.Collections;
using System.ComponentModel;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Export;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
    {
        /// <summary>
        /// Class for adjustment of the report Export.
        /// </summary>
        public sealed partial class Export
        {
            /// <summary>
            /// Gets or sets a text of checkbox for true
            /// </summary>
            [DefaultValue("true")]
            [Description("Gets or sets a text of checkbox for true.")]
            [StiSerializable]
            public static string CheckBoxTextForTrue { get; set; } = "true";

            /// <summary>
            /// Gets or sets a text of checkbox for false
            /// </summary>
            [DefaultValue("false")]
            [Description("Gets or sets a text of checkbox for false.")]
            [StiSerializable]
            public static string CheckBoxTextForFalse { get; set; } = "false";

            /// <summary>
            /// Gets or sets value which indicates that file name for Send EMail function will be generated automatically.
            /// </summary>
            [DefaultValue(false)]
            [Description("Gets or sets value which indicates that file name for Send EMail function will be generated automatically.")]
            [StiSerializable]
            public static bool AutoGenerateFileNameInSendEMail { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether it is necessary to disable ClearType feature during export.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating whether it is necessary to disable ClearType feature during export.")]
            [StiSerializable]
            public static bool DisableClearTypeDuringExport { get; set; } = true;

            /// <summary>
            /// Gets or sets a value indicating whether it is necessary to use cache mode for the StiMatrix.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating whether it is necessary to use cache mode for the StiMatrix.")]
            [StiSerializable]
            public static bool UseCacheModeForStiMatrix { get; set; } = true;

            /// <summary>
            /// Gets or sets a value indicating whether it is necessary to optimize exports in DataOnly mode.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating whether it is necessary to optimize exports in DataOnly mode.")]
            [StiSerializable]
            public static bool OptimizeDataOnlyMode { get; set; } = true;

            /// <summary>
            /// Gets or sets a value indicating the image CutEdges mode.
            /// </summary>
            [DefaultValue(StiImageCutEdgesMode.ExceptFeed)]
            [Description("Gets or sets a value indicating the image CutEdges mode.")]
            [StiSerializable]
            public static StiImageCutEdgesMode ImageCutEdgesMode { get; set; } = StiImageCutEdgesMode.ExceptFeed;


            /// <summary>
            /// Gets or sets a value indicating whether it is necessary to use alternative font names.
            /// </summary>
            [DefaultValue(true)]
            [Description("Gets or sets a value indicating whether it is necessary to use alternative font names.")]
            [StiSerializable]
            public static bool UseAlternativeFontNames { get; set; } = true;

            private static Hashtable _alternativeFontNames = null;
            /// Table of the alternative font names.
            public static Hashtable AlternativeFontNames
            {
                get
                {
                    if (_alternativeFontNames == null)
                    {
                        #region Init table
                        _alternativeFontNames = new Hashtable
                        {
                            { "\uFF2D\uFF33 \uFF30\u30B4\u30B7\u30C3\u30AF", "MS PGothic" }, // ＭＳ Ｐゴシック, MS PGothic
                            { "\uFF2D\uFF33 \uFF30\u660E\u671D", "MS PMincho" }, // ＭＳ Ｐ明朝, MS PMincho
                            { "\uFF2D\uFF33 \u30B4\u30B7\u30C3\u30AF", "MS Gothic" }, // ＭＳゴシック, MS Gothic
                            { "\uFF2D\uFF33 \u660E\u671D", "MS Mincho" }, // ＭＳ 明朝, MS Mincho
                            { "\u30E1\u30A4\u30EA\u30AA", "Meiryo" }, // メイリオ, Meiryo
                            { "\uBC14\uD0D5", "Batang" }, // 바탕, Batang
                            { "\uBC14\uD0D5\uCCB4", "Batangche" }, // 바탕체, Batangche
                            { "\uAD74\uB9BC", "Gulim" }, // 굴림, Gulim
                            { "\uAD74\uB9BC\uCCB4", "Gulimche" }, // 굴림체, Gulimche
                            { "\uB3CB\uC6C0", "Dotum" }, // 돋움, Dotum
                            { "\uB3CB\uC6C0\uCCB4", "Dotumche" }, // 돋움체, Dotumche
                            { "\uAD81\uC11C", "Gungsuh" }, // 궁서, Gungsuh
                            { "\uAD81\uC11C\uCCB4", "Gungsuhche" }, // 궁서체, Gungsuhche
                            { "\uB9D1\uC740 \uACE0\uB515", "Malgun Gothic" }, // 맑은 고딕, Malgun Gothic
                            { "\u5B8B\u4F53", "SimSun" }, // 宋体, SimSun
                            { "\u5B8B\u4F53-ExtB", "SimSun-ExtB" }, // 宋体-ExtB, SimSun-ExtB
                            { "\u9ED1\u4F53", "SimHei" }, // 黑体, SimHei
                            { "\u65B0\u5B8B\u4F53", "NSimSun" }, // 新宋体, NSimSun
                            { "\u5FAE\u8F6F\u96C5\u9ED1", "Microsoft YaHei" }, // 微软雅黑, Microsoft YaHei
                            { "\u4EFF\u5B8B", "FangSong" }, // 仿宋, FangSong
                            { "\u6977\u4F53", "KaiTi" }, // 楷体, KaiTi
                            { "\u4EFF\u5B8B_GB2312", "FangSong_GB2312" }, // 仿宋_GB2312, FangSong_GB2312
                            { "\u65B0\u7D30\u660E\u9AD4", "PMingLiU" }, // 新細明體, PMingLiU
                            { "\u65B0\u7D30\u660E\u9AD4-ExtB", "PMingLiU-ExtB" }, // 新細明體-ExtB, PMingLiU-ExtB
                            { "\u7D30\u660E\u9AD4", "MingLiU" }, // 細明體, MingLiU
                            { "\u7D30\u660E\u9AD4-ExtB", "MingLiU-ExtB" }, // 細明體-ExtB, MingLiU-ExtB
                            { "\u5FAE\u8EDF\u6B63\u9ED1\u9AD4", "Microsoft JhengHei" }, // 微軟正黑體, Microsoft JhengHei
                            { "\u6A19\u6977\u9AD4", "DFKai-SB" }, // 標楷體, DFKai-SB
                            { "\u6587\u6cc9\u9a5b\u6b63\u9ed1", "WenQuanYi Zen Hei" }, // WenQuanYi Zen Hei
                            { "\u6587\u6cc9\u9a7f\u6b63\u9ed1", "WenQuanYi Zen Hei" }, // WenQuanYi Zen Hei
                            { "\u6587\u9f0e\u0050\u004c\u7d30\u4e0a\u6d77\u5b8b\u0055\u006e\u0069", "AR PL ShanHeiSun Uni" }, // AR PL ShanHeiSun Uni,
                            { "\u6587\u9f0e\u0050\u004c\u7ec6\u4e0a\u6d77\u5b8b\u0055\u006e\u0069", "AR PL ShanHeiSun Uni" }, // AR PL ShanHeiSun Uni,
                            { "\u6587\u0050\u004C\u4E2D\u6977\u0055\u006E\u0069", "AR PL ZenKai Uni" }, // AR PL ZenKai Uni
                            { "\u7d30\u66e0\u9AD4_HKSCS", "MingLiU_HKSCS" }, // 細明體_HKSCS, MingLiU_HKSCS
                            { "\u7d30\u66e0\u9AD4_HKSCS-ExtB", "MingLiU_HKSCS-ExtB" } // 細明體_HKSCS-ExtB, MingLiU_HKSCS-ExtB
                            //{ "\u6977\u4F53", "KaiTi_GB2312" }  // 楷体_GB2312, KaiTi_GB2312
                        };
                        #endregion
                    }
                    return _alternativeFontNames;
                }
            }

        }
    }
}