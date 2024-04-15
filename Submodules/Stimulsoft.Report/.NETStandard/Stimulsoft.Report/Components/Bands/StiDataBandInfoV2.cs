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
using Stimulsoft.Report.Engine;

namespace Stimulsoft.Report.Components
{	
	public class StiDataBandInfoV2 : StiComponentInfo
	{
		/// <summary>
		/// Коллекция содержит все подчиненные GroupHeaderBand's текущего DataBand. Для заполнения
		/// коллекцию необходимо вызвать метод FindGroupHeaders.
		/// </summary>
		public StiComponentsCollection GroupHeaders { get; set; }

        /// <summary>
        /// Коллекция содержит все подчиненные GroupFooterBand's текущего DataBand. Для заполнения
        /// коллекцию необходимо вызвать метод FindGroupFooters.
        /// </summary>
        public StiComponentsCollection GroupFooters { get; set; }

        /// <summary>
        /// Коллекция содержит все подчиненные DataBand's текущего DataBand. Для заполнения
        /// коллекцию необходимо вызвать метод FindDetailDataBands.
        /// </summary>
        public StiComponentsCollection DetailDataBands { get; set; }

        /// <summary>
        /// Коллекция содержит все подчиненные компоненты текущего DataBand. Для заполнения
        /// коллекцию необходимо вызвать метод FindDetails.
        /// </summary>
        public StiComponentsCollection Details { get; set; }

        /// <summary>
        /// Коллекция содержит все подчиненные SubReport's текущего DataBand. Для заполнения
        /// коллекцию необходимо вызвать метод FindSubReports.
        /// </summary>
        public StiComponentsCollection SubReports { get; set; }

        /// <summary>
        /// Коллекция содержит все подчиненные EmptyBand's текущего DataBand. Для заполнения
        /// коллекцию необходимо вызвать метод FindEmptyBands.
        /// </summary>
        public StiComponentsCollection EmptyBands { get; set; }

        /// <summary>
        /// Коллекция содержит все подчиненные HeaderBand's текущего DataBand. Для заполнения
        /// коллекцию необходимо вызвать метод FindHeaders.
        /// </summary>
        public StiComponentsCollection Headers { get; set; }

        /// <summary>
        /// Коллекция содержит все подчиненные HeaderBand's текущего HierarchicalDataBand. Для заполнения
        /// коллекцию необходимо вызвать метод FindHierarchicalHeaders. Метод нужно вызывать после вызова метода FindHeaders.
        /// </summary>
        public StiComponentsCollection HierarchicalHeaders { get; set; }

        /// <summary>
        /// Коллекция содержит все подчиненные Footer's, которые выводятся на всех страницах,
        /// текущего DataBand. Для заполнения коллекцию необходимо вызвать метод FindFootersOnAllPages.
        /// </summary>
        public StiComponentsCollection FootersOnAllPages { get; set; }

        /// <summary>
        /// Коллекция содержит все подчиненные Footer's, которые выводятся в конце списка,
        /// текущего DataBand. Для заполнения коллекцию необходимо вызвать метод FindFootersOnLastPage.
        /// </summary>
        public StiComponentsCollection FootersOnLastPage { get; set; }

        /// <summary>
        /// Коллекция содержит все подчиненные Footer's, которые выводятся в конце списка текущего HierarchicalDataBand. 
        /// Для заполнения коллекцию необходимо вызвать метод FindHierarchicalFooters. Метод нужно вызывать после вызова метода FindFooters.
        /// </summary>
        public StiComponentsCollection HierarchicalFooters { get; set; }

        /// <summary>
        /// Массив результатов вычисления группировки для GroupHeaderBand's.
        /// </summary>
        public bool[] GroupHeaderResults { get; set; }

        public bool[][] GroupHeaderCachedResults { get; set; }

        /// <summary>
        /// Массив результатов вычисления группировки для GroupFooterBand's.
        /// </summary>
        public bool[] GroupFooterResults { get; set; }

        public bool[][] GroupFooterCachedResults { get; set; }

        public List<StiReportTitleBand> ReportTitles { get; set; }

        public List<StiReportSummaryBand> ReportSummaries { get; set; }

        /// <summary>
        /// Коллекция содержит все подчиненные DataBand's текущего DataBand, которые связаны с BusinessObject и находятся в саб-репортах.
        /// Бэнды из этой коллекции не должны выводится как details, т.к. они выводятся в другом месте
        /// </summary>
        internal Hashtable DetailDataBandsFromSubReports { get; set; }

        public StiBookmark StoredParentPointer { get; set; }

        /// <summary>
        /// Поле содержит резервную копию ParentBookmark. Копия используется для заполнения поля ParentBookmark у выводимых бэндов, поскольку 
        /// ParentBookmark может быть искажен группой.
        /// </summary>
        public StiBookmark StoredParentBookmark { get; set; }
}
}
