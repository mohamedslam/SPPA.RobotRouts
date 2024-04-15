
//Loading Methods
StiMobileDesigner.prototype.LoadScriptWithProcessImage = function (src, callback, appendTo) {
    var processImage = this.options.processImage || this.InitializeProcessImage();
    processImage.show();

    this.LoadScript(src, function () {
        processImage.hide();
        callback();
    }, appendTo);
}

StiMobileDesigner.prototype.LoadScript = function (src, callback, appendTo) {
    var script = document.createElement('script');
    appendTo = appendTo || this.options.head;
    if (script.readyState && !script.onload) {
        // IE, Opera
        script.onreadystatechange = function () {
            if (script.readyState == 'loaded' || script.readyState == 'complete') {
                script.onreadystatechange = null;
                if (callback) callback();
            }
        }
    }
    else {
        // Rest
        if (callback) script.onload = callback;
    }
    script.src = src;
    appendTo.appendChild(script);
}

StiMobileDesigner.prototype.InitializeScriptText = function (scriptText) {
    var script = document.createElement('script');
    if (this.options.head) {
        script.innerHTML = scriptText;
        this.options.head.appendChild(script);
    }
    return script;
}

StiMobileDesigner.prototype.LoadStyle = function (src) {
    var link = document.createElement("link");
    link.setAttribute("rel", "stylesheet");
    link.setAttribute("type", "text/css");
    link.setAttribute("href", src);
    link.setAttribute("stimulsoft", "stimulsoft");
    this.options.head.appendChild(link);
}

StiMobileDesigner.prototype.ExecuteScript = function (scriptProps, callbackFunction, allwaysCreate, args) {
    var jsObject = this;

    if (jsObject[scriptProps.initMethod + "_"])
        callbackFunction(allwaysCreate ? jsObject[scriptProps.initMethod + "_"](args) : (jsObject.options.forms[scriptProps.formName] || jsObject[scriptProps.initMethod + "_"](args)));
    else
        jsObject.LoadScriptWithProcessImage(jsObject.options.scriptsUrl + scriptProps.scriptName, function () { callbackFunction(jsObject[scriptProps.initMethod + "_"](args)); });
}

StiMobileDesigner.prototype.InitializeBorderSetupForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeBorderSetupForm", formName: "borderSetup", scriptName: "InitializeBorderSetupForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeConditionsForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeConditionsForm", formName: "conditionsForm", scriptName: "InitializeConditionsForm" }, callbackFunction, true);
}

StiMobileDesigner.prototype.InitializeCreateDataForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeCreateDataForm", formName: "createDataForm", scriptName: "InitializeCreateDataForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeCreateStyleCollectionForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeCreateStyleCollectionForm", formName: "createStyleCollectionForm", scriptName: "InitializeCreateStyleCollectionForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeCrossTabForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeCrossTabForm", formName: "crossTabForm", scriptName: "InitializeCrossTabForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeBarCodeForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeBarCodeForm", formName: "barCodeForm", scriptName: "InitializeBarCodeForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeDataBusinessObjectForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeDataBusinessObjectForm", formName: "dataBusinessObject", scriptName: "InitializeDataBusinessObjectForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeDataColumnForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeDataColumnForm", formName: "dataColumn", scriptName: "InitializeDataColumnForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeDataForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeDataForm", formName: "dataForm", scriptName: "InitializeDataForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditCategoryForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditCategoryForm", formName: "editCategoryForm", scriptName: "InitializeEditCategoryForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditChartForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditChartForm", formName: "editChart", scriptName: "InitializeEditChartForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditGaugeForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditGaugeForm", formName: "editGauge", scriptName: "InitializeEditGaugeForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditColumnForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditColumnForm", formName: "editColumnForm", scriptName: "InitializeEditColumnForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditConnectionForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditConnectionForm", formName: "editConnectionForm", scriptName: "InitializeEditConnectionForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditDataSourceForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditDataSourceForm", formName: "editDataSourceForm", scriptName: "InitializeEditDataSourceForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditDataSourceFromOtherDatasourcesForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditDataSourceFromOtherDatasourcesForm", formName: "editDataSourceFromOtherDatasourcesForm", scriptName: "InitializeEditDataSourceFromOtherDatasourcesForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditParameterForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditParameterForm", formName: "editParameterForm", scriptName: "InitializeEditParameterForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditRelationForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditRelationForm", formName: "editRelationForm", scriptName: "InitializeEditRelationForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditRichTextForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditRichTextForm", formName: "richTextForm", scriptName: "InitializeEditRichTextForm" }, callbackFunction, true);
}

