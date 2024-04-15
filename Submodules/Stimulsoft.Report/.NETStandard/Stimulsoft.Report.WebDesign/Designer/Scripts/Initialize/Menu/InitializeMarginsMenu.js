
StiMobileDesigner.prototype.MarginsMenu = function () {

    var menu = this.VerticalMenu("marginsMenu", this.options.buttons.marginsPage, "Down", this.GetPageMarginsItems(), "stiDesignerMenuMiddleItem");

    menu.constMargins = this.GetConstMargins();

    menu.action = function (menuItem) {
        var mInUnit = this.constMargins[this.jsObject.options.report.properties.reportUnit][menuItem.key];
        this.jsObject.options.currentPage.properties.unitMargins = mInUnit + "!" + mInUnit + "!" + mInUnit + "!" + mInUnit;
        menuItem.setSelected(true);
        this.changeVisibleState(false);
        this.jsObject.SendCommandSendProperties(this.jsObject.options.currentPage, ["unitMargins"]);
    }

    menu.onshow = function () {
        var text = function (options, num) {
            var unit = options.report.properties.reportUnit;
            var leftWord = menu.jsObject.loc.PropertyEnum.StiBorderSidesLeft;
            var topWord = menu.jsObject.loc.PropertyEnum.StiBorderSidesTop;
            var rightWord = menu.jsObject.loc.PropertyEnum.StiBorderSidesRight;
            var bottomWord = menu.jsObject.loc.PropertyEnum.StiBorderSidesBottom;

            return "<nobr>" + leftWord + ": " + num + unit + ", " + topWord + ": " + num + unit + ", " +
                rightWord + ": " + num + unit + ", " + bottomWord + ": " + num + unit + "</nobr>"
        }
        var options = this.jsObject.options;

        for (var itemName in this.items) {
            var locName = itemName.charAt(0).toUpperCase() + itemName.substr(1);
            var headText = menu.jsObject.loc.FormDesigner[locName];
            this.items[itemName].caption.innerHTML = "<b>" + headText +
                "</b><br>" + text(options, this.constMargins[options.report.properties.reportUnit][itemName]);
        }

        for (var marginName in this.constMargins[this.jsObject.options.report.properties.reportUnit]) {
            var constValue = this.constMargins[this.jsObject.options.report.properties.reportUnit][marginName];
            this.items[marginName].setSelected(constValue + "!" + constValue + "!" + constValue + "!" + constValue == this.jsObject.options.currentPage.properties.unitMargins);
        }
    }

    return menu;
}