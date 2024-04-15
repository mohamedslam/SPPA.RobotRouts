﻿
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

using Stimulsoft.Report.Dictionary.Databases.Google;
using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Stimulsoft.Report.Dictionary.Design
{
    public class StiBigQueryDatabaseConverter : StiSqlDatabaseConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
            object value, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor) && value != null)
            {
                var data = (StiBigQueryDatabase)value;

                var types = new[]
                {
                    typeof(string),
                    typeof(string),
                    typeof(string),
                    typeof(bool),
                    typeof(string)
                };

                var info = typeof(StiBigQueryDatabase).GetConstructor(types);
                if (info != null)
                {
                    var objs = new object[]
                    {
                        data.Name,
                        data.Alias,
                        data.ConnectionString,
                        data.PromptUserNameAndPassword,
                        data.Key
                    };

                    return CreateNewInstanceDescriptor(info, objs);
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
