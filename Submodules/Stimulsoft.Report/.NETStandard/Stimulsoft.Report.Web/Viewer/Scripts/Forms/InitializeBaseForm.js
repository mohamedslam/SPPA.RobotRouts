
StiJsViewer.prototype.BaseForm = function (name, caption, level, helpUrl) {
    var form = document.createElement("div");
    form.name = name;
    form.id = this.generateKey();
    form.className = "stiJsViewerForm";
    var jsObject = form.jsObject = this;
    form.level = level;
    form.caption = null;
    form.visible = false;
    form.style.display = "none";
    form.helpUrl = helpUrl;
    if (level == null) level = 1;
    form.style.zIndex = (level * 10) + 1;
    if (name) {
        if (!this.controls.forms) this.controls.forms = {};
        if (this.controls.forms[name] != null) {
            this.controls.forms[name].changeVisibleState(false);
            this.controls.mainPanel.removeChild(this.controls.forms[name]);
        }
        this.controls.forms[name] = form;
    }
    this.controls.mainPanel.appendChild(form);

    //Header
    form.header = document.createElement("div");
    form.header.thisForm = form;
    form.appendChild(form.header);
    form.header.className = "stiJsViewerFormHeader";

    var headerTable = this.CreateHTMLTable();
    headerTable.style.width = "100%";
    form.header.appendChild(headerTable);

    form.caption = headerTable.addCell();
    if (caption != null) {
        if (caption) form.caption.innerHTML = caption;
        form.caption.style.textAlign = "left";
        form.caption.style.padding = "5px 10px 8px 15px";
    }

    //Help Button
    if (helpUrl && this.options.appearance.showDialogsHelp) {
        var buttonHelp = this.SmallButton(null, null, "Help.png");
        buttonHelp.image.style.margin = "0 2px 0 2px";
        buttonHelp.style.display = "inline-block";
        buttonHelp.action = function () { jsObject.showHelpWindow(form.helpUrl); };
        headerTable.addCell(buttonHelp).setAttribute("style", "width: 20px; text-align: right; padding: 2px 0px 1px 0px; vertical-align: top;");
    }

    form.buttonClose = this.SmallButton(null, null, "CloseForm.png");
    form.buttonClose.image.style.margin = "0 2px 0 2px";
    form.buttonClose.style.display = "inline-block";

    form.buttonClose.action = function () {
        if (form["cancelAction"]) form.cancelAction();
        form.changeVisibleState(false);
    };

    var closeButtonCell = headerTable.addCell(form.buttonClose);
    closeButtonCell.style.verticalAlign = "top";
    closeButtonCell.style.width = "30px";
    closeButtonCell.style.textAlign = "right";
    closeButtonCell.style.padding = "2px 2px 1px 1px";

    //Container
    form.container = document.createElement("div");
    form.appendChild(form.container);
    form.container.className = "stiJsViewerFormContainer";

    //Buttons
    form.buttonsPanel = document.createElement("div");
    form.appendChild(form.buttonsPanel);
    form.buttonsPanel.className = "stiJsViewerFormButtonsPanel";
    var buttonsTable = this.CreateHTMLTable();
    form.buttonsPanel.appendChild(buttonsTable);

    form.buttonOk = this.FormButton(null, this.collections.loc["ButtonOk"], null, null, "stiJsViewerFormButtonTheme");
    form.buttonOk.action = function () { form.action(); };
    buttonsTable.addCell(form.buttonOk).style.padding = "12px";

    form.buttonCancel = this.FormButton(null, this.collections.loc["ButtonCancel"]);
    form.buttonCancel.action = function () {
        if (form["cancelAction"]) form.cancelAction();
        form.changeVisibleState(false);
    };
    buttonsTable.addCell(form.buttonCancel).style.padding = "12px 12px 12px 0";

    if (jsObject.options.osWin11 && jsObject.options.standaloneJsMode) {
        form.style.borderRadius = "10px";
        form.header.style.borderRadius = "10px 10px 0 0";
        form.buttonsPanel.style.borderRadius = "0 0 10px 10px";
        form.buttonClose.style.marginRight = "5px";
    }

    form.changeVisibleState = function (state) {
        if (state) {
            this.style.display = "";
            this.onshow();
            jsObject.setObjectToCenter(this, 150);
            jsObject.controls.disabledPanels[this.level].changeVisibleState(true);
            jsObject.options.currentForm = this;
            this.visible = true;
            var d = new Date();
            var endTime = d.getTime() + jsObject.options.formAnimDuration;
            this.flag = false;
            jsObject.ShowAnimationForm(this, endTime);
        }
        else {
            clearTimeout(this.animationTimer);
            this.visible = false;
            this.style.display = "none";
            this.onhide();
            jsObject.controls.disabledPanels[this.level].changeVisibleState(false);
            jsObject.options.currentForm = null;
        }
    }

    form.action = function () { };
    form.onshow = function () { };
    form.onhide = function () { };

    form.onmousedown = function () {
        if (this.isTouchStartFlag) return;
        this.ontouchstart(true);
    }

    form.ontouchstart = function (mouseProcess) {
        var this_ = this;
        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);
        jsObject.options.formPressed = this;
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    //Mouse Events
    form.header.onmousedown = function (event) {
        if (!event || this.isTouchStartFlag) return;
        var mouseStartX = event.clientX;
        var mouseStartY = event.clientY;
        var formStartX = jsObject.FindPosX(this.thisForm, "stiJsViewerMainPanel");
        var formStartY = jsObject.FindPosY(this.thisForm, "stiJsViewerMainPanel");
        jsObject.options.formInDrag = [mouseStartX, mouseStartY, formStartX, formStartY, this.thisForm];
    }

    //Touch Events
    form.header.ontouchstart = function (event) {
        var this_ = this;
        this.isTouchStartFlag = true;
        clearTimeout(this.isTouchStartTimer);
        var fingerStartX = event.touches[0].pageX;
        var fingerStartY = event.touches[0].pageY;
        var formStartX = jsObject.FindPosX(this.thisForm, "stiJsViewerMainPanel");
        var formStartY = jsObject.FindPosY(this.thisForm, "stiJsViewerMainPanel");
        jsObject.options.formInDrag = [fingerStartX, fingerStartY, formStartX, formStartY, this.thisForm];
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    form.header.ontouchmove = function (event) {
        event.preventDefault();

        if (jsObject.options.formInDrag) {
            var formInDrag = jsObject.options.formInDrag;
            var formStartX = formInDrag[2];
            var formStartY = formInDrag[3];
            var fingerCurrentXPos = event.touches[0].pageX;
            var fingerCurrentYPos = event.touches[0].pageY;
            var deltaX = formInDrag[0] - fingerCurrentXPos;
            var deltaY = formInDrag[1] - fingerCurrentYPos;
            var newPosX = formStartX - deltaX;
            var newPosY = formStartY - deltaY;
            formInDrag[4].style.left = newPosX + "px";
            formInDrag[4].style.top = newPosY + "px";
        }
    }

    form.header.ontouchend = function () {
        event.preventDefault();
        jsObject.options.formInDrag = false;
    }

    form.move = function (evnt) {
        var leftPos = jsObject.options.formInDrag[2] + (evnt.clientX - jsObject.options.formInDrag[0]);
        var topPos = jsObject.options.formInDrag[3] + (evnt.clientY - jsObject.options.formInDrag[1]);

        this.style.left = leftPos > 0 ? leftPos + "px" : 0;
        this.style.top = topPos > 0 ? topPos + "px" : 0;
    }

    form.addControlRow = function (table, textControl, controlName, control, margin, minWidth) {
        if (!this.controls) this.controls = {};
        this.controls[controlName] = control;
        this.controls[controlName + "Row"] = table.addRow();

        if (textControl != null) {
            var text = table.addCellInLastRow();
            this.controls[controlName + "Text"] = text;
            text.innerHTML = textControl;
            text.className = "stiJsViewerCaptionControls";
            text.style.paddingLeft = "12px";
            text.style.minWidth = (minWidth || 100) + "px";
        }

        if (control) {
            control.style.margin = margin;
            var controlCell = table.addCellInLastRow(control);
            if (textControl == null) controlCell.setAttribute("colspan", 2);
        }

        return controlCell;
    }

    form.hideButtonsPanel = function () {
        form.buttonsPanel.style.display = "none";
    }

    return form;
}

//Separator
StiJsViewer.prototype.FormSeparator = function () {
    var separator = document.createElement("div");
    separator.className = "stiJsViewerFormSeparator";

    return separator;
}