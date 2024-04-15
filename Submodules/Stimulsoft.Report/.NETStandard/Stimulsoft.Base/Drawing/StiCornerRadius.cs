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

using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using System;
using System.ComponentModel;

namespace Stimulsoft.Base.Drawing
{
    [TypeConverter(typeof(StiCornerRadiusConverter))]
    public class StiCornerRadius :
        ICloneable,
        IStiDefault
    {        
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
        public virtual bool IsDefault => TopLeft == 0f
            && TopRight == 0f
            && BottomRight == 0f
            && BottomLeft == 0f;
        #endregion

        #region Methods
        /// <summary>
        /// Tests to see whether the specified object is a StiCornerRadius with the same dimensions as this StiCornerRadius.
        /// </summary>
        /// <param name="obj">The Object to test.</param>
        /// <returns>This method returns true if obj is a StiCornerRadius and has the same width and height as this StiCornerRadius; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var radius = (StiCornerRadius)obj;
            return radius != null
                   && radius.TopLeft == TopLeft
                   && radius.TopRight == TopRight
                   && radius.BottomLeft == BottomLeft
                   && radius.BottomRight == BottomRight;
        }

        /// <summary>
        /// Returns a hash code for this StiMargins structure.
        /// </summary>
        /// <returns>An integer value that specifies a hash value for this StiMargins structure.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            var converter = new StiCornerRadiusConverter();

            return converter.ConvertToString(this);
        }

        public static StiCornerRadius TryParse(string str)
        {
            try
            {
                var converter = new StiCornerRadiusConverter();
                return converter.ConvertFromString(str) as StiCornerRadius;
            }
            catch
            {
                return new StiCornerRadius();
            }
        }

        public int GetUniqueCode()
        {
            unchecked
            {
                var hashCode = 0;

                hashCode = (hashCode * 397) ^ TopLeft.GetHashCode();
                hashCode = (hashCode * 397) ^ TopRight.GetHashCode();
                hashCode = (hashCode * 397) ^ BottomLeft.GetHashCode();
                hashCode = (hashCode * 397) ^ BottomRight.GetHashCode();

                return hashCode;
            }
        }
        #endregion

        #region Properties
        private float topLeft = 0f;
        [DefaultValue(0f)]
        [StiSerializable]
        [StiOrder(1)]
        [RefreshProperties(RefreshProperties.All)]
        public float TopLeft
        {
            get 
            { 
                return topLeft; 
            }
            set
            {
                if (topLeft != value)
                {
                    if (value < 0)
                        value = 0;

                    if (value > 30)
                        value = 30;

                    topLeft = value;
                }
            }
        }

        private float topRight = 0f;
        [DefaultValue(0f)]
        [StiSerializable]
        [StiOrder(2)]
        [RefreshProperties(RefreshProperties.All)]
        public float TopRight
        {
            get
            {
                return topRight;
            }
            set
            {
                if (topRight != value)
                {
                    if (value < 0)
                        value = 0;

                    if (value > 30)
                        value = 30;

                    topRight = value;
                }
            }
        }

        private float bottomRight = 0f;
        [DefaultValue(0f)]
        [StiSerializable]
        [StiOrder(3)]
        [RefreshProperties(RefreshProperties.All)]
        public float BottomRight
        {
            get
            {
                return bottomRight;
            }
            set
            {
                if (bottomRight != value)
                {
                    if (value < 0)
                        value = 0;

                    if (value > 30)
                        value = 30;

                    bottomRight = value;
                }
            }
        }

        private float bottomLeft = 0f;
        [DefaultValue(0f)]
        [StiSerializable]
        [StiOrder(4)]
        [RefreshProperties(RefreshProperties.All)]
        public float BottomLeft
        {
            get
            {
                return bottomLeft;
            }
            set
            {
                if (bottomLeft != value)
                {
                    if (value < 0)
                        value = 0;

                    if (value > 30)
                        value = 30;

                    bottomLeft = value;
                }
            }
        }

        /// <summary>
        /// Gets true if all corners are equal to 0. Otherwise, false. 
        /// </summary>
        [Browsable(false)]
        public bool IsEmpty => TopLeft == 0 && TopRight == 0 && BottomLeft == 0 && BottomRight == 0;

        /// <summary>
        /// Gets true if all corners are equal to each other. Otherwise, false. 
        /// </summary>
        [Browsable(false)]
        public bool IsAllSame => (TopLeft == TopRight) && (BottomLeft == BottomRight) && (TopLeft == BottomRight);
        #endregion

        public StiCornerRadius()
        {
        }

        public StiCornerRadius(float all)
            : this(all, all, all, all)
        {
        }

        public StiCornerRadius(float topLeft, float topRight, float bottomRight, float bottomLeft)
        {
            this.TopLeft = topLeft;
            this.TopRight = topRight;
            this.BottomRight = bottomRight;
            this.BottomLeft = bottomLeft;
        }
    }
}