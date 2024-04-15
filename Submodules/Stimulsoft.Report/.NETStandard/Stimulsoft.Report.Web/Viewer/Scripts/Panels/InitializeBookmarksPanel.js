
StiJsViewer.prototype.InitializeBookmarksPanel = function () {
    if (this.reportParams.type == "Dashboard") return;

    var createAndShow = !this.options.isMobileDevice;
    if (this.controls.bookmarksPanel) {
        createAndShow = this.controls.bookmarksPanel.visible;
        this.controls.bookmarksPanel.changeVisibleState(false);
        this.controls.mainPanel.removeChild(this.controls.bookmarksPanel);
        delete this.controls.bookmarksPanel;
    }
    if (this.options.toolbar.visible && this.options.toolbar.showBookmarksButton) {
        this.controls.toolbar.controls.Bookmarks.setEnabled(this.reportParams.bookmarksContent != null);
    }
    if (!this.reportParams.bookmarksContent) return;

    var bookmarksPanel = document.createElement("div");
    bookmarksPanel.jsObject = this;
    bookmarksPanel.id = this.controls.viewer.id + "_BookmarksPanel";
    bookmarksPanel.style.fontFamily = this.options.toolbar.fontFamily;
    if (this.options.toolbar.fontColor != "") bookmarksPanel.style.color = this.options.toolbar.fontColor;
    this.controls.mainPanel.appendChild(bookmarksPanel);
    this.controls.bookmarksPanel = bookmarksPanel;

    bookmarksPanel.className = "stiJsViewerBookmarksPanel";
    if (this.options.toolbar.displayMode == "Separated") bookmarksPanel.className += " stiJsViewerBookmarksPanelSeparated";

    bookmarksPanel.style.width = this.options.appearance.bookmarksTreeWidth - (this.options.toolbar.displayMode == "Simple" ? 0 : 1) + "px";
    bookmarksPanel.style.display = "none";
    bookmarksPanel.visible = false;

    if (this.options.isMobileDevice) bookmarksPanel.style.bottom = this.options.toolbar.autoHide ? "0" : "0.5in";
    else bookmarksPanel.style.bottom = this.options.toolbar.displayMode == "Separated" && this.options.toolbar.visible ? "35px" : "0";

    var styleTop = this.options.toolbar.visible ? this.controls.toolbar.offsetHeight : 0;
    if (this.options.isMobileDevice && this.options.toolbar.autoHide) styleTop = 0;
    styleTop += this.controls.parametersPanel && (this.options.currentParametersPanelPosition || this.options.appearance.parametersPanelPosition) == "Top" ? this.controls.parametersPanel.offsetHeight : 0;
    styleTop += this.controls.findPanel ? this.controls.findPanel.offsetHeight : 0;
    styleTop += this.controls.drillDownPanel ? this.controls.drillDownPanel.offsetHeight : 0
    bookmarksPanel.style.top = styleTop + "px";

    if (this.options.isMobileDevice) bookmarksPanel.style.transition = "opacity 300ms ease";

    bookmarksPanel.container = document.createElement("div");
    bookmarksPanel.container.className = "stiJsViewerBookmarksContainer";

    if (this.options.toolbar.displayMode == "Simple") {
        bookmarksPanel.container.className += " stiJsViewerBookmarksContainerSimple";
    }

    if (this.options.toolbar.backgroundColor != "") {
        bookmarksPanel.container.style.background = this.options.toolbar.backgroundColor;
    }

    if (this.options.toolbar.borderColor != "") {
        bookmarksPanel.container.style.border = "1px solid " + this.options.toolbar.borderColor;
    }

    bookmarksPanel.appendChild(bookmarksPanel.container);

    bookmarksPanel.changeVisibleState = function (state) {
        var isStateChanged = this.visible != state;
        this.style.display = state ? "" : "none";
        this.visible = state;
        if (this.jsObject.options.toolbar.visible && this.jsObject.options.toolbar.showBookmarksButton) this.jsObject.controls.toolbar.controls.Bookmarks.setSelected(state);

        if (isStateChanged) this.jsObject.updateLayout();

        if (this.jsObject.options.isMobileDevice) {
            var controls = this.jsObject.controls;
            if (state && controls.parametersPanel) controls.parametersPanel.changeVisibleState(false);
            setTimeout(function () {
                bookmarksPanel.style.opacity = state ? "1" : "0";
                if (state) controls.reportPanel.hideToolbar();
                else controls.reportPanel.showToolbar();
            });
        }
    }

    bookmarksPanel.addContent = function (content) {
        this.container.innerHTML = content;
    }

    var imagesForBookmarks = this.GetImagesForBookmarks();
    var bookmarksContent = this.reportParams.bookmarksContent.replace("imagesForBookmarks", imagesForBookmarks);
    eval(bookmarksContent);
    // eslint-disable-next-line no-undef
    bookmarksPanel.addContent(bookmarks);
    if (createAndShow) bookmarksPanel.changeVisibleState(true);
}

