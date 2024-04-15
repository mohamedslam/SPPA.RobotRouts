
StiJsViewer.prototype.InitializeParametersPanel = function () {
    var jsObject = this;
    var createAndShow = !this.options.isMobileDevice;
    if (this.controls.parametersPanel) {
        createAndShow = this.controls.parametersPanel.visible;
        this.controls.parametersPanel.isUpdatePanel = true;
        this.controls.parametersPanel.changeVisibleState(false);
        this.controls.mainPanel.removeChild(this.controls.parametersPanel);
        delete this.controls.parametersPanel;
    }
    if (this.options.toolbar.visible && this.options.toolbar.showParametersButton) {
        this.controls.toolbar.controls.Parameters.setEnabled(this.options.paramsVariables != null);
    }
    if (this.controls.dashboardsPanel && this.controls.buttons.ParametersDashboard) {
        this.controls.buttons.ParametersDashboard.style.display = this.controls.buttons.ParametersDashboard.allowToShow && this.options.paramsVariables ? "" : "none";
    }

    if (this.options.paramsVariables == null) return;

    var parametersPanel = document.createElement("div");
    parametersPanel.menus = {};
    this.controls.parametersPanel = parametersPanel;
    this.controls.mainPanel.appendChild(parametersPanel);

    parametersPanel.className = "stiJsViewerParametersPanel";

    var parametersPanelPosition = this.options.currentParametersPanelPosition || this.options.appearance.parametersPanelPosition;

    if (parametersPanelPosition == "Top") {
        parametersPanel.className += " stiJsViewerParametersPanelTop";

        if (this.options.toolbar.displayMode == "Separated") {
            parametersPanel.className += " stiJsViewerParametersPanelSeparatedTop";
        }
    }

    parametersPanel.id = this.controls.viewer.id + "_ParametersPanel";
    parametersPanel.style.display = "none";
    parametersPanel.visible = false;
    parametersPanel.style.fontFamily = this.options.toolbar.fontFamily;
    if (this.options.toolbar.fontColor != "") parametersPanel.style.color = this.options.toolbar.fontColor;

    var jsObject = parametersPanel.jsObject = this;
    parametersPanel.currentOpeningParameter = null;
    parametersPanel.dropDownButtonWasClicked = false;
    parametersPanel.dateTimeButtonWasClicked = false;

    var styleTop = this.options.toolbar.visible ? this.controls.toolbar.offsetHeight : 0;
    if (this.options.isMobileDevice && this.options.toolbar.autoHide) styleTop = 0;
    styleTop += this.controls.drillDownPanel ? this.controls.drillDownPanel.offsetHeight : 0;
    styleTop += this.controls.findPanel ? this.controls.findPanel.offsetHeight : 0;
    styleTop += this.controls.resourcesPanel ? this.controls.resourcesPanel.offsetHeight : 0;
    parametersPanel.style.top = styleTop + "px";

    if (parametersPanelPosition == "Left") {
        if (this.options.isMobileDevice) parametersPanel.style.bottom = this.options.toolbar.autoHide ? "0" : "0.5in";
        else parametersPanel.style.bottom = this.options.toolbar.displayMode == "Separated" && this.options.toolbar.visible ? "35px" : "0";
    }

    if (this.options.isMobileDevice) parametersPanel.style.transition = "opacity 300ms ease";

    var innerPanel = document.createElement("div");
    parametersPanel.appendChild(innerPanel);

    if (this.options.toolbar.displayMode == "Simple") {
        innerPanel.style.marginTop = "2px";
        innerPanel.className = "stiJsViewerInnerParametersPanelSimple";
    }

    if (parametersPanelPosition == "Left") {
        innerPanel.className += " stiJsViewerInnerParametersPanelLeft";
        if (this.options.toolbar.displayMode == "Separated") {
            innerPanel.className += " stiJsViewerInnerParametersPanelSeparatedLeft";
        }
    }

    //Container
    parametersPanel.container = document.createElement("div");
    parametersPanel.container.id = parametersPanel.id + "Container";
    parametersPanel.container.className = "stiJsViewerInnerContainerParametersPanel";
    parametersPanel.container.jsObject = this;
    innerPanel.appendChild(parametersPanel.container);

    if (this.options.toolbar.backgroundColor != "") {
        parametersPanel.container.style.background = this.options.toolbar.backgroundColor;
    }

    if (this.options.toolbar.borderColor != "") {
        parametersPanel.container.style.border = "1px solid " + this.options.toolbar.borderColor;
    }

    if (parametersPanelPosition == "Top") {
        parametersPanel.container.style.maxHeight = this.options.appearance.parametersPanelMaxHeight + "px";
    }

    if (this.reportParams.type == "Dashboard") {
        parametersPanel.style.border = "0";
        innerPanel.style.border = "0";
        parametersPanel.container.style.background = "transparent";
        innerPanel.style.background = "transparent";
    }

    //Buttons
    var mainButtons = this.CreateHTMLTable();
    parametersPanel.mainButtons = mainButtons;
    mainButtons.setAttribute("align", "right");
    mainButtons.style.margin = "12px 0 2px 0";
    mainButtons.ID = parametersPanel.id + "MainButtons";

    parametersPanel.mainButtons.reset = this.FormButton("Reset", this.collections.loc["Reset"], null, 80);
    parametersPanel.mainButtons.submit = this.FormButton("Submit", this.collections.loc["Submit"], null, 80, "stiJsViewerFormButtonTheme");
    mainButtons.addCell(parametersPanel.mainButtons.reset);
    mainButtons.addCell(parametersPanel.mainButtons.submit).style.paddingLeft = "10px";

    if (!this.options.isTouchDevice) {
        parametersPanel.container.onscroll = function () { parametersPanel.hideAllMenus(); }
    }

    parametersPanel.changeVisibleState = function (state) {
        var isStateChanged = this.visible != state;
        parametersPanel.style.display = state ? "" : "none";
        parametersPanel.visible = state;
        if (!state) parametersPanel.hideAllMenus();
        if (jsObject.options.toolbar.visible && jsObject.options.toolbar.showParametersButton) jsObject.controls.toolbar.controls.Parameters.setSelected(state);
        if (jsObject.controls.buttons.ParametersDashboard) jsObject.controls.buttons.ParametersDashboard.setSelected(state);

        if (isStateChanged) jsObject.updateLayout();

        if (jsObject.options.isMobileDevice) {
            var controls = jsObject.controls;
            if (state && controls.bookmarksPanel) controls.bookmarksPanel.changeVisibleState(false);
            setTimeout(function () {
                parametersPanel.style.opacity = state ? "1" : "0";
                if (state) controls.reportPanel.hideToolbar();
                else if (!this.isUpdatePanel) controls.reportPanel.showToolbar();
            });
        }

        if (state) parametersPanel.hideHiddenRows(true);
    }

    parametersPanel.checkCategory = function (paramsVariables, categoryName) {
        for (var i = 0; i < jsObject.getCountObjects(paramsVariables); i++) {
            if (paramsVariables[i].isCategory && paramsVariables[i].category == categoryName) {
                return true;
            }
        }
        return false;
    }

    parametersPanel.hasCategories = function (paramsVariables) {
        for (var i = 0; i < jsObject.getCountObjects(paramsVariables); i++) {
            if (paramsVariables[i].isCategory) {
                return true;
            }
        }
        return false;
    }

    parametersPanel.getOnlyVariables = function (paramsVariables) {
        var variables = [];
        for (var i = 0; i < jsObject.getCountObjects(paramsVariables); i++) {
            if (!paramsVariables[i].isCategory) {
                variables.push(paramsVariables[i]);
            }
        }
        return variables;
    }

    parametersPanel.addParameters = function () {
        var paramsVariables = jsObject.copyObject(jsObject.options.paramsVariables);

        if (this.hasCategories(paramsVariables) && parametersPanelPosition == "Left") {
            this.addParametersByCategories(paramsVariables);
        }
        else {
            this.addParametersWithoutCategories(paramsVariables);
        }
    }

    parametersPanel.addParametersByCategories = function (paramsVariables) {
        var otherVars = [];
        var categories = {}

        var addCategory = function (categoryName, collapsed) {
            categories[categoryName] = {
                collapsed: collapsed,
                variables: []
            };
        }

        for (var i = 0; i < jsObject.getCountObjects(paramsVariables); i++) {
            var category = paramsVariables[i].category;
            var name = paramsVariables[i].name;
            var collapsed = !paramsVariables[i].readOnly;

            if (category != "" && name == "") {
                if (!categories[category]) {
                    addCategory(category, collapsed);
                }
                else {
                    categories[category].collapsed = collapsed;
                }
            }
            else if (category == "" || !this.checkCategory(paramsVariables, category)) {
                otherVars.push(paramsVariables[i]);
            }
            else {
                if (!categories[category]) {
                    addCategory(category, false);
                }
                categories[category].variables.push(paramsVariables[i]);
            }
        }

        var table = jsObject.CreateHTMLTable();
        this.container.appendChild(table);

        var addParameterToCurrentRow = function (varParams) {
            var nameCell = table.addTextCellInLastRow(varParams.alias);
            nameCell.style.padding = "0 8px 0 8px";
            if (parametersPanelPosition == "Left") nameCell.style.whiteSpace = "nowrap";
            if (varParams.description) nameCell.title = varParams.description;
            table.addCellInLastRow(jsObject.CreateParameter(varParams));
        }

        this.hiddenRows = [];

        for (var catName in categories) {
            var category = categories[catName];
            var catButton = jsObject.ParameterCategoryButton(catName, category.collapsed);
            catButton.style.margin = "3px 0 3px 0";
            catButton.rows = [];

            if (table.tr.length > 1) table.addRow();
            table.addCellInLastRow(catButton).setAttribute("colspan", "2");

            for (var i = 0; i < category.variables.length; i++) {
                var row = table.addRow();
                if (!catButton.isOpened) {
                    this.hiddenRows.push(row);
                }                
                catButton.rows.push(row);

                catButton.onAction = function () {
                    for (var i = 0; i < this.rows.length; i++) {
                        this.rows[i].style.display = this.isOpened ? "" : "none";
                    }
                }

                addParameterToCurrentRow(category.variables[i]);
            }
        }
                
        for (var i = 0; i < otherVars.length; i++) {
            var row = table.addRow();
            addParameterToCurrentRow(otherVars[i]);
        }

        table.addCellInNextRow(parametersPanel.mainButtons).setAttribute("colspan", "2");
    }

    parametersPanel.setFixWidth = function () {
        if (this.container.offsetWidth > 0) {
            this.container.style.width = this.container.offsetWidth + "px";
            jsObject.updateLayout();
        }
    }

    parametersPanel.hideHiddenRows = function (setFixWidth) {
        if (this.hiddenRows && this.hiddenRows.length > 0) {
            if (setFixWidth) {
                this.setFixWidth();
            }
            for (var i = 0; i < this.hiddenRows.length; i++) {
                this.hiddenRows[i].style.display = "none";
            }
            this.hiddenRows = null;
        }
    }

    parametersPanel.addParametersWithoutCategories = function (paramsVariables) {
        paramsVariables = this.getOnlyVariables(paramsVariables);
        var countParameters = paramsVariables.length;
        var countColumns = parametersPanelPosition == "Left" ? 1 : ((countParameters <= jsObject.options.minParametersCountForMultiColumns) ? 1 : jsObject.options.appearance.parametersPanelColumnsCount);

        var countInColumn = parseInt(countParameters / countColumns);
        if (countInColumn * countColumns < countParameters) countInColumn++;

        var table = document.createElement("table");
        table.cellPadding = 0;
        table.cellSpacing = 0;
        table.style.border = 0;
        var tbody = document.createElement("tbody");
        table.appendChild(tbody);
        this.container.appendChild(table);

        var cellsVar = {};
        for (var indexRow = 0; indexRow < countInColumn + 1; indexRow++) {
            var row = document.createElement("tr");
            tbody.appendChild(row);

            for (indexColumn = 0; indexColumn < countColumns; indexColumn++) {
                var cellForName = document.createElement("td");
                cellForName.style.padding = "0 10px 0 " + ((indexColumn > 0) ? "30px" : 0);
                if (parametersPanelPosition == "Left") cellForName.style.whiteSpace = "nowrap";
                row.appendChild(cellForName);

                var cellForControls = document.createElement("td");
                cellForControls.style.padding = 0;
                row.appendChild(cellForControls);

                cellsVar[indexRow + "_" + indexColumn + "_name"] = cellForName;
                cellsVar[indexRow + "_" + indexColumn + "_controls"] = cellForControls;
            }
        }

        var indexColumn = 0;
        var indexRow = 0;

        for (var index = 0; index < countParameters; index++) {
            var nameCell = cellsVar[indexRow + "_" + indexColumn + "_name"];
            nameCell.style.whiteSpace = "nowrap";
            nameCell.innerText = paramsVariables[index].alias;
            if (paramsVariables[index].description) nameCell.title = paramsVariables[index].description;

            cellsVar[indexRow + "_" + indexColumn + "_controls"].appendChild(jsObject.CreateParameter(paramsVariables[index]));
            indexRow++;

            if (index == countParameters - 1) cellsVar[indexRow + "_" + indexColumn + "_controls"].appendChild(parametersPanel.mainButtons);
            if (indexRow == countInColumn) { indexRow = 0; indexColumn++; }
        }
    }

    parametersPanel.clearParameters = function () {
        while (parametersPanel.container.childNodes[0]) {
            parametersPanel.container.removeChild(parametersPanel.container.childNodes[0]);
        }
    }

    parametersPanel.getParametersValues = function () {
        var parametersValues = {};
        for (var name in jsObject.options.parameters) {
            var parameter = jsObject.options.parameters[name];
            parametersValues[name] = parameter.getValue();
        }

        return parametersValues;
    }

    parametersPanel.hideAllMenus = function () {
        if (jsObject.options.currentMenu) jsObject.options.currentMenu.changeVisibleState(false);
        if (jsObject.options.currentDatePicker) jsObject.options.currentDatePicker.changeVisibleState(false);
    }

    this.options.parameters = {};
    parametersPanel.addParameters();
    parametersPanel.changeVisibleState(createAndShow);
}

