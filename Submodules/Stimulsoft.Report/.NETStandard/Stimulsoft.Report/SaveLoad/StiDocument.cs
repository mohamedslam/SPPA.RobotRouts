#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	Stimulsoft.Report Library										}
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

using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Units;
using Stimulsoft.Report.Viewer;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.SaveLoad
{
    /// <summary>
    /// Describes the class that is used for for storing a rendered report.
    /// </summary>
    public class StiDocument
	{
        /// <summary>
        /// Gets or sets a report name.
        /// </summary>
        [StiSerializable]
		public string ReportName
		{
			get 
			{
				return Report.ReportName;
			}
			set 
			{
				Report.ReportName = value;
			}
		}

		/// <summary>
		/// Gets or sets a report alias.
		/// </summary>
		[StiSerializable]
		public string ReportAlias
		{
			get 
			{
				return Report.ReportAlias;
			}
			set 
			{
				Report.ReportAlias = value;
			}
		}

		/// <summary>
		/// Gets or sets a report author.
		/// </summary>
		[StiSerializable]
		public string ReportAuthor
		{
			get 
			{
				return Report.ReportAuthor;
			}
			set 
			{
				Report.ReportAuthor = value;
			}
		}

		/// <summary>
		/// Gets or sets version a report engine.
		/// </summary>
		[StiSerializable]
		public string ReportVersion
		{
			get 
			{
				return Report.ReportVersion;
			}
			set 
			{
				Report.ReportVersion = value;
			}
		}

		/// <summary>
		/// Gets or sets a report description.
		/// </summary>
		[StiSerializable]
		public string ReportDescription
		{
			get 
			{
				return Report.ReportDescription;
			}
			set 
			{
				Report.ReportDescription = value;
			}
		}

		/// <summary>
		/// Gets or sets the date of the report creation.
		/// </summary>
		[StiSerializable]
		public DateTime ReportCreated
		{
			get
			{
				return DateTime.Now;
			}
			set
			{
			}
		}

        /// <summary>
        /// Gets or sets a technology a report was rendered with.
        /// </summary>
        /// 
        [StiSerializable]
        [DefaultValue(StiRenderedWith.Unknown)]
        public StiRenderedWith RenderedWith
        {
            get
            {
                return Report.RenderedWith;
            }
            set
            {
				Report.RenderedWith = value;
            }
        }

		[StiSerializable(StiSerializationVisibility.Class)]
		public StiUnit Unit
		{
			get
			{
				return Report.Unit;
			}
			set
			{
				Report.Unit = value;
			}
		}

		/// <summary>
		/// Gets or sets default report culture
		/// </summary>
		[StiSerializable]
		[DefaultValue("")]
		public string Culture
		{
			get
			{
				return Report.Culture;
			}
			set
			{
				Report.Culture = value;
			}
		}

		/// <summary>
		/// Gets or sets controls which will be shown in the Viewer Window.
		/// </summary>	
		[StiSerializable]
        [DefaultValue((int)StiPreviewSettings.Default)]
        public int PreviewSettings
        {
            get
            {
                return Report.PreviewSettings;
            }
            set
            {
				Report.PreviewSettings = value;
            }
        }

        /// <summary>
        /// Gets collection of rendered report pages.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        public StiPagesCollection Pages => Report.RenderedPages;

        /// <summary>
        /// Gets collection of variables of report.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        public StiVariablesCollection Variables => Report.Dictionary.Variables;

        private StiResourcesCollection resources;
        /// <summary>
        /// Gets or sets collection of the report resources.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        public StiResourcesCollection Resources
        {
            get
            {
                if (resources == null)
                    resources = Report.Dictionary.Resources;
                
                return resources;
            }
            set
            {
                resources = value;
            }
        }

        /// <summary>
        /// Gets a collection which consists of report styles.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        public StiStylesCollection Styles => Report.Styles;

        [Browsable(false)]
        [StiSerializable]
        public string Guid
        {
            get
            {
                return Report.ReportGuid;
            }
            set
            {
				Report.ReportGuid = value;
            }
        }

		/// <summary>
		/// Gets or sets the root bookmark of a document.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.Class)]
		public StiBookmark Bookmark
		{
			get
			{
				return Report.Bookmark;
			}
            set
            {
				Report.Bookmark = value;
            }
		}

		/// <summary>
		/// Gets or sets the report which rendered pages are used for rendering of a document.
		/// </summary>
		public StiReport Report { get; set; }

		/// <summary>
		/// Creates a new object of the type StiDocument.
		/// </summary>
		/// <param name="report">Rendered report.</param>
		public StiDocument(StiReport report)
		{
			this.Report = report;
		}
	}
}
