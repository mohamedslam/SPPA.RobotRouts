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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Localization;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Helper
{
	public sealed class StiLocExt
	{
        public static void Init()
        {            
        }

        private static string GetCategory(Type type)
        {
            var category = type.ToString();
            var index = category.LastIndexOf(".", StringComparison.Ordinal);

            return index == -1 
                ? type.ToString() 
                : category.Substring(index + 1);
        }

        private static void ThisProcessDescription(object sender, StiProcessDescriptionEventArgs e)
        {
            var type = e.ComponentType;

            #region Font
            if (type == typeof(Font))
            {
                switch (e.Name)
                {
                    case "Name":
                        e.Description = StiLocalizationExt.Get("Font", "Name", false);
                        return;

                    case "Size":
                        e.Description = StiLocalizationExt.Get("Font", "Size", false);
                        return;

                    case "Bold":
                        e.Description = StiLocalizationExt.Get("Font", "Bold", false);
                        return;

                    case "Italic":
                        e.Description = StiLocalizationExt.Get("Font", "Italic", false);
                        return;

                    case "Strikeout":
                        e.Description = StiLocalizationExt.Get("Font", "Strikeout", false);
                        return;

                    case "Underline":
                        e.Description = StiLocalizationExt.Get("Font", "Underline", false);
                        return;
                }
            }
            #endregion

            #region Size
            if (type == typeof(Stimulsoft.Base.Drawing.SizeD) || type == typeof(Size))
            {
                switch (e.Name)
                {
                    case "Width":
                        e.Description = StiLocalizationExt.Get("StiComponent", "Width", false);
                        return;

                    case "Height":
                        e.Description = StiLocalizationExt.Get("StiComponent", "Height", false);
                        return;
                }
            }
            #endregion

            while (true)
            {
                if (type == typeof(object)) return;

                var category = GetCategory(type);
                
                var desc = StiLocalizationExt.Get(category, e.Name, false);
                if (desc != null)
                {
                    e.Description = desc;
                    return;
                }

                type = type.BaseType;
            }
        }

        static StiLocExt()
        {
            StiPropertyDescriptor.ProcessDescription += ThisProcessDescription;
        }        
	}
}