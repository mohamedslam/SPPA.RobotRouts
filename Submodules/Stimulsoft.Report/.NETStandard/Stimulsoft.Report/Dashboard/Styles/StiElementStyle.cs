#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Base;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.PropertyGrid;
using System.Drawing;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.Dashboard.Styles
{
    public abstract class StiElementStyle : StiBaseStyle
    {
        #region IStiPropertyGridObject
        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            return new StiPropertyCollection();
        }
        #endregion

        #region Properties
        public abstract StiElementStyleIdent Ident { get; }
        #endregion

        #region Methods.override
        public override void DrawBox(Graphics g, Rectangle rect, bool paintValue, bool paintImage)
        {

        }

        public override void DrawStyle(Graphics g, Rectangle rect, bool paintValue, bool paintImage)
        {

        }

        public override void GetStyleFromComponent(StiComponent component, StiStyleElements styleElements)
        {

        }

        public override void SetStyleToComponent(StiComponent component)
        {

        }
        #endregion
    }
}