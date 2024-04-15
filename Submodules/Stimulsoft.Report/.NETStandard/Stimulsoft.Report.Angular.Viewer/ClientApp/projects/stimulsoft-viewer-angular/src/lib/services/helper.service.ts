import { Injectable } from '@angular/core';
import { ModelService } from './model.service';
import { AnimationService } from './animation.service';
import { DateTimeObject } from './objects';
import { MenuItem } from '../menu/meni-item.component';

@Injectable()
export class HelperService {

  constructor(private model: ModelService, private animationService: AnimationService) { }

  public val(value: string, def?: string): string {
    return value != null && value.trim() !== '' ? value : (def || '');
  }

  public findPosY(obj: any, mainClassName?: string, noScroll?: boolean) {
    let curtop = noScroll ? 0 : this.getScrollYOffset(obj, mainClassName);
    if (obj.offsetParent) {
      while (obj.className !== mainClassName) {
        curtop += obj.offsetTop;
        if (!obj.offsetParent) {
          break;
        }
        obj = obj.offsetParent;
      }
    } else if (obj.y) {
      curtop += obj.y;
    }
    return curtop;
  }

  public getScrollYOffset(obj: any, mainClassName: string): number {
    let scrolltop = 0;
    if (obj.parentElement) {
      while (obj.className !== mainClassName) {
        if ('scrollTop' in obj) {
          scrolltop -= obj.scrollTop;
        }
        if (!obj.parentElement) {
          break;
        }
        obj = obj.parentElement;
      }
    }

    return scrolltop;
  }

  public findPosX(obj: any, mainClassName: string, noScroll?: boolean) {
    let curleft = noScroll ? 0 : this.getScrollXOffset(obj, mainClassName);
    if (obj.offsetParent) {
      while (obj.className !== mainClassName) {
        curleft += obj.offsetLeft;
        if (!obj.offsetParent) {
          break;
        }
        obj = obj.offsetParent;
      }
    } else if (obj.x) {
      curleft += obj.x;
    }
    return curleft;
  }

  public getScrollXOffset(obj: any, mainClassName: string) {
    let scrollleft = 0;
    if (obj.parentElement) {
      while (obj.className !== mainClassName) {
        if ('scrollLeft' in obj) {
          scrollleft -= obj.scrollLeft;
        }
        if (!obj.parentElement) {
          break;
        }
        obj = obj.parentElement;
      }
    }

    return scrollleft;
  }

  public showHelpWindow(url: string) {
    let helpLanguage = 'en';
    switch (this.model.options.cultureName) {
      case 'ru': helpLanguage = 'ru';
      // case 'de': helpLanguage = 'de';
    }
    this.openNewWindow('https://www.stimulsoft.com/' + helpLanguage + '/documentation/online/' + url);
  }

  public openNewWindow(url: string, name?: string, specs?: string) {
    return window.open(url, name, specs);
  }

