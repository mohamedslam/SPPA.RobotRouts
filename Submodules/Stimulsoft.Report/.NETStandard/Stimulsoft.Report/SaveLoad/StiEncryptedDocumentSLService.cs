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
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.SaveLoad
{
    /// <summary>
    /// Describes the class that allows to save / load packed documents.
    /// </summary>
    public class StiEncryptedDocumentSLService : StiDocumentSLService
    {
        #region StiService override
        /// <summary>
        /// Gets a service type.
        /// </summary>
        public override Type ServiceType => typeof(StiDocumentSLService);
        #endregion

        #region Properties
        /// <summary>
        /// Key for encryption.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// If the provider handles with multitude of files then true. If does not then false.
        /// </summary>
        public override bool MultiplePages => false;
        #endregion

        #region Methods
        /// <summary>
        /// Saves the current document into the stream.
        /// </summary>
        /// <param name="report">Rendered report for saving.</param>
        /// <param name="stream">Stream to save documents.</param>
        public override void Save(StiReport report, Stream stream)
        {
            byte[] bytes = StiEncryption.Encrypt(StiGZipHelper.Pack(report.SaveDocumentToByteArray()), Key);
            byte[] dest = new byte[bytes.Length + 3];
            dest[0] = (byte)'m';
            dest[1] = (byte)'d';
            dest[2] = (byte)'x';
            Array.Copy(bytes, 0, dest, 3, bytes.Length);

            stream.Write(dest, 0, dest.Length);
        }

        /// <summary>
        /// Loads a document from the stream.
        /// </summary>
        /// <param name="report">Report in which loading will be done.</param>
        /// <param name="stream">Stream to load document.</param>
        public override void Load(StiReport report, Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, bytes.Length);

            if ((char)bytes[0] != 'm' || (char)bytes[1] != 'd' || (char)bytes[2] != 'x')
            {
                throw new Exception("This file is a not '.mdx' format.");
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

            report.LoadDocument(dest);
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
            return StiLocalization.Get("FileFilters", "EncryptedDocumentFiles");
        }
        #endregion
    }
}
