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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Stimulsoft.Base.Drawing
{
    public class StiFontReader
    {
#pragma warning disable CS3003 // Type is not CLS-compliant

        #region Structures
        public class Table
        {
            public UInt32 Tag { get; set; }
            public string TagString { get; set; }
            public UInt32 Checksum { get; set; }
            public UInt32 Offset { get; set; }
            public UInt32 Length { get; set; }
        }

        public class Head
        {
            public UInt16 MajorVersion { get; set; }
            public UInt16 MinorVersion { get; set; }
            public int FontRevision { get; set; }   //fixed
            public UInt32 ChecksumAdjustment { get; set; }
            public UInt32 MagicNumber { get; set; }
            public UInt16 Flags { get; set; }
            public UInt16 UnitsPerEm { get; set; }
            public DateTime Created { get; set; }
            public DateTime Modified { get; set; }
            public Int16 YMin { get; set; }
            public Int16 XMin { get; set; }
            public Int16 XMax { get; set; }
            public Int16 YMax { get; set; }
            public UInt16 MacStyle { get; set; }
            public UInt16 LowestRecPPEM { get; set; }
            public Int16 FontDirectionHint { get; set; }
            public Int16 IndexToLocFormat { get; set; }
            public Int16 GlyphDataFormat { get; set; }
        }

        public class NameRecord
        {
            public UInt16 PlatformID { get; set; }
            public UInt16 EncodingID { get; set; }
            public UInt16 LanguageID { get; set; }
            public UInt16 NameID { get; set; }
            public UInt16 Length { get; set; }
            public UInt16 Offset { get; set; }
            public string String { get; set; }
        }

        public class LangTagRecord
        {
            public UInt16 Length { get; set; }
            public UInt16 Offset { get; set; }
        }

        public class Name
        {
            public UInt16 Format { get; set; } = 1;
            public UInt16 Count { get; set; }
            public UInt16 StringOffset { get; set; }
            public List<NameRecord> NameRecord { get; set; }
            public UInt16 LangTagCount { get; set; }
            public List<LangTagRecord> LangTagRecord { get; set; }
            public Hashtable FontNames { get; set; }
        }

        public class EncodingRecord
        {
            public UInt16 PlatformID { get; set; }
            public UInt16 EncodingID { get; set; }
            public UInt32 Offset { get; set; }
        }

        public class Format4
        {
            public UInt16 Format { get; set; }
            public UInt16 Length { get; set; }
            public UInt16 Language { get; set; }
            public UInt16 SegCountX2 { get; set; }
            public UInt16 SearchRange { get; set; }
            public UInt16 EntrySelector { get; set; }
            public UInt16 RangeShift { get; set; }
            public List<UInt16> EndCode { get; set; }
            public List<UInt16> StartCode { get; set; }
            public List<Int16> IdDelta { get; set; }
            public List<UInt16> IdRangeOffset { get; set; }
            public Dictionary<uint, uint> GlyphIndexMap { get; set; }   //must be UInt16, but CLS-compliant ....
        }

        public class Cmap
        {
            public UInt16 Version { get; set; }
            public UInt16 NumTables { get; set; }
            public List<EncodingRecord> EncodingRecords { get; set; }
            public Dictionary<uint, uint> GlyphIndexMap { get; set; }
        }

        public class Maxp
        {
            public UInt32 Version { get; set; }
            public UInt16 NumGlyphs { get; set; }
            public UInt16 MaxPoints { get; set; }
            public UInt16 MaxContours { get; set; }
            public UInt16 MaxCompositePoints { get; set; }
            public UInt16 MaxCompositeContours { get; set; }
            public UInt16 MaxZones { get; set; }
            public UInt16 MaxTwilightPoints { get; set; }
            public UInt16 MaxStorage { get; set; }
            public UInt16 MaxFunctionDefs { get; set; }
            public UInt16 MaxInstructionDefs { get; set; }
            public UInt16 MaxStackElements { get; set; }
            public UInt16 MaxSizeOfInstructions { get; set; }
            public UInt16 MaxComponentElements { get; set; }
            public UInt16 MaxComponentDepth { get; set; }
        }

        public class Hhea
        {
            public UInt32 Version { get; set; }
            public Int16 Ascent { get; set; }
            public Int16 Descent { get; set; }
            public Int16 LineGap { get; set; }
            public UInt16 AdvanceWidthMax { get; set; }
            public Int16 MinLeftSideBearing { get; set; }
            public Int16 MinRightSideBearing { get; set; }
            public Int16 XMaxExtent { get; set; }
            public Int16 CaretSlopeRise { get; set; }
            public Int16 CaretSlopeRun { get; set; }
            public Int16 CaretOffset { get; set; }
            public Int16 MetricDataFormat { get; set; }
            public UInt16 NumOfLongHorMetrics { get; set; }
        }

        public class LongHorMetric
        {
            public UInt16 AdvanceWidth { get; set; }
            public Int16 LeftSideBearing { get; set; }
        }

        public class Hmtx
        {
            public List<LongHorMetric> HMetrics { get; set; }
            public List<Int16> LeftSideBearing { get; set; }
        }

        public class GlyphHeader
        {
            public Int16 NumberOfContours { get; set; }
            public Int16 XMin { get; set; }
            public Int16 YMin { get; set; }
            public Int16 XMax { get; set; }
            public Int16 YMax { get; set; }
        }

        public class Glyph
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int Lsb { get; set; }
            public int Rsb { get; set; }
            public int AdvanceWidth => Lsb + Width + Rsb;
        }

        public class OS2
        {
            public UInt16 Version { get; set; }
            public Int16 XAvgCharWidth { get; set; }
            public UInt16 UsWeightClass { get; set; }
            public UInt16 UsWidthClass { get; set; }
            public UInt16 FsType { get; set; }
            public Int16 YSubscriptXSize { get; set; }
            public Int16 YSubscriptYSize { get; set; }
            public Int16 YSubscriptXOffset { get; set; }
            public Int16 YSubscriptYOffset { get; set; }
            public Int16 YSuperscriptXSize { get; set; }
            public Int16 YSuperscriptYSize { get; set; }
            public Int16 YSuperscriptXOffset { get; set; }
            public Int16 YSuperscriptYOffset { get; set; }
            public Int16 YStrikeoutSize { get; set; }
            public Int16 YStrikeoutPosition { get; set; }
            public Int16 SFamilyClass { get; set; }
            public byte[] Panose { get; set; }
            public UInt32 UlUnicodeRange1 { get; set; }
            public UInt32 UlUnicodeRange2 { get; set; }
            public UInt32 UlUnicodeRange3 { get; set; }
            public UInt32 UlUnicodeRange4 { get; set; }
            public byte[] AchVendID { get; set; }
            public UInt16 FsSelection { get; set; }
            public UInt16 UsFirstCharIndex { get; set; }
            public UInt16 UsLastCharIndex { get; set; }
            public Int16 STypoAscender { get; set; }
            public Int16 STypoDescender { get; set; }
            public Int16 STypoLineGap { get; set; }
            public UInt16 UsWinAscent { get; set; }
            public UInt16 UsWinDescent { get; set; }
            public UInt32 UlCodePageRange1 { get; set; }
            public UInt32 UlCodePageRange2 { get; set; }
            public Int16 SxHeight { get; set; }
            public Int16 SCapHeight { get; set; }
            public UInt16 UsDefaultChar { get; set; }
            public UInt16 UsBreakChar { get; set; }
            public UInt16 UsMaxContext { get; set; }
            public UInt16 UsLowerOpticalPointSize { get; set; }
            public UInt16 UsUpperOpticalPointSize { get; set; }
        }
        #endregion

        #region  Class TtfInfo
        public class TtfInfo
        {
            public uint HeaderOffset;
            public UInt32 SfntVersion;
            public UInt16 NumTables;
            //public UInt16 SearchRange;
            //public UInt16 EntrySelector;
            //public UInt16 RangeShift;

            public Dictionary<string, Table> Tables { get; set; }
            public Head Head { get; set; }
            public Name Name { get; set; }
            public Maxp Maxp { get; set; }
            public Hhea Hhea { get; set; }
            public Hmtx Hmtx { get; set; }
            public Cmap Cmap { get; set; }
            public OS2 OS2 { get; set; }

            public StiFontReader FontReader = null;

#pragma warning disable CS3002 // Return type is not CLS-compliant
            public Dictionary<uint, uint> GetCmapDictionary() => FontReader.GetCmapDictionary();
#pragma warning restore CS3002 // Return type is not CLS-compliant

            public string GetFamilyName()
            {
                string name = (string)Name.FontNames["1033"];
                if (string.IsNullOrWhiteSpace(name))
                {
                    foreach (DictionaryEntry de in Name.FontNames)
                    {
                        name = (string)de.Value;
                        break;
                    }
                }
                return name;
            }

            public FontStyle GetStyle()
            {
                FontStyle fs = FontStyle.Regular;
                if (OS2 != null)
                {
                    if ((OS2.FsSelection & 0x01) > 0) fs |= FontStyle.Italic;
                    if (OS2.UsWeightClass >= 700) fs |= FontStyle.Bold;
                }
                return fs;
            }
        }
        #endregion

        #region Methods

        #region Read 'head' table
        private Head ReadHeadTable()
        {
            SetPosition(ttf.Tables["head"].Offset);

            var head = new Head()
            {
                MajorVersion = getUint16(),
                MinorVersion = getUint16(),
                FontRevision = getFixed(),
                ChecksumAdjustment = getUint32(),
                MagicNumber = getUint32(),
                Flags = getUint16(),
                UnitsPerEm = getUint16(),
                Created = getDate(),
                Modified = getDate(),
                XMin = getInt16(),
                YMin = getInt16(),
                XMax = getInt16(),
                YMax = getInt16(),
                MacStyle = getUint16(),
                LowestRecPPEM = getUint16(),
                FontDirectionHint = getInt16(),
                IndexToLocFormat = getInt16(),
                GlyphDataFormat = getInt16()
            };

            if (head.MagicNumber != 0x5f0f3cf5)
            {
                throw new Exception("Magic number is incorrect");
            }

            return head;
        }
        #endregion

        #region Read 'name' table
        private Name ReadNameTable()
        {
            uint offset = ttf.Tables["name"].Offset;
            SetPosition(offset);

            var format = getUint16();
            Name name;
            if (format == 0)
            {
                name = new Name()
                {
                    Format = 0,
                    Count = getUint16(),
                    StringOffset = getOffset16(),
                    NameRecord = new List<NameRecord>()
                };

                for (var i = 0; i < name.Count; i++)
                {
                    name.NameRecord.Add(new NameRecord()
                    {
                        PlatformID = getUint16(),
                        EncodingID = getUint16(),
                        LanguageID = getUint16(),
                        NameID = getUint16(),
                        Length = getUint16(),
                        Offset = getOffset16()
                    });
                }
            }
            else if (format == 1)
            {
                name = new Name()
                {
                    Format = 1,
                    Count = getUint16(),
                    StringOffset = getUint16(),
                    NameRecord = new List<NameRecord>()
                };

                for (var i = 0; i < name.Count; i++)
                {
                    name.NameRecord.Add(new NameRecord()
                    {
                        PlatformID = getUint16(),
                        EncodingID = getUint16(),
                        LanguageID = getUint16(),
                        NameID = getUint16(),
                        Length = getUint16(),
                        Offset = getOffset16()
                    });
                }

                name.LangTagCount = getUint16();
                name.LangTagRecord = new List<LangTagRecord>();

                for (var i = 0; i < name.LangTagCount; i++)
                {
                    name.LangTagRecord.Add(new LangTagRecord()
                    {
                        Length = getUint16(),
                        Offset = getOffset16()
                    });
                }
                name = name as Name;
            }
            else
            {
                throw new Exception("Incorrect format");
            }

            name.FontNames = new Hashtable();
            foreach (var record in name.NameRecord)
            {
                SetPosition(offset + name.StringOffset + record.Offset);
                if (record.PlatformID == 3)
                {
                    record.String = GetStringUtf16(record.Length);
                }
                else
                {
                    record.String = GetString(record.Length);
                }
                if (((record.PlatformID == 3) && (record.EncodingID == 0 || record.EncodingID == 1) && (record.NameID == 1)) ||
                    ((record.PlatformID == 0) && (record.EncodingID == 3) && (record.NameID == 1)))
                {
                    name.FontNames[record.LanguageID.ToString()] = record.String;
                }
            }
            if (name.FontNames.Count == 0)
            {
                foreach (var record in name.NameRecord)
                {
                    if (record.NameID == 1)
                    {
                        name.FontNames[record.LanguageID.ToString()] = record.String;
                    }
                }
            }

            return name;
        }
        #endregion

        #region Read 'maxp' table
        // https://docs.microsoft.com/en-us/typography/opentype/spec/maxp
        private Maxp ReadMaxpTable()
        {
            SetPosition(ttf.Tables["maxp"].Offset);

            var maxp = new Maxp()
            {
                Version = getUint32(),
                NumGlyphs = getUint16(),
                MaxPoints = getUint16(),
                MaxContours = getUint16(),
                MaxCompositePoints = getUint16(),
                MaxCompositeContours = getUint16(),
                MaxZones = getUint16(),
                MaxTwilightPoints = getUint16(),
                MaxStorage = getUint16(),
                MaxFunctionDefs = getUint16(),
                MaxInstructionDefs = getUint16(),
                MaxStackElements = getUint16(),
                MaxSizeOfInstructions = getUint16(),
                MaxComponentElements = getUint16(),
                MaxComponentDepth = getUint16()
            };

            return maxp;
        }
        #endregion

        #region Read 'hhea' table
        // https://docs.microsoft.com/en-us/typography/opentype/spec/hhea
        private Hhea ReadHheaTable()
        {
            SetPosition(ttf.Tables["hhea"].Offset);

            var hhea = new Hhea()
            {
                Version = getUint32(),
                Ascent = getFWord(),
                Descent = getFWord(),
                LineGap = getFWord(),
                AdvanceWidthMax = getUFWord(),
                MinLeftSideBearing = getFWord(),
                MinRightSideBearing = getFWord(),
                XMaxExtent = getFWord(),
                CaretSlopeRise = getInt16(),
                CaretSlopeRun = getInt16(),
                CaretOffset = getFWord()
            };

            // Skip 4 reserved places.
            getInt16();
            getInt16();
            getInt16();
            getInt16();

            hhea.MetricDataFormat = getInt16();
            hhea.NumOfLongHorMetrics = getUint16();

            return hhea;
        }
        #endregion

        #region Read 'hmtx' table
        // https://docs.microsoft.com/en-us/typography/opentype/spec/hmtx
        private Hmtx ReadHmtxTable(UInt16 numOfLongHorMetrics, uint numGlyphs)
        {
            SetPosition(ttf.Tables["hmtx"].Offset);

            var hMetrics = new List<LongHorMetric>();
            for (var i = 0; i < numOfLongHorMetrics; i++)
            {
                hMetrics.Add(new LongHorMetric()
                {
                    AdvanceWidth = getUint16(),
                    LeftSideBearing = getInt16()
                });
            }

            var leftSideBearing = new List<Int16>();
            for (var i = 0; i < numGlyphs - numOfLongHorMetrics; i++)
            {
                leftSideBearing.Add(getFWord());
            }

            var hmtx = new Hmtx()
            {
                HMetrics = hMetrics,
                LeftSideBearing = leftSideBearing,
            };

            return hmtx;
        }
        #endregion

        #region Read 'cmap' table
        // https://docs.microsoft.com/en-us/typography/opentype/spec/cmap#format-4-segment-mapping-to-delta-values
        private Format4 ParseFormat4()
        {
            var format4 = new Format4()
            {
                Format = 4,
                Length = getUint16(),
                Language = getUint16(),
                SegCountX2 = getUint16(),
                SearchRange = getUint16(),
                EntrySelector = getUint16(),
                RangeShift = getUint16(),
                EndCode = new List<UInt16>(),
                StartCode = new List<UInt16>(),
                IdDelta = new List<Int16>(),
                IdRangeOffset = new List<UInt16>(),
                GlyphIndexMap = new Dictionary<uint, uint>()
            };

            var segCount = format4.SegCountX2 >> 1;

            for (var i = 0; i < segCount; i++)
            {
                format4.EndCode.Add(getUint16());
            }

            getUint16(); // Reserved pad.

            for (var i = 0; i < segCount; i++)
            {
                format4.StartCode.Add(getUint16());
            }

            for (var i = 0; i < segCount; i++)
            {
                format4.IdDelta.Add(getInt16());
            }

            var idRangeOffsetsStart = position;

            for (var i = 0; i < segCount; i++)
            {
                format4.IdRangeOffset.Add(getUint16());
            }

            for (var i = 0; i < segCount - 1; i++)
            {
                uint glyphIndex = 0;
                var endCode = format4.EndCode[i];
                var startCode = format4.StartCode[i];
                var idDelta = format4.IdDelta[i];
                var idRangeOffset = format4.IdRangeOffset[i];

                for (var c = startCode; c <= endCode; c++)
                {
                    if (idRangeOffset != 0)
                    {
                        var startCodeOffset = (c - startCode) * 2;
                        var currentRangeOffset = i * 2; // 2 because the numbers are 2 byte big.

                        uint glyphIndexOffset = (uint)(idRangeOffsetsStart + idRangeOffset + currentRangeOffset + startCodeOffset);

                        SetPosition(glyphIndexOffset);
                        glyphIndex = getUint16();
                        if (glyphIndex != 0)
                        {
                            // & 0xffff is modulo 65536.
                            glyphIndex = (uint)((glyphIndex + idDelta) & 0xffff);
                        }
                    }
                    else
                    {
                        glyphIndex = (uint)((c + idDelta) & 0xffff);
                    }
                    format4.GlyphIndexMap[c] = (UInt16)glyphIndex;
                }
            }
            return format4;
        }

        // https://docs.microsoft.com/en-us/typography/opentype/spec/cmap
        public Cmap ReadCmapTable()
        {
            uint offset = ttf.Tables["cmap"].Offset;
            SetPosition(offset);

            var cmap = new Cmap()
            {
                Version = getUint16(),
                NumTables = getUint16(),
                EncodingRecords = new List<EncodingRecord>()
                //GlyphIndexMap = new Dictionary<uint, uint>()
            };

            if (cmap.Version != 0)
            {
                throw new Exception($"cmap version should be 0 but is {cmap.Version}");
            }

            for (var i = 0; i < cmap.NumTables; i++)
            {
                cmap.EncodingRecords.Add(new EncodingRecord()
                {
                    PlatformID = getUint16(),
                    EncodingID = getUint16(),
                    Offset = getOffset32()
                });
            }
            return cmap;
        }

        private Dictionary<uint, uint> GetCmapDictionary()
        {
            if (ttf.Cmap.GlyphIndexMap != null) return ttf.Cmap.GlyphIndexMap;

            foreach (var table in ttf.Cmap.EncodingRecords)
            {
                if ((table.PlatformID == 3) && (table.EncodingID == 1 || table.EncodingID == 0)) //Unicode BMP (UCS-2)  or  Symbol
                {
                    SetPosition(ttf.Tables["cmap"].Offset + table.Offset);
                    var format = getUint16();
                    if (format == 4)
                    {
                        ttf.Cmap.GlyphIndexMap = ParseFormat4().GlyphIndexMap;
                    }
                    else
                    {
                        throw new Exception($"Unsupported format: {format}.Required: 4.");
                    }
                }
            }
            if (ttf.Cmap.GlyphIndexMap == null) ttf.Cmap.GlyphIndexMap = new Dictionary<uint, uint>();

            return ttf.Cmap.GlyphIndexMap;
        }

        private ushort[] GetCmapTable()
        {
            ushort[] charToGlyph = null;

            uint tableOffset = ttf.Tables["cmap"].Offset;

            foreach (var table in ttf.Cmap.EncodingRecords)
            {
                if ((table.PlatformID == 3) && (table.EncodingID == 1)) //Unicode BMP (UCS-2)
                {
                    UInt16 formatNumber = GetUInt16(tableOffset + table.Offset);
                    if (formatNumber == 4)
                    {
                        #region cmap table Format 4
                        charToGlyph = new ushort[65536];
                        for (int index = 0; index < 65536; index++) charToGlyph[index] = 0xffff;

                        uint segCountX2 = GetUInt16(tableOffset + table.Offset + 6);

                        for (int indexSeg = 0; indexSeg < segCountX2; indexSeg += 2)
                        {
                            uint offsetSeg = (uint)(tableOffset + table.Offset + 14 + indexSeg);
                            UInt16 endCode = GetUInt16(offsetSeg);
                            UInt16 startCode = GetUInt16(offsetSeg + 2 + segCountX2);
                            Int16 idDelta = (Int16)GetUInt16(offsetSeg + 2 + segCountX2 * 2);
                            UInt16 idRangeOffset = GetUInt16(offsetSeg + 2 + segCountX2 * 3);

                            if (startCode != 0xFFFF)
                            {
                                for (int indexChar = startCode; indexChar <= endCode; indexChar++)
                                {
                                    int glyphId = 0xFFFF;
                                    if (idRangeOffset == 0)
                                    {
                                        glyphId = idDelta + indexChar;
                                    }
                                    else
                                    {
                                        uint glyphIndexAddress = (uint)(idRangeOffset + 2 * (indexChar - startCode) + (offsetSeg + 2 + segCountX2 * 3));
                                        glyphId = GetUInt16(glyphIndexAddress);
                                        if (glyphId != 0) glyphId += idDelta;
                                    }
                                    charToGlyph[indexChar] = (ushort)glyphId;
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
            return charToGlyph;
        }
        #endregion

        #region Read 'loca' table
        // https://docs.microsoft.com/en-us/typography/opentype/spec/loca
        private List<uint> ReadLocaTable()
        {
            SetPosition(ttf.Tables["loca"].Offset);

            var loca = new List<uint>();
            for (var i = 0; i < ttf.Maxp.NumGlyphs + 1; i++)
            {
                if (ttf.Head.IndexToLocFormat == 0)
                    loca.Add(getOffset16());
                else
                    loca.Add(getOffset32());
            }

            return loca;
        }
        #endregion

        #region Read 'glyf' table
        // https://docs.microsoft.com/en-us/typography/opentype/spec/glyf
        private List<GlyphHeader> ReadGlyfTable()
        {
            var loca = ReadLocaTable();

            uint offset = ttf.Tables["glyf"].Offset;
            SetPosition(offset);

            var glyf = new List<GlyphHeader>();

            for (var i = 0; i < loca.Count - 1; i++)
            {
                var multiplier = ttf.Head.IndexToLocFormat == 0 ? 2 : 1;
                var locaOffset = loca[i] * multiplier;

                SetPosition((uint)(offset + locaOffset));

                glyf.Add(new GlyphHeader()
                {
                    NumberOfContours = getInt16(),
                    XMin = getInt16(),
                    YMin = getInt16(),
                    XMax = getInt16(),
                    YMax = getInt16()
                });
            }

            return glyf;
        }
        #endregion

        #region Read 'OS/2' table
        private OS2 ReadOS2Table()
        {
            if (!ttf.Tables.ContainsKey("OS/2")) return null;

            uint offset = ttf.Tables["OS/2"].Offset;
            SetPosition(offset);

            var os2 = new OS2();

            os2.Version = getUint16();
            os2.XAvgCharWidth = getInt16();
            os2.UsWeightClass = getUint16();
            os2.UsWidthClass = getUint16();
            os2.FsType = getUint16();
            os2.YSubscriptXSize = getInt16();
            os2.YSubscriptYSize = getInt16();
            os2.YSubscriptXOffset = getInt16();
            os2.YSubscriptYOffset = getInt16();
            os2.YSuperscriptXSize = getInt16();
            os2.YSuperscriptYSize = getInt16();
            os2.YSuperscriptXOffset = getInt16();
            os2.YSuperscriptYOffset = getInt16();
            os2.YStrikeoutSize = getInt16();
            os2.YStrikeoutPosition = getInt16();
            os2.SFamilyClass = getInt16();

            os2.Panose = new byte[10];
            for (int index = 0; index < 10; index++)
            {
                os2.Panose[index] = getUint8();
            }

            os2.UlUnicodeRange1 = getUint32();
            os2.UlUnicodeRange2 = getUint32();
            os2.UlUnicodeRange3 = getUint32();
            os2.UlUnicodeRange4 = getUint32();

            os2.AchVendID = new byte[4];
            for (int index = 0; index < 4; index++)
            {
                os2.AchVendID[index] = getUint8();
            }

            os2.FsSelection = getUint16();
            os2.UsFirstCharIndex = getUint16();
            os2.UsLastCharIndex = getUint16();
            os2.STypoAscender = getInt16();
            os2.STypoDescender = getInt16();
            os2.STypoLineGap = getInt16();
            os2.UsWinAscent = getUint16();
            os2.UsWinDescent = getUint16();
            os2.UlCodePageRange1 = getUint32();
            os2.UlCodePageRange2 = getUint32();

            if (os2.Version >= 2)
            {
                os2.SxHeight = getInt16();
                os2.SCapHeight = getInt16();
                os2.UsDefaultChar = getUint16();
                os2.UsBreakChar = getUint16();
                os2.UsMaxContext = getUint16();
            }
            if (os2.Version >= 5)
            {
                os2.UsLowerOpticalPointSize = getUint16();
                os2.UsUpperOpticalPointSize = getUint16();
            }            

            return os2;
        }
        #endregion

        #endregion

        #region Utils
        private byte getUint8() => data[position++];
        private UInt16 getUint16() => (UInt16)((getUint8() << 8) | getUint8());
        private UInt32 getUint32() => (UInt32)((getUint8() << 24) | (getUint8() << 16) | (getUint8() << 8) | getUint8());
        private Int16 getInt16()
        {
            UInt16 number = getUint16();
            if ((number & 0x8000) != 0) return (Int16)(number - (1 << 16));
            return (Int16)number;
        }

        private int getInt32() => (getUint8() << 24) | (getUint8() << 16) | (getUint8() << 8) | getUint8();

        private Int16 getFWord() => getInt16();
        private UInt16 getUFWord() => getUint16();
        private UInt16 getOffset16() => getUint16();
        private UInt32 getOffset32() => getUint32();
        private Int16 getF2Dot14() => getInt16();  //  / (1 << 14); need float
        private Int32 getFixed() => getInt32();  //    / (1 << 16); need float

        private string GetString(int length)
        {
            var byteArray = new byte[length];
            for (var i = 0; i < length; i++)
            {
                byteArray[i] = getUint8();
            }

            return Encoding.UTF8.GetString(byteArray);
        }
        private string GetStringUtf16(int length)
        {
            StringBuilder sb = new StringBuilder();
            for (var i = 0; i < length/2; i++)
            {
                sb.Append((char)getUint16());
            }
            return sb.ToString();
        }

        private DateTime getDate()
        {
            var macTime = getUint32() * 0x100000000 + getUint32();
            //var utcTime = macTime * 1000 + (new DateTime(1904, 1, 1).Millisecond);
            //return new DateTime(utcTime);
            return new DateTime();
        }

        //private long CalculateTableChecksum(int offset, int length)
        //{
        //    var old = GetPosition();
        //    SetPosition(offset);
        //    long sum = 0;
        //    var nlongs = ((length + 3) / 4) | 0;
        //    while (nlongs > 0)
        //    {
        //        sum = ((sum + getUint32()) & 0xffffffff) >> 0;
        //        nlongs -= 1;
        //    }
        //    SetPosition(old);
        //    return sum;
        //}

        private void SetPosition(uint pos) => position = pos;

        private UInt16 GetUInt16(uint pos) => (UInt16)((data[pos] << 8) | data[pos + 1]);
        private UInt32 GetUInt32(uint pos) => (UInt32)((data[pos] << 24) | (data[pos + 1] << 16) | (data[pos + 2] << 8) | data[pos + 3]);
        #endregion

        #region Fields
        private uint position = 0;
        private byte[] data = null;
        private TtfInfo ttf = null;
        #endregion

        #region Properties
        public TtfInfo Ttf => ttf;
        public byte[] Data => data;
        #endregion

#pragma warning restore CS3003 // Type is not CLS-compliant

        #region Main.ReadFont
        public TtfInfo ReadFont(int position = 0)
        {
            this.position = (uint)position;

            ttf = new TtfInfo();

            ttf.HeaderOffset = (uint)position;
            ttf.SfntVersion = getUint32(); // scalarType
            ttf.NumTables = getUint16();
            getUint16(); // searchRange
            getUint16(); // entrySelector
            getUint16(); // rangeShift

            ttf.Tables = new Dictionary<string, Table>();
            for (var i = 0; i < ttf.NumTables; i++)
            {
                var tag = getUint32();
                this.position -= 4;
                var tagString = GetString(4);
                ttf.Tables[tagString] = new Table()
                {
                    Tag = tag,
                    TagString = tagString,
                    Checksum = getUint32(),
                    Offset = getUint32(),
                    Length = getUint32()
                };

                //if (tag != "head")
                //{
                //    var calculatedChecksum = CalculateTableChecksum(
                //      ttf.Tables[tag].Offset,
                //      ttf.Tables[tag].Length
                //    );
                //    var checksum = ttf.Tables[tag].Checksum;
                //    if (calculatedChecksum != checksum)
                //    {
                //        throw new Exception($"Checksum doesn't match for table {tag}");
                //    }
                //}
            }

            ttf.Head = ReadHeadTable();
            ttf.Name = ReadNameTable();
            ttf.Maxp = ReadMaxpTable();
            ttf.Hhea = ReadHheaTable();
            ttf.Hmtx = ReadHmtxTable(ttf.Hhea.NumOfLongHorMetrics, ttf.Maxp.NumGlyphs);
            ttf.Cmap = ReadCmapTable();
            ttf.OS2 = ReadOS2Table();

            //var Loca = ReadLocaTable();
            //var Glyf = ReadGlyfTable();

            return ttf;
        }
        #endregion

        #region Main.ScanFontFile
        /// <summary>
        /// Scan binary array for font file data
        /// </summary>
        /// <param name="fontName">Name of the font. Used for select font from TTC.</param>
        /// <returns>TtfInfo data</returns>
        public TtfInfo ScanFontFile(string fontName)
        {
            uint tag = GetUInt32(0);
            if ((tag == 0x00010000) || (tag == 0x4F54544F)) //sfnt version 1.0 or OTF 'OTTO'
            {
                //TTF font
                return ReadFont(0);
            }
            else
            {
                uint ttcVer = GetUInt32(4);
                if ((tag == 0x74746366) && ((ttcVer == 0x00010000) || (ttcVer == 0x00020000))) //ttcf version 1.0 or 2.0
                {
                    #region TTC font
                    uint numFontsTtc = GetUInt32(8);
                    for (uint indexTtc = numFontsTtc; indexTtc > 0; indexTtc--)
                    {
                        uint offset = GetUInt32(12 + (indexTtc - 1) * 4);
                        uint tagTtc = GetUInt32(offset);
                        if ((tagTtc == 0x00010000) || (tagTtc == 0x4F54544F)) //sfnt version 1.0 or OTF 'OTTO'
                        {
                            //TTC font
                            ReadFont((int)offset);
                            if (ttf.Name.FontNames.ContainsValue(fontName))
                            {
                                return ttf;
                            }
                        }
                    }
                    #endregion
                }
            }
            return null;
        }

        public static TtfInfo ScanFontFile(byte[] data, string fontName)
        {
            var fontReader = new StiFontReader(data);
            var font = fontReader.ScanFontFile(fontName);
            if (font != null)
            {
                font.FontReader = fontReader;
            }
            return font;
        }
        #endregion

        public StiFontReader(byte[] data)
        {
            this.data = data;
        }
    }
}
