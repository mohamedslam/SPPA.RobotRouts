
StiJsViewer.prototype.SetEditableMode = function (state) {
    this.options.editableMode = state;
    if (this.controls.buttons.Editor) this.controls.buttons.Editor.setSelected(state);

    if (state)
        this.ShowAllEditableFields();
    else
        this.HideAllEditableFields();
}

StiJsViewer.prototype.ShowAllEditableFields = function () {
    this.options.editableFields = [];
    var pages = this.controls.reportPanel.pages;

    for (var i = 0; i < pages.length; i++) {
        var page = pages[i];
        var pageElements = page.getElementsByTagName('*');

        for (var k = 0; k < pageElements.length; k++) {
            var editableStrAttr = pageElements[k].getAttribute("editable");
            if (editableStrAttr) {
                var attrArray = editableStrAttr.split(";");
                var params = {};
                params.compIndex = attrArray[0];
                params.pageIndex = this.reportParams.viewMode == "SinglePage" ? this.reportParams.pageNumber : i.toString();
                params.type = attrArray[1];

                if (params.type == "CheckBox") {
                    this.ShowCheckBoxEditableField(pageElements[k], params, attrArray);
                }
                else if (params.type == "Text") {
                    this.ShowTextEditableField(pageElements[k], params);
                }
                else if (params.type == "RichText") {
                    this.ShowRichTextEditableField(pageElements[k], params);
                }
            }
        }
    }
}

StiJsViewer.prototype.HideAllEditableFields = function () {
    var editableFields = this.options.editableFields;
    if (this.options.currentEditableTextArea) this.options.currentEditableTextArea.onblur();

    for (var i = 0; i < editableFields.length; i++) {
        editableFields[i].className = editableFields[i].className.replace(" stiEditableField stiEditableFieldSelected", "");
        editableFields[i].onclick = null;
        editableFields[i].style.outline = "";
    }

    if (this.controls.reportPanel.pages) {
        for (var i = 0; i < this.controls.reportPanel.pages.length; i++) {
            this.InitializeInteractions(this.controls.reportPanel.pages[i]);
        }
    }
}

StiJsViewer.prototype.ShowCheckBoxEditableField = function (editableCell, params, attrArray) {
    if (!editableCell.sizes) {
        var imgElements = editableCell.getElementsByTagName('IMG');
        if (imgElements.length == 0) imgElements = editableCell.getElementsByTagName('SVG');
        if (imgElements.length == 0) imgElements = editableCell.getElementsByTagName('svg');
        var imgElement = (imgElements.length > 0) ? imgElements[0] : null;
        if (!imgElement) return;
        if (imgElement.offsetWidth) {
            editableCell.sizes = {
                inPixels: imgElement.offsetWidth > imgElement.offsetHeight ? imgElement.offsetHeight : imgElement.offsetWidth,
                widthStyle: imgElement.style.width,
                heightStyle: imgElement.style.height
            }
        } else {
            editableCell.sizes = {
                inPixels: imgElement.clientWidth > imgElement.clientHeight ? imgElement.clientHeight : imgElement.clientWidth,
                widthStyle: imgElement.clientWidth + "px",
                heightStyle: imgElement.clientHeight + "px"
            }
        }
    }

    if (this.getNavigatorName() != "Google Chrome") editableCell.style.outline = "1px solid gray";
    editableCell.style.textAlign = "center";
    editableCell.className += " stiEditableField stiEditableFieldSelected";

    var trueSvgImage = this.GetSvgCheckBox(attrArray[3], attrArray[5], this.StrToInt(attrArray[6]), attrArray[7], editableCell.sizes.inPixels);
    var falseSvgImage = this.GetSvgCheckBox(attrArray[4], attrArray[5], this.StrToInt(attrArray[6]), attrArray[7], editableCell.sizes.inPixels);
    params.falseImage = "<div style='width:" + editableCell.sizes.widthStyle + ";height:" + editableCell.sizes.heightStyle + ";'>" + trueSvgImage + "</div>";
    params.trueImage = "<div style='width:" + editableCell.sizes.widthStyle + ";height:" + editableCell.sizes.heightStyle + ";'>" + falseSvgImage + "</div>";
    params.checked = attrArray[2] == "true" || attrArray[2] == "True";
    editableCell.params = params;
    editableCell.jsObject = this;

    if (!editableCell.hasChanged) {
        editableCell.checked = params.checked;
        editableCell.innerHTML = params.checked ? params.trueImage : params.falseImage;
    }

    editableCell.onclick = function () {
        this.checked = !this.checked;
        this.innerHTML = this.checked ? params.trueImage : params.falseImage;
        this.hasChanged = true;
        this.jsObject.AddEditableParameters(this);
    }
    this.options.editableFields.push(editableCell);
}

