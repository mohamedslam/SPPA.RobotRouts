
StiMobileDesigner.prototype.BaseForm = function (name, caption, level, helpUrl) {
    var form = document.createElement("div");
    var jsObject = form.jsObject = this;
    form.style.webkitAppRegion = "no-drag"; //for node js
    form.name = name != null ? name : this.generateKey();
    form.id = this.options.mobileDesigner.id + form.name;
    if (this.options.forms[name] != null) {
        this.options.mainPanel.removeChild(this.options.forms[name]);
    }
    if (name != null) this.options.forms[name] = form;
    this.options.mainPanel.appendChild(form);
    form.className = "stiDesignerForm";
    form.name = name;
    form.level = level;
    form.helpUrl = helpUrl;
    form.caption = null;
    form.visible = false;
    form.style.display = "none";
    form.style.zIndex = level * 10 + 1;
    this.options.forms[name] = form;

    //Header
    var header = form.header = document.createElement("div");
    header.thisForm = form;
    form.appendChild(header);
    header.className = "stiDesignerFormHeader";

    var headerTable = this.CreateHTMLTable();
    headerTable.style.width = "100%";
    header.appendChild(headerTable);

    form.caption = headerTable.addCell();
    if (caption != null || typeof (caption) == "undefined") {
        if (caption) form.caption.innerHTML = caption;
        form.caption.style.textAlign = "left";
        form.caption.style.padding = "0px 10px 0 15px";
    }

    //Help Button
    if (helpUrl && this.options.showDialogsHelp) {
        var buttonHelp = form.buttonHelp = this.StandartSmallButton(name + "HelpButton", null, null, "HelpIcon.png", null, null, null);
        buttonHelp.image.style.margin = "0 2px 0 2px";
        buttonHelp.style.display = "inline-block";
        buttonHelp.allwaysEnabled = true;
        buttonHelp.action = function () {
            jsObject.ShowHelpWindow(form.helpUrl);
        };

        var helpButtonCell = headerTable.addCell(buttonHelp);
        helpButtonCell.style.verticalAlign = "top";
        helpButtonCell.style.width = "20px";
        helpButtonCell.style.textAlign = "right";
        helpButtonCell.style.padding = "2px 0px 1px 0px";
    }

    //Close Button
    var buttonClose = form.buttonClose = this.StandartSmallButton(name + "CloseButton", null, null, "CloseForm.png", null, null, null);
    buttonClose.image.style.margin = "0 2px 0 2px";
    buttonClose.style.display = "inline-block";
    buttonClose.allwaysEnabled = true;
    buttonClose.action = function () {
        if (form["cancelAction"]) form.cancelAction();
        form.changeVisibleState(false);
    };

    var closeButtonCell = headerTable.addCell(buttonClose);
    closeButtonCell.style.verticalAlign = "top";
    closeButtonCell.style.width = "20px";
    closeButtonCell.style.textAlign = "right";
    closeButtonCell.style.padding = "2px 2px 1px 0px";

    //Container
    var container = form.container = document.createElement("div");
    form.appendChild(container);
    container.className = "stiDesignerFormContainer";

    container.clear = function () {
        while (this.childNodes[0]) {
            this.removeChild(this.childNodes[0]);
        }
    }

    //Buttons
    var buttonsPanel = form.buttonsPanel = document.createElement("div");
    form.appendChild(buttonsPanel);
    buttonsPanel.className = "stiDesignerFormButtonsPanel";

    var buttonsTable = this.CreateHTMLTable();
    buttonsPanel.appendChild(buttonsTable);

    var buttonOk = form.buttonOk = this.FormButton(form, name + "ButtonOk", this.loc.Buttons.Ok.replace("&", ""), null, null, null, null, "stiDesignerFormButtonTheme");
    buttonsTable.addCell(buttonOk).style.lineHeight = "0";
    buttonOk.style.margin = "12px";
    buttonOk.style.display = "inline-block";

    buttonOk.action = function () {
        form.action();
    };

    var buttonCancel = form.buttonCancel = this.FormButton(form, name + "ButtonCancel", this.loc.Buttons.Cancel.replace("&", ""));
    buttonCancel.style.display = "inline-block";
    buttonCancel.style.margin = "12px 12px 12px 0";
    buttonsTable.addCell(buttonCancel).style.lineHeight = "0";

    buttonCancel.action = function () {
        if (form["cancelAction"]) form.cancelAction();
        form.changeVisibleState(false);
    };

    if (jsObject.options.osWin11 && jsObject.options.standaloneJsMode) {
        form.style.borderRadius = "10px";
        header.style.borderRadius = "10px 10px 0 0";
        buttonsPanel.style.borderRadius = "0 0 10px 10px";
        buttonClose.style.marginRight = "5px";
    }

    form.changeVisibleState = function (state) {
        if (state) {
            if (jsObject.options.paintPanel.copyStyleMode)
                jsObject.options.paintPanel.setCopyStyleMode(false);

            if (this.visible) this.changeVisibleState(false);
            this.style.display = "";

            if (jsObject.options.currentForm && this.parentElement == jsObject.options.mainPanel) {
                jsObject.options.mainPanel.removeChild(this);
                jsObject.options.mainPanel.appendChild(this);
            }

            this.onshow();

            if (this.isDockableToComponent)
                this.dockToComponent();
            else
                jsObject.SetObjectToCenter(this);

            if (!this.isNotModal) {
                if (!jsObject.options.disabledPanels) jsObject.InitializeDisabledPanels();
                jsObject.options.disabledPanels[this.level].changeVisibleState(true);
            }

            this.visible = true;
            this.currentFormDownLevel = jsObject.options.currentForm && jsObject.options.currentForm.visible ? jsObject.options.currentForm : null;
            jsObject.options.currentForm = this;

            var d = new Date();
            var endTime = d.getTime() + jsObject.options.formAnimDuration;
            this.flag = false;
            jsObject.ShowAnimationForm(this, endTime);
            this.movedByUser = false;
        }
        else {
            clearTimeout(this.animationTimer);
            this.visible = false;
            jsObject.options.currentForm = this.currentFormDownLevel || null;
            this.style.display = "none";
            if (!jsObject.options.forms[this.name]) {
                jsObject.options.mainPanel.removeChild(this);
            }
            this.onhide();
            if (!this.isNotModal) {
                if (!jsObject.options.disabledPanels) jsObject.InitializeDisabledPanels();
                jsObject.options.disabledPanels[this.level].changeVisibleState(false);
            }
        }
    }

    form.action = function () { };
    form.onshow = function () { };
    form.oncompleteshow = function () { };
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

    form.header.saveStartPosition = function (event, currStartX, currStartY) {
        var formStartX = jsObject.FindPosX(form, "stiDesignerMainPanel");
        var formStartY = jsObject.FindPosY(form, "stiDesignerMainPanel");
        jsObject.options.formInDrag = [currStartX, currStartY, formStartX, formStartY, form];
    }

    //Mouse Events
    form.header.onmousedown = function (event) {
        if (jsObject.options.buttonPressed &&
            (jsObject.options.buttonPressed == form.buttonClose ||
                jsObject.options.buttonPressed == form.buttonHelp)) {
            return;
        }
        if (!event || form.isTouchStartFlag) return;
        var mouseStartX = event.clientX;
        var mouseStartY = event.clientY;
        form.header.saveStartPosition(event, mouseStartX, mouseStartY);
    }

    //Touch Events
    form.header.ontouchstart = function (event) {
        var this_ = this;
        this.isTouchStartFlag = true;
        clearTimeout(this.isTouchStartTimer);
        var fingerStartX = event.touches[0].pageX;
        var fingerStartY = event.touches[0].pageY;
        form.header.saveStartPosition(event, fingerStartX, fingerStartY);
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    form.header.ontouchmove = function (event) {
        if (event) event.preventDefault();

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
            formInDrag.movedByUser = true;
        }
    }

    form.header.ontouchend = function (event) {
        if (event) event.preventDefault();
        jsObject.options.formInDrag = false;
    }

    //Form Move
    form.move = function (evnt) {
        var leftPos = jsObject.options.formInDrag[2] + (evnt.clientX - jsObject.options.formInDrag[0]);
        var topPos = jsObject.options.formInDrag[3] + (evnt.clientY - jsObject.options.formInDrag[1]);
        this.style.left = leftPos > 0 ? leftPos + "px" : 0;
        this.style.top = topPos > 0 ? topPos + "px" : 0;
        this.movedByUser = true;
    }

    form.addControlRow = function (table, textControl, controlName, control, margin, minWidth, paddingLeft) {
        if (!this.controls) this.controls = {};
        this.controls[controlName] = control;
        this.controls[controlName + "Row"] = table.addRow();

        if (textControl != null) {
            var text = table.addCellInLastRow();
            this.controls[controlName + "Text"] = text;
            text.innerHTML = textControl;
            text.className = "stiDesignerCaptionControls";
            text.style.paddingLeft = (paddingLeft || 12) + "px";
            text.style.minWidth = (minWidth || 100) + "px";
        }

        if (control) {
            control.style.margin = margin;
            var controlCell = table.addCellInLastRow(control);
            if (textControl == null) controlCell.setAttribute("colspan", 2);
        }

        return controlCell;
    }

    form.correctTopPosition = function () {
        var formTop = jsObject.FindPosY(form, "stiDesignerMainPanel");
        if (formTop + form.offsetHeight > jsObject.options.mainPanel.offsetHeight) {
            form.style.top = (jsObject.options.mainPanel.offsetHeight - form.offsetHeight - 5) + "px";
        }
    }

    form.correctWidth = function (minWidth) {
        var formLeft = parseInt(form.style.left.replace("px", ""));
        if (formLeft + form.offsetWidth > jsObject.options.mainPanel.offsetWidth) {
            var width = jsObject.options.mainPanel.offsetWidth - formLeft - 5;
            if (minWidth && width < minWidth) width = minWidth;
            form.style.width = width + "px";
        }
    }

    form.dockToComponent = function () {
        var component = jsObject.options.selectedObject;
        if (component) {
            var top = parseInt(component.getAttribute("top"));
            var left = parseInt(component.getAttribute("left"));
            var right = left + parseInt(component.getAttribute("width"));

            var pagePos = jsObject.FindPagePositions(jsObject.options.mainPanel);
            var compAbsPosLeft = pagePos.posX + left;
            var compAbsPosRight = pagePos.posX + right;
            var compAbsPosTop = pagePos.posY + top;

            //Calculate left pos
            var formLeft = compAbsPosRight + 10;

            if (formLeft + this.offsetWidth > jsObject.options.mainPanel.offsetWidth && compAbsPosLeft - this.offsetWidth - 10 > 0) {
                formLeft = compAbsPosLeft - this.offsetWidth - 10;
            }
            else if (formLeft + this.offsetWidth > jsObject.options.mainPanel.offsetWidth) {
                formLeft -= (formLeft + this.offsetWidth - jsObject.options.mainPanel.offsetWidth + 5);
            }

            //Calculate top pos
            var formTop = compAbsPosTop;

            if (formTop + this.offsetHeight > jsObject.options.mainPanel.offsetHeight) {
                formTop = jsObject.options.mainPanel.offsetHeight - this.offsetHeight - 5;
            }

            if (formTop < 0) formTop = 5;
            if (formLeft < 0) formLeft = 5;

            this.style.left = formLeft + "px";
            this.style.top = formTop + "px";

            this.dockingComponent = component;
        }
    }

    form.hideButtonsPanel = function () {
        form.buttonsPanel.style.display = "none";        
        form.container.style.borderBottom = "0";

        if (jsObject.options.osWin11 && jsObject.options.standaloneJsMode) {
            form.container.style.borderRadius = "0 0 10px 10px";
        }
    }

    return form;
}

