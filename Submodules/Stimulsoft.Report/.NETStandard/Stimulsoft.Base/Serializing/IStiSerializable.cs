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
using System.Xml;

namespace Stimulsoft.Base.Serializing
{
	/// <summary>
	/// Describes an interface that realizes the capability of special serialization for an object.
	/// </summary>
	public interface IStiSerializable
	{
		/// <summary>
		/// Serializes object in XmlTextWriter.
		/// </summary>
		/// <param name="converter">Conveter for transforming objects in strings.</param>
		/// <param name="tw">XmlTextWriter for serialization.</param>
		void Serialize(StiObjectStringConverter converter, XmlTextWriter tw);

		/// <summary>
		/// Deserializes object in XmlTextReader.
		/// </summary>
		/// <param name="converter">Conveter for tansforming strings into objects.</param>
		/// <param name="tr">XmlTextWriter for deserialization.</param>
		void Deserialize(StiObjectStringConverter converter, XmlTextReader tr);
	}
}
