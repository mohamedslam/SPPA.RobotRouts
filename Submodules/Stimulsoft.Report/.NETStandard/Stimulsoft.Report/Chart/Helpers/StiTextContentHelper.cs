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


using Stimulsoft.Base.Context;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Chart
{
    internal static class StiTextContentHelper
    {
        #region Methods
        internal static string GetMeasureText(StiContext context, string text, Font font, double maxWidth)
        {
            return GetMeasureText(context, text, new StiFontGeom(font), maxWidth);
        }

        internal static string GetMeasureText(StiContext context, string text, StiFontGeom font, double maxWidth)
        {
            for (var index = 0; text.Length > index; index++)
            {
                var currentText = text.Substring(0, index);

                if (maxWidth < context.MeasureString(currentText, font).Width)
                {
                    if (currentText.Length > 4)
                    {
                        return $"{currentText.Substring(0, currentText.Length - 3)}...";
                    }
                    else if (currentText.Length > 1)
                    {
                        return $"{currentText.Substring(0, 1)}...";
                    }
                }
            }

            return text;
        } 
        #endregion
    }
}
