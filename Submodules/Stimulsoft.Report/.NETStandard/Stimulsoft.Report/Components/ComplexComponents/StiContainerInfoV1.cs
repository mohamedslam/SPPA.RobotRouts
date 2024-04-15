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
using Stimulsoft.Report.Engine;

namespace Stimulsoft.Report.Components
{	
	public class StiContainerInfoV1 : StiComponentInfo
	{
		public bool ForceBreak { get; set; }

        public bool IsColumn { get; set; }

        /// <summary>
        /// Gets or sets the current column(used for definition of the current column).
        /// </summary>
        public int CurrentColumn { get; set; }

        /// <summary>
        /// Gets or sets the current component for printing.
        /// </summary>
        public StiComponent CurrentComponent { get; set; }

        /// <summary>
        /// Gets or sets a collection of clones.
        /// </summary>
        public StiComponentsCollection CloneComponents { get; set; }

        public Hashtable BottomRenderedFooters { get; set; }

        public Hashtable BottomRenderedGroupFooters { get; set; }

        public Hashtable BottomRenderedDataBands { get; set; }

        public Hashtable BottomRenderedHeaders { get; set; }

        public StiComponentsCollection BottomRenderedParentsFooters { get; set; }

        public StiComponentsCollection BottomRenderedParentsGroupFooters { get; set; }

        public StiComponentsCollection BottomRenderedParentsDataBands { get; set; }

        public StiComponentsCollection BottomRenderedParentsHeaders { get; set; }

        public StiComponent LastDataBand { get; set; }
    }
}