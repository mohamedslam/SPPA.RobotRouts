
StiMobileDesigner.prototype.InitializeRichTextEditorForm_ = function () {
    var form = this.BaseForm("richTextEditorForm", this.loc.PropertyMain.Text, 3);
    form.richTextEditor = this.RichTextEditor("richTextEditorFormMainControl", this.options.isTouchDevice ? 695 : 630, 400);
    form.richTextEditor.richText.style.margin = "12px 12px 0 12px";
    form.container.appendChild(form.richTextEditor);

    form.show = function (text, control) {
        form.changeVisibleState(true);
        form.richTextEditor.setText(text);
        var richTextEditor = form.richTextEditor;
        var showExp = control && control.interactionIdent;
        richTextEditor.controls.buttonClear.parentElement.style.display = showExp ? "" : "none";
        richTextEditor.controls.expButton.parentElement.style.display = showExp ? "" : "none";
        richTextEditor.controls.buttonUnorderedList.parentElement.style.display = !showExp ? "" : "none";
        richTextEditor.controls.buttonOrderedList.parentElement.style.display = !showExp ? "" : "none";
        richTextEditor.controls.expButton.style.display = control.interactionIdent != "Text" && control.interactionIdent != "Image" ? "" : "none";
        richTextEditor.controls.expMenu.addItems(form.jsObject.GetInsertExpressionItems(control ? control.interactionIdent : null, control ? control.columnNames : null, control ? control.chartIsRange : null));
        richTextEditor.controls.buttonHyperlink.parentElement.style.display = showExp ? "" : "none";
        richTextEditor.controls.buttonHyperlink.style.display = showExp ? "" : "none";
    }

    return form;
}