
StiMobileDesigner.prototype.FontList = function (name, width, height, cutMenu, toolTip) {
    var fontList = this.DropDownList(name, width, toolTip ? toolTip : this.loc.HelpDesigner.FontName, this.GetFontNamesItems(), true, false, height, cutMenu);
    fontList.isFontList = true;

    var jsObject = this;
    var innerContent = fontList.menu.innerContent;
    var sysFHeader = this.StandartMenuHeader(this.loc.PropertyMain.SystemFonts);
    var customFContainer = document.createElement("div");

    if (this.options.showSystemFonts === false) {
        while (innerContent.childNodes[0])
            innerContent.removeChild(innerContent.childNodes[0]);
    }
    else {
        if (innerContent.firstChild)
            innerContent.insertBefore(sysFHeader, innerContent.firstChild);
        else
            innerContent.appendChild(sysFHeader);
    }

    if (innerContent.firstChild)
        innerContent.insertBefore(customFContainer, innerContent.firstChild);
    else
        innerContent.appendChild(customFContainer);

    fontList.checkEmpty = function () {
        if (jsObject.options.showSystemFonts === false) {
            if (fontList.emptyText && fontList.emptyText.parentElement) {
                fontList.emptyText.parentElement.removeChild(fontList.emptyText);
                fontList.emptyText = null;
            }
            if (jsObject.GetCountObjects(jsObject.options.resourcesFonts) == 0 &&
                (!jsObject.options.opentypeFonts || jsObject.options.opentypeFonts.length == 0) &&
                innerContent.childNodes.length == 1 && innerContent.childNodes[0] == customFContainer) {
                var emptyText = document.createElement("div");
                emptyText.className = "stiCreateDataHintText";
                emptyText.style.padding = "50px";
                emptyText.innerHTML = jsObject.loc.Report.StiEmptyBrush;
                innerContent.appendChild(emptyText);
                fontList.emptyText = emptyText;
            }
        }
    }

    fontList.menu.onshow = function () {
        while (customFContainer.childNodes[0])
            customFContainer.removeChild(customFContainer.childNodes[0]);

        //Add custom fonts
        var resourcesFonts = jsObject.options.resourcesFonts;
        if (resourcesFonts && jsObject.GetCountObjects(resourcesFonts) > 0) {
            if (jsObject.options.showSystemFonts !== false) {
                customFContainer.appendChild(jsObject.StandartMenuHeader(jsObject.loc.PropertyMain.CustomFonts));
            }

            for (var resourceName in resourcesFonts) {
                var menuItem = jsObject.VerticalMenuItem(fontList.menu, "fontItemCustom" + resourceName, resourcesFonts[resourceName].originalFontFamily, null, resourcesFonts[resourceName].originalFontFamily);
                customFContainer.appendChild(menuItem);
            }
        }

        var opentypeFonts = jsObject.options.opentypeFonts;
        if (opentypeFonts) {
            for (var i = 0; i < opentypeFonts.length; i++) {
                var item = jsObject.VerticalMenuItem(fontList.menu, "fontItemOpentypeFont" + opentypeFonts[i], opentypeFonts[i], null, opentypeFonts[i]);
                customFContainer.appendChild(item);
            }
        }

        fontList.checkEmpty();
    }

    if (this.options.jsMode) {
        fontList.menu.addItems = function (items) {
            while (this.innerContent.childNodes[0]) {
                this.innerContent.removeChild(this.innerContent.childNodes[0]);
            }

            if (jsObject.options.showSystemFonts !== false) {
                innerContent.appendChild(sysFHeader);
            }

            if (innerContent.firstChild)
                innerContent.insertBefore(customFContainer, innerContent.firstChild);
            else
                innerContent.appendChild(customFContainer);

            if (items && items.length) {
                for (var i = 0; i < items.length; i++) {
                    var item = jsObject.VerticalMenuItem(this, items[i].name, items[i].caption, items[i].imageName, items[i].key, this.itemsStyle, items[i].haveSubMenu);
                    this.innerContent.appendChild(item);
                }
            }

            fontList.checkEmpty();
        }

        fontList.addItems = function (items) {
            fontList.items = items;
            fontList.menu.addItems(items);
        }
    }

    //Override
    fontList.setKey = function (key) {
        this.key = key;
        if (key == null) return;
        if (key == "StiEmptyValue") {
            this.textBox.value = "";
            return;
        }
        if (this.menu && this.menu.items) {
            for (var itemName in this.menu.items)
                if (key == this.menu.items[itemName].key) {
                    this.textBox.value = this.menu.items[itemName].captionText;
                    if (this.image) StiMobileDesigner.setImageSource(this.image, jsObject.options, this.menu.items[itemName].imageName);
                    return;
                }
        }
        this.textBox.value = key.toString();
    }

    return fontList;
}