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

using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helper;
using System.Xml;

namespace Stimulsoft.Report.Check
{
    public class StiComponentExpressionCheck : StiComponentCheck
    {
        #region Properties
        public override bool PreviewVisible
        {
            get
            {
                return true;
            }
        }

        public override string ShortMessage
        {
            get
            {
                return StiLocalizationExt.Get("CheckComponent", "StiComponentExpressionCheckShort");
            }
        }

        public override string LongMessage
        {
            get
            {
                return string.Format(StiLocalizationExt.Get("CheckComponent", "StiComponentExpressionCheckLong") + " " + Message, PropertyName, this.ElementName);
            }
        }

        public override StiCheckStatus Status
        {
            get
            {
                return StiCheckStatus.Warning;
            }
        }

        public string ComponentName = null;

        public string PropertyName = null;

        public string Message = null;
        #endregion

        #region Methods
        private string CheckExpression(StiReport report, StiComponent comp, string expression)
        {
            var result = Engine.StiParser.CheckExpression(expression, comp);
            if (result != null)
            {
                return result.Message;
            }
            return null;
        }

        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;

            try
            {
                if ((report != null) && (report.CalculationMode != StiCalculationMode.Interpretation)) return null;

                string result;

                StiText stiText = obj as StiText;
                if (stiText != null)
                {
                    result = CheckExpression(report, stiText, stiText.Text);
                    if (result != null)
                    {
                        StiComponentExpressionCheck check = new StiComponentExpressionCheck();
                        check.Element = obj;
                        check.PropertyName = "Text";
                        check.ComponentName = stiText.Name;
                        check.Message = result;
                        check.Actions.Add(new StiEditPropertyAction());
                        return check;
                    }
                }

                StiRichText richText = obj as StiRichText;
                if (richText != null)
                {
                    string textToParse = XmlConvert.DecodeName(richText.Text.Value).Replace((char)0, ' ');
                    result = CheckExpression(report, richText, textToParse);
                    if (result != null)
                    {
                        StiComponentExpressionCheck check = new StiComponentExpressionCheck();
                        check.Element = obj;
                        check.PropertyName = "Text";
                        check.ComponentName = richText.Name;
                        check.Message = result;
                        check.Actions.Add(new StiEditPropertyAction());
                        return check;
                    }
                }

                return null;
            }
            finally
            {
                this.Element = null;
            }
        }
        #endregion
    }
}