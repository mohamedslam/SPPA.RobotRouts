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
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Help;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// A class which controls of settings of the report engine. 
        /// </summary>
		public sealed partial class Engine
        {
            public sealed partial class GlobalEvents
            {
                #region OpenHelp
#if !NETSTANDARD
                public static event CustomHelpEventHandler OpenHelp;
                public static bool InvokeOpenCustomHelp(StiHelpViewerForm helpViewerForm)
                {
                    if (OpenHelp == null) return false;

                    var e = new CustomHelpEventArgs();
                    OpenHelp(helpViewerForm, e);
                    if (!e.Processed) return e.Processed;

                    StiHelpViewerForm.Url = e.Url;
                    helpViewerForm.Text = e.HeaderText;
                    return e.Processed;
                }
#endif
                #endregion

                #region GetValue
                public static event StiGetValueEventHandler GetValue;

				public static void InvokeGetValue(object sender, StiGetValueEventArgs e)
				{
                    GetValue?.Invoke(sender, e);
                }
				#endregion

				#region DataSourceAssigned
				public static event EventHandler DataSourceAssigned;

				public static void InvokeDataSourceAssigned(object sender, EventArgs e)
				{
                    DataSourceAssigned?.Invoke(sender, e);
                }
				#endregion

                #region BusinessObjectAssigned
                public static event EventHandler BusinessObjectAssigned;

                public static void InvokeBusinessObjectAssigned(object sender, EventArgs e)
                {
                    BusinessObjectAssigned?.Invoke(sender, e);
                }
                #endregion

				#region BeforePrint
				public static event EventHandler BeforePrint;

				public static void InvokeBeforePrint(object sender, EventArgs e)
				{
                    BeforePrint?.Invoke(sender, e);
                }
				#endregion

				#region AfterPrint
				public static event EventHandler AfterPrint;

				public static void InvokeAfterPrint(object sender, EventArgs e)
				{
                    AfterPrint?.Invoke(sender, e);
                }
				#endregion

				#region TextChanged
				/// <summary>
				/// Occurs when Text or TextValue of text component is changed.
				/// </summary>
				public static event StiTextChangedEventHandler TextChanged;

				public static void InvokeTextChanged(object sender, StiTextChangedEventArgs e)
				{
                    TextChanged?.Invoke(sender, e);
                }
				#endregion

				#region ComponentCreated
				public static event StiComponentCreationHandler ComponentCreated;

				public static void InvokeComponentCreated(object sender, StiComponentCreationEventArgs e)
				{
                    ComponentCreated?.Invoke(sender, e);
                }
				#endregion

				#region ComponentRemoved
				public static event StiComponentRemovingHandler ComponentRemoved;

				public static void InvokeComponentRemoved(object sender, StiComponentRemovingEventArgs e)
				{
                    ComponentRemoved?.Invoke(sender, e);
                }
				#endregion

                #region ProcessExport
                public static event StiProcessExportEventHandler ProcessExport;

                public static void InvokeProcessExport(object sender, StiProcessExportEventArgs e)
                {
                    ProcessExport?.Invoke(sender, e);
                }
                #endregion

                #region GlobalGuiStyleChanged
                /// <summary>
                /// Occurs when StiOptions.Windows.GlobaGuiStyle property is changed.
                /// </summary>
                public static event EventHandler GlobalGuiStyleChanged;

                private static void InvokeGlobalGuiStyleChanged(Control parent)
                {
                    var applyStyle = parent as IStiApplyStyle;
                    if (applyStyle != null)
                        applyStyle.ApplyStyle();

                    foreach (Control control in parent.Controls)
                    {
                        InvokeGlobalGuiStyleChanged(control);
                    }
                }

                public static void InvokeGlobalGuiStyleChanged(object sender, EventArgs e)
                {
                    foreach (Form form in Application.OpenForms)
                    {
                        InvokeGlobalGuiStyleChanged(form);                        
                    }
                    GlobalGuiStyleChanged?.Invoke(sender, e);
                }
                #endregion

                #region AutoLargeHeightChanged
                public static event StiAutoLargeHeightEventHandler AutoLargeHeightChanged;

                public static void InvokeAutoLargeHeightChanged(object sender, StiAutoLargeHeightEventArgs e)
                {
                    AutoLargeHeightChanged?.Invoke(sender, e);
                }
                #endregion

                #region GetProperties
                public static event StiGetPropertiesEventHandler GetProperties;

                public static void InvokeGetProperties(object sender, StiGetPropertiesEventArgs e)
                {
                    GetProperties?.Invoke(sender, e);
                }
                #endregion
            }
        }
    }
}