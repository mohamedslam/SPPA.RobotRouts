import { Injectable } from '@angular/core';
import { ControlClass, DrillDown, ErrorMessage, InteractionObject, BookmarkNode, Form, NotificationFormOptions } from './objects';

@Injectable()
export class ModelService {
  private _options: any;
  private _reportParams: any = {};
  private _imagesForBookmark: any;

  public requestUrl: string;
  public action: string;
  public properties: any;

  public localization: any;
  public controls: ControlClass = new ControlClass();
  public pages: any[] = [];
  public months: string[] = [];
  public dayOfWeek: string[] = [];
  public dateRanges: string[] = [];
  public encodingData: any[] = [];
  public pdfSecurityCertificates: any[] = [];
  public paperSizes: string[] = [];
  public viewerSize: any;
  public drillDownButtons: DrillDown[] = [];
  public openDialogFileMask: string;
  public images: any;
  public errorMessage: ErrorMessage;
  public httpError: any;
  public interactions: InteractionObject;
  public nodes: BookmarkNode[];
  public navigateNode: BookmarkNode;
  public fullScreenOptions: any;
  public showProgress = false;
  public timerAutoUpdateCache: any;
  public refreshReportTimer: any;
  public toolbarHideTimer: any;
  public form: Form;
  public notificationFormOptions: NotificationFormOptions;
  public postParametersFunction: any;

  constructor() { }

  public get reportParams() {
    return this._reportParams;
  }

  public set reportParams(parameters: any) {
    this._reportParams.pagesArray = parameters.pagesArray;

    // Apply new report parameters, if not update current page
    if (parameters.action && parameters.action !== 'GetPages') {
      this.reportParams.type = parameters.reportType;
      this.reportParams.drillDownGuid = parameters.drillDownGuid;
      this.reportParams.dashboardDrillDownGuid = parameters.dashboardDrillDownGuid;
      this.reportParams.pagesCount = parameters.pagesCount;
      if (parameters.pageNumber != null) { this.reportParams.pageNumber = parameters.pageNumber; }
      this.reportParams.zoom = parameters.zoom;
      this.reportParams.viewMode = parameters.viewMode;
      this.reportParams.reportFileName = parameters.reportFileName;
      this.reportParams.collapsingStates = parameters.collapsingStates;
      if (parameters.bookmarksContent) { this.reportParams.bookmarksContent = parameters.bookmarksContent; }
      if (parameters.resources) { this.reportParams.resources = parameters.resources; }
      this.reportParams.isCompilationMode = parameters.isCompilationMode;
      if (parameters.variablesValues) { this.reportParams.variablesValues = parameters.variablesValues; }
      if (parameters.parametersDateFormat) { this.options.appearance.parametersPanelDateFormat = parameters.parametersDateFormat; }
      if (parameters.tableOfContentsPointers) this.reportParams.tableOfContentsPointers = parameters.tableOfContentsPointers;
      this.reportParams.isEditableReport = parameters.isEditableReport;
      if (parameters.userValues) { this.options.userValues = parameters.userValues; }
      this.reportParams.dashboards = parameters.dashboards;
      this.reportParams.previewSettings = parameters.previewSettings;
    }
  }

  public setReportParams(parameters: any) {
    this._reportParams = parameters;
  }

  public getReportParams() {
    return this.copyObject(this._reportParams);
  }

  public copyObject(o: any): any {
    if (!o || 'object' !== typeof o) {
      return o;
    }
    const c = 'function' === typeof o.pop ? [] : {};
    let p;
    let v;
    for (p in o) {
      if (o.hasOwnProperty(p) && p !== 'pagesArray') {
        v = o[p];
        if (v && 'object' === typeof v) {
          c[p] = this.copyObject(v);
        } else {
          c[p] = v;
        }
      }
    }
    return c;
  }

