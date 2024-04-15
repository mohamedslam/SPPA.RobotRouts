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

using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
	public class StiOddEvenStylesHelper
	{
	    /// <summary>
	    /// Applies OddStyle and EvenStyle to the specified container that is formed on the base of a band that realizes the IStiOddEvenStyles interface.
	    /// </summary>
	    /// <param name="report">The report.</param>
	    /// <param name="styles">A band that realizes the IStiOddEvenStyles interface that formed the specified container.</param>
	    /// <param name="cont">A container formed of the specified band.</param>
        public static StiBaseStyle ApplyOddEvenStyles(StiReport report, IStiOddEvenStyles styles, StiContainer cont)
		{
            if (report == null || report.Styles.Count <= 0) return null;
		    if (styles == null) return null;

		    StiBaseStyle parentStyle = null;

            var comp = styles as StiComponent;
		    var flag = true;

		    #region Event Style
		    if (comp.IsPropertyPresent(StiDataBand.PropertyEvenStyle))
		    {
		        var isEven = styles is StiDataBand 
		            ? (((StiDataBand) styles).Position & 1) == 1 
		            : (report.LineThrough & 1) == 0;

		        if (isEven)
		        {
		            var eventStyle = styles.EvenStyle;
		            if (!string.IsNullOrEmpty(eventStyle))
		            {
		                var st = report.Styles[eventStyle];
		                if (st != null)
		                {
		                    st.SetStyleToComponent(cont);
		                    parentStyle = st;
		                    flag = false;
		                }
		            }
		        }
		    }
		    #endregion

		    #region Odd Style
		    if (flag && comp.IsPropertyPresent(StiDataBand.PropertyOddStyle))
		    {
		        var isEven = styles is StiDataBand 
		            ? (((StiDataBand) styles).Position & 1) == 1 
		            : (report.LineThrough & 1) == 0;

		        if (!isEven)
		        {
		            var oddStyle = styles.OddStyle;
		            if (!string.IsNullOrEmpty(oddStyle))
		            {
		                var st = report.Styles[oddStyle];
		                if (st != null)
		                {
		                    st.SetStyleToComponent(cont);
		                    parentStyle = st;
		                }
		            }
		        }
		    }
		    #endregion

		    return parentStyle;
		}
	}
}
