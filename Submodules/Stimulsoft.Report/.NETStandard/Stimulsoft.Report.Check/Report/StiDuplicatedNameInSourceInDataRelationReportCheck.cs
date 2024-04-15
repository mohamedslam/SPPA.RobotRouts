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
using System.Collections.Generic;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiDuplicatedNameInSourceInDataRelationReportCheck : StiReportCheck
    {
        #region Properties
        public override bool PreviewVisible => false;

        public string RelationsNames { get; set; }
        
        public string RelationsNameInSource { get; set; }

        public override string ShortMessage => 
            string.Format(StiLocalizationExt.Get("CheckReport", "StiDuplicatedNameInSourceInDataRelationReportCheckShort"), this.RelationsNames, this.RelationsNameInSource);

        public override string LongMessage => 
            string.Format(StiLocalizationExt.Get("CheckReport", "StiDuplicatedNameInSourceInDataRelationReportCheckLong"), this.RelationsNames, this.RelationsNameInSource);

        public override StiCheckStatus Status => StiCheckStatus.Warning;
        #endregion

        #region Methods
        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;

            List<StiCheck> checks = null;

            Hashtable rels = new Hashtable();            

            foreach (StiDataRelation relation in report.Dictionary.Relations)
            {
                List<StiDataRelation> list = rels[relation.NameInSource] as List<StiDataRelation>;
                if (list == null)
                {
                    list = new List<StiDataRelation>();
                    rels[relation.NameInSource] = list;
                }
                list.Add(relation);
            }

            foreach (List<StiDataRelation> list in rels.Values)
            {
                if (list.Count > 1)
                {
                    string names = string.Empty;
                    string nameInSource = string.Empty;
                    foreach (StiDataRelation relation in list)
                    {
                        if (names.Length == 0)
                            names += relation.Name;
                        else
                            names += "; " + relation.Name;

                        nameInSource = relation.NameInSource;
                    }

                    if (!string.IsNullOrEmpty(nameInSource))
                    {
                        StiDuplicatedNameInSourceInDataRelationReportCheck check = new StiDuplicatedNameInSourceInDataRelationReportCheck();
                        check.Element = obj;
                        check.RelationsNames = names;
                        check.RelationsNameInSource = nameInSource;

                        if (checks == null) checks = new List<StiCheck>();

                        checks.Add(check);
                    }
                }
            }

            return checks;
        }
        #endregion
    }
}