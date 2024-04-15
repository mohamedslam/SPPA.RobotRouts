
StiMobileDesigner.prototype.InitializeTableOfContentsForm_ = function () {
    var jsObject = this;
    var form = this.BaseFormPanel("tableOfContentsForm", this.loc.Components.StiTableOfContents, 1);
    form.controls = {};
    this.AddProgressToControl(form);

    var tree = this.TableOfContentTree(form);
    form.container.appendChild(tree);

    var description = document.createElement("div");
    description.className = "stiCreateDataHintText";
    description.style.width = "340px";
    description.style.textAlign = "left";
    description.style.fontSize = "10px";
    description.style.padding = "0 12px 12px 12px";
    description.style.lineHeight = "1.1";
    description.innerHTML = this.loc.Report.LabelTableOfContentsHint;
    form.container.appendChild(description);

    var expTable = this.CreateHTMLTable();
    var textCell = expTable.addTextCell(this.loc.PropertyMain.Expression);
    textCell.className = "stiDesignerCaptionControls";
    textCell.style.padding = "12px 12px 6px 12px";

    var expression = this.TableOfContentsExpressionControl(null, 370);
    expression.style.margin = "0 0 12px 12px";
    expTable.addCellInNextRow(expression);
    form.container.appendChild(expTable);
    form.controls.expression = expression;

    expression.menu = this.VerticalMenu("tableOfContentsExpMenu", expression.dropDownButton, "Down", []);

    expression.menu.action = function (menuItem) {
        this.changeVisibleState(false);
        expression.textBox.value = menuItem.key;
        expression.action();
    }

    expression.dropDownButton.action = function () {
        expression.menu.changeVisibleState(!expression.menu.visible);
    }

    expression.action = function () {
        if (tree.selectedItem) {
            var itemObject = tree.selectedItem.itemObject;
            tree.selectedItem.pointerValue = this.textBox.value;
            tree.addChangedValue(itemObject.typeComponent, itemObject.properties.name || "", form.controls.expression.textBox.value);
        }
    }

    form.updateComponentsPointerValues = function () {
        for (var key in tree.changedComponents) {
            var componentParams = tree.changedComponents[key];
            if (componentParams.componentType == "StiReport") {
                form.currentComponent.properties.reportPointer = componentParams.pointerValue;
            }
            else {
                var component = jsObject.options.report.getComponentByName(componentParams.componentName);
                if (component != null) component.properties.pointerValue = componentParams.pointerValue;
            }
        }
    }

    form.show = function () {
        this.changeVisibleState(true);
        this.currentComponent = jsObject.options.selectedObject;

        if (this.currentComponent) {
            expression.setEnabled(false);
            expression.textBox.value = "";
            tree.build();
        }
        else {
            this.changeVisibleState(false);
        }
    }

    form.action = function () {
        this.changeVisibleState(false);
        if (jsObject.GetCountObjects(tree.changedComponents) > 0) {
            jsObject.SendCommandToDesignerServer("UpdateComponentsPointerValues", { changedComponents: tree.changedComponents, tableOfContentsName: form.currentComponent.properties.name }, function (answer) {
                form.updateComponentsPointerValues();
                form.currentComponent.repaint();
            });
        }
    }

    return form;
}

