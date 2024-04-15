
StiMobileDesigner.prototype.MapIDControl = function (name, width) {
    var jsObject = this;
    var control = this.CreateHTMLTable();
    control.isEnabled = true;

    var button = control.button = this.SmallButton(null, null, "World ", "Maps.Small.World.png", null, null, "stiDesignerFormButton", true, { width: 32, height: 32 });
    if (width) button.style.width = width + "px";
    button.style.height = "35px";
    button.innerTable.style.width = "100%";
    button.imageCell.style.width = "1px";
    button.imageCell.style.padding = "0 6px 0 6px";
    control.addCell(button);

    var capBlock = document.createElement("div");
    capBlock.setAttribute("style", "text-overflow: ellipsis; overflow: hidden; white-space: nowrap;");
    if (width) capBlock.style.width = (width - (this.options.isTouchDevice ? 50 : 45)) + "px";
    button.caption.innerHTML = "";
    button.caption.appendChild(capBlock);

    var allItems = [jsObject.Item("World", "World", "World.png", "World")];
    allItems = allItems.concat(jsObject.GetEuropeMapsItems());
    allItems = allItems.concat(jsObject.GetNorthAmericaMapsItems());
    allItems = allItems.concat(jsObject.GetSouthAmericaMapsItems());
    allItems = allItems.concat(jsObject.GetAsiaMapsItems());
    allItems = allItems.concat(jsObject.GetOceaniaMapsItems());
    allItems = allItems.concat(jsObject.GetAfricaMapsItems());

    button.action = function () {
        jsObject.InitializeMapCategoriesForm(function (form) {
            form.show(null, control);
        });
    }

    control.action = function () { }

    control.setKey = function (key) {
        this.key = key;
        for (var i = 0; i < allItems.length; i++) {
            if (key == allItems[i].key) {
                //not custom
                StiMobileDesigner.setImageSource(button.image, jsObject.options, "Maps.Small." + allItems[i].imageName || "StiMap.png");
                capBlock.innerText = allItems[i].caption;
                capBlock.setAttribute("title", capBlock.innerText);
                return;
            }
        }
        //custom
        var customMapResources = jsObject.GetCustomMapResources();
        for (var i in customMapResources) {
            var res = customMapResources[i];
            if (res.name == key) {
                capBlock.innerText = key;
                capBlock.setAttribute("title", capBlock.innerText);
                if (res.icon) button.image.src = res.icon;
                else StiMobileDesigner.setImageSource(button.image, jsObject.options, "StiMap.png");
                return;
            }
        }
        capBlock.innerText = key;
        capBlock.setAttribute("title", capBlock.innerText);
        StiMobileDesigner.setImageSource(button.image, jsObject.options, "StiMap.png");
    }

    control.setEnabled = function (state) {
        control.isEnabled = state;
        button.setEnabled(state);
    }

    return control;
}