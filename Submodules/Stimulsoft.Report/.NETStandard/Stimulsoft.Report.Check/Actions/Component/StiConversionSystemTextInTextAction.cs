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
    public class StiConversionSystemTextInTextAction : StiAction
    {
        public override string Name => StiLocalizationExt.Get("CheckActions", "Convert");

        public override string Description => StiLocalizationExt.Get("CheckActions", "Convert");

        public override void Invoke(StiReport report, object element, string elementName)
        {
            base.Invoke(report, null, null);

            var systemText = element as StiSystemText;
            if (systemText == null)return;

            var text = new StiText();

            text.Text = systemText.Text;
            text.TextBrush = systemText.TextBrush;
            text.TextFormat = systemText.TextFormat;
            text.TextOptions = systemText.TextOptions;
            text.TextQuality = systemText.TextQuality;
            text.WordWrap = systemText.WordWrap;
            text.RenderTo = systemText.RenderTo;
            text.ProcessAt = systemText.ProcessAt;
            text.ProcessAtEnd = systemText.ProcessAtEnd;
            text.ProcessingDuplicates = systemText.ProcessingDuplicates;
            text.NullValue = systemText.NullValue;
            text.LinesOfUnderline = systemText.LinesOfUnderline;
            text.LinesOfUnderlining = systemText.LinesOfUnderlining;
            text.Format = systemText.Format;
            text.Font = systemText.Font;
            text.ExcelValue = systemText.ExcelValue;
            text.Angle = systemText.Angle;
            text.AllowHtmlTags = systemText.AllowHtmlTags;
            text.HorAlignment = systemText.HorAlignment;
            text.VertAlignment = systemText.VertAlignment;

            text.Parent = systemText.Parent;
            text.Page = systemText.Page;

            text.SetPaintRectangle(systemText.GetPaintRectangle());
            text.ClientRectangle = systemText.ClientRectangle;
            text.MinSize = systemText.MinSize;
            text.MaxSize = systemText.MaxSize;
            text.Border = systemText.Border;
            text.Brush = systemText.Brush;

            text.Conditions = systemText.Conditions;
            text.ComponentStyle = systemText.ComponentStyle;
            text.UseParentStyles = systemText.UseParentStyles;

            text.CanGrow = systemText.CanGrow;
            text.CanShrink = systemText.CanShrink;
            text.GrowToHeight = systemText.GrowToHeight;
            text.DockStyle = systemText.DockStyle;
            text.Enabled = systemText.Enabled;
            text.Interaction = systemText.Interaction;
            text.Printable = systemText.Printable;
            text.PrintOn = systemText.PrintOn;
            text.ShiftMode = systemText.ShiftMode;

            text.Name = systemText.Name;
            text.Alias = systemText.Alias;
            text.Restrictions = systemText.Restrictions;
            text.Locked = systemText.Locked;
            text.Linked = systemText.Linked;

            var parent = systemText.Parent;
            if (parent != null)
            {
                int index = parent.Components.IndexOf(systemText);
                if (index != -1)
                {
                    parent.Components.RemoveAt(index);
                    parent.Components.Insert(index, text);
                }
            }

        }
    }
}