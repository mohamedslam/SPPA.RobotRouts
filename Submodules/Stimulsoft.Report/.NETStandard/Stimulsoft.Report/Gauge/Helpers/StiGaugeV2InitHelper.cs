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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components.Gauge;
using Stimulsoft.Report.Components.Gauge.Primitives;
using System;
using System.Linq;
using System.Diagnostics;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Gauge.Helpers
{
    public static class StiGaugeV2InitHelper
    {
        #region Properties
        public static bool AllowOldEditor
        {
            get
            {
                return StiSettings.GetBool("Designer", "ShowOldGaugeEditorV1", false);
            }
            set
            {
                StiSettings.Set("Designer", "ShowOldGaugeEditorV1", value);
            }
        }
        #endregion

        #region Methods
        public static bool IsGaugeV2(StiGauge gauge)
        {
            if (gauge.Mode == StiScaleMode.V1)
                return false;

            if (gauge.Scales.Count != 1) return false;
            var scale = gauge.Scales[0];

            switch (gauge.Type)
            {
                case StiGaugeType.FullCircular:
                    return IsFullCircularScale(scale);

                case StiGaugeType.HalfCircular:
                    return IsHalfCircularScale(scale);

                case StiGaugeType.Linear:
                case StiGaugeType.HorizontalLinear:
                    return IsLinearScale(scale);

                case StiGaugeType.Bullet:
                    return IsBulletScale(scale);
            }

            return false;
        }

        public static void Init(StiGauge gauge, StiGaugeType type, bool addRange, bool skipText)
        {
            string indicatorValue = null;
            StiIndicatorBase indicator = null;
            if (gauge.Scales.Count > 0)
            {
                indicator = gauge.Scales[0].Items.ToArray().FirstOrDefault(x => x is StiIndicatorBase) as StiIndicatorBase;
                if (indicator != null && indicator.Value != null)
                    indicatorValue = indicator.Value.Value;
            }

            StiScaleBase scale = null;
            switch (type)
            {
                case StiGaugeType.FullCircular:
                    scale = CreateFullCircularScale(skipText);
                    break;

                case StiGaugeType.HalfCircular:
                    scale = CreateHalfCircularScale(skipText);
                    break;

                case StiGaugeType.Linear:
                    scale = CreateLinearScale(skipText, false);
                    break;

                case StiGaugeType.HorizontalLinear:
                    scale = CreateLinearScale(skipText, true);
                    break;

                case StiGaugeType.Bullet:
                    scale = CreateBullet(gauge, addRange, skipText);
                    break;
            }

            if (!string.IsNullOrEmpty(indicatorValue))
            {
                indicator = scale.Items.ToArray().FirstOrDefault(x => x is StiIndicatorBase) as StiIndicatorBase;
                indicator.Value.Value = indicatorValue;
            }

            gauge.Scales.Clear();
            gauge.Scales.Add(scale);
        }

        public static void GetScaleMinMax(StiGaugeType type, out float min, out float max)
        {
            switch (type)
            {
                case StiGaugeType.FullCircular:
                    min = 0;
                    max = 180;
                    break;

                case StiGaugeType.HalfCircular:
                    min = 0;
                    max = 150;
                    break;

                case StiGaugeType.Linear:
                case StiGaugeType.HorizontalLinear:
                case StiGaugeType.Bullet:
                    min = 0;
                    max = 100;
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        internal static void Prepare(StiGauge gauge)
        {
            if (IsGaugeV2(gauge))
            {
                var scale = gauge.Scales[0];
                var indicator = scale.Items.ToArray().FirstOrDefault(x => x is StiIndicatorBase) as StiIndicatorBase;
                {
                    var e = new Stimulsoft.Report.Gauge.Events.StiGetValueEventArgs();
                    indicator.InvokeGetValue(indicator, e);

                    float value = StiGaugeHelper.GetFloatValueFromObject(e.Value, scale);

                    scale.Items.ToArray().Where(x => x is StiIndicatorBase).Cast<StiIndicatorBase>().ToList().ForEach(x => x.ValueObj = value);
                    //indicator.ValueObj = value;

                    Debug.WriteLine($"{gauge.Report.IsDesigning},  {indicator.Value.Value},    {value}");

                    if (gauge.CalculationMode == StiGaugeCalculationMode.Auto)
                    {
                        if (value == 0f)
                        {
                            scale.Minimum = 0;
                            scale.Maximum = 100;
                        }
                        else
                        {
                            scale.Minimum = value - Math.Abs(value);
                            scale.Maximum = value + Math.Abs(value);
                        }
                    }
                    else
                    {
                        scale.Minimum = (float)gauge.Minimum;
                        scale.Maximum = (float)gauge.Maximum;
                    }

                    #region Fill Range Value
                    StiScaleRangeList rangeList = null;
                    foreach (var item in scale.Items)
                    {
                        if (item is StiScaleRangeList)
                        {
                            rangeList = (StiScaleRangeList)item;
                            break;
                        }
                    }
                    if (rangeList != null && rangeList.Ranges.Count > 0)
                    {
                        float min = scale.Minimum;

                        var totalLength = Math.Max(scale.Maximum, scale.Minimum) - Math.Min(scale.Maximum, scale.Minimum);
                        float step = (float)Math.Round(totalLength / rangeList.Ranges.Count);

                        float startValue = min;
                        foreach (StiRangeBase range in rangeList.Ranges)
                        {
                            range.StartValue = startValue;
                            startValue += step;
                            range.EndValue = startValue;
                        }

                        rangeList.Ranges[rangeList.Ranges.Count - 1].EndValue = scale.Maximum;
                    }
                    #endregion

                    var sum = scale.Maximum - scale.Minimum;
                    scale.MajorInterval = Math.Abs((float)Math.Round(sum / 10));
                    scale.MinorInterval = Math.Abs(scale.MajorInterval / 2);

                    scale.CalculateMinMaxScaleHelper();
                    scale.CalculateWidthScaleHelper();
                }
            }
            else
            {
                foreach (StiScaleBase scale in gauge.Scales)
                {
                    scale.CalculateMinMaxScaleHelper();
                    scale.CalculateWidthScaleHelper();
                }
            }
        }
        #endregion
        
        #region Methods.FullCircularScale
        private static bool IsFullCircularScale(StiScaleBase scale)
        {
            if (!(scale is StiRadialScale)) return false;
            if (scale.Items.Count < 3) return false;

            return true;
        }

        private static StiScaleBase CreateFullCircularScale(bool skipText)
        {
            var scale = new StiRadialScale
            {
                StartAngle = 120,
                SweepAngle = 300,
                StartWidth = 0.05f,
                EndWidth = 0.05f,
                Minimum = 0,
                Maximum = 180,
                MajorInterval = 90,
                MinorInterval = 10,
                Brush = new StiSolidBrush(Color.FromArgb(238, 238, 238))
            };

            if (!skipText)
            {
                scale.Items.Add(new StiRadialTickMarkMajor
                {
                    Placement = StiPlacement.Inside,
                    RelativeWidth = 0.05f,
                    RelativeHeight = 0.015f,
                    Skin = StiTickMarkSkin.Rectangle,
                    Brush = new StiSolidBrush(Color.Black)
                });
            }

            if (!skipText)
            {
                scale.Items.Add(new StiRadialTickLabelMajor
                {
                    Offset = 0.1f,
                    Placement = StiPlacement.Inside
                });
            }

            scale.Items.Add(new StiNeedle
            {
                RelativeHeight = 0.3f,
                RelativeWidth = 0.4f,
                StartWidth = 0.1f,
                EndWidth = 1,
                CapBrush = new StiEmptyBrush(),
                Brush = new StiEmptyBrush(),
                TextBrush = new StiSolidBrush(Color.FromArgb(0, 150, 136)),
                ValueObj = 135f
            });

            scale.Items.Add(new StiRadialBar
            {
                StartWidth = 0.1f,
                EndWidth = 0.1f,
                Offset = 0.1f,
                ValueObj = 135f,
                Placement = StiPlacement.Outside,
                Brush = new StiSolidBrush(Color.Red),
            });

            return scale;
        }
        #endregion

        #region Methods.HalfCircularScale
        private static bool IsHalfCircularScale(StiScaleBase scale)
        {
            if (!(scale is StiRadialScale)) return false;
            if (scale.Items.Count != 2) return false;
            if (!(scale.Items[0] is StiNeedle)) return false;
            if (!(scale.Items[1] is StiRadialBar)) return false;

            var radialBar = (StiRadialBar)scale.Items[1];
            if (radialBar.RangeList.Count != 0) return false;

            return true;
        }

        private static StiScaleBase CreateHalfCircularScale(bool skipText)
        {
            var scale = new StiRadialScale
            {
                StartAngle = 180,
                SweepAngle = 180,
                StartWidth = 0.05f,
                EndWidth = 0.05f,
                Minimum = 0,
                Maximum = 180,
                Radius = 0.8f,
                MajorInterval = 90,
                MinorInterval = 10,
                Center = new PointF(0.5f, 0.7f),
                RadiusMode = StiRadiusMode.Auto,
                Brush = new StiSolidBrush(Color.FromArgb(238, 238, 238))
            };

            scale.Items.Add(new StiNeedle
            {
                RelativeHeight = 0.05f,
                RelativeWidth = 0.4f,
                StartWidth = 0.1f,
                EndWidth = 0.2f,
                CapBrush = new StiSolidBrush(Color.White),
                CapBorderBrush = new StiSolidBrush(Color.FromArgb(0, 150, 136)),
                CapBorderWidth = 2,
                Brush = new StiSolidBrush(Color.FromArgb(0, 150, 136)),
                BorderWidth = 0,
                TextBrush = new StiSolidBrush(Color.FromArgb(0, 150, 136)),
                ValueObj = 135f
            });

            scale.Items.Add(new StiRadialBar
            {
                StartWidth = 0.05f,
                EndWidth = 0.05f,
                Offset = 0.1f,
                ValueObj = 135f,
                Placement = StiPlacement.Overlay,
                Brush = new StiSolidBrush(Color.Red),
            });

            return scale;
        }
        #endregion

        #region Methods.LinearScale
        private static bool IsBulletScale(StiScaleBase scale)
        {
            if (!(scale is StiLinearScale)) return false;
            if (scale.Items.Count != 3) return false;
            if (!(scale.Items[0] is StiLinearRangeList)) return false;
            if (!(scale.Items[1] is StiLinearTickLabelMajor)) return false;
            if (!(scale.Items[2] is StiLinearBar)) return false;

            return true;
        }

        private static bool IsLinearScale(StiScaleBase scale)
        {
            if (!(scale is StiLinearScale)) return false;
            if (scale.Items.Count != 3) return false;
            if (!(scale.Items[0] is StiLinearTickLabelMajor)) return false;
            if (!(scale.Items[1] is StiLinearTickMarkMajor)) return false;
            if (!(scale.Items[2] is StiLinearMarker)) return false;

            return true;
        }

        private static StiScaleBase CreateLinearScale(bool skipText, bool isHorizontal)
        {
            var scale = new StiLinearScale()
            {
                Orientation = isHorizontal ? System.Windows.Forms.Orientation.Horizontal : System.Windows.Forms.Orientation.Vertical, 
                StartWidth = 0.1f,
                EndWidth = 0.1f,
                Maximum = 100f,
                MinorInterval = 5,
                BorderBrush = new StiEmptyBrush(),
                Brush = new StiEmptyBrush()
            };

            if (!skipText)
            {
                scale.Items.Add(new StiLinearTickLabelMajor()
                {
                    Placement = StiPlacement.Inside,
                    Font = new Font("Arial", 8f),
                    TextBrush = new StiSolidBrush(Color.FromArgb(158, 158, 158))
                });
                scale.Items.Add(new StiLinearTickMarkMajor()
                {
                    BorderBrush = new StiEmptyBrush(),
                    BorderWidth = 0f,
                    RelativeHeight = isHorizontal ? 0.05f : 0.005f,
                    RelativeWidth = isHorizontal ? 0.005f : 0.05f,
                    Brush = new StiSolidBrush(Color.FromArgb(158, 158, 158))
                });
            }
            var marker = new StiLinearMarker()
            {
                RelativeWidth = 0.10f,
                RelativeHeight = 0.04f,
                Placement = StiPlacement.Overlay,
                Brush = new StiSolidBrush(Color.FromArgb(205, 220, 57)),
                ValueObj = 85f
            };
            scale.Items.Add(marker);

            if (skipText)
            {
                marker.RelativeWidth = 0.07f;
                marker.RelativeHeight = 0.08f;
            }

            return scale;
        }

        private static StiScaleBase CreateBullet(StiGauge gauge, bool addRange, bool skipText)
        {
            var scale = new StiLinearScale()
            {
                Orientation = System.Windows.Forms.Orientation.Horizontal,
                RelativeHeight = 0.85f,
                StartWidth = 0.01f,
                EndWidth = 0.01f,
                Maximum = 100f,
                BorderBrush = new StiEmptyBrush(),
                Brush = new StiEmptyBrush()
            };

            if (addRange)
                AddLinearRanges(0, 100, scale);

            if (!skipText)
            {
                scale.Items.Add(new StiLinearTickLabelMajor()
                {
                    FormatService = gauge.ValueFormat,
                    Placement = StiPlacement.Outside,
                    Offset = 0.25f
                });
            }
            var marker = new StiLinearBar()
            {
                StartWidth = 0.15f,
                EndWidth = 0.15f,
                Brush = new StiSolidBrush(Color.FromArgb(205, 220, 57)),
                ValueObj = 85f
            };
            marker.Value.Value = "85";
            scale.Items.Add(marker);

            return scale;
        }

        private static void AddLinearRanges(double minValue, double maxValue, StiLinearScale scale)
        {
            var list = new StiLinearRangeList();

            var linearRange1 = new StiLinearRange
            {
                Placement = StiPlacement.Overlay,
                StartValue = (float)(minValue + (maxValue - minValue) * 0),
                EndValue = (float)(minValue + (maxValue - minValue) * 0.3d),
                StartWidth = 0.3f,
                EndWidth = 0.3f,
                BorderBrush = new StiEmptyBrush(),
                Brush = new StiSolidBrush(Color.Red)
            };
            list.Ranges.Add(linearRange1);

            var linearRange2 = new StiLinearRange
            {
                Placement = StiPlacement.Overlay,
                StartValue = (float)(minValue + (maxValue - minValue) * 0.3),
                EndValue = (float)(minValue + (maxValue - minValue) * 0.65d),
                StartWidth = 0.3f,
                EndWidth = 0.3f,
                BorderBrush = new StiEmptyBrush(),
                Brush = new StiSolidBrush(Color.Yellow)
            };
            list.Ranges.Add(linearRange2);

            var linearRange3 = new StiLinearRange
            {
                Placement = StiPlacement.Overlay,
                StartValue = (float)(minValue + (maxValue - minValue) * 0.65),
                EndValue = (float)(minValue + (maxValue - minValue) * 1d),
                StartWidth = 0.3f,
                EndWidth = 0.3f,
                BorderBrush = new StiEmptyBrush(),
                Brush = new StiSolidBrush(Color.Green)
            };
            list.Ranges.Add(linearRange3);

            scale.Items.Add(list);
        }

        #endregion
    }
}