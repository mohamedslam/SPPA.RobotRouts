
StiJsViewer.prototype.CreateSvgElement = function (tagName) {
    return ("createElementNS" in document ? document.createElementNS("http://www.w3.org/2000/svg", tagName) : document.createElement(tagName));
}

StiJsViewer.prototype.ColumnIsNumericType = function (type) {
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

StiJsViewer.prototype.ColumnIsDateType = function (type) {
    return (
        type == "datetime" ||
        type == "timespan"
    );
}

StiJsViewer.prototype.DataFilterObject = function (key, path, condition, value, value2, isEnabled, isExpression) {
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

StiJsViewer.prototype.DataSortObject = function (key, direction) {
    return {
        typeItem: "SortRule",
        key: key,
        direction: direction
    }
}

StiJsViewer.prototype.RemoveElementFromArray = function (array, element) {
    for (var i = 0; i < array.length; i++) {
        if (element == array[i]) {
            array.splice(array.indexOf(element), 1);
        }
    }
}