StiMobileDesigner.prototype.InitializeEditVariableForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditVariableForm", formName: "editVariableForm", scriptName: "InitializeEditVariableForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeExpressionEditorForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeExpressionEditorForm", formName: "expressionEditor", scriptName: "InitializeExpressionEditorForm" }, callbackFunction, true);
}

StiMobileDesigner.prototype.InitializeFilterForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeFilterForm", formName: "filterForm", scriptName: "InitializeFilterForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeGroupHeaderForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeGroupHeaderForm", formName: "groupHeaderForm", scriptName: "InitializeGroupHeaderForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeImageForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeImageForm", formName: "imageForm", scriptName: "InitializeImageForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeInteractionForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeInteractionForm", formName: "interactionForm", scriptName: "InitializeInteractionForm" }, callbackFunction, true);
}

StiMobileDesigner.prototype.InitializeMoreColorsForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeMoreColorsForm", formName: "moreColors", scriptName: "InitializeMoreColorsForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeNameInSourceForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeNameInSourceForm", formName: "nameInSourceForm", scriptName: "InitializeNameInSourceForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeOptionsForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeOptionsForm", formName: "optionsForm", scriptName: "InitializeOptionsForm" }, callbackFunction, true);
}

StiMobileDesigner.prototype.InitializePageSetupForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializePageSetupForm", formName: "pageSetup", scriptName: "InitializePageSetupForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeParametersValuesForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeParametersValuesForm", formName: "editParametersValuesForm", scriptName: "InitializeParametersValuesForm" }, callbackFunction, true);
}

StiMobileDesigner.prototype.InitializeReportSetupForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeReportSetupForm", formName: "reportSetupForm", scriptName: "InitializeReportSetupForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeSaveDescriptionForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeSaveDescriptionForm", formName: "saveDescriptionForm", scriptName: "InitializeSaveDescriptionForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeSelectConnectionForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeSelectConnectionForm", formName: "selectConnectionForm", scriptName: "InitializeSelectConnectionForm" }, callbackFunction, true);
}

StiMobileDesigner.prototype.InitializeSelectDataForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeSelectDataForm", formName: "selectDataForm", scriptName: "InitializeSelectDataForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeSortForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeSortForm", formName: "sortForm", scriptName: "InitializeSortForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeStyleDesignerForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeStyleDesignerForm", formName: "styleDesignerForm", scriptName: "InitializeStyleDesignerForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeSubReportForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeSubReportForm", formName: "subReportForm", scriptName: "InitializeSubReportForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeTextEditorForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeTextEditorForm", formName: "textEditor", scriptName: "InitializeTextEditorForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeTextEditorFormOnlyText = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeTextEditorFormOnlyText", formName: "textEditorOnlyText", scriptName: "InitializeTextEditorFormOnlyText" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeTextFormatForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeTextFormatForm", formName: "textFormatForm", scriptName: "InitializeTextFormatForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeVariableItemsForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeVariableItemsForm", formName: "variableItemsForm", scriptName: "InitializeVariableItemsForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeViewDataForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeViewDataForm", formName: "viewDataForm", scriptName: "InitializeViewDataForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeStyleConditionsForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeStyleConditionsForm", formName: "styleConditionsForm", scriptName: "InitializeStyleConditionsForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeCheckExpressionPopupPanel = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeCheckExpressionPopupPanel", formName: "checkPopupPanel", scriptName: "InitializeCheckExpressionPopupPanel" }, callbackFunction, true);
}

