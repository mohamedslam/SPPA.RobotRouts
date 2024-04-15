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

using Stimulsoft.Base;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.Viewer;
using System;

namespace Stimulsoft.Report
{
    public class StiGuiOptions
    {
        public static Type ProgressGdiType { get; set; }

        public static Type ProgressWpfType { get; set; }

        public static bool AllowWinViewer
        {
            get
            {
                try
                {
                    var type = Type.GetType($"Stimulsoft.Report.Win.StiProgressInformation, Stimulsoft.Report.Win, {StiVersion.VersionInfo}");
                    return (type != null);
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool AllowWinDesigner
        {
            get
            {
                try
                {
                    var type = Type.GetType($"Stimulsoft.Report.Design.StiDesigner, Stimulsoft.Report.Design, {StiVersion.VersionInfo}");
                    return type != null;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool AllowWpfViewer
        {
            get
            {
                try
                {
                    var type = Type.GetType($"Stimulsoft.Report.Wpf.StiWpfFormRunner, Stimulsoft.Report.Wpf, {StiVersion.VersionInfo}");
                    return type != null;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool AllowWpfDesigner
        {
            get
            {
                try
                {
                    var type = Type.GetType($"Stimulsoft.Report.WpfDesign.StiWpfDesigner, Stimulsoft.Report.WpfDesign, {StiVersion.VersionInfo}");
                    return type != null;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool AllowWpfV2Designer
        {
            get
            {
                try
                {
                    var type = Type.GetType($"Stimulsoft.Client.Designer.Native.StiCloudDesignerLauncherWindow, Stimulsoft.Client.Designer, {StiVersion.VersionInfo}");
                    return type != null;
                }
                catch
                {
                    return false;
                }
            }
        }

        private static IStiFormRunner GetFormRunner(StiGuiMode guiMode)
        {
            switch (guiMode)
            {
                case StiGuiMode.Wpf:
                    {
                        var type = Type.GetType($"Stimulsoft.Report.Wpf.StiWpfFormRunner, Stimulsoft.Report.Wpf, {StiVersion.VersionInfo}");
                        if (type == null)
                            throw new Exception("Assembly 'Stimulsoft.Report.Wpf' is not found");

                        return StiActivator.CreateObject(type) as IStiFormRunner;
                    }
            }

            return new StiWinFormRunner();
        }

        public static IStiFormRunner GetExportFormRunner(string formName, StiGuiMode guiMode, object ownerWindow)
        {
            var formRunner = GetFormRunner(guiMode);
            formRunner.OwnerWindow = ownerWindow;

            switch (guiMode)
            {
                case StiGuiMode.Wpf:
                    formRunner.Create($"Stimulsoft.Report.Export.Wpf.{formName}", "Stimulsoft.Report.Wpf");
                    break;

                default:
                    formRunner.Create($"Stimulsoft.Report.Export.Win.{formName}", "Stimulsoft.Report.Win");
                    break;
            }

            return formRunner;
        }

        public static IStiProgressInformation GetProgressInformation(StiGuiMode guiMode)
        {
            return GetProgressInformation(null, guiMode);
        }

        public static IStiProgressInformation GetProgressInformation(object ownerForm, StiGuiMode guiMode)
        {
            switch (guiMode)
            {
                case StiGuiMode.Wpf:
                    {
                        if (ProgressWpfType != null)
                            return StiActivator.CreateObject(ProgressWpfType, new[] { ownerForm }) as IStiProgressInformation;

                        var type = Type.GetType($"Stimulsoft.Report.Wpf.StiProgressInformation, Stimulsoft.Report.Wpf, {StiVersion.VersionInfo}");
                        if (type == null)
                            throw new Exception("Assembly 'Stimulsoft.Report.Wpf' is not found");

                        return StiActivator.CreateObject(type, new[] { ownerForm }) as IStiProgressInformation;
                    }

                default:
                {
                    if (ProgressGdiType != null)
                        return StiActivator.CreateObject(ProgressGdiType, new[] { ownerForm }) as IStiProgressInformation;

                    var type = Type.GetType($"Stimulsoft.Report.Win.StiProgressInformation, Stimulsoft.Report.Win, {StiVersion.VersionInfo}");
                    if (type == null)
                        throw new Exception("Assembly 'Stimulsoft.Report.Win' is not found");

                    return StiActivator.CreateObject(type, new[] { ownerForm }) as IStiProgressInformation;                
                }
            }
        }

		public static IStiViewerForm GetViewerForm(StiReport report, StiGuiMode guiMode)
		{
		    switch (guiMode)
		    {
		        case StiGuiMode.Wpf:
		        {
		            if (report != null && report.ContainsDashboard)
		            {
		                var type = Type.GetType($"Stimulsoft.Dashboard.Viewer.Wpf.StiWpfDashboardViewerWindow, Stimulsoft.Dashboard.Viewer.Wpf, {StiVersion.VersionInfo}");
		                if (type == null)
		                    throw new Exception("Assembly 'Stimulsoft.Dashboard.Viewer.Wpf' is not found");

		                return StiActivator.CreateObject(type, new object[] { report }) as IStiViewerForm;
                    }
		            else
		            {
		                var type = Type.GetType($"Stimulsoft.Report.Viewer.StiWpfViewerWindow, Stimulsoft.Report.Wpf, {StiVersion.VersionInfo}");
		                if (type == null)
		                    throw new Exception("Assembly 'Stimulsoft.Report.Wpf' is not found");

		                return StiActivator.CreateObject(type, new object[] { report }) as IStiViewerForm;
		            }
		        }

		        default:
		        {
		            if (report != null && report.ContainsDashboard)
		            {
		                var type = Type.GetType($"Stimulsoft.Dashboard.Viewer.StiDashboardViewerForm, Stimulsoft.Dashboard.Viewer, {StiVersion.VersionInfo}");
		                if (type == null)
		                    throw new Exception("Assembly 'Stimulsoft.Dashboard.Viewer' is not found");

		                return StiActivator.CreateObject(type, new object[] { report }) as IStiViewerForm;
                    }
		            else
		            {
		                var type = Type.GetType($"Stimulsoft.Report.Viewer.StiViewerForm, Stimulsoft.Report.Win, {StiVersion.VersionInfo}");
		                if (type == null)
		                    throw new Exception("Assembly 'Stimulsoft.Report.Win' is not found");

		                return StiActivator.CreateObject(type, new object[] { report }) as IStiViewerForm;
		            }
		        }
		    }
		}

        public static IStiViewerForm GetViewerFormWithRibbonGUI(StiReport report, StiGuiMode guiMode)
        {
            switch (guiMode)
            {
                case StiGuiMode.Wpf:
                {
                    var type = Type.GetType($"Stimulsoft.Report.Viewer.StiWpfRibbonViewerWindow, Stimulsoft.Report.Wpf, {StiVersion.VersionInfo}");
                    if (type == null)
                        throw new Exception("Assembly 'Stimulsoft.Report.Wpf' is not found");

                    return StiActivator.CreateObject(type, new object[] { report }) as IStiViewerForm;
                }

                default:
                {
                    var type = Type.GetType($"Stimulsoft.Report.Viewer.StiRibbonViewerForm, Stimulsoft.Report.Win, {StiVersion.VersionInfo}");
                    if (type == null)
                        throw new Exception("Assembly 'Stimulsoft.Report.Win' is not found");

                    return StiActivator.CreateObject(type, new object[] { report }) as IStiViewerForm;
                }
            }
        }

		public static IStiViewerForm GetDotMatrixViewerForm(StiReport report, StiGuiMode guiMode)
		{
		    switch (guiMode)
		    {
		        case StiGuiMode.Wpf:
		        {
		            var type = Type.GetType($"Stimulsoft.Report.Viewer.StiWpfDotMatrixViewerWindow, Stimulsoft.Report.Wpf, {StiVersion.VersionInfo}");
		            if (type == null)
		                throw new Exception("Assembly 'Stimulsoft.Report.Wpf' is not found");

		            return StiActivator.CreateObject(type, new object[]{ report }) as IStiViewerForm;
		        }

		        default:
		        {
		            var type = Type.GetType($"Stimulsoft.Report.Viewer.StiDotMatrixViewerForm, Stimulsoft.Report.Win, {StiVersion.VersionInfo}");
		            if (type == null)
		                throw new Exception("Assembly 'Stimulsoft.Report.Win' is not found");

		            return StiActivator.CreateObject(type, new object[]{ report }) as IStiViewerForm;                
		        }
		    }
		}

        public static IStiViewerForm GetDotMatrixViewerFormWithRibbonGUI(StiReport report, StiGuiMode guiMode)
        {
            switch (guiMode)
            {
                case StiGuiMode.Wpf:
                    throw new Exception("Dot-Matrix viewer with Ribbon GUI is not available for WPF!");

                default:
                    var type = Type.GetType($"Stimulsoft.Report.Viewer.StiRibbonDotMatrixViewerForm, Stimulsoft.Report.Win, {StiVersion.VersionInfo}");
                    if (type == null)
                        throw new Exception("Assembly 'Stimulsoft.Report.Win' is not found");

                    return StiActivator.CreateObject(type, new object[] { report }) as IStiViewerForm;
            }
        }
    }
}