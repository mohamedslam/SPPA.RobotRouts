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

namespace Stimulsoft.Report.Components.TextFormats
{
	/// <summary>
	/// Class describes the attribute that associates with the component of the component designer.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class StiWpfFormatEditorAttribute : Attribute
	{
		#region Properties
	    public string EditorTypeName { get; }
	    #endregion

		public StiWpfFormatEditorAttribute(string editorTypeName)
		{
			this.EditorTypeName = editorTypeName;
		}

        public StiWpfFormatEditorAttribute(Type type)
		{
			this.EditorTypeName = type.AssemblyQualifiedName;
		} 
	}
}