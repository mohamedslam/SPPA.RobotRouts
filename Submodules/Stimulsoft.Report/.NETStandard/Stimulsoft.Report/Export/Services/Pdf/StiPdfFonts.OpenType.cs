#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft   							}
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft 

using System;
using System.Text;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Stimulsoft.Base.Drawing;
using System.Linq;

namespace Stimulsoft.Report.Export
{
    internal partial class PdfFonts
    {
        internal class StiOpenTypeHelper
        {

            #region Constants
            private const int TtfHeaderSize = 12;

            //OpenType font flags field
            private const UInt16 ARG_1_AND_2_ARE_WORDS = 0x0001;
            private const UInt16 ARGS_ARE_XY_VALUES = 0x0002;
            private const UInt16 ROUND_XY_TO_GRID = 0x0004;
            private const UInt16 WE_HAVE_A_SCALE = 0x0008;
            private const UInt16 MORE_COMPONENTS = 0x0020;
            private const UInt16 WE_HAVE_AN_X_AND_Y_SCALE = 0x0040;
            private const UInt16 WE_HAVE_A_TWO_BY_TWO = 0x0080;
            private const UInt16 WE_HAVE_INSTRUCTIONS = 0x0100;
            private const UInt16 USE_MY_METRICS = 0x0200;
            private const UInt16 OVERLAP_COMPOUND = 0x0400;
            private const UInt16 SCALED_COMPONENT_OFFSET = 0x0800;
            private const UInt16 UNSCALED_COMPONENT_OFFSET = 0x1000;

            #endregion

            #region Fields
            private byte[] bufGetData = new byte[4];

            private static List<string> RequiredTablesNames = new List<string>
            {
                "head", //+		+ 
                "hhea", //+		+
                "hmtx", //+		+
                "maxp", //+		+
                "cmap", //+		
                "OS/2", //+		
                "post", //+		
                "cvt ", //++	++
                "fpgm", //++	++
                "glyf", //+		+
                "loca", //+		+
                "prep", //++	++
                "name" //this table required for Windows
            };
            #endregion

            #region Methods

