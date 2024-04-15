
StiMobileDesigner.prototype.GetPageOrientationItems = function (showImage) {
    var items = [];
    items.push(this.Item("portrait", this.loc.FormPageSetup.PageOrientationPortrait, showImage ? "Portrait.png" : null, "Portrait"));
    items.push(this.Item("landscape", this.loc.FormPageSetup.PageOrientationLandscape, showImage ? "Landscape.png" : null, "Landscape"));

    return items;
}

StiMobileDesigner.prototype.GetUnitItems = function (showImage) {
    var items = [];
    items.push(this.Item("cm", this.loc.PropertyEnum.StiReportUnitTypeCentimeters, null, "cm"));
    items.push(this.Item("hi", this.loc.PropertyEnum.StiReportUnitTypeHundredthsOfInch, null, "hi"));
    items.push(this.Item("in", this.loc.PropertyEnum.StiReportUnitTypeInches, null, "in"));
    items.push(this.Item("mm", this.loc.PropertyEnum.StiReportUnitTypeMillimeters, null, "mm"));

    return items;
}

StiMobileDesigner.prototype.GetFilterIngineItems = function () {
    var items = [];
    items.push(this.Item("ReportEngine", this.loc.PropertyEnum.StiFilterEngineReportEngine, null, "ReportEngine"));
    items.push(this.Item("SQLQuery", this.loc.PropertyEnum.StiFilterEngineSQLQuery, null, "SQLQuery"));

    return items
}

StiMobileDesigner.prototype.GetBorderStyleItems = function () {
    var items = [];
    items.push(this.Item("borderStyleSolid", this.loc.PropertyEnum.StiPenStyleSolid, "BorderStyleSolid.png", "0"));
    items.push(this.Item("borderStyleDash", this.loc.PropertyEnum.StiPenStyleDash, "BorderStyleDash.png", "1"));
    items.push(this.Item("borderStyleDashDot", this.loc.PropertyEnum.StiPenStyleDashDot, "BorderStyleDashDot.png", "2"));
    items.push(this.Item("borderStyleDashDotDot", this.loc.PropertyEnum.StiPenStyleDashDotDot, "BorderStyleDashDotDot.png", "3"));
    items.push(this.Item("borderStyleDot", this.loc.PropertyEnum.StiPenStyleDot, "BorderStyleDot.png", "4"));
    items.push(this.Item("borderStyleDouble", this.loc.PropertyEnum.StiPenStyleDouble, "BorderStyleDouble.png", "5"));
    items.push(this.Item("borderStyleNone", this.loc.PropertyEnum.StiPenStyleNone, "BorderStyleNone.png", "6"));

    return items;
}

StiMobileDesigner.prototype.GetImageRotationItems = function () {
    var items = [];
    items.push(this.Item("itemNone", this.loc.PropertyEnum.StiImageRotationNone, null, "None"));
    items.push(this.Item("itemRotate90CW", this.loc.PropertyEnum.StiImageRotationRotate90CW, null, "Rotate90CW"));
    items.push(this.Item("itemRotate90CCW", this.loc.PropertyEnum.StiImageRotationRotate90CCW, null, "Rotate90CCW"));
    items.push(this.Item("itemRotate180", this.loc.PropertyEnum.StiImageRotationRotate180, null, "Rotate180"));
    items.push(this.Item("itemFlipHorizontal", this.loc.PropertyEnum.StiImageRotationFlipHorizontal, null, "FlipHorizontal"));
    items.push(this.Item("itemFlipVertical", this.loc.PropertyEnum.StiImageRotationFlipVertical, null, "FlipVertical"));

    return items;
}

StiMobileDesigner.prototype.GetBarCodeAngleItems = function () {
    var items = [];

    items.push(this.Item("angle0", this.loc.PropertyEnum.StiAngleAngle0, null, "Angle0"));
    items.push(this.Item("angle90", this.loc.PropertyEnum.StiAngleAngle90, null, "Angle90"));
    items.push(this.Item("angle180", this.loc.PropertyEnum.StiAngleAngle180, null, "Angle180"));
    items.push(this.Item("angle270", this.loc.PropertyEnum.StiAngleAngle270, null, "Angle270"));

    return items;
}

StiMobileDesigner.prototype.GetTextAngleItems = function () {
    var items = [];

    items.push(this.Item("angle0", this.loc.PropertyEnum.StiAngleAngle0, null, "0"));
    items.push(this.Item("angle45", this.loc.PropertyEnum.StiAngleAngle45, null, "45"));
    items.push(this.Item("angle90", this.loc.PropertyEnum.StiAngleAngle90, null, "90"));
    items.push(this.Item("angle180", this.loc.PropertyEnum.StiAngleAngle180, null, "180"));
    items.push(this.Item("angle270", this.loc.PropertyEnum.StiAngleAngle270, null, "270"));

    return items;
}

StiMobileDesigner.prototype.GetSortDirectionItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiSortDirectionAsc, null, "0"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiSortDirectionDesc, null, "1"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiSortDirectionNone, null, "2"));

    return items;
}

StiMobileDesigner.prototype.GetPrintOnItems = function () {
    var items = [];
    items.push(this.Item("itemAllPagesItem", this.loc.PropertyEnum.StiPrintOnTypeAllPages, null, "AllPages"));
    items.push(this.Item("itemExceptFirstPage", this.loc.PropertyEnum.StiPrintOnTypeExceptFirstPage, null, "ExceptFirstPage"));
    items.push(this.Item("itemExceptLastPage", this.loc.PropertyEnum.StiPrintOnTypeExceptLastPage, null, "ExceptLastPage"));
    items.push(this.Item("itemExceptFirstAndLastPage", this.loc.PropertyEnum.StiPrintOnTypeExceptFirstAndLastPage, null, "ExceptFirstAndLastPage"));
    items.push(this.Item("itemOnlyFirstPage", this.loc.PropertyEnum.StiPrintOnTypeOnlyFirstPage, null, "OnlyFirstPage"));
    items.push(this.Item("itemOnlyLastPage", this.loc.PropertyEnum.StiPrintOnTypeOnlyLastPage, null, "OnlyLastPage"));
    items.push(this.Item("itemOnlyFirstAndLastPage", this.loc.PropertyEnum.StiPrintOnTypeOnlyFirstAndLastPage, null, "OnlyFirstAndLastPage"));

    return items;
}

StiMobileDesigner.prototype.GetSortDirectionItemsForSortForm = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiSortDirectionAsc, null, "ASC"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiSortDirectionDesc, null, "DESC"));

    return items;
}

StiMobileDesigner.prototype.GetFilterTypeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiFilterModeAnd, null, "And"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiFilterModeOr, null, "Or"));

    return items;
}

StiMobileDesigner.prototype.GetFilterOperationItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiFilterModeAnd, null, "AND"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiFilterModeOr, null, "OR"));

    return items;
}

StiMobileDesigner.prototype.GetFilterFieldIsItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiFilterItemValue, null, "Value"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiFilterItemExpression, null, "Expression"));

    return items;
}

StiMobileDesigner.prototype.GetFilterDataTypeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiFilterDataTypeString, null, "String"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiFilterDataTypeNumeric, null, "Numeric"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiFilterDataTypeDateTime, null, "DateTime"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StiFilterDataTypeBoolean, null, "Boolean"));
    items.push(this.Item("item4", this.loc.PropertyEnum.StiFilterDataTypeExpression, null, "Expression"));

    return items;
}

StiMobileDesigner.prototype.GetFilterConditionItems = function (dataType, isChartFilter, isDataTransformFilter) {
    var items = [];
    switch (dataType) {
        case "String":
            {
                items.push(this.Item("item0", this.loc.PropertyEnum.StiFilterConditionEqualTo, null, "EqualTo"));
                items.push(this.Item("item1", this.loc.PropertyEnum.StiFilterConditionNotEqualTo, null, "NotEqualTo"));
                items.push("separator1");
                items.push(this.Item("item2", this.loc.PropertyEnum.StiFilterConditionContaining, null, "Containing"));
                items.push(this.Item("item3", this.loc.PropertyEnum.StiFilterConditionNotContaining, null, "NotContaining"));
                items.push("separator2");
                items.push(this.Item("item4", this.loc.PropertyEnum.StiFilterConditionBeginningWith, null, "BeginningWith"));
                items.push(this.Item("item5", this.loc.PropertyEnum.StiFilterConditionEndingWith, null, "EndingWith"));
                if (isDataTransformFilter) {
                    items.push("separator3");
                    items.push(this.Item("item6", this.loc.PropertyEnum.StiFilterConditionBetween, null, "Between"));
                    items.push(this.Item("item7", this.loc.PropertyEnum.StiFilterConditionNotBetween, null, "NotBetween"));
                    items.push("separator4");
                    items.push(this.Item("item8", this.loc.PropertyEnum.StiFilterConditionGreaterThan, null, "GreaterThan"));
                    items.push(this.Item("item9", this.loc.PropertyEnum.StiFilterConditionGreaterThanOrEqualTo, null, "GreaterThanOrEqualTo"));
                    items.push("separator5");
                    items.push(this.Item("item10", this.loc.PropertyEnum.StiFilterConditionLessThan, null, "LessThan"));
                    items.push(this.Item("item11", this.loc.PropertyEnum.StiFilterConditionLessThanOrEqualTo, null, "LessThanOrEqualTo"));
                }
                if (!isChartFilter) {
                    items.push("separator6");
                    items.push(this.Item("item12", this.loc.PropertyEnum.StiFilterConditionIsNull, null, "IsNull"));
                    items.push(this.Item("item13", this.loc.PropertyEnum.StiFilterConditionIsNotNull, null, "IsNotNull"));
                }
                if (isDataTransformFilter) {
                    items.push("separator7");
                    items.push(this.Item("item14", this.loc.PropertyEnum.StiFilterConditionIsBlank, null, "IsBlank"));
                    items.push(this.Item("item15", this.loc.PropertyEnum.StiFilterConditionIsNotBlank, null, "IsNotBlank"));
                }
                break;
            }
        case "Numeric":
        case "DateTime":
            {
                items.push(this.Item("item0", this.loc.PropertyEnum.StiFilterConditionEqualTo, null, "EqualTo"));
                items.push(this.Item("item1", this.loc.PropertyEnum.StiFilterConditionNotEqualTo, null, "NotEqualTo"));
                items.push("separator1");
                if (!isChartFilter) {
                    items.push(this.Item("item2", this.loc.PropertyEnum.StiFilterConditionBetween, null, "Between"));
                    items.push(this.Item("item3", this.loc.PropertyEnum.StiFilterConditionNotBetween, null, "NotBetween"));
                    items.push("separator2");
                }
                items.push(this.Item("item4", this.loc.PropertyEnum.StiFilterConditionGreaterThan, null, "GreaterThan"));
                items.push(this.Item("item5", this.loc.PropertyEnum.StiFilterConditionGreaterThanOrEqualTo, null, "GreaterThanOrEqualTo"));
                items.push("separator3");
                items.push(this.Item("item6", this.loc.PropertyEnum.StiFilterConditionLessThan, null, "LessThan"));
                items.push(this.Item("item7", this.loc.PropertyEnum.StiFilterConditionLessThanOrEqualTo, null, "LessThanOrEqualTo"));
                if (!isChartFilter) {
                    items.push("separator4");
                    items.push(this.Item("item8", this.loc.PropertyEnum.StiFilterConditionIsNull, null, "IsNull"));
                    items.push(this.Item("item9", this.loc.PropertyEnum.StiFilterConditionIsNotNull, null, "IsNotNull"));
                }
                break;
            }
        case "Boolean":
            {
                items.push(this.Item("item0", this.loc.PropertyEnum.StiFilterConditionEqualTo, null, "EqualTo"));
                items.push(this.Item("item1", this.loc.PropertyEnum.StiFilterConditionNotEqualTo, null, "NotEqualTo"));
                break;
            }
        case "Expression":
            {
                items.push(this.Item("item0", this.loc.PropertyEnum.StiFilterConditionEqualTo, null, "EqualTo"));
                items.push(this.Item("item1", this.loc.PropertyEnum.StiFilterConditionNotEqualTo, null, "NotEqualTo"));
                items.push("separator1");
                items.push(this.Item("item2", this.loc.PropertyEnum.StiFilterConditionBetween, null, "Between"));
                items.push(this.Item("item3", this.loc.PropertyEnum.StiFilterConditionNotBetween, null, "NotBetween"));
                items.push("separator2");
                items.push(this.Item("item4", this.loc.PropertyEnum.StiFilterConditionGreaterThan, null, "GreaterThan"));
                items.push(this.Item("item5", this.loc.PropertyEnum.StiFilterConditionGreaterThanOrEqualTo, null, "GreaterThanOrEqualTo"));
                items.push("separator3");
                items.push(this.Item("item6", this.loc.PropertyEnum.StiFilterConditionLessThan, null, "LessThan"));
                items.push(this.Item("item7", this.loc.PropertyEnum.StiFilterConditionLessThanOrEqualTo, null, "LessThanOrEqualTo"));
                items.push("separator4");
                items.push(this.Item("item8", this.loc.PropertyEnum.StiFilterConditionContaining, null, "Containing"));
                items.push(this.Item("item9", this.loc.PropertyEnum.StiFilterConditionNotContaining, null, "NotContaining"));
                items.push("separator5");
                items.push(this.Item("item10", this.loc.PropertyEnum.StiFilterConditionBeginningWith, null, "BeginningWith"));
                items.push(this.Item("item11", this.loc.PropertyEnum.StiFilterConditionEndingWith, null, "EndingWith"));
                items.push("separator6");
                items.push(this.Item("item12", this.loc.PropertyEnum.StiFilterConditionIsNull, null, "IsNull"));
                items.push(this.Item("item13", this.loc.PropertyEnum.StiFilterConditionIsNotNull, null, "IsNotNull"));
                break;
            }
    }

    return items;
}

StiMobileDesigner.prototype.GetSummaryTypeItems = function () {
    var items = [];
    for (var i = 0; i < this.options.summaryTypes.length; i++)
        items.push(this.Item("item" + i, this.options.summaryTypes[i].value, null, this.options.summaryTypes[i].key));

    return items;
}

StiMobileDesigner.prototype.GetShapeTypeItems = function () {
    var items = [];

    items.push(this.Item("shape0", this.loc.Shapes.Arrow, null, "StiArrowShapeType"));
    items.push(this.Item("shape1", this.loc.Shapes.DiagonalDownLine, null, "StiDiagonalDownLineShapeType"));
    items.push(this.Item("shape2", this.loc.Shapes.DiagonalUpLine, null, "StiDiagonalUpLineShapeType"));
    items.push(this.Item("shape3", this.loc.Shapes.HorizontalLine, null, "StiHorizontalLineShapeType"));
    items.push(this.Item("shape4", this.loc.Shapes.LeftAndRightLine, null, "StiLeftAndRightLineShapeType"));
    items.push(this.Item("shape5", this.loc.Shapes.Oval, null, "StiOvalShapeType"));
    items.push(this.Item("shape6", this.loc.Shapes.Rectangle, null, "StiRectangleShapeType"));
    items.push(this.Item("shape7", this.loc.Shapes.RoundedRectangle, null, "StiRoundedRectangleShapeType"));
    items.push(this.Item("shape8", this.loc.Shapes.TopAndBottomLine, null, "StiTopAndBottomLineShapeType"));
    items.push(this.Item("shape9", this.loc.Shapes.Triangle, null, "StiTriangleShapeType"));
    items.push(this.Item("shape10", this.loc.Shapes.VerticalLine, null, "StiVerticalLineShapeType"));
    items.push(this.Item("shape11", this.loc.Shapes.ComplexArrow, null, "StiComplexArrowShapeType"));
    items.push(this.Item("shape12", this.loc.Shapes.BentArrow, null, "StiBentArrowShapeType"));
    items.push(this.Item("shape13", this.loc.Shapes.Chevron, null, "StiChevronShapeType"));
    items.push(this.Item("shape14", this.loc.Shapes.Division, null, "StiDivisionShapeType"));
    items.push(this.Item("shape15", this.loc.Shapes.Equal, null, "StiEqualShapeType"));
    items.push(this.Item("shape16", this.loc.Shapes.FlowchartCard, null, "StiFlowchartCardShapeType"));
    items.push(this.Item("shape17", this.loc.Shapes.FlowchartCollate, null, "StiFlowchartCollateShapeType"));
    items.push(this.Item("shape18", this.loc.Shapes.FlowchartDecision, null, "StiFlowchartDecisionShapeType"));
    items.push(this.Item("shape19", this.loc.Shapes.FlowchartManualInput, null, "StiFlowchartManualInputShapeType"));
    items.push(this.Item("shape20", this.loc.Shapes.FlowchartOffPageConnector, null, "StiFlowchartOffPageConnectorShapeType"));
    items.push(this.Item("shape21", this.loc.Shapes.FlowchartPreparation, null, "StiFlowchartPreparationShapeType"));
    items.push(this.Item("shape22", this.loc.Shapes.FlowchartSort, null, "StiFlowchartSortShapeType"));
    items.push(this.Item("shape23", this.loc.Shapes.Frame, null, "StiFrameShapeType"));
    items.push(this.Item("shape24", this.loc.Shapes.Minus, null, "StiMinusShapeType"));
    items.push(this.Item("shape25", this.loc.Shapes.Multiply, null, "StiMultiplyShapeType"));
    items.push(this.Item("shape26", this.loc.Shapes.Parallelogram, null, "StiParallelogramShapeType"));
    items.push(this.Item("shape27", this.loc.Shapes.Plus, null, "StiPlusShapeType"));
    items.push(this.Item("shape28", this.loc.Shapes.RegularPentagon, null, "StiRegularPentagonShapeType"));
    items.push(this.Item("shape29", this.loc.Shapes.Octagon, null, "StiOctagonShapeType"));
    items.push(this.Item("shape30", this.loc.Shapes.Trapezoid, null, "StiTrapezoidShapeType"));
    items.push(this.Item("shape31", this.loc.Shapes.SnipSameSideCornerRectangle, null, "StiSnipSameSideCornerRectangleShapeType"));
    items.push(this.Item("shape32", this.loc.Shapes.SnipDiagonalSideCornerRectangle, null, "StiSnipDiagonalSideCornerRectangleShapeType"));

    return items;
}

StiMobileDesigner.prototype.GetCheckBoxStyleItems = function () {
    var items = [];

    items.push(this.Item("style0", this.loc.PropertyEnum.StiCheckStyleCross, "CheckBoxCross.png", "Cross"));
    items.push(this.Item("style1", this.loc.PropertyEnum.StiCheckStyleCheck, "CheckBoxCheck.png", "Check"));
    items.push(this.Item("style2", this.loc.PropertyEnum.StiCheckStyleCrossRectangle, "CheckBoxCrossRectangle.png", "CrossRectangle"));
    items.push(this.Item("style3", this.loc.PropertyEnum.StiCheckStyleCheckRectangle, "CheckBoxCheckRectangle.png", "CheckRectangle"));
    items.push(this.Item("style4", this.loc.PropertyEnum.StiCheckStyleCrossCircle, "CheckBoxCrossCircle.png", "CrossCircle"));
    items.push(this.Item("style5", this.loc.PropertyEnum.StiCheckStyleDotCircle, "CheckBoxDotCircle.png", "DotCircle"));
    items.push(this.Item("style6", this.loc.PropertyEnum.StiCheckStyleDotRectangle, "CheckBoxDotRectangle.png", "DotRectangle"));
    items.push(this.Item("style7", this.loc.PropertyEnum.StiCheckStyleNoneCircle, "CheckBoxNoneCircle.png", "NoneCircle"));
    items.push(this.Item("style8", this.loc.PropertyEnum.StiCheckStyleNoneRectangle, "CheckBoxNoneRectangle.png", "NoneRectangle"));
    items.push(this.Item("style9", this.loc.PropertyEnum.StiCheckStyleNone, "CheckBoxNone.png", "None"));

    return items;
}

StiMobileDesigner.prototype.GetCheckBoxValuesItems = function () {
    var items = [];

    items.push(this.Item("value0", this.loc.FormFormatEditor.nameTrue + "/" + this.loc.FormFormatEditor.nameFalse, null, "true/false"));
    items.push(this.Item("value1", this.loc.FormFormatEditor.nameYes + "/" + this.loc.FormFormatEditor.nameNo, null, "yes/no"));
    items.push(this.Item("value2", this.loc.FormFormatEditor.nameOn + "/" + this.loc.FormFormatEditor.nameOff, null, "on/off"));
    items.push(this.Item("value3", "1/0", null, "1/0"));

    return items;
}

StiMobileDesigner.prototype.GetCloneContainerItems = function (currentComponentName) {
    var items = [];
    if (this.options.report != null) {
        items.push(this.Item("NotAssigned", this.loc.Report.NotAssigned, null, "[Not Assigned]"));

        for (var pageName in this.options.report.pages)
            for (var componentName in this.options.report.pages[pageName].components) {
                var component = this.options.report.pages[pageName].components[componentName];
                if (component.typeComponent == "StiPanel" ||
                    component.typeComponent == "StiSubReport" ||
                    component.typeComponent == "StiCrossTab" ||
                    (component.typeComponent == "StiClone" && currentComponentName != component.properties.name)) {
                    items.push(this.Item(componentName, componentName, null, componentName));
                }
            }

    }
    return items;
}

StiMobileDesigner.prototype.GetMasterComponentItems = function (resultForDataForm) {
    if (!this.options.report) return null;
    var selectedObjects = this.options.selectedObjects || [this.options.selectedObject];
    if (!selectedObjects) return null;

    var currPageName = selectedObjects[0].properties.pageName;
    var selectedNames = [];
    for (var i = 0; i < selectedObjects.length; i++) selectedNames.push(selectedObjects[i].properties.name)

    var isMasterComponent = function (typeComponent) {
        return typeComponent == "StiTable" || typeComponent == "StiDataBand" || typeComponent == "StiHierarchicalBand" || typeComponent == "StiCrossDataBand" || typeComponent == "StiChart";
    }

    var items = [];
    for (var componentName in this.options.report.pages[currPageName].components) {
        var component = this.options.report.pages[currPageName].components[componentName];
        if (isMasterComponent(component.typeComponent) && !this.IsContains(selectedNames, componentName)) {
            items.push(resultForDataForm ? { name: componentName, type: component.typeComponent } : this.Item(componentName, componentName, null, componentName));
        }
    }

    //subreport
    for (var pageName in this.options.report.pages) {
        for (var componentName in this.options.report.pages[pageName].components) {
            var component = this.options.report.pages[pageName].components[componentName];
            if (component.typeComponent == "StiSubReport" && component.properties.subReportPage == currPageName) {
                var parentName = component.properties.parentName;
                var parentComponent = this.options.report.pages[pageName].components[parentName];

                if (parentComponent.typeComponent == "StiChildBand") {
                    var index = parseInt(parentComponent.properties.componentIndex) - 1;
                    while (index >= 0) {
                        var masterComp = this.GetComponentByIndex(this.options.report.pages[pageName], index);
                        if (masterComp && masterComp.typeComponent != "StiChildBand" && isMasterComponent(masterComp.typeComponent)) {
                            items.push(resultForDataForm ? { name: masterComp.properties.name, type: masterComp.typeComponent } : this.Item(masterComp.properties.name, masterComp.properties.name, null, masterComp.properties.name));
                            break;
                        }
                        index--;
                    }
                }
                else if (parentComponent.typeComponent == "StiPanel") {
                    var parent = this.options.report.getComponentByName(parentComponent.properties.parentName);
                    while (parent != null && parent.typeComponent == "StiPanel") {
                        parent = this.options.report.pages[pageName].components[parent.properties.parentName];
                    }
                    if (parent != null && isMasterComponent(parent.typeComponent)) {
                        items.push(resultForDataForm ? { name: parent.properties.name, type: parent.typeComponent } : this.Item(parent.properties.name, parent.properties.name, null, parent.properties.name));
                    }
                }
                else {
                    while (parentComponent) {
                        if ((parentComponent.typeComponent == "StiDataBand" || parentComponent.typeComponent == "StiHierarchicalBand" || parentComponent.typeComponent == "StiCrossDataBand") && parentComponent) {
                            items.push(resultForDataForm ? { name: parentComponent.properties.name, type: parentComponent.typeComponent } : this.Item(parentComponent.properties.name, parentComponent.properties.name, null, parentComponent.properties.name));
                        }
                        parentComponent = this.options.report.pages[pageName].components[parentComponent.properties.parentName];
                    }
                }
            }
        }
    }

    if (items.length == 0) return null;

    return items;
}

