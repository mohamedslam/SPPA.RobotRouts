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

using System.Collections;
using Stimulsoft.Report.CrossTab.Core;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Engine;

namespace Stimulsoft.Report.CrossTab
{	
	public class StiCrossTabInfo : StiComponentInfo
	{	
		public int DefaultWidth { get; set; } = 60;

		public int DefaultHeight { get; set; } = 14;
		
		public Hashtable HidedCells { get; } = new Hashtable();
		
		public StiCross Cross { get; set; }

        public RectangleD RenderRect { get; set; } = RectangleD.Empty;

		public bool FinishRender { get; set; } = false;
	}
}
