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

using System.Reflection;

namespace Stimulsoft.Base.Helpers
{
    public static class StiMathHelper
    {
        #region Const
        public static string NameAssembly = "Stimulsoft.MathFX";
        #endregion

        #region Fields.Static
        private static object lockObject = new object();
        private static bool tryToLoadAssembly = false;
        #endregion

        #region Properties.Status
        private static Assembly mathAssembly;
        public static Assembly MathAssembly
        {
            get
            {
                lock (lockObject)
                {
                    if (mathAssembly == null && !tryToLoadAssembly)
                    {
                        try
                        {
                            mathAssembly = StiAssemblyFinder.GetAssembly($"{NameAssembly}.dll");
                            tryToLoadAssembly = true;
                        }
                        catch
                        {
                            return null;
                        }
                    }
                    return mathAssembly;
                }
            }
        }

        public static bool IsMathAssemblyLoaded
        {
            get
            {
                return mathAssembly != null;
            }
        }
        #endregion

        #region Methods.Static

        public static string GetMatmML(string latextContent)
        {
            var latexService = Reflection.Create(MathAssembly, $"{NameAssembly}.StiLatexService", new object[] { latextContent });
            return Reflection.InvokeMethod<string>(latexService, "GetMathML");
        }

        public static string GetSvg(string matmMLContent, float fontSize, string colorHex)
        {
            var mathService = Reflection.Create(MathAssembly, $"{NameAssembly}.StiMathMLService", new object[] { matmMLContent });
            return Reflection.InvokeMethod<string>(mathService, "GetSVG", new object[] { fontSize, colorHex });
        }
        #endregion
    }
}