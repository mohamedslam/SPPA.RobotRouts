#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports 									            }
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stimulsoft.Base
{
    /// <summary>
    /// This class describes base class of Stimulsoft Server exceptions.
    /// </summary>
    public class StiServerException : Exception
    {
        #region Properties
        public StiNotice Notice { get; }
        #endregion

        public StiServerException(string message, Exception innerException = null)
            : base(message, innerException)
        {
            this.Notice = StiNotice.Create(message);
        }

        public StiServerException(StiNoticeIdent noticeIdent, Exception innerException = null)
            : base(noticeIdent.ToString(), innerException)
        {
            this.Notice = StiNotice.Create(noticeIdent);
        }

        public StiServerException(StiNotice notice, Exception innerException = null)
            : base(notice != null ? notice.ToString() : string.Empty, innerException)
        {
            this.Notice = notice;
        }

        public StiServerException(Exception innerException)
            : base(innerException != null ? innerException.Message : null, innerException)
        {
        }
    }
}
