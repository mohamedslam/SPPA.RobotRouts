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

using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiFixCrossLinePrimitiveAction : StiAction
    {
        public override string Name => StiLocalizationExt.Get("CheckActions", "Fix");

        public override string Description => StiLocalizationExt.Get("CheckActions", "StiFixCrossLinePrimitiveActionLong");

        public override void Invoke(StiReport report, object element, string elementName)
        {
            base.Invoke(report, null, null);

            var line = element as StiCrossLinePrimitive;
            if (line == null)return;

            var startPoint = line.GetStartPoint(line.Page);
            var endPoint = line.GetEndPoint(line.Page);

            var left = line.Left;
            var top = line.Top;
            var bottom = line.Bottom;
            var right = line.Right;

            if (startPoint == null)
            {
                startPoint = new StiStartPointPrimitive();
                startPoint.ReferenceToGuid = line.Guid;
                startPoint.Left = left;
                startPoint.Top = top;
                startPoint.Name = StiNameCreation.CreateName(report, StiNameCreation.GenerateName(startPoint), true, true, true);
                line.Page.Components.Add(startPoint);
            }

            if (endPoint == null)
            {
                endPoint = new StiEndPointPrimitive();
                endPoint.ReferenceToGuid = line.Guid;
                endPoint.Left = right;
                endPoint.Top = bottom;
                endPoint.Name = StiNameCreation.CreateName(report, StiNameCreation.GenerateName(endPoint), true, true, true);
                line.Page.Components.Add(endPoint);
            }

            line.Page.Correct();
        }
    }
}
