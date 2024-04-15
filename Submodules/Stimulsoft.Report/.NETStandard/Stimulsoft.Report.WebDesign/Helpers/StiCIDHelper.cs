#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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

using Stimulsoft.Base;
using System.Collections;

namespace Stimulsoft.Report.Web
{
    internal class StiCIDHelper
    {
        #region Consts
        private const string Key = "fjk2dpfko5epefko4prmk";
        private const string Prefix = "PL534950";
        #endregion

        internal static void EncryptMachineName(Hashtable param, Hashtable callbackResult)
        {
            callbackResult["machineName"] = Prefix + StiEncryption.Encrypt(new StiCID(param["countryName"] as string, param["ipAddress"] as string, "OnlineDesigner").SaveToString(), Key);
        }
    }
}