
StiMobileDesigner.prototype.InitializeODataConnectionForm_ = function () {
    var jsObject = this;
    var form = this.BaseForm("oDataConnectionForm", this.loc.FormDatabaseEdit.ConnectionString.replace(":", ""), 4);

    var testConnection = this.FormButton(null, null, this.loc.DesignerFx.TestConnection, null);
    testConnection.style.display = "inline-block";
    testConnection.style.margin = "12px";

    testConnection.action = function () {
        var connectionString = form.getConnectionString();
        var messageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();

        if (!connectionString) {
            messageForm.show(jsObject.loc.Notices.IsNotSpecified.replace("{0}", jsObject.loc.PropertyMain.ConnectionString), "Info");
        }
        else {
            jsObject.SendCommandToDesignerServer("TestODataConnection", { connectionString: StiBase64.encode(connectionString) },
                function (answer) {
                    messageForm.show(answer.resultTest, "Info");
                });
        }
    }

    var footerTable = this.CreateHTMLTable();
    footerTable.style.width = "100%";
    var buttonsPanel = form.buttonsPanel;
    form.removeChild(buttonsPanel);
    form.appendChild(footerTable);
    footerTable.addCell(testConnection).style.width = "1px";
    footerTable.addCell();
    footerTable.addCell(form.buttonOk).style.width = "1px";
    footerTable.addCell(form.buttonCancel).style.width = "1px";

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "6px 0 6px 0";
    form.container.appendChild(innerTable);

    var addressBearerControl = this.TextBox(null, 300);
    addressBearerControl.checkBox = this.CheckBox();
    var addressBearerTable = this.CreateHTMLTable();
    addressBearerTable.addCell(addressBearerControl.checkBox).style.padding = "0 6px 0 0";
    addressBearerTable.addCell(addressBearerControl);

    var controlProps = [
        ["address", this.loc.Report.Address, this.TextBox(null, 300)],
        ["addressBearerTable", this.loc.Report.Address + " (Bearer)", addressBearerTable],
        ["token", this.loc.FormDatabaseEdit.Token, this.TextBox(null, 300)],
        ["userName", this.loc.Report.LabelUserName.replace(":", ""), this.TextBox(null, 200)],
        ["password", this.loc.Cloud.labelPassword.replace(":", ""), this.TextBox(null, 200)],
        ["clientId", this.loc.FormDatabaseEdit.ClientId, this.TextBox(null, 200)],
        ["useToken", "", this.CheckBox(null, this.loc.FormDatabaseEdit.UseToken)]
    ]

    //PathToData
    for (var i = 0; i < controlProps.length; i++) {
        form[controlProps[i][0] + "ControlRow"] = innerTable.addRow();
        var labelCell = innerTable.addTextCellInLastRow(controlProps[i][1]);
        labelCell.className = "stiDesignerCaptionControlsBigIntervals";
        var control = controlProps[i][2];
        form[controlProps[i][0] + "Control"] = control;
        innerTable.addCellInLastRow(control).className = "stiDesignerControlCellsBigIntervals2";
        control.style.marginLeft = controlProps[i][0] == "addressBearerTable" ? "0" : "25px";
    }

    form.useTokenControl.style.margin = "3px 0 3px 25px";
    form.passwordControl.setAttribute("type", "password");

    form.useTokenControl.action = function () {
        form.updateControlsVisibleStates();
    }

    addressBearerControl.checkBox.action = function () {
        addressBearerControl.setEnabled(this.isChecked);
    }

    form.updateControlsVisibleStates = function () {
        form.tokenControlRow.style.display = form.useTokenControl.isChecked ? "" : "none";
        form.addressBearerTableControlRow.style.display = form.userNameControlRow.style.display = form.passwordControlRow.style.display =
            form.clientIdControlRow.style.display = !form.useTokenControl.isChecked ? "" : "none";
        addressBearerControl.setEnabled(addressBearerControl.checkBox.isChecked);
    }

    form.getConnectionString = function () {
        var useToken = form.useTokenControl.isChecked;
        var connectStr = this.addressControl.value;
        if (addressBearerControl.isEnabled && addressBearerControl.value && !useToken) { connectStr += ";AddressBearer=" + addressBearerControl.value; }
        if (this.userNameControl.value && !useToken) { connectStr += ";UserName=" + this.userNameControl.value; }
        if (this.passwordControl.value && !useToken) { connectStr += ";Password=" + this.passwordControl.value; }
        if (this.clientIdControl.value && !useToken) { connectStr += ";Client_Id=" + this.clientIdControl.value; }
        if (this.tokenControl.value && useToken) { connectStr += ";Token=" + this.tokenControl.value; }

        return connectStr;
    }

    form.getConnectionStringKey = function (key, connectionString) {
        if (!connectionString) return null;
        var strs = connectionString.split(/;|,/);
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
            value = value.substr(0, value.length - 2);

        return value;
    }

    form.show = function (connection) {
        var addressBearer = form.getConnectionStringKey("AddressBearer", connection);
        var userName = form.getConnectionStringKey("UserName", connection);
        var password = form.getConnectionStringKey("Password", connection);
        var clientId = form.getConnectionStringKey("Client_Id", connection);
        var token = form.getConnectionStringKey("Token", connection);

        var adressArray = connection != null ? connection.split(";") : "";
        this.addressControl.value = adressArray.length > 0 ? adressArray[0] : "";

        addressBearerControl.checkBox.setChecked(addressBearer != null);
        addressBearerControl.value = addressBearer || "";
        this.useTokenControl.setChecked(token);
        this.userNameControl.value = userName || "";
        this.passwordControl.value = password || "";
        this.clientIdControl.value = clientId || "";
        if (token) this.tokenControl.value = token;
        this.changeVisibleState(true);
        this.addressControl.focus();
        this.updateControlsVisibleStates();
    }

    form.action = function () {
        this.changeVisibleState(false);
    }

    return form;
}