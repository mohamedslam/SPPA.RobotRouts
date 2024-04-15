
StiMobileDesigner.prototype.ComponentButton = function (name, caption, imageName, style, toolTip, haveSubMenu) {
    var jsObject = this;
    var button = this.BigButton(name, "Components", caption, imageName, toolTip ? toolTip : caption, haveSubMenu, style, true, 90);

    button.action = function () {
        if (jsObject.options.currentMenu) {
            if (this.name == "StiTable") {
                var tableSubMenu = jsObject.TableSubMenu(this);
                tableSubMenu.changeVisibleState(true);
                return;
            }
            var parentButton = jsObject.options.currentMenu.parentButton;
            jsObject.options.currentMenu.changeVisibleState(false);
            parentButton.setSelected(true);
        }
        else if (jsObject.options.insertPanel) {
            jsObject.options.insertPanel.resetChoose();
        }
        this.setSelected(!this.isSelected);
        jsObject.options.drawComponent = this.isSelected;
        jsObject.options.paintPanel.setCopyStyleMode(false);
        jsObject.options.paintPanel.changeCursorType(this.isSelected);
        if (jsObject.options.insertPanel) jsObject.options.insertPanel.selectedComponent = this.isSelected ? this : null;
    }

    button.onmousedown = function (event) {
        if (this.isTouchStartFlag || !this.isEnabled) return;
        jsObject.options.buttonPressed = this;
        this.ontouchstart(event, true);
    }

    button.ontouchstart = function (event, mouseProcess) {
        var this_ = this;
        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);
        jsObject.options.fingerIsMoved = false;
        jsObject.options.buttonPressed = this;
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);

        if (jsObject.options.controlsIsFocused) {
            jsObject.options.controlsIsFocused.blur(); //fixed bug when drag&drop component from toolbar
        }
        if (event && !this.isTouchStartFlag) event.preventDefault();
        if (event.button != 2) {
            var componentButtonInDrag = this.isDashboardElement
                ? jsObject.DashboardElementForDragDrop(null, this.name)
                : jsObject.ComponentForDragDrop(null, this.name);

            if (componentButtonInDrag) {
                componentButtonInDrag.ownerButton = this;
                componentButtonInDrag.beginingOffset = 0;
                jsObject.options.componentButtonInDrag = componentButtonInDrag;
            }
        }
    }

    button.ondblclick = function (evnt) {
        var params = { createdByDblClick: true }
        var currComp = jsObject.options.selectedObject;
        if (currComp && currComp.typeComponent != "StiPage" && currComp.typeComponent != "StiReport") {
            params.currentComponent = currComp.properties.name;
        }
        jsObject.SendCommandCreateComponent(jsObject.options.currentPage.properties.name, this.name, "0!0!0!0", params);
        if (jsObject.options.insertPanel) jsObject.options.insertPanel.resetChoose();
    }

    jsObject.addEvent(button, "touchend", function (event) {
        if (jsObject.options.componentButtonInDrag && jsObject.options.componentButtonInDrag.beginingOffset >= 10) {
            jsObject.DropDragableItemToActiveContainer(jsObject.options.componentButtonInDrag);
        }
    });

    return button;
}

StiMobileDesigner.prototype.TableSubMenu = function (parentButton) {
    var menu = this.BaseMenu("tableSubMenu", parentButton, "Down");
    menu.type = "HorMenu";

    var header = document.createElement("div");
    header.innerHTML = this.loc.Components.StiTable;
    header.className = "stiDesignerMenuHeader";
    menu.innerContent.appendChild(header);
    menu.header = header;

    var sampleTable = this.CreateHTMLTable();
    menu.innerContent.appendChild(sampleTable);
    sampleTable.tableCells = {};

    sampleTable.update = function (columnCount, rowCount) {
        for (var i = 1; i <= 10; i++) {
            for (var k = 1; k <= 10; k++) {
                sampleTable.tableCells["cell" + i.toString() + k.toString()].innerCell.style.border = k <= columnCount && i <= rowCount ? "1px solid #ef4810" : "1px solid #646464";
            }
        }
        header.innerHTML = columnCount > 0 && rowCount > 0 ? this.jsObject.loc.Components.StiTable + " " + columnCount + " X " + rowCount : this.jsObject.loc.Components.StiTable;
    }

    var tableCell = function (columnNum, rowNum) {
        var cell = document.createElement("div");
        cell.style.background = "#ffffff";
        cell.columnNum = columnNum;
        cell.rowNum = rowNum;

        var innerCell = document.createElement("div");
        cell.innerCell = innerCell;
        cell.appendChild(innerCell);
        innerCell.style.border = "1px solid #646464";
        var size = menu.jsObject.options.isTouchDevice ? 25 : 16;
        innerCell.style.width = size + "px";
        innerCell.style.height = size + "px";
        innerCell.style.margin = "1px";

        cell.onmouseover = function () {
            sampleTable.update(this.columnNum, this.rowNum);
        }

        cell.onmouseout = function () {
            sampleTable.update(0, 0);
        }

        cell.onclick = function () {
            if (this.isTouchEndFlag) return;
            this.action();
        }

        cell.ontouchend = function () {
            this.isTouchEndFlag = true;
            clearTimeout(this.isTouchEndTimer);
            this.action();
            var this_ = this;
            this.isTouchEndTimer = setTimeout(function () {
                this_.isTouchEndFlag = false;
            }, 1000);
        }

        cell.action = function () {
            menu.changeVisibleState(false);
            if (menu.jsObject.options.currentMenu) {
                var mainMenuParentButton = menu.jsObject.options.currentMenu.parentButton;
                menu.jsObject.options.currentMenu.changeVisibleState(false);
                mainMenuParentButton.setSelected(true);
            }
            menu.jsObject.options.drawComponent = true;
            menu.jsObject.options.paintPanel.setCopyStyleMode(false);
            menu.jsObject.options.paintPanel.changeCursorType(true);

            parentButton.rowCount = this.rowNum;
            parentButton.columnCount = this.columnNum;
            if (menu.jsObject.options.insertPanel) menu.jsObject.options.insertPanel.selectedComponent = parentButton;
        }

        return cell;
    }

    for (var i = 1; i <= 10; i++) {
        for (var k = 1; k <= 10; k++) {
            var cell = tableCell(k, i);
            sampleTable.tableCells["cell" + i.toString() + k.toString()] = cell;
            sampleTable.addCellInLastRow(cell);
        }
        sampleTable.addRow();
    }

    menu.ontouchstart = function (mouseProcess) {
        var this_ = this;
        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);
        this.jsObject.options.horMenuPressed = this;
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    return menu;
}