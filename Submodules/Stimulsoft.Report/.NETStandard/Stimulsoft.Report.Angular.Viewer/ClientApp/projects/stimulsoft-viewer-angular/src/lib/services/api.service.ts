import { Injectable } from '@angular/core';
import { ModelService } from './model.service';
import { ControllerService } from './controller.service';
import { ExportService } from './export.service';
import { HelperService } from './helper.service';
import { MailService } from './mail.service';

@Injectable()
export class ApiService {

  public EXPORT_FORMATS: string[] = ['Document', 'Pdf', 'Xps', 'Ppt2007', 'Html', 'Html5', 'Mht', 'Text', 'Rtf', 'Word2007', 'Odt', 'Excel',
    'ExcelBinary', 'ExcelXml', 'Excel2007', 'Ods', 'Csv', 'Dbf', 'Dif', 'Sylk', 'Json', 'Xml', 'ImageBmp',
    'ImageGif', 'ImageJpeg', 'ImagePcx', 'ImagePng', 'ImageTiff', 'ImageEmf', 'ImageSvg', 'ImageSvgz'];

  constructor(private model: ModelService, private controller: ControllerService, private exportService: ExportService,
    private helper: HelperService, private mailService: MailService) { }

  /**
   * The current page number
   */
  public get currentPage(): number {
    return this.model.reportParams.pageNumber;
  }

  public set currentPage(value: number) {
    if (value >= 0 && value < this.model.reportParams.pagesCount) {
      this.model.reportParams.pageNumber = value;
      this.controller.getPages();
    }
  }

  /**
   * The total pages count
   */
  public get pageCount() {
    return this.model.reportParams.pageNumber;
  }

  /**
   * The view mode, can be 'SinglePage', 'Continuous' & 'MultiplePages'
   */
  public get viewMode(): string {
    return this.model.reportParams.viewMode;
  }

  public set viewMode(value: string) {
    if (value === 'SinglePage' || value === 'Continuous' || value === 'MultiplePages') {
      this.model.reportParams.viewMode = value;
      this.controller.getPages();
    }
  }

  /**
   * The page zoom in percent
   * From 1 to 1000
   */
  public get zoom(): number {
    return this.model.reportParams.zoom;
  }

  public set zoom(value: number) {
    if (value >= 1 && value <= 1000) {
      this.model.reportParams.zoom = value;
      this.controller.getPages();
    }
  }

  /**
   * Zoom page in height
   */
  public zoomPageHeight() {
    this.controller.action({ name: 'ZoomOnePage' });
  }

  /**
   * Zoom page in width
   */
  public zoomPageWidth() {
    this.controller.action({ name: 'ZoomPageWidth' });
  }

  /**
   * Print to PDF
   */
  public printPdf() {
    this.controller.action({ name: 'PrintPdf' });
  }

  /**
   * Print with preview
   */
  public printWithPreview() {
    this.controller.action({ name: 'PrintWithPreview' });
  }

  /**
   * Print without preview
   */
  public printWithoutPreview() {
    this.controller.action({ name: 'PrintWithoutPreview' });
  }

  /**
   * Show export form
   *
   * @param format The format to export, can be 'Document', 'Pdf', 'Xps', 'Ppt2007', 'Html', 'Html5', 'Mht', 'Text', 'Rtf', 'Word2007', 'Odt', 'Excel',
   * 'ExcelBinary', 'ExcelXml', 'Excel2007', 'Ods', 'Csv', 'Dbf', 'Dif', 'Sylk', 'Json', 'Xml', 'ImageBmp',
   * 'ImageGif', 'ImageJpeg', 'ImagePcx', 'ImagePng', 'ImageTiff', 'ImageEmf', 'ImageSvg', 'ImageSvgz'
   */
  public showExportForm(format: string) {
    if (this.EXPORT_FORMATS.some(f => f === format)) {
      this.controller.actionSubject.next({ action: 'Export', data: { format } });
    }
  }

  /**
   * Show export form & email
   *
   * @param format The format to export, can be 'Document', 'Pdf', 'Xps', 'Ppt2007', 'Html', 'Html5', 'Mht', 'Text', 'Rtf', 'Word2007', 'Odt', 'Excel',
   * 'ExcelBinary', 'ExcelXml', 'Excel2007', 'Ods', 'Csv', 'Dbf', 'Dif', 'Sylk', 'Json', 'Xml', 'ImageBmp',
   * 'ImageGif', 'ImageJpeg', 'ImagePcx', 'ImagePng', 'ImageTiff', 'ImageEmf', 'ImageSvg', 'ImageSvgz'
   */
  public showExportEmailForm(format: string) {
    if (this.EXPORT_FORMATS.some(f => f === format)) {
      this.controller.actionSubject.next({ action: 'SendEmail', data: { format } });
    }
  }

  /**
   * Export report/dashboard to selected format
   * Use default settings if not specified
   *
   * @param format The format to export, can be 'Document', 'Pdf', 'Xps', 'Ppt2007', 'Html', 'Html5', 'Mht', 'Text', 'Rtf', 'Word2007', 'Odt', 'Excel',
   * 'ExcelBinary', 'ExcelXml', 'Excel2007', 'Ods', 'Csv', 'Dbf', 'Dif', 'Sylk', 'Json', 'Xml', 'ImageBmp',
   * 'ImageGif', 'ImageJpeg', 'ImagePcx', 'ImagePng', 'ImageTiff', 'ImageEmf', 'ImageSvg', 'ImageSvgz'
   *
   * @param settings The export settings
   */
  public export(format: string, settings?: any) {
    if (this.EXPORT_FORMATS.some(f => f === format)) {
      const exportSettings = this.getExportSettings(format, settings);
      this.controller.postExport(format, exportSettings);
    }
  }

  /**
   * Export report/dashboard to seleted format & email
   * Use default settings if not specified
   * Use default email settings if not specified
   *
   * @param format The format to export, can be 'Document', 'Pdf', 'Xps', 'Ppt2007', 'Html', 'Html5', 'Mht', 'Text', 'Rtf', 'Word2007', 'Odt', 'Excel',
   * 'ExcelBinary', 'ExcelXml', 'Excel2007', 'Ods', 'Csv', 'Dbf', 'Dif', 'Sylk', 'Json', 'Xml', 'ImageBmp',
   * 'ImageGif', 'ImageJpeg', 'ImagePcx', 'ImagePng', 'ImageTiff', 'ImageEmf', 'ImageSvg', 'ImageSvgz'
   *
   * @param settings The export settings
   *
   * @param email The email
   *
   * @param message The email message
   *
   * @param subject The email subject
   */
  public exportEmail(format: string, settings?: any, email?: string, subject?: string, message?: string) {
    if (this.EXPORT_FORMATS.some(f => f === format)) {
      this.exportService.exportSettings = this.getExportSettings(format, settings);
      this.exportService.format = format;
      this.mailService.fillDefaults();

      if (email) {
        this.exportService.exportSettings.Email = email;
      }

      if (message) {
        this.exportService.exportSettings.Message = message;
      }

      if (subject) {
        this.exportService.exportSettings.Subject = subject;
      }

      this.mailService.sendMail();
    }
  }

  private getExportSettings(format: string, settings?: any) {
    this.exportService.sendMail = false;
    this.exportService.format = format;
    const exportSettings = this.helper.copyObject(this.exportService.getDefaultExportSettings(format));
    if (settings) {
      Object.keys(settings).forEach(key => exportSettings[key] = settings[key]);
    }
    return exportSettings;
  }

}