//Button
StiJsViewer.prototype.ParameterButton = function (buttonType, parameter) {
    var button = this.SmallButton(null, null, buttonType + ".png", null, null, "stiJsViewerSmallButtonWithBorder");
    button.style.height = this.options.isTouchDevice ? "26px" : "21px";
    button.innerTable.style.width = "100%";
    button.imageCell.style.textAlign = "center";
    button.parameter = parameter;
    button.buttonType = buttonType;

    return button;
}

//TextBox
StiJsViewer.prototype.ParameterTextBox = function (parameter) {
    var textBox = this.TextBox(null, null, null, true);
    textBox.parameter = parameter;
    if (parameter.params.type == "Char") textBox.maxLength = 1;

    var paramWidth = this.options.currentParameterWidth;
    var width = "";

    if (parameter.params.basicType == "Range") {
        width = "106px";
        if (parameter.params.type == "Guid") width = "190px";
        if (parameter.params.type == "DateTime" || parameter.params.type == "DateTimeOffset") width = "130px";
        if (parameter.params.type == "Char") width = "60px";
        if (paramWidth) width = parseInt(Math.max(0, paramWidth - 18) / 2) + "px";
    }
    else {
        width = (paramWidth ? parseInt(paramWidth) : 230) +  "px";
    }

    textBox.style.width = width;

    if (parameter.params.type == "DateTime" || parameter.params.type == "DateTimeOffset") {
        textBox.action = function () {
            if (this.oldValue == this.value) return;
            try {
                var timeString = new Date().toLocaleTimeString();
                var isAmericanFormat = timeString.toLowerCase().indexOf("am") >= 0 || timeString.toLowerCase().indexOf("pm") >= 0;
                var formatDate = isAmericanFormat ? "MM/dd/yyyy" : "dd.MM.yyyy";
                var format = formatDate + (isAmericanFormat ? " h:mm:ss tt" : " hh:mm:ss");
                if (textBox.parameter.params.dateTimeType == "Date") format = formatDate;
                if (textBox.parameter.params.dateTimeType == "Time") format = "hh:mm:ss";
                var date = textBox.jsObject.GetDateTimeFromString(this.value, this.jsObject.options.appearance.parametersPanelDateFormat || format);
                var dateTimeObject = textBox.jsObject.getDateTimeObject(date);
                textBox.parameter.params[textBox.parameter.controls.secondTextBox == textBox ? "keyTo" : "key"] = dateTimeObject;
                textBox.value = textBox.jsObject.dateTimeObjectToString(dateTimeObject, textBox.parameter.params.dateTimeType);
            }
            catch (e) {
                alert(e);
            }
        }
    }

    return textBox;
}

