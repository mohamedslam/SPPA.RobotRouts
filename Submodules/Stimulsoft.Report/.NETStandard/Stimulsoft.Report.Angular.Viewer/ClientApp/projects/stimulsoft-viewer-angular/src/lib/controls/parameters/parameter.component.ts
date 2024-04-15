import { Component, OnInit, Input, ElementRef, ViewChild } from '@angular/core';
import { ModelService } from '../../services/model.service';
import { Variable } from '../../services/objects';
import { HelperService } from '../../services/helper.service';
import { MenuService } from '../../menu/menu.service';
import { InteractionsService } from '../../services/interactions.service';

@Component({
  selector: 'sti-parameter',
  template: `
    <table #element class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" #rangeFrom>
       <tbody>
          <tr class="stiJsViewerClearAllStyles" >
            <td *ngIf="params.type == 'Bool' && (params.basicType == 'Value' || params.basicType == 'NullableValue')"
                style="padding: 0px 2px;" [style.height.px]="model.options.parameterRowHeight">
                <sti-parameter-checkbox [params]="params" [isEnabled]="params.allowUserValues && !params.isNull" (action)="params.value = $event">
                </sti-parameter-checkbox>
            </td>

            <td *ngIf="params.basicType == 'Range' && model.options.appearance.parametersPanelPosition === 'Top'"  style="padding: 0px 2px;" [style.height.px]="model.options.parameterRowHeight">
                {{model.loc('RangeFrom')}}
            </td>

            <td *ngIf="params.type != 'Bool' || params.basicType == 'List'"
                style="padding: 0px 2px;" [style.height.px]="model.options.parameterRowHeight">
                <sti-parameter-text-box [variable]="params"
                    [readOnly]="getReadOnlyCheckbox()"
                    [value]="getFirstTextBoxValue()">
                </sti-parameter-text-box>
            </td>

            <td *ngIf="params.type == 'DateTime' && params.allowUserValues && params.basicType != 'List' && params.basicType != 'Range'"
                style="padding: 0px 2px;" [style.height.px]="model.options.parameterRowHeight" #firstDateTimeButton>
                <sti-parameter-button [buttonType]="'DateTimeButton'" [params]="params"  (action)="firstDateTimeAction($event)">
                </sti-parameter-button>
            </td>

            <td *ngIf="params.type == 'Guid' && params.allowUserValues && params.basicType != 'List'"
                style="padding: 0px 2px;" [style.height.px]="model.options.parameterRowHeight">
                <sti-parameter-button  [buttonType]="'GuidButton'" [params]="params" (action)="firstGuidAction()">
                </sti-parameter-button>
            </td>

            <td *ngIf="!showParameterInTwoRows && params.basicType == 'Range' && model.options.appearance.parametersPanelPosition === 'Top'" style="padding: 0px 2px;" [style.height.px]="model.options.parameterRowHeight">
              {{model.loc('RangeTo')}}
            </td>

            <!-- second -->
            <td *ngIf="!showParameterInTwoRows && params.basicType == 'Range'" style="padding: 0px 2px;" [style.height.px]="model.options.parameterRowHeight">
              <sti-parameter-text-box [variable]="params"
                    [readOnly]="!params.allowUserValues"
                    [value]="helper.getStringKey(params.keyTo, params)"
                    [secondTextBox]="true">
              </sti-parameter-text-box>
            </td>

            <td *ngIf="!showParameterInTwoRows && params.basicType == 'Range' && params.type == 'DateTime' && params.allowUserValues" #doubleDateTimeButton
              style="padding: 0px 2px;" [style.height.px]="model.options.parameterRowHeight">
              <sti-parameter-button  [buttonType]="'DateTimeButton'" [params]="params" (action)="doubleDateTimeAction($event)">
              </sti-parameter-button>
            </td>

            <td *ngIf="!showParameterInTwoRows && params.basicType == 'Range' && params.type == 'Guid' && params.allowUserValues"
              style="padding: 0px 2px;" [style.height.px]="model.options.parameterRowHeight">
              <sti-parameter-button  [buttonType]="'GuidButton'" [params]="params" (action)="params.keyTo=helper.newGuid()">
              </sti-parameter-button>
            </td>

            <td *ngIf="!showParameterInTwoRows && params.items != null || (params.basicType == 'List' && params.allowUserValues)"
                style="padding: 0px 2px;" [style.height.px]="model.options.parameterRowHeight">
              <sti-parameter-button [buttonType]="'DropDownButton'" [params]="params" (action)="dropDownButtonAction($event)">
              </sti-parameter-button>
            </td>

            <td *ngIf="!showParameterInTwoRows && params.basicType == 'NullableValue' && params.allowUserValues"
                style="padding: 0px 2px;" [style.height.px]="model.options.parameterRowHeight">
                <sti-parameter-checkbox [params]="params" [isEnabled]="params.allowUserValues" (action)="nullableAction($event)" [nullable]="true" >
                </sti-parameter-checkbox>
            </td>

            <td *ngIf="!showParameterInTwoRows && params.basicType == 'NullableValue' && params.allowUserValues"
                style="padding: 0px" [style.height.px]="model.options.parameterRowHeight">
                {{model.loc('Null')}}
            </td>
          </tr>

          <!-- NEW LINE -->
          <tr *ngIf="showParameterInTwoRows">
            <td *ngIf="params.basicType == 'Range' && model.options.appearance.parametersPanelPosition === 'Top'" style="padding: 0px 2px;" [style.height.px]="model.options.parameterRowHeight">
              {{model.loc('RangeTo')}}
            </td>

            <!-- second -->
            <td *ngIf="params.basicType == 'Range'" style="padding: 0px 2px;" [style.height.px]="model.options.parameterRowHeight">
              <sti-parameter-text-box [variable]="params"
                    [readOnly]="!params.allowUserValues"
                    [value]="helper.getStringKey(params.keyTo, params)"
                    [secondTextBox]="true">
              </sti-parameter-text-box>
            </td>

            <td *ngIf="params.basicType == 'Range' && params.type == 'DateTime' && params.allowUserValues" #doubleDateTimeButton
              style="padding: 0px 2px;" [style.height.px]="model.options.parameterRowHeight">
              <sti-parameter-button  [buttonType]="'DateTimeButton'" [params]="params" (action)="doubleDateTimeAction($event)">
              </sti-parameter-button>
            </td>

            <td *ngIf="params.basicType == 'Range' && params.type == 'Guid' && params.allowUserValues"
              style="padding: 0px 2px;" [style.height.px]="model.options.parameterRowHeight">
              <sti-parameter-button  [buttonType]="'GuidButton'" [params]="params" (action)="params.keyTo=helper.newGuid()">
              </sti-parameter-button>
            </td>

            <td *ngIf="params.items != null || (params.basicType == 'List' && params.allowUserValues)"
                style="padding: 0px 2px;" [style.height.px]="model.options.parameterRowHeight">
              <sti-parameter-button [buttonType]="'DropDownButton'" [params]="params" (action)="dropDownButtonAction($event)">
              </sti-parameter-button>
            </td>

            <td *ngIf="params.basicType == 'NullableValue' && params.allowUserValues"
                style="padding: 0px 2px;" [style.height.px]="model.options.parameterRowHeight">
                <sti-parameter-checkbox [params]="params" [isEnabled]="params.allowUserValues" (action)="nullableAction($event)" [nullable]="true" >
                </sti-parameter-checkbox>
            </td>

            <td *ngIf="params.basicType == 'NullableValue' && params.allowUserValues"
                style="padding: 0px" [style.height.px]="model.options.parameterRowHeight">
                {{model.loc('Null')}}
            </td>

          </tr>
       </tbody>
    </table>
  `
})

