#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.ShapeTypes;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Export;
using System.Collections;
using System.Drawing.Imaging;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiShapeElementHelper
    {
        #region Fields
        private IStiShapeElement shapeElement;
        #endregion

        #region Helper Methods
        internal static StiComponent CreateShapeElement(string componentTypeArray)
        {
            var shapeElement = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.Shape.StiShapeElement") as IStiShapeElement;
            if (shapeElement != null)
            {
                string[] compAttributes = componentTypeArray.Split(';');
                string shapeType = compAttributes[1];
                SetShapeTypeProperty(shapeElement as IStiShapeElement, shapeType);
            }

            return shapeElement as StiComponent;
        }

        internal static void SetShapeTypeProperty(IStiShapeElement comp, object propValue)
        {
            if (comp == null) return;
            string shapeType = propValue as string;

            if (shapeType.StartsWith("StiArrowShapeType"))
            {
                StiArrowShapeType arrowShapeType = new StiArrowShapeType();
                switch (shapeType)
                {
                    case "StiArrowShapeTypeUp": arrowShapeType.Direction = StiShapeDirection.Up; break;
                    case "StiArrowShapeTypeDown": arrowShapeType.Direction = StiShapeDirection.Down; break;
                    case "StiArrowShapeTypeRight": arrowShapeType.Direction = StiShapeDirection.Right; break;
                    case "StiArrowShapeTypeLeft": arrowShapeType.Direction = StiShapeDirection.Left; break;
                }
                comp.ShapeType = arrowShapeType;
            }
            else if (shapeType == "StiDiagonalDownLineShapeType") comp.ShapeType = new StiDiagonalDownLineShapeType();
            else if (shapeType == "StiDiagonalUpLineShapeType") comp.ShapeType = new StiDiagonalUpLineShapeType();
            else if (shapeType == "StiHorizontalLineShapeType") comp.ShapeType = new StiHorizontalLineShapeType();
            else if (shapeType == "StiLeftAndRightLineShapeType") comp.ShapeType = new StiLeftAndRightLineShapeType();
            else if (shapeType == "StiOvalShapeType") comp.ShapeType = new StiOvalShapeType();
            else if (shapeType == "StiRectangleShapeType") comp.ShapeType = new StiRectangleShapeType();
            else if (shapeType == "StiRoundedRectangleShapeType") comp.ShapeType = new StiRoundedRectangleShapeType();
            else if (shapeType == "StiTopAndBottomLineShapeType") comp.ShapeType = new StiTopAndBottomLineShapeType();
            else if (shapeType == "StiTriangleShapeType") comp.ShapeType = new StiTriangleShapeType();
            else if (shapeType == "StiVerticalLineShapeType") comp.ShapeType = new StiVerticalLineShapeType();
            else if (shapeType == "StiComplexArrowShapeType") comp.ShapeType = new StiComplexArrowShapeType();
            else if (shapeType == "StiBentArrowShapeType") comp.ShapeType = new StiBentArrowShapeType();
            else if (shapeType == "StiChevronShapeType") comp.ShapeType = new StiChevronShapeType();
            else if (shapeType == "StiDivisionShapeType") comp.ShapeType = new StiDivisionShapeType();
            else if (shapeType == "StiEqualShapeType") comp.ShapeType = new StiEqualShapeType();
            else if (shapeType == "StiFlowchartCardShapeType") comp.ShapeType = new StiFlowchartCardShapeType();
            else if (shapeType == "StiFlowchartCollateShapeType") comp.ShapeType = new StiFlowchartCollateShapeType();
            else if (shapeType == "StiFlowchartDecisionShapeType") comp.ShapeType = new StiFlowchartDecisionShapeType();
            else if (shapeType == "StiFlowchartManualInputShapeType") comp.ShapeType = new StiFlowchartManualInputShapeType();
            else if (shapeType == "StiFlowchartOffPageConnectorShapeType") comp.ShapeType = new StiFlowchartOffPageConnectorShapeType();
            else if (shapeType == "StiFlowchartPreparationShapeType") comp.ShapeType = new StiFlowchartPreparationShapeType();
            else if (shapeType == "StiFlowchartSortShapeType") comp.ShapeType = new StiFlowchartSortShapeType();
            else if (shapeType == "StiFrameShapeType") comp.ShapeType = new StiFrameShapeType();
            else if (shapeType == "StiMinusShapeType") comp.ShapeType = new StiMinusShapeType();
            else if (shapeType == "StiMultiplyShapeType") comp.ShapeType = new StiMultiplyShapeType();
            else if (shapeType == "StiParallelogramShapeType") comp.ShapeType = new StiParallelogramShapeType();
            else if (shapeType == "StiPlusShapeType") comp.ShapeType = new StiPlusShapeType();
            else if (shapeType == "StiRegularPentagonShapeType") comp.ShapeType = new StiRegularPentagonShapeType();
            else if (shapeType == "StiTrapezoidShapeType") comp.ShapeType = new StiTrapezoidShapeType();
            else if (shapeType == "StiOctagonShapeType") comp.ShapeType = new StiOctagonShapeType();
            else if (shapeType == "StiSnipSameSideCornerRectangleShapeType") comp.ShapeType = new StiSnipSameSideCornerRectangleShapeType();
            else if (shapeType == "StiSnipDiagonalSideCornerRectangleShapeType") comp.ShapeType = new StiSnipDiagonalSideCornerRectangleShapeType();
        }

        public static string GetShapeTypeProperty(IStiShapeElement comp)
        {

            if (comp.ShapeType is StiArrowShapeType)
            {
                StiArrowShapeType arrowShapeType = new StiArrowShapeType();
                switch (((StiArrowShapeType)comp.ShapeType).Direction)
                {
                    case StiShapeDirection.Up: return "StiArrowShapeTypeUp";
                    case StiShapeDirection.Down: return "StiArrowShapeTypeDown";
                    case StiShapeDirection.Right: return "StiArrowShapeTypeRight";
                    case StiShapeDirection.Left: return "StiArrowShapeTypeLeft";
                }
            }

            return comp.ShapeType.GetType().Name;
        }

        private Hashtable GetShapeElementJSProperties(Hashtable parameters)
        {
            Hashtable properties = StiReportEdit.GetAllProperties(shapeElement as StiComponent);

            if ((string)parameters["command"] != "GetShapeElementProperties")
            {
                properties["svgContent"] = StiEncodingHelper.Encode(StiDashboardsSvgHelper.SaveElementToString(shapeElement, 1, 1, true));
            }

            return properties;
        }

        private void SetPropertyValue(Hashtable parameters, Hashtable callbackResult)
        {
            var propertyName = parameters["propertyName"] as string;
            var propertyValue = parameters["propertyValue"] as string;

            switch (propertyName)
            {
                case "ShapeType":
                    {
                        SetShapeTypeProperty(shapeElement, propertyValue);
                        break;
                    }
                case "Size":
                case "Stroke":
                case "Fill":
                    {
                        StiReportEdit.SetPropertyValue(shapeElement.Report, propertyName, shapeElement, propertyValue);
                        break;
                    }
            }
        }
        #endregion

        #region Methods
        public void ExecuteJSCommand(Hashtable parameters, Hashtable callbackResult)
        {
            switch ((string)parameters["command"])
            {
                case "SetPropertyValue":
                    {
                        SetPropertyValue(parameters, callbackResult);
                        break;
                    }
                case "GetShapeElementProperties":
                    {
                        break;
                    }
            }

            callbackResult["shapeElement"] = GetShapeElementJSProperties(parameters);
        }
        #endregion

        #region Constructor
        public StiShapeElementHelper(IStiShapeElement shapeElement)
        {
            this.shapeElement = shapeElement;
        }
        #endregion   
    }
}
