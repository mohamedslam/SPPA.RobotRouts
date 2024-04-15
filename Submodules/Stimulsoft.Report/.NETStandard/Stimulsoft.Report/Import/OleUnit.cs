#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	Stimulsoft.Report Library										}
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
using System.Text;

namespace Stimulsoft.Report.Import
{
    /// <summary>
    /// Summary description for OleUnit.
    /// </summary>
    public class OleUnit
	{
        #region class OleContainer
        public class OleContainer
        {
            #region struct DirEntry
            public struct DirEntry
            {
                public string Name;
                public byte Type;
                public int DIDLeft;
                public int DIDRight;
                public int DIDRoot;
                public int SIDFirstSector;
                public int Size;
            }
            #endregion

            #region Fields
            public byte[] Data = null;
            public DirEntry[] Dir = null;

            private uint BigSectorSize = 512;
            private uint ShortSectorSize = 64;
            private uint MinimumStreamSize = 4096;
            private uint[] BSAT = null;
            private uint[] SSAT = null;

            private const int HeaderSize = 512;
            private const uint SIDFree = 0xffffffff;
            private const uint SIDEoC = 0xfffffffe;
            private const uint SIDSAT = 0xfffffffd;
            private const uint SIDMSAT = 0xfffffffc;
            #endregion

            #region Methods
            private UInt16 GetUInt16(uint pos)
            {
                return GetUInt16((uint)pos);
            }

            private UInt16 GetUInt16(int pos)
            {
                return BitConverter.ToUInt16(Data, pos);
            }

            private UInt32 GetUInt32(uint pos)
            {
                return GetUInt32((uint)pos);
            }

            private UInt32 GetUInt32(int pos)
            {
                return BitConverter.ToUInt32(Data, pos);
            }

            private int GetBigSectorOffset(uint number)
            {
                return (int)(HeaderSize + number * BigSectorSize);
            }

            private int GetShortSectorOffset(uint number)
            {
                var shortStreamChain = GetBSatChain((uint)Dir[0].SIDFirstSector);
                var shortSectorsInBigSector = BigSectorSize / ShortSectorSize;
                var bigSector = number / shortSectorsInBigSector;
                var bigSectorEntry = number - bigSector * shortSectorsInBigSector;

                return (int)(GetBigSectorOffset(shortStreamChain[bigSector]) + bigSectorEntry * ShortSectorSize);
            }

            private uint[] GetBSatChain(uint startSector)
            {
                var chain = new ArrayList();
                var currentSector = startSector;
                do
                {
                    chain.Add(currentSector);
                    currentSector = BSAT[currentSector];
                }
                while (currentSector != SIDEoC);
                
                var chainSec = new uint[chain.Count];
                chain.CopyTo(chainSec);
                return chainSec;
            }

            private uint[] GetSSatChain(uint startSector)
            {
                var chain = new ArrayList();
                var currentSector = startSector;
                do
                {
                    chain.Add(currentSector);
                    currentSector = SSAT[currentSector];
                }
                while (currentSector != SIDEoC);
                var chainSec = new uint[chain.Count];
                chain.CopyTo(chainSec);
                return chainSec;
            }

            private byte[] GetBigStreamData(uint[] chain)
            {
                var streamData = new byte[chain.Length * BigSectorSize];
                for (var index = 0; index < chain.Length; index++)
                {
                    var source = GetBigSectorOffset(chain[index]);
                    var destination = (int)(index * BigSectorSize);
                    Array.Copy(Data, source, streamData, (int)destination, (int)BigSectorSize);
                }
                return streamData;
            }

            private byte[] GetShortStreamData(uint[] chain)
            {
                var streamData = new byte[chain.Length * ShortSectorSize];
                for (var index = 0; index < chain.Length; index++)
                {
                    var source = GetShortSectorOffset(chain[index]);
                    var destination = (int)(index * ShortSectorSize);
                    Array.Copy(Data, source, streamData, (int)destination, (int)ShortSectorSize);
                }
                return streamData;
            }

            public byte[] GetStreamData(int streamNumber)
            {
                if (Dir[streamNumber].Type == 0 || Dir[streamNumber].Size == 0 || streamNumber == 0)
                    return null;

                var streamSize = Dir[streamNumber].Size;

                var data = streamSize < MinimumStreamSize
                    ? GetShortStreamData(GetSSatChain((uint)Dir[streamNumber].SIDFirstSector))
                    : GetBigStreamData(GetBSatChain((uint)Dir[streamNumber].SIDFirstSector));

                if (data.Length > streamSize)
                {
                    var outData = new byte[streamSize];
                    Array.Copy(data, 0, outData, 0, streamSize);
                    return outData;
                }

                return data;
            }

