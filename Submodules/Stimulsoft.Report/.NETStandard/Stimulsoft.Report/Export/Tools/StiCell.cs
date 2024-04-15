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
	public class StiCell : ICloneable
	{
		#region ICloneable
		public virtual object Clone()
		{
			return this.MemberwiseClone() as StiCell;
		}
		#endregion

		#region Methods
		internal bool ForceExportAsImage(object exportImage)
		{
			if (ExportFormat == StiExportFormat.Html) return false;
			var textOptions = exportImage as IStiTextOptions;
			return textOptions != null && textOptions.TextOptions.Angle != 0;
		}
		#endregion

		#region Properties
	    public virtual StiExportFormat ExportFormat { get; }

	    private StiComponent component;
		public virtual StiComponent Component
		{
			get
			{
				return component;
			}
			set
			{
				component = value;
				ExportImage = value as IStiExportImage;
				
				if (ExportImage != null && ExportImage is IStiExportImageExtended)
				{
					if (!((IStiExportImageExtended)ExportImage).IsExportAsImage(ExportFormat) && !ForceExportAsImage(value))
					    ExportImage = null; 
				}
			}
		}


	    public virtual IStiExportImage ExportImage { get; set; }

	    public virtual StiCellStyle CellStyle { get; set; }

	    public int Left { get; set; }

	    public int Top { get; set; }

        private int width;
	    public int Width 
        { 
            get
            {
                return width;
            }
            set
            {
                width = Math.Max(value, 0);
            }
        }

	    private int height;
		public int Height
		{
			get
			{
				return height;
			}
			set
			{
				height = Math.Max(value, 0);
			}
		}

	    public string Text { get; set; } = string.Empty;
	    #endregion

		public StiCell() : this(StiExportFormat.None)
		{
		}

		public StiCell(StiExportFormat exportFormat)
		{
			this.ExportFormat = exportFormat;
		} 
	}
}
