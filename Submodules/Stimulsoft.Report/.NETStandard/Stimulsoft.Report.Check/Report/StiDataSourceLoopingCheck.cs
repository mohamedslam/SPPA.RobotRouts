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
using Stimulsoft.Report.Components;
using System;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Check
{
    public class StiDataSourceLoopingCheck : StiReportCheck
    {
        #region Properties
        public override string ElementName
        {
            get
            {
                if (Element == null)
                    return null;

                if (Element is StiDataSource)
                    return ((StiDataSource)Element).Name;

                return null;
            }
        }

        public override bool PreviewVisible => false;

        public override string ShortMessage => 
            string.Format(StiLocalizationExt.Get("CheckReport", "StiDataSourceLoopingCheckShort"));

        public override string LongMessage => 
            string.Format(StiLocalizationExt.Get("CheckReport", "StiDataSourceLoopingCheckLong"), band1, band2, this.ElementName);

        public override StiCheckStatus Status => StiCheckStatus.Warning;
        #endregion

        #region Fields
        private string band1 = null;
        private string band2 = null;
        #endregion

        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;

            #region Prepare
            var isModified = report.IsModified;//#6421
            var strSav = report.SaveToString();
            report.IsModified = isModified;

            var report2 = new StiReport();
            report2.LoadFromString(strSav);

            var builder = new Engine.StiDataBandV2Builder();
            List<StiDataBand> loops = new List<StiDataBand>();

            var comps = report2.GetComponents();
            foreach(StiComponent comp in comps)
            {
                var dataBand = comp as StiDataBand;
                if(dataBand != null)
                {
                    builder.FindDetailDataBands(dataBand, loops);
                    builder.FindDetails(dataBand, loops);
                }
            }
            #endregion

            List<StiCheck> checks = null;

            for (int index = 0; index < loops.Count; index += 2)
            {
                var masterBand = loops[index];
                var slaveBand = loops[index + 1];

                StiDataSourceLoopingCheck check = new StiDataSourceLoopingCheck
                {
                    Element = report.Dictionary.DataSources[masterBand.DataSource.Name],
                    band1 = masterBand.Name,
                    band2 = slaveBand.Name
                };

                if (checks == null) checks = new List<StiCheck>();

                checks.Add(check);
            }

            report2.Dispose();

            return checks;
        }        
    }
}