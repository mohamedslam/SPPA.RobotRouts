
StiMobileDesigner.prototype.InitializeEditOnlineMapElementForm_ = function () {
    var form = this.DashboardBaseForm("editOnlineMapElementForm", this.loc.Components.StiMap, 1, this.HelpLinks["onlineMapElement"]);
    form.isDockableToComponent = true;
    form.container.style.borderTop = "0px";
    form.caption.style.padding = "0px 10px 0 12px";
    form.hideButtonsPanel();
    var jsObject = this;

    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.minWidth = "327px";
    form.container.appendChild(controlsTable);
    form.container.style.padding = "0 0 6px 0";
    form.showMore = false;

    var expressionControl = this.ExpressionControlWithMenu(null, 325, null, null);
    var onlineMapExpressionMenu = this.options.menus.onlineMapExpressionMenu || this.InitializeDataColumnExpressionMenu("onlineMapExpressionMenu", expressionControl, form);
    onlineMapExpressionMenu.parentButton = expressionControl.button;
    expressionControl.menu = onlineMapExpressionMenu;
    expressionControl.textBox.setAttribute("placeholder", jsObject.loc.PropertyMain.Field);

    expressionControl.action = function () {
        if (this.currentContainer) {
            form.applyExpressionPropertyToOnlineMapElement(this.currentContainer, this.textBox.value);
        }
    }

    //OnlineDataColumns
    var dataColumns = [
        ["location", this.loc.PropertyMain.Location],
        ["value", this.loc.PropertyMain.Value],
        ["argument", this.loc.PropertyMain.Argument],
        ["color", this.loc.Buttons.MoreOptions],
        ["latitude", this.loc.PropertyMain.Latitude],
        ["longitude", this.loc.PropertyMain.Longitude]
    ];

    for (var i = 0; i < dataColumns.length; i++) {
        var container = this.DashboardDataColumnContainer(form, dataColumns[i][0], dataColumns[i][1], null, null, onlineMapExpressionMenu);
        container.allowSelected = true;
        container.maxWidth = 315;
        form.addControlRow(controlsTable, null, dataColumns[i][0] + "DataColumn", container, "0px 12px " + (i == dataColumns.length - 1 ? "12px" : "0px") + " 12px");

        container.action = function (actionName) {
            if (actionName == "rename" && this.dataColumnObject) {
                form.sendCommand({
                    command: "RenameMeter",
                    containerName: form.jsObject.UpperFirstChar(this.name),
                    newLabel: this.dataColumnObject.label
                },
                    function (answer) {
                        form.updateControls(answer.elementProperties);
                        form.updateIframeContent(answer.elementProperties.iframeContent);
                    }
                );
                return;
            }
            if (!this.dataColumnObject) {
                form.controls.expression.currentContainer = null;
                form.controls.expression.setEnabled(false);
            }
            form.applyDataColumnPropertyToOnlineMapElement(this);
        }

        container.onSelected = function () {
            for (var i = 0; i < dataColumns.length; i++) {
                if (form.controls[dataColumns[i][0] + "DataColumn"] != this)
                    form.controls[dataColumns[i][0] + "DataColumn"].setSelected(false);
            }

            if (this.dataColumnObject && this.dataColumnObject.expression != null) {
                form.controls.expression.currentContainer = this;
                form.controls.expression.setEnabled(true);
                form.controls.expression.textBox.value = StiBase64.decode(this.dataColumnObject.expression);
            }
        }

        if (i == 3) {
            //separator
            var separator = this.SeparatorOr();
            form.addControlRow(controlsTable, null, "onlineMapSeparator", separator, "16px 12px 0px 12px").style.textAlign = "center";
        }
    }

    form.addControlRow(controlsTable, this.loc.PropertyMain.Expression, "expressionControlCaption", null, null, "12px 12px 6px 12px");
    form.addControlRow(controlsTable, null, "expression", expressionControl, "6px 12px 8px 12px");

    var valueViewModeNames = [
        { caption: form.jsObject.loc.Chart.Bubble, name: "Bubble", key: "Bubble" },
        { caption: form.jsObject.loc.PropertyMain.Value, name: "Value", key: "Value" },
        { caption: form.jsObject.loc.PropertyMain.Icon, name: "Icon", key: "Icon" },
        { caption: form.jsObject.loc.PropertyCategory.ChartCategory, name: "Chart", key: "Chart" }];

    var valueViewMode = this.DropDownList("valueViewMode", 200, null, valueViewModeNames, true);
    form.addControlRow(controlsTable, this.loc.FormViewer.ViewMode, "valueViewMode", valueViewMode, "8px 12px 8px 12px", null, true, true);

    var customIcon = this.ImageControl(null, 200, 62);
    customIcon.style.borderStyle = "solid";
    form.addControlRow(controlsTable, "", "customIcon", customIcon, "8px 12px 8px 12px", null, true, true);

    var iconControl = this.IconControl("onlineMapIcon", 70, null, null, customIcon);
    var iconColor = this.ColorControl(null, null, null, 100, true);
    iconColor.style.marginLeft = "24px";
    var iconTable = this.CreateHTMLTable();
    iconTable.addCellInLastRow(iconControl);
    iconTable.addCellInLastRow(iconColor);
    iconTable.icon = iconControl;
    iconTable.iconColor = iconColor;
    form.addControlRow(controlsTable, this.loc.PropertyMain.Icon, "iconTable", iconTable, "8px 12px 8px 12px", null, true, true);

    var locationTypeNames = [{ caption: form.jsObject.loc.PropertyEnum.StiOnlineMapLocationTypeAuto, name: "Auto", key: "Auto" },
    { caption: form.jsObject.loc.PropertyEnum.StiOnlineMapLocationTypeAdminDivision1, name: "AdminDivision1", key: "AdminDivision1" },
    { caption: form.jsObject.loc.PropertyEnum.StiOnlineMapLocationTypeAdminDivision2, name: "AdminDivision2", key: "AdminDivision2" },
    { caption: form.jsObject.loc.PropertyEnum.StiOnlineMapLocationTypeCountryRegion, name: "CountryRegion", key: "CountryRegion" },
    { caption: form.jsObject.loc.PropertyEnum.StiOnlineMapLocationTypeNeighborhood, name: "Neighborhood", key: "Neighborhood" },
    { caption: form.jsObject.loc.PropertyEnum.StiOnlineMapLocationTypePopulatedPlace, name: "PopulatedPlace", key: "PopulatedPlace" },
    { caption: form.jsObject.loc.PropertyEnum.StiOnlineMapLocationTypePostcode1, name: "Postcode1", key: "Postcode1" },
    { caption: form.jsObject.loc.PropertyEnum.StiOnlineMapLocationTypePostcode2, name: "Postcode2", key: "Postcode2" },
    { caption: form.jsObject.loc.PropertyEnum.StiOnlineMapLocationTypePostcode3, name: "Postcode3", key: "Postcode3" },
    { caption: form.jsObject.loc.PropertyEnum.StiOnlineMapLocationTypePostcode4, name: "Postcode4", key: "Postcode4" }
    ];

    var locationType = this.DropDownList("locationType", 200, null, locationTypeNames, true);
    form.addControlRow(controlsTable, this.loc.Export.Type.replace(":", ""), "locationType", locationType, "8px 12px 8px 12px", null, true, true);

    var culture = this.DropDownList("locationCulture", 200, null, [], true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Culture, "culture", culture, "8px 12px 8px 12px", null, true, true);

    var locationColorTypes = [{ caption: form.jsObject.loc.PropertyEnum.BorderStyleFixedSingle, name: "Single", key: "Single" },
    { caption: form.jsObject.loc.PropertyMain.ColorEach, name: "ColorEach", key: "ColorEach" },
    { caption: form.jsObject.loc.PropertyMain.Group, name: "Group", key: "Group" },
    { caption: form.jsObject.loc.PropertyMain.Value, name: "Value", key: "Value" }
    ];

    var locationColorType = this.DropDownList("locationColorType", 200, null, locationColorTypes, true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Color, "locationColorType", locationColorType, "8px 12px 8px 12px", null, true, true);

    var locationColor = this.ColorControl(null, null, null, 120, true);
    locationColor.style.display = "inline-block";
    form.addControlRow(controlsTable, " ", "locationColor", locationColor, "8px 92px 8px 12px", null, null, true);

    var showMoreButton = this.FormButton(null, null, this.loc.Buttons.LessOptions);
    showMoreButton.style.marginRight = "12px";

    form.addControlRow(controlsTable, " ", "showMoreButton", showMoreButton, "8px 12px 8px 95px");

    form.controls.locationColorType.action = function () {
        form.updateProps();
    }
    form.controls.locationType.action = function () {
        form.updateProps();
    }
    form.controls.culture.action = function () {
        form.updateProps();
    }
    form.controls.locationColor.action = function () {
        form.updateProps();
    }
    form.controls.valueViewMode.action = function () {
        form.updateProps();
    }
    form.controls.showMoreButton.action = function () {
        form.showMore = !form.showMore;
        form.updateProps();
    }

    customIcon.action = function () {
        form.updateProps();
    }

    iconControl.action = function () {
        form.updateProps();
    }

    iconColor.action = function () {
        form.updateProps();
    }

    var parentLocationContainer = form.controls.locationDataColumn.innerContainer.parentElement;

    form.setValues = function () {
        form.isUpdating = true;
        expressionControl.textBox.value = "";
        expressionControl.setEnabled(false);

        var meters = this.onlineMapProperties.meters;

        var locPresent = false;
        var llPresent = false;
        for (var i = 0; i < dataColumns.length; i++) {
            var meter = meters[dataColumns[i][0]];
            var container = this.controls[dataColumns[i][0] + "DataColumn"];

            if (meter) {
                container.addColumn(meter.label, meter);
                if (i == 0) locPresent = true;
                if (i == 4 || i == 5) llPresent = true;
                if (container.isSelected && container.item)
                    container.item.action();
            }
            else
                container.clear();
        }

        var cultures = [];
        var cs = form.currentOnlineMapElement.properties.cultures || this.onlineMapProperties.cultures;
        for (var i in cs)
            cultures.push({ caption: cs[i].replace("_", "-"), name: cs[i], key: cs[i] });
        this.controls.culture.addItems(cultures);
        if (this.onlineMapProperties.culture && this.controls.culture.menu.items[this.onlineMapProperties.culture])
            this.controls.culture.menu.items[this.onlineMapProperties.culture].action();

        if (this.onlineMapProperties.locationType && this.controls.locationType.menu.items[this.onlineMapProperties.locationType])
            this.controls.locationType.menu.items[this.onlineMapProperties.locationType].action();

        if (this.onlineMapProperties.valueViewMode && this.controls.valueViewMode.menu.items[this.onlineMapProperties.valueViewMode])
            this.controls.valueViewMode.menu.items[this.onlineMapProperties.valueViewMode].action();

        if (this.onlineMapProperties.locationColorType && this.controls.locationColorType.menu.items[this.onlineMapProperties.locationColorType])
            this.controls.locationColorType.menu.items[this.onlineMapProperties.locationColorType].action();

        if (this.onlineMapProperties.locationColor)
            this.controls.locationColor.setKey(this.onlineMapProperties.locationColor);

        if (this.onlineMapProperties.iconColor) {
            this.controls.iconTable.iconColor.setKey(this.onlineMapProperties.iconColor);
            this.controls.iconTable.icon.textBox.style.color = this.onlineMapProperties.iconColor == "transparent" ? "transparent" : "rgb(" + this.onlineMapProperties.iconColor + ")";
        }

        if (this.onlineMapProperties.icon)
            this.controls.iconTable.icon.setKey(this.onlineMapProperties.icon);

        if (this.onlineMapProperties.customIcon)
            this.controls.customIcon.setImage(this.onlineMapProperties.customIcon);

        if (locPresent) {
            this.controls.latitudeDataColumnRow.style.display = "none";
            this.controls.longitudeDataColumnRow.style.display = "none";
            this.controls.onlineMapSeparatorRow.style.display = "none";

            this.controls.showMoreButtonRow.style.display = "";
            this.controls.expressionRow.style.display = "";
            this.controls.expressionControlCaptionRow.style.display = "";
            this.controls.locationDataColumnRow.style.display = "";
            this.controls.valueDataColumnRow.style.display = "";

            this.controls.cultureRow.style.display = "";
            this.controls.locationColorTypeRow.style.display = "";
            this.controls.locationTypeRow.style.display = "";

            this.controls.valueViewModeRow.style.display = "";
            this.controls.customIconRow.style.display = this.onlineMapProperties.valueViewMode == "Icon" && (this.onlineMapProperties.customIcon != null && this.onlineMapProperties.customIcon.length > 0) ? "" : "none";
            this.controls.iconTableRow.style.display = this.onlineMapProperties.valueViewMode == "Icon" && (this.onlineMapProperties.customIcon == null || this.onlineMapProperties.customIcon.length == 0) ? "" : "none";
            this.controls.argumentDataColumnRow.style.display = this.onlineMapProperties.valueViewMode == "Chart" ? "" : "none";

            var cType = this.controls.locationColorType.key;
            if (cType == "Single") {
                this.controls.locationColorRow.style.display = "";
                this.controls.colorDataColumnRow.style.display = "none";
            } else if (cType == "ColorEach") {
                this.controls.locationColorRow.style.display = "none";
                this.controls.colorDataColumnRow.style.display = "none";
            } else if (cType == "Group") {
                this.controls.locationColorRow.style.display = "none";
                this.controls.colorDataColumnRow.style.display = "";
                this.controls.colorDataColumn.headerCell.innerHTML = this.jsObject.loc.PropertyMain.Color + " " + this.jsObject.loc.PropertyMain.Group;
            } else if (cType == "Value") {
                this.controls.locationColorRow.style.display = "none";
                this.controls.colorDataColumnRow.style.display = "";
                this.controls.colorDataColumn.headerCell.innerHTML = this.jsObject.loc.PropertyMain.Color + " " + this.jsObject.loc.PropertyMain.Value;
            }

            showMoreButton.caption.innerHTML = form.showMore ? this.jsObject.loc.Buttons.LessOptions : this.jsObject.loc.Buttons.MoreOptions;
            if (!form.showMore) {
                this.controls.locationColorRow.style.display = "none";
                this.controls.colorDataColumnRow.style.display = "none";
                this.controls.cultureRow.style.display = "none";
                this.controls.locationColorTypeRow.style.display = "none";
                this.controls.locationTypeRow.style.display = "none";
                this.controls.valueViewModeRow.style.display = "none";
                this.controls.customIconRow.style.display = "none";
                this.controls.iconTableRow.style.display = "none";
                this.controls.argumentDataColumnRow.style.display = "none";
            }
        }

        if (llPresent) {
            this.controls.onlineMapSeparatorRow.style.display = "none";

            this.controls.latitudeDataColumnRow.style.display = "";
            this.controls.longitudeDataColumnRow.style.display = "";
            this.controls.expressionRow.style.display = "";
            this.controls.expressionControlCaptionRow.style.display = "";
            this.controls.customIconRow.style.display = (this.onlineMapProperties.customIcon != null && this.onlineMapProperties.customIcon.length > 0) ? "" : "none";
            this.controls.iconTableRow.style.display = (this.onlineMapProperties.customIcon == null || this.onlineMapProperties.customIcon.length == 0) ? "" : "none";

            this.controls.showMoreButtonRow.style.display = "none";
            this.controls.locationDataColumnRow.style.display = "none";
            this.controls.valueDataColumnRow.style.display = "none";
            this.controls.colorDataColumnRow.style.display = "none";

            this.controls.valueViewModeRow.style.display = "none";
            this.controls.cultureRow.style.display = "none";
            this.controls.locationColorTypeRow.style.display = "none";
            this.controls.locationTypeRow.style.display = "none";
            this.controls.locationColorRow.style.display = "none";
            this.controls.valueViewModeRow.style.display = "none";
            this.controls.argumentDataColumnRow.style.display = "none";
        }

        if (!locPresent && !llPresent) {
            this.controls.onlineMapSeparatorRow.style.display = "";

            this.controls.latitudeDataColumnRow.style.display = "";
            this.controls.longitudeDataColumnRow.style.display = "";
            this.controls.expressionRow.style.display = "none";
            this.controls.expressionControlCaptionRow.style.display = "none";

            this.controls.showMoreButtonRow.style.display = "none";
            this.controls.locationDataColumnRow.style.display = "";
            this.controls.valueDataColumnRow.style.display = "none";
            this.controls.colorDataColumnRow.style.display = "none";

            this.controls.valueViewModeRow.style.display = "none";
            this.controls.cultureRow.style.display = "none";
            this.controls.locationColorTypeRow.style.display = "none";
            this.controls.locationTypeRow.style.display = "none";
            this.controls.locationColorRow.style.display = "none";
            this.controls.valueViewModeRow.style.display = "none";
            this.controls.customIconRow.style.display = "none";
            this.controls.iconTableRow.style.display = "none";
            this.controls.argumentDataColumnRow.style.display = "none";
        }

        form.isUpdating = false;
    }

    form.updateProps = function () {
        if (!form.isUpdating) {
            form.sendCommand(
                {
                    command: "UpdateOnlineMapElementProperties",
                    locationType: this.controls.locationType.key,
                    culture: this.controls.culture.key,
                    locationColor: this.controls.locationColor.key,
                    locationColorType: this.controls.locationColorType.key,
                    valueViewMode: this.controls.valueViewMode.key,
                    icon: this.controls.iconTable.icon.key,
                    iconColor: this.controls.iconTable.iconColor.key,
                    customIcon: this.controls.customIcon.src
                },
                function (answer) {
                    form.updateControls(answer.elementProperties);
                    form.updateIframeContent(answer.elementProperties.iframeContent);
                }
            );
        }
    }

    form.updateSvgContent = function () {
        form.updateProps();
    }

    form.onshow = function () {
        form.currentPanelName = jsObject.options.propertiesPanel.getCurrentPanelName();
        if (jsObject.options.showDictionary) jsObject.options.propertiesPanel.showContainer("Dictionary");
        expressionControl.textBox.value = "";
        expressionControl.setEnabled(false);

        for (var i = 0; i < dataColumns.length; i++) {
            var container = this.controls[dataColumns[i][0] + "DataColumn"];
            container.clear();
        }

        form.sendCommand({ command: "GetOnlineMapElementProperties" },
            function (answer) {
                var map = answer.elementProperties;
                if (map.locationColor != "50,205,50" || map.valueViewMode != "Bubble" ||
                    map.locationType != "Auto" || map.culture != "en_US" ||
                    map.locationColorType != "Single" || map.meters.color)
                    form.showMore = true
                else form.showMore = false;

                form.updateControls(answer.elementProperties);
                form.correctTopPosition();
            }
        );
    }

    form.onhide = function () {
        jsObject.options.propertiesPanel.showContainer(form.currentPanelName);
    }

    form.updateControls = function (onlineMapProperties) {
        form.onlineMapProperties = onlineMapProperties;
        form.setValues();
    }

    form.applyDataColumnPropertyToOnlineMapElement = function (container) {
        form.sendCommand(
            {
                command: "SetDataColumn",
                containerName: jsObject.UpperFirstChar(container.name),
                dataColumnObject: container.dataColumnObject
            },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.updateIframeContent(answer.elementProperties.iframeContent);
                if (container.item) container.item.action();
            }
        );
    }

    form.applyExpressionPropertyToOnlineMapElement = function (container, expressionValue) {
        if (container) {
            form.sendCommand(
                {
                    command: "SetExpression",
                    containerName: jsObject.UpperFirstChar(container.name),
                    expressionValue: StiBase64.encode(expressionValue)
                },
                function (answer) {
                    form.updateControls(answer.elementProperties);
                    form.updateIframeContent(answer.elementProperties.iframeContent);
                }
            );
        }
    }

    form.sendCommand = function (updateParameters, callbackFunction) {
        jsObject.SendCommandToDesignerServer("UpdateOnlineMapElement",
            {
                componentName: form.currentOnlineMapElement.properties.name,
                updateParameters: updateParameters
            },
            function (answer) {
                callbackFunction(answer);
            });
    }

    form.updateIframeContent = function (iframeContent) {
        this.currentOnlineMapElement.properties.iframeContent = iframeContent;
        this.currentOnlineMapElement.repaint();
    }

    return form;
}


