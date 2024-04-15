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
using System.Collections;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Class helps apply style with name to component.
    /// </summary>
    public class StiConditionHelper : StiFilter
	{
        #region Methods
        /// <summary>
        /// Apply formatting which specifed in condtition to component.
        /// </summary>
        public static void Apply(object comp, string styleName)
        {
            StiComponent component = comp as StiComponent;

            #region Set Default Style
            if (styleName.StartsWith("##") && styleName.EndsWith("##"))
            {
                styleName = styleName.Substring(2, styleName.Length - 4);
                foreach (StiStyle st in StiOptions.Designer.Styles)
                {
                    if (st.Name == styleName)
                    {
                        st.SetStyleToComponent(component);
                        ApplyParentStyle(st, component);
                        return;
                    }
                }
            }
            #endregion

            #region Report Styles
            StiBaseStyle st2 = component.Report.Styles[styleName];
            if (st2 != null)
            {
                st2.SetStyleToComponent(component);
                ApplyParentStyle(st2, component);
                return;
            }

            component.Report.WriteToReportRenderingMessages($"'{styleName}' style not found in conditions of '{component.Name}' component!");
            #endregion
        }

        private static void ApplyParentStyle(StiBaseStyle style, StiComponent comp)
        {
            if (comp == null || comp.Report == null || comp.Report.Engine == null) return;

            if (comp.Report.Engine.HashParentStyles == null)
                comp.Report.Engine.HashParentStyles = new Hashtable();

            comp.Report.Engine.HashParentStyles[comp] = style;
        }

        public static void ApplyFont(object component, Font font, StiConditionPermissions perms)
        {
            var fontComp = component as IStiFont;
            if (fontComp == null) return;

            bool fontUsed = false;
            string fontName = fontComp.Font.Name;
            float fontSize = fontComp.Font.Size;
            bool fontStyleBold = fontComp.Font.Bold;
            bool fontStyleItalic = fontComp.Font.Italic;
            bool fontStyleUnderline = fontComp.Font.Underline;
            bool fontStyleStrikeout = fontComp.Font.Strikeout;

            #region FontName
            if ((perms & StiConditionPermissions.Font) > 0)
            {
                fontUsed = true;
                fontName = font.Name;
            }
            #endregion

            #region FontSize
            if ((perms & StiConditionPermissions.FontSize) > 0)
            {
                fontUsed = true;
                fontSize = font.Size;
            }
            #endregion

            #region FontStyleBold
            if ((perms & StiConditionPermissions.FontStyleBold) > 0)
            {
                fontUsed = true;
                fontStyleBold = font.Bold;
            }
            #endregion

            #region FontStyleItalic
            if ((perms & StiConditionPermissions.FontStyleItalic) > 0)
            {
                fontUsed = true;
                fontStyleItalic = font.Italic;
            }
            #endregion

            #region FontStyleUnderline
            if ((perms & StiConditionPermissions.FontStyleUnderline) > 0)
            {
                fontUsed = true;
                fontStyleUnderline = font.Underline;
            }
            #endregion

            #region FontStyleStrikeout
            if ((perms & StiConditionPermissions.FontStyleStrikeout) > 0)
            {
                fontUsed = true;
                fontStyleStrikeout = font.Strikeout;
            }
            #endregion

            if (!fontUsed) return;

            var fontStyle = FontStyle.Regular;
            if (fontStyleBold)
                fontStyle |= FontStyle.Bold;

            if (fontStyleItalic)
                fontStyle |= FontStyle.Italic;

            if (fontStyleUnderline)
                fontStyle |= FontStyle.Underline;

            if (fontStyleStrikeout)
                fontStyle |= FontStyle.Strikeout;

            fontComp.Font = StiFontCollection.CreateFont(fontName, fontSize, fontStyle);
        }
        #endregion
    }
}
