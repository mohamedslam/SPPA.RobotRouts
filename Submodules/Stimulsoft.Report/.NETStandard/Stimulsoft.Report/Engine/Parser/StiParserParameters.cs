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

namespace Stimulsoft.Report.Engine
{
    public class StiParserParameters
    {
        public bool StoreToPrint { get; set; }

        public bool ExecuteIfStoreToPrint { get; set; } = true;

        public bool ReturnAsmList { get; set; }

        public bool CheckSyntaxMode { get; set; }

        public bool? SyntaxCaseSensitive { get; set; }

        public StiParser Parser { get; set; }

        public Hashtable ConversionStore { get; set; }

        public string GlobalizedNameExt { get; set; }

        public bool IgnoreGlobalizedName { get; set; }

        public Hashtable Constants { get; set; }

        public StiParser.StiParserGetDataFieldValueDelegate GetDataFieldValue { get; set; }

        public bool UseAliases { get; set; }

        //for internal using only
        internal Hashtable VariablesRecursionCheck { get; set; }
    }
}