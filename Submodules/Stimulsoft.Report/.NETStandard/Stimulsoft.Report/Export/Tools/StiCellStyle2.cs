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
using Stimulsoft.Base.Drawing;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Export
{
	public class StiCellStyle2 : ICloneable
	{
        #region ICloneable
		public object Clone()
		{
			StiCellStyle style =    this.MemberwiseClone() as StiCellStyle;
            //style.HorAlignment =    this.HorAlignment;
            //style.VertAlignment =   this.VertAlignment;

			if (this.Font != null)
				style.Font =            this.Font.Clone() as Font;
			
			if (this.TextOptions != null)
				style.TextOptions =     this.TextOptions.Clone() as StiTextOptions;

            //style.HorAlignment =    this.HorAlignment;

			return style;
		}
        #endregion

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	
		public override bool Equals(Object obj) 
		{
			StiCellStyle2 style = obj as StiCellStyle2;

			if (this.Color			!= style.Color)return false;
			if (this.HorAlignment	!= style.HorAlignment)return false;
			if (this.VertAlignment	!= style.VertAlignment)return false;
			if (this.TextColor		!= style.TextColor)return false;
			
			if (this.Font.Name		!= style.Font.Name)return false;
			if (this.Font.Bold		!= style.Font.Bold)return false;
			if (this.Font.Italic	!= style.Font.Italic)return false;
			if (this.Font.Strikeout != style.Font.Strikeout)return false;
			if (this.Font.Underline != style.Font.Underline)return false;
			if (this.Font.Size		!= style.Font.Size)return false;

			if (this.WordWrap		!= style.WordWrap)return false;

			if (this.TextOptions == null && style.TextOptions != null)return false;
			if (this.TextOptions != null && style.TextOptions == null)return false;
			
			if (this.TextOptions != null && style.TextOptions != null)
			{
				if (this.TextOptions.Angle != style.TextOptions.Angle)return false;
				if (this.TextOptions.WordWrap != style.TextOptions.WordWrap)return false;
				if (this.TextOptions.RightToLeft != style.TextOptions.RightToLeft)return false;
			}

			if (this.Format	!= style.Format)return false;

			if (this.InternalStyleName != style.InternalStyleName) return false;

			return true;
		}

		public bool AbsolutePosition = false;
		public Color Color;
		public Font Font;
		public StiTextHorAlignment HorAlignment;
		public StiVertAlignment VertAlignment;
		public StiTextOptions TextOptions;	
		public Color TextColor;
		public bool WordWrap;
		public string Format;

		private string internalStyleName = null;
		public string InternalStyleName
		{
			get
			{
				return internalStyleName;
			}
			set
			{
				if (value != null)
				{
					internalStyleName = value.Trim().Replace(" ", "");
				}
			}
		}

		private string styleName = null;
		public string StyleName
		{
			get
			{
				if (styleName == null)
				{
					styleName = InternalStyleName;
				    if (styleName == null)
				        styleName = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);
				}
				return styleName;
			}
			set
			{
				styleName = value;
			}
		}

		public StiCellStyle2(Color color, Color textColor, Font font, 
			StiTextHorAlignment horAlignment, StiVertAlignment vertAlignment,
			StiTextOptions textOptions, bool wordWrap, string format):
			this(color, textColor, font, horAlignment, vertAlignment, textOptions, wordWrap, format, null)
		{
		}

		public StiCellStyle2(Color color, Color textColor, Font font, 
			StiTextHorAlignment horAlignment, StiVertAlignment vertAlignment,
            StiTextOptions textOptions, bool wordWrap, string format, string styleName)
		{
			this.Color = color;
			this.TextColor = textColor;
			this.Font = font;
			this.HorAlignment = horAlignment;
			this.VertAlignment = vertAlignment;
			this.TextOptions = textOptions;
			this.WordWrap = wordWrap;
			this.Format = format;
			this.InternalStyleName = styleName;
		}

    }
}