StiMobileDesigner.prototype.GetRelationsInSourceItems = function (object) {
    if (!object) return null;
    var items = [];

    for (var i = 0; i < object.relations.length; i++) {
        items.push(this.Item("relation" + i, object.relations[i].name, null, object.relations[i].nameInSource));
    }

    return items.length == 0 ? null : items;
}

StiMobileDesigner.prototype.GetSubReportItems = function (withDashboards) {
    var items = [];
    var selectedObjects = this.options.selectedObjects || [this.options.selectedObject];
    if (this.options.report && selectedObjects) {
        items.push(this.Item("NotAssigned", this.loc.Report.NotAssigned, null, "[Not Assigned]"));
        var currentPageName = selectedObjects[0].properties.pageName || selectedObjects[0].properties.name;

        for (var pageName in this.options.report.pages) {
            if (pageName != currentPageName && (withDashboards || !this.options.report.pages[pageName].isDashboard))
                items.push(this.Item(pageName, pageName, null, pageName));
        }
    }

    return items;
}

StiMobileDesigner.prototype.GetImageAlignItems = function () {
    var imgAlignArray = ["TopLeft", "TopCenter", "TopRight", "MiddleLeft", "MiddleCenter", "MiddleRight", "BottomLeft", "BottomCenter", "BottomRight"];
    var imgAlignItems = [];
    for (var i = 0; i < imgAlignArray.length; i++)
        imgAlignItems.push(this.Item("AlignItem" + i, this.loc.PropertyEnum["ContentAlignment" + imgAlignArray[i]],
            null, imgAlignArray[i]));

    return imgAlignItems;
}

StiMobileDesigner.prototype.GetComponentStyleItems = function (includeCollections, ignoreTypeComponent, predefinedStyles) {
    var jsObject = this;
    var defaultFont = "Arial!8!0!0!0!0";
    var defaultBrush = "1!transparent";
    var defaultTextBrush = "1!0,0,0";
    var defaultBorder = "default";
    var items = [];
    var collectionNames = {};

    var commonSelectedObject = this.options.selectedObject || this.GetCommonObject(this.options.selectedObjects);
    var typeComponent = (commonSelectedObject && commonSelectedObject.typeComponent && !ignoreTypeComponent) ? commonSelectedObject.typeComponent : "Any";

    items.push(this.Item("styleNone", this.loc.Report.No, null, "[None]", { type: "StiStyle", font: defaultFont, brush: defaultBrush, textBrush: defaultTextBrush, border: defaultBorder }));

    if (this.options.report) {
        for (var i = 0; i < this.options.report.stylesCollection.length; i++) {
            var properties = this.options.report.stylesCollection[i].properties;
            var styleType = this.options.report.stylesCollection[i].type;
            if (includeCollections && properties.collectionName) {
                collectionNames[properties.collectionName] = true;
            }

            //Chart
            if (typeComponent == "StiChart") {
                if (styleType == "StiChartStyle") items.push(this.Item("style" + i, properties.name, null, properties.name));
            }
            //CrossTab
            else if (typeComponent == "StiCrossTab") {
                if (styleType == "StiCrossTabStyle") items.push(this.Item("style" + i, properties.name, null, properties.name));
            }
            //Table
            else if (typeComponent == "StiTable") {
                if (styleType == "StiTableStyle") items.push(this.Item("style" + i, properties.name, null, properties.name));
            }
            //Map
            else if (typeComponent == "StiMap") {
                if (styleType == "StiMapStyle") items.push(this.Item("style" + i, properties.name, null, properties.name));
            }
            //Gauge
            else if (typeComponent == "StiGauge") {
                if (styleType == "StiGaugeStyle") items.push(this.Item("style" + i, properties.name, null, properties.name));
            }
            //Sparkline
            else if (typeComponent == "StiSparkline") {
                if (styleType == "StiIndicatorStyle") items.push(this.Item("style" + i, properties.name, null, properties.name));
            }
            //Other
            else if (styleType == "StiStyle") {
                var styleProperties = {
                    type: "StiStyle",
                    font: (properties["font"] != null && properties["allowUseFont"]) ? properties["font"] : defaultFont,
                    brush: (properties["brush"] != null && properties["allowUseBrush"]) ? properties["brush"] : defaultBrush,
                    textBrush: (properties["textBrush"] != null && properties["allowUseTextBrush"]) ? properties["textBrush"] : defaultTextBrush,
                    border: (properties["border"] != null && properties["allowUseBorderSides"]) ? properties["border"] : defaultBorder
                }
                items.push(this.Item("style" + i, properties.name, null, properties.name, styleProperties));
            }
        }

        if (includeCollections) {
            var styleCollections = [];
            for (var collectionName in collectionNames) {
                styleCollections.push(collectionName);
            }
            return {
                styleCollections: styleCollections,
                styleItems: items
            };
        }

        if (predefinedStyles) {
            for (var i = 0; i < predefinedStyles.length; i++) {
                var properties = predefinedStyles[i].properties;
                var styleType = predefinedStyles[i].type;

                var styleProperties = {
                    type: styleType,
                    font: properties.font,
                    brush: properties.brush,
                    textBrush: properties.textBrush,
                    border: properties.border
                }
                items.push(this.Item("style" + i, properties.name, null, "##" + properties.name + "##", styleProperties));
            }
        }
    }

    return items;
}

StiMobileDesigner.prototype.GetHorizontalAlignmentItems = function (shotType) {
    var items = [];
    items.push(this.Item("horAlignLeft", this.loc.PropertyEnum.StiTextHorAlignmentLeft, null, "Left"));
    items.push(this.Item("horAlignCenter", this.loc.PropertyEnum.StiTextHorAlignmentCenter, null, "Center"));
    items.push(this.Item("horAlignRight", this.loc.PropertyEnum.StiTextHorAlignmentRight, null, "Right"));
    if (!shotType) items.push(this.Item("horAlignWidth", this.loc.PropertyEnum.StiTextHorAlignmentWidth, null, "Width"));

    return items;
}

StiMobileDesigner.prototype.GetVerticalAlignmentItems = function () {
    var items = [];
    items.push(this.Item("vertAlignTop", this.loc.PropertyEnum.StiVertAlignmentTop, null, "Top"));
    items.push(this.Item("vertAlignCenter", this.loc.PropertyEnum.StiVertAlignmentCenter, null, "Center"));
    items.push(this.Item("vertAlignBottom", this.loc.PropertyEnum.StiVertAlignmentBottom, null, "Bottom"));

    return items;
}

StiMobileDesigner.prototype.GetCrossTabHorAlignItems = function (shotType) {
    var items = [];
    items.push(this.Item("crossTabAlignNone", this.loc.PropertyEnum.StiCrossHorAlignmentNone, null, "None"));
    items.push(this.Item("crossTabAlignLeft", this.loc.PropertyEnum.StiTextHorAlignmentLeft, null, "Left"));
    items.push(this.Item("crossTabAlignCenter", this.loc.PropertyEnum.StiTextHorAlignmentCenter, null, "Center"));
    items.push(this.Item("crossTabAlignRight", this.loc.PropertyEnum.StiTextHorAlignmentRight, null, "Right"));
    items.push(this.Item("crossTabAlignWidth", this.loc.PropertyEnum.StiTextHorAlignmentWidth, null, "Width"));

    return items;
}

StiMobileDesigner.prototype.GetTextFormatItems = function (showImage) {
    var items = [];
    items.push(this.Item("StiGeneralFormatService", this.loc.FormFormatEditor.General, showImage ? "FormatGeneral.png" : null, "StiGeneralFormatService"));
    items.push(this.Item("StiNumberFormatService", this.loc.FormFormatEditor.Number, showImage ? "FormatNumber.png" : null, "StiNumberFormatService"));
    items.push(this.Item("StiCurrencyFormatService", this.loc.FormFormatEditor.Currency, showImage ? "FormatCurrency.png" : null, "StiCurrencyFormatService"));
    items.push(this.Item("StiDateFormatService", this.loc.FormFormatEditor.Date, showImage ? "FormatDate.png" : null, "StiDateFormatService"));
    items.push(this.Item("StiTimeFormatService", this.loc.FormFormatEditor.Time, showImage ? "FormatTime.png" : null, "StiTimeFormatService"));
    items.push(this.Item("StiPercentageFormatService", this.loc.FormFormatEditor.Percentage, showImage ? "FormatPercentage.png" : null, "StiPercentageFormatService"));
    items.push(this.Item("StiBooleanFormatService", this.loc.FormFormatEditor.Boolean, showImage ? "FormatBoolean.png" : null, "StiBooleanFormatService"));

    return items;
}

StiMobileDesigner.prototype.GetRelationsItemsFromDataSource = function (dataSource) {
    var relationsArray = [];
    this.GetAllRelationsFromDataSource(dataSource, "", null, relationsArray);

    var items = [];
    items.push(this.Item("NotAssigned", this.loc.Report.NotAssigned, null, "NotAssigned"));

    for (var i = 0; i < relationsArray.length; i++) {
        items.push(this.Item("relationItem" + relationsArray[i], relationsArray[i], null, relationsArray[i]));
    }

    return items;
}

StiMobileDesigner.prototype.GetTotalFuntionItems = function (notNone) {
    var items = [];
    if (!notNone) items.push(this.Item("TotalFuntionNone", this.loc.PropertyEnum.StiBorderSidesNone, null, "none"));

    for (var i = 0; i < this.options.aggrigateFunctions.length; i++) {
        items.push(this.Item("TotalFuntion" + i, this.options.aggrigateFunctions[i].serviceName, null, this.options.aggrigateFunctions[i].serviceName));
    }

    return items;
}

StiMobileDesigner.prototype.GetLanguagesItems = function () {
    var items = [];
    items.push(this.Item("languageC", "C#", null, "C"));

    if (!this.options.jsMode)
        items.push(this.Item("languageVB", "VB.Net", null, "VB"));
    else
        items.push(this.Item("languageJS", "JS", null, "JS"));

    return items;
}

StiMobileDesigner.prototype.GetComponentTypeItems = function () {
    var items = [];
    items.push(this.Item("componentType0", this.loc.Components.StiDataBand, null, "Data"));
    items.push(this.Item("componentType1", this.loc.Components.StiTable, null, "Table"));

    return items;
}

StiMobileDesigner.prototype.GetFontSizes = function () {
    return ["5", "6", "7", "8", "9", "10", "11", "12", "14", "16", "18", "20", "22", "24", "26", "28", "36", "48", "72"];
}

StiMobileDesigner.prototype.GetDictionaryNewItems = function () {
    var items = [];
    items.push(this.Item("dataSourceNew", this.loc.MainMenu.menuEditDataSourceNew, "DataSourceNew.png", "dataSourceNew"));
    if (!this.options.isJava) {
        items.push(this.Item("dataSourceNewFromResource", this.loc.MainMenu.menuEditDataSourceNew, "DataSourceNew.png", "dataSourceNewFromResource"));
        items.push(this.Item("dataTransformationNew", this.loc.MainMenu.menuEditDataTransformationNew, "DataTransformationNew.png", "dataTransformationNew"));
    }
    items.push("separator1");
    if (!this.options.isJava && !this.options.jsMode) {
        items.push(this.Item("businessObjectNew", this.loc.MainMenu.menuEditBusinessObjectNew, "BusinessObjectNew.png", "businessObjectNew"));
    }
    items.push(this.Item("columnNew", this.loc.MainMenu.menuEditColumnNew, "ColumnNew.png", "columnNew"));
    items.push(this.Item("calcColumnNew", this.loc.MainMenu.menuEditCalcColumnNew, "CalcColumnNew.png", "calcColumnNew"));
    items.push(this.Item("parameterNew", this.loc.MainMenu.menuEditDataParameterNew, "ParameterNew.png", "parameterNew"));
    items.push(this.Item("relationNew", this.loc.MainMenu.menuEditRelationNew, "RelationNew.png", "relationNew"));
    items.push("separator2");
    items.push(this.Item("categoryNew", this.loc.MainMenu.menuEditCategoryNew, "CategoryNew.png", "categoryNew"));
    items.push(this.Item("variableNew", this.loc.MainMenu.menuEditVariableNew, "VariableNew.png", "variableNew"));
    items.push(this.Item("resourceNew", this.loc.MainMenu.menuEditResourceNew, "Resources.ResourceNew.png", "resourceNew"));
    items.push("separator2_1");
    items.push(this.Item("menuMakeThisRelationActive", this.loc.MainMenu.menuMakeThisRelationActive, "RelationActive.png", "menuMakeThisRelationActive"));
    return items;
}

StiMobileDesigner.prototype.GetDictionaryActionsItems = function () {
    var items = [];
    items.push(this.Item("newDictionary", this.loc.FormDictionaryDesigner.DictionaryNew, "New.png", "newDictionary"));
    items.push(this.Item("openDictionary", this.loc.FormDictionaryDesigner.DictionaryOpen, "Open.png", "openDictionary"));
    items.push(this.Item("mergeDictionary", this.loc.FormDictionaryDesigner.DictionaryMerge, "Merge.png", "mergeDictionary"));
    items.push("separator1");
    items.push(this.Item("saveDictionary", this.loc.FormDictionaryDesigner.DictionarySaveAs, "Save.png", "saveDictionary"));
    items.push("separator2");
    items.push(this.Item("synchronize", this.loc.FormDictionaryDesigner.Synchronize, "Synchronize.png", "synchronize"));
    items.push(this.Item("embedAllDataToResources", this.loc.MainMenu.menuEmbedAllDataToResources, "EmbedAllData.png", "embedAllDataToResources"));

    return items;
}

StiMobileDesigner.prototype.GetColumnTypesItems = function () {
    var items = [];
    items.push(this.Item("bool", "bool", null, "bool"));
    items.push(this.Item("byte", "byte", null, "byte"));
    items.push(this.Item("byte[]", "byte[]", null, "byte[]"));
    items.push(this.Item("char", "char", null, "char"));
    items.push(this.Item("datetime", "datetime", null, "datetime"));
    items.push(this.Item("datetimeoffset", "datetime offset", null, "datetimeoffset"));
    items.push(this.Item("decimal", "decimal", null, "decimal"));
    items.push(this.Item("double", "double", null, "double"));
    items.push(this.Item("guid", "guid", null, "guid"));
    items.push(this.Item("short", "short", null, "short"));
    items.push(this.Item("int", "int", null, "int"));
    items.push(this.Item("long", "long", null, "long"));
    items.push(this.Item("sbyte", "sbyte", null, "sbyte"));
    items.push(this.Item("float", "float", null, "float"));
    items.push(this.Item("string", "string", null, "string"));
    items.push(this.Item("timespan", "timespan", null, "timespan"));
    items.push(this.Item("ushort", "ushort", null, "ushort"));
    items.push(this.Item("uint", "uint", null, "uint"));
    items.push(this.Item("ulong", "ulong", null, "ulong"));
    items.push(this.Item("image", "image", null, "image"));
    items.push(this.Item("bool (Nullable)", "bool (Nullable)", null, "bool (Nullable)"));
    items.push(this.Item("byte (Nullable)", "byte (Nullable)", null, "byte (Nullable)"));
    items.push(this.Item("char (Nullable)", "char (Nullable)", null, "char (Nullable)"));
    items.push(this.Item("datetime (Nullable)", "datetime (Nullable)", null, "datetime (Nullable)"));
    items.push(this.Item("datetimeoffset (Nullable)", "datetime offset (Nullable)", null, "datetimeoffset (Nullable)"));
    items.push(this.Item("decimal (Nullable)", "decimal (Nullable)", null, "decimal (Nullable)"));
    items.push(this.Item("double (Nullable)", "double (Nullable)", null, "double (Nullable)"));
    items.push(this.Item("guid (Nullable)", "guid (Nullable)", null, "guid (Nullable)"));
    items.push(this.Item("short (Nullable)", "short (Nullable)", null, "short (Nullable)"));
    items.push(this.Item("int (Nullable)", "int (Nullable)", null, "int (Nullable)"));
    items.push(this.Item("long (Nullable)", "long (Nullable)", null, "long (Nullable)"));
    items.push(this.Item("byte (Nullable)", "byte (Nullable)", null, "byte (Nullable)"));
    items.push(this.Item("sbyte (Nullable)", "sbyte (Nullable)", null, "sbyte (Nullable)"));
    items.push(this.Item("float (Nullable)", "float (Nullable)", null, "float (Nullable)"));
    items.push(this.Item("timespan (Nullable)", "timespan (Nullable)", null, "timespan (Nullable)"));
    items.push(this.Item("ushort (Nullable)", "ushort (Nullable)", null, "ushort (Nullable)"));
    items.push(this.Item("uint (Nullable)", "uint (Nullable)", null, "uint (Nullable)"));
    items.push(this.Item("ulong (Nullable)", "ulong (Nullable)", null, "ulong (Nullable)"));
    items.push(this.Item("object", "object", null, "object"));
    items.push(this.Item("refcursor", "refcursor", null, "object"));

    return items;
}

StiMobileDesigner.prototype.GetQueryTextTypeItems = function () {
    var items = [];
    items.push(this.Item("table", this.loc.PropertyEnum.StiSqlSourceTypeTable, null, "Table"));
    items.push(this.Item("storedProcedure", this.loc.PropertyEnum.StiSqlSourceTypeStoredProcedure, null, "StoredProcedure"));

    return items;
}

StiMobileDesigner.prototype.GetVariableTypesItems = function () {
    var items = [];
    items.push(this.Item("string", "string", "DataColumnString.png", "string"));
    items.push(this.Item("float", "float", "DataColumnFloat.png", "float"));
    items.push(this.Item("double", "double", "DataColumnFloat.png", "double"));
    items.push(this.Item("decimal", "decimal", "DataColumnDecimal.png", "decimal"));
    items.push("separator1");
    items.push(this.Item("datetime", "datetime", "DataColumnDateTime.png", "datetime"));
    items.push(this.Item("datetimeoffset", "datetime offset", "DataColumnDateTime.png", "datetimeoffset"));
    items.push(this.Item("timespan", "timespan", "DataColumnDateTime.png", "timespan"));
    items.push("separator2");
    items.push(this.Item("sbyte", "sbyte", "DataColumnInt.png", "sbyte"));
    items.push(this.Item("byte", "byte", "DataColumnInt.png", "byte"));
    items.push(this.Item("byte[]", "byte[]", "DataColumnInt.png", "byte[]"));
    items.push(this.Item("short", "short", "DataColumnInt.png", "short"));
    items.push(this.Item("ushort", "ushort", "DataColumnInt.png", "ushort"));
    items.push(this.Item("int", "int", "DataColumnInt.png", "int"));
    items.push(this.Item("uint", "uint", "DataColumnInt.png", "uint"));
    items.push(this.Item("long", "long", "DataColumnInt.png", "long"));
    items.push(this.Item("ulong", "ulong", "DataColumnInt.png", "ulong"));
    items.push("separator3");
    items.push(this.Item("bool", "bool", "DataColumnBool.png", "bool"));
    items.push(this.Item("char", "char", "DataColumnChar.png", "char"));
    items.push(this.Item("guid", "guid", "DataColumnString.png", "guid"));
    items.push(this.Item("object", "object", "DataColumnString.png", "object"));
    items.push("separator3");
    items.push(this.Item("image", "image", "DataColumnImage.png", "image"));

    return items;
}

StiMobileDesigner.prototype.GetVariableBasicTypesItems = function () {
    var items = [];
    items.push(this.Item("value", this.loc.PropertyEnum.StiTypeModeValue, null, "Value"));
    items.push(this.Item("nullablevalue", this.loc.PropertyEnum.StiTypeModeNullableValue, null, "NullableValue"));
    items.push(this.Item("range", this.loc.PropertyEnum.StiTypeModeRange, null, "Range"));
    items.push(this.Item("list", this.loc.PropertyEnum.StiTypeModeList, null, "List"));

    return items;
}

StiMobileDesigner.prototype.GetMonthesForDatePickerItems = function () {
    var items = [];
    for (var i = 0; i < this.options.monthesCollection.length; i++)
        items.push(this.Item("Month" + i, this.loc.A_WebViewer["Month" + this.options.monthesCollection[i]], null, i));

    return items;
}

StiMobileDesigner.prototype.GetVariableDataSourceItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyMain.Items, null, "Items"));
    items.push(this.Item("item1", this.loc.PropertyMain.DataColumns, null, "Columns"));

    return items;
}

StiMobileDesigner.prototype.GetVariableDateTimeTypesItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiDateTimeTypeDateAndTime, null, "DateAndTime"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiDateTimeTypeDate, null, "Date"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiDateTimeTypeTime, null, "Time"));

    return items;
}

StiMobileDesigner.prototype.GetVariableSortDirectionItems = function () {
    var items = [];
    items.push(this.Item("None", this.loc.FormBand.NoSort, null, "None"));
    items.push(this.Item("Asc", this.loc.FormBand.Ascending, null, "Asc"));
    items.push(this.Item("Desc", this.loc.FormBand.Descending, null, "Desc"));

    return items;
}

StiMobileDesigner.prototype.GetVariableSortFieldItems = function () {
    var items = [];
    items.push(this.Item("Key", this.loc.PropertyMain.Key, null, "Key"));
    items.push(this.Item("Label", this.loc.PropertyMain.Label, null, "Label"));

    return items;
}

StiMobileDesigner.prototype.GetBoolItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.FormFormatEditor.nameTrue, null, "True"));
    items.push(this.Item("item1", this.loc.FormFormatEditor.nameFalse, null, "False"));

    return items;
}

StiMobileDesigner.prototype.GetAddStyleMenuItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.Components.StiComponent, "Styles.StiStyle32.png", "StiStyle"));
    items.push(this.Item("item1", this.loc.Components.StiChart, "Styles.StiChartStyle32.png", "StiChartStyle"));
    items.push(this.Item("item2", this.loc.Components.StiGauge, "Styles.StiGaugeStyle32.png", "StiGaugeStyle"));
    items.push(this.Item("item3", this.loc.Components.StiMap, "Styles.StiMapStyle32.png", "StiMapStyle"));
    items.push(this.Item("item4", this.loc.Components.StiCrossTab, "Styles.StiCrossTabStyle32.png", "StiCrossTabStyle"));
    items.push(this.Item("item5", this.loc.Components.StiTable, "Styles.StiTableStyle32.png", "StiTableStyle"));

    if (this.options.dashboardAssemblyLoaded) {
        items.push(this.Item("item6", this.loc.Components.StiCards, "Styles.StiCardsStyle32.png", "StiCardsStyle"));
        items.push(this.Item("item7", this.loc.Components.StiIndicator, "Styles.StiIndicatorStyle32.png", "StiIndicatorStyle"));
        items.push(this.Item("item8", this.loc.Components.StiProgress, "Styles.StiProgressStyle32.png", "StiProgressStyle"));
        items.push(this.Item("item9", this.loc.PropertyCategory.ControlCategory, "Styles.StiDialogStyle32.png", "StiDialogStyle"));
    }

    return items;
}

StiMobileDesigner.prototype.GetNestedLevelsItems = function () {
    var items = [];
    for (var i = 1; i <= 10; i++)
        items.push(this.Item("item" + i, i, null, i.toString()));

    return items;
}

StiMobileDesigner.prototype.GetNestedFactorItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiNestedFactorHigh, null, "High"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiNestedFactorNormal, null, "Normal"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiNestedFactorLow, null, "Low"));

    return items;
}

StiMobileDesigner.prototype.GetPageMarginsItems = function () {
    var items = [];
    items.push(this.Item("marginsNormal", "<b>" + this.loc.FormDesigner.MarginsNormal + "</b>", "MarginsNormal.png", "marginsNormal"));
    items.push(this.Item("marginsNarrow", "<b>" + this.loc.FormDesigner.MarginsNarrow + "</b>", "MarginsNarrow.png", "marginsNarrow"));
    items.push(this.Item("marginsWide", "<b>" + this.loc.FormDesigner.MarginsWide + "</b>", "MarginsWide.png", "marginsWide"));

    return items;
}

StiMobileDesigner.prototype.GetPageColumnsItems = function () {
    var items = [];
    items.push(this.Item("oneColumn", this.loc.FormDesigner.ColumnsOne, "OneColumn.png", "0"));
    items.push(this.Item("twoColumn", this.loc.FormDesigner.ColumnsTwo, "TwoColumn.png", "2"));
    items.push(this.Item("threeColumn", this.loc.FormDesigner.ColumnsThree, "ThreeColumn.png", "3"));

    return items;
}

