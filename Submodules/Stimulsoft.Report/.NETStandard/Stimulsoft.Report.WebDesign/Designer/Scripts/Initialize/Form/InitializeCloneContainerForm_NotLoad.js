
StiMobileDesigner.prototype.InitializeCloneContainerForm_ = function () {
    //Data Column Form
    var cloneContainerForm = this.BaseForm("cloneContainerForm", this.loc.FormTitles.ContainerSelectForm, 3, this.HelpLinks["cloneform"]);

    var containerItems = this.EasyContainer(300, 350);
    containerItems.className = "stiSimpleContainerWithBorder";
    containerItems.style.margin = "12px";
    cloneContainerForm.container.appendChild(containerItems);

    cloneContainerForm.show = function () {
        this.changeVisibleState(true);
        containerItems.clear();
        var currentObject = this.jsObject.options.selectedObject || this.jsObject.GetCommonObject(this.jsObject.options.selectedObjects);

        var items = this.jsObject.GetCloneContainerItems(currentObject ? currentObject.properties.name : null);
        for (var i = 0; i < items.length; i++) {
            var item = containerItems.addItem(items[i].name, items[i], items[i].caption, null, true);
            if (currentObject && currentObject.properties.container == items[i].key) {
                item.select();
            }
        }
    }

    cloneContainerForm.action = function () {
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            if (selectedObjects[i].properties.container != null) {
                selectedObjects[i].properties.container = containerItems.selectedItem ? containerItems.selectedItem.itemObject.key : "[Not Assigned]";
            }
        }
        this.changeVisibleState(false);
        this.jsObject.SendCommandSendProperties(selectedObjects, ["container"]);
    }

    return cloneContainerForm;
}