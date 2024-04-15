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
using System.Collections;
using System.Globalization;
using System.Threading;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Gauge.Helpers
{
    public static class StiGaugeHelper
    {
        #region Fields
        public static TimeSpan GlobalDurationElement = TimeSpan.FromMilliseconds(1800);
        public static TimeSpan GlobalBeginTimeElement = TimeSpan.FromMilliseconds(300);
        #endregion

        #region Methods.Builder.Helper
        public static float GetFloatValueFromObject(object valueObj, StiScaleBase scale)
        {
            float result = scale.Minimum;

            if (valueObj != null)
            {
                if (valueObj is string)
                {
                    var currentCulture = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                    var valueStr = ((string)valueObj).Replace(",", ".");

                    if (float.TryParse(valueStr, out result))
                    {
                        if (result < scale.Minimum)
                            result = scale.Minimum;
                        else if (result > scale.Maximum)
                            result = scale.Maximum;
                    }
                    else
                    {
                        result = scale.Minimum;
                    }

                    Thread.CurrentThread.CurrentCulture = currentCulture;
                }
                else
                {
                    result = Convert.ToSingle(valueObj);
                }
            }

            return result;
        }

        public static float GetFloatValueFromObject(object valueObj, float defaultValue)
        {
            float result = defaultValue;

            if (valueObj != null)
            {
                if (valueObj is string)
                {
                    var currentCulture = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                    if (!float.TryParse((string)valueObj, out result))
                    {
                        result = defaultValue;
                    }

                    Thread.CurrentThread.CurrentCulture = currentCulture;
                }
                else
                {
                    result = Convert.ToSingle(valueObj);
                }
            }

            return result;
        }

        public static float[] GetFloatArrayValueFromString(object value)
        {
            var valueStr = value as string;
            if (string.IsNullOrEmpty(valueStr))
                return null;

            var strs = valueStr.Split(';');
            var values = new float[strs.Length];

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

            int index = -1;
            while(++index < strs.Length)
            {
                float result;
                if (float.TryParse(strs[index], out result))
                {
                    values[index] = result;
                }
                else
                {
                    values = null;
                    break;
                }
            }

            Thread.CurrentThread.CurrentCulture = currentCulture;

            return values;
        }
        #endregion

        #region Init Gauges
        private static void InitializeGauge(StiGauge gauge, double width, double height)
        {
            gauge.Scales.Clear();
            
            if (gauge.Page != null)
            {
                var unit = gauge.Page.Unit;
                gauge.ClientRectangle = new RectangleD(gauge.Left, gauge.Top, unit.ConvertFromHInches(width), unit.ConvertFromHInches(height));
            }
        }

        private static void InitializeName(StiGauge gauge, StiReport report)
        {
            if (string.IsNullOrEmpty(gauge.Name))
            {
                string name = StiNameCreation.CreateSimpleName(report, StiNameCreation.GenerateName(gauge));
                gauge.Name = name;

                int index = -1;
                while(++index < gauge.Scales.Count)
                {
                    var tableNames = new Hashtable();

                    var scale = gauge.Scales[index];

                    int index1 = -1;
                    while(++index1 < scale.Items.Count)
                    {
                        var element = scale.Items[index1];

                        var type = element.GetType();
                        int count = 0;

                        if (tableNames.ContainsKey(type))
                            count = (int)tableNames[type];

                        count++;
                        tableNames[type] = count;                        
                    }
                }
            }
        }

        public static void CheckGaugeName(StiGauge gauge)
        {
            int index = -1;
            while(++index < gauge.Scales.Count)
            {
                var tableNames = new Hashtable();

                var scale = gauge.Scales[index];

                int index1 = -1;
                while(++index1 < scale.Items.Count)
                {
                    var element = scale.Items[index1];

                    var type = element.GetType();
                    int count = 0;
                    if (tableNames.ContainsKey(type))
                        count = (int)tableNames[type];
                    count++;
                    tableNames[type] = count;                    
                }
            }
        }

        public static void SimpleRadialGauge(StiGauge gauge, StiReport report)
        {
            InitializeGauge(gauge, 250, 250);

            var radialScale1 = new StiRadialScale
            {
                StartAngle = 0f,
                SweepAngle = 340,
                StartWidth = 0.05f,
                EndWidth = 0.05f,
                Minimum = 0f,
                Maximum = 100f,
                MajorInterval = 10,
                MinorInterval = 5,
                Brush = new StiSolidBrush(Color.FromArgb(238, 238, 238))
            };

            var tickMarkMajor1 = new StiRadialTickMarkMajor
            {
                Placement = StiPlacement.Overlay,
                OffsetAngle = -2,
                RelativeWidth = 0.05f,
                RelativeHeight = 0.02f,
                Skin = StiTickMarkSkin.Rectangle,
                Brush = new StiSolidBrush(Color.Black)
            };

            var tickMarkMinor1 = new StiRadialTickMarkMinor
            {
                Placement = StiPlacement.Overlay,
                RelativeWidth = 0.01f,
                RelativeHeight = 0.01f,
                Skin = StiTickMarkSkin.Rectangle
            };

            var tickLabelMajor1 = new StiRadialTickLabelMajor();

            var needle1 = new StiNeedle
            {
                RelativeHeight = 0.06f,
                RelativeWidth = 0.45f,
                StartWidth = 0.1f,
                EndWidth = 1f,
                CapBrush = new StiSolidBrush(Color.White),
                CapBorderBrush = new StiSolidBrush(Color.FromArgb(244, 67, 54)),
                CapBorderWidth = 2,
                Brush = new StiSolidBrush(Color.FromArgb(244, 67, 54)),
                Value = new StiValueExpression("60")
            };

            radialScale1.Items.Add(tickMarkMajor1);
            radialScale1.Items.Add(tickMarkMinor1);
            radialScale1.Items.Add(tickLabelMajor1);
            radialScale1.Items.Add(needle1);

            gauge.Scales.Add(radialScale1);
            InitializeName(gauge, report);
        }

        public static void RadialTwoScalesGauge(StiGauge gauge, StiReport report)
        {
            InitializeGauge(gauge, 250, 250);

            var radialScale1 = new StiRadialScale
            {
                StartAngle = 270f,
                SweepAngle = 360,
                StartWidth = 0.05f,
                EndWidth = 0.05f,
                Minimum = 0f,
                Maximum = 60f,
                Radius = 0.5f,
                MajorInterval = 10,
                MinorInterval = 1,
                Brush = new StiSolidBrush(Color.FromArgb(238, 238, 238))
            };

            var tickMarkMajor1 = new StiRadialTickMarkMajor
            {
                Placement = StiPlacement.Overlay,
                RelativeWidth = 0.05f,
                OffsetAngle = -2f,
                RelativeHeight = 0.03f,
                Skin = StiTickMarkSkin.Rectangle,
                Brush = new StiSolidBrush(Color.Black)
            };

            var tickLabelMajor1 = new StiRadialTickLabelMajor
            {
                MinimumValue = 1,
                Font = new Font("Arial", 7f)
            };

            radialScale1.Items.Add(tickMarkMajor1);
            radialScale1.Items.Add(tickLabelMajor1);
            
            gauge.Scales.Add(radialScale1);

            var radialScale2 = new StiRadialScale
            {
                StartAngle = 270f,
                SweepAngle = 360,
                StartWidth = 0.05f,
                EndWidth = 0.05f,
                Minimum = 0f,
                Maximum = 12f,
                Radius = 0.75f,
                MajorInterval = 1,
                MinorInterval = 0,
                Brush = new StiSolidBrush(Color.FromArgb(238, 238, 238))
            };

            var tickMarkMajor2 = new StiRadialTickMarkMajor
            {
                MinimumValue = 1,
                Placement = StiPlacement.Overlay,
                OffsetAngle = -2f,
                RelativeHeight = 0.03f,
                RelativeWidth = 0.05f,
                Skin = StiTickMarkSkin.Rectangle,
                Brush = new StiSolidBrush(Color.Black),
                BorderBrush = new StiSolidBrush(Color.White),
                BorderWidth = 3
            };

            var tickLabelMajor2 = new StiRadialTickLabelMajor
            {
                MinimumValue = 1
            };

            var needle1 = new StiNeedle
            {
                Value = new StiValueExpression("2"),
                OffsetNeedle = 0.15f,
                StartWidth = 0.05f,
                EndWidth = 0.5f,
                RelativeHeight = 0.12f,
                RelativeWidth = 0.6f,
                CapBrush = new StiSolidBrush(Color.FromArgb(244, 67, 54)),
                Brush = new StiSolidBrush(Color.FromArgb(244, 67, 54))
            };

            var needle2 = new StiNeedle
            {
                Value = new StiValueExpression("10"),
                OffsetNeedle = 0.15f,
                StartWidth = 0.05f,
                EndWidth = 0.5f,
                RelativeHeight = 0.08f,
                RelativeWidth = 0.4f,
                CapBrush = new StiSolidBrush(Color.FromArgb(3, 169, 244)),
                Brush = new StiSolidBrush(Color.FromArgb(3, 169, 244))
            };

            radialScale2.Items.Add(tickMarkMajor2);
            radialScale2.Items.Add(tickLabelMajor2);
            radialScale2.Items.Add(needle1);
            radialScale2.Items.Add(needle2);

            gauge.Scales.Add(radialScale2);


            InitializeName(gauge, report);
        }

        public static void RadialBarGauge(StiGauge gauge, StiReport report)
        {
            InitializeGauge(gauge, 250, 250);

            var radialScale1 = new StiRadialScale()
            {
                StartAngle = 120f,
                SweepAngle = 300,
                StartWidth = 0.05f,
                EndWidth = 0.05f,
                Minimum = 0f,
                Maximum = 180f,
                MajorInterval = 90,
                MinorInterval = 10,
                Brush = new StiSolidBrush(Color.FromArgb(238, 238, 238))
            };

            var tickMarkMajor1 = new StiRadialTickMarkMajor
            {
                Placement = StiPlacement.Inside,
                OffsetAngle = -1,
                RelativeWidth = 0.05f,
                RelativeHeight = 0.015f,
                Skin = StiTickMarkSkin.Rectangle,
                Brush = new StiSolidBrush(Color.Black)
            };

            var tickMarkMinor1 = new StiRadialTickMarkMinor
            {
                Placement = StiPlacement.Inside,
                RelativeWidth = 0.03f,
                RelativeHeight = 0.005f,
                Skin = StiTickMarkSkin.Rectangle,
                Brush = new StiSolidBrush(Color.Black)
            };

            var tickLabelMajor1 = new StiRadialTickLabelMajor()
            {
                Placement = StiPlacement.Inside,
                MinimumValue = 1,
                Offset = 0.15f
            };
            
            var radialBar = new StiRadialBar()
            {
                StartWidth = 0.1f,
                EndWidth = 0.1f,
                Offset = 0.1f,
                Placement = StiPlacement.Outside,
                Brush = new StiSolidBrush(Color.FromArgb(0, 150, 136)),
                Value = new StiValueExpression("90")
            };

            var needle1 = new StiNeedle()
            {
                RelativeHeight = 0.3f,
                RelativeWidth = 0.4f,
                StartWidth = 0.1f,
                EndWidth = 1f,
                CapBrush = new StiEmptyBrush(),
                Brush = new StiEmptyBrush(),
                TextBrush = new StiSolidBrush(Color.FromArgb(0, 150, 136)),
                Font = new Font("Arial", 36f),
                Value = new StiValueExpression("90")
            };

            radialScale1.Items.Add(tickMarkMajor1);
            radialScale1.Items.Add(tickMarkMinor1);
            radialScale1.Items.Add(tickLabelMajor1);
            radialScale1.Items.Add(radialBar);
            radialScale1.Items.Add(needle1);

            gauge.Scales.Add(radialScale1);
            InitializeName(gauge, report);
        }

        public static void SimpleTwoBarGauge(StiGauge gauge, StiReport report)
        {
            InitializeGauge(gauge, 250, 250);

            var radialScale1 = new StiRadialScale()
            {
                Minimum = 0f,
                Maximum = 180f,
                MajorInterval = 20,
                MinorInterval = 10,
                StartWidth = 0.05f,
                EndWidth = 0.05f,
                StartAngle = 120f,
                SweepAngle = 300,
                Brush = new StiSolidBrush(Color.FromArgb(238, 238, 238))
            };

            var tickMarkMajor1 = new StiRadialTickMarkMajor()
            {
                Placement = StiPlacement.Overlay,
                OffsetAngle = -1,
                RelativeWidth = 0.05f,
                RelativeHeight = 0.02f,
                Skin = StiTickMarkSkin.Rectangle,
                Brush = new StiSolidBrush(Color.Black),
                BorderBrush = new StiSolidBrush(Color.White),
                BorderWidth = 1
            };

            var tickMarkMinor1 = new StiRadialTickMarkMinor()
            {
                Placement = StiPlacement.Overlay,
                RelativeWidth = 0.01f,
                RelativeHeight = 0.01f,
                Skin = StiTickMarkSkin.Diamond,
                Brush = new StiSolidBrush(Color.Black),
                BorderBrush = new StiSolidBrush(Color.Gray),
                BorderWidth = 1
            };

            var tickLabelMajor1 = new StiRadialTickLabelMajor()
            {
                Placement = StiPlacement.Inside,
                Offset = 0.15f
            };

            var radialBar1 = new StiRadialBar()
            {
                StartWidth = 0.05f,
                EndWidth = 0.05f,
                Offset = 0f,
                Placement = StiPlacement.Outside,
                Brush = new StiSolidBrush(Color.FromArgb(3, 169, 244)),
                Value = new StiValueExpression("80")
            };

            var radialBar2 = new StiRadialBar()
            {
                StartWidth = 0.05f,
                EndWidth = 0.05f,
                Offset = 0f,
                Placement = StiPlacement.Inside,
                Brush = new StiSolidBrush(Color.FromArgb(255, 235, 59)),
                Value = new StiValueExpression("120")
            };

            var radialMarker1 = new StiRadialMarker()
            {
                Value = new StiValueExpression("100"),
                Offset = 0.25f,
                RelativeHeight = 0.05f,
                RelativeWidth = 0.05f,
                Skin = StiMarkerSkin.TriangleRight,
                Brush = new StiSolidBrush(Color.FromArgb(76, 175, 80))
            };

            radialScale1.Items.Add(tickMarkMajor1);
            radialScale1.Items.Add(tickMarkMinor1);
            radialScale1.Items.Add(tickLabelMajor1);
            radialScale1.Items.Add(radialBar1);
            radialScale1.Items.Add(radialBar2);
            radialScale1.Items.Add(radialMarker1);

            gauge.Scales.Add(radialScale1);
            InitializeName(gauge, report);
        }

        public static void DefaultRadialGauge(StiGauge gauge, StiReport report)
        {
            InitializeGauge(gauge, 250, 250);

            var radialScale1 = new StiRadialScale
            {
                StartAngle = 0f,
                SweepAngle = 340,
                StartWidth = 0.05f,
                EndWidth = 0.05f,
                Minimum = 0f,
                Maximum = 100f,
                MajorInterval = 10,
                MinorInterval = 5
            };

            var tickMarkMajor1 = new StiRadialTickMarkMajor
            {
                Placement = StiPlacement.Overlay,
                OffsetAngle = -2,
                RelativeWidth = 0.06f,
                RelativeHeight = 0.03f,
                Skin = StiTickMarkSkin.TriangleRight
            };

            var tickLabelMajor1 = new StiRadialTickLabelMajor();

            var needle1 = new StiNeedle
            {
                RelativeHeight = 0.14f,
                RelativeWidth = 0.5f
            };

            radialScale1.Items.Add(tickMarkMajor1);
            radialScale1.Items.Add(tickLabelMajor1);
            radialScale1.Items.Add(needle1);

            gauge.Scales.Add(radialScale1);
            InitializeName(gauge, report);            
        }

        public static void DefaultLinearGauge(StiGauge gauge, StiReport report)
        {
            InitializeGauge(gauge, 120, 240);

            var scale = new StiLinearScale()
            {
                StartWidth = 0.1f,
                EndWidth = 0.1f,
                Maximum = 100f,
                MinorInterval = 5,
                BorderBrush = new StiSolidBrush(Color.FromArgb(158, 158, 158)),
                Brush = new StiSolidBrush(Color.FromArgb(238, 238, 238))
            };

            var tickMarkMajor = new StiLinearTickMarkMajor()
            {
                BorderBrush = new StiEmptyBrush(),
                BorderWidth = 0f,
                RelativeHeight = 0.005f,
                RelativeWidth = 0.05f,
                Brush = new StiSolidBrush(Color.FromArgb(158, 158, 158))
            };

            var tickLabelMajor = new StiLinearTickLabelMajor()
            {
                Placement = StiPlacement.Inside,
                TextBrush = new StiSolidBrush(Color.FromArgb(158, 158, 158))
            };

            var linearBar = new StiLinearBar()
            {
                StartWidth = 0.1f,
                EndWidth = 0.1f,
                Brush = new StiSolidBrush(Color.FromArgb(0, 150, 136)),
                Value = new StiValueExpression("65")
            };
                
            scale.Items.Add(tickMarkMajor);
            scale.Items.Add(tickLabelMajor);
            scale.Items.Add(linearBar);

            gauge.Scales.Add(scale);
            InitializeName(gauge, report);
        }
        
        public static void LinearGaugeRangeList(StiGauge gauge, StiReport report)
        {
            InitializeGauge(gauge, 120, 240);

            var scale = new StiLinearScale()
            {
                StartWidth = 0.1f,
                EndWidth = 0.1f,
                Maximum = 100f,
                MinorInterval = 5,
                BorderBrush = new StiSolidBrush(Color.FromArgb(158, 158, 158)),
                Brush = new StiSolidBrush(Color.FromArgb(238, 238, 238))
            };

            var linearRangeList = new StiLinearRangeList();

            var linearRange1 = new StiLinearRange()
            {
                StartValue = 0f,
                EndValue = 50f,
                Brush = new StiGradientBrush(Color.FromArgb(205, 220, 57), Color.FromArgb(139, 195, 74), 90f),
                StartWidth = 0.07f,
                EndWidth = 0.07f
            };

            var linearRange2 = new StiLinearRange()
            {
                StartValue = 45f,
                EndValue = 75f,
                Brush = new StiGradientBrush(Color.FromArgb(255, 193, 7), Color.FromArgb(255, 235, 59), 90f),
                StartWidth = 0.07f,
                EndWidth = 0.07f
            };

            var linearRange3 = new StiLinearRange()
            {
                StartValue = 75f,
                EndValue = 100f,
                Brush = new StiGradientBrush(Color.FromArgb(255, 152, 0), Color.FromArgb(255, 152, 0), 90f),
                StartWidth = 0.07f,
                EndWidth = 0.07f
            };

            linearRangeList.Ranges.Add(linearRange1);
            linearRangeList.Ranges.Add(linearRange2);
            linearRangeList.Ranges.Add(linearRange3);

            var linearTickLabelMajor = new StiLinearTickLabelMajor()
            {
                Placement = StiPlacement.Inside,
                TextBrush = new StiSolidBrush(Color.FromArgb(158, 158, 158))
            };

            var linearTickMarkMajor = new StiLinearTickMarkMajor()
            {
                BorderBrush = new StiEmptyBrush(),
                BorderWidth = 0f,
                RelativeHeight = 0.005f,
                RelativeWidth = 0.05f,
                Brush = new StiSolidBrush(Color.FromArgb(158, 158, 158))
            };

            var linearTickMarkMinor = new StiLinearTickMarkMinor()
            {
                Placement = StiPlacement.Overlay,
                RelativeWidth = 0.08f,
                RelativeHeight = 0.006f,
                BorderBrush = new StiEmptyBrush()
            };

            var linearMarker1 = new StiLinearMarker()
            {
                RelativeWidth = 0.18f,
                RelativeHeight = 0.04f,
                Placement = StiPlacement.Overlay,
                Brush = new StiSolidBrush(Color.FromArgb(205, 220, 57))
            };
            
            scale.Items.Add(linearRangeList);
            scale.Items.Add(linearTickLabelMajor);
            scale.Items.Add(linearTickMarkMajor);
            scale.Items.Add(linearTickMarkMinor);
            scale.Items.Add(linearMarker1);

            gauge.Scales.Add(scale);
            InitializeName(gauge, report);
        }
        
        public static void BulletGraphsGreen(StiGauge gauge, StiReport report)
        {
            InitializeGauge(gauge, 300, 100);

            var linearScale1 = new StiLinearScale()
            {
                RelativeHeight = 0.94f,
                StartWidth = 0.01f,
                EndWidth = 0.01f,
                Maximum = 100f,
                MajorInterval = 20f,
                Orientation = System.Windows.Forms.Orientation.Horizontal,
                Brush = new StiEmptyBrush(),
                BorderBrush = new StiEmptyBrush()
            };

            #region Range
            var linearRangeList1 = new StiLinearRangeList();

            float startValue = 0f;
            for (int index = 1; index <= 10; index++)
            {
                var linearRange = new StiLinearRange
                {
                    StartValue = startValue,
                    EndValue = startValue + 10f,
                    StartWidth = 0.5f,
                    EndWidth = 0.5f,
                    Placement = StiPlacement.Overlay,
                    BorderBrush = new StiSolidBrush(Color.FromArgb(150, 139, 138, 135))
                };

                if (index >= 1 && index <= 3)
                    linearRange.Brush = new StiSolidBrush(Color.FromArgb(165, 214, 167));
                else if (index >= 4 && index <= 7)
                    linearRange.Brush = new StiSolidBrush(Color.FromArgb(76, 175, 80));
                else
                    linearRange.Brush = new StiSolidBrush(Color.FromArgb(46, 125, 50));

                linearRangeList1.Ranges.Add(linearRange);

                startValue += 10f;
            }
            #endregion

            var linearTickLabelMajor1 = new StiLinearTickLabelMajor()
            {
                Offset = 0.25f
            };

            var linearTickMarkCustom1 = new StiLinearTickMarkCustom()
            {
                Placement = StiPlacement.Overlay,
                RelativeWidth = 0.015f,
                RelativeHeight = 0.3f,
                Brush = new StiGradientBrush(Color.FromArgb(100, 100, 100), Color.FromArgb(10, 10, 10), 90f),
                ValueObj = 80f
            };

            var linearBar1 = new StiLinearBar()
            {
                Brush = new StiSolidBrush(Color.Black),
                StartWidth = 0.1f,
                EndWidth = 0.1f
            };

            linearScale1.Items.Add(linearRangeList1);
            linearScale1.Items.Add(linearTickLabelMajor1);
            linearScale1.Items.Add(linearTickMarkCustom1);
            linearScale1.Items.Add(linearBar1);

            gauge.Scales.Add(linearScale1);
            InitializeName(gauge, report);
        }
        
        public static void HalfDonutsGauge(StiGauge gauge, StiReport report)
        {
            InitializeGauge(gauge, 280, 165);

            var radialScale1 = new StiRadialScale()
            {
                StartAngle = 180f,
                SweepAngle = 180f,
                Minimum = 0,
                Maximum = 150f,
                Radius = 0.75f,
                Center = new PointF(0.5f, 0.8f),
                RadiusMode = StiRadiusMode.Width,
                Skin = StiRadialScaleSkin.Empty
            };

            var radialBar1 = new StiRadialBar()
            {
                BorderBrush = new StiEmptyBrush(),
                BorderWidth = 0,
                StartWidth = 0.3f,
                EndWidth = 0.3f,
                EmptyBrush = new StiGradientBrush(Color.FromArgb(221, 221, 221), Color.FromArgb(240, 240, 240), 90f),
                UseRangeColor = true,
                Value = new StiValueExpression("60")
            };

            var range1 = new StiRadialIndicatorRangeInfo
            {
                Value = 0,
                Brush = new StiSolidBrush(Color.FromArgb(139, 195, 74))
            };
            var range2 = new StiRadialIndicatorRangeInfo
            {
                Value = 50,
                Brush = new StiSolidBrush(Color.FromArgb(255, 235, 59))
            };
            var range3 = new StiRadialIndicatorRangeInfo
            {
                Value = 100,
                Brush = new StiSolidBrush(Color.FromArgb(244, 67, 54))
            };

            radialBar1.RangeList.Add(range1);
            radialBar1.RangeList.Add(range2);
            radialBar1.RangeList.Add(range3);

            var needle1 = new StiNeedle()
            {
                RelativeHeight = 0.05f,
                RelativeWidth = 0.4f,
                StartWidth = 0.1f,
                EndWidth = 0.2f,
                CapBrush = new StiSolidBrush(Color.White),
                CapBorderBrush = new StiSolidBrush(Color.FromArgb(0, 150, 136)),
                CapBorderWidth = 2f,
                Brush = new StiSolidBrush(Color.FromArgb(0, 150, 136)),
                BorderWidth = 0f,
                TextBrush = new StiSolidBrush(Color.FromArgb(0, 150, 136)),
                Value = new StiValueExpression("60")
            };

            radialScale1.Items.Add(radialBar1);
            radialScale1.Items.Add(needle1);

            gauge.Scales.Add(radialScale1);
            InitializeName(gauge, report);
        }

        public static void HalfDonutsGauge2(StiGauge gauge, StiReport report)
        {
            InitializeGauge(gauge, 280, 165);

            var radialScale1 = new StiRadialScale
            {
                StartAngle = 180f,
                SweepAngle = 180f,
                Minimum = 0,
                Maximum = 100f,
                Radius = 0.75f,
                Center = new PointF(0.5f, 0.8f),
                RadiusMode = StiRadiusMode.Width,
                Skin = StiRadialScaleSkin.Empty
            };

            var radialBar1 = new StiRadialBar
            {
                BorderBrush = new StiEmptyBrush(),
                StartWidth = 0.3f,
                EndWidth = 0.3f,
                Brush = new StiSolidBrush(Color.FromArgb(79, 134, 194)),
                EmptyBrush = new StiGradientBrush(Color.FromArgb(221, 221, 221), Color.FromArgb(240, 240, 240), 90f),
                EmptyBorderBrush = new StiSolidBrush(Color.FromArgb(79, 134, 194)),
                EmptyBorderWidth = 3f
            };

            radialScale1.Items.Add(radialBar1);

            gauge.Scales.Add(radialScale1);
            InitializeName(gauge, report);
        }

        public static void RadialGaugeHalfCircleN(StiGauge gauge, StiReport report)
        {
            InitializeGauge(gauge, 260, 150);

            var radialScale1 = new StiRadialScale()
            {
                RadiusMode = StiRadiusMode.Width,
                Skin = StiRadialScaleSkin.RadialScaleHalfCircleN,
                StartAngle = 180f,
                SweepAngle = 180f,
                Maximum = 100,
                Center = new PointF(0.5f, 0.85f),
                Radius = 0.75f,
                StartWidth = 0.005f,
                EndWidth = 0.005f,
                MajorInterval = 10,
                MinorInterval = 5,
                Brush = new StiSolidBrush(Color.Gray),
            };

            var radialTickMarkMajor1 = new StiRadialTickMarkMajor()
            {
                Placement = StiPlacement.Overlay,
                Offset = 0.035f,
                RelativeWidth = 0.05f,
                RelativeHeight = 0.007f,
                Brush = new StiSolidBrush(Color.Gray),
                BorderBrush = new StiEmptyBrush()
            };

            var radialTickMarkMinor1 = new StiRadialTickMarkMinor()
            {
                Placement = StiPlacement.Overlay,
                Offset = 0.017f,
                RelativeWidth = 0.03f,
                RelativeHeight = 0.007f,
                Brush = new StiSolidBrush(Color.LightGray),
                BorderBrush = new StiEmptyBrush()
            };

            var radialTickLabelMajor1 = new StiRadialTickLabelMajor()
            {
                LabelRotationMode = StiLabelRotationMode.Automatic,
                TextBrush = new StiSolidBrush(Color.Black)
            };

            var needle1 = new StiNeedle()
            {
                Value = new StiValueExpression("45"),
                CapBrush = new StiSolidBrush(Color.FromArgb(3, 169, 244)),
                Brush = new StiSolidBrush(Color.FromArgb(3, 169, 244)),
                StartWidth = 0.1f,
                EndWidth = 0.99f,
                RelativeWidth = 0.5f,
                RelativeHeight = 0.04f,
            };

            radialScale1.Items.Add(radialTickMarkMajor1);
            radialScale1.Items.Add(radialTickMarkMinor1);
            radialScale1.Items.Add(radialTickLabelMajor1);
            radialScale1.Items.Add(needle1);

            gauge.Scales.Add(radialScale1);
            InitializeName(gauge, report);
        }

        public static void RadialGaugeHalfCircleS(StiGauge gauge, StiReport report)
        {
            InitializeGauge(gauge, 260, 150);
            
            var radialScale1 = new StiRadialScale()
            {
                RadiusMode = StiRadiusMode.Width,
                Skin = StiRadialScaleSkin.RadialScaleHalfCircleS,
                StartAngle = 0,
                SweepAngle = 180f,
                Maximum = 100,
                Center = new PointF(0.5f, 0.15f),
                Radius = 0.75f,
                StartWidth = 0.005f,
                EndWidth = 0.005f,
                MajorInterval = 10,
                MinorInterval = 5,
                Brush = new StiSolidBrush(Color.Gray),
            };

            var radialTickMarkMajor1 = new StiRadialTickMarkMajor()
            {
                Placement = StiPlacement.Overlay,
                Offset = 0.035f,
                RelativeWidth = 0.05f,
                RelativeHeight = 0.007f,
                Brush = new StiSolidBrush(Color.Gray),
                BorderBrush = new StiEmptyBrush()
            };

            var radialTickMarkMinor1 = new StiRadialTickMarkMinor()
            {
                Placement = StiPlacement.Overlay,
                Offset = 0.017f,
                RelativeWidth = 0.03f,
                RelativeHeight = 0.007f,
                Brush = new StiSolidBrush(Color.LightGray),
                BorderBrush = new StiEmptyBrush()
            };

            var radialTickLabelMajor1 = new StiRadialTickLabelMajor()
            {
                LabelRotationMode = StiLabelRotationMode.Automatic,
                TextBrush = new StiSolidBrush(Color.Black)
            };

            var needle1 = new StiNeedle()
            {
                Value = new StiValueExpression("45"),
                CapBrush = new StiSolidBrush(Color.FromArgb(3, 169, 244)),
                Brush = new StiSolidBrush(Color.FromArgb(3, 169, 244)),
                StartWidth = 0.1f,
                EndWidth = 0.99f,
                RelativeWidth = 0.5f,
                RelativeHeight = 0.04f,
            };

            radialScale1.Items.Add(radialTickMarkMajor1);
            radialScale1.Items.Add(radialTickMarkMinor1);
            radialScale1.Items.Add(radialTickLabelMajor1);
            radialScale1.Items.Add(needle1);

            gauge.Scales.Add(radialScale1);
            InitializeName(gauge, report);
        }

        public static void RadialGaugeQuarterCircleNW(StiGauge gauge, StiReport report)
        {
            RadialGaugeQuarterCircle(gauge, report, StiRadialScaleSkin.RadialScaleQuarterCircleNW,
                180f, new PointF(0.9f, 0.9f));
        }

        public static void RadialGaugeQuarterCircleNE(StiGauge gauge, StiReport report)
        {
            RadialGaugeQuarterCircle(gauge, report, StiRadialScaleSkin.RadialScaleQuarterCircleNE,
                270f, new PointF(0.1f, 0.9f));
        }

        public static void RadialGaugeQuarterCircleSW(StiGauge gauge, StiReport report)
        {
            RadialGaugeQuarterCircle(gauge, report, StiRadialScaleSkin.RadialScaleQuarterCircleSW,
                90f, new PointF(0.9f, 0.1f));
        }

        public static void RadialGaugeQuarterCircleSE(StiGauge gauge, StiReport report)
        {
            RadialGaugeQuarterCircle(gauge, report, StiRadialScaleSkin.RadialScaleQuarterCircleSE,
                0f, new PointF(0.1f, 0.1f));
        }

        private static void RadialGaugeQuarterCircle(StiGauge gauge, StiReport report, StiRadialScaleSkin scaleSkin, float startAngle, PointF center)
        {
            InitializeGauge(gauge, 250, 250);

            var radialScale1 = new StiRadialScale()
            {
                Skin = scaleSkin,
                StartAngle = startAngle,
                SweepAngle = 90f,
                Maximum = 100f,
                Center = center,
                Radius = 1.5f,
                StartWidth = 0.005f,
                EndWidth = 0.005f,
                MajorInterval = 10f,
                MinorInterval = 5f,
            };
            var radialTickMarkMajor1 = new StiRadialTickMarkMajor()
            {
                Placement = StiPlacement.Overlay,
                Offset = 0.045f,
                RelativeWidth = 0.03f,
                RelativeHeight = 0.004f,
                BorderBrush = new StiEmptyBrush(),
                Brush = new StiSolidBrush(Color.Gray)
            };

            var radialTickMarkMinor1 = new StiRadialTickMarkMinor()
            {
                Placement = StiPlacement.Overlay,
                Offset = 0.04f,
                RelativeWidth = 0.02f,
                RelativeHeight = 0.004f,
                BorderBrush = new StiEmptyBrush(),
                Brush = new StiSolidBrush(Color.Gray)
            };

            var radialTickLabelMajor1 = new StiRadialTickLabelMajor()
            {
                LabelRotationMode = StiLabelRotationMode.Automatic
            };

            var needle1 = new StiNeedle()
            {
                Value = new StiValueExpression("45"),
                CapBrush = new StiSolidBrush(Color.FromArgb(244, 67, 54)),
                Brush = new StiSolidBrush(Color.FromArgb(244, 67, 54)),
                StartWidth = 0.1f,
                EndWidth = 0.99f,
                RelativeWidth = 0.5f,
                RelativeHeight = 0.04f,
            };

            radialScale1.Items.Add(radialTickMarkMajor1);
            radialScale1.Items.Add(radialTickMarkMinor1);
            radialScale1.Items.Add(radialTickLabelMajor1);
            radialScale1.Items.Add(needle1);

            gauge.Scales.Add(radialScale1);
            InitializeName(gauge, report);
        }

        public static void HorizontalThermometer(StiGauge gauge, StiReport report)
        {
            InitializeGauge(gauge, 300, 50);

            var linearScale = new StiLinearScale
            {
                Orientation = System.Windows.Forms.Orientation.Horizontal,
                RelativeHeight = 0.85f,
                Left = 0.03f,
                StartWidth = 0.17f,
                EndWidth = 0.17f,
                Maximum = 80f,
                MajorInterval = 20,
                MinorInterval = 4
            };

            var linearBar = new StiLinearBar
            {
                Skin = StiLinearBarSkin.HorizontalThermometer,
                Placement = StiPlacement.Outside,
                StartWidth = 0.2f,
                EndWidth = 0.2f,
                UseRangeColor = true
            };

            var indicatorRange1 = new StiLinearIndicatorRangeInfo
            {
                Value = 0,
                Brush = new StiGradientBrush(Color.FromArgb(137, 188, 41), Color.FromArgb(111, 163, 14), 90f)
            };
            var indicatorRange2 = new StiLinearIndicatorRangeInfo
            {
                Value = 40,
                Brush = new StiGradientBrush(Color.FromArgb(217, 173, 45), Color.FromArgb(222, 166, 0), 90f)
            };
            var indicatorRange3 = new StiLinearIndicatorRangeInfo
            {
                Value = 65,
                Brush = new StiGradientBrush(Color.FromArgb(208, 49, 52), Color.FromArgb(186, 6, 10), 90f)
            };

            linearBar.RangeList.Add(indicatorRange1);
            linearBar.RangeList.Add(indicatorRange2);
            linearBar.RangeList.Add(indicatorRange3);

            var tickMarkMajor = new StiLinearTickMarkMajor
            {
                Placement = StiPlacement.Overlay,
                Brush = new StiSolidBrush(Color.FromArgb(159, 159, 159)),
                RelativeWidth = 0.01f,
                RelativeHeight = 0.18f,
                Offset = -0.1f
            };

            var tickMarkMinor = new StiLinearTickMarkMinor
            {
                Brush = new StiSolidBrush(Color.FromArgb(159, 159, 159)),
                RelativeWidth = 0.005f,
                RelativeHeight = 0.1f,
                Offset = -0.14f
            };

            var tickLabelMajor = new StiLinearTickLabelMajor
            {
                Placement = StiPlacement.Inside,
                Offset = 0.11f,
                TextBrush = new StiSolidBrush(Color.FromArgb(83, 85, 86)),
                Font = new Font("Arial", 10f)
            };

            var stateIndicator = new StiStateIndicator
            {
                Left = 0.02f,
                Top = 0.07f,
                RelativeWidth = 0.083f,
                RelativeHeight = 0.5f
            };

            var indicatorFilter1 = new StiStateIndicatorFilter
            {
                StartValue = 0f,
                EndValue = 40f,
                Brush = new StiSolidBrush(Color.FromArgb(112, 156, 28))
            };

            var indicatorFilter2 = new StiStateIndicatorFilter
            {
                StartValue = 40f,
                EndValue = 65f,
                Brush = new StiSolidBrush(Color.FromArgb(225, 174, 25))
            };

            var indicatorFilter3 = new StiStateIndicatorFilter
            {
                StartValue = 65f,
                EndValue = 100f,
                Brush = new StiSolidBrush(Color.FromArgb(194, 45, 48))
            };

            stateIndicator.Filters.Add(indicatorFilter1);
            stateIndicator.Filters.Add(indicatorFilter2);
            stateIndicator.Filters.Add(indicatorFilter3);

            linearScale.Items.Add(linearBar);
            linearScale.Items.Add(tickMarkMajor);
            linearScale.Items.Add(tickMarkMinor);
            linearScale.Items.Add(tickLabelMajor);
            linearScale.Items.Add(stateIndicator);

            gauge.Scales.Add(linearScale);
            InitializeName(gauge, report);
        }

        public static void VerticalThermometer(StiGauge gauge, StiReport report)
        {
            InitializeGauge(gauge, 50, 300);

            var linearScale = new StiLinearScale
            {
                Orientation = System.Windows.Forms.Orientation.Vertical,
                RelativeHeight = 0.85f,
                StartWidth = 0.17f,
                EndWidth = 0.17f,
                Maximum = 80f,
                MajorInterval = 20,
                MinorInterval = 4
            };

            var linearBar = new StiLinearBar
            {
                Skin = StiLinearBarSkin.VerticalThermometer,
                Placement = StiPlacement.Outside,
                StartWidth = 0.2f,
                EndWidth = 0.2f,
                UseRangeColor = true
            };

            var indicatorRange1 = new StiLinearIndicatorRangeInfo
            {
                Value = 0,
                Brush = new StiGradientBrush(Color.FromArgb(137, 188, 41), Color.FromArgb(111, 163, 14), 0f)
            };
            var indicatorRange2 = new StiLinearIndicatorRangeInfo
            {
                Value = 40,
                Brush = new StiGradientBrush(Color.FromArgb(217, 173, 45), Color.FromArgb(222, 166, 0), 0f)
            };
            var indicatorRange3 = new StiLinearIndicatorRangeInfo
            {
                Value = 65,
                Brush = new StiGradientBrush(Color.FromArgb(208, 49, 52), Color.FromArgb(186, 6, 10), 0f)
            };

            linearBar.RangeList.Add(indicatorRange1);
            linearBar.RangeList.Add(indicatorRange2);
            linearBar.RangeList.Add(indicatorRange3);

            var tickMarkMajor = new StiLinearTickMarkMajor
            {
                Placement = StiPlacement.Overlay,
                Brush = new StiSolidBrush(Color.FromArgb(159, 159, 159)),
                RelativeWidth = 0.18f,
                RelativeHeight = 0.01f,
                Offset = -0.1f
            };

            var tickMarkMinor = new StiLinearTickMarkMinor
            {
                Brush = new StiSolidBrush(Color.FromArgb(159, 159, 159)),
                RelativeWidth = 0.1f,
                RelativeHeight = 0.005f,
                Offset = -0.14f
            };

            var tickLabelMajor = new StiLinearTickLabelMajor
            {
                Placement = StiPlacement.Inside,
                Offset = 0.07f,
                TextBrush = new StiSolidBrush(Color.FromArgb(83, 85, 86)),
                Font = new Font("Arial", 10f)
            };

            var stateIndicator = new StiStateIndicator
            {
                Left = 0.05f,
                Top = 0.9f,
                RelativeWidth = 0.5f,
                RelativeHeight = 0.083f
            };

            var indicatorFilter1 = new StiStateIndicatorFilter
            {
                StartValue = 0f,
                EndValue = 40f,
                Brush = new StiSolidBrush(Color.FromArgb(112, 156, 28))
            };

            var indicatorFilter2 = new StiStateIndicatorFilter
            {
                StartValue = 40f,
                EndValue = 65f,
                Brush = new StiSolidBrush(Color.FromArgb(225, 174, 25))
            };

            var indicatorFilter3 = new StiStateIndicatorFilter
            {
                StartValue = 65f,
                EndValue = 100f,
                Brush = new StiSolidBrush(Color.FromArgb(194, 45, 48))
            };

            stateIndicator.Filters.Add(indicatorFilter1);
            stateIndicator.Filters.Add(indicatorFilter2);
            stateIndicator.Filters.Add(indicatorFilter3);

            linearScale.Items.Add(linearBar);
            linearScale.Items.Add(tickMarkMajor);
            linearScale.Items.Add(tickMarkMinor);
            linearScale.Items.Add(tickLabelMajor);
            linearScale.Items.Add(stateIndicator);

            gauge.Scales.Add(linearScale);
            InitializeName(gauge, report);
        }

        public static void LightSpeedometer(StiGauge gauge, StiReport report)
        {
            InitializeGauge(gauge, 350, 350);

            var radialScale1 = new StiRadialScale
            {
                Skin = StiRadialScaleSkin.Empty,
                Radius = 0.65f,
                StartAngle = 115,
                SweepAngle = 310,
                StartWidth = 0.03f,
                EndWidth = 0.03f,
                Brush = new StiSolidBrush(Color.FromArgb(81, 84, 101)),
                MajorInterval = 10,
                MinorInterval = 2,
                Maximum = 100
            };

            var radialRangeList1 = new StiRadialRangeList();

            var radialRange1 = new StiRadialRange
            {
                StartValue = 40,
                EndValue = 101.2f,
                StartWidth = 0.04f,
                EndWidth = 0.04f,
                Placement = StiPlacement.Inside,
                UseValuesFromTheSpecifiedRange = false,
                Brush = new StiGradientBrush(Color.FromArgb(213, 227, 118), Color.FromArgb(118, 71, 24), 90f),
                BorderBrush = new StiSolidBrush(Color.White)
            };

            radialRangeList1.Ranges.Add(radialRange1);

            var radialTickMarkMajor1 = new StiRadialTickMarkMajor
            {
                SkipValues = new StiSkipValuesExpression("100"),
                RelativeHeight = 0.03f,
                RelativeWidth = 0.055f,
                Brush = new StiSolidBrush(Color.FromArgb(81, 84, 101))
            };

            var radialTickMarkMinor1 = new StiRadialTickMarkMinor
            {
                Offset = 0.04f,
                RelativeHeight = 0.01f,
                RelativeWidth = 0.03f,
                Brush = new StiSolidBrush(Color.FromArgb(81, 84, 101))
            };

            var radialTickMarkCustom1 = new StiRadialTickMarkCustom
            {
                ValueObj = 100f,
                Offset = -0.057f,
                RelativeWidth = 0.08f,
                RelativeHeight = 0.03f,
                Brush = new StiSolidBrush(Color.FromArgb(81, 84, 101))
            };

            var radialTickLabelMajor1 = new StiRadialTickLabelMajor
            {
                LabelRotationMode = StiLabelRotationMode.None,
                Offset = 0.14f,
                TextBrush = new StiSolidBrush(Color.Black),
                Font = new Font("Arial", 13f, FontStyle.Bold)
            };

            var needle1 = new StiNeedle
            {
                Brush = new StiSolidBrush(Color.FromArgb(250, 250, 250)),
                BorderBrush = new StiSolidBrush(Color.FromArgb(163, 163, 163)),
                BorderWidth = 1f,
                Placement = StiPlacement.Outside,
                RelativeWidth = 0.63f,
                RelativeHeight = 0.14f,
                Skin = StiNeedleSkin.SpeedometerNeedle
            };

            radialScale1.Items.Add(radialRangeList1);
            radialScale1.Items.Add(radialTickMarkMajor1);
            radialScale1.Items.Add(radialTickMarkMinor1);
            radialScale1.Items.Add(radialTickMarkCustom1);
            radialScale1.Items.Add(radialTickLabelMajor1);
            radialScale1.Items.Add(needle1);

            gauge.Scales.Add(radialScale1);
            InitializeName(gauge, report);
        }

        public static void DarkSpeedometer(StiGauge gauge, StiReport report)
        {
            InitializeGauge(gauge, 350, 350);

            var radialScale1 = new StiRadialScale
            {
                Skin = StiRadialScaleSkin.Empty,
                Radius = 0.65f,
                StartAngle = 115,
                SweepAngle = 310,
                StartWidth = 0.03f,
                EndWidth = 0.03f,
                Brush = new StiSolidBrush(Color.FromArgb(81, 84, 101)),
                MajorInterval = 10,
                MinorInterval = 2,
                Maximum = 200
            };

            var radialRangeList1 = new StiRadialRangeList();

            var radialRange1 = new StiRadialRange
            {
                StartValue = 0f,
                EndValue = 60,
                BorderWidth = 1f,
                StartWidth = 0.06f,
                EndWidth = 0.06f,
                Placement = StiPlacement.Overlay,
                Brush = new StiGradientBrush(Color.FromArgb(101, 134, 101), Color.FromArgb(66, 228, 66), 90f)
            };

            var radialRange2 = new StiRadialRange
            {
                StartValue = 60f,
                EndValue = 100,
                BorderWidth = 1f,
                StartWidth = 0.06f,
                EndWidth = 0.06f,
                Placement = StiPlacement.Overlay,
                Brush = new StiGradientBrush(Color.FromArgb(255, 255, 0), Color.FromArgb(143, 174, 126), 90f)
            };

            var radialRange3 = new StiRadialRange
            {
                StartValue = 100f,
                EndValue = 140,
                BorderWidth = 1f,
                StartWidth = 0.06f,
                EndWidth = 0.06f,
                Placement = StiPlacement.Overlay,
                Brush = new StiGradientBrush(Color.FromArgb(255, 255, 0), Color.FromArgb(156, 156, 124), 90f)
            };

            var radialRange4 = new StiRadialRange
            {
                StartValue = 140f,
                EndValue = 200,
                BorderWidth = 1f,
                StartWidth = 0.06f,
                EndWidth = 0.06f,
                Placement = StiPlacement.Overlay,
                Brush = new StiGradientBrush(Color.FromArgb(125, 86, 80), Color.FromArgb(208, 45, 44), 90f)
            };

            radialRangeList1.Ranges.Add(radialRange1);
            radialRangeList1.Ranges.Add(radialRange2);
            radialRangeList1.Ranges.Add(radialRange3);
            radialRangeList1.Ranges.Add(radialRange4);

            var radialTickMarkMajor1 = new StiRadialTickMarkMajor
            {
                Placement = StiPlacement.Overlay,
                Offset = 0.04f,
                OffsetAngle = -2,
                RelativeWidth = 0.1f,
                RelativeHeight = 0.035f,
                Skin = StiTickMarkSkin.TriangleLeft
            };

            var radialTickMarkMinor1 = new StiRadialTickMarkMinor
            {
                Placement = StiPlacement.Overlay,
                SkipMajorValues = false,
                RelativeWidth = 0.04f,
                RelativeHeight = 0.018f,
                Skin = StiTickMarkSkin.Rectangle,
                SkipIndices = new StiSkipIndicesExpression("0;4;8;12;16;20;24;28;32;36;40")
            };

            var radialTickLabelMajor1 = new StiRadialTickLabelMajor
            {
                LabelRotationMode = StiLabelRotationMode.None,
                Offset = 0.05f,
                Font = new Font("Arial", 11f)
            };

            var radialTickMarkCustom1 = new StiRadialTickMarkCustom
            {
                Placement = StiPlacement.Inside,
                Offset = 0.3f,
                RelativeHeight = 0.05f,
                RelativeWidth = 0.05f,
                Skin = StiTickMarkSkin.Ellipse
            };

            var custom1 = new StiRadialTickMarkCustomValue(20)
            {
                Brush = new StiGradientBrush(Color.FromArgb(68, 223, 68), Color.FromArgb(0, 153, 0), 90f)
            };
            var custom2 = new StiRadialTickMarkCustomValue(97)
            {
                Brush = new StiGradientBrush(Color.FromArgb(255, 255, 0), Color.FromArgb(186, 169, 2), 90f)
            };
            var custom3 = new StiRadialTickMarkCustomValue(173)
            {
                Brush = new StiGradientBrush(Color.FromArgb(184, 29, 29), Color.FromArgb(121, 30, 30), 90f)
            };
            radialTickMarkCustom1.Values.Add(custom1);
            radialTickMarkCustom1.Values.Add(custom2);
            radialTickMarkCustom1.Values.Add(custom3);

            var radialTickLabelCustom1 = new StiRadialTickLabelCustom
            {
                Placement = StiPlacement.Inside,
                LabelRotationMode = StiLabelRotationMode.None,
                Font = new Font("Arial", 10f)
            };

            radialTickLabelCustom1.Values.Add(new StiRadialTickLabelCustomValue(17, "Safe", 0.16f));
            radialTickLabelCustom1.Values.Add(new StiRadialTickLabelCustomValue(102, "Caution", 0.25f));
            radialTickLabelCustom1.Values.Add(new StiRadialTickLabelCustomValue(181, "Danger", 0.06f));

            var needle1 = new StiNeedle
            {
                BorderBrush = new StiSolidBrush(Color.FromArgb(153, 9, 8)),
                Brush = new StiGradientBrush(Color.FromArgb(255, 198, 172), Color.FromArgb(197, 25, 19), 90f),
                BorderWidth = 1f,
                Placement = StiPlacement.Outside,
                RelativeWidth = 0.57f,
                RelativeHeight = 0.17f,
                Skin = StiNeedleSkin.SpeedometerNeedle2
            };

            radialScale1.Items.Add(radialRangeList1);
            radialScale1.Items.Add(radialTickMarkMajor1);
            radialScale1.Items.Add(radialTickMarkMinor1);
            radialScale1.Items.Add(radialTickLabelMajor1);
            radialScale1.Items.Add(radialTickMarkCustom1);
            radialScale1.Items.Add(radialTickLabelCustom1);
            radialScale1.Items.Add(needle1);

            gauge.Scales.Add(radialScale1);
            InitializeName(gauge, report);
        }
        #endregion
    }
}