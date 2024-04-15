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
using System.Collections;
using System.Collections.Generic;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using System.Globalization;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Engine
{
    public partial class StiParser
    {
        #region TypesList
        private static Hashtable typesList = null;
        private static Hashtable typesList_low = null;
        private static Hashtable typesList_vb = null;
        private Hashtable TypesList
        {
            get
            {
                if (typesList == null)
                {
                    typesList = new Hashtable();
                    typesList["bool"] = TypeCode.Boolean;
                    typesList["Boolean"] = TypeCode.Boolean;
                    typesList["byte"] = TypeCode.Byte;
                    typesList["Byte"] = TypeCode.Byte;
                    typesList["sbyte"] = TypeCode.SByte;
                    typesList["Sbyte"] = TypeCode.SByte;
                    typesList["char"] = TypeCode.Char;
                    typesList["Char"] = TypeCode.Char;
                    typesList["decimal"] = TypeCode.Decimal;
                    typesList["Decimal"] = TypeCode.Decimal;
                    typesList["double"] = TypeCode.Double;
                    typesList["Double"] = TypeCode.Double;
                    typesList["float"] = TypeCode.Single;
                    typesList["Single"] = TypeCode.Single;
                    typesList["int"] = TypeCode.Int32;
                    typesList["uint"] = TypeCode.UInt32;
                    typesList["long"] = TypeCode.Int64;
                    typesList["ulong"] = TypeCode.UInt64;
                    typesList["short"] = TypeCode.Int16;
                    typesList["Int16"] = TypeCode.Int16;
                    typesList["Int32"] = TypeCode.Int32;
                    typesList["Int64"] = TypeCode.Int64;
                    typesList["ushort"] = TypeCode.UInt16;
                    typesList["UInt16"] = TypeCode.UInt16;
                    typesList["UInt32"] = TypeCode.UInt32;
                    typesList["UInt64"] = TypeCode.UInt64;
                    typesList["object"] = TypeCode.Object;
                    typesList["string"] = TypeCode.String;
                    typesList["String"] = TypeCode.String;
                    typesList["DateTime"] = TypeCode.DateTime;
                    typesList["TimeSpan"] = typeof(TimeSpan);
                    typesList["List"] = typeof(List<>);

                    typesList_low = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                    foreach (DictionaryEntry de in typesList)
                    {
                        typesList_low[((string)de.Key).ToLowerInvariant()] = de.Value;
                    }

                    typesList_vb = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                    typesList_vb["Boolean"] = TypeCode.Boolean;
                    typesList_vb["Byte"] = TypeCode.Byte;
                    typesList_vb["Char"] = TypeCode.Char;
                    typesList_vb["Date"] = TypeCode.DateTime;
                    typesList_vb["Decimal"] = TypeCode.Decimal;
                    typesList_vb["Double"] = TypeCode.Double;
                    typesList_vb["Integer"] = TypeCode.Int32;
                    typesList_vb["Long"] = TypeCode.Int64;
                    typesList_vb["Object"] = TypeCode.Object;
                    typesList_vb["SByte"] = TypeCode.SByte;
                    typesList_vb["Short"] = TypeCode.Int16;
                    typesList_vb["Single"] = TypeCode.Single;
                    typesList_vb["String"] = TypeCode.String;
                    typesList_vb["UInteger"] = TypeCode.UInt32;
                    typesList_vb["ULong"] = TypeCode.UInt64;
                    typesList_vb["UShort"] = TypeCode.UInt16;
                    typesList_vb["TimeSpan"] = typeof(TimeSpan);
                }

                if (IsVB) return typesList_vb;
                return syntaxCaseSensitive ? typesList : typesList_low;
            }
        }
        #endregion

        #region SystemVariablesList
        private static Hashtable systemVariablesList = null;
        private static Hashtable systemVariablesList_low = null;
        private Hashtable SystemVariablesList
        {
            get
            {
                if (systemVariablesList == null)
                {
                    systemVariablesList = new Hashtable();
                    systemVariablesList["Column"] = StiSystemVariableType.Column;
                    systemVariablesList["Line"] = StiSystemVariableType.Line;
                    systemVariablesList["LineThrough"] = StiSystemVariableType.LineThrough;
                    systemVariablesList["LineABC"] = StiSystemVariableType.LineABC;
                    systemVariablesList["LineRoman"] = StiSystemVariableType.LineRoman;
                    systemVariablesList["GroupLine"] = StiSystemVariableType.GroupLine;
                    systemVariablesList["PageNumber"] = StiSystemVariableType.PageNumber;
                    systemVariablesList["PageNumberThrough"] = StiSystemVariableType.PageNumberThrough;
                    systemVariablesList["PageNofM"] = StiSystemVariableType.PageNofM;
                    systemVariablesList["PageNofMThrough"] = StiSystemVariableType.PageNofMThrough;
                    systemVariablesList["TotalPageCount"] = StiSystemVariableType.TotalPageCount;
                    systemVariablesList["TotalPageCountThrough"] = StiSystemVariableType.TotalPageCountThrough;
                    systemVariablesList["IsFirstPage"] = StiSystemVariableType.IsFirstPage;
                    systemVariablesList["IsFirstPageThrough"] = StiSystemVariableType.IsFirstPageThrough;
                    systemVariablesList["IsLastPage"] = StiSystemVariableType.IsLastPage;
                    systemVariablesList["IsLastPageThrough"] = StiSystemVariableType.IsLastPageThrough;
                    systemVariablesList["PageCopyNumber"] = StiSystemVariableType.PageCopyNumber;
                    systemVariablesList["ReportAlias"] = StiSystemVariableType.ReportAlias;
                    systemVariablesList["ReportAuthor"] = StiSystemVariableType.ReportAuthor;
                    systemVariablesList["ReportChanged"] = StiSystemVariableType.ReportChanged;
                    systemVariablesList["ReportCreated"] = StiSystemVariableType.ReportCreated;
                    systemVariablesList["ReportDescription"] = StiSystemVariableType.ReportDescription;
                    systemVariablesList["ReportName"] = StiSystemVariableType.ReportName;
                    systemVariablesList["Time"] = StiSystemVariableType.Time;
                    systemVariablesList["Today"] = StiSystemVariableType.Today;
                    systemVariablesList["Date"] = StiSystemVariableType.Today;
                    systemVariablesList["value"] = StiSystemVariableType.ConditionValue;
                    systemVariablesList["index"] = StiSystemVariableType.Index;
                    systemVariablesList["value2"] = StiSystemVariableType.ConditionValue2;
                    systemVariablesList["tag"] = StiSystemVariableType.ConditionTag;
                    systemVariablesList["sender"] = StiSystemVariableType.Sender;

                    systemVariablesList["DateTime.Now"] = StiSystemVariableType.DateTimeNow;
                    systemVariablesList["DateTime.Today"] = StiSystemVariableType.DateTimeToday;
                    systemVariablesList["DateTime.UtcNow"] = StiSystemVariableType.DateTimeUtcNow;

                    systemVariablesList_low = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                    foreach (DictionaryEntry de in systemVariablesList)
                    {
                        systemVariablesList_low[((string)de.Key).ToLowerInvariant()] = de.Value;
                    }
                }
                return syntaxCaseSensitive ? systemVariablesList : systemVariablesList_low;
            }
        }
        #endregion

        #region PropertiesList
        private static Hashtable propertiesList = null;
        private static Hashtable propertiesList_low = null;
        private Hashtable PropertiesList
        {
            get
            {
                if (propertiesList == null)
                {
                    propertiesList = new Hashtable();
                    propertiesList["Year"] = StiPropertyType.Year;
                    propertiesList["Month"] = StiPropertyType.Month;
                    propertiesList["Day"] = StiPropertyType.Day;
                    propertiesList["Hour"] = StiPropertyType.Hour;
                    propertiesList["Minute"] = StiPropertyType.Minute;
                    propertiesList["Second"] = StiPropertyType.Second;
                    propertiesList["Date"] = StiPropertyType.Date;
                    propertiesList["Length"] = StiPropertyType.Length;
                    propertiesList["From"] = StiPropertyType.From;
                    propertiesList["To"] = StiPropertyType.To;
                    propertiesList["FromDate"] = StiPropertyType.FromDate;
                    propertiesList["ToDate"] = StiPropertyType.ToDate;
                    propertiesList["FromTime"] = StiPropertyType.FromTime;
                    propertiesList["ToTime"] = StiPropertyType.ToTime;
                    propertiesList["SelectedLine"] = StiPropertyType.SelectedLine;
                    propertiesList["Name"] = StiPropertyType.Name;
                    propertiesList["TagValue"] = StiPropertyType.TagValue;

                    propertiesList["Days"] = StiPropertyType.Days;
                    propertiesList["Hours"] = StiPropertyType.Hours;
                    propertiesList["Milliseconds"] = StiPropertyType.Milliseconds;
                    propertiesList["Minutes"] = StiPropertyType.Minutes;
                    propertiesList["Seconds"] = StiPropertyType.Seconds;
                    propertiesList["Ticks"] = StiPropertyType.Ticks;
                    propertiesList["TotalDays"] = StiPropertyType.TotalDays;
                    propertiesList["TotalHours"] = StiPropertyType.TotalHours;
                    propertiesList["TotalMinutes"] = StiPropertyType.TotalMinutes;
                    propertiesList["TotalSeconds"] = StiPropertyType.TotalSeconds;
                    propertiesList["TotalMilliseconds"] = StiPropertyType.TotalMilliseconds;
                    propertiesList["DayOfWeek"] = StiPropertyType.DayOfWeek;

                    propertiesList["Count"] = StiPropertyType.Count;
                    propertiesList["BusinessObjectValue"] = StiPropertyType.BusinessObjectValue;
                    propertiesList["Position"] = StiPropertyType.Position;
                    propertiesList["Line"] = StiPropertyType.Line;
                    propertiesList["Rows"] = StiPropertyType.Rows;

                    propertiesList["Checked"] = StiPropertyType.Checked;
                    propertiesList["SelectedIndex"] = StiPropertyType.SelectedIndex;
                    propertiesList["SelectedValue"] = StiPropertyType.SelectedValue;
                    propertiesList["SelectedLabel"] = StiPropertyType.SelectedLabel;
                    propertiesList["SelectedItem"] = StiPropertyType.SelectedItem;
                    propertiesList["Text"] = StiPropertyType.Text;
                    propertiesList["Value"] = StiPropertyType.Value;
                    propertiesList["Enabled"] = StiPropertyType.Enabled;
                    propertiesList["Skip"] = StiPropertyType.Skip;                    

                    propertiesList_low = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                    foreach (DictionaryEntry de in propertiesList)
                    {
                        propertiesList_low[((string)de.Key).ToLowerInvariant()] = de.Value;
                    }
                }
                return syntaxCaseSensitive ? propertiesList : propertiesList_low;
            }
        }
        #endregion

        #region FunctionsList
        private static Hashtable functionsList = null;
        private static Hashtable functionsList_low = null;
        private static Hashtable functionsList_vb = null;
        private Hashtable FunctionsList
        {
            get
            {
                if (functionsList == null)
                {
                    functionsList = new Hashtable();

                    #region Aggregate functions - Report
                    functionsList["Count"] = StiFunctionType.Count;
                    functionsList["CountDistinct"] = StiFunctionType.CountDistinct;
                    functionsList["Avg"] = StiFunctionType.Avg;
                    functionsList["AvgD"] = StiFunctionType.AvgD;
                    functionsList["AvgDate"] = StiFunctionType.AvgDate;
                    functionsList["AvgI"] = StiFunctionType.AvgI;
                    functionsList["AvgTime"] = StiFunctionType.AvgTime;
                    functionsList["Max"] = StiFunctionType.Max;
                    functionsList["MaxD"] = StiFunctionType.MaxD;
                    functionsList["MaxDate"] = StiFunctionType.MaxDate;
                    functionsList["MaxI"] = StiFunctionType.MaxI;
                    functionsList["MaxStr"] = StiFunctionType.MaxStr;
                    functionsList["MaxTime"] = StiFunctionType.MaxTime;
                    functionsList["Median"] = StiFunctionType.Median;
                    functionsList["MedianD"] = StiFunctionType.MedianD;
                    functionsList["MedianI"] = StiFunctionType.MedianI;
                    functionsList["Min"] = StiFunctionType.Min;
                    functionsList["MinD"] = StiFunctionType.MinD;
                    functionsList["MinDate"] = StiFunctionType.MinDate;
                    functionsList["MinI"] = StiFunctionType.MinI;
                    functionsList["MinStr"] = StiFunctionType.MinStr;
                    functionsList["MinTime"] = StiFunctionType.MinTime;
                    functionsList["Mode"] = StiFunctionType.Mode;
                    functionsList["ModeD"] = StiFunctionType.ModeD;
                    functionsList["ModeI"] = StiFunctionType.ModeI;
                    functionsList["Sum"] = StiFunctionType.Sum;
                    functionsList["SumD"] = StiFunctionType.SumD;
                    functionsList["SumDistinct"] = StiFunctionType.SumDistinct;
                    functionsList["SumI"] = StiFunctionType.SumI;
                    functionsList["SumTime"] = StiFunctionType.SumTime;
                    functionsList["First"] = StiFunctionType.First;
                    functionsList["Last"] = StiFunctionType.Last;

                    functionsList["CountRunning"] = StiFunctionType.rCount;
                    functionsList["CountDistinctRunning"] = StiFunctionType.rCountDistinct;
                    functionsList["AvgRunning"] = StiFunctionType.rAvg;
                    functionsList["AvgDRunning"] = StiFunctionType.rAvgD;
                    functionsList["AvgDateRunning"] = StiFunctionType.rAvgDate;
                    functionsList["AvgIRunning"] = StiFunctionType.rAvgI;
                    functionsList["AvgTimeRunning"] = StiFunctionType.rAvgTime;
                    functionsList["MaxRunning"] = StiFunctionType.rMax;
                    functionsList["MaxDRunning"] = StiFunctionType.rMaxD;
                    functionsList["MaxDateRunning"] = StiFunctionType.rMaxDate;
                    functionsList["MaxIRunning"] = StiFunctionType.rMaxI;
                    functionsList["MaxStrRunning"] = StiFunctionType.rMaxStr;
                    functionsList["MaxTimeRunning"] = StiFunctionType.rMaxTime;
                    functionsList["MedianRunning"] = StiFunctionType.rMedian;
                    functionsList["MedianDRunning"] = StiFunctionType.rMedianD;
                    functionsList["MedianIRunning"] = StiFunctionType.rMedianI;
                    functionsList["MinRunning"] = StiFunctionType.rMin;
                    functionsList["MinDRunning"] = StiFunctionType.rMinD;
                    functionsList["MinDateRunning"] = StiFunctionType.rMinDate;
                    functionsList["MinIRunning"] = StiFunctionType.rMinI;
                    functionsList["MinStrRunning"] = StiFunctionType.rMinStr;
                    functionsList["MinTimeRunning"] = StiFunctionType.rMinTime;
                    functionsList["ModeRunning"] = StiFunctionType.rMode;
                    functionsList["ModeDRunning"] = StiFunctionType.rModeD;
                    functionsList["ModeIRunning"] = StiFunctionType.rModeI;
                    functionsList["SumRunning"] = StiFunctionType.rSum;
                    functionsList["SumDRunning"] = StiFunctionType.rSumD;
                    functionsList["SumDistinctRunning"] = StiFunctionType.rSumDistinct;
                    functionsList["SumIRunning"] = StiFunctionType.rSumI;
                    functionsList["SumTimeRunning"] = StiFunctionType.rSumTime;
                    functionsList["FirstRunning"] = StiFunctionType.rFirst;
                    functionsList["LastRunning"] = StiFunctionType.rLast;

                    functionsList["CountIf"] = StiFunctionType.iCount;
                    functionsList["CountDistinctIf"] = StiFunctionType.iCountDistinct;
                    functionsList["AvgIf"] = StiFunctionType.iAvg;
                    functionsList["AvgDIf"] = StiFunctionType.iAvgD;
                    functionsList["AvgDateIf"] = StiFunctionType.iAvgDate;
                    functionsList["AvgIIf"] = StiFunctionType.iAvgI;
                    functionsList["AvgTimeIf"] = StiFunctionType.iAvgTime;
                    functionsList["MaxIf"] = StiFunctionType.iMax;
                    functionsList["MaxDIf"] = StiFunctionType.iMaxD;
                    functionsList["MaxDateIf"] = StiFunctionType.iMaxDate;
                    functionsList["MaxIIf"] = StiFunctionType.iMaxI;
                    functionsList["MaxStrIf"] = StiFunctionType.iMaxStr;
                    functionsList["MaxTimeIf"] = StiFunctionType.iMaxTime;
                    functionsList["MedianIf"] = StiFunctionType.iMedian;
                    functionsList["MedianDIf"] = StiFunctionType.iMedianD;
                    functionsList["MedianIIf"] = StiFunctionType.iMedianI;
                    functionsList["MinIf"] = StiFunctionType.iMin;
                    functionsList["MinDIf"] = StiFunctionType.iMinD;
                    functionsList["MinDateIf"] = StiFunctionType.iMinDate;
                    functionsList["MinIIf"] = StiFunctionType.iMinI;
                    functionsList["MinStrIf"] = StiFunctionType.iMinStr;
                    functionsList["MinTimeIf"] = StiFunctionType.iMinTime;
                    functionsList["ModeIf"] = StiFunctionType.iMode;
                    functionsList["ModeDIf"] = StiFunctionType.iModeD;
                    functionsList["ModeIIf"] = StiFunctionType.iModeI;
                    functionsList["SumIf"] = StiFunctionType.iSum;
                    functionsList["SumDIf"] = StiFunctionType.iSumD;
                    functionsList["SumDistinctIf"] = StiFunctionType.iSumDistinct;
                    functionsList["SumIIf"] = StiFunctionType.iSumI;
                    functionsList["SumTimeIf"] = StiFunctionType.iSumTime;
                    functionsList["FirstIf"] = StiFunctionType.iFirst;
                    functionsList["LastIf"] = StiFunctionType.iLast;

                    functionsList["CountIfRunning"] = StiFunctionType.riCount;
                    functionsList["CountDistinctIfRunning"] = StiFunctionType.riCountDistinct;
                    functionsList["AvgIfRunning"] = StiFunctionType.riAvg;
                    functionsList["AvgDIfRunning"] = StiFunctionType.riAvgD;
                    functionsList["AvgDateIfRunning"] = StiFunctionType.riAvgDate;
                    functionsList["AvgIIfRunning"] = StiFunctionType.riAvgI;
                    functionsList["AvgTimeIfRunning"] = StiFunctionType.riAvgTime;
                    functionsList["MaxIfRunning"] = StiFunctionType.riMax;
                    functionsList["MaxDIfRunning"] = StiFunctionType.riMaxD;
                    functionsList["MaxDateIfRunning"] = StiFunctionType.riMaxDate;
                    functionsList["MaxIIfRunning"] = StiFunctionType.riMaxI;
                    functionsList["MaxStrIfRunning"] = StiFunctionType.riMaxStr;
                    functionsList["MaxTimeIfRunning"] = StiFunctionType.riMaxTime;
                    functionsList["MedianIfRunning"] = StiFunctionType.riMedian;
                    functionsList["MedianDIfRunning"] = StiFunctionType.riMedianD;
                    functionsList["MedianIIfRunning"] = StiFunctionType.riMedianI;
                    functionsList["MinIfRunning"] = StiFunctionType.riMin;
                    functionsList["MinDIfRunning"] = StiFunctionType.riMinD;
                    functionsList["MinDateIfRunning"] = StiFunctionType.riMinDate;
                    functionsList["MinIIfRunning"] = StiFunctionType.riMinI;
                    functionsList["MinStrIfRunning"] = StiFunctionType.riMinStr;
                    functionsList["MinTimeIfRunning"] = StiFunctionType.riMinTime;
                    functionsList["ModeIfRunning"] = StiFunctionType.riMode;
                    functionsList["ModeDIfRunning"] = StiFunctionType.riModeD;
                    functionsList["ModeIIfRunning"] = StiFunctionType.riModeI;
                    functionsList["SumIfRunning"] = StiFunctionType.riSum;
                    functionsList["SumDIfRunning"] = StiFunctionType.riSumD;
                    functionsList["SumDistinctIfRunning"] = StiFunctionType.riSumDistinct;
                    functionsList["SumIIfRunning"] = StiFunctionType.riSumI;
                    functionsList["SumTimeIfRunning"] = StiFunctionType.riSumTime;
                    functionsList["FirstIfRunning"] = StiFunctionType.riFirst;
                    functionsList["LastIfRunning"] = StiFunctionType.riLast;
                    #endregion

                    #region Aggregate functions - Column
                    functionsList["colCount"] = StiFunctionType.cCount;
                    functionsList["colCountDistinct"] = StiFunctionType.cCountDistinct;
                    functionsList["colAvg"] = StiFunctionType.cAvg;
                    functionsList["colAvgD"] = StiFunctionType.cAvgD;
                    functionsList["colAvgDate"] = StiFunctionType.cAvgDate;
                    functionsList["colAvgI"] = StiFunctionType.cAvgI;
                    functionsList["colAvgTime"] = StiFunctionType.cAvgTime;
                    functionsList["colMax"] = StiFunctionType.cMax;
                    functionsList["colMaxD"] = StiFunctionType.cMaxD;
                    functionsList["colMaxDate"] = StiFunctionType.cMaxDate;
                    functionsList["colMaxI"] = StiFunctionType.cMaxI;
                    functionsList["colMaxStr"] = StiFunctionType.cMaxStr;
                    functionsList["colMaxTime"] = StiFunctionType.cMaxTime;
                    functionsList["colMedian"] = StiFunctionType.cMedian;
                    functionsList["colMedianD"] = StiFunctionType.cMedianD;
                    functionsList["colMedianI"] = StiFunctionType.cMedianI;
                    functionsList["colMin"] = StiFunctionType.cMin;
                    functionsList["colMinD"] = StiFunctionType.cMinD;
                    functionsList["colMinDate"] = StiFunctionType.cMinDate;
                    functionsList["colMinI"] = StiFunctionType.cMinI;
                    functionsList["colMinStr"] = StiFunctionType.cMinStr;
                    functionsList["colMinTime"] = StiFunctionType.cMinTime;
                    functionsList["colMode"] = StiFunctionType.cMode;
                    functionsList["colModeD"] = StiFunctionType.cModeD;
                    functionsList["colModeI"] = StiFunctionType.cModeI;
                    functionsList["colSum"] = StiFunctionType.cSum;
                    functionsList["colSumD"] = StiFunctionType.cSumD;
                    functionsList["colSumDistinct"] = StiFunctionType.cSumDistinct;
                    functionsList["colSumI"] = StiFunctionType.cSumI;
                    functionsList["colSumTime"] = StiFunctionType.cSumTime;
                    functionsList["colFirst"] = StiFunctionType.cFirst;
                    functionsList["colLast"] = StiFunctionType.cLast;

                    functionsList["colCountRunning"] = StiFunctionType.crCount;
                    functionsList["colCountDistinctRunning"] = StiFunctionType.crCountDistinct;
                    functionsList["colAvgRunning"] = StiFunctionType.crAvg;
                    functionsList["colAvgDRunning"] = StiFunctionType.crAvgD;
                    functionsList["colAvgDateRunning"] = StiFunctionType.crAvgDate;
                    functionsList["colAvgIRunning"] = StiFunctionType.crAvgI;
                    functionsList["colAvgTimeRunning"] = StiFunctionType.crAvgTime;
                    functionsList["colMaxRunning"] = StiFunctionType.crMax;
                    functionsList["colMaxDRunning"] = StiFunctionType.crMaxD;
                    functionsList["colMaxDateRunning"] = StiFunctionType.crMaxDate;
                    functionsList["colMaxIRunning"] = StiFunctionType.crMaxI;
                    functionsList["colMaxStrRunning"] = StiFunctionType.crMaxStr;
                    functionsList["colMaxTimeRunning"] = StiFunctionType.crMaxTime;
                    functionsList["colMedianRunning"] = StiFunctionType.crMedian;
                    functionsList["colMedianDRunning"] = StiFunctionType.crMedianD;
                    functionsList["colMedianIRunning"] = StiFunctionType.crMedianI;
                    functionsList["colMinRunning"] = StiFunctionType.crMin;
                    functionsList["colMinDRunning"] = StiFunctionType.crMinD;
                    functionsList["colMinDateRunning"] = StiFunctionType.crMinDate;
                    functionsList["colMinIRunning"] = StiFunctionType.crMinI;
                    functionsList["colMinStrRunning"] = StiFunctionType.crMinStr;
                    functionsList["colMinTimeRunning"] = StiFunctionType.crMinTime;
                    functionsList["colModeRunning"] = StiFunctionType.crMode;
                    functionsList["colModeDRunning"] = StiFunctionType.crModeD;
                    functionsList["colModeIRunning"] = StiFunctionType.crModeI;
                    functionsList["colSumRunning"] = StiFunctionType.crSum;
                    functionsList["colSumDRunning"] = StiFunctionType.crSumD;
                    functionsList["colSumDistinctRunning"] = StiFunctionType.crSumDistinct;
                    functionsList["colSumIRunning"] = StiFunctionType.crSumI;
                    functionsList["colSumTimeRunning"] = StiFunctionType.crSumTime;
                    functionsList["colFirstRunning"] = StiFunctionType.crFirst;
                    functionsList["colLastRunning"] = StiFunctionType.crLast;

                    functionsList["colCountIf"] = StiFunctionType.ciCount;
                    functionsList["colCountDistinctIf"] = StiFunctionType.ciCountDistinct;
                    functionsList["colAvgIf"] = StiFunctionType.ciAvg;
                    functionsList["colAvgDIf"] = StiFunctionType.ciAvgD;
                    functionsList["colAvgDateIf"] = StiFunctionType.ciAvgDate;
                    functionsList["colAvgIIf"] = StiFunctionType.ciAvgI;
                    functionsList["colAvgTimeIf"] = StiFunctionType.ciAvgTime;
                    functionsList["colMaxIf"] = StiFunctionType.ciMax;
                    functionsList["colMaxDIf"] = StiFunctionType.ciMaxD;
                    functionsList["colMaxDateIf"] = StiFunctionType.ciMaxDate;
                    functionsList["colMaxIIf"] = StiFunctionType.ciMaxI;
                    functionsList["colMaxStrIf"] = StiFunctionType.ciMaxStr;
                    functionsList["colMaxTimeIf"] = StiFunctionType.ciMaxTime;
                    functionsList["colMedianIf"] = StiFunctionType.ciMedian;
                    functionsList["colMedianDIf"] = StiFunctionType.ciMedianD;
                    functionsList["colMedianIIf"] = StiFunctionType.ciMedianI;
                    functionsList["colMinIf"] = StiFunctionType.ciMin;
                    functionsList["colMinDIf"] = StiFunctionType.ciMinD;
                    functionsList["colMinDateIf"] = StiFunctionType.ciMinDate;
                    functionsList["colMinIIf"] = StiFunctionType.ciMinI;
                    functionsList["colMinStrIf"] = StiFunctionType.ciMinStr;
                    functionsList["colMinTimeIf"] = StiFunctionType.ciMinTime;
                    functionsList["colModeIf"] = StiFunctionType.ciMode;
                    functionsList["colModeDIf"] = StiFunctionType.ciModeD;
                    functionsList["colModeIIf"] = StiFunctionType.ciModeI;
                    functionsList["colSumIf"] = StiFunctionType.ciSum;
                    functionsList["colSumDIf"] = StiFunctionType.ciSumD;
                    functionsList["colSumDistinctIf"] = StiFunctionType.ciSumDistinct;
                    functionsList["colSumIIf"] = StiFunctionType.ciSumI;
                    functionsList["colSumTimeIf"] = StiFunctionType.ciSumTime;
                    functionsList["colFirstIf"] = StiFunctionType.ciFirst;
                    functionsList["colLastIf"] = StiFunctionType.ciLast;

                    functionsList["colCountIfRunning"] = StiFunctionType.criCount;
                    functionsList["colCountDistinctIfRunning"] = StiFunctionType.criCountDistinct;
                    functionsList["colAvgIfRunning"] = StiFunctionType.criAvg;
                    functionsList["colAvgDIfRunning"] = StiFunctionType.criAvgD;
                    functionsList["colAvgDateIfRunning"] = StiFunctionType.criAvgDate;
                    functionsList["colAvgIIfRunning"] = StiFunctionType.criAvgI;
                    functionsList["colAvgTimeIfRunning"] = StiFunctionType.criAvgTime;
                    functionsList["colMaxIfRunning"] = StiFunctionType.criMax;
                    functionsList["colMaxDIfRunning"] = StiFunctionType.criMaxD;
                    functionsList["colMaxDateIfRunning"] = StiFunctionType.criMaxDate;
                    functionsList["colMaxIIfRunning"] = StiFunctionType.criMaxI;
                    functionsList["colMaxStrIfRunning"] = StiFunctionType.criMaxStr;
                    functionsList["colMaxTimeIfRunning"] = StiFunctionType.criMaxTime;
                    functionsList["colMedianIfRunning"] = StiFunctionType.criMedian;
                    functionsList["colMedianDIfRunning"] = StiFunctionType.criMedianD;
                    functionsList["colMedianIIfRunning"] = StiFunctionType.criMedianI;
                    functionsList["colMinIfRunning"] = StiFunctionType.criMin;
                    functionsList["colMinDIfRunning"] = StiFunctionType.criMinD;
                    functionsList["colMinDateIfRunning"] = StiFunctionType.criMinDate;
                    functionsList["colMinIIfRunning"] = StiFunctionType.criMinI;
                    functionsList["colMinStrIfRunning"] = StiFunctionType.criMinStr;
                    functionsList["colMinTimeIfRunning"] = StiFunctionType.criMinTime;
                    functionsList["colModeIfRunning"] = StiFunctionType.criMode;
                    functionsList["colModeDIfRunning"] = StiFunctionType.criModeD;
                    functionsList["colModeIIfRunning"] = StiFunctionType.criModeI;
                    functionsList["colSumIfRunning"] = StiFunctionType.criSum;
                    functionsList["colSumDIfRunning"] = StiFunctionType.criSumD;
                    functionsList["colSumDistinctIfRunning"] = StiFunctionType.criSumDistinct;
                    functionsList["colSumIIfRunning"] = StiFunctionType.criSumI;
                    functionsList["colSumTimeIfRunning"] = StiFunctionType.criSumTime;
                    functionsList["colFirstIfRunning"] = StiFunctionType.criFirst;
                    functionsList["colLastIfRunning"] = StiFunctionType.criLast;
                    #endregion

                    #region Aggregate functions - Page
                    functionsList["cCount"] = StiFunctionType.pCount;
                    functionsList["cCountDistinct"] = StiFunctionType.pCountDistinct;
                    functionsList["cAvg"] = StiFunctionType.pAvg;
                    functionsList["cAvgD"] = StiFunctionType.pAvgD;
                    functionsList["cAvgDate"] = StiFunctionType.pAvgDate;
                    functionsList["cAvgI"] = StiFunctionType.pAvgI;
                    functionsList["cAvgTime"] = StiFunctionType.pAvgTime;
                    functionsList["cMax"] = StiFunctionType.pMax;
                    functionsList["cMaxD"] = StiFunctionType.pMaxD;
                    functionsList["cMaxDate"] = StiFunctionType.pMaxDate;
                    functionsList["cMaxI"] = StiFunctionType.pMaxI;
                    functionsList["cMaxStr"] = StiFunctionType.pMaxStr;
                    functionsList["cMaxTime"] = StiFunctionType.pMaxTime;
                    functionsList["cMedian"] = StiFunctionType.pMedian;
                    functionsList["cMedianD"] = StiFunctionType.pMedianD;
                    functionsList["cMedianI"] = StiFunctionType.pMedianI;
                    functionsList["cMin"] = StiFunctionType.pMin;
                    functionsList["cMinD"] = StiFunctionType.pMinD;
                    functionsList["cMinDate"] = StiFunctionType.pMinDate;
                    functionsList["cMinI"] = StiFunctionType.pMinI;
                    functionsList["cMinStr"] = StiFunctionType.pMinStr;
                    functionsList["cMinTime"] = StiFunctionType.pMinTime;
                    functionsList["cMode"] = StiFunctionType.pMode;
                    functionsList["cModeD"] = StiFunctionType.pModeD;
                    functionsList["cModeI"] = StiFunctionType.pModeI;
                    functionsList["cSum"] = StiFunctionType.pSum;
                    functionsList["cSumD"] = StiFunctionType.pSumD;
                    functionsList["cSumDistinct"] = StiFunctionType.pSumDistinct;
                    functionsList["cSumI"] = StiFunctionType.pSumI;
                    functionsList["cSumTime"] = StiFunctionType.pSumTime;
                    functionsList["cFirst"] = StiFunctionType.pFirst;
                    functionsList["cLast"] = StiFunctionType.pLast;

                    functionsList["cCountRunning"] = StiFunctionType.prCount;
                    functionsList["cCountDistinctRunning"] = StiFunctionType.prCountDistinct;
                    functionsList["cAvgRunning"] = StiFunctionType.prAvg;
                    functionsList["cAvgDRunning"] = StiFunctionType.prAvgD;
                    functionsList["cAvgDateRunning"] = StiFunctionType.prAvgDate;
                    functionsList["cAvgIRunning"] = StiFunctionType.prAvgI;
                    functionsList["cAvgTimeRunning"] = StiFunctionType.prAvgTime;
                    functionsList["cMaxRunning"] = StiFunctionType.prMax;
                    functionsList["cMaxDRunning"] = StiFunctionType.prMaxD;
                    functionsList["cMaxDateRunning"] = StiFunctionType.prMaxDate;
                    functionsList["cMaxIRunning"] = StiFunctionType.prMaxI;
                    functionsList["cMaxStrRunning"] = StiFunctionType.prMaxStr;
                    functionsList["cMaxTimeRunning"] = StiFunctionType.prMaxTime;
                    functionsList["cMedianRunning"] = StiFunctionType.prMedian;
                    functionsList["cMedianDRunning"] = StiFunctionType.prMedianD;
                    functionsList["cMedianIRunning"] = StiFunctionType.prMedianI;
                    functionsList["cMinRunning"] = StiFunctionType.prMin;
                    functionsList["cMinDRunning"] = StiFunctionType.prMinD;
                    functionsList["cMinDateRunning"] = StiFunctionType.prMinDate;
                    functionsList["cMinIRunning"] = StiFunctionType.prMinI;
                    functionsList["cMinStrRunning"] = StiFunctionType.prMinStr;
                    functionsList["cMinTimeRunning"] = StiFunctionType.prMinTime;
                    functionsList["cModeRunning"] = StiFunctionType.prMode;
                    functionsList["cModeDRunning"] = StiFunctionType.prModeD;
                    functionsList["cModeIRunning"] = StiFunctionType.prModeI;
                    functionsList["cSumRunning"] = StiFunctionType.prSum;
                    functionsList["cSumDRunning"] = StiFunctionType.prSumD;
                    functionsList["cSumDistinctRunning"] = StiFunctionType.prSumDistinct;
                    functionsList["cSumIRunning"] = StiFunctionType.prSumI;
                    functionsList["cSumTimeRunning"] = StiFunctionType.prSumTime;
                    functionsList["cFirstRunning"] = StiFunctionType.prFirst;
                    functionsList["cLastRunning"] = StiFunctionType.prLast;

                    functionsList["cCountIf"] = StiFunctionType.piCount;
                    functionsList["cCountDistinctIf"] = StiFunctionType.piCountDistinct;
                    functionsList["cAvgIf"] = StiFunctionType.piAvg;
                    functionsList["cAvgDIf"] = StiFunctionType.piAvgD;
                    functionsList["cAvgDateIf"] = StiFunctionType.piAvgDate;
                    functionsList["cAvgIIf"] = StiFunctionType.piAvgI;
                    functionsList["cAvgTimeIf"] = StiFunctionType.piAvgTime;
                    functionsList["cMaxIf"] = StiFunctionType.piMax;
                    functionsList["cMaxDIf"] = StiFunctionType.piMaxD;
                    functionsList["cMaxDateIf"] = StiFunctionType.piMaxDate;
                    functionsList["cMaxIIf"] = StiFunctionType.piMaxI;
                    functionsList["cMaxStrIf"] = StiFunctionType.piMaxStr;
                    functionsList["cMaxTimeIf"] = StiFunctionType.piMaxTime;
                    functionsList["cMedianIf"] = StiFunctionType.piMedian;
                    functionsList["cMedianDIf"] = StiFunctionType.piMedianD;
                    functionsList["cMedianIIf"] = StiFunctionType.piMedianI;
                    functionsList["cMinIf"] = StiFunctionType.piMin;
                    functionsList["cMinDIf"] = StiFunctionType.piMinD;
                    functionsList["cMinDateIf"] = StiFunctionType.piMinDate;
                    functionsList["cMinIIf"] = StiFunctionType.piMinI;
                    functionsList["cMinStrIf"] = StiFunctionType.piMinStr;
                    functionsList["cMinTimeIf"] = StiFunctionType.piMinTime;
                    functionsList["cModeIf"] = StiFunctionType.piMode;
                    functionsList["cModeDIf"] = StiFunctionType.piModeD;
                    functionsList["cModeIIf"] = StiFunctionType.piModeI;
                    functionsList["cSumIf"] = StiFunctionType.piSum;
                    functionsList["cSumDIf"] = StiFunctionType.piSumD;
                    functionsList["cSumDistinctIf"] = StiFunctionType.piSumDistinct;
                    functionsList["cSumIIf"] = StiFunctionType.piSumI;
                    functionsList["cSumTimeIf"] = StiFunctionType.piSumTime;
                    functionsList["cFirstIf"] = StiFunctionType.piFirst;
                    functionsList["cLastIf"] = StiFunctionType.piLast;

                    functionsList["cCountIfRunning"] = StiFunctionType.priCount;
                    functionsList["cCountDistinctIfRunning"] = StiFunctionType.priCountDistinct;
                    functionsList["cAvgIfRunning"] = StiFunctionType.priAvg;
                    functionsList["cAvgDIfRunning"] = StiFunctionType.priAvgD;
                    functionsList["cAvgDateIfRunning"] = StiFunctionType.priAvgDate;
                    functionsList["cAvgIIfRunning"] = StiFunctionType.priAvgI;
                    functionsList["cAvgTimeIfRunning"] = StiFunctionType.priAvgTime;
                    functionsList["cMaxIfRunning"] = StiFunctionType.priMax;
                    functionsList["cMaxDIfRunning"] = StiFunctionType.priMaxD;
                    functionsList["cMaxDateIfRunning"] = StiFunctionType.priMaxDate;
                    functionsList["cMaxIIfRunning"] = StiFunctionType.priMaxI;
                    functionsList["cMaxStrIfRunning"] = StiFunctionType.priMaxStr;
                    functionsList["cMaxTimeIfRunning"] = StiFunctionType.priMaxTime;
                    functionsList["cMedianIfRunning"] = StiFunctionType.priMedian;
                    functionsList["cMedianDIfRunning"] = StiFunctionType.priMedianD;
                    functionsList["cMedianIIfRunning"] = StiFunctionType.priMedianI;
                    functionsList["cMinIfRunning"] = StiFunctionType.priMin;
                    functionsList["cMinDIfRunning"] = StiFunctionType.priMinD;
                    functionsList["cMinDateIfRunning"] = StiFunctionType.priMinDate;
                    functionsList["cMinIIfRunning"] = StiFunctionType.priMinI;
                    functionsList["cMinStrIfRunning"] = StiFunctionType.priMinStr;
                    functionsList["cMinTimeIfRunning"] = StiFunctionType.priMinTime;
                    functionsList["cModeIfRunning"] = StiFunctionType.priMode;
                    functionsList["cModeDIfRunning"] = StiFunctionType.priModeD;
                    functionsList["cModeIIfRunning"] = StiFunctionType.priModeI;
                    functionsList["cSumIfRunning"] = StiFunctionType.priSum;
                    functionsList["cSumDIfRunning"] = StiFunctionType.priSumD;
                    functionsList["cSumDistinctIfRunning"] = StiFunctionType.priSumDistinct;
                    functionsList["cSumIIfRunning"] = StiFunctionType.priSumI;
                    functionsList["cSumTimeIfRunning"] = StiFunctionType.priSumTime;
                    functionsList["cFirstIfRunning"] = StiFunctionType.priFirst;
                    functionsList["cLastIfRunning"] = StiFunctionType.priLast;
                    #endregion

                    #region Totals
                    //functionsList["Totals"] = StiFunctionType.NameSpace;

                    functionsList["Totals.Count"] = StiFunctionType.Count;
                    functionsList["Totals.CountDistinct"] = StiFunctionType.CountDistinct;
                    functionsList["Totals.Avg"] = StiFunctionType.Avg;
                    functionsList["Totals.AvgD"] = StiFunctionType.AvgD;
                    functionsList["Totals.AvgDate"] = StiFunctionType.AvgDate;
                    functionsList["Totals.AvgI"] = StiFunctionType.AvgI;
                    functionsList["Totals.AvgTime"] = StiFunctionType.AvgTime;
                    functionsList["Totals.Max"] = StiFunctionType.Max;
                    functionsList["Totals.MaxD"] = StiFunctionType.MaxD;
                    functionsList["Totals.MaxDate"] = StiFunctionType.MaxDate;
                    functionsList["Totals.MaxI"] = StiFunctionType.MaxI;
                    functionsList["Totals.MaxStr"] = StiFunctionType.MaxStr;
                    functionsList["Totals.MaxTime"] = StiFunctionType.MaxTime;
                    functionsList["Totals.Median"] = StiFunctionType.Median;
                    functionsList["Totals.MedianD"] = StiFunctionType.MedianD;
                    functionsList["Totals.MedianI"] = StiFunctionType.MedianI;
                    functionsList["Totals.Min"] = StiFunctionType.Min;
                    functionsList["Totals.MinD"] = StiFunctionType.MinD;
                    functionsList["Totals.MinDate"] = StiFunctionType.MinDate;
                    functionsList["Totals.MinI"] = StiFunctionType.MinI;
                    functionsList["Totals.MinStr"] = StiFunctionType.MinStr;
                    functionsList["Totals.MinTime"] = StiFunctionType.MinTime;
                    functionsList["Totals.Mode"] = StiFunctionType.Mode;
                    functionsList["Totals.ModeD"] = StiFunctionType.ModeD;
                    functionsList["Totals.ModeI"] = StiFunctionType.ModeI;
                    functionsList["Totals.Sum"] = StiFunctionType.Sum;
                    functionsList["Totals.SumD"] = StiFunctionType.SumD;
                    functionsList["Totals.SumDistinct"] = StiFunctionType.SumDistinct;
                    functionsList["Totals.SumI"] = StiFunctionType.SumI;
                    functionsList["Totals.SumTime"] = StiFunctionType.SumTime;
                    functionsList["Totals.First"] = StiFunctionType.First;
                    functionsList["Totals.Last"] = StiFunctionType.Last;

                    functionsList["Totals.cCount"] = StiFunctionType.pCount;
                    functionsList["Totals.cCountDistinct"] = StiFunctionType.pCountDistinct;
                    functionsList["Totals.cAvg"] = StiFunctionType.pAvg;
                    functionsList["Totals.cAvgD"] = StiFunctionType.pAvgD;
                    functionsList["Totals.cAvgDate"] = StiFunctionType.pAvgDate;
                    functionsList["Totals.cAvgI"] = StiFunctionType.pAvgI;
                    functionsList["Totals.cAvgTime"] = StiFunctionType.pAvgTime;
                    functionsList["Totals.cMax"] = StiFunctionType.pMax;
                    functionsList["Totals.cMaxD"] = StiFunctionType.pMaxD;
                    functionsList["Totals.cMaxDate"] = StiFunctionType.pMaxDate;
                    functionsList["Totals.cMaxI"] = StiFunctionType.pMaxI;
                    functionsList["Totals.cMaxStr"] = StiFunctionType.pMaxStr;
                    functionsList["Totals.cMaxTime"] = StiFunctionType.pMaxTime;
                    functionsList["Totals.cMedian"] = StiFunctionType.pMedian;
                    functionsList["Totals.cMedianD"] = StiFunctionType.pMedianD;
                    functionsList["Totals.cMedianI"] = StiFunctionType.pMedianI;
                    functionsList["Totals.cMin"] = StiFunctionType.pMin;
                    functionsList["Totals.cMinD"] = StiFunctionType.pMinD;
                    functionsList["Totals.cMinDate"] = StiFunctionType.pMinDate;
                    functionsList["Totals.cMinI"] = StiFunctionType.pMinI;
                    functionsList["Totals.cMinStr"] = StiFunctionType.pMinStr;
                    functionsList["Totals.cMinTime"] = StiFunctionType.pMinTime;
                    functionsList["Totals.cMode"] = StiFunctionType.pMode;
                    functionsList["Totals.cModeD"] = StiFunctionType.pModeD;
                    functionsList["Totals.cModeI"] = StiFunctionType.pModeI;
                    functionsList["Totals.cSum"] = StiFunctionType.pSum;
                    functionsList["Totals.cSumD"] = StiFunctionType.pSumD;
                    functionsList["Totals.cSumDistinct"] = StiFunctionType.pSumDistinct;
                    functionsList["Totals.cSumI"] = StiFunctionType.pSumI;
                    functionsList["Totals.cSumTime"] = StiFunctionType.pSumTime;
                    functionsList["Totals.cFirst"] = StiFunctionType.pFirst;
                    functionsList["Totals.cLast"] = StiFunctionType.pLast;

                    functionsList["Totals.cCountRunning"] = StiFunctionType.prCount;
                    functionsList["Totals.cCountDistinctRunning"] = StiFunctionType.prCountDistinct;
                    functionsList["Totals.cAvgRunning"] = StiFunctionType.prAvg;
                    functionsList["Totals.cAvgDRunning"] = StiFunctionType.prAvgD;
                    functionsList["Totals.cAvgDateRunning"] = StiFunctionType.prAvgDate;
                    functionsList["Totals.cAvgIRunning"] = StiFunctionType.prAvgI;
                    functionsList["Totals.cAvgTimeRunning"] = StiFunctionType.prAvgTime;
                    functionsList["Totals.cMaxRunning"] = StiFunctionType.prMax;
                    functionsList["Totals.cMaxDRunning"] = StiFunctionType.prMaxD;
                    functionsList["Totals.cMaxDateRunning"] = StiFunctionType.prMaxDate;
                    functionsList["Totals.cMaxIRunning"] = StiFunctionType.prMaxI;
                    functionsList["Totals.cMaxStrRunning"] = StiFunctionType.prMaxStr;
                    functionsList["Totals.cMaxTimeRunning"] = StiFunctionType.prMaxTime;
                    functionsList["Totals.cMedianRunning"] = StiFunctionType.prMedian;
                    functionsList["Totals.cMedianDRunning"] = StiFunctionType.prMedianD;
                    functionsList["Totals.cMedianIRunning"] = StiFunctionType.prMedianI;
                    functionsList["Totals.cMinRunning"] = StiFunctionType.prMin;
                    functionsList["Totals.cMinDRunning"] = StiFunctionType.prMinD;
                    functionsList["Totals.cMinDateRunning"] = StiFunctionType.prMinDate;
                    functionsList["Totals.cMinIRunning"] = StiFunctionType.prMinI;
                    functionsList["Totals.cMinStrRunning"] = StiFunctionType.prMinStr;
                    functionsList["Totals.cMinTimeRunning"] = StiFunctionType.prMinTime;
                    functionsList["Totals.cModeRunning"] = StiFunctionType.prMode;
                    functionsList["Totals.cModeDRunning"] = StiFunctionType.prModeD;
                    functionsList["Totals.cModeIRunning"] = StiFunctionType.prModeI;
                    functionsList["Totals.cSumRunning"] = StiFunctionType.prSum;
                    functionsList["Totals.cSumDRunning"] = StiFunctionType.prSumD;
                    functionsList["Totals.cSumDistinctRunning"] = StiFunctionType.prSumDistinct;
                    functionsList["Totals.cSumIRunning"] = StiFunctionType.prSumI;
                    functionsList["Totals.cSumTimeRunning"] = StiFunctionType.prSumTime;
                    functionsList["Totals.cFirstRunning"] = StiFunctionType.prFirst;
                    functionsList["Totals.cLastRunning"] = StiFunctionType.prLast;

                    functionsList["Totals.Rank"] = StiFunctionType.Rank;
                    #endregion

                    #region Totals Hierarchical
                    functionsList["Totals.CountAllLevels"] = StiFunctionType.CountAllLevels;
                    functionsList["Totals.CountAllLevelsOnlyChilds"] = StiFunctionType.CountAllLevelsOnlyChilds;
                    functionsList["Totals.CountOnlyChilds"] = StiFunctionType.CountOnlyChilds;
                    functionsList["Totals.CountDistinctAllLevels"] = StiFunctionType.CountDistinctAllLevels;
                    functionsList["Totals.CountDistinctAllLevelsOnlyChilds"] = StiFunctionType.CountDistinctAllLevelsOnlyChilds;
                    functionsList["Totals.CountDistinctOnlyChilds"] = StiFunctionType.CountDistinctOnlyChilds;

                    functionsList["Totals.SumAllLevels"] = StiFunctionType.SumAllLevels;
                    functionsList["Totals.SumAllLevelsOnlyChilds"] = StiFunctionType.SumAllLevelsOnlyChilds;
                    functionsList["Totals.SumOnlyChilds"] = StiFunctionType.SumOnlyChilds;
                    functionsList["Totals.SumDAllLevels"] = StiFunctionType.SumDAllLevels;
                    functionsList["Totals.SumDAllLevelsOnlyChilds"] = StiFunctionType.SumDAllLevelsOnlyChilds;
                    functionsList["Totals.SumDOnlyChilds"] = StiFunctionType.SumDOnlyChilds;
                    functionsList["Totals.SumIAllLevels"] = StiFunctionType.SumIAllLevels;
                    functionsList["Totals.SumIAllLevelsOnlyChilds"] = StiFunctionType.SumIAllLevelsOnlyChilds;
                    functionsList["Totals.SumIOnlyChilds"] = StiFunctionType.SumIOnlyChilds;
                    functionsList["Totals.SumTimeAllLevels"] = StiFunctionType.SumTimeAllLevels;
                    functionsList["Totals.SumTimeAllLevelsOnlyChilds"] = StiFunctionType.SumTimeAllLevelsOnlyChilds;
                    functionsList["Totals.SumTimeOnlyChilds"] = StiFunctionType.SumTimeOnlyChilds;

                    functionsList["Totals.AvgAllLevels"] = StiFunctionType.AvgAllLevels;
                    functionsList["Totals.AvgAllLevelsOnlyChilds"] = StiFunctionType.AvgAllLevelsOnlyChilds;
                    functionsList["Totals.AvgOnlyChilds"] = StiFunctionType.AvgOnlyChilds;
                    functionsList["Totals.AvgDAllLevels"] = StiFunctionType.AvgDAllLevels;
                    functionsList["Totals.AvgDAllLevelsOnlyChilds"] = StiFunctionType.AvgDAllLevelsOnlyChilds;
                    functionsList["Totals.AvgDOnlyChilds"] = StiFunctionType.AvgDOnlyChilds;
                    functionsList["Totals.AvgIAllLevels"] = StiFunctionType.AvgIAllLevels;
                    functionsList["Totals.AvgIAllLevelsOnlyChilds"] = StiFunctionType.AvgIAllLevelsOnlyChilds;
                    functionsList["Totals.AvgIOnlyChilds"] = StiFunctionType.AvgIOnlyChilds;
                    functionsList["Totals.AvgDateAllLevels"] = StiFunctionType.AvgDateAllLevels;
                    functionsList["Totals.AvgDateAllLevelsOnlyChilds"] = StiFunctionType.AvgDateAllLevelsOnlyChilds;
                    functionsList["Totals.AvgDateOnlyChilds"] = StiFunctionType.AvgDateOnlyChilds;
                    functionsList["Totals.AvgTimeAllLevels"] = StiFunctionType.AvgTimeAllLevels;
                    functionsList["Totals.AvgTimeAllLevelsOnlyChilds"] = StiFunctionType.AvgTimeAllLevelsOnlyChilds;
                    functionsList["Totals.AvgTimeOnlyChilds"] = StiFunctionType.AvgTimeOnlyChilds;

                    functionsList["Totals.MaxAllLevels"] = StiFunctionType.MaxAllLevels;
                    functionsList["Totals.MaxAllLevelsOnlyChilds"] = StiFunctionType.MaxAllLevelsOnlyChilds;
                    functionsList["Totals.MaxOnlyChilds"] = StiFunctionType.MaxOnlyChilds;
                    functionsList["Totals.MaxDAllLevels"] = StiFunctionType.MaxDAllLevels;
                    functionsList["Totals.MaxDAllLevelsOnlyChilds"] = StiFunctionType.MaxDAllLevelsOnlyChilds;
                    functionsList["Totals.MaxDOnlyChilds"] = StiFunctionType.MaxDOnlyChilds;
                    functionsList["Totals.MaxIAllLevels"] = StiFunctionType.MaxIAllLevels;
                    functionsList["Totals.MaxIAllLevelsOnlyChilds"] = StiFunctionType.MaxIAllLevelsOnlyChilds;
                    functionsList["Totals.MaxIOnlyChilds"] = StiFunctionType.MaxIOnlyChilds;

                    functionsList["Totals.MinAllLevels"] = StiFunctionType.MinAllLevels;
                    functionsList["Totals.MinAllLevelsOnlyChilds"] = StiFunctionType.MinAllLevelsOnlyChilds;
                    functionsList["Totals.MinOnlyChilds"] = StiFunctionType.MinOnlyChilds;
                    functionsList["Totals.MinDAllLevels"] = StiFunctionType.MinDAllLevels;
                    functionsList["Totals.MinDAllLevelsOnlyChilds"] = StiFunctionType.MinDAllLevelsOnlyChilds;
                    functionsList["Totals.MinDOnlyChilds"] = StiFunctionType.MinDOnlyChilds;
                    functionsList["Totals.MinIAllLevels"] = StiFunctionType.MinIAllLevels;
                    functionsList["Totals.MinIAllLevelsOnlyChilds"] = StiFunctionType.MinIAllLevelsOnlyChilds;
                    functionsList["Totals.MinIOnlyChilds"] = StiFunctionType.MinIOnlyChilds;

                    functionsList["Totals.MedianAllLevels"] = StiFunctionType.MedianAllLevels;
                    functionsList["Totals.MedianAllLevelsOnlyChilds"] = StiFunctionType.MedianAllLevelsOnlyChilds;
                    functionsList["Totals.MedianOnlyChilds"] = StiFunctionType.MedianOnlyChilds;
                    functionsList["Totals.MedianDAllLevels"] = StiFunctionType.MedianDAllLevels;
                    functionsList["Totals.MedianDAllLevelsOnlyChilds"] = StiFunctionType.MedianDAllLevelsOnlyChilds;
                    functionsList["Totals.MedianDOnlyChilds"] = StiFunctionType.MedianDOnlyChilds;
                    functionsList["Totals.MedianIAllLevels"] = StiFunctionType.MedianIAllLevels;
                    functionsList["Totals.MedianIAllLevelsOnlyChilds"] = StiFunctionType.MedianIAllLevelsOnlyChilds;
                    functionsList["Totals.MedianIOnlyChilds"] = StiFunctionType.MedianIOnlyChilds;

                    functionsList["Totals.ModeAllLevels"] = StiFunctionType.ModeAllLevels;
                    functionsList["Totals.ModeAllLevelsOnlyChilds"] = StiFunctionType.ModeAllLevelsOnlyChilds;
                    functionsList["Totals.ModeOnlyChilds"] = StiFunctionType.ModeOnlyChilds;
                    functionsList["Totals.ModeDAllLevels"] = StiFunctionType.ModeDAllLevels;
                    functionsList["Totals.ModeDAllLevelsOnlyChilds"] = StiFunctionType.ModeDAllLevelsOnlyChilds;
                    functionsList["Totals.ModeDOnlyChilds"] = StiFunctionType.ModeDOnlyChilds;
                    functionsList["Totals.ModeIAllLevels"] = StiFunctionType.ModeIAllLevels;
                    functionsList["Totals.ModeIAllLevelsOnlyChilds"] = StiFunctionType.ModeIAllLevelsOnlyChilds;
                    functionsList["Totals.ModeIOnlyChilds"] = StiFunctionType.ModeIOnlyChilds;

                    functionsList["Totals.FirstAllLevels"] = StiFunctionType.FirstAllLevels;
                    functionsList["Totals.FirstAllLevelsOnlyChilds"] = StiFunctionType.FirstAllLevelsOnlyChilds;
                    functionsList["Totals.FirstOnlyChilds"] = StiFunctionType.FirstOnlyChilds;
                    functionsList["Totals.LastAllLevels"] = StiFunctionType.LastAllLevels;
                    functionsList["Totals.LastAllLevelsOnlyChilds"] = StiFunctionType.LastAllLevelsOnlyChilds;
                    functionsList["Totals.LastOnlyChilds"] = StiFunctionType.LastOnlyChilds;

                    functionsList["Totals.MinDateAllLevels"] = StiFunctionType.MinDateAllLevels;
                    functionsList["Totals.MinDateAllLevelsOnlyChilds"] = StiFunctionType.MinDateAllLevelsOnlyChilds;
                    functionsList["Totals.MinDateOnlyChilds"] = StiFunctionType.MinDateOnlyChilds;
                    functionsList["Totals.MinTimeAllLevels"] = StiFunctionType.MinTimeAllLevels;
                    functionsList["Totals.MinTimeAllLevelsOnlyChilds"] = StiFunctionType.MinTimeAllLevelsOnlyChilds;
                    functionsList["Totals.MinTimeOnlyChilds"] = StiFunctionType.MinTimeOnlyChilds;
                    functionsList["Totals.MinStrAllLevels"] = StiFunctionType.MinStrAllLevels;
                    functionsList["Totals.MinStrAllLevelsOnlyChilds"] = StiFunctionType.MinStrAllLevelsOnlyChilds;
                    functionsList["Totals.MinStrOnlyChilds"] = StiFunctionType.MinStrOnlyChilds;

                    functionsList["Totals.MaxDateAllLevels"] = StiFunctionType.MaxDateAllLevels;
                    functionsList["Totals.MaxDateAllLevelsOnlyChilds"] = StiFunctionType.MaxDateAllLevelsOnlyChilds;
                    functionsList["Totals.MaxDateOnlyChilds"] = StiFunctionType.MaxDateOnlyChilds;
                    functionsList["Totals.MaxTimeAllLevels"] = StiFunctionType.MaxTimeAllLevels;
                    functionsList["Totals.MaxTimeAllLevelsOnlyChilds"] = StiFunctionType.MaxTimeAllLevelsOnlyChilds;
                    functionsList["Totals.MaxTimeOnlyChilds"] = StiFunctionType.MaxTimeOnlyChilds;
                    functionsList["Totals.MaxStrAllLevels"] = StiFunctionType.MaxStrAllLevels;
                    functionsList["Totals.MaxStrAllLevelsOnlyChilds"] = StiFunctionType.MaxStrAllLevelsOnlyChilds;
                    functionsList["Totals.MaxStrOnlyChilds"] = StiFunctionType.MaxStrOnlyChilds;
                    #endregion

                    #region Math
                    functionsList["Abs"] = StiFunctionType.Abs;
                    functionsList["Acos"] = StiFunctionType.Acos;
                    functionsList["Asin"] = StiFunctionType.Asin;
                    functionsList["Atan"] = StiFunctionType.Atan;
                    functionsList["Ceiling"] = StiFunctionType.Ceiling;
                    functionsList["Cos"] = StiFunctionType.Cos;
                    functionsList["Div"] = StiFunctionType.Div;
                    functionsList["Exp"] = StiFunctionType.Exp;
                    functionsList["Floor"] = StiFunctionType.Floor;
                    functionsList["Log"] = StiFunctionType.Log;
                    functionsList["Maximum"] = StiFunctionType.Maximum;
                    functionsList["Minimum"] = StiFunctionType.Minimum;
                    functionsList["Round"] = StiFunctionType.Round;
                    functionsList["Sign"] = StiFunctionType.Sign;
                    functionsList["Sin"] = StiFunctionType.Sin;
                    functionsList["Sqrt"] = StiFunctionType.Sqrt;
                    functionsList["Tan"] = StiFunctionType.Tan;
                    functionsList["Truncate"] = StiFunctionType.Truncate;
                    #endregion

                    #region Date
                    functionsList["DateDiff"] = StiFunctionType.DateDiff;
                    functionsList["DateSerial"] = StiFunctionType.DateSerial;
                    functionsList["Day"] = StiFunctionType.Day;
                    functionsList["DayOfWeek"] = StiFunctionType.DayOfWeek;
                    functionsList["DayOfYear"] = StiFunctionType.DayOfYear;
                    functionsList["DaysInMonth"] = StiFunctionType.DaysInMonth;
                    functionsList["DaysInYear"] = StiFunctionType.DaysInYear;
                    functionsList["Hour"] = StiFunctionType.Hour;
                    functionsList["Minute"] = StiFunctionType.Minute;
                    functionsList["Month"] = StiFunctionType.Month;
                    functionsList["Second"] = StiFunctionType.Second;
                    functionsList["TimeSerial"] = StiFunctionType.TimeSerial;
                    functionsList["Year"] = StiFunctionType.Year;
                    functionsList["MonthName"] = StiFunctionType.MonthName;
                    functionsList["WeekOfYear"] = StiFunctionType.WeekOfYear;
                    functionsList["WeekOfMonth"] = StiFunctionType.WeekOfMonth;
                    functionsList["FromOADate"] = StiFunctionType.FromOADate;
                    functionsList["ToOADate"] = StiFunctionType.ToOADate;
                    #endregion

                    #region String
                    functionsList["DateToStr"] = StiFunctionType.DateToStr;
                    functionsList["DateToStrPl"] = StiFunctionType.DateToStrPl;
                    functionsList["DateToStrRu"] = StiFunctionType.DateToStrRu;
                    functionsList["DateToStrUa"] = StiFunctionType.DateToStrUa;
                    functionsList["DateToStrPt"] = StiFunctionType.DateToStrPt;
                    functionsList["DateToStrPtBr"] = StiFunctionType.DateToStrPtBr;
                    functionsList["Insert"] = StiFunctionType.Insert;
                    functionsList["Length"] = StiFunctionType.Length;
                    functionsList["Remove"] = StiFunctionType.Remove;
                    functionsList["Replace"] = StiFunctionType.Replace;
                    functionsList["Roman"] = StiFunctionType.Roman;
                    functionsList["Substring"] = StiFunctionType.Substring;
                    functionsList["ToCurrencyWords"] = StiFunctionType.ToCurrencyWords;
                    functionsList["ToCurrencyWordsAr"] = StiFunctionType.ToCurrencyWordsAr;
                    functionsList["ToCurrencyWordsEnGb"] = StiFunctionType.ToCurrencyWordsEnGb;
                    functionsList["ToCurrencyWordsEnIn"] = StiFunctionType.ToCurrencyWordsEnIn;
                    functionsList["ToCurrencyWordsEs"] = StiFunctionType.ToCurrencyWordsEs;
                    functionsList["ToCurrencyWordsFr"] = StiFunctionType.ToCurrencyWordsFr;
                    functionsList["ToCurrencyWordsNl"] = StiFunctionType.ToCurrencyWordsNl;
                    functionsList["ToCurrencyWordsPl"] = StiFunctionType.ToCurrencyWordsPl;
                    functionsList["ToCurrencyWordsPt"] = StiFunctionType.ToCurrencyWordsPt;
                    functionsList["ToCurrencyWordsPtBr"] = StiFunctionType.ToCurrencyWordsPtBr;
                    functionsList["ToCurrencyWordsRu"] = StiFunctionType.ToCurrencyWordsRu;
                    functionsList["ToCurrencyWordsThai"] = StiFunctionType.ToCurrencyWordsThai;
                    functionsList["ToCurrencyWordsTr"] = StiFunctionType.ToCurrencyWordsTr;
                    functionsList["ToCurrencyWordsUa"] = StiFunctionType.ToCurrencyWordsUa;
                    functionsList["ToCurrencyWordsZh"] = StiFunctionType.ToCurrencyWordsZh;
                    functionsList["ToLowerCase"] = StiFunctionType.ToLowerCase;
                    functionsList["ToProperCase"] = StiFunctionType.ToProperCase;
                    functionsList["ToUpperCase"] = StiFunctionType.ToUpperCase;
                    functionsList["ToWords"] = StiFunctionType.ToWords;
                    functionsList["ToWordsEs"] = StiFunctionType.ToWordsEs;
                    functionsList["ToWordsEnIn"] = StiFunctionType.ToWordsEnIn;
                    functionsList["ToWordsAr"] = StiFunctionType.ToWordsAr;
                    functionsList["ToWordsFa"] = StiFunctionType.ToWordsFa;
                    functionsList["ToWordsPl"] = StiFunctionType.ToWordsPl;
                    functionsList["ToWordsPt"] = StiFunctionType.ToWordsPt;
                    functionsList["ToWordsRu"] = StiFunctionType.ToWordsRu;
                    functionsList["ToWordsTr"] = StiFunctionType.ToWordsTr;
                    functionsList["ToWordsUa"] = StiFunctionType.ToWordsUa;
                    functionsList["ToWordsZh"] = StiFunctionType.ToWordsZh;
                    functionsList["Trim"] = StiFunctionType.Trim;
                    functionsList["TryParseDecimal"] = StiFunctionType.TryParseDecimal;
                    functionsList["TryParseDouble"] = StiFunctionType.TryParseDouble;
                    functionsList["TryParseLong"] = StiFunctionType.TryParseLong;
                    functionsList["Arabic"] = StiFunctionType.Arabic;
                    functionsList["Persian"] = StiFunctionType.Persian;
                    functionsList["ToOrdinal"] = StiFunctionType.ToOrdinal;
                    functionsList["Left"] = StiFunctionType.Left;
                    functionsList["Mid"] = StiFunctionType.Mid;
                    functionsList["Right"] = StiFunctionType.Right;
                    functionsList["StrToDateTime"] = StiFunctionType.StrToDateTime;
                    functionsList["StrToNullableDateTime"] = StiFunctionType.StrToNullableDateTime;
                    functionsList["ConvertToBase64String"] = StiFunctionType.ConvertToBase64String;
                    #endregion

                    functionsList["IsNull"] = StiFunctionType.IsNull;
                    functionsList["Next"] = StiFunctionType.Next;
                    functionsList["NextIsNull"] = StiFunctionType.NextIsNull;
                    functionsList["Previous"] = StiFunctionType.Previous;
                    functionsList["PreviousIsNull"] = StiFunctionType.PreviousIsNull;

                    functionsList["IIF"] = StiFunctionType.IIF;
                    functionsList["Choose"] = StiFunctionType.Choose;
                    functionsList["Switch"] = StiFunctionType.Switch;

                    functionsList["ToString"] = StiFunctionType.ToString;
                    functionsList["Format"] = StiFunctionType.Format;

                    #region System.Convert
                    //functionsList["System"] = StiFunctionType.NameSpace;
                    //functionsList["System.Convert"] = StiFunctionType.NameSpace;
                    functionsList["System.Convert.ToBoolean"] = StiFunctionType.SystemConvertToBoolean;
                    functionsList["System.Convert.ToByte"] = StiFunctionType.SystemConvertToByte;
                    functionsList["System.Convert.ToChar"] = StiFunctionType.SystemConvertToChar;
                    functionsList["System.Convert.ToDateTime"] = StiFunctionType.SystemConvertToDateTime;
                    functionsList["System.Convert.ToDecimal"] = StiFunctionType.SystemConvertToDecimal;
                    functionsList["System.Convert.ToDouble"] = StiFunctionType.SystemConvertToDouble;
                    functionsList["System.Convert.ToInt16"] = StiFunctionType.SystemConvertToInt16;
                    functionsList["System.Convert.ToInt32"] = StiFunctionType.SystemConvertToInt32;
                    functionsList["System.Convert.ToInt64"] = StiFunctionType.SystemConvertToInt64;
                    functionsList["System.Convert.ToSByte"] = StiFunctionType.SystemConvertToSByte;
                    functionsList["System.Convert.ToSingle"] = StiFunctionType.SystemConvertToSingle;
                    functionsList["System.Convert.ToString"] = StiFunctionType.SystemConvertToString;
                    functionsList["System.Convert.ToUInt16"] = StiFunctionType.SystemConvertToUInt16;
                    functionsList["System.Convert.ToUInt32"] = StiFunctionType.SystemConvertToUInt32;
                    functionsList["System.Convert.ToUInt64"] = StiFunctionType.SystemConvertToUInt64;
                    //functionsList["Convert"] = StiFunctionType.NameSpace;
                    functionsList["Convert.ToBoolean"] = StiFunctionType.SystemConvertToBoolean;
                    functionsList["Convert.ToByte"] = StiFunctionType.SystemConvertToByte;
                    functionsList["Convert.ToChar"] = StiFunctionType.SystemConvertToChar;
                    functionsList["Convert.ToDateTime"] = StiFunctionType.SystemConvertToDateTime;
                    functionsList["Convert.ToDecimal"] = StiFunctionType.SystemConvertToDecimal;
                    functionsList["Convert.ToDouble"] = StiFunctionType.SystemConvertToDouble;
                    functionsList["Convert.ToInt16"] = StiFunctionType.SystemConvertToInt16;
                    functionsList["Convert.ToInt32"] = StiFunctionType.SystemConvertToInt32;
                    functionsList["Convert.ToInt64"] = StiFunctionType.SystemConvertToInt64;
                    functionsList["Convert.ToSByte"] = StiFunctionType.SystemConvertToSByte;
                    functionsList["Convert.ToSingle"] = StiFunctionType.SystemConvertToSingle;
                    functionsList["Convert.ToString"] = StiFunctionType.SystemConvertToString;
                    functionsList["Convert.ToUInt16"] = StiFunctionType.SystemConvertToUInt16;
                    functionsList["Convert.ToUInt32"] = StiFunctionType.SystemConvertToUInt32;
                    functionsList["Convert.ToUInt64"] = StiFunctionType.SystemConvertToUInt64;
                    #endregion

                    #region Other
                    //functionsList["Math"] = StiFunctionType.NameSpace;
                    functionsList["Math.Round"] = StiFunctionType.MathRound;
                    functionsList["Math.Pow"] = StiFunctionType.MathPow;

                    functionsList["AddAnchor"] = StiFunctionType.AddAnchor;
                    functionsList["GetAnchorPageNumber"] = StiFunctionType.GetAnchorPageNumber;
                    functionsList["GetAnchorPageNumberThrough"] = StiFunctionType.GetAnchorPageNumberThrough;

                    functionsList["ConvertRtf"] = StiFunctionType.ConvertRtf;
                    functionsList["GetLabel"] = StiFunctionType.GetLabel;
                    functionsList["GetParam"] = StiFunctionType.GetParam;

                    functionsList["string.IsNullOrEmpty"] = StiFunctionType.StringIsNullOrEmpty;
                    functionsList["String.IsNullOrEmpty"] = StiFunctionType.StringIsNullOrEmpty;
                    functionsList["string.IsNullOrWhiteSpace"] = StiFunctionType.StringIsNullOrWhiteSpace;
                    functionsList["String.IsNullOrWhiteSpace"] = StiFunctionType.StringIsNullOrWhiteSpace;
                    functionsList["string.Format"] = StiFunctionType.Format;
                    functionsList["String.Format"] = StiFunctionType.Format;

                    //functionsList["Func"] = StiFunctionType.NameSpace;
                    //functionsList["Func.EngineHelper"] = StiFunctionType.NameSpace;
                    functionsList["Func.EngineHelper.JoinColumnContent"] = StiFunctionType.EngineHelperJoinColumnContent;
                    functionsList["Func.EngineHelper.ToQueryString"] = StiFunctionType.EngineHelperToQueryString;
                    functionsList["Func.EngineHelper.GetRealPageNumber"] = StiFunctionType.EngineHelperGetRealPageNumber;

                    functionsList["StiNullValuesHelper.IsNull"] = StiFunctionType.StiNullValuesHelperIsNull;

                    functionsList["TimeSpan.FromDays"] = StiFunctionType.TimeSpanFromDays;
                    functionsList["TimeSpan.FromHours"] = StiFunctionType.TimeSpanFromHours;
                    functionsList["TimeSpan.FromMilliseconds"] = StiFunctionType.TimeSpanFromMilliseconds;
                    functionsList["TimeSpan.FromMinutes"] = StiFunctionType.TimeSpanFromMinutes;
                    functionsList["TimeSpan.FromSeconds"] = StiFunctionType.TimeSpanFromSeconds;
                    functionsList["TimeSpan.FromTicks"] = StiFunctionType.TimeSpanFromTicks;

                    functionsList["Image.FromFile"] = StiFunctionType.ImageFromFile;
                    #endregion


                    functionsList_vb = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                    foreach (DictionaryEntry de in functionsList)
                    {
                        functionsList_vb[((string)de.Key).ToLowerInvariant()] = de.Value;
                    }

                    functionsList_vb["Integer.Parse"] = StiFunctionType.ParseInt;
                    functionsList_vb["Double.Parse"] = StiFunctionType.ParseDouble;
                    functionsList_vb["Decimal.Parse"] = StiFunctionType.ParseDecimal;
                    functionsList_vb["Date.Parse"] = StiFunctionType.ParseDateTime;    //for VB, temp.

                    functionsList["int.Parse"] = StiFunctionType.ParseInt;
                    functionsList["Int16.Parse"] = StiFunctionType.ParseInt;
                    functionsList["Int32.Parse"] = StiFunctionType.ParseInt;
                    functionsList["Int64.Parse"] = StiFunctionType.ParseInt64;
                    functionsList["double.Parse"] = StiFunctionType.ParseDouble;
                    functionsList["Double.Parse"] = StiFunctionType.ParseDouble;
                    functionsList["decimal.Parse"] = StiFunctionType.ParseDecimal;
                    functionsList["Decimal.Parse"] = StiFunctionType.ParseDecimal;
                    functionsList["TimeSpan.Parse"] = StiFunctionType.ParseTimeSpan;
                    functionsList["DateTime.Parse"] = StiFunctionType.ParseDateTime;
                    functionsList["DateTimeOffset.Parse"] = StiFunctionType.ParseDateTimeOffset;
                    functionsList["Date.Parse"] = StiFunctionType.ParseDateTime;    //for VB, temp.

                    functionsList["DateTime.DaysInMonth"] = StiFunctionType.DaysInMonth;

                    functionsList_low = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                    foreach (DictionaryEntry de in functionsList)
                    {
                        functionsList_low[((string)de.Key).ToLowerInvariant()] = de.Value;
                    }
                }
                if (IsVB) return functionsList_vb;
                return syntaxCaseSensitive ? functionsList : functionsList_low;
            }
        }
        #endregion

        #region MethodsList
        private static Hashtable methodsList = null;
        private static Hashtable methodsList_low = null;
        private Hashtable MethodsList
        {
            get
            {
                if (methodsList == null)
                {
                    methodsList = new Hashtable();
                    //methodsList[""] = StiFunctionType.FunctionNameSpace;
                    methodsList["Substring"] = StiMethodType.Substring;
                    methodsList["ToString"] = StiMethodType.ToString;
                    methodsList["ToLower"] = StiMethodType.ToLower;
                    methodsList["ToUpper"] = StiMethodType.ToUpper;
                    methodsList["IndexOf"] = StiMethodType.IndexOf;
                    methodsList["StartsWith"] = StiMethodType.StartsWith;
                    methodsList["EndsWith"] = StiMethodType.EndsWith;
                    methodsList["Replace"] = StiMethodType.Replace;
                    methodsList["PadLeft"] = StiMethodType.PadLeft;
                    methodsList["PadRight"] = StiMethodType.PadRight;
                    methodsList["TrimStart"] = StiMethodType.TrimStart;
                    methodsList["TrimEnd"] = StiMethodType.TrimEnd;

                    methodsList["Parse"] = StiMethodType.Parse;
                    methodsList["Contains"] = StiMethodType.Contains;
                    methodsList["GetData"] = StiMethodType.GetData;
                    methodsList["ToQueryString"] = StiMethodType.ToQueryString;

                    methodsList["AddYears"] = StiMethodType.AddYears;
                    methodsList["AddMonths"] = StiMethodType.AddMonths;
                    methodsList["AddDays"] = StiMethodType.AddDays;
                    methodsList["AddHours"] = StiMethodType.AddHours;
                    methodsList["AddMinutes"] = StiMethodType.AddMinutes;
                    methodsList["AddSeconds"] = StiMethodType.AddSeconds;
                    methodsList["AddMilliseconds"] = StiMethodType.AddMilliseconds;
                    methodsList["ToShortDateString"] = StiMethodType.ToShortDateString;
                    methodsList["ToShortTimeString"] = StiMethodType.ToShortTimeString;
                    methodsList["ToLongDateString"] = StiMethodType.ToLongDateString;
                    methodsList["ToLongTimeString"] = StiMethodType.ToLongTimeString;

                    methodsList["GetCurrentConditionValue"] = StiMethodType.GetCurrentConditionValue;

                    methodsList["Add"] = StiMethodType.Add;
                    methodsList["Subtract"] = StiMethodType.Subtract;

                    methodsList_low = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                    foreach (DictionaryEntry de in methodsList)
                    {
                        methodsList_low[((string)de.Key).ToLowerInvariant()] = de.Value;
                    }
                }
                return syntaxCaseSensitive ? methodsList : methodsList_low;
            }
        }
        #endregion

        #region ParametersList
        // список параметров функций, которые не надо вычислять сразу, 
        // а оставлять в виде кода для последующего вычисления
        private static Hashtable parametersList = null;
        private static Hashtable ParametersList
        {
            get
            {
                if (parametersList == null)
                {
                    parametersList = new Hashtable();

                    #region Fill parameters list

                    #region Aggregate functions - Report
                    parametersList[StiFunctionType.CountDistinct] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.Avg] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.AvgD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.AvgDate] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.AvgI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.AvgTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.Max] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxDate] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxStr] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.Median] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MedianD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MedianI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.Min] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinDate] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinStr] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.Mode] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.ModeD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.ModeI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.Sum] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.SumD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.SumDistinct] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.SumI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.SumTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.First] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.Last] = StiParameterNumber.Param2;

                    parametersList[StiFunctionType.rCountDistinct] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rAvg] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rAvgD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rAvgDate] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rAvgI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rAvgTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rMax] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rMaxD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rMaxDate] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rMaxI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rMaxStr] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rMaxTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rMedian] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rMedianD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rMedianI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rMin] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rMinD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rMinDate] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rMinI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rMinStr] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rMinTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rMode] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rModeD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rModeI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rSum] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rSumD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rSumDistinct] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.rSumI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rSumTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rFirst] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.rLast] = StiParameterNumber.Param2;

                    parametersList[StiFunctionType.iCount] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.iCountDistinct] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iAvg] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iAvgD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iAvgDate] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iAvgI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iAvgTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iMax] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iMaxD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iMaxDate] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iMaxI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iMaxStr] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iMaxTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iMedian] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iMedianD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iMedianI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iMin] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iMinD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iMinDate] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iMinI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iMinStr] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iMinTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iMode] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iModeD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iModeI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iSum] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iSumD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iSumDistinct] = StiParameterNumber.Param2 | StiParameterNumber.Param3 | StiParameterNumber.Param4;
                    parametersList[StiFunctionType.iSumI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iSumTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iFirst] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.iLast] = StiParameterNumber.Param2 | StiParameterNumber.Param3;

                    parametersList[StiFunctionType.riCount] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.riCountDistinct] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riAvg] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riAvgD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riAvgDate] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riAvgI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riAvgTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riMax] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riMaxD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riMaxDate] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riMaxI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riMaxStr] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riMaxTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riMedian] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riMedianD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riMedianI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riMin] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riMinD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riMinDate] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riMinI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riMinStr] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riMinTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riMode] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riModeD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riModeI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riSum] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riSumD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riSumDistinct] = StiParameterNumber.Param2 | StiParameterNumber.Param3 | StiParameterNumber.Param4;
                    parametersList[StiFunctionType.riSumI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riSumTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riFirst] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.riLast] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    #endregion

                    #region Aggregate functions - Column
                    parametersList[StiFunctionType.cCountDistinct] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cAvg] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cAvgD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cAvgDate] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cAvgI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cAvgTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cMax] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cMaxD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cMaxDate] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cMaxI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cMaxStr] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cMaxTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cMedian] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cMedianD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cMedianI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cMin] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cMinD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cMinDate] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cMinI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cMinStr] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cMinTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cMode] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cModeD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cModeI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cSum] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cSumD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cSumDistinct] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.cSumI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cSumTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cFirst] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.cLast] = StiParameterNumber.Param2;

                    parametersList[StiFunctionType.crCountDistinct] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crAvg] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crAvgD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crAvgDate] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crAvgI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crAvgTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crMax] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crMaxD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crMaxDate] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crMaxI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crMaxStr] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crMaxTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crMedian] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crMedianD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crMedianI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crMin] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crMinD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crMinDate] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crMinI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crMinStr] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crMinTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crMode] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crModeD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crModeI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crSum] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crSumD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crSumDistinct] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.crSumI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crSumTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crFirst] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.crLast] = StiParameterNumber.Param2;

                    parametersList[StiFunctionType.ciCount] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.ciCountDistinct] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciAvg] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciAvgD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciAvgDate] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciAvgI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciAvgTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciMax] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciMaxD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciMaxDate] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciMaxI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciMaxStr] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciMaxTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciMedian] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciMedianD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciMedianI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciMin] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciMinD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciMinDate] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciMinI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciMinStr] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciMinTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciMode] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciModeD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciModeI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciSum] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciSumD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciSumDistinct] = StiParameterNumber.Param2 | StiParameterNumber.Param3 | StiParameterNumber.Param4;
                    parametersList[StiFunctionType.ciSumI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciSumTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciFirst] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.ciLast] = StiParameterNumber.Param2 | StiParameterNumber.Param3;

                    parametersList[StiFunctionType.criCount] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.criCountDistinct] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criAvg] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criAvgD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criAvgDate] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criAvgI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criAvgTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criMax] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criMaxD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criMaxDate] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criMaxI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criMaxStr] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criMaxTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criMedian] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criMedianD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criMedianI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criMin] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criMinD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criMinDate] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criMinI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criMinStr] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criMinTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criMode] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criModeD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criModeI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criSum] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criSumD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criSumDistinct] = StiParameterNumber.Param2 | StiParameterNumber.Param3 | StiParameterNumber.Param4;
                    parametersList[StiFunctionType.criSumI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criSumTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criFirst] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.criLast] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    #endregion

                    #region Aggregate functions - Page
                    parametersList[StiFunctionType.pCountDistinct] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pAvg] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pAvgD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pAvgDate] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pAvgI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pAvgTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pMax] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pMaxD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pMaxDate] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pMaxI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pMaxStr] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pMaxTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pMedian] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pMedianD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pMedianI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pMin] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pMinD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pMinDate] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pMinI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pMinStr] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pMinTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pMode] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pModeD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pModeI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pSum] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pSumD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pSumDistinct] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.pSumI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pSumTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pFirst] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.pLast] = StiParameterNumber.Param2;

                    parametersList[StiFunctionType.prCountDistinct] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prAvg] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prAvgD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prAvgDate] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prAvgI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prAvgTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prMax] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prMaxD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prMaxDate] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prMaxI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prMaxStr] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prMaxTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prMedian] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prMedianD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prMedianI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prMin] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prMinD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prMinDate] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prMinI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prMinStr] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prMinTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prMode] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prModeD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prModeI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prSum] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prSumD] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prSumDistinct] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.prSumI] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prSumTime] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prFirst] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.prLast] = StiParameterNumber.Param2;

                    parametersList[StiFunctionType.piCount] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.piCountDistinct] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piAvg] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piAvgD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piAvgDate] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piAvgI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piAvgTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piMax] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piMaxD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piMaxDate] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piMaxI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piMaxStr] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piMaxTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piMedian] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piMedianD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piMedianI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piMin] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piMinD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piMinDate] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piMinI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piMinStr] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piMinTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piMode] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piModeD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piModeI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piSum] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piSumD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piSumDistinct] = StiParameterNumber.Param2 | StiParameterNumber.Param3 | StiParameterNumber.Param4;
                    parametersList[StiFunctionType.piSumI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piSumTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piFirst] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.piLast] = StiParameterNumber.Param2 | StiParameterNumber.Param3;

                    parametersList[StiFunctionType.priCount] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.priCountDistinct] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priAvg] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priAvgD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priAvgDate] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priAvgI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priAvgTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priMax] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priMaxD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priMaxDate] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priMaxI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priMaxStr] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priMaxTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priMedian] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priMedianD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priMedianI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priMin] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priMinD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priMinDate] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priMinI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priMinStr] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priMinTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priMode] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priModeD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priModeI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priSum] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priSumD] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priSumDistinct] = StiParameterNumber.Param2 | StiParameterNumber.Param3 | StiParameterNumber.Param4;
                    parametersList[StiFunctionType.priSumI] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priSumTime] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priFirst] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    parametersList[StiFunctionType.priLast] = StiParameterNumber.Param2 | StiParameterNumber.Param3;
                    #endregion

                    #region Totals Hierarchical
                    parametersList[StiFunctionType.CountDistinctAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.CountDistinctAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.CountDistinctOnlyChilds] = StiParameterNumber.Param2;

                    parametersList[StiFunctionType.SumAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.SumAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.SumOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.SumDAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.SumDAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.SumDOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.SumIAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.SumIAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.SumIOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.SumTimeAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.SumTimeAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.SumTimeOnlyChilds] = StiParameterNumber.Param2;

                    parametersList[StiFunctionType.AvgAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.AvgAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.AvgOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.AvgDAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.AvgDAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.AvgDOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.AvgIAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.AvgIAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.AvgIOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.AvgDateAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.AvgDateAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.AvgDateOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.AvgTimeAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.AvgTimeAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.AvgTimeOnlyChilds] = StiParameterNumber.Param2;

                    parametersList[StiFunctionType.MaxAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxDAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxDAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxDOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxIAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxIAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxIOnlyChilds] = StiParameterNumber.Param2;

                    parametersList[StiFunctionType.MinAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinDAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinDAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinDOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinIAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinIAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinIOnlyChilds] = StiParameterNumber.Param2;

                    parametersList[StiFunctionType.MedianAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MedianAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MedianOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MedianDAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MedianDAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MedianDOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MedianIAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MedianIAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MedianIOnlyChilds] = StiParameterNumber.Param2;

                    parametersList[StiFunctionType.ModeAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.ModeAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.ModeOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.ModeDAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.ModeDAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.ModeDOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.ModeIAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.ModeIAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.ModeIOnlyChilds] = StiParameterNumber.Param2;

                    parametersList[StiFunctionType.FirstAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.FirstAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.FirstOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.LastAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.LastAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.LastOnlyChilds] = StiParameterNumber.Param2;

                    parametersList[StiFunctionType.MinDateAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinDateAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinDateOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinTimeAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinTimeAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinTimeOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinStrAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinStrAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MinStrOnlyChilds] = StiParameterNumber.Param2;

                    parametersList[StiFunctionType.MaxDateAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxDateAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxDateOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxTimeAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxTimeAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxTimeOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxStrAllLevels] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxStrAllLevelsOnlyChilds] = StiParameterNumber.Param2;
                    parametersList[StiFunctionType.MaxStrOnlyChilds] = StiParameterNumber.Param2;
                    #endregion

                    parametersList[StiFunctionType.Rank] = StiParameterNumber.Param2;
                    #endregion
                }
                return parametersList;
            }
        }
        #endregion

        #region ComponentsList
        private Hashtable componentsList = null;
        private Hashtable ComponentsList
        {
            get
            {
                if (componentsList == null)
                {
                    componentsList = new Hashtable();
                    StiComponentsCollection comps = report.GetComponents();
                    foreach (StiComponent comp in comps)
                    {
                        componentsList[comp.Name] = comp;
                    }
                    componentsList["this"] = report;
                }
                return componentsList;
            }
        }
        #endregion

        #region MethodsHash
        private static object lockMethodsHash = new object();
        private static Hashtable methodsHash = null;
        private static Hashtable MethodsHash
        {
            get
            {
                if (methodsHash == null)
                {
                    lock (lockMethodsHash)
                    {
                        StiParserMethodInfo[] methods = new StiParserMethodInfo[]
                        {
                            #region Date
                            new StiParserMethodInfo(StiFunctionType.DateDiff, 1, new Type[] {typeof(DateTime), typeof(DateTime)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.DateDiff, 2, new Type[] {typeof(DateTime?), typeof(DateTime?)}, typeof(TimeSpan?)),
                            new StiParserMethodInfo(StiFunctionType.DateDiff, 3, new Type[] {typeof(DateTimeOffset), typeof(DateTimeOffset) }, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.DateDiff, 4, new Type[] {typeof(DateTimeOffset?), typeof(DateTimeOffset?)}, typeof(TimeSpan?)),

                            new StiParserMethodInfo(StiFunctionType.DateSerial, 1, new Type[] {typeof(long)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.DateSerial, 2, new Type[] {typeof(long), typeof(long), typeof(long)}, typeof(DateTime)),

                            new StiParserMethodInfo(StiFunctionType.TimeSerial, 1, new Type[] {typeof(long), typeof(long), typeof(long)}, typeof(TimeSpan)),

                            new StiParserMethodInfo(StiFunctionType.Year, 1, new Type[] {typeof(DateTime)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Year, 2, new Type[] {typeof(DateTime?)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Year, 3, new Type[] {typeof(DateTimeOffset)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Year, 4, new Type[] {typeof(DateTimeOffset?)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Month, 1, new Type[] {typeof(DateTime)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Month, 2, new Type[] {typeof(DateTime?)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Month, 3, new Type[] {typeof(DateTimeOffset) }, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Month, 4, new Type[] {typeof(DateTimeOffset?)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Day, 1, new Type[] {typeof(DateTime)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Day, 2, new Type[] {typeof(DateTime?)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Day, 3, new Type[] {typeof(DateTimeOffset) }, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Day, 4, new Type[] {typeof(DateTimeOffset?)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Hour, 1, new Type[] {typeof(DateTime)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Hour, 2, new Type[] {typeof(DateTime?)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Hour, 3, new Type[] {typeof(DateTimeOffset) }, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Hour, 4, new Type[] {typeof(DateTimeOffset?)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Minute, 1, new Type[] {typeof(DateTime)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Minute, 2, new Type[] {typeof(DateTime?)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Minute, 3, new Type[] {typeof(DateTimeOffset) }, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Minute, 4, new Type[] {typeof(DateTimeOffset?)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Second, 1, new Type[] {typeof(DateTime)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Second, 2, new Type[] {typeof(DateTime?)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Second, 3, new Type[] {typeof(DateTimeOffset) }, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Second, 4, new Type[] {typeof(DateTimeOffset?)}, typeof(Int64)),

                            new StiParserMethodInfo(StiFunctionType.DayOfWeek, 1, new Type[] {typeof(DateTime)}),
                            new StiParserMethodInfo(StiFunctionType.DayOfWeek, 2, new Type[] {typeof(DateTime?)}),
                            new StiParserMethodInfo(StiFunctionType.DayOfWeek, 3, new Type[] {typeof(DateTime), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.DayOfWeek, 4, new Type[] {typeof(DateTime?), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.DayOfWeek, 5, new Type[] {typeof(DateTime), typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.DayOfWeek, 6, new Type[] {typeof(DateTime?), typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.DayOfWeek, 7, new Type[] {typeof(DateTime), typeof(string), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.DayOfWeek, 8, new Type[] {typeof(DateTime?), typeof(string), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.DayOfWeek, 9, new Type[] {typeof(DateTimeOffset) }),
                            new StiParserMethodInfo(StiFunctionType.DayOfWeek, 10, new Type[] {typeof(DateTimeOffset?)}),
                            new StiParserMethodInfo(StiFunctionType.DayOfWeek, 11, new Type[] {typeof(DateTimeOffset), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.DayOfWeek, 12, new Type[] {typeof(DateTimeOffset?), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.DayOfWeek, 13, new Type[] {typeof(DateTimeOffset), typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.DayOfWeek, 14, new Type[] {typeof(DateTimeOffset?), typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.DayOfWeek, 15, new Type[] {typeof(DateTimeOffset), typeof(string), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.DayOfWeek, 16, new Type[] {typeof(DateTimeOffset?), typeof(string), typeof(bool)}),

                            new StiParserMethodInfo(StiFunctionType.DayOfYear, 1, new Type[] {typeof(DateTime)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.DayOfYear, 2, new Type[] {typeof(DateTime?)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.DayOfYear, 3, new Type[] {typeof(DateTimeOffset) }, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.DayOfYear, 4, new Type[] {typeof(DateTimeOffset?)}, typeof(long)),

                            new StiParserMethodInfo(StiFunctionType.DaysInMonth, 1, new Type[] {typeof(DateTime)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.DaysInMonth, 2, new Type[] {typeof(DateTime?)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.DaysInMonth, 3, new Type[] {typeof(long), typeof(long)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.DaysInMonth, 4, new Type[] {typeof(DateTimeOffset) }, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.DaysInMonth, 5, new Type[] {typeof(DateTimeOffset?)}, typeof(long)),

                            new StiParserMethodInfo(StiFunctionType.DaysInYear, 1, new Type[] {typeof(DateTime)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.DaysInYear, 2, new Type[] {typeof(DateTime?)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.DaysInYear, 3, new Type[] {typeof(long)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.DaysInYear, 4, new Type[] {typeof(DateTimeOffset) }, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.DaysInYear, 5, new Type[] {typeof(DateTimeOffset?)}, typeof(long)),

                            new StiParserMethodInfo(StiFunctionType.MonthName, 1, new Type[] {typeof(DateTime)}),
                            new StiParserMethodInfo(StiFunctionType.MonthName, 2, new Type[] {typeof(DateTime?)}),
                            new StiParserMethodInfo(StiFunctionType.MonthName, 3, new Type[] {typeof(DateTime), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.MonthName, 4, new Type[] {typeof(DateTime?), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.MonthName, 5, new Type[] {typeof(DateTime), typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.MonthName, 6, new Type[] {typeof(DateTime?), typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.MonthName, 7, new Type[] {typeof(DateTime), typeof(string), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.MonthName, 8, new Type[] {typeof(DateTime?), typeof(string), typeof(bool)}),
                            //new StiParserMethodInfo(StiFunctionType.MonthName, 9, new Type[] {typeof(DateTimeOffset) }),
                            //new StiParserMethodInfo(StiFunctionType.MonthName, 10, new Type[] {typeof(DateTimeOffset?)}),
                            //new StiParserMethodInfo(StiFunctionType.MonthName, 11, new Type[] {typeof(DateTimeOffset), typeof(bool)}),
                            //new StiParserMethodInfo(StiFunctionType.MonthName, 12, new Type[] {typeof(DateTimeOffset?), typeof(bool)}),
                            //new StiParserMethodInfo(StiFunctionType.MonthName, 13, new Type[] {typeof(DateTimeOffset), typeof(string)}),
                            //new StiParserMethodInfo(StiFunctionType.MonthName, 14, new Type[] {typeof(DateTimeOffset?), typeof(string)}),
                            //new StiParserMethodInfo(StiFunctionType.MonthName, 15, new Type[] {typeof(DateTimeOffset), typeof(string), typeof(bool)}),
                            //new StiParserMethodInfo(StiFunctionType.MonthName, 16, new Type[] {typeof(DateTimeOffset?), typeof(string), typeof(bool)}),

                            new StiParserMethodInfo(StiFunctionType.WeekOfYear, 1, new Type[] {typeof(DateTime)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.WeekOfYear, 2, new Type[] {typeof(DateTime?)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.WeekOfYear, 3, new Type[] {typeof(DateTime), typeof(DayOfWeek)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.WeekOfYear, 4, new Type[] {typeof(DateTime?), typeof(DayOfWeek)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.WeekOfYear, 5, new Type[] {typeof(DateTime), typeof(DayOfWeek), typeof(CalendarWeekRule)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.WeekOfYear, 6, new Type[] {typeof(DateTime?), typeof(DayOfWeek), typeof(CalendarWeekRule)}, typeof(long)),
                            //new StiParserMethodInfo(StiFunctionType.WeekOfYear, 7, new Type[] {typeof(DateTimeOffset) }, typeof(long)),
                            //new StiParserMethodInfo(StiFunctionType.WeekOfYear, 8, new Type[] {typeof(DateTimeOffset?)}, typeof(long)),
                            //new StiParserMethodInfo(StiFunctionType.WeekOfYear, 9, new Type[] {typeof(DateTimeOffset), typeof(DayOfWeek)}, typeof(long)),
                            //new StiParserMethodInfo(StiFunctionType.WeekOfYear, 10, new Type[] {typeof(DateTimeOffset?), typeof(DayOfWeek)}, typeof(long)),
                            //new StiParserMethodInfo(StiFunctionType.WeekOfYear, 11, new Type[] {typeof(DateTimeOffset), typeof(DayOfWeek), typeof(CalendarWeekRule)}, typeof(long)),
                            //new StiParserMethodInfo(StiFunctionType.WeekOfYear, 12, new Type[] {typeof(DateTimeOffset?), typeof(DayOfWeek), typeof(CalendarWeekRule)}, typeof(long)),

                            new StiParserMethodInfo(StiFunctionType.WeekOfMonth, 1, new Type[] {typeof(DateTime)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.WeekOfMonth, 2, new Type[] {typeof(DateTime?)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.WeekOfMonth, 3, new Type[] {typeof(DateTime), typeof(DayOfWeek)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.WeekOfMonth, 4, new Type[] {typeof(DateTime?), typeof(DayOfWeek)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.WeekOfMonth, 5, new Type[] {typeof(DateTime), typeof(DayOfWeek), typeof(CalendarWeekRule)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.WeekOfMonth, 6, new Type[] {typeof(DateTime?), typeof(DayOfWeek), typeof(CalendarWeekRule)}, typeof(long)),
                            //new StiParserMethodInfo(StiFunctionType.WeekOfMonth, 7, new Type[] {typeof(DateTimeOffset) }, typeof(long)),
                            //new StiParserMethodInfo(StiFunctionType.WeekOfMonth, 8, new Type[] {typeof(DateTimeOffset?)}, typeof(long)),
                            //new StiParserMethodInfo(StiFunctionType.WeekOfMonth, 9, new Type[] {typeof(DateTimeOffset), typeof(DayOfWeek)}, typeof(long)),
                            //new StiParserMethodInfo(StiFunctionType.WeekOfMonth, 10, new Type[] {typeof(DateTimeOffset?), typeof(DayOfWeek)}, typeof(long)),
                            //new StiParserMethodInfo(StiFunctionType.WeekOfMonth, 11, new Type[] {typeof(DateTimeOffset), typeof(DayOfWeek), typeof(CalendarWeekRule)}, typeof(long)),
                            //new StiParserMethodInfo(StiFunctionType.WeekOfMonth, 12, new Type[] {typeof(DateTimeOffset?), typeof(DayOfWeek), typeof(CalendarWeekRule)}, typeof(long)),

                            new StiParserMethodInfo(StiFunctionType.FromOADate, 1, new Type[] {typeof(double)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.ToOADate, 1, new Type[] {typeof(DateTime)}, typeof(double)),
                            #endregion
                            
                            #region Math
                            new StiParserMethodInfo(StiFunctionType.Abs, 1, new Type[] {typeof(long)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.Abs, 2, new Type[] {typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.Abs, 3, new Type[] {typeof(decimal)}, typeof(decimal)),

                            new StiParserMethodInfo(StiFunctionType.Acos, 1, new Type[] {typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.Asin, 1, new Type[] {typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.Atan, 1, new Type[] {typeof(double)}, typeof(double)),

                            new StiParserMethodInfo(StiFunctionType.Cos, 1, new Type[] {typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.Sin, 1, new Type[] {typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.Tan, 1, new Type[] {typeof(double)}, typeof(double)),

                            new StiParserMethodInfo(StiFunctionType.Ceiling, 1, new Type[] {typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.Ceiling, 2, new Type[] {typeof(decimal)}, typeof(decimal)),

                            new StiParserMethodInfo(StiFunctionType.Div, 1, new Type[] {typeof(long), typeof(long)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.Div, 2, new Type[] {typeof(long), typeof(long), typeof(long)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.Div, 3, new Type[] {typeof(double), typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.Div, 4, new Type[] {typeof(double), typeof(double), typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.Div, 5, new Type[] {typeof(decimal), typeof(decimal)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.Div, 6, new Type[] {typeof(decimal), typeof(decimal), typeof(decimal)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.Div, 7, new Type[] {typeof(long?), typeof(long?)}, typeof(long?)),
                            new StiParserMethodInfo(StiFunctionType.Div, 8, new Type[] {typeof(long?), typeof(long?), typeof(long?)}, typeof(long?)),
                            new StiParserMethodInfo(StiFunctionType.Div, 9, new Type[] {typeof(double?), typeof(double?)}, typeof(double?)),
                            new StiParserMethodInfo(StiFunctionType.Div, 10, new Type[] {typeof(double?), typeof(double?), typeof(double?)}, typeof(double?)),
                            new StiParserMethodInfo(StiFunctionType.Div, 11, new Type[] {typeof(decimal?), typeof(decimal?)}, typeof(decimal?)),
                            new StiParserMethodInfo(StiFunctionType.Div, 12, new Type[] {typeof(decimal?), typeof(decimal?), typeof(decimal?)}, typeof(decimal?)),

                            new StiParserMethodInfo(StiFunctionType.Exp, 1, new Type[] {typeof(double)}, typeof(double)),

                            new StiParserMethodInfo(StiFunctionType.Floor, 1, new Type[] {typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.Floor, 2, new Type[] {typeof(decimal)}, typeof(decimal)),

                            new StiParserMethodInfo(StiFunctionType.Log, 1, new Type[] {typeof(double)}, typeof(double)),

                            new StiParserMethodInfo(StiFunctionType.Maximum, 1, new Type[] {typeof(long), typeof(long)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.Maximum, 2, new Type[] {typeof(double), typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.Maximum, 3, new Type[] {typeof(decimal), typeof(decimal)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.Maximum, 4, new Type[] {typeof(long?), typeof(long?)}, typeof(long?)),
                            new StiParserMethodInfo(StiFunctionType.Maximum, 5, new Type[] {typeof(double?), typeof(double?)}, typeof(double?)),
                            new StiParserMethodInfo(StiFunctionType.Maximum, 6, new Type[] {typeof(decimal?), typeof(decimal?)}, typeof(decimal?)),

                            new StiParserMethodInfo(StiFunctionType.Minimum, 1, new Type[] {typeof(long), typeof(long)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.Minimum, 2, new Type[] {typeof(double), typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.Minimum, 3, new Type[] {typeof(decimal), typeof(decimal)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.Minimum, 4, new Type[] {typeof(long?), typeof(long?)}, typeof(long?)),
                            new StiParserMethodInfo(StiFunctionType.Minimum, 5, new Type[] {typeof(double?), typeof(double?)}, typeof(double?)),
                            new StiParserMethodInfo(StiFunctionType.Minimum, 6, new Type[] {typeof(decimal?), typeof(decimal?)}, typeof(decimal?)),

                            new StiParserMethodInfo(StiFunctionType.Round, 1, new Type[] {typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.Round, 2, new Type[] {typeof(double), typeof(int)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.Round, 3, new Type[] {typeof(decimal)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.Round, 4, new Type[] {typeof(decimal), typeof(int)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.Round, 5, new Type[] {typeof(double), typeof(MidpointRounding)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.Round, 6, new Type[] {typeof(double), typeof(int), typeof(MidpointRounding)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.Round, 7, new Type[] {typeof(decimal), typeof(MidpointRounding)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.Round, 8, new Type[] {typeof(decimal), typeof(int), typeof(MidpointRounding)}, typeof(decimal)),

                            new StiParserMethodInfo(StiFunctionType.Sign, 1, new Type[] {typeof(long)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.Sign, 2, new Type[] {typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.Sign, 3, new Type[] {typeof(decimal)}, typeof(decimal)),

                            new StiParserMethodInfo(StiFunctionType.Sqrt, 1, new Type[] {typeof(double)}, typeof(double)),

                            new StiParserMethodInfo(StiFunctionType.Truncate, 1, new Type[] {typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.Truncate, 2, new Type[] {typeof(decimal)}, typeof(decimal)),
                            #endregion

                            #region Print state
                            new StiParserMethodInfo(StiFunctionType.IsNull, 1, new Type[] {typeof(object), typeof(string)}, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.Next, 1, new Type[] {typeof(object), typeof(string)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.NextIsNull, 1, new Type[] {typeof(object), typeof(string)}, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.Previous, 1, new Type[] {typeof(object), typeof(string)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.PreviousIsNull, 1, new Type[] {typeof(object), typeof(string)}, typeof(bool)),
                            #endregion

                            #region Programming Shortcut
                            new StiParserMethodInfo(StiFunctionType.IIF, 1, new Type[] {typeof(bool), typeof(int), typeof(int)}, typeof(int)),
                            new StiParserMethodInfo(StiFunctionType.IIF, 2, new Type[] {typeof(bool), typeof(long), typeof(long) }, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.IIF, 3, new Type[] {typeof(bool), typeof(double), typeof(double) }, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.IIF, 4, new Type[] {typeof(bool), typeof(decimal), typeof(decimal) }, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.IIF, 5, new Type[] {typeof(bool), typeof(char), typeof(char) }, typeof(char)),
                            new StiParserMethodInfo(StiFunctionType.IIF, 6, new Type[] {typeof(bool), typeof(string), typeof(string) }, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.IIF, 7, new Type[] {typeof(bool), typeof(DateTime), typeof(DateTime) }, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.IIF, 8, new Type[] {typeof(bool), typeof(object), typeof(object)}, typeof(object)),

                            new StiParserMethodInfo(StiFunctionType.Choose, 1, new Type[] {typeof(int), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Choose, 2, new Type[] {typeof(int), typeof(object), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Choose, 3, new Type[] {typeof(int), typeof(object), typeof(object), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Choose, 4, new Type[] {typeof(int), typeof(object), typeof(object), typeof(object), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Choose, 5, new Type[] {typeof(int), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Choose, 6, new Type[] {typeof(int), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Choose, 7, new Type[] {typeof(int), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Choose, 8, new Type[] {typeof(int), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Choose, 9, new Type[] {typeof(int), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Choose, 10, new Type[] {typeof(int), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Choose, 11, new Type[] {typeof(int), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Choose, 12, new Type[] {typeof(int), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Choose, 13, new Type[] {typeof(int), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Choose, 14, new Type[] {typeof(int), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object)}, typeof(object)),

                            new StiParserMethodInfo(StiFunctionType.Switch, 1, new Type[] {typeof(bool), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Switch, 2, new Type[] {typeof(bool), typeof(object), typeof(bool), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Switch, 3, new Type[] {typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Switch, 4, new Type[] {typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Switch, 5, new Type[] {typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Switch, 6, new Type[] {typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Switch, 7, new Type[] {typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Switch, 8, new Type[] {typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Switch, 9, new Type[] {typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Switch, 10, new Type[] {typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Switch, 11, new Type[] {typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Switch, 12, new Type[] {typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Switch, 13, new Type[] {typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Switch, 14, new Type[] {typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Switch, 15, new Type[] {typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Switch, 16, new Type[] {typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Switch, 17, new Type[] {typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Switch, 18, new Type[] {typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Switch, 19, new Type[] {typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Switch, 20, new Type[] {typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object), typeof(bool), typeof(object)}, typeof(object)),
                            #endregion

                            #region Strings
                            new StiParserMethodInfo(StiFunctionType.Arabic, 1, new Type[] {typeof(int)}),
                            new StiParserMethodInfo(StiFunctionType.Arabic, 2, new Type[] {typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.Persian, 1, new Type[] {typeof(int)}),
                            new StiParserMethodInfo(StiFunctionType.Persian, 2, new Type[] {typeof(string)}),

                            new StiParserMethodInfo(StiFunctionType.DateToStr, 1, new Type[] {typeof(DateTime)}),
                            new StiParserMethodInfo(StiFunctionType.DateToStr, 2, new Type[] {typeof(DateTime?)}),
                            new StiParserMethodInfo(StiFunctionType.DateToStr, 3, new Type[] {typeof(DateTime), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.DateToStr, 4, new Type[] {typeof(DateTime?), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.DateToStrPl, 1, new Type[] {typeof(DateTime), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.DateToStrRu, 1, new Type[] {typeof(DateTime)}),
                            new StiParserMethodInfo(StiFunctionType.DateToStrRu, 2, new Type[] {typeof(DateTime), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.DateToStrUa, 1, new Type[] {typeof(DateTime)}),
                            new StiParserMethodInfo(StiFunctionType.DateToStrUa, 2, new Type[] {typeof(DateTime), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.DateToStrPt, 1, new Type[] {typeof(DateTime)}),
                            new StiParserMethodInfo(StiFunctionType.DateToStrPtBr, 1, new Type[] {typeof(DateTime)}),

                            new StiParserMethodInfo(StiFunctionType.Insert, 1, new Type[] {typeof(object), typeof(int), typeof(object)}),   //string by specification, but object by code
                            new StiParserMethodInfo(StiFunctionType.Left, 1, new Type[] {typeof(object), typeof(int)}),
                            new StiParserMethodInfo(StiFunctionType.Right, 1, new Type[] {typeof(object), typeof(int)}),
                            new StiParserMethodInfo(StiFunctionType.Mid, 1, new Type[] {typeof(object), typeof(int), typeof(int)}),
                            new StiParserMethodInfo(StiFunctionType.Length, 1, new Type[] {typeof(object)}, typeof(int)),
                            new StiParserMethodInfo(StiFunctionType.Remove, 1, new Type[] {typeof(object), typeof(int)}),
                            new StiParserMethodInfo(StiFunctionType.Remove, 2, new Type[] {typeof(object), typeof(int), typeof(int)}),
                            new StiParserMethodInfo(StiFunctionType.Replace, 1, new Type[] {typeof(object), typeof(object), typeof(object)}),
                            new StiParserMethodInfo(StiFunctionType.Substring, 1, new Type[] {typeof(object), typeof(int), typeof(int)}),
                            new StiParserMethodInfo(StiFunctionType.Trim, 1, new Type[] {typeof(object)}),

                            new StiParserMethodInfo(StiFunctionType.Roman, 1, new Type[] {typeof(int)}),
                            new StiParserMethodInfo(StiFunctionType.ToOrdinal, 1, new Type[] {typeof(long)}),
                            //new StiParserMethodInfo(StiFunctionType.Abc, 1, new Type[] {typeof(int)}),     //exist in code

                            new StiParserMethodInfo(StiFunctionType.ToLowerCase, 1, new Type[] {typeof(object)}),
                            new StiParserMethodInfo(StiFunctionType.ToUpperCase, 1, new Type[] {typeof(object)}),
                            new StiParserMethodInfo(StiFunctionType.ToProperCase, 1, new Type[] {typeof(object)}),

                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWords, 1, new Type[] {typeof(long)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWords, 2, new Type[] {typeof(double)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWords, 3, new Type[] {typeof(decimal)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWords, 4, new Type[] {typeof(long), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWords, 5, new Type[] {typeof(double), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWords, 6, new Type[] {typeof(decimal), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWords, 7, new Type[] {typeof(long), typeof(bool), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWords, 8, new Type[] {typeof(double), typeof(bool), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWords, 9, new Type[] {typeof(decimal), typeof(bool), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWords, 10, new Type[] {typeof(double), typeof(bool), typeof(bool), typeof(string), typeof(string)}),

                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsEnGb, 1, new Type[] {typeof(decimal), typeof(string), typeof(int)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsEnGb, 2, new Type[] {typeof(double), typeof(string), typeof(int)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsEnIn, 1, new Type[] {typeof(string), typeof(string), typeof(decimal), typeof(int), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsEnIn, 2, new Type[] {typeof(string), typeof(string), typeof(double), typeof(int), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsEs, 1, new Type[] {typeof(decimal), typeof(string), typeof(int)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsEs, 2, new Type[] {typeof(decimal), typeof(string), typeof(int), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsEs, 3, new Type[] {typeof(double), typeof(string), typeof(int)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsEs, 4, new Type[] {typeof(double), typeof(string), typeof(int), typeof(bool)}),

                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsAr, 1, new Type[] {typeof(decimal), typeof(string), typeof(string) }),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsAr, 2, new Type[] {typeof(double), typeof(string), typeof(string) }),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsFr, 1, new Type[] {typeof(decimal), typeof(string), typeof(int)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsFr, 2, new Type[] {typeof(double), typeof(string), typeof(int)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsNl, 1, new Type[] {typeof(decimal), typeof(string), typeof(int)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsNl, 2, new Type[] {typeof(double), typeof(string), typeof(int)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsPl, 1, new Type[] {typeof(decimal), typeof(string), typeof(bool), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsPl, 2, new Type[] {typeof(double), typeof(string), typeof(bool), typeof(bool)}),

                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsPt, 1, new Type[] {typeof(decimal), typeof(bool), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsPt, 2, new Type[] {typeof(decimal), typeof(bool), typeof(bool), typeof(string), typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsPt, 3, new Type[] {typeof(decimal), typeof(bool), typeof(bool), typeof(string), typeof(int)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsPt, 4, new Type[] {typeof(double), typeof(bool), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsPt, 5, new Type[] {typeof(double), typeof(bool), typeof(bool), typeof(string), typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsPt, 6, new Type[] {typeof(double), typeof(bool), typeof(bool), typeof(string), typeof(int)}),

                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsPtBr, 1, new Type[] {typeof(decimal)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsPtBr, 2, new Type[] {typeof(decimal), typeof(bool), typeof(string), typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsPtBr, 3, new Type[] {typeof(double)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsPtBr, 4, new Type[] {typeof(double), typeof(bool), typeof(string), typeof(string)}),

                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsRu, 1, new Type[] {typeof(long)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsRu, 2, new Type[] {typeof(double)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsRu, 3, new Type[] {typeof(decimal)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsRu, 4, new Type[] {typeof(long), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsRu, 5, new Type[] {typeof(double), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsRu, 6, new Type[] {typeof(decimal), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsRu, 7, new Type[] {typeof(long), typeof(bool), typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsRu, 8, new Type[] {typeof(double), typeof(bool), typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsRu, 9, new Type[] {typeof(decimal), typeof(bool), typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsRu, 10, new Type[] {typeof(long), typeof(string), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsRu, 11, new Type[] {typeof(double), typeof(string), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsRu, 12, new Type[] {typeof(decimal), typeof(string), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsThai, 1, new Type[] {typeof(long)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsThai, 2, new Type[] {typeof(double)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsThai, 3, new Type[] {typeof(decimal)}),

                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsTr, 1, new Type[] {typeof(decimal)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsTr, 2, new Type[] {typeof(decimal), typeof(string), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsTr, 3, new Type[] {typeof(double) }),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsTr, 4, new Type[] {typeof(double), typeof(string), typeof(bool)}),

                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsUa, 1, new Type[] {typeof(long)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsUa, 2, new Type[] {typeof(double)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsUa, 3, new Type[] {typeof(decimal)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsUa, 4, new Type[] {typeof(long), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsUa, 5, new Type[] {typeof(double), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsUa, 6, new Type[] {typeof(decimal), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsUa, 7, new Type[] {typeof(long), typeof(bool), typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsUa, 8, new Type[] {typeof(double), typeof(bool), typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsUa, 9, new Type[] {typeof(decimal), typeof(bool), typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsZh, 1, new Type[] {typeof(decimal)}),
                            new StiParserMethodInfo(StiFunctionType.ToCurrencyWordsZh, 2, new Type[] {typeof(double)}),

                            new StiParserMethodInfo(StiFunctionType.ToWords, 1, new Type[] {typeof(long)}),     //проверить соответствие перегрузок
                            new StiParserMethodInfo(StiFunctionType.ToWords, 3, new Type[] {typeof(double)}),
                            new StiParserMethodInfo(StiFunctionType.ToWords, 2, new Type[] {typeof(decimal)}),
                            new StiParserMethodInfo(StiFunctionType.ToWords, 4, new Type[] {typeof(long), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToWords, 6, new Type[] {typeof(double), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToWords, 5, new Type[] {typeof(decimal), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsAr, 1, new Type[] {typeof(decimal)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsAr, 2, new Type[] {typeof(double)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsEs, 1, new Type[] {typeof(long), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsEs, 2, new Type[] {typeof(long), typeof(bool), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsEnIn, 1, new Type[] {typeof(long), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsFa, 1, new Type[] {typeof(long)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsPl, 1, new Type[] {typeof(long), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsPt, 1, new Type[] {typeof(long), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsRu, 1, new Type[] {typeof(long)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsRu, 3, new Type[] {typeof(double)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsRu, 2, new Type[] {typeof(decimal)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsRu, 4, new Type[] {typeof(long), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsRu, 6, new Type[] {typeof(double), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsRu, 5, new Type[] {typeof(decimal), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsTr, 1, new Type[] {typeof(decimal)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsTr, 2, new Type[] {typeof(double)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsUa, 1, new Type[] {typeof(long)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsUa, 3, new Type[] {typeof(double)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsUa, 2, new Type[] {typeof(decimal)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsUa, 4, new Type[] {typeof(long), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsUa, 6, new Type[] {typeof(double), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsUa, 5, new Type[] {typeof(decimal), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsZh, 1, new Type[] {typeof(decimal)}),
                            new StiParserMethodInfo(StiFunctionType.ToWordsZh, 2, new Type[] {typeof(double)}),

                            new StiParserMethodInfo(StiFunctionType.TryParseDecimal, 1, new Type[] {typeof(string)}, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.TryParseDouble, 1, new Type[] {typeof(string)}, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.TryParseLong, 1, new Type[] {typeof(string)}, typeof(bool)),

                            new StiParserMethodInfo(StiFunctionType.StrToDateTime, 1, new Type[] {typeof(string)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.StrToNullableDateTime, 1, new Type[] {typeof(string)}, typeof(DateTime?)),

                            new StiParserMethodInfo(StiFunctionType.Format, 1, new Type[] {typeof(string), typeof(object)}),
                            new StiParserMethodInfo(StiFunctionType.Format, 2, new Type[] {typeof(string), typeof(object), typeof(object)}),
                            new StiParserMethodInfo(StiFunctionType.Format, 3, new Type[] {typeof(string), typeof(object), typeof(object), typeof(object)}),
                            new StiParserMethodInfo(StiFunctionType.Format, 4, new Type[] {typeof(string), typeof(object), typeof(object), typeof(object), typeof(object)}),
                            new StiParserMethodInfo(StiFunctionType.Format, 5, new Type[] {typeof(string), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object)}),
                            new StiParserMethodInfo(StiFunctionType.Format, 6, new Type[] {typeof(string), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object)}),
                            new StiParserMethodInfo(StiFunctionType.Format, 7, new Type[] {typeof(string), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object)}),

                            new StiParserMethodInfo(StiFunctionType.ConvertToBase64String, 1, new Type[] {typeof(string)}, typeof(string)),
                            #endregion

                            #region Aggregate functions

                            new StiParserMethodInfo(StiFunctionType.Count,   1, new Type[] {typeof(object)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.CountDistinct, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Avg,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.AvgD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.AvgDate, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.AvgI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.AvgTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.Max,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.MaxD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.MaxDate, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.MaxI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.MaxStr,  1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.MaxTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.Median,  1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.MedianD, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.MedianI, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Min,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.MinD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.MinDate, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.MinI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.MinStr,  1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.MinTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.Mode,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.ModeD,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.ModeI,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.Sum,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.SumD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.SumDistinct, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.SumI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.SumTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.First,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.Last,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(object)),

                            new StiParserMethodInfo(StiFunctionType.pCount,   1, new Type[] {typeof(object)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.pCountDistinct, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.pAvg,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.pAvgD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.pAvgDate, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.pAvgI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.pAvgTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.pMax,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.pMaxD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.pMaxDate, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.pMaxI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.pMaxStr,  1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.pMaxTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.pMedian,  1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.pMedianD, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.pMedianI, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.pMin,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.pMinD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.pMinDate, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.pMinI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.pMinStr,  1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.pMinTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.pMode,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.pModeD,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.pModeI,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.pSum,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.pSumD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.pSumDistinct, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.pSumI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.pSumTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.pFirst,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.pLast,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(object)),

                            new StiParserMethodInfo(StiFunctionType.prCount,   1, new Type[] {typeof(object)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.prCountDistinct, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.prAvg,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.prAvgD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.prAvgDate, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.prAvgI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.prAvgTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.prMax,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.prMaxD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.prMaxDate, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.prMaxI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.prMaxStr,  1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.prMaxTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.prMedian,  1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.prMedianD, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.prMedianI, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.prMin,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.prMinD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.prMinDate, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.prMinI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.prMinStr,  1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.prMinTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.prMode,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.prModeD,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.prModeI,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.prSum,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.prSumD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.prSumDistinct, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.prSumI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.prSumTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.prFirst,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.prLast,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(object)),


                            new StiParserMethodInfo(StiFunctionType.iCount,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.iCountDistinct, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.iAvg,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.iAvgD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.iAvgDate, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.iAvgI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.iAvgTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.iMax,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.iMaxD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.iMaxDate, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.iMaxI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.iMaxStr,  1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.iMaxTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.iMedian,  1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.iMedianD, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.iMedianI, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.iMin,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.iMinD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.iMinDate, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.iMinI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.iMinStr,  1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.iMinTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.iMode,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.iModeD,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.iModeI,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.iSum,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.iSumD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.iSumDistinct, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.iSumI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.iSumTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.iFirst,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.iLast,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(object)),

                            new StiParserMethodInfo(StiFunctionType.piCount,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.piCountDistinct, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.piAvg,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.piAvgD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.piAvgDate, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.piAvgI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.piAvgTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.piMax,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.piMaxD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.piMaxDate, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.piMaxI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.piMaxStr,  1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.piMaxTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.piMedian,  1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.piMedianD, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.piMedianI, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.piMin,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.piMinD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.piMinDate, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.piMinI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.piMinStr,  1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.piMinTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.piMode,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.piModeD,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.piModeI,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.piSum,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.piSumD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.piSumDistinct, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.piSumI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.piSumTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.piFirst,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.piLast,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(object)),

                            new StiParserMethodInfo(StiFunctionType.priCount,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.priCountDistinct, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.priAvg,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.priAvgD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.priAvgDate, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.priAvgI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.priAvgTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.priMax,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.priMaxD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.priMaxDate, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.priMaxI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.priMaxStr,  1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.priMaxTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.priMedian,  1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.priMedianD, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.priMedianI, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.priMin,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.priMinD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.priMinDate, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.priMinI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.priMinStr,  1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.priMinTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.priMode,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.priModeD,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.priModeI,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.priSum,     1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.priSumD,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.priSumDistinct, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.priSumI,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.priSumTime, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.priFirst,   1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.priLast,    1, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(List<StiAsmCommand>)}, typeof(object)),

                            new StiParserMethodInfo(StiFunctionType.Rank, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}),
                            new StiParserMethodInfo(StiFunctionType.Rank, 2, new Type[] {typeof(object), typeof(List<StiAsmCommand>), typeof(bool), typeof(StiRankOrder)}),
                            #endregion

                            #region Totals Hierarchical
                            new StiParserMethodInfo(StiFunctionType.CountAllLevels, 1, new Type[] {typeof(object)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.CountAllLevelsOnlyChilds, 1, new Type[] {typeof(object)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.CountOnlyChilds, 1, new Type[] {typeof(object)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.CountDistinctAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.CountDistinctAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.CountDistinctOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),

                            new StiParserMethodInfo(StiFunctionType.SumAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.SumAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.SumOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.SumDAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.SumDAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.SumDOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.SumIAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.SumIAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.SumIOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.SumTimeAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.SumTimeAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.SumTimeOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),

                            new StiParserMethodInfo(StiFunctionType.AvgAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.AvgAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.AvgOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.AvgDAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.AvgDAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.AvgDOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.AvgIAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.AvgIAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.AvgIOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.AvgDateAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.AvgDateAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.AvgDateOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.AvgTimeAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.AvgTimeAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.AvgTimeOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),

                            new StiParserMethodInfo(StiFunctionType.MaxAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.MaxAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.MaxOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.MaxDAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.MaxDAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.MaxDOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.MaxIAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.MaxIAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.MaxIOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),

                            new StiParserMethodInfo(StiFunctionType.MinAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.MinAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.MinOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.MinDAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.MinDAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.MinDOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.MinIAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.MinIAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.MinIOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),

                            new StiParserMethodInfo(StiFunctionType.MedianAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.MedianAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.MedianOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.MedianDAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.MedianDAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.MedianDOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.MedianIAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.MedianIAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.MedianIOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),

                            new StiParserMethodInfo(StiFunctionType.ModeAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.ModeAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.ModeOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.ModeDAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.ModeDAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.ModeDOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.ModeIAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.ModeIAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.ModeIOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(Int64)),

                            new StiParserMethodInfo(StiFunctionType.FirstAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.FirstAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.FirstOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.LastAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.LastAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(object)),
                            new StiParserMethodInfo(StiFunctionType.LastOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(object)),

                            new StiParserMethodInfo(StiFunctionType.MinDateAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.MinDateAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.MinDateOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.MinTimeAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.MinTimeAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.MinTimeOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.MinStrAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.MinStrAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.MinStrOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(string)),

                            new StiParserMethodInfo(StiFunctionType.MaxDateAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.MaxDateAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.MaxDateOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.MaxTimeAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.MaxTimeAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.MaxTimeOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.MaxStrAllLevels, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.MaxStrAllLevelsOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.MaxStrOnlyChilds, 1, new Type[] {typeof(object), typeof(List<StiAsmCommand>)}, typeof(string)),
                            #endregion

                            #region Others
                            new StiParserMethodInfo(StiFunctionType.MathRound, 1, new Type[] {typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.MathRound, 2, new Type[] {typeof(double), typeof(MidpointRounding)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.MathRound, 3, new Type[] {typeof(double), typeof(int)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.MathRound, 4, new Type[] {typeof(double), typeof(int), typeof(MidpointRounding)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.MathRound, 5, new Type[] {typeof(decimal)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.MathRound, 6, new Type[] {typeof(decimal), typeof(MidpointRounding)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.MathRound, 7, new Type[] {typeof(decimal), typeof(int)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.MathRound, 8, new Type[] {typeof(decimal), typeof(int), typeof(MidpointRounding)}, typeof(decimal)),

                            new StiParserMethodInfo(StiFunctionType.MathPow, 1, new Type[] {typeof(double), typeof(double)}, typeof(double)),

                            new StiParserMethodInfo(StiFunctionType.GetAnchorPageNumber, 1, new Type[] {typeof(object)}, typeof(int)),
                            new StiParserMethodInfo(StiFunctionType.GetAnchorPageNumberThrough, 1, new Type[] {typeof(object)}, typeof(int)),

                            new StiParserMethodInfo(StiFunctionType.ParseTimeSpan, 1, new Type[] {typeof(string)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.ParseDateTime, 1, new Type[] {typeof(string)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.ParseDateTimeOffset, 1, new Type[] {typeof(string)}, typeof(DateTimeOffset)),
                            new StiParserMethodInfo(StiFunctionType.ParseDecimal, 1, new Type[] {typeof(string)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.ParseDouble, 1, new Type[] {typeof(string)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.ParseInt, 1, new Type[] {typeof(string)}, typeof(int)),
                            new StiParserMethodInfo(StiFunctionType.ParseInt64, 1, new Type[] {typeof(string)}, typeof(Int64)),

                            new StiParserMethodInfo(StiFunctionType.ToString, 1, new Type[] {typeof(object)}),
                            new StiParserMethodInfo(StiFunctionType.StringIsNullOrEmpty, 1, new Type[] {typeof(string)}, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.StringIsNullOrWhiteSpace, 1, new Type[] {typeof(string)}, typeof(bool)),

                            new StiParserMethodInfo(StiFunctionType.EngineHelperJoinColumnContent, 1, new Type[] {typeof(StiDataSource), typeof(string), typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.EngineHelperJoinColumnContent, 2, new Type[] {typeof(StiBusinessObject), typeof(string), typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.EngineHelperJoinColumnContent, 3, new Type[] {typeof(StiDataSource), typeof(string), typeof(string), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.EngineHelperJoinColumnContent, 4, new Type[] {typeof(StiBusinessObject), typeof(string), typeof(string), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.EngineHelperToQueryString, 1, new Type[] {typeof(IList), typeof(string), typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.EngineHelperToQueryString, 2, new Type[] {typeof(IList), typeof(string), typeof(string), typeof(bool)}),
                            new StiParserMethodInfo(StiFunctionType.EngineHelperGetRealPageNumber, 1, new Type[] {typeof(object)}, typeof(int)),

                            new StiParserMethodInfo(StiFunctionType.SystemConvertToBoolean, 1, new Type[] {typeof(object)}, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.SystemConvertToByte, 2, new Type[] {typeof(object)}, typeof(byte)),
                            new StiParserMethodInfo(StiFunctionType.SystemConvertToChar, 3, new Type[] {typeof(object)}, typeof(char)),
                            new StiParserMethodInfo(StiFunctionType.SystemConvertToDateTime, 4, new Type[] {typeof(object)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.SystemConvertToDecimal, 5, new Type[] {typeof(object)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.SystemConvertToDouble, 6, new Type[] {typeof(object)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.SystemConvertToInt16, 7, new Type[] {typeof(object)}, typeof(Int16)),
                            new StiParserMethodInfo(StiFunctionType.SystemConvertToInt32, 8, new Type[] {typeof(object)}, typeof(Int32)),
                            new StiParserMethodInfo(StiFunctionType.SystemConvertToInt64, 9, new Type[] {typeof(object)}, typeof(Int64)),
                            new StiParserMethodInfo(StiFunctionType.SystemConvertToSByte, 10, new Type[] {typeof(object)}, typeof(sbyte)),
                            new StiParserMethodInfo(StiFunctionType.SystemConvertToSingle, 11, new Type[] {typeof(object)}, typeof(Single)),
                            new StiParserMethodInfo(StiFunctionType.SystemConvertToString, 12, new Type[] {typeof(object)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.SystemConvertToUInt16, 13, new Type[] {typeof(object)}, typeof(UInt16)),
                            new StiParserMethodInfo(StiFunctionType.SystemConvertToUInt32, 14, new Type[] {typeof(object)}, typeof(UInt32)),
                            new StiParserMethodInfo(StiFunctionType.SystemConvertToUInt64, 15, new Type[] {typeof(object)}, typeof(UInt64)),

                            new StiParserMethodInfo(StiFunctionType.StiNullValuesHelperIsNull, 1, new Type[] {typeof(StiReport), typeof(string)}, typeof(bool)),

                            new StiParserMethodInfo(StiFunctionType.TimeSpanFromDays, 1, new Type[] {typeof(double)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.TimeSpanFromHours, 1, new Type[] {typeof(double)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.TimeSpanFromMilliseconds, 1, new Type[] {typeof(double)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.TimeSpanFromMinutes, 1, new Type[] {typeof(double)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.TimeSpanFromSeconds, 1, new Type[] {typeof(double)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.TimeSpanFromTicks, 1, new Type[] {typeof(Int64)}, typeof(TimeSpan)),

                            new StiParserMethodInfo(StiFunctionType.NewType, 1, new Type[] {typeof(DateTime)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.NewType, 2, new Type[] {typeof(DateTime), typeof(Int64)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.NewType, 3, new Type[] {typeof(DateTime), typeof(Int32), typeof(Int32), typeof(Int32)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.NewType, 4, new Type[] {typeof(DateTime), typeof(Int32), typeof(Int32), typeof(Int32), typeof(Int32), typeof(Int32), typeof(Int32) }, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.NewType, 5, new Type[] {typeof(DateTime), typeof(Int32), typeof(Int32), typeof(Int32), typeof(Int32), typeof(Int32), typeof(Int32), typeof(Int32) }, typeof(DateTime)),

                            new StiParserMethodInfo(StiFunctionType.ImageFromFile, 1, new Type[] {typeof(string)}, typeof(Image)),
                            #endregion

                            #region Methods
                            new StiParserMethodInfo(StiFunctionType.m_ToShortDateString, 1, new Type[] {typeof(DateTime)}),
                            new StiParserMethodInfo(StiFunctionType.m_ToShortTimeString, 1, new Type[] {typeof(DateTime)}),
                            new StiParserMethodInfo(StiFunctionType.m_ToLongDateString, 1, new Type[] {typeof(DateTime)}),
                            new StiParserMethodInfo(StiFunctionType.m_ToLongTimeString, 1, new Type[] {typeof(DateTime)}),

                            new StiParserMethodInfo(StiFunctionType.m_Substring, 1, new Type[] {typeof(string), typeof(int) }),
                            new StiParserMethodInfo(StiFunctionType.m_Substring, 2, new Type[] {typeof(string), typeof(int), typeof(int) }),
                            new StiParserMethodInfo(StiFunctionType.m_ToLower, 1, new Type[] {typeof(string) }),
                            new StiParserMethodInfo(StiFunctionType.m_ToUpper, 1, new Type[] {typeof(string) }),
                            new StiParserMethodInfo(StiFunctionType.m_IndexOf, 1, new Type[] {typeof(string), typeof(string) }, typeof(int)),
                            new StiParserMethodInfo(StiFunctionType.m_StartsWith, 1, new Type[] {typeof(string), typeof(string) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_EndsWith, 1, new Type[] {typeof(string), typeof(string) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Replace, 1, new Type[] {typeof(string), typeof(char), typeof(char) }),
                            new StiParserMethodInfo(StiFunctionType.m_Replace, 2, new Type[] {typeof(string), typeof(string), typeof(string) }),
                            new StiParserMethodInfo(StiFunctionType.m_PadLeft, 1, new Type[] {typeof(string), typeof(int) }),
                            new StiParserMethodInfo(StiFunctionType.m_PadLeft, 2, new Type[] {typeof(string), typeof(int), typeof(char) }),
                            new StiParserMethodInfo(StiFunctionType.m_PadRight, 1, new Type[] {typeof(string), typeof(int) }),
                            new StiParserMethodInfo(StiFunctionType.m_PadRight, 2, new Type[] {typeof(string), typeof(int), typeof(char) }),
                            new StiParserMethodInfo(StiFunctionType.m_TrimStart, 1, new Type[] {typeof(string)}),
                            new StiParserMethodInfo(StiFunctionType.m_TrimEnd, 1, new Type[] {typeof(string)}),

                            new StiParserMethodInfo(StiFunctionType.m_Contains, 1, new Type[] {typeof(string), typeof(string) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 210, new Type[] {typeof(BoolList), typeof(bool) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 220, new Type[] {typeof(CharList), typeof(char) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 230, new Type[] {typeof(DateTimeList), typeof(DateTime) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 231, new Type[] {typeof(DateTimeList), typeof(string) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 240, new Type[] {typeof(TimeSpanList), typeof(TimeSpan) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 241, new Type[] {typeof(TimeSpanList), typeof(string) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 250, new Type[] {typeof(DecimalList), typeof(decimal) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 251, new Type[] {typeof(DecimalList), typeof(Int64) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 252, new Type[] {typeof(DecimalList), typeof(string) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 260, new Type[] {typeof(FloatList), typeof(float) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 261, new Type[] {typeof(FloatList), typeof(Int64) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 262, new Type[] {typeof(FloatList), typeof(string) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 270, new Type[] {typeof(DoubleList), typeof(double) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 271, new Type[] {typeof(DoubleList), typeof(Int64) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 272, new Type[] {typeof(DoubleList), typeof(string) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 280, new Type[] {typeof(ByteList), typeof(byte) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 282, new Type[] {typeof(ByteList), typeof(decimal) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 283, new Type[] {typeof(ByteList), typeof(string) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 290, new Type[] {typeof(ShortList), typeof(short) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 292, new Type[] {typeof(ShortList), typeof(decimal) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 293, new Type[] {typeof(ShortList), typeof(string) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 300, new Type[] {typeof(IntList), typeof(int) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 302, new Type[] {typeof(IntList), typeof(decimal) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 303, new Type[] {typeof(IntList), typeof(string) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 310, new Type[] {typeof(LongList), typeof(long) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 312, new Type[] {typeof(LongList), typeof(decimal) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 313, new Type[] {typeof(LongList), typeof(string) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 320, new Type[] {typeof(GuidList), typeof(Guid) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 330, new Type[] {typeof(StringList), typeof(string) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 331, new Type[] {typeof(StringList), typeof(Int64) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 332, new Type[] {typeof(StringList), typeof(decimal) }, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.m_Contains, 333, new Type[] {typeof(StringList), typeof(double) }, typeof(bool)),

                            #endregion

                            #region Operators
                            new StiParserMethodInfo(StiFunctionType.op_Add, 1, new Type[] {typeof(int), typeof(int)}, typeof(int)),
                            new StiParserMethodInfo(StiFunctionType.op_Add, 2, new Type[] {typeof(uint), typeof(uint)}, typeof(uint)),
                            new StiParserMethodInfo(StiFunctionType.op_Add, 3, new Type[] {typeof(long), typeof(long)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.op_Add, 4, new Type[] {typeof(ulong), typeof(ulong)}, typeof(ulong)),
                            new StiParserMethodInfo(StiFunctionType.op_Add, 5, new Type[] {typeof(float), typeof(float)}, typeof(float)),
                            new StiParserMethodInfo(StiFunctionType.op_Add, 6, new Type[] {typeof(double), typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.op_Add, 7, new Type[] {typeof(decimal), typeof(decimal)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.op_Add, 8, new Type[] {typeof(string), typeof(string)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.op_Add, 9, new Type[] {typeof(string), typeof(object)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.op_Add, 10, new Type[] {typeof(object), typeof(string)}, typeof(string)),
                            new StiParserMethodInfo(StiFunctionType.op_Add, 11, new Type[] {typeof(DateTime), typeof(TimeSpan)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.op_Add, 12, new Type[] {typeof(TimeSpan), typeof(DateTime)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.op_Add, 13, new Type[] {typeof(DateTimeOffset), typeof(TimeSpan)}, typeof(DateTimeOffset)),
                            new StiParserMethodInfo(StiFunctionType.op_Add, 14, new Type[] {typeof(TimeSpan), typeof(DateTimeOffset)}, typeof(DateTimeOffset)),

                            new StiParserMethodInfo(StiFunctionType.op_Sub, 1, new Type[] {typeof(int), typeof(int)}, typeof(int)),
                            new StiParserMethodInfo(StiFunctionType.op_Sub, 2, new Type[] {typeof(uint), typeof(uint)}, typeof(uint)),
                            new StiParserMethodInfo(StiFunctionType.op_Sub, 3, new Type[] {typeof(long), typeof(long)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.op_Sub, 4, new Type[] {typeof(ulong), typeof(ulong)}, typeof(ulong)),
                            new StiParserMethodInfo(StiFunctionType.op_Sub, 5, new Type[] {typeof(float), typeof(float)}, typeof(float)),
                            new StiParserMethodInfo(StiFunctionType.op_Sub, 6, new Type[] {typeof(double), typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.op_Sub, 7, new Type[] {typeof(decimal), typeof(decimal)}, typeof(decimal)),
                            new StiParserMethodInfo(StiFunctionType.op_Sub, 8, new Type[] {typeof(DateTime), typeof(DateTime)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.op_Sub, 9, new Type[] {typeof(DateTime), typeof(TimeSpan)}, typeof(DateTime)),
                            new StiParserMethodInfo(StiFunctionType.op_Sub, 10, new Type[] {typeof(DateTimeOffset), typeof(DateTimeOffset)}, typeof(TimeSpan)),
                            new StiParserMethodInfo(StiFunctionType.op_Sub, 11, new Type[] {typeof(DateTimeOffset), typeof(TimeSpan)}, typeof(DateTimeOffset)),

                            new StiParserMethodInfo(StiFunctionType.op_Mult, 1, new Type[] {typeof(int), typeof(int)}, typeof(int)),
                            new StiParserMethodInfo(StiFunctionType.op_Mult, 2, new Type[] {typeof(uint), typeof(uint)}, typeof(uint)),
                            new StiParserMethodInfo(StiFunctionType.op_Mult, 3, new Type[] {typeof(long), typeof(long)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.op_Mult, 4, new Type[] {typeof(ulong), typeof(ulong)}, typeof(ulong)),
                            new StiParserMethodInfo(StiFunctionType.op_Mult, 5, new Type[] {typeof(float), typeof(float)}, typeof(float)),
                            new StiParserMethodInfo(StiFunctionType.op_Mult, 6, new Type[] {typeof(double), typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.op_Mult, 7, new Type[] {typeof(decimal), typeof(decimal)}, typeof(decimal)),

                            new StiParserMethodInfo(StiFunctionType.op_Div, 1, new Type[] {typeof(int), typeof(int)}, typeof(int)),
                            new StiParserMethodInfo(StiFunctionType.op_Div, 2, new Type[] {typeof(uint), typeof(uint)}, typeof(uint)),
                            new StiParserMethodInfo(StiFunctionType.op_Div, 3, new Type[] {typeof(long), typeof(long)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.op_Div, 4, new Type[] {typeof(ulong), typeof(ulong)}, typeof(ulong)),
                            new StiParserMethodInfo(StiFunctionType.op_Div, 5, new Type[] {typeof(float), typeof(float)}, typeof(float)),
                            new StiParserMethodInfo(StiFunctionType.op_Div, 6, new Type[] {typeof(double), typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.op_Div, 7, new Type[] {typeof(decimal), typeof(decimal)}, typeof(decimal)),

                            new StiParserMethodInfo(StiFunctionType.op_Mod, 1, new Type[] {typeof(int), typeof(int)}, typeof(int)),
                            new StiParserMethodInfo(StiFunctionType.op_Mod, 2, new Type[] {typeof(uint), typeof(uint)}, typeof(uint)),
                            new StiParserMethodInfo(StiFunctionType.op_Mod, 3, new Type[] {typeof(long), typeof(long)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.op_Mod, 4, new Type[] {typeof(ulong), typeof(ulong)}, typeof(ulong)),
                            new StiParserMethodInfo(StiFunctionType.op_Mod, 5, new Type[] {typeof(float), typeof(float)}, typeof(float)),
                            new StiParserMethodInfo(StiFunctionType.op_Mod, 6, new Type[] {typeof(double), typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.op_Mod, 7, new Type[] {typeof(decimal), typeof(decimal)}, typeof(decimal)),

                            new StiParserMethodInfo(StiFunctionType.op_Shl, 1, new Type[] {typeof(int), typeof(int)}, typeof(int)),
                            new StiParserMethodInfo(StiFunctionType.op_Shl, 2, new Type[] {typeof(uint), typeof(int)}, typeof(uint)),
                            new StiParserMethodInfo(StiFunctionType.op_Shl, 3, new Type[] {typeof(long), typeof(int)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.op_Shl, 4, new Type[] {typeof(ulong), typeof(int)}, typeof(ulong)),

                            new StiParserMethodInfo(StiFunctionType.op_Shr, 1, new Type[] {typeof(int), typeof(int)}, typeof(int)),
                            new StiParserMethodInfo(StiFunctionType.op_Shr, 2, new Type[] {typeof(uint), typeof(int)}, typeof(uint)),
                            new StiParserMethodInfo(StiFunctionType.op_Shr, 3, new Type[] {typeof(long), typeof(int)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.op_Shr, 4, new Type[] {typeof(ulong), typeof(int)}, typeof(ulong)),

                            new StiParserMethodInfo(StiFunctionType.op_And, 1, new Type[] {typeof(int), typeof(int)}, typeof(int)),
                            new StiParserMethodInfo(StiFunctionType.op_And, 2, new Type[] {typeof(uint), typeof(uint)}, typeof(uint)),
                            new StiParserMethodInfo(StiFunctionType.op_And, 3, new Type[] {typeof(long), typeof(long)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.op_And, 4, new Type[] {typeof(ulong), typeof(ulong)}, typeof(ulong)),
                            new StiParserMethodInfo(StiFunctionType.op_And, 5, new Type[] {typeof(bool), typeof(bool)}, typeof(bool)),

                            new StiParserMethodInfo(StiFunctionType.op_Or, 1, new Type[] {typeof(int), typeof(int)}, typeof(int)),
                            new StiParserMethodInfo(StiFunctionType.op_Or, 2, new Type[] {typeof(uint), typeof(uint)}, typeof(uint)),
                            new StiParserMethodInfo(StiFunctionType.op_Or, 3, new Type[] {typeof(long), typeof(long)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.op_Or, 4, new Type[] {typeof(ulong), typeof(ulong)}, typeof(ulong)),
                            new StiParserMethodInfo(StiFunctionType.op_Or, 5, new Type[] {typeof(bool), typeof(bool)}, typeof(bool)),

                            new StiParserMethodInfo(StiFunctionType.op_Xor, 1, new Type[] {typeof(int), typeof(int)}, typeof(int)),
                            new StiParserMethodInfo(StiFunctionType.op_Xor, 2, new Type[] {typeof(uint), typeof(uint)}, typeof(uint)),
                            new StiParserMethodInfo(StiFunctionType.op_Xor, 3, new Type[] {typeof(long), typeof(long)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.op_Xor, 4, new Type[] {typeof(ulong), typeof(ulong)}, typeof(ulong)),
                            new StiParserMethodInfo(StiFunctionType.op_Xor, 5, new Type[] {typeof(bool), typeof(bool)}, typeof(bool)),

                            new StiParserMethodInfo(StiFunctionType.op_And2, 1, new Type[] {typeof(bool), typeof(bool)}, typeof(bool)),
                            new StiParserMethodInfo(StiFunctionType.op_Or2, 1, new Type[] {typeof(bool), typeof(bool)}, typeof(bool)),

                            new StiParserMethodInfo(StiFunctionType.op_Neg, 1, new Type[] {typeof(int)}, typeof(int)),
                            new StiParserMethodInfo(StiFunctionType.op_Neg, 2, new Type[] {typeof(long)}, typeof(long)),
                            new StiParserMethodInfo(StiFunctionType.op_Neg, 3, new Type[] {typeof(float)}, typeof(float)),
                            new StiParserMethodInfo(StiFunctionType.op_Neg, 4, new Type[] {typeof(double)}, typeof(double)),
                            new StiParserMethodInfo(StiFunctionType.op_Neg, 5, new Type[] {typeof(decimal)}, typeof(decimal)),

                            new StiParserMethodInfo(StiFunctionType.op_Not, 1, new Type[] {typeof(bool)}, typeof(bool)),
                            #endregion
                        };

                        methodsHash = new Hashtable();
                        foreach (StiParserMethodInfo methodInfo in methods)
                        {
                            List<StiParserMethodInfo> list = (List<StiParserMethodInfo>)methodsHash[methodInfo.Name];
                            if (list == null)
                            {
                                list = new List<StiParserMethodInfo>();
                                methodsHash[methodInfo.Name] = list;
                            }
                            list.Add(methodInfo);
                        }
                    }
                }
                return methodsHash;
            }
        }
        #endregion

        #region ConstantsList
        private static Hashtable constantsList = null;
        private static Hashtable constantsList_low = null;
        private Hashtable ConstantsList
        {
            get
            {
                if (constantsList == null)
                {
                    constantsList = new Hashtable();
                    constantsList["true"] = true;
                    constantsList["True"] = true;
                    constantsList["false"] = false;
                    constantsList["False"] = false;
                    constantsList["null"] = null;
                    constantsList["DBNull"] = namespaceObj;
                    constantsList["DBNull.Value"] = DBNull.Value;

                    constantsList["string.Empty"] = string.Empty;

                    constantsList["MidpointRounding"] = namespaceObj;
                    constantsList["MidpointRounding.ToEven"] = MidpointRounding.ToEven;
                    constantsList["MidpointRounding.AwayFromZero"] = MidpointRounding.AwayFromZero;

                    constantsList["StiRankOrder"] = namespaceObj;
                    constantsList["StiRankOrder.Asc"] = StiRankOrder.Asc;
                    constantsList["StiRankOrder.Desc"] = StiRankOrder.Desc;

                    constantsList["DayOfWeek"] = namespaceObj;
                    constantsList["DayOfWeek.Sunday"] = DayOfWeek.Sunday;
                    constantsList["DayOfWeek.Monday"] = DayOfWeek.Monday;
                    constantsList["DayOfWeek.Tuesday"] = DayOfWeek.Tuesday;
                    constantsList["DayOfWeek.Wednesday"] = DayOfWeek.Wednesday;
                    constantsList["DayOfWeek.Thursday"] = DayOfWeek.Thursday;
                    constantsList["DayOfWeek.Friday"] = DayOfWeek.Friday;
                    constantsList["DayOfWeek.Saturday"] = DayOfWeek.Saturday;

                    constantsList["CalendarWeekRule"] = namespaceObj;
                    constantsList["CalendarWeekRule.FirstDay"] = CalendarWeekRule.FirstDay;
                    constantsList["CalendarWeekRule.FirstFullWeek"] = CalendarWeekRule.FirstFullWeek;
                    constantsList["CalendarWeekRule.FirstFourDayWeek"] = CalendarWeekRule.FirstFourDayWeek;
                    constantsList["System.Globalization.CalendarWeekRule"] = namespaceObj;
                    constantsList["System.Globalization.CalendarWeekRule.FirstDay"] = CalendarWeekRule.FirstDay;
                    constantsList["System.Globalization.CalendarWeekRule.FirstFullWeek"] = CalendarWeekRule.FirstFullWeek;
                    constantsList["System.Globalization.CalendarWeekRule.FirstFourDayWeek"] = CalendarWeekRule.FirstFourDayWeek;

                    #region Color
                    constantsList["Color.AliceBlue"] = Color.AliceBlue;
                    constantsList["Color.AntiqueWhite"] = Color.AntiqueWhite;
                    constantsList["Color.Aqua"] = Color.Aqua;
                    constantsList["Color.Aquamarine"] = Color.Aquamarine;
                    constantsList["Color.Azure"] = Color.Azure;
                    constantsList["Color.Beige"] = Color.Beige;
                    constantsList["Color.Bisque"] = Color.Bisque;
                    constantsList["Color.Black"] = Color.Black;
                    constantsList["Color.BlanchedAlmond"] = Color.BlanchedAlmond;
                    constantsList["Color.Blue"] = Color.Blue;
                    constantsList["Color.BlueViolet"] = Color.BlueViolet;
                    constantsList["Color.Brown"] = Color.Brown;
                    constantsList["Color.BurlyWood"] = Color.BurlyWood;
                    constantsList["Color.CadetBlue"] = Color.CadetBlue;
                    constantsList["Color.Chartreuse"] = Color.Chartreuse;
                    constantsList["Color.Chocolate"] = Color.Chocolate;
                    constantsList["Color.Coral"] = Color.Coral;
                    constantsList["Color.CornflowerBlue"] = Color.CornflowerBlue;
                    constantsList["Color.Cornsilk"] = Color.Cornsilk;
                    constantsList["Color.Crimson"] = Color.Crimson;
                    constantsList["Color.Cyan"] = Color.Cyan;
                    constantsList["Color.DarkBlue"] = Color.DarkBlue;
                    constantsList["Color.DarkCyan"] = Color.DarkCyan;
                    constantsList["Color.DarkGoldenrod"] = Color.DarkGoldenrod;
                    constantsList["Color.DarkGray"] = Color.DarkGray;
                    constantsList["Color.DarkGreen"] = Color.DarkGreen;
                    constantsList["Color.DarkKhaki"] = Color.DarkKhaki;
                    constantsList["Color.DarkMagenta"] = Color.DarkMagenta;
                    constantsList["Color.DarkOliveGreen"] = Color.DarkOliveGreen;
                    constantsList["Color.DarkOrange"] = Color.DarkOrange;
                    constantsList["Color.DarkOrchid"] = Color.DarkOrchid;
                    constantsList["Color.DarkRed"] = Color.DarkRed;
                    constantsList["Color.DarkSalmon"] = Color.DarkSalmon;
                    constantsList["Color.DarkSeaGreen"] = Color.DarkSeaGreen;
                    constantsList["Color.DarkSlateBlue"] = Color.DarkSlateBlue;
                    constantsList["Color.DarkSlateGray"] = Color.DarkSlateGray;
                    constantsList["Color.DarkTurquoise"] = Color.DarkTurquoise;
                    constantsList["Color.DarkViolet"] = Color.DarkViolet;
                    constantsList["Color.DeepPink"] = Color.DeepPink;
                    constantsList["Color.DeepSkyBlue"] = Color.DeepSkyBlue;
                    constantsList["Color.DimGray"] = Color.DimGray;
                    constantsList["Color.DodgerBlue"] = Color.DodgerBlue;
                    constantsList["Color.Firebrick"] = Color.Firebrick;
                    constantsList["Color.FloralWhite"] = Color.FloralWhite;
                    constantsList["Color.ForestGreen"] = Color.ForestGreen;
                    constantsList["Color.Fuchsia"] = Color.Fuchsia;
                    constantsList["Color.Gainsboro"] = Color.Gainsboro;
                    constantsList["Color.GhostWhite"] = Color.GhostWhite;
                    constantsList["Color.Gold"] = Color.Gold;
                    constantsList["Color.Goldenrod"] = Color.Goldenrod;
                    constantsList["Color.Gray"] = Color.Gray;
                    constantsList["Color.Green"] = Color.Green;
                    constantsList["Color.GreenYellow"] = Color.GreenYellow;
                    constantsList["Color.Honeydew"] = Color.Honeydew;
                    constantsList["Color.HotPink"] = Color.HotPink;
                    constantsList["Color.IndianRed"] = Color.IndianRed;
                    constantsList["Color.Indigo"] = Color.Indigo;
                    constantsList["Color.Ivory"] = Color.Ivory;
                    constantsList["Color.Khaki"] = Color.Khaki;
                    constantsList["Color.Lavender"] = Color.Lavender;
                    constantsList["Color.LavenderBlush"] = Color.LavenderBlush;
                    constantsList["Color.LawnGreen"] = Color.LawnGreen;
                    constantsList["Color.LemonChiffon"] = Color.LemonChiffon;
                    constantsList["Color.LightBlue"] = Color.LightBlue;
                    constantsList["Color.LightCoral"] = Color.LightCoral;
                    constantsList["Color.LightCyan"] = Color.LightCyan;
                    constantsList["Color.LightGoldenrodYellow"] = Color.LightGoldenrodYellow;
                    constantsList["Color.LightGray"] = Color.LightGray;
                    constantsList["Color.LightGreen"] = Color.LightGreen;
                    constantsList["Color.LightPink"] = Color.LightPink;
                    constantsList["Color.LightSalmon"] = Color.LightSalmon;
                    constantsList["Color.LightSeaGreen"] = Color.LightSeaGreen;
                    constantsList["Color.LightSkyBlue"] = Color.LightSkyBlue;
                    constantsList["Color.LightSlateGray"] = Color.LightSlateGray;
                    constantsList["Color.LightSteelBlue"] = Color.LightSteelBlue;
                    constantsList["Color.LightYellow"] = Color.LightYellow;
                    constantsList["Color.Lime"] = Color.Lime;
                    constantsList["Color.LimeGreen"] = Color.LimeGreen;
                    constantsList["Color.Linen"] = Color.Linen;
                    constantsList["Color.Magenta"] = Color.Magenta;
                    constantsList["Color.Maroon"] = Color.Maroon;
                    constantsList["Color.MediumAquamarine"] = Color.MediumAquamarine;
                    constantsList["Color.MediumBlue"] = Color.MediumBlue;
                    constantsList["Color.MediumOrchid"] = Color.MediumOrchid;
                    constantsList["Color.MediumPurple"] = Color.MediumPurple;
                    constantsList["Color.MediumSeaGreen"] = Color.MediumSeaGreen;
                    constantsList["Color.MediumSlateBlue"] = Color.MediumSlateBlue;
                    constantsList["Color.MediumSpringGreen"] = Color.MediumSpringGreen;
                    constantsList["Color.MediumTurquoise"] = Color.MediumTurquoise;
                    constantsList["Color.MediumVioletRed"] = Color.MediumVioletRed;
                    constantsList["Color.MidnightBlue"] = Color.MidnightBlue;
                    constantsList["Color.MintCream"] = Color.MintCream;
                    constantsList["Color.MistyRose"] = Color.MistyRose;
                    constantsList["Color.Moccasin"] = Color.Moccasin;
                    constantsList["Color.NavajoWhite"] = Color.NavajoWhite;
                    constantsList["Color.Navy"] = Color.Navy;
                    constantsList["Color.OldLace"] = Color.OldLace;
                    constantsList["Color.Olive"] = Color.Olive;
                    constantsList["Color.OliveDrab"] = Color.OliveDrab;
                    constantsList["Color.Orange"] = Color.Orange;
                    constantsList["Color.OrangeRed"] = Color.OrangeRed;
                    constantsList["Color.Orchid"] = Color.Orchid;
                    constantsList["Color.PaleGoldenrod"] = Color.PaleGoldenrod;
                    constantsList["Color.PaleGreen"] = Color.PaleGreen;
                    constantsList["Color.PaleTurquoise"] = Color.PaleTurquoise;
                    constantsList["Color.PaleVioletRed"] = Color.PaleVioletRed;
                    constantsList["Color.PapayaWhip"] = Color.PapayaWhip;
                    constantsList["Color.PeachPuff"] = Color.PeachPuff;
                    constantsList["Color.Peru"] = Color.Peru;
                    constantsList["Color.Pink"] = Color.Pink;
                    constantsList["Color.Plum"] = Color.Plum;
                    constantsList["Color.PowderBlue"] = Color.PowderBlue;
                    constantsList["Color.Purple"] = Color.Purple;
                    constantsList["Color.Red"] = Color.Red;
                    constantsList["Color.RosyBrown"] = Color.RosyBrown;
                    constantsList["Color.RoyalBlue"] = Color.RoyalBlue;
                    constantsList["Color.SaddleBrown"] = Color.SaddleBrown;
                    constantsList["Color.Salmon"] = Color.Salmon;
                    constantsList["Color.SandyBrown"] = Color.SandyBrown;
                    constantsList["Color.SeaGreen"] = Color.SeaGreen;
                    constantsList["Color.SeaShell"] = Color.SeaShell;
                    constantsList["Color.Sienna"] = Color.Sienna;
                    constantsList["Color.Silver"] = Color.Silver;
                    constantsList["Color.SkyBlue"] = Color.SkyBlue;
                    constantsList["Color.SlateBlue"] = Color.SlateBlue;
                    constantsList["Color.SlateGray"] = Color.SlateGray;
                    constantsList["Color.Snow"] = Color.Snow;
                    constantsList["Color.SpringGreen"] = Color.SpringGreen;
                    constantsList["Color.SteelBlue"] = Color.SteelBlue;
                    constantsList["Color.Tan"] = Color.Tan;
                    constantsList["Color.Teal"] = Color.Teal;
                    constantsList["Color.Thistle"] = Color.Thistle;
                    constantsList["Color.Tomato"] = Color.Tomato;
                    constantsList["Color.Transparent"] = Color.Transparent;
                    constantsList["Color.Turquoise"] = Color.Turquoise;
                    constantsList["Color.Violet"] = Color.Violet;
                    constantsList["Color.Wheat"] = Color.Wheat;
                    constantsList["Color.White"] = Color.White;
                    constantsList["Color.WhiteSmoke"] = Color.WhiteSmoke;
                    constantsList["Color.Yellow"] = Color.Yellow;
                    constantsList["Color.YellowGreen"] = Color.YellowGreen;
                    #endregion

                    #region HatchStyle
                    constantsList["HatchStyle.BackwardDiagonal"] = HatchStyle.BackwardDiagonal;
                    constantsList["HatchStyle.Cross"] = HatchStyle.Cross;
                    constantsList["HatchStyle.DarkDownwardDiagonal"] = HatchStyle.DarkDownwardDiagonal;
                    constantsList["HatchStyle.DarkHorizontal"] = HatchStyle.DarkHorizontal;
                    constantsList["HatchStyle.DarkUpwardDiagonal"] = HatchStyle.DarkUpwardDiagonal;
                    constantsList["HatchStyle.DarkVertical"] = HatchStyle.DarkVertical;
                    constantsList["HatchStyle.DashedDownwardDiagonal"] = HatchStyle.DashedDownwardDiagonal;
                    constantsList["HatchStyle.DashedHorizontal"] = HatchStyle.DashedHorizontal;
                    constantsList["HatchStyle.DashedUpwardDiagonal"] = HatchStyle.DashedUpwardDiagonal;
                    constantsList["HatchStyle.DashedVertical"] = HatchStyle.DashedVertical;
                    constantsList["HatchStyle.DiagonalBrick"] = HatchStyle.DiagonalBrick;
                    constantsList["HatchStyle.DiagonalCross"] = HatchStyle.DiagonalCross;
                    constantsList["HatchStyle.Divot"] = HatchStyle.Divot;
                    constantsList["HatchStyle.DottedDiamond"] = HatchStyle.DottedDiamond;
                    constantsList["HatchStyle.DottedGrid"] = HatchStyle.DottedGrid;
                    constantsList["HatchStyle.ForwardDiagonal"] = HatchStyle.ForwardDiagonal;
                    constantsList["HatchStyle.Horizontal"] = HatchStyle.Horizontal;
                    constantsList["HatchStyle.HorizontalBrick"] = HatchStyle.HorizontalBrick;
                    constantsList["HatchStyle.LargeCheckerBoard"] = HatchStyle.LargeCheckerBoard;
                    constantsList["HatchStyle.LargeConfetti"] = HatchStyle.LargeConfetti;
                    constantsList["HatchStyle.LargeGrid"] = HatchStyle.LargeGrid;
                    constantsList["HatchStyle.LightDownwardDiagonal"] = HatchStyle.LightDownwardDiagonal;
                    constantsList["HatchStyle.LightHorizontal"] = HatchStyle.LightHorizontal;
                    constantsList["HatchStyle.LightUpwardDiagonal"] = HatchStyle.LightUpwardDiagonal;
                    constantsList["HatchStyle.LightVertical"] = HatchStyle.LightVertical;
                    constantsList["HatchStyle.Max"] = HatchStyle.Max;
                    constantsList["HatchStyle.Min"] = HatchStyle.Min;
                    constantsList["HatchStyle.NarrowHorizontal"] = HatchStyle.NarrowHorizontal;
                    constantsList["HatchStyle.NarrowVertical"] = HatchStyle.NarrowVertical;
                    constantsList["HatchStyle.OutlinedDiamond"] = HatchStyle.OutlinedDiamond;
                    constantsList["HatchStyle.Percent05"] = HatchStyle.Percent05;
                    constantsList["HatchStyle.Percent10"] = HatchStyle.Percent10;
                    constantsList["HatchStyle.Percent20"] = HatchStyle.Percent20;
                    constantsList["HatchStyle.Percent25"] = HatchStyle.Percent25;
                    constantsList["HatchStyle.Percent30"] = HatchStyle.Percent30;
                    constantsList["HatchStyle.Percent40"] = HatchStyle.Percent40;
                    constantsList["HatchStyle.Percent50"] = HatchStyle.Percent50;
                    constantsList["HatchStyle.Percent60"] = HatchStyle.Percent60;
                    constantsList["HatchStyle.Percent70"] = HatchStyle.Percent70;
                    constantsList["HatchStyle.Percent75"] = HatchStyle.Percent75;
                    constantsList["HatchStyle.Percent80"] = HatchStyle.Percent80;
                    constantsList["HatchStyle.Percent90"] = HatchStyle.Percent90;
                    constantsList["HatchStyle.Plaid"] = HatchStyle.Plaid;
                    constantsList["HatchStyle.Shingle"] = HatchStyle.Shingle;
                    constantsList["HatchStyle.SmallCheckerBoard"] = HatchStyle.SmallCheckerBoard;
                    constantsList["HatchStyle.SmallConfetti"] = HatchStyle.SmallConfetti;
                    constantsList["HatchStyle.SmallGrid"] = HatchStyle.SmallGrid;
                    constantsList["HatchStyle.SolidDiamond"] = HatchStyle.SolidDiamond;
                    constantsList["HatchStyle.Sphere"] = HatchStyle.Sphere;
                    constantsList["HatchStyle.Trellis"] = HatchStyle.Trellis;
                    constantsList["HatchStyle.Vertical"] = HatchStyle.Vertical;
                    constantsList["HatchStyle.Wave"] = HatchStyle.Wave;
                    constantsList["HatchStyle.Weave"] = HatchStyle.Weave;
                    constantsList["HatchStyle.WideDownwardDiagonal"] = HatchStyle.WideDownwardDiagonal;
                    constantsList["HatchStyle.WideUpwardDiagonal"] = HatchStyle.WideUpwardDiagonal;
                    constantsList["HatchStyle.ZigZag"] = HatchStyle.ZigZag;
                    #endregion

                    //constantsList[""] = ;


                    constantsList_low = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                    foreach (DictionaryEntry de in constantsList)
                    {
                        constantsList_low[((string)de.Key).ToLowerInvariant()] = de.Value;
                    }
                }
                return syntaxCaseSensitive ? constantsList : constantsList_low;
            }
        }
        #endregion

        #region NamespacesList
        private static object namespaceObj = new object();
        private static Hashtable namespacesList = null;
        private static Hashtable namespacesList_low = null;
        private Hashtable NamespacesList
        {
            get
            {
                if (namespacesList == null)
                {
                    namespacesList = new Hashtable();
                    namespacesList["Totals"] = namespaceObj;
                    namespacesList["System"] = namespaceObj;
                    namespacesList["System.Convert"] = namespaceObj;
                    namespacesList["System.Globalization"] = namespaceObj;
                    namespacesList["Convert"] = namespaceObj;
                    namespacesList["Math"] = namespaceObj;
                    namespacesList["Func"] = namespaceObj;
                    namespacesList["Func.EngineHelper"] = namespaceObj;
                    namespacesList["StiNullValuesHelper"] = namespaceObj;
                    namespacesList["Image"] = namespaceObj;
                    namespacesList["Color"] = namespaceObj;
                    namespacesList["HatchStyle"] = namespaceObj;

                    //namespacesList["DBNull"] = namespaceObj;
                    //namespacesList["MidpointRounding"] = namespaceObj;
                    //namespacesList["StiRankOrder"] = namespaceObj;

                    namespacesList_low = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                    foreach (DictionaryEntry de in namespacesList)
                    {
                        namespacesList_low[((string)de.Key).ToLowerInvariant()] = de.Value;
                    }
                }
                return syntaxCaseSensitive ? namespacesList : namespacesList_low;
            }
        }
        #endregion

        #region UserFunctionsList
        private object lockUserFunctionsList = new object();
        private Hashtable userFunctionsList = null;
        private Hashtable UserFunctionsList
        {
            get
            {
                if (userFunctionsList == null)
                {
                    lock (lockUserFunctionsList)
                    {
                        userFunctionsList = new Hashtable();
                        var tempUserFunctionsList = new Hashtable();
                        var functions = StiFunctions.GetFunctions(false);
                        foreach (StiFunction func in functions)
                        {
                            var list = tempUserFunctionsList[func.FunctionName] as List<StiFunction>;
                            if (list == null)
                            {
                                list = new List<StiFunction>();
                                tempUserFunctionsList[func.FunctionName] = list;
                                userFunctionsList[func.FunctionName] = (int)StiFunctionType.UserFunction + userFunctionsList.Count;
                            }
                            list.Add(func);
                        }
                    }
                }
                return userFunctionsList;
            }
        }
        #endregion

        #region OperatorsList
        private static Hashtable operatorsList = null;
        private static Hashtable operatorsList_low = null;
        private static Hashtable operatorsList_vb = null;
        private Hashtable OperatorsList
        {
            get
            {
                if (operatorsList == null)
                {
                    operatorsList = new Hashtable();
                    //operatorsList["operator"] = StiTokenType.Unknown;


                    operatorsList_vb = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                    foreach (DictionaryEntry de in operatorsList)
                    {
                        operatorsList_vb[((string)de.Key).ToLowerInvariant()] = de.Value;
                    }

                    operatorsList_vb["Not"] = StiTokenType.Not;
                    operatorsList_vb["And"] = StiTokenType.And;
                    operatorsList_vb["Or"] = StiTokenType.Or;
                    operatorsList_vb["Xor"] = StiTokenType.Xor;
                    operatorsList_vb["AndAlso"] = StiTokenType.DoubleAnd;
                    operatorsList_vb["OrElse"] = StiTokenType.DoubleOr;
                    operatorsList_vb["Mod"] = StiTokenType.Percent;


                    operatorsList_low = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                    foreach (DictionaryEntry de in operatorsList)
                    {
                        operatorsList_low[((string)de.Key).ToLowerInvariant()] = de.Value;
                    }

                }
                if (IsVB) return operatorsList_vb;
                return syntaxCaseSensitive ? operatorsList : operatorsList_low;
            }
        }
        #endregion

        #region KeywordsList
        private static Hashtable keywordsList = null;
        //private static Hashtable keywordsList_low = null;
        //private static Hashtable keywordsList_vb = null;
        private Hashtable KeywordsList
        {
            get
            {
                if (keywordsList == null)
                {
                    keywordsList = new Hashtable();
                    keywordsList["ref"] = StiTokenType.RefVariable;
                    keywordsList["out"] = StiTokenType.RefVariable;
                    keywordsList["new"] = StiTokenType.New;

                    //keywordsList_vb = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                    //foreach (DictionaryEntry de in keywordsList)
                    //{
                    //    keywordsList_vb[((string)de.Key).ToLowerInvariant()] = de.Value;
                    //}
                    //keywordsList_vb["Not"] = StiTokenType.Not;

                    //keywordsList_low = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                    //foreach (DictionaryEntry de in keywordsList)
                    //{
                    //    keywordsList_low[((string)de.Key).ToLowerInvariant()] = de.Value;
                    //}
                }
                //if (isVB) return keywordsList_vb;
                //return syntaxCaseSensitive ? keywordsList : keywordsList_low;
                return keywordsList;
            }
        }
        #endregion
    }
}

