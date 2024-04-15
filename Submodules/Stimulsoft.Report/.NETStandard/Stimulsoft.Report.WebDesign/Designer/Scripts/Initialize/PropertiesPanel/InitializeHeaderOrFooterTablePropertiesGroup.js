
StiMobileDesigner.prototype.HeaderOrFooterTablePropertiesGroup = function (groupName) {
    var upperGroupName = this.UpperFirstChar(groupName);

    var propertiesGroup = this.PropertiesGroup(groupName + "TablePropertiesGroup", this.loc.PropertyCategory[upperGroupName + "TableCategory"]);
    propertiesGroup.style.display = "none";

    var props = [
        [groupName + "PrintOn", this.loc.PropertyMain[upperGroupName + "PrintOn"],
        this.PropertyDropDownList("controlProperty" + upperGroupName + "PrintOn", this.options.propertyControlWidth, this.GetPrintOnItems(), true)],
        [groupName + "CanGrow", this.loc.PropertyMain[upperGroupName + "CanGrow"], this.CheckBox("controlProperty" + upperGroupName + "CanGrow")],
        [groupName + "CanShrink", this.loc.PropertyMain[upperGroupName + "CanShrink"], this.CheckBox("controlProperty" + upperGroupName + "CanShrink")],
        [groupName + "CanBreak", this.loc.PropertyMain[upperGroupName + "CanBreak"], this.CheckBox("controlProperty" + upperGroupName + "CanBreak")],
        [groupName + "PrintAtBottom", this.loc.PropertyMain[upperGroupName + "PrintAtBottom"], this.CheckBox("controlProperty" + upperGroupName + "PrintAtBottom")],
        [groupName + "PrintIfEmpty", this.loc.PropertyMain[upperGroupName + "PrintIfEmpty"], this.CheckBox("controlProperty" + upperGroupName + "PrintIfEmpty")],
        [groupName + "PrintOnAllPages", this.loc.PropertyMain[upperGroupName + "PrintOnAllPages"], this.CheckBox("controlProperty" + upperGroupName + "PrintOnAllPages")],
        [groupName + "PrintOnEvenOddPages", this.loc.PropertyMain[upperGroupName + "PrintOnEvenOddPages"],
        this.PropertyDropDownList("controlProperty" + upperGroupName + "PrintOnEvenOddPages", this.options.propertyControlWidth, this.GetPrintOnEvenOddPagesItems(), true)]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        propertiesGroup.container.appendChild(this.Property(props[i][0], props[i][1], control));

        control.action = function () {
            this.jsObject.ApplyPropertyValue(this.propertyName, this.jsObject.GetControlValue(this));
        }
    }

    return propertiesGroup;
}

