#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	Stimulsoft.Report Library										}
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
using System.IO;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.SaveLoad
{
    /// <summary>
    /// Describes the class that allows to save/load packed reports.
    /// </summary>
    public class StiPackedReportSLService : StiReportSLService
    {
        #region StiService override
        /// <summary>
        /// Gets a service type.
        /// </summary>
        public override Type ServiceType => typeof(StiReportSLService);
        #endregion

        #region Methods
        /// <summary>
        /// Saves report in the stream.
        /// </summary>
        /// <param name="report">Report for saving.</param>
        /// <param name="stream">Stream to save report.</param>
        public override void Save(StiReport report, Stream stream)
        {
            try
            {
                report.IsSerializing = true;
                //report.ScriptPack();

                byte[] bytes = StiGZipHelper.Pack(report.SaveToByteArray());
                stream.Write(bytes, 0, bytes.Length);

                //report.ScriptUnpack();
            }
            finally
            {
                report.IsSerializing = false;
            }
        }

        /// <summary>
        /// Loads a report from the stream.
        /// </summary>
        /// <param name="report">The report in which loading will be done.</param>
        /// <param name="stream">Stream to save report.</param>
        public override void Load(StiReport report, Stream stream)
        {
            try
            {
                report.Clear();

                report.IsSerializing = true;

                StiDataCollection datas = report.DataStore;

                byte[] bytes = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(bytes, 0, bytes.Length);

                bytes = StiGZipHelper.Unpack(bytes);
                report.Load(bytes);

                report.DataStore.Clear();
                report.DataStore.RegData(datas);
                //report.ScriptUnpack();
            }
            finally
            {
                report.IsSerializing = false;
            }
        }

        /// <summary>
        /// Returns actions available for the provider.
        /// </summary>
        /// <returns>Available actions.</returns>
        public override StiSLActions GetAction()
        {
            return StiSLActions.Load | StiSLActions.Save;
        }

        /// <summary>
        /// Returns a filter for the provider.
        /// </summary>
        /// <returns>String with filter.</returns>
        public override string GetFilter()
        {
            return StiLocalization.Get("FileFilters", "PackedReportFiles");
        }
        #endregion
    }
}