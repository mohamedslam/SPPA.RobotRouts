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

using System;
using System.Collections.Generic;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Events
{
	public delegate void StiStyleDesignerOpeningEventHandler(object sender, StiStyleDesignerOpeningEventArgs e);

	public sealed class StiStyleDesignerOpeningEventArgs : EventArgs
	{
        #region Properties
        public List<StiCustomStyleButtonInfo> CustomAddButtons { get; set; } = new List<StiCustomStyleButtonInfo>();
        #endregion
    }


    public sealed class StiCustomStyleButtonInfo
    {
        public StiCustomStyleButtonInfo(Image icon16, Image icon32, string text, Type styleType)
        {
            this.Icon16 = icon16;
            this.Icon32 = icon32;
            this.Text = text;
            this.StyleType = styleType;
        }

        public StiCustomStyleButtonInfo(object iconWpf16, object iconWpf32, string text, Type styleType)
        {
            this.IconWpf16 = iconWpf16;
            this.IconWpf32 = iconWpf32;
            this.Text = text;
            this.StyleType = styleType;
        }

        #region Properties
        public object IconWpf16 { get; private set; }
        public object IconWpf32 { get; private set; }
        public Image Icon16 { get; private set; }
        public Image Icon32 { get; private set; }

        public string Text { get; private set; }
        public Type StyleType { get; private set; }
        #endregion
    }
}