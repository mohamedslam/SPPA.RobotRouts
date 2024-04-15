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

namespace Stimulsoft.Report
{
    /// <summary>
    /// This class contains information about WPF theme for Reports.Wpf.
    /// </summary>
    public class StiWpfThemeInfo
    {
        #region Properties
        public string Name { get; set; }

        public string Assembly { get; set; }

        public string Path { get; set; }

        public string CloudToolTipPath { get; set; }

        public string CheckPath { get; set; }

        public bool IsLoaded
        {
            get
            {
                // Если CheckPath == null, значит это базованая тема в сборке Stimulsoft.Report.Wpf
                if (CheckPath == null)
                    return true;

                var type = Type.GetType(CheckPath + StiVersion.VersionInfo);
                return type != null;
            }
        }

        #endregion

        #region Methods
        public override string ToString()
        {
            return Name;
        }
        #endregion

        /// <summary>
        /// Creates new instance of StiWpfThemeInfo.
        /// </summary>
        /// <param name="name">Name of theme. For example: "Black".</param>
        /// <param name="assembly">Name of assembly which contains resources for WPF theme.</param>
        /// <param name="path">Path to resource dictionary in assembly. For example: "/Stimulsoft.Report.Wpf.Office2007BlueTheme;component/Office2007BlueTheme.xaml"</param>
        public StiWpfThemeInfo(string name, string assembly, string path, string cloudToolTipPath, string checkPath)
        {
            this.Name = name;
            this.Assembly = assembly;
            this.Path = path;
            this.CloudToolTipPath = cloudToolTipPath;
            this.CheckPath = checkPath;
        }
    }
}