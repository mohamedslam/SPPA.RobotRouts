import { Injectable } from '@angular/core';
import { ModelService } from './model.service';
import { PageService } from './page.service';

@Injectable()
export class FullScreenService {

  constructor(public model: ModelService, public pageService: PageService) { }

  public changeFullScreenMode(fullScreenMode: boolean) {
    this.model.options.appearance.fullScreenMode = fullScreenMode;
    const viewer = this.model.controls.viewer.el.nativeElement;

    if (fullScreenMode) {
      this.model.fullScreenOptions = {
        scrollbarsMode: this.model.options.appearance.scrollbarsMode,
        zIndex: viewer.style.zIndex,
        position: viewer.style.position,
        width: viewer.style.width,
        height: viewer.style.height,
        overflow: document.body.style.overflow
      };

      this.model.options.appearance.scrollbarsMode = true;
      viewer.style.zIndex = '1000000';
      viewer.style.position = this.model.options.reportDesignerMode ? 'absolute' : 'fixed';
      viewer.style.width = null;
      viewer.style.height = null;
      document.body.style.overflow = 'hidden';
    } else if (this.model.fullScreenOptions) {
      this.model.options.appearance.scrollbarsMode = this.model.fullScreenOptions.scrollbarsMode;
      viewer.style.zIndex = this.model.fullScreenOptions.zIndex;
      viewer.style.position = this.model.fullScreenOptions.position;
      viewer.style.width = this.model.fullScreenOptions.width;
      viewer.style.height = this.model.fullScreenOptions.height;
      document.body.style.overflow = this.model.fullScreenOptions.overflow;

      this.model.fullScreenOptions = null;
    }

    this.pageService.calculateLayout();
  }

}