//CheckBox
StiJsViewer.prototype.ParameterCheckBox = function (parameter, caption) {
    var checkBox = this.CheckBox(null, caption);
    checkBox.parameter = parameter;

    return checkBox;
}

//Menu
StiJsViewer.prototype.ParameterMenu = function (parameter) {
    var menu = this.BaseMenu(null, parameter.controls.dropDownButton, "Down", "stiJsViewerDropdownMenu");
    menu.parameter = parameter;

    menu.changeVisibleState = function (state, parentButton) {
        var mainClassName = "stiJsViewerMainPanel";
        if (parentButton) {
            this.parentButton = parentButton;
            parentButton.haveMenu = true;
        }
        if (state) {
            this.style.display = "";
            this.onshow();
            this.visible = true;
            this.style.overflow = "hidden";
            this.parentButton.setSelected(true);
            this.jsObject.options.currentMenu = this;
            this.style.width = this.innerContent.offsetWidth + "px";
            this.style.height = this.innerContent.offsetHeight + "px";
            this.style.left = (this.jsObject.FindPosX(parameter, mainClassName)) + "px";
            var animDirect = this.animationDirection;
            var browserHeight = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
            if (this.parentButton && animDirect == "Down" &&
                this.jsObject.FindPosY(this.parentButton) + this.parentButton.offsetHeight + this.innerContent.offsetHeight > browserHeight &&
                this.jsObject.FindPosY(this.parentButton) - this.innerContent.offsetHeight > 0) {
                animDirect = "Up";
            }
            this.style.top = (animDirect == "Down")
                ? (this.jsObject.FindPosY(this.parentButton, mainClassName) + this.parentButton.offsetHeight + 2) + "px"
                : (this.jsObject.FindPosY(this.parentButton, mainClassName) - this.offsetHeight) + "px";
            this.innerContent.style.top = ((animDirect == "Down" ? -1 : 1) * this.innerContent.offsetHeight) + "px";

            parameter.menu = this;

            var d = new Date();
            var endTime = d.getTime();
            if (this.jsObject.options.toolbar.menuAnimation) endTime += this.jsObject.options.menuAnimDuration;
            this.jsObject.ShowAnimationVerticalMenu(this, (animDirect == "Down" ? 0 : -1), endTime);
        }
        else {
            this.onHide();
            clearTimeout(this.innerContent.animationTimer);
            this.visible = false;
            this.parentButton.setSelected(false);
            this.style.display = "none";
            this.jsObject.controls.mainPanel.removeChild(this);
            parameter.menu = null;
            if (this.jsObject.options.currentMenu == this) this.jsObject.options.currentMenu = null;
        }
    }

    var table = this.CreateHTMLTable();
    table.style.fontFamily = this.options.toolbar.fontFamily;
    if (this.options.toolbar.fontColor != "") table.style.color = this.options.toolbar.fontColor;
    table.style.fontSize = "12px";
    table.style.width = (parameter.offsetWidth - 5) + "px";
    table.className = "stiJsViewerClearAllStyles stiJsViewerParametersMenuInnerTable";
    menu.innerContent.appendChild(table);
    menu.innerTable = table;

    return menu;
}

