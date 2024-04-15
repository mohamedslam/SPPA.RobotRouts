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

using System.Text;

namespace Stimulsoft.Base
{
    /// <summary>
    /// Class for adjustment aspects of Stimulsoft Reports.
    /// </summary>
    public sealed class StiBaseOptions
    {
        public static bool FullTrust { get; set; } = true;

        public static bool FIPSCompliance { get; set; }

        public static Encoding WebClientEncoding { get; set; } = Encoding.UTF8;

        public static bool WebClientCheckTlsProtocols { get; set; } = true;

        public static bool JsonParseCreatingTableProperties { get; set; }

        public static bool JsonParseTypeAsString { get; set; }

        public static bool TryParseDateTime { get; set; } = true;

        public static StiJsonConverterVersion DefaultJsonConverterVersion { get; set; } = StiJsonConverterVersion.ConverterV2;

        public static string RetrieveSchemaNamePostgreSql { get; set; } = "public";

        public static bool FixBandedGradients { get; set; }

        public static bool? ForceLoadExtendedRichTextLibrary { get; set; } = true;

        public static string ExtendedRichTextLibraryClassName { get; set; } = "RICHEDIT50W";

        public static bool AllowInsertLineBreaksWhenSavingByteArray { get; set; } = true;

        internal static bool IsDashboardViewerWPF { get; set; }

        private static bool allowSetCurrentDirectory { get; set; } = true;
        public static bool AllowSetCurrentDirectory
        {
            get
            {
                return allowSetCurrentDirectory && FullTrust;
            }
            set
            {
                allowSetCurrentDirectory = value;
            }
        }

        public static bool AllowHtmlListItemSecondLineIndent { get; set; } = true;

    }
}
