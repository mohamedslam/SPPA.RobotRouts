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

namespace Stimulsoft.Data.Expressions.Antlr.Runtime.Tree
{
    /** <summary>
     *  This is identical to the ParserRuleReturnScope except that
     *  the start property is a tree nodes not Token object
     *  when you are parsing trees.
     *  </summary>
     */
    [Serializable]
    public class TreeRuleReturnScope<TTree> : IRuleReturnScope<TTree>
    {
        private TTree _start;

        /** <summary>Gets the first node or root node of tree matched for this rule.</summary> */
        public TTree Start
        {
            get
            {
                return _start;
            }

            set
            {
                _start = value;
            }
        }

        object IRuleReturnScope.Start
        {
            get
            {
                return Start;
            }
        }

        TTree IRuleReturnScope<TTree>.Stop
        {
            get
            {
                return default(TTree);
            }
        }

        object IRuleReturnScope.Stop
        {
            get
            {
                return default(TTree);
            }
        }
    }
}
