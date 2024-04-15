
StiMobileDesigner.prototype.DashboardPropertiesGroup = function () {
    var dashboardPropertiesGroup = this.PropertiesGroup("dashboardPropertiesGroup", this.loc.Components.StiDashboard);
    dashboardPropertiesGroup.style.display = "none";

    //Dashboard Width
    var widthControl = this.PropertyTextBox("controlPropertyDashboardWidth", this.options.propertyNumbersControlWidth);
    widthControl.action = function () {
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.options.currentPage.properties.unitWidth = this.value;
        this.jsObject.SendCommandSendProperties(this.jsObject.options.currentPage, ["unitWidth"]);
    }
    dashboardPropertiesGroup.container.appendChild(this.Property("pageWidth", this.loc.PropertyMain.Width, widthControl));

    //Dashboard Height
    var heightControl = this.PropertyTextBox("controlPropertyDashboardHeight", this.options.propertyNumbersControlWidth);
    heightControl.action = function () {
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.options.currentPage.properties.unitHeight = this.value;
        this.jsObject.SendCommandSendProperties(this.jsObject.options.currentPage, ["unitHeight"]);
    }
    dashboardPropertiesGroup.container.appendChild(this.Property("pageHeight", this.loc.PropertyMain.Height, heightControl));

    //Device Width
    var deviceWidthControl = this.PropertyTextBox("controlPropertyDeviceWidth", this.options.propertyNumbersControlWidth);
    deviceWidthControl.action = function () {
        this.value = this.jsObject.StrToCorrectPositiveInt(this.value);
        this.jsObject.options.currentPage.properties.deviceWidth = this.value;
        this.jsObject.SendCommandSendProperties(this.jsObject.options.currentPage, ["deviceWidth"]);
    }
    dashboardPropertiesGroup.container.appendChild(this.Property("deviceWidth", this.loc.PropertyMain.DeviceWidth, deviceWidthControl));

    return dashboardPropertiesGroup;
}