#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using Stimulsoft.Base.Serializing;
using System.ComponentModel;
using System.Drawing;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Base.Drawing
{
    /// <summary>
    /// Class describes a multi-border.
    /// </summary>
    public class StiAdvancedBorder : StiBorder
	{
		#region ICloneable
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			var border = base.Clone() as StiAdvancedBorder;
            
            border.TopSide = TopSide.Clone() as StiBorderSide;
            border.BottomSide = BottomSide.Clone() as StiBorderSide;
            border.LeftSide = LeftSide.Clone() as StiBorderSide;
            border.RightSide = RightSide.Clone() as StiBorderSide;

			return border;
		}
		#endregion

        #region IEquatable
	    protected bool Equals(StiAdvancedBorder other)
	    {
	        return base.Equals(other) && Equals(BottomSide, other.BottomSide) && Equals(TopSide, other.TopSide) && Equals(LeftSide, other.LeftSide) && Equals(RightSide, other.RightSide);
	    }

	    public override bool Equals(object obj)
	    {
	        if (ReferenceEquals(null, obj))                 
                return false;

	        if (ReferenceEquals(this, obj)) 
                return true;

	        if (obj.GetType() != GetType()) 
                return false;

	        return Equals((StiAdvancedBorder) obj);
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            int hashCode = base.GetHashCode();

	            hashCode = (hashCode*397) ^ (BottomSide != null ? BottomSide.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ (TopSide != null ? TopSide.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ (LeftSide != null ? LeftSide.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ (RightSide != null ? RightSide.GetHashCode() : 0);

	            return hashCode;
	        }
	    }

	    public static bool operator ==(StiAdvancedBorder left, StiAdvancedBorder right)
	    {
	        return Equals(left, right);
	    }

	    public static bool operator !=(StiAdvancedBorder left, StiAdvancedBorder right)
	    {
	        return !Equals(left, right);
	    }
	    #endregion

        #region Methods
		/// <summary>
		/// Draws this border on the indicated Graphics.
		/// </summary>
		/// <param name="g">Graphics on which a border can be drawn.</param>
		/// <param name="rect">The rectangle that indicates an area of the border drawing.</param>
		/// <param name="zoom">The scale of a border to draw.</param>
		/// <param name="emptyColor">The color of space between double lines (used only when border style equal Double).</param>
        public override void Draw(Graphics g, RectangleF rect, float zoom, Color emptyColor, bool drawBorderFormatting, bool drawBorderSides)
		{
		    if (drawBorderFormatting)
		        DrawBorderShadow(g, rect, zoom);

		    if (drawBorderSides)
            {
                if (IsLeftBorderSidePresent) 
                    LeftSide.Draw(g, rect, zoom, emptyColor, this);

                if (IsRightBorderSidePresent) 
                    RightSide.Draw(g, rect, zoom, emptyColor, this);

                if (IsTopBorderSidePresent) 
                    TopSide.Draw(g, rect, zoom, emptyColor, this);

                if (IsBottomBorderSidePresent) 
                    BottomSide.Draw(g, rect, zoom, emptyColor, this);
            }
		}
        #endregion

        #region Properties
        public override StiBorderIdent Ident => StiBorderIdent.AdvancedBorder;

	    /// <summary>
        /// Gets or sets frame of left side.
        /// </summary>
        [StiSerializable]
        [StiOrder(10)]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiBorderSideConverter))]
        [Description("Gets or sets frame of left side.")]
        [RefreshProperties(RefreshProperties.All)]
        public virtual StiBorderSide LeftSide { get; set; }

	    /// <summary>
        /// Gets or sets frame of right side.
        /// </summary>
        [StiSerializable]
        [StiOrder(20)]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiBorderSideConverter))]
        [Description("Gets or sets frame of right side.")]
        [RefreshProperties(RefreshProperties.All)]
        public virtual StiBorderSide RightSide { get; set; }

	    /// <summary>
        /// Gets or sets frame of top side.
        /// </summary>
        [StiSerializable]
        [StiOrder(30)]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiBorderSideConverter))]
        [Description("Gets or sets frame of top side.")]
        [RefreshProperties(RefreshProperties.All)]
        public virtual StiBorderSide TopSide { get; set; }

	    /// <summary>
        /// Gets or sets frame of bottom side.
        /// </summary>
        [StiSerializable]
        [StiOrder(40)]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiBorderSideConverter))]
        [Description("Gets or sets frame of bottom side.")]
        [RefreshProperties(RefreshProperties.All)]
        public virtual StiBorderSide BottomSide { get; set; }

	    /// <summary>
		/// Gets value which indicates that top side of border is present.
		/// </summary>
		public override bool IsTopBorderSidePresent => TopSide.Style != StiPenStyle.None;

	    /// <summary>
		/// Gets value which indicates that bottom side of border is present.
		/// </summary>
		public override bool IsBottomBorderSidePresent => BottomSide.Style != StiPenStyle.None;

	    /// <summary>
		/// Gets value which indicates that left side of border is present.
		/// </summary>
		public override bool IsLeftBorderSidePresent => LeftSide.Style != StiPenStyle.None;

	    /// <summary>
		/// Gets value which indicates that right side of border is present.
		/// </summary>
		public override bool IsRightBorderSidePresent => RightSide.Style != StiPenStyle.None;

	    /// <summary>
		/// Gets value which indicates that all sides of border is present.
		/// </summary>
		public override bool IsAllBorderSidesPresent => IsLeftBorderSidePresent &&
		                                                IsRightBorderSidePresent &&
		                                                IsTopBorderSidePresent &&
		                                                IsBottomBorderSidePresent;

	    /// <summary>
        /// Gets or sets frame borders. Not used in StiAdvancedBorder.
		/// </summary>
        public override StiBorderSides Side
		{
			get 
			{
                var side = StiBorderSides.None;

                if (IsLeftBorderSidePresent) 
                    side |= StiBorderSides.Left;

                if (IsRightBorderSidePresent) 
                    side |= StiBorderSides.Right;

                if (IsTopBorderSidePresent)
                    side |= StiBorderSides.Top;

                if (IsBottomBorderSidePresent) 
                    side |= StiBorderSides.Bottom;

				return side;
			}
			set 
			{
                if ((value & StiBorderSides.Left) > 0)
                {
                    if (!IsLeftBorderSidePresent)
                        LeftSide.Style = StiPenStyle.Solid;
                }
                else
                {
                    LeftSide.Style = StiPenStyle.None;
                }

                if ((value & StiBorderSides.Right) > 0)
                {
                    if (!IsRightBorderSidePresent)
                        RightSide.Style = StiPenStyle.Solid;
                }
                else
                {
                    RightSide.Style = StiPenStyle.None;
                }

                if ((value & StiBorderSides.Top) > 0)
                {
                    if (!IsTopBorderSidePresent)
                        TopSide.Style = StiPenStyle.Solid;
                }
                else
                {
                    TopSide.Style = StiPenStyle.None;
                }

                if ((value & StiBorderSides.Bottom) > 0)
                {
                    if (!IsBottomBorderSidePresent)
                        BottomSide.Style = StiPenStyle.Solid;
                }
                else
                {
                    BottomSide.Style = StiPenStyle.None;
                }
			}
		}

		/// <summary>
        /// Gets or sets a border color. Not used in StiAdvancedBorder.
		/// </summary>
        [Browsable(false)]
        [StiNonSerialized]
        public override Color Color
		{
			get 
			{
			    return LeftSide.Color;
			}
			set 
			{
                LeftSide.Color = value;
                RightSide.Color = value;
                TopSide.Color = value;
                BottomSide.Color = value;
			}
		}

		/// <summary>
        /// Gets or sets a border size. Not used in StiAdvancedBorder.
		/// </summary>
        [Browsable(false)]
        [StiNonSerialized]
        public override double Size
		{
			get 
			{
				return LeftSide.Size;
			}
			set 
			{
                LeftSide.Size = value;
                RightSide.Size = value;
                TopSide.Size = value;
                BottomSide.Size = value;
			}
		}

		/// <summary>
        /// Gets or sets a border style. Not used in StiAdvancedBorder.
		/// </summary>		
        [Browsable(false)]
        [StiNonSerialized]
        public override StiPenStyle Style
		{
			get 
			{
				return LeftSide.Style;
			}
			set 
			{
                LeftSide.Style = value;
                RightSide.Style = value;
                TopSide.Style = value;
                BottomSide.Style = value;
			}
		}

		/// <summary>
		/// Gets value indicates, that this object-frame is by default.
		/// </summary>
        public override bool IsDefault
		{
			get
			{
				return                     
					!DropShadow &&
                    !Topmost &&
					ShadowSize == 4d &&					
					ShadowBrush is StiSolidBrush &&
					((StiSolidBrush)ShadowBrush).Color == Color.Black &&
                    LeftSide.IsDefault &&
                    RightSide.IsDefault &&
                    TopSide.IsDefault &&
                    BottomSide.IsDefault;
			}
		}
		#endregion	

		/// <summary>
        /// Creates a new instance of the StiAdvancedBorder class.
		/// </summary>
		public StiAdvancedBorder() 
            : this(
                  new StiBorderSide(),
                  new StiBorderSide(),
                  new StiBorderSide(),
                  new StiBorderSide(), 
                  false, 4d, new StiSolidBrush(Color.Black))
		{
		}
	
		/// <summary>
        /// Creates a new instance of the StiAdvancedBorder class.
		/// </summary>
        public StiAdvancedBorder(
                Color topSideColor, double topSideSize, StiPenStyle topSideStyle, 
                Color bottomSideColor, double bottomSideSize, StiPenStyle bottomSideStyle, 
                Color leftSideColor, double leftSideSize, StiPenStyle leftSideStyle, 
                Color rightSideColor, double rightSideSize, StiPenStyle rightSideStyle, 
                bool dropShadow, double shadowSize, StiBrush shadowBrush) 
            : this(
                new StiBorderSide(topSideColor, topSideSize, topSideStyle), 
                new StiBorderSide(bottomSideColor, bottomSideSize, bottomSideStyle), 
                new StiBorderSide(leftSideColor, leftSideSize, leftSideStyle), 
                new StiBorderSide(rightSideColor, rightSideSize, rightSideStyle), 
                dropShadow, shadowSize, shadowBrush)
        {
        }

        /// <summary>
        /// Creates a new instance of the StiAdvancedBorder class.
        /// </summary>
        public StiAdvancedBorder(
            Color topSideColor, double topSideSize, StiPenStyle topSideStyle,
            Color bottomSideColor, double bottomSideSize, StiPenStyle bottomSideStyle,
            Color leftSideColor, double leftSideSize, StiPenStyle leftSideStyle,
            Color rightSideColor, double rightSideSize, StiPenStyle rightSideStyle,
            bool dropShadow, double shadowSize, StiBrush shadowBrush, bool topmost)
            : this(
                new StiBorderSide(topSideColor, topSideSize, topSideStyle),
                new StiBorderSide(bottomSideColor, bottomSideSize, bottomSideStyle),
                new StiBorderSide(leftSideColor, leftSideSize, leftSideStyle),
                new StiBorderSide(rightSideColor, rightSideSize, rightSideStyle),
                dropShadow, shadowSize, shadowBrush, topmost)
        {
        }

        /// <summary>
        /// Creates a new instance of the StiAdvancedBorder class.
        /// </summary>
        /// <param name="topSide">Top side of border.</param>
        /// <param name="bottomSide">Bottom side of border.</param>
        /// <param name="leftSide">Left side of border.</param>
        /// <param name="rightSide">Right side of border.</param>
        /// <param name="dropShadow">Drop shadow or not.</param>
        /// <param name="shadowSize">Shadow siz.</param>
        /// <param name="shadowBrush">Brush for drawing shadow of border.</param>
        public StiAdvancedBorder(StiBorderSide topSide, StiBorderSide bottomSide, StiBorderSide leftSide, StiBorderSide rightSide,
            bool dropShadow, double shadowSize, StiBrush shadowBrush) 
            : this(topSide, bottomSide, leftSide, rightSide, dropShadow, shadowSize, shadowBrush, false)
        {
        }

		/// <summary>
        /// Creates a new instance of the StiAdvancedBorder class.
		/// </summary>
		/// <param name="topSide">Top side of border.</param>
        /// <param name="bottomSide">Bottom side of border.</param>
        /// <param name="leftSide">Left side of border.</param>
        /// <param name="rightSide">Right side of border.</param>
		/// <param name="dropShadow">Drop shadow or not.</param>
		/// <param name="shadowSize">Shadow siz.</param>
		/// <param name="shadowBrush">Brush for drawing shadow of border.</param>
        /// <param name="topmost">Value which indicates that border sides will be drawn on top of all components.</param>
        public StiAdvancedBorder(StiBorderSide topSide, StiBorderSide bottomSide, StiBorderSide leftSide, StiBorderSide rightSide,
            bool dropShadow, double shadowSize, StiBrush shadowBrush, bool topmost)
		{
            TopSide = topSide;
            BottomSide = bottomSide;
            LeftSide = leftSide;
            RightSide = rightSide;

            LeftSide.side = StiBorderSides.Left;
            RightSide.side = StiBorderSides.Right;
            TopSide.side = StiBorderSides.Top;
            BottomSide.side = StiBorderSides.Bottom;

            ShadowBrush = shadowBrush;
            ShadowSize = shadowSize;
            DropShadow = dropShadow;
            Topmost = topmost;
		}
	}
}
