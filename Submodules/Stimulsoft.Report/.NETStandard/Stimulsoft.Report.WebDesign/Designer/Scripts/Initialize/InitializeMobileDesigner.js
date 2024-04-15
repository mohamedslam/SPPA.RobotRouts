
StiMobileDesigner.prototype.InitializeDesigner = function () {
    var jsObject = this;

    this.options.mobileDesigner.pressedDown = function () {
        var options = jsObject.options;

        //Close Current Menu
        if (options.currentMenu != null) {
            if (options.menuPressed != options.currentMenu && options.currentMenu.parentButton != options.buttonPressed &&
                !options.horMenuPressed && options.horMenuPressed != options.currentMenu.parentMenu &&
                (!(options.imageListMenuPressed || options.dropDownListMenuPressed || options.colorDialogPressed))) {
                options.currentMenu.changeVisibleState(false);
            }
        }

        //Close Current HorMenu
        if (options.currentHorMenu != null) {
            if (options.horMenuPressed != options.currentHorMenu && options.currentHorMenu.parentButton != options.buttonPressed &&
                options.currentHorMenu.parentButton != options.menuItemPressed && (!(options.imageListMenuPressed || options.dropDownListMenuPressed || options.colorDialogPressed)))
                options.currentHorMenu.changeVisibleState(false);
        }

        //Close Current ImageList
        if (options.currentImageListMenu != null)
            if (options.imageListMenuPressed != options.currentImageListMenu && options.currentImageListMenu.parentButton != options.buttonPressed)
                options.currentImageListMenu.changeVisibleState(false);

        //Close Current DropDownList
        if (options.currentDropDownListMenu != null)
            if (options.dropDownListMenuPressed != options.currentDropDownListMenu && options.currentDropDownListMenu.parentButton != options.buttonPressed)
                options.currentDropDownListMenu.changeVisibleState(false);

        //Close Current ColorDialog
        if (options.currentColorDialog != null)
            if (options.colorDialogPressed != options.currentColorDialog && options.currentColorDialog.parentButton != options.buttonPressed)
                options.currentColorDialog.changeVisibleState(false);

        //Close Current DatePicker
        if (options.currentDatePicker != null)
            if (options.datePickerPressed != options.currentDatePicker && options.currentDatePicker.parentButton != options.buttonPressed)
                options.currentDatePicker.changeVisibleState(false);

        //Close Draw Component Mode
        if (((!options.buttonPressed && !options.pagePressed) || (options.buttonPressed && options.buttonPressed.groupName != "Components")) && !options.menuPressed && options.insertPanel)
            options.insertPanel.resetChoose();

        //Close Draw Component Mode
        if (((!options.buttonPressed && !options.pagePressed) || (options.buttonPressed && !options.buttonPressed.toolboxOwner)) && !options.menuItemPressed && options.toolbox) {
            options.toolbox.resetChoose();
        }

        //Close Properties Panel       
        if (!options.propertiesPanelPressed && options.propertiesPanel.fixedViewMode && !options.menuPressed && !options.dropDownListMenuPressed &&
            !options.imageListMenuPressed && !options.colorDialogPressed && !options.datePickerPressed && !options.formPressed && !options.disabledPanelPressed)
            options.propertiesPanel.changeVisibleState(false);

        //Hide Check Popup Panel
        if (options.checkPopupPanel) {
            options.checkPopupPanel.hide();
        }

        //Hide Check Preview Panel
        if (options.checkPreviewPanel) {
            options.checkPreviewPanel.hide();
        }

        //Dictionary Panel Pressed
        if (options.dictionaryPanel && !options.dictionaryPanelPressed) {
            options.dictionaryPanel.setFocused(false);
        }

        //Report Tree Panel Pressed
        if (options.reportTreePanel && !options.reportTreePanelPressed) {
            options.reportTreePanel.setFocused(false);
        }

        options.buttonPressed = false;
        options.menuItemPressed = false;
        options.menuPressed = false;
        options.horMenuPressed = false;
        options.formPressed = false;
        options.dropDownListMenuPressed = false;
        options.imageListMenuPressed = false;
        options.colorDialogPressed = false;
        options.pagePressed = false;
        options.fingerIsMoved = false;
        options.disabledPanelPressed = false;
        options.datePickerPressed = false;
        options.propertiesPanelPressed = false;
        options.dictionaryPanelPressed = false;
        options.reportTreePanelPressed = false;
        options.startInsertDataToElement = false;
    }

    this.options.mobileDesigner.onmousedown = function () {
        if (this.isTouchStartFlag) return;
        jsObject.options.isTouchClick = false;
        this.pressedDown();
        jsObject.options.designerIsFocused = true;
    }

    this.options.mobileDesigner.ontouchstart = function (event) {
        var this_ = this;
        this.isTouchStartFlag = true;
        jsObject.options.fingerStartPos = [event.touches[0].pageX, event.touches[0].pageY];
        clearTimeout(this.isTouchStartTimer);
        if (jsObject.options.buttonsTimer) {
            clearTimeout(jsObject.options.buttonsTimer[2]);
            jsObject.options.buttonsTimer[0].className = jsObject.options.buttonsTimer[1];
            jsObject.options.buttonsTimer = null;
        }
        jsObject.options.isTouchClick = true;
        this.pressedDown();
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    this.options.mobileDesigner.onmouseup = function (event) {
        if (this.isTouchEndFlag) return;
        this.ontouchend(event, true);
        clearTimeout(jsObject.options.scrollLeftTimer);
        clearTimeout(jsObject.options.scrollRightTimer);
    }

    this.options.mobileDesigner.ontouchend = function (event, mouseProcess) {
        var this_ = this;
        this.isTouchEndFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchEndTimer);
        jsObject.options.fingerIsMoved = false;
        this.isTouchEndTimer = setTimeout(function () {
            this_.isTouchEndFlag = false;
        }, 1000);
    }

    this.options.mobileDesigner.ontouchmove = function () {
        var minOffset = jsObject.options.touchMovingMinOffset;
        var startPos = jsObject.options.fingerStartPos;
        var currPos = [event.touches[0].pageX, event.touches[0].pageY];
        if (!jsObject.options.fingerIsMoved && (Math.abs(startPos[0] - currPos[0]) > minOffset || Math.abs(startPos[1] - currPos[1]) > minOffset)) {
            jsObject.options.fingerIsMoved = true;
        }
    }

    this.options.mobileDesigner.isVisible = function () {
        return (jsObject.options.mobileDesigner.offsetWidth > 0 || jsObject.options.mobileDesigner.offsetHeight > 0)
    }

    setTimeout(function () {
        jsObject.addEvent(window, 'blur', function (event) {
            jsObject.options.designerIsFocused = false;
            jsObject.options.CTRL_pressed = false;
            jsObject.options.ALT_pressed = false;
            jsObject.options.SHIFT_pressed = false;
        });

        jsObject.addEvent(window, 'focus', function (event) {
            jsObject.options.designerIsFocused = true;
        });
    }, 5000);

    this.addEvent(window, 'keyup', function (e) {
        if (jsObject.options.previewMode || !jsObject.options.mobileDesigner.isVisible()) return;

        if (e) {
            if (e.keyCode == 17 || ((jsObject.GetOSName() == "MacOS" || jsObject.options.macMode) && e.keyCode == 91)) {
                jsObject.options.CTRL_pressed = false;
                jsObject.options.ALT_pressed = false;
                jsObject.options.startMouseWheelDelta = null;
                jsObject.options.mouseWheelOldDelta = null;
            }
            if (e.keyCode == 16) {
                jsObject.options.SHIFT_pressed = false;
            }
            if (e.keyCode == 18) {
                jsObject.options.ALT_pressed = false;
            }
            if (event.keyCode == 13) {
                if (jsObject.options.currentForm && jsObject.options.currentForm.visible && jsObject.options.currentForm.name == "authForm") {
                    jsObject.options.currentForm.action();
                }
            }
        }
    });

    this.addEvent(window, 'keydown', function (e) {
        if (!jsObject.options.designerIsFocused || jsObject.options.previewMode || !jsObject.options.mobileDesigner.isVisible()) return;

        if (e) {
            //Arrows
            if (e.keyCode >= 37 || e.keyCode <= 40) {
                if (!jsObject.options.controlsIsFocused) {
                    if (jsObject.options.dictionaryPanel && jsObject.options.dictionaryPanel.isFocused && jsObject.options.dictionaryTree) {
                        jsObject.options.dictionaryTree.moveSelector(e.keyCode);
                    }
                    else if (jsObject.options.reportTreePanel && jsObject.options.reportTreePanel.isFocused && jsObject.options.reportTree) {
                        jsObject.options.reportTree.moveSelector(e.keyCode, true);
                    }
                }
            }
            //Ctrl
            if (e.keyCode == 17 || ((jsObject.GetOSName() == "MacOS" || jsObject.options.macMode) && e.keyCode == 91)) {
                jsObject.options.CTRL_pressed = true;
            }
            //Shift
            if (e.keyCode == 16) {
                jsObject.options.SHIFT_pressed = true;
            }
            //Alt
            if (e.keyCode == 18) {
                jsObject.options.ALT_pressed = true;
            }
            //Delete
            if (e.keyCode == 46) {
                if (!jsObject.options.controlsIsFocused && (!jsObject.options.currentForm || jsObject.options.currentForm.name == "mobileViewComponentsForm")) {
                    if (jsObject.options.dictionaryPanel && jsObject.options.dictionaryPanel.isFocused) {
                        if (jsObject.options.dictionaryPanel.toolBar.controls.DeleteItem.isEnabled) {
                            jsObject.DeleteItemDictionaryTree();
                        }
                    }
                    else if (jsObject.options.buttons.removeComponent.isEnabled) {
                        jsObject.ExecuteAction("removeComponent");
                    }
                }
                else if (jsObject.options.currentForm && jsObject.options.currentForm.name == "styleDesignerForm" && !jsObject.options.controlsIsFocused) {
                    var selectedItem = jsObject.options.currentForm.stylesTree.selectedItem;
                    if (selectedItem && selectedItem.itemObject.typeItem != "MainItem") {
                        selectedItem.remove();
                    }
                }
            }
            //Enter
            if (e.keyCode == 13) {
                if (jsObject.options.CTRL_pressed) {
                    if (jsObject.options.currentForm && jsObject.options.currentForm.visible) {
                        jsObject.options.currentForm.action();
                    }
                }
                else if (jsObject.options.currentForm && jsObject.options.currentForm.name == "messageForm" && jsObject.options.currentForm.visible) {
                    jsObject.options.currentForm.action(true);
                    jsObject.options.currentForm.changeVisibleState(false);
                }
            }
            //Esc
            if (e.keyCode == 27) {
                if (jsObject.options.currentForm && jsObject.options.currentForm.visible) {
                    var form = jsObject.options.currentForm;
                    if (jsObject.options.jsMode && form.name == "authForm") {
                        form.buttonClose.action();
                        return;
                    }
                    form.changeVisibleState(false);
                    if (form.cancelAction) form.cancelAction();
                    jsObject.options.mobileDesigner.pressedDown();
                }
                else if (jsObject.options.menus.fileMenu && jsObject.options.menus.fileMenu.visible) {
                    jsObject.options.menus.fileMenu.changeVisibleState(false);
                }
                else if (jsObject.options.componentButtonInDrag) {
                    jsObject.options.mainPanel.removeChild(jsObject.options.componentButtonInDrag);
                    jsObject.options.componentButtonInDrag = false;
                }
                else if (jsObject.options.in_drag) {
                    var components = jsObject.Is_array(jsObject.options.in_drag[0]) ? jsObject.options.in_drag[0] : [jsObject.options.in_drag[0]];
                    var childs = jsObject.Is_array(jsObject.options.in_drag[3]) ? jsObject.options.in_drag[3] : [jsObject.options.in_drag[3]];

                    for (var i = 0; i < components.length; i++) {
                        if (components[i].startPosLeft != null && components[i].startPosTop != null) {
                            components[i].setAttribute("left", components[i].startPosLeft);
                            components[i].setAttribute("top", components[i].startPosTop);
                            components[i].setAttribute("transform", "translate(" + components[i].startPosLeft + ", " + components[i].startPosTop + ")");
                            components[i].startPosLeft = null;
                            components[i].startPosTop = null;
                            var childComps = childs[i];
                            for (var name in childComps) {
                                var childComp = childComps[name];
                                childComp.setAttribute("left", childComp.startPosLeft);
                                childComp.setAttribute("top", childComp.startPosTop);
                                childComp.setAttribute("transform", "translate(" + childComp.startPosLeft + ", " + childComp.startPosTop + ")");
                                childComp.startPosLeft = null;
                                childComp.startPosTop = null;
                            }
                        }
                    }
                    jsObject.options.in_drag = false;
                }
            }
            //Ctrl+C
            if (e.keyCode == 67 && jsObject.options.CTRL_pressed) {
                if ((!jsObject.options.currentForm || !jsObject.options.currentForm.visible) && jsObject.options.buttons.copyComponent.isEnabled && !jsObject.options.controlsIsFocused && !jsObject.checkFocusedTextControls()) {
                    jsObject.ExecuteAction("copyComponent");
                }
                else if (jsObject.options.currentForm && jsObject.options.currentForm.name == "styleDesignerForm" && !jsObject.options.controlsIsFocused) {
                    var selectedItem = jsObject.options.currentForm.stylesTree.selectedItem;
                    if (selectedItem && selectedItem.itemObject.properties) {
                        jsObject.options.currentForm.copiedStyle = jsObject.CopyObject(selectedItem.itemObject);
                    }
                }
            }
            //Ctrl+V
            if (e.keyCode == 86 && jsObject.options.CTRL_pressed) {
                if ((!jsObject.options.currentForm || !jsObject.options.currentForm.visible) &&
                    !jsObject.options.controlsIsFocused && !jsObject.options.selectingRect && !jsObject.options.movingCloneComponents && !jsObject.options.drawComponent &&
                    !jsObject.options.cursorRect && !jsObject.options.startCopyWithCTRL && !jsObject.checkFocusedTextControls()) {
                    jsObject.PasteCurrentClipboardComponent();
                    jsObject.readTextFromClipboard(function (clipboardResult) {
                        if (clipboardResult)
                            jsObject.SendCommandGetFromClipboard(clipboardResult)
                        else
                            jsObject.ExecuteAction("pasteComponent");
                    });
                }
                else if (jsObject.options.currentForm && jsObject.options.currentForm.name == "styleDesignerForm" &&
                    !jsObject.options.controlsIsFocused && jsObject.options.currentForm.copiedStyle) {
                    var newName = jsObject.options.currentForm.copiedStyle.properties.name + "_" + jsObject.loc.Report.CopyOf;
                    jsObject.options.currentForm.stylesTree.addItem(jsObject.CopyObject(jsObject.options.currentForm.copiedStyle), newName);
                }
            }
            //Ctrl+X
            if (e.keyCode == 88 && jsObject.options.CTRL_pressed) {
                if ((!jsObject.options.currentForm || !jsObject.options.currentForm.visible) &&
                    jsObject.options.buttons.cutComponent.isEnabled && !jsObject.options.controlsIsFocused) {
                    jsObject.ExecuteAction("cutComponent");
                }
                else if (jsObject.options.currentForm && jsObject.options.currentForm.name == "styleDesignerForm" && !jsObject.options.controlsIsFocused) {
                    var selectedItem = jsObject.options.currentForm.stylesTree.selectedItem;
                    if (selectedItem && selectedItem.itemObject.properties) {
                        jsObject.options.currentForm.copiedStyle = jsObject.CopyObject(selectedItem.itemObject);
                        selectedItem.remove();
                    }
                }
            }
            //Ctrl+Z
            if (e.keyCode == 90 && jsObject.options.CTRL_pressed) {
                if ((!jsObject.options.currentForm || !jsObject.options.currentForm.visible) &&
                    jsObject.options.buttons.undoButton.isEnabled && !jsObject.options.controlsIsFocused) {
                    jsObject.ExecuteAction("undoButton");
                }
            }
            //Ctrl+S
            if (e.keyCode == 83 && jsObject.options.CTRL_pressed && !jsObject.options.ALT_pressed) {
                if (jsObject.options.report) {
                    jsObject.ActionSaveReport();
                    e.preventDefault();
                }
            }
            //Ctrl+A
            if (e.keyCode == 65 && jsObject.options.CTRL_pressed) {
                if ((!jsObject.options.currentForm || !jsObject.options.currentForm.visible) && !jsObject.options.controlsIsFocused && jsObject.options.currentPage && !jsObject.checkFocusedTextControls()) {
                    e.preventDefault();
                    jsObject.SelectAllComponentsOnPage(jsObject.options.currentPage);
                }
            }
            //Arrows
            if (e.keyCode >= 37 || e.keyCode <= 40) {
                if ((!jsObject.options.currentForm || !jsObject.options.currentForm.visible) && !jsObject.options.controlsIsFocused) {
                    // + Ctrl
                    if (jsObject.options.CTRL_pressed) {
                        jsObject.MoveComponentsByArrowButtons(e.keyCode);
                    }
                    // + Shift
                    else if (jsObject.options.SHIFT_pressed) {
                        jsObject.ResizeComponentsByArrowButtons(e.keyCode);
                    }
                    // + Alt
                    else if (jsObject.options.ALT_pressed) {
                        jsObject.MoveComponentsByArrowButtons(e.keyCode, true);
                    }
                    // Only Arrow
                    else if ((!jsObject.options.dictionaryPanel || !jsObject.options.dictionaryPanel.isFocused) && (!jsObject.options.reportTreePanel || !jsObject.options.reportTreePanel.isFocused)) {
                        jsObject.MoveSelectorByComponents(e.keyCode);
                    }
                }
            }
            //F5
            if (e.keyCode == 116) {
                if (jsObject.options.standaloneJsMode && !jsObject.options.controlsIsFocused && (!jsObject.options.currentForm || !jsObject.options.currentForm.visible)) {
                    jsObject.options.buttons.previewToolButton.action();
                    jsObject.options.buttons.previewToolButton.setSelected(true);
                }
            }
        }
    });

    this.addEvent(window, 'beforeunload', function (event) {
        if (jsObject.options.reportIsModified && !jsObject.options.scrollbiMode && !jsObject.options.ignoreBeforeUnload && !jsObject.options.closeDesignerWithoutAsking) {
            var msg = "Are you sure you want to leave this page without saving the report?";
            if (typeof event == "undefined") event = window.event;
            if (event) event.returnValue = msg;

            return msg;
        }
    });

    this.addEvent(window, 'resize', function (event) {
        if (jsObject.options.currentForm && !jsObject.options.currentForm.isNotModal && jsObject.options.currentForm.visible) {
            clearTimeout(jsObject.resizeTimer);
            jsObject.resizeTimer = setTimeout(function () {
                jsObject.SetObjectToCenter(jsObject.options.currentForm);
            }, 50);
        }
    });

    //messages from designer forms
    this.addEvent(window, 'message', function (args) {
        //var origin = jsObject.options.formsDesignerUrl;

        if (args.data/* && args.origin.slice(-origin.length) == origin*/) {
            try {
                jsObject.ProcessDesignerFormsEvents(JSON.parse(args.data));
            }
            catch (e) { }
        }
    });

    //Add Drag & Drop to Designer

    this.AddDragAndDropToContainer(jsObject.options.mainPanel, function (files, content) {
        var reportExts = ["mrt", "mrx", "mrz"];
        for (var i = 0; i < files.length; i++) {
            if (i == 1) return; //Temporarily

            var fileName = files[i].name;
            var fileSize = files[i].size;
            var fileExt = fileName.substring(fileName.lastIndexOf(".") + 1).toLowerCase();

            if (reportExts.indexOf(fileExt) >= 0) {
                var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
                if (fileMenu.visible) fileMenu.changeVisibleState(false);

                if (jsObject.options.cloudMode) {
                    if (jsObject.GetCloudPlanNumberValue() == 0) {
                        if (jsObject.options.cloudParameters && jsObject.options.cloudParameters.sessionKey) {
                            jsObject.InitializeNotificationForm(function (form) {
                                form.show(jsObject.NotificationMessages("openReportInTrial"), jsObject.NotificationMessages("upgradeYourPlan"), "Notifications.Blocked.png");
                            });
                        }
                        else {
                            jsObject.InitializeNotificationForm(function (form) {
                                form.show("Please login using your Stimulsoft account credentials or register a new account before opening report file.", null, "Notifications.Blocked.png");
                                form.upgradeButton.caption.innerHTML = jsObject.loc.Cloud.Login;
                                form.upgradeButton.action = function () {
                                    form.changeVisibleState(false);
                                    jsObject.options.forms.authForm.show();
                                }
                            });
                        }
                        return;
                    }
                    var maxFileSize = jsObject.GetCurrentPlanLimitValue("MaxFileSize");
                    if (maxFileSize && fileSize > maxFileSize) {
                        jsObject.InitializeNotificationForm(function (form) {
                            form.show(
                                jsObject.loc.Notices.QuotaMaximumFileSizeExceeded + "<br>" + jsObject.loc.PropertyMain.Maximum + ": " + jsObject.GetHumanFileSize(maxFileSize, true),
                                jsObject.NotificationMessages("upgradeYourPlan"),
                                "Notifications.Elements.png"
                            );
                        });
                        return;
                    }
                }

                jsObject.OpenReport(fileName, content, null, fileSize);

                if (jsObject.options.jsMode && jsObject.options.cloudParameters) {
                    jsObject.options.cloudParameters.reportTemplateItemKey = null;
                }
            }
            else {
                jsObject.AddResourceFile(files[i], content);
            }
        }
    });

    //Mouse wheel
    if ('onwheel' in document) {
        // IE9+, FF17+, Ch31+
        this.addEvent(jsObject.options.mainPanel, "wheel", function (e) { jsObject.onWheel(e); });
    } else if ('onmousewheel' in document) {
        // old event
        this.addEvent(jsObject.options.mainPanel, "mousewheel", function (e) { jsObject.onWheel(e); });
    } else {
        // Firefox < 17
        this.addEvent(jsObject.options.mainPanel, "MozMousePixelScroll", function (e) { jsObject.onWheel(e); });
    }

    var navigatorName = this.GetNavigatorName();
    var startZoom = 1;

    this.onWheel = function (e) {
        if (!jsObject.options.report) return;
        e = e || window.event;
        var delta = e.deltaY || e.detail || e.wheelDelta;
        var deltaStep = 0.1;

        if (jsObject.options.CTRL_pressed) {
            if (jsObject.options.startMouseWheelDelta == null ||
                jsObject.options.mouseWheelOldDelta == null ||
                (jsObject.options.mouseWheelOldDelta > 0 && delta < 0) ||
                (jsObject.options.mouseWheelOldDelta < 0 && delta > 0)) {
                jsObject.options.startMouseWheelDelta = 0;
                startZoom = jsObject.options.report.zoom;
            }

            if ((delta > 0 && navigatorName != "MSIE") || (navigatorName == "MSIE" && delta < 0))
                jsObject.options.startMouseWheelDelta += deltaStep;
            else
                jsObject.options.startMouseWheelDelta -= deltaStep;

            var newZoom = Math.round((startZoom - jsObject.options.startMouseWheelDelta) * 10) / 10;
            if (newZoom < 0.1) newZoom = 0.1;
            if (newZoom > 2) newZoom = 2;
            jsObject.options.report.zoom = newZoom;
            if (jsObject.options.currentPage) jsObject.PreZoomPage(jsObject.options.currentPage);

            e.preventDefault ? e.preventDefault() : (e.returnValue = false);
        }
        else {
            jsObject.options.startMouseWheelDelta = null;
            jsObject.options.mouseWheelOldDelta = null;
        }

        jsObject.options.mouseWheelOldDelta = delta;
    }
}