
StiMobileDesigner.prototype.HyperlinkMenu = function (parentButton) {
    var menu = this.VerticalMenu("hyperlinkMenu", parentButton, "Down");

    var innerCont = menu.innerContent;
    innerCont.style.textAlign = "right";
    innerCont.style.overflowX = "hidden";
    innerCont.style.overflowY = "hidden";

    var header = document.createElement("div");
    header.setAttribute("style", "margin: 12px 0 6px 12px;text-align: left;");
    header.innerHTML = this.loc.Editor.InsertLink;
    header.className = "stiDesignerFormHeader";
    innerCont.appendChild(header);

    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.padding = "6px 0 6px 6px";

    controlsTable.addTextCell("URL:").className = "stiDesignerCaptionControls";
    var urlControl = this.TextBox(null, 200);
    menu.urlControl = urlControl;
    controlsTable.addCell(urlControl).style.padding = "6px 12px 6px 12px";

    controlsTable.addTextCellInNextRow(this.loc.PropertyMain.Text + ":").className = "stiDesignerCaptionControls";
    var textControl = this.TextBox(null, 200);
    menu.textControl = textControl;
    controlsTable.addCellInLastRow(textControl).style.padding = "6px 12px 6px 12px";

    var buttonsTable = this.CreateHTMLTable();
    buttonsTable.style.display = "inline-block";
    var buttonOk = this.FormButton(null, null, this.loc.Buttons.Ok.replace("&", ""), null);
    buttonsTable.addCell(buttonOk).style.padding = "0 0 6px 0";
    var buttonCancel = this.FormButton(null, null, this.loc.Buttons.Cancel.replace("&", ""), null);
    buttonsTable.addCell(buttonCancel).style.padding = "0 12px 6px 6px";

    innerCont.appendChild(controlsTable);
    innerCont.appendChild(buttonsTable);

    menu.onshow = function () {
        textControl.value = "";
        urlControl.value = "";
        urlControl.focus();
    }

    buttonCancel.action = function () {
        menu.changeVisibleState(false);
    }

    buttonOk.action = function () {
        menu.changeVisibleState(false);
        menu.action();
    }

    menu.action = function () { }

    return menu;
}