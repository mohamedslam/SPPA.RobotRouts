#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

namespace Stimulsoft.Drawing.Imaging
{
    public sealed class Encoder
    {
        private string guid = "";

        public static readonly Encoder ChrominanceTable;
        public static readonly Encoder ColorDepth;
        public static readonly Encoder Compression;
        public static readonly Encoder LuminanceTable;
        public static readonly Encoder Quality;
        public static readonly Encoder RenderMethod;
        public static readonly Encoder SaveFlag;
        public static readonly Encoder ScanMethod;
        public static readonly Encoder Transformation;
        public static readonly Encoder Version;

        static Encoder()
        {
            ChrominanceTable = new Encoder("f2e455dc-09b3-4316-8260-676ada32481c");
            ColorDepth = new Encoder("66087055-ad66-4c7c-9a18-38a2310b8337");
            Compression = new Encoder("e09d739d-ccd4-44ee-8eba-3fbf8be4fc58");
            LuminanceTable = new Encoder("edb33bce-0266-4a77-b904-27216099e717");
            Quality = new Encoder("1d5be4b5-fa4a-452d-9cdd-5db35105e7eb");
            RenderMethod = new Encoder("6d42c53a-229a-4825-8bb7-5c99e2b9a8b8");
            SaveFlag = new Encoder("292266fc-ac40-47bf-8cfc-a85b89a655de");
            ScanMethod = new Encoder("3a4e2661-3109-4e56-8536-42c156e7dcfa");
            Transformation = new Encoder("8d0eb2d1-a58e-4ea8-aa14-108074b7b6f9");
            Version = new Encoder("24d18c76-814a-41a4-bf53-1c219cccf797");
        }

        internal Encoder(String guid)
        {
            this.guid = guid;
        }

        public static implicit operator System.Drawing.Imaging.Encoder(Encoder encoder)
        {
            var ff = Encoder.ChrominanceTable.guid;

            if (encoder.guid == Encoder.ChrominanceTable.guid) return System.Drawing.Imaging.Encoder.ChrominanceTable;
            if (encoder.guid == Encoder.ColorDepth.guid) return System.Drawing.Imaging.Encoder.ColorDepth;
            if (encoder.guid == Encoder.Compression.guid) return System.Drawing.Imaging.Encoder.Compression;
            if (encoder.guid == Encoder.LuminanceTable.guid) return System.Drawing.Imaging.Encoder.LuminanceTable;
            if (encoder.guid == Encoder.Quality.guid) return System.Drawing.Imaging.Encoder.Quality;
            if (encoder.guid == Encoder.RenderMethod.guid) return System.Drawing.Imaging.Encoder.RenderMethod;
            if (encoder.guid == Encoder.SaveFlag.guid) return System.Drawing.Imaging.Encoder.SaveFlag;
            if (encoder.guid == Encoder.ScanMethod.guid) return System.Drawing.Imaging.Encoder.ScanMethod;
            if (encoder.guid == Encoder.Transformation.guid) return System.Drawing.Imaging.Encoder.Transformation;
            if (encoder.guid == Encoder.Version.guid) return System.Drawing.Imaging.Encoder.Version;

            return null;
        }
    }
}
