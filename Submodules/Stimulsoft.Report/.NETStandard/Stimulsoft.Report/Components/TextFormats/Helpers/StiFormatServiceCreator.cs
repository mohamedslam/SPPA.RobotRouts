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
using System.Linq;
using Stimulsoft.Report;

namespace Stimulsoft.Report.Components.TextFormats
{
    /// <summary>
    /// Class helps in creation new object of the StiMeter type.
    /// </summary>
    internal static class StiFormatServiceCreator
    {
        #region Methods
        /// <summary>
        /// Creates new format service object with help of its identification type name.
        /// </summary>
        /// <param name="identName">A name of the identification type which is used for the format service creation.</param>
        /// <returns>Created format service object.</returns>
        public static StiFormatService New(string identName)
        {
            return StiOptions.Services.Formats.FirstOrDefault(x => x.GetType().Name == identName).CreateNew();
        }
        #endregion
    }
}
