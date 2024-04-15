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

using System.ComponentModel;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Engine;
using Stimulsoft.Base.Json.Linq;
using System;
using Stimulsoft.Base;

namespace Stimulsoft.Report.Components
{
	[StiToolbox(false)]
	[StiDesigner("Stimulsoft.Report.Components.Design.StiPointPrimitiveDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfPointPrimitiveDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiGdiPainter(typeof(StiPointPrimitiveGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiPointPrimitiveWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	[StiV1Builder(typeof(StiPointPrimitiveV1Builder))]
	[StiV2Builder(typeof(StiPointPrimitiveV2Builder))]
	public abstract class StiPointPrimitive : 
		StiPrimitive, 
		IStiHideFromReportTree,
		IStiComponentGuidReference		
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("IsSelected");

            // StiPointPrimitive
            jObject.AddPropertyString("ReferenceToGuid", ReferenceToGuid);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "ReferenceToGuid":
                        this.ReferenceToGuid = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region IStiSelect override
        /// <summary>
        /// Gets or sets value indicates is the component selected or not.
        /// </summary>
        [StiNonSerialized]
		public override bool IsSelected
		{
			get 
			{
				return false;
			}
			set
			{
			}
		}
		#endregion

		#region IStiComponentGuidReference
	    /// <summary>
		/// Gets or sets a reference to component with guid.
		/// </summary>
		[StiSerializable]
		[Browsable(false)]
		public string ReferenceToGuid { get; set; }
	    #endregion

		#region StiComponent override
		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition => 0;

	    public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Shapes;

	    /// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName => "";
	    #endregion

		#region StiComponent.Position
		public override double Width
		{
			get 
			{
				return 0;
			}
			set 
			{
			}
		}

		public override double Height
		{
			get 
			{
				return 0;
			}
			set 
			{
			}
		}
		#endregion

		#region Properties
        /// <summary>
        /// Содержит номер колонки для правильного распределения примитивов при пост-рендеринге страницы.
        /// </summary>
        [Browsable(false)]
		internal int StoredColumn { get; set; } = 0;
        #endregion

        /// <summary>
        /// Creates a new StiPointPrimitive.
        /// </summary>
        public StiPointPrimitive() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new StiPointPrimitive.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		public StiPointPrimitive(RectangleD rect) : base(rect)
		{			
			PlaceOnToolbox = true;
		}
	}
}