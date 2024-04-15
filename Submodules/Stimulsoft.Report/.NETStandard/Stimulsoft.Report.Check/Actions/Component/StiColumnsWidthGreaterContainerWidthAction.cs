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
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiColumnsWidthGreaterContainerWidthAction : StiAction
    {
        public override string Name => StiLocalizationExt.Get("CheckActions", "Change");

        public override string Description => StiLocalizationExt.Get("CheckActions", "StiColumnsWidthGreaterContainerWidthActionLong");

        public override void Invoke(StiReport report, object element, string elementName)
        {
            base.Invoke(report, null, null);

            var page = element as StiPage;
            var band = element as StiDataBand;
            var panel = element as StiPanel;

            if (page != null)
            {
                var restWidth = page.Width - (page.Columns * page.ColumnGaps);
                if (restWidth > 0)
                {
                    page.ColumnWidth = restWidth / page.Columns;
                }
                else
                {
                    page.ColumnGaps = 0;
                    page.ColumnWidth = page.Width / page.Columns;
                }
            }
            else if (band != null)
            {
                var restWidth = band.Width - (band.Columns * band.ColumnGaps);
                if (restWidth > 0)
                {
                    band.ColumnWidth = restWidth / band.Columns;
                }
                else
                {
                    band.ColumnGaps = 0;
                    band.ColumnWidth = band.Width / band.Columns;
                }
            }
            else if (panel != null)
            {
                var restWidth = panel.Width - (panel.Columns * panel.ColumnGaps);
                if (restWidth > 0)
                {
                    panel.ColumnWidth = restWidth / panel.Columns;
                }
                else
                {
                    panel.ColumnGaps = 0;
                    panel.ColumnWidth = panel.Width / panel.Columns;
                }
            }
        }
    }
}