  public set options(data: any) {
    this._options = this.toLower(data.options);
    if (data.jsOptions) {
      for (const key in data.jsOptions) {
        if (data.jsOptions.hasOwnProperty(key)) {
          this._options[key] = data.jsOptions[key];
        }
      }
    }

    this._options.exports.defaultSettings = data.defaultSettings;
    this.localization = data.loc;
    this.months = data.months;
    this.dayOfWeek = data.dayOfWeek;
    this.images = data.images;
    this.dateRanges = data.dateRanges;
    this.paperSizes = data.paperSizes;
    this.encodingData = data.encodingData;
    this.pdfSecurityCertificates = data.pdfSecurityCertificates;
    this.clearViewerState();
    this.setupOptions();
  }

  public get options() {
    return this._options;
  }

  public loc(attr: string): string {
    return this.localization[attr] ?? attr;
  }

  public img(name: string): string {
    if (name != null && name.length > 0 && !this.images[name]) {
      //console.warn(`Image: ${name} not found`);
    }
    return this.images[name] ?? '';
  }

  private toLower(obj: any): any {
    const result = {};
    Object.keys(obj).forEach(key => {
      result[key.substr(0, 1).toLowerCase() + key.substr(1)] = (typeof (obj[key]) === 'object' && obj[key] !== null) ? this.toLower(obj[key]) : obj[key];
    });
    return result;
  }

  public clear() {
    this._options = undefined;
    this._reportParams = {};
    this._imagesForBookmark = undefined;
    this.pages = [];
    this.form = null;
  }

  public clearViewerState() {
    this.reportParams = {};
    this.reportParams.type = 'Auto';
    this.reportParams.pageNumber = 0;
    this.reportParams.originalPageNumber = 0;
    this.reportParams.drillDownGuid = null;
    this.reportParams.dashboardDrillDownGuid = null;
    this.reportParams.collapsingStates = null;
    this.reportParams.bookmarksContent = null;
    this.reportParams.editableParameters = null;
    this.reportParams.resources = null;
    this.reportParams.drillDownParameters = [];
    this.reportParams.elementName = null;
    this.reportParams.variablesValues = null;
    this.reportParams.tableOfContentsPointers = [];
    this.reportParams.isEditableReport = false;

    this.options.viewerId = this.newToken();
    this.options.clientGuid = this.newToken();
    this.options.paramsVariables = null;
    this.options.multiFilterStates = null;
    this.options.isParametersReceived = false;
    this.options.drillDownInProgress = false;
    this.options.displayModeFromReport = null;
    // this.controls.mainPanel.style.background = '';
    // this.tableElementGridStates = {};

    // Restore current page number, if reload current report
    if (this.reportParams.prevPageNumber) {
      this.reportParams.pageNumber = this.reportParams.prevPageNumber;
      delete this.reportParams.prevPageNumber;
    }

    this.fullScreenOptions = null;
    this.drillDownButtons = [];
    this.controls.parametersPanel.visible = false;
    this.controls.bookmarksPanel.visible = false;
    this.interactions = null;
    /*
    this.controls.drillDownPanel.visible = false;
    this.controls.findPanel.visible = false;
    this.controls.parametersPanel.visible = false;
    this.controls.resourcesPanel.visible = false;*/
  }

  public style(value: string): string {
    return value !== '' && value != null ? value : '';
  }