//MenuItem
StiJsViewer.prototype.parameterMenuItem = function (parameter) {
    var menuItem = document.createElement("div");
    menuItem.jsObject = this;
    menuItem.parameter = parameter;
    menuItem.isOver = false;
    menuItem.className = "stiJsViewerParametersMenuItem";
    menuItem.style.height = this.options.isTouchDevice ? "30px" : "24px";

    var table = this.CreateHTMLTable();
    table.className = "stiJsViewerClearAllStyles stiJsViewerParametersMenuItemInnerTable";
    menuItem.innerTable = table;
    menuItem.appendChild(table);

    menuItem.onmouseover = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseenter();
    }

    menuItem.onmouseout = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseleave();
    }

    menuItem.onmouseenter = function () {
        this.className = "stiJsViewerParametersMenuItemOver";
        this.isOver = true;
        if (this.parameter && this.parameter.menu && this.parameter.menu.currentFindedIndex != null) {
            var menu = this.parameter.menu;
            menu.findedItems[menu.currentFindedIndex].setSelected(false);
            for (var i = 0; i < menu.findedItems.length; i++) {
                if (menu.findedItems[i] == this) {
                    this.setSelected(true);
                    menu.currentFindedIndex = i;
                    break;
                }
            }
        };
    }
    menuItem.onmouseleave = function () {
        this.className = this.isSelected ? "stiJsViewerParametersMenuItemOver" : "stiJsViewerParametersMenuItem";
        this.isOver = false;
    }

    menuItem.onmousedown = function () {
        if (this.isTouchStartFlag) return;
        this.className = "stiJsViewerParametersMenuItemPressed";
    }

    menuItem.ontouchstart = function () {
        var this_ = this;
        this.isTouchStartFlag = true;
        clearTimeout(this.isTouchStartTimer);
        this.parameter.jsObject.options.fingerIsMoved = false;
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    menuItem.onmouseup = function () {
        if (this.isTouchEndFlag) return;
        this.parameter.jsObject.TouchEndMenuItem(this.id, false);
    }

    menuItem.ontouchend = function () {
        var this_ = this;
        this.isTouchEndFlag = true;
        clearTimeout(this.isTouchEndTimer);
        this.parameter.jsObject.TouchEndMenuItem(this.id, true);
        this.isTouchEndTimer = setTimeout(function () {
            this_.isTouchEndFlag = false;
        }, 1000);
    }

    menuItem.setSelected = function (state) {
        this.isSelected = state;
        this.className = state ? "stiJsViewerParametersMenuItemOver" : "stiJsViewerParametersMenuItem";
    }

    return menuItem;
}

StiJsViewer.prototype.addFindControlToParameterMenu = function (parameterMenu, parameter, notShowFindTextControl) {
    var findControl = this.CreateHTMLTable();

    var findTextbox = this.TextBox(null, 228);
    findTextbox.setAttribute("placeholder", this.collections.loc.TypeToSearch);
    findTextbox.style.margin = "4px";
    findControl.addCell(findTextbox);
    findControl.findTextbox = findTextbox;

    var scrollContainer = document.createElement("div");
    scrollContainer.style.maxHeight = "400px";
    scrollContainer.style.overflowX = "hidden";
    scrollContainer.style.overflowY = "auto";
    scrollContainer.appendChild(parameterMenu.innerTable);

    parameterMenu.innerContent.style.overflowX = parameterMenu.innerContent.style.overflowY = "visible";
    parameterMenu.innerContent.style.maxHeight = "";
    if (!notShowFindTextControl) {
        parameterMenu.innerContent.appendChild(findControl);
        parameterMenu.innerContent.appendChild(this.parameterMenuSeparator());
    }
    parameterMenu.innerContent.appendChild(scrollContainer);
    parameterMenu.scrollContainer = scrollContainer;

    parameterMenu.findItems = function (findText) {
        if (parameterMenu.currentFindedIndex != null) {
            parameterMenu.findedItems[parameterMenu.currentFindedIndex].setSelected(false);
            scrollContainer.scrollTop = 0;
        }

        parameterMenu.findedItems = [];
        parameterMenu.currentFindedIndex = null;

        for (var i = 0; i < parameterMenu.paramsItems.length; i++) {
            var itemText = parameterMenu.paramsItems[i].checkBox
                ? parameterMenu.paramsItems[i].checkBox.captionCell.innerText
                : parameterMenu.paramsItems[i].innerTable.tr[0].firstChild.innerText;

            if (parameterMenu.paramsItems[i].isOver) {
                parameterMenu.paramsItems[i].onmouseleave();
            }

            if (itemText.toLowerCase().indexOf(findText.toLowerCase()) >= 0) {
                parameterMenu.paramsItems[i].style.display = "";
                parameterMenu.findedItems.push(parameterMenu.paramsItems[i]);
            }
            else {
                parameterMenu.paramsItems[i].style.display = "none";
            }
        }

        if (parameterMenu.findedItems.length > 0) {
            parameterMenu.showFindedItem(0, findText == "");
        }

        var findNext = function () {
            if (parameterMenu.currentLoadedIndex != null && parameterMenu.currentLoadedIndex < parameter.params.items.length - 1) {
                setTimeout(function () {
                    parameterMenu.addItems(parameterMenu.currentLoadedIndex + 1);
                    findNext();
                }, 0);
            }
        }

        if (findText != "") {
            findNext();
        }
    }

    parameterMenu.showFindedItem = function (index, notVisualSelect) {
        if (parameterMenu.currentFindedIndex != null) {
            parameterMenu.findedItems[parameterMenu.currentFindedIndex].setSelected(false);
        }

        parameterMenu.currentFindedIndex = index;
        if (index >= parameterMenu.findedItems.length) {
            parameterMenu.currentFindedIndex = 0;
        }
        else if (index < 0) {
            parameterMenu.currentFindedIndex = parameterMenu.findedItems.length - 1;
        }

        if (!notVisualSelect) {
            parameterMenu.findedItems[parameterMenu.currentFindedIndex].setSelected(true);
            var yPos = this.jsObject.FindPosY(parameterMenu.findedItems[parameterMenu.currentFindedIndex], "stiJsViewerDropdownMenu", true);
            scrollContainer.scrollTop = yPos - scrollContainer.offsetHeight;
        }
    }

    findTextbox.onChange = function () {
        parameterMenu.findItems(this.value);
    }

    return findControl;
}

