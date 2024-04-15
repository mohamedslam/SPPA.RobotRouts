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

using Stimulsoft.Report.Components;
using Stimulsoft.Report.Design;
using Stimulsoft.Report.QuickButtons;
using System;
using System.Collections;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Web
{
    public class StiWebDesignerBase: IStiDesignerBase
    {
        #region Properties
        public IStiDesignerBase DesignerControl { get; }

        public StiDesignerInfo Info { get; }

        public bool IsModalMode { get; set; }

        public StiNodesMemory NodesMemory { get; }

        public bool UseAliases { get; set; }

        public StiReport Report { get; set; }

        public Hashtable TextAliasesHash { get; }

        public StiComponentsCollection SelectedComponentsOnPage { get; }

        public StiComponentsCollection SelectedTextComponentsOnPage { get; }

        public StiComponentsCollection ComponentsOnReport { get; }

        public StiComponentsCollection ComponentsOnPage { get; }
        #endregion

        #region Methods
        public void Invalidate(StiComponent component)
        {
            throw new NotImplementedException();
        }

        public StiQuickButton[] GetQuickButtons(StiComponent component)
        {
            return null;
        }

        public double XToPage(int x)
        {
            return 0;
        }

        public double YToPage(int y)
        {
            return 0;
        }

        public void ShowReportCheckerError(StiReportCheckerErrorParameters parameters)
        {
        }

        public void UndoRedoSave(string name)
        {
        }

        public void UndoRedoSave(string name, bool onlyCurrentPage)
        {
        }

        public void AddOwnedForm(Form form)
        {
        }

        public void Refresh()
        {
        }

        public StiComponentsCollection GetSelected()
        {
            return null;
        }
        #endregion
    }
}
