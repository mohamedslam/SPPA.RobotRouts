
StiMobileDesigner.prototype.InitializeDatePicker = function () {
    var jsObject = this;
    var datePicker = this.BaseMenu("datePicker", null, "Down");    
    datePicker.style.zIndex = "100";    
    datePicker.parentDateControl = null;
    datePicker.dayButtons = [];
    datePicker.key = new Date();
    datePicker.showTime = true;
    datePicker.innerContent.style.overflowY = "hidden";

    //Add Header Buttons
    var headerTable = this.CreateHTMLTable();
    datePicker.innerContent.appendChild(headerTable);

    //Prev Month
    var prevMonthButton = datePicker.prevMonthButton = this.StandartSmallButton("datePickerPrevMonthButton", null, null, "Arrows.ArrowLeft.png", null, null, true);
    prevMonthButton.style.margin = "1px 2px 0 1px";
    prevMonthButton.datePicker = datePicker;
    headerTable.addCell(prevMonthButton);

    prevMonthButton.action = function () {
        var month = datePicker.key.getMonth();
        var year = datePicker.key.getFullYear();
        month--;
        if (month == -1) { month = 11; year--; }
        var countDaysInMonth = jsObject.GetCountDaysOfMonth(year, month);
        if (countDaysInMonth < datePicker.key.getDate()) datePicker.key.setDate(countDaysInMonth);
        datePicker.key.setMonth(month); datePicker.key.setYear(year);
        datePicker.fill();
        datePicker.action();
    };

    //Month DropDownList
    var monthDropDownList = datePicker.monthDropDownList = this.DropDownList("datePickerMonthDropDownList", this.options.isTouchDevice ? 89 : 87, null, this.GetMonthesForDatePickerItems(), true);
    monthDropDownList.style.margin = "1px 2px 0 0";
    monthDropDownList.datePicker = datePicker;
    headerTable.addCell(monthDropDownList);

    monthDropDownList.action = function () {
        var countDaysInMonth = jsObject.GetCountDaysOfMonth(datePicker.key.getFullYear(), parseInt(this.key));
        if (countDaysInMonth < datePicker.key.getDate()) datePicker.key.setDate(countDaysInMonth);
        datePicker.key.setMonth(parseInt(this.key));
        datePicker.repaintDays();
        datePicker.action();
    };


    //Override menu
    monthDropDownList.menu.style.zIndex = "101";
    monthDropDownList.menu.datePicker = datePicker;
    monthDropDownList.menu.onmousedown = function () {
        if (!this.isTouchStartFlag) this.ontouchstart(true);
    }
    monthDropDownList.menu.ontouchstart = function (mouseProcess) {
        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);
        jsObject.options.dropDownListMenuPressed = this;
        datePicker.ontouchstart();
        this.isTouchStartTimer = setTimeout(function () {
            monthDropDownList.menu.isTouchStartFlag = false;
        }, 1000);
    }

    //Year TextBox
    var yearTextBox = datePicker.yearTextBox = this.TextBox("datePickerYearTextBox", 30, null, "Year");
    yearTextBox.style.margin = "1px 2px 0 0";
    yearTextBox.datePicker = datePicker;
    headerTable.addCell(yearTextBox);

    yearTextBox.action = function () {
        var year = jsObject.StrToCorrectPositiveInt(this.value);
        this.value = year;
        datePicker.key.setYear(year);
        datePicker.repaintDays();
        datePicker.action();
    };

    //Next
    var nextMonthButton = datePicker.nextMonthButton = this.StandartSmallButton("datePickerNextMonthButton", null, null, "Arrows.ArrowRight.png", null, null, true);
    nextMonthButton.datePicker = datePicker;
    nextMonthButton.style.margin = "1px 1px 0 0";
    headerTable.addCell(nextMonthButton);

    nextMonthButton.action = function () {
        var month = datePicker.key.getMonth();
        var year = datePicker.key.getFullYear();
        month++;
        if (month == 12) { month = 0; year++; }
        var countDaysInMonth = jsObject.GetCountDaysOfMonth(year, month);
        if (countDaysInMonth < datePicker.key.getDate()) datePicker.key.setDate(countDaysInMonth);
        datePicker.key.setMonth(month); datePicker.key.setYear(year);
        datePicker.fill();
        datePicker.action();
    };

    //Separator
    var sep = document.createElement("div");
    sep.style.margin = "2px 0 2px 0";
    sep.className = "stiDesignerDatePickerSeparator";
    datePicker.innerContent.appendChild(sep);

    datePicker.daysTable = this.CreateHTMLTable();
    datePicker.innerContent.appendChild(datePicker.daysTable);

    var startDay = this.options.datePickerFirstDayOfWeek == "Auto" ? this.GetFirstDayOfWeek() : this.options.datePickerFirstDayOfWeek;

    for (var i = 0; i < 7; i++) {
        var dayOfWeekCell = datePicker.daysTable.addCell();
        dayOfWeekCell.className = "stiDesignerDatePickerDayOfWeekCell";
        var dayName = this.options.dayOfWeekCollection[i];
        if (dayName) {
            if (dayName.length > 3) dayName = dayName.substring(0, 2);
            if (dayName.length == 2) dayOfWeekCell.style.fontSize = "11px";
            if (dayName.length == 3) dayOfWeekCell.style.fontSize = "8px";
            dayOfWeekCell.innerHTML = dayName;
        }
        if (i == (startDay == "Sunday" ? 6 : 5)) dayOfWeekCell.style.color = "#0000ff";
        if (i == (startDay == "Sunday" ? 0 : 6)) dayOfWeekCell.style.color = "#ff0000";
    }

    //Add Day Cells    
    datePicker.daysTable.addRow();
    var rowCount = 1;
    for (var i = 0; i < 42; i++) {
        var dayButton = this.DatePickerDayButton("dayButton" + i);
        dayButton.datePicker = datePicker;
        dayButton.style.margin = "1px";
        datePicker.dayButtons.push(dayButton);
        datePicker.daysTable.addCellInRow(rowCount, dayButton);
        if ((i + 1) % 7 == 0) { datePicker.daysTable.addRow(); rowCount++ }
    }

    //Separator2
    var sep2 = document.createElement("div");
    sep2.style.margin = "2px 0 2px 0";
    sep2.className = "stiDesignerDatePickerSeparator";
    datePicker.innerContent.appendChild(sep2);

    //Time
    var timeTable = this.CreateHTMLTable();
    timeTable.style.width = "100%";
    timeTable.className = "stiDesignerTextContainer";
    datePicker.innerContent.appendChild(timeTable);
    timeTable.addTextCell(this.loc.FormFormatEditor.Time ).setAttribute("style", "padding: 0 4px 0 4px; white-space: nowrap;");

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
        this.value = jsObject.formatDate(datePicker.key, "h:nn:ss");
        datePicker.action();
    };

    datePicker.repaintDays = function () {
        var month = this.key.getMonth();
        var year = this.key.getFullYear();
        var countDaysInMonth = jsObject.GetCountDaysOfMonth(year, month);
        var firstDay = jsObject.GetDayOfWeek(year, month, 1);
        var startDay = jsObject.options.datePickerFirstDayOfWeek == "Auto" ? jsObject.GetFirstDayOfWeek() : jsObject.options.datePickerFirstDayOfWeek;
        if (startDay == "Monday") firstDay--;
        else if (firstDay == 7 && startDay == "Sunday") firstDay = 0;

        for (var i = 0; i < 42; i++) {
            var numDay = i - firstDay + 1;
            var isSelectedDay = (numDay == this.key.getDate());
            var dayButton = this.dayButtons[i];

            if (!((i < firstDay) || (i - firstDay > countDaysInMonth - 1))) {
                dayButton.numberOfDay = numDay;
                dayButton.caption.innerHTML = numDay;
                dayButton.setEnabled(true);
                dayButton.setSelected(isSelectedDay);
            }
            else {
                dayButton.caption.innerHTML = "";
                dayButton.setEnabled(false);
            }
        }
    }

    datePicker.fill = function () {
        yearTextBox.value = this.key.getFullYear();
        monthDropDownList.setKey(this.key.getMonth());
        this.repaintDays();
        if (this.showTime) {
            this.time.value = jsObject.formatDate(this.key, "h:nn:ss");
        }
    }

    datePicker.onshow = function () {
        this.showTime = !this.parentDateControl.shortFormat;
        this.key = new Date(this.parentDateControl.key.toString());
        this.fill();
        sep2.style.display = this.showTime ? "" : "none";
        timeTable.style.display = this.showTime ? "" : "none";
    };

    datePicker.action = function () {
        this.parentDateControl.setKey(this.key);
        this.parentDateControl.action();
    };

    //Ovveride Methods
    datePicker.onmousedown = function () {
        if (!this.isTouchStartFlag) this.ontouchstart(true);
    }

    datePicker.ontouchstart = function (mouseProcess) {
        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);
        jsObject.options.datePickerPressed = this;
        this.isTouchStartTimer = setTimeout(function () {
            datePicker.isTouchStartFlag = false;
        }, 1000);
    }

    datePicker.changeVisibleState = function (state) {
        if (state) {
            jsObject.options.currentDatePicker = this;

            this.onshow();
            this.style.display = "";
            this.visible = true;
            this.style.overflow = "hidden";
            this.parentDateControl.setSelected(true);
            this.parentButton.setSelected(true);
            this.style.width = this.innerContent.offsetWidth + "px";
            this.style.height = this.innerContent.offsetHeight + "px";
            this.style.left = (jsObject.FindPosX(this.parentDateControl, "stiDesignerMainPanel")) + "px";

            var browserHeight = (window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight) - jsObject.FindPosY(jsObject.options.mainPanel);
            var browserWidth = (window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth) - jsObject.FindPosX(jsObject.options.mainPanel);

            var animationDirection =
                (jsObject.FindPosY(this.parentDateControl, "stiDesignerMainPanel") + this.parentDateControl.offsetHeight + this.offsetHeight > browserHeight) &&
                    (jsObject.FindPosY(this.parentDateControl, "stiDesignerMainPanel") - this.offsetHeight > 0)
                    ? "Up" : "Down";

            this.style.top = animationDirection == "Down"
                ? (jsObject.FindPosY(this.parentDateControl, "stiDesignerMainPanel") + this.parentDateControl.offsetHeight + 1) + "px"
                : (jsObject.FindPosY(this.parentDateControl, "stiDesignerMainPanel") - this.offsetHeight) + "px";

            this.innerContent.style.top = (animationDirection == "Down" ? -this.innerContent.offsetHeight : this.innerContent.offsetHeight) + "px";

            var resultLeftPos = parseInt(this.style.left);
            var resultTopPos = parseInt(this.style.top);

            if (resultTopPos < 0) {
                this.style.top = "10px";
            }
            else if (resultTopPos + this.innerContent.offsetHeight > browserHeight) {
                this.style.top = (browserHeight - this.innerContent.offsetHeight - 10) + "px";
            }

            if (resultLeftPos < 0) {
                this.style.left = "10px";
            }
            else if (resultLeftPos + this.innerContent.offsetWidth > browserWidth) {
                this.style.left = (browserWidth - this.innerContent.offsetWidth - 10) + "px";
            }

            var d = new Date();
            var endTime = d.getTime() + jsObject.options.menuAnimDuration;
            jsObject.ShowAnimationVerticalMenu(this, 0, endTime);
        }
        else {
            clearTimeout(this.innerContent.animationTimer);
            this.visible = false;
            this.parentDateControl.setSelected(false);
            this.parentButton.setSelected(false);
            this.style.display = "none";
            if (jsObject.options.currentDatePicker == this) jsObject.options.currentDatePicker = null;
        }
    }

    return datePicker;
}

StiMobileDesigner.prototype.DatePickerDayButton = function (name) {
    var button = this.SmallButton(name, null, "10", null, null, null, "stiDesignerDatePickerDayButton");
    var size = this.options.isTouchDevice ? "27px" : "25px";
    button.style.width = button.style.height = size;
    button.innerTable.style.width = "100%";
    button.caption.style.textAlign = "center";
    button.caption.style.padding = "0px";
    button.numberOfDay = 1;

    button.action = function () {
        this.datePicker.key.setDate(parseInt(this.numberOfDay));
        this.setSelected(true);
        this.datePicker.action();
        this.datePicker.changeVisibleState(false);
    }

    return button;
}

//Helper Methods
StiMobileDesigner.prototype.GetDayOfWeek = function (year, month) {
    var result = new Date(year, month, 1).getDay();
    if (result == 0) result = 7;
    return result;
}

StiMobileDesigner.prototype.GetCountDaysOfMonth = function (year, month) {
    var countDaysInMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
    var count = countDaysInMonth[month];

    if (month == 1)
        if (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0))
            count = 29;
        else
            count = 28;
    return count;
}