
StiMobileDesigner.prototype.BaseContextMenu = function (name, direction, items, itemsStyle) {

    var menu = this.VerticalMenu(name, null, direction, items, itemsStyle)
    menu.direction = direction;

    menu.show = function (xPos, yPos, vertDirection, horDirection) {
        this.onshow();
        this.innerContent.style.top = "0px";
        this.style.display = "";
        var leftPos = (horDirection == "Right" ? xPos : xPos - this.innerContent.offsetWidth);
        var topPos = (vertDirection == "Up" ? yPos - this.innerContent.offsetHeight : yPos);
        var browserHeight = (window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight) - this.jsObject.FindPosY(this.jsObject.options.mainPanel);
        var browserWidth = (window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth) - this.jsObject.FindPosX(this.jsObject.options.mainPanel);;
        if (topPos + this.innerContent.offsetHeight > browserHeight) {
            topPos = browserHeight - this.innerContent.offsetHeight - 10;
        }
        if (leftPos + this.innerContent.offsetWidth > browserWidth) {
            leftPos = browserWidth - this.innerContent.offsetWidth - 10;
        }
        this.style.left = leftPos + "px";
        this.style.top = topPos + "px";
        this.jsObject.options.currentMenu = this;
    }

    return menu;
}