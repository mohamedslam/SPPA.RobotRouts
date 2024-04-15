
StiMobileDesigner.prototype.PageAndColumnBreakPropertiesGroup = function () {
    var pageAndColumnBreakPropertiesGroup = this.PropertiesGroup("pageAndColumnBreakPropertiesGroup", this.loc.PropertyCategory.PageColumnBreakCategory);
    pageAndColumnBreakPropertiesGroup.style.display = "none";

    //NewPageBefore
    var controlPropertyNewPageBefore = this.CheckBox("controlPropertyNewPageBefore");
    controlPropertyNewPageBefore.action = function () {
        this.jsObject.ApplyPropertyValue("newPageBefore", this.isChecked);
    }
    pageAndColumnBreakPropertiesGroup.container.appendChild(this.Property("newPageBefore", this.loc.PropertyMain.NewPageBefore, controlPropertyNewPageBefore));

    //NewPageAfter
    var controlPropertyNewPageAfter = this.CheckBox("controlPropertyNewPageAfter");
    controlPropertyNewPageAfter.action = function () {
        this.jsObject.ApplyPropertyValue("newPageAfter", this.isChecked);
    }
    pageAndColumnBreakPropertiesGroup.container.appendChild(this.Property("newPageAfter", this.loc.PropertyMain.NewPageAfter, controlPropertyNewPageAfter));

    //NewColumnBefore
    var controlPropertyNewColumnBefore = this.CheckBox("controlPropertyNewColumnBefore");
    controlPropertyNewColumnBefore.action = function () {
        this.jsObject.ApplyPropertyValue("newColumnBefore", this.isChecked);
    }
    pageAndColumnBreakPropertiesGroup.container.appendChild(this.Property("newColumnBefore", this.loc.PropertyMain.NewColumnBefore, controlPropertyNewColumnBefore));

    //NewColumnAfter
    var controlPropertyNewColumnAfter = this.CheckBox("controlPropertyNewColumnAfter");
    controlPropertyNewColumnAfter.action = function () {
        this.jsObject.ApplyPropertyValue("newColumnAfter", this.isChecked);
    }
    pageAndColumnBreakPropertiesGroup.container.appendChild(this.Property("newColumnAfter", this.loc.PropertyMain.NewColumnAfter, controlPropertyNewColumnAfter));

    //BreakIfLessThan
    var controlPropertyBreakIfLessThan = this.PropertyTextBox("controlPropertyBreakIfLessThan", this.options.propertyNumbersControlWidth);
    controlPropertyBreakIfLessThan.action = function () {
        var value = Math.abs(this.jsObject.StrToInt(this.value));
        this.value = (value > 100) ? 100 : value;
        this.jsObject.ApplyPropertyValue("breakIfLessThan", this.value);
    }
    pageAndColumnBreakPropertiesGroup.container.appendChild(this.Property("breakIfLessThan", this.loc.PropertyMain.BreakIfLessThan, controlPropertyBreakIfLessThan));

    //SkipFirst
    var controlPropertySkipFirst = this.CheckBox("controlPropertySkipFirst");
    controlPropertySkipFirst.action = function () {
        this.jsObject.ApplyPropertyValue("skipFirst", this.isChecked);
    }
    pageAndColumnBreakPropertiesGroup.container.appendChild(this.Property("skipFirst", this.loc.PropertyMain.SkipFirst, controlPropertySkipFirst));

    //LimitRows
    var controlPropertyLimitRows = this.PropertyExpressionControl("controlPropertyLimitRows", this.options.propertyControlWidth, false);
    controlPropertyLimitRows.action = function () {
        this.jsObject.ApplyPropertyValue("limitRows", StiBase64.encode(this.textBox.value));
    }
    pageAndColumnBreakPropertiesGroup.container.appendChild(this.Property("limitRows", this.loc.PropertyMain.LimitRows, controlPropertyLimitRows));

    return pageAndColumnBreakPropertiesGroup;
}