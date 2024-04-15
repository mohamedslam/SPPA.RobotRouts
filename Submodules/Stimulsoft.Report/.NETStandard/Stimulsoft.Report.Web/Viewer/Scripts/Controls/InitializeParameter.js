
StiJsViewer.prototype.CreateParameter = function (params) {
    var parameter = this.CreateHTMLTable();
    this.options.parameters[params.name] = parameter;
    parameter.params = params;
    parameter.controls = {};
    parameter.jsObject = this;
    parameter.params.isNull = false;
    parameter.menu = null;

    parameter.addCell = function (control) {
        var cell = document.createElement("td");
        cell.style.height = parameter.jsObject.options.parameterRowHeight + "px";
        cell.style.padding = "0px 2px 0 2px";
        this.tr[0].appendChild(cell);
        if (control) cell.appendChild(control);

        return cell;
    }

    parameter.oldAddCellInNextRow = parameter.addCellInNextRow;
    parameter.addCellInNextRow = function (control) {
        var cell = this.oldAddCellInNextRow(control);
        cell.style.height = parameter.jsObject.options.parameterRowHeight + "px";
        cell.style.padding = "0px 2px 0 2px";

        return cell;
    }

    parameter.oldAddCellInLastRow = parameter.addCellInLastRow;
    parameter.addCellInLastRow = function (control) {
        var cell = this.oldAddCellInLastRow(control);
        cell.style.height = parameter.jsObject.options.parameterRowHeight + "px";
        cell.style.padding = "0px 2px 0 2px";

        return cell;
    }

    //boolCheckBox
    if (parameter.params.type == "Bool" && (parameter.params.basicType == "Value" || parameter.params.basicType == "NullableValue"))
        parameter.addCell(this.CreateBoolCheckBox(parameter));
    //firstTextBox
    if (parameter.params.type != "Bool" || parameter.params.basicType == "List") parameter.addCell(this.CreateFirstTextBox(parameter));
    //firstDateTimeButton
    if ((parameter.params.type == "DateTime" || parameter.params.type == "DateTimeOffset") && parameter.params.allowUserValues && parameter.params.basicType != "List" && parameter.params.basicType != "Range")
        parameter.addCell(this.CreateFirstDateTimeButton(parameter));
    //firstGuidButton
    if (parameter.params.type == "Guid" && parameter.params.allowUserValues && parameter.params.basicType != "List") parameter.addCell(this.CreateFirstGuidButton(parameter));
    //labelTo
    if (parameter.params.basicType == "Range") {
        var middleCell = parameter.addTextCell("-");
        middleCell.style.width = "8px";
        middleCell.style.textAlign = "center";
    }
    //secondTextBox
    if (parameter.params.basicType == "Range") parameter.addCellInLastRow(this.CreateSecondTextBox(parameter));
    //secondDateTimeButton
    if (parameter.params.basicType == "Range" && (parameter.params.type == "DateTime" || parameter.params.type == "DateTimeOffset") && parameter.params.allowUserValues) parameter.addCellInLastRow(this.CreateSecondDateTimeButton(parameter));
    //secondGuidButton
    if (parameter.params.basicType == "Range" && parameter.params.type == "Guid" && parameter.params.allowUserValues) parameter.addCellInLastRow(this.CreateSecondGuidButton(parameter));
    //dropDownButton
    if (parameter.params.items != null || (parameter.params.basicType == "List" && parameter.params.allowUserValues)) parameter.addCellInLastRow(this.CreateDropDownButton(parameter));
    //nullableCheckBox
    if (parameter.params.basicType == "NullableValue" && parameter.params.allowUserValues) parameter.addCellInLastRow(this.CreateNullableCheckBox(parameter));
    //nullableText
    if (parameter.params.basicType == "NullableValue" && parameter.params.allowUserValues) {
        var nullableCell = parameter.addCellInLastRow();
        nullableCell.innerText = this.collections.loc["Null"];
        nullableCell.style.padding = "0px";
    }

    parameter.setEnabled = function (state) {
        this.params.isNull = !state;
        for (var controlName in this.controls) {
            if (controlName != "nullableCheckBox")
                this.controls[controlName].setEnabled(state);
        }
    }

    parameter.changeVisibleStateMenu = function (state) {
        if (state) {
            var menu = null;
            switch (this.params.basicType) {
                case "Value":
                case "NullableValue":
                    menu = this.jsObject.parameterMenuForValue(this);
                    break;

                case "Range":
                    menu = this.jsObject.parameterMenuForRange(this);
                    break;

                case "List":
                    menu = (this.params.allowUserValues) ? this.jsObject.parameterMenuForEditList(this) : this.jsObject.parameterMenuForNotEditList(this);
                    break;
            }

            if (menu != null) menu.changeVisibleState(true);
        }
        else {
            if (parameter.menu != null) {
                if (parameter.params.allowUserValues && parameter.params.basicType == "List") parameter.menu.updateItems();
                parameter.menu.changeVisibleState(false);
            }
        }
    }

    parameter.getStringDateTime = function (object, dateTimeType, isFinishOfDay) {
        if (object && object.isNull) return "";

        if (dateTimeType == "Date") {
            object.hours = isFinishOfDay ? 23 : 0;
            object.minutes = isFinishOfDay ? 59 : 0;
            object.seconds = isFinishOfDay ? 59 : 0;
        }

        var hours = object.hours > 12 ? object.hours - 12 : object.hours;
        if (hours == 0) hours = 12;

        return object.month + "/" + object.day + "/" + object.year + " " +
            hours + ":" + object.minutes + ":" + object.seconds + " " +
            (object.hours < 12 ? "AM" : "PM");
    }

    parameter.getValue = function () {
        var value = null;
        if (parameter.params.isNull) return null;

        var isDateTimeVar = parameter.params.type == "DateTime" || parameter.params.type == "DateTimeOffset";
        var isBoolVar = parameter.params.type == "Bool";

        if (parameter.params.basicType == "Value" || parameter.params.basicType == "NullableValue") {
            if (isBoolVar) return parameter.controls.boolCheckBox.isChecked;
            if (isDateTimeVar) return this.getStringDateTime(parameter.params.key, parameter.params.dateTimeType);

            value = parameter.params.allowUserValues
                ? parameter.controls.firstTextBox.value
                : parameter.params.key;
        }

        if (parameter.params.basicType == "Range") {
            value = {};
            value.from = isDateTimeVar ? this.getStringDateTime(parameter.params.key, parameter.params.dateTimeType) : parameter.controls.firstTextBox.value;
            value.to = isDateTimeVar ? this.getStringDateTime(parameter.params.keyTo, parameter.params.dateTimeType, true) : parameter.controls.secondTextBox.value;
        }

        if (parameter.params.basicType == "List") {
            value = []
            if (parameter.params.allowUserValues) {
                for (var index in parameter.params.items)
                    value[index] = isDateTimeVar
                        ? this.getStringDateTime(parameter.params.items[index].key, parameter.params.dateTimeType)
                        : parameter.params.items[index].key;
            }
            else {
                var num = 0;
                for (var index in parameter.params.items)
                    if (parameter.params.items[index].isChecked) {
                        value[num] = isDateTimeVar
                            ? this.getStringDateTime(parameter.params.items[index].key, parameter.params.dateTimeType)
                            : parameter.params.items[index].key;
                        num++;
                    }
            }
        }

        return value;
    };

    //Methods For Stimulsoft Server

    parameter.getDateTimeForReportServer = function (value) {
        var date = new Date(value.year, value.month - 1, value.day, value.hours, value.minutes, value.seconds);
        return (parameter.jsObject.options.cloudReportsClient.options.const_dateTime1970InTicks + date * 10000).toString();
    }

    parameter.getTimeSpanForReportServer = function (value) {
        var jsObject = parameter.jsObject;

        var timeArray = value.split(":");
        var daysHoursArray = timeArray[0].split(".");
        var days = (daysHoursArray.length > 1) ? jsObject.strToInt(daysHoursArray[0]) : 0;
        var hours = jsObject.strToInt((daysHoursArray.length > 1) ? daysHoursArray[1] : daysHoursArray[0]);
        var minutes = (timeArray.length > 1) ? jsObject.strToInt(timeArray[1]) : 0;
        var seconds = (timeArray.length > 2) ? jsObject.strToInt(timeArray[2]) : 0;

        return ((days * 86400000 + hours * 3600000 + minutes * 60000 + seconds * 1000) * 10000).toString();
    }

    parameter.getSingleValueForReportServer = function () {
        var value = null;
        if (parameter.params.isNull) return null;

        if (parameter.params.basicType == "Value" || parameter.params.basicType == "NullableValue") {
            if (parameter.params.type == "Bool") return parameter.controls.boolCheckBox.isChecked ? "True" : "False";
            if (parameter.params.type == "DateTime" || parameter.params.type == "DateTimeOffset") return parameter.getDateTimeForReportServer(parameter.params.key);
            value = parameter.params.allowUserValues ? parameter.controls.firstTextBox.value : parameter.params.key;
            if (parameter.params.type == "TimeSpan") value = parameter.getTimeSpanForReportServer(value);
        }

        return value;
    };

    parameter.getRangeValuesForReportServer = function () {
        var values = {};
        values.from = (parameter.params.type == "DateTime" || parameter.params.type == "DateTimeOffset")
            ? parameter.getDateTimeForReportServer(parameter.params.key)
            : (parameter.params.type == "TimeSpan") ? parameter.getTimeSpanForReportServer(parameter.controls.firstTextBox.value) : parameter.controls.firstTextBox.value;

        values.to = (parameter.params.type == "DateTime" || parameter.params.type == "DateTimeOffset")
            ? parameter.getDateTimeForReportServer(parameter.params.keyTo)
            : (parameter.params.type == "TimeSpan") ? parameter.getTimeSpanForReportServer(parameter.controls.secondTextBox.value) : parameter.controls.secondTextBox.value;

        return values;
    };

    parameter.getListValuesForReportServer = function () {
        var values = [];
        var num = 0;

        for (var index in parameter.params.items) {
            var valuesItem = {};
            valuesItem.Ident = "Single";

            if (parameter.params.allowUserValues || (!parameter.params.allowUserValues && parameter.params.items[index].isChecked)) {
                valuesItem.Value = (parameter.params.type == "DateTime" || parameter.params.type == "DateTimeOffset")
                    ? parameter.getDateTimeForReportServer(parameter.params.items[index].key)
                    : (parameter.params.type == "TimeSpan")
                        ? parameter.getTimeSpanForReportServer(parameter.params.items[index].key)
                        : parameter.params.items[index].key;
                valuesItem.Type = (valuesItem.Value == null) ? null : parameter.getSingleType();
                values.push(valuesItem);
            }
        }

        return values;
    };

    parameter.getParameterObjectForReportServer = function () {
        var parameterObject = {};
        parameterObject.Ident = parameter.params.basicType.indexOf("Value") != -1 ? "Single" : parameter.params.basicType;
        parameterObject.Name = parameter.params.name;

        switch (parameterObject.Ident) {
            case "Single":
                parameterObject.Value = parameter.getSingleValueForReportServer();
                parameterObject.Type = (parameterObject.Value == null) ? null : parameter.getSingleType();
                break;

            case "Range":
                var values = parameter.getRangeValuesForReportServer();
                parameterObject.FromValue = values.from;
                parameterObject.ToValue = values.to;
                parameterObject.RangeType = parameter.params.type + "Range";
                parameterObject.FromType = (parameterObject.FromValue == null) ? null : parameter.getSingleType();
                parameterObject.ToType = (parameterObject.ToValue == null) ? null : parameter.getSingleType();
                break;

            case "List":
                parameterObject.ListType = parameter.params.type + "List";
                parameterObject.Values = parameter.getListValuesForReportServer();
                break;
        }

        return parameterObject;
    };

    parameter.getSingleType = function () {
        var type = parameter.params.type;
        if (type != "DateTime" && type != "DateTimeOffset" && type != "TimeSpan" && type != "Guid" && type != "Decimal") return type.toLowerCase();

        return type;
    }

    if (parameter.controls.nullableCheckBox && (parameter.params.type == "DateTime" || parameter.params.type == "DateTimeOffset") && parameter.params.value == null) {
        parameter.controls.nullableCheckBox.setChecked(true);
    }

    return parameter;
}