StiMobileDesigner.prototype.TableOfContentTree = function (form) {
    var jsObject = this;
    var tree = this.Tree();
    tree.className = "stiSimpleContainerWithBorder";
    tree.style.overflow = "auto";
    tree.style.margin = "12px";
    tree.style.padding = "12px";
    tree.style.width = "350px";
    tree.style.height = "370px";

    tree.build = function () {
        this.clear();
        this.componentsNames = [];
        this.changedComponents = {};

        if (jsObject.options.report) {
            form.progress.show();

            var reportName = jsObject.ExtractBase64Value(jsObject.options.report.properties.reportName);
            var reportAlias = jsObject.ExtractBase64Value(jsObject.options.report.properties.reportAlias);
            var reportCaption = (!reportAlias || reportName == reportAlias) ? reportName : reportName + " [" + reportAlias + "]";

            tree.reportItem = jsObject.TableOfContentTreeItem(reportCaption, "SmallComponents.StiReport.png", jsObject.options.report, tree, "reportItem");
            tree.reportItem.setChecked(form.currentComponent.properties.reportPointer);
            tree.reportItem.setOpening(true);
            tree.addPages(tree.reportItem, jsObject.options.report.pages);
            tree.reportItem.pointerValue = form.currentComponent.properties.reportPointer ? StiBase64.decode(form.currentComponent.properties.reportPointer) : "";

            var idents = [];
            if (reportAlias) idents.push(reportAlias);
            if (reportName) idents.push(reportName);
            tree.reportItem.idents = idents;

            jsObject.SendCommandToDesignerServer("GetTableOfContentsIdents", { tableOfContentsName: form.currentComponent.properties.name, componentsNames: tree.componentsNames }, function (answer) {
                form.progress.hide();
                tree.componentsIdents = answer.idents;
                tree.appendChild(tree.reportItem);
            });
        }
    }

    tree.addPages = function (parentItem, pages) {
        var pagesByIndexes = [];
        for (var pageName in pages) {
            var page = pages[pageName];
            pagesByIndexes[parseInt(page.properties.pageIndex)] = page;
        }
        for (var i = 0; i < pagesByIndexes.length; i++) {
            var page = pagesByIndexes[i];
            if (!page) continue;
            var pageName = page.properties.name;
            var pageAlias = StiBase64.decode(page.properties.aliasName);
            var captionText = (!pageAlias || pageName == pageAlias) ? pageName : pageName + " [" + pageAlias + "]";
            if (this.jsObject.options.useAliases && this.jsObject.options.showOnlyAliasForPages && pageAlias) captionText = pageAlias;

            var pageItem = jsObject.TableOfContentTreeItem(captionText, "SmallComponents." + (page.isDashboard ? "StiDashboard.png" : "StiPage.png"), page, tree, pageName);
            pageItem.setChecked(page.properties.pointerValue);
            pageItem.setOpening(true);
            parentItem.addChild(pageItem);
            pageItem.pointerValue = page.properties.pointerValue ? StiBase64.decode(page.properties.pointerValue) : "";

            var idents = [];
            if (pageName) idents.push(pageName);
            if (pageAlias) idents.push(pageAlias);
            parentItem.idents = idents;

            var pageChilds = {};
            for (var compName in page.components) {
                if (page.components[compName].properties.parentName == pageName)
                    pageChilds[compName] = page.components[compName];
            }
            tree.addComponents(pageItem, pageChilds);
        }
    }

    tree.addComponents = function (parentItem, components, lastLevel) {
        var componentsArray = [];
        for (var componentName in components) {
            componentsArray.push(components[componentName]);
        }
        componentsArray.sort(jsObject.SortByLeft);
        for (var i = 0; i < componentsArray.length; i++) {
            var component = componentsArray[i];
            if (!tree.isAllowedComponent(component)) continue;

            var componentName = component.properties.name;
            var imagesPath = component.isDashboardElement ? "Dashboards.SmallComponents." : "SmallComponents.";
            var imageName = StiMobileDesigner.checkImageSource(jsObject.options, imagesPath + component.typeComponent + ".png") != null ? imagesPath + component.typeComponent + ".png" : "SmallComponents.StiText.png";
            this.componentsNames.push(componentName);

            var componentAlias = component.properties.aliasName != null ? StiBase64.decode(component.properties.aliasName) : component.properties.alias;
            var captionText = (!componentAlias || componentName == componentAlias) ? componentName : componentName + " [" + componentAlias + "]";
            if (this.jsObject.options.useAliases && this.jsObject.options.showOnlyAliasForComponents && componentAlias) captionText = componentAlias;

            var componentItem = jsObject.TableOfContentTreeItem(captionText, imageName, component, tree, componentName);
            parentItem.addChild(componentItem);
            componentItem.setChecked(component.properties.pointerValue);
            componentItem.setOpening(true);
            componentItem.pointerValue = component.properties.pointerValue ? StiBase64.decode(component.properties.pointerValue) : "";

            if (!lastLevel) {
                if (component.typeComponent == "StiCrossTab") {
                    var crossTabComponents = {}
                    var crossTabChilds = component.controls.crossTabContainer.childNodes;
                    for (var i = 0; i < crossTabChilds.length; i++) {
                        crossTabComponents[crossTabChilds[i].properties.name] = crossTabChilds[i];
                    }
                    tree.addComponents(componentItem, crossTabComponents, true);
                }
                else {
                    var childsStr = component.properties.childs;
                    if (childsStr) {
                        var childNames = childsStr.split(",");
                        var childs = {};
                        for (var indexChild = 0; indexChild < childNames.length; indexChild++) {
                            var child = jsObject.options.report.getComponentByName(childNames[indexChild]);
                            if (child && child.properties.parentName == componentName) childs[child.properties.name] = child;
                        }
                        tree.addComponents(componentItem, childs, true);
                    }
                }
            }
        }
    }

    tree.isAllowedComponent = function (component) {
        if (component.typeComponent == "StiPageHeaderBand" || component.typeComponent == "StiPageFooterBand" || component.typeComponent == "StiTableOfContents")
            return false;

        if (!(jsObject.IsBandComponent(component) || component.typeComponent == "StiText" || component.typeComponent == "StiTextInCells" || component.typeComponent == "StiImage" || component.typeComponent == "StiPanel" ||
            component.typeComponent == "StiCrossTab" || component.typeComponent == "StiCrossField" || component.typeComponent == "StiSubReport" || component.typeComponent == "StiClone"))
            return false;

        if (component.typeComponent == "StiPanel" && component.properties.parentName) {
            var parentComponent = jsObject.options.report.getComponentByName(component.properties.parentName);
            if (parentComponent && jsObject.IsBandComponent(parentComponent))
                return false;
        }

        return true;
    }

    tree.addChangedValue = function (componentType, componentName, pointerValue) {
        this.changedComponents[componentType + componentName] = {
            componentType: componentType,
            componentName: componentName,
            pointerValue: StiBase64.encode(pointerValue)
        }
    }

    tree.onActionItem = function (item) {
        var expressionControl = form.controls.expression;
        expressionControl.setEnabled(item.isChecked);

        if (item.itemObject) {
            if (item.itemObject.typeComponent != "StiReport" && item.itemObject.typeComponent != "StiPage") {
                item.idents = tree.componentsIdents[item.itemObject.properties.name] || [];
            }
            if (item.isChecked) {
                var expressionValue = item.pointerValue || (item.idents && item.idents.length > 0 ? item.idents[0] : "");
                expressionControl.textBox.value = expressionValue;
            }
            else {
                item.pointerValue = "";
                expressionControl.textBox.value = "";
            }
            if (item.idents) {
                var items = [];
                for (var i = 0; i < item.idents.length; i++) {
                    items.push(jsObject.Item("item" + i, item.idents[i], null, item.idents[i]));
                }
                expressionControl.menu.addItems(items);
            }

            var showDropDown = item.idents && item.idents.length > 0;
            expressionControl.dropDownButton.parentElement.style.display = showDropDown ? "" : "none";

            expressionControl.buttonCell.style.borderRight = showDropDown ? "0px" : "";
            expressionControl.buttonCell.style.borderRadius = showDropDown || !jsObject.allowRoundedControls() ? "0" : "0 3px 3px 0";
            expressionControl.textBox.style.width = (showDropDown ? expressionControl.defaultWidth : (expressionControl.defaultWidth + jsObject.options.controlsButtonsWidth + 2)) + "px";
        }
    };

    tree.onCheckedItem = function (item) {
        if (item.itemObject) {
            tree.addChangedValue(item.itemObject.typeComponent, item.itemObject.properties.name || "", form.controls.expression.textBox.value);
        }
    };

    return tree;
}

StiMobileDesigner.prototype.TableOfContentTreeItem = function (caption, imageName, itemObject, tree, id) {
    var item = this.TreeItem(caption, imageName, itemObject, tree, true, id);

    item.button.onmousedown = function () { }
    item.button.ontouchstart = function () { }

    item.button.onclick = function () {
        this.action();
    }

    item.checkBox.action = function () {
        this.treeItem.setChecked(this.isChecked);
        this.treeItem.tree.onActionItem(this.treeItem);
        this.treeItem.tree.onCheckedItem(this.treeItem);
    }

    return item;
}

StiMobileDesigner.prototype.TableOfContentsExpressionControl = function (name, width) {
    var control = this.ExpressionControl(name, width, null, null, null, true);
    control.defaultWidth = parseInt(control.textBox.style.width);

    var dropDownButton = control.dropDownButton = control.clearButton;
    StiMobileDesigner.setImageSource(dropDownButton.image, this.options, "Arrows.SmallArrowDown.png");

    return control;
}