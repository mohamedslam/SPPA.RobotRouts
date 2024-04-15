import { Injectable } from '@angular/core';
import { ModelService } from './model.service';
import { InteractionsService } from './interactions.service';
import { HelperService } from './helper.service';
import { EditableFieldsService } from './editable-fields.service';
import { ControllerService } from './controller.service';
import { ComponentDescription, Message } from './objects';
import { MenuItem } from '../menu/meni-item.component';

@Injectable()
export class ToolbarService {

  public comps: ComponentDescription[] = [];
  public dopComps: ComponentDescription[] = [];

  constructor(public model: ModelService, public controller: ControllerService, public editableFieldsService: EditableFieldsService,
    public interactionService: InteractionsService, public helper: HelperService) {
    this.controller.getMessage().subscribe((message: Message) => {
      if (message.action !== 'viewer_loaded') {
        this.updateButtons();
      }
    });
  }

  updateButtons() {
    this.comps.filter(i => i.action === 'ViewMode').forEach(m => m.caption = this.model.loc(this.model.reportParams.viewMode));
    this.comps.filter(i => i.action === 'Zoom').forEach((m) => {
      m.menuItems.forEach(n => n.selected = n.name === ('Zoom' + this.model.reportParams.zoom));
      m.caption = this.model.reportParams.zoom ? Math.round(this.model.reportParams.zoom) + '%' : '';
    });

    this.comps.filter(i => i.action === 'ShowFind').forEach((m) => {
      m.selected = this.model.controls.findPanel.visible;
    });
  }

