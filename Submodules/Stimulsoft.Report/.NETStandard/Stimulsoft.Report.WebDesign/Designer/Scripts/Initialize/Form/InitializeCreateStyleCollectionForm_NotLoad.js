
StiMobileDesigner.prototype.InitializeCreateStyleCollectionForm_ = function () {
    var createStyleCollectionForm = this.BaseForm("createStyleCollectionForm", this.loc.FormStyleDesigner.CreateStyleCollection, 3, this.HelpLinks["createStyleCollection"]);
    createStyleCollectionForm.controls = {};

    var upTable = this.CreateHTMLTable();
    upTable.style.marginTop = "6px";
    createStyleCollectionForm.container.appendChild(upTable);
    createStyleCollectionForm.container.appendChild(this.CreateStyleCollectionFormSeparator());

    var downTable = this.CreateHTMLTable();
    downTable.style.width = "100%";
    downTable.style.marginBottom = "6px";
    createStyleCollectionForm.container.appendChild(downTable);

    var controlsAttributes = [
        ["collectionName", this.loc.PropertyMain.CollectionName, this.TextBox("createStyleCollectionFormCollectionName", 150)],
        ["color", this.loc.PropertyMain.Color, this.ColorControl("createStyleCollectionFormColor", null, null, 150, true)],
        ["nestedLevel", this.loc.PropertyMain.NestedLevel, this.DropDownList("createStyleCollectionFormNestedLevel", 150, null, this.GetNestedLevelsItems(), true)],
        ["nestedFactor", this.loc.Report.LabelFactorLevel.replace(":", ""), this.DropDownList("createStyleCollectionFormNestedFactor", 150, null, this.GetNestedFactorItems(), true)],
        ["borders", "", this.CheckBox("createStyleCollectionFormBorders", this.loc.PropertyMain.Borders)],
        ["removeExistingStyles", "", this.CheckBox("createStyleCollectionFormRemoveExistingStyles", this.loc.FormStyleDesigner.RemoveExistingStyles)],
        ["groupHeader", "", this.CheckBox("createStyleCollectionFormGroupHeader", this.loc.Components.StiGroupHeaderBand)],
        ["reportTitle", "", this.CheckBox("createStyleCollectionFormReportTitle", this.loc.Components.StiReportTitleBand)],
        ["groupFooter", "", this.CheckBox("createStyleCollectionFormGroupFooter", this.loc.Components.StiGroupFooterBand)],
        ["reportSummary", "", this.CheckBox("createStyleCollectionFormReportSummary", this.loc.Components.StiReportSummaryBand)],
        ["header", "", this.CheckBox("createStyleCollectionFormHeader", this.loc.Components.StiHeaderBand)],
        ["pageHeader", "", this.CheckBox("createStyleCollectionFormPageHeader", this.loc.Components.StiPageHeaderBand)],
        ["data", "", this.CheckBox("createStyleCollectionFormData", this.loc.Components.StiDataBand)],
        ["pageFooter", "", this.CheckBox("createStyleCollectionFormPageFooter", this.loc.Components.StiPageFooterBand)],
        ["footer", "", this.CheckBox("createStyleCollectionFormFooter", this.loc.Components.StiFooterBand)]
    ]

    for (var i = 0; i < controlsAttributes.length; i++) {
        var control = createStyleCollectionForm.controls[controlsAttributes[i][0]] = controlsAttributes[i][2];
        var table = (i < 4) ? upTable : downTable;

        if (i < 4) {
            var textCell = table.addCellInNextRow();
            textCell.className = "stiDesignerCaptionControlsBigIntervals";
            textCell.innerHTML = controlsAttributes[i][1];
        }

        var controlCell = (i < 4)
            ? table.addCellInLastRow()
            : ((i < 6 || i % 2 == 0) ? table.addCellInNextRow() : table.addCellInLastRow());

        control.style.margin = i > 4 ? "8px 15px 8px 15px" : "6px 15px 6px 15px";
        controlCell.appendChild(control);

        if (controlsAttributes[i][0] == "removeExistingStyles") {
            table.addCellInNextRow(this.CreateStyleCollectionFormSeparator()).setAttribute("colspan", "2");
        }
    }

    createStyleCollectionForm.controls.removeExistingStyles.parentElement.className = null;

    var firstCell = createStyleCollectionForm.buttonsPanel.firstChild.tr[0].insertCell(0);
    firstCell.style.paddingRight = "30px";
    firstCell.appendChild(createStyleCollectionForm.controls.removeExistingStyles);

    createStyleCollectionForm.controls.color.setKey("251,220,192");
    createStyleCollectionForm.controls.nestedLevel.setKey("3");
    createStyleCollectionForm.controls.nestedFactor.setKey("Normal");

    for (var cName in createStyleCollectionForm.controls) {
        if (cName != "removeExistingStyles" && createStyleCollectionForm.controls[cName]["setChecked"] != null) {
            createStyleCollectionForm.controls[cName].setChecked(true);
        }
    }

    createStyleCollectionForm.getCollectionName = function () {
        var styleDesignerForm = this.jsObject.options.forms.styleDesignerForm;

        var stylesCollection = styleDesignerForm ? styleDesignerForm.stylesCollection : [];
        var fail = true;
        var index = 1;
        var name = this.jsObject.loc.Report.Collection;

        while (fail) {
            fail = false;
            name = this.jsObject.loc.Report.Collection + index;
            for (var i = 0; i < stylesCollection.length; i++) {
                if (stylesCollection[i].properties.collectionName == name) {
                    fail = true;
                    break;
                }
            }
            index++;
        }

        return name;
    }

    createStyleCollectionForm.onshow = function () {
        this.controls.collectionName.value = this.getCollectionName();
    }

    createStyleCollectionForm.action = function () {
        var styleCollectionProperties = {};
        for (var name in this.controls) {
            if (this.controls[name]["setKey"] != null) styleCollectionProperties[name] = this.controls[name].key;
            else if (this.controls[name]["setChecked"] != null) styleCollectionProperties[name] = this.controls[name].isChecked;
            else if (this.controls[name]["value"] != null) styleCollectionProperties[name] = this.controls[name].value;
        }
        this.jsObject.SendCommandCreateStyleCollection(styleCollectionProperties);
        this.changeVisibleState(false);
    }

    return createStyleCollectionForm;
}

StiMobileDesigner.prototype.CreateStyleCollectionFormSeparator = function () {
    var separator = this.FormSeparator();
    separator.style.margin = "6px 0 6px 0";

    return separator
}