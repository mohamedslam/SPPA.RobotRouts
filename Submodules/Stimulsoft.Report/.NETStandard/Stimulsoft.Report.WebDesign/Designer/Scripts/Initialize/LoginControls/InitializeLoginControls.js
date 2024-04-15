
StiMobileDesigner.prototype.InitializeLoginControls = function () {
    //Override designer
    this.options.STI_DATE_TIME_FORMAT = "dd.MM.yyyy h:nn";
    this.options.STI_DATE_TIME_FORMAT_SEC = "dd.MM.yyyy h:nn:ss";
    this.options.STI_DATE_FORMAT = "dd.MM.yyyy";
    this.options.maxUploadFileSize = 10000000;
    this.options.cookiesDomain = ".stimulsoft.com";
    this.options.cookiesPath = "/";

    this.AddLocalizations();
    this.OverrideCreateHTMLTable();
    this.OverrideTextbox();
    this.OverrideToolbar();
    this.OverrideOptionsForm();
    this.OverrideBaseForm();
    this.InitMessagesTooltip();
    this.InitializeAuthForm();
}