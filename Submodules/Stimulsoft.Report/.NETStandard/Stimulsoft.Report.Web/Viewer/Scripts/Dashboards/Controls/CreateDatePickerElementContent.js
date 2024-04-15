
StiJsViewer.prototype.CreateDatePickerElementContent = function (element) {
    var jsObject = this;
    var elementAttrs = element.elementAttributes;
    var contentAttrs = elementAttrs.contentAttributes;

    var mainButton = this.SmallButton(null, " ", null, null, "Down", null, contentAttrs.settings);
    jsObject.ApplyAttributesToObject(mainButton.innerTable, elementAttrs);
    mainButton.innerTable.style.minHeight = "auto";
    mainButton.style.height = mainButton.innerTable.style.height = "100%";
    mainButton.style.border = "0";
    mainButton.style.overflow = "hidden";
    mainButton.caption.style.width = "100%";
    mainButton.caption.style.padding = "0 4px 0 4px";
    mainButton.arrow.style.width = mainButton.arrow.style.height = "16px";
    StiJsViewer.setImageSource(mainButton.arrow, this.options, this.collections, "Dashboards.IconCloseItem" + (contentAttrs.settings.isDarkStyle ? "White.png" : ".png"));

    element.contentPanel.appendChild(mainButton);
    element.contentPanel.style.overflow = "hidden";

    mainButton.update = function () {
        var filters = contentAttrs.filters;
        var firstDate = new Date();
        var secondDate = null;

        try {
            if (filters.length == 0) {
                if (contentAttrs.variableRangeValues) {
                    firstDate = new Date(contentAttrs.variableRangeValues.selectionStart);
                    secondDate = new Date(contentAttrs.variableRangeValues.selectionEnd);
                }
                else if (contentAttrs.variableValue) {
                    firstDate = new Date(contentAttrs.variableValue);
                }
                else {
                    if (contentAttrs.selectionMode == "AutoRange" && contentAttrs.autoRangeValues) {
                        firstDate = new Date(contentAttrs.autoRangeValues.selectionStart);
                        secondDate = new Date(contentAttrs.autoRangeValues.selectionEnd);
                    }
                    else if (contentAttrs.selectionMode == "Range") {
                        var values = jsObject.GetValuesByDateRangeName(jsObject.ConvertDateRangeSelectionToDateRangeKind(contentAttrs.initialRangeSelection));
                        if (values) {
                            firstDate = values[0];
                            secondDate = values[1];
                        }
                    }
                }
            }
            else {
                if (filters[0].value) firstDate = new Date(filters[0].value);
                if (filters[0].value2) secondDate = new Date(filters[0].value2);
            }
        }
        catch (e) { }

        var caption = firstDate.toLocaleDateString();
        if (secondDate) caption += " - " + secondDate.toLocaleDateString();

        mainButton.firstDate = firstDate;
        mainButton.secondDate = secondDate || new Date();

        var getStringDate = function (date) {
            var dateObj = jsObject.getDateTimeObject(date);
            return dateObj.month + "/" + dateObj.day + "/" + dateObj.year;
        }

        var dateValues = { value1: getStringDate(firstDate) };
        if (secondDate) dateValues.value2 = getStringDate(secondDate);

        jsObject.postAjax(jsObject.getActionRequestUrl(jsObject.options.requestUrl, jsObject.options.actions.viewerEvent),
            {
                action: "GetDatePickerFormattedValues",
                dateValues: dateValues,
                datePickerElementName: elementAttrs.name
            },
            function (answer) {
                if (answer) {
                    var data = JSON.parse(jsObject.options.server.useCompression ? StiGZipHelper.unpack(answer) : answer);
                    if (data && data.value1) {
                        caption = data.value1;
                        if (data.value2) caption += " - " + data.value2;
                    }
                }
                mainButton.caption.innerText = caption;
            });
    }

    mainButton.update();

    mainButton.action = function () {
        mainButton.isModified = false;

        if (contentAttrs.selectionMode == "Single") {
            var datePicker = jsObject.controls.datePicker;

            if (!datePicker.visible) {
                datePicker.key = this.firstDate;
                datePicker.parentButton = this;
                datePicker.changeVisibleState(true);
            }
            else {
                datePicker.changeVisibleState(false);
            }

            datePicker.action = function () {
                mainButton.isModified = true;
            }

            datePicker.onHide = function () {
                if (mainButton.isModified) {
                    contentAttrs.filters = [{ condition: contentAttrs.condition, value: jsObject.formatDate(this.key, "MM/dd/yyyy"), path: contentAttrs.columnPath }];
                    mainButton.update();
                    jsObject.ApplyFiltersToDashboardElement(element, contentAttrs.filters, true);
                }
                datePicker.onHide = null;
            }
        }
        else {
            var params = {
                firstParentButton: mainButton,
                secondParentButton: mainButton
            }

            var datePicker = jsObject.InitializeDoubleDatePicker(params);
            datePicker.firstDatePicker.key = this.firstDate;
            datePicker.secondDatePicker.key = this.secondDate;
            datePicker.changeVisibleState(!datePicker.visible, mainButton);

            datePicker.action = function () {
                mainButton.isModified = true;
            }

            datePicker.onHide = function () {
                if (mainButton.isModified) {
                    var value = jsObject.formatDate(this.firstDatePicker.key, "MM/dd/yyyy");
                    var value2 = jsObject.formatDate(this.secondDatePicker.key, "MM/dd/yyyy");
                    contentAttrs.filters = [{ condition: "Between", value: value, value2: value2, path: contentAttrs.columnPath }];
                    mainButton.update();
                    jsObject.ApplyFiltersToDashboardElement(element, contentAttrs.filters, true);
                }
                datePicker.onHide = null;
            }
        }
    }
}

StiJsViewer.prototype.ConvertDateRangeSelectionToDateRangeKind = function (rangeSelection) {
    switch (rangeSelection) {
        case "DayTomorrow": return "Tomorrow";
        case "DayToday": return "Today";
        case "DayYesterday": return "Yesterday";
        case "WeekNext": return "NextWeek";
        case "WeekCurrent": return "CurrentWeek";
        case "WeekPrevious": return "PreviousWeek";
        case "MonthNext": return "NextMonth";
        case "MonthCurrent": return "CurrentMonth";
        case "MonthPrevious": return "PreviousMonth";
        case "QuarterNext": return "NextQuarter";
        case "QuarterCurrent": return "CurrentQuarter";
        case "QuarterPrevious": return "PreviousQuarter";
        case "QuarterFirst": return "FirstQuarter";
        case "QuarterSecond": return "SecondQuarter";
        case "QuarterThird": return "ThirdQuarter";
        case "QuarterFourth": return "FourthQuarter";
        case "YearNext": return "NextYear";
        case "YearCurrent": return "CurrentYear";
        case "YearPrevious": return "PreviousYear";
        case "Last7Days": return "Last7Days";
        case "Last14Days": return "Last14Days";
        case "Last30Days": return "Last30Days";
        case "DateToWeek": return "WeekToDate";
        case "DateToMonth": return "MonthToDate";
        case "DateToQuarter": return "QuarterToDate";
        case "DateToYear": return "YearToDate";
    }

    return "Today";
}