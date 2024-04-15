import { Injectable } from '@angular/core';
import { ModelService } from './model.service';
import { Md5Service } from './md5.service';
import { InteractionParams, InteractionObject, Variable, Message } from './objects';
import { HelperService } from './helper.service';
import { ControllerService } from './controller.service';
import { DrillDownService } from './drill-down.service';

@Injectable()
export class InteractionsService {

  constructor(public model: ModelService, public md5: Md5Service, public controller: ControllerService, public helper: HelperService,
    public drillDownService: DrillDownService) {

    controller.getMessage().subscribe((message: Message) => {
      if (message.action === 'InitVars') {
        this.showParametersPanel(message.data);
      }
    });

    controller.getActionMessage().subscribe((message: Message) => {
      switch (message.action) {
        case 'Variables':
          this.postInteraction({ action: 'Variables', variables: this.getParametersValues() });
          break;

        case 'Reset':
          this.showParametersPanel(this.model.options.paramsVariables);
          break;

        default:
          break;
      }
    });
  }

  public postInteraction(params: InteractionParams) {
    this.drillDownService.saveState();
    // Add new drill-down parameters to drill-down queue and calc guid
    if (params.action !== 'InitVars' && (params.action === 'DrillDown' || params.action === 'DashboardDrillDown')) {
      if (this.model.options.drillDownInProgress) {
        return;
      }

      if (params.action === 'DashboardDrillDown' && params.drillDownParameters) {
        params.drillDownParameters.isDashboardDrillDown = true; // add dbs flag
      }
      const drillDownParameters = this.model.reportParams.drillDownParameters || [];
      params.drillDownParameters = params.drillDownParameters ? drillDownParameters.concat(params.drillDownParameters) : drillDownParameters;

      if (params.action === 'DrillDown') {
        params.drillDownGuid = this.md5.hex_md5(JSON.stringify(this.sortPropsInDrillDownParameters(params.drillDownParameters)));
      } else {
        params.dashboardDrillDownGuid = this.md5.hex_md5(JSON.stringify(this.sortPropsInDrillDownParameters(params.drillDownParameters)));
      }

      this.model.options.drillDownInProgress = true;
    }

    this.controller.post(params.action, this.model.options.actions.interaction, params);
  }

  showParametersPanel(data: any) {
    if (!data) {
      return;
    }

    if (this.controller.showError(data)) {
      data = null;
    }

    this.model.options.isParametersReceived = true;
    this.model.options.paramsVariables = typeof data === 'string' ? JSON.parse(data) : data;
    this.model.controls.parametersPanel.visible = !this.model.options.isMobileDevice;

    // if (this.model.reportParams.type === 'Dashboard') jsObject.postAction('GetPages');

    const paramsVariables = this.helper.copyObject(this.model.options.paramsVariables);
    const countParameters: number = this.helper.getCountObjects(paramsVariables);
    const countColumns: number = (countParameters <= this.model.options.minParametersCountForMultiColumns)
      ? 1 : this.model.options.appearance.parametersPanelColumnsCount;

    let countInColumn = Math.trunc(countParameters / countColumns);
    if (countInColumn * countColumns < countParameters) {
      countInColumn++;
    }

    // setup lists
    for (let i = 0; i < countParameters; i++) {
      const variable = paramsVariables[i] as Variable;
      if (variable.items) {
        variable.items.forEach((v, index) => {
          let isChecked = true;

          if (variable.value instanceof Array && !variable.allowUserValues &&
            variable.value.indexOf(variable.items[index].value) < 0 && variable.value.indexOf(variable.items[index].key) < 0)
            isChecked = false;

          if (variable.isFirstInitialization && variable.checkedStates && index < variable.checkedStates.length) {
            isChecked = variable.checkedStates[index];
          }

          v.isChecked = isChecked;
          v.visible = true;
        });
      }

      variable.isNull = (variable.type === 'DateTime' && variable.value === null && variable.basicType === 'NullableValue');
    }

    this.model.interactions = { paramsVariables, countColumns: new Array(countColumns), countInColumn: new Array(countInColumn) };
  }

  sortPropsInDrillDownParameters(inArray: []): any[] {
    if (!inArray || !inArray.length) {
      return inArray;
    }
    const outArray: any[] = [];
    for (let i = 0; i < inArray.length; i++) {
      const propNames = [];
      const el: any = inArray[i];
      const copyEl = {};
      Object.keys(el).forEach(p => {
        if (el.hasOwnProperty(p)) {
          propNames.push(p);
        }
      });

      propNames.sort();
      propNames.forEach(propName => copyEl[propName] = el[propName]);
      outArray[i] = copyEl;
    }
    return outArray;
  }