StiMobileDesigner.prototype.GetAddSeriesItems = function () {
    var items = [];
    items.push(this.Item("ClusteredColumn", this.loc.Chart.ClusteredColumn, "Charts.Small.ClusteredColumn.png", "ClusteredColumn", null, true));
    items.push(this.Item("Line", this.loc.Chart.Line, "Charts.Small.Line.png", "Line", null, true));
    items.push(this.Item("Pie", this.loc.Chart.Pie, "Charts.Small.Pie.png", "Pie", null, true));
    items.push(this.Item("ClusteredBar", this.loc.Chart.ClusteredBar, "Charts.Small.ClusteredBar.png", "ClusteredBar", null, true));
    items.push(this.Item("Area", this.loc.Chart.Area, "Charts.Small.Area.png", "Area", null, true));
    items.push(this.Item("Range", this.loc.Chart.Range, "Charts.Small.Range.png", "Range", null, true));
    items.push(this.Item("Scatter", this.loc.Chart.Scatter, "Charts.Small.Scatter.png", "Scatter", null, true));
    items.push(this.Item("Radar", this.loc.Chart.Radar, "Charts.Small.RadarArea.png", "Radar", null, true));
    items.push(this.Item("Funnel", this.loc.Chart.Funnel, "Charts.Small.Funnel.png", "Funnel", null, true));
    items.push(this.Item("Financial", this.loc.Chart.Financial, "Charts.Small.Candlestick.png", "Financial", null, true));
    items.push(this.Item("Treemap", this.loc.Chart.Treemap, "Charts.Small.Treemap.png", "Treemap", null, true));
    items.push(this.Item("Sunburst", this.loc.Chart.Sunburst, "Charts.Small.Sunburst.png", "Sunburst", null, true));
    items.push(this.Item("Histogram", this.loc.Chart.Histogram, "Charts.Small.Histogram.png", "Histogram", null, true));
    items.push(this.Item("BoxAndWhisker", this.loc.Chart.BoxAndWhisker, "Charts.Small.BoxAndWhisker.png", "BoxAndWhisker", null, true));
    items.push(this.Item("Waterfall", this.loc.Chart.Waterfall, "Charts.Small.Waterfall.png", "Waterfall", null, true));
    items.push(this.Item("Pictorial", this.loc.Chart.Pictorial, "Charts.Small.Pictorial.png", "Pictorial", null, true));

    return items;
}

StiMobileDesigner.prototype.GetChartClusteredColumnItems = function () {
    var items = [];
    items.push(this.Item("StiClusteredColumnSeries", this.loc.Chart.ClusteredColumn, "Charts.Small.ClusteredColumn.png", "StiClusteredColumnSeries"));
    items.push(this.Item("StiStackedColumnSeries", this.loc.Chart.StackedColumn, "Charts.Small.StackedColumn.png", "StiStackedColumnSeries"));
    items.push(this.Item("StiFullStackedColumnSeries", this.loc.Chart.FullStackedColumn, "Charts.Small.FullStackedColumn.png", "StiFullStackedColumnSeries"));
    items.push(this.Item("StiClusteredColumnSeries3D", "3D " + this.loc.Chart.ClusteredColumn, "Charts.Small.ClusteredColumn3D.png", "StiClusteredColumnSeries3D"));
    items.push(this.Item("StiStackedColumnSeries3D", "3D " + this.loc.Chart.StackedColumn, "Charts.Small.StackedColumn3D.png", "StiStackedColumnSeries3D"));
    items.push(this.Item("StiFullStackedColumnSeries3D", "3D " + this.loc.Chart.FullStackedColumn, "Charts.Small.FullStackedColumn3D.png", "StiFullStackedColumnSeries3D"));

    return items;
}

StiMobileDesigner.prototype.GetChartLineItems = function () {
    var items = [];
    items.push(this.Item("StiLineSeries", this.loc.Chart.Line, "Charts.Small.Line.png", "StiLineSeries"));
    items.push(this.Item("StiStackedLineSeries", this.loc.Chart.StackedLine, "Charts.Small.StackedLine.png", "StiStackedLineSeries"));
    items.push(this.Item("StiFullStackedLineSeries", this.loc.Chart.FullStackedLine, "Charts.Small.FullStackedLine.png", "StiFullStackedLineSeries"));
    items.push("separator");
    items.push(this.Item("StiSplineSeries", this.loc.Chart.Spline, "Charts.Small.Spline.png", "StiSplineSeries"));
    items.push(this.Item("StiStackedSplineSeries", this.loc.Chart.StackedSpline, "Charts.Small.StackedSpline.png", "StiStackedSplineSeries"));
    items.push(this.Item("StiFullStackedSplineSeries", this.loc.Chart.FullStackedSpline, "Charts.Small.FullStackedSpline.png", "StiFullStackedSplineSeries"));
    items.push("separator");
    items.push(this.Item("StiSteppedLineSeries", this.loc.Chart.SteppedLine, "Charts.Small.SteppedLine.png", "StiSteppedLineSeries"));

    return items;
}

StiMobileDesigner.prototype.GetChartAreaItems = function () {
    var items = [];
    items.push(this.Item("StiAreaSeries", this.loc.Chart.Area, "Charts.Small.Area.png", "StiAreaSeries"));
    items.push(this.Item("StiStackedAreaSeries", this.loc.Chart.StackedArea, "Charts.Small.StackedArea.png", "StiStackedAreaSeries"));
    items.push(this.Item("StiFullStackedAreaSeries", this.loc.Chart.FullStackedArea, "Charts.Small.FullStackedArea.png", "StiFullStackedAreaSeries"));
    items.push("separator");
    items.push(this.Item("StiSplineAreaSeries", this.loc.Chart.SplineArea, "Charts.Small.SplineArea.png", "StiSplineAreaSeries"));
    items.push(this.Item("StiStackedSplineAreaSeries", this.loc.Chart.StackedSplineArea, "Charts.Small.StackedSplineArea.png", "StiStackedSplineAreaSeries"));
    items.push(this.Item("StiFullStackedSplineAreaSeries", this.loc.Chart.FullStackedSplineArea, "Charts.Small.FullStackedSplineArea.png", "StiFullStackedSplineAreaSeries"));
    items.push("separator");
    items.push(this.Item("StiSteppedAreaSeries", this.loc.Chart.SteppedArea, "Charts.Small.SteppedArea.png", "StiSteppedAreaSeries"));

    return items;
}

StiMobileDesigner.prototype.GetChartRangeItems = function () {
    var items = [];
    items.push(this.Item("StiRangeSeries", this.loc.Chart.Range, "Charts.Small.Range.png", "StiRangeSeries"));
    items.push(this.Item("StiSplineRangeSeries", this.loc.Chart.SplineRange, "Charts.Small.SplineRange.png", "StiSplineRangeSeries"));
    items.push(this.Item("StiSteppedRangeSeries", this.loc.Chart.SteppedRange, "Charts.Small.SteppedRange.png", "StiSteppedRangeSeries"));
    items.push("separator");
    items.push(this.Item("StiRangeBarSeries", this.loc.Chart.RangeBar, "Charts.Small.RangeBar.png", "StiRangeBarSeries"));

    return items;
}

StiMobileDesigner.prototype.GetChartClusteredBarItems = function () {
    var items = [];
    items.push(this.Item("StiClusteredBarSeries", this.loc.Chart.ClusteredBar, "Charts.Small.ClusteredBar.png", "StiClusteredBarSeries"));
    items.push(this.Item("StiStackedBarSeries", this.loc.Chart.StackedBar, "Charts.Small.StackedBar.png", "StiStackedBarSeries"));
    items.push(this.Item("StiFullStackedBarSeries", this.loc.Chart.FullStackedBar, "Charts.Small.FullStackedBar.png", "StiFullStackedBarSeries"));
    items.push(this.Item("StiGanttSeries", this.loc.Chart.Gantt, "Charts.Small.Gantt.png", "StiGanttSeries"));

    return items;
}

StiMobileDesigner.prototype.GetChartScatterItems = function () {
    var items = [];
    items.push(this.Item("StiScatterSeries", this.loc.Chart.Scatter, "Charts.Small.Scatter.png", "StiScatterSeries"));
    items.push(this.Item("StiScatterLineSeries", this.loc.Chart.ScatterLine, "Charts.Small.ScatterLine.png", "StiScatterLineSeries"));
    items.push(this.Item("StiScatterSplineSeries", this.loc.Chart.ScatterSpline, "Charts.Small.ScatterSpline.png", "StiScatterSplineSeries"));
    items.push(this.Item("StiBubbleSeries", this.loc.Chart.Bubble, "Charts.Small.Bubble.png", "StiBubbleSeries"));

    return items;
}

StiMobileDesigner.prototype.GetChartPieItems = function () {
    var items = [];
    items.push(this.Item("StiPieSeries", this.loc.Chart.Pie, "Charts.Small.Pie.png", "StiPieSeries"));
    items.push(this.Item("StiPie3dSeries", "3D " + this.loc.Chart.Pie, "Charts.Small.Pie3d.png", "StiPie3dSeries"));
    items.push(this.Item("StiDoughnutSeries", this.loc.Chart.Doughnut, "Charts.Small.Doughnut.png", "StiDoughnutSeries"));

    return items;
}

StiMobileDesigner.prototype.GetChartRadarItems = function () {
    var items = [];
    items.push(this.Item("StiRadarPointSeries", this.loc.Chart.RadarPoint, "Charts.Small.RadarPoint.png", "StiRadarPointSeries"));
    items.push(this.Item("StiRadarLineSeries", this.loc.Chart.RadarLine, "Charts.Small.RadarLine.png", "StiRadarLineSeries"));
    items.push(this.Item("StiRadarAreaSeries", this.loc.Chart.RadarArea, "Charts.Small.RadarArea.png", "StiRadarAreaSeries"));

    return items;
}

StiMobileDesigner.prototype.GetChartFunnelItems = function () {
    var items = [];
    items.push(this.Item("StiFunnelSeries", this.loc.Chart.Funnel, "Charts.Small.Funnel.png", "StiFunnelSeries"));
    if (!this.options.isJava)
        items.push(this.Item("StiFunnelWeightedSlicesSeries", this.loc.Chart.FunnelWeightedSlices, "Charts.Small.FunnelWeightedSlices.png", "StiFunnelWeightedSlicesSeries"));

    return items;
}

StiMobileDesigner.prototype.GetChartFinancialItems = function () {
    var items = [];
    items.push(this.Item("StiCandlestickSeries", this.loc.Chart.Candlestick, "Charts.Small.Candlestick.png", "StiCandlestickSeries"));
    items.push(this.Item("StiStockSeries", this.loc.Chart.Stock, "Charts.Small.Stock.png", "StiStockSeries"));

    return items;
}

StiMobileDesigner.prototype.GetChartTreemapItems = function () {
    var items = [];
    items.push(this.Item("StiTreemapSeries", this.loc.Chart.Treemap, "Charts.Small.Treemap.png", "StiTreemapSeries"));

    return items;
}

StiMobileDesigner.prototype.GetChartSunburstItems = function () {
    var items = [];
    items.push(this.Item("StiSunburstSeries", this.loc.Chart.Sunburst, "Charts.Small.Sunburst.png", "StiSunburstSeries"));

    return items;
}

StiMobileDesigner.prototype.GetChartHistogramItems = function () {
    var items = [];
    items.push(this.Item("StiHistogramSeries", this.loc.Chart.Histogram, "Charts.Small.Histogram.png", "StiHistogramSeries"));
    items.push(this.Item("StiParetoSeries", this.loc.Chart.Pareto, "Charts.Small.Pareto.png", "StiParetoSeries"));
    items.push(this.Item("StiRibbonSeries", this.loc.Chart.Ribbon, "Charts.Small.Ribbon.png", "StiRibbonSeries"));

    return items;
}

StiMobileDesigner.prototype.GetChartWaterfallItems = function () {
    var items = [];
    items.push(this.Item("StiWaterfallSeries", this.loc.Chart.Waterfall, "Charts.Small.Waterfall.png", "StiWaterfallSeries"));

    return items;
}

StiMobileDesigner.prototype.GetChartPictorialItems = function () {
    var items = [];
    items.push(this.Item("StiPictorialSeries", this.loc.Chart.Pictorial, "Charts.Small.Pictorial.png", "StiPictorialSeries"));
    items.push(this.Item("StiPictorialStackedSeries", this.loc.Chart.PictorialStacked, "Charts.Small.PictorialStacked.png", "StiPictorialStackedSeries"));

    return items;
}

StiMobileDesigner.prototype.GetChartBoxAndWhiskerItems = function () {
    var items = [];
    items.push(this.Item("StiBoxAndWhiskerSeries", this.loc.Chart.BoxAndWhisker, "Charts.Small.BoxAndWhisker.png", "StiBoxAndWhiskerSeries"));

    return items;
}

StiMobileDesigner.prototype.GetSortByItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyMain.Value, null, "Value"));
    items.push(this.Item("item1", this.loc.PropertyMain.Argument, null, "Argument"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiCheckStyleNone, null, "None"));

    return items;
}

StiMobileDesigner.prototype.GetChartSortDirectionItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiSortDirectionAsc, null, "Ascending"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiSortDirectionDesc, null, "Descending"));

    return items;
}

StiMobileDesigner.prototype.GetShowSeriesLabelsItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiShowSeriesLabelsFromChart, null, "FromChart"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiShowSeriesLabelsFromSeries, null, "FromSeries"));

    return items;
}

StiMobileDesigner.prototype.GetYAxisItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiSeriesYAxisLeftYAxis, null, "LeftYAxis"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiSeriesYAxisRightYAxis, null, "RightYAxis"));

    return items;
}

StiMobileDesigner.prototype.GetXAxisItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiSeriesXAxisBottomXAxis, null, "BottomXAxis"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiSeriesXAxisTopXAxis, null, "TopXAxis"));

    return items;
}

StiMobileDesigner.prototype.GetMarkerTypeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiMarkerTypeRectangle, null, "Rectangle"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiMarkerTypeTriangle, null, "Triangle"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiMarkerTypeCircle, null, "Circle"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StiMarkerTypeHalfCircle, null, "HalfCircle"));
    items.push(this.Item("item4", this.loc.PropertyEnum.StiMarkerTypeStar5, null, "Star5"));
    items.push(this.Item("item5", this.loc.PropertyEnum.StiMarkerTypeStar6, null, "Star6"));
    items.push(this.Item("item6", this.loc.PropertyEnum.StiMarkerTypeStar7, null, "Star7"));
    items.push(this.Item("item7", this.loc.PropertyEnum.StiMarkerTypeStar8, null, "Star8"));
    items.push(this.Item("item8", this.loc.PropertyEnum.StiMarkerTypeHexagon, null, "Hexagon"));

    return items;
}

StiMobileDesigner.prototype.GetMarkerVisibleItems = function () {
    var items = [];
    items.push(this.Item("True", this.loc.PropertyEnum.StiExtendedStyleBoolTrue, null, "True"));
    items.push(this.Item("False", this.loc.PropertyEnum.StiExtendedStyleBoolFalse, null, "False"));
    items.push(this.Item("FromStyle", this.loc.PropertyEnum.StiExtendedStyleBoolFromStyle, null, "FromStyle"));

    return items;
}

StiMobileDesigner.prototype.GetTopNModeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiCheckStyleNone, null, "None"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiBorderSidesTop, null, "Top"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiBorderSidesBottom, null, "Bottom"));

    return items;
}

StiMobileDesigner.prototype.GetRadarStyleItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiRadarStyleXFPolygon, null, "Polygon"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiRadarStyleXFCircle, null, "Circle"));

    return items;
}

StiMobileDesigner.prototype.GetXAxisArrowStyleItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiCheckStyleNone, null, "None"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiArrowStyleTriangle, null, "Triangle"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiArrowStyleLines, null, "Lines"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StiArrowStyleCircle, null, "Circle"));
    items.push(this.Item("item4", this.loc.PropertyEnum.StiArrowStyleArc, null, "Arc"));
    items.push(this.Item("item5", this.loc.PropertyEnum.StiArrowStyleArcAndCircle, null, "ArcAndCircle"));

    return items;
}

StiMobileDesigner.prototype.GetXAxisStepItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiCheckStyleNone, null, "None"));
    items.push(this.Item("item1", "Second", null, "Second"));
    items.push(this.Item("item2", "Minute", null, "Minute"));
    items.push(this.Item("item3", "Hour", null, "Hour"));
    items.push(this.Item("item4", "Day", null, "Day"));
    items.push(this.Item("item5", "Month", null, "Month"));
    items.push(this.Item("item6", "Year", null, "Year"));

    return items;
}

StiMobileDesigner.prototype.GetXAxisPlacementItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiCheckStyleNone, null, "None"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiLabelsPlacementOneLine, null, "OneLine"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiLabelsPlacementTwoLines, null, "TwoLines"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StiLabelsPlacementAutoRotation, null, "AutoRotation"));

    return items;
}

StiMobileDesigner.prototype.GetDockItems = function () {
    var items = [];

    items.push(this.Item("item0", this.loc.PropertyEnum.StiVertAlignmentTop, null, "Top"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiTextHorAlignmentRight, null, "Right"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiVertAlignmentBottom, null, "Bottom"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StiTextHorAlignmentLeft, null, "Left"));

    return items;
}

StiMobileDesigner.prototype.GetXAxisTextAlignmentItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiTextHorAlignmentLeft, null, "Left"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiTextHorAlignmentRight, null, "Right"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiTextHorAlignmentCenter, null, "Center"));

    return items;
}

StiMobileDesigner.prototype.GetXAxisTitleAlignmentItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StringAlignmentNear, null, "Near"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StringAlignmentCenter, null, "Center"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StringAlignmentFar, null, "Far"));

    return items;
}

StiMobileDesigner.prototype.GetXAxisTitleDirectionItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiLegendDirectionLeftToRight, null, "LeftToRight"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiLegendDirectionRightToLeft, null, "RightToLeft"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiLegendDirectionTopToBottom, null, "TopToBottom"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StiLegendDirectionBottomToTop, null, "BottomToTop"));

    return items;
}

StiMobileDesigner.prototype.GetXAxisTitlePositionItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiTitlePositionInside, null, "Inside"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiTitlePositionOutside, null, "Outside"));

    return items;
}

StiMobileDesigner.prototype.GetTargetPlacementPositionItems = function () {
    var items = [];
    items.push(this.Item("item2", this.loc.Chart.LabelsOutside, null, "Outside"));
    items.push(this.Item("item1", this.loc.Chart.LabelsOverlay, null, "Overlay"));
    items.push(this.Item("item0", this.loc.Chart.LabelsInside, null, "Inside"));

    return items;
}

StiMobileDesigner.prototype.GetShowXAxisItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiShowXAxisBottom, null, "Bottom"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiShowYAxisCenter, null, "Center"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiShowYAxisBoth, null, "Both"));

    return items;
}

StiMobileDesigner.prototype.GetLabelsPositionItems = function (chartElement) {
    var items = [];
    items.push(this.Item("None", this.loc.Chart.LabelsNone, null, "None"));

    if (chartElement) {
        items.push("separator1");
        if (chartElement.isAxisAreaChart || chartElement.isPieChart || chartElement.isWaterfallChart)
            items.push(this.Item("InsideEnd", this.loc.Chart.LabelsInsideEnd, null, "InsideEnd"));
        if (chartElement.isAxisAreaChart || chartElement.isWaterfallChart)
            items.push(this.Item("InsideBase", this.loc.Chart.LabelsInsideBase, null, "InsideBase"));
        if (chartElement.isPieChart || chartElement.isFunnelChart || chartElement.isTreemapChart || chartElement.isDoughnutChart || chartElement.isAxisAreaChart ||
            chartElement.isWaterfallChart || chartElement.isSunburstChart || chartElement.isRadarChart || chartElement.isPictorialStackedChart || chartElement.isAxisAreaChart3D)
            items.push(this.Item("Center", this.loc.Chart.LabelsCenter, null, "Center"));
        if (chartElement.isAxisAreaChart || chartElement.isWaterfallChart)
            items.push(this.Item("OutsideEnd", this.loc.Chart.LabelsOutsideEnd, null, "OutsideEnd"));
        if (chartElement.isAxisAreaChart || chartElement.isWaterfallChart)
            items.push(this.Item("OutsideBase", this.loc.Chart.LabelsOutsideBase, null, "OutsideBase"));
        if (chartElement.isAxisAreaChart || chartElement.isPieChart || chartElement.isWaterfallChart || chartElement.isClusteredColumnChart3D)
            items.push(this.Item("Outside", this.loc.Chart.LabelsOutside, null, "Outside"));
        if (chartElement.isAxisAreaChart || chartElement.isPieChart)
            items.push("separator2");
        if (chartElement.isAxisAreaChart || chartElement.isPictorialStackedChart)
            items.push(this.Item("Left", this.loc.PropertyMain.Left, null, "Left"));
        if (chartElement.isAxisAreaChart)
            items.push(this.Item("Value", this.loc.PropertyMain.Value, null, "Value"));
        if (chartElement.isAxisAreaChart || chartElement.isPictorialStackedChart)
            items.push(this.Item("Right", this.loc.PropertyMain.Right, null, "Right"));
        if (chartElement.isPieChart)
            items.push(this.Item("TwoColumns", this.loc.Chart.LabelsTwoColumns, null, "TwoColumns"));
        if (chartElement.isStackedChart) {
            items.push("separator3");
            items.push(this.Item("Total", this.loc.PropertyMain.Total, null, "Total"));
        }
    }
    else {
        items.push(this.Item("Center", this.loc.Chart.LabelsCenter, null, "Center"));
        items.push(this.Item("InsideEnd", this.loc.Chart.LabelsInsideEnd, null, "InsideEnd"));
        items.push(this.Item("Outside", this.loc.Chart.LabelsOutside, null, "Outside"));
        items.push(this.Item("TwoColumns", this.loc.Chart.LabelsTwoColumns, null, "TwoColumns"));
    }

    return items;
}

StiMobileDesigner.prototype.GetLabelsStyleItems = function (chartElement) {
    var items = [];
    items.push(this.Item("Value", this.loc.PropertyEnum.StiChartLabelsStyleValue, null, "Value"));
    if (chartElement && (chartElement.isPieChart || chartElement.isDoughnutChart))
        items.push(this.Item("PercentOfTotal", this.loc.PropertyEnum.StiChartLabelsStylePercentOfTotal, null, "PercentOfTotal"));
    items.push(this.Item("Category", this.loc.PropertyEnum.StiChartLabelsStyleCategory, null, "Category"));
    items.push(this.Item("CategoryValue", this.loc.PropertyEnum.StiChartLabelsStyleCategoryValue, null, "CategoryValue"));
    if (chartElement && (chartElement.isPieChart || chartElement.isDoughnutChart))
        items.push(this.Item("CategoryPercentOfTotal", this.loc.PropertyEnum.StiChartLabelsStyleCategoryPercentOfTotal, null, "CategoryPercentOfTotal"));

    return items;
}

StiMobileDesigner.prototype.GetLegendValueTypeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiSeriesLabelsValueTypeValue, null, "Value"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiSeriesLabelsValueTypeSeriesTitle, null, "SeriesTitle"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiSeriesLabelsValueTypeArgument, null, "Argument"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StiSeriesLabelsValueTypeTag, null, "Tag"));
    items.push(this.Item("item4", this.loc.PropertyEnum.StiSeriesLabelsValueTypeWeight, null, "Weight"));
    items.push(this.Item("item5", this.loc.PropertyEnum.StiSeriesLabelsValueTypeValueArgument, null, "ValueArgument"));
    items.push(this.Item("item6", this.loc.PropertyEnum.StiSeriesLabelsValueTypeArgumentValue, null, "ArgumentValue"));
    items.push(this.Item("item7", this.loc.PropertyEnum.StiSeriesLabelsValueTypeSeriesTitleValue, null, "SeriesTitleValue"));
    items.push(this.Item("item8", this.loc.PropertyEnum.StiSeriesLabelsValueTypeSeriesTitleArgument, null, "SeriesTitleArgument"));

    return items;
}

StiMobileDesigner.prototype.GetDrillDownPageItems = function () {
    var items = [];
    if (this.options.report != null) {
        items.push(this.Item("NotAssigned", this.loc.Report.NotAssigned, null, ""));
        for (var pageName in this.options.report.pages) {
            if (pageName != this.options.selectedObject.properties.pageName) {
                var page = this.options.report.pages[pageName];
                var text = page.properties.aliasName && StiBase64.decode(page.properties.aliasName) != pageName ? pageName + " [" + StiBase64.decode(page.properties.aliasName) + "]" : pageName;
                items.push(this.Item(pageName, text, null, pageName));
            }
        }
    }

    return items;
}

StiMobileDesigner.prototype.GetLegendHorAlignmentItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiLegendHorAlignmentLeftOutside, null, "LeftOutside"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiLegendHorAlignmentLeft, null, "Left"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiLegendHorAlignmentCenter, null, "Center"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StiLegendHorAlignmentRight, null, "Right"));
    items.push(this.Item("item4", this.loc.PropertyEnum.StiLegendHorAlignmentRightOutside, null, "RightOutside"));

    return items;
}

