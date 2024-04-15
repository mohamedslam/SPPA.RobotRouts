
StiMobileDesigner.prototype.PageSizeMenu = function (name, parentButton) {
    var items = [];
    for (var i = 0; i < this.options.paperSizes.length; i++) {
        if (this.options.paperSizes[i] != "none")
            items.push(this.Item("pageSize" + i, this.options.paperSizes[i], null, i.toString()));
    }

    var menu = this.VerticalMenu(name, parentButton, "Down", items, "stiDesignerMenuMiddleItem");

    menu.action = function (menuItem) {
        menuItem.setSelected(true);
        this.changeVisibleState(false);
        this.jsObject.options.currentPage.properties.paperSize = menuItem.key;
        if (menuItem.key != "0") {
            var pageWidth = this.jsObject.ConvertPixelToUnit(this.jsObject.PaperSizesInPixels[parseInt(menuItem.key)][0]).toFixed(1);
            var pageHeight = this.jsObject.ConvertPixelToUnit(this.jsObject.PaperSizesInPixels[parseInt(menuItem.key)][1]).toFixed(1);
            if (Math.round(pageWidth) == pageWidth) pageWidth = Math.round(pageWidth);
            if (Math.round(pageHeight) == pageHeight) pageHeight = Math.round(pageHeight);
            var orientation = this.jsObject.options.currentPage.properties.orientation;
            this.jsObject.options.currentPage.properties.unitWidth = (orientation == "Portrait") ? pageWidth.toString() : pageHeight.toString();
            this.jsObject.options.currentPage.properties.unitHeight = (orientation == "Portrait") ? pageHeight.toString() : pageWidth.toString();
        }
        this.jsObject.SendCommandSendProperties(this.jsObject.options.currentPage, ["paperSize", "unitWidth", "unitHeight"]);
    }

    menu.onshow = function () {
        var textSizes = function (jsObject, num) {
            if (num == 0) return "";
            var unit = jsObject.options.report.properties.reportUnit;
            var pageWidth = jsObject.ConvertPixelToUnit(jsObject.PaperSizesInPixels[num][0]).toFixed(1);
            var pageHeight = jsObject.ConvertPixelToUnit(jsObject.PaperSizesInPixels[num][1]).toFixed(1);

            return pageWidth + unit + " X " + pageHeight + unit;
        }

        for (var itemName in this.items) {
            var num = parseInt(this.items[itemName].key);
            if (this.items[itemName].key == this.jsObject.options.currentPage.properties.paperSize) this.items[itemName].setSelected(true);
            this.items[itemName].caption.innerHTML = "<b>" + this.jsObject.options.paperSizes[num] + "</b><br>  " +
                textSizes(this.jsObject, num);
        }
    }

    return menu;
}

