import { Component, OnInit, Input, OnChanges, ViewChild } from '@angular/core';
import { ModelService } from '../services/model.service';
import { ExportFormSettings, ExportComponent, ExportGroup } from '../services/objects';
import { ExportService } from '../services/export.service';
import { BaseFormComponent } from './base-form.component';
import { HelperService } from '../services/helper.service';
import { FormService } from './form.service';
import { MenuService } from '../menu/menu.service';
import { ButtonComponent } from '../controls/button.component';
import { CollectionsService } from '../services/collections.service';
import { RadioButtonComponent } from '../controls/radio-button.components';
import { ControllerService } from '../services/controller.service';

@Component({
  selector: 'sti-export-form',
  template: `
    <sti-base-form #baseForm [fontFamily]="model.options.toolbar.fontFamily"
    [color]="model.options.toolbar.fontColor"
    [fontSize]="'12px'"
    [containerPadding]="'3px'"
    [name]="'exportForm'"
    [caption]="model.loc('ExportFormTitle')"
    [defaultTop]="150"
    (changeVisibility)="changeVisibility($event)"
    (action)="action()">
    <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="width: 100%;">
      <tbody>
          <tr *ngIf="exportFormSettings?.groups.savingReportGroup.visible" class="stiJsViewerClearAllStyles">
            <td class="stiJsViewerClearAllStyles" colspan="2">
              <sti-group-panel [caption]="model.loc('SavingReport')"
               [width]="390"
               [margin]="'4px'"
               [innerPadding]="'4px 0 4px 0'"
               [opened]="exportFormSettings.groups.savingReportGroup.opened"
               (action)="exportFormSettings.groups.savingReportGroup.opened=$event">
                <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="width: 100%;">
                  <tbody>
                    <tr class="stiJsViewerClearAllStyles">
                      <td class="stiJsViewerClearAllStyles" colspan="2">
                        <sti-radio-button #saveReportMdc
                           [name]="baseForm.name + 'SaveReportMdc'"
                           [groupName]="baseForm.name + 'SavingReportGroup'"
                           [caption]="model.loc('SaveReportMdc')"
                           [margin]="'6px 8px 3px 8px'"
                           [checked]="true"
                           (action)="exportService.exportSettings.Format = 'Mdc'">
                        </sti-radio-button>
                      </td>
                    </tr>
                    <tr class="stiJsViewerClearAllStyles">
                      <td class="stiJsViewerClearAllStyles" colspan="2">
                        <sti-radio-button
                           [name]="baseForm.name + 'SaveReportMdz'"
                           [groupName]="baseForm.name + 'SavingReportGroup'"
                           [caption]="model.loc('SaveReportMdz')"
                           [margin]="'3px 8px 3px 8px'"
                           (action)="exportService.exportSettings.Format = 'Mdz'">
                        </sti-radio-button>
                      </td>
                    </tr>
                    <tr class="stiJsViewerClearAllStyles">
                      <td class="stiJsViewerClearAllStyles" colspan="2">
                        <sti-radio-button
                           [name]="baseForm.name + 'SaveReportMdx'"
                           [groupName]="baseForm.name + 'SavingReportGroup'"
                           [caption]="model.loc('SaveReportMdx')"
                           [margin]="'3px 8px 0px 8px'"
                           (action)="exportService.exportSettings.Format = 'Mdx'">
                        </sti-radio-button>
                      </td>
                    </tr>
                    <tr class="stiJsViewerClearAllStyles">
                      <td class="stiJsViewerClearAllStyles" style="padding: 0px 8px; min-width: 150px;" [attr.title]="model.loc('PasswordSaveReportTooltip')">
                      {{model.loc('PasswordSaveReport')}}
                      </td>
                      <td>
                        <sti-text-box
                           [width]="140"
                           [tooltip]="model.loc('PasswordSaveReportTooltip')"
                           [margin]="'4px 8px 0px 8px'"
                           [type]="getTextBoxType('SaveReportPassword')"
                           [enabled]="exportService.exportSettings.Format == 'Mdx'"
                           (onchange)="exportService.exportSettings.Password = $event.value">
                        </sti-text-box>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </sti-group-panel>
            </td>
          </tr>

          <tr *ngIf="exportFormSettings?.groups.pageRangeGroup.visible" class="stiJsViewerClearAllStyles">
            <td class="stiJsViewerClearAllStyles" colspan="2">
              <sti-group-panel [caption]="model.loc('PagesRange')"
               [width]="390"
               [margin]="'4px'"
               [innerPadding]="'4px 0 4px 0'"
               [opened]="exportFormSettings.groups.pageRangeGroup.opened"
               (action)="exportFormSettings.groups.pageRangeGroup.opened=$event">
                <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="width: 100%;">
                  <tbody>
                    <tr class="stiJsViewerClearAllStyles">
                      <td class="stiJsViewerClearAllStyles" colspan="2">
                        <sti-radio-button #pagesRangeAll
                           [name]="baseForm.name + 'PagesRangeAll'"
                           [groupName]="baseForm.name + 'PageRangeGroup'"
                           [caption]="model.loc('PagesRangeAll')"
                           [tooltip]="model.loc('PagesRangeAllTooltip')"
                           [margin]="'6px 8px 6px 8px'"
                           [checked]="!exportFormSettings.groups.pageRangeGroup.pageRangeAllIsDisabled"
                           [enabled]="!exportFormSettings.groups.pageRangeGroup.pageRangeAllIsDisabled"
                           (action)="exportService.exportSettings.PageRange = 'All'">
                        </sti-radio-button>
                      </td>
                    </tr>
                    <tr class="stiJsViewerClearAllStyles">
                      <td class="stiJsViewerClearAllStyles" colspan="2">
                        <sti-radio-button #pageRangeCurrentPage
                           [name]="baseForm.name + 'PageRangeCurrentPage'"
                           [groupName]="baseForm.name + 'PageRangeGroup'"
                           [caption]="model.loc('PagesRangeCurrentPage')"
                           [tooltip]="model.loc('PagesRangeCurrentPageTooltip')"
                           [margin]="'0px 8px 4px 8px'"
                           [checked]="exportFormSettings.groups.pageRangeGroup.pageRangeAllIsDisabled"
                           (action)="exportService.exportSettings.PageRange = (model.reportParams.pageNumber + 1).toString()">
                        </sti-radio-button>
                      </td>
                    </tr>
                    <tr class="stiJsViewerClearAllStyles">
                      <td class="stiJsViewerClearAllStyles" colspan="2">
                        <sti-radio-button
                           [name]="baseForm.name + 'PageRangePages'"
                           [groupName]="baseForm.name + 'PageRangeGroup'"
                           [caption]="model.loc('PagesRangePages')"
                           [tooltip]="model.loc('PagesRangePagesTooltip')"
                           [margin]="'0px 8px 0px 8px'"
                           [paddingLeftLastCell]="'60px'"
                           (action)="exportService.exportSettings.PageRange = pagesRange.element.nativeElement.value">
                              <sti-text-box #pagesRange
                                [width]="130"
                                [tooltip]="model.loc('PagesRangePagesTooltip')"
                                [margin]="'0px 0px 0px 30px'"
                                (onchange)="exportService.exportSettings.PageRange = $event.value"
                                (onblur)="exportService.exportSettings.PageRange = $event.value">
                              </sti-text-box>
                        </sti-radio-button>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </sti-group-panel>
            </td>
          </tr>

        <tr *ngIf="exportFormSettings?.groups.settingsGroup.visible"class="stiJsViewerClearAllStyles">
            <td class="stiJsViewerClearAllStyles" colspan="2">
              <sti-group-panel [caption]="model.loc('SettingsGroup')"
                [opened]="exportFormSettings.groups.settingsGroup.opened"
                [width]="390"
                [margin]="'4px'"
                [innerPadding]="'4px 0 4px 0'"
                (action)="exportFormSettings.groups.settingsGroup.opened=$event">
                <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="width: 100%;">
                  <tbody>
                    <tr *ngFor="let comp of exportFormSettings.components">
                      <td *ngIf="comp.label != null" style="padding: 0px 8px; min-width: 150px;" [attr.title]="comp.tooltip">
                        {{comp.label}}
                      </td>

                      <td *ngIf="comp.name == 'UseDigitalSignature'">
                        <sti-check-box
                           [tooltip]="comp.tooltip"
                           [margin]="comp.margin"
                           [captionText]="comp.caption"
                           [isChecked]="getValue(comp)"
                           (action)="componentAction(comp, $event)">
                        </sti-check-box>
                      </td>

                      <td [attr.colSpan]="comp.label != null || comp.name == 'DocumentSecurityButton' || comp.name == 'UseDigitalSignature' ? 1 : 2">
                        <sti-drop-down-list-for-export-form *ngIf="comp.type == 'DropDownListForExportForm'"
                           [width]="comp.width"
                           [items]="comp.items"
                           [key]="getValue(comp, 'dropDownList')"
                           [margin]="comp.margin"
                           [verticalAlign]="comp.name == 'ImageQuality' || comp.name == 'ImageResolution' ? 'middle' : null"
                           [styleDisplay]="comp.name == 'ImageQuality' || comp.name == 'ImageResolution' ? 'inline-block' : null"
                           [enabled]="!disabledComponents[comp.name]"
                           (action)="componentAction(comp, $event.key)">
                        </sti-drop-down-list-for-export-form>

                        <div *ngIf="comp.name == 'ImageQuality' || comp.name == 'ImageResolution'"
                           [style.display]="'inline-block'">
                          {{comp.name == 'ImageQuality' ? '%' : 'dpi'}}
                        </div>

                        <sti-text-box *ngIf="comp.type == 'TextBox'"
                           [width]="comp.width"
                           [tooltip]="comp.tooltip"
                           [margin]="comp.margin"
                           [type]="getTextBoxType(comp.name)"
                           [enabled]="!comp.disabled"
                           [value]="getValue(comp)"
                           [enabled]="!disabledComponents[comp.name]"
                           (onchange)="componentAction(comp, $event.value)">
                        </sti-text-box>

                        <sti-check-box *ngIf="comp.type == 'CheckBox' || comp.type == 'DigitalSignature'"
                           [tooltip]="comp.tooltip"
                           [margin]="comp.margin"
                           [captionText]="comp.caption"
                           [isChecked]="getValue(comp)"
                           [isEnabled]="!disabledComponents[comp.name]"
                           (action)="componentAction(comp, $event)">
                        </sti-check-box>

                      </td>

                      <td *ngIf="comp.name == 'DocumentSecurityButton'">
                        <sti-button #button
                           [width]="comp.width"
                           [arrow]="'Down'"
                           [minWidth]="'163px'"
                           [captionAlign]="'center'"
                           [display]="'inline-block'"
                           [styleName]="'stiJsViewerFormButton'"
                           [margin]="'2px 8px'"
                           [innerTableWidth]="'100%'"
                           [captionWidth]="'100%'"
                           [caption]="comp.name == 'DocumentSecurityButton' ? comp.caption : ''"
                           [enabled]="!disabledComponents[comp.name + 'Button']"
                           (action)="showMenu(comp, button)">
                        </sti-button>
                      </td>
                    </tr>
                  </tbody>
                </table>

              </sti-group-panel>
            </td>
        </tr>

        <tr *ngIf="model.options.exports.showOpenAfterExport && exportFormSettings?.openAfterExport && !exportService.sendMail" class="stiJsViewerClearAllStyles">
            <td class="stiJsViewerClearAllStyles" colspan="2">
                <sti-check-box [captionText]="model.loc('OpenAfterExport')"
                  [tooltip]="model.loc('OpenAfterExportTooltip')"
                  [margin]="'4px 8px 4px 8px'"
                  [isChecked]="exportService.exportSettings.OpenAfterExport"
                  (action)="exportService.exportSettings.OpenAfterExport = $event">
                </sti-check-box>
            </td>
        </tr>
      </tbody>
    </table>

    </sti-base-form>
  `
})

