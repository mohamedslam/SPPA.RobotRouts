﻿#region Copyright (C) 2003-2022 Stimulsoft
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

namespace Stimulsoft.Report.Engine
{
    public partial class StiParser
    {
        #region Enums
        public enum StiTokenType
        {
            /// <summary>
            /// Empty token
            /// </summary>
            Empty = 0,
            Delimiter,
            Variable,
            SystemVariable,
            DataSourceField,
            BusinessObjectField,
            Number,
            Function,   //args
            Method,     //parent + args
            Property,   //parent
            Component,
            Cast,
            String,
            Char,
            RefVariable,
            New,

            /// <summary>
            /// .
            /// </summary>
            Dot,
            /// <summary>
            /// ,
            /// </summary>
            Comma,
            /// <summary>
            /// :
            /// </summary>
            Colon,
            /// <summary>
            /// ;
            /// </summary>
            SemiColon,
            /// <summary>
            /// Shift to the left Token.
            /// </summary>
            Shl,
            /// <summary>
            /// Shift to the right Token.
            /// </summary>
            Shr,
            /// <summary>
            /// = Assign Token.
            /// </summary>
            Assign,
            /// <summary>
            /// Equal Token.
            /// </summary>
            Equal,
            /// <summary>
            /// NotEqual Token.
            /// </summary>
            NotEqual,
            /// <summary>
            /// LeftEqual Token.
            /// </summary>
            LeftEqual,
            /// <summary>
            /// Left Token.
            /// </summary>
            Left,
            /// <summary>
            /// RightEqual Token.
            /// </summary>
            RightEqual,
            /// <summary>
            /// Right Token.
            /// </summary>
            Right,
            /// <summary>
            /// Logical NOT Token.
            /// </summary>
            Not,
            /// <summary>
            /// Logical OR Token.
            /// </summary>
            Or,
            /// <summary>
            /// Logical AND Token.
            /// </summary>
            And,
            /// <summary>
            /// ^
            /// </summary>
            Xor,
            /// <summary>
            /// Double logical OR Token.
            /// </summary>
            DoubleOr,
            /// <summary>
            /// Double logical AND Token.
            /// </summary>
            DoubleAnd,
            ///// <summary>
            ///// Copyright
            ///// </summary>
            //Copyright,
            /// <summary>
            /// ?
            /// </summary>
            Question,
            /// <summary>
            /// +
            /// </summary>
            Plus,
            /// <summary>
            /// -
            /// </summary>
            Minus,
            /// <summary>
            /// *
            /// </summary>
            Mult,
            /// <summary>
            /// /
            /// </summary>
            Div,
            ///// <summary>
            ///// \
            ///// </summary>
            //Splash,
            /// <summary>
            /// %
            /// </summary>
            Percent,
            ///// <summary>
            ///// @
            ///// </summary>
            //Ampersand,
            ///// <summary>
            ///// #
            ///// </summary>
            //Sharp,
            ///// <summary>
            ///// $
            ///// </summary>
            //Dollar,
            ///// <summary>
            ///// €
            ///// </summary>
            //Euro,
            ///// <summary>
            ///// ++
            ///// </summary>
            //DoublePlus,
            ///// <summary>
            ///// --
            ///// </summary>
            //DoubleMinus,
            /// <summary>
            /// (
            /// </summary>
            LParenthesis,
            /// <summary>
            /// )
            /// </summary>
            RParenthesis,
            ///// <summary>
            ///// {
            ///// </summary>
            //LBrace,
            ///// <summary>
            ///// }
            ///// </summary>
            //RBrace,
            /// <summary>
            /// [
            /// </summary>
            LBracket,
            /// <summary>
            /// ]
            /// </summary>
            RBracket,
            ///// <summary>
            ///// Token contains value.
            ///// </summary>
            //Value,
            /// <summary>
            /// Token contains identifier.
            /// </summary>
            Identifier,
            /// <summary>
            /// 
            /// </summary>
            Unknown,
            ///// <summary>
            ///// EOF Token.
            ///// </summary>
            //EOF
        }

