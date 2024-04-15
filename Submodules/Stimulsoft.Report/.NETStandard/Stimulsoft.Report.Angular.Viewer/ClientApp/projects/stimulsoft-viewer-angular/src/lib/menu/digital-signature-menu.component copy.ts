import { Component, OnInit, ViewChild } from '@angular/core';
import { ModelService } from '../services/model.service';
import { ExportService } from '../services/export.service';

@Component({
  selector: 'sti-digital-signature-menu',
  template: `
    <div *ngFor="let item of model.pdfSecurityCertificates" class="stiJsViewerMenuStandartItem stiJsViewerMenuStandartItemSelected" style="height: auto; line-height: 1.3;">
      <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="height: 100%; width: 100%;">
        <tbody>
          <tr class="stiJsViewerClearAllStyles">
            <td class="stiJsViewerClearAllStyles" style="padding: 8px 20px 8px 8px; text-align: left; white-space: nowrap;">
            </td>
          </tr>
        </tbody>
      </table>
    </div>
`
})

export class DigitalSignatureMenuComponent implements OnInit {

  constructor(public model: ModelService, public exportService: ExportService) { }

  ngOnInit() { }

}