  public createPostParameters(data?: any, asObject: boolean = true, useOptions: boolean = true): any {
    let params: any;

    // Object params
    const postParams = {
      stiweb_component: 'Viewer',
      stiweb_imagesScalingFactor: this.getImagesScalingFactor()
    };

    if (this.properties) {
      postParams['properties'] = this.encode(JSON.stringify(this.properties));
    }

    if (this.options && useOptions) {
      params = {
        viewerId: this.options.viewerId,
        routes: this.options.routes,
        formValues: this.options.formValues,
        clientGuid: this.options.clientGuid,
        drillDownGuid: this.reportParams.drillDownGuid,
        dashboardDrillDownGuid: this.reportParams.dashboardDrillDownGuid,
        cacheMode: this.options.server.cacheMode,
        cacheTimeout: this.options.server.cacheTimeout,
        cacheItemPriority: this.options.server.cacheItemPriority,
        pageNumber: this.reportParams.pageNumber,
        originalPageNumber: this.reportParams.originalPageNumber,
        reportType: this.reportParams.type,
        zoom: (this.reportParams.zoom && this.reportParams.zoom > 0) ? this.reportParams.zoom : (this.options?.toolbar?.zoom > 0 ? this.options?.toolbar?.zoom : 100),
        viewMode: this.reportParams.viewMode || this.options.toolbar.viewMode,
        multiPageWidthCount: this.reportParams.multiPageWidthCount,
        multiPageHeightCount: this.reportParams.multiPageHeightCount,
        multiPageContainerWidth: this.reportParams.multiPageContainerWidth,
        multiPageContainerHeight: this.reportParams.multiPageContainerHeight,
        multiPageMargins: this.reportParams.multiPageMargins,
        showBookmarks: this.options.toolbar.showBookmarksButton,
        openLinksWindow: this.options.appearance.openLinksWindow,
        chartRenderType: this.options.appearance.chartRenderType,
        reportDisplayMode: (this.options.displayModeFromReport || this.options.appearance.reportDisplayMode),
        drillDownParameters: this.reportParams.drillDownParameters,
        editableParameters: this.reportParams.editableParameters,
        useRelativeUrls: this.options.server.useRelativeUrls,
        passQueryParametersForResources: this.options.server.passQueryParametersForResources,
        passQueryParametersToReport: this.options.server.passQueryParametersToReport,
        version: this.options.shortProductVersion,
        reportDesignerMode: this.options.reportDesignerMode,
        imagesQuality: this.options.appearance.imagesQuality,
        parametersPanelSortDataItems: this.options.appearance.parametersPanelSortDataItems,
        combineReportPages: this.options.appearance.combineReportPages,
        isAngular: true,
        allowAutoUpdateCookies: this.options.server.allowAutoUpdateCookies
      };

      if (this.options.server.useLocalizedCache && this.options.localization) {
        params['useLocalizedCache'] = true;
        params['localization'] = this.options.localization;
      }

      if (this.options.userValues) {
        params['userValues'] = this.options.userValues;
      }

    } else {
      params = {};
    }

    if (data) {
      Object.keys(data).forEach(key => params[key] = data[key]);
    }

    // Object params
    if (params.action) {
      postParams['stiweb_action'] = params.action;
      delete params.action;
    }
    if (params.base64Data) {
      postParams['stiweb_data'] = params.base64Data;
      delete params.base64Data;
    }

    if (this.options && useOptions) {
      // Params
      const jsonParams = JSON.stringify(params);
      if (this.options.server.useCompression) {
        // postParams['stiweb_packed_parameters'] = StiGZipHelper.pack(jsonParams);
        postParams['stiweb_parameters'] = this.encode(jsonParams);
      } else {
        postParams['stiweb_parameters'] = this.encode(jsonParams);
      }
    }

    if (this.postParametersFunction) {
      let postParamsF = this.postParametersFunction(data);
      if (postParamsF) {
        Object.keys(postParamsF).forEach(key => postParams[key] = postParamsF[key]);
      }
    }

    if (asObject) { return postParams; }
  }

  private newToken(): string {
    const a = '1234567890abcdefghijklmnopqrstuvwxyz'.split('');
    const b = [];
    const length = 32;
    for (let i = 0; i < length; i++) {
      const j = (Math.random() * (a.length - 1)).toFixed(0);
      b[i] = a[j];
    }
    return b.join('');
  }

