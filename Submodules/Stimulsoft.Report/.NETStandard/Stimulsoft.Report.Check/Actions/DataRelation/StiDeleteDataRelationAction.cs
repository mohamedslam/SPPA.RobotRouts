﻿#region Copyright (C) 2003-2022 Stimulsoft
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

using Stimulsoft.Report.Helper;
using Stimulsoft.Report.Dictionary;

namespace Stimulsoft.Report.Check
{
    public class StiDeleteDataRelationAction : StiAction
    {
        public override string Name => StiLocalizationExt.Get("CheckActions", "Delete");

        public override string Description => StiLocalizationExt.Get("CheckActions", "StiDeleteDataRelationActionLong");

        public override void Invoke(StiReport report, object element, string elementName)
        {
            base.Invoke(report, null, null);

            var dataRelation = element as StiDataRelation;
            if (dataRelation != null)
                report.Dictionary.Relations.Remove(dataRelation);
        }
    }
}
