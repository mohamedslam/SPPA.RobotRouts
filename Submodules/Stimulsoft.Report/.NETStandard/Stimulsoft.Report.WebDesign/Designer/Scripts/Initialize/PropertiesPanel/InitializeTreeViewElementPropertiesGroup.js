
StiMobileDesigner.prototype.TreeViewElementPropertiesGroup = function () {
    var group = this.PropertiesGroup("treeViewElementPropertiesGroup", this.loc.PropertyCategory.TreeViewCategory);
    group.style.display = "none";

    var props = [
        ["dataTransformation", this.loc.PropertyMain.DataTransformation, this.PropertyDataTransformationControl("treeViewElementDataTransformation", this.options.propertyControlWidth), "dataTransformationTreeView"],
        ["group", this.loc.PropertyMain.Group, this.PropertyTextBox("controlPropertyTreeViewElementGroup", this.options.propertyControlWidth), "groupTreeView"]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        group.container.appendChild(this.Property(props[i].length > 3 ? props[i][3] : props[i][0], props[i][1], control));

        control.action = function () {
            this.jsObject.ApplyPropertyValue(this.propertyName, this.jsObject.GetControlValue(this));
        }
    }

    return group;
}

