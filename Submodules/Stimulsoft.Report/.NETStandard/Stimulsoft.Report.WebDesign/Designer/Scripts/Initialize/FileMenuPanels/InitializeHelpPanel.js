
StiMobileDesigner.prototype.InitializeHelpPanel = function () {
    var helpPanel = document.createElement("div");
    var jsObject = helpPanel.jsObject = this;
    this.options.helpPanel = helpPanel;
    this.options.mainPanel.appendChild(helpPanel);
    helpPanel.style.display = "none";
    helpPanel.className = "stiDesignerNewReportPanel";

    var header = this.FileMenuPanelHeader(this.loc.Buttons.Help);
    helpPanel.appendChild(header);

    var mainTable = this.CreateHTMLTable();
    helpPanel.appendChild(mainTable);
    mainTable.style.margin = "10px 30px 0px 30px";
    mainTable.style.height = "calc(100% - 130px)";

    var mainButtonsTable = this.CreateHTMLTable();
    mainTable.addCell(mainButtonsTable).className = "wizardFormStepsPanel";

    //Main Buttons
    var buttons = [];
    buttons.push(["helpDocumentation", this.loc.MainMenu.menuHelpDocumentation, "Help.HelpDocumentation.png"]);
    buttons.push(["helpSupport", this.loc.MainMenu.menuHelpSupport.replace("&", ""), "Help.HelpSupport.png"]);
    buttons.push(["helpVideos", this.loc.MainMenu.menuHelpVideos, "Help.HelpVideos.png"]);
    buttons.push(["helpSamples", this.loc.MainMenu.menuHelpSamples, "Help.HelpSamples.png"]);
    buttons.push(["helpForum", this.loc.MainMenu.menuHelpForum, "Help.HelpForum.png"]);
    buttons.push(["helpDemos", this.loc.MainMenu.menuHelpDemos, "Help.HelpDemos.png"]);
    buttons.push(["helpTrainingCourses", this.loc.MainMenu.menuHelpTrainingCourses, "Help.HelpTrainingCourses.png"]);

    for (var i = 0; i < buttons.length; i++) {
        var button = this.FileMenuInnerPanelButton(buttons[i][0], null, buttons[i][1], buttons[i][2]);
        button.style.margin = "0 6px 3px 0";
        mainButtonsTable.addCellInNextRow(button);

        button.action = function () {
            var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
            fileMenu.changeVisibleState(false);

            switch (this.name) {
                case "helpDocumentation":
                    setTimeout(function () { jsObject.openNewWindow("https://www.stimulsoft.com/" + (jsObject.options.helpLanguage || "en") + "/documentation/online/user-manual/index.html?introduction.htm"); }, 200);
                    break;

                case "helpSupport":
                    setTimeout(function () { jsObject.openNewWindow("https://www.stimulsoft.com/" + (jsObject.options.helpLanguage || "en") + "/support"); }, 200);
                    break;

                case "helpVideos":
                    setTimeout(function () { jsObject.openNewWindow("http://www.youtube.com/user/StimulsoftVideos"); }, 200);
                    break;

                case "helpSamples":
                    setTimeout(function () { jsObject.openNewWindow("https://www.stimulsoft.com/en/samples"); }, 200);
                    break;

                case "helpForum":
                    setTimeout(function () { jsObject.openNewWindow((jsObject.options.helpLanguage == "ru" ? "https://forumru" : "https://forum") + ".stimulsoft.com/"); }, 200);
                    break;

                case "helpDemos":
                    setTimeout(function () { jsObject.openNewWindow("http://demo.stimulsoft.com/"); }, 200);
                    break;

                case "helpTrainingCourses":
                    setTimeout(function () { jsObject.openNewWindow("https://www.stimulsoft.com/en/training-courses"); }, 200);
                    break;
            }
        }
    }

    helpPanel.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
    }

    return helpPanel;
}