﻿#region Copyright (C) 2003-2022 Stimulsoft
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

using System.Drawing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiPictorialAreaGeom : StiAreaGeom
    {
        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            RectangleF rect = ClientRectangle;
            if (rect.IsEmpty) return;

            if (rect.Width > 0 && rect.Height > 0)
            {
                #region Draw Shadow
                if (Area.ShowShadow)
                    context.DrawCachedShadow(rect, StiShadowSides.All, context.Options.IsPrinting);
                #endregion
            }

            base.Draw(context);
        }

        public override void DrawGeom(StiContext context)
        {
            base.DrawGeom(context);

            //We need paint border over all other geoms
            DrawBorder(context);
        }

        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        private void DrawBorder(StiContext context)
        {
            var rect = this.ClientRectangle;
            if (rect.IsEmpty) return;

            #region Draw Border
            var pen = new StiPenGeom(Area.BorderColor, Area.BorderThickness);
            context.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
            #endregion
        }
        #endregion

        public StiPictorialAreaGeom(IStiArea area, RectangleF clientRectangle)
            : base(area, clientRectangle)
        {
        }
    }
}