            #region Reduce font size
            public void ReduceFontSize(ref byte[] buff, string fontName, bool remakeGlyphTable, ushort[] GlyphList, ushort[] GlyphRtfList)
            {
                try
                {
                    var ttf = StiFontReader.ScanFontFile(buff, fontName);
                    if (ttf != null)
                    {
                        List<string> tablesNames = new List<string>(RequiredTablesNames);
                        if (!ttf.Tables.ContainsKey("glyf"))
                        {
                            if (ttf.Tables.ContainsKey("CFF "))
                            {
                                tablesNames.Add("CFF ");
                            }
                            else
                            {
                                remakeGlyphTable = false;
                            }
                        }

                        int numTablesRequired = 0;
                        foreach (var table in ttf.Tables)
                        {
                            if (tablesNames.Contains(table.Key)) numTablesRequired++;
                        }

                        var newTables = new Dictionary<string, NewTableItem>();

                        if (remakeGlyphTable)
                        {
                            #region remake tables

                            #region prepare array of glyphs need
                            bool[] glyphNeeds = new bool[ttf.Maxp.NumGlyphs];
                            bool[] glyphNeedsScan = new bool[ttf.Maxp.NumGlyphs];
                            for (int indexGlyph = 0; indexGlyph < GlyphList.Length; indexGlyph++)
                            {
                                ushort glyphNumber = GlyphList[indexGlyph];
                                if (glyphNumber < ttf.Maxp.NumGlyphs)
                                {
                                    glyphNeeds[glyphNumber] = true;
                                    glyphNeedsScan[glyphNumber] = true;
                                }
                            }
                            if (GlyphRtfList != null)
                            {
                                for (int indexGlyph = 0; indexGlyph < GlyphRtfList.Length; indexGlyph++)
                                {
                                    ushort glyphNumber = GlyphRtfList[indexGlyph];
                                    if (glyphNumber < ttf.Maxp.NumGlyphs)
                                    {
                                        glyphNeeds[glyphNumber] = true;
                                        glyphNeedsScan[glyphNumber] = true;
                                    }
                                }
                            }
                            glyphNeeds[0] = true;
                            glyphNeeds[1] = true;
                            glyphNeeds[2] = true;
                            glyphNeeds[3] = true;
                            glyphNeedsScan[0] = true;
                            glyphNeedsScan[1] = true;
                            glyphNeedsScan[2] = true;
                            glyphNeedsScan[3] = true;
                            #endregion

                            if (ttf.Tables.ContainsKey("glyf"))
                            {
                                var locaTable = ttf.Tables["loca"];
                                var glyfTable = ttf.Tables["glyf"];

                                #region scan glyf table for composite glyph
                                bool needScan = true;
                                while (needScan)
                                {
                                    needScan = false;
                                    for (int indexGlyph = 0; indexGlyph < ttf.Maxp.NumGlyphs; indexGlyph++)
                                    {
                                        if (glyphNeedsScan[indexGlyph] == true)
                                        {
                                            glyphNeedsScan[indexGlyph] = false;
                                            int locaOffsetScan = (int)(locaTable.Offset + indexGlyph * (ttf.Head.IndexToLocFormat == 1 ? 4 : 2));
                                            int offsetGlyf = 0;
                                            if (ttf.Head.IndexToLocFormat == 1)
                                            {
                                                offsetGlyf = (int)GetUInt32(buff, locaOffsetScan);
                                            }
                                            else
                                            {
                                                offsetGlyf = 2 * GetUInt16(buff, locaOffsetScan);
                                            }
                                            offsetGlyf += (int)glyfTable.Offset;
                                            if (GetInt16(buff, offsetGlyf) == -1) //composite glyph
                                            {
                                                needScan = true;

                                                #region scan composite glyph
                                                offsetGlyf += 10; //glyf header length
                                                UInt16 flags = 0;
                                                do
                                                {
                                                    flags = GetUInt16(buff, offsetGlyf);
                                                    UInt16 glyphIndex = GetUInt16(buff, offsetGlyf + 2);
                                                    if (glyphIndex < ttf.Maxp.NumGlyphs)
                                                    {
                                                        glyphNeeds[glyphIndex] = true;
                                                        glyphNeedsScan[glyphIndex] = true;
                                                    }
                                                    offsetGlyf += 4;

                                                    if ((flags & ARG_1_AND_2_ARE_WORDS) > 0)
                                                    {
                                                        offsetGlyf += 2; //argument1;
                                                        offsetGlyf += 2; //argument2;
                                                    }
                                                    else
                                                    {
                                                        offsetGlyf += 2; //argument 1 and 2;
                                                    }
                                                    if ((flags & WE_HAVE_A_SCALE) > 0)
                                                    {
                                                        offsetGlyf += 2; // Format 2.14
                                                    }
                                                    else
                                                    {
                                                        if ((flags & WE_HAVE_AN_X_AND_Y_SCALE) > 0)
                                                        {
                                                            offsetGlyf += 2; // Format 2.14
                                                            offsetGlyf += 2; // Format 2.14
                                                        }
                                                        else
                                                        {
                                                            if ((flags & WE_HAVE_A_TWO_BY_TWO) > 0)
                                                            {
                                                                offsetGlyf += 2; // Format 2.14
                                                                offsetGlyf += 2; // Format 2.14
                                                                offsetGlyf += 2; // Format 2.14
                                                                offsetGlyf += 2; // Format 2.14
                                                            }
                                                        }
                                                    }
                                                } while ((flags & MORE_COMPONENTS) > 0);
                                                #endregion
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region remake glyf table
                                //first pass - calculate new length
                                int newLength = 0;
                                int locaOffset = (int)locaTable.Offset;
                                for (int indexGlyph = 0; indexGlyph < ttf.Maxp.NumGlyphs; indexGlyph++)
                                {
                                    int offsetBegin = 0;
                                    int offsetEnd = 0;
                                    if (ttf.Head.IndexToLocFormat == 1)
                                    {
                                        offsetBegin = (int)GetUInt32(buff, locaOffset);
                                        offsetEnd = (int)GetUInt32(buff, locaOffset + 4);
                                        locaOffset += 4;
                                    }
                                    else
                                    {
                                        offsetBegin = 2 * GetUInt16(buff, locaOffset);
                                        offsetEnd = 2 * GetUInt16(buff, locaOffset + 2);
                                        locaOffset += 2;
                                    }
                                    if (glyphNeeds[indexGlyph])
                                    {
                                        newLength += offsetEnd - offsetBegin;
                                    }
                                }

                                //second pass - create new table
                                byte[] newGlyfTable = new byte[newLength + 4];
                                if (glyfTable.Offset + glyfTable.Length + 4 < buff.Length)
                                {
                                    Array.Copy(buff, glyfTable.Offset + glyfTable.Length, newGlyfTable, newLength, 4);
                                }
                                byte[] newLocaTable = new byte[locaTable.Length + 4];
                                if (locaTable.Offset + locaTable.Length + 4 < buff.Length)
                                {
                                    Array.Copy(buff, locaTable.Offset + locaTable.Length, newLocaTable, locaTable.Length, 4);
                                }
                                uint currentGlyfPos = 0;
                                locaOffset = (int)locaTable.Offset;
                                int locaOffset2 = 0;
                                for (int indexGlyph = 0; indexGlyph < ttf.Maxp.NumGlyphs; indexGlyph++)
                                {
                                    int offsetBegin = 0;
                                    int offsetEnd = 0;
                                    if (ttf.Head.IndexToLocFormat == 1)
                                    {
                                        offsetBegin = (int)GetUInt32(buff, locaOffset);
                                        offsetEnd = (int)GetUInt32(buff, locaOffset + 4);
                                        locaOffset += 4;
                                    }
                                    else
                                    {
                                        offsetBegin = 2 * GetUInt16(buff, locaOffset);
                                        offsetEnd = 2 * GetUInt16(buff, locaOffset + 2);
                                        locaOffset += 2;
                                    }
                                    int lengthGlyf = offsetEnd - offsetBegin;

                                    if (glyphNeeds[indexGlyph] == true)
                                    {
                                        for (int indexPos = 0; indexPos < lengthGlyf; indexPos++)
                                        {
                                            newGlyfTable[currentGlyfPos + indexPos] = buff[glyfTable.Offset + offsetBegin + indexPos];
                                        }
                                    }
                                    else
                                    {
                                        lengthGlyf = 0;
                                    }

                                    if (ttf.Head.IndexToLocFormat == 0)
                                    {
                                        SetUInt16(newLocaTable, locaOffset2, (ushort)(currentGlyfPos / 2));
                                    }
                                    else
                                    {
                                        SetUInt32(newLocaTable, locaOffset2, currentGlyfPos);
                                    }
                                    currentGlyfPos += (uint)lengthGlyf;
                                    locaOffset2 += (ttf.Head.IndexToLocFormat == 1) ? 4 : 2;
                                }
                                if (ttf.Head.IndexToLocFormat == 0)
                                {
                                    SetUInt16(newLocaTable, locaOffset2, (ushort)(currentGlyfPos / 2));
                                }
                                else
                                {
                                    SetUInt32(newLocaTable, locaOffset2, currentGlyfPos);
                                }

                                var newGlyfTableItem = new NewTableItem() { Tag = glyfTable.Tag, NewLength = currentGlyfPos, NewTable = newGlyfTable };
                                var newLocaTableItem = new NewTableItem() { Tag = locaTable.Tag, NewLength = locaTable.Length, NewTable = newLocaTable };
                                newTables["glyf"] = newGlyfTableItem;
                                newTables["loca"] = newLocaTableItem;
                                #endregion
                            }
                            else
                            {
                                var cffTable = ttf.Tables["CFF "];

                                var cff = new Cff();
                                cff.Read(buff, (int)cffTable.Offset, (int)cffTable.Length);

                                cff.GlyphsNeed = glyphNeeds;
                                cff.CutUnusedSubr();

                                byte[] newCffBytes = cff.Save();

                                var newCffTableItem = new NewTableItem() { Tag = cffTable.Tag, NewLength = (uint)newCffBytes.Length, NewTable = newCffBytes };
                                newTables["CFF "] = newCffTableItem;
                            }
                            #endregion
                        }

                        byte[] newHeader = new byte[TtfHeaderSize + numTablesRequired * 16];
                        Array.Copy(buff, ttf.HeaderOffset, newHeader, 0, newHeader.Length);

                        #region make new offset table
                        int maximumPower = 1;
                        while (2 << (maximumPower - 1) <= numTablesRequired)
                        {
                            maximumPower++;
                        }
                        maximumPower--;
                        int searchRange = (2 << (maximumPower - 1)) * 16;
                        int entrySelector = maximumPower;
                        int rangeShift = numTablesRequired * 16 - searchRange;
                        SetUInt16(newHeader, 4, (UInt16)numTablesRequired);
                        SetUInt16(newHeader, 6, (UInt16)searchRange);
                        SetUInt16(newHeader, 8, (UInt16)entrySelector);
                        SetUInt16(newHeader, 10, (UInt16)rangeShift);
                        #endregion

                        #region make new table info
                        //uint globalCheckSum = 0;
                        uint currentTable = 0;
                        uint currentPos = (uint)(TtfHeaderSize + numTablesRequired * 16);
                        foreach (var table in ttf.Tables.Values)
                        {
                            if (tablesNames.Contains(table.TagString))
                            {
                                //calculate checksum for this table
                                //uint checkSum = 0;
                                //uint lengthTable = (ttf.Tables[indexTable].NewLength + (remainder > 0 ? 4 - remainder : 0)) / 4;
                                //byte[] buff2 = buff;
                                //int offsetCheck = (int)ttf.Tables[indexTable].Offset;
                                //if (ttf.Tables[indexTable].NewTable != null)
                                //{
                                //    buff2 = ttf.Tables[indexTable].NewTable;
                                //    offsetCheck = 0;
                                //}
                                //for (int indexLen = 0; indexLen < lengthTable; indexLen++)
                                //{
                                //    checkSum += GetUInt32(buff2, offsetCheck);
                                //    offsetCheck += 4;
                                //}
                                //ttf.Tables[indexTable].CheckSum = checkSum;
                                //globalCheckSum += checkSum;

                                uint tableLength = table.Length;
                                if (newTables.ContainsKey(table.TagString)) tableLength = newTables[table.TagString].NewLength;

                                int pos = (int)(TtfHeaderSize + currentTable * 16);
                                SetUInt32(newHeader, pos, table.Tag);
                                SetUInt32(newHeader, pos + 4, table.Checksum);
                                SetUInt32(newHeader, pos + 8, currentPos);
                                SetUInt32(newHeader, pos + 12, tableLength);

                                currentPos += tableLength;
                                uint remainder = currentPos % 4;
                                if (remainder > 0)
                                {
                                    currentPos += 4 - remainder;
                                }
                                currentTable++;
                            }
                        }
                        #endregion

                        #region correct global checksum
                        //for (int indexSum = 0; indexSum < (ttf.HeaderOffset + TtfHeaderSize + ttf.NumTablesRequired * 16) / 4; indexSum ++)
                        //{
                        //    globalCheckSum += GetUInt32(buff, (int)(indexSum * 4));
                        //}
                        //globalCheckSum = 0xB1B0AFBA - globalCheckSum;
                        //SetUInt32(newHeader, ttf.HeadCheckSumOffset, globalCheckSum);
                        #endregion

                        #region write new font
                        using (MemoryStream mem = new MemoryStream())
                        {
                            mem.Write(newHeader, 0, newHeader.Length);

                            foreach (var table in ttf.Tables.Values)
                            {
                                if (tablesNames.Contains(table.TagString))
                                {
                                    uint tableLength = table.Length;
                                    if (newTables.ContainsKey(table.TagString))
                                    {
                                        tableLength = newTables[table.TagString].NewLength;
                                        mem.Write(newTables[table.TagString].NewTable, 0, (int)tableLength);
                                    }
                                    else
                                    {
                                        mem.Write(buff, (int)table.Offset, (int)table.Length);
                                    }

                                    int remainder = (int)tableLength % 4;
                                    if (remainder > 0)
                                    {
                                        mem.Write(new byte[4 - remainder], 0, 4 - remainder);
                                    }
                                }
                            }
                            mem.Flush();
                            buff = mem.ToArray();
                            mem.Close();
                        }
                        #endregion
                    }
                }
                catch { } //nothing to change, return original buffer
            }

            public struct NewTableItem
            {
                public uint Tag;
                public uint CheckSum;
                public uint NewOffset;
                public uint NewLength;
                public byte[] NewTable;
            }
            #endregion

            #region GetCmapTable
            public Dictionary<uint, uint> GetCharToGlyphTable(byte[] buff, string fontName)
            {
                var ttf = Stimulsoft.Base.Drawing.StiFontReader.ScanFontFile(buff, fontName);
                if (ttf != null)
                {
                    return ttf.GetCmapDictionary();
                }
                return null;
            }
            #endregion

            #region GetData & SetData
            //private byte GetUInt8(byte[] buff, int pos)
            //{
            //    return buff[pos];
            //}

            private UInt16 GetUInt16(byte[] buff, int pos)
            {
                bufGetData[0] = buff[pos + 1];
                bufGetData[1] = buff[pos + 0];
                return BitConverter.ToUInt16(bufGetData, 0);
            }

            private UInt32 GetUInt32(byte[] buff, int pos)
            {
                bufGetData[0] = buff[pos + 3];
                bufGetData[1] = buff[pos + 2];
                bufGetData[2] = buff[pos + 1];
                bufGetData[3] = buff[pos + 0];
                return BitConverter.ToUInt32(bufGetData, 0);
            }

            //private sbyte GetInt8(byte[] buff, int pos)
            //{
            //    return (sbyte)buff[pos];
            //}
            private Int16 GetInt16(byte[] buff, int pos)
            {
                return (Int16)GetUInt16(buff, pos);
            }

            //private Int32 GetInt32(byte[] buff, int pos)
            //{
            //    return (Int32)GetUInt32(buff, pos);
            //}

            private void SetUInt16(byte[] buff, int pos, UInt16 value)
            {
                BitConverter.GetBytes(value).CopyTo(bufGetData, 0);
                buff[pos + 0] = bufGetData[1];
                buff[pos + 1] = bufGetData[0];
            }

            private void SetUInt32(byte[] buff, int pos, UInt32 value)
            {
                BitConverter.GetBytes(value).CopyTo(bufGetData, 0);
                buff[pos + 0] = bufGetData[3];
                buff[pos + 1] = bufGetData[2];
                buff[pos + 2] = bufGetData[1];
                buff[pos + 3] = bufGetData[0];
            }
            #endregion

            #endregion

            #region Class Cff
            public class Cff
            {
                #region Block
                public class Block
                {
                    public Cff Cff;
                    public Int32 Cff_Offset;
                    public Int32 Cff_Size;
                    public Int32 Cff_NewOffset;
                    public Int32 Cff_NewSize;
                    public string Name;

                    public virtual void Read(int offset, int length)
                    {
                        Cff_Offset = offset;
                        Cff_Size = length;
                    }

                    public virtual int GetSize(bool onlyMain = false)
                    {
                        return Cff_Size;
                    }

                    public virtual void Save(bool main = false)
                    {
                        Cff_NewOffset = Cff.newBytesOffset;
                        Cff_NewSize = Cff_Size;

                        Array.Copy(Cff.fontBytes, Cff_Offset, Cff.newBytes, Cff.newBytesOffset, Cff_Size);
                        Cff.newBytesOffset += Cff_Size;
                    }

                    public override string ToString()
                    {
                        return $"{Name} ({Cff_Offset}, {Cff_Size})";
                    }

                    public Block(Cff cff, string name, bool addToPool = true)
                    {
                        this.Cff = cff;
                        this.Name = name;
                        if (addToPool) Cff.TablesList.Add(this);
                    }
                }
                #endregion

                #region Index
                public class Index : Block
                {
                    public ushort Count;
                    public byte OffSize;
                    public UInt32[] Offsets;

                    public override void Read(int offset, int length)
                    {
                        Cff.fontBytesOffset = offset;
                        Cff_Offset = offset;
                        Cff_Size = 2;

                        Count = Cff.getCard16();
                        if (Count > 0)
                        {
                            OffSize = Cff.getOffSize();
                            Offsets = new UInt32[Count + 1];
                            uint addOffset = (uint)(2 + 1 + (Count + 1) * OffSize - 1);
                            for (int i = 0; i < Count + 1; i++)
                            {
                                Offsets[i] = Cff.getOffset(OffSize) + addOffset;
                            }

                            Cff_Size = (int)Offsets[Offsets.Length - 1];

                            Cff.fontBytesOffset = Cff_Offset + Cff_Size;
                        }
                    }

                    public Index(Cff cff, string name, bool addToPool = true) : base(cff, name, addToPool)
                    {
                    }
                }
                #endregion

                #region Dict
                public class Dict : Block
                {
                    #region DictItem
                    public class DictItem
                    {
                        public uint Operator;
                        public List<object> Operands;
                        public int RawDataOffset;
                        public int RawDataLength;
                        public byte[] RawData;

                        public override string ToString()
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int index = 0; index < Operands.Count; index++)
                            {
                                sb.Append(Operands[index]);
                                if (index < Operands.Count - 1) sb.Append(", ");
                            }
                            return $"{Operator} [{sb}]";
                        }
                    }
                    #endregion

                    public Dictionary<uint, DictItem> Items;
                    public Dictionary<uint, int> Adresses;

                    public Block Charset;
                    public Block Encoding;
                    public Cff_CharStringsIndex CharStringsIndex;
                    public Dict Private;
                    public DictIndex FontDictIndex;
                    public Block FDSelect;
                    public Index SubrIndex;

                    public override void Read(int offset, int length)
                    {
                        Cff_Offset = offset;
                        Cff_Size = length;

                        #region Read dict
                        Items = new Dictionary<uint, DictItem>();

                        Cff.fontBytesOffset = offset;
                        int lastPos = offset;
                        List<object> operands = new List<object>();

                        while (Cff.fontBytesOffset < offset + length)
                        {
                            byte b0 = Cff.getCard8();
                            if (b0 <= 21)   //operators
                            {
                                var item = new DictItem();
                                item.Operator = b0;
                                if (b0 == 12)
                                {
                                    item.Operator = (ushort)((b0 << 8) | Cff.getCard8());
                                }
                                item.Operands = operands;
                                item.RawDataOffset = lastPos;
                                item.RawDataLength = Cff.fontBytesOffset - lastPos;
                                Items.Add(item.Operator, item);

                                operands = new List<object>();
                                lastPos = Cff.fontBytesOffset;
                            }
                            if (b0 >= 28 && b0 <= 254 && b0 != 31)  //operands
                            {
                                object operand = null;
                                if (b0 >= 32 && b0 <= 246) operand = b0 - 139;
                                if (b0 >= 247 && b0 <= 250) operand = (b0 - 247) * 256 + Cff.getCard8() + 108;
                                if (b0 >= 251 && b0 <= 254) operand = -(b0 - 251) * 256 - Cff.getCard8() - 108;
                                if (b0 == 28) operand = (short)((Cff.getCard8() << 8) | Cff.getCard8());
                                if (b0 == 29) operand = (int)((Cff.getCard8() << 24) | (Cff.getCard8() << 16) | (Cff.getCard8() << 8) | Cff.getCard8());
                                if (b0 == 30) operand = Cff.getReal();

                                operands.Add(operand);
                            }
                            //22-27, 31, 255 - reserved
                        }
                        #endregion

                        #region Read childs
                        if (Items.ContainsKey(17))    //CharStringsIndex
                        {
                            var charStringsOffset = global::System.Convert.ToInt32(Items[17].Operands[0]);
                            CharStringsIndex = new Cff_CharStringsIndex(Cff, "CharStringsIndex");
                            CharStringsIndex.Read(Cff.Offset + charStringsOffset, 0);
                            Items[17].RawData = new byte[] { 29, 0, 0, 0, 0, 17 };
                        }

                        if (Items.ContainsKey(15))    //Charset
                        {
                            var charsetOffset = global::System.Convert.ToInt32(Items[15].Operands[0]);
                            if (charsetOffset > 3)  //not common charset
                            {
                                #region Get Charset block
                                charsetOffset += Cff.Offset;
                                byte charsetFormat = Cff.GetUInt8(Cff.fontBytes, charsetOffset);
                                int charsetLength = 0;
                                if (charsetFormat == 0)
                                {
                                    charsetLength = 1 + (CharStringsIndex.Count - 1) * 2;
                                }
                                if (charsetFormat == 1 || charsetFormat == 2)
                                {
                                    Cff.fontBytesOffset = charsetOffset + 1;
                                    int countGlyphs = 1;
                                    while (countGlyphs < CharStringsIndex.Count)
                                    {
                                        Cff.getCard16();    //SID
                                        int nLeft = charsetFormat == 1 ? Cff.getCard8() : Cff.getCard16();
                                        countGlyphs += nLeft + 1;
                                    }
                                    charsetLength = Cff.fontBytesOffset - charsetOffset;
                                }
                                //if (charsetLength > 0)
                                //{
                                Charset = new Block(Cff, "Charset");
                                Charset.Cff_Offset = charsetOffset;
                                Charset.Cff_Size = charsetLength;
                                //}
                                #endregion

                                Items[15].RawData = new byte[] { 29, 0, 0, 0, 0, 15 };
                            }
                        }

                        if (Items.ContainsKey(16))    //Encoding
                        {
                            var encodingOffset = global::System.Convert.ToInt32(Items[16].Operands[0]);
                            if (encodingOffset > 1)  //not common encoding
                            {
                                #region Get Charset block
                                encodingOffset += Cff.Offset;
                                byte encodingFormat = Cff.GetUInt8(Cff.fontBytes, encodingOffset);
                                int encodingLength = 0;
                                if (encodingFormat == 0)
                                {
                                    encodingLength = 2 + Cff.GetUInt8(Cff.fontBytes, encodingOffset + 1);
                                }
                                if (encodingFormat == 1)
                                {
                                    encodingLength = 2 + Cff.GetUInt8(Cff.fontBytes, encodingOffset + 1) * 2;
                                }
                                if ((encodingFormat & 0x80) > 0)
                                {
                                    encodingLength = 2 + Cff.GetUInt8(Cff.fontBytes, encodingOffset + 1) * 3;
                                }
                                //if (encodingLength > 0)
                                //{
                                Encoding = new Block(Cff, "Encoding");
                                Encoding.Cff_Offset = encodingOffset;
                                Encoding.Cff_Size = encodingLength;
                                //}
                                #endregion

                                Items[16].RawData = new byte[] { 29, 0, 0, 0, 0, 16 };
                            }
                        }

                        if (Items.ContainsKey(18))    //PrivateDict
                        {
                            var privateOffset = global::System.Convert.ToInt32(Items[18].Operands[1]);
                            var privateLength = global::System.Convert.ToInt32(Items[18].Operands[0]);
                            Private = new Dict(Cff, "PrivateDict");
                            Private.Read(Cff.Offset + privateOffset, privateLength);
                            Items[18].RawData = new byte[] { 29, 0, 0, 0, 0, 29, 0, 0, 0, 0, 18 };
                        }

                        if (Items.ContainsKey(19))  //Subr
                        {
                            var subrOffset = global::System.Convert.ToInt32(Items[19].Operands[0]);
                            SubrIndex = new Index(Cff, "Subr");
                            SubrIndex.Read(Cff_Offset + subrOffset, 0);
                            Items[19].RawData = new byte[] { 29, 0, 0, 0, 0, 19 };
                        }

                        if (Items.ContainsKey(0x0c24))    //FontDictIndex
                        {
                            var fontDictIndexOffset = global::System.Convert.ToInt32(Items[0x0c24].Operands[0]);
                            FontDictIndex = new DictIndex(Cff, "FontDictIndex");
                            FontDictIndex.Read(Cff.Offset + fontDictIndexOffset, 0);
                            Items[0x0c24].RawData = new byte[] { 29, 0, 0, 0, 0, 12, 36 };
                        }

                        if (Items.ContainsKey(0x0c25))    //FDSelect
                        {
                            var fDSelectOffset = Cff.Offset + global::System.Convert.ToInt32(Items[0x0c25].Operands[0]);

                            #region Get FDSelect block
                            byte fDSelectFormat = Cff.GetUInt8(Cff.fontBytes, fDSelectOffset);
                            int fDSelectLength = 0;
                            if (fDSelectFormat == 0)
                            {
                                fDSelectLength = 1 + (CharStringsIndex.Count - 1);
                            }
                            if (fDSelectFormat == 3)
                            {
                                fDSelectLength = 1 + 2 + Cff.GetUInt16(Cff.fontBytes, fDSelectOffset + 1) * 3 + 2;
                            }
                            //if (fDSelectLength > 0)
                            //{
                            FDSelect = new Block(Cff, "FDSelect");
                            FDSelect.Cff_Offset = fDSelectOffset;
                            FDSelect.Cff_Size = fDSelectLength;
                            //}
                            #endregion

                            Items[0x0c25].RawData = new byte[] { 29, 0, 0, 0, 0, 12, 37 };
                        }
                        #endregion
                    }

                    public override void Save(bool main = false)
                    {
                        if (main)
                        {
                            #region Save main
                            Cff_NewOffset = Cff.newBytesOffset;

                            Adresses = new Dictionary<uint, int>();
                            MemoryStream ms = new MemoryStream();
                            foreach (var dictItem in Items)
                            {
                                Adresses[dictItem.Key] = (int)(Cff.newBytesOffset + ms.Position);

                                if (dictItem.Value.RawData == null)
                                    ms.Write(Cff.fontBytes, dictItem.Value.RawDataOffset, dictItem.Value.RawDataLength);
                                else
                                    ms.Write(dictItem.Value.RawData, 0, dictItem.Value.RawData.Length);
                            }
                            byte[] buf = ms.ToArray();
                            ms.Close();

                            Array.Copy(buf, 0, Cff.newBytes, Cff.newBytesOffset, buf.Length);
                            Cff.newBytesOffset += buf.Length;
                            Cff_NewSize = buf.Length;
                            #endregion
                        }
                        else
                        {
                            #region Save childs
                            if (Charset != null)
                            {
                                StoreOffset(15);
                                Charset.Save();
                            }
                            if (Encoding != null)
                            {
                                StoreOffset(16);
                                Encoding.Save();
                            }
                            if (FDSelect != null)
                            {
                                StoreOffset(0x0c25);
                                FDSelect.Save();
                            }
                            if (CharStringsIndex != null)
                            {
                                StoreOffset(17);
                                CharStringsIndex.Save();
                            }
                            if (Private != null)
                            {
                                StoreOffset(18, 2);
                                Private.Save(true);
                                StoreOffset(18, 1, Private.Cff_NewSize);
                                Private.Save();
                            }
                            if (FontDictIndex != null)
                            {
                                StoreOffset(0x0c24);
                                FontDictIndex.Save(true);
                                FontDictIndex.Save();
                            }
                            if (SubrIndex != null)
                            {
                                StoreOffset(19, 1, Cff_NewOffset);  //local, do not need to change
                                SubrIndex.Save();
                            }
                            #endregion
                        }
                    }

                    public override int GetSize(bool onlyMain = false)
                    {
                        int size = 0;
                        foreach (var dictItem in Items)
                        {
                            if (dictItem.Value.RawData == null)
                                size += dictItem.Value.RawDataLength;
                            else
                                size += dictItem.Value.RawData.Length;
                        }

                        if (!onlyMain)
                        {
                            #region Childs
                            if (CharStringsIndex != null)
                            {
                                size += CharStringsIndex.GetSize();
                            }
                            if (Charset != null)
                            {
                                size += Charset.GetSize();
                            }
                            if (Encoding != null)
                            {
                                size += Encoding.GetSize();
                            }
                            if (Private != null)
                            {
                                size += Private.GetSize();
                            }
                            if (FontDictIndex != null)
                            {
                                size += FontDictIndex.GetSize();
                            }
                            if (FDSelect != null)
                            {
                                size += FDSelect.GetSize();
                            }
                            if (SubrIndex != null)
                            {
                                size += SubrIndex.GetSize();
                            }
                            #endregion
                        }

                        return size;
                    }

                    private void StoreOffset(uint code, int pos = 1, int arg = -1)
                    {
                        var newOffset = Cff.newBytesOffset;
                        var addr = Adresses[code];
                        byte bb;

                        if (pos > 1)
                        {
                            addr += 5;
                        }
                        if (code == 18 && pos == 1)
                        {
                            newOffset = arg;
                        }
                        if (code == 19)
                        {
                            newOffset -= arg;
                        }

                        bb = Cff.newBytes[addr];
                        if (bb == 29)
                        {
                            Cff.setOffset(Cff.newBytes, addr + 1, 4, (uint)newOffset);
                        }
                        else
                        {
                            if (bb == 28)
                            {
                                if (newOffset >= -32768 && newOffset <= 32767)
                                {
                                    Cff.setOffset(Cff.newBytes, addr + 1, 2, (uint)newOffset);
                                }
                                else
                                    throw new Exception("todo, too big value for short");
                            }
                            else
                            {
                                if (bb >= 247 && bb <= 250)
                                {
                                    if (newOffset >= 108 && newOffset <= 1131)
                                    {
                                        int tempB = newOffset - 108;
                                        byte b0 = (byte)((tempB >> 8) & 0xFF);
                                        byte b1 = (byte)((tempB - b0 * 256) & 0xFF);
                                        Cff.newBytes[addr] = (byte)(b0 + 247);
                                        Cff.newBytes[addr + 1] = b1;
                                    }
                                    else
                                        throw new Exception("todo, too big value for size2");
                                }
                                else if (bb >= 251 && bb <= 254)
                                {
                                    if (newOffset >= -1131 && newOffset <= -108)
                                    {
                                        int tempB = Math.Abs(newOffset) - 108;
                                        byte b0 = (byte)((tempB >> 8) & 0xFF);
                                        byte b1 = (byte)((tempB - b0 * 256) & 0xFF);
                                        Cff.newBytes[addr] = (byte)(b0 + 251);
                                        Cff.newBytes[addr + 1] = b1;
                                    }
                                    else
                                        throw new Exception("todo, too big value for size2");
                                }
                                else
                                    throw new Exception("todo, small address");
                            }
                        }
                    }

                    public Dict(Cff cff, string name, bool addToPool = true) : base(cff, name, addToPool)
                    {
                    }
                }
                #endregion

                #region DictIndex
                public class DictIndex : Index
                {
                    public List<Dict> Dicts;

                    public override void Read(int offset, int length)
                    {
                        base.Read(offset, length);

                        Dicts = new List<Dict>();

                        for (int index = 0; index < Offsets.Length - 1; index++)
                        {
                            Dict item = new Dict(Cff, Name + $"-Dict{index}", false);
                            item.Read((int)(Cff_Offset + Offsets[index]), (int)(Offsets[index + 1] - Offsets[index]));
                            Dicts.Add(item);
                        }

                        if (OffSize == 1 && Count > 1) OffSize = 2; //fast fix
                    }

                    public override int GetSize(bool onlyMain = false)
                    {
                        int size = 2 + 1 + (Count + 1) * OffSize;
                        foreach (var dictItem in Dicts)
                        {
                            size += dictItem.GetSize(onlyMain);
                        }
                        return size;
                    }

                    public override void Save(bool main = false)
                    {
                        if (main)
                        {
                            Cff_NewOffset = Cff.newBytesOffset;
                            int offset = Cff.newBytesOffset;
                            Cff.SetUInt16(Cff.newBytes, offset, (ushort)(Count));
                            Cff.newBytes[offset + 2] = OffSize;

                            int addOffs = 2 + 1 + (Count + 1) * OffSize - 1;
                            int offsetCur = 1;

                            Cff.setOffset(Cff.newBytes, offset + 3, OffSize, (uint)offsetCur);  //first offset
                            for (int i = 0; i < Count; i++)
                            {
                                Cff.newBytesOffset = offset + offsetCur + addOffs;
                                Dicts[i].Save(true);
                                offsetCur += Dicts[i].GetSize(true);
                                Cff.setOffset(Cff.newBytes, (int)(offset + 3 + (i + 1) * OffSize), OffSize, (uint)offsetCur);   //offset
                            }
                            var newSize = GetSize(true);
                            Cff.newBytesOffset = offset + newSize;
                            Cff_NewSize = newSize;
                        }
                        else
                        {
                            //int offset = Cff.newBytesOffset;
                            for (int i = 0; i < Count; i++)
                            {
                                Dicts[i].Save(false);
                            }
                        }
                    }

                    public DictIndex(Cff cff, string name, bool addToPool = true) : base(cff, name, addToPool)
                    {
                    }
                }
                #endregion

                #region Cff_Header
                public class Cff_Header : Block
                {
                    public byte Major;
                    public byte Minor;
                    public byte HdrSize;
                    public byte OffSize;

                    public override void Read(int offset, int length)
                    {
                        Cff_Offset = offset;
                        Cff_Size = length;

                        Major = Cff.getCard8();
                        Minor = Cff.getCard8();
                        HdrSize = Cff.getCard8();
                        OffSize = Cff.getOffSize();
                    }

                    public Cff_Header(Cff cff, string name, bool addToPool = true) : base(cff, name, addToPool)
                    {
                    }
                }
                #endregion

                #region Cff_CharStringsIndex
                public class Cff_CharStringsIndex : Index
                {
                    public override int GetSize(bool onlyMain = false)
                    {
                        if (Cff.GlyphsNeed == null) return Cff_Size;

                        int size = 2 + 1 + (Count + 1) * OffSize;
                        for (int i = 0; i < Count; i++)
                        {
                            if ((i < Cff.GlyphsNeed.Length) && Cff.GlyphsNeed[i])
                            {
                                size += (int)(Offsets[i + 1] - Offsets[i]);
                            }
                            else
                            {
                                size++;
                            }
                        }
                        return size;
                    }

                    public override void Save(bool main = false)
                    {
                        if (Cff.GlyphsNeed == null)
                        {
                            base.Save(main);
                            return;
                        }

                        Cff_NewOffset = Cff.newBytesOffset;
                        int offset = Cff.newBytesOffset;
                        Cff.SetUInt16(Cff.newBytes, offset, (ushort)(Count));   //count
                        Cff.newBytes[offset + 2] = OffSize;

                        int addOffs = 2 + 1 + (Count + 1) * OffSize - 1;
                        int offsetCur = 1;

                        Cff.setOffset(Cff.newBytes, offset + 3, OffSize, (uint)offsetCur);  //first offset
                        for (int i = 0; i < Count; i++)
                        {
                            Cff.newBytesOffset = offset + offsetCur + addOffs;
                            if ((i < Cff.GlyphsNeed.Length) && Cff.GlyphsNeed[i])
                            {
                                uint csLen = Offsets[i + 1] - Offsets[i];
                                Array.Copy(Cff.fontBytes, (int)(Cff_Offset + Offsets[i]), Cff.newBytes, offset + offsetCur + addOffs, csLen);
                                offsetCur += (int)csLen;
                            }
                            else
                            {
                                Cff.newBytes[offset + offsetCur + addOffs] = 14;    //endchar command
                                offsetCur++;
                            }
                            Cff.setOffset(Cff.newBytes, (int)(offset + 3 + (i + 1) * OffSize), OffSize, (uint)offsetCur);   //offset
                        }
                        var newSize = GetSize();
                        Cff.newBytesOffset = offset + newSize;
                        Cff_NewSize = newSize;
                    }

                    public Cff_CharStringsIndex(Cff cff, string name, bool addToPool = true) : base(cff, name, addToPool)
                    {
                    }
                }
                #endregion

                #region Utils
                private string getReal()
                {
                    StringBuilder sb = new StringBuilder();
                    while (true)
                    {
                        byte b = getCard8();
                        if (addRealPart((byte)((b >> 4) & 0x0f), sb)) break;
                        if (addRealPart((byte)(b & 0x0f), sb)) break;
                    }
                    return sb.ToString();
                }
                private bool addRealPart(byte val, StringBuilder sb)
                {
                    if (val <= 9) sb.Append((char)(48 + val));
                    if (val == 10) sb.Append(".");
                    if (val == 11) sb.Append("E");
                    if (val == 12) sb.Append("E-");
                    if (val == 14) sb.Append("-");
                    return (val == 15);
                }

                private byte getCard8()
                {
                    return fontBytes[fontBytesOffset++];
                }
                private ushort getCard16()
                {
                    return (ushort)((fontBytes[fontBytesOffset++] << 8) | fontBytes[fontBytesOffset++]);
                }
                private byte getOffSize()
                {
                    return getCard8();
                }
                private UInt32 getOffset(byte offSize)
                {
                    if (offSize == 1) return getCard8();
                    if (offSize == 2) return getCard16();
                    if (offSize == 3) return (UInt32)((fontBytes[fontBytesOffset++] << 16) | (fontBytes[fontBytesOffset++] << 8) | fontBytes[fontBytesOffset++]);
                    if (offSize == 4) return (UInt32)((fontBytes[fontBytesOffset++] << 24) | (fontBytes[fontBytesOffset++] << 16) | (fontBytes[fontBytesOffset++] << 8) | fontBytes[fontBytesOffset++]);
                    throw new ArgumentException("Unknown OffSize value");
                }
                private void setOffset(byte[] buf, int offset, byte offSize, uint value)
                {
                    if (offSize == 1)
                    {
                        buf[offset] = (byte)value;
                    }
                    if (offSize == 2)
                    {
                        buf[offset] = (byte)((value >> 8) & 0xFF);
                        buf[offset + 1] = (byte)(value & 0xFF);
                    }
                    if (offSize == 3)
                    {
                        buf[offset] = (byte)((value >> 16) & 0xFF);
                        buf[offset + 1] = (byte)((value >> 8) & 0xFF);
                        buf[offset + 2] = (byte)(value & 0xFF);
                    }
                    if (offSize == 4)
                    {
                        buf[offset] = (byte)((value >> 24) & 0xFF);
                        buf[offset + 1] = (byte)((value >> 16) & 0xFF);
                        buf[offset + 2] = (byte)((value >> 8) & 0xFF);
                        buf[offset + 3] = (byte)(value & 0xFF);
                    }
                }

                private byte GetUInt8(byte[] buff, int pos)
                {
                    return buff[pos];
                }

                private UInt16 GetUInt16(byte[] buff, int pos)
                {
                    bufGetData[0] = buff[pos + 1];
                    bufGetData[1] = buff[pos + 0];
                    return BitConverter.ToUInt16(bufGetData, 0);
                }

                private UInt32 GetUInt32(byte[] buff, int pos)
                {
                    bufGetData[0] = buff[pos + 3];
                    bufGetData[1] = buff[pos + 2];
                    bufGetData[2] = buff[pos + 1];
                    bufGetData[3] = buff[pos + 0];
                    return BitConverter.ToUInt32(bufGetData, 0);
                }

                //private sbyte GetInt8(byte[] buff, int pos)
                //{
                //    return (sbyte)buff[pos];
                //}
                private Int16 GetInt16(byte[] buff, int pos)
                {
                    return (Int16)GetUInt16(buff, pos);
                }

                //private Int32 GetInt32(byte[] buff, int pos)
                //{
                //    return (Int32)GetUInt32(buff, pos);
                //}

                private void SetUInt16(byte[] buff, int pos, UInt16 value)
                {
                    BitConverter.GetBytes(value).CopyTo(bufGetData, 0);
                    buff[pos + 0] = bufGetData[1];
                    buff[pos + 1] = bufGetData[0];
                }

                //private void SetUInt32(byte[] buff, int pos, UInt32 value)
                //{
                //    BitConverter.GetBytes(value).CopyTo(bufGetData, 0);
                //    buff[pos + 0] = bufGetData[3];
                //    buff[pos + 1] = bufGetData[2];
                //    buff[pos + 2] = bufGetData[1];
                //    buff[pos + 3] = bufGetData[0];
                //}
                #endregion

                #region Variables
                private byte[] fontBytes;
                private byte[] newBytes;
                private byte[] bufGetData = new byte[4];
                private Int32 fontBytesOffset;
                private Int32 newBytesOffset;
                private int baseLength;

                public Int32 Offset = 0;

                public Cff_Header Header;
                public Index NameIndex;
                public DictIndex TopDictIndex;
                public Index StringIndex;
                public Index GlobalSubrIndex;
                #endregion

                #region Properties
                public bool[] GlyphsNeed;
                public List<Block> TablesList;
                #endregion

                #region Methods
                public void Read(byte[] fontBytes, int offset, int length)
                {
                    TablesList = new List<Block>();

                    this.fontBytes = fontBytes;
                    this.Offset = offset;
                    this.baseLength = length;
                    fontBytesOffset = offset;

                    Header = new Cff_Header(this, "Header");
                    NameIndex = new Index(this, "NameIndex");
                    TopDictIndex = new DictIndex(this, "TopDictIndex");
                    StringIndex = new Index(this, "StringIndex");
                    GlobalSubrIndex = new Index(this, "GlobalSubrIndex");

                    Header.Read(offset, 4);
                    NameIndex.Read(Header.Cff_Offset + Header.Cff_Size, 0);
                    TopDictIndex.Read(NameIndex.Cff_Offset + NameIndex.Cff_Size, 0);
                    StringIndex.Read(TopDictIndex.Cff_Offset + TopDictIndex.Cff_Size, 0);
                    GlobalSubrIndex.Read(StringIndex.Cff_Offset + StringIndex.Cff_Size, 0);
                }

                public int GetSize()
                {
                    return Header.GetSize() + NameIndex.GetSize() + TopDictIndex.GetSize() + StringIndex.GetSize() + GlobalSubrIndex.GetSize();
                }

                public byte[] Save()
                {
                    newBytes = new byte[GetSize()];
                    newBytesOffset = 0;

                    Header.Save();
                    NameIndex.Save();
                    TopDictIndex.Save(true);
                    StringIndex.Save();
                    GlobalSubrIndex.Save();
                    TopDictIndex.Save(false);

                    return newBytes;
                }

                public void CutUnusedSubr()
                {
                    try
                    {
                        if (GlyphsNeed != null && TopDictIndex.Dicts[0].FDSelect != null)
                        {
                            byte[] gids = new byte[TopDictIndex.Dicts[0].CharStringsIndex.Count];

                            #region Unpack gids
                            int start = TopDictIndex.Dicts[0].FDSelect.Cff_Offset;
                            fontBytesOffset = start;
                            byte format = getCard8();
                            if (format == 0)
                            {
                                Array.Copy(fontBytes, start + 1, gids, 0, gids.Length);
                            }
                            else if (format == 3)
                            {
                                int nRanges = getCard16();
                                byte fd = 0;
                                int prev = -1;
                                for (int i = 0; i < nRanges; i++)
                                {
                                    int first = getCard16();
                                    if (prev >= 0)
                                    {
                                        for (int glyphId = prev; glyphId < first; glyphId++)
                                        {
                                            gids[glyphId] = fd;
                                        }
                                    }
                                    prev = first;
                                    fd = getCard8();
                                }
                                if (prev >= 0)
                                {
                                    int first = getCard16();
                                    for (int glyphId = prev; glyphId < first; glyphId++)
                                    {
                                        gids[glyphId] = fd;
                                    }
                                }
                            }
                            else return; //other unsupported
                            #endregion

                            bool[] fds = new bool[TopDictIndex.Dicts[0].FontDictIndex.Count];
                            for (int index = 0; index < GlyphsNeed.Length; index++)
                            {
                                if (GlyphsNeed[index])
                                {
                                    fds[gids[index]] = true;
                                }
                            }

                            for (int index = 0; index < TopDictIndex.Dicts[0].FontDictIndex.Count; index++)
                            {
                                if (!fds[index])
                                {
                                    TopDictIndex.Dicts[0].FontDictIndex.Dicts[index].Private.SubrIndex = null;
                                    TopDictIndex.Dicts[0].FontDictIndex.Dicts[index].Private.Items.Remove(19);
                                }
                            }
                        }
                    }
                    catch { }
                }

                #region GetTablesString
                public string GetBaseTablesString()
                {
                    var list = TablesList.OrderBy(x => x.Cff_Offset).ToList();
                    StringBuilder sb = new StringBuilder();

                    int pos = Offset;
                    for (int index = 0; index < list.Count; index++)
                    {
                        var item = list[index];
                        if (item.Cff_Offset < pos)
                        {
                            sb.Append($"  Наложение {pos - item.Cff_Offset} ! \r\n");
                        }
                        if (item.Cff_Offset > pos)
                        {
                            sb.Append($"  Зазор {item.Cff_Offset - pos} ! \r\n");
                        }
                        sb.Append($"{Offset + item.Cff_Offset:X8} {item.Cff_Size:X8} {item.Name}  \r\n");
                        pos = item.Cff_Offset + item.Cff_Size;
                    }
                    if (Offset + baseLength > pos)
                    {
                        sb.Append($"  Зазор {Offset + baseLength - pos} ! \r\n");
                    }
                    sb.Append($"{baseLength:X8} \r\n");

                    return sb.ToString();
                }

                public string GetNewTablesString()
                {
                    var list = TablesList.OrderBy(x => x.Cff_NewOffset).ToList();
                    StringBuilder sb = new StringBuilder();

                    int pos = Offset;
                    for (int index = 0; index < list.Count; index++)
                    {
                        var item = list[index];
                        if (item.Cff_NewOffset < pos)
                        {
                            sb.Append($"  Наложение {pos - item.Cff_NewOffset} ! \r\n");
                        }
                        if (item.Cff_NewOffset > pos)
                        {
                            sb.Append($"  Зазор {item.Cff_NewOffset - pos} ! \r\n");
                        }
                        sb.Append($"{Offset + item.Cff_NewOffset:X8} {item.Cff_NewSize:X8} {item.Name}  \r\n");
                        pos = item.Cff_NewOffset + item.Cff_NewSize;
                    }
                    int size = GetSize();
                    if (Offset + size > pos)
                    {
                        sb.Append($"  Зазор {Offset + size - pos} ! \r\n");
                    }
                    sb.Append($"{size:X8} \r\n");

                    return sb.ToString();
                }
                #endregion

                #endregion
            }
            #endregion
        }
    }
}
