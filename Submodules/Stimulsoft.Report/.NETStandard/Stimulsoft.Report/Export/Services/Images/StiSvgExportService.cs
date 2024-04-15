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

using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// A class for the export in the SVG image format.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceImage.png")]
	public class StiSvgExportService: StiImageExportService
    {
        #region Properties
        /// <summary>
        /// Gets a name of the export in the context menu.
        /// </summary>
        public override string ExportNameInMenu
        {
            get
            {
                return StiLocalization.Get("Export", "ExportTypeSvgFile");
            }
        }

        /// <summary>
        /// Returns a filter for files with svg images.
        /// </summary>
        /// <returns>Returns a filter for files with svg images.</returns>
        public override string GetFilter()
        {
            return StiLocalization.Get("FileFilters", "SvgFiles");
        }
        #endregion
    }
}