export class ExportFormComponent implements OnInit, OnChanges {

  private PROPERTY_ALIAS = {
    ImageFormatForHtml: 'ImageFormat',
    EncodingTextOrCsvFile: 'Encoding',
    ExportModeRtf: 'ExportMode',
    EncodingDifFile: 'Encoding',
    EncodingDbfFile: 'CodePage'
  };

  @ViewChild('baseForm') baseForm: BaseFormComponent;
  @ViewChild('saveReportMdc') saveReportMdc: RadioButtonComponent;
  @ViewChild('pagesRangeAll') pagesRangeAll: RadioButtonComponent;
  @ViewChild('pageRangeCurrentPage') pageRangeCurrentPage: RadioButtonComponent;

  @Input() exportFormSettings: ExportFormSettings;

  formVisible = false;
  disabledComponents: any = {};

  constructor(public model: ModelService, public exportService: ExportService, public helper: HelperService, public formService: FormService,
    public menuService: MenuService, public collections: CollectionsService, public controller: ControllerService) { }

  ngOnChanges(changes: import('@angular/core').SimpleChanges): void {
    if (this.baseForm && this.formService?.form?.name === this.baseForm.name) {
      setTimeout(() => this.updateDisabledComponents());
    }
  }

  ngOnInit() { }

  action() {
    this.formService.closeForm(this.baseForm.name);

    if (this.model.options.exports.storeExportSettings) {
      this.helper.setCookie('StimulsoftWebViewerExportSettings' + this.exportService.getCommonExportFormat(), JSON.stringify(this.exportService.exportSettings));
    }

    this.exportService.postExport();
  }

