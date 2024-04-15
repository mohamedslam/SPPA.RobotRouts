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
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base;

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// Describes class that realizes base component for all primitives.
	/// </summary>
	[StiToolbox(true)]
    [StiServiceCategoryBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.StiShape.png")]
	public abstract class StiPrimitive : StiComponent
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("CanShrink");
            jObject.RemoveProperty("CanGrow");
            jObject.RemoveProperty("Shift");
            jObject.RemoveProperty("UseParentStyles");
            jObject.RemoveProperty("DockStyle");
            jObject.RemoveProperty("GrowToHeight");

            return jObject;
        }
        #endregion

        #region IStiCanShrink override
        [Browsable(false)]
		[StiNonSerialized]
		public override bool CanShrink
		{
			get
			{
				return base.CanShrink;
			}
			set
			{
			}
		}
		#endregion

		#region IStiCanGrow override
		[Browsable(false)]
		[StiNonSerialized]
		public override bool CanGrow
		{
			get
			{
				return base.CanGrow;
			}
			set
			{
			}
		}
		#endregion

		#region IStiShift override
		[Browsable(false)]
		[StiNonSerialized]
		public override bool Shift
		{
			get
			{
				return base.Shift;
			}
			set
			{
			}
		}
		#endregion

		#region StiComponent override
        [Browsable(false)]
        [StiNonSerialized]
        public override bool UseParentStyles
        {
            get
            {
                return base.UseParentStyles;
            }
            set
            {
                base.UseParentStyles = value;
            }
        }

		[Browsable(false)]
		[StiNonSerialized]
		public override StiDockStyle DockStyle
		{
			get 
			{
				return base.DockStyle;
			}
			set 
			{
			}
		}


		[Browsable(false)]
		[StiNonSerialized]
		public override bool GrowToHeight
		{
			get 
			{
				return base.GrowToHeight;
			}
			set 
			{
			}
		}

		/// <summary>
		/// Gets a localized name of the component category.
		/// </summary>
		public override string LocalizedCategory => StiLocalization.Get("Report", "Shapes");

	    /// <summary>
		/// Gets the type of processing when printing.
		/// </summary>
		public override StiComponentType ComponentType => StiComponentType.Simple;

	    /// <summary>
		/// Gets a component priority.
		/// </summary>
		[Browsable(false)]
		public override int Priority => (int)StiComponentPriority.Primitive;
	    #endregion
		
		#region Properties
		/// <summary>
		/// Gets or sets the client area of a component.
		/// </summary>
		[Browsable(false)]
		[StiSerializable]
		public override RectangleD ClientRectangle
		{
			get
			{
				return new RectangleD(Left, Top, Width, Height);
			}
			set
			{
				this.Left =		Math.Round(value.Left, 2);
				this.Top =		Math.Round(value.Top, 2);
				this.Width =	Math.Round(value.Width, 2);
				this.Height =	Math.Round(value.Height, 2);
			}
		}

		/// <summary>
		/// Gets or sets a rectangle of the component which it fills. Docking occurs in accordance to the area
		/// (Cross - components are docked by ClientRectangle).
		/// </summary>
		[Browsable(false)]
		public override RectangleD DisplayRectangle
		{
			get
			{
				return new RectangleD(Left, Top, Width, Height);
			}
			set
			{
				this.Left =		Math.Round(value.Left, 2);
				this.Top =		Math.Round(value.Top, 2);
				this.Width =	Math.Round(value.Width, 2);
				this.Height =	Math.Round(value.Height, 2);
			}
		}
        #endregion

        #region Methods
        protected internal override void SetDirectDisplayRectangle(RectangleD rect)
        {
            this.DisplayRectangle = rect;
        }
        #endregion

        /// <summary>
        /// Creates a new StiPrimitive.
        /// </summary>
        public StiPrimitive() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new StiPrimitive.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		public StiPrimitive(RectangleD rect): base(rect)
		{
			PlaceOnToolbox = false;
		}
	}
}
