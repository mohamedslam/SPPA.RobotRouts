
StiJsViewer.prototype.InitializeDatePicker = function (doubleDatePicker) {
    var jsObject = this;
    var datePicker = this.BaseMenu(null, null, "Down", "stiJsViewerDropdownMenu");    
    datePicker.style.fontFamily = this.options.toolbar.fontFamily;
    datePicker.style.zIndex = "36";
    datePicker.parentDateControl = null;
    datePicker.dayButtons = [];
    datePicker.showDate = true;
    datePicker.showTime = false;
    datePicker.doubleDatePicker = doubleDatePicker;
    datePicker.key = new Date();
    datePicker.innerContent.style.overflowY = "hidden";

    if (this.options.toolbar.fontColor != "")
        datePicker.style.color = this.options.toolbar.fontColor;

    if (!doubleDatePicker) {
        this.controls.datePicker = datePicker;
        this.controls.mainPanel.appendChild(datePicker);
    }

    //Add Header Buttons
    var headerButtonsTable = this.CreateHTMLTable();
    datePicker.innerContent.appendChild(headerButtonsTable);

    //Prev Month
    datePicker.prevMonthButton = this.SmallButton(null, null, "Arrows.BigArrowLeft.png");
    datePicker.prevMonthButton.style.margin = "1px 2px 0 1px";
    datePicker.prevMonthButton.datePicker = datePicker;
    datePicker.prevMonthButton.action = function () {
        var month = this.datePicker.key.getMonth();
        var year = this.datePicker.key.getFullYear();
        month--;
        if (month == -1) { month = 11; year--; }
        var countDaysInMonth = jsObject.GetCountDaysOfMonth(year, month);
        if (countDaysInMonth < this.datePicker.key.getDate()) this.datePicker.key.setDate(countDaysInMonth);
        this.datePicker.key.setMonth(month); this.datePicker.key.setYear(year);
        this.datePicker.fill();
        this.datePicker.applyValues();
    };
    headerButtonsTable.addCell(datePicker.prevMonthButton);

    //Month DropDownList
    datePicker.monthDropDownList = this.DropDownList(null, this.options.isTouchDevice ? 79 : 81, null, this.GetMonthesForDatePickerItems(), true);
    datePicker.monthDropDownList.style.margin = "1px 2px 0 0";
    datePicker.monthDropDownList.datePicker = datePicker;
    datePicker.monthDropDownList.action = function () {
        var countDaysInMonth = jsObject.GetCountDaysOfMonth(this.datePicker.key.getFullYear(), parseInt(this.key));
        if (countDaysInMonth < this.datePicker.key.getDate()) this.datePicker.key.setDate(countDaysInMonth);
        this.datePicker.key.setMonth(parseInt(this.key));
        this.datePicker.repaintDays();
        this.datePicker.applyValues();
    };
    headerButtonsTable.addCell(datePicker.monthDropDownList);

    //Override menu
    datePicker.monthDropDownList.menu.style.zIndex = "37";
    datePicker.monthDropDownList.menu.datePicker = datePicker;
    datePicker.monthDropDownList.menu.onmousedown = function () {
        if (!this.isTouchEndFlag) this.ontouchstart(true);
    }
    datePicker.monthDropDownList.menu.ontouchstart = function (mouseProcess) {
        var this_ = this;
        this.isTouchEndFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchEndTimer);
        jsObject.options.dropDownListMenuPressed = this;
        this.datePicker.ontouchstart();
        this.isTouchEndTimer = setTimeout(function () {
            this_.isTouchEndFlag = false;
        }, 1000);
    }

    //Year TextBox
    datePicker.yearTextBox = this.TextBox(null, 40, "Year");
    datePicker.yearTextBox.style.margin = "1px 2px 0 0";
    datePicker.yearTextBox.datePicker = datePicker;
    datePicker.yearTextBox.action = function () {
        var oldYear = this.datePicker.key.getFullYear();
        var year = jsObject.strToCorrectPositiveInt(this.value);
        this.value = year;
        if (oldYear != year) {
            this.datePicker.key.setYear(year);
            this.datePicker.repaintDays();
            this.datePicker.applyValues();
        }
    };
    headerButtonsTable.addCell(datePicker.yearTextBox);

    //Next Month
    datePicker.nextMonthButton = this.SmallButton(null, null, "Arrows.BigArrowRight.png");
    datePicker.nextMonthButton.datePicker = datePicker;
    datePicker.nextMonthButton.style.margin = "1px 1px 0 0";
    datePicker.nextMonthButton.action = function () {
        var month = this.datePicker.key.getMonth();
        var year = this.datePicker.key.getFullYear();
        month++;
        if (month == 12) { month = 0; year++; }
        var countDaysInMonth = jsObject.GetCountDaysOfMonth(year, month);
        if (countDaysInMonth < this.datePicker.key.getDate()) this.datePicker.key.setDate(countDaysInMonth);
        this.datePicker.key.setMonth(month); this.datePicker.key.setYear(year);
        this.datePicker.fill();
        this.datePicker.applyValues();
    };
    headerButtonsTable.addCell(datePicker.nextMonthButton);

    //Separator
    var separator = document.createElement("div");
    separator.style.margin = "2px 0 2px 0";
    separator.className = "stiJsViewerDatePickerSeparator";
    datePicker.innerContent.appendChild(separator);

    datePicker.daysTable = this.CreateHTMLTable();
    datePicker.innerContent.appendChild(datePicker.daysTable);

    var startDay = jsObject.options.appearance.datePickerFirstDayOfWeek == "Auto" ? jsObject.GetFirstDayOfWeek() : jsObject.options.appearance.datePickerFirstDayOfWeek;

    for (var i = 0; i < 7; i++) {
        var dayOfWeekCell = datePicker.daysTable.addCell();
        dayOfWeekCell.className = "stiJsViewerDatePickerDayOfWeekCell";
        var dayName = this.collections.dayOfWeek[i];
        if (dayName) {
            if (dayName.length > 3) dayName = dayName.substring(0, 2);
            if (dayName.length == 2) dayOfWeekCell.style.fontSize = "11px";
            if (dayName.length == 3) dayOfWeekCell.style.fontSize = "8px";
            dayOfWeekCell.innerText = dayName;
        }
        if (i == (startDay == "Sunday" ? 6 : 5)) dayOfWeekCell.style.color = "#0000ff";
        if (i == (startDay == "Sunday" ? 0 : 6)) dayOfWeekCell.style.color = "#ff0000";
    }

    //Add Day Cells    
    datePicker.daysTable.addRow();
    var rowCount = 1;
    for (var i = 0; i < 42; i++) {
        var dayButton = this.DatePickerDayButton();
        dayButton.datePicker = datePicker;
        dayButton.style.margin = "1px";
        datePicker.dayButtons.push(dayButton);
        datePicker.daysTable.addCellInRow(rowCount, dayButton);
        if ((i + 1) % 7 == 0) { datePicker.daysTable.addRow(); rowCount++ }
    }

    //Separator2
    var separator2 = document.createElement("div");
    separator2.style.margin = "2px 0 2px 0";
    separator2.className = "stiJsViewerDatePickerSeparator";
    datePicker.innerContent.appendChild(separator2);

    //Time
    var timeTable = this.CreateHTMLTable();
    timeTable.style.width = "100%";
    datePicker.innerContent.appendChild(timeTable);
    timeTable.addTextCell(this.collections.loc.Time + ":").setAttribute("style", "padding: 0 4px 0 4px; white-space: nowrap;");
    var timeControl = this.TextBox(null, 90);
    timeControl.style.margin = "1px 2px 2px 2px";
    var timeControlCell = timeTable.addCell(timeControl);
    timeControlCell.style.width = "100%";
    timeControlCell.style.textAlign = "right";
    datePicker.time = timeControl;

    timeControl.action = function () {
        var time = jsObject.stringToTime(this.value);
        datePicker.key.setHours(time.hours);
        datePicker.key.setMinutes(time.minutes);
        datePicker.key.setSeconds(time.seconds);
        this.value = jsObject.formatDate(datePicker.key, "H:mm:ss");
        datePicker.applyValues();
    };

    datePicker.repaintDays = function () {
        var month = this.key.getMonth();
        var year = this.key.getFullYear();
        var countDaysInMonth = jsObject.GetCountDaysOfMonth(year, month);
        var firstDay = jsObject.GetDayOfWeek(year, month, 1);
        var startDay = jsObject.options.appearance.datePickerFirstDayOfWeek == "Auto" ? jsObject.GetFirstDayOfWeek() : jsObject.options.appearance.datePickerFirstDayOfWeek;
        if (startDay == "Monday") firstDay--;
        else if (firstDay == 7 && startDay == "Sunday") firstDay = 0;

        for (var i = 0; i < 42; i++) {
            var numDay = i - firstDay + 1;
            var isSelectedDay = (numDay == this.key.getDate());
            var dayButton = this.dayButtons[i];

            if (!((i < firstDay) || (i - firstDay > countDaysInMonth - 1))) {
                dayButton.numberOfDay = numDay;
                dayButton.caption.innerText = numDay;
                dayButton.setEnabled(true);
                dayButton.setSelected(isSelectedDay);
            }
            else {
                dayButton.caption.innerText = "";
                dayButton.setEnabled(false);
            }
        }
    }

    datePicker.fill = function () {
        this.yearTextBox.value = this.key.getFullYear();
        this.monthDropDownList.setKey(this.key.getMonth());
        this.repaintDays();
        if (this.showTime) {
            this.time.value = jsObject.formatDate(this.key, "H:mm:ss");
        }
    }

    datePicker.onshow = function () {
        if (!this.key)
            this.key = new Date();

        if (this.value) {
            this.key = new Date(this.value.year, this.value.month - 1, this.value.day,
                this.value.hours, this.value.minutes, this.value.seconds);
        }

        headerButtonsTable.style.display = datePicker.daysTable.style.display = separator.style.display = this.showDate ? "" : "none";
        separator2.style.display = this.showTime && this.showDate ? "" : "none";
        timeTable.style.display = this.showTime ? "" : "none";
        this.fill();
    };

    datePicker.onHide = function () { }

    datePicker.applyValues = function (ignoreAction) {
        if (!this.value)
            this.value = jsObject.getDateTimeObject();
        else
            this.value.isNull = false;

        //change parent control values
        this.value.year = this.key.getFullYear();
        this.value.month = this.key.getMonth() + 1;
        this.value.day = this.key.getDate();
        this.value.hours = this.key.getHours();
        this.value.minutes = this.key.getMinutes();
        this.value.seconds = this.key.getSeconds();

        if (this.parentDateControl) {
            var dateTimeType = this.parentDateControl.parameter
                ? this.parentDateControl.parameter.params.dateTimeType
                : !this.showTime ? "Date" : null;

            this.parentDateControl.value = jsObject.dateTimeObjectToString(datePicker.value, dateTimeType);
        }

        if (this.action && !ignoreAction) this.action();
    };

    datePicker.action = function () { }

    //Ovveride Methods
    datePicker.onmousedown = function () {
        if (!this.isTouchStartFlag) this.ontouchstart(true);
    }

    datePicker.ontouchstart = function (mouseProcess) {
        var this_ = this;
        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);
        jsObject.options.datePickerPressed = this;
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    datePicker.changeVisibleState = function (state) {
        var mainClassName = "stiJsViewerMainPanel";
        if (state) {
            this.onshow();
            this.style.display = "";
            this.visible = true;
            this.style.overflow = "hidden";
            if (this.parentDateControl && this.parentDateControl.setSelected) this.parentDateControl.setSelected(true);
            this.parentButton.setSelected(true);
            jsObject.options.currentDatePicker = this;
            this.style.width = this.innerContent.offsetWidth + "px";
            this.style.height = this.innerContent.offsetHeight + "px";
            this.style.left = (jsObject.FindPosX(this.parentButton, mainClassName)) + "px";
            this.style.top = (jsObject.FindPosY(this.parentButton, mainClassName) + this.parentButton.offsetHeight + 1) + "px";
            this.innerContent.style.top = -this.innerContent.offsetHeight + "px";

            var viewerLeft = jsObject.FindPosX(jsObject.controls.mainPanel);
            var viewerTop = jsObject.FindPosY(jsObject.controls.mainPanel);
            var browserHeight = (window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight);
            var browserWidth = (window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth);
            var resultLeftPos = parseInt(this.style.left);
            var resultTopPos = parseInt(this.style.top);

            if (resultTopPos < 0) {
                this.style.top = "10px";
            }
            else if (resultTopPos + this.innerContent.offsetHeight > browserHeight - viewerTop) {
                this.style.top = (browserHeight - viewerTop - this.innerContent.offsetHeight - 10) + "px";
            }

            if (resultLeftPos < 0) {
                this.style.left = "10px";
            }
            else if (resultLeftPos + this.innerContent.offsetWidth > browserWidth - viewerLeft) {
                this.style.left = (browserWidth - viewerLeft - this.innerContent.offsetWidth - 10) + "px";
            }

            var d = new Date();
            var endTime = d.getTime();
            if (jsObject.options.toolbar.menuAnimation) endTime += jsObject.options.menuAnimDuration;
            jsObject.ShowAnimationVerticalMenu(this, 0, endTime);
        }
        else {
            this.onHide();
            clearTimeout(this.innerContent.animationTimer);
            this.showTime = false;
            this.showDate = true;
            this.visible = false;
            if (this.parentDateControl && this.parentDateControl.setSelected) this.parentDateControl.setSelected(false);
            this.parentButton.setSelected(false);
            this.style.display = "none";
            this.action = null;
            if (jsObject.options.currentDatePicker == this) jsObject.options.currentDatePicker = null;
        }
    }

    return datePicker;
}

