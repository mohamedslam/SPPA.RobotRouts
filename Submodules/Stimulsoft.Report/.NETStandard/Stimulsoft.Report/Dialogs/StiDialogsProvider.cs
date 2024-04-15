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
using System.Linq;
using Stimulsoft.Base;
using Stimulsoft.Base.Services;

namespace Stimulsoft.Report.Dialogs
{
	
	/// <summary>
	/// This class provide base service for dialogs rendering.
	/// </summary>
	public abstract class StiDialogsProvider : StiService
	{
        #region StiService override
        /// <summary>
        /// Gets a service type.
        /// </summary>
        public override Type ServiceType => typeof(StiDialogsProvider);

        protected abstract StiGuiMode GuiMode { get; }
		#endregion

		#region Properties
		public abstract StiReport Report { get;	set; }
		#endregion

        #region Methods.Static
        public static StiDialogsProvider GetProvider(StiReport report)
        {
            StiGuiMode guiMode;
            if (report != null)
            {
                guiMode = (report.IsWpf ? StiGuiMode.Wpf : StiGuiMode.Gdi);                
            }
            else
            {
                guiMode = (StiOptions.Configuration.IsWPF ? StiGuiMode.Wpf : StiGuiMode.Gdi);                
            }

            if (guiMode == StiGuiMode.Wpf)
            {
                var providerType = Type.GetType("Stimulsoft.Report.Dialogs.StiWpfDialogsProvider, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo);
                if (providerType != null)
                    return StiActivator.CreateObject(providerType) as StiDialogsProvider;
            }
            else
            {
                foreach (var provider in StiOptions.Services.DialogProviders.Where(p => p.GuiMode == guiMode))
                {
                    return StiActivator.CreateObject(provider.GetType(), new object[] { report }) as StiDialogsProvider;
                }
            }
            return null;
        }
        #endregion

        #region Methods
        /// <summary>
		/// Render all forms in report.
		/// </summary>
		public abstract bool Render(StiReport report, StiFormStartMode startMode);

		public abstract void PrepareForm();

		public abstract void DisposeForm();

		public abstract void LoadForm(IStiForm formControl);

		public abstract void CloseForm();

		public abstract bool RenderForm(IStiForm formControl);

		public abstract IStiForm CreateForm(StiReport report);

		public abstract IStiTextBoxControl CreateTextBoxControl();

		public abstract IStiLabelControl CreateLabelControl();

		public abstract IStiCheckBoxControl CreateCheckBoxControl();

		public abstract IStiPictureBoxControl CreatePictureBoxControl();
		#endregion

		#region Events
		#region ButtonClick
		public event EventHandler ButtonClick;
		
		protected void InvokeButtonClick(object sender, EventArgs e)
		{
            ButtonClick?.Invoke(sender, e);
        }
		#endregion

		#region EventFired
		public event EventHandler EventFired;
		
		protected void InvokeEventFired(object sender, EventArgs e)
		{
            EventFired?.Invoke(sender, e);
        }
		#endregion
		#endregion
	}
}
