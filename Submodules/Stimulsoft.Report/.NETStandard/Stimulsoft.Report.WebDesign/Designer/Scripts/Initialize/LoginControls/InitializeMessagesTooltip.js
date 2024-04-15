
StiMobileDesigner.prototype.InitMessagesTooltip = function () {
    var opt = this.options;

    this.addEvent(window, 'mouseup', function (e) {
        if (opt.messagesTooltip) {
            try {
                opt.messagesTooltip.close();
            } catch (e) { console.log(e); };
        }
    });

    this.addEvent(window, 'keydown', function (e) {
        if (opt.messagesTooltip) {
            try {
                opt.messagesTooltip.close();
            } catch (e) { console.log(e); };
        }
    });
}

StiMobileDesigner.prototype.ShowMessagesTooltip = function (msg, component, hideFunc) {
    if (this.options.messagesTooltip != null) {
        try {
            $(this.options.messagesTooltip).remove();
        } catch (e) { console.log(e); };
    }
    this.options.messagesTooltip = this.createMessageTooltip(msg, component, hideFunc, true, true);
    return this.options.messagesTooltip;
}

StiMobileDesigner.prototype.createMessageTooltip = function (msg, component, hideFunc, showCloseBtn, showImg) {
    var text = (msg && typeof (msg) != "string") ? msg.text : msg;
    var msgType = msg["type"] || "Error";
    var arrowSide = msg["arrowSide"] || "Left";
    var tooltipSide = msg["tooltipSide"] || arrowSide;
    var controller = {};

    var tooltip = $("<div class='stiMessagesTooltip sti" + msgType + "MessagesTooltip'></div>").appendTo((showCloseBtn && showImg) ? "body" : $(this.options.paintPanel)).css({ opacity: 0 });
    var innerTable = this.CreateHTMLTable();
    tooltip.append(innerTable);
    tooltip.append("<div class='stiMessagesTooltipArrow stiMessagesTooltipArrow" + arrowSide + " sti" + msgType + "MessagesTooltipArrow'></div>");
    tooltip.hideFunc = hideFunc;

    var img = $("<img style='width: 32px; height: 32px; margin: 10px;'></img>");
    StiMobileDesigner.setImageSource(img[0], this.options, "MsgForm" + msgType + ".png");
    var textPadding = !showCloseBtn && !showImg ? "5px 15px 7px 15px" : "0px 25px 10px 0px";
    var textContent = $("<div style='padding: " + textPadding + "; max-width: 300px;min-height:24px;pointer-events: none;'>" + text + "</div>")[0];
    var tTable = this.CreateHTMLTable();
    var closeBtn = $("<img style='width: 16px;height: 16px;' style='padding:2px 0 1px;'>");
    StiMobileDesigner.setImageSource(closeBtn[0], this.options, "LoginControls.Window.CloseWhite.png");
    closeBtn.mouseover(function () {
        closeBtn.css("background-color", "#ee8080");
    }).mousedown(function () {
        closeBtn.css("background-color", "#f1e1e1");
    }).mouseout(function () {
        closeBtn.css("background-color", "#ff0000");
    }).click(function () {
        tooltip.close();
    });
    if (showCloseBtn) {
        tTable.addCell(closeBtn[0], null, "height:17px;float:right");
    }
    tTable.addCellInNextRow(textContent);

    if (showImg) {
        innerTable.addCell(img[0]);
    }
    innerTable.addCell(tTable);

    //debugger;
    var xPos;
    if (tooltipSide == "Left") { xPos = $(component).offset().left + $(component).width() + 15; }
    else if (tooltipSide == "Right") { xPos = $(component).offset().left - tooltip.width() - 30; }
    else { xPos = $(component).offset().left + ($(component).width() / 2 - tooltip.width() / 2) + 2; }

    var yPos;
    if (showCloseBtn && showImg) {
        if (tooltipSide == "Bottom") { yPos = $(component).offset().top - tooltip.height() - 20; }
        else if (tooltipSide == "Top") { yPos = $(component).offset().top + $(component).height() + 20; }
        else { yPos = $(component).offset().top + ($(component).height() / 2 - tooltip.height() / 2) }
    } else {
        xPos = "-9999px";
        if (tooltipSide == "Bottom") { yPos = $(component).offset().top - tooltip.height() - 20; }
        else if (tooltipSide == "Top") { yPos = ($(component).position().top == 0 ? 10 : $(component).position().top) + $(component).height() + 10; }
        else { yPos = $(component).offset().top + ($(component).height() / 2 - tooltip.height() / 2) }
    }


    tooltip.css({ left: xPos, top: yPos - 40 }).animate({ top: yPos, opacity: 1 }, { duration: 120 });


    tooltip.close = function () {
        var hideFunc = tooltip.hideFunc;
        $.when(tooltip.fadeOut(200).remove()).then(function () {
            if (hideFunc) {
                hideFunc();
            }
        });
    }

    return tooltip;
}

