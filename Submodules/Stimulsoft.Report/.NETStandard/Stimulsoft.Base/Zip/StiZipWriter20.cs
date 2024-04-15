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
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Globalization;

namespace Stimulsoft.Base.Zip
{
    public class StiZipWriter20
    {
        #region Constants
        private static class Constants
        {
            public const int Version = 20;

            public const int LocalHeaderBaseSize = 30;      //excluding variable length fields at end
            public const int CentralHeaderBaseSize = 46;    //excluding variable length fields at end
            public const int DataDescriptorSize = 16;

            public const int CentralHeaderSignature         = 0x02014B50;
            public const int LocalHeaderSignature           = 0x04034B50;
            public const int EndOfCentralDirectorySignature = 0x06054B50;
            public const int DataDescriptorSignature        = 0x08074B50;

            public static Encoding DefaultEncoding = null;

            static Constants()
            {
                try
                {
                    DefaultEncoding = Encoding.GetEncoding(CultureInfo.InstalledUICulture.TextInfo.OEMCodePage);
                }
                catch
                {
                    try
                    {
                        DefaultEncoding = Encoding.GetEncoding(CultureInfo.InstalledUICulture.TextInfo.ANSICodePage);
                    }
                    catch
                    {
                        DefaultEncoding = Encoding.GetEncoding(Encoding.ASCII.CodePage);
                    }
                }
            }
        }
        #endregion

        #region Static methods
        private static byte[] ConvertToArray(bool useUnicode, string str)
        {
            if (str == null)
            {
                return new byte[0];
            }
            if (useUnicode)
            {
                return Encoding.UTF8.GetBytes(str);
            }
            else
            {
                return Constants.DefaultEncoding.GetBytes(str);
            }
        }

        private static uint GetDosTime(DateTime dt)
        {
            return ((uint)dt.Year - 1980 & 0x7f) << 25 |
                    ((uint)dt.Month) << 21 |
                    ((uint)dt.Day) << 16 |
                    ((uint)dt.Hour) << 11 |
                    ((uint)dt.Minute) << 5 |
                    ((uint)dt.Second) >> 1;
        }
        #endregion

        #region Enumerations
        private enum CompressionMethod
        {
            /// <summary>
            /// Direct copy of the file contents
            /// </summary>
            Stored = 0,

            /// <summary>
            /// Compression method LZ/Huffman
            /// </summary>
            Deflated = 8
        }

        /// <summary>
        /// Defines the contents of the general bit flags field for an archive entry.
        /// </summary>
        [Flags]
        private enum GeneralBitFlags
        {
            None = 0,
            /// <summary>
            /// Bit 3 if set indicates a trailing data desciptor is appended to the entry data
            /// </summary>
            Descriptor = 0x0008,
            /// <summary>
            /// Bit 11 if set indicates the filename and comment fields for this file must be encoded using UTF-8.
            /// </summary>
            UnicodeText = 0x0800,
        }
        #endregion

        #region Class ZipException
        public class ZipException : ApplicationException
        {
            public ZipException(string message)
                : base(message)
            {
            }
        }
        #endregion
        
        #region Class ZipEntry
        private class ZipEntry
        {
            #region Variables
            private string name;
            private ulong size;
            private ulong compressedSize;
            private uint crc;
            private DateTime dateTime;
            private CompressionMethod method = CompressionMethod.Deflated;
            private int flags;          //general purpose bit flags
            private long offset;        //offset in ZipOutputStream
            private bool isUnknownSize = true;
            #endregion

            #region Properties
            public bool IsUnknownSize
            {
                get
                {
                    return isUnknownSize;
                }
            }
 
            public int Flags
            {
                get
                {
                    return flags;
                }
                set
                {
                    flags = value;
                }
            }

            /// <summary>
            /// Get/set a flag indicating wether entry name are encoded in Unicode UTF8
            /// </summary>
            public bool UseUnicodeText
            {
                get
                {
                    return (flags & (int)GeneralBitFlags.UnicodeText) != 0;
                }
                set
                {
                    if (value)
                    {
                        flags |= (int)GeneralBitFlags.UnicodeText;
                    }
                    else
                    {
                        flags &= ~(int)GeneralBitFlags.UnicodeText;
                    }
                }
            }

