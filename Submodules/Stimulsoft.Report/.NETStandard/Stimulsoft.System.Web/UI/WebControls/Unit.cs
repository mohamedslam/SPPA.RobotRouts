using System;
using System.ComponentModel;
using System.Globalization;

namespace Stimulsoft.System.Web.UI.WebControls
{
    public struct Unit
    {
        #region Fields
        public static readonly Unit Empty = new Unit();

        private readonly UnitType type;
        private readonly double value;

        internal const int MaxValue = 32767;
        internal const int MinValue = -32768;
        #endregion

        #region Properties
        public bool IsEmpty
        {
            get
            {
                return type == (UnitType)0;
            }
        }

        public UnitType Type
        {
            get
            {
                if (!IsEmpty)
                {
                    return this.type;
                }
                else
                {
                    return UnitType.Pixel;
                }
            }
        }

        public double Value
        {
            get
            {
                return this.value;
            }
        }
        #endregion

        #region Methods.Static
        public static Unit Percentage(double n)
        {
            return new Unit(n, UnitType.Percentage);
        }

        public static Unit Pixel(int n)
        {
            return new Unit(n, UnitType.Pixel);
        }

        public static Unit Point(int n)
        {
            return new Unit(n, UnitType.Point);
        }

        internal static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }

        public override int GetHashCode()
        {
            return CombineHashCodes(type.GetHashCode(), value.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Unit))
            {
                return false;
            }
            Unit u = (Unit)obj;

            if (u.type == type && u.value == value)
            {
                return true;
            }

            return false;
        }

        public static bool operator ==(Unit left, Unit right)
        {
            return (left.type == right.type && left.value == right.value);
        }

        public static bool operator !=(Unit left, Unit right)
        {
            return (left.type != right.type || left.value != right.value);
        }

        private static string GetStringFromType(UnitType type)
        {
            switch (type)
            {
                case UnitType.Pixel:
                    return "px";
                case UnitType.Point:
                    return "pt";
                case UnitType.Pica:
                    return "pc";
                case UnitType.Inch:
                    return "in";
                case UnitType.Mm:
                    return "mm";
                case UnitType.Cm:
                    return "cm";
                case UnitType.Percentage:
                    return "%";
                case UnitType.Em:
                    return "em";
                case UnitType.Ex:
                    return "ex";
            }
            return String.Empty;
        }

        private static UnitType GetTypeFromString(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                if (value.Equals("px"))
                {
                    return UnitType.Pixel;
                }
                else if (value.Equals("pt"))
                {
                    return UnitType.Point;
                }
                else if (value.Equals("%"))
                {
                    return UnitType.Percentage;
                }
                else if (value.Equals("pc"))
                {
                    return UnitType.Pica;
                }
                else if (value.Equals("in"))
                {
                    return UnitType.Inch;
                }
                else if (value.Equals("mm"))
                {
                    return UnitType.Mm;
                }
                else if (value.Equals("cm"))
                {
                    return UnitType.Cm;
                }
                else if (value.Equals("em"))
                {
                    return UnitType.Em;
                }
                else if (value.Equals("ex"))
                {
                    return UnitType.Ex;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value");
                }
            }
            return UnitType.Pixel;
        }

        #endregion

        #region Methods
        public override string ToString()
        {
            return ToString((IFormatProvider)CultureInfo.CurrentCulture);
        }

        public string ToString(CultureInfo culture)
        {
            return ToString((IFormatProvider)culture);
        }

        public string ToString(IFormatProvider formatProvider)
        {
            if (IsEmpty)
                return String.Empty;

            // Double.ToString does not do the right thing, we get extra bits at the end
            string valuePart;
            if (type == UnitType.Pixel)
            {
                valuePart = ((int)value).ToString(formatProvider);
            }
            else
            {
                valuePart = ((float)value).ToString(formatProvider);
            }

            return valuePart + Unit.GetStringFromType(type);
        }
        #endregion

        #region Constructor
        public Unit(int value)
        {
            if ((value < MinValue) || (value > MaxValue))
            {
                throw new ArgumentOutOfRangeException("value");
            }

            this.value = value;
            this.type = UnitType.Pixel;
        }

        public Unit(double value)
        {
            if ((value < MinValue) || (value > MaxValue))
            {
                throw new ArgumentOutOfRangeException("value");
            }
            this.value = (int)value;
            this.type = UnitType.Pixel;
        }

        public Unit(double value, UnitType type)
        {
            if ((value < MinValue) || (value > MaxValue))
            {
                throw new ArgumentOutOfRangeException("value");
            }
            if (type == UnitType.Pixel)
            {
                this.value = (int)value;
            }
            else
            {
                this.value = value;
            }
            this.type = type;
        }

        public Unit(string value) : this(value, UnitType.Pixel)
        {
        }

        internal Unit(string value, UnitType defaultType)
        {
            if (String.IsNullOrEmpty(value))
            {
                this.value = 0;
                this.type = (UnitType)0;
            }
            else
            {
                // This is invariant because it acts like an enum with a number together. 
                // The enum part is invariant, but the number uses current culture. 
                string trimLcase = value.Trim().ToLower(CultureInfo.InvariantCulture);
                int len = trimLcase.Length;

                int lastDigit = -1;
                for (int i = 0; i < len; i++)
                {
                    char ch = trimLcase[i];
                    if (((ch < '0') || (ch > '9')) && (ch != '-') && (ch != '.') && (ch != ','))
                        break;
                    lastDigit = i;
                }
                if (lastDigit == -1)
                {
                    throw new FormatException($"UnitParseNoDigits {value}");
                }
                if (lastDigit < len - 1)
                {
                    type = (UnitType)GetTypeFromString(trimLcase.Substring(lastDigit + 1).Trim());
                }
                else
                {
                    type = defaultType;
                }

                string numericPart = trimLcase.Substring(0, lastDigit + 1);
                // Cannot use Double.FromString, because we don't use it in the ToString implementation
                try
                {
                    TypeConverter converter = new SingleConverter();
                    this.value = (Single)converter.ConvertFromString(null, CultureInfo.InvariantCulture, numericPart);

                    if (type == UnitType.Pixel)
                    {
                        this.value = (int)this.value;
                    }
                }
                catch
                {
                    throw new FormatException($"UnitParseNumericPart {value}, {numericPart}, {type.ToString("G")}");
                }
                if ((this.value < MinValue) || (this.value > MaxValue))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
            }
        }
        #endregion
    }
}
