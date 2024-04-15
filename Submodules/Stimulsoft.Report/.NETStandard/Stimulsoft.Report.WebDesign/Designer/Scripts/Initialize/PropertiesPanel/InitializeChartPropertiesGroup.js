
StiMobileDesigner.prototype.ChartPropertiesGroup = function () {
    var group = this.PropertiesGroup("chartPropertiesGroup", this.loc.PropertyCategory.ChartCategory);
    group.style.display = "none";
    group.innerGroups = {};

    var groupNames = [
        ["area", this.loc.PropertyCategory.AreaCategory, "main", 1],
        ["areaGridLinesHor", this.loc.ChartRibbon.GridLinesHorizontal, "area", 2],
        ["areaGridLinesVert", this.loc.ChartRibbon.GridLinesVertical, "area", 2],
        ["areaInterlacingHor", this.loc.PropertyMain.InterlacingHor, "area", 2],
        ["areaInterlacingVert", this.loc.PropertyMain.InterlacingVert, "area", 2],
        ["labels", this.loc.Chart.Labels, "main", 1],
        ["legend", this.loc.PropertyCategory.LegendCategory, "main", 1],
        ["legendLabels", this.loc.Chart.Labels, "legend", 2],
        ["legendTitle", this.loc.PropertyCategory.TitleCategory, "legend", 2],
        ["marker", this.loc.PropertyCategory.MarkerCategory, "main", 1],
        ["xAxis", this.loc.PropertyMain.XAxis, "area", 2],
        ["xAxisLabels", this.loc.Chart.Labels, "xAxis", 3],
        ["xAxisRange", this.loc.Chart.Range, "xAxis", 3],
        ["xAxisTitle", this.loc.PropertyCategory.TitleCategory, "xAxis", 3],
        ["xTopAxis", this.loc.PropertyMain.XTopAxis, "area", 2],
        ["xTopAxisLabels", this.loc.Chart.Labels, "xTopAxis", 3],
        ["xTopAxisTitle", this.loc.PropertyCategory.TitleCategory, "xTopAxis", 3],
        ["yAxis", this.loc.PropertyMain.YAxis, "area", 2],
        ["yAxisLabels", this.loc.Chart.Labels, "yAxis", 3],
        ["yAxisRange", this.loc.Chart.Range, "yAxis", 3],
        ["yAxisTitle", this.loc.PropertyCategory.TitleCategory, "yAxis", 3],
        ["yRightAxis", this.loc.PropertyMain.YRightAxis, "area", 2],
        ["yRightAxisLabels", this.loc.Chart.Labels, "yRightAxis", 3],
        ["yRightAxisTitle", this.loc.PropertyCategory.TitleCategory, "yRightAxis", 3],
        ["options3D", this.loc.PropertyMain.Options3D, "main", 1],
        ["title", this.loc.PropertyCategory.TitleCategory, "main", 1],
        ["table", this.loc.PropertyCategory.TableCategory, "main", 1],
        ["tableDataCells", this.loc.PropertyCategory.DataCells, "table", 2],
        ["tableHeader", this.loc.Components.StiHeaderBand, "table", 2]
    ]

    for (var i = 0; i < groupNames.length; i++) {
        var innerGroup = this.PropertiesGroup(null, groupNames[i][1], null, groupNames[i][3]);
        group.innerGroups[groupNames[i][0]] = innerGroup;
        innerGroup.parentGroup = groupNames[i][2] == "main" ? group : group.innerGroups[groupNames[i][2]];
    }

    var props = [
        ["crossFiltering", this.loc.PropertyMain.CrossFiltering, this.CheckBox("chartElementCrossFiltering"), "main", "crossFilteringChart"],
        ["group", this.loc.PropertyMain.Group, this.PropertyTextBox("chartElementGroup", this.options.propertyControlWidth), "main", "groupChart"],
        ["chartConstantLines", this.loc.PropertyMain.ConstantLines, this.PropertyConstantLinesControl("chartElementConstantLines", this.options.propertyControlWidth), "main"],
        ["chartSeries", this.loc.PropertyMain.Series, this.PropertySeriesControl("chartElementSeries", this.options.propertyControlWidth), "main"],
        ["chartStrips", this.loc.PropertyMain.Strips, this.PropertyStripsControl("chartElementStrips", this.options.propertyControlWidth), "main"],
        ["dataTransformation", this.loc.PropertyMain.DataTransformation, this.PropertyDataTransformationControl("chartElementDataTransformation", this.options.propertyControlWidth), "main", "dataTransformationChart"],
        ["chartTrendLines", this.loc.PropertyMain.TrendLines, this.PropertyTrendLinesControl("chartElementTrendLines", this.options.propertyControlWidth), "main"],

        ["areaAllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("chartElementAreaAllowApplyStyle"), "area"],
        ["areaBrush", this.loc.PropertyMain.Brush, this.PropertyBrushControl("chartElementAreaBrush", null, this.options.propertyControlWidth), "area"],
        ["areaBorderColor", this.loc.PropertyMain.BorderColor, this.PropertyColorControl("chartElementAreaBorderColor", null, this.options.propertyControlWidth), "area"],
        ["areaBorderThickness", this.loc.PropertyMain.BorderThickness, this.PropertyTextBox("chartElementAreaBorderThickness", this.options.propertyNumbersControlWidth), "area"],
        ["areaColorEach", this.loc.PropertyMain.ColorEach, this.CheckBox("chartElementAreaColorEach"), "area"],
        ["areaReverseHor", this.loc.PropertyMain.ReverseHor, this.CheckBox("chartElementAreaReverseHor"), "area"],
        ["areaReverseVert", this.loc.PropertyMain.ReverseVert, this.CheckBox("chartElementAreaReverseVert"), "area"],
        ["areaSideBySide", this.loc.PropertyMain.SideBySide, this.CheckBox("chartElementAreaSideBySide"), "area"],

        ["areaGridLinesHorAllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("chartElementAreaGridLinesHorAllowApplyStyle"), "areaGridLinesHor"],
        ["areaGridLinesHorColor", this.loc.PropertyMain.Color, this.PropertyColorControl("chartElementAreaGridLinesHorColor", null, this.options.propertyControlWidth), "areaGridLinesHor"],
        ["areaGridLinesHorMinorColor", this.loc.PropertyMain.MinorColor, this.PropertyColorControl("chartElementAreaGridLinesHorMinorColor", null, this.options.propertyControlWidth), "areaGridLinesHor"],
        ["areaGridLinesHorMinorCount", this.loc.PropertyMain.MinorCount, this.PropertyTextBox("chartElementAreaGridLinesHorMinorCount", this.options.propertyNumbersControlWidth), "areaGridLinesHor"],
        ["areaGridLinesHorMinorStyle", this.loc.PropertyMain.MinorStyle, this.PropertyDropDownList("chartElementAreaGridLinesHorMinorStyle", this.options.propertyControlWidth, this.GetBorderStyleItems(), true, true), "areaGridLinesHor"],
        ["areaGridLinesHorMinorVisible", this.loc.PropertyMain.MinorVisible, this.CheckBox("chartElementAreaGridLinesHorMinorVisible"), "areaGridLinesHor"],
        ["areaGridLinesHorStyle", this.loc.PropertyMain.Style, this.PropertyDropDownList("chartElementAreaGridLinesHorStyle", this.options.propertyControlWidth, this.GetBorderStyleItems(), true, true), "areaGridLinesHor"],
        ["areaGridLinesHorVisible", this.loc.PropertyMain.Visible, this.CheckBox("chartElementAreaGridLinesHorVisible"), "areaGridLinesHor"],

        ["areaGridLinesVertAllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("chartElementAreaGridLinesVertAllowApplyStyle"), "areaGridLinesVert"],
        ["areaGridLinesVertColor", this.loc.PropertyMain.Color, this.PropertyColorControl("chartElementAreaGridLinesVertColor", null, this.options.propertyControlWidth), "areaGridLinesVert"],
        ["areaGridLinesVertMinorColor", this.loc.PropertyMain.MinorColor, this.PropertyColorControl("chartElementAreaGridLinesVertMinorColor", null, this.options.propertyControlWidth), "areaGridLinesVert"],
        ["areaGridLinesVertMinorCount", this.loc.PropertyMain.MinorCount, this.PropertyTextBox("chartElementAreaGridLinesVertMinorCount", this.options.propertyNumbersControlWidth), "areaGridLinesVert"],
        ["areaGridLinesVertMinorStyle", this.loc.PropertyMain.MinorStyle, this.PropertyDropDownList("chartElementAreaGridLinesVertMinorStyle", this.options.propertyControlWidth, this.GetBorderStyleItems(), true, true), "areaGridLinesVert"],
        ["areaGridLinesVertMinorVisible", this.loc.PropertyMain.MinorVisible, this.CheckBox("chartElementAreaGridLinesVertMinorVisible"), "areaGridLinesVert"],
        ["areaGridLinesVertStyle", this.loc.PropertyMain.Style, this.PropertyDropDownList("chartElementAreaGridLinesVertStyle", this.options.propertyControlWidth, this.GetBorderStyleItems(), true, true), "areaGridLinesVert"],
        ["areaGridLinesVertVisible", this.loc.PropertyMain.Visible, this.CheckBox("chartElementAreaGridLinesVertVisible"), "areaGridLinesVert"],

        ["areaInterlacingHorAllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("chartElementAreaInterlacingHorAllowApplyStyle"), "areaInterlacingHor"],
        ["areaInterlacingHorColor", this.loc.PropertyMain.Color, this.PropertyColorControl("chartElementAreaInterlacingHorColor", null, this.options.propertyControlWidth), "areaInterlacingHor"],
        ["areaInterlacingHorVisible", this.loc.PropertyMain.Visible, this.CheckBox("chartElementAreaInterlacingHorVisible"), "areaInterlacingHor"],
        ["areaInterlacingHorInterlacedBrush", this.loc.PropertyMain.InterlacedBrush, this.PropertyBrushControl("chartElementAreaInterlacingHorInterlacedBrush", null, this.options.propertyControlWidth), "areaInterlacingHor"],

        ["areaInterlacingVertAllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("chartElementAreaInterlacingVertAllowApplyStyle"), "areaInterlacingVert"],
        ["areaInterlacingVertColor", this.loc.PropertyMain.Color, this.PropertyColorControl("chartElementAreaInterlacingVertColor", null, this.options.propertyControlWidth), "areaInterlacingVert"],
        ["areaInterlacingVertVisible", this.loc.PropertyMain.Visible, this.CheckBox("chartElementAreaInterlacingVertVisible"), "areaInterlacingVert"],
        ["areaInterlacingVertInterlacedBrush", this.loc.PropertyMain.InterlacedBrush, this.PropertyBrushControl("chartElementAreaInterlacingVertInterlacedBrush", null, this.options.propertyControlWidth), "areaInterlacingVert"],

        ["labelsLabelsType", this.loc.PropertyMain.Type, this.PropertyChartSeriesLabelsControl("chartElementLabelsLabelsType", this.options.propertyControlWidth), "labels"],
        ["labelsAllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("chartElementLabelsAllowApplyStyle"), "labels"],
        ["labelsAngle", this.loc.PropertyMain.Angle, this.PropertyTextBox("chartElementLabelsAngle", this.options.propertyNumbersControlWidth), "labels"],
        ["labelsAntialiasing", this.loc.PropertyMain.Antialiasing, this.CheckBox("chartElementLabelsAntialiasing"), "labels"],
        ["labelsAutoRotate", this.loc.PropertyMain.AutoRotate, this.CheckBox("chartElementLabelsAutoRotate"), "labels"],
        ["labelsBorderColor", this.loc.PropertyMain.BorderColor, this.PropertyColorControl("chartElementLabelsBorderColor", null, this.options.propertyControlWidth), "labels"],
        ["labelsBrush", this.loc.PropertyMain.Brush, this.PropertyBrushControl("chartElementLabelsBrush", null, this.options.propertyControlWidth), "labels"],
        ["labelsDrawBorder", this.loc.PropertyMain.DrawBorder, this.CheckBox("chartElementLabelsDrawBorder"), "labels"],
        ["labelsFont", this.loc.PropertyMain.Font, this.PropertyFontControl("chartElementLabelsFont", null, true), "labels"],
        ["labelsFormat", this.loc.PropertyMain.Format, this.PropertyDropDownList("chartElementLabelsFormat", this.options.propertyControlWidth, this.GetTableHeaderFormatItems(), false), "labels"],
        ["labelsForeColor", this.loc.PropertyMain.ForeColor, this.PropertyColorControl("chartElementLabelsForeColor", null, this.options.propertyControlWidth), "labels"],
        ["labelsLabelColor", this.loc.PropertyMain.LabelColor, this.PropertyColorControl("chartElementLabelsLabelColor", null, this.options.propertyControlWidth), "labels"],
        ["labelsLineColor", this.loc.PropertyMain.LineColor, this.PropertyColorControl("chartElementLabelsLineColor", null, this.options.propertyControlWidth), "labels"],
        ["labelsLegendValueType", this.loc.PropertyMain.LegendValueType, this.PropertyDropDownList("chartElementLabelsLegendValueType", this.options.propertyControlWidth, this.GetLegendValueTypeItems(), true), "labels"],
        ["labelsLineLength", this.loc.PropertyMain.Length, this.PropertyTextBox("chartElementLabelsLineLength", this.options.propertyNumbersControlWidth), "labels"],
        ["labelsMarkerAlignment", this.loc.PropertyMain.MarkerAlignment, this.PropertyDropDownList("chartElementLabelsMarkerAlignment", this.options.propertyControlWidth, this.GetXAxisTextAlignmentItems(), true), "labels"],
        ["labelsMarkerSize", this.loc.PropertyMain.MarkerSize, this.PropertySizeControl("chartElementLabelsMarkerSize", this.options.propertyNumbersControlWidth + 40), "labels"],
        ["labelsMarkerVisible", this.loc.PropertyMain.MarkerVisible, this.CheckBox("chartElementLabelsMarkerVisible"), "labels"],
        ["labelsPreventIntersection", this.loc.PropertyMain.PreventIntersection, this.CheckBox("chartElementLabelsPreventIntersection"), "labels"],
        ["labelsShowInPercent", this.loc.PropertyMain.ShowInPercent, this.CheckBox("chartElementLabelsShowInPercent"), "labels"],
        ["labelsShowNulls", this.loc.PropertyMain.ShowNulls, this.CheckBox("chartElementLabelsShowNulls"), "labels"],
        ["labelsShowZeros", this.loc.PropertyMain.ShowZeros, this.CheckBox("chartElementLabelsShowZeros"), "labels"],
        ["labelsStep", this.loc.PropertyMain.Step, this.PropertyTextBox("chartElementLabelsStep", this.options.propertyNumbersControlWidth), "labels"],
        ["labelsTextAfter", this.loc.PropertyMain.TextAfter, this.PropertyTextBox("chartElementLabelsTextAfter", this.options.propertyControlWidth), "labels"],
        ["labelsTextBefore", this.loc.PropertyMain.TextBefore, this.PropertyTextBox("chartElementLabelsTextBefore", this.options.propertyControlWidth), "labels"],
        ["labelsUseSeriesColor", this.loc.PropertyMain.UseSeriesColor, this.CheckBox("chartElementLabelsUseSeriesColor"), "labels"],
        ["labelsValueType", this.loc.PropertyMain.ValueType, this.PropertyDropDownList("chartElementLabelsValueType", this.options.propertyControlWidth, this.GetLegendValueTypeItems(), true), "labels"],
        ["labelsValueTypeSeparator", this.loc.PropertyMain.ValueTypeSeparator, this.PropertyTextBox("chartElementLabelsValueTypeSeparator", this.options.propertyNumbersControlWidth), "labels"],
        ["labelsVisible", this.loc.PropertyMain.Visible, this.CheckBox("chartElementLabelsVisible"), "labels"],
        ["labelsWidth", this.loc.PropertyMain.Width, this.PropertyTextBox("chartElementLabelsWidth", this.options.propertyNumbersControlWidth), "labels"],
        ["labelsWordWrap", this.loc.PropertyMain.WordWrap, this.CheckBox("chartElementLabelsWordWrap"), "labels"],
        ["labelsPosition", this.loc.PropertyMain.Position, this.PropertyDropDownList("chartElementLabelsPosition", this.options.propertyControlWidth, this.GetLabelsPositionItems(), true), "labels"],
        ["labelsStyle", this.loc.PropertyMain.Style, this.PropertyDropDownList("chartElementLabelsStyle", this.options.propertyControlWidth, this.GetLabelsStyleItems(), true), "labels"],

        ["legendAllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("chartElementLegendAllowApplyStyle"), "legend"],
        ["legendBorderColor", this.loc.PropertyMain.BorderColor, this.PropertyColorControl("chartElementLegendBorderColor", null, this.options.propertyControlWidth), "legend"],
        ["legendBrush", this.loc.PropertyMain.Brush, this.PropertyBrushControl("chartElementLegendBrush", null, this.options.propertyControlWidth), "legend"],
        ["legendColumns", this.loc.PropertyMain.Columns, this.PropertyTextBox("chartElementLegendColumns", this.options.propertyControlWidth), "legend"],
        ["legendDirection", this.loc.PropertyMain.Direction, this.PropertyDropDownList("chartElementLegendDirection", this.options.propertyControlWidth, this.GetLegendDirectionItems(), true), "legend"],
        ["legendFont", this.loc.PropertyMain.Font, this.PropertyFontControl("chartElementLegendFont", null, true), "legend"],
        ["legendHorAlignment", this.loc.PropertyMain.HorAlignment, this.PropertyDropDownList("chartElementLegendHorAlignment", this.options.propertyControlWidth, this.GetLegendHorAlignmentItems(), true), "legend"],
        ["legendVertAlignment", this.loc.PropertyMain.VertAlignment, this.PropertyDropDownList("chartElementLegendVertAlignment", this.options.propertyControlWidth, this.GetLegendVertAlignmentItems(), true), "legend"],
        ["legendVisibility", this.loc.PropertyMain.Visible, this.PropertyDropDownList("chartElementLegendVisibility", this.options.propertyControlWidth, this.GetLegendVisibleItems(), true), "legend"],
        ["legendLabelsColor", this.loc.PropertyMain.Color, this.PropertyColorControl("chartElementLegendLabelsColor", null, this.options.propertyControlWidth), "legendLabels"],
        ["legendLabelsFont", this.loc.PropertyMain.Font, this.PropertyFontControl("chartElementLegendLabelsFont", null, true), "legendLabels"],
        ["legendLabelsValueType", this.loc.PropertyMain.ValueType, this.PropertyDropDownList("chartElementLegendLabelsValueType", this.options.propertyControlWidth, this.GetLegendLabelsValueTypeItems(), true), "legendLabels"],
        ["legendTitleColor", this.loc.PropertyMain.Color, this.PropertyColorControl("chartElementLegendTitleColor", null, this.options.propertyControlWidth), "legendTitle"],
        ["legendTitleFont", this.loc.PropertyMain.Font, this.PropertyFontControl("chartElementLegendTitleFont", null, true), "legendTitle"],
        ["legendTitleText", this.loc.PropertyMain.Text, this.PropertyTextBox("chartElementLegendTitleText", this.options.propertyControlWidth), "legendTitle"],

        ["xAxisLabelsAngle", this.loc.PropertyMain.Angle, this.PropertyTextBox("chartElementXAxisLabelsAngle", this.options.propertyNumbersControlWidth), "xAxisLabels"],
        ["xAxisLabelsColor", this.loc.PropertyMain.Color, this.PropertyColorControl("chartElementXAxisLabelsColor", null, this.options.propertyControlWidth), "xAxisLabels"],
        ["xAxisLabelsFont", this.loc.PropertyMain.Font, this.PropertyFontControl("chartElementXAxisLabelsFont", null, true), "xAxisLabels"],
        ["xAxisLabelsPlacement", this.loc.PropertyMain.Placement, this.PropertyDropDownList("chartElementXAxisLabelsPlacement", this.options.propertyControlWidth, this.GetXAxisPlacementItems(), true), "xAxisLabels"],
        ["xAxisLabelsStep", this.loc.PropertyMain.Step, this.PropertyTextBox("chartElementXAxisLabelsStep", this.options.propertyControlWidth), "xAxisLabels"],
        ["xAxisLabelsTextAfter", this.loc.PropertyMain.TextAfter, this.PropertyTextBox("chartElementXAxisLabelsTextAfter", this.options.propertyControlWidth), "xAxisLabels"],
        ["xAxisLabelsTextAlignment", this.loc.PropertyMain.TextAlignment, this.PropertyDropDownList("chartElementXAxisLabelsTextAlignment", this.options.propertyControlWidth, this.GetXAxisTextAlignmentItems(), true), "xAxisLabels"],
        ["xAxisLabelsTextBefore", this.loc.PropertyMain.TextBefore, this.PropertyTextBox("chartElementXAxisLabelsTextBefore", this.options.propertyControlWidth), "xAxisLabels"],

        ["xAxisRangeAuto", this.loc.PropertyMain.Auto, this.CheckBox("chartElementXAxisRangeAuto"), "xAxisRange"],
        ["xAxisRangeMinimum", this.loc.PropertyMain.Minimum, this.PropertyTextBox("chartElementXAxisRangeMinimum", this.options.propertyNumbersControlWidth), "xAxisRange"],
        ["xAxisRangeMaximum", this.loc.PropertyMain.Maximum, this.PropertyTextBox("chartElementXAxisRangeMaximum", this.options.propertyNumbersControlWidth), "xAxisRange"],

        ["xAxisTitleAlignment", this.loc.PropertyMain.Alignment, this.PropertyDropDownList("chartElementXAxisTitleAlignment", this.options.propertyControlWidth, this.GetXAxisTitleAlignmentItems(), true), "xAxisTitle"],
        ["xAxisTitleColor", this.loc.PropertyMain.Color, this.PropertyColorControl("chartElementXAxisTitleColor", null, this.options.propertyControlWidth), "xAxisTitle"],
        ["xAxisTitleDirection", this.loc.PropertyMain.Direction, this.PropertyDropDownList("chartElementXAxisTitleDirection", this.options.propertyControlWidth, this.GetXAxisTitleDirectionItems(), true), "xAxisTitle"],
        ["xAxisTitleFont", this.loc.PropertyMain.Font, this.PropertyFontControl("chartElementXAxisTitleFont", null, true), "xAxisTitle"],
        ["xAxisTitlePosition", this.loc.PropertyMain.Position, this.PropertyDropDownList("chartElementXAxisTitlePosition", this.options.propertyControlWidth, this.GetXAxisTitlePositionItems(), true), "xAxisTitle"],
        ["xAxisTitleText", this.loc.PropertyMain.Text, this.PropertyTextBox("chartElementXAxisTitleText", this.options.propertyControlWidth), "xAxisTitle"],
        ["xAxisTitleVisible", this.loc.PropertyMain.Visible, this.CheckBox("chartElementXAxisTitleVisible"), "xAxisTitle"],

        ["xAxisShowEdgeValues", this.loc.PropertyMain.ShowEdgeValues, this.PropertyDropDownList("chartElementXAxisShowEdgeValues", this.options.propertyControlWidth, this.GetShowEdgeValuesItems(), true), "xAxis"],
        ["xAxisStartFromZero", this.loc.PropertyMain.StartFromZero, this.PropertyDropDownList("chartElementXAxisStartFromZero", this.options.propertyControlWidth, this.GetShowEdgeValuesItems(), true), "xAxis"],
        ["xAxisVisible", this.loc.PropertyMain.Visible, this.CheckBox("chartElementXAxisVisible"), "xAxis"],

        ["xTopAxisLabelsAngle", this.loc.PropertyMain.Angle, this.PropertyTextBox("chartElementXTopAxisLabelsAngle", this.options.propertyNumbersControlWidth), "xTopAxisLabels"],
        ["xTopAxisLabelsColor", this.loc.PropertyMain.Color, this.PropertyColorControl("chartElementXTopAxisLabelsColor", null, this.options.propertyControlWidth), "xTopAxisLabels"],
        ["xTopAxisLabelsFont", this.loc.PropertyMain.Font, this.PropertyFontControl("chartElementXTopAxisLabelsFont", null, true), "xTopAxisLabels"],
        ["xTopAxisLabelsPlacement", this.loc.PropertyMain.Placement, this.PropertyDropDownList("chartElementXTopAxisLabelsPlacement", this.options.propertyControlWidth, this.GetXAxisPlacementItems(), true), "xTopAxisLabels"],
        ["xTopAxisLabelsStep", this.loc.PropertyMain.Step, this.PropertyTextBox("chartElementXTopAxisLabelsStep", this.options.propertyControlWidth), "xTopAxisLabels"],
        ["xTopAxisLabelsTextAfter", this.loc.PropertyMain.TextAfter, this.PropertyTextBox("chartElementXTopAxisLabelsTextAfter", this.options.propertyControlWidth), "xTopAxisLabels"],
        ["xTopAxisLabelsTextAlignment", this.loc.PropertyMain.TextAlignment, this.PropertyDropDownList("chartElementXTopAxisLabelsTextAlignment", this.options.propertyControlWidth, this.GetXAxisTextAlignmentItems(), true), "xTopAxisLabels"],
        ["xTopAxisLabelsTextBefore", this.loc.PropertyMain.TextBefore, this.PropertyTextBox("chartElementXTopAxisLabelsTextBefore", this.options.propertyControlWidth), "xTopAxisLabels"],

        ["xTopAxisTitleAlignment", this.loc.PropertyMain.Alignment, this.PropertyDropDownList("chartElementXTopAxisTitleAlignment", this.options.propertyControlWidth, this.GetXAxisTitleAlignmentItems(), true), "xTopAxisTitle"],
        ["xTopAxisTitleColor", this.loc.PropertyMain.Color, this.PropertyColorControl("chartElementXTopAxisTitleColor", null, this.options.propertyControlWidth), "xTopAxisTitle"],
        ["xTopAxisTitleDirection", this.loc.PropertyMain.Direction, this.PropertyDropDownList("chartElementXTopAxisTitleDirection", this.options.propertyControlWidth, this.GetXAxisTitleDirectionItems(), true), "xTopAxisTitle"],
        ["xTopAxisTitleFont", this.loc.PropertyMain.Font, this.PropertyFontControl("chartElementXTopAxisTitleFont", null, true), "xTopAxisTitle"],
        ["xTopAxisTitlePosition", this.loc.PropertyMain.Position, this.PropertyDropDownList("chartElementXTopAxisTitlePosition", this.options.propertyControlWidth, this.GetXAxisTitlePositionItems(), true), "xTopAxisTitle"],
        ["xTopAxisTitleText", this.loc.PropertyMain.Text, this.PropertyTextBox("chartElementXTopAxisTitleText", this.options.propertyControlWidth), "xTopAxisTitle"],
        ["xTopAxisTitleVisible", this.loc.PropertyMain.Visible, this.CheckBox("chartElementXTopAxisTitleVisible"), "xTopAxisTitle"],

        ["xTopAxisShowEdgeValues", this.loc.PropertyMain.ShowEdgeValues, this.PropertyDropDownList("chartElementXTopAxisShowEdgeValues", this.options.propertyControlWidth, this.GetShowEdgeValuesItems(), true), "xTopAxis"],
        ["xTopAxisVisible", this.loc.PropertyMain.Visible, this.CheckBox("chartElementXTopAxisVisible"), "xTopAxis"],

        ["yAxisLabelsAngle", this.loc.PropertyMain.Angle, this.PropertyTextBox("chartElementYAxisLabelsAngle", this.options.propertyNumbersControlWidth), "yAxisLabels"],
        ["yAxisLabelsColor", this.loc.PropertyMain.Color, this.PropertyColorControl("chartElementYAxisLabelsColor", null, this.options.propertyControlWidth), "yAxisLabels"],
        ["yAxisLabelsFont", this.loc.PropertyMain.Font, this.PropertyFontControl("chartElementYAxisLabelsFont", null, true), "yAxisLabels"],
        ["yAxisLabelsPlacement", this.loc.PropertyMain.Placement, this.PropertyDropDownList("chartElementYAxisLabelsPlacement", this.options.propertyControlWidth, this.GetXAxisPlacementItems(), true), "yAxisLabels"],
        ["yAxisLabelsStep", this.loc.PropertyMain.Step, this.PropertyTextBox("chartElementYAxisLabelsStep", this.options.propertyControlWidth), "yAxisLabels"],
        ["yAxisLabelsTextAfter", this.loc.PropertyMain.TextAfter, this.PropertyTextBox("chartElementYAxisLabelsTextAfter", this.options.propertyControlWidth), "yAxisLabels"],
        ["yAxisLabelsTextAlignment", this.loc.PropertyMain.TextAlignment, this.PropertyDropDownList("chartElementYAxisLabelsTextAlignment", this.options.propertyControlWidth, this.GetXAxisTextAlignmentItems(), true), "yAxisLabels"],
        ["yAxisLabelsTextBefore", this.loc.PropertyMain.TextBefore, this.PropertyTextBox("chartElementYAxisLabelsTextBefore", this.options.propertyControlWidth), "yAxisLabels"],

        ["yAxisRangeAuto", this.loc.PropertyMain.Auto, this.CheckBox("chartElementYAxisRangeAuto"), "yAxisRange"],
        ["yAxisRangeMinimum", this.loc.PropertyMain.Minimum, this.PropertyTextBox("chartElementYAxisRangeMinimum", this.options.propertyNumbersControlWidth), "yAxisRange"],
        ["yAxisRangeMaximum", this.loc.PropertyMain.Maximum, this.PropertyTextBox("chartElementYAxisRangeMaximum", this.options.propertyNumbersControlWidth), "yAxisRange"],

        ["yAxisTitleAlignment", this.loc.PropertyMain.Alignment, this.PropertyDropDownList("chartElementYAxisTitleAlignment", this.options.propertyControlWidth, this.GetXAxisTitleAlignmentItems(), true), "yAxisTitle"],
        ["yAxisTitleColor", this.loc.PropertyMain.Color, this.PropertyColorControl("chartElementYAxisTitleColor", null, this.options.propertyControlWidth), "yAxisTitle"],
        ["yAxisTitleDirection", this.loc.PropertyMain.Direction, this.PropertyDropDownList("chartElementYAxisTitleDirection", this.options.propertyControlWidth, this.GetXAxisTitleDirectionItems(), true), "yAxisTitle"],
        ["yAxisTitleFont", this.loc.PropertyMain.Font, this.PropertyFontControl("chartElementYAxisTitleFont", null, true), "yAxisTitle"],
        ["yAxisTitlePosition", this.loc.PropertyMain.Position, this.PropertyDropDownList("chartElementYAxisTitlePosition", this.options.propertyControlWidth, this.GetXAxisTitlePositionItems(), true), "yAxisTitle"],
        ["yAxisTitleText", this.loc.PropertyMain.Text, this.PropertyTextBox("chartElementYAxisTitleText", this.options.propertyControlWidth), "yAxisTitle"],
        ["yAxisTitleVisible", this.loc.PropertyMain.Visible, this.CheckBox("chartElementYAxisTitleVisible"), "yAxisTitle"],

        ["yAxisStartFromZero", this.loc.PropertyMain.StartFromZero, this.CheckBox("chartElementYAxisStartFromZero"), "yAxis"],
        ["yAxisVisible", this.loc.PropertyMain.Visible, this.CheckBox("chartElementYAxisVisible"), "yAxis"],

        ["yRightAxisLabelsAngle", this.loc.PropertyMain.Angle, this.PropertyTextBox("chartElementYRightAxisLabelsAngle", this.options.propertyNumbersControlWidth), "yRightAxisLabels"],
        ["yRightAxisLabelsColor", this.loc.PropertyMain.Color, this.PropertyColorControl("chartElementYRightAxisLabelsColor", null, this.options.propertyControlWidth), "yRightAxisLabels"],
        ["yRightAxisLabelsFont", this.loc.PropertyMain.Font, this.PropertyFontControl("chartElementYRightAxisLabelsFont", null, true), "yRightAxisLabels"],
        ["yRightAxisLabelsPlacement", this.loc.PropertyMain.Placement, this.PropertyDropDownList("chartElementYRightAxisLabelsPlacement", this.options.propertyControlWidth, this.GetXAxisPlacementItems(), true), "yRightAxisLabels"],
        ["yRightAxisLabelsTextAfter", this.loc.PropertyMain.TextAfter, this.PropertyTextBox("chartElementYRightAxisLabelsTextAfter", this.options.propertyControlWidth), "yRightAxisLabels"],
        ["yRightAxisLabelsTextAlignment", this.loc.PropertyMain.TextAlignment, this.PropertyDropDownList("chartElementYRightAxisLabelsTextAlignment", this.options.propertyControlWidth, this.GetXAxisTextAlignmentItems(), true), "yRightAxisLabels"],
        ["yRightAxisLabelsTextBefore", this.loc.PropertyMain.TextBefore, this.PropertyTextBox("chartElementYRightAxisLabelsTextBefore", this.options.propertyControlWidth), "yRightAxisLabels"],

        ["yRightAxisTitleAlignment", this.loc.PropertyMain.Alignment, this.PropertyDropDownList("chartElementYRightAxisTitleAlignment", this.options.propertyControlWidth, this.GetXAxisTitleAlignmentItems(), true), "yRightAxisTitle"],
        ["yRightAxisTitleColor", this.loc.PropertyMain.Color, this.PropertyColorControl("chartElementYRightAxisTitleColor", null, this.options.propertyControlWidth), "yRightAxisTitle"],
        ["yRightAxisTitleDirection", this.loc.PropertyMain.Direction, this.PropertyDropDownList("chartElementYRightAxisTitleDirection", this.options.propertyControlWidth, this.GetXAxisTitleDirectionItems(), true), "yRightAxisTitle"],
        ["yRightAxisTitleFont", this.loc.PropertyMain.Font, this.PropertyFontControl("chartElementYRightAxisTitleFont", null, true), "yRightAxisTitle"],
        ["yRightAxisTitlePosition", this.loc.PropertyMain.Position, this.PropertyDropDownList("chartElementYRightAxisTitlePosition", this.options.propertyControlWidth, this.GetXAxisTitlePositionItems(), true), "yRightAxisTitle"],
        ["yRightAxisTitleText", this.loc.PropertyMain.Text, this.PropertyTextBox("chartElementYRightAxisTitleText", this.options.propertyControlWidth), "yRightAxisTitle"],
        ["yRightAxisTitleVisible", this.loc.PropertyMain.Visible, this.CheckBox("chartElementYRightAxisTitleVisible"), "yRightAxisTitle"],

        ["yRightAxisStartFromZero", this.loc.PropertyMain.StartFromZero, this.CheckBox("chartElementYRightAxisStartFromZero"), "yRightAxis"],
        ["yRightAxisVisible", this.loc.PropertyMain.Visible, this.CheckBox("chartElementYRightAxisVisible"), "yRightAxis"],

        ["markerAngle", this.loc.PropertyMain.Angle, this.PropertyTextBox("chartElementMarkerAngle", this.options.propertyNumbersControlWidth), "marker"],
        ["markerSize", this.loc.PropertyMain.Size, this.PropertyTextBox("chartElementMarkerSize", this.options.propertyNumbersControlWidth), "marker"],
        ["markerType", this.loc.PropertyMain.Type, this.PropertyDropDownList("chartElementMarkerType", this.options.propertyControlWidth, this.GetMarkerTypeItems(), true), "marker"],
        ["markerVisible", this.loc.PropertyMain.Visible, this.PropertyDropDownList("chartElementMarkerVisible", this.options.propertyControlWidth, this.GetMarkerVisibleItems(), true), "marker"],

        ["chartTitleAllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("chartElementTitleAllowApplyStyle"), "title"],
        ["chartTitleAlignment", this.loc.PropertyMain.Alignment, this.PropertyDropDownList("chartElementTitleAlignment", this.options.propertyControlWidth, this.GetXAxisTitleAlignmentItems(), true), "title"],
        ["chartTitleAntialiasing", this.loc.PropertyMain.Antialiasing, this.CheckBox("chartElementTitleAntialiasing"), "title"],
        ["chartTitleBrush", this.loc.PropertyMain.Brush, this.PropertyBrushControl("chartElementTitleBrush", null, this.options.propertyControlWidth), "title"],
        ["chartTitleDock", this.loc.PropertyMain.Dock, this.PropertyDropDownList("chartElementTitleDock", this.options.propertyControlWidth, this.GetDockItems(), true), "title"],
        ["chartTitleFont", this.loc.PropertyMain.Font, this.PropertyFontControl("chartElementTitleFont", null, true), "title"],
        ["chartTitleSpacing", this.loc.PropertyMain.Spacing, this.PropertyTextBox("chartElementTitleSpacing", this.options.propertyNumbersControlWidth), "title"],
        ["chartTitleText", this.loc.PropertyMain.Text, this.PropertyTextBox("chartElementTitleText", this.options.propertyControlWidth), "title"],
        ["chartTitleVisible", this.loc.PropertyMain.Visible, this.CheckBox("chartElementTitleVisible"), "title"],

        ["chartTableAllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("chartElementTableAllowApplyStyle"), "table"],
        ["chartTableGridLineColor", this.loc.PropertyMain.GridLineColor, this.PropertyColorControl("chartElementTableGridLineColor", null, this.options.propertyControlWidth), "table"],
        ["chartTableGridLinesHor", this.loc.PropertyMain.GridLinesHor, this.CheckBox("chartElementTableGridLinesHor"), "table"],
        ["chartTableGridLinesVert", this.loc.PropertyMain.GridLinesVert, this.CheckBox("chartElementTableGridLinesVert"), "table"],
        ["chartTableGridOutline", this.loc.PropertyMain.GridOutline, this.CheckBox("chartElementTableGridOutline"), "table"],
        ["chartTableMarkerVisible", this.loc.PropertyMain.MarkerVisible, this.CheckBox("chartElementTableMarkerVisible"), "table"],
        ["chartTableVisible", this.loc.PropertyMain.Visible, this.CheckBox("chartElementTableVisible"), "table"],
        ["chartTableDataCellsTextColor", this.loc.PropertyMain.TextColor, this.PropertyColorControl("chartElementTableDataCellsTextColor", null, this.options.propertyControlWidth), "tableDataCells"],
        ["chartTableDataCellsShrinkFontToFit", this.loc.PropertyMain.ShrinkFontToFit, this.CheckBox("chartElementTableDataCellsShrinkFontToFit"), "tableDataCells"],
        ["chartTableDataCellsShrinkFontToFitMinimumSize", this.loc.PropertyMain.ShrinkFontToFitMinimumSize, this.PropertyTextBox("chartElementTableDataCellsShrinkFontToFitMinimumSize", this.options.propertyNumbersControlWidth), "tableDataCells"],
        ["chartTableDataCellsFont", this.loc.PropertyMain.Font, this.PropertyFontControl("chartElementTableDataCellsFont", null, true), "tableDataCells"],
        ["chartTableHeaderTextAfter", this.loc.PropertyMain.TextAfter, this.PropertyTextBox("chartElementTableHeaderTextAfter", this.options.propertyControlWidth), "tableHeader"],
        ["chartTableHeaderTextColor", this.loc.PropertyMain.TextColor, this.PropertyColorControl("chartElementTableHeaderTextColor", null, this.options.propertyControlWidth), "tableHeader"],
        ["chartTableHeaderWordWrap", this.loc.PropertyMain.WordWrap, this.CheckBox("chartElementTableHeaderWordWrap"), "tableHeader"],
        ["chartTableHeaderBrush", this.loc.PropertyMain.Brush, this.PropertyBrushControl("chartElementTableHeaderBrush", null, this.options.propertyControlWidth), "tableHeader"],
        ["chartTableHeaderFont", this.loc.PropertyMain.Font, this.PropertyFontControl("chartElementTableHeaderFont", null, true), "tableHeader"],

        ["options3DDistance", this.loc.PropertyMain.Distance, this.PropertyTextBox("chartElementOptions3DDistance", this.options.propertyNumbersControlWidth), "options3D"],
        ["options3DHeight", this.loc.PropertyMain.Height, this.PropertyTextBox("chartElementOptions3DHeight", this.options.propertyNumbersControlWidth), "options3D"],
        ["options3DLighting", this.loc.PropertyMain.Lighting, this.PropertyDropDownList("chartElementOptions3DLighting", this.options.propertyControlWidth, this.GetOptions3DLightingItems(), true), "options3D"],
        ["options3DOpacity", this.loc.PropertyMain.Opacity, this.PropertyTextBox("chartElementOptions3DOpacity", this.options.propertyNumbersControlWidth), "options3D"]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        var currentGroup = props[i][3] == "main" ? group : group.innerGroups[props[i][3]];
        currentGroup.container.appendChild(this.Property(props[i].length > 4 ? props[i][4] : props[i][0], props[i][1], control, null, currentGroup.nestingLevel));

        if (props[i][0] == "argumentFormat" || props[i][0] == "valueFormat") {
            control.button.propertyName = props[i][0];
            control.button.action = function () {
                var propertyName = this.propertyName;
                this.jsObject.InitializeTextFormatForm(function (textFormatForm) {
                    textFormatForm.show(null, propertyName);
                });
            }
        }
        else {
            control.action = function () {
                if (this.propertyName == "legendColumns") {
                    this.value = this.jsObject.StrToCorrectPositiveInt(this.value);
                }
                this.jsObject.ApplyPropertyValue([this.propertyName], [this.jsObject.GetControlValue(this)]);
            }
        }
    }

    for (var i = 0; i < groupNames.length; i++) {
        group.innerGroups[groupNames[i][0]].parentGroup.container.appendChild(group.innerGroups[groupNames[i][0]]);
    }

    return group;
}

StiMobileDesigner.prototype.ShowChartElementMarkerProperty = function (valueMeters) {
    if (valueMeters) {
        var chartMarkerSeries = ["Line", "StackedLine", "FullStackedLine", "Spline", "StackedSpline", "FullStackedSpline", "SteppedLine", "Area", "StackedArea",
            "FullStackedArea", "SplineArea", "StackedSplineArea", "FullStackedSplineArea", "SteppedArea", "Scatter", "ScatterSpline", "ScatterLine", "RadarLine",
            "RadarArea", "RadarPoint", "Range", "SplineRange", "SteppedRange"];

        for (var i = 0; i < valueMeters.length; i++) {
            if (valueMeters[i].seriesType && chartMarkerSeries.indexOf(valueMeters[i].seriesType) >= 0)
                return true;
        }
    }
    return false;
}