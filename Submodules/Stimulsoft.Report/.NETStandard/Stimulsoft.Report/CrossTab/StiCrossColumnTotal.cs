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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.PropertyGrid;

namespace Stimulsoft.Report.CrossTab
{
    /// <summary>
    /// Summary description for StiCrossRowTotal.
    /// </summary>
    public class StiCrossColumnTotal : StiCrossTotal
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiCrossColumnTotal;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var collection = new StiPropertyCollection();
            var propHelper = propertyGrid.PropertiesHelper;

            collection.Add(StiPropertyCategories.Text, new[]
            {
                propHelper.Text()
            });

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.TextAdditional, new[]
                {
                    propHelper.TextBrush(),
                    propHelper.fAngle(),
                    propHelper.Font(),
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
                    propHelper.MergeHeaders(),
                    propHelper.Enabled()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.MergeHeaders(),
                    propHelper.Enabled()
                });
            }

            return collection;
        }
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiCrossColumnTotal");
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiCrossColumnTotal();
        }
        #endregion

        public StiCrossColumnTotal()
        {
            this.Text = "Total";
        }
    }
}