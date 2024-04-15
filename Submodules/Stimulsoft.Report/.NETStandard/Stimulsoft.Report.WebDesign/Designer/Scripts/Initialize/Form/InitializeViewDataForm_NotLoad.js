
StiMobileDesigner.prototype.InitializeViewDataForm_ = function () {
    var viewDataForm = this.BaseForm("viewDataForm", this.loc.FormDictionaryDesigner.ViewData, 4);
    viewDataForm.buttonCancel.style.display = "none";
    viewDataForm.container.style.maxWidth = "900px";
    viewDataForm.container.style.minWidth = "500px";
    viewDataForm.container.style.height = "500px";
    viewDataForm.container.style.overflowY = "hidden";
    viewDataForm.container.style.overflowX = "auto";

    var jsObject = this;
    var countVisibleRows = 5000;

    viewDataForm.show = function (params) {
        while (this.container.childNodes[0]) this.container.removeChild(this.container.childNodes[0]);

        var dataSourceName = params.typeItem == "BusinessObject" ? params.fullName : (params.oldName || params.name);
        viewDataForm.caption.innerText = viewDataForm.jsObject.loc.FormDictionaryDesigner.ViewData + (dataSourceName ? " - " + dataSourceName : "");
        viewDataForm.changeVisibleState(true);

        jsObject.AddProgressToControl(viewDataForm.container);

        params.command = "ViewData";
        params.isAsyncCommand = true;

        setTimeout(function () {
            viewDataForm.container.progress.show();

            jsObject.SendCommandToDesignerServer("ViewData", params, function (answer) {
                if (!viewDataForm.visible) return;

                var data = answer.resultData;

                if (!data) {
                    viewDataForm.container.progress.hide();
                    return;
                }

                var dataTableHead = jsObject.CreateHTMLTable();
                dataTableHead.id = "stiDataTableHead";
                var tableHeadRow = dataTableHead.tr[0];
                viewDataForm.container.appendChild(dataTableHead);

                var scrollContainer = document.createElement("div");
                scrollContainer.id = "stiScrollContainer";
                scrollContainer.style.overflowY = "auto";
                scrollContainer.style.overflowX = "hidden";
                scrollContainer.style.height = "calc(100% - 20px)";
                viewDataForm.container.appendChild(scrollContainer);

                var dataTable = jsObject.CreateHTMLTable();
                dataTable.id = "stiDataTable";
                var tableBodyRow = dataTable.tr[0];
                tableBodyRow.id = "stiTableBodyRow";
                scrollContainer.appendChild(dataTable);

                var checkDisplaingRows = function () {
                    if (viewDataForm.hideRows) {
                        clearTimeout(scrollContainer.scrollTimer);

                        scrollContainer.scrollTimer = setTimeout(function () {
                            var commonRowsHeight = 0;
                            for (var i = 0; i < dataTable.rows.length; i++) {
                                commonRowsHeight += dataTable.rows[i].offsetHeight;
                                if (commonRowsHeight > scrollContainer.scrollTop) {
                                    var startIndex = Math.max(i - 5, 0);
                                    var endIndex = Math.min(startIndex + 100, dataTable.rows.length);
                                    for (var k = startIndex; k < endIndex; k++) {
                                        var row = dataTable.rows[k];
                                        if (row.style.display == "none") {
                                            row.style.display = "";
                                        }
                                    }
                                    break;
                                }
                            }
                        }, 20);
                    }
                }

                var updateCellsWidth = function () {
                    if (tableHeadRow.childNodes.length == 0 ||
                        tableBodyRow.childNodes.length == 0 ||
                        tableHeadRow.childNodes.length != tableBodyRow.childNodes.length)
                        return;

                    for (var i = 0; i < tableBodyRow.childNodes.length; i++) {
                        var width = Math.max(tableBodyRow.childNodes[i].offsetWidth, tableHeadRow.childNodes[i].offsetWidth);
                        if (width > 500) width = Math.min(tableBodyRow.childNodes[i].offsetWidth, tableHeadRow.childNodes[i].offsetWidth);
                        if (width > 500) width = 500;
                        var divHeader = document.createElement("div");
                        var divBody = document.createElement("div");
                        divHeader.style.width = divBody.style.width = width + "px";
                        divHeader.style.overflow = "hidden";
                        divBody.style.overflow = "hidden";
                        divHeader.innerHTML = tableHeadRow.childNodes[i].innerHTML;
                        divBody.innerHTML = tableBodyRow.childNodes[i].innerHTML;
                        tableHeadRow.childNodes[i].innerHTML = tableBodyRow.childNodes[i].innerHTML = "";
                        tableHeadRow.childNodes[i].appendChild(divHeader);
                        tableBodyRow.childNodes[i].appendChild(divBody);
                    }

                    if (dataTableHead.offsetWidth < 450) {
                        scrollContainer.style.width = dataTableHead.style.width = dataTable.style.width = "600px";
                    }
                    else {
                        scrollContainer.style.width = (dataTableHead.offsetWidth) + "px";
                    }
                }

                scrollContainer.onscroll = function () {
                    checkDisplaingRows();
                }

                var cuttingCells = {};

                if (data.length > 1) {
                    for (var i = 0; i < data[0].length; i++) {
                        var headCell = dataTableHead.addTextCell(data[0][i]);
                        headCell.style.fontWeight = "bold";
                        headCell.style.textAlign = "center";
                        headCell.className = "stiDesignerViewDataTableCell";
                    }

                    for (var i = 1; i < data.length; i++) {
                        for (var k = 0; k < data[i].length; k++) {
                            var cell = dataTable.addCellInLastRow();
                            cell.className = "stiDesignerViewDataTableCell";

                            if (data[i][k].type == "Image" || data[i][k].type == "image") {
                                var img = document.createElement("img");
                                img.style.maxWidth = "60px";
                                img.style.maxHeight = "60px";
                                img.src = data[i][k].value;
                                cell.style.width = "60px";
                                cell.style.height = "60px";
                                cell.style.textAlign = "center";
                                cell.appendChild(img);
                            }
                            else if (data[i][k].type == "Boolean" || data[i][k].type == "bool") {
                                var checkBox = jsObject.CheckBox();
                                checkBox.setChecked(data[i][k].value && data[i][k].value.toString().toLowerCase() == "true");
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
                                if (data[i][k].value.length > 40 || cuttingCells[k] != null) {
                                    cuttingCells[k] = true;
                                    var div = document.createElement("div");
                                    div.style.width = "250px";
                                    div.style.overflow = "hidden";
                                    div.style.textOverflow = "ellipsis";
                                    div.innerText = data[i][k].value.substring(0, 150);
                                    cell.appendChild(div);
                                    cell.style.width = "250px";
                                }
                                else {
                                    cell.innerText = data[i][k].value;
                                }
                            }
                        }
                        var row = dataTable.addRow();
                                                
                        if (i > countVisibleRows) {
                            row.style.display = "none";
                        }

                        viewDataForm.hideRows = data.length > countVisibleRows;
                    }
                }

                try {
                    var win = jsObject.openNewWindow();

                    if (win && win.document) {
                        updateCellsWidth();

                        win.document.write("<script>document.title = '" + viewDataForm.jsObject.loc.FormDictionaryDesigner.ViewData + (dataSourceName ? " - " + dataSourceName : "") +
                            "';</script><style> .stiDesignerViewDataTableCell { font-family: Arial; font-size: 12px; padding: 4px 15px 4px 4px; color: #444444;" +
                            "border-bottom: 1px dotted #c6c6c6; border-right: 1px dotted #c6c6c6; } </style>");
                        win.document.write(viewDataForm.container.innerHTML);

                        var body = win.document.getElementsByTagName("body")[0];
                        body.style.overflow = "hidden";
                        body.style.overflowY = "hidden";
                        body.style.overflowX = "auto";

                        var scrollCont = win.document.getElementById("stiScrollContainer");
                        if (scrollCont) {
                            scrollContainer.onscroll = null;
                            scrollContainer = scrollCont;
                            dataTableHead = win.document.getElementById("stiDataTableHead");
                            dataTable = win.document.getElementById("stiDataTable");
                            tableBodyRow = win.document.getElementById("stiTableBodyRow");

                            scrollCont.onscroll = function () {
                                checkDisplaingRows();                                
                            }
                        }

                        viewDataForm.changeVisibleState(false);
                    }
                    else {
                        viewDataForm.container.progress.hide();
                        updateCellsWidth();
                        return;
                    }
                }
                catch (e) {
                    viewDataForm.container.progress.hide();
                    updateCellsWidth();
                }

                jsObject.SetObjectToCenter(viewDataForm);
            });
        }, jsObject.options.formAnimDuration);
    }

    viewDataForm.onhide = function () {
        if (jsObject.options.processImageStatusPanel) jsObject.options.processImageStatusPanel.hide();
        // eslint-disable-next-line no-undef
        if (jsObject.options.jsMode && typeof (StiDesigner) != "undefined" && StiDesigner.asyncPromise != null) {
            // eslint-disable-next-line no-undef
            StiDesigner.asyncPromise.abort();
        }
    }

    viewDataForm.action = function () {
        viewDataForm.changeVisibleState(false);
    }

    return viewDataForm;
}