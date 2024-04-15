
StiMobileDesigner.prototype.PropertyInteractionControl = function (name, width) {
    var interactionControl = document.createElement("div");
    interactionControl.key = null;
    interactionControl.jsObject = this;

    var interactionButton = this.SmallButton(null, null, this.loc.PropertyMain.Interaction, null, null, null, "stiDesignerFormButton");
    interactionButton.style.margin = "8px";
    interactionButton.innerTable.style.width = "100%";
    interactionButton.caption.style.textAlign = "center";
    interactionControl.appendChild(interactionButton);

    interactionButton.action = function () {
        this.jsObject.InitializeInteractionForm(function (interactionForm) {

            interactionForm.action = function () {
                this.applyValues();
                this.changeVisibleState(false);
                interactionControl.setKey(this.interaction);
                interactionControl.action();
            }

            interactionForm.show(interactionControl.key);
        });
    }

    interactionControl.setKey = function (key) {
        this.key = key;
    }

    interactionControl.action = function () { }

    return interactionControl;
}