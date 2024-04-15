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
using Stimulsoft.Report.CrossTab.Core;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using System;
using Stimulsoft.Report.Components;
using Stimulsoft.Base.Design;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.CrossTab
{
	public class StiCrossColumn : StiCrossHeader
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiCrossColumn
            jObject.AddPropertyEnum("EnumeratorType", EnumeratorType, StiEnumeratorType.None);
            jObject.AddPropertyString("EnumeratorSeparator", EnumeratorSeparator, ".");

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "EnumeratorType":
                        this.EnumeratorType = property.DeserializeEnum<StiEnumeratorType>();
                        break;

                    case "EnumeratorSeparator":
                        this.EnumeratorSeparator = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiCrossColumn;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var collection = new StiPropertyCollection();
            var propHelper = propertyGrid.PropertiesHelper;

            collection.Add(StiPropertyCategories.Data, new[]
            {
                propHelper.DisplayValue(),
                propHelper.SortDirection(),
                propHelper.SortType(),
                propHelper.Value()
            });

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.TextAdditional, new[]
                {
                    propHelper.TextBrush(),
                    propHelper.fAngle(),
                    propHelper.Font(),
                    propHelper.TextFormat(),
                    propHelper.WordWrap()
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.TextAdditional, new[]
                {
                    propHelper.AllowHtmlTags(),
                    propHelper.TextBrush(),
                    propHelper.fAngle(),
                    propHelper.Font(),
                    propHelper.Margins(),
                    propHelper.TextFormat(),
                    propHelper.WordWrap()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.TextAdditional, new[]
                {
                    propHelper.AllowHtmlTags(),
                    propHelper.TextBrush(),
                    propHelper.fAngle(),
                    propHelper.Font(),
                    propHelper.Margins(),
                    propHelper.TextFormat(),
                    propHelper.TextOptions(),
                    propHelper.WordWrap()
                });
            }
            

            if (level != StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.MinSize(),
                    propHelper.MaxSize()
                });
            }

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle(),
                    propHelper.HorAlignment(),
                    propHelper.VertAlignment()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle(),
                    propHelper.HorAlignment(),
                    propHelper.VertAlignment(),
                    propHelper.UseParentStyles()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.EnumeratorSeparator(),
                    propHelper.EnumeratorType(),
                    propHelper.MergeHeaders(),
                    propHelper.PrintOnAllPages(),
                    propHelper.ShowTotal()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.EnumeratorSeparator(),
                    propHelper.EnumeratorType(),
                    propHelper.MergeHeaders(),
                    propHelper.PrintOnAllPages(),
                    propHelper.ShowTotal()
                });
            }

            return collection;
        }
        #endregion

        #region StiComponent override
        /// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName => StiLocalization.Get("Components", "StiCrossColumn");
        #endregion

        #region Properties
	    /// <summary>
        /// Gets or sets enumerator type.
        /// </summary>
        [DefaultValue(StiEnumeratorType.None)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets enumerator type.")]
        public StiEnumeratorType EnumeratorType { get; set; } = StiEnumeratorType.None;

	    /// <summary>
        /// Gets or sets enumerator separator.
        /// </summary>
        [DefaultValue(".")]
        [StiSerializable]
        [StiCategory("Behavior")]
        [Description("Gets or sets enumerator separator.")]
        public string EnumeratorSeparator { get; set; } = ".";
	    #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiCrossColumn();
        }
        #endregion
    }
}