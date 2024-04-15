
StiMobileDesigner.prototype.ConnectionObject = function (typeConnection) {
    var isXmlConnection = typeConnection && this.EndsWith(typeConnection, "StiXmlDatabase");
    var isJsonConnection = typeConnection && this.EndsWith(typeConnection, "StiJsonDatabase");
    var isDBaseConnection = typeConnection && this.EndsWith(typeConnection, "StiDBaseDatabase");
    var isCsvConnection = typeConnection && this.EndsWith(typeConnection, "StiCsvDatabase");
    var isExcelConnection = typeConnection && this.EndsWith(typeConnection, "StiExcelDatabase");
    var isGisDataConnection = typeConnection && this.EndsWith(typeConnection, "StiGisDatabase");
    var isFileDataConnection = isXmlConnection || isJsonConnection || isDBaseConnection || isCsvConnection || isExcelConnection || isGisDataConnection;

    var connectionName = null;
    if (isXmlConnection) connectionName = "XML";
    else if (isJsonConnection) connectionName = "JSON";
    else if (isDBaseConnection) connectionName = "DBASE";
    else if (isCsvConnection) connectionName = "CSV";
    else if (isExcelConnection) connectionName = "EXCEL";

    var name = this.GetNewName("DataBase", null, connectionName);

    var connectionObject = {
        "typeItem": "DataBase",
        "typeConnection": typeConnection,
        "name": name,
        "alias": name
    }

    if (isFileDataConnection) {
        connectionObject.pathData = "";
    }

    if (isXmlConnection) {
        connectionObject.pathSchema = "";
        connectionObject.xmlType = "AdoNetXml";
        connectionObject.relationDirection = "ParentToChild";
    }
    else if (isJsonConnection) {
        connectionObject.relationDirection = "ParentToChild";
    }
    else if (isExcelConnection) {
        connectionObject.firstRowIsHeader = true;
    }
    else if (isDBaseConnection) {
        connectionObject.codePage = "0";
    }
    else if (isCsvConnection) {
        connectionObject.codePage = "0";
        connectionObject.separator = "";
    }
    else if (!isFileDataConnection) {
        connectionObject.connectionString = "";
        connectionObject.promptUserNameAndPassword = false;
    }
    else if (isGisDataConnection) {
        connectionObject.dataType = "Wkt";
        connectionObject.separator = "|";
    }

    return connectionObject;
}

StiMobileDesigner.prototype.CategoryObject = function (name, requestFromUser, readOnly) {
    var categoryObject = {
        "typeItem": "Category",
        "typeIcon": "Folder",
        "name": name != null ? name : this.GetNewName("Category")
    }
    if (requestFromUser != null) categoryObject.requestFromUser = requestFromUser;
    if (readOnly != null) categoryObject.readOnly = readOnly;

    return categoryObject;
}

StiMobileDesigner.prototype.ColumnObject = function (isCalcColumn, columnCollection) {
    var name = this.GetNewName("Column", columnCollection);

    var columnObject = {
        "typeItem": "Column",
        "typeIcon": isCalcColumn ? "CalcColumnString" : "DataColumnString",
        "name": name,
        "alias": name,
        "nameInSource": name,
        "type": "string",
        "isCalcColumn": isCalcColumn,
        "expression": ""
    }

    return columnObject;
}

StiMobileDesigner.prototype.DataSourceObject = function (typeDataAdapter, nameInSource) {
    var name = this.GetNewName("DataSource");

    var dataSourceObject = {
        "typeItem": "DataSource",
        "nameInSource": nameInSource || "",
        "name": name,
        "alias": name,
        "typeDataSource": this.GetDataSourceTypeFromDataAdapterType(typeDataAdapter),
        "typeDataAdapter": typeDataAdapter,
        "columns": [],
        "parameters": []
    }

    if (dataSourceObject.typeDataSource &&
        dataSourceObject.typeDataSource != "StiBusinessObjectSource" &&
        dataSourceObject.typeDataSource != "StiDataTableSource" &&
        dataSourceObject.typeDataSource != "StiCsvSource" &&
        dataSourceObject.typeDataSource != "StiDBaseSource" &&
        dataSourceObject.typeDataSource != "StiVirtualSource" &&
        dataSourceObject.typeDataSource != "StiCrossTabDataSource") {
        dataSourceObject["sqlCommand"] = "";
        dataSourceObject["type"] = "Table";
        dataSourceObject["commandTimeout"] = "30";
        dataSourceObject["reconnectOnEachRow"] = false;
    }

    if (dataSourceObject.typeDataSource == "StiVirtualSource") {
        dataSourceObject["sortData"] = "";
        dataSourceObject["filterData"] = StiBase64.encode("[]");
        dataSourceObject["filterMode"] = "And";
        dataSourceObject["filterOn"] = true;
        dataSourceObject["groupsData"] = "";
        dataSourceObject["resultsData"] = "";
    }

    return dataSourceObject;
}

