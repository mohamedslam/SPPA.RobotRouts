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
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Json;
using System.Globalization;
using System.Threading;

namespace Stimulsoft.Report.SaveLoad
{
    /// <summary>
    /// Describes the class that allows to save / load dictionaries of data.
    /// </summary>
    public class StiJsonDictionarySLService : StiDictionarySLService
    {
        #region StiService override
        /// <summary>
        /// Gets a service type.
        /// </summary>
        public override Type ServiceType => typeof(StiDictionarySLService);
        #endregion

        #region Methods
        /// <summary>
        /// Saves a dictionary of data in the stream.
        /// </summary>
        /// <param name="dictionary">Dictionary of data for saving.</param>
        /// <param name="stream">Stream to save dictionary of data.</param>
        public override void Save(StiDictionary dictionary, Stream stream)
        {
        }

        /// <summary>
        /// Loads a dictionay of data from the stream.
        /// </summary>
        /// <param name="dictionary">Dictionary of data in which loading will be done.</param>
        /// <param name="stream">Stream to load dictionary of data.</param>
        public override void Load(StiDictionary dictionary, Stream stream)
        {
            string jsonStr;
            using (var readed = new StreamReader(stream))
            {
                jsonStr = readed.ReadToEnd();
            }

            var currentCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                var jObject = (JObject)JsonConvert.DeserializeObject(jsonStr);

                dictionary.LoadFromJsonObject(jObject);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), "Loading dictionary...ERROR");
                StiLogService.Write(this.GetType(), ex);

                if ((!StiOptions.Engine.HideExceptions) || dictionary.Report.IsDesigning) throw;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        /// <summary>
        /// Merge dictionary of data from the stream.
        /// </summary>
        /// <param name="dictionary">Merge dictionary of data.</param>
        /// <param name="stream">Stream to merge dictionary of data.</param>
        public override void Merge(StiDictionary dictionary, Stream stream)
        {
            var dict = new StiDictionary(dictionary.Report);
            Load(dict, stream);
            dictionary.Merge(dict);
        }

        /// <summary>
        /// Returns actions available for the provider.
        /// </summary>
        /// <returns>Available actions.</returns>
        public override StiSLActions GetAction()
        {
            return StiSLActions.Load | StiSLActions.Merge;
        }

        /// <summary>
        /// Returns a filter for the provider.
        /// </summary>
        /// <returns>String with filter.</returns>
        public override string GetFilter()
        {
            return StiLocalization.Get("FileFilters", "DictionaryFiles");
        }
        #endregion
    }
}
