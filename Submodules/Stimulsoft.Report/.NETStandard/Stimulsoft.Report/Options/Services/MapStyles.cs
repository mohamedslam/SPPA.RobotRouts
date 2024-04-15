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

using System.Collections.Generic;
using Stimulsoft.Report.Maps;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        public sealed partial class Services
        {
            private static List<StiMapStyleFX> mapStyles;
            public static List<StiMapStyleFX> MapStyles
            {
                get
                {
                    lock (lockObject)
                    {
                        return mapStyles ?? (mapStyles = new List<StiMapStyleFX>
                        {
                            new StiMap21StyleFX(),
                            new StiMap24StyleFX(),
                            new StiMap25StyleFX(),
                            new StiMap26StyleFX(),
                            new StiMap27StyleFX(),
                            new StiMap28StyleFX(),
                            new StiMap29StyleFX(),
                            new StiMap30StyleFX(),
                            new StiMap31StyleFX(),
                            new StiMap32StyleFX(),
                            new StiMap33StyleFX(),
                            new StiMap34StyleFX(),
                            new StiMap35StyleFX()
                        });
                    }
                }
            }
        }
	}
}