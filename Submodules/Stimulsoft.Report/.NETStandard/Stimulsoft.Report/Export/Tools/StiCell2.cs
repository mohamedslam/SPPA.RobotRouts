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
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Export
{
    public class StiCell2 : StiCell
    {
        #region ICloneable
        public override object Clone()
        {
            return this.MemberwiseClone() as StiCell2;
        }
        #endregion

        #region Fields
        private bool isExportImageNull;
        #endregion

        #region Properties
        internal StiMatrix Matrix { get; set; }

        public override StiExportFormat ExportFormat
        {
            get
            {
                return Matrix == null ? base.ExportFormat : Matrix.exportFormat;
            }
        }

        public override StiComponent Component
        {
            get
            {
                return Matrix.GetCellComponent(this);
            }
            set
            {
                Matrix.SetCellComponent(this, value, -1);
            }
        }
        
        public override IStiExportImage ExportImage
        {
            get
            {
                if (isExportImageNull) return null;

                var exportImage = Component as IStiExportImage;
                if (exportImage != null && exportImage is IStiExportImageExtended)
                {
                    if (!((IStiExportImageExtended)exportImage).IsExportAsImage(Matrix.exportFormat) && !ForceExportAsImage(Component))
                        exportImage = null;
                }

                return exportImage;
            }
            set
            {
                if (value == null)
                {
                    isExportImageNull = true;
                    return;
                }
                throw new Exception("Error in StiMatrix cache mode");
            }
        }

        internal int PageId { get; set; } = -1;

        internal int ComponentId { get; set; } = -1;

        //надо будет переписать может даже базовый класс
        internal int cellStyleId = -1;
        public override StiCellStyle CellStyle
        {
            get
            {
                return Matrix.Styles[cellStyleId] as StiCellStyle;
            }
            set
            {
                cellStyleId = Matrix.Styles.IndexOf(value);
            }
        }
        #endregion

        public StiCell2() : base(StiExportFormat.None)
        {
        }

        public StiCell2(StiExportFormat exportFormat) : base(exportFormat)
        {
        }

        public StiCell2(StiMatrix matrix) : base(StiExportFormat.None)
        {
            this.Matrix = matrix;
        }
    }
}
