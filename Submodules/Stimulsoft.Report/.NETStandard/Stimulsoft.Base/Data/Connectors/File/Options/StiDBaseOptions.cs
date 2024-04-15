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
    public class StiDBaseOptions : StiFileDataOptions
    {
        public int MaxDataRows { get; set; }

        public string Path { get; }

        public string TableName { get; }

        public int CodePage { get; }

        public StiDBaseOptions(byte[] content, int codePage = 0) : this(content, null, codePage)
        {
        }

        public StiDBaseOptions(string path, int codePage = 0) : this(path, null, codePage)
        {
        }

        public StiDBaseOptions(byte[] content, string tableName, int codePage = 0)
            : base(content)
        {
            this.TableName = tableName;
            this.CodePage = codePage;
        }

        public StiDBaseOptions(string path, string tableName, int codePage = 0)
            : base(null)
        {
            this.Path = path;
            this.TableName = tableName;
            this.CodePage = codePage;
        }
    }
}