﻿/*
 * [The "BSD license"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Pixel Mine, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *    derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;

namespace Stimulsoft.Data.Expressions.Antlr.Runtime.Misc
{
    public class ListStack<T> : List<T>
    {
        public T Peek()
        {
            return Peek(0);
        }

        public T Peek(int depth)
        {
            T item;
            if (!TryPeek(depth, out item))
                throw new InvalidOperationException();

            return item;
        }

        public bool TryPeek(out T item)
        {
            return TryPeek(0, out item);
        }

        public bool TryPeek(int depth, out T item)
        {
            if (depth >= Count)
            {
                item = default(T);
                return false;
            }

            item = this[Count - depth - 1];
            return true;
        }

        public T Pop()
        {
            T result;
            if (!TryPop(out result))
                throw new InvalidOperationException();

            return result;
        }

        public bool TryPop(out T item)
        {
            if (Count == 0)
            {
                item = default(T);
                return false;
            }

            item = this[Count - 1];
            RemoveAt(Count - 1);
            return true;
        }

        public void Push(T item)
        {
            Add(item);
        }
    }
}