  private setupOptions() {
    if (!this.options.exports.showExportToPowerPoint && !this.options.exports.showExportToPdf && !this.options.exports.showExportToXps &&
      !this.options.exports.showExportToOpenDocumentWriter && !this.options.exports.showExportToOpenDocumentCalc && !this.options.exports.showExportToText &&
      !this.options.exports.showExportToRtf && !this.options.exports.showExportToWord2007 && !this.options.exports.showExportToCsv && !this.options.exports.showExportToJson &&
      !this.options.exports.showExportToDbf && !this.options.exports.showExportToXml && !this.options.exports.showExportToDif && !this.options.exports.showExportToSylk &&
      !this.options.exports.showExportToExcel && !this.options.exports.showExportToExcel2007 && !this.options.exports.showExportToExcelXml && !this.options.exports.showExportToHtml &&
      !this.options.exports.showExportToHtml5 && !this.options.exports.showExportToMht && !this.options.exports.showExportToImageBmp && !this.options.exports.showExportToImageGif &&
      !this.options.exports.showExportToImageJpeg && !this.options.exports.showExportToImageMetafile && !this.options.exports.showExportToImagePcx &&
      !this.options.exports.showExportToImagePng && !this.options.exports.showExportToImageTiff && !this.options.exports.showExportToImageSvg && !this.options.exports.showExportToImageSvgz) {
      if (!this.options.exports.showExportToDocument) {
        this.options.toolbar.showSaveButton = false;
      }
      this.options.toolbar.showSendEmailButton = false;
    }
    // Options
    this.options.isTouchDevice = this.options.appearance.interfaceType === 'Auto'
      ? this.isTouchDevice() && this.isMobileDevice()
      : this.options.appearance.interfaceType === 'Touch';
    this.options.isMobileDevice = this.options.appearance.interfaceType === 'Auto' && !this.options.reportDesignerMode
      ? this.isTouchDevice() && this.isMobileDevice()
      : this.options.appearance.interfaceType === 'Mobile';

    if (this.options.isMobileDevice) {
      this.initializeMobile();
    } else {
      this.options.toolbar.showPinToolbarButton = false;
    }

    this.controls.toolbar.visible = this.options.toolbar.visible;
    this.options.menuAnimDuration = 150;
    this.options.formAnimDuration = 200;
    this.options.scrollDuration = 350;
    this.options.menuHideDelay = 250;

    this.options.server.timeoutAutoUpdateCache = 180000;
    this.options.toolbar.backgroundColor = this.getHTMLColor(this.options.toolbar.backgroundColor);
    this.options.toolbar.borderColor = this.getHTMLColor(this.options.toolbar.borderColor);
    this.options.toolbar.fontColor = this.getHTMLColor(this.options.toolbar.fontColor);
    this.options.appearance.pageBorderColor = this.getHTMLColor(this.options.appearance.pageBorderColor);
    this.options.parametersValues = {};
    this.options.parameterRowHeight = this.options.isTouchDevice ? 35 : 30;
    this.options.minParametersCountForMultiColumns = 5;

    // First Day Of Week
    if (this.options.appearance.datePickerFirstDayOfWeek === 'Sunday') {
      this.dayOfWeek.splice(6, 1);
      this.dayOfWeek.splice(0, 0, 'Sunday');
    }

    if (this.options?.toolbar?.zoom === -1 || this.options?.toolbar?.zoom === -2) {
      this.reportParams.autoZoom = this.options?.toolbar?.zoom;
    }

    // Actions
    if (!this.options.actions.getReport) { this.options.actions.getReport = this.options.actions.viewerEvent; }
    if (!this.options.actions.printReport) { this.options.actions.printReport = this.options.actions.viewerEvent; }
    if (!this.options.actions.openReport) { this.options.actions.openReport = this.options.actions.viewerEvent; }
    if (!this.options.actions.exportReport) { this.options.actions.exportReport = this.options.actions.viewerEvent; }
    if (!this.options.actions.interaction) { this.options.actions.interaction = this.options.actions.viewerEvent; }

    if (!(window.File && window.FileReader && window.FileList && window.Blob)) { this.options.toolbar.showOpenButton = false; }

    // Render JsViewer styles into HEAD
    if (this.options.requestResourcesUrl || this.options.appearance.customStylesUrl) {
      const viewerStyles = document.createElement('link');
      viewerStyles.setAttribute('type', 'text/css');
      viewerStyles.setAttribute('rel', 'stylesheet');
      viewerStyles.setAttribute('href', this.options.appearance.customStylesUrl || this.getResourceUrl('styles'));
      this.controls.head.appendChild(viewerStyles);
    }

    this.options.viewerId = (Math.random() * 1000000).toString();
  }

