
StiMobileDesigner.prototype.DataColumnContainer = function (name, headerText, width, showItemImage, allowManuallyData) {
    var container = document.createElement("div");
    container.name = name;
    container.isSelected = false;
    var jsObject = this;
    container.dataColumn = "";
    container.dataColumnObject = null;
    container.allowSelected = false;
    if (width) container.style.width = width + "px";

    var innerTable = this.CreateHTMLTable();
    innerTable.style.width = innerTable.style.height = "100%";
    container.innerTable = innerTable;
    container.appendChild(innerTable);

    if (headerText) {
        var headerCell = innerTable.addTextCell(headerText);
        headerCell.className = "stiDesignerTextContainer";
        headerCell.style.padding = "12px 0 12px 0";
        container.headerCell = headerCell;
    }

    var innerContainer = document.createElement("div");
    container.innerContainer = innerContainer;
    innerContainer.className = "stiDataColumnContainer";
    innerContainer.style.height = "30px";
    innerTable.addCellInNextRow(innerContainer);
    jsObject.AddDroppedContainerToCollection(innerContainer);

    container.hintTextBlock = function () {
        if (allowManuallyData && innerContainer.style.height != "30px") {
            var table = jsObject.CreateHTMLTable();
            table.setAttribute("style", "position: absolute; height: 100%; width: 100%; text-align: center");
            table.addCell().style.height = "35%";

            var text = document.createElement("div");
            text.className = "stiCreateDataHintText";
            text.style.padding = "6px";
            text.innerHTML = jsObject.loc.Dashboard.DragDropDataFromDictionary;
            var cell1 = table.addCellInNextRow(text);
            cell1.style.textAlign = "center";
            cell1.style.height = "20px";

            var separator = jsObject.SeparatorOr();
            separator.style.width = "calc(100% - 70px)";
            separator.style.display = "inline-block";
            var cell2 = table.addCellInNextRow(separator);
            cell2.style.textAlign = "center";
            cell2.style.height = "20px";

            var text2 = document.createElement("div");
            text2.className = "stiCreateDataHintHeaderText";
            text2.style.padding = "6px";
            text2.style.fontSize = "12px";
            text2.style.cursor = "pointer";
            text2.innerHTML = jsObject.loc.Report.EnterDataManually;
            var cell3 = table.addCellInNextRow(text2);
            cell3.style.textAlign = "center";
            cell3.style.height = "20px";

            text2.onclick = function () {
                if (container.actionEnterManuallyData) container.actionEnterManuallyData();
            }

            var text3 = document.createElement("div");
            text3.className = "stiCreateDataHintHeaderText";
            text3.style.padding = "6px 6px 12px 6px";
            text3.style.fontSize = "12px";
            text3.style.cursor = "pointer";
            text3.innerHTML = jsObject.loc.Buttons.ShowMore;

            text3.onclick = function () {
                if (container.actionShowMore) container.actionShowMore();
            }

            var cell4 = table.addCellInNextRow(text3)
            cell4.style.verticalAlign = "bottom";

            return table;
        }
        else {
            var hintText = document.createElement("div");
            hintText.setAttribute("style", "position: absolute; top: calc(50% - 6px); width: 100%; text-align: center");
            hintText.innerHTML = jsObject.loc.Dashboard.DragDropDataFromDictionary;

            return hintText;
        }
    }

    container.clear = function () {
        container.dataColumn = "";
        container.dataColumnObject = null;
        container.item = null;
        container.setSelected(false);
        while (innerContainer.childNodes[0]) innerContainer.removeChild(innerContainer.childNodes[0]);
        innerContainer.appendChild(container.hintTextBlock());
        innerContainer.style.borderStyle = "dashed";
    }

    container.addColumn = function (dataColumn, dataColumnObject) {
        while (innerContainer.childNodes[0]) innerContainer.removeChild(innerContainer.childNodes[0]);

        container.dataColumn = dataColumn;
        container.dataColumnObject = dataColumnObject;
        innerContainer.style.borderStyle = "solid";
        innerContainer.style.borderColor = "";

        var imageName = null;
        var captionText = dataColumn;

        if (dataColumnObject) {
            if (dataColumnObject.typeItem == "Column" && StiMobileDesigner.checkImageSource(jsObject.options, dataColumnObject.typeIcon + ".png"))
                imageName = dataColumnObject.typeIcon + ".png";
            else if (dataColumnObject.typeItem == "Meter" && StiMobileDesigner.checkImageSource(jsObject.options, "Meters." + dataColumnObject.typeIcon + ".png"))
                imageName = "Meters." + dataColumnObject.typeIcon + ".png";

            if (dataColumnObject.currentFunction) captionText = dataColumn + " (" + dataColumnObject.currentFunction + ")";
        }

        var button = container.item = jsObject.StandartSmallButton(null, null, captionText, showItemImage ? imageName : null);
        innerContainer.appendChild(button);

        button.style.height = "30px";
        button.style.border = "0";
        button.innerTable.style.width = "100%";
        button.itemObject = dataColumnObject;
        button.container = container;

        if (button.caption) {
            button.caption.style.width = "100%";
            var captCont = document.createElement("div");
            button.captionContainer = captCont;
            captCont.style.position = "relative";
            captCont.style.overflow = "hidden";
            captCont.style.textOverflow = "ellipsis";
            captCont.style.padding = "2px 0 2px 0";
            if (container.width || container.maxWidth) {
                var textMaxWidth = (container.width || container.maxWidth) - 50;
                if (imageName) textMaxWidth -= 25;
                captCont.style.maxWidth = textMaxWidth + "px";
            }
            captCont.innerHTML = captionText;
            button.caption.innerHTML = "";
            button.caption.appendChild(captCont);
        }

        button.onmousedown = function (event) {
            if (this.isTouchStartFlag || this.editableTextBox) return;
            if (event) event.preventDefault();

            if (dataColumnObject && dataColumnObject.typeItem == "Meter" && event.button != 2 && !jsObject.options.controlsIsFocused) {
                var itemInDragObject = jsObject.TreeItemForDragDrop({ name: dataColumn }, null, true);
                if (itemInDragObject.button.captionCell) itemInDragObject.button.captionCell.style.padding = "3px 15px 3px 5px";
                itemInDragObject.originalItem = this;
                itemInDragObject.beginingOffset = 0;
                jsObject.options.itemInDrag = itemInDragObject;
            }
        }

        var closeButton = jsObject.StandartSmallButton(null, null, null, "CloseForm.png");
        closeButton.image.style.margin = "0 2px 0 2px";
        closeButton.imageCell.style.padding = jsObject.options.isTouchDevice ? "0 4px 0 4px" : "0 3px 0 3px";
        closeButton.style.background = "white";
        closeButton.style.height = closeButton.style.width = "26px";
        closeButton.style.marginRight = "1px";
        closeButton.style.display = "none";
        closeButton.innerTable.style.width = "100%";
        button.closeButton = closeButton;
        button.innerTable.addCell(closeButton);

        closeButton.onmouseenter = function () {
            if (!this.isEnabled || jsObject.options.isTouchClick) return;
            this.className = this.overClass;
            this.isOver = true;
            closeButton.style.background = "lightgray";
        }

        closeButton.onmouseleave = function () {
            this.isOver = false;
            if (!this.isEnabled) return;
            this.className = this.isSelected ? this.selectedClass : this.defaultClass;
            this.style.background = "white";
        }

        closeButton.action = function () {
            closeButton.clicked = true;
            container.clear();
            container.action();
        }

        button.setSelected_ = button.setSelected;
        button.setSelected = function (state) {
            this.setSelected_(state);
            if (jsObject.options.isTouchDevice) {
                closeButton.style.display = state ? "" : "none";
            }
        }

        button.onmouseenter = function () {
            if (!this.isEnabled || jsObject.options.isTouchClick) return;
            this.className = this.overClass;
            this.isOver = true;
            closeButton.style.display = "";
        }

        button.onmouseleave = function () {
            this.isOver = false;
            if (!this.isEnabled) return;
            this.className = this.isSelected ? this.selectedClass : this.defaultClass;
            closeButton.style.display = "none";
        }

        button.setEditable = function (state) {
            if (state) {
                if (this.caption && this.itemObject && this.itemObject.label != null) {
                    var textBox = jsObject.TextBox(null);
                    this.captionContainer.innerHTML = "";
                    this.captionContainer.appendChild(textBox);
                    this.captionContainer.style.overflow = "visible";
                    this.editableTextBox = textBox;

                    textBox.value = this.itemObject.label;
                    textBox.style.position = "absolute";
                    textBox.style.width = (this.caption.offsetWidth - 45) + "px";
                    textBox.style.left = "0px";
                    textBox.style.top = "-8px";
                    textBox.style.border = "0px";
                    textBox.style.height = "20px";
                    textBox.focus();
                    container.editableItem = this;

                    textBox.onblur = function () {
                        textBox.action();
                    }

                    var this_ = this;
                    textBox.action = function () {
                        this_.setEditable(false);
                        jsObject.options.controlsIsFocused = null;
                    }
                }
            }
            else {
                if (this.editableTextBox) {
                    if (this.itemObject.label != this.editableTextBox.value && this.editableTextBox.value) {
                        this.itemObject.label = this.editableTextBox.value;
                        if (container.action) container.action("rename");
                    }
                    this.captionContainer.removeChild(this.editableTextBox);
                    this.captionContainer.style.overflow = "hidden";
                    this.captionContainer.innerHTML = this.itemObject.label;
                }
                this.editableTextBox = null;
                container.editableItem = null;
            }
        }

        if (container.allowSelected) {
            button.action = function () {
                if (!closeButton.clicked) {
                    container.setSelected(true);
                    container.onSelected();
                }
                closeButton.clicked = false;
            }
        }
    }

    innerContainer.canInsert = function () {
        var typeItem = jsObject.options.itemInDrag ? jsObject.options.itemInDrag.originalItem.itemObject.typeItem : null;

        return (typeItem && typeItem == "Column" || typeItem == "DataSource" || typeItem == "BusinessObject" || typeItem == "Meter" || typeItem == "Variable");
    }

    innerContainer.onmouseover = function () {
        if (innerContainer.canInsert()) {
            innerContainer.style.borderStyle = "dashed";
            innerContainer.style.borderColor = jsObject.options.themeColors[jsObject.GetThemeColor()];
        }
    }

    innerContainer.onmouseout = function () {
        innerContainer.style.borderStyle = container.dataColumn ? "solid" : "dashed";
        innerContainer.style.borderColor = "";
        innerContainer.style.borderWidth = "1px";
    }

    innerContainer.onmouseup = function () {
        if (innerContainer.canInsert()) {
            var itemObject = jsObject.CopyObject(jsObject.options.itemInDrag.originalItem.itemObject);
            var columnText = jsObject.options.itemInDrag.originalItem.getResultForEditForm(true);

            if (itemObject.typeItem == "Column" || itemObject.typeItem == "Variable") {
                var columnParent = jsObject.options.dictionaryTree.getCurrentColumnParent();
                if (columnParent) {
                    itemObject.currentParentType = columnParent.type;
                    itemObject.currentParentName = (columnParent.type == "BusinessObject") ? jsObject.options.itemInDrag.originalItem.getBusinessObjectFullName() : columnParent.name;
                }
            }
            else {
                if (itemObject.columns && itemObject.columns.length > 0) {
                    var columnObject = jsObject.CopyObject(itemObject.columns[0]);
                    columnObject.currentParentType = itemObject.typeItem;
                    columnObject.currentParentName = itemObject.name;
                    itemObject = columnObject;

                    jsObject.options.itemInDrag.originalItem.completeBuildTree();

                    for (var key in jsObject.options.itemInDrag.originalItem.childs) {
                        var columnItem = jsObject.options.itemInDrag.originalItem.childs[key];
                        if (columnItem && columnItem.itemObject && columnItem.itemObject.typeItem == "Column" && columnItem.itemObject.name == columnObject.name) {
                            var columnText = columnItem.getResultForEditForm(true);
                        }
                    }
                }
            }
            container.addColumn(columnText, itemObject);
            container.action();
        }
    }

    container.setSelected = function (state) {
        this.isSelected = state;
        if (this.item) this.item.setSelected(state);
    }

    container.action = function () { }

    container.onSelected = function () { }

    container.clear();

    return container;
}