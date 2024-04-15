import { Component, OnInit, Input } from '@angular/core';
import { Menu } from './menu.service';
import { ModelService } from '../services/model.service';
import { HelperService } from '../services/helper.service';

@Component({
  selector: 'sti-double-date-picker-menu',
  template: `
      <table class="stiJsViewerClearAllStyles" cellpadding="0" cellspacing="0" style="margin: 4px; border: 1px dotted rgb(198, 198, 198);">
          <tbody>
            <tr class="stiJsViewerClearAllStyles">
              <td class="stiJsViewerClearAllStyles" style="vertical-align: top;">
                  <div style="margin:4px">
                    <sti-date-picker-menu [menu]="menu" [closeOnAction]="false">
                    </sti-date-picker-menu>
                  </div>
              </td>

              <td class="stiJsViewerClearAllStyles" style="border-left: 1px dotted rgb(198, 198, 198); vertical-align: top;">
                  <div style="margin:4px">
                    <sti-date-picker-menu [menu]="menu" [keyTo]="true" [closeOnAction]="false">
                    </sti-date-picker-menu>
                  </div>
              </td>

              <td class="stiJsViewerClearAllStyles" style="border-left: 1px dotted rgb(198, 198, 198); vertical-align: top;">
                <div style="width: 150px; overflow: auto; margin: 4px;" [style.height.px]="showTime ? 250 : 220">
                    <ng-container *ngFor="let item of model.dateRanges">
                        <sti-button [caption]="model.loc(item)" (action)="getValuesByDateRangeName(item)">
                        </sti-button>

                        <div *ngIf="item=='Yesterday' || item=='PreviousWeek' || item=='PreviousMonth' || item=='PreviousQuarter' || item=='PreviousYear' || item=='FourthQuarter' || item=='Last30Days'"
                            class="stiJsViewerVerticalMenuSeparator" style="margin:2px"></div>
                    </ng-container>
                </div>
              </td>
            </tr>
          </tbody>
      </table>
  `
})

export class DoubleDatePickerMenuComponent implements OnInit {

  @Input() menu: Menu;

  constructor(public model: ModelService, public helper: HelperService) { }

  ngOnInit() { }

  get showTime(): boolean {
    return this.menu?.params?.dateTimeType !== 'Date';
  }

  setTimeInterval(firstDate: Date, secondDate: Date) {
    firstDate.setHours(0);
    firstDate.setMinutes(0);
    firstDate.setSeconds(0);
    secondDate.setHours(23);
    secondDate.setMinutes(59);
    secondDate.setSeconds(59);
  }

  getFirstDayOfWeek() {
    const date = new Date();
    const timeString = date.toLocaleTimeString();
    return (timeString.toLowerCase().indexOf('am') >= 0 || timeString.toLowerCase().indexOf('pm') >= 0 ? 0 : 1);
  }

  getWeekInterval(now: Date) {
    const startDay = this.getFirstDayOfWeek();
    let dayWeek = startDay === 0 ? now.getDay() : now.getDay() - 1;
    if (dayWeek < 0) { dayWeek = 6; }
    const values = [new Date(now.valueOf() - dayWeek * 86400000)];
    values.push(new Date(values[0].valueOf() + 6 * 86400000));
    this.setTimeInterval(values[0], values[1]);

    return values;
  }


