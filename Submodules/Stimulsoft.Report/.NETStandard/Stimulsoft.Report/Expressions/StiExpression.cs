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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Events;
using System;
using System.ComponentModel;
using System.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes class that contains an expression.
    /// </summary>
    [Editor("Stimulsoft.Report.Components.Design.StiExpressionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
	[TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiExpressionConverter))]
	public class StiExpression :
        ICloneable, 
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            // StiExpression
            jObject.AddPropertyStringNullOrEmpty("Value", Value);

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Value":
                        this.Value = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region Properties
        private string val;
		/// <summary>
		/// Gets or sets value of the expression.
		/// </summary>
		[StiSerializable(
			 StiSerializeTypes.SerializeToDesigner | 
			 StiSerializeTypes.SerializeToSaveLoad | 
			 StiSerializeTypes.SerializeToDocument)]
		[StiCategory("Data")]
		public virtual string Value
		{
			get
			{
				return val;
			}
			set
			{
			    if (val == value) return;

			    if (ParentComponent != null && ParentComponent is StiText)
			        StiOptions.Engine.GlobalEvents.InvokeTextChanged(ParentComponent, new StiTextChangedEventArgs(val, value));

			    val = value;
			}
		}

		internal object ParentComponent { get; set; }

		/// <summary>
		/// Gets value, indicates that the value is to be a string.
		/// </summary>
		[Browsable(false)]
		public virtual bool FullConvert => true;
        
        [Browsable(false)]
		public virtual bool ApplyFormat => true;

        /// <summary>
		/// Gets value, indicates that it is necessary to add methods of getting the expression value to the event handler.
		/// </summary>
		[Browsable(false)]
		public virtual bool GenAddEvent => true;
        #endregion

		#region Methods
		/// <summary>
		/// Returns the event for processing of expression when genertion of the report script.
		/// </summary>
		/// <returns>Event for expression processing.</returns>
		public virtual StiEvent GetDefaultEvent()
		{
			return new StiGetValueEvent();
		}
		#endregion

		#region implicit
		public static implicit operator string(StiExpression exp) 
		{
			return exp?.val;
		}

		public static implicit operator StiExpression(string s) 
		{
			return new StiExpression(s);
		}

		public override string ToString()
		{
            return Value;
		}
		#endregion

		#region ICloneable
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public virtual object Clone()
		{
			return this.MemberwiseClone();
		}
		#endregion

		/// <summary>
		/// Creates a new expression.
		/// </summary>
		public StiExpression() : this(string.Empty)
		{
		}
		
		/// <summary>
		/// Creates a new expression.
		/// </summary>
        /// <param name="value">Gets or sets the expression value.</param>
		public StiExpression(string value)
		{
			this.val = value;
		}
    }
}
