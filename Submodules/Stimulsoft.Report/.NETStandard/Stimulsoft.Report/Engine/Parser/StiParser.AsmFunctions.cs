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
using System.Reflection;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using System.Globalization;
using Stimulsoft.Base.Helpers;
using System.Linq;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Engine
{
    public partial class StiParser
    {
        #region Evaluate the value of the function.
        private object call_func(object name, ArrayList argsList, StiAsmCommand asmCommand)
        {
            int category;
            int category2;
            StiFunctionType functionType = (StiFunctionType)name;

            int overload = CheckParserMethodInfo2(functionType, argsList, asmCommand);

            switch (functionType)
            {
                #region Totals
                case StiFunctionType.Count:
                    return Stimulsoft.Report.Totals.Count(argsList[0], report, null);

                case StiFunctionType.CountDistinct:
                    return Stimulsoft.Report.Totals.CountDistinct(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.Avg:
                    return Stimulsoft.Report.Totals.Avg(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.AvgD:
                    return Stimulsoft.Report.Totals.AvgD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.AvgDate:
                    return Stimulsoft.Report.Totals.AvgDate(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.AvgI:
                    return Stimulsoft.Report.Totals.AvgI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.AvgTime:
                    return Stimulsoft.Report.Totals.AvgTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.Max:
                    return Stimulsoft.Report.Totals.Max(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.MaxD:
                    return Stimulsoft.Report.Totals.MaxD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.MaxDate:
                    return Stimulsoft.Report.Totals.MaxDate(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.MaxI:
                    return Stimulsoft.Report.Totals.MaxI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.MaxStr:
                    return Stimulsoft.Report.Totals.MaxStr(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.MaxTime:
                    return Stimulsoft.Report.Totals.MaxTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.Median:
                    return Stimulsoft.Report.Totals.Median(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.MedianD:
                    return Stimulsoft.Report.Totals.MedianD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.MedianI:
                    return Stimulsoft.Report.Totals.MedianI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.Min:
                    return Stimulsoft.Report.Totals.Min(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.MinD:
                    return Stimulsoft.Report.Totals.MinD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.MinDate:
                    return Stimulsoft.Report.Totals.MinDate(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.MinI:
                    return Stimulsoft.Report.Totals.MinI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.MinStr:
                    return Stimulsoft.Report.Totals.MinStr(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.MinTime:
                    return Stimulsoft.Report.Totals.MinTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.Mode:
                    return Stimulsoft.Report.Totals.Mode(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.ModeD:
                    return Stimulsoft.Report.Totals.ModeD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.ModeI:
                    return Stimulsoft.Report.Totals.ModeI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.Sum:
                    return Stimulsoft.Report.Totals.Sum(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.SumD:
                    return Stimulsoft.Report.Totals.SumD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.SumDistinct:
                    var dataSumDistinct = new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this);
                    dataSumDistinct.AsmList2 = (List<StiAsmCommand>)argsList[2];
                    return Stimulsoft.Report.Totals.SumDistinct(dataSumDistinct, report, null);

                case StiFunctionType.SumI:
                    return Stimulsoft.Report.Totals.SumI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.SumTime:
                    return Stimulsoft.Report.Totals.SumTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.First:
                    return Stimulsoft.Report.Totals.First(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.Last:
                    return Stimulsoft.Report.Totals.Last(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);


                case StiFunctionType.pCount:
                    return Stimulsoft.Report.Totals.cCount(argsList[0], report, null);

                case StiFunctionType.pCountDistinct:
                    return Stimulsoft.Report.Totals.cCountDistinct(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pAvg:
                    return Stimulsoft.Report.Totals.cAvg(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pAvgD:
                    return Stimulsoft.Report.Totals.cAvgD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pAvgDate:
                    return Stimulsoft.Report.Totals.cAvgDate(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pAvgI:
                    return Stimulsoft.Report.Totals.cAvgI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pAvgTime:
                    return Stimulsoft.Report.Totals.cAvgTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pMax:
                    return Stimulsoft.Report.Totals.cMax(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pMaxD:
                    return Stimulsoft.Report.Totals.cMaxD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pMaxDate:
                    return Stimulsoft.Report.Totals.cMaxDate(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pMaxI:
                    return Stimulsoft.Report.Totals.cMaxI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pMaxStr:
                    return Stimulsoft.Report.Totals.cMaxStr(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pMaxTime:
                    return Stimulsoft.Report.Totals.cMaxTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pMedian:
                    return Stimulsoft.Report.Totals.cMedian(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pMedianD:
                    return Stimulsoft.Report.Totals.cMedianD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pMedianI:
                    return Stimulsoft.Report.Totals.cMedianI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pMin:
                    return Stimulsoft.Report.Totals.cMin(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pMinD:
                    return Stimulsoft.Report.Totals.cMinD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pMinDate:
                    return Stimulsoft.Report.Totals.cMinDate(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pMinI:
                    return Stimulsoft.Report.Totals.cMinI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pMinStr:
                    return Stimulsoft.Report.Totals.cMinStr(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pMinTime:
                    return Stimulsoft.Report.Totals.cMinTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pMode:
                    return Stimulsoft.Report.Totals.cMode(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pModeD:
                    return Stimulsoft.Report.Totals.cModeD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pModeI:
                    return Stimulsoft.Report.Totals.cModeI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pSum:
                    return Stimulsoft.Report.Totals.cSum(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pSumD:
                    return Stimulsoft.Report.Totals.cSumD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pSumDistinct:
                    return Stimulsoft.Report.Totals.cSum(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null); // !!! ????????

                case StiFunctionType.pSumI:
                    return Stimulsoft.Report.Totals.cSumI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pSumTime:
                    return Stimulsoft.Report.Totals.cSumTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pFirst:
                    return Stimulsoft.Report.Totals.cFirst(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.pLast:
                    return Stimulsoft.Report.Totals.cLast(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);


                case StiFunctionType.prCount:
                    return Stimulsoft.Report.Totals.cCountRunning(argsList[0], report, null);

                case StiFunctionType.prCountDistinct:
                    return Stimulsoft.Report.Totals.cCountDistinctRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prAvg:
                    return Stimulsoft.Report.Totals.cAvgRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prAvgD:
                    return Stimulsoft.Report.Totals.cAvgDRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prAvgDate:
                    return Stimulsoft.Report.Totals.cAvgDate(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);  // !!! ????????

                case StiFunctionType.prAvgI:
                    return Stimulsoft.Report.Totals.cAvgIRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prAvgTime:
                    return Stimulsoft.Report.Totals.cAvgTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);  // !!! ????????

                case StiFunctionType.prMax:
                    return Stimulsoft.Report.Totals.cMaxRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prMaxD:
                    return Stimulsoft.Report.Totals.cMaxDRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prMaxDate:
                    return Stimulsoft.Report.Totals.cMaxDate(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);  // !!! ????????

                case StiFunctionType.prMaxI:
                    return Stimulsoft.Report.Totals.cMaxIRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prMaxStr:
                    return Stimulsoft.Report.Totals.cMaxStr(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);  // !!! ????????

                case StiFunctionType.prMaxTime:
                    return Stimulsoft.Report.Totals.cMaxTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);  // !!! ????????

                case StiFunctionType.prMedian:
                    return Stimulsoft.Report.Totals.cMedianRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prMedianD:
                    return Stimulsoft.Report.Totals.cMedianDRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prMedianI:
                    return Stimulsoft.Report.Totals.cMedianIRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prMin:
                    return Stimulsoft.Report.Totals.cMinRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prMinD:
                    return Stimulsoft.Report.Totals.cMinDRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prMinDate:
                    return Stimulsoft.Report.Totals.cMinDate(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);  // !!! ????????

                case StiFunctionType.prMinI:
                    return Stimulsoft.Report.Totals.cMinIRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prMinStr:
                    return Stimulsoft.Report.Totals.cMinStr(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);  // !!! ????????

                case StiFunctionType.prMinTime:
                    return Stimulsoft.Report.Totals.cMinTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);  // !!! ????????

                case StiFunctionType.prMode:
                    return Stimulsoft.Report.Totals.cModeRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prModeD:
                    return Stimulsoft.Report.Totals.cModeDRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prModeI:
                    return Stimulsoft.Report.Totals.cModeIRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prSum:
                    return Stimulsoft.Report.Totals.cSumRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prSumD:
                    return Stimulsoft.Report.Totals.cSumDRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prSumDistinct:
                    return Stimulsoft.Report.Totals.cSum(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);  // !!! ????????

                case StiFunctionType.prSumI:
                    return Stimulsoft.Report.Totals.cSumIRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prSumTime:
                    return Stimulsoft.Report.Totals.cSumTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);  // !!! ????????

                case StiFunctionType.prFirst:
                    return Stimulsoft.Report.Totals.cFirstRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.prLast:
                    return Stimulsoft.Report.Totals.cLastRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);


                case StiFunctionType.iCount:
                    return Stimulsoft.Report.Totals.Count(new StiParserData(argsList[0], null, this, (List<StiAsmCommand>)argsList[1]), report, null);

                case StiFunctionType.iCountDistinct:
                    return Stimulsoft.Report.Totals.CountDistinct(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iAvg:
                    return Stimulsoft.Report.Totals.Avg(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iAvgD:
                    return Stimulsoft.Report.Totals.AvgD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iAvgDate:
                    return Stimulsoft.Report.Totals.AvgDate(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iAvgI:
                    return Stimulsoft.Report.Totals.AvgI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iAvgTime:
                    return Stimulsoft.Report.Totals.AvgTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iMax:
                    return Stimulsoft.Report.Totals.Max(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iMaxD:
                    return Stimulsoft.Report.Totals.MaxD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iMaxDate:
                    return Stimulsoft.Report.Totals.MaxDate(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iMaxI:
                    return Stimulsoft.Report.Totals.MaxI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iMaxStr:
                    return Stimulsoft.Report.Totals.MaxStr(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iMaxTime:
                    return Stimulsoft.Report.Totals.MaxTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iMedian:
                    return Stimulsoft.Report.Totals.Median(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iMedianD:
                    return Stimulsoft.Report.Totals.MedianD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iMedianI:
                    return Stimulsoft.Report.Totals.MedianI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iMin:
                    return Stimulsoft.Report.Totals.Min(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iMinD:
                    return Stimulsoft.Report.Totals.MinD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iMinDate:
                    return Stimulsoft.Report.Totals.MinDate(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iMinI:
                    return Stimulsoft.Report.Totals.MinI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iMinStr:
                    return Stimulsoft.Report.Totals.MinStr(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iMinTime:
                    return Stimulsoft.Report.Totals.MinTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iMode:
                    return Stimulsoft.Report.Totals.Mode(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iModeD:
                    return Stimulsoft.Report.Totals.ModeD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iModeI:
                    return Stimulsoft.Report.Totals.ModeI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iSum:
                    return Stimulsoft.Report.Totals.Sum(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iSumD:
                    return Stimulsoft.Report.Totals.SumD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iSumDistinct:
                    return Stimulsoft.Report.Totals.SumDistinct(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iSumI:
                    return Stimulsoft.Report.Totals.SumI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iSumTime:
                    return Stimulsoft.Report.Totals.SumTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iFirst:
                    return Stimulsoft.Report.Totals.First(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.iLast:
                    return Stimulsoft.Report.Totals.Last(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);


                case StiFunctionType.piCount:
                    return Stimulsoft.Report.Totals.cCount(new StiParserData(argsList[0], null, this, (List<StiAsmCommand>)argsList[1]), report, null);

                case StiFunctionType.piCountDistinct:
                    return Stimulsoft.Report.Totals.cCountDistinct(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piAvg:
                    return Stimulsoft.Report.Totals.cAvg(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piAvgD:
                    return Stimulsoft.Report.Totals.cAvgD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piAvgDate:
                    return Stimulsoft.Report.Totals.cAvgDate(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piAvgI:
                    return Stimulsoft.Report.Totals.cAvgI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piAvgTime:
                    return Stimulsoft.Report.Totals.cAvgTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piMax:
                    return Stimulsoft.Report.Totals.cMax(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piMaxD:
                    return Stimulsoft.Report.Totals.cMaxD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piMaxDate:
                    return Stimulsoft.Report.Totals.cMaxDate(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piMaxI:
                    return Stimulsoft.Report.Totals.cMaxI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piMaxStr:
                    return Stimulsoft.Report.Totals.cMaxStr(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piMaxTime:
                    return Stimulsoft.Report.Totals.cMaxTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piMedian:
                    return Stimulsoft.Report.Totals.cMedian(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piMedianD:
                    return Stimulsoft.Report.Totals.cMedianD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piMedianI:
                    return Stimulsoft.Report.Totals.cMedianI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piMin:
                    return Stimulsoft.Report.Totals.cMin(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piMinD:
                    return Stimulsoft.Report.Totals.cMinD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piMinDate:
                    return Stimulsoft.Report.Totals.cMinDate(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piMinI:
                    return Stimulsoft.Report.Totals.cMinI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piMinStr:
                    return Stimulsoft.Report.Totals.cMinStr(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piMinTime:
                    return Stimulsoft.Report.Totals.cMinTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piMode:
                    return Stimulsoft.Report.Totals.cMode(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piModeD:
                    return Stimulsoft.Report.Totals.cModeD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piModeI:
                    return Stimulsoft.Report.Totals.cModeI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piSum:
                    return Stimulsoft.Report.Totals.cSum(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piSumD:
                    return Stimulsoft.Report.Totals.cSumD(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piSumDistinct:
                    return Stimulsoft.Report.Totals.cSum(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null); // !!! ????????

                case StiFunctionType.piSumI:
                    return Stimulsoft.Report.Totals.cSumI(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piSumTime:
                    return Stimulsoft.Report.Totals.cSumTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piFirst:
                    return Stimulsoft.Report.Totals.cFirst(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.piLast:
                    return Stimulsoft.Report.Totals.cLast(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);


                case StiFunctionType.priCount:
                    return Stimulsoft.Report.Totals.cCountRunning(new StiParserData(argsList[0], null, this, (List<StiAsmCommand>)argsList[1]), report, null);

                case StiFunctionType.priCountDistinct:
                    return Stimulsoft.Report.Totals.cCountDistinctRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priAvg:
                    return Stimulsoft.Report.Totals.cAvgRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priAvgD:
                    return Stimulsoft.Report.Totals.cAvgDRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priAvgDate:
                    return Stimulsoft.Report.Totals.cAvgDate(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);  // !!! ????????

                case StiFunctionType.priAvgI:
                    return Stimulsoft.Report.Totals.cAvgIRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priAvgTime:
                    return Stimulsoft.Report.Totals.cAvgTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);  // !!! ????????

                case StiFunctionType.priMax:
                    return Stimulsoft.Report.Totals.cMaxRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priMaxD:
                    return Stimulsoft.Report.Totals.cMaxDRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priMaxDate:
                    return Stimulsoft.Report.Totals.cMaxDate(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);  // !!! ????????

                case StiFunctionType.priMaxI:
                    return Stimulsoft.Report.Totals.cMaxIRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priMaxStr:
                    return Stimulsoft.Report.Totals.cMaxStr(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);  // !!! ????????

                case StiFunctionType.priMaxTime:
                    return Stimulsoft.Report.Totals.cMaxTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);  // !!! ????????

                case StiFunctionType.priMedian:
                    return Stimulsoft.Report.Totals.cMedianRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priMedianD:
                    return Stimulsoft.Report.Totals.cMedianDRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priMedianI:
                    return Stimulsoft.Report.Totals.cMedianIRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priMin:
                    return Stimulsoft.Report.Totals.cMinRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priMinD:
                    return Stimulsoft.Report.Totals.cMinDRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priMinDate:
                    return Stimulsoft.Report.Totals.cMinDate(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);  // !!! ????????

                case StiFunctionType.priMinI:
                    return Stimulsoft.Report.Totals.cMinIRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priMinStr:
                    return Stimulsoft.Report.Totals.cMinStr(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);  // !!! ????????

                case StiFunctionType.priMinTime:
                    return Stimulsoft.Report.Totals.cMinTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);  // !!! ????????

                case StiFunctionType.priMode:
                    return Stimulsoft.Report.Totals.cModeRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priModeD:
                    return Stimulsoft.Report.Totals.cModeDRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priModeI:
                    return Stimulsoft.Report.Totals.cModeIRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priSum:
                    return Stimulsoft.Report.Totals.cSumRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priSumD:
                    return Stimulsoft.Report.Totals.cSumDRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priSumDistinct:
                    return Stimulsoft.Report.Totals.cSum(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);  // !!! ????????

                case StiFunctionType.priSumI:
                    return Stimulsoft.Report.Totals.cSumIRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priSumTime:
                    return Stimulsoft.Report.Totals.cSumTime(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);  // !!! ????????

                case StiFunctionType.priFirst:
                    return Stimulsoft.Report.Totals.cFirstRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);

                case StiFunctionType.priLast:
                    return Stimulsoft.Report.Totals.cLastRunning(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this, (List<StiAsmCommand>)argsList[2]), report, null);


                case StiFunctionType.Rank:
                    switch (overload)
                    {
                        case 1: return Totals.Rank(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                        case 2: return Totals.Rank(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null, Convert.ToBoolean(argsList[2]), (StiRankOrder)(argsList[3]));
                    }
                    break;
                #endregion

                #region Totals Hierarchical
                case StiFunctionType.CountAllLevels:
                    return Stimulsoft.Report.Totals.CountAllLevels(argsList[0]);
                case StiFunctionType.CountAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.CountAllLevelsOnlyChilds(argsList[0]);
                case StiFunctionType.CountOnlyChilds:
                    return Stimulsoft.Report.Totals.CountOnlyChilds(argsList[0]);
                case StiFunctionType.CountDistinctAllLevels:
                    return Stimulsoft.Report.Totals.CountDistinctAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.CountDistinctAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.CountDistinctAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.CountDistinctOnlyChilds:
                    return Stimulsoft.Report.Totals.CountDistinctOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.SumAllLevels:
                    return Stimulsoft.Report.Totals.SumAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.SumAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.SumAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.SumOnlyChilds:
                    return Stimulsoft.Report.Totals.SumOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.SumDAllLevels:
                    return Stimulsoft.Report.Totals.SumDAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.SumDAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.SumDAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.SumDOnlyChilds:
                    return Stimulsoft.Report.Totals.SumDOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.SumIAllLevels:
                    return Stimulsoft.Report.Totals.SumIAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.SumIAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.SumIAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.SumIOnlyChilds:
                    return Stimulsoft.Report.Totals.SumIOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.SumTimeAllLevels:
                    return Stimulsoft.Report.Totals.SumTimeAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.SumTimeAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.SumTimeAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.SumTimeOnlyChilds:
                    return Stimulsoft.Report.Totals.SumTimeOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.AvgAllLevels:
                    return Stimulsoft.Report.Totals.AvgAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.AvgAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.AvgAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.AvgOnlyChilds:
                    return Stimulsoft.Report.Totals.AvgOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.AvgDAllLevels:
                    return Stimulsoft.Report.Totals.AvgDAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.AvgDAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.AvgDAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.AvgDOnlyChilds:
                    return Stimulsoft.Report.Totals.AvgDOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.AvgIAllLevels:
                    return Stimulsoft.Report.Totals.AvgIAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.AvgIAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.AvgIAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.AvgIOnlyChilds:
                    return Stimulsoft.Report.Totals.AvgIOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.AvgDateAllLevels:
                    return Stimulsoft.Report.Totals.AvgDateAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.AvgDateAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.AvgDateAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.AvgDateOnlyChilds:
                    return Stimulsoft.Report.Totals.AvgDateOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.AvgTimeAllLevels:
                    return Stimulsoft.Report.Totals.AvgTimeAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.AvgTimeAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.AvgTimeAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.AvgTimeOnlyChilds:
                    return Stimulsoft.Report.Totals.AvgTimeOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.MaxAllLevels:
                    return Stimulsoft.Report.Totals.MaxAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MaxAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.MaxAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MaxOnlyChilds:
                    return Stimulsoft.Report.Totals.MaxOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MaxDAllLevels:
                    return Stimulsoft.Report.Totals.MaxDAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MaxDAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.MaxDAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MaxDOnlyChilds:
                    return Stimulsoft.Report.Totals.MaxDOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MaxIAllLevels:
                    return Stimulsoft.Report.Totals.MaxIAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MaxIAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.MaxIAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MaxIOnlyChilds:
                    return Stimulsoft.Report.Totals.MaxIOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.MinAllLevels:
                    return Stimulsoft.Report.Totals.MinAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MinAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.MinAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MinOnlyChilds:
                    return Stimulsoft.Report.Totals.MinOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MinDAllLevels:
                    return Stimulsoft.Report.Totals.MinDAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MinDAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.MinDAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MinDOnlyChilds:
                    return Stimulsoft.Report.Totals.MinDOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MinIAllLevels:
                    return Stimulsoft.Report.Totals.MinIAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MinIAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.MinIAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MinIOnlyChilds:
                    return Stimulsoft.Report.Totals.MinIOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.MedianAllLevels:
                    return Stimulsoft.Report.Totals.MedianAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MedianAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.MedianAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MedianOnlyChilds:
                    return Stimulsoft.Report.Totals.MedianOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MedianDAllLevels:
                    return Stimulsoft.Report.Totals.MedianDAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MedianDAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.MedianDAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MedianDOnlyChilds:
                    return Stimulsoft.Report.Totals.MedianDOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MedianIAllLevels:
                    return Stimulsoft.Report.Totals.MedianIAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MedianIAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.MedianIAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MedianIOnlyChilds:
                    return Stimulsoft.Report.Totals.MedianIOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.ModeAllLevels:
                    return Stimulsoft.Report.Totals.ModeAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.ModeAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.ModeAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.ModeOnlyChilds:
                    return Stimulsoft.Report.Totals.ModeOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.ModeDAllLevels:
                    return Stimulsoft.Report.Totals.ModeDAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.ModeDAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.ModeDAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.ModeDOnlyChilds:
                    return Stimulsoft.Report.Totals.ModeDOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.ModeIAllLevels:
                    return Stimulsoft.Report.Totals.ModeIAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.ModeIAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.ModeIAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.ModeIOnlyChilds:
                    return Stimulsoft.Report.Totals.ModeIOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.FirstAllLevels:
                    return Stimulsoft.Report.Totals.FirstAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.FirstAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.FirstAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.FirstOnlyChilds:
                    return Stimulsoft.Report.Totals.FirstOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.LastAllLevels:
                    return Stimulsoft.Report.Totals.LastAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.LastAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.LastAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.LastOnlyChilds:
                    return Stimulsoft.Report.Totals.LastOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.MinDateAllLevels:
                    return Stimulsoft.Report.Totals.MinDateAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MinDateAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.MinDateAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MinDateOnlyChilds:
                    return Stimulsoft.Report.Totals.MinDateOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MinTimeAllLevels:
                    return Stimulsoft.Report.Totals.MinTimeAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MinTimeAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.MinTimeAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MinTimeOnlyChilds:
                    return Stimulsoft.Report.Totals.MinTimeOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MinStrAllLevels:
                    return Stimulsoft.Report.Totals.MinStrAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MinStrAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.MinStrAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MinStrOnlyChilds:
                    return Stimulsoft.Report.Totals.MinStrOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);

                case StiFunctionType.MaxDateAllLevels:
                    return Stimulsoft.Report.Totals.MaxDateAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MaxDateAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.MaxDateAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MaxDateOnlyChilds:
                    return Stimulsoft.Report.Totals.MaxDateOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MaxTimeAllLevels:
                    return Stimulsoft.Report.Totals.MaxTimeAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MaxTimeAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.MaxTimeAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MaxTimeOnlyChilds:
                    return Stimulsoft.Report.Totals.MaxTimeOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MaxStrAllLevels:
                    return Stimulsoft.Report.Totals.MaxStrAllLevels(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MaxStrAllLevelsOnlyChilds:
                    return Stimulsoft.Report.Totals.MaxStrAllLevelsOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                case StiFunctionType.MaxStrOnlyChilds:
                    return Stimulsoft.Report.Totals.MaxStrOnlyChilds(new StiParserData(argsList[0], (List<StiAsmCommand>)argsList[1], this), report, null);
                #endregion

                #region Math
                case StiFunctionType.Abs:
                    category = get_category(argsList[0]);
                    if (category <= 1 || category >= 8)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Abs", "1", GetTypeName(argsList[0]), "double");
                    }
                    else if (category == 2) return Math.Abs(Convert.ToDecimal(argsList[0]));
                    else if (category == 3) return Math.Abs(Convert.ToDouble(argsList[0]));
                    return Math.Abs(Convert.ToInt32(argsList[0]));

                case StiFunctionType.Acos:
                    category = get_category(argsList[0]);
                    if (category <= 1 || category >= 8)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Acos", "1", GetTypeName(argsList[0]), "double");
                    }
                    return Math.Acos(Convert.ToDouble(argsList[0]));

                case StiFunctionType.Asin:
                    category = get_category(argsList[0]);
                    if (category <= 1 || category >= 8)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Asin", "1", GetTypeName(argsList[0]), "double");
                    }
                    return Math.Asin(Convert.ToDouble(argsList[0]));

                case StiFunctionType.Atan:
                    category = get_category(argsList[0]);
                    if (category <= 1 || category >= 8)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Atan", "1", GetTypeName(argsList[0]), "double");
                    }
                    return Math.Atan(Convert.ToDouble(argsList[0]));

                case StiFunctionType.Ceiling:
                    category = get_category(argsList[0]);
                    if (category <= 1 || category >= 8)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Ceiling", "1", GetTypeName(argsList[0]), "double");
                    }
                    else if (category == 2) return Math.Ceiling(Convert.ToDecimal(argsList[0]));
                    return Math.Ceiling(Convert.ToDouble(argsList[0]));

                case StiFunctionType.Cos:
                    category = get_category(argsList[0]);
                    if (category <= 1 || category >= 8)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Cos", "1", GetTypeName(argsList[0]), "double");
                    }
                    return Math.Cos(Convert.ToDouble(argsList[0]));

                case StiFunctionType.Div:
                    category = get_category(argsList[0]);
                    category2 = get_category(argsList[1]);
                    if (category <= 1 || category >= 8)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Div", "1", GetTypeName(argsList[0]), "double");
                    }
                    else if (category2 <= 1 || category2 >= 8)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Div", "2", GetTypeName(argsList[1]), "double");
                    }
                    else if (argsList.Count == 3)
                    {
                        if (category == 2)
                        {
                            if (Convert.ToDecimal(argsList[1]) == 0) return Convert.ToDecimal(argsList[2]);
                            return Convert.ToDecimal(argsList[0]) / Convert.ToDecimal(argsList[1]);
                        }
                        else
                        {
                            if (Convert.ToDouble(argsList[1]) == 0) return Convert.ToDouble(argsList[2]);
                            return Convert.ToDouble(argsList[0]) / Convert.ToDouble(argsList[1]);
                        }
                    }
                    else if (argsList.Count == 2)
                    {
                        if (category == 2) return Convert.ToDecimal(argsList[0]) / Convert.ToDecimal(argsList[1]);
                        else return Convert.ToDouble(argsList[0]) / Convert.ToDouble(argsList[1]);
                    }
                    ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "Div", argsList.Count.ToString());
                    break;

                case StiFunctionType.Exp:
                    category = get_category(argsList[0]);
                    if (category <= 1 || category >= 8)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Exp", "1", GetTypeName(argsList[0]), "double");
                    }
                    return Math.Exp(Convert.ToDouble(argsList[0]));

                case StiFunctionType.Floor:
                    category = get_category(argsList[0]);
                    if (category <= 1 || category >= 4)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Floor", "1", GetTypeName(argsList[0]), "double");
                    }
                    if (category == 2) return Math.Floor(Convert.ToDecimal(argsList[0]));
                    return Math.Floor(Convert.ToDouble(argsList[0]));

                case StiFunctionType.Log:
                    category = get_category(argsList[0]);
                    if (category <= 1 || category >= 8)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Log", "1", GetTypeName(argsList[0]), "double");
                    }
                    return Math.Log(Convert.ToDouble(argsList[0]));

                case StiFunctionType.Maximum:
                    if (argsList.Count != 2) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "Maximum", argsList.Count.ToString());
                    category = get_category(argsList[0]);
                    category2 = get_category(argsList[1]);
                    if (category <= 1 || category >= 8)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Maximum", "1", GetTypeName(argsList[0]), "double");
                    }
                    else if (category2 <= 1 || category2 >= 8)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Maximum", "2", GetTypeName(argsList[1]), "double");
                    }
                    else if (category == 2) return Math.Max(Convert.ToDecimal(argsList[0]), Convert.ToDecimal(argsList[1]));
                    else if (category == 3) return Math.Max(Convert.ToDouble(argsList[0]), Convert.ToDouble(argsList[1]));
                    return Math.Max(Convert.ToInt64(argsList[0]), Convert.ToInt64(argsList[1]));

                case StiFunctionType.Minimum:
                    if (argsList.Count != 2) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "Minimum", argsList.Count.ToString());
                    category = get_category(argsList[0]);
                    category2 = get_category(argsList[1]);
                    if (category <= 1 || category >= 8)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Minimum", "1", GetTypeName(argsList[0]), "double");
                    }
                    else if (category2 <= 1 || category2 >= 8)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Minimum", "2", GetTypeName(argsList[1]), "double");
                    }
                    else if (category == 2) return Math.Min(Convert.ToDecimal(argsList[0]), Convert.ToDecimal(argsList[1]));
                    else if (category == 3) return Math.Min(Convert.ToDouble(argsList[0]), Convert.ToDouble(argsList[1]));
                    return Math.Min(Convert.ToInt64(argsList[0]), Convert.ToInt64(argsList[1]));

                case StiFunctionType.Round:
                    switch (overload)
                    {
                        case 1: return Math.Round(Convert.ToDouble(argsList[0]));
                        case 2: return Math.Round(Convert.ToDouble(argsList[0]), Convert.ToInt32(argsList[1]));
                        case 3: return Math.Round(Convert.ToDecimal(argsList[0]));
                        case 4: return Math.Round(Convert.ToDecimal(argsList[0]), Convert.ToInt32(argsList[1]));
                        case 5: return Math.Round(Convert.ToDouble(argsList[0]), (MidpointRounding)(argsList[1]));
                        case 6: return Math.Round(Convert.ToDouble(argsList[0]), Convert.ToInt32(argsList[1]), (MidpointRounding)(argsList[2]));
                        case 7: return Math.Round(Convert.ToDecimal(argsList[0]), (MidpointRounding)(argsList[1]));
                        case 8: return Math.Round(Convert.ToDecimal(argsList[0]), Convert.ToInt32(argsList[1]), (MidpointRounding)(argsList[2]));
                    }
                    break;

                case StiFunctionType.Sign:
                    category = get_category(argsList[0]);
                    if (category <= 1 || category >= 8)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Sign", "1", GetTypeName(argsList[0]), "double");
                    }
                    else if (category == 2) return Math.Sign(Convert.ToDecimal(argsList[0]));
                    else if (category == 3) return Math.Sign(Convert.ToDouble(argsList[0]));
                    return Math.Sign(Convert.ToInt32(argsList[0]));

                case StiFunctionType.Sin:
                    category = get_category(argsList[0]);
                    if (category <= 1 || category >= 8)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Sin", "1", GetTypeName(argsList[0]), "double");
                    }
                    return Math.Sin(Convert.ToDouble(argsList[0]));

                case StiFunctionType.Sqrt:
                    category = get_category(argsList[0]);
                    if (category <= 1 || category >= 8)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Sqrt", "1", GetTypeName(argsList[0]), "double");
                    }
                    return Math.Sqrt(Convert.ToDouble(argsList[0]));

                case StiFunctionType.Tan:
                    category = get_category(argsList[0]);
                    if (category <= 1 || category >= 8)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Tan", "1", GetTypeName(argsList[0]), "double");
                    }
                    return Math.Tan(Convert.ToDouble(argsList[0]));

                case StiFunctionType.Truncate:
                    category = get_category(argsList[0]);
                    if (category <= 1 || category >= 4)
                    {
                        ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Truncate", "1", GetTypeName(argsList[0]), "double");
                    }
                    else if (category == 2) return Math.Truncate(Convert.ToDecimal(argsList[0]));
                    return Math.Truncate(Convert.ToDouble(argsList[0]));
                #endregion

                #region Date
                case StiFunctionType.DateDiff:
                    if (overload == 1) return Convert.ToDateTime(argsList[0]).Subtract(Convert.ToDateTime(argsList[1]));
                    if (overload == 2) return Convert.ToDateTime(argsList[0]).Subtract(Convert.ToDateTime(argsList[1]));
                    if (overload == 3) return ((DateTimeOffset)argsList[0]).Subtract((DateTimeOffset)argsList[1]);
                    if (overload == 4) return ((DateTimeOffset)argsList[0]).Subtract((DateTimeOffset)argsList[1]);
                    break;

                case StiFunctionType.DateSerial:
                    if (overload == 1) return new DateTime(Convert.ToInt64(argsList[0]));
                    if (overload == 2) return new DateTime(Convert.ToInt32(argsList[0]), Convert.ToInt32(argsList[1]), Convert.ToInt32(argsList[2]));
                    break;

                case StiFunctionType.TimeSerial:
                    if (overload == 1) return new TimeSpan(Convert.ToInt32(argsList[0]), Convert.ToInt32(argsList[1]), Convert.ToInt32(argsList[2]));
                    break;

                case StiFunctionType.Year:
                    if (overload == 1 || overload == 2) return Convert.ToDateTime(argsList[0]).Year;
                    if (overload == 3 || overload == 4) return ((DateTimeOffset)argsList[0]).Year;
                    break;

                case StiFunctionType.Month:
                    if (overload == 1 || overload == 2) return Convert.ToDateTime(argsList[0]).Month;
                    if (overload == 3 || overload == 4) return ((DateTimeOffset)argsList[0]).Month;
                    break;

                case StiFunctionType.Day:
                    if (overload == 1 || overload == 2) return Convert.ToDateTime(argsList[0]).Day;
                    if (overload == 3 || overload == 4) return ((DateTimeOffset)argsList[0]).Day;
                    break;

                case StiFunctionType.Hour:
                    if (overload == 1 || overload == 2) return Convert.ToDateTime(argsList[0]).Hour;
                    if (overload == 3 || overload == 4) return ((DateTimeOffset)argsList[0]).Hour;
                    break;

                case StiFunctionType.Minute:
                    if (overload == 1 || overload == 2) return Convert.ToDateTime(argsList[0]).Minute;
                    if (overload == 3 || overload == 4) return ((DateTimeOffset)argsList[0]).Minute;
                    break;

                case StiFunctionType.Second:
                    if (overload == 1 || overload == 2) return Convert.ToDateTime(argsList[0]).Second;
                    if (overload == 3 || overload == 4) return ((DateTimeOffset)argsList[0]).Second;
                    break;

                case StiFunctionType.DayOfWeek:
                    switch (overload)
                    {
                        case 1: return StiFunctionsDate.DayOfWeek(Convert.ToDateTime(argsList[0]));
                        case 3: return StiFunctionsDate.DayOfWeek(Convert.ToDateTime(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 5: return StiFunctionsDate.DayOfWeek(Convert.ToDateTime(argsList[0]), Convert.ToString(argsList[1]));
                        case 7: return StiFunctionsDate.DayOfWeek(Convert.ToDateTime(argsList[0]), Convert.ToString(argsList[1]), Convert.ToBoolean(argsList[2]));
                        case 2: return StiFunctionsDate.DayOfWeek((DateTime?)(argsList[0]));
                        case 4: return StiFunctionsDate.DayOfWeek((DateTime?)(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 6: return StiFunctionsDate.DayOfWeek((DateTime?)(argsList[0]), Convert.ToString(argsList[1]));
                        case 8: return StiFunctionsDate.DayOfWeek((DateTime?)(argsList[0]), Convert.ToString(argsList[1]), Convert.ToBoolean(argsList[2]));
                        case 9: return StiFunctionsDate.DayOfWeek((DateTimeOffset)argsList[0]);
                        case 11: return StiFunctionsDate.DayOfWeek((DateTimeOffset)argsList[0], Convert.ToBoolean(argsList[1]));
                        case 13: return StiFunctionsDate.DayOfWeek((DateTimeOffset)argsList[0], Convert.ToString(argsList[1]));
                        case 15: return StiFunctionsDate.DayOfWeek((DateTimeOffset)argsList[0], Convert.ToString(argsList[1]), Convert.ToBoolean(argsList[2]));
                        case 10: return StiFunctionsDate.DayOfWeek((DateTimeOffset?)(argsList[0]));
                        case 12: return StiFunctionsDate.DayOfWeek((DateTimeOffset?)(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 14: return StiFunctionsDate.DayOfWeek((DateTimeOffset?)(argsList[0]), Convert.ToString(argsList[1]));
                        case 16: return StiFunctionsDate.DayOfWeek((DateTimeOffset?)(argsList[0]), Convert.ToString(argsList[1]), Convert.ToBoolean(argsList[2]));
                    }
                    break;

                case StiFunctionType.DayOfYear:
                    if (overload == 1 || overload == 2) return Convert.ToDateTime(argsList[0]).DayOfYear;
                    if (overload == 3 || overload == 4) return ((DateTimeOffset)argsList[0]).DayOfYear;
                    break;

                case StiFunctionType.DaysInMonth:
                    switch (overload)
                    {
                        case 1: return (long)DateTime.DaysInMonth(Convert.ToDateTime(argsList[0]).Year, Convert.ToDateTime(argsList[0]).Month);
                        case 3: return (long)DateTime.DaysInMonth(Convert.ToInt32(argsList[0]), Convert.ToInt32(argsList[1]));
                        case 2: return (long)DateTime.DaysInMonth(((DateTime?)argsList[0]).Value.Year, Convert.ToDateTime(argsList[0]).Month);
                        case 4: return (long)DateTime.DaysInMonth(((DateTimeOffset)argsList[0]).Year, ((DateTimeOffset)argsList[0]).Month);
                        case 5: return (long)DateTime.DaysInMonth(((DateTimeOffset?)argsList[0]).Value.Year, ((DateTimeOffset?)argsList[0]).Value.Month);
                    }
                    break;

                case StiFunctionType.DaysInYear:
                    switch (overload)
                    {
                        case 1: return (long)(DateTime.IsLeapYear(Convert.ToDateTime(argsList[0]).Year) ? 366 : 365);
                        case 3: return (long)(DateTime.IsLeapYear(Convert.ToInt32(argsList[0])) ? 366 : 365);
                        case 2: return (long)(DateTime.IsLeapYear(((DateTime?)argsList[0]).Value.Year) ? 366 : 365);
                        case 4: return (long)(DateTime.IsLeapYear(((DateTimeOffset)argsList[0]).Year) ? 366 : 365);
                        case 5: return (long)(DateTime.IsLeapYear(((DateTimeOffset?)argsList[0]).Value.Year) ? 366 : 365);
                    }
                    break;

                case StiFunctionType.MonthName:
                    switch (overload)
                    {
                        case 1: return StiFunctionsDate.MonthName(Convert.ToDateTime(argsList[0]));
                        case 3: return StiFunctionsDate.MonthName(Convert.ToDateTime(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 5: return StiFunctionsDate.MonthName(Convert.ToDateTime(argsList[0]), Convert.ToString(argsList[1]));
                        case 7: return StiFunctionsDate.MonthName(Convert.ToDateTime(argsList[0]), Convert.ToString(argsList[1]), Convert.ToBoolean(argsList[2]));
                        case 2: return StiFunctionsDate.MonthName((DateTime?)(argsList[0]));
                        case 4: return StiFunctionsDate.MonthName((DateTime?)(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 6: return StiFunctionsDate.MonthName((DateTime?)(argsList[0]), Convert.ToString(argsList[1]));
                        case 8: return StiFunctionsDate.MonthName((DateTime?)(argsList[0]), Convert.ToString(argsList[1]), Convert.ToBoolean(argsList[2]));
                            //case 9: return StiFunctionsDate.MonthName((DateTimeOffset)argsList[0]);
                            //case 11: return StiFunctionsDate.MonthName((DateTimeOffset)argsList[0], Convert.ToBoolean(argsList[1]));
                            //case 13: return StiFunctionsDate.MonthName((DateTimeOffset)argsList[0], Convert.ToString(argsList[1]));
                            //case 15: return StiFunctionsDate.MonthName((DateTimeOffset)argsList[0], Convert.ToString(argsList[1]), Convert.ToBoolean(argsList[2]));
                            //case 10: return StiFunctionsDate.MonthName((DateTimeOffset?)argsList[0]);
                            //case 12: return StiFunctionsDate.MonthName((DateTimeOffset?)argsList[0], Convert.ToBoolean(argsList[1]));
                            //case 14: return StiFunctionsDate.MonthName((DateTimeOffset?)argsList[0], Convert.ToString(argsList[1]));
                            //case 16: return StiFunctionsDate.MonthName((DateTimeOffset?)argsList[0], Convert.ToString(argsList[1]), Convert.ToBoolean(argsList[2]));
                    }
                    break;

                case StiFunctionType.WeekOfYear:
                    switch (overload)
                    {
                        case 1: return StiFunctionsDate.WeekOfYear(Convert.ToDateTime(argsList[0]));
                        case 3: return StiFunctionsDate.WeekOfYear(Convert.ToDateTime(argsList[0]), (DayOfWeek)argsList[1]);
                        case 5: return StiFunctionsDate.WeekOfYear(Convert.ToDateTime(argsList[0]), (DayOfWeek)argsList[1], (CalendarWeekRule)argsList[2]);
                        case 2: return StiFunctionsDate.WeekOfYear((DateTime?)argsList[0]);
                        case 4: return StiFunctionsDate.WeekOfYear((DateTime?)argsList[0], (DayOfWeek)argsList[1]);
                        case 6: return StiFunctionsDate.WeekOfYear((DateTime?)argsList[0], (DayOfWeek)argsList[1], (CalendarWeekRule)argsList[2]);
                            //case 7: return StiFunctionsDate.WeekOfYear((DateTimeOffset)argsList[0]);
                            //case 9: return StiFunctionsDate.WeekOfYear((DateTimeOffset)argsList[0], (DayOfWeek)argsList[1]);
                            //case 11: return StiFunctionsDate.WeekOfYear((DateTimeOffset)argsList[0], (DayOfWeek)argsList[1], (CalendarWeekRule)argsList[2]);
                            //case 8: return StiFunctionsDate.WeekOfYear((DateTimeOffset?)argsList[0]);
                            //case 10: return StiFunctionsDate.WeekOfYear((DateTimeOffset?)argsList[0], (DayOfWeek)argsList[1]);
                            //case 12: return StiFunctionsDate.WeekOfYear((DateTimeOffset?)argsList[0], (DayOfWeek)argsList[1], (CalendarWeekRule)argsList[2]);
                    }
                    break;

                case StiFunctionType.WeekOfMonth:
                    switch (overload)
                    {
                        case 1: return StiFunctionsDate.WeekOfMonth(Convert.ToDateTime(argsList[0]));
                        case 3: return StiFunctionsDate.WeekOfMonth(Convert.ToDateTime(argsList[0]), (DayOfWeek)argsList[1]);
                        case 5: return StiFunctionsDate.WeekOfMonth(Convert.ToDateTime(argsList[0]), (DayOfWeek)argsList[1], (CalendarWeekRule)argsList[2]);
                        case 2: return StiFunctionsDate.WeekOfMonth((DateTime?)argsList[0]);
                        case 4: return StiFunctionsDate.WeekOfMonth((DateTime?)argsList[0], (DayOfWeek)argsList[1]);
                        case 6: return StiFunctionsDate.WeekOfMonth((DateTime?)argsList[0], (DayOfWeek)argsList[1], (CalendarWeekRule)argsList[2]);
                            //case 7: return StiFunctionsDate.WeekOfMonth((DateTimeOffset)argsList[0]);
                            //case 9: return StiFunctionsDate.WeekOfMonth((DateTimeOffset)argsList[0], (DayOfWeek)argsList[1]);
                            //case 11: return StiFunctionsDate.WeekOfMonth((DateTimeOffset)argsList[0], (DayOfWeek)argsList[1], (CalendarWeekRule)argsList[2]);
                            //case 8: return StiFunctionsDate.WeekOfMonth((DateTimeOffset?)argsList[0]);
                            //case 10: return StiFunctionsDate.WeekOfMonth((DateTimeOffset?)argsList[0], (DayOfWeek)argsList[1]);
                            //case 12: return StiFunctionsDate.WeekOfMonth((DateTimeOffset?)argsList[0], (DayOfWeek)argsList[1], (CalendarWeekRule)argsList[2]);
                    }
                    break;

                case StiFunctionType.FromOADate:
                    if (overload == 1) return DateTime.FromOADate(Convert.ToDouble(argsList[0]));
                    break;
                case StiFunctionType.ToOADate:
                    if (overload == 1) return Convert.ToDateTime(argsList[0]).ToOADate();
                    break;
                #endregion

                #region Strings
                case StiFunctionType.Insert:
                    if (overload == 1) return StiFunctionsStrings.Insert(argsList[0], Convert.ToInt32(argsList[1]), argsList[2]);
                    break;

                case StiFunctionType.Length:
                    if (overload == 1) return StiFunctionsStrings.Length(argsList[0]);
                    break;

                case StiFunctionType.Remove:
                    if (overload == 1) return StiFunctionsStrings.Remove(argsList[0], Convert.ToInt32(argsList[1]));
                    if (overload == 2) return StiFunctionsStrings.Remove(argsList[0], Convert.ToInt32(argsList[1]), Convert.ToInt32(argsList[2]));
                    break;

                case StiFunctionType.Replace:
                    if (overload == 1) return StiFunctionsStrings.Replace(argsList[0], argsList[1], argsList[2]);
                    break;

                case StiFunctionType.Roman:
                    if (overload == 1) return StiFunctionsStrings.Roman(Convert.ToInt32(argsList[0]));
                    break;

                case StiFunctionType.Substring:
                    if (overload == 1) return StiFunctionsStrings.Substring(argsList[0], Convert.ToInt32(argsList[1]), Convert.ToInt32(argsList[2]));
                    break;

                case StiFunctionType.ToLowerCase:
                    if (overload == 1) return StiFunctionsStrings.ToLowerCase(argsList[0]);
                    break;

                case StiFunctionType.ToProperCase:
                    if (overload == 1) return StiFunctionsStrings.ToProperCase(argsList[0]);
                    break;

                case StiFunctionType.ToUpperCase:
                    if (overload == 1) return StiFunctionsStrings.ToUpperCase(argsList[0]);
                    break;

                case StiFunctionType.Trim:
                    if (overload == 1) return StiFunctionsStrings.Trim(argsList[0]);
                    break;

                case StiFunctionType.TryParseDecimal:
                    if (overload == 1) return StiFunctionsStrings.TryParseDecimal(Convert.ToString(argsList[0]));
                    break;

                case StiFunctionType.TryParseDouble:
                    if (overload == 1) return StiFunctionsStrings.TryParseDouble(Convert.ToString(argsList[0]));
                    break;

                case StiFunctionType.TryParseLong:
                    if (overload == 1) return StiFunctionsStrings.TryParseLong(Convert.ToString(argsList[0]));
                    break;

                case StiFunctionType.Arabic:
                    if (overload == 1) return StiFunctionsStrings.Arabic(Convert.ToInt32(argsList[0]));
                    if (overload == 2) return StiFunctionsStrings.Arabic(Convert.ToString(argsList[0]));
                    break;

                case StiFunctionType.Persian:
                    if (overload == 1) return StiFunctionsStrings.Persian(Convert.ToInt32(argsList[0]));
                    if (overload == 2) return StiFunctionsStrings.Persian(Convert.ToString(argsList[0]));
                    break;

                case StiFunctionType.ToOrdinal:
                    if (overload == 1) return StiFunctionsStrings.ToOrdinal(Convert.ToInt64(argsList[0]));
                    break;

                case StiFunctionType.Left:
                    if (overload == 1) return Dictionary.StiFunctionsStrings.Left(Convert.ToString(argsList[0]), Convert.ToInt32(argsList[1]));
                    break;
                case StiFunctionType.Right:
                    if (overload == 1) return Dictionary.StiFunctionsStrings.Right(Convert.ToString(argsList[0]), Convert.ToInt32(argsList[1]));
                    break;
                case StiFunctionType.Mid:
                    if (overload == 1) return Dictionary.StiFunctionsStrings.Mid(Convert.ToString(argsList[0]), Convert.ToInt32(argsList[1]), Convert.ToInt32(argsList[2]));
                    break;

                case StiFunctionType.ToWords:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToWords(Convert.ToInt64(argsList[0]));
                        case 2: return Dictionary.StiFunctionsStrings.ToWords(Convert.ToDecimal(argsList[0]));
                        case 3: return Dictionary.StiFunctionsStrings.ToWords(Convert.ToDouble(argsList[0]));
                        case 4: return Dictionary.StiFunctionsStrings.ToWords(Convert.ToInt64(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 5: return Dictionary.StiFunctionsStrings.ToWords(Convert.ToDecimal(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 6: return Dictionary.StiFunctionsStrings.ToWords(Convert.ToDouble(argsList[0]), Convert.ToBoolean(argsList[1]));
                    }
                    break;
                case StiFunctionType.ToWordsEs:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToWordsEs(Convert.ToInt64(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 2: return Dictionary.StiFunctionsStrings.ToWordsEs(Convert.ToInt64(argsList[0]), Convert.ToBoolean(argsList[1]), Convert.ToBoolean(argsList[2]));
                    }
                    break;
                case StiFunctionType.ToWordsAr:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToWordsAr(Convert.ToDecimal(argsList[0]));
                        case 2: return Dictionary.StiFunctionsStrings.ToWordsAr(Convert.ToDouble(argsList[0]));
                    }
                    break;
                case StiFunctionType.ToWordsEnIn: return Dictionary.StiFunctionsStrings.ToWordsEnIn(Convert.ToInt64(argsList[0]), Convert.ToBoolean(argsList[1]));
                case StiFunctionType.ToWordsFa: return Dictionary.StiFunctionsStrings.ToWordsFa(Convert.ToInt64(argsList[0]));
                case StiFunctionType.ToWordsPl: return Dictionary.StiFunctionsStrings.ToWordsPl(Convert.ToInt64(argsList[0]), Convert.ToBoolean(argsList[1]));
                case StiFunctionType.ToWordsPt: return Dictionary.StiFunctionsStrings.ToWordsPt(Convert.ToInt64(argsList[0]), Convert.ToBoolean(argsList[1]));
                case StiFunctionType.ToWordsRu:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToWordsRu(Convert.ToInt64(argsList[0]));
                        case 2: return Dictionary.StiFunctionsStrings.ToWordsRu(Convert.ToDecimal(argsList[0]));
                        case 3: return Dictionary.StiFunctionsStrings.ToWordsRu(Convert.ToDouble(argsList[0]));
                        case 4: return Dictionary.StiFunctionsStrings.ToWordsRu(Convert.ToInt64(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 5: return Dictionary.StiFunctionsStrings.ToWordsRu(Convert.ToDecimal(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 6: return Dictionary.StiFunctionsStrings.ToWordsRu(Convert.ToDouble(argsList[0]), Convert.ToBoolean(argsList[1]));
                    }
                    break;
                case StiFunctionType.ToWordsTr: return Dictionary.StiFunctionsStrings.ToWordsTr(Convert.ToDecimal(argsList[0]));
                case StiFunctionType.ToWordsUa:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToWordsUa(Convert.ToInt64(argsList[0]));
                        case 2: return Dictionary.StiFunctionsStrings.ToWordsUa(Convert.ToDecimal(argsList[0]));
                        case 3: return Dictionary.StiFunctionsStrings.ToWordsUa(Convert.ToDouble(argsList[0]));
                        case 4: return Dictionary.StiFunctionsStrings.ToWordsUa(Convert.ToInt64(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 5: return Dictionary.StiFunctionsStrings.ToWordsUa(Convert.ToDecimal(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 6: return Dictionary.StiFunctionsStrings.ToWordsUa(Convert.ToDouble(argsList[0]), Convert.ToBoolean(argsList[1]));
                    }
                    break;
                case StiFunctionType.ToWordsZh:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToWordsZh(Convert.ToDecimal(argsList[0]));
                        case 2: return Dictionary.StiFunctionsStrings.ToWordsZh(Convert.ToDouble(argsList[0]));
                    }
                    break;

                case StiFunctionType.ToCurrencyWords:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToCurrencyWords(Convert.ToInt64(argsList[0]));
                        case 2: return Dictionary.StiFunctionsStrings.ToCurrencyWords(Convert.ToDouble(argsList[0]));
                        case 3: return Dictionary.StiFunctionsStrings.ToCurrencyWords(Convert.ToDecimal(argsList[0]));
                        case 4: return Dictionary.StiFunctionsStrings.ToCurrencyWords(Convert.ToInt64(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 5: return Dictionary.StiFunctionsStrings.ToCurrencyWords(Convert.ToDouble(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 6: return Dictionary.StiFunctionsStrings.ToCurrencyWords(Convert.ToDecimal(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 7: return Dictionary.StiFunctionsStrings.ToCurrencyWords(Convert.ToInt64(argsList[0]), Convert.ToBoolean(argsList[1]), Convert.ToBoolean(argsList[2]));
                        case 8: return Dictionary.StiFunctionsStrings.ToCurrencyWords(Convert.ToDouble(argsList[0]), Convert.ToBoolean(argsList[1]), Convert.ToBoolean(argsList[2]));
                        case 9: return Dictionary.StiFunctionsStrings.ToCurrencyWords(Convert.ToDecimal(argsList[0]), Convert.ToBoolean(argsList[1]), Convert.ToBoolean(argsList[2]));
                        case 10: return Dictionary.StiFunctionsStrings.ToCurrencyWords(Convert.ToDouble(argsList[0]), Convert.ToBoolean(argsList[1]), Convert.ToBoolean(argsList[2]), Convert.ToString(argsList[3]), Convert.ToString(argsList[4]));
                    }
                    break;
                case StiFunctionType.ToCurrencyWordsEnGb:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToCurrencyWordsEnGb(Convert.ToDecimal(argsList[0]), Convert.ToString(argsList[1]), Convert.ToInt32(argsList[2]));
                        case 2: return Dictionary.StiFunctionsStrings.ToCurrencyWordsEnGb(Convert.ToDouble(argsList[0]), Convert.ToString(argsList[1]), Convert.ToInt32(argsList[2]));
                    }
                    break;
                case StiFunctionType.ToCurrencyWordsEnIn:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToCurrencyWordsEnIn(Convert.ToString(argsList[0]), Convert.ToString(argsList[1]), Convert.ToDecimal(argsList[2]), Convert.ToInt32(argsList[3]), Convert.ToBoolean(argsList[4]));
                        case 2: return Dictionary.StiFunctionsStrings.ToCurrencyWordsEnIn(Convert.ToString(argsList[0]), Convert.ToString(argsList[1]), Convert.ToDouble(argsList[2]), Convert.ToInt32(argsList[3]), Convert.ToBoolean(argsList[4]));
                    }
                    break;
                case StiFunctionType.ToCurrencyWordsEs:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToCurrencyWordsEs(Convert.ToDecimal(argsList[0]), Convert.ToString(argsList[1]), Convert.ToInt32(argsList[2]));
                        case 2: return Dictionary.StiFunctionsStrings.ToCurrencyWordsEs(Convert.ToDecimal(argsList[0]), Convert.ToString(argsList[1]), Convert.ToInt32(argsList[2]), Convert.ToBoolean(argsList[3]));
                        case 3: return Dictionary.StiFunctionsStrings.ToCurrencyWordsEs(Convert.ToDouble(argsList[0]), Convert.ToString(argsList[1]), Convert.ToInt32(argsList[2]));
                        case 4: return Dictionary.StiFunctionsStrings.ToCurrencyWordsEs(Convert.ToDouble(argsList[0]), Convert.ToString(argsList[1]), Convert.ToInt32(argsList[2]), Convert.ToBoolean(argsList[3]));
                    }
                    break;
                case StiFunctionType.ToCurrencyWordsAr:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToCurrencyWordsAr(Convert.ToDecimal(argsList[0]), Convert.ToString(argsList[1]), Convert.ToString(argsList[2]));
                        case 2: return Dictionary.StiFunctionsStrings.ToCurrencyWordsAr(Convert.ToDouble(argsList[0]), Convert.ToString(argsList[1]), Convert.ToString(argsList[2]));
                    }
                    break;
                case StiFunctionType.ToCurrencyWordsFr:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToCurrencyWordsFr(Convert.ToDecimal(argsList[0]), Convert.ToString(argsList[1]), Convert.ToInt32(argsList[2]));
                        case 2: return Dictionary.StiFunctionsStrings.ToCurrencyWordsFr(Convert.ToDouble(argsList[0]), Convert.ToString(argsList[1]), Convert.ToInt32(argsList[2]));
                    }
                    break;
                case StiFunctionType.ToCurrencyWordsNl:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToCurrencyWordsNl(Convert.ToDecimal(argsList[0]), Convert.ToString(argsList[1]), Convert.ToInt32(argsList[2]));
                        case 2: return Dictionary.StiFunctionsStrings.ToCurrencyWordsNl(Convert.ToDouble(argsList[0]), Convert.ToString(argsList[1]), Convert.ToInt32(argsList[2]));
                    }
                    break;
                case StiFunctionType.ToCurrencyWordsPl:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToCurrencyWordsPl(Convert.ToDecimal(argsList[0]), Convert.ToString(argsList[1]), Convert.ToBoolean(argsList[2]), Convert.ToBoolean(argsList[3]));
                        case 2: return Dictionary.StiFunctionsStrings.ToCurrencyWordsPl(Convert.ToDouble(argsList[0]), Convert.ToString(argsList[1]), Convert.ToBoolean(argsList[2]), Convert.ToBoolean(argsList[3]));
                    }
                    break;
                case StiFunctionType.ToCurrencyWordsPt:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToCurrencyWordsPt(Convert.ToDecimal(argsList[0]), Convert.ToBoolean(argsList[1]), Convert.ToBoolean(argsList[2]));
                        case 2: return Dictionary.StiFunctionsStrings.ToCurrencyWordsPt(Convert.ToDecimal(argsList[0]), Convert.ToBoolean(argsList[1]), Convert.ToBoolean(argsList[2]), Convert.ToString(argsList[3]), Convert.ToString(argsList[4]));
                        case 3: return Dictionary.StiFunctionsStrings.ToCurrencyWordsPt(Convert.ToDecimal(argsList[0]), Convert.ToBoolean(argsList[1]), Convert.ToBoolean(argsList[2]), Convert.ToString(argsList[3]), Convert.ToInt32(argsList[4]));
                        case 4: return Dictionary.StiFunctionsStrings.ToCurrencyWordsPt(Convert.ToDouble(argsList[0]), Convert.ToBoolean(argsList[1]), Convert.ToBoolean(argsList[2]));
                        case 5: return Dictionary.StiFunctionsStrings.ToCurrencyWordsPt(Convert.ToDouble(argsList[0]), Convert.ToBoolean(argsList[1]), Convert.ToBoolean(argsList[2]), Convert.ToString(argsList[3]), Convert.ToString(argsList[4]));
                        case 6: return Dictionary.StiFunctionsStrings.ToCurrencyWordsPt(Convert.ToDouble(argsList[0]), Convert.ToBoolean(argsList[1]), Convert.ToBoolean(argsList[2]), Convert.ToString(argsList[3]), Convert.ToInt32(argsList[4]));
                    }
                    break;
                case StiFunctionType.ToCurrencyWordsPtBr:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToCurrencyWordsPtBr(Convert.ToDecimal(argsList[0]));
                        case 2: return Dictionary.StiFunctionsStrings.ToCurrencyWordsPtBr(Convert.ToDecimal(argsList[0]), Convert.ToBoolean(argsList[1]), Convert.ToString(argsList[2]), Convert.ToString(argsList[3]));
                        case 3: return Dictionary.StiFunctionsStrings.ToCurrencyWordsPtBr(Convert.ToDouble(argsList[0]));
                        case 4: return Dictionary.StiFunctionsStrings.ToCurrencyWordsPtBr(Convert.ToDouble(argsList[0]), Convert.ToBoolean(argsList[1]), Convert.ToString(argsList[2]), Convert.ToString(argsList[3]));
                    }
                    break;
                case StiFunctionType.ToCurrencyWordsRu:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToCurrencyWordsRu(Convert.ToInt64(argsList[0]));
                        case 2: return Dictionary.StiFunctionsStrings.ToCurrencyWordsRu(Convert.ToDouble(argsList[0]));
                        case 3: return Dictionary.StiFunctionsStrings.ToCurrencyWordsRu(Convert.ToDecimal(argsList[0]));
                        case 4: return Dictionary.StiFunctionsStrings.ToCurrencyWordsRu(Convert.ToInt64(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 5: return Dictionary.StiFunctionsStrings.ToCurrencyWordsRu(Convert.ToDouble(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 6: return Dictionary.StiFunctionsStrings.ToCurrencyWordsRu(Convert.ToDecimal(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 7: return Dictionary.StiFunctionsStrings.ToCurrencyWordsRu(Convert.ToInt64(argsList[0]), Convert.ToBoolean(argsList[1]), Convert.ToString(argsList[2]));
                        case 8: return Dictionary.StiFunctionsStrings.ToCurrencyWordsRu(Convert.ToDouble(argsList[0]), Convert.ToBoolean(argsList[1]), Convert.ToString(argsList[2]));
                        case 9: return Dictionary.StiFunctionsStrings.ToCurrencyWordsRu(Convert.ToDecimal(argsList[0]), Convert.ToBoolean(argsList[1]), Convert.ToString(argsList[2]));
                        case 10: return Dictionary.StiFunctionsStrings.ToCurrencyWordsRu(Convert.ToInt64(argsList[0]), Convert.ToString(argsList[1]), Convert.ToBoolean(argsList[2]));
                        case 11: return Dictionary.StiFunctionsStrings.ToCurrencyWordsRu(Convert.ToDouble(argsList[0]), Convert.ToString(argsList[1]), Convert.ToBoolean(argsList[2]));
                        case 12: return Dictionary.StiFunctionsStrings.ToCurrencyWordsRu(Convert.ToDecimal(argsList[0]), Convert.ToString(argsList[1]), Convert.ToBoolean(argsList[2]));
                    }
                    break;
                case StiFunctionType.ToCurrencyWordsThai:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToCurrencyWordsThai(Convert.ToInt64(argsList[0]));
                        case 2: return Dictionary.StiFunctionsStrings.ToCurrencyWordsThai(Convert.ToDouble(argsList[0]));
                        case 3: return Dictionary.StiFunctionsStrings.ToCurrencyWordsThai(Convert.ToDecimal(argsList[0]));
                    }
                    break;
                case StiFunctionType.ToCurrencyWordsTr:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToCurrencyWordsTr(Convert.ToDecimal(argsList[0]));
                        case 2: return Dictionary.StiFunctionsStrings.ToCurrencyWordsTr(Convert.ToDecimal(argsList[0]), Convert.ToString(argsList[1]), Convert.ToBoolean(argsList[2]));
                    }
                    break;
                case StiFunctionType.ToCurrencyWordsUa:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToCurrencyWordsUa(Convert.ToInt64(argsList[0]));
                        case 2: return Dictionary.StiFunctionsStrings.ToCurrencyWordsUa(Convert.ToDouble(argsList[0]));
                        case 3: return Dictionary.StiFunctionsStrings.ToCurrencyWordsUa(Convert.ToDecimal(argsList[0]));
                        case 4: return Dictionary.StiFunctionsStrings.ToCurrencyWordsUa(Convert.ToInt64(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 5: return Dictionary.StiFunctionsStrings.ToCurrencyWordsUa(Convert.ToDouble(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 6: return Dictionary.StiFunctionsStrings.ToCurrencyWordsUa(Convert.ToDecimal(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 7: return Dictionary.StiFunctionsStrings.ToCurrencyWordsUa(Convert.ToInt64(argsList[0]), Convert.ToBoolean(argsList[1]), Convert.ToString(argsList[2]));
                        case 8: return Dictionary.StiFunctionsStrings.ToCurrencyWordsUa(Convert.ToDouble(argsList[0]), Convert.ToBoolean(argsList[1]), Convert.ToString(argsList[2]));
                        case 9: return Dictionary.StiFunctionsStrings.ToCurrencyWordsUa(Convert.ToDecimal(argsList[0]), Convert.ToBoolean(argsList[1]), Convert.ToString(argsList[2]));
                    }
                    break;
                case StiFunctionType.ToCurrencyWordsZh:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.ToCurrencyWordsZh(Convert.ToDecimal(argsList[0]));
                        case 2: return Dictionary.StiFunctionsStrings.ToCurrencyWordsZh(Convert.ToDouble(argsList[0]));
                    }
                    break;

                case StiFunctionType.DateToStr:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.DateToStr(Convert.ToDateTime(argsList[0]));
                        case 3: return Dictionary.StiFunctionsStrings.DateToStr(Convert.ToDateTime(argsList[0]), Convert.ToBoolean(argsList[1]));
                        case 2: return Dictionary.StiFunctionsStrings.DateToStr((DateTime?)(argsList[0]));
                        case 4: return Dictionary.StiFunctionsStrings.DateToStr((DateTime?)(argsList[0]), Convert.ToBoolean(argsList[1]));
                    }
                    break;
                case StiFunctionType.DateToStrPl: return Dictionary.StiFunctionsStrings.DateToStrPl(Convert.ToDateTime(argsList[0]), Convert.ToBoolean(argsList[1]));
                case StiFunctionType.DateToStrRu:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.DateToStrRu(Convert.ToDateTime(argsList[0]));
                        case 2: return Dictionary.StiFunctionsStrings.DateToStrRu(Convert.ToDateTime(argsList[0]), Convert.ToBoolean(argsList[1]));
                    }
                    break;
                case StiFunctionType.DateToStrUa:
                    switch (overload)
                    {
                        case 1: return Dictionary.StiFunctionsStrings.DateToStrUa(Convert.ToDateTime(argsList[0]));
                        case 2: return Dictionary.StiFunctionsStrings.DateToStrUa(Convert.ToDateTime(argsList[0]), Convert.ToBoolean(argsList[1]));
                    }
                    break;
                case StiFunctionType.DateToStrPt: return Dictionary.StiFunctionsStrings.DateToStrPt(Convert.ToDateTime(argsList[0]));
                case StiFunctionType.DateToStrPtBr: return Dictionary.StiFunctionsStrings.DateToStrPtBr(Convert.ToDateTime(argsList[0]));

                case StiFunctionType.StringIsNullOrEmpty:
                    if (overload == 1) return string.IsNullOrEmpty(Convert.ToString(argsList[0]));
                    break;
                case StiFunctionType.StringIsNullOrWhiteSpace:
                    if (overload == 1) return string.IsNullOrWhiteSpace(Convert.ToString(argsList[0]));
                    break;

                case StiFunctionType.StrToDateTime:
                    switch (overload)
                    {
                        case 1: return StiFunctionsStrings.StrToDateTime(Convert.ToString(argsList[0]));
                    }
                    break;
                case StiFunctionType.StrToNullableDateTime:
                    switch (overload)
                    {
                        case 1: return StiFunctionsStrings.StrToNullableDateTime(Convert.ToString(argsList[0]));
                    }
                    break;

                case StiFunctionType.ConvertToBase64String:
                    switch (overload)
                    {
                        case 1: return StiFunctionsStrings.ConvertToBase64String(Convert.ToString(argsList[0]));
                    }
                    break;
                #endregion

                #region PrintState
                case StiFunctionType.IsNull:
                    switch (overload)
                    {
                        case 1: return StiFunctionsPrintState.IsNull(argsList[0], Convert.ToString(argsList[1]));
                    }
                    break;
                case StiFunctionType.Next:
                    switch (overload)
                    {
                        case 1: return StiFunctionsPrintState.Next(argsList[0], Convert.ToString(argsList[1]));
                    }
                    break;
                case StiFunctionType.NextIsNull:
                    switch (overload)
                    {
                        case 1: return StiFunctionsPrintState.NextIsNull(argsList[0], Convert.ToString(argsList[1]));
                    }
                    break;
                case StiFunctionType.Previous:
                    switch (overload)
                    {
                        case 1: return StiFunctionsPrintState.Previous(argsList[0], Convert.ToString(argsList[1]));
                    }
                    break;
                case StiFunctionType.PreviousIsNull:
                    switch (overload)
                    {
                        case 1: return StiFunctionsPrintState.PreviousIsNull(argsList[0], Convert.ToString(argsList[1]));
                    }
                    break;
                #endregion

                #region Programming
                case StiFunctionType.IIF:
                    switch (overload)
                    {
                        case 1: return Convert.ToInt32(Convert.ToBoolean(argsList[0]) ? argsList[1] : argsList[2]);
                        case 2: return Convert.ToInt64(Convert.ToBoolean(argsList[0]) ? argsList[1] : argsList[2]);
                        case 3: return Convert.ToDouble(Convert.ToBoolean(argsList[0]) ? argsList[1] : argsList[2]);
                        case 4: return Convert.ToDecimal(Convert.ToBoolean(argsList[0]) ? argsList[1] : argsList[2]);
                        case 5: return Convert.ToChar(Convert.ToBoolean(argsList[0]) ? argsList[1] : argsList[2]);
                        case 6: return Convert.ToString(Convert.ToBoolean(argsList[0]) ? argsList[1] : argsList[2]);
                        case 7: return Convert.ToDateTime(Convert.ToBoolean(argsList[0]) ? argsList[1] : argsList[2]);
                        case 8: return Convert.ToBoolean(argsList[0]) ? argsList[1] : argsList[2];
                    }
                    break;

                case StiFunctionType.Choose:
                    if (argsList.Count < 2) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "Choose", argsList.Count.ToString());
                    category = get_category(argsList[0]);
                    if (category < 4 || category > 7) ThrowError(ParserErrorCode.FunctionHasInvalidArgument, "Choose", "1", GetTypeName(argsList[0]), "int");
                    int chooseIndex = Convert.ToInt32(argsList[0]);
                    if (chooseIndex > 0 && chooseIndex < argsList.Count) return argsList[chooseIndex];
                    return null;

                case StiFunctionType.Switch:
                    if (argsList.Count < 2) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "Switch", argsList.Count.ToString());
                    int switchIndex = 0;
                    while (switchIndex + 1 < argsList.Count)
                    {
                        if (Convert.ToBoolean(argsList[switchIndex])) return argsList[switchIndex + 1];
                        switchIndex += 2;
                    }
                    return null;
                #endregion

                #region ToString
                case StiFunctionType.ToString:
                    if (argsList[0] == null || argsList[0] == DBNull.Value) return string.Empty;
                    category = get_category(argsList[0]);
                    if (category == 1)
                    {
                        return Convert.ToString(argsList[0]);
                    }
                    else if (category == 2 || category == 3)
                    {
                        decimal resDecimal = Convert.ToDecimal(argsList[0]);
                        if (argsList.Count == 1) return resDecimal.ToString();
                        else return resDecimal.ToString(Convert.ToString(argsList[1]));
                    }
                    else if (category == 4 || category == 6)
                    {
                        ulong resUlong = Convert.ToUInt64(argsList[0]);
                        if (argsList.Count == 1) return resUlong.ToString();
                        else return resUlong.ToString(Convert.ToString(argsList[1]));
                    }
                    else if (category == 5 || category == 7)
                    {
                        long resLong = Convert.ToInt64(argsList[0]);
                        if (argsList.Count == 1) return resLong.ToString();
                        else return resLong.ToString(Convert.ToString(argsList[1]));
                    }
                    else if (category == 8)
                    {
                        DateTime resDate = Convert.ToDateTime(argsList[0]);
                        if (argsList.Count == 1) return resDate.ToString();
                        else return resDate.ToString(Convert.ToString(argsList[1]));
                    }
                    else if (category == 8)
                    {
                        return Convert.ToBoolean(argsList[0]).ToString();
                    }
                    else return argsList[0].ToString();
                #endregion

                #region Format
                case StiFunctionType.Format:
                    switch (overload)
                    {
                        case 1: return string.Format(Convert.ToString(argsList[0]), argsList[1]);
                        case 2: return string.Format(Convert.ToString(argsList[0]), argsList[1], argsList[2]);
                        case 3: return string.Format(Convert.ToString(argsList[0]), argsList[1], argsList[2], argsList[3]);
                        case 4: return string.Format(Convert.ToString(argsList[0]), argsList[1], argsList[2], argsList[3], argsList[4]);
                        case 5: return string.Format(Convert.ToString(argsList[0]), argsList[1], argsList[2], argsList[3], argsList[4], argsList[5]);
                        case 6: return string.Format(Convert.ToString(argsList[0]), argsList[1], argsList[2], argsList[3], argsList[4], argsList[5], argsList[6]);
                        case 7: return string.Format(Convert.ToString(argsList[0]), argsList[1], argsList[2], argsList[3], argsList[4], argsList[5], argsList[6], argsList[7]);
                    }
                    break;
                #endregion

                #region System.Convert
                case StiFunctionType.SystemConvertToBoolean:
                    if (argsList.Count != 1) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "System.Convert.ToBoolean", argsList.Count.ToString());
                    return Convert.ToBoolean(argsList[0]);

                case StiFunctionType.SystemConvertToByte:
                    if (argsList.Count != 1) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "System.Convert.ToByte", argsList.Count.ToString());
                    return Convert.ToByte(argsList[0]);

                case StiFunctionType.SystemConvertToChar:
                    if (argsList.Count != 1) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "System.Convert.ToChar", argsList.Count.ToString());
                    return Convert.ToChar(argsList[0]);

                case StiFunctionType.SystemConvertToDateTime:
                    if (argsList.Count != 1) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "System.Convert.ToDateTime", argsList.Count.ToString());
                    return Convert.ToDateTime(argsList[0]);

                case StiFunctionType.SystemConvertToDecimal:
                    if (argsList.Count != 1) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "System.Convert.ToDecimal", argsList.Count.ToString());
                    return Convert.ToDecimal(argsList[0]);

                case StiFunctionType.SystemConvertToDouble:
                    if (argsList.Count != 1) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "System.Convert.ToDouble", argsList.Count.ToString());
                    return Convert.ToDouble(argsList[0]);

                case StiFunctionType.SystemConvertToInt16:
                    if (argsList.Count != 1) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "System.Convert.ToInt16", argsList.Count.ToString());
                    return Convert.ToInt16(argsList[0]);

                case StiFunctionType.SystemConvertToInt32:
                    if (argsList.Count != 1) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "System.Convert.ToInt32", argsList.Count.ToString());
                    return Convert.ToInt32(argsList[0]);

                case StiFunctionType.SystemConvertToInt64:
                    if (argsList.Count != 1) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "System.Convert.ToInt64", argsList.Count.ToString());
                    return Convert.ToInt64(argsList[0]);

                case StiFunctionType.SystemConvertToSByte:
                    if (argsList.Count != 1) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "System.Convert.ToSByte", argsList.Count.ToString());
                    return Convert.ToSByte(argsList[0]);

                case StiFunctionType.SystemConvertToSingle:
                    if (argsList.Count != 1) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "System.Convert.ToSingle", argsList.Count.ToString());
                    return Convert.ToSingle(argsList[0]);

                case StiFunctionType.SystemConvertToString:
                    if (argsList.Count != 1) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "System.Convert.ToString", argsList.Count.ToString());
                    return Convert.ToString(argsList[0]);

                case StiFunctionType.SystemConvertToUInt16:
                    if (argsList.Count != 1) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "System.Convert.ToUInt16", argsList.Count.ToString());
                    return Convert.ToUInt16(argsList[0]);

                case StiFunctionType.SystemConvertToUInt32:
                    if (argsList.Count != 1) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "System.Convert.ToUInt32", argsList.Count.ToString());
                    return Convert.ToUInt32(argsList[0]);

                case StiFunctionType.SystemConvertToUInt64:
                    if (argsList.Count != 1) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "System.Convert.ToUInt64", argsList.Count.ToString());
                    return Convert.ToUInt64(argsList[0]);
                #endregion

                #region System.Math
                case StiFunctionType.MathRound:
                    switch (overload)
                    {
                        case 1: return Math.Round(Convert.ToDouble(argsList[0]));
                        case 2: return Math.Round(Convert.ToDouble(argsList[0]), (MidpointRounding)(argsList[1]));
                        case 3: return Math.Round(Convert.ToDouble(argsList[0]), Convert.ToInt32(argsList[1]));
                        case 4: return Math.Round(Convert.ToDouble(argsList[0]), Convert.ToInt32(argsList[1]), (MidpointRounding)(argsList[2]));
                        case 5: return Math.Round(Convert.ToDecimal(argsList[0]));
                        case 6: return Math.Round(Convert.ToDecimal(argsList[0]), (MidpointRounding)(argsList[1]));
                        case 7: return Math.Round(Convert.ToDecimal(argsList[0]), Convert.ToInt32(argsList[1]));
                        case 8: return Math.Round(Convert.ToDecimal(argsList[0]), Convert.ToInt32(argsList[1]), (MidpointRounding)(argsList[2]));
                    }
                    break;

                case StiFunctionType.MathPow:
                    switch (overload)
                    {
                        case 1: return Math.Pow(Convert.ToDouble(argsList[0]), Convert.ToDouble(argsList[1]));
                    }
                    break;
                #endregion

                #region ConvertRtf
                case StiFunctionType.ConvertRtf:
                    if (argsList.Count < 1) ThrowError(ParserErrorCode.NoOverloadForMethodTakesNArguments, "ConvertRtf", argsList.Count.ToString());
                    object[] objList = new object[argsList.Count];
                    Array.Copy(argsList.ToArray(), objList, argsList.Count);
                    object obj = typeof(StiReport).InvokeMember("ConvertRtf", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, report, objList);
                    return Convert.ToString(obj);
                #endregion

                #region GetAnchorPageNumber
                case StiFunctionType.GetAnchorPageNumber:
                    switch (overload)
                    {
                        case 1: return report.GetAnchorPageNumber(argsList[0]);
                    }
                    break;
                case StiFunctionType.GetAnchorPageNumberThrough:
                    switch (overload)
                    {
                        case 1: return report.GetAnchorPageNumberThrough(argsList[0]);
                    }
                    break;
                #endregion

                #region Parse
                case StiFunctionType.ParseTimeSpan:
                    switch (overload)
                    {
                        case 1: 
                            return StiValueHelper.TryToTimeSpan(Convert.ToString(argsList[0]));
                    }
                    break;

                case StiFunctionType.ParseDateTime:
                    switch (overload)
                    {
                        case 1: 
                            return StiValueHelper.TryToDateTime(Convert.ToString(argsList[0]));
                    }
                    break;

                case StiFunctionType.ParseDateTimeOffset:
                    switch (overload)
                    {
                        case 1:
                            return StiValueHelper.TryToDateTimeOffset(Convert.ToString(argsList[0]));
                    }
                    break;

                case StiFunctionType.ParseDecimal:
                    switch (overload)
                    {
                        case 1: 
                            return StiValueHelper.TryToDecimal(Convert.ToString(argsList[0]));
                    }
                    break;

                case StiFunctionType.ParseDouble:
                    switch (overload)
                    {
                        case 1: 
                            return StiValueHelper.TryToDouble(Convert.ToString(argsList[0]));
                    }
                    break;

                case StiFunctionType.ParseInt:
                    switch (overload)
                    {
                        case 1: 
                            return StiValueHelper.TryToInt(Convert.ToString(argsList[0]));
                    }
                    break;

                case StiFunctionType.ParseInt64:
                    switch (overload)
                    {
                        case 1: 
                            return StiValueHelper.TryToLong(Convert.ToString(argsList[0]));
                    }
                    break;
                #endregion

                #region EngineHelper
                case StiFunctionType.EngineHelperJoinColumnContent:
                    switch (overload)
                    {
                        case 1: return Func.EngineHelper.JoinColumnContent((StiDataSource)argsList[0], Convert.ToString(argsList[1]), Convert.ToString(argsList[2]));
                        case 2: return Func.EngineHelper.JoinColumnContent((StiBusinessObject)argsList[0], Convert.ToString(argsList[1]), Convert.ToString(argsList[2]));
                        case 3: return Func.EngineHelper.JoinColumnContent((StiDataSource)argsList[0], Convert.ToString(argsList[1]), Convert.ToString(argsList[2]), Convert.ToBoolean(argsList[3]));
                        case 4: return Func.EngineHelper.JoinColumnContent((StiBusinessObject)argsList[0], Convert.ToString(argsList[1]), Convert.ToString(argsList[2]), Convert.ToBoolean(argsList[3]));
                    }
                    break;
                case StiFunctionType.EngineHelperToQueryString:
                    if (overload > 0 && argsList[0] is IEnumerable)
                    {
                        List<object> list = new List<object>();
                        try
                        {
                            IEnumerator enumerator = ((IEnumerable)argsList[0]).GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                list.Add(enumerator.Current);
                            }
                        }
                        catch
                        {
                        }
                        switch (overload)
                        {
                            case 1: return Func.EngineHelper.ToQueryString(list, Convert.ToString(argsList[1]), Convert.ToString(argsList[2]));
                            case 2: return Func.EngineHelper.ToQueryString(list, Convert.ToString(argsList[1]), Convert.ToString(argsList[2]), Convert.ToBoolean(argsList[3]));
                        }
                    }
                    break;
                case StiFunctionType.EngineHelperGetRealPageNumber:
                    switch (overload)
                    {
                        case 1: return Func.EngineHelper.GetRealPageNumber(argsList[0]);
                    }
                    break;
                #endregion

                #region StiNullValuesHelper
                case StiFunctionType.StiNullValuesHelperIsNull:
                    switch (overload)
                    {
                        case 1: return StiNullValuesHelper.IsNull(argsList[0] as StiReport, Convert.ToString(argsList[1]));
                    }
                    break;
                #endregion

                #region TimeSpan
                case StiFunctionType.TimeSpanFromDays:
                    switch (overload)
                    {
                        case 1: return TimeSpan.FromDays(Convert.ToDouble(argsList[0]));
                    }
                    break;
                case StiFunctionType.TimeSpanFromHours:
                    switch (overload)
                    {
                        case 1: return TimeSpan.FromHours(Convert.ToDouble(argsList[0]));
                    }
                    break;
                case StiFunctionType.TimeSpanFromMilliseconds:
                    switch (overload)
                    {
                        case 1: return TimeSpan.FromMilliseconds(Convert.ToDouble(argsList[0]));
                    }
                    break;
                case StiFunctionType.TimeSpanFromMinutes:
                    switch (overload)
                    {
                        case 1: return TimeSpan.FromMinutes(Convert.ToDouble(argsList[0]));
                    }
                    break;
                case StiFunctionType.TimeSpanFromSeconds:
                    switch (overload)
                    {
                        case 1: return TimeSpan.FromSeconds(Convert.ToDouble(argsList[0]));
                    }
                    break;
                case StiFunctionType.TimeSpanFromTicks:
                    switch (overload)
                    {
                        case 1: return TimeSpan.FromTicks(Convert.ToInt64(argsList[0]));
                    }
                    break;
                #endregion

                #region Image
                case StiFunctionType.ImageFromFile:
                    switch (overload)
                    {
                        case 1: return Image.FromFile(Convert.ToString(argsList[0]));
                    }
                    break;
                #endregion

                #region NewType
                case StiFunctionType.NewType:
                    switch (overload)
                    {
                        case 1: return new DateTime();
                        case 2: return new DateTime(Convert.ToInt64(argsList[1]));
                        case 3: return new DateTime(Convert.ToInt32(argsList[1]), Convert.ToInt32(argsList[2]), Convert.ToInt32(argsList[3]));
                        case 4: return new DateTime(Convert.ToInt32(argsList[1]), Convert.ToInt32(argsList[2]), Convert.ToInt32(argsList[3]), Convert.ToInt32(argsList[4]), Convert.ToInt32(argsList[5]), Convert.ToInt32(argsList[6]));
                        case 5: return new DateTime(Convert.ToInt32(argsList[1]), Convert.ToInt32(argsList[2]), Convert.ToInt32(argsList[3]), Convert.ToInt32(argsList[4]), Convert.ToInt32(argsList[5]), Convert.ToInt32(argsList[6]), Convert.ToInt32(argsList[7]));
                    }
                    break;
                    #endregion

                case StiFunctionType.GetLabel:
                    return report.GetLabel(Convert.ToString(argsList[0]));

                case StiFunctionType.GetParam:
                    return report.GetParam(Convert.ToString(argsList[0]));
            }

            if ((functionType >= StiFunctionType.rCount && functionType <= StiFunctionType.rLast) ||
                (functionType >= StiFunctionType.riCount && functionType <= StiFunctionType.riLast) ||
                (functionType >= StiFunctionType.cCount && functionType <= StiFunctionType.cLast) ||
                (functionType >= StiFunctionType.crCount && functionType <= StiFunctionType.crLast) ||
                (functionType >= StiFunctionType.ciCount && functionType <= StiFunctionType.ciLast) ||
                (functionType >= StiFunctionType.criCount && functionType <= StiFunctionType.criLast))
            {
                ThrowError(ParserErrorCode.FunctionNotYetImplemented, functionType.ToString());
            }

            #region UserFunctions
            if (functionType >= StiFunctionType.UserFunction)
            {
                //find function name
                string functionName = null;
                foreach (DictionaryEntry de in UserFunctionsList)
                {
                    if ((StiFunctionType)de.Value == functionType)
                    {
                        functionName = (string)de.Key;
                        break;
                    }
                }
                if (functionName != null)
                {
                    //prepare arrays
                    int count = argsList.Count;
                    Type[] types = new Type[count];
                    object[] args = new object[count];
                    for (int index = 0; index < count; index++)
                    {
                        args[index] = argsList[index];
                        if (args[index] is StiRefVariableObject)
                        {
                            args[index] = (args[index] as StiRefVariableObject).Value;
                        }
                        if (args[index] == null)
                        {
                            types[index] = typeof(object);
                        }
                        else
                        {
                            types[index] = args[index].GetType();
                        }
                    }

                    //find function by name and arguments
                    var functions = StiFunctions.GetFunctions(false);
                    foreach (StiFunction func in functions)
                    {
                        if (func.FunctionName != functionName) continue;
                        if ((func.ArgumentTypes != null ? func.ArgumentTypes.Length : 0) != count) continue;

                        //check arguments type
                        bool flag2 = true;
                        for (int index = 0; index < count; index++)
                        {
                            if (IsImplicitlyCastableTo(types[index], func.ArgumentTypes[index], args[index])) continue;
                            flag2 = false;
                            break;
                        }
                        if (flag2)
                        {
                            var methods = func.TypeOfFunction.GetMethods().Where(m => m.Name == func.FunctionName);
                            var method = methods.Count() > 1
                                ? methods.Where(m => IsMethodParametersEqual(m, func.ArgumentTypes)).FirstOrDefault()
                                : methods.FirstOrDefault();

                            try
                            {
                                var result = method?.Invoke(report, args);

                                for (int index = 0; index < count; index++)
                                {
                                    StiRefVariableObject refVar = argsList[index] as StiRefVariableObject;
                                    if (refVar != null)
                                    {
                                        report[refVar.Name] = args[index];
                                    }
                                }

                                return result;
                            }
                            catch (Exception ex)
                            {
                                throw new StiCustomFunctionException((ex.InnerException ?? ex).Message, functionName);
                            }
                        }
                    }
                }
            }
            #endregion

            return null;
        }

        private bool IsMethodParametersEqual(MethodInfo method, Type[] argumentTypes)
        {
            return method.GetParameters().Select(p => p.ParameterType).SequenceEqual(argumentTypes);
        }
        #endregion
    }
}