StiMobileDesigner.prototype.PaperSizesInPixels = [
    [0, 0], //0 - Custom
    [850, 1100], //1 - Letter
    [850, 1100], //2 - LetterSmall
    [1100, 1700], //3 - Tabloid
    [1700, 1100], //4 - Ledger
    [850, 1400], //5 - Legal
    [550, 850], //6 - Statement
    [725, 1050], //7 - Executive
    [1169, 1654], //8 - A3
    [827, 1169], //9 - A4
    [827, 1169], //10 - A4Small
    [583, 827], //11 - A5
    [1012, 1433], //12 - B4
    [717, 1012], //13 - B5
    [850, 1300], //14 - Folio
    [846, 1083], //15 - Quarto
    [1000, 1400], //16 - Standard10x14
    [1100, 1700], //17 - Standard11x17
    [850, 1100], //18 - Note
    [387, 887], //19 - Number9Envelope
    [412, 950], //20 - Number10Envelope
    [450, 1037], //21 - Number11Envelope
    [475, 1100], //22 - Number12Envelope
    [500, 1150], //23 - Number14Envelope
    [1700, 2200], //24 - CSheet
    [2200, 3400], //25 - DSheet
    [3400, 4400], //26 - ESheet
    [433, 866], //27 - DLEnvelope
    [638, 902], //28 - C5Envelope
    [1276, 1803], //29 - C3Envelope
    [902, 1276], //30 - C4Envelope
    [449, 638], //31 - C6Envelope
    [449, 902], //32 - C65Envelope
    [984, 1390], //33 - B4Envelope
    [693, 984], //34 - B5Envelope
    [693, 492], //35 - B6Envelope
    [433, 906], //36 - ItalyEnvelope
    [387, 750], //37 - MonarchEnvelope
    [362, 650], //38 - PersonalEnvelope
    [1487, 1100], //39 - USStandardFanfold
    [850, 1200], //40 - GermanStandardFanfold
    [850, 1300], //41 - GermanLegalFanfold
    [984, 1390], //42 - IsoB4
    [394, 583], //43 - JapanesePostcard
    [900, 1100], //44 - Standard9x11
    [1000, 1100], //45 - Standard10x11
    [1500, 1100], //46 - Standard15x11
    [866, 866], //47 - InviteEnvelope
    [0, 0], //48 - 
    [0, 0], //49 -  
    [950, 1200], //50 - LetterExtra
    [950, 1500], //51 - LegalExtra
    [1200, 1800], //52 - TabloidExtra
    [927, 1269], //53 - A4Extra
    [850, 1100], //54 - LetterTransverse
    [827, 1169], //55 - A4Transverse
    [950, 1200], //56 - LetterExtraTransverse
    [894, 1402], //57 - APlus
    [1201, 1917], //58 - BPlus
    [850, 1269], //59 - LetterPlus
    [827, 1299], //60 - A4Plus
    [583, 827], //61 - A5Transverse
    [717, 1012], //62 - B5Transverse
    [1268, 1752], //63 - A3Extra
    [685, 925], //64 - A5Extra
    [791, 1087], //65 - B5Extra
    [1654, 2339], //66 - A2
    [1169, 1654], //67 - A3Transverse
    [1268, 1752], //68 - A3ExtraTransverse
    [787, 583], //69 - JapaneseDoublePostcard
    [413, 583], //70 - A6
    [945, 1307], //71 - JapaneseEnvelopeKakuNumber2
    [850, 1091], //72 - JapaneseEnvelopeKakuNumber3
    [472, 925], //73 - JapaneseEnvelopeChouNumber3
    [354, 807], //74 - JapaneseEnvelopeChouNumber4
    [1100, 850], //75 - LetterRotated
    [1654, 1169], //76 - A3Rotated
    [1169, 827], //77 - A4Rotated
    [827, 583], //78 - A5Rotated
    [1433, 1012], //79 - B4JisRotated
    [1012, 717], //80 - B5JisRotated
    [583, 394], //81 - JapanesePostcardRotated
    [583, 787], //82 - JapaneseDoublePostcardRotated
    [583, 413], //83 - A6Rotated
    [1307, 945], //84 - JapaneseEnvelopeKakuNumber2Rotated
    [1091, 850], //85 - JapaneseEnvelopeKakuNumber3Rotated
    [925, 472], //86 - JapaneseEnvelopeChouNumber3Rotated
    [807, 354], //87 - JapaneseEnvelopeChouNumber4Rotated
    [504, 717], //88 - B6Jis
    [717, 504], //89 - B6JisRotated 
    [1200, 1100], //90 - Standard12x11 
    [413, 925], //91 - JapaneseEnvelopeYouNumber4
    [925, 413], //92 - JapaneseEnvelopeYouNumber4Rotated 
    [0, 0], //93 - Prc16K
    [0, 0], //94 - Prc32K
    [0, 0], //95 - Prc32KBig 
    [1276, 1803], //96 - PrcEnvelopeNumber1
    [650, 402], //97 - PrcEnvelopeNumber2
    [492, 693], //98 - PrcEnvelopeNumber3
    [433, 819], //99 - PrcEnvelopeNumber4
    [433, 866], //100 - PrcEnvelopeNumber5
    [472, 906], //101 - PrcEnvelopeNumber6
    [630, 906], //102 - PrcEnvelopeNumber7
    [472, 1217], //103 - PrcEnvelopeNumber8
    [902, 1276], //104 - PrcEnvelopeNumber9
    [1276, 1803], //105 - PrcEnvelopeNumber10
    [0, 0], //106 - Prc16KRotated
    [0, 0], //107 - Prc32KRotated
    [0, 0], //108 - Prc32KBigRotated
    [650, 402], //109 - PrcEnvelopeNumber1Rotated
    [402, 650], //110 - PrcEnvelopeNumber2Rotated
    [693, 492], //111 - PrcEnvelopeNumber3Rotated
    [819, 433], //112 - PrcEnvelopeNumber4Rotated
    [866, 433], //113 - PrcEnvelopeNumber5Rotated
    [906, 472], //114 - PrcEnvelopeNumber6Rotated
    [906, 630], //115 - PrcEnvelopeNumber7Rotated
    [1217, 472], //116 - PrcEnvelopeNumber8Rotated
    [1276, 902], //117 - PrcEnvelopeNumber9Rotated
    [1803, 1276] //118 - PrcEnvelopeNumber10Rotated                            
];