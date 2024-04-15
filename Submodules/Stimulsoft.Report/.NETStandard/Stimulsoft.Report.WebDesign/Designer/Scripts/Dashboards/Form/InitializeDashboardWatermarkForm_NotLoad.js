
StiMobileDesigner.prototype.InitializeDashboardWatermarkForm_ = function () {
    var jsObject = this;
    var form = this.DashboardBaseForm("dashboardWatermark", this.loc.PropertyMain.Watermark, 1);
    form.container.style.borderTop = "0px";
    form.caption.style.padding = "0px 10px 0 12px";
    form.hideButtonsPanel()
    form.controls = {};

    //Main Table
    var mainTable = this.CreateHTMLTable();
    mainTable.className = "stiDesignerImageFormMainPanel";
    form.container.appendChild(mainTable);
    form.container.style.padding = "0px";

    //Buttons
    var buttonProps = [
        ["text", "DashboardWatermarkForm.DashboardText.png", this.loc.PropertyMain.Text],
        ["image", "DashboardWatermarkForm.DashboardImage.png", this.loc.PropertyMain.Image],
        ["weave", "DashboardWatermarkForm.DashboardWeave.png", this.loc.PropertyHatchStyle.Weave]
    ];

    //Add Panels && Buttons
    var buttonsPanel = mainTable.addCell();
    var panelsContainer = mainTable.addCell();
    buttonsPanel.style.verticalAlign = "top";
    buttonsPanel.style.paddingTop = "6px";
    form.mainButtons = {};
    form.panels = {};

    for (var i = 0; i < buttonProps.length; i++) {
        var panel = document.createElement("Div");
        panel.className = "stiDesignerDbsWatermarkFormPanel";
        panel.style.display = i != 0 ? "none" : "";
        panelsContainer.appendChild(panel);
        form.panels[buttonProps[i][0]] = panel;

        var button = this.FormTabPanelButton(null, buttonProps[i][2], buttonProps[i][1], buttonProps[i][2], null, { width: 24, height: 24 }, 34);

        form.mainButtons[buttonProps[i][0]] = button;
        buttonsPanel.appendChild(button);
        button.panelName = buttonProps[i][0];

        button.action = function () {
            form.setMode(this.panelName);
        }

        //add marker
        var marker = document.createElement("div");
        marker.style.display = "none";
        marker.className = "stiUsingMarker";
        var markerInner = document.createElement("div");
        marker.appendChild(markerInner);
        button.style.position = "relative";
        button.appendChild(marker);
        button.marker = marker;
    }

    var textTable = this.CreateHTMLTable();
    var imageTable = this.CreateHTMLTable();
    var weaveTable = this.CreateHTMLTable();

    textTable.style.width = imageTable.style.width = weaveTable.style.width = "100%";

    form.panels.text.appendChild(textTable);
    form.panels.image.appendChild(imageTable);
    form.panels.weave.appendChild(weaveTable);

    var controlProps = [
        ["textEnabled", " ", this.CheckBox(null, this.loc.PropertyMain.Enabled), textTable, "18px 6px 6px 6px"],
        ["text", this.loc.PropertyMain.Text, this.TextAreaWithEditButton(null, 250, 45, false, true), textTable, "6px"],
        ["textFont", this.loc.PropertyMain.Font, this.PropertyFontControl("dbsWatermarkFont", null, true), textTable, "6px"],
        ["textColor", this.loc.PropertyMain.TextColor, this.PropertyColorControl(null, null, 150), textTable, "6px"],
        ["textAngle", this.loc.PropertyMain.Angle, this.TextBoxEnumerator(null, 150, null, false, 360, 0), textTable, "6px"],

        ["imageEnabled", " ", this.CheckBox(null, this.loc.PropertyMain.Enabled), imageTable, "18px 6px 6px 6px"],
        ["image", null, this.ImageControl(null, 425, 80), imageTable, "6px 0 6px 12px"],
        ["imageAlignment", this.loc.PropertyMain.ImageAlignment, this.ContentAlignmentControl(), imageTable, "6px"],
        ["imageTransparency", this.loc.PropertyMain.ImageTransparency, this.SladerControl(null, 150, 0, 255, true), imageTable, "6px"],
        ["imageMultipleFactor", this.loc.PropertyMain.ImageMultipleFactor, this.SladerControl(null, 150, 1, 50, true), imageTable, "6px"],
        ["imageAspectRatio", " ", this.CheckBox(null, this.loc.PropertyMain.AspectRatio), imageTable, "6px"],
        ["imageStretch", " ", this.CheckBox(null, this.loc.PropertyMain.ImageStretch), imageTable, "6px"],
        ["imageTiling", " ", this.CheckBox(null, this.loc.PropertyMain.ImageTiling), imageTable, "6px"],

        ["weaveEnabled", " ", this.CheckBox(null, this.loc.PropertyMain.Enabled), weaveTable, "18px 6px 6px 6px"],
        ["majorGroup", null, this.FormBlockHeader(this.loc.ChartRibbon.AxesTicksMajor), weaveTable, "6px"],
        ["weaveMajorIcon", this.loc.PropertyMain.Icon, this.IconControl("dbsWMajorIcon", 120, null, null, null, true), weaveTable, "6px"],
        ["weaveMajorColor", this.loc.PropertyMain.Color, this.PropertyColorControl(null, null, 140), weaveTable, "6px"],
        ["weaveMajorSize", this.loc.PropertyMain.Size, this.SladerControl(null, 120, 5, 30, true), weaveTable, "6px"],
        ["minorGroup", null, this.FormBlockHeader(this.loc.ChartRibbon.AxesTicksMinor), weaveTable, "6px"],
        ["weaveMinorIcon", this.loc.PropertyMain.Icon, this.IconControl("dbsWMinorIcon", 120, null, null, null, true), weaveTable, "6px"],
        ["weaveMinorColor", this.loc.PropertyMain.Color, this.PropertyColorControl(null, null, 140), weaveTable, "6px"],
        ["weaveMinorSize", this.loc.PropertyMain.Size, this.SladerControl(null, 120, 5, 30, true), weaveTable, "6px"],
        ["weaveSeparator1", null, this.FormSeparator(), weaveTable, "6px 0 6px 0"],
        ["weaveDistance", this.loc.PropertyMain.Distance, this.SladerControl(null, 120, 50, 200, true), weaveTable, "6px"],
        ["weaveAngle", this.loc.PropertyMain.Angle, this.TextBoxEnumerator(null, 120, null, false, 360, 0), weaveTable, "6px"]
    ];

    for (var i = 0; i < controlProps.length; i++) {
        var control = controlProps[i][2];
        control.propertyName = controlProps[i][0];
        form.addControlRow(controlProps[i][3], controlProps[i][1], control.propertyName, control, controlProps[i][4]);

        control.action = function () {
            if (this.propertyName == "textEnabled" || this.propertyName == "imageEnabled" || this.propertyName == "weaveEnabled" || this.propertyName == "weaveMajorColor" ||
                this.propertyName == "image" || this.propertyName == "weaveMajorIcon" || this.propertyName == "weaveMinorIcon" || this.propertyName == "weaveMinorColor") {
                form.updateControlsStates();
                form.updateMarkers();
            }
            if (this.propertyName == "imageTiling" && this.isChecked) {
                form.controls.imageAspectRatio.setChecked(false);
                form.controls.imageStretch.setChecked(false);
            }
            if ((this.propertyName == "imageAspectRatio" || this.propertyName == "imageStretch") && this.isChecked) {
                form.controls.imageTiling.setChecked(false);
            }
            if (form.currentObject) {
                form.currentObject.properties.dashboardWatermark = form.getControlsValues();
                jsObject.SendCommandSendProperties(form.currentObject, ["dashboardWatermark"]);
            }
        }
    }

    form.controls.weaveMajorColorRow.style.display = form.controls.weaveMinorColorRow.style.display = "none";
    form.controls.weaveMajorColor.style.display = form.controls.weaveMinorColor.style.display = form.controls.weaveMajorIcon.style.display = form.controls.weaveMinorIcon.style.display = "inline-block";
    form.controls.weaveMajorColor.style.marginLeft = form.controls.weaveMinorColor.style.marginLeft = "12px";
    form.controls.weaveMajorIcon.parentElement.appendChild(form.controls.weaveMajorColor);
    form.controls.weaveMinorIcon.parentElement.appendChild(form.controls.weaveMinorColor);

    form.updateControlsStates = function () {
        var cs = form.controls;

        var textE = cs.textEnabled.isChecked;
        cs.text.setEnabled(textE);
        cs.textFont.setEnabled(textE);
        cs.textColor.setEnabled(textE);
        cs.textAngle.setEnabled(textE);

        var imageE = cs.imageEnabled.isChecked;
        cs.image.setEnabled(imageE);
        cs.imageAlignment.setEnabled(imageE && cs.image.src);
        cs.imageTransparency.setEnabled(imageE && cs.image.src);
        cs.imageMultipleFactor.setEnabled(imageE && cs.image.src);
        cs.imageAspectRatio.setEnabled(imageE && cs.image.src);
        cs.imageStretch.setEnabled(imageE && cs.image.src);
        cs.imageTiling.setEnabled(imageE && cs.image.src);

        var weaveE = cs.weaveEnabled.isChecked;
        cs.weaveMajorIcon.setEnabled(weaveE);
        cs.weaveMinorIcon.setEnabled(weaveE);
        cs.weaveMajorColor.setEnabled(weaveE && cs.weaveMajorIcon.key);
        cs.weaveMajorSize.setEnabled(weaveE && cs.weaveMajorIcon.key);
        cs.weaveMinorColor.setEnabled(weaveE && cs.weaveMinorIcon.key);
        cs.weaveMinorSize.setEnabled(weaveE && cs.weaveMinorIcon.key);
        cs.weaveDistance.setEnabled(weaveE && (cs.weaveMinorIcon.key || cs.weaveMajorIcon.key));
        cs.weaveAngle.setEnabled(weaveE && (cs.weaveMinorIcon.key || cs.weaveMajorIcon.key));

        cs.weaveMajorIcon.textBox.style.color = jsObject.GetHTMLColor(cs.weaveMajorColor.key);
        cs.weaveMinorIcon.textBox.style.color = jsObject.GetHTMLColor(cs.weaveMinorColor.key);
    }

    form.fillControls = function () {
        for (var propertyName in form.watermarkProps) {
            var control = form.controls[propertyName];
            if (control) {
                var value = form.watermarkProps[propertyName];
                if (propertyName == "text") value = StiBase64.decode(value);
                if (propertyName == "imageMultipleFactor") value = parseInt(jsObject.StrToDouble(value) * 10);
                jsObject.SetControlValue(control, value);
            }
        }
    }

    form.getControlsValues = function () {
        var values = {};
        for (var propertyName in form.watermarkProps) {
            var control = form.controls[propertyName];
            if (control) {
                var value = jsObject.GetControlValue(control);
                if (propertyName == "text") value = StiBase64.encode(value);
                if (propertyName == "imageMultipleFactor") value = (value / 10).toString();
                values[propertyName] = value;
            }
        }
        return values;
    }

    form.setMode = function (mode) {
        form.mode = mode;
        for (var panelName in form.panels) {
            form.panels[panelName].style.display = mode == panelName ? "" : "none";
            form.mainButtons[panelName].setSelected(mode == panelName);
        }
    }

    form.updateMarkers = function () {
        this.mainButtons.text.marker.style.display = this.controls.textEnabled.isChecked ? "" : "none";
        this.mainButtons.image.marker.style.display = this.controls.imageEnabled.isChecked ? "" : "none";
        this.mainButtons.weave.marker.style.display = this.controls.weaveEnabled.isChecked ? "" : "none";
    }

    form.show = function () {
        var currentPage = jsObject.options.currentPage;
        var selectedObject = jsObject.options.selectedObject || (jsObject.options.selectedObjects && jsObject.options.selectedObjects.length > 0 ? jsObject.options.selectedObjects[0] : null);
        var watermarkOwner = selectedObject && selectedObject.properties.dashboardWatermark ? selectedObject : (currentPage && currentPage.isDashboard ? currentPage : null);

        if (watermarkOwner) {
            form.currentObject = watermarkOwner;
            form.watermarkProps = watermarkOwner.properties.dashboardWatermark;
            form.fillControls();
            form.updateControlsStates();
            form.updateMarkers();
            form.changeVisibleState(true);
            var currentPanel = null;
            var cs = form.controls;
            if (cs.weaveEnabled.isChecked) currentPanel = "weave";
            if (cs.imageEnabled.isChecked) currentPanel = "image";
            if (cs.textEnabled.isChecked || !currentPanel) currentPanel = "text";
            form.setMode(currentPanel);
        }
    }

    return form
}