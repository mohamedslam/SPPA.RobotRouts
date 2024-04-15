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
    public class StiNoNameDataRelationCheck : StiDataRelationCheck
    {
        #region Properties
        public override bool PreviewVisible
        {
            get
            {
                return true;
            }
        }

        private string dataSources;
        public string DataSources
        {
            get
            {
                return dataSources;
            }
            set
            {
                dataSources = value;
            }
        }

        public override string ShortMessage
        {
            get
            {
                return string.Format(StiLocalizationExt.Get("CheckDataRelation", "StiNoNameDataRelationCheckShort"), this.DataSources);
            }
        }

        public override string LongMessage
        {
            get
            {
                return string.Format(StiLocalizationExt.Get("CheckDataRelation", "StiNoNameDataRelationCheckLong"), this.DataSources);
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
        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;

            StiDataRelation relation = obj as StiDataRelation;
            if (relation != null && string.IsNullOrEmpty(relation.Name))
            {
                StiNoNameDataRelationCheck check = new StiNoNameDataRelationCheck();
                check.Element = obj;

                check.dataSources = "";
                if (relation.ParentSource != null)
                    check.dataSources = relation.ParentSource.Name;

                if (relation.ParentSource != null)
                {
                    if (string.IsNullOrEmpty(check.DataSources))
                        check.dataSources = relation.ChildSource.Name;
                    else
                        check.dataSources += "; " + relation.ChildSource;
                }

                check.Actions.Add(new StiGenerateNewNameRelationAction());
                check.Actions.Add(new StiDeleteDataRelationAction());

                return check;
            }
            return null;
        }
        #endregion
    }
}