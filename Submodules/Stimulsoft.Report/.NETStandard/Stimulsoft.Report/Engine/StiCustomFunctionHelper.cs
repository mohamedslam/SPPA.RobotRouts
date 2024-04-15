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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Stimulsoft.Report.Engine
{
    public class StiCustomFunctionHelper
    {
        #region Fields
        private static CultureInfo enUsCulture = new CultureInfo("en-US");
        #endregion

        #region Methods
        internal static string CheckExceptionForCustomFunction(Exception ex, StiReport report, bool full = true)
        {
            string st = null;
            if (ex is StiCustomFunctionException)
            {
                st = (ex as StiCustomFunctionException).FunctionName;
            }
            else
            {
                bool isCaseSensitive = report == null || report.ScriptLanguage != StiReportLanguageType.VB;
                st = CheckExceptionStackForCustomFunction(ex, isCaseSensitive);
                if (st == null) st = CheckExceptionStackForCustomFunction(ex.InnerException, isCaseSensitive);
            }
            if (st != null)
            {
                if (full)
                {
                    return $"Exception at function {st}: ";
                }
                return $", exception at function {st}";
            }
            return null;
        }

        private static string CheckExceptionStackForCustomFunction(Exception ex, bool isCaseSensitive)
        {
            if (ex == null || string.IsNullOrWhiteSpace(ex.StackTrace)) return null;

            global::System.Globalization.CultureInfo storedCulture = global::System.Threading.Thread.CurrentThread.CurrentUICulture;
            try
            {
                global::System.Threading.Thread.CurrentThread.CurrentUICulture = enUsCulture;
                string stack = ex.StackTrace;

                int posAt = stack.IndexOf(" at ", StringComparison.InvariantCulture);
                int posIn = stack.IndexOf(" in ", StringComparison.InvariantCulture);
                if (posAt < 0 || posIn < 0) return null;

                string st = stack.Substring(posAt + 4, posIn - posAt - 4).Trim();

                if (st.StartsWith("Stimulsoft.") || st.StartsWith("System.")) return null;

                int posBracket = st.IndexOf("(", StringComparison.InvariantCulture);
                if (posBracket < 0) return null;
                int posDot = st.LastIndexOf(".", posBracket);
                if (posDot < 0) return null;

                string lastName = st.Substring(posDot + 1, posBracket - posDot - 1);

                if (isCaseSensitive)
                {
                    if (StiAppFunctions.FunctionsToCompile.ContainsKey(lastName))
                        return lastName;
                }
                else
                {
                    if (StiAppFunctions.FunctionsToCompileLower.ContainsKey(lastName.ToLowerInvariant()))
                        return lastName;
                }
            }
            catch
            {
            }
            finally
            {
                global::System.Threading.Thread.CurrentThread.CurrentUICulture = storedCulture;
            }
            return null;
        }
        #endregion
    }
}