StiMobileDesigner.prototype.GetLegendVertAlignmentItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiLegendVertAlignmentTopOutside, null, "TopOutside"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiLegendVertAlignmentTop, null, "Top"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiLegendVertAlignmentCenter, null, "Center"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StiLegendVertAlignmentBottom, null, "Bottom"));
    items.push(this.Item("item4", this.loc.PropertyEnum.StiLegendVertAlignmentBottomOutside, null, "BottomOutside"));

    return items;
}

StiMobileDesigner.prototype.GetLegendLabelsValueTypeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyMain.Auto, null, "Auto"));
    items.push(this.Item("item0", this.loc.PropertyEnum.StiSeriesLabelsValueTypeArgument, null, "Argument"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiSeriesLabelsValueTypeArgumentValue, null, "ArgumentValue"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiSeriesLabelsValueTypeSeriesTitle, null, "SeriesTitle"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StiSeriesLabelsValueTypeSeriesTitleArgument, null, "SeriesTitleArgument"));
    items.push(this.Item("item4", this.loc.PropertyEnum.StiSeriesLabelsValueTypeSeriesTitleValue, null, "SeriesTitleValue"));
    items.push(this.Item("item5", this.loc.PropertyEnum.StiSeriesLabelsValueTypeValue, null, "Value"));
    items.push(this.Item("item6", this.loc.PropertyEnum.StiSeriesLabelsValueTypeValueArgument, null, "ValueArgument"));

    return items;
}

StiMobileDesigner.prototype.GetLegendDirectionItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiLegendDirectionLeftToRight, null, "LeftToRight"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiLegendDirectionRightToLeft, null, "RightToLeft"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiLegendDirectionTopToBottom, null, "TopToBottom"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StiLegendDirectionBottomToTop, null, "BottomToTop"));

    return items;
}

StiMobileDesigner.prototype.GetConstantLinesOrientationItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiOrientationHorizontal, null, "Horizontal"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiOrientationVertical, null, "Vertical"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiOrientationHorizontalRight, null, "HorizontalRight"));

    return items;
}

StiMobileDesigner.prototype.GetConstantLinesPositionItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiTextPositionLeftTop, null, "LeftTop"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiTextPositionLeftBottom, null, "LeftBottom"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiTextPositionCenterTop, null, "CenterTop"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StiTextPositionCenterBottom, null, "CenterBottom"));
    items.push(this.Item("item4", this.loc.PropertyEnum.StiTextPositionRightTop, null, "RightTop"));
    items.push(this.Item("item5", this.loc.PropertyEnum.StiTextPositionRightBottom, null, "RightBottom"));

    return items;
}

StiMobileDesigner.prototype.GetTrendLinesPositionItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiTextPositionLeftTop, null, "LeftTop"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiTextPositionLeftBottom, null, "LeftBottom"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiTextPositionRightTop, null, "RightTop"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StiTextPositionRightBottom, null, "RightBottom"));

    return items;
}

StiMobileDesigner.prototype.GetConditionsFieldIsItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyMain.Value, null, "Value"));
    items.push(this.Item("item1", this.loc.PropertyMain.Argument, null, "Argument"));

    return items;
}

StiMobileDesigner.prototype.GetConditionsDataTypeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiFilterDataTypeString, null, "String"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiFilterDataTypeNumeric, null, "Numeric"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiFilterDataTypeDateTime, null, "DateTime"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StiFilterDataTypeBoolean, null, "Boolean"));

    return items;
}

StiMobileDesigner.prototype.GetFiltersFieldIsItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiFilterItemValue, null, "Value"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiFilterItemArgument, null, "Argument"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiFilterItemExpression, null, "Expression"));

    return items;
}

StiMobileDesigner.prototype.GetSeriesGanttFiltersFieldIsItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiFilterItemValue, null, "Value"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiFilterItemValueEnd, null, "ValueEnd"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StiFilterItemArgument, null, "Argument"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiFilterItemExpression, null, "Expression"));

    return items;
}

StiMobileDesigner.prototype.GetSeriesFinancialFiltersFieldIsItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiFilterItemValueOpen, null, "ValueOpen"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiFilterItemValueClose, null, "ValueClose"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiFilterItemValueLow, null, "ValueLow"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StiFilterItemValueHigh, null, "ValueHigh"));
    items.push(this.Item("item4", this.loc.PropertyEnum.StiFilterItemArgument, null, "Argument"));
    items.push(this.Item("item5", this.loc.PropertyEnum.StiFilterItemExpression, null, "Expression"));

    return items;
}

StiMobileDesigner.prototype.GetChartStyleBrushTypeItems = function () {
    var items = [];
    items.push(this.Item("StiBrushTypeGlare", this.loc.PropertyEnum.StiBrushTypeGlare, null, "Glare"));
    items.push(this.Item("StiBrushTypeGradient0", this.loc.PropertyEnum.StiBrushTypeGradient0, null, "Gradient0"));
    items.push(this.Item("StiBrushTypeGradient180", this.loc.PropertyEnum.StiBrushTypeGradient180, null, "Gradient180"));
    items.push(this.Item("StiBrushTypeGradient270", this.loc.PropertyEnum.StiBrushTypeGradient270, null, "Gradient270"));
    items.push(this.Item("StiBrushTypeGradient45", this.loc.PropertyEnum.StiBrushTypeGradient45, null, "Gradient45"));
    items.push(this.Item("StiBrushTypeGradient90", this.loc.PropertyEnum.StiBrushTypeGradient90, null, "Gradient90"));
    items.push(this.Item("StiBrushTypeSolid", this.loc.PropertyEnum.StiBrushTypeSolid, null, "Solid"));

    return items;
}

StiMobileDesigner.prototype.GetDataBandItems = function () {
    if (this.options.report == null || (!this.options.selectedObjects && !this.options.selectedObject)) return null;
    var items = [];
    items.push(this.Item("NotAssigned", this.loc.Report.NotAssigned, null, "NotAssigned"));
    var pageName = this.options.selectedObjects ? this.options.selectedObjects[0].properties.pageName : this.options.selectedObject.properties.pageName;
    var page = pageName ? this.options.report.pages[pageName] : this.options.currentPage;
    if (!page) return;

    for (var componentName in page.components) {
        var component = page.components[componentName];
        if (component.typeComponent == "StiDataBand" ||
            component.typeComponent == "StiCrossDataBand" ||
            component.typeComponent == "StiGroupHeaderBand" ||
            component.typeComponent == "StiCrossGroupHeaderBand" ||
            component.typeComponent == "StiHierarchicalBand" ||
            component.typeComponent == "StiTable" ||
            component.typeComponent == "StiCrossTab")
            items.push(this.Item(componentName, componentName, null, componentName));
    }

    return items;
}

StiMobileDesigner.prototype.GetAddConditionMenuItems = function () {
    var items = [];
    items.push(this.Item("StiHighlightCondition", this.loc.PropertyMain.HighlightCondition, "Conditions.Highlight.png", "StiHighlightCondition"));
    items.push(this.Item("StiDataBarCondition", this.loc.PropertyMain.DataBarCondition, "Conditions.DataBar.png", "StiDataBarCondition"));
    items.push(this.Item("StiColorScaleCondition", this.loc.PropertyMain.ColorScaleCondition, "Conditions.ColorScale.png", "StiColorScaleCondition"));
    items.push(this.Item("StiIconSetCondition", this.loc.PropertyMain.IconSetCondition, "Conditions.IconSet.png", "StiIconSetCondition"));

    return items;
}

StiMobileDesigner.prototype.GetFontSizeItems = function () {
    var sizeItems = [];
    for (var i = 0; i < this.options.fontSizes.length; i++) {
        sizeItems.push(this.Item("homeSizesFont" + i, this.options.fontSizes[i], null, this.options.fontSizes[i]));
    }

    return sizeItems;
}

StiMobileDesigner.prototype.GetConditionsMinimumTypeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyMain.Auto, null, "Auto"));
    items.push(this.Item("item1", this.loc.FormFormatEditor.Percentage, null, "Percent"));
    items.push(this.Item("item2", this.loc.PropertyMain.Value, null, "Value"));
    items.push(this.Item("item3", this.loc.PropertyMain.Minimum, null, "Minimum"));

    return items;
}

StiMobileDesigner.prototype.GetConditionsMaximumTypeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyMain.Auto, null, "Auto"));
    items.push(this.Item("item1", this.loc.FormFormatEditor.Percentage, null, "Percent"));
    items.push(this.Item("item2", this.loc.PropertyMain.Value, null, "Value"));
    items.push(this.Item("item3", this.loc.PropertyMain.Maximum, null, "Maximum"));

    return items;
}

StiMobileDesigner.prototype.GetConditionsDirectionItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyMain.Default, null, "Default"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiLegendDirectionLeftToRight, null, "LeftToRight"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiLegendDirectionRightToLeft, null, "RighToLeft"));

    return items;
}

StiMobileDesigner.prototype.GetConditionsBrushTypeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.Report.StiSolidBrush, null, "Solid"));
    items.push(this.Item("item1", this.loc.Report.StiGradientBrush, null, "Gradient"));

    return items;
}

StiMobileDesigner.prototype.GetConditionsShowBordersItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiBorderSidesNone, null, "None"));
    items.push(this.Item("item1", this.loc.Report.StiSolidBrush, null, "Solid"));

    return items;
}

StiMobileDesigner.prototype.GetConditionsColorScaleTypeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiColorScaleTypeColor2, null, "Color2"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiColorScaleTypeColor3, null, "Color3"));

    return items;
}

StiMobileDesigner.prototype.GetConditionsValueTypeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyMain.Auto, null, "Auto"));
    items.push(this.Item("item1", this.loc.FormFormatEditor.Percentage, null, "Percent"));
    items.push(this.Item("item2", this.loc.PropertyMain.Value, null, "Value"));

    return items;
}

StiMobileDesigner.prototype.GetConditionsAlignmentItems = function () {
    var imgAlignArray = ["TopLeft", "TopCenter", "TopRight", "MiddleLeft", "MiddleCenter", "MiddleRight", "BottomLeft", "BottomCenter", "BottomRight"];
    var imgAlignItems = [];
    for (var i = 0; i < imgAlignArray.length; i++) {
        imgAlignItems.push(this.Item("AlignItem" + i, this.loc.PropertyEnum["ContentAlignment" + imgAlignArray[i]],
            null, imgAlignArray[i]));
        if (i == 2 || i == 5) imgAlignItems.push("separator" + i);
    }

    return imgAlignItems;
}

StiMobileDesigner.prototype.GetConditionsOperationItems = function () {
    var items = [];
    items.push(this.Item("item0", ">=", null, "MoreThanOrEqual"));
    items.push(this.Item("item1", ">", null, "MoreThan"));

    return items;
}

StiMobileDesigner.prototype.GetConditionsIconSetValueTypeItems = function () {
    var items = [];
    items.push(this.Item("item1", this.loc.FormFormatEditor.Percentage, null, "Percent"));
    items.push(this.Item("item2", this.loc.PropertyMain.Value, null, "Value"));

    return items;
}

StiMobileDesigner.prototype.GetCalculationModeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiCalculationModeCompilation, null, "Compilation"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiCalculationModeInterpretation, null, "Interpretation"));

    return items;
}

StiMobileDesigner.prototype.GetEngineVersionItems = function () {
    var items = [];
    items.push(this.Item("item0", "EngineV1", null, "EngineV1"));
    items.push(this.Item("item1", "EngineV2", null, "EngineV2"));

    return items;
}

StiMobileDesigner.prototype.GetNumberOfPassItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiNumberOfPassSinglePass, null, "SinglePass"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiNumberOfPassDoublePass, null, "DoublePass"));

    return items;
}

StiMobileDesigner.prototype.GetReportCacheModeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiReportCacheModeOff, null, "Off"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiReportCacheModeOn, null, "On"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiReportCacheModeAuto, null, "Auto"));

    return items;
}

StiMobileDesigner.prototype.GetPreviewModeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiPreviewModeStandard, null, "Standard"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiPreviewModeStandardAndDotMatrix, null, "StandardAndDotMatrix"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiPreviewModeDotMatrix, null, "DotMatrix"));

    return items;
}

StiMobileDesigner.prototype.GetParametersOrientationItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiOrientationHorizontal, null, "Horizontal"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiOrientationVertical, null, "Vertical"));

    return items;
}

StiMobileDesigner.prototype.GetParametersDateFormatItems = function () {
    var items = [];
    items.push(this.Item("item0", "MM/dd/yyyy", null, "MM/dd/yyyy"));
    items.push(this.Item("item1", "MM/dd/yyyy h:mm:ss tt", null, "MM/dd/yyyy h:mm:ss tt"));
    items.push(this.Item("item2", "dd.MM.yyyy", null, "dd.MM.yyyy"));
    items.push(this.Item("item3", "dd.MM.yyyy HH:mm:ss", null, "dd.MM.yyyy HH:mm:ss"));
    items.push(this.Item("item4", "yyyy/MM/dd", null, "yyyy/MM/dd"));
    items.push(this.Item("item5", "yyyy-MM-dd HH:mm:ss", null, "yyyy-MM-dd HH:mm:ss"));
    items.push(this.Item("item6", "dd/MM/yyyy", null, "dd/MM/yyyy"));
    items.push(this.Item("item7", "dd/MM/yyyy HH:mm:ss", null, "dd/MM/yyyy HH:mm:ss"));
    items.push(this.Item("item8", "dd-MM-yyyy HH.mm.ss", null, "dd-MM-yyyy HH.mm.ss"));

    return items;
}

StiMobileDesigner.prototype.GetScriptLanguageItems = function () {
    var items = [];
    items.push(this.Item("item0", "CSharp", null, "CSharp"));
    items.push(this.Item("item1", "VB", null, "VB"));

    return items;
}

StiMobileDesigner.prototype.GetTrimmingItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StringTrimmingNone, null, "None"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StringTrimmingCharacter, null, "Character"));
    items.push(this.Item("item5", this.loc.PropertyEnum.StringTrimmingWord, null, "Word"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StringTrimmingEllipsisCharacter, null, "EllipsisCharacter"));
    items.push(this.Item("item4", this.loc.PropertyEnum.StringTrimmingEllipsisWord, null, "EllipsisWord"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StringTrimmingEllipsisPath, null, "EllipsisPath"));

    return items;
}

StiMobileDesigner.prototype.GetProcessAtItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiProcessAtNone, null, "None"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiProcessAtEndOfReport, null, "EndOfReport"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiProcessAtEndOfPage, null, "EndOfPage"));

    return items;
}

StiMobileDesigner.prototype.GetProcessingDuplicatesItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiProcessingDuplicatesTypeNone, null, "None"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiProcessingDuplicatesTypeMerge, null, "Merge"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiProcessingDuplicatesTypeHide, null, "Hide"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StiProcessingDuplicatesTypeRemoveText, null, "RemoveText"));
    items.push(this.Item("item4", this.loc.PropertyEnum.StiProcessingDuplicatesTypeBasedOnTagMerge, null, "BasedOnTagMerge"));
    items.push(this.Item("item5", this.loc.PropertyEnum.StiProcessingDuplicatesTypeBasedOnTagHide, null, "BasedOnTagHide"));
    items.push(this.Item("item6", this.loc.PropertyEnum.StiProcessingDuplicatesTypeBasedOnTagRemoveText, null, "BasedOnTagRemoveText"));
    items.push(this.Item("item7", this.loc.PropertyEnum.StiProcessingDuplicatesTypeGlobalRemoveText, null, "GlobalRemoveText"));
    items.push(this.Item("item8", this.loc.PropertyEnum.StiProcessingDuplicatesTypeGlobalMerge, null, "GlobalMerge"));
    items.push(this.Item("item9", this.loc.PropertyEnum.StiProcessingDuplicatesTypeGlobalHide, null, "GlobalHide"));
    items.push(this.Item("item10", this.loc.PropertyEnum.StiProcessingDuplicatesTypeBasedOnValueRemoveText, null, "BasedOnValueRemoveText"));
    items.push(this.Item("item11", this.loc.PropertyEnum.StiProcessingDuplicatesTypeBasedOnValueAndTagMerge, null, "BasedOnValueAndTagMerge"));
    items.push(this.Item("item12", this.loc.PropertyEnum.StiProcessingDuplicatesTypeBasedOnValueAndTagHide, null, "BasedOnValueAndTagHide"));
    items.push(this.Item("item13", this.loc.PropertyEnum.StiProcessingDuplicatesTypeGlobalBasedOnValueAndTagMerge, null, "GlobalBasedOnValueAndTagMerge"));
    items.push(this.Item("item14", this.loc.PropertyEnum.StiProcessingDuplicatesTypeGlobalBasedOnValueAndTagHide, null, "GlobalBasedOnValueAndTagHide"));
    items.push(this.Item("item15", this.loc.PropertyEnum.StiProcessingDuplicatesTypeGlobalBasedOnValueRemoveText, null, "GlobalBasedOnValueRemoveText"));

    return items;
}

StiMobileDesigner.prototype.GetXmlTypeItems = function () {
    var items = [];
    items.push(this.Item("item0", "ADO.NET XML", null, "AdoNetXml"));
    items.push(this.Item("item1", "XML", null, "Xml"));

    return items;
}

StiMobileDesigner.prototype.GetAutoSaveItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.FormOptions.Minutes.replace("{0}", "5"), null, "5"));
    items.push(this.Item("item1", this.loc.FormOptions.Minutes.replace("{0}", "10"), null, "10"));
    items.push(this.Item("item2", this.loc.FormOptions.Minutes.replace("{0}", "15"), null, "15"));
    items.push(this.Item("item3", this.loc.FormOptions.Minutes.replace("{0}", "20"), null, "20"));
    items.push(this.Item("item4", this.loc.FormOptions.Minutes.replace("{0}", "30"), null, "30"));
    items.push(this.Item("item5", this.loc.FormOptions.Minutes.replace("{0}", "60"), null, "60"));

    return items;
}

StiMobileDesigner.prototype.GetDBaseCodePageItems = function () {
    var items = [];
    for (var i = 0; i < this.options.dBaseCodePages.length; i++) {
        items.push(this.Item("item" + i, this.options.dBaseCodePages[i].name, null, this.options.dBaseCodePages[i].key.toString()));
    }

    return items;
}

StiMobileDesigner.prototype.GetCsvCodePageItems = function () {
    var items = [];
    for (var i = 0; i < this.options.csvCodePages.length; i++) {
        items.push(this.Item("item" + i, this.options.csvCodePages[i].name, null, this.options.csvCodePages[i].key.toString()));
    }

    return items;
}

StiMobileDesigner.prototype.GetCsvSeparatorItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.FormDictionaryDesigner.CsvSeparatorSystem, null, "System"));
    items.push(this.Item("item1", this.loc.FormDictionaryDesigner.CsvSeparatorTab, null, "Tab"));
    items.push(this.Item("item2", this.loc.FormDictionaryDesigner.CsvSeparatorSemicolon, null, "Semicolon"));
    items.push(this.Item("item3", this.loc.FormDictionaryDesigner.CsvSeparatorComma, null, "Comma"));
    items.push(this.Item("item4", this.loc.FormDictionaryDesigner.CsvSeparatorSpace, null, "Space"));
    items.push(this.Item("item5", this.loc.FormDictionaryDesigner.CsvSeparatorOther, null, "Other"));

    return items;
}

StiMobileDesigner.prototype.GetParameterTypeItems = function (parameterTypes) {
    if (!parameterTypes) return [];
    var items = [];

    for (var i = 0; i < parameterTypes.length; i++) {
        items.push(this.Item("item" + i, parameterTypes[i].typeName, null, parameterTypes[i].typeValue.toString()));
    }
    return items;
}

StiMobileDesigner.prototype.GetLayoutAlignItems = function (isContextMenu) {
    var items = [];
    if (isContextMenu) {
        items.push(this.Item("AlignToGrid", this.loc.Toolbars.AlignToGrid, "ContextMenu.AlignToGrid.png", "AlignToGrid"));
        items.push("separator0");
    }
    items.push(this.Item("AlignLeft", this.loc.Toolbars.AlignLeft, "Layout.AlignLeft.png", "AlignLeft"));
    items.push(this.Item("AlignCenter", this.loc.Toolbars.AlignCenter, "Layout.AlignCenter.png", "AlignCenter"));
    items.push(this.Item("AlignRight", this.loc.Toolbars.AlignRight, "Layout.AlignRight.png", "AlignRight"));
    items.push("separator1");
    items.push(this.Item("AlignTop", this.loc.Toolbars.AlignTop, "Layout.AlignTop.png", "AlignTop"));
    items.push(this.Item("AlignMiddle", this.loc.Toolbars.AlignMiddle, "Layout.AlignMiddle.png", "AlignMiddle"));
    items.push(this.Item("AlignBottom", this.loc.Toolbars.AlignBottom, "Layout.AlignBottom.png", "AlignBottom"));
    items.push("separator2");
    items.push(this.Item("MakeHorizontalSpacingEqual", this.loc.Toolbars.MakeHorizontalSpacingEqual, "Layout.MakeHorizontalSpacingEqual.png", "MakeHorizontalSpacingEqual"));
    items.push(this.Item("MakeVerticalSpacingEqual", this.loc.Toolbars.MakeVerticalSpacingEqual, "Layout.MakeVerticalSpacingEqual.png", "MakeVerticalSpacingEqual"));
    items.push("separator3");
    items.push(this.Item("CenterHorizontally", this.loc.Toolbars.CenterHorizontally, "Layout.CenterHorizontally.png", "CenterHorizontally"));
    items.push(this.Item("CenterVertically", this.loc.Toolbars.CenterVertically, "Layout.CenterVertically.png", "CenterVertically"));

    return items;
}

StiMobileDesigner.prototype.GetRetrieveColumnsAndParametersItems = function () {
    var items = [];
    items.push(this.Item("retrieveColumnsAndParameters", this.loc.FormDictionaryDesigner.RetrieveColumnsAndParameters, null, "retrieveColumnsAndParameters"));
    items.push(this.Item("retrieveParameters", this.loc.FormDictionaryDesigner.RetrieveParameters, null, "retrieveParameters"));
    items.push("separator");

    return items;
}

StiMobileDesigner.prototype.GetDecimalSeparatorItems = function () {
    var items = [];
    items.push(this.Item("item0", ".", null, "."));
    items.push(this.Item("item1", ",", null, ","));

    return items;
}

StiMobileDesigner.prototype.GetGroupSeparatorItems = function () {
    var items = [];
    items.push(this.Item("item0", "", null, ""));
    items.push(this.Item("item1", ".", null, "."));
    items.push(this.Item("item2", ",", null, ","));

    return items;
}

StiMobileDesigner.prototype.GetNumberNegativePatternItems = function () {
    var items = [];
    items.push(this.Item("item0", "(n)", null, 0));
    items.push(this.Item("item1", "-n", null, 1));
    items.push(this.Item("item2", "- n", null, 2));
    items.push(this.Item("item3", "n-", null, 3));
    items.push(this.Item("item4", "n -", null, 4));

    return items;
}

StiMobileDesigner.prototype.GetCurrencyPositivePatternItems = function () {
    var items = [];
    items.push(this.Item("item0", "$n", null, 0));
    items.push(this.Item("item1", "n$", null, 1));
    items.push(this.Item("item2", "$ n", null, 2));
    items.push(this.Item("item3", "n $", null, 3));

    return items;
}

StiMobileDesigner.prototype.GetCurrencyNegativePatternItems = function () {
    var items = [];
    items.push(this.Item("item0", "($n)", null, 0));
    items.push(this.Item("item1", "-$n", null, 1));
    items.push(this.Item("item2", "$-n", null, 2));
    items.push(this.Item("item3", "$n-", null, 3));
    items.push(this.Item("item4", "(n$)", null, 4));
    items.push(this.Item("item5", "-n$", null, 5));
    items.push(this.Item("item6", "n-$", null, 6));
    items.push(this.Item("item7", "n$-", null, 7));
    items.push(this.Item("item8", "-n $", null, 8));
    items.push(this.Item("item9", "-$ n", null, 9));
    items.push(this.Item("item10", "n $-", null, 10));
    items.push(this.Item("item11", "$ n-", null, 11));
    items.push(this.Item("item12", "$ -n", null, 12));
    items.push(this.Item("item13", "n- $", null, 13));
    items.push(this.Item("item14", "($ n)", null, 14));
    items.push(this.Item("item15", "(n $)", null, 15));

    return items;
}

StiMobileDesigner.prototype.GetPercentagePositivePatternItems = function () {
    var items = [];
    items.push(this.Item("item0", "n %", null, 0));
    items.push(this.Item("item1", "n%", null, 1));
    items.push(this.Item("item2", "%n", null, 2));

    return items;
}