StiJsViewer.prototype.DatePickerDayButton = function () {
    var button = this.SmallButton(null, "0", null, null, null, "stiJsViewerDatePickerDayButton");
    var size = this.options.isTouchDevice ? "25px" : "23px";
    button.style.width = size;
    button.style.height = size;
    button.caption.style.textAlign = "center";
    button.innerTable.style.width = "100%";
    button.caption.style.padding = "0px";
    button.numberOfDay = 1;
    button.action = function () {
        this.datePicker.key.setDate(parseInt(this.numberOfDay));
        this.setSelected(true);
        this.datePicker.applyValues();
        if (!this.datePicker.doubleDatePicker) this.datePicker.changeVisibleState(false);
    }

    button.setSelected = function (state) {
        if (state) {
            if (this.datePicker.selectedButton) this.datePicker.selectedButton.setSelected(false);
            this.datePicker.selectedButton = this;
        }
        this.isSelected = state;
        this.className = this.styleName + " " + this.styleName +
            (state ? "Selected" : (this.isEnabled ? (this.isOver ? "Over" : "Default") : "Disabled"));
    }

    return button;
}


//Helper Methods
StiJsViewer.prototype.GetDayOfWeek = function (year, month) {
    var result = new Date(year, month, 1).getDay();
    if (result == 0) result = 7;
    return result;
}

StiJsViewer.prototype.GetCountDaysOfMonth = function (year, month) {
    var countDaysInMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
    var count = countDaysInMonth[month];

    if (month == 1)
        if (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0))
            count = 29;
        else
            count = 28;
    return count;
}

/* Monthes */
StiJsViewer.prototype.GetMonthesForDatePickerItems = function () {
    var items = [];
    for (var i = 0; i < this.collections.months.length; i++)
        items.push(this.Item("Month" + i, this.collections.loc["Month" + this.collections.months[i]], null, i));

    return items;
}

StiJsViewer.prototype.GetFirstDayOfWeek = function () {
    var date = new Date();
    var timeString = date.toLocaleTimeString();
    return (timeString.toLowerCase().indexOf("am") >= 0 || timeString.toLowerCase().indexOf("pm") >= 0 ? "Sunday" : "Monday");
}