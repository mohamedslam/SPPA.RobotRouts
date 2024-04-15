
StiMobileDesigner.prototype.InitializeEditConnectionForm_ = function () {

    //Edit Connection Form
    var jsObject = this;
    var editConnectionForm = this.BaseForm("editConnectionForm", this.loc.Database.Connection, 3, this.HelpLinks["connectionEdit"]);
    editConnectionForm.connection = null;
    editConnectionForm.mode = "Edit";

    var moveToResource = this.FormButton(null, null, this.loc.Buttons.MoveToResource, null);
    moveToResource.style.display = "inline-block";
    moveToResource.style.margin = "12px";

    moveToResource.action = function () {
        jsObject.SendCommandMoveConnectionDataToResource(editConnectionForm);
    }

    moveToResource.updateEnabledState = function () {
        var isXmlConnection = editConnectionForm.connection.typeConnection && jsObject.EndsWith(editConnectionForm.connection.typeConnection, "StiXmlDatabase");

        this.setEnabled(
            (editConnectionForm.pathDataControl.textBox.value &&
                editConnectionForm.pathDataControl.textBox.value.indexOf(jsObject.options.resourceIdent) < 0) ||
            (isXmlConnection && editConnectionForm.xmlTypeControl.key == "AdoNetXml" && editConnectionForm.pathSchemaControl.textBox.value &&
                editConnectionForm.pathSchemaControl.textBox.value.indexOf(jsObject.options.resourceIdent) < 0)
        );
    }

    var footerTable = this.CreateHTMLTable();
    footerTable.style.width = "100%";
    var buttonsPanel = editConnectionForm.buttonsPanel;
    editConnectionForm.removeChild(buttonsPanel);
    editConnectionForm.appendChild(footerTable);
    footerTable.addCell(moveToResource).style.width = "1px";
    footerTable.addCell();
    footerTable.addCell(editConnectionForm.buttonOk).style.width = "1px";
    footerTable.addCell(editConnectionForm.buttonCancel).style.width = "1px";

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "5px 0 5px 0";
    editConnectionForm.container.appendChild(innerTable);

    var csvSeparatorTable = this.CreateHTMLTable();
    editConnectionForm.csvSeparatorControl = this.DropDownList(null, 120, null, this.GetCsvSeparatorItems(), true, false, null, true);
    editConnectionForm.csvSeparatorTextControl = this.TextBox(null, 80);
    csvSeparatorTable.addCell(editConnectionForm.csvSeparatorControl);
    csvSeparatorTable.addCell(editConnectionForm.csvSeparatorTextControl).style.paddingLeft = "10px";

    editConnectionForm.csvSeparatorControl.action = function () {
        editConnectionForm.csvSeparatorTextControl.style.display = this.key == "Other" ? "" : "none";
    }

    var textSchema = "<b>File: </b>c:\\file.{0}  |  <b>Web: </b> https://www.site.com/schema";
    var textData = "<b>File: </b>c:\\file.{0}  |  <b>Web: </b> https://www.site.com/data";
    var textWithFolder = "<b>File: </b>c:\\file.{0}  |  <b>Folder: </b> c:\\folder\  |  <b>Web: </b> https://www.site.com/data";

    var controlProps = [
        ["name", this.loc.PropertyMain.Name, this.TextBox(null, 180)],
        ["alias", this.loc.PropertyMain.Alias, this.TextBox(null, 180)],
        ["quickBooksUseApp", "", this.CheckBox(null, this.loc.FormDatabaseEdit.UseOwnClientID)],
        ["quickBooksClientId", this.loc.FormDatabaseEdit.ClientId, this.TextBox(null, 400)],
        ["quickBooksClientSecret", this.loc.FormDatabaseEdit.ClientSecret, this.TextBox(null, 400)],
        ["quickBooksRedirectUrl", this.loc.FormDatabaseEdit.RedirectUrl, this.TextBox(null, 400)],
        ["quickBooksGetAuthCode", "", this.FormButton(null, null, this.loc.FormDatabaseEdit.GetAuthorizationCode, null, null, null, true)],
        ["quickBooksAuthorizationCode", this.loc.FormDatabaseEdit.AuthorizationCode, this.TextBox(null, 400)],
        ["quickBooksRealmId", "RealmId", this.TextBox(null, 400)],
        ["quickBooksGetTokens", "", this.FormButton(null, null, this.loc.FormDatabaseEdit.GetTokens, null, null, null, true)],
        ["quickBooksAccessToken", this.loc.FormDatabaseEdit.AccessToken, this.TextBox(null, 400)],
        ["quickBooksRefreshToken", this.loc.FormDatabaseEdit.RefreshToken, this.TextBox(null, 400)],
        ["quickBooksRefreshAccessToken", "", this.FormButton(null, null, this.loc.FormDatabaseEdit.RefreshAccessToken, null, null, null, true)],
        ["xmlType", this.loc.FormDatabaseEdit.XmlType.replace(":", ""), this.DropDownList("editConnectionFormXmlType", 250, null, this.GetXmlTypeItems(), true, false, null, true)],
        ["serviceAccountKeyFile", this.loc.FormDatabaseEdit.ServiceAccountKeyFile, this.TextAreaWithOpenDialog("connectionFormServiceAccountKeyFile", 300, 100, ".json")],
        ["gglAnalyticAccount", this.loc.Cloud.Account, this.DropDownList("editConnectionFormGglAnalyticsAccount", 300, null, [], true, false, null, true)],
        ["gglAnalyticProperty", this.loc.PropertyMain.Property, this.DropDownList("editConnectionFormGglAnalyticsProperty", 300, null, [], true, false, null, true)],
        ["gglAnalyticView", this.loc.Cloud.ButtonView, this.DropDownList("editConnectionFormGglAnalyticsView", 300)],
        ["gglAnalyticMetrics", this.loc.Dashboard.Metrics, this.TreeControl("editConnectionFormGglAnalyticsMetrics", 300, true, this.loc.Editor.BetweenMetrics)],
        ["gglAnalyticDimensions", this.loc.Dashboard.Dimensions, this.TreeControl("editConnectionFormGglAnalyticsDimensions", 300, true, this.loc.Editor.MaximumDimensions)],
        ["gglAnalyticStartDate", this.loc.Cloud.LabelStartDate.replace(":", ""), this.DropDownList("editConnectionFormGglAnalyticsStartDate", 144, null, this.GglAnalyticDateType(), true, false, null, true)],
        ["gglAnalyticStartDatePicker", this.loc.Cloud.LabelStartDate.replace(":", ""), this.DateControl("editConnectionFormGglAnalyticsStartDatePicker", 144, null, true)],
        ["gglAnalyticStartDateEnumerator", this.loc.Cloud.LabelStartDate.replace(":", ""), this.TextBoxEnumerator("editConnectionFormGglAnalyticsStartDateEnumerator", 144, null, false, null, 0)],
        ["gglAnalyticEndDate", this.loc.Cloud.LabelEndDate.replace(":", ""), this.DropDownList("editConnectionFormGglAnalyticsEndDate", 144, null, this.GglAnalyticDateType(), true, false, null, true)],
        ["gglAnalyticEndDatePicker", "", this.DateControl("editConnectionFormGglAnalyticsEndDatePicker", 144, null, true)],
        ["gglAnalyticEndDateEnumerator", "", this.TextBoxEnumerator("editConnectionFormGglAnalyticsEndDateEnumerator", 144, null, false, null, 0)],
        ["graphQLEndPoint", "Url", this.TextBox(null, 400)],
        ["graphQLQuery", this.loc.QueryBuilder.Query, this.TextArea(null, 400, 150)],
        ["graphQLHeaders", this.loc.PropertyMain.Headers, this.InitializeManualDataControl(406, 145, [["Key", this.loc.PropertyMain.Key], ["Value", this.loc.PropertyMain.Value]], 5)],
        ["pathSchema", this.loc.FormDatabaseEdit.PathSchema.replace(":", ""), this.TextBoxWithOpenDialog("connectionFormPathSchema", 280)],
        ["pathData", this.loc.FormDatabaseEdit.PathToData.replace(":", ""), this.TextBoxWithOpenDialog("connectionFormPathData", 280)],
        ["gisDataType", this.loc.PropertyMain.DataType, this.DropDownList("editConnectionFormGisDataType", 144, null, this.GetGisDataTypeItems(), true, false, null, true)],
        ["gisDataSeparator", this.loc.Export.Separator.replace(":", ""), this.TextBox("editConnectionFormGisDataSeparator", 144)],
        ["relationDirection", this.loc.FormDatabaseEdit.RelationDirection.replace(":", ""), this.DropDownList("editConnectionFormRelationDirection", 180, null, this.GetRelationDirectionItems(), true, false, null, true)],
        ["accountName", this.loc.FormDatabaseEdit.AccountName, this.TextBox(null, 300)],
        ["accountKey", this.loc.FormDatabaseEdit.AccountKey, this.TextArea(null, 300, 80)],
        ["containerName", this.loc.FormDatabaseEdit.ContainerName, this.DropDownList("editConnectionFormContainerName", 180, null, [], false, false, null, true)],
        ["blobName", this.loc.FormDatabaseEdit.BlobName, this.DropDownList("editConnectionFormBlobName", 180, null, [], false, false, null, true)],
        ["blobContentType", this.loc.FormDatabaseEdit.BlobContentType, this.DropDownList("editConnectionFormBlobContentType", 180, null, this.GetAzureBlobContentTypeItems(), true, false, null, true)],
        ["codePageDbase", this.loc.Export.Encoding.replace(":", ""), this.DropDownList(null, 300, null, this.GetDBaseCodePageItems(), true, false, null, true)],
        ["codePageCsv", this.loc.Export.Encoding.replace(":", ""), this.DropDownList(null, 120, null, this.GetCsvCodePageItems(), true, false, null, true)],
        ["csvSeparatorTable", this.loc.Export.Separator.replace(":", ""), csvSeparatorTable],
        ["clientId", this.loc.FormDatabaseEdit.ClientId, this.TextBox(null, 300)],
        ["clientSecret", this.loc.FormDatabaseEdit.ClientSecret, this.TextBox(null, 300)],
        ["spreadsheetId", this.loc.FormDatabaseEdit.SpreadsheetId, this.TextBox(null, 300)],
        ["owner", this.loc.Cloud.TextOwner, this.TextBox(null, 400)],
        ["database", this.loc.QueryBuilder.Database, this.TextBox(null, 400)],
        ["token", this.loc.FormDatabaseEdit.Token, this.TextBox(null, 400)],
        ["databaseSecret", this.loc.FormDatabaseEdit.DatabaseSecret, this.TextBox(null, 400)],
        ["dataUrl", this.loc.PropertyMain.DataUrl, this.TextBox(null, 400)],
        ["projectId", "Project ID", this.TextBox(null, 400)],
        ["datasetId", "Dataset ID", this.TextBox(null, 400)],
        ["serviceAccountKey", this.loc.FormDatabaseEdit.ServiceAccountKeyFile, this.FileEditorControl(null, 400, 200, ".json")],
        ["firstRowIsHeader", "", this.CheckBox(null, this.loc.FormDatabaseEdit.FirstRowIsHeader)]
    ]

    //PathToData
    for (var i = 0; i < controlProps.length; i++) {
        editConnectionForm[controlProps[i][0] + "ControlRow"] = innerTable.addRow();
        var text = innerTable.addCellInLastRow();
        text.className = "stiDesignerCaptionControlsBigIntervals";
        text.innerHTML = controlProps[i][1];
        var control = controlProps[i][2];
        control.textCell = text;
        editConnectionForm[controlProps[i][0] + "Control"] = control;
        innerTable.addCellInLastRow(control).className = "stiDesignerControlCellsBigIntervals2";
    }

    editConnectionForm.gglAnalyticStartDateControl.style.display = editConnectionForm.gglAnalyticEndDateControl.style.display = "inline-block";

    editConnectionForm.gglAnalyticStartDatePickerControl.style.display = editConnectionForm.gglAnalyticStartDateEnumeratorControl.style.display =
        editConnectionForm.gglAnalyticEndDatePickerControl.style.display = editConnectionForm.gglAnalyticEndDateEnumeratorControl.style.display = "none";
    editConnectionForm.gglAnalyticStartDatePickerControl.style.marginLeft = editConnectionForm.gglAnalyticStartDateEnumeratorControl.style.marginLeft =
        editConnectionForm.gglAnalyticEndDatePickerControl.style.marginLeft = editConnectionForm.gglAnalyticEndDateEnumeratorControl.style.marginLeft = "6px";

    editConnectionForm.gglAnalyticStartDateControl.parentElement.appendChild(editConnectionForm.gglAnalyticStartDatePickerControl);
    editConnectionForm.gglAnalyticStartDateControl.parentElement.appendChild(editConnectionForm.gglAnalyticStartDateEnumeratorControl);
    editConnectionForm.gglAnalyticEndDateControl.parentElement.appendChild(editConnectionForm.gglAnalyticEndDatePickerControl);
    editConnectionForm.gglAnalyticEndDateControl.parentElement.appendChild(editConnectionForm.gglAnalyticEndDateEnumeratorControl);

    editConnectionForm.gglAnalyticStartDatePickerControlRow.style.display = "none";
    editConnectionForm.gglAnalyticStartDateEnumeratorControlRow.style.display = "none";
    editConnectionForm.gglAnalyticEndDatePickerControlRow.style.display = "none";
    editConnectionForm.gglAnalyticEndDateEnumeratorControlRow.style.display = "none";

    editConnectionForm.gglAnalyticStartDateControl.action = function () {
        editConnectionForm.updateGglAnaliticsControlsVisibleStates();
    }

    editConnectionForm.gglAnalyticEndDateControl.action = function () {
        editConnectionForm.updateGglAnaliticsControlsVisibleStates();
    }

    editConnectionForm.firstRowIsHeaderControl.style.margin = "3px 0 3px 0";
    editConnectionForm.quickBooksUseAppControl.style.margin = "3px 0 3px 0";
    editConnectionForm.pathSchemaControl.textBox.style.padding = "0px 20px 0px 4px";
    editConnectionForm.pathDataControl.textBox.style.padding = "0px 20px 0px 4px";
    StiMobileDesigner.setImageSource(editConnectionForm.pathSchemaControl.openButton.image, this.options, "ThreeDots.png");
    StiMobileDesigner.setImageSource(editConnectionForm.pathDataControl.openButton.image, this.options, "ThreeDots.png");

    var pathDataHintText = document.createElement("div");
    var pathSchemaHintText = document.createElement("div");
    pathDataHintText.className = pathSchemaHintText.className = "stiConnectionPathHintText";
    editConnectionForm.pathDataControl.parentElement.appendChild(pathDataHintText);
    editConnectionForm.pathSchemaControl.parentElement.appendChild(pathSchemaHintText);
    editConnectionForm.pathDataControl.textCell.style.paddingBottom = editConnectionForm.pathSchemaControl.textCell.style.paddingBottom = "12px";

    //Additional methods
    jsObject.AddQuickBooksMethodsToConnectionForm(editConnectionForm);
    jsObject.AddAzureBlobStorageMethodsToConnectionForm(editConnectionForm);
    jsObject.AddGoogleAnalyticsMethodsToConnectionForm(editConnectionForm);

    //Resources Gallery
    var resourcesGallery = this.ImageGallery(null, 500, 85, this.loc.PropertyMain.Resources);
    editConnectionForm.container.appendChild(resourcesGallery);

    resourcesGallery.update = function (typeConnection) {
        this.clear();
        if (!jsObject.options.report) return;

        var typeConnection = editConnectionForm.connection.typeConnection;
        var types = [];
        switch (typeConnection) {
            case "StiXmlDatabase": types = ["Xml", "Xsd"]; break;
            case "StiJsonDatabase": types = ["Json"]; break;
            case "StiDBaseDatabase": types = ["Dbf"]; break;
            case "StiCsvDatabase": types = ["Csv"]; break;
            case "StiExcelDatabase": types = ["Excel"]; break;
        }

        var resources = jsObject.options.report.dictionary.resources;
        var resourceIdent = jsObject.options.resourceIdent;
        var pathResourceName = editConnectionForm.pathDataControl.textBox.value.indexOf(resourceIdent) == 0 ? editConnectionForm.pathDataControl.textBox.value.substring(resourceIdent.length) : null;
        var schemaResourceName = editConnectionForm.pathSchemaControl.textBox.value.indexOf(resourceIdent) == 0 ? editConnectionForm.pathSchemaControl.textBox.value.substring(resourceIdent.length) : null;

        for (var i = 0; i < resources.length; i++) {
            if (types.indexOf(resources[i].type) >= 0) {
                var itemObject = jsObject.CopyObject(resources[i]);
                itemObject.imageName = "Resources.BigResource" + resources[i].type;
                var item = this.addItem(itemObject);

                item.action = function () {
                    resourcesGallery.action(this);
                }

                item.select = function (state) {
                    if (state) {
                        var items = resourcesGallery.getItems();
                        for (var i = 0; i < items.length; i++) {
                            if (this.itemObject.type == items[i].itemObject.type && items[i].isSelected) {
                                items[i].setSelected(false);
                            }
                        }
                        this.setSelected(true);
                    }
                    else {
                        this.setSelected(false);
                    }
                }

                if (itemObject.name == pathResourceName || itemObject.name == schemaResourceName) {
                    item.select(true);
                }
            }
        };

        if (!this.innerContainer.innerTable) this.style.display = "none";
    }

    resourcesGallery.action = function (item) {
        item.select(true);
        editConnectionForm[item.itemObject.type == "Xsd" ? "pathSchemaControl" : "pathDataControl"].textBox.value = jsObject.options.resourceIdent + item.itemObject.name;

        if (item.itemObject.type == "Xsd" && editConnectionForm.xmlTypeControl.key == "Xml") {
            editConnectionForm.xmlTypeControl.setKey("AdoNetXml");
            editConnectionForm.xmlTypeControl.action();
        }

        editConnectionForm.pathDataControl.action();
    }

    //Open Dialogs for not Standalone mode
    if (!this.options.standaloneJsMode) {
        var pathDataOpenButton = editConnectionForm.pathDataControl.openButton;
        var pathSchemaOpenButton = editConnectionForm.pathSchemaControl.openButton;

        var openFile = function (control) {
            if (jsObject.options.canOpenFiles) {
                var openDialog = jsObject.InitializeOpenDialog(control.name, function (evt) {
                    var files = evt.target.files;
                    if (files && files.length > 0) {
                        var file = files[0];
                        var reader = new FileReader();
                        reader.onload = (function () {
                            return function (e) {
                                jsObject.AddResourceFile(file, e.target.result, function (resourceObject) {
                                    if (resourceObject) {
                                        editConnectionForm.loadedResources.push(resourceObject.name);
                                        control.textBox.value = "resource://" + resourceObject.name;
                                        resourcesGallery.style.display = "";
                                        resourcesGallery.update();
                                    }
                                });
                            };
                        })(file);
                        reader.readAsDataURL(file);
                    }
                }, control.filesMask);
                openDialog.action();
            }
        }

        pathSchemaOpenButton.action = function () {
            openFile(editConnectionForm.pathSchemaControl);
        }

        pathDataOpenButton.action = function () {
            openFile(editConnectionForm.pathDataControl);
        }
    }

    //Controls actions
    editConnectionForm.xmlTypeControl.action = function () {
        editConnectionForm.pathSchemaControlRow.style.display = this.key == "AdoNetXml" ? "" : "none";
        editConnectionForm.relationDirectionControlRow.style.display = this.key == "Xml" ? "" : "none";
    }

    editConnectionForm.nameControl.action = function () {
        if (editConnectionForm.mode == "New" && editConnectionForm.connection.name != editConnectionForm.nameControl.value) editConnectionForm.nameIsChanged = true;
        if (this.oldValue == editConnectionForm.aliasControl.value) {
            editConnectionForm.aliasControl.value = this.value;
        }
    }

    editConnectionForm.aliasControl.action = function () {
        if (editConnectionForm.mode == "New" && editConnectionForm.connection.alias != editConnectionForm.aliasControl.value) editConnectionForm.aliasIsChanged = true;
    }

    editConnectionForm.pathDataControl.action = function () {
        if (editConnectionForm.mode == "New" && !editConnectionForm.nameIsChanged) {
            var newName = jsObject.GetConnectionNameFromPathData(this.textBox.value) || editConnectionForm.connection.name;
            editConnectionForm.nameControl.value = newName;

            if (jsObject.options.report) {
                var i = 2;
                while (!editConnectionForm.nameControl.checkExists(jsObject.options.report.dictionary.databases, "name")) {
                    editConnectionForm.nameControl.value = newName + i;
                    i++;
                }
                editConnectionForm.nameControl.hideError();
            }

            if (!editConnectionForm.aliasIsChanged) editConnectionForm.aliasControl.value = editConnectionForm.nameControl.value;
        }

        moveToResource.updateEnabledState();
    }

    editConnectionForm.pathDataControl.textBox.onchange = function () {
        editConnectionForm.pathDataControl.action();
    }

    editConnectionForm.pathSchemaControl.textBox.onchange = function () {
        moveToResource.updateEnabledState();
    }

    editConnectionForm.pathSchemaControl.action = function () {
        moveToResource.updateEnabledState();
    }

    editConnectionForm.gisDataTypeControl.action = function () {
        editConnectionForm.gisDataSeparatorControlRow.style.display = this.key == "Wkt" ? "" : "none";
        editConnectionForm.pathDataControl.filesMask = this.key == "Wkt" ? ".wkt" : ".json";
        pathDataHintText.innerHTML = textData.replace("{0}", this.key == "Wkt" ? ".wkt" : ".json");
    }

    //Connection String
    editConnectionForm.connectionStringRow = innerTable.addRow();
    var connectionStringText = innerTable.addCellInLastRow();
    connectionStringText.className = "stiDesignerCaptionControlsBigIntervals";
    connectionStringText.style.paddingTop = "10px";
    connectionStringText.style.verticalAlign = "top";
    connectionStringText.innerHTML = this.loc.FormDatabaseEdit.ConnectionString.replace(":", "");

    var connectStrTable = this.CreateHTMLTable();
    connectStrTable.className = "stiColorControlWithBorder";
    innerTable.addCellInLastRow(connectStrTable).className = "stiDesignerControlCellsBigIntervals2";

    var connectionStringControl = this.TextArea("editConnectionFormConnectionStringTextBox", 380, 180);
    connectionStringControl.style.border = "0px";
    connectionStringControl.style.overflowY = "hidden";
    editConnectionForm.connectionStringControl = connectionStringControl;
    connectStrTable.addCell(connectionStringControl);

    var connectStrButtons = this.CreateHTMLTable();
    connectStrTable.addCell(connectStrButtons).style.verticalAlign = "top";

    var buttonProps = [
        ["buildConnection", "ConnectionString.Build.png", this.loc.Buttons.Build.replace("...", "")],
        ["cleanConnection", "ConnectionString.Clean.png", this.loc.MainMenu.menuEditClearContents],
        ["testConnection", "ConnectionString.Check.png", this.loc.DesignerFx.TestConnection],
        ["infoConnection", "ConnectionString.Info.png", this.loc.PropertyMain.ConnectionString]
    ]

    for (var i = 0; i < buttonProps.length; i++) {
        var button = this.SmallButton(null, null, null, buttonProps[i][1], buttonProps[i][2], null, "stiDesignerFormButton", true);
        button.style.margin = "4px 4px 0 4px";
        editConnectionForm[buttonProps[i][0]] = button;
        connectStrButtons.addRow();
        connectStrButtons.addCellInLastRow(button);
    }

    //Test Connection
    editConnectionForm.testConnection.action = function () {
        this.setEnabled(false);
        jsObject.SendCommandTestConnection(
            editConnectionForm.connection,
            StiBase64.encode(connectionStringControl.value),
            editConnectionForm.connection.serviceName
        );
    }

    //Build Connection
    editConnectionForm.buildConnection.action = function () {
        switch (editConnectionForm.connection.typeConnection) {
            case "StiODataDatabase": {
                jsObject.InitializeODataConnectionForm(function (oDataConnectionForm) {
                    oDataConnectionForm.show(connectionStringControl.value);

                    oDataConnectionForm.action = function () {
                        connectionStringControl.value = this.getConnectionString();
                        this.changeVisibleState(false);
                    };
                });
                break;
            }
        }
    }

    //Clean Connection
    editConnectionForm.cleanConnection.action = function () {
        connectionStringControl.value = "";
    }

    //Info Connection
    editConnectionForm.infoConnection.action = function () {
        jsObject.SendCommandGetSampleConnectionString(editConnectionForm.connection.typeConnection, editConnectionForm.connection.serviceName, function (answer) {
            if (answer.connectionString) {
                connectionStringControl.value = answer.connectionString;
            }
        });
    }

    //PromptUserNameAndPassword
    editConnectionForm.promptUserNameAndPasswordRow = innerTable.addRow();
    innerTable.addCellInLastRow().className = "stiDesignerCaptionControlsBigIntervals";
    editConnectionForm.promptUserNameAndPasswordControl = this.CheckBox("editConnectionFormPromptUserNameAndPassword", this.loc.FormDatabaseEdit.PromptUserNameAndPassword);
    editConnectionForm.promptUserNameAndPasswordControl.style.margin = "6px 0 6px 0";
    innerTable.addCellInLastRow(editConnectionForm.promptUserNameAndPasswordControl).className = "stiDesignerControlCellsBigIntervals2";

    //ODataVersion
    editConnectionForm.versionRow = innerTable.addRow();
    innerTable.addCellInLastRow().className = "stiDesignerCaptionControlsBigIntervals";
    editConnectionForm.versionControl = this.CheckBox("editConnectionFormVersion", "OData V3");
    editConnectionForm.versionControl.style.margin = "6px 0 6px 0";
    innerTable.addCellInLastRow(editConnectionForm.versionControl).className = "stiDesignerControlCellsBigIntervals2";

    //OAuthV2
    editConnectionForm.oAuthV2Row = innerTable.addRow();
    innerTable.addCellInLastRow().className = "stiDesignerCaptionControlsBigIntervals";
    var oAuthV2Control = this.CheckBox("editConnectionFormVersion", "OAuth V2");
    editConnectionForm.oAuthV2Control = oAuthV2Control;
    oAuthV2Control.style.margin = "6px 0 6px 0";
    innerTable.addCellInLastRow(oAuthV2Control).className = "stiDesignerControlCellsBigIntervals2";

    oAuthV2Control.action = function () {
        editConnectionForm.tokenControl.setEnabled(!this.isChecked);
    }

    //CosmosDB API
    editConnectionForm.apiControlRow = innerTable.addRow();
    innerTable.addTextCellInLastRow("API").className = "stiDesignerCaptionControlsBigIntervals";
    editConnectionForm.apiControl = this.DropDownList(null, 140, null, this.GetCosmoDBApiItems(), true, false, null, true);
    innerTable.addCellInLastRow(editConnectionForm.apiControl).className = "stiDesignerControlCellsBigIntervals2";

    editConnectionForm.getCsvSeparatorType = function (sepText) {
        switch (sepText) {
            case "": return "System";
            case "\t": return "Tab"
            case ";": return "Semicolon";
            case ",": return "Comma";
            case " ": return "Space";
            default: return "Other";
        }
    }

    editConnectionForm.getCsvSeparatorText = function (sepType) {
        switch (sepType) {
            case "System": return "";
            case "Tab": return "\t";
            case "Semicolon": return ";";
            case "Comma": return ",";
            case "Space": return " ";
            case "Other": return this.csvSeparatorTextControl.value;
        }
    }

    editConnectionForm.getConnectionStringKey = function (key, connectionString, splitSymbols) {
        if (!connectionString) return null;
        var strs = connectionString.split(splitSymbols || /;|,/);
        var address = null;

        for (var i = 0; i < strs.length; i++) {
            if (strs[i].toLowerCase().indexOf(key.toLowerCase()) == 0) {
                address = strs[i];
                break;
            }
        }

        if (address == null) return null;
        var startIndex = address.indexOf('=');

        var pairs = address.split('=');
        if (pairs.length < 2) return null;

        var value = address.substr(startIndex + 1, address.length - startIndex - 1);

        if (value.indexOf("\"") == 0 && jsObject.EndsWith(value, "\""))
            value = value.substr(1, value.length - 2);

        return value;
    }

    editConnectionForm.setConnectionStringKey = function (connectionString, key, value) {
        if (value == "") return connectionString;
        if (!connectionString) return key + "=" + value;

        var strs = connectionString.split(/;|,/);
        var strs2 = [];

        for (var i = 0; i < strs.length; i++) {
            if (strs[i].toLowerCase().indexOf(key.toLowerCase()) != 0) {
                strs2.push(strs[i]);
            }
        }
        return strs2.join(";", strs2) + ";" + key + "=" + value;
    }

    editConnectionForm.onshow = function () {
        this.mode = "Edit";
        this.nameIsChanged = false;
        this.aliasIsChanged = false;
        this.loadedResources = [];

        if (typeof (this.connection) == "string") {
            this.connection = jsObject.ConnectionObject(this.connection);
            if (this.connection.typeConnection == "StiCustomDatabase")
                this.connection.serviceName = this.serviceName;
            this.mode = "New";
        } else if (this.connection.isRecentConnection) {
            this.mode = "New";
        }
        var caption = (this.connection.typeConnection && (jsObject.EndsWith(this.connection.typeConnection, "StiXmlDatabase") || jsObject.EndsWith(this.connection.typeConnection, "StiJsonDatabase")))
            ? jsObject.loc.FormDatabaseEdit[(this.connection.typeConnection && jsObject.EndsWith(this.connection.typeConnection, "StiXmlDatabase") ? "Xml" : "Json") + this.mode]
            : jsObject.loc.FormDatabaseEdit[this.mode + "Connection"].replace("{0}", jsObject.GetConnectionNames(this.connection.typeConnection, true));

        this.editableDictionaryItem = this.mode == "Edit" && jsObject.options.dictionaryTree
            ? jsObject.options.dictionaryTree.selectedItem : null;

        if (caption) this.caption.innerHTML = caption;
        this.csvSeparatorTextControl.value = "";
        this.csvSeparatorTextControl.style.display = "none";
        this.nameControl.hideError();
        this.nameControl.focus();
        this.nameControl.value = this.connection.name;
        this.aliasControl.value = this.connection.alias;
        var isXmlConnection = this.connection.typeConnection && jsObject.EndsWith(this.connection.typeConnection, "StiXmlDatabase");
        var isJsonConnection = this.connection.typeConnection && jsObject.EndsWith(this.connection.typeConnection, "StiJsonDatabase");
        var isDBaseConnection = this.connection.typeConnection && jsObject.EndsWith(this.connection.typeConnection, "StiDBaseDatabase");
        var isCsvConnection = this.connection.typeConnection && jsObject.EndsWith(this.connection.typeConnection, "StiCsvDatabase");
        var isExcelConnection = this.connection.typeConnection && jsObject.EndsWith(this.connection.typeConnection, "StiExcelDatabase");
        var isGisDataConnection = this.connection.typeConnection && jsObject.EndsWith(this.connection.typeConnection, "StiGisDatabase");
        var isGoogleSheetsConnection = this.connection.typeConnection == "StiGoogleSheetsDatabase";
        var isODataConnection = this.connection.typeConnection == "StiODataDatabase";
        var isCosmosDbConnection = this.connection.typeConnection == "StiCosmosDbDatabase";
        var isMSAccessConnection = this.connection.typeConnection == "StiMSAccessDatabase";
        var isFileDataConnection = isXmlConnection || isJsonConnection || isDBaseConnection || isCsvConnection || isExcelConnection || isGisDataConnection;
        var isDataWorldConnection = this.connection.typeConnection == "StiDataWorldDatabase";
        var isQuickBooksConnection = this.connection.typeConnection == "StiQuickBooksDatabase";
        var isFirebaseConnection = this.connection.typeConnection == "StiFirebaseDatabase";
        var isBigQueryConnection = this.connection.typeConnection == "StiBigQueryDatabase";
        var isAzureBlobStorageConnection = this.connection.typeConnection == "StiAzureBlobStorageDatabase";
        var isGoogleAnalyticsConnection = this.connection.typeConnection == "StiGoogleAnalyticsDatabase";
        var isGraphQLConnection = this.connection.typeConnection == "StiGraphQLDatabase";

        if (isXmlConnection) {
            this.pathDataControl.textCell.innerHTML = jsObject.loc.FormDatabaseEdit.PathData.replace(":", "");
            if (this.connection.pathSchema != null) this.pathSchemaControl.textBox.value = StiBase64.decode(this.connection.pathSchema);
            this.xmlTypeControl.setKey(this.connection.xmlType);
            this.pathSchemaControl.filesMask = ".xsd";
            this.pathDataControl.filesMask = ".xml";
            this.relationDirectionControl.setKey(this.connection.relationDirection);
            pathDataHintText.innerHTML = textData.replace("{0}", "xml");
            pathSchemaHintText.innerHTML = textSchema.replace("{0}", "xsd");
        }
        else if (isJsonConnection) {
            this.pathDataControl.textCell.innerHTML = jsObject.loc.FormDatabaseEdit.PathJsonData.replace(":", "");
            this.pathDataControl.filesMask = ".json";
            this.relationDirectionControl.setKey(this.connection.relationDirection);
            pathDataHintText.innerHTML = textData.replace("{0}", "json");
        }
        else if (isDBaseConnection) {
            this.pathDataControl.textCell.innerHTML = jsObject.loc.FormDatabaseEdit.PathToData.replace(":", "");
            this.codePageDbaseControl.setKey(this.connection.codePage);
            this.pathDataControl.filesMask = "*";
            pathDataHintText.innerHTML = textWithFolder.replace("{0}", "dbf");
        }
        else if (isCsvConnection) {
            this.pathDataControl.textCell.innerHTML = jsObject.loc.FormDatabaseEdit.PathToData.replace(":", "");
            this.codePageCsvControl.setKey(this.connection.codePage);
            var sepType = this.getCsvSeparatorType(this.connection.separator);
            this.csvSeparatorControl.setKey(sepType);
            this.csvSeparatorTextControl.style.display = sepType == "Other" ? "" : "none";
            if (sepType == "Other") this.csvSeparatorTextControl.value = this.connection.separator;
            this.pathDataControl.filesMask = ".csv";
            pathDataHintText.innerHTML = textWithFolder.replace("{0}", "csv");;
        }
        else if (isExcelConnection) {
            this.pathDataControl.textCell.innerHTML = jsObject.loc.FormDatabaseEdit.PathToData.replace(":", "");
            this.firstRowIsHeaderControl.setChecked(this.connection.firstRowIsHeader);
            this.pathDataControl.filesMask = ".xlsx,.xls";
            pathDataHintText.innerHTML = textWithFolder.replace("{0}", "xlsx");
        }
        else if (isGisDataConnection) {
            this.gisDataTypeControl.setKey(this.connection.dataType);
            this.gisDataSeparatorControl.value = this.connection.separator;
            this.pathDataControl.textCell.innerHTML = jsObject.loc.FormDatabaseEdit.PathGisData.replace(":", "");
            this.pathDataControl.filesMask = this.connection.dataType == "Wkt" ? ".wkt" : ".json";
            pathDataHintText.innerHTML = textData.replace("{0}", this.connection.dataType == "Wkt" ? ".wkt" : ".json");
        }
        else {
            this.connectionStringControl.value = "";

            if (isCosmosDbConnection) {
                var connectStr = StiBase64.decode(this.connection.connectionString);
                var indexApi = connectStr.indexOf(";Api=");
                this.connectionStringControl.value = indexApi > 0 ? connectStr.substring(0, indexApi) : connectStr;
                var apiString = this.getConnectionStringKey("Api", connectStr);
                this.apiControl.setKey(apiString || "SQL");
            }
            else if (isFirebaseConnection) {
                var connectStr = StiBase64.decode(this.connection.connectionString);
                var basePath = this.getConnectionStringKey("BasePath", connectStr) || "";
                var authSecret = this.getConnectionStringKey("AuthSecret", connectStr) || "";
                this.dataUrlControl.value = basePath;
                this.databaseSecretControl.value = authSecret;
            }
            else if (isGoogleSheetsConnection) {
                this.clientIdControl.value = StiBase64.decode(this.connection.clientId);
                this.clientSecretControl.value = StiBase64.decode(this.connection.clientSecret);
                this.spreadsheetIdControl.value = StiBase64.decode(this.connection.spreadsheetId);
                this.firstRowIsHeaderControl.setChecked(this.connection.firstRowIsHeader);
            }
            else if (isGraphQLConnection) {
                var connectStr = StiBase64.decode(this.connection.connectionString);
                this.graphQLEndPointControl.value = this.getConnectionStringKey("EndPoint", connectStr) || "";
                this.graphQLQueryControl.value = this.getConnectionStringKey("Query", connectStr) || "";
                this.graphQLHeadersControl.setValue(StiBase64.decode(this.getConnectionStringKey("Headers", connectStr) || ""));
            }
            else if (isQuickBooksConnection) {
                if (this.connection.connectionString != null) {
                    var connectStr = StiBase64.decode(this.connection.connectionString);
                    var useAppStr = this.getConnectionStringKey("UseApp", connectStr);
                    var useApp = useAppStr && useAppStr.toLowerCase() == "true";
                    var clientId = this.getConnectionStringKey("ClientId", connectStr);
                    var clientSecret = this.getConnectionStringKey("ClientSecret", connectStr);
                    var redirectURL = this.getConnectionStringKey("RedirectURL", connectStr);
                    var realmId = this.getConnectionStringKey("RealmId", connectStr);
                    var authorizationCode = this.getConnectionStringKey("AuthorizationCode", connectStr);
                    var accessToken = this.getConnectionStringKey("AccessToken", connectStr);
                    var refreshToken = this.getConnectionStringKey("RefreshToken", connectStr);

                    this.quickBooksUseAppControl.setChecked(useApp);
                    this.quickBooksClientIdControl.setEnabled(useApp);
                    this.quickBooksClientSecretControl.setEnabled(useApp);
                    this.quickBooksRedirectUrlControl.setEnabled(useApp);
                    this.quickBooksGetAuthCodeControl.setEnabled(!useApp);
                    this.quickBooksClientIdControl.value = useApp && clientId ? clientId : "";
                    this.quickBooksClientSecretControl.value = useApp && clientSecret ? clientSecret : "";
                    this.quickBooksRedirectUrlControl.value = useApp && redirectURL ? redirectURL : "";
                    this.quickBooksAuthorizationCodeControl.value = authorizationCode || "";
                    this.quickBooksRealmIdControl.value = realmId || "";
                    this.quickBooksAccessTokenControl.value = accessToken || "";
                    this.quickBooksRefreshTokenControl.value = refreshToken || "";
                    this.quickBooksGetTokensControl.setEnabled(authorizationCode && realmId);
                    this.quickBooksRefreshAccessTokenControl.setEnabled(refreshToken);
                }
                if (!jsObject.options.cloudMode && !jsObject.options.standaloneJsMode) {
                    this.quickBooksUseAppControl.setChecked(true);
                    this.quickBooksUseAppControl.action();
                }
                this.quickBooksUseAppControlRow.style.display = !jsObject.options.cloudMode && !jsObject.options.standaloneJsMode ? "none" : "";
                this.updateButtonsState();
            }
            else if (isDataWorldConnection) {
                this.ownerControl.value = StiBase64.decode(this.connection.owner);
                this.databaseControl.value = StiBase64.decode(this.connection.database);
                this.tokenControl.value = StiBase64.decode(this.connection.token);

                if (jsObject.options.standaloneJsMode || jsObject.options.cloudMode) {
                    this.tokenControl.setEnabled(this.mode == "Edit");
                    this.oAuthV2Control.setChecked(this.mode != "Edit");
                }
                else {
                    this.tokenControl.setEnabled(true);
                    this.oAuthV2Control.setChecked(false);
                }
            }
            else if (isBigQueryConnection) {
                this.projectIdControl.value = StiBase64.decode(this.connection.projectId);
                this.datasetIdControl.value = StiBase64.decode(this.connection.datasetId);
                this.serviceAccountKeyControl.textArea.value = StiBase64.decode(this.connection.base64EncodedAuthSecret);
            }
            else if (isAzureBlobStorageConnection) {
                var connectStr = StiBase64.decode(this.connection.connectionString);
                var accountName = this.getConnectionStringKey("AccountName", connectStr);
                var accountKey = this.getConnectionStringKey("AccountKey", connectStr);
                var containerName = this.getConnectionStringKey("ContainerName", connectStr);
                var blobName = this.getConnectionStringKey("BlobName", connectStr);
                var blobContentType = this.getConnectionStringKey("BlobContentType", connectStr);
                var codePage = this.getConnectionStringKey("CodePage", connectStr);
                var delimiter = this.getDelimiterFromConnectionString(connectStr);
                var firstRowIsHeader = this.getConnectionStringKey("FirstRowIsHeader", connectStr);

                this.accountNameControl.value = accountName || "";
                this.accountKeyControl.value = accountKey || "";
                this.containerNameControl.setKey(containerName || "");
                this.blobNameControl.setKey(blobName || "");
                this.blobContentTypeControl.setKey(blobContentType || "");
                this.codePageCsvControl.setKey(codePage || "0");
                this.csvSeparatorControl.setKey(delimiter != null ? this.getCsvDelimeterValueName(delimiter) : "System");
                this.csvSeparatorTextControl.style.display = this.csvSeparatorControl.key == "Other" ? "" : "none";
                this.csvSeparatorTextControl.value = this.csvSeparatorControl.key == "Other" ? delimiter : "";
                this.firstRowIsHeaderControl.setChecked(firstRowIsHeader && firstRowIsHeader.toLowerCase() == "true");
            }
            else if (isGoogleAnalyticsConnection) {
                this.fillGoogleAnaliticsControls(StiBase64.decode(this.connection.connectionString));
            }
            else if (this.connection.connectionString != null) {
                var connectStr = StiBase64.decode(this.connection.connectionString);
                this.connectionStringControl.value = connectStr;
            }

            if (isODataConnection) {
                this.versionControl.setChecked(this.connection.version == "V3" || jsObject.options.jsMode);
            }

            if (this.connection.promptUserNameAndPassword != null) {
                this.promptUserNameAndPasswordControl.setChecked(this.connection.promptUserNameAndPassword);
            }
        }

        if (isFileDataConnection) {
            if (this.connection.pathData != null) this.pathDataControl.textBox.value = StiBase64.decode(this.connection.pathData);
            resourcesGallery.style.display = "";
            resourcesGallery.update();
        }
        else {
            resourcesGallery.style.display = "none";
        }

        if (isFileDataConnection) moveToResource.updateEnabledState();
        moveToResource.style.display = isFileDataConnection && (!jsObject.options.jsMode || jsObject.options.standaloneJsMode) ? "" : "none";

        this.xmlTypeControlRow.style.display = isXmlConnection && (!jsObject.options.isJava) ? "" : "none";
        this.pathDataControlRow.style.display = isFileDataConnection ? "" : "none";
        this.pathSchemaControlRow.style.display = isXmlConnection && this.xmlTypeControl.key == "AdoNetXml" ? "" : "none";
        this.connectionStringRow.style.display = !isFirebaseConnection && !isFileDataConnection && !isGoogleSheetsConnection && !isDataWorldConnection && !isQuickBooksConnection && !isBigQueryConnection && !isAzureBlobStorageConnection && !isGoogleAnalyticsConnection && !isGraphQLConnection ? "" : "none";
        this.relationDirectionControlRow.style.display = isJsonConnection || (isXmlConnection && this.xmlTypeControl.key == "Xml") ? "" : "none";
        this.buildConnection.style.display = jsObject.CanEditConnectionString(this.connection) ? "" : "none";
        this.promptUserNameAndPasswordRow.style.display = this.connectionStringRow.style.display == "" && !isMSAccessConnection && !isODataConnection && !isCosmosDbConnection ? "" : "none";
        this.versionRow.style.display = isODataConnection && !jsObject.options.jsMode ? "" : "none";
        this.codePageDbaseControlRow.style.display = isDBaseConnection ? "" : "none";
        this.codePageCsvControlRow.style.display = (isCsvConnection || (isAzureBlobStorageConnection && this.blobContentTypeControl.key == "CSV")) ? "" : "none";
        this.csvSeparatorTableControlRow.style.display = (isCsvConnection || (isAzureBlobStorageConnection && this.blobContentTypeControl.key == "CSV")) ? "" : "none";
        this.firstRowIsHeaderControlRow.style.display = isExcelConnection || isGoogleSheetsConnection || (isAzureBlobStorageConnection && this.blobContentTypeControl.key == "Excel") ? "" : "none";
        this.apiControlRow.style.display = isCosmosDbConnection ? "" : "none";
        this.accountNameControlRow.style.display = this.accountKeyControlRow.style.display = this.containerNameControlRow.style.display = this.blobNameControlRow.style.display = this.blobContentTypeControlRow.style.display = isAzureBlobStorageConnection ? "" : "none";
        this.projectIdControlRow.style.display = this.datasetIdControlRow.style.display = this.serviceAccountKeyControlRow.style.display = isBigQueryConnection ? "" : "none";
        this.clientIdControlRow.style.display = this.clientSecretControlRow.style.display = this.spreadsheetIdControlRow.style.display = isGoogleSheetsConnection ? "" : "none";
        this.ownerControlRow.style.display = this.databaseControlRow.style.display = this.tokenControlRow.style.display = this.oAuthV2Row.style.display = isDataWorldConnection ? "" : "none";
        this.oAuthV2Row.style.display = isDataWorldConnection && (jsObject.options.standaloneJsMode || jsObject.options.cloudMode) ? "" : "none";
        this.databaseSecretControlRow.style.display = this.dataUrlControlRow.style.display = isFirebaseConnection ? "" : "none";
        this.gisDataTypeControlRow.style.display = isGisDataConnection ? "" : "none";
        this.gisDataSeparatorControlRow.style.display = isGisDataConnection && this.gisDataTypeControl.key == "Wkt" ? "" : "none";
        this.graphQLEndPointControlRow.style.display = this.graphQLQueryControlRow.style.display = this.graphQLHeadersControlRow.style.display = isGraphQLConnection ? "" : "none";

        this.quickBooksClientIdControlRow.style.display = this.quickBooksClientSecretControlRow.style.display = this.quickBooksUseAppControlRow.style.display =
            this.quickBooksRedirectUrlControlRow.style.display = this.quickBooksGetAuthCodeControlRow.style.display = this.quickBooksAuthorizationCodeControlRow.style.display =
            this.quickBooksRealmIdControlRow.style.display = this.quickBooksGetTokensControlRow.style.display = this.quickBooksAccessTokenControlRow.style.display =
            this.quickBooksRefreshTokenControlRow.style.display = this.quickBooksRefreshAccessTokenControlRow.style.display = isQuickBooksConnection ? "" : "none";

        this.serviceAccountKeyFileControlRow.style.display = this.gglAnalyticAccountControlRow.style.display = this.gglAnalyticPropertyControlRow.style.display =
            this.gglAnalyticViewControlRow.style.display = this.gglAnalyticMetricsControlRow.style.display = this.gglAnalyticDimensionsControlRow.style.display =
            this.gglAnalyticStartDateControlRow.style.display = this.gglAnalyticEndDateControlRow.style.display = isGoogleAnalyticsConnection ? "" : "none";

    }

    editConnectionForm.action = function () {
        this.connection.mode = this.mode;
        this.connection.skipSchemaWizard = this.skipSchemaWizard;

        if (!this.nameControl.checkNotEmpty(jsObject.loc.PropertyMain.Name)) return;
        var allDatabases = jsObject.options.report.dictionary.databases;
        var connections = [];
        for (var i = 0; i < allDatabases.length; i++) {
            if (allDatabases[i].typeConnection) connections.push(allDatabases[i]);
        }
        if ((this.mode == "New" || this.connection.name != this.nameControl.value) &&
            !this.nameControl.checkExists(connections, "name")) return;

        if (this.mode == "Edit") this.connection["oldName"] = this.connection.name;

        this.connection.name = this.nameControl.value;
        this.connection.alias = this.aliasControl.value;

        var isXmlConnection = this.connection.typeConnection && jsObject.EndsWith(this.connection.typeConnection, "StiXmlDatabase");
        var isJsonConnection = this.connection.typeConnection && jsObject.EndsWith(this.connection.typeConnection, "StiJsonDatabase");
        var isDBaseConnection = this.connection.typeConnection && jsObject.EndsWith(this.connection.typeConnection, "StiDBaseDatabase");
        var isCsvConnection = this.connection.typeConnection && jsObject.EndsWith(this.connection.typeConnection, "StiCsvDatabase");
        var isExcelConnection = this.connection.typeConnection && jsObject.EndsWith(this.connection.typeConnection, "StiExcelDatabase");
        var isGisDataConnection = this.connection.typeConnection && jsObject.EndsWith(this.connection.typeConnection, "StiGisDatabase");
        var isGoogleSheetsConnection = this.connection.typeConnection == "StiGoogleSheetsDatabase";
        var isODataConnection = this.connection.typeConnection == "StiODataDatabase";
        var isCosmosDbConnection = this.connection.typeConnection == "StiCosmosDbDatabase";
        var isMSAccessConnection = this.connection.typeConnection == "StiMSAccessDatabase";
        var isDataWorldConnection = this.connection.typeConnection == "StiDataWorldDatabase";
        var isQuickBooksConnection = this.connection.typeConnection == "StiQuickBooksDatabase";
        var isFirebaseConnection = this.connection.typeConnection == "StiFirebaseDatabase";
        var isBigQueryConnection = this.connection.typeConnection == "StiBigQueryDatabase";
        var isAzureBlobStorageConnection = this.connection.typeConnection == "StiAzureBlobStorageDatabase";
        var isGoogleAnalyticsConnection = this.connection.typeConnection == "StiGoogleAnalyticsDatabase";
        var isGraphQLConnection = this.connection.typeConnection == "StiGraphQLDatabase";
        var isFileDataConnection = isXmlConnection || isJsonConnection || isDBaseConnection || isCsvConnection || isExcelConnection || isGisDataConnection;

        if (isXmlConnection) {
            this.connection.xmlType = this.xmlTypeControl.key;
            this.connection.pathData = StiBase64.encode(this.pathDataControl.textBox.value);
            this.connection.pathSchema = this.xmlTypeControl.key == "AdoNetXml" ? StiBase64.encode(this.pathSchemaControl.textBox.value) : "";
            this.connection.relationDirection = this.relationDirectionControl.key;
        }
        else if (isJsonConnection) {
            this.connection.pathData = StiBase64.encode(this.pathDataControl.textBox.value);
            this.connection.relationDirection = this.relationDirectionControl.key;
        }
        else if (isDBaseConnection) {
            this.connection.pathData = StiBase64.encode(this.pathDataControl.textBox.value);
            this.connection.codePage = this.codePageDbaseControl.key;
        }
        else if (isCsvConnection) {
            this.connection.pathData = StiBase64.encode(this.pathDataControl.textBox.value);
            this.connection.codePage = this.codePageCsvControl.key;
            this.connection.separator = this.getCsvSeparatorText(this.csvSeparatorControl.key);
        }
        else if (isExcelConnection) {
            this.connection.pathData = StiBase64.encode(this.pathDataControl.textBox.value);
            this.connection.firstRowIsHeader = this.firstRowIsHeaderControl.isChecked;
        }
        else if (isGisDataConnection) {
            this.connection.pathData = StiBase64.encode(this.pathDataControl.textBox.value);
            this.connection.dataType = this.gisDataTypeControl.key;
            this.connection.separator = this.gisDataSeparatorControl.value;
        }
        else {
            if (isGoogleSheetsConnection) {
                this.connection.clientId = StiBase64.encode(this.clientIdControl.value);
                this.connection.clientSecret = StiBase64.encode(this.clientSecretControl.value);
                this.connection.spreadsheetId = StiBase64.encode(this.spreadsheetIdControl.value);
                this.connection.firstRowIsHeader = this.firstRowIsHeaderControl.isChecked;
            }
            else if (isCosmosDbConnection) {
                this.connection.connectionString = StiBase64.encode(this.connectionStringControl.value + ";Api=" + this.apiControl.key);
            }
            else if (isDataWorldConnection) {
                this.connection.connectionString = StiBase64.encode("Owner=" + this.ownerControl.value.toLowerCase() + ";Database=" + this.databaseControl.value.toLowerCase() + ";Token=" + this.tokenControl.value);
            }
            else if (isFirebaseConnection) {
                this.connection.connectionString = StiBase64.encode("AuthSecret=" + this.databaseSecretControl.value + ";BasePath=" + this.dataUrlControl.value);
            }
            else if (isGraphQLConnection) {
                this.connection.connectionString = StiBase64.encode("EndPoint=" + this.graphQLEndPointControl.value + ";Query=" + this.graphQLQueryControl.value + ";Headers=" + StiBase64.encode(JSON.stringify(this.graphQLHeadersControl.getValue() || [["Key", "Value"]])));
            }
            else if (isQuickBooksConnection) {
                this.connection.connectionString = StiBase64.encode(this.getQuickBooksConnectionString());
            }
            else if (isBigQueryConnection) {
                this.connection.connectionString = StiBase64.encode("Base64EncodedAuthSecret=" + StiBase64.encode(this.serviceAccountKeyControl.textArea.value) + ";ProjectId=" + this.projectIdControl.value + ";DatasetId=" + this.datasetIdControl.value);
            }
            else if (isAzureBlobStorageConnection) {
                if (!this.accountNameControl.checkNotEmpty(jsObject.loc.FormDatabaseEdit.AccountName)) return;
                if (!this.accountKeyControl.checkNotEmpty(jsObject.loc.FormDatabaseEdit.AccountKey)) return;
                if (!this.containerNameControl.textBox.checkNotEmpty(jsObject.loc.FormDatabaseEdit.ContainerName)) return;
                if (!this.blobNameControl.textBox.checkNotEmpty(jsObject.loc.FormDatabaseEdit.BlobName)) return;
                if (!this.blobContentTypeControl.textBox.checkNotEmpty(jsObject.loc.FormDatabaseEdit.BlobContentType)) return;
                this.connection.connectionString = this.getAzureBlobStorageConnectionString();
            }
            else if (isGoogleAnalyticsConnection) {
                this.connection.connectionString = StiBase64.encode(this.getGglAnalyticsConnectionString());
            }
            else {
                this.connection.connectionString = StiBase64.encode(this.connectionStringControl.value);
            }

            if (!isMSAccessConnection) {
                this.connection.promptUserNameAndPassword = this.promptUserNameAndPasswordControl.isChecked;
            }
            if (isODataConnection) {
                this.connection.version = this.versionControl.isChecked ? "V3" : "V4";
            }
        }

        if (this.mode == "New" && !this.connection.isRecentConnection) {
            jsObject.SaveRecentConnectionToCookies(this.connection);
        }

        if (this.connection.typeConnection == "StiCustomDatabase" && this.serviceName) {
            this.connection.serviceName = this.serviceName;
        }

        this.changeVisibleState(false);

        if (isExcelConnection && jsObject.EndsWith(StiBase64.decode(this.connection.pathData).toLowerCase(), ".csv")) {
            this.connection.typeConnection = "StiCsvDatabase";
        }

        if (isDataWorldConnection && this.oAuthV2Control.isChecked) {
            jsObject.InitializeDataWorldAuthForm(function (form) {
                form.show(editConnectionForm.connection, editConnectionForm.ownerControl.value.toLowerCase(), editConnectionForm.databaseControl.value.toLowerCase());
            });
        }
        else {
            this.jsObject.SendCommandCreateOrEditConnection(this.connection);
        }
    }

    editConnectionForm.cancelAction = function () {
        if (this.loadedResources && this.loadedResources.length > 0) {
            for (var i = 0; i < this.loadedResources.length; i++) {
                jsObject.SendCommandDeleteResource({ itemObject: { name: this.loadedResources[i] } }, true);
            }
        }
    }

    return editConnectionForm;
}