            /// <summary>
            /// Get/set offset for use in central header
            /// </summary>
            public long Offset
            {
                get
                {
                    return offset;
                }
                set
                {
                    offset = value;
                }
            }

            /// <summary>
            /// Gets/Sets the time of last modification of the entry.
            /// </summary>
            public DateTime DateTime
            {
                get
                {
                    return dateTime;
                }
                set
                {
                    dateTime = value;
                }
            }

            /// <summary>
            /// Returns the entry name
            /// </summary>
            public string Name
            {
                get
                {
                    return name;
                }
            }

            /// <summary>
            /// Gets/Sets the size of the uncompressed data.
            /// </summary>
            public long Size
            {
                get
                {
                    return (long)size;
                }
                set
                {
                    this.size = (ulong)value;
                    isUnknownSize = false;
                }
            }

            /// <summary>
            /// Gets/Sets the size of the compressed data.
            /// </summary>
            public long CompressedSize
            {
                get
                {
                    return (long)compressedSize;
                }
                set
                {
                    this.compressedSize = (ulong)value;
                }
            }

            /// <summary>
            /// Gets/Sets the crc of the uncompressed data.
            /// </summary>
            public long Crc
            {
                get
                {
                    return (long)crc;
                }
                set
                {
                    this.crc = (uint)value;
                }
            }

            public CompressionMethod CompressionMethod
            {
                get
                {
                    return method;
                }

                set
                {
                    this.method = value;
                }
            }

            /// <summary>
            /// Gets a value indicating if the entry is a directory.
            /// </summary>
            public bool IsDirectory
            {
                get
                {
                    int nameLength = name.Length;
                    return ((nameLength > 0) && ((name[nameLength - 1] == '/') || (name[nameLength - 1] == '\\')));
                }
            }
            #endregion

            #region Constructors
            public ZipEntry(string name) : this(name, CompressionMethod.Deflated)
            {
            }

            public ZipEntry(string name, CompressionMethod method)
            {
                if (name == null)
                {
                    throw new ZipException("ZipEntry name is null");
                }
                if (name.Length > 0xFFFF)
                {
                    throw new ZipException("ZipEntry name is too long");
                }

                this.name = name;
                this.DateTime = DateTime.Now;
                this.method = method;
                this.flags = (int)GeneralBitFlags.None;
                this.isUnknownSize = true;
            }
            #endregion
        }
        #endregion

        #region Class DeflaterOutputStream
        private class DeflaterOutputStream : Stream
        {
            #region Variables
            protected int TotalOut = 0;
            protected Stream baseOutputStream;
            private DeflateStream def = null;
            private bool isClosed = false;
            private bool isFinished = false;
            private bool isStreamOwner = false;
            #endregion

            #region Properties
            public bool IsStreamOwner
            {
                get
                {
                    return isStreamOwner;
                }
                set
                {
                    isStreamOwner = value;
                }
            }

            ///	<summary>
            /// Allows client to determine if an entry can be patched after its added
            /// </summary>
            public bool CanPatchEntries
            {
                get
                {
                    return baseOutputStream.CanSeek;
                }
            }
            #endregion

            #region Stream Overrides
            public override bool CanRead
            {
                get
                {
                    return false;
                }
            }

            public override bool CanSeek
            {
                get
                {
                    return false;
                }
            }

            public override bool CanWrite
            {
                get
                {
                    return baseOutputStream.CanWrite;
                }
            }

            public override long Length
            {
                get
                {
                    return baseOutputStream.Length;
                }
            }

            public override long Position
            {
                get
                {
                    return baseOutputStream.Position;
                }
                set
                {
                    throw new NotSupportedException("Position property not supported");
                }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException("DeflaterOutputStream Seek not supported");
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException("DeflaterOutputStream SetLength not supported");
            }

            public override int ReadByte()
            {
                throw new NotSupportedException("DeflaterOutputStream ReadByte not supported");
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException("DeflaterOutputStream Read not supported");
            }

