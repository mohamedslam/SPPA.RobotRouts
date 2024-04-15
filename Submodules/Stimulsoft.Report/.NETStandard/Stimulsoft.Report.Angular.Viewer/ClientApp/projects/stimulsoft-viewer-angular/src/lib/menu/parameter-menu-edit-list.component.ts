import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { ModelService } from '../services/model.service';
import { MenuItem } from './meni-item.component';
import { Menu, MenuService } from './menu.service';
import { HelperService } from '../services/helper.service';
import { Variable } from '../services/objects';

@Component({
  selector: 'sti-parameter-menu-edit-list',
  template: `
        <table class="stiJsViewerClearAllStyles stiJsViewerParametersMenuInnerTable" cellpadding="0" cellspacing="0"
        [style.fontFamily]="model.options.toolbar.fontFamily"
        [style.color]="model.options.toolbar.fontColor"
        [style.fontSize]="'12px'"
        [style.width]="(menu.parent.nativeElement.offsetWidth - 5) + 'px'">
          <tbody>
            <tr class="stiJsViewerClearAllStyles">
              <td class="stiJsViewerClearAllStyles">
                <!--New Item Button -->
                <sti-parameter-menu-item (action)="newItem($event)">
                  {{model.loc('NewItem')}}
                </sti-parameter-menu-item>
              </td>
            </tr>
            <tr class="stiJsViewerClearAllStyles">
              <td class="stiJsViewerClearAllStyles">
                <sti-parameter-menu-item *ngFor="let item of menu.params.items" [active]="false">
                  <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0">
                    <tbody>
                      <tr class="stiJsViewerClearAllStyles">
                         <td class="stiJsViewerClearAllStyles" style="padding: 0px 1px 0px 0px;">
                            <sti-parameter-text-box [item]="item" [isMenu]="true"
                              [variable]="menu.params" [focusOnCreate]="item.focusOnCreate">
                            </sti-parameter-text-box>
                         </td>

                         <td *ngIf="menu.params.type == 'DateTime'" class="stiJsViewerClearAllStyles" style="padding: 0 1px 0 1px;" #dateTimeButton>
                           <sti-parameter-button [buttonType]="'DateTimeButton'" (action)="dateTimeButtonAction(item)">
                           </sti-parameter-button>
                         </td>

                         <td *ngIf="menu.params.type == 'Guid'" class="stiJsViewerClearAllStyles" style="padding: 0 1px 0 1px;">
                           <sti-parameter-button [buttonType]="'GuidButton'" (action)="guidButtonAction(item)">
                           </sti-parameter-button>
                         </td>

                         <td class="stiJsViewerClearAllStyles" style="padding: 0 1px 0 1px;">
                           <sti-parameter-button [buttonType]="'RemoveItemButton'" (action)="removeButtonAction(item)" [params]="item">
                           </sti-parameter-button>
                         </td>
                       </tr>
                     </tbody>
                  </table>
                </sti-parameter-menu-item>
              </td>
            </tr>

            <tr class="stiJsViewerClearAllStyles">
              <td class="stiJsViewerClearAllStyles">
                  <sti-parameter-menu-item (action)="removeAllAction()">
                      {{model.loc('RemoveAll')}}
                  </sti-parameter-menu-item>

                  <sti-parameter-menu-separator></sti-parameter-menu-separator>

                  <sti-parameter-menu-item (action)="closeAction()">
                      {{model.loc('Close')}}
                  </sti-parameter-menu-item>
              </td>
            </tr>
          </tbody>
        </table>
  `
})

export class ParameterMenuEditListComponent implements OnInit {

  @ViewChild('dateTimeButton') dateTimeButton: ElementRef;

  @Input() menu: Menu;

  constructor(public model: ModelService, public helper: HelperService, public menuService: MenuService) { }

  ngOnInit() { }

  newItem(event: any) {
    const item: any = new Variable();
    if (this.menu.params.type === 'DateTime') {
      item.key = this.helper.getDateTimeObject();
      item.value = this.helper.dateTimeObjectToString(item.key);
    } else if (this.menu.params.type === 'TimeSpan') {
      item.key = '00:00:00';
      item.value = '00:00:00';
    } else if (this.menu.params.type === 'Bool') {
      item.key = 'False';
      item.value = 'False';
    } else {
      item.key = '';
      item.value = '';
    }
    item.focusOnCreate = true;
    if (!this.menu.params.items) {
      this.menu.params.items = [];
    }
    this.menu.params.items.push(item);
    setTimeout(() => {
      item.focusOnCreate = false;
    });
  }

  dateTimeButtonAction(item: Variable) {
    this.menuService.addMenu({
      type: 'datePickerMenu', name: 'datePickerMenu', items: [], parent: this.dateTimeButton,
      params: item,
      state: ''
    });

    setTimeout(() => {
      this.menuService.showMenu('datePickerMenu');
    });
  }

  guidButtonAction(item: Variable) {
    item.key = this.helper.newGuid();
  }

  removeButtonAction(item: Variable) {
    this.menu.params.items.splice(this.menu.params.items.indexOf(item), 1);
  }

  removeAllAction() {
    this.menu.params.items.splice(0, this.menu.params.items.length);
  }

  closeAction() {
    this.menu.state = 'initialDown';
  }
}
