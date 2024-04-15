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
using System.Collections.Generic;
using Stimulsoft.Report.Helper;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
using Stimulsoft.Base;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Brushes = Stimulsoft.Drawing.Brushes;
using Image = Stimulsoft.Drawing.Image;
using Pen = Stimulsoft.Drawing.Pen;
using Pens = Stimulsoft.Drawing.Pens;
#endif

namespace Stimulsoft.Report.Check
{
    public class StiCompilationErrorCheck : StiReportCheck
    {
        #region Properties
        public override bool PreviewVisible => true;

        public CompilerError Error { get; set; }

        public string ComponentName { get; set; }

        public string PropertyName { get; set; }

        public override string ShortMessage
        {
            get
            {
                return Error == null 
                    ? StiLocalizationExt.Get("CheckReport", "StiCompilationErrorCheckShort") 
                    : string.Format(StiLocalizationExt.Get("CheckReport", "StiCompilationErrorCheckShort"), this.ElementName, Error.ErrorText);
            }
        }

        public override string LongMessage
        {
            get
            {
                if (Error == null)
                    return StiLocalizationExt.Get("CheckReport", "StiCompilationErrorCheckLong");

                string message;

                if (ComponentName == null)
                    message = string.Format(StiLocalizationExt.Get("CheckReport", "StiCompilationErrorCheckLong"), this.ElementName);

                else
                {
                    if (PropertyName.EndsWith("Event"))
                        message = string.Format(StiLocalizationExt.Get("CheckReport", "StiCompilationErrorCheck3Long"), this.ElementName, this.PropertyName.Substring(0, this.PropertyName.Length - "Event".Length), this.ComponentName);

                    else
                        message = string.Format(StiLocalizationExt.Get("CheckReport", "StiCompilationErrorCheck2Long"), this.ElementName, this.PropertyName, this.ComponentName);
                }

                return $"{message}\n{Error.ErrorText}";
            }
        }
        
        public override StiCheckStatus Status
        {
            get
            {
                return Error != null && Error.IsWarning 
                    ? StiCheckStatus.Warning 
                    : StiCheckStatus.Error;
            }
        }
        #endregion

        #region Methods
        public override void CreatePreviewImage(out Image elementImage, out Image highlightedElementImage, bool useScale = false)
        {
            var report = this.Element as StiReport;

            if (ComponentName != null)
            {
                var comp = report.GetComponentByName(ComponentName);
                if (comp != null)
                {
                    this.Element = comp;
                    base.CreatePreviewImage(out elementImage, out highlightedElementImage, useScale);
                    this.Element = report;
                    return;
                }
            }

            string storedScript = report.Script;
            report.ScriptUnpack();
            string text = report.Script;
            report.Script = storedScript;

            int currentLine = 0;
            int positionInText = 0;

            #region Search position in text
            if (currentLine != Error.Line)
            {
                while (positionInText < text.Length)
                {
                    char c = text[positionInText];
                    if (c == (char)13)
                    {
                        currentLine++;
                        if (currentLine == Error.Line)
                            break;
                    }
                    positionInText++;
                }
            }
            if (positionInText == text.Length) positionInText--;
            #endregion

            #region Search top position in text
            int positionInTextTop = positionInText;
            if (positionInTextTop != 0)
            {
                int count = 12;
                while (true)
                {
                    positionInTextTop--;
                    if (positionInTextTop == 0)
                        break;
                    char c = text[positionInTextTop];
                    if (c == (char)13)
                    {
                        count--;
                        if (count == 0)
                            break;
                    }
                }
            }
            #endregion

            #region Search bottom position in text
            int positionInTextBottom = positionInText;
            if (positionInTextBottom != text.Length - 1)
            {
                int count = 11;
                while (true)
                {
                    positionInTextBottom++;
                    if (positionInTextBottom == text.Length - 1)
                        break;
                    char c = text[positionInTextBottom];
                    if (c == (char)13)
                    {
                        count--;
                        if (count == 0)
                            break;
                    }
                }
            }
            #endregion

            float fontSize = (useScale) ? 16 : 8;
            using (var font = new Font("Arial", fontSize))
            {
                float fontHeight = font.GetHeight();

                int viewWidth = 500;
                int viewHeight = (int)(24 * fontHeight);
                if (useScale)
                {
                    viewWidth *= 2;
                    viewHeight *= 2;
                }

                string str = text.Substring(positionInTextTop, positionInTextBottom - positionInTextTop);

                #region Prepare element image
                var bmp = new Bitmap(viewWidth, viewHeight);
                using (var g = Graphics.FromImage(bmp))
                {                    
                    SizeF size = g.MeasureString(str, font);

                    float posY = (viewHeight - size.Height) / 2 - fontHeight / 2;

                    g.Clear(Color.White);
                    g.DrawString(str, font, Brushes.Black, new PointF(0, posY));
                    g.DrawRectangle(Pens.Gray, new Rectangle(0, 0, bmp.Width - 1, bmp.Height - 1));
                }
                elementImage = bmp;
                #endregion

                #region Prepare highlighted element image
                var image = bmp.Clone() as Image;
                using (var g = Graphics.FromImage(image))
                using (var pen = new Pen(Color.FromArgb(0x77ff0000)))
                {
                    pen.Width = 3;
                    g.DrawRectangle(pen, 2, image.Height / 2 - fontHeight / 2, image.Width - 4, fontHeight);
                }

                highlightedElementImage = image;
                #endregion
            }
        }

