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
    public class StiConversionContourTextInTextAction : StiAction
    {
        public override string Name => StiLocalizationExt.Get("CheckActions", "Convert");

        public override string Description => StiLocalizationExt.Get("CheckActions", "Convert");

        public override void Invoke(StiReport report, object element, string elementName)
        {
            base.Invoke(report, null, null);

            var contourText = element as StiContourText;
            if (contourText == null)return;

            var text = new StiText();

            text.Text = contourText.Text;
            text.TextBrush = contourText.TextBrush;
            text.TextFormat = contourText.TextFormat;
            text.TextOptions = contourText.TextOptions;
            text.TextQuality = contourText.TextQuality;
            text.WordWrap = contourText.WordWrap;
            text.RenderTo = contourText.RenderTo;
            text.ProcessAt = contourText.ProcessAt;
            text.ProcessAtEnd = contourText.ProcessAtEnd;
            text.ProcessingDuplicates = contourText.ProcessingDuplicates;
            text.NullValue = contourText.NullValue;
            text.LinesOfUnderline = contourText.LinesOfUnderline;
            text.LinesOfUnderlining = contourText.LinesOfUnderlining;
            text.Format = contourText.Format;
            text.Font = contourText.Font;
            text.ExcelValue = contourText.ExcelValue;
            text.Angle = contourText.Angle;
            text.AllowHtmlTags = contourText.AllowHtmlTags;
            text.HorAlignment = contourText.HorAlignment;
            text.VertAlignment = contourText.VertAlignment;

            text.Parent = contourText.Parent;
            text.Page = contourText.Page;

            text.SetPaintRectangle(contourText.GetPaintRectangle());
            text.ClientRectangle = contourText.ClientRectangle;
            text.MinSize = contourText.MinSize;
            text.MaxSize = contourText.MaxSize;
            text.Border = contourText.Border;
            text.Brush = contourText.Brush;

            text.Conditions = contourText.Conditions;
            text.ComponentStyle = contourText.ComponentStyle;
            text.UseParentStyles = contourText.UseParentStyles;

            text.CanGrow = contourText.CanGrow;
            text.CanShrink = contourText.CanShrink;
            text.GrowToHeight = contourText.GrowToHeight;
            text.DockStyle = contourText.DockStyle;
            text.Enabled = contourText.Enabled;
            text.Interaction = contourText.Interaction;
            text.Printable = contourText.Printable;
            text.PrintOn = contourText.PrintOn;
            text.ShiftMode = contourText.ShiftMode;

            text.Name = contourText.Name;
            text.Alias = contourText.Alias;
            text.Restrictions = contourText.Restrictions;
            text.Locked = contourText.Locked;
            text.Linked = contourText.Linked;

            var parent = contourText.Parent;
            if (parent != null)
            {
                var index = parent.Components.IndexOf(contourText);
                if (index != -1)
                {
                    parent.Components.RemoveAt(index);
                    parent.Components.Insert(index, text);
                }
            }

        }
    }
}