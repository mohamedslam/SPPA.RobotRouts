import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { ModelService } from '../services/model.service';
import { HelperService } from '../services/helper.service';
import { ControllerService } from '../services/controller.service';
import { BookmarkNode, Message } from '../services/objects';
import { trigger, state, transition, animate, style, keyframes, AnimationEvent } from '@angular/animations';

@Component({
  selector: 'sti-bookmarks-panel',
  template: `
    <div #element [style]="{fontFamily: model.options.toolbar.fontFamily, fontColor: helper.val(model.options.toolbar.fontColor)}"
      [class]="'stiJsViewerBookmarksPanel' + (model.options.toolbar.displayMode == 'Separated' ? ' stiJsViewerBookmarksPanelSeparated' : '')"
      [style.width.px]="model.options.appearance.bookmarksTreeWidth - (model.options.toolbar.displayMode == 'Simple' ? 0 : 1)"
      [style.bottom]="bottom"
      [style.top.px]="model.controls.bookmarksPanel.layout.top"
      [style.transition]="model.options.isMobileDevice ? 'opacity 300ms ease' : null"
      [style.display]="!this.model.options.isMobileDevice ? (model.controls.bookmarksPanel.visible ? '' : 'none') : null"
      [@visibility]="!this.model.options.isMobileDevice ? null : (model.controls.bookmarksPanel.visible ? 'visible' : 'hidden')">
      <div #bookmarksPanel [class]="'stiJsViewerBookmarksContainer' + (model.options.toolbar.displayMode == 'Simple' ? ' stiJsViewerBookmarksContainerSimple' : '')"
           [style.background]="helper.val(model.options.toolbar.backgroundColor)"
           [style.border]="helper.val(model.options.toolbar.borderColor) != '' ? '1px solid ' + helper.val(model.options.toolbar.borderColor): ''">
           <div class="stiTree">
              <div class="stiTreeNode">
                <img style="width: 16px; height: 16px;" [src]="model.imagesForBookmark['root']"/>
                <a class="node">{{rootName}}</a>
              </div>
              <div class="clip" style="display:block;">
                <ng-container *ngFor="let node of model.nodes; index as i">
                    <div class="stiTreeNode">
                        <a (click)="node.open = !node.open">
                            <img style="width: 18px; height: 18px"
                             [src]="getImg1(node, i)"/>
                        </a>
                        <img style="width: 16px; height: 16px;" [src]="node.nodes?.length == 0 ? model.imagesForBookmark['node'] :(node.open ? model.imagesForBookmark['folderOpen'] : model.imagesForBookmark['folder'])"/>
                        <a [class]="node.selected ? 'nodeSel' : 'node'" (click)="postAction(node)">{{node.name}}</a>
                    </div>
                    <div class="clip" [style.display]="node.open ? 'block' : 'none'">
                      <div *ngFor="let subNode of node.nodes; index as k" class="stiTreeNode">
                        <img style="width: 18px; height: 18px;" [src]="i != model.nodes.length - 1 ? model.imagesForBookmark['line'] : model.imagesForBookmark['empty']"/>
                        <img style="width: 18px; height: 18px;" [src]="k == node.nodes.length - 1 ? model.imagesForBookmark['joinBottom'] : model.imagesForBookmark['join']"/>
                        <img style="width: 16px; height: 16px;" [src]="model.imagesForBookmark['node']" />
                        <a [class]="subNode.selected ? 'nodeSel' : 'node'" (click)="postAction(subNode)">{{subNode.name}}</a>
                      </div>
                    </div>
                </ng-container>
              </div>
          </div>
      </div>
    </div>
  `,
  animations: [
    trigger('visibility', [
      state('visible', style({ opacity: 1, display: 'block' })),
      state('hidden', style({ opacity: 0, display: 'none' })),
      transition('hidden => visible', [
        animate('300ms ease-in-out', keyframes([
          style({ display: 'block', opacity: 0, offset: 0 }),
          style({ display: 'block', opacity: 1, offset: 1 }),
        ]))
      ]),
      transition('visible => hidden', [
        animate('300ms ease-in-out', keyframes([
          style({ display: 'block', opacity: 1, offset: 0 }),
          style({ display: 'none', opacity: 0, offset: 1 }),
        ]))
      ])
    ])
  ]
})

export class BookmarksPanelComponent implements OnInit, AfterViewInit {

