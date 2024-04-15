var StiGZipHelper = (function () {
    function StiGZipHelper() {
    }

    var Compression;
    (function (Compression) {
        var DeflateCT = (function () {
            function DeflateCT() {
                this.fc = 0;
                this.dl = 0;
            }
            return DeflateCT;
        }());
        var DeflateTreeDesc = (function () {
            function DeflateTreeDesc() {
                this.dynamicTree = null;
                this.staticTree = null;
                this.extraBits = null;
                this.extraBase = 0;
                this.elements = 0;
                this.maxLength = 0;
                this.maxCode = 0;
            }
            return DeflateTreeDesc;
        }());
        var DeflateConfiguration = (function () {
            function DeflateConfiguration(a, b, c, d) {
                this.goodLength = a;
                this.maxLazy = b;
                this.niceLength = c;
                this.maxChain = d;
            }
            return DeflateConfiguration;
        }());
        var DeflateBuffer = (function () {
            function DeflateBuffer() {
                this.next = null;
                this.length = 0;
                this.ptr = new Array(Constants.outBufferSize);
                this.offset = 0;
            }
            return DeflateBuffer;
        }());
        var HuftList = (function () {
            function HuftList() {
                this.next = null;
                this.list = null;
            }
            return HuftList;
        }());
        var HuftNode = (function () {
            function HuftNode() {
                this.e = 0;
                this.b = 0;
                this.n = 0;
                this.t = null;
            }
            return HuftNode;
        }());
        var HuftBuild = (function () {
            function HuftBuild(b, n, s, d, e, mm) {
                this.status = 0;
                this.root = null;
                this.m = 0;
                var i;
                var j;
                var y;
                var o;
                var c = new Array(HuftBuild.bMax + 1);
                var lx = new Array(HuftBuild.bMax + 1);
                var q;
                var r = new HuftNode();
                var u = new Array(HuftBuild.bMax);
                var v = new Array(HuftBuild.nMax);
                var x = new Array(HuftBuild.bMax + 1);
                var tail = null;
                this.root = null;
                for (i = 0; i < c.length; i++)
                    c[i] = 0;
                for (i = 0; i < lx.length; i++)
                    lx[i] = 0;
                for (i = 0; i < u.length; i++)
                    u[i] = null;
                for (i = 0; i < v.length; i++)
                    v[i] = 0;
                for (i = 0; i < x.length; i++)
                    x[i] = 0;
                var el = n > 256 ? b[256] : HuftBuild.bMax;
                var p = b;
                var pidx = 0;
                i = n;
                do {
                    c[p[pidx]]++;
                    pidx++;
                } while (--i > 0);
                if (c[0] == n) {
                    this.root = null;
                    this.m = 0;
                    this.status = 0;
                    return;
                }
                for (j = 1; j <= HuftBuild.bMax; j++)
                    if (c[j] != 0)
                        break;
                var k = j;
                if (mm < j)
                    mm = j;
                for (i = HuftBuild.bMax; i != 0; i--)
                    if (c[i] != 0)
                        break;
                var g = i;
                if (mm > i)
                    mm = i;
                for (y = 1 << j; j < i; j++, y <<= 1)
                    if ((y -= c[j]) < 0) {
                        this.status = 2;
                        this.m = mm;
                        return;
                    }
                if ((y -= c[i]) < 0) {
                    this.status = 2;
                    this.m = mm;
                    return;
                }
                c[i] += y;
                x[1] = j = 0;
                p = c;
                pidx = 1;
                var xp = 2;
                while (--i > 0)
                    x[xp++] = (j += p[pidx++]);
                p = b;
                pidx = 0;
                i = 0;
                do {
                    if ((j = p[pidx++]) != 0)
                        v[x[j]++] = i;
                } while (++i < n);
                n = x[g];
                x[0] = i = 0;
                p = v;
                pidx = 0;
                var h = -1;
                var w = lx[0] = 0;
                var z = 0;
                q = null;
                var a;
                var f;
                for (; k <= g; k++) {
                    a = c[k];
                    while (a-- > 0) {
                        while (k > w + lx[1 + h]) {
                            w += lx[1 + h];
                            h++;
                            z = (z = g - w) > mm ? mm : z;
                            if ((f = 1 << (j = k - w)) > a + 1) {
                                f -= a + 1;
                                xp = k;
                                while (++j < z) {
                                    if ((f <<= 1) <= c[++xp])
                                        break;
                                    f -= c[xp];
                                }
                            }
                            if (w + j > el && w < el)
                                j = el - w;
                            z = 1 << j;
                            lx[1 + h] = j;
                            q = new Array(z);
                            for (o = 0; o < z; o++) {
                                q[o] = new HuftNode();
                            }
                            if (tail == null)
                                tail = this.root = new HuftList();
                            else
                                tail = tail.next = new HuftList();
                            tail.next = null;
                            tail.list = q;
                            u[h] = q;
                            if (h > 0) {
                                x[h] = i;
                                r.b = lx[h];
                                r.e = 16 + j;
                                r.t = q;
                                j = (i & ((1 << w) - 1)) >> (w - lx[h]);
                                u[h - 1][j].e = r.e;
                                u[h - 1][j].b = r.b;
                                u[h - 1][j].n = r.n;
                                u[h - 1][j].t = r.t;
                            }
                        }
                        r.b = k - w;
                        if (pidx >= n)
                            r.e = 99;
                        else if (p[pidx] < s) {
                            r.e = (p[pidx] < 256 ? 16 : 15);
                            r.n = p[pidx++];
                        }
                        else {
                            r.e = e[p[pidx] - s];
                            r.n = d[p[pidx++] - s];
                        }
                        f = 1 << (k - w);
                        for (j = i >> w; j < z; j += f) {
                            q[j].e = r.e;
                            q[j].b = r.b;
                            q[j].n = r.n;
                            q[j].t = r.t;
                        }
                        for (j = 1 << (k - 1); (i & j) != 0; j >>= 1)
                            i ^= j;
                        i ^= j;
                        while ((i & ((1 << w) - 1)) != x[h]) {
                            w -= lx[h];
                            h--;
                        }
                    }
                }
                this.m = lx[1];
                this.status = ((y != 0 && g != 1) ? 1 : 0);
            }
            HuftBuild.bMax = 16;
            HuftBuild.nMax = 288;
            return HuftBuild;
        }());
        var Constants = (function () {
            function Constants() {
            }
            Constants.wSize = 32768;
            Constants.storedBlock = 0;
            Constants.staticTrees = 1;
            Constants.dynamicTrees = 2;
            Constants.defaultLevel = 6;
            Constants.fullSearch = true;
            Constants.lBits = 9;
            Constants.dBits = 6;
            Constants.inBufferSize = 32768;
            Constants.inBufferExtra = 64;
            Constants.outBufferSize = 1024 * 8;
            Constants.windowSize = 2 * Constants.wSize;
            Constants.minMatch = 3;
            Constants.maxMatch = 258;
            Constants.bits = 16;
            Constants.litBufferSize = 0x2000;
            Constants.distBufferSize = Constants.litBufferSize;
            Constants.hashBits = 13;
            Constants.hashSize = 1 << Constants.hashBits;
            Constants.hashMask = Constants.hashSize - 1;
            Constants.wMask = Constants.wSize - 1;
            Constants.nil = 0;
            Constants.tooFar = 4096;
            Constants.minLookahead = Constants.maxMatch + Constants.minMatch + 1;
            Constants.maxDist = Constants.wSize - Constants.minLookahead;
            Constants.smallest = 1;
            Constants.maxBits = 15;
            Constants.maxBLBits = 7;
            Constants.lengthCodes = 29;
            Constants.literals = 256;
            Constants.endBlock = 256;
            Constants.lCodes = Constants.literals + 1 + Constants.lengthCodes;
            Constants.dCodes = 30;
            Constants.blCodes = 19;
            Constants.rep_3_6 = 16;
            Constants.repz_3_10 = 17;
            Constants.repz_11_138 = 18;
            Constants.heapSize = 2 * Constants.lCodes + 1;
            Constants.hShift = parseInt(((Constants.hashBits + Constants.minMatch - 1) / Constants.minMatch).toString());
            Constants.bufferSize = 16;
            Constants.maskBits = [0x0000, 0x0001, 0x0003, 0x0007, 0x000f, 0x001f, 0x003f, 0x007f, 0x00ff, 0x01ff, 0x03ff, 0x07ff, 0x0fff, 0x1fff, 0x3fff, 0x7fff, 0xffff];
            Constants.cplens = [3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 17, 19, 23, 27, 31, 35, 43, 51, 59, 67, 83, 99, 115, 131, 163, 195, 227, 258, 0, 0];
            Constants.cplext = [0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0, 99, 99];
            Constants.cpdist = [1, 2, 3, 4, 5, 7, 9, 13, 17, 25, 33, 49, 65, 97, 129, 193, 257, 385, 513, 769, 1025, 1537, 2049, 3073, 4097, 6145, 8193, 12289, 16385, 24577];
            Constants.cpdext = [0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13];
            Constants.border = [16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15];
            Constants.extraLBits = [0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0];
            Constants.extraDBits = [0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13];
            Constants.extraBLBits = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 3, 7];
            Constants.blOrder = [16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15];
            Constants.configTable = [
                new DeflateConfiguration(0, 0, 0, 0),
                new DeflateConfiguration(4, 4, 8, 4),
                new DeflateConfiguration(4, 5, 16, 8),
                new DeflateConfiguration(4, 6, 32, 32),
                new DeflateConfiguration(4, 4, 16, 16),
                new DeflateConfiguration(8, 16, 32, 32),
                new DeflateConfiguration(8, 16, 128, 128),
                new DeflateConfiguration(8, 32, 128, 256),
                new DeflateConfiguration(32, 128, 258, 1024),
                new DeflateConfiguration(32, 258, 258, 4096)
            ];
            return Constants;
        }());
        var Helper = (function () {
            function Helper() {
                this._outBuffer = null;
                this._fixedTL = null;
            }
            Helper.deflate = function (data, level) {
                if (data == null || data === undefined)
                    return null;
                if (this._helper == null)
                    this._helper = new Helper();
                if (typeof data == 'string')
                    return this._helper.deflate(data.toUnicodeString().toBytesArray(), level);
                return this._helper.deflate(data, level);
            };
            Helper.inflate = function (data) {
                if (this._helper == null)
                    this._helper = new Helper();
                return this._helper.inflate(data);
            };
            Helper.prototype.deflateStart = function (level) {
                var i;
                if (!level)
                    level = Constants.defaultLevel;
                else if (level < 1)
                    level = 1;
                else if (level > 9)
                    level = 9;
                this._compressLevel = level;
                this._initFlag = false;
                this._eoFile = false;
                if (this._outBuffer != null)
                    return;
                this._freeQueue = null;
                this._qHead = null;
                this._qTail = null;
                this._outBuffer = new Array(Constants.outBufferSize);
                this._window = new Array(Constants.windowSize);
                this._dBuffer = new Array(Constants.distBufferSize);
                this._lBuffer = new Array(Constants.inBufferSize + Constants.inBufferExtra);
                this._prev = new Array(1 << Constants.bits);
                this._dynamicLtree = new Array(Constants.heapSize);
                for (i = 0; i < Constants.heapSize; i++)
                    this._dynamicLtree[i] = new DeflateCT();
                this._dynamicDtree = new Array(2 * Constants.dCodes + 1);
                for (i = 0; i < 2 * Constants.dCodes + 1; i++)
                    this._dynamicDtree[i] = new DeflateCT();
                this._staticLtree = new Array(Constants.lCodes + 2);
                for (i = 0; i < Constants.lCodes + 2; i++)
                    this._staticLtree[i] = new DeflateCT();
                this._staticDtree = new Array(Constants.dCodes);
                for (i = 0; i < Constants.dCodes; i++)
                    this._staticDtree[i] = new DeflateCT();
                this._blTree = new Array(2 * Constants.blCodes + 1);
                for (i = 0; i < 2 * Constants.blCodes + 1; i++)
                    this._blTree[i] = new DeflateCT();
                this._lDesc = new DeflateTreeDesc();
                this._dDesc = new DeflateTreeDesc();
                this._blDesc = new DeflateTreeDesc();
                this._blCount = new Array(Constants.maxBits + 1);
                this._heap = new Array(2 * Constants.lCodes + 1);
                this._depth = new Array(2 * Constants.lCodes + 1);
                this._lengthCode = new Array(Constants.maxMatch - Constants.minMatch + 1);
                this._distCode = new Array(512);
                this._baseLength = new Array(Constants.lengthCodes);
                this._baseDist = new Array(Constants.dCodes);
                this._flagBuf = new Array(parseInt((Constants.litBufferSize / 8).toString()));
            };
            Helper.prototype.deflateEnd = function () {
                this._freeQueue = null;
                this._qHead = null;
                this._qTail = null;
                this._outBuffer = null;
                this._window = null;
                this._dBuffer = null;
                this._lBuffer = null;
                this._prev = null;
                this._dynamicLtree = null;
                this._dynamicDtree = null;
                this._staticLtree = null;
                this._staticDtree = null;
                this._blTree = null;
                this._lDesc = null;
                this._dDesc = null;
                this._blDesc = null;
                this._blCount = null;
                this._heap = null;
                this._depth = null;
                this._lengthCode = null;
                this._distCode = null;
                this._baseLength = null;
                this._baseDist = null;
                this._flagBuf = null;
            };
            Helper.prototype.reuseQueue = function (p) {
                p.next = this._freeQueue;
                this._freeQueue = p;
            };
            Helper.prototype.newQueue = function () {
                var p;
                if (this._freeQueue != null) {
                    p = this._freeQueue;
                    this._freeQueue = this._freeQueue.next;
                }
                else
                    p = new DeflateBuffer();
                p.next = null;
                p.length = 0;
                p.offset = 0;
                return p;
            };
            Helper.prototype.head1 = function (i) {
                return this._prev[Constants.wSize + i];
            };
            Helper.prototype.head2 = function (i, val) {
                return this._prev[Constants.wSize + i] = val;
            };
            Helper.prototype.putByte = function (c) {
                this._outBuffer[this._outOffset + this._outCount++] = c;
                if (this._outOffset + this._outCount == Constants.outBufferSize)
                    this.qOutBuffer();
            };
            Helper.prototype.putShort = function (w) {
                w &= 0xffff;
                if (this._outOffset + this._outCount < Constants.outBufferSize - 2) {
                    this._outBuffer[this._outOffset + this._outCount++] = (w & 0xff);
                    this._outBuffer[this._outOffset + this._outCount++] = (w >>> 8);
                }
                else {
                    this.putByte(w & 0xff);
                    this.putByte(w >>> 8);
                }
            };
            Helper.prototype.insertString = function () {
                this._insH = ((this._insH << Constants.hShift) ^ (this._window[this._strStart + Constants.minMatch - 1] & 0xff)) & Constants.hashMask;
                this._hashHead = this.head1(this._insH);
                this._prev[this._strStart & Constants.wMask] = this._hashHead;
                this.head2(this._insH, this._strStart);
            };
            Helper.prototype.sendCode = function (c, tree) {
                this.sendBits(tree[c].fc, tree[c].dl);
            };
            Helper.prototype.dCode = function (dist) {
                return (dist < 256 ? this._distCode[dist] : this._distCode[256 + (dist >> 7)]) & 0xff;
            };
            Helper.prototype.smaller = function (tree, n, m) {
                return tree[n].fc < tree[m].fc || (tree[n].fc == tree[m].fc && this._depth[n] <= this._depth[m]);
            };
            Helper.prototype.readBuffer = function (buffer, offset, n) {
                var i;
                for (i = 0; i < n && this._deflatePos < this._deflateData.length; i++)
                    buffer[offset + i] = this._deflateData[this._deflatePos++] & 0xff;
                return i;
            };
            Helper.prototype.lmInit = function () {
                var j;
                for (j = 0; j < Constants.hashSize; j++)
                    this._prev[Constants.wSize + j] = 0;
                this._maxLazyMatch = Constants.configTable[this._compressLevel].maxLazy;
                this._goodMatch = Constants.configTable[this._compressLevel].goodLength;
                if (!Constants.fullSearch)
                    this._niceMatch = Constants.configTable[this._compressLevel].niceLength;
                this._maxChainLength = Constants.configTable[this._compressLevel].maxChain;
                this._strStart = 0;
                this._blockStart = 0;
                this._lookahead = this.readBuffer(this._window, 0, 2 * Constants.wSize);
                if (this._lookahead <= 0) {
                    this._eoFile = true;
                    this._lookahead = 0;
                    return;
                }
                this._eoFile = false;
                while (this._lookahead < Constants.minLookahead && !this._eoFile)
                    this.fillWindow();
                this._insH = 0;
                for (j = 0; j < Constants.minMatch - 1; j++) {
                    this._insH = ((this._insH << Constants.hShift) ^ (this._window[j] & 0xff)) & Constants.hashMask;
                }
            };
            Helper.prototype.longestMatch = function (curMatch) {
                var chainLength = this._maxChainLength;
                var scanP = this._strStart;
                var matchP;
                var length;
                var bestLength = this._prevLength;
                var limit = (this._strStart > Constants.maxDist ? this._strStart - Constants.maxDist : Constants.nil);
                var strendp = this._strStart + Constants.maxMatch;
                var scanEnd1 = this._window[scanP + bestLength - 1];
                var scanEnd2 = this._window[scanP + bestLength];
                if (this._prevLength >= this._goodMatch)
                    chainLength >>= 2;
                do {
                    matchP = curMatch;
                    if (this._window[matchP + bestLength] != scanEnd2 ||
                        this._window[matchP + bestLength - 1] != scanEnd1 ||
                        this._window[matchP] != this._window[scanP] ||
                        this._window[++matchP] != this._window[scanP + 1]) {
                        continue;
                    }
                    scanP += 2;
                    matchP++;
                    do {
                    } while (this._window[++scanP] == this._window[++matchP] &&
                    this._window[++scanP] == this._window[++matchP] &&
                    this._window[++scanP] == this._window[++matchP] &&
                    this._window[++scanP] == this._window[++matchP] &&
                    this._window[++scanP] == this._window[++matchP] &&
                    this._window[++scanP] == this._window[++matchP] &&
                    this._window[++scanP] == this._window[++matchP] &&
                    this._window[++scanP] == this._window[++matchP] &&
                        scanP < strendp);
                    length = Constants.maxMatch - (strendp - scanP);
                    scanP = strendp - Constants.maxMatch;
                    if (length > bestLength) {
                        this._matchStart = curMatch;
                        bestLength = length;
                        if (Constants.fullSearch) {
                            if (length >= Constants.maxMatch)
                                break;
                        }
                        else {
                            if (length >= this._niceMatch)
                                break;
                        }
                        scanEnd1 = this._window[scanP + bestLength - 1];
                        scanEnd2 = this._window[scanP + bestLength];
                    }
                } while ((curMatch = this._prev[curMatch & Constants.wMask]) > limit && --chainLength != 0);
                return bestLength;
            };
            Helper.prototype.fillWindow = function () {
                var n;
                var m;
                var more = Constants.windowSize - this._lookahead - this._strStart;
                if (more == -1)
                    more--;
                else if (this._strStart >= Constants.wSize + Constants.maxDist) {
                    for (n = 0; n < Constants.wSize; n++)
                        this._window[n] = this._window[n + Constants.wSize];
                    this._matchStart -= Constants.wSize;
                    this._strStart -= Constants.wSize;
                    this._blockStart -= Constants.wSize;
                    for (n = 0; n < Constants.hashSize; n++) {
                        m = this.head1(n);
                        this.head2(n, m >= Constants.wSize ? m - Constants.wSize : Constants.nil);
                    }
                    for (n = 0; n < Constants.wSize; n++) {
                        m = this._prev[n];
                        this._prev[n] = (m >= Constants.wSize ? m - Constants.wSize : Constants.nil);
                    }
                    more += Constants.wSize;
                }
                if (!this._eoFile) {
                    n = this.readBuffer(this._window, this._strStart + this._lookahead, more);
                    if (n <= 0)
                        this._eoFile = true;
                    else
                        this._lookahead += n;
                }
            };
            Helper.prototype.deflateFast = function () {
                while (this._lookahead != 0 && this._qHead == null) {
                    var flush = void 0;
                    this.insertString();
                    if (this._hashHead != Constants.nil && this._strStart - this._hashHead <= Constants.maxDist) {
                        this._matchLength = this.longestMatch(this._hashHead);
                        if (this._matchLength > this._lookahead)
                            this._matchLength = this._lookahead;
                    }
                    if (this._matchLength >= Constants.minMatch) {
                        flush = this.ctTally(this._strStart - this._matchStart, this._matchLength - Constants.minMatch);
                        this._lookahead -= this._matchLength;
                        if (this._matchLength <= this._maxLazyMatch) {
                            this._matchLength--;
                            do {
                                this._strStart++;
                                this.insertString();
                            } while (--this._matchLength != 0);
                            this._strStart++;
                        }
                        else {
                            this._strStart += this._matchLength;
                            this._matchLength = 0;
                            this._insH = this._window[this._strStart] & 0xff;
                            this._insH = ((this._insH << Constants.hShift) ^ (this._window[this._strStart + 1] & 0xff)) & Constants.hashMask;
                        }
                    }
                    else {
                        flush = this.ctTally(0, this._window[this._strStart] & 0xff);
                        this._lookahead--;
                        this._strStart++;
                    }
                    if (flush) {
                        this.flushBlock(0);
                        this._blockStart = this._strStart;
                    }
                    while (this._lookahead < Constants.minLookahead && !this._eoFile)
                        this.fillWindow();
                }
            };
            Helper.prototype.deflateBetter = function () {
                while (this._lookahead != 0 && this._qHead == null) {
                    this.insertString();
                    this._prevLength = this._matchLength;
                    this._prevMatch = this._matchStart;
                    this._matchLength = Constants.minMatch - 1;
                    if (this._hashHead != Constants.nil &&
                        this._prevLength < this._maxLazyMatch &&
                        this._strStart - this._hashHead <= Constants.maxDist) {
                        this._matchLength = this.longestMatch(this._hashHead);
                        if (this._matchLength > this._lookahead)
                            this._matchLength = this._lookahead;
                        if (this._matchLength == Constants.minMatch && this._strStart - this._matchStart > Constants.tooFar)
                            this._matchLength--;
                    }
                    if (this._prevLength >= Constants.minMatch && this._matchLength <= this._prevLength) {
                        var flush = this.ctTally(this._strStart - 1 - this._prevMatch, this._prevLength - Constants.minMatch);
                        this._lookahead -= this._prevLength - 1;
                        this._prevLength -= 2;
                        do {
                            this._strStart++;
                            this.insertString();
                        } while (--this._prevLength != 0);
                        this._matchAvailable = 0;
                        this._matchLength = Constants.minMatch - 1;
                        this._strStart++;
                        if (flush) {
                            this.flushBlock(0);
                            this._blockStart = this._strStart;
                        }
                    }
                    else if (this._matchAvailable != 0) {
                        if (this.ctTally(0, this._window[this._strStart - 1] & 0xff)) {
                            this.flushBlock(0);
                            this._blockStart = this._strStart;
                        }
                        this._strStart++;
                        this._lookahead--;
                    }
                    else {
                        this._matchAvailable = 1;
                        this._strStart++;
                        this._lookahead--;
                    }
                    while (this._lookahead < Constants.minLookahead && !this._eoFile)
                        this.fillWindow();
                }
            };
            Helper.prototype.initDeflate = function () {
                if (this._eoFile)
                    return;
                this._biBuffer = 0;
                this._biValid = 0;
                this.ctInit();
                this.lmInit();
                this._qHead = null;
                this._outCount = 0;
                this._outOffset = 0;
                this._matchAvailable = 0;
                if (this._compressLevel <= 3) {
                    this._prevLength = Constants.minMatch - 1;
                    this._matchLength = 0;
                }
                else {
                    this._matchLength = Constants.minMatch - 1;
                    this._matchAvailable = 0;
                    this._matchAvailable = 0;
                }
                this._complete = false;
            };
            Helper.prototype.deflateInternal = function (buffer, offset, bufferSize) {
                var n;
                if (!this._initFlag) {
                    this.initDeflate();
                    this._initFlag = true;
                    if (this._lookahead == 0) {
                        this._complete = true;
                        return 0;
                    }
                }
                if ((n = this.qCopy(buffer, offset, bufferSize)) == bufferSize)
                    return bufferSize;
                if (this._complete)
                    return n;
                if (this._compressLevel <= 3)
                    this.deflateFast();
                else
                    this.deflateBetter();
                if (this._lookahead == 0) {
                    if (this._matchAvailable != 0)
                        this.ctTally(0, this._window[this._strStart - 1] & 0xff);
                    this.flushBlock(1);
                    this._complete = true;
                }
                return n + this.qCopy(buffer, n + offset, bufferSize - n);
            };
            Helper.prototype.qCopy = function (buffer, offset, bufferSize) {
                var n = 0;
                var i;
                var j;
                while (this._qHead != null && n < bufferSize) {
                    i = bufferSize - n;
                    if (i > this._qHead.length)
                        i = this._qHead.length;
                    for (j = 0; j < i; j++)
                        buffer[offset + n + j] = this._qHead.ptr[this._qHead.offset + j];
                    this._qHead.offset += i;
                    this._qHead.length -= i;
                    n += i;
                    if (this._qHead.length == 0) {
                        var p = this._qHead;
                        this._qHead = this._qHead.next;
                        this.reuseQueue(p);
                    }
                }
                if (n == bufferSize)
                    return n;
                if (this._outOffset < this._outCount) {
                    i = bufferSize - n;
                    if (i > this._outCount - this._outOffset)
                        i = this._outCount - this._outOffset;
                    for (j = 0; j < i; j++)
                        buffer[offset + n + j] = this._outBuffer[this._outOffset + j];
                    this._outOffset += i;
                    n += i;
                    if (this._outCount == this._outOffset)
                        this._outCount = this._outOffset = 0;
                }
                return n;
            };
            Helper.prototype.ctInit = function () {
                var n;
                var code;
                if (this._staticDtree[0].dl != 0)
                    return;
                this._lDesc.dynamicTree = this._dynamicLtree;
                this._lDesc.staticTree = this._staticLtree;
                this._lDesc.extraBits = Constants.extraLBits;
                this._lDesc.extraBase = Constants.literals + 1;
                this._lDesc.elements = Constants.lCodes;
                this._lDesc.maxLength = Constants.maxBits;
                this._lDesc.maxCode = 0;
                this._dDesc.dynamicTree = this._dynamicDtree;
                this._dDesc.staticTree = this._staticDtree;
                this._dDesc.extraBits = Constants.extraDBits;
                this._dDesc.extraBase = 0;
                this._dDesc.elements = Constants.dCodes;
                this._dDesc.maxLength = Constants.maxBits;
                this._dDesc.maxCode = 0;
                this._blDesc.dynamicTree = this._blTree;
                this._blDesc.staticTree = null;
                this._blDesc.extraBits = Constants.extraBLBits;
                this._blDesc.extraBase = 0;
                this._blDesc.elements = Constants.blCodes;
                this._blDesc.maxLength = Constants.maxBLBits;
                this._blDesc.maxCode = 0;
                var length = 0;
                for (code = 0; code < Constants.lengthCodes - 1; code++) {
                    this._baseLength[code] = length;
                    for (n = 0; n < (1 << Constants.extraLBits[code]); n++)
                        this._lengthCode[length++] = code;
                }
                this._lengthCode[length - 1] = code;
                var dist = 0;
                for (code = 0; code < 16; code++) {
                    this._baseDist[code] = dist;
                    for (n = 0; n < (1 << Constants.extraDBits[code]); n++) {
                        this._distCode[dist++] = code;
                    }
                }
                dist >>= 7;
                for (; code < Constants.dCodes; code++) {
                    this._baseDist[code] = dist << 7;
                    for (n = 0; n < (1 << (Constants.extraDBits[code] - 7)); n++)
                        this._distCode[256 + dist++] = code;
                }
                for (var bits = 0; bits <= Constants.maxBits; bits++)
                    this._blCount[bits] = 0;
                n = 0;
                while (n <= 143) {
                    this._staticLtree[n++].dl = 8;
                    this._blCount[8]++;
                }
                while (n <= 255) {
                    this._staticLtree[n++].dl = 9;
                    this._blCount[9]++;
                }
                while (n <= 279) {
                    this._staticLtree[n++].dl = 7;
                    this._blCount[7]++;
                }
                while (n <= 287) {
                    this._staticLtree[n++].dl = 8;
                    this._blCount[8]++;
                }
                this.genCodes(this._staticLtree, Constants.lCodes + 1);
                for (n = 0; n < Constants.dCodes; n++) {
                    this._staticDtree[n].dl = 5;
                    this._staticDtree[n].fc = this.biReverse(n, 5);
                }
                this.initBlock();
            };
            Helper.prototype.initBlock = function () {
                var n;
                for (n = 0; n < Constants.lCodes; n++)
                    this._dynamicLtree[n].fc = 0;
                for (n = 0; n < Constants.dCodes; n++)
                    this._dynamicDtree[n].fc = 0;
                for (n = 0; n < Constants.blCodes; n++)
                    this._blTree[n].fc = 0;
                this._dynamicLtree[Constants.endBlock].fc = 1;
                this._optLen = this._staticLen = 0;
                this._lastLit = this._lastDist = this._lastFlags = 0;
                this._flags = 0;
                this._flagBit = 1;
            };
            Helper.prototype.pqDownHeap = function (tree, k) {
                var v = this._heap[k];
                var j = k << 1;
                while (j <= this._heapLen) {
                    if (j < this._heapLen && this.smaller(tree, this._heap[j + 1], this._heap[j]))
                        j++;
                    if (this.smaller(tree, v, this._heap[j]))
                        break;
                    this._heap[k] = this._heap[j];
                    k = j;
                    j <<= 1;
                }
                this._heap[k] = v;
            };
            Helper.prototype.genBitLength = function (desc) {
                var tree = desc.dynamicTree;
                var extra = desc.extraBits;
                var base = desc.extraBase;
                var maxCode = desc.maxCode;
                var maxLength = desc.maxLength;
                var staticTree = desc.staticTree;
                var h;
                var n;
                var m;
                var bits;
                var xbits;
                var f;
                var overflow = 0;
                for (bits = 0; bits <= Constants.maxBits; bits++)
                    this._blCount[bits] = 0;
                tree[this._heap[this._heapMax]].dl = 0;
                for (h = this._heapMax + 1; h < Constants.heapSize; h++) {
                    n = this._heap[h];
                    bits = tree[tree[n].dl].dl + 1;
                    if (bits > maxLength) {
                        bits = maxLength;
                        overflow++;
                    }
                    tree[n].dl = bits;
                    if (n > maxCode)
                        continue;
                    this._blCount[bits]++;
                    xbits = 0;
                    if (n >= base)
                        xbits = extra[n - base];
                    f = tree[n].fc;
                    this._optLen += f * (bits + xbits);
                    if (staticTree != null)
                        this._staticLen += f * (staticTree[n].dl + xbits);
                }
                if (overflow == 0)
                    return;
                do {
                    bits = maxLength - 1;
                    while (this._blCount[bits] == 0)
                        bits--;
                    this._blCount[bits]--;
                    this._blCount[bits + 1] += 2;
                    this._blCount[maxLength]--;
                    overflow -= 2;
                } while (overflow > 0);
                for (bits = maxLength; bits != 0; bits--) {
                    n = this._blCount[bits];
                    while (n != 0) {
                        m = this._heap[--h];
                        if (m > maxCode)
                            continue;
                        if (tree[m].dl != bits) {
                            this._optLen += (bits - tree[m].dl) * tree[m].fc;
                            tree[m].fc = bits;
                        }
                        n--;
                    }
                }
            };
            Helper.prototype.genCodes = function (tree, maxCode) {
                var nextCode = new Array(Constants.maxBits + 1);
                var code = 0;
                for (var bits = 1; bits <= Constants.maxBits; bits++) {
                    code = ((code + this._blCount[bits - 1]) << 1);
                    nextCode[bits] = code;
                }
                for (var n = 0; n <= maxCode; n++) {
                    var len = tree[n].dl;
                    if (len == 0)
                        continue;
                    tree[n].fc = this.biReverse(nextCode[len]++, len);
                }
            };
            Helper.prototype.buildTree = function (desc) {
                var tree = desc.dynamicTree;
                var staticTree = desc.staticTree;
                var elems = desc.elements;
                var n;
                var m;
                var maxCode = -1;
                var node = elems;
                this._heapLen = 0;
                this._heapMax = Constants.heapSize;
                for (n = 0; n < elems; n++) {
                    if (tree[n].fc != 0) {
                        this._heap[++this._heapLen] = maxCode = n;
                        this._depth[n] = 0;
                    }
                    else
                        tree[n].dl = 0;
                }
                while (this._heapLen < 2) {
                    var xnew = this._heap[++this._heapLen] = (maxCode < 2 ? ++maxCode : 0);
                    tree[xnew].fc = 1;
                    this._depth[xnew] = 0;
                    this._optLen--;
                    if (staticTree != null)
                        this._staticLen -= staticTree[xnew].dl;
                }
                desc.maxCode = maxCode;
                for (n = this._heapLen >> 1; n >= 1; n--)
                    this.pqDownHeap(tree, n);
                do {
                    n = this._heap[Constants.smallest];
                    this._heap[Constants.smallest] = this._heap[this._heapLen--];
                    this.pqDownHeap(tree, Constants.smallest);
                    m = this._heap[Constants.smallest];
                    this._heap[--this._heapMax] = n;
                    this._heap[--this._heapMax] = m;
                    tree[node].fc = tree[n].fc + tree[m].fc;
                    if (this._depth[n] > this._depth[m] + 1)
                        this._depth[node] = this._depth[n];
                    else
                        this._depth[node] = this._depth[m] + 1;
                    tree[n].dl = tree[m].dl = node;
                    this._heap[Constants.smallest] = node++;
                    this.pqDownHeap(tree, Constants.smallest);
                } while (this._heapLen >= 2);
                this._heap[--this._heapMax] = this._heap[Constants.smallest];
                this.genBitLength(desc);
                this.genCodes(tree, maxCode);
            };
            Helper.prototype.scanTree = function (tree, maxCode) {
                var prevLength = -1;
                var currentLength;
                var nextLength = tree[0].dl;
                var count = 0;
                var minCount = 4;
                var maxCount = 7;
                if (nextLength == 0) {
                    minCount = 3;
                    maxCount = 138;
                }
                tree[maxCode + 1].dl = 0xffff;
                for (var n = 0; n <= maxCode; n++) {
                    currentLength = nextLength;
                    nextLength = tree[n + 1].dl;
                    if (++count < maxCount && currentLength == nextLength)
                        continue;
                    else if (count < minCount)
                        this._blTree[currentLength].fc += count;
                    else if (currentLength != 0) {
                        if (currentLength != prevLength)
                            this._blTree[currentLength].fc++;
                        this._blTree[Constants.rep_3_6].fc++;
                    }
                    else if (count <= 10)
                        this._blTree[Constants.repz_3_10].fc++;
                    else
                        this._blTree[Constants.repz_11_138].fc++;
                    count = 0;
                    prevLength = currentLength;
                    if (nextLength == 0) {
                        minCount = 3;
                        maxCount = 138;
                    }
                    else if (currentLength == nextLength) {
                        minCount = 3;
                        maxCount = 6;
                    }
                    else {
                        minCount = 4;
                        maxCount = 7;
                    }
                }
            };
            Helper.prototype.sendTree = function (tree, maxCode) {
                var prevLen = -1;
                var currentLen;
                var nextLen = tree[0].dl;
                var count = 0;
                var minCount = 4;
                var maxCount = 7;
                if (nextLen == 0) {
                    minCount = 3;
                    maxCount = 138;
                }
                for (var n = 0; n <= maxCode; n++) {
                    currentLen = nextLen;
                    nextLen = tree[n + 1].dl;
                    if (++count < maxCount && currentLen == nextLen)
                        continue;
                    else if (count < minCount) {
                        do {
                            this.sendCode(currentLen, this._blTree);
                        } while (--count != 0);
                    }
                    else if (currentLen != 0) {
                        if (currentLen != prevLen) {
                            this.sendCode(currentLen, this._blTree);
                            count--;
                        }
                        this.sendCode(Constants.rep_3_6, this._blTree);
                        this.sendBits(count - 3, 2);
                    }
                    else if (count <= 10) {
                        this.sendCode(Constants.repz_3_10, this._blTree);
                        this.sendBits(count - 3, 3);
                    }
                    else {
                        this.sendCode(Constants.repz_11_138, this._blTree);
                        this.sendBits(count - 11, 7);
                    }
                    count = 0;
                    prevLen = currentLen;
                    if (nextLen == 0) {
                        minCount = 3;
                        maxCount = 138;
                    }
                    else if (currentLen == nextLen) {
                        minCount = 3;
                        maxCount = 6;
                    }
                    else {
                        minCount = 4;
                        maxCount = 7;
                    }
                }
            };
            Helper.prototype.buildBlTree = function () {
                var maxBlIndex;
                this.scanTree(this._dynamicLtree, this._lDesc.maxCode);
                this.scanTree(this._dynamicDtree, this._dDesc.maxCode);
                this.buildTree(this._blDesc);
                for (maxBlIndex = Constants.blCodes - 1; maxBlIndex >= 3; maxBlIndex--) {
                    if (this._blTree[Constants.blOrder[maxBlIndex]].dl != 0)
                        break;
                }
                this._optLen += 3 * (maxBlIndex + 1) + 5 + 5 + 4;
                return maxBlIndex;
            };
            Helper.prototype.sendAllTrees = function (lCodes, dCodes, blCodes) {
                this.sendBits(lCodes - 257, 5);
                this.sendBits(dCodes - 1, 5);
                this.sendBits(blCodes - 4, 4);
                for (var rank = 0; rank < blCodes; rank++) {
                    this.sendBits(this._blTree[Constants.blOrder[rank]].dl, 3);
                }
                this.sendTree(this._dynamicLtree, lCodes - 1);
                this.sendTree(this._dynamicDtree, dCodes - 1);
            };
            Helper.prototype.flushBlock = function (eof) {
                var storedLen = this._strStart - this._blockStart;
                this._flagBuf[this._lastFlags] = this._flags;
                this.buildTree(this._lDesc);
                this.buildTree(this._dDesc);
                var maxBlIndex = this.buildBlTree();
                var optLengthB = (this._optLen + 3 + 7) >> 3;
                var staticLengthB = (this._staticLen + 3 + 7) >> 3;
                if (staticLengthB <= optLengthB)
                    optLengthB = staticLengthB;
                if (storedLen + 4 <= optLengthB && this._blockStart >= 0) {
                    this.sendBits((Constants.storedBlock << 1) + eof, 3);
                    this.biWindup();
                    this.putShort(storedLen);
                    this.putShort(~storedLen);
                    for (var i = 0; i < storedLen; i++)
                        this.putByte(this._window[this._blockStart + i]);
                }
                else if (staticLengthB == optLengthB) {
                    this.sendBits((Constants.staticTrees << 1) + eof, 3);
                    this.compressBlock(this._staticLtree, this._staticDtree);
                }
                else {
                    this.sendBits((Constants.dynamicTrees << 1) + eof, 3);
                    this.sendAllTrees(this._lDesc.maxCode + 1, this._dDesc.maxCode + 1, maxBlIndex + 1);
                    this.compressBlock(this._dynamicLtree, this._dynamicDtree);
                }
                this.initBlock();
                if (eof != 0)
                    this.biWindup();
            };
            Helper.prototype.ctTally = function (dist, lc) {
                this._lBuffer[this._lastLit++] = lc;
                if (dist == 0)
                    this._dynamicLtree[lc].fc++;
                else {
                    dist--;
                    this._dynamicLtree[this._lengthCode[lc] + Constants.literals + 1].fc++;
                    this._dynamicDtree[this.dCode(dist)].fc++;
                    this._dBuffer[this._lastDist++] = dist;
                    this._flags |= this._flagBit;
                }
                this._flagBit <<= 1;
                if ((this._lastLit & 7) == 0) {
                    this._flagBuf[this._lastFlags++] = this._flags;
                    this._flags = 0;
                    this._flagBit = 1;
                }
                if (this._compressLevel > 2 && (this._lastLit & 0xfff) == 0) {
                    var outLength = this._lastLit * 8;
                    var inLength = this._strStart - this._blockStart;
                    for (var dCode = 0; dCode < Constants.dCodes; dCode++) {
                        outLength += this._dynamicDtree[dCode].fc * (5 + Constants.extraDBits[dCode]);
                    }
                    outLength >>= 3;
                    if (this._lastDist < parseInt((this._lastLit / 2).toString()) &&
                        outLength < parseInt((inLength / 2).toString()))
                        return true;
                }
                return (this._lastLit == Constants.litBufferSize - 1 || this._lastDist == Constants.distBufferSize);
            };
            Helper.prototype.compressBlock = function (lTree, dTree) {
                var lx = 0;
                var dx = 0;
                var fx = 0;
                var flag = 0;
                var lc;
                var code;
                var extra;
                var dist;
                if (this._lastLit != 0)
                    do {
                        if ((lx & 7) == 0)
                            flag = this._flagBuf[fx++];
                        lc = this._lBuffer[lx++] & 0xff;
                        if ((flag & 1) == 0)
                            this.sendCode(lc, lTree);
                        else {
                            code = this._lengthCode[lc];
                            this.sendCode(code + Constants.literals + 1, lTree);
                            extra = Constants.extraLBits[code];
                            if (extra != 0) {
                                lc -= this._baseLength[code];
                                this.sendBits(lc, extra);
                            }
                            dist = this._dBuffer[dx++];
                            code = this.dCode(dist);
                            this.sendCode(code, dTree);
                            extra = Constants.extraDBits[code];
                            if (extra != 0) {
                                dist -= this._baseDist[code];
                                this.sendBits(dist, extra);
                            }
                        }
                        flag >>= 1;
                    } while (lx < this._lastLit);
                this.sendCode(Constants.endBlock, lTree);
            };
            Helper.prototype.sendBits = function (value, length) {
                if (this._biValid > Constants.bufferSize - length) {
                    this._biBuffer |= (value << this._biValid);
                    this.putShort(this._biBuffer);
                    this._biBuffer = (value >> (Constants.bufferSize - this._biValid));
                    this._biValid += length - Constants.bufferSize;
                }
                else {
                    this._biBuffer |= value << this._biValid;
                    this._biValid += length;
                }
            };
            Helper.prototype.biReverse = function (code, len) {
                var res = 0;
                do {
                    res |= code & 1;
                    code >>= 1;
                    res <<= 1;
                } while (--len > 0);
                return res >> 1;
            };
            Helper.prototype.biWindup = function () {
                if (this._biValid > 8)
                    this.putShort(this._biBuffer);
                else if (this._biValid > 0)
                    this.putByte(this._biBuffer);
                this._biBuffer = 0;
                this._biValid = 0;
            };
            Helper.prototype.qOutBuffer = function () {
                if (this._outCount != 0) {
                    var q = this.newQueue();
                    if (this._qHead == null)
                        this._qHead = this._qTail = q;
                    else
                        this._qTail = this._qTail.next = q;
                    q.length = this._outCount - this._outOffset;
                    for (var i = 0; i < q.length; i++)
                        q.ptr[i] = this._outBuffer[this._outOffset + i];
                    this._outCount = this._outOffset = 0;
                }
            };
            Helper.prototype.deflate = function (data, level) {
                if (level === undefined)
                    level = Constants.defaultLevel;
                this._deflateData = data;
                this._deflatePos = 0;
                this.deflateStart(level);
                var i;
                var buffer = new Array(1024);
                var aout = [];
                while ((i = this.deflateInternal(buffer, 0, buffer.length)) > 0)
                    for (var j = 0; j < i; j++)
                        aout[aout.length] = buffer[j];
                this._deflateData = null;
                return aout;
            };
            Helper.prototype.getByte = function () {
                if (this._inflateData.length == this._inflatePos)
                    return -1;
                return this._inflateData[this._inflatePos++] & 0xff;
            };
            Helper.prototype.needBits = function (n) {
                while (this._bitLength < n) {
                    this._bitBuffer |= this.getByte() << this._bitLength;
                    this._bitLength += 8;
                }
            };
            Helper.prototype.getBits = function (n) {
                return this._bitBuffer & Constants.maskBits[n];
            };
            Helper.prototype.dumpBits = function (n) {
                this._bitBuffer >>= n;
                this._bitLength -= n;
            };
            Helper.prototype.inflateCodes = function (buffer, offset, size) {
                var e;
                var t;
                var n = 0;
                if (size == 0)
                    return 0;
                for (; ;) {
                    this.needBits(this._bl);
                    t = this._tl.list[this.getBits(this._bl)];
                    e = t.e;
                    while (e > 16) {
                        if (e == 99)
                            return -1;
                        this.dumpBits(t.b);
                        e -= 16;
                        this.needBits(e);
                        t = t.t[this.getBits(e)];
                        e = t.e;
                    }
                    this.dumpBits(t.b);
                    if (e == 16) {
                        this._wp &= Constants.wSize - 1;
                        buffer[offset + n++] = this._slide[this._wp++] = t.n;
                        if (n == size)
                            return size;
                        continue;
                    }
                    if (e == 15)
                        break;
                    this.needBits(e);
                    this._copyLength = t.n + this.getBits(e);
                    this.dumpBits(e);
                    this.needBits(this._bd);
                    t = this._td.list[this.getBits(this._bd)];
                    e = t.e;
                    while (e > 16) {
                        if (e == 99)
                            return -1;
                        this.dumpBits(t.b);
                        e -= 16;
                        this.needBits(e);
                        t = t.t[this.getBits(e)];
                        e = t.e;
                    }
                    this.dumpBits(t.b);
                    this.needBits(e);
                    this._copyDist = this._wp - t.n - this.getBits(e);
                    this.dumpBits(e);
                    while (this._copyLength > 0 && n < size) {
                        this._copyLength--;
                        this._copyDist &= Constants.wSize - 1;
                        this._wp &= Constants.wSize - 1;
                        buffer[offset + n++] = this._slide[this._wp++] = this._slide[this._copyDist++];
                    }
                    if (n == size)
                        return size;
                }
                this._method = -1;
                return n;
            };
            Helper.prototype.inflateStored = function (buffer, offset, size) {
                var n = this._bitLength & 7;
                this.dumpBits(n);
                this.needBits(16);
                n = this.getBits(16);
                this.dumpBits(16);
                this.needBits(16);
                if (n != ((~this._bitBuffer) & 0xffff))
                    return -1;
                this.dumpBits(16);
                this._copyLength = n;
                n = 0;
                while (this._copyLength > 0 && n < size) {
                    this._copyLength--;
                    this._wp &= Constants.wSize - 1;
                    this.needBits(8);
                    buffer[offset + n++] = this._slide[this._wp++] = this.getBits(8);
                    this.dumpBits(8);
                }
                if (this._copyLength == 0)
                    this._method = -1;
                return n;
            };
            Helper.prototype.inflateFixed = function (buffer, offset, size) {
                if (this._fixedTL == null) {
                    var i = void 0;
                    var l = new Array(288);
                    for (i = 0; i < 144; i++)
                        l[i] = 8;
                    for (; i < 256; i++)
                        l[i] = 9;
                    for (; i < 280; i++)
                        l[i] = 7;
                    for (; i < 288; i++)
                        l[i] = 8;
                    this._fixedBL = 7;
                    var h = new HuftBuild(l, 288, 257, Constants.cplens, Constants.cplext, this._fixedBL);
                    if (h.status != 0) {
                        throw "Error: " + h.status;
                    }
                    this._fixedTL = h.root;
                    this._fixedBL = h.m;
                    for (i = 0; i < 30; i++)
                        l[i] = 5;
                    this._fixedBD = 5;
                    h = new HuftBuild(l, 30, 0, Constants.cpdist, Constants.cpdext, this._fixedBD);
                    if (h.status > 1) {
                        this._fixedTL = null;
                        throw "Error: " + h.status;
                    }
                    this._fixedTD = h.root;
                    this._fixedBD = h.m;
                }
                this._tl = this._fixedTL;
                this._td = this._fixedTD;
                this._bl = this._fixedBL;
                this._bd = this._fixedBD;
                return this.inflateCodes(buffer, offset, size);
            };
            Helper.prototype.inflateDynamic = function (buffer, offset, size) {
                var i;
                var j;
                var ll = new Array(286 + 30);
                for (i = 0; i < ll.length; i++)
                    ll[i] = 0;
                this.needBits(5);
                var nl = 257 + this.getBits(5);
                this.dumpBits(5);
                this.needBits(5);
                var nd = 1 + this.getBits(5);
                this.dumpBits(5);
                this.needBits(4);
                var nb = 4 + this.getBits(4);
                this.dumpBits(4);
                if (nl > 286 || nd > 30)
                    return -1;
                for (j = 0; j < nb; j++) {
                    this.needBits(3);
                    ll[Constants.border[j]] = this.getBits(3);
                    this.dumpBits(3);
                }
                for (; j < 19; j++)
                    ll[Constants.border[j]] = 0;
                this._bl = 7;
                var h = new HuftBuild(ll, 19, 19, null, null, this._bl);
                if (h.status != 0)
                    return -1;
                this._tl = h.root;
                this._bl = h.m;
                i = 0;
                var n = nl + nd;
                var l = 0;
                var t;
                while (i < n) {
                    this.needBits(this._bl);
                    t = this._tl.list[this.getBits(this._bl)];
                    j = t.b;
                    this.dumpBits(j);
                    j = t.n;
                    if (j < 16)
                        ll[i++] = l = j;
                    else if (j == 16) {
                        this.needBits(2);
                        j = 3 + this.getBits(2);
                        this.dumpBits(2);
                        if (i + j > n)
                            return -1;
                        while (j-- > 0)
                            ll[i++] = l;
                    }
                    else if (j == 17) {
                        this.needBits(3);
                        j = 3 + this.getBits(3);
                        this.dumpBits(3);
                        if (i + j > n)
                            return -1;
                        while (j-- > 0)
                            ll[i++] = 0;
                        l = 0;
                    }
                    else {
                        this.needBits(7);
                        j = 11 + this.getBits(7);
                        this.dumpBits(7);
                        if (i + j > n)
                            return -1;
                        while (j-- > 0)
                            ll[i++] = 0;
                        l = 0;
                    }
                }
                this._bl = Constants.lBits;
                h = new HuftBuild(ll, nl, 257, Constants.cplens, Constants.cplext, this._bl);
                if (this._bl == 0)
                    h.status = 1;
                if (h.status != 0) {
                    return -1;
                }
                this._tl = h.root;
                this._bl = h.m;
                for (i = 0; i < nd; i++)
                    ll[i] = ll[i + nl];
                this._bd = Constants.dBits;
                h = new HuftBuild(ll, nd, 0, Constants.cpdist, Constants.cpdext, this._bd);
                this._td = h.root;
                this._bd = h.m;
                if (this._bd == 0 && nl > 257) {
                    return -1;
                }
                if (h.status != 0)
                    return -1;
                return this.inflateCodes(buffer, offset, size);
            };
            Helper.prototype.inflateStart = function () {
                if (this._slide == null)
                    this._slide = new Array(2 * Constants.wSize);
                this._wp = 0;
                this._bitBuffer = 0;
                this._bitLength = 0;
                this._method = -1;
                this._eof = false;
                this._copyLength = 0;
                this._copyDist = 0;
                this._tl = null;
            };
            Helper.prototype.inflateInternal = function (buffer, offset, size) {
                var n = 0;
                while (n < size) {
                    if (this._eof && this._method == -1)
                        return n;
                    if (this._copyLength > 0) {
                        if (this._method != Constants.storedBlock) {
                            while (this._copyLength > 0 && n < size) {
                                this._copyLength--;
                                this._copyDist &= Constants.wSize - 1;
                                this._wp &= Constants.wSize - 1;
                                buffer[offset + n++] = this._slide[this._wp++] =
                                    this._slide[this._copyDist++];
                            }
                        }
                        else {
                            while (this._copyLength > 0 && n < size) {
                                this._copyLength--;
                                this._wp &= Constants.wSize - 1;
                                this.needBits(8);
                                buffer[offset + n++] = this._slide[this._wp++] = this.getBits(8);
                                this.dumpBits(8);
                            }
                            if (this._copyLength == 0)
                                this._method = -1;
                        }
                        if (n == size)
                            return n;
                    }
                    if (this._method == -1) {
                        if (this._eof)
                            break;
                        this.needBits(1);
                        if (this.getBits(1) != 0)
                            this._eof = true;
                        this.dumpBits(1);
                        this.needBits(2);
                        this._method = this.getBits(2);
                        this.dumpBits(2);
                        this._tl = null;
                        this._copyLength = 0;
                    }
                    var i = void 0;
                    switch (this._method) {
                        case 0:
                            i = this.inflateStored(buffer, offset + n, size - n);
                            break;
                        case 1:
                            if (this._tl != null)
                                i = this.inflateCodes(buffer, offset + n, size - n);
                            else
                                i = this.inflateFixed(buffer, offset + n, size - n);
                            break;
                        case 2:
                            if (this._tl != null)
                                i = this.inflateCodes(buffer, offset + n, size - n);
                            else
                                i = this.inflateDynamic(buffer, offset + n, size - n);
                            break;
                        default:
                            i = -1;
                            break;
                    }
                    if (i == -1) {
                        if (this._eof)
                            return 0;
                        return -1;
                    }
                    n += i;
                }
                return n;
            };
            Helper.prototype.inflate = function (data) {
                this._inflateData = data;
                this._inflatePos = 0;
                this.inflateStart();
                var i;
                var j;
                var buffer = new Array(1024);
                var aout = [];
                while ((i = this.inflateInternal(buffer, 0, buffer.length)) > 0) {
                    for (j = 0; j < i; j++)
                        aout[aout.length] = buffer[j];
                }
                this._inflateData = null;
                return aout;
            };
            return Helper;
        }());
        Compression.Helper = Helper;
    })(Compression || (Compression = {}));

    var Flags;
    (function (Flags) {
        Flags[Flags["FText"] = 1] = "FText";
        Flags[Flags["FHcrc"] = 2] = "FHcrc";
        Flags[Flags["FExtra"] = 4] = "FExtra";
        Flags[Flags["FName"] = 8] = "FName";
        Flags[Flags["FComment"] = 16] = "FComment";
    })(Flags || (Flags = {}));

    Object.defineProperty(StiGZipHelper, "crcTable", {
        get: function () {
            if (this._crcTable == null) {
                this._crcTable = new Array(256);
                var value = void 0;
                for (var i = 0; i < 256; i++) {
                    value = i;
                    for (var j = 0; j < 8; j++) {
                        value = ((value & 1) != 0) ? (0xedb88320 ^ (value >>> 1)) : (value >>> 1);
                    }
                    this._crcTable[i] = value;
                }
            }
            return this._crcTable;
        },
        enumerable: true,
        configurable: true
    });
    StiGZipHelper.crc32 = function (data) {
        var crc = 0 ^ (-1);
        for (var i = 0; i < data.length; i++) {
            crc = (crc >>> 8) ^ this.crcTable[(crc ^ data[i]) & 0xff];
        }
        return (crc ^ (-1)) >>> 0;
    };
    StiGZipHelper.putByte = function (n, arr) {
        arr.push(n & 0xff);
    };
    StiGZipHelper.putShort = function (n, arr) {
        arr.push(n & 0xff);
        arr.push(n >>> 8);
    };
    StiGZipHelper.putLong = function (n, arr) {
        StiGZipHelper.putShort(n & 0xffff, arr);
        StiGZipHelper.putShort(n >>> 16, arr);
    };
    StiGZipHelper.putString = function (s, arr) {
        for (var i = 0; i < s.length; i += 1) {
            StiGZipHelper.putByte(s.charCodeAt(i), arr);
        }
    };
    StiGZipHelper.readByte = function (arr) {
        return arr.shift();
    };
    StiGZipHelper.readShort = function (arr) {
        return arr.shift() | (arr.shift() << 8);
    };
    StiGZipHelper.readLong = function (arr) {
        var n1 = StiGZipHelper.readShort(arr);
        var n2 = StiGZipHelper.readShort(arr);
        if (n2 > 32768) {
            n2 -= 32768;
            return ((n2 << 16) | n1) + 32768 * Math.pow(2, 16);
        }
        return (n2 << 16) | n1;
    };
    StiGZipHelper.readString = function (arr) {
        var charArr = [];
        while (arr[0] !== 0) {
            charArr.push(String.fromCharCode(arr.shift()));
        }
        arr.shift();
        return charArr.join("");
    };
    StiGZipHelper.readBytes = function (arr, n) {
        var ret = [];
        for (var i = 0; i < n; i += 1) {
            ret.push(arr.shift());
        }
        return ret;
    };
    StiGZipHelper.pack = function (data, name) {
        if (data == null || data === undefined) return null;
        var flags = 0;
        var out = [];
        var buffer = data;
        if (typeof data == 'string') buffer = Array.prototype.map.call(unescape(encodeURIComponent(data)), function (char) { return char.charCodeAt(0) });
        StiGZipHelper.putByte(StiGZipHelper.ID1, out);
        StiGZipHelper.putByte(StiGZipHelper.ID2, out);
        StiGZipHelper.putByte(StiGZipHelper.DefaultMethod, out);
        if (typeof name != 'undefined' && name != null && name != '') flags |= Flags.FName;
        StiGZipHelper.putByte(flags, out);
        StiGZipHelper.putLong(parseInt((Date.now() / 1000).toString(), 10), out);
        if (StiGZipHelper.DefaultLevel == 1) StiGZipHelper.putByte(4, out);
        else if (StiGZipHelper.DefaultLevel == 9) StiGZipHelper.putByte(2, out);
        else StiGZipHelper.putByte(0, out);
        if (navigator.appVersion.indexOf("Win") != -1) StiGZipHelper.putByte(11, out);
        else StiGZipHelper.putByte(3, out);
        if (typeof name != 'undefined' && name != null && name != '') {
            StiGZipHelper.putString(name.substring(name.lastIndexOf('/') + 1), out);
            StiGZipHelper.putByte(0, out);
        }
        Compression.Helper.deflate(buffer, StiGZipHelper.DefaultLevel).forEach(function (byte) {
            StiGZipHelper.putByte(byte, out);
        });
        StiGZipHelper.putLong(StiGZipHelper.crc32(buffer), out);
        StiGZipHelper.putLong(buffer.length, out);

        var input = new Uint8Array(out);
        data = "";
        for (var index = 0; index < input.byteLength; index++) {
            data += String.fromCharCode(input[index]);
        }

        return window.btoa(data);
    };
    StiGZipHelper.unpack = function (data) {
        var buffer = data;
        if (typeof data == 'string') {
            var output = (window && window.atob) ? window.atob(data.split("\r\n").join("\n").split("\n").join("")) : StiBase64.decode(data);
            buffer = Array.prototype.map.call(output, function (char) { return char.charCodeAt(0) });
        }
        if (StiGZipHelper.readByte(buffer) !== StiGZipHelper.ID1 || StiGZipHelper.readByte(buffer) !== StiGZipHelper.ID2) alert("The stream is not a GZip file");
        else if (StiGZipHelper.readByte(buffer) !== 8) alert("Unsupported GZip compression method");
        var flags = StiGZipHelper.readByte(buffer);
        var mtime = StiGZipHelper.readLong(buffer);
        var xflags = StiGZipHelper.readByte(buffer);
        var os = StiGZipHelper.readByte(buffer);
        if (flags & Flags.FExtra) {
            var count = StiGZipHelper.readShort(buffer);
            StiGZipHelper.readBytes(buffer, count);
        }
        if (flags & Flags.FName) StiGZipHelper.readString(buffer);
        if (flags & Flags.FComment) StiGZipHelper.readString(buffer);
        if (flags & Flags.FHcrc) StiGZipHelper.readShort(buffer);
        var out = Compression.Helper.inflate(buffer.splice(0, buffer.length - 8));
        if (StiGZipHelper.readLong(buffer) !== StiGZipHelper.crc32(out)) alert("GZip checksum does not match");
        if (StiGZipHelper.readLong(buffer) !== out.length) alert("Size of GZip decompressed file not correct");
        if (Array.isArray(data)) return out;

        var result = '';
        for (var _i = 0, out_1 = out; _i < out_1.length; _i++) {
            var byte = out_1[_i];
            result += String.fromCharCode(byte);
        }
        return decodeURIComponent(escape(result));
    };
    StiGZipHelper.DefaultLevel = 6;
    StiGZipHelper.DefaultMethod = 8;
    StiGZipHelper.ID1 = 0x1F;
    StiGZipHelper.ID2 = 0x8B;
    StiGZipHelper._crcTable = null;
    return StiGZipHelper;
}());