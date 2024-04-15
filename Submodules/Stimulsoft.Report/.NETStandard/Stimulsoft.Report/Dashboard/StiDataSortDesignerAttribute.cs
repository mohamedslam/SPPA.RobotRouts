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

using System;

namespace Stimulsoft.Report.Components.Design
{
    /// <summary>
    /// This class describes the attribute that associates with an element of a data sort designer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
	public sealed class StiDataSortDesignerAttribute : Attribute
	{
		#region Properties
	    public string DesignerTypeName { get; }
	    #endregion

		public StiDataSortDesignerAttribute(string designerTypeName)
		{
			this.DesignerTypeName = designerTypeName;
		}

		public StiDataSortDesignerAttribute(Type type)
		{
			this.DesignerTypeName = type.AssemblyQualifiedName;
		} 
	}
}
