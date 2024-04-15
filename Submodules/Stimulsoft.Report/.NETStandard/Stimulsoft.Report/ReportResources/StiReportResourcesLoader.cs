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
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Helpers;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report
{
    public static class StiReportResourcesLoader
    {
        #region Methods
        public static void LoadReportResourcesFromAssembly(StiReport report)
        {
            if (report.ReportResources == null) return;

            var a = report.GetType().Assembly;

            foreach (StiReportResource resource in report.ReportResources)
            {
                var stream = a.GetManifestResourceStream(resource.Resource);
                if (stream != null)
                {
                    var content = new byte[stream.Length];
                    stream.Read(content, 0, (int) stream.Length);

                    if (LoadPageWatermarkImageFromContent(report, resource, content)) continue;
                    if (LoadVariableImageFromContent(report, resource, content)) continue;
                    if (LoadReportResourceFromContent(report, resource, content)) continue;
                }
            }
        }
        
        public static void LoadReportResourcesFromReport(StiReport source, StiReport dest)
        {
            StiReportResourcesFinder.GetResourcePages(source).ForEach(p =>
            {
                var page = dest.Pages[p.Name];
                if (page != null)
                    page.Watermark.PutImage(p.Watermark.TakeImage().Clone() as byte[]);//Image should be cloned!
            });

            StiReportResourcesFinder.GetResourceImageComponents(source).ForEach(i =>
            {
                var image = dest.GetComponentByName(i.Name) as StiImage;
                if (image != null && i.ExistImage())
                    image.PutImage(i.TakeImage().Clone() as byte[]);//Image should be cloned!
            });

            StiReportResourcesFinder.GetResourceVariables(source).ForEach(v =>
            {
                var image = v.Value;

                var variable = dest.Dictionary.Variables[v.Name];
                if (variable == null)
                {
                    dest.Dictionary.Variables.Add(new StiVariable(null, v.Name, image));
                    return;
                }

                variable.Value = image;
                
                if (!dest.CheckNeedsCompiling())
                    dest[v.Name] = variable.ValueObject;
            });

            StiReportResourcesFinder.GetResourceReport(source).ForEach(r =>
            {
                var content = StiArrayCloner.Clone(r.Content);//Array should be cloned!

                var resource = dest.Dictionary.Resources[r.Name];
                if (resource != null)
                    resource.Content = content;
                else
                    dest.Dictionary.Resources.Add(new StiResource(r.Name, r.Type, content));
            });
        }

        private static bool LoadVariableImageFromContent(StiReport report, StiReportResource resource, object content)
        {
            StiVariable variable = report.Dictionary.Variables[resource.Component];
            if (variable != null)
            {
                variable.ValueObject = content as byte[];
                if (!report.CheckNeedsCompiling())
                    report[resource.Component] = variable.ValueObject as Image;
                return true;
            }
            return false;
        }

        private static bool LoadPageWatermarkImageFromContent(StiReport report, StiReportResource resource, byte[] content)
        {
            var component = report.GetComponentByName(resource.Component);
            if (component != null)
            {
                var page = component as StiPage;
                if (page != null && resource.Property == StiReportResourcesFinder.IdentResourceWatermark)
                    page.Watermark.PutImage(content);

                var imageComp = component as StiImage;
                if (imageComp != null && resource.Property == StiReportResourcesFinder.IdentResourceImage)
                    imageComp.PutImage(content);

                return true;
            }
            return false;
        }

        private static bool LoadReportResourceFromContent(StiReport report, StiReportResource resource, object content)
        {
            var dictionaryResource = report.Dictionary.Resources[resource.Component];
            if (dictionaryResource != null)
            {
                dictionaryResource.Content = content as byte[];
                return true;
            }
            return false;
        }
        #endregion		
    }
}