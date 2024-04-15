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

namespace Stimulsoft.Report.Engine
{
	public class StiRenderState
    {
        #region Properties
        internal decimal LatestProgressValue { get; set; }

        public int FromPage { get; }

        public int ToPage { get; }

        public bool ShowProgress { get; set; }

        internal bool IsSubReportMode { get; set; }

        public bool DestroyPagesWhichNotInRange { get; }

        public bool RenderOnlyPagesFromRange { get; }
        #endregion

		public StiRenderState(bool showProgress) : 
			this(-1, -1, showProgress)
		{

		}

		public StiRenderState(int fromPage, int toPage, bool showProgress) : 
			this(fromPage, toPage, showProgress, true, false)
		{

		}
	
		public StiRenderState(int fromPage, int toPage, bool showProgress, 
			bool destroyPagesWhichNotInRange, bool renderOnlyPagesFromRange)
		{
			this.FromPage = fromPage;
			this.ToPage = toPage;

			if (fromPage == -1 && toPage == -1)
			    destroyPagesWhichNotInRange = false;

			this.ShowProgress = showProgress;
			this.DestroyPagesWhichNotInRange = destroyPagesWhichNotInRange;
			this.RenderOnlyPagesFromRange = renderOnlyPagesFromRange;
		}
	}
}
