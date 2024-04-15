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

using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Xml;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes the class that realizes the component - StiText.
    /// </summary>
    [StiV1Builder(typeof(StiSimpleTextV1Builder))]
	[StiV2Builder(typeof(StiSimpleTextV2Builder))]
	public abstract class StiSimpleText : 
		StiComponent,
		IStiGlobalizedName,
		IStiText,
		IStiEditable,
		IStiProcessAtEnd,
        IStiProcessAt,
		IStiOnlyText
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiSimpleText
            jObject.AddPropertyStringNullOrEmpty("GlobalizedName", GlobalizedName);
            jObject.AddPropertyEnum("LinesOfUnderline", LinesOfUnderline, StiPenStyle.None);
            jObject.AddPropertyBool("HideZeros", HideZeros);
            jObject.AddPropertyEnum("ProcessingDuplicates", ProcessingDuplicates, StiProcessingDuplicatesType.None);
            jObject.AddPropertyInt("MaxNumberOfLines", MaxNumberOfLines);
            jObject.AddPropertyBool("OnlyText", OnlyText);
            jObject.AddPropertyBool("Editable", Editable);
            jObject.AddPropertyBool("ProcessAtEnd", ProcessAtEnd);
            jObject.AddPropertyEnum("ProcessAt", ProcessAt, StiProcessAt.None);
            jObject.AddPropertyJObject("Text", Text.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetValueEvent", GetValueEvent.SaveToJsonObject(mode));
            jObject.AddPropertyStringNullOrEmpty("TotalValueHelp", TotalValueHelp);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "GlobalizedName":
                        this.GlobalizedName = property.DeserializeString();
                        break;

                    case "LinesOfUnderline":
                        this.LinesOfUnderline = property.DeserializeEnum<StiPenStyle>();
                        break;

                    case "HideZeros":
                        this.HideZeros = property.DeserializeBool();
                        break;

                    case "ProcessingDuplicates":
                        this.ProcessingDuplicates = property.DeserializeEnum<StiProcessingDuplicatesType>();
                        break;

                    case "MaxNumberOfLines":
                        this.MaxNumberOfLines = property.DeserializeInt();
                        break;

                    case "OnlyText":
                        this.OnlyText = property.DeserializeBool();
                        break;

                    case "Editable":
                        this.Editable = property.DeserializeBool();
                        break;

                    case "ProcessAtEnd":
                        this.ProcessAtEnd = property.DeserializeBool();
                        break;

                    case "ProcessAt":
                        this.ProcessAt = property.DeserializeEnum<StiProcessAt>();
                        break;

                    case "Text":
                        {
                            var _expression = new StiExpression();
                            _expression.LoadFromJsonObject((JObject)property.Value);
                            this.text = _expression;
                        }
                        break;

                    case "GetValueEvent":
                        {
                            var _event = new StiGetValueEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.GetValueEvent = _event;
                        }
                        break;

                    case "TotalValueHelp":
                        this.TotalValueHelp = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region IStiGlobalizedName
        protected static object PropertyGlobalizedName = new object();
		/// <summary>
        /// Gets or sets special identificator which will be used for report globalization.
		/// </summary>
		[StiCategory("Design")]
		[StiOrder(StiPropertyOrder.DesignGlobalizedName)]
		[StiSerializable]
		[DefaultValue("")]
		[ParenthesizePropertyName(true)]
        [Description("Gets or sets special identificator which will be used for report globalization.")]
		[Editor("Stimulsoft.Report.Design.StiGlobabalizationManagerEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual string GlobalizedName
		{
			get
			{
                return Properties.Get(PropertyGlobalizedName, string.Empty) as string;
			}
			set
			{
                Properties.Set(PropertyGlobalizedName, value, string.Empty); 
			}
		}
		#endregion

		#region IStiStateSaveRestore
		/// <summary>
		/// Saves the current state of an object.
		/// </summary>
		/// <param name="stateName">A name of the state being saved.</param>
		public override void SaveState(string stateName)
		{
			base.SaveState(stateName);

			States.Push(stateName, this, "textValue", textValue);
		}

		/// <summary>
		/// Restores the earlier saved object state.
		/// </summary>
		/// <param name="stateName">A name of the state being restored.</param>
		public override void RestoreState(string stateName)
		{
		    if (States.IsExist(stateName, this))
		        textValue = (string) States.Pop(stateName, this, "textValue");

		    base.RestoreState(stateName);
		}
		#endregion

		#region ICloneable override
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone(bool cloneProperties)
		{
			var textBox =	(StiSimpleText)base.Clone(cloneProperties);

			if (this.text != null)
			    textBox.text = (StiExpression)this.text.Clone();
			else
			    textBox.text = null;

			return textBox;
		}
		#endregion		

		#region IStiText
		public string GetTextWithoutZero(string text)
		{
			if (!HideZeros || text == null)
			    return text;

		    if (text == "0")
		        return CanShrink ? "" : " ";

		    var str = text.Trim();
            if (str.Length == 0)
                return text;

            if (str.IndexOf('0') == -1)
                return text;

            if (str.IndexOfAny(new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' }) != -1)
                return text;

            #region Parse zero
            var sep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            try
            {
                var currencySymbol = "$";
                if (this is StiText && (this as StiText).TextFormat is StiCurrencyFormatService)
                    currencySymbol = ((this as StiText).TextFormat as StiCurrencyFormatService).Symbol;

                if (str.StartsWith(currencySymbol))
                    str = str.Substring(currencySymbol.Length);

                if (str.EndsWith(currencySymbol))
                    str = str.Substring(0, str.Length - currencySymbol.Length);

                str = str.Replace(".", ",").Replace(",", sep);

                decimal value;
                if (decimal.TryParse(str, NumberStyles.Currency, NumberFormatInfo.CurrentInfo, out value))
                {
                    if (value == 0)
                        return CanShrink ? "" : " ";

                    return text;
                }

                //Try detect zeros from percent values
                str = str.Replace("%", string.Empty);

                if (decimal.TryParse(str, NumberStyles.Number, NumberFormatInfo.CurrentInfo, out value))
                {
                    if (value == 0)
                        return CanShrink ? "" : " ";

                    return text;
                }
            }
            catch
            {
            }
            #endregion

            Report?.WriteToReportRenderingMessages($"{Name} HideZero Format Exception");

            return text;
		}

		public void SetText(StiGetValue getValue)
		{
			SetText(getValue, null);
		}

		public void SetExcelText(StiGetExcelValue getValue)
		{
			SetText(getValue, null);
		}

		public void SetText(string value)
		{
			SetText(null, value);
		}

        public void SetText(StiGetValue getValue, bool isRunning)
        {
            SetText(getValue, null, isRunning);
        }

        public void SetExcelText(StiGetExcelValue getValue, bool isRunning)
        {
            SetText(getValue, null, isRunning);
        }

        /// <summary>
        /// Sets the text value in all printed objects.
        /// </summary>
        /// <param name="value">Value for setting.</param>
        public virtual void SetText(object getValue, string value, bool isRunning = false)
		{
			var getValueHandler = getValue as StiGetValue;
			var getExcelValueHandler = getValue as StiGetExcelValue;

			#region Foreach all rendered earlier components
			if (text != null)
			{
				string name = this.Name;
				if (getExcelValueHandler != null)
				    name += "Excel";

				var renderedComponents = Report.Totals[name] as ArrayList;

                #region Check Rendered Componens collection
                if (renderedComponents != null && renderedComponents.Count > 0)
                {
                    Hashtable hashNotSaved = new Hashtable();
                    //StiRuntimeVariables lastRuntime = null;
                    foreach (StiRuntimeVariables runtime in renderedComponents)
                    {
                        //lastRuntime = runtime;

                        #region Process Cache Mode
                        var pageIndex = runtime.PageIndex;
                        if (Report.Engine != null)
                            Report.Engine.IndexPageForPageTotal = pageIndex - 1;

                        if (Report.EngineVersion == StiEngineVersion.EngineV2)
                            pageIndex--;

                        if (Report.RenderedPages.CacheMode && pageIndex < Report.RenderedPages.Count)
                        {
                            var page = Report.RenderedPages.GetPageWithoutCache(pageIndex);
                            if (Report.RenderedPages.IsNotSavedPage(page) && !hashNotSaved.ContainsKey(pageIndex))
                            {
                                SetTextTo(runtime.TextBox, runtime, getValue, value);
                                continue;
                            }

                            if (StiOptions.Engine.ReportCache.OptimizeEndRenderSetText)
                            {
                                #region Sort data by pages and store
                                var pagesStoreArray = Report.Totals["%%%=Pages=%%%"] as Hashtable;
                                if (pagesStoreArray == null)
                                {
                                    pagesStoreArray = new Hashtable();
                                    Report.Totals["%%%=Pages=%%%"] = pagesStoreArray;
                                }

                                var pageStore = pagesStoreArray[pageIndex] as List<DictionaryEntry>;
                                if (pageStore == null)
                                {
                                    pageStore = new List<DictionaryEntry>();
                                    pagesStoreArray[pageIndex] = pageStore;
                                }

                                var de = new DictionaryEntry(runtime, getValue);
                                pageStore.Add(de);
                                continue;
                                #endregion
                            }

                            hashNotSaved[pageIndex] = null;
                            Report.RenderedPages.GetPage(page);

                            var comps = page.GetComponents();
                            foreach (StiComponent comp in comps)
                            {
                                var compText = comp as StiSimpleText;
                                if (compText != null && comp.Name == this.Name)
                                {
                                    if (compText.TotalValueHelp == null)
                                    {
                                        SetTextTo(comp, runtime, getValue, value);
                                        break;
                                    }
                                }
                            }

                            //update, 2016.02.10
                            if (Report.EngineVersion == StiEngineVersion.EngineV1 && !Report.RenderedPages.IsNotSavedPage(page))
                                Report.RenderedPages.SavePage(page);

                            //update, 2016.04.14
                            if (Report.EngineVersion == StiEngineVersion.EngineV2)
                                Report.RenderedPages.MarkPageAsNotSaved(page);
                        }
                        #endregion
                        else
                        {
                            SetTextTo(runtime.TextBox, runtime, getValue, value);
                        }
                    }

					// когда-то этот фикс был нужен (2019.2.x), некоторые неотрендеренные значения агрегатных функций заменял на последние отрендеренные.
					// но с этим фиксом ломается сумма по колонке страницы, только для первой колонки считается.
					// к сожалению, не удалось воспроизвести ситуацию, для которой фикс нужен. поэтому пока просто оставляю закомментированным (2020.5.2)
					//
                    //var lastText = lastRuntime.TextBox as StiText;
                    //if (lastText != null && !isRunning && Report.EngineVersion == StiEngineVersion.EngineV2)
                    //{
                    //    if (getValueHandler != null) text = lastText.Text;
                    //    if (getExcelValueHandler != null) ((StiText)this).ExcelDataValue = lastText.ExcelDataValue;
                    //}

                    hashNotSaved.Clear();
                    renderedComponents.Clear();
				}
				#endregion
				else
				{
				    if (value != null)
				    {
				        text = value;
				    }
					else 
					{
                        try
                        {
                            if (getValueHandler != null)
                                text = getValueHandler(this);

                            if (getExcelValueHandler != null)
                                ((StiText)this).ExcelDataValue = getExcelValueHandler(this);
                        }
                        catch (Exception ex)
                        {
                            var str = $"Expression in Text property of '{Name}' can't be evaluated! {ex.Message}";
                            StiLogService.Write(this.GetType(), str);
                            StiLogService.Write(this.GetType(), ex.Message);
                            Report.WriteToReportRenderingMessages(str);
                        }
                    }
				}
			}			
			#endregion
			
			//if further printout is met this component
			//that is necessary to use this text
			if (getExcelValueHandler == null)
			    textValue = ProcessText(GetTextWithoutZero(text));
		}

        //Check if not all stored runtimes are calculated
        public static void CheckEndRenderRuntimes(StiReport report)
        {
            if (report == null || report.Totals == null || report.Totals.Count == 0) return;

            var totalsCopy = new Hashtable(report.Totals);
            foreach (DictionaryEntry de in totalsCopy)
            {
                string compName = de.Key as string;
                if (compName != null && compName.StartsWith("#%#"))
                {
                    try
                    {
                        compName = compName.Substring(3);
                        var comp = report.GetComponentByName(compName).Clone() as StiText;
                        var getValueHandler = (StiGetValue)Delegate.CreateDelegate(typeof(StiGetValue), report, compName + "_GetValue_End");
                        comp.SetText(getValueHandler, null);
                    }
                    catch { }
                }
            }
        }

        public static void ProcessEndRenderSetText(StiReport report)
        {
            if (report == null || report.Totals == null) return;

            var pagesStoreArray = report.Totals["%%%=Pages=%%%"] as Hashtable;
            if (pagesStoreArray == null || pagesStoreArray.Count == 0) return;

            for (int pageIndex = 0; pageIndex < report.RenderedPages.Count; pageIndex++)
            {
                var pageStore = pagesStoreArray[pageIndex] as List<DictionaryEntry>;
                if (pageStore == null || pageStore.Count == 0) continue;

                StiPage page = report.RenderedPages[pageIndex];
                report.RenderedPages.GetPage(page);
                StiComponentsCollection comps = page.GetComponents();

                foreach (DictionaryEntry de in pageStore)
                {
                    var runtime = de.Key as StiRuntimeVariables;
                    var getValue = de.Value;
                    var rtName = runtime.TextBox.Name;

                    foreach (StiComponent comp in comps)
                    {
                        StiSimpleText compText = comp as StiSimpleText;
                        if (compText != null && comp.Name == rtName)
                        {
                            if (compText.TotalValueHelp == null)
                            {
                                //optimization
                                if (report.EngineVersion == StiEngineVersion.EngineV2)
                                {
                                    runtime.CurrentPrintPage = pageIndex + 1;
                                    runtime.TextBox = null;
                                }

                                compText.SetTextTo(comp, runtime, getValue, null);    //optimized
                                break;
                            }
                        }
                    }
                }

                if (report.EngineVersion == StiEngineVersion.EngineV1 && !report.RenderedPages.IsNotSavedPage(page))
                    report.RenderedPages.SavePage(page);

                if (report.EngineVersion == StiEngineVersion.EngineV2)
                    report.RenderedPages.MarkPageAsNotSaved(page);
            }
            pagesStoreArray.Clear();
        }

        internal void SetTextTo(StiComponent comp, StiRuntimeVariables runtime, object getValue, string value)
		{
			var textComp = comp as IStiText;
		    if (textComp == null)return;

		    var getValueHandler = getValue as StiGetValue;
		    var getExcelValueHandler = getValue as StiGetExcelValue;

		    ((StiSimpleText)comp).TotalValueHelp = "1";
		    var pageNumber = Report.PageNumber;
		    var storedVariables = new StiRuntimeVariables(Report);
		    runtime.SetVariables(Report);
							
		    //Get string which necessary to assign component
		    var txt = string.Empty;
		    var valueExcel = "-";

		    #region isCompilationMode
		    var isCompilationMode = true;
		    if (Report != null)
		        isCompilationMode = Report.CalculationMode == StiCalculationMode.Compilation;
		    #endregion

		    if (value != null)
		    {
		        text = value;
		    }
		    else
		    {
		        #region Compilation Mode
		        if (isCompilationMode)
		        {
		            if (getValueHandler != null)
		            {
		                try
		                {
		                    txt = getValueHandler(comp);
		                }
		                catch (Exception ex)
		                {
		                    var str = $"Expression in Text property of '{Name}' can't be evaluated! {ex.Message}";
		                    StiLogService.Write(this.GetType(), str);
		                    StiLogService.Write(this.GetType(), ex.Message);
		                    Report.WriteToReportRenderingMessages(str);
		                }
		            }
		        }
		        #endregion

		        #region Interpretation Mode
		        else
		        {
		            try
		            {
		                var stiText = comp as StiText;
		                var hasExcelValue = stiText != null && stiText.ExcelValue.Value != null && stiText.ExcelValue.Value.Length > 0;
		                var textExpression = stiText.Text.Value;
                            
		                if (Report.Totals.Contains($"#%#{comp.Name}"))
		                    textExpression = Report.Totals[$"#%#{comp.Name}"] as string;

		                var storeToPrint = false;
		                var parserResult = StiParser.ParseTextValue(textExpression, this, stiText, ref storeToPrint, true);
                            
		                if (stiText != null && stiText.Format != null && stiText.Format != "G")
		                {
		                    txt = hasExcelValue
		                        ? (this as StiText).TextFormat.Format(parserResult)
		                        : (this as StiText).TextFormat.Format(Report.CheckExcelValue(comp, parserResult));
		                }
		                else
		                {
		                    txt = Report.ToString(comp, parserResult, !hasExcelValue);
		                }
		            }
		            catch (Exception ex)
		            {
		                var str = $"Expression in Text property of '{Name}' can't be evaluated! {ex.Message}";
		                StiLogService.Write(this.GetType(), str);
		                StiLogService.Write(this.GetType(), ex.Message);
		                Report.WriteToReportRenderingMessages(str);
		            }
		        }
		        #endregion

		        #region Invoke GetExcelValueHandler
		        if (getExcelValueHandler != null)
		        {
		            try
		            {
		                valueExcel = getExcelValueHandler(comp);
		            }
		            catch (Exception ex)
		            {
		                var str = $"Expression in Text property of '{Name}' can't be evaluated! {ex.Message}";
		                StiLogService.Write(this.GetType(), str);
		                StiLogService.Write(this.GetType(), ex.Message);
		                Report.WriteToReportRenderingMessages(str);
		            }
		        }
		        #endregion
		    }

		    storedVariables.SetVariables(Report);

		    Report.PageNumber = pageNumber;

		    #region Compilation Mode
		    if (isCompilationMode)
		    {
		        if (getValueHandler != null)
		        {
		            var args = new StiGetValueEventArgs();
		            args.Value = txt;
		            ((StiSimpleText)textComp).InvokeGetValue(textComp as StiText, args);

		            if (args.StoreToPrinted && (string.IsNullOrEmpty(args.Value) || args.Value.StartsWith("#%#", StringComparison.InvariantCulture)))
		                args.Value = txt;

		            var e = new StiValueEventArgs(args.Value);
		            InvokeTextProcess(comp, e);

		            var textString = ProcessText(GetTextWithoutZero(e.Value as string));
		            if (textComp is StiTextInCells)
		            {
		                var cont = StiTextInCells.SplitByCells(this as StiTextInCells, comp, textString);
		                StiTextInCells.ReplaceContainerWithContentCells(comp, cont);
		            }
		            else
		            {
		                textComp.SetTextInternal(textString);
		            }
		        }
		    }
		    #endregion

		    #region Interpretation Mode
		    else
		    {
		        var args = new StiGetValueEventArgs();
		        args.Value = txt;
		        ((StiSimpleText)textComp).InvokeGetValue(textComp as StiText, args);

		        if (args.StoreToPrinted && (string.IsNullOrEmpty(args.Value) || args.Value.StartsWith("#%#", StringComparison.InvariantCulture)))
		            args.Value = txt;

		        var e = new StiValueEventArgs(args.Value);
		        InvokeTextProcess(comp, e);

		        var textString = ProcessText(GetTextWithoutZero(e.Value as string));
		        if (textComp is StiTextInCells)
		        {
		            var cont = StiTextInCells.SplitByCells(this as StiTextInCells, comp, textString);
		            StiTextInCells.ReplaceContainerWithContentCells(comp, cont);
		        }
		        else
		        {
		            textComp.SetTextInternal(textString);
		        }
		    }
		    #endregion

		    #region Invoke GetExcelValueHandler
		    if (getExcelValueHandler != null)
		    {
		        var args = new StiGetExcelValueEventArgs();
		        args.Value = valueExcel;
		        ((StiText)textComp).InvokeGetExcelValue(textComp as StiText, args);

		        if (args.StoreToPrinted && (string.IsNullOrEmpty(args.Value) || args.Value.StartsWithInvariant("#%#")))
		            args.Value = valueExcel;

		        ((StiText)textComp).ExcelDataValue = args.Value;
		    }
		    #endregion
		}

        /// <summary>
		/// Gets or sets value indicates that it is necessary to lines of underline.
		/// </summary>
		[StiSerializable]
		[Editor(StiEditors.PenStyle, typeof(UITypeEditor))]
		[DefaultValue(StiPenStyle.None)]
		[StiCategory("TextAdditional")]
		[StiOrder(StiPropertyOrder.TextLinesOfUnderline)]
		[TypeConverter(typeof(StiEnumConverter))]
		[Description("Gets or sets value indicates that it is necessary to lines of underline.")]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual StiPenStyle LinesOfUnderline { get; set; } = StiPenStyle.None;

        /// <summary>
		/// Gets or sets value indicates that it is necessary to lines of underlining.
		/// </summary>
		[StiNonSerialized]
		[Browsable(false)]
		public virtual bool LinesOfUnderlining
		{
			get
			{
				return LinesOfUnderline != StiPenStyle.None;
			}
			set
			{
			    LinesOfUnderline = value ? StiPenStyle.Solid : StiPenStyle.None;
			}
		}

        /// <summary>
		/// Gets or sets value indicates that no need show zeroes.
		/// </summary>
		[DefaultValue(false)]
		[StiSerializable]
		[StiCategory("TextAdditional")]
		[StiOrder(StiPropertyOrder.TextHideZeros)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[StiShowInContextMenu]
		[Description("Gets or sets value indicates that no need show zeroes.")]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual bool HideZeros { get; set; }

        [StiNonSerialized]
		[Browsable(false)]
		[Obsolete("Use ProcessingDuplicates property")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool MergeDuplicates
		{
			get
			{
				return ProcessingDuplicates == StiProcessingDuplicatesType.Merge;
			}
			set
			{
				ProcessingDuplicates = StiProcessingDuplicatesType.Merge;
			}
		}

        protected static object PropertyProcessingDuplicates = new object();
		/// <summary>
		/// Gets or sets value which indicates how report engine processes duplicated values.
		/// </summary>
		[DefaultValue(StiProcessingDuplicatesType.None)]
		[StiSerializable]
		[StiCategory("TextAdditional")]
		[StiOrder(StiPropertyOrder.TextProcessingDuplicates)]
		[TypeConverter(typeof(StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[Description("Gets or sets value which indicates how report engine processes duplicated values.")]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual StiProcessingDuplicatesType ProcessingDuplicates
		{
			get
			{
                return (StiProcessingDuplicatesType)Properties.Get(PropertyProcessingDuplicates, StiProcessingDuplicatesType.None);
			}
			set
			{
                Properties.Set(PropertyProcessingDuplicates, value, StiProcessingDuplicatesType.None); 
			}
		}

        protected static object PropertyMaxNumberOfLines = new object();
		/// <summary>
		/// Gets or sets maximum number of lines which specify the limit of the height stretch.
		/// </summary>
		[DefaultValue(0)]
		[StiSerializable]
		[StiCategory("TextAdditional")]
		[StiOrder(StiPropertyOrder.TextMaxNumberOfLines)]
		[Description("Gets or sets maximum number of lines which specify the limit of the height stretch.")]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual int MaxNumberOfLines
		{
			get
			{
                return Properties.GetInt(PropertyMaxNumberOfLines, 0);
			}
			set
			{
                Properties.SetInt(PropertyMaxNumberOfLines, value, 0); 
			}
		}

		public virtual string ProcessText(string text)
		{
            if (StiOptions.Engine.ForceNormalizeEndOfLineSymbols)
            {
                var newText = NormalizeEndOfLineSymbols(text);
                if (newText != null)
                    return newText;
            }
			return text;
		}
		#endregion
		
		#region IStiOnlyText
        protected static object PropertyOnlyText = new object();
		/// <summary>
		/// Gets or sets value indicates that the text expression contains a text only.
		/// </summary>
		[DefaultValue(false)]
		[StiSerializable]
		[StiCategory("TextAdditional")]
		[StiOrder(StiPropertyOrder.TextOnlyText)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[StiShowInContextMenu]
		[Description("Gets or sets value indicates that the text expression contains a text only.")]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual bool OnlyText
		{
			get
			{
                return Properties.GetBool(PropertyOnlyText, false);
			}
			set
			{
                Properties.SetBool(PropertyOnlyText, value, false); 
			}
		}
		#endregion

		#region IStiEditable
        /// <summary>
		/// Gets or sets value indicates that a component can be edited in the window of viewer.
		/// </summary>
		[StiSerializable]
		[DefaultValue(false)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[StiCategory("TextAdditional")]
		[StiOrder(StiPropertyOrder.TextEditable)]
		[Description("Gets or sets value indicates that a component can be edited in the window of viewer.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual bool Editable { get; set; } = false;

        /// <summary>
		/// Saves state of editable value.
		/// </summary>
		string IStiEditable.SaveState()
		{
            return this.GetTextInternal();
		}

		/// <summary>
		/// Restores state of editable value.
		/// </summary>
		void IStiEditable.RestoreState(string value)
		{
			this.SetTextInternal(value);
		}
		#endregion

		#region IStiProcessAtEnd
		/// <summary>
		/// Gets or sets value indicates that a text is processed at the end of the report execution.
		/// </summary>
		[StiSerializable]
		[DefaultValue(false)]
		[StiCategory("TextAdditional")]
		[StiOrder(StiPropertyOrder.TextProcessAtEnd)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets value indicates that a text is processed at the end of the report execution.")]
        [StiPropertyLevel(StiLevel.Professional)]
        [StiEngine(StiEngineVersion.EngineV1)]
		public virtual bool ProcessAtEnd
		{
			get
			{
                return ProcessAt == StiProcessAt.EndOfReport;
			}
			set
			{
                if (value)
                    ProcessAt = StiProcessAt.EndOfReport;
                else
                    ProcessAt = StiProcessAt.None;
			}
		}
		#endregion

        #region IStiProcessAt
        protected static object PropertyProcessAt = new object();
        /// <summary>
        /// Gets or sets a value indicates whether to process a text expression of this component at the end of the page rendering or at the end of the report rendering.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiProcessAt.None)]
        [StiCategory("TextAdditional")]
        [StiOrder(StiPropertyOrder.TextProcessAt)]
        [TypeConverter(typeof(StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[Description("Gets or sets a value indicates whether to process a text expression of this component at the end of the page rendering or at the end of the report rendering.")]
        [StiPropertyLevel(StiLevel.Professional)]
        [StiEngine(StiEngineVersion.EngineV2)]
        public virtual StiProcessAt ProcessAt
        {
            get
            {
                return (StiProcessAt)Properties.Get(PropertyProcessAt, StiProcessAt.None);
            }
            set
            {
                Properties.Set(PropertyProcessAt, value, StiProcessAt.None);
            }
        }
        #endregion

		#region Render override
		public virtual void InvokeRenderTo(StiSimpleText textBox)
		{
		}
		#endregion		

		#region StiComponent Override
		private string GetFields()
		{
            if (Text == null || GetTextInternal() == null)
                return string.Empty;

            var startIndex = GetTextInternal().LastIndexOf('.');
			if (startIndex == -1)
			    return string.Empty;

            var endIndex = GetTextInternal().LastIndexOf('}');
			if (endIndex == -1)
			    return string.Empty;

			if (endIndex - startIndex - 1 < 1)
			    return string.Empty;

            if (GetTextInternal().Length <= (startIndex + 1))
                return string.Empty;

            if (GetTextInternal().Length <= (endIndex - startIndex - 1))
                return string.Empty;

			if ((startIndex + 1) < 0)
			    return string.Empty;

			if ((endIndex - startIndex - 1) < 0)
			    return string.Empty;

            return GetTextInternal().Substring(startIndex + 1, endIndex - startIndex - 1);
		}


		public override string GetQuickInfo()
		{
			switch (Report.Info.QuickInfoType)
			{
				case StiQuickInfoType.ShowAliases:
					if (!string.IsNullOrEmpty(this.Alias))
					    return this.Alias;

                    return GetTextInternal();

				case StiQuickInfoType.ShowFields:
					string fields = GetFields();
					if (fields.Length == 0)
					    return Text;

					return fields;

				case StiQuickInfoType.ShowFieldsOnly:
					return GetFields();

				case StiQuickInfoType.ShowContent:
                    return GetTextInternal();

				default:
					return base.GetQuickInfo();
			}
		}

		/// <summary>
		/// Return events collection of this component.
		/// </summary>
		public override StiEventsCollection GetEvents()
		{
			var events = base.GetEvents();

		    if (GetValueEvent != null)
			    events.Add(GetValueEvent);

			return events;
		}
		#endregion

		#region Expressions
		#region Text
		private StiExpression text = new StiExpression();
		/// <summary>
		/// Gets or sets text expression.
		/// </summary>
		[StiCategory("Text")]
		[StiOrder(StiPropertyOrder.TextText)]
		[StiSerializable]
		[Description("Gets or sets text expression.")]
		[Editor("Stimulsoft.Report.Components.Design.StiTextExpressionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual StiExpression Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			    if (text != null)
			        text.ParentComponent = this;
			}
		}

        /// <summary>
        /// Internal use only.
        /// </summary>
        public virtual string GetTextInternal()
        {
            return this.Text.Value;
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        public virtual void SetTextInternal(string value)
        {
            this.Text.Value = value;
        }
	
		private string textValue;
		/// <summary>
		/// Gets or sets text value. If the text is not null thet it is necessary to use this text.
		/// </summary>
		[Browsable(false)]
		[Description("Gets or sets text value.")]
		public virtual string TextValue
		{
			get 
			{
				return textValue;
			}
			set 
			{
				if (textValue != value)
				{
					StiOptions.Engine.GlobalEvents.InvokeTextChanged(this, 
						new StiTextChangedEventArgs(textValue, value));
					textValue = value;
				}
			}
		}
		#endregion
		#endregion

		#region Events
		#region GetValue
		private static readonly object EventGetValue = new object();

		/// <summary>
		/// Occurs when the text is being prepared for rendering.
		/// </summary>
		public event StiGetValueEventHandler GetValue
		{
			add
			{
				Events.AddHandler(EventGetValue, value);
			}
			remove
			{
				Events.RemoveHandler(EventGetValue, value);
			}
		}

		/// <summary>
		/// Raises the GetValue event.
		/// </summary>
		protected virtual void OnGetValue(StiGetValueEventArgs e)
		{
		}
		
		/// <summary>
		/// Raises the GetValue event.
		/// </summary>
		public virtual void InvokeGetValue(StiComponent sender, StiGetValueEventArgs e)
		{
			try
			{
				OnGetValue(e);

                if (Report?.CalculationMode == StiCalculationMode.Interpretation)
                {
                    var stiText = sender as StiText;
                    var hasExcelValue = stiText != null && !string.IsNullOrEmpty(stiText.ExcelValue.Value);
					var processAtEndOfPage = stiText != null && stiText.ProcessAt == StiProcessAt.EndOfPage;
                    var parserParameters = new StiParserParameters { ExecuteIfStoreToPrint = processAtEndOfPage };
                    object parserResult = null;

                    var richText = sender as StiRichText;
                    if (richText != null)
                    {
                        if (richText.OnlyText)
                        {
                            parserResult = XmlConvert.DecodeName(richText.Text.Value).Replace((char)0, ' ');
                        }
                        else
                        {
                            if (Report.Engine != null)
                            {
                                var obj = Report.Engine.ParserConversionStore["*StiRichTextExpression*" + sender.Name];
                                if (obj != null)
                                    parserResult = StiParser.ParseTextValue("{" + (string) obj + "}", richText, parserParameters);
                            }
                        }
                    }
                    else
                    {
                        var textComp = this as StiText;
                        if (textComp != null && textComp.Type == StiSystemTextType.DataColumn && !string.IsNullOrEmpty(textComp.NullValue))
                        {
                            var expression = stiText.Text.Value;
                            if (expression.StartsWithInvariant("{") && expression.EndsWithInvariant("}") && expression.Length > 2)
                            {
                                expression = expression.Substring(1, expression.Length - 2);
                                parserResult = StiNullValuesHelper.IsNull(Report, expression)
                                    ? textComp.NullValue
                                    : StiParser.ParseTextValue(stiText.Text.Value, stiText, parserParameters);
                            }
                        }
                        else
                        {
                            parserResult = StiParser.ParseTextValue(stiText.Text.Value, stiText, parserParameters);
                        }
                    }

                    if (parserParameters.StoreToPrint && !processAtEndOfPage)
                    {
                        e.StoreToPrinted = true;
						string stt = stiText.Text.Value;
						if (stt == null || !stt.StartsWith("#%#")) parserResult = "#%#" + stt;
						hasExcelValue = true;	//fix
                    }

                    if (stiText != null && stiText.Format != null && stiText.Format != "G")
                    {
						var eValue = hasExcelValue ? parserResult : Report.CheckExcelValue(sender, parserResult);

						var customFormat = stiText.TextFormat as StiCustomFormatService;
						if ((customFormat != null) && !string.IsNullOrWhiteSpace(customFormat.StringFormat) && customFormat.StringFormat.Contains("{"))
						{
							var storeFormat = customFormat.StringFormat;
							try
							{
								customFormat.StringFormat = global::System.Convert.ToString(StiParser.ParseTextValue(storeFormat, stiText, parserParameters));
								e.Value = customFormat.Format(eValue);
							}
							finally
                            {
								customFormat.StringFormat = storeFormat;
							}
						}
						else
						{
							e.Value = stiText.TextFormat.Format(eValue);
						}
                    }
                    else
                    {
                        e.Value = Report.ToString(sender, parserResult, !hasExcelValue);
                    }
                }

                var handler = Events[EventGetValue] as StiGetValueEventHandler;
                handler?.Invoke(sender, e);

				StiBlocklyHelper.InvokeBlockly(this.Report, sender, GetValueEvent, e);

				StiOptions.Engine.GlobalEvents.InvokeGetValue(sender, e);
            }
			catch (Exception ex)
			{
				string atCustomFunction = StiCustomFunctionHelper.CheckExceptionForCustomFunction(ex, this.Report);
                var str = $"Expression in Text property of '{Name}' can't be evaluated! {atCustomFunction}{ex.Message}";
				StiLogService.Write(GetType(), str);
				Report.WriteToReportRenderingMessages(str);
			}
		}

        public void CheckDuplicates(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                var comp = sender as StiText;

                var processingDuplicates = this.ProcessingDuplicates;
                if (processingDuplicates == StiProcessingDuplicatesType.BasedOnValueAndTagMerge ||
                    processingDuplicates == StiProcessingDuplicatesType.BasedOnValueAndTagHide ||
                    processingDuplicates == StiProcessingDuplicatesType.BasedOnValueRemoveText)
                {
                    var tag = processingDuplicates == StiProcessingDuplicatesType.BasedOnValueAndTagMerge ||
                               processingDuplicates == StiProcessingDuplicatesType.BasedOnValueAndTagHide
                        ? global::System.Convert.ToString(comp.TagValue)
                        : null;

                    if (this.Report.Engine.CheckForDuplicate(this.Name, e.Value, tag))
                    {
                        e.Value = string.Empty;
                        var text = sender as StiText;
                        if (text != null && !string.IsNullOrWhiteSpace(text.Format) && text.Format != "G")
                            text.ExcelDataValue = null;
                    }
                }
                if (processingDuplicates == StiProcessingDuplicatesType.GlobalBasedOnValueAndTagMerge ||
                    processingDuplicates == StiProcessingDuplicatesType.GlobalBasedOnValueAndTagHide ||
                    processingDuplicates == StiProcessingDuplicatesType.GlobalBasedOnValueRemoveText)
                {
                    var tag = processingDuplicates == StiProcessingDuplicatesType.GlobalBasedOnValueAndTagMerge ||
                               processingDuplicates == StiProcessingDuplicatesType.GlobalBasedOnValueAndTagHide
                        ? global::System.Convert.ToString(comp.TagValue)
                        : null;

                    if (this.Report.Engine.CheckForDuplicate($"{Left}_{Width}", e.Value, tag))
                    {
                        e.Value = string.Empty;
                        var text = sender as StiText;
                        if (text != null && !string.IsNullOrWhiteSpace(text.Format) && text.Format != "G")
                            text.ExcelDataValue = null;
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Occurs when the text is being prepared for rendering.
        /// </summary>
        [StiSerializable]
		[StiCategory("ValueEvents")]
		[Browsable(false)]
		[Description("Occurs when the text is being prepared for rendering.")]
		public StiGetValueEvent GetValueEvent
		{
			get
			{				
				return new StiGetValueEvent(this);
			}
			set
			{
				if (value != null)
				    value.Set(this, value.Script);
			}
		}
		#endregion

		#region TextProcess
		private static readonly object EventTextProcess = new object();

		/// <summary>
		/// Occurs when the text for showing is prepared and its checking is being processed.
		/// </summary>
		public event StiValueEventHandler TextProcess
		{
			add
			{
				Events.AddHandler(EventTextProcess, value);
			}
			remove
			{
				Events.RemoveHandler(EventTextProcess, value);
			}
		}

		/// <summary>
		/// Raises the TextProcess event.
		/// </summary>
		protected virtual void OnTextProcess(StiValueEventArgs e)
		{
		}
		
		/// <summary>
		/// Raises the TextProcess event.
		/// </summary>
		public void InvokeTextProcess(StiComponent sender, StiValueEventArgs e)
		{
            try
            {
                OnTextProcess(e);

                var isCompilationMode = true;
                if (Report != null)
                    isCompilationMode = Report.CalculationMode == StiCalculationMode.Compilation;

                if (isCompilationMode)
                {
                    var handler = Events[EventTextProcess] as StiValueEventHandler;
                    handler?.Invoke(sender, e);
                }
                else
                {
                    var handler = Events[EventTextProcess] as StiValueEventHandler;
                    handler?.Invoke(sender, e);

                    #region Conditions
                    if (Report != null && Report.Engine != null)
                    {
                        var obj = Report.Engine.ParserConversionStore["*StiConditionExpression*" + this.Name];
                        if (obj != null)
                        {
                            Report.Engine.LastInvokeTextProcessValueEventArgsValue = e.Value;
                            ApplyConditionsAssignExpression(sender, obj as ArrayList);

                            if (this is StiCrossCell)
                                ApplyConditions(sender, obj as ArrayList, e);
                        }
                    }
					#endregion
				}
			}
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), $"{Name} InvokeTextProcess...ERROR");
                StiLogService.Write(this.GetType(), $"{Name} {ex.Message}");

                Report?.WriteToReportRenderingMessages($"{Name} {ex.Message}");
            }
		}

        private void ApplyConditionsAssignExpression(object sender, ArrayList conditions)
        {
            if (conditions == null) return;

            foreach (DictionaryEntry de in conditions)
            {
                var condition = de.Key as StiCondition;
                if (!condition.CanAssignExpression || condition.AssignExpression == null ||
                    condition.AssignExpression.Trim().Length <= 0) continue;

                var result = StiParser.ParseTextValue((string)de.Value, this, sender);
                if (!(result is bool) || !(bool) result)continue;

                var result2 = StiParser.ParseTextValue("{" + condition.AssignExpression + "}", this, sender);
                ((StiText)sender).TextValue = Report.ToString(sender, result2, true);
            }
        }
		#endregion
		#endregion		

		#region Properties
        /// <summary>
		/// Internal use only.
		/// </summary>
		[Browsable(false)]
		[StiSerializable(StiSerializeTypes.SerializeToDocument)]
		[DefaultValue(null)]
		public virtual string TotalValueHelp { get; set; }

        /// <summary>
		/// Internal use only.
		/// </summary>
  //      [Browsable(false)]
  //      public string ComponentGuid
		//{
		//	get
		//	{
		//		return TotalValueHelp;
		//	}
		//	set
		//	{
		//		TotalValueHelp = value;
		//	}
		//}
        #endregion

        #region Methods
        private static string NormalizeEndOfLineSymbols(string inputText)
        {
            if (inputText == null || inputText.Length < 2 || (inputText.IndexOf('\r') == -1 && inputText.IndexOf('\n') == -1))
                return null;

            var sb = new StringBuilder();
            for (var index = 0; index < inputText.Length; index++)
            {
                var ch = inputText[index];
                if (ch == '\r' || ch == '\n')
                {
                    if (index + 1 < inputText.Length)
                    {
                        var ch2 = inputText[index + 1];
                        if ((ch2 == '\r' || ch2 == '\n') && (ch2 != ch))
                            index++;
                    }
                    sb.Append("\r\n");
                }
                else
                {
                    sb.Append(ch);
                }
            }

            if (sb.Length == inputText.Length)
                return null;

            return sb.ToString();
        }
        #endregion

        /// <summary>
        /// Creates a new object of the type StiSimpleText.
        /// </summary>
        public StiSimpleText()
            : this(RectangleD.Empty, string.Empty)
		{
		}
		
		/// <summary>
		/// Creates a new component of the type StiSimpleText.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		public StiSimpleText(RectangleD rect) : this(rect, string.Empty)
		{
		}
		
		/// <summary>
		/// Creates a new component of the type StiSimpleText.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		/// <param name="text">Text expression</param>
		public StiSimpleText(RectangleD rect, string text) : base(rect)
		{
			SetTextInternal(text);

			PlaceOnToolbox = false;
		}
	}
}
