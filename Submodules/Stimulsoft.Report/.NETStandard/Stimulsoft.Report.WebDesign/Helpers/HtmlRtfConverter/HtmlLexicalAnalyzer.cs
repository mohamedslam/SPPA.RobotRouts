#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
using System.IO;
using System.Diagnostics;
using System.Text;

namespace Stimulsoft.Report.Web
{
    internal enum HtmlTokenType
    {
        OpeningTagStart,
        ClosingTagStart,
        TagEnd,
        EmptyTagEnd,
        EqualSign,
        Name,
        Atom, 
        Text, 
        Comment,
        EOF,
    }

    internal class HtmlLexicalAnalyzer
    {
        internal HtmlLexicalAnalyzer(string inputTextString)
        {
            _inputStringReader = new StringReader(inputTextString);
            _nextCharacterCode = 0;
            _nextCharacter = ' ';
            _lookAheadCharacterCode = _inputStringReader.Read();
            _lookAheadCharacter = (char)_lookAheadCharacterCode;
            _previousCharacter = ' ';
            _ignoreNextWhitespace = true;
            _nextToken = new StringBuilder(100);
            _nextTokenType = HtmlTokenType.Text;
            this.GetNextCharacter();
        }
        
        #region Internal Methods

        internal void GetNextContentToken()
        {
            Debug.Assert(_nextTokenType != HtmlTokenType.EOF);
            _nextToken.Length = 0;
            if (this.IsAtEndOfStream)
            {
                _nextTokenType = HtmlTokenType.EOF;
                return;
            }

            if (this.IsAtTagStart)
            {
                this.GetNextCharacter();

                if (this.NextCharacter == '/')
                {
                    _nextToken.Append("</");
                    _nextTokenType = HtmlTokenType.ClosingTagStart;

                    // advance
                    this.GetNextCharacter();
                    _ignoreNextWhitespace = false;
                }
                else
                {
                    _nextTokenType = HtmlTokenType.OpeningTagStart;
                    _nextToken.Append("<");
                    _ignoreNextWhitespace = true;
                }
            }
            else if (this.IsAtDirectiveStart)
            {
                // either a comment or CDATA
                this.GetNextCharacter();
                if (_lookAheadCharacter == '[')
                {
                    // cdata
                    this.ReadDynamicContent();
                }
                else if (_lookAheadCharacter == '-')
                {
                    this.ReadComment();
                }
                else
                {
                    this.ReadUnknownDirective();
                }
            }
            else
            {
                // read text content, unless you encounter a tag
                _nextTokenType = HtmlTokenType.Text;
                while (!this.IsAtTagStart && !this.IsAtEndOfStream && !this.IsAtDirectiveStart)
                {
                    if (this.NextCharacter == '<' && !this.IsNextCharacterEntity && _lookAheadCharacter == '?')
                    {
                        // ignore processing directive
                        this.SkipProcessingDirective();
                    }
                    else
                    {
                        if (this.NextCharacter <= ' ')
                        {
                            //  Respect xml:preserve or its equivalents for whitespace processing
                            if (_ignoreNextWhitespace)
                            {
                                // Ignore repeated whitespaces
                            }
                            else
                            {
                                // Treat any control character sequence as one whitespace
                                _nextToken.Append(' ');
                            }
                            _ignoreNextWhitespace = true; // and keep ignoring the following whitespaces
                        }
                        else
                        {
                            _nextToken.Append(this.NextCharacter);
                            _ignoreNextWhitespace = false;
                        }
                        this.GetNextCharacter();
                    }
                }
            }
        }