StiJsViewer.prototype.ShowTextEditableField = function (editableCell, params) {
    var jsObject = editableCell.jsObject = this;
    editableCell.className += " stiEditableField stiEditableFieldSelected";
    editableCell.params = params;

    if (this.getNavigatorName() != "Google Chrome") {
        editableCell.style.outline = "1px solid gray";
    }

    editableCell.onclick = function () {
        if (this.editMode) return;
        if (jsObject.options.currentEditableTextArea) jsObject.options.currentEditableTextArea.onblur();
        this.editMode = true;

        var textArea = document.createElement("textarea");
        textArea.style.width = (this.offsetWidth - 5) + "px";
        textArea.style.height = (this.offsetHeight - 5) + "px";
        textArea.style.maxWidth = (this.offsetWidth - 5) + "px";
        textArea.style.maxHeight = (this.offsetHeight - 5) + "px";
        textArea.className = this.className.replace(" stiEditableField stiEditableFieldSelected", "") + " stiEditableTextArea";
        textArea.style.border = "0px";

        var textContainer = this;
        var includedInnerDiv = function (el) {
            return (el.firstChild && el.firstChild.nodeName && el.firstChild.nodeName.toLowerCase() == "div")
        }
        while (includedInnerDiv(textContainer)) {
            textContainer = textContainer.firstChild;
        }

        textArea.value = textContainer.innerHTML.replace(/<br>/g, "\n");
        this.appendChild(textArea);
        textArea.focus();
        jsObject.options.currentEditableTextArea = textArea;

        textArea.onblur = function () {
            if (editableCell.editMode) {
                editableCell.editMode = false;
                textContainer.innerHTML = this.value.replace(/\n/g, "<br>");

                if (this && this.parentNode) {
                    this.parentNode.removeChild(this);
                }

                jsObject.options.currentEditableTextArea = null;
                jsObject.AddEditableParameters(editableCell, this.value);
            }
        }

        textArea.getPrevTextField = function (index) {
            if (index > 0) {
                for (var i = index - 1; i >= 0; i--) {
                    var field = jsObject.options.editableFields[i];
                    if (field.params.type == "Text") {
                        return field;
                    }
                }
            }
            return null;
        }

        textArea.getNextTextField = function (index) {
            if (index < jsObject.options.editableFields.length - 1) {
                for (var i = index + 1; i < jsObject.options.editableFields.length; i++) {
                    var field = jsObject.options.editableFields[i];
                    if (field.params.type == "Text") {
                        return field;
                    }
                }
            }
            return null;
        }

        textArea.onkeydown = function (e) {
            if (e && e.keyCode == 9) {
                if (jsObject.options.editableFields) {
                    var index = jsObject.options.editableFields.indexOf(editableCell);
                    var field = jsObject.options.SHIFT_pressed ? textArea.getPrevTextField(index) : textArea.getNextTextField(index);
                    if (field) {
                        e.preventDefault();
                        field.onclick();
                    }
                }
            }
        }
    }

    this.options.editableFields.push(editableCell);
}

StiJsViewer.prototype.ShowRichTextEditableField = function (editableCell, params) {
    //TO DO
}