export class ParameterComponent implements OnInit {

  @ViewChild('element') element: ElementRef;
  @ViewChild('firstDateTimeButton') firstDateTimeButton: ElementRef;
  @ViewChild('doubleDateTimeButton') doubleDateTimeButton: ElementRef;
  @ViewChild('rangeFrom') rangeFrom: ElementRef;

  private _params: Variable;

  constructor(public model: ModelService, public helper: HelperService, public menuService: MenuService, public intearctionService: InteractionsService) { }

  ngOnInit() { }

  @Input() get params(): Variable {
    return this._params;
  }

  set params(params: Variable) {
    this._params = params;

    if (params.basicType === 'Range') {
      if (params.type === 'DateTime' && params.keyTo && params.keyTo.isNull) {
        params.keyTo = this.helper.getDateTimeObject(new Date());
      }
    }
  }

  get showParameterInTwoRows(): boolean {
    return this.params.basicType === 'Range' && this.model.options.appearance.parametersPanelPosition === 'Left';
  }

  getReadOnlyCheckbox(): boolean {
    return this.params.basicType === 'List' || !this.params.allowUserValues;
  }

  getFirstTextBoxValue(): string {
    let value = '';
    if (this.params.basicType === 'Value' || this.params.basicType === 'NullableValue') {
      if (this.params.type === 'DateTime' && this.params.value === null) {
        this.params.value = new Date();
        this.params.key = this.helper.getDateTimeObject(this.params.value);
      }

      value = (this.params.type === 'DateTime') ? this.helper.getStringKey(this.params.key, this.params) : this.params.value;
    }

    // Range
    if (this.params.basicType === 'Range') {
      if (this.params.type === 'DateTime' && this.params.key && this.params.key.isNull) {
        this.params.key = this.helper.getDateTimeObject(new Date());
      }
      value = this.helper.getStringKey(this.params.key, this.params);
    }

    // List
    if (this.params.basicType === 'List' && this.params.items) {
      this.params.items.forEach((item) => {
        if (item.isChecked) {
          if (value !== '') {
            value += (this.model.options.listSeparator ? this.model.options.listSeparator + " " : "; ");
          }

          if (this.params.allowUserValues) {
            value += this.helper.getStringKey(item.key, this.params);
          } else {
            value += item.value !== '' ? item.value : this.helper.getStringKey(item.key, this.params);
          }
        }
      });
    }

    return value != null ? value.toString() : value;
  }