  @ViewChild('element') element: ElementRef;
  @ViewChild('bookmarksPanel') bookmarksPanel: ElementRef;

  public rootName: string;

  constructor(public model: ModelService, public helper: HelperService, public controller: ControllerService) {
    controller.getMessage().subscribe((message: Message) => {
      if (message.action === 'GetReport' || message.action === 'OpenReport') {
        setTimeout(() => {
          if (this.model.reportParams.bookmarksContent != null) {
            this.create();
          }
          this.model.controls.bookmarksPanel.visible = this.model.reportParams.bookmarksContent !== null && !this.model.options.isMobileDevice;
        });
      } else {
        // Go to the bookmark, if it present
        setTimeout(() => {
          if (this.model.options.bookmarkAnchor != null) {
            this.helper.scrollToAnchor(this.model.options.bookmarkAnchor, this.model.options.componentGuid);
            this.model.options.bookmarkAnchor = null;
            this.model.options.componentGuid = null;
          }
        });
      }
    });

    this.model.controls.bookmarksPanel.getVisibility().subscribe((value) => {
      if (!value) {
        this.helper.removeBookmarksLabel();
        this.clearSelected();
      }
    });
  }

  ngAfterViewInit(): void {
    this.model.controls.bookmarksPanel.el = this.element;
  }

  ngOnInit() { }

  public getImg1(node: BookmarkNode, i: number): string {
    if (node.nodes?.length === 0) {
      return i !== this.model.nodes.length - 1 ? this.model.imagesForBookmark['join'] : this.model.imagesForBookmark['joinBottom'];
    }
    return node.open ? (i === this.model.nodes.length - 1 ? this.model.imagesForBookmark['minusBottom'] : this.model.imagesForBookmark['minus']) :
      (i === this.model.nodes.length - 1 ? this.model.imagesForBookmark['plusBottom'] : this.model.imagesForBookmark['plus']);
  }

  public postAction(node: BookmarkNode) {
    this.clearSelected();
    this.controller.postBookmarkNodeAction(node);
  }

  clearSelected() {
    this.model.nodes?.forEach((n) => {
      n.selected = false;
      n.nodes.forEach(element => element.selected = false);
    });
  }

  create() {
    const bookmarks = this.model.reportParams.bookmarksContent.split('bookmarks.add(');

    const root = bookmarks[1].replace('0,-1,\'', '');
    this.rootName = root.substr(0, root.indexOf('\''));

    const nodes: BookmarkNode[] = [];
    this.parseNodes(bookmarks.splice(2), 0, nodes);
    this.model.nodes = nodes;
  }

  parseNodes(bookmarks: string[], index: number, nodes: BookmarkNode[]) {
    let folder: BookmarkNode;
    bookmarks.forEach((bookmark) => {
      const str = bookmark.substr(bookmark.indexOf(',') + 1);
      const nodeType = parseInt(str.substr(0, str.indexOf(',')), 10);
      const node = this.parseNode(str);
      if (nodeType === 0) {
        nodes.push(node);
        folder = node;
      } else {
        folder.nodes.push(node);
      }
    });
  }

  parseNode(str: string): BookmarkNode {
    str = str.substr(str.indexOf(',') + 2);
    const name = this.unescape(str.substr(0, str.indexOf('\',')));
    str = str.substr(str.indexOf('\',') + 3);
    const url = str.substr(0, str.indexOf('\','));
    str = str.substr(str.indexOf('\',') + 3);
    const pageTitle = str.substr(0, str.indexOf('\','));
    str = str.substr(str.indexOf('\',') + 3);
    const componentGuid = str.substr(0, str.length - 3);
    return { name, url, page: parseInt(pageTitle.substr(5), 10) - 1, compunentGuid: componentGuid, nodes: [], open: false, selected: false };
  }

  unescape(str: string): string {
    return str.replace(/\\&apos;/g, '\'')
      .replace(/\\&quot;/g, '"')
      .replace(/\\&gt;/g, '>')
      .replace(/\\&lt;/g, '<')
      .replace(/\\&amp;/g, '&');
  }

  get bottom(): string {
    if (this.model.options.isMobileDevice) {
      return this.model.options.toolbar.autoHide ? '0' : '0.5in';
    } else {
      return this.model.options.toolbar.displayMode === 'Separated' && this.model.options.toolbar.visible ? '35px' : '0';
    }
  }

}