        internal void GetNextTagToken()
        {
            _nextToken.Length = 0;
            if (this.IsAtEndOfStream)
            {
                _nextTokenType = HtmlTokenType.EOF;
                return;
            }

            this.SkipWhiteSpace();

            if (this.NextCharacter == '>' && !this.IsNextCharacterEntity)
            {
                // &gt; should not end a tag, so make sure it's not an entity
                _nextTokenType = HtmlTokenType.TagEnd;
                _nextToken.Append('>');
                this.GetNextCharacter();
                // Note: _ignoreNextWhitespace must be set appropriately on tag start processing
            }
            else if (this.NextCharacter == '/' && _lookAheadCharacter == '>')
            {
                // could be start of closing of empty tag
                _nextTokenType = HtmlTokenType.EmptyTagEnd;
                _nextToken.Append("/>");
                this.GetNextCharacter();
                this.GetNextCharacter();
                _ignoreNextWhitespace = false; // Whitespace after no-scope tags are sifnificant
            }
            else if (IsGoodForNameStart(this.NextCharacter))
            {
                _nextTokenType = HtmlTokenType.Name;

                while (IsGoodForName(this.NextCharacter) && !this.IsAtEndOfStream)
                {
                    _nextToken.Append(this.NextCharacter);
                    this.GetNextCharacter();
                }
            }
            else
            {
                // Unexpected type of token for a tag. Reprot one character as Atom, expecting that HtmlParser will ignore it.
                _nextTokenType = HtmlTokenType.Atom;
                _nextToken.Append(this.NextCharacter);
                this.GetNextCharacter();
            }
        }

        internal void GetNextEqualSignToken()
        {
            Debug.Assert(_nextTokenType != HtmlTokenType.EOF);
            _nextToken.Length = 0;

            _nextToken.Append('=');
            _nextTokenType = HtmlTokenType.EqualSign;

            this.SkipWhiteSpace();

            if (this.NextCharacter == '=')
            {
                this.GetNextCharacter();
            }
        }

        internal void GetNextAtomToken()
        {
            Debug.Assert(_nextTokenType != HtmlTokenType.EOF);
            _nextToken.Length = 0;

            this.SkipWhiteSpace();

            _nextTokenType = HtmlTokenType.Atom;

            if ((this.NextCharacter == '\'' || this.NextCharacter == '"') && !this.IsNextCharacterEntity)
            {
                char startingQuote = this.NextCharacter;
                this.GetNextCharacter();

                // Consume all characters between quotes
                while (!(this.NextCharacter == startingQuote && !this.IsNextCharacterEntity) && !this.IsAtEndOfStream)
                {
                    _nextToken.Append(this.NextCharacter);
                    this.GetNextCharacter();
                }
                if (this.NextCharacter == startingQuote)
                {
                    this.GetNextCharacter();
                }
            }
            else
            {
                while (!this.IsAtEndOfStream && !Char.IsWhiteSpace(this.NextCharacter) && this.NextCharacter != '>')
                {
                    _nextToken.Append(this.NextCharacter);
                    this.GetNextCharacter();
                }
            }
        }

        #endregion Internal Methods


        #region Internal Properties

        internal HtmlTokenType NextTokenType
        {
            get
            {
                return _nextTokenType;
            }
        }

        internal string NextToken
        {
            get
            {
                return _nextToken.ToString();
            }
        }

        #endregion


        #region Private Methods

