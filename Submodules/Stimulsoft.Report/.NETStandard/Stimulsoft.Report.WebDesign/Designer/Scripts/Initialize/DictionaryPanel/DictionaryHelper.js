//Helper Methods
StiMobileDesigner.prototype.GetObjectByPropertyValueFromCollection = function (collection, propertyName, propertyValue) {
    if (collection && collection.length) {
        for (var i = 0; i < collection.length; i++)
            if (collection[i][propertyName] && propertyValue != null && (collection[i][propertyName] == propertyValue || collection[i][propertyName].replace(/ /g, '_') == propertyValue.replace(/ /g, '_')))
                return collection[i];
    }

    return null;
}

StiMobileDesigner.prototype.GetCollectionByCollectionName = function (collection, collectionName) {
    var resultCollection = [];
    if (collection && collection.length) {
        for (var index = 0; index < collection.length; index++) {
            var innerCollection = collection[index][collectionName];
            if (innerCollection && innerCollection.length) {
                for (var index2 = 0; index2 < innerCollection.length; index2++)
                    resultCollection.push(innerCollection[index2]);
            }
        }
    }

    return resultCollection;
}

StiMobileDesigner.prototype.GetCollectionFromCategoriesCollection = function (categoriesCollection) {
    var resultCollection = [];
    for (var i = 0; i < categoriesCollection.length; i++) {
        if (categoriesCollection[i].typeItem == "Category" && categoriesCollection[i].categoryItems) {
            var categoryItems = categoriesCollection[i].categoryItems;
            for (var k = 0; k < categoryItems.length; k++) {
                resultCollection.push(categoryItems[k]);
            }
        }
        else
            resultCollection.push(categoriesCollection[i]);
    }

    return resultCollection;
}

//Get DataSources Collection
StiMobileDesigner.prototype.GetDataSourcesFromDictionary = function (dictionary) {
    return this.GetCollectionByCollectionName(dictionary.databases, "dataSources");
}

//Get Relations Collection
StiMobileDesigner.prototype.GetRelationsFromDictionary = function (dictionary) {
    return this.GetCollectionByCollectionName(this.GetDataSourcesFromDictionary(dictionary), "relations");
}

//Get Variables Collection
StiMobileDesigner.prototype.GetVariablesFromDictionary = function (dictionary) {
    return this.GetCollectionFromCategoriesCollection(dictionary.variables);
}

//Get BusinessObjects Collection
StiMobileDesigner.prototype.GetBusinessObjectsFromDictionary = function (dictionary) {
    var result = [];

    var addBusinessObjectsToArray = function (array, businessObjects) {
        for (var i = 0; i < businessObjects.length; i++) {
            array.push(businessObjects[i]);
            if (businessObjects[i].businessObjects) {
                addBusinessObjectsToArray(array, businessObjects[i].businessObjects);
            }
        }
    }

    var parentBusinessObjects = this.GetCollectionFromCategoriesCollection(dictionary.businessObjects);
    for (var i = 0; i < parentBusinessObjects.length; i++) {
        result.push(parentBusinessObjects[i]);
        if (parentBusinessObjects[i].businessObjects) {
            addBusinessObjectsToArray(result, parentBusinessObjects[i].businessObjects);
        }
    }

    return result;
}

//Get DataSources&BusinessObjects Collection
StiMobileDesigner.prototype.GetDataSourcesAndBusinessObjectsFromDictionary = function (dictionary) {
    var collection = this.GetBusinessObjectsFromDictionary(dictionary);
    return collection.concat(this.GetDataSourcesFromDictionary(dictionary));
}

//Get Variable Categories Collection
StiMobileDesigner.prototype.GetVariableCategoriesFromDictionary = function (dictionary) {
    var categoriesCollection = [];
    for (var i = 0; i < dictionary.variables.length; i++)
        if (dictionary.variables[i].typeItem == "Category")
            categoriesCollection.push(dictionary.variables[i]);

    return categoriesCollection;
}

//DataSource By Name
StiMobileDesigner.prototype.GetDataSourceByNameFromDictionary = function (name) {
    var dataSources = this.GetDataSourcesFromDictionary(this.options.report.dictionary);

    return this.GetObjectByPropertyValueFromCollection(dataSources, "name", name);
}

//DataSource By Name And NameInSource
StiMobileDesigner.prototype.GetDataSourceByNameAndNameInSourceFromDictionary = function (name, nameInSource) {
    var dataSources = this.GetDataSourcesFromDictionary(this.options.report.dictionary);

    if (dataSources && dataSources.length) {
        for (var i = 0; i < dataSources.length; i++) {
            if (dataSources[i].name == name && dataSources[i].nameInSource == nameInSource)
                return dataSources[i];
        }
    }

    return null;
}