        private List<int> SplitScriptToLines(string script)
        {
            var lines = new List<int>();
            int pos = 0;
            while (pos < script.Length)
            {
                lines.Add(pos);
                for (; pos < script.Length; pos++)
                {
                    char ch = script[pos];
                    if (ch == '\r' || ch == '\n')
                    {
                        if (pos + 1 < script.Length)
                        {
                            char ch2 = script[pos + 1];
                            if ((ch2 == '\r' || ch2 == '\n') && (ch2 != ch))
                            {
                                pos++;
                            }
                        }
                        break;
                    }
                }
                if (pos < script.Length) pos++;
            }
            lines.Add(pos);
            return lines;
        }

        private string[] FindCheckerInfo(StiReport report, string script, List<int> lines, CompilerError error)
        {
            string commentString = 
                report.ScriptLanguage == StiReportLanguageType.CSharp ||
                report.ScriptLanguage == StiReportLanguageType.JS
                ? "// " : "'";

            string checkerString = commentString + CodeDom.StiCodeDomSerializator.StiCheckerInfoString;
            int index = error.Line - 1;
            while (index > 0 && index < lines.Count)
            {
                var startIndex = lines[index];
                var length = index + 1 < lines.Count ? lines[index + 1] - lines[index] : -1;
                if (length != -1 && startIndex >= 0 && startIndex < script.Length && (startIndex + length) < script.Length)
                {
                    var st = script.Substring(startIndex, length).Trim();
                    if (st.StartsWith(commentString))
                    {
                        if (st.StartsWith(checkerString))
                        {
                            st = st.Substring(checkerString.Length).Trim();
                            int pos2 = st.IndexOf(' ');
                            if (pos2 != -1)
                            {
                                string[] arr = new string[2];
                                arr[0] = st.Substring(0, pos2);
                                arr[1] = st.Substring(pos2).Trim();
                                if (arr[0] == "*None*") return null;
                                return arr;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                index--;
            }
            return null;
        }

        public override object ProcessCheck(StiReport report, object obj)
        {
            if (report.CalculationMode == StiCalculationMode.Interpretation) return null;

            this.Element = obj;

            if (report.CompilerResults != null)
            {
                string storedScript = report.Script;
                report.ScriptUnpack();
                string script = report.Script.Replace("+\r\n", "+ ");
                report.Script = storedScript;
                var lines = SplitScriptToLines(script);

                var checks = new List<StiCheck>();
                foreach (CompilerError error in report.CompilerResults.Errors)
                {
                    var check = new StiCompilationErrorCheck
                    {
                        Element = obj,
                        Error = error
                    };

                    var arr = FindCheckerInfo(report, script, lines, error);
                    if (arr != null)
                    {
                        check.PropertyName = arr[0];
                        check.ComponentName = arr[1];
                        check.Actions.Add(new StiEditPropertyAction());
                    }

                    checks.Add(check);

                    if (report.Designer != null)
                        check.Actions.Add(new StiGoToCodeAction());
                }
                return checks;
            }

            #region Check ReferencedAssemblies
            if (report.CompilerResults == null && report.CompiledReport == null && report.ReferencedAssemblies != null)
            {
                var checks = new List<StiCheck>();

                foreach (string referencedAssembly in report.ReferencedAssemblies)
                {
                    if (referencedAssembly == null || referencedAssembly.Trim().Length == 0) continue;

                    var refAssemblyStr = referencedAssembly.Trim().ToLowerInvariant();

                    if (refAssemblyStr == "stimulsoft.controls.dll") continue;
                    if (refAssemblyStr == "stimulsoft.base.dll") continue;
                    if (refAssemblyStr == "stimulsoft.report.dll") continue;

                    //Remove reference to old dlls
                    if (refAssemblyStr == "stimulsoft.report.design.dll") continue;
                    if (refAssemblyStr == "stimulsoft.report.odbcdatabase.dll") continue;
                    if (refAssemblyStr == "stimulsoft.report.oracledatabase.dll") continue;

#if NETSTANDARD
                    if (refAssemblyStr.StartsWith("system.windows.")) continue;
#endif

                    try
                    {
                        var a = StiAssemblyFinder.GetAssembly(referencedAssembly);
                        if (a == null)
                            Assembly.Load(referencedAssembly); //fix after Assembly.LoadWithPartialName
                    }
                    catch (Exception ex)
                    {
                        var fileEx = ex as FileNotFoundException;
                        if (fileEx != null)
                        {
                            var message = StiLocalizationExt.Get("CheckReport", "StiCompilationErrorAssemblyCheckLong");
                            checks.Add(new StiCompilationErrorCheck
                            {
                                Element = report,
                                Error = new CompilerError("", 0, 0, "", string.Format(message, fileEx.FileName))
                            });
                        }
                    }
                }

                if (checks.Count > 0)
                    return checks;
            }
            #endregion

            return null;
        }
        #endregion
    }
}
