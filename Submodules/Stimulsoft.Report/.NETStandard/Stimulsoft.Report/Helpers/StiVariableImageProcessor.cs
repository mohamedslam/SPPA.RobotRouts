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

using Stimulsoft.Base.Drawing;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Helpers
{
    public static class StiVariableImageProcessor
    {
        #region Methods
        public static byte[] GetImage(StiReport report, string expression)
        {
            if (string.IsNullOrWhiteSpace(expression)) return null;

            var variableName = GetVariableNameFromExpression(expression);
            if (variableName == null) return null;
            var variable = report.Dictionary.Variables[variableName];

            if (variable != null)
            {
                if (variable.ValueObject is Image)
                    return StiImageConverter.ImageToBytes(variable.ValueObject as Image);

                if (variable.ValueObject is byte[] && StiImageHelper.IsImage(variable.ValueObject as byte[]))
                    return variable.ValueObject as byte[];
            }
            return null;
        }

        public static string GetVariableNameFromExpression(string expression)
        {
            if (expression == null) return null;

            var exp = expression.Trim();
            if (!(exp.StartsWith("{") && exp.EndsWith("}"))) return null;
            return exp.Substring(1, exp.Length - 2);
        }
        #endregion
    }
}
