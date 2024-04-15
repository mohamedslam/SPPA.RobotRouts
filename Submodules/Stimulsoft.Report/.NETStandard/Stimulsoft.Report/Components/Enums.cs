#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft     							}
{	ALL RIGHTS RESERVED												}
{																	}
{	The entire contents of this file is protected by U.S. and		}
{	International Copyright Laws. Unauthorized reproduction,		}
{	reverse-engineering, and distribution of all or any portion of	}
{	the code contained in this file is strictly prohibited and may	}
{	result in severe civil and criminal penalties and will be		}
{	prosecuted to the maximum extent possible under the law.		}
{																	}
{	RESTRICTIONS													}
{																	}
{	THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES			}
{	ARE CONFIDENTIAL AND PROPRIETARY								}
{	TRADE SECRETS OF Stimulsoft										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System;

namespace Stimulsoft.Report.Components
{
    #region StiTextFormatState
    [Flags]
    public enum StiTextFormatState
    {
        None = 0,
        DecimalDigits = 1,
        DecimalSeparator = 2,
        GroupSeparator = 4,
        GroupSize = 8,
        PositivePattern = 16,
        NegativePattern = 32,                
        CurrencySymbol = 64,
        PercentageSymbol = 128,
        Abbreviation = 256,
        NegativeInRed = 512
    }
    #endregion

    #region StiIconSet
    /// <summary>
    /// List of icons for Icon Sets Indicator.
    /// </summary>
    public enum StiIconSet
    {
        None,
        Full,

        ArrowsColored3,
        ArrowsColored4,
        ArrowsColored5,

        ArrowsGray3,
        ArrowsGray4,
        ArrowsGray5,

        Flags3,
        Latin4,
        Quarters5,
        QuartersGreen5,
        QuartersRed5,
        
        Ratings3,
        Ratings4,
        Ratings5,

        RedToBlack4,  
        Signs3,
        Squares5,

        Stars3,
        Stars5,

        SymbolsCircled3,
        SymbolsUncircled3,       
        
        TrafficLights4,
        TrafficLightsRimmed3,
        TrafficLightsUnrimmed3,        

        Triangles3
    }
    #endregion

    #region StiIcon
    /// <summary>
    /// Icons for Icon Sets Indicator.
    /// </summary>
    public enum StiIcon
    {
        None,
        ArrowRightDownGray,
        ArrowRightUpGray,
        ArrowDownGray,
        ArrowRightGray,
        ArrowUpGray,
        ArrowUpGreen,
        ArrowDownRed,
        ArrowRightYellow,
        ArrowRightDownYellow,
        ArrowRightUpYellow,
        CheckGreen,
        CircleBlack,
        CircleGreen,
        CircleCheckGreen,
        CircleRed,
        CircleCrossRed,
        CircleYellow,
        CircleExclamationYellow,
        CrossRed,        
        ExclamationYellow,
        FlagGreen,
        FlagRed,
        FlagYellow,
        FromRedToBlackGray,
        FromRedToBlackPink,        
        FromRedToBlackRed,        
        Latin1,
        Latin2,
        Latin3,
        Latin4,
        LightsGreen,
        LightsRed,
        LightsYellow,
        MinusYellow,
        QuarterFull,
        QuarterFullGreen,
        QuarterFullRed,
        QuarterHalf,
        QuarterHalfGreen,
        QuarterHalfRed,
        QuarterNone,
        QuarterNoneGreen,
        QuarterNoneRed,
        QuarterQuarter,
        QuarterQuarterGreen,
        QuarterQuarterRed,
        QuarterThreeFourth,
        QuarterThreeFourthGreen,
        QuarterThreeFourthRed,
        Rating0,
        Rating1,
        Rating2,
        Rating3,
        Rating4,
        RhombRed,
        Square0,
        Square1,
        Square2,
        Square3,
        Square4,
        StarFull,
        StarHalf,
        StarNone,
        StarQuarter,        
        StarThreeFourth,        
        TriangleGreen,
        TriangleRed,
        TriangleYellow,
        
    }
    #endregion 

    #region StiIconSetOperation
    /// <summary>
    /// Operation which used in Icon Sets.
    /// </summary>
    public enum StiIconSetOperation
    {
        /// <summary>
        /// >.
        /// </summary>
        MoreThan,
        /// <summary>
        /// >=.
        /// </summary>
        MoreThanOrEqual,
    }
    #endregion 

    #region StiMinimumType
    /// <summary>
    /// Types of value of Icon Sets.
    /// </summary>
    public enum StiIconSetValueType
    {
        /// <summary>
        /// Specified value.
        /// </summary>
        Value,
        /// <summary>
        /// Specified percent.
        /// </summary>
        Percent
    }
    #endregion 

    #region StiProcessAt
    /// <summary>
    /// A modes of the text processing.
    /// </summary>
    public enum StiProcessAt
    {
        /// <summary>
        /// No special text processing.
        /// </summary>
        None,
        /// <summary>
        /// Process text component at end of the report rendering.
        /// </summary>
        EndOfReport,
        /// <summary>
        /// Process text component at end of the page rendering.
        /// </summary>
        EndOfPage
    }
    #endregion 

    #region StiMinimumType
    /// <summary>
    /// Types of minimal values in indicator classes.
    /// </summary>
    public enum StiMinimumType
    {
        /// <summary>
        /// Minimal value will be calculated automatically based on values from data list. 
        /// If calculated minimal value will be more than 0 then 0 will be used as minimum value.
        /// </summary>
        Auto,
        /// <summary>
        /// Specified minimal value.
        /// </summary>
        Value,
        /// <summary>
        /// Miminal value specified as percentage from all minimal values from data list.
        /// </summary>
        Percent,
        /// <summary>
        /// Minimal value will be calculated automatically based on values from data list.
        /// If calculated minimal value will be more than 0 then minimal value will be used as minimum value.
        /// </summary>
        Minimum
    }
    #endregion 

    #region StiMidType
    /// <summary>
    /// Types of mid values in indicator classes.
    /// </summary>
    public enum StiMidType
    {
        /// <summary>
        /// Mid value will be calculated as mid value between minimal and maximal values from data list.
        /// </summary>
        Auto,
        /// <summary>
        /// Specified mid value.
        /// </summary>
        Value,
        /// <summary>
        /// Mid value specified as percentage from all minimal and maximal values from data list.
        /// </summary>
        Percent
    }
    #endregion 

    #region StiMaximumType
    /// <summary>
    /// Types of maximal values in indicator classes.
    /// </summary>
    public enum StiMaximumType
    {
        /// <summary>
        /// Maximal value will be calculated automatically based on values from data list. 
        /// If calculated maximal value will be less than 0 then 0 will be used as maximum value.
        /// </summary>
        Auto,
        /// <summary>
        /// Specified maximal value.
        /// </summary>
        Value,
        /// <summary>
        /// Maximal value specified as percentage from all maximal values from data list.
        /// </summary>
        Percent,
        /// <summary>
        /// Maximal value will be calculated automatically based on values from data list.
        /// If calculated maximal value will be less than 0 then maximal value will be used as maximum value.
        /// </summary>
        Maximum
    }
    #endregion 

    #region StiBrushType
    /// <summary>
    /// Styles of brush for drawing Data Bar Indicator
    /// </summary>
    public enum StiBrushType
    {
        /// <summary>
        /// Solid brush.
        /// </summary>
        Solid,
        /// <summary>
        /// Gradient brush.
        /// </summary>
        Gradient
    }
    #endregion 

    #region StiColorScaleType
    /// <summary>
    /// Types of color scale indicator.
    /// </summary>
    public enum StiColorScaleType
    {
        /// <summary>
        /// Color scale with two colors.
        /// </summary>
        Color2,
        /// <summary>
        /// Color scale with three colors.
        /// </summary>
        Color3
    }
    #endregion 

    #region StiDataBarDirection
    /// <summary>
    /// In which direction data bar will be filled by brush, from left to right or from right to left.
    /// </summary>
    public enum StiDataBarDirection
    {
        /// <summary>
        /// Default direction. Direction taked from text component.
        /// </summary>
        Default,
        /// <summary>
        /// From left to right direction.
        /// </summary>
        LeftToRight,
        /// <summary>
        /// From right to left direction.
        /// </summary>
        RighToLeft
    }
    #endregion

    #region StiDrillDownMode
    public enum StiDrillDownMode
    {
        SinglePage,
        MultiPage
    }
    #endregion

    #region StiConditionsBorderSides
    /// <summary>
    /// Sides of the border in conditions.
    /// </summary>
    [Flags]
    public enum StiConditionBorderSides
    {        
        /// <summary>
        /// No border.
        /// </summary>
        None = 0,
        /// <summary>
        /// Border from all sides.
        /// </summary>
        All = 15,
        /// <summary>
        /// Border on the top.
        /// </summary>
        Top = 1,
        /// <summary>
        /// Border on the left.
        /// </summary>
        Left = 2,
        /// <summary>
        /// Border on the right.
        /// </summary>
        Right = 4,
        /// <summary>
        /// Border on the bottom.
        /// </summary>
        Bottom = 8,
        /// <summary>
        /// Not assigned.
        /// </summary>
        NotAssigned = 16
    }
    #endregion 
    
    #region StiConditionPermissions
    /// <summary>
    /// Permissions of report conditions.
    /// </summary>
    [Flags]
    public enum StiConditionPermissions
    {
        /// <summary>
        /// Without any options.
        /// </summary>
        None = 0,
        /// <summary>
        /// Allow use Font name in component.
        /// </summary>
        Font = 1,
        /// <summary>
        /// Allow use Font Size in component.
        /// </summary>
        FontSize = 2,
        /// <summary>
        /// Allow use Bold style of Font in component.
        /// </summary>
        FontStyleBold = 4,
        /// <summary>
        /// Allow use Italic style of Font in component.
        /// </summary>
        FontStyleItalic = 8,
        /// <summary>
        /// Allow use Underline style of Font in component.
        /// </summary>
        FontStyleUnderline = 16,
        /// <summary>
        /// Allow use Strikeout style of Font in component.
        /// </summary>
        FontStyleStrikeout = 32,
        /// <summary>
        /// Allow use TextColor in component.
        /// </summary>
        TextColor = 64,
        /// <summary>
        /// Allow use BackColor in component.
        /// </summary>
        BackColor = 128,
        /// <summary>
        /// Allow use Borders in component.
        /// </summary>
        Borders = 256,
        /// <summary>
        /// All options.
        /// </summary>
        All = 511
    }
    #endregion 

	#region StiQuickInfoType
	public enum StiQuickInfoType
	{
		None,
		ShowComponentsNames,
		ShowAliases,
		ShowFieldsOnly,
		ShowFields,
		ShowEvents,
		ShowContent
	}
	#endregion

	#region StiAngle
	public enum StiAngle
	{
		Angle0 = 0,
		Angle90 = 90,
		Angle180 = 180,
		Angle270 = 270
	}
	#endregion

	#region StiDockStyle
	public enum StiDockStyle
	{
		Left,
		Right,
		Top,
		Bottom,
		None,
		Fill
	}
	#endregion

	#region StiFilterCondition
	public enum StiFilterCondition
	{
		EqualTo,
		NotEqualTo,
		GreaterThan,
		GreaterThanOrEqualTo,
		LessThan,
		LessThanOrEqualTo,
		Between,
		NotBetween,
		Containing,
		NotContaining,
		BeginningWith,
		EndingWith,
        IsNull,
        IsNotNull
	}
	#endregion

	#region StiFilterItem
	public enum StiFilterItem
	{
		Argument,
		Value,
        ValueEnd,
		Expression,
        ValueOpen,
        ValueClose,
        ValueLow,
        ValueHigh,
        Tooltip
	}
	#endregion

	#region StiFilterDataType
	public enum StiFilterDataType
	{
		String,
		Numeric,
		DateTime,
		Boolean,
		Expression
	}
	#endregion

	#region StiFilterMode
	public enum StiFilterMode
	{
		And,
		Or
	}
	#endregion

    #region StiFilterEngine
    public enum StiFilterEngine
    {
        ReportEngine,
        SQLQuery
    }
    #endregion

    #region StiKeepDetails
    public enum StiKeepDetails
    {
        None,
        KeepFirstRowTogether,
        KeepFirstDetailTogether,
        KeepDetailsTogether
    }
    #endregion

	#region StiPrintOnType
	[Flags]
	public enum StiPrintOnType
	{
		AllPages = 0,
		ExceptFirstPage = 1,
		ExceptLastPage = 2,
		ExceptFirstAndLastPage = 3,
		OnlyFirstPage = 4,
		OnlyLastPage = 8,
		OnlyFirstAndLastPage = 12
	}
	#endregion

	#region StiPrintOnEvenOddPagesType
	public enum StiPrintOnEvenOddPagesType
	{
		Ignore,
		PrintOnEvenPages,
		PrintOnOddPages
	}
	#endregion

	#region StiShiftMode
	[Flags]
	public enum StiShiftMode
	{
		None = 0,
		IncreasingSize = 1,
		DecreasingSize = 2,
		OnlyInWidthOfComponent = 4,
	}
	#endregion

	#region StiExceedMargins
	[Flags]
	public enum StiExceedMargins
	{
		Left = 1,
		Right = 2,
		Top = 4,
		Bottom = 8,
		None = 0,
		All = Left + Right + Top + Bottom
	}
	#endregion

	#region StiAnchorMode
	[Flags]
    public enum StiAnchorMode
    {
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8
    }
    #endregion

	#region StiProcessingDuplicatesType
	public enum StiProcessingDuplicatesType
	{
		None,
		Merge,
		Hide,
        RemoveText,

        BasedOnTagMerge,
        BasedOnTagHide,
        BasedOnTagRemoveText,

		GlobalMerge,
		GlobalHide,
		GlobalRemoveText,

        BasedOnValueRemoveText,
        BasedOnValueAndTagMerge,
        BasedOnValueAndTagHide,

        GlobalBasedOnValueRemoveText,
        GlobalBasedOnValueAndTagMerge,
        GlobalBasedOnValueAndTagHide
	}
	#endregion

    #region StiImageProcessingDuplicatesType
    public enum StiImageProcessingDuplicatesType
    {
        None,
        Merge,
        Hide,
        RemoveImage,
        GlobalMerge,
        GlobalHide,
        GlobalRemoveImage
    }
    #endregion

	#region StiCheckStyle
	/// <summary>
	/// Check style.
	/// </summary>
	public enum StiCheckStyle
	{
		/// <summary>
		/// Cross style.
		/// </summary>
		Cross,

		/// <summary>
		/// Check style.
		/// </summary>
		Check,

		/// <summary>
		/// Check style.
		/// </summary>
		CrossRectangle,

		/// <summary>
		/// Check style.
		/// </summary>
		CheckRectangle,

		/// <summary>
		/// Check style.
		/// </summary>
		CrossCircle,

		/// <summary>
		/// Check style.
		/// </summary>
		DotCircle,

		/// <summary>
		/// Check style.
		/// </summary>
		DotRectangle,

		/// <summary>
		/// Check style.
		/// </summary>
		NoneCircle,

		/// <summary>
		/// Check style.
		/// </summary>
		NoneRectangle,

		/// <summary>
		/// Check style.
		/// </summary>
		None
	}
	#endregion

	#region StiReportControlToolboxPosition
	/// <summary>
	/// Enumeration which sets positions of the report controls on the toolbox in the report designer.
	/// </summary>
	public enum StiReportControlToolboxPosition
	{
		ReportControl = 0,
		/// <summary>
		/// Sets a position of the Label control on the toolbox of the designer.
		/// </summary>
		LabelControl = 1,
		/// <summary>
		/// Sets a position of the TextBox control on the toolbox of the designer.
		/// </summary>
		TextBoxControl = 2,
		/// <summary>
		/// Sets a position of the GroupBox control on the toolbox of the designer.
		/// </summary>
		GroupBoxControl = 3,
		/// <summary>
		/// Sets a position of the Button control on the toolbox of the designer.
		/// </summary>
		ButtonControl = 4,
		/// <summary>
		/// Sets a position of the CheckBox control on the toolbox of the designer.
		/// </summary>
		CheckBoxControl = 5,
		/// <summary>
		/// Sets a position of the RadioButton control on the toolbox of the designer.
		/// </summary>
		RadioButtonControl = 6,
		/// <summary>
		/// Sets a position of the ListBox control on the toolbox of the designer.
		/// </summary>
		ListBoxControl = 7,
		/// <summary>
		/// Sets a position of the ComboBox control on the toolbox of the designer.
		/// </summary>
		ComboBoxControl = 8,
		/// <summary>
		/// Sets a position of the LookUpBox control on the toolbox of the designer.
		/// </summary>
		LookUpBoxControl = 9,
		/// <summary>
		/// Sets a position of the CheckListBox control on the toolbox of the designer.
		/// </summary>
		CheckedListBoxControl = 10,
		/// <summary>
		/// Sets a position of the DateTimePicker control on the toolbox of the designer.
		/// </summary>
		DateTimePickerControl = 11,
		/// <summary>
		/// Sets a position of the NumericUpDown control on the toolbox of the designer.
		/// </summary>
		NumericUpDownControl = 12,
		/// <summary>
		/// Sets a position of the PictureBox control on the toolbox of the designer.
		/// </summary>
		PictureBoxControl = 13,
		/// <summary>
		/// Sets a position of the Grid control on the toolbox of the designer.
		/// </summary>
		GridControl = 14,
		/// <summary>
		/// Sets a position of the TreeView control on the toolbox of the designer.
		/// </summary>
		TreeViewControl = 15,
		/// <summary>
		/// Sets a position of the ListView control on the toolbox of the designer.
		/// </summary>
		ListViewControl = 16,
		/// <summary>
		/// Sets a position of the Panel control on the toolbox of the designer.
		/// </summary>
		PanelControl = 17,
		/// <summary>
		/// Sets a position of the RichTextBox control on the toolbox of the designer.
		/// </summary>
		RichTextBoxControl = 18,
		/// <summary>
		/// Sets a position of the User control on the toolbox of the designer.
		/// </summary>
		UserControl = 100
	}
    #endregion

    #region StiToolboxCategory
    public enum StiToolboxCategory
    {
        Bands = 0,
        Cross = 1,
        Components = 2,
        Shapes = 3,
        Controls = 4,
        Dashboards = 5,
		ComponentUIs = 6
    }
    #endregion

    #region StiComponentToolboxPosition
    /// <summary>
	/// Enumeration which sets positions of the report components on the toolbox of the report designer.
	/// </summary>
	public enum StiComponentToolboxPosition
	{
		Component = 0,
		/// <summary>
		/// Sets a position of the ReportTitle band on the toolbox of the designer.
		/// </summary>
		ReportTitleBand = 1,
		/// <summary>
		/// Sets a position of the ReportSummary band on the toolbox of the designer.
		/// </summary>
		ReportSummaryBand = 2,
		/// <summary>
		/// Sets a position of the PageHeader band on the toolbox of the designer.
		/// </summary>
		PageHeaderBand = 3,
		/// <summary>
		/// Sets a position of the PageFooter band on the toolbox of the designer.
		/// </summary>
		PageFooterBand = 4,
		/// <summary>
		/// Sets a position of the GroupHeader band on the toolbox of the designer.
		/// </summary>
		GroupHeaderBand = 5,
		/// <summary>
		/// Sets a position of the GroupFooter band on the toolbox of the designer.
		/// </summary>
		GroupFooterBand = 6,
		/// <summary>
		/// Sets a position of the Header band on the toolbox of the designer.
		/// </summary>
		HeaderBand = 7,
		/// <summary>
		/// Sets a position of the Footer band on the toolbox of the designer.
		/// </summary>
		FooterBand = 8,
		/// <summary>
		/// Sets a position of the ColumnHeader band on the toolbox of the designer.
		/// </summary>
		ColumnHeaderBand = 9,
		/// <summary>
		/// Sets a position of the ColumnFooter band on the toolbox of the designer.
		/// </summary>
		ColumnFooterBand = 10,
		/// <summary>
		/// Sets a position of the Data band on the toolbox of the designer.
		/// </summary>		
		DataBand = 11,
		/// <summary>
		/// Sets a position of the Hierarchical band on the toolbox of the designer.
		/// </summary>		
		HierarchicalBand= 13,		
		/// <summary>
		/// Sets a position of the Child band on the toolbox of the designer.
		/// </summary>
		ChildBand = 14,
		/// <summary>
		/// Sets a position of the Empty band on the toolbox of the designer.
		/// </summary>
		EmptyBand = 15,
		/// <summary>
		/// Sets a position of the Overlay band on the toolbox of the designer.
		/// </summary>	
		OverlayBand = 16,
		/// <summary>
		/// Sets a position of the CrossGroupHeader band on the toolbox of the designer.
		/// </summary>
		CrossGroupHeaderBand = 21,
		/// <summary>
		/// Sets a position of the CrossGroupFooter band on the toolbox of the designer.
		/// </summary>
		CrossGroupFooterBand = 22,
		/// <summary>
		/// Sets a position of the CrossHeader band on the toolbox of the designer.
		/// </summary>
		CrossHeaderBand = 23,
		/// <summary>
		/// Sets a position of the CrossFooter band on the toolbox of the designer.
		/// </summary>
		CrossFooterBand = 24,
		/// <summary>
		/// Sets a position of the CrossData band on the toolbox of the designer.
		/// </summary>		
		CrossDataBand = 25,	
		/// <summary>
		/// Sets a position of the Text component on the toolbox of the designer.
		/// </summary>
		Text = 101,
		/// <summary>
		/// Sets a position of the TextInCells component on the toolbox of the designer.
		/// </summary>
		TextInCells = 102,
		/// <summary>
		/// Sets a position of the SystemText component on the toolbox of the designer.
		/// </summary>
		SystemText = 103,
		/// <summary>
		/// Sets a position of the ContourText component on the toolbox of the designer.
		/// </summary>
		ContourText = 104,
		/// <summary>
		/// Sets a position of the RichText component on the toolbox of the designer.
		/// </summary>
		RichText = 105,
		/// <summary>
		/// Sets a position of the Image component on the toolbox of the designer.
		/// </summary>
		Image = 106,
        /// <summary>
        /// Sets a position of the ElectronicSignature component on the toolbox of the designer.
        /// </summary>
        ElectronicSignature = 107,
        /// <summary>
        /// Sets a position of the PdfDigitalSignature component on the toolbox of the designer.
        /// </summary>
        PdfDigitalSignature = 108,
        /// <summary>
        /// Sets a position of the BarCode component on the toolbox of the designer.
        /// </summary>
        BarCode = 109,
		/// <summary>
		/// Sets a position of the Shape component on the toolbox of the designer.
		/// </summary>	
		Shape = 110,
		/// <summary>
		/// Sets a position of the Line component on the toolbox of the designer.
		/// </summary>
		Line = 111,
		/// <summary>
		/// Sets a position of the Container component on the toolbox of the designer.
		/// </summary>
		Container = 112,
        /// <summary>
        /// Sets a position of the Panel component on the toolbox of the designer.
        /// </summary>
        Panel = 113,
		/// <summary>
		/// Sets a position of the Clone component on the toolbox of the designer.
		/// </summary>
		Clone = 114,
		/// <summary>
		/// Sets a position of the CheckBox component on the toolbox of the designer.
		/// </summary>
		CheckBox = 115,
		/// <summary>
		/// Sets a position of the SubReport component on the toolbox of the designer.
		/// </summary>
		SubReport = 116,
		/// <summary>
		/// Sets a position of the WinControl component on the toolbox of the designer.
		/// </summary>		
		WinControl = 117,
		/// <summary>
		/// Sets a position of the ZipCode component on the toolbox of the designer.
		/// </summary>
		ZipCode = 118,
		/// <summary>
		/// Sets a position of the TableOfContents component on the toolbox of the designer.
		/// </summary>
		TableOfContents = 119,
        /// <summary>
        /// Sets a position of the HorizontalLinePrimitive component on the toolbox of the designer.
        /// </summary>
        HorizontalLinePrimitive = 150,
		/// <summary>
		/// Sets a position of the VerticalLinePrimitive component on the toolbox of the designer.
		/// </summary>
		VerticalLinePrimitive = 151,
		/// <summary>
		/// Sets a position of the RectanglePrimitive component on the toolbox of the designer.
		/// </summary>
		RectanglePrimitive = 152,
        /// <summary>
        /// Sets a position of the RoundedRectanglePrimitive component on the toolbox of the designer.
        /// </summary>
        RoundedRectanglePrimitive = 153,
        
        /// <summary>
        /// Sets a position of the Chart component on the toolbox of the designer.
        /// </summary>
        Chart = 200,
        /// <summary>
        /// Sets a position of the Table on the toolbox of the designer.
        /// </summary>	
        Table = 201,
        /// <summary>
        /// Sets a position of the CrossTab band on the toolbox of the designer.
        /// </summary>
        CrossTab = 202,
        /// <summary>
        /// Sets a position of the Gauge component on the toolbox of the designer.
        /// </summary>
        Gauge = 210,
        /// <summary>
        /// Sets a position of the Gauge component on the toolbox of the designer.
        /// </summary>
        Map = 220,
		/// <summary>
		/// Sets a position of the Sparkline component on the toolbox of the designer.
		/// </summary>
		Sparkline = 225,
		/// <summary>
		/// Sets a position of the Math Formula component on the toolbox of the designer.
		/// </summary>
		MathFormula = 230,

		/// <summary>
		/// Sets a position of the Dashboard Table component on the toolbox of the designer.
		/// </summary>
		TableElement = 301,
		/// <summary>
		/// Sets a position of the Dashboard Crads component on the toolbox of the designer.
		/// </summary>
		CardsElement = 302,
		/// <summary>
		/// Sets a position of the Dashboard Pivot component on the toolbox of the designer.
		/// </summary>
		PivotTableElement = 303,
		/// <summary>
		/// Sets a position of the Dashboard Chart component on the toolbox of the designer.
		/// </summary>
		ChartElement = 304,        
        /// <summary>
        /// Sets a position of the Dashboard Gauge component on the toolbox of the designer.
        /// </summary>
        GaugeElement = 305,
	    /// <summary>
	    /// Sets a position of the Dashboard Indicator component on the toolbox of the designer.
	    /// </summary>
	    IndicatorElement = 306,
	    /// <summary>
	    /// Sets a position of the Dashboard Progress component on the toolbox of the designer.
	    /// </summary>
	    ProgressElement = 307,
		/// <summary>
		/// Sets a position of the Dashboard Map component on the toolbox of the designer.
		/// </summary>
		RegionMapElement = 308,
        /// <summary>
	    /// Sets a position of the Dashboard Online Map component on the toolbox of the designer.
	    /// </summary>
	    OnlineMapElement = 310,
        /// <summary>
        /// Sets a position of the Dashboard Image component on the toolbox of the designer.
        /// </summary>
        ImageElement = 311,
	    /// <summary>
	    /// Sets a position of the Dashboard Text component on the toolbox of the designer.
	    /// </summary>
	    TextElement = 312,
	    /// <summary>
	    /// Sets a position of the Dashboard Panel component on the toolbox of the designer.
	    /// </summary>
	    PanelElement = 313,
		/// <summary>
		/// Sets a position of the Dashboard Shape component on the toolbox of the designer.
		/// </summary>
		ShapeElement = 314,
		/// <summary>
		/// Sets a position of the Dashboard Button component on the toolbox of the designer.
		/// </summary>
		ButtonElement = 315,
		/// <summary>
		/// Sets a position of the Dashboard ComboBox component on the toolbox of the designer.
		/// </summary>
		ComboBoxElement = 316,
		/// <summary>
		/// Sets a position of the Dashboard DatePicker component on the toolbox of the designer.
		/// </summary>
		DatePickerElement = 317,
		/// <summary>
		/// Sets a position of the Dashboard ListBox component on the toolbox of the designer.
		/// </summary>
		ListBoxElement = 318,
		/// <summary>
		/// Sets a position of the Dashboard TreeViewBox component on the toolbox of the designer.
		/// </summary>
		TreeViewBoxElement = 319,
		/// <summary>
		/// Sets a position of the Dashboard TreeView component on the toolbox of the designer.
		/// </summary>
		TreeViewElement = 320,

		/// <summary>
		/// Sets a position of the Apps Button component on the toolbox of the designer.
		/// </summary>
		ButtonUI = 400,
		/// <summary>
		/// Sets a position of the Apps CheckBox component on the toolbox of the designer.
		/// </summary>
		CheckBoxUI = 401,
		/// <summary>
		/// Sets a position of the Apps Panel component on the toolbox of the designer.
		/// </summary>
		PanelUI = 402,
		
		/// <summary>
		/// Sets a position of the UserCode component on the toolbox of the designer.
		/// </summary>
		UserCode = 1000
	}
	#endregion

	#region StiComponentPriority
	/// <summary>
	/// Enumeration which sets the priority of processing of components.
	/// </summary>
	public enum StiComponentPriority
	{
		Component = 0,

		CrossTab = 1500,
		SubReportsV1 = 1500,		
		SubReportsV2 = 0,
        Container = 0,
        Panel = 0,

		ReportTitleBandBefore = -400,
		ReportTitleBandAfterV1 = -200,
		ReportTitleBandAfterV2 = 200,
		ReportSummaryBand = 500,
		PageHeaderBandBefore = -200,
		PageHeaderBandAfter = -400,
		PageFooterBandBottom = -300,
		PageFooterBandTop = 1000,
		GroupHeaderBand = 300,
		GroupFooterBand = 300,
		HeaderBand = 300,
		FooterBand = 300,
		ColumnHeaderBand = 300,
		ColumnFooterBand = 300,		
		DataBand = 300,
		Table = 300,
		ChildBand = 300,
		EmptyBand = 300,
		TableOfContents = 300,
		OverlayBand = 700,

		Primitive = 1500,

		CrossGroupHeaderBand = 300,
		CrossGroupFooterBand = 300,
		CrossHeaderBand = 300,
		CrossFooterBand = 300,		
		CrossDataBand = 300,	

	}
	#endregion

	#region StiMarkersStyle
	/// <summary>
	/// Enumeration which sets a style of markers of the component in the designer.
	/// </summary>
	public enum StiMarkersStyle
	{
		/// <summary>
		/// No markers visible around the component.
		/// </summary>
		None,
		/// <summary>
		/// Corners of the component are visible as markers of its size. 
		/// </summary>
		Corners,
		/// <summary>
		/// Markers represents by the dashed rectangle.
		/// </summary>
		DashedRectangle
	}
	#endregion

	#region StiPageType
	/// <summary>
	/// Types of a page.
	/// </summary>
	[Flags]
	public enum StiPageType
	{
		/// <summary>
		/// Dialog form.
		/// </summary>
		Form = 1,
		/// <summary>
		/// Page.
		/// </summary>
		Page = 2,
	    /// <summary>
	    /// Dashboard.
	    /// </summary>
	    Dashboard = 4,
		/// <summary>
		/// ScreenUI.
		/// </summary>
		Screen = 8,
		/// <summary>
		/// All.
		/// </summary>
		All = Form + Page + Dashboard + Screen
	}
	#endregion

	#region StiComponentType
	/// <summary>
	/// A type of processing of a component when printing.
	/// </summary>
	public enum StiComponentType 
	{
		/// <summary>
		/// Simple components - only one copy in one print cycle is output.
		/// </summary>
		Simple, 
		/// <summary>
		/// Master components – more than one copy in one print cycle are output.
		/// </summary>
		Master, 
		/// <summary>
		/// Detailed components – these components are output with Master components.
		/// </summary>
		Detail,
        /// <summary>
        /// Static components – these components are the same as simple components but are output on pages only.
        /// </summary>
        Static
	}
	#endregion

	#region StiHighlightState
	/// <summary>
	/// Types of selection of the component in the window of viewer.
	/// </summary>
	public enum StiHighlightState
	{
		/// <summary>
		/// Do not select.
		/// </summary>
		Hide,
		/// <summary>
		/// Show selection.
		/// </summary>
		Show,
		/// <summary>
		/// Active selection.
		/// </summary>
		Active
	}
	#endregion	

	#region StiRestrictions
	[Flags]
	public enum StiRestrictions
	{
		None			= 0,
		AllowMove		= 1,
		AllowResize		= 2,
		AllowSelect		= 4,
		AllowChange		= 8,
		AllowDelete		= 16,
		All				= 31
	}
	#endregion

	#region StiAligning
	/// <summary>
	/// Modes of the components alignment.
	/// </summary>
	public enum StiAligning
	{
		/// <summary>
		/// Align left.
		/// </summary>
		Left, 
		/// <summary>
		/// Align center horizontally.
		/// </summary>
		Center, 
		/// <summary>
		/// Align right.
		/// </summary>
		Right, 
		/// <summary>
		/// Align top.
		/// </summary>
		Top, 
		/// <summary>
		/// Align center vertically.
		/// </summary>
		Middle, 
		/// <summary>
		/// Align bottom.
		/// </summary>
		Bottom
	}
	#endregion

	#region StiColumnDirection
	/// <summary>
	/// Enumeration contains type of column direction.
	/// </summary>
	public enum StiColumnDirection
	{
		/// <summary>
		/// Down then Accross
		/// </summary>
		DownThenAcross,
		/// <summary>
		/// Across then down
		/// </summary>
		AcrossThenDown
	}
	#endregion

	#region StiEmptySizeMode
	public enum StiEmptySizeMode
	{
		IncreaseLastRow,
		DecreaseLastRow,
		AlignFooterToBottom,
		AlignFooterToTop,
	}
	#endregion

	#region StiGroupSortDirection
	public enum StiGroupSortDirection
	{
		Ascending,
		Descending,
		None
	}
	#endregion

    #region StiGroupSummarySortDirection
    public enum StiGroupSummarySortDirection
    {
        Ascending,
        Descending,
        None
    }
    #endregion

    #region StiGroupSummaryType
    public enum StiGroupSummaryType
    {
        Avg,

        AvgDate,
        AvgTime,

        Count,
        CountDistinct,

        MaxDate,
        MaxTime,
        Max,

        MinDate,
        MinTime,
        Min,

        Median,

        Mode,
        
        Sum,
        SumTime
    }
    #endregion

	#region StiPageOrientation
	/// <summary>
	/// Type of page orientation.
	/// </summary>
	public enum StiPageOrientation 
	{
		/// <summary>
		/// Portrait orientation.
		/// </summary>
		Portrait, 
		/// <summary>
		/// Landscape orientation.
		/// </summary>
		Landscape
	}
	#endregion

	#region StiRtfFormatType
	public enum StiRtfFormatType
	{
		DrawRtf,
		TotalRtfHeight,
		MeasureRtf
	}
	#endregion

	#region StiTextQuality
	public enum StiTextQuality
	{
		Standard,
		Typographic,
		Wysiwyg
	}
	#endregion

	#region StiSystemTextType
	public enum StiSystemTextType
	{
		Totals,
		SystemVariables,
		Expression,
		DataColumn,
		None
	}
	#endregion

	#region StiInteractionSortDirection
	public enum StiInteractionSortDirection
	{
		Ascending,
		Descending,
		None
	}
	#endregion

	#region StiImageRotation
	public enum StiImageRotation
	{
		None,
		Rotate90CW,
		Rotate90CCW,
		Rotate180,
		FlipHorizontal,
		FlipVertical
	}
    #endregion

    #region StiDashboardViewMode
    public enum StiDashboardViewMode
    {
        Desktop,
        Mobile
    }
	#endregion

	#region StiSparklineType
	/// <summary>
	/// Types of the sparkline.
	/// </summary>
	public enum StiSparklineType
	{
		/// <summary>
		/// Line type.
		/// </summary>
		Line,
		/// <summary>
		/// Area type.
		/// </summary>
		Area,
		/// <summary>
		/// Column.
		/// </summary>
		Column,
		/// <summary>
		/// WinLoss
		/// </summary>
		WinLoss
	}
	#endregion

	#region StiSparklineType
	/// <summary>
	/// Types of the sparkline.
	/// </summary>
	public enum StiMathFormulaGroupType
	{
		Basic,
		Alphabets,
		Arrows,
		Functions,
		Formula,
		Operators,
		Maths
	}
    #endregion

    #region StiSignatureMode
    public enum StiSignatureMode
	{
		Type,
		Draw
	}
	#endregion

	#region StiLabelAlignment
	public enum StiLabelAlignment
	{
		Left,
		Right,
		Top
	}
	#endregion
}