  updateDisabledComponents() {
    ['StandardPdfFonts', 'EmbeddedFonts', 'UseUnicode'].forEach(n => this.disabledComponents[n] = (this.exportService.exportSettings.PdfACompliance === true));

    this.disabledComponents.ImageQuality = this.exportService.exportSettings.ImageCompressionMethod && this.exportService.exportSettings.ImageCompressionMethod !== 'Jpeg';

    this.disabledComponents.ExportObjectFormatting = this.exportService.exportSettings.DataExportMode === 'AllBands';
    this.disabledComponents.UseOnePageHeaderAndFooter = this.exportService.exportSettings.DataExportMode != 'AllBands';

    this.disabledComponents.EncodingDifFile = this.exportService.exportSettings.UseDefaultSystemEncoding;

    this.disabledComponents.TiffCompressionScheme = this.exportService.exportSettings.ImageType !== 'Tiff';

    this.disabledComponents.UseDigitalSignature = !this.exportService.exportSettings.UseDigitalSignature || !(this.model.pdfSecurityCertificates?.length > 0);

    this.disabledComponents.RemoveEmptySpaceAtBottom = this.exportService.exportSettings.UsePageHeadersAndFooters;

    if (this.exportService.exportSettings.ImageType != null) {
      const ifComponent = this.exportService.exportFormSettings.components.find(c => c.name === 'ImageFormat');
      if (ifComponent != null) {
        ifComponent.items = this.collections.getImageFormatItems(this.exportService.exportSettings.ImageType === 'Emf');
      }
    }

    this.disabledComponents.DitheringType = this.exportService.exportSettings.ImageFormat !== 'Monochrome';

    if (this.saveReportMdc) { this.saveReportMdc.click(); }
    if (this.pagesRangeAll && !this.exportFormSettings.groups.pageRangeGroup.pageRangeAllIsDisabled) {
      this.pagesRangeAll.click();
    }
    if (this.pageRangeCurrentPage && this.exportFormSettings.groups.pageRangeGroup.pageRangeAllIsDisabled) {
      this.pageRangeCurrentPage.click();
    }

    if (this.exportService.exportSettings.CompressToArchive === true) {
      this.exportFormSettings.groups.pageRangeGroup.pageRangeAllIsDisabled = false;
    }

    if (this.exportService.exportSettings.CompressToArchive === false) {
      this.exportFormSettings.groups.pageRangeGroup.pageRangeAllIsDisabled = true;
    }
  }

