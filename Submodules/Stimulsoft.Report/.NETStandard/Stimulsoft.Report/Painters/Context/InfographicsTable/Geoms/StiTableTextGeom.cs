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

using Stimulsoft.Base.Drawing;
using System.Drawing;

namespace Stimulsoft.Base.Context
{
    public class StiTableTextGeom : StiTableGeom
    {
        #region Properties

        public string Text { get; set; }

        public StiFontGeom Font { get; set; }

        public StiBrush Foreground { get; set; }

        public RectangleF Rect { get; set; }

        public StringAlignment Alignment { get; set; }

        #endregion

        #region Properties.Override
        public override StiTableGeomType Type => StiTableGeomType.Text;
        #endregion

        public StiTableTextGeom(string text, StiFontGeom font, StiBrush foreground,
            RectangleF rect, StringAlignment alignment)
        {
            this.Text = text;
            this.Font = font;
            this.Foreground = foreground;
            this.Rect = rect;
            this.Alignment = alignment;
        }
    }
}
