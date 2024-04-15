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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Dashboard.Design;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Dashboard
{
    /// <summary>
    /// Describes the class that realizes object margin.
    /// </summary>
    [TypeConverter(typeof(StiMarginTypeConverter))]
    public class StiMargin : ICloneable
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

            jObject.AddPropertyDouble("Left", Left);            
            jObject.AddPropertyDouble("Top", Top);
            jObject.AddPropertyDouble("Right", Right);
            jObject.AddPropertyDouble("Bottom", Bottom);

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
        public virtual bool IsDefault => Left == 3d
            && Right == 3d
            && Top == 3d
            && Bottom == 3d;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets left margin size on the object.
        /// </summary>
        [Description("Gets or sets left margin size on the object.")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(3d)]
        [StiOrder(1)]
        public double Left { get; set; }
        
        /// <summary>
        /// Gets or sets top margin size on the object.
        /// </summary>
        [Description("Gets or sets top margin size on the object.")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(3d)]
        [StiOrder(2)]
        public double Top { get; set; }

        /// <summary>
        /// Gets or sets right margin size on the object.
        /// </summary>
        [Description("Gets or sets right margin size on the object.")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(3d)]
        [StiOrder(3)]
        public double Right { get; set; }

        /// <summary>
        /// Gets or sets bottom margin size on the object.
        /// </summary>
        [Description("Gets or sets bottom margin size on the object.")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(3d)]
        [StiOrder(4)]
        public double Bottom { get; set; }

        [Browsable(false)]
        public bool IsEmpty => Left == 0d && Top == 0d && Right == 0d && Bottom == 0d;
        #endregion

        #region Methods
        /// <summary>
        /// Tests to see whether the specified object is a StiMargin with the same dimensions as this StiMargin.
        /// </summary>
        /// <param name="obj">The Object to test.</param>
        /// <returns>This method returns true if obj is a StiMargin and has the same width and height as this StiMargin; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var margins = (StiMargin) obj;
            return margins != null
                   && margins.Left == Left                   
                   && margins.Top == Top
                   && margins.Right == Right
                   && margins.Bottom == Bottom;
        }

        /// <summary>
        /// Returns a hash code for this StiMargins structure.
        /// </summary>
        /// <returns>An integer value that specifies a hash value for this StiMargins structure.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Fields
        public static StiMargin Empty = new StiMargin();
        #endregion

        /// <summary>
        /// Creates a new object of the type StiMargin.
        /// </summary>
        public StiMargin() : this(3d)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiMargin.
        /// </summary>
        /// <param name="all">The margin size for all sides of the object.</param>
        public StiMargin(double all)
        {
            this.Left = this.Right = this.Top = this.Bottom = all;
        }

        /// <summary>
        /// Creates a new object of the type StiMargin.
        /// </summary>
        /// <param name="left">Left margin size on the object.</param>
        /// <param name="top">Top margin size on the object.</param>
        /// <param name="right">Right margin size on the object.</param>
        /// <param name="bottom">Bottom margin size on the object.</param>
        public StiMargin(double left, double top, double right, double bottom)
        {
            this.Left = left;
            this.Right = right;
            this.Top = top;            
            this.Bottom = bottom;
        }
    }
}