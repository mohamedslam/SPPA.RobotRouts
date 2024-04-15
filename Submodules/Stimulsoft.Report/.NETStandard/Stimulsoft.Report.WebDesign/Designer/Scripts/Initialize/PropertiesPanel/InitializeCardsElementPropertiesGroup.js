
StiMobileDesigner.prototype.CardsElementPropertiesGroup = function () {
    var jsObject = this;
    var group = this.PropertiesGroup("cardsElementPropertiesGroup", this.loc.Components.StiCards);
    group.style.display = "none";

    var props = [
        ["cardsColorEach", this.loc.PropertyMain.ColorEach, this.CheckBox("controlPropertyCardsColorEach"), "cardsColorEach"],
        ["cardsCornerRadius", this.loc.PropertyMain.CornerRadius, this.PropertyCornerRadiusControl("controlPropertyCardsCornerRadius", this.options.propertyControlWidth + 61)],
        ["columnCount", this.loc.PropertyMain.ColumnCount, this.PropertyTextBox("controlPropertyCardsColumnCount", this.options.propertyNumbersControlWidth), "cardsColumnCount"],
        ["crossFiltering", this.loc.PropertyMain.CrossFiltering, this.CheckBox("controlPropertyCardsCrossFiltering"), "crossFilteringCards"],
        ["dataTransformation", this.loc.PropertyMain.DataTransformation, this.PropertyDataTransformationControl("cardsElementDataTransformation", this.options.propertyControlWidth), "dataTransformationCards"],
        ["group", this.loc.PropertyMain.Group, this.PropertyTextBox("controlPropertyCardsGroup", this.options.propertyControlWidth), "groupCards"],
        ["cardsMargin", this.loc.PropertyMain.Margin, this.PropertyMarginsControl("controlPropertyCardsMargin", this.options.propertyControlWidth + 61)],
        ["orientation", this.loc.PropertyMain.Orientation, this.PropertyDropDownList("controlPropertyCardsOrientation", this.options.propertyControlWidth, this.GetParametersOrientationItems(), true), "cardsOrientation"],
        ["cardsPadding", this.loc.PropertyMain.Padding, this.PropertyMarginsControl("controlPropertyCardsPadding", this.options.propertyControlWidth + 61)]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        group.container.appendChild(this.Property(props[i].length > 3 ? props[i][3] : props[i][0], props[i][1], control));

        control.action = function () {
            if (this.propertyName == "columnCount") {
                this.value = jsObject.StrToCorrectPositiveInt(this.value);
            }
            jsObject.ApplyPropertyValue(this.propertyName, jsObject.GetControlValue(this));
        }
    }

    return group;
}