StiMobileDesigner.prototype.FormBlock = function (width, height) {
    var formBlock = document.createElement("div");
    formBlock.className = "stiDesignerFormBlock";
    if (width) formBlock.style.minWidth = width + "px";
    if (height) formBlock.style.minHeight = height + "px";

    return formBlock;
}

StiMobileDesigner.prototype.FormBlockHeader = function (caption) {
    var formBlockHeader = document.createElement("div");
    formBlockHeader.className = "stiDesignerFormBlockHeader";

    var formBlockCaption = document.createElement("div");
    formBlockHeader.caption = formBlockCaption;
    formBlockCaption.style.padding = "6px 6px 6px 15px";
    formBlockCaption.innerHTML = "<b>" + caption + "<b>";
    formBlockHeader.appendChild(formBlockCaption);

    return formBlockHeader;
}

StiMobileDesigner.prototype.FormSeparator = function () {
    var separator = document.createElement("div");
    separator.className = "stiDesignerFormSeparator";

    return separator;
}

StiMobileDesigner.prototype.SeparatorOr = function (width, lineColor) {
    var sep = document.createElement("div");
    if (width) sep.style.width = width + "px";

    var sepTable = this.CreateHTMLTable();
    sepTable.style.width = "100%";
    sep.appendChild(sepTable);

    var line1 = document.createElement("div");
    line1.className = "stiCreateDataSeparatorLine";
    if (lineColor) line1.style.borderColor = lineColor;
    sepTable.addCell(line1);

    var textOr = document.createElement("div");
    textOr.className = "stiCreateDataHintText";
    textOr.innerHTML = this.loc.Report.FilterOr.toString().toLowerCase();
    textOr.style.padding = "0 8px 0 8px";
    sepTable.addCell(textOr).style.width = "1px";

    var line2 = document.createElement("div");
    line2.className = "stiCreateDataSeparatorLine";
    if (lineColor) line2.style.borderColor = lineColor;
    sepTable.addCell(line2);

    return sep;
}