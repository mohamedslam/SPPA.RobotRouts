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
using System.Drawing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Units;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Design
{
	/// <summary>
	/// Describes the class of report parameters for the designer.
	/// </summary>
    [Serializable]
	public class StiDesignerInfo
	{
        #region Properties
        /// <summary>
        /// Temporarily sets mode of the designing of components.
        /// </summary>
        [DefaultValue(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ForceDesigningMode { get; set; }

        [DefaultValue(StiQuickInfoType.None)]
        [StiSerializable]
        public StiQuickInfoType QuickInfoType { get; set; } = StiQuickInfoType.None;

        [DefaultValue(false)]
        [StiSerializable]
        public bool GenerateLocalizedName { get; set; }

        [DefaultValue(true)]
        [StiSerializable]
        public bool ShowDimensionLines { get; set; } = true;

        [DefaultValue(true)]
        [StiSerializable]
        public bool QuickInfoOverlay { get; set; } = true;

        public bool IsComponentsMoving { get; set; }

        /// <summary>
        /// True, if the report designer is creating a new component.
        /// </summary>
        internal bool IsComponentCreating { get; set; }

        /// <summary>
        /// True, if the report designer is processing drag and drop operation.
        /// </summary>
        internal bool IsDragDropComponent { get; set; }

        /// <summary>
        /// Dragging Style from the style designer.
        /// </summary>
        internal StiBaseStyle DraggingStyle { get; set; }

        /// <summary>
        /// True, if the report designer is processing drag and drop operation from the report dictionary.
        /// </summary>
        internal bool IsDragDropFromToolBox { get; set; }

        public StiComponent DraggingComponent { get; set; }

        public StiComponent DraggingLabelComponent { get; set; }        

        /// <summary>
        /// Currect action in designer.
        /// </summary>
        public StiAction CurrentAction { get; set; } = StiAction.None;

        [StiSerializable]
        [DefaultValue(StiMarkersStyle.Corners)]
        public StiMarkersStyle MarkersStyle { get; set; } = StiMarkersStyle.Corners;

        public bool IsTableMode { get; set; }

        [StiSerializable]
        [DefaultValue(true)]
        public bool DrawEventMarkers { get; set; } = true;

        [StiSerializable]
        [DefaultValue(true)]
        public bool DrawMarkersWhenMoving { get; set; } = true;
        
        /// <summary>
        /// Gets or sets value, indicates that it is necessary to run the component designer after the component has been created.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        public bool RunDesignerAfterInsert { get; set; } = true;
        
        /// <summary>
        /// Gets or sets value, indicates that format from last selected component copy to new components.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        public bool UseLastFormat { get; set; } = true;

        [StiSerializable]
        [DefaultValue(15)]
        public int AutoSaveInterval { get; set; } = 15;

        [StiSerializable]
        [DefaultValue(false)]
        public bool EnableAutoSaveMode { get; set; }
        
        /// <summary>
        /// Gets or sets value, indicates that order of components is to be shown on the page.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        public bool ShowOrder { get; set; }

        /// <summary>
        /// Gets or sets value, indicates that components are to be aligned to grid.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        public bool AlignToGrid { get; set; } = true;

        [StiSerializable]
        [DefaultValue(false)]
        public bool AutoSaveReportToReportClass { get; set; }
        
        /// <summary>
        /// Gets or sets value, indicates that component headers are to be shown from components.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        public bool ShowHeaders { get; set; } = true;
        
        /// <summary>
        /// Gets or sets value indicates that it is necessary to show grid on a page.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        public bool ShowGrid { get; set; } = true;

        public bool ShowInteractive { get; set; } = true;
        
        /// <summary>
        /// Gets or sets zoom of a report (1 = 100%).
        /// </summary>
        [StiSerializable]
        public double Zoom { get; set; } = .75f;
        
        /// <summary>
        /// Gets or sets value, indicates that it is necessary to show rulers.
        /// </summary>
        [Browsable(false)]
        [StiSerializable]
        public bool ShowRulers { get; set; } = true;
        
        /// <summary>
        /// Gets or sets the view mode of a page.
        /// </summary>
        [DefaultValue(StiViewMode.Normal)]
        [StiSerializable]
        public StiViewMode ViewMode { get; set; } = StiViewMode.Normal;

        /// <summary>
        /// Gets or sets a grid size in points for a dashboard object.
        /// </summary>
        [StiSerializable]
        [DefaultValue(20d)]
        public double GridSizePoints { get; set; } = 20d;

        /// <summary>
        /// Gets or sets a grid size in points for a screen object.
        /// </summary>
        [StiSerializable]
        [DefaultValue(10d)]
        public double GridSizeScreenPoints { get; set; } = 10d;

        /// <summary>
        /// Gets or sets grid size in pixels for a dialog form.
        /// </summary>
        [StiSerializable]
        [DefaultValue(8d)]
        public double GridSizePixels { get; set; } = 8d;
        
        /// <summary>
        /// Gets or sets grid size in centimeters.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0.2d)]
        public double GridSizeCentimetres { get; set; } = 0.2d;
        
        /// <summary>
        /// Gets or sets grid size in hundredths of inch.
        /// </summary>
        [StiSerializable]
        [DefaultValue(10d)]
        public double GridSizeHundredthsOfInch { get; set; } = 10d;
        
        /// <summary>
        /// Gets or sets grid size in inches.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0.1d)]
        public double GridSizeInch { get; set; } = 0.1d;
        
        /// <summary>
        /// Gets or sets grid size in millimeters.
        /// </summary>
        [StiSerializable]
        [DefaultValue(2d)]
        public double GridSizeMillimeters { get; set; } = 2d;

        public double GridSize
		{
			get
			{
				if (Report.Unit is StiMillimetersUnit)
                    return Report.Info.GridSizeMillimeters;

				if (Report.Unit is StiCentimetersUnit)
                    return Report.Info.GridSizeCentimetres;

				if (Report.Unit is StiHundredthsOfInchUnit)
                    return Report.Info.GridSizeHundredthsOfInch;

				return Report.Info.GridSizeInch;
			}
		}
        
        /// <summary>
        /// Gets or sets value, indicates that bands are to be filled when drawing them in the designer.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        public bool FillBands { get; set; } = true;
        
        /// <summary>
        /// Gets or sets value, indicates that cross-bands are to be filled when drawing them in the designer.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        public bool FillCrossBands { get; set; } = true;
        
        /// <summary>
        /// Gets or sets value indicates that it is necessary to fill containers when drawing them in the designer.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        public bool FillContainer { get; set; } = true;
        
        /// <summary>
        /// Gets or sets value indicates that it is necessary to fill components when drawing them in the designer.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        public bool FillComponent { get; set; } = true;
        
        /// <summary>
        /// Gets or sets value indicates that, when filling components, use colors in the designer parameters.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        public bool UseComponentColor { get; set; } = true;
        
        /// <summary>
        /// Gets or sets grid mode.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiGridMode.Lines)]
        public StiGridMode GridMode { get; set; } = StiGridMode.Lines;

		/// <summary>
		/// Gets or sets the report to which these parameters belong.
		/// </summary>
		public StiReport Report { get; set; }
		#endregion

		#region Methods
		/// <summary>
		/// Gets color of filling.
		/// </summary>
		/// <param name="color">Component color.</param>
		/// <returns>Color of filling.</returns>
		public Color GetFillColor(Color color)
		{
			if (!UseComponentColor)
			{
				var baseColor = StiColorUtils.Dark(Color.White, 40);
				return Color.FromArgb(100, baseColor);
			}
			else
			    return Color.FromArgb(40, color);
		}
		#endregion

		/// <summary>
		/// Creates a new object of the type StiDesignerInfo.
		/// </summary>
		/// <param name="report">Report are these parameters belong to.</param>
		public StiDesignerInfo(StiReport report)
		{
            this.Report = report;			
		}

		/// <summary>
		/// Creates a new object of the type StiDesignerInfo.
		/// </summary>
		public StiDesignerInfo() : this(null)
		{
		}
	}
}
