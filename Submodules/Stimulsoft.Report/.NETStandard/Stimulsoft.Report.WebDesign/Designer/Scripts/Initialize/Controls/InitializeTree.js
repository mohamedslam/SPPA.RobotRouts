
StiMobileDesigner.prototype.TreeItem = function (caption, imageName, itemObject, tree, showCheckBox, id, imageSizes) {
    var treeItem = this.CreateHTMLTable();
    treeItem.id = id || this.generateKey();
    var jsObject = treeItem.jsObject = this;
    treeItem.isSelected = false;
    treeItem.isOpening = false;
    treeItem.isChecked = false;
    treeItem.itemObject = itemObject;
    treeItem.tree = tree;
    treeItem.childs = {};
    treeItem.parent = null;
    tree.items[treeItem.id] = treeItem;

    treeItem.addChild = function (childItem) {
        this.childsContainer.appendChild(childItem);
        childItem.parent = this;
        this.childs[childItem.id] = childItem;
        this.iconOpening.style.visibility = "visible";
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
            this.parent.isOpening = jsObject.GetCountObjects(this.parent.childs) > 0;
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
            for (var i = 0; i < this.parent.childsContainer.childNodes.length; i++) {
                if (this == this.parent.childsContainer.childNodes[i]) {
                    return i;
                }
            }
        }
        return -1;
    }

    treeItem.getCountChilds = function () {
        return this.childsContainer.childNodes.length;
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
        var baseClass = "stiDesignerTreeItemButton stiDesignerTreeItemButton";
        var selectedItem = tree.selectedItem;
        if (selectedItem) {
            selectedItem.button.className = baseClass + "Default";
            selectedItem.isSelected = false;
        }
        this.button.className = baseClass + "Selected";
        tree.selectedItem = this;
        this.isSelected = true;
        tree.onSelectedItem(this);
    }

    treeItem.openTree = function () {
        var item = this.parent;
        while (item != null) {
            item.isOpening = true;
            item.childsRow.style.display = "";
            StiMobileDesigner.setImageSource(item.iconOpening, jsObject.options, jsObject.options.isTouchDevice ? "IconCloseItemBig.png" : "IconCloseItem.png");
            item = item.parent;
        }
    }

    treeItem.setOpening = function (state) {
        if (this.buildChildsNotCompleted && state) this.completeBuildTree(true);
        this.isOpening = state;
        this.childsRow.style.display = state ? "" : "none";
        var imageType = state ? "Close" : "Open";
        StiMobileDesigner.setImageSource(this.iconOpening, jsObject.options, jsObject.options.isTouchDevice ? "Icon" + imageType + "ItemBig.png" : "Icon" + imageType + "Item.png");
    }

    //Opening icon
    var iconOpening = treeItem.iconOpening = document.createElement("img");
    iconOpening.treeItem = treeItem;
    iconOpening.style.width = iconOpening.style.height = this.options.isTouchDevice ? "24px" : "12px";
    iconOpening.jsObject = this;
    iconOpening.className = "stiDesignerTreeItemIconOpening" + (this.options.isTouchDevice ? "_Touch" : "_Mouse");
    StiMobileDesigner.setImageSource(iconOpening, this.options, this.options.isTouchDevice ? "IconOpenItemBig.png" : "IconOpenItem.png");
    iconOpening.style.visibility = "hidden";
    treeItem.addCell(iconOpening).style.width = "1px";

    iconOpening.onmousedown = function () {
        if (this.isTouchStartFlag) return;
        this.action();
    }

    iconOpening.ontouchstart = function () {
        this.isTouchStartFlag = true;
        clearTimeout(this.isTouchStartTimer);
        this.action();
        this.isTouchStartTimer = setTimeout(function () {
            iconOpening.isTouchStartFlag = false;
        }, 1000);
    }

    iconOpening.action = function () {
        if (tree.isDisable) return;
        treeItem.isOpening = !treeItem.isOpening;
        treeItem.childsRow.style.display = treeItem.isOpening ? "" : "none";
        var imgName = treeItem.isOpening ? "IconCloseItem.png" : "IconOpenItem.png";
        if (jsObject.options.isTouchDevice) imgName = imgName.replace(".png", "Big.png");
        StiMobileDesigner.setImageSource(iconOpening, jsObject.options, imgName);
        treeItem.setSelected();
    }

    //Button
    var button = treeItem.button = this.CreateHTMLTable();
    treeItem.addCell(button);
    button.treeItem = treeItem;
    button.jsObject = this;
    button.isSelected = false;
    button.className = "stiDesignerTreeItemButton stiDesignerTreeItemButtonDefault";

    treeItem.getAllChilds = function (childsArray) {
        if (!childsArray) childsArray = [];
        for (var itemKey in this.childs) {
            childsArray.push(this.childs[itemKey]);
            this.childs[itemKey].getAllChilds(childsArray);
        }

        return childsArray;
    }

    treeItem.getChildByName = function (name, useOtherNameProperties) {
        for (var itemKey in this.childs)
            if (this.childs[itemKey].itemObject.name == name ||
                (useOtherNameProperties && (this.childs[itemKey].itemObject.correctName == name || this.childs[itemKey].itemObject.nameInSource == name)))
                return this.childs[itemKey];

        return false;
    }

    treeItem.getFullName = function (useNameInSourcesInRelation) {
        var currItem = this;
        var fullName = "";
        while (currItem.parent != null) {
            if (fullName != "") fullName = "." + fullName;
            if (currItem.itemObject.typeItem == "Relation" && useNameInSourcesInRelation && currItem.itemObject.nameInSource) {
                fullName = currItem.itemObject.nameInSource + fullName;
            }
            else {
                fullName = (currItem.itemObject.correctName || currItem.itemObject.name) + fullName;
            }
            currItem = currItem.parent;
        }
        return fullName;
    }

    //Checkbox
    if (showCheckBox) {
        var checkBox = treeItem.checkBox = this.CheckBox(null);
        button.addCell(checkBox).style.width = "1px";
        checkBox.treeItem = treeItem;
        checkBox.style.marginLeft = "2px";

        treeItem.setChecked = function (state) {
            this.isChecked = state;
            checkBox.setChecked(state);
        }

        checkBox.action = function () {
            if (tree.isDisable) return;
            treeItem.setChecked(this.isChecked);
            var childs = treeItem.getAllChilds();
            for (var i = 0; i < childs.length; i++) {
                childs[i].setChecked(this.isChecked);
            }
            if (treeItem.parent) {
                if (this.isChecked) {
                    treeItem.parent.setChecked(true);
                }
            }
            if (tree["onChecked"]) {
                tree.onChecked(treeItem);
            }
        }
    }

    if (imageName != null) {
        var image = button.image = document.createElement("img");
        image.style.width = (imageSizes ? imageSizes.width : 16) + "px";
        image.style.height = (imageSizes ? imageSizes.height : 16) + "px";
        image.className = "stiDesignerTreeItemButtonImage" + (this.options.isTouchDevice ? "_Touch" : "_Mouse");
        if (StiMobileDesigner.checkImageSource(this.options, imageName)) StiMobileDesigner.setImageSource(image, this.options, imageName);

        var imageCell = button.imageCell = button.addCell(image);
        imageCell.style.fontSize = "0px"
    }

    if (caption != null || typeof (caption) == "undefined") {
        button.captionCell = button.addCell();
        button.captionCell.className = "stiDesignerTreeItemButtonCaption" + (this.options.isTouchDevice ? "_Touch" : "_Mouse");
        if (caption) button.captionCell.innerText = caption;
    }

    button.onmousedown = function () {
        if (this.isTouchStartFlag) return;
        this.action();
    }

    button.ontouchstart = function () {
        this.isTouchStartFlag = true;
        clearTimeout(this.isTouchStartTimer);
        this.action();
        this.isTouchStartTimer = setTimeout(function () {
            button.isTouchStartFlag = false;
        }, 1000);
    }

    button.action = function () {
        if (tree.isDisable) return;
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

    return treeItem;
}

