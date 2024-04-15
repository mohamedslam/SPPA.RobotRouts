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
using System.ComponentModel;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Engine;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base;
using Stimulsoft.Report.PropertyGrid;

namespace Stimulsoft.Report.CrossTab
{
	public abstract class StiCrossCell : StiCrossField
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiCrossCell
            jObject.AddPropertyJObject("GetCrossValueEvent", GetCrossValueEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Value", Value.SaveToJsonObject(mode));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "GetCrossValueEvent":
                        {
                            var crossValueEvent = new StiGetCrossValueEvent();
                            crossValueEvent.LoadFromJsonObject((JObject)property.Value);

                            this.GetCrossValueEvent = crossValueEvent;
                        }
                        break;

                    case "Value":
                        {
                            var crossValueExpression = new StiCrossValueExpression();
                            crossValueExpression.LoadFromJsonObject((JObject)property.Value);
                            this.Value = crossValueExpression;
                        }
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            var collection = new StiEventCollection();

            collection.Add(StiPropertyCategories.DataEvents, new[]
            {
                StiPropertyEventId.GetCrossValueEvent,
                StiPropertyEventId.ProcessCellEvent
            });
            
            collection.Add(StiPropertyCategories.ValueEvents, new[]
            {
                StiPropertyEventId.GetExcelValueEvent,
                StiPropertyEventId.GetValueEvent,
                StiPropertyEventId.GetToolTipEvent,
                StiPropertyEventId.GetTagEvent
            });

            collection.Add(StiPropertyCategories.NavigationEvents, new[]
            {
                StiPropertyEventId.GetHyperlinkEvent,
                StiPropertyEventId.GetBookmarkEvent
            });

            collection.Add(StiPropertyCategories.PrintEvents, new[]
            {
                StiPropertyEventId.BeforePrintEvent,
                StiPropertyEventId.AfterPrintEvent
            });

            collection.Add(StiPropertyCategories.MouseEvents, new[]
            {
                StiPropertyEventId.GetDrillDownReportEvent,
                StiPropertyEventId.ClickEvent,
                StiPropertyEventId.DoubleClickEvent,
                StiPropertyEventId.MouseEnterEvent,
                StiPropertyEventId.MouseLeaveEvent
            });

            return collection;
        }
        #endregion

        #region Methods.Paint
        public override void Paint(StiPaintEventArgs e)
		{
		    if (Parent is StiCrossTab)
		        this.SetTextInternal(CellText);

		    base.Paint(e);
		}
		#endregion

		#region Events
        #region GetCrossValue
        public event StiGetCrossValueEventHandler GetCrossValue;

		/// <summary>
		/// Raises the GetValue event for this component.
		/// </summary>
		protected virtual void OnGetCrossValue(StiGetCrossValueEventArgs e)
		{
		}
		
		public void InvokeGetCrossValue(StiGetCrossValueEventArgs e)
		{
            try
            {
                if (Report.CalculationMode == StiCalculationMode.Compilation)
                {
                    OnGetCrossValue(e);
                    this.GetCrossValue?.Invoke(this, e);
                }
                else
                {
                    OnGetCrossValue(e);

                    var parserResult = StiParser.ParseTextValue(this.Value, this);
                    if (parserResult != null)
                        e.Value = parserResult;

                    GetCrossValue?.Invoke(this, e);
                }
            }
            catch (Exception ee)
            {
                StiLogService.Write(this.GetType(), "Do GetCrossValue ... ERROR");
                StiLogService.Write(this.GetType(), ee);

                Report?.WriteToReportRenderingMessages($"{Name} {ee.Message}");
            }
        }

	    /// <summary>
		/// Gets or sets a script of the event GetValueEvent.
		/// </summary>
		[StiSerializable]
		[Browsable(false)]
		[StiCategory("Data")]
		[Description("Gets or sets a script of the event GetValueEvent.")]
		public StiGetCrossValueEvent GetCrossValueEvent { get; set; } = new StiGetCrossValueEvent();
	    #endregion
		#endregion

		#region Expressions
		#region Value
	    /// <summary>
		/// Gets or sets the expression that is used for calculation of a cell value.
		/// </summary>
		[Description("Gets or sets the expression that is used for calculation of a cell value.")]
		[StiSerializable(
			 StiSerializeTypes.SerializeToCode |
			 StiSerializeTypes.SerializeToDesigner |
			 StiSerializeTypes.SerializeToSaveLoad)]
		[StiCategory("Data")]
		public virtual StiCrossValueExpression Value { get; set; } = new StiCrossValueExpression();
	    #endregion
		#endregion        

        #region Expressions.Browsable(false)
        [Browsable(false)]
		public override StiExpression Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}
		#endregion
	}
}