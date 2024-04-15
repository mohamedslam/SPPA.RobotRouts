
StiMobileDesigner.prototype.PropertyDashboardInteractionControl = function (name, width, height) {
    var control = this.PropertyTextBoxWithEditButton(name, width, true, true, height);
    control.key = null;

    control.button.action = function () {
        this.jsObject.InitializeDashboardInteractionForm(function (form) {
            control.preOpening();
            form.show(control.key, control.columnNames);

            form.action = function () {
                form.changeVisibleState(false);
                control.setKey(form.getResultInteraction());
                control.action();
            }
        });
    }

    control.clearButton.action = function () {
        control.setToDefault();
        control.action();
    }

    control.setKey = function (key) {
        this.key = key;
        this.textBox.value = "[" + (this.isDefault() ? this.jsObject.loc.PropertyMain.Default : this.jsObject.loc.FormFormatEditor.Custom) + "]";
        this.clearButton.setEnabled(!this.isDefault());
    }

    control.setToDefault = function () {
        if (this.key) {
            this.key.hyperlinkDestination = "NewTab";
            this.key.hyperlink = "";
            this.key.toolTip = "";
            this.key.drillDownPageKey = "";
            this.key.drillDownParameters = [];

            switch (this.key.ident) {
                case "TableColumn": {
                    this.key.onHover = "None";
                    this.key.onClick = "None";
                    break;
                }
                case "Table": {
                    this.key.allowUserFiltering = true;
                    this.key.allowUserSorting = true;
                    this.key.onHover = "ShowToolTip";
                    this.key.onClick = "ApplyFilter";
                    break;
                }
                case "Chart":
                case "RegionMap": {
                    this.key.onHover = "ShowToolTip";
                    this.key.onClick = "ApplyFilter";
                    break;
                }
                case "Text":
                case "Image": {
                    this.key.onHover = "ShowToolTip";
                    this.key.onClick = "OpenHyperlink";
                    break;
                }
                default: {
                    this.key.onHover = "None";
                    this.key.onClick = "None";
                }
            }
        }
        control.clearButton.setEnabled(false);
        this.textBox.value = "[" + this.jsObject.loc.PropertyMain.Default + "]";
    }

    control.isDefault = function () {
        return (this.key && !this.key.dashboardKey && this.key.hyperlinkDestination == "NewTab" && !this.key.hyperlink && !this.key.toolTip &&
            ((this.key.ident == "TableColumn" && this.key.onHover == "None" && this.key.onClick == "None") ||
                ((this.key.ident == "Chart" || this.key.ident == "RegionMap") && this.key.onHover == "ShowToolTip" && this.key.onClick == "ApplyFilter") ||
                ((this.key.ident == "Text" || this.key.ident == "Image") && this.key.onHover == "ShowToolTip" && this.key.onClick == "OpenHyperlink") ||
                (this.key.ident == "Table" && this.key.allowUserSorting && this.key.allowUserFiltering)));
    }

    control.preOpening = function () { };

    return control;
}