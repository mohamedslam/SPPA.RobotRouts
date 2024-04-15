
StiMobileDesigner.prototype.EventsPropertiesPanel = function () {
    var eventsPropertiesPanel = document.createElement("div");
    eventsPropertiesPanel.style.display = "none";
    var jsObject = eventsPropertiesPanel.jsObject = this;

    eventsPropertiesPanel.show = function () {
        this.style.display = "";
        this.update();
    }

    eventsPropertiesPanel.hide = function () {
        this.style.display = "none";
    }

    eventsPropertiesPanel.selectedComponentParams = function () {
        var selectedObjects = jsObject.options.selectedObject ? [jsObject.options.selectedObject] : jsObject.options.selectedObjects;
        var selectedComponentParams = [];

        if (selectedObjects) {
            for (var i = 0; i < selectedObjects.length; i++) {
                selectedComponentParams.push({
                    name: selectedObjects[i].properties.name,
                    typeComponent: selectedObjects[i].typeComponent
                });
            }
        }

        return selectedComponentParams;
    }

    eventsPropertiesPanel.update = function () {
        if (this.groups) {
            for (var groupName in this.groups)
                this.groups[groupName].style.display = "none";
        }

        var selectedObjects = jsObject.options.selectedObject ? [jsObject.options.selectedObject] : jsObject.options.selectedObjects;
        var commonObject = jsObject.GetCommonObject(selectedObjects);
        if (commonObject) eventsPropertiesPanel.fillEvents(commonObject.properties.events, commonObject);
    }

    eventsPropertiesPanel.fillEvents = function (eventValues, commonObject) {
        if (!eventValues) return;
        if (!this.properties) this.properties = {};
        if (!this.groups) this.groups = {};

        var dashboardsPresent = jsObject.options.report && jsObject.options.report.dashboardsPresent();
        var pagesPresent = jsObject.options.report && jsObject.options.report.pagesPresent();

        var eventGroups = [
            ["ButtonEvents", jsObject.loc.PropertyCategory.ButtonCategory, ["CheckedChangedEvent"]],
            ["ExportEvents", jsObject.loc.PropertyCategory.ExportEventsCategory, ["ExportedEvent", "ExportingEvent"]],
            ["MouseEvents", jsObject.loc.PropertyCategory.MouseEventsCategory, ["ClickEvent", "DoubleClickEvent", "MouseEnterEvent", "MouseLeaveEvent"]],
            ["NavigationEvents", jsObject.loc.PropertyCategory.NavigationEventsCategory, ["GetBookmarkEvent", "GetDrillDownReportEvent", "GetHyperlinkEvent"]],
            ["PrintEvents", jsObject.loc.PropertyCategory.PrintEventsCategory, ["AfterPrintEvent", "BeforePrintEvent", "PrintedEvent", "PrintingEvent"]],
            ["RenderEvents", jsObject.loc.PropertyCategory.RenderEventsCategory, ["BeginRenderEvent", "ColumnBeginRenderEvent", "ColumnEndRenderEvent", "EndRenderEvent", "RenderingEvent", "RefreshingEvent"]],
            ["ValueEvents", jsObject.loc.PropertyCategory.ValueEventsCategory, ["GetExcelValueEvent", "GetCollapsedEvent", "GetTagEvent", "GetToolTipEvent", "GetValueEvent", "GetDataUrlEvent",
                "GetImageDataEvent", "GetImageURLEvent", "GetBarCodeEvent", "GetCheckedEvent", "FillParametersEvent", "GetZipCodeEvent", "GetSummaryExpressionEvent", "ProcessChartEvent"]]
        ]

        for (var i = 0; i < eventGroups.length; i++) {
            var group = this.groups[eventGroups[i][0]];

            if (!group) {
                group = jsObject.PropertiesGroup(eventGroups[i][0], eventGroups[i][1]);
                this.groups[eventGroups[i][0]] = group;
                this.appendChild(group);
            }

            if (group.name == "MouseEvents") {
                group.headerButton.caption.innerText = commonObject.typeComponent == "StiButtonElement" ? jsObject.loc.FormDictionaryDesigner.Actions : jsObject.loc.PropertyCategory.MouseEventsCategory;
            }

            group.style.display = "none";
            var eventNames = eventGroups[i][2];
            var propsCount = 0;

            for (var k = 0; k < eventNames.length; k++) {
                var property = this.properties[eventNames[k]];

                if (!property) {
                    var propertyControl = jsObject.PropertyEventExpressionControl("eventProperty" + eventNames[k], jsObject.options.propertyControlWidth);
                    propertyControl.textBox.useHiddenValue = true;
                    propertyControl.eventName = eventNames[k];
                    property = jsObject.Property(null, jsObject.loc.PropertyEvents[eventNames[k]] || eventNames[k], propertyControl);
                    property.name = eventNames[k];
                    property.updateCaption();
                    this.properties[eventNames[k]] = property;
                    group.container.appendChild(property);

                    propertyControl.action = function () {
                        var selectedObjects = jsObject.options.selectedObject ? [jsObject.options.selectedObject] : jsObject.options.selectedObjects;
                        var value = StiBase64.encode(this.textBox.hiddenValue || this.textBox.value);
                        for (var i = 0; i < selectedObjects.length; i++) {
                            selectedObjects[i].properties.events[this.eventName] = value;
                        }
                        jsObject.SendCommandSetEventValue(eventsPropertiesPanel.selectedComponentParams(), this.eventName, value);
                    }
                }

                property.style.display = "none";

                if (eventValues[eventNames[k]] != null) {
                    property.style.display = "";
                    propsCount++;
                    var value = StiBase64.decode(eventValues[eventNames[k]]);
                    var hiddenValue = property.propertyControl.textBox.hiddenValue = value != "StiEmptyValue" ? value : "";
                    property.propertyControl.textBox.value = jsObject.isBlocklyValue(hiddenValue) ? "[" + jsObject.loc.PropertyMain.Blocks + "]" : hiddenValue;
                }
            }

            if (propsCount > 0) group.style.display = "";
        }

        if (!pagesPresent)
            this.groups.PrintEvents.style.display = this.properties.BeginRenderEvent.style.display = this.properties.ColumnBeginRenderEvent.style.display =
                this.properties.RenderingEvent.style.display = this.properties.ColumnEndRenderEvent.style.display = this.properties.EndRenderEvent.style.display = "none";

        if (!dashboardsPresent)
            this.properties.RefreshingEvent.style.display = "none";
    }

    eventsPropertiesPanel.updatePropertiesCaptions = function () {
        for (var propertyName in this.properties) {
            this.properties[propertyName].updateCaption();
        }
    }

    return eventsPropertiesPanel;
}