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

using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiConversionContainerInPanelAction : StiAction
    {
        public override string Name => StiLocalizationExt.Get("CheckActions", "Convert");

        public override string Description => StiLocalizationExt.Get("CheckActions", "Convert");

        public override void Invoke(StiReport report, object element, string elementName)
        {
            base.Invoke(report, null, null);

            var container = element as StiContainer;
            if (container == null)return;

            var panel = new StiPanel();

            panel.Parent = container.Parent;
            panel.Page = container.Page;

            panel.SetPaintRectangle(container.GetPaintRectangle());
            panel.ClientRectangle = container.ClientRectangle;
            panel.MinSize = container.MinSize;
            panel.MaxSize = container.MaxSize;
            panel.Border = container.Border;
            panel.Brush = container.Brush;

            panel.Conditions = container.Conditions;
            panel.ComponentStyle = container.ComponentStyle;
            panel.UseParentStyles = container.UseParentStyles;

            panel.CanGrow = container.CanGrow;
            panel.CanShrink = container.CanShrink;
            panel.GrowToHeight = container.GrowToHeight;
            panel.DockStyle = container.DockStyle;
            panel.Enabled = container.Enabled;
            panel.Interaction = container.Interaction;
            panel.Printable = container.Printable;
            panel.PrintOn = container.PrintOn;
            panel.ShiftMode = container.ShiftMode;

            panel.Name = container.Name;
            panel.Alias = container.Alias;
            panel.Restrictions = container.Restrictions;
            panel.Locked = container.Locked;
            panel.Linked = container.Linked;

            foreach (StiComponent comp in container.Components)
            {
                comp.Parent = panel;
                panel.Components.Add(comp);
            }

            var parent = container.Parent;
            if (parent != null)
            {
                var index = parent.Components.IndexOf(container);
                if (index != -1)
                {
                    parent.Components.RemoveAt(index);
                    parent.Components.Insert(index, panel);
                }
            }

            var page = panel.Page;
            if (page != null)
            {
                foreach (StiComponent comp in page.GetComponents())
                {
                    var clone = comp as StiClone;
                    if (clone != null && clone.Container == container)
                        clone.Container = panel;
                }
            }
        }
    }
}