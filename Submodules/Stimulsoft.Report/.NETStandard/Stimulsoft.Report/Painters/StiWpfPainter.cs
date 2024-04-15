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
using System;
using System.Collections;

namespace Stimulsoft.Report.Painters
{
    public abstract class StiWpfPainter : StiPainter
    {
        #region Methods
        public static Stimulsoft.Base.Drawing.SizeD MeasureComponent(double width, StiComponent comp)
        {
            var painter = StiWpfPainter.GetPainter(comp.GetType(), Stimulsoft.Base.StiGuiMode.Wpf) as StiWpfPainter;
            if (painter != null)
                return painter.Measure(width, comp);

            throw new Exception($"StiWpfPainter for component '{comp}' is not found!");
        }

        public virtual Stimulsoft.Base.Drawing.SizeD Measure(double width, StiComponent comp)
        {
            return Stimulsoft.Base.Drawing.SizeD.Empty;
        }

        protected Hashtable GetHashtableForViewWpfPainter(StiReport report)
        {
            if (report.HashViewWpfPainter == null)
                report.HashViewWpfPainter = new Hashtable();

            return report.HashViewWpfPainter;
        }
        #endregion
    }
}
