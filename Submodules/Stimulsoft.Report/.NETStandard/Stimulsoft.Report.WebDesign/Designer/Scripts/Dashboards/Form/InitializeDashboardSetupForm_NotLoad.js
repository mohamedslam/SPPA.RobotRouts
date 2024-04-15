
StiMobileDesigner.prototype.InitializeDashboardSetupForm_ = function () {
    var form = this.DashboardBaseForm("dashboardSetup", this.loc.Components.StiDashboard, 1);
    form.container.style.borderTop = "0px";
    form.caption.style.padding = "0px 10px 0 12px";
    form.hideButtonsPanel();
    var jsObject = this;

    var controlsTable = this.CreateHTMLTable();
    form.container.appendChild(controlsTable);
    form.container.style.padding = "0 0 6px 0";

    //Width
    var widthTable = this.CreateHTMLTable();
    var widthControl = this.TextBox(null, 100);
    var horSpacingButton = this.FormImageButton(null, "Layout.MakeHorizontalSpacingEqual.png", this.loc.Toolbars.MakeHorizontalSpacingEqual);
    horSpacingButton.style.marginLeft = "6px";
    horSpacingButton.style.width = horSpacingButton.style.height = this.options.isTouchDevice ? "26px" : "21px";
    widthTable.addCell(widthControl);
    widthTable.addCell(horSpacingButton);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Width, "width", widthTable, "6px 30px 6px 12px");

    horSpacingButton.action = function () {
        form.sendCommandToDashboard("MakeHorSpacingEqual", function (answer) {
            widthControl.focus();
            form.updateDashboard(answer);
        });
    }

    widthControl.action = function () {
        var value = Math.abs(this.jsObject.StrToDouble(this.value));
        if (value < 50) value = 50;
        if (value > 10000) value = 10000;
        this.value = value;
        form.sendCommandToDashboard("ChangeWidth", function (answer) {
            form.updateDashboard(answer);
        });
    }

    //Height
    var heightTable = this.CreateHTMLTable();
    var heightControl = this.TextBox(null, 100);
    var vertSpacingButton = this.FormImageButton(null, "Layout.MakeVerticalSpacingEqual.png", this.loc.Toolbars.MakeVerticalSpacingEqual);
    vertSpacingButton.style.marginLeft = "6px";
    vertSpacingButton.style.width = vertSpacingButton.style.height = this.options.isTouchDevice ? "26px" : "21px";
    heightTable.addCell(heightControl);
    heightTable.addCell(vertSpacingButton);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Height, "height", heightTable, "6px 30px 6px 12px");

    vertSpacingButton.action = function () {
        form.sendCommandToDashboard("MakeVertSpacingEqual", function (answer) {
            heightControl.focus();
            form.updateDashboard(answer);
        });
    }

    heightControl.action = function () {
        var value = Math.abs(this.jsObject.StrToDouble(this.value));
        if (value < 50) value = 50;
        if (value > 10000) value = 10000;
        this.value = value;
        form.sendCommandToDashboard("ChangeHeight", function (answer) {
            form.updateDashboard(answer);
        });
    }

    var scaleContentControl = this.CheckBox(null, this.loc.FormPageSetup.ScaleContent);
    form.addControlRow(controlsTable, " ", "scaleContent", scaleContentControl, "10px 30px 10px 12px");
    scaleContentControl.action = function () {
        this.jsObject.options.report.properties.scaleContent = this.isChecked;
        this.jsObject.SendCommandSetReportProperties(["scaleContent"]);
    }

    form.addControlRow(controlsTable, null, "separator", this.FormSeparator(), "6px 12px 6px 12px");
    var contentAlignment = this.DropDownList("contentAlignmentEditDbsForm", 170, null, this.GetDashboardContentAlignmentItems(), true, null, null, true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.ContentAlignment, "contentAlignment", contentAlignment, "6px 12px 6px 12px");

    contentAlignment.action = function () {
        form.sendCommandToDashboard("ChangeContentAlignment", function (answer) { form.updateDashboard(answer); });
    }

    form.onshow = function () {
        var currentPage = this.jsObject.options.currentPage;
        widthControl.value = currentPage.properties.unitWidth;
        heightControl.value = currentPage.properties.unitHeight;
        scaleContentControl.setChecked(this.jsObject.options.report.properties.scaleContent);
        contentAlignment.setKey(currentPage.properties.contentAlignment);
        widthControl.focus();
    }

    form.updateDashboard = function (answer) {
        if (answer.newWidth != null) {
            widthControl.value = answer.newWidth;
        }
        if (answer.newHeight != null) {

            heightControl.value = answer.newHeight;
        }
        var currentPage = jsObject.options.currentPage;
        if (currentPage && answer.properties) {
            if (answer.properties) jsObject.WriteAllProperties(currentPage, answer.properties);
            currentPage.repaint(true);
            if (answer.rebuildProps) currentPage.rebuild(answer.rebuildProps);
        }
    }

    form.getControlsValues = function () {
        return {
            width: widthControl.value,
            height: heightControl.value,
            scaleContent: scaleContentControl.isChecked,
            contentAlignment: contentAlignment.key
        }
    }

    form.sendCommandToDashboard = function (commandName, callbackFunc) {
        var params = {
            commandName: commandName,
            dashboardName: jsObject.options.currentPage.properties.name,
            width: widthControl.value,
            height: heightControl.value,
            scaleContent: scaleContentControl.isChecked,
            contentAlignment: contentAlignment.key
        }
        jsObject.SendCommandToDesignerServer("ChangeDashboardSettingsValue", params, callbackFunc);
    }

    return form
}