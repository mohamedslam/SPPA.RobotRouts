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

using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Helpers;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace Stimulsoft.Report.Export
{
    #region Infomation about encoding
    //	0x01 437    U.S. MS-DOS 
    //	0x69 620*    Mazovia (Polish) MS-DOS 
    //	0x6A 737*    Greek MS-DOS (437G) 
    //	0x02 850    International MS-DOS 
    //	0x64 852    Eastern European MS-DOS 
    //	0x67 861    Icelandic MS-DOS 
    //	0x66 865    Nordic MS-DOS 
    //	0x65 866    Russian MS-DOS 
    //	0x68 895    Kamenicky (Czech) MS-DOS 
    //	0x6B 857    Turkish MS-DOS 
    //	0xC8 1250   EasternEuropean MS-DOS 
    //	0xC9 1251   Russian Windows 
    //	0x03 1252   Windows ANSI 
    //	0xCB 1253   Greek Windows 
    //	0xCA 1254   Turkish Windows 
    //	0x04 10000  Standard Macintosh 
    //	0x98 10006  Greek Macintosh 
    //	0x96 10007*  Russian Macintosh 
    //	0x97 10029  Eastern European Macintosh 
    #endregion

    /// <summary>
    /// A class for the export to Dbf.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceDbf.png")]
	public class StiDbfExportService : StiExportService
    {
		#region Fields
		private StiReport report;
		private string fileName;
		private bool sendEMail;
		private StiGuiMode guiMode;
		private Stream writer2;
		private string separator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
		#endregion

		#region StiExportService override
		/// <summary>
		/// Gets or sets a default extension of export. 
		/// </summary>
		public override string DefaultExtension => "dbf";

		public override StiExportFormat ExportFormat => StiExportFormat.Dbf;

		/// <summary>
        /// Gets a group of the export in the context menu.
		/// </summary>
		public override string GroupCategory => "Data";

		/// <summary>
        /// Gets a position of the export in the context menu.
		/// </summary>
		public override int Position => (int)StiExportPosition.Data;

        /// <summary>
        /// Gets a name of the export in the context menu.
        /// </summary>
		public override string ExportNameInMenu => StiLocalization.Get("Export", "ExportTypeDbfFile");

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportDbf(report, stream, settings as StiDataExportSettings);
			InvokeExporting(100, 100, 1, 1);
		}

        /// <summary>
        /// Exports a document to the file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        /// <param name="sendEMail">A parameter indicating whether is exported report will be sended via e-mail.</param>
        public override void Export(StiReport report, string fileName, bool sendEMail, StiGuiMode guiMode)
		{
            using (var form = StiGuiOptions.GetExportFormRunner("StiDbfExportSetupForm", guiMode, this.OwnerWindow))
			{
				form["CurrentPage"] = report.CurrentPrintPage;
                form["OpenAfterExportEnabled"] = !sendEMail;

                this.report = report;
                this.fileName = fileName;
                this.sendEMail = sendEMail;
                this.guiMode = guiMode;
                form.Complete += Form_Complete;
				form.ShowDialog();
			}
		}

		/// <summary>
		/// Gets a value indicating a number of files in exported document as a result of export
		/// of one page of the rendered report.
		/// </summary>        
		public override bool MultipleFiles => false;

        /// <summary>
        /// Returns a filter for the dbf files.
        /// </summary>
        /// <returns>String with filter.</returns>
		public override string GetFilter() => StiLocalization.Get("FileFilters", "DbfFiles");
		#endregion

		#region Handlers
		private void Form_Complete(IStiFormRunner form, StiShowDialogCompleteEvetArgs e)
		{
			if (e.DialogResult)
			{
				if (string.IsNullOrEmpty(fileName))
					fileName = base.GetFileName(report, sendEMail);

				if (fileName != null)
				{
					var codePage = (StiDbfCodePages)Enum.Parse(typeof(StiDbfCodePages),
						codePageCodes[(int)form["EncodingSelectedIndex"], 1].ToString());

					StiFileUtils.ProcessReadOnly(fileName);
					try
					{
						using (var stream = new FileStream(fileName, FileMode.Create))
						{
							StartProgress(guiMode);

							var settings = new StiDataExportSettings
							{
								PageRange = form["PagesRange"] as StiPagesRange,
								CodePage = codePage
							};

							base.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
						}
					}
					catch (Exception ex)
					{
						throw ex;
					}
				}
			}
			else IsStopped = true;
		}
		#endregion

		#region Constants
		/// <summary>
		/// Returns a code name of the page.
		/// </summary>
		public static string[] copePageNames =
		{
			"Default",
			"437  U.S. MS-DOS",
			"620  Mazovia (Polish) MS-DOS",
			"737  Greek MS-DOS (437G)",
			"850  International MS-DOS",
			"852  Eastern European MS-DOS",
			"861  Icelandic MS-DOS",
			"865  Nordic MS-DOS",
			"866  Russian MS-DOS",
			"895  Kamenicky (Czech) MS-DOS",
			"857  Turkish MS-DOS",
			"1250  EasternEuropean MS-DOS",
			"1251  Russian Windows",
			"1252  Windows ANSI",
			"1253  Greek Windows",
			"1254  Turkish Windows",
			"10000  Standard Macintosh",
			"10006  Greek Macintosh",
			"10007  Russian Macintosh",
			"10029  Eastern European Macintosh"
		};

		internal static int[,] codePageCodes = 
		{
			{0x00,  0},
			{0x01,  437},
			{0x69,  620}, 
			{0x6A,  737}, 
			{0x02,  850},  
			{0x64,  852},  
			{0x67,  861},  
			{0x66,  865},  
			{0x65,  866},  
			{0x68,  895},  
			{0x6B,  857},  
			{0xC8,  1250}, 
			{0xC9,  1251}, 
			{0x03,  1252}, 
			{0xCB,  1253}, 
			{0xCA,  1254}, 
			{0x04,  10000},
			{0x98,  10006},
			{0x96,  10007},
			{0x97,  10029}
		};
		#endregion

		#region prepare data for dbf
		private string PrepareData(string input)
		{
			var data = new StringBuilder(input);
			int pos = 0;
			while ((pos <= data.Length - 2) && (data[pos] == '0') && (data[pos + 1] != '.'))
			{
				data[pos] = ' ';
				pos++;
			}
			return data.ToString();
		}
		#endregion

		#region this
        /// <summary>
        /// Exports rendered report to a dbf file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        /// <param name="codePage">A parameter which sets a code page of the exported file.</param>
		public void ExportDbf(StiReport report, string fileName, StiDbfCodePages codePage)
		{
			ExportDbf(report, fileName, StiPagesRange.All, codePage);
		}

      
		/// <summary>
        /// Exports rendered report to a dbf file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        /// <param name="pageRange">Describes pages range of the document for the export.</param>
        /// <param name="codePage">A parameter which sets a code page of the exported file.</param>
        public void ExportDbf(StiReport report, string fileName, StiPagesRange pageRange, StiDbfCodePages codePage)
		{
			StiFileUtils.ProcessReadOnly(fileName);
			using (var stream = new FileStream(fileName, FileMode.Create))
			{
				ExportDbf(report, stream, pageRange, codePage);
				stream.Flush();
				stream.Close();
			}
		}

       
		/// <summary>
        /// Exports a rendered report to a dbf file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        public void ExportDbf(StiReport report, string fileName)
		{
			StiFileUtils.ProcessReadOnly(fileName);
			using (var stream = new FileStream(fileName, FileMode.Create))
			{
				ExportDbf(report, stream);
				stream.Flush();
				stream.Close();
			}
		}

        
    
		/// <summary>
        /// Exports a rendered report to a dbf file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for the export of a document.</param>
        public void ExportDbf(StiReport report, Stream stream)
		{			
			ExportDbf(report, stream, new StiDbfExportSettings());
		}

        
		/// <summary>
        /// Exports a rendered report to a dbf file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for the export of a document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        /// <param name="codePage">A parameter which sets a code page of the exported file.</param>
		public void ExportDbf(StiReport report, Stream stream, StiPagesRange pageRange, StiDbfCodePages codePage)
		{
            var settings = new StiDbfExportSettings
            {
                PageRange = pageRange,
                CodePage = codePage
            };

            ExportDbf(report, stream, settings);
		}

		/// <summary>
		/// Exports a rendered report to a dbf file.
		/// </summary>
        public void ExportDbf(StiReport report, Stream stream, StiDataExportSettings settings)
		{
			StiLogService.Write(this.GetType(), "Export report to Dbf format");

            #region Export Dashboard
            if (!report.IsDocument && report.GetCurrentPage() is IStiDashboard)
            {
                StiDashboardExport.Export(report, stream, settings);
                return;
            }
            #endregion

#if NETSTANDARD || NETCOREAPP
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            #region Read settings
            if (settings == null)
                throw new ArgumentNullException("The 'settings' argument cannot be equal in null.");

            var pageRange = settings.PageRange;
			var codePage = settings.CodePage;
            var mode = settings.DataExportMode;

#if NETSTANDARD || NETCOREAPP
            try
            {
				if (codePage == StiDbfCodePages.Default)
					codePage = (StiDbfCodePages)CultureInfo.CurrentCulture.TextInfo.ANSICodePage;
			}
			catch
			{
				codePage = StiDbfCodePages.WindowsANSI;
			}
#endif
            #endregion

            #region codepages
            int codePageIndex = 0;
			for (int index = 0; index < codePageCodes.Length; index++)
			{
				if (codePageCodes[index, 1] == (int)codePage)
				{
					codePageIndex = index;
					break;
				}
			}

			var enc = (codePageIndex == 0)
				? Encoding.Default.GetEncoder()
				: Encoding.GetEncoding(codePageCodes[codePageIndex, 1]).GetEncoder();
            #endregion

            var pages = pageRange.GetSelectedPages(report.RenderedPages);

            CurrentPassNumber = 0;
            MaximumPassNumber = 3;

			var matrix = new StiMatrix(pages, false, this);
			matrix.ScanComponentsPlacement(false);
			if (IsStopped)return;

			matrix.PrepareDocument(this, mode);

			#region Prepare data
			int fullFieldsLength = 0;
			for (int columnIndex = 0; columnIndex < matrix.Fields.Length; columnIndex++)
			{
				//count max line length
				int maxLen = 0;
				for (int rowIndex = 0; rowIndex < matrix.DataArrayLength; rowIndex++)
				{
					string textData = matrix.Fields[columnIndex].DataArray[rowIndex];
					if (textData == null) textData = string.Empty;
					int currentLen = textData.Length;
					if (currentLen > maxLen)
					{
						maxLen = currentLen;
					}
				}
				if (maxLen == 0) maxLen = 1;
				if ((matrix.Fields[columnIndex].Info[0] == (int)StiExportDataType.String) &&
					(matrix.Fields[columnIndex].Info[1] == 0))
				{
					matrix.Fields[columnIndex].Info[1] = maxLen;
				}
				fullFieldsLength += matrix.Fields[columnIndex].Info[1];

				//create formatting string
				var fieldFormatString = new StringBuilder();
				if ((matrix.Fields[columnIndex].Info[0] == (int)StiExportDataType.Float) ||
					(matrix.Fields[columnIndex].Info[0] == (int)StiExportDataType.Double))
				{
					int len1 = matrix.Fields[columnIndex].Info[1] - matrix.Fields[columnIndex].Info[2] - 1;
					if (len1 > 0) fieldFormatString.Append('0', len1);
					fieldFormatString.Append(".");
					fieldFormatString.Append('0', matrix.Fields[columnIndex].Info[2]);
				}
				if ((matrix.Fields[columnIndex].Info[0] == (int)StiExportDataType.Int) ||
					(matrix.Fields[columnIndex].Info[0] == (int)StiExportDataType.Long))
				{
					fieldFormatString.Append('0', matrix.Fields[columnIndex].Info[1]);
				}

				if (fieldFormatString.Length > 1)
				{
					fieldFormatString.Append(";-" + fieldFormatString.ToString().Substring(1, fieldFormatString.Length - 1));
				}
				matrix.Fields[columnIndex].FormatString = fieldFormatString.ToString();
			}
			fullFieldsLength ++;
			#endregion

			#region Check fields names
			var hs = new Hashtable();
			for (int index = 0; index < matrix.Fields.Length; index ++)
			{
				string st = matrix.Fields[index].Name.ToUpper(CultureInfo.InvariantCulture);
				string stNum = string.Empty;
				int num = 0;
				while (true)
				{
					if (!hs.Contains(st + stNum)) break;
					num ++;
					stNum = num.ToString();
					if ((st.Length + stNum.Length) > 10)
					{
						st = st.Substring(0, 10 - stNum.Length);
					}
				}
				st += stNum;
				matrix.Fields[index].Name = st;
				hs.Add(st, st);
			}
			#endregion

			#region Make header structure
			int lenBuf = 32 + matrix.Fields.Length * 32 + 1;
			byte[] buf = new byte[lenBuf];

			buf[0] = 0x03;          //dBASE III PLUS, without memo-field 
			var dt = DateTime.Now;
			buf[1] = (byte)(dt.Year - (int)(dt.Year/100)*100);	//last change (year)
			buf[2] = (byte)dt.Month;	//last change (month)
			buf[3] = (byte)dt.Day;		//last change (day)
			BitConverter.GetBytes((int)matrix.DataArrayLength).CopyTo(buf, 4);		//records in file
			BitConverter.GetBytes((short)lenBuf).CopyTo(buf, 8);	//offset of first record
			BitConverter.GetBytes((short)fullFieldsLength).CopyTo(buf, 10);	//length of one field (include del mark)
			
//			int fieldOffset = 1;
			for (int index = 0; index < matrix.Fields.Length; index ++)
			{
				int offset = 32 + index * 32;
				var st = matrix.Fields[index].Name.ToUpper(CultureInfo.InvariantCulture);

				//for (int tempIndex = 0; tempIndex < st.Length; tempIndex ++)
				//{
				//    buf[offset + tempIndex] = (byte)st[tempIndex];
				//}
				var bufSt = new byte[st.Length];
                enc.GetBytes(st.ToCharArray(), 0, st.Length, bufSt, 0, true);
                bufSt.CopyTo(buf, offset);

				buf[offset + 11] = (byte)'C';					//data type
				if ((matrix.Fields[index].Info[0] == (int)StiExportDataType.Int) ||
					(matrix.Fields[index].Info[0] == (int)StiExportDataType.Long) ||
					(matrix.Fields[index].Info[0] == (int)StiExportDataType.Float)||
					(matrix.Fields[index].Info[0] == (int)StiExportDataType.Double))
					buf[offset + 11] = (byte)'N';					//data type
				if (matrix.Fields[index].Info[0] == (int)StiExportDataType.Date)
					buf[offset + 11] = (byte)'D';					//data type
                if (matrix.Fields[index].Info[0] == (int)StiExportDataType.Bool)
                    buf[offset + 11] = (byte)'L';					//data type
                //BitConverter.GetBytes((short)fieldOffset).CopyTo(buf, offset + 12);		//field offset
				buf[offset + 16] = (byte)matrix.Fields[index].Info[1];		//field length
				buf[offset + 17] = (byte)matrix.Fields[index].Info[2];		//field decimal places
			}

			buf[29] = (byte)codePageCodes[codePageIndex, 0];
			buf[lenBuf - 1] = 0x0D;		//end of header
			#endregion

			writer2 = stream;
			writer2.Write(buf, 0, lenBuf);		//write header

			#region Write records
			buf = new byte[fullFieldsLength];

            CurrentPassNumber = 2;
			StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");
			for (int rowIndex = 0; rowIndex < matrix.DataArrayLength; rowIndex++)
			{
                InvokeExporting(rowIndex, matrix.DataArrayLength, CurrentPassNumber, MaximumPassNumber);
				if (IsStopped) return;

				int fieldOffset = 1;
				for (int columnIndex = 0; columnIndex < matrix.Fields.Length; columnIndex ++)
				{
					var text = matrix.Fields[columnIndex].DataArray[rowIndex];
					if (text == null) text = string.Empty;

					#region convert text
					int param1 = matrix.Fields[columnIndex].Info[1];
					int param2 = matrix.Fields[columnIndex].Info[2];
					switch (matrix.Fields[columnIndex].Info[0])
					{
						case (int)StiExportDataType.String:
							{
								if (text.Length > param1)
								{
									text = text.Remove(param1, text.Length - param1);
								}
								if (text.Length < param1)
								{
									text = text + new string(' ', param1 - text.Length);
								}
							}
							break;

						case (int)StiExportDataType.Int:
							{
								int val = 0;
							    if (int.TryParse(text, out val))
							    {
                                    text = PrepareData(val.ToString(matrix.Fields[columnIndex].FormatString));
                                    if (text.Length > param1)
                                    {
                                        text = text.Remove(0, text.Length - param1);
                                    }
							    }
							    else
							    {
                                    val = 0;
							    }
							}
							break;

						case (int)StiExportDataType.Long:
							{
								long val = 0;
                                if (long.TryParse(text, out val))
							    {
                                    text = PrepareData(val.ToString(matrix.Fields[columnIndex].FormatString));
                                    if (text.Length > param1)
                                    {
                                        text = text.Remove(0, text.Length - param1);
                                    }
							    }
							    else
							    {
                                    val = 0;
							    }
							}
							break;

						case (int)StiExportDataType.Float:
							{
								float val = 0;
                                if (float.TryParse(text.Replace(".", ",").Replace(",", separator), out val))
							    {
                                    text = PrepareData(val.ToString(matrix.Fields[columnIndex].FormatString).Replace(separator, "."));
                                    if (text.Length > param1)
                                    {
                                        text = text.Remove(0, text.Length - param1);
                                    }
							    }
							    else
							    {
                                    val = 0;
							    }
							}
							break;

						case (int)StiExportDataType.Double:
							{
								double val = 0;
							    if (double.TryParse(text.Replace(".", ",").Replace(",", separator), out val))
							    {
                                    text = PrepareData(val.ToString(matrix.Fields[columnIndex].FormatString).Replace(separator, "."));
                                    if (text.Length > param1)
                                    {
                                        text = text.Remove(0, text.Length - param1);
                                    }
							    }
							    else
							    {
                                    val = 0;
							    }
							}
							break;

                        case (int)StiExportDataType.Date:
                            {
                                const string emptyDateString = "        ";
                                if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text.TrimEnd()))
                                {
                                    text = emptyDateString;
                                }
                                else
                                {
                                    DateTime val;
                                    if (DateTime.TryParse(text, out val))
                                    {
                                        text = PrepareData(val.ToString("yyyyMMdd"));
                                    }
                                    else
                                    {
                                        val = DateTime.MinValue;
                                    }
                                }
                            }
                            break;

                        case (int)StiExportDataType.Bool:
                            {
                                bool val = false;
                                bool.TryParse(text, out val);
                                text = val ? "T" : "F";
                            }
                            break;
                    }
					#endregion

                    var bufSt = new byte[text.Length];
					enc.GetBytes(text.ToCharArray(), 0, text.Length, bufSt, 0, true);
					bufSt.CopyTo(buf, fieldOffset);
					for (int tempIndex = text.Length; tempIndex < matrix.Fields[columnIndex].Info[1]; tempIndex ++)
					{
						buf[fieldOffset + tempIndex] = (byte)' ';
					}
					fieldOffset += matrix.Fields[columnIndex].Info[1];
				}
				buf[0] = 0x20;	//space

				writer2.Write(buf, 0, buf.Length);	//write record
			}
			#endregion

			buf[0] = 0x1a;
			writer2.Write(buf, 0, 1);			//write end marker

			writer2.Flush();
			
			enc = null;
			if (matrix != null)
			{
				matrix.Clear();
				matrix = null;
			}
		}
		#endregion
	}
}