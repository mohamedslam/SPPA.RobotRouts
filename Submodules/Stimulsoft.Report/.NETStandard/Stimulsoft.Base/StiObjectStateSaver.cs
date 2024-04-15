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

using System.IO;
using System.Text;
using System.Globalization;
using Stimulsoft.Base.Serializing;
using System.Threading;

namespace Stimulsoft.Base
{
	/// <summary>
	/// This class helps with searialing/deserializing state of object to string.
	/// </summary>
	public class StiObjectStateSaver 
	{
		/// <summary>
		/// Write object state to string.
		/// </summary>
		/// <param name="obj">Object which state will be save to string.</param>
		/// <returns>String which contains string representation of object.</returns>
		public static string WriteObjectStateToString(object obj)
		{
			return WriteObjectStateToString(obj, new StiObjectStringConverter());
		}

		/// <summary>
		/// Object which state will be save to string.
		/// </summary>
		/// <param name="obj">Object which state will be save to string.</param>
		/// <param name="converter">Object converter which used for writing to string.</param>
		/// <returns>String which contain string representation of object.</returns>
		public static string WriteObjectStateToString(object obj, StiObjectStringConverter converter)
		{
			var currentCulture = Thread.CurrentThread.CurrentCulture;

			try
			{
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

				var sb = new StringBuilder();
			    var sr = new StiSerializing(converter);
			    using (var writer = new StringWriter(sb))
				{
				    sr.SortProperties = false;
					sr.CheckSerializable = true;
					sr.Serialize(obj, writer, "State", StiSerializeTypes.SerializeToAll);
				}
				return sb.ToString();
			}
			finally
			{
                Thread.CurrentThread.CurrentCulture = currentCulture;
			}
		}


		/// <summary>
		/// Read object state from string.
		/// </summary>
		/// <param name="obj">Object for storing state.</param>
		/// <param name="str">String which contain state of object.</param>
		public static void ReadObjectStateFromString(object obj, string str)
		{
			ReadObjectStateFromString(obj, str, new StiObjectStringConverter());
		}
		
		/// <summary>
		/// Read object state from string.
		/// </summary>
		/// <param name="obj">Object for storing state.</param>
		/// <param name="str">String which contain state of object.</param>
		/// <param name="converter">Object converter which used for writing to string.</param>
		public static void ReadObjectStateFromString(object obj, string str, StiObjectStringConverter converter)
		{
			var currentCulture = Thread.CurrentThread.CurrentCulture;

			try
			{
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

				using (var reader = new StringReader(str))
				{				
					var sr = new StiSerializing(converter);
					sr.Deserialize(obj, reader, "State");
				}
			}
			finally
			{
                Thread.CurrentThread.CurrentCulture = currentCulture;
			}
		}
	}
}