        public enum StiAsmCommandType
        {
            PushValue       = 2000,
            PushVariable,
            PushRefVariable,
            PushSystemVariable,
            PushDataSourceField,
            PushBusinessObjectField,
            PushFunction,
            PushMethod,
            PushProperty,
            PushComponent,
            PushArrayElement,
            CopyToVariable,
            Add             = 2020,
            Sub,
            Mult,
            Div,
            Mod,
            Power,
            Neg,
            Cast,
            Not,
            CompareLeft,
            CompareLeftEqual,
            CompareRight,
            CompareRightEqual,
            CompareEqual,
            CompareNotEqual,
            Shl,
            Shr,
            And,
            And2,
            Or,
            Or2,
            Xor,
            Jump,
            JumpTrue,
            JumpFalse
        }

        public enum StiSystemVariableType
        {
            Column,
            Line,
            LineThrough,
            LineABC,
            LineRoman,
            GroupLine,
            PageNumber,
            PageNumberThrough,
            PageNofM,
            PageNofMThrough,
            TotalPageCount,
            TotalPageCountThrough,
            IsFirstPage,
            IsFirstPageThrough,
            IsLastPage,
            IsLastPageThrough,
            PageCopyNumber,
            ReportAlias,
            ReportAuthor,
            ReportChanged,
            ReportCreated,
            ReportDescription,
            ReportName,
            Time,
            Today,
            ConditionValue, //only for CrossTab condition
            ConditionValue2, //only for CrossTab condition
            Index,//only for CrossTab 
            ConditionTag,   //only for condition
            Sender,

            DateTimeNow,
            DateTimeToday,
            DateTimeUtcNow
        }

        public enum StiPropertyType
        {
            Year,
            Month,
            Day,
            Hour,
            Minute,
            Second,
            Date,
            DayOfWeek,

            Length,
            From,
            To,
            FromDate,
            ToDate,
            FromTime,
            ToTime,
            SelectedLine,
            Name,
            TagValue,

            Days,
            Hours,
            Milliseconds,
            Minutes,
            Seconds,
            Ticks,
            TotalDays,
            TotalHours,
            TotalMinutes,
            TotalSeconds,
            TotalMilliseconds,

            Count,
            BusinessObjectValue,
            Position,
            Line,
            Rows,

            Checked,
            SelectedIndex,
            SelectedValue,
            SelectedItem,
            SelectedLabel,
            Text,
            Value,
            Enabled,
            Skip            
        }

        //[Flags]
        //public enum StiParserDataType
        //{
        //    None = 0x0000,
        //    Object = 0x0001,
        //    Object = 0x7FFFFFFF,
        //    zFloat = 0x0002,
        //    zDouble = 0x0004,
        //    zDecimal = 0x0008,
        //    Byte = 0x0010,
        //    SByte = 0x0020,
        //    Int16 = 0x0040,
        //    UInt16 = 0x0080,
        //    Int32 = 0x0100,
        //    UInt32 = 0x0200,
        //    Int64 = 0x0400,
        //    UInt64 = 0x0800,
        //    Bool = 0x1000,
        //    Char = 0x2000,
        //    String = 0x4000,
        //    DateTime = 0x8000,
        //    TimeSpan = 0x00010000,
        //    Image = 0x00020000,

        //    Short = Byte | SByte | Int16,
        //    UShort = Byte | UInt16 | Char,
        //    Int = Short | UInt16 | Int32,
        //    UInt = UShort | UInt32,
        //    Long = Int | UInt32 | Int64,
        //    ULong = UInt | UInt64,
        //    Float = zFloat | Long | ULong,
        //    Double = zDouble | Long | ULong | zFloat,
        //    Decimal = zDecimal | Long | ULong,

        //    BasedType = 0x10000000,
        //    FixedType = 0x20000000,
        //    Nullable = 0x40000000
        //}

        public enum StiFunctionType
        {
            NameSpace = 0,

            #region Totals Functions
            Count,
            CountDistinct,
            Avg,
            AvgD,
            AvgDate,
            AvgI,
            AvgTime,
            Max,
            MaxD,
            MaxDate,
            MaxI,
            MaxStr,
            MaxTime,
            Median,
            MedianD,
            MedianI,
            Min,
            MinD,
            MinDate,
            MinI,
            MinStr,
            MinTime,
            Mode,
            ModeD,
            ModeI,
            Sum,
            SumD,
            SumDistinct,
            SumI,
            SumTime,
            First,
            Last,