StiMobileDesigner.prototype.GetPercentageNegativePatternItems = function () {
    var items = [];
    items.push(this.Item("item0", "-n %", null, 0));
    items.push(this.Item("item1", "-n%", null, 1));
    items.push(this.Item("item2", "-%n", null, 2));

    return items;
}

StiMobileDesigner.prototype.GetPercentageSymbolItems = function () {
    var items = [];
    items.push(this.Item("item0", "", null, ""));
    items.push(this.Item("item1", "%", null, "%"));

    return items;
}

StiMobileDesigner.prototype.GetCurrencySymbolItems = function () {
    var items = [];
    for (var i = 0; i < this.options.currencySymbols.length; i++) {
        items.push(this.Item("item" + i, this.options.currencySymbols[i], null, this.options.currencySymbols[i]));
    }

    return items;
}

StiMobileDesigner.prototype.GetBooleanFormatItems = function (lower) {
    var items = [];
    items.push(this.Item("item0", "False", null, "False"));
    items.push(this.Item("item1", "True", null, "True"));
    items.push(this.Item("item2", "No", null, "No"));
    items.push(this.Item("item3", "Yes", null, "Yes"));
    items.push(this.Item("item4", "Off", null, "Off"));
    items.push(this.Item("item5", "On", null, "On"));

    if (lower) {
        for (var i = 0; i < items.length; i++) {
            items[i].caption = items[i].caption.toLowerCase();
            items[i].key = items[i].key.toLowerCase();
        }
    }

    return items;
}

StiMobileDesigner.prototype.GetSortDirectionItemsForGroupsControl = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiSortDirectionAsc, null, "ASC"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiSortDirectionDesc, null, "DESC"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiSortDirectionNone, null, "NONE"));

    return items;
}

StiMobileDesigner.prototype.GetResultFunctionItems = function () {
    var items = [];
    items.push(this.Item("No", "No", null, "No"));
    items.push(this.Item("Sum", "Sum", null, "Sum"));
    items.push(this.Item("SumDistinct", "SumDistinct", null, "SumDistinct"));
    items.push(this.Item("Count", "Count", null, "Count"));
    items.push(this.Item("CountDistinct", "CountDistinct", null, "CountDistinct"));
    items.push(this.Item("Min", "Min", null, "Min"));
    items.push(this.Item("Max", "Max", null, "Max"));
    items.push(this.Item("MinDate", "MinDate", null, "MinDate"));
    items.push(this.Item("MaxDate", "MaxDate", null, "MaxDate"));
    items.push(this.Item("MinTime", "MinTime", null, "MinTime"));
    items.push(this.Item("MaxTime", "MaxTime", null, "MaxTime"));
    items.push(this.Item("MinStr", "MinStr", null, "MinStr"));
    items.push(this.Item("MaxStr", "MaxStr", null, "MaxStr"));
    items.push(this.Item("Median", "Median", null, "Median"));
    items.push(this.Item("Mode", "Mode", null, "Mode"));
    items.push(this.Item("Avg", "Avg", null, "Avg"));
    items.push(this.Item("First", "First", null, "First"));
    items.push(this.Item("Last", "Last", null, "Last"));

    return items;
}

StiMobileDesigner.prototype.GetDrillDownModeItems = function () {
    var items = [];
    items.push(this.Item("SinglePage", this.loc.PropertyEnum.StiDrillDownModeSinglePage, null, "SinglePage"));
    items.push(this.Item("MultiPage", this.loc.PropertyEnum.StiDrillDownModeMultiPage, null, "MultiPage"));

    return items;
}

StiMobileDesigner.prototype.GetHyperlinkTypeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.FormInteraction.HyperlinkUsingInteractionBookmark, null, "HyperlinkUsingInteractionBookmark"));
    items.push(this.Item("item1", this.loc.FormInteraction.HyperlinkUsingInteractionTag, null, "HyperlinkUsingInteractionTag"));
    items.push(this.Item("item2", this.loc.FormInteraction.HyperlinkExternalDocuments, null, "HyperlinkExternalDocuments"));

    return items;
}

StiMobileDesigner.prototype.GetDataBandsForInteractionSort = function () {
    if (!this.options.report) return null;

    var items = [];
    for (var componentName in this.options.currentPage.components) {
        var component = this.options.currentPage.components[componentName];
        if ((component.typeComponent == "StiTable" || component.typeComponent == "StiDataBand" || component.typeComponent == "StiHierarchicalBand" ||
            component.typeComponent == "StiCrossDataBand") && component.properties.dataSource && component.properties.dataSource != "[Not Assigned]")
            items.push({ componentName: componentName, dataSourceName: component.properties.dataSource });
    }

    if (items.length == 0) return null;

    return items;
}

StiMobileDesigner.prototype.GetHotkeyPrefixItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.HotkeyPrefixNone, null, "None"));
    items.push(this.Item("item1", this.loc.PropertyEnum.HotkeyPrefixShow, null, "Show"));
    items.push(this.Item("item2", this.loc.PropertyEnum.HotkeyPrefixHide, null, "Hide"));

    return items;
}

StiMobileDesigner.prototype.GetTextQualityItems = function () {
    var items = [];
    items.push(this.Item("item0", "Standard", null, "Standard"));
    items.push(this.Item("item1", "Typographic", null, "Typographic"));
    items.push(this.Item("item2", "Wysiwyg", null, "Wysiwyg"));

    return items;
}

StiMobileDesigner.prototype.GetSortDirectionForCrossTabField = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiSortDirectionAsc, null, "Asc"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiSortDirectionDesc, null, "Desc"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiSortDirectionNone, null, "None"));

    return items;
}

StiMobileDesigner.prototype.GetSortType = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiSortTypeByValue, null, "ByValue"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiSortTypeByDisplayValue, null, "ByDisplayValue"));

    return items;
}

StiMobileDesigner.prototype.GetEnumeratorTypeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiEnumeratorTypeNone, null, "None"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiEnumeratorTypeArabic, null, "Arabic"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiEnumeratorTypeRoman, null, "Roman"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StiEnumeratorTypeABC, null, "ABC"));

    return items;
}

StiMobileDesigner.prototype.GetSummaryValuesItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyEnum.StiSummaryValuesAllValues, null, "AllValues"));
    items.push(this.Item("item1", this.loc.PropertyEnum.StiSummaryValuesSkipZerosAndNulls, null, "SkipZerosAndNulls"));
    items.push(this.Item("item2", this.loc.PropertyEnum.StiSummaryValuesSkipNulls, null, "SkipNulls"));

    return items;
}

StiMobileDesigner.prototype.GetSummaryTypeForCrossTabFiledItems = function () {
    var items = [];
    items.push(this.Item("item0", "None", null, "None"));
    items.push(this.Item("item1", "Sum", null, "Sum"));
    items.push(this.Item("item3", "Average", null, "Average"));
    items.push(this.Item("item4", "Min", null, "Min"));
    items.push(this.Item("item5", "Max", null, "Max"));
    items.push(this.Item("item6", "Count", null, "Count"));
    items.push(this.Item("item7", "CountDistinct", null, "CountDistinct"));
    items.push(this.Item("item8", "Image", null, "Image"));

    return items;
}

StiMobileDesigner.prototype.GetCellTypeItems = function () {
    var items = [];
    items.push(this.Item("Text", this.loc.PropertyEnum.StiTablceCellTypeText, null, "Text"));
    items.push(this.Item("Image", this.loc.PropertyEnum.StiTablceCellTypeImage, null, "Image"));
    items.push(this.Item("CheckBox", this.loc.PropertyEnum.StiTablceCellTypeCheckBox, null, "CheckBox"));
    if (!this.options.isJava && !this.options.jsMode) {
        items.push(this.Item("RichText", this.loc.PropertyEnum.StiTablceCellTypeRichText, null, "RichText"));
    }

    return items;
}

StiMobileDesigner.prototype.GetCellDockStyleItems = function () {
    var items = [];
    items.push(this.Item("Left", this.loc.PropertyEnum.StiDockStyleLeft, null, "Left"));
    items.push(this.Item("Right", this.loc.PropertyEnum.StiDockStyleRight, null, "Right"));
    items.push(this.Item("Top", this.loc.PropertyEnum.StiDockStyleTop, null, "Top"));
    items.push(this.Item("Bottom", this.loc.PropertyEnum.StiDockStyleBottom, null, "Bottom"));
    items.push(this.Item("None", this.loc.PropertyEnum.StiDockStyleNone, null, "None"));
    items.push(this.Item("Fill", this.loc.PropertyEnum.StiDockStyleFill, null, "Fill"));

    return items;
}

StiMobileDesigner.prototype.GetTableContextSubMenuItems = function () {
    var items = [];
    items.push(this.Item("joinCells", this.loc.MainMenu.menuJoinCells, "ContextMenu.JoinCells.png", "joinCells"));
    items.push("separator0");
    items.push(this.Item("insertColumnToLeft", this.loc.MainMenu.menuInsertColumnToLeft, "ContextMenu.InsertColumnToLeft.png", "insertColumnToLeft"));
    items.push(this.Item("insertColumnToRight", this.loc.MainMenu.menuInsertColumnToRight, "ContextMenu.InsertColumnToRight.png", "insertColumnToRight"));
    items.push(this.Item("deleteColumn", this.loc.MainMenu.menuDeleteColumn, "ContextMenu.DeleteColumn.png", "deleteColumn"));
    //items.push(this.Item("selectColumn", this.loc.MainMenu.menuSelectColumn, "ContextMenu.SelectColumn.png", "selectColumn"));
    items.push("separator1");
    items.push(this.Item("insertRowAbove", this.loc.MainMenu.menuInsertRowAbove, "ContextMenu.InsertRowAbove.png", "insertRowAbove"));
    items.push(this.Item("insertRowBelow", this.loc.MainMenu.menuInsertRowBelow, "ContextMenu.InsertRowBelow.png", "insertRowBelow"));
    items.push(this.Item("deleteRow", this.loc.MainMenu.menuDeleteRow, "ContextMenu.DeleteRow.png", "deleteRow"));
    //items.push(this.Item("selectRow", this.loc.MainMenu.menuSelectRow, "ContextMenu.SelectRow.png", "selectRow"));
    items.push("separator2");
    items.push(this.Item("convertToText", this.loc.MainMenu.menuConvertToText, "ContextMenu.StiText.png", "convertToText"));
    items.push(this.Item("convertToImage", this.loc.MainMenu.menuConvertToImage, "ContextMenu.StiImage.png", "convertToImage"));
    items.push(this.Item("convertToCheckBox", this.loc.MainMenu.menuConvertToCheckBox, "ContextMenu.StiCheckBox.png", "convertToCheckBox"));
    if (!this.options.isJava) {
        items.push(this.Item("convertToRichText", this.loc.MainMenu.MenuConvertToRichText, "ContextMenu.StiRichText.png", "convertToRichText"));
    }

    return items;
}

StiMobileDesigner.prototype.GetOrderContextSubMenuItems = function () {
    var items = [];
    items.push(this.Item("BringToFront", this.loc.Toolbars.BringToFront, "ContextMenu.BringToFront.png", "BringToFront"));
    items.push(this.Item("SendToBack", this.loc.Toolbars.SendToBack, "ContextMenu.SendToBack.png", "SendToBack"));
    items.push(this.Item("MoveForward", this.loc.Toolbars.MoveForward, "ContextMenu.MoveForward.png", "MoveForward"));
    items.push(this.Item("MoveBackward", this.loc.Toolbars.MoveBackward, "ContextMenu.MoveBackward.png", "MoveBackward"));

    return items;
}

StiMobileDesigner.prototype.GetTableAutoWidthItems = function () {
    var items = [];
    items.push(this.Item("None", this.loc.PropertyEnum.StiTableAutoWidthNone, null, "None"));
    items.push(this.Item("Page", this.loc.PropertyEnum.StiTableAutoWidthPage, null, "Page"));
    items.push(this.Item("Table", this.loc.PropertyEnum.StiTableAutoWidthTable, null, "Table"));

    return items;
}

StiMobileDesigner.prototype.GetTableAutoWidthTypeItems = function () {
    var items = [];
    items.push(this.Item("None", this.loc.PropertyEnum.StiTableAutoWidthTypeNone, null, "None"));
    items.push(this.Item("LastColumns", this.loc.PropertyEnum.StiTableAutoWidthTypeLastColumns, null, "LastColumns"));
    items.push(this.Item("FullTable", this.loc.PropertyEnum.StiTableAutoWidthTypeFullTable, null, "FullTable"));

    return items;
}

StiMobileDesigner.prototype.GetPrintOnEvenOddPagesItems = function () {
    var items = [];
    items.push(this.Item("Ignore", this.loc.PropertyEnum.StiPrintOnEvenOddPagesTypeIgnore, null, "Ignore"));
    items.push(this.Item("PrintOnEvenPages", this.loc.PropertyEnum.StiPrintOnEvenOddPagesTypePrintOnEvenPages, null, "PrintOnEvenPages"));
    items.push(this.Item("PrintOnOddPages", this.loc.PropertyEnum.StiPrintOnEvenOddPagesTypePrintOnOddPages, null, "PrintOnOddPages"));

    return items;
}

StiMobileDesigner.prototype.GetVariableSelectionItems = function () {
    var items = [];
    items.push(this.Item("FromVariable", this.loc.PropertyEnum.StiSelectionModeFromVariable, null, "FromVariable"));
    items.push(this.Item("Nothing", this.loc.PropertyEnum.StiSelectionModeNothing, null, "Nothing"));
    items.push(this.Item("First", this.loc.PropertyEnum.StiSelectionModeFirst, null, "First"));

    return items;
}

StiMobileDesigner.prototype.GetBoolAndExpressionItems = function () {
    var items = [];
    items.push(this.Item("True", this.loc.PropertyEnum.boolTrue, null, "True"));
    items.push(this.Item("False", this.loc.PropertyEnum.boolFalse, null, "False"));
    items.push(this.Item("Expression", this.loc.PropertyMain.Expression, null, "Expression"));

    return items;
}

StiMobileDesigner.prototype.GetColumnDirectionItems = function () {
    var items = [];
    items.push(this.Item("DownThenAcross", this.loc.PropertyEnum.StiColumnDirectionDownThenAcross, null, "DownThenAcross"));
    items.push(this.Item("AcrossThenDown", this.loc.PropertyEnum.StiColumnDirectionAcrossThenDown, null, "AcrossThenDown"));

    return items;
}

StiMobileDesigner.prototype.GetLayoutSizeItems = function () {
    var items = [];
    items.push(this.Item("MakeSameSize", this.loc.Toolbars.MakeSameSize, "Layout.MakeSameSize.png", "MakeSameSize"));
    items.push(this.Item("MakeSameWidth", this.loc.Toolbars.MakeSameWidth, "Layout.MakeSameWidth.png", "MakeSameWidth"));
    items.push(this.Item("MakeSameHeight", this.loc.Toolbars.MakeSameHeight, "Layout.MakeSameHeight.png", "MakeSameHeight"));

    return items;
}

StiMobileDesigner.prototype.GetDockStyleItems = function () {
    var items = [];
    items.push(this.Item("Left", this.loc.PropertyEnum.StiDockStyleLeft, null, "Left"));
    items.push(this.Item("Right", this.loc.PropertyEnum.StiDockStyleRight, null, "Right"));
    items.push(this.Item("Top", this.loc.PropertyEnum.StiDockStyleTop, null, "Top"));
    items.push(this.Item("Bottom", this.loc.PropertyEnum.StiDockStyleBottom, null, "Bottom"));
    items.push(this.Item("Fill", this.loc.PropertyEnum.StiDockStyleFill, null, "Fill"));
    items.push(this.Item("None", this.loc.PropertyEnum.StiDockStyleNone, null, "None"));

    return items;
}

StiMobileDesigner.prototype.GetCultureItems = function () {
    var items = [];
    items.push(this.Item("none", " ", null, ""));

    for (var i = 0; i < this.options.cultures.length; i++)
        items.push(this.Item(i.toString(), this.options.cultures[i].displayName, null, this.options.cultures[i].name));

    return items;
}

StiMobileDesigner.prototype.GetFontNamesItems = function () {
    var items = [];
    for (var i = 0; i < this.options.fontNames.length; i++)
        items.push(this.Item("fontItem" + i, this.options.fontNames[i].value, null, this.options.fontNames[i].value));

    return items;
}

StiMobileDesigner.prototype.GetBorderPrimitiveCapStyleItems = function () {
    var items = [];
    items.push(this.Item("None", this.loc.PropertyEnum.StiCapStyleDiamond, null, "None"));
    items.push(this.Item("Arrow", this.loc.PropertyEnum.StiCapStyleArrow, null, "Arrow"));
    items.push(this.Item("Open", this.loc.PropertyEnum.StiCapStyleOpen, null, "Open"));
    items.push(this.Item("Stealth", this.loc.PropertyEnum.StiCapStyleStealth, null, "Stealth"));
    items.push(this.Item("Diamond", this.loc.PropertyEnum.StiCapStyleDiamond, null, "Diamond"));
    items.push(this.Item("Square", this.loc.PropertyEnum.StiCapStyleSquare, null, "Square"));
    items.push(this.Item("Oval", this.loc.PropertyEnum.StiCapStyleOval, null, "Oval"));

    return items;
}

StiMobileDesigner.prototype.GetBarCodeChecksumItems = function () {
    var items = [];
    items.push(this.Item("None", this.loc.PropertyEnum.StiCode11CheckSumNone, null, "None"));
    items.push(this.Item("OneDigit", this.loc.PropertyEnum.StiCode11CheckSumOneDigit, null, "OneDigit"));
    items.push(this.Item("TwoDigits", this.loc.PropertyEnum.StiCode11CheckSumTwoDigits, null, "TwoDigits"));
    items.push(this.Item("Auto", this.loc.PropertyEnum.StiCode11CheckSumAuto, null, "Auto"));

    return items;
}

StiMobileDesigner.prototype.GetBarCodeCheckSumItems = function () {
    var items = [];
    items.push(this.Item("Yes", this.loc.PropertyEnum.StiCheckSumYes, null, "Yes"));
    items.push(this.Item("No", this.loc.PropertyEnum.StiCheckSumNo, null, "No"));

    return items;
}

StiMobileDesigner.prototype.GetBarCodePlesseyCheckSumItems = function () {
    var items = [];
    items.push(this.Item("None", this.loc.PropertyEnum.StiPlesseyCheckSumNone, null, "None"));
    items.push(this.Item("Modulo10", this.loc.PropertyEnum.StiPlesseyCheckSumModulo10, null, "Modulo10"));
    items.push(this.Item("Modulo11", this.loc.PropertyEnum.StiPlesseyCheckSumModulo11, null, "Modulo11"));

    return items;
}

StiMobileDesigner.prototype.GetBarCodeEncodingTypeItems = function () {
    var items = [];
    items.push(this.Item("Ascii", "Ascii", null, "Ascii"));
    items.push(this.Item("C40", "C40", null, "C40"));
    items.push(this.Item("Text", "Text", null, "Text"));
    items.push(this.Item("X12", "X12", null, "X12"));
    items.push(this.Item("Edifact", "Edifact", null, "Edifact"));
    items.push(this.Item("Binary", "Binary", null, "Binary"));

    return items;
}

StiMobileDesigner.prototype.GetBarCodeMatrixSizeDataMatrixItems = function () {
    var items = [];
    items.push(this.Item("Automatic", "Automatic", null, "Automatic"));
    items.push(this.Item("s10x10", "s10x10", null, "s10x10"));
    items.push(this.Item("s12x12", "s12x12", null, "s12x12"));
    items.push(this.Item("s8x18", "s8x18", null, "s8x18"));
    items.push(this.Item("s14x14", "s14x14", null, "s14x14"));
    items.push(this.Item("s8x32", "s8x32", null, "s8x32"));
    items.push(this.Item("s16x16", "s16x16", null, "s16x16"));
    items.push(this.Item("s12x26", "s12x26", null, "s12x26"));
    items.push(this.Item("s18x18", "s18x18", null, "s18x18"));
    items.push(this.Item("s20x20", "s20x20", null, "s20x20"));
    items.push(this.Item("s12x36", "s12x36", null, "s12x36"));
    items.push(this.Item("s22x22", "s22x22", null, "s22x22"));
    items.push(this.Item("s16x36", "s16x36", null, "s16x36"));
    items.push(this.Item("s24x24", "s24x24", null, "s24x24"));
    items.push(this.Item("s26x26", "s26x26", null, "s26x26"));
    items.push(this.Item("s16x48", "s16x48", null, "s16x48"));
    items.push(this.Item("s32x32", "s32x32", null, "s32x32"));
    items.push(this.Item("s36x36", "s36x36", null, "s36x36"));
    items.push(this.Item("s40x40", "s40x40", null, "s40x40"));
    items.push(this.Item("s44x44", "s44x44", null, "s44x44"));
    items.push(this.Item("s48x48", "s48x48", null, "s48x48"));
    items.push(this.Item("s52x52", "s52x52", null, "s52x52"));
    items.push(this.Item("s64x64", "s64x64", null, "s64x64"));
    items.push(this.Item("s72x72", "s72x72", null, "s72x72"));
    items.push(this.Item("s80x80", "s80x80", null, "s80x80"));
    items.push(this.Item("s88x88", "s88x88", null, "s88x88"));
    items.push(this.Item("s96x96", "s96x96", null, "s96x96"));
    items.push(this.Item("s104x104", "s104x104", null, "s104x104"));
    items.push(this.Item("s120x120", "s120x120", null, "s120x120"));
    items.push(this.Item("s132x132", "s132x132", null, "s132x132"));
    items.push(this.Item("s144x144", "s144x144", null, "s144x144"));

    return items;
}

StiMobileDesigner.prototype.GetBarCodeSupplementTypeItems = function () {
    var items = [];
    items.push(this.Item("None", this.loc.PropertyEnum.StiEanSupplementTypeNone, null, "None"));
    items.push(this.Item("TwoDigit", this.loc.PropertyEnum.StiEanSupplementTypeTwoDigit, null, "TwoDigit"));
    items.push(this.Item("FiveDigit", this.loc.PropertyEnum.StiEanSupplementTypeFiveDigit, null, "FiveDigit"));

    return items;
}

StiMobileDesigner.prototype.GetBarCodeEncodingModeItems = function () {
    var items = [];
    items.push(this.Item("Text", "Text", null, "Text"));
    items.push(this.Item("Numeric", "Numeric", null, "Numeric"));
    items.push(this.Item("Byte", "Byte", null, "Byte"));

    return items;
}

StiMobileDesigner.prototype.GetBarCodeErrorsCorrectionLevelItems = function () {
    var items = [];
    items.push(this.Item("Automatic", "Automatic", null, "Automatic"));
    for (var i = 0; i <= 8; i++) {
        items.push(this.Item("Level" + i, "Level" + i, null, "Level" + i));
    }

    return items;
}

StiMobileDesigner.prototype.GetBarCodeErrorCorrectionLevelItems = function () {
    var items = [];
    for (var i = 1; i <= 4; i++) {
        items.push(this.Item("Level" + i, "Level" + i, null, "Level" + i));
    }

    return items;
}

StiMobileDesigner.prototype.GetBarCodeMatrixSizeQRCodeItems = function () {
    var items = [];
    items.push(this.Item("Automatic", "Automatic", null, "Automatic"));
    for (var i = 1; i <= 40; i++) {
        items.push(this.Item("v" + i, "v" + i, null, "v" + i));
    }

    return items;
}

StiMobileDesigner.prototype.GetBarCodeModeItems = function () {
    var items = [];
    items.push(this.Item("Mode2", "Mode2", null, "Mode2"));
    items.push(this.Item("Mode3", "Mode3", null, "Mode3"));
    items.push(this.Item("Mode4", "Mode4", null, "Mode4"));
    items.push(this.Item("Mode5", "Mode5", null, "Mode5"));
    items.push(this.Item("Mode6", "Mode6", null, "Mode6"));

    return items;
}

