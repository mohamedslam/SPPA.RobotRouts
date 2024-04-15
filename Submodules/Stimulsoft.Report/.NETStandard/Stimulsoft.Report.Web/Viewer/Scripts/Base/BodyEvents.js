
//Document MouseUp
StiJsViewer.prototype.DocumentMouseUp = function (event) {
    this.options.formInDrag = false;

    if (this.gridHeaderInDrag) {
        this.controls.mainPanel.removeChild(this.gridHeaderInDrag);
        this.gridHeaderInDrag = false;
    }
}

//Document Mouse Move
StiJsViewer.prototype.DocumentMouseMove = function (event) {
    if (this.options.formInDrag) this.options.formInDrag[4].move(event);

    if (this.gridHeaderInDrag) {
        if (this.gridHeaderInDrag.beginingOffset < 10)
            this.gridHeaderInDrag.beginingOffset++;
        else
            this.gridHeaderInDrag.move(event);
    }
}