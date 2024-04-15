#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Json;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Base.Services
{
	/// <summary>
	/// Describes an asbtract class that serves for services realization.
	/// </summary>
	[StiServiceBitmap(typeof(StiService), "Stimulsoft.Base.Bmp.Service.bmp")]
	[StiServiceCategoryBitmap(typeof(StiService), "Stimulsoft.Base.Bmp.Service.bmp")]
	public abstract class StiService : ICloneable
	{
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

        #region Methods
        public bool IsPropertyPresent(object key)
        {
            return properties != null && properties.IsPresent(key);
        }

	    /// <summary>
	    /// Internal use only, for LoadDocument optimization.
	    /// </summary>
	    /// <returns>Returns true if Properties property is initialized.</returns>
	    protected bool IsPropertiesInitializedProtected()
	    {
	        return properties != null;
	    }

	    public virtual void PackService()
	    {
	    }
        #endregion

        #region Properties
        private StiRepositoryItems properties;
        [Browsable(false)]
        [JsonIgnore]
        public StiRepositoryItems Properties
        {
            get
            {
                return properties ?? (properties = new StiRepositoryItems());
            }
            set
            {
                properties = value;
            }
        }

		/// <summary>
		/// Gets a service category.
		/// </summary>
		[Browsable(false)]
		[JsonIgnore]
        public virtual string ServiceCategory => "Misc";

        /// <summary>
        /// Gets a service name.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public virtual string ServiceName => GetType().Name;

        /// <summary>
        /// Gets a service description.
        /// </summary>
        [Browsable(false)]
		[JsonIgnore]
        public virtual string ServiceInfo => string.Empty;

	    /// <summary>
		/// Gets a service type.
		/// </summary>
		[Browsable(false)]
		[JsonIgnore]
        public abstract Type ServiceType { get; }

		/// <summary>
		/// Gets or sets the value whether a service is enabled or not. 
		/// </summary>
		[StiServiceParam]
		[Browsable(false)]
		[DefaultValue(true)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public bool ServiceEnabled
		{
			get
			{
				return Properties.GetBool("ServiceEnabled", true);
			}
			set
			{
				Properties.SetBool("ServiceEnabled", value, true);
			}
		}
        #endregion
    }
}