StiJsViewer.prototype.GetImagesForBookmarks = function () {
    var names = ["root", "folder", "folderOpen", "node", "empty", "line", "join", "joinBottom", "plus", "plusBottom", "minus", "minusBottom"]
    var imagesForBookmarks = {};
    for (var i = 0; i < names.length; i++) {
        imagesForBookmarks[names[i]] = StiJsViewer.getImageSource(this.options, this.collections, "Bookmarks" + names[i] + ".png");
    }
    return JSON.stringify(imagesForBookmarks);
}

// Node object
function stiTreeNode(id, pid, name, url, pageTitle, componentGuid) {
    this.id = id;
    this.pid = pid;
    this.name = name;
    this.url = url ? url.replace(/'/g, "\\\'") : url;
    this.title = pageTitle;
    this.page == null;
    if (pageTitle) this.page = parseInt(pageTitle.substr(5)) - 1;
    this.componentGuid = componentGuid;
    this.target = null;
    this.icon = null;
    this.iconOpen = null;
    this._io = false; // Open
    this._is = false;
    this._ls = false;
    this._hc = false;
    this._ai = 0;
    this._p;

    //id, pid, name, url, page, componentGuid
}

// Tree object
function stiTree(objName, mobileViewerId, currentPageNumber, imagesForBookmarks) {

    this.config = {
        target: null,
        folderLinks: true,
        useSelection: true,
        useCookies: false,
        useLines: true,
        useIcons: true,
        useStatusText: false,
        closeSameLevel: false,
        inOrder: false
    }
    this.icon = {
        nlPlus: 'img/nolines_plus.gif',
        nlMinus: 'img/nolines_minus.gif'
    };

    for (var imageName in imagesForBookmarks) {
        this.icon[imageName] = imagesForBookmarks[imageName];
    }

    this.obj = objName;
    this.mobileViewerId = mobileViewerId;
    this.currentPageNumber = currentPageNumber;
    this.aNodes = [];
    this.aIndent = [];
    this.root = new stiTreeNode(-1);
    this.selectedNode = null;
    this.selectedFound = false;
    this.completed = false;
}

// Adds a new node to the node array
stiTree.prototype.add = function (id, pid, name, url, pageTitle, componentGuid) {
    this.aNodes[this.aNodes.length] = new stiTreeNode(id, pid, name, url, pageTitle, componentGuid);
}

// Open/close all nodes
stiTree.prototype.openAll = function () {
    this.oAll(true);
}

stiTree.prototype.closeAll = function () {
    this.oAll(false);
}

// Outputs the tree to the page
stiTree.prototype.toString = function () {
    var str = '<div class="stiTree">\n';
    if (document.getElementById) {
        if (this.config.useCookies) this.selectedNode = this.getSelected();
        str += this.addNode(this.root);
    } else str += 'Browser not supported.';
    str += '</div>';
    if (!this.selectedFound) this.selectedNode = null;
    this.completed = true;
    return str;
}

// Creates the tree structure
stiTree.prototype.addNode = function (pNode) {
    var str = '';
    var n = 0;
    if (this.config.inOrder) n = pNode._ai;
    for (n; n < this.aNodes.length; n++) {
        if (this.aNodes[n].pid == pNode.id) {
            var cn = this.aNodes[n];
            cn._p = pNode;
            cn._ai = n;
            this.setCS(cn);
            if (!cn.target && this.config.target) cn.target = this.config.target;
            if (cn._hc && !cn._io && this.config.useCookies) cn._io = this.isOpen(cn.id);
            if (!this.config.folderLinks && cn._hc) cn.url = null;
            if (this.config.useSelection && cn.id == this.selectedNode && !this.selectedFound) {
                cn._is = true;
                this.selectedNode = n;
                this.selectedFound = true;
            }
            str += this.node(cn, n);
            if (cn._ls) break;
        }
    }
    return str;
}

// Creates the node icon, url and text
stiTree.prototype.node = function (node, nodeId) {
    var str = '<div class="stiTreeNode">' + this.indent(node, nodeId);
    if (this.config.useIcons) {
        if (!node.icon) node.icon = (this.root.id == node.pid) ? this.icon.root : ((node._hc) ? this.icon.folder : this.icon.node);
        if (!node.iconOpen) node.iconOpen = (node._hc) ? this.icon.folderOpen : this.icon.node;
        if (this.root.id == node.pid) {
            node.icon = this.icon.root;
            node.iconOpen = this.icon.root;
        }
        str += '<img style=\'width: 16px; height: 16px;\' id="i' + this.obj + nodeId + '" src="' + ((node._io) ? node.iconOpen : node.icon) + '" alt="" />';
    }
    if (node.url) {
        str += '<a id="s' + this.obj + nodeId + '" class="' + ((this.config.useSelection) ? ((node._is ? 'nodeSel' : 'node')) : 'node') + '"';
        if (node.target) str += ' target="' + node.target + '"';
        if (this.config.useStatusText) str += ' onmouseover="window.status=\'' + node.name + '\';return true;" onmouseout="window.status=\'\';return true;" ';

        var clc = "";
        if (this.config.useSelection && ((node._hc && this.config.folderLinks) || !node._hc)) clc += this.obj + ".s(" + nodeId + ");";
        if (node.page != null) clc += "document.getElementById('" + this.mobileViewerId + "').jsObject.postAction('BookmarkAction'," + node.page + ",'" + node.url.substr(1) + "','" + node.componentGuid + "');";
        if (clc.length > 0 && node.page >= 0) str += ' onclick="' + clc + '"';

        str += '>';
    }
    else if ((!this.config.folderLinks || !node.url) && node._hc && node.pid != this.root.id)
        str += '<a href="javascript: ' + this.obj + '.o(' + nodeId + ');" class="node">';
    str += node.name;
    if (node.url || ((!this.config.folderLinks || !node.url) && node._hc)) str += '</a>';
    str += '</div>';
    if (node._hc) {
        str += '<div id="d' + this.obj + nodeId + '" class="clip" style="display:' + ((this.root.id == node.pid || node._io) ? 'block' : 'none') + ';">';
        str += this.addNode(node);
        str += '</div>';
    }
    this.aIndent.pop();
    return str;
}

// Adds the empty and line icons
stiTree.prototype.indent = function (node, nodeId) {
    var str = '';
    if (this.root.id != node.pid) {
        for (var n = 0; n < this.aIndent.length; n++)
            str += '<img style=\'width: 18px; height: 18px;\' src="' + ((this.aIndent[n] == 1 && this.config.useLines) ? this.icon.line : this.icon.empty) + '" alt="" />';
        (node._ls) ? this.aIndent.push(0) : this.aIndent.push(1);
        if (node._hc) {
            str += '<a href="javascript: ' + this.obj + '.o(' + nodeId + ');"><img style=\'width: 18px; height: 18px;\' id="j' + this.obj + nodeId + '" src="';
            if (!this.config.useLines) str += (node._io) ? this.icon.nlMinus : this.icon.nlPlus;
            else str += ((node._io) ? ((node._ls && this.config.useLines) ? this.icon.minusBottom : this.icon.minus) : ((node._ls && this.config.useLines) ? this.icon.plusBottom : this.icon.plus));
            str += '" alt="" /></a>';
        } else str += '<img style=\'width: 18px; height: 18px;\' src="' + ((this.config.useLines) ? ((node._ls) ? this.icon.joinBottom : this.icon.join) : this.icon.empty) + '" alt="" />';
    }
    return str;
}

// Checks if a node has any children and if it is the last sibling
stiTree.prototype.setCS = function (node) {
    var lastId;
    for (var n = 0; n < this.aNodes.length; n++) {
        if (this.aNodes[n].pid == node.id) node._hc = true;
        if (this.aNodes[n].pid == node.pid) lastId = this.aNodes[n].id;
    }
    if (lastId == node.id) node._ls = true;
}

// Returns the selected node
stiTree.prototype.getSelected = function () {
    var sn = this.getCookie('cs' + this.obj);
    return (sn) ? sn : null;
}

// Highlights the selected node
stiTree.prototype.s = function (id) {
    if (!this.config.useSelection) return;
    var cn = this.aNodes[id];
    if (cn._hc && !this.config.folderLinks) return;
    if (this.selectedNode != id) {
        if (this.selectedNode || this.selectedNode == 0) {
            var eOld = document.getElementById("s" + this.obj + this.selectedNode);
            eOld.className = "node";
        }
        var eNew = document.getElementById("s" + this.obj + id);
        eNew.className = "nodeSel";
        this.selectedNode = id;
        if (this.config.useCookies) this.setCookie('cs' + this.obj, cn.id);
    }
}

// Toggle Open or close
stiTree.prototype.o = function (id) {
    var cn = this.aNodes[id];
    this.nodeStatus(!cn._io, id, cn._ls);
    cn._io = !cn._io;
    if (this.config.closeSameLevel) this.closeLevel(cn);
    if (this.config.useCookies) this.updateCookie();
}

// Open or close all nodes
stiTree.prototype.oAll = function (status) {
    for (var n = 0; n < this.aNodes.length; n++) {
        if (this.aNodes[n]._hc && this.aNodes[n].pid != this.root.id) {
            this.nodeStatus(status, n, this.aNodes[n]._ls)
            this.aNodes[n]._io = status;
        }
    }
    if (this.config.useCookies) this.updateCookie();
}

// Opens the tree to a specific node
stiTree.prototype.openTo = function (nId, bSelect, bFirst) {
    if (!bFirst) {
        for (var n = 0; n < this.aNodes.length; n++) {
            if (this.aNodes[n].id == nId) {
                nId = n;
                break;
            }
        }
    }
    var cn = this.aNodes[nId];
    if (cn.pid == this.root.id || !cn._p) return;
    cn._io = true;
    cn._is = bSelect;
    if (this.completed && cn._hc) this.nodeStatus(true, cn._ai, cn._ls);
    if (this.completed && bSelect) this.s(cn._ai);
    else if (bSelect) this._sn = cn._ai;
    this.openTo(cn._p._ai, false, true);
}

// Closes all nodes on the same level as certain node
stiTree.prototype.closeLevel = function (node) {
    for (var n = 0; n < this.aNodes.length; n++) {
        if (this.aNodes[n].pid == node.pid && this.aNodes[n].id != node.id && this.aNodes[n]._hc) {
            this.nodeStatus(false, n, this.aNodes[n]._ls);
            this.aNodes[n]._io = false;
            this.closeAllChildren(this.aNodes[n]);
        }
    }
}

// Closes all children of a node
stiTree.prototype.closeAllChildren = function (node) {
    for (var n = 0; n < this.aNodes.length; n++) {
        if (this.aNodes[n].pid == node.id && this.aNodes[n]._hc) {
            if (this.aNodes[n]._io) this.nodeStatus(false, n, this.aNodes[n]._ls);
            this.aNodes[n]._io = false;
            this.closeAllChildren(this.aNodes[n]);
        }
    }
}

// Change the status of a node(open or closed)
stiTree.prototype.nodeStatus = function (status, id, bottom) {
    var eDiv = document.getElementById('d' + this.obj + id);
    var eJoin = document.getElementById('j' + this.obj + id);
    if (this.config.useIcons) {
        var eIcon = document.getElementById('i' + this.obj + id);
        eIcon.src = (status) ? this.aNodes[id].iconOpen : this.aNodes[id].icon;
    }
    eJoin.src = (this.config.useLines) ?
        ((status) ? ((bottom) ? this.icon.minusBottom : this.icon.minus) : ((bottom) ? this.icon.plusBottom : this.icon.plus)) :
        ((status) ? this.icon.nlMinus : this.icon.nlPlus);
    eDiv.style.display = (status) ? 'block' : 'none';
}

// [Cookie] Clears a cookie
stiTree.prototype.clearCookie = function () {
    var now = new Date();
    var yesterday = new Date(now.getTime() - 1000 * 60 * 60 * 24);
    this.setCookie('co' + this.obj, 'cookieValue', yesterday);
    this.setCookie('cs' + this.obj, 'cookieValue', yesterday);
}

// [Cookie] Sets value in a cookie
stiTree.prototype.setCookie = function (cookieName, cookieValue, expires, path, domain, secure) {
    document.cookie =
        escape(cookieName) + '=' + escape(cookieValue)
        + (expires ? '; expires=' + expires.toGMTString() : '')
        + (path ? '; path=' + path : '')
        + (domain ? '; domain=' + domain : '') + "; samesite=strict;" +
        + (secure ? '; secure' : '')
}

// [Cookie] Gets a value from a cookie
stiTree.prototype.getCookie = function (cookieName) {
    var cookieValue = '';
    var posName = document.cookie.indexOf(escape(cookieName) + '=');
    if (posName != -1) {
        var posValue = posName + (escape(cookieName) + '=').length;
        var endPos = document.cookie.indexOf(';', posValue);
        if (endPos != -1) cookieValue = unescape(document.cookie.substring(posValue, endPos));
        else cookieValue = unescape(document.cookie.substring(posValue));
    }
    return (cookieValue);
}

// [Cookie] Returns ids of open nodes as a string
stiTree.prototype.updateCookie = function () {
    var str = '';
    for (var n = 0; n < this.aNodes.length; n++) {
        if (this.aNodes[n]._io && this.aNodes[n].pid != this.root.id) {
            if (str) str += '.';
            str += this.aNodes[n].id;
        }
    }
    this.setCookie('co' + this.obj, str);
}

// [Cookie] Checks if a node id is in a cookie
stiTree.prototype.isOpen = function (id) {
    var aOpen = this.getCookie('co' + this.obj).split('.');
    for (var n = 0; n < aOpen.length; n++)
        if (aOpen[n] == id) return true;
    return false;
}

// If Push and pop is not implemented by the browser
if (!Array.prototype.push) {
    Array.prototype.push = function array_push() {
        for (var i = 0; i < arguments.length; i++)
            this[this.length] = arguments[i];
        return this.length;
    }
}

if (!Array.prototype.pop) {
    Array.prototype.pop = function array_pop() {
        var lastElement = this[this.length - 1];
        this.length = Math.max(this.length - 1, 0);
        return lastElement;
    }
}