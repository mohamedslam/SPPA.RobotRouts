// 0 - Default BackGroundColor (if Brush == "none" or "transparent")
// 1 - Default Border (if Border == "none")
// 2 - Header Type
// 3 - Type Name Text
// 4 - ResizingIcons
// 5 - ShowPositionsProperty
// 6 - ShowCorners
// 7 - ShowEditIcon

var ComponentCollection =
{
    "StiText": ["#ffffff", "none", "none", "none", "0,1,2,3", "1,1,1,1", "1", true],
    "StiTextInCells": ["#ffffff", "none", "none", "none", "0,1,2,3", "1,1,1,1", "1", true],
    "StiRichText": ["transparent", "none", "none", "center", "0,1,2,3", "1,1,1,1", "1", true],
    "StiImage": ["#005600", "none", "none", "center", "0,1,2,3", "1,1,1,1", "1", true],
    "StiBarCode": ["transparent", "none", "none", "none", "0,1,2,3", "1,1,1,1", "1", true],
    "StiShape": ["transparent", "none", "none", "none", "0,1,2,3", "1,1,1,1", "1", true],
    "StiPanel": ["#f1f1f1", "3,1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false],
    "StiClone": ["#9e9e9e", "3,1", "none", "center", "0,1,2,3", "1,1,1,1", "0", true],
    "StiCheckBox": ["transparent", "none", "none", "none", "0,1,2,3", "1,1,1,1", "1", false],
    "StiSubReport": ["#b8dde9", "3,1", "none", "center", "0,1,2,3", "1,1,1,1", "0", true],
    "StiZipCode": ["transparent", "none", "none", "none", "0,1,2,3", "1,1,1,1", "1", true],
    "StiChart": ["transparent", "3,1", "none", "none", "0,1,2,3", "1,1,1,1", "0", true],
    "StiGauge": ["transparent", "3,1", "none", "none", "0,1,2,3", "1,1,1,1", "0", true],
    "StiMap": ["#005600", "3,1", "none", "none", "0,1,2,3", "1,1,1,1", "0", true],
    "StiPageHeaderBand": ["#cecfce", "3,1", "up", "up", "0,3", "0,0,0,1", "0", false],
    "StiPageFooterBand": ["#cecfce", "3,1", "up", "up", "0,3", "0,0,0,1", "0", false],
    "StiReportTitleBand": ["#9fd5b7", "3,1", "up", "up", "0,3", "0,0,0,1", "0", false],
    "StiGroupHeaderBand": ["#ef9b34", "3,1", "up", "up", "0,3", "0,0,0,1", "0", true],
    "StiGroupFooterBand": ["#ef9b34", "3,1", "up", "up", "0,3", "0,0,0,1", "0", false],
    "StiHeaderBand": ["#b2c5df", "3,1", "up", "up", "0,3", "0,0,0,1", "0", false],
    "StiFooterBand": ["#b2c5df", "3,1", "up", "up", "0,3", "0,0,0,1", "0", false],
    "StiColumnHeaderBand": ["#ef6d49", "3,1", "up", "up", "0,3", "0,0,0,1", "0", false],
    "StiColumnFooterBand": ["#ef6d49", "3,1", "up", "up", "0,3", "0,0,0,1", "0", false],
    "StiDataBand": ["#5a93cc", "3,1", "up", "up", "0,3", "0,0,0,1", "0", true],
    "StiTable": ["#90c0f1", "3,1", "up", "up", "0,3", "0,0,0,0", "0", true],
    "StiTableCell": ["#ffffff", "none", "none", "none", "0,1,2,3", "0,0,0,0", "1", false],
    "StiTableCellImage": ["#005600", "none", "none", "center", "0,1,2,3", "0,0,0,0", "1", false],
    "StiTableCellRichText": ["transparent", "none", "none", "center", "0,1,2,3", "0,0,0,0", "1", false],
    "StiTableCellCheckBox": ["transparent", "none", "none", "none", "0,1,2,3", "0,0,0,0", "1", false],
    "StiHierarchicalBand": ["#76a797", "3,1", "up", "up", "0,3", "0,0,0,1", "0", true],
    "StiChildBand": ["#b2deae", "3,1", "up", "up", "0,3", "0,0,0,1", "0", false],
    "StiEmptyBand": ["#baeb89", "3,1", "up", "up", "0,3", "0,0,0,1", "0", false],
    "StiReportSummaryBand": ["#9fd5b7", "3,1", "up", "up", "0,3", "0,0,0,1", "0", false],
    "StiOverlayBand": ["#837cae", "3,1", "up", "up", "0,3", "0,0,0,1", "0", false],
    "StiTableOfContents": ["#efd090", "3,1", "up", "up", "0,3", "0,0,0,1", "0", false],
    "StiCrossTab": ["#71bb00", "", "none", "center", "0,1,2,3", "1,1,1,1", "0", true],
    "StiCrossGroupHeaderBand": ["#ffac16", "", "down", "down", "0,1", "0,0,1,0", "0", true],
    "StiCrossGroupFooterBand": ["#ffac16", "", "down", "down", "0,1", "0,0,1,0", "0", false],
    "StiCrossHeaderBand": ["#bacae3", "", "down", "down", "0,1", "0,0,1,0", "0", false],
    "StiCrossFooterBand": ["#bacae3", "", "down", "down", "0,1", "0,0,1,0", "0", false],
    "StiCrossDataBand": ["#6c9cec", "", "down", "down", "0,1", "0,0,1,0", "0", true],
    "StiCrossField": ["#ffffff", "none", "none", "none", "0,1,2,3", "1,1,1,1", "1", false],
    "StiSparkline": ["transparent", "3,1", "none", "none", "0,1,2,3", "1,1,1,1", "0", true],
    "StiMathFormula": ["transparent", "3,1", "none", "none", "0,1,2,3", "1,1,1,1", "0", true],
    "StiElectronicSignature": ["#5a93cc", "3,1", "none", "none", "0,1,2,3", "1,1,1,1", "0", true],
    "StiPdfDigitalSignature": ["#5a93cc", "3,1", "none", "none", "0,1,2,3", "1,1,1,1", "0", true],

    "StiHorizontalLinePrimitive": ["transparent", "none", "none", "none", "0,1", "1,1,1,0", "0", false],
    "StiVerticalLinePrimitive": ["transparent", "none", "none", "none", "0,3", "1,1,0,1", "0", false],
    "StiRectanglePrimitive": ["transparent", "none", "none", "none", "0,1,2,3", "1,1,1,1", "0", false],
    "StiRoundedRectanglePrimitive": ["transparent", "none", "none", "none", "0,1,2,3", "1,1,1,1", "0", false],

    "StiTableElement": ["#ffffff", "1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false],
    "StiChartElement": ["#ffffff", "1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false],
    "StiGaugeElement": ["#ffffff", "1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false],
    "StiPivotTableElement": ["#ffffff", "1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false],
    "StiIndicatorElement": ["#ffffff", "1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false],
    "StiProgressElement": ["#ffffff", "1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false],
    "StiRegionMapElement": ["#ffffff", "1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false],
    "StiOnlineMapElement": ["#ffffff", "1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false],
    "StiImageElement": ["#ffffff", "1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false],
    "StiTextElement": ["#ffffff", "1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false],
    "StiPanelElement": ["#ffffff", "1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false],
    "StiShapeElement": ["#ffffff", "1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false],
    "StiListBoxElement": ["#ffffff", "1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false],
    "StiComboBoxElement": ["#ffffff", "1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false],
    "StiTreeViewElement": ["#ffffff", "1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false],
    "StiTreeViewBoxElement": ["#ffffff", "1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false],
    "StiDatePickerElement": ["#ffffff", "1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false],
    "StiCardsElement": ["#ffffff", "1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false],
    "StiButtonElement": ["#ffffff", "1", "none", "center", "0,1,2,3", "1,1,1,1", "0", false]
};