StiJsViewer.prototype.TouchEndMenuItem = function (menuItemId, flag) {
    var menuItem = document.getElementById(menuItemId);
    if (!menuItem || menuItem.parameter.jsObject.options.fingerIsMoved) return;

    if (flag) {
        menuItem.className = "stiJsViewerParametersMenuItemPressed";
        if (typeof event !== "undefined" && ('preventDefault' in event)) event.preventDefault();
        setTimeout(function () {
            menuItem.parameter.jsObject.TouchEndMenuItem(menuItem.id, false);
        }, 200);
        return;
    }

    menuItem.className = menuItem.isOver ? "stiJsViewerParametersMenuItemOver" : "stiJsViewerParametersMenuItem";
    if (menuItem.action != null) menuItem.action();
}

//MenuSeparator
StiJsViewer.prototype.parameterMenuSeparator = function () {
    var separator = document.createElement("Div");
    separator.className = "stiJsViewerParametersMenuSeparator";

    return separator;
}

//Menu For Value
StiJsViewer.prototype.parameterMenuForValue = function (parameter) {
    var jsObject = this;
    var loadingStep = 200;
    var items = parameter.params.items;
    var menuParent = this.ParameterMenu(parameter);
    menuParent.paramsItems = [];
    menuParent.currentLoadedIndex = 0;

    if (items) {
        var findControl = this.addFindControlToParameterMenu(menuParent, parameter, items.length < 10);

        menuParent.onshow = function () {
            menuParent.findItems("");
            setTimeout(function () { findControl.findTextbox.focus(); }, 200);
        }

        var valuesHash = {};

        menuParent.addItems = function (startIndex, loadAll) {
            for (var i = startIndex; i < items.length; i++) {
                var menuItem = jsObject.parameterMenuItem(parameter);
                menuItem.id = parameter.jsObject.controls.viewer.id + parameter.params.name + "Item" + i;
                menuItem.parameter = parameter;
                menuItem.key = items[i].key;
                menuItem.value = items[i].value;

                var itemValue = (menuItem.value != "" && parameter.params.type != "DateTime" && parameter.params.type != "DateTimeOffset" && parameter.params.type != "TimeSpan") ? menuItem.value : jsObject.getStringKey(menuItem.key, menuItem.parameter);
                if (valuesHash[itemValue] === true) continue;

                menuParent.paramsItems.push(menuItem);
                menuParent.innerTable.addCellInNextRow(menuItem);

                menuItem.innerTable.addTextCell(itemValue).style.padding = "0 5px 0 5px";
                valuesHash[itemValue] = true;

                menuItem.action = function () {
                    var variableParams = this.parameter.params;
                    variableParams.key = this.key;

                    if (variableParams.type != "Bool") {
                        this.parameter.controls.firstTextBox.value = (variableParams.type == "DateTime" || variableParams.type == "DateTimeOffset" || variableParams.type == "TimeSpan")
                            ? this.parameter.jsObject.getStringKey(this.key, this.parameter)
                            : (variableParams.allowUserValues ? this.key : (this.value != "" ? this.value : this.key));
                    }
                    else {
                        this.parameter.controls.boolCheckBox.setChecked(this.key && this.key.toString().toLowerCase() == "true");
                    }

                    this.parameter.changeVisibleStateMenu(false);

                    if (variableParams.binding) {
                        if (!jsObject.options.paramsVariablesStartValues) {
                            jsObject.options.paramsVariablesStartValues = jsObject.copyObject(jsObject.options.paramsVariables);
                        }
                        jsObject.postInteraction({ action: "InitVars", variables: jsObject.controls.parametersPanel.getParametersValues(), isBindingVariable: true });
                    }
                }

                if (findControl.findTextbox.value != "" && itemValue.toLowerCase().indexOf(findControl.findTextbox.value.toLowerCase()) < 0)
                    menuItem.style.display = "none";

                menuParent.currentLoadedIndex = i;

                if ((i >= startIndex + loadingStep && !loadAll) || i == items.length - 1) {
                    break;
                }
            }
        }

        menuParent.addItems(menuParent.currentLoadedIndex);

        if (menuParent.scrollContainer) {
            menuParent.scrollContainer.onscroll = function () {
                if (menuParent.innerTable.offsetHeight > 0 &&
                    menuParent.scrollContainer.scrollTop > menuParent.innerTable.offsetHeight - menuParent.scrollContainer.offsetHeight * 2 &&
                    menuParent.currentLoadedIndex < items.length - 1) {
                    menuParent.addItems(menuParent.currentLoadedIndex + 1);
                }
            }
        }
    }

    return menuParent;
}

//Menu For Range
StiJsViewer.prototype.parameterMenuForRange = function (parameter) {
    var items = parameter.params.items;
    var menuParent = this.ParameterMenu(parameter);
    menuParent.paramsItems = [];

    if (items) {
        var findControl = this.addFindControlToParameterMenu(menuParent, parameter, items.length < 10);

        menuParent.onshow = function () {
            menuParent.findItems("");
            setTimeout(function () { findControl.findTextbox.focus(); }, 200);
        }

        for (var i = 0; i < items.length; i++) {
            var menuItem = this.parameterMenuItem(parameter);
            menuParent.innerTable.addCellInNextRow(menuItem);
            menuParent.paramsItems.push(menuItem);

            menuItem.id = parameter.jsObject.controls.viewer.id + parameter.params.name + "Item" + i;
            menuItem.parameter = parameter;
            menuItem.value = items[i].value;
            menuItem.key = items[i].key;
            menuItem.keyTo = items[i].keyTo;
            menuItem.innerTable.addTextCell(menuItem.value + " [" + this.getStringKey(menuItem.key, menuItem.parameter) + " - " + this.getStringKey(menuItem.keyTo, menuItem.parameter) + "]").style.padding = "0 5px 0 5px";

            menuItem.action = function () {
                this.parameter.params.key = this.key;
                this.parameter.params.keyTo = this.keyTo;
                this.parameter.controls.firstTextBox.value = this.parameter.jsObject.getStringKey(this.key, this.parameter);
                this.parameter.controls.secondTextBox.value = this.parameter.jsObject.getStringKey(this.keyTo, this.parameter);
                this.parameter.changeVisibleStateMenu(false);
            }
        }
    }

    return menuParent;
}

