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

namespace Stimulsoft.Report.Viewer
{
	/// <summary>
	/// Desribed base interface for all types of report viewer forms.
	/// </summary>
	public interface IStiViewerForm
	{
		/// <summary>
		/// Gets or sets viewed report.
		/// </summary>
		StiReport Report { get; set; }

		bool ShowInTaskbar { get; set; }

		bool TopLevel { get; set; }

		object ViewerOwner { get; set; }

		object GetOwner(object form);

		void ShowViewer();

		void ShowDialogViewer();

		void ShowViewer(object win32Window);

        void ShowDialogViewer(object win32Window);
	}
}