StiJsViewer.prototype.AddEditableParameters = function (editableCell, newText) {
    if (!this.reportParams.editableParameters) this.reportParams.editableParameters = {};
    var params = {};
    params.type = editableCell.params.type;
    if (params.type == "CheckBox") params.checked = editableCell.checked;
    if (params.type == "Text") params.text = newText != null ? newText : editableCell.innerHTML;

    if (!this.reportParams.editableParameters[editableCell.params.pageIndex]) this.reportParams.editableParameters[editableCell.params.pageIndex] = {};
    this.reportParams.editableParameters[editableCell.params.pageIndex][editableCell.params.compIndex] = params;
}

StiJsViewer.prototype.GetSvgCheckBox = function (style, contourColor, size, backColor, width) {
    var head = "<svg version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" x=\"0\" y=\"0\" width=\"" + width + "px\" height=\"" + width + "px\">";
    var path = "<path stroke=\"" + contourColor + "\" stroke-width=\"" + size + "\" fill=\"" + backColor +
        "\" stroke-linecap=\"round\" stroke-linejoin=\"round\" transform=\"scale(" + (1 / (200 / width)) + ")\" d=\"";

    var shape = "";
    switch (style) {
        case "Cross":
            shape = "m 62.567796,147.97593 c -0.55,-0.14223 -2.162828,-0.5128 -3.584062,-0.82348 -3.647667,-0.79738 -9.670499,-5.83775 -14.242817,-11.91949 l " +
                "-3.902341,-5.19058 5.080199,-1.13481 c 7.353071,-1.64253 13.640456,-5.71752 21.826811,-14.14646 l 7.208128,-7.42171 " +
                "-6.410736,-7.513354 c -11.773129,-13.79803 -14.346726,-23.01954 -8.627769,-30.91434 2.894109,-3.9952 11.818482,-12.369333 " +
                "13.182086,-12.369333 0.411356,0 1.063049,1.6875 1.448207,3.750003 0.980474,5.25038 6.456187,16.76587 10.936694,23 2.075266,2.8875 " +
                "3.991125,5.25 4.257464,5.25 0.266339,0 3.775242,-3.4875 7.797566,-7.75 16.397034,-17.37615 29.674184,-19.76481 38.280564,-6.88699 " +
                "4.15523,6.21753 4.18631,8.07093 0.14012,8.3552 -5.84833,0.41088 -17.16241,8.5342 -25.51465,18.319104 l -4.63153,5.42599 " +
                "4.87803,4.31529 c 6.55108,5.79533 18.8991,11.89272 25.84076,12.76002 3.0455,0.38051 5.53727,1.10582 5.53727,1.6118 0,2.7809 " +
                "-9.26611,14.41872 -13.03,16.36511 -7.96116,4.11687 -16.36991,0.71207 -32.764584,-13.26677 l -4.985957,-4.25125 -7.086791,8.97188 c " +
                "-3.897736,4.93454 -8.82141,10.1198 -10.9415,11.52281 -3.906121,2.58495 -8.86588,4.41339 -10.691162,3.94136 z";
            break;

        case "Check":
            shape = "M 60.972125,162.49704 C 51.172676,136.72254 43.561975,123.37669 35.370344,117.6027 l -4.45827,-3.14248 2.75159,-2.89559 c 3.875121,-4.07793 " +
                "10.034743,-7.49924 14.902472,-8.27747 3.859874,-0.61709 4.458306,-0.38024 8.535897,3.37835 2.660692,2.45254 6.265525,7.60856 9.167226,13.11196 " +
                "2.630218,4.98849 4.910542,9.06999 5.067388,9.06999 0.156846,0 2.31372,-3.0375 4.793052,-6.75 C 96.259164,91.956015 129.68299,58.786374 157.56485,41.281603 l " +
                "8.84913,-5.555656 2.2633,2.631238 2.26329,2.631237 -7.76266,6.294183 C 139.859,66.19023 108.01682,105.51363 89.042715,138.83563 c -6.680477,11.73214 " +
                "-7.172359,12.31296 -15.090788,17.81963 -4.501873,3.13071 -9.044031,6.30443 -10.093684,7.05271 -1.708923,1.21826 -2.010678,1.09165 -2.886118,-1.21093 z";
            break;

        case "CrossRectangle":
            shape = "m 24.152542,102.04237 0,-72.499996 74.5,0 74.499998,0 0,72.499996 0,72.5 -74.499998,0 -74.5,0 0,-72.5 z m 133.758188,0.25 -0.25819,-57.249996 " +
                "-58.999998,0 -59,0 -0.259695,55.999996 c -0.142833,30.8 -0.04446,56.5625 0.218615,57.25 0.375181,0.98048 13.207991,1.25 59.517885,1.25 l " +
                "59.039573,0 -0.25819,-57.25 z m -90.574091,43.18692 c -1.823747,-0.3912 -4.926397,-1.85716 -6.894778,-3.25768 -3.319254,-2.36169 -12.289319,-12.40741 " +
                "-12.289319,-13.76302 0,-0.32888 2.417494,-1.13897 5.372209,-1.80021 7.185193,-1.60797 13.747505,-5.93496 21.803114,-14.3763 l 6.675323,-6.99496 " +
                "-6.379078,-7.31436 C 64.931387,85.71231 61.643682,76.29465 65.471903,68.89169 67.054097,65.83207 78.56175,54.542374 80.098251,54.542374 c 0.45744,0 " +
                "1.146839,1.6875 1.531997,3.75 0.980474,5.250386 6.456187,16.765876 10.936694,22.999996 2.075266,2.8875 3.991125,5.25 4.257464,5.25 0.266339,0 " +
                "3.775244,-3.4875 7.797564,-7.75 16.39704,-17.376139 29.67419,-19.764806 38.28057,-6.88698 4.15523,6.21752 4.18631,8.07092 0.14012,8.35519 -5.82996,0.40959 " +
                "-18.23707,9.34942 -25.91566,18.67328 -3.90068,4.73647 -3.97203,4.95414 -2.2514,6.86861 3.19054,3.54997 13.7039,10.54321 18.97191,12.61967 2.83427,1.11716 " +
                "7.43737,2.33421 10.22912,2.70455 2.79175,0.37034 5.07591,0.9956 5.07591,1.38947 0,2.11419 -8.37504,13.20895 -11.6517,15.4355 -8.39423,5.70403 " +
                "-16.63203,2.77 -34.14289,-12.16054 l -4.985955,-4.25125 -7.086791,8.97188 c -9.722344,12.3085 -16.524852,16.55998 -23.948565,14.96754 z";
            break;

        case "CheckRectangle":
            shape = "m 19.915254,103.5 0,-72.5 71.942245,0 71.942241,0 6.55727,-4.11139 6.55726,-4.11139 1.96722,2.36139 c 1.08197,1.298765 1.98219,2.644166 2.00049,2.98978 " +
                "0.0183,0.345615 -2.44173,2.53784 -5.46673,4.87161 l -5.5,4.243219 0,69.378391 0,69.37839 -74.999991,0 -75.000005,0 0,-72.5 z m 133.999996,3.87756 c " +
                "0,-49.33933 -0.12953,-53.514947 -1.62169,-52.276568 -2.78014,2.307312 -15.68408,17.90053 -24.32871,29.399008 -10.4919,13.955575 -23.47926,33.53736 " +
                "-29.514025,44.5 -4.457326,8.09707 -5.134776,8.80812 -14.291256,15 -5.28667,3.575 -9.903486,6.62471 -10.259592,6.77712 -0.356107,0.15242 -1.912439,-2.99758 " +
                "-3.458515,-7 -1.546077,-4.00241 -5.258394,-12.41205 -8.249593,-18.68809 -4.285436,-8.99155 -6.676569,-12.64898 -11.27758,-17.25 C 47.70282,104.62757 " +
                "44.364254,102 43.495254,102 c -2.798369,0 -1.704872,-1.66044 3.983717,-6.049158 5.593548,-4.31539 13.183139,-7.091307 16.801313,-6.145133 3.559412,0.930807 " +
                "9.408491,8.154973 13.919775,17.192241 l 4.46286,8.94025 4.54378,-6.83321 C 95.518219,96.605618 108.21371,81.688517 125.80695,63.75 L 143.21531,46 l " +
                "-53.650021,0 -53.650035,0 0,57.5 0,57.5 59.000005,0 58.999991,0 0,-53.62244 z";
            break;

        case "CrossCircle":
            shape = "M 83.347458,173.13597 C 61.069754,168.04956 42.193415,152.8724 32.202285,132.01368 23.4014,113.63986 23.679644,89.965903 32.91889,71.042373 " +
                "41.881579,52.685283 60.867647,37.139882 80.847458,31.799452 c 10.235111,-2.735756 31.264662,-2.427393 40.964762,0.600679 26.18668,8.174684 " +
                "46.06876,28.926852 51.62012,53.879155 2.43666,10.952327 1.56754,28.058524 -1.98036,38.977594 -6.65679,20.48707 -25.64801,38.95163 -47.32647,46.01402 " +
                "-6.3909,2.08202 -10.18566,2.59644 -21.27805,2.88446 -9.033911,0.23456 -15.484931,-0.10267 -19.500002,-1.01939 z M 112.4138,158.45825 c 17.13137,-3.13002 " +
                "33.71724,-15.96081 41.41353,-32.03742 14.8975,-31.119027 -1.10807,-67.659584 -34.40232,-78.540141 -6.71328,-2.193899 -9.93541,-2.643501 " +
                "-19.07755,-2.661999 -9.354252,-0.01893 -12.16228,0.37753 -18.768532,2.649866 -17.155451,5.900919 -29.669426,17.531424 -36.438658,33.866137 " +
                "-2.152301,5.193678 -2.694658,8.35455 -3.070923,17.89744 -0.518057,13.139047 0.741843,19.201887 6.111644,29.410237 4.106815,7.80733 15.431893,19.09359 " +
                "23.36818,23.28808 12.061362,6.37467 27.138828,8.6356 40.864629,6.1278 z M 69.097458,133.41654 c -2.8875,-2.75881 -5.25,-5.35869 -5.25,-5.77751 " +
                "0,-0.41882 5.658529,-6.30954 12.57451,-13.0905 l 12.57451,-12.329 L 76.198053,89.392633 63.399628,76.565738 68.335951,71.554056 c 2.714978,-2.756426 " +
                "5.304859,-5.011683 5.75529,-5.011683 0.450432,0 6.574351,5.611554 13.608709,12.470121 l 12.78974,12.470119 4.42889,-4.553471 c 2.43588,-2.50441 " +
                "8.39186,-8.187924 13.23551,-12.630032 l 8.80663,-8.076559 5.34744,5.281006 5.34743,5.281007 -12.96155,12.557899 -12.96154,12.557897 13.13318,13.16027 " +
                "13.13319,13.16027 -5.18386,4.66074 c -2.85112,2.5634 -5.70472,4.66073 -6.34134,4.66073 -0.63661,0 -6.5434,-5.4 -13.12621,-12 -6.58281,-6.6 -12.3871,-12 " +
                "-12.89844,-12 -0.511329,0 -6.593363,5.60029 -13.515627,12.44509 l -12.585935,12.44508 -5.25,-5.016 z";
            break;

        case "DotCircle":
            shape = "M 81.652542,170.5936 C 59.374838,165.50719 40.498499,150.33003 30.507369,129.47131 21.706484,111.09749 21.984728,87.42353 31.223974,68.5 " +
                "40.186663,50.14291 59.172731,34.597509 79.152542,29.257079 89.387653,26.521323 110.4172,26.829686 120.1173,29.857758 c 26.18668,8.174684 " +
                "46.06876,28.926852 51.62012,53.879152 2.43666,10.95233 1.56754,28.05853 -1.98036,38.9776 -6.65679,20.48707 -25.64801,38.95163 -47.32647,46.01402 " +
                "-6.3909,2.08202 -10.18566,2.59644 -21.27805,2.88446 -9.033907,0.23456 -15.484927,-0.10267 -19.499998,-1.01939 z m 29.999998,-15.098 c 20.68862,-4.34363 " +
                "38.01874,-20.45437 44.09844,-40.9956 2.36228,-7.9813 2.36228,-22.0187 0,-30 C 150.08927,65.371023 134.63549,50.297336 114.65254,44.412396 " +
                "106.5531,42.027127 90.741304,42.026386 82.695253,44.4109 62.460276,50.407701 46.686742,66.039241 41.6053,85.13096 c -1.948821,7.32201 -1.86506,23.11641 " +
                "0.158766,29.93754 8.730326,29.42481 38.97193,46.91812 69.888474,40.4271 z M 90.004747,122.6703 C 76.550209,117.63801 69.825047,101.82445 " +
                "75.898143,89.5 c 2.136718,-4.33615 7.147144,-9.356192 11.754399,-11.776953 5.578622,-2.931141 16.413098,-2.927504 22.052908,0.0074 18.03,9.382663 " +
                "19.07573,32.784373 1.91442,42.841563 -5.57282,3.26589 -15.830952,4.2617 -21.615123,2.09829 z";
            break;

        case "DotRectangle":
            shape = "m 23.847458,101.19491 0,-72.499995 74.5,0 74.499992,0 0,72.499995 0,72.5 -74.499992,0 -74.5,0 0,-72.5 z m 133.999992,-0.008 0,-57.507925 " +
                "-59.249992,0.25793 -59.25,0.25793 -0.25819,57.249995 -0.258189,57.25 59.508189,0 59.508182,0 0,-57.50793 z m -94.320573,33.85402 c -0.37368,-0.37368 " +
                "-0.679419,-15.67942 -0.679419,-34.01275 l 0,-33.333335 35.513302,0 35.51329,0 -0.2633,33.749995 -0.2633,33.75 -34.570573,0.26275 c -19.013819,0.14452 " +
                "-34.876319,-0.043 -35.25,-0.41666 z";
            break;

        case "NoneCircle":
            shape = "M 83.5,170.5936 C 61.222296,165.50719 42.345957,150.33003 32.354827,129.47131 23.553942,111.09749 23.832186,87.423523 33.071432,68.5 " +
                "42.034121,50.14291 61.020189,34.597509 81,29.257079 c 10.235111,-2.735756 31.26466,-2.427393 40.96476,0.600679 26.18668,8.174684 46.06876,28.926852 " +
                "51.62012,53.879155 2.43666,10.95232 1.56754,28.058527 -1.98036,38.977597 -6.65679,20.48707 -25.64801,38.95163 -47.32647,46.01402 -6.3909,2.08202 " +
                "-10.18566,2.59644 -21.27805,2.88446 -9.033909,0.23456 -15.484929,-0.10267 -19.5,-1.01939 z m 30,-15.098 c 20.68862,-4.34363 38.01874,-20.45437 " +
                "44.09844,-40.9956 2.36228,-7.9813 2.36228,-22.018707 0,-29.999997 C 151.93673,65.371023 136.48295,50.297336 116.5,44.412396 108.40056,42.027127 " +
                "92.588762,42.026386 84.542711,44.410896 64.307734,50.407697 48.5342,66.039237 43.452758,85.130959 c -1.948821,7.322 -1.86506,23.116411 " +
                "0.158766,29.937541 8.730326,29.42481 38.97193,46.91812 69.888476,40.4271 z";
            break;

        case "NoneRectangle":
            shape = "m 24.152542,102.04237 0,-72.499997 74.5,0 74.500008,0 0,72.499997 0,72.5 -74.500008,0 -74.5,0 0,-72.5 z m 133.758198,0.25 " +
                "-0.25819,-57.249997 -59.000008,0 -59,0 -0.259695,55.999997 c -0.142833,30.8 -0.04446,56.5625 0.218615,57.25 0.375181,0.98048 " +
                "13.207991,1.25 59.517885,1.25 l 59.039583,0 -0.25819,-57.25 z";
            break;
    }

    return head + path + shape + "\" /></svg>";
}
