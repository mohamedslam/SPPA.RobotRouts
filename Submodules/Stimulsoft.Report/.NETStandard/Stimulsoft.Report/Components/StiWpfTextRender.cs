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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Painters;
using System;

namespace Stimulsoft.Report.Components
{
    public static class StiWpfTextRender
    {
        public static SizeD MeasureString(double width, StiText textBox)
        {
            var actualSize = StiWpfPainter.MeasureComponent(width, textBox);

            if (textBox.Angle == 90 || textBox.Angle == 180)
                actualSize.Width *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorWpf;
            else
                actualSize.Height *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorWpf;

            return actualSize;
        }

        public static SizeD MeasureRtfString(double width, StiRichText textBox)
        {
            return StiWpfPainter.MeasureComponent(width, textBox);
        }

        public static string BreakText(RectangleD rect, ref string text, StiText textComp)
        {
            var type = Type.GetType($"Stimulsoft.Report.Wpf.StiWpfBreakTextHelper, Stimulsoft.Report.Wpf, {StiVersion.VersionInfo}");
            if (type == null)
                throw new Exception("Assembly 'Stimulsoft.Report.Wpf' is not found");

            var helper = StiActivator.CreateObject(type, new object[0]) as IStiWpfBreakTextHelper;
            return helper.BreakText(rect, ref text, textComp);
        }
    }
}
