using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System;
using System.Drawing;

namespace Stimulsoft.System.Drawing
{
    public class ColorConverter : TypeConverter
    {
        static StandardValuesCollection cached;
        static object creatingCached = new object();

        public ColorConverter() { }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        internal static Color StaticConvertFromString(ITypeDescriptorContext context, string s, CultureInfo culture)
        {
            if (culture == null)
                culture = CultureInfo.InvariantCulture;

            s = s.Trim();

            if (s.Length == 0)
                return Color.Empty;

            // Try to process both NamedColor and SystemColors from the KnownColor enumeration
            if (Char.IsLetter(s[0]))
            {
                KnownColor kc;
                try
                {
                    kc = (KnownColor)Enum.Parse(typeof(KnownColor), s, true);
                }
                catch (Exception e)
                {
                    // whatever happens MS throws an basic Exception
                    string msg = string.Format("Invalid color name '{0}'.", s);
                    throw new Exception(msg, new FormatException(msg, e));
                }
                return KnownColors.FromKnownColor(kc);
            }

            String numSeparator = culture.TextInfo.ListSeparator;
            Color result = Color.Empty;

            if (s.IndexOf(numSeparator) == -1)
            {
                bool sharp = (s[0] == '#');
                int start = sharp ? 1 : 0;
                bool hex = false;
                // deal with #hex, 0xhex and #0xhex
                if ((s.Length > start + 1) && (s[start] == '0'))
                {
                    hex = ((s[start + 1] == 'x') || (s[start + 1] == 'X'));
                    if (hex)
                        start += 2;
                }

                if (sharp || hex)
                {
                    s = s.Substring(start);
                    int argb;
                    try
                    {
                        argb = Int32.Parse(s, NumberStyles.HexNumber);
                    }
                    catch (Exception e)
                    {
                        // whatever happens MS throws an basic Exception
                        string msg = string.Format("Invalid Int32 value '{0}'.", s);
                        throw new Exception(msg, e);
                    }

                    // note that the default alpha value for a 6 hex digit (i.e. when none are present) is 
                    // 0xFF while shorter string defaults to 0xFF - unless both # an 0x are specified
                    if ((s.Length < 6) || ((s.Length == 6) && sharp && hex))
                        argb &= 0x00FFFFFF;
                    else if ((argb >> 24) == 0)
                        argb |= unchecked((int)0xFF000000);
                    result = Color.FromArgb(argb);
                }
            }

            if (result.IsEmpty)
            {
                Int32Converter converter = new Int32Converter();
                String[] components = s.Split(numSeparator.ToCharArray());

                // MS seems to convert the indivual component to int before
                // checking the number of components
                int[] numComponents = new int[components.Length];
                for (int i = 0; i < numComponents.Length; i++)
                {
                    numComponents[i] = (int)converter.ConvertFrom(context,
                        culture, components[i]);
                }

                switch (components.Length)
                {
                    case 1:
                        result = Color.FromArgb(numComponents[0]);
                        break;
                    case 3:
                        result = Color.FromArgb(numComponents[0], numComponents[1],
                            numComponents[2]);
                        break;
                    case 4:
                        result = Color.FromArgb(numComponents[0], numComponents[1],
                            numComponents[2], numComponents[3]);
                        break;
                    default:
                        throw new ArgumentException(s + " is not a valid color value.");
                }
            }

            if (!result.IsEmpty)
            {
                // Look for a named or system color with those values
                Color known = KnownColors.FindColorMatch(result);
                if (!known.IsEmpty)
                    return known;
            }

            return result;
        }


        public override object ConvertFrom(ITypeDescriptorContext context,
                            CultureInfo culture,
                            object value)
        {
            string s = value as string;
            if (s == null)
                return base.ConvertFrom(context, culture, value);

            return StaticConvertFromString(context, s, culture);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
                          CultureInfo culture,
                          object value,
                          Type destinationType)
        {
            if (value is Color)
            {
                Color color = (Color)value;
                if (destinationType == typeof(string))
                {
                    if (color == Color.Empty)
                        return string.Empty;

                    if (color.GetIsKnownColor() || color.IsNamedColor)
                        return color.Name;

                    String numSeparator = culture.TextInfo.ListSeparator;

                    StringBuilder sb = new StringBuilder();
                    if (color.A != 255)
                    {
                        sb.Append(color.A);
                        sb.Append(numSeparator);
                        sb.Append(" ");
                    }
                    sb.Append(color.R);
                    sb.Append(numSeparator);
                    sb.Append(" ");

                    sb.Append(color.G);
                    sb.Append(numSeparator);
                    sb.Append(" ");

                    sb.Append(color.B);
                    return sb.ToString();
                }
                else if (destinationType == typeof(InstanceDescriptor))
                {
                    if (color.IsEmpty)
                    {
                        return new InstanceDescriptor(typeof(Color).GetField("Empty"), null);
                    }
                    else if (color.GetIsSystemColor())
                    {
                        return new InstanceDescriptor(typeof(SystemColors).GetProperty(color.Name), null);
                    }
                    else if (color.GetIsKnownColor())
                    {
                        return new InstanceDescriptor(typeof(Color).GetProperty(color.Name), null);
                    }
                    else
                    {
                        MethodInfo met = typeof(Color).GetMethod("FromArgb", new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) });
                        return new InstanceDescriptor(met, new object[] { color.A, color.R, color.G, color.B });
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            lock (creatingCached)
            {
                if (cached != null)
                    return cached;
                Array colors = Array.CreateInstance(typeof(Color), KnownColors.ArgbValues.Length - 1);
                for (int i = 1; i < KnownColors.ArgbValues.Length; i++)
                {
                    colors.SetValue(KnownColors.FromKnownColor((KnownColor)i), i - 1);
                }

                Array.Sort(colors, 0, colors.Length, new CompareColors());
                cached = new StandardValuesCollection(colors);
            }

            return cached;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        sealed class CompareColors : IComparer
        {

            public int Compare(object x, object y)
            {
                return String.Compare(((Color)x).Name, ((Color)y).Name);
            }
        }
    }
}