
StiMobileDesigner.prototype.DbsIconSetControl = function (name, width, height, toolTip) {
    var control = this.DropDownList(name, width, toolTip, [], true, false, height, false, { width: 120, height: 16 });
    control.items = {};

    var itemsTable = this.CreateHTMLTable();
    control.menu.innerContent.appendChild(itemsTable);
    control.menu.innerContent.style.minWidth = "auto";

    control.textBox.style.fontFamily = "Stimulsoft";
    control.textBox.style.fontSize = height ? (height - 6) + "px" : "15px";
    control.textBox.style.color = "#4472c4";

    for (var i = 0; i < this.options.fontIconSets.length; i++) {
        if (i != 0) itemsTable.addRow();

        var itemButton = this.StandartSmallButton(null, null, this.options.fontIconSets[i].text);
        itemButton.style.height = "26px";
        itemButton.key = this.options.fontIconSets[i].key;
        control.items[itemButton.key] = itemButton;

        if (itemButton.caption) {
            itemButton.caption.style.padding = "0 5px 0 5px";
            itemButton.caption.style.fontFamily = "Stimulsoft";
            itemButton.caption.style.fontSize = "18px";
        }
        itemsTable.addCellInLastRow(itemButton);

        itemButton.action = function () {
            control.setKey(this.key);
            control.menu.changeVisibleState(false);
            control.action();
        }
    }

    control.menu.onshow = function () {
        for (var key in control.items) {
            control.items[key].setSelected(key == control.key);

            if (control.items[key].caption) {
                control.items[key].caption.style.color = control.textBox.style.color;
            }
        }
    }

    control.setKey = function (key) {
        this.key = key;
        if (this.items[key]) {
            this.textBox.value = this.items[key].caption.innerHTML;
        }
    }

    return control;
}