#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports 									            }
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System;

namespace Stimulsoft.Base.Drawing
{
    /// <summary>
    /// This class helps in creation new object of call type.
    /// </summary>
    internal static class StiBrushCreator
    {
        #region Methods

        public static StiBrush New(string identName)
        {
            if (string.IsNullOrEmpty(identName)) return null;

            StiBrushIdent ident;
            return Enum.TryParse(identName, true, out ident) ? New(ident) : null;
        }

        public static StiBrush New(StiBrushIdent ident)
        {
            switch (ident)
            {
                case StiBrushIdent.Empty:
                    return new StiEmptyBrush();

                case StiBrushIdent.Solid:
                    return new StiSolidBrush();

                case StiBrushIdent.Gradient:
                    return new StiGradientBrush();

                case StiBrushIdent.Glare:
                    return new StiGlareBrush();

                case StiBrushIdent.Glass:
                    return new StiGlassBrush();

                case StiBrushIdent.Hatch:
                    return new StiHatchBrush();

                default:
                    throw new NotSupportedException();
            }
        }
        #endregion
    }
}