//BusinessObject By Name
StiMobileDesigner.prototype.GetBusinessObjectByNameFromDictionary = function (name) {
    if (!name) return null;
    var businessObjects = this.GetBusinessObjectsFromDictionary(this.options.report.dictionary);

    if (typeof (name) == "string") {
        //String
        if (name.indexOf(".") >= 0) name = name.split(".").reverse();
    }
    else {
        //Array
        if (name.length == 1) name = name[0];
    }

    if (typeof (name) == "string") {
        return this.GetObjectByPropertyValueFromCollection(businessObjects, "name", name);
    }
    else {
        for (var i = name.length - 1; i >= 0; i--) {
            var businessObject = this.GetObjectByPropertyValueFromCollection(businessObjects, "name", name[i]);
            if (businessObject) businessObjects = businessObject.businessObjects;
        }
        return businessObject;
    }
}

//Relation By Name
StiMobileDesigner.prototype.GetRelationByNameFromObject = function (object, name) {
    if (!object) return null;
    return this.GetObjectByPropertyValueFromCollection(object.relations, "name", name);
}

//Relation By NameInSource
StiMobileDesigner.prototype.GetRelationByNameInSourceFromObject = function (object, nameInSource) {
    if (!object) return null;
    return this.GetObjectByPropertyValueFromCollection(object.relations, "nameInSource", nameInSource);
}

//Columns By Name
StiMobileDesigner.prototype.GetColumnByNameFromDataSource = function (dataSource, name) {
    if (!dataSource) return null;
    return this.GetObjectByPropertyValueFromCollection(dataSource.columns, "name", name);
}

//Variable By Name
StiMobileDesigner.prototype.GetVariableByNameFromDictionary = function (name) {
    var allVariables = this.GetCollectionFromCategoriesCollection(this.options.report.dictionary.variables);
    return this.GetObjectByPropertyValueFromCollection(allVariables, "name", name);
}

//Get Items For Menu
StiMobileDesigner.prototype.GetDataSourceItemsFromDictionary = function () {
    var dataSources = this.GetDataSourcesFromDictionary(this.options.report.dictionary);
    var items = [];
    if (dataSources && dataSources.length) {
        for (var i = 0; i < dataSources.length; i++) {
            items.push(this.Item("dataSource" + i, dataSources[i].name, null, dataSources[i].name));
        }
    }

    return items;
}


StiMobileDesigner.prototype.GetAllRelationsFromDataSource = function (dataSource, relationFullName, relations, relationsArray) {
    if (dataSource) {
        if (relations == null) relations = dataSource.relations;
        for (var i = 0; i < relations.length; i++) {
            var currentRelationFullName = relationFullName + (relationFullName != "" ? "." : "") + relations[i].name;
            relationsArray.push(currentRelationFullName);
            var currentRelation = relations[i];
            this.GetAllRelationsFromDataSource(dataSource, currentRelationFullName, currentRelation.relations, relationsArray);
        }
    }
}

StiMobileDesigner.prototype.GetRelationsItemsFromDictionary = function (object) {
    if (!object) return null;
    var items = [];

    if (object.relations && object.relations.length) {
        for (var i = 0; i < object.relations.length; i++) {
            items.push(this.Item("relation" + i, object.relations[i].name, null,
                this.options.selectedObject.typeComponent != "StiImage" ? object.relations[i].name : object.relations[i].nameInSource));
        }
    }

    return items.length == 0 ? null : items;
}

StiMobileDesigner.prototype.GetColumnsItemsFromDictionary = function (dataSource) {
    if (!dataSource) return null;
    var items = [];
    if (dataSource.columns && dataSource.columns.length) {
        for (var i = 0; i < dataSource.columns.length; i++) {
            items.push(this.Item("columns" + i, dataSource.columns[i].name, null, dataSource.columns[i].name, undefined, undefined, dataSource.columns[i].type));
        }
    }

    return items;
}

StiMobileDesigner.prototype.GetNewName = function (type, collection, name) {

    var defaultName = name || (type == "DataBase" ? this.loc.Database.Connection : this.loc.PropertyMain[type]);
    if (defaultName) defaultName = defaultName.replace(/ /g, '');

    if (!collection && this.options.report) {
        switch (type) {
            case "DataBase": { collection = this.options.report.dictionary.databases; break; }
            case "DataSource": { collection = this.GetDataSourcesFromDictionary(this.options.report.dictionary); break; }
            case "BusinessObject": { collection = this.GetBusinessObjectsFromDictionary(this.options.report.dictionary); break; }
            case "Relation": { collection = this.GetRelationsFromDictionary(this.options.report.dictionary); break; }
            case "Variable": { collection = this.GetVariablesFromDictionary(this.options.report.dictionary); break; }
            case "Category": { collection = this.GetVariableCategoriesFromDictionary(this.options.report.dictionary); break; }
            case "Resource": { collection = this.options.report.dictionary.resources; break; }
            case "DataTransformation": { collection = this.options.report.dictionary.dataTransformations; break; }
        }
    }

    var index = 0;
    var flag = false;
    if (collection) {
        while (!flag) {
            index++;
            flag = true;
            for (var i = 0; i < collection.length; i++) {
                if (collection[i].name && typeof (collection[i].name) == "string" &&
                    collection[i].name.toLowerCase() == defaultName.toLowerCase() + (index == 1 ? "" : index)) {
                    flag = false;
                    break;
                }
            }
        }
    }

    return defaultName + (index < 2 ? "" : index);
}

