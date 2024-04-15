
StiMobileDesigner.prototype.DataGrid = function (width, height, isViewQuery) {
    var grid = document.createElement("div");
    var jsObject = grid.jsObject = this;
    grid.className = "stiDataGrid";
    if (width) grid.style.width = width + "px";
    if (height) grid.style.height = height + "px";
    if (isViewQuery) grid.style.border = "0px";

    grid.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        grid.data = null;
    }

    grid.showData = function (data, sortLabels, filterLabels) {
        grid.data = data;
        if (!data) return;

        if (data.length < 2) {
            grid.clear();
            var noDataPanel = document.createElement("div");
            noDataPanel.innerHTML = jsObject.loc.Errors.DataNotFound;
            noDataPanel.className = "stiCreateDataHintText";
            noDataPanel.setAttribute("style", "position:absolute; width:300px; left:calc(50% - 150px); top:50%;");
            grid.appendChild(noDataPanel);
        }

        var headerTable = jsObject.CreateHTMLTable();
        var headerTableRow = headerTable.tr[0];
        this.appendChild(headerTable);

        var scrollContainer = document.createElement("div");
        scrollContainer.style.overflowY = "auto";
        scrollContainer.style.overflowX = "hidden";
        scrollContainer.style.height = "calc(100% - " + (jsObject.options.isTouchDevice ? "30px" : "26px") + ")";
        this.appendChild(scrollContainer);

        var dataTable = jsObject.CreateHTMLTable();
        var dataTableRow = dataTable.tr[0];
        scrollContainer.appendChild(dataTable);

        var headerButtons = [];

        if (data.length > 0) {
            for (var i = 0; i < data[0].length; i++) {
                var headerButton = jsObject.GridHeaderButton(data[0][i], isViewQuery);
                headerButton.columnName = data[0][i];
                headerButton.columnIndex = i;
                var headerCell = headerTable.addCell(headerButton);
                headerCell.className = "stiDesignerDataGridCell";
                headerCell.style.padding = "0";
                headerCell.style.background = "#f0f0f0";
                headerButtons.push(headerButton);

                headerButton.action = function () {
                    grid.action(this);
                }

                headerButton.onclick = null;

                headerButton.onmouseup = function () {
                    this.action();
                }

                if (grid.selectedHeaderButton && grid.selectedHeaderButton.columnName == data[0][i]) {
                    headerButton.setSelected(true);
                    grid.selectedHeaderButton = headerButton;
                }
            }
        }

        //set header sort and filter labels
        if (sortLabels) {
            var sortCounter = 1;
            for (var i = 0; i < sortLabels.length; i++) {
                if (sortLabels[i].columnIndex < headerButtons.length) {
                    headerButtons[sortLabels[i].columnIndex].showSortDirection(sortLabels[i].direction, sortLabels.length > 1 ? sortCounter : null);
                    sortCounter++;
                }
            }
        }

        if (filterLabels) {
            for (var i = 0; i < filterLabels.length; i++) {
                if (filterLabels[i].columnIndex < headerButtons.length) {
                    headerButtons[filterLabels[i].columnIndex].showFilter(true);
                }
            }
        }

        if (data.length > 1) {
            for (var i = 1; i < data.length; i++) {
                for (var k = 0; k < data[i].length; k++) {
                    var cell = dataTable.addCellInLastRow();
                    cell.className = "stiDesignerDataGridCell";
                    cell.style.background = i % 2 == 0 ? "#f5f5f5" : "#ffffff";
                    cell.style.whiteSpace = "nowrap";

                    if (data[i][k].type == "Image" || data[i][k].type == "image") {
                        var img = document.createElement("img");
                        img.style.maxWidth = img.style.maxHeight = "60px";
                        img.src = data[i][k].displayString;
                        cell.style.width = cell.style.height = "60px";
                        cell.style.textAlign = "center";
                        cell.appendChild(img);
                    }
                    else if (data[i][k].type == "Boolean" || data[i][k].type == "bool") {
                        var checkBox = jsObject.CheckBox();
                        checkBox.setChecked(data[i][k].displayString && data[i][k].displayString.toString().toLowerCase() == "true");
                        checkBox.onmouseover = null;
                        checkBox.onmouseout = null;
                        checkBox.onmouseenter = null;
                        checkBox.onmouseleave = null;
                        checkBox.onclick = null;
                        checkBox.style.display = "inline-block";
                        cell.style.width = "50px";
                        cell.style.textAlign = "center";
                        cell.appendChild(checkBox);
                    }
                    else {
                        cell.innerHTML = data[i][k].displayString != null ? data[i][k].displayString : "";
                    }
                }
                dataTable.addRow();
            }
        }

        if (headerTableRow.childNodes.length == 0 ||
            dataTableRow.childNodes.length == 0 ||
            headerTableRow.childNodes.length != dataTableRow.childNodes.length)
            return;

        for (var i = 0; i < dataTableRow.childNodes.length; i++) {
            var width = Math.max(dataTableRow.childNodes[i].offsetWidth, headerTableRow.childNodes[i].offsetWidth);
            if (width > 300) width = 300;

            var headerButton = headerTableRow.childNodes[i].firstChild;
            headerButton.style.width = width + "px";

            if (headerButton.caption.offsetWidth > headerButton.offsetWidth && headerButton.caption.offsetWidth != 0) {
                width = headerButton.caption.offsetWidth + 50;
                headerButton.style.width = width + "px";
            }

            for (var k = 0; k < dataTable.tr.length; k++) {
                if (i < dataTable.tr[k].childNodes.length) {
                    var cell = dataTable.tr[k].childNodes[i];

                    var divCell = document.createElement("div");
                    divCell.style.width = (width + 2) + "px";
                    divCell.style.overflow = "hidden";
                    divCell.style.whiteSpace = "normal";

                    divCell.innerHTML = cell.innerHTML;
                    cell.innerHTML = "";
                    cell.appendChild(divCell);
                }
            }
        }

        scrollContainer.style.width = headerTable.offsetWidth + "px";
    }

    grid.action = function () {

    }

    return grid;
}

