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

namespace Stimulsoft.Base.Serializing
{
	/// <summary>
	/// Class keep an object properties information.
	/// </summary>
	public class StiPropertyInfo 
	{
        #region Properties
        /// <summary>
        /// Gets or sets an object property that is the main for this object property.
        /// </summary>
        public StiPropertyInfo Parent { get; set; }

	    /// <summary>
		/// Gets or sets the name of an object property.
		/// </summary>
		public string Name { get; set; }

	    /// <summary>
		/// Gets or sets the value of an object property.
		/// </summary>
		public object Value { get; set; }

	    /// <summary>
		/// Gets or sets the value of an object property by default.
		/// </summary>
		public object DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the value which indicates that default value specified.
        /// </summary>
        public bool IsDefaultValueSpecified { get; set; }

	    /// <summary>
		/// Gets or sets value indicates that this property describes  an object.
		/// </summary>
		public bool IsKey { get; set; }

	    /// <summary>
		/// Gets or sets value indicates that this is a reference to an object.
		/// </summary>
		public bool IsReference { get; set; }

	    /// <summary>
		/// Gets or sets value indicates that this object realizes the interface IStiSerializable.
		/// </summary>
		public bool IsSerializable { get; set; }

	    /// <summary>
		/// Gets or sets value indicates that this is a collection.
		/// </summary>
		public bool IsList { get; set; }

	    /// <summary>
		/// Gets or sets the number of elements in the collection.
		/// </summary>
		public int Count { get; set; }

	    /// <summary>
		/// Gets or sets the reference code.
		/// </summary>
		public int ReferenceCode { get; set; }

	    public string TypeName { get; set; }

	    /// <summary>
		/// Gets an object type.
		/// </summary>
		public Type Type => Value != null ? Value.GetType() : typeof(object);
        
	    /// <summary>
		/// Gets or sets the collection of subordinated properties.
		/// </summary>
		public StiPropertyInfoCollection Properties { get; set; }
		#endregion

		#region Methods
		public override string ToString()
		{
			return Name;
		}
		#endregion

		/// <summary>
		/// Creates a new instance of the StiPropertyInfo class.
		/// </summary>
		/// <param name="name">Name of an object properties</param>
		/// <param name="value">Value of an object property.</param>
		/// <param name="defaultValue">Value of an object property by default.</param>
		/// <param name="isDefaultValueSpecified">Value indicates that default value specified.</param>
		/// <param name="isKey">Value indicates that this property describes an object.</param>
		/// <param name="isReference">Value indicates that this is a reference to an object.</param>
		/// <param name="isList">Value indicates that this is a collection.</param>
		public StiPropertyInfo(string name, object value, object defaultValue, bool isDefaultValueSpecified,
            bool isKey, bool isReference, bool isList, string typeName)
		{
			Properties = new StiPropertyInfoCollection(this);

			this.Name = name;
			this.Value = value;
			this.DefaultValue = defaultValue;
		    this.IsDefaultValueSpecified = isDefaultValueSpecified;
            this.IsKey = isKey;
			this.IsReference = isReference;
			this.IsList = isList;
			this.ReferenceCode = -1;
			this.TypeName = typeName;
		}

		/// <summary>
		/// Creates a new instance of the StiPropertyInfo class.
		/// </summary>
		/// <param name="name">Name of an object properties</param>
		/// <param name="value">Value of an object property.</param>
		/// <param name="isKey">Value indicates that this property describes an object.</param>
		/// <param name="isReference">Value indicates that this is a reference to an object.</param>
		/// <param name="isList">Value indicates that this is a collection.</param>
		public StiPropertyInfo(string name, object value, bool isKey, bool isReference, bool isList, string typeName) :
			this(name, value, null, false, isKey, isReference, isList, typeName)
		{
		}
	}
}