  initButtons() {
    const comps: ComponentDescription[] = [];
    const dopComps: ComponentDescription[] = [];
    let isFirst = false;

    if (this.model.options.toolbar.showAboutButton) { dopComps.push({ type: 'button', action: 'About', img: 'Help.png' }); }
    if (this.model.options.toolbar.showAboutButton && this.model.options.toolbar.showDesignButton) { dopComps.push({ type: 'separator' }); }
    if (this.model.options.toolbar.showDesignButton) { dopComps.push({ type: 'button', action: 'Design', caption: this.model.loc('Design'), img: 'Design.png' }); }
    if (this.model.options.toolbar.showPinToolbarButton && this.model.options.toolbar.showDesignButton) { dopComps.push({ type: 'separator' }); }
    if (this.model.options.toolbar.showPinToolbarButton) { dopComps.push({ type: 'button', action: 'Pin', img: 'Pin.png' }); }


    if (this.model.options.toolbar.showPrintButton) {
      comps.push({
        type: 'button', action: 'Print', caption: this.model.loc('Print'), img: 'Print.png', tooltip: true,
        arrow: this.model.options.toolbar.printDestination === 'Default' ? 'Down' : null,
        menuItems: this.model.options.toolbar.printDestination === 'Default' ?
          [{ name: 'PrintPdf', caption: this.model.loc('PrintPdf'), img: 'Save.Small.Pdf.png' },
          { name: 'PrintWithPreview', caption: this.model.loc('PrintWithPreview'), img: 'ViewMode.png' },
          { name: 'PrintWithoutPreview', caption: this.model.loc('PrintWithoutPreview'), img: 'Print.png' }] :
          null
      });
      isFirst = false;
    }
    if (this.model.options.toolbar.showOpenButton) {
      comps.push({ type: 'button', action: 'Open', caption: this.model.loc('Open'), img: 'Open.png', tooltip: true });
      isFirst = false;
    }
    if (this.model.options.toolbar.showSaveButton) {
      comps.push({
        type: 'button', action: 'Save', caption: this.model.loc('Save'), img: 'Save.png', tooltip: true, arrow: 'Down',
        menuItems: this.getSaveMenuItems('saveMenu')
      });
      isFirst = false;
    }
    if (this.model.options.toolbar.showSendEmailButton) {
      comps.push({
        type: 'button', action: 'SendEmail', caption: this.model.loc('SendEmail'), img: 'SendEmail.png', tooltip: true, arrow: 'Down',
        menuItems: this.getSaveMenuItems('sendEmailMenu')
      });
      isFirst = false;
    }
    if (this.model.options.toolbar.showBookmarksButton || this.model.options.toolbar.showParametersButton) {
      if (!isFirst) {
        comps.push({ type: 'separator' });
      }
      isFirst = false;
    }
    if (this.model.options.toolbar.showBookmarksButton) {
      comps.push({ type: 'button', action: 'Bookmarks', caption: this.model.options.toolbar.displayMode === 'Separated' ? this.model.loc('Bookmarks') : null, img: 'Bookmarks.png', tooltip: true });
      isFirst = false;
    }
    if (this.model.options.toolbar.showParametersButton) {
      comps.push({ type: 'button', action: 'Parameters', caption: this.model.options.toolbar.displayMode === 'Separated' ? this.model.loc('Parameters') : null, img: 'Parameters.png', tooltip: true });
      isFirst = false;
    }
    if (this.model.options.toolbar.showResourcesButton) {
      comps.push({ type: 'button', action: 'Resources', caption: this.model.options.toolbar.displayMode === 'Separated' ? this.model.loc('Resources') : null, img: 'Resources.png', tooltip: true });
      isFirst = false;
    }
    if (this.model.options.toolbar.showFindButton || this.model.options.toolbar.showEditorButton) {
      if (!isFirst) {
        comps.push({ type: 'separator' });
      }
      isFirst = false;
    }
    if (this.model.options.toolbar.showFindButton) {
      comps.push({ type: 'button', action: 'Find', img: 'Find.png', tooltip: true });
      isFirst = false;
    }
    if (this.model.options.toolbar.showEditorButton) {
      comps.push({ type: 'button', action: 'Editor', img: 'Editor.png', tooltip: true });
      isFirst = false;
    }

    if (this.model.options.toolbar.displayMode !== 'Separated') {
      if (this.model.options.toolbar.showFirstPageButton || this.model.options.toolbar.showPreviousPageButton || this.model.options.toolbar.showNextPageButton ||
        this.model.options.toolbar.showLastPageButton || this.model.options.toolbar.showCurrentPageControl) {
        if (!isFirst) {
          comps.push({ type: 'separator' });
        }
        isFirst = false;
      }
      if (this.model.options.toolbar.showFirstPageButton) { comps.push({ type: 'button', action: 'FirstPage', img: this.model.options.appearance.rightToLeft ? 'LastPage.png' : 'FirstPage.png', tooltip: true }); isFirst = false; }
      if (this.model.options.toolbar.showPreviousPageButton) { comps.push({ type: 'button', action: 'PrevPage', img: this.model.options.appearance.rightToLeft ? 'NextPage.png' : 'PrevPage.png', tooltip: true }); isFirst = false; }
      if (this.model.options.toolbar.showCurrentPageControl) { comps.push({ type: 'pageControl' }); isFirst = false; }

      if (this.model.options.toolbar.showNextPageButton) { comps.push({ type: 'button', action: 'NextPage', img: this.model.options.appearance.rightToLeft ? 'PrevPage.png' : 'NextPage.png', tooltip: true }); isFirst = false; }
      if (this.model.options.toolbar.showLastPageButton) { comps.push({ type: 'button', action: 'LastPage', img: this.model.options.appearance.rightToLeft ? 'FirstPage.png' : 'LastPage.png', tooltip: true }); isFirst = false; }
      if (this.model.options.toolbar.showViewModeButton || this.model.options.toolbar.showZoomButton) {
        if (!isFirst) {
          comps.push({ type: 'separator' });
        }
        isFirst = false;
      }
    }

    if (this.model.options.toolbar.showFullScreenButton) {
      comps.push({ type: 'button', action: 'FullScreen', img: 'FullScreen.png', tooltip: true });
      comps.push({ type: 'separator' });
      isFirst = false;
    }

    if (this.model.options.toolbar.showZoomButton && this.model.options.toolbar.displayMode !== 'Separated') {
      comps.push({
        type: 'button', action: 'Zoom', caption: '100%', img: 'Zoom.png', tooltip: true, arrow: 'Down',
        menuItems: this.helper.getZoomMenuItems()
      });
      isFirst = false;
    }
    if (this.model.options.toolbar.showViewModeButton) {
      comps.push({
        type: 'button', action: 'ViewMode', caption: this.model.loc('SinglePage'), img: 'SinglePage.png', tooltip: true, arrow: 'Down',
        menuItems: [{ name: 'ViewModeSinglePage', caption: this.model.loc('SinglePage'), img: 'SinglePage.png' },
        { name: 'ViewModeContinuous', caption: this.model.loc('Continuous'), img: 'Continuous.png' },
        { name: 'ViewModeMultiplePages', caption: this.model.loc('MultiplePages'), img: 'MultiplePages.png' }]
      });
      isFirst = false;
    }
    if (typeof (this.model.options.toolbar.multiPageWidthCount) !== 'undefined') {
      this.model.reportParams.multiPageWidthCount = this.model.options.toolbar.multiPageWidthCount;
    }
    if (typeof (this.model.options.toolbar.multiPageHeightCount) !== 'undefined') {
      this.model.reportParams.multiPageHeightCount = this.model.options.toolbar.multiPageHeightCount;
    }

    if (!this.model.options.appearance.rightToLeft && this.model.options.toolbar.alignment === 'right' &&
      (this.model.options.toolbar.showPinToolbarButton || this.model.options.toolbar.showAboutButton || this.model.options.toolbar.showDesignButton)) {
      comps.push({ type: 'separator6' });
    }

    this.comps = this.model.options.appearance.rightToLeft ? comps.reverse() : comps;
    this.dopComps = this.model.options.appearance.rightToLeft ? dopComps.reverse() : dopComps;
  }