// ---------------------  Controls   ----------------------------

//boolCheckBox
StiJsViewer.prototype.CreateBoolCheckBox = function (parameter) {
    var checkBox = this.ParameterCheckBox(parameter);
    checkBox.name = parameter.params.name;
    parameter.controls.boolCheckBox = checkBox;
    checkBox.setChecked((typeof (parameter.params.value) == "boolean" && parameter.params.value) || parameter.params.value == "true" || parameter.params.value == "True");
    checkBox.setEnabled(parameter.params.allowUserValues);

    return checkBox;
}

//firstTextBox
StiJsViewer.prototype.CreateFirstTextBox = function (parameter) {
    var textBox = this.ParameterTextBox(parameter);
    textBox.name = parameter.params.name;
    parameter.controls.firstTextBox = textBox;
    textBox.setReadOnly(parameter.params.basicType == "List" || !parameter.params.allowUserValues);

    if (parameter.params.formatMask) {
        this.maskTextBox(textBox, StiBase64.decode(parameter.params.formatMask));
    }

    var isDateTimeVar = parameter.params.type == "DateTime" || parameter.params.type == "DateTimeOffset";

    //Value    
    if (parameter.params.basicType == "Value" || parameter.params.basicType == "NullableValue") {
        var isDateNull = isDateTimeVar && parameter.params.key != null && parameter.params.key.isNull;

        if (isDateTimeVar && (parameter.params.value == null || isDateNull)) {
            parameter.params.key = this.getDateTimeObject(new Date());
            if (isDateNull) parameter.params.key.isNull = true;
        }

        textBox.value = isDateTimeVar
            ? (!isDateNull ? this.getStringKey(parameter.params.key, parameter) : "")
            : parameter.params.value;
    }

    //Range
    if (parameter.params.basicType == "Range") {
        var isDateNull = isDateTimeVar && parameter.params.key != null && parameter.params.key.isNull;

        if (isDateTimeVar && (parameter.params.value == null || isDateNull)) {
            parameter.params.key = this.getDateTimeObject(new Date());
            if (isDateNull) parameter.params.key.isNull = true;
        }

        textBox.value = !isDateNull ? this.getStringKey(parameter.params.key, parameter) : "";
    }

    //List
    if (parameter.params.basicType == "List") {
        if (parameter.params.items) {
            for (var index = 0; index < parameter.params.items.length; index++) {
                var isChecked = true;

                if (parameter.params.value instanceof Array && !parameter.params.allowUserValues &&
                    parameter.params.value.indexOf(parameter.params.items[index].value) < 0 && parameter.params.value.indexOf(parameter.params.items[index].key) < 0)
                    isChecked = false;

                if (parameter.params.isFirstInitialization && parameter.params.checkedStates && index < parameter.params.checkedStates.length) {
                    isChecked = parameter.params.checkedStates[index];
                }

                parameter.params.items[index].isChecked = isChecked;

                if (isChecked && index < 50) {
                    if (textBox.value != "") textBox.value += (parameter.jsObject.options.listSeparator ? parameter.jsObject.options.listSeparator + " " : "; ");

                    if (parameter.params.allowUserValues)
                        textBox.value += this.getStringKey(parameter.params.items[index].key, parameter);
                    else
                        textBox.value += parameter.params.items[index].value != "" ? parameter.params.items[index].value : this.getStringKey(parameter.params.items[index].key, parameter);
                }
            }
        }
    }

    return textBox;
}

