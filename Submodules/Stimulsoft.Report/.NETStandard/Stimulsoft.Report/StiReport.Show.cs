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
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Report.Viewer;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report
{
    public partial class StiReport
    {
        #region Properties
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public IStiViewerControl ViewerControl { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        public Type ViewerForm { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        [Obsolete("Please use property 'ViewerControl' instead 'PreviewControl' property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IStiViewerControl PreviewControl
        {
            get
            {
                return this.ViewerControl;
            }
            set
            {
                this.ViewerControl = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [StiBrowsable(false)]
        [Obsolete("Please use property 'ViewerForm' instead 'PreviewForm' property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Type PreviewForm
        {
            get
            {
                return ViewerForm;
            }
            set
            {
                ViewerForm = value;
            }
        }
        #endregion

        #region Mehtods.WinForms
        /// <summary>
        /// Shows a rendered report. If the report is not rendered then its rendering starts.
        /// </summary>
        public StiReport Show()
        {
            return Show(null, false);
        }

        /// <summary>
        /// Shows the rendered report as a dialog form or not. If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="dialogForm">If this parameter is true then the report will be rendered as a dialog form.</param>
        public StiReport Show(bool dialogForm)
        {
            return Show(null, dialogForm);
        }

        /// <summary>
        /// Shows the rendered report as MDI child window. If the report is not rendered then its rendering starts. 
        /// </summary>
        /// <param name="parentForm">A parent form in the MDI application.</param>
        public StiReport Show(Form parentForm)
        {
            return Show(parentForm, false);
        }

        /// <summary>
        /// Shows the rendered report as MDI child window. If the report is not rendered then its rendering starts. 
        /// </summary>
        /// <param name="owner">Provides an interface to expose parent Win32 HWND handle.</param>
        public StiReport Show(IWin32Window owner)
        {
            return Show(null, owner, true);
        }

        /// <summary>
        /// Shows the rendered report as MDI child window. If the report is not rendered then its rendering starts. 
        /// </summary>
        /// <param name="owner">Provides an interface to expose parent Win32 HWND handle.</param>
        /// <param name="dialogForm">If this parameter is true then the report will be shown as a dialog form.</param>
        public StiReport Show(IWin32Window owner, bool dialogForm)
        {
            return Show(null, owner, dialogForm);
        }

        /// <summary>
        /// Shows the rendered report as MDI child window as a dialog form or not.
        /// If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="parentForm">A parent form in the MDI application.</param>
        /// <param name="dialogForm">If this parameter is true then the report will be shown as a dialog form.</param>
        private StiReport Show(Form parentForm, bool dialogForm)
        {
            return Show(parentForm, null, dialogForm);
        }

        /// <summary>
        /// Shows the rendered report as MDI child window as a dialog form or not.
        /// If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="parentForm">A parent form in the MDI application.</param>
        /// <param name="dialogForm">If this parameter is true then the report will be shown as a dialog form.</param>
        /// <param name="win32Window">Provides an interface to expose parent Win32 HWND handle.</param>
        private StiReport Show(Form parentForm, IWin32Window win32Window, bool dialogForm)
        {
            if (!StiGuiOptions.AllowWinViewer && StiGuiOptions.AllowWpfViewer)
            {
                ShowWithWpf(parentForm, dialogForm);
                return this;
            }

            if (this.PreviewMode == StiPreviewMode.DotMatrix)
            {
                this.ShowDotMatrix(parentForm, win32Window, dialogForm);
            }
            else
            {
                RegReportDataSources();

                if (ContainsDashboard)
                {
                    if (CheckNeedsCompiling())
                        Compile();
                }
                else if (!IsRendered)
                    this.Render(true);

                StiLogService.Write(this.GetType(), "Showing report");

                if (this.CheckNeedsCompiling() && !this.IsCompiled)
                    throw new Exception("Can't showing report because report require compilation.");

                try
                {
                    if (!this.IsStopped)
                    {
                        var report = CheckNeedsCompiling() ? this.CompiledReport : this;

                        if (ViewerControl != null)
                            ViewerControl.Report = report;

                        else
                        {
                            IStiViewerForm viewerForm;
                            if (ViewerForm == null)
                            {
                                viewerForm = StiGuiOptions.GetViewerForm(report, StiGuiMode.Gdi);
                                viewerForm.Report = report;
                            }
                            else
                            {
                                viewerForm = StiActivator.CreateObject(ViewerForm, new object[] { null }) as IStiViewerForm;
                                viewerForm.Report = report;
                            }

                            if (parentForm != null && (parentForm.IsMdiContainer || parentForm.MdiParent != null))
                            {
                                viewerForm.TopLevel = false;
                                viewerForm.ShowInTaskbar = false;
                                viewerForm.ViewerOwner = viewerForm.GetOwner(parentForm) ?? parentForm;

                                viewerForm.ShowViewer();
                            }
                            else
                            {
                                if (dialogForm)
                                {
                                    viewerForm.ShowInTaskbar = StiOptions.Viewer.Windows.ShowInTaskbar;
                                    if (win32Window == null)
                                        viewerForm.ShowDialogViewer();
                                    else
                                        viewerForm.ShowDialogViewer(win32Window);

                                    if (viewerForm is IDisposable)
                                        ((IDisposable)viewerForm).Dispose();
                                }
                                else
                                {
                                    viewerForm.ShowViewer(win32Window ?? parentForm);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    StiLogService.Write(this.GetType(), "Showing report...ERROR");
                    StiLogService.Write(this.GetType(), e);

                    if (!StiOptions.Engine.HideExceptions) throw;
                }
            }
            return this;
        }
        #endregion

        #region Methods.WinForms.RibbonGUI
        /// <summary>
        /// Shows a rendered report in viewer with Ribbon GUI. If the report is not rendered then its rendering starts.
        /// </summary>
        public StiReport ShowWithRibbonGUI()
        {
            return ShowWithRibbonGUI(null, false);
        }

        /// <summary>
        /// Shows the rendered report as a dialog form or not with Ribbon GUI. If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="dialogForm">If this parameter is true then the report will be rendered as a dialog form.</param>
        public StiReport ShowWithRibbonGUI(bool dialogForm)
        {
            return ShowWithRibbonGUI(null, dialogForm);
        }

        /// <summary>
        /// Shows the rendered report as MDI child window with Ribbon GUI. If the report is not rendered then its rendering starts. 
        /// </summary>
        /// <param name="parentForm">A parent form in the MDI application.</param>
        public StiReport ShowWithRibbonGUI(Form parentForm)
        {
            return ShowWithRibbonGUI(parentForm, false);
        }

        /// <summary>
        /// Shows the rendered report as MDI child window with Ribbon GUI. If the report is not rendered then its rendering starts. 
        /// </summary>
        /// <param name="owner">Provides an interface to expose parent Win32 HWND handle.</param>
        public StiReport ShowWithRibbonGUI(IWin32Window owner)
        {
            return ShowWithRibbonGUI(null, owner, true);
        }

        /// <summary>
        /// Shows the rendered report as MDI child window with Ribbon GUI. If the report is not rendered then its rendering starts. 
        /// </summary>
        /// <param name="owner">Provides an interface to expose parent Win32 HWND handle.</param>
        /// <param name="dialogForm">If this parameter is true then the report will be shown as a dialog form.</param>
        public StiReport ShowWithRibbonGUI(IWin32Window owner, bool dialogForm)
        {
            return ShowWithRibbonGUI(null, owner, dialogForm);
        }

        /// <summary>
        /// Shows the rendered report as MDI child window as a dialog form or not with Ribbon GUI.
        /// If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="parentForm">A parent form in the MDI application.</param>
        /// <param name="dialogForm">If this parameter is true then the report will be shown as a dialog form.</param>
        private StiReport ShowWithRibbonGUI(Form parentForm, bool dialogForm)
        {
            return ShowWithRibbonGUI(parentForm, null, dialogForm);
        }

        /// <summary>
        /// Shows the rendered report as MDI child window as a dialog form or not with Ribbon GUI.
        /// If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="parentForm">A parent form in the MDI application.</param>
        /// <param name="dialogForm">If this parameter is true then the report will be shown as a dialog form.</param>
        /// <param name="win32Window">Provides an interface to expose parent Win32 HWND handle.</param>
        private StiReport ShowWithRibbonGUI(Form parentForm, IWin32Window win32Window, bool dialogForm)
        {
            if (ContainsDashboard)
                throw new NotSupportedException("Viewer with Ribbon GUI doesn't supported for the dashboards!");

            if (!StiGuiOptions.AllowWinViewer && StiGuiOptions.AllowWpfViewer)
            {
                ShowWithWpfRibbonGUI(parentForm, dialogForm);
                return this;
            }

            if (this.PreviewMode == StiPreviewMode.DotMatrix)
            {
                this.ShowDotMatrix(parentForm, win32Window, dialogForm);
            }
            else
            {
                RegReportDataSources();
                if (ContainsDashboard)
                {
                    if (CheckNeedsCompiling())
                        Compile();
                }
                else if (!IsRendered)
                {
                    this.Render(true);
                }

                StiLogService.Write(this.GetType(), "Showing report");

                if (CheckNeedsCompiling() && !IsCompiled)
                    throw new Exception("Can't showing report because report require compilation.");

                try
                {
                    if (!this.IsStopped)
                    {
                        var report = CheckNeedsCompiling() ? this.CompiledReport : this;

                        if (ViewerControl != null)
                            ViewerControl.Report = report;

                        else
                        {
                            IStiViewerForm viewerForm;
                            if (ViewerForm == null)
                            {
                                viewerForm = StiGuiOptions.GetViewerFormWithRibbonGUI(report, StiGuiMode.Gdi);
                                viewerForm.Report = report;
                            }
                            else
                            {
                                var typeRibbonViewerForm = Type.GetType("Stimulsoft.Report.Viewer.StiRibbonViewerForm, Stimulsoft.Report.Win, " + StiVersion.VersionInfo);
                                if (typeRibbonViewerForm == null)
                                    throw new Exception("Assembly 'Stimulsoft.Report.Win' is not found");

                                viewerForm = StiActivator.CreateObject(typeRibbonViewerForm, new object[] { null }) as IStiViewerForm;
                                viewerForm.Report = report;
                            }

                            if (parentForm != null && (parentForm.IsMdiContainer || parentForm.MdiParent != null))
                            {
                                viewerForm.TopLevel = false;
                                viewerForm.ShowInTaskbar = false;
                                viewerForm.ViewerOwner = viewerForm.GetOwner(parentForm) ?? parentForm;

                                viewerForm.ShowViewer();
                            }
                            else
                            {
                                if (dialogForm)
                                {
                                    viewerForm.ShowInTaskbar = StiOptions.Viewer.Windows.ShowInTaskbar;
                                    if (win32Window == null) viewerForm.ShowDialogViewer();
                                    else viewerForm.ShowDialogViewer(win32Window);

                                    if (viewerForm is IDisposable)
                                        ((IDisposable)viewerForm).Dispose();
                                }
                                else
                                {
                                    viewerForm.ShowViewer(win32Window ?? parentForm);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    StiLogService.Write(this.GetType(), "Showing report...ERROR");
                    StiLogService.Write(this.GetType(), e);

                    if (!StiOptions.Engine.HideExceptions) throw;
                }
            }

            return this;
        }
        #endregion

        #region Methods.WPF
        /// <summary>
        /// Shows the rendered report with using WPF technology. 
        /// If the report is not rendered then its rendering starts.
        /// </summary>
        public StiReport ShowWithWpf()
        {
            return ShowWithWpf(false);
        }

        /// <summary>
        /// Shows the rendered report with using WPF technology.
        /// If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="dialogWindow">If this parameter is true then the report will be shown as a dialog window.</param>
        public StiReport ShowWithWpf(bool dialogWindow)
        {
            return ShowWithWpf(null, dialogWindow);
        }

        /// <summary>
        /// Shows the rendered report with using WPF technology.
        /// If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="ownerWindow">A parent form.</param>
        public StiReport ShowWithWpf(object ownerWindow)
        {
            return ShowWithWpf(ownerWindow, false);
        }

        /// <summary>
        /// Shows the rendered report with using WPF technology.
        /// If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="ownerWindow">A parent form.</param>
        /// <param name="dialogWindow">If this parameter is true then the report will be shown as a dialog window.</param>
        public StiReport ShowWithWpf(object ownerWindow, bool dialogWindow)
        {
            StiOptions.Configuration.IsWPF = true;

            if (this.PreviewMode == StiPreviewMode.DotMatrix)
            {
                this.ShowDotMatrixWithWpf(dialogWindow);
            }
            else
            {
                RegReportDataSources();

                if (ContainsDashboard)
                {
                    if (CheckNeedsCompiling())
                        Compile();
                }
                else if (!IsRendered)
                {
                    this.RenderWithWpf(true);
                }

                StiLogService.Write(this.GetType(), "Showing report");

                if (CheckNeedsCompiling() && !IsCompiled)
                    throw new Exception("Can't showing report because report require compilation.");

                try
                {
                    if (!this.IsStopped)
                    {
                        var report = CheckNeedsCompiling() ? this.CompiledReport : this;

                        if (ViewerControl != null)
                        {
                            ViewerControl.Report = report;
                        }
                        else
                        {
                            IStiViewerForm viewerForm;
                            if (ViewerForm == null)
                            {
                                viewerForm = StiGuiOptions.GetViewerForm(report, StiGuiMode.Wpf);
                                viewerForm.ViewerOwner = ownerWindow;
                                viewerForm.Report = report;
                            }
                            else
                            {
                                viewerForm = StiActivator.CreateObject(ViewerForm, new object[] { null }) as IStiViewerForm;
                                viewerForm.ViewerOwner = ownerWindow;
                                viewerForm.Report = report;
                            }

                            if (dialogWindow)
                            {
                                viewerForm.ShowInTaskbar = StiOptions.Viewer.Windows.ShowInTaskbar;
                                viewerForm.ShowDialogViewer(ownerWindow);
                            }
                            else
                                viewerForm.ShowViewer(ownerWindow);
                        }
                    }
                }
                catch (Exception e)
                {
                    StiLogService.Write(this.GetType(), "Showing report...ERROR");
                    StiLogService.Write(this.GetType(), e);

                    if (!StiOptions.Engine.HideExceptions) throw;
                }
            }

            return this;
        }
        #endregion

        #region Methods.WPF.RibbonGUI
        /// <summary>
        /// Shows the rendered report with using WPF Ribbon GUI technology. 
        /// If the report is not rendered then its rendering starts.
        /// </summary>
        public StiReport ShowWithWpfRibbonGUI()
        {
            return ShowWithWpfRibbonGUI(false);
        }

        /// <summary>
        /// Shows the rendered report with using WPF Ribbon GUI technology.
        /// If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="dialogWindow">If this parameter is true then the report will be shown as a dialog window.</param>
        public StiReport ShowWithWpfRibbonGUI(bool dialogWindow)
        {
            return ShowWithWpfRibbonGUI(null, dialogWindow);
        }

        /// <summary>
        /// Shows the rendered report with using WPF Ribbon GUI technology.
        /// If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="ownerWindow">A parent form.</param>
        public StiReport ShowWithWpfRibbonGUI(object ownerWindow)
        {
            return ShowWithWpfRibbonGUI(ownerWindow, false);
        }

        /// <summary>
        /// Shows the rendered report with using WPF Ribbon GUI technology.
        /// If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="ownerWindow">A parent form.</param>
        /// <param name="dialogWindow">If this parameter is true then the report will be shown as a dialog window.</param>
        public StiReport ShowWithWpfRibbonGUI(object ownerWindow, bool dialogWindow)
        {
            if (this.PreviewMode == StiPreviewMode.DotMatrix)
            {
                this.ShowDotMatrixWithWpf(dialogWindow);
            }
            else
            {
                RegReportDataSources();

                if (ContainsDashboard)
                {
                    if (CheckNeedsCompiling())
                        Compile();
                }
                else if (!IsRendered)
                {
                    this.RenderWithWpf(true);
                }

                StiLogService.Write(this.GetType(), "Showing report");

                if (CheckNeedsCompiling() && !IsCompiled)
                    throw new Exception("Can't showing report because report require compilation.");

                try
                {
                    if (!this.IsStopped)
                    {
                        var report = CheckNeedsCompiling() ? this.CompiledReport : this;

                        if (ViewerControl != null)
                            ViewerControl.Report = report;

                        else
                        {
                            IStiViewerForm viewerForm;
                            if (ViewerForm == null)
                            {
                                viewerForm = StiGuiOptions.GetViewerFormWithRibbonGUI(null, StiGuiMode.Wpf);
                                viewerForm.ViewerOwner = ownerWindow;
                                viewerForm.Report = report;
                            }
                            else
                            {
                                viewerForm = StiActivator.CreateObject(ViewerForm, new object[] { null }) as IStiViewerForm;
                                viewerForm.ViewerOwner = ownerWindow;
                                viewerForm.Report = report;
                            }

                            if (dialogWindow)
                            {
                                viewerForm.ShowInTaskbar = StiOptions.Viewer.Windows.ShowInTaskbar;
                                viewerForm.ShowDialogViewer(ownerWindow);
                            }
                            else
                            {
                                viewerForm.ShowViewer(ownerWindow);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    StiLogService.Write(this.GetType(), "Showing report...ERROR");
                    StiLogService.Write(this.GetType(), e);

                    if (!StiOptions.Engine.HideExceptions) throw;
                }
            }

            return this;
        }
        #endregion

        #region Methods.DotMatrix
        /// <summary>
        /// Shows a rendered report in DotMatrix mode. If the report is not rendered then its rendering starts.
        /// </summary>
        public StiReport ShowDotMatrix()
        {
            return ShowDotMatrix(null, false);
        }

        /// <summary>
        /// Shows the rendered report in DotMatrix mode as a dialog form or not. If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="dialogForm">If this parameter is true then the report will be rendered as a dialog form.</param>
        public StiReport ShowDotMatrix(bool dialogForm)
        {
            return ShowDotMatrix(null, dialogForm);
        }

        /// <summary>
        /// Shows the rendered report in DotMatrix mode as MDI child window. If the report is not rendered then its rendering starts. 
        /// </summary>
        /// <param name="parentForm">A parent form in the MDI application.</param>
        public StiReport ShowDotMatrix(Form parentForm)
        {
            return ShowDotMatrix(parentForm, false);
        }

        /// <summary>
        /// Shows the rendered report in DotMatrix mode as MDI child window. If the report is not rendered then its rendering starts. 
        /// </summary>
        /// <param name="owner">Provides an interface to expose parent Win32 HWND handle.</param>
        public StiReport ShowDotMatrix(IWin32Window owner)
        {
            return ShowDotMatrix(null, owner, true);
        }

        /// <summary>
        /// Shows the rendered report in DotMatrix mode as MDI child window. If the report is not rendered then its rendering starts. 
        /// </summary>
        /// <param name="owner">Provides an interface to expose parent Win32 HWND handle.</param>
        /// <param name="dialogForm">If this parameter is true then the report will be shown as a dialog form.</param>
        public StiReport ShowDotMatrix(IWin32Window owner, bool dialogForm)
        {
            return ShowDotMatrix(null, owner, dialogForm);
        }

        /// <summary>
        /// Shows the rendered report in DotMatrix mode as MDI child window as a dialog form or not.
        /// If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="parentForm">A parent form in the MDI application.</param>
        /// <param name="dialogForm">If this parameter is true then the report will be shown as a dialog form.</param>
        private StiReport ShowDotMatrix(Form parentForm, bool dialogForm)
        {
            return ShowDotMatrix(parentForm, null, dialogForm);
        }

        /// <summary>
        /// Shows the rendered report in DotMatrix mode as MDI child window as a dialog form or not.
        /// If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="parentForm">A parent form in the MDI application.</param>
        /// <param name="dialogForm">If this parameter is true then the report will be shown as a dialog form.</param>
        /// <param name="win32Window">Provides an interface to expose parent Win32 HWND handle.</param>
        private StiReport ShowDotMatrix(Form parentForm, IWin32Window win32Window, bool dialogForm)
        {
            if (ContainsDashboard)
                throw new NotSupportedException("DotMatrix Viewer doesn't supported for the dashboards!");

            if (!StiGuiOptions.AllowWinViewer && StiGuiOptions.AllowWpfViewer)
            {
                ShowDotMatrixWithWpf(dialogForm);
                return this;
            }

            RegReportDataSources();

            if (ContainsDashboard)
            {
                if (CheckNeedsCompiling())
                    Compile();
            }
            else if (!IsRendered)
            {
                this.Render(true);
            }

            StiLogService.Write(this.GetType(), "Showing report in DotMatrixMode");

            if (CheckNeedsCompiling() && !IsCompiled)
                throw new Exception("Can't showing report in DotMatrix mode because report require compilation.");

            try
            {
                if (!this.IsStopped)
                {
                    var report = CheckNeedsCompiling() ? this.CompiledReport : this;
                    if (ViewerControl != null)
                        ViewerControl.Report = report;

                    else
                    {
                        IStiViewerForm viewerForm;
                        if (ViewerForm == null)
                            viewerForm = StiGuiOptions.GetDotMatrixViewerForm(report, StiGuiMode.Gdi);
                        
                        else
                            viewerForm = StiActivator.CreateObject(ViewerForm, new object[] { report }) as IStiViewerForm;

                        if (parentForm != null && (parentForm.IsMdiContainer || parentForm.MdiParent != null))
                        {
                            viewerForm.TopLevel = false;
                            viewerForm.ShowInTaskbar = false;

                            viewerForm.ViewerOwner = viewerForm.GetOwner(parentForm) ?? parentForm;

                            viewerForm.ShowViewer();
                        }
                        else
                        {
                            if (dialogForm)
                            {
                                viewerForm.ShowInTaskbar = StiOptions.Viewer.Windows.ShowInTaskbar;
                                if (win32Window == null)
                                    viewerForm.ShowDialogViewer();
                                else
                                    viewerForm.ShowDialogViewer(win32Window);
                            }
                            else
                            {
                                if (win32Window == null)
                                    viewerForm.ShowViewer();
                                else
                                    viewerForm.ShowViewer(win32Window);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Showing report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }

            return this;
        }
        #endregion

        #region Methods.DotMatrix.RibbonGUI
        /// <summary>
        /// Shows a rendered report in DotMatrix mode with Ribbon GUI. If the report is not rendered then its rendering starts.
        /// The method ShowDotMatrixWithRibbonGUI is obsoleted. Please use 'ShowDotMatrix'.
        /// </summary>
        [Obsolete("The method ShowDotMatrixWithRibbonGUI is obsoleted. Please use 'ShowDotMatrix'.")]
        public StiReport ShowDotMatrixWithRibbonGUI()
        {
            return ShowDotMatrixWithRibbonGUI(null, false);
        }

        /// <summary>
        /// Shows the rendered report in DotMatrix mode as a dialog form or not with Ribbon GUI. If the report is not rendered then its rendering starts.
        /// The method ShowDotMatrixWithRibbonGUI is obsoleted. Please use 'ShowDotMatrix'.
        /// </summary>
        /// <param name="dialogForm">If this parameter is true then the report will be rendered as a dialog form.</param>
        [Obsolete("The method ShowDotMatrixWithRibbonGUI is obsoleted. Please use 'ShowDotMatrix'.")]
        public StiReport ShowDotMatrixWithRibbonGUI(bool dialogForm)
        {
            return ShowDotMatrixWithRibbonGUI(null, dialogForm);
        }

        /// <summary>
        /// Shows the rendered report in DotMatrix mode as MDI child window with Ribbon GUI. If the report is not rendered then its rendering starts. 
        /// The method ShowDotMatrixWithRibbonGUI is obsoleted. Please use 'ShowDotMatrix'.
        /// </summary>
        /// <param name="parentForm">A parent form in the MDI application.</param>
        [Obsolete("The method ShowDotMatrixWithRibbonGUI is obsoleted. Please use 'ShowDotMatrix'.")]
        public StiReport ShowDotMatrixWithRibbonGUI(Form parentForm)
        {
            return ShowDotMatrixWithRibbonGUI(parentForm, false);
        }

        /// <summary>
        /// Shows the rendered report in DotMatrix mode as MDI child window with Ribbon GUI. If the report is not rendered then its rendering starts. 
        /// The method ShowDotMatrixWithRibbonGUI is obsoleted. Please use 'ShowDotMatrix'.
        /// </summary>
        /// <param name="owner">Provides an interface to expose parent Win32 HWND handle.</param>
        [Obsolete("The method ShowDotMatrixWithRibbonGUI is obsoleted. Please use 'ShowDotMatrix'.")]
        public StiReport ShowDotMatrixWithRibbonGUI(IWin32Window owner)
        {
            return ShowDotMatrixWithRibbonGUI(null, owner, true);
        }

        /// <summary>
        /// Shows the rendered report in DotMatrix mode as MDI child window with Ribbon GUI. If the report is not rendered then its rendering starts. 
        /// The method ShowDotMatrixWithRibbonGUI is obsoleted. Please use 'ShowDotMatrix'.
        /// </summary>
        /// <param name="owner">Provides an interface to expose parent Win32 HWND handle.</param>
        /// <param name="dialogForm">If this parameter is true then the report will be shown as a dialog form.</param>
        [Obsolete("The method ShowDotMatrixWithRibbonGUI is obsoleted. Please use 'ShowDotMatrix'.")]
        public StiReport ShowDotMatrixWithRibbonGUI(IWin32Window owner, bool dialogForm)
        {
            return ShowDotMatrixWithRibbonGUI(null, owner, dialogForm);
        }

        /// <summary>
        /// Shows the rendered report in DotMatrix mode as MDI child window as a dialog form or not with Ribbon GUI.
        /// If the report is not rendered then its rendering starts.
        /// The method ShowDotMatrixWithRibbonGUI is obsoleted. Please use 'ShowDotMatrix'.
        /// </summary>
        /// <param name="parentForm">A parent form in the MDI application.</param>
        /// <param name="dialogForm">If this parameter is true then the report will be shown as a dialog form.</param>
        [Obsolete("The method ShowDotMatrixWithRibbonGUI is obsoleted. Please use 'ShowDotMatrix'.")]
        private StiReport ShowDotMatrixWithRibbonGUI(Form parentForm, bool dialogForm)
        {
            return ShowDotMatrixWithRibbonGUI(parentForm, null, dialogForm);
        }

        /// <summary>
        /// Shows the rendered report in DotMatrix mode as MDI child window as a dialog form or not with Ribbon GUI.
        /// If the report is not rendered then its rendering starts.
        /// The method ShowDotMatrixWithRibbonGUI is obsoleted. Please use 'ShowDotMatrix'.
        /// </summary>
        /// <param name="parentForm">A parent form in the MDI application.</param>
        /// <param name="dialogForm">If this parameter is true then the report will be shown as a dialog form.</param>
        /// <param name="win32Window">Provides an interface to expose parent Win32 HWND handle.</param>
        [Obsolete("The method ShowDotMatrixWithRibbonGUI is obsoleted. Please use 'ShowDotMatrix'.")]
        private StiReport ShowDotMatrixWithRibbonGUI(Form parentForm, IWin32Window win32Window, bool dialogForm)
        {
            RegReportDataSources();

            if (ContainsDashboard)
            {
                if (CheckNeedsCompiling())
                    Compile();
            }
            else if (!IsRendered)
            {
                this.Render(true);
            }

            StiLogService.Write(this.GetType(), "Showing report in DotMatrixMode");

            if (CheckNeedsCompiling() && !IsCompiled)
                throw new Exception("Can't showing report in DotMatrix mode because report require compilation.");

            try
            {
                if (!this.IsStopped)
                {
                    StiReport report = CheckNeedsCompiling() ? this.CompiledReport : this;

                    if (ViewerControl != null)
                        ViewerControl.Report = report;

                    else
                    {
                        IStiViewerForm viewerForm;
                        if (ViewerForm == null)
                        {
                            viewerForm = StiGuiOptions.GetDotMatrixViewerFormWithRibbonGUI(report, StiGuiMode.Gdi);
                        }
                        else
                        {
                            var typeRibbonDotMatrix = Type.GetType("Stimulsoft.Report.Viewer.StiRibbonDotMatrixViewerForm, Stimulsoft.Report.Win, " + StiVersion.VersionInfo);
                            if (typeRibbonDotMatrix == null)
                                throw new Exception("Assembly 'Stimulsoft.Report.Win' is not found");

                            viewerForm = StiActivator.CreateObject(typeRibbonDotMatrix, new object[] { report }) as IStiViewerForm;
                        }

                        if (parentForm != null && (parentForm.IsMdiContainer || parentForm.MdiParent != null))
                        {
                            viewerForm.TopLevel = false;
                            viewerForm.ShowInTaskbar = false;

                            viewerForm.ViewerOwner = viewerForm.GetOwner(parentForm) ?? parentForm;

                            viewerForm.ShowViewer();
                        }
                        else
                        {
                            if (dialogForm)
                            {
                                viewerForm.ShowInTaskbar = StiOptions.Viewer.Windows.ShowInTaskbar;
                                if (win32Window == null)
                                    viewerForm.ShowDialogViewer();
                                else
                                    viewerForm.ShowDialogViewer(win32Window);
                            }
                            else
                            {
                                if (win32Window == null)
                                    viewerForm.ShowViewer();
                                else
                                    viewerForm.ShowViewer(win32Window);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Showing report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }

            return this;
        }
        #endregion

        #region Methods.DotMatrix.WPF
        /// <summary>
        /// Shows a rendered report in DotMatrix mode with using WPF technology. If the report is not rendered then its rendering starts.
        /// </summary>
        public StiReport ShowDotMatrixWithWpf()
        {
            return ShowDotMatrixWithWpf(false);
        }

        /// <summary>
        /// Shows the rendered report in DotMatrix mode as a dialog form or not with using WPF technology. 
        /// If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="dialogWindow">If this parameter is true then the report will be rendered as a dialog window.</param>
        public StiReport ShowDotMatrixWithWpf(bool dialogWindow)
        {
            RegReportDataSources();

            if (ContainsDashboard)
            {
                if (CheckNeedsCompiling())
                    Compile();
            }
            else if (!IsRendered)
            {
                this.Render(true);
            }

            StiLogService.Write(this.GetType(), "Showing report in DotMatrixMode");

            if (CheckNeedsCompiling() && !IsCompiled)
                throw new Exception("Can't showing report in DotMatrix mode because report require compilation.");

            try
            {
                if (!this.IsStopped)
                {
                    var report = CheckNeedsCompiling() ? this.CompiledReport : this;

                    if (ViewerControl != null)
                    {
                        ViewerControl.Report = report;
                    }
                    else
                    {
                        IStiViewerForm viewerForm;
                        if (ViewerForm == null)
                            viewerForm = StiGuiOptions.GetDotMatrixViewerForm(report, StiGuiMode.Wpf);

                        else
                            viewerForm = StiActivator.CreateObject(ViewerForm, new object[] { report }) as IStiViewerForm;

                        if (dialogWindow)
                        {
                            viewerForm.ShowInTaskbar = StiOptions.Viewer.Windows.ShowInTaskbar;
                            viewerForm.ShowDialogViewer();
                        }
                        else
                        {
                            viewerForm.ShowViewer();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Showing report...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }

            return this;
        }
        #endregion
    }
}