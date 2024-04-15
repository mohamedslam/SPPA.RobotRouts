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

using System.IO;

namespace Stimulsoft.Report.Export.Tools
{
    public class StiCachedStream : MemoryStream
    {
        #region Fields
        private Stream baseOutputStream;
        private bool isStreamOwnerValue;
        private string fileName;
        private bool tryCache;
        #endregion

        #region Properties.Static
        public static int SizeLimitToStartUseCache { get; set; } = 50000000;
        #endregion

        #region Properties
        public override bool CanRead => baseOutputStream.CanRead;

        public override bool CanSeek => baseOutputStream.CanSeek;

        public override bool CanWrite => baseOutputStream.CanWrite;

        public override long Length => baseOutputStream.Length;

        public override long Position
        {
            get
            {
                return baseOutputStream.Position;
            }
            set
            {
                baseOutputStream.Position = value;
            }
        }

        public bool IsStreamOwner
        {
            get
            {
                return isStreamOwnerValue;
            }
            set
            {
                isStreamOwnerValue = value;
            }
        }
        #endregion

        #region Methods
        public override void Flush()
        {
            baseOutputStream.Flush();
        }

        public virtual void Finish()
        {
            baseOutputStream.Flush();
        }

        public override void Close()
        {
            Finish();

            if (isStreamOwnerValue)
                baseOutputStream.Close();

            if (fileName != null && File.Exists(fileName))
            {
                File.Delete(fileName);
                fileName = null;
            }
        }

        public override void WriteByte(byte bval)
        {
            Write(new[] { bval }, 0, 1);
        }

        public override void Write(byte[] buf, int off, int len)
        {
            baseOutputStream.Write(buf, off, len);

            if (tryCache && baseOutputStream.Length > SizeLimitToStartUseCache)
            {
                #region Replace memoryStream with fileStream
                try
                {
                    fileName = Path.GetTempFileName();
                    var fs = new FileStream(fileName, FileMode.Create);
                    (baseOutputStream as MemoryStream).WriteTo(fs);
                    baseOutputStream.Close();
                    baseOutputStream = fs;
                    tryCache = false;
                }
                catch
                {
                    tryCache = false;
                    try
                    {
                        if (fileName != null && File.Exists(fileName))
                            File.Delete(fileName);
                    }
                    catch
                    {
                    }
                    finally
                    {
                        fileName = null;
                    }
                }
                #endregion
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return baseOutputStream.Seek(offset, origin);
        }

        public override void SetLength(long val)
        {
            baseOutputStream.SetLength(val);
        }

        public override int ReadByte()
        {
            return baseOutputStream.ReadByte();
        }

        public override int Read(byte[] b, int off, int len)
        {
            return baseOutputStream.Read(b, off, len);
        }

        public override void WriteTo(Stream stream)
        {
            if (baseOutputStream is MemoryStream)
                (baseOutputStream as MemoryStream).WriteTo(stream);

            else
            {
                var fs = baseOutputStream as FileStream;
                fs.Seek(0, SeekOrigin.Begin);
                var buf = new byte[1024 * 1024];
                var len = fs.Length;
                while (len > 0)
                {
                    var len2 = fs.Read(buf, 0, buf.Length);
                    if (len2 > 0)
                        stream.Write(buf, 0, len2);

                    len = len2;
                }
            }
        }
        #endregion

        public StiCachedStream()
        {
            this.baseOutputStream = new MemoryStream();
            isStreamOwnerValue = true;
            tryCache = true;
        }
    }
}
