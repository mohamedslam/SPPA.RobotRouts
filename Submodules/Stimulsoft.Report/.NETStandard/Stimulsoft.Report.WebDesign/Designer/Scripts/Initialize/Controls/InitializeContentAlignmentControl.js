
StiMobileDesigner.prototype.ContentAlignmentControl = function (name) {
    var control = this.CreateHTMLTable();
    control.buttons = {};
    control.isEnabled = true;

    if (name) {
        this.options.controls[name] = control;
        control.name = name;
    }

    var buttonsProps = [
        ["Left", this.loc.HelpDesigner.AlignLeft, "AlignLeft.png", "hor"],
        ["Center", this.loc.HelpDesigner.AlignCenter, "AlignCenter.png", "hor"],
        ["Right", this.loc.HelpDesigner.AlignRight, "AlignRight.png", "hor"],
        ["Top", this.loc.HelpDesigner.AlignTop, "AlignTop.png", "vert"],
        ["Middle", this.loc.HelpDesigner.AlignMiddle, "AlignMiddle.png", "vert"],
        ["Bottom", this.loc.HelpDesigner.AlignBottom, "AlignBottom.png", "vert"]
    ];

    for (var i = 0; i < buttonsProps.length; i++) {
        var button = this.SmallImageButtonWithBorder(null, null, buttonsProps[i][2], null, buttonsProps[i][1]);
        button.propertyName = buttonsProps[i][0];
        button.group = buttonsProps[i][3];
        button.style.marginRight = button.propertyName == "Right" ? "25px" : "4px";

        control.addCell(button);
        control.buttons[button.propertyName] = button;

        button.action = function () {
            var vertKey = this.propertyName;
            var horKey = this.propertyName;

            for (var key in control.buttons) {
                var b = control.buttons[key];

                if (this.group == b.group) {
                    b.setSelected(false);
                }
                else if (b.group && b.isSelected) {
                    if (b.group == "vert")
                        vertKey = b.propertyName;
                    else
                        horKey = b.propertyName;
                }
            }

            this.setSelected(true);
            control.key = vertKey + horKey;
            control.action();
        }
    }

    control.setEnabled = function (state) {
        this.isEnabled = state;

        for (var key in control.buttons) {
            control.buttons[key].setEnabled(state);
        }
    }

    control.setKey = function (key) {
        this.key = key;
        this.buttons.Left.setSelected(key.indexOf("Left") >= 0);
        this.buttons.Center.setSelected(key.indexOf("Center") >= 0);
        this.buttons.Right.setSelected(key.indexOf("Right") >= 0);
        this.buttons.Top.setSelected(key.indexOf("Top") >= 0);
        this.buttons.Middle.setSelected(key.indexOf("Middle") >= 0);
        this.buttons.Bottom.setSelected(key.indexOf("Bottom") >= 0);
    }

    control.action = function () { }

    return control;
}