  getValuesByDateRangeName(item: string) {
    const now = new Date();
    let values = [new Date(), new Date()];

    switch (item) {
      case 'CurrentMonth': {
        values[0].setDate(1);
        values[1].setDate(this.helper.getCountDaysOfMonth(now.getFullYear(), now.getMonth()));
        break;
      }
      case 'CurrentQuarter': {
        const firstMonth = parseInt((now.getMonth() / 3).toString(), 10) * 3;
        values[0] = new Date(now.getFullYear(), firstMonth, 1);
        values[1] = new Date(now.getFullYear(), firstMonth + 2, this.helper.getCountDaysOfMonth(now.getFullYear(), firstMonth + 2));
        break;
      }
      case 'CurrentWeek': {
        values = this.getWeekInterval(now);
        break;
      }
      case 'CurrentYear': {
        values[0] = new Date(now.getFullYear(), 0, 1);
        values[1] = new Date(now.getFullYear(), 11, 31);
        break;
      }
      case 'NextMonth': {
        let month = now.getMonth() + 1;
        let year = now.getFullYear();
        if (month > 11) {
          month = 0;
          year++;
        }
        values[0] = new Date(year, month, 1);
        values[1] = new Date(year, month, this.helper.getCountDaysOfMonth(year, month));
        break;
      }
      case 'NextQuarter': {
        let year = now.getFullYear();
        let firstMonth = parseInt((now.getMonth() / 3).toString(), 10) * 3 + 3;
        if (firstMonth > 11) {
          firstMonth = 0;
          year++;
        }
        values[0] = new Date(year, firstMonth, 1);
        values[1] = new Date(year, firstMonth + 2, this.helper.getCountDaysOfMonth(year, firstMonth + 2));
        break;
      }
      case 'NextWeek': {
        values = this.getWeekInterval(now);
        values[0] = new Date(values[0].valueOf() + 7 * 86400000);
        values[1] = new Date(values[1].valueOf() + 7 * 86400000);
        break;
      }
      case 'NextYear': {
        values[0] = new Date(now.getFullYear() + 1, 0, 1);
        values[1] = new Date(now.getFullYear() + 1, 11, 31);
        break;
      }
      case 'PreviousMonth': {
        let month = now.getMonth() - 1;
        let year = now.getFullYear();
        if (month < 0) {
          month = 11;
          year--;
        }
        values[0] = new Date(year, month, 1);
        values[1] = new Date(year, month, this.helper.getCountDaysOfMonth(year, month));
        break;
      }
      case 'PreviousQuarter': {
        let year = now.getFullYear();
        let firstMonth = parseInt((now.getMonth() / 3).toString(), 10) * 3 - 3;
        if (firstMonth < 0) {
          firstMonth = 9;
          year--;
        }
        values[0] = new Date(year, firstMonth, 1);
        values[1] = new Date(year, firstMonth + 2, this.helper.getCountDaysOfMonth(year, firstMonth + 2));
        break;
      }
      case 'PreviousWeek': {
        values = this.getWeekInterval(now);
        values[0] = new Date(values[0].valueOf() - 7 * 86400000);
        values[1] = new Date(values[1].valueOf() - 7 * 86400000);
        break;
      }
      case 'PreviousYear': {
        values[0] = new Date(now.getFullYear() - 1, 0, 1);
        values[1] = new Date(now.getFullYear() - 1, 11, 31);
        break;
      }
      case 'FirstQuarter': {
        values[0] = new Date(now.getFullYear(), 0, 1);
        values[1] = new Date(now.getFullYear(), 2, this.helper.getCountDaysOfMonth(now.getFullYear(), 2));
        break;
      }
      case 'SecondQuarter': {
        values[0] = new Date(now.getFullYear(), 3, 1);
        values[1] = new Date(now.getFullYear(), 5, this.helper.getCountDaysOfMonth(now.getFullYear(), 5));
        break;
      }
      case 'ThirdQuarter': {
        values[0] = new Date(now.getFullYear(), 6, 1);
        values[1] = new Date(now.getFullYear(), 8, this.helper.getCountDaysOfMonth(now.getFullYear(), 8));
        break;
      }
      case 'FourthQuarter': {
        values[0] = new Date(now.getFullYear(), 9, 1);
        values[1] = new Date(now.getFullYear(), 11, this.helper.getCountDaysOfMonth(now.getFullYear(), 11));
        break;
      }
      case 'MonthToDate': {
        values[0].setDate(1);
        break;
      }
      case 'QuarterToDate': {
        const firstMonth = parseInt((now.getMonth() / 3).toString(), 10) * 3;
        values[0].setDate(1);
        values[0].setMonth(firstMonth);
        break;
      }
      case 'WeekToDate': {
        const weekValues = this.getWeekInterval(now);
        values[0] = weekValues[0];
        break;
      }
      case 'YearToDate': {
        values[0].setDate(1);
        values[0].setMonth(0);
        break;
      }
      case 'Today': {
        break;
      }
      case 'Tomorrow': {
        values[0] = new Date(values[0].valueOf() + 86400000);
        values[1] = new Date(values[1].valueOf() + 86400000);
        break;
      }
      case 'Yesterday': {
        values[0] = new Date(values[0].valueOf() - 86400000);
        values[1] = new Date(values[1].valueOf() - 86400000);
        break;
      }
      case 'Last7Days': {
        if (this.model.options.appearance.datePickerIncludeCurrentDayForRanges) {
          values[0] = new Date(values[0].valueOf() - 6 * 86400000);
        } else {
          values[0] = new Date(values[0].valueOf() - 7 * 86400000);
        }
        break;
      }
      case 'Last14Days': {
        if (this.model.options.appearance.datePickerIncludeCurrentDayForRanges) {
          values[0] = new Date(values[0].valueOf() - 13 * 86400000);
        } else {
          values[0] = new Date(values[0].valueOf() - 14 * 86400000);
        }
        break;
      }
      case 'Last30Days': {
        if (this.model.options.appearance.datePickerIncludeCurrentDayForRanges) {
          values[0] = new Date(values[0].valueOf() - 29 * 86400000);
        } else {
          values[0] = new Date(values[0].valueOf() - 30 * 86400000);
        }
        break;
      }
    }

    this.setTimeInterval(values[0], values[1]);

    this.menu.params.key = this.helper.getDateTimeObject(values[0]);
    this.menu.params.keyTo = this.helper.getDateTimeObject(values[1]);
  }
}
