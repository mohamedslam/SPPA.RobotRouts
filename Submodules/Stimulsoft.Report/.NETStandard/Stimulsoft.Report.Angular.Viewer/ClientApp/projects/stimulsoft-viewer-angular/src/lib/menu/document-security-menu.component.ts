import { Component, OnInit, ViewChild } from '@angular/core';
import { ModelService } from '../services/model.service';
import { FormService } from '../forms/form.service';
import { ExportService } from '../services/export.service';
import { CollectionsService } from '../services/collections.service';
import { CheckboxComponent } from '../controls/checkbox.component';

@Component({
  selector: 'sti-document-security-menu',
  template: `
    <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="width: 100%;">
                  <tbody>
                    <tr class="stiJsViewerClearAllStyles">
                      <td style="padding: 0px 8px; min-width: 150px;" [attr.title]="model.loc('UserPasswordTooltip')">
                        {{model.loc('UserPassword')}}
                      </td>
                      <td class="stiJsViewerClearAllStyles">
                        <sti-text-box
                           [width]="160"
                           [tooltip]="model.loc('UserPasswordTooltip')"
                           [margin]="'8px 8px 2px 8px'"
                           [type]="getTextBoxType()"
                           [value]="exportService.exportSettings?.PasswordInputUser"
                           (onchange)="exportService.exportSettings.PasswordInputUser =  $event.value">
                        </sti-text-box>
                      </td>
                    </tr>

                    <tr class="stiJsViewerClearAllStyles" [attr.title]="model.loc('OwnerPasswordTooltip')">
                      <td style="padding: 0px 8px; min-width: 150px;">
                        {{model.loc('OwnerPassword')}}
                      </td>
                      <td class="stiJsViewerClearAllStyles">
                        <sti-text-box
                           [width]="160"
                           [tooltip]="model.loc('OwnerPasswordTooltip')"
                           [margin]="'2px 8px 2px 8px'"
                           [type]="getTextBoxType()"
                           [value]="exportService.exportSettings?.PasswordInputOwner"
                           (onchange)="exportService.exportSettings.PasswordInputOwner =  $event.value">
                        </sti-text-box>
                      </td>
                    </tr>

                    <tr class="stiJsViewerClearAllStyles">
                      <td class="stiJsViewerClearAllStyles" colspan="2">
                        <sti-check-box #AllowPrintDocument
                           [captionText]="model.loc('AllowPrintDocument')"
                           [tooltip]="model.loc('AllowPrintDocumentTooltip')"
                           [margin]="'4px 8px 4px 8px'"
                           [isChecked]="getUserAccessPrivileges('PrintDocument')"
                           (action)="updateUserAccessPrivileges()">
                        </sti-check-box>
                      </td>
                    </tr>

                    <tr class="stiJsViewerClearAllStyles">
                      <td class="stiJsViewerClearAllStyles" colspan="2">
                        <sti-check-box #AllowModifyContents
                           [captionText]="model.loc('AllowModifyContents')"
                           [tooltip]="model.loc('AllowModifyContentsTooltip')"
                           [margin]="'4px 8px 4px 8px'"
                           [isChecked]="getUserAccessPrivileges('ModifyContents')"
                           (action)="updateUserAccessPrivileges()">
                        </sti-check-box>
                      </td>
                    </tr>

                    <tr class="stiJsViewerClearAllStyles">
                      <td class="stiJsViewerClearAllStyles" colspan="2">
                        <sti-check-box #AllowCopyTextAndGraphics
                           [captionText]="model.loc('AllowCopyTextAndGraphics')"
                           [tooltip]="model.loc('AllowCopyTextAndGraphicsTooltip')"
                           [margin]="'4px 8px 4px 8px'"
                           [isChecked]="getUserAccessPrivileges('CopyTextAndGraphics')"
                           (action)="updateUserAccessPrivileges()">
                        </sti-check-box>
                      </td>
                    </tr>

                    <tr class="stiJsViewerClearAllStyles">
                      <td class="stiJsViewerClearAllStyles" colspan="2">
                        <sti-check-box #AllowAddOrModifyTextAnnotations
                           [captionText]="model.loc('AllowAddOrModifyTextAnnotations')"
                           [tooltip]="model.loc('AllowAddOrModifyTextAnnotationsTooltip')"
                           [margin]="'4px 8px 4px 8px'"
                           [isChecked]="getUserAccessPrivileges('AddOrModifyTextAnnotations')"
                           (action)="updateUserAccessPrivileges()">
                        </sti-check-box>
                      </td>
                    </tr>
                    <tr class="stiJsViewerClearAllStyles">
                      <td class="stiJsViewerClearAllStyles" style="padding: 0px 8px; min-width: 150px;" [attr.title]="model.loc('EncryptionKeyLengthTooltip')">
                        {{model.loc('EncryptionKeyLength')}}
                      </td>
                      <td>
                        <sti-drop-down-list-for-export-form
                           [width]="160"
                           [items]="collections.getEncryptionKeyLengthItems()"
                           [key]="exportService.exportSettings?.KeyLength"
                           [margin]="'2px 8px 4px 8px'"
                           (action)="exportService.exportSettings.KeyLength = $event.key">
                        </sti-drop-down-list-for-export-form>
                      </td>
                    </tr>
                  </tbody>
                </table>
`
})

export class DocumentSecurityMenuComponent implements OnInit {

  @ViewChild('AllowPrintDocument') allowPrintDocument: CheckboxComponent;
  @ViewChild('AllowModifyContents') allowModifyContents: CheckboxComponent;
  @ViewChild('AllowCopyTextAndGraphics') allowCopyTextAndGraphics: CheckboxComponent;
  @ViewChild('AllowAddOrModifyTextAnnotations') allowAddOrModifyTextAnnotations: CheckboxComponent;

  constructor(public model: ModelService, public formService: FormService, public exportService: ExportService, public collections: CollectionsService) { }

  ngOnInit() { }

  getTextBoxType() {
    return this.formService.form?.name === 'exportForm' ? 'password' : '';
  }

  updateUserAccessPrivileges() {
    const privileges = [];
    if (this.allowPrintDocument.isChecked) { privileges.push('PrintDocument'); }
    if (this.allowModifyContents.isChecked) { privileges.push('ModifyContents'); }
    if (this.allowCopyTextAndGraphics.isChecked) { privileges.push('CopyTextAndGraphics'); }
    if (this.allowAddOrModifyTextAnnotations.isChecked) { privileges.push('AddOrModifyTextAnnotations'); }
    this.exportService.exportSettings.UserAccessPrivileges = privileges.join(',');
  }

  getUserAccessPrivileges(name: string) {
    return this.exportService.exportSettings?.UserAccessPrivileges === 'All' || this.exportService.exportSettings?.UserAccessPrivileges?.indexOf(name) >= 0;
  }
}
