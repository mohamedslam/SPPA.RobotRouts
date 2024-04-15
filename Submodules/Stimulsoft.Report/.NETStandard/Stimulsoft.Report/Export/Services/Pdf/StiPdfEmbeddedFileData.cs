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

using System;

namespace Stimulsoft.Report.Export
{
    public class StiPdfEmbeddedFileData
    {
        #region Properties
        public string Name { get; set; }

        public string Description { get; set; }

        public byte[] Data { get; set; }

        private string mimeType = null;
        public string MIMEType
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(mimeType)) return mimeType;

                #region Process file extension to get MIME type
                string subType = "text/plain";  //txt
                int pos = Name.LastIndexOf('.');
                if (pos != -1)
                {
                    string extension = Name.Substring(pos + 1);
                    switch (extension)
                    {
                        case "xml": subType = "text/xml"; break;
                        case "htm": subType = "text/html"; break;
                        case "html": subType = "text/html"; break;
                        default: subType = "text/plain"; break;
                    }
                }
                return subType;
                #endregion
            }
            set
            {
                mimeType = value;
            }
        }
        #endregion

        public StiPdfEmbeddedFileData(string name, string description, byte[] data, string mimeType = null)
        {
            this.Name = name;
            this.Description = description;
            this.Data = data;
            if (!string.IsNullOrWhiteSpace(mimeType)) this.MIMEType = mimeType;
        }
    }
}
