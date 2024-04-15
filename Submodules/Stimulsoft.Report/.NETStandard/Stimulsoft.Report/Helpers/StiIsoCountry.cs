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

namespace Stimulsoft.Report.Helpers
{
    /// <summary>
    /// Representation of an ISO3166-1 Country or State
    /// </summary>
    public class StiIsoCountry
    {
        #region Properties
        public string[] Names { get; set; }

        public string[] RuNames { get; set; }

        public string[] FrNames { get; set; }

        public string Alpha2 { get; set; }

        public string Alpha3 { get; set; }
        #endregion

        #region Methods
        internal StiIsoCountry Ru(params string[] names)
        {
            this.RuNames = names;

            return this;
        }

        internal StiIsoCountry Fr(params string[] names)
        {
            this.FrNames = names;

            return this;
        }

        internal StiIsoCountry Iso(string alpha2, string alpha3 = null)
        {
            this.Alpha2 = alpha2;
            this.Alpha3 = alpha3;

            return this;
        }
        #endregion

        public StiIsoCountry(params string[] names)
        {
            this.Names = names;
        }
    }
}