  componentAction(comp: ExportComponent, value?: any) {
    let property = this.PROPERTY_ALIAS[comp.name] || comp.name;

    switch (property) {
      case 'EmbeddedFonts':
      case 'UseUnicode':
        if (value) {
          this.exportService.exportSettings.StandardPdfFonts = false;
        }
        break;

      case 'StandardPdfFonts':
        if (value) {
          this.exportService.exportSettings.EmbeddedFonts = false;
          this.exportService.exportSettings.UseUnicode = false;
        }
        break;

      case 'ImageType':
        this.exportService.export('Image' + value, true);
        break;

      case 'DataType':
      case 'HtmlType':
        this.exportService.export(value, true);
        break;

      case 'ExcelType':
        const exportFormat = value === 'ExcelBinary' ? 'Excel' : value;
        this.exportService.export(exportFormat, true);
        break;

      case 'UseDigitalSignature':
        if (!(value === true || value === false)) {
          property = 'CertificateThumbprint';
        }
        if (value === true && this.collections.getPdfSecurityCertificatesItems().length == 0) {
          value = false;
          this.controller.showError('Warning', 'Certificate Not Found!');
        }
        break;

      case 'UsePageHeadersAndFooters':
        if (value === true) {
          this.exportService.exportSettings.RemoveEmptySpaceAtBottom = true;
        }
        break;
    }

    this.exportService.exportSettings[property] = value;

    this.updateDisabledComponents();
  }

