
StiJsViewer.prototype.InitializeOpenDialog = function (nameDialog, actionFunction, fileMask) {
    if (this.controls[nameDialog]) {
        this.controls.mainPanel.removeChild(this.controls[nameDialog]);
    }
    var inputFile = document.createElement("input");
    this.controls.mainPanel.appendChild(inputFile);
    this.controls[nameDialog] = inputFile;
    inputFile.style.display = "none";
    inputFile.id = nameDialog;
    inputFile.jsObject = this;
    inputFile.setAttribute("type", "file");
    inputFile.setAttribute("name", "files[]");
    inputFile.setAttribute("multiple", "");
    if (fileMask) inputFile.setAttribute("accept", fileMask);

    this.addEvent(inputFile, 'change', function (evt) {
        var files = evt.target.files;
        var fileName = files[0] ? files[0].name : "Report";
        var filePath = evt.target.value;

        for (var i = 0; i < files.length; i++) {
            var f = files[i];
            var reader = new FileReader();
            reader.jsObject = this.jsObject;

            reader.onload = (function (theFile) {
                return function (e) {
                    inputFile.setAttribute("name", "files[]");
                    inputFile.setAttribute("multiple", "");
                    inputFile.setAttribute("value", "");
                    actionFunction(fileName, filePath, e.target.result);
                };
            })(f);

            reader.readAsDataURL(f);
        }
    });

    inputFile.action = function () {
        //this.style.display = "";
        this.focus();
        this.click();
        //this.style.display = "none";
    }

    return inputFile;
}