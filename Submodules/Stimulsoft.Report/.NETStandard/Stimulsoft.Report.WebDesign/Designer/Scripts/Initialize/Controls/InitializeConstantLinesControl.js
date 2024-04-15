
StiMobileDesigner.prototype.ConstantLinesControl = function (width, height) {
    var control = document.createElement("div");
    control.toolBar = this.CreateHTMLTable();
    control.toolBar.style.margin = "4px";
    control.appendChild(control.toolBar);

    //Add
    var addButton = control.addButton = this.StandartSmallButton(null, null, this.loc.Chart.AddConstantLine, "AddConstantLine.png");
    control.toolBar.addCell(addButton);

    addButton.action = function () {
        control.container.addConstantLine({ text: "", axisValue: StiBase64.encode("1"), lineColor: "0,0,0", lineStyle: "0", lineWidth: "1" });
    }

    //Remove
    var removeButton = control.removeButton = this.StandartSmallButton(null, null, this.loc.Buttons.Remove, "Remove.png");
    control.toolBar.addCell(removeButton);

    removeButton.action = function () {
        control.container.removeConstantLine();
    }

    control.toolBar.addCell(this.HomePanelSeparator()).style.padding = "0 2px 0 2px";
    control.moveUpButton = this.StandartSmallButton(null, null, null, "Arrows.ArrowUpBlue.png", null, null);
    control.moveDownButton = this.StandartSmallButton(null, null, null, "Arrows.ArrowDownBlue.png", null, null);
    control.moveUpButton.setEnabled(false);
    control.moveDownButton.setEnabled(false);
    control.toolBar.addCell(control.moveUpButton);
    control.toolBar.addCell(control.moveDownButton);

    control.moveUpButton.action = function () {
        if (control.container.selectedItem) { control.container.selectedItem.move("Up"); }
    }

    control.moveDownButton.action = function () {
        if (control.container.selectedItem) { control.container.selectedItem.move("Down"); }
    }

    //Container
    control.container = this.ConstantLinesContainer(control);
    if (width) control.container.style.width = width + "px";
    if (height) control.container.style.height = height + "px";
    control.appendChild(control.container);

    control.container.onAction = function () {
        var count = this.getCountItems();
        var index = this.selectedItem ? this.selectedItem.getIndex() : -1;
        control.moveUpButton.setEnabled(index > 0);
        control.moveDownButton.setEnabled(index != -1 && index < count - 1);
    }

    control.fill = function (constLines) {
        this.container.clear();
        if (!constLines) return;
        for (var i = 0; i < constLines.length; i++) this.container.addConstantLine(constLines[i], true);
        this.container.onAction();
    }

    control.getValue = function () {
        var result = [];
        for (var i = 0; i < this.container.childNodes.length; i++) {
            var item = this.container.childNodes[i];
            result.push({
                text: StiBase64.encode(item.controls.text.value),
                axisValue: StiBase64.encode(item.controls.axisValue.textBox.value),
                lineColor: item.controls.lineColor.key,
                lineStyle: item.controls.lineStyle.key,
                lineWidth: item.controls.lineWidth.value
            });
        }
        return result;
    }

    return control;
}

