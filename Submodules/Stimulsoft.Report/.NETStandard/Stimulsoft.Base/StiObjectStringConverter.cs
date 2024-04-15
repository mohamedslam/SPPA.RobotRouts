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
using System.Drawing;
using System.Reflection;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Base
{
	/// <summary>
	/// Helps a converts object to string and a converts string to object.
	/// </summary>
	public class StiObjectStringConverter
	{
		public virtual void SetProperty(PropertyInfo p, object parentObject, object obj)
		{
		}

		/// <summary>
		/// Converts object into the string.
		/// </summary>
		/// <param name="obj">Object for convertation.</param>
		/// <returns>String that represents object.</returns>
		public virtual string ObjectToString(object obj) 
		{
			if (obj is string strObject) 
				return strObject;

			if (obj is byte[])
			{
				return Convert.ToBase64String(obj as byte[],
					StiBaseOptions.AllowInsertLineBreaksWhenSavingByteArray 
						? Base64FormattingOptions.InsertLineBreaks 
						: Base64FormattingOptions.None);
			}

			if (obj is Type)
				return obj.ToString();

			return GetConverter(obj.GetType()).ConvertToString(obj);			
		}
        
		/// <summary>
		/// Convertes string into object.
		/// </summary>
		/// <param name="str">String that represents object.</param>
		/// <param name="type">Object type.</param>
		/// <returns>Converted object.</returns>
		public virtual object StringToObject(string str, Type type) 
		{
			if (type == typeof(string))
			    return str;

		    if (type == typeof(byte[]))
		        return Convert.FromBase64String(str);

		    if (type == typeof(decimal))
		        return decimal.Parse(str);

		    if (type == typeof(Type))
		    {
		        var tp = StiTypeFinder.GetType(str);
		        if (tp != null) 
					return tp;

		        var assemblys = AppDomain.CurrentDomain.GetAssemblies();
		        foreach (var assembly in assemblys)
		        {
		            tp = assembly.GetType(str);
		            if (tp != null) 
						return tp;
		        }

		        if (true)
		            throw new TypeLoadException($"Type \"{str}\" not found");
		    }

		    if (type == typeof(object))
				return str;

			return GetConverter(type).ConvertFromString(str);
		}

        private TypeConverter GetConverter(Type type)
        {
            return TypeDescriptor.GetConverter(type);
        }
    }
}
