import { Component, OnInit, Input } from '@angular/core';
import { ModelService } from '../services/model.service';
import { Menu } from './menu.service';
import { HelperService } from '../services/helper.service';
import { Item, DateTimeObject } from '../services/objects';

@Component({
  selector: 'sti-date-picker-menu',
  template: `
        <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0">
          <tbody>
            <tr class="stiJsViewerClearAllStyles">
              <td class="stiJsViewerClearAllStyles">
                <sti-button [imageName]="'Arrows.BigArrowLeft.png'" [margin]="'1px 2px 0 1px'" (action)="prevMonthButtonAction()">
                </sti-button>
              </td>

              <td class="stiJsViewerClearAllStyles">
                <sti-drop-down-list
                  [width]="model.options.isTouchDevice ? 79 : 81"
                  [readOnly]="true"
                  [margin]="'1px 2px 0 0'"
                  [items]="monthesForDatePickerItems"
                  [key]="key.month - 1"
                  (action)="key.month = $event.key + 1">
                </sti-drop-down-list>
              </td>

              <td class="stiJsViewerClearAllStyles">
                <sti-text-box [width]="40" [margin]="'1px 2px 0 0'" [value]="key.year" (action)="yearAction($event)">
                </sti-text-box>
              </td>

              <td class="stiJsViewerClearAllStyles">
                <sti-button [imageName]="'Arrows.BigArrowRight.png'" [margin]="'1px 1px 0 0'" (action)="nextMonthButtonAction()">
                </sti-button>
              </td>
            </tr>
          </tbody>
        </table>
        <div class="stiJsViewerDatePickerSeparator" style="margin: 2px 0px;"></div>

        <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0">
          <tbody>
            <tr class="stiJsViewerClearAllStyles">
              <td *ngFor="let item of model.dayOfWeek; let i = index" class="stiJsViewerDatePickerDayOfWeekCell"
                  [style.fontSize]="countLetters == 2 ? '11px' : ''"
                  [style.color]="getShortDayColor(i)">
                  {{getShortDayName(i)}}
              </td>
            </tr>
            <tr *ngFor="let row of rows" class="stiJsViewerClearAllStyles">
                <td *ngFor="let col of cols" class="stiJsViewerClearAllStyles">
                  <sti-date-picker-day-button
                      [col]="col"
                      [row]="row"
                      [date]="key"
                      [selected]="getButtonSelected(col, row)"
                      [caption]="getButtonCaption(col, row)"
                      [enabled]="getButtonCaption(col, row) != ''">
                      [closeOnAction]="closeOnAction"
                  </sti-date-picker-day-button>
                </td>
            </tr>
          </tbody>
        </table>

        <div *ngIf="showTime" class="stiJsViewerDatePickerSeparator" style="margin: 2px 0px;"></div>
        <table *ngIf="showTime" class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="width:100%">
          <tbody>
            <tr class="stiJsViewerClearAllStyles">
              <td class="stiJsViewerClearAllStyles" style="padding: 0 4px 0 4px; white-space: nowrap;">
                {{this.model.loc('Time') + ':'}}
              </td>

              <td class="stiJsViewerClearAllStyles" style="text-align: right;">
                <sti-text-box [width]="90"
                    [margin]="'1px 2px 2px 2px'"
                    [value]="getTime()"
                    (action)="timeAction($event)">
                </sti-text-box>
              </td>
            </tr>
          </tbody>
        </table>
  `
})

export class DatePickerMenuComponent implements OnInit {

  @Input() menu: Menu;
  @Input() keyTo = false;
  @Input() closeOnAction = true;

  public monthesForDatePickerItems: Item[] = [];
  public countLetters: number;
  public cols = [0, 1, 2, 3, 4, 5, 6];
  public rows = [0, 1, 2, 3, 4, 5];

  constructor(public model: ModelService, public helper: HelperService) {
    this.model.months.forEach((m, i) => this.monthesForDatePickerItems.push({ name: 'Month' + i, caption: this.model.loc('Month' + m), key: i }));

    const firstLetters = {};
    this.model.dayOfWeek.forEach((d) => {
      const dayName = this.model.loc('Day' + d);
      firstLetters[dayName.toString().substring(0, 1).toUpperCase()] = true;
    });
    this.countLetters = Object.keys(firstLetters).length < 5 ? 2 : 1;
  }

  ngOnInit() { }

  getButtonSelected(col: number, row: number): boolean {
    const firstDay = this.getFirstDay();
    const curDay = row * 7 + col;
    const day = curDay - firstDay + 1;

    return day === this.key.day;
  }

  getButtonCaption(col: number, row: number): string {
    const firstDay = this.getFirstDay();
    const daysInMonth = this.helper.getCountDaysOfMonth(this.key.year, this.key.month - 1);
    const curDay = row * 7 + col;
    const day = curDay - firstDay + 1;
    if (curDay >= firstDay && day <= daysInMonth) {
      return day.toString();
    }
    return '';
  }


  getTime(): string {
    return this.helper.formatDate(this.helper.getDate(this.key), 'H:mm:ss');
  }

  getFirstDay(): number {
    let firstDay = new Date(this.key.year, this.key.month - 1, 1).getDay();
    if (firstDay === 0) {
      firstDay = 7;
    }
    if (this.model.options.appearance.datePickerFirstDayOfWeek !== 'Sunday') {
      firstDay--;
    }
    return firstDay;
  }

  get showTime(): boolean {
    return this.menu?.params?.dateTimeType !== 'Date';
  }

  get key(): DateTimeObject {
    return this.keyTo ? this.menu.params.keyTo : this.menu.params.key;
  }

  yearAction(input: any) {
    try {
      this.key.year = parseInt(input.value, 10);
    } catch { }
  }

  timeAction(input: any) {
    const time = this.helper.stringToTime(input.value);
    this.key.seconds = time.seconds;
    this.key.minutes = time.minutes;
    this.key.hours = time.hours;
  }

  getShortDayName(index: number): string {
    const dayName = this.model.loc('Day' + this.model.dayOfWeek[index]);
    if (dayName) {
      return dayName.toString().substring(0, this.countLetters <= dayName.length ? this.countLetters : 1).toUpperCase();
    }
    return '';
  }

  getShortDayColor(index: number): string {
    if (index === (this.model.options.appearance.datePickerFirstDayOfWeek === 'Sunday' ? 6 : 5)) {
      return '#0000ff';
    }

    if (index === (this.model.options.appearance.datePickerFirstDayOfWeek === 'Sunday' ? 0 : 6)) {
      return '#ff0000';
    }
    return '';
  }

  prevMonthButtonAction() {
    let month = this.key.month;
    let year = this.key.year;
    month--;
    if (month === 0) {
      month = 12;
      year--;
    }
    const countDaysInMonth = this.helper.getCountDaysOfMonth(year, month - 1);
    if (countDaysInMonth < this.key.day) {
      this.key.day = countDaysInMonth;
    }
    this.key.month = month;
    this.key.year = year;
  }

  nextMonthButtonAction() {
    let month = this.key.month;
    let year = this.key.year;
    month++;
    if (month === 13) {
      month = 1;
      year++;
    }
    const countDaysInMonth = this.helper.getCountDaysOfMonth(year, month - 1);
    if (countDaysInMonth < this.key.day) {
      this.key.day = countDaysInMonth;
    }
    this.key.month = month;
    this.key.year = year;
  }


}
