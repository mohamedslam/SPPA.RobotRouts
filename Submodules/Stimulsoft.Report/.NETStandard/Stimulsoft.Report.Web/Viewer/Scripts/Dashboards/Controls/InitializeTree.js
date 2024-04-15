
StiJsViewer.prototype.TreeItem = function (caption, imageName, itemObject, tree, showCheckBox, id, styleColors) {
    var treeItem = this.CreateHTMLTable();
    treeItem.id = id || this.newGuid().replace(/-/g, '');
    var jsObject = treeItem.jsObject = this;
    treeItem.isSelected = false;
    treeItem.isOpening = false;
    treeItem.isChecked = false;
    treeItem.itemObject = itemObject;
    treeItem.tree = tree;
    tree.items[treeItem.id] = treeItem;
    treeItem.childs = {};
    treeItem.parent = null;

    //Opening icon
    var iconOpening = document.createElement("img");
    treeItem.iconOpening = iconOpening;
    treeItem.addCell(iconOpening).style.width = "1px";
    iconOpening.treeItem = treeItem;
    StiJsViewer.setImageSource(iconOpening, this.options, this.collections, "Dashboards.IconOpenItem" + (styleColors && styleColors.isDarkStyle ? "White.png" : ".png"));
    iconOpening.className = "stiJsViewerTreeItemIconOpening";
    iconOpening.style.visibility = "hidden";
    iconOpening.style.opacity = "0.6";
    iconOpening.style.width = iconOpening.style.height = "16px";

    iconOpening.onmouseover = function () {
        if (jsObject.options.isTouchClick) return;
        this.style.opacity = "1";
    }

    iconOpening.onmouseout = function () {
        this.style.opacity = "0.6";
    }

    iconOpening.onmousedown = function () {
        if (this.isTouchStartFlag) return;
        this.action();
    }

    iconOpening.ontouchstart = function () {
        var this_ = this;
        this.isTouchStartFlag = true;
        clearTimeout(this.isTouchStartTimer);
        this.action();
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    iconOpening.action = function () {
        if (tree.isDisable) return;
        treeItem.isOpening = !treeItem.isOpening;
        treeItem.childsRow.style.display = treeItem.isOpening ? "" : "none";
        var imgName = (treeItem.isOpening ? "Dashboards.IconCloseItem" : "Dashboards.IconOpenItem") + (styleColors && styleColors.isDarkStyle ? "White.png" : ".png");
        StiJsViewer.setImageSource(iconOpening, jsObject.options, jsObject.collections, imgName);
    }

    //Button
    var button = this.CreateHTMLTable();
    button.className = "stiJsViewerTreeItem";
    treeItem.button = button;
    treeItem.addCell(button);

    if (styleColors) {
        button.style.borderRadius = "0";
        button.style.background = styleColors.backColor;
        button.style.color = styleColors.foreColor;
    }

    button.onmouseover = function () {
        if (jsObject.options.isTouchClick) return;
        treeItem.isOver = true;

        if (styleColors) {
            this.style.background = treeItem.isSelected ? styleColors.hotSelectedBackColor : styleColors.hotBackColor;
            this.style.color = treeItem.isSelected ? styleColors.hotSelectedForeColor : styleColors.hotForeColor;
        }
        else {
            this.className = "stiJsViewerTreeItemOver";
        }
    }

    button.onmouseout = function () {
        treeItem.isOver = false;
        if (styleColors) {
            this.style.background = treeItem.isSelected ? styleColors.selectedBackColor : styleColors.backColor;
            this.style.color = treeItem.isSelected ? styleColors.selectedForeColor : styleColors.foreColor;
        }
        else {
            this.className = treeItem.isSelected ? "stiJsViewerTreeItemSelected" : "stiJsViewerTreeItem";
        }
    }

    treeItem.getAllChilds = function (childsArray) {
        if (!childsArray) childsArray = [];
        for (var itemKey in this.childs) {
            childsArray.push(this.childs[itemKey]);
            this.childs[itemKey].getAllChilds(childsArray);
        }

        return childsArray;
    }

    //Checkbox
    if (showCheckBox) {
        var checkBox = this.CheckBox(null, null, null, styleColors);
        treeItem.checkBox = checkBox;
        button.addCell(checkBox).style.width = "1px";
        checkBox.style.marginLeft = "7px";

        treeItem.setChecked = function (state) {
            this.isChecked = state;
            checkBox.setChecked(state);
            treeItem.setIndeterminate(false);
        }

        treeItem.setIndeterminate = function (state) {
            checkBox.setIndeterminate(state);
        }

        checkBox.onmousedown = function () {
            treeItem.checkBoxClicked = true;
        }

        checkBox.action = function () {
            if (tree.isDisable) return;
            treeItem.setChecked(this.isChecked);

            var childs = treeItem.getAllChilds();
            for (var i = 0; i < childs.length; i++) {
                childs[i].setChecked(this.isChecked);
            }
            if (treeItem.parent && treeItem.parent.setChecked && this.isChecked) {
                treeItem.parent.setChecked(true);
            }
            if (tree["onChecked"])
                tree.onChecked(treeItem);
        }
    }

    if (imageName != null) {
        button.image = document.createElement("img");
        button.imageCell = button.addCell(button.image);
        button.imageCell.style.fontSize = "0px"
        if (StiJsViewer.checkImageSource(jsObject.options, jsObject.collections, imageName)) StiJsViewer.setImageSource(button.image, jsObject.options, jsObject.collections, jsObject.collections, imageName);
        button.image.className = "stiJsViewerTreeItemImage";
    }

    if (caption != null || typeof (caption) == "undefined") {
        var captionCell = button.addCell();
        captionCell.className = "stiJsViewerTreeItemCaption";
        captionCell.style.padding = "2px 7px 2px 7px"
        captionCell.innerHTML = caption;
        button.captionCell = captionCell;
    }

    button.onmousedown = function () {
        if (this.isTouchStartFlag) return;
        if (!treeItem.checkBoxClicked) this.action();
        treeItem.checkBoxClicked = false;
    }

    button.ontouchstart = function () {
        var this_ = this;
        this.isTouchStartFlag = true;
        clearTimeout(this.isTouchStartTimer);
        this.action();
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    button.action = function () {
        if (tree.isDisable || showCheckBox) return;
        if (tree.selectedItem != treeItem) {
            treeItem.setSelected();
            tree.onActionItem(treeItem);
        }
        else
            treeItem.setSelected();
    }

    //ChildsRow
    treeItem.childsRow = treeItem.addRow();
    treeItem.childsRow.style.display = "none";

    //Empty Cell
    treeItem.addCellInLastRow();

    //Childs Container
    treeItem.childsContainer = treeItem.addCellInLastRow();
    treeItem.childsContainer.style.textAlign = "left";

    //Methods
    treeItem.addChild = function (childItem) {
        this.childsContainer.appendChild(childItem);
        childItem.parent = this;
        this.childs[childItem.id] = childItem;
        iconOpening.style.visibility = "visible";
        tree.onAddItem(childItem);

        return childItem;
    }

    treeItem.remove = function () {
        if (this.parent && this.parent.childsContainer.contains(this)) {
            var nextItem = this.nextSibling;
            if (!nextItem) nextItem = this.previousSibling;
            if (!nextItem) nextItem = this.parent;
            this.parent.childsContainer.removeChild(this);
            delete this.parent.childs[this.id];
            delete tree.items[this.id];
            for (var key in this.childs) {
                this.childs[key].remove();
            }
            this.parent.isOpening = jsObject.getCountObjects(this.parent.childs) > 0;
            this.parent.iconOpening.style.visibility = this.parent.isOpening ? "visible" : "hidden";
            if (nextItem) nextItem.setSelected();
            tree.onRemoveItem(this);
        }
    }

    treeItem.move = function (direction) {
        if (this.parent) {
            var index = this.getIndex();
            this.parent.childsContainer.removeChild(this);
            var count = this.parent.childsContainer.childNodes.length;
            var newIndex = direction == "Up" ? index - 1 : index + 1;
            if (direction == "Up" && newIndex == -1) newIndex = 0;
            if (direction == "Down" && newIndex >= count) {
                this.parent.childsContainer.appendChild(this);
                return;
            }
            this.parent.childsContainer.insertBefore(this, this.parent.childsContainer.childNodes[newIndex]);
        }
    }

    treeItem.getIndex = function () {
        if (this.parent) {
            var index = 0;
            for (var i = 0; i < this.parent.childsContainer.childNodes.length; i++) {
                if (this == this.parent.childsContainer.childNodes[i]) {
                    return i;
                }
            }
        }
        return -1;
    }

    treeItem.getCountElementsInCurrent = function () {
        return this.parent ? this.parent.childsContainer.childNodes.length : 0;
    }

    treeItem.removeAllChilds = function () {
        for (var itemKey in this.childs) {
            this.childs[itemKey].remove();
        }
    }

    treeItem.setSelected = function () {
        if (tree.selectedItem) {
            if (styleColors) {
                tree.selectedItem.button.style.background = styleColors.backColor;
                tree.selectedItem.button.style.color = styleColors.foreColor;
            }
            else {
                tree.selectedItem.button.className = "stiJsViewerTreeItem";
            }
            tree.selectedItem.isSelected = false;
        }

        if (styleColors) {
            button.style.background = this.isOver ? styleColors.hotSelectedBackColor : styleColors.selectBackgroundColor;
            button.style.color = this.isOver ? styleColors.hotSelectedForeColor : styleColors.selectedForeColor;
        }
        else {
            button.className = this.isOver ? "stiJsViewerTreeItemOver" : "stiJsViewerTreeItemSelected";
        }

        this.isSelected = true;
        tree.selectedItem = this;
        tree.onSelectedItem(this);
    }

    treeItem.openTree = function () {
        var item = this.parent;
        while (item != null) {
            item.isOpening = true;
            item.childsRow.style.display = "";
            StiJsViewer.setImageSource(item.iconOpening, jsObject.options, jsObject.collections, "Dashboards.IconCloseItem" + (styleColors && styleColors.isDarkStyle ? "White.png" : ".png"));
            item = item.parent;
        }
    }

    treeItem.setOpening = function (state) {
        this.isOpening = state;
        this.childsRow.style.display = state ? "" : "none";
        var imageType = state ? "Close" : "Open";
        StiJsViewer.setImageSource(this.iconOpening, jsObject.options, jsObject.collection, "Dashboards.Icon" + imageType + "Item" + (styleColors && styleColors.isDarkStyle ? "White.png" : ".png"));
    }

    return treeItem;
}

StiJsViewer.prototype.Tree = function (width, height) {
    var tree = document.createElement("div");
    tree.jsObject = this;
    tree.items = {};
    tree.selectedItem = null;
    if (width) tree.style.width = width + "px";
    if (height) tree.style.height = height + "px";

    //Events
    tree.onSelectedItem = function (item) { };
    tree.onRemoveItem = function (item) { };
    tree.onAddItem = function (item) { };
    tree.onChecked = function (item) { };
    tree.onActionItem = function (item) { };
    tree.action = function () { };

    tree.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        this.items = {};
        this.selectedItem = null;
    }

    tree.autoscroll = function () {
        if (this.selectedItem && this.offsetHeight > 0) {
            var scrollContainer = this.style.overflow == "auto" ? this : this.parentNode;

            if (scrollContainer) {
                scrollContainer.scrollTop = 0;
                var yPos = this.jsObject.FindPosY(this.selectedItem, null, null, scrollContainer);
                if (yPos + 100 > scrollContainer.offsetHeight) scrollContainer.scrollTop = yPos - scrollContainer.offsetHeight + 100;
            }
        }
    }

    return tree;
}  