StiMobileDesigner.prototype.GetTypeIcon = function (type) {
    if (type == "bool" || type == "bool (Nullable)") return "DataColumnBool";
    if (type == "char" || type == "char (Nullable)") return "DataColumnChar";
    if (type == "datetime" || type == "timespan" || type == "datetime (Nullable)" || type == "timespan (Nullable)") return "DataColumnDateTime";
    if (type == "decimal" || type == "decimal (Nullable)") return "DataColumnDecimal";
    if (type == "int" || type == "uint" || type == "long" || type == "ulong" || type == "byte" || type == "sbyte" || type == "short" || type == "ushort" ||
        type == "int (Nullable)" || type == "uint (Nullable)" || type == "long (Nullable)" || type == "ulong (Nullable)" || type == "byte (Nullable)" ||
        type == "sbyte (Nullable)" || type == "short (Nullable)" || type == "ushort (Nullable)")
        return "DataColumnInt";
    if (type == "float" || type == "double" || type == "float (Nullable)" || type == "double (Nullable)") return "DataColumnFloat";
    if (type == "image") return "DataColumnImage";
    if (type == "object") return "DataColumnBinary";

    return "DataColumnString";
}

StiMobileDesigner.prototype.CanEditConnectionString = function (connection) {
    return connection.typeConnection == "StiODataDatabase";
}

StiMobileDesigner.prototype.UpdateResourcesFonts = function () {
    if (!this.options.resourcesFonts) this.options.resourcesFonts = {};
    var resourcesFonts = this.options.resourcesFonts;

    for (var fontName in resourcesFonts) {
        resourcesFonts[fontName].prepareToRemove = true;
    }

    //add new styles
    if (this.options.report) {
        var resources = this.options.report.dictionary.resources;
        for (var i = 0; i < resources.length; i++) {
            if (resources[i].contentForCss && resources[i].originalFontFamily) {
                if (resourcesFonts[resources[i].name]) {
                    resourcesFonts[resources[i].name].prepareToRemove = false;
                }
                else {
                    resourcesFonts[resources[i].name] = {
                        styleElement: this.AddCustomFontsCss(this.GetCustomFontsCssText(resources[i].contentForCss, resources[i].originalFontFamily)),
                        originalFontFamily: resources[i].originalFontFamily
                    }
                }
            }
        }
    }

    //remove old styles
    for (var fontName in resourcesFonts) {
        if (resourcesFonts[fontName].prepareToRemove) {
            if (resourcesFonts[fontName].styleElement) {
                try {
                    if (resourcesFonts[fontName].styleElement.parentNode) {
                        resourcesFonts[fontName].styleElement.parentNode.removeChild(resourcesFonts[fontName].styleElement);
                    }
                    delete resourcesFonts[fontName];
                }
                catch (e) {
                    console.log(e);
                }
            }
        }
    }
}

StiMobileDesigner.prototype.ColumnIsNumericType = function (type) {
    return (
        type == "sbyte" ||
        type == "byte" ||
        type == "short" ||
        type == "ushort" ||
        type == "int" ||
        type == "uint" ||
        type == "long" ||
        type == "ulong" ||
        type == "float" ||
        type == "decimal" ||
        type == "double"
    );
}

StiMobileDesigner.prototype.ColumnIsDateType = function (type) {
    return (
        type == "datetime" ||
        type == "timespan"
    );
}

StiMobileDesigner.prototype.GetCustomMapResources = function () {
    var customResources = [];

    if (this.options.report) {
        var resources = this.options.report.dictionary.resources;
        for (var i = 0; i < resources.length; i++) {
            if (resources[i].type == "Map") {
                var icon = resources[i].mapIcon ? "data:image/png;base64," + resources[i].mapIcon : null;
                customResources.push({
                    name: resources[i].name,
                    icon: icon
                });
            }
        }
    }
    return customResources;
}

StiMobileDesigner.prototype.ReplaceRelationsToShortNames = function (dataColumnPath) {
    if (this.options.report) {
        var relations = this.GetRelationsFromDictionary(this.options.report.dictionary);
        var pathArray = dataColumnPath.split(".");
        if (pathArray.length > 2) {
            for (var i = 1; i < pathArray.length - 1; i++) {
                var relationFullName = pathArray[i];
                for (var k = 0; k < relations.length; k++) {
                    if (relationFullName == relations[k].nameInSource) {
                        pathArray[i] = relations[k].name;
                        break;
                    }
                }
            }
            dataColumnPath = pathArray.join(".");
        }
    }
    return dataColumnPath;
}
