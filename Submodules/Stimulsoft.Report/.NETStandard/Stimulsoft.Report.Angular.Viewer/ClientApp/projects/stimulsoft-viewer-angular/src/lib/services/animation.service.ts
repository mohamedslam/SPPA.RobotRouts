import { Injectable } from '@angular/core';
import { ModelService } from './model.service';

@Injectable()
export class AnimationService {
  constructor(private model: ModelService) { }

  public showAnimationForScroll(reportPanel, finishScrollTop, endTime, completeFunction) {
    if (!reportPanel) {
      return;
    }
    let currentScrollTop = 0;
    if (this.model.options.appearance.scrollbarsMode) {
      currentScrollTop = reportPanel.scrollTop;
    } else {
      currentScrollTop = document.documentElement.scrollTop;
      if (currentScrollTop === 0) {
        currentScrollTop = document.getElementsByTagName('BODY')[0].scrollTop;
      }
    }

    clearTimeout(reportPanel.animationTimer);
    const d = new Date();
    const t = d.getTime();
    let step = Math.round((finishScrollTop - currentScrollTop) / ((Math.abs(endTime - t) + 1) / 30));

    // Last step
    if (Math.abs(step) > Math.abs(finishScrollTop - currentScrollTop)) {
      step = finishScrollTop - currentScrollTop;
    }

    currentScrollTop += step;
    let resultScrollTop: number;

    if (t < endTime) {
      resultScrollTop = currentScrollTop;
      reportPanel.animationTimer = setTimeout(() => {
        this.showAnimationForScroll(reportPanel, finishScrollTop, endTime, completeFunction);
      }, 30);
    } else {
      resultScrollTop = finishScrollTop;
      if (completeFunction) {
        completeFunction();
      }
    }

    if (this.model.options.appearance.scrollbarsMode) {
      reportPanel.scrollTop = resultScrollTop;
    } else {
      window.scrollTo(0, resultScrollTop);
    }
  }

}
