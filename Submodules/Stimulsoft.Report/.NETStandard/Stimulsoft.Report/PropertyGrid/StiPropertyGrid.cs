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
using System.Collections;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
using Stimulsoft.System.Windows.Forms.PropertyGridInternal;
#else
using System.Windows.Forms;
using System.Windows.Forms.PropertyGridInternal;
#endif

#if STIDRAWING
using FontFamily = Stimulsoft.Drawing.FontFamily;
using Font = Stimulsoft.Drawing.Font;

#endif

namespace Stimulsoft.Base.Design
{
    /// <summary>
    /// Describes the control StiPropertyGrid.
    /// </summary>
    [ToolboxItem(false)]
    public class StiPropertyGrid :
        PropertyGrid,
        IStiVisualThemeControl
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Color LineColor
        {
            get
            {
                return base.LineColor;
            }
            set
            {
                base.LineColor = value;
            }
        }

        #region IStiVisualThemeControl
        public void RefreshThemeColors()
        {
            this.CategoryForeColor = StiUX.PropertyGridForeground;
            this.CategorySplitterColor = StiUX.PropertyGridSeparator;

            this.LineColor = StiUX.PropertyGridSeparator;

            this.ViewBackColor = StiUX.PropertyGridViewBackground;
            this.ViewForeColor = StiUX.PropertyGridViewForeground;
            this.ViewBorderColor = StiUX.PropertyGridSeparator;

            this.ForeColor = StiUX.PropertyGridForeground;
            this.BackColor = StiUX.PropertyGridBackground;

            this.HelpBackColor = StiUX.PropertyGridBackground;
            this.HelpForeColor = StiUX.PropertyGridForeground;

            this.HelpBorderColor = StiUX.PropertyGridSeparator;

            this.SelectedItemWithFocusBackColor = StiUX.PropertyGridBackgroundSelected;
            this.SelectedItemWithFocusForeColor = StiUX.PropertyGridForegroundSelected;
        }
        #endregion

