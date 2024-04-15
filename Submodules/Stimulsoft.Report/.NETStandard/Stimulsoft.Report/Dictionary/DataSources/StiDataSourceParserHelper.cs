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

using System;

using Stimulsoft.Report.Components;
using Stimulsoft.Report.Engine;

namespace Stimulsoft.Report.Dictionary
{
    public static class StiDataSourceParserHelper
	{
        public static void ConnectSqlSource(StiSqlSource sqlSource)
        {
            var tempText = new StiText
            {
                Name = "**DataSourceParameter**",
                Page = sqlSource.Dictionary?.Report?.Pages[0]
            };

            foreach (StiDataParameter param in sqlSource.Parameters)
            {
                param.ParameterValue = StiParser.ParseTextValue("{" + param.Value + "}", tempText);
            }

            sqlSource.SqlCommand = Convert.ToString(StiParser.ParseTextValue(sqlSource.SqlCommand, tempText));
        }
    }
}
