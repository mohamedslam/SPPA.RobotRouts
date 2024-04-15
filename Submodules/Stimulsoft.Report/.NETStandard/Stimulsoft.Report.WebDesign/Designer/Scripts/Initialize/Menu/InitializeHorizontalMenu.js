
StiMobileDesigner.prototype.HorizontalMenu = function (name, parentButton, animDirection, items, itemsStyle, itemsHeight) {
    var menu = this.BaseMenu(name, parentButton, animDirection);
    menu.itemsStyle = itemsStyle;

    menu.clear = function () {
        while (this.innerContent.childNodes[0]) {
            this.innerContent.removeChild(this.innerContent.childNodes[0]);
        }
    }

    menu.addItems = function (items) {
        this.clear();
        if (items && items.length) {
            for (var i = 0; i < items.length; i++) {
                if (typeof (items[i]) != "string") {
                    var item = this.jsObject.VerticalMenuItem(this, items[i].name, items[i].caption, items[i].imageName, items[i].key, this.itemsStyle, items[i].haveSubMenu);
                    if (itemsHeight) item.style.height = itemsHeight + "px";
                    this.innerContent.appendChild(item);
                }
                else {
                    this.innerContent.appendChild(this.jsObject.VerticalMenuSeparator(this, items[i]));
                }
            }
        }
    }

    menu.onmousedown = function () {
        if (!this.isTouchStartFlag) this.ontouchstart(true);
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

    menu.addItems(items);

    return menu;
}