
StiMobileDesigner.prototype.InitializeTextEditorFormOnlyText_ = function () {

    //Text Editor Form Only Text
    var textEditorFormOnlyText = this.BaseForm("textEditorOnlyText", this.loc.PropertyMain.Text, 4, this.HelpLinks["expression"]);
    textEditorFormOnlyText.showFunction = null;
    textEditorFormOnlyText.actionFunction = null;

    textEditorFormOnlyText.textArea = this.TextArea("textEditorOnlyTextTextArea", 600, 400);
    textEditorFormOnlyText.textArea.style.margin = "12px";
    textEditorFormOnlyText.container.appendChild(textEditorFormOnlyText.textArea);

    textEditorFormOnlyText.onshow = function () {
        if (this.showFunction) this.showFunction();
        this.textArea.focus();
    }

    textEditorFormOnlyText.action = function () {
        if (this.actionFunction) this.actionFunction();
        this.changeVisibleState(false);
    }

    return textEditorFormOnlyText;
}