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
	/// Report units - hundredths of inch.
	/// </summary>
	public class StiHundredthsOfInchUnit : StiUnit
    {
        #region Properties
        /// <summary>
		/// Gets ruler step.
		/// </summary>
		public override double RollerStep => 100;

        /// <summary>
		/// Gets the ruler scale in hundredths of inch on the one step.
		/// </summary>
		public override double Factor => 100;

        /// <summary>
		/// Gets the shor unit name.
		/// </summary>
		public override string ShortName => "hi";
        #endregion

        #region Methods
        public override string ToString()
        {
            return Loc.Get("PropertyEnum", "StiReportUnitTypeHundredthsOfInch");
        }

		/// <summary>
		/// Converts a value from hundredths of inch into units of this class.
		/// </summary>
		/// <param name="value">Value for conversion.</param>
		/// <returns>Converted value.</returns>
		public override double ConvertToHInches(double value)
		{
			return value;
		}

        /// <summary>
        /// Converts a value from hundredths of inch into units of this class.
        /// </summary>
        /// <param name="value">Value for conversion.</param>
        /// <returns>Converted value.</returns>
        public override decimal ConvertToHInches(decimal value)
        {
            return value;
        }

		/// <summary>
		///  Converts a value from hundredths of inch into units of this class.
		/// </summary>
		/// <param name="value">Value for conversion.</param>
		/// <returns>Converted value.</returns>
		public override double ConvertFromHInches(double value)
		{
			return value;
        }

        /// <summary>
        ///  Converts a value from hundredths of inch into units of this class.
        /// </summary>
        /// <param name="value">Value for conversion.</param>
        /// <returns>Converted value.</returns>
        public override decimal ConvertFromHInches(decimal value)
        {
            return value;
        }
        #endregion
    }
}