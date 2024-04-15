#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Base;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Design;
using System;

namespace Stimulsoft.Report.Dashboard
{
    /// <summary>
    /// This class describes the designer of the data sort for elements.
    /// </summary>
    public class StiDataSortDesigner : StiComponentDesigner
    {
        #region Methods
        /// <summary>
        /// Returns a designer of the component.
        /// </summary>
        /// <param name="designer">Report designer.</param>
        /// <param name="componentType">Component type.</param>
        /// <returns>Component designer.</returns>
        public static StiComponentDesigner GetDataSortDesigner(IStiDesignerBase designer, Type componentType)
        {
            var attrs = componentType.GetCustomAttributes(typeof(StiDataSortDesignerAttribute), true) as StiDataSortDesignerAttribute[];
            if (attrs == null || attrs.Length <= 0)
                return null;

            var designerTypeName = attrs[0].DesignerTypeName;
            var designerType = GetTypeFromName(designerTypeName);
            if (designerType == null)
                return null;

            return StiActivator.CreateObject(designerType, new object[] { designer }) as StiComponentDesigner;
        }
        #endregion

        public StiDataSortDesigner(IStiDesignerBase designer) : base(designer)
        {
        }
    }
}
