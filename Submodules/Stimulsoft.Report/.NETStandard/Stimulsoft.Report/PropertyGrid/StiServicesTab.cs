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
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using System.Drawing;
using Stimulsoft.Report.Images;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms.Design;
#else
using System.Windows.Forms.Design;
#endif

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Base.Design
{
	/// <summary>
	/// The class describes the panel of events for StiPropertyGrid.
	/// </summary>
	public class StiServicesTab : PropertyTab
	{
		public override bool CanExtend(object extendee)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(
			ITypeDescriptorContext context, object component, Attribute[] attributes)
		{
			return GetProperties(component, attributes);
		}

		public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
		{
			var props = TypeDescriptor.GetProperties(component);
			var properties = new PropertyDescriptorCollection(null);

			foreach (PropertyDescriptor prop in props)
			{
				if (!StiPropertyGrid.IsAllowedProperty(component.GetType(), prop.Name))continue;

			    if (prop.Attributes[typeof(StiServiceParamAttribute)] != null)
			        properties.Add(new StiPropertyDescriptor(prop));
			}

			return properties;
		}
		

		public override Bitmap Bitmap => StiReportImages.PropertyGrid.Events() as Bitmap;

	    public override string TabName => StiLocalization.Get("Report", "PropertiesTab");
	}
}
