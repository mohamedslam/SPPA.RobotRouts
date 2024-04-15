#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Data.Engine;
using System;
using System.Linq;

namespace Stimulsoft.Report.Dashboard
{
    public static class StiElementChangedProcessor
    {
        public static void ProcessElementChanging(object element, StiElementChangedArgs args)
        {
            switch (args.Action)
            {
                case StiElementMeterAction.Rename:
                    ProcessElementRenaming(element, args);
                    break;

                case StiElementMeterAction.ClearAll:
                    ProcessElementClearing(element);
                    break;

                case StiElementMeterAction.Delete:
                    ProcessElementDeleting(element, args);
                    break;
            }
        }

        private static void ProcessElementRenaming(object element, StiElementChangedArgs args)
        {
            var actions = element as IStiTransformActions;
            actions?.TransformActions?.Where(a => a.Path == args.OldName).ToList()
                .ForEach(a => a.Path = args.NewName);

            var filters = element as IStiTransformFilters;
            filters?.TransformFilters?.Where(a => a.Path == args.OldName).ToList()
                .ForEach(a => a.Path = args.NewName);
        }

        private static void ProcessElementClearing(object element)
        {
            var actions = element as IStiTransformActions;
            actions?.TransformActions?.Clear();

            var filters = element as IStiTransformFilters;
            filters?.TransformFilters?.Clear();
        }

        private static void ProcessElementDeleting(object element, StiElementChangedArgs args)
        {
            var actions = element as IStiTransformActions;
            actions?.TransformActions?.Where(a => a.Path == args.OldName).ToList()
                .ForEach(a => actions?.TransformActions.Remove(a));

            var filters = element as IStiTransformFilters;
            filters?.TransformFilters?.Where(a => a.Path == args.OldName).ToList()
                .ForEach(a => filters?.TransformFilters.Remove(a));
        }
    }
}
