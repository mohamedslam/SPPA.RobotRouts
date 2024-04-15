import { Injectable } from '@angular/core';
import { StiHttpClientService } from './http-client.service';
import { Observable, Subject } from 'rxjs';
import { ModelService } from './model.service';
import { StylesService } from './styles.service';
import { ViewerEvent, BookmarkNode, Message } from './objects';
import { PageService } from './page.service';
import { HelperService } from './helper.service';
import { FormService } from '../forms/form.service';
import { FullScreenService } from './full-screen.service';
import { MenuService, Menu } from '../menu/menu.service';
import { AnimationService } from './animation.service';

@Injectable()
export class ControllerService {

  private subject = new Subject<any>();
  public actionSubject = new Subject<Message>();

  constructor(private httpClient: StiHttpClientService, private model: ModelService,
    private stylesService: StylesService, private pageService: PageService, private helper: HelperService,
    private formService: FormService, private fullScreenService: FullScreenService, private menuService: MenuService,
    private styleService: StylesService, private animationService: AnimationService) {

    setInterval(() => {
      if (this.model.httpError) {
        if (this.model.httpError.error?.text) {
          this.showError(this.model.httpError.error?.text);
        } else if (this.model.httpError.message) {
          this.showError(null, this.model.httpError.message);
        } else if (this.model.httpError.error instanceof ErrorEvent) {
          this.showError(this.model.httpError.error.message);
        }
        this.model.httpError = null;
      }
      if (this.model.navigateNode) {
        this.postBookmarkNodeAction(this.model.navigateNode);
        this.model.navigateNode = null;
      }
    }, 250);

    this.setupHandlers();
  }

  public setupHandlers() {
    this.getMessage().subscribe((message: Message) => {
      switch (message.action) {
        case 'GetReport':
        case 'OpenReport':
          setTimeout(() => {
            this.styleService.addCustomFontStyles(message.data?.customFonts);

            if (this.model.options.appearance.fullScreenMode) {
              this.fullScreenService.changeFullScreenMode(true);
            }

            if (message.data?.stimulsoftFontContent && !this.model.options.stimulsoftFontContentLoaded) {
              this.model.options.stimulsoftFontContentLoaded = message.data.stimulsoftFontContent;
              this.styleService.addCustomFontStyles([{
                contentForCss: message.data.stimulsoftFontContent,
                originalFontFamily: 'Stimulsoft'
              }]);
            }
          });
          break;
      }
    });

    this.model.controls.bookmarksPanel.getVisibility().subscribe((value) => {
      if (value && this.model.options.isMobileDevice) {
        this.model.controls.parametersPanel.visible = false;
      }
      if (value) {
        this.hideToolbar();
      } else {
        this.showToolbar();
      }
    });

    this.model.controls.parametersPanel.getVisibility().subscribe((value) => {
      if (value && this.model.options.isMobileDevice) {
        this.model.controls.bookmarksPanel.visible = false;
      }
      if (value) {
        this.hideToolbar();
      } else {
        this.showToolbar();
      }
    });

    this.menuService.getVisibility().subscribe((menu: Menu) => {
      this.hideToolbar();
    });
  }

  public hideToolbar() {
    if (this.model.options.isMobileDevice && this.model.options.toolbar.autoHide) {
      this.model.controls.toolbar.visible = false;
      this.model.controls.navigatePanel.visible = false;
    }
  }

  public showToolbar() {
    if (this.model.options.isMobileDevice && this.model.options.toolbar.autoHide) {
      this.model.controls.toolbar.visible = true;
      this.model.controls.navigatePanel.visible = true;
      setTimeout(() => {
        this.keepToolbar();
      }, 300);
    }
  }

  public keepToolbar() {
    if (this.model.options.isMobileDevice && this.model.options.toolbar.autoHide) {
      clearTimeout(this.model.toolbarHideTimer);
      this.model.toolbarHideTimer = setTimeout(() => {
        this.hideToolbar();
      }, 4000);
    }
  }

  public getMessage(): Observable<any> {
    return this.subject.asObservable();
  }

  public getActionMessage(): Observable<Message> {
    return this.actionSubject.asObservable();
  }