  public pagesNavigationIsActive(): boolean {
    return (this.options.appearance.fullScreenMode || this.options.appearance.scrollbarsMode) && this.reportParams.viewMode === 'Continuous';
  }

  get imagesForBookmark(): any {
    if (!this._imagesForBookmark) {
      const names = ['root', 'folder', 'folderOpen', 'node', 'empty', 'line', 'join', 'joinBottom', 'plus', 'plusBottom', 'minus', 'minusBottom'];
      const imagesForBookmarks = {};
      names.forEach(name => imagesForBookmarks[name] = this.images['Bookmarks' + name + '.png']);
      this._imagesForBookmark = imagesForBookmarks;
    }
    return this._imagesForBookmark;
  }

  public isTouchDevice() {
    return ('ontouchstart' in document.documentElement);
  }

  public isMobileDevice() {
    return /iPhone|iPad|iPod|Macintosh|Android|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
  }

  public initializeMobile() {
    let isViewPortExist = false;
    const metas: any = this.controls.head.getElementsByTagName('meta');
    for (const meta of metas) {
      if (meta.name && meta.name.toLowerCase() === 'viewport') {
        isViewPortExist = true;
        break;
      }
    }

    if (!isViewPortExist) {
      const viewPortTag = document.createElement('meta');
      viewPortTag.id = 'viewport';
      viewPortTag.name = 'viewport';
      viewPortTag.content = 'initial-scale=1.0,width=device-width,user-scalable=0';
      this.controls.head.appendChild(viewPortTag);
    }

    this.options.appearance.fullScreenMode = true;
    this.options.appearance.scrollbarsMode = true;
    this.options.appearance.parametersPanelPosition = 'Left';
    this.options.appearance.parametersPanelColumnsCount = 1;
    this.options.toolbar.displayMode = 'Separated';
    this.options.toolbar.viewMode = 'SinglePage';
    this.options.toolbar.showZoomButton = false;
    const defaultZoom = this.options.toolbar.zoom === -2 ? -2 : -1; // PageWidth or PageHeight
    this.options.toolbar.zoom = this.reportParams.zoom = defaultZoom;
    this.options.toolbar.showButtonCaptions = false;
    this.options.toolbar.showOpenButton = false;
    this.options.toolbar.showFindButton = false;
    this.options.toolbar.showEditorButton = false;
    this.options.toolbar.showFullScreenButton = false;
    this.options.toolbar.showAboutButton = false;
    this.options.toolbar.showViewModeButton = false;
  }

  public getImagesScalingFactor() {
    const wnd: any = window;
    const devicePixelRatio = window.devicePixelRatio || (wnd.deviceXDPI && wnd.logicalXDPI ? wnd.deviceXDPI / wnd.logicalXDPI : 1);
    if (!devicePixelRatio || devicePixelRatio <= 1) {
      return '1';
    } else {
      return devicePixelRatio.toString();
    }
  }

  public getHTMLColor(color: any) {
    if (color.indexOf(',') > 0 && color.indexOf('rgb') < 0) {
      return `rgb(${color})`;
    }
    return color;
  }

  public getResourceUrl(resourceParameter: string) {
    let url = this.getActionRequestUrl(this.options.requestResourcesUrl, this.options.actions.viewerEvent);
    url += url.indexOf('?') > 0 ? '&' : '?';
    url += 'stiweb_component=Viewer&stiweb_action=Resource&stiweb_data=' + resourceParameter + '&stiweb_theme=' + this.options.theme;
    url += '&stiweb_cachemode=' + (this.options.server.useCacheForResources
      ? this.options.server.cacheMode === 'ObjectSession' || this.options.server.cacheMode === 'StringSession'
        ? 'session'
        : 'cache'
      : 'none');
    url += '&stiweb_version=' + this.options.shortProductVersion;

    return url;
  }

