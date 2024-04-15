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
using System.Xml;
using System.Collections.Generic;

namespace Stimulsoft.Report.Web
{
    internal class HtmlParser
    {
        private HtmlParser(string inputString)
        {
            // Create an output xml document
            _document = new XmlDocument();

            // initialize open tag stack
            _openedElements = new Stack<XmlElement>();

            _pendingInlineElements = new Stack<XmlElement>();

            // initialize lexical analyzer
            _htmlLexicalAnalyzer = new HtmlLexicalAnalyzer(inputString);

            // get first token from input, expecting text
            _htmlLexicalAnalyzer.GetNextContentToken();
        }
        
        #region Internal Methods

        internal static XmlElement ParseHtml(string htmlString)
        {
            HtmlParser htmlParser = new HtmlParser(htmlString);

            XmlElement htmlRootElement = htmlParser.ParseHtmlContent();

            return htmlRootElement;
        }

        internal const string HtmlHeader = "Version:1.0\r\nStartHTML:{0:D10}\r\nEndHTML:{1:D10}\r\nStartFragment:{2:D10}\r\nEndFragment:{3:D10}\r\nStartSelection:{4:D10}\r\nEndSelection:{5:D10}\r\n";
        internal const string HtmlStartFragmentComment = "<!--StartFragment-->";
        internal const string HtmlEndFragmentComment = "<!--EndFragment-->";

        internal static string ExtractHtmlFromClipboardData(string htmlDataString)
        {
            int startHtmlIndex = htmlDataString.IndexOf("StartHTML:");
            if (startHtmlIndex < 0)
            {
                return "ERROR: Urecognized html header";
            }
            startHtmlIndex = Int32.Parse(htmlDataString.Substring(startHtmlIndex + "StartHTML:".Length, "0123456789".Length));
            if (startHtmlIndex < 0 || startHtmlIndex > htmlDataString.Length)
            {
                return "ERROR: Urecognized html header";
            }

            int endHtmlIndex = htmlDataString.IndexOf("EndHTML:");
            if (endHtmlIndex < 0)
            {
                return "ERROR: Urecognized html header";
            }
            endHtmlIndex = Int32.Parse(htmlDataString.Substring(endHtmlIndex + "EndHTML:".Length, "0123456789".Length));
            if (endHtmlIndex > htmlDataString.Length)
            {
                endHtmlIndex = htmlDataString.Length;
            }

            return htmlDataString.Substring(startHtmlIndex, endHtmlIndex - startHtmlIndex);
        }
        
        #endregion
        
        #region Private Methods

        private void InvariantAssert(bool condition, string message)
        {
            if (!condition)
            {
                throw new Exception("Assertion error: " + message);
            }
        }

