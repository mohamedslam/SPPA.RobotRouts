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
    internal class HtmlToRtfConverter
    {        
        #region Methods

        public string ConvertHtmlToRtf(string htmlText)
        {
            var thread = new Thread(ConvertRtfInSTAThread);
            var threadData = new ConvertHtmlThreadData { HtmlText = htmlText };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(threadData);
            thread.Join();
            return threadData.RtfText;
        }

        private void ConvertRtfInSTAThread(object html)
        {
            var threadData = html as ConvertHtmlThreadData;
            var xamlText = HtmlToXamlConverter.ConvertHtmlToXaml(threadData.HtmlText);
            threadData.RtfText = ConvertXamlToRtf(xamlText);
        }

        private class ConvertHtmlThreadData
        {
            public string RtfText { get; set; }
            public string HtmlText { get; set; }
        }

        //----------
        
        private static string ConvertXamlToRtf(string xamlText)
        {
#if NETSTANDARD
            return xamlText;
#else
            var richTextBox = new RichTextBox();
            if (string.IsNullOrEmpty(xamlText)) return "";

            var textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);

            using (var xamlMemoryStream = new MemoryStream())
            {
                using (var xamlStreamWriter = new StreamWriter(xamlMemoryStream))
                {
                    xamlStreamWriter.Write(xamlText);
                    xamlStreamWriter.Flush();
                    xamlMemoryStream.Seek(0, SeekOrigin.Begin);

                    textRange.Load(xamlMemoryStream, DataFormats.Xaml);
                }
            }

            using (var rtfMemoryStream = new MemoryStream())
            {

                textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                textRange.Save(rtfMemoryStream, DataFormats.Rtf);
                rtfMemoryStream.Seek(0, SeekOrigin.Begin);
                using (var rtfStreamReader = new StreamReader(rtfMemoryStream))
                {
                    return rtfStreamReader.ReadToEnd();
                }
            }
#endif
        }
        #endregion
    }
}