StiMobileDesigner.prototype.InitializeReportCheckForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeReportCheckForm", formName: "reportCheckForm", scriptName: "InitializeReportCheckForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeGlobalizationEditorForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeGlobalizationEditorForm", formName: "globalizationEditorForm", scriptName: "InitializeGlobalizationEditorForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditResourceForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditResourceForm", formName: "editResourceForm", scriptName: "InitializeEditResourceForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditCustomMapForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditCustomMapForm", formName: "editCustomMapForm", scriptName: "InitializeCustomMapForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeResourceViewDataForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeResourceViewDataForm", formName: "resourceViewDataForm", scriptName: "InitializeResourceViewDataForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeODataConnectionForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeODataConnectionForm", formName: "oDataConnectionForm", scriptName: "InitializeODataConnectionForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeSaveReportForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeSaveReportForm", formName: "saveReport", scriptName: "InitializeSaveReportForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeNewFolderForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeNewFolderForm", formName: "newFolder", scriptName: "InitializeNewFolderForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeWizardForm = function (callbackFunction) {
    this.ExecuteScript({
        initMethod: "InitializeWizardForm", formName: "wizardForm",
        scriptName: "InitializeWizardFormControls;InitializeWizardFormColumns;InitializeWizardFormColumnsOrder;InitializeWizardFormDataSource;" +
            "InitializeWizardFormFilter;InitializeWizardFormGroups;InitializeWizardFormLayout;InitializeWizardFormSort;InitializeWizardFormTheme;" +
            "InitializeWizardFormTotals;InitializeWizardRelationsForm;InitializeWizardForm"
    }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeWizardForm2 = function (callbackFunction) {
    this.ExecuteScript({
        initMethod: "InitializeWizardForm2", formName: "wizardForm2",
        scriptName: "InitializeWizardStepPanel;InitializeWizardThemePanel;InitializeWizardLabelsPanel;InitializeWizardMappingPanel;InitializeWizardCompanyPanel;InitializeWizardLanguagePanel;InitializeWizardSelectTemplatePanel;InitializeWizardSelectDatasourcePanel;InitializeWizardForm2"
    }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeSetupToolboxForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeSetupToolboxForm", formName: "setupToolboxForm", scriptName: "InitializeSetupToolboxForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeColorsCollectionForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeColorsCollectionForm", formName: "colorsCollectionForm", scriptName: "InitializeColorsCollectionForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeCloneContainerForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeCloneContainerForm", formName: "cloneContainerForm", scriptName: "InitializeCloneContainerForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeSelectColumnsForVariableForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeSelectColumnsForVariableForm", formName: "selectColumnsForVariable", scriptName: "InitializeSelectColumnsForVariableForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializePublishForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializePublishForm", formName: "publishForm", scriptName: "InitializePublishForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeShapeForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeShapeForm", formName: "shapeForm", scriptName: "InitializeShapeForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditDataSourceFromCrossTabForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditDataSourceFromCrossTabForm", formName: "editDataSourceFromCrossTabForm", scriptName: "InitializeEditDataSourceFromCrossTabForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeShareForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeShareForm", formName: "shareForm", scriptName: "InitializeShareForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditMapForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditMapForm", formName: "editMapForm", scriptName: "InitializeEditMapForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditTableElementForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditTableElementForm", formName: "editTableElementForm", scriptName: "InitializeEditTableElementForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditCardsElementForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditCardsElementForm", formName: "editCardsElementForm", scriptName: "InitializeEditCardsElementForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditImageElementForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditImageElementForm", formName: "editImageElementForm", scriptName: "InitializeEditImageElementForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditTextElementForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditTextElementForm", formName: "editTextElementForm", scriptName: "InitializeEditTextElementForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditRegionMapElementForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditRegionMapElementForm", formName: "editRegionMapElementForm", scriptName: "InitializeEditRegionMapElementForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditOnlineMapElementForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditOnlineMapElementForm", formName: "editOnlineMapElementForm", scriptName: "InitializeEditOnlineMapElementForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditProgressElementForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditProgressElementForm", formName: "editProgressElementForm", scriptName: "InitializeEditProgressElementForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditCardsElementForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditCardsElementForm", formName: "editCardsElementForm", scriptName: "InitializeEditCardsElementForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditIndicatorElementForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditIndicatorElementForm", formName: "editIndicatorElementForm", scriptName: "InitializeEditIndicatorElementForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditChartElementForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditChartElementForm", formName: "editChartElementForm", scriptName: "InitializeEditChartElementForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditGaugeElementForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditGaugeElementForm", formName: "editGaugeElementForm", scriptName: "InitializeEditGaugeElementForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditShapeElementForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditShapeElementForm", formName: "editShapeElementForm", scriptName: "InitializeEditShapeElementForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeDashboardSetupForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeDashboardSetupForm", formName: "dashboardSetup", scriptName: "InitializeDashboardSetupForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditPivotTableElementForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditPivotTableElementForm", formName: "editPivotTableElementForm", scriptName: "InitializeEditPivotTableElementForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditDataTransformationForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditDataTransformationForm", formName: "editDataTransformationForm", scriptName: "InitializeEditDataTransformationForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeFilterRulesForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeFilterRulesForm", formName: "filterRulesForm", scriptName: "InitializeFilterRulesForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeLimitRowsForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeLimitRowsForm", formName: "limitRowsForm", scriptName: "InitializeLimitRowsForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeRunningTotalForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeRunningTotalForm", formName: "runningTotalForm", scriptName: "InitializeRunningTotalForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeReplaceValuesForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeReplaceValuesForm", formName: "replaceValuesForm", scriptName: "InitializeReplaceValuesForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeRichTextEditorForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeRichTextEditorForm", formName: "richTextEditorForm", scriptName: "InitializeRichTextEditorForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditListBoxElementForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditListBoxElementForm", formName: "editListBoxElementForm", scriptName: "InitializeEditListBoxElementForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditComboBoxElementForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditComboBoxElementForm", formName: "editComboBoxElementForm", scriptName: "InitializeEditComboBoxElementForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditTreeViewElementForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditTreeViewElementForm", formName: "editTreeViewElementForm", scriptName: "InitializeEditTreeViewElementForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditTreeViewBoxElementForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditTreeViewBoxElementForm", formName: "editTreeViewBoxElementForm", scriptName: "InitializeEditTreeViewBoxElementForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditDatePickerElementForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditDatePickerElementForm", formName: "editDatePickerElementForm", scriptName: "InitializeEditDatePickerElementForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEasyImageForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEasyImageForm", formName: "easyImageForm", scriptName: "InitializeEasyImageForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializePreviewSettingsForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializePreviewSettingsForm", formName: "previewSettingsForm", scriptName: "InitializePreviewSettingsForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeElementDataFiltersForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeElementDataFiltersForm", formName: "elementDataFiltersForm", scriptName: "InitializeElementDataFiltersForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeTopNForm = function (isNotModal, callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeTopNForm", formName: "topNForm", scriptName: "InitializeTopNForm" }, callbackFunction, true, isNotModal);
}

StiMobileDesigner.prototype.InitializeDashboardInteractionForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeDashboardInteractionForm", formName: "dashboardInteractionForm", scriptName: "InitializeDashboardInteractionForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeHyperlinkEditorForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeHyperlinkEditorForm", formName: "hyperlinkEditorForm", scriptName: "InitializeHyperlinkEditorForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditElementDataTransformationForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditElementDataTransformationForm", formName: "editElementDataTransformationForm", scriptName: "InitializeEditElementDataTransformationForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeRenamePageForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeRenamePageForm", formName: "renamePageForm", scriptName: "InitializeRenamePageForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeChartConditionsForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeChartConditionsForm", formName: "chartConditionsForm", scriptName: "InitializeChartConditionsForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializePivotTableConditionsForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializePivotTableConditionsForm", formName: "pivotTableConditionsForm", scriptName: "InitializePivotTableConditionsForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeIndicatorConditionsForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeIndicatorConditionsForm", formName: "indicatorConditionsForm", scriptName: "InitializeIndicatorConditionsForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeProgressConditionsForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeProgressConditionsForm", formName: "progressConditionsForm", scriptName: "InitializeProgressConditionsForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeTableConditionsForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeTableConditionsForm", formName: "tableConditionsForm", scriptName: "InitializeTableConditionsForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeMobileViewComponentsForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeMobileViewComponentsForm", formName: "mobileViewComponentsForm", scriptName: "InitializeMobileViewComponentsForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeOfflineStoreItemsForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeOfflineStoreItemsForm", formName: "offlineStoreItemsForm", scriptName: "InitializeOfflineStoreItemsForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeGetDataForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeGetDataForm", formName: "getDataFormm", scriptName: "InitializeGetDataForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeDataWorldAuthForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeDataWorldAuthForm", formName: "dataWorldAuthForm", scriptName: "InitializeDataWorldAuthForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeInsertSymbolForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeInsertSymbolForm", formName: "insertSymbolForm", scriptName: "InitializeInsertSymbolForm" }, callbackFunction, true);
}

StiMobileDesigner.prototype.InitializeMapCategoriesForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeMapCategoriesForm", formName: "mapCategoriesForm", scriptName: "InitializeMapCategoriesForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeTrendLinesForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeTrendLinesForm", formName: "trendLinesForm", scriptName: "InitializeTrendLinesForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeNotificationForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeNotificationForm", formName: "notificationForm", scriptName: "InitializeNotificationForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeNotificationCheckActivatedForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeNotificationCheckActivatedForm", formName: "notificationCheckActivatedForm", scriptName: "InitializeNotificationForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeNotificationCheckTrDaysForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeNotificationCheckTrDaysForm", formName: "notificationCheckTrDaysForm", scriptName: "InitializeNotificationForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeWhoAreYouForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeWhoAreYouForm", formName: "whoAreYouForm", scriptName: "InitializeWhoAreYouForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditChartSimpleForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditChartSimpleForm", formName: "editChartSimpleForm", scriptName: "InitializeEditChartSimpleForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditChartSeriesForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditChartSeriesForm", formName: "editChartSeriesForm", scriptName: "InitializeEditChartSeriesForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditSparklineForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditSparklineForm", formName: "editSparkline", scriptName: "InitializeEditSparklineForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeViewQueryForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeViewQueryForm", formName: "viewQueryForm", scriptName: "InitializeViewQueryForm" }, callbackFunction, true);
}

StiMobileDesigner.prototype.InitializeEditChartStripsForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditChartStripsForm", formName: "editChartStripsForm", scriptName: "InitializeEditChartStripsForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditChartConstantLinesForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditChartConstantLinesForm", formName: "editChartConstantLinesForm", scriptName: "InitializeEditChartConstantLinesForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeChartSeriesFilterForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeChartSeriesFilterForm", formName: "chartSeriesFilterForm", scriptName: "InitializeChartSeriesFilterForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeChartSeriesConditionsForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeChartSeriesConditionsForm", formName: "chartSeriesConditionsForm", scriptName: "InitializeChartSeriesConditionsForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeChartSeriesTrendLinesForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeChartSeriesTrendLinesForm", formName: "chartSeriesTrendLinesForm", scriptName: "InitializeChartSeriesTrendLinesForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeTableOfContentsForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeTableOfContentsForm", formName: "tableOfContentsForm", scriptName: "InitializeTableOfContentsForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditMathFormulaForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditMathFormulaForm", formName: "editMathFormula", scriptName: "InitializeEditMathFormulaForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeBlocklyEditorForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeBlocklyEditorForm", formName: "blocklyEditor", scriptName: "InitializeBlocklyEditorForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEventEditorForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEventEditorForm", formName: "eventEditor", scriptName: "InitializeExpressionEditorForm;InitializeEventEditorForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeDashboardWatermarkForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeDashboardWatermarkForm", formName: "dashboardWatermark", scriptName: "InitializeDashboardWatermarkForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeShadowForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeShadowForm", formName: "shadowForm", scriptName: "InitializeShadowForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeEditElectronicSignatureForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeEditElectronicSignatureForm", formName: "editElectronicSignature", scriptName: "InitializeEditElectronicSignatureForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeSignatureImageForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeSignatureImageForm", formName: "signatureImage", scriptName: "InitializeSignatureImageForm" }, callbackFunction);
}

StiMobileDesigner.prototype.InitializeSignatureTextForm = function (callbackFunction) {
    this.ExecuteScript({ initMethod: "InitializeSignatureTextForm", formName: "signatureText", scriptName: "InitializeSignatureTextForm" }, callbackFunction);
}