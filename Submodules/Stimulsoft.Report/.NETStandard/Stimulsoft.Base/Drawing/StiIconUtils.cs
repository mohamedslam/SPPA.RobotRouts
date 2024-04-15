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
using System.Reflection;
using System.Linq;
using System.Drawing;

namespace Stimulsoft.Base.Drawing
{
	public class StiIconUtils
	{
		/// <summary>
		/// Gets the Icon object associated with Type.
		/// </summary>
		/// <param name="type">The type with which the Icon object is associated.</param>
		/// <param name="iconName">The name of the icon file to look for.</param>
		/// <returns>The Icon object.</returns>
		public static Icon GetIcon(Type type, string iconName)
		{
			return GetIcon(type.Module.Assembly, iconName);
		}

		/// <summary>
		/// Gets the Icon object placed in assembly.
		/// </summary>
		/// <param name="assemblyName">The name of assembly in which the Icon object is placed.</param>
		/// <param name="iconName">The name of the Icon file to look for.</param>
		/// <returns>The Icon object.</returns>
		public static Icon GetIcon(string assemblyName, string iconName)
		{
		    var assembly = AppDomain.CurrentDomain
		        .GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);

            if (assembly != null)
				return GetIcon(assembly, iconName);

			throw new Exception($"Can't find assembly '{assemblyName}'");
		}

		/// <summary>
		/// Gets the Icon object placed in assembly.
		/// </summary>
		/// <param name="cursorAssembly">Assembly in which the Icon object is placed.</param>
		/// <param name="iconName">The name of the Icon file to look for.</param>
		/// <returns>The Icon object.</returns>
		public static Icon GetIcon(Assembly cursorAssembly, string iconName)
		{
			var stream = cursorAssembly.GetManifestResourceStream(iconName);
		    if (stream != null)
		        return new Icon(stream);

		    throw new Exception($"Can't find icon '{iconName}' in resources");
		}

#if !STIDRAWING
		/// <summary>
		/// Converts Image to an Icon.
		/// </summary>
		public static Icon ToIcon(Image image)
        {
            var bitmap = image as Bitmap;
            if (bitmap == null)
                return null;

            var Hicon = bitmap.GetHicon();
            return Icon.FromHandle(Hicon);
        }


		/// <summary>
		/// Resizes icon to specified size.
		/// </summary>
		public static Icon ResizeIcon(Icon icon, int width, int height)
        {
			if (icon.Width == width && icon.Height == height)
				return icon;

			return ToIcon(StiImageUtils.ResizeImage(icon.ToBitmap(), width, height));
		}
#endif
	}
}
