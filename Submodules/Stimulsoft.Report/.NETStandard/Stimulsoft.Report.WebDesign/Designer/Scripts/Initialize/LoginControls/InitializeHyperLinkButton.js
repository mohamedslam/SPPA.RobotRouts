
StiMobileDesigner.prototype.HiperLinkButton = function (name, caption, height) {
    var hiperLinkButton = this.SmallButton(name, null, caption, null, null, null, "stiDesignerHyperlinkButton");
    if (height != null) hiperLinkButton.style.height = this.options.isTouchDevice ? (height + 5) + "px" : height + "px";
    hiperLinkButton.caption.style.padding = "";
    hiperLinkButton.style.fontSize = "14px";

    return hiperLinkButton;
}

StiMobileDesigner.prototype.HiperLinkButtonForAuthForm = function (name, caption, nowrap) {
    var hiperLinkButton = this.SmallButton(name, null, caption, null, null, null, "stiDesignerHyperlinkButton");
    hiperLinkButton.style.maxWidth = "300px";
    hiperLinkButton.style.fontSize = "14px";

    if (hiperLinkButton.caption) {
        hiperLinkButton.caption.style.padding = "";
        hiperLinkButton.caption.style.whiteSpace = nowrap ? "nowrap" : "normal";
    }

    return hiperLinkButton;
}