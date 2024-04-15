using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Crypt
{
    public class AES
    {
        private WordArray key;
        private WordArray data;
        private int blockSize = 128 / 32;
        private List<int> iv;
        private List<int> prevBlock;

        private int[] SBOX = new int[256];
        private int[] INV_SBOX = new int[256];
        private int[] SUB_MIX_0 = new int[256];
        private int[] SUB_MIX_1 = new int[256];
        private int[] SUB_MIX_2 = new int[256];
        private int[] SUB_MIX_3 = new int[256];
        private int[] INV_SUB_MIX_0 = new int[256];
        private int[] INV_SUB_MIX_1 = new int[256];
        private int[] INV_SUB_MIX_2 = new int[256];
        private int[] INV_SUB_MIX_3 = new int[256];
        private int[] RCON = new int[] { 0x00, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36 };
        private int nRounds;
        private int[] invKeySchedule;
        private int[] keySchedule;

        private void DoReset()
        {
            var keySize = key.sigBytes / 4;
            nRounds = keySize + 6;
            var ksRows = (nRounds + 1) * 4;

            keySchedule = new int[ksRows];
            for (var ksRow = 0; ksRow < ksRows; ksRow++)
            {
                if (ksRow < keySize)
                {
                    keySchedule[ksRow] = key.words[ksRow];
                }
                else
                {
                    var t = keySchedule[ksRow - 1];

                    if (ksRow % keySize == 0)
                    {
                        t = (t << 8) | (int)((uint)t >> 24);
                        t = (SBOX[(int)((uint)t >> 24)] << 24) | (SBOX[(int)((uint)t >> 16) & 0xff] << 16) | (SBOX[(int)((uint)t >> 8) & 0xff] << 8) | SBOX[t & 0xff];
                        t ^= RCON[(ksRow / keySize) | 0] << 24;
                    }
                    else if (keySize > 6 && ksRow % keySize == 4)
                    {
                        t = (SBOX[(int)((uint)t >> 24)] << 24) | (SBOX[(int)((uint)t >> 16) & 0xff] << 16) | (SBOX[(int)((uint)t >> 8) & 0xff] << 8) | SBOX[t & 0xff];
                    }

                    keySchedule[ksRow] = keySchedule[ksRow - keySize] ^ t;
                }
            }

            invKeySchedule = new int[ksRows];
            for (var invKsRow = 0; invKsRow < ksRows; invKsRow++)
            {
                var ksRow = ksRows - invKsRow;
                int t;

                if (invKsRow % 4 != 0) t = keySchedule[ksRow];
                else t = keySchedule[ksRow - 4];

                if (invKsRow < 4 || ksRow <= 4)
                {
                    invKeySchedule[invKsRow] = t;
                }
                else
                {
                    invKeySchedule[invKsRow] = INV_SUB_MIX_0[SBOX[(int)((uint)t >> 24)]] ^ INV_SUB_MIX_1[SBOX[(int)((uint)t >> 16) & 0xff]] ^
                        INV_SUB_MIX_2[SBOX[(int)((uint)t >> 8) & 0xff]] ^ INV_SUB_MIX_3[SBOX[t & 0xff]];
                }
            }
        }

        private WordArray Process()
        {
            var nBlocksReadyD = (double)data.sigBytes / blockSize;
            var nBlocksReady = (int)Math.Ceiling(nBlocksReadyD);

            var nWordsReady = nBlocksReady * blockSize;

            var nBytesReady = Math.Min(nWordsReady * 4, data.sigBytes);

            nWordsReady = nBlocksReady;

            List<int> processedWords = null;

            if (nBlocksReady != 0)
            {
                for (var offset = 0; offset < nWordsReady; offset += blockSize)
                {
                    ProcessBlock(data.words, offset);
                }

                processedWords = data.words.GetRange(0, nWordsReady);
                data.words.RemoveRange(0, nWordsReady);
                data.sigBytes -= nBytesReady;
            }

            return new WordArray(processedWords, nBytesReady);
        }

        private void ProcessBlock(List<int> words, int offset)
        { 
            var size = blockSize;
            if (words.Count < offset + blockSize) size = words.Count - offset;

            var thisBlock = words.GetRange(offset, size);

            DecryptBlock(words, offset);
            XorBlock(words, offset, blockSize);

            prevBlock = thisBlock;
        }

        private void XorBlock(List<int> words, int offset, int blockSize)
        {
            List<int> block;
            if (iv != null)
            {
                block = iv;
                iv = null;
            }
            else
            {
                block = prevBlock;
            }

            for (var i = 0; i < blockSize; i++)
            {
                if (i < block.Count)
                    words[offset + i] ^= block[i];
            }
        }

        private void Pkcs7Unpad(WordArray data)
        {
            var nPaddingBytes = data.words[(data.sigBytes - 1) >> 2] & 0xff;
            data.sigBytes -= nPaddingBytes;
        }

        private void DecryptBlock(List<int> M, int offset)
        {
            var t = M[offset + 1];
            M[offset + 1] = M[offset + 3];
            M[offset + 3] = t;

            DoCryptBlock(M, offset, invKeySchedule, INV_SUB_MIX_0, INV_SUB_MIX_1, INV_SUB_MIX_2, INV_SUB_MIX_3, INV_SBOX);

            t = M[offset + 1];
            M[offset + 1] = M[offset + 3];
            M[offset + 3] = t;
        }

        private void DoCryptBlock(List<int> M, int offset, int[] keySchedule, int[] SUB_MIX_0, int[] SUB_MIX_1, int[] SUB_MIX_2, int[] SUB_MIX_3, int[] SBOX)
        {
            var s0 = M[offset] ^ keySchedule[0];
            var s1 = M[offset + 1] ^ keySchedule[1];
            var s2 = M[offset + 2] ^ keySchedule[2];
            var s3 = M[offset + 3] ^ keySchedule[3];

            var ksRow = 4;

            for (var round = 1; round < nRounds; round++)
            {
                var tt0 = SUB_MIX_0[(int)((uint)s0 >> 24)] ^ SUB_MIX_1[((int)((uint)s1 >> 16)) & 0xff] ^ SUB_MIX_2[(int)((uint)s2 >> 8) & 0xff] ^ SUB_MIX_3[s3 & 0xff] ^ keySchedule[ksRow++];
                var tt1 = SUB_MIX_0[(int)((uint)s1 >> 24)] ^ SUB_MIX_1[((int)((uint)s2 >> 16)) & 0xff] ^ SUB_MIX_2[(int)((uint)s3 >> 8) & 0xff] ^ SUB_MIX_3[s0 & 0xff] ^ keySchedule[ksRow++];
                var tt2 = SUB_MIX_0[(int)((uint)s2 >> 24)] ^ SUB_MIX_1[((int)((uint)s3 >> 16)) & 0xff] ^ SUB_MIX_2[(int)((uint)s0 >> 8) & 0xff] ^ SUB_MIX_3[s1 & 0xff] ^ keySchedule[ksRow++];
                var tt3 = SUB_MIX_0[(int)((uint)s3 >> 24)] ^ SUB_MIX_1[((int)((uint)s0 >> 16)) & 0xff] ^ SUB_MIX_2[(int)((uint)s1 >> 8) & 0xff] ^ SUB_MIX_3[s2 & 0xff] ^ keySchedule[ksRow++];

                s0 = tt0;
                s1 = tt1;
                s2 = tt2;
                s3 = tt3;
            }

            var t0 = ((SBOX[(int)((uint)s0 >> 24)] << 24) | (SBOX[(int)((uint)s1 >> 16) & 0xff] << 16) | (SBOX[(int)((uint)s2 >> 8) & 0xff] << 8) | SBOX[s3 & 0xff]) ^ keySchedule[ksRow++];
            var t1 = ((SBOX[(int)((uint)s1 >> 24)] << 24) | (SBOX[(int)((uint)s2 >> 16) & 0xff] << 16) | (SBOX[(int)((uint)s3 >> 8) & 0xff] << 8) | SBOX[s0 & 0xff]) ^ keySchedule[ksRow++];
            var t2 = ((SBOX[(int)((uint)s2 >> 24)] << 24) | (SBOX[(int)((uint)s3 >> 16) & 0xff] << 16) | (SBOX[(int)((uint)s0 >> 8) & 0xff] << 8) | SBOX[s1 & 0xff]) ^ keySchedule[ksRow++];
            var t3 = ((SBOX[(int)((uint)s3 >> 24)] << 24) | (SBOX[(int)((uint)s0 >> 16) & 0xff] << 16) | (SBOX[(int)((uint)s1 >> 8) & 0xff] << 8) | SBOX[s2 & 0xff]) ^ keySchedule[ksRow++];

            M[offset] = t0;
            M[offset + 1] = t1;
            M[offset + 2] = t2;
            M[offset + 3] = t3;
        }

        public static string Decrypt(string text, string key)
        {
            return new AES().Decrypt(Base64.Parse(text), Base64.Parse(key)).ToString();
        }


        private WordArray Decrypt(WordArray text, WordArray key)
        {
            this.key = key;
            data = new WordArray();

            DoReset();

            iv = Hex.Parse("0000000000000000").words;

            data.Concat(text);

            var finalProcessedBlocks = Process();

            Pkcs7Unpad(finalProcessedBlocks);

            return finalProcessedBlocks;
        }

        public AES()
        {
            var d = new int[256];
            for (var i = 0; i < 256; i++)
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

            var x = 0;
            var xi = 0;
            for (var i = 0; i < 256; i++)
            {
                var sx = xi ^ (xi << 1) ^ (xi << 2) ^ (xi << 3) ^ (xi << 4);
                sx = (int)((uint)sx >> 8) ^ (sx & 0xff) ^ 0x63;
                SBOX[x] = sx;
                INV_SBOX[sx] = x;

                var x2 = d[x];
                var x4 = d[x2];
                var x8 = d[x4];

                var t = (d[sx] * 0x101) ^ (sx * 0x1010100);
                SUB_MIX_0[x] = (t << 24) | (int)((uint)t >> 8);
                SUB_MIX_1[x] = (t << 16) | (int)((uint)t >> 16);
                SUB_MIX_2[x] = (t << 8) | (int)((uint)t >> 24);
                SUB_MIX_3[x] = t;

                t = (x8 * 0x1010101) ^ (x4 * 0x10001) ^ (x2 * 0x101) ^ (x * 0x1010100);
                INV_SUB_MIX_0[sx] = (t << 24) | (int)((uint)t >> 8);
                INV_SUB_MIX_1[sx] = (t << 16) | (int)((uint)t >> 16);
                INV_SUB_MIX_2[sx] = (t << 8) | (int)((uint)t >> 24);
                INV_SUB_MIX_3[sx] = t;

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
    }

    class WordArray
    {
        public List<int> words;
        public int sigBytes;

        public WordArray()
        {
            words = new List<int>();
            sigBytes = 0;
        }

        public WordArray(List<int> words, int sigBytes)
        {
            if (words == null) words = new List<int>();

            this.sigBytes = sigBytes;
            this.words = words;
        }

        public override string ToString()
        {
            var latin1 = new byte[sigBytes];
            for (var i = 0; i < sigBytes; i++)
            {
                byte bite = (byte)((words[i >> 2] >> (24 - (i % 4) * 8)) & 0xff);
                latin1[i] = bite;
            }

            return Encoding.UTF8.GetString(latin1);
        }

        public WordArray Concat(WordArray wordArray)
        {
            Clamp();

            if (sigBytes % 4 != 0)
            {
                for (var i = 0; i < wordArray.sigBytes; i++)
                {
                    var thatByte = (wordArray.words[i >> 2] >> (24 - (i % 4) * 8)) & 0xff;
                    words[(sigBytes + i) >> 2] |= thatByte << (24 - ((sigBytes + i) % 4) * 8);
                }
            }
            else if (wordArray.words.Count > 0xffff)
            {
                for (var i = 0; i < wordArray.sigBytes; i += 4)
                {
                    words[(sigBytes + i) >> 2] = wordArray.words[i >> 2];
                }
            }
            else
            {
                words.AddRange(wordArray.words);
            }
            sigBytes += wordArray.sigBytes;

            return this;
        }

        public void Clamp()
        {
            if (words.Count == sigBytes >> 2) words.Add(0);
            words[sigBytes >> 2] &= 0xffff << (32 - (sigBytes % 4) * 8);
            var length = (int)Math.Ceiling((double)sigBytes / 4);
            words.RemoveRange(length, words.Count - length);
        }
    }

    class Hex
    {
        public static WordArray Parse(string hexStr)
        {
            var words = new List<int>();
            for (var i = 0; i < hexStr.Length; i += 2)
            {
                if (words.Count == i >> 3) words.Add(0);
                words[i >> 3] |= Convert.ToByte(hexStr.Substring(i, 2), 16) << (24 - (i % 8) * 4);
            }

            return new WordArray(words, hexStr.Length / 2);
        }
    }

    internal class Base64
    {
        private static string map = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

        public static WordArray Parse(string base64Str)
        {
            var base64StrLength = base64Str.Length;

            var paddingIndex = base64Str.IndexOf(map[64]);
            if (paddingIndex != -1) base64StrLength = paddingIndex;

            var words = new List<int>();
            var nBytes = 0;
            for (var i = 0; i < base64StrLength; i++)
            {
                if (i % 4 != 0)
                {
                    var bits1 = map.IndexOf(base64Str[i - 1]) << ((i % 4) * 2);
                    var bits2 = map.IndexOf(base64Str[i]) >> (6 - (i % 4) * 2);
                    if (words.Count == nBytes >> 2) words.Add(0);
                    words[nBytes >> 2] |= (bits1 | bits2) << (24 - (nBytes % 4) * 8);
                    nBytes++;
                }
            }

            return new WordArray(words, nBytes);
        }
    }
}
