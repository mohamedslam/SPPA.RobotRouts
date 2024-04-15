import { Component, OnInit, Input } from '@angular/core';
import { Menu } from './menu.service';
import { ModelService } from '../services/model.service';
import { HelperService } from '../services/helper.service';
import { Variable } from '../services/objects';

@Component({
  selector: 'sti-parameter-menu-not-edit-list',
  template: `
      <sti-parameter-find *ngIf="menu.params.items.length > 10" [variable]="menu.params"></sti-parameter-find>

      <sti-parameter-menu-separator *ngIf="menu.params.items.length > 10"></sti-parameter-menu-separator>

      <div style="max-height: 400px; overflow: hidden auto;">
        <table class="stiJsViewerClearAllStyles stiJsViewerParametersMenuInnerTable" cellpadding="0" cellspacing="0"
          [style.fontFamily]="model.options.toolbar.fontFamily"
          [style.color]="model.options.toolbar.fontColor"
          [style.fontSize]="'12px'"
          [style.width]="(menu.parent.nativeElement.offsetWidth - 5) + 'px'">
            <tbody>
              <tr class="stiJsViewerClearAllStyles">
                <td class="stiJsViewerClearAllStyles">
                  <sti-check-box [captionText]="model.loc('SelectAll')"
                    [margin]="'8px 7px 8px 7px'"
                    [isChecked]="isAllSelected()"
                    (action)="selectAll()" >
                  </sti-check-box>
                </td>
              </tr>

              <tr class="stiJsViewerClearAllStyles">
                <td class="stiJsViewerClearAllStyles">
                  <sti-parameter-menu-separator></sti-parameter-menu-separator>
                </td>
              </tr>

              <tr *ngFor="let item of menu.params.items" class="stiJsViewerClearAllStyles" [style.display]="item.visible ? '' : 'none'">
                <td class="stiJsViewerClearAllStyles">
                  <sti-parameter-menu-item>
                      <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="width: 100%">
                          <tbody>
                            <tr class="stiJsViewerClearAllStyles">
                              <td class="stiJsViewerClearAllStyles">
                                  <sti-parameter-checkbox [params]="item"
                                      [captionText]="helper.val(item.value, helper.getStringKey(item.key, menu.params))"
                                      [margin]="'0 5px 0 0'"
                                      [width]="'100%'"
                                      [imageBlockParentWidth]="'1px'"
                                      [isMenuParameter]="true"
                                      (action)="check(item)">
                                  </sti-parameter-checkbox>
                              </td>
                            </tr>
                          </tbody>
                      </table>
                  </sti-parameter-menu-item>
                </td>
              </tr>

              <tr class="stiJsViewerClearAllStyles">
                <td class="stiJsViewerClearAllStyles">
                  <sti-parameter-menu-separator></sti-parameter-menu-separator>
                </td>
              </tr>

              <tr class="stiJsViewerClearAllStyles">
                <td class="stiJsViewerClearAllStyles">
                    <sti-parameter-menu-item [padding]="'0px 5px 0px 13px'" (action)="closeAction()">
                      {{model.loc('Close')}}
                    </sti-parameter-menu-item>
                </td>
              </tr>
            </tbody>
          </table>
      </div>
  `
})

export class ParameterMenuNotEditListComponent implements OnInit {

  @Input() menu: Menu;

  constructor(public model: ModelService, public helper: HelperService) { }

  ngOnInit() { }

  selectAll() {
    const isAllSelected = this.isAllSelected();
    if (this.menu.params.items) {
      this.menu.params.items.forEach(e => e.isChecked = !isAllSelected);
    }
  }

  isAllSelected() {
    return this.menu.params.items.every(e => e.isChecked);
  }

  check(item: Variable) {
    item.isChecked = !item.isChecked;
  }

  closeAction() {
    this.menu.state = 'initialDown';
  }
}