            rCount,
            rCountDistinct,
            rAvg,
            rAvgD,
            rAvgDate,
            rAvgI,
            rAvgTime,
            rMax,
            rMaxD,
            rMaxDate,
            rMaxI,
            rMaxStr,
            rMaxTime,
            rMedian,
            rMedianD,
            rMedianI,
            rMin,
            rMinD,
            rMinDate,
            rMinI,
            rMinStr,
            rMinTime,
            rMode,
            rModeD,
            rModeI,
            rSum,
            rSumD,
            rSumDistinct,
            rSumI,
            rSumTime,
            rFirst,
            rLast,

            iCount,
            iCountDistinct,
            iAvg,
            iAvgD,
            iAvgDate,
            iAvgI,
            iAvgTime,
            iMax,
            iMaxD,
            iMaxDate,
            iMaxI,
            iMaxStr,
            iMaxTime,
            iMedian,
            iMedianD,
            iMedianI,
            iMin,
            iMinD,
            iMinDate,
            iMinI,
            iMinStr,
            iMinTime,
            iMode,
            iModeD,
            iModeI,
            iSum,
            iSumD,
            iSumDistinct,
            iSumI,
            iSumTime,
            iFirst,
            iLast,

            riCount,
            riCountDistinct,
            riAvg,
            riAvgD,
            riAvgDate,
            riAvgI,
            riAvgTime,
            riMax,
            riMaxD,
            riMaxDate,
            riMaxI,
            riMaxStr,
            riMaxTime,
            riMedian,
            riMedianD,
            riMedianI,
            riMin,
            riMinD,
            riMinDate,
            riMinI,
            riMinStr,
            riMinTime,
            riMode,
            riModeD,
            riModeI,
            riSum,
            riSumD,
            riSumDistinct,
            riSumI,
            riSumTime,
            riFirst,
            riLast,

            cCount,
            cCountDistinct,
            cAvg,
            cAvgD,
            cAvgDate,
            cAvgI,
            cAvgTime,
            cMax,
            cMaxD,
            cMaxDate,
            cMaxI,
            cMaxStr,
            cMaxTime,
            cMedian,
            cMedianD,
            cMedianI,
            cMin,
            cMinD,
            cMinDate,
            cMinI,
            cMinStr,
            cMinTime,
            cMode,
            cModeD,
            cModeI,
            cSum,
            cSumD,
            cSumDistinct,
            cSumI,
            cSumTime,
            cFirst,
            cLast,
            
            crCount,
            crCountDistinct,
            crAvg,
            crAvgD,
            crAvgDate,
            crAvgI,
            crAvgTime,
            crMax,
            crMaxD,
            crMaxDate,
            crMaxI,
            crMaxStr,
            crMaxTime,
            crMedian,
            crMedianD,
            crMedianI,
            crMin,
            crMinD,
            crMinDate,
            crMinI,
            crMinStr,
            crMinTime,
            crMode,
            crModeD,
            crModeI,
            crSum,
            crSumD,
            crSumDistinct,
            crSumI,
            crSumTime,
            crFirst,
            crLast,
            
            ciCount,
            ciCountDistinct,
            ciAvg,
            ciAvgD,
            ciAvgDate,
            ciAvgI,
            ciAvgTime,
            ciMax,
            ciMaxD,
            ciMaxDate,
            ciMaxI,
            ciMaxStr,
            ciMaxTime,
            ciMedian,
            ciMedianD,
            ciMedianI,
            ciMin,
            ciMinD,
            ciMinDate,
            ciMinI,
            ciMinStr,
            ciMinTime,
            ciMode,
            ciModeD,
            ciModeI,
            ciSum,
            ciSumD,
            ciSumDistinct,
            ciSumI,
            ciSumTime,
            ciFirst,
            ciLast,
            