        private void GetNextCharacter()
        {
            if (_nextCharacterCode == -1)
            {
                throw new InvalidOperationException("GetNextCharacter method called at the end of a stream");
            }

            _previousCharacter = _nextCharacter;

            _nextCharacter = _lookAheadCharacter;
            _nextCharacterCode = _lookAheadCharacterCode;
            _isNextCharacterEntity = false;

            this.ReadLookAheadCharacter();

            if (_nextCharacter == '&')
            {
                if (_lookAheadCharacter == '#')
                {
                    // numeric entity - parse digits - &#DDDDD;
                    int entityCode;
                    entityCode = 0;
                    this.ReadLookAheadCharacter();

                    // largest numeric entity is 7 characters
                    for (int i = 0; i < 7 && Char.IsDigit(_lookAheadCharacter); i++)
                    {
                        entityCode = 10 * entityCode + (_lookAheadCharacterCode - (int)'0');
                        this.ReadLookAheadCharacter();
                    }
                    if (_lookAheadCharacter == ';')
                    {
                        // correct format - advance
                        this.ReadLookAheadCharacter();
                        _nextCharacterCode = entityCode;

                        // if this is out of range it will set the character to '?'
                        _nextCharacter = (char)_nextCharacterCode;

                        // as far as we are concerned, this is an entity
                        _isNextCharacterEntity = true;
                    }
                    else
                    {
                        // not an entity, set next character to the current lookahread character
                        // we would have eaten up some digits
                        _nextCharacter = _lookAheadCharacter;
                        _nextCharacterCode = _lookAheadCharacterCode;
                        this.ReadLookAheadCharacter();
                        _isNextCharacterEntity = false;
                    }
                }
                else if (Char.IsLetter(_lookAheadCharacter))
                {
                    // entity is written as a string
                    string entity = "";

                    // maximum length of string entities is 10 characters
                    for (int i = 0; i < 10 && (Char.IsLetter(_lookAheadCharacter) || Char.IsDigit(_lookAheadCharacter)); i++)
                    {
                        entity += _lookAheadCharacter;
                        this.ReadLookAheadCharacter();
                    }
                    if (_lookAheadCharacter == ';')
                    {
                        // advance
                        this.ReadLookAheadCharacter();

                        if (HtmlSchema.IsEntity(entity))
                        {
                            _nextCharacter = HtmlSchema.EntityCharacterValue(entity);
                            _nextCharacterCode = (int)_nextCharacter;
                            _isNextCharacterEntity = true;
                        }
                        else
                        {
                            // just skip the whole thing - invalid entity
                            // move on to the next character
                            _nextCharacter = _lookAheadCharacter;
                            _nextCharacterCode = _lookAheadCharacterCode;
                            this.ReadLookAheadCharacter();

                            // not an entity
                            _isNextCharacterEntity = false;
                        }
                    }
                    else
                    {
                        // skip whatever we read after the ampersand
                        // set next character and move on
                        _nextCharacter = _lookAheadCharacter;
                        this.ReadLookAheadCharacter();
                        _isNextCharacterEntity = false;
                    }
                }
            }
        }

        private void ReadLookAheadCharacter()
        {
            if (_lookAheadCharacterCode != -1)
            {
                _lookAheadCharacterCode = _inputStringReader.Read();
                _lookAheadCharacter = (char)_lookAheadCharacterCode;
            }
        }

        /// <summary>
        /// skips whitespace in the input string
        /// leaves the first non-whitespace character available in the NextCharacter property
        /// this may be the end-of-file character, it performs no checking 
        /// </summary>
        private void SkipWhiteSpace()
        {
            while (true)
            {
                if (_nextCharacter == '<' && (_lookAheadCharacter == '?' || _lookAheadCharacter == '!'))
                {
                    this.GetNextCharacter();

                    if (_lookAheadCharacter == '[')
                    {
                        // Skip CDATA block and DTDs(?)
                        while (!this.IsAtEndOfStream && !(_previousCharacter == ']' && _nextCharacter == ']' && _lookAheadCharacter == '>'))
                        {
                            this.GetNextCharacter();
                        }
                        if (_nextCharacter == '>')
                        {
                            this.GetNextCharacter();
                        }
                    }
                    else
                    {
                        // Skip processing instruction, comments
                        while (!this.IsAtEndOfStream && _nextCharacter != '>')
                        {
                            this.GetNextCharacter();
                        }
                        if (_nextCharacter == '>')
                        {
                            this.GetNextCharacter();
                        }
                    }
                }


                if (!Char.IsWhiteSpace(this.NextCharacter))
                {
                    break;
                }

                this.GetNextCharacter();
            }
        }

        private bool IsGoodForNameStart(char character)
        {
            return character == '_' || Char.IsLetter(character);
        }

        private bool IsGoodForName(char character)
        {
            // we are not concerned with escaped characters in names
            // we assume that character entities are allowed as part of a name
            return
                this.IsGoodForNameStart(character) ||
                character == '.' ||
                character == '-' ||
                character == ':' ||
                Char.IsDigit(character);
        }
        
        private void ReadDynamicContent()
        {
            Debug.Assert(_previousCharacter == '<' && _nextCharacter == '!' && _lookAheadCharacter == '[');

            _nextTokenType = HtmlTokenType.Text;
            _nextToken.Length = 0;

            this.GetNextCharacter();
            this.GetNextCharacter();

            while (!(_nextCharacter == ']' && _lookAheadCharacter == '>') && !this.IsAtEndOfStream)
            {
                // advance
                this.GetNextCharacter();
            }

            if (!this.IsAtEndOfStream)
            {
                // advance, first to the last >
                this.GetNextCharacter();

                // then advance past it to the next character after processing directive
                this.GetNextCharacter();
            }
        }

