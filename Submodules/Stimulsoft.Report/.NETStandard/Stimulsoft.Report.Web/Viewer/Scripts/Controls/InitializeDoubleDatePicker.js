
StiJsViewer.prototype.InitializeDoubleDatePicker = function (params) {
    if (this.controls.doubleDatePicker) {
        this.controls.mainPanel.removeChild(this.controls.doubleDatePicker);
    }

    var datePicker = this.BaseMenu(null, params.secondParentButton, "Down", "stiJsViewerDropdownMenu");
    datePicker.style.fontFamily = this.options.toolbar.fontFamily;
    if (this.options.toolbar.fontColor != "") datePicker.style.color = this.options.toolbar.fontColor;
    datePicker.style.zIndex = "36";
    datePicker.dayButtons = [];
    datePicker.showDate = params.showDate != null ? params.showDate : true;
    datePicker.showTime = params.showTime != null ? params.showTime : false;
    datePicker.key = new Date();
    this.controls.doubleDatePicker = datePicker;
    this.controls.mainPanel.appendChild(datePicker);

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "4px";
    innerTable.style.border = "1px dotted #c6c6c6";
    datePicker.innerContent.appendChild(innerTable);
    datePicker.innerContent.style.maxHeight = "600px";

    //First DatePicker
    var firstDatePicker = this.InitializeDatePicker(datePicker);
    datePicker.firstDatePicker = firstDatePicker;
    firstDatePicker.value = params.firstValue;
    firstDatePicker.showTime = datePicker.showTime;
    firstDatePicker.showDate = datePicker.showDate;
    firstDatePicker.parentDateControl = params.firstParentDateControl;
    firstDatePicker.parentButton = params.firstParentButton;

    firstDatePicker.action = function () {
        datePicker.action();
    }

    //Second DatePicker
    var secondDatePicker = this.InitializeDatePicker(datePicker);
    datePicker.secondDatePicker = secondDatePicker;
    secondDatePicker.value = params.secondValue;
    secondDatePicker.showTime = datePicker.showTime;
    secondDatePicker.showDate = datePicker.showDate;
    secondDatePicker.parentDateControl = params.secondParentDateControl;
    secondDatePicker.parentButton = params.secondParentButton;

    secondDatePicker.action = function () {
        datePicker.action();
    }

    //Add Pickers to Double Picker Panel
    firstDatePicker.innerContent.className = "";
    secondDatePicker.innerContent.className = "";
    firstDatePicker.innerContent.style.margin = "4px";
    secondDatePicker.innerContent.style.margin = "4px";
    innerTable.addCell(firstDatePicker.innerContent).style.verticalAlign = "top";
    var cell = innerTable.addCell(secondDatePicker.innerContent);
    cell.style.borderLeft = "1px dotted #c6c6c6";
    cell.style.verticalAlign = "top";

    var container = document.createElement("div");
    innerTable.addCell(container).style.borderLeft = "1px dotted #c6c6c6";

    container.jsObject = this;
    container.style.width = "150px";
    container.style.height = datePicker.showTime ? (datePicker.showDate ? "250px" : "50px") : "220px";
    container.style.overflow = "auto";
    container.style.margin = "4px";
    container.style.display = datePicker.showDate ? "" : "none";

    for (var i = 0; i < this.collections.dateRanges.length; i++) {
        var dateRangeName = this.collections.dateRanges[i];
        var item = this.SmallButton(null, this.collections.loc[dateRangeName]);
        item.name = dateRangeName;
        container.appendChild(item);

        item.action = function () {
            var values = datePicker.jsObject.GetValuesByDateRangeName(this.name);
            if (values) {
                datePicker.setValuesToDatePickers(values[0], values[1]);
                if (params.hideOnClick) datePicker.changeVisibleState(false);
            }
        }

        if (dateRangeName == "Yesterday" || dateRangeName == "PreviousWeek" || dateRangeName == "PreviousMonth" ||
            dateRangeName == "PreviousQuarter" || dateRangeName == "PreviousYear" || dateRangeName == "FourthQuarter" || dateRangeName == "Last30Days") {
            var sep = this.VerticalMenuSeparator(datePicker, dateRangeName + "Sep");
            sep.style.margin = "2px";
            container.appendChild(sep);
        }
    }

    datePicker.checkWidth = function () {
        var browserWidth = (window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth);
        if (firstDatePicker.innerContent.offsetWidth + secondDatePicker.innerContent.offsetWidth + container.offsetWidth > browserWidth) {
            datePicker.innerContent.appendChild(secondDatePicker.innerContent);
        }
    }

    datePicker.onshow = function () {
        firstDatePicker.onshow();
        secondDatePicker.onshow();
        datePicker.checkWidth();
    }

    datePicker.onhide = function () {
        cell.appendChild(secondDatePicker.innerContent);
    }

    datePicker.setValuesToDatePickers = function (value1, value2) {
        firstDatePicker.key = value1;
        secondDatePicker.key = value2;
        firstDatePicker.fill();
        secondDatePicker.fill();
        firstDatePicker.applyValues(true);
        secondDatePicker.applyValues(true);
        datePicker.action();
    }

    datePicker.action = function () { };

    return datePicker;
}

