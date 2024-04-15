
StiJsViewer.prototype.FontList = function (name, width) {
    return this.DropDownList(name, width, null, this.GetFontNamesItems(), true);
}