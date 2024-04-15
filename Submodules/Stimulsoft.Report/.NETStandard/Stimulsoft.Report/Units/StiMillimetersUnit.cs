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

using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Units
{
	/// <summary>
	/// Report units - millimeters.
	/// </summary>
	public class StiMillimetersUnit : StiUnit
	{
        #region Properties
        public override string ToString()
		{
			return Loc.Get("PropertyEnum", "StiReportUnitTypeMillimeters");
		}

		/// <summary>
		/// Gets ruler step.
		/// </summary>
		public override double RollerStep => 10;

	    /// <summary>
		/// Gets the ruler scale in hundredths of inch on the one step.
		/// </summary>
		public override double Factor => 100d / 2.54d;

	    /// <summary>
		/// Gets the shor unit name.
		/// </summary>
		public override string ShortName => "mm";
	    #endregion

        #region Methods
        /// <summary>
		/// Converts a value from hundredths of inch into units of this class.
		/// </summary>
		/// <param name="value">Value for conversion.</param>
		/// <returns>Converted value.</returns>
		public override double ConvertToHInches(double value)
		{
			return value * 10d / 2.54d;
		}

        /// <summary>
        /// Converts a value from hundredths of inch into units of this class.
        /// </summary>
        /// <param name="value">Value for conversion.</param>
        /// <returns>Converted value.</returns>
        public override decimal ConvertToHInches(decimal value)
        {
            return value * 10m / 2.54m;
        }

		/// <summary>
		///  Converts a value from hundredths of inch into units of this class.
		/// </summary>
		/// <param name="value">Value for conversion.</param>
		/// <returns>Converted value.</returns>
		public override double ConvertFromHInches(double value)
		{
			return value * 2.54d / 10d;
        }

        /// <summary>
        ///  Converts a value from hundredths of inch into units of this class.
        /// </summary>
        /// <param name="value">Value for conversion.</param>
        /// <returns>Converted value.</returns>
        public override decimal ConvertFromHInches(decimal value)
        {
            return value * 2.54m / 10m;
        }
        #endregion
    }
}
