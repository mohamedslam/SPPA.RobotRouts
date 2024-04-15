
StiMobileDesigner.prototype.AddDashboard = function (answer, notShowAfterCreated) {
    var page = this.CreateDashboard(answer);
    page.repaint();
    this.options.report.pages[page.properties.name] = page;
    this.options.report.pages[page.properties.name].components = {};
    this.ChangePageIndexes(answer.pageIndexes);
    this.options.paintPanel.addPage(page);
    if (!notShowAfterCreated) {
        this.options.paintPanel.showPage(page);
        page.setSelected();
    }
    this.UpdatePropertiesControls();
    this.options.pagesPanel.pagesContainer.updatePages();

    return page;
}