StiMobileDesigner.prototype.Tree = function (width, height) {
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

    tree.moveSelector = function (keyCode, moveAndAction) {
        var getNextItem = function (currentItem) {
            var nextItem = currentItem.nextSibling;
            while (nextItem && nextItem.style.display == "none") {
                nextItem = nextItem.nextSibling;
            }
            return nextItem;
        }
        var getPrevItem = function (currentItem) {
            var prevItem = currentItem.previousSibling;
            while (prevItem && prevItem.style.display == "none") {
                prevItem = prevItem.previousSibling;
            }
            return prevItem;
        }

        var selectedItem = this.selectedItem;
        if (selectedItem) {
            switch (keyCode) {
                case 37: {
                    //left
                    if (selectedItem.getCountChilds() > 0 && selectedItem.isOpening) {
                        selectedItem.setOpening(false);
                    }
                    else if (selectedItem.parent) {
                        selectedItem.parent.setSelected(moveAndAction);
                    }
                    break;
                }
                case 38: {
                    //up
                    var prevItem = getPrevItem(selectedItem);
                    if (prevItem && prevItem.getCountChilds() > 0 && prevItem.isOpening) {
                        prevItem.childsContainer.childNodes[prevItem.childsContainer.childNodes.length - 1].setSelected(moveAndAction);
                    }
                    else if (prevItem) {
                        prevItem.setSelected(moveAndAction);
                    }
                    else if (!prevItem && selectedItem.parent) {
                        selectedItem.parent.setSelected(moveAndAction);
                    }
                    break;
                }
                case 39: {
                    //right
                    if (selectedItem.buildChildsNotCompleted) {
                        selectedItem.completeBuildTree(true);
                    }
                    if (selectedItem.getCountChilds() > 0 && !selectedItem.isOpening) {
                        selectedItem.setOpening(true);
                    }
                    else if (selectedItem.getCountChilds() > 0) {
                        selectedItem.childsContainer.childNodes[0].setSelected(moveAndAction);
                    }
                    break;
                }
                case 40: {
                    //down
                    var nextItem = getNextItem(selectedItem);
                    if (selectedItem.getCountChilds() > 0 && selectedItem.isOpening) {
                        selectedItem.childsContainer.childNodes[0].setSelected(moveAndAction);
                    }
                    else if (nextItem) {
                        nextItem.setSelected(moveAndAction);
                    }
                    else if (!nextItem && selectedItem.parent) {
                        var nextParentItem = getNextItem(selectedItem.parent);
                        if (nextParentItem) nextParentItem.setSelected(moveAndAction);
                    }
                    break;
                }
            }
        }
    }

    return tree;
}

StiMobileDesigner.prototype.TreeItemWithCheckBox = function (caption, imageName, itemObject, tree) {
    var treeItem = this.TreeItem(caption, imageName, itemObject, tree, true);
    treeItem.style.margin = "2px 0 2px 0";

    if (treeItem.checkBox) {
        treeItem.checkBox.action = function () {
            if (tree.isDisable) return;
            treeItem.setChecked(this.isChecked);
            var childs = treeItem.getAllChilds();

            for (var i = 0; i < childs.length; i++) {
                childs[i].setChecked(this.isChecked);
            }

            var setCheckedParent = function (item) {
                if (item.parent) {
                    if (item.isChecked) {
                        item.parent.setChecked(true);
                        setCheckedParent(item.parent);
                    }
                }
            }

            setCheckedParent(treeItem);

            if (tree.onChecked) tree.onChecked(treeItem);
        }
    }

    return treeItem;
}