  public getParametersValues(): any {
    const parametersValues = {};
    Object.keys(this.model.interactions.paramsVariables).forEach(i => {
      const parameter: Variable = this.model.interactions.paramsVariables[i];
      parametersValues[parameter.name] = this.getVariableValue(parameter);
    });
    return parametersValues;
  }

  getVariableValue(parameter: Variable): any {
    if (parameter.isNull) {
      return null;
    }

    switch (parameter.basicType) {
      case 'Value':
      case 'NullableValue':
        return this.val(parameter.key, parameter, parameter.allowUserValues ? parameter.value : parameter.key);

      case 'Range':
        return { from: this.val(parameter.key, parameter), to: this.val(parameter.keyTo, parameter) };

      case 'List':
        const value = [];
        if (parameter.items) {
          parameter.items.filter(i => parameter.allowUserValues || i.isChecked).forEach(item => value.push(this.val(item.key, parameter)));
        }
        return value;
    }
  }

  val(key: any, parameter: Variable, value?: any): any {
    return parameter.type === 'DateTime' ? this.helper.getStringDateTime(key, parameter.dateTimeType) : (value === undefined ? key : value);
  }

  public initializeInteractions(page: any) {
    if (!page) {
      return;
    }
    const elems = page.querySelectorAll ? page.querySelectorAll('td,div,span,rect,path,ellipse') : page.getElementsByTagName('td');
    const collapsedHash = [];
    for (const elem of elems) {
      if (elem.getAttribute('interaction') && (
        elem.getAttribute('pageguid') ||
        elem.getAttribute('reportfile') ||
        elem.getAttribute('collapsed') ||
        elem.getAttribute('databandsort'))) {

        elem.style.cursor = 'pointer';

        const sort = elem.getAttribute('sort');
        if (sort) {
          this.paintSortingArrow(elem, sort);
        }

        const collapsed = elem.getAttribute('collapsed');
        if (collapsed) {
          const compId = elem.getAttribute('compindex') + '|' + elem.getAttribute('interaction');
          if (collapsedHash.indexOf(compId) < 0) {
            this.paintCollapsingIcon(elem, collapsed === 'true');
            collapsedHash.push(compId);
          }
        }

        elem.onclick = (e: any) => {
          if (elem.getAttribute('pageguid') || elem.getAttribute('reportfile')) {
            this.postInteractionDrillDown(elem);
          } else if (elem.getAttribute('collapsed')) {
            this.postInteractionCollapsing(elem);
          } else {
            this.postInteractionSorting(elem, e.ctrlKey);
          }
        };

        if (elem.getAttribute('pageguid') || elem.getAttribute('reportfile')) {
          elem.onmouseover = () => elem.style.opacity = 0.75;
          elem.onmouseout = () => elem.style.opacity = 1;
        }
      }

    }
  }

  public getComponentOffset(component: any) {
    let offsetX = 0;
    let offsetY = 0;
    const startComponent = component;
    while (component && !isNaN(component.offsetLeft) && !isNaN(component.offsetTop)
      && (component === startComponent || component.style.position === '' || component.style.position === 'static')) {
      offsetX += component.offsetLeft - component.scrollLeft;
      offsetY += component.offsetTop - component.scrollTop;
      component = component.offsetParent;
    }
    return { top: offsetY, left: offsetX };
  }

