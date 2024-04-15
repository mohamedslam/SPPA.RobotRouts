import { Injectable } from '@angular/core';
import { Form } from '../services/objects';
import { MouseService } from '../services/mouse.service';
import { MenuService } from '../menu/menu.service';
import { HelperService } from '../services/helper.service';
import { BaseFormComponent } from './base-form.component';
import { ModelService } from '../services/model.service';

@Injectable()
export class FormService {

  private mouseX = 0;
  private mouseY = 0;
  private formX = 0;
  private formY = 0;

  constructor(public mouseService: MouseService, public menuService: MenuService, public helper: HelperService, public model: ModelService) {
    mouseService.getDocumentMouseMove().subscribe((event: MouseEvent) => {
      this.move(event);
    });

    mouseService.getDocumentMouseUp().subscribe((event: MouseEvent) => {
      this.stopMove();
    });
  }

  public get form(): Form {
    return this.model.form;
  }

  public set form(form: Form) {
    this.model.form = form;
  }

  public showForm(name: string, formData?: any) {
    this.form = { name, left: 0, top: 0, isMooving: false, formData };
  }

  public closeForm(name: string) {
    if (this.form?.name === 'notificationForm' && this.model.notificationFormOptions?.action) {
      this.model.notificationFormOptions.action();
    }
    this.form = null;
  }

  public centerForm(baseForm: BaseFormComponent, defaultTop: number) {
    if (this.form != null) {
      setTimeout(() => {
        const position = this.helper.setObjectToCenter(baseForm.element.nativeElement, defaultTop);
        this.form.left = position.left;
        this.form.top = position.top;
        this.form.level = baseForm.level;
      });
    }
  }

  public startMove(name: string, event: MouseEvent, touchEvent?: TouchEvent) {
    if (event || touchEvent && touchEvent.changedTouches.length > 0) {
      this.formX = this.form.left;
      this.formY = this.form.top;
      this.mouseX = event?.screenX || touchEvent.changedTouches[0].screenX;
      this.mouseY = event?.screenY || touchEvent.changedTouches[0].screenY;
      this.form.isMooving = true;
    }
  }

  public move(event: any) {
    if (this.form?.isMooving) {
      const screenX = event.screenX || (event.changedTouches.length > 0 ? event.changedTouches[0].screenX : -1);
      const screenY = event.screenY || (event.changedTouches.length > 0 ? event.changedTouches[0].screenY : -1);
      this.form.left = this.formX + (screenX - this.mouseX);
      this.form.top = this.formY + (screenY - this.mouseY);
      this.menuService.closeAllMenus();
    }
  }

  public stopMove() {
    if (this.form?.isMooving) {
      this.form.isMooving = false;
    }
  }

}