            criCount,
            criCountDistinct,
            criAvg,
            criAvgD,
            criAvgDate,
            criAvgI,
            criAvgTime,
            criMax,
            criMaxD,
            criMaxDate,
            criMaxI,
            criMaxStr,
            criMaxTime,
            criMedian,
            criMedianD,
            criMedianI,
            criMin,
            criMinD,
            criMinDate,
            criMinI,
            criMinStr,
            criMinTime,
            criMode,
            criModeD,
            criModeI,
            criSum,
            criSumD,
            criSumDistinct,
            criSumI,
            criSumTime,
            criFirst,
            criLast,

            pCount,
            pCountDistinct,
            pAvg,
            pAvgD,
            pAvgDate,
            pAvgI,
            pAvgTime,
            pMax,
            pMaxD,
            pMaxDate,
            pMaxI,
            pMaxStr,
            pMaxTime,
            pMedian,
            pMedianD,
            pMedianI,
            pMin,
            pMinD,
            pMinDate,
            pMinI,
            pMinStr,
            pMinTime,
            pMode,
            pModeD,
            pModeI,
            pSum,
            pSumD,
            pSumDistinct,
            pSumI,
            pSumTime,
            pFirst,
            pLast,

            prCount,
            prCountDistinct,
            prAvg,
            prAvgD,
            prAvgDate,
            prAvgI,
            prAvgTime,
            prMax,
            prMaxD,
            prMaxDate,
            prMaxI,
            prMaxStr,
            prMaxTime,
            prMedian,
            prMedianD,
            prMedianI,
            prMin,
            prMinD,
            prMinDate,
            prMinI,
            prMinStr,
            prMinTime,
            prMode,
            prModeD,
            prModeI,
            prSum,
            prSumD,
            prSumDistinct,
            prSumI,
            prSumTime,
            prFirst,
            prLast,

            piCount,
            piCountDistinct,
            piAvg,
            piAvgD,
            piAvgDate,
            piAvgI,
            piAvgTime,
            piMax,
            piMaxD,
            piMaxDate,
            piMaxI,
            piMaxStr,
            piMaxTime,
            piMedian,
            piMedianD,
            piMedianI,
            piMin,
            piMinD,
            piMinDate,
            piMinI,
            piMinStr,
            piMinTime,
            piMode,
            piModeD,
            piModeI,
            piSum,
            piSumD,
            piSumDistinct,
            piSumI,
            piSumTime,
            piFirst,
            piLast,

            priCount,
            priCountDistinct,
            priAvg,
            priAvgD,
            priAvgDate,
            priAvgI,
            priAvgTime,
            priMax,
            priMaxD,
            priMaxDate,
            priMaxI,
            priMaxStr,
            priMaxTime,
            priMedian,
            priMedianD,
            priMedianI,
            priMin,
            priMinD,
            priMinDate,
            priMinI,
            priMinStr,
            priMinTime,
            priMode,
            priModeD,
            priModeI,
            priSum,
            priSumD,
            priSumDistinct,
            priSumI,
            priSumTime,
            priFirst,
            priLast,
            #endregion

            #region Totals Hierarchical
            CountAllLevels,
            CountAllLevelsOnlyChilds,
            CountOnlyChilds,
            CountDistinctAllLevels,
            CountDistinctAllLevelsOnlyChilds,
            CountDistinctOnlyChilds,

            SumAllLevels,
            SumAllLevelsOnlyChilds,
            SumOnlyChilds,
            SumDAllLevels,
            SumDAllLevelsOnlyChilds,
            SumDOnlyChilds,
            SumIAllLevels,
            SumIAllLevelsOnlyChilds,
            SumIOnlyChilds,
            SumTimeAllLevels,
            SumTimeAllLevelsOnlyChilds,
            SumTimeOnlyChilds,

            AvgAllLevels,
            AvgAllLevelsOnlyChilds,
            AvgOnlyChilds,
            AvgDAllLevels,
            AvgDAllLevelsOnlyChilds,
            AvgDOnlyChilds,
            AvgIAllLevels,
            AvgIAllLevelsOnlyChilds,
            AvgIOnlyChilds,
            AvgDateAllLevels,
            AvgDateAllLevelsOnlyChilds,
            AvgDateOnlyChilds,
            AvgTimeAllLevels,
            AvgTimeAllLevelsOnlyChilds,
            AvgTimeOnlyChilds,

