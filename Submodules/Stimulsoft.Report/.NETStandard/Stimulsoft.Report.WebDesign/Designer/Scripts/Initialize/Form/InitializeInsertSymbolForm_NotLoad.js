
StiMobileDesigner.prototype.InitializeInsertSymbolForm_ = function () {
    var form = this.BaseForm("insertSymbolForm", this.loc.Editor.InsertSymbol, 1);
    var jsObject = this;

    var mainContainer = this.SymbolsContainer(610, 500);
    form.container.appendChild(mainContainer);
    form.buttonOk.caption.innerHTML = this.loc.PropertyMain.Insert;

    var recentText = document.createElement("div");
    recentText.innerHTML = this.loc.FormDatabaseEdit.RecentConnections;
    recentText.className = "stiDesignerTextContainer";
    recentText.style.margin = "12px 12px 6px 12px";
    form.container.appendChild(recentText);

    var recentContainer = this.SymbolsContainer(610, 30);
    recentContainer.style.margin = "0px 12px 12px 12px";
    recentContainer.style.overflow = "hidden";
    form.container.appendChild(recentContainer);

    form.show = function (richTextControl) {
        mainContainer.clear();
        recentContainer.clear();
        this.selectedButton = null;
        this.richTextControl = richTextControl;
        this.changeVisibleState(true);
        this.buttonOk.setEnabled(false);

        var fontName = richTextControl.controls.fontList.key;
        var symbolsCookie = jsObject.GetCookie("StimulsoftMobileRecentSymbols");
        this.recentSymbols = symbolsCookie ? StiBase64.decode(symbolsCookie) : "€£¥©®™≠≤≥÷∞µαβπΩ∑§†‡";

        for (var i = 0; i < this.recentSymbols.length; i++) {
            var button = jsObject.SymbolsButton(form, this.recentSymbols[i], fontName);
            recentContainer.appendChild(button);
        }

        jsObject.SendCommandToDesignerServer("GetSpecialSymbols", { fontName: fontName }, function (answer) {
            if (answer.symbols) {
                for (var i = 0; i < answer.symbols.length; i++) {
                    var button = jsObject.SymbolsButton(form, answer.symbols[i], fontName);
                    mainContainer.appendChild(button);
                }
            }
        });
    }

    form.action = function () {
        this.changeVisibleState(false);

        var insrtSymbol = form.selectedButton.symbol;
        if (this.recentSymbols.indexOf(insrtSymbol) < 0) {
            this.recentSymbols += insrtSymbol;
        }
        var startIndex = this.recentSymbols.length - 20;
        if (startIndex < 0) startIndex = 0;
        jsObject.SetCookie("StimulsoftMobileRecentSymbols", StiBase64.encode(this.recentSymbols.substring(startIndex, this.recentSymbols.length)));

        if (form.selectedButton)
            this.richTextControl.insertText(insrtSymbol);

        this.richTextControl.onchange();
    }

    form.onhide = function () {
        delete jsObject.options.forms[form.name];
        jsObject.options.mainPanel.removeChild(form);
    }

    return form;
}

StiMobileDesigner.prototype.SymbolsContainer = function (width, height) {
    var container = document.createElement("div");
    container.className = "stiDesignerSymbolsContainer";
    container.style.width = width + "px";
    container.style.height = height + "px";

    container.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
    }

    return container;
}

StiMobileDesigner.prototype.SymbolsButton = function (form, symbol, fontName) {
    var button = this.StandartSmallButton(null, null, symbol);
    button.style.display = "inline-block";
    button.style.width = button.style.height = "28px";
    button.innerTable.style.width = "100%";
    button.caption.style.textAlign = "center";
    button.caption.style.fontSize = "14px";
    button.caption.style.fontFamily = fontName;
    button.style.margin = "0 2px 2px 0";
    button.style.overflow = "hidden";
    button.symbol = symbol;

    button.action = function () {
        if (form.selectedButton) form.selectedButton.setSelected(true);
        this.setSelected(true);
        form.selectedButton = this;
        form.buttonOk.setEnabled(true);
    };

    button.ondblclick = function () {
        form.selectedButton = this;
        form.action();
    };

    return button;
}