//Menu For ListNotEdit
StiJsViewer.prototype.parameterMenuForNotEditList = function (parameter) {
    var jsObject = this;
    var loadingStep = 200;
    var items = parameter.params.items;
    var menuParent = this.ParameterMenu(parameter);
    menuParent.paramsItems = [];
    menuParent.currentLoadedIndex = 0;

    if (items) {
        var findControl = this.addFindControlToParameterMenu(menuParent, parameter, items.length < 10);

        var checkBoxSelectAll = this.CheckBox(null, this.collections.loc["SelectAll"]);
        checkBoxSelectAll.style.margin = "8px 7px 8px 7px";
        checkBoxSelectAll.setChecked(true);
        menuParent.checkBoxSelectAll = checkBoxSelectAll;
        menuParent.innerTable.addCellInNextRow(checkBoxSelectAll);
        menuParent.innerTable.addCellInNextRow(this.parameterMenuSeparator());

        checkBoxSelectAll.action = function () {
            for (var i = 0; i < items.length; i++) {
                items[i].isChecked = this.isChecked;
                if (menuParent.paramsItems[i]) {
                    menuParent.paramsItems[i].checkBox.setChecked(this.isChecked, true);
                }
            }
            menuParent.updateParameterValue(!this.isChecked);
            menuParent.isModified = true;
        }

        menuParent.onshow = function () {
            menuParent.isModified = false;
            menuParent.findItems("");
            menuParent.updateItems();
            menuParent.updateParameterValue();
            setTimeout(function () { findControl.findTextbox.focus(); }, 200);
        }

        menuParent.onHide = function () {
            if (menuParent.isModified) {
                this.checkBindingVariables();
            }
        }

        menuParent.updateItems = function (itemIndex) {
            var selectAll = true;
            if (itemIndex != null && this.paramsItems[itemIndex]) {
                items[itemIndex].isChecked = this.paramsItems[itemIndex].checkBox.isChecked;
                if (!items[itemIndex].isChecked) selectAll = false;
            }
            if (selectAll) {
                for (var i = 0; i < this.paramsItems.length; i++) {
                    if (!this.paramsItems[i].checkBox.isChecked) {
                        selectAll = false;
                        break;
                    }
                }
            }
            this.checkBoxSelectAll.setChecked(selectAll);
        }

        menuParent.updateParameterValue = function (isEmpty) {
            var firstTextBox = parameter.controls.firstTextBox;
            firstTextBox.value = "";
            if (isEmpty) return;

            var checkedCount = 0;
            for (var i = 0; i < this.paramsItems.length; i++) {
                if (this.paramsItems[i].checkBox.isChecked) {
                    if (firstTextBox.value != "") {
                        firstTextBox.value += (jsObject.options.listSeparator ? jsObject.options.listSeparator + " " : "; ");
                    }
                    firstTextBox.value += this.paramsItems[i].value != "" ? this.paramsItems[i].value : jsObject.getStringKey(this.paramsItems[i].key, parameter);
                    checkedCount++;
                    if (checkedCount > 30) break;
                }
            }
        }

        menuParent.checkBindingVariables = function () {
            if (this.parameter.params.binding) {
                if (!jsObject.options.paramsVariablesStartValues) {
                    jsObject.options.paramsVariablesStartValues = jsObject.copyObject(jsObject.options.paramsVariables);
                }
                var params = { action: "InitVars", variables: jsObject.controls.parametersPanel.getParametersValues(), isBindingVariable: true };
                jsObject.postInteraction(params);
            }
        }

        var closeButton = this.parameterMenuItem(parameter);
        closeButton.id = jsObject.controls.viewer.id + parameter.params.name + "ItemClose";
        closeButton.innerTable.addTextCell(this.collections.loc["Close"]).style.paddingLeft = "13px";

        closeButton.action = function () {
            this.parameter.changeVisibleStateMenu(false);
        }

        var separator2 = this.parameterMenuSeparator();

        menuParent.addItems = function (startIndex, loadAll) {
            for (var i = startIndex; i < items.length; i++) {
                var itemValue = items[i].value != "" ? items[i].value : jsObject.getStringKey(items[i].key, parameter);
                var menuItem = jsObject.parameterMenuItem(parameter);
                menuItem.id = parameter.jsObject.controls.viewer.id + parameter.params.name + "Item" + i;
                menuItem.value = parameter.params.items[i].value;
                menuItem.key = parameter.params.items[i].key;
                menuItem.parameter = parameter;
                menuParent.innerTable.addCellInNextRow(menuItem);
                menuParent.paramsItems.push(menuItem);

                var checkBox = jsObject.ParameterCheckBox(parameter, itemValue);
                checkBox.style.margin = "0 5px 0 5px";
                checkBox.style.width = "100%";
                checkBox.imageBlock.parentElement.style.width = "1px";
                checkBox.menuParent = menuParent;
                checkBox.setChecked(items[i].isChecked);
                checkBox.itemIndex = i;
                menuItem.checkBox = checkBox;
                menuItem.innerTable.addCell(checkBox);
                menuItem.innerTable.style.width = "100%";

                checkBox.onChecked = function () {
                    menuParent.updateItems(this.itemIndex);
                    menuParent.updateParameterValue();
                    menuParent.isModified = true;
                }

                if (findControl.findTextbox.value != "" && itemValue.toLowerCase().indexOf(findControl.findTextbox.value.toLowerCase()) < 0)
                    menuItem.style.display = "none";

                menuParent.currentLoadedIndex = i;

                if ((i >= startIndex + loadingStep && !loadAll) || i == items.length - 1) {
                    menuParent.innerTable.addCellInNextRow(separator2);
                    menuParent.innerTable.addCellInNextRow(closeButton);
                    break;
                }
            }
        }

        menuParent.addItems(menuParent.currentLoadedIndex);

        if (menuParent.scrollContainer) {
            menuParent.scrollContainer.onscroll = function () {
                if (menuParent.innerTable.offsetHeight > 0 &&
                    menuParent.scrollContainer.scrollTop > menuParent.innerTable.offsetHeight - menuParent.scrollContainer.offsetHeight * 2 &&
                    menuParent.currentLoadedIndex < items.length - 1) {
                    menuParent.addItems(menuParent.currentLoadedIndex + 1);
                }
            }
        }
    }

    return menuParent;
}

