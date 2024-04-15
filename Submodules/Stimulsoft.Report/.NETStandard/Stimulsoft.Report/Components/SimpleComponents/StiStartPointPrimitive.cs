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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.PropertyGrid;

namespace Stimulsoft.Report.Components
{
    [StiToolbox(false)]
	public class StiStartPointPrimitive : StiPointPrimitive
	{
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiStartPointPrimitive;

		public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level) => null;

		public override StiEventCollection GetEvents(IStiPropertyGrid propertyGrid) => null;
        #endregion

		#region StiComponent override
		/// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName => "Start Point Primitive";
	    #endregion

        #region Methods
        public override StiComponent CreateNew()
        {
            return new StiStartPointPrimitive();
        }
        #endregion

		/// <summary>
		/// Creates a new StiStartPointPrimitive.
		/// </summary>
		public StiStartPointPrimitive() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new StiStartPointPrimitive.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		public StiStartPointPrimitive(RectangleD rect): base(rect)
		{			
		}
	}
}