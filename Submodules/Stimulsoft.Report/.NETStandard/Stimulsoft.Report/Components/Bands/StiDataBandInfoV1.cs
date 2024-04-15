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
using System.Collections.Generic;
using System.ComponentModel;
using Stimulsoft.Report.Engine;

namespace Stimulsoft.Report.Components
{	
	public class StiDataBandInfoV1 : StiComponentInfo
	{
		#region Render.SubReports
		/// <summary>
		/// Gets or sets a collection of header groups.
		/// </summary>
		[Browsable(false)]
		public StiComponentsCollection SubReportsComponents { get; set; }
		#endregion

		#region Render.Headers
		/// <summary>
		/// Gets or sets a collection of rendered headers.
		/// </summary>
		[Browsable(false)]
		public StiComponentsCollection RenderedHeaders { get; set; } = new StiComponentsCollection();

		/// <summary>
		/// Gets or sets a collection of headers.
		/// </summary>
		public StiComponentsCollection HeaderComponents { get; set; }
        #endregion

        #region Render.Footers
        /// <summary>
        /// Gets or sets collection of footers.
        /// </summary>
        public StiComponentsCollection FooterComponents { get; set; }
        #endregion

        #region Render.Groups
        /// <summary>
        /// Gets or sets a collection of header groups.
        /// </summary>
        [Browsable(false)]
		public StiComponentsCollection GroupHeaderComponents { get; set; }

        /// <summary>
        /// Gets or sets a collection of footer groups.
        /// </summary>
        [Browsable(false)]
		public StiComponentsCollection GroupFooterComponents { get; set; }
        #endregion

        #region Render.Details
        /// <summary>
        /// Gets or sets collection of details.
        /// </summary>
        public StiComponentsCollection DetailComponents { get; set; }

        /// <summary>
        /// Gets or sets collection of data details.
        /// </summary>
        public StiComponentsCollection DetailDataComponents { get; set; }

        /// <summary>
        /// Gets or sets the current child component. If it equal in null then Master component is printed.
        /// </summary>
        public StiComponent CurrentDetailComponent { get; set; }
        #endregion

        #region Render.Main.Fields
        public int LastPositionRendering { get; set; }

        public bool IsFirstPassOfBreak { get; set; } = true;

		public StiComponentsCollection BreakableComps { get; set; }

        public bool AlwaysKeepChildTogether { get; set; }

        /// <summary>
        /// Gets or sets free space for rendering.
        /// </summary>
        public double FreeSpace { get; set; }

        public bool DataIsPrepared { get; set; }

        public int ColumnIndex { get; set; } = 1;

		public StiContainer ParentColumnContainer { get; set; }

		public bool LatestDataBandBreaked { get; set; }

        public StiBookmark ResParentPointer { get; set; }

        /// <summary>
        /// Parent bookmark of the current component.
        /// </summary>
        public StiBookmark ResParentBookmark { get; set; }

        /// <summary>
        /// Contains the amount of the typed groups.
        /// Begin position the group in container.
        /// </summary>
        public int StartGroupIndex { get; set; } = - 1;

		/// <summary>
		/// From what lines is calculation Line.
		/// </summary>
		public int StartLine { get; set; }

        public int RuntimeLine { get; set; }

        /// <summary>
        /// Last rendered component was breaked.
        /// </summary>
        public bool LastRenderBreaked { get; set; }

        /// <summary>
        /// Last rendered component.
        /// </summary>
        public StiComponent LastComponent { get; set; }

        /// <summary>
        /// Render first row.
        /// </summary>
        public bool FirstRow { get; set; }

        /// <summary>
        /// Render first row in the current pass.
        /// </summary>
        public bool FirstRowInPath { get; set; }

        /// <summary>
        /// Force start new page when on page exist footers from previus page.
        /// </summary>
        public bool ForceStartNewPage { get; set; }

        /// <summary>
        /// Collection of components, which will be render in next pass.
        /// </summary>
        public StiComponentsCollection RemmitedCollection { get; set; } = new StiComponentsCollection();													

		/// <summary>
		/// Collection rendered lines.
		/// </summary>
        public List<StiComponent> RenderedItems { get; set; }

        /// <summary>
        /// Save line to collection?
        /// </summary>
        public bool ItemsActive { get; set; }

        public bool FirstCall { get; set; } = true;

		public bool FirstGroupOnPass { get; set; } = true;

		public bool SkipStartNewPage { get; set; }

        public int StartMasterIndex { get; set; }

        public double ResHeightOfContainerBeforeRendering { get; set; }

        public bool CrossTabExistOnDataBand { get; set; }
        #endregion
    }
}