            public override void WriteByte(byte value)
            {
                byte[] b = new byte[1];
                b[0] = value;
                Write(b, 0, 1);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                if (isFinished)
                {
                    throw new ZipException("DeflateStream is finished");
                }
                long positionOld = baseOutputStream.Position;
                def.Write(buffer, offset, count);
                TotalOut += (int)(baseOutputStream.Position - positionOld);
            }

            public void Reset()
            {
                isFinished = false;
                def = new DeflateStream(baseOutputStream, CompressionMode.Compress, true);
                TotalOut = 0;
            }

            public virtual void Finish()
            {
                if (!isFinished)
                {
                    isFinished = true;
                    long positionOld = baseOutputStream.Position;
                    def.Close();
                    def = null;
                    TotalOut += (int)(baseOutputStream.Position - positionOld);
                    baseOutputStream.Flush();
                }
            }

            public override void Flush()
            {
                def.Flush();
                baseOutputStream.Flush();
            }

            public override void Close()
            {
                if (!isClosed)
                {
                    isClosed = true;
                    Finish();
                    if (isStreamOwner)
                    {
                        baseOutputStream.Close();
                    }
                    baseOutputStream = null;
                }
            }
            #endregion

            #region Constructors
            public DeflaterOutputStream(Stream baseOutputStream)
            {
                if (baseOutputStream == null)
                {
                    throw new ZipException("OutputStream cannot be null");
                }
                if (baseOutputStream.CanWrite == false)
                {
                    throw new ZipException("OutputStream must support writing");
                }

                this.baseOutputStream = baseOutputStream;
            }
            #endregion
        }
        #endregion

        #region Class ZipOutputStream
        private class ZipOutputStream : DeflaterOutputStream
        {
            #region Variables
            private ArrayList entriesList = new ArrayList();
            private Crypto.Crc32 crc = new Crypto.Crc32();
            private ZipEntry currentEntry;
            private long size;  //Used to track the size of data for an entry during writing.
            private long offset;
            bool patchEntryHeader;
            long crcPatchPos = 0;
            private const int extraLength = 0;
            private const int commentLength = 0;
            #endregion

            #region Utils.WriteData
            private void WriteDataShort(int value)
            {
                baseOutputStream.WriteByte((byte)(value & 0xff));
                baseOutputStream.WriteByte((byte)((value >> 8) & 0xff));
            }
            private void WriteDataInt(int value)
            {
                WriteDataShort(value);
                WriteDataShort(value >> 16);
            }
            private void WriteDataLong(long value)
            {
                WriteDataInt((int)value);
                WriteDataInt((int)(value >> 32));
            }
            #endregion

            #region Methods.PutNextEntry
            /// <summary>
            /// Put next entry into Zip file
            /// </summary>
            /// <param name="entry">Entry to put into zip file</param>
            public void PutNextEntry(ZipEntry entry)
            {
                if (entry == null)
                {
                    throw new ZipException("ZipEntry is null");
                }
                if (entriesList == null)
                {
                    throw new ZipException("ZipOutputStream was finished");
                }
                if (entriesList.Count == int.MaxValue)
                {
                    throw new ZipException("Too many entries for Zip file");
                }

                if (currentEntry != null)
                {
                    CloseEntry();
                }

                if (entry.CompressionMethod == CompressionMethod.Stored)
                {
                    if (CanPatchEntries != true)
                    {
                        // Can't patch entries so storing is not possible.
                        entry.CompressionMethod = CompressionMethod.Deflated;
                    }
                }

                patchEntryHeader = true;
                if (CanPatchEntries == false)
                {
                    // Only way to record size and compressed size is to append a data descriptor after compressed data.
                    entry.Flags |= (int)GeneralBitFlags.Descriptor;
                    patchEntryHeader = false;
                }
                entry.Offset = offset;

                #region Write the local file header
                WriteDataInt(Constants.LocalHeaderSignature);
                WriteDataShort(Constants.Version);
                WriteDataShort(entry.Flags);
                WriteDataShort((byte)entry.CompressionMethod);
                WriteDataInt((int)GetDosTime(entry.DateTime));

                if (patchEntryHeader)
                {
                    crcPatchPos = baseOutputStream.Position;
                }
                WriteDataInt(0);	// Crc
                WriteDataInt(0);	// Compressed size
                WriteDataInt(0);	// Uncompressed size

                byte[] name = ConvertToArray(entry.UseUnicodeText, entry.Name);
                if (name.Length > 0xFFFF)
                {
                    throw new ZipException("Entry name too long");
                }
                WriteDataShort(name.Length);
                WriteDataShort(extraLength);
                if (name.Length > 0)
                {
                    baseOutputStream.Write(name, 0, name.Length);
                }
                //no extra

                offset += Constants.LocalHeaderBaseSize + name.Length + extraLength;
                #endregion

                currentEntry = entry;
                size = 0;
                crc.Reset();
                if (entry.CompressionMethod == CompressionMethod.Deflated)
                {
                    Reset();
                }
            }
            #endregion

