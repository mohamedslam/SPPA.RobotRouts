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
using Stimulsoft.Report.Painters;

namespace Stimulsoft.Report.Components.ShapeTypes
{
    /// <summary>
    /// The class describes the shape type - Snip Diagonal Side Corner Rectangle.
    /// </summary>
    [StiServiceBitmap(typeof(StiShapeTypeService), "Stimulsoft.Report.Bmp.ShapeTypes.SnipDiagonalSideCornerRectangle.png")]
    [StiGdiShapeTypePainter(typeof(Stimulsoft.Report.Painters.StiSnipDiagonalSideCornerRectangleGdiShapeTypePainter))]
    [StiWpfShapeTypePainter("Stimulsoft.Report.Painters.StiSnipDiagonalSideCornerRectangleWpfShapeTypePainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    public class StiSnipDiagonalSideCornerRectangleShapeType : StiShapeTypeService
    {
        #region Properties.override
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string ServiceName => StiLocalization.Get("Shapes", "SnipDiagonalSideCornerRectangle");
        #endregion

        #region Methods.override
        public override StiComponentId ComponentId => StiComponentId.StiSnipDiagonalSideCornerRectangleShapeType;

        public override StiShapeTypeService CreateNew() => new StiSnipDiagonalSideCornerRectangleShapeType();
        #endregion
    }
}