//Menu For ListEdit
StiJsViewer.prototype.parameterMenuForEditList = function (parameter) {
    var menuParent = this.ParameterMenu(parameter);

    //New Item Method
    menuParent.newItem = function (item, parameter) {
        var menuItem = parameter.jsObject.parameterMenuItem(parameter);
        menuItem.id = parameter.jsObject.controls.viewer.id + parameter.params.name + "Item" + parameter.jsObject.newGuid().replace(/-/g, '');
        menuItem.onmouseover = null;
        menuItem.onmousedown = null;
        menuItem.ontouchend = null;
        menuItem.action = null;
        menuItem.parameter = parameter;
        menuItem.value = item.value;
        menuItem.key = item.key;

        //Text Box        
        var textBox = parameter.jsObject.ParameterTextBox(parameter);
        menuItem.textBox = textBox;
        textBox.value = parameter.jsObject.getStringKey(menuItem.key, menuItem.parameter);
        textBox.thisMenu = menuParent;
        menuItem.innerTable.addCell(textBox).style.padding = "0 1px 0 5px";

        //DateTime Button
        if (parameter.params.type == "DateTime" || parameter.params.type == "DateTimeOffset") {
            var dateTimeButton = parameter.jsObject.ParameterButton("DateTimeButton", parameter);
            dateTimeButton.id = menuItem.id + "DateTimeButton";
            dateTimeButton.parameter = parameter;
            dateTimeButton.thisItem = menuItem;
            menuItem.innerTable.addCell(dateTimeButton).style.padding = "0 1px 0 1px";

            dateTimeButton.action = function () {
                var datePicker = dateTimeButton.jsObject.controls.datePicker;
                datePicker.value = this.thisItem.key;
                datePicker.parentDateControl = this.thisItem.textBox;
                datePicker.parentButton = this;
                datePicker.changeVisibleState(!datePicker.visible);
            }
        }

        //Guid Button
        if (parameter.params.type == "Guid") {
            var guidButton = parameter.jsObject.ParameterButton("GuidButton", parameter);
            guidButton.id = menuItem.id + "GuidButton";
            guidButton.thisItem = menuItem;
            guidButton.thisMenu = menuParent;
            menuItem.innerTable.addCell(guidButton).style.padding = "0 1px 0 1px";

            guidButton.action = function () {
                this.thisItem.textBox.value = this.parameter.jsObject.newGuid();
                this.thisMenu.updateItems();
                this.thisMenu.isModified = true;
            }
        }

        //Remove Button                        
        var removeButton = parameter.jsObject.ParameterButton("RemoveItemButton", parameter);
        removeButton.id = menuItem.id + "RemoveButton";
        removeButton.itemsContainer = this.itemsContainer;
        removeButton.thisItem = menuItem;
        removeButton.thisMenu = menuParent;
        menuItem.innerTable.addCell(removeButton).style.padding = "0 6px 0 1px";
        removeButton.action = function () {
            this.itemsContainer.removeChild(this.thisItem);
            this.thisMenu.updateItems();
            this.thisMenu.isModified = true;
        }

        return menuItem;
    }

    menuParent.checkBindingVariables = function () {
        if (this.parameter.params.binding) {
            if (!this.jsObject.options.paramsVariablesStartValues) {
                this.jsObject.options.paramsVariablesStartValues = this.jsObject.copyObject(this.jsObject.options.paramsVariables);
            }
            var params = { action: "InitVars", variables: this.jsObject.controls.parametersPanel.getParametersValues(), isBindingVariable: true };
            this.jsObject.postInteraction(params);
        }
    }

    //Update Items
    menuParent.updateItems = function () {
        this.parameter.params.items = [];
        this.parameter.controls.firstTextBox.value = "";
        for (index = 0; index < this.itemsContainer.childNodes.length; index++) {
            var itemMenu = this.itemsContainer.childNodes[index];
            this.parameter.params.items[index] = {};
            this.parameter.params.items[index].key =
                (this.parameter.params.type == "DateTime" || this.parameter.params.type == "DateTimeOffset")
                    ? itemMenu.key
                    : itemMenu.textBox.value;
            this.parameter.params.items[index].value = itemMenu.value;
            if (this.parameter.controls.firstTextBox.value != "") this.parameter.controls.firstTextBox.value += (this.parameter.jsObject.options.listSeparator ? this.parameter.jsObject.options.listSeparator + " " : "; ");
            this.parameter.controls.firstTextBox.value += this.parameter.jsObject.getStringKey(this.parameter.params.items[index].key, this.parameter);
        }

        if (this.parameter.menu.innerTable.offsetHeight > 400) this.parameter.menu.style.height = "350px;"
        else this.parameter.menu.style.height = this.parameter.menu.innerTable.offsetHeight + "px";
    }

    //New Item Button
    var newItemButton = this.parameterMenuItem(parameter);
    menuParent.innerTable.addCell(newItemButton);
    newItemButton.id = parameter.jsObject.controls.viewer.id + parameter.params.name + "ItemNew";
    newItemButton.innerTable.addTextCell(this.collections.loc["NewItem"]).style.padding = "0 5px 0 5px";
    newItemButton.thisMenu = menuParent;
    newItemButton.action = function () {
        var item_ = {};
        if (this.parameter.params.type == "DateTime" || this.parameter.params.type == "DateTimeOffset") {
            item_.key = this.parameter.jsObject.getDateTimeObject();
            item_.value = this.parameter.jsObject.dateTimeObjectToString(item_.key, this.parameter);
        }
        else if (this.parameter.params.type == "TimeSpan") {
            item_.key = "00:00:00";
            item_.value = "00:00:00";
        }
        else if (this.parameter.params.type == "Bool") {
            item_.key = "False";
            item_.value = "False";
        }
        else {
            item_.key = "";
            item_.value = "";
        }
        var newItem = this.thisMenu.newItem(item_, this.parameter);
        this.thisMenu.itemsContainer.appendChild(newItem);
        if ("textBox" in newItem) newItem.textBox.focus();
        this.thisMenu.updateItems();
        this.thisMenu.isModified = true;
    }

    //Add Items
    menuParent.itemsContainer = menuParent.innerTable.addCellInNextRow();

    if (parameter.params.items) {
        for (var index = 0; index < parameter.params.items.length; index++) {
            menuParent.itemsContainer.appendChild(menuParent.newItem(parameter.params.items[index], parameter));
        }
    }

    var cellDown = menuParent.innerTable.addCellInNextRow();

    //Remove All Button
    var removeAllButton = this.parameterMenuItem(parameter);
    cellDown.appendChild(removeAllButton);
    removeAllButton.id = parameter.jsObject.controls.viewer.id + parameter.params.name + "ItemRemoveAll";
    removeAllButton.innerTable.addTextCell(this.collections.loc["RemoveAll"]).style.padding = "0 5px 0 5px";
    removeAllButton.thisMenu = menuParent;
    removeAllButton.action = function () {
        while (this.thisMenu.itemsContainer.childNodes[0]) {
            this.thisMenu.itemsContainer.removeChild(this.thisMenu.itemsContainer.childNodes[0]);
        }
        this.thisMenu.updateItems();
        this.thisMenu.isModified = true;
    }

    //Close Button
    cellDown.appendChild(this.parameterMenuSeparator());
    var closeButton = this.parameterMenuItem(parameter);
    cellDown.appendChild(closeButton);
    closeButton.id = parameter.jsObject.controls.viewer.id + parameter.params.name + "ItemClose";
    closeButton.innerTable.addTextCell(this.collections.loc["Close"]).style.padding = "0 5px 0 5px";
    closeButton.action = function () { this.parameter.changeVisibleStateMenu(false); }

    menuParent.onHide = function () {
        this.updateItems();
        if (this.isModified) {
            this.checkBindingVariables();
        }
    }

    return menuParent;
}

