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

using System.Linq;

namespace Stimulsoft.Report.Chart
{
    public static class StiBoxAndWhiskerHelper
    {
        #region Methods
        public static void CheckArgument(StiChart chart)
        {
            var series = chart.Series.ToList().Where(s => s is StiBoxAndWhiskerSeries).ToList();
            if (series.Count > 0)
            {
                var arguments = new object[series.Count];
                for(var index = 0; index < series.Count; index++)
                {
                    arguments[index] = series[index].CoreTitle;
                }

                for (var index = 0; index < series.Count; index++)
                {
                    series[index].Arguments = arguments;
                    series[index].OriginalArguments = arguments;
                }                 
            }
        }
        #endregion
    }
}
