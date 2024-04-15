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
using System.Drawing.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.CodeDom;
using Stimulsoft.Report.Components;
using Stimulsoft.Base.Json.Linq;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Events
{
	/// <summary>
	/// Describes the base class for realization of the Event.
	/// </summary>
	[Editor("Stimulsoft.Report.Events.Design.StiEventEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
	[TypeConverter(typeof(Stimulsoft.Report.Events.Design.StiEventConverter))]
	public abstract class StiEvent :
        ICloneable, 
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            if (string.IsNullOrEmpty(Script))
                return null;

            var jObject = new JObject();

            jObject.AddPropertyStringNullOrEmpty("Script", Script);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var prop in jObject.Properties())
            {
                if (prop.Name == "Script")
                    this.Script = prop.DeserializeString();
            }
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
		{
			return this.MemberwiseClone();
		}
		#endregion
		
		#region Properties
        [Browsable(false)]
		public string PropertyName => $"{ToString()}Event";

        private string script = string.Empty;
		/// <summary>
		/// Gets or sets Script of the event.
		/// </summary>
		[StiSerializable]
		public string Script
		{
			get
			{
			    return parent != null 
			        ? parent.Properties.Get(PropertyName, string.Empty) as string 
			        : script;
			}
			set
			{
				if (parent != null)
				    parent.Properties.Set(PropertyName, value, string.Empty);
				else
				    script = value;
			}
		}
		#endregion

		#region Fields
		private StiComponent parent;
		#endregion

		#region Methods
		public void Set(StiComponent parent, string value)
		{
			this.parent = parent;
			this.Script = value;
		}

		/// <summary>
		/// Returns an array of event parameters.
		/// </summary>
		/// <returns>Array of event parameters.</returns>
		public virtual StiParameterInfo[] GetParameters()
		{
			return new[]
			{
				new StiParameterInfo(typeof(object), "sender"),
				new StiParameterInfo(typeof(EventArgs), "e")
			};
		}

		/// <summary>
		/// Return the type of the event.
		/// </summary>
		/// <returns>Event type.</returns>
		public virtual Type GetEventType()
		{
			return typeof(EventHandler);
		}
		#endregion

		/// <summary>
		/// Creates a new object of the type StiEvent.
		/// </summary>
		public StiEvent() : this(string.Empty)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiEvent with specified arguments.
		/// </summary>
		/// <param name="script">Script of the event.</param>
		public StiEvent(string script)
		{
			this.script = script;
		}

		/// <summary>
		/// Creates a new object of the type StiEvent with specified arguments.
		/// </summary>
		/// <param name="parent">Component which contain this event.</param>
		public StiEvent(StiComponent parent)
		{
			this.parent = parent;
		}
	}
}
