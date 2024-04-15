
StiMobileDesigner.prototype.OverrideTextbox = function () {

    this.OldTextBox = this.TextBox;

    this.TextBox = function (name, width, toolTip) {
        var textBox = this.OldTextBox(name, width, toolTip);

        textBox.checkEmpty = function (msg, func, showDelay, ignoreSpaces) {
            var value = ignoreSpaces ? this.value.trim() : this.value;

            if (value != null && value != "") {
                return true;
            } else {
                if (msg) {
                    if (func) {
                        func();
                    }
                    var this_ = this;
                    if (showDelay) setTimeout(function () { this_.jsObject.ShowMessagesTooltip(msg, this_); }, showDelay);
                    else this.jsObject.ShowMessagesTooltip(msg, this);
                }
                return false;
            }
        }

        textBox.checkLength = function (ln, msg, showDelay) {
            if (this.value != null && this.value.length >= ln) {
                return true;
            } else {
                if (msg) {
                    var this_ = this;
                    if (showDelay) setTimeout(function () { this_.jsObject.ShowMessagesTooltip(msg, this_); }, showDelay);
                    else this.jsObject.ShowMessagesTooltip(msg, this);
                }
                return false;
            }
        }

        textBox.checkEmail = function (msg, showDelay) {
            var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            var value = this.value.trim();
            var result = this.checkEmpty() && re.test(value);
            var this_ = this;
            if (!result && msg) {
                if (showDelay) setTimeout(function () { this_.jsObject.ShowMessagesTooltip(msg, this_); }, showDelay);
                else this.jsObject.ShowMessagesTooltip(msg, this);
            }
            return result;
        }

        return textBox;
    }
}