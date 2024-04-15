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

namespace Stimulsoft.Report.Viewer
{
	#region StiPageViewMode
	public enum StiPageViewMode
	{
		SinglePage,
		Continuous,
		MultiplePages
	}
	#endregion

	#region StiPreviewSettings
	[Flags]	
	public enum StiPreviewSettings
	{	
		All				=	0xfffffff,
		None			=	0x0000000,
        Default = All ^ PageDesign ^ PageDelete ^ PageNew ^ PageSize ^ Close,    
	
		PageViewMode	=	0x0000001,
		VertScrollBar	=	0x0000002,
		HorScrollBar	=	0x0000004,						
		
		StatusBar		=	0x0000008,
		
		Print			=	0x0000010,
		Open			=	0x0000020,
		Save			=	0x0000040,
        //[Obsolete("Please use 'Save' value instead 'Export' value.")]
        //Export			=	0x0000080,
        Parameters      =   0x0000080,
        SendEMail		=	0x0000100,
		PageNew			=	0x0000200,
		PageDelete		=	0x0000400,
		PageDesign		=	0x0000800,
		PageSize		=	0x0001000,

		//[Obsolete("Value 'SelectTool' is obsolete.")]
		//SelectTool		=	0x0002000,
        Resources       =   0x0002000,
        [Obsolete("Value 'HandTool' is obsolete.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        HandTool		=	0x0008000,
		[Obsolete("Please use 'Editor' value instead 'EditorTool' value.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        EditorTool		=	0x0010000,
		[Obsolete("Please use 'Find' value instead 'FindTool' value.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        FindTool		=	0x0020000,

		Editor	=			0x0010000,
		Find			=	0x0020000,

		Zoom			=	0x0040000,
		PageControl		=	0x0080000,

		Bookmarks		=	0x0100000,
		Thumbs			=	0x0200000,
		ContextMenu		=	0x0400000,
		Close			=	0x0800000,
		Toolbar			=	0x1000000,
		Signature		=	0x2000000,
		Help			=	0x4000000
	}
	#endregion
}