  getValue(comp: ExportComponent, component?: string): string {
    const property = this.PROPERTY_ALIAS[comp.name] || comp.name;
    switch (property) {
      case 'Zoom2':
        return this.exportService.exportSettings[property].toString();

      case 'UseDigitalSignature':
        if (component == 'dropDownList') {
          return this.exportService.exportSettings['CertificateThumbprint'];
        }
      default:
        return this.exportService.exportSettings[property];
    }
  }

  changeVisibility(state: string) {
    this.formVisible = state === 'visible';

    if (state === 'hidden' && this.exportFormSettings?.groups?.savingReportGroup) {
      this.helper.setCookie('StimulsoftWebViewerExportSettingsOpeningGroups', JSON.stringify({
        SavingReportGroup: this.exportFormSettings.groups.savingReportGroup.opened,
        PageRangeGroup: this.exportFormSettings.groups.pageRangeGroup.opened,
        SettingsGroup: this.exportFormSettings.groups.settingsGroup.opened
      }));
    }
    if (state === 'visible') {
      this.updateDisabledComponents();
    }
  }

  showMenu(comp: ExportComponent, button: ButtonComponent) {
    let menuName = 'documentSecurityMenu';
    if (comp.name === 'DocumentSecurityButton') {
      this.menuService.addMenu({
        type: menuName, name: menuName, items: [], parent: button.button,
        itemStyleName: 'stiJsViewerMenuStandartItem', menuStyleName: 'stiJsViewerDropdownMenu',
        state: ''
      });
    } else {
      menuName = this.menuService.VERTICAL_MENU_NAME;
      this.menuService.addMenu({
        type: 'buttonMenu', name: menuName, items: this.collections.getPdfSecurityCertificatesItems(), parent: button.button,
        itemStyleName: 'stiJsViewerMenuStandartItem', menuStyleName: 'stiJsViewerDropdownMenu',
        state: ''
      });
    }

    setTimeout(() => {
      this.menuService.showMenu(menuName);
    });
  }

  getTextBoxType(name: string) {
    if (name === 'SaveReportPassword' || name === 'PasswordInputUser' || name === 'PasswordInputOwner') {
      return this.formVisible ? 'password' : '';
    }
    return null;
  }
}
