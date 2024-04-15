#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	Stimulsoft.Report Library										}
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
using System.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;

namespace Stimulsoft.Report.SaveLoad
{

	/// <summary>
	/// Describes base class for saving / loading / merging of the providers  data. 
	/// </summary>
	[StiServiceBitmap(typeof(StiSLService), "Stimulsoft.Report.Bmp.SL.SL.bmp")]
	[StiServiceCategoryBitmap(typeof(StiSLService), "Stimulsoft.Report.Bmp.SL.SLCategory.bmp")]
	public abstract class StiSLService : StiService
	{
		#region StiService override
		/// <summary>
		/// Gets a service category.
		/// </summary>
		public sealed override string ServiceCategory => StiLocalization.Get("Services", "categorySL");
	    #endregion

		#region Events
		#region OnSaving
		/// <summary>
		/// Event occurs when saving report engine save one element.
		/// </summary>
		public event EventHandler Saving;

		protected virtual void OnSaving(EventArgs e)
		{
			
		}

		/// <summary>
		/// Raises the Saving event for this object.
		/// </summary>
		protected void InvokeSaving()
		{
			OnSaving(EventArgs.Empty);
            Saving?.Invoke(null, EventArgs.Empty);
        }

		protected void OnSaving(object sender, EventArgs e)
		{
			InvokeSaving();
		}
		#endregion

		#region OnLoading
		/// <summary>
		/// Event occurs when loading report engine load one element.
		/// </summary>
		public event EventHandler Loading;

		protected virtual void OnLoading(EventArgs e)
		{
			
		}

		/// <summary>
		/// Raises the Loading event for this object.
		/// </summary>
		protected void InvokeLoading()
		{
			OnLoading(EventArgs.Empty);
            Loading?.Invoke(null, EventArgs.Empty);
        }

		protected void OnLoading(object sender, EventArgs e)
		{
			InvokeLoading();
		}
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Returns actions available for the provider.
        /// </summary>
        /// <returns>Available actions.</returns>
        public abstract StiSLActions GetAction();

		/// <summary>
		/// Returns a filter for the provider.
		/// </summary>
		/// <returns>String with filter.</returns>
		public abstract string GetFilter();

		/// <summary>
		/// Returns a container of services for all available services for saving / loading of a eport.
		/// </summary>
		/// <param name="action">Action required from services.</param>
		/// <returns>Container of services.</returns>
        public static List<StiReportSLService> GetReportSLServices(StiSLActions action)
		{
            return StiOptions.Services.ReportSLs
                .Where(s => s.ServiceEnabled)
                .Where(s => (s.GetAction() & action) > 0)
                .ToList();
		}

        /// <summary>
        /// Returns a container of services  for all available services for saving / loading of a page.
        /// </summary>
        /// <param name="action">Action required from services.</param>
        /// <returns>Container of services.</returns>
        public static List<StiPageSLService> GetPageSLServices(StiSLActions action)
		{
            return StiOptions.Services.PageSLs
                .Where(s => s.ServiceEnabled)
                .Where(s => (s.GetAction() & action) > 0)
                .ToList();
		}

		/// <summary>
		/// Returns a container of services for all available services for saving / loading of a document.
		/// </summary>
		/// <param name="action">Action required from services.</param>
		/// <returns>Container of services.</returns>
        public static List<StiDocumentSLService> GetDocumentSLServices(StiSLActions action)
		{
            return StiOptions.Services.DocumentSLs
                .Where(s => s.ServiceEnabled)
                .Where(s => (s.GetAction() & action) > 0)
                .ToList();
		}
		
		/// <summary>
		/// Returns a filter for all available services for saving / loading of a dictioary of data.
		/// </summary>
		/// <param name="action">Action required from services.</param>
		/// <returns>Container of services.</returns>
		public static List<StiDictionarySLService> GetDictionarySLServices(StiSLActions action)
		{
            return StiOptions.Services.DictionarySLs
                .Where(s => s.ServiceEnabled)
                .Where(s => (s.GetAction() & action) > 0)
                .ToList();
		}

		/// <summary>
		/// Returns a filter for all available services for saving / loading of a page.
		/// </summary>
		/// <param name="action">Action required from services.</param>
		/// <returns>Filter.</returns>
		public static string GetPageFilters(StiSLActions action)
		{
			var filter = "";
			var first = true;
			
			var services = GetPageSLServices(action);

			foreach (var service in services)
			{
				if (!first)filter += '|';
				filter += service.GetFilter();
				first = false;
			}
			return filter;
		}

		/// <summary>
		/// Returns a filter for all available services for saving / loading of a document.
		/// </summary>
		/// <param name="action">Action required from services.</param>
		/// <returns>Filter.</returns>
		public static string GetDocumentFilters(StiSLActions action)
		{
			var filter = "";
			var first = true;
			
			var services = GetDocumentSLServices(action);
			foreach (var service in services)
			{
				var serviceFilter = service.GetFilter();
				if (action == StiSLActions.Load || action == StiSLActions.Merge)
				{
					if (service is StiPackedDocumentSLService) continue;
                    if (service is StiEncryptedDocumentSLService) continue;
                    if (service is StiJsonDocumentSLService) continue;
                    if (service is StiXmlDocumentSLService) serviceFilter = serviceFilter.Replace("mdc", "mdc;*.mdz;*.mdx");
				}

				if (!first)filter += '|';
				filter += serviceFilter;
				first = false;
			}
			return filter;
		}

		/// <summary>
		/// Returns a filter for all available services for saving / loading of a report.
		/// </summary>
		/// <param name="action">Action required from services.</param>
		/// <returns>Filter.</returns>
		public static string GetReportFilters(StiSLActions action)
		{
			var filter = "";
			var first = true;
			
            var services = GetReportSLServices(action);
			foreach (var service in services)
			{
				var serviceFilter = service.GetFilter();
                if (action == StiSLActions.Load || action == StiSLActions.Merge)
                {
                    if (service is StiPackedReportSLService) continue;
                    if (service is StiEncryptedReportSLService) continue;
                    if (service is StiJsonReportSLService) continue;
                    if (service is StiXmlReportSLService) serviceFilter = serviceFilter.Replace("mrt", "mrt;*.mrz;*.mrx");
                }

				if (!first)filter += '|';
				filter += serviceFilter;
				first = false;
			}
			return filter;
		}

        /// <summary>
        /// Returns a filter for all available services for saving / loading / merging of dictionary of data.
        /// </summary>
        /// <param name="action">Action required from services.</param>
        /// <returns>Filter.</returns>
        public static string GetDictionaryFilters(StiSLActions action)
		{
			var filter = "";
			var first = true;
			
            var services = GetDictionarySLServices(action);
			foreach (var service in services)
			{
				if (!first)filter += '|';
				filter += service.GetFilter();
				first = false;
			}
			return filter;
		}
        #endregion
    }
}