StiMobileDesigner.prototype.GetAllComponentsItems = function (sort) {
    var items = [];
    if (this.options.report) {
        var repName = StiBase64.decode(this.options.report.properties.reportName.replace("Base64Code;", ""));
        var repText = repName + " : " + this.loc.Components.StiReport;
        items.push(this.Item(repName, repText, null, repText));

        for (var pageName in this.options.report.pages) {
            var page = this.options.report.pages[pageName];
            var pageText = pageName + " : " + (page.isDashboard ? this.loc.Components.StiDashboard : this.loc.Components.StiPage);
            items.push(this.Item(pageName, pageText, null, pageText));

            for (var componentName in page.components) {
                var component = page.components[componentName];
                var compText = componentName;
                var componentType = "";

                if (component.properties.cellType) {
                    componentType = this.loc.Components["Sti" + component.properties.cellType];
                }
                else {
                    componentType = this.loc.Components[component.typeComponent.replace("Element", "")];
                }

                if (componentType) compText += " : " + componentType;
                items.push(this.Item(componentName, compText, null, compText));

                if (component.typeComponent == "StiCrossTab") {
                    var crossTabChilds = component.controls.crossTabContainer.childNodes;
                    for (var i = 0; i < crossTabChilds.length; i++) {
                        var fieldType = this.loc.Components[crossTabChilds[i].properties.typeCrossField];
                        var fieldName = crossTabChilds[i].properties.name;
                        var fieldText = crossTabChilds[i].properties.name;
                        if (fieldType) fieldText += " : " + fieldType;
                        items.push(this.Item(fieldName, fieldText, null, ["StiCrossField", componentName, fieldName]));
                    }
                }
            }
        }
    }

    if (sort) {
        items.sort(this.SortByCaption);
    }

    return items;
}

StiMobileDesigner.prototype.GetBarCodeCategoriesItems = function () {
    var items = [];
    items.push(this.Item("TwoDimensional", this.loc.BarCode.TwoDimensional, "BarCodes.StiQRCodeBarCodeType.png", "TwoDimensional", null, true));
    items.push(this.Item("EANUPC", "EAN\UPC", "BarCodes.StiCodabarBarCodeType.png", "EANUPC", null, true));
    items.push(this.Item("GS1", "GS1", "BarCodes.StiCodabarBarCodeType.png", "GS1", null, true));
    items.push(this.Item("Post", this.loc.BarCode.Post, "BarCodes.StiAustraliaPost4StateBarCodeType.png", "Post", null, true));
    items.push(this.Item("Others", this.loc.FormDesigner.Others, "BarCodes.StiCodabarBarCodeType.png", "Others", null, true));

    return items;
}

StiMobileDesigner.prototype.GetBarCodeTwoDimensionalItems = function () {
    var items = [];
    items.push(this.Item("StiQRCodeBarCodeType", "QR Code", "BarCodes.StiQRCodeBarCodeType.png", "StiQRCodeBarCodeType"));
    items.push(this.Item("StiDataMatrixBarCodeType", "DataMatrix", "BarCodes.StiDataMatrixBarCodeType.png", "StiDataMatrixBarCodeType"));
    items.push(this.Item("StiMaxicodeBarCodeType", "Maxicode", "BarCodes.StiMaxicodeBarCodeType.png", "StiMaxicodeBarCodeType"));
    items.push(this.Item("StiPdf417BarCodeType", "Pdf417", "BarCodes.StiPdf417BarCodeType.png", "StiPdf417BarCodeType"));
    items.push(this.Item("StiAztecBarCodeType", "Aztec", "BarCodes.StiAztecBarCodeType.png", "StiAztecBarCodeType"));

    return items;
}

StiMobileDesigner.prototype.GetBarCodeEANUPCItems = function () {
    var items = [];
    items.push(this.Item("StiEAN128aBarCodeType", "EAN-128a", "BarCodes.StiCodabarBarCodeType.png", "StiEAN128aBarCodeType"));
    items.push(this.Item("StiEAN128bBarCodeType", "EAN-128b", "BarCodes.StiCodabarBarCodeType.png", "StiEAN128bBarCodeType"));
    items.push(this.Item("StiEAN128cBarCodeType", "EAN-128c", "BarCodes.StiCodabarBarCodeType.png", "StiEAN128cBarCodeType"));
    items.push(this.Item("StiEAN128AutoBarCodeType", "EAN-128 Auto", "BarCodes.StiCodabarBarCodeType.png", "StiEAN128AutoBarCodeType"));
    items.push(this.Item("StiEAN13BarCodeType", "EAN-13", "BarCodes.StiEANBarCodeType.png", "StiEAN13BarCodeType"));
    items.push(this.Item("StiEAN8BarCodeType", "EAN-8", "BarCodes.StiEANBarCodeType.png", "StiEAN8BarCodeType"));
    items.push(this.Item("StiUpcABarCodeType", "UPC-A", "BarCodes.StiCodabarBarCodeType.png", "StiUpcABarCodeType"));
    items.push(this.Item("StiUpcEBarCodeType", "UPC-E", "BarCodes.StiCodabarBarCodeType.png", "StiUpcEBarCodeType"));
    items.push(this.Item("StiUpcSup2BarCodeType", "UPC-Supp2", "BarCodes.StiCodabarBarCodeType.png", "StiUpcSup2BarCodeType"));
    items.push(this.Item("StiUpcSup5BarCodeType", "UPC-Supp5", "BarCodes.StiCodabarBarCodeType.png", "StiUpcSup5BarCodeType"));
    items.push(this.Item("StiJan13BarCodeType", "JAN-13", "BarCodes.StiEANBarCodeType.png", "StiJan13BarCodeType"));
    items.push(this.Item("StiJan8BarCodeType", "JAN-8", "BarCodes.StiEANBarCodeType.png", "StiJan8BarCodeType"));

    return items;
}

StiMobileDesigner.prototype.GetBarCodeGS1Items = function () {
    var items = [];
    items.push(this.Item("StiGS1_128BarCodeType", "GS1-128", "BarCodes.StiCodabarBarCodeType.png", "StiGS1_128BarCodeType"));
    items.push(this.Item("StiGS1QRCodeBarCodeType", "GS1 QR Code", "BarCodes.StiQRCodeBarCodeType.png", "StiGS1QRCodeBarCodeType"));
    items.push(this.Item("StiGS1DataMatrixBarCodeType", "GS1 DataMatrix", "BarCodes.StiDataMatrixBarCodeType.png", "StiGS1DataMatrixBarCodeType"));
    items.push(this.Item("StiSSCC18BarCodeType", "SSCC", "BarCodes.StiCodabarBarCodeType.png", "StiSSCC18BarCodeType"));
    items.push(this.Item("StiITF14BarCodeType", "ITF-14", "BarCodes.StiITF14BarCodeType.png", "StiITF14BarCodeType"));

    return items;
}

StiMobileDesigner.prototype.GetBarCodePostItems = function () {
    var items = [];
    items.push(this.Item("StiAustraliaPost4StateBarCodeType", "Australia Post 4-state", "BarCodes.StiAustraliaPost4StateBarCodeType.png", "StiAustraliaPost4StateBarCodeType"));
    items.push(this.Item("StiIntelligentMail4StateBarCodeType", "IntelligentMail USPS 4-State", "BarCodes.StiAustraliaPost4StateBarCodeType.png", "StiIntelligentMail4StateBarCodeType"));
    items.push(this.Item("StiPostnetBarCodeType", "Postnet", "BarCodes.StiPostnetBarCodeType.png", "StiPostnetBarCodeType"));
    items.push(this.Item("StiDutchKIXBarCodeType", "Royal TPG Post KIX 4-State", "BarCodes.StiAustraliaPost4StateBarCodeType.png", "StiDutchKIXBarCodeType"));
    items.push(this.Item("StiRoyalMail4StateBarCodeType", "Royal Mail 4-state", "BarCodes.StiAustraliaPost4StateBarCodeType.png", "StiRoyalMail4StateBarCodeType"));
    items.push(this.Item("StiFIMBarCodeType", "FIM", "BarCodes.StiFIMBarCodeType.png", "StiFIMBarCodeType"));

    return items;
}

StiMobileDesigner.prototype.GetBarCodeOthersItems = function () {
    var items = [];
    items.push(this.Item("StiPharmacodeBarCodeType", "Pharmacode", "BarCodes.StiCodabarBarCodeType.png", "StiPharmacodeBarCodeType"));
    items.push(this.Item("StiCode11BarCodeType", "Code11", "BarCodes.StiCodabarBarCodeType.png", "StiCode11BarCodeType"));
    items.push(this.Item("StiCode128aBarCodeType", "Code128a", "BarCodes.StiCodabarBarCodeType.png", "StiCode128aBarCodeType"));
    items.push(this.Item("StiCode128bBarCodeType", "Code128b", "BarCodes.StiCodabarBarCodeType.png", "StiCode128bBarCodeType"));
    items.push(this.Item("StiCode128cBarCodeType", "Code128c", "BarCodes.StiCodabarBarCodeType.png", "StiCode128cBarCodeType"));
    items.push(this.Item("StiCode128AutoBarCodeType", "Code128 Auto", "BarCodes.StiCodabarBarCodeType.png", "StiCode128AutoBarCodeType"));
    items.push(this.Item("StiCode39BarCodeType", "Code39", "BarCodes.StiCodabarBarCodeType.png", "StiCode39BarCodeType"));
    items.push(this.Item("StiCode39ExtBarCodeType", "Code39 Extended", "BarCodes.StiCodabarBarCodeType.png", "StiCode39ExtBarCodeType"));
    items.push(this.Item("StiCode93BarCodeType", "Code93", "BarCodes.StiCodabarBarCodeType.png", "StiCode93BarCodeType"));
    items.push(this.Item("StiCode93ExtBarCodeType", "Code93 Extended", "BarCodes.StiCodabarBarCodeType.png", "StiCode93ExtBarCodeType"));
    items.push(this.Item("StiCodabarBarCodeType", "Codabar", "BarCodes.StiCodabarBarCodeType.png", "StiCodabarBarCodeType"));
    items.push(this.Item("StiIsbn10BarCodeType", "ISBN-10", "BarCodes.StiEANBarCodeType.png", "StiIsbn10BarCodeType"));
    items.push(this.Item("StiIsbn13BarCodeType", "ISBN-13", "BarCodes.StiEANBarCodeType.png", "StiIsbn13BarCodeType"));
    items.push(this.Item("StiMsiBarCodeType", "Msi", "BarCodes.StiCodabarBarCodeType.png", "StiMsiBarCodeType"));
    items.push(this.Item("StiPlesseyBarCodeType", "Plessey", "BarCodes.StiCodabarBarCodeType.png", "StiPlesseyBarCodeType"));
    items.push(this.Item("StiInterleaved2of5BarCodeType", "2of5 Interleaved", "BarCodes.StiCodabarBarCodeType.png", "StiInterleaved2of5BarCodeType"));
    items.push(this.Item("StiStandard2of5BarCodeType", "2of5 Standard", "BarCodes.StiCodabarBarCodeType.png", "StiStandard2of5BarCodeType"));

    return items;
}

StiMobileDesigner.prototype.GetStylesActionsMenuItems = function () {
    var items = [];
    items.push(this.Item("openStyle", this.loc.MainMenu.menuFileOpen.replace("&", ""), "Open.png", "openStyle"));
    items.push(this.Item("saveStyle", this.loc.MainMenu.menuFileSaveAs, "Save.png", "saveStyle"));
    items.push("separator");
    items.push(this.Item("createStyleCollection", this.loc.FormStyleDesigner.CreateStyleCollection + "...", "StylesCreate.png", "createStyleCollection"));

    return items;
}

StiMobileDesigner.prototype.GetAllStyleCollectionNames = function () {
    var styles = {};
    var report = this.options.report;
    if (report) {
        for (var pageName in report.pages) {
            var page = report.pages[pageName];
            if (page.properties.componentStyle) {
                styles[page.properties.componentStyle] = true;
            }
            for (var cName in page.components) {
                var component = page.components[cName];
                if (component.properties.componentStyle) {
                    styles[component.properties.componentStyle] = true;
                }
            }
        }
    }

    return styles;
}

StiMobileDesigner.prototype.GetRenderToItems = function (currentComponent) {
    var items = [];
    var report = this.options.report;
    items.push(this.Item("empty", "", null, ""));

    if (currentComponent && currentComponent.typeComponent == "StiText") {
        var page = report.pages[currentComponent.properties.pageName];
        var parentName = currentComponent.properties.parentName;

        for (var componentName in page.components) {
            var component = page.components[componentName];
            if (currentComponent != component && parentName == component.properties.parentName &&
                (component.typeComponent == "StiText" || component.typeComponent == "StiTextInCells")) {
                var itemText = component.properties.name;
                items.push(this.Item(itemText, itemText, null, itemText));
            }
        }
    }

    return items;
}

StiMobileDesigner.prototype.GetSaveTypeItems = function () {
    var items = [];
    items.push(this.Item("XML", "XML", null, "xml"));
    items.push(this.Item("JSON", "JSON", null, "json"));

    return items;
}

StiMobileDesigner.prototype.GetRefreshTimeItems = function () {
    var items = [];
    items.push(this.Item("0", this.loc.PropertyEnum.DialogResultNone, null, "0"));
    if (!this.options.cloudMode) {
        items.push(this.Item("10", "10 Seconds", null, "10"));
        items.push(this.Item("20", "20 Seconds", null, "20"));
    }
    items.push(this.Item("30", "30 Seconds", null, "30"));
    items.push(this.Item("60", "1 Minute", null, "60"));
    items.push(this.Item("120", "2 Minute", null, "120"));
    items.push(this.Item("300", "5 Minutes", null, "300"));
    items.push(this.Item("600", "10 Minutes", null, "600"));
    items.push(this.Item("1800", "30 Minutes", null, "1800"));
    items.push(this.Item("3600", "1 Hour", null, "3600"));

    return items;
}

StiMobileDesigner.prototype.GetExportFormatTypesItems = function () {
    var items = [];
    items.push(this.Item("ReportSnapshot", this.loc.FormViewer.DocumentFile.replace("...", ""), "Resources.ResourceReportSnapshot.png", "ReportSnapshot"));
    items.push(this.Item("Pdf", this.loc.Export.ExportTypePdfFile.replace("...", ""), "Resources.ResourcePdf.png", "Pdf"));
    items.push(this.Item("Xps", this.loc.Export.ExportTypeXpsFile.replace("...", ""), "Resources.ResourceXps.png", "Xps"));
    items.push(this.Item("PowerPoint", this.loc.Export.ExportTypePpt2007File.replace("...", ""), "Resources.ResourcePowerPoint.png", "PowerPoint"));
    items.push(this.Item("Html", this.loc.Export.ExportTypeHtmlFile.replace("...", ""), "Resources.ResourceHtml.png", "Html"));
    items.push(this.Item("Text", this.loc.Export.ExportTypeTxtFile.replace("...", ""), "Resources.ResourceTxt.png", "Text"));
    items.push(this.Item("RichText", this.loc.Export.ExportTypeRtfFile.replace("...", ""), "Resources.ResourceRtf.png", "RichText"));
    items.push(this.Item("Word", this.loc.Export.ExportTypeWord2007File.replace("...", ""), "Resources.ResourceWord.png", "Word"));
    items.push(this.Item("OpenDocumentWriter", this.loc.Export.ExportTypeWriterFile.replace("...", ""), "Resources.ResourceOpenDocumentWriter.png", "OpenDocumentWriter"));
    items.push(this.Item("Excel", this.loc.Export.ExportTypeExcelFile.replace("...", ""), "Resources.ResourceExcel.png", "Excel"));
    items.push(this.Item("OpenDocumentCalc", this.loc.Export.ExportTypeCalcFile.replace("...", ""), "Resources.ResourceOpenDocumentCalc.png", "OpenDocumentCalc"));
    items.push(this.Item("Data", this.loc.Export.ExportTypeDataFile.replace("...", ""), "Resources.ResourceData.png", "Data"));
    items.push(this.Item("Image", this.loc.Export.ExportTypeImageFile.replace("...", ""), "Resources.ResourceImage.png", "Image"));

    return items;
}

StiMobileDesigner.prototype.GetMapsCategoriesItems = function () {
    var isRus = this.IsRusCulture(this.options.cultureName);

    var items = [];
    items.push(this.Item("All", this.loc.Report.RangeAll, null, "All"));
    items.push(this.Item("Europe", (isRus ? "Европа" : "Europe") + " (" + this.GetEuropeMapsItems().length + ")", null, "Europe"));
    items.push(this.Item("NorthAmerica", (isRus ? "Северная Америка" : "North America") + " (" + this.GetNorthAmericaMapsItems().length + ")", null, "NorthAmerica"));
    items.push(this.Item("SouthAmerica", (isRus ? "Южная Америка" : "South America") + " (" + this.GetSouthAmericaMapsItems().length + ")", null, "SouthAmerica"));
    items.push(this.Item("Asia", (isRus ? "Азия" : "Asia") + " (" + this.GetAsiaMapsItems().length + ")", null, "Asia"));
    items.push(this.Item("Oceania", (isRus ? "Океания" : "Oceania") + " (" + this.GetOceaniaMapsItems().length + ")", null, "Oceania"));
    items.push(this.Item("Africa", (isRus ? "Африка" : "Africa") + " (" + this.GetAfricaMapsItems().length + ")", null, "Africa"));

    var customMaps = this.GetCustomMapResources();
    if (customMaps.length > 0) {
        items.push(this.Item("Custom", (isRus ? "Пользовательские" : this.loc.FormFormatEditor.Custom) + " (" + customMaps.length + ")", null, "Custom"));
    }

    return items;
}

StiMobileDesigner.prototype.GetEuropeMapsItems = function () {
    var items = [];
    items.push(this.Item("Europe", "Europe", "Europe.png", "Europe"));
    items.push(this.Item("EuropeUnion", "Europe Union", "EU.png", "EU"));
    items.push(this.Item("EuropeWithRussia", "Europe (with Russia)", "EuropeWithRussia.png", "EuropeWithRussia"));
    items.push(this.Item("EuropeUnionWithUnitedKingdom", "Europe Union (with United Kingdom)", "EUWithUnitedKingdom.png", "EUWithUnitedKingdom"));
    items.push(this.Item("France", "France", "France.png", "France"));
    items.push(this.Item("Germany", "Germany", "Germany.png", "Germany"));
    items.push(this.Item("Italy", "Italy", "Italy.png", "Italy"));
    items.push(this.Item("Netherlands", "Netherlands", "Netherlands.png", "Netherlands"));
    items.push(this.Item("Russia", "Russia", "Russia.png", "Russia"));
    items.push(this.Item("Switzerland", "Switzerland", "Switzerland.png", "Switzerland"));
    items.push(this.Item("UK", "UK", "UK.png", "UK"));
    items.push(this.Item("UKCountries", "UK (Countries)", "UKCountries.png", "UKCountries"));
    items.push(this.Item("Albania", "Albania", "Albania.png", "Albania"));
    items.push(this.Item("Andorra", "Andorra", "Andorra.png", "Andorra"));
    items.push(this.Item("Austria", "Austria", "Austria.png", "Austria"));
    items.push(this.Item("Belarus", "Belarus", "Belarus.png", "Belarus"));
    items.push(this.Item("Belgium", "Belgium", "Belgium.png", "Belgium"));
    items.push(this.Item("Benelux", "Benelux", "Benelux.png", "Benelux"));
    items.push(this.Item("BosniaAndHerzegovina", "Bosnia and Herzegovina", "BosniaAndHerzegovina.png", "BosniaAndHerzegovina"));
    items.push(this.Item("Bulgaria", "Bulgaria", "Bulgaria.png", "Bulgaria"));
    items.push(this.Item("Croatia", "Croatia", "Croatia.png", "Croatia"));
    items.push(this.Item("CzechRepublic", "Czech Republic", "CzechRepublic.png", "CzechRepublic"));
    items.push(this.Item("Denmark", "Denmark", "Denmark.png", "Denmark"));
    items.push(this.Item("Estonia", "Estonia", "Estonia.png", "Estonia"));
    items.push(this.Item("Finland", "Finland", "Finland.png", "Finland"));
    items.push(this.Item("France18Regions", "France (18 Regions)", "France18Regions.png", "France18Regions"));
    items.push(this.Item("FranceDepartments", "France (Departments)", "FranceDepartments.png", "FranceDepartments"));
    items.push(this.Item("Georgia", "Georgia", "Georgia.png", "Georgia"));
    items.push(this.Item("Greece", "Greece", "Greece.png", "Greece"));
    items.push(this.Item("Hungary", "Hungary", "Hungary.png", "Hungary"));
    items.push(this.Item("Iceland", "Iceland", "Iceland.png", "Iceland"));
    items.push(this.Item("Ireland", "Ireland", "Ireland.png", "Ireland"));
    items.push(this.Item("Latvia", "Latvia", "Latvia.png", "Latvia"));
    items.push(this.Item("Liechtenstein", "Liechtenstein", "Liechtenstein.png", "Liechtenstein"));
    items.push(this.Item("Lithuania", "Lithuania", "Lithuania.png", "Lithuania"));
    items.push(this.Item("Luxembourg", "Luxembourg", "Luxembourg.png", "Luxembourg"));
    items.push(this.Item("Macedonia", "Macedonia", "Macedonia.png", "Macedonia"));
    items.push(this.Item("Malta", "Malta", "Malta.png", "Malta"));
    items.push(this.Item("Moldova", "Moldova", "Moldova.png", "Moldova"));
    items.push(this.Item("Monaco", "Monaco", "Monaco.png", "Monaco"));
    items.push(this.Item("Montenegro", "Montenegro", "Montenegro.png", "Montenegro"));
    items.push(this.Item("Norway", "Norway", "Norway.png", "Norway"));
    items.push(this.Item("Poland", "Poland", "Poland.png", "Poland"));
    items.push(this.Item("Portugal", "Portugal", "Portugal.png", "Portugal"));
    items.push(this.Item("Romania", "Romania", "Romania.png", "Romania"));
    items.push(this.Item("SanMarino", "SanMarino", "SanMarino.png", "SanMarino"));
    items.push(this.Item("Serbia", "Serbia", "Serbia.png", "Serbia"));
    items.push(this.Item("Scandinavia", "Scandinavia", "Scandinavia.png", "Scandinavia"));
    items.push(this.Item("Slovakia", "Slovakia", "Slovakia.png", "Slovakia"));
    items.push(this.Item("Slovenia", "Slovenia", "Slovenia.png", "Slovenia"));
    items.push(this.Item("Spain", "Spain", "Spain.png", "Spain"));
    items.push(this.Item("Sweden", "Sweden", "Sweden.png", "Sweden"));
    items.push(this.Item("Turkey", "Turkey", "Turkey.png", "Turkey"));
    items.push(this.Item("Ukraine", "Ukraine", "Ukraine.png", "Ukraine"));
    items.push(this.Item("Vatican", "Vatican", "Vatican.png", "Vatican"));

    return items;
}

StiMobileDesigner.prototype.GetNorthAmericaMapsItems = function () {
    var items = [];
    items.push(this.Item("NorthAmerica", "North America", "NorthAmerica.png", "NorthAmerica"));
    items.push(this.Item("USA", "USA", "USA.png", "USA"));
    items.push(this.Item("Canada", "Canada", "Canada.png", "Canada"));
    items.push(this.Item("USAAndCanada", "USA+Canada", "USAAndCanada.png", "USAAndCanada"));
    items.push(this.Item("Mexico", "Mexico", "Mexico.png", "Mexico"));

    return items;
}

StiMobileDesigner.prototype.GetSouthAmericaMapsItems = function () {
    var items = [];
    items.push(this.Item("SouthAmerica", "South America", "SouthAmerica.png", "SouthAmerica"));
    items.push(this.Item("Argentina", "Argentina", "Argentina.png", "Argentina"));
    items.push(this.Item("ArgentinaFD", "Argentina (FD)", "ArgentinaFD.png", "ArgentinaFD"));
    items.push(this.Item("Bolivia", "Bolivia", "Bolivia.png", "Bolivia"));
    items.push(this.Item("Brazil", "Brazil", "Brazil.png", "Brazil"));
    items.push(this.Item("Chile", "Chile", "Chile.png", "Chile"));
    items.push(this.Item("Colombia", "Colombia", "Colombia.png", "Colombia"));
    items.push(this.Item("Ecuador", "Ecuador", "Ecuador.png", "Ecuador"));
    items.push(this.Item("FalklandIslands", "Falkland Islands", "FalklandIslands.png", "FalklandIslands"));
    items.push(this.Item("Guyana", "Guyana", "Guyana.png", "Guyana"));
    items.push(this.Item("Paraguay", "Paraguay", "Paraguay.png", "Paraguay"));
    items.push(this.Item("Peru", "Peru", "Peru.png", "Peru"));
    items.push(this.Item("Suriname", "Suriname", "Suriname.png", "Suriname"));
    items.push(this.Item("Uruguay", "Uruguay", "Uruguay.png", "Uruguay"));
    items.push(this.Item("Venezuela", "Venezuela", "Venezuela.png", "Venezuela"));

    return items;
}

