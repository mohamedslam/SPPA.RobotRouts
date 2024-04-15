
StiMobileDesigner.prototype.InitializeEditElementDataTransformationForm_ = function () {
    var jsObject = this;
    var form = this.BaseFormPanel("editElementDataTransformationForm", this.loc.PropertyMain.DataTransformation, 1, this.HelpLinks["dataTransformation"]);
    form.buttonsPanel.style.display = "none";
    form.controls = {};

    var dataGrid = form.controls.dataGrid = this.DataGrid(650, 400);
    dataGrid.style.margin = "12px";
    dataGrid.style.borderStyle = "solid";
    form.container.appendChild(dataGrid);
    dataGrid.parentNode.style.position = "relative";
    this.AddProgressToControl(dataGrid.parentNode);

    var columnsContainer = form.controls.columnsContainer = this.DataTransformationContainer(190, 290, form);
    columnsContainer.onAction = function () { };
    columnsContainer.style.margin = "6px 0 6px 12px";

    //override addColumns
    columnsContainer.addColumns = function (columnsObjects) {
        for (var i = 0; i < columnsObjects.length; i++) {
            var columnObject = this.checkColumnName(columnsObjects[i]);
            this.addItem(jsObject.GetItemCaption(columnObject), "Meters." + columnObject.mode + ".png", columnObject);
        }
    }

    var hintBlock = document.createElement("div");
    hintBlock.innerText = this.loc.Dashboard.TransformationHint;
    hintBlock.className = "stiDataTransformHintBlock";
    form.container.appendChild(hintBlock);

    var errorBlock = document.createElement("div");
    errorBlock.className = "stiDataTransformErrorBlock";
    errorBlock.style.display = "none";
    form.container.appendChild(errorBlock);

    hintBlock.style.width = errorBlock.style.width = "650px";

    dataGrid.update = function () {
        if (form.elementName) {
            dataGrid.parentNode.progress.show();

            var params = {
                command: "GetDataGridContentForElementDataTransform",
                elementName: form.elementName,
                dataTransformationCacheGuid: form.dataTransformationCacheGuid
            };

            if (form.dataTransformation) {
                params.sortRules = form.dataTransformation.sortRules;
                params.filterRules = form.dataTransformation.filterRules;
                params.actionRules = form.dataTransformation.actionRules;
            }

            form.sendCommand(params,
                function (answer) {
                    dataGrid.parentNode.progress.hide();

                    if (!form.dataTransformation && answer.dataTransformation && answer.dataTransformationCacheGuid) {
                        form.dataTransformation = answer.dataTransformation;
                        form.dataTransformationCacheGuid = answer.dataTransformationCacheGuid;
                        columnsContainer.addColumns(answer.dataTransformation.columns);
                    }

                    if (answer.dataGridContent) {
                        var content = answer.dataGridContent;
                        if (content.errorMessage) {
                            errorBlock.style.display = "";
                            errorBlock.innerHTML = content.errorMessage;
                        }
                        else {
                            errorBlock.style.display = "none";
                            var oldScrollLeft = dataGrid.scrollLeft;
                            dataGrid.clear();
                            dataGrid.showData(content.data, content.sortLabels, content.filterLabels);
                            dataGrid.scrollLeft = oldScrollLeft;
                        }
                    }
                }
            );
        }
    }

    dataGrid.action = function (headerButton) {
        var sortFilterMenu = jsObject.SortFilterMenu(headerButton, form);
        sortFilterMenu.changeVisibleState(true);
        this.selectedHeaderButton = headerButton;
    }

    form.onshow = function () {
        this.elementName = jsObject.options.selectedObject ? jsObject.options.selectedObject.properties.name : null;
        this.dataTransformation = null;
        this.dataTransformationCacheGuid = null;
        columnsContainer.clear();
        dataGrid.clear();
        dataGrid.update();
    }

    form.onhide = function () {
        form.sendCommand({
            command: "ClearDataTransformCache",
            dataTransformationCacheGuid: form.dataTransformationCacheGuid
        }, function (answer) { });
    }

    form.action = function () {
        form.sendCommand({
            command: "SetElementDataTransformRules",
            elementName: form.elementName,
            dataTransformation: form.dataTransformation
        }, function (answer) {
            var currentElement = (jsObject.options.selectedObject && jsObject.options.selectedObject.properties.name == form.elementName)
                ? jsObject.options.selectedObject
                : jsObject.options.report.getComponentByName(form.elementName);
            if (currentElement) {
                currentElement.properties.svgContent = answer.svgContent;
                currentElement.repaint();
            }
        });

        form.changeVisibleState(false);
    }

    form.sendCommand = function (parameters, callbackFunction) {
        form.jsObject.SendCommandToDesignerServer("ExecuteCommandForDataTransformation",
            {
                parameters: parameters
            },
            function (answer) {
                callbackFunction(answer);
            });
    }

    return form;
}