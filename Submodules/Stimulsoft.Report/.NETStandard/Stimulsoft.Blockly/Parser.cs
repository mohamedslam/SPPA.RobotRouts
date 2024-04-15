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

using Stimulsoft.Blockly.Model;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Stimulsoft.Blockly
{
    public class Parser
    {
        #region Fields
        private IDictionary<string, Func<IronBlock>> blocks = new Dictionary<string, Func<IronBlock>>();
        #endregion

        #region Methods
        public Parser AddBlock<T>(string type) where T : IronBlock, new()
        {
            this.AddBlock(type, () => new T());
            return this;
        }

        public Parser AddBlock<T>(string type, T block) where T : IronBlock
        {
            this.AddBlock(type, () => block);
            return this;
        }

        public Parser AddBlock(string type, Func<IronBlock> blockFactory)
        {
            if (this.blocks.ContainsKey(type))
            {
                this.blocks[type] = blockFactory;
                return this;
            }
            this.blocks.Add(type, blockFactory);
            return this;
        }

        public Workspace Parse(string xml, bool preserveWhitespace = false)
        {
            var xdoc = new XmlDocument { PreserveWhitespace = preserveWhitespace };
            xdoc.LoadXml(xml);

            var workspace = new Workspace();
            foreach (XmlNode node in xdoc.DocumentElement.ChildNodes)
            {
                if (node.LocalName == "block" || node.LocalName == "shadow")
                {
                    var block = ParseBlock(node);
                    if (null != block) workspace.Blocks.Add(block);
                }
            }

            return workspace;
        }

        public IronBlock ParseBlock(XmlNode node)
        {
            if (bool.Parse(node.GetAttribute("disabled") ?? "false")) return null;

            var type = node.GetAttribute("type");
            if (!this.blocks.ContainsKey(type)) throw new ApplicationException($"block type not registered: '{type}'");
            var block = this.blocks[type]();

            block.Type = type;
            block.Id = node.GetAttribute("id");

            foreach (XmlNode childNode in node.ChildNodes)
            {
                switch (childNode.LocalName)
                {
                    case "mutation":
                        ParseMutation(childNode, block);
                        break;
                    case "field":
                        ParseField(childNode, block);
                        break;
                    case "value":
                        ParseValue(childNode, block);
                        break;
                    case "statement":
                        ParseStatement(childNode, block);
                        break;
                    case "comment":
                        // comments are ignored
                        break;
                    case "next":
                        var nextBlock = ParseBlock(childNode.FirstChild);
                        if (null != nextBlock) block.Next = nextBlock;
                        break;
                    default:
                        throw new ArgumentException($"unknown xml type: {childNode.LocalName}");
                }
            }
            return block;
        }

        public void ParseField(XmlNode fieldNode, IronBlock block)
        {
            var field = new Field
            {
                Name = fieldNode.GetAttribute("name"),
                Value = fieldNode.InnerText
            };
            block.Fields.Add(field);
        }

        public void ParseValue(XmlNode valueNode, IronBlock block)
        {
            var childNode = valueNode.GetChild("block") ?? valueNode.GetChild("shadow");
            if (childNode == null) return;
            var childBlock = ParseBlock(childNode);

            var value = new Value
            {
                Name = valueNode.GetAttribute("name"),
                Block = childBlock
            };
            block.Values.Add(value);
        }

        public void ParseStatement(XmlNode statementNode, IronBlock block)
        {
            var childNode = statementNode.GetChild("block") ?? statementNode.GetChild("shadow");
            if (childNode == null) return;
            var childBlock = ParseBlock(childNode);

            var statement = new Statement
            {
                Name = statementNode.GetAttribute("name"),
                Block = childBlock
            };
            block.Statements.Add(statement);
        }

        public void ParseMutation(XmlNode mutationNode, IronBlock block)
        {
            foreach (XmlAttribute attribute in mutationNode.Attributes)
            {
                block.Mutations.Add(new Mutation("mutation", attribute.Name, attribute.Value));
            }

            foreach (XmlNode node in mutationNode.ChildNodes)
            {
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    block.Mutations.Add(new Mutation(node.Name, attribute.Name, attribute.Value));
                }
            }
        } 
        #endregion
    }

    internal static class ParserExtensions
    {
        public static XmlNode GetChild(this XmlNode node, string name)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.LocalName == name) return childNode;
            }
            return null;
        }

        public static string GetAttribute(this XmlNode node, string name)
        {
            foreach (XmlAttribute attribute in node.Attributes)
            {
                if (attribute.Name == name) return attribute.Value;
            }
            return null;

        }
    }
}