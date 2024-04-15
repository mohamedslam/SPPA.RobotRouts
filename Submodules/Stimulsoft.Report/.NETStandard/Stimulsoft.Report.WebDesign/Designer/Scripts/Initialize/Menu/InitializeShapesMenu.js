
StiMobileDesigner.prototype.ShapesMenu = function (menuName, parentButton, withoutPrimitives, isDashboardElement, isToolboxMenu) {
    var menu = isToolboxMenu
        ? this.HorizontalMenu(menuName, parentButton, "Right")
        : this.VerticalMenu(menuName, parentButton, "Down");

    menu.innerContent.style.minWidth = "240px";
    menu.innerContent.style.lineHeight = "0";
    menu.items = {};

    if (isToolboxMenu) {
        menu.type = "Menu";
        menu.innerContent.style.width = "240px";
    }

    if (!withoutPrimitives) {
        var basicShapesHeader = this.FormBlockHeader(this.loc.Shapes.BasicShapes);
        basicShapesHeader.style.display = "none";
        menu.innerContent.appendChild(basicShapesHeader);

        var basicShapes = ["StiHorizontalLinePrimitive", "StiVerticalLinePrimitive", "StiRectanglePrimitive", "StiRoundedRectanglePrimitive"];

        for (var i = 0; i < basicShapes.length; i++) {
            if (this.options.visibilityComponents[basicShapes[i]]) {
                basicShapesHeader.style.display = "";
                var button = this.ShapesMenuButton(menu, basicShapes[i], "Shapes." + basicShapes[i] + ".png", this.loc.HelpComponents[basicShapes[i]]);
                button.key = basicShapes[i];
                menu.innerContent.appendChild(button);
                menu.items[basicShapes[i]] = button;

                this.AddDragEventsToComponentButton(button);
            }
        }
    }

    if (this.options.visibilityComponents.StiShape) {

        var otherShapes = [
            {
                category: this.loc.Shapes.EquationShapes,
                items: ["StiPlusShapeType", "StiMinusShapeType", "StiMultiplyShapeType", "StiDivisionShapeType", "StiEqualShapeType"]
            },
            {
                category: this.loc.Shapes.BlockArrows,
                items: ["StiArrowShapeTypeRight", "StiArrowShapeTypeLeft", "StiArrowShapeTypeUp", "StiArrowShapeTypeDown", "StiComplexArrowShapeType",
                    "StiFlowchartSortShapeType", "StiBentArrowShapeType", "StiChevronShapeType"]
            },
            {
                category: this.loc.Shapes.Lines,
                items: ["StiDiagonalUpLineShapeType", "StiDiagonalDownLineShapeType", "StiHorizontalLineShapeType", "StiLeftAndRightLineShapeType",
                    "StiTopAndBottomLineShapeType", "StiVerticalLineShapeType"]
            },
            {
                category: this.loc.Shapes.Flowchart,
                items: ["StiOvalShapeType", "StiRectangleShapeType", "StiRoundedRectangleShapeType", "StiTriangleShapeType", "StiFlowchartCardShapeType",
                    "StiFlowchartCollateShapeType", "StiFlowchartDecisionShapeType", "StiFlowchartManualInputShapeType", "StiFlowchartOffPageConnectorShapeType",
                    "StiFlowchartPreparationShapeType", "StiFrameShapeType", "StiParallelogramShapeType", "StiRegularPentagonShapeType", "StiTrapezoidShapeType",
                    "StiOctagonShapeType", "StiSnipSameSideCornerRectangleShapeType", "StiSnipDiagonalSideCornerRectangleShapeType"]
            }
        ]

        for (var i = 0; i < otherShapes.length; i++) {
            var header = this.FormBlockHeader(otherShapes[i].category);
            menu.innerContent.appendChild(header);

            for (var k = 0; k < otherShapes[i].items.length; k++) {
                var toolTip = this.loc.Shapes[otherShapes[i].items[k].replace("ShapeType", "").replace("Sti", "")];
                if (otherShapes[i].items[k].indexOf("StiArrowShapeType") == 0) toolTip = this.loc.Shapes.Arrow;

                var button = this.ShapesMenuButton(menu, (isDashboardElement ? "StiShapeElement;" : "StiShape;") + otherShapes[i].items[k], "Shapes." + otherShapes[i].items[k] + ".png", toolTip);
                button.key = otherShapes[i].items[k];
                button.imageCell.style.padding = "0";
                menu.innerContent.appendChild(button);
                menu.items[otherShapes[i].items[k]] = button;

                this.AddDragEventsToComponentButton(button);
            }
        }
    }

    menu.action = function (menuItem) {
        this.changeVisibleState(false);
        var panel = isToolboxMenu ? this.jsObject.options.toolbox : this.jsObject.options.insertPanel;
        panel.resetChoose();
        panel.setChoose(menuItem);
    }

    return menu;
}

StiMobileDesigner.prototype.ShapesMenuButton = function (menu, name, imageName, toolTip) {
    var button = this.StandartSmallButton(null, null, null, imageName, toolTip, null, true);
    button.style.display = "inline-block";
    button.name = name;
    button.style.width = button.style.height = "26px";
    button.innerTable.style.width = "100%";
    button.style.margin = "1px";

    button.action = function () {
        menu.action(this);
    }

    return button;
}