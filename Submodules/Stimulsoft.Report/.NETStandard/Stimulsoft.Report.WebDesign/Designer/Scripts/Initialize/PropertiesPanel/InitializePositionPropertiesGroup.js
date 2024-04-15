
StiMobileDesigner.prototype.PositionPropertiesGroup = function () {
    var positionPropertiesGroup = this.PropertiesGroup("positionPropertiesGroup", this.loc.PropertyMain.Position);
    positionPropertiesGroup.style.display = "none";

    //Left
    var controlPropertyLeft = this.PropertyTextBox("controlPropertyLeft", this.options.propertyNumbersControlWidth);
    controlPropertyLeft.action = function () {
        if (this.value.trim() == "") return;
        this.value = this.jsObject.StrToDouble(this.value);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        var resultSelectedObjects = [];
        for (var i = 0; i < selectedObjects.length; i++) {
            if (selectedObjects[i].properties["clientLeft"] == null) continue;
            var deltaLeft = this.jsObject.StrToDouble(this.value) - this.jsObject.StrToDouble(selectedObjects[i].properties.clientLeft);
            selectedObjects[i].properties.unitLeft = this.jsObject.StrToDouble(selectedObjects[i].properties.unitLeft) + deltaLeft;
            resultSelectedObjects.push(selectedObjects[i]);
        }
        this.jsObject.SendCommandChangeRectComponent(resultSelectedObjects, "ResizeComponent", true);
    }
    positionPropertiesGroup.container.appendChild(this.Property("left", this.loc.PropertyMain.Left, controlPropertyLeft));

    //Top
    var controlPropertyTop = this.PropertyTextBox("controlPropertyTop", this.options.propertyNumbersControlWidth);
    controlPropertyTop.action = function () {
        if (this.value.trim() == "") return;
        this.value = this.jsObject.StrToDouble(this.value);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        var resultSelectedObjects = [];
        for (var i = 0; i < selectedObjects.length; i++) {
            if (selectedObjects[i].properties["clientTop"] == null) continue;
            var deltaTop = this.jsObject.StrToDouble(this.value) - this.jsObject.StrToDouble(selectedObjects[i].properties.clientTop);
            selectedObjects[i].properties.unitTop = this.jsObject.StrToDouble(selectedObjects[i].properties.unitTop) + deltaTop;
            resultSelectedObjects.push(selectedObjects[i]);
        }
        this.jsObject.SendCommandChangeRectComponent(resultSelectedObjects, "ResizeComponent", true);
    }
    positionPropertiesGroup.container.appendChild(this.Property("top", this.loc.PropertyMain.Top, controlPropertyTop));


    positionPropertiesGroup.setPropertyHelper = function (propertyName, propertyValue) {
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        if (!selectedObjects) return;
        var resultSelectedObjects = [];
        for (var i = 0; i < selectedObjects.length; i++) {
            if (selectedObjects[i].properties[propertyName] != null) {
                selectedObjects[i].properties[propertyName] = propertyValue;
                resultSelectedObjects.push(selectedObjects[i]);
            }
        }
        return resultSelectedObjects;
    }

    //Width
    var controlPropertyWidth = this.PropertyTextBox("controlPropertyWidth", this.options.propertyNumbersControlWidth);
    controlPropertyWidth.action = function () {
        if (this.value.trim() == "") return;
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        var selectedObjects = positionPropertiesGroup.setPropertyHelper("unitWidth", this.value);
        this.jsObject.SendCommandChangeRectComponent(selectedObjects, "ResizeComponent", true);
    }
    positionPropertiesGroup.container.appendChild(this.Property("width", this.loc.PropertyMain.Width, controlPropertyWidth));

    //Height
    var controlPropertyHeight = this.PropertyTextBox("controlPropertyHeight", this.options.propertyNumbersControlWidth);
    controlPropertyHeight.action = function () {
        if (this.value.trim() == "") return;
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        var selectedObjects = positionPropertiesGroup.setPropertyHelper("unitHeight", this.value);
        this.jsObject.SendCommandChangeRectComponent(selectedObjects, "ResizeComponent", true);
    }
    positionPropertiesGroup.container.appendChild(this.Property("height", this.loc.PropertyMain.Height, controlPropertyHeight));

    //Min Size
    var controlPropertyMinSize = this.PropertySizeControl("controlPropertyMinSize", this.options.propertyNumbersControlWidth + 74);
    controlPropertyMinSize.action = function () {
        if (this.getValue().trim() == "") return;
        this.jsObject.ApplyPropertyValue("minSize", this.getValue(), true);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        if (!selectedObjects) return;
        this.jsObject.SendCommandChangeRectComponent(selectedObjects, "ResizeComponent", true);
    }
    positionPropertiesGroup.container.appendChild(this.Property("minSize", this.loc.PropertyMain.MinSize, controlPropertyMinSize));

    //Max Size
    var controlPropertyMaxSize = this.PropertySizeControl("controlPropertyMaxSize", this.options.propertyNumbersControlWidth + 74);
    controlPropertyMaxSize.action = function () {
        if (this.getValue().trim() == "") return;
        this.jsObject.ApplyPropertyValue("maxSize", this.getValue());
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        if (!selectedObjects) return;
        this.jsObject.SendCommandChangeRectComponent(selectedObjects, "ResizeComponent", true);
    }
    positionPropertiesGroup.container.appendChild(this.Property("maxSize", this.loc.PropertyMain.MaxSize, controlPropertyMaxSize));

    //Min Height
    var controlPropertyMinHeight = this.PropertyTextBox("controlPropertyMinHeight", this.options.propertyNumbersControlWidth);
    controlPropertyMinHeight.action = function () {
        if (this.value.trim() == "") return;
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.ApplyPropertyValue("minHeight", this.value);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        if (!selectedObjects) return;
        this.jsObject.SendCommandChangeRectComponent(selectedObjects, "ResizeComponent", true);
    }
    positionPropertiesGroup.container.appendChild(this.Property("minHeight", this.loc.PropertyMain.MinHeight, controlPropertyMinHeight));

    //Max Height
    var controlPropertyMaxHeight = this.PropertyTextBox("controlPropertyMaxHeight", this.options.propertyNumbersControlWidth);
    controlPropertyMaxHeight.action = function () {
        if (this.value.trim() == "") return;
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.ApplyPropertyValue("maxHeight", this.value);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        if (!selectedObjects) return;
        this.jsObject.SendCommandChangeRectComponent(selectedObjects, "ResizeComponent", true);
    }
    positionPropertiesGroup.container.appendChild(this.Property("maxHeight", this.loc.PropertyMain.MaxHeight, controlPropertyMaxHeight));

    //Min Width
    var controlPropertyMinWidth = this.PropertyTextBox("controlPropertyMinWidth", this.options.propertyNumbersControlWidth);
    controlPropertyMinWidth.action = function () {
        if (this.value.trim() == "") return;
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.ApplyPropertyValue("minWidth", this.value);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        if (!selectedObjects) return;
        this.jsObject.SendCommandChangeRectComponent(selectedObjects, "ResizeComponent", true);
    }
    positionPropertiesGroup.container.appendChild(this.Property("minWidth", this.loc.PropertyMain.MinWidth, controlPropertyMinWidth));

    //Max Width
    var controlPropertyMaxWidth = this.PropertyTextBox("controlPropertyMaxWidth", this.options.propertyNumbersControlWidth);
    controlPropertyMaxWidth.action = function () {
        if (this.value.trim() == "") return;
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.ApplyPropertyValue("maxWidth", this.value);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        if (!selectedObjects) return;
        this.jsObject.SendCommandChangeRectComponent(selectedObjects, "ResizeComponent", true);
    }
    positionPropertiesGroup.container.appendChild(this.Property("maxWidth", this.loc.PropertyMain.MaxWidth, controlPropertyMaxWidth));

    return positionPropertiesGroup;
}