        #region Properties
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolStrip ToolStrip
        {
            get
            {
#if NETCOREAPP
                return Controls.Cast<Control>().FirstOrDefault(c => c.GetType().Name.EndsWith("ToolStrip")) as ToolStrip;
#else
                return Controls.Cast<Control>().FirstOrDefault(c => c.GetType().Name == "ToolStrip") as ToolStrip;
#endif
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolStripItem ItemPropertiesTab => ToolStrip?.Items.Cast<ToolStripItem>()
            .FirstOrDefault(i => i?.AccessibilityObject?.Name == "Properties Tab" || i?.AccessibilityObject?.Name == Loc.Get("Report", "PropertiesTab"));

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolStripItem ItemEventsTab => ToolStrip?.Items.Cast<ToolStripItem>()
            .FirstOrDefault(i => i?.AccessibilityObject?.Name == "Events Tab" || i?.AccessibilityObject?.Name == Loc.Get("Report", "EventsTab"));

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ScrollOffset
        {
            get
            {
                var field = typeof(PropertyGrid).GetField("gridView", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                    return 0;

                var obj = field.GetValue(this);
                if (obj == null)
                    return 0;

                var method = obj.GetType().GetMethod("GetScrollOffset", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance);
                if (method == null)
                    return 0;

                var value = method.Invoke(obj, new object[0]);
                return value is int ? (int)value : 0;
            }
            set
            {
                var field = typeof(PropertyGrid).GetField("gridView", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null) return;

                var obj = field.GetValue(this);
                if (obj == null) return;

                var method = obj.GetType().GetMethod("SetScrollOffset", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance);
                method?.Invoke(obj, new object[] { value });
            }
        }

        public override bool HelpVisible
        {
            get
            {
                return StiPropertyGridOptions.ShowDescription;
            }
            set
            {
                base.HelpVisible = StiPropertyGridOptions.ShowDescription;
            }
        }
        #endregion

        #region Methods
        private static string GetPropertyKey(Type type, string propertyName)
        {
            return GetPropertyKey(type.ToString(), propertyName);
        }

        private static string GetPropertyKey(string type, string propertyName)
        {
            return $"{type}::{propertyName}";
        }

        public static void HideProperty(Type type, string propertyName)
        {
            HideProperty(type.ToString(), propertyName);
        }

        public static void HideProperty(string type, string propertyName)
        {
            var str = GetPropertyKey(type, propertyName);
            hidedProperties[str] = str;
        }

        public static void ShowProperty(Type type, string propertyName)
        {
            ShowProperty(type.ToString(), propertyName);
        }

        public static void ShowProperty(string type, string propertyName)
        {
            var str = GetPropertyKey(type, propertyName);
            if (hidedProperties[str] != null)
                hidedProperties.Remove(str);
        }

        public static bool IsAllowedProperty(Type type, string propertyName)
        {
            if (type == typeof(Font))
                return !(propertyName == "GdiCharSet" || propertyName == "GdiVerticalFont" || propertyName == "Unit");

            var str1 = GetPropertyKey(type, propertyName);
            var str2 = GetPropertyKey("All", propertyName);

            return hidedProperties[str1] == null && hidedProperties[str2] == null;
        }

        public static bool IsAllowedProperty(string propertyName)
        {
            var str = GetPropertyKey("All", propertyName);

            return hidedProperties[str] == null;
        }

        public virtual void SetStandardPropertyTabs()
        {
            PropertyTabs.RemoveTabType(typeof(PropertiesTab));
            PropertyTabs.AddTabType(typeof(StiPropertiesTab), PropertyTabScope.Document);

            SetPropertyGrid();
        }

        public virtual void SetServicesPropertyTabs()
        {
            PropertyTabs.RemoveTabType(typeof(PropertiesTab));
            PropertyTabs.AddTabType(typeof(StiServicesTab), PropertyTabScope.Document);

            SetPropertyGrid();
        }

        private void SetPropertyGrid()
        {
            foreach (object tab in this.PropertyTabs)
            {
                var propTab = tab as StiPropertiesTab;
                if (propTab != null)
                    propTab.PropertyGrid = this;
            }
        }
        
        private void PropertyGridView_FontChanged(object sender, EventArgs e)
        {
            foreach (Control control in this.Controls)
            {
                var controlType = control.GetType();
                if (controlType.Name != "PropertyGridView") continue;

                var cachedRowHeightField = controlType.GetField("cachedRowHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                cachedRowHeightField?.SetValue(control, control.Font.Height + StiScale.I2);

                if (StiScale.X != 1)
                {
                    var outlineSizeExplorerTreeStyleField = controlType.GetField("outlineSizeExplorerTreeStyle", BindingFlags.NonPublic | BindingFlags.Static);
                    outlineSizeExplorerTreeStyleField?.SetValue(control, control.Font.Height);
                }
            }

        }

        private void DropDownListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            var listBox = sender as ListBox;

            var g = e.Graphics;
            var itemRect = e.Bounds;
            var state = e.State;

            itemRect.Height++;

            if ((state & DrawItemState.Selected) > 0)
                itemRect.Width--;

            if (e.Index != -1 && listBox.Items.Count > 0)
            {
                StiControlPaint.DrawItem(g, itemRect, state, listBox.GetItemText(listBox.Items[e.Index]),
                    null, 0, listBox.Font, listBox.BackColor, listBox.ForeColor, listBox.RightToLeft);
            }
        }
        #endregion

        #region Fields
        private static Hashtable hidedProperties = new Hashtable();
        #endregion

        public StiPropertyGrid()
        {
#if NETCOREAPP
            this.Font = new Font(new FontFamily("Microsoft Sans Serif"), 8f);
#endif
            RefreshThemeColors();

            foreach (Control control in this.Controls)
            {
                try
                {
                    var controlType = control.GetType();
                    if (controlType.Name == "PropertyGridView")
                    {
                        control.FontChanged += PropertyGridView_FontChanged;
                        control.Font = new Font(control.Font.FontFamily, control.Font.Size, control.Font.Style);

                        var paintWidthField = controlType.GetField("paintWidth", BindingFlags.NonPublic | BindingFlags.Static);
                        paintWidthField?.SetValue(control, StiScale.XXI(20));

                        var paintIndentField = controlType.GetField("paintIndent", BindingFlags.NonPublic | BindingFlags.Static);
                        paintIndentField?.SetValue(control, StiScale.XXI(26));

                        var dropDownListBoxProperty = controlType.GetProperty("DropDownListBox", BindingFlags.NonPublic | BindingFlags.Instance);
                        var dropDownListBox = dropDownListBoxProperty?.GetValue(control, null) as ListBox;
                        if (dropDownListBox != null)
                        {
                            dropDownListBox.DrawMode = DrawMode.OwnerDrawVariable;
                            dropDownListBox.DrawItem += DropDownListBox_DrawItem;
                        }
                    }

                    if (controlType.Name == "ToolStrip")
                    {
                        var imageSize = StiScale.XXI(16);

                        var normalButtonSizeField = typeof(PropertyGrid).GetField("normalButtonSize",
                            BindingFlags.NonPublic | BindingFlags.Static);
                        normalButtonSizeField?.SetValue(this, new Size(imageSize, imageSize));

                        var toolStrip = control as ToolStrip;
                        toolStrip.ImageScalingSize = new Size(imageSize, imageSize);
                        toolStrip.GripMargin = new Padding(StiScale.I2);

                        var setupToolbarMethod = typeof(PropertyGrid).GetMethod("SetupToolbar",
                            BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(bool) }, null);

                        setupToolbarMethod?.Invoke(this, new object[] { true });
                    }
                }
                catch
                {
                }
            }
        }
    }
}