StiMobileDesigner.prototype.ConstantLinesContainer = function (mainControl) {
    var jsObject = this;
    var container = document.createElement("div");
    mainControl.container = container;

    container.className = "stiDesignerSortContainer";
    container.mainControl = mainControl;
    container.selectedItem = null;

    container.addConstantLine = function (constLineObject, notAction) {
        var item = jsObject.ConstantLineItem(this);
        this.appendChild(item);

        item.controls.text.value = StiBase64.decode(constLineObject.text);
        item.controls.axisValue.textBox.value = StiBase64.decode(constLineObject.axisValue);
        item.controls.lineColor.setKey(constLineObject.lineColor);
        item.controls.lineStyle.setKey(constLineObject.lineStyle);
        item.controls.lineWidth.value = constLineObject.lineWidth;

        item.setSelected();
        this.mainControl.removeButton.setEnabled(true);
        if (!notAction) this.onAction();
    }

    container.removeConstantLine = function () {
        for (var i = 0; i < this.childNodes.length; i++) {
            if (this.childNodes[i].isSelected) {
                this.removeChild(this.childNodes[i]);
                this.selectedItem = null;
            }
        }
        if (this.childNodes.length > 0) this.childNodes[0].setSelected();
        this.mainControl.removeButton.setEnabled(this.childNodes.length > 0);
        this.onAction();
    }

    container.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        this.mainControl.removeButton.setEnabled(false);
        this.mainControl.moveUpButton.setEnabled(false);
        this.mainControl.moveDownButton.setEnabled(false);
    }

    container.getCountItems = function () {
        return container.childNodes.length;
    }

    container.onAction = function () { };

    return container;
}


StiMobileDesigner.prototype.ConstantLineItem = function (container) {
    var jsObject = this;
    var item = document.createElement("div");
    item.controls = {};
    item.container = container;
    item.isSelected = false;
    item.className = "stiDesignerSortPanel";
    item.style.padding = "6px 0 6px 0";
    var innerTable = this.CreateHTMLTable();
    innerTable.style.width = "100%";
    item.appendChild(innerTable);

    var props = [
        ["text", this.loc.PropertyMain.Title, this.TextBox(null, 150)],
        ["axisValue", this.loc.PropertyMain.Value, this.ExpressionControl(null, 150)],
        ["lineColor", this.loc.PropertyMain.Color, this.ColorControl(null, null, true, 150, true)],
        ["lineStyle", this.loc.PropertyMain.Style, this.DropDownList(null, 150, null, this.GetBorderStyleItems(), true, true)],
        ["lineWidth", this.loc.PropertyMain.Width, this.TextBoxPositiveIntValue(null, 150)]
    ];

    for (var i = 0; i < props.length; i++) {
        var textCell = innerTable.addTextCellInLastRow(props[i][1]);
        textCell.className = "stiDesignerCaptionControlsBigIntervals";
        textCell.style.width = "100%";
        var control = props[i][2];
        control.style.margin = "6px 12px 6px 12px";
        item.controls[props[i][0]] = control;
        innerTable.addCellInLastRow(control);
        if (i < props.length - 1) innerTable.addRow();
    }

    item.setSelected = function () {
        for (var i = 0; i < this.container.childNodes.length; i++) {
            this.container.childNodes[i].className = "stiDesignerSortPanel";
            this.container.childNodes[i].isSelected = false;
        }
        this.container.selectedItem = this;
        this.isSelected = true;
        this.className = "stiDesignerSortPanelSelected";
    }

    item.onclick = function () {
        if (this.isTouchEndFlag || jsObject.options.isTouchClick) return;
        this.action();
    }

    item.ontouchend = function () {
        this.isTouchEndFlag = true;
        clearTimeout(this.isTouchEndTimer);
        if (jsObject.options.fingerIsMoved) return;
        this.action();
        this.isTouchEndTimer = setTimeout(function () {
            item.isTouchEndFlag = false;
        }, 1000);
    }

    item.action = function () {
        this.setSelected();
        container.onAction();
    }

    item.getIndex = function () {
        for (var i = 0; i < container.childNodes.length; i++)
            if (container.childNodes[i] == this) return i;
    };

    item.move = function (direction) {
        var index = this.getIndex();
        container.removeChild(this);
        var count = container.getCountItems();
        var newIndex = direction == "Up" ? index - 1 : index + 1;
        if (direction == "Up" && newIndex == -1) newIndex = 0;
        if (direction == "Down" && newIndex >= count) {
            container.appendChild(this);
            container.onAction();
            return;
        }
        container.insertBefore(this, container.childNodes[newIndex]);
        container.onAction();
    }

    return item;
}