  public paintSortingArrow(component: any, sort: string) {
    if (component.arrowImg) { return; }

    const arrowImg = document.createElement("div");
    let sortUpSrc = "<svg xmlns='http://www.w3.org/2000/svg' width='12' height='12'><path d='M1 9l5-4 5 4z' fill='#eeeeee' stroke='#666'/></svg>";
    let sortDownSrc = "<svg xmlns='http://www.w3.org/2000/svg' width='12' height='12'><path d='M1 5l5 4 5-4z' fill='#eeeeee' stroke='#666'/></svg>";
    arrowImg.innerHTML = sort == "asc" ? sortDownSrc : sortUpSrc;

    const arrowWidth = (this.model.reportParams.zoom / 100) * 16;
    const arrowHeight = (this.model.reportParams.zoom / 100) * 16;

    arrowImg.style.position = 'absolute';
    arrowImg.style.width = arrowWidth + 'px';
    arrowImg.style.height = arrowHeight + 'px';
    component.appendChild(arrowImg);
    component.arrowImg = arrowImg;

    const oldPosition = component.style.position;
    const oldClassName = component.className;
    let reportDisplayMode = this.model.options.displayModeFromReport || this.model.options.appearance.reportDisplayMode;

    if (reportDisplayMode == "Table") component.style.position = "relative";
    if (!oldClassName) { component.className = 'stiSortingParentElement'; }

    const arrowLeftPos = this.helper.findPosX(arrowImg, component.className);
    const arrowTopPos = this.helper.findPosY(arrowImg, component.className);

    arrowImg.style.marginLeft = (component.offsetWidth - arrowLeftPos - arrowWidth - ((this.model.reportParams.zoom / 100) * 3)) + 'px';
    arrowImg.style.marginTop = (component.offsetHeight / 2 - arrowHeight / 2 - arrowTopPos) + 'px';
    if (oldPosition && reportDisplayMode == "Table") component.style.position = oldPosition;
    component.className = oldClassName;
  }

  public paintCollapsingIcon(component: any, collapsed: boolean) {
    if (component.collapsImg) { return; }
    const collapsImg = document.createElement('img');
    collapsImg.src = collapsed ? this.model.img('CollapsingPlus.png') : this.model.img('CollapsingMinus.png');
    collapsImg.style.position = 'absolute';
    const collapsWidth = (this.model.reportParams.zoom / 100) * 10;
    const collapsHeight = (this.model.reportParams.zoom / 100) * 10;
    collapsImg.style.width = collapsWidth + 'px';
    collapsImg.style.height = collapsHeight + 'px';
    component.appendChild(collapsImg);
    component.collapsImg = collapsImg;

    const componentOffset = this.getComponentOffset(component);
    const collapsOffset = this.getComponentOffset(collapsImg);
    collapsImg.style.marginLeft = (componentOffset.left - collapsOffset.left + collapsWidth / 3) + 'px';
    collapsImg.style.marginTop = (componentOffset.top - collapsOffset.top + collapsWidth / 3) + 'px';
  }

  public postInteractionSorting(component: any, isCtrl: boolean) {
    const params: InteractionParams = {
      action: 'Sorting',
      sortingParameters: {
        ComponentName: component.getAttribute('interaction') + ';' + isCtrl.toString(),
        DataBand: component.getAttribute('databandsort')
      }
    };

    if (this.model.options.isParametersReceived) {
      params.variables = this.getParametersValues();
    }

    this.postInteraction(params);
  }

  public postInteractionDrillDown(component: any) {
    const params: InteractionParams = {
      action: 'DrillDown',
      drillDownParameters: {
        ComponentIndex: component.getAttribute('compindex'),
        ElementIndex: component.getAttribute('elementindex'),
        PageIndex: component.getAttribute('pageindex'),
        PageGuid: component.getAttribute('pageguid'),
        ReportFile: component.getAttribute('reportfile')
      }
    };

    this.postInteraction(params);
  }

  public postInteractionCollapsing(component: any) {
    const componentName = component.getAttribute('interaction');
    const collapsingIndex = component.getAttribute('compindex');
    const collapsed = component.getAttribute('collapsed') === 'true' ? false : true;

    if (!this.model.reportParams.collapsingStates) { this.model.reportParams.collapsingStates = {}; }
    if (!this.model.reportParams.collapsingStates[componentName]) { this.model.reportParams.collapsingStates[componentName] = {}; }
    this.model.reportParams.collapsingStates[componentName][collapsingIndex] = collapsed;

    const params: InteractionParams = {
      action: 'Collapsing',
      collapsingParameters: {
        ComponentName: componentName,
        CollapsingStates: this.model.reportParams.collapsingStates
      }
    };

    if (this.model.options.isParametersReceived) {
      params.variables = this.getParametersValues();
    }

    this.postInteraction(params);
  }

