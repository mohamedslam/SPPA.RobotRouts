#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

namespace Stimulsoft.Base
{
    public class StiCsvOptions : StiFileDataOptions
    {
        public int MaxDataRows { get; set; }

        public string Path { get; }

        public string TableName { get; }

        public int CodePage { get; }

        public string Separator { get; }

        public StiCsvOptions(byte[] content, int codePage = 0, string separator = ";") : this(content, null, codePage, separator)
        {
        }

        public StiCsvOptions(string path, int codePage = 0, string separator = ";")
            : this(path, null, codePage, separator)
        {
        }

        public StiCsvOptions(byte[] content, string tableName, int codePage = 0, string separator = ";") : base(content)
        {
            this.TableName = tableName;
            this.CodePage = codePage;
            this.Separator = separator;
        }

        public StiCsvOptions(string path, string tableName, int codePage = 0, string separator = ";")
            : base(null)
        {
            this.Path = path;
            this.TableName = tableName;
            this.CodePage = codePage;
            this.Separator = separator;
        }
    }
}