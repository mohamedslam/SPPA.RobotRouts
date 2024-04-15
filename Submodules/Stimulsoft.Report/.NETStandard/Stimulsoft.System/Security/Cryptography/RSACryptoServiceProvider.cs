using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Stimulsoft.System.Security.Cryptography
{
    public class RSACryptoServiceProvider : IDisposable
    {
        private RSAKey rsaKey;
        public RSACryptoServiceProvider(int v)
        {
            this.rsaKey = new RSAKey();
        }

        public bool VerifyData(byte[] check, SHA1CryptoServiceProvider sha, byte[] signature)
        {
            try
            {
                string message = Regex.Replace(Encoding.UTF7.GetString(check), "(?<!\r)\n", "\r\n");
                return this.rsaKey.VerifyString(message, Convert.ToBase64String(signature));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void FromXmlString(string xmlString)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus": this.rsaKey.SetPublic(node.InnerText, "10001"); break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }

            //rsa.ImportParameters(parameters);
        }

        public void Dispose()
        {
        }

        private static int rrr(int value, int pos)
        {
            if (pos != 0)
            {
                int mask = 0x7fffffff;
                value >>= 1;
                value &= mask;
                value >>= pos - 1;
            }
            return value;
        }

        private static uint rrr(uint value, int pos)
        {
            if (pos != 0)
            {
                uint mask = 0x7fffffff;
                value >>= 1;
                value &= mask;
                value >>= pos - 1;
            }
            return value;
        }

        class SHA1
        {
            private int blockLength = 64;
            private uint[] state = new uint[] { 0x67452301, 0xefcdab89, 0x98badcfe, 0x10325476, 0xc3d2e1f0 };
            private uint[] k = new uint[] { 0x5a827999, 0x6ed9eba1, 0x8f1bbcdc, 0xca62c1d6 };

            public static string signature = "3021300906052b0e03021a05000414";

            public static string Hex(string data)
            {
                SHA1 s = new SHA1();

                return s.ToHex(s.GetMD(data));
            }

            private uint[] GetMD(string data)
            {
                uint[] datz;

                datz = this.Unpack(data);
                datz = this.PaddingData(datz);

                return this.Round(datz);
            }

            private uint Rotl(uint v, int s)
            {
                return (v << s) | rrr(v, 32 - s);
            }

            private uint[] Round(uint[] blk)
            {
                uint[] stt = new uint[this.state.Length];
                uint[] tmpS = new uint[this.state.Length];
                uint tmp;
                uint[] x;

                for (int j = 0; j < this.state.Length; j++)
                {
                    stt[j] = this.state[j];
                }

                for (int i = 0; i < blk.Length; i += this.blockLength)
                {

                    for (int j = 0; j < this.state.Length; j++)
                    {
                        tmpS[j] = stt[j];
                    }

                    uint[] tb = new uint[this.blockLength];
                    Array.Copy(blk, i, tb, 0, this.blockLength);
                    x = this.ToBigEndian32(tb);

                    Array.Resize<uint>(ref x, 80);
                    for (int j = 16; j < 80; j++)
                    {
                        x[j] = this.Rotl(x[j - 3] ^ x[j - 8] ^ x[j - 14] ^ x[j - 16], 1);
                    }

                    for (int j = 0; j < 80; j++)
                    {
                        if (j < 20)
                            tmp = ((stt[1] & stt[2]) ^ (~stt[1] & stt[3])) + this.k[0];
                        else if (j < 40)
                            tmp = (stt[1] ^ stt[2] ^ stt[3]) + this.k[1];
                        else if (j < 60)
                            tmp = ((stt[1] & stt[2]) ^ (stt[1] & stt[3]) ^ (stt[2] & stt[3])) + this.k[2];
                        else
                            tmp = (stt[1] ^ stt[2] ^ stt[3]) + this.k[3];

                        tmp += this.Rotl(stt[0], 5) + x[j] + stt[4];
                        stt[4] = stt[3];
                        stt[3] = stt[2];
                        stt[2] = this.Rotl(stt[1], 30);
                        stt[1] = stt[0];
                        stt[0] = tmp;
                    }
                    for (int j = 0; j < this.state.Length; j++)
                        stt[j] += tmpS[j];
                }

                return this.FromBigEndian32(stt);
            }

            private uint[] PaddingData(uint[] datz)
            {
                int datLen = datz.Length;
                int n = datLen;
                Array.Resize<uint>(ref datz, n + 1);
                datz[n++] = 0x80;
                while (n % this.blockLength != 56)
                {
                    Array.Resize<uint>(ref datz, n + 1);
                    datz[n++] = 0;
                }

                datLen *= 8;

                Array.Resize<uint>(ref datz, datz.Length + 8);
                uint[] r = this.FromBigEndian32(new uint[] { (uint)datLen });
                Array.Copy(r, 0, datz, datz.Length - 4, r.Length);

                return datz;
            }

            private string ToHex(uint[] decz)
            {
                string hex = "";

                for (int i = 0; i < decz.Length; i++)
                    hex += (decz[i] > 0xf ? "" : "0") + decz[i].ToString("x");

                return hex;
            }

            private uint[] FromBigEndian32(uint[] blk)
            {
                uint[] tmp = new uint[blk.Length * 4];
                int n = 0;
                for (int i = 0; i < blk.Length; i++)
                {
                    tmp[n++] = rrr(blk[i], 24) & 0xff;
                    tmp[n++] = rrr(blk[i], 16) & 0xff;
                    tmp[n++] = rrr(blk[i], 8) & 0xff;
                    tmp[n++] = blk[i] & 0xff;
                }
                return tmp;
            }

            private uint[] ToBigEndian32(uint[] blk)
            {
                uint[] tmp = new uint[blk.Length / 4];
                int n = 0;
                for (int i = 0; i < blk.Length; i += 4, n++)
                {
                    tmp[n] = (blk[i] << 24) | (blk[i + 1] << 16) | (blk[i + 2] << 8) | blk[i + 3];
                }
                return tmp;
            }

            private uint[] Unpack(string dat)
            {
                Dictionary<int, uint> tmp = new Dictionary<int, uint>();
                int n = 0;
                uint c;

                for (int i = 0; i < dat.Length; i++)
                {
                    c = (uint)dat[i];
                    if (c <= 0xff) tmp[n++] = c;
                    else
                    {
                        tmp[n++] = rrr(c, 8);
                        tmp[n++] = c & 0xff;
                    }
                }

                uint[] r = new uint[tmp.Count];
                tmp.Values.CopyTo(r, 0);

                return r;
            }
        }

        /*class AES
        {
            class WordArray
            {
                public int[] words;
                public int sigBytes;

                public WordArray(int[] words = null, int? sigBytes = null)
                {
                    if (words == null) words = new int[0];
                    this.words = words;

                    if (sigBytes != null) this.sigBytes = (int)sigBytes;
                    else this.sigBytes = words.Length * 4;
                }

                public override string ToString()
                {
                    char[] latin1Chars = new char[this.sigBytes];
                    for (int i = 0; i < this.sigBytes; i++)
                    {
                        int bite = (rrr(this.words[rrr(i, 2)], (24 - (i % 4) * 8))) & 0xff;
                        latin1Chars[i] = (char)bite;
                    }

                    return new string(latin1Chars);//.fromUnicodeString();
                }

                public WordArray concat(WordArray wordArray)
                {
                    this.Clamp();

                    if (this.sigBytes % 4 != 0)
                    {
                        for (int i = 0; i < wordArray.sigBytes; i++)
                        {
                            int thatByte = rrr(wordArray.words[rrr(i, 2)], (24 - (i % 4) * 8)) & 0xff;
                            this.words[rrr(this.sigBytes + i, 2)] |= thatByte << (24 - ((this.sigBytes + i) % 4) * 8);
                        }
                    }
                    else if (wordArray.words.Length > 0xffff)
                    {
                        for (int i = 0; i < wordArray.sigBytes; i += 4)
                        {
                            this.words[rrr(this.sigBytes + i, 2)] = wordArray.words[rrr(i, 2)];
                        }
                    }
                    else
                    {
                        int s = this.words.Length;
                        Array.Resize<int>(ref this.words, this.words.Length + wordArray.words.Length);
                        wordArray.words.CopyTo(this.words, s);
                    }
                    this.sigBytes += wordArray.sigBytes;

                    return this;
                }

                public void Clamp()
                {
                    if (rrr(this.sigBytes, 2) + 1 > this.words.Length) Array.Resize<int>(ref this.words, rrr(this.sigBytes, 2) + 1);

                    this.words[rrr(this.sigBytes, 2)] &= (int)(0xffffffff << (32 - (this.sigBytes % 4) * 8));
                    Array.Resize<int>(ref this.words, (int)(this.sigBytes / 4));
                }
            }

            class Hex
            {
                public static WordArray Parse(string hexStr)
                {
                    int[] words = new int[hexStr.Length];
                    for (int i = 0; i < hexStr.Length; i += 2)
                    {
                        words[rrr(i, 3)] |= Convert.ToInt32(hexStr.Substring(i, 2), 16) << (24 - (i % 8) * 4);
                    }

                    return new WordArray(words, hexStr.Length / 2);
                }
            }

            class Base64
            {
                private static string map = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

                public static WordArray Parse(string base64Str)
                {
                    int base64StrLength = base64Str.Length;

                    char paddingChar = Base64.map[64];

                    int paddingIndex = base64Str.IndexOf(paddingChar);
                    if (paddingIndex != -1) base64StrLength = paddingIndex;


                    Dictionary<int, int> words = new Dictionary<int, int>();
                    int nBytes = 0;
                    for (int i = 0; i < base64StrLength; i++)
                    {
                        if (i % 4 != 0)
                        {
                            int bits1 = Base64.map.IndexOf(base64Str[i - 1]) << ((i % 4) * 2);
                            int bits2 = rrr(Base64.map.IndexOf(base64Str[i]), 6 - (i % 4) * 2);
                            if (!words.ContainsKey(rrr(nBytes, 2))) words[rrr(nBytes, 2)] = 0;
                            words[rrr(nBytes, 2)] |= (bits1 | bits2) << (24 - (nBytes % 4) * 8);
                            nBytes++;
                        }
                    }

                    int[] r = new int[words.Count];
                    words.Values.CopyTo(r, 0);
                    return new WordArray(r, nBytes);
                }
            }

            private WordArray key;
            private WordArray data;
            private int nDataBytes;
            private int blockSize = 128 / 32;
            private int[] iv;
            private int[] prevBlock;

            private Dictionary<int, int> SBOX = new Dictionary<int, int>();
            private Dictionary<int, int> INV_SBOX = new Dictionary<int, int>();
            private Dictionary<int, int> SUB_MIX_0 = new Dictionary<int, int>();
            private Dictionary<int, int> SUB_MIX_1 = new Dictionary<int, int>();
            private Dictionary<int, int> SUB_MIX_2 = new Dictionary<int, int>();
            private Dictionary<int, int> SUB_MIX_3 = new Dictionary<int, int>();
            private Dictionary<int, int> INV_SUB_MIX_0 = new Dictionary<int, int>();
            private Dictionary<int, int> INV_SUB_MIX_1 = new Dictionary<int, int>();
            private Dictionary<int, int> INV_SUB_MIX_2 = new Dictionary<int, int>();
            private Dictionary<int, int> INV_SUB_MIX_3 = new Dictionary<int, int>();
            private int[] RCON = new int[] { 0x00, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36 };
            private int nRounds;
            private int[] invKeySchedule;
            private int[] keySchedule;

            private void DoReset()
            {
                int keySize = this.key.sigBytes / 4;
                this.nRounds = keySize + 6;
                int ksRows = (this.nRounds + 1) * 4;

                this.keySchedule = new int[ksRows];
                for (int ksRow = 0; ksRow < ksRows; ksRow++)
                {
                    if (ksRow < keySize)
                    {
                        this.keySchedule[ksRow] = this.key.words[ksRow];
                    }
                    else
                    {
                        int t = this.keySchedule[ksRow - 1];

                        if (ksRow % keySize == 0)
                        {
                            t = (t << 8) | rrr(t, 24);
                            t = (this.SBOX[rrr(t, 24)] << 24) | (this.SBOX[rrr(t, 16) & 0xff] << 16) | (this.SBOX[rrr(t, 8) & 0xff] << 8) | this.SBOX[t & 0xff];
                            t ^= this.RCON[(ksRow / keySize) | 0] << 24;
                        }
                        else if (keySize > 6 && ksRow % keySize == 4)
                        {
                            t = (this.SBOX[rrr(t, 24)] << 24) | (this.SBOX[rrr(t, 16) & 0xff] << 16) | (this.SBOX[rrr(t, 8) & 0xff] << 8) | this.SBOX[t & 0xff];
                        }

                        this.keySchedule[ksRow] = this.keySchedule[ksRow - keySize] ^ t;
                    }
                }

                this.invKeySchedule = new int[ksRows];
                for (int invKsRow = 0; invKsRow < ksRows; invKsRow++)
                {
                    int ksRow = ksRows - invKsRow;
                    int t;

                    if (invKsRow % 4 != 0) t = this.keySchedule[ksRow];
                    else t = this.keySchedule[ksRow - 4];

                    if (invKsRow < 4 || ksRow <= 4)
                    {
                        this.invKeySchedule[invKsRow] = t;
                    }
                    else
                    {
                        this.invKeySchedule[invKsRow] = this.INV_SUB_MIX_0[this.SBOX[rrr(t, 24)]] ^ this.INV_SUB_MIX_1[this.SBOX[rrr(t, 16) & 0xff]] ^
                            this.INV_SUB_MIX_2[this.SBOX[rrr(t, 8) & 0xff]] ^ this.INV_SUB_MIX_3[this.SBOX[t & 0xff]];
                    }
                }
            }

            private WordArray Process()
            {
                int nBlocksReady = this.data.sigBytes / this.blockSize * 4;
                nBlocksReady = (int)Math.Ceiling((double)nBlocksReady);

                int nWordsReady = nBlocksReady * this.blockSize;

                int nBytesReady = Math.Min(nWordsReady * 4, this.data.sigBytes);
                int[] processedWords = null;

                if (nWordsReady != 0)
                {
                    for (int offset = 0; offset < nWordsReady; offset += this.blockSize)
                    {
                        this.ProcessBlock(ref this.data.words, offset);
                    }

                    processedWords = new int[nWordsReady];
                    this.data.words.CopyTo(processedWords, 0);
                    this.data.sigBytes -= nBytesReady;
                }

                return new WordArray(processedWords, nBytesReady);
            }

            private void ProcessBlock(ref int[] words, int offset)
            {
                int[] thisBlock = new int[this.blockSize];
                int size = this.blockSize;
                if (words.Length < offset + this.blockSize)
                {
                    size = words.Length - offset;
                    thisBlock = new int[size];
                }

                Array.Copy(words, offset, thisBlock, 0, size);

                this.DecryptBlock(ref words, offset);
                this.XorBlock(ref words, offset, this.blockSize);

                this.prevBlock = thisBlock;
            }

            private void XorBlock(ref int[] words, int offset, int blockSize)
            {
                int[] block;
                if (this.iv != null)
                {
                    block = this.iv;
                    this.iv = null;
                }
                else
                {
                    block = this.prevBlock;
                }

                if (block.Length < 4) Array.Resize<int>(ref block, 4);

                for (int i = 0; i < blockSize; i++)
                {
                    words[offset + i] ^= block[i];
                }
            }

            private void Pkcs7Unpad(WordArray data)
            {
                int nPaddingBytes = data.words[rrr(data.sigBytes - 1, 2)] & 0xff;
                data.sigBytes -= nPaddingBytes;
            }

            private void DecryptBlock(ref int[] m, int offset)
            {
                if (m.Length - 1 < offset + 1) Array.Resize<int>(ref m, offset + 1);
                if (m.Length - 1 < offset + 2) Array.Resize<int>(ref m, offset + 2);
                if (m.Length - 1 < offset + 3) Array.Resize<int>(ref m, offset + 3);
                if (m.Length - 1 < offset + 4) Array.Resize<int>(ref m, offset + 4);

                int t = m[offset + 1];
                m[offset + 1] = m[offset + 3];
                m[offset + 3] = t;

                this.DoCryptBlock(m, offset, this.invKeySchedule, this.INV_SUB_MIX_0, this.INV_SUB_MIX_1, this.INV_SUB_MIX_2, this.INV_SUB_MIX_3, this.INV_SBOX);

                t = m[offset + 1];
                m[offset + 1] = m[offset + 3];
                m[offset + 3] = t;
            }

            private void DoCryptBlock(int[] m, int offset, int[] keySchedule, Dictionary<int, int> SUB_MIX_0, Dictionary<int, int> SUB_MIX_1, Dictionary<int, int> SUB_MIX_2, Dictionary<int, int> SUB_MIX_3, Dictionary<int, int> SBOX)
            {
                int s0 = m[offset] ^ keySchedule[0];
                int s1 = m[offset + 1] ^ keySchedule[1];
                int s2 = m[offset + 2] ^ keySchedule[2];
                int s3 = m[offset + 3] ^ keySchedule[3];

                int ksRow = 4;

                for (int round = 1; round < this.nRounds; round++)
                {
                    int t10 = SUB_MIX_0[rrr(s0, 24)] ^ SUB_MIX_1[rrr(s1, 16) & 0xff] ^ SUB_MIX_2[rrr(s2, 8) & 0xff] ^ SUB_MIX_3[s3 & 0xff] ^ keySchedule[ksRow++];
                    int t11 = SUB_MIX_0[rrr(s1, 24)] ^ SUB_MIX_1[rrr(s2, 16) & 0xff] ^ SUB_MIX_2[rrr(s3, 8) & 0xff] ^ SUB_MIX_3[s0 & 0xff] ^ keySchedule[ksRow++];
                    int t12 = SUB_MIX_0[rrr(s2, 24)] ^ SUB_MIX_1[rrr(s3, 16) & 0xff] ^ SUB_MIX_2[rrr(s0, 8) & 0xff] ^ SUB_MIX_3[s1 & 0xff] ^ keySchedule[ksRow++];
                    int t13 = SUB_MIX_0[rrr(s3, 24)] ^ SUB_MIX_1[rrr(s0, 16) & 0xff] ^ SUB_MIX_2[rrr(s1, 8) & 0xff] ^ SUB_MIX_3[s2 & 0xff] ^ keySchedule[ksRow++];

                    s0 = t10;
                    s1 = t11;
                    s2 = t12;
                    s3 = t13;
                }

                int t0 = ((SBOX[rrr(s0, 24)] << 24) | (SBOX[rrr(s1, 16) & 0xff] << 16) | (SBOX[rrr(s2, 8) & 0xff] << 8) | SBOX[s3 & 0xff]) ^ keySchedule[ksRow++];
                int t1 = ((SBOX[rrr(s1, 24)] << 24) | (SBOX[rrr(s2, 16) & 0xff] << 16) | (SBOX[rrr(s3, 8) & 0xff] << 8) | SBOX[s0 & 0xff]) ^ keySchedule[ksRow++];
                int t2 = ((SBOX[rrr(s2, 24)] << 24) | (SBOX[rrr(s3, 16) & 0xff] << 16) | (SBOX[rrr(s0, 8) & 0xff] << 8) | SBOX[s1 & 0xff]) ^ keySchedule[ksRow++];
                int t3 = ((SBOX[rrr(s3, 24)] << 24) | (SBOX[rrr(s0, 16) & 0xff] << 16) | (SBOX[rrr(s1, 8) & 0xff] << 8) | SBOX[s2 & 0xff]) ^ keySchedule[ksRow++];

                m[offset] = t0;
                m[offset + 1] = t1;
                m[offset + 2] = t2;
                m[offset + 3] = t3;
            }

            public static string Decrypt(string text, string key)
            {
                return new AES().Decrypt(Base64.Parse(text), Base64.Parse(key)).ToString();
            }

            private WordArray Decrypt(WordArray text, WordArray key)
            {
                this.key = key;
                this.data = new WordArray();
                this.nDataBytes = 0;

                this.DoReset();

                WordArray iv = Hex.Parse("0000000000000000");
                this.iv = iv.words;

                this.data.concat(text);
                this.nDataBytes += text.sigBytes;

                WordArray finalProcessedBlocks = this.Process();

                this.Pkcs7Unpad(finalProcessedBlocks);

                return finalProcessedBlocks;
            }

            public AES()
            {
                int[] d = new int[256];
                for (int i = 0; i < 256; i++)
                {
                    if (i < 128)
                    {
                        d[i] = i << 1;
                    }
                    else
                    {
                        d[i] = (i << 1) ^ 0x11b;
                    }
                }

                int x = 0;
                int xi = 0;
                for (int i = 0; i < 256; i++)
                {
                    int sx = xi ^ (xi << 1) ^ (xi << 2) ^ (xi << 3) ^ (xi << 4);
                    sx = rrr(sx, 8) ^ (sx & 0xff) ^ 0x63;
                    this.SBOX[x] = sx;
                    this.INV_SBOX[sx] = x;

                    int x2 = d[x];
                    int x4 = d[x2];
                    int x8 = d[x4];

                    int t = (d[sx] * 0x101) ^ (sx * 0x1010100);
                    this.SUB_MIX_0[x] = (t << 24) | rrr(t, 8);
                    this.SUB_MIX_1[x] = (t << 16) | rrr(t, 16);
                    this.SUB_MIX_2[x] = (t << 8) | rrr(t, 24);
                    this.SUB_MIX_3[x] = t;

                    t = (x8 * 0x1010101) ^ (x4 * 0x10001) ^ (x2 * 0x101) ^ (x * 0x1010100);
                    this.INV_SUB_MIX_0[sx] = (t << 24) | rrr(t, 8);
                    this.INV_SUB_MIX_1[sx] = (t << 16) | rrr(t, 16);
                    this.INV_SUB_MIX_2[sx] = (t << 8) | rrr(t, 24);
                    this.INV_SUB_MIX_3[sx] = t;

                    if (x == 0)
                    {
                        x = xi = 1;
                    }
                    else
                    {
                        x = x2 ^ d[d[d[x8 ^ x2]]];
                        xi ^= d[d[xi]];
                    }
                }
            }
        }*/

        class RSAKey
        {
            class Biginteger : Dictionary<int, int>
            {
                class Classic
                {
                    public Biginteger m;

                    public virtual Biginteger Convert(Biginteger x)
                    {
                        if (x.s < 0 || x.CompareTo(this.m) >= 0) return x.Mod(this.m);
                        else return x;
                    }

                    public virtual Biginteger Revert(Biginteger x)
                    {
                        return x;
                    }

                    public virtual void Reduce(Biginteger x)
                    {
                        x.DivRemTo(this.m, null, x);
                    }

                    public virtual void MulTo(Biginteger x, Biginteger y, Biginteger r)
                    {
                        x.MultiplyTo(y, r);
                        this.Reduce(r);
                    }

                    public virtual void SqrTo(Biginteger x, Biginteger r)
                    {
                        x.SquareTo(r);
                        this.Reduce(r);
                    }

                    public Classic(Biginteger m)
                    {
                        this.m = m;
                    }
                }

                class Montgomery : Classic
                {
                    private int mp;
                    private int mpl;
                    private int mph;
                    private int um;
                    private int mt2;

                    // xR mod m
                    public override Biginteger Convert(Biginteger x)
                    {
                        Biginteger r = new Biginteger();
                        x.Abs().DlShiftTo(this.m.t, r);
                        r.DivRemTo(this.m, null, r);
                        if (x.s < 0 && r.CompareTo(Biginteger.ZERO) > 0) this.m.SubTo(r, r);
                        return r;
                    }

                    // x/R mod m
                    public override Biginteger Revert(Biginteger x)
                    {
                        Biginteger r = new Biginteger();
                        x.CopyTo(r);
                        this.Reduce(r);
                        return r;
                    }

                    // x = x/R mod m (HAC 14.32)
                    public override void Reduce(Biginteger x)
                    {
                        while (x.t <= this.mt2) // pad x so am has enough room later
                            x[x.t++] = 0;
                        for (int i = 0; i < this.m.t; ++i)
                        {
                            // faster way of calculating u0 = x[i]*mp mod DV
                            int j = x[i] & 0x7fff;
                            int u0 = (j * this.mpl + (((j * this.mph + (x[i] >> 15) * this.mpl) & this.um) << 15)) & x.DM;
                            // use am to combine the multiply-shift-add into one call
                            j = i + this.m.t;
                            x[j] += this.m.Am(0, u0, x, i, 0, this.m.t);
                            // propagate carry
                            while (x[j] >= x.DV) { x[j] -= x.DV; x[++j]++; }
                        }
                        x.Clamp();
                        x.DrShiftTo(this.m.t, x);
                        if (x.CompareTo(this.m) >= 0) x.SubTo(this.m, x);
                    }

                    public Montgomery(Biginteger m) : base(m)
                    {
                        this.mp = m.InvDigit();
                        this.mpl = this.mp & 0x7fff;
                        this.mph = this.mp >> 15;
                        this.um = (1 << (m.DB - 15)) - 1;
                        this.mt2 = 2 * m.t;
                    }
                }

                private static string BI_RM = "0123456789abcdefghijklmnopqrstuvwxyz";
                private static Dictionary<int, int> BI_RC = new Dictionary<int, int>();

                // JavaScript engine analysis
                private static long canary = 0xdeadbeefcafe;
                private static bool j_lm = ((Biginteger.canary & 0xffffff) == 0xefcafe);

                // Bits per digit
                private static int dbits = 28;

                private static int[] lowprimes = new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541, 547, 557, 563, 569, 571, 577, 587, 593, 599, 601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659, 661, 673, 677, 683, 691, 701, 709, 719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797, 809, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887, 907, 911, 919, 929, 937, 941, 947, 953, 967, 971, 977, 983, 991, 997 };
                private static int lplim = (1 << 26) / Biginteger.lowprimes[Biginteger.lowprimes.Length - 1];

                static Biginteger()
                {
                    int rr = (int)'0';
                    for (int vv = 0; vv <= 9; ++vv)
                    {
                        Biginteger.BI_RC[rr++] = vv;
                    }
                    rr = (int)'a';
                    for (int vv = 10; vv < 36; ++vv)
                    {
                        Biginteger.BI_RC[rr++] = vv;
                    }
                    rr = (int)'A';
                    for (int vv = 10; vv < 36; ++vv)
                    {
                        Biginteger.BI_RC[rr++] = vv;
                    }
                }

                public static Biginteger ZERO = Biginteger.Fromint(0);
                public static Biginteger ONE = Biginteger.Fromint(1);

                public int DV
                {
                    get
                    {
                        return (1 << Biginteger.dbits);
                    }
                }

                public int DB
                {
                    get
                    {
                        return Biginteger.dbits;
                    }
                }

                public int DM
                {
                    get
                    {
                        return ((1 << Biginteger.dbits) - 1);
                    }
                }

                private int BI_FP = 52;
                private double FV
                {
                    get
                    {
                        return Math.Pow(2, this.BI_FP);
                    }
                }

                private int F1
                {
                    get
                    {
                        return this.BI_FP - Biginteger.dbits;
                    }
                }

                private int F2
                {
                    get
                    {
                        return 2 * Biginteger.dbits - this.BI_FP;
                    }
                }

                public int t;
                public int s;

                public int Am(int i, int x, Biginteger w, int j, int c, int n)
                {
                    int xl = x & 0x3fff;
                    int xh = x >> 14;

                    while (--n >= 0)
                    {
                        int l = this[i] & 0x3fff;
                        int h = this[i++] >> 14;
                        int m = xh * l + h * xl;
                        l = xl * l + ((m & 0x3fff) << 14) + w[j] + c;
                        c = (l >> 28) + (m >> 14) + xh * h;
                        w[j++] = l & 0xfffffff;
                    }

                    return c;
                }

                public static char Int2char(int n)
                {
                    return Biginteger.BI_RM[n];
                }

                private int IntAt(string s, int i)
                {
                    return Biginteger.BI_RC[(int)s[i]];
                }

                // (protected) copy this to r
                public void CopyTo(Biginteger r)
                {
                    for (int i = this.t - 1; i >= 0; --i)
                    {
                        r[i] = this[i];
                    }
                    r.t = this.t;
                    r.s = this.s;
                }

                public static Biginteger Fromint(int x)
                {
                    Biginteger r = new Biginteger();
                    r.t = 1;
                    r.s = (x < 0) ? -1 : 0;
                    if (x > 0) r[0] = x;
                    else if (x < -1) r[0] = x + r.DV;
                    else r.t = 0;

                    return r;
                }

                // (protected) set from string and radix
                public static Biginteger FromString(string s, int b = 256)
                {
                    Biginteger r = new Biginteger();
                    int k;

                    if (b == 16) k = 4;
                    else if (b == 8) k = 3;
                    else if (b == 256) k = 8; // byte array
                    else if (b == 2) k = 1;
                    else if (b == 32) k = 5;
                    else if (b == 4) k = 2;
                    else return Biginteger.FromRadix(s, b);

                    r.t = 0;
                    r.s = 0;

                    int i = s.Length;
                    bool mi = false;
                    int sh = 0;

                    while (--i >= 0)
                    {
                        int x = (k == 8) ? int.Parse(s[i].ToString()) & 0xff : r.IntAt(s, i);
                        if (x < 0)
                        {
                            if (s[i] == '-') mi = true;
                            continue;
                        }
                        mi = false;
                        if (sh == 0)
                            r[r.t++] = x;
                        else if (sh + k > r.DB)
                        {
                            r[r.t - 1] |= (x & ((1 << (r.DB - sh)) - 1)) << sh;
                            r[r.t++] = (x >> (r.DB - sh));
                        }
                        else
                            r[r.t - 1] |= x << sh;
                        sh += k;
                        if (sh >= r.DB) sh -= r.DB;
                    }
                    if (k == 8 && (int.Parse(s[0].ToString()) & 0x80) != 0)
                    {
                        r.s = -1;
                        if (sh > 0) r[r.t - 1] |= ((1 << (r.DB - sh)) - 1) << sh;
                    }
                    r.Clamp();
                    if (mi) Biginteger.ZERO.SubTo(r, r);

                    return r;
                }

                // (protected) clamp off excess high words
                public void Clamp()
                {
                    int c = this.s & this.DM;
                    while (this.t > 0 && this[this.t - 1] == c)
                    {
                        --this.t;
                    }
                }

                // (public) return string representation in given radix
                public string ToString(int radix)
                {
                    if (this.s < 0) return "-" + this.Negate().ToString(radix);

                    int k;
                    if (radix == 2) k = 1;
                    else if (radix == 4) k = 2;
                    else if (radix == 8) k = 3;
                    else if (radix == 16) k = 4;
                    else if (radix == 32) k = 5;
                    else return "";

                    int km = (1 << k) - 1;
                    int d;
                    bool m = false;
                    string r = "";
                    int i = this.t;
                    int p = this.DB - (i * this.DB) % k;

                    if (i-- > 0)
                    {
                        if (p < this.DB && (d = this[i] >> p) > 0)
                        {
                            m = true;
                            r = Biginteger.Int2char(d).ToString();
                        }
                        while (i >= 0)
                        {
                            if (p < k)
                            {
                                d = (this[i] & ((1 << p) - 1)) << (k - p);
                                d |= this[--i] >> (p += this.DB - k);
                            }
                            else
                            {
                                d = (this[i] >> (p -= k)) & km;
                                if (p <= 0)
                                {
                                    p += this.DB;
                                    --i;
                                }
                            }
                            if (d > 0) m = true;
                            if (m) r += Biginteger.Int2char(d);
                        }
                    }

                    return m ? r : "0";
                }

                private Biginteger Negate()
                {
                    Biginteger r = new Biginteger();
                    Biginteger.ZERO.SubTo(this, r);
                    return r;
                }

                // (public) |this|
                public Biginteger Abs()
                {
                    return (this.s < 0) ? this.Negate() : this;
                }

                // (public) return + if this > a, - if this < a, 0 if equal
                public int CompareTo(Biginteger a)
                {
                    int r = this.s - a.s;
                    if (r != 0) return r;
                    int i = this.t;
                    r = i - a.t;

                    if (r != 0)
                    {
                        return (this.s < 0) ? -r : r;
                    }

                    while (--i >= 0)
                    {
                        if ((r = this[i] - a[i]) != 0)
                        {
                            return r;
                        }
                    }
                    return 0;
                }

                // returns bit length of the integer x
                private int Nbits(int x)
                {
                    int r = 1;
                    int t;

                    if ((t = (x >> 16)) != 0)
                    {
                        x = t;
                        r += 16;
                    }
                    if ((t = x >> 8) != 0)
                    {
                        x = t;
                        r += 8;
                    }
                    if ((t = x >> 4) != 0)
                    {
                        x = t;
                        r += 4;
                    }
                    if ((t = x >> 2) != 0)
                    {
                        x = t;
                        r += 2;
                    }
                    if ((t = x >> 1) != 0)
                    {
                        x = t;
                        r += 1;
                    }
                    return r;
                }

                // (protected) r = this << n*DB
                public void DlShiftTo(int n, Biginteger r)
                {
                    for (int i = this.t - 1; i >= 0; --i)
                    {
                        r[i + n] = this[i];
                    }
                    for (int i = n - 1; i >= 0; --i)
                    {
                        r[i] = 0;
                    }

                    r.t = this.t + n;
                    r.s = this.s;
                }

                // (protected) r = this >> n*DB
                public void DrShiftTo(int n, Biginteger r)
                {
                    for (int i = n; i < this.t; ++i)
                    {
                        r[i - n] = this[i];
                    }

                    r.t = Math.Max(this.t - n, 0);
                    r.s = this.s;
                }

                // (protected) r = this << n
                private void LShiftTo(int n, Biginteger r)
                {
                    int bs = n % this.DB;
                    int cbs = this.DB - bs;
                    int bm = (1 << cbs) - 1;
                    int ds = (int)(n / this.DB);
                    int c = (this.s << bs) & this.DM;

                    for (int i = this.t - 1; i >= 0; --i)
                    {
                        r[i + ds + 1] = (this[i] >> cbs) | c;
                        c = (this[i] & bm) << bs;
                    }
                    for (int i = ds - 1; i >= 0; --i)
                    {
                        r[i] = 0;
                    }
                    r[ds] = c;
                    r.t = this.t + ds + 1;
                    r.s = this.s;
                    r.Clamp();
                }

                // (protected) r = this >> n
                private void RShiftTo(int n, Biginteger r)
                {
                    r.s = this.s;
                    int ds = (int)(n / this.DB);
                    if (ds >= this.t)
                    {
                        r.t = 0;
                        return;
                    }

                    int bs = n % this.DB;
                    int cbs = this.DB - bs;
                    int bm = (1 << bs) - 1;
                    r[0] = this[ds] >> bs;

                    for (int i = ds + 1; i < this.t; ++i)
                    {
                        r[i - ds - 1] |= (this[i] & bm) << cbs;
                        r[i - ds] = this[i] >> bs;
                    }
                    if (bs > 0)
                    {
                        r[this.t - ds - 1] |= (this.s & bm) << cbs;
                    }
                    r.t = this.t - ds;
                    r.Clamp();
                }

                // (protected) r = this - a
                public void SubTo(Biginteger a, Biginteger r)
                {
                    int i = 0;
                    int c = 0;
                    int m = Math.Min(a.t, this.t);

                    while (i < m)
                    {
                        c += this[i] - a[i];
                        r[i++] = c & this.DM;
                        c >>= this.DB;
                    }

                    if (a.t < this.t)
                    {
                        c -= a.s;
                        while (i < this.t)
                        {
                            c += this[i];
                            r[i++] = c & this.DM;
                            c >>= this.DB;
                        }
                        c += this.s;
                    }
                    else
                    {
                        c += this.s;
                        while (i < a.t)
                        {
                            c -= a[i];
                            r[i++] = c & this.DM;
                            c >>= this.DB;
                        }
                        c -= a.s;
                    }
                    r.s = (c < 0) ? -1 : 0;
                    if (c < -1) r[i++] = this.DV + c;
                    else if (c > 0) r[i++] = c;
                    r.t = i;
                    r.Clamp();
                }

                // (protected) r = this * a, r != this,a (HAC 14.12)
                // "this" should be the larger one if appropriate.
                public void MultiplyTo(Biginteger a, Biginteger r)
                {
                    Biginteger x = this.Abs();
                    Biginteger y = a.Abs();
                    int j = x.t;
                    r.t = j + y.t;
                    while (--j >= 0)
                    {
                        r[j] = 0;
                    }
                    for (int i = 0; i < y.t; ++i)
                    {
                        r[i + x.t] = x.Am(0, y[i], r, i, 0, x.t);
                    }

                    r.s = 0;
                    r.Clamp();
                    if (this.s != a.s) Biginteger.ZERO.SubTo(r, r);
                }

                // (protected) r = this^2, r != this (HAC 14.16)
                public void SquareTo(Biginteger r)
                {
                    Biginteger x = this.Abs();
                    int i = r.t = 2 * x.t;
                    while (--i >= 0)
                    {
                        r[i] = 0;
                    }
                    for (i = 0; i < x.t - 1; ++i)
                    {
                        int c = x.Am(i, x[i], r, 2 * i, 0, 1);
                        if ((r[i + x.t] += x.Am(i + 1, 2 * x[i], r, 2 * i + 1, c, x.t - i - 1)) >= x.DV)
                        {
                            r[i + x.t] -= x.DV;
                            r[i + x.t + 1] = 1;
                        }
                    }

                    if (r.t > 0) r[r.t - 1] += x.Am(i, x[i], r, 2 * i, 0, 1);
                    r.s = 0;
                    r.Clamp();
                }

                // (protected) divide this by m, quotient and remainder to q, r (HAC 14.20)
                // r != q, this != m.  q or r may be null.
                public void DivRemTo(Biginteger m, Biginteger q, Biginteger r)
                {
                    Biginteger pm = m.Abs();
                    if (pm.t <= 0) return;

                    Biginteger pt = this.Abs();
                    if (pt.t < pm.t)
                    {
                        if (q != null) Biginteger.Fromint(0);
                        if (r != null) this.CopyTo(r);
                        return;
                    }

                    if (r == null) r = new Biginteger();

                    Biginteger y = new Biginteger();
                    int ts = this.s;
                    int ms = m.s;

                    int nsh = this.DB - this.Nbits(pm[pm.t - 1]);   // normalize modulus
                    if (nsh > 0)
                    {
                        pm.LShiftTo(nsh, y);
                        pt.LShiftTo(nsh, r);
                    }
                    else
                    {
                        pm.CopyTo(y);
                        pt.CopyTo(r);
                    }

                    int ys = y.t;

                    double y0 = y[ys - 1];
                    if (y0 == 0) return;

                    double yt = y0 * (1 << this.F1) + ((ys > 1) ? y[ys - 2] >> this.F2 : 0);
                    double d1 = this.FV / yt;
                    double d2 = (1 << this.F1) / yt;
                    double e = 1 << this.F2;
                    int i = r.t;
                    int j = i - ys;
                    Biginteger t = (q == null) ? new Biginteger() : q;

                    y.DlShiftTo(j, t);
                    if (r.CompareTo(t) >= 0)
                    {
                        r[r.t++] = 1;
                        r.SubTo(t, r);
                    }
                    Biginteger.ONE.DlShiftTo(ys, t);
                    t.SubTo(y, y);  // "negative" y so we can replace sub with am later
                    while (y.t < ys)
                    {
                        y[y.t++] = 0;
                    }
                    while (--j >= 0)
                    {
                        // Estimate quotient digit
                        int qd = (r[--i] == y0) ? this.DM : (int)(r[i] * d1 + (r[i - 1] + e) * d2);
                        if ((r[i] += y.Am(0, qd, r, j, 0, ys)) < qd)
                        {   // Try it out
                            y.DlShiftTo(j, t);
                            r.SubTo(t, r);
                            while (r[i] < --qd) r.SubTo(t, r);
                        }
                    }
                    if (q != null)
                    {
                        r.DrShiftTo(ys, q);
                        if (ts != ms) Biginteger.ZERO.SubTo(q, q);
                    }
                    r.t = ys;
                    r.Clamp();
                    if (nsh > 0) r.RShiftTo(nsh, r);    // Denormalize remainder
                    if (ts < 0) Biginteger.ZERO.SubTo(r, r);
                }

                // (public) this mod a
                public Biginteger Mod(Biginteger a)
                {
                    Biginteger r = new Biginteger();
                    this.Abs().DivRemTo(a, null, r);
                    if (this.s < 0 && r.CompareTo(Biginteger.ZERO) > 0) a.SubTo(r, r);
                    return r;
                }

                // (protected) return "-1/this % 2^DB"; useful for Mont. reduction
                // justification:
                //         xy == 1 (mod m)
                //         xy =  1+km
                //   xy(2-xy) = (1+km)(1-km)
                // x[y(2-xy)] = 1-k^2m^2
                // x[y(2-xy)] == 1 (mod m^2)
                // if y is 1/x mod m, then y(2-xy) is 1/x mod m^2
                // should reduce x and y(2-xy) by m^2 at each step to keep size bounded.
                // JS multiply "overflows" differently from C/C++, so care is needed here.
                public int InvDigit()
                {
                    if (this.t < 1) return 0;
                    int x = this[0];
                    if ((x & 1) == 0) return 0;
                    int y = x & 3;      // y == 1/x mod 2^2
                    y = (y * (2 - (x & 0xf) * y)) & 0xf;    // y == 1/x mod 2^4
                    y = (y * (2 - (x & 0xff) * y)) & 0xff;  // y == 1/x mod 2^8
                    y = (y * (2 - (((x & 0xffff) * y) & 0xffff))) & 0xffff; // y == 1/x mod 2^16
                                                                            // last step - calculate inverse mod DV directly;
                                                                            // assumes 16 < DB <= 32 and assumes ability to handle 48-bit ints
                    y = (y * (2 - x * y % this.DV)) % this.DV;      // y == 1/x mod 2^dbits
                                                                    // we really want the negative inverse, and -DV < y < DV
                    return (y > 0) ? this.DV - y : -y;
                }

                // (protected) true iff this is even
                private bool IsEven()
                {
                    return ((this.t > 0) ? (this[0] & 1) : this.s) == 0;
                }

                // (protected) this^e, e < 2^32, doing sqr and mul with "r" (HAC 14.79)
                private Biginteger Exp(int e, Classic z)
                {
                    if ((uint)e > 0xffffffff || e < 1) return Biginteger.ONE;
                    Biginteger r = new Biginteger();
                    Biginteger r2 = new Biginteger();
                    Biginteger g = z.Convert(this);
                    int i = this.Nbits(e) - 1;

                    g.CopyTo(r);
                    while (--i >= 0)
                    {
                        z.SqrTo(r, r2);
                        if ((e & (1 << i)) > 0)
                        {
                            z.MulTo(r2, g, r);
                        }
                        else
                        {
                            Biginteger t = r;
                            r = r2;
                            r2 = t;
                        }
                    }
                    return z.Revert(r);
                }

                // (public) this^e % m, 0 <= e < 2^32
                public Biginteger ModPowint(int e, Biginteger m)
                {
                    Classic z;
                    if (e < 256 || m.IsEven()) z = new Classic(m);
                    else z = new Montgomery(m);
                    return this.Exp(e, z);
                }

                // (protected) return x s.t. r^x < DV
                private int ChunkSize(int r)
                {
                    return (int)(0.6931471805599453 * this.DB / Math.Log(r));
                }

                // (public) 0 if this == 0, 1 if this > 0
                private int Signum()
                {
                    if (this.s < 0) return -1;
                    else if (this.t <= 0 || (this.t == 1 && this[0] <= 0)) return 0;
                    else return 1;
                }

                // (protected) convert from radix string
                public static Biginteger FromRadix(string s, int b)
                {
                    Biginteger r = Biginteger.Fromint(0);
                    int cs = r.ChunkSize(b);
                    int d = (int)Math.Pow(b, cs);
                    bool mi = false;
                    int j = 0;
                    int w = 0;

                    for (int i = 0; i < s.Length; ++i)
                    {
                        int x = r.IntAt(s, i);
                        if (x < 0)
                        {
                            if (s[i] == '-' && r.Signum() == 0) mi = true;
                            continue;
                        }
                        w = b * w + x;
                        if (++j >= cs)
                        {
                            r.Multiply2(d);
                            r.AddOffset2(w, 0);
                            j = 0;
                            w = 0;
                        }
                    }
                    if (j > 0)
                    {
                        r.Multiply2((int)Math.Pow(b, j));
                        r.AddOffset2(w, 0);
                    }
                    if (mi) Biginteger.ZERO.SubTo(r, r);

                    return r;
                }

                // (protected) this *= n, this >= 0, 1 < n < DV
                private void Multiply2(int n)
                {
                    this[this.t] = this.Am(0, n - 1, this, 0, 0, this.t);
                    ++this.t;
                    this.Clamp();
                }

                // (protected) this += n << w words, this >= 0
                public void AddOffset2(int n, int w)
                {
                    if (n == 0) return;
                    while (this.t <= w)
                    {
                        this[this.t++] = 0;
                    }
                    this[w] += n;
                    while (this[w] >= this.DV)
                    {
                        this[w] -= this.DV;
                        if (++w >= this.t) this[this.t++] = 0;
                        ++this[w];
                    }
                }
            }

            private Biginteger n;
            private int e = 65537;

            public bool VerifyString(string message, string signature)
            {
                signature = this.Base64toHex(signature);

                Biginteger biSignature = this.ParseBigint(signature, 16);
                Biginteger biDecryptedSignature = this.DoPublic(biSignature);
                string digestInfo = Regex.Replace(biDecryptedSignature.ToString(16), "^1f+00", "");

                if (digestInfo.Substring(0, SHA1.signature.Length) != SHA1.signature) return false;

                string digestHash = digestInfo.Substring(SHA1.signature.Length);
                string messageHash = SHA1.Hex(message/*.toUnicodeString()*/);

                return digestHash == messageHash;
            }

            private string Base64toHex(string s)
            {
                string b64map = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
                char b64pad = '=';
                string ret = "";
                int k = 0;
                int slop = 0;
                for (int i = 0; i < s.Length; ++i)
                {
                    if (s[i] == b64pad) break;
                    int v = b64map.IndexOf(s[i]);
                    if (v < 0) continue;
                    if (k == 0)
                    {
                        ret += Biginteger.Int2char(v >> 2);
                        slop = v & 3;
                        k = 1;
                    }
                    else if (k == 1)
                    {
                        ret += Biginteger.Int2char((slop << 2) | (v >> 4));
                        slop = v & 0xf;
                        k = 2;
                    }
                    else if (k == 2)
                    {
                        ret += Biginteger.Int2char(slop);
                        ret += Biginteger.Int2char(v >> 2);
                        slop = v & 3;
                        k = 3;
                    }
                    else
                    {
                        ret += Biginteger.Int2char((slop << 2) | (v >> 4));
                        ret += Biginteger.Int2char(v & 0xf);
                        k = 0;
                    }
                }
                if (k == 1)
                    ret += Biginteger.Int2char(slop << 2);
                return ret;
            }

            // convert a (hex) string to a bignum object
            private Biginteger ParseBigint(string str, int r)
            {
                return Biginteger.FromString(str, r);
            }

            // Set the public key fields N and e from hex strings
            public void SetPublic(string n, string e)
            {
                n = this.Base64toHex(n);

                if (n != null && e != null && n.Length > 0 && e.Length > 0)
                {
                    this.n = this.ParseBigint(n, 16);
                    this.e = Convert.ToInt32(e, 16);
                }
            }

            // Perform raw public operation on "x": return x^e (mod n)
            private Biginteger DoPublic(Biginteger x)
            {
                return x.ModPowint(this.e, this.n);
            }

            public RSAKey()
            {
                this.n = null;
                this.e = 0;
            }
        }
    }
}