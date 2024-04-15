
StiMobileDesigner.prototype.PdfSignaturePropertiesGroup = function () {
    var jsObject = this;
    var group = this.PropertiesGroup("pdfSignaturePropertiesGroup", this.loc.Components.StiSignature);
    group.style.display = "none";
    group.innerGroups = {};

    var props = [
        ["placeholder", this.loc.PropertyMain.Placeholder, this.PropertyTextBoxWithEditButton("controlPropertyPlaceholder", null, false, false, null, true)]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        group.container.appendChild(this.Property(props[i].length > 3 ? props[i][3] : props[i][0], props[i][1], control, null, group.nestingLevel));

        control.action = function () {
            var value = jsObject.GetControlValue(this);
            jsObject.ApplyPropertyValue(this.propertyName, this.propertyName == "placeholder" ? StiBase64.encode(value) : value);
        }
    }

    return group;
}