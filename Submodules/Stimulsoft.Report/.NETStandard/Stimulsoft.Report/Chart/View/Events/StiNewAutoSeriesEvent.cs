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
using System.Reflection;
using System.Drawing.Design;

using Stimulsoft.Base;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.CodeDom;
using Stimulsoft.Report.Components;


namespace Stimulsoft.Report.Events
{
	/// <summary>
	/// Represents the method that handles the events which occurs when new auto series is created.
	/// </summary>
	public delegate void StiNewAutoSeriesEventHandler(object sender, StiNewAutoSeriesEventArgs e);

	/// <summary>
	/// Describes an argument for the event StiNewAutoSeries. 
	/// </summary>
	public class StiNewAutoSeriesEventArgs : EventArgs
	{
		private int seriesIndex;
		/// <summary>
		/// Gets or sets the index of series.
		/// </summary>
		public virtual int SeriesIndex
		{
			get 
			{
				return seriesIndex;
			}
			set 
			{
				seriesIndex = value;
			}
		}

		private object color;
		/// <summary>
		/// Gets or sets the series color.
		/// </summary>
		public virtual object Color
		{
			get 
			{
				return color;
			}
			set 
			{
				color = value;
			}
		}


        private IStiSeries series;
		/// <summary>
		/// Gets or sets the series.
		/// </summary>
        public virtual IStiSeries Series
		{
			get 
			{
				return series;
			}
			set 
			{
				series = value;
			}
		}

		/// <summary>
		/// Creates a new object of the type StiNewAutoSeriesEventArgs.
		/// </summary>
        public StiNewAutoSeriesEventArgs(int seriesIndex, IStiSeries series, object color)
		{
			this.seriesIndex = seriesIndex;
			this.series = series;
			this.color = color;
		}
	}


	/// <summary>
	/// Describes the class for realization of the event StiNewAutoSeriesEvent.
	/// </summary>
	[TypeConverter(typeof(Stimulsoft.Report.Events.Design.StiNewAutoSeriesEventConverter))]
	public class StiNewAutoSeriesEvent : StiEvent
	{
		/// <summary>
		/// Returns the string representation of the event.
		/// </summary>
		public override string ToString()
		{
			return "NewAutoSeries";
		}

		/// <summary>
		/// Returns an array of event parameters.
		/// </summary>
		/// <returns>Array of event parameters.</returns>
		public override StiParameterInfo[] GetParameters()
		{
			return new StiParameterInfo[]{
											 new StiParameterInfo(typeof(object), "sender"),
											 new StiParameterInfo(typeof(StiNewAutoSeriesEventArgs), "e")};
		}

		/// <summary>
		/// Return the type of the event.
		/// </summary>
		/// <returns>Event type.</returns>
		public override Type GetEventType()
		{
			return typeof(StiNewAutoSeriesEventHandler);
		}

		/// <summary>
		/// Creates a new object of the type StiNewAutoSeriesEvent.
		/// </summary>
		public StiNewAutoSeriesEvent() : this("")
		{
		}

		/// <summary>
		/// Creates a new object of the type StiNewAutoSeriesEvent with specified arguments.
		/// </summary>
		/// <param name="script">Script of the event.</param>
		public StiNewAutoSeriesEvent(string script) : base(script)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiEvent with specified arguments.
		/// </summary>
		/// <param name="parent">Component which contain this event.</param>
		public StiNewAutoSeriesEvent(StiComponent parent) : base(parent)
		{
		}
	}
}
