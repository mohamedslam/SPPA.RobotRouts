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

namespace Stimulsoft.Base
{
	public static class StiAppKey
    {
        public static string GetOrGeneratedKey(IStiReportComponent component)
        {
            var app = component?.GetApp();
            return GetOrGeneratedKey(app);
        }

        public static string GetOrGeneratedKey(IStiApp app)
        {
            app?.SetKey(StiKeyHelper.GetOrGeneratedKey(app.GetKey()));
            return app?.GetKey();
        }

        public static string GetOrGeneratedKey(IStiAppDictionary dictionary)
        {
            var app = dictionary?.GetApp();
            return GetOrGeneratedKey(app);
        }

        public static string GetOrGeneratedKey(IStiAppDataSource dataSource)
        {
            var app = dataSource?.GetDictionary();
            return GetOrGeneratedKey(app);
        }
    }
}
