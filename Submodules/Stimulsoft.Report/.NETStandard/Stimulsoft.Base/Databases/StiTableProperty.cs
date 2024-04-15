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
using System.Reflection;

namespace Stimulsoft.Base.Databases
{
    public class StiTableProperty
    {
        #region Properties
        public string Name { get; set; }

        public Type Type { get; set; }

        public PropertyInfo PropertyInfo { get; }

        public bool IsJson => PropertyInfo == null;

        public bool IsTableField => PropertyInfo.GetCustomAttributes(typeof(StiTableFieldAttribute), true).Length > 0;
        #endregion

        #region Methods
        public override string ToString()
        {
            return this.Name;
        }
        #endregion

        public StiTableProperty()
        {
            
        }

        public StiTableProperty(PropertyInfo prop)
        {
            this.Name = prop.Name;
            this.Type = prop.PropertyType;
            this.PropertyInfo = prop;
        }
    }
}
