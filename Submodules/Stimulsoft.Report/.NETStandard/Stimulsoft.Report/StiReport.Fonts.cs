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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helpers;
using System.Collections;
using System.Linq;

namespace Stimulsoft.Report
{
    public partial class StiReport
    {
        #region Fields
        private Hashtable resourceToFontHash = new Hashtable();
        #endregion

        #region Methods
        public StiReport LoadFonts()
        {
            if (FontVHelper.LoadFontsFromResources(this))
            {
                LoadFontsForComponents();
                LoadFontsForConditions();
                LoadFontsForWatermark();
            }

            return this;
        }

        public StiReport LoadDocumentFonts()
        {
            if (FontVHelper.LoadFontsFromResources(this))
            {
                LoadDocumentFontsForComponents();
                LoadDocumentFontsForWatermark();
            }

            return this;
        }

        private void LoadFontsForComponents()
        {
            var comps = this.GetComponents()
                .ToList()
                .Where(c => c is IStiFont)
                .Cast<IStiFont>()
                .Where(c => c.Font != null && c.Font.OriginalFontName != null && c.Font.OriginalFontName != c.Font.Name)
                .ToList();

            comps.ForEach(c =>
            {
                c.Font = StiFontCollection.CreateFont(c.Font.OriginalFontName, c.Font.Size, c.Font.Style);
            });
        }

        private void LoadFontsForConditions()
        {
            var conditions = this.GetComponents()
                .ToList()
                .Where(c => (c is IStiConditions) && ((c as IStiConditions).Conditions.Count > 0))
                .Cast<IStiConditions>()
                .ToList();

            conditions.ForEach(c =>
            {
                foreach (StiBaseCondition cond in c.Conditions)
                {
                    var hc = cond as StiCondition;
                    if (hc != null && hc.Font != null && hc.Font.OriginalFontName != null && hc.Font.OriginalFontName != hc.Font.Name)
                    {
                        hc.Font = StiFontCollection.CreateFont(hc.Font.OriginalFontName, hc.Font.Size, hc.Font.Style);
                    }
                }
            });
        }

        private void LoadDocumentFontsForComponents()
        {
            for (int indexPage = 0; indexPage < RenderedPages.Count; indexPage++)
            {
                var page = RenderedPages.GetPageWithoutCache(indexPage);

                var comps = page.GetComponents()
                    .ToList()
                    .Where(c => c is IStiFont)
                    .Cast<IStiFont>()
                    .Where(c => c.Font != null && c.Font.OriginalFontName != null && c.Font.OriginalFontName != c.Font.Name)
                    .ToList();

                comps.ForEach(c =>
                {
                    c.Font = StiFontCollection.CreateFont(c.Font.OriginalFontName, c.Font.Size, c.Font.Style);
                });
            }        
        }

        private void LoadFontsForWatermark()
        {
            var watermarks = this.Pages
                .ToList()
                .Select(p => p.Watermark)
                .Where(w => w != null && w.Font.OriginalFontName != null && w.Font.OriginalFontName != w.Font.Name)
                .ToList();

            watermarks.ForEach(w =>
            {
                w.Font = StiFontCollection.CreateFont(w.Font.OriginalFontName, w.Font.Size, w.Font.Style);
            });
        }

        private void LoadDocumentFontsForWatermark()
        {
            for (int indexPage = 0; indexPage < RenderedPages.Count; indexPage++)
            {
                var page = RenderedPages.GetPageWithoutCache(indexPage);

                var w = page.Watermark;
                if (w != null && w.Font.OriginalFontName != null && w.Font.OriginalFontName != w.Font.Name)
                {
                    w.Font = StiFontCollection.CreateFont(w.Font.OriginalFontName, w.Font.Size, w.Font.Style);
                }
            }
        }

        public string GetResourceFontName(string resourceName)
        {
            string name = resourceToFontHash[resourceName] as string;
            if (name == null)
            {
                var res = this.Dictionary.Resources
                .ToList()
                .Where(r => FontVHelper.IsFont(r.Type) && r.Content != null)
                .ToList()
                .FirstOrDefault(r => r.Name == resourceName);

                if (res != null)
                {
                    var family = StiFontCollection.GetFontFamilyByContent(resourceName, res.Content, res.Type.ToString().Substring(4), res.Alias);
                    if (family != null) name = family.Name;
                    if (res.Name != res.Alias) name = res.Alias;
                }

                if (!string.IsNullOrWhiteSpace(name))
                    resourceToFontHash[resourceName] = name;
            }
            return name;
        }

        public string GetFontResourceName(string fontName)
        {
            foreach (DictionaryEntry de in resourceToFontHash.Values)
            {
                if ((string)de.Value == fontName) return de.Key as string;
            }

            string resName = null;

            this.Dictionary.Resources
                .ToList()
                .Where(r => FontVHelper.IsFont(r.Type) && r.Content != null)
                .ToList()
                .ForEach(r =>
                {
                    string name = GetResourceFontName(r.Name);
                    if (name == fontName) resName = r.Name;
                });

            resourceToFontHash[resName] = fontName;

            return resName;
        }
        #endregion
    }
}