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
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Stimulsoft.Report.Helpers
{
    public static class StiUniversalDataLoader
    {
        public static List<StiDataLoaderHelper.Data> LoadMutiple(StiReport report, string path, string filter)
        {
            if (string.IsNullOrEmpty(path)) return null;

            if (StiHyperlinkProcessor.IsResourceHyperlink(path))
            {
                return new StiDataLoaderHelper.Data(
                    StiHyperlinkProcessor.GetResourceNameFromHyperlink(path),
                    StiHyperlinkProcessor.GetBytes(report, path)).ToList();
            }
            
            return StiDataLoaderHelper.LoadMultiple(path, filter);
        }

        public static StiDataLoaderHelper.Data LoadSingle(StiReport report, string path, NameValueCollection headers = null)
        {
            if (string.IsNullOrEmpty(path)) return null;

            if (StiHyperlinkProcessor.IsResourceHyperlink(path))
            {
                return new StiDataLoaderHelper.Data(
                    StiHyperlinkProcessor.GetResourceNameFromHyperlink(path),
                    StiHyperlinkProcessor.GetBytes(report, path));
            }

            return StiDataLoaderHelper.LoadSingle(path, report?.CookieContainer, headers);
        }
    }
}
