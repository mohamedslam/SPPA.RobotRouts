
StiMobileDesigner.prototype.InitializeShadowForm_ = function () {
    var jsObject = this;
    var form = this.BaseForm("shadowForm", this.loc.PropertyMain.Shadow, 1);
    form.controls = {};

    var controlsTable = this.CreateHTMLTable();
    form.container.appendChild(controlsTable);

    var props = [
        ["samplesHeader", null, this.FormBlockHeader(this.loc.MainMenu.menuHelpSamples), "6px 6px 0 6px"],
        ["samplesBlock", null, this.ShadowSamplesBlock(form), "0 6px 0 6px"],
        ["shadowHeader", null, this.FormBlockHeader(this.loc.PropertyMain.Shadow), "6px"],
        ["color", this.loc.PropertyMain.Color, this.PropertyColorControl("shadowFormColor", null, this.options.propertyControlWidth), "6px"],
        ["size", this.loc.PropertyMain.Size, this.SladerControl("shadowFormSize", 180, 0, 10, true), "6px"],
        ["x", "X", this.SladerControl("shadowFormX", 180, -10, 10, true), "6px"],
        ["y", "Y", this.SladerControl("shadowFormY", 180, -10, 10, true), "6px"],
        ["visible", " ", this.CheckBox(null, this.loc.PropertyMain.Visible), "6px 6px 12px 6px"]
    ]

    for (var i = 0; i < props.length; i++) {
        var controlCell = form.addControlRow(controlsTable, props[i][1], props[i][0], props[i][2], props[i][3], 40, 24);
        if (props[i][0] == "samplesBlock") controlCell.style.textAlign = "center";
    }

    form.controls.samplesBlock.style.display = "inline-block";

    form.onshow = function () {
        var selectedObjects = jsObject.options.selectedObject ? [jsObject.options.selectedObject] : jsObject.options.selectedObjects;
        if (selectedObjects && selectedObjects.length > 0) {
            var properties = selectedObjects[0].properties;
            var locSizes = properties.shadowLocation.split(";");

            form.controls.color.setKey(properties.shadowColor);
            form.controls.size.setValue(properties.shadowSize);
            form.controls.visible.setChecked(properties.shadowVisible);
            form.controls.x.setValue(parseInt(locSizes[0]));
            form.controls.y.setValue(parseInt(locSizes[1]));
        }
    }

    form.action = function () {
        jsObject.ApplyPropertyValue(
            ["shadowColor", "shadowLocation", "shadowSize", "shadowVisible"],
            [form.controls.color.key, form.controls.x.getValue().toString() + ";" + form.controls.y.getValue().toString(), form.controls.size.getValue().toString(), form.controls.visible.isChecked]
        );
        this.changeVisibleState(false);
    }

    return form;
}

StiMobileDesigner.prototype.ShadowSamplesBlock = function (form) {
    var table = this.CreateHTMLTable();
    table.style.margin = "12px";

    var props = [
        [5, -3, -3],
        [4, 0, -4],
        [5, 3, -3],
        [4, -4, 0],
        [10, 0, 0],
        [4, 4, 0],
        [5, -3, 3],
        [4, 0, 4],
        [5, 3, 3]
    ]

    for (var i = 0; i < 9; i++) {
        var button = this.ShadowSampleButton(props[i]);

        button.action = function () {
            form.controls.size.setValue(this.props[0]);
            form.controls.x.setValue(this.props[1]);
            form.controls.y.setValue(this.props[2]);
        }

        if (i != 0 && i % 3 == 0)
            table.addCellInNextRow(button);
        else
            table.addCellInLastRow(button);
    }

    return table;
}

StiMobileDesigner.prototype.ShadowSampleButton = function (props) {
    var button = this.SmallButton(null, null, null, "Shadow.png");
    button.style.width = button.style.height = "auto";
    button.style.margin = "12px 12px 0 0";
    button.props = props;

    var bar = document.createElement("div");
    bar.style.border = "2px solid #4d82b8";
    bar.style.width = bar.style.height = "24px";
    bar.style.boxShadow = props[1] + "px " + props[2] + "px " + props[0] + "px black";
    bar.style.margin = "10px";

    button.imageCell.innerHTML = "";
    button.imageCell.appendChild(bar);

    return button;
}