StiMobileDesigner.prototype.DataTransformationObject = function () {
    var name = this.GetNewName("DataSource", null, "Data");

    var dataTransformationObject = {
        "typeItem": "DataSource",
        "nameInSource": this.loc.PropertyMain.Category,
        "name": name,
        "alias": name,
        "typeDataSource": "StiDataTransformation",
        "columns": [],
        "sortRules": [],
        "filterRules": [],
        "actionRules": []
    }

    return dataTransformationObject;
}

StiMobileDesigner.prototype.BusinessObject = function () {
    var name = this.GetNewName("BusinessObject");

    var businessObject = {
        "typeItem": "BusinessObject",
        "category": "",
        "name": name,
        "alias": name,
        "columns": []
    }

    return businessObject;
}

StiMobileDesigner.prototype.ParameterObject = function (parameterCollection) {
    var name = this.GetNewName("Parameter", parameterCollection);

    var parameterObject = {
        "typeItem": "Parameter",
        "typeIcon": "Parameter",
        "name": name,
        "type": "0",
        "size": "0",
        "expression": ""
    }

    return parameterObject;
}

StiMobileDesigner.prototype.RelationObject = function () {
    var name = this.GetNewName("Relation");
    var relationObject = {
        "typeItem": "Relation",
        "name": name,
        "alias": name,
        "active": false,
        "nameInSource": name,
        "parentDataSource": "",
        "childDataSource": "",
        "childColumns": [],
        "parentColumns": []
    }

    return relationObject;
}

StiMobileDesigner.prototype.VariableObject = function () {
    var name = this.GetNewName("Variable");
    var variableObject = {
        "typeItem": "Variable",
        "name": name,
        "alias": name,
        "description": "",
        "basicType": "Value",
        "type": "string",
        "value": "",
        "category": "",
        "initBy": "Value",
        "readOnly": false,
        "requestFromUser": false,
        "allowUseAsSqlParameter": false,
        "allowUserValues": true,
        "dateTimeFormat": "DateAndTime",
        "dataSource": "Items",
        "sortDirection": "None",
        "sortField": "Label",
        "selection": "FromVariable",
        "formatMask": "",
        "items": null,
        "keys": "",
        "values": "",
        "dependentValue": false,
        "dependentVariable": "",
        "dependentColumn": ""
    }

    return variableObject;
}

StiMobileDesigner.prototype.ResourceObject = function () {
    var name = this.GetNewName("Resource");

    var resourceObject = {
        "typeItem": "Resource",
        "typeIcon": "Resources.Resource",
        "type": "Image",
        "name": name,
        "alias": name,
        "content": null
    }

    return resourceObject;
}

StiMobileDesigner.prototype.StyleConditionObject = function () {
    var styleCondition = {
        "type": "Placement",
        "placement": "",
        "operationPlacement": "EqualTo",
        "placementNestedLevel": "1",
        "operationPlacementNestedLevel": "EqualTo",
        "componentType": " Text,",
        "operationComponentType": "EqualTo",
        "location": "",
        "operationLocation": "EqualTo",
        "componentName": "",
        "operationComponentName": "EqualTo"
    }

    return styleCondition;
}

StiMobileDesigner.prototype.DataFilterRuleObject = function (key, path, condition, value, value2, isEnabled, isExpression) {
    return {
        typeItem: "FilterRule",
        key: key,
        path: path,
        condition: typeof (condition) != "undefined" ? condition : "EqualTo",
        value: typeof (value) != "undefined" ? value : null,
        value2: typeof (value2) != "undefined" ? value2 : null,
        isEnabled: typeof (isEnabled) != "undefined" ? isEnabled : true,
        isExpression: typeof (isExpression) != "undefined" ? isExpression : false
    }
}

StiMobileDesigner.prototype.DataSortRuleObject = function (key, direction) {
    return {
        typeItem: "SortRule",
        key: key,
        direction: direction
    }
}

StiMobileDesigner.prototype.DataActionRuleObject = function (key, path, type, startIndex, rowsCount, initialValue, valueFrom, valueTo, matchCase, matchWholeWord, priority) {
    return {
        typeItem: "ActionRule",
        key: key,
        path: path,
        type: typeof (type) != "undefined" ? type : "Limit",
        startIndex: startIndex != null ? startIndex : 0,
        rowsCount: rowsCount != null ? rowsCount : -1,
        initialValue: typeof (initialValue) != "undefined" ? initialValue : null,
        valueFrom: typeof (valueFrom) != "undefined" ? valueFrom : null,
        valueTo: typeof (valueTo) != "undefined" ? valueTo : null,
        matchCase: matchCase != null ? matchCase : false,
        matchWholeWord: matchWholeWord != null ? matchWholeWord : false,
        priority: priority != null ? priority : "AfterGroupingData"
    }
}