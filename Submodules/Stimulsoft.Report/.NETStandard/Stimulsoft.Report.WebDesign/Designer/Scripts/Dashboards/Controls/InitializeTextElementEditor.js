
StiMobileDesigner.prototype.TextElementEditor = function (name, width, height, hintText) {

    var richTextControl = document.createElement("div");
    if (name != null) this.options.controls[name] = richTextControl;
    richTextControl.controls = {};
    var jsObject = richTextControl.jsObject = this;
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
    var defaultFontSize = "8";

    /* Tool Bar 1 */
    var toolBar1 = this.CreateHTMLTable();
    toolBar1.addRow();
    richTextControl.toolBar1 = toolBar1;
    richTextControl.appendChild(toolBar1);

    var fontList = this.FontList(name + "FontList", 180, null, true);
    richTextControl.controls.fontList = fontList;
    fontList.setKey(defaultFontName);
    toolBar1.addCell(fontList);

    var sizes = [5, 6, 7, 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 76];
    var items = [];
    for (var size in sizes) items.push(this.Item("sizeItem" + size, sizes[size], null, sizes[size]));
    var sizeList = this.DropDownList(name + "FontSize", 55, null, items, true, false, null, true);
    richTextControl.controls.sizeList = sizeList;
    sizeList.setKey(defaultFontSize);
    sizeList.style.marginLeft = "4px";
    toolBar1.addCell(sizeList);

    var sep1 = this.HomePanelSeparator();
    sep1.style.height = "26px";
    sep1.style.marginLeft = "4px";
    toolBar1.addCell(sep1);

    var buttonStyleBold = this.StandartSmallButton(null, null, null, "Bold.png");
    richTextControl.controls.buttonStyleBold = buttonStyleBold;
    buttonStyleBold.style.marginLeft = "4px";
    toolBar1.addCell(buttonStyleBold);
    buttonStyleBold.action = function () {
        richText.restoreSelection();
        richText.win.document.execCommand("bold", null, "");
        richTextControl.updateState();
        richTextControl.onchange();
    }

    var buttonStyleItalic = this.StandartSmallButton(null, null, null, "Italic.png");
    richTextControl.controls.buttonStyleItalic = buttonStyleItalic;
    buttonStyleItalic.style.marginLeft = "4px";
    toolBar1.addCell(buttonStyleItalic);
    buttonStyleItalic.action = function () {
        richText.restoreSelection();
        richText.win.document.execCommand("italic", null, "");
        richTextControl.updateState();
        richTextControl.onchange();
    }

    var buttonStyleUnderline = this.StandartSmallButton(null, null, null, "Underline.png");
    richTextControl.controls.buttonStyleUnderline = buttonStyleUnderline;
    buttonStyleUnderline.style.marginLeft = "4px";
    toolBar1.addCell(buttonStyleUnderline);
    buttonStyleUnderline.action = function () {
        richText.restoreSelection();
        richText.win.document.execCommand("underline", null, "");
        richTextControl.updateState();
        richTextControl.onchange();
    }

    var sep2 = this.HomePanelSeparator();
    sep2.style.height = "26px";
    sep2.style.marginLeft = "4px";
    toolBar1.addCell(sep2);

    var colorControl = this.ColorControlWithImage(null, "TextColor.png", null, true);
    richTextControl.controls.colorControl = colorControl;
    colorControl.setKey("255,0,0");
    colorControl.style.marginLeft = "4px";
    toolBar1.addCell(colorControl);
    colorControl.action = function () {
        var hexColor = "#ffffff";

        if (this.key != "transparent" && this.key != "0,255,255,255") {
            var color = [255, 255, 255];
            var colorArray = this.key.split(",");
            if (colorArray.length == 4) color = [colorArray[1], colorArray[2], colorArray[3]];
            if (colorArray.length == 3) color = [colorArray[0], colorArray[1], colorArray[2]];
            hexColor = jsObject.RgbToHex(parseInt(color[0]), parseInt(color[1]), parseInt(color[2]));
        }

        richText.restoreSelection();
        richText.win.document.execCommand("foreColor", null, hexColor);
        richTextControl.updateState();
        richTextControl.onchange();
    }

    var sep3 = this.HomePanelSeparator();
    sep3.style.height = "26px";
    sep3.style.marginLeft = "4px";
    toolBar1.addCell(sep3);

    var buttonAlignLeft = this.StandartSmallButton(null, null, null, "AlignLeft.png");
    richTextControl.controls.buttonAlignLeft = buttonAlignLeft;
    buttonAlignLeft.style.marginLeft = "4px";
    toolBar1.addCell(buttonAlignLeft);

    var buttonAlignCenter = this.StandartSmallButton(null, null, null, "AlignCenter.png");
    richTextControl.controls.buttonAlignCenter = buttonAlignCenter;
    buttonAlignCenter.style.marginLeft = "4px";
    toolBar1.addCell(buttonAlignCenter);

    var buttonAlignRight = this.StandartSmallButton(null, null, null, "AlignRight.png");
    richTextControl.controls.buttonAlignRight = buttonAlignRight;
    buttonAlignRight.style.marginLeft = "4px";
    toolBar1.addCell(buttonAlignRight);

    var buttonAlignWidth = this.StandartSmallButton(null, null, null, "AlignWidth.png");
    richTextControl.controls.buttonAlignWidth = buttonAlignWidth;
    buttonAlignWidth.style.marginLeft = "4px";
    toolBar1.addCell(buttonAlignWidth);

    var sep4 = this.HomePanelSeparator();
    sep4.style.height = "26px";
    sep4.style.marginLeft = "4px";
    toolBar1.addCell(sep4);

    if (!this.options.netCoreMode && !this.options.jsMode) {
        var buttonInsertSymbol = this.StandartSmallButton(null, null, null, "Symbol.png", this.loc.Editor.InsertSymbol);
        richTextControl.controls.buttonInsertSymbol = buttonInsertSymbol;
        buttonInsertSymbol.style.marginLeft = "4px";
        toolBar1.addCell(buttonInsertSymbol);

        buttonInsertSymbol.action = function () {
            richTextControl.richText.onblur();
            jsObject.InitializeInsertSymbolForm(function (form) {
                form.show(richTextControl);
            });
        }
    }

    var buttonHyperlink = this.StandartSmallButton(null, null, null, "Layout.Link.png", this.loc.Editor.InsertLink);
    richTextControl.controls.buttonHyperlink = buttonHyperlink;
    buttonHyperlink.style.marginLeft = "4px";
    toolBar1.addCell(buttonHyperlink);

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

    var buttonClearFormatting = this.StandartSmallButton(null, null, null, "ClearAllFormatting.png", this.loc.Dashboard.ClearAllFormatting);
    richTextControl.controls.buttonClearFormatting = buttonClearFormatting;
    buttonClearFormatting.style.marginLeft = "4px";
    toolBar1.addCell(buttonClearFormatting);

    var separator = document.createElement("div");
    separator.className = "stiDesignerFormSeparator";
    separator.style.margin = "6px 0 6px 0";
    richTextControl.toolBarsSeparator = separator;
    richTextControl.appendChild(separator);

    /* Rich Text */
    var richText = document.createElement("iframe");
    richText.setAttribute("frameborder", "no");
    richText.setAttribute("src", "about:blank");
    richText.style.width = width + "px";
    richText.style.height = height + "px";
    richText.style.minWidth = width + "px";
    richText.style.minHeight = height + "px";
    richText.style.padding = "0";
    richText.style.margin = "2px 0px 0px 6px";
    richText.className = "stiDesignerTextArea stiDesignerTextAreaDefault";
    if (hintText) richText.setAttribute(this.GetNavigatorName() == "MSIE" ? "title" : "placeholder", hintText);
    richTextControl.richText = richText;
    richTextControl.appendChild(richText);

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
        richTextControl.onLoadComplete();

        var body = this.doc.getElementsByTagName("body")[0];
        if (body) {
            body.style.margin = "0px";

            if (richTextControl.isEnabled) {
                body.click();
                body.focus();

                body.onfocus = function () {
                    richTextControl.isFocused = true;
                    richTextControl.setSelected(true);
                }

                body.onblur = function () {
                    richTextControl.isFocused = false;
                    richTextControl.setSelected(false);
                }
            }
            else {
                fontList.textBox.focus();
            }
        }

        this.doc.onclick = function () {
            richTextControl.updateState();
        }

        this.doc.onmousedown = function () {
            richText.ranges = [];
            jsObject.options.mobileDesigner.pressedDown();
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
            var keyTimer;

            this.win.onkeyup = function (event) {
                richTextControl.writingInProgress = true;
                clearTimeout(keyTimer);
                keyTimer = setTimeout(function () {
                    richTextControl.writingInProgress = false;
                    richTextControl.onchangetext();
                }, 1000);
            };

            var itemInDragMove = function (event) {
                if (jsObject.options.itemInDrag) {
                    jsObject.options.itemInDrag.move(event, jsObject.FindPosX(richText, null, true), jsObject.FindPosY(richText, null, true));
                }
            }

            var getItemText = function () {
                var text = "";
                var dictionaryTree = jsObject.options.dictionaryTree;
                if (dictionaryTree && jsObject.options.itemInDrag && jsObject.options.itemInDrag.originalItem) {
                    text = jsObject.options.itemInDrag.originalItem.getResultForEditForm();
                }
                return text;
            }

            this.win.onmousemove = function (event) { itemInDragMove(event); };
            this.win.ontouchmove = function (event) { itemInDragMove(event); };

            this.win.onmouseup = function (event) {
                var text = getItemText();
                if (text) {
                    richTextControl.insertText(getItemText());
                    richTextControl.onchangetext();
                }
                richTextControl.jsObject.DocumentMouseUp(event);
            }
            this.win.ontouchend = function (event) {
                var text = getItemText();
                if (text) {
                    richTextControl.insertText(getItemText());
                    richTextControl.onchangetext();
                }
                richTextControl.jsObject.DocumentTouchEnd(event);
            }
        }
    }

    richText.restoreSelection = function () {
        this.win.focus();
        if (this.win.getSelection && this.ranges && this.ranges.length > 0) {
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
        var richText = this.richText;
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

    richTextControl.setText = function (text, currentFont, horAlignment) {
        var win = this.richText.contentWindow || this.richText.window;
        var doc = win.document || win.contentDocument;
        this.richText.win = win;

        //replace "<font-color>" --> "<font color" & "size=" --> "style="font-size"
        text = text.replace(/<font-color/g, '<font color');
        text = text.replace(/<\/font-color>/g, '</font>');
        text = text.replace(/<!--/g, '<\!--').replace(/<script/g, '<\script').replace(/<\/script/g, '<\\/script');

        var oldString = "size=";
        while (text.indexOf(oldString) >= 0) {
            var newString = "style=\"font-size:";
            var startIndex = text.indexOf(oldString) + newString.length;
            text = text.replace("size=\"", newString);

            var firstPart = text.substring(0, startIndex);
            var tempPart = text.substring(startIndex);
            var middlePart = tempPart.substring(0, tempPart.indexOf("\""));
            var lastPart = tempPart.substring(tempPart.indexOf("\""));
            text = firstPart + middlePart + "px" + lastPart;
        }

        var cursorPosParams = richTextControl.getCursorPosParameters();

        if (doc) {
            doc.open();
            doc.write(text);
            doc.close();
        }

        richTextControl.tryToRestoreCursorPos(cursorPosParams, win, doc);

        this.updateState();
    }

    richTextControl.getText = function (text) {
        var win = this.richText.contentWindow || this.richText.window;
        var doc = win.document || win.contentDocument;
        this.richText.win = win;

        var htmlText = doc ? doc.body.innerHTML : "";
        htmlText = htmlText.replace(/&gt;/g, '>').replace(/&lt;/g, '<');

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

        //replace "font-size" --> "size"
        var oldString = "style=\"font-size:";
        while (htmlText.indexOf(oldString) >= 0) {
            var newString = "size=\"";
            startIndex = htmlText.indexOf(oldString) + newString.length;
            htmlText = htmlText.replace(oldString, newString);

            var firstPart = htmlText.substring(0, startIndex);
            var tempPart = htmlText.substring(startIndex);
            var middlePart = tempPart.substring(0, tempPart.indexOf("\""));
            var lastPart = tempPart.substring(tempPart.indexOf("\""));
            htmlText = firstPart + middlePart.replace(/pt;/g, '').replace(/pt/g, '').replace(/px;/g, '').replace(/px/g, '').trim() + lastPart;
        }

        return htmlText;
    }

    richTextControl.onmouseenter = function () {
        if (!this.isEnabled || this.jsObject.options.isTouchClick) return;
        this.isOver = true;
        if (!this.isSelected && !this.isFocused)
            this.richText.className = "stiDesignerTextArea stiDesignerTextAreaOver";
    }

    richTextControl.onmouseleave = function () {
        if (!this.isEnabled) return;
        this.isOver = false;
        if (!this.isSelected && !this.isFocused)
            this.richText.className = "stiDesignerTextArea stiDesignerTextAreaDefault";
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

        if (richText.doc && !state) {
            richText.doc.designMode = "off";
            var body = richText.doc.getElementsByTagName("body")[0];
            if (body) body.onfocus = null;
        }
        this.isEnabled = state;
        this.disabled = !state;
        this.richText.className = "stiDesignerTextArea stiDesignerTextArea" + (state ? "Default" : "Disabled");
    }

    richTextControl.setSelected = function (state) {
        this.isSelected = state;
        this.richText.className = "stiDesignerTextArea stiDesignerTextArea" + (state ? "Over" : (this.isEnabled ? (this.isOver ? "Over" : "Default") : "Disabled"));
    }

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

    richTextControl.action = function () { };
    richTextControl.onchange = function () { };
    richTextControl.onchangetext = function () { };
    richTextControl.onLoadComplete = function () { };

    return richTextControl;
}