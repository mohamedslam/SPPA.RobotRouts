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
using System.IO;

using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Events;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using System.Text;

namespace Stimulsoft.Report.SaveLoad
{
	/// <summary>
	/// Describes the class that allows to save/load reports.
	/// </summary>
    public class StiJsonReportSLService : StiReportSLService
	{
		#region StiService override
		/// <summary>
		/// Gets a service type.
		/// </summary>
		public override Type ServiceType => typeof(StiReportSLService);
        #endregion

        #region Methods
        /// <summary>
        /// Saves report in the stream.
        /// </summary>
        /// <param name="report">Report for saving.</param>
        /// <param name="stream">Stream to save report.</param>
        public override void Save(StiReport report, Stream stream)
		{
			report.IsSerializing = true;

			try
			{
                var jsonStr = report.SaveToJsonInternal(StiJsonSaveMode.Report);
                byte[] buf = Encoding.UTF8.GetBytes(jsonStr);
                stream.Write(buf, 0, buf.Length);
                stream.Flush();
			}
			finally
			{
				report.IsSerializing = false;
			}
		}


		/// <summary>
		/// Loads a report from the stream.
		/// </summary>
		/// <param name="report">The report in which loading will be done.</param>
		/// <param name="stream">Stream to save report.</param>
		public override void Load(StiReport report, Stream stream)
		{
			report.Clear(false);
            report.Dictionary.Clear();
            report.Dictionary.Databases.Clear();
            report.Dictionary.Resources.Clear();
            report.IsSerializing = true;					

			StiDataCollection datas = report.DataStore;
			
			var sr = new StiSerializing(new StiReportObjectStringConverter());

			try
			{
                report.LoadFromJsonInternal(stream);

                #region Удаляем Control из списка Components у StiForm
                for (int index = 0; index < report.Pages.Count; index++)
                {
                    if (report.Pages[index] is Stimulsoft.Report.Dialogs.IStiForm)
                    {
                        Stimulsoft.Report.Components.StiPage form = report.Pages[index];
                        int indexComp = 0;
                        while (indexComp < form.Components.Count)
                        {
                            Stimulsoft.Report.Components.StiComponent comp = form.Components[indexComp];
                            if (comp == null)
                            {
                                form.Components.RemoveAt(indexComp);
                            }
                            else
                            {
                                indexComp++;
                            }
                        }
                    }
                }
                #endregion

                #region Проверяем наличие Page у компонентов и устанавливаем если нужно и возможно
                for (int index = 0; index < report.Pages.Count; index++)
                {
                    if (!(report.Pages[index] is Stimulsoft.Report.Dialogs.IStiForm))
                    {
                        foreach (Stimulsoft.Report.Components.StiComponent comp in report.Pages[index].GetComponents())
                        {
                            if (comp.Page == null && comp.Parent != null)
                            {
                                Stimulsoft.Report.Components.StiComponent parent = comp.Parent;
                                while (parent != null)
                                {
                                    if (parent.Page != null)
                                    {
                                        comp.Page = parent.Page;
                                        break;
                                    }
                                    parent = parent.Parent;
                                }
                            }
                        }
                    }
                }
                #endregion

                report.DataStore.Clear();
				report.DataStore.RegData(datas);
			}
			finally
			{
				sr.Deserializing -= OnLoading;
				report.IsSerializing = false;
			}
		}
		
		/// <summary>
		/// Returns actions available for the provider.
		/// </summary>
		/// <returns>Available actions.</returns>
		public override StiSLActions GetAction()
		{
			return StiSLActions.Load | StiSLActions.Save;
		}
		

		/// <summary>
		/// Returns a filter for the provider.
		/// </summary>
		/// <returns>String with filter.</returns>
		public override string GetFilter()
		{
            return StiLocalization.Get("FileFilters", "JsonReportFiles");
		}
        #endregion
    }
}