StiMobileDesigner.prototype.AddQuickBooksMethodsToConnectionForm = function (form) {
    var jsObject = this;

    var clearAllCookies = function () {
        jsObject.RemoveCookie("sti_QuickBooksAuthCode");
        jsObject.RemoveCookie("sti_QuickBooksAuthError");
    };

    form.quickBooksUseAppControl.action = function () {
        form.quickBooksClientIdControl.setEnabled(this.isChecked);
        form.quickBooksClientSecretControl.setEnabled(this.isChecked);
        form.quickBooksRedirectUrlControl.setEnabled(this.isChecked);
        form.quickBooksGetAuthCodeControl.setEnabled(!this.isChecked);
    }

    form.quickBooksGetAuthCodeControl.action = function () {
        var params = {
            realmId: StiBase64.encode(form.quickBooksRealmIdControl.value)
        };
        if (form.quickBooksUseAppControl.isChecked) {
            params.clientId = StiBase64.encode(form.quickBooksClientIdControl.value);
            params.redirectUrl = StiBase64.encode(form.quickBooksRedirectUrlControl.value);
        }
        var errorForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
        var authWindow = jsObject.openNewWindow(null, null, "width=600,height=700");

        if (authWindow) {
            jsObject.SendCommandToDesignerServer("GetQuickBooksAuthorizationUrl", params, function (answer) {
                authWindow.location.href = StiBase64.decode(answer.url);

                var waitResultTimer = setInterval(function () {
                    if (!authWindow) clearInterval(waitResultTimer);

                    var authCode = jsObject.GetCookie("sti_QuickBooksAuthCode");
                    var realmId = jsObject.GetCookie("sti_QuickBooksAuthRealmId");
                    var error = jsObject.GetCookie("sti_QuickBooksAuthError");

                    if (authCode || error) {
                        clearAllCookies();
                        clearInterval(waitResultTimer);

                        if (authCode) {
                            form.quickBooksAuthorizationCodeControl.value = authCode;
                            if (realmId) form.quickBooksRealmIdControl.value = realmId;
                            form.updateButtonsState();
                        }
                        else {
                            errorForm.show(error);
                        }
                    }
                }, 500);

            });
        }
    }

    form.quickBooksGetTokensControl.action = function () {
        var params = {
            authorizationCode: StiBase64.encode(form.quickBooksAuthorizationCodeControl.value)
        };
        if (form.quickBooksUseAppControl.isChecked) {
            params.clientId = StiBase64.encode(form.quickBooksClientIdControl.value);
            params.clientSecret = StiBase64.encode(form.quickBooksClientSecretControl.value);
            params.redirectUrl = StiBase64.encode(form.quickBooksRedirectUrlControl.value);
        }
        jsObject.SendCommandToDesignerServer("GetQuickBooksTokens", params, function (answer) {
            if (answer["accessToken"] && answer["refreshToken"]) {
                form.quickBooksAccessTokenControl.value = StiBase64.decode(answer["accessToken"]);
                form.quickBooksRefreshTokenControl.value = StiBase64.decode(answer["refreshToken"]);
            }
            form.updateButtonsState();
        });
    }

    form.quickBooksRefreshAccessTokenControl.action = function () {
        var params = {
            refreshToken: StiBase64.encode(form.quickBooksRefreshTokenControl.value)
        };
        if (form.quickBooksUseAppControl.isChecked) {
            params.clientId = StiBase64.encode(form.quickBooksClientIdControl.value);
            params.clientSecret = StiBase64.encode(form.quickBooksClientSecretControl.value);
        }
        jsObject.SendCommandToDesignerServer("RefreshQuickBooksTokens", params, function (answer) {
            if (answer["accessToken"] && answer["refreshToken"]) {
                form.quickBooksAccessTokenControl.value = StiBase64.decode(answer["accessToken"]);
                form.quickBooksRefreshTokenControl.value = StiBase64.decode(answer["refreshToken"]);
            }
            form.updateButtonsState();
        });
    }

    form.updateButtonsState = function () {
        this.quickBooksGetAuthCodeControl.setEnabled(!this.quickBooksUseAppControl.isChecked || (this.quickBooksClientIdControl.value && this.quickBooksClientSecretControl.value && this.quickBooksRedirectUrlControl.value));
        this.quickBooksGetTokensControl.setEnabled(this.quickBooksAuthorizationCodeControl.value && this.quickBooksRealmIdControl.value);
        this.quickBooksRefreshAccessTokenControl.setEnabled(this.quickBooksRefreshTokenControl.value);
    }

    form.getQuickBooksConnectionString = function () {
        var connectStr = "";
        connectStr = this.setConnectionStringKey(connectStr, "UseApp", this.quickBooksUseAppControl.isChecked ? "True" : "False");
        connectStr = this.setConnectionStringKey(connectStr, "ClientId", this.quickBooksClientIdControl.value);
        connectStr = this.setConnectionStringKey(connectStr, "ClientSecret", this.quickBooksClientSecretControl.value);
        connectStr = this.setConnectionStringKey(connectStr, "RedirectURL", this.quickBooksRedirectUrlControl.value);
        connectStr = this.setConnectionStringKey(connectStr, "RealmId", this.quickBooksRealmIdControl.value);
        connectStr = this.setConnectionStringKey(connectStr, "AuthorizationCode", this.quickBooksAuthorizationCodeControl.value);
        connectStr = this.setConnectionStringKey(connectStr, "AccessToken", this.quickBooksAccessTokenControl.value);
        connectStr = this.setConnectionStringKey(connectStr, "RefreshToken", this.quickBooksRefreshTokenControl.value);
        return connectStr;
    }

    var actionControls = ["quickBooksClientId", "quickBooksClientSecret", "quickBooksRedirectUrl", "quickBooksAuthorizationCode", "quickBooksRealmId", "quickBooksRefreshToken"];
    for (var i = 0; i < actionControls.length; i++) {
        form[actionControls[i] + "Control"].onchange = function () {
            form.updateButtonsState();
        }
    }
}

