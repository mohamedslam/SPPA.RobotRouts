
StiMobileDesigner.prototype.InitializeEditResourceForm_ = function () {
    var editResourceForm = this.BaseForm("editResourceForm", this.loc.PropertyMain.Column, 3, this.HelpLinks["columnEdit"]);
    editResourceForm.resource = null;
    editResourceForm.controls = {};

    var saveCopyButton = this.FormButton(null, null, this.loc.Buttons.SaveCopy, null);
    saveCopyButton.style.display = "inline-block";
    saveCopyButton.style.margin = "12px";

    saveCopyButton.action = function () {
        var sourceResName = editResourceForm.controls.name.value;
        if (!editResourceForm.controls.name.checkExists(this.jsObject.options.report.dictionary.resources, "name")) {
            if (editResourceForm.controls.name.value == editResourceForm.controls.alias.value) {
                editResourceForm.controls.alias.value += "Copy";
            }
            editResourceForm.controls.name.value += "Copy";

            var resultName = editResourceForm.controls.name.value;
            var i = 2;
            while (!editResourceForm.controls.name.checkExists(this.jsObject.options.report.dictionary.resources, "name")) {
                editResourceForm.controls.name.value = editResourceForm.controls.alias.value = resultName + i;
                i++;
            }
        }
        editResourceForm.mode = "New";
        editResourceForm.action(sourceResName);
    }

    var footerTable = this.CreateHTMLTable();
    footerTable.style.width = "100%";
    var buttonsPanel = editResourceForm.buttonsPanel;
    editResourceForm.removeChild(buttonsPanel);
    editResourceForm.appendChild(footerTable);
    footerTable.addCell(saveCopyButton).style.width = "1px";
    footerTable.addCell();
    footerTable.addCell(editResourceForm.buttonOk).style.width = "1px";
    footerTable.addCell(editResourceForm.buttonCancel).style.width = "1px";

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "6px 0 6px 0";
    editResourceForm.container.appendChild(innerTable);

    var controlProps = [
        ["name", this.loc.PropertyMain.Name, this.TextBox(null, 270)],
        ["alias", this.loc.PropertyMain.Alias, this.TextBox(null, 270)]
    ]

    for (var i = 0; i < controlProps.length; i++) {
        innerTable.addTextCellInNextRow(controlProps[i][1]).className = "stiDesignerCaptionControlsBigIntervals";
        innerTable.addCellInLastRow(controlProps[i][2]).className = "stiDesignerControlCellsBigIntervals2";
        editResourceForm.controls[controlProps[i][0]] = controlProps[i][2];
    }

    var resourceContainer = this.ResourceContainer(null, 450, 250);
    resourceContainer.style.margin = "6px 12px 6px 12px";
    editResourceForm.controls.resourceContainer = resourceContainer;
    editResourceForm.container.appendChild(resourceContainer);

    var availableInTheViewer = this.CheckBox(null, this.loc.PropertyMain.AvailableInTheViewer);
    availableInTheViewer.style.margin = "12px";
    editResourceForm.container.appendChild(availableInTheViewer);

    editResourceForm.updateStateAvailableInTheViewer = function () {
        var resourceType = resourceContainer.resourceType;

        availableInTheViewer.setEnabled(
            resourceContainer.haveContent && (
                resourceType == "Image" ||
                resourceType == "Excel" ||
                resourceType == "Rtf" ||
                resourceType == "Txt" ||
                resourceType == "Pdf" ||
                resourceType == "Word" ||
                resourceType == "Report" ||
                resourceType == "ReportSnapshot" ||
                resourceType == "Csv"
            )
        );
    }

    resourceContainer.action = function () {
        if (editResourceForm.mode == "New") {
            editResourceForm.controls.name.value = editResourceForm.controls.alias.value = this.resourceName;
            var i = 2;
            while (!editResourceForm.controls.name.checkExists(this.jsObject.options.report.dictionary.resources, "name")) {
                editResourceForm.controls.name.value = editResourceForm.controls.alias.value = this.resourceName + i;
                i++;
            }
            editResourceForm.controls.name.hideError();
        }
    }

    resourceContainer.onChange = function () {
        editResourceForm.updateStateAvailableInTheViewer();
    }

    editResourceForm.controls.name.action = function () {
        if (this.oldValue == editResourceForm.controls.alias.value) {
            editResourceForm.controls.alias.value = this.value;
        }
    }

    editResourceForm.onshow = function () {
        this.mode = "Edit";
        if (this.resource == null) {
            this.resource = this.jsObject.ResourceObject();
            this.mode = "New";
        }
        saveCopyButton.style.display = resourceContainer.saveButton.style.display = this.mode == "Edit" ? "" : "none";
        this.caption.innerHTML = this.jsObject.loc.FormDictionaryDesigner["Resource" + this.mode];
        this.controls.name.hideError();
        this.controls.name.focus();
        this.controls.name.value = this.resource.name;
        this.controls.alias.value = this.resource.alias;
        resourceContainer.clear();
        availableInTheViewer.setChecked(this.resource && this.resource.availableInTheViewer);

        if (this.mode == "Edit") {
            resourceContainer.getResourceContentFromServer(this.resource.name);
        }
        else {
            resourceContainer.setResource(null, this.resource.type, this.resource.name, 0, null, false);
        }
    }

    editResourceForm.action = function (saveCopy) {
        var jsObject = this.jsObject;
        this.changeVisibleState(false);

        setTimeout(function () {
            var resource = {};
            resource.mode = editResourceForm.mode;

            if (!editResourceForm.controls.name.checkNotEmpty(jsObject.loc.PropertyMain.Name)) return;
            if ((editResourceForm.mode == "New" || editResourceForm.resource.name != editResourceForm.controls.name.value) &&
                !editResourceForm.controls.name.checkExists(jsObject.options.report.dictionary.resources, "name"))
                return;

            resource.oldName = editResourceForm.resource.name;
            resource.name = editResourceForm.controls.name.value;
            resource.alias = editResourceForm.controls.alias.value;
            resource.type = resourceContainer.resourceType;
            resource.loadedContent = jsObject.options.mvcMode ? encodeURIComponent(resourceContainer.loadedContent) : resourceContainer.loadedContent;
            resource.haveContent = resourceContainer.haveContent;
            resource.availableInTheViewer = availableInTheViewer.isEnabled && availableInTheViewer.isChecked;
            resource.saveCopy = saveCopy;

            jsObject.SendCommandCreateOrEditResource(resource);
        }, 50);
    }

    return editResourceForm;
}