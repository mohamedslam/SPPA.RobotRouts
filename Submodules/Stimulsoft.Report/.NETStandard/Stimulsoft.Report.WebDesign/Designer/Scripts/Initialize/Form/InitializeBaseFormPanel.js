
StiMobileDesigner.prototype.BaseFormPanel = function (name, caption, level, helpUrl) {
    var jsObject = this;
    var formPanel = this.BaseForm(name, caption, level, helpUrl);
    formPanel.isFormPanel = true;

    //Override Methods
    formPanel.changeVisibleState = function (state) {
        jsObject.options.propertiesPanel.setZIndex(state, formPanel.level);
        if (state) {
            if (jsObject.options.paintPanel.copyStyleMode)
                jsObject.options.paintPanel.setCopyStyleMode(false);

            if (this.visible) {
                this.changeVisibleState(false);
            }

            this.style.display = "";

            if (jsObject.options.currentForm && this.parentElement == jsObject.options.mainPanel) {
                jsObject.options.mainPanel.removeChild(this);
                jsObject.options.mainPanel.appendChild(this);
            }

            this.onshow();

            if (!this.resultControl || !this.resultControl.notShowDictionary) {
                jsObject.options.propertiesPanel.changeVisibleState(true);
            }

            if (this.isDockableToComponent)
                this.dockToComponent();
            else
                jsObject.SetObjectToCenter(this);

            if (!jsObject.options.disabledPanels) jsObject.InitializeDisabledPanels();
            jsObject.options.disabledPanels[this.level].changeVisibleState(true);

            this.visible = true;

            this.currentFormDownLevel = jsObject.options.currentForm && jsObject.options.currentForm.visible
                ? jsObject.options.currentForm : null;
            jsObject.options.currentForm = this;

            var d = new Date();
            var endTime = d.getTime() + jsObject.options.formAnimDuration;
            this.flag = false;
            jsObject.ShowAnimationForm(this, endTime);
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
            var propertiesPanel = jsObject.options.propertiesPanel;
            if (propertiesPanel && !propertiesPanel.styleDesignerMode && !propertiesPanel.editChartMode && !propertiesPanel.dictionaryMode && propertiesPanel.fixedViewMode) {
                propertiesPanel.changeVisibleState(false);
            }
            if (!jsObject.options.disabledPanels) jsObject.InitializeDisabledPanels();
            jsObject.options.disabledPanels[this.level].changeVisibleState(false);
        }
    }

    formPanel.addControlRow = function (table, textControl, controlName, control, margin) {
        if (!this.controls) this.controls = {};
        this.controls[controlName] = control;
        this.controls[controlName + "Row"] = table.addRow();

        if (textControl != null) {
            var text = table.addCellInLastRow();
            this.controls[controlName + "Text"] = text;
            text.innerHTML = textControl;
            text.className = "stiDesignerCaptionControls";
            text.style.paddingLeft = "12px";
            text.style.minWidth = "130px";
        }

        if (control) {
            control.style.margin = margin;
            var controlCell = table.addCellInLastRow(control);
            if (textControl == null) controlCell.setAttribute("colspan", 2);
        }

        return controlCell;
    }

    return formPanel;
}