            #region Methods.Write
            /// <summary>
            /// Write data to the current entry
            /// </summary>
            public override void Write(byte[] buffer, int offset, int count)
            {
                if (currentEntry == null)
                {
                    throw new ZipException("No open entry");
                }
                if (buffer == null)
                {
                    throw new ZipException("Buffer is null");
                }
                if (offset < 0)
                {
                    throw new ZipException("Offset cannot be negative");
                }
                if (count < 0)
                {
                    throw new ZipException("Count cannot be negative");
                }
                if ((buffer.Length - offset) < count)
                {
                    throw new ZipException("Invalid offset/count combination");
                }

                crc.Update(buffer, offset, count);
                size += count;

                switch (currentEntry.CompressionMethod)
                {
                    case CompressionMethod.Deflated:
                        base.Write(buffer, offset, count);
                        break;

                    case CompressionMethod.Stored:
                        baseOutputStream.Write(buffer, offset, count);
                        break;
                }
            }
            #endregion

            #region Methods.CloseEntry
            /// <summary>
            /// Close current entry
            /// </summary>
            public void CloseEntry()
            {
                if (currentEntry == null)
                {
                    throw new ZipException("No open entry");
                }

                if (currentEntry.CompressionMethod == CompressionMethod.Deflated)
                {
                    base.Finish();
                }

                long csize = (currentEntry.CompressionMethod == CompressionMethod.Deflated ? TotalOut : size);

                currentEntry.Size = size;
                currentEntry.CompressedSize = csize;
                currentEntry.Crc = crc.Value;

                offset += csize;

                // Patch the header if possible
                if (patchEntryHeader == true)
                {
                    patchEntryHeader = false;

                    long curPos = baseOutputStream.Position;
                    baseOutputStream.Seek(crcPatchPos, SeekOrigin.Begin);
                    WriteDataInt((int)currentEntry.Crc);
                    WriteDataInt((int)currentEntry.CompressedSize);
                    WriteDataInt((int)currentEntry.Size);
                    baseOutputStream.Seek(curPos, SeekOrigin.Begin);
                }

                // Add data descriptor if flagged as required
                if ((currentEntry.Flags & (int)GeneralBitFlags.Descriptor) != 0)
                {
                    WriteDataInt(Constants.DataDescriptorSignature);
                    WriteDataInt((int)currentEntry.Crc);
                    WriteDataInt((int)currentEntry.CompressedSize);
                    WriteDataInt((int)currentEntry.Size);
                    offset += Constants.DataDescriptorSize;
                }

                entriesList.Add(currentEntry);
                currentEntry = null;
            }
            #endregion

