import { Component, OnInit, Input } from '@angular/core';
import { ModelService } from '../../services/model.service';
import { Variable } from '../../services/objects';
import { HelperService } from '../../services/helper.service';

@Component({
  selector: 'sti-parameter-find',
  template: `
      <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0">
          <tbody>
            <tr class="stiJsViewerClearAllStyles">
              <td class="stiJsViewerClearAllStyles" style="padding-left: 8px;">
                <sti-text-block [text]="model.loc('FindWhat')"></sti-text-block>
              </td>

              <td class="stiJsViewerClearAllStyles" style="padding-left: 8px;">
                <sti-text-box [width]="80"
                  [margin]="'4px'"
                  [focusOnCreate]="true"
                  (onchange)="onchange($event)">
                </sti-text-box>
              </td>
            </tr>
          </tbody>
      </table>

  `
})

export class ParameterFindComponent implements OnInit {

  @Input() variable: Variable;

  constructor(public model: ModelService, public helper: HelperService) { }

  ngOnInit() { }

  onchange(event: any) {
    const text = event.value.toLowerCase();
    this.variable.items.forEach((item) => {
      const itemText = this.helper.val(item.value, this.helper.getStringKey(item.key, this.variable)).toLowerCase();
      item.visible = itemText.indexOf(text) >= 0;
    });
  }
}