        private void ReadComment()
        {
            // verify that we are at a comment
            Debug.Assert(_previousCharacter == '<' && _nextCharacter == '!' && _lookAheadCharacter == '-');

            // Initialize a token
            _nextTokenType = HtmlTokenType.Comment;
            _nextToken.Length = 0;

            this.GetNextCharacter(); 
            this.GetNextCharacter();
            this.GetNextCharacter();

            while (true)
            {
                while (!this.IsAtEndOfStream && !(_nextCharacter == '-' && _lookAheadCharacter == '-' || _nextCharacter == '!' && _lookAheadCharacter == '>'))
                {
                    _nextToken.Append(this.NextCharacter);
                    this.GetNextCharacter();
                }

                this.GetNextCharacter();
                if (_previousCharacter == '-' && _nextCharacter == '-' && _lookAheadCharacter == '>')
                {
                    this.GetNextCharacter();
                    break;
                }
                else if (_previousCharacter == '!' && _nextCharacter == '>')
                {
                    break;
                }
                else
                {
                    _nextToken.Append(_previousCharacter);
                    continue;
                }
            }

            if (_nextCharacter == '>')
            {
                this.GetNextCharacter();
            }
        }

        private void ReadUnknownDirective()
        {
            // verify that we are at an unknown directive
            Debug.Assert(_previousCharacter == '<' && _nextCharacter == '!' && !(_lookAheadCharacter == '-' || _lookAheadCharacter == '['));

            // Let's treat this as empty text
            _nextTokenType = HtmlTokenType.Text;
            _nextToken.Length = 0;

            // advance to the next character
            this.GetNextCharacter();

            // skip to the first tag end we find
            while (!(_nextCharacter == '>' && !IsNextCharacterEntity) && !this.IsAtEndOfStream)
            {
                this.GetNextCharacter();
            }

            if (!this.IsAtEndOfStream)
            {
                // advance past the tag end
                this.GetNextCharacter();
            }
        }

        private void SkipProcessingDirective()
        {
            Debug.Assert(_nextCharacter == '<' && _lookAheadCharacter == '?');

            this.GetNextCharacter();
            this.GetNextCharacter();

            while (!((_nextCharacter == '?' || _nextCharacter == '/') && _lookAheadCharacter == '>') && !this.IsAtEndOfStream)
            {
                this.GetNextCharacter();
            }

            if (!this.IsAtEndOfStream)
            {
                this.GetNextCharacter();

                this.GetNextCharacter();
            }
        }

        #endregion


        #region Private Properties

        private char NextCharacter
        {
            get
            {
                return _nextCharacter;
            }
        }

        private bool IsAtEndOfStream
        {
            get
            {
                return _nextCharacterCode == -1;
            }
        }

        private bool IsAtTagStart
        {
            get
            {
                return _nextCharacter == '<' && (_lookAheadCharacter == '/' || IsGoodForNameStart(_lookAheadCharacter)) && !_isNextCharacterEntity;
            }
        }

        private bool IsAtDirectiveStart
        {
            get
            {
                return (_nextCharacter == '<' && _lookAheadCharacter == '!' && !this.IsNextCharacterEntity);
            }
        }

        private bool IsNextCharacterEntity
        {
            get
            {
                return _isNextCharacterEntity;
            }
        }

        #endregion


        #region Fields

        // string reader which will move over input text
        private StringReader _inputStringReader;
        // next character code read from input that is not yet part of any token
        // and the character it represents
        private int _nextCharacterCode;
        private char _nextCharacter;
        private int _lookAheadCharacterCode;
        private char _lookAheadCharacter;
        private char _previousCharacter;
        private bool _ignoreNextWhitespace;
        private bool _isNextCharacterEntity;

        // store token and type in local variables before copying them to output parameters
        StringBuilder _nextToken;
        HtmlTokenType _nextTokenType;

        #endregion
    }
}
