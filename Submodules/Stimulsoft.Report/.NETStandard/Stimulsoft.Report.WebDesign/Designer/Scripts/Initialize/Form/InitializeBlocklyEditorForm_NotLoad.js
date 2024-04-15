
StiMobileDesigner.prototype.InitializeBlocklyEditorForm_ = function () {
    var jsObject = this;
    var form = this.BaseFormPanel("blocklyEditor", "Blockly", 3);
    form.container.style.border = "0";

    var switchButton = this.FormButton(null, null, this.loc.Buttons.SwitchTo.replace("{0}", this.loc.FormDesigner.Code), null);
    switchButton.style.display = "inline-block";
    switchButton.style.margin = "12px";

    switchButton.action = function () {
        var messageForm = jsObject.MessageFormForSwitchingEventMode();
        messageForm.changeVisibleState(true);

        messageForm.action = function (state) {
            if (state) {
                var control = form.resultControl.textBox || form.resultControl;
                control.value = control.hiddenValue = "";
                form.changeVisibleState(false);

                jsObject.InitializeEventEditorForm(function (eventForm) {
                    var propertiesPanel = jsObject.options.propertiesPanel;
                    eventForm.propertiesPanelZIndex = propertiesPanel.style.zIndex;
                    eventForm.propertiesPanelIsEnabled = propertiesPanel.isEnabled;
                    eventForm.resultControl = form.resultControl;
                    eventForm.changeVisibleState(true);
                });
            }
            else {
                messageForm.changeVisibleState(false);
            }
        }
    }

    var footerTable = this.CreateHTMLTable();
    footerTable.style.width = "100%";
    var buttonsPanel = form.buttonsPanel;
    form.removeChild(buttonsPanel);
    form.appendChild(footerTable);
    footerTable.addCell(switchButton).style.width = "1px";
    footerTable.addCell();
    footerTable.addCell(form.buttonOk).style.width = "1px";
    footerTable.addCell(form.buttonCancel).style.width = "1px";

    var mainTable = this.CreateHTMLTable();
    form.container.appendChild(mainTable);

    var container = document.createElement("div");
    container.className = "stiBlocklyContainer";
    mainTable.addCell(container);

    var host = document.createElement("div");
    host.className = host.id = "stiBlocklyHost";
    container.appendChild(host);

    var emptyBlock = document.createElement("div");
    emptyBlock.className = "stiBlocklyDivEmpty";
    form.container.appendChild(emptyBlock);

    var emptyImg = document.createElement("img");
    emptyImg.className = "stiBlocklyImageEmpty";
    StiMobileDesigner.setImageSource(emptyImg, this.options, "Blockly.BlocklyEmpty.png", false);
    emptyBlock.appendChild(emptyImg);

    var emptyText = document.createElement("div");
    emptyText.className = "stiBlocklyTextEmpty";
    emptyText.innerText = this.loc.Dashboard.DragDropBlocks;
    emptyBlock.appendChild(emptyText);

    var rightToolbar = this.CreateHTMLTable();
    rightToolbar.style.margin = "30px 0 0 -55px";
    rightToolbar.style.position = "absolute";
    mainTable.addCell(rightToolbar).style.verticalAlign = "top";

    var openButton = this.FormImageButton(null, "Open.png", this.loc.Buttons.Open);
    var saveButton = this.FormImageButton(null, "Save.png", this.loc.Buttons.Save);
    var sampleButton = this.FormImageButton(null, "ConnectionString.Info.png", this.loc.FormFormatEditor.Sample);

    saveButton.style.marginTop = sampleButton.style.marginTop = "6px";
    sampleButton.style.display = "none" //Temporarily

    rightToolbar.addCell(openButton);
    rightToolbar.addCellInNextRow(saveButton);
    rightToolbar.addCellInNextRow(sampleButton);

    openButton.action = function () {
        if (jsObject.options.canOpenFiles) {
            jsObject.InitializeOpenDialog("loadBlocksFromFile", function (evt) {
                var files = evt.target.files;

                for (var i = 0; i < files.length; i++) {
                    var f = files[i];
                    var reader = new FileReader();

                    reader.onload = (function () {
                        return function (e) {
                            jsObject.ResetOpenDialogs();
                            form.insertXmlToBlockly(jsObject.GetDecodedFileContent(e.target.result));
                            jsObject.ReturnFocusToDesigner();

                        };
                    })(f);

                    reader.readAsDataURL(f);
                }
            }, ".blockly");
            jsObject.options.openDialogs.loadBlocksFromFile.action();
        }
    }

    saveButton.action = function () {
        jsObject.SendCommandSaveBlockly(form.getXmlFromBlockly(), form.eventName);
    }

    form.blocklyInitialized = function () {
        return (typeof Blockly != "undefined" && Blockly.mainWorkspace);
    }

    form.clearBlockly = function () {
        if (form.blocklyInitialized()) {
            Blockly.mainWorkspace.clear();
        }
        while (host.childNodes[0]) {
            host.removeChild(host.childNodes[0]);
        }
        this.horScroll = null;
        this.vertScroll = null;
    }

    form.getXmlFromBlockly = function () {
        if (form.blocklyInitialized()) {
            var dom = Blockly.Xml.workspaceToDom(Blockly.mainWorkspace);
            return Blockly.Xml.domToText(dom);
        }
        return "";
    }

    form.insertXmlToBlockly = function (xml) {
        if (form.blocklyInitialized()) {
            var dom = Blockly.Xml.textToDom(xml);
            Blockly.mainWorkspace.clear();
            Blockly.Xml.domToWorkspace(dom, Blockly.mainWorkspace);
        }
    }

    form.getBlockCount = function () {
        return Blockly.mainWorkspace.getAllBlocks().length;
    }

    form.checkState = function () {
        var countBlocks = form.getBlockCount();
        emptyBlock.style.display = countBlocks > 0 ? "none" : "block";

        form.findScrolls();
        if (form.horScroll && form.vertScroll) {
            form.horScroll.style.display = form.vertScroll.style.display = countBlocks > 0 ? "" : "none";
        }
    }

    form.findScrolls = function () {
        if (!this.horScroll && !this.horScroll) {
            var hScrolls = host.getElementsByClassName("blocklyScrollbarHorizontal");
            if (hScrolls && hScrolls.length > 0) this.horScroll = hScrolls[0];
            var vScrolls = host.getElementsByClassName("blocklyScrollbarVertical");
            if (vScrolls && vScrolls.length > 0) this.vertScroll = vScrolls[0];
        }
    }

    form.initBlockly = function (eventValue, showCurrentValue) {
        jsObject.SendCommandToDesignerServer("GetBlocklyInitParameters", { eventValue: StiBase64.encode(eventValue), showCurrentValue: showCurrentValue }, function (answer) {
            var toolboxXML = StiBase64.decode(answer.params.toolboxXML);
            var workspaceXML = StiBase64.decode(answer.params.workspaceXML);
            var initBlocksJsCode = StiBase64.decode(answer.params.initBlocksJsCode);

            if (form.initBlocksScript)
                form.initBlocksScript.parentElement.removeChild(form.initBlocksScript);

            form.initBlocksScript = jsObject.InitializeScriptText(initBlocksJsCode);
            // eslint-disable-next-line no-undef
            initBlocklyBlocks();

            var blocklyOptions = {
                toolbox: toolboxXML,
                zoom: {
                    controls: true,
                    wheel: true,
                    startScale: 1,
                    maxScale: 3,
                    minScale: 0.3,
                    scaleSpeed: 1.2
                }
            }

            Blockly.inject('stiBlocklyHost', blocklyOptions);
            Blockly.mainWorkspace.addChangeListener(form.checkState);

            form.insertXmlToBlockly(workspaceXML);
        });
    }

    form.onhide = function () {
        form.clearBlockly();
    }

    form.show = function (eventValue, eventName) {
        this.changeVisibleState(true);
        this.eventName = eventName;
        var showCurrentValue = eventName == "StiGetDataUrlEvent" || eventName == "StiGetExcelSheetEvent" || eventName == "StiGetExcelValueEvent" || eventName == "StiGetValueEvent" || eventName == "StiGetTextEvent";

        form.clearBlockly();
        form.initBlockly(eventValue, showCurrentValue);
    }

    form.action = function () {
        if (this.resultControl) {
            var countBlocks = form.getBlockCount() > 0;
            var resultTextBox = this.resultControl.textBox || this.resultControl;
            resultTextBox.value = countBlocks ? "[" + jsObject.loc.PropertyMain.Blocks + "]" : "";
            resultTextBox.hiddenValue = countBlocks > 0 ? form.getXmlFromBlockly() : "";
            if (this.resultControl.action) this.resultControl.action();
        }
        this.changeVisibleState(false);
    }

    return form;
}