  disableNaviButtons(): boolean {
    return this.model.reportParams.viewMode === 'MultiplePages' || this.model.reportParams.viewMode === 'WholeReport' ||
      (this.model.reportParams.viewMode === 'Continuous' && !this.model.options.appearance.scrollbarsMode && !this.model.options.appearance.fullScreenMode);
  }

  getImage(imageName: string): string {
    switch (this.model.options.appearance.saveMenuImageSize) {
      case 'Big':
        return `Save.Big.${imageName}.png`;
      case 'None':
        return null;
      default:
        return `Save.Small.${imageName}.png`;
    }
  }

  getSaveMenuItems(menuName: string): any {
    let isFirst = true;
    const items: MenuItem[] = [];
    const imageSize = this.model.options.appearance.saveMenuImageSize;
    if (this.model.options.exports.showExportToDocument && menuName === 'saveMenu') {
      items.push({ name: menuName + 'Document', caption: this.model.loc('ReportSnapshot'), imageSize, img: this.getImage('Document') });
      isFirst = false;
    }
    if (menuName === 'saveMenu' && this.model.options.exports.showExportToPdf || this.model.options.exports.showExportToXps || this.model.options.exports.showExportToPowerPoint) {
      if (!isFirst) {
        items.push({ imageSize, type: 'separator' });
      }
      isFirst = false;
    }
    if (this.model.options.exports.showExportToPdf) {
      items.push({ name: menuName + 'Pdf', caption: 'Adobe PDF', imageSize, img: this.getImage('Pdf') });
    }
    if (this.model.options.exports.showExportToXps) {
      items.push({ name: menuName + 'Xps', caption: 'Microsoft XPS', imageSize, img: this.getImage('Xps') });
    }
    if (this.model.options.exports.showExportToPowerPoint) {
      items.push({ name: menuName + 'Ppt2007', caption: 'Microsoft PowerPoint', imageSize, img: this.getImage('Ppt') });
    }

    if (this.model.options.exports.showExportToHtml || this.model.options.exports.showExportToHtml5 || this.model.options.exports.showExportToMht) {
      if (!isFirst) {
        items.push({ imageSize, type: 'separator' });
      }
      isFirst = false;
      let htmlType = this.model.options.exports.defaultSettings['StiHtmlExportSettings'].HtmlType;
      if (!this.model.options.exports['showExportTo' + htmlType]) {
        if (this.model.options.exports.showExportToHtml) {
          htmlType = 'Html';
        } else if (this.model.options.exports.showExportToHtml5) {
          htmlType = 'Html5';
        } else if (this.model.options.exports.showExportToMht) {
          htmlType = 'Mht';
        }
      }
      items.push({ name: menuName + htmlType, caption: 'HTML', imageSize, img: this.getImage('Html') });
    }
    if (this.model.options.exports.showExportToText || this.model.options.exports.showExportToRtf || this.model.options.exports.showExportToWord2007 || this.model.options.exports.showExportToOdt) {
      if (!isFirst) {
        items.push({ imageSize, type: 'separator' });
      }
      isFirst = false;
    }
    if (this.model.options.exports.showExportToText) {
      items.push({ name: menuName + 'Text', caption: this.model.loc('Text'), imageSize, img: this.getImage('Text') });
    }
    if (this.model.options.exports.showExportToRtf) {
      items.push({ name: menuName + 'Rtf', caption: 'RTF', imageSize, img: this.getImage('Rtf') });
    }
    if (this.model.options.exports.showExportToWord2007) {
      items.push({ name: menuName + 'Word2007', caption: 'Microsoft Word', imageSize, img: this.getImage('Word') });
    }
    if (this.model.options.exports.showExportToOpenDocumentWriter) {
      items.push({ name: menuName + 'Odt', caption: 'OpenDocument Writer', imageSize, img: this.getImage('Odt') });
    }
    if (this.model.options.exports.showExportToExcel || this.model.options.exports.showExportToExcel2007 || this.model.options.exports.showExportToExcelXml || this.model.options.exports.showExportToOpenDocumentWriter) {
      if (!isFirst) {
        items.push({ imageSize, type: 'separator' });
      }
      isFirst = false;
    }
    if (this.model.options.exports.showExportToExcel || this.model.options.exports.showExportToExcelXml || this.model.options.exports.showExportToExcel2007) {
      let excelType = this.model.options.exports.defaultSettings['StiExcelExportSettings'].ExcelType;
      if (excelType === 'ExcelBinary') {
        excelType = 'Excel';
      }
      if (!this.model.options.exports['showExportTo' + excelType]) {
        if (this.model.options.exports.showExportToExcel) {
          excelType = 'Excel';
        } else if (this.model.options.exports.showExportToExcel2007) {
          excelType = 'Excel2007';
        } else if (this.model.options.exports.showExportToExcelXml) {
          excelType = 'ExcelXml';
        }
      }
      items.push({ name: menuName + excelType, caption: 'Microsoft Excel', imageSize, img: this.getImage('Excel') });
    }
    if (this.model.options.exports.showExportToOpenDocumentCalc) {
      items.push({ name: menuName + 'Ods', caption: 'OpenDocument Calc', imageSize, img: this.getImage('Ods') });
    }
    if (this.model.options.exports.showExportToCsv || this.model.options.exports.showExportToDbf || this.model.options.exports.showExportToXml ||
      this.model.options.exports.showExportToDif || this.model.options.exports.showExportToSylk || this.model.options.exports.showExportToJson) {
      if (!isFirst) {
        items.push({ imageSize, type: 'separator' });
      }
      isFirst = false;
      let dataType = this.model.options.exports.defaultSettings['StiDataExportSettings'].DataType;
      if (!this.model.options.exports['showExportTo' + dataType]) {
        if (this.model.options.exports.showExportToCsv) {
          dataType = 'Csv';
        } else if (this.model.options.exports.showExportToDbf) {
          dataType = 'Dbf';
        } else if (this.model.options.exports.showExportToXml) {
          dataType = 'Xml';
        } else if (this.model.options.exports.showExportToDif) {
          dataType = 'Dif';
        } else if (this.model.options.exports.showExportToSylk) {
          dataType = 'Sylk';
        } else if (this.model.options.exports.showExportToJson) {
          dataType = 'Json';
        }
      }
      items.push({ name: menuName + dataType, caption: this.model.loc('Data'), imageSize, img: this.getImage('Data') });
    }
    if (this.model.options.exports.showExportToImageBmp || this.model.options.exports.showExportToImageGif || this.model.options.exports.showExportToImageJpeg || this.model.options.exports.showExportToImagePcx ||
      this.model.options.exports.showExportToImagePng || this.model.options.exports.showExportToImageTiff || this.model.options.exports.showExportToImageMetafile || this.model.options.exports.showExportToImageSvg || this.model.options.exports.showExportToImageSvgz) {
      if (!isFirst) {
        items.push({ imageSize, type: 'separator' });
      }
      isFirst = false;
      let imageType = this.model.options.exports.defaultSettings['StiImageExportSettings'].ImageType;
      let imageType_ = imageType === 'Emf' ? 'Metafile' : imageType;
      if (!this.model.options.exports['showExportToImage' + imageType_]) {
        if (this.model.options.exports.showExportToImageBmp) {
          imageType = 'Bmp';
        } else if (this.model.options.exports.showExportToImageGif) {
          imageType = 'Gif';
        } else if (this.model.options.exports.showExportToImageJpeg) {
          imageType = 'Jpeg';
        } else if (this.model.options.exports.showExportToImagePcx) {
          imageType = 'Pcx';
        } else if (this.model.options.exports.showExportToImagePng) {
          imageType = 'Png';
        } else if (this.model.options.exports.showExportToImageTiff) {
          imageType = 'Tiff';
        } else if (this.model.options.exports.showExportToImageMetafile) {
          imageType = 'Emf';
        } else if (this.model.options.exports.showExportToImageSvg) {
          imageType = 'Svg';
        } else if (this.model.options.exports.showExportToImageSvgz) { imageType = 'Svgz'; }
      }
      items.push({ name: menuName + 'Image' + imageType, caption: this.model.loc('Image'), imageSize, img: this.getImage('Image') });


    }
    return items;
  }

