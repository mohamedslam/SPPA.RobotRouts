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

using System.Collections.Generic;
using System.Drawing;
using Stimulsoft.Base.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Image = Stimulsoft.Drawing.Image;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Painters
{
    public interface IStiBarCodePainter
    {
        void BaseTransform(object context, float x, float y, float angle, float dx, float dy);

        void BaseRollbackTransform(object context);

        void BaseFillRectangle(object context, StiBrush brush, float x, float y, float width, float height);

        void BaseFillRectangle2D(object context, StiBrush brush, float x, float y, float width, float height);

        void BaseFillPolygon(object context, StiBrush brush, PointF[] points);

        void BaseFillPolygons(object context, StiBrush brush, List<List<PointF>> points, RectangleF? rectf = null);

        void BaseFillEllipse(object context, StiBrush brush, float x, float y, float width, float height);

        void BaseDrawRectangle(object context, Color penColor, float penSize, float x, float y, float width, float height);

        void BaseDrawImage(object context, Image image, StiReport report, float x, float y, float width, float height);

        void BaseDrawString(object context, string st, Font font, StiBrush brush, RectangleF rect, StringFormat sf);

        SizeF BaseMeasureString(object context, string st, Font font);
    }
}