  public post(action: string, cAction?: string, postData?: any, responseType: string = 'json', subAction?: string) {
    this.model.controls.navigatePanel.enabled = false;
    this.model.controls.toolbar.enabled = false;
    let url: string;
    if (!cAction) {
      url = this.model.requestUrl.replace('{action}', action === 'GetReport' || this.model.options.server.cacheMode === 'None' ? this.model.options.actions.getReport : this.model.options.actions.viewerEvent);
    } else {
      url = this.model.requestUrl.replace('{action}', cAction);
    }
    if (postData) {
      postData.action = action;
    } else {
      postData = { action };
    }

    this.httpClient.post(url, this.model.createPostParameters(postData), responseType).subscribe(data => {
      this.model.showProgress = false;
      this.model.controls.navigatePanel.enabled = true;
      this.model.controls.toolbar.enabled = true;

      if (data === 'ServerError:The report is not specified.' && this.model.options?.isReportRecieved) {
        this.model.options.isReportRecieved = false;
        this.model.reportParams.prevPageNumber = this.model.reportParams.pageNumber;
        this.post('GetReport');
        return;
      }

      if (data) {
        this.model.reportParams = data;
        this.startRefreshReportTimer(data.refreshTime);
        if (data.reportDisplayMode) {
          this.model.options.displayModeFromReport = data.reportDisplayMode;
        }
      }
      this.subject.next({ action, data, subAction });
    });
  }

  public loadViewer(): void {
    const url = this.model.requestUrl.replace('{action}', this.model.action);
    this.httpClient.post(url, this.model.createPostParameters({ action: 'AngularViewerData' }, true, false), 'json',).subscribe(data => {
      this.model.clear();
      this.stylesService.setupStyle(atob(data['styles']), 'viewer');
      this.model.options = data;
      this.checkTrExp();
      this.initAutoUpdateCache();
      this.subject.next({ action: 'viewer_loaded' });
      this.getReport();
    });
  }

  public getReport(): void {
    this.model.options.paramsVariablesStartValues = null;
    this.post('GetReport');
  }

  public getPages(): void {
    this.post('GetPages');
  }

  public postExport(format: string, settings: any, elementName?: string, isDashboardExport: boolean = false) {
    const data = {
      action: isDashboardExport ? 'ExportDashboard' : 'ExportReport',
      exportFormat: format,
      exportSettings: settings,
      elementName
    };

    const doc = settings && settings.OpenAfterExport && this.model.options.appearance.openExportedReportWindow === '_blank' ? this.helper.openNewWindow('about:blank', '_blank').document : null;
    const url = this.model.requestUrl;
    this.httpClient.postForm(url.replace('{action}', this.model.options.actions.exportReport), data, doc);
    this.actionSubject.next({ action: 'ExportReport', data });
  }

  public loadFile(fileName: string, content: any) {
    if (typeof content !== 'string' || content === '') { return; }

    if (content.indexOf('<?xml') === 0 || content.indexOf('{') === 0) { content = btoa(content); }

    const data = {
      action: 'OpenReport',
      openingFileName: fileName || 'Report.mdc',
      base64Data: content.indexOf('base64,') > 0 ? content.substr(content.indexOf('base64,') + 7) : content
    };

    if (fileName && (fileName.toLowerCase().indexOf('.mdx') >= 0 || fileName.toLowerCase().indexOf('.mrx') >= 0)) {
      this.formService.showForm('passwordForm', data);
    } else {
      this.postOpen(data);
    }
  }

  public postOpen(data: any) {
    this.model.clearViewerState();
    this.model.reportParams.reportFileName = data.openingFileName;
    this.post('OpenReport', this.model.options.actions.openReport, data);
  }

