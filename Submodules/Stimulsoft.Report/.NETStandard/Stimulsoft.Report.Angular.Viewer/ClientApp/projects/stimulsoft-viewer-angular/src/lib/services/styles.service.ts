import { Injectable } from '@angular/core';
import { ModelService } from './model.service';

@Injectable()
export class StylesService {
  private pagesCss = {};

  constructor(private model: ModelService) { }

  public setupStyle(style: string, id: string) {
    if (this.pagesCss[id] == null) {
      this.pagesCss[id] = document.createElement('STYLE');
      this.pagesCss[id].id = this.model.options?.viewerId + 'Styles';
      this.pagesCss[id].setAttribute('type', 'text/css');
      this.model.controls.head.appendChild(this.pagesCss[id]);
    }
    if (this.pagesCss[id].styleSheet) {
      this.pagesCss[id].styleSheet.cssText = style;
    } else {
      this.pagesCss[id].innerHTML = style;
    }
  }

  public addCustomFontStyles(customFonts: any[]) {
    if (!customFonts) { return; }

    let existsStyles: any = [];
    try {
      existsStyles = document.getElementsByTagName('head')[0].getElementsByTagName('style');
    } catch (e) { }

    customFonts.forEach((customFont: any) => {
      if (customFont.contentForCss && customFont.originalFontFamily) {
        const style = document.createElement('style');
        style.innerHTML = `@font-face {\r\nfont-family: '${customFont.originalFontFamily}';\r\n src: url(${customFont.contentForCss});\r\n }`;

        let existsThisStyle = false;

        for (const estyle of existsStyles) {
          if (estyle.innerHTML.indexOf(`font-family: '${customFont.originalFontFamily}'`) > 0) {
            existsThisStyle = true;
            break;
          }
        }

        if (!existsThisStyle) {
          document.getElementsByTagName('head')[0].appendChild(style);
        }
      }
    });
  }


  public addChartScript(script: string) {
    const currChartScripts = document.getElementById(this.model.options.viewerId + 'chartScriptJsViewer');
    if (currChartScripts) {
      this.model.controls.head.removeChild(currChartScripts);
    }

    if (script) {
      const chartScripts = document.createElement('Script');
      chartScripts.setAttribute('type', 'text/javascript');
      chartScripts.id = this.model.options.viewerId + 'chartScriptJsViewer';
      chartScripts.textContent = script;
      this.model.controls.head.appendChild(chartScripts);
    }
  }


}
