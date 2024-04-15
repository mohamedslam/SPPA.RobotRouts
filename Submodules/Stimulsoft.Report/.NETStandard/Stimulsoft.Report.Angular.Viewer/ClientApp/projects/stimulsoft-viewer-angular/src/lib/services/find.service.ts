import { Injectable } from '@angular/core';
import { ModelService } from './model.service';
import { HelperService } from './helper.service';
import { AnimationService } from './animation.service';

@Injectable()
export class FindService {

  private symbols = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890';
  private findLabels: any[] = [];

  public text = '';
  public lastFindText = '';
  public matchCase = false;
  public matchWholeWord = false;
  public findMode = false;

  constructor(private model: ModelService, private helper: HelperService, private animationService: AnimationService) {
    model.controls.findPanel.getVisibility().subscribe((value) => {
      if (!value) {
        this.hideFindLabels();
        this.lastFindText = '';
      }
    });
  }

  public hideFindLabels() {
    this.findLabels.forEach((findLabel) => {
      const parentElement = findLabel.parentElement;
      parentElement.removeChild(findLabel);
      if (parentElement.oldPositionStyle) {
        parentElement.style.position = parentElement.oldPositionStyle;
      }
    });

    this.findLabels = [];
    this.findMode = false;
    this.lastFindText = '';
  }

  public selectFindLabel(direction: string) {
    if (this.findLabels.length === 0) {
      return;
    }
    let selectedIndex = 0;
    const labels = this.findLabels;
    for (let i = 0; i < labels.length; i++) {
      if (labels[i].isSelected) {
        labels[i].setSelected(false);
        selectedIndex = i;
        break;
      }
    }
    if (direction === 'Next') {
      selectedIndex++;
      if (selectedIndex > labels.length - 1) {
        selectedIndex = 0;
      }
    } else {
      selectedIndex--;
      if (selectedIndex < 0) {
        selectedIndex = labels.length - 1;
      }
    }
    labels[selectedIndex].setSelected(true);
    this.goToFindedElement(labels[selectedIndex]);
  }

  public showFindLabels() {
    this.hideFindLabels();
    this.findMode = true;
    this.lastFindText = this.text;
    const text = this.matchCase ? this.text : this.text.toLowerCase();

    this.model.pages.forEach(page => {
      const pageElements = page.page.getElementsByTagName('*');
      for (const pageElement of pageElements) {
        let innerText = pageElement.innerHTML;
        if (innerText && pageElement.childNodes.length === 1 && pageElement.childNodes[0].nodeName === '#text') {
          if (!this.matchCase) {
            innerText = innerText.toLowerCase();
          }
          if (innerText.indexOf(text) >= 0) {
            if (this.matchWholeWord && !this.isWholeWord(innerText, text)) {
              continue;
            }
            const label: any = document.createElement('div');
            label.ownerElement = pageElement;
            label.className = 'stiJsViewerFindLabel';
            label.style.width = (pageElement.offsetWidth - 4) + 'px';
            const labelHeight = pageElement.offsetHeight - 4;
            label.style.height = labelHeight + 'px';
            label.style.top = '0px';
            label.style.left = '0px';
            label.ownerElement.oldPositionStyle = label.ownerElement.style.position;
            if (label.ownerElement.style.position !== 'absolute' && label.ownerElement.style.position !== 'fixed') {
              label.ownerElement.style.position = 'relative';
            }
            pageElement.insertBefore(label, pageElement.childNodes[0]);

            label.setSelected = function (state) {
              this.isSelected = state;
              this.style.border = '2px solid ' + (state ? 'red' : '#8a8a8a');
            };

            if (this.findLabels.length === 0) {
              label.setSelected(true);
            }
            this.findLabels.push(label);
          }
        }
      }
    });

    if (this.findLabels.length > 0) {
      this.goToFindedElement(this.findLabels[0]);
    }
  }

  goToFindedElement(findLabel: any) {
    if (findLabel && findLabel.ownerElement) {
      const targetTop = this.helper.findPosY(findLabel.ownerElement, this.model.options.appearance.scrollbarsMode ? 'stiJsViewerReportPanel' : null, true) - findLabel.ownerElement.offsetHeight - 50;
      const d = new Date();
      const endTime = d.getTime() + this.model.options.scrollDuration;
      this.animationService.showAnimationForScroll(this.model.controls.reportPanel.el.nativeElement, targetTop, endTime, () => { });
    }
  }

  isWholeWord(str: string, word: string): boolean {
    const index = str.indexOf(word);
    const preSymbol = str.substring(index - 1, index);
    const nextSymbol = str.substring(index + word.length, index + word.length + 1);

    return ((preSymbol === '' || this.symbols.indexOf(preSymbol) === -1) && (nextSymbol === '' || this.symbols.indexOf(nextSymbol) === -1));
  }
}