            MaxAllLevels,
            MaxAllLevelsOnlyChilds,
            MaxOnlyChilds,
            MaxDAllLevels,
            MaxDAllLevelsOnlyChilds,
            MaxDOnlyChilds,
            MaxIAllLevels,
            MaxIAllLevelsOnlyChilds,
            MaxIOnlyChilds,

            MinAllLevels,
            MinAllLevelsOnlyChilds,
            MinOnlyChilds,
            MinDAllLevels,
            MinDAllLevelsOnlyChilds,
            MinDOnlyChilds,
            MinIAllLevels,
            MinIAllLevelsOnlyChilds,
            MinIOnlyChilds,

            MedianAllLevels,
            MedianAllLevelsOnlyChilds,
            MedianOnlyChilds,
            MedianDAllLevels,
            MedianDAllLevelsOnlyChilds,
            MedianDOnlyChilds,
            MedianIAllLevels,
            MedianIAllLevelsOnlyChilds,
            MedianIOnlyChilds,

            ModeAllLevels,
            ModeAllLevelsOnlyChilds,
            ModeOnlyChilds,
            ModeDAllLevels,
            ModeDAllLevelsOnlyChilds,
            ModeDOnlyChilds,
            ModeIAllLevels,
            ModeIAllLevelsOnlyChilds,
            ModeIOnlyChilds,

            FirstAllLevels,
            FirstAllLevelsOnlyChilds,
            FirstOnlyChilds,
            LastAllLevels,
            LastAllLevelsOnlyChilds,
            LastOnlyChilds,

            MinDateAllLevels,
            MinDateAllLevelsOnlyChilds,
            MinDateOnlyChilds,
            MinTimeAllLevels,
            MinTimeAllLevelsOnlyChilds,
            MinTimeOnlyChilds,
            MinStrAllLevels,
            MinStrAllLevelsOnlyChilds,
            MinStrOnlyChilds,

            MaxDateAllLevels,
            MaxDateAllLevelsOnlyChilds,
            MaxDateOnlyChilds,
            MaxTimeAllLevels,
            MaxTimeAllLevelsOnlyChilds,
            MaxTimeOnlyChilds,
            MaxStrAllLevels,
            MaxStrAllLevelsOnlyChilds,
            MaxStrOnlyChilds,
            #endregion

            Rank,

            Abs,
            Acos,
            Asin,
            Atan,
            Ceiling,
            Cos,
            Div,
            Exp,
            Floor,
            Log,
            Maximum,
            Minimum,
            Round,
            Sign,
            Sin,
            Sqrt,
            Tan,
            Truncate,

            DateDiff,
            DateSerial,
            Day,
            DayOfWeek,
            DayOfYear,
            DaysInMonth,
            DaysInYear,
            Hour,
            Minute,
            Month,
            Second,
            TimeSerial,
            Year,
            MonthName,
            WeekOfYear,
            WeekOfMonth,
            FromOADate,
            ToOADate,

            DateToStr,
            DateToStrPl,
            DateToStrRu,
            DateToStrUa,
            DateToStrPt,
            DateToStrPtBr,
            Insert,
            Length,
            Remove,
            Replace,
            Roman,
            Substring,
            ToCurrencyWords,
            ToCurrencyWordsAr,
            ToCurrencyWordsEnGb,
            ToCurrencyWordsEnIn,
            ToCurrencyWordsEs,
            ToCurrencyWordsFr,
            ToCurrencyWordsNl,
            ToCurrencyWordsPl,
            ToCurrencyWordsPt,
            ToCurrencyWordsPtBr,
            ToCurrencyWordsRu,
            ToCurrencyWordsThai,
            ToCurrencyWordsTr,
            ToCurrencyWordsUa,
            ToCurrencyWordsZh,
            ToLowerCase,
            ToProperCase,
            ToUpperCase,
            ToWords,
            ToWordsEs,
            ToWordsEnIn,
            ToWordsAr,
            ToWordsFa,
            ToWordsPl,
            ToWordsPt,
            ToWordsRu,
            ToWordsTr,
            ToWordsUa,
            ToWordsZh,
            Trim,
            TryParseDecimal,
            TryParseDouble,
            TryParseLong,
            Arabic,
            Persian,
            ToOrdinal,
            Left,
            Mid,
            Right,
            StrToDateTime,
            StrToNullableDateTime,

