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

namespace Stimulsoft.Base.Helpers
{
    internal static class StiBuildHelper
    {
        internal static bool DecodeVersion(string version, out int major, out int minor, out int build)
        {
            major = 0;
            minor = 0;
            build = 0;

            if (string.IsNullOrWhiteSpace(version)) return false;
            var strs = version.Split('.');
            if (strs.Length < 3) return false;

            if (!int.TryParse(strs[0], out major)) return false;
            if (!int.TryParse(strs[1], out minor)) return false;
            if (!int.TryParse(strs[2], out build)) return false;

            return true;
        }
    }
}
