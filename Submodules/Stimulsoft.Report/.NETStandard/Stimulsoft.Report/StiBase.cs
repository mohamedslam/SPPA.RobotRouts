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

using System.Drawing.Design;
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report
{
	/// <summary>
	/// The base class for report components.
	/// </summary>
	public abstract class StiBase :
		StiService,
		IStiName
	{
		#region IStiName
		private string name = "";
		/// <summary>
		/// Indicates the name which is used to identify the component.
		/// </summary>
		[StiCategory("Design")]
		[StiOrder(StiPropertyOrder.DesignName)]
		[StiSerializable]
		[ParenthesizePropertyName(true)]
		[Description("Indicates the name which is used to identify the component.")]
		[Editor("Stimulsoft.Report.Components.Design.StiSimpleTextEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = string.Intern(value);
			}
		}
        #endregion

        #region Properties
        /// <summary>
        /// Indicates the localized name of the component.
        /// </summary>
        [Browsable(false)]
        public virtual string LocalizedName => Name;

        /// <summary>
        /// Indicates the service name of the component.
        /// </summary>
        [Browsable(false)]
        public override string ServiceName => LocalizedName;

        /// <summary>
        /// Indicates the localized name of the component category.
        /// </summary>
        [Browsable(false)]
        public virtual string LocalizedCategory => Name;
        #endregion
    }
}
