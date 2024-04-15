#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports 									            }
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft


namespace Stimulsoft.Report.Components.TextFormats
{
    public static class StiNegativeColorChecker
    {
        public static bool IsNegativeInRed(StiFormatService format)
        {
            if (format == null)
                return false;

            if (format is StiNumberFormatService &&
                (((StiNumberFormatService)format).State & StiTextFormatState.NegativeInRed) > 0)
            {
                return true;
            }

            if (format is StiCurrencyFormatService &&
                (((StiCurrencyFormatService)format).State & StiTextFormatState.NegativeInRed) > 0)
            {
                return true;
            }

            if (format is StiPercentageFormatService &&
                (((StiPercentageFormatService)format).State & StiTextFormatState.NegativeInRed) > 0)
            {
                return true;
            }

            return false;
        }
    }
}
