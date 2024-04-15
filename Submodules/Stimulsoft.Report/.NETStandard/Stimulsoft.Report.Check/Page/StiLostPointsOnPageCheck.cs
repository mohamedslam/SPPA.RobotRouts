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
using System.Text;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiLostPointsOnPageCheck : StiPageCheck
    {
        #region Properties
        public override bool PreviewVisible => false;

        public override string ShortMessage => 
            string.Format(StiLocalizationExt.Get("CheckPage", "StiLostPointsOnPageCheckShort"), this.ElementName, this.LostPointsNames);

        public override string LongMessage => 
            string.Format(StiLocalizationExt.Get("CheckPage", "StiLostPointsOnPageCheckLong"), this.ElementName, this.LostPointsNames);

        public string LostPointsNames { get; set; } = string.Empty;

        public override StiCheckStatus Status => StiCheckStatus.Warning;
        #endregion

        #region Methods
        public static List<StiPointPrimitive> GetLostPointsOnPage(StiPage page)
        {
            StiComponentsCollection comps = page.GetComponents();
            Dictionary<StiPointPrimitive, StiPointPrimitive> points = new Dictionary<StiPointPrimitive, StiPointPrimitive>();
            List<StiCrossLinePrimitive> lines = new List<StiCrossLinePrimitive>();

            foreach (StiComponent comp in comps)
            {
                if (comp is StiPointPrimitive)
                    points.Add(comp as StiPointPrimitive, comp as StiPointPrimitive);
                else if (comp is StiCrossLinePrimitive)
                    lines.Add(comp as StiCrossLinePrimitive);
            }

            foreach (StiCrossLinePrimitive line in lines)
            {
                StiStartPointPrimitive startPoint = line.GetStartPoint(line.Page);
                StiEndPointPrimitive endPoint = line.GetEndPoint(line.Page);

                if (startPoint != null && points.ContainsKey(startPoint))
                {
                    points.Remove(startPoint);
                }

                if (endPoint != null && points.ContainsKey(endPoint))
                {
                    points.Remove(endPoint);
                }
            }

            List <StiPointPrimitive>lostPoints = new List<StiPointPrimitive>();
            foreach (StiPointPrimitive point in points.Values)
            {
                lostPoints.Add(point);
            }
            return lostPoints;
        }

        public override object ProcessCheck(StiReport report, object obj)
        {
            StiPage page = obj as StiPage;

            List<StiPointPrimitive> points = GetLostPointsOnPage(page);

            if (points != null && points.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (StiPointPrimitive point in points)
                {
                    if (sb.Length > 0)
                        sb = sb.Append(", ");
                    sb = sb.Append(point.Name);
                }

                StiLostPointsOnPageCheck check = new StiLostPointsOnPageCheck();
                check.Element = obj;
                check.LostPointsNames = sb.ToString();
                check.Actions.Add(new StiDeleteLostPointsAction());
                return check;
            }
            else return null;
        }
        #endregion
    }
}