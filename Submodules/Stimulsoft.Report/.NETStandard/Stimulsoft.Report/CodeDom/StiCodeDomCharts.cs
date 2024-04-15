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

using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using System.CodeDom;

namespace Stimulsoft.Report.CodeDom
{
    internal class StiCodeDomCharts
	{
        internal static void Serialize(StiCodeDomSerializator serializator, StiReport report)
        {
            if (report.CalculationMode == StiCalculationMode.Interpretation)
                return;

            var comps = report.GetComponents();

            foreach (StiComponent comp in comps)
            {
                var chart = comp as StiChart;
                if (chart == null)continue;

                var seriesIndex = 0;
                foreach (IStiSeries series in chart.Series)
                {
                    var filterIndex = 0;
                    foreach (IStiChartFilter filter in series.Filters)
                    {
                        if (filter.Item == StiFilterItem.Expression)
                        {
                            var name = $"{chart.Name}Filters_{seriesIndex}_{filterIndex}";
                            AddFilterMethod(serializator, report, name, filter.Value);

                            if (serializator.Members.Count > 0)
                            {
                                var ctm = serializator.Members[serializator.Members.Count - 1] as CodeMemberMethod;
                                if (ctm != null)
                                    ctm.Statements.Insert(0, new CodeCommentStatement(StiCodeDomSerializator.GetCheckerInfoString(comp.Name, "Filters")));
                            }
                        }
                        filterIndex++;
                    }
                    seriesIndex++;
                }
            }            
        }

        internal static void AddFilterMethod(StiCodeDomSerializator serializator, StiReport report, string methodName, string expression)
        {
            expression = StiCodeDomFunctions.ParseFunctions(serializator, expression);

            serializator.GenReturnMethodForExpresion(methodName, expression, typeof(bool));
        }
	}
}