            public void Clear()
            {
                Data = null;
                Dir = null;
                BSAT = null;
                SSAT = null;
            }
            #endregion

            public OleContainer(byte[] inData)
            {
                Data = inData;
                if (Data == null) return;

                //header
                BigSectorSize = (uint)(1 << GetUInt16(30));
                ShortSectorSize = (uint)(1 << GetUInt16(32));
                MinimumStreamSize = GetUInt32(56);
                var satLengthInSectors = GetUInt32(44);
                var msatFirstSector = GetUInt32(68);
                var ssatFirstSector = GetUInt32(60);
                var dirFirstSector = GetUInt32(48);

                //get MSAT
                uint[] MSAT = null;
                MSAT = new uint[satLengthInSectors];
                uint msatRecCount = 109;    //value count in header
                var msatSecOffset = 76; //offset of the first value in header
                var msatIndex = satLengthInSectors;
                while (msatIndex > 0)
                {
                    MSAT[satLengthInSectors - msatIndex] = GetUInt32(msatSecOffset);
                    msatSecOffset += 4;
                    msatRecCount--;
                    msatIndex--;

                    if (msatRecCount == 0)
                    {
                        msatSecOffset = GetBigSectorOffset(msatSecOffset == HeaderSize
                            ? msatFirstSector
                            : GetUInt32(msatSecOffset));

                        msatRecCount = (BigSectorSize >> 2) - 1;
                    }
                }

                //get BSAT
                var bsatLength = (uint)(satLengthInSectors * (BigSectorSize >> 2));
                BSAT = new uint[bsatLength];
                for (var index = 0; index < satLengthInSectors; index++)
                {
                    var satSectorOffset = GetBigSectorOffset(MSAT[index]);
                    for (var index2 = 0; index2 < (BigSectorSize >> 2); index2++)
                    {
                        BSAT[index * (BigSectorSize >> 2) + index2] = GetUInt32(satSectorOffset + index2 * 4);
                    }
                }

                //get SSAT
                if (ssatFirstSector != SIDEoC)
                {
                    var SSATChain = GetBSatChain(ssatFirstSector);
                    var ssatLength = (uint)(SSATChain.Length * (BigSectorSize >> 2));
                    SSAT = new uint[ssatLength];
                    for (var index = 0; index < SSATChain.Length; index++)
                    {
                        var ssatSectorOffset = GetBigSectorOffset(SSATChain[index]);
                        for (var index2 = 0; index2 < (BigSectorSize >> 2); index2++)
                        {
                            SSAT[index * (BigSectorSize >> 2) + index2] = GetUInt32(ssatSectorOffset + index2 * 4);
                        }
                    }
                }

                //get directory
                var dirChain = GetBSatChain(dirFirstSector);
                var entryCountInSector = (int)(BigSectorSize >> 7);
                var dirLength = (uint)(dirChain.Length * entryCountInSector);
                Dir = new DirEntry[dirLength];
                for (var index = 0; index < dirLength; index++)
                {
                    var sector = (int)(index / entryCountInSector);
                    var entryInSector = (index - sector * entryCountInSector);
                    var dataOffset = (GetBigSectorOffset(dirChain[sector]) + entryInSector * 0x80);
                    Dir[index] = new DirEntry();

                    var sb = new StringBuilder();
                    for (var index3 = 0; index3 < 31; index3++)
                    {
                        var ch = BitConverter.ToChar(Data, dataOffset + index3 * 2);
                        if (ch == 0x0000) break;
                        sb.Append(ch);
                    }
                    Dir[index].Name = sb.ToString();
                    Dir[index].Type = Data[dataOffset + 66];
                    Dir[index].DIDLeft = (int)GetUInt32(dataOffset + 68);
                    Dir[index].DIDRight = (int)GetUInt32(dataOffset + 72);
                    Dir[index].DIDRoot = (int)GetUInt32(dataOffset + 76);
                    Dir[index].SIDFirstSector = (int)GetUInt32(dataOffset + 116);
                    Dir[index].Size = (int)GetUInt32(dataOffset + 120);
                }
            }
        }
        #endregion

        #region class ObjectHeader
        public class ObjectHeader
        {
            #region object structure info

            //http://support.microsoft.com/default.aspx?scid=kb;EN-US;147727

