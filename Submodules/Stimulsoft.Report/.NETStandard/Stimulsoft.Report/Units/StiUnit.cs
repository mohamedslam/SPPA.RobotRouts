#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	Stimulsoft.Report Library										}
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

using System.Linq;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;

namespace Stimulsoft.Report.Units
{
	/// <summary>
	/// Report units.
	/// </summary>
    public abstract class StiUnit
    {
        #region IStiJsonReportObject.override
        public static JObject SaveToJsonObject(StiUnit unit)
        {
            var jObject = new JObject();
            jObject.AddPropertyIdent("Ident", unit.GetType().Name);
            return jObject;
        }

        public static StiUnit LoadFromJsonObject(JObject jObject)
        {
            var ident = jObject.Properties().FirstOrDefault(x => x.Name == "Ident").Value.ToObject<string>();

            switch (ident)
            {
                case "StiMillimetersUnit":
                    return new StiMillimetersUnit();

                case "StiInchesUnit":
                    return new StiInchesUnit();

                case "StiHundredthsOfInchUnit":
                    return new StiHundredthsOfInchUnit();

                case "StiCentimetersUnit":
                    return new StiCentimetersUnit();
            }

            return null;
        }
        #endregion

        #region Consts
        public static StiCentimetersUnit Centimeters = new StiCentimetersUnit();
		public static StiHundredthsOfInchUnit HundredthsOfInch = new StiHundredthsOfInchUnit();
		public static StiInchesUnit Inches = new StiInchesUnit();
		public static StiMillimetersUnit Millimeters = new StiMillimetersUnit();
        #endregion

        #region Properties
        /// <summary>
		/// Gets ruler step.
		/// </summary>
		public abstract double RollerStep { get; }

		/// <summary>
		/// Gets the ruler scale in hundredths of inch on the one step.
		/// </summary>
		public abstract double Factor { get; }

		/// <summary>
		/// Gets the shor unit name.
		/// </summary>
		public abstract string ShortName { get; }
        #endregion

        #region Methods
        public static StiUnit GetUnitFromReportUnit(StiReportUnitType reportUnit)
        {
            switch (reportUnit)
            {
                case StiReportUnitType.HundredthsOfInch:
                    return HundredthsOfInch;

                case StiReportUnitType.Inches:
                    return Inches;

                case StiReportUnitType.Millimeters:
                    return Millimeters;

                default:
                    return Centimeters;
            }
        }

        /// <summary>
		/// Converts a value from hundredths of inch into units of this class.
		/// </summary>
		/// <param name="value">Value for conversion.</param>
		/// <returns>Converted value.</returns>
		public abstract double ConvertToHInches(double value);

        /// <summary>
        /// Converts a value from hundredths of inch into units of this class.
        /// </summary>
        /// <param name="value">Value for conversion.</param>
        /// <returns>Converted value.</returns>
        public abstract decimal ConvertToHInches(decimal value);

		/// <summary>
		///  Converts a value from hundredths of inch into units of this class.
		/// </summary>
		/// <param name="value">Value for conversion.</param>
		/// <returns>Converted value.</returns>
		public abstract double ConvertFromHInches(double value);

        /// <summary>
        ///  Converts a value from hundredths of inch into units of this class.
        /// </summary>
        /// <param name="value">Value for conversion.</param>
        /// <returns>Converted value.</returns>
        public abstract decimal ConvertFromHInches(decimal value);

		/// <summary>
		/// Converts a rectangle from units of this class into hundredths of inch.
		/// </summary>
		/// <param name="rect">Rectangle for conversion.</param>
		/// <returns>Converted rectangle.</returns>
		public RectangleD ConvertToHInches(RectangleD rect)
		{
			return new RectangleD(
				ConvertToHInches(rect.Left),
				ConvertToHInches(rect.Top),
				ConvertToHInches(rect.Width),
				ConvertToHInches(rect.Height));
		}

        /// <summary>
        /// Converts a rectangle from units of this class into hundredths of inch.
        /// </summary>
        /// <param name="rect">Rectangle for conversion.</param>
        /// <returns>Converted rectangle.</returns>
        public RectangleM ConvertToHInches(RectangleM rect)
        {
            return new RectangleM(
                ConvertToHInches(rect.Left),
                ConvertToHInches(rect.Top),
                ConvertToHInches(rect.Width),
                ConvertToHInches(rect.Height));
        }
		
		/// <summary>
		/// Converts a rectangle from hundredths of inch into units of this class.
		/// </summary>
		/// <param name="rect">Rectangle for conversion.</param>
		/// <returns>Converted rectangle.</returns>
		public RectangleD ConvertFromHInches(RectangleD rect)
		{
			return new RectangleD(
				ConvertFromHInches(rect.Left),
				ConvertFromHInches(rect.Top),
				ConvertFromHInches(rect.Width),
				ConvertFromHInches(rect.Height));
		}

        /// <summary>
        /// Converts a rectangle from hundredths of inch into units of this class.
        /// </summary>
        /// <param name="rect">Rectangle for conversion.</param>
        /// <returns>Converted rectangle.</returns>
        public RectangleM ConvertFromHInches(RectangleM rect)
        {
            return new RectangleM(
                ConvertFromHInches(rect.Left),
                ConvertFromHInches(rect.Top),
                ConvertFromHInches(rect.Width),
                ConvertFromHInches(rect.Height));
        }

		/// <summary>
		/// Converts a size from units of this class into hundredths of inch.
		/// </summary>
		/// <param name="size">Size for conversion.</param>
		/// <returns>Converted size.</returns>
		public SizeD ConvertToHInches(SizeD size)
		{
			return new SizeD(
				ConvertToHInches(size.Width),
				ConvertToHInches(size.Height));
		}

        /// <summary>
        /// Converts a size from units of this class into hundredths of inch.
        /// </summary>
        /// <param name="size">Size for conversion.</param>
        /// <returns>Converted size.</returns>
        public SizeM ConvertToHInches(SizeM size)
        {
            return new SizeM(
                ConvertToHInches(size.Width),
                ConvertToHInches(size.Height));
        }

		
		/// <summary>
		/// Converts a size from hundredths of inch into units of this class.
		/// </summary>
		/// <param name="size">Size for conversion.</param>
		/// <returns>Converted size.</returns>
		public SizeD ConvertFromHInches(SizeD size)
		{
			return new SizeD(
				ConvertFromHInches(size.Width),
				ConvertFromHInches(size.Height));
        }

        /// <summary>
        /// Converts a size from hundredths of inch into units of this class.
        /// </summary>
        /// <param name="size">Size for conversion.</param>
        /// <returns>Converted size.</returns>
        public SizeM ConvertFromHInches(SizeM size)
        {
            return new SizeM(
                ConvertFromHInches(size.Width),
                ConvertFromHInches(size.Height));
        }
        #endregion
    }
}