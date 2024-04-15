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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Helpers;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Class helps in working with Icon Set images.
    /// </summary>
    public static class StiIconSetHelper
    {
        #region Methods
        /// <summary>
        /// Returns array of icon keys for specified Icon Set.
        /// </summary>
        /// <param name="iconSet">Specified Icon Set.</param>
        /// <returns>Array of icon keys.</returns>
        public static StiIcon[] GetIconSet(StiIconSet iconSet)
        {
            switch (iconSet)
            {
                case StiIconSet.ArrowsColored3:
                    return new[] { StiIcon.ArrowUpGreen, StiIcon.ArrowRightYellow, StiIcon.ArrowDownRed };

                case StiIconSet.ArrowsColored4:
                    return new[] { StiIcon.ArrowUpGreen, StiIcon.ArrowRightUpYellow, StiIcon.ArrowRightDownYellow, StiIcon.ArrowDownRed };

                case StiIconSet.ArrowsColored5:
                    return new[] { StiIcon.ArrowUpGreen, StiIcon.ArrowRightUpYellow, StiIcon.ArrowRightYellow, StiIcon.ArrowRightDownYellow, StiIcon.ArrowDownRed };

                case StiIconSet.ArrowsGray3:
                    return new[] { StiIcon.ArrowUpGray, StiIcon.ArrowRightGray, StiIcon.ArrowDownGray };

                case StiIconSet.ArrowsGray4:
                    return new[] { StiIcon.ArrowUpGray, StiIcon.ArrowRightUpGray, StiIcon.ArrowRightDownGray, StiIcon.ArrowDownGray };

                case StiIconSet.ArrowsGray5:
                    return new[] { StiIcon.ArrowUpGray, StiIcon.ArrowRightUpGray, StiIcon.ArrowRightGray, StiIcon.ArrowRightDownGray, StiIcon.ArrowDownGray };

                case StiIconSet.Flags3:
                    return new[] { StiIcon.FlagGreen, StiIcon.FlagYellow, StiIcon.FlagRed };

                case StiIconSet.Latin4:
                    return new[] { StiIcon.Latin1, StiIcon.Latin2, StiIcon.Latin3, StiIcon.Latin4 };

                case StiIconSet.Quarters5:
                    return new[] { StiIcon.QuarterFull, StiIcon.QuarterThreeFourth, StiIcon.QuarterHalf, StiIcon.QuarterQuarter, StiIcon.QuarterNone };

                case StiIconSet.QuartersGreen5:
                    return new[] { StiIcon.QuarterFullGreen, StiIcon.QuarterThreeFourthGreen, StiIcon.QuarterHalfGreen, StiIcon.QuarterQuarterGreen, StiIcon.QuarterNoneGreen };

                case StiIconSet.QuartersRed5:
                    return new[] { StiIcon.QuarterFullRed, StiIcon.QuarterThreeFourthRed, StiIcon.QuarterHalfRed, StiIcon.QuarterQuarterRed, StiIcon.QuarterNoneRed };

                case StiIconSet.Ratings3:
                    return new[] { StiIcon.Rating4, StiIcon.Rating2, StiIcon.Rating0};

                case StiIconSet.Ratings4:
                    return new[] { StiIcon.Rating4, StiIcon.Rating3, StiIcon.Rating2, StiIcon.Rating1 };

                case StiIconSet.Ratings5:
                    return new[] { StiIcon.Rating4, StiIcon.Rating3, StiIcon.Rating2, StiIcon.Rating1, StiIcon.Rating0 };

                case StiIconSet.RedToBlack4:
                    return new[] { StiIcon.FromRedToBlackRed, StiIcon.FromRedToBlackPink, StiIcon.FromRedToBlackGray, StiIcon.QuarterFull };

                case StiIconSet.Signs3:
                    return new[] { StiIcon.CircleGreen, StiIcon.TriangleYellow, StiIcon.RhombRed };

                case StiIconSet.Squares5:
                    return new[] { StiIcon.Square0, StiIcon.Square1, StiIcon.Square2, StiIcon.Square3, StiIcon.Square4 };

                case StiIconSet.Stars3:
                    return new[] { StiIcon.StarFull, StiIcon.StarHalf, StiIcon.StarNone };

                case StiIconSet.Stars5:
                    return new[] { StiIcon.StarFull, StiIcon.StarThreeFourth, StiIcon.StarHalf, StiIcon.StarQuarter, StiIcon.StarNone };

                case StiIconSet.SymbolsCircled3:
                    return new[] { StiIcon.CircleCheckGreen, StiIcon.CircleExclamationYellow, StiIcon.CircleCrossRed };

                case StiIconSet.SymbolsUncircled3:
                    return new[] { StiIcon.CheckGreen, StiIcon.ExclamationYellow, StiIcon.CrossRed };

                case StiIconSet.TrafficLights4:
                    return new[] { StiIcon.CircleGreen, StiIcon.CircleYellow, StiIcon.CircleRed, StiIcon.CircleBlack };

                case StiIconSet.TrafficLightsRimmed3:
                    return new[] { StiIcon.LightsGreen, StiIcon.LightsYellow, StiIcon.LightsRed};

                case StiIconSet.TrafficLightsUnrimmed3:
                    return new[] { StiIcon.CircleGreen, StiIcon.CircleYellow, StiIcon.CircleRed };

                case StiIconSet.Triangles3:
                    return new[] { StiIcon.TriangleGreen, StiIcon.MinusYellow, StiIcon.TriangleRed };

                case StiIconSet.Full:
                    return new[] {
                        
                        StiIcon.ArrowUpGreen,
                        StiIcon.ArrowRightYellow,
                        StiIcon.ArrowDownRed,

                        StiIcon.ArrowUpGray,
                        StiIcon.ArrowRightGray,
                        StiIcon.ArrowDownGray,

                        StiIcon.ArrowRightUpYellow,
                        StiIcon.ArrowRightDownYellow,

                        StiIcon.ArrowRightUpGray,
                        StiIcon.ArrowRightDownGray,
                                                                        
                        StiIcon.TriangleGreen,
                        StiIcon.MinusYellow,
                        StiIcon.TriangleRed,

                        StiIcon.FlagGreen,                        
                        StiIcon.FlagYellow,
                        StiIcon.FlagRed,

                        StiIcon.Latin1,
                        StiIcon.Latin2,
                        StiIcon.Latin3,
                        StiIcon.Latin4,

                        StiIcon.CheckGreen,
                        StiIcon.ExclamationYellow,
                        StiIcon.CrossRed,        

                        StiIcon.CircleCheckGreen,
                        StiIcon.CircleExclamationYellow,
                        StiIcon.CircleCrossRed,
                                                
                        StiIcon.CircleGreen,                        
                        StiIcon.CircleYellow,
                        StiIcon.CircleRed,
                        StiIcon.CircleBlack,
                        StiIcon.TriangleYellow,
                        StiIcon.RhombRed,
                                
                        StiIcon.FromRedToBlackRed,        
                        StiIcon.FromRedToBlackPink,
                        StiIcon.FromRedToBlackGray,

                        StiIcon.LightsGreen,                        
                        StiIcon.LightsYellow,                        
                        StiIcon.LightsRed,

                        StiIcon.QuarterFull,
                        StiIcon.QuarterThreeFourth,
                        StiIcon.QuarterHalf,
                        StiIcon.QuarterQuarter,
                        StiIcon.QuarterNone,

                        StiIcon.QuarterFullGreen,
                        StiIcon.QuarterThreeFourthGreen,
                        StiIcon.QuarterHalfGreen,
                        StiIcon.QuarterQuarterGreen,
                        StiIcon.QuarterNoneGreen,

                        StiIcon.QuarterFullRed,
                        StiIcon.QuarterThreeFourthRed,
                        StiIcon.QuarterHalfRed,
                        StiIcon.QuarterQuarterRed,
                        StiIcon.QuarterNoneRed,
                                                
                        StiIcon.Rating0,
                        StiIcon.Rating1,
                        StiIcon.Rating2,
                        StiIcon.Rating3,
                        StiIcon.Rating4,
                        
                        StiIcon.Square0,
                        StiIcon.Square1,
                        StiIcon.Square2,
                        StiIcon.Square3,
                        StiIcon.Square4,
                        
                        StiIcon.StarFull,
                        StiIcon.StarThreeFourth,    
                        StiIcon.StarHalf,                        
                        StiIcon.StarQuarter,        
                        StiIcon.StarNone                                                    
                        
                    };

                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns aray of icon images for specified Icon Set.
        /// </summary>
        /// <param name="iconSet">Specified Icon Set.</param>
        /// <returns>Array of icon images.</returns>
        public static Image[] GetIcons(StiIconSet iconSet)
        {
            var icons = GetIconSet(iconSet);
            var images = new Image[icons.Length];
            var index = 0;
            foreach (var icon in icons)
            {
                images[index] = GetIcon(icon);
                index++;
            }
            return images;
        }

        /// <summary>
        /// Returns image for specified icon key.
        /// </summary>
        /// <param name="icon">Specified icon key.</param>
        /// <returns>Image for specified icon key.</returns>
        public static Image GetIcon(StiIcon icon)
        {
            return icon == StiIcon.None ? null : GetIcon(icon.ToString());
        }

        /// <summary>
        /// Returns image for specified indicator.
        /// </summary>
        /// <param name="icon">Specified indicator.</param>
        /// <returns>Image for specified indicator.</returns>
        public static Image GetIcon(StiIconSetIndicator indicator)
        {
            if (indicator.CustomIcon != null)
            {
                try
                {
                    if (StiSvgHelper.IsSvg(indicator.CustomIcon))
                    {
                        var svgDoc = StiSvgHelper.OpenSvg(indicator.CustomIcon);
                        var svgSize = StiSvgHelper.GetSvgSize(svgDoc);
                        return StiSvgHelper.ConvertSvgToImage(indicator.CustomIcon, (int)svgSize.Width, (int)svgSize.Height, false, true);
                    }
                    var stream = new MemoryStream(indicator.CustomIcon);
                    return Image.FromStream(stream);
                }
                catch (Exception) { }                
            }

            return indicator.Icon == StiIcon.None ? null : GetIcon(indicator.Icon.ToString());
        }

        /// <summary>
        /// Returns icon image with specified name.
        /// </summary>
        /// <param name="name">Icon name.</param>
        /// <returns>Icon image for specified name.</returns>
        private static Image GetIcon(string name)
        {
            return StiImageUtils.GetImage("Stimulsoft.Report", "Stimulsoft.Report.Components.Indicators.Images." + name + ".png");
        }
        #endregion
    }
}
