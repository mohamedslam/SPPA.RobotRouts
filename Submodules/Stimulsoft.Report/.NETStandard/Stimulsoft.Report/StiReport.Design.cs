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
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Report.Design;
using Stimulsoft.Report.Dictionary;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report
{
    public partial class StiReport
    {
        #region Methods.WinForms
        /// <summary>
        /// Calls the designer for the report in the Modal window.
        /// </summary>
        public DialogResult Design()
        {
            var dr = Design(true);
            ResetRenderedState();
            return dr;
        }

        /// <summary>
        /// Calls the designer for the report.
        /// </summary>
        /// <param name="dialogForm">Is this window a dialog box or not.</param>
        public DialogResult Design(bool dialogForm)
        {
            return Design(null, dialogForm, null);
        }

        /// <summary>
        /// Calls the designer for the report.
        /// </summary>
        /// <param name="dialogForm">Is this window a dialog box or not.</param>
        /// <param name="win32Window">Provides an interface to expose parent Win32 HWND handle.</param>
        public DialogResult Design(bool dialogForm, IWin32Window win32Window)
        {
            return Design(null, dialogForm, win32Window);
        }

        /// <summary>
        /// Calls the designer for the report in the Parent Form.
        /// </summary>
        /// <param name="parentForm">Parent Form.</param>
        public DialogResult Design(Form parentForm)
        {
            return Design(parentForm, false, null);
        }

        private DialogResult Design(Form parentForm, bool dialogForm, IWin32Window win32Window)
        {
            if (!StiGuiOptions.AllowWinDesigner)
            {
                if (StiGuiOptions.AllowWpfDesigner)
                    return DesignWithWpf(true) ? DialogResult.OK : DialogResult.Cancel;

                if (StiGuiOptions.AllowWpfV2Designer)
                    return DesignV2WithWpf(true) ? DialogResult.OK : DialogResult.Cancel;
            }

            this.UpdateInheritedReport();
            IsRendered = false;

            var result = StartDesigner(parentForm, dialogForm, win32Window);
            ResetRenderedState();
            return result;
        }

        private DialogResult StartDesigner(Form parentForm, bool dialogForm, IWin32Window win32Window)
        {
            try
            {
                StiLogService.Write(this.GetType(), "Starting report designer");
                RegReportDataSources();
                var designer = PrepareForDesign();
                this.Pages.SortByPriority();

                if (parentForm != null)
                {
                    designer.TopLevel = false;
                    designer.MdiParent = parentForm;
                    designer.ShowInTaskbar = StiOptions.Designer.ShowDesignerInTaskbar;

                    designer.ShowDesigner();
                }
                else
                {
                    if (dialogForm)
                    {
                        designer.ShowInTaskbar = StiOptions.Designer.ShowDesignerInTaskbar;
                        designer.DesignerControl.IsModalMode = true;

                        if (win32Window == null)
                            return designer.ShowDialogDesigner();
                        else
                            return designer.ShowDialogDesigner(win32Window);
                    }
                    else
                    {
                        designer.ShowInTaskbar = StiOptions.Designer.ShowDesignerInTaskbar;

                        if (win32Window == null)
                            designer.ShowDesigner();
                        else
                            designer.ShowDesigner(win32Window);

                        return DialogResult.OK;
                    }
                }
            }

            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Starting report designer...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }

            return DialogResult.None;
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        /// <returns>Returns the Designer of the report.</returns>
        internal IStiDesigner PrepareForDesign()
        {
            var typeStr = $"Stimulsoft.Report.Design.StiRibbonDesigner, Stimulsoft.Report.Design, {StiVersion.VersionInfo}";

            var designerType = Type.GetType(typeStr);
            if (designerType == null)
                throw new Exception("Assembly 'Stimulsoft.Report.Design' is not found!");

            this.Designer = StiActivator.CreateObject(designerType, new object[] { this }) as IStiDesigner;

            StiDictionary.DoAutoSynchronize(this);

            return this.Designer as IStiDesigner;
        }
        #endregion

        #region Methods.WPF
        /// <summary>
        /// Calls the designer for the report in the Modal window with using WPF technology. 
        /// </summary>
        public bool DesignWithWpf()
        {
            return DesignWithWpf(true);
        }

        public bool DesignWithWpf(bool dialogWindow)
        {
            return DesignWithWpf(null, dialogWindow);
        }

        public bool DesignWithWpf(object ownerWindow, bool dialogWindow)
        {
            if (!StiGuiOptions.AllowWpfDesigner && StiGuiOptions.AllowWpfV2Designer)
                return DesignV2WithWpf(dialogWindow);

            this.UpdateInheritedReport();
            IsRendered = false;

            var dr = StartDesignerWithWpf(ownerWindow, dialogWindow);
            ResetRenderedState();
            return dr;
        }

        private bool StartDesignerWithWpf(object ownerWindow, bool dialogWindow)
        {
            try
            {
                StiLogService.Write(this.GetType(), "Starting report designer");
                StiOptions.Configuration.IsWPF = true;
                RegReportDataSources();
                var designer = PrepareForDesignWithWpf();
                this.Pages.SortByPriority();

                if (dialogWindow)
                {
                    if (designer.ShowInTaskbar != StiOptions.Designer.ShowDesignerInTaskbar)
                        designer.ShowInTaskbar = StiOptions.Designer.ShowDesignerInTaskbar;

                    designer.DesignerControl.IsModalMode = true;
                    return designer.ShowDialogDesigner(ownerWindow) == DialogResult.OK;
                }
                else
                {
                    if (designer.ShowInTaskbar != StiOptions.Designer.ShowDesignerInTaskbar)
                        designer.ShowInTaskbar = StiOptions.Designer.ShowDesignerInTaskbar;

                    designer.ShowDesigner(ownerWindow);
                    return true;
                }
            }

            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Starting report designer...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }

            return false;
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        /// <returns>Returns the Designer of the report.</returns>
        internal IStiDesigner PrepareForDesignWithWpf()
        {
            var designerType = Type.GetType("Stimulsoft.Report.WpfDesign.StiWpfDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo);

            if (designerType == null)
                throw new Exception("Assembly 'Stimulsoft.Report.WpfDesign' is not found!");

            this.Designer = StiActivator.CreateObject(designerType, new object[] { this }) as IStiDesigner;
            StiDictionary.DoAutoSynchronize(this);

            return Designer as IStiDesigner;
        }
        #endregion

        #region Methods.WPF.V2
        /// <summary>
        /// Calls the designer for the report in the Modal window with using WPF technology. 
        /// </summary>
        public bool DesignV2WithWpf()
        {
            return DesignV2WithWpf(null, true);
        }

        /// <summary>
        /// Calls the designer for the report in the Modal window with using WPF technology. 
        /// </summary>
        public bool DesignV2WithWpf(object ownerWindow)
        {
            return DesignV2WithWpf(ownerWindow, true);
        }

        /// <summary>
        /// Calls the designer for the report in the Modal window with using WPF technology. 
        /// </summary>
        public bool DesignV2WithWpf(bool dialogWindow)
        {
            return DesignV2WithWpf(null, dialogWindow);
        }

        /// <summary>
        /// Calls the designer for the report in the Modal window with using WPF technology. 
        /// </summary>
        public bool DesignV2WithWpf(object ownerWindow, bool dialogWindow)
        {
            this.UpdateInheritedReport();
            IsRendered = false;

            var dr = StartDesignerV2WithWpf(ownerWindow, dialogWindow);
            ResetRenderedState();
            return dr;
        }

        private bool StartDesignerV2WithWpf(object ownerWindow, bool dialogForm)
        {
            try
            {
                StiLogService.Write(this.GetType(), "Starting report designerV2");
                StiOptions.Configuration.IsWPF = true;
                RegReportDataSources();

                var designerLauncher = PrepareForDesignV2WithWpf();
                this.Pages.SortByPriority();

                designerLauncher.Launch(ownerWindow, this, dialogForm);
                return true;
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Starting report designerV2...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }

            return false;
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        /// <returns>Returns the Designer of the report.</returns>
        internal IStiDeignerLauncher PrepareForDesignV2WithWpf()
        {
            var designerLauncherType = Type.GetType("Stimulsoft.Client.Designer.StiDesigner, Stimulsoft.Client.Designer, " + StiVersion.VersionInfo);
            if (designerLauncherType == null)
                throw new Exception("Assembly 'Stimulsoft.Client.Designer' is not found!");

            var designerLauncher = StiActivator.CreateObject(designerLauncherType, new object[0]) as IStiDeignerLauncher;

            StiDictionary.DoAutoSynchronize(this);

            return designerLauncher;
        }
        #endregion
    }
}