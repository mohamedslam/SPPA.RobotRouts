
StiMobileDesigner.prototype.PagesButton = function (page) {
    var jsObject = this;
    var name = page.properties.name;
    var pageIcon = page.properties.pageIcon;
    var alias = StiBase64.decode(page.properties.aliasName);
    var captionText = (!alias || name == alias) ? name : name + " [" + alias + "]";
    var image = "SmallComponents." + (page.isDashboard ? "StiDashboard.png" : "StiPage.png");

    if (this.options.useAliases && this.options.showOnlyAliasForPages && alias) {
        captionText = alias;
    }

    var button = this.TabButton(null, null, captionText, image, captionText);
    button.key = name;

    if (pageIcon) {
        button.image.src = pageIcon;
    }

    button.action = function () {
        var pages = jsObject.options.pagesPanel.pagesContainer.pages;
        for (var i = 0; i < pages.length; i++) {
            if (this != pages[i]) {
                pages[i].setSelected(false);
            }
        }
        this.setSelected(true);

        if (jsObject.options.currentPage != jsObject.options.report.pages[this.key]) {
            jsObject.options.paintPanel.showPage(jsObject.options.report.pages[this.key]);
        }
    }

    //Override
    button.setSelected = function (state) {
        if (!state) this.setEditMode(false);
        this.isSelected = state;
        this.className = state ? this.selectedClass : (this.isEnabled ? (this.isOver ? this.overClass : this.defaultClass) : this.disabledClass);
        this.footer.className = state ? "stiDesignerStandartTabFooter" : "";
    }

    button.onmouseup = function (event) {
        if (this.isTouchEndFlag || jsObject.options.isTouchClick) return;
        if (event.button == 2) {
            event.stopPropagation();
            this.action();
            var pageMenu = jsObject.options.menus.pageContextMenu || jsObject.InitializePageContextMenu();
            var point = jsObject.FindMousePosOnMainPanel(event);
            pageMenu.pageButton = this;
            pageMenu.show(point.xPixels + 3, point.yPixels + 3, "Down", "Right");
        }
        return false;
    }

    var contextTimer = null;
    button.ontouchstart = function (event) {
        jsObject.options.fingerIsMoved = false;
        jsObject.options.buttonPressed = this;
        var this_ = this;
        contextTimer = setTimeout(function () {
            if (jsObject.options.fingerIsMoved) return;
            this_.action();
            var pageMenu = jsObject.options.menus.pageContextMenu || jsObject.InitializePageContextMenu();
            var point = jsObject.FindMousePosOnMainPanel(event);
            pageMenu.pageButton = this_;
            pageMenu.show(point.xPixels - 2, point.yPixels - 2, "Up");
        }, 1000);
    }

    button.ontouchend = function (event) {
        var this_ = this;
        this.isTouchEndFlag = true;
        clearTimeout(this.isTouchEndTimer);
        clearTimeout(contextTimer);
        if (!this.isEnabled || jsObject.options.fingerIsMoved) return;
        this.action();
        this.isTouchEndTimer = setTimeout(function () {
            this_.isTouchEndFlag = false;
        }, 1000);
    }

    button.oncontextmenu = function (event) {
        return false;
    }

    var editTextBox = this.TextBox(null, 70);
    button.innerTable.addCell(editTextBox);
    editTextBox.style.display = "none";
    editTextBox.style.margin = "0px 4px 1px 4px";
    editTextBox.style.height = "18px";
    editTextBox.style.border = "0px";

    editTextBox.onblur = function () {
        this.isOver = false;
        this.setSelected(false);
        jsObject.options.controlsIsFocused = false;
        if (!this.readOnly && (this.oldValue != this.value || this.keyEnterPressed)) this.action();
        this.keyEnterPressed = false;
        button.setEditMode(false);
    }

    editTextBox.action = function () {
        jsObject.SendCommandRenameComponent(button.ownerPage, this.value);
    }

    button.setEditMode = function (state) {
        editTextBox.style.display = state ? "" : "none";
        this.caption.style.display = !state ? "" : "none";
        if (state) {
            editTextBox.value = this.captionText;
            editTextBox.focus();
        }
    }

    button.ondblclick = function () {
        jsObject.InitializeRenamePageForm(function (form) {
            form.show();
        });
    }

    return button;
}