StiJsViewer.prototype.ReplaceMonths = function (value) {
    for (var i = 1; i <= 12; i++) {
        var enName = "";
        var locName = "";
        switch (i) {
            case 1:
                enName = "January";
                locName = this.collections.loc.MonthJanuary;
                break;

            case 2:
                enName = "February";
                locName = this.collections.loc.MonthFebruary;
                break;

            case 3:
                enName = "March";
                locName = this.collections.loc.MonthMarch;
                break;

            case 4:
                enName = "April";
                locName = this.collections.loc.MonthApril;
                break;

            case 5:
                enName = "May";
                locName = this.collections.loc.MonthMay;
                break;

            case 6:
                enName = "June";
                locName = this.collections.loc.MonthJune;
                break;

            case 7:
                enName = "July";
                locName = this.collections.loc.MonthJuly;
                break;

            case 8:
                enName = "August";
                locName = this.collections.loc.MonthAugust;
                break;

            case 9:
                enName = "September";
                locName = this.collections.loc.MonthSeptember;
                break;

            case 10:
                enName = "October";
                locName = this.collections.loc.MonthOctober;
                break;

            case 11:
                enName = "November";
                locName = this.collections.loc.MonthNovember;
                break;

            case 12:
                enName = "December";
                locName = this.collections.loc.MonthDecember;
                break;
        }

        var enShortName = enName.substring(0, 3);
        var locShortName = locName.substring(0, 3);
        value = value.replace(enName, i).replace(enName.toLowerCase(), i).replace(enShortName, i).replace(enShortName.toLowerCase(), i);
        value = value.replace(locName, i).replace(locName.toLowerCase(), i).replace(locShortName, i).replace(locShortName.toLowerCase(), i);

    }

    return value;
}

StiJsViewer.prototype.GetDateTimeFromString = function (value, format) {
    var charIsDigit = function (char) {
        return ("0123456789".indexOf(char) >= 0);
    }

    if (!value) return new Date();
    value = this.ReplaceMonths(value);

    var dateTime = new Date();

    // If the date format is not specified, then deserializator for getting date and time is applied
    if (format == null) format = "dd.MM.yyyy hh:mm:ss";
    // Otherwise the format is parsed. Now only numeric date and time formats are supported

    var year = 1970;
    var month = 1;
    var day = 1;
    var hour = 0;
    var minute = 0;
    var second = 0;
    var millisecond = 0;

    var char = "";
    var pos = 0;
    var values = [];

    // Parse date and time into separate numeric values
    while (pos < value.length) {
        char = value.charAt(pos);
        if (charIsDigit(char)) {
            values.push(char);
            pos++;

            while (pos < value.length && charIsDigit(value.charAt(pos))) {
                values[values.length - 1] += value.charAt(pos);
                pos++;
            }

            values[values.length - 1] = this.StrToInt(values[values.length - 1]);
        }

        pos++;
    }

    pos = 0;
    var charCount = 0;
    var index = -1;
    var is12hour = false;

    // Parsing format and replacement of appropriate values of date and time
    while (pos < format.length) {
        char = format.charAt(pos);
        charCount = 0;

        if (char == "Y" || char == "y" || char == "M" || char == "d" || char == "h" || char == "H" ||
            char == "m" || char == "s" || char == "f" || char == "F" || char == "t" || char == "z") {
            index++;

            while (pos < format.length && format.charAt(pos) == char) {
                pos++;
                charCount++;
            }
        }

        switch (char) {
            case "Y": // full year
                year = values[index];
                break;

            case "y": // year
                if (values[index] < 1000) year = 2000 + values[index];
                else year = values[index];
                break;

            case "M": // month
                month = values[index];
                break;

            case "d": // day
                day = values[index];
                break;

            case "h": // (hour 12)
                is12hour = true;
                hour = values[index];
                break;

            case "H": // (hour 24)
                hour = values[index];
                break;

            case "m": // minute
                minute = values[index];
                break;

            case "s": // second
                second = values[index];
                break;

            case "f": // second fraction
            case "F": // second fraction, trailing zeroes are trimmed
                millisecond = values[index];
                break;

            case "t": // PM or AM
                if (value.toLowerCase().indexOf("am") >= 0 && hour == 12) hour = 0;
                if (value.toLowerCase().indexOf("pm") >= 0 && hour < 12) hour += 12;
                break;

            default:
                pos++;
                break;
        }
    }

    if (day > 31 && !year && !month) {
        var dayStr = day.toString();
        if (dayStr.length >= 4) year = parseInt(dayStr.substring(dayStr.length - 4));
        if (dayStr.length >= 6) month = parseInt(dayStr.substring(dayStr.length - 6, dayStr.length - 4));
        if (dayStr.length >= 7) day = parseInt(dayStr.substring(0, dayStr.length - 6));
    }

    dateTime = new Date(year || new Date().getFullYear(), month - 1 || 0, day || 1, hour || 0, minute || 0, second || 0, millisecond || 0);

    if (!dateTime || isNaN(dateTime)) return new Date();

    return dateTime;
}

StiJsViewer.prototype.ParameterCategoryButton = function (caption, isOpened) {
    var jsObject = this;
    var button = this.FormButton(null, caption, isOpened ? "VariableCategory.Minus.png" : "VariableCategory.Plus.png", null, "stiJsViewerGroupHeaderButton");
    button.isOpened = isOpened;
    button.caption.style.textAlign = "left";

    button.action = function () {
        this.isOpened = !this.isOpened;
        StiJsViewer.setImageSource(this.image, jsObject.options, jsObject.collections, this.isOpened ? "VariableCategory.Minus.png" : "VariableCategory.Plus.png");
        this.onAction();
    }

    button.onAction = function () { }

    return button;
}