//firstDateTimeButton
StiJsViewer.prototype.CreateFirstDateTimeButton = function (parameter) {
    var dateTimeButton = this.ParameterButton("DateTimeButton", parameter);
    parameter.controls.firstDateTimeButton = dateTimeButton;
    dateTimeButton.action = function () {
        var datePicker = dateTimeButton.jsObject.controls.datePicker;
        datePicker.value = this.parameter.params.key;
        datePicker.showTime = this.parameter.params.dateTimeType != "Date";
        datePicker.showDate = this.parameter.params.dateTimeType != "Time";
        datePicker.parentDateControl = this.parameter.controls.firstTextBox;
        datePicker.parentButton = this;
        datePicker.changeVisibleState(!datePicker.visible);
    }

    return dateTimeButton;
}

//firstGuidButton
StiJsViewer.prototype.CreateFirstGuidButton = function (parameter) {
    var guidButton = this.ParameterButton("GuidButton", parameter);
    parameter.controls.firstGuidButton = guidButton;
    guidButton.action = function () {
        this.parameter.controls.firstTextBox.value = this.parameter.jsObject.newGuid();
    }

    return guidButton;
}

//secondTextBox
StiJsViewer.prototype.CreateSecondTextBox = function (parameter) {
    var isDateNull = (parameter.params.type == "DateTime" || parameter.params.type == "DateTimeOffset") && parameter.params.keyTo != null && parameter.params.keyTo.isNull;
    var textBox = this.ParameterTextBox(parameter);
    textBox.name = parameter.params.name + "_To";
    parameter.controls.secondTextBox = textBox;
    textBox.setReadOnly(!parameter.params.allowUserValues);

    if ((parameter.params.type == "DateTime" || parameter.params.type == "DateTimeOffset") && (parameter.params.value == null || isDateNull)) {
        parameter.params.keyTo = this.getDateTimeObject(new Date);
        parameter.params.keyTo.isNull = true;
    }
    textBox.value = !isDateNull ? this.getStringKey(parameter.params.keyTo, parameter) : "";

    return textBox;
}

