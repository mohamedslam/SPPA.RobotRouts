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

using System.Collections;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.QuickButtons;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Design
{
	public interface IStiDesignerBase
	{
		#region Properties
        IStiDesignerBase DesignerControl { get; }

        StiDesignerInfo Info { get; }

		bool IsModalMode { get; set; }

		StiNodesMemory NodesMemory { get; }		

		bool UseAliases { get; set; }

		StiReport Report { get; set; }

		Hashtable TextAliasesHash { get; }

		StiComponentsCollection SelectedComponentsOnPage { get; }

        StiComponentsCollection SelectedTextComponentsOnPage { get; }

        StiComponentsCollection ComponentsOnReport { get; }
        
        StiComponentsCollection ComponentsOnPage { get; }
		#endregion

		#region Methods
		StiQuickButton[] GetQuickButtons(StiComponent component);		

		double XToPage(int x);

		double YToPage(int y);

	    void ShowReportCheckerError(StiReportCheckerErrorParameters parameters);

		void UndoRedoSave(string name);

        void UndoRedoSave(string name, bool onlyCurrentPage);

		void AddOwnedForm(Form form);

		void Refresh();

		void Invalidate(StiComponent component);

        StiComponentsCollection GetSelected();

		#endregion
	}
}