            //object header
            //			short Signature;		// Type signature (0x1c15).
            //			short HeaderSize;		// Size of header (sizeof(struct OBJECTHEADER) + cchName + cchClass).
            //			int ObjectType;			// OLE Object type code (OT_STATIC, OT_LINKED, OT_EMBEDDED).
            //			short NameLen;			// Count of characters in object name (CchSz(szName) + 1).
            //			short ClassLen;			// Count of characters in class name (CchSz(szClass) + 1).
            //			short NameOffset;		// Offset of object name in structure.
            //			short ClassOffset;		// Offset of class name in structure.
            //			short ObjectSizeWidth;	// Original size of object
            //			short ObjectSizeHeight;	// Original size of object
            //			string Name;
            //			string Class;

            //ole header
            //			int OleVersion;
            //			int Format;
            //			int OleInfoLen;
            //			string OleInfo;
            //			int Unknown1;
            //			int Unknown2;
            //			int Unknown3;
            #endregion

            #region Methods
            private string GetString(byte[] data, int offset)
            {
                var sb = new StringBuilder();
                while (data[offset] != 0)
                {
                    sb.Append((char)data[offset]);
                    offset++;
                }
                return sb.ToString();
            }
            #endregion

            #region Fields
            private short Signature;        // Type signature (0x1c15).
            private short HeaderSize;       // Size of header (sizeof(struct OBJECTHEADER) + cchName + cchClass).
            private int ObjectType;         // OLE Object type code (OT_STATIC, OT_LINKED, OT_EMBEDDED).
            private short NameLen;          // Count of characters in object name (CchSz(szName) + 1).
            private short ClassLen;         // Count of characters in class name (CchSz(szClass) + 1).
            private short NameOffset;       // Offset of object name in structure.
            private short ClassOffset;      // Offset of class name in structure.
            private short ObjectSizeWidth;  // Original size of object
            private short ObjectSizeHeight; // Original size of object
                                            //ole header
            private int OleVersion;
            private int Format;
            private int OleInfoLen;

            public string Name;
            public string Class;
            public string OleInfo;
            public int HeaderLen;
            #endregion

            public ObjectHeader(byte[] data)
            {
                HeaderLen = 0;
                if (data == null || data.Length <= 64 || BitConverter.ToUInt16(data, 0) != OleLinkIdentifier) return;

                Signature = BitConverter.ToInt16(data, 0);
                HeaderSize = BitConverter.ToInt16(data, 2);
                ObjectType = BitConverter.ToInt32(data, 4);
                NameLen = BitConverter.ToInt16(data, 8);
                ClassLen = BitConverter.ToInt16(data, 10);
                NameOffset = BitConverter.ToInt16(data, 12);
                ClassOffset = BitConverter.ToInt16(data, 14);
                ObjectSizeWidth = BitConverter.ToInt16(data, 16);
                ObjectSizeHeight = BitConverter.ToInt16(data, 18);

                OleVersion = BitConverter.ToInt32(data, HeaderSize);
                Format = BitConverter.ToInt32(data, HeaderSize + 4);
                OleInfoLen = BitConverter.ToInt32(data, HeaderSize + 8);

                Name = GetString(data, NameOffset);
                Class = GetString(data, ClassOffset);
                OleInfo = GetString(data, HeaderSize + 12);

                HeaderLen = HeaderSize + 12 + OleInfoLen + 12;
            }
        }
        #endregion

        #region Consts
        private const UInt64 OleContainerIdentifier = 0xE11AB1A1E011CFD0; //Compound document file identifier
	    private const UInt16 OleLinkIdentifier = 0x1C15; //Ole-object signature
	    public const int OlePresHeaderLength = 40;
        #endregion

		#region Methods
		public static bool IsOleContainer(byte[] data)
		{
		    return data != null && data.Length > 64 && BitConverter.ToUInt64(data, 0) == OleContainerIdentifier;
		}

		public static bool IsOleHeader(byte[] data)
		{
		    return data != null && data.Length > 64 && BitConverter.ToUInt16(data, 0) == OleLinkIdentifier;
		}

		public static bool CheckForOlePres(byte[] data)
		{
		    return data != null && data.Length > 64 &&
		           BitConverter.ToUInt32(data, 0) == 0xFFFFFFFF && 
		           BitConverter.ToUInt32(data, 4) == 0x00000003 && 
		           BitConverter.ToUInt32(data, 8) == 0x00000004;
		}
		#endregion
	}
}