  nullableAction(checked: boolean) {
    this.params.isNull = !this.params.isNull;
  }

  firstGuidAction() {
    if (this.params.basicType === 'Range') {
      this.params.key = this.helper.newGuid();
    } else {
      this.params.value = this.helper.newGuid();
    }
  }

  doubleDateTimeAction(event: any) {
    this.menuService.addMenu({
      type: 'doubleDatePickerMenu', name: 'doubleDatePickerMenu', items: [], parent: this.rangeFrom,
      params: this.params,
      state: ''
    });

    setTimeout(() => {
      this.menuService.showMenu('doubleDatePickerMenu');
    });
  }

  firstDateTimeAction(event: any) {
    this.menuService.addMenu({
      type: 'datePickerMenu', name: 'datePickerMenu', items: [], parent: this.firstDateTimeButton,
      params: this.params,
      state: ''
    });

    setTimeout(() => {
      this.menuService.showMenu('datePickerMenu');
    });
  }

  dropDownButtonAction(event: any) {
    let menuType = 'parameterMenuForValue';
    switch (this.params.basicType) {
      case 'Range':
        menuType = 'parameterMenuForRange';
        break;
      case 'List':
        menuType = this.params.allowUserValues ? 'parameterMenuForEditList' : 'parameterMenuForNotEditList';
        break;
    }

    if (this.params.items) {
      this.params.items.forEach(i => i.visible = true);
    }

    let this_ = this;

    this.menuService.addMenu({
      type: menuType, name: 'parameterMenu', items: [], parent: this.element,
      params: this.params,
      state: '',
      onCloseEvent: () => {
        this_.onCloseMenuEvent();
      }
    });

    setTimeout(() => {
      this.menuService.showMenu('parameterMenu');
    });
  }

  onCloseMenuEvent() {
    if (this.params.binding) {
      if (!this.model.options.paramsVariablesStartValues) {
        this.model.options.paramsVariablesStartValues = this.helper.copyObject(this.model.options.paramsVariables);
      }
      this.intearctionService.postInteraction({ action: 'InitVars', variables: this.intearctionService.getParametersValues(), isBindingVariable: true });
    }
  }
}
