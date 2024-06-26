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

namespace Stimulsoft.Base.Localization
{          
	public class Loc
	{
        #region Methods
        public static string Get(string category, string key)
	    {
	        return StiLocalization.Get(category, key);
	    }

        public static string GetCleaned(string category, string key)
        {
            return Get(category, key).Replace(":", "").Replace("&", "").Replace("...", "");
        }

        public static string GetMain(string key)
	    {
	        return StiLocalization.Get("PropertyMain", key);
	    }

        public static string GetReport(string key)
        {
            return StiLocalization.Get("Report", key);
        }

        public static string GetEnum(string key)
	    {
	        return StiLocalization.Get("PropertyEnum", key);
	    }

        public static string GetNotice(string key)
        {
            return StiLocalization.Get("Notices", key);
        }
        #endregion

        #region Properties
        public static bool IsEn => StiLocalization.IsEn;

        public static bool IsRussian => StiLocalization.IsRussian;

        public static bool IsCyrillic => StiLocalization.IsCyrillic;
        #endregion
    }
}