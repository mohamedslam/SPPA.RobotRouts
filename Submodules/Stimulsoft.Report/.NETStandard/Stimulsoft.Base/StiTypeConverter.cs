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
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Base
{
    public static class StiTypeConverter
    {
        public static string ToString(Type type)
        {
            if (type == null) return "null";

            if (type == typeof(byte[])) return "byte[]";
            if (type == typeof(Image)) return "image";
            if (type == typeof(string)) return "string";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(char)) return "char";
            if (type == typeof(Guid)) return "guid";

            if (type == typeof(DateTime)) return "datetime";
            if (type == typeof(DateTimeOffset)) return "datetime offset";
            if (type == typeof(TimeSpan)) return "timespan";

            if (type == typeof(float)) return "float";
            if (type == typeof(double)) return "double";
            if (type == typeof(decimal)) return "decimal";
            if (type == typeof(sbyte)) return "sbyte";
            if (type == typeof(short)) return "short";
            if (type == typeof(int)) return "int";
            if (type == typeof(long)) return "long";
            if (type == typeof(byte)) return "byte";
            if (type == typeof(ushort)) return "ushort";
            if (type == typeof(uint)) return "uint";
            if (type == typeof(ulong)) return "ulong";
            
            if (type == typeof(bool?)) return "bool (Nullable)";
            if (type == typeof(char?)) return "char (Nullable)";
            if (type == typeof(Guid?)) return "guid (Nullable)";

            if (type == typeof(DateTime?)) return "datetime (Nullable)";
            if (type == typeof(DateTimeOffset?)) return "datetime offset (Nullable)";
            if (type == typeof(TimeSpan?)) return "timespan (Nullable)";

            if (type == typeof(float?)) return "float (Nullable)";
            if (type == typeof(double?)) return "double (Nullable)";
            if (type == typeof(decimal?)) return "decimal (Nullable)";
            if (type == typeof(sbyte?)) return "sbyte (Nullable)";
            if (type == typeof(short?)) return "short (Nullable)";
            if (type == typeof(int?)) return "int (Nullable)";
            if (type == typeof(long?)) return "long (Nullable)";
            if (type == typeof(byte?)) return "byte (Nullable)";
            if (type == typeof(ushort?)) return "ushort (Nullable)";
            if (type == typeof(uint?)) return "uint (Nullable)";
            if (type == typeof(ulong?)) return "ulong (Nullable)";

            if (type == typeof(object)) return "object";

            return type.ToString();
        }

        public static Type FromString(string type)
        {
            if (type == "null") return null;

            if (type == "byte[]") return typeof(byte[]);
            if (type == "image") return typeof(Image);
            if (type == "string") return typeof(string);
            if (type == "bool") return typeof(bool);
            if (type == "char") return typeof(char);
            if (type == "guid") return typeof(Guid);

            if (type == "datetime") return typeof(DateTime);
            if (type == "datetime offset") return typeof(DateTimeOffset);
            if (type == "timespan") return typeof(TimeSpan);

            if (type == "float") return typeof(float);
            if (type == "double") return typeof(double);
            if (type == "decimal") return typeof(decimal);
            if (type == "sbyte") return typeof(sbyte);
            if (type == "short") return typeof(short);
            if (type == "int") return typeof(int);
            if (type == "long") return typeof(long);
            if (type == "byte") return typeof(byte);
            if (type == "ushort") return typeof(ushort);
            if (type == "uint") return typeof(uint);
            if (type == "ulong") return typeof(ulong);

            if (type == "bool (Nullable)") return typeof(bool?);
            if (type == "char (Nullable)") return typeof(char?);
            if (type == "guid (Nullable)") return typeof(Guid?);

            if (type == "datetime (Nullable)") return typeof(DateTime?);
            if (type == "datetime offset (Nullable)") return typeof(DateTimeOffset?);
            if (type == "timespan (Nullable)") return typeof(TimeSpan?);
            
            if (type == "float (Nullable)") return typeof(float?);
            if (type == "double (Nullable)") return typeof(double?);
            if (type == "decimal (Nullable)") return typeof(decimal?);
            if (type == "sbyte (Nullable)") return typeof(sbyte?);
            if (type == "short (Nullable)") return typeof(short?);
            if (type == "int (Nullable)") return typeof(int?);
            if (type == "long (Nullable)") return typeof(long?);
            if (type == "byte (Nullable)") return typeof(byte?);
            if (type == "ushort (Nullable)") return typeof(ushort?);
            if (type == "uint (Nullable)") return typeof(uint?);
            if (type == "ulong (Nullable)") return typeof(ulong?);

            if (type == "object") return typeof(object);

            return StiTypeFinder.GetType(type);
        }
    }
}