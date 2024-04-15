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

namespace Stimulsoft.Base.Forms.Toolbox
{
    internal class StiFormsToolboxActions
    {
        #region Methods
        internal static void DoDrag(object element)
        {
            var typeFullName = StiFormsCreater.GetTypeName("StiDesignerActions");

            var type = Type.GetType($"{typeFullName}, Stimulsoft.Form.Wpf");

            var method = type.GetMethod("DoDrag");
            method.Invoke(null, new object[] { element });
        }

        internal static void DoubleClick(object formDesignControl, object element)
        {
            var typeFullName = StiFormsCreater.GetTypeName("StiDesignerActions");

            var type = Type.GetType($"{typeFullName}, Stimulsoft.Form.Wpf");

            var method = type.GetMethod("DoubleClick");
            method.Invoke(null, new object[] { formDesignControl, element });
        }
        #endregion
    }
}
