
StiMobileDesigner.prototype.InitializeEditCustomMapForm_ = function () {
    var editCustomMapForm = this.BaseForm("editCustomMapForm", this.loc.Map.MapEditorForm, 4);
    editCustomMapForm.resource = null;
    editCustomMapForm.controls = {};
    editCustomMapForm.modeIso = false;

    var items = [];
    items.push(this.Item("item0", "EnglishName", null, "EnglishName"));
    items.push(this.Item("item1", "ISOCode", null, "ISOCode"));
    var viewModeList = this.DropDownList(null, 130, null, items, true, false);
    viewModeList.menu.style.zIndex = 10000;
    viewModeList.style.marginLeft = "15px";
    viewModeList.setKey("EnglishName");
    viewModeList.action = function () {
        editCustomMapForm.modeIso = viewModeList.key == "ISOCode";

        for (var i in treeView.items) {
            var item = treeView.items[i];
            item.checkBox.setChecked(true);
            var rect = editCustomMapForm.modeIso ? item.path.RectIso.split(",") : item.path.Rect.split(",");
            rect = { x: parseFloat(rect[0]), y: parseFloat(rect[1]), width: parseFloat(rect[2]), height: parseFloat(rect[3]) };
            item.rect = rect;
            item.svgRect.setAttribute("x", rect.x);
            item.svgRect.setAttribute("y", rect.y);
            item.svgRect.setAttribute("width", rect.width);
            item.svgRect.setAttribute("height", rect.height);

            item.textCell.innerHTML = editCustomMapForm.modeIso ? editCustomMapForm.getISO(item.path.ISOCode) : item.path.EnglishName;

            editCustomMapForm.drawText(item);
        }
        if (treeView.items.length > 0)
            treeView.select(treeView.items[0].table);
    }


    var footerTable = this.CreateHTMLTable();
    footerTable.style.width = "100%";
    var buttonsPanel = editCustomMapForm.buttonsPanel;
    editCustomMapForm.removeChild(buttonsPanel);
    editCustomMapForm.appendChild(footerTable);
    footerTable.addCell(viewModeList);
    footerTable.addCell(editCustomMapForm.buttonOk).style.width = "1px";
    footerTable.addCell(editCustomMapForm.buttonCancel).style.width = "1px";

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "5px 0 5px 0";
    editCustomMapForm.container.style.borderTop = "none";
    editCustomMapForm.container.appendChild(innerTable);

    var treeView = document.createElement("div");
    treeView.className = "stiSimpleContainerWithBorder";
    treeView.style.margin = "5px 10px 10px 15px";
    treeView.style.paddingTop = "10px";
    treeView.style.width = "200px";
    treeView.style.height = Math.min(390, Math.max(window.innerHeight - 260, 290)) + "px";
    treeView.style.overflow = "auto";
    treeView.jsObject = this;
    innerTable.addCell(treeView);

    var mapView = document.createElement("div");
    mapView.className = "stiSimpleContainerWithBorder";
    mapView.style.margin = "5px 15px 10px 5px";
    mapView.style.width = Math.min(600, Math.max(window.innerWidth - 350, 470)) + "px";
    mapView.style.height = Math.min(400, Math.max(window.innerHeight - 250, 300)) + "px";
    mapView.style.overflow = "hidden";
    innerTable.addCell(mapView);

    var zoomPlusButton = this.SmallButton(null, null, null, "Dashboards.ZoomPlus.png", null, null, "stiDesignerFormButton");
    var zoomMinusButton = this.SmallButton(null, null, null, "Dashboards.ZoomMinus.png", null, null, "stiDesignerFormButton");
    editCustomMapForm.zoomButtons = [zoomPlusButton, zoomMinusButton];
    editCustomMapForm.zoomButtons.forEach(function (button) {
        mapView.appendChild(button);
        button.style.position = "absolute";
        button.style.zIndex = 10000;
    });

    zoomPlusButton.action = function () {
        editCustomMapForm.scale = Math.min(10, editCustomMapForm.scale / 0.7);
        editCustomMapForm.transform();
    }

    zoomMinusButton.action = function () {
        editCustomMapForm.scale = Math.max(0.01, editCustomMapForm.scale * 0.7);
        editCustomMapForm.transform();
    }

    innerTable.addTextCellInNextRow(this.loc.Chart.LabelAlignment.replace(":", "")).className = "stiDesignerCaptionControlsBigIntervals";

    var alignTable = this.CreateHTMLTable();
    innerTable.addCellInLastRow(alignTable);
    alignTable.style.marginBottom = "10px";
    alignTable.style.paddingLeft = "5px";

    var alignTopButton = this.StandartSmallButton("customMapEditorAlignTop", "customMapVerticalAlign", null, "AlignTop.png",
        [this.loc.HelpDesigner.AlignTop, this.HelpLinks["alignment"]], null);
    var alignMiddleButton = this.StandartSmallButton("customMapEditorAlignMiddle", "customMapVerticalAlign", null, "AlignMiddle.png",
        [this.loc.HelpDesigner.AlignMiddle, this.HelpLinks["alignment"]], null);
    var alignBottomButton = this.StandartSmallButton("customMapEditorAlignBottom", "customMapVerticalAlign", null, "AlignBottom.png",
        [this.loc.HelpDesigner.AlignBottom, this.HelpLinks["alignment"]], null);

    var alignLeftButton = this.StandartSmallButton("customMapEditorAlignLeft", "customMapHorizontalAlign", null, "AlignLeft.png",
        [this.loc.HelpDesigner.AlignLeft, this.HelpLinks["alignment"]], null);
    var alignCenterButton = this.StandartSmallButton("customMapEditorAlignCenter", "customMapHorizontalAlign", null, "AlignCenter.png",
        [this.loc.HelpDesigner.AlignCenter, this.HelpLinks["alignment"]], null);
    var alignRightButton = this.StandartSmallButton("customMapEditorAlignRight", "customMapHorizontalAlign", null, "AlignRight.png",
        [this.loc.HelpDesigner.AlignRight, this.HelpLinks["alignment"]], null);

    alignTopButton.action = function (e) {
        this.setSelected(true);
        if (editCustomMapForm.modeIso) treeView.selectedItem.data.path.VertAlignmentIso = "Top";
        else treeView.selectedItem.data.path.VertAlignment = "Top";
        editCustomMapForm.drawText(treeView.selectedItem.data);
    }

    alignMiddleButton.action = function (e) {
        this.setSelected(true);
        if (editCustomMapForm.modeIso) treeView.selectedItem.data.path.VertAlignmentIso = "Center";
        else treeView.selectedItem.data.path.VertAlignment = "Center";
        editCustomMapForm.drawText(treeView.selectedItem.data);
    }

    alignBottomButton.action = function (e) {
        this.setSelected(true);
        if (editCustomMapForm.modeIso) treeView.selectedItem.data.path.VertAlignmentIso = "Bottom";
        else treeView.selectedItem.data.path.VertAlignment = "Bottom";
        editCustomMapForm.drawText(treeView.selectedItem.data);
    }

    alignLeftButton.action = function (e) {
        this.setSelected(true);
        if (editCustomMapForm.modeIso) treeView.selectedItem.data.path.HorAlignmentIso = "Left";
        else treeView.selectedItem.data.path.HorAlignment = "Left";
        editCustomMapForm.drawText(treeView.selectedItem.data);
    }

    alignCenterButton.action = function (e) {
        this.setSelected(true);
        if (editCustomMapForm.modeIso) treeView.selectedItem.data.path.HorAlignmentIso = "Center";
        else treeView.selectedItem.data.path.HorAlignment = "Center";
        editCustomMapForm.drawText(treeView.selectedItem.data);
    }

    alignRightButton.action = function (e) {
        this.setSelected(true);
        if (editCustomMapForm.modeIso) treeView.selectedItem.data.path.HorAlignmentIso = "Right";
        else treeView.selectedItem.data.path.HorAlignment = "Right";
        editCustomMapForm.drawText(treeView.selectedItem.data);
    }

    alignTable.addCell(alignTopButton).style.padding = "0 1px 0 1px";
    alignTable.addCell(alignMiddleButton).style.padding = "0 1px 0 1px";
    alignTable.addCell(alignBottomButton).style.padding = "0 1px 0 1px";

    var cell = alignTable.addCell();
    cell.style.width = "10px";
    cell.style.borderRight = "1px dashed rgb(171, 171, 171)";
    cell = alignTable.addCell();
    cell.style.width = "10px";

    alignTable.addCell(alignLeftButton).style.padding = "0 1px 0 1px";
    alignTable.addCell(alignCenterButton).style.padding = "0 1px 0 1px";
    alignTable.addCell(alignRightButton).style.padding = "0 1px 0 1px";

    innerTable.addCellInNextRow();

    var table2 = this.CreateHTMLTable();
    innerTable.addCellInLastRow(table2);
    table2.style.paddingLeft = "5px";
    table2.style.marginBottom = "10px";
    var wordWrapButton = this.StandartSmallButton("customMapEditorWordWrap", null, null, "WordWrap.png",
        [this.loc.PropertyMain.WordWrap, this.HelpLinks["alignment"]], null);
    wordWrapButton.style.marginRight = "10px";

    var hideTextChechBox = this.CheckBox("hideText", "Hide Text");
    table2.addCell(wordWrapButton);
    table2.addCell(hideTextChechBox);
    hideTextChechBox.action = function () {
        if (editCustomMapForm.modeIso) treeView.selectedItem.data.path.SkipTextIso = this.isChecked;
        else treeView.selectedItem.data.path.SkipText = this.isChecked;
        editCustomMapForm.drawText(treeView.selectedItem.data);
    }

    wordWrapButton.action = function () {
        treeView.selectedItem.data.path.SetMaxWidth = !treeView.selectedItem.data.path.SetMaxWidth;
        this.setSelected(treeView.selectedItem.data.path.SetMaxWidth);
        editCustomMapForm.drawText(treeView.selectedItem.data);
    }

    innerTable.addTextCellInNextRow(this.loc.PropertyMain.Icon).className = "stiDesignerCaptionControlsBigIntervals";

    table2 = this.CreateHTMLTable();
    table2.style.height = "60px";
    table2.style.marginLeft = "5px";
    var img = document.createElement("img");
    img.jsObject = this;
    img.style.border = "1px dashed rgb(171, 171, 171)";
    img.style.width = "48px";
    img.style.height = "48px";
    img.style.boxSizing = "border-box";

    if (this.options.canOpenFiles) {
        img.onmouseenter = function () {
            img.style.border = "2px dashed rgb(171, 171, 171)";
        };
        img.onmouseleave = function () {
            img.style.border = "1px dashed rgb(171, 171, 171)";
        };

        img.onclick = function () {
            var filesMask = ".bmp,.gif,.jpeg,.jpg,.png,.ico";

            var openDialog = this.jsObject.InitializeOpenDialog("customMapEditorImageDialog", function (evt) {
                var file = evt.target.files[0];
                if (file) {
                    var reader = new FileReader();
                    reader.jsObject = this.jsObject;

                    reader.onload = (function (theFile) {
                        return function (e) {
                            img.src = e.target.result;
                            editCustomMapForm.resource.Icon = e.target.result.substr(e.target.result.indexOf("base64,") + 7);
                        };
                    })(file);
                    reader.readAsDataURL(file);
                }
            }, filesMask);
            openDialog.action();
        }
    }

    table2.addCell(img).style.width = "60px";
    table2.addTextCell("Keys (<b>Left, Top, Right, Bottom</b>)  change the position of the text<br><br><b>Shift</b> + keys (<b>Left, Top, Right, Bottom</b>) change the size of the text area").className = "stiDesignerCaptionControlsBigIntervals";
    innerTable.addCellInLastRow(table2);

    document.addEventListener("keydown", function (e) {
        var step = 5;
        if (editCustomMapForm.visible && editCustomMapForm.resource.Paths.length > 0) {
            var data = treeView.selectedItem.data;
            var changed = true;
            if (!e.shiftKey) {
                if (e.keyCode == 37) {
                    data.rect.x -= step;
                    data.svgRect.setAttribute("x", data.rect.x);
                } else if (e.keyCode == 39) {
                    data.rect.x += step;
                    data.svgRect.setAttribute("x", data.rect.x);
                } else if (e.keyCode == 38) {
                    data.rect.y -= step;
                    data.svgRect.setAttribute("y", data.rect.y);
                } else if (e.keyCode == 40) {
                    data.rect.y += step;
                    data.svgRect.setAttribute("y", data.rect.y);
                } else {
                    changed = false;
                }
            } else {
                if (e.keyCode == 37) {
                    data.rect.width -= step;
                    data.svgRect.setAttribute("width", data.rect.width);
                } else if (e.keyCode == 39) {
                    data.rect.width += step;
                    data.svgRect.setAttribute("width", data.rect.width);
                } else if (e.keyCode == 38) {
                    data.rect.height -= step;
                    data.svgRect.setAttribute("height", data.rect.height);
                } else if (e.keyCode == 40) {
                    data.rect.height += step;
                    data.svgRect.setAttribute("height", data.rect.height);
                } else {
                    changed = false;
                }
                data.rect.height = Math.max(data.rect.height, 0);
                data.rect.width = Math.max(data.rect.width, 0);
            }
            if (changed) {
                if (editCustomMapForm.modeIso) data.path.RectIso = data.rect.x + ", " + data.rect.y + ", " + data.rect.width + ", " + data.rect.height;
                else data.path.Rect = data.rect.x + ", " + data.rect.y + ", " + data.rect.width + ", " + data.rect.height;
                editCustomMapForm.drawText(data);
            }
        }
    });

    var items = [];
    items.push(this.Item("checkedAll", "Checked All", null, "checkedAll"));
    items.push(this.Item("uncheckedAll", "Unchecked All", null, "uncheckedAll"));

    var treeViewMemu = this.BaseContextMenu("customMapEditorContextMenu", "Up", items);
    treeViewMemu.style.zIndex = 60;
    treeViewMemu.action = function (menuItem) {
        this.changeVisibleState(false);
        for (var i in treeView.items) {
            var item = treeView.items[i];
            if ((item.checkBox.isChecked && menuItem.key == "uncheckedAll") ||
                (!item.checkBox.isChecked && menuItem.key == "checkedAll")) {
                item.checkBox.setChecked(menuItem.key == "checkedAll");
                item.checkBox.action();
            }
        }
    }

    treeView.oncontextmenu = function (event) {
        return false;
    }

    treeView.onmouseup = function (event) {
        if (event.button == 2) {
            event.stopPropagation();
            var point = this.jsObject.FindMousePosOnMainPanel(event);
            treeViewMemu.show(point.xPixels + 3, point.yPixels + 3, "Down", "Right");
        }
        return false;
    }

    var itemBaseStyle = "stiDesignerClearAllStyles stiDesignerTreeItemButton";

    treeView.select = function (table) {
        if (treeView.selectedItem) {
            treeView.selectedItem.className = itemBaseStyle + " stiDesignerTreeItemButtonDefault";
        }

        treeView.selectedItem = table;
        table.className = itemBaseStyle + " stiDesignerTreeItemButtonSelected";

        if (editCustomMapForm.modeIso) {
            if (table.data.path.VertAlignmentIso == "Top") {
                alignTopButton.setSelected(true);
            } else if (table.data.path.VertAlignmentIso == "Center") {
                alignMiddleButton.setSelected(true);
            } else {
                alignBottomButton.setSelected(true);
            }

            if (table.data.path.HorAlignmentIso == "Left") {
                alignLeftButton.setSelected(true);
            } else if (table.data.path.HorAlignmentIso == "Center") {
                alignCenterButton.setSelected(true);
            } else {
                alignRightButton.setSelected(true);
            }

            hideTextChechBox.setChecked(table.data.path.SkipTextIso);
        } else {
            if (table.data.path.VertAlignment == "Top") {
                alignTopButton.setSelected(true);
            } else if (table.data.path.VertAlignment == "Center") {
                alignMiddleButton.setSelected(true);
            } else {
                alignBottomButton.setSelected(true);
            }

            if (table.data.path.HorAlignment == "Left") {
                alignLeftButton.setSelected(true);
            } else if (table.data.path.HorAlignment == "Center") {
                alignCenterButton.setSelected(true);
            } else {
                alignRightButton.setSelected(true);
            }

            hideTextChechBox.setChecked(table.data.path.SkipText);
        }

        wordWrapButton.setSelected(table.data.path.SetMaxWidth);
    }

    editCustomMapForm.onshow = function () {

        editCustomMapForm.zoomButtons.forEach(function (button) {
            button.style.left = (treeView.offsetLeft + treeView.offsetWidth + mapView.offsetLeft + 15) + "px";
        });
        zoomPlusButton.style.top = (mapView.offsetHeight / 2) + "px";
        zoomMinusButton.style.top = (mapView.offsetHeight / 2 + 5 + zoomMinusButton.offsetHeight) + "px";

        treeView.innerHTML = "";
        if (mapView.svg) {
            mapView.removeChild(mapView.svg);
            delete mapView.svg;
        }

        viewModeList.setKey("EnglishName");
        editCustomMapForm.modeIso = false;

        var resource = editCustomMapForm.resource;

        if (resource.Icon && resource.Icon.length > 0) {
            img.src = "data:image/png;base64," + resource.Icon;
        } else {
            StiMobileDesigner.setImageSource(img, this.jsObject.options, "StiMap.png");
            resource.Icon = img.src.substr(img.src.indexOf("base64,") + 7);
        }

        var svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
        mapView.svg = svg;
        mapView.appendChild(svg);
        editCustomMapForm.gTranslate = document.createElementNS("http://www.w3.org/2000/svg", "g");
        svg.appendChild(editCustomMapForm.gTranslate);

        editCustomMapForm.gScale = document.createElementNS("http://www.w3.org/2000/svg", "g");
        editCustomMapForm.gTranslate.appendChild(editCustomMapForm.gScale);

        svg.setAttribute("width", mapView.offsetWidth);
        svg.setAttribute("height", mapView.offsetHeight);

        var isScaleX = mapView.offsetWidth / resource.Width > mapView.offsetHeight / resource.Height;
        editCustomMapForm.translateX = 0;
        editCustomMapForm.translateY = 0;
        editCustomMapForm.sScale = Math.min(mapView.offsetWidth / resource.Width, mapView.offsetHeight / resource.Height);
        editCustomMapForm.scale = 1;
        editCustomMapForm.clientX = 0;
        editCustomMapForm.clientY = 0;
        editCustomMapForm.isMouseDown = false;

        treeView.items = [];
        for (var i = 0; i < resource.Paths.length; i++) {
            var path = resource.Paths[i];
            var color = editCustomMapForm.colors[i % editCustomMapForm.colors.length];
            var color2 = editCustomMapForm.lightenDarkenColor(color, -50);

            var svgPath = document.createElementNS("http://www.w3.org/2000/svg", "path");
            svgPath.setAttribute("d", path.Data);
            svgPath.setAttribute("stroke", "white");
            svgPath.setAttribute("fill", color);
            editCustomMapForm.gScale.appendChild(svgPath);

            var rect = {};
            try {
                rect = path.Rect.split(",");
                rect = { x: parseFloat(rect[0]), y: parseFloat(rect[1]), width: parseFloat(rect[2]), height: parseFloat(rect[3]) };
            } catch (e) { }

            if (!rect.width || !rect.height) {
                rect = svgPath.getBBox();
                rect = { x: Math.round(rect.x), y: Math.round(rect.y), width: Math.round(rect.width), height: Math.round(rect.height) };
                path.Rect = rect.x + ", " + rect.y + ", " + rect.width + ", " + rect.height;
            }

            if (!path.VertAlignment) path.VertAlignment = "Center";
            if (!path.HorAlignment) path.HorAlignment = "Center";
            if (path.SkipText == undefined) path.SkipText = false;
            if (path.SetMaxWidth == undefined) path.SetMaxWidth = false;

            if (!path.RectIso || path.RectIso == "0, 0, 0, 0") {
                path.RectIso = path.Rect;
            }
            if (!path.VertAlignmentIso) path.VertAlignmentIso = path.VertAlignment;
            if (!path.HorAlignmentIso) path.HorAlignmentIso = path.HorAlignment;
            if (path.SkipTextIso == undefined) path.SkipTextIso = path.SkipText;

            var svgRect = document.createElementNS("http://www.w3.org/2000/svg", "rect");
            svgRect.setAttribute("x", rect.x);
            svgRect.setAttribute("y", rect.y);
            svgRect.setAttribute("width", rect.width);
            svgRect.setAttribute("height", rect.height);
            svgRect.setAttribute("stroke", color2);
            svgRect.setAttribute("stroke-width", 2);
            svgRect.setAttribute("fill", "none");

            var table = this.jsObject.CreateHTMLTable();
            table.className = itemBaseStyle + " stiDesignerTreeItemButtonDefault";
            table.style.margin = "3px 3px 8px 13px";
            table.style.padding = "2px";

            table.onmouseover = function (e) {
                e.currentTarget.data.svgRect.setAttribute("stroke-width", 5);
            }
            table.onmouseout = function (e) {
                e.currentTarget.data.svgRect.setAttribute("stroke-width", 2);
            }

            var checkBox = this.jsObject.CheckBox(null);
            checkBox.setChecked(true);
            table.addCell(checkBox);
            checkBox.action = function (e) {
                var visibility = this.isChecked ? "visible" : "hidden";
                this.data.svgRect.setAttribute("visibility", visibility);
                this.data.svgPath.setAttribute("visibility", visibility);
                this.data.svgText.setAttribute("visibility", this.isChecked && !this.data.path.SkipText ? "visible" : "hidden");
            }

            var textCell = table.addTextCell(path.EnglishName);
            textCell.className = "stiDesignerCaptionControlsBigIntervals";;
            textCell.style.padding = "0px";

            var colorDiv = document.createElement("div");
            colorDiv.style.backgroundColor = color;
            colorDiv.style.width = "6px";
            colorDiv.style.height = "6px";
            colorDiv.style.marginLeft = "5px";
            table.addCell(colorDiv);

            var data = {
                path: path,
                svgRect: svgRect,
                rect: rect,
                svgPath: svgPath,
                checkBox: checkBox,
                table: table,
                textCell: textCell
            }
            table.data = data;
            checkBox.data = data;

            treeView.appendChild(table);

            table.onmousedown = function (e) {
                treeView.select(this);
            }

            treeView.items.push(data);
        }

        for (var i = 0; i < treeView.items.length; i++) {
            editCustomMapForm.gScale.appendChild(treeView.items[i].svgRect);
            editCustomMapForm.drawText(treeView.items[i]);

            if (i == 0) {
                treeView.select(treeView.items[i].table);
            }
        }

        if (isScaleX) {
            editCustomMapForm.bbox = { width: mapView.offsetWidth, height: 0 };
        } else {
            editCustomMapForm.bbox = { width: 0, height: mapView.offsetHeight };
        }

        editCustomMapForm.transform();

        svg.onmousedown = function (e) {
            editCustomMapForm.clientX = editCustomMapForm.translateX - e.clientX;
            editCustomMapForm.clientY = editCustomMapForm.translateY - e.clientY;
            editCustomMapForm.isMouseDown = true;
        }

        svg.onmousemove = function (e) {
            if (editCustomMapForm.isMouseDown) {
                editCustomMapForm.translateX = e.clientX + editCustomMapForm.clientX;
                editCustomMapForm.translateY = e.clientY + editCustomMapForm.clientY;
                editCustomMapForm.transform();
            }
        }

        window.addEventListener('mouseup', function (e) {
            editCustomMapForm.isMouseDown = false;
        });


    }

    editCustomMapForm.drawText = function (data) {
        if (data.svgText) {
            if (data.svgText.parentNode) {
                data.svgText.parentNode.removeChild(data.svgText);
            }
            if (!data.path.SkipText) {
                delete data.svgText;
            }
        }
        if ((editCustomMapForm.modeIso && data.path.SkipTextIso) || (!editCustomMapForm.modeIso && data.path.SkipText)) {
            return;
        }

        var tempG = document.createElementNS("http://www.w3.org/2000/svg", "g");
        mapView.svg.appendChild(tempG);
        var svgText = document.createElementNS("http://www.w3.org/2000/svg", "text");
        svgText.setAttribute("font-family", "Calibri");
        svgText.setAttribute("font-size", "18px");
        svgText.setAttribute("font-weight", "bold");
        tempG.appendChild(svgText);
        data.svgText = svgText;

        var dataText = editCustomMapForm.modeIso ? editCustomMapForm.getISO(data.path.ISOCode) : data.path.EnglishName;
        var words = data.path.SetMaxWidth ? dataText.split(" ") : [dataText];
        var tspan = editCustomMapForm.createTSpan(words.length > 0 ? words[0] : "", svgText);

        var bounds = svgText.getBBox();
        var i = 1;
        while (i < words.length) {
            var tempText = tspan.textContent;
            tspan.textContent += " " + words[i];
            var bounds = svgText.getBBox();
            if (bounds.width >= data.rect.width) {
                tspan.textContent = tempText;
                bounds = svgText.getBBox();

                tspan = editCustomMapForm.createTSpan(words[i], svgText);
            }
            i++;
        }
        mapView.svg.removeChild(tempG);
        editCustomMapForm.gScale.appendChild(tempG);
        var x, y;
        if (editCustomMapForm.modeIso) {
            x = data.path.HorAlignmentIso == "Left" ? data.rect.x : (data.path.HorAlignmentIso == "Center" ? data.rect.x + data.rect.width / 2 - bounds.width / 2 : data.rect.x + data.rect.width - bounds.width);
            y = data.path.VertAlignmentIso == "Top" ? data.rect.y : (data.path.VertAlignmentIso == "Center" ? data.rect.y + data.rect.height / 2 - bounds.height / 2 : data.rect.y + data.rect.height - bounds.height);
        } else {
            x = data.path.HorAlignment == "Left" ? data.rect.x : (data.path.HorAlignment == "Center" ? data.rect.x + data.rect.width / 2 - bounds.width / 2 : data.rect.x + data.rect.width - bounds.width);
            y = data.path.VertAlignment == "Top" ? data.rect.y : (data.path.VertAlignment == "Center" ? data.rect.y + data.rect.height / 2 - bounds.height / 2 : data.rect.y + data.rect.height - bounds.height);
        }
        tempG.setAttribute("transform", "translate(" + x + "," + y + ")");
    }

    editCustomMapForm.getISO = function (iso) {
        return iso != null && iso.length > 0 && iso.indexOf("-") >= 0 ? iso.substring(iso.indexOf("-") + 1) : iso;
    }

    editCustomMapForm.createTSpan = function (text, svgText) {
        var tspan = document.createElementNS("http://www.w3.org/2000/svg", "tspan");
        tspan.setAttribute("x", "0");
        tspan.setAttribute("dy", "1.2em");
        tspan.setAttribute("fill", "black");
        tspan.setAttribute("stroke", "white");
        tspan.setAttribute("stroke-width", "0.2");
        tspan.textContent = text;
        svgText.appendChild(tspan);
        return tspan;
    }

    editCustomMapForm.transform = function () {
        var x = mapView.offsetWidth / 2 - editCustomMapForm.resource.Width / 2 * editCustomMapForm.sScale;
        var y = mapView.offsetHeight / 2 - editCustomMapForm.resource.Height / 2 * editCustomMapForm.sScale;
        editCustomMapForm.gTranslate.setAttribute("transform", "translate(" + (editCustomMapForm.translateX + x) + ", " +
            (editCustomMapForm.translateY + y) + ") scale(" + editCustomMapForm.sScale + ")");

        editCustomMapForm.gScale.setAttribute("transform", "translate(" + ((1 - editCustomMapForm.scale) * editCustomMapForm.resource.Width / 2) + ", " +
            ((1 - editCustomMapForm.scale) * editCustomMapForm.resource.Height / 2) + ") scale(" + editCustomMapForm.scale + ")");
    }

    editCustomMapForm.action = function () {
        var jsObject = this.jsObject;
        this.changeVisibleState(false);
        var resourceContainer = editCustomMapForm.resourceContainer;
        var content = StiBase64.encode(JSON.stringify(editCustomMapForm.resource));
        if (resourceContainer.loadedContent) {
            content = "data:text/plain;base64," + content;
            resourceContainer.setResource(content, resourceContainer.resourceType, resourceContainer.resourceName,
                resourceContainer.resourceSize, null, resourceContainer.haveContent);
            editCustomMapForm.repaintMaps();
        }
        else {
            jsObject.SendCommandSetResourceText(resourceContainer.resourceName, content, function (e) {
                editCustomMapForm.repaintMaps();
            });
            resourceContainer.setResource(null, resourceContainer.resourceType, resourceContainer.resourceName,
                resourceContainer.resourceSize, content, resourceContainer.haveContent);
        }
    }

    editCustomMapForm.repaintMaps = function () {
        var pages = this.jsObject.options.report.pages;
        var maps = [];
        var regionMaps = {};
        for (var i in pages) {
            var page = pages[i];

            for (var j in page.components) {
                var component = page.components[j];
                if (component.typeComponent == "StiMap") {
                    maps.push(component.properties.name);
                } else if (component.typeComponent == "StiRegionMapElement") {
                    regionMaps[component.properties.name] = component;
                }
            }
        }

        for (var map in maps) {
            this.jsObject.SendCommandUpdateMapData(maps[map], null, null, null);
        }

        for (var map in regionMaps) {
            this.jsObject.SendCommandToDesignerServer("UpdateRegionMapElement",
                {
                    componentName: map,
                    updateParameters: {}
                },
                function (answer) {
                    var mapElement = regionMaps[answer.elementProperties.name];
                    mapElement.properties.svgContent = answer.elementProperties.svgContent;
                    mapElement.repaint();
                });
        }
    }


    editCustomMapForm.lightenDarkenColor = function (col, amt) {
        var usePound = false;
        if (col[0] == "#") {
            col = col.slice(1);
            usePound = true;
        }
        var num = parseInt(col, 16);
        var r = (num >> 16) + amt;
        if (r > 255) r = 255;
        else if (r < 0) r = 0;
        var b = ((num >> 8) & 0x00FF) + amt;
        if (b > 255) b = 255;
        else if (b < 0) b = 0;
        var g = (num & 0x0000FF) + amt;
        if (g > 255) g = 255;
        else if (g < 0) g = 0;
        return (usePound ? "#" : "") + String("000000" + (g | (b << 8) | (r << 16)).toString(16)).slice(-6);
    }

    editCustomMapForm.colors = [
        "#EEE8AA", "#DA70D6",
        "#FF4500",
        "#FFA500",
        "#6B8E23",
        "#808000",
        "#000080",
        "#FFDEAD",
        "#FFE4B5",
        "#FFE4E1",
        "#191970",
        "#C71585",
        "#48D1CC",
        "#00FA9A",
        "#7B68EE",
        "#87CEFA",
        "#778899",
        "#B0C4DE",
        "#00FF00",
        "#32CD32",
        "#98FB98",
        "#800000",
        "#66CDAA",
        "#0000CD",
        "#BA55D3",
        "#9370DB",
        "#3CB371",
        "#FF00FF",
        "#AFEEEE",
        "#DB7093",
        "#708090",
        "#00FF7F",
        "#4682B4",
        "#D2B48C",
        "#008080",
        "#6A5ACD",
        "#D8BFD8",
        "#40E0D0",
        "#EE82EE",
        "#F5DEB3",
        "#FF6347",
        "#20B2AA",
        "#87CEEB",
        "#A0522D",
        "#FFDAB9",
        "#CD853F",
        "#FFC0CB",
        "#DDA0DD",
        "#B0E0E6",
        "#800080",
        "#C0C0C0",
        "#FF0000",
        "#4169E1",
        "#8B4513",
        "#FA8072",
        "#F4A460",
        "#2E8B57",
        "#BC8F8F",
        "#FFFF00",
        "#FFA07A",
        "#90EE90",
        "#8B0000",
        "#9932CC",
        "#FF8C00",
        "#556B2F",
        "#8B008B",
        "#BDB76B",
        "#006400",
        "#A9A9A9",
        "#B8860B",
        "#008B8B",
        "#00008B",
        "#00FFFF",
        "#DC143C",
        "#6495ED",
        "#FF7F50",
        "#D2691E",
        "#FAEBD7",
        "#00FFFF",
        "#7FFFD4",
        "#F5F5DC",
        "#FFE4C4",
        "#E9967A",
        "#000000",
        "#0000FF",
        "#8A2BE2",
        "#A52A2A",
        "#DEB887",
        "#5F9EA0",
        "#7FFF00",
        "#FFEBCD",
        "#8FBC8B",
        "#483D8B",
        "#2F4F4F",
        "#FF69B4",
        "#CD5C5C",
        "#4B0082",
        "#FFFFF0",
        "#F0E68C",
        "#FFFACD",
        "#ADD8E6",
        "#F08080",
        "#E0FFFF",
        "#FAFAD2",
        "#D3D3D3",
        "#7CFC00",
        "#FFB6C1",
        "#ADFF2F",
        "#808080",
        "#00CED1",
        "#9400D3",
        "#FF1493",
        "#00BFFF",
        "#696969",
        "#1E90FF",
        "#008000",
        "#B22222",
        "#228B22",
        "#FF00FF",
        "#FFD700",
        "#DAA520",
        "#9ACD32",
    ];

    return editCustomMapForm;
}