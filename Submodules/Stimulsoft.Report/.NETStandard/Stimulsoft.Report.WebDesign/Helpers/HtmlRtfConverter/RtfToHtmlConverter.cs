#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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

using System.IO;
using System.Threading;
using System.Windows;

#if !NETSTANDARD
using System.Windows.Controls;
using System.Windows.Documents;
#endif

namespace Stimulsoft.Report.Web
{
    internal class RtfToHtmlConverter
    {
        private const string FlowDocumentFormat = "<FlowDocument>{0}</FlowDocument>";

        public string ConvertRtfToHtml(string rtfText)
        { 
            var thread = new Thread(ConvertRtfInSTAThread);
            var threadData = new ConvertRtfThreadData { RtfText = rtfText };
            try
            {               
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start(threadData);
                thread.Join();
            }
            catch 
            {
                return string.Empty;
            }
            return threadData.HtmlText;
        }

        private void ConvertRtfInSTAThread(object rtf)
        {
            var threadData = rtf as ConvertRtfThreadData;

            var xamlText = string.Format(FlowDocumentFormat, ConvertRtfToXaml(threadData.RtfText));
            threadData.HtmlText = HtmlFromXamlConverter.ConvertXamlToHtml(xamlText, false);
        }

        private class ConvertRtfThreadData
        {
            public string RtfText { get; set; }
            public string HtmlText { get; set; }
        }
  

        private static string ConvertRtfToXaml(string rtfText)
        {
#if NETSTANDARD
            return rtfText;
#else
            var richTextBox = new RichTextBox();
            if (string.IsNullOrEmpty(rtfText)) return "";

            var textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);

            using (var rtfMemoryStream = new MemoryStream())
            {
                using (var rtfStreamWriter = new StreamWriter(rtfMemoryStream))
                {
                    rtfStreamWriter.Write(rtfText);
                    rtfStreamWriter.Flush();
                    rtfMemoryStream.Seek(0, SeekOrigin.Begin);

                    textRange.Load(rtfMemoryStream, DataFormats.Rtf);
                }
            }

            using (var rtfMemoryStream = new MemoryStream())
            {

                textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                textRange.Save(rtfMemoryStream, DataFormats.Xaml);
                rtfMemoryStream.Seek(0, SeekOrigin.Begin);
                using (var rtfStreamReader = new StreamReader(rtfMemoryStream))
                {
                    return rtfStreamReader.ReadToEnd();
                }
            }
#endif
        }
    }
}
