
StiMobileDesigner.prototype.DropDictionaryItemToDashboardElement = function (component, itemInDrag, event) {
    var form = this.options.currentForm && this.options.currentForm.visible && this.options.currentForm.dockingComponent == component ? this.options.currentForm : null;

    switch (component.typeComponent) {
        case "StiTableElement": {
            if (form) {
                form.controls.dataContainer.onmouseup(event);
            }
            else {
                this.InitializeEditTableElementForm(function (form) {
                    form.currentTableElement = component;
                    form.controls.dataContainer.onmouseup(event);
                });
            }
            break;
        }
        case "StiPivotTableElement": {
            if (form) {
                form.controls.columnsBlock.container.onmouseup(event);
            }
            else {
                this.InitializeEditPivotTableElementForm(function (form) {
                    form.currentPivotTableElement = component;
                    form.controls.columnsBlock.container.onmouseup(event);
                });
            }
            break;
        }
        case "StiChartElement": {
            if (form) {
                form.controls.valuesBlock.container.onmouseup(event);
            }
            else {
                this.InitializeEditChartElementForm(function (form) {
                    form.currentChartElement = component;
                    form.controls.valuesBlock.container.onmouseup(event);
                });
            }
            break;
        }
        case "StiGaugeElement": {
            if (form) {
                form.controls.valueDataColumn.innerContainer.onmouseup(event);
            }
            else {
                this.InitializeEditGaugeElementForm(function (form) {
                    form.currentGaugeElement = component;
                    form.controls.valueDataColumn.innerContainer.onmouseup(event);
                });
            }
            break;
        }
        case "StiIndicatorElement": {
            if (form) {
                form.controls.valueDataColumn.innerContainer.onmouseup(event);
            }
            else {
                this.InitializeEditIndicatorElementForm(function (form) {
                    form.currentIndicatorElement = component;
                    form.controls.valueDataColumn.innerContainer.onmouseup(event);
                });
            }
            break;
        }
        case "StiProgressElement": {
            if (form) {
                form.controls.valueDataColumn.innerContainer.onmouseup(event);
            }
            else {
                this.InitializeEditProgressElementForm(function (form) {
                    form.currentProgressElement = component;
                    form.controls.valueDataColumn.innerContainer.onmouseup(event);
                });
            }
            break;
        }
        case "StiRegionMapElement": {
            var applyDropEvent = function (form, component, event) {
                if (component.properties.dataFrom == "DataColumns") {
                    form.controls.keyDataColumn.innerContainer.onmouseup(event);
                }
            }

            if (form) {
                applyDropEvent(form, component, event);
            }
            else {
                this.InitializeEditRegionMapElementForm(function (form) {
                    form.currentRegionMapElement = component;
                    applyDropEvent(form, component, event);
                });
            }
            break;
        }
        case "StiOnlineMapElement": {
            if (form) {
                form.controls.latitudeDataColumn.innerContainer.onmouseup(event);
            }
            else {
                this.InitializeEditOnlineMapElementForm(function (form) {
                    form.currentOnlineMapElement = component;
                    form.controls.latitudeDataColumn.innerContainer.onmouseup(event);
                });
            }
            break;
        }
        case "StiListBoxElement": {
            if (form) {
                form.controls.keyDataColumn.innerContainer.onmouseup(event);
            }
            else {
                this.InitializeEditListBoxElementForm(function (form) {
                    form.currentListBoxElement = component;
                    form.controls.keyDataColumn.innerContainer.onmouseup(event);
                });
            }
            break;
        }
        case "StiComboBoxElement": {
            if (form) {
                form.controls.keyDataColumn.innerContainer.onmouseup(event);
            }
            else {
                this.InitializeEditComboBoxElementForm(function (form) {
                    form.currentComboBoxElement = component;
                    form.controls.keyDataColumn.innerContainer.onmouseup(event);
                });
            }
            break;
        }
        case "StiTreeViewElement": {
            if (form) {
                form.controls.keysBlock.container.onmouseup(event);
            }
            else {
                this.InitializeEditTreeViewElementForm(function (form) {
                    form.currentTreeViewElement = component;
                    form.controls.keysBlock.container.onmouseup(event);
                });
            }
            break;
        }
        case "StiTreeViewBoxElement": {
            if (form) {
                form.controls.keysBlock.container.onmouseup(event);
            }
            else {
                this.InitializeEditTreeViewBoxElementForm(function (form) {
                    form.currentTreeViewBoxElement = component;
                    form.controls.keysBlock.container.onmouseup(event);
                });
            }
            break;
        }
        case "StiDatePickerElement": {
            if (form) {
                form.controls.valueDataColumn.innerContainer.onmouseup(event);
            }
            else {
                this.InitializeEditDatePickerElementForm(function (form) {
                    form.currentDatePickerElement = component;
                    form.controls.valueDataColumn.innerContainer.onmouseup(event);
                });
            }
            break;
        }
        case "StiCardsElement": {
            if (form) {
                form.controls.valueDataColumn.innerContainer.onmouseup(event);
            }
            else {
                this.InitializeEditCardsElementForm(function (form) {
                    form.currentCardsElement = component;
                    form.controls.dataContainer.onmouseup(event);
                });
            }
            break;
        }
    }
}