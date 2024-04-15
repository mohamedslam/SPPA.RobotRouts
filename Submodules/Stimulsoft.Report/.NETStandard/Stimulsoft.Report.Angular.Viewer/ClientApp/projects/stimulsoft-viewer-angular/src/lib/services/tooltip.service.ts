import { Injectable } from '@angular/core';
import { ModelService } from './model.service';
import { MenuService } from '../menu/menu.service';

@Injectable()
export class TooltipService {

  public state = 'initial';
  public caption = '';
  public helpLink = '';
  public top = 0;
  public left = 0;

  private innerState = 'initial';
  private timeout: any;

  constructor(public model: ModelService, public menuService: MenuService) { }

  public show(left: number, top: number, caption: string, below: boolean = true, helpLink?: string) {
    if (this.menuService.isMenuVisible() || !this.model.options.appearance.showTooltips) {
      this.hideImmediately();
      return;
    }

    if (this.caption === caption && this.helpLink === helpLink && this.state !== 'initial') {
      this.showInternal();
      return;
    }

    this.caption = caption;
    this.helpLink = helpLink;
    this.state = this.innerState = this.state !== 'expanded' ? 'preInitial' : 'preInitial2';

    setTimeout(() => {
      const tooltip = this.model.controls.tooltip.el.nativeElement;
      const browserWidth = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
      if (!below) {
        top -= tooltip.offsetHeight;
      }
      if (left + tooltip.offsetWidth > browserWidth) {
        left = browserWidth - tooltip.offsetWidth;
      }

      this.left = left;
      this.top = top;
      if (this.innerState !== 'initial') {
        this.showInternal();
      }
    });
  }

  private showInternal() {
    this.state = 'expanded';
    this.innerState = 'expanded';
  }

  public showFromTooltip() {
    if (this.state !== 'preInitial') {
      this.state = 'expanded';
      this.innerState = 'expanded';
    }
  }

  public hideImmediately() {
    this.innerState = 'initial';
    this.state = 'initial';
  }

  public hide() {
    this.innerState = 'initial';
    if (this.timeout) {
      clearTimeout(this.timeout);
    }
    this.timeout = setTimeout(() => {
      if (this.innerState === 'initial') {
        this.state = 'initial';
      }
      this.timeout = null;
    }, 1000);
  }

}
