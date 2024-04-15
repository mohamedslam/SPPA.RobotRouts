
StiMobileDesigner.prototype.RichTextEditor = function (name, width, height, hintText, conpactSize) {
    var jsObject = this;
    var richTextControl = document.createElement("div");
    richTextControl.controls = {};
    if (name != null) this.options.controls[name] = richTextControl;
    richTextControl.jsObject = this;
    richTextControl.name = name != null ? name : this.generateKey();
    richTextControl.id = richTextControl.name;
    richTextControl.isEnabled = true;
    richTextControl.isSelected = false;
    richTextControl.isOver = false;
    richTextControl.isFocused = false;
    richTextControl.width = width;
    richTextControl.height = height;
    richTextControl.style.position = "relative";
    var defaultFontName = "Arial";
    var defaultFontSize = 3;
    var padding = conpactSize ? "12px 1px 0 1px" : "12px 3px 0 3px";

    var removeButton = this.SmallButton(null, null, null, "Remove.png", this.loc.MainMenu.menuEditDelete.replace("&", ""), null, "stiDesignerFormButton");
    removeButton.style.position = "absolute";
    richTextControl.appendChild(removeButton);
    removeButton.style.top = "55px";
    removeButton.style.right = "30px";
    removeButton.style.display = "none";

    removeButton.action = function () {
        richTextControl.clearResourceContent();
    }

    /* Tool Bar 1 */
    var toolBar1 = richTextControl.toolBar1 = this.CreateHTMLTable();
    richTextControl.appendChild(toolBar1);

    var fontList = this.FontList(name + "FontList", conpactSize ? 130 : 170);
    fontList.setKey(defaultFontName);
    toolBar1.addCell(fontList).style.padding = conpactSize ? "12px 1px 0 12px" : "12px 3px 0 12px";

    fontList.action = function () {
        var key = this.key;
        richText.restoreSelection();
        richText.win.document.execCommand("fontName", null, key);
        richTextControl.updateState();
        richTextControl.onchange();
    }

    var items = [];
    var sizes = [[1, "8pt"], [2, "10pt"], [3, "12pt"], [4, "14pt"], [5, "18pt"], [6, "24pt"], [7, "36pt"]];
    for (var i = 0; i < sizes.length; i++) {
        items.push(this.Item("sizeItem" + i, sizes[i][1], null, sizes[i][0]));
    }

    var sizeList = this.DropDownList(name + "FontSize", 55, null, items, true, false, null, true);
    sizeList.setKey(defaultFontSize);
    toolBar1.addCell(sizeList).style.padding = padding;

    sizeList.action = function () {
        var key = this.key;
        richText.restoreSelection();
        richText.win.document.execCommand("fontSize", null, key);
        richTextControl.updateState();
        richTextControl.onchange();
    }

    var buttonStyleBold = this.StandartSmallButton(null, null, null, "Bold.png");
    toolBar1.addCell(buttonStyleBold).style.padding = conpactSize ? "12px 1px 0 8px" : "12px 3px 0 8px";;

    buttonStyleBold.action = function () {
        richText.restoreSelection();
        richText.win.document.execCommand("bold", null, "");
        richTextControl.updateState();
        richTextControl.onchange();
    }

    var buttonStyleItalic = this.StandartSmallButton(null, null, null, "Italic.png");
    toolBar1.addCell(buttonStyleItalic).style.padding = padding;

    buttonStyleItalic.action = function () {
        richText.restoreSelection();
        richText.win.document.execCommand("italic", null, "");
        richTextControl.updateState();
        richTextControl.onchange();
    }

    var buttonStyleUnderline = this.StandartSmallButton(null, null, null, "Underline.png");
    toolBar1.addCell(buttonStyleUnderline).style.padding = padding;

    buttonStyleUnderline.action = function () {
        richText.restoreSelection();
        richText.win.document.execCommand("underline", null, "");
        richTextControl.updateState();
        richTextControl.onchange();
    }

    var sep1 = this.HomePanelSeparator();
    sep1.style.height = "26px";
    toolBar1.addCell(sep1).style.paddingTop = "12px";

    var colorControl = this.ColorControlWithImage(null, "TextColor.png", null, true);
    colorControl.setKey("255,0,0");
    toolBar1.addCell(colorControl).style.padding = padding;

    colorControl.action = function () {
        var hexColor = jsObject.RgbColorStrToHexColor(this.key);
        richText.restoreSelection();
        richText.win.document.execCommand("foreColor", null, hexColor);
        richTextControl.updateState();
        richTextControl.onchange();
    }

    var sep2 = this.HomePanelSeparator();
    sep2.style.height = "26px";
    toolBar1.addCell(sep2).style.paddingTop = "12px";

    var buttonAlignLeft = this.StandartSmallButton(null, null, null, "AlignLeft.png");
    toolBar1.addCell(buttonAlignLeft).style.padding = padding;

    buttonAlignLeft.action = function () {
        richText.restoreSelection();
        richText.win.document.execCommand("justifyLeft", null, "");
        richTextControl.updateState();
        richTextControl.onchange();
    }

    var buttonAlignCenter = this.StandartSmallButton(null, null, null, "AlignCenter.png");
    toolBar1.addCell(buttonAlignCenter).style.padding = padding;

    buttonAlignCenter.action = function () {
        richText.restoreSelection();
        richText.win.document.execCommand("justifyCenter", null, "");
        richTextControl.updateState();
        richTextControl.onchange();
    }

    var buttonAlignRight = this.StandartSmallButton(null, null, null, "AlignRight.png");
    toolBar1.addCell(buttonAlignRight).style.padding = padding;

    buttonAlignRight.action = function () {
        richText.restoreSelection();
        richText.win.document.execCommand("justifyRight", null, "");
        richTextControl.updateState();
        richTextControl.onchange();
    }

    var buttonAlignWidth = this.StandartSmallButton(null, null, null, "AlignWidth.png");
    toolBar1.addCell(buttonAlignWidth).style.padding = padding;

    buttonAlignWidth.action = function () {
        richText.restoreSelection();
        richText.win.document.execCommand("justifyFull", null, "");
        richTextControl.updateState();
        richTextControl.onchange();
    }

    var sep3 = this.HomePanelSeparator();
    sep3.style.height = "26px";
    toolBar1.addCell(sep3).style.paddingTop = "12px";

    var buttonUnorderedList = richTextControl.controls.buttonUnorderedList = this.StandartSmallButton(null, null, null, "UnorderedList.png");
    toolBar1.addCell(buttonUnorderedList).style.padding = padding;

    buttonUnorderedList.action = function () {
        richText.restoreSelection();
        richText.win.document.execCommand("insertUnorderedList", null, "");
        richTextControl.updateState();
        richTextControl.onchange();
    }

    var buttonOrderedList = richTextControl.controls.buttonOrderedList = this.StandartSmallButton(null, null, null, "OrderedList.png");
    toolBar1.addCell(buttonOrderedList).style.padding = padding;

    buttonOrderedList.action = function () {
        richText.restoreSelection();
        richText.win.document.execCommand("insertOrderedList", null, "");
        richTextControl.updateState();
        richTextControl.onchange();
    }

    var buttonHyperlink = richTextControl.controls.buttonHyperlink = this.StandartSmallButton(null, null, null, "Layout.Link.png", this.loc.Editor.InsertLink);
    var hLinkCell = toolBar1.addCell(buttonHyperlink)
    hLinkCell.style.padding = padding;
    hLinkCell.style.display = "none";

    var hyperlinkMenu = this.HyperlinkMenu(buttonHyperlink);

    buttonHyperlink.action = function () {
        hyperlinkMenu.changeVisibleState(!hyperlinkMenu.visible);
    }

    hyperlinkMenu.action = function () {
        if (this.textControl.value || this.urlControl.value) {
            var text = this.textControl.value || this.urlControl.value;
            richTextControl.insertText("<a href ='" + this.urlControl.value + "' target='_blank' >" + text + "</a>");
            richTextControl.onchange();
            richTextControl.setText(richTextControl.getText());
        };
    }

    var buttonClear = richTextControl.controls.buttonClear = this.StandartSmallButton(null, null, null, "ClearAllFormatting.png");
    var clearCell = toolBar1.addCell(buttonClear)
    clearCell.style.padding = padding;
    clearCell.style.display = "none";

    buttonClear.action = function () {
        var removeTagsFromText = function (text, tagName) {
            var startSymbols = ["<", "</"];

            for (var i = 0; i < startSymbols.length; i++) {
                var startSymbol = startSymbols[i]
                while (text.indexOf(startSymbol + tagName) >= 0) {
                    var startIndex = text.indexOf(startSymbol + tagName);
                    if (startIndex >= 0) {
                        var endIndex = text.indexOf(">", startIndex, text.length - startIndex);
                        if (endIndex >= 0)
                            text = text.substring(0, startIndex) + (endIndex < text.length - 1 ? text.substring(endIndex + 1) : "");
                        else
                            break;
                    }
                }
            }
            return text;
        }

        var text = richTextControl.getText();
        text = removeTagsFromText(text, "b");
        text = removeTagsFromText(text, "i");
        text = removeTagsFromText(text, "u");
        text = removeTagsFromText(text, "em");
        text = removeTagsFromText(text, "strong");
        text = removeTagsFromText(text, "font-color");
        text = removeTagsFromText(text, "text-align");
        text = text.replace(/color=/g, "");
        text = text.replace(/face=/g, "");

        richTextControl.setText(text);
        richText.selectAllText();
        richText.win.document.execCommand("fontSize", null, 3);
        richText.win.document.execCommand("fontName", null, "Arial");
        richText.win.document.execCommand("justifyLeft", null, "");
        richTextControl.updateState();
    }

    var expButton = richTextControl.controls.expButton = this.StandartSmallButton(null, null, null, "Function.png", this.loc.FormRichTextEditor.Insert, "Down");
    var expCell = toolBar1.addCell(expButton);
    expCell.style.padding = padding;
    expCell.style.display = "none";

    var expMenu = this.VerticalMenu(richTextControl.name + "RtfExpressionMenu", expButton, "Down", this.GetInsertExpressionItems());
    richTextControl.controls.expMenu = expMenu;

    expButton.action = function () {
        expMenu.changeVisibleState(!expMenu.visible);
    }

    expMenu.action = function (menuItem) {
        richTextControl.insertText("{" + menuItem.key + "}");
        expMenu.changeVisibleState(false);
    }

    /* Rich Text */

    var richText = document.createElement("iframe");
    richText.setAttribute("frameborder", "no");
    richText.setAttribute("src", "about:blank");
    richText.style.width = width + "px";
    richText.style.height = height + "px";
    richText.style.minWidth = width + "px";
    richText.style.minHeight = height + "px";
    richText.style.padding = "0";
    richText.style.margin = "12px";
    richText.className = "stiDesignerTextArea stiDesignerTextAreaDefault";
    if (hintText) richText.setAttribute(this.GetNavigatorName() == "MSIE" ? "title" : "placeholder", hintText);
    richTextControl.richText = richText;
    richTextControl.appendChild(richText);

    richTextControl.insertText = function (text) {
        var selection = richText.win ? richText.win.getSelection() : null;

        var alternativeInsert = function (text) {
            var currText = richTextControl.getText().replace(/&nbsp;/g, ' ');
            var resultText = currText + text;
            try {
                var mainPos = 0;
                var selectionPos = richText.selectionPos;
                if (selectionPos == null && selection) selectionPos = selection.focusOffset;
                if (richText.textContent) mainPos = currText.indexOf(richText.textContent);
                else if (selection && selection.focusNode) mainPos = currText.indexOf(selection.focusNode.textContent);
                if (mainPos == -1) mainPos = 0;
                selectionPos += mainPos;
                resultText = currText.substr(0, selectionPos) + text + currText.substr(selectionPos);
                richTextControl.setText(resultText);
            }
            catch (e) {
                richTextControl.setText(resultText);
            }
        }

        if (text) {
            try {
                if (richText.ranges && richText.ranges.length > 0 && richText.ranges[0].startContainer) {
                    var startContainer = richText.ranges[0].startContainer;
                    var startOffset = richText.ranges[0].startOffset;
                    var currText = startContainer.textContent || "";
                    currText = currText.replace(/&nbsp;/g, ' ');
                    currText = (currText && startOffset < currText.length)
                        ? currText.substring(0, startOffset) + text + currText.substring(startOffset)
                        : currText + text;
                    startContainer.textContent = currText;
                    if (selection) {
                        try {
                            richText.win.focus();
                            var range = document.createRange();
                            var tempDiv = document.createElement("div");
                            tempDiv.innerHTML = text;
                            range.setStart(startContainer, startOffset + tempDiv.textContent.length);
                            range.collapse(true);
                            selection.removeAllRanges();
                            selection.addRange(range);
                        }
                        catch (e) { }
                    }
                }
                else {
                    alternativeInsert(text);
                }
            }
            catch (e) {
                alternativeInsert(text);
            }
        }
    }

    richText.onblur = function () {
        if (!richText.win) return;
        var sel = richText.win.getSelection();
        richText.selectionPos = sel ? sel.focusOffset : null;
        richText.textContent = sel && sel.focusNode ? sel.focusNode.textContent : null;

        if (sel && sel.rangeCount) {
            richText.ranges = [];
            for (var i = 0, len = sel.rangeCount; i < len; ++i) {
                richText.ranges.push(sel.getRangeAt(i));
            }
        }
    }

    richText.onload = function () {
        this.win = this.contentWindow || this.window;
        this.doc = this.contentDocument || this.document;
        this.doc.designMode = "on";
        this.doc.addEventListener("drag", function (event) { event.preventDefault(); });
        this.doc.addEventListener("drop", function (event) { event.preventDefault(); });
        this.ranges = [];

        richText.win.document.execCommand("fontName", null, fontList.key);
        richText.win.document.execCommand("fontSize", null, sizeList.key);

        var body = this.doc.getElementsByTagName("body")[0];
        if (body) {
            body.style.margin = "0px";

            if (richTextControl.isEnabled) {
                body.click();
                body.focus();

                body.onfocus = function () { richTextControl.isFocused = true; richTextControl.setSelected(true); }
                body.onblur = function () { richTextControl.isFocused = false; richTextControl.setSelected(false); richTextControl.action(); richTextControl.onchange(); }
            }
            else {
                fontList.textBox.focus();
            }
        }

        if (richTextControl.needToConvertHtmlAttributes) {
            richTextControl.needToConvertHtmlAttributes = false;
            richText.convertHtmlAttributesForWysiwyg();
        }

        this.doc.onclick = function () {
            richTextControl.updateState();
        }

        this.doc.onmousedown = function () {
            richText.ranges = [];
            richTextControl.jsObject.options.mobileDesigner.pressedDown();
        }

        this.doc.onselect = function () {
            richTextControl.updateState();

            if (richText.win.getSelection) {
                richText.ranges = [];
                var sel = richText.win.getSelection();
                if (sel.rangeCount) {
                    for (var i = 0, len = sel.rangeCount; i < len; ++i) {
                        richText.ranges.push(sel.getRangeAt(i));
                    }
                }
            }
        }

        //Events
        if (this.win) {
            var itemInDragMove = function (event) {
                if (richTextControl.jsObject.options.itemInDrag) {
                    richTextControl.jsObject.options.itemInDrag.move(event, richTextControl.jsObject.FindPosX(richText, null, true), richTextControl.jsObject.FindPosY(richText, null, true));
                }
            }

            this.win.onmousemove = function (event) { itemInDragMove(event); };
            this.win.onmouseup = function (event) { richTextControl.insertText(); richTextControl.jsObject.DocumentMouseUp(event); }
            this.win.ontouchmove = function (event) { itemInDragMove(event); };
            this.win.ontouchend = function (event) { richTextControl.insertText(); richTextControl.jsObject.DocumentTouchEnd(event); }
        }
    }

    richText.convertHtmlAttributesForWysiwyg = function () {
        var tags = this.doc.getElementsByTagName("*");
        for (var i = 0; i < tags.length; i++) {
            var tag = tags[i];
            var size = tag.getAttribute("size");
            if (size) {
                var newSize = richTextControl.convertSizeMmToHtmlSize(parseInt(size));
                tag.setAttribute("size", newSize);
            }
        }
        for (var i = 0; i < tags.length; i++) {
            var tag = tags[i];
            if (tag.style) {
                if (jsObject.GetNavigatorName() == "MSIE" && tag.style.textAlign) {
                    tag.setAttribute("align", tag.style.textAlign.toLowerCase());
                    tag.style.textAlign = "";
                }
                if (tag.style.fontSize) {
                    var childs = [];
                    for (var k = 0; k < tag.childNodes.length; k++) {
                        childs.push(tag.childNodes[k]);
                    }
                    var font = document.createElement("font");
                    for (var k = 0; k < childs.length; k++) {
                        font.appendChild(childs[k]);
                    }
                    tag.appendChild(font);
                    font.setAttribute("size", richTextControl.convertSizeMmToHtmlSize(parseInt(tag.style.fontSize)));
                    tag.style.fontSize = "";
                }
            }
        }
    }

    richText.convertHtmlAttributesForReportEngine = function (isRtfComponent) {
        var tags = this.doc.getElementsByTagName("*");
        for (var i = 0; i < tags.length; i++) {
            var tag = tags[i];
            var size = tag.getAttribute("size");
            if (size) {
                var newSize = richTextControl.convertSizeHtmlSizeToMm(parseInt(size));
                tag.setAttribute("size", newSize);
            }
            var align = tag.getAttribute("align");
            if (align) {
                tag.style.textAlign = align;
                tag.removeAttribute("align");
            }
        }
        if (isRtfComponent) {
            for (var i = 0; i < tags.length; i++) {
                var tag = tags[i];
                var size = tag.getAttribute("size");
                if (size) {
                    tag.style.fontSize = size;
                    tag.removeAttribute("size");
                }
            }
        }
    }

    richText.restoreSelection = function () {
        this.win.focus();
        if (this.win.getSelection && this.ranges.length > 0) {
            var sel = this.win.getSelection();
            sel.removeAllRanges();
            for (var i = 0, len = this.ranges.length; i < len; ++i) {
                sel.addRange(this.ranges[i]);
            }
        }
    }

    richText.selectAllText = function () {
        var body = this.doc.getElementsByTagName("body")[0];

        if (body.createTextRange) {
            var range = body.createTextRange();
            range.moveToElementText(body);
            range.select();
        }
        else if (window.getSelection) {
            var selection = richText.win.getSelection();
            var range = this.doc.createRange();
            range.selectNodeContents(body);
            selection.removeAllRanges();
            selection.addRange(range);
        }
    }

    richText.removeAllSelection = function () {
        this.win.focus();
        if (this.win.getSelection) {
            var sel = this.win.getSelection();
            sel.removeAllRanges();
        }
    }

    richTextControl.getChildNodesByContent = function (parentNode, content, nodeName, nodes) {
        if (parentNode.childNodes.length == 0)
            return;

        for (var i = 0; i < parentNode.childNodes.length; i++) {
            var childNode = parentNode.childNodes[i];
            if (childNode.textContent == content && childNode.nodeName && childNode.nodeName == nodeName) {
                nodes.push(childNode);
            }
            richTextControl.getChildNodesByContent(childNode, content, nodeName, nodes);
        }
    }

    richTextControl.getCursorPosParameters = function () {
        richText.onblur();
        var params = {
            nodeName: null,
            currText: null,
            startOffset: null
        }
        if (richText.ranges && richText.ranges.length > 0 && richText.ranges[0].startContainer) {
            var startContainer = richText.ranges[0].startContainer;
            params.startOffset = richText.ranges[0].startOffset;
            params.currText = startContainer.textContent;
            params.nodeName = startContainer.nodeName;
        }
        return params;
    }

    richTextControl.tryToRestoreCursorPos = function (params, win, doc) {
        if (params.currText && params.nodeName) {
            var nodes = [];
            richTextControl.getChildNodesByContent(doc, params.currText, params.nodeName, nodes);
            var selection = win.getSelection();
            if (selection && nodes.length > 0) {
                win.focus();
                var range = doc.createRange();
                range.setStart(nodes[0], params.startOffset);
                range.collapse(true);
                selection.removeAllRanges();
                selection.addRange(range);
            }
        }
    }

    richTextControl.resize = function (newWidth, newHeight) {
        richTextControl.width = newWidth;
        richTextControl.height = newHeight;
        richText.style.width = newWidth + "px";
        richText.style.height = newHeight + "px";
        richText.style.minWidth = newWidth + "px";
        richText.style.minHeight = newHeight + "px";
    }

    richTextControl.updateState = function () {
        if (!this.isEnabled) return;
        buttonStyleBold.setSelected(richText.win.document.queryCommandState("bold"));
        buttonStyleItalic.setSelected(richText.win.document.queryCommandState("italic"));
        buttonStyleUnderline.setSelected(richText.win.document.queryCommandState("underline"));
        buttonAlignLeft.setSelected(richText.win.document.queryCommandState("justifyLeft"));
        buttonAlignCenter.setSelected(richText.win.document.queryCommandState("justifyCenter"));
        buttonAlignRight.setSelected(richText.win.document.queryCommandState("justifyRight"));
        buttonAlignWidth.setSelected(richText.win.document.queryCommandState("justifyFull"));
        buttonUnorderedList.setSelected(richText.win.document.queryCommandState("insertUnorderedList"));
        buttonOrderedList.setSelected(richText.win.document.queryCommandState("insertOrderedList"));

        var fontName = richText.win.document.queryCommandValue("fontName") || defaultFontName;
        if (fontName && fontName.indexOf("\"") == 0 && jsObject.EndsWith(fontName, "\"")) {
            fontName = fontName.substring(1, fontName.length - 1);
        }
        fontList.setKey(fontName);

        var fontSize = richText.win.document.queryCommandValue("fontSize");
        sizeList.setKey(fontSize || defaultFontSize);

        var rgbColor = "0,0,0";
        var fontColor = richText.win.document.queryCommandValue("foreColor");
        if (fontColor) {
            if (fontColor.toString().toLowerCase().indexOf("rgb") == 0) {
                rgbColor = fontColor.substring(4, fontColor.indexOf(")"));
            }
            else {
                var b = (fontColor >> 16) & 255;
                var g = (fontColor >> 8) & 255;
                var r = fontColor & 255;
                rgbColor = r + "," + g + "," + b;
            }
        }
        colorControl.setKey(rgbColor);
    }

    richTextControl.checkEvents = function (text) {
        var events = ["addEventListener", "attachEvent", "onload", "onclick", "onerror", "onfocus", "onabort", "onbeforeunload", "onblur", "onchange", "oninvalid",
            "onmousedown", "onmouseover", "onmouseout", "onmouseeneter", "onmouseleave", "onunload", "javascript:"];
        text = text.toLowerCase();

        for (var i = 0; i < events.length; i++) {
            if (text.indexOf(events[i].toLowerCase()) >= 0)
                return true;
        }
    }

    richTextControl.setText = function (text, currentFont, convertAttributes) {
        var win = richText.contentWindow || richText.window;
        var doc = win.document || win.contentDocument;
        richText.win = win;

        //replace "<font-color>" --> "<font color" & "size=" --> "style="font-size"
        text = text.replace(/<font-color/g, '<font color');
        text = text.replace(/<\/font-color>/g, '</font>');
        text = text.replace(/<!--/g, '<\!--').replace(/<script/g, '<\script').replace(/<\/script/g, '<\\/script');

        if (richTextControl.checkEvents(text)) text = "";

        if (convertAttributes) {
            richTextControl.needToConvertHtmlAttributes = true;
        }

        var cursorPosParams = richTextControl.getCursorPosParameters();

        if (doc) {
            doc.open();
            doc.write(text);
            doc.close();

            if (currentFont) {
                var body = doc.getElementsByTagName("body")[0];
                body.style.fontFamily = currentFont.name;
                defaultFontName = currentFont.name;
                defaultFontSize = richTextControl.convertSizeMmToHtmlSize(parseInt(currentFont.size));
                buttonStyleBold.setSelected(currentFont.bold == "1");
                buttonStyleItalic.setSelected(currentFont.italic == "1");
            }
        }

        richTextControl.tryToRestoreCursorPos(cursorPosParams, win, doc);

        sizeList.setKey(defaultFontSize);
        this.updateState();
    }

    richTextControl.convertFontSizes = function (text, mmToHtml) {
        var oldString = "size=\"";
        while (text.indexOf(oldString) >= 0) {
            var newString = "convertedSize=\"";
            var startIndex = text.indexOf(oldString) + newString.length;
            text = text.replace("size=\"", newString);

            var firstPart = text.substring(0, startIndex);
            var tempPart = text.substring(startIndex);
            var middlePart = tempPart.substring(0, tempPart.indexOf("\""));
            var lastPart = tempPart.substring(tempPart.indexOf("\""));
            text = firstPart + (mmToHtml ? richTextControl.convertSizeMmToHtmlSize(parseInt(middlePart.trim())) : richTextControl.convertSizeHtmlSizeToMm(parseInt(middlePart.trim()))) + lastPart;
        }
        text = text.replace(/convertedSize=\"/g, "size=\"");
        return text;
    }

    richTextControl.convertSizeMmToHtmlSize = function (sizeMm) {
        if (sizeMm <= 8) return 1;
        else if (sizeMm > 8 && sizeMm <= 10) return 2;
        else if (sizeMm > 10 && sizeMm <= 12) return 3;
        else if (sizeMm > 12 && sizeMm <= 14) return 4;
        else if (sizeMm > 14 && sizeMm <= 18) return 5;
        else if (sizeMm > 18 && sizeMm <= 24) return 6;
        else if (sizeMm > 24) return 7;
    }

    richTextControl.convertSizeHtmlSizeToMm = function (sizeHtml) {
        var sizes = { 1: 8, 2: 10, 3: 12, 4: 14, 5: 18, 6: 24, 7: 36 };
        return sizes[sizeHtml];
    }

    richTextControl.getText = function (convertAttributes, isRtfComponent) {
        var win = richText.contentWindow || richText.window;
        var doc = win.document || win.contentDocument;
        richText.win = win;

        if (convertAttributes) {
            richText.convertHtmlAttributesForReportEngine(isRtfComponent);
        }

        var htmlText = doc ? doc.body.innerHTML : "";
        htmlText = htmlText.replace(/&gt;/g, '>').replace(/&lt;/g, '<').replace(/&amp;/g, "&");

        //replace rgb color -> hex color
        var startIndex = 0;
        while (startIndex >= 0) {
            startIndex = htmlText.indexOf("rgb(");
            if (startIndex == -1) startIndex = htmlText.indexOf("RGB(");
            if (startIndex >= 0) {
                var endIndex = htmlText.indexOf(")", startIndex);
                var hexColor = richTextControl.jsObject.RgbColorStrToHexColor(htmlText.substr(startIndex, endIndex - startIndex + 1));
                htmlText = htmlText.substring(0, startIndex) + hexColor + htmlText.substring(endIndex + 1);
            }
        }

        return htmlText;
    }

    richTextControl.setRichTextContent = function (itemObject) {
        if (!itemObject) {
            richTextControl.clearResourceContent();
        }
        else if (itemObject.content) {
            richTextControl.loadResourceContent(itemObject.content);
        }
        else {
            this.jsObject.SendCommandToDesignerServer("GetRichTextContent", { itemObject: itemObject }, function (answer) {
                itemObject.content = StiBase64.decode(answer.content);
                richTextControl.loadResourceContent(itemObject.content);
            });
        }
    }

    richTextControl.loadResourceContent = function (content) {
        this.setText(content || "");
        richTextControl.setEnabled(false);
        removeButton.style.display = "";
        richTextControl.onLoadResourceContent(content);
    }

    richTextControl.clearResourceContent = function () {
        richTextControl.setEnabled(true);
        removeButton.style.display = "none";
        this.setText("");
        richTextControl.onchange();
    }

    richTextControl.onmouseenter = function () {
        if (!this.isEnabled || this.jsObject.options.isTouchClick) return;
        this.isOver = true;
        if (!this.isSelected && !this.isFocused)
            richText.className = "stiDesignerTextArea stiDesignerTextAreaOver";
    }

    richTextControl.onmouseleave = function () {
        if (!this.isEnabled) return;
        this.isOver = false;
        if (!this.isSelected && !this.isFocused)
            richText.className = "stiDesignerTextArea stiDesignerTextAreaDefault";
    }

    richTextControl.setEnabled = function (state) {
        sizeList.setEnabled(state);
        fontList.setEnabled(state);
        colorControl.setEnabled(state);
        buttonStyleBold.setEnabled(state);
        buttonStyleItalic.setEnabled(state);
        buttonStyleUnderline.setEnabled(state);
        buttonAlignLeft.setEnabled(state);
        buttonAlignCenter.setEnabled(state);
        buttonAlignRight.setEnabled(state);
        buttonAlignWidth.setEnabled(state);
        buttonUnorderedList.setEnabled(state);
        buttonOrderedList.setEnabled(state);
        if (richText.doc && !state) {
            richText.doc.designMode = "off";
            var body = richText.doc.getElementsByTagName("body")[0];
            if (body) body.onfocus = null;
        }

        this.isEnabled = state;
        this.disabled = !state;
        richText.className = "stiDesignerTextArea stiDesignerTextArea" + (state ? "Default" : "Disabled");
    }

    richTextControl.setSelected = function (state) {
        this.isSelected = state;
        richText.className = "stiDesignerTextArea stiDesignerTextArea" + (state ? "Over" : (this.isEnabled ? (this.isOver ? "Over" : "Default") : "Disabled"));
    }

    richTextControl.action = function () { };
    richTextControl.onchange = function () { };
    richTextControl.onLoadResourceContent = function (content) { };

    return richTextControl;
}

StiMobileDesigner.prototype.RichTextContainer = function (width, height) {
    var container = document.createElement("div");
    container.jsObject = this;
    container.className = "stiImageContainerWithBorder";
    container.style.overflow = "auto";
    container.style.width = width + "px";
    container.style.height = height + "px";
    this.AddProgressToControl(container);
    var innerContainer = document.createElement("div");
    container.appendChild(innerContainer);

    container.resize = function (newWidth, newHeight) {
        container.style.width = newWidth + "px";
        container.style.height = newHeight + "px";
    }

    container.setRichTextContent = function (itemObject) {
        container.progress.hide();
        if (itemObject) {
            if (itemObject.content) {
                innerContainer.innerHTML = itemObject.content;
            }
            else if (itemObject.url) {
                container.progress.show();
                this.jsObject.SendCommandToDesignerServer("GetRichTextContent", { itemObject: itemObject }, function (answer) {
                    container.progress.hide();
                    itemObject.content = StiBase64.decode(answer.content);
                    innerContainer.innerHTML = itemObject.content;
                });
            }
        }
        else {
            this.innerHTML = "";
        }
    }

    return container;
}