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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.BarCodes.Design;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - GS1-DataMatrix.
    /// </summary>
    [TypeConverter(typeof(StiDataMatrixBarCodeTypeConverter))]
    public class StiGS1DataMatrixBarCodeType : StiDataMatrixBarCodeType
    {
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiGS1DataMatrixBarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.MatrixSize(),
                propHelper.Module(),
                propHelper.ProcessTilde(),
                propHelper.UseRectangularSymbols()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }
        #endregion

        #region ServiceName
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string ServiceName => "GS1 DataMatrix";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "[21]012345[3103]000123";

        [Browsable(false)]
        [StiBrowsable(false)]
        public new StiDataMatrixEncodingType EncodingType { get; set; } = StiDataMatrixEncodingType.Ascii;

        public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];

                props[9] = true;
                props[12] = true;
                props[13] = true;
                props[21] = true;

                return props;
            }
        }
        #endregion

        #region Methods
        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
        {
            string code = GetCode(barCode);
            code = CheckCodeSymbols(code, "!\"%&'()*+,-./0123456789:;<=>?ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz[]");

            var sbCode = new StringBuilder();
            var sbText = new StringBuilder();
            string errorMessage = StiGS1ApplicationIdentifiers.ParseCode(code, sbCode, sbText, (char)BarcodeCommandCode.Fnc1, true);

            if (errorMessage != null && (barCode.CodeValue == null) && barCode.Code.Value.Contains("{"))
            {
                errorMessage = null;
                sbCode = new StringBuilder(barCode.Code.Value);
                sbText = new StringBuilder(barCode.Code.Value);
            }

            BarCodeData.Code = sbCode.ToString();

            if (errorMessage == null)
            {
                var dm = new StiDataMatrix(sbCode.ToString(), EncodingType, UseRectangularSymbols, MatrixSize, ProcessTilde);

                BarCodeData.MatrixGrid = dm.Matrix;
                BarCodeData.MatrixWidth = dm.Width;
                BarCodeData.MatrixHeight = dm.Height;
                BarCodeData.MatrixRatioY = 1;

                errorMessage = dm.ErrorMessage;
            }

            if (errorMessage == null)
            {
                Draw2DBarCode(context, rect, barCode, zoom);
            }
            else
            {
                DrawBarCodeError(context, rect, barCode, errorMessage);
            }
        }
  
        public override StiBarCodeTypeService CreateNew() => new StiGS1DataMatrixBarCodeType();
        #endregion

        public StiGS1DataMatrixBarCodeType() : this(40f, StiDataMatrixEncodingType.Ascii, false, StiDataMatrixSize.Automatic, false)
        {
        }

        public StiGS1DataMatrixBarCodeType(float module, StiDataMatrixEncodingType encodingType, bool useRectangularSymbols, StiDataMatrixSize matrixSize, bool processTilde)
		{
			this.Module = module;
			this.EncodingType = encodingType;
			this.UseRectangularSymbols = useRectangularSymbols;
			this.MatrixSize = matrixSize;
            this.ProcessTilde = processTilde;
		}
	}
}