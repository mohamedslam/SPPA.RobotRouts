
StiJsViewer.prototype.InitializeJsViewer = function () {
    var jsObject = this.controls.viewer.jsObject = this;

    this.controls.viewer.pressedDown = function () {
        var options = jsObject.options;

        jsObject.removeBookmarksLabel();

        //Close Current Menu
        if (options.currentMenu != null) {
            if (options.menuPressed != options.currentMenu &&
                options.currentMenu.parentButton != options.buttonPressed &&
                !options.datePickerPressed &&
                !(options.dropDownListMenuPressed || options.colorDialogPressed) &&
                !options.horMenuPressed &&
                options.horMenuPressed != options.currentMenu.parentMenu)
                options.currentMenu.changeVisibleState(false);
        }

        //Close Current HorMenu
        if (options.currentHorMenu != null) {
            if (options.horMenuPressed != options.currentHorMenu &&
                options.currentHorMenu.parentButton != options.buttonPressed &&
                options.currentHorMenu.parentButton != options.menuItemPressed)
                options.currentHorMenu.changeVisibleState(false);
        }

        //Close Current DropDownList
        if (options.currentDropDownListMenu != null)
            if (options.dropDownListMenuPressed != options.currentDropDownListMenu && options.currentDropDownListMenu.parentButton != options.buttonPressed)
                options.currentDropDownListMenu.changeVisibleState(false);

        //Close Current DatePicker
        if (options.currentDatePicker != null)
            if (options.datePickerPressed != options.currentDatePicker && options.currentDatePicker.parentButton != options.buttonPressed)
                options.currentDatePicker.changeVisibleState(false);

        //Close Current ColorDialog
        if (options.currentColorDialog != null)
            if (options.colorDialogPressed != options.currentColorDialog && options.currentColorDialog.parentButton != options.buttonPressed)
                options.currentColorDialog.changeVisibleState(false);

        options.buttonPressed = false;
        options.menuItemPressed = false;
        options.menuPressed = false;
        options.horMenuPressed = false;
        options.formPressed = false;
        options.dropDownListMenuPressed = false;
        options.disabledPanelPressed = false;
        options.datePickerPressed = false;
        options.fingerIsMoved = false;
        options.colorDialogPressed = false;
    }

    this.controls.viewer.onmousedown = function () {
        if (this.isTouchStartFlag) return;
        jsObject.options.isTouchClick = false;
        this.pressedDown();
    }

    this.controls.viewer.ontouchstart = function (event) {
        var this_ = this;
        this.isTouchStartFlag = true;
        jsObject.options.fingerStartPos = [event.touches[0].pageX, event.touches[0].pageY];;
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

    this.controls.viewer.onmouseup = function () {
        if (this.isTouchEndFlag) return;
        this.ontouchend();
    }

    this.controls.viewer.ontouchend = function () {
        var this_ = this;
        this.isTouchEndFlag = true;
        clearTimeout(this.isTouchEndTimer);
        jsObject.options.fingerIsMoved = false;
        this.isTouchEndTimer = setTimeout(function () {
            this_.isTouchEndFlag = false;
        }, 1000);
    }

    this.controls.viewer.ontouchmove = function () {
        var minOffset = 15;
        var startPos = jsObject.options.fingerStartPos;
        var currPos = [event.touches[0].pageX, event.touches[0].pageY];
        if (!jsObject.options.fingerIsMoved && (Math.abs(startPos[0] - currPos[0]) > minOffset || Math.abs(startPos[1] - currPos[1]) > minOffset)) {
            jsObject.options.fingerIsMoved = true;
        }
    }

    this.addEvent(window, 'keypress', function (e) {
        if (e) {
            if (jsObject.options.currentMenu && jsObject.options.currentMenu.currentFindedIndex != null) {
                //Enter
                if (e.keyCode == 13) {
                    if (jsObject.options.currentMenu.findedItems[jsObject.options.currentMenu.currentFindedIndex].action != null) {
                        jsObject.options.currentMenu.findedItems[jsObject.options.currentMenu.currentFindedIndex].action();
                        e.stopPropagation();
                        e.cancelBubble = true;
                    }
                }
            }
        }
    });

    this.addEvent(window, 'keyup', function (e) {
        if (e) {
            //Arrows
            if (jsObject.options.currentMenu && jsObject.options.currentMenu.currentFindedIndex != null) {
                var currentMenu = jsObject.options.currentMenu;
                if (e.keyCode == 40 || e.keyCode == 38) {
                    var selectIndex = (currentMenu.currentFindedIndex == 0 &&
                        currentMenu.findedItems.length > 0 &&
                        !currentMenu.findedItems[currentMenu.currentFindedIndex].isSelected)
                        ? 0 : jsObject.options.currentMenu.currentFindedIndex + (e.keyCode == 40 ? 1 : -1);
                    jsObject.options.currentMenu.showFindedItem(selectIndex);
                }
            }
            //Enter
            if (e.keyCode == 13) {
                if (jsObject.controls.forms.errorMessageForm && jsObject.controls.forms.errorMessageForm.visible) {
                    jsObject.controls.forms.errorMessageForm.changeVisibleState(false);
                }
                else if (jsObject.controls.forms && jsObject.controls.forms.authForm && jsObject.controls.forms.authForm.visible) {
                    jsObject.controls.forms.authForm.action();
                }
            }
            ///Shift
            if (e.keyCode == 16) {
                jsObject.options.SHIFT_pressed = false;
            }
        }
    });

    this.addEvent(window, 'keydown', function (e) {
        if (e) {
            //Shift
            if (e.keyCode == 16) {
                jsObject.options.SHIFT_pressed = true;
            }
        }
    });
}