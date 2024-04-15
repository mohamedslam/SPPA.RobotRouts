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

namespace Stimulsoft.Report.Components
{
    public static class StiCrossLinePrimitiveHelper
    {
        #region Methods
        public static void RemoveIncorrectLinesFromContainer(StiComponentsCollection comps)
        {
            int index = 0;
            while (index < comps.Count)
            {
                var crossPrimitive = comps[index] as StiCrossLinePrimitive;
                if (crossPrimitive != null)
                {
                    crossPrimitive.StoredStartPoint = null;
                    crossPrimitive.StoredEndPoint = null;

                    crossPrimitive.GetStartPoint();
                    crossPrimitive.GetEndPoint();

                    if (crossPrimitive.StoredStartPoint == null || crossPrimitive.StoredEndPoint == null)
                    {
                        comps.RemoveAt(index);

                        if (crossPrimitive.StoredStartPoint != null && crossPrimitive.StoredStartPoint.Parent != null)
                            crossPrimitive.StoredStartPoint.Parent.Components.Remove(crossPrimitive.StoredStartPoint);

                        if (crossPrimitive.StoredEndPoint != null && crossPrimitive.StoredEndPoint.Parent != null)
                            crossPrimitive.StoredEndPoint.Parent.Components.Remove(crossPrimitive.StoredEndPoint);

                        continue;
                    }
                }
                index++;
            }
        }
        #endregion
    }
}