        private XmlElement ParseHtmlContent()
        {
            XmlElement htmlRootElement = _document.CreateElement("html", XhtmlNamespace);
            OpenStructuringElement(htmlRootElement);

            while (_htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.EOF)
            {
                if (_htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.OpeningTagStart)
                {
                    _htmlLexicalAnalyzer.GetNextTagToken();
                    if (_htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Name)
                    {
                        string htmlElementName = _htmlLexicalAnalyzer.NextToken.ToLower();
                        _htmlLexicalAnalyzer.GetNextTagToken();

                        // Create an element
                        XmlElement htmlElement = _document.CreateElement(htmlElementName, XhtmlNamespace);

                        // Parse element attributes
                        ParseAttributes(htmlElement);

                        if (_htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.EmptyTagEnd || HtmlSchema.IsEmptyElement(htmlElementName))
                        {
                            // It is an element without content (because of explicit slash or based on implicit knowledge aboout html)
                            AddEmptyElement(htmlElement);
                        }
                        else if (HtmlSchema.IsInlineElement(htmlElementName))
                        {
                            // Elements known as formatting are pushed to some special
                            // pending stack, which allows them to be transferred
                            // over block tags - by doing this we convert
                            // overlapping tags into normal heirarchical element structure.
                            OpenInlineElement(htmlElement);
                        }
                        else if (HtmlSchema.IsBlockElement(htmlElementName) || HtmlSchema.IsKnownOpenableElement(htmlElementName))
                        {
                            // This includes no-scope elements
                            OpenStructuringElement(htmlElement);
                        }
                    }
                }
                else if (_htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.ClosingTagStart)
                {
                    _htmlLexicalAnalyzer.GetNextTagToken();
                    if (_htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Name)
                    {
                        string htmlElementName = _htmlLexicalAnalyzer.NextToken.ToLower();

                        _htmlLexicalAnalyzer.GetNextTagToken();

                        CloseElement(htmlElementName);
                    }
                }
                else if (_htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Text)
                {
                    AddTextContent(_htmlLexicalAnalyzer.NextToken);
                }
                else if (_htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Comment)
                {
                    AddComment(_htmlLexicalAnalyzer.NextToken);
                }

                _htmlLexicalAnalyzer.GetNextContentToken();
            }

            if (htmlRootElement.FirstChild is XmlElement &&
                htmlRootElement.FirstChild == htmlRootElement.LastChild &&
                htmlRootElement.FirstChild.LocalName.ToLower() == "html")
            {
                htmlRootElement = (XmlElement)htmlRootElement.FirstChild;
            }

            return htmlRootElement;
        }

        private XmlElement CreateElementCopy(XmlElement htmlElement)
        {
            XmlElement htmlElementCopy = _document.CreateElement(htmlElement.LocalName, XhtmlNamespace);
            for (int i = 0; i < htmlElement.Attributes.Count; i++)
            {
                XmlAttribute attribute = htmlElement.Attributes[i];
                htmlElementCopy.SetAttribute(attribute.Name, attribute.Value);
            }
            return htmlElementCopy;
        }

        private void AddEmptyElement(XmlElement htmlEmptyElement)
        {
            InvariantAssert(_openedElements.Count > 0, "AddEmptyElement: Stack of opened elements cannot be empty, as we have at least one artificial root element");
            XmlElement htmlParent = _openedElements.Peek();
            htmlParent.AppendChild(htmlEmptyElement);
        }

        private void OpenInlineElement(XmlElement htmlInlineElement)
        {
            _pendingInlineElements.Push(htmlInlineElement);
        }

        private void OpenStructuringElement(XmlElement htmlElement)
        {
            if (HtmlSchema.IsBlockElement(htmlElement.LocalName))
            {
                while (_openedElements.Count > 0 && HtmlSchema.IsInlineElement(_openedElements.Peek().LocalName))
                {
                    XmlElement htmlInlineElement = _openedElements.Pop();
                    InvariantAssert(_openedElements.Count > 0, "OpenStructuringElement: stack of opened elements cannot become empty here");

                    _pendingInlineElements.Push(CreateElementCopy(htmlInlineElement));
                }
            }

            // Add this block element to its parent
            if (_openedElements.Count > 0)
            {
                XmlElement htmlParent = _openedElements.Peek();

                // Check some known block elements for auto-closing (LI and P)
                if (HtmlSchema.ClosesOnNextElementStart(htmlParent.LocalName, htmlElement.LocalName))
                {
                    _openedElements.Pop();
                    htmlParent = _openedElements.Count > 0 ? _openedElements.Peek() : null;
                }

                if (htmlParent != null)
                {
                    htmlParent.AppendChild(htmlElement);
                }
            }

            _openedElements.Push(htmlElement);
        }

        private bool IsElementOpened(string htmlElementName)
        {
            foreach (XmlElement openedElement in _openedElements)
            {
                if (openedElement.LocalName == htmlElementName)
                {
                    return true;
                }
            }
            return false;
        }

