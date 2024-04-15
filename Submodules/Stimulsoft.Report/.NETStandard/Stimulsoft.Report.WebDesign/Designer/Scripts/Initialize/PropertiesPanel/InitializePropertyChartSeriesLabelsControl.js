
StiMobileDesigner.prototype.PropertyChartSeriesLabelsControl = function (name, width) {
    var jsObject = this;
    var control = this.DropDownList(name, width, null, [], true);

    control.menu.onshow = function () {
        var labelsContainer = control.menu.innerContent;
        labelsContainer.style.height = "300px";
        labelsContainer.style.width = width + "px";

        this.clear();
        control.items = [];

        var seriesIndex = -1;
        var componentName = jsObject.options.selectedObject ? jsObject.options.selectedObject.properties.name : "";
        var forms = jsObject.options.forms;

        if (forms.editChartSeriesForm && forms.editChartSeriesForm.visible) {
            seriesIndex = forms.editChartSeriesForm.seriesContainer.getSelectedItemIndex();
            componentName = forms.editChartSeriesForm.chartProperties.name;
        }
        else if (forms.editChartSimpleForm && forms.editChartSimpleForm.visible) {
            seriesIndex = forms.editChartSimpleForm.controls.valuesBlock.container.getSelectedItemIndex();
            componentName = forms.editChartSimpleForm.chartProperties.name;
        }

        jsObject.SendCommandToDesignerServer("GetLabelsContent", { componentName: componentName, seriesIndex: seriesIndex }, function (answer) {
            var labelsContent = answer.labelsContent;

            if (labelsContent) {
                for (var i = 0; i < labelsContent.length; i++) {
                    var item = jsObject.BigButton(null, null, labelsContent[i].caption, " ", null, null);
                    item.key = labelsContent[i].type;
                    item.caption.style.height = "24px";
                    item.caption.style.verticalAlign = "top";
                    item.cellImage.removeChild(item.image);
                    item.image = document.createElement("div");
                    item.image.innerHTML = labelsContent[i].image;
                    item.cellImage.appendChild(item.image);
                    labelsContainer.appendChild(item);
                    item.setSelected(control.key == item.key);
                    control.items.push(item);

                    item.action = function () {
                        control.menu.changeVisibleState(false);
                        control.key = this.key;
                        control.textBox.value = this.caption.innerHTML;
                        control.action();
                    }
                }
            }
        });
    }

    return control;
}