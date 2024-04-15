#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Dashboard.Design;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Dashboard
{
    /// <summary>
    /// Describes the class that realizes object padding.
    /// </summary>
    [TypeConverter(typeof(StiPaddingTypeConverter))]
	public class StiPadding : 
        ICloneable,
        IStiDefault
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode, double defLeft, double defTop, double defRight, double defBotttom)
        {
            if (this.Left == defLeft &&                 
                this.Top == defTop &&
                this.Right == defRight &&
                this.Bottom == defBotttom)
                return null;

            var jObject = new JObject();

            jObject.AddPropertyDouble("Left", Left, defLeft);            
            jObject.AddPropertyDouble("Top", Top, defTop);
            jObject.AddPropertyDouble("Right", Right, defRight);
            jObject.AddPropertyDouble("Bottom", Bottom, defBotttom);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Left":
                        this.Left = property.DeserializeDouble();
                        break;                    

                    case "Top":
                        this.Top = property.DeserializeDouble();
                        break;

                    case "Right":
                        this.Right = property.DeserializeDouble();
                        break;

                    case "Bottom":
                        this.Bottom = property.DeserializeDouble();
                        break;
                }
            }
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
		{
			return this.MemberwiseClone();
		}
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public virtual bool IsDefault => Left == 5d
            && Right == 5d
            && Top == 5d
            && Bottom == 5d;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets left padding size on the object.
        /// </summary>
        [Description("Gets or sets left padding size on the object.")]
        [RefreshProperties(RefreshProperties.All)]
		[DefaultValue(5d)]
        [StiOrder(1)]
		public double Left { get; set; }

        /// <summary>
		/// Gets or sets top padding size on the object.
		/// </summary>
		[Description("Gets or sets top padding size on the object.")]
        [RefreshProperties(RefreshProperties.All)]
		[DefaultValue(5d)]
        [StiOrder(2)]
		public double Top { get; set; }

        /// <summary>
		/// Gets or sets right padding size on the object.
		/// </summary>
		[Description("Gets or sets right padding size on the object.")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(5d)]
        [StiOrder(3)]
        public double Right { get; set; }

        /// <summary>
		/// Gets or sets bottom padding size on the object.
		/// </summary>
		[Description("Gets or sets bottom padding size on the object.")]
        [RefreshProperties(RefreshProperties.All)]
		[DefaultValue(5d)]
        [StiOrder(4)]
		public double Bottom { get; set; }

        [Browsable(false)]
		public bool IsEmpty => IsAllEqual(0d);

        /// <summary>
        /// Gets true if all corners are equal to each other. Otherwise, false. 
        /// </summary>
        [Browsable(false)]
        public bool IsAllSame => IsAllEqual(Left);

        [Browsable(false)]
        public static StiPadding Empty => new StiPadding();
        #endregion

        #region Methods
        /// <summary>
        /// Tests to see whether the specified object is a StiPadding with the same dimensions as this StiPadding.
        /// </summary>
        /// <param name="obj">The Object to test.</param>
        /// <returns>This method returns true if obj is a StiPadding and has the same width and height as this StiPadding; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var paddings = (StiPadding)obj;
            return paddings != null 
                   && paddings.Left == Left                    
                   && paddings.Top == Top
                   && paddings.Right == Right
                   && paddings.Bottom == Bottom;
        }

        /// <summary>
		/// Returns a hash code for this StiPaddings structure.
		/// </summary>
		/// <returns>An integer value that specifies a hash value for this StiPaddings structure.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode(); 
		}

        public bool IsAllEqual(double value)
        {
            return Left == value &&
                Right == value &&
                Top == value &&
                Bottom == value;
        }
        #endregion

        /// <summary>
        /// Creates a new object of the type StiPadding.
        /// </summary>
        public StiPadding() : this(5d)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiPadding.
        /// </summary>
        /// <param name="all">The padding size for all sides of the object.</param>
        public StiPadding(double all)
        {
            Left = Top = Right = Bottom = all;
        }

        /// <summary>
        /// Creates a new object of the type StiPadding.
        /// </summary>
        /// <param name="left">Left padding size on the object.</param>
        /// <param name="top">Top padding size on the object.</param>
        /// /// <param name="right">Right padding size on the object.</param>
        /// <param name="bottom">Bottom padding size on the object.</param>
        public StiPadding(double left, double top, double right, double bottom)
		{
			this.Left = left;
			this.Top = top;
			this.Right = right;
			this.Bottom = bottom;
		}
	}
}