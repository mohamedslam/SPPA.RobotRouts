import { Component, OnInit, Input } from '@angular/core';
import { Menu } from './menu.service';
import { ModelService } from '../services/model.service';
import { HelperService } from '../services/helper.service';
import { Variable } from '../services/objects';
import { InteractionsService } from '../services/interactions.service';

@Component({
  selector: 'sti-parameter-menu-for-value',
  template: `
      <sti-parameter-find *ngIf="menu.params.items.length > 10" [variable]="menu.params"></sti-parameter-find>

      <sti-parameter-menu-separator *ngIf="menu.params.items.length > 10"></sti-parameter-menu-separator>

      <div style="max-height: 400px" [style.overflow]="menu.params.items.length > 10 ? 'hidden auto' : 'hidden'">
        <table class="stiJsViewerClearAllStyles stiJsViewerParametersMenuInnerTable" cellpadding="0" cellspacing="0"
          [style.fontFamily]="model.options.toolbar.fontFamily"
          [style.color]="model.options.toolbar.fontColor"
          [style.fontSize]="'12px'"
          [style.width]="(menu.parent.nativeElement.offsetWidth - 5) + 'px'">
            <tbody>
              <tr *ngFor="let item of menu.params.items" class="stiJsViewerClearAllStyles" [style.display]="item.visible ? '' : 'none'">
                <td class="stiJsViewerClearAllStyles">
                  <sti-parameter-menu-item>
                      <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="width: 100%">
                          <tbody>
                            <tr class="stiJsViewerClearAllStyles">
                              <td class="stiJsViewerClearAllStyles">
                                  <sti-parameter-menu-item (action)="action(item)">
                                    {{getValue(item)}}
                                  </sti-parameter-menu-item>
                              </td>
                            </tr>
                          </tbody>
                      </table>
                  </sti-parameter-menu-item>
                </td>
              </tr>
            </tbody>
          </table>
      </div>
  `
})

export class ParameterMenuForValueComponent implements OnInit {

  @Input() menu: Menu;

  constructor(public model: ModelService, public helper: HelperService, public intearctionService: InteractionsService) { }

  ngOnInit() { }

  action(item: Variable) {
    this.menu.params.value = this.menu.params.key = item.key;
    this.menu.state = 'initialDown';

    if (this.menu.params.binding) {
      if (!this.model.options.paramsVariablesStartValues) {
        this.model.options.paramsVariablesStartValues = this.helper.copyObject(this.model.options.paramsVariables);
      }

      this.intearctionService.postInteraction({ action: 'InitVars', variables: this.intearctionService.getParametersValues() });
    }

  }

  getValue(item: Variable) {
    return (item.value !== '' && this.menu.params.type !== 'DateTime' && this.menu.params.type !== 'TimeSpan')
      ? item.value
      : this.helper.getStringKey(item.key, this.menu.params);
  }
}
