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
using System.Linq;
using Stimulsoft.Base;

namespace Stimulsoft.Report.Dictionary
{
	public static class StiVariableAsParameterHelper
    {
        public static bool ParameterExists(List<StiToken> tokens, string name)
        {
            name = name.ToLowerInvariant();

            for (var index = 1; index < tokens.Count; index++)
            {
                var prevToken = tokens[index - 1];
                var token = tokens[index];

                if (prevToken.Type == StiTokenType.Ampersand && token.Type == StiTokenType.Ident &&
                    token.Data is string && token.Data.ToString() == name) return true;
            }
            return false;
        }

	    public static List<StiVariable> FetchAll(string query, List<StiDataParameter> parameters, StiReport report)
	    {
	        query = query.ToLowerInvariant();
	        var tokens = StiLexer.GetAllTokens(query.ToLowerInvariant());

	        return report.Dictionary.Variables.ToList().Where(v => v.AllowUseAsSqlParameter && ParameterExists(tokens, v.Name) && !ParameterExists(parameters, v.Name)).ToList();
	    }

	    private static bool ParameterExists(List<StiDataParameter> parameters, string name)
	    {
	        name = name.ToLowerInvariant();
	        return parameters.Any(p => p.Name.ToLowerInvariant() == name || p.Name.ToLowerInvariant() == "@" + name);
	    }
	}
}
