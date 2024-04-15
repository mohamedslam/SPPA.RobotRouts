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

using Stimulsoft.Base.Context;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiAreaGeom : StiCellGeom
    {
        #region Properties
        public IStiArea Area { get; private set; }
        #endregion

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
                #region Fill rectangle
                context.FillRectangle(Area.Brush, rect.X, rect.Y, rect.Width, rect.Height, null);
                #endregion
            }
        }
        #endregion

        public StiAreaGeom(IStiArea area, RectangleF clientRectangle)
            : base(Rectangle.Round(clientRectangle))
        {
            this.Area = area;
        }
    }
}
