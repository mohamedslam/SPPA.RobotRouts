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

using System;
using System.Linq;
using Stimulsoft.Drawing.Text;

namespace Stimulsoft.Drawing
{
    public sealed class StringFormat : IDisposable, ICloneable
    {
        private float[] tabStops;
        private float firstTabOffset;
        internal CharacterRange[] measurableCharacterRanges = new CharacterRange[0];

        public static StringFormat GenericTypographic { get; } = new StringFormat();

        public static StringFormat GenericDefault { get; } = new StringFormat();

        internal System.Drawing.StringFormat netFormat;

        private System.Drawing.StringAlignment alignment = System.Drawing.StringAlignment.Near;
        public System.Drawing.StringAlignment Alignment
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFormat.Alignment;
                else
                    return alignment;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netFormat.Alignment = value;
                else
                    alignment = value;
            }
        }

        private System.Drawing.StringAlignment lineAlignment = System.Drawing.StringAlignment.Near;
        public System.Drawing.StringAlignment LineAlignment
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFormat.LineAlignment;
                else
                    return lineAlignment;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netFormat.LineAlignment = (System.Drawing.StringAlignment)value;
                else
                    lineAlignment = value;
            }
        }

        private System.Drawing.StringFormatFlags formatFlags = 0;
        public System.Drawing.StringFormatFlags FormatFlags
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFormat.FormatFlags;
                else
                    return formatFlags;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netFormat.FormatFlags = (System.Drawing.StringFormatFlags)value;
                else
                    formatFlags = value;
            }
        }

        private System.Drawing.StringTrimming trimming = System.Drawing.StringTrimming.None;
        public System.Drawing.StringTrimming Trimming
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFormat.Trimming;
                else
                    return trimming;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netFormat.Trimming = (System.Drawing.StringTrimming)value;
                else
                    trimming = value;
            }
        }

        private System.Drawing.Text.HotkeyPrefix hotkeyPrefix;
        public System.Drawing.Text.HotkeyPrefix HotkeyPrefix
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFormat.HotkeyPrefix;
                else
                    return hotkeyPrefix;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netFormat.HotkeyPrefix = (System.Drawing.Text.HotkeyPrefix)value;
                else
                    hotkeyPrefix = value;
            }
        }

        public void SetTabStops(float firstTabOffset, float[] tabStops)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netFormat.SetTabStops(firstTabOffset, tabStops);
            else
            {
                this.firstTabOffset = firstTabOffset;
                this.tabStops = tabStops;
            }
        }

        public float[] GetTabStops(out float firstTabOffset)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return netFormat.GetTabStops(out firstTabOffset);
            else
            {
                firstTabOffset = this.firstTabOffset;
                return tabStops;
            }
        }

        public void SetMeasurableCharacterRanges(CharacterRange[] ranges)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
            {
                var cr = Array.ConvertAll(ranges, (range => (System.Drawing.CharacterRange)range));
                netFormat.SetMeasurableCharacterRanges(cr);
            }
            else
                measurableCharacterRanges = ranges;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Dispose()
        {
        }

        public override string ToString()
        {
            return "[StringFormat, FormatFlags=" + this.FormatFlags.ToString() + "]";
        }

        public StringFormat()
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netFormat = new System.Drawing.StringFormat();
        }

        public StringFormat(StringFormat format)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netFormat = new System.Drawing.StringFormat(format.netFormat);
            else
            {
                this.tabStops = format.tabStops;
                this.firstTabOffset = format.firstTabOffset;
                this.measurableCharacterRanges = format.measurableCharacterRanges;
                this.alignment = format.alignment;
                this.lineAlignment = format.lineAlignment;
                this.formatFlags = format.formatFlags;
                this.trimming = format.trimming;
                this.hotkeyPrefix = format.hotkeyPrefix;
            }
        }
    }
}
