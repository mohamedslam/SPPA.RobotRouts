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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report
{
    public class StiReportResourcesCompiler : IDisposable
    {
        #region Fields
        private string tempFolder;
        private StiReport report;
        private bool inMemory;
        private int reportIndex = -1;
        #endregion

        #region Properties
        private List<StiResourceItem> resources;
        public List<StiResourceItem> Resources
        {
            get
            {
                return resources ?? (resources = new List<StiResourceItem>());
            }
        }

        private List<string> files;
        public List<string> Files
        {
            get
            {
                return files ?? (files = new List<string>());
            }
        }
        #endregion

        #region Methods
        private void CreateResourceFiles()
        {
            if (this.Resources != null)
                this.Resources.Clear();

            if (this.report.ReportResources != null)
                this.report.ReportResources.Clear();

            StiReportResourcesFinder.GetResourcePages(report).ForEach(p =>
            {
                CreateResourceFile(p.Name, p, p.Watermark.TakeImage(), StiReportResourcesFinder.IdentResourceWatermark);
                p.Watermark.ResetImage();
            });

            StiReportResourcesFinder.GetResourceImageComponents(report).Where(image => image != null).ToList().ForEach(image =>
            {
                #region Process Globalization
                if (!string.IsNullOrWhiteSpace(image.GlobalizedName) && image.Report != null && image.Report.GlobalizationManager != null)
                {
                    var newImage = image.Report.GlobalizationManager.GetObject(image.GlobalizedName) as Image;
                    if (newImage != null)
                    {
                        CreateResourceFile(image.Name, image, StiImageConverter.ImageToBytes(newImage), StiReportResourcesFinder.IdentResourceImage);
                        newImage.Dispose();
                        return;
                    }
                }
                #endregion

                if (!image.ExistImage()) return;

                CreateResourceFile(image.Name, image, image.TakeImage(), StiReportResourcesFinder.IdentResourceImage);
                image.ResetImage();
            });

            StiReportResourcesFinder.GetResourceVariables(report).ForEach(v =>
            {
                var valueContent = v.ValueObject is Image ? StiImageConverter.ImageToBytes(v.ValueObject as Image) : v.ValueObject as byte[];
                CreateResourceFile(v.Name, v, valueContent, StiReportResourcesFinder.IdentResourceVariable);
                v.ValueObject = null;
            });

            StiReportResourcesFinder.GetResourceReport(report).ForEach(r =>
            {
                CreateResourceFile(r.Name, r, r.Content, StiReportResourcesFinder.IdentResourceReport);
                r.Content = null;
            });
        }

        private void CreateResourceFile(string resourceName, object component, byte[] resource, string property)
        {
            var prefix = reportIndex > -1 ? reportIndex + "." : "";
            string resName = prefix + resourceName + ".res";
            this.Resources.Add(new StiResourceItem(component, resource, resName, property));

            if (inMemory) return;

            this.report.ReportResources.Add(new StiReportResource(resName, resourceName, property));

#if !NETSTANDARD && !NETCOREAPP
            var filePath = Path.Combine(this.tempFolder, prefix + resourceName) + ".res";
            this.Files.Add(filePath);
            File.WriteAllBytes(filePath, resource);
#endif
        }

        private void RestoreResourcesInReport()
        {
            if (Resources == null) return;

            foreach (var resource in Resources)
            {
                var page = resource.Component as StiPage;
                if (page != null)
                    page.Watermark.PutImage(resource.Resource);

                var image = resource.Component as StiImage;
                if (image != null)
                    image.PutImage(resource.Resource);

                var variable = resource.Component as StiVariable;
                if (variable != null)
                {
                    if (StiTypeFinder.FindType(variable.Type, typeof(Image)))
                        variable.ValueObject = StiImageConverter.BytesToImage(resource.Resource);
                    else
                        variable.ValueObject = resource.Resource;
                }

                var reportResource = resource.Component as StiResource;
                if (reportResource != null)
                    reportResource.Content = resource.Resource;
            }

            Resources.Clear();
            resources = null;
        }

        private string GetResourcesFolder()
        {
#if NETSTANDARD || NETCOREAPP
            return string.Empty;
#else
            var folder = StiOptions.Engine.ReportResources.ResourcesPath;
            if (string.IsNullOrWhiteSpace(folder)) folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Stimulsoft", "Resources");
            folder = Path.Combine(folder, StiGuidUtils.NewGuid());
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            return folder;
#endif
        }

        public void ProcessReport(StiReport report)
        {
            if (this.report != null)
                RestoreResourcesInReport();

            reportIndex++;
            this.report = report;

            if (report != null && StiReportResourcesFinder.ResourcesExits(report))
            {
                if (!inMemory && tempFolder == null)
                    tempFolder = GetResourcesFolder();

                CreateResourceFiles();
            }
        }
        #endregion

        #region IDispose
        public void Dispose()
        {
            if (report != null)
                RestoreResourcesInReport();

            if (!inMemory)
            {
                if (Directory.Exists(tempFolder))
                    Directory.Delete(tempFolder, true);
            }

            files = null;
        }
        #endregion

        public StiReportResourcesCompiler(StiReport report, bool inMemory)
        {
            this.report = report;
            this.inMemory = inMemory;

            if (report != null && StiReportResourcesFinder.ResourcesExits(report))
            {
                if (!inMemory)
                    tempFolder = GetResourcesFolder();

                CreateResourceFiles();
            }
        }
    }
}
