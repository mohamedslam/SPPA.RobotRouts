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
using Stimulsoft.Base;
using System.IO;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.SaveLoad
{
    /// <summary>
    /// Describes the class that allows to save/load encrypted reports.
    /// </summary>
    public class StiEncryptedReportSLService : StiReportSLService
    {
        #region StiService override
        /// <summary>
        /// Gets a service type.
        /// </summary>
        public override Type ServiceType => typeof(StiReportSLService);
        #endregion

        #region Properties
        /// <summary>
        /// Key for encryption.
        /// </summary>
        public string Key { get; set; } = string.Empty;
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

                byte[] bytes = StiEncryption.Encrypt(StiGZipHelper.Pack(report.SaveToByteArray()), Key);
                byte[] dest = new byte[bytes.Length + 3];
                dest[0] = (byte)'m';
                dest[1] = (byte)'r';
                dest[2] = (byte)'x';
                Array.Copy(bytes, 0, dest, 3, bytes.Length);

                stream.Write(dest, 0, dest.Length);

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

                if ((char)bytes[0] != 'm' || (char)bytes[1] != 'r' || (char)bytes[2] != 'x')
                {
                    throw new Exception("This file is a not '.mrx' format.");
                }

                byte[] dest = new byte[bytes.Length - 3];
                Array.Copy(bytes, 3, dest, 0, bytes.Length - 3);
                dest = StiEncryption.Decrypt(dest, Key);

                try
                {
                    dest = StiGZipHelper.Unpack(dest);
                }
                catch
                {
                    throw new Exception("File decryption error: wrong key.");
                }

                report.Load(dest);

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
            return StiLocalization.Get("FileFilters", "EncryptedReportFiles");
        }
        #endregion
    }
}
