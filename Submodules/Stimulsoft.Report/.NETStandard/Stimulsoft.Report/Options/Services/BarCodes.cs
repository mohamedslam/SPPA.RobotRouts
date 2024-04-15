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


using System.Collections.Generic;
using Stimulsoft.Report.BarCodes;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        public sealed partial class Services
        {
            private static List<StiBarCodeTypeService> barCodes;
            public static List<StiBarCodeTypeService> BarCodes
            {
                get
                {
                    lock (lockObject)
                    {
                        return barCodes ?? (barCodes = new List<StiBarCodeTypeService>
                        {
                            new StiQRCodeBarCodeType(),
                            new StiDataMatrixBarCodeType(),
                            new StiMaxicodeBarCodeType(),
                            new StiPdf417BarCodeType(),
                            new StiAztecBarCodeType(),
                            new StiAustraliaPost4StateBarCodeType(),
                            new StiIntelligentMail4StateBarCodeType(),
                            new StiPharmacodeBarCodeType(),
                            new StiCode11BarCodeType(),
                            new StiCode128aBarCodeType(),
                            new StiCode128bBarCodeType(),
                            new StiCode128cBarCodeType(),
                            new StiCode128AutoBarCodeType(),
                            new StiCode39BarCodeType(),
                            new StiCode39ExtBarCodeType(),
                            new StiCode93BarCodeType(),
                            new StiCode93ExtBarCodeType(),
                            new StiCodabarBarCodeType(),
                            new StiEAN128aBarCodeType(),
                            new StiEAN128bBarCodeType(),
                            new StiEAN128cBarCodeType(),
                            new StiEAN128AutoBarCodeType(),
                            new StiGS1_128BarCodeType(),
                            new StiEAN13BarCodeType(),
                            new StiEAN8BarCodeType(),
                            new StiFIMBarCodeType(),
                            new StiIsbn10BarCodeType(),
                            new StiIsbn13BarCodeType(),
                            new StiITF14BarCodeType(),
                            new StiJan13BarCodeType(),
                            new StiJan8BarCodeType(),
                            new StiMsiBarCodeType(),
                            new StiPlesseyBarCodeType(),
                            new StiPostnetBarCodeType(),
                            new StiDutchKIXBarCodeType(),
                            new StiRoyalMail4StateBarCodeType(),
                            new StiSSCC18BarCodeType(),
                            new StiUpcABarCodeType(),
                            new StiUpcEBarCodeType(),
                            new StiUpcSup2BarCodeType(),
                            new StiUpcSup5BarCodeType(),
                            new StiInterleaved2of5BarCodeType(),
                            new StiStandard2of5BarCodeType()
                        });
                    }
                }
            }
        }
    }
}