StiMobileDesigner.prototype.AddAzureBlobStorageMethodsToConnectionForm = function (form) {
    var jsObject = this;

    form.getAzureBlobStorageConnectionString = function () {
        var connectionString = "AccountKey=" + this.accountKeyControl.value + ";AccountName=" + this.accountNameControl.value + ";ContainerName=" + this.containerNameControl.key + ";BlobName=" + this.blobNameControl.key + ";BlobContentType=" + this.blobContentTypeControl.key + ";";

        if (this.blobContentTypeControl.key == "CSV")
            connectionString += ("CodePage=" + this.codePageCsvControl.key + ";Delimiter=\"" + (this.csvSeparatorControl.key == "Other" ? this.csvSeparatorTextControl.value : this.getCsvDelimeterValue(this.csvSeparatorControl.key)) + "\";");

        if (this.blobContentTypeControl.key == "Excel")
            connectionString += "FirstRowIsHeader=" + (this.firstRowIsHeaderControl.isChecked ? "True" : "False") + ";";

        return StiBase64.encode(connectionString);
    }

    form.getDelimiterFromConnectionString = function (connectionString) {
        var startIndex = connectionString.indexOf("Delimiter=\"");
        if (startIndex >= 0) {
            connectionString = connectionString.substring(startIndex + "Delimiter=\"".length);
            var endIndex = connectionString.indexOf("\"");
            if (endIndex >= 0)
                return connectionString.substring(0, endIndex);
        }
        return null;
    }

    form.getCsvDelimeterValueName = function (delimeterValue) {
        switch (delimeterValue) {
            case "": return "System";
            case "\t": return "Tab";
            case ";": return "Semicolon";
            case ",": return "Comma";
            case " ": return "Space";
        }
        return "Other";
    }

    form.getCsvDelimeterValue = function (delimeterValueName) {
        switch (delimeterValueName) {
            case "System": return "";
            case "Tab": return "\t";
            case "Semicolon": return ";";
            case "Comma": return ",";
            case "Space": return " ";
        }
        return "";
    }

    form.blobContentTypeControl.action = function () {
        form.csvSeparatorTableControlRow.style.display = form.codePageCsvControlRow.style.display = this.key == "CSV" ? "" : "none";
        form.firstRowIsHeaderControlRow.style.display = this.key == "Excel" ? "" : "none";
    }

    form.containerNameControl.menu.onshow = function () {
        if (!form.accountNameControl.checkNotEmpty(jsObject.loc.FormDatabaseEdit.AccountName) || !form.accountKeyControl.checkNotEmpty(jsObject.loc.FormDatabaseEdit.AccountKey)) {
            form.containerNameControl.menu.changeVisibleState(false);
            return;
        }
        jsObject.SendCommandToDesignerServer("GetAzureBlobStorageContainerNamesItems", { connectionString: form.getAzureBlobStorageConnectionString() }, function (answer) {
            var items = [];
            if (answer.items) {
                for (var i = 0; i < answer.items.length; i++) {
                    items.push(jsObject.Item("item" + i, answer.items[i], null, answer.items[i]));
                }
                form.containerNameControl.menu.addItems(items);
            }
        });
    }

    form.blobNameControl.menu.onshow = function () {
        if (!form.accountNameControl.checkNotEmpty(jsObject.loc.FormDatabaseEdit.AccountName) ||
            !form.accountKeyControl.checkNotEmpty(jsObject.loc.FormDatabaseEdit.AccountKey) ||
            !form.containerNameControl.textBox.checkNotEmpty(jsObject.loc.FormDatabaseEdit.ContainerName)) {
            form.blobNameControl.menu.changeVisibleState(false);
            return;
        }
        jsObject.SendCommandToDesignerServer("GetAzureBlobStorageBlobNameItems", { connectionString: form.getAzureBlobStorageConnectionString() }, function (answer) {
            var items = [];
            if (answer.items) {
                for (var i = 0; i < answer.items.length; i++) {
                    items.push(jsObject.Item("item" + i, answer.items[i], null, answer.items[i]));
                }
                form.blobNameControl.menu.addItems(items);
            }
        });
    }

    form.blobNameControl.action = function () {
        jsObject.SendCommandToDesignerServer("GetAzureBlobContentTypeOrDefault", { connectionString: form.getAzureBlobStorageConnectionString() }, function (answer) {
            if (answer.blobContentType) {
                form.blobContentTypeControl.setKey(answer.blobContentType);
                form.blobContentTypeControl.action();
            }
        });
    }

    form.blobNameControl.textBox.action = function () {
        form.blobNameControl.action();
    }
}

