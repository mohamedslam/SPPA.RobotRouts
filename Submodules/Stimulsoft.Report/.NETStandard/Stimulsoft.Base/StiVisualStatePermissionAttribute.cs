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
using System.ComponentModel;
using System.Linq;

namespace Stimulsoft.Base
{
    /// <summary>
    /// Is used for marking property with allowed Default and Style value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class StiVisualStatePermissionAttribute : Attribute
    {
        #region Properties
        public StiVisualStatePermissionKind Kinds { get; set; }
        #endregion

        #region Methods.Static
        public static bool IsFromDefaultStateAllowed(ITypeDescriptorContext context)
        {
            var visualStateAttr = context.PropertyDescriptor.Attributes.Cast<Attribute>()
                .FirstOrDefault(a => a is StiVisualStatePermissionAttribute) as StiVisualStatePermissionAttribute;
            var brushKinds = visualStateAttr != null ? visualStateAttr.Kinds : StiVisualStatePermissionKind.Deny;

            return brushKinds == StiVisualStatePermissionKind.AllowDefaultAndStyle;
        }
        #endregion

        public StiVisualStatePermissionAttribute(StiVisualStatePermissionKind kinds)
        {
            this.Kinds = kinds;
        }
    }
}