  public action(event: ViewerEvent): void {
    switch (event.name) {
      case 'Find':
        this.model.controls.findPanel.visible = !this.model.controls.findPanel.visible;
        return;

      case 'Bookmarks':
        this.model.controls.bookmarksPanel.visible = !this.model.controls.bookmarksPanel.visible;
        return;

      case 'Parameters':
        this.model.controls.parametersPanel.visible = !this.model.controls.parametersPanel.visible;
        return;

      case 'BookmarkAction':
        if (this.model.reportParams.pageNumber === event.bookmarkPage || this.model.reportParams.viewMode !== 'SinglePage') {
          this.helper.scrollToAnchor(event.bookmarkAnchor, event.componentGuid);
          return;
        } else {
          this.model.reportParams.pageNumber = event.bookmarkPage;
          this.model.options.bookmarkAnchor = event.bookmarkAnchor;
          this.model.options.componentGuid = event.componentGuid;
        }
        break;

      case 'GoToPage':
        this.model.reportParams.pageNumber = event.value;
        this.scrollToPage();
        break;

      case 'FirstPage':
        this.model.reportParams.pageNumber = 0;
        this.scrollToPage()
        break;

      case 'PrevPage':
        this.model.reportParams.pageNumber = Math.max(0, this.model.reportParams.pageNumber - 1);
        this.scrollToPage()
        break;

      case 'NextPage':
        this.model.reportParams.pageNumber = Math.min(this.model.reportParams.pagesCount - 1, this.model.reportParams.pageNumber + 1);
        this.scrollToPage();
        break;

      case 'LastPage':
        this.model.reportParams.pageNumber = this.model.reportParams.pagesCount - 1;
        this.scrollToPage()
        break;

      case 'ViewModeSinglePage':
        this.model.reportParams.viewMode = 'SinglePage';
        break;

      case 'ViewModeContinuous':
        this.model.reportParams.viewMode = 'Continuous';
        break;

      case 'ViewModeMultiplePages':
        this.model.reportParams.viewMode = 'MultiplePages';
        break;

      case 'Zoom25': this.model.reportParams.zoom = 25; break;
      case 'Zoom50': this.model.reportParams.zoom = 50; break;
      case 'Zoom75': this.model.reportParams.zoom = 75; break;
      case 'Zoom100': this.model.reportParams.zoom = 100; break;
      case 'Zoom150': this.model.reportParams.zoom = 150; break;
      case 'Zoom200': this.model.reportParams.zoom = 200; break;

      case 'ZoomOnePage':
      case 'ZoomPageWidth':
        this.model.reportParams.zoom = event.name === 'ZoomPageWidth' ? this.pageService.getZoomByPageWidth() : this.pageService.getZoomByPageHeight();
        break;

      case 'Submit':
        this.model.reportParams.editableParameters = null;
        if (this.model.reportParams.type === 'Report') { this.model.reportParams.pageNumber = 0; }
        if (this.model.options.isMobileDevice) { this.model.controls.parametersPanel.visible = false; }
        this.actionSubject.next({ action: 'Variables' });
        return;

      case 'Reset':
        if (this.model.options.paramsVariablesStartValues) {
          this.model.options.paramsVariables = this.model.options.paramsVariablesStartValues;
        }
        this.actionSubject.next({ action: 'Reset' });
        return;

      case 'Editor':
        this.actionSubject.next({ action: 'Editor', data: {} });
        return;

      case 'Resources':
        this.model.controls.resourcesPanel.visible = !this.model.controls.resourcesPanel.visible;
        return;

      case 'PrintPdf':
      case 'PrintWithPreview':
      case 'PrintWithoutPreview':
        this.actionSubject.next({ action: 'Print', data: { format: event.name } });
        return;

      case 'Print':
        let format = 'PrintPdf';
        switch (this.model.options.toolbar.printDestination) {
          case 'Pdf':
            format = 'PrintPdf';
            break;
          case 'Direct':
            format = 'PrintWithoutPreview';
            break;
          case 'WithPreview':
            format = 'PrintWithPreview';
            break;
        }
        this.actionSubject.next({ action: 'Print', data: { format } });
        return;

      case 'Open':
        this.model.openDialogFileMask = null;
        setTimeout(() => {
          this.model.openDialogFileMask = '.mdc,.mdz,.mdx,.mrt,.mrz,.mrx';
        });
        return;

      case 'ResourceView':
        this.postReportResource(event.value.name, 'View');
        return;

      case 'ResourceSaveFile':
        this.postReportResource(event.value.name, 'SaveFile');
        return;

      case 'FullScreen':
        this.fullScreenService.changeFullScreenMode(!this.model.options.appearance.fullScreenMode);
        return;

      case 'About':
        this.actionSubject.next({ action: 'About' });
        return;

      case 'Pin':
        this.model.options.toolbar.autoHide = !this.model.options.toolbar.autoHide;
        this.pageService.calculateLayout();
        if (this.model.options.toolbar.autoHide) {
          setTimeout(() => {
            this.hideToolbar();
          }, 200);
        }
        return;

      case 'Design':
        this.actionSubject.next({ action: 'Design' });
        break;

      default:
        if (event.name.indexOf('saveMenu') === 0) {
          this.actionSubject.next({ action: 'Export', data: { format: event.name.substr(8) } });
          return;
        } else if (event.name.indexOf('sendEmailMenu') === 0) {
          this.actionSubject.next({ action: 'SendEmail', data: { format: event.name.substr(13) } });
          return;
        }
    }

    this.getPages();
  }

