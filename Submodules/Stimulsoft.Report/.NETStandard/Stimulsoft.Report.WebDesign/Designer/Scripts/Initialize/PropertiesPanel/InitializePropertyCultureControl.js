
StiMobileDesigner.prototype.PropertyCultureControl = function (name, width) {
    var jsObject = this;
    var control = this.PropertyDropDownList(name, width, this.GetCultureItems(), true, false, null, false);
    var menuContainer = control.menu.innerContent;    
    menuContainer.style.maxWidth = "250px";

    var findTable = this.CreateHTMLTable();
    findTable.style.margin = "2px";
    findTable.style.width = "calc(100% - 4px)";
    menuContainer.insertBefore(findTable, menuContainer.firstChild);

    var findControl = this.FindControl(null, null, 24, true);
    findControl.style.width = findControl.textBox.style.width = "calc(100% - 6px)";
    findTable.addCell(findControl);

    findControl.action = function () {
        var value = this.getValue().toLowerCase();
        for (var itemName in control.menu.items) {
            var item = control.menu.items[itemName];
            item.style.display = value == "" || (item.caption && item.caption.innerHTML.toLowerCase().indexOf(value) >= 0) ? "" : "none";
        }
    }

    findControl.textBox.onchange = function () {
        findControl.action();
    }

    control.menu.onshow = function () {
        findControl.setValue("");
        findControl.action();
        findControl.textBox.focus();

        if (control.key == null) return;

        for (var itemName in this.items) {
            if (control.key == this.items[itemName].key) {
                this.items[itemName].setSelected(true);
                return;
            }
            else if (itemName.indexOf("separator") != 0) {
                this.items[itemName].setSelected(false);
            }
        }
    }

    var fxButton = this.SmallButton(null, null, null, "Function.png", this.loc.FormRichTextEditor.Insert, null, "stiDesignerFormButton");
    findTable.addCell(fxButton).style.width = "1px";
    fxButton.style.width = fxButton.style.height = "22px";
    fxButton.innerTable.style.width = "100%";
    fxButton.imageCell.style.padding = "0";

    fxButton.action = function () {
        control.menu.changeVisibleState(false);

        jsObject.InitializeTextEditorFormOnlyText(function (form) {
            form.showFunction = function () {
                this.textArea.value = control.key;
            }
            form.actionFunction = function () {
                var val = this.textArea.value;
                if (val.length > 0) {
                    if (val.indexOf("{") < 0) {
                        val = "{" + val;
                    }
                    if (!jsObject.EndsWith(val, "}")) {
                        val += "}";
                    }
                }
                control.setKey(val);
                control.action();
            }
            form.changeVisibleState(true);
        });
    }

    return control;
}