import { NgModule } from '@angular/core';
import { StimulsoftViewerComponent } from './stimulsoft-viewer-angular.component';
import { StiHttpClientService } from './services/http-client.service';
import { HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { ButtonComponent } from './controls/button.component';
import { PageComponent } from './components/page.component';
import { ReportPanelComponent } from './components/report-panel.component';
import { ModelService } from './services/model.service';
import { ControllerService } from './services/controller.service';
import { StylesService } from './services/styles.service';
import { ToolbarComponent } from './components/toolbar.component';
import { ToolbarSeparatorComponent } from './controls/toolbar-separator.component';
import { HelperService } from './services/helper.service';
import { PageControlComponent } from './controls/page-control.component';
import { TextBoxComponent } from './controls/text-box.componet';
import { MenuComponent } from './menu/menu.component';
import { MenuItemComponent } from './menu/meni-item.component';
import { MenuService } from './menu/menu.service';
import { MouseService } from './services/mouse.service';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { PageService } from './services/page.service';
import { TooltipComponent } from './controls/tooltip.component';
import { TooltipService } from './services/tooltip.service';
import { FindPanelComponent } from './components/find-panel.component';
import { TextBlockComponent } from './controls/text-block.component';
import { FindService } from './services/find.service';
import { AnimationService } from './services/animation.service';
import { BookmarksPanelComponent } from './components/bookmarks-panel.component';
import { Md5Service } from './services/md5.service';
import { InteractionsService } from './services/interactions.service';
import { InteractionsPanelComponent } from './components/interactions-panel.component';
import { FormButtonComponent } from './controls/form-button.component';
import { ParameterComponent } from './controls/parameters/parameter.component';
import { ParameterTextBoxComponent } from './controls/parameters/parameter-text-box.component';
import { CheckboxComponent } from './controls/checkbox.component';
import { ParameterCheckboxComponent } from './controls/parameters/parameter-checkbox.component';
import { ParameterButtonComponent } from './controls/parameters/parameter-button.component';
import { ParameterMenuEditListComponent } from './menu/parameter-menu-edit-list.component';
import { ParameterMenuItemComponent } from './menu/parameter-menu-item.component';
import { ParameterMenuSeparatorComponent } from './menu/parameter-menu-separator.component';
import { ParameterFindComponent } from './controls/parameters/parameter-find.component';
import { ParameterMenuNotEditListComponent } from './menu/parameter-menu-not-edit-list.component';
import { ParameterMenuForValueComponent } from './menu/parameter-menu-for-value.component';
import { ParameterMenuForRangeComponent } from './menu/parameter-menu-for-range.component';
import { DatePickerMenuComponent } from './menu/date-picker-menu.component';
import { DropDownListComponent } from './controls/drop-down-list.component';
import { VerticalMenuComponent } from './menu/vertical-menu.component';
import { VerticalMenuItemComponent } from './menu/vertical-menu-item.component';
import { DatePickerDayButtonComponent } from './controls/date-picker-day-button.component';
import { DoubleDatePickerMenuComponent } from './menu/double-date-picker-menu.component';
import { BaseFormComponent } from './forms/base-form.component';
import { FormService } from './forms/form.service';
import { ExportFormComponent } from './forms/export-form.component';
import { GroupPanelComponent } from './forms/group-panel.component';
import { RadioButtonComponent } from './controls/radio-button.components';
import { RadioButtonService } from './services/radio-button.service';
import { ExportService } from './services/export.service';
import { DropDownListForExportFormComponent } from './controls/drop-down-list-for-export-form.component';
import { CollectionsService } from './services/collections.service';
import { DocumentSecurityMenuComponent } from './menu/document-security-menu.component';
import { DigitalSignatureMenuComponent } from './menu/digital-signature-menu.component copy';
import { SendEmailFormComponent } from './forms/send-email-form.component';
import { MailService } from './services/mail.service';
import { TextAreaComponent } from './controls/text-area.component';
import { EditableFieldsService } from './services/editable-fields.service';
import { DrillDownPanelComponent } from './components/drill-down-panel.component';
import { DrillDownService } from './services/drill-down.service';
import { PrintService } from './services/print.service';
import { OpenDialogComponent } from './components/open-dialog.component';
import { PasswordFormComponent } from './forms/password-form.component';
import { ErrorMessageFormComponent } from './forms/error-message-form.component';
import { ResourcesPanelComponent } from './components/resources-panel.component';
import { FullScreenService } from './services/full-screen.service';
import { ProgressComponent } from './components/progress.component';
import { NavigatePanelComponent } from './components/navigate-panel.component';
import { AboutPanelComponent } from './components/about-panel.component';
import { ToolbarDopComponent } from './components/toolbar-dop.component';
import { ToolbarService } from './services/toolbar.service';
import { CenterTextComponent } from './components/center-text.component';
import { NotificationFormComponent } from './forms/notification-form.component';
import { ApiService } from './services/api.service';
import { DashboardsPanelComponent } from './components/dashboards-panel.component';
import { DashboardService } from './services/dashboard.service';
import { DashboardButtonComponent } from './controls/dashboard-panel-button.component';

@NgModule({
  declarations: [StimulsoftViewerComponent, ToolbarComponent, ButtonComponent, PageComponent, ReportPanelComponent, ToolbarSeparatorComponent, PageControlComponent, TextBoxComponent,
    MenuComponent, MenuItemComponent, TooltipComponent, FindPanelComponent, TextBlockComponent, BookmarksPanelComponent, InteractionsPanelComponent, FormButtonComponent,
    ParameterComponent, ParameterTextBoxComponent, CheckboxComponent, ParameterCheckboxComponent, ParameterButtonComponent, ParameterMenuEditListComponent,
    ParameterMenuItemComponent, ParameterMenuSeparatorComponent, ParameterFindComponent, ParameterMenuNotEditListComponent, ParameterMenuForValueComponent,
    ParameterMenuForRangeComponent, DatePickerMenuComponent, DropDownListComponent, VerticalMenuComponent, VerticalMenuItemComponent, DatePickerDayButtonComponent,
    DoubleDatePickerMenuComponent, BaseFormComponent, ExportFormComponent, GroupPanelComponent, RadioButtonComponent, DropDownListForExportFormComponent,
    DocumentSecurityMenuComponent, DigitalSignatureMenuComponent, SendEmailFormComponent, TextAreaComponent, DrillDownPanelComponent, OpenDialogComponent,
    PasswordFormComponent, ErrorMessageFormComponent, ResourcesPanelComponent, ProgressComponent, NavigatePanelComponent, AboutPanelComponent, ToolbarDopComponent,
    CenterTextComponent, NotificationFormComponent, DashboardsPanelComponent, DashboardButtonComponent],
  imports: [
    CommonModule
  ],
  providers: [],
  exports: [StimulsoftViewerComponent]
})
export class StimulsoftViewerModule { }