  public updateAllHyperLinks() {
    let pointers = this.model.reportParams.tableOfContentsPointers;
    //var bookmarksPanel = this.controls.bookmarksPanel;
    if (this.model.reportParams.bookmarksContent != null || (pointers && pointers.length > 0)) {
      this.model.pages.forEach(p => {
        const page = p.page;

        const aHyperlinks = page.getElementsByTagName('a');

        for (const aHyperlink of aHyperlinks) {
          aHyperlink.hrefContent = aHyperlink.getAttribute('href');

          if (aHyperlink.hrefContent) {

            if (aHyperlink.hrefContent.indexOf("#") == 0) {
              let anchorParams = aHyperlink.hrefContent.substring(1).split("#GUID#");
              aHyperlink.anchorName = anchorParams[0];
              aHyperlink.componentGuid = anchorParams.length > 1 ? anchorParams[1] : "";

              aHyperlink.onclick = () => {
                let currAnchorName = aHyperlink.anchorName;
                let cuurCompGuid = aHyperlink.componentGuid;

                try {
                  currAnchorName = decodeURI(aHyperlink.anchorName)
                }
                catch (e) {
                  currAnchorName = aHyperlink.anchorName;
                }

                if (pointers.length > 0) {
                  let pageIndex = 1;

                  for (let i = 0; i < pointers.length; i++) {
                    if (cuurCompGuid) {
                      if (pointers[i].componentGuid == cuurCompGuid) {
                        pageIndex = pointers[i].pageIndex;
                        break;
                      }
                    }
                    else if (currAnchorName) {
                      let pointerAnchor = pointers[i].anchor.indexOf("#") == 0 ? pointers[i].anchor.substring(1) : pointers[i].anchor;
                      if (pointerAnchor == currAnchorName) {
                        pageIndex = pointers[i].pageIndex;
                        break;
                      }
                    }
                  }

                  let anchorName = "";
                  if (currAnchorName) anchorName += currAnchorName;
                  if (cuurCompGuid) anchorName += ("#GUID#" + cuurCompGuid);
                  if (anchorName) {
                    this.controller.action({ name: 'BookmarkAction', bookmarkPage: Math.max(pageIndex - 1, 0), bookmarkAnchor: anchorName, componentGuid: aHyperlink.compunentGuid });
                    return false;
                  }
                }

                if (this.model.reportParams.bookmarksContent != null) {
                  const node = this.model.nodes.find(n => n.name === currAnchorName);
                  if (node) {
                    this.model.navigateNode = node;
                  } else {
                    for (let k = 0; k < document.anchors.length; k++) {
                      if (document.anchors[k].name == currAnchorName) {
                        //jsObject.scrollToAnchor(currAnchorName);
                        this.helper.scrollToAnchor(currAnchorName);
                        return;
                      }
                    }
                  }
                  this.controller.post('BookmarkAction', null, 0);


                  /*let aBookmarks = bookmarksPanel.getElementsByTagName("a");
                  for (let k = 0; k < aBookmarks.length; k++) {
                    let clickFunc = aBookmarks[k].getAttribute("onclick");
                    if (clickFunc && clickFunc.indexOf("'" + currAnchorName + "'") >= 0) {
                      try {
                        eval(clickFunc);
                        return false;
                      }
                      catch (e) { }
                    }
                  }

                  this.controller.post('BookmarkAction', null, 0);*/
                  return false;
                }

                //=================================
                /*let pageIndex = 1;
                for (let i = 0; i < pointers.length; i++) {
                  if (aHyperlink.componentGuid) {
                    if (pointers[i].componentGuid == aHyperlink.componentGuid) {
                      pageIndex = pointers[i].pageIndex;
                      break;
                    }
                  }
                  else if (aHyperlink.bookmarkAnchor) {
                    if (pointers[i].anchor == decodeURI(aHyperlink.bookmarkAnchor)) {
                      pageIndex = pointers[i].pageIndex;
                      break;
                    }
                  }
                }
                this.controller.action({ name: 'BookmarkAction', bookmarkPage: pageIndex - 1, bookmarkAnchor: decodeURI(aHyperlink.bookmarkAnchor), componentGuid: aHyperlink.compunentGuid });
                return false;
                };
                } else if (this.model.reportParams.bookmarksContent != null) {
                aHyperlink.anchorName = aHyperlink.hrefContent.replace('#', '');

                aHyperlink.onclick = () => {
                const node = this.model.nodes.find(n => n.url.replace(/\\\'/g, '\'').substr(1) === aHyperlink.anchorName);
                if (node) {
                  this.model.navigateNode = node;
                } else if (aHyperlink.hrefContent.indexOf('#') === 0) {
                  for (let i = 0; i < document.anchors.length; i++) {
                    const anchor = document.anchors[i];
                    if (anchor.name === aHyperlink.anchorName) {
                      this.helper.scrollToAnchor(aHyperlink.anchorName);
                      return;
                    }
                  }
                  this.controller.post('BookmarkAction', null, 0);
                }
                return false;
                };*/
              }
            }
          }
        }
      });

    }
  }

}
