import { Injectable } from '@angular/core';
import { ExportFormSettings, ExportComponent, Message } from './objects';
import { FormService } from '../forms/form.service';
import { HelperService } from './helper.service';
import { ModelService } from './model.service';
import { CollectionsService } from './collections.service';
import { ControllerService } from './controller.service';
import { Subject, Observable } from 'rxjs';

@Injectable()
export class ExportService {

  public exportFormSettings: ExportFormSettings;
  public exportSettings: any = {};
  public format: string;
  public sendMail = false;

  private subject = new Subject<any>();

  constructor(public formService: FormService, public helper: HelperService, public model: ModelService, public collections: CollectionsService,
    public controller: ControllerService) {

    controller.getActionMessage().subscribe((message: Message) => {
      switch (message.action) {
        case 'Export':
          this.export(message.data.format);
          break;
        case 'SendEmail':
          this.export(message.data.format, false, true);
          break;
      }
    });
  }

  public getMessage(): Observable<any> {
    return this.subject.asObservable();
  }

  public export(format: string, update: boolean = false, sendMail: boolean = false) {
    if (!this.helper.checkCloudAuthorization('export')) {
      return;
    }

    this.sendMail = sendMail;
    this.format = format;
    this.exportSettings = this.getDefaultExportSettings(this.format);

    if ((this.model.options.exports.showExportDialog && !sendMail) || (this.model.options.email.showExportDialog && sendMail)) {
      this.exportFormSettings = this.getExportSetings(update);
      if (!update) {
        this.formService.closeForm('exportForm');
        setTimeout(() => {
          this.formService.showForm('exportForm');
        });
      }
    } else {
      this.postExport();
    }
  }

  public postExport() {
    if (!this.sendMail) {
      this.controller.postExport(this.format, this.exportSettings);
    } else {
      this.subject.next('postMail');
    }
  }

