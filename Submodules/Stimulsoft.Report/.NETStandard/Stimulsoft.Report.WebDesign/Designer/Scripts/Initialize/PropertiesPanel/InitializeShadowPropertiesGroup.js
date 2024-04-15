
StiMobileDesigner.prototype.ShadowPropertiesGroup = function () {
    var jsObject = this;
    var group = this.PropertiesGroup("shadowPropertiesGroup", this.loc.PropertyMain.Shadow, null, 1);
    group.style.display = "none";

    var props = [
        ["shadowColor", this.loc.PropertyMain.Color, this.PropertyColorControl("controlPropertyShadowColor", null, this.options.propertyControlWidth + 5)],
        ["shadowLocation", this.loc.PropertyMain.Location, this.PropertyPointControl("controlPropertyShadowLocation", this.options.propertyNumbersControlWidth * 2)],
        ["shadowSize", this.loc.PropertyMain.Size, this.PropertyTextBox("controlPropertyShadowSize", this.options.propertyNumbersControlWidth)],
        ["shadowVisible", this.loc.PropertyMain.Visible, this.CheckBox("controlPropertyShadowVisible")]
    ]

    var designButtonBlock = this.PropertyBlockWithButton("propertiesDesignShadowBlock", "ShadowBig.png", this.loc.Buttons.Design + "...", this.options.propertyControlWidth + 123);
    designButtonBlock.style.marginTop = "5px";
    group.container.appendChild(designButtonBlock);

    designButtonBlock.button.action = function () {
        jsObject.InitializeShadowForm(function (form) {
            form.changeVisibleState(true);
        });
    }

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        group.container.appendChild(this.Property(props[i].length > 3 ? props[i][3] : props[i][0], props[i][1], control, null, 2));

        control.action = function () {
            if (this.propertyName == "shadowSize") {
                this.value = jsObject.StrToCorrectPositiveInt(this.value);
            }
            jsObject.ApplyPropertyValue(this.propertyName, this.propertyName == "shadowLocation" ? this.getValue() : jsObject.GetControlValue(this));
        }
    }

    return group;
}