//secondDateTimeButton
StiJsViewer.prototype.CreateSecondDateTimeButton = function (parameter) {
    var jsObject = this;
    var dateTimeButton = this.ParameterButton("DateTimeButton", parameter);
    parameter.controls.secondDateTimeButton = dateTimeButton;

    dateTimeButton.action = function () {
        var datePickerParams = {
            showTime: this.parameter.params.dateTimeType != "Date",
            showDate: this.parameter.params.dateTimeType != "Time",
            firstParentDateControl: this.parameter.controls.firstTextBox,
            firstParentButton: this.parameter.controls.firstDateTimeButton,
            firstValue: this.parameter.params.key,
            secondParentDateControl: this.parameter.controls.secondTextBox,
            secondParentButton: this,
            secondValue: this.parameter.params.keyTo
        }

        var parametersPanelPosition = jsObject.options.currentParametersPanelPosition || jsObject.options.appearance.parametersPanelPosition;
        var datePicker = jsObject.InitializeDoubleDatePicker(datePickerParams);
        datePicker.changeVisibleState(!datePicker.visible, null, parametersPanelPosition == "Left" ? false : true, parametersPanelPosition == "Left" ? 245 : 0);
    }

    return dateTimeButton;
}

//secondGuidButton
StiJsViewer.prototype.CreateSecondGuidButton = function (parameter) {
    var guidButton = this.ParameterButton("GuidButton", parameter);
    parameter.controls.secondGuidButton = guidButton;
    guidButton.action = function () {
        this.parameter.controls.secondTextBox.value = this.parameter.jsObject.newGuid();
    }

    return guidButton;
}