StiMobileDesigner.prototype.GetAsiaMapsItems = function () {
    var items = [];
    items.push(this.Item("Asia", "Asia", "Asia.png", "Asia"));
    items.push(this.Item("Afghanistan", "Afghanistan", "Afghanistan.png", "Afghanistan"));
    items.push(this.Item("Armenia", "Armenia", "Armenia.png", "Armenia"));
    items.push(this.Item("Azerbaijan", "Azerbaijan", "Azerbaijan.png", "Azerbaijan"));
    items.push(this.Item("China", "China", "China.png", "China"));
    items.push(this.Item("ChinaWithHongKongAndMacau", "China with Hong Kong and Macau", "ChinaWithHongKongAndMacau.png", "ChinaWithHongKongAndMacau"));
    items.push(this.Item("ChinaWithHongKongMacauAndTaiwan", "China with Hong Kong, Macau, and Taiwan", "ChinaWithHongKongMacauAndTaiwan.png", "ChinaWithHongKongMacauAndTaiwan"));
    items.push(this.Item("India", "India", "India.png", "India"));
    items.push(this.Item("Israel", "Israel", "Israel.png", "Israel"));
    items.push(this.Item("Japan", "Japan", "Japan.png", "Japan"));
    items.push(this.Item("Kazakhstan", "Kazakhstan", "Kazakhstan.png", "Kazakhstan"));
    items.push(this.Item("Malaysia", "Malaysia", "Malaysia.png", "Malaysia"));
    items.push(this.Item("MiddleEast", "Middle East", "MiddleEast.png", "MiddleEast"));
    items.push(this.Item("Oman", "Oman", "Oman.png", "Oman"));
    items.push(this.Item("Philippines", "Philippines", "Philippines.png", "Philippines"));
    items.push(this.Item("Qatar", "Qatar", "Qatar.png", "Qatar"));
    items.push(this.Item("SaudiArabia", "Saudi Arabia", "SaudiArabia.png", "SaudiArabia"));
    items.push(this.Item("SoutheastAsia", "Southeast Asia", "SoutheastAsia.png", "SoutheastAsia"));
    items.push(this.Item("SouthKorea", "South Korea", "SouthKorea.png", "SouthKorea"));
    items.push(this.Item("Taiwan", "Taiwan", "Taiwan.png", "Taiwan"));
    items.push(this.Item("Thailand", "Thailand", "Thailand.png", "Thailand"));
    items.push(this.Item("Vietnam", "Vietnam", "Vietnam.png", "Vietnam"));

    return items;
}

StiMobileDesigner.prototype.GetOceaniaMapsItems = function () {
    var items = [];
    items.push(this.Item("Australia", "Australia", "Australia.png", "Australia"));
    items.push(this.Item("Indonesia", "Indonesia", "Indonesia.png", "Indonesia"));
    items.push(this.Item("NewZealand", "New Zealand", "NewZealand.png", "NewZealand"));
    items.push(this.Item("Oceania", "Oceania", "Oceania.png", "Oceania"));

    return items;
}

StiMobileDesigner.prototype.GetAfricaMapsItems = function () {
    var items = [];
    items.push(this.Item("CentralAfricanRepublic", "Central African Republic", "CentralAfricanRepublic.png", "CentralAfricanRepublic"));
    items.push(this.Item("SouthAfrica", "South Africa", "SouthAfrica.png", "SouthAfrica"));

    return items;
}

StiMobileDesigner.prototype.GetChoroplethDataTypesItems = function () {
    var items = [];
    items.push(this.Item("dataColumns", this.loc.PropertyMain.DataColumns, null, "DataColumns"));
    items.push(this.Item("manual", this.loc.PropertyEnum.FormStartPositionManual, null, "Manual"));

    return items;
}

StiMobileDesigner.prototype.GetChoroplethMapTypesItems = function () {
    var items = [];
    items.push(this.Item("Individual", this.loc.PropertyEnum.StiMapTypeIndividual, null, "Individual"));
    items.push(this.Item("Group", this.loc.PropertyEnum.StiMapTypeGroup, null, "Group"));
    items.push(this.Item("Heatmap", this.loc.PropertyEnum.StiMapTypeHeatmap, null, "Heatmap"));
    items.push(this.Item("HeatmapWithGroup", this.loc.PropertyEnum.StiMapTypeHeatmapWithGroup, null, "HeatmapWithGroup"));

    return items;
}

StiMobileDesigner.prototype.GetMapDisplayNameTypeItems = function () {
    var items = [];
    items.push(this.Item("None", this.loc.PropertyEnum.StiDisplayNameTypeNone, null, "None"));
    items.push(this.Item("Full", this.loc.PropertyEnum.StiDisplayNameTypeFull, null, "Full"));
    items.push(this.Item("Short", this.loc.PropertyEnum.StiDisplayNameTypeShort, null, "Short"));

    return items;
}

StiMobileDesigner.prototype.GetGaugeCalculationModeItems = function () {
    var items = [];
    items.push(this.Item("Auto", this.loc.PropertyEnum.StiGaugeCalculationModeAuto, null, "Auto"));
    items.push(this.Item("Custom", this.loc.PropertyEnum.StiGaugeCalculationModeCustom, null, "Custom"));

    return items;
}

StiMobileDesigner.prototype.GetLineSpacingItems = function () {
    var items = [];
    items.push(this.Item("item_1", "1", null, "1"));
    items.push(this.Item("item_1.15", "1.15", null, "1.15"));
    items.push(this.Item("item_1.5", "1.5", null, "1.5"));
    items.push(this.Item("item_2", "2", null, "2"));
    items.push(this.Item("item_2.5", "2.5", null, "2.5"));
    items.push(this.Item("item_3", "3", null, "3"));

    return items;
}

StiMobileDesigner.prototype.GetRowsCountItems = function () {
    var items = [];
    items.push(this.Item("SelectAll", this.loc.Dashboard.SelectAll.replace("&", ""), null, "-1"));
    var counts = ["10", "20", "50", "100", "200", "300", "500", "1000", "2000", "3000", "5000", "10000"];
    for (var i = 0; i < counts.length; i++) {
        items.push(this.Item("item" + i, counts[i], null, counts[i]));
    }

    return items;
}

StiMobileDesigner.prototype.GetSelectionModeItems = function () {
    var items = [];
    items.push(this.Item("One", this.loc.PropertyEnum.StiItemSelectionModeOne, null, "One"));
    items.push(this.Item("Multi", this.loc.PropertyEnum.StiItemSelectionModeMulti, null, "Multi"));

    return items;
}

StiMobileDesigner.prototype.GetDateSelectionModeItems = function () {
    var items = [];
    items.push(this.Item("Single", this.loc.PropertyEnum.StiDateSelectionModeSingle, null, "Single"));
    items.push(this.Item("Range", this.loc.PropertyEnum.StiDateSelectionModeRange, null, "Range"));
    items.push(this.Item("AutoRange", this.loc.PropertyEnum.StiDateSelectionModeAutoRange, null, "AutoRange"));

    return items;
}

StiMobileDesigner.prototype.GetDateConditionItems = function () {
    var items = [];
    items.push(this.Item("EqualTo", this.UpperFirstChar(this.loc.PropertyEnum.StiFilterConditionEqualTo), null, "EqualTo"));
    items.push(this.Item("NotEqualTo", this.UpperFirstChar(this.loc.PropertyEnum.StiFilterConditionNotEqualTo), null, "NotEqualTo"));
    items.push(this.Item("GreaterThan", this.UpperFirstChar(this.loc.PropertyEnum.StiFilterConditionGreaterThan), null, "GreaterThan"));
    items.push(this.Item("GreaterThanOrEqualTo", this.UpperFirstChar(this.loc.PropertyEnum.StiFilterConditionGreaterThanOrEqualTo), null, "GreaterThanOrEqualTo"));
    items.push(this.Item("LessThan", this.UpperFirstChar(this.loc.PropertyEnum.StiFilterConditionLessThan), null, "LessThan"));
    items.push(this.Item("LessThanOrEqualTo", this.UpperFirstChar(this.loc.PropertyEnum.StiFilterConditionLessThanOrEqualTo), null, "LessThanOrEqualTo"));

    return items;
}

StiMobileDesigner.prototype.GetSizeModeItems = function () {
    var items = [];
    items.push(this.Item("IncreaseLastRow", this.loc.PropertyEnum.StiEmptySizeModeIncreaseLastRow, null, "IncreaseLastRow"));
    items.push(this.Item("DecreaseLastRow", this.loc.PropertyEnum.StiEmptySizeModeDecreaseLastRow, null, "DecreaseLastRow"));
    items.push(this.Item("AlignFooterToBottom", this.loc.PropertyEnum.StiEmptySizeModeAlignFooterToBottom, null, "AlignFooterToBottom"));
    items.push(this.Item("AlignFooterToTop", this.loc.PropertyEnum.StiEmptySizeModeAlignFooterToTop, null, "AlignFooterToTop"));

    return items;
}

StiMobileDesigner.prototype.GetFilterElementsItems = function (currentElementKey) {
    if (!this.options.report) return null;

    var items = [];
    items.push(this.Item("empty", "", null, ""));

    for (var componentName in this.options.currentPage.components) {
        var component = this.options.currentPage.components[componentName];
        if (this.IsFilterElement(component.typeComponent) && currentElementKey != component.properties.elementKey) {
            items.push(this.Item(component.properties.elementKey, component.properties.name, null, component.properties.elementKey));
        }
    }

    return items;
}

StiMobileDesigner.prototype.GetGaugeRangeTypeItems = function () {
    var items = [];
    items.push(this.Item("None", this.loc.PropertyEnum.StiGaugeRangeTypeNone, null, "None"));
    items.push(this.Item("Color", this.loc.PropertyEnum.StiGaugeRangeTypeColor, null, "Color"));

    return items;
}

StiMobileDesigner.prototype.GetGaugeRangeModeItems = function () {
    var items = [];
    items.push(this.Item("Percentage", this.loc.PropertyEnum.StiGaugeRangeModePercentage, null, "Percentage"));
    items.push(this.Item("Value", this.loc.PropertyEnum.StiGaugeRangeModeValue, null, "Value"));

    return items;
}

StiMobileDesigner.prototype.GetGaugeTypesItems = function () {
    var items = [];
    items.push(this.Item("FullCircular", this.loc.PropertyEnum.StiGaugeTypeFullCircular, "Gauge.Small.FullCircular.png", "FullCircular"));
    items.push(this.Item("HalfCircular", this.loc.PropertyEnum.StiGaugeTypeHalfCircular, "Gauge.Small.HalfCircular.png", "HalfCircular"));
    items.push(this.Item("Linear", this.loc.PropertyEnum.StiGaugeTypeLinear, "Gauge.Small.Linear.png", "Linear"));
    items.push(this.Item("HorizontalLinear", this.loc.PropertyEnum.StiGaugeTypeHorizontalLinear, "Gauge.Small.HorizontalLinear.png", "HorizontalLinear"));
    items.push(this.Item("Bullet", this.loc.PropertyEnum.StiGaugeTypeBullet, "Gauge.Small.Bullet.png", "Bullet"));

    return items;
}

StiMobileDesigner.prototype.GetDateInitialRangeSelectionItems = function () {
    var items = [];
    items.push(this.Item("DayTomorrow", this.loc.DatePickerRanges.Tomorrow, null, "DayTomorrow"));
    items.push(this.Item("DayToday", this.loc.DatePickerRanges.Today, null, "DayToday"));
    items.push(this.Item("DayYesterday", this.loc.DatePickerRanges.Yesterday, null, "DayYesterday"));
    items.push(this.Item("Last7Days", this.loc.DatePickerRanges.Last7Days, null, "Last7Days"));
    items.push(this.Item("Last14Days", this.loc.DatePickerRanges.Last14Days, null, "Last14Days"));
    items.push(this.Item("Last30Days", this.loc.DatePickerRanges.Last30Days, null, "Last30Days"));
    items.push(this.Item("WeekNext", this.loc.DatePickerRanges.NextWeek, null, "WeekNext"));
    items.push(this.Item("WeekCurrent", this.loc.DatePickerRanges.CurrentWeek, null, "WeekCurrent"));
    items.push(this.Item("WeekPrevious", this.loc.DatePickerRanges.PreviousWeek, null, "WeekPrevious"));
    items.push(this.Item("MonthNext", this.loc.DatePickerRanges.NextMonth, null, "MonthNext"));
    items.push(this.Item("MonthCurrent", this.loc.DatePickerRanges.CurrentMonth, null, "MonthCurrent"));
    items.push(this.Item("MonthPrevious", this.loc.DatePickerRanges.PreviousMonth, null, "MonthPrevious"));
    items.push(this.Item("QuarterNext", this.loc.DatePickerRanges.NextQuarter, null, "QuarterNext"));
    items.push(this.Item("QuarterCurrent", this.loc.DatePickerRanges.CurrentQuarter, null, "QuarterCurrent"));
    items.push(this.Item("QuarterPrevious", this.loc.DatePickerRanges.PreviousQuarter, null, "QuarterPrevious"));
    items.push(this.Item("YearNext", this.loc.DatePickerRanges.NextYear, null, "YearNext"));
    items.push(this.Item("YearCurrent", this.loc.DatePickerRanges.CurrentYear, null, "YearCurrent"));
    items.push(this.Item("YearPrevious", this.loc.DatePickerRanges.PreviousYear, null, "YearPrevious"));
    items.push(this.Item("QuarterFirst", this.loc.DatePickerRanges.FirstQuarter, null, "QuarterFirst"));
    items.push(this.Item("QuarterSecond", this.loc.DatePickerRanges.SecondQuarter, null, "QuarterSecond"));
    items.push(this.Item("QuarterThird", this.loc.DatePickerRanges.ThirdQuarter, null, "QuarterThird"));
    items.push(this.Item("QuarterFourth", this.loc.DatePickerRanges.FourthQuarter, null, "QuarterFourth"));
    items.push(this.Item("DateToWeek", this.loc.DatePickerRanges.WeekToDate, null, "DateToWeek"));
    items.push(this.Item("DateToMonth", this.loc.DatePickerRanges.MonthToDate, null, "DateToMonth"));
    items.push(this.Item("DateToQuarter", this.loc.DatePickerRanges.QuarterToDate, null, "DateToQuarter"));
    items.push(this.Item("DateToYear", this.loc.DatePickerRanges.YearToDate, null, "DateToYear"));

    return items;
}

StiMobileDesigner.prototype.GetDateInitialRangeSelectionSourceItems = function () {
    var items = [];
    items.push(this.Item("Selection", this.loc.PropertyMain.Selection, null, "Selection"));
    items.push(this.Item("Variable", this.loc.PropertyMain.Variable, null, "Variable"));

    return items;
}

StiMobileDesigner.prototype.GetTableElementSizeModeItems = function () {
    var items = [];
    items.push(this.Item("AutoSize", this.loc.PropertyEnum.StiSizeModeAutoSize, null, "AutoSize"));
    items.push(this.Item("Fit", this.loc.PropertyEnum.StiSizeModeFit, null, "Fit"));

    return items;
}

StiMobileDesigner.prototype.GetTextElementSizeModeItems = function () {
    var items = [];
    items.push(this.Item("Fit", this.loc.PropertyEnum.StiSizeModeFit, null, "Fit"));
    items.push(this.Item("WordWrap", this.loc.PropertyMain.WordWrap, null, "WordWrap"));
    items.push(this.Item("Trimming", this.loc.PropertyMain.Trimming, null, "Trimming"));


    return items;
}

StiMobileDesigner.prototype.GetRelationDirectionItems = function () {
    var items = [];
    items.push(this.Item("ParentToChild", this.loc.PropertyEnum.RelationDirectionParentToChild, null, "ParentToChild"));
    items.push(this.Item("ChildToParent", this.loc.PropertyEnum.RelationDirectionChildToParent, null, "ChildToParent"));

    return items;
}

StiMobileDesigner.prototype.GetProgressModeItems = function () {
    var items = [];
    items.push(this.Item("Circle", this.loc.PropertyEnum.StiProgressElementModeCircle, null, "Circle"));
    items.push(this.Item("Pie", this.loc.PropertyEnum.StiProgressElementModePie, null, "Pie"));
    items.push(this.Item("DataBars", this.loc.PropertyEnum.StiProgressElementModeDataBars, null, "DataBars"));

    return items;
}

StiMobileDesigner.prototype.GetGaugeElementTypeItems = function () {
    var items = [];
    items.push(this.Item("FullCircular", this.loc.PropertyEnum.StiGaugeTypeFullCircular, null, "FullCircular"));
    items.push(this.Item("HalfCircular", this.loc.PropertyEnum.StiGaugeTypeHalfCircular, null, "HalfCircular"));
    items.push(this.Item("Linear", this.loc.PropertyEnum.StiGaugeTypeLinear, null, "Linear"));

    return items;
}

StiMobileDesigner.prototype.GetStartScreenItems = function () {
    var items = [];
    items.push(this.Item("Welcome", this.loc.FormOptions.Welcome, null, "Welcome"));
    items.push(this.Item("BlankReport", this.loc.Wizards.BlankReport, null, "BlankReport"));
    if (this.options.dashboardAssemblyLoaded) {
        items.push(this.Item("BlankDashboard", this.loc.Wizards.BlankDashboard, null, "BlankDashboard"));
    }

    return items;
}

StiMobileDesigner.prototype.GetDesignerSpecificationItems = function () {
    var items = [];
    items.push(this.Item("Auto", this.loc.PropertyEnum.StiDesignerSpecificationAuto, null, "Auto"));
    items.push(this.Item("Developer", this.loc.PropertyEnum.StiDesignerSpecificationDeveloper, null, "Developer"));
    items.push(this.Item("BICreator", this.loc.PropertyEnum.StiDesignerSpecificationBICreator, null, "BICreator"));

    return items;
}

StiMobileDesigner.prototype.GetOnHoverInteractionItems = function () {
    var items = [];
    items.push(this.Item("None", this.loc.PropertyEnum.StiInteractionOnHoverNone, null, "None"));
    items.push(this.Item("ShowToolTip", this.loc.PropertyEnum.StiInteractionOnHoverShowToolTip, null, "ShowToolTip"));
    items.push(this.Item("ShowHyperlink", this.loc.PropertyEnum.StiInteractionOnHoverShowHyperlink, null, "ShowHyperlink"));

    return items;
}

StiMobileDesigner.prototype.GetOnClickInteractionItems = function (interactionIdent) {
    var items = [];

    items.push(this.Item("None", this.loc.PropertyEnum.StiInteractionOnHoverNone, null, "None"));
    items.push(this.Item("ShowDashboard", this.loc.PropertyEnum.StiInteractionOnClickShowDashboard, null, "ShowDashboard"));

    if (interactionIdent == "Chart" || interactionIdent == "RegionMap" || interactionIdent == "Text" || interactionIdent == "Image" || interactionIdent == "Indicator")
        items.push(this.Item("OpenHyperlink", this.loc.PropertyEnum.StiInteractionOnClickOpenHyperlink, null, "OpenHyperlink"));

    if (interactionIdent != "Text" && interactionIdent != "Image" && interactionIdent != "Indicator")
        items.push(this.Item("ApplyFilter", this.loc.PropertyEnum.StiInteractionOnClickApplyFilter, null, "ApplyFilter"));

    if (interactionIdent == "Chart")
        items.push(this.Item("DrillDown", this.loc.Dashboard.DrillDown, null, "DrillDown"));

    return items;
}

StiMobileDesigner.prototype.GetInsertExpressionItems = function (interactionIdent, columnNames, chartIsRange) {
    var items = [];

    if (interactionIdent == "RegionMap")
        items.push(this.Item("Ident", "Ident", null, "Ident"));

    if (interactionIdent == "Chart")
        items.push(this.Item("Argument", this.loc.PropertyMain.Argument, null, "Argument"));

    if (interactionIdent != "Image")
        items.push(this.Item("Value", this.loc.PropertyMain.Value, null, "Value"));

    if (interactionIdent == "Chart" && chartIsRange)
        items.push(this.Item("EndValue", this.loc.PropertyMain.EndValue, null, "EndValue"));

    if (interactionIdent == "Chart" || interactionIdent == "Indicator")
        items.push(this.Item("Series", this.loc.PropertyMain.Series, null, "Series"));

    if (interactionIdent == "Indicator")
        items.push(this.Item("Target", this.loc.PropertyMain.Target, null, "Target"));

    if (columnNames) {
        for (var i = 0; i < columnNames.length; i++) {
            var key = "Row." + columnNames[i];
            items.push(this.Item("column_" + columnNames[i], key, null, key));
        }
    }

    if (interactionIdent == "expandExpression") {
        items = [];
        items.push(this.Item("index", "index", null, "index"));
        items.push(this.Item("value", "value", null, "value"));
    }

    return items;
}

StiMobileDesigner.prototype.GetIconAlignmentItems = function (withCenter) {
    var items = [];
    items.push(this.Item("None", this.loc.PropertyEnum.StiIconAlignmentNone, null, "None"));
    items.push(this.Item("Left", this.loc.PropertyEnum.StiIconAlignmentLeft, null, "Left"));
    items.push(this.Item("Right", this.loc.PropertyEnum.StiIconAlignmentRight, null, "Right"));
    items.push(this.Item("Top", this.loc.PropertyEnum.StiIconAlignmentTop, null, "Top"));
    items.push(this.Item("Bottom", this.loc.PropertyEnum.StiIconAlignmentBottom, null, "Bottom"));

    if (withCenter)
        items.push(this.Item("Center", this.loc.PropertyEnum.StiHorAlignmentCenter, null, "Center"));

    return items;
}

StiMobileDesigner.prototype.GetTargetIconAlignmentItems = function () {
    var items = [];
    items.push(this.Item("None", this.loc.PropertyEnum.StiIconAlignmentNone, null, "None"));
    items.push(this.Item("Left", this.loc.PropertyEnum.StiIconAlignmentLeft, null, "Left"));
    items.push(this.Item("Right", this.loc.PropertyEnum.StiIconAlignmentRight, null, "Right"));

    return items;
}

StiMobileDesigner.prototype.GetSummaryColumnTypeItems = function () {
    var items = [];
    items.push(this.Item("Sum", "Sum", null, "Sum"));
    items.push(this.Item("Min", "Min", null, "Min"));
    items.push(this.Item("Max", "Max", null, "Max"));
    items.push(this.Item("Count", "Count", null, "Count"));
    items.push(this.Item("Average", "Average", null, "Average"));

    return items;
}

StiMobileDesigner.prototype.GetIndicatorConditionsFieldIsItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyMain.Value, null, "Value"));
    items.push(this.Item("item1", this.loc.PropertyMain.Series, null, "Series"));
    items.push(this.Item("item2", this.loc.PropertyMain.Target, null, "Target"));
    items.push(this.Item("item3", this.loc.PropertyMain.Variation, null, "Variation"));

    return items;
}

StiMobileDesigner.prototype.GetProgressConditionsFieldIsItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyMain.Value, null, "Value"));
    items.push(this.Item("item1", this.loc.PropertyMain.Series, null, "Series"));
    items.push(this.Item("item2", this.loc.PropertyMain.Target, null, "Target"));
    items.push(this.Item("item3", this.loc.PropertyEnum.StiTargetModePercentage, null, "Percentage"));

    return items;
}

StiMobileDesigner.prototype.GetCosmoDBApiItems = function () {
    var items = [];
    items.push(this.Item("item0", "SQL", null, "SQL"));
    items.push(this.Item("item1", "MongoDB", null, "MongoDB"));

    return items;
}

StiMobileDesigner.prototype.GetTargetModeItems = function () {
    var items = [];
    items.push(this.Item("Percentage", this.loc.PropertyEnum.StiTargetModePercentage, null, "Percentage"));
    items.push(this.Item("Variation", this.loc.PropertyEnum.StiTargetModeVariation, null, "Variation"));

    return items;
}

StiMobileDesigner.prototype.GetTrendLinesTypeItems = function () {
    var items = [];
    items.push(this.Item("None", this.loc.PropertyEnum.StiChartTrendLineTypeNone, null, "None"));
    items.push(this.Item("Exponential", this.loc.PropertyEnum.StiChartTrendLineTypeExponential, null, "Exponential"));
    items.push(this.Item("Linear", this.loc.PropertyEnum.StiChartTrendLineTypeLinear, null, "Linear"));
    items.push(this.Item("Logarithmic", this.loc.PropertyEnum.StiChartTrendLineTypeLogarithmic, null, "Logarithmic"));

    return items;
}

