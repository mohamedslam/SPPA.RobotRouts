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

using System.Collections.Generic;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helper;
using Stimulsoft.Report.Engine;

namespace Stimulsoft.Report.Check
{
    public class StiComponentDataColumnCheck : StiComponentCheck
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
                return string.Format(StiLocalizationExt.Get("CheckComponent", "StiComponentExpressionCheckLong"), "DataColumn", this.ElementName);
            }
        }

        public override StiCheckStatus Status
        {
            get
            {
                return StiCheckStatus.Warning;
            }
        }
        #endregion

        #region Methods
        private bool Check()
        {
            StiImage image = Element as StiImage;
            if (image != null && image.DataColumn != null && image.DataColumn.Length > 0)
            {
                try
                {
                    bool refStoreToPrint = false;
                    object obj = Engine.StiParser.ParseTextValue("{" + image.DataColumn + "}", image, ref refStoreToPrint, false, true);
                    List<StiParser.StiAsmCommand> list = obj as List<StiParser.StiAsmCommand>;
                    if (list != null)
                    {
                        if (list.Count > 1) return true;
                    }
                }
                catch
                {
                    return true;
                }
            }

            StiRichText richText = Element as StiRichText;
            if (richText != null && richText.DataColumn != null && richText.DataColumn.Length > 0)
            {
                try
                {
                    bool refStoreToPrint = false;
                    object obj = Engine.StiParser.ParseTextValue("{" + richText.DataColumn + "}", richText, ref refStoreToPrint, false, true);
                    List<StiParser.StiAsmCommand> list = obj as List<StiParser.StiAsmCommand>;
                    if (list != null)
                    {
                        if (list.Count > 1) return true;
                    }
                }
                catch
                {
                    return true;
                }
            }

            return false;
        }

        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;

            try
            {
                bool failed = Check();

                if (failed)
                {
                    StiComponentDataColumnCheck check = new StiComponentDataColumnCheck();
                    check.Element = obj;
                    return check;
                }
                else return null;
            }
            finally
            {
                this.Element = null;
            }
        }
        #endregion
    }
}