  scrollToPage() {
    if (this.model.reportParams.viewMode === 'Continuous') {
      let panel = this.model.controls.reportPanel.el.nativeElement;
      const endTime = (new Date()).getTime() + this.model.options.scrollDuration;
      let targetTop = this.model.pages[this.model.reportParams.pageNumber].page.offsetTop;
      this.animationService.showAnimationForScroll(panel, targetTop, endTime, () => { });
    }
  }

  public showError(message: any, messageText?: string): boolean {
    let type = 'Error';

    // Check for error in 'ServerError:' string format
    if (message != null && typeof (message) === 'string' && message.substr(0, 12) === 'ServerError:') {
      if (message.length <= 13) {
        messageText = 'An unknown error occurred (the server returned an empty value).';
      } else {
        messageText = message.substr(12);
      }
    }

    // Check for error in JSON format
    if (message != null && message.success === false && message.type && message.text) {
      type = message.type;
      messageText = message.text;
    }

    this.model.errorMessage = { error: 'Unknown error', type };

    if (messageText != null) {
      if (messageText === 'The report is not specified.' && !this.model.options.appearance.showReportIsNotSpecifiedMessage) { return true; }

      this.model.errorMessage = { error: messageText.replace('\n', '<br>'), type };

      if (this.model.images) {
        this.formService.showForm('errorMessageForm');
      } else {
        alert(messageText);
      }
      this.actionSubject.next({ action: 'Error' });
      return true;
    }

    return false;
  }

  public postReportResource(resourceName: string, viewType: string) {
    const data = {
      action: 'ReportResource',
      reportResourceParams: {
        resourceName,
        viewType
      }
    };

    const doc = viewType === 'View' ? this.helper.openNewWindow('about:blank', '_blank').document : null;
    const url = this.model.requestUrl;
    this.httpClient.postForm(url.replace('{action}', this.model.options.actions.viewerEvent), data, doc);
  }

  public postBookmarkNodeAction(node: BookmarkNode) {
    node.selected = true;
    this.action({ name: 'BookmarkAction', bookmarkPage: node.page, bookmarkAnchor: node.url.replace(/\\\'/g, '\'').substr(1), componentGuid: node.compunentGuid });
  }

  public viewerResized() {
    this.menuService.closeAllMenus();
  }

  public initAutoUpdateCache() {
    if (this.model.options.server.allowAutoUpdateCache) {
      if (this.model.timerAutoUpdateCache) {
        clearInterval(this.model.timerAutoUpdateCache);
      }
      this.model.timerAutoUpdateCache = setInterval(() => this.post('UpdateCache'), this.model.options.server.timeoutAutoUpdateCache);
    }
  }

  public startRefreshReportTimer(timeout: number) {
    if (this.model.refreshReportTimer != null) {
      clearInterval(this.model.refreshReportTimer);
      this.model.refreshReportTimer = null;
    }

    if (timeout && timeout > 0) {
      this.model.refreshReportTimer = setInterval(() => {
        if (!this.model.showProgress) {
          this.post('Refresh');
        }
      }, timeout * 1000);
    }
  }

  public checkTrExp() {
    if (!this.model.options.cloudMode && !this.model.options.serverMode && !this.model.options.standaloneJsMode && !this.model.options.reportDesignerMode && !this.model.options.alternateValid) {
      let buildDate = new Date();
      try {
        /*if (this.model.options.jsMode && typeof Stimulsoft != 'undefined') {
          let innerDate = Stimulsoft.StiVersion.created.innerDate;
          if (innerDate['getFullYear'] && innerDate.getFullYear() > 2017)
            buildDate = Stimulsoft.StiVersion.created.innerDate;
        }
        else*/ if (this.model.options.buildDate) {
          buildDate = new Date(this.model.options.buildDate);
        }
      } catch (e) {
        buildDate = new Date();
      }

      const trDays = Math.floor(((new Date()).getTime() - buildDate.getTime()) / 1000 / 60 / 60 / 24);
      if (trDays > 60) {
        setTimeout(() => {
          const message = trDays > 120 ? this.model.loc('NoticesYourTrialHasExpired') : this.model.loc('NoticesYouUsingTrialVersion');
          const image = 'Notifications.Warning.png';
          const buttonCaption = this.model.loc('ButtonOk');
          let cancelAction: any;
          let action: any;

          if (trDays > 120) {
            action = cancelAction = () => { window.location.href = 'https://www.stimulsoft.com/en/online-store'; };
          }

          this.model.notificationFormOptions = { message, image, buttonCaption, cancelAction, action };
          this.formService.showForm('notificationForm');
        }, 3000);
      }
    }
  }
}
