
StiMobileDesigner.prototype.InitializeImageForm_ = function () {
    var jsObject = this;
    var imageForm = this.BaseFormPanel("imageForm", this.loc.PropertyCategory.ImageCategory, 2, this.HelpLinks["image"]);
    imageForm.mode = "ImageSrc";
    var panelWidth = 650;
    var panelHeight = 500;
    var resourceIdent = this.options.resourceIdent;
    var variableIdent = this.options.variableIdent;

    //Main Table
    var mainTable = this.CreateHTMLTable();
    mainTable.className = "stiDesignerImageFormMainPanel";
    imageForm.container.appendChild(mainTable);
    imageForm.container.style.padding = "0px";

    //Buttons
    var buttonProps = [
        ["ImageSrc", "ImageForm.ImageImage.png", this.loc.PropertyCategory.ImageCategory],
        ["ImageDataColumn", "ImageForm.ImageDataColumn.png", this.loc.PropertyMain.DataColumn],
        ["ImageData", "ImageForm.ImageExpression.png", this.loc.PropertyMain.Expression],
        ["ImageUrl", "ImageForm.ImageHyperlink.png", this.loc.PropertyMain.Hyperlink],
        ["ImageFile", "ImageForm.ImageFile.png", this.loc.MainMenu.menuFile.replace("&", "")],
        ["ImageIcon", "ImageForm.ImageIcon.png", this.loc.PropertyMain.Icon]
    ];

    if (this.options.serverMode) {
        buttonProps.push(["ImageServer", "ImageForm.ImageCloud.png", "Server"]);
    }

    //Add Panels && Buttons
    var buttonsPanel = mainTable.addCell();
    var panelsContainer = mainTable.addCell();
    buttonsPanel.style.verticalAlign = "top";
    buttonsPanel.style.paddingTop = "6px";
    imageForm.mainButtons = {};
    imageForm.panels = {};

    for (var i = 0; i < buttonProps.length; i++) {
        var panel = document.createElement("Div");
        panel.className = "stiDesignerEditFormPanel";
        panel.style.display = i != 0 ? "none" : "inline-block";
        panel.style.height = panelHeight + "px";
        panel.style.width = panelWidth + "px";
        panelsContainer.appendChild(panel);
        imageForm.panels[buttonProps[i][0]] = panel;

        var button = this.FormTabPanelButton(null, buttonProps[i][2], buttonProps[i][1], buttonProps[i][2], null, { width: 24, height: 24 }, 34);
        imageForm.mainButtons[buttonProps[i][0]] = button;
        buttonsPanel.appendChild(button);
        button.panelName = buttonProps[i][0];
        button.action = function () {
            imageForm.setMode(this.panelName);
        }
    }

    //Image
    var imageSrcContainer = this.ImageControl(null, panelWidth - 28, panelHeight - 136);
    imageSrcContainer.style.margin = "12px";
    imageForm.panels.ImageSrc.appendChild(imageSrcContainer);

    imageSrcContainer.action = function () {
        imageSrcContainer.variableName = null;
        imageSrcContainer.resourceName = null;
        imageSrcContainer.columnName = null;
        imageUrlControl.value = "";
        imageUrlContainer.setImage(null);
        if (imageSrcGallery.selectedItem) {
            imageSrcGallery.selectedItem.select(false);
        }
    }

    var saveButton = this.SmallButton(null, null, null, "Save.png", this.loc.MainMenu.menuFileSave.replace("&", ""), null, "stiDesignerFormButton");
    saveButton.style.position = "absolute";
    saveButton.style.right = "4px";
    saveButton.style.top = this.options.isTouchDevice ? "36px" : "31px";
    imageSrcContainer.buttonsCell.appendChild(saveButton);

    saveButton.action = function () {
        var imageSrc = !imageSrcContainer.src ? "" : imageSrcContainer.src;
        var imageData = jsObject.options.mvcMode ? encodeURIComponent(imageSrc) : imageSrc;
        if (imageData) jsObject.SendCommandDownloadImageContent(imageData);
    }

    var moveToResButton = this.SmallButton(null, null, null, "Resources.Resource.png", this.loc.Buttons.MoveToResource, null, "stiDesignerFormButton");
    moveToResButton.style.position = "absolute";
    moveToResButton.style.right = "4px";
    moveToResButton.style.top = this.options.isTouchDevice ? "63px" : "58px";
    imageSrcContainer.buttonsCell.appendChild(moveToResButton);

    moveToResButton.action = function () {
        var imageSrc = !imageSrcContainer.src ? "" : imageSrcContainer.src;
        var imageData = jsObject.options.mvcMode ? encodeURIComponent(imageSrc) : imageSrc;
        if (imageData) {
            var resourceName = jsObject.GetNewName("Resource", null, "Image");

            jsObject.SendCommandToDesignerServer("MoveImageToResource", { resourceName: resourceName, imageData: imageData }, function (answer) {
                if (answer.itemObject) {
                    jsObject.options.dictionaryTree.addResource(answer.itemObject);
                    jsObject.options.report.dictionary.resources = answer.resources;
                    jsObject.UpdateStateUndoRedoButtons();

                    if (answer.imagesGallery) {
                        jsObject.options.imagesGallery = answer.imagesGallery;
                        imageForm.fillImageAndColumnsGalleries(answer.imagesGallery);
                        imageSrcGallery.changeVisibleState(true);

                        var item = imageSrcGallery.getItemByPropertyValue("name", resourceName);
                        if (item) item.action(true);
                    }
                }
            });
        }
    }

    imageSrcContainer.removeButton.style.top = this.options.isTouchDevice ? "90px" : "85px";

    imageSrcContainer.setImage_ = imageSrcContainer.setImage;

    imageSrcContainer.setImage = function (imageSrc, onLoadFunc) {
        this.setImage_(imageSrc, onLoadFunc);
        saveButton.setEnabled(imageSrc);
        moveToResButton.setEnabled(imageSrc);
    }

    var imageSrcGallery = this.ImageGallery(null, panelWidth, 100, this.loc.Report.Gallery);
    imageForm.panels.ImageSrc.appendChild(imageSrcGallery);

    imageSrcGallery.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        imageSrcContainer.resize(panelWidth - 28, state ? panelHeight - 150 : panelHeight - 26);
    }

    //Image Data Column
    var dataTreePlace = document.createElement("div");
    dataTreePlace.className = "stiSimpleContainerWithBorder";
    dataTreePlace.style.margin = "12px";
    dataTreePlace.style.overflow = "auto";
    dataTreePlace.style.width = (panelWidth - 28) + "px";
    dataTreePlace.style.height = (panelHeight - 150) + "px";
    var dataColumnTree = this.options.dataTree;
    imageForm.panels.ImageDataColumn.appendChild(dataTreePlace);

    var dataColumnGallery = this.ImageGallery(null, panelWidth, 100, this.loc.Report.Gallery);
    imageForm.panels.ImageDataColumn.appendChild(dataColumnGallery);

    dataColumnGallery.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        dataTreePlace.style.height = (state ? panelHeight - 150 : panelHeight - 26) + "px";
    }

    dataColumnGallery.action = function (item) {
        dataColumnTree.setKey(item.itemObject.name);
        dataColumnTree.autoscroll();
    }

    //Image Data
    var imageDataControl = this.TextArea("imageFormImageData", panelWidth - 32, panelHeight - 26);
    imageDataControl.style.margin = "12px";
    imageForm.panels.ImageData.appendChild(imageDataControl);
    imageDataControl.addInsertButton();

    //Image Url
    var tableImageUrl = this.CreateHTMLTable();
    tableImageUrl.style.width = "100%";
    imageForm.panels.ImageUrl.appendChild(tableImageUrl);

    var textCell = tableImageUrl.addTextCell(this.loc.PropertyMain.Hyperlink);
    textCell.style.paddingLeft = "12px";
    textCell.style.width = "100px";

    var imageUrlControl = this.TextBox(null, 500);
    imageUrlControl.setAttribute("placeholder", "http://site.com/image.png");
    imageUrlControl.style.margin = "12px 14px 12px 0";
    tableImageUrl.addCell(imageUrlControl).style.textAlign = "right";

    imageUrlControl.onkeyup = function () {
        clearTimeout(this.keyTimer);
        this.keyTimer = setTimeout(function () {
            imageUrlControl.action();
        }, 800);
    }

    var imageUrlContainer = this.ImageControl(null, panelWidth - 28, panelHeight - 185, true, true);
    imageUrlContainer.style.margin = "0 12px 12px 12px";
    imageForm.panels.ImageUrl.appendChild(imageUrlContainer);

    var imageUrlContextMenu = this.InitializeDeleteItemsContextMenu("imageUrlContextMenu");

    var imageUrlGallery = this.ImageGallery(null, panelWidth, 100, this.loc.Report.History, imageUrlContextMenu);
    imageForm.panels.ImageUrl.appendChild(imageUrlGallery);

    imageUrlContextMenu.action = function (item) {
        this.changeVisibleState(false);
        switch (item.key) {
            case "delete": {
                if (imageUrlGallery.selectedItem) {
                    imageForm.removeImageItemFromHistory(imageUrlGallery.selectedItem.itemObject.src, "StimulsoftMobileDesignerImageUrlHistory");
                    imageUrlGallery.selectedItem.remove();
                }
                break;
            }
            case "deleteAll": {
                imageUrlGallery.clear();
                jsObject.SetCookie("StimulsoftMobileDesignerImageUrlHistory", JSON.stringify([]));
                break;
            }
        }
    }

    imageUrlGallery.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        var height = state ? panelHeight - 185 : panelHeight - 61;
        if (jsObject.options.isTouchDevice) height -= 5;
        imageUrlContainer.resize(panelWidth - 28, height);
    }

    imageUrlGallery.action = function (item) {
        imageUrlContainer.setImage(item.itemObject.src);
        imageUrlControl.value = item.itemObject.src;
    }

    imageUrlControl.action = function () {
        if (this.value.indexOf(resourceIdent) == 0 || this.value.indexOf(variableIdent) == 0) {
            var ident = this.value.indexOf(resourceIdent) == 0 ? resourceIdent : variableIdent;
            var itemName = this.value.substring(this.value.indexOf(ident) + ident.length);
            var item = imageSrcGallery.getItemByPropertyValue("name", itemName);
            if (item) {
                item.action(true);
            }
            else {
                imageUrlContainer.setImage(null);
                imageSrcContainer.setImage(null);
                imageSrcContainer.variableName = null;
                imageSrcContainer.resourceName = null;
                imageSrcContainer.columnName = null;
                if (imageSrcGallery.selectedItem) {
                    imageSrcGallery.selectedItem.select(false);
                }
            }
        }
        else {
            imageUrlContainer.setImage(this.value);
        }
    }

    imageUrlControl.onmouseup = function () {
        if (jsObject.options.itemInDrag) {
            var dictionaryTree = jsObject.options.dictionaryTree;
            if (dictionaryTree.selectedItem) {
                this.value += dictionaryTree.selectedItem.getResultForEditForm();
            }
        }
    }

    imageUrlControl.ontouchend = function () { this.onmouseup(); }

    //File Name
    var tableFileName = this.CreateHTMLTable();
    tableFileName.style.width = "100%";
    imageForm.panels.ImageFile.appendChild(tableFileName);
    var textCell2 = tableFileName.addTextCell(this.loc.Cloud.labelFileName.replace(":", ""));
    textCell2.style.paddingLeft = "12px";
    textCell2.style.width = "100px";

    var fileNameControl = this.TextBoxWithOpenDialog("imageFormFileName", 500, ".bmp,.gif,.jpeg,.jpg,.png,.tiff,.ico,.emf,.wmf,.svg");
    if (!this.options.standaloneJsMode) fileNameControl.openButton.style.display = "none";
    fileNameControl.style.margin = "12px 14px 12px 0";
    tableFileName.addCell(fileNameControl).style.textAlign = "right";

    var fileNameContainer = this.ImageControl(null, panelWidth - 28, panelHeight - 171, true, false);
    fileNameContainer.style.margin = "0 12px 12px 12px";
    imageForm.panels.ImageFile.appendChild(fileNameContainer);
    fileNameContainer.style.display = "none";

    var fileNameGallery = this.ImageGallery(null, panelWidth, 100, this.loc.Report.History);
    imageForm.panels.ImageFile.appendChild(fileNameGallery);

    fileNameGallery.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        var height = state ? panelHeight - 171 : panelHeight - 61;
        if (jsObject.options.isTouchDevice) panelHeight -= 5;
        fileNameContainer.resize(panelWidth - 28, height);
    }

    imageSrcGallery.action = function (item) {
        imageSrcContainer.setImage(item.itemObject.src);
        imageSrcContainer.variableName = item.itemObject.type == "StiVariable" ? item.itemObject.name : null;
        imageSrcContainer.resourceName = item.itemObject.type == "StiResource" ? item.itemObject.name : null;
        imageSrcContainer.columnName = item.itemObject.type == "StiDataColumn" ? item.itemObject.name : null;
        imageUrlControl.value = item.itemObject.type == "StiResource"
            ? resourceIdent + item.itemObject.name
            : (item.itemObject.type == "StiVariable" ? variableIdent + item.itemObject.name : "");
        imageUrlContainer.setImage(item.itemObject.type == "StiResource" || item.itemObject.type == "StiVariable" ? item.itemObject.src : null);
        imageDataControl.value = item.itemObject.type == "StiVariable" ? "{" + item.itemObject.name + "}" : "";
        if (item.itemObject.type == "StiDataColumn") {
            var item = dataColumnGallery.getItemByPropertyValue("name", item.itemObject.name);
            if (item) item.action(true);
        }
    }

    //Image Icon
    var iconTable = this.CreateHTMLTable();
    iconTable.style.margin = "12px 12px 0 12px";
    imageForm.panels.ImageIcon.appendChild(iconTable);
    iconTable.addTextCell(this.loc.PropertyMain.Icon);

    var iconControl = this.IconControl("imageFormIcon", 80, null, null, null, true);
    iconControl.style.marginLeft = "50px";
    iconTable.addCell(iconControl);

    var iconColorControl = this.ColorControl("imageFormIconColor", null, true, 80, true);
    iconColorControl.style.marginLeft = "12px";
    iconTable.addCell(iconColorControl);

    var iconContainer = document.createElement("div");
    iconContainer.style.width = (panelWidth - 28) + "px";
    iconContainer.style.height = (this.options.isTouchDevice ? panelHeight - 69 : panelHeight - 64) + "px";
    iconContainer.className = "stiIconImageContainerWithBorder";
    iconContainer.style.margin = "12px";
    imageForm.panels.ImageIcon.appendChild(iconContainer);

    var iconContTable = this.CreateHTMLTable();
    iconContTable.style.width = iconContTable.style.height = "100%";
    iconContainer.appendChild(iconContTable);

    var iconCell = iconContTable.addCell();
    iconCell.style.verticalAlign = "middle";

    iconControl.action = function () {
        iconCell.innerHTML = this.textBox.value;
    }

    iconColorControl.action = function () {
        iconCell.style.color = iconControl.textBox.style.color = jsObject.GetHTMLColor(this.key);
    }

    //Image Server
    if (this.options.serverMode) {
        imageForm.cloudContainer = this.CloudContainer("imageFormCloudContainer", ["Image"], null, panelHeight);
        //imageForm.cloudContainer.style.margin = "8px auto 0 auto";
        imageForm.panels.ImageServer.appendChild(imageForm.cloudContainer);
        if (this.options.dictionaryTree.selectedItem) {
            this.options.dictionaryTree.selectedItem.setSelected();
        }
    }

    //Form Methods
    imageForm.reset = function () {
        imageSrcContainer.setImage(null);
        imageSrcContainer.variableName = null;
        imageSrcContainer.resourceName = null;
        imageSrcContainer.columnName = null;
        imageSrcGallery.clear();
        dataColumnTree.setKey("");
        dataColumnGallery.clear();
        imageDataControl.value = "";
        imageUrlControl.value = "";
        imageUrlContainer.setImage(null);
        fileNameControl.setValue("");
        fileNameContainer.setImage(null);
        fileNameGallery.clear();
        if (imageForm.cloudContainer) imageForm.cloudContainer.clear();
        imageForm.setMode("ImageSrc");
    }

    imageForm.setMode = function (mode) {
        imageForm.mode = mode;
        for (var panelName in imageForm.panels) {
            imageForm.panels[panelName].style.display = mode == panelName ? "inline-block" : "none";
            imageForm.mainButtons[panelName].setSelected(mode == panelName);
        }
        var propertiesPanel = jsObject.options.propertiesPanel;
        propertiesPanel.editFormControl = null;
        propertiesPanel.setEnabled(mode == "ImageUrl" || mode == "ImageData" || mode == "ImageServer");
        if (mode == "ImageUrl") {
            propertiesPanel.editFormControl = imageUrlControl;
            imageUrlControl.focus();
        }
        if (mode == "ImageData") {
            propertiesPanel.editFormControl = imageDataControl;
            imageDataControl.focus();
        }
        if (mode == "ImageServer") {
            propertiesPanel.editFormControl = imageForm.cloudContainer;
        }
        if (mode == "ImageFile") {
            fileNameControl.textBox.focus();
        }
    }

    imageForm.addImageItemToHistory = function (imageSrc, cookiesKey) {
        var historyStr = jsObject.GetCookie(cookiesKey);
        var historyArray = historyStr ? JSON.parse(historyStr) : [];
        var newItem = {
            name: jsObject.GetFileNameFromPath(imageSrc),
            src: imageSrc
        }
        var haveThisItem = false;
        for (var i = 0; i < historyArray.length; i++) {
            if (historyArray[i].name == newItem.name && historyArray[i].src == newItem.src) {
                haveThisItem = true;
                break;
            }
        }
        if (!haveThisItem) {
            if (historyArray.length > 9) historyArray.splice(9, 10);
            historyArray.splice(0, 0, newItem);
            jsObject.SetCookie(cookiesKey, JSON.stringify(historyArray));
        }
    }

    imageForm.removeImageItemFromHistory = function (imageSrc, cookiesKey) {
        var historyStr = jsObject.GetCookie(cookiesKey);
        var historyArray = historyStr ? JSON.parse(historyStr) : [];
        for (var i = 0; i < historyArray.length; i++) {
            if (historyArray[i].src == imageSrc) {
                historyArray.splice(i, 1);
                break;
            }
        }
        jsObject.SetCookie(cookiesKey, JSON.stringify(historyArray));
    }

    imageForm.fillImageAndColumnsGalleries = function (imagesGallery) {
        imageSrcGallery.progress.hide();
        dataColumnGallery.progress.hide();
        var allImages = [].concat(imagesGallery.variables, imagesGallery.resources, imagesGallery.columns);

        if (allImages.length > 0) {
            imageSrcGallery.addItems(allImages);
            if (imagesGallery.columns.length > 0) {
                dataColumnGallery.addItems(imagesGallery.columns);
            }
            else {
                dataColumnGallery.changeVisibleState(false);
            }
        }
        else {
            imageSrcGallery.changeVisibleState(false);
            dataColumnGallery.changeVisibleState(false);
        }
    }

    imageForm.onhide = function () {
        jsObject.options.propertiesPanel.setDictionaryMode(false);
    }

    imageForm.onshow = function () {
        jsObject.options.propertiesPanel.setDictionaryMode(true);

        //Data Tree Build
        dataTreePlace.appendChild(dataColumnTree);
        dataColumnTree.build(null, null, null, true);
        dataColumnTree.action = function () { imageForm.action(); }

        imageForm.reset();

        //Update galleries
        if (jsObject.options.imagesGallery || jsObject.CheckImagesInDictionary()) {
            imageSrcGallery.changeVisibleState(true);
            dataColumnGallery.changeVisibleState(true);
            imageSrcGallery.progress.show(280, -25);
            dataColumnGallery.progress.show(280, -25);

            if (!jsObject.options.imagesGallery) {
                jsObject.SendCommandToDesignerServer("GetImagesGallery", null, function (answer) {
                    jsObject.options.imagesGallery = answer.imagesGallery;
                    imageForm.fillImageAndColumnsGalleries(answer.imagesGallery);
                    var itemName = imageSrcContainer.variableName || imageSrcContainer.resourceName || imageSrcContainer.columnName;
                    if (itemName) {
                        var item = imageSrcGallery.getItemByPropertyValue("name", itemName);
                        if (item) item.action(true);
                    }
                });
            }
            else {
                imageForm.fillImageAndColumnsGalleries(jsObject.options.imagesGallery)
            }
        }
        else {
            imageSrcGallery.changeVisibleState(false);
            dataColumnGallery.changeVisibleState(false);
        }

        var imageUrlHistory = jsObject.GetCookie("StimulsoftMobileDesignerImageUrlHistory");
        imageUrlGallery.changeVisibleState(imageUrlHistory && JSON.parse(imageUrlHistory).length > 0);
        if (imageUrlHistory) imageUrlGallery.addItems(JSON.parse(imageUrlHistory));

        fileNameGallery.changeVisibleState(false);

        if (jsObject.options.selectedObjects) {
            imageForm.setMode("ImageSrc");
        }
        else {
            var selectedObject = jsObject.options.selectedObject;
            var props = ["imageSrc", "imageUrl", "imageFile", "imageDataColumn", "imageData"];
            for (var i = 0; i < props.length; i++) {
                imageForm[props[i]] = selectedObject.properties[props[i]] != null
                    ? (props[i] == "imageSrc" ? selectedObject.properties[props[i]] : StiBase64.decode(selectedObject.properties[props[i]]))
                    : null;
            }

            //Icon
            iconControl.setKey(selectedObject.properties.icon);
            iconColorControl.setKey(selectedObject.properties.iconColor);
            iconCell.style.color = iconControl.textBox.style.color = jsObject.GetHTMLColor(selectedObject.properties.iconColor);
            iconCell.innerHTML = iconControl.textBox.value;

            if (imageForm.imageSrc) {
                imageForm.setMode("ImageSrc");
                imageSrcContainer.setImage(imageForm.imageSrc);
                imageSrcContainer.variableName = null;
                imageSrcContainer.resourceName = null;
                imageSrcContainer.columnName = null;
            }
            else if (imageForm.imageUrl) {
                if (imageForm.imageUrl.indexOf(jsObject.options.cloudServerUrl) == 0) {
                    imageForm.setMode("ImageServer");
                    var key = imageForm.imageUrl.replace(jsObject.options.cloudServerUrl, "");
                    var item = jsObject.options.dictionaryTree.getCloudItemByKey("Image", key);
                    if (item && imageForm.cloudContainer) imageForm.cloudContainer.addItem(item.itemObject);
                } else {
                    if (imageForm.imageUrl.indexOf(resourceIdent) == 0 || imageForm.imageUrl.indexOf(variableIdent) == 0) {
                        var ident = imageForm.imageUrl.indexOf(resourceIdent) == 0 ? resourceIdent : variableIdent;
                        imageUrlControl.value = imageForm.imageUrl;
                        imageSrcContainer.resourceName = imageForm.imageUrl.substring(imageForm.imageUrl.indexOf(ident) + ident.length);
                        imageForm.setMode("ImageSrc");
                        if (jsObject.options.imagesGallery) {
                            var item = imageSrcGallery.getItemByPropertyValue("name", imageSrcContainer.resourceName);
                            if (item) {
                                item.action(true);
                            }
                            else {
                                imageForm.setMode("ImageUrl");
                            }
                        }
                    }
                    else {
                        imageForm.setMode("ImageUrl");
                        imageUrlControl.value = imageForm.imageUrl;
                        imageUrlContainer.setImage(imageForm.imageUrl);
                    }
                }
            }
            else if (imageForm.imageFile) {
                imageForm.setMode("ImageFile");
                fileNameControl.setValue(imageForm.imageFile);
            }
            else if (imageForm.imageDataColumn) {
                imageForm.setMode("ImageDataColumn");
                imageSrcContainer.columnName = imageForm.imageDataColumn;
                if (jsObject.options.imagesGallery) {
                    var item = dataColumnGallery.getItemByPropertyValue("name", imageForm.imageDataColumn);
                    if (item) item.select(true);
                }
                dataColumnTree.setKey(imageForm.imageDataColumn);
                setTimeout(function () { dataColumnTree.autoscroll(); });
            }
            else if (imageForm.imageData) {
                imageDataControl.value = imageForm.imageData;
                var variableName = imageForm.imageData.length > 1 ? imageForm.imageData.substring(1, imageForm.imageData.length - 1) : "";
                var variable = jsObject.GetVariableByNameFromDictionary(variableName);
                if (variable) {
                    imageSrcContainer.variableName = variableName;
                    imageForm.setMode("ImageSrc");
                    if (jsObject.options.imagesGallery) {
                        var item = imageSrcGallery.getItemByPropertyValue("name", variableName);
                        if (item) item.action(true);
                    }
                }
                else {
                    imageForm.setMode("ImageData");
                }
            }
            else if (selectedObject.properties.icon) {
                imageForm.setMode("ImageIcon");
            }
        }

        if (jsObject.options.designerSpecification != "Developer" && !imageForm.imageData) {
            imageForm.setMode("ImageSrc");
            imageForm.mainButtons.ImageData.style.display = "none";
        }
    }

    imageForm.action = function () {
        this.changeVisibleState(false);
        var selectedObjects = jsObject.options.selectedObjects || [jsObject.options.selectedObject];
        var propertyNames = ["imageSrc", "imageUrl", "imageFile", "imageDataColumn", "imageData", "icon", "iconColor"];

        for (var i = 0; i < selectedObjects.length; i++) {
            var selectedObject = selectedObjects[i];

            for (var k = 0; k < propertyNames.length; k++) {
                selectedObject.properties[propertyNames[k]] = propertyNames[k] == "iconColor" ? iconColorControl.key : "";
            }

            switch (imageForm.mode) {
                case "ImageSrc":
                    {
                        if (imageSrcContainer.variableName != null) {
                            selectedObject.properties.imageData = StiBase64.encode("{" + imageSrcContainer.variableName + "}");
                        }
                        else if (imageSrcContainer.resourceName != null) {
                            selectedObject.properties.imageUrl = StiBase64.encode(resourceIdent + imageSrcContainer.resourceName);
                        }
                        else if (imageSrcContainer.columnName != null) {
                            selectedObject.properties.imageDataColumn = StiBase64.encode(imageSrcContainer.columnName);
                        }
                        else {
                            var srcValue = !imageSrcContainer.src ? "" : imageSrcContainer.src;
                            selectedObject.properties.imageSrc = jsObject.options.mvcMode ? encodeURIComponent(srcValue) : srcValue;
                        }
                        break;
                    }
                case "ImageUrl":
                    {
                        selectedObject.properties.imageUrl = StiBase64.encode(imageUrlControl.value);
                        if (imageUrlControl.value && imageUrlContainer.src &&
                            imageUrlControl.value.indexOf(resourceIdent) != 0 &&
                            imageUrlControl.value.indexOf(variableIdent) != 0) {
                            imageForm.addImageItemToHistory(imageUrlControl.value, "StimulsoftMobileDesignerImageUrlHistory");
                        }
                        break;
                    }
                case "ImageFile":
                    {
                        selectedObject.properties.imageFile = StiBase64.encode(fileNameControl.getValue());
                        break;
                    }
                case "ImageDataColumn":
                    {
                        var dataColumnFullName = dataColumnTree.selectedItem && dataColumnTree.selectedItem.itemObject &&
                            (dataColumnTree.selectedItem.itemObject.typeItem == "Column" || dataColumnTree.selectedItem.itemObject.typeItem == "Parameter")
                            ? dataColumnTree.selectedItem.getFullName(true) : "";
                        selectedObject.properties.imageDataColumn = StiBase64.encode(dataColumnFullName);
                        break;
                    }
                case "ImageData":
                    {
                        selectedObject.properties.imageData = StiBase64.encode(imageDataControl.value);
                        break;
                    }
                case "ImageIcon":
                    {
                        selectedObject.properties.icon = iconControl.key;
                        break;
                    }
                case "ImageServer":
                    {
                        if (imageForm.cloudContainer && imageForm.cloudContainer.item && jsObject.options.cloudServerUrl) {
                            selectedObject.properties.imageUrl = StiBase64.encode(jsObject.options.cloudServerUrl + imageForm.cloudContainer.item.itemObject.key);
                        }
                        else
                            selectedObject.properties.imageUrl = "";
                        break;
                    }
            }
        }

        jsObject.SendCommandSendProperties(selectedObjects, propertyNames);
    }

    return imageForm;
}