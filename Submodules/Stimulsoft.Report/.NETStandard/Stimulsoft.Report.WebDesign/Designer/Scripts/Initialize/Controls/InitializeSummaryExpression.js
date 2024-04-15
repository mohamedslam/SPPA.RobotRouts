
StiMobileDesigner.prototype.SummaryExpression = function (name) {
    var sumExp = document.createElement("div");
    sumExp.name = name;
    sumExp.controls = {};
    sumExp.jsObject = this;

    var expTextArea = this.TextArea(null, null, 50);
    expTextArea.style.width = "calc(100% - 31px)";
    expTextArea.style.margin = "12px 12px 6px 12px";
    sumExp.appendChild(expTextArea);
    sumExp.controls.expressionTextArea = expTextArea;

    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.width = "100%";
    sumExp.appendChild(controlsTable);

    var controlProps = [
        ["summaryFunction", this.loc.FormSystemTextEditor.LabelSummaryFunction.replace(":", ""), this.DropDownList(name + "SummaryFunction", 334, null, this.GetTotalFuntionItems(true), true, false, null, true), "6px 12px 6px 25px"],
        ["dataBand", this.loc.FormSystemTextEditor.LabelDataBand.replace(":", ""), this.DropDownList(name + "DataBand", 334, null, null, true, false, null, true), "6px 12px 6px 25px"],
        ["dataColumn", this.loc.FormSystemTextEditor.LabelDataColumn.replace(":", ""), this.DataControl(name + "DataColumn", 334), "6px 12px 6px 25px"],
        ["summaryRunning", this.loc.FormSystemTextEditor.SummaryRunning, this.DropDownList(name + "DataBand", 334, null, this.GetSummaryRunningItems(), true, false, null, true), "6px 12px 6px 25px"],
        ["conditionText", this.loc.FormSystemTextEditor.Condition, this.ExpressionControl(name + "ConditionText", 334, null, null, true), "6px 12px 6px 0"],
        ["condition", " ", this.CheckBox(name + "Condition"), this.options.isTouchDevice ? "0 3px 10px 0" : "0 6px 10px 0"],
        ["runningTotal", " ", this.CheckBox(name + "RunningTotal", this.loc.FormSystemTextEditor.RunningTotal), "8px 12px 6px 25px"]
    ]

    for (var i = 0; i < controlProps.length; i++) {
        var control = controlProps[i][2];
        if (controlProps[i][3]) control.style.margin = controlProps[i][3];
        sumExp.controls[controlProps[i][0]] = control;

        var textCell = controlsTable.addCellInNextRow();
        textCell.innerHTML = controlProps[i][1];
        textCell.className = "stiDesignerCaptionControlsBigIntervals";
        textCell.style.padding = "0 0 0 12px";
        textCell.style.whiteSpace = "normal";
        textCell.style.minWidth = "160px";
        controlsTable.addCellInLastRow(control);

        if ("action" in control) control.action = function () {
            expTextArea.value = sumExp.getValue();
        }
    }

    sumExp.controls.condition.parentElement.display = "none";
    sumExp.controls.condition.style.display = "inline-block";

    var condText = sumExp.controls.conditionText;
    condText.style.display = "inline-block";
    condText.parentElement.style.whiteSpace = "nowrap";
    condText.parentElement.insertBefore(sumExp.controls.condition, condText);


    sumExp.controls.condition.action = function () {
        sumExp.controls.conditionText.setEnabled(this.isChecked);
        expTextArea.value = sumExp.getValue();
    }

    sumExp.reset = function () {
        var dataBandItems = this.jsObject.GetDataBandItems();
        sumExp.controls.dataBand.items = dataBandItems;
        sumExp.controls.dataBand.menu.addItems(dataBandItems);
        sumExp.controls.dataBand.setKey("NotAssigned");
        sumExp.controls.conditionText.setEnabled(false);
        sumExp.controls.conditionText.textBox.value = "";
        sumExp.controls.condition.setChecked(false);
        sumExp.controls.runningTotal.setChecked(false);
        sumExp.controls.summaryFunction.setKey("Sum");
        sumExp.controls.dataColumn.textBox.value = "";
        sumExp.controls.summaryRunning.setKey("report");
        sumExp.controls.summaryRunning.action();
    }

    sumExp.fill = function (text) {
        sumExp.reset();
        if (!text) return;
        var startIndex = text.indexOf('(');
        var endIndex = text.lastIndexOf(')');

        if ((endIndex - startIndex) > 0) {
            var args = text.substr(startIndex + 1, endIndex - startIndex - 1);
            var strs = args.split(",");

            var funcStart = text.indexOf("{");
            if (funcStart != -1) {
                var funcEnd = text.indexOf("(", funcStart);
                if ((funcEnd - funcStart - 1) > 0) {
                    var func = text.substr(funcStart + 1, funcEnd - funcStart - 1);

                    var isRunningTotal = false;
                    var isCondition = false;

                    if (this.jsObject.EndsWith(func, "If")) {
                        func = func.substr(0, func.length - 2);
                        isCondition = true;
                    }
                    if (this.jsObject.EndsWith(func, "Running")) {
                        func = func.substr(0, func.length - 7);
                        isRunningTotal = true;
                    }
                    if (this.jsObject.EndsWith(func, "If")) {
                        func = func.substr(0, func.length - 2); //Second check, do not remove it
                        isCondition = true;
                    }

                    for (var i = 0; i < this.jsObject.options.aggrigateFunctions.length; i++) {
                        var srv = this.jsObject.options.aggrigateFunctions[i];
                        var result = false;
                        if (srv.serviceName == func) result = true;
                        if (("c" + srv.serviceName) == func) result = true;
                        if (("col" + srv.serviceName) == func) result = true;

                        if (result) {
                            sumExp.controls.summaryFunction.setKey(srv.serviceName);
                            if (text.indexOf("col" + srv.serviceName) != -1) {
                                sumExp.controls.summaryRunning.setKey("column");
                                sumExp.controls.summaryRunning.action();
                            }
                            if (text.indexOf('c' + srv.serviceName) != -1) {
                                sumExp.controls.summaryRunning.setKey("page");
                                sumExp.controls.summaryRunning.action();
                            }
                            sumExp.controls.condition.setChecked(isCondition);
                            sumExp.controls.conditionText.setEnabled(isCondition);
                            sumExp.controls.runningTotal.setChecked(isRunningTotal);
                            if (srv.recureParam) {
                                var strsLength = strs.length;
                                if (isCondition && strsLength > 0) {
                                    sumExp.controls.conditionText.textBox.value = strs[strsLength - 1];
                                    strsLength--;
                                }

                                if (func == "Count") {
                                    if (strsLength == 1) {
                                        sumExp.controls.dataBand.setKey(strs[0]);
                                    }
                                }
                                else {
                                    if (strsLength == 1) {
                                        if (sumExp.controls.dataBand.haveKey(strs[0]))
                                            sumExp.controls.dataBand.setKey(strs[0]);
                                        else
                                            sumExp.controls.dataColumn.textBox.value = strs[0];
                                    }
                                    else if (strsLength == 2) {
                                        sumExp.controls.dataBand.setKey(strs[0]);
                                        sumExp.controls.dataColumn.textBox.value = strs[1];
                                    }
                                }
                            }
                            else {
                                if (strs.length == 1) {
                                    if (isCondition) {
                                        sumExp.controls.conditionText.textBox.value = strs[0];
                                    }
                                    else
                                        sumExp.controls.dataBand.setKey(strs[0]);
                                }
                                else if (strs.length == 2) {
                                    sumExp.controls.dataBand.setKey(strs[0]);
                                    sumExp.controls.conditionText.textBox.value = strs[1];
                                }
                            }
                        }
                    }
                }
            }
        }
        expTextArea.value = sumExp.getValue();
    }

    sumExp.getValue = function () {
        var text = "";
        if (sumExp.controls.summaryFunction.key) {
            var funcName = sumExp.controls.summaryFunction.key;

            if (sumExp.controls.summaryRunning.key == "page") funcName = "c" + funcName;
            else if (sumExp.controls.summaryRunning.key == "column") funcName = "col" + funcName;

            if (sumExp.controls.condition.isChecked) funcName += "If";
            if (sumExp.controls.runningTotal.isChecked) funcName += "Running";

            funcName += "(";
            if (sumExp.controls.summaryFunction.key != "Count") {
                var flag = false;
                if (sumExp.controls.dataBand.key.length > 0 && sumExp.controls.dataBand.key != "NotAssigned") {
                    funcName += sumExp.controls.dataBand.key;
                    flag = true;
                }
                if (sumExp.controls.dataColumn.textBox.value.length > 0) {
                    funcName += (flag ? "," : "") + sumExp.controls.dataColumn.textBox.value;
                    flag = true;
                }
                if (sumExp.controls.condition.isChecked) {
                    funcName += (flag ? "," : "") + sumExp.controls.conditionText.textBox.value;
                }
            }
            else {
                var flag = false;
                if (sumExp.controls.dataBand.key.length > 0 && sumExp.controls.dataBand.key != "NotAssigned") {
                    funcName += sumExp.controls.dataBand.key;
                    flag = true;
                }
                if (sumExp.controls.condition.isChecked) funcName += (flag ? "," : "") + sumExp.controls.conditionText.textBox.value;
            }
            funcName += ")";

            text = "{" + funcName + "}";
        }
        return text;
    }

    return sumExp;
}