            #region Methods.Finish
            public override void Finish()
            {
                if (entriesList == null)
                {
                    return;
                }
                if (currentEntry != null)
                {
                    CloseEntry();
                }

                long numEntries = entriesList.Count;
                long sizeEntries = 0;

                foreach (ZipEntry entry in entriesList)
                {
                    WriteDataInt(Constants.CentralHeaderSignature);
                    WriteDataShort(Constants.Version);
                    WriteDataShort(Constants.Version);
                    WriteDataShort(entry.Flags);
                    WriteDataShort((short)entry.CompressionMethod);
                    WriteDataInt((int)GetDosTime(entry.DateTime));
                    WriteDataInt((int)entry.Crc);
                    WriteDataInt((int)entry.CompressedSize);
                    WriteDataInt((int)entry.Size);

                    byte[] name = ConvertToArray(entry.UseUnicodeText, entry.Name);
                    if (name.Length > 0xFFFF)
                    {
                        throw new ZipException("Entry name too long.");
                    }

                    WriteDataShort(name.Length);
                    WriteDataShort(extraLength);
                    WriteDataShort(commentLength);
                    WriteDataShort(0);	// disk number
                    WriteDataShort(0);	// internal file attributes
                    // no external file attributes

                    if (entry.IsDirectory)
                    {                         // mark entry as directory
                        WriteDataInt(16);
                    }
                    else
                    {
                        WriteDataInt(0);
                    }
                    WriteDataInt((int)entry.Offset);

                    if (name.Length > 0)
                    {
                        baseOutputStream.Write(name, 0, name.Length);
                    }

                    //no extra
                    //no comment

                    sizeEntries += Constants.CentralHeaderBaseSize + name.Length + extraLength + commentLength;
                }

                long startOfCentralDirectory = offset;

                #region WriteEndOfCentralDirectory
                WriteDataInt(Constants.EndOfCentralDirectorySignature);
                WriteDataShort(0);                    // number of this disk
                WriteDataShort(0);                    // no of disk with start of central dir
                WriteDataShort((short)numEntries);    // entries in central dir for this disk
                WriteDataShort((short)numEntries);    // total entries in central directory
                WriteDataInt((int)sizeEntries);       // size of the central directory
                WriteDataInt((int)startOfCentralDirectory);   // offset of start of central dir
                WriteDataShort(0);    //commentLength
                #endregion

                entriesList = null;
            }
            #endregion

            #region Methods.Close
            public override void Close()
            {
                entriesList = null;
                crc = null;
                currentEntry = null;
                base.Close();
            }
            #endregion

            #region Constructors
            /// <summary>
            /// Creates a new Zip output stream, writing a zip archive.
            /// </summary>
            /// <param name="baseOutputStream">
            /// The output stream to which the archive contents are written.
            /// </param>
            public ZipOutputStream(Stream baseOutputStream)
                : base(baseOutputStream)
            {
            }
            #endregion
        }
        #endregion

        #region ZipHelper
        private Stream mainStream = null;
        private ZipOutputStream zipStream = null;

        public void Begin(Stream stream, bool leaveOpen)
        {
            mainStream = stream;
            if (mainStream == null)
            {
                throw new ZipException("Output stream is null");
            }
            zipStream = new ZipOutputStream(mainStream);
            zipStream.IsStreamOwner = !leaveOpen;
        }

        public void AddFile(string fileName, MemoryStream dataStream)
        {
            AddFile(fileName, dataStream, false);
        }

        public void AddFile(string fileName, string sourceFileName)
        {
            AddFile(fileName, sourceFileName, false);
        }

        public void AddFile(string fileName, string sourceFileName, bool useUnicodeFileName)
        {
            if (File.Exists(sourceFileName))
            {
                var buffer = File.ReadAllBytes(sourceFileName);
                var dataStream = new MemoryStream(buffer);
                AddFile(fileName, dataStream, false, useUnicodeFileName);
            }
        }

        public void AddFile(string fileName, MemoryStream dataStream, bool closeDataStream)
        {
            AddFile(fileName, dataStream, closeDataStream, false);
        }

        public void AddFile(string fileName, MemoryStream dataStream, bool closeDataStream, bool useUnicodeFileName)
        {
            ZipEntry entry = new ZipEntry(fileName);
            entry.UseUnicodeText = useUnicodeFileName;
            zipStream.PutNextEntry(entry);
            dataStream.WriteTo(zipStream);
            zipStream.CloseEntry();
            if (closeDataStream) dataStream.Close();
        }

        public void End()
        {
            zipStream.Finish();
            zipStream.Close();
            mainStream = null;
            zipStream = null;
        }

        public StiZipWriter20()
        {
            mainStream = null;
            zipStream = null;
        }
        #endregion
    }

}
