
StiMobileDesigner.prototype.InitializeWizardStepPanel = function (items) {
    var stepPanel = document.createElement("div");
    stepPanel.className = "wizardFormstepPanel";
    stepPanel.style.textAlign = "center";
    stepPanel.style.paddingTop = "10px";
    stepPanel.style.paddingBottom = "0px";

    stepPanel.name = name;
    stepPanel.jsObject = this;
    stepPanel.numbers = [];
    stepPanel.texts = [];
    stepPanel.ellipses = [];
    stepPanel.lines = [];
    stepPanel.pathOk = [];
    stepPanel.items = items;
    var svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
    svg.setAttribute("width", 100 * (items.length + 1));
    svg.setAttribute("height", 70);

    for (var i = 1; i <= items.length; i++) {
        var ellipse = document.createElementNS("http://www.w3.org/2000/svg", "circle");
        ellipse.setAttribute("cx", 100 * i);
        ellipse.setAttribute("cy", 15);
        ellipse.setAttribute("r", 15);
        ellipse.setAttribute("class", "wizardStepEllipse");
        svg.appendChild(ellipse);
        stepPanel.ellipses.push(ellipse);

        var number = document.createElementNS("http://www.w3.org/2000/svg", "text");
        number.setAttribute("x", 100 * i);
        number.setAttribute("y", 15);
        number.setAttribute("dy", "0.35em");
        number.setAttribute("fill", "DimGray");
        number.textContent = i;
        number.setAttribute("class", "wizardStepNumber");
        svg.appendChild(number);
        stepPanel.numbers.push(number);

        var pathOk = document.createElementNS("http://www.w3.org/2000/svg", "path");
        pathOk.setAttribute("fill", "transparent");
        pathOk.setAttribute("transform", "translate(" + (100 * i - 8) + " 8) scale(0.7 0.7)");
        pathOk.setAttribute("d", "M21.652,3.211c-0.293-0.295-0.77-0.295-1.061,0L9.41,14.34  c-0.293,0.297-0.771,0.297-1.062,0L3.449,9.351C3.304,9.203,3.114,9.13,2.923,9.129C2.73,9.128,2.534,9.201,2.387,9.351  l-2.165,1.946C0.078,11.445,0,11.63,0,11.823c0,0.194,0.078,0.397,0.223,0.544l4.94,5.184c0.292,0.296,0.771,0.776,1.062,1.07  l2.124,2.141c0.292,0.293,0.769,0.293,1.062,0l14.366-14.34c0.293-0.294,0.293-0.777,0-1.071L21.652,3.211z");
        svg.appendChild(pathOk);
        stepPanel.pathOk.push(pathOk);

        if (items[i - 1].length < 13) {
            var text = document.createElementNS("http://www.w3.org/2000/svg", "text");
            text.setAttribute("x", 100 * i);
            text.setAttribute("y", 45);
            text.setAttribute("dy", "0.35em");
            text.textContent = items[i - 1];
            text.setAttribute("class", "wizardStepText");
            svg.appendChild(text);
            stepPanel.texts.push([text]);
        } else {
            var spi = items[i - 1].indexOf(" ") > 0 ? items[i - 1].indexOf(" ") : Math.round(items[i - 1].length / 2);
            var text1 = items[i - 1].substr(0, spi);
            var text2 = items[i - 1].substr(spi, items[i - 1].length - spi);

            var text = document.createElementNS("http://www.w3.org/2000/svg", "text");
            text.setAttribute("x", 100 * i);
            text.setAttribute("y", 45);
            text.setAttribute("dy", "0.35em");
            text.textContent = text1;
            text.setAttribute("class", "wizardStepText");
            svg.appendChild(text);

            var vtext = document.createElementNS("http://www.w3.org/2000/svg", "text");
            vtext.setAttribute("x", 100 * i);
            vtext.setAttribute("y", 60);
            vtext.setAttribute("dy", "0.35em");
            vtext.textContent = text2;
            vtext.setAttribute("class", "wizardStepText");
            svg.appendChild(vtext);
            stepPanel.texts.push([text, vtext]);
        }
        if (i > 1) {
            var line = document.createElementNS("http://www.w3.org/2000/svg", "line");
            line.setAttribute("x1", 100 * i - 22);
            line.setAttribute("y1", 15);
            line.setAttribute("x2", 100 * i - 82);
            line.setAttribute("y2", 15);
            line.setAttribute("stroke-dasharray", "6 6");
            line.setAttribute("stroke-width", 4);
            line.setAttribute("class", "wizardStepLine");
            svg.appendChild(line);
            stepPanel.lines.push(line);
        }
    }

    stepPanel.svg = svg;
    stepPanel.appendChild(svg);

    stepPanel.setStep = function (step) {
        for (var i = 0; i < stepPanel.items.length; i++) {
            if (i < step) {
                stepPanel.numbers[i].setAttribute("fill", "transparent");
                for (var j in stepPanel.texts[i])
                    stepPanel.texts[i][j].setAttribute("class", "wizardStepTextChecked");
                stepPanel.ellipses[i].setAttribute("class", "wizardStepEllipseChecked");
                if (i > 0) stepPanel.lines[i - 1].setAttribute("class", "wizardStepLineCheked");
                stepPanel.pathOk[i].setAttribute("fill", "white");
            } else if (i == step) {
                stepPanel.numbers[i].setAttribute("fill", "White");
                for (var j in stepPanel.texts[i])
                    stepPanel.texts[i][j].setAttribute("class", "wizardStepText");
                stepPanel.ellipses[i].setAttribute("class", "wizardStepEllipse");
                if (i > 0) stepPanel.lines[i - 1].setAttribute("class", "wizardStepLine");
                stepPanel.pathOk[i].setAttribute("fill", "transparent");
            } else {
                stepPanel.numbers[i].setAttribute("fill", "#898989");
                for (var j in stepPanel.texts[i])
                    stepPanel.texts[i][j].setAttribute("class", "wizardStepTextInactive");
                stepPanel.ellipses[i].setAttribute("class", "wizardStepEllipseInactive");
                if (i > 0) stepPanel.lines[i - 1].setAttribute("class", "wizardStepLineInactive");
                stepPanel.pathOk[i].setAttribute("fill", "transparent");
            }
        }
    }
    stepPanel.onShow = function () { };
    stepPanel.onHide = function () { };


    stepPanel.setStep(0);
    return stepPanel;
}