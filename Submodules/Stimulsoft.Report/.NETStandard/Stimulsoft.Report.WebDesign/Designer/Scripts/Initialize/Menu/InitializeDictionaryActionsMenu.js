
StiMobileDesigner.prototype.InitializeDictionaryActionsMenu = function () {
    var menu = this.VerticalMenu("dictionaryActionsMenu", this.options.dictionaryPanel.toolBar.controls["Actions"], "Down", this.GetDictionaryActionsItems());
    var jsObject = this;

    menu.action = function (menuItem) {
        this.changeVisibleState(false);
        switch (menuItem.key) {
            case "newDictionary":
                {
                    var msgForm = jsObject.MessageFormNewDictionary();
                    msgForm.changeVisibleState(true);
                    msgForm.action = function (state) {
                        if (state) jsObject.SendCommandNewDictionary();
                    }
                    break;
                }
            case "openDictionary":
                {
                    if (jsObject.options.canOpenFiles) {
                        jsObject.InitializeOpenDialog("loadDictionaryFromFile", jsObject.StiHandleOpenDictionary, ".dct");
                        jsObject.options.openDialogs.loadDictionaryFromFile.action();
                    }
                    break;
                }
            case "mergeDictionary":
                {
                    if (jsObject.options.canOpenFiles) {
                        jsObject.InitializeOpenDialog("mergeDictionaryFromFile", jsObject.StiHandleMergeDictionary, ".dct");
                        jsObject.options.openDialogs.mergeDictionaryFromFile.action();
                    }
                    break;
                }
            case "saveDictionary":
                {
                    jsObject.SendCommandSaveDictionary();
                    break;
                }
            case "synchronize":
                {
                    jsObject.SendCommandSynchronizeDictionary();
                    break;
                }
            case "embedAllDataToResources":
                {
                    var msgForm = jsObject.MessageFormEmbedsAllData();
                    msgForm.changeVisibleState(true);
                    msgForm.action = function (state) {
                        if (state) jsObject.SendCommandEmbedAllDataToResources();
                    }
                    break;
                }
        }
    }

    return menu;
}