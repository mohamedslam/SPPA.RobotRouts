#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports.Net											}
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
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Map;
using Stimulsoft.Base.Maps.Geoms;
using Stimulsoft.Report.Maps.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Stimulsoft.Report.Maps
{
    public static class StiMapLoader
    {
        #region enum StiGeomIdent
        private enum StiGeomIdent
        {
            None = 1,
            MoveToM,
            MoveTom,
            Line_L,
            Line_l,
            Bezier_C,
            Bezier_c,
            Beziers_S,
            Beziers_s,
            QuadraticCurve_T,
            QuadraticCurve_t,
            VerticalLineto_V,
            VerticalLineto_v,
            HorizontalLineto_H,
            HorizontalLineto_h,
            Close
        }
        #endregion

        #region Fields
        private static Hashtable hashMaps;
        private static IStiMapResourceFinder resourceFinder;
        private static bool isLoading = false;
        #endregion

        #region Properties
        public static bool IsMapAssemblyLoaded { get; private set; }
        #endregion

        #region Methods
        public static void DeleteAllCustomMaps()
        {
            if (hashMaps != null && hashMaps.Count > 0)
            {
                var keys = new List<string>();

                foreach (string key in hashMaps.Keys)
                {
                    var container = (StiMapSvgContainer)hashMaps[key];
                    if (container.IsCustom)
                        keys.Add(key);
                }

                foreach (var key in keys)
                {
                    hashMaps.Remove(key);
                }
            }
        }

        private static string FinalResourceNmae(string resourceName, string lang)
        {
            if (resourceName.ToLowerInvariant() == "Germany".ToLowerInvariant() && lang == "DE")
                return "Germany_DE";
            if (resourceName.ToLowerInvariant() == "France".ToLowerInvariant() && lang == "FR")
                return "France_FR";
            if (resourceName.ToLowerInvariant() == "Italy".ToLowerInvariant() && lang == "IT")
                return "Italy_IT";
            if (resourceName.ToLowerInvariant() == "Russia".ToLowerInvariant() && lang == "RU")
                return "Russia_RU";

            return resourceName;
        }

        public static StiMapSvgContainer LoadResource(StiReport report, string resourceName, string lang)
        {
            if (hashMaps == null)
                hashMaps = new Hashtable();

            var finalResourceName = FinalResourceNmae(resourceName, lang);

            if (hashMaps.ContainsKey(finalResourceName))
                return (StiMapSvgContainer) hashMaps[finalResourceName];

            if (resourceFinder == null)
            {
                if (isLoading)
                    return null;

                try
                {
                    isLoading = true;
                    var assembly = StiAssemblyFinder.GetAssembly($"Stimulsoft.Map, {StiVersion.VersionInfo}");
                    if (assembly == null)
                        return null;

                    IsMapAssemblyLoaded = true;
                    var type = StiTypeFinder.GetType($"Stimulsoft.Map.StiMapResourceFinder, Stimulsoft.Map, {StiVersion.VersionInfo}");

                    resourceFinder = Activator.CreateInstance(type) as IStiMapResourceFinder;

                    if (resourceFinder == null)
                        return null;
                }
                catch
                {
                }
            }

            StiMapSvgContainer container;
            if (StiCustomMapFinder.IsCustom(finalResourceName))
            {
                container = StiCustomMapFinder.GetContainer(report, finalResourceName);
            }
            else
            {
                string jsonText = resourceFinder.Get(finalResourceName);
                container = new StiMapSvgContainer();

                JsonConvert.PopulateObject(jsonText, container, StiJsonHelper.DefaultSerializerSettings);
                container.Prepare();
            }

            if (!hashMaps.ContainsKey(finalResourceName))
                hashMaps.Add(finalResourceName, container);

            return container;
        }

        private static PointD Reflect(double pointX, double pointY, PointD mirror)
        {
            var dx = Math.Abs(mirror.X - pointX);
            var dy = Math.Abs(mirror.Y - pointY);

            var x = mirror.X + (mirror.X >= pointX ? dx : -dx);
            var y = mirror.Y + (mirror.Y >= pointY ? dy : -dy);

            return new PointD(x, y);
        }

        public static StiMapGeomsContainer GetGeomsObject(StiReport report, string resourceName, string lang)
        {
            var container = LoadResource(report, resourceName, lang);
            if (container == null)
                return new StiMapGeomsContainer();

            var result = new StiMapGeomsContainer
            {
                Width = container.Width,
                Height = container.Height,
                Name = container.Name
            };

            if (container.Geoms == null)
            {
                var duration = TimeSpan.FromMilliseconds(100);
                var beginTime = TimeSpan.Zero;

                foreach (string key in container.HashPaths.Keys)
                {
                    var pt = container.HashPaths[key];
                    var obj = new StiMapGeomsObject
                    {
                        Key = key,
                        Geoms = ParsePath(pt.Data),
                        Animation = new StiOpacityAnimation(duration, beginTime)
                    };
                    result.Geoms.Add(obj);
                    beginTime = TimeSpan.FromMilliseconds(200 / container.HashPaths.Count * result.Geoms.Count);
                }

                container.Geoms = result.Geoms;
            }
            else
            {
                result.Geoms = container.Geoms;
            }

            return result;
        }
        #endregion

        #region Methods.Helper
        private static void CreateGeom(StiGeomIdent ident, List<double> values, StiMapGeomCollection geoms, bool skipException = false)
        {
            switch (ident)
            {
                case StiGeomIdent.MoveToM:
                    {
                        if (values.Count != 2)
                        {
                            if (skipException) return;
                            throw new NotSupportedException();
                        }

                        var geom = new StiMoveToMapGeom
                        {
                            X = Math.Round(values[0], 3),
                            Y = Math.Round(values[1], 3)
                        };
                        geoms.Add(geom);
                    }
                    break;

                case StiGeomIdent.MoveTom:
                    {
                        if (values.Count != 2)
                        {
                            if (skipException) return;
                            throw new NotSupportedException();
                        }

                        var lastPos = geoms.GetLastPoint();
                        var geom = new StiMoveToMapGeom
                        {
                            X = Math.Round(lastPos.X + values[0], 3),
                            Y = Math.Round(lastPos.Y + values[1], 3)
                        };
                        geoms.Add(geom);
                    }
                    break;

                case StiGeomIdent.Line_L:
                    {
                        if (values.Count != 2)
                        {
                            if (skipException) return;

                            if (values.Count % 2 == 0)
                            {
                                for (int index3 = 0; index3 < values.Count; index3 += 2)
                                {
                                    var geom1 = new StiLineMapGeom
                                    {
                                        X = Math.Round(values[index3], 3),
                                        Y = Math.Round(values[index3 + 1], 3)
                                    };
                                    geoms.Add(geom1);
                                }

                                break;
                            }
                            throw new NotSupportedException();
                        }

                        var geom = new StiLineMapGeom
                        {
                            X = Math.Round(values[0], 3),
                            Y = Math.Round(values[1], 3)
                        };
                        geoms.Add(geom);
                    }
                    break;

                case StiGeomIdent.Line_l:
                    {
                        if (values.Count % 2 != 0)
                        {
                            if (skipException) return;
                            throw new NotSupportedException();
                        }

                        var lastPos = geoms[geoms.Count - 1].GetLastPoint();

                        if (values.Count == 2)
                        {
                            var geom = new StiLineMapGeom
                            {
                                X = Math.Round(lastPos.X + values[0], 3),
                                Y = Math.Round(lastPos.Y + values[1], 3)
                            };
                            geoms.Add(geom);
                        }
                        else
                        {
                            int index5 = 0;
                            while (index5 < values.Count)
                            {
                                var geom = new StiLineMapGeom
                                {
                                    X = Math.Round(lastPos.X + values[index5], 3),
                                    Y = Math.Round(lastPos.Y + values[index5 + 1], 3)
                                };
                                geoms.Add(geom);

                                lastPos = new PointD(geom.X, geom.Y);
                                index5 += 2;
                            }
                        }
                    }
                    break;


                case StiGeomIdent.QuadraticCurve_T:
                case StiGeomIdent.QuadraticCurve_t:
                    {
                        StiQuadraticCurveMapGeom lastQuadCurve = null;
                        PointD lastPos;
                        PointD controlPoint;
                        if (values.Count != 2)
                        {
                            if (skipException) return;

                            if (values.Count % 2 == 0)
                            {
                                for (int index3 = 0; index3 < values.Count; index3 += 2)
                                {
                                    lastQuadCurve = geoms.LastOrDefault() as StiQuadraticCurveMapGeom;
                                    lastPos = geoms[geoms.Count - 1].GetLastPoint();

                                    controlPoint = lastQuadCurve != null
                                        ? Reflect(lastQuadCurve.ControlPointX, lastQuadCurve.ControlPointY, lastPos)
                                        : lastPos;

                                    var geom1 = new StiQuadraticCurveMapGeom
                                    {
                                        StartX = lastPos.X,
                                        StartY = lastPos.Y,
                                        ControlPointX = controlPoint.X,
                                        ControlPointY = controlPoint.Y,
                                        EndX = Math.Round(values[index3], 3),
                                        EndY = Math.Round(values[index3 + 1], 3),
                                    };
                                    if (ident == StiGeomIdent.QuadraticCurve_t)
                                    {
                                        geom1.EndX += lastPos.X;
                                        geom1.EndY += lastPos.Y;
                                    }
                                    geoms.Add(geom1);
                                }

                                break;
                            }
                            throw new NotSupportedException();
                        }

                        lastQuadCurve = geoms.LastOrDefault() as StiQuadraticCurveMapGeom;
                        lastPos = geoms[geoms.Count - 1].GetLastPoint();

                        controlPoint = lastQuadCurve != null
                            ? Reflect(lastQuadCurve.ControlPointX, lastQuadCurve.ControlPointY, lastPos)
                            : lastPos;
                        var geom = new StiQuadraticCurveMapGeom
                        {
                            StartX = lastPos.X,
                            StartY = lastPos.Y,
                            ControlPointX = controlPoint.X,
                            ControlPointY = controlPoint.Y,
                            EndX = Math.Round(values[0], 3),
                            EndY = Math.Round(values[1], 3)
                        };
                        if (ident == StiGeomIdent.QuadraticCurve_t)
                        {
                            geom.EndX += lastPos.X;
                            geom.EndY += lastPos.Y;
                        }
                        geoms.Add(geom);
                    }
                    break;

                case StiGeomIdent.Bezier_C:
                    {
                        //if (values.Count == 5)
                        //{
                        //    values.Add(values[4]);
                        //}
                        if (values.Count != 6)
                        {
                            if (skipException) return;
                            throw new NotSupportedException();
                        }

                        var geom = new StiBezierMapGeom
                        {
                            X1 = Math.Round(values[0], 3),
                            Y1 = Math.Round(values[1], 3),

                            X2 = Math.Round(values[2], 3),
                            Y2 = Math.Round(values[3], 3),

                            X3 = Math.Round(values[4], 3),
                            Y3 = Math.Round(values[5], 3)
                        };
                        geoms.Add(geom);
                    }
                    break;

                case StiGeomIdent.Bezier_c:
                    {
                        //if (values.Count == 5)
                        //{
                        //    values.Add(values[4]);
                        //}
                        if (values.Count % 6 != 0)
                        {
                            if (skipException) return;
                            throw new NotSupportedException();
                        }

                        var lastPos = geoms[geoms.Count - 1].GetLastPoint();

                        int steps = values.Count / 6;
                        int indexC = 0;
                        for (int index2 = 0; index2 < steps; index2++)
                        {
                            var geom = new StiBezierMapGeom
                            {
                                X1 = Math.Round(lastPos.X + values[indexC], 3),
                                Y1 = Math.Round(lastPos.Y + values[indexC + 1], 3),

                                X2 = Math.Round(lastPos.X + values[indexC + 2], 3),
                                Y2 = Math.Round(lastPos.Y + values[indexC + 3], 3),

                                X3 = Math.Round(lastPos.X + values[indexC + 4], 3),
                                Y3 = Math.Round(lastPos.Y + values[indexC + 5], 3)
                            };
                            geoms.Add(geom);

                            lastPos = new PointD(geom.X3, geom.Y3);
                            indexC += 6;
                        }
                    }
                    break;

                case StiGeomIdent.VerticalLineto_V:
                    {
                        var lastPoint = geoms[geoms.Count - 1].GetLastPoint();
                        var geom = new StiLineMapGeom
                        {
                            X = Math.Round(lastPoint.X, 3),
                            Y = Math.Round(values[values.Count - 1], 3)
                        };
                        geoms.Add(geom);
                    }
                    break;

                case StiGeomIdent.VerticalLineto_v:
                    {
                        var lastPoint = geoms[geoms.Count - 1].GetLastPoint();
                        var geom = new StiLineMapGeom
                        {
                            X = Math.Round(lastPoint.X, 3),
                            Y = Math.Round(lastPoint.Y + values[values.Count - 1], 3)
                        };
                        geoms.Add(geom);
                    }
                    break;

                case StiGeomIdent.HorizontalLineto_H:
                    {
                        var lastPoint = geoms[geoms.Count - 1].GetLastPoint();
                        var geom = new StiLineMapGeom
                        {
                            X = Math.Round(values[values.Count - 1], 3),
                            Y = Math.Round(lastPoint.Y, 3)
                        };
                        geoms.Add(geom);
                    }
                    break;

                case StiGeomIdent.HorizontalLineto_h:
                    {
                        var lastPoint = geoms[geoms.Count - 1].GetLastPoint();
                        var geom = new StiLineMapGeom
                        {
                            X = Math.Round(lastPoint.X + values[values.Count - 1], 3),
                            Y = Math.Round(lastPoint.Y, 3)
                        };
                        geoms.Add(geom);
                    }
                    break;

                case StiGeomIdent.Beziers_S:
                    {
                        if (values.Count < 6)
                        {
                            if (skipException) return;
                            throw new NotSupportedException();
                        }

                        var list = new List<double>();
                        foreach (var value in values)
                        {
                            list.Add(Math.Round(value, 3));
                        }

                        var geom = new StiBeziersMapGeom
                        {
                            Array = list.ToArray()
                        };
                        geoms.Add(geom);
                    }
                    break;


                case StiGeomIdent.Beziers_s:
                    {
                        if (values.Count < 6)
                        {
                            if (skipException) return;
                            throw new NotSupportedException();
                        }

                        var lastPoint = geoms[geoms.Count - 1].GetLastPoint();
                        lastPoint.X += values[4];
                        lastPoint.Y += values[5];

                        bool state = true;
                        var list = new List<double>();
                        foreach (double value in values)
                        {
                            double newValue = state ? value + lastPoint.X : value + lastPoint.Y;
                            list.Add(Math.Round(newValue, 3));

                            state = !state;
                        }

                        var geom = new StiBeziersMapGeom
                        {
                            SvgValues = values.ToArray(),
                            Array = list.ToArray()
                        };
                        geoms.Add(geom);

                        list.Clear();
                        list = null;
                    }
                    break;

                case StiGeomIdent.Close:
                    {
                        if (values.Count != 0)
                        {
                            if (skipException) return;
                            throw new NotSupportedException();
                        }

                        geoms.Add(new StiCloseMapGeom());
                    }
                    break;
            }

            values.Clear();
        }

        public static List<StiMapGeom> ParsePath(string text)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                var geoms = new StiMapGeomCollection();

                int startIndex = 0;
                int valuesCount = 0;
                var ident = StiGeomIdent.None;

                int valueIndex = 0;
                var values = new List<double>();

                double temp = 0.0;

                int index = 0;
                int count = text.Length;
                while (index < count)
                {
                    char ch = text[index];

                    switch (ch)
                    {
                        case 'M':
                            {
                                if (double.TryParse(text.Substring(valueIndex, index - valueIndex), out temp))
                                    values.Add(temp);

                                if (ident != StiGeomIdent.None)
                                {
                                    CreateGeom(ident, values, geoms);
                                    valuesCount = 0;
                                }

                                ident = StiGeomIdent.MoveToM;
                                startIndex = index + 1;
                                valueIndex = startIndex;
                            }
                            break;

                        case 'm':
                            {
                                if (double.TryParse(text.Substring(valueIndex, index - valueIndex), out temp))
                                    values.Add(temp);

                                if (ident != StiGeomIdent.None)
                                {
                                    CreateGeom(ident, values, geoms);
                                    valuesCount = 0;
                                }

                                ident = StiGeomIdent.MoveTom;
                                startIndex = index + 1;
                                valueIndex = startIndex;
                            }
                            break;

                        case 'C':
                            {
                                if (double.TryParse(text.Substring(valueIndex, index - valueIndex), out temp))
                                    values.Add(temp);

                                if (ident != StiGeomIdent.None)
                                    CreateGeom(ident, values, geoms);

                                ident = StiGeomIdent.Bezier_C;
                                startIndex = index + 1;
                                valueIndex = startIndex;
                            }
                            break;

                        case 'c':
                            {
                                if (double.TryParse(text.Substring(valueIndex, index - valueIndex), out temp))
                                    values.Add(temp);

                                if (ident != StiGeomIdent.None)
                                    CreateGeom(ident, values, geoms);

                                ident = StiGeomIdent.Bezier_c;
                                startIndex = index + 1;
                                valueIndex = startIndex;
                            }
                            break;

                        case 'S':
                            {
                                if (double.TryParse(text.Substring(valueIndex, index - valueIndex), out temp))
                                    values.Add(temp);

                                ident = StiGeomIdent.Beziers_S;
                                startIndex = index + 1;
                                valueIndex = startIndex;
                            }
                            break;

                        case 's':
                            {
                                if (double.TryParse(text.Substring(valueIndex, index - valueIndex), out temp))
                                    values.Add(temp);
                                
                                ident = StiGeomIdent.Beziers_s;
                                startIndex = index + 1;
                                valueIndex = startIndex;
                            }
                            break;

                        case 'L':
                            {
                                if (double.TryParse(text.Substring(valueIndex, index - valueIndex), out temp))
                                    values.Add(temp);

                                if (ident != StiGeomIdent.None)
                                    CreateGeom(ident, values, geoms);

                                ident = StiGeomIdent.Line_L;
                                startIndex = index + 1;
                                valueIndex = startIndex;
                            }
                            break;

                        case 'l':
                            {
                                if (double.TryParse(text.Substring(valueIndex, index - valueIndex), out temp))
                                    values.Add(temp);

                                if (ident != StiGeomIdent.None)
                                    CreateGeom(ident, values, geoms);

                                ident = StiGeomIdent.Line_l;
                                startIndex = index + 1;
                                valueIndex = startIndex;
                            }
                            break;

                        case 'T':
                        case 't':
                            {
                                if (double.TryParse(text.Substring(valueIndex, index - valueIndex), out temp))
                                    values.Add(temp);

                                if (ident != StiGeomIdent.None)
                                    CreateGeom(ident, values, geoms);

                                ident = (ch == 't') ? StiGeomIdent.QuadraticCurve_t : StiGeomIdent.QuadraticCurve_T;
                                startIndex = index + 1;
                                valueIndex = startIndex;
                            }
                            break;

                        case 'Z':
                        case 'z':
                            {
                                if (double.TryParse(text.Substring(valueIndex, index - valueIndex), out temp))
                                    values.Add(temp);

                                if (ident != StiGeomIdent.None)
                                    CreateGeom(ident, values, geoms);

                                ident = StiGeomIdent.Close;
                                startIndex = index + 1;
                                valueIndex = startIndex;
                            }
                            break;

                        case 'V':
                            {
                                if (double.TryParse(text.Substring(valueIndex, index - valueIndex), out temp))
                                    values.Add(temp);

                                if (ident != StiGeomIdent.None)
                                    CreateGeom(ident, values, geoms);

                                ident = StiGeomIdent.VerticalLineto_V;
                                startIndex = index + 1;
                                valueIndex = startIndex;
                            }
                            break;

                        case 'v':
                            {
                                if (double.TryParse(text.Substring(valueIndex, index - valueIndex), out temp))
                                    values.Add(temp);

                                if (ident != StiGeomIdent.None)
                                    CreateGeom(ident, values, geoms);

                                ident = StiGeomIdent.VerticalLineto_v;
                                startIndex = index + 1;
                                valueIndex = startIndex;
                            }
                            break;

                        case 'H':
                            {
                                if (double.TryParse(text.Substring(valueIndex, index - valueIndex), out temp))
                                    values.Add(temp);

                                if (ident != StiGeomIdent.None)
                                    CreateGeom(ident, values, geoms);

                                ident = StiGeomIdent.HorizontalLineto_H;
                                startIndex = index + 1;
                                valueIndex = startIndex;
                            }
                            break;

                        case 'h':
                            {
                                if (double.TryParse(text.Substring(valueIndex, index - valueIndex), out temp))
                                    values.Add(temp);

                                if (ident != StiGeomIdent.None)
                                    CreateGeom(ident, values, geoms);

                                ident = StiGeomIdent.HorizontalLineto_h;
                                startIndex = index + 1;
                                valueIndex = startIndex;
                            }
                            break;

                        case 'A':
                        case 'a':
                            {
                                if (double.TryParse(text.Substring(valueIndex, index - valueIndex), out temp))
                                    values.Add(temp);

                                if (ident != StiGeomIdent.None)
                                    CreateGeom(ident, values, geoms);

                                ident = StiGeomIdent.None;
                                ParseEllipticalArc(text, ref index, ref geoms, ch == 'a');
                                index--;
                                startIndex = index + 1;
                                valueIndex = startIndex;
                            }
                            break;

                        case '-':
                            {
                                valuesCount++;
                                if (double.TryParse(text.Substring(valueIndex, index - valueIndex), out temp))
                                    values.Add(temp);

                                valueIndex = index;
                            }
                            break;

                        case ',':
                        case ' ':
                            {
                                valuesCount++;
                                if (double.TryParse(text.Substring(valueIndex, index - valueIndex), out temp))
                                    values.Add(temp);
                                valueIndex = index + 1;

                                if (values.Count == 2)
                                {
                                    if (ident == StiGeomIdent.MoveToM || ident == StiGeomIdent.MoveTom)
                                    {
                                        CreateGeom(ident, values, geoms);
                                        valuesCount = 0;
                                        ident = StiGeomIdent.None;
                                    }
                                    else if (ident == StiGeomIdent.None)
                                    {
                                        CreateGeom(StiGeomIdent.Line_l, values, geoms);
                                        valuesCount = 0;
                                    }
                                }
                            }
                            break;


                        case 'Q':
                        case 'q':
                            throw new Exception("");
                    }

                    index++;
                }

                if (double.TryParse(text.Substring(valueIndex, index - valueIndex), out temp))
                    values.Add(temp);

                if (ident != StiGeomIdent.None && values.Count > 1)
                    CreateGeom(ident, values, geoms);

                return geoms;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        private static void ParseEllipticalArc(string text, ref int index, ref StiMapGeomCollection geoms, bool isRelative)
        {
            index++;
            int startIndex = index;

            int count = text.Length;
            while (index < count)
            {
                char ch = text[index];
                if (IsCloseEllipticalArcSymbol(ch))
                {
                    break;
                }

                index++;
            }

            var ellipticalArcText = text.Substring(startIndex, index - startIndex);

            var lastPos = geoms[geoms.Count - 1].GetLastPoint();

            int index1 = 0;
            var geom = new StiEllipticalArcMapGeom();
            geom.StartX = lastPos.X;
            geom.StartY = lastPos.Y;

            geom.RadiusX = ParseDouble(ellipticalArcText, ref index1);
            geom.RadiusY = ParseDouble(ellipticalArcText, ref index1);
            geom.Angle = ParseDouble(ellipticalArcText, ref index1);
            var size = ParseBool(ellipticalArcText, ref index1);
            geom.Size = size ? StiSvgArcSize.Large : StiSvgArcSize.Small;
            var sweep = ParseBool(ellipticalArcText, ref index1);
            geom.Sweep = sweep ? StiSvgArcSweep.Positive : StiSvgArcSweep.Negative;
            geom.EndX = ParseDouble(ellipticalArcText, ref index1);
            geom.EndY = ParseDouble(ellipticalArcText, ref index1);
            if (isRelative)
            {
                geom.EndX += lastPos.X;
                geom.EndY += lastPos.Y;
            }

            geoms.Add(geom);
        }

        private static double ParseDouble(string text, ref int index1)
        {
            int startIndex = index1;
            int count1 = text.Length;
            while (index1 < count1)
            {
                char ch = text[index1];
                if (ch == ',' || ch == ' ') break;
                if (ch == '-' && index1 - startIndex > 0) break;

                index1++;
            }

            int len = index1 - startIndex;
            if (len == 0)
                throw new Exception("");

            index1++;
            return double.Parse(text.Substring(startIndex, len).Trim());
        }

        private static bool ParseBool(string text, ref int index1)
        {
            int count1 = text.Length;
            while (index1 < count1)
            {
                char ch = text[index1];
                if (Char.IsDigit(ch))
                {
                    index1++;
                    return ch == '1';
                }

                index1++;
            }

            throw new Exception("");
        }

        private static bool IsCloseEllipticalArcSymbol(char ch)
        {
            switch (ch)
            {
                case 'M':
                case 'm':
                case 'C':
                case 'c':
                case 'S':
                case 's':
                case 'L':
                case 'l':
                case 'Z':
                case 'z':
                case 'V':
                case 'v':
                case 'H':
                case 'h':
                case 'A':
                case 'a':
                case 'T':
                case 't':
                    return true;

                case 'Q':
                case 'q':
                    throw new Exception("");
            }

            return false;
        }
        #endregion
    }
}