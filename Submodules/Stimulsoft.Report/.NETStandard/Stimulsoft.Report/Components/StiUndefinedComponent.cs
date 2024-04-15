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

using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using System.Collections.Generic;
using Stimulsoft.Base.Json.Linq;
using System;

namespace Stimulsoft.Report.Components
{
	[StiToolbox(false)]
    [StiGdiPainter(typeof(Stimulsoft.Report.Painters.StiUndefinedComponentGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiUndefinedComponentWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	public class StiUndefinedComponent : StiComponent 
	{
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiUndefinedComponent;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            return null;
        }

        public override StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }
        #endregion

		#region StiComponent override
		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition
		{
			get
			{
				return (int)StiComponentToolboxPosition.Text;
			}
		}

        public override StiToolboxCategory ToolboxCategory
        {
            get
            {
                return StiToolboxCategory.Components;
            }
        }

		
		/// <summary>
		/// Gets a localized name of the component category.
		/// </summary>
		public override string LocalizedCategory
		{
			get 
			{
				return StiLocalization.Get("Report", "Components");
			}
		}

		
		/// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName
		{
			get 
			{
				return "Undefined Component";
			}
		}
		#endregion

		#region this
		/// <summary>
		/// Creates a new StiText.
		/// </summary>
		public StiUndefinedComponent() : this(RectangleD.Empty)
		{
		}


		/// <summary>
		/// Creates a new StiText.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		public StiUndefinedComponent(RectangleD rect) : base(rect)
		{
			PlaceOnToolbox = true;
		}
		#endregion
	}
}