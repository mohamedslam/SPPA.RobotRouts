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

using System;
using System.Linq;
using System.ComponentModel;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;
using Stimulsoft.Base;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using System.Drawing;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.Components.ShapeTypes
{
	/// <summary>
	/// The class describes the base type of the shape.
	/// </summary>
	[StiServiceBitmap(typeof(StiShapeTypeService), "Stimulsoft.Report.Bmp.ShapeTypes.ShapeType.bmp")]
	[StiServiceCategoryBitmap(typeof(StiShapeTypeService), "Stimulsoft.Report.Bmp.ShapeTypes.ShapeType.bmp")]
	[TypeConverter(typeof(Stimulsoft.Report.Components.ShapeTypes.Design.StiShapeTypeServiceConverter))]
	public abstract class StiShapeTypeService : 
        StiService,
        IStiPropertyGridObject,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {

        }

        internal static StiShapeTypeService CreateFromJsonObject(JObject jObject)
        {
            var ident = jObject.Properties().FirstOrDefault(x => x.Name == "Ident").Value.ToObject<string>();

            var service = StiOptions.Services.Shapes.FirstOrDefault(x => (x.ServiceEnabled && x.GetType().Name == ident));
            if (service == null)
                throw new Exception("Type is not found!");

            var shapeTypeService = service.CreateNew();
            shapeTypeService.LoadFromJsonObject(jObject);
            return shapeTypeService;
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
	    public virtual StiComponentId ComponentId => StiComponentId.StiShapeTypeService;

        [Browsable(false)]
        public string PropName => string.Empty;

        public virtual StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level) => null;

        public virtual StiEventCollection GetEvents(IStiPropertyGrid propertyGrid) => null;
        #endregion

		#region StiService override
		/// <summary>
		/// Gets a service category.
		/// </summary>
		[Browsable(false)]
		public sealed override string ServiceCategory => StiLocalization.Get("Services", "categoryShapes");

		/// <summary>
		/// Gets a service type.
		/// </summary>
		[Browsable(false)]
		public sealed override Type ServiceType => typeof(StiShapeTypeService);
        #endregion

        #region Methods
        /// <summary>
        /// Draws the shape with the specified parameters.
        /// </summary>
        /// <param name="context">Graphics for drawing.</param>
        /// <param name="shape">Component that invokes drawing.</param>
        /// <param name="rect">The rectangle that shows coordinates for drawing.</param>
        /// <param name="zoom">Zoom of drawing.</param>
        public virtual void Paint(object context, StiShape shape, RectangleF rect, float zoom)
        {
            var painter = StiShapeTypePainter.GetPainter(this.GetType(), context is Graphics ? StiGuiMode.Gdi : StiGuiMode.Wpf);
            painter.Paint(context, shape, this, rect, zoom);
        }

		public override string ToString() => ServiceName;
        #endregion

        #region Methods.virtual
        public virtual StiShapeTypeService CreateNew() => throw new NotImplementedException();
        #endregion
    }
}