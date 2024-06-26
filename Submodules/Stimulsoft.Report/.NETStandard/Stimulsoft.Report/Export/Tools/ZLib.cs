#region ZipLib
/*
{*******************************************************************}
{					ZipLib from PdfCreator							}
{*******************************************************************}
*/
#endregion ZipLib

using System;
using System.IO;

namespace Stimulsoft.Report.Export
{
	#region DeflaterOutputStream
	public class DeflaterOutputStream : Stream
	{
		#region class variables
		protected byte[] buf;
		protected Deflater def;
        protected Stream baseOutputStream;
        protected bool isStreamOwnerValue;
		#endregion
		
		#region class properties
		public override bool CanRead 
		{
			get 
			{
				return baseOutputStream.CanRead;
			}
		}
		
		public override bool CanSeek 
		{
			get 
			{
				return baseOutputStream.CanSeek;
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
		
		#region class methods
		public override void Flush()
		{
			def.Flush();
			deflate();
			baseOutputStream.Flush();
		}
		
		public virtual void Finish()
		{
			def.Finish();
			while (!def.IsFinished) 
			{
				int len = def.Deflate(buf, 0, buf.Length);
				if (len <= 0) 
				{
					break;
				}
				baseOutputStream.Write(buf, 0, len);
			}
			if (!def.IsFinished) 
			{
				throw new ApplicationException("Can't deflate all input?");
			}
			baseOutputStream.Flush();
		}
		
		public override void Close()
		{
			Finish();
			if (isStreamOwnerValue)
            {
                baseOutputStream.Close();
            }
		}
		
		public override void WriteByte(byte bval)
		{
			byte[] b = new byte[1];
			b[0] = (byte) bval;
			Write(b, 0, 1);
		}
		
		public override void Write(byte[] buf, int off, int len)
		{
			def.SetInput(buf, off, len);
			deflate();
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
		
		protected void deflate()
		{
			while (!def.IsNeedingInput) 
			{
				int len = def.Deflate(buf, 0, buf.Length);
				
				if (len <= 0) 
				{
					break;
				}
				baseOutputStream.Write(buf, 0, len);
			}
			
			if (!def.IsNeedingInput) 
			{
				throw new ApplicationException("Can't deflate all input?");
			}
		}
		#endregion
		
		#region constructor
		public DeflaterOutputStream(Stream baseOutputStream) : this(baseOutputStream, new Deflater(), 512)
		{			
		}
		
		public DeflaterOutputStream(Stream baseOutputStream, Deflater defl) :this(baseOutputStream, defl, 512)
		{
		}
		
		public DeflaterOutputStream(Stream baseOutputStream, Deflater defl, int bufsize)
		{
			this.baseOutputStream = baseOutputStream;
			if (bufsize <= 0) 
			{
				throw new InvalidOperationException("bufsize <= 0");
			}
			buf = new byte[bufsize];
			def = defl;
			isStreamOwnerValue = true;
		}
		#endregion	
	}
	#endregion

	#region Deflater
	public class Deflater
	{
		#region class variables
		public static  int BEST_COMPRESSION = 9;
		public static  int BEST_SPEED = 1;
		public static  int DEFAULT_COMPRESSION = -1;
		public static  int NO_COMPRESSION = 0;
		public static  int DEFLATED = 8;
			
		private static  int IS_SETDICT              = 0x01;
		private static  int IS_FLUSHING             = 0x04;
		private static  int IS_FINISHING            = 0x08;
		
		private static  int INIT_STATE              = 0x00;
		private static  int SETDICT_STATE           = 0x01;
		private static  int BUSY_STATE              = 0x10;
		private static  int FLUSHING_STATE          = 0x14;
		private static  int FINISHING_STATE         = 0x1c;
		private static  int FINISHED_STATE          = 0x1e;
		private static  int CLOSED_STATE            = 0x7f;
		
		private int level;
		private bool noHeader;		
		private int state;
		private int totalOut;
		private DeflaterPending pending;
		private DeflaterEngine engine;
		#endregion

		#region constructor
		public Deflater() : this(DEFAULT_COMPRESSION, false)
		{
			
		}

		public Deflater(int lvl) : this(lvl, false)
		{
			
		}
		
		public Deflater(int lvl, bool nowrap)
		{
			if (lvl == DEFAULT_COMPRESSION) 
			{
				lvl = 5;
			} 
			else if (lvl < NO_COMPRESSION || lvl > BEST_COMPRESSION) 
			{
				throw new ArgumentOutOfRangeException("lvl");
			}
			
			pending = new DeflaterPending();
			engine = new DeflaterEngine(pending);
			this.noHeader = nowrap;
			SetStrategy(DeflateStrategy.Default);
			SetLevel(lvl);
			Reset();
		}
		#endregion
		
		#region class methods
		public void SetInput(byte[] input)
		{
			SetInput(input, 0, input.Length);
		}
		
		public void SetInput(byte[] input, int off, int len)
		{
			if ((state & IS_FINISHING) != 0) 
			{
				throw new InvalidOperationException("finish()/end() already called");
			}
			engine.SetInput(input, off, len);
		}
		
		public void SetLevel(int lvl)
		{
			if (lvl == DEFAULT_COMPRESSION) 
			{
				lvl = 6;
			} 
			else if (lvl < NO_COMPRESSION || lvl > BEST_COMPRESSION) 
			{
				throw new ArgumentOutOfRangeException("lvl");
			}
			
			
			if (level != lvl) 
			{
				level = lvl;
				engine.SetLevel(lvl);
			}
		}
		
		public void SetStrategy(DeflateStrategy stgy)
		{
			engine.Strategy = stgy;
		}
		
		public int Deflate(byte[] output)
		{
			return Deflate(output, 0, output.Length);
		}
		
		public int Deflate(byte[] output, int offset, int length)
		{
			int origLength = length;
			
			if (state == CLOSED_STATE) 
			{
				throw new InvalidOperationException("Deflater closed");
			}
			
			if (state < BUSY_STATE) 
			{
				int header = (DEFLATED +
					((DeflaterConstants.MAX_WBITS - 8) << 4)) << 8;
				int level_flags = (level - 1) >> 1;
				if (level_flags < 0 || level_flags > 3) 
				{
					level_flags = 3;
				}
				header |= level_flags << 6;
				if ((state & IS_SETDICT) != 0) 
				{
					header |= DeflaterConstants.PRESET_DICT;
				}
				header += 31 - (header % 31);
				
				
				pending.WriteShortMSB(header);
				if ((state & IS_SETDICT) != 0) 
				{
					int chksum = engine.Adler;
					engine.ResetAdler();
					pending.WriteShortMSB(chksum >> 16);
					pending.WriteShortMSB(chksum & 0xffff);
				}
				
				state = BUSY_STATE | (state & (IS_FLUSHING | IS_FINISHING));
			}
			
			for (;;) 
			{
				int count = pending.Flush(output, offset, length);
				offset   += count;
				totalOut += count;
				length   -= count;
				
				if (length == 0 || state == FINISHED_STATE) 
				{
					break;
				}
				
				if (!engine.Deflate((state & IS_FLUSHING) != 0, (state & IS_FINISHING) != 0)) 
				{
					if (state == BUSY_STATE) 
					{
						return origLength - length;
					} 
					else if (state == FLUSHING_STATE) 
					{
						if (level != NO_COMPRESSION) 
						{
							int neededbits = 8 + ((-pending.BitCount) & 7);
							while (neededbits > 0) 
							{
								pending.WriteBits(2, 10);
								neededbits -= 10;
							}
						}
						state = BUSY_STATE;
					} 
					else if (state == FINISHING_STATE) 
					{
						pending.AlignToByte();
						if (!noHeader) 
						{
							int adler = engine.Adler;
							pending.WriteShortMSB(adler >> 16);
							pending.WriteShortMSB(adler & 0xffff);
						}
						state = FINISHED_STATE;
					}
				}
			}
			return origLength - length;
		}
		
		public void SetDictionary(byte[] dict)
		{
			SetDictionary(dict, 0, dict.Length);
		}
		
		public void SetDictionary(byte[] dict, int offset, int length)
		{
			if (state != INIT_STATE) 
			{
				throw new InvalidOperationException();
			}
			
			state = SETDICT_STATE;
			engine.SetDictionary(dict, offset, length);
		}

		public void Flush() 
		{
			state |= IS_FLUSHING;
		}
		
		public void Finish() 
		{
			state |= IS_FLUSHING | IS_FINISHING;
		}

		public void Reset()
		{
			state = (noHeader ? BUSY_STATE : INIT_STATE);
			totalOut = 0;
			pending.Reset();
			engine.Reset();
		}
		#endregion
		
		#region class properties
		public bool IsFinished 
		{
			get 
			{
				return state == FINISHED_STATE && pending.IsFlushed;
			}
		}
		
		public bool IsNeedingInput 
		{
			get 
			{
				return engine.NeedsInput();
			}
		}

		public int Adler 
		{
			get 
			{
				return engine.Adler;
			}
		}
		
		public int TotalIn 
		{
			get 
			{
				return engine.TotalIn;
			}
		}
		
		public int TotalOut 
		{
			get 
			{
				return totalOut;
			}
		}
		#endregion	
	}
	#endregion

	#region DeflaterEngine
	public class DeflaterEngine : DeflaterConstants 
	{
		#region class variables
		static int TOO_FAR = 4096;		
		int ins_h;
		short[] head;
		short[] prev;		
		int    matchStart, matchLen;
		bool   prevAvailable;
		int    blockStart;
		int    strstart, lookahead;
		byte[] window;
		
		DeflateStrategy strategy;
		int max_chain, max_lazy, niceLength, goodLength;
		int comprFunc;
		byte[] inputBuf;
		int totalIn;
		int inputOff;
		int inputEnd;
		
		DeflaterPending pending;
		DeflaterHuffman huffman;
		Adler32 adler;
		#endregion
		
		#region constructor
		public DeflaterEngine(DeflaterPending pending) 
		{
			this.pending = pending;
			huffman = new DeflaterHuffman(pending);
			adler = new Adler32();
			
			window = new byte[2 * WSIZE];
			head   = new short[HASH_SIZE];
			prev   = new short[WSIZE];
			blockStart = strstart = 1;
		}
		#endregion
		
		#region class methods
		public void SetLevel(int lvl)
		{
			goodLength = DeflaterConstants.GOOD_LENGTH[lvl];
			max_lazy   = DeflaterConstants.MAX_LAZY[lvl];
			niceLength = DeflaterConstants.NICE_LENGTH[lvl];
			max_chain  = DeflaterConstants.MAX_CHAIN[lvl];
			
			if (DeflaterConstants.COMPR_FUNC[lvl] != comprFunc) 
			{
				switch (comprFunc) 
				{
					case DEFLATE_STORED:
						if (strstart > blockStart) 
						{
							huffman.FlushStoredBlock(window, blockStart,
								strstart - blockStart, false);
							blockStart = strstart;
						}
						UpdateHash();
						break;
					case DEFLATE_FAST:
						if (strstart > blockStart) 
						{
							huffman.FlushBlock(window, blockStart, strstart - blockStart,
								false);
							blockStart = strstart;
						}
						break;
					case DEFLATE_SLOW:
						if (prevAvailable) 
						{
							huffman.TallyLit(window[strstart-1] & 0xff);
						}
						if (strstart > blockStart) 
						{
							huffman.FlushBlock(window, blockStart, strstart - blockStart,
								false);
							blockStart = strstart;
						}
						prevAvailable = false;
						matchLen = MIN_MATCH - 1;
						break;
				}
				comprFunc = COMPR_FUNC[lvl];
			}
		}
		
		private void UpdateHash() 
		{
			ins_h = (window[strstart] << HASH_SHIFT) ^ window[strstart + 1];
		}
		
		private int InsertString() 
		{
			short match;
			int hash = ((ins_h << HASH_SHIFT) ^ window[strstart + (MIN_MATCH -1)]) & HASH_MASK;
			
			prev[strstart & WMASK] = match = head[hash];
			head[hash] = (short)strstart;
			ins_h = hash;
			return match & 0xffff;
		}
		
		void SlideWindow()
		{
			Array.Copy(window, WSIZE, window, 0, WSIZE);
			matchStart -= WSIZE;
			strstart   -= WSIZE;
			blockStart -= WSIZE;
			
			for (int i = 0; i < HASH_SIZE; ++i) 
			{
				int m = head[i] & 0xffff;
				head[i] = (short)(m >= WSIZE ? (m - WSIZE) : 0);
			}
			
			for (int i = 0; i < WSIZE; i++) 
			{
				int m = prev[i] & 0xffff;
				prev[i] = (short)(m >= WSIZE ? (m - WSIZE) : 0);
			}
		}
		
		public void FillWindow()
		{
			if (strstart >= WSIZE + MAX_DIST) 
			{
				SlideWindow();
			}
			
			while (lookahead < DeflaterConstants.MIN_LOOKAHEAD && inputOff < inputEnd) 
			{
				int more = 2 * WSIZE - lookahead - strstart;
				
				if (more > inputEnd - inputOff) 
				{
					more = inputEnd - inputOff;
				}
				
				Array.Copy(inputBuf, inputOff, window, strstart + lookahead, more);
				adler.Update(inputBuf, inputOff, more);
				
				inputOff += more;
				totalIn  += more;
				lookahead += more;
			}
			
			if (lookahead >= MIN_MATCH) 
			{
				UpdateHash();
			}
		}
		
		private bool FindLongestMatch(int curMatch) 
		{
			int chainLength = this.max_chain;
			int niceLength  = this.niceLength;
			short[] prev    = this.prev;
			int scan        = this.strstart;
			int match;
			int best_end = this.strstart + matchLen;
			int best_len = Math.Max(matchLen, MIN_MATCH - 1);
			
			int limit = Math.Max(strstart - MAX_DIST, 0);
			
			int strend = strstart + MAX_MATCH - 1;
			byte scan_end1 = window[best_end - 1];
			byte scan_end  = window[best_end];
			
			if (best_len >= this.goodLength) 
			{
				chainLength >>= 2;
			}
			
			if (niceLength > lookahead) 
			{
				niceLength = lookahead;
			}
			
			do 
			{
				if (window[curMatch + best_len] != scan_end      || 
					window[curMatch + best_len - 1] != scan_end1 || 
					window[curMatch] != window[scan]             || 
					window[curMatch + 1] != window[scan + 1]) 
				{
					continue;
				}
				
				match = curMatch + 2;
				scan += 2;
				
			while (window[++scan] == window[++match] && 
				window[++scan] == window[++match] && 
				window[++scan] == window[++match] && 
				window[++scan] == window[++match] && 
				window[++scan] == window[++match] && 
				window[++scan] == window[++match] && 
				window[++scan] == window[++match] && 
				window[++scan] == window[++match] && scan < strend) ;
				
				if (scan > best_end) 
				{
					matchStart = curMatch;
					best_end = scan;
					best_len = scan - strstart;
					if (best_len >= niceLength) 
					{
						break;
					}
					
					scan_end1  = window[best_end - 1];
					scan_end   = window[best_end];
				}
				scan = strstart;
			} while ((curMatch = (prev[curMatch & WMASK] & 0xffff)) > limit && --chainLength != 0);
			
			matchLen = Math.Min(best_len, lookahead);
			return matchLen >= MIN_MATCH;
		}
		
		public void SetDictionary(byte[] buffer, int offset, int length) 
		{
			adler.Update(buffer, offset, length);
			if (length < MIN_MATCH) 
			{
				return;
			}
			if (length > MAX_DIST) 
			{
				offset += length - MAX_DIST;
				length = MAX_DIST;
			}
			
			Array.Copy(buffer, offset, window, strstart, length);
			
			UpdateHash();
			--length;
			while (--length > 0) 
			{
				InsertString();
				strstart++;
			}
			strstart += 2;
			blockStart = strstart;
		}
		
		private bool DeflateStored(bool flush, bool finish)
		{
			if (!flush && lookahead == 0) 
			{
				return false;
			}
			
			strstart += lookahead;
			lookahead = 0;
			
			int storedLen = strstart - blockStart;
			
			if ((storedLen >= DeflaterConstants.MAX_BLOCK_SIZE) || 
				(blockStart < WSIZE && storedLen >= MAX_DIST) ||   
				flush) 
			{
				bool lastBlock = finish;
				if (storedLen > DeflaterConstants.MAX_BLOCK_SIZE) 
				{
					storedLen = DeflaterConstants.MAX_BLOCK_SIZE;
					lastBlock = false;
				}
									
				huffman.FlushStoredBlock(window, blockStart, storedLen, lastBlock);
				blockStart += storedLen;
				return !lastBlock;
			}
			return true;
		}
		
		private bool DeflateFast(bool flush, bool finish)
		{
			if (lookahead < MIN_LOOKAHEAD && !flush) 
			{
				return false;
			}
			
			while (lookahead >= MIN_LOOKAHEAD || flush) 
			{
				if (lookahead == 0) 
				{
					huffman.FlushBlock(window, blockStart, strstart - blockStart, finish);
					blockStart = strstart;
					return false;
				}
				
				if (strstart > 2 * WSIZE - MIN_LOOKAHEAD)
				{
					SlideWindow();
				}
				
				int hashHead;
				if (lookahead >= MIN_MATCH && 
					(hashHead = InsertString()) != 0 && 
					strategy != DeflateStrategy.HuffmanOnly &&
					strstart - hashHead <= MAX_DIST && 
					FindLongestMatch(hashHead)) 
				{
					
					huffman.TallyDist(strstart - matchStart, matchLen);
					
					lookahead -= matchLen;
					if (matchLen <= max_lazy && lookahead >= MIN_MATCH) 
					{
						while (--matchLen > 0) 
						{
							++strstart;
							InsertString();
						}
						++strstart;
					} 
					else 
					{
						strstart += matchLen;
						if (lookahead >= MIN_MATCH - 1) 
						{
							UpdateHash();
						}
					}
					matchLen = MIN_MATCH - 1;
					continue;
				} 
				else 
				{
					huffman.TallyLit(window[strstart] & 0xff);
					++strstart;
					--lookahead;
				}
				
				if (huffman.IsFull()) 
				{
					bool lastBlock = finish && lookahead == 0;
					huffman.FlushBlock(window, blockStart, strstart - blockStart,
						lastBlock);
					blockStart = strstart;
					return !lastBlock;
				}
			}
			return true;
		}
		
		private bool DeflateSlow(bool flush, bool finish)
		{
			if (lookahead < MIN_LOOKAHEAD && !flush) 
			{
				return false;
			}
			
			while (lookahead >= MIN_LOOKAHEAD || flush) 
			{
				if (lookahead == 0) 
				{
					if (prevAvailable) 
					{
						huffman.TallyLit(window[strstart-1] & 0xff);
					}
					prevAvailable = false;
					
					huffman.FlushBlock(window, blockStart, strstart - blockStart,
						finish);
					blockStart = strstart;
					return false;
				}
				
				if (strstart >= 2 * WSIZE - MIN_LOOKAHEAD)
				{
					SlideWindow();
				}
				
				int prevMatch = matchStart;
				int prevLen = matchLen;
				if (lookahead >= MIN_MATCH) 
				{
					int hashHead = InsertString();
					if (strategy != DeflateStrategy.HuffmanOnly && hashHead != 0 && strstart - hashHead <= MAX_DIST && FindLongestMatch(hashHead))
					{
						if (matchLen <= 5 && (strategy == DeflateStrategy.Filtered || (matchLen == MIN_MATCH && strstart - matchStart > TOO_FAR))) 
						{
							matchLen = MIN_MATCH - 1;
						}
					}
				}
				
				if (prevLen >= MIN_MATCH && matchLen <= prevLen) 
				{
					huffman.TallyDist(strstart - 1 - prevMatch, prevLen);
					prevLen -= 2;
					do 
					{
						strstart++;
						lookahead--;
						if (lookahead >= MIN_MATCH) 
						{
							InsertString();
						}
					} while (--prevLen > 0);
					strstart ++;
					lookahead--;
					prevAvailable = false;
					matchLen = MIN_MATCH - 1;
				} 
				else 
				{
					if (prevAvailable) 
					{
						huffman.TallyLit(window[strstart-1] & 0xff);
					}
					prevAvailable = true;
					strstart++;
					lookahead--;
				}
				
				if (huffman.IsFull()) 
				{
					int len = strstart - blockStart;
					if (prevAvailable) 
					{
						len--;
					}
					bool lastBlock = (finish && lookahead == 0 && !prevAvailable);
					huffman.FlushBlock(window, blockStart, len, lastBlock);
					blockStart += len;
					return !lastBlock;
				}
			}
			return true;
		}
		
		public bool Deflate(bool flush, bool finish)
		{
			bool progress;
			do 
			{
				FillWindow();
				bool canFlush = flush && inputOff == inputEnd;
				switch (comprFunc) 
				{
					case DEFLATE_STORED:
						progress = DeflateStored(canFlush, finish);
						break;
					case DEFLATE_FAST:
						progress = DeflateFast(canFlush, finish);
						break;
					case DEFLATE_SLOW:
						progress = DeflateSlow(canFlush, finish);
						break;
					default:
						throw new InvalidOperationException("unknown comprFunc");
				}
			} while (pending.IsFlushed && progress); 
			return progress;
		}
		
		public void SetInput(byte[] buf, int off, int len)
		{
			if (inputOff < inputEnd) 
			{
				throw new InvalidOperationException("Old input was not completely processed");
			}
			
			int end = off + len;
			
			if (0 > off || off > end || end > buf.Length) 
			{
				throw new ArgumentOutOfRangeException();
			}
			
			inputBuf = buf;
			inputOff = off;
			inputEnd = end;
		}
		
		public bool NeedsInput()
		{
			return inputEnd == inputOff;
		}

		public void Reset()
		{
			huffman.Reset();
			adler.Reset();
			blockStart = strstart = 1;
			lookahead = 0;
			totalIn   = 0;
			prevAvailable = false;
			matchLen = MIN_MATCH - 1;
			
			for (int i = 0; i < HASH_SIZE; i++) 
			{
				head[i] = 0;
			}
			
			for (int i = 0; i < WSIZE; i++) 
			{
				prev[i] = 0;
			}
		}
		
		public void ResetAdler()
		{
			adler.Reset();
		}
		#endregion
		
		#region class properties
		public int Adler 
		{
			get 
			{
				return (int)adler.Value;
			}
		}
		
		public int TotalIn 
		{
			get 
			{
				return totalIn;
			}
		}
		
		public DeflateStrategy Strategy 
		{
			get 
			{
				return strategy;
			}
			set 
			{
				strategy = value;
			}
		}
		#endregion	
	}
	#endregion

	#region Adler32
	public sealed class Adler32 : IChecksum
	{
		#region class variables
		readonly static uint BASE = 65521;		
		uint checksum;
		#endregion

		#region class properties
		public long Value 
		{
			get 
			{
				return checksum;
			}
		}
		#endregion
		
		#region constructor
		public Adler32()
		{
			Reset();
		}
		#endregion
		
		#region class methods
		public void Reset()
		{
			checksum = 1; 
		}
		
		public void Update(int bval)
		{
			uint s1 = checksum & 0xFFFF;
			uint s2 = checksum >> 16;
			
			s1 = (s1 + ((uint)bval & 0xFF)) % BASE;
			s2 = (s1 + s2) % BASE;
			
			checksum = (s2 << 16) + s1;
		}
		
		public void Update(byte[] buffer)
		{
			Update(buffer, 0, buffer.Length);
		}
		
		public void Update(byte[] buf, int off, int len)
		{
			if (buf == null) 
			{
				throw new ArgumentNullException("buf");
			}
			
			if (off < 0 || len < 0 || off + len > buf.Length) 
			{
				throw new ArgumentOutOfRangeException();
			}
			
			uint s1 = checksum & 0xFFFF;
			uint s2 = checksum >> 16;
			
			while (len > 0) 
			{
				int n = 3800;
				if (n > len) 
				{
					n = len;
				}
				len -= n;
				while (--n >= 0) 
				{
					s1 = s1 + (uint)(buf[off++] & 0xFF);
					s2 = s2 + s1;
				}
				s1 %= BASE;
				s2 %= BASE;
			}
			
			checksum = (s2 << 16) | s1;
		}
		#endregion
	}
	#endregion

	#region DeflaterConstants
	public class DeflaterConstants 
	{
		#region class variables
		public const bool DEBUGGING = false;
		
		public const int STORED_BLOCK = 0;
		public const int STATIC_TREES = 1;
		public const int DYN_TREES    = 2;
		public const int PRESET_DICT  = 0x20;
		
		public const int DEFAULT_MEM_LEVEL = 7;     //initial value 8, set to 7 to reduce memory consumption
		public const int MAX_WBITS = 14;            //initial value 15, set to 14 to reduce memory consumption

		public const int MAX_MATCH = 258;
		public const int MIN_MATCH = 3;
		
		public const int WSIZE = 1 << MAX_WBITS;
		public const int WMASK = WSIZE - 1;
		
		public const int HASH_BITS = DEFAULT_MEM_LEVEL + 7;
		public const int HASH_SIZE = 1 << HASH_BITS;
		public const int HASH_MASK = HASH_SIZE - 1;
		public const int HASH_SHIFT = (HASH_BITS + MIN_MATCH - 1) / MIN_MATCH;
		
		public const int MIN_LOOKAHEAD = MAX_MATCH + MIN_MATCH + 1;
		public const int MAX_DIST = WSIZE - MIN_LOOKAHEAD;
		
		public const int PENDING_BUF_SIZE = 1 << (DEFAULT_MEM_LEVEL + 8);
		public static int MAX_BLOCK_SIZE = Math.Min(65535, PENDING_BUF_SIZE-5);
		
		public const int DEFLATE_STORED = 0;
		public const int DEFLATE_FAST   = 1;
		public const int DEFLATE_SLOW   = 2;
		
		public static int[] GOOD_LENGTH = { 0, 4, 4, 4, 4, 8,  8,  8,  32,  32 };
		public static int[] MAX_LAZY    = { 0, 4, 5, 6, 4,16, 16, 32, 128, 258 };
		public static int[] NICE_LENGTH = { 0, 8,16,32,16,32,128,128, 258, 258 };
		public static int[] MAX_CHAIN   = { 0, 4, 8,32,16,32,128,256,1024,4096 };
		public static int[] COMPR_FUNC  = { 0, 1, 1, 1, 1, 2,  2,  2,   2,   2 };
		#endregion
	}
	#endregion

	#region DeflaterHuffman
	public class DeflaterHuffman
	{
		#region class variables
		public DeflaterPending pending;
		private Tree literalTree, distTree, blTree;
		
		private short[] d_buf;
		private byte[]  l_buf;
		private int last_lit;
		private int extra_bits;
		
		private static short[] staticLCodes;
		private static byte[]  staticLLength;
		private static short[] staticDCodes;
		private static byte[]  staticDLength;

		private static  int BUFSIZE = 1 << (DeflaterConstants.DEFAULT_MEM_LEVEL + 6);
		private static  int LITERAL_NUM = 286;
		private static  int DIST_NUM = 30;
		private static  int BITLEN_NUM = 19;
		private static  int REP_3_6    = 16;
		private static  int REP_3_10   = 17;
		private static  int REP_11_138 = 18;
		private static  int EOF_SYMBOL = 256;
		private static  int[] BL_ORDER = { 16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15 };
		
		private static byte[] bit4Reverse = {
												0,
												8,
												4,
												12,
												2,
												10,
												6,
												14,
												1,
												9,
												5,
												13,
												3,
												11,
												7,
												15
											};
		#endregion
		
		#region Tree
		public class Tree 
		{
			#region class variables
			public short[] freqs;
			public short[] codes;
			public byte[]  length;
			public int[]   bl_counts;
			public int     minNumCodes, numCodes;
			public int     maxLength;
			DeflaterHuffman dh;
			#endregion
			
			#region constructor
			public Tree(DeflaterHuffman dh, int elems, int minCodes, int maxLength) 
			{
				this.dh =  dh;
				this.minNumCodes = minCodes;
				this.maxLength  = maxLength;
				freqs  = new short[elems];
				bl_counts = new int[maxLength];
			}
			#endregion
			
			#region class methods
			public void Reset() 
			{
				for (int i = 0; i < freqs.Length; i++) 
				{
					freqs[i] = 0;
				}
				codes = null;
				length = null;
			}
			
			public void WriteSymbol(int code)
			{
				dh.pending.WriteBits(codes[code] & 0xffff, length[code]);
			}
			
			public void CheckEmpty()
			{
				bool empty = true;
				for (int i = 0; i < freqs.Length; i++) 
				{
					if (freqs[i] != 0) 
					{
						Console.WriteLine("freqs["+i+"] == "+freqs[i]);
						empty = false;
					}
				}
				if (!empty) 
				{
					throw new Exception();
				}
				Console.WriteLine("checkEmpty suceeded!");
			}
			
			public void SetStaticCodes(short[] stCodes, byte[] stLength)
			{
				codes = stCodes;
				length = stLength;
			}
			
			public void BuildCodes() 
			{
				int numSymbols = freqs.Length;
				int[] nextCode = new int[maxLength];
				int code = 0;
				codes = new short[freqs.Length];
				
				for (int bits = 0; bits < maxLength; bits++) 
				{
					nextCode[bits] = code;
					code += bl_counts[bits] << (15 - bits);
				}

				for (int i=0; i < numCodes; i++) 
				{
					int bits = length[i];
					if (bits > 0) 
					{
						codes[i] = BitReverse(nextCode[bits-1]);
						nextCode[bits-1] += 1 << (16 - bits);
					}
				}
			}
			
			private void BuildLength(int[] childs)
			{
				this.length = new byte [freqs.Length];
				int numNodes = childs.Length / 2;
				int numLeafs = (numNodes + 1) / 2;
				int overflow = 0;
				
				for (int i = 0; i < maxLength; i++) 
				{
					bl_counts[i] = 0;
				}
				
				int[] lengths = new int[numNodes];
				lengths[numNodes-1] = 0;
				
				for (int i = numNodes - 1; i >= 0; i--) 
				{
					if (childs[2*i+1] != -1) 
					{
						int bitLength = lengths[i] + 1;
						if (bitLength > maxLength) 
						{
							bitLength = maxLength;
							overflow++;
						}
						lengths[childs[2*i]] = lengths[childs[2*i+1]] = bitLength;
					} 
					else 
					{
						int bitLength = lengths[i];
						bl_counts[bitLength - 1]++;
						this.length[childs[2*i]] = (byte) lengths[i];
					}
				}
				
				if (overflow == 0) 
				{
					return;
				}
				
				int incrBitLen = maxLength - 1;
				do 
				{
				while (bl_counts[--incrBitLen] == 0)
					;
					
					do 
					{
						bl_counts[incrBitLen]--;
						bl_counts[++incrBitLen]++;
						overflow -= 1 << (maxLength - 1 - incrBitLen);
					} while (overflow > 0 && incrBitLen < maxLength - 1);
				} while (overflow > 0);
				
				bl_counts[maxLength-1] += overflow;
				bl_counts[maxLength-2] -= overflow;
				
				int nodePtr = 2 * numLeafs;
				for (int bits = maxLength; bits != 0; bits--) 
				{
					int n = bl_counts[bits-1];
					while (n > 0) 
					{
						int childPtr = 2*childs[nodePtr++];
						if (childs[childPtr + 1] == -1) 
						{
							length[childs[childPtr]] = (byte) bits;
							n--;
						}
					}
				}
			}
			
			public void BuildTree()
			{
				int numSymbols = freqs.Length;
				int[] heap = new int[numSymbols];
				int heapLen = 0;
				int maxCode = 0;
				for (int n = 0; n < numSymbols; n++) 
				{
					int freq = freqs[n];
					if (freq != 0) 
					{
						int pos = heapLen++;
						int ppos;
						while (pos > 0 && freqs[heap[ppos = (pos - 1) / 2]] > freq) 
						{
							heap[pos] = heap[ppos];
							pos = ppos;
						}
						heap[pos] = n;
						
						maxCode = n;
					}
				}
				
				while (heapLen < 2) 
				{
					int node = maxCode < 2 ? ++maxCode : 0;
					heap[heapLen++] = node;
				}
				
				numCodes = Math.Max(maxCode + 1, minNumCodes);
				
				int numLeafs = heapLen;
				int[] childs = new int[4*heapLen - 2];
				int[] values = new int[2*heapLen - 1];
				int numNodes = numLeafs;
				for (int i = 0; i < heapLen; i++) 
				{
					int node = heap[i];
					childs[2*i]   = node;
					childs[2*i+1] = -1;
					values[i] = freqs[node] << 8;
					heap[i] = i;
				}
				
				do 
				{
					int first = heap[0];
					int last  = heap[--heapLen];
					int ppos = 0;
					int path = 1;
				while (path < heapLen) 
				{
					if (path + 1 < heapLen && values[heap[path]] > values[heap[path+1]]) 
					{
						path++;
					}
						
					heap[ppos] = heap[path];
					ppos = path;
					path = path * 2 + 1;
				}
					int lastVal = values[last];
				while ((path = ppos) > 0 && values[heap[ppos = (path - 1)/2]] > lastVal) 
				{
					heap[path] = heap[ppos];
				}
					heap[path] = last;
					
					
					int second = heap[0];
					
					last = numNodes++;
					childs[2*last] = first;
					childs[2*last+1] = second;
					int mindepth = Math.Min(values[first] & 0xff, values[second] & 0xff);
					values[last] = lastVal = values[first] + values[second] - mindepth + 1;
					
					ppos = 0;
					path = 1;
				while (path < heapLen) 
				{
					if (path + 1 < heapLen && values[heap[path]] > values[heap[path+1]]) 
					{
						path++;
					}
						
					heap[ppos] = heap[path];
					ppos = path;
					path = ppos * 2 + 1;
				}
					
				while ((path = ppos) > 0 && values[heap[ppos = (path - 1)/2]] > lastVal) 
				{
					heap[path] = heap[ppos];
				}
					heap[path] = last;
				} while (heapLen > 1);
				
				if (heap[0] != childs.Length / 2 - 1) 
				{
					throw new Exception("Weird!");
				}
				BuildLength(childs);
			}
			
			public int GetEncodedLength()
			{
				int len = 0;
				for (int i = 0; i < freqs.Length; i++) 
				{
					len += freqs[i] * length[i];
				}
				return len;
			}
			
			public void CalcBLFreq(Tree blTree) 
			{
				int max_count;               
				int min_count;               
				int count;                   
				int curlen = -1;              
				
				int i = 0;
				while (i < numCodes) 
				{
					count = 1;
					int nextlen = length[i];
					if (nextlen == 0) 
					{
						max_count = 138;
						min_count = 3;
					} 
					else 
					{
						max_count = 6;
						min_count = 3;
						if (curlen != nextlen) 
						{
							blTree.freqs[nextlen]++;
							count = 0;
						}
					}
					curlen = nextlen;
					i++;
					
					while (i < numCodes && curlen == length[i]) 
					{
						i++;
						if (++count >= max_count) 
						{
							break;
						}
					}
					
					if (count < min_count) 
					{
						blTree.freqs[curlen] += (short)count;
					} 
					else if (curlen != 0) 
					{
						blTree.freqs[REP_3_6]++;
					} 
					else if (count <= 10) 
					{
						blTree.freqs[REP_3_10]++;
					} 
					else 
					{
						blTree.freqs[REP_11_138]++;
					}
				}
			}
			
			public void WriteTree(Tree blTree)
			{
				int max_count;              
				int min_count;              
				int count;                  
				int curlen = -1;            
				
				int i = 0;
				while (i < numCodes) 
				{
					count = 1;
					int nextlen = length[i];
					if (nextlen == 0) 
					{
						max_count = 138;
						min_count = 3;
					} 
					else 
					{
						max_count = 6;
						min_count = 3;
						if (curlen != nextlen) 
						{
							blTree.WriteSymbol(nextlen);
							count = 0;
						}
					}
					curlen = nextlen;
					i++;
					
					while (i < numCodes && curlen == length[i]) 
					{
						i++;
						if (++count >= max_count) 
						{
							break;
						}
					}
					
					if (count < min_count) 
					{
						while (count-- > 0) 
						{
							blTree.WriteSymbol(curlen);
						}
					}
					else if (curlen != 0) 
					{
						blTree.WriteSymbol(REP_3_6);
						dh.pending.WriteBits(count - 3, 2);
					} 
					else if (count <= 10) 
					{
						blTree.WriteSymbol(REP_3_10);
						dh.pending.WriteBits(count - 3, 3);
					} 
					else 
					{
						blTree.WriteSymbol(REP_11_138);
						dh.pending.WriteBits(count - 11, 7);
					}
				}
			}
			#endregion
		}
		#endregion
		
		#region class methods
		public void Reset() 
		{
			last_lit = 0;
			extra_bits = 0;
			literalTree.Reset();
			distTree.Reset();
			blTree.Reset();
		}
		
		private int L_code(int len) 
		{
			if (len == 255) 
			{
				return 285;
			}
			
			int code = 257;
			while (len >= 8) 
			{
				code += 4;
				len >>= 1;
			}
			return code + len;
		}
		
		private int D_code(int distance) 
		{
			int code = 0;
			while (distance >= 4) 
			{
				code += 2;
				distance >>= 1;
			}
			return code + distance;
		}
		
		public void SendAllTrees(int blTreeCodes)
		{
			blTree.BuildCodes();
			literalTree.BuildCodes();
			distTree.BuildCodes();
			pending.WriteBits(literalTree.numCodes - 257, 5);
			pending.WriteBits(distTree.numCodes - 1, 5);
			pending.WriteBits(blTreeCodes - 4, 4);
			for (int rank = 0; rank < blTreeCodes; rank++) 
			{
				pending.WriteBits(blTree.length[BL_ORDER[rank]], 3);
			}
			literalTree.WriteTree(blTree);
			distTree.WriteTree(blTree);
		}
		
		public void CompressBlock()
		{
			for (int i = 0; i < last_lit; i++) 
			{
				int litlen = l_buf[i] & 0xff;
				int dist = d_buf[i];
				if (dist-- != 0) 
				{					
					int lc = L_code(litlen);
					literalTree.WriteSymbol(lc);
					
					int bits = (lc - 261) / 4;
					if (bits > 0 && bits <= 5) 
					{
						pending.WriteBits(litlen & ((1 << bits) - 1), bits);
					}
					
					int dc = D_code(dist);
					distTree.WriteSymbol(dc);
					
					bits = dc / 2 - 1;
					if (bits > 0) 
					{
						pending.WriteBits(dist & ((1 << bits) - 1), bits);
					}
				} 
				else 
				{
					literalTree.WriteSymbol(litlen);
				}
			}
			literalTree.WriteSymbol(EOF_SYMBOL);
		}
		
		public void FlushStoredBlock(byte[] stored, int stored_offset, int stored_len, bool lastBlock)
		{
			pending.WriteBits((DeflaterConstants.STORED_BLOCK << 1)
				+ (lastBlock ? 1 : 0), 3);
			pending.AlignToByte();
			pending.WriteShort(stored_len);
			pending.WriteShort(~stored_len);
			pending.WriteBlock(stored, stored_offset, stored_len);
			Reset();
		}
		
		public void FlushBlock(byte[] stored, int stored_offset, int stored_len, bool lastBlock)
		{
			literalTree.freqs[EOF_SYMBOL]++;
			
			literalTree.BuildTree();
			distTree.BuildTree();
			
			literalTree.CalcBLFreq(blTree);
			distTree.CalcBLFreq(blTree);
			
			blTree.BuildTree();
			
			int blTreeCodes = 4;
			for (int i = 18; i > blTreeCodes; i--) 
			{
				if (blTree.length[BL_ORDER[i]] > 0) 
				{
					blTreeCodes = i+1;
				}
			}
			int opt_len = 14 + blTreeCodes * 3 + blTree.GetEncodedLength() + 
				literalTree.GetEncodedLength() + distTree.GetEncodedLength() + 
				extra_bits;
			
			int static_len = extra_bits;
			for (int i = 0; i < LITERAL_NUM; i++) 
			{
				static_len += literalTree.freqs[i] * staticLLength[i];
			}
			for (int i = 0; i < DIST_NUM; i++) 
			{
				static_len += distTree.freqs[i] * staticDLength[i];
			}
			if (opt_len >= static_len) 
			{
				opt_len = static_len;
			}
			
			if (stored_offset >= 0 && stored_len+4 < opt_len >> 3) 
			{
				FlushStoredBlock(stored, stored_offset, stored_len, lastBlock);
			} 
			else if (opt_len == static_len) 
			{
				pending.WriteBits((DeflaterConstants.STATIC_TREES << 1) + (lastBlock ? 1 : 0), 3);
				literalTree.SetStaticCodes(staticLCodes, staticLLength);
				distTree.SetStaticCodes(staticDCodes, staticDLength);
				CompressBlock();
				Reset();
			} 
			else 
			{
				pending.WriteBits((DeflaterConstants.DYN_TREES << 1) + (lastBlock ? 1 : 0), 3);
				SendAllTrees(blTreeCodes);
				CompressBlock();
				Reset();
			}
		}
		
		public bool IsFull()
		{
			return last_lit + 16 >= BUFSIZE; 
		}
		
		public bool TallyLit(int lit)
		{
			d_buf[last_lit] = 0;
			l_buf[last_lit++] = (byte)lit;
			literalTree.freqs[lit]++;
			return IsFull();
		}
		
		public bool TallyDist(int dist, int len)
		{			
			d_buf[last_lit]   = (short)dist;
			l_buf[last_lit++] = (byte)(len - 3);
			
			int lc = L_code(len - 3);
			literalTree.freqs[lc]++;
			if (lc >= 265 && lc < 285) 
			{
				extra_bits += (lc - 261) / 4;
			}
			
			int dc = D_code(dist - 1);
			distTree.freqs[dc]++;
			if (dc >= 4) 
			{
				extra_bits += dc / 2 - 1;
			}
			return IsFull();
		}

		public static short BitReverse(int value) 
		{
			return (short) (bit4Reverse[value & 0xF] << 12
				| bit4Reverse[(value >> 4) & 0xF] << 8
				| bit4Reverse[(value >> 8) & 0xF] << 4
				| bit4Reverse[value >> 12]);
		}
		#endregion
		
		#region constructor
		static DeflaterHuffman() 
		{
			staticLCodes = new short[LITERAL_NUM];
			staticLLength = new byte[LITERAL_NUM];
			int i = 0;
			while (i < 144) 
			{
				staticLCodes[i] = BitReverse((0x030 + i) << 8);
				staticLLength[i++] = 8;
			}
			while (i < 256) 
			{
				staticLCodes[i] = BitReverse((0x190 - 144 + i) << 7);
				staticLLength[i++] = 9;
			}
			while (i < 280) 
			{
				staticLCodes[i] = BitReverse((0x000 - 256 + i) << 9);
				staticLLength[i++] = 7;
			}
			while (i < LITERAL_NUM) 
			{
				staticLCodes[i] = BitReverse((0x0c0 - 280 + i)  << 8);
				staticLLength[i++] = 8;
			}
			
			staticDCodes = new short[DIST_NUM];
			staticDLength = new byte[DIST_NUM];
			for (i = 0; i < DIST_NUM; i++) 
			{
				staticDCodes[i] = BitReverse(i << 11);
				staticDLength[i] = 5;
			}
		}
		
		public DeflaterHuffman(DeflaterPending pending)
		{
			this.pending = pending;
			
			literalTree = new Tree(this, LITERAL_NUM, 257, 15);
			distTree    = new Tree(this, DIST_NUM, 1, 15);
			blTree      = new Tree(this, BITLEN_NUM, 4, 7);
			
			d_buf = new short[BUFSIZE];
			l_buf = new byte [BUFSIZE];
		}
		#endregion	
	}
	#endregion

	#region DeflaterPending
	public class DeflaterPending : PendingBuffer
	{
		#region constructor
		public DeflaterPending() : base(DeflaterConstants.PENDING_BUF_SIZE)
		{
		}
		#endregion
	}
	#endregion

	#region enums
	public enum DeflateStrategy 
	{
		Default  = 0,
		Filtered = 1,
		HuffmanOnly = 2
	}
	#endregion

	#region IChecksum
	public interface IChecksum
	{
		#region properties
		long Value 
		{
			get;
		}
		#endregion
		
		#region methods
		void Reset();		
		void Update(int bval);		
		void Update(byte[] buffer);		
		void Update(byte[] buf, int off, int len);
		#endregion
	}
	#endregion

	#region PendingBuffer
	public class PendingBuffer
	{
		#region class variables
		protected byte[] buf;
		int    start;
		int    end;		
		uint    bits;
		int    bitCount;
		#endregion
		
		#region constructor
		public PendingBuffer() : this( 4096 )
		{
			
		}
		
		public PendingBuffer(int bufsize)
		{
			buf = new byte[bufsize];
		}
		#endregion
		
		#region class methods
		public int Flush(byte[] output, int offset, int length) 
		{
			if (bitCount >= 8) 
			{
				buf[end++] = (byte) bits;
				bits >>= 8;
				bitCount -= 8;
			}
			if (length > end - start) 
			{
				length = end - start;
				Array.Copy(buf, start, output, offset, length);
				start = 0;
				end = 0;
			} 
			else 
			{
				Array.Copy(buf, start, output, offset, length);
				start += length;
			}
			return length;
		}
		
		public byte[] ToByteArray()
		{
			byte[] ret = new byte[ end - start ];
			Array.Copy(buf, start, ret, 0, ret.Length);
			start = 0;
			end = 0;
			return ret;
		}

		public void AlignToByte() 
		{
			if (bitCount > 0) 
			{
				buf[end++] = (byte) bits;
				if (bitCount > 8) 
				{
					buf[end++] = (byte) (bits >> 8);
				}
			}
			bits = 0;
			bitCount = 0;
		}
		
		public void WriteBits(int b, int count)
		{
			bits |= (uint)(b << bitCount);
			bitCount += count;
			if (bitCount >= 16) 
			{
				buf[end++] = (byte) bits;
				buf[end++] = (byte) (bits >> 8);
				bits >>= 16;
				bitCount -= 16;
			}
		}
		
		public void WriteShortMSB(int s) 
		{
			buf[end++] = (byte) (s >> 8);
			buf[end++] = (byte) s;
		}

		public void Reset() 
		{
			start = end = bitCount = 0;
		}
		
		public void WriteByte(int b)
		{
			buf[end++] = (byte) b;
		}
		
		public void WriteShort(int s)
		{
			buf[end++] = (byte) s;
			buf[end++] = (byte) (s >> 8);
		}
		
		public void WriteInt(int s)
		{
			buf[end++] = (byte) s;
			buf[end++] = (byte) (s >> 8);
			buf[end++] = (byte) (s >> 16);
			buf[end++] = (byte) (s >> 24);
		}
		
		public void WriteBlock(byte[] block, int offset, int len)
		{
			Array.Copy(block, offset, buf, end, len);
			end += len;
		}
		#endregion
		
		#region class properties
		public bool IsFlushed 
		{
			get 
			{
				return end == 0;
			}
		}

		public int BitCount 
		{
			get 
			{
				return bitCount;
			}
		}
		#endregion	
	}
	#endregion
}
