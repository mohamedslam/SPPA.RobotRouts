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
using System.Collections;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Helpers
{
    public sealed class StiConditionsHelper
    {
        #region Methods
        public static StiConditionsCollection GetConditions(StiComponentsCollection comps, ref Hashtable globalConditions)
        {
            var conditions = new StiConditionsCollection();
            foreach (StiComponent comp in comps)
            {
                var conds = comp.Conditions;

                if (conds.Count > 0)
                {
                    foreach (StiBaseCondition cond in conds)
                    {
                        var guid = Guid.NewGuid().ToString().Replace("-", "");
                        cond.Tag = guid;
                        globalConditions.Add(guid, comp);
                    }

                    conditions.AddRange(conds);
                }
            }

            return conditions;
        }

        public static void SetConditions(StiComponentsCollection comps, StiConditionsCollection conditions, Hashtable globalConditions)
        {
            // Удаляем все старые
            foreach (StiComponent comp in comps)
            {
                comp.Conditions.Clear();
            }

            foreach (StiBaseCondition cond in conditions)
            {

                if (cond.Tag != null && globalConditions.Contains(cond.Tag))
                {
                    var newCondition = cond.Clone() as StiBaseCondition;
                    newCondition.Tag = null;
                    ((StiComponent)globalConditions[cond.Tag]).Conditions.Add(newCondition);
                }
                else // иначе это условие добавили в редакторе и его надо добавить всем выделенным компонентам
                {
                    SetConditionAllComponents(cond, comps);
                }
            }
        }

        private static void SetConditionAllComponents(StiBaseCondition condition, StiComponentsCollection comps)
        {
            foreach (StiComponent comp in comps)
            {
                var newCondition = condition.Clone() as StiBaseCondition;
                newCondition.Tag = null;
                comp.Conditions.Add(newCondition);
            }
        }
        #endregion
    }
}