StiJsViewer.prototype.GetValuesByDateRangeName = function (dateRangeName) {
    var now = new Date();
    var jsObject = this;

    var setTimeInterval = function (firstDate, secondDate) {
        firstDate.setHours(0);
        firstDate.setMinutes(0);
        firstDate.setSeconds(0);
        secondDate.setHours(23);
        secondDate.setMinutes(59);
        secondDate.setSeconds(59);
    }

    var getWeekInterval = function (date) {
        var startDay = jsObject.options.appearance.datePickerFirstDayOfWeek == "Auto" ? jsObject.GetFirstDayOfWeek() : jsObject.options.appearance.datePickerFirstDayOfWeek;
        var dayWeek = startDay == "Sunday" ? now.getDay() : now.getDay() - 1;
        if (dayWeek < 0) dayWeek = 6;
        var values = [new Date(now.valueOf() - dayWeek * 86400000)];
        values.push(new Date(values[0].valueOf() + 6 * 86400000));
        setTimeInterval(values[0], values[1]);

        return values;
    }

    var values = [new Date(), new Date()];

    switch (dateRangeName) {
        case "CurrentMonth":
            {
                values[0].setDate(1);
                values[1].setDate(jsObject.GetCountDaysOfMonth(now.getFullYear(), now.getMonth()));
                break;
            }
        case "CurrentQuarter":
            {
                var firstMonth = parseInt(now.getMonth() / 3) * 3;
                values[0] = new Date(now.getFullYear(), firstMonth, 1);
                values[1] = new Date(now.getFullYear(), firstMonth + 2, jsObject.GetCountDaysOfMonth(now.getFullYear(), firstMonth + 2));
                break;
            }
        case "CurrentWeek":
            {
                values = getWeekInterval(now);
                break;
            }
        case "CurrentYear":
            {
                values[0] = new Date(now.getFullYear(), 0, 1);
                values[1] = new Date(now.getFullYear(), 11, 31);
                break;
            }
        case "NextMonth":
            {
                var month = now.getMonth() + 1;
                var year = now.getFullYear();
                if (month > 11) {
                    month = 0;
                    year++;
                }
                values[0] = new Date(year, month, 1);
                values[1] = new Date(year, month, jsObject.GetCountDaysOfMonth(year, month));
                break;
            }
        case "NextQuarter":
            {
                var year = now.getFullYear();
                var firstMonth = parseInt(now.getMonth() / 3) * 3 + 3;
                if (firstMonth > 11) {
                    firstMonth = 0;
                    year++;
                }
                values[0] = new Date(year, firstMonth, 1);
                values[1] = new Date(year, firstMonth + 2, jsObject.GetCountDaysOfMonth(year, firstMonth + 2));
                break;
            }
        case "NextWeek":
            {
                values = getWeekInterval(now);
                values[0] = new Date(values[0].valueOf() + 7 * 86400000);
                values[1] = new Date(values[1].valueOf() + 7 * 86400000);
                break;
            }
        case "NextYear":
            {
                values[0] = new Date(now.getFullYear() + 1, 0, 1);
                values[1] = new Date(now.getFullYear() + 1, 11, 31);
                break;
            }
        case "PreviousMonth":
            {
                var month = now.getMonth() - 1;
                var year = now.getFullYear();
                if (month < 0) {
                    month = 11;
                    year--;
                }
                values[0] = new Date(year, month, 1);
                values[1] = new Date(year, month, jsObject.GetCountDaysOfMonth(year, month));
                break;
            }
        case "PreviousQuarter":
            {
                var year = now.getFullYear();
                var firstMonth = parseInt(now.getMonth() / 3) * 3 - 3;
                if (firstMonth < 0) {
                    firstMonth = 9;
                    year--;
                }
                values[0] = new Date(year, firstMonth, 1);
                values[1] = new Date(year, firstMonth + 2, jsObject.GetCountDaysOfMonth(year, firstMonth + 2));
                break;
            }
        case "PreviousWeek":
            {
                values = getWeekInterval(now);
                values[0] = new Date(values[0].valueOf() - 7 * 86400000);
                values[1] = new Date(values[1].valueOf() - 7 * 86400000);
                break;
            }
        case "PreviousYear":
            {
                values[0] = new Date(now.getFullYear() - 1, 0, 1);
                values[1] = new Date(now.getFullYear() - 1, 11, 31);
                break;
            }
        case "FirstQuarter":
            {
                values[0] = new Date(now.getFullYear(), 0, 1);
                values[1] = new Date(now.getFullYear(), 2, jsObject.GetCountDaysOfMonth(now.getFullYear(), 2));
                break;
            }
        case "SecondQuarter":
            {
                values[0] = new Date(now.getFullYear(), 3, 1);
                values[1] = new Date(now.getFullYear(), 5, jsObject.GetCountDaysOfMonth(now.getFullYear(), 5));
                break;
            }
        case "ThirdQuarter":
            {
                values[0] = new Date(now.getFullYear(), 6, 1);
                values[1] = new Date(now.getFullYear(), 8, jsObject.GetCountDaysOfMonth(now.getFullYear(), 8));
                break;
            }
        case "FourthQuarter":
            {
                values[0] = new Date(now.getFullYear(), 9, 1);
                values[1] = new Date(now.getFullYear(), 11, jsObject.GetCountDaysOfMonth(now.getFullYear(), 11));
                break;
            }
        case "MonthToDate":
            {
                values[0].setDate(1);
                break;
            }
        case "QuarterToDate":
            {
                var firstMonth = parseInt(now.getMonth() / 3) * 3;
                values[0].setDate(1);
                values[0].setMonth(firstMonth);
                break;
            }
        case "WeekToDate":
            {
                var weekValues = getWeekInterval(now);
                values[0] = weekValues[0];
                break;
            }
        case "YearToDate":
            {
                values[0].setDate(1);
                values[0].setMonth(0);
                break;
            }
        case "Today":
            {
                break;
            }
        case "Tomorrow":
            {
                values[0] = new Date(values[0].valueOf() + 86400000);
                values[1] = new Date(values[1].valueOf() + 86400000);
                break;
            }
        case "Yesterday":
            {
                values[0] = new Date(values[0].valueOf() - 86400000);
                values[1] = new Date(values[1].valueOf() - 86400000);
                break;
            }
        case "Last7Days":
            {
                if (jsObject.options.appearance.datePickerIncludeCurrentDayForRanges)
                    values[0] = new Date(values[0].valueOf() - 6 * 86400000);
                else {
                    values[0] = new Date(values[0].valueOf() - 7 * 86400000);
                    values[1] = new Date(values[1].valueOf() - 1 * 86400000);
                }
                break;
            }
        case "Last14Days":
            {
                if (jsObject.options.appearance.datePickerIncludeCurrentDayForRanges)
                    values[0] = new Date(values[0].valueOf() - 13 * 86400000);
                else {
                    values[0] = new Date(values[0].valueOf() - 14 * 86400000);
                    values[1] = new Date(values[1].valueOf() - 1 * 86400000);
                }
                break;
            }
        case "Last30Days":
            {
                if (jsObject.options.appearance.datePickerIncludeCurrentDayForRanges)
                    values[0] = new Date(values[0].valueOf() - 29 * 86400000);
                else {
                    values[0] = new Date(values[0].valueOf() - 30 * 86400000);
                    values[1] = new Date(values[1].valueOf() - 1 * 86400000);
                }
                break;
            }
    }

    setTimeInterval(values[0], values[1]);

    return values;
}