  enabled(desc: ComponentDescription): boolean {
    const disableNaviButtons = this.disableNaviButtons();
    switch (desc.action) {
      case 'FirstPage':
      case 'PrevPage':
        return this.model.reportParams.pageNumber > 0 && !disableNaviButtons;
      case 'NextPage':
      case 'LastPage':
        return this.model.reportParams.pageNumber < this.model.reportParams.pagesCount - 1 && !disableNaviButtons;
      case 'Bookmarks':
        return this.model.reportParams.bookmarksContent != null;
      case 'Parameters':
        return this.model.interactions?.paramsVariables != null;
      case 'Resources':
        return this.model.reportParams.resources?.length > 0;
    }
    return true;
  }

  selected(desc: ComponentDescription): boolean {
    switch (desc.action) {
      case 'Find':
        return this.model.controls.findPanel.visible;
      case 'Bookmarks':
        return this.model.controls.bookmarksPanel.visible;
      case 'Parameters':
        return this.model.controls.parametersPanel.visible;
      case 'Editor':
        return this.editableFieldsService.visible;
      case 'Resources':
        return this.model.controls.resourcesPanel.visible;
      case 'FullScreen':
        return this.model.options.appearance.fullScreenMode;
      case 'Pin':
        return !this.model.options.toolbar.autoHide;
    }
    return false;
  }

  display(desc: ComponentDescription): string {
    switch (desc.action) {
      case 'Editor':
        return this.model.reportParams.isEditableReport ? '' : 'none';

    }
    return '';
  }

  getButonWidth(comp: ComponentDescription): string {
    if (this.model.options.toolbar.displayMode === 'Separated') {
      if (comp.action === 'Find' || comp.action === 'Editor' || comp.action === 'FullScreen' || comp.action === 'About') {
        return '28px';
      }
      if (this.model.options.isMobileDevice) {
        return '0.4in';
      }
    }
    return null;
  }

  getInnerTableWidth(comp: ComponentDescription): string {
    if (this.model.options.toolbar.displayMode === 'Separated' &&
      (this.model.options.isMobileDevice || comp.action === 'Find' || comp.action === 'Editor' || comp.action === 'FullScreen' || comp.action === 'About')) {
      return '100%';
    }
    return null;
  }

}