  getExportSetings(update: boolean): ExportFormSettings {
    let result: ExportFormSettings;

    this.getDefaultSettings();

    switch (this.format) {
      case 'Document':
        result = { components: this.getComponents(['SaveReportMdc', 'SaveReportMdz', 'SaveReportMdx', 'SaveReportPassword']) };
        break;

      case 'Pdf':
        result = {
          components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'ImageResolution', 'ImageCompressionMethod',
            'AllowEditable', 'ImageQuality', /*'StandardPdfFonts',*/ 'EmbeddedFonts', /*'UseUnicode', 'Compressed',*/ 'ExportRtfTextAsImage', 'PdfACompliance', 'UseDigitalSignature', 'DocumentSecurityButton', 'DigitalSignatureButton',
            'OpenAfterExport', 'PasswordInputUser', 'PasswordInputOwner', 'PrintDocument', 'ModifyContents', 'CopyTextAndGraphics',
            'AddOrModifyTextAnnotations', 'KeyLength', 'GetCertificateFromCryptoUI', 'SubjectNameString']),
          openAfterExport: true
        };
        break;
      case 'Xps':
        result = {
          components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'ImageResolution', 'ImageQuality', 'OpenAfterExport',
            'ExportRtfTextAsImage']),
          openAfterExport: true
        };
        break;
      case 'Ppt2007':
        result = { components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'ImageResolution', 'ImageQuality']) };
        break;

      case 'Html':
        result = {
          components: this.getComponents(['HtmlType', 'Zoom', 'ImageFormatForHtml', 'ExportMode', 'UseEmbeddedImages', 'AddPageBreaks']),
          openAfterExport: true
        };
        break;

      case 'Html5':
        result = {
          components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'HtmlType', 'ImageFormatForHtml', 'ImageResolution',
            'ImageQuality', 'ContinuousPages', 'OpenAfterExport']),
          openAfterExport: true
        };
        break;
      case 'Mht':
        result = {
          components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'HtmlType', 'Zoom', 'ImageFormatForHtml',
            'ExportMode', 'AddPageBreaks'])
        };
        break;
      case 'Text':
        result = {
          components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'KillSpaceLines',
            'PutFeedPageCode', 'DrawBorder', 'CutLongLines', 'BorderType', 'ZoomX', 'ZoomY', 'EncodingTextOrCsvFile'])
        };
        break;
      case 'Rtf':
        result = {
          components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'ImageResolution',
            'ImageQuality', 'ExportModeRtf', 'UsePageHeadersAndFooters', 'RemoveEmptySpaceAtBottom'])
        };
        break;
      case 'Word2007':
        result = {
          components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'ImageResolution',
            'ImageQuality', 'UsePageHeadersAndFooters', 'RemoveEmptySpaceAtBottom'])
        };
        break;
      case 'Odt':
        result = {
          components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'ImageResolution',
            'ImageQuality', 'RemoveEmptySpaceAtBottom'])
        };
        break;
      case 'Excel':
      case 'ExcelBinary':
        result = {
          components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'ExcelType', 'ImageResolution',
            'ImageQuality', 'DataExportMode', 'ExportObjectFormatting', 'UseOnePageHeaderAndFooter', 'ExportEachPageToSheet', 'ExportPageBreaks']),
          openAfterExport: true
        };
        break;
      case 'ExcelXml':
        result = {
          components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'ExcelType'])
        };
        break;
      case 'Excel2007':
        result = {
          components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'ExcelType', 'ImageResolution',
            'ImageQuality', 'DataExportMode', 'ExportObjectFormatting', 'UseOnePageHeaderAndFooter', 'ExportEachPageToSheet', 'ExportPageBreaks'])
        };
        break;
      case 'Ods':
        result = {
          components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'ImageResolution',
            'ImageQuality'])
        };
        break;
      case 'Csv':
        result = {
          components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'DataType', 'EncodingTextOrCsvFile',
            'Separator', 'DataExportMode', 'SkipColumnHeaders'])
        };
        break;
      case 'Dbf':
        result = {
          components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'DataType', 'EncodingDbfFile'])
        };
        break;
      case 'Dif':
        result = {
          components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'DataType', 'ExportDataOnly',
            'UseDefaultSystemEncoding', 'EncodingDifFile'])
        };
        break;
      case 'Sylk':
        result = {
          components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'DataType', 'ExportDataOnly',
            'UseDefaultSystemEncoding', 'EncodingDifFile'])
        };
        break;
      case 'Json':
        result = {
          components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'DataType', 'DataExportMode'])
        };
        break;
      case 'Xml':
        result = {
          components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'DataType'])
        };
        break;
      case 'ImageBmp':
      case 'ImageGif':
      case 'ImageJpeg':
      case 'ImagePcx':
      case 'ImagePng':
      case 'ImageTiff':
      case 'ImageEmf':
      case 'ImageSvg':
      case 'ImageSvgz':
        result = {
          components: this.getComponents(['PageRangeGroup', 'PageRangeAll', 'PageRangeCurrentPage', 'PageRangePages', 'PageRangePagesText', 'SettingsGroup', 'ImageType', 'ImageZoom', 'ImageResolution',
            'ImageFormat', 'DitheringType', 'TiffCompressionScheme', 'CompressToArchive', 'CutEdges'])
        };
        break;
    }

    result.groups = this.getGroups(this.format);
    result.update = update;
    return result;
  }

  getComponents(names: string[]): ExportComponent[] {
    const mrgn = '8px';
    const components: ExportComponent[] = [];
    names.forEach(name => {
      switch (name) {
        case 'ImageType':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('Type'), width: 160, tooltip: this.model.loc('TypeTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getImageTypesItems()
          });
          break;

        case 'DataType':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('Type'), width: 160, tooltip: this.model.loc('TypeTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getDataTypesItems()
          });
          break;

        case 'ExcelType':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('Type'), width: 160, tooltip: this.model.loc('TypeTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getExcelTypesItems()
          });
          break;

        case 'HtmlType':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('Type'), width: 160, tooltip: this.model.loc('TypeTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getHtmlTypesItems()
          });
          break;

        case 'Zoom':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('ZoomHtml'), width: 160, tooltip: this.model.loc('ZoomHtmlTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getZoomItems()
          });
          break;

        case 'ImageZoom':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('ZoomHtml'), width: 160, tooltip: this.model.loc('ZoomHtmlTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getZoomItems()
          });
          break;

        case 'ImageFormatForHtml':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('ImageFormatForHtml'), width: 160, tooltip: this.model.loc('ImageFormatForHtmlTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getImageFormatForHtmlItems()
          });
          break;

        case 'ExportMode':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('ExportMode'), width: 160, tooltip: this.model.loc('ExportModeTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getExportModeItems()
          });
          break;

        case 'CompressToArchive':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('CompressToArchive'), tooltip: this.model.loc('CompressToArchiveTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'UseEmbeddedImages':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('EmbeddedImageData'), tooltip: this.model.loc('EmbeddedImageDataTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'AddPageBreaks':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('AddPageBreaks'), tooltip: this.model.loc('AddPageBreaksTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'ImageResolution':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('ImageResolution'), width: 160, tooltip: this.model.loc('ImageResolutionTooltip'),
            margin: '2px 4px 2px ' + mrgn, items: this.collections.getImageResolutionItems()
          });
          break;

        case 'ImageCompressionMethod':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('ImageCompressionMethod'), width: 160, tooltip: this.model.loc('ImageCompressionMethodTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getImageCompressionMethodItems()
          });
          break;

        case 'AllowEditable':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('AllowEditable'), width: 160, tooltip: this.model.loc('AllowEditableTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getAllowEditableItems()
          });
          break;

        case 'ImageQuality':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('ImageQuality'), width: 160, tooltip: this.model.loc('ImageQualityTooltip'),
            margin: '2px 4px 2px ' + mrgn, items: this.collections.getImageQualityItems()
          });
          break;

        case 'ContinuousPages':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('ContinuousPages'), tooltip: this.model.loc('ContinuousPagesTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'StandardPdfFonts':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('StandardPDFFonts'), tooltip: this.model.loc('StandardPDFFontsTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'EmbeddedFonts':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('EmbeddedFonts'), tooltip: this.model.loc('EmbeddedFontsTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'UseUnicode':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('UseUnicode'), tooltip: this.model.loc('UseUnicodeTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'Compressed':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('Compressed'), tooltip: this.model.loc('CompressedTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'ExportRtfTextAsImage':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('ExportRtfTextAsImage'), tooltip: this.model.loc('ExportRtfTextAsImageTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'PdfACompliance':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('PdfACompliance'), tooltip: this.model.loc('PdfAComplianceTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'KillSpaceLines':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('KillSpaceLines'), tooltip: this.model.loc('KillSpaceLinesTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'PutFeedPageCode':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('PutFeedPageCode'), tooltip: this.model.loc('PutFeedPageCodeTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'DrawBorder':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('DrawBorder'), tooltip: this.model.loc('DrawBorderTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'CutLongLines':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('CutLongLines'), tooltip: this.model.loc('CutLongLinesTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'BorderType':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('BorderType'), width: 160, tooltip: this.model.loc('BorderTypeTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getBorderTypeItems()
          });
          break;

        case 'ZoomX':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('ZoomXY').replace(':', '') + ' X ', width: 160, tooltip: this.model.loc('ZoomXYTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getZoomItems()
          });
          break;

        case 'ZoomY':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('ZoomXY').replace(':', '') + ' Y ', width: 160, tooltip: this.model.loc('ZoomXYTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getZoomItems()
          });
          break;

        case 'EncodingTextOrCsvFile':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('EncodingData'), width: 160, tooltip: this.model.loc('EncodingDataTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getEncodingDataItems()
          });
          break;

        case 'ImageFormat':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('ImageFormat'), width: 160, tooltip: this.model.loc('ImageFormatTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getImageFormatItems()
          });
          break;

        case 'DitheringType':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('MonochromeDitheringType'), width: 160, tooltip: this.model.loc('MonochromeDitheringTypeTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getMonochromeDitheringTypeItems()
          });
          break;

        case 'TiffCompressionScheme':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('TiffCompressionScheme'), width: 160, tooltip: this.model.loc('TiffCompressionSchemeTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getTiffCompressionSchemeItems()
          });
          break;

        case 'CutEdges':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('CutEdges'), tooltip: this.model.loc('CutEdgesTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'MultipleFiles':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('MultipleFiles'), tooltip: this.model.loc('MultipleFilesTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'ExportDataOnly':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('ExportDataOnly'), tooltip: this.model.loc('ExportDataOnlyTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'UseDefaultSystemEncoding':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('UseDefaultSystemEncoding'), tooltip: this.model.loc('UseDefaultSystemEncodingTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'EncodingDifFile':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('EncodingDifFile'), width: 160, tooltip: this.model.loc('EncodingDifFileTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getEncodingDifFileItems()
          });
          break;

        case 'ExportModeRtf':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('ExportModeRtf'), width: 160, tooltip: this.model.loc('ExportModeRtfTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getExportModeRtfItems()
          });
          break;

        case 'UsePageHeadersAndFooters':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('UsePageHeadersFooters'), tooltip: this.model.loc('UsePageHeadersFootersTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'RemoveEmptySpaceAtBottom':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('RemoveEmptySpace'), tooltip: this.model.loc('RemoveEmptySpaceTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'Separator':
          components.push({
            name, type: 'TextBox', label: this.model.loc('Separator'), tooltip: this.model.loc('SeparatorTooltip'), width: 160,
            margin: '2px ' + mrgn + ' 2px ' + mrgn
          });
          break;

        case 'DataExportMode':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('BandsFilter'), width: 160, tooltip: this.model.loc('BandsFilterTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getDataExportModeItems()
          });
          break;

        case 'SkipColumnHeaders':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('SkipColumnHeaders'), tooltip: this.model.loc('SkipColumnHeadersTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'ExportObjectFormatting':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('ExportObjectFormatting'), tooltip: this.model.loc('ExportObjectFormattingTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'UseOnePageHeaderAndFooter':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('UseOnePageHeaderFooter'), tooltip: this.model.loc('UseOnePageHeaderFooterTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'ExportEachPageToSheet':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('ExportEachPageToSheet'), tooltip: this.model.loc('ExportEachPageToSheetTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'ExportPageBreaks':
          components.push({
            name, type: 'CheckBox', caption: this.model.loc('ExportPageBreaks'), tooltip: this.model.loc('ExportPageBreaksTooltip'),
            margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        case 'EncodingDbfFile':
          components.push({
            name, type: 'DropDownListForExportForm', label: this.model.loc('EncodingDbfFile'), width: 160, tooltip: this.model.loc('EncodingDbfFileTooltip'),
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getEncodingDbfFileItems()
          });
          break;

        case 'DocumentSecurityButton':
          components.push({
            name, type: 'SmallButton', caption: this.model.loc('DocumentSecurityButton'), width: 160,
            margin: '2px ' + mrgn + ' 2px ' + mrgn, items: this.collections.getEncodingDbfFileItems()
          });
          break;

        case 'UseDigitalSignature':
          components.push({
            name, caption: this.model.loc('DigitalSignatureButton'), type: 'DropDownListForExportForm', tooltip: this.model.loc('UseDigitalSignatureTooltip'), width: 160,
            items: this.collections.getPdfSecurityCertificatesItems(), margin: '4px ' + mrgn + ' 4px ' + mrgn
          });
          break;

        default:
          break;
      }
    });
    return components;
  }

  getGroups(format: string): any {
    const openingGroupsStr = this.helper.getCookie('StimulsoftWebViewerExportSettingsOpeningGroups');
    const openingGroups = openingGroupsStr ? JSON.parse(openingGroupsStr) : null;
    const pageRangeAllIsDisabled = format.indexOf('Image') === 0 && format !== 'ImageTiff';

    return {
      savingReportGroup: { visible: format === 'Document', opened: openingGroups ? openingGroups.SavingReportGroup : true },
      pageRangeGroup: { visible: format !== 'Document', opened: openingGroups ? openingGroups.PageRangeGroup : true, pageRangeAllIsDisabled },
      settingsGroup: { visible: format !== 'Document', opened: openingGroups ? openingGroups.SettingsGroup : false }
    };
  }

  getDefaultSettings(): any {
    let settings: any;
    if (this.model.options.exports.storeExportSettings && this.helper.getCookie('StimulsoftWebViewerExportSettings' + this.getCommonExportFormat())) {
      const exportSettingsStr = this.helper.getCookie('StimulsoftWebViewerExportSettings' + this.getCommonExportFormat());
      const exportSettings = JSON.parse(exportSettingsStr);
      let exportFormat = exportSettings.ImageType || exportSettings.DataType || exportSettings.ExcelType || exportSettings.HtmlType;
      if (exportFormat === 'ExcelBinary') { exportFormat = 'Excel'; }
      this.format = exportFormat ? (exportSettings.ImageType ? 'Image' + exportFormat : exportFormat) : this.format;

      const defSettings = this.getDefaultExportSettings(this.format);
      const resultSettings: any = {};
      Object.keys(defSettings).forEach(key => resultSettings[key] = exportSettings[key] || defSettings[key]);
      Object.keys(exportSettings).forEach(key => resultSettings[key] = exportSettings[key] || defSettings[key]);

      if (this.model.options.exports.showOpenAfterExport === false) {
        resultSettings.OpenAfterExport = true;
      } else if (!(this.format === 'Pdf' || this.format === 'Xps' || this.format === 'Html' || this.format === 'Excel' || this.format === 'ExcelBinary')) {
        resultSettings.OpenAfterExport = false;
      }

      if (this.model.options.exports.openAfterExport === true || this.model.options.exports.openAfterExport === false) {
        resultSettings.OpenAfterExport = this.model.options.exports.openAfterExport;
      }

      settings = resultSettings;
    } else {
      settings = this.getDefaultExportSettings(this.format);
    }

    this.updateTypes(settings);

    this.exportSettings = this.helper.copyObject(settings);
  }

  updateTypes(defaultExportSettings: any) {
    const types = ['ImageType', 'DataType', 'ExcelType', 'HtmlType'];
    types.forEach((propertyName) => {
      if (defaultExportSettings[propertyName]) {
        switch (propertyName) {
          case 'ImageType':
            if (!this.model.options.exports.showExportToImageBmp && defaultExportSettings[propertyName] === 'Bmp') { defaultExportSettings[propertyName] = 'Gif'; }
            if (!this.model.options.exports.showExportToImageGif && defaultExportSettings[propertyName] === 'Gif') { defaultExportSettings[propertyName] = 'Jpeg'; }
            if (!this.model.options.exports.showExportToImageJpeg && defaultExportSettings[propertyName] === 'Jpeg') { defaultExportSettings[propertyName] = 'Pcx'; }
            if (!this.model.options.exports.showExportToImagePcx && defaultExportSettings[propertyName] === 'Pcx') { defaultExportSettings[propertyName] = 'Png'; }
            if (!this.model.options.exports.showExportToImagePng && defaultExportSettings[propertyName] === 'Png') { defaultExportSettings[propertyName] = 'Tiff'; }
            if (!this.model.options.exports.showExportToImageTiff && defaultExportSettings[propertyName] === 'Tiff') { defaultExportSettings[propertyName] = 'Emf'; }
            if (!this.model.options.exports.showExportToImageMetafile && defaultExportSettings[propertyName] === 'Emf') { defaultExportSettings[propertyName] = 'Svg'; }
            if (!this.model.options.exports.showExportToImageSvg && defaultExportSettings[propertyName] === 'Svg') { defaultExportSettings[propertyName] = 'Svgz'; }
            if (!this.model.options.exports.showExportToImageSvgz && defaultExportSettings[propertyName] === 'Svgz') { defaultExportSettings[propertyName] = 'Bmp'; }
            break;

          case 'DataType':
            if (!this.model.options.exports.showExportToCsv && defaultExportSettings[propertyName] === 'Csv') { defaultExportSettings[propertyName] = 'Dbf'; }
            if (!this.model.options.exports.showExportToDbf && defaultExportSettings[propertyName] === 'Dbf') { defaultExportSettings[propertyName] = 'Xml'; }
            if (!this.model.options.exports.showExportToXml && defaultExportSettings[propertyName] === 'Xml') { defaultExportSettings[propertyName] = 'Dif'; }
            if (!this.model.options.exports.showExportToDif && defaultExportSettings[propertyName] === 'Dif') { defaultExportSettings[propertyName] = 'Sylk'; }
            if (!this.model.options.exports.showExportToSylk && defaultExportSettings[propertyName] === 'Sylk') { defaultExportSettings[propertyName] = 'Csv'; }
            if (!this.model.options.exports.showExportToJson && defaultExportSettings[propertyName] === 'Json') { defaultExportSettings[propertyName] = 'Json'; }
            break;

          case 'ExcelType':
            if (!this.model.options.exports.showExportToExcel2007 && defaultExportSettings[propertyName] === 'Excel2007') { defaultExportSettings[propertyName] = 'ExcelBinary'; }
            if (!this.model.options.exports.showExportToExcel && defaultExportSettings[propertyName] === 'ExcelBinary') { defaultExportSettings[propertyName] = 'ExcelXml'; }
            if (!this.model.options.exports.showExportToExcelXml && defaultExportSettings[propertyName] === 'ExcelXml') { defaultExportSettings[propertyName] = 'Excel2007'; }
            break;

          case 'HtmlType':
            if (!this.model.options.exports.showExportToHtml && defaultExportSettings[propertyName] === 'Html') { defaultExportSettings[propertyName] = 'Html5'; }
            if (!this.model.options.exports.showExportToHtml5 && defaultExportSettings[propertyName] === 'Html5') { defaultExportSettings[propertyName] = 'Mht'; }
            if (!this.model.options.exports.showExportToMht && defaultExportSettings[propertyName] === 'Mht') { defaultExportSettings[propertyName] = 'Html'; }
            break;
        }
      }
    });
  }

  getCommonExportFormat(): string {
    if (this.format === 'Html' || this.format === 'Html5' || this.format === 'Mht') { return 'Html'; }
    if (this.format === 'Excel' || this.format === 'Excel2007' || this.format === 'ExcelXml') { return 'Excel'; }
    if (this.format === 'Csv' || this.format === 'Dbf' || this.format === 'Xml' || this.format === 'Dif' || this.format === 'Sylk') { return 'Data'; }
    if (this.format === 'ImageBmp' || this.format === 'ImageGif' || this.format === 'ImageJpeg' || this.format === 'ImagePcx' || this.format === 'ImagePng' ||
      this.format === 'ImageTiff' || this.format === 'ImageEmf' || this.format === 'ImageSvg' || this.format === 'ImageSvgz') { return 'Image'; }

    return this.format;
  }

  public getDefaultExportSettings(format: string, isDashboardExport: boolean = false) {
    let exportSettings = null;

    if (isDashboardExport) {
      return this.model.options.exports.defaultSettings['Dashboard' + format];
    }

    switch (format) {
      case 'Document':
        exportSettings = { Format: 'Mdc' };
        break;

      case 'Pdf':
        exportSettings = this.model.options.exports.defaultSettings['StiPdfExportSettings'];
        break;

      case 'Xps':
        exportSettings = this.model.options.exports.defaultSettings['StiXpsExportSettings'];
        break;

      case 'Ppt2007':
        exportSettings = this.model.options.exports.defaultSettings['StiPpt2007ExportSettings'];
        break;

      case 'Html':
        exportSettings = this.model.options.exports.defaultSettings['StiHtmlExportSettings'];
        exportSettings.HtmlType = 'Html';
        break;

      case 'Html5':
        exportSettings = this.model.options.exports.defaultSettings['StiHtmlExportSettings'];
        exportSettings.HtmlType = 'Html5';
        break;

      case 'Mht':
        exportSettings = this.model.options.exports.defaultSettings['StiHtmlExportSettings'];
        exportSettings.HtmlType = 'Mht';
        break;

      case 'Text':
        exportSettings = this.model.options.exports.defaultSettings['StiTxtExportSettings'];
        break;

      case 'Rtf':
        exportSettings = this.model.options.exports.defaultSettings['StiRtfExportSettings'];
        break;

      case 'Word2007':
        exportSettings = this.model.options.exports.defaultSettings['StiWord2007ExportSettings'];
        break;

      case 'Odt':
        exportSettings = this.model.options.exports.defaultSettings['StiOdtExportSettings'];
        break;

      case 'Excel':
        exportSettings = this.model.options.exports.defaultSettings['StiExcelExportSettings'];
        exportSettings.ExcelType = 'ExcelBinary';
        break;

      case 'ExcelXml':
        exportSettings = this.model.options.exports.defaultSettings['StiExcelExportSettings'];
        exportSettings.ExcelType = 'ExcelXml';
        break;

      case 'Excel2007':
        exportSettings = this.model.options.exports.defaultSettings['StiExcelExportSettings'];
        exportSettings.ExcelType = 'Excel2007';
        break;

      case 'Ods':
        exportSettings = this.model.options.exports.defaultSettings['StiOdsExportSettings'];
        break;

      case 'ImageBmp':
        exportSettings = this.model.options.exports.defaultSettings['StiImageExportSettings'];
        exportSettings.ImageType = 'Bmp';
        break;

      case 'ImageGif':
        exportSettings = this.model.options.exports.defaultSettings['StiImageExportSettings'];
        exportSettings.ImageType = 'Gif';
        break;

      case 'ImageJpeg':
        exportSettings = this.model.options.exports.defaultSettings['StiImageExportSettings'];
        exportSettings.ImageType = 'Jpeg';
        break;

      case 'ImagePcx':
        exportSettings = this.model.options.exports.defaultSettings['StiImageExportSettings'];
        exportSettings.ImageType = 'Pcx';
        break;

      case 'ImagePng':
        exportSettings = this.model.options.exports.defaultSettings['StiImageExportSettings'];
        exportSettings.ImageType = 'Png';
        break;

      case 'ImageTiff':
        exportSettings = this.model.options.exports.defaultSettings['StiImageExportSettings'];
        exportSettings.ImageType = 'Tiff';
        break;

      case 'ImageSvg':
        exportSettings = this.model.options.exports.defaultSettings['StiImageExportSettings'];
        exportSettings.ImageType = 'Svg';
        break;

      case 'ImageSvgz':
        exportSettings = this.model.options.exports.defaultSettings['StiImageExportSettings'];
        exportSettings.ImageType = 'Svgz';
        break;

      case 'ImageEmf':
        exportSettings = this.model.options.exports.defaultSettings['StiImageExportSettings'];
        exportSettings.ImageType = 'Emf';
        break;

      case 'Xml':
        exportSettings = this.model.options.exports.defaultSettings['StiDataExportSettings'];
        exportSettings.DataType = 'Xml';
        break;

      case 'Csv':
        exportSettings = this.model.options.exports.defaultSettings['StiDataExportSettings'];
        exportSettings.DataType = 'Csv';
        break;

      case 'Dbf':
        exportSettings = this.model.options.exports.defaultSettings['StiDataExportSettings'];
        exportSettings.DataType = 'Dbf';
        break;

      case 'Dif':
        exportSettings = this.model.options.exports.defaultSettings['StiDataExportSettings'];
        exportSettings.DataType = 'Dif';
        break;

      case 'Sylk':
        exportSettings = this.model.options.exports.defaultSettings['StiDataExportSettings'];
        exportSettings.DataType = 'Sylk';
        break;
    }

    exportSettings.OpenAfterExport = !(this.model.options.exports.showOpenAfterExport === false);
    if (!(format === 'Pdf' || format === 'Xps' || format === 'Html' || format === 'Excel' || format === 'ExcelBinary')) {
      exportSettings.OpenAfterExport = false;
    }

    return exportSettings;
  }

}
