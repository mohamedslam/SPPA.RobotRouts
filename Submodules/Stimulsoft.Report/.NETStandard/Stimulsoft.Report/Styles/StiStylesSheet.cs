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
using System.Globalization;
using System.IO;
using System.Threading;
using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Serializing;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class describes a list of styles.
    /// </summary>
    public class StiStylesSheet
    {
        /// <summary>
        /// Gets or sets styles of report.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        public StiStylesCollection Styles { get; set; }

        /// <summary>
        /// Saves a list of styles in the stream.
        /// </summary>
        /// <param name="stream">Stream for saving the style list.</param>
        public virtual void Save(Stream stream)
        {
            var ser = new StiSerializing(new StiReportObjectStringConverter());
            ser.Serialize(this, stream, "StiStylesSheet");
        }

        /// <summary>
        /// Loads a list of styles from the stream.
        /// </summary>
        /// <param name="stream">Stream for list of styles loading.</param>
        public virtual void Load(Stream stream)
        {
            var ser = new StiSerializing(new StiReportObjectStringConverter());
            ser.Deserialize(this, stream, "StiStylesSheet");
        }

        /// <summary>
        /// Saves a list of styles from the file.
        /// </summary>
        /// <param name="file">File for list of styles saving.</param>
        public virtual void Save(string file)
        {
            StiFileUtils.ProcessReadOnly(file);
            var stream = new FileStream(file, FileMode.Create, FileAccess.Write);

            var ser = new StiSerializing(new StiReportObjectStringConverter());
            ser.Serialize(this, stream, "StiStylesSheet");

            stream.Flush();
            stream.Close();
        }

        /// <summary>
        /// Loads a list of styles from the file.
        /// </summary>
        /// <param name="file">File for list of styles loading.</param>
        public virtual void Load(string file)
        {
            var stream = new FileStream(file, FileMode.Open, FileAccess.Read);

            var ser = new StiSerializing(new StiReportObjectStringConverter());
            ser.Deserialize(this, stream, "StiStylesSheet");

            stream.Flush();
            stream.Close();
        }

        /// <summary>
        /// Loads a list of styles from the json file.
        /// </summary>
	    public virtual void LoadFromJsonFile(string file)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                var content = File.ReadAllText(file);
                var jObject = JObject.Parse(content);

                this.Styles.Clear();
                this.Styles.LoadFromJsonObject(jObject);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), "Loading styles...ERROR");
                StiLogService.Write(this.GetType(), ex);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        /// <summary>
        /// Creates a new object of the type StiStylesSheet.
        /// </summary>
        /// <param name="styles">Style collection.</param>
        public StiStylesSheet(StiStylesCollection styles)
        {
            this.Styles = styles;
        }
    }
}
