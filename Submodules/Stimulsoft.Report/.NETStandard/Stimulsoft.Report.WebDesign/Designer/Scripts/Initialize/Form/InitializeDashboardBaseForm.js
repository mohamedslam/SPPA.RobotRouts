
StiMobileDesigner.prototype.DashboardBaseForm = function (name, caption, level, helpUrl) {
    var form = this.BaseForm(name, caption, level, helpUrl);
    form.isNotModal = true;
    var jsObject = this;

    form.changeVisibleState = function (state) {
        if (state) {
            if (jsObject.options.currentForm && jsObject.options.currentForm.isNotModal) {
                jsObject.options.currentForm.changeVisibleState(false);
            }

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

            var selectedObject = jsObject.options.selectedObject;
            if (selectedObject && selectedObject.isDashboardElement && this.dockingComponent == selectedObject) {
                var controls = selectedObject.controls;
                if (controls.editDbsButton) controls.editDbsButton.style.visibility = "visible";
                if (controls.filtersDbsButton) controls.filtersDbsButton.style.visibility = "visible";
                if (controls.changeTypeDbsButton) controls.changeTypeDbsButton.style.visibility = "visible";
                if (controls.topNDbsButton) controls.topNDbsButton.style.visibility = "visible";
            }

            this.dockingComponent = null;
            jsObject.options.currentForm = this.currentFormDownLevel || null;
            this.style.display = "none";
            if (!jsObject.options.forms[this.name]) {
                jsObject.options.mainPanel.removeChild(this);
            }
            this.onhide();
        }
    }

    form.correctTopPosition = function () {
        if (!form.contOldStyles) {
            form.contOldStyles = {
                height: form.container.style.height,
                width: form.container.style.width,
                overflowX: form.container.style.overflowX,
                overflowY: form.container.style.overflowY
            }
        }

        form.container.style.height = form.contOldStyles.height;
        form.container.style.width = form.contOldStyles.width;
        form.container.style.overflowX = form.contOldStyles.overflowX;
        form.container.style.overflowY = form.contOldStyles.overflowY;

        var formTop = jsObject.FindPosY(form, "stiDesignerMainPanel");
        if (formTop + form.offsetHeight > jsObject.options.mainPanel.offsetHeight) {
            var newTop = jsObject.options.mainPanel.offsetHeight - form.offsetHeight - 5;
            form.style.top = (newTop >= 0 ? newTop : 5) + "px";
        }

        if (form.offsetHeight > jsObject.options.mainPanel.offsetHeight) {
            var newHeight = jsObject.options.mainPanel.offsetHeight - form.header.offsetHeight - form.buttonsPanel.offsetHeight - 20;
            if (newHeight > 0) {
                form.container.style.overflowY = "auto";
                form.container.style.overflowX = "hidden";
                form.container.style.height = newHeight + "px";
                if (!jsObject.options.isTouchDevice) form.container.style.width = (form.container.offsetWidth + 10) + "px";
            }
        }
    }

    form.dockToComponent_ = form.dockToComponent;

    form.dockToComponent = function () {
        this.dockToComponent_();

        var component = this.dockingComponent;
        if (component) {
            var controls = component.controls;
            if (controls.editDbsButton) controls.editDbsButton.style.visibility = "hidden";
            if (controls.filtersDbsButton) controls.filtersDbsButton.style.visibility = "hidden";
            if (controls.changeTypeDbsButton) controls.changeTypeDbsButton.style.visibility = "hidden";
            if (controls.topNDbsButton) controls.topNDbsButton.style.visibility = "hidden";
        }
    }

    form.addControlRow = function (table, textControl, controlName, control, margin, textPadding, textWordWrap, controlAlignRight, captionMaxSize) {
        if (!this.controls) this.controls = {};
        this.controls[controlName] = control;
        this.controls[controlName + "Row"] = table.addRow();

        if (textControl != null) {
            var text = this.controls[controlName + "Text"] = table.addCellInLastRow();
            text.innerHTML = textControl;
            text.className = "stiDesignerCaptionControls";
            text.style.paddingLeft = "12px";

            if (captionMaxSize) {
                text.style.width = "100%";
            }
            if (textPadding) {
                text.style.padding = textPadding;
            }
            if (textWordWrap) {
                text.style.whiteSpace = "normal";
            }
        }

        if (control) {
            control.style.margin = margin;
            var controlCell = table.addCellInLastRow(control);
            controlCell.style.width = "1px";

            if (textControl == null) {
                controlCell.setAttribute("colspan", 2);
            }
            if (controlAlignRight) {
                control.style.display = "inline-block";
                controlCell.style.textAlign = "right";
                controlCell.style.lineHeight = "0";
            }
        }

        return controlCell;
    }

    return form;
}