  public getActionRequestUrl(requestUrl: string, action?: string) {
    if (!action) {
      return requestUrl;
    }

    if (action.indexOf('?') < 0) {
      return requestUrl.replace('{action}', action);
    }

    const query = action.substring(action.indexOf('?') + 1);
    action = action.substring(0, action.indexOf('?'));

    return requestUrl.replace('{action}', action) + (requestUrl.indexOf('?') > 0 ? '&' : '?') + query;
  }

  _keyStr: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

  public encode(input) {

    // Decode from UTF-8 text
    if (typeof unescape != 'undefined') {
      input = unescape(encodeURIComponent(input));
    }
    else {
      input = input.replace(/\r\n/g, "\n");
      var utftext = "";
      for (var n = 0; n < input.length; n++) {
        var c = input.charCodeAt(n);

        if (c < 128) {
          utftext += String.fromCharCode(c);
        }
        else if ((c > 127) && (c < 2048)) {
          utftext += String.fromCharCode((c >> 6) | 192);
          utftext += String.fromCharCode((c & 63) | 128);
        }
        else {
          utftext += String.fromCharCode((c >> 12) | 224);
          utftext += String.fromCharCode(((c >> 6) & 63) | 128);
          utftext += String.fromCharCode((c & 63) | 128);
        }
      }

      input = utftext;
    }

    // Encode to Base64 string
    if (typeof window.btoa != 'undefined') return window.btoa(input);

    var output = "";
    var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
    var i = 0;

    while (i < input.length) {
      chr1 = input.charCodeAt(i++);
      chr2 = input.charCodeAt(i++);
      chr3 = input.charCodeAt(i++);

      enc1 = chr1 >> 2;
      enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
      enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
      enc4 = chr3 & 63;

      if (isNaN(chr2)) enc3 = enc4 = 64;
      else if (isNaN(chr3)) enc4 = 64;

      output = output +
        this._keyStr.charAt(enc1) + this._keyStr.charAt(enc2) +
        this._keyStr.charAt(enc3) + this._keyStr.charAt(enc4);
    }

    return output;
  }

  public decode(input) {
    var output = "";

    // Decode from Base64 string
    if (typeof window.atob != 'undefined') {
      output = window.atob(input);
    }
    else {
      var chr1, chr2, chr3;
      var enc1, enc2, enc3, enc4;
      var i = 0;

      input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");
      while (i < input.length) {
        enc1 = this._keyStr.indexOf(input.charAt(i++));
        enc2 = this._keyStr.indexOf(input.charAt(i++));
        enc3 = this._keyStr.indexOf(input.charAt(i++));
        enc4 = this._keyStr.indexOf(input.charAt(i++));

        chr1 = (enc1 << 2) | (enc2 >> 4);
        chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
        chr3 = ((enc3 & 3) << 6) | enc4;

        output = output + String.fromCharCode(chr1);

        if (enc3 != 64) {
          output = output + String.fromCharCode(chr2);
        }
        if (enc4 != 64) {
          output = output + String.fromCharCode(chr3);
        }
      }
    }

    // Encode to UTF-8 string
    if (typeof escape != 'undefined') return decodeURIComponent(escape(output));

    var string = "";
    var i = 0;
    var c = 0;
    var c1 = 0
    var c2 = 0;
    var c3 = 0;

    while (i < output.length) {
      c = output.charCodeAt(i);

      if (c < 128) {
        string += String.fromCharCode(c);
        i++;
      }
      else if ((c > 191) && (c < 224)) {
        c2 = output.charCodeAt(i + 1);
        string += String.fromCharCode(((c & 31) << 6) | (c2 & 63));
        i += 2;
      }
      else {
        c2 = output.charCodeAt(i + 1);
        c3 = output.charCodeAt(i + 2);
        string += String.fromCharCode(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63));
        i += 3;
      }
    }

    return string;
  }
}
