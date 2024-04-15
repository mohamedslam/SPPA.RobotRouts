import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';

@Injectable()
export class MouseService {

  private documetMouseDown = new Subject<any>();
  private documetMouseMove = new Subject<any>();
  private documetMouseUp = new Subject<any>();

  constructor() {
    document.addEventListener('mouseup', (event) => {
      this.documetMouseUp.next(event);
    });

    document.addEventListener('mousemove', (event) => {
      this.documetMouseMove.next(event);
    });

    document.addEventListener('mousedown', (event) => {
      this.documetMouseDown.next(event);
    });
  }

  public getDocumentMouseUp(): Observable<any> {
    return this.documetMouseUp.asObservable();
  }

  public getDocumentMouseMove(): Observable<any> {
    return this.documetMouseMove.asObservable();
  }

  public getDocumentMouseDown(): Observable<any> {
    return this.documetMouseDown.asObservable();
  }

}
