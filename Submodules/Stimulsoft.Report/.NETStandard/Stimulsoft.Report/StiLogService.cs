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
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Design;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report
{
	/// <summary>
	/// The service that is responsible for keeping the log in the report system.
	/// </summary>
	public class StiLogService : StiService, IDisposable
	{
		#region StiService override
		/// <summary>
		/// Gets a service category.
		/// </summary>
		public override string ServiceCategory
		{
			get
			{
				return StiLocalization.Get("Services", "categorySystem");
			}
		}

		/// <summary>
		/// Gets a service type.
		/// </summary>
		public override Type ServiceType
		{
			get
			{
				return typeof(StiLogService);
			}
		}
		#endregion

		#region IDisposable
		private bool disposed;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if(!this.disposed)
			{
				if(disposing)CloseWriter();
			}
			disposed = true;         
		}
		#endregion
		
		#region Properties
	    /// <summary>
		/// Gets or sets value indicates that it is necessary to clear the log when running of the report system.
		/// </summary>
		[StiServiceParam]
		[DefaultValue(true)]
		[StiCategory("Parameters")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		public virtual bool ClearLogOnStart { get; set; } = true;

	    private static bool logEnabled;
		public static bool LogEnabled
		{
			get
			{
				return logEnabled;
			}
			set
			{
				logEnabled = value;
				if (!value)
				{
                    foreach (var service in StiOptions.Services.Logs)
					{
						service.CloseWriter();
					}
				}
			}
		}

	    public static bool TraceEnabled { get; set; }
        
	    private string logPath = "Report.log";
		/// <summary>
		/// Gets or sets a path to the file in which logs are kept.
		/// </summary>
		[StiServiceParam]
		[DefaultValue("Report.log")]
		[StiCategory("Parameters")]
		public virtual string LogPath
		{
			get
			{
				return logPath;
			}
			set
			{
				if (logPath != value)
				{
					CloseWriter();
					logPath = value;
				}
			}
		}
		#endregion
		
		#region Methods
		/// <summary>
		/// Writes an message into the log.
		/// </summary>
		/// <param name="s">The message which will be added to the log.</param>
		public virtual void WriteLogString(string s)
		{
			if (ServiceEnabled)
			{
				try
				{
					OpenWriter();
					logStream.Seek(0, SeekOrigin.End);
					logWriter.WriteLine(s);
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// Writes an message into the log.
		/// </summary>
		/// <param name="type">The type to add to a message.</param>
		/// <param name="message">The message which will be added to the log.</param>
		public static void Write(Type type, string message)
		{
			Write(type.Name + ": " + message);
		}

		/// <summary>
		/// Writes the log message about exeption.
		/// </summary>
		/// <param name="type">The type to add to a message.</param>
		/// <param name="e">The exeption which will be added to the log.</param>
		public static void Write(Type type, Exception e)
		{
			Write(type.Name + ": " + "Method : [" + e.TargetSite.Name + "] : " + e.Message + "\r\n" +
				"====================================================\r\n" +
				e.StackTrace + "\r\n" +
				"====================================================");
		}

		/// <summary>
		/// Writes an message into the log.
		/// </summary>
		/// <param name="message">The message which will be added to the log.</param>
		public static void Write(string message)
		{
            try
            {
                message = DateTime.Today.ToString("yyyy:MM:dd") + ", " + DateTime.Now.ToString("HH:mm") + ": " + message;
            }
            catch (ArgumentOutOfRangeException)
            {
                message = DateTime.Today + ", " + DateTime.Now + ": " + message;
            }

			#region Write to log
            if (LogEnabled && (!StiOptions.Configuration.IsWeb))
			{
				var service = StiOptions.Services.Logs.FirstOrDefault();
				if (service == null)return;
				service.WriteLogString(message);
			}
			#endregion

			#region Write to Trace
			if (TraceEnabled)
			{
				Trace.WriteLine(message);
			}
			#endregion
		}

        public static void Write(Exception exception)
        {
            try
            {
                Debug.WriteLine(DateTime.UtcNow);

                Debug.WriteLine("==================================");

                Debug.WriteLine("Exception.Message: {0}", exception.Message);
                Debug.WriteLine("Exception.Source: {0}", exception.Source);
                Debug.WriteLine("Exception.TargetSite: {0}", exception.TargetSite);
                Debug.WriteLine("Exception.StackTrace: {0}", exception.StackTrace);

                if (exception.InnerException != null)
                {
                    Debug.WriteLine("==================================");
                    Debug.WriteLine("InnerException.Message: {0}", exception.InnerException.Message);
                    Debug.WriteLine("InnerException.Source: {0}", exception.InnerException.Source);
                    Debug.WriteLine("InnerException.TargetSite: {0}", exception.InnerException.TargetSite);
                    Debug.WriteLine("InnerException.StackTrace: {0}", exception.InnerException.StackTrace);
                }
                Debug.WriteLine("==================================");
            }
            catch
            {

            }
        }

		private void CloseWriter()
		{
			if (logWriter != null)
			{
				try
				{
					logWriter.Close();
					logWriter = null;

					logStream.Close();
					logStream = null;
				}
				catch
				{
				}
			}
		}

		private void OpenWriter()
		{
			if (logWriter == null)
			{
				try
				{			
					string file = StiPath.GetPath(LogPath);

					if (first && ClearLogOnStart)
					{
						first = false;
						if (File.Exists(file))File.Delete(file);
					}

					StiFileUtils.ProcessReadOnly(file);
					logStream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
					logWriter = new StreamWriter(logStream);
					logWriter.AutoFlush = true;
				}
				catch
				{
				}
			}
		}
		#endregion
		
		#region Fields
		private FileStream logStream;
		private StreamWriter logWriter;
		private bool first = true;
		#endregion				
	}
}
