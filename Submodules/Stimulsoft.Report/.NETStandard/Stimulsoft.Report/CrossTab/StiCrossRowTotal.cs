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
    public class StiCrossRowTotal : StiCrossTotal
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiCrossRowTotal;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var objHelper = new StiPropertyCollection();

            var propHelper = propertyGrid.PropertiesHelper;

            // TextCategory
            var list = new[]
            {
                propHelper.Text()
            };
            objHelper.Add(StiPropertyCategories.Text, list);

            // TextAdditionalCategory
            if (level == StiLevel.Basic)
            {
                list = new[]
                {
                    propHelper.TextBrush(),
                    propHelper.fAngle(),
                    propHelper.Font(),
                    propHelper.WordWrap()
                };
            }
            else if (level == StiLevel.Standard)
            {
                list = new[]
                {
                    propHelper.AllowHtmlTags(),
                    propHelper.TextBrush(),
                    propHelper.fAngle(),
                    propHelper.Font(),
                    propHelper.Margins(),
                    propHelper.WordWrap()
                };
            }
            else
            {
                list = new[]
                {
                    propHelper.AllowHtmlTags(),
                    propHelper.TextBrush(),
                    propHelper.fAngle(),
                    propHelper.Font(),
                    propHelper.Margins(),
                    propHelper.TextOptions(),
                    propHelper.WordWrap()
                };
            }
            objHelper.Add(StiPropertyCategories.TextAdditional, list);

            // PositionCategory
            if (level != StiLevel.Basic)
            {
                list = new[]
                {
                    propHelper.MinSize(),
                    propHelper.MaxSize()
                };

                objHelper.Add(StiPropertyCategories.Position, list);
            }

            // AppearanceCategory
            if (level == StiLevel.Basic)
            {
                list = new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle(),
                    propHelper.HorAlignment(),
                    propHelper.VertAlignment()
                };
            }
            else
            {
                list = new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle(),
                    propHelper.HorAlignment(),
                    propHelper.VertAlignment(),
                    propHelper.UseParentStyles()
                };
            }

            objHelper.Add(StiPropertyCategories.Appearance, list);

            // BehaviorCategory
            if (level == StiLevel.Basic)
            {
                list = new[]
                {
                    propHelper.MergeHeaders(),
                    propHelper.Enabled()
                };
            }
            else
            {
                list = new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.MergeHeaders(),
                    propHelper.Enabled()
                };
            }
            objHelper.Add(StiPropertyCategories.Behavior, list);

            return objHelper;
        }
        #endregion

        #region StiComponent override
        /// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName => StiLocalization.Get("Components", "StiCrossRowTotal");
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiCrossRowTotal();
        }
        #endregion

        public StiCrossRowTotal()
        {
            this.Text = "Total";
        }
    }
}