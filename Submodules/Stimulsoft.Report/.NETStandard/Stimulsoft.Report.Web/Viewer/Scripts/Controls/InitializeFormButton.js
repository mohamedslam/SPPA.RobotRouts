
StiJsViewer.prototype.FormButton = function (name, caption, imageName, minWidth, styleName) {
    var button = this.SmallButton(name, caption || "", imageName, null, null, styleName || "stiJsViewerFormButton");
    button.innerTable.style.width = "100%";
    button.style.minWidth = (minWidth || 80) + "px";

    if (button.caption) {
        button.caption.style.textAlign = "center";
        button.caption.style.width = "100%";
    }

    return button;
}