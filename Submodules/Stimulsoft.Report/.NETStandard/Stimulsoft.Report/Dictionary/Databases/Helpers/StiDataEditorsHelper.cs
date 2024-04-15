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

using Stimulsoft.Base;
using System;

namespace Stimulsoft.Report.Dictionary
{
    internal static class StiDataEditorsHelper
    {
        internal static IStiDataEditors Get()
        {
            Type dataEditorsType;
            if (StiOptions.Configuration.IsWPF)
            {
                dataEditorsType = Type.GetType($"Stimulsoft.Client.Designer.Dictionary.StiDataEditors, Stimulsoft.Client.Designer, {StiVersion.VersionInfo}");
                if (dataEditorsType != null)
                    return StiActivator.CreateObject(dataEditorsType, new object[0]) as IStiDataEditors;
            }

            dataEditorsType = Type.GetType($"Stimulsoft.Report.Dictionary.Design.StiDataEditors, Stimulsoft.Report.Design, {StiVersion.VersionInfo}");
            return StiActivator.CreateObject(dataEditorsType, new object[0]) as IStiDataEditors;
        }
    }
}