StiMobileDesigner.prototype.GetLineStyleItems = function () {
    var items = [];
    items.push(this.Item("borderStyleSolid", this.loc.PropertyEnum.StiPenStyleSolid, "BorderStyleSolid.png", "Solid"));
    items.push(this.Item("borderStyleDash", this.loc.PropertyEnum.StiPenStyleDash, "BorderStyleDash.png", "Dash"));
    items.push(this.Item("borderStyleDashDot", this.loc.PropertyEnum.StiPenStyleDashDot, "BorderStyleDashDot.png", "DashDot"));
    items.push(this.Item("borderStyleDashDotDot", this.loc.PropertyEnum.StiPenStyleDashDotDot, "BorderStyleDashDotDot.png", "DashDotDot"));
    items.push(this.Item("borderStyleDot", this.loc.PropertyEnum.StiPenStyleDot, "BorderStyleDot.png", "Dot"));
    items.push(this.Item("borderStyleDouble", this.loc.PropertyEnum.StiPenStyleDouble, "BorderStyleDouble.png", "Double"));
    items.push(this.Item("borderStyleNone", this.loc.PropertyEnum.StiPenStyleNone, "BorderStyleNone.png", "None"));

    return items;
}

StiMobileDesigner.prototype.GetKeepDetailsItems = function () {
    var items = [];
    items.push(this.Item("None", this.loc.PropertyEnum.StiKeepDetailsNone, null, "None"));
    items.push(this.Item("KeepFirstRowTogether", this.loc.PropertyEnum.StiKeepDetailsKeepFirstRowTogether, null, "KeepFirstRowTogether"));
    items.push(this.Item("KeepFirstDetailTogether", this.loc.PropertyEnum.StiKeepDetailsKeepFirstDetailTogether, null, "KeepFirstDetailTogether"));
    items.push(this.Item("KeepDetailsTogether", this.loc.PropertyEnum.StiKeepDetailsKeepDetailsTogether, null, "KeepDetailsTogether"));

    return items;
}

StiMobileDesigner.prototype.GetShowEdgeValuesItems = function (notAuto) {
    var items = [];
    items.push(this.Item("True", this.loc.PropertyEnum.StiExtendedStyleBoolTrue, null, "True"));
    items.push(this.Item("False", this.loc.PropertyEnum.StiExtendedStyleBoolFalse, null, "False"));

    if (!notAuto)
        items.push(this.Item("Auto", this.loc.PropertyEnum.StiFontSizeModeAuto, null, "Auto"));

    return items;
}

StiMobileDesigner.prototype.GetShowZerosOrNullsItems = function () {
    var items = [];
    items.push(this.Item("Zero", this.loc.PropertyEnum.StiEmptyCellsAsZero, null, "Zero"));
    items.push(this.Item("Gap", this.loc.PropertyEnum.StiEmptyCellsAsGap, null, "Gap"));
    items.push(this.Item("ConnectPointsWithLine", this.loc.PropertyEnum.StiEmptyCellsAsConnectPointsWithLine, null, "ConnectPointsWithLine"));

    return items;
}

StiMobileDesigner.prototype.GetShowZerosOrNullsInSimpleWayItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.FormFormatEditor.nameTrue, null, "Zero"));
    items.push(this.Item("item1", this.loc.FormFormatEditor.nameFalse, null, "Gap"));

    return items;
}

StiMobileDesigner.prototype.GetNewReportDictionaryItems = function () {
    var items = [];
    items.push(this.Item("DictionaryNew", this.loc.FormDictionaryDesigner.DictionaryNew.replace("...", ""), null, "DictionaryNew"));
    items.push(this.Item("DictionaryMerge", this.loc.FormDictionaryDesigner.DictionaryMerge.replace("...", ""), null, "DictionaryMerge"));

    return items;
}

StiMobileDesigner.prototype.GetFontSizeModeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.loc.PropertyMain.Auto, null, "Auto"));
    items.push(this.Item("item1", this.loc.PropertyMain.Value, null, "Value"));
    items.push(this.Item("item2", this.loc.PropertyMain.Target, null, "Target"));

    return items;
}

StiMobileDesigner.prototype.GetChartEditorTypeItems = function () {
    var items = [];
    items.push(this.Item("Simple", this.loc.PropertyMain.Simple, null, "Simple"));
    items.push(this.Item("Advanced", this.loc.PropertyMain.Advanced, null, "Advanced"));

    return items;
}

StiMobileDesigner.prototype.GetAllowCustomColorsItems = function () {
    var items = [];
    items.push(this.Item("FromStyle", this.loc.FormStyleDesigner.FromStyle, null, "FromStyle"));
    items.push(this.Item("Custom", this.loc.FormFormatEditor.Custom, null, "Custom"));

    return items;
}

StiMobileDesigner.prototype.GetImageProcessingDuplicatesItems = function () {
    var items = [];
    items.push(this.Item("None", this.loc.PropertyEnum.StiImageProcessingDuplicatesTypeNone, null, "None"));
    items.push(this.Item("Merge", this.loc.PropertyEnum.StiImageProcessingDuplicatesTypeMerge, null, "Merge"));
    items.push(this.Item("Hide", this.loc.PropertyEnum.StiImageProcessingDuplicatesTypeHide, null, "Hide"));
    items.push(this.Item("RemoveImage", this.loc.PropertyEnum.StiImageProcessingDuplicatesTypeRemoveImage, null, "RemoveImage"));
    items.push(this.Item("GlobalMerge", this.loc.PropertyEnum.StiImageProcessingDuplicatesTypeGlobalMerge, null, "GlobalMerge"));
    items.push(this.Item("GlobalHide", this.loc.PropertyEnum.StiImageProcessingDuplicatesTypeGlobalHide, null, "GlobalHide"));
    items.push(this.Item("GlobalRemoveImage", this.loc.PropertyEnum.StiImageProcessingDuplicatesTypeGlobalRemoveImage, null, "GlobalRemoveImage"));

    return items;
}

StiMobileDesigner.prototype.GetInsteadNullValuesItems = function () {
    var items = [];
    items.push(this.Item(" ", " ", null, ""));
    items.push(this.Item("-", "-", null, "-"));
    items.push(this.Item("None", "None", null, "None"));
    items.push(this.Item("Absent", "Absent", null, "Absent"));
    items.push(this.Item("Empty", "Empty", null, "Empty"));
    items.push(this.Item("N/A", "N/A", null, "N/A"));

    return items;
}

StiMobileDesigner.prototype.GetTableHeaderFormatItems = function () {
    var items = [];
    items.push(this.Item("N", "N", null, "N"));
    items.push(this.Item("N2", "N2", null, "N2"));
    items.push(this.Item("C", "C", null, "C"));
    items.push(this.Item("C2", "C2", null, "C2"));
    items.push(this.Item("P", "P", null, "P"));
    items.push(this.Item("P2", "P2", null, "P2"));
    items.push(this.Item("E", "E", null, "E"));
    items.push(this.Item("E2", "E2", null, "E2"));
    items.push(this.Item("F", "F", null, "F"));
    items.push(this.Item("F2", "F2", null, "F2"));
    items.push(this.Item("MM/dd/yyyy", "MM/dd/yyyy", null, "MM/dd/yyyy"));
    items.push(this.Item("MMMM dd", "MMMM dd", null, "MMMM dd"));
    items.push(this.Item("yyyy MMMM", "yyyy MMMM", null, "yyyy MMMM"));

    return items;
}

StiMobileDesigner.prototype.GetLegendVisibleItems = function () {
    var items = [];
    items.push(this.Item("Always", this.loc.PropertyMain.Always, null, "Always"));
    items.push(this.Item("Auto", this.loc.PropertyMain.Auto, null, "Auto"));
    items.push(this.Item("False", this.loc.FormFormatEditor.nameFalse, null, "False"));

    return items;
}

StiMobileDesigner.prototype.GetDashboardContentAlignmentItems = function () {
    var items = [];
    items.push(this.Item("Left", this.loc.PropertyEnum.StiDashboardContentAlignmentLeft, null, "Left"));
    items.push(this.Item("Center", this.loc.PropertyEnum.StiDashboardContentAlignmentCenter, null, "Center"));
    items.push(this.Item("Right", this.loc.PropertyEnum.StiDashboardContentAlignmentRight, null, "Right"));
    items.push(this.Item("StretchX", this.loc.PropertyEnum.StiDashboardContentAlignmentStretchX, null, "StretchX"));
    items.push(this.Item("StretchXY", this.loc.PropertyEnum.StiDashboardContentAlignmentStretchXY, null, "StretchXY"));

    return items;
}

StiMobileDesigner.prototype.GetPrintOnEvenOddPagesItems = function () {
    var items = [];
    items.push(this.Item("Ignore", this.loc.PropertyEnum.StiPrintOnEvenOddPagesTypeIgnore, null, "Ignore"));
    items.push(this.Item("PrintOnEvenPages", this.loc.PropertyEnum.StiPrintOnEvenOddPagesTypePrintOnEvenPages, null, "PrintOnEvenPages"));
    items.push(this.Item("PrintOnOddPages", this.loc.PropertyEnum.StiPrintOnEvenOddPagesTypePrintOnOddPages, null, "PrintOnOddPages"));

    return items;
}

StiMobileDesigner.prototype.GetAzureBlobContentTypeItems = function () {
    var items = [];
    items.push(this.Item("Empty", "", null, ""));
    items.push(this.Item("CSV", "CSV", null, "CSV"));
    items.push(this.Item("Excel", "Excel", null, "Excel"));
    items.push(this.Item("JSON", "JSON", null, "JSON"));
    items.push(this.Item("XML", "XML", null, "XML"));

    return items;
}

StiMobileDesigner.prototype.GetHeatmapModeItems = function () {
    var items = [];
    items.push(this.Item("Lightness", "Lightness", null, "Lightness"));
    items.push(this.Item("Darkness", "Darkness", null, "Darkness"));

    return items;
}

StiMobileDesigner.prototype.GetDeveloperRoleItems = function () {
    var items = [];
    items.push(this.Item("OwnerDeveloper", this.loc.Cloud.TextOwner + " " + this.loc.Desktop.Developer, null, "OwnerDeveloper"));
    items.push(this.Item("Owner", this.loc.Cloud.TextOwner, null, "Owner"));
    items.push(this.Item("Developer", this.loc.Desktop.Developer, null, "Developer"));

    return items;
}

StiMobileDesigner.prototype.GglAnalyticDateType = function () {
    var items = [];
    items.push(this.Item("Today", this.loc.DatePickerRanges.Today, null, "Today"));
    items.push(this.Item("Yesterday", this.loc.DatePickerRanges.Yesterday, null, "Yesterday"));
    items.push(this.Item("DaysAgo", this.loc.DatePickerRanges.DaysAgo.replace("{0}", "N"), null, "DaysAgo"));
    items.push(this.Item("Custom", this.loc.FormFormatEditor.Custom, null, "Custom"));

    return items;
}

StiMobileDesigner.prototype.GetBorderSizeItems = function () {
    var items = [];
    items.push(this.Item("item1", "1", null, "1"));
    items.push(this.Item("item2", "2", null, "2"));
    items.push(this.Item("item3", "3", null, "3"));
    items.push(this.Item("item4", "4", null, "4"));
    items.push(this.Item("item5", "5", null, "5"));

    return items;
}

StiMobileDesigner.prototype.GetManualDataContextMenuItems = function () {
    var items = [];
    items.push(this.Item("InsertRowAbove", this.loc.MainMenu.menuInsertRowAbove, null, "InsertRowAbove"));
    items.push(this.Item("InsertRowBelow", this.loc.MainMenu.menuInsertRowBelow, null, "InsertRowBelow"));
    items.push("separator1");
    items.push(this.Item("MoveUp", this.loc.QueryBuilder.MoveUp, null, "MoveUp"));
    items.push(this.Item("MoveDown", this.loc.QueryBuilder.MoveDown, null, "MoveDown"));
    items.push("separator2");
    items.push(this.Item("DeleteRow", this.loc.MainMenu.menuDeleteRow, null, "DeleteRow"));

    return items;
}

StiMobileDesigner.prototype.GetDefaultScriptModeItems = function () {
    var items = [];
    items.push(this.Item("Blocks", this.loc.PropertyMain.Blocks, null, "Blocks"));
    items.push(this.Item("Code", this.loc.PropertyMain.Code, null, "Code"));

    return items;
}

StiMobileDesigner.prototype.GetButtonTypeItems = function () {
    var items = [];
    items.push(this.Item("Button", this.loc.Dialogs.StiButtonControl, null, "Button"));
    items.push(this.Item("RadioButton", this.loc.Dialogs.StiRadioButtonControl, null, "RadioButton"));
    items.push(this.Item("CheckBox", this.loc.Dialogs.StiCheckBoxControl, null, "CheckBox"));

    return items;
}

StiMobileDesigner.prototype.GetButtonShapeTypeItems = function () {
    var items = [];
    items.push(this.Item("Rectangle", this.loc.Shapes.Rectangle, null, "Rectangle"));
    items.push(this.Item("Circle", this.loc.PropertyEnum.StiMarkerTypeCircle, null, "Circle"));

    return items;
}

StiMobileDesigner.prototype.GetButtonStretchItems = function () {
    var items = [];
    items.push(this.Item("StretchX", this.loc.PropertyEnum.StiDashboardContentAlignmentStretchX, null, "StretchX"));
    items.push(this.Item("StretchXY", this.loc.PropertyEnum.StiDashboardContentAlignmentStretchXY, null, "StretchXY"));

    return items;
}

StiMobileDesigner.prototype.GetDataPriorityItems = function () {
    var items = [];
    items.push(this.Item("BeforeTransformation", this.loc.Dashboard.BeforeTransformation, null, "BeforeTransformation"));
    items.push(this.Item("AfterGroupingData", this.loc.Dashboard.AfterGroupingData, null, "AfterGroupingData"));
    items.push(this.Item("AfterSortingData", this.loc.Dashboard.AfterSortingData, null, "AfterSortingData"));

    return items;
}

StiMobileDesigner.prototype.GetOptions3DLightingItems = function () {
    var items = [];
    items.push(this.Item("No", this.loc.Report.No, null, "No"));
    items.push(this.Item("Solid", this.loc.Report.StiSolidBrush, null, "Solid"));
    items.push(this.Item("Gradient", this.loc.Report.StiGradientBrush, null, "Gradient"));

    return items;
}


StiMobileDesigner.prototype.GetFormatsDictionaryItems = function () {
    var items = [];
    items.push(this.Item("general", this.loc.FormFormatEditor.General, "Dictionary.FormatGeneral.png", "{Format(\"{0}\", \"\")}"));
    items.push(this.Item("number", this.loc.FormFormatEditor.Number, "Dictionary.FormatNumber.png", "{Format(\"{0:N2}\", \"\")}"));
    items.push(this.Item("currency", this.loc.FormFormatEditor.Currency, "Dictionary.FormatCurrency.png", "{Format(\"{0:C2}\", \"\")}"));
    items.push(this.Item("date", this.loc.FormFormatEditor.Date, "Dictionary.FormatDate.png", "{Format(\"{0:MM.dd.yyyy}\", \"\")}"));
    items.push(this.Item("time", this.loc.FormFormatEditor.Time, "Dictionary.FormatTime.png", "{Format(\"{0:HH:mm}\", \"\")}"));
    items.push(this.Item("percentage", this.loc.FormFormatEditor.Percentage, "Dictionary.FormatPercentage.png", "{Format(\"{0:P2}\", \"\")}"));
    items.push(this.Item("boolean", this.loc.FormFormatEditor.Boolean, "Dictionary.FormatBoolean.png", "{Format(\"{0}\", \"\")}"));
    items.push(this.Item("custom", this.loc.FormFormatEditor.Custom, "Dictionary.Format.png", "{Format(\"{0}\", \"\")}"));

    return items;
}


StiMobileDesigner.prototype.GetHtmlTagsDictionaryItems = function () {
    var items = [];
    items.push(this.Item("1", "<b> </b>", null, "<b></b>"));
    items.push(this.Item("2", "<i> </i>", null, "<i></i>"));
    items.push(this.Item("3", "<u> </u>", null, "<u></u>"));
    items.push(this.Item("4", "<s> </s>", null, "<s></s>"));
    items.push(this.Item("5", "<sub> </sub>", null, "<sub></sub>"));
    items.push(this.Item("6", "<sup> </sup>", null, "<sup></sup>"));
    items.push(this.Item("7", "<font color=\"red\" face=\"Arial\" size=\"8\"> </font>", null, "<font color=\"red\" face=\"Arial\" size=\"8\"> </font>"));
    items.push(this.Item("8", "<font-name=\"Arial\"> </font-name>", null, "<font-name=\"Arial\"></font-name>"));
    items.push(this.Item("9", "<font-size=\"8\"> </font-size>", null, "<font-size=\"8\"></font-size>"));
    items.push(this.Item("10", "<font-color=\"red\"> </font-color>", null, "<font-color=\"red\"></font-color>"));
    items.push(this.Item("11", "<color=\"red\"> </color>", null, "<color=\"red\"></color>"));
    items.push(this.Item("12", "<background-color=\"red\"> </background-color>", null, "<background-color=\"red\"></background-color>"));
    items.push(this.Item("13", "<letter-spacing=\"0\"> </letter-spacing>", null, "<letter-spacing=\"0\"></letter-spacing>"));
    items.push(this.Item("14", "<word-spacing=\"0\"> </word-spacing>", null, "<word-spacing=\"0\"></word-spacing>"));
    items.push(this.Item("15", "<line-height=\"1\"> </line-height>", null, "<line-height=\"1\"></line-height>"));
    items.push(this.Item("16", "<text-align=\"left\"> </text-align>", null, "<text-align=\"left\"></text-align>"));
    items.push(this.Item("17", "<text-align=\"center\"> </text-align>", null, "<text-align=\"center\"></text-align>"));
    items.push(this.Item("18", "<text-align=\"right\"> </text-align>", null, "<text-align=\"right\"></text-align>"));
    items.push(this.Item("19", "<text-align=\"justify\"> </text-align>", null, "<text-align=\"justify\"></text-align>"));
    items.push(this.Item("20", "<br>", null, "<br>"));
    items.push(this.Item("21", "&amp;", null, "&amp;"));
    items.push(this.Item("22", "&lt;", null, "&lt;"));
    items.push(this.Item("23", "&gt;", null, "&gt;"));
    items.push(this.Item("24", "&quot;", null, "&quot;"));
    items.push(this.Item("25", "&nbsp;", null, "&nbsp;"));


    return items;
}

StiMobileDesigner.prototype.GetTitleSizeModeItems = function () {
    var items = [];
    items.push(this.Item("Fit", this.loc.PropertyEnum.StiTextSizeModeFit, null, "Fit"));
    items.push(this.Item("WordWrap", this.loc.PropertyEnum.StiTextSizeModeWordWrap, null, "WordWrap"));
    items.push(this.Item("Trimming", this.loc.PropertyEnum.StiTextSizeModeTrimming, null, "Trimming"));

    return items;
}

StiMobileDesigner.prototype.GetBarCodeBodyShapeItems = function () {
    var items = [];
    items.push(this.Item("Square", this.loc.PropertyEnum.StiQRCodeBodyShapeTypeSquare, null, "Square"));
    items.push(this.Item("RoundedSquare", this.loc.PropertyEnum.StiQRCodeBodyShapeTypeRoundedSquare, null, "RoundedSquare"));
    items.push(this.Item("Dot", this.loc.PropertyEnum.StiQRCodeBodyShapeTypeDot, null, "Dot"));
    items.push(this.Item("Circle", this.loc.PropertyEnum.StiQRCodeBodyShapeTypeCircle, null, "Circle"));
    items.push(this.Item("Diamond", this.loc.PropertyEnum.StiQRCodeBodyShapeTypeDiamond, null, "Diamond"));
    items.push(this.Item("Star", this.loc.PropertyEnum.StiQRCodeBodyShapeTypeStar, null, "Star"));
    items.push(this.Item("ZebraHorizontal", this.loc.PropertyEnum.StiQRCodeBodyShapeTypeZebraHorizontal, null, "ZebraHorizontal"));
    items.push(this.Item("ZebraVertical", this.loc.PropertyEnum.StiQRCodeBodyShapeTypeZebraVertical, null, "ZebraVertical"));
    items.push(this.Item("ZebraCross1", this.loc.PropertyEnum.StiQRCodeBodyShapeTypeZebraCross + " 1", null, "ZebraCross1"));
    items.push(this.Item("ZebraCross2", this.loc.PropertyEnum.StiQRCodeBodyShapeTypeZebraCross + " 2", null, "ZebraCross2"));
    items.push(this.Item("Circular", this.loc.PropertyEnum.StiQRCodeBodyShapeTypeCircular, null, "Circular"));
    items.push(this.Item("DockedDiamonds", this.loc.PropertyEnum.StiQRCodeBodyShapeTypeDockedDiamonds, null, "DockedDiamonds"));

    return items;
}

StiMobileDesigner.prototype.GetBarCodeEyeBallShapeItems = function () {
    var items = [];
    items.push(this.Item("Square", this.loc.PropertyEnum.StiQRCodeEyeBallShapeTypeSquare, null, "Square"));
    items.push(this.Item("Dots", this.loc.PropertyEnum.StiQRCodeEyeBallShapeTypeDots, null, "Dots"));
    items.push(this.Item("Circle", this.loc.PropertyEnum.StiQRCodeEyeBallShapeTypeCircle, null, "Circle"));
    items.push(this.Item("Round", this.loc.PropertyEnum.StiQRCodeEyeBallShapeTypeRound, null, "Round"));
    items.push(this.Item("Round1", this.loc.PropertyEnum.StiQRCodeEyeBallShapeTypeRound + " 1", null, "Round1"));
    items.push(this.Item("Round3", this.loc.PropertyEnum.StiQRCodeEyeBallShapeTypeRound + " 3", null, "Round3"));
    items.push(this.Item("Star", this.loc.PropertyEnum.StiQRCodeEyeBallShapeTypeStar, null, "Star"));
    items.push(this.Item("ZebraHorizontal", this.loc.PropertyEnum.StiQRCodeEyeBallShapeTypeZebraHorizontal, null, "ZebraHorizontal"));
    items.push(this.Item("ZebraVertical", this.loc.PropertyEnum.StiQRCodeEyeBallShapeTypeZebraVertical, null, "ZebraVertical"));

    return items;
}

StiMobileDesigner.prototype.GetBarCodeEyeFrameShapeItems = function () {
    var items = [];
    items.push(this.Item("Square", this.loc.PropertyEnum.StiQRCodeEyeBallShapeTypeSquare, null, "Square"));
    items.push(this.Item("Dots", this.loc.PropertyEnum.StiQRCodeEyeBallShapeTypeDots, null, "Dots"));
    items.push(this.Item("Circle", this.loc.PropertyEnum.StiQRCodeEyeBallShapeTypeCircle, null, "Circle"));
    items.push(this.Item("Round", this.loc.PropertyEnum.StiQRCodeEyeBallShapeTypeRound, null, "Round"));
    items.push(this.Item("Round1", this.loc.PropertyEnum.StiQRCodeEyeBallShapeTypeRound + " 1", null, "Round1"));
    items.push(this.Item("Round3", this.loc.PropertyEnum.StiQRCodeEyeBallShapeTypeRound + " 3", null, "Round3"));

    return items;
}

StiMobileDesigner.prototype.GetHtmlPreviewModeItems = function () {
    var items = [];
    items.push(this.Item("Div", "DIV", null, "Div"));
    items.push(this.Item("Table", "TABLE", null, "Table"));

    return items;
}

StiMobileDesigner.prototype.GetGisDataTypeItems = function () {
    var items = [];
    items.push(this.Item("item0", "WKT", null, "Wkt"));
    items.push(this.Item("item1", "GeoJSON", null, "GeoJSON"));

    return items;
}

StiMobileDesigner.prototype.GetSummaryRunningItems = function () {
    var items = [];
    items.push(this.Item("report", this.loc.FormSystemTextEditor.SummaryRunningByReport, null, "report"));
    items.push(this.Item("page", this.loc.FormSystemTextEditor.SummaryRunningByPage, null, "page"));
    items.push(this.Item("column", this.loc.FormSystemTextEditor.SummaryRunningByColumn, null, "column"));

    return items;
}

StiMobileDesigner.prototype.GetColumnShapeItems = function () {
    var items = [];
    items.push(this.Item("Box", this.loc.PropertyEnum.StiColumnShape3DBox, null, "Box"));
    items.push(this.Item("Pyramid", this.loc.PropertyEnum.StiColumnShape3DPyramid, null, "Pyramid"));
    items.push(this.Item("PartialPyramid", this.loc.PropertyEnum.StiColumnShape3DPartialPyramid, null, "PartialPyramid"));

    return items;
}

StiMobileDesigner.prototype.GetViewsStateInteractionItems = function () {
    var items = [];
    items.push(this.Item("Always", this.loc.PropertyMain.Always, null, "Always"));
    items.push(this.Item("OnHover", this.loc.PropertyMain.OnHover, null, "OnHover"));

    return items;
}