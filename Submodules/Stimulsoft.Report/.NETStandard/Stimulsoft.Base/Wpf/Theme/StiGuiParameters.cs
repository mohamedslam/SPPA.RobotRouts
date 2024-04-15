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
using System.IO;

namespace Stimulsoft.Base.Wpf
{
    public class StiGuiParameters
    {
        #region class StiGuiParametersInternal
        private class StiOldGuiParametersInternal
        {
            public StiWpfOffice2013Theme Theme { get; set; }

            public StiSkinForeground Style { get; set; }
        }

        private class StiGuiParametersInternal
        {
            public StiSkinBackground Background { get; set; }

            public StiSkinForeground Style { get; set; }

            public bool IsDark { get; set; }
        }
        #endregion

        #region Consts
        public const StiSkinForeground DefaultForeground = StiSkinForeground.Blue;

        public const StiSkinBackground DefaultBackground = StiSkinBackground.White;
        #endregion

        #region Properties
        public StiSkinForeground Style { get; private set; }

        public StiSkinBackground Background { get; private set; }
        #endregion

        #region Methods
        public static string GetDefaultGuiParametersPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Stimulsoft", "Stimulsoft.Designer.GUI.json");
        }

        public static void SaveParameters(StiGuiParameters parameters)
        {
            try
            {
                var path = GetDefaultGuiParametersPath();

                var content = parameters.SaveToString();
                File.WriteAllText(path, content);
            }
            catch { }
        }

        public static StiGuiParameters LoadParameters()
        {
            try
            {
                var path = GetDefaultGuiParametersPath();
                if (!File.Exists(path)) return null;

                var content = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(content)) return null;

                var parameters = new StiGuiParameters();
                parameters.LoadFromString(content);
                return parameters;
            }
            catch { }

            return null;
        }

        public static StiGuiParameters LoadParametersOrDefault()
        {
            var parameters = LoadParameters();
            return parameters != null ? parameters : new StiGuiParameters();
        }

        public static void SaveThemeAndStyle(StiSkinForeground style, StiSkinBackground background)
        {
            var parameters = new StiGuiParameters
            {
                Style = style,
                Background = background
            };

            SaveParameters(parameters);
        }

        public string SaveToString()
        {
            return StiJsonHelper.SaveToJsonString(this);
        }

        public void LoadFromString(string str)
        {
            if (string.IsNullOrEmpty(str)) return;

            if (str.Contains("Office2013"))
            {
                var parameters = new StiOldGuiParametersInternal();
                StiJsonHelper.LoadFromJsonString(str, parameters);

                this.Style = parameters.Style;
                this.Background = ConvertToSkinBackground(parameters.Theme);
            }
            else
            {
                var parameters = new StiGuiParametersInternal();
                StiJsonHelper.LoadFromJsonString(str, parameters);

                this.Style = parameters.Style;
                this.Background = parameters.Background;
            }
        }

        private static StiSkinBackground ConvertToSkinBackground(StiWpfOffice2013Theme theme)
        {
            switch (theme)
            {
                case StiWpfOffice2013Theme.Office2013White:
                    return StiSkinBackground.White;
                case StiWpfOffice2013Theme.Office2013LightGray:
                    return StiSkinBackground.LightGray;
                case StiWpfOffice2013Theme.Office2013VeryDarkGray:
                    return StiSkinBackground.Black;

                default:
                    return StiSkinBackground.Black;
            }
        }
        #endregion

        public StiGuiParameters()
        {
            this.Style = DefaultForeground;
            this.Background = DefaultBackground;
        }
    }
}