StiMobileDesigner.prototype.GridHeaderButton = function (caption, isViewQuery) {
    var button = this.SmallButton(null, null, caption, null, null, "Down");
    button.sort = null;
    button.filter = null;
    button.style.overflow = "hidden";
    button.style.padding = "0 4px 0 4px";

    var innerTable = this.CreateHTMLTable();
    innerTable.style.height = "100%";
    innerTable.style.width = "100%";
    button.removeChild(button.innerTable);
    button.appendChild(innerTable);

    var sortImage = document.createElement("img");
    sortImage.style.marginLeft = "4px";
    sortImage.style.width = "5px";
    sortImage.style.height = "12px";
    StiMobileDesigner.setImageSource(sortImage, this.options, "DataTransformation.SortAsc.png");
    innerTable.addCell(sortImage);
    sortImage.style.visibility = "hidden";

    var sortNumber = document.createElement("div");
    var numberCell = innerTable.addCell(sortNumber);
    sortNumber.style.width = "6px";
    sortNumber.style.marginRight = "4px";
    sortNumber.style.fontSize = "8px";
    sortNumber.style.color = "#8a8a8a";
    sortNumber.style.textAlign = "right";
    numberCell.style.verticalAlign = "bottom";
    numberCell.style.paddingBottom = "4px";

    button.caption = innerTable.addTextCell(caption);
    button.caption.style.whiteSpace = "nowrap";
    button.caption.style.textAlign = "center";
    button.caption.style.width = "100%";

    var filterImage = document.createElement("img");
    filterImage.style.width = "16px";
    filterImage.style.height = "16px";
    filterImage.style.margin = "2px 2px 0 2px";
    StiMobileDesigner.setImageSource(filterImage, this.options, "Arrows.ArrowDown.png");
    filterImage.style.visibility = "hidden";
    innerTable.addCell(filterImage);

    button.onmouseenter = function () {
        if (this.jsObject.options.isTouchClick) return;
        this.className = this.overClass;
        this.isOver = true;
        if (!isViewQuery) filterImage.style.visibility = "visible";
    }

    button.onmouseleave = function () {
        this.isOver = false;
        this.className = this.isSelected ? this.selectedClass : this.defaultClass;
        if (!isViewQuery && !this.filter) filterImage.style.visibility = "hidden";
    }

    button.showSortDirection = function (sortDirection, number) {
        sortImage.style.visibility = sortDirection == "Ascending" || sortDirection == "Descending" ? "visible" : "hidden";
        StiMobileDesigner.setImageSource(sortImage, this.jsObject.options, sortDirection == "Ascending" ? "DataTransformation.SortAsc.png" : "DataTransformation.SortDesc.png");
        sortNumber.innerHTML = number || "";
        this.sortDirection = sortDirection;
    }

    button.hideSortDirection = function () {
        sortImage.style.visibility = "hidden";
        sortNumber.innerHTML = "";
    }

    button.showFilter = function (filter) {
        StiMobileDesigner.setImageSource(filterImage, this.jsObject.options, filter ? "DataTransformation.Filter.png" : "Arrows.ArrowDown.png");
        filterImage.style.width = filter ? "11px" : "16px";
        filterImage.style.height = filter ? "8px" : "16px";
        filterImage.style.visibility = filter ? "visible" : "hidden";
        filterImage.style.margin = filter ? "0 4px 0 5px" : "2px 2px 0 2px";
        this.filter = filter;
    }

    return button;
}