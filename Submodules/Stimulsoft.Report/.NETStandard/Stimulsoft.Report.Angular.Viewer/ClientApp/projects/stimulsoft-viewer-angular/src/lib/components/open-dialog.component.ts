import { Component, OnInit, Input, OnChanges, ViewChild, ElementRef } from '@angular/core';
import { ModelService } from '../services/model.service';
import { ControllerService } from '../services/controller.service';

@Component({
  selector: 'sti-open-dialog',
  template: `
    <input #element *ngIf="model.openDialogFileMask != null"  type="file" name="files[]" multiple="" style="display: none;"
      [attr.accept]="model.openDialogFileMask"
      (change)="onchange($event)">
  `
})

export class OpenDialogComponent implements OnInit, OnChanges {

  @ViewChild('element') element: ElementRef;

  @Input() fileMask: string;

  constructor(public model: ModelService, public controller: ControllerService) { }

  ngOnChanges(changes: import('@angular/core').SimpleChanges): void {
    if (this.fileMask != null && this.model.openDialogFileMask != null) {
      setTimeout(() => {
        this.element.nativeElement.focus();
        this.element.nativeElement.click();
      });
    }
  }

  ngOnInit() { }

  onchange(event: any) {
    const files: any[] = event.target.files;
    const fileName = files[0] ? files[0].name : 'Report';
    const filePath = event.target.value;
    const reader = new FileReader();

    reader.onload = (e: any) => {
      this.controller.loadFile(fileName, e.target.result);
    };

    this.model.openDialogFileMask = null;
    reader.readAsDataURL(files[0]);
  }

}