            IsNull,
            Next,
            NextIsNull,
            Previous,
            PreviousIsNull,

            IIF,
            Choose,
            Switch,

            ToString,
            Format,

            SystemConvertToBoolean,
            SystemConvertToByte,
            SystemConvertToChar,
            SystemConvertToDateTime,
            SystemConvertToDecimal,
            SystemConvertToDouble,
            SystemConvertToInt16,
            SystemConvertToInt32,
            SystemConvertToInt64,
            SystemConvertToSByte,
            SystemConvertToSingle,
            SystemConvertToString,
            SystemConvertToUInt16,
            SystemConvertToUInt32,
            SystemConvertToUInt64,

            MathRound,
            MathPow,

            AddAnchor,
            GetAnchorPageNumber,
            GetAnchorPageNumberThrough,

            ConvertRtf,
            GetLabel,
            GetParam,

            ParseInt,
            ParseInt64,
            ParseDouble,
            ParseDecimal,
            ParseDateTime,
            ParseDateTimeOffset,
            ParseTimeSpan,

            StringIsNullOrEmpty,
            StringIsNullOrWhiteSpace,

            EngineHelperJoinColumnContent,
            EngineHelperToQueryString,
            EngineHelperGetRealPageNumber,

            StiNullValuesHelperIsNull,

            TimeSpanFromDays,
            TimeSpanFromHours,
            TimeSpanFromMilliseconds,
            TimeSpanFromMinutes,
            TimeSpanFromSeconds,
            TimeSpanFromTicks,

            NewType,

            ImageFromFile,
            ConvertToBase64String,

            //methods

            m_Substring = 1000,
            m_ToString,
            m_ToLower,
            m_ToUpper,
            m_IndexOf,
            m_StartsWith,
            m_EndsWith,
            m_Replace,
            m_PadLeft,
            m_PadRight,
            m_TrimStart,
            m_TrimEnd,

            m_Parse,
            m_Contains,
            m_GetData,
            m_ToQueryString,
            
            m_AddYears,
            m_AddMonths,
            m_AddDays,
            m_AddHours,
            m_AddMinutes,
            m_AddSeconds,
            m_AddMilliseconds,
            m_ToShortDateString,
            m_ToShortTimeString,
            m_ToLongDateString,
            m_ToLongTimeString,

            m_GetCurrentConditionValue,

            m_Add,
            m_Subtract,

            m_MethodNameSpace,


            //operators

            op_Add = 2020,
            op_Sub,
            op_Mult,
            op_Div,
            op_Mod,
            op_Power,
            op_Neg,
            op_Cast,
            op_Not,
            op_CompareLeft,
            op_CompareLeftEqual,
            op_CompareRight,
            op_CompareRightEqual,
            op_CompareEqual,
            op_CompareNotEqual,
            op_Shl,
            op_Shr,
            op_And,
            op_And2,
            op_Or,
            op_Or2,
            op_Xor,

            UserFunction = 3000

        }

        public enum StiMethodType
        {
            Substring = 1000,
            ToString,
            ToLower,
            ToUpper,
            IndexOf,
            StartsWith,
            EndsWith,
            Replace,
            PadLeft,
            PadRight,
            TrimStart,
            TrimEnd,

            Parse,
            Contains,
            GetData,
            ToQueryString,

            AddYears,
            AddMonths,
            AddDays,
            AddHours,
            AddMinutes,
            AddSeconds,
            AddMilliseconds,
            ToShortDateString,
            ToShortTimeString,
            ToLongDateString,
            ToLongTimeString,

            GetCurrentConditionValue,

            Add,
            Subtract,

            MethodNameSpace
        }

        [Flags]
        public enum StiParameterNumber
        {
            Param1 = 1,
            Param2 = 2,
            Param3 = 4,
            Param4 = 8
        }

        #endregion
    }
}
