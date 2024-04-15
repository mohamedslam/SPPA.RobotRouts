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
using System.Data;

namespace Stimulsoft.Base
{
    public class StiDbTypeConversion
    {
        /// <summary>
        /// Returns a .NET type from the specified string representaion of the database type.
        /// </summary>
        public static Type GetNetType(DbType type)
        {
            switch (type)
            {
                case DbType.SByte:
                    return typeof (SByte);

                case DbType.Int16:
                    return typeof (Int16);

                case DbType.Int32:
                    return typeof (Int32);

                case DbType.Int64:
                    return typeof (Int64);

                case DbType.Byte:
                    return typeof (Byte);

                case DbType.UInt16:
                    return typeof (UInt16);

                case DbType.UInt32:
                    return typeof (UInt32);

                case DbType.UInt64:
                    return typeof (UInt64);

                case DbType.Single:
                    return typeof (Single);

                case DbType.Double:
                    return typeof (Double);

                case DbType.Decimal:
                case DbType.Currency:
                    return typeof (Decimal);

                case DbType.Guid:
                    return typeof (Guid);

                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.Time:
                    return typeof (DateTime);

                case DbType.DateTimeOffset:
                    return typeof (DateTimeOffset);

                case DbType.Boolean:
                    return typeof (Boolean);

                case DbType.Binary:
                    return typeof (byte[]);

                case DbType.String:
                case DbType.StringFixedLength:
                    return typeof (String);

                default:
                    return typeof (string);
            }
        }
    }
}
