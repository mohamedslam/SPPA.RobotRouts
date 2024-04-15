
StiMobileDesigner.prototype.InitializeWizardLabelsPanel = function (templates, form, level) {
    var labelsPanel = document.createElement("div");
    labelsPanel.name = name;
    labelsPanel.className = "wizardStepPanel";
    labelsPanel.jsObject = this;
    labelsPanel.style.maxHeight = "410px";
    labelsPanel.level = level;
    labelsPanel.components = {};
    if (form.typeReport == "Label") {
        labelsPanel.style.overflowX = "hidden";
        labelsPanel.style.overflowY = "hidden";
    }

    var mainTable = this.CreateHTMLTable();
    mainTable.style.marginLeft = "auto";
    mainTable.style.marginRight = "auto";
    mainTable.style.marginTop = "10px";
    mainTable.style.paddingRight = "30px";
    mainTable.style.paddingLeft = "30px";
    labelsPanel.appendChild(mainTable);

    var comboBoxUnitItems = [{ caption: this.loc.PropertyEnum.StiReportUnitTypeCentimeters, name: "cm", key: "cm" },
    { caption: this.loc.PropertyEnum.StiReportUnitTypeMillimeters, name: "mm", key: "mm" },
    { caption: this.loc.PropertyEnum.StiReportUnitTypeInches, name: "in", key: "in" },
    { caption: this.loc.PropertyEnum.StiReportUnitTypeHundredthsOfInch, name: "hi", key: "hi" }];

    var comboBoxDirectionItems = [{ caption: this.loc.PropertyEnum.StiColumnDirectionAcrossThenDown, name: "0", key: "0" },
    { caption: this.loc.PropertyEnum.StiColumnDirectionDownThenAcross, name: "1", key: "1" }];

    labelsPanel.update = function (needReset) {
        if (needReset && !labelsPanel.unit) {
            labelsPanel.unit = "cm";
            var data = form.jsObject.options.wizardData[form.typeReport];

            mainTable.addTextCellInLastRow(labelsPanel.jsObject.loc.Wizards.LabelLabelType).className = "wizardLablesText";
            var label = data.label[labelsPanel.unit][0];
            var cell = mainTable.addCellInLastRow(labelsPanel.components.treeViewBoxLabelType = labelsPanel.jsObject.DropDownList("treeViewBoxLabelType", 370, null, [{ caption: label.caption, name: "label", key: label }], true));
            labelsPanel.components.treeViewBoxLabelType.menu.innerContent.childNodes[0].action();
            cell.align = "right";
            cell.style.paddingRight = "100px";
            mainTable.addCellInLastRow(labelsPanel.components.comboBoxUnit = labelsPanel.jsObject.DropDownList("comboBoxUnit", 200, null, comboBoxUnitItems, true));

            cell = mainTable.addCellInNextRow();
            cell.style.borderBottom = "1px dotted #c6c6c6";
            cell.colSpan = 3;

            labelsPanel.createRow(labelsPanel.jsObject.loc.Wizards.LabelWidth, "textBoxWidth", "labelUnit0", mainTable);
            mainTable.addCellInLastRow(labelsPanel.components.panelPreview = document.createElement("div")).rowSpan = 14;
            labelsPanel.components.panelPreview.style.height = "300px";
            labelsPanel.components.panelPreview.style.width = "190px";
            labelsPanel.components.panelPreview.style.border = "1px solid gray";
            labelsPanel.createRow(labelsPanel.jsObject.loc.Wizards.LabelHeight, "textBoxHeight", "labelUnit1", mainTable);
            labelsPanel.createRow(labelsPanel.jsObject.loc.Wizards.LabelHorizontalGap, "textBoxHorizontal", "labelUnit2", mainTable);
            labelsPanel.createRow(labelsPanel.jsObject.loc.Wizards.LabelVerticalGap, "textBoxVertical", "labelUnit3", mainTable);

            cell = mainTable.addCellInNextRow();
            cell.style.borderBottom = "1px dotted #c6c6c6";
            cell.colSpan = 2;

            var paper = data.paper[labelsPanel.unit][0];
            labelsPanel.createRow(labelsPanel.jsObject.loc.Wizards.LabelSize, "comboBoxPapers", "", mainTable, labelsPanel.jsObject.DropDownList("comboBoxPapers", 200, null, [{ caption: paper.caption, name: "paper", key: paper }], true));
            labelsPanel.components.comboBoxPapers.menu.innerContent.childNodes[0].action();
            labelsPanel.createRow(labelsPanel.jsObject.loc.Wizards.LabelPageWidth, "textBoxPageWidth", "labelUnit6", mainTable);
            labelsPanel.createRow(labelsPanel.jsObject.loc.Wizards.LabelPageHeight, "textBoxPageHeight", "labelUnit7", mainTable);
            labelsPanel.createRow(labelsPanel.jsObject.loc.Wizards.LabelLeftMargin, "textBoxLeft", "labelUnit4", mainTable);
            labelsPanel.createRow(labelsPanel.jsObject.loc.Wizards.LabelTopMargin, "textBoxTop", "labelUnit5", mainTable);

            cell = mainTable.addCellInNextRow();
            cell.style.borderBottom = "1px dotted #c6c6c6";
            cell.colSpan = 2;

            labelsPanel.createRow(labelsPanel.jsObject.loc.Wizards.LabelNumberOfColumns, "numericNumberOfColumns", "", mainTable, labelsPanel.jsObject.TextBoxEnumerator(null, 75, null, false, 999999, 0));
            labelsPanel.createRow(labelsPanel.jsObject.loc.Wizards.LabelNumberOfRows, "numericNumberOfRows", "", mainTable, labelsPanel.jsObject.TextBoxEnumerator(null, 75, null, false, 999999, 0));
            labelsPanel.createRow(labelsPanel.jsObject.loc.Wizards.LabelDirection, "comboBoxDirection", "", mainTable, labelsPanel.jsObject.DropDownList("comboBoxDirection", 200, null, comboBoxDirectionItems, true));
            labelsPanel.components.comboBoxDirection.menu.innerContent.childNodes[0].action();

            labelsPanel.enableGroup(false);
            //actions
            labelsPanel.components.comboBoxUnit.action = function () {
                var item = this.menu.dropDownList.key;
                var lastUnit = labelsPanel.unit;
                labelsPanel.unit = item;

                for (var i = 0; i < 8; i++)
                    labelsPanel.components["labelUnit" + i].textContent = item;

                labelsPanel.components.textBoxWidth.value = labelsPanel.convertUnit(labelsPanel.components.textBoxWidth.value, lastUnit);
                labelsPanel.components.textBoxHeight.value = labelsPanel.convertUnit(labelsPanel.components.textBoxHeight.value, lastUnit);
                labelsPanel.components.textBoxLeft.value = labelsPanel.convertUnit(labelsPanel.components.textBoxLeft.value, lastUnit);
                labelsPanel.components.textBoxTop.value = labelsPanel.convertUnit(labelsPanel.components.textBoxTop.value, lastUnit);
                labelsPanel.components.textBoxHorizontal.value = labelsPanel.convertUnit(labelsPanel.components.textBoxHorizontal.value, lastUnit);
                labelsPanel.components.textBoxVertical.value = labelsPanel.convertUnit(labelsPanel.components.textBoxVertical.value, lastUnit);
                labelsPanel.components.textBoxPageWidth.value = labelsPanel.convertUnit(labelsPanel.components.textBoxPageWidth.value, lastUnit);
                labelsPanel.components.textBoxPageHeight.value = labelsPanel.convertUnit(labelsPanel.components.textBoxPageHeight.value, lastUnit);

                labelsPanel.updatePaper();
                labelsPanel.updateLabel();
            }

            labelsPanel.components.comboBoxDirection.action = function () {
                labelsPanel.drawPreview();
            }

            labelsPanel.components.numericNumberOfColumns.action = function () {
                labelsPanel.drawPreview();
            }

            labelsPanel.components.numericNumberOfRows.action = function () {
                labelsPanel.drawPreview();
            }

            if (data.isMetrics) labelsPanel.components.comboBoxUnit.menu.items.cm.action()
            else labelsPanel.components.comboBoxUnit.menu.items.in.action();

            form.enableButtons(true, false, false);
        } else {
            labelsPanel.drawPreview();
        }

        if (this.level > form.stepLevel)
            form.stepLevel = this.level;
    }

    labelsPanel.updatePaper = function () {
        var key = labelsPanel.components.comboBoxPapers.menu.dropDownList.key;
        var data = form.jsObject.options.wizardData[form.typeReport];
        var comboBoxPapersItems = [];
        for (var i in data.paper[labelsPanel.unit]) {
            var paper = data.paper[labelsPanel.unit][i];
            comboBoxPapersItems.push({ caption: paper.caption, name: "paper", key: paper });
        }
        var list = labelsPanel.jsObject.DropDownList("comboBoxPapers", 200, null, comboBoxPapersItems, true);
        if (key != null)
            for (var i = 0; i < list.menu.innerContent.childNodes.length; i++) {
                var item = list.menu.innerContent.childNodes[i];
                if (item.key.name == key.name) {
                    item.action();
                    break;
                }
            }

        labelsPanel.components.comboBoxPapers.parentNode.insertBefore(list, null);
        labelsPanel.components.comboBoxPapers.parentNode.removeChild(labelsPanel.components.comboBoxPapers);
        labelsPanel.components.comboBoxPapers = list;
        labelsPanel.components.comboBoxPapers.setEnabled(labelsPanel.groupEnabled);

        labelsPanel.components.comboBoxPapers.action = function () {
            if (!this.disableUpdate) {
                labelsPanel.disableUpdate = true;
                var key = labelsPanel.components.comboBoxPapers.menu.dropDownList.key;
                if (key.width != 0 && key.height != 0) {
                    labelsPanel.components.textBoxPageWidth.value = labelsPanel.convertUnit(key.width, "cm");
                    labelsPanel.components.textBoxPageHeight.value = labelsPanel.convertUnit(key.height, "cm");
                }
                labelsPanel.disableUpdate = false;
                labelsPanel.drawPreview();
            }
        }
    }

    labelsPanel.updateLabel = function () {
        var key = labelsPanel.components.treeViewBoxLabelType.menu.dropDownList.key;
        var data = form.jsObject.options.wizardData[form.typeReport];
        var treeViewBoxLabelTypeItems = [];
        for (var i in data.label[labelsPanel.unit]) {
            var label = data.label[labelsPanel.unit][i];
            treeViewBoxLabelTypeItems.push({ caption: label.caption, name: "label", key: label });
        }

        var list = labelsPanel.jsObject.DropDownList("treeViewBoxLabelType", 370, null, treeViewBoxLabelTypeItems, true);
        if (key != null)
            for (var i = 0; i < list.menu.innerContent.childNodes.length; i++) {
                var item = list.menu.innerContent.childNodes[i];
                if (item.key.labelName == key.labelName) {
                    item.action();
                    break;
                }
            }

        labelsPanel.components.treeViewBoxLabelType.parentNode.insertBefore(list, null);
        labelsPanel.components.treeViewBoxLabelType.parentNode.removeChild(labelsPanel.components.treeViewBoxLabelType);
        labelsPanel.components.treeViewBoxLabelType = list;

        labelsPanel.components.treeViewBoxLabelType.action = function () {
            var value = this.menu.dropDownList.key;
            labelsPanel.disableUpdate = true;
            labelsPanel.components.textBoxWidth.value = labelsPanel.convertUnit(value.labelWidth, "in");
            labelsPanel.components.textBoxHeight.value = labelsPanel.convertUnit(value.labelHeight, "in");
            labelsPanel.components.textBoxLeft.value = labelsPanel.convertUnit(value.leftMargin, "in");
            labelsPanel.components.textBoxTop.value = labelsPanel.convertUnit(value.topMargin, "in");
            labelsPanel.components.textBoxHorizontal.value = labelsPanel.convertUnit(value.horizontalGap, "in");
            labelsPanel.components.textBoxVertical.value = labelsPanel.convertUnit(value.verticalGap, "in");
            labelsPanel.components.textBoxPageWidth.value = labelsPanel.convertUnit(value.paperWidth, "in");
            labelsPanel.components.textBoxPageHeight.value = labelsPanel.convertUnit(value.paperHeight, "in");
            labelsPanel.components.numericNumberOfColumns.setValue(value.numberOfColumns);
            labelsPanel.components.numericNumberOfRows.setValue(value.numberOfRows);
            labelsPanel.disableUpdate = false;

            var list = labelsPanel.components.comboBoxPapers;
            list.disableUpdate = true;
            list.menu.innerContent.childNodes[0].action();
            for (var i = 0; i < list.menu.innerContent.childNodes.length; i++) {
                var item = list.menu.innerContent.childNodes[i];
                if (item.key.width == value.paperWidth && item.key.height == value.paperHeight) {
                    item.action();
                    break;
                }
            }
            list.disableUpdate = false;

            labelsPanel.enableGroup(true);
            form.enableButtons(true, false, true);

            labelsPanel.drawPreview();
        }
    }

    labelsPanel.enableGroup = function (enabled) {
        labelsPanel.components.textBoxWidth.setEnabled(enabled);
        labelsPanel.components.textBoxHeight.setEnabled(enabled);
        labelsPanel.components.textBoxLeft.setEnabled(enabled);
        labelsPanel.components.textBoxTop.setEnabled(enabled);
        labelsPanel.components.textBoxHorizontal.setEnabled(enabled);
        labelsPanel.components.textBoxVertical.setEnabled(enabled);
        labelsPanel.components.textBoxPageWidth.setEnabled(enabled);
        labelsPanel.components.textBoxPageHeight.setEnabled(enabled);
        labelsPanel.components.numericNumberOfColumns.setEnabled(enabled);
        labelsPanel.components.numericNumberOfRows.setEnabled(enabled);
        labelsPanel.components.comboBoxDirection.setEnabled(enabled);
        labelsPanel.components.comboBoxPapers.setEnabled(enabled);
        labelsPanel.groupEnabled = enabled;
    }

    labelsPanel.createRow = function (label, boxName, unitName, table, element) {
        table.addTextCellInNextRow(label).className = "wizardLablesText";
        var cell = table.addCellInLastRow();
        cell.align = "right";
        cell.style.paddingRight = "100px";
        if (element) {
            cell.appendChild(element);
            labelsPanel.components[boxName] = element;
        } else {
            var inTable = labelsPanel.jsObject.CreateHTMLTable();
            inTable.addCell(labelsPanel.components[boxName] = labelsPanel.jsObject.TextBox(null, 50));
            labelsPanel.components[boxName].action = function () {
                labelsPanel.drawPreview();
            }
            var unitDiv = document.createElement("div");
            unitDiv.innerHTML = "";
            unitDiv.style.paddingLeft = "5px";
            inTable.addCell(unitDiv);
            labelsPanel.components[unitName] = unitDiv;
            cell.appendChild(inTable);
        }
    }

    labelsPanel.convertUnit = function (value, unit) {
        var vl = 0;
        if (unit == "cm") {
            vl = value * 100 / 2.54;
        } else if (unit == "mm") {
            vl = value * 10 / 2.54;
        } else if (unit == "in") {
            vl = value * 100;
        } else if (unit == "hi") {
            vl = value;
        }
        return labelsPanel.round(labelsPanel.convertFromHInches(vl, labelsPanel.unit));
    }

    labelsPanel.drawPreview = function () {
        if (labelsPanel.disableUpdate) return;

        form.enableButtons(true, false, false);

        while (labelsPanel.components.panelPreview.childNodes.length > 0)
            labelsPanel.components.panelPreview.removeChild(labelsPanel.components.panelPreview.childNodes[0]);

        var rect = {
            width: labelsPanel.components.panelPreview.offsetWidth,
            height: labelsPanel.components.panelPreview.offsetHeight
        };

        var unit = labelsPanel.unit;
        try {
            if (labelsPanel.components.textBoxWidth.value.length == 0) return;
            if (labelsPanel.components.textBoxHeight.value.length == 0) return;
            if (labelsPanel.components.textBoxLeft.value.length == 0) return;
            if (labelsPanel.components.textBoxTop.value.length == 0) return;
            if (labelsPanel.components.textBoxHorizontal.value.length == 0) return;
            if (labelsPanel.components.textBoxVertical.value.length == 0) return;
            if (labelsPanel.components.textBoxPageWidth.value.length == 0) return;
            if (labelsPanel.components.textBoxPageHeight.value.length == 0) return;

            labelsPanel.unit = "in";
            // Change value
            var result;
            var labelWidth = labelsPanel.convertUnit(labelsPanel.components.textBoxWidth.value, unit);
            var labelHeight = labelsPanel.convertUnit(labelsPanel.components.textBoxHeight.value, unit);
            var labelLeft = labelsPanel.convertUnit(labelsPanel.components.textBoxLeft.value, unit);
            var labelTop = labelsPanel.convertUnit(labelsPanel.components.textBoxTop.value, unit);
            var labelHorizontal = labelsPanel.convertUnit(labelsPanel.components.textBoxHorizontal.value, unit);
            var labelVertical = labelsPanel.convertUnit(labelsPanel.components.textBoxVertical.value, unit);
            var paperWidth = labelsPanel.convertUnit(labelsPanel.components.textBoxPageWidth.value, unit);
            var paperHeight = labelsPanel.convertUnit(labelsPanel.components.textBoxPageHeight.value, unit);

            if (isNaN(labelWidth) || isNaN(labelHeight) || isNaN(labelLeft) || isNaN(labelTop) || isNaN(labelHorizontal) || isNaN(labelVertical) || isNaN(paperWidth) || isNaN(paperHeight))
                return;

            var maxSize = Math.max(paperWidth, paperHeight);

            var previewWidth = rect.width - 10;
            var previewHeight = rect.height - 10;

            var zoom = maxSize > 0 ? (previewWidth / maxSize) : 1;

            var paperLeft = (previewWidth - paperWidth * zoom) / 2 + 5;
            var paperTop = (previewHeight - paperHeight * zoom) / 2 + 5;

            var paperRect = { x: paperLeft, y: paperTop, width: (paperWidth * zoom), height: (paperHeight * zoom) };

            var svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
            svg.setAttribute("width", labelsPanel.components.panelPreview.offsetWidth);
            svg.setAttribute("height", labelsPanel.components.panelPreview.offsetHeight);

            //paperRect.Offset(1, 1);
            var rect = document.createElementNS("http://www.w3.org/2000/svg", "rect");
            rect.setAttribute("x", paperRect.x);
            rect.setAttribute("y", paperRect.y);
            rect.setAttribute("width", paperRect.width);
            rect.setAttribute("height", paperRect.height);
            rect.setAttribute("style", "fill:rgb(255,255,255);stroke-width:1;stroke:dimgray");
            svg.appendChild(rect);

            var x = labelLeft;
            var y = labelTop;

            //var paperGeometryRect = new RectangleGeometry(paperRect);
            //drawingContext.PushClip(paperGeometryRect);
            var defs = document.createElementNS("http://www.w3.org/2000/svg", "defs");
            var clipPath = document.createElementNS("http://www.w3.org/2000/svg", "clipPath");
            defs.appendChild(clipPath);
            clipPath.setAttribute("id", "preview");
            rect = document.createElementNS("http://www.w3.org/2000/svg", "rect");
            rect.setAttribute("x", paperRect.x);
            rect.setAttribute("y", paperRect.y);
            rect.setAttribute("width", paperRect.width);
            rect.setAttribute("height", paperRect.height);
            clipPath.appendChild(rect);
            svg.appendChild(defs);

            var index = 1;
            var fontSize = labelHeight * zoom / 3;
            if (labelsPanel.components.comboBoxDirection.menu.dropDownList.key == "0") {
                for (var indexY = 0; indexY < labelsPanel.components.numericNumberOfRows.getValue(); indexY++) {
                    x = labelLeft;
                    for (var indexX = 0; indexX < labelsPanel.components.numericNumberOfColumns.getValue(); indexX++) {
                        var labelRect = {
                            x: x * zoom + paperLeft, y: (y * zoom + paperTop),
                            width: (labelWidth * zoom), height: (labelHeight * zoom)
                        };

                        rect = document.createElementNS("http://www.w3.org/2000/svg", "rect");
                        rect.setAttribute("x", labelRect.x);
                        rect.setAttribute("y", labelRect.y);
                        rect.setAttribute("width", labelRect.width);
                        rect.setAttribute("height", labelRect.height);
                        rect.setAttribute("style", "fill:#f5f5f5;stroke-width:1;stroke:black");
                        rect.setAttribute("clip-path", "url(#preview)");
                        svg.appendChild(rect);

                        var text = document.createElementNS("http://www.w3.org/2000/svg", "text");
                        text.setAttribute("x", labelRect.x);
                        text.setAttribute("y", labelRect.y);
                        text.setAttribute("dy", "1em");
                        text.setAttribute("fill", "black");
                        text.setAttribute("style", "font-family: Arial; font-size:" + fontSize + "px");
                        text.setAttribute("clip-path", "url(#preview)");
                        text.textContent = index;
                        svg.appendChild(text);

                        x += labelWidth + labelHorizontal;
                        index++;
                    }
                    y += labelHeight + labelVertical;
                }
            }
            else {
                for (var indexX = 0; indexX < labelsPanel.components.numericNumberOfColumns.getValue(); indexX++) {
                    y = labelTop;
                    for (var indexY = 0; indexY < labelsPanel.components.numericNumberOfRows.getValue(); indexY++) {
                        var labelRect = {
                            x: (x * zoom + paperLeft), y: (y * zoom + paperTop),
                            width: (labelWidth * zoom), height: (labelHeight * zoom)
                        };

                        rect = document.createElementNS("http://www.w3.org/2000/svg", "rect");
                        rect.setAttribute("x", labelRect.x);
                        rect.setAttribute("y", labelRect.y);
                        rect.setAttribute("width", labelRect.width);
                        rect.setAttribute("height", labelRect.height);
                        rect.setAttribute("style", "fill:#f5f5f5;stroke-width:1;stroke:black");
                        rect.setAttribute("clip-path", "url(#preview)");
                        svg.appendChild(rect);

                        var text = document.createElementNS("http://www.w3.org/2000/svg", "text");
                        text.setAttribute("x", labelRect.x);
                        text.setAttribute("y", labelRect.y);
                        text.setAttribute("dy", "1em");
                        text.setAttribute("fill", "black");
                        text.setAttribute("style", "font-family: Arial; font-size:" + fontSize + "px");
                        text.setAttribute("clip-path", "url(#preview)");
                        text.textContent = index;
                        svg.appendChild(text);

                        y += labelHeight + labelVertical;
                        index++;
                    }
                    x += labelWidth + labelHorizontal;
                }
            }

            labelsPanel.components.panelPreview.appendChild(svg);
        }
        catch (ee) {
            labelsPanel.unit = unit;
            console.log(ee);
        }
        form.enableButtons(true, false, true);
        labelsPanel.unit = unit;
    }

    labelsPanel.convertFromHInches = function (value, unit) {
        if (unit == "cm") {
            return value * 2.54 / 100;
        } else if (unit == "mm") {
            return value * 2.54 / 10;
        } else if (unit == "in") {
            return value / 100;
        } else if (unit == "hi") {
            return value;
        }
    }

    labelsPanel.convertFromCM = function (value, unit) {
        if (unit == "cm") {
            return labelsPanel.round(value);
        } else {
            var vl = value * 100 / 2.54;
            if (unit == "mm") {
                return labelsPanel.round(vl * 2.54 / 10);
            } else if (unit == "in") {
                return labelsPanel.round(vl / 100);
            } else if (unit == "hi") {
                return labelsPanel.round(vl);
            }
        }
    }

    labelsPanel.round = function (value) {
        return Math.round(value * 10) / 10;
    }

    labelsPanel.getReportOptions = function (options) {
        options["labelWidth"] = labelsPanel.components.textBoxWidth.value;
        options["labelHeight"] = labelsPanel.components.textBoxHeight.value;
        options["labelLeft"] = labelsPanel.components.textBoxLeft.value;
        options["labelTop"] = labelsPanel.components.textBoxTop.value;
        options["labelHorizontal"] = labelsPanel.components.textBoxHorizontal.value;
        options["labelVertical"] = labelsPanel.components.textBoxVertical.value;
        options["paperWidth"] = labelsPanel.components.textBoxPageWidth.value;
        options["paperHeight"] = labelsPanel.components.textBoxPageHeight.value;
        options["numericNumberOfColumns"] = labelsPanel.components.numericNumberOfColumns.getValue();
        options["numericNumberOfRows"] = labelsPanel.components.numericNumberOfRows.getValue();
        options["direction"] = labelsPanel.components.comboBoxDirection.menu.dropDownList.key;

        options["unit"] = labelsPanel.unit;
    }

    labelsPanel.onShow = function () { };
    labelsPanel.onHide = function () { };

    form.appendStepPanel(labelsPanel, form.jsObject.loc.Wizards.infoLabelSettings);
    return labelsPanel;
}