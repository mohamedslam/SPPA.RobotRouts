
StiMobileDesigner.prototype.ContainerPropertiesGroup = function () {
    var containerPropertiesGroup = this.PropertiesGroup("containerPropertiesGroup", this.loc.PropertyMain.Data);
    containerPropertiesGroup.style.display = "none";

    //Container
    var controlPropertyCloneContainer = this.PropertyTextBoxWithEditButton("controlPropertyCloneContainer", this.options.propertyControlWidth, true);
    controlPropertyCloneContainer.button.action = function () {
        this.jsObject.InitializeCloneContainerForm(function (cloneContainerForm) {
            cloneContainerForm.show();
        });
    }
    containerPropertiesGroup.container.appendChild(this.Property("container", this.loc.PropertyMain.Container, controlPropertyCloneContainer));

    return containerPropertiesGroup;
}