import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, timeout } from 'rxjs/operators';
import { ModelService } from './model.service';
import { HelperService } from './helper.service';

@Injectable()
export class StiHttpClientService {

  constructor(private httpClient: HttpClient, public model: ModelService, private helper: HelperService) { }

  public post(url: string, data: any, responseType: string = 'json'): Observable<any> {
    const model = this.model;
    model.showProgress = true;
    const reqTimeout = this.model.options?.server?.requestTimeout > 0 ? this.model.options?.server?.requestTimeout * 1000 : 1000 * 1000;
    if (responseType === 'json') {
      return this.httpClient.post(url, this.getFormData(data)).pipe(
        // retry(3),
        timeout(reqTimeout),

        catchError((error: any) => {
          model.httpError = error;
          model.showProgress = false;
          this.model.controls.navigatePanel.enabled = true;
          this.model.controls.toolbar.enabled = true;
          return throwError('Something bad happened; please try again later.');
        }));
    } else {
      return this.httpClient.post(url, this.getFormData(data), { responseType: 'text' }).pipe(
        // retry(3),
        timeout(reqTimeout),
        catchError((error: any) => {
          model.httpError = error;
          model.showProgress = false;
          this.model.controls.navigatePanel.enabled = true;
          this.model.controls.toolbar.enabled = true;
          return throwError('Something bad happened; please try again later.');
        }));
    }
  }

  private getFormData(data: any): FormData {
    const formData: FormData = new FormData();
    Object.keys(data).forEach(key => formData.append(key, data[key]));
    return formData;
  }

  public postForm(url: string, data: any, doc: any, postOnlyData: boolean = false) {
    if (!doc) { doc = document; }
    const form = doc.createElement('FORM');
    form.setAttribute('method', 'POST');
    form.setAttribute('action', url);

    const params = postOnlyData ? data : this.model.createPostParameters(data, true);

    Object.keys(params).forEach(key => {
      const paramsField = doc.createElement('INPUT');
      paramsField.setAttribute('type', 'hidden');
      paramsField.setAttribute('name', key);
      paramsField.setAttribute('value', params[key]);
      form.appendChild(paramsField);
    });

    if (this.model.options.jsDesigner) {
      this.model.options.jsDesigner.options.ignoreBeforeUnload = true;
    }

    doc.body.appendChild(form);
    form.submit();
    doc.body.removeChild(form);

    setTimeout(() => {
      if (this.model.options.jsDesigner) {
        this.model.options.jsDesigner.options.ignoreBeforeUnload = false;
      }
    }, 500);
  }

}
