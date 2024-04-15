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
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.Engine;
using Stimulsoft.Base;
using System.Drawing;
using Stimulsoft.Report.Images;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiCrossHeaderGdiPainter : StiTextGdiPainter
    {
        #region Methods
		public override void PaintInteraction(StiComponent component, Graphics g)
		{
            if (component.IsDesigning) return;
			if (!(component is StiCrossHeader)) return;
			if (component.Report.Info.Zoom < .5) return;
			if (!component.Report.Info.ShowInteractive) return;
			if (component.Report.EngineVersion == Stimulsoft.Report.Engine.StiEngineVersion.EngineV1) return;

            var crossHeader = component as StiCrossHeader;
			var interaction = component as IStiInteraction;
            if (interaction != null && interaction.Interaction is StiCrossHeaderInteraction && ((StiCrossHeaderInteraction)interaction.Interaction).CollapsingEnabled)
			{
				var rect = component.GetPaintRectangle().ToRectangleF();

                bool collapsed = StiCrossTabV2Builder.IsCollapsed(crossHeader);
				var image = collapsed ? StiReportImages.Engine.Collapsed() : StiReportImages.Engine.Expanded();

                int x = (int)rect.Left + StiScale.I3;

                if (StiOptions.Viewer.Pins.EventsRightToLeft)
                    x = (int)rect.Right - StiScale.I3 - image.Width;

				g.DrawImage(
					image,
					x, (int)rect.Y + StiScale.I3,
					image.Width,
					image.Height);
			}
		}
        #endregion
    }
}
