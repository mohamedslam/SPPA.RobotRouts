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
using System.Linq;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helper;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Helpers;

namespace Stimulsoft.Report.Check
{
    public class StiComponentResourceCheck : StiComponentCheck
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
                return StiLocalizationExt.Get("CheckComponent", "StiComponentResourceCheckShort");
            }
        }

        public override string LongMessage
        {
            get
            {
                var resourceName = string.Empty;
                if (Element is StiImage) resourceName = StiHyperlinkProcessor.GetResourceNameFromHyperlink(((StiImage)Element).ImageURL.Value);
                else if (Element is StiRichText) resourceName = StiHyperlinkProcessor.GetResourceNameFromHyperlink(((StiRichText)Element).DataUrl.Value);
                else if (Element is StiSubReport) resourceName = StiHyperlinkProcessor.GetResourceNameFromHyperlink(((StiSubReport)Element).SubReportUrl);

                return string.Format(StiLocalizationExt.Get("CheckComponent", "StiComponentResourceCheckLong"), resourceName, ElementName);
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
            var image = Element as StiImage;
            if (image != null && StiHyperlinkProcessor.IsResourceHyperlink(image.ImageURL.Value))
            {
                try
                {
                    var resourceName = StiHyperlinkProcessor.GetResourceNameFromHyperlink(image.ImageURL.Value).ToLowerInvariant().Trim();
                    var resource = image.Report.Dictionary.Resources.ToList().FirstOrDefault(r => r.Name != null && r.Name.ToLowerInvariant().Trim() == resourceName);

                    return resource == null;
                }
                catch
                {
                    return true;
                }
            }

            var richText = Element as StiRichText;
            if (richText != null && StiHyperlinkProcessor.IsResourceHyperlink(richText.DataUrl.Value))
            {
                try
                {
                    var resourceName = StiHyperlinkProcessor.GetResourceNameFromHyperlink(richText.DataUrl.Value).ToLowerInvariant().Trim();
                    var resource = richText.Report.Dictionary.Resources.ToList().FirstOrDefault(r => r.Name != null && r.Name.ToLowerInvariant().Trim() == resourceName);

                    return resource == null;
                }
                catch
                {
                    return true;
                }
            }

            var subReport = Element as StiSubReport;
            if (subReport != null && StiHyperlinkProcessor.IsResourceHyperlink(subReport.SubReportUrl))
            {
                try
                {
                    var resourceName = StiHyperlinkProcessor.GetResourceNameFromHyperlink(subReport.SubReportUrl).ToLowerInvariant().Trim();
                    var resource = subReport.Report.Dictionary.Resources.ToList().FirstOrDefault(r => r.Name != null && r.Name.ToLowerInvariant().Trim() == resourceName);

                    return resource == null;
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
                    var check = new StiComponentResourceCheck();
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