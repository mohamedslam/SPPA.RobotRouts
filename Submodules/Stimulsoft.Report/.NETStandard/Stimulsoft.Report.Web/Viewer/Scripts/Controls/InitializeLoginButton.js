
StiJsViewer.prototype.LoginButton = function (name, caption, imageName, minWidth, tooltip) {
    var button = this.SmallButton(name, caption || "", imageName, tooltip, null, "stiJsViewerLoginButton");
    button.innerTable.style.width = "100%";
    button.style.height = "40px";
    button.style.minWidth = (minWidth || 80) + "px";
    button.caption.style.textAlign = "center";
    button.style.cursor = "pointer";

    return button;
}