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

using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiKeysNotFoundRelationCheck : StiDataRelationCheck
    {
        #region Properties
        public override bool PreviewVisible
        {
            get
            {
                return true;
            }
        }

        private string columns;
        public string Columns
        {
            get
            {
                return columns;
            }
            set
            {
                columns = value;
            }
        }

        public override string ShortMessage
        {
            get
            {
                return string.Format(StiLocalizationExt.Get("CheckDataRelation", "StiKeysNotFoundRelationCheckShort"), this.ElementName, this.Columns);
            }
        }

        public override string LongMessage
        {
            get
            {
                return string.Format(StiLocalizationExt.Get("CheckDataRelation", "StiKeysNotFoundRelationCheckLong"), this.ElementName, this.Columns);
            }
        }

        public override StiCheckStatus Status
        {
            get
            {
                return StiCheckStatus.Error;
            }
        }
        #endregion

        #region Methods
        private bool IsColumnsExist(StiDataRelation relation)
        {
            foreach (string column in relation.ParentColumns)
            {
                if (!relation.ParentSource.Columns.Contains(column))return false;
            }

            foreach (string column in relation.ChildColumns)
            {
                if (!relation.ChildSource.Columns.Contains(column)) return false;
            }

            return true;
        }

        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;

            StiDataRelation relation = obj as StiDataRelation;
            if (relation != null && 
                relation.ParentSource != null && relation.ChildSource != null &&                 
                (!IsColumnsExist(relation)))
            {
                bool finded = false;
                string columns = string.Empty;

                foreach (string column in relation.ParentColumns)
                {
                    if (!relation.ParentSource.Columns.Contains(column))
                    {
                        if (columns.Length == 0)
                            columns += column;
                        else
                            columns += "; " + column;

                        finded = true;
                    }
                }

                foreach (string column in relation.ChildColumns)
                {
                    if (!relation.ChildSource.Columns.Contains(column))
                    {
                        if (columns.Length == 0)
                            columns += column;
                        else
                            columns += "; " + column;

                        finded = true;
                    }
                }                

                if (finded)
                {
                    StiKeysNotFoundRelationCheck check = new StiKeysNotFoundRelationCheck();
                    check.columns = columns;
                    check.Element = obj;
                    return check;
                }
            }
            return null;
        }
        #endregion
    }
}