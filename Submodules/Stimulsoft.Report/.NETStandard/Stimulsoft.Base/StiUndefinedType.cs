#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using System.Reflection;

namespace Stimulsoft.Base
{
    /// <summary>
    /// Class describes a special class wrapper for the undefined type.
    /// </summary>
    public class StiUndefinedType : TypeDelegator
    {
        #region Properties
        /// <summary>
        /// Gets or sets the string representation of the type.
        /// </summary>
        public string Type { get; }

        public override Type BaseType => typeof(object);

        public override Type UnderlyingSystemType => typeof(object);

        public override string Name => Type;

        public override string FullName => Type;
        #endregion

        #region Methods
        protected override bool IsValueTypeImpl()
        {
            return false;
        }

        protected override bool IsArrayImpl()
        {
            return false;
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            return new TypeAttributes();
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            return new PropertyInfo[0];
        }

        public override Type[] GetInterfaces()
        {
            return new Type[0];
        }
        
        public override string ToString()
        {
            return Type;
        }
        #endregion

        /// <summary>
        /// Creates a new instance of the StiUndefinedType class.
        /// </summary>
        public StiUndefinedType(string type)
        {
            this.Type = type;
        }
    }
}