        private void CloseElement(string htmlElementName)
        {
            // Check if the element is opened and already added to the parent
            InvariantAssert(_openedElements.Count > 0, "CloseElement: Stack of opened elements cannot be empty, as we have at least one artificial root element");

            // Check if the element is opened and still waiting to be added to the parent
            if (_pendingInlineElements.Count > 0 && _pendingInlineElements.Peek().LocalName == htmlElementName)
            {
                // Closing an empty inline element.
                // Note that HtmlConverter will skip empty inlines, but for completeness we keep them here on parser level.
                XmlElement htmlInlineElement = _pendingInlineElements.Pop();
                InvariantAssert(_openedElements.Count > 0, "CloseElement: Stack of opened elements cannot be empty, as we have at least one artificial root element");
                XmlElement htmlParent = _openedElements.Peek();
                htmlParent.AppendChild(htmlInlineElement);
                return;
            }
            else if (IsElementOpened(htmlElementName))
            {
                while (_openedElements.Count > 1) // we never pop the last element - the artificial root
                {
                    // Close all unbalanced elements.
                    XmlElement htmlOpenedElement = _openedElements.Pop();

                    if (htmlOpenedElement.LocalName == htmlElementName)
                    {
                        return;
                    }

                    if (HtmlSchema.IsInlineElement(htmlOpenedElement.LocalName))
                    {
                        // Unbalances Inlines will be transfered to the next element content
                        _pendingInlineElements.Push(CreateElementCopy(htmlOpenedElement));
                    }
                }
            }
        }

        private void AddTextContent(string textContent)
        {
            OpenPendingInlineElements();

            InvariantAssert(_openedElements.Count > 0, "AddTextContent: Stack of opened elements cannot be empty, as we have at least one artificial root element");

            XmlElement htmlParent = _openedElements.Peek();
            XmlText textNode = _document.CreateTextNode(textContent);
            htmlParent.AppendChild(textNode);
        }

        private void AddComment(string comment)
        {
            OpenPendingInlineElements();

            InvariantAssert(_openedElements.Count > 0, "AddComment: Stack of opened elements cannot be empty, as we have at least one artificial root element");

            XmlElement htmlParent = _openedElements.Peek();
            XmlComment xmlComment = _document.CreateComment(comment);
            htmlParent.AppendChild(xmlComment);
        }

        private void OpenPendingInlineElements()
        {
            if (_pendingInlineElements.Count > 0)
            {
                XmlElement htmlInlineElement = _pendingInlineElements.Pop();

                OpenPendingInlineElements();

                InvariantAssert(_openedElements.Count > 0, "OpenPendingInlineElements: Stack of opened elements cannot be empty, as we have at least one artificial root element");

                XmlElement htmlParent = _openedElements.Peek();
                htmlParent.AppendChild(htmlInlineElement);
                _openedElements.Push(htmlInlineElement);
            }
        }

        private void ParseAttributes(XmlElement xmlElement)
        {
            while (_htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.EOF && //
                _htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.TagEnd && //
                _htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.EmptyTagEnd)
            {
                // read next attribute (name=value)
                if (_htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Name)
                {
                    string attributeName = _htmlLexicalAnalyzer.NextToken;
                    _htmlLexicalAnalyzer.GetNextEqualSignToken();

                    _htmlLexicalAnalyzer.GetNextAtomToken();

                    string attributeValue = _htmlLexicalAnalyzer.NextToken;
                    xmlElement.SetAttribute(attributeName, attributeValue);
                }
                _htmlLexicalAnalyzer.GetNextTagToken();
            }
        }

        #endregion

        #region Fields

        internal const string XhtmlNamespace = "http://www.w3.org/1999/xhtml";

        private HtmlLexicalAnalyzer _htmlLexicalAnalyzer;

        private XmlDocument _document;

        Stack<XmlElement> _openedElements;
        Stack<XmlElement> _pendingInlineElements;
        #endregion
    }
}