  public scrollToAnchor(anchorName: string, componentGuid?: string) {
    let aHyperlinks = this.model.controls.reportPanel.el.nativeElement.getElementsByTagName("a");
    const identicalAnchors = [];
    if (anchorName) {
      anchorName = anchorName.replace(/!!#92/g, '\\');
    }

    if (componentGuid) {
      for (let i = 0; i < aHyperlinks.length; i++) {
        if (aHyperlinks[i].getAttribute('guid') === componentGuid) {
          identicalAnchors.push(aHyperlinks[i]);
        }
      }
    }

    if (identicalAnchors.length === 0) {
      let guidIndex = anchorName.indexOf("#GUID#");

      if (identicalAnchors.length == 0) {
        let aHyperlinks = this.model.controls.reportPanel.el.nativeElement.getElementsByTagName("a");
        for (var i = 0; i < aHyperlinks.length; i++) {
          if (aHyperlinks[i].name && ((guidIndex >= 0 && (aHyperlinks[i].name.indexOf(anchorName.substring(guidIndex + 6)) >= 0 || anchorName.substring(0, guidIndex) == aHyperlinks[i].name)) || aHyperlinks[i].name == anchorName))
            identicalAnchors.push(aHyperlinks[i]);
        }
      }
    }

    if (identicalAnchors.length > 0) {
      const anchor = identicalAnchors[0];
      const anchorParent = anchor.parentElement || anchor;
      let anchorHeight = anchorParent.offsetHeight;
      const anchorOffsetTop = anchorParent.offsetTop;

      identicalAnchors.forEach((identicalAnchor) => {
        const nextAnchorParent = identicalAnchor.parentElement || identicalAnchor;
        if (nextAnchorParent.offsetTop > anchorOffsetTop) {
          anchorHeight = Math.max(anchorHeight, nextAnchorParent.offsetTop - anchorOffsetTop + nextAnchorParent.offsetHeight);
        }
      });

      const date = new Date();
      const endTime = date.getTime() + this.model.options.scrollDuration;
      const targetTop = this.findPosY(anchor, this.model.options.appearance.scrollbarsMode ? 'stiJsViewerReportPanel' : null, true) - anchorParent.offsetHeight * 2;

      this.animationService.showAnimationForScroll(this.model.controls.reportPanel.el.nativeElement, targetTop, endTime, () => {
        const page = this.getPageFromAnchorElement(anchor);
        const anchorParentTopPos = this.findPosY(anchorParent, 'stiJsViewerReportPanel', true);
        const pageTopPos = page ? this.findPosY(page, 'stiJsViewerReportPanel', true) : anchorParentTopPos;

        this.removeBookmarksLabel();

        const label = document.createElement('div');
        this.model.controls.bookmarksLabel = label;
        label.className = 'stiJsViewerBookmarksLabel';

        const labelMargin = 20 * (this.model.reportParams.zoom / 100);
        const labelWidth = page ? page.offsetWidth - labelMargin - 6 : anchorParent.offsetWidth;
        const labelHeight = anchorHeight - 2;
        label.style.width = labelWidth + 'px';
        label.style.height = labelHeight + 'px';

        const margins = page.pageAttributes.margins?.replace('px', '').split(' ');
        const pageLeftMargin = page.pageAttributes.margins ? this.strToInt(margins[3]) : 0;
        const pageTopMargin = page.pageAttributes.margins ? this.strToInt(margins[0]) : 0;
        label.style.marginLeft = (labelMargin / 2 - pageLeftMargin) + 'px';
        label.style.marginTop = (anchorParentTopPos - pageTopPos - pageTopMargin - (this.model.reportParams.zoom / 100) - 1) + 'px';

        page.insertBefore(label, page.childNodes[0]);
      });
    }
  }

  public removeBookmarksLabel() {
    if (this.model.controls.bookmarksLabel) {
      this.model.controls.bookmarksLabel.parentElement.removeChild(this.model.controls.bookmarksLabel);
      this.model.controls.bookmarksLabel = null;
    }
  }

  public getPageFromAnchorElement(anchorElement: any) {
    let obj = anchorElement;
    while (obj.parentElement) {
      if (obj.className && obj.className.indexOf('stiJsViewerPage') === 0) {
        return obj;
      }
      obj = obj.parentElement;
    }
    return obj;
  }

  public strToInt(value: any): number {
    const result = parseInt(value, 10);
    return result || 0;
  }

  public strToDouble(value: any): number {
    if (value === null) {
      return null;
    }
    const result = parseFloat(value.toString().replace(',', '.').trim());
    return result || 0;
  }

  public copyObject(o: any): any {
    if (!o || 'object' !== typeof o) {
      return o;
    }
    const c = 'function' === typeof o.pop ? [] : {};
    let p;
    let v;
    for (p in o) {
      if (o.hasOwnProperty(p)) {
        v = o[p];
        if (v && 'object' === typeof v) {
          c[p] = this.copyObject(v);
        } else {
          c[p] = v;
        }
      }
    }
    return c;
  }

  public getCountObjects(objectArray: any): number {
    let count = 0;
    if (objectArray) {
      for (const singleObject in objectArray) {
        count++;
      }
    }
    return count;
  }

  public replaceMonths(value: string): string {
    for (let i = 1; i <= 12; i++) {
      let enName = '';
      let locName = '';
      switch (i) {
        case 1:
          enName = 'January';
          locName = this.model.loc('MonthJanuary');
          break;

        case 2:
          enName = 'February';
          locName = this.model.loc('MonthFebruary');
          break;

        case 3:
          enName = 'March';
          locName = this.model.loc('MonthMarch');
          break;

        case 4:
          enName = 'April';
          locName = this.model.loc('MonthApril');
          break;

        case 5:
          enName = 'May';
          locName = this.model.loc('MonthMay');
          break;

        case 6:
          enName = 'June';
          locName = this.model.loc('MonthJune');
          break;

        case 7:
          enName = 'July';
          locName = this.model.loc('MonthJuly');
          break;

        case 8:
          enName = 'August';
          locName = this.model.loc('MonthAugust');
          break;

        case 9:
          enName = 'September';
          locName = this.model.loc('MonthSeptember');
          break;

        case 10:
          enName = 'October';
          locName = this.model.loc('MonthOctober');
          break;

        case 11:
          enName = 'November';
          locName = this.model.loc('MonthNovember');
          break;

        case 12:
          enName = 'December';
          locName = this.model.loc('MonthDecember');
          break;
      }

      const enShortName = enName.substring(0, 3);
      const locShortName = locName.substring(0, 3);
      value = value.replace(enName, i as any).replace(enName.toLowerCase(), i as any).replace(enShortName, i as any).replace(enShortName.toLowerCase(), i as any);
      value = value.replace(locName, i as any).replace(locName.toLowerCase(), i as any).replace(locShortName, i as any).replace(locShortName.toLowerCase(), i as any);

    }

    return value;
  }

  getDate(dateTimeObj: DateTimeObject): Date {
    return new Date(dateTimeObj.year, dateTimeObj.month - 1, dateTimeObj.day, dateTimeObj.hours, dateTimeObj.minutes, dateTimeObj.seconds);
  }

  public getDateTimeObject(date?: Date) {
    if (!date) { date = new Date(); }
    const dateTimeObject: DateTimeObject = new DateTimeObject();
    dateTimeObject.year = date.getFullYear();
    dateTimeObject.month = date.getMonth() + 1;
    dateTimeObject.day = date.getDate();
    dateTimeObject.hours = date.getHours();
    dateTimeObject.minutes = date.getMinutes();
    dateTimeObject.seconds = date.getSeconds();

    return dateTimeObject;
  }

  public getDateTimeFromString(value: string, format: string): any {
    const charIsDigit = (ch: string) => {
      return ('0123456789'.indexOf(ch) >= 0);
    };

    if (!value) {
      return new Date();
    }
    value = this.replaceMonths(value);

    let dateTime = new Date();

    // If the date format is not specified, then deserializator for getting date and time is applied
    if (format === null) {
      format = 'dd.MM.yyyy hh:mm:ss';
    }
    // Otherwise the format is parsed. Now only numeric date and time formats are supported

    let year = 1970;
    let month = 1;
    let day = 1;
    let hour = 0;
    let minute = 0;
    let second = 0;
    let millisecond = 0;

    let char = '';
    let pos = 0;
    const values = [];

    // Parse date and time into separate numeric values
    while (pos < value.length) {
      char = value.charAt(pos);
      if (charIsDigit(char)) {
        values.push(char);
        pos++;

        while (pos < value.length && charIsDigit(value.charAt(pos))) {
          values[values.length - 1] += value.charAt(pos);
          pos++;
        }

        values[values.length - 1] = this.strToInt(values[values.length - 1]);
      }

      pos++;
    }

    pos = 0;
    let charCount = 0;
    let index = -1;
    let is12hour = false;

    // Parsing format and replacement of appropriate values of date and time
    while (pos < format.length) {
      char = format.charAt(pos);
      charCount = 0;

      if (char === 'Y' || char === 'y' || char === 'M' || char === 'd' || char === 'h' || char === 'H' ||
        char === 'm' || char === 's' || char === 'f' || char === 'F' || char === 't' || char === 'z') {
        index++;

        while (pos < format.length && format.charAt(pos) === char) {
          pos++;
          charCount++;
        }
      }

      switch (char) {
        case 'Y': // full year
          year = values[index];
          break;

        case 'y': // year
          if (values[index] < 1000) {
            year = 2000 + values[index];
          } else {
            year = values[index];
          }
          break;

        case 'M': // month
          month = values[index];
          break;

        case 'd': // day
          day = values[index];
          break;

        case 'h': // (hour 12)
          is12hour = true;
          hour = values[index];
          break;

        case 'H': // (hour 24)
          hour = values[index];
          break;

        case 'm': // minute
          minute = values[index];
          break;

        case 's': // second
          second = values[index];
          break;

        case 'f': // second fraction
        case 'F': // second fraction, trailing zeroes are trimmed
          millisecond = values[index];
          break;

        case 't': // PM or AM
          if (value.toLowerCase().indexOf('am') >= 0 && hour === 12) { hour = 0; }
          if (value.toLowerCase().indexOf('pm') >= 0 && hour < 12) { hour += 12; }
          break;

        default:
          pos++;
          break;
      }
    }

    dateTime = new Date(year || new Date().getFullYear(), month - 1 || 0, day || 1, hour || 0, minute || 0, second, millisecond || 0);

    if (!dateTime || isNaN(dateTime as any)) {
      return new Date();
    }

    return dateTime;
  }

  public getStringDateTime(object, dateTimeType) {
    if (dateTimeType === 'Date') {
      object.hours = 0;
      object.minutes = 0;
      object.seconds = 0;
    }
    return object.month + '/' + object.day + '/' + object.year + ' ' +
      (object.hours > 12 ? object.hours - 12 : object.hours) + ':' + object.minutes + ':' + object.seconds + ' ' +
      (object.hours < 12 ? 'AM' : 'PM');
  }

  public dateTimeObjectToString(dateTimeObject, typeDateTimeObject?) {
    const date = new Date(dateTimeObject.year, dateTimeObject.month - 1, dateTimeObject.day, dateTimeObject.hours, dateTimeObject.minutes, dateTimeObject.seconds);

    if (this.model.options.appearance.parametersPanelDateFormat !== '') {
      return this.formatDate(date, this.model.options.appearance.parametersPanelDateFormat);
    }

    return this.dateToLocaleString(date, typeDateTimeObject);
  }

  public dateToLocaleString(date, dateTimeType?): string {
    const timeString = date.toLocaleTimeString();
    const isAmericanFormat = timeString.toLowerCase().indexOf('am') >= 0 || timeString.toLowerCase().indexOf('pm') >= 0;
    let formatDate = isAmericanFormat ? 'MM/dd/yyyy' : 'dd.MM.yyyy';

    const yyyy = date.getFullYear();
    const yy = yyyy.toString().substring(2);
    const M = date.getMonth() + 1;
    const MM = M < 10 ? '0' + M : M;
    const d = date.getDate();
    const dd = d < 10 ? '0' + d : d;

    formatDate = formatDate.replace(/yyyy/i, yyyy);
    formatDate = formatDate.replace(/yy/i, yy);
    formatDate = formatDate.replace(/MM/i, MM);
    formatDate = formatDate.replace(/M/i, M);
    formatDate = formatDate.replace(/dd/i, dd);
    formatDate = formatDate.replace(/d/i, d);

    let h = date.getHours();
    let tt = '';

    if (isAmericanFormat) {
      tt = h < 12 ? ' AM' : ' PM';
      h = h > 12 ? h - 12 : h;
      if (h === 0) {
        h = 12;
      }
    } else {
      h = h < 10 ? '0' + h : h;
    }

    let m = date.getMinutes();
    m = m < 10 ? '0' + m : m;
    let s = date.getSeconds();
    s = s < 10 ? '0' + s : s;

    const formatTime = h + ':' + m + ':' + s + tt;

    if (dateTimeType === 'Time') {
      return formatTime;
    }
    if (dateTimeType === 'Date') {
      return formatDate;
    }

    return formatDate + ' ' + formatTime;
  }

  public formatDate(formatDate, formatString: string): string {
    const yyyy = formatDate.getFullYear();
    const yy = yyyy.toString().substring(2);
    const m = formatDate.getMonth() + 1;
    const mm = m < 10 ? '0' + m : m;
    const d = formatDate.getDate();
    const dd = d < 10 ? '0' + d : d;

    const h = formatDate.getHours();
    const hh = h < 10 ? '0' + h : h;
    const h12 = h > 12 ? h - 12 : (h > 0 ? h : 12);
    const hh12 = h12 < 10 ? '0' + h12 : h12;
    const n = formatDate.getMinutes();
    const nn = n < 10 ? '0' + n : n;
    const s = formatDate.getSeconds();
    const ss = s < 10 ? '0' + s : s;
    const tt = h < 12 ? 'AM' : 'PM';

    formatString = formatString.replace(/yyyy/gi, yyyy);
    formatString = formatString.replace(/yy/gi, yy);
    formatString = formatString.replace(/Y/, yyyy);
    formatString = formatString.replace(/MM/g, mm);
    formatString = formatString.replace(/M/g, m);
    formatString = formatString.replace(/dd/g, dd);
    formatString = formatString.replace(/d/g, d);
    formatString = formatString.replace(/hh/g, hh12);
    formatString = formatString.replace(/h/g, h12);
    formatString = formatString.replace(/HH/g, hh);
    formatString = formatString.replace(/H/g, h);
    formatString = formatString.replace(/mm/g, nn);
    formatString = formatString.replace(/m/g, n);
    formatString = formatString.replace(/ss/g, ss);
    formatString = formatString.replace(/s/g, s);
    formatString = formatString.replace(/tt/gi, tt);
    formatString = formatString.replace(/t/gi, tt.substr(0, 1));

    return formatString;
  }

  public getStringKey(key: string, params): string {
    if (key === null) {
      return '';
    }

    return ((params.type === 'DateTime') ? this.dateTimeObjectToString(key, params.dateTimeType) : key);
  }

  public stringToTime(timeStr: string) {
    const timeArray = timeStr.split(':');
    const time = { hours: 0, minutes: 0, seconds: 0 };

    time.hours = this.strToInt(timeArray[0]);
    if (timeArray.length > 1) { time.minutes = this.strToInt(timeArray[1]); }
    if (timeArray.length > 2) { time.seconds = this.strToInt(timeArray[2]); }
    if (time.hours < 0) { time.hours = 0; }
    if (time.minutes < 0) { time.minutes = 0; }
    if (time.seconds < 0) { time.seconds = 0; }

    if (time.hours > 23) { time.hours = 23; }
    if (time.minutes > 59) { time.minutes = 59; }
    if (time.seconds > 59) { time.seconds = 59; }

    return time;
  }

  public newGuid() {
    const chars = '0123456789abcdefghijklmnopqrstuvwxyz'.split('');
    const uuid = [];
    const rnd = Math.random;
    let r;
    uuid[8] = uuid[13] = uuid[18] = uuid[23] = '-';
    uuid[14] = '4';

    for (let i = 0; i < 36; i++) {
      if (!uuid[i]) {
        r = 0 | rnd() * 16;
        uuid[i] = chars[(i === 19) ? (r & 0x3) | 0x8 : r & 0xf];
      }
    }

    return uuid.join('');
  }

  public getCountDaysOfMonth(year: number, month: number): number {
    const countDaysInMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
    let count = countDaysInMonth[month];

    if (month === 1) {
      if (year % 4 === 0 && (year % 100 !== 0 || year % 400 === 0)) {
        count = 29;
      } else {
        count = 28;
      }
    }
    return count;
  }

  public setObjectToCenter(object: any, defaultTop?: number): any {
    const leftPos = (this.model.viewerSize.width / 2 - object.offsetWidth / 2);
    const topPos = this.model.options.appearance.fullScreenMode ? (this.model.viewerSize.height / 2 - object.offsetHeight / 2) : (defaultTop ? defaultTop : 250);
    return { left: Math.max(leftPos, 0), top: Math.max(topPos, 0) };
  }

  public checkCloudAuthorization(action): boolean {
    return true;
  }

  private getCookie_(name: string): string {
    const cookie = ' ' + document.cookie;
    const search = ' ' + name + '=';
    let setStr = null;
    let offset = 0;
    let end = 0;
    if (cookie.length > 0) {
      offset = cookie.indexOf(search);
      if (offset !== -1) {
        offset += search.length;
        end = cookie.indexOf(';', offset);
        if (end === -1) {
          end = cookie.length;
        }
        setStr = unescape(cookie.substring(offset, end));
      }
    }
    return setStr;
  }

  public getCookie(name: string): string {
    if (this.model.options.standaloneJsMode || typeof localStorage === 'undefined' || name.indexOf('sti_') == 0 || name.indexOf('login') == 0) {
      return this.getCookie_(name);
    }

    let value = localStorage.getItem(name);
    if (value != null) {
      return value;
    } else {
      value = this.getCookie_(name);
      if (value != null) {
        this.removeCookie(name);
        localStorage.setItem(name, value);
      }
      return value;
    }
  }

  public removeCookie(name: string) {
    document.cookie = name + '=;expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/';
  }

  public setCookie(name: string, value, path?, domain?, secure?, expires?) {
    if (this.model.options.standaloneJsMode || typeof localStorage === 'undefined' || name.indexOf('sti_') === 0 || name.indexOf('login') === 0) {
      // save to cookies
      if (value && typeof (value) === 'string' && value.length >= 4096) { return; }
      const pathName = location.pathname;
      const expDate = new Date();
      expDate.setTime(expDate.getTime() + (365 * 24 * 3600 * 1000));
      document.cookie = name + '=' + escape(value) +
        '; expires=' + (expires || expDate['toGMTString']()) +
        ((path) ? '; path=' + path : '; path=/') +
        ((domain) ? '; domain=' + domain : '') +
        ((secure) ? '; secure' : '');
    } else {
      // save to localstorage
      localStorage.setItem(name, value);
    }
  }

  public getNavigatorName(): string {
    if (!navigator) { return 'Unknown'; }
    const userAgent = navigator.userAgent;

    if (userAgent.indexOf('Edge') >= 0) { return 'Edge'; }
    if (userAgent.indexOf('MSIE') >= 0 || userAgent.indexOf('Trident') >= 0) { return 'MSIE'; }
    if (userAgent.indexOf('Gecko') >= 0) {
      if (userAgent.indexOf('Chrome') >= 0) { return 'Chrome'; }
      if (userAgent.indexOf('Safari') >= 0) { return 'Safari'; }
      return 'Mozilla';
    }
    if (userAgent.indexOf('Opera') >= 0) { return 'Opera'; }
  }

  public isTouchDevice(): boolean {
    return ('ontouchstart' in document.documentElement);
  }

  public isMobileDevice(): boolean {
    return /iPhone|iPad|iPod|Macintosh|Android|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
  }

  public getHumanFileSize(value: number, decimals: number): string {
    const i = Math.floor(Math.log(value) / Math.log(1024));
    return ((value / Math.pow(1024, i)).toFixed(decimals)) + ' ' + ['B', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'][i];
  }

  getZoomMenuItems(): any {
    const items: MenuItem[] = [{ name: 'Zoom25', caption: '25%', img: '' },
    { name: 'Zoom50', caption: '50%', img: '' },
    { name: 'Zoom75', caption: '75%', img: '' },
    { name: 'Zoom100', caption: '100%', img: '' },
    { name: 'Zoom150', caption: '150%', img: '' },
    { name: 'Zoom200', caption: '200%', img: '' }];

    if (this.model.options.toolbar.displayMode !== 'Separated') {
      items.push({ type: 'separator' });
      items.push({ name: 'ZoomOnePage', caption: this.model.loc('ZoomOnePage'), img: 'ZoomOnePage.png' });
      items.push({ name: 'ZoomPageWidth', caption: this.model.loc('ZoomPageWidth'), img: 'ZoomPageWidth.png' });
    }
    return items;
  }

  public get helpLinks(): any {
    return {
      Print: 'user-manual/index.html?viewer_reports.htm#toolbar',
      Save: 'user-manual/index.html?viewer_reports.htm#toolbar',
      SendEmail: 'user-manual/index.html?viewer_reports.htm#toolbar',
      Bookmarks: 'user-manual/index.html?viewer_reports.htm#toolbar',
      Parameters: 'user-manual/index.html?viewer_reports.htm#toolbar',
      FirstPage: 'user-manual/index.html?viewer_reports.htm#statusbar',
      PrevPage: 'user-manual/index.html?viewer_reports.htm#statusbar',
      NextPage: 'user-manual/index.html?viewer_reports.htm#statusbar',
      LastPage: 'user-manual/index.html?viewer_reports.htm#statusbar',
      FullScreen: 'user-manual/index.html?viewer_reports.htm#toolbar',
      Zoom: 'user-manual/index.html?viewer_reports.htm#statusbar',
      ViewMode: 'user-manual/index.html?viewer_reports.htm#displayingmode',
      Editor: 'user-manual/index.html?viewer_reports.htm#toolbar',
      Find: 'user-manual/index.html?viewer_reports.htm#searchpanel',
      DashboardToolbar: 'user-manual/index.html?viewer_dashboards.htm#controlbuttonsofthedashboard',
      DashboardElementToolbar: 'user-manual/index.html?viewer_dashboards.htm#elementcontrols',
      DashboardExport: 'user-manual/index.html?exports_dashboards.htm',
      DashboardPdfExport: 'user-manual/index.html?exports_dashboards.htm#pdfexportsettings',
      DashboardExcelExport: 'user-manual/index.html?exports_dashboards.htm#excelexportsettings',
      DashboardImageExport: 'user-manual/index.html?exports_dashboards.htm#imageexportsettings',
      DashboardDataExport: 'user-manual/index.html?exports_dashboards.htm#exportsettingsofdata'
    };
  }

  public getBackText(withoutBrackets?: boolean): string {
    const backText = String.fromCharCode(84) + 'r' + String.fromCharCode(105) + 'a';
    if (withoutBrackets) {
      return backText + String.fromCharCode(108);
    }
    return String.fromCharCode(91) + backText + String.fromCharCode(108) + String.fromCharCode(93);
  }
}
