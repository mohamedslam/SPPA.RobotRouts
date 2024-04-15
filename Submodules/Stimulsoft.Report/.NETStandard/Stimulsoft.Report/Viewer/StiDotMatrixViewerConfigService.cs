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
using Stimulsoft.Base.Services;
using Stimulsoft.Report;
using Stimulsoft.Report.Design;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Design;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Viewer
{
	/// <summary>
	/// Class describes the configuration of the dot-matrix viewer window.
	/// </summary>
	public class StiDotMatrixViewerConfigService : StiService
	{
		#region StiService override
		/// <summary>
		/// Gets a service category.
		/// </summary>
		public sealed override string ServiceCategory
		{
			get
			{
				return StiLocalization.Get("Services", "categoryRender");
			}
		}

		/// <summary>
		/// Gets a service type.
		/// </summary>
		public sealed override Type ServiceType
		{
			get
			{
				return typeof(StiDotMatrixViewerConfigService);
			}
		}
		#endregion

		/// <summary>
		/// Gets or sets value indicates is enabled possibility of the saving or no.
		/// </summary>
		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? SaveEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowSaveButton;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowSaveButton = value;
			}
		}


		/// <summary>
		/// Gets or sets value indicates is enabled possibility of the opening or no.
		/// </summary>
		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? OpenEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowOpenButton;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowOpenButton = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicates is enabled possibility of the printing or no.
		/// </summary>
		[StiServiceParam]
        [DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? PrintEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowPrintButton;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowPrintButton = value;
			}
		}


		/// <summary>
		/// Gets or sets value indicates is enabled possibility of the closing or no.
		/// </summary>
		[StiServiceParam]
		[DefaultValue(null)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool? CloseEnabled
		{
			get
			{
				return StiOptions.Viewer.Windows.ShowCloseButton;
			}
			set
			{
				StiOptions.Viewer.Windows.ShowCloseButton = value;
			}
		}
	}
}