//dropDownButton
StiJsViewer.prototype.CreateDropDownButton = function (parameter) {
    var dropDownButton = this.ParameterButton("DropDownButton", parameter);
    parameter.controls.dropDownButton = dropDownButton;
    dropDownButton.action = function () {
        this.parameter.changeVisibleStateMenu(this.parameter.menu == null);
    }

    return dropDownButton;
}

//nullableCheckBox
StiJsViewer.prototype.CreateNullableCheckBox = function (parameter) {
    var checkBox = this.ParameterCheckBox(parameter);
    checkBox.name = parameter.params.name + "_Nullable";
    parameter.controls.nullableCheckBox = checkBox;
    checkBox.onChecked = function () {
        this.parameter.setEnabled(!this.isChecked);

        var textColor = !this.isChecked
            ? (this.jsObject.options.toolbar.fontColor && this.jsObject.options.toolbar.fontColor != "Empty" ? this.jsObject.options.toolbar.fontColor : "#444444")
            : (this.parameter.params.type == "DateTime" || this.parameter.params.type == "DateTimeOffset") ? "transparent" : "#c6c6c6";

        if (this.parameter.controls.firstTextBox) this.parameter.controls.firstTextBox.style.color = textColor;
        if (this.parameter.controls.secondTextBox) this.parameter.controls.secondTextBox.style.color = textColor;
    }

    return checkBox;
}