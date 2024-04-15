﻿#region Copyright (C) 2003-2022 Stimulsoft
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

namespace Stimulsoft.Report.Painters
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class StiWpfShapeTypePainterAttribute : Attribute
    {
		#region Properties
		private string painterTypeName = string.Empty;
		public string PainterTypeName
		{
			get
			{
				return this.painterTypeName;
			}
		}
        #endregion

		public StiWpfShapeTypePainterAttribute(string painterTypeName)
		{
			this.painterTypeName = painterTypeName;
		}

		public StiWpfShapeTypePainterAttribute(Type painterType)
		{
			this.painterTypeName = painterType.AssemblyQualifiedName;
		}
    }
}