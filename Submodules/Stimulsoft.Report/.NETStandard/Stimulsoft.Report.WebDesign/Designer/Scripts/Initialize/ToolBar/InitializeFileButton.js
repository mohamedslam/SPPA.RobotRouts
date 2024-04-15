
StiMobileDesigner.prototype.FileButton = function () {
    var caption = this.loc.MainMenu.menuFile;
    if (caption) caption = caption.replace("&", "");

    var button = this.SmallButton("fileButton", null, caption, null, null, null, this.isOffice2022Theme() ? "stiDesignerFormButtonTheme" : "stiDesignerToolButton", true);
    button.style.border = button.style.borderRadius = "0";
    button.style.height = "30px";
    button.style.minWidth = "70px";
    button.innerTable.style.width = "100%";
    button.caption.style.textAlign = "center";
    button.caption.style.padding = "0 10px 0 10px";

    return button;
}