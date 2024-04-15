
StiMobileDesigner.prototype.InitializeTopNForm_ = function (isNotModal) {
    var form = this[isNotModal ? "DashboardBaseForm" : "BaseForm"]("topNForm", this.loc.PropertyMain.TopN, 1, this.HelpLinks["topN"]);
    form.controls = {};

    if (isNotModal) {
        form.isDockableToComponent = true;
        form.container.style.borderTop = "0px";
        form.caption.style.padding = "0px 10px 0 12px";
        form.hideButtonsPanel();
    }

    var properties = [
        ["mode", this.loc.PropertyMain.Mode, this.DropDownList("topNMode", 200, null, this.GetTopNModeItems(), true, false, null, true), "12px 12px 6px 12px"],
        ["count", this.loc.PropertyMain.Count, this.TextBoxEnumerator("topNCount", 200, null, false, null, 1), "6px 12px 6px 12px"],
        ["measureField", this.loc.Dashboard.Measure, this.DropDownList("topNMeasureField", 200, null, null, true, false, null, true), "12px 12px 6px 12px"],
        ["showOthers", " ", this.CheckBox("topNShowOthers", this.loc.PropertyMain.ShowOthers), "6px 12px 6px 12px"],
        ["othersText", this.loc.PropertyMain.OthersText, this.TextBox("topNCountOthersText", 200), "6px 12px 12px 12px"]
    ];

    var proprtiesTable = this.CreateHTMLTable();
    proprtiesTable.style.margin = "";
    form.container.appendChild(proprtiesTable);

    for (var i = 0; i < properties.length; i++) {
        var control = properties[i][2];
        control.controlName = properties[i][0];

        control.action = function () {
            if (this.controlName == "mode") {
                var state = this.key == "Top" || this.key == "Bottom";
                var oldState = form.controls.measureField.isEnabled;
                form.controls.measureField.setEnabled(state);
                form.controls.count.setEnabled(state);
                form.controls.showOthers.setEnabled(state);
                form.controls.othersText.setEnabled(state && form.controls.showOthers.isChecked);
                if (!oldState && state && form.controls.measureField.key == "" && form.controls.measureField.items && form.controls.measureField.items.length > 0)
                    form.controls.measureField.setKey(form.controls.measureField.items[0].key);
            }
            else if (this.controlName == "showOthers") {
                form.controls.othersText.setEnabled(this.isChecked);
            }

            if (isNotModal) {
                form.applyTopNProperty();
            }
        }

        form.addControlRow(proprtiesTable, properties[i][1], properties[i][0], control, properties[i][3]);
    }

    form.applyTopNProperty = function () {
        var topN = {
            mode: form.controls.mode.key,
            count: form.controls.count.getValue(),
            showOthers: form.controls.showOthers.isChecked,
            othersText: form.controls.othersText.value,
            measureField: form.controls.measureField.key
        }
        if (this.topNControl) {
            this.topNControl.setKey(topN);
            this.topNControl.action();
        }
        else if (this.dbsElement) {
            this.jsObject.ApplyPropertyValue(["topN"], topN);
        }
    }

    form.show = function (topNControl, summaries, dbsElement) {
        this.summaries = summaries;
        this.topNControl = topNControl;
        this.dbsElement = dbsElement;
        var topN = topNControl ? topNControl.key : (dbsElement ? dbsElement.properties.topN : null);
        if (topN) {
            form.controls.mode.setKey(topN.mode);
            var enabledState = topN.mode == "Top" || topN.mode == "Bottom";
            form.controls.count.setEnabled(enabledState);
            form.controls.showOthers.setEnabled(enabledState);
            form.controls.measureField.setEnabled(enabledState);
            form.controls.othersText.setEnabled(enabledState && topN.showOthers);
            form.controls.count.setValue(topN.count);
            form.controls.showOthers.setChecked(topN.showOthers);
            form.controls.othersText.value = topN.othersText;

            if (summaries) {
                form.controls.measureField.addItems(summaries);
                form.controls.measureField.setKey(topN.measureField);
            } else {
                form.controls.measureFieldRow.style.display = "none";
            }
        }
        form.changeVisibleState(true);
    }

    form.action = function () {
        form.changeVisibleState(false);
        form.applyTopNProperty();
    }

    return form;
}