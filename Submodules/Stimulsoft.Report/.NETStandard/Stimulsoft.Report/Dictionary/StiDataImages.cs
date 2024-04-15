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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Images;
using System;
using System.Drawing;
using System.Windows.Forms;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Dictionary
{
    public static class StiDataImages
    {
        #region Properties        
        public static int ImageWidth => Images.ImageSize.Width;
        
        public static int ImageHeight => Images.ImageSize.Height;

        private static ImageList images;
        public static ImageList Images
        {
            get
            {
                if (images == null)
                    FillImages();

                return images;
            }
        }
        #endregion

        #region Methods
        private static void FillImages()
        {
            images = new ImageList
            {
                ColorDepth = ColorDepth.Depth24Bit,
                ImageSize = new Size(StiScale.I16, StiScale.I16)
            };

            images.Images.Add("Close.bmp", StiReportImages.Engine.Close(StiImageSize.Normal, false));
            images.AddImage("DataSource");
            images.AddImage("DataTransformation");

            #region DataColumn
            images.AddImage("DataColumn");
            images.AddImage("DataColumnBinary");
            images.AddImage("DataColumnBool");
            images.AddImage("DataColumnChar");
            images.AddImage("DataColumnDateTime");
            images.AddImage("DataColumnDecimal");
            images.AddImage("DataColumnFloat");
            images.AddImage("DataColumnImage");
            images.AddImage("DataColumnInt");
            images.AddImage("DataColumnString");
            #endregion

            #region CalcColumn
            images.AddImage("CalcColumn");
            images.AddImage("CalcColumnBinary");
            images.AddImage("CalcColumnBool");
            images.AddImage("CalcColumnChar");
            images.AddImage("CalcColumnDateTime");
            images.AddImage("CalcColumnDecimal");
            images.AddImage("CalcColumnFloat");
            images.AddImage("CalcColumnImage");
            images.AddImage("CalcColumnInt");
            images.AddImage("CalcColumnString");
            #endregion

            #region LockedDataColumn
            images.AddImage("LockedDataColumn");
            images.AddImage("LockedDataColumnBinary");
            images.AddImage("LockedDataColumnBool");
            images.AddImage("LockedDataColumnChar");
            images.AddImage("LockedDataColumnDateTime");
            images.AddImage("LockedDataColumnDecimal");
            images.AddImage("LockedDataColumnFloat");
            images.AddImage("LockedDataColumnImage");
            images.AddImage("LockedDataColumnInt");
            images.AddImage("LockedDataColumnString");
            #endregion

            #region LockedCalcColumn
            images.AddImage("LockedCalcColumn");
            images.AddImage("LockedCalcColumnBinary");
            images.AddImage("LockedCalcColumnBool");
            images.AddImage("LockedCalcColumnChar");
            images.AddImage("LockedCalcColumnDateTime");
            images.AddImage("LockedCalcColumnDecimal");
            images.AddImage("LockedCalcColumnFloat");
            images.AddImage("LockedCalcColumnImage");
            images.AddImage("LockedCalcColumnInt");
            images.AddImage("LockedCalcColumnString");
            #endregion

            #region Total
            images.AddImage("Total");
            images.AddImage("Totals");
            #endregion

            #region LockedTotal
            images.AddImage("LockedTotal");
            #endregion

            #region Variable
            images.AddImage("Variable");
            images.AddImage("Variables");
            images.AddImage("VariableBinary");
            images.AddImage("VariableBool");
            images.AddImage("VariableChar");
            images.AddImage("VariableDateTime");
            images.AddImage("VariableDecimal");
            images.AddImage("VariableFloat");
            images.AddImage("VariableImage");
            images.AddImage("VariableInt");
            images.AddImage("VariableString");
            #endregion

            #region Resource
            images.AddImage("Resource");
            images.AddImage("Resources");

            images.AddImage("ResourceCsv");
            images.AddImage("ResourceDbf");
            images.AddImage("ResourceExcel");
            images.AddImage("ResourceImage");
            images.AddImage("ResourceJson");
            images.AddImage("ResourceReport");
            images.AddImage("ResourceReportSnapshot");
            images.AddImage("ResourceRtf");
            images.AddImage("ResourceTxt");
            images.AddImage("ResourceXml");
            images.AddImage("ResourceXsd");
            images.AddImage("ResourcePdf");
            images.AddImage("ResourceWord");
            images.AddImage("ResourceMap");
            images.AddImage("ResourceGis");

            images.AddImage("ResourceFontEot");
            images.AddImage("ResourceFontOtf");
            images.AddImage("ResourceFontTtc");
            images.AddImage("ResourceFontTtf");
            images.AddImage("ResourceFontWoff");
            #endregion

            #region VariableRange
            images.AddImage("VariableRangeChar");
            images.AddImage("VariableRangeDateTime");
            images.AddImage("VariableRangeDecimal");
            images.AddImage("VariableRangeFloat");
            images.AddImage("VariableRangeInt");
            images.AddImage("VariableRangeString");
            #endregion

            #region VariableList
            images.AddImage("VariableListImage");
            images.AddImage("VariableListBool");
            images.AddImage("VariableListChar");
            images.AddImage("VariableListDateTime");
            images.AddImage("VariableListDecimal");
            images.AddImage("VariableListFloat");
            images.AddImage("VariableListInt");
            images.AddImage("VariableListString");
            #endregion

            #region LockedVariable
            images.AddImage("LockedVariable");
            images.AddImage("LockedVariableBinary");
            images.AddImage("LockedVariableBool");
            images.AddImage("LockedVariableChar");
            images.AddImage("LockedVariableDateTime");
            images.AddImage("LockedVariableDecimal");
            images.AddImage("LockedVariableFloat");
            images.AddImage("LockedVariableImage");
            images.AddImage("LockedVariableInt");
            images.AddImage("LockedVariableString");
            #endregion

            #region LockedVariableRange
            images.AddImage("LockedVariableRangeChar");
            images.AddImage("LockedVariableRangeDateTime");
            images.AddImage("LockedVariableRangeDecimal");
            images.AddImage("LockedVariableRangeFloat");
            images.AddImage("LockedVariableRangeInt");
            images.AddImage("LockedVariableRangeString");
            #endregion

            #region LockedVariableList
            images.AddImage("LockedVariableListImage");
            images.AddImage("LockedVariableListBool");
            images.AddImage("LockedVariableListChar");
            images.AddImage("LockedVariableListDateTime");
            images.AddImage("LockedVariableListDecimal");
            images.AddImage("LockedVariableListFloat");
            images.AddImage("LockedVariableListInt");
            images.AddImage("LockedVariableListString");
            #endregion

            #region SystemVariable
            images.AddImage("SystemVariable");
            images.AddImage("SystemVariables");
            images.AddImage("SystemVariableColumn");
            images.AddImage("SystemVariableGroupLine");
            images.AddImage("SystemVariableIsFirstPage");
            images.AddImage("SystemVariableIsFirstPageThrough");
            images.AddImage("SystemVariableIsLastPage");
            images.AddImage("SystemVariableIsLastPageThrough");
            images.AddImage("SystemVariableLine");
            images.AddImage("SystemVariableLineABC");
            images.AddImage("SystemVariableLineThrough");
            images.AddImage("SystemVariableLineRoman");
            images.AddImage("SystemVariablePageNofM");
            images.AddImage("SystemVariablePageNofMThrough");
            images.AddImage("SystemVariablePageNumber");
            images.AddImage("SystemVariablePageNumberThrough");
            images.AddImage("SystemVariableReportAlias");
            images.AddImage("SystemVariableReportAuthor");
            images.AddImage("SystemVariableReportChanged");
            images.AddImage("SystemVariableReportCreated");
            images.AddImage("SystemVariableReportDescription");
            images.AddImage("SystemVariableReportName");
            images.AddImage("SystemVariableTime");
            images.AddImage("SystemVariableToday");
            images.AddImage("SystemVariableTotalPageCount");
            images.AddImage("SystemVariableTotalPageCountThrough");
            #endregion

            images.AddImage("DataRelation");
            images.AddImage("DataRelationActive");
            images.AddImage("Parameter");

            images.AddImage("Function");
            images.AddImage("AggregateFunction");

            images.AddImage("Category");
            images.AddImage("Database");

            images.Images.Add("Format", StiReportImages.Formats.Format(StiImageSize.Normal, false));
            images.Images.Add("FormatGeneral", StiReportImages.Formats.General(StiImageSize.Normal, false));
            images.Images.Add("FormatBoolean", StiReportImages.Formats.Boolean(StiImageSize.Normal, false));
            images.Images.Add("FormatCurrency", StiReportImages.Formats.Currency(StiImageSize.Normal, false));
            images.Images.Add("FormatPercentage", StiReportImages.Formats.Percentage(StiImageSize.Normal, false));
            images.Images.Add("FormatNumber", StiReportImages.Formats.Number(StiImageSize.Normal, false));
            images.Images.Add("FormatDate", StiReportImages.Formats.Date(StiImageSize.Normal, false));
            images.Images.Add("FormatTime", StiReportImages.Formats.Time(StiImageSize.Normal, false));

            images.AddImage("LockedCategory");
            images.AddImage("LockedDataSource");
            images.AddImage("LockedDataTransformation");
            images.AddImage("LockedDataRelation");
            images.AddImage("LockedDataRelationActive");
            images.AddImage("LockedParameter");
            images.AddImage("LockedDatabase");

            images.AddImage("UndefinedDataSource");
            images.AddImage("UndefinedDatabase");

            images.AddImage("Function");
            images.AddImage("DataStore");
            images.AddImage("DatabaseFail");
            images.AddImage("DatabaseTransformation");
            images.AddImage("HtmlTag");

            images.AddImage("BusinessObject");
            images.AddImage("LockedBusinessObject");

            //Artem Not change the sequence of adding.
            images.AddImage("SystemVariablePageCopyNumber");
        }

        public static Image Get(string key)
        {
            return Images.Images[key];
        }

        public static Image GetImageFromColumn(StiDataColumn dataColumn)
        {
            return Get(GetImageKeyFromColumn(dataColumn));
        }

        private static void AddImage(this ImageList imageList, string imageKey, string imageName = null)
        {
            if (imageName == null)
                imageName = imageKey;

            imageList.Images.Add(imageKey, StiReportImages.GetDictionaryImage(imageName, StiImageSize.Normal, false));
        }


        internal static string GetImageKeyFromType(Type type)
        {
            #region Simple Types
            if (type == typeof(bool) || type == typeof(bool?))
                return "Bool";

            if (type == typeof(char) || type == typeof(char?))
                return "Char";

            if (type == typeof(DateTime) ||
                type == typeof(DateTimeOffset) ||
                type == typeof(TimeSpan) ||
                type == typeof(DateTime?) ||
                type == typeof(DateTimeOffset?) ||
                type == typeof(TimeSpan?))
                return "DateTime";

            if (type == typeof(decimal) ||
                type == typeof(decimal?))
                return "Decimal";

            if (type == typeof(int) ||
                type == typeof(uint) ||
                type == typeof(long) ||
                type == typeof(ulong) ||
                type == typeof(byte) ||
                type == typeof(sbyte) ||
                type == typeof(short) ||
                type == typeof(ushort) ||
                type == typeof(int?) ||
                type == typeof(uint?) ||
                type == typeof(long?) ||
                type == typeof(ulong?) ||
                type == typeof(byte?) ||
                type == typeof(sbyte?) ||
                type == typeof(short?) ||
                type == typeof(ushort?))
                return "Int";

            if (type == typeof(float) ||
                type == typeof(double) ||
                type == typeof(float?) ||
                type == typeof(double?))
                return "Float";

            if (type == typeof(Image) ||
                type == typeof(Bitmap))
                return "Image";
            #endregion

            #region Range Types
            if (type == typeof(CharRange))
                return "RangeChar";

            if (type == typeof(DateTimeRange) ||
                type == typeof(TimeSpanRange))
                return "RangeDateTime";

            if (type == typeof(DecimalRange))
                return "RangeDecimal";

            if (type == typeof(IntRange) ||
                type == typeof(LongRange) ||
                type == typeof(ByteRange) ||
                type == typeof(ShortRange))
                return "RangeInt";

            if (type == typeof(FloatRange) ||
                type == typeof(DoubleRange))
                return "RangeFloat";

            if (type == typeof(StringRange) ||
                type == typeof(GuidRange))
                return "RangeString";
            #endregion

            #region List Types
            if (type == typeof(BoolList))
                return "ListBool";

            if (type == typeof(StringList) ||
                type == typeof(GuidList))
                return "ListString";

            if (type == typeof(CharList))
                return "ListChar";

            if (type == typeof(DateTimeList) ||
                type == typeof(TimeSpanList))
                return "ListDateTime";

            if (type == typeof(DecimalList))
                return "ListDecimal";

            if (type == typeof(IntList) ||
                type == typeof(LongList) ||
                type == typeof(ByteList) ||
                type == typeof(ShortList))
                return "ListInt";

            if (type == typeof(FloatList) ||
                type == typeof(DoubleList))
                return "ListFloat";
            #endregion

            if (type != null && type.IsArray)
                return "Binary";

            return "String";
        }

        private static string GetImageKeyFromType(StiResourceType type)
        {
            return type.ToString();
        }

        public static string GetImageKeyFromColumn(StiDataColumn column)
        {
            var imageKey = GetImageKeyFromType(column?.Type);
            if (column is StiCalcDataColumn)
            {
                if (column.DataSource != null)
                {
                    return column.DataSource.Inherited
                        ? $"LockedCalcColumn{imageKey}"
                        : $"CalcColumn{imageKey}";
                }
                else
                {
                    return column.BusinessObject.Inherited
                        ? $"LockedCalcColumn{imageKey}"
                        : $"CalcColumn{imageKey}";
                }
            }

            if (column.DataSource != null)
            {
                return column.DataSource.Inherited
                    ? $"LockedDataColumn{imageKey}"
                    : $"DataColumn{imageKey}";
            }
            else if (column.BusinessObject != null)
            {
                return column.BusinessObject.Inherited
                    ? $"LockedDataColumn{imageKey}"
                    : $"DataColumn{imageKey}";
            }

            return string.Empty;
        }

        public static string GetImageKeyFromVariable(StiVariable variable)
        {
            var imageKey = GetImageKeyFromType(variable?.Type);

            return variable.Inherited
                ? $"LockedVariable{imageKey}"
                : $"Variable{imageKey}";
        }

        public static string GetImageKeyFromResource(StiResource resource)
        {
            return $"Resource{GetImageKeyFromType(resource.Type)}";
        }
        #endregion

    }
}