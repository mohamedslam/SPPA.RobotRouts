import { Component, OnInit, Input } from '@angular/core';
import { ModelService } from '../services/model.service';
import { DateTimeObject } from '../services/objects';
import { HelperService } from '../services/helper.service';
import { MenuService } from '../menu/menu.service';

@Component({
  selector: 'sti-date-picker-day-button',
  template: `
    <sti-button [styleName]="'stiJsViewerDatePickerDayButton'"
                [width]="model.options.isTouchDevice ? '25px' : '23px'"
                [height]="model.options.isTouchDevice ? '25px' : '23px'"
                [captionAlign]="'center'"
                [innerTableWidth]="'100%'"
                [captionPadding]="'0px'"
                [margin]="'1px'"
                [caption]="caption"
                [enabled]="enabled"
                [selected]="selected"
                (action)="action()">
    </sti-button>
  `
})

export class DatePickerDayButtonComponent implements OnInit {

  @Input() col: number;
  @Input() row: number;
  @Input() caption = '';
  @Input() enabled = false;
  @Input() selected = false;
  @Input() date: DateTimeObject;
  @Input() closeOnAction = true;

  constructor(public model: ModelService, public helper: HelperService, public menuService: MenuService) { }

  action() {
    this.date.day = this.row * 7 + this.col - this.getFirstDay() + 1;
    if (this.closeOnAction && this.menuService.getMenu('datePickerMenu') != null) {
      this.menuService.getMenu('datePickerMenu').state = 'initialUp';
    }
  }

  getFirstDay(): number {
    let firstDay = new Date(this.date.year, this.date.month - 1, 1).getDay();
    if (firstDay === 0) {
      firstDay = 7;
    }
    if (this.model.options.appearance.datePickerFirstDayOfWeek !== 'Sunday') {
      firstDay--;
    }
    return firstDay;
  }

  ngOnInit() { }
}
