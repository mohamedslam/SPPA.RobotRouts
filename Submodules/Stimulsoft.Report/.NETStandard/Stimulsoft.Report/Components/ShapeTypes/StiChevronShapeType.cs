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

using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using System;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Design;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components.ShapeTypes
{
    /// <summary>
    /// The class describes the shape type - Chevron.
    /// </summary>
    [StiServiceBitmap(typeof(StiShapeTypeService), "Stimulsoft.Report.Bmp.ShapeTypes.Chevron.png")]
    [TypeConverter(typeof(Stimulsoft.Report.Components.ShapeTypes.Design.StiChevronShapeTypeConverter))]
    [StiGdiShapeTypePainter(typeof(Stimulsoft.Report.Painters.StiChevronGdiShapeTypePainter))]
    [StiWpfShapeTypePainter("Stimulsoft.Report.Painters.StiChevronWpfShapeTypePainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    public class StiChevronShapeType : StiShapeTypeService
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiChevronShapeType
            jObject.AddPropertyEnum("Direction", Direction, StiShapeDirection.Right);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Direction":
                        this.Direction = property.DeserializeEnum<StiShapeDirection>();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiChevronShapeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.Direction()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }
        #endregion

        #region Properties.override
        /// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => StiLocalization.Get("Shapes", "Chevron");
        #endregion

        #region Properties
		/// <summary>
		/// Gets or sets the arrow direction.
		/// </summary>
		[StiCategory("Behavior")]
        [StiOrder(100)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public virtual StiShapeDirection Direction { get; set; } = StiShapeDirection.Right;
        #endregion

        #region Methods.override
        public override StiShapeTypeService CreateNew() => new StiChevronShapeType();
        #endregion

        /// <summary>
        /// Creates a new Chevron.
		/// </summary>
        public StiChevronShapeType() : this(StiShapeDirection.Right)
		{
		}

		/// <summary>
        /// Creates a new Chevron with specified arguments.
		/// </summary>
		/// <param name="direction">Arrow direction.</param>
        public StiChevronShapeType(StiShapeDirection direction)
		{
			this.Direction = direction;
		}
    }
}