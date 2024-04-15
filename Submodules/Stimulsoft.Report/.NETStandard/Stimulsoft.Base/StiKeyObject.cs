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

using Stimulsoft.Base.Databases;
using Stimulsoft.Base.Json;

namespace Stimulsoft.Base
{
    /// <summary>
    /// This is a base class for all objects which store information.
    /// </summary>
    public abstract class StiKeyObject : StiObject
    {
        #region Properties
        /// <summary>
        /// Gets or sets unically object identificator.
        /// </summary>
        [StiTableField]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets property which indicates that this object stored in the Stimulsoft Server database or not.
        /// </summary>
        [StiIgnoreTableField]
        [JsonIgnore]
        protected internal bool? IsStored { get; set; }
        #endregion

        public StiKeyObject()
        {
            this.Key = StiKeyHelper.GenerateKey();
        }
    }
}