StiMobileDesigner.prototype.AddGoogleAnalyticsMethodsToConnectionForm = function (form) {
    var jsObject = this;

    form.updateGglAnaliticsControlsVisibleStates = function () {
        form.gglAnalyticStartDatePickerControl.style.display = form.gglAnalyticStartDateControl.key == "Custom" ? "inline-block" : "none";
        form.gglAnalyticStartDateEnumeratorControl.style.display = form.gglAnalyticStartDateControl.key == "DaysAgo" ? "inline-block" : "none";
        form.gglAnalyticEndDatePickerControl.style.display = form.gglAnalyticEndDateControl.key == "Custom" ? "inline-block" : "none";
        form.gglAnalyticEndDateEnumeratorControl.style.display = form.gglAnalyticEndDateControl.key == "DaysAgo" ? "inline-block" : "none";
    }

    form.setDateControlsValues = function (dateType, dateValue) {
        form["gglAnalytic" + dateType + "DatePickerControl"].setKey(new Date());
        form["gglAnalytic" + dateType + "DateEnumeratorControl"].setValue(0);
        form["gglAnalytic" + dateType + "DateControl"].setKey(dateType == "Start" ? "Yesterday" : "Today");

        if (dateValue == "today") {
            form["gglAnalytic" + dateType + "DateControl"].setKey("Today");
        }
        else if (dateValue == "yesterday") {
            form["gglAnalytic" + dateType + "DateControl"].setKey("Yesterday");
        }
        else if (dateValue && dateValue.indexOf("daysago") >= 0) {
            form["gglAnalytic" + dateType + "DateControl"].setKey("DaysAgo");
            form["gglAnalytic" + dateType + "DateEnumeratorControl"].setValue(parseInt(dateValue.replace("daysago", "")));
        }
        else if (dateValue) {
            var dateArray = dateValue.split("-");
            if (dateArray.length >= 3) {
                form["gglAnalytic" + dateType + "DateControl"].setKey("Custom");
                form["gglAnalytic" + dateType + "DatePickerControl"].setKey(new Date(parseInt(dateArray[0]), parseInt(dateArray[1]) - 1, parseInt(dateArray[2])));
            }
        }
    }

    form.getDateControlsValue = function (dateType) {
        var dateKey = form["gglAnalytic" + dateType + "DateControl"].key;

        if (dateKey == "Today") {
            return "today";
        }
        else if (dateKey == "Yesterday") {
            return "yesterday";
        }
        else if (dateKey == "DaysAgo") {
            return (form["gglAnalytic" + dateType + "DateEnumeratorControl"].getValue() + "daysago");
        }
        else if (dateKey == "Custom") {
            var date = form["gglAnalytic" + dateType + "DatePickerControl"].key;
            var day = date.getDate();
            var month = date.getMonth() + 1;
            var year = date.getFullYear();
            return year + "-" + (month < 10 ? "0" + month : month) + "-" + (day < 10 ? "0" + day : day);
        }
    }

    form.convertToDropDownListItems = function (collection) {
        var items = [];
        if (collection) {
            for (var i = 0; i < collection.length; i++) {
                items.push(jsObject.Item("item" + i, collection[i].value, null, collection[i].key));
            }
        }

        return items;
    }

    form.getGglAnalyticsConnectionString = function () {
        var connectionString = "Base64EncodedAuthSecret=" + StiBase64.encode(form.serviceAccountKeyFileControl.getValue()) + ";";
        connectionString += "AccountId=" + form.gglAnalyticAccountControl.key + ";";
        connectionString += "PropertyId=" + form.gglAnalyticPropertyControl.key + ";";
        connectionString += "ViewId=" + form.gglAnalyticViewControl.key + ";";
        connectionString += "Metrics=" + form.gglAnalyticMetricsControl.getValue() + ";";
        connectionString += "Dimensions=" + form.gglAnalyticDimensionsControl.getValue() + ";";
        connectionString += "StartDate=" + form.getDateControlsValue("Start") + ";";
        connectionString += "EndDate=" + form.getDateControlsValue("End") + ";";

        return connectionString;
    }

    form.getGoogleAnalyticsParameters = function (connectionString, accountId, propertyId, callbackFunc) {
        var params = {
            connectionString: StiBase64.encode(connectionString),
            accountId: accountId,
            propertyId: propertyId
        }
        jsObject.SendCommandToDesignerServer("GetGoogleAnalyticsParameters", params, function (answer) {
            if (callbackFunc) callbackFunc(answer);
        });
    }

    form.fillGoogleAnaliticsControls = function (connectionString) {
        var encodedAuthSecret = this.getConnectionStringKey("Base64EncodedAuthSecret", connectionString, ";");
        var startDate = this.getConnectionStringKey("StartDate", connectionString, ";");
        var endDate = this.getConnectionStringKey("EndDate", connectionString, ";");

        form.serviceAccountKeyFileControl.setValue(encodedAuthSecret ? StiBase64.decode(encodedAuthSecret) : "");
        form.setDateControlsValues("Start", startDate);
        form.setDateControlsValues("End", endDate);
        form.updateGglAnaliticsControlsVisibleStates();
        form.gglAnalyticAccountControl.setKey("");
        form.gglAnalyticViewControl.setKey("");
        form.gglAnalyticPropertyControl.setKey("");
        form.gglAnalyticMetricsControl.setValue("");
        form.gglAnalyticDimensionsControl.setValue("");

        form.gglAnalyticUpdateDependentControls(connectionString);
    }

    form.gglAnalyticUpdateDependentControls = function (connectionString) {
        var accountId = connectionString ? this.getConnectionStringKey("AccountId", connectionString, ";") : form.gglAnalyticAccountControl.key;
        var viewId = connectionString ? this.getConnectionStringKey("ViewId", connectionString, ";") : form.gglAnalyticViewControl.key;
        var propertyId = connectionString ? this.getConnectionStringKey("PropertyId", connectionString, ";") : form.gglAnalyticPropertyControl.key;
        var metrics = connectionString ? this.getConnectionStringKey("Metrics", connectionString, ";") : form.gglAnalyticMetricsControl.getValue();
        var dimensions = connectionString ? this.getConnectionStringKey("Dimensions", connectionString, ";") : form.gglAnalyticDimensionsControl.getValue();

        if (!connectionString) connectionString = form.getGglAnalyticsConnectionString();
        var encodedAuthSecret = this.getConnectionStringKey("Base64EncodedAuthSecret", connectionString, ";");

        if (encodedAuthSecret) {
            form.getGoogleAnalyticsParameters(connectionString, accountId, propertyId, function (answer) {
                form.gglAnalyticAccountControl.addItems(form.convertToDropDownListItems(answer.parameters.accounts));
                form.gglAnalyticViewControl.addItems(form.convertToDropDownListItems(answer.parameters.views));
                form.gglAnalyticPropertyControl.addItems(form.convertToDropDownListItems(answer.parameters.properties));
                form.gglAnalyticAccountControl.setKeyIfExist(accountId);
                form.gglAnalyticViewControl.setKeyIfExist(viewId);
                form.gglAnalyticPropertyControl.setKeyIfExist(propertyId);
                form.gglAnalyticMetricsControl.buildTree(answer.parameters.metrics);
                form.gglAnalyticDimensionsControl.buildTree(answer.parameters.dimensions);
                form.gglAnalyticMetricsControl.setValue(metrics);
                form.gglAnalyticDimensionsControl.setValue(dimensions);
            });
        }
        else {
            form.gglAnalyticAccountControl.clear();
            form.gglAnalyticPropertyControl.clear();
            form.gglAnalyticViewControl.clear();
            form.gglAnalyticAccountControl.setKey("");
            form.gglAnalyticViewControl.setKey("");
            form.gglAnalyticPropertyControl.setKey("");
        }
    }

    form.gglAnalyticAccountControl.action = function () {
        form.gglAnalyticUpdateDependentControls();
    }

    form.gglAnalyticPropertyControl.action = function () {
        form.gglAnalyticUpdateDependentControls();
    }

    form.serviceAccountKeyFileControl.action = function () {